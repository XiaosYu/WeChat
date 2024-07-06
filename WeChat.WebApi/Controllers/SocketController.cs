

namespace WeChat.WebApi.Controllers
{
    [Route("[controller]")]
    //[ApiController]
    public class SocketController:ControllerBaseEx
    {
        public SocketController()
        {
        }

        public static ConcurrentDictionary<string, WebSocket> Clients = new();
        public static ConcurrentDictionary<string, string> Users = new();

        [HttpGet]
        public async Task Connect(string uid)
        {
            if (string.IsNullOrEmpty(uid))
            {
                HttpContext.Response.StatusCode = 404;
            }
            else if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using WebSocket socket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                Console.WriteLine($"|INFO|\t{uid}已连接");


                Users.TryAdd(uid, HttpContext.Connection.Id);
                Clients.TryAdd(HttpContext.Connection.Id, socket);

                await Run(uid,socket);

                Users.TryRemove(uid, out string ConnId);
                Clients.TryRemove(ConnId, out WebSocket outsocket);
            }
            else
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }

        }

        private async Task<bool> Run(string uid, WebSocket socket)
        {
            try
            {
                var db = Factory.Create<DB_WeChatContext>();
                var buffer = new byte[16384];  //16kb缓存

                WebSocketReceiveResult result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                while (!result.CloseStatus.HasValue)
                {
                    var data = Encoding.UTF8.GetString(buffer);
                    JObject pairs = JsonConvert.DeserializeObject<JObject>(data);

                    if (!pairs.ContainsKey("Verfiy")) break; //非软件连接

                    //写入消息Log
                    TbChatLog log = new()
                    {
                        Guid = (string)pairs["Guid"]??" ",
                        Action = (string)pairs["Action"]??" ",
                        Sender = (string)pairs["Sender"]??" ",
                        Receiever = (string)pairs["Receiever"]??" ",
                        Body = JsonConvert.SerializeObject(pairs["Body"]??" "),
                        SendTime = DateTime.Now
                    };
                    db.TbChatLog.Add(log);
                    db.SaveChanges();

                    await Forward(pairs, (string)pairs["Action"], socket);

                    buffer = new byte[16384];
                    
                    result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                }
                await socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                Console.WriteLine($"|INFO|\t{uid}已断开连接");
                return true;

            }catch(Exception ex)
            {
                return false;
            }
        }

        private async Task<bool> Forward(object data,string type,WebSocket socket)
        {
            switch(type)
            {
                case "Message":
                    {
                        var result = ((JObject)data).ToObject<BaseModel<MessageBody>>();
                        if(result.Receiever == "HOST")
                        {
                            Console.WriteLine($"|INFO|\t{result.Body.Message}");
                            var sender = result.Sender;
                            result.Sender = "HOST";
                            result.Receiever = sender;
                            //转发群消息
                            foreach(var item in Clients.Values.Except(new List<WebSocket>() { socket}))
                            {
                                await item.SendAsync(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(result)), WebSocketMessageType.Text, true, CancellationToken.None);
                            }
                            return true;
                        }
                        //寻找Socket并转发消息
                        var client = Clients[Users[result.Receiever]];
                        await client.SendAsync(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(result)), WebSocketMessageType.Text, true, CancellationToken.None);
                        Console.WriteLine($"|INFO|\t{result.Body.Message}");
                    }break;
            }
            return true;
        }

        public static async Task<bool> SendMessage(object data,string uid)
        {
            //判断是否在线
            if (!Users.ContainsKey(uid)) return false;
            var client = Clients[Users[uid]];
            await client.SendAsync(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data)), WebSocketMessageType.Text, true, CancellationToken.None);
            return true;

        }
        

       


    }

   
}
