using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Simple.Common.Net
{
    public class WebClient2 : WebClient
    {
        public WebClient2()
            : this(10000)
        {
        }

        public WebClient2(Int32 timeout)
            : base()
        {
            this.Timeout = timeout;
            Encoding = Encoding.UTF8;
        }

        public int Timeout { get; set; }
        public string ErrorMessage { get; set; }
        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            request.Timeout = Timeout;
            return request;
        }

        private bool DoResquest(string method, string url, string requestString, out string responseString)
        {
            //ServicePointManager.ServerCertificateValidationCallback = ValidateServerCertificate;

            responseString = "";
            ErrorMessage = "";
            try
            {
                byte[] byRequest = Encoding.UTF8.GetBytes(requestString);
                byte[] byResponse = this.UploadData(url, method, byRequest);
                responseString = Encoding.UTF8.GetString(byResponse);
            }
            catch (Exception exception)
            {
                this.ErrorMessage = exception.Message;
                return false;
            }
            return true;
        }

        public bool DoGet(string url, out string responseString)
        {
            responseString = "";
            try
            {
                byte[] responseData = DownloadData(url);

                if (responseData != null)
                {
                    responseString = Encoding.UTF8.GetString(responseData);
                    return true;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return false;
            }
            return true;
        }

        public bool DoPut(string url, string requestString, out string responseString)
        {
            return this.DoResquest("PUT", url, requestString, out responseString);
        }

        public bool DoPost(string url, string requestString, out string responseString)
        {
            return this.DoResquest("POST", url, requestString, out responseString);
        }

        public bool DoDelete(string url, out string responseString)
        {
            return this.DoResquest("DELETE", url, "", out responseString);
        }

        private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
