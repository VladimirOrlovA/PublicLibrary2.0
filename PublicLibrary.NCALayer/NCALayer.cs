using System;
using System.Collections.Generic;
using System.Linq;
using WebSocketSharp;
using WebSocketSharp.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PublicLibrary.NCALayer
{
    public class ActiveTokens
    {
        public string[] responseObject { get; set; }
        public string code { get; set; }
        public string message { get; set; }
    }

    public class Info
    {
        public Info(string module, string method, Object[] args)
        {
            this.module = module;
            this.method = method;
            this.args = args;
        }

        public string module { get; set; }
        public string method { get; set; }
        public Object[] args { get; set; }
    }



    public class NCALayer
    {
        public WebSocket socket = null;

        public NCALayer() : this("http://127.0.0.1:13579", "wss://127.0.0.1:13579/")
        { }

        public NCALayer(string origin, string cryptoSocketUri)
        {
            socket = new WebSocket(cryptoSocketUri);
            socket.Origin = origin;

            socket.OnOpen += Socket_OnOpen;
            socket.OnClose += Socket_OnClose;
            socket.OnError += Socket_OnError;

            socket.Connect();
        }

        bool isConnect = false;

        public bool getActiveTokens()
        {
            if (!isConnect)
            {
                //error
                return false;
            }

            socket.OnMessage += (sender, e) =>
            {
                ActiveTokens result = 
                JsonConvert.DeserializeObject<ActiveTokens>(e.Data);
            };

            Info info = new Info("kz.gov.pki.knca.commonUtils", "getActiveTokens", null);

            socket.SendAsync(JsonConvert.SerializeObject(info), (result) => 
            {
                var test = result;
            });

            return true;

        }

        private void Socket_OnError(object sender, ErrorEventArgs e)
        {
            isConnect = false;
        }

        private void Socket_OnClose(object sender, CloseEventArgs e)
        {
            isConnect = false;
        }

        private void Socket_OnOpen(object sender, EventArgs e)
        {
            isConnect = true;
        }
    }
}
