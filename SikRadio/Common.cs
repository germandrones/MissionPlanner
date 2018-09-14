using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
// dll imports

namespace MissionPlanner
{
    public class Common
    {
        public static bool getFilefromNet(string url, string saveto)
        {
            try
            {
                // this is for mono to a ssl server
                //ServicePointManager.CertificatePolicy = new NoCheckCertificatePolicy(); 

                ServicePointManager.ServerCertificateValidationCallback =
                    (sender, certificate, chain, policyErrors) => { return true; };

                // Create a request using a URL that can receive a post. 
                var request = WebRequest.Create(url);
                request.Timeout = 10000;
                // Set the Method property of the request to POST.
                request.Method = "GET";
                // Get the response.
                var response = request.GetResponse();
                // Display the status.
                if (((HttpWebResponse) response).StatusCode != HttpStatusCode.OK)
                    return false;
                // Get the stream containing content returned by the server.
                var dataStream = response.GetResponseStream();

                var bytes = response.ContentLength;
                var contlen = bytes;

                var buf1 = new byte[1024];

                var fs = new FileStream(saveto + ".new", FileMode.Create);

                var dt = DateTime.Now;

                while (dataStream.CanRead && bytes > 0)
                {
                    Application.DoEvents();
                    var len = dataStream.Read(buf1, 0, buf1.Length);
                    bytes -= len;
                    fs.Write(buf1, 0, len);
                }

                fs.Close();
                dataStream.Close();
                response.Close();

                File.Delete(saveto);
                File.Move(saveto + ".new", saveto);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}