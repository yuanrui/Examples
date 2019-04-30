using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.SessionState;

namespace Simple.Common.Web
{
    public class LockFreeSessionStateModule : IHttpModule, IDisposable
    {
        private Hashtable pSessionItems = new Hashtable();
        private Timer pTimer;
        private int pTimerSeconds = 10;
        private bool pInitialized = false;
        private int pTimeout;
        private HttpCookieMode pCookieMode = HttpCookieMode.UseCookies;
        private ReaderWriterLockSlim pHashtableLock = new ReaderWriterLockSlim();
        private ISessionIDManager pSessionIDManager;
        private SessionStateSection pConfig;
        private bool acquireCalled;
        private bool releaseCalled;

        // The SessionItem class is used to store data for a particular session along with
        // an expiration date and time. SessionItem objects are added to the local Hashtable
        // in the OnReleaseRequestState event handler and retrieved from the local Hashtable
        // in the OnAcquireRequestState event handler. The ExpireCallback method is called
        // periodically by the local Timer to check for all expired SessionItem objects in the
        // local Hashtable and remove them.
        private class SessionItem
        {
            internal SessionStateItemCollection Items;
            internal HttpStaticObjectsCollection StaticObjects;
            internal DateTime Expires;
        }

        //
        // IHttpModule.Init
        //
        public void Init(HttpApplication app)
        {
            // Add event handlers.
            app.AcquireRequestState += new EventHandler(this.OnAcquireRequestState);
            app.ReleaseRequestState += new EventHandler(this.OnReleaseRequestState);
            app.EndRequest += new EventHandler(this.OnEndRequest);

            // Create a SessionIDManager.
            pSessionIDManager = new SessionIDManager();
            pSessionIDManager.Initialize();

            // If not already initialized, initialize timer and configuration.
            if (!pInitialized)
            {
                lock (typeof(LockFreeSessionStateModule))
                {
                    if (!pInitialized)
                    {
                        // Create a Timer to invoke the ExpireCallback method based on
                        // the pTimerSeconds value (e.g. every 10 seconds).

                        pTimer = new Timer(new TimerCallback(this.ExpireCallback),
                                           null,
                                           0,
                                           pTimerSeconds * 1000);

                        // Get the configuration section and set timeout and CookieMode values.
                        Configuration cfg =
                          WebConfigurationManager.OpenWebConfiguration(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
                        pConfig = (SessionStateSection)cfg.GetSection("system.web/sessionState");

                        pTimeout = (int)pConfig.Timeout.TotalMinutes;
                        pCookieMode = pConfig.Cookieless;

                        pInitialized = true;
                    }
                }
            }
        }

        //
        // IHttpModule.Dispose
        //
        public void Dispose()
        {
            if (pTimer != null)
            {
                this.pTimer.Dispose();
                ((IDisposable)pTimer).Dispose();
            }
        }

        //
        // Called periodically by the Timer created in the Init method to check for 
        // expired sessions and remove expired data.
        //
        void ExpireCallback(object state)
        {
            try
            {
                pHashtableLock.EnterWriteLock();
                
                this.RemoveExpiredSessionData();
            }
            finally
            {
                pHashtableLock.ExitWriteLock();
            }
        }

        //
        // Recursivly remove expired session data from session collection.
        //
        private void RemoveExpiredSessionData()
        {
            string sessionID;

            foreach (DictionaryEntry entry in pSessionItems)
            {
                SessionItem item = (SessionItem)entry.Value;

                if (DateTime.Compare(item.Expires, DateTime.Now) <= 0)
                {
                    sessionID = entry.Key.ToString();
                    pSessionItems.Remove(entry.Key);

                    HttpSessionStateContainer stateProvider =
                      new HttpSessionStateContainer(sessionID,
                                                   item.Items,
                                                   item.StaticObjects,
                                                   pTimeout,
                                                   false,
                                                   pCookieMode,
                                                   SessionStateMode.Custom,
                                                   false);

                    SessionStateUtility.RaiseSessionEnd(stateProvider, this, EventArgs.Empty);
                    this.RemoveExpiredSessionData();
                    break;
                }
            }

        }

        //
        // Event handler for HttpApplication.AcquireRequestState
        //
        private void OnAcquireRequestState(object source, EventArgs args)
        {
            acquireCalled = true;
            HttpApplication app = (HttpApplication)source;
            HttpContext context = app.Context;
            bool isNew = false;
            string sessionID;
            SessionItem sessionData = null;
            bool supportSessionIDReissue = true;

            pSessionIDManager.InitializeRequest(context, false, out supportSessionIDReissue);
            sessionID = pSessionIDManager.GetSessionID(context);


            if (sessionID != null)
            {
                try
                {
                    pHashtableLock.EnterReadLock();
                    sessionData = (SessionItem)pSessionItems[sessionID];

                    if (sessionData != null)
                        sessionData.Expires = DateTime.Now.AddMinutes(pTimeout);
                }
                finally
                {
                    pHashtableLock.ExitReadLock();
                }
            }
            else
            {
                bool redirected, cookieAdded;

                sessionID = pSessionIDManager.CreateSessionID(context);
                pSessionIDManager.SaveSessionID(context, sessionID, out redirected, out cookieAdded);

                if (redirected)
                    return;
            }

            if (sessionData == null)
            {
                // Identify the session as a new session state instance. Create a new SessionItem
                // and add it to the local Hashtable.

                isNew = true;

                sessionData = new SessionItem();

                sessionData.Items = new SessionStateItemCollection();
                sessionData.StaticObjects = SessionStateUtility.GetSessionStaticObjects(context);
                sessionData.Expires = DateTime.Now.AddMinutes(pTimeout);

                try
                {
                    pHashtableLock.EnterWriteLock();
                    pSessionItems[sessionID] = sessionData;
                }
                finally
                {
                    pHashtableLock.ExitWriteLock();
                }
            }

            // Add the session data to the current HttpContext.
            SessionStateUtility.AddHttpSessionStateToContext(context,
                             new HttpSessionStateContainer(sessionID,
                                                          sessionData.Items,
                                                          sessionData.StaticObjects,
                                                          pTimeout,
                                                          isNew,
                                                          pCookieMode,
                                                          SessionStateMode.Custom,
                                                          false));

            // Execute the Session_OnStart event for a new session.
            if (isNew && Start != null)
            {
                Start(this, EventArgs.Empty);
            }
        }

        //
        // Event for Session_OnStart event in the Global.asax file.
        //
        public event EventHandler Start;

        //
        // Event handler for HttpApplication.ReleaseRequestState
        //
        private void OnReleaseRequestState(object source, EventArgs args)
        {
            releaseCalled = true;
            HttpApplication app = (HttpApplication)source;
            HttpContext context = app.Context;
            string sessionID;

            // Read the session state from the context
            HttpSessionStateContainer stateProvider =
              (HttpSessionStateContainer)(SessionStateUtility.GetHttpSessionStateFromContext(context));

            // If Session.Abandon() was called, remove the session data from the local Hashtable
            // and execute the Session_OnEnd event from the Global.asax file.
            if (stateProvider.IsAbandoned)
            {
                try
                {
                    pHashtableLock.EnterWriteLock();

                    sessionID = pSessionIDManager.GetSessionID(context);
                    pSessionItems.Remove(sessionID);
                }
                finally
                {
                    pHashtableLock.ExitWriteLock();
                }

                SessionStateUtility.RaiseSessionEnd(stateProvider, this, EventArgs.Empty);
            }

            SessionStateUtility.RemoveHttpSessionStateFromContext(context);
        }

        private void OnEndRequest(object source, EventArgs eventArgs)
        {
            try
            {
                // Thread is aborted - need to release session state anyway.
                if (!releaseCalled && acquireCalled)
                {
                    OnReleaseRequestState(source, eventArgs);
                }
            }
            finally
            {
                acquireCalled = false;
                releaseCalled = false;
            }
        }
    }
}
