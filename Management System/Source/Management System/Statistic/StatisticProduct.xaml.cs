using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Management_System.Statistic
{
    public partial class StatisticProduct : Page
    {
        /// <summary>
        /// ViewModel cho biểu đồ
        /// </summary>
        public class ProductStatistic
        {
            public string Name { get; set; }
            public int NumSold { get; set; }
            public double Sold { get; set; }
        }

        public StatisticProduct(DateTime dayBegin, DateTime dayEnd, string productTypeId, string name)
        {
            InitializeComponent();
            txtTitle.Content = name.ToUpper() + " TỪ " + dayBegin.ToShortDateString() + " ĐẾN " + dayEnd.ToShortDateString();

            // Chạy nền lấy dữ liệu và hiển thị
            Thread thread = new Thread(delegate ()
            {
                // Lấy CSDL
                var db = new Management_SystemEntities();

                // Lấy hóa đơn trong khoảng thời gian đã cho
                var Bills = new ObservableCollection<Bill>(db.Bills.Where(x => dayBegin <= x.Date && x.Date <= dayEnd));

                // Lấy sản phẩm thuộc loại đã cho
                var Products = new ObservableCollection<Product>(db.Products.Where(x => x.ProductType == productTypeId));

                // ViewModels để hiển thị lên Chart
                ObservableCollection<ProductStatistic> viewModels = new ObservableCollection<ProductStatistic>();

                // Duyệt từng sản phẩm của loại
                for (int i = 0; i < Products.Count; i++)
                {
                    try
                    {
                        // Xét tất cả hóa đơn có mã SP là sản phẩm đang xét
                        string ProductId = Products[i].Id;
                        var neededBills = Bills.Where(x => x.ProductId == ProductId);

                        // Tính tổng số lượng & tổng số tiền trong các hóa đơn trên
                        int numSold = neededBills.Sum(x => x.Number) + neededBills.Sum(x => x.NumberGiven);
                        double moneySold = neededBills.Sum(x => x.FinalPrice);

                        // Tạo đối tượng ProductTypeStatistic => Đưa vào danh sách View Models
                        viewModels.Add(new ProductStatistic()
                        {
                            Name = Products[i].Name,
                            NumSold = numSold,
                            Sold = moneySold
                        });
                    }
                    catch (Exception) { }
                }

                // Đưa lên UI
                Dispatcher.Invoke(() =>
                {
                    columnChart1.ItemsSource = viewModels;
                    columnChart2.ItemsSource = viewModels;
                    ProgressBar.IsEnabled = false;
                    ProgressBar.Visibility = Visibility.Hidden;
                });
            });
            thread.Start();
        }
    }
}
