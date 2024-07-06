namespace WeChat.WebApi
{
    public class ControllerBaseEx : ControllerBase,IDisposable
    {
        protected DB_WeChatContext DBContext { get; }
        protected DBContextFactory Factory { get; }
        protected string RequestIP
        {
            get
            {
                var ip = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (string.IsNullOrEmpty(ip))
                {
                    ip = HttpContext.Connection.RemoteIpAddress.ToString();
                }
                return ip;
            }
            
        }
        protected Config Config { get; } = new Config();
        protected List<Task> WaitTasks = new List<Task>();
        protected ControllerBaseEx()
        {
            Factory = new DBContextFactory(Config);
            DBContext = Factory.Create<DB_WeChatContext>();
        }
        void IDisposable.Dispose()
        {
            //等待所有Task执行完毕
            foreach (var c in WaitTasks) c.Wait();
            //释放掉上下文
            DBContext.Dispose();
            //释放掉Tasks
            WaitTasks.Clear();
        }
        protected void StartToWait(Action action)
        {
            var task = Task.Factory.StartNew(action);
            WaitTasks.Add(task);
        }

        protected void Log(object args,string identity = "")
        {
            Address address = new();
            string region = (ControllerContext.RouteData.Values["controller"] ?? "NULL").ToString();
            string action = (ControllerContext.RouteData.Values["action"] ?? "NULL").ToString();
            var log = new TbApiLog()
            {
                Action = action,
                Region = region,
                Ip = RequestIP.Length < 5 ? "" : RequestIP,
                Address = RequestIP.Length < 5 ? "" : address.GetAddress(RequestIP),
                LogTime = DateTime.Now,
                Guid = identity,
                Arguments = JsonConvert.SerializeObject(args)
            };
            DBContext.TbApiLog.Add(log);
            DBContext.SaveChanges();
            
        }
        protected void Log(string args, string identity = "")
        {
            Address address = new();
            string region = (ControllerContext.RouteData.Values["controller"] ?? "NULL").ToString();
            string action = (ControllerContext.RouteData.Values["action"] ?? "NULL").ToString();
            var log = new TbApiLog()
            {
                Action = action,
                Region = region,
                Ip = RequestIP.Length < 5 ? "" : RequestIP,
                Address = RequestIP.Length < 5 ? "" : address.GetAddress(RequestIP),
                LogTime = DateTime.Now,
                Guid = identity,
                Arguments = args
            };
            DBContext.TbApiLog.Add(log);
            DBContext.SaveChanges();
        }
        protected string JsonString(object args) => JsonConvert.SerializeObject(args);
        protected ActionResult Json(object args) => new JsonResult(args);
        protected BadRequestObjectResult Error(Exception e)
        {
#if DEBUG
            return BadRequest(new { code = 400, result = e.Message, stack = e.StackTrace });
#else
            return BadRequest(new { code = 400, result = e.Message });
            
#endif
        }

        protected BadRequestObjectResult Error(string e) => BadRequest(new { code = 400, result = e });
        protected string Random(int length,string dic = "")
        {
            dic = dic == "" ? "qwertyuiopasdfghjklzxcvbnm0123456789QWERTYUIOPASDFGHJKLZXCVBNM" : dic == "-1" ? "0123456789" : dic;
            string data = "";Random rand = new Random();
            for(int i= 0; i < length; i++)
            {
                data += dic[rand.Next(dic.Length)];
            }
            return data;
        }
        protected void CheckNull(object data,string msg, Action action=null)
        {
            if(data == null)
            {
                if (action == null)
                    throw new Exception(msg);
                else
                {
                    action.Invoke();
                    throw new Exception(msg);
                }
            }
        }

        protected void CheckNotNull(object data, string msg, Action action=null)
        {
            if (data != null)
            {
                if(action == null)
                    throw new Exception(msg);
                else
                {
                    action.Invoke();
                    throw new Exception(msg);
                }
            }
        }

        protected byte[] ComputeMD5(params string[] data)
        {
            var md5 = MD5.Create();
            var result = md5.ComputeHash(Encoding.UTF8.GetBytes(string.Join("", data)));
            return result;
        }
       
      
    }
}
