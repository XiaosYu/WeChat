using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WeChat.WPF.Models.Chat;

namespace WeChat.WPF.UI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(User user)
        {
            InitializeComponent();
            User = user;
        }

        private User User { get; set; }
        private FriendHelper Friend { get; set; }
        private People ChatPeople { set; get; }
        private Bucket Bucket = new Bucket();
        private List<People> Peoples = new List<People>();
        private List<People> Shows = new List<People>();
        private UNameCollection UNames;
        private OnlineHelper Helper { set; get; }
        private ClientWebSocket ClientSocket = new ClientWebSocket();
        private string Uid { set; get; }

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
        public async Task<bool> SendMessage(string point, string message)
        {
            //先验证是否目标是否在线
            if (point != "HOST")
            {
                if (!(await Helper.IsOnline(point)))
                {
                    return false;
                }
            }

            var model = new BaseModel<MessageBody>();
            model.Action = "Message";
            model.Body = new MessageBody() { Message = message };
            model.Guid = Tools.Random(10);
            model.Sender = Uid;
            model.Receiever = point;
            model.Verfiy = "swu";

            var buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(model));

            
            await ClientSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);

            Record record;
            //自己的消息入桶
            if(ChatPeople.Quid == "HOST")
            {
                record = new()
                {
                    Sender = ChatPeople.Quid,
                    Receiever = Uid,
                    DateTime = DateTime.Now,
                    Message = TB_Message.Text
                };
            }
            else
            {
                record = new()
                {
                    Sender = Uid,
                    Receiever = ChatPeople.Quid,
                    DateTime = DateTime.Now,
                    Message = TB_Message.Text
                };           
            }
            lock(Bucket)
            {
                Bucket.Put(record);
            }
            ShowMessage(record);          
            return true;
        }

        public async void Close()
        {
            await ClientSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
        }


        private void TB_Search_KeyDown(object sender, KeyEventArgs e)
        {
            //回车搜索
            if(e.Key == Key.Enter)
            {
                Updata();
            }
        }

        private async void PB_AddFriend_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RelationWindow window = new RelationWindow(Uid);
            window.ShowDialog();
            //更新好友列表
            //查询好友在线情况
            LB_Friend.Items.Clear();
            Peoples.Clear();
            Shows.Clear();
            
            var result = await Friend.ListFriends();
            if (result == null)
            {
                MessageBox.Show("好友加载出错,请重启");
            }
            else
            {
                Peoples.Add(new People { Quid = "HOST", Name = "幸福一家人", Sign = "这是幸福的一家人" });
                Peoples.AddRange(result.Data);
                Shows.AddRange(Peoples);
                Shows.ForEach(s => LB_Friend.Items.Add(s.Name));
            }
            //选中Host
            ChatPeople = Shows[0];
        }

        private async void BT_Send_Click(object sender, RoutedEventArgs e)
        {
            var result = await SendMessage(ChatPeople.Quid, TB_Message.Text);
            if (!result)
            {
                MessageBox.Show("用户未在线或者发送失败","失败",MessageBoxButton.OK,MessageBoxImage.Error);
                return;
            }
            TB_Message.Text = "";
                
        }

        private async void Window_LoadedAsync(object sender, RoutedEventArgs e)
        {
            //初始化
            Uid = User.Uid;
            Helper = new OnlineHelper(Uid);
            Friend = new FriendHelper(User.Uid);
            Lab_UserName.Content = User.Name;
            TB_Sign.Text = User.Sign;
            Connect();
            //载入用户名称
            UNames = await new UserHelper().GetUserNames(Uid);
            if(UNames == null)
            {
                MessageBox.Show("载入内部数据出错");
            }
            //查询好友在线情况
            var result = await Friend.ListFriends();
            if (result == null)
            {
                MessageBox.Show("好友加载出错,请重启");
            }
            else
            {
                Peoples.Add(new People { Quid = "HOST", Name = "幸福一家人",Sign = "这是幸福的一家人" });
                Peoples.AddRange(result.Data);

                Shows.AddRange(Peoples);
                Shows.ForEach(s => LB_Friend.Items.Add(s.Name));
                LB_Friend.SelectedIndex = 0;
            }
            //选中Host
            ChatPeople = Shows[0];


        }

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
        private async void Updata()
        {
            if (TB_Search.Text == "")
            {
                //显示所有好友
                Shows.Clear();
                Shows.AddRange(Peoples);
                LB_Friend.Items.Clear();
                Shows.ForEach(s => LB_Friend.Items.Add(s.Name));
                LB_Friend.SelectedIndex = -1;
                return;
            }
            var result = await Friend.SearchFriend(TB_Search.Text);
            if (result == null) return;
            Shows.Clear();
            Shows.AddRange(result.Data);
            LB_Friend.Items.Clear();
            Shows.ForEach(s => LB_Friend.Items.Add(s.Name));
            LB_Friend.SelectedIndex = -1;
        }
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

        private async void LB_Friend_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LB_Friend.SelectedIndex == -1) return;
            //选择了某个对象
            var user = Shows[LB_Friend.SelectedIndex];
            if (user == null) return;
            
            ChatPeople = user;
            //更新显示
            Lab_ChatName.Content = ChatPeople.Name;
            TB_ChatSign.Text = ChatPeople.Sign;
            if (user.Quid != "HOST")
            {
                //查询该用户是否在线
                var result = await Helper.IsOnline(ChatPeople.Quid);
                if (!result)
                {
                    Lab_ChatName.Content += "    未在线";
                }
                else
                {
                    Lab_ChatName.Content += "    在线";
                }
            }
            //清空聊天信息板
            LV_Message.Items.Clear();
            TB_Message.Text = "";
            //查找Bucket是否有待加入的信息
            var items = Bucket.GetUserMessage(Uid, user.Quid);
            items.ForEach(s => ShowMessage(s));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Close();
        }

        private async void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (LB_Friend.SelectedIndex == -1) return;
            var chat = Shows[LB_Friend.SelectedIndex];
            if (chat == null) return;
            if (chat.Quid == "HOST") return;

            var result = MessageBox.Show("是否删除所选好友", "删除好友", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if(result == MessageBoxResult.Yes)
            {
                var data = await Friend.DeleteRelation(chat.Quid);
                if(data)
                {
                    Peoples.Remove(chat);
                    LB_Friend.Items.RemoveAt(LB_Friend.SelectedIndex);
                    LB_Friend.SelectedIndex = 0;
                    Shows.Remove(chat);
                }
               else
                {
                    MessageBox.Show("遇到错误,请重试");
                }
            }

        }

        private void TB_Message_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                BT_Send_Click(sender,e);
            }
        }

        private void BT_Clear_Click(object sender, RoutedEventArgs e)
        {
            if (ChatPeople == null || LB_Friend.SelectedIndex == -1) return;
            var result = MessageBox.Show("是否删除该用户的聊天记录","删除聊天记录",MessageBoxButton.YesNo,MessageBoxImage.Question);
            if(result == MessageBoxResult.Yes)
            {
                Bucket.Clear(Uid, ChatPeople.Quid);
                LV_Message.Items.Clear();
            }
        }

        private void BT_ClearText_Click(object sender, RoutedEventArgs e)
        {
            TB_Message.Text = "";
        }


        private void TB_Sign_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ClickCount == 2)
            {
                TB_H_Sign.Visibility = Visibility.Visible;
                TB_Sign.Visibility = Visibility.Hidden;
                TB_H_Sign.Text = TB_Sign.Text;
            }
        }

        private async void TB_H_Sign_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                var result = MessageBox.Show("是否修改个性签名", "确认", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    var sign = TB_H_Sign.Text;
                    UserHelper helper = new();
                    var data = await helper.Modify(Uid, User.Name, sign);
                    User.Sign = sign;
                    if (!data) MessageBox.Show("修改错误");
                    else
                    {
                        TB_Sign.Text = sign;
                    }

                    TB_H_Sign.Visibility = Visibility.Hidden;
                    TB_H_Sign.Text = "";
                    TB_Sign.Visibility = Visibility.Visible;


                }
            }
        }

        private void Lab_UserName_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Lab_H_UserName.Visibility = Visibility.Visible;
            Lab_UserName.Visibility = Visibility.Hidden;
            Lab_H_UserName.Text = (string)Lab_UserName.Content;
        }

        private async void Lab_H_UserName_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                var result = MessageBox.Show("是否修改名称", "确认", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if(result == MessageBoxResult.Yes)
                {
                    var name = Lab_H_UserName.Text;
                    UserHelper helper = new();
                    var data = await helper.Modify(Uid, name, User.Sign);
                    User.Name = name;
                    if (!data) MessageBox.Show("名称修改错误");
                    else
                    {
                        Lab_UserName.Content = name;
                    }

                    Lab_UserName.Visibility = Visibility.Visible;
                    Lab_H_UserName.Visibility= Visibility.Hidden;
                }
            }
        }
    }
}
