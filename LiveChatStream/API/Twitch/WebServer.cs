using System;
using System.IO;
using System.Linq;
using System.Net;

namespace LiveChatStream.API.Twitch
{
    public class WebServer
    {
        private HttpListener _listener { get; }

        public WebServer(string uri)
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add(uri);
        }

        public Models.Authorization Listen()
        {
            _listener.Start();
            return onRequest();
        }
        public void Stop()
        {
            _listener.Stop();
            _listener.Abort();
        }
        private Models.Authorization onRequest()
        {
            while (_listener.IsListening)
            {
                try
                {
                    var ctx = _listener.GetContext();
                    var req = ctx.Request;
                    var resp = ctx.Response;

                    using (var writer = new StreamWriter(resp.OutputStream))
                    {
                        if (req.QueryString.AllKeys.Any("code".Contains))
                        {
                            writer.WriteLine("Authorization started! Check your application!");
                            writer.Flush();
                            return new Models.Authorization(req.QueryString["code"]);
                        }
                        else
                        {
                            writer.WriteLine("No code found in query string!");
                            writer.Flush();
                        }
                    }
                }
                catch (HttpListenerException ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            }
            return null;
        }
    }
}
