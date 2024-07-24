# Wechat
Author：Jingyu Li lijingyu_ai@163.com

## Description of the mandate
### Goal
Imitation of QQ's chat programme , the server side using WebApi and WebSocket as the core technology , with and EntityFramework framework using the SqlServer database for the storage of Asp.Net applications , while the client is the use of Wpf framework to do the desktop application of the UI layer , real-time communication with the server to achieve the function of data transfer

### Functionality
+ Login to account, including registration and password recovery
+ Add friends and have a friend bar
+ Live chat with selected friends
+ Possibility to delete friends
+ Public chat room
+ You can search for your friends, and you can search for friends when you add them.

## Functional requirements analysis
### Login, Register, Reset Function
<P>The use of WebApi as a data management system , you can let the client and the database to isolate , to avoid direct reading and writing to the database , play a role in protecting the database . In the client login , construct a login URL for POST request , and login return value is only 400 and 200 , if it is 200 then login successfully , and when the return value is 200 , the server will return the user information of the logged in user .</P>
<p>Similarly, in the registration time, because the registration required is a mobile phone number, when the request is sent to the server, the server will use the external API to send a registration information to the user's mobile phone, and for the server, it will construct a VerifyCodeModel instance of the request for storage, the instance will contain an authentication code Code, mobile phone number Phone and the callback function CallBack and statically stored in the UserController class RegisterCodes field. When the user clicks the confirmation button, the Verfiy function will be requested, this time in the RegisterCodes to find this instance, if found and in the required time to match the success of the function, it is good to call the CallBack function will be registered to write information into the database!</p>
<p>Retrieve the password is the same, is also the use of Verfiy request, different from the time of registration, in the reset of the password is to modify the database operations</p>

### Communication Function
<p>First of all, for the server side, the server application will open the WebSocket service, and there is a SocketController will receive the login. Connect interface for the controller for the GET request, containing the parameter uid, because here the WebSocket connection will be connected after the completion of the login, so there is no request sent to the uid verification, and when the server handshake is successful, it will be opened to listen to the thread to the Run method, and the Run method is essentially a client uploading information for blocking the listener and forwarded according to the information. The WebSocket transmission is a Json string, which is serialised by BaseModel[T], so the server only needs to see the Sender and Receiever fields in BaseModel to forward the message.</p>
<p>For the client, the user input message will be sent according to the UID of the object to form a BaseModel[MessageBody] instance object, and then serialised and sent to the server, by the server through the connection of the socket sent to the target, and the target in the login is complete and handshake with the server is successful, will open a listening thread, if received from the server's Json text received from the server, it will judge the Sender field, if it is now in the chat interface with the person who sent the message, the message will be passed into the SendMessage function to be displayed in the ListView control, if it is not in the chat interface, the message will be stored in a Bucket in the cache, when the user opens the chat dialogue, the Bucket will be automatically stored in the Bucket. When the user opens a chat with them, the message in the Bucekt is automatically loaded into the ListView.</p>
<p>For the chat room, after loading the main page, an Item called ‘Happy Family’ will be added to the friend column, and the UID of this Item is ‘HOST’, if the message is from ‘HOST’, it will be loaded into the ListView automatically. If it is a message from ‘HOST’, the client's parsing function will parse ‘HOST’ to this interface, while the message sent to the service, the server will exchange Sender and Receiever, and when parsing, it will no longer be [Sender] -> [ Receiever] and directly Receiever, and Receiever is the person who sent the message.</p>

### Search Friend Function
<p>In the search box on the top of the friend column, enter the keyword and press enter to search, when you click enter, the client will request the Friend/SearchFriend interface, the key is the keyword, and the server will compare the keyword, it will compare to the EMail, Name, Phone, QUID and other fields, if there is a match, it will be returned by the client parsing. After the client parsing will appear on the left side of the friends column, and in the client to enter the main page and add friends will be reloaded after all the friends, so you need to re-check the friends of the time directly blank input and then enter can be</p>

### Add Friend Function
<p>Add Friends button in the search box next to the click will enter a search interface, when you enter the keywords, as in the search for friends, will be so not a friend of the search, and return value, you can choose the data inside the ListBox, elected to add friends click on Add Friends will send a request to the server and add a friend to close the window will be refreshed after the list of friends.For those who are online, if they are not refreshed, they do not have each other, which will create a one-way friend situation. In order to solve this problem, Friend/AddRelation interface will search for all connected Clients, if the search, it will construct an instance of BaseModel[AddBody], serialise it and send it to the object, and the client receives the Json string, and determines that the Action is an Add, it will deserialise the string and send it to the client. After deserialisation, the client will take the initiative to add the friend to the friend list.</p>

### Delete Friend Function
<p>Deleting a friend is done by right clicking on the friend bar to open the context menu and deleting the selected target, before deleting, it will be asked by MessageBox, and after confirming it, it will make a request to the Friend/DeleteRelation interface to delete the object at the database level, and then delete the object at the UI level.</p>

### Change of name and signature
<p>Modify the name and signature in the signature or name of the place to double-click to enter a TextBox, in the TextBox, enter the signature or name to be modified, and then press ENTER to submit, submit the background of the User/Modify interface request, the server to modify the database, and then complete the modification to return the results, if the modification is successful, client UI thread will update the display</p>

## Overall software design
### Software development environment and development model design
> IDE: Visual Studio 2022, Blend For Visual Studio 2022
> NET Framework: Net 6.0
> Development Language: C#
> IIS Version: 8.5
> Project Framework: Windows Presentation Foundation + ASP.NET Core WebApi
> Development Model: C/S  
> Client: UI - BLL - DAL

## Main functions, algorithm design description
### WeChat.WebApi.Controllers.SocketController.Run
> The Run function listens on each connected socket and forwards the message to the Forward function.

``` cs
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
```

### WeChat.WebApi.Controllers.SocketController.Forward
> The Forward function forwards the received data through the Sender and Reciever relationship

``` cs
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
```

### WeChat.WPF.UI.MainWindow.Connect
> In the MainWindow's Load function will be loaded inside the Connect function, Connect function will connect to the server's WebSocket and then open a listening thread to receive messages from the server.

``` cs
private async void Connect()
        {
            await ClientSocket.ConnectAsync(new Uri($"{StaticResource.WsUrl}?uid={Uid}"), CancellationToken.None);

            _ = Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    var buffer = new byte[1024 * 16];
                    var result = await ClientSocket.ReceiveAsync(buffer, CancellationToken.None);
                    CallBack(JsonConvert.DeserializeObject<JObject>(Encoding.UTF8.GetString(buffer)));
                    buffer = new byte[1024 * 16];
                }
            });

        }
```

### WeChat.WPF.UI.MainWindow.CallBack
> CallBack function will automatically be based on the received message to classify and display , if it is a Message message , the message will also be added to the Bucket inside , and the use of Dispatcher is to prevent thread UI conflicts .

``` cs
private void CallBack(JObject obj)
        {
            this.Dispatcher.Invoke(async () =>
            {
                if ((string)obj["Action"] == "Add")
                {
                    var result = (await Friend.SearchFriend((string)obj["Sender"])).Data.FirstOrDefault();
                    if (result == null) return;
                    Peoples.Add(result);
                    Shows.Add(result);
                    LB_Friend.Items.Add(result.Name);
                    return;
                }
                if ((string)obj["Action"] == "Message")
                {
                    Record record = new Record()
                    {
                        Sender = (string)obj["Sender"],
                        Receiever = (string)obj["Receiever"],
                        DateTime = DateTime.Now,
                        Message = (string)obj["Body"]["Message"]
                    };
                    if((string)obj["Sender"] == ChatPeople.Quid.TrimEnd())
                    {
                        ShowMessage(record);
                    }
                    //Message类加入Bucket
                    lock(Bucket)
                    {
                        Bucket.Put(record);
                    }                
                }
            });          
        }
```
### WeChat.WPF.UI.MainWindow.ShowMessage
> With a Record, analyse the composition of the Record and display it on the ListView control.
``` cs
 private void ShowMessage(Record record)
        {
            
            if (record.Sender == "HOST")
            {
                string name = UNames.SearchName(record.Receiever, () => new UserHelper().GetUserNames(Uid).Result);
                LV_Message.Items.Add($"[{DateTime.Now.ToString()}]\n{name}\n{record.Message}\n");
                LV_Message.ScrollIntoView(LV_Message.Items[LV_Message.Items.Count - 1]);
                return;
            }
            if (record.Sender == Uid)
            {
                string name = UNames.SearchName(record.Receiever, () => new UserHelper().GetUserNames(Uid).Result);
                LV_Message.Items.Add($"[{DateTime.Now.ToString()}]\nMe -> {name}\n{record.Message}\n");
            }
            else
            {
                string name = UNames.SearchName(record.Sender, () => new UserHelper().GetUserNames(Uid).Result);
                LV_Message.Items.Add($"[{DateTime.Now.ToString()}]\n{name} -> Me\n{record.Message}\n");
            }
            
            LV_Message.ScrollIntoView(LV_Message.Items[LV_Message.Items.Count - 1]);
        }
```

## Detailed software design and coding
In order to prevent direct client injection into the database , all use WebApi as an intermediary medium for the transfer of data , provide a User interface to do the login , registration and reset , and in the client side , write a UserHelper class as the content of the DAL layer , and in order to achieve the function of sending messages , the use of WebSocket technology , because in the public network environment , it is impossible to do one-to-one socket binding , so the use of the server as a medium , the client will send the message to the server , forwarded to the target server . Because in the public network environment, it is impossible to do one-to-one socket binding, so the use of the server as a medium, the client will send the message to the server, the server forwarded to the target.
	The client will also open a listening thread to receive messages from the server, through the CallBack for classification and display. The principle of the chat room is that there is a user with UID ‘HOST’, and the server detects the message sent to HOST, it will forward the message to all users except the sender, this is the principle of the chat room implementation.
	In the chat message storage, through a structure called Bucket for storage, when the user sends a message will be stored in the Bucket a Record object, this object stores the message sender, the message receiver and the message body, and listening to the server thread to receive the Message message, will also create a Record into the Bucket (in order to prevent the server from sending messages to the HOST). Bucket ( in order to prevent multi-threaded access conflicts, with lock lock object ), so the main page, bound to the event [SelectionChanged], when the event is triggered, the ChatPeople field will be changed to update the ChatPeople.QUid to the Bucket to look for chat records belonging to it, and then load all the chat records into the List. QUid goes to Bucket to find the chat records belonging to it, and then loads all these chat records into ListView, and there is a ‘Clear Cache’ button, the principle is to clear the records of Bucket about the chat object.
	The search function, consider the use of key parameters on the webapi request, in the server processing, for the parameter key, in the TBUser table filtering, and if the filtering is successful, it will constitute a collection of return. This also applies to the search for friends.
	Add a friend is also a difficult point in the client 1 to add the client 2 as a friend, if the client 2 is online at the moment, then there will be no client 2 of the client 1 of the friends of this situation, and if the client 1 to send BaseModel [AddBody] message to the client 2, will be slightly troublesome, so in the design of the time, in the User / AddRelation interface, the server directly to the client to send a request, so that proactive message notification will be better!

## How to use it
+ First run WeChat.WebApi project, in the runtime will automatically create a SQLite database (if necessary, you can configure their own SQL connection string to use other types of relational databases, such as SQL SERVER, MY SQL)
+ Then run the WeChat.WPF project, remember to create an account first, due to the removal of the mobile phone verification function, all the verification code for the unity of 00000000
