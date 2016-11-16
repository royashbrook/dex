using System;
using System.Web;
using System.Net;

namespace dex
{
    public class LogEvent : System.Web.Management.WebRequestErrorEvent
    {
        public LogEvent(string message)
            : base(null, null, 100001, new Exception(message))
        {
        }
    }

    public class dexi
    {
        private HttpContext cx;
        public dexi()
        {
        }
        public dexi(HttpContext c)
        {
            this.cx = c;
            new LogEvent(string.Format("{0},{1},{2}", cx.Request.UserHostAddress, cx.Request.UserHostName, cx.Request.Url.ToString())).Raise();
        }
        
        public string defaultmsg = @"
            sample invocation:<br />
            <pre>http://thisurl/dex.ashx?k=yourkey&u=pathtoexefilewithoutexe</pre>
            <pre>http://royashbrook.apphb.com/dex.ashx?k=123&u=http://whatever.com/somefile</pre>
            by default, file will be sent as renameme.txt<br />
            optionally you can include the gzs=1 argument and you will receive a gzip stream<br />
            note that 'somefile' must *actually* exist on the target server as somefile.exe<br />
            if it's too big service may crash if you use the default. this instance has very little memory.<br />
            if you run into a problem with memory, add gzs=1 as it uses a 10k buffer.";
        public void ProcessRequest(){
            if (!sane())
                printDefault();
            else
                if (cx.Request["gzs"] != "1")
                    GetFileUsingWC();
                else
                    getfile();
        }
        private void GetFileUsingWC()
        {
            string safeurl = cx.Request["u"];
            cx.Response.ContentType = "application/octet-stream";
            cx.Response.AddHeader("Content-Disposition", "attachment; filename=\"renameme.txt\"");
            cx.Response.BinaryWrite(FileToByteArray(safeurl + ".exe"));
        }
        public byte[] FileToByteArray(string f)
        {
            using (var wc = new WebClient())
                return wc.DownloadData(f);
        }
        private void getfile()
        {
            string safeurl = cx.Request["u"];
            string url = safeurl + ".exe";
            string fileName = System.IO.Path.GetFileName(new Uri(safeurl).LocalPath) + ".gz";

            try
            {
                int bytesToRead = 10000;
                byte[] buffer = new Byte[bytesToRead];
                int length;
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
                using (HttpWebResponse rf = (HttpWebResponse)req.GetResponse())
                {
                    if (req.ContentLength > 0)
                        rf.ContentLength = req.ContentLength;
                    var resp = cx.Response;
                    resp.ContentType = "application/octet-stream";
                    resp.AddHeader("Content-Disposition", "attachment; filename=\"" + fileName + "\"");
                    //resp.AddHeader("Content-Length", rf.ContentLength.ToString());
                    using (System.IO.Stream s = rf.GetResponseStream())
                    using (var gz = new System.IO.Compression.GZipStream(resp.OutputStream, System.IO.Compression.CompressionMode.Compress))
                        while ((length = s.Read(buffer, 0, bytesToRead)) > 0 && resp.IsClientConnected)
                        {
                            gz.Write(buffer, 0, length);
                            resp.Flush();
                            buffer = new Byte[bytesToRead];
                        }
                }
            }
            catch (Exception ex)
            {
                cx.Response.ContentType = "text/plain";
                cx.Response.Write("error logged");
                new LogEvent(ex.Message).Raise();
            }
        }
        private bool sane()
        {
            //test key included while testing - will be something else in prod
            return cx.Request["k"] == "YaTl7a4akBMefeCZ" && cx.Request["u"] != null;
        }

        private void printDefault()
        {
            cx.Response.ContentType = "text/html";
            cx.Response.Write(defaultmsg);
        }
    }
}