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
    /// ResetWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ResetWindow : Window
    {
        public ResetWindow()
        {
            InitializeComponent();
        }
        private UserHelper UserHelper = new UserHelper();
        private async void Lab_SendCode_Click(object sender, RoutedEventArgs e)
        {
            string uid = TB_Uid.Text;
            string pwd = TB_Password.Password;
            var result = await UserHelper.Reset(uid, pwd);
            if (result)
            {
                MessageBox.Show("已发送验证码,注意查收", "通知", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("请不要频繁发送验证码,五分钟后再试", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BT_Login_Click(object sender, RoutedEventArgs e)
        {
            var code = TB_VerfiyCode.Text;
            string phone = TB_Uid.Text;
            if (code == "" || phone == "")
            {
                MessageBox.Show("验证码和手机号不能为空", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            var result = await UserHelper.Verfiy(phone, code);
            if (result == null)
            {
                MessageBox.Show("验证码不存在或手机号不匹配", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                this.Close();
            }
        }
    }
}
