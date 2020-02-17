using System;
using System.Collections.Generic;
using System.Linq;
using WebSocketSharp;
using WebSocketSharp.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using static ERDROffline.NCAlayer.Lib.NCAlayer;

namespace ERDROffline.NCAlayer.Lib
{
    //https://ezsigner.kz/
    //file:///C:/Users/ayako/Documents/SDK2/NCALayer/commonbundle_sample/index.html
    // public delegate void OnMessage_ResultKey(ResultKey e);
    public delegate void ErrorMessage(string message);
    public delegate bool ResultSIGNATURE(CAdESFromFile caResult, string destinationFilePath);
    public delegate void ResultActiveTokens(ActiveTokens activeTokens);
    public delegate void ResultKeyInfo(KeyInfo keyInfo);

    public class NCAlayer
    {
        public WebSocket socket = null;
        public bool isConnect { get; set; }
        public NCAlayer() : this("http://127.0.0.1:13579", "wss://127.0.0.1:13579/") { }

        public NCAlayer(string origin, string cryptoSocketUri)
        {
            socket = new WebSocket(cryptoSocketUri);
            socket.Origin = origin;

            socket.OnOpen += Socket_OnOpen;
            socket.OnClose += Socket_OnClose;
            socket.OnError += Socket_OnError;

            socket.Connect();
        }

        public ErrorMessage errMessage;
        public ResultSIGNATURE resSIGNATURE;
        public ResultActiveTokens activeTokens;
        public ResultKeyInfo keyInfo;

        private void Socket_OnClose(object sender, CloseEventArgs e)
        {
            isConnect = false;
        }

        private void Socket_OnError(object sender, WebSocketSharp.ErrorEventArgs e)
        {
            var test = "";
        }

        private void Socket_OnOpen(object sender, EventArgs e)
        {
            isConnect = true;
        }


        public bool getKeyInfo()
        {
            if (!isConnect)
            {
                errMessage.Invoke("CryptoSocket not avaible");
                return false;
            }

            string[] args = new string[1];
            args[0] = "PKCS12";

            Info keyInfo = new Info("kz.gov.pki.knca.commonUtils", "getKeyInfo", args) { };
            socket.OnMessage += (sender, e) =>
            {
                Console.WriteLine(e.Data);
                /*
                 
                 {"responseObject":{"certNotBefore":"1565414209000","issuerCn":"?ЛТТЫ? КУ?ЛАНДЫРУШЫ ОРТАЛЫ? (RSA)","authorityKeyIdentifier":"5b6a7411","serialNumber":"6c10cd02a1a5001ddd2f7aedabbc37c9a4b7c093","certNotAfter":"1596950209000","issuerDn":"C=KZ,CN=?ЛТТЫ? КУ?ЛАНДЫРУШЫ ОРТАЛЫ? (RSA)","keyId":"4c28140f289aeafc8c8a6b6055fb2079d4139386","alias":"4c28140f289aeafc8c8a6b6055fb2079d4139386","pem":"SERTIFICATE Code","subjectCn":"ГЕРЦЕН ЕВГЕНИЙ","algorithm":"RSA","subjectDn":"CN=ГЕРЦЕН ЕВГЕНИЙ,SURNAME=ГЕРЦЕН,SERIALNUMBER=IIN880111300392,C=KZ,L=КАРАСАЙСКИЙ РАЙОН,S=АЛМАТИНСКАЯ ОБЛАСТЬ,G=АЛЕКСАНДРОВИЧ"},"code":"200"}
                 */
            };

            socket.SendAsync(JsonConvert.SerializeObject(keyInfo), (result) => { Console.WriteLine(result); });
            return true;
        }

        public bool showFileChooser(string fileExtension = "ALL", string currentDirectory = "")
        {
            if (!isConnect)
            {
                errMessage.Invoke("CryptoSocket not avaible");
                return false;
            }

            string[] args = new string[2];
            args[0] = fileExtension;
            args[1] = currentDirectory;

            Info info = new Info("kz.gov.pki.knca.commonUtils", "showFileChooser", args) { };
            socket.OnMessage += (sender, e) =>
            {
                Console.WriteLine(e.Data);
                //{"responseObject":"C:\\Users\\ayako\\Documents\\Копия документа.txt","code":"200"}
            };

            socket.SendAsync(JsonConvert.SerializeObject(info), (result) => { Console.WriteLine(result); });

            return true;
        }

        public bool createCAdESFromFile(string storageName, string keyType, string filePath, bool flag)
        {
            if (!isConnect)
            {
                errMessage.Invoke("CryptoSocket not avaible");
                return false;
            }

            Object[] args = new Object[4];
            args[0] = storageName;
            args[1] = keyType;
            args[2] = filePath;
            args[3] = flag;

            Info info = new Info("kz.gov.pki.knca.commonUtils", "createCAdESFromFile", args) { };
            socket.OnMessage += (sender, e) =>
            {
                Console.WriteLine(e.Data);
                CAdESFromFile result = JsonConvert.DeserializeObject<CAdESFromFile>(e.Data);

                resSIGNATURE.Invoke(result, filePath);

                //{"responseObject":"","code":"200"}
            };

            socket.SendAsync(JsonConvert.SerializeObject(info), (result) => { Console.WriteLine(result); });

            return true;
        }

        public bool getActiveTokens()
        {
            if (!isConnect)
            {
                errMessage.Invoke("CryptoSocket not avaible");
                return false;
            }

            Info info = new Info("kz.gov.pki.knca.commonUtils", "getActiveTokens", null) { };
            socket.OnMessage += (sender, e) =>
            {
                Console.WriteLine(e.Data);
                ActiveTokens result = JsonConvert.DeserializeObject<ActiveTokens>(e.Data);

                activeTokens.Invoke(result);


                //{"responseObject":["AKKaztokenStore"],"code":"200"}
            };

            socket.SendAsync(JsonConvert.SerializeObject(info), (result) => { Console.WriteLine(result); });
            return true;
        }

        public bool getKeyInfo(string storageName)
        {
            if (!isConnect)
            {
                errMessage.Invoke("CryptoSocket not avaible");
                return false;
            }

            Object[] args = new Object[1];
            args[0] = storageName;

            Info info = new Info("kz.gov.pki.knca.commonUtils", "getKeyInfo", args) { };
            socket.OnMessage += (sender, e) =>
            {
                Console.WriteLine(e.Data);
                KeyInfo result = JsonConvert.DeserializeObject<KeyInfo>(e.Data);
                keyInfo.Invoke(result);
            };

            socket.SendAsync(JsonConvert.SerializeObject(info), (result) => { Console.WriteLine(result); });

            return true;
        }
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

    public class ActiveTokens
    {
        public string[] responseObject { get; set; }
        public string code { get; set; }
        public string message { get; set; }
    }

    public class CAdESFromFile
    {
        public string responseObject { get; set; }
        public string code { get; set; }
        public string message { get; set; }
    }

    public class KeyInfo
    {
        public CertInfo responseObject { get; set; }
        public string code { get; set; }
    }
    public class CertInfo
    {
        /// <summary>
        /// Дата начала действия сертификата
        /// </summary>
        public string certNotBefore { get; set; }
        public DateTime certStartDate
        {
            get
            {
                System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                dtDateTime = dtDateTime.AddMilliseconds(double.Parse(certNotBefore)).ToLocalTime();
                return dtDateTime;
            }
        }

        /// <summary>
        /// Дата окончания действия сертификата
        /// </summary>
        public string certNotAfter { get; set; }

        public DateTime certEndtDate
        {
            get
            {
                System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                dtDateTime = dtDateTime.AddMilliseconds(double.Parse(certNotAfter)).ToLocalTime();
                return dtDateTime;
            }
        }

        public string issuerCn { get; set; }
        public string authorityKeyIdentifier { get; set; }
        public string serialNumber { get; set; }
        public string issuerDn { get; set; }
        public string keyId { get; set; }
        public string alias { get; set; }
        public string pem { get; set; }
        public string subjectCn { get; set; }
        public string algorithm { get; set; }
        public string subjectDn { get; set; }
        public string CertificateStore { get; set; }

        //CN=ГЕРЦЕН ЕВГЕНИЙ,SURNAME=ГЕРЦЕН,SERIALNUMBER=IIN880111300392,C=KZ,L=АЛМАТЫ,S=АЛМАТЫ,G=АЛЕКСАНДРОВИЧ
        public string getIIN { get
            {
                return subjectDn.Substring(subjectDn.IndexOf("IIN") + 3, 12);
            }
        }

        public string getSurname
        {
            get
            {
                if(!string.IsNullOrWhiteSpace(subjectDn))
                {
                    var subjectDnArr = subjectDn.Split(',');

                    if (subjectDnArr.Count() > 0)
                    {
                        string surname = subjectDnArr[1];
                        return surname.Substring(surname.IndexOf("=") + 1);
                    }
                    else
                        return "";
                }
                else
                    return "";
            }
        }
    }
    
     
    /* {"responseObject":{"certNotBefore":"1575361915000","issuerCn":"ҰЛТТЫҚ КУӘЛАНДЫРУШЫ ОРТАЛЫҚ (RSA)","authorityKeyIdentifier":"5b6a7411","serialNumber":"49f5317f33dbf807f73f86661fe8b8b106ed42c0","certNotAfter":"1669969915000","issuerDn":"C=KZ,CN=ҰЛТТЫҚ КУӘЛАНДЫРУШЫ ОРТАЛЫҚ (RSA)","keyId":"69ca78131697edd2200bf564","alias":"69ca78131697edd2200bf564","pem":"-----BEGIN CERTIFICATE-----\r\nMIIGbTCCBFWgAwIBAgIUSfUxfzPb+Af3P4ZmH+i4sQbtQsAwDQYJKoZIhvcNAQEL\r\nBQAwUjELMAkGA1UEBhMCS1oxQzBBBgNVBAMMOtKw0JvQotCi0KvSmiDQmtCj05jQ\r\nm9CQ0J3QlNCr0KDQo9Co0Ksg0J7QoNCi0JDQm9Cr0pogKFJTQSkwHhcNMTkxMjAz\r\nMDgzMTU1WhcNMjIxMjAyMDgzMTU1WjCBtzEkMCIGA1UEAwwb0JPQldCg0KbQldCd\r\nINCV0JLQk9CV0J3QmNCZMRUwEwYDVQQEDAzQk9CV0KDQptCV0J0xGDAWBgNVBAUT\r\nD0lJTjg4MDExMTMwMDM5MjELMAkGA1UEBhMCS1oxFTATBgNVBAcMDNCQ0JvQnNCQ\r\n0KLQqzEVMBMGA1UECAwM0JDQm9Cc0JDQotCrMSMwIQYDVQQqDBrQkNCb0JXQmtCh\r\n0JDQndCU0KDQntCS0JjQpzCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEB\r\nAMz6nTtNoaPh3GdhudmUTUsROnK+R9XsOEGYBDMTJjDuz0oQH44AfL9R0xM1XtAB\r\nIzIZJyUjVO4/AjG32HasacQAgoiPFMR0q0hpI9O5K1N3AqevF4t9yLit6mEBG9wJ\r\nqlSZXlSA7kZcJc6bDkCz792hwTZpXDtf6uiHGuw3PArSoalcozM/P/qyo9AtI7s4\r\nM4DPOnMFCECcZJS+xfXjDONvWYDp4pkcvHN0wgoNgXwrutH0Pv9aHcI9dg8wWrn+\r\nesunuyYs/RQSkolHzeZDDTEt2F2+XtKWDOzJJfI0zsOv/uu+e70gNUGgoQjefRSt\r\n6P9Lup3qY/VWEE1MXR2WUwsCAwEAAaOCAdMwggHPMA4GA1UdDwEB/wQEAwIFoDAd\r\nBgNVHSUEFjAUBggrBgEFBQcDAgYIKoMOAwMEAQEwDwYDVR0jBAgwBoAEW2p0ETAV\r\nBgNVHQ4EDgQMacp4ExaX7dIgC/VkMF4GA1UdIARXMFUwUwYHKoMOAwMCBDBIMCEG\r\nCCsGAQUFBwIBFhVodHRwOi8vcGtpLmdvdi5rei9jcHMwIwYIKwYBBQUHAgIwFwwV\r\naHR0cDovL3BraS5nb3Yua3ovY3BzMFYGA1UdHwRPME0wS6BJoEeGIWh0dHA6Ly9j\r\ncmwucGtpLmdvdi5rei9uY2FfcnNhLmNybIYiaHR0cDovL2NybDEucGtpLmdvdi5r\r\nei9uY2FfcnNhLmNybDBaBgNVHS4EUzBRME+gTaBLhiNodHRwOi8vY3JsLnBraS5n\r\nb3Yua3ovbmNhX2RfcnNhLmNybIYkaHR0cDovL2NybDEucGtpLmdvdi5rei9uY2Ff\r\nZF9yc2EuY3JsMGIGCCsGAQUFBwEBBFYwVDAuBggrBgEFBQcwAoYiaHR0cDovL3Br\r\naS5nb3Yua3ovY2VydC9uY2FfcnNhLmNlcjAiBggrBgEFBQcwAYYWaHR0cDovL29j\r\nc3AucGtpLmdvdi5rejANBgkqhkiG9w0BAQsFAAOCAgEAcf3dEWmQ7Ewkw1WpvdYW\r\ne8SUh1IlEAx+Cs8bFCeC5c/X8uB116ApE84Q8CPqs/WWjkwIyC+SOD44pDb7S2pw\r\nbqWyFCO7Ilvh2fctyT+suewtYE7s+hItTlqSQC2nLQH/4QsHtGGIqdNNmptuQDVB\r\n+dTPO10W4hepqHD+YOFURELeRSAClbI6bEC/ER2Dn3+IrZ5d9pPSXTUpOfw6U6s+\r\nWz0WsGUF1GGdN+vK8v21dWG9xyQCOay+K99oCBBjlOnMK22cumwcnovHGwDTNSld\r\nl0yR9fx7ljARRr3W0MaY8Vjr5g3QYPBdU3N/ewVyM7NdNqFiHRYN70dooXAnqKic\r\nJhWQwTmjUmxjyAv3ibYxWO6Lg61gdqWdszhZnikaadN7PDQLm5Qo2GlGJCWXZR1M\r\nPWZ30DoQgMeAte/E/hGJpbbcR9N8InVQ720lEpX9vAtFVUo2TlkJcwY1+gQdGwmw\r\nctW5pWd75ow5+5xwOwoQGI93a78+wpmQBtGQlXP1d/Uzbwcp4w2lX4j0+fR3L519\r\nh31asCNEeGJPTdOpV1QgtuQcumWeJ7ih+TWcHf7VxALwD7zrd7hE3+tyKgDkqHvs\r\n3ysj7jfFtYRQcmKbv7P/PlX9mYPKaXvIAkHLCd/6nnuJ8S1iyjHUE909lk+U3fBi\r\nvjg0EK1V5jqJ1xnHlw2Ikeg=\r\n-----END CERTIFICATE-----\r\n","subjectCn":"ГЕРЦЕН ЕВГЕНИЙ","algorithm":"RSA","subjectDn":"CN=ГЕРЦЕН ЕВГЕНИЙ,SURNAME=ГЕРЦЕН,SERIALNUMBER=IIN880111300392,C=KZ,L=АЛМАТЫ,S=АЛМАТЫ,G=АЛЕКСАНДРОВИЧ"},"code":"200"}
     */
}
