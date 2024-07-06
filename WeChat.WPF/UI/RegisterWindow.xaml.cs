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
    /// RegisterWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }
        private UserHelper UserHelper = new UserHelper();
        public User RegisterUser = null;
        private async void Lab_SendCode_Click(object sender, RoutedEventArgs e)
        {
            string phone = TB_Uid.Text;
            string pwd = TB_Password.Password;
            string name = TB_Name.Text;
            var result = await UserHelper.Register(phone, pwd, name);
            if(result)
            {
                MessageBox.Show("已发送验证码,主要查收", "通知", MessageBoxButton.OK, MessageBoxImage.Information);
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
            if(result == null)
            {
                MessageBox.Show("验证码不存在或手机号不匹配", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                //注册成功
                RegisterUser = result;
                this.Close();
            }
        }
    }
}
