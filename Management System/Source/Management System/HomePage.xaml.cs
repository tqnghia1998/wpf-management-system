using Management_System.Statistic;
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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Management_System
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
        }

        #region Xử lý hiệu ứng
        /// <summary>
        /// Hiệu ứng khi rê chuột vào hình
        /// </summary>
        private void img_MouseMove(object sender, MouseEventArgs e)
        {
            DropShadowEffect effect = new DropShadowEffect()
            {
                Color = Colors.DarkGray,
                Direction = 270,
                BlurRadius = 20,
                ShadowDepth = 10
            };

            Image img = sender as Image;
            if (img.Tag.Equals("imgListProduct"))
            {
                listProductBorder.Effect = effect;
            }
            else if (img.Tag.Equals("imgAddProduct"))
            {
                addProductBorder.Effect = effect;
            }
            else if (img.Tag.Equals("imgSellProduct"))
            {
                sellProductBorder.Effect = effect;
            }
            else if (img.Tag.Equals("imgStatistic"))
            {
                statisticBorder.Effect = effect;
            }
        }

        /// <summary>
        /// Hiệu ứng khi ra chuột ra khỏi hình
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void img_MouseLeave(object sender, MouseEventArgs e)
        {
            DropShadowEffect effect = new DropShadowEffect()
            {
                Color = Colors.DarkGray,
                Direction = 270,
                BlurRadius = 20,
                ShadowDepth = 5
            };

            Image img = sender as Image;
            if (img.Tag.Equals("imgListProduct"))
            {
                listProductBorder.Effect = effect;
            }
            else if (img.Tag.Equals("imgAddProduct"))
            {
                addProductBorder.Effect = effect;
            }
            else if (img.Tag.Equals("imgSellProduct"))
            {
                sellProductBorder.Effect = effect;
            }
            else if (img.Tag.Equals("imgStatistic"))
            {
                statisticBorder.Effect = effect;
            }
        }
        #endregion

        /// <summary>
        /// Chọn thêm sản phẩm
        /// </summary>
        private void NewProductPage_MouseEnter(object sender, MouseEventArgs e)
        {
            NavigationService.Navigate(new NewProductPage());
        }

        /// <summary>
        /// Chọn xem danh sách sản phẩm
        /// </summary>
        private void ProductPage_MouseEnter(object sender, MouseEventArgs e)
        {
            NavigationService.Navigate(new ProductPage());
        }

        /// <summary>
        /// Thêm đơn hàng mới
        /// </summary>
        private void BillPage_MouseUp(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new NewBillPage());
        }

        /// <summary>
        /// Thống kê dữ liệu
        /// </summary>
        private void StatisticPage_MouseUp(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new StatisticPage());
        }
    }
}
