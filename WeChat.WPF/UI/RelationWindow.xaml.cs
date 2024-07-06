using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WeChat.WPF.UI
{
    /// <summary>
    /// RelationWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RelationWindow : Window
    {
        public RelationWindow(string uid)
        {
            InitializeComponent();
            Helper = new FriendHelper(uid);
        }

        private FriendHelper Helper;
        private List<People> Peoples =new List<People>();
        private People Select;
        private async void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (TB_Key.Text == "") return;
            //搜索
            LB_User.Items.Clear();
            var result = await Helper.SearchUser(TB_Key.Text);
            var friends = (await Helper.ListFriends()).Data.Select(s => s.Quid).ToList();
            //不能加自己为好友
            var list = result.Data.Where(s => !friends.Contains(s.Quid)).Where(s => s.Quid != Uid).ToList();
            Peoples.AddRange(list);
            if(list != null)
            {
                list.ForEach(s => LB_User.Items.Add(s.Name));
            }
        }

        private async void TB_Add_Click(object sender, RoutedEventArgs e)
        {
            if (Select == null) return;
            var result = MessageBox.Show("是否添加好友", "确认", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if(result == MessageBoxResult.Yes)
            {
                var data = await Helper.CheckRelation(Select.Quid);
                if(data == false)
                {
                    var state = await Helper.AddRelation(Select.Quid);
                    if (!state)
                    {
                        MessageBox.Show("添加错误,请联系管理员");
                        return;
                    }
                    else
                    {
                        //添加完成
                        Peoples.Remove(Select);
                        LB_User.Items.Remove(Select.Name);
                        Select = null;
                        LB_User.SelectedIndex = -1;
                        TB_UserMessage.Text = "";

                    }
                }
                else
                {
                    MessageBox.Show("你们已经是好友了");
                }
                
            }
        }

        private void LB_User_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LB_User.SelectedIndex == -1) return;
            Select = Peoples[LB_User.SelectedIndex];
            //更新显示
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"名称:{Select.Name}");
            sb.AppendLine($"个性签名:{Select.Sign}");
            sb.AppendLine($"账号:{Select.Quid}");
            sb.AppendLine($"关联邮箱:{Select.Email}");
            sb.AppendLine($"关联手机:{Select.Phone}");
            TB_UserMessage.Text = sb.ToString();
        }
    }
}
