

namespace WeChat.WebApi.Servers.SMS
{
    class MmsHttpClient : IDisposable
    {
        public CookieContainer CookieContainer { get; set; }
        public WebHeaderCollection Headers { get; private set; }
        public string UserAgent { get; set; }
        public int Timeout { get; set; }
        public string Accept { get; set; }
        public Encoding Aencding { get; set; }
        public MmsHttpClient()
        {
            this.Timeout = 100 * 1000;
            this.CookieContainer = new CookieContainer();
            Headers = new WebHeaderCollection();
            Aencding = Encoding.UTF8;
        }

        public MmsHttpClient(int timeout)
        {
            this.Timeout = timeout;
        }

        public enum Method
        {
            GET = 1, POST = 2
        };
        public string Request(string url, Method method, string data, Encoding encoding, string contentType = 
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            null, CredentialCache mycache = null, NameValueCollection moreHeader = null)
        {
            string urlFormated = url;

            if (method == Method.GET && !string.IsNullOrEmpty(data))
            {
                urlFormated = string.Format("{0}{1}{2}", url, url.IndexOf('?') > 0 ? "&" : "?", data);
//                Console.WriteLine("Url:" + urlFormated);
            }
 //           Console.WriteLine("body:" + data);
            HttpWebRequest req = CreateRequest(urlFormated);
            if (!string.IsNullOrEmpty(Accept))
            {
                req.Accept = Accept;
            }
            foreach (string key in Headers.Keys)
            {
                req.Headers.Add(key, Headers.Get(key));
            }
            if (null != moreHeader)
            {
                req.Headers.Add(moreHeader);
            }
            //req.Headers.Add(this.Header);
            req.ReadWriteTimeout = Timeout;
            req.Method = method.ToString();
            if (!string.IsNullOrEmpty(this.UserAgent))
            {
                req.UserAgent = UserAgent;
            }

            if (mycache != null)
            {
                req.Credentials = mycache;
            }
            if (!string.IsNullOrEmpty(contentType))
            {
                req.ContentType = contentType;
            }
            /*else if (method == Method.POST)
            {
                if (encoding == Encoding.UTF8)
                {
                    req.ContentType = string.Format("application/x-www-form-urlencoded;charset=utf-8");
                }
                else
                {
                    req.ContentType = string.Format("application/x-www-form-urlencoded");
                }
            }*/

            req.SendChunked = false;
            req.KeepAlive = true;
            req.ServicePoint.Expect100Continue = false;
            req.ServicePoint.ConnectionLimit = Int16.MaxValue;
            req.AutomaticDecompression = DecompressionMethods.None;

            req.CookieContainer = this.CookieContainer;

            byte[] binaryData = null;

            if (method == Method.POST && !string.IsNullOrEmpty(data))
            {
                byte[] bytes = encoding.GetBytes(data);

                req.ContentLength = bytes.Length + (binaryData == null ? 0 : binaryData.Length);
                Stream rs = req.GetRequestStream();
                rs.Write(bytes, 0, bytes.Length);

                if (binaryData != null)
                {
                    rs.Write(binaryData, 0, binaryData.Length);
                }

                rs.Flush();
                rs.Close();
            }
            string tempData = string.Empty;
            try
            {
                using (WebResponse res = req.GetResponse())
                {
                    this.CookieContainer = req.CookieContainer;

                    using (StreamReader sr = new StreamReader(res.GetResponseStream(), encoding))
                    {
                        tempData = sr.ReadToEnd();
                    }
                }
            }
            catch (WebException e)
            {
                using (StreamReader sr = new StreamReader(e.Response.GetResponseStream(), encoding))
                {
                    return sr.ReadToEnd();
                }
            }
            return tempData;
        }
        public void BinRequest(string url, Method method, string data, Encoding encoding, string contentType = null, CredentialCache mycache = null, NameValueCollection moreHeader = null)
        {
            string urlFormated = url;

            if (method == Method.GET && !string.IsNullOrEmpty(data))
            {
                urlFormated = string.Format("{0}{1}{2}", url, url.IndexOf('?') > 0 ? "&" : "?", data);
                Console.WriteLine("Url:" + urlFormated);
            }
            Console.WriteLine("body:" + data);
            req = CreateRequest(urlFormated);
            if (!string.IsNullOrEmpty(Accept))
            {
                req.Accept = Accept;
            }
            foreach (string key in Headers.Keys)
            {
                req.Headers.Add(key, Headers.Get(key));
            }
            if (null != moreHeader)
            {
                req.Headers.Add(moreHeader);
            }
            req.ReadWriteTimeout = Timeout;
            req.Method = method.ToString();
            if (!string.IsNullOrEmpty(this.UserAgent))
            {
                req.UserAgent = UserAgent;
            }

            if (mycache != null)
            {
                req.Credentials = mycache;
            }
            if (!string.IsNullOrEmpty(contentType))
            {
                req.ContentType = contentType;
            }
            req.SendChunked = false;
            req.KeepAlive = true;
            req.ServicePoint.Expect100Continue = false;
            req.ServicePoint.ConnectionLimit = Int16.MaxValue;
            req.AutomaticDecompression = DecompressionMethods.None;

            req.CookieContainer = this.CookieContainer;

            byte[] binaryData = null;

            if (method == Method.POST && !string.IsNullOrEmpty(data))
            {
                byte[] bytes = encoding.GetBytes(data);

                req.ContentLength = bytes.Length + (binaryData == null ? 0 : binaryData.Length);
                Stream rs = req.GetRequestStream();
                rs.Write(bytes, 0, bytes.Length);

                if (binaryData != null)
                {
                    rs.Write(binaryData, 0, binaryData.Length);
                }

                rs.Flush();
                rs.Close();
            }
            object parameter = new object();
            var result = req.BeginGetResponse(new AsyncCallback(ReceivedResource), parameter);
            /*try
            {
                using (WebResponse res = req.GetResponse())
                {
                    try
                    {
                        this.CookieContainer = req.CookieContainer;

                        using (StreamReader sr = new StreamReader(res.GetResponseStream(), Aencding))
                        {
                            byte[] temp = new byte[res.ContentLength];
                            Stream seam = res.GetResponseStream();
                            seam.Read(temp, 0, (int)res.ContentLength);
                            CreatFileContent(@"F:\QQdowload\X3333333333.jpg", temp);
                            return sr.ReadToEnd();
                        }
                    }
                    finally
                    {
                        res.Close();
                    }
                }
            }
            catch (WebException e)
            {
                using (StreamReader sr = new StreamReader(e.Response.GetResponseStream(), encoding))
                {
                    return sr.ReadToEnd();
                }
            }*/
        }
        HttpWebRequest req;
        private byte[] _data;//= new byte[BUFFER_SIZE];
        private void ReceivedResource(IAsyncResult ar)
        {
            try
            {

                HttpWebResponse res = (HttpWebResponse)req.EndGetResponse(ar);
                if (res != null && res.StatusCode == HttpStatusCode.OK)
                {
                    Stream resStream = res.GetResponseStream();
                    _data = new byte[res.ContentLength];
                    resStream.Read(_data, 0, _data.Length);
                    string temp = Encoding.Default.GetString(_data, 0, _data.Length);
                    //var result = resStream.BeginRead(_data, 0, (int)res.ContentLength,new AsyncCallback(ReceivedData), resStream);
                }
                else
                {
                    res.Close();
                }
            }
            catch (WebException we)
            {

            }
        }
        public ArrayList arraylistImg = new ArrayList();
        private void ReceivedData(IAsyncResult ar)
        {
            Stream resStream = (Stream)ar.AsyncState;
            int read;
            read = resStream.EndRead(ar);
            if (read > 0)
            {
                //把异步从流中读到的字节存到arraylistImg里
                string temp = Encoding.Default.GetString(_data, 0, read);
                //var result = resStream.BeginRead(_data, 0, BUFFER_SIZE, new AsyncCallback(ReceivedData), resStream);
                return;
            }
        }
        delegate void DoTask(string res);
        public void CreatFileContent(string path, byte[] data)
        {
            FileStream fileContent = new FileStream(path, FileMode.Create);
            fileContent.Write(data, 0, data.Length);
            fileContent.Close();
        }
        public string Request(MultiPartsRequest mr, Encoding encoding, string referer = "")
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                mr.ToStream(ms, encoding);
                HttpWebRequest webRequest = CreateRequest(mr.Url);
                webRequest.ReadWriteTimeout = Timeout;
                if (!string.IsNullOrEmpty(Accept))
                {
                    webRequest.Accept = Accept;
                }
                if (!string.IsNullOrEmpty(referer))
                {
                    webRequest.Referer = referer;
                }
                foreach (string key in Headers.Keys)
                {
                    webRequest.Headers.Add(key, Headers.Get(key));
                }
                webRequest.Method = Method.POST.ToString();
                webRequest.UserAgent = UserAgent;
                webRequest.SendChunked = false;
                webRequest.KeepAlive = true;
                webRequest.ServicePoint.Expect100Continue = false;
                webRequest.ServicePoint.ConnectionLimit = 100;
                webRequest.ContentType = mr.ContentType;
                webRequest.CookieContainer = CookieContainer;
                byte[] bytes = ms.ToArray();
                webRequest.ContentLength = bytes.Length;
                Stream rs = webRequest.GetRequestStream();
                rs.Write(bytes, 0, bytes.Length);
                rs.Flush();
                rs.Close();
                try
                {
                    using (WebResponse res = webRequest.GetResponse())
                    {
                        this.CookieContainer = webRequest.CookieContainer;
                        using (StreamReader sr = new StreamReader(res.GetResponseStream(), encoding))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
                catch (WebException e)
                {
                    string responseText = e.Message;
                    using (WebResponse response = e.Response)
                    {
                        HttpWebResponse httpResponse = (HttpWebResponse)response;
                        Console.WriteLine("Error code: {0}", httpResponse.StatusCode);
                        using (Stream responseData = response.GetResponseStream())
                        using (var reader = new StreamReader(responseData))
                        {
                            responseText = reader.ReadToEnd();
                        }
                    }
                    throw new WebException(string.IsNullOrEmpty(responseText) ? e.Status.ToString() : responseText, e);
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        private static HttpWebRequest CreateRequest(string url)
        {
            HttpWebRequest req = null;

            if (url.Contains("https"))
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolTypeExtensions.Tls12 | SecurityProtocolTypeExtensions.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                req = (HttpWebRequest)WebRequest.Create(url);
                req.ProtocolVersion = HttpVersion.Version11;
            }
            else
            {
                req = (HttpWebRequest)WebRequest.Create(url);
            }

            return req;
        }

        #region IDisposable 成员

        public void Dispose()
        {
        }

        #endregion
        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

        private NameValueCollection Header = new NameValueCollection();

        public void SetHeader(string key, string value)
        {
            this.Header[key] = value;
        }

        public void ClearHeader()
        {
            this.Header.Clear();
        }
        private static class SecurityProtocolTypeExtensions
        {
            public const SecurityProtocolType Tls12 = (SecurityProtocolType)SslProtocolsExtensions.Tls12;
            public const SecurityProtocolType Tls11 = (SecurityProtocolType)SslProtocolsExtensions.Tls11;
            public const SecurityProtocolType SystemDefault = (SecurityProtocolType)0;
        }
        private static class SslProtocolsExtensions
        {
            public const SslProtocols Tls12 = (SslProtocols)0x00000C00;
            public const SslProtocols Tls11 = (SslProtocols)0x00000300;
        }
        private string NameValueCollectionToString(NameValueCollection collection, Encoding encoding)
        {
            string temp = string.Empty;

            int n = 0;

            foreach (string key in collection)
            {
                if (n == collection.Count - 1)
                {
                    temp += string.Format("{0}={1}", key, HttpUtility.UrlEncode(collection[key]));
                }
                else
                {
                    temp += string.Format("{0}={1}&", key, HttpUtility.UrlEncode(collection[key], encoding));
                }
                n++;
            }

            return temp;
        }
    }
    class FilePart : Part
    {
        private byte[] Data;
        private string Name;
        private string FileName;
        public FilePart(byte[] data, string name, string filename)
        {
            this.Name = name;
            this.FileName = filename;
            this.Data = data;
        }
        public override void ToStream(Stream stream, Encoding encoding, string boundary)
        {
            string contentType = "application/octet-stream";
            /*if (FileName.EndsWith("txt"))
            {
                contentType = "text/plain";
            }
            else if (FileName.EndsWith("smil"))
            {
                contentType = "text/xml";
            }
            else if (FileName.EndsWith("jpg"))
            {
                contentType = "image/jpeg";
            }
            else if (FileName.EndsWith("png"))
            {
                contentType = "image/png";
            }*/
            string header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type:{3}\r\n\r\n",
                boundary, this.Name, this.FileName, contentType);
            Part.StringToStream(stream, encoding, header);
            stream.Write(this.Data, 0, this.Data.Length);
            //Part.StringToStream(stream, encoding, "\r\n");
        }
    }
    class TxtPart : Part
    {
        private string Data;
        private string Name;
        public TxtPart(string name, string data)
        {
            this.Name = name;
            this.Data = data;
        }
        public override void ToStream(Stream stream, Encoding encoding, string boundary)
        {
            string header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}",
                boundary, this.Name, this.Data);
            Part.StringToStream(stream, encoding, header);
        }
    }
    class MultiPartsRequest
    {
        // 边界符
        private string Boundary;
        // 边界符
        private string BeginBoundary;
        // 最后的结束符
        private string EndBoundary;

        private List<Part> List = new List<Part>();
        public string ContentType
        {
            get
            {
                return string.Format("multipart/form-data; boundary={0}", Boundary);
            }
        }

        public string Url
        {
            get;
            set;
        }

        public MultiPartsRequest(string url)
        {
            this.Url = url;
            // 边界符
            this.Boundary = string.Format("---{0}", DateTime.Now.Ticks.ToString("x"));
            // 边界符
            this.BeginBoundary = string.Format("--{0}\r\n", Boundary);
            // 最后的结束符 
            this.EndBoundary = string.Format("--{0}--\r\n", Boundary);
        }
        public MultiPartsRequest(string url, string boundary)
        {
            this.Url = url;
            // 边界符
            this.Boundary = boundary;
            // 边界符
            this.BeginBoundary = string.Format("--{0}\r\n", Boundary);
            // 最后的结束符 
            this.EndBoundary = string.Format("--{0}--\r\n", Boundary);
        }

        public void AddPart(Part part)
        {
            List.Add(part);
        }

        public void Clear()
        {
            List.Clear();
        }

        public void RemovePart(Part part)
        {
            List.Remove(part);
        }

        public void ToStream(Stream stream, Encoding encoding)
        {
            foreach (Part part in this.List)
            {
                part.ToStream(stream, encoding, Boundary);
                Part.StringToStream(stream, encoding, "\r\n");
            }
            Part.StringToStream(stream, encoding, EndBoundary);

            stream.Flush();
        }
    }
    public abstract class Part
    {
        protected Part() { }

        public static void StringToStream(Stream stream, Encoding encoding, string data)
        {
            byte[] st = encoding.GetBytes(data);
            stream.Write(st, 0, st.Length);
            /*using (StreamWriter sw = new StreamWriter(stream, encoding))
            {
                sw.Write(data);
                //sw.Flush();
                //sw.Close();
            }*/
        }
        public virtual void ToStream(Stream stream, Encoding encoding, string boundary) { }
    }
}
