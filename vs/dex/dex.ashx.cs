using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Web.Management;


namespace dex
{
    
    /// <summary>
    /// Summary description for dex
    /// </summary>
    public class dex : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            var i = new dexi(context);
            i.ProcessRequest();
            i = null;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

    }
}