using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using dex;
using System.Web;
using System.Diagnostics;

namespace dexTest
{
    //todo: need to cleanup these tests quite a bit
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void CreationWorks()
        {
            var d = new dex.dex();
            Assert.IsTrue(d != null);
            d = null;
        }

        [TestMethod]
        public void IsReusable()
        {
            var d = new dex.dex();
            Assert.IsFalse(d.IsReusable);
            d = null;
        }

        [TestMethod]
        public void EmptyQuerystring()
        {
            var di = new dex.dexi();
            var d = new dex.dex();
            
            HttpRequest req = new HttpRequest(@"~\dex.ashx","http://localhost/dex.ashx","");
            using (var writer = new System.IO.StringWriter())
            {
                HttpResponse resp = new HttpResponse(writer);
                var cx = new HttpContext(req, resp);
                d.ProcessRequest(cx);
                Assert.AreEqual(writer.ToString(), di.defaultmsg);
            }
            d = null;
            di = null;
        }

        [TestMethod]
        public void ExistingFile()
        {
            var d = new dex.dexi();
            //Debug.Assert(false, d.FileToByteArray("https://pirate.googlecode.com/files/Pirate-1008.exe").Length.ToString());
            Assert.AreEqual(
                d.FileToByteArray("https://pirate.googlecode.com/files/Pirate-1008.exe").Length, 991232);
            d = null;
        }

    }
}
