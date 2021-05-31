using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Button = System.Windows.Controls.Button;

namespace Management_System
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        #region Xử lý hiệu ứng
        /// <summary>
        /// Hiệu ứng khi rê chuột vào avatar
        /// </summary>
        /// <param name="sender"></param>
        private void imgAvatar_MouseMove(object sender, MouseEventArgs e)
        {
            avatarBorder.Effect = new DropShadowEffect()
            {
                Color = Colors.Green,
                Direction = 270,
                BlurRadius = 20,
                ShadowDepth = 1
            };
        }

        /// <summary>
        /// Hiệu ứng khi ra chuột ra khỏi avatar
        /// </summary>
        private void imgAvatar_MouseLeave(object sender, MouseEventArgs e)
        {
            avatarBorder.Effect = new DropShadowEffect()
            {
                Color = Colors.Green,
                Direction = 270,
                BlurRadius = 20,
                ShadowDepth = 5
            };
        }
        #endregion

        /// <summary>
        /// Sự kiện khi click vào button Đăng nhập
        /// </summary>
        private void btnSignIn_Click(object sender, RoutedEventArgs e)
        {
            // Kiểm tra nếu chưa nhập username hay password
            if (editUsername.Text.Length == 0 && editPassword.Password.Length == 0)
            {
                // Hiện thông báo lỗi
                var dialog = new Dialog() { Message = "Vui lòng nhập tài khoản và mật khẩu" };
                dialog.Owner = Window.GetWindow(this);
                dialog.ShowDialog();
                return;
            }
            else if (editUsername.Text.Length == 0)
            {
                // Hiện thông báo lỗi
                var dialog = new Dialog() { Message = "Vui lòng nhập tài khoản" };
                dialog.Owner = Window.GetWindow(this);
                dialog.ShowDialog();
                return;
            }
            else if (editPassword.Password.Length == 0)
            {
                // Hiện thông báo lỗi
                var dialog = new Dialog() { Message = "Vui lòng nhập mật khẩu" };
                dialog.Owner = Window.GetWindow(this);
                dialog.ShowDialog();
                return;
            }

            // Nếu thông tin tài khoản nhập vào sai
            if (!editUsername.Text.Equals("admin") || !editPassword.Password.Equals("admin"))
            {
                // Hiện thông báo lỗi
                var dialog = new Dialog() { Message = "Tài khoản hoặc mật khẩu không chính xác" };
                dialog.Owner = Window.GetWindow(this);
                dialog.ShowDialog();
                return;
            }

            // Nếu đăng nhập thành công, ẩn màn hình login
            Dashboard dashboard = new Dashboard();
            dashboard.Left = Application.Current.MainWindow.Left;
            dashboard.Top = Application.Current.MainWindow.Top;
            dashboard.Show();
            this.Close();
        }

        /// <summary>
        /// Sự kiện khi ấn Enter khi đang ở 2 TextBox
        /// </summary>
        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                btnSignIn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }

        /// <summary>
        /// Sự kiện khi ấn các button command
        /// </summary>
        public void btnCommands_Click(object sender, RoutedEventArgs e)
        {
            Button curButton = sender as Button;
            if (curButton.Tag.Equals("btnClose")) {
                this.Close();
            }
            else if (curButton.Tag.Equals("btnMinim")) {
                this.WindowState = WindowState.Minimized;
            }
            else if (curButton.Tag.Equals("btnMaxim")) {
                if (this.WindowState == WindowState.Maximized) {
                    this.WindowState = WindowState.Normal;
                }
                else {
                    this.WindowState = WindowState.Maximized;
                }
            }
        }

        /// <summary>
        /// Sự kiện di chuyển cửa sổ
        /// </summary>
        private void ColorZone_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
