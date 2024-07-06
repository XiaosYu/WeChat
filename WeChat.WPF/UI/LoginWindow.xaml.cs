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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WeChat.WPF.UI
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private UserHelper UserHelper = new UserHelper();
        private User LoginUser = null;

        private async void BT_Login_Click(object sender, RoutedEventArgs e)
        {
            string uid = TB_Uid.Text;
            string pwd = TB_Password.Password;
            var user = await UserHelper.Login(uid,pwd);
            if(user == null)
            {
                MessageBox.Show("密码错误或者账号不存在", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                //进入主界面
                //user为登录账号
                LoginUser = user;
                MainWindow window = new MainWindow(LoginUser);
                window.Show();
                this.Close();
            }
        }

        private void Lab_Regsiter_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow window = new RegisterWindow();
            window.ShowDialog();
            LoginUser = window.RegisterUser;  
            if(LoginUser != null)
            {
                TB_Uid.Text = LoginUser.Uid;
            }
        }

        private void Lab_Reset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ResetWindow window = new ResetWindow();
                window.ShowDialog();
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace);
            }
        }
    }
}
