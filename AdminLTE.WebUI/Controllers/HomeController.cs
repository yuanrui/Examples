using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdminLTE.WebUI.Common;
using AdminLTE.WebUI.Models;

namespace AdminLTE.WebUI.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        public class HttpTraceListener : System.Diagnostics.TraceListener
        {
            public HttpContext Context
            {
                get
                {
                    return System.Web.HttpContext.Current;
                }
            }

            public override void  Write(string message)
            {
                if (Context == null)
                {
                    return;
                }

                Context.Response.Write(message);
                Context.Response.Write("<br/>");
            }

            public override void  WriteLine(string message)
            {
                if (Context == null)
                {
                    return;
                }

                Context.Response.Write(message);
                Context.Response.Write("<br/>");
            }
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Install()
        {
            System.Diagnostics.Debug.Listeners.Add(new HttpTraceListener());

            SimpleDataContext ctx = new SimpleDataContext();
            ctx.Database.Initialize(true);
                        
            return Content("End");
        }

        private string GetCurrentUrl()
        {
            var routeData = HttpContext.Request.RequestContext.RouteData;
            var result = "/" + UrlUtils.Combine((routeData.DataTokens["area"] ?? string.Empty).ToString(),
                    routeData.Values["controller"].ToString(),
                    routeData.Values["action"].ToString());
            
            return result;
        }

        [ChildActionOnly, AllowAnonymous]
        public ActionResult Menu()
        {
            return View();
        }
    }
}
