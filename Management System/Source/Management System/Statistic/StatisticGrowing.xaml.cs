using InteractiveDataDisplay.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// <summary>
    /// Interaction logic for StatisticGrowing.xaml
    /// </summary>
    public partial class StatisticGrowing : Page
    {
        // Ngày nhập hàng đầu tiên
        public DateTime dayBeginning;

        // Doanh thu
        LineGraph line1 = new LineGraph();
        LineGraph line2 = new LineGraph();

        // Nếu gọi từ giao diện thống kê doanh thu
        public StatisticGrowing(Product _product)
        {
            InitializeComponent();

            // Đường doanh thu
            Lines.Children.Add(line1);
            line1.Stroke = new SolidColorBrush(Colors.Red);
            line1.Description = "Doanh thu";
            line1.StrokeThickness = 2;

            // Đường vốn
            Lines.Children.Add(line2);
            line2.Stroke = new SolidColorBrush(Colors.Blue);
            line2.Description = "Vốn bỏ ra";
            line2.StrokeThickness = 2;

            refresh(_product);
        }

        /// <summary>
        /// Thống kê tăng trưởng doanh thu
        /// </summary>
        public void refresh(Product product)
        {
            // Tìm ngày đầu tiên (ngày nhập hàng đầu tiên của tất cả mặt hàng)
            dayBeginning = DateTime.Now;

            // Lấy CSDL
            var db = new Management_SystemEntities();
            var Products = new ObservableCollection<Product>(db.Products);
            if (Products.Count > 0) dayBeginning = Products[0].Date;

            // Tìm ngày lâu nhất
            for (int i = 1; i < Products.Count; i++)
            {
                if (Products[i].Date < dayBeginning) dayBeginning = Products[i].Date;
            }

            // Tính khoảng cách từ ngày đó tới bây giờ
            int numOfDay = (DateTime.Now - dayBeginning).Days;
            if (numOfDay == 0) return;

            // Khởi tạo mảng ngày
            double[] days = new double[numOfDay];
            for (int i = 0; i < numOfDay; i++) days[i] = i + 1;

            // Khởi tạo mảng doanh thu
            double[] totalProfit = new double[numOfDay];
            for (int i = 0; i < numOfDay; i++)
            {
                // Lấy tất cả hóa đơn
                DateTime dayEnd = Index2Day(i + 1);

                // Nếu đang thống kê tất cả sản phẩm
                if (product == null)
                {
                    var bills = new ObservableCollection<Bill>(db.Bills.Where(x => dayBeginning <= x.Date && x.Date <= dayEnd));
                    totalProfit[i] = bills.Sum(x => x.FinalPrice);
                }
                // Nếu đang thống kê một sản phẩm cụ thể
                else
                {
                    var bills = new ObservableCollection<Bill>(db.Bills.Where(x => dayBeginning <= x.Date && x.Date <= dayEnd && x.ProductId == product.Id));
                    totalProfit[i] = bills.Sum(x => x.FinalPrice);
                }
            }

            // Khởi tạo mảng vốn
            double[] totalCapital = new double[numOfDay];
            for (int i = 0; i < numOfDay; i++)
            {
                // Lấy tất cả các sản phẩm nhập hàng trong khoảng thời gian dayBeginning -> Ngày thứ i + 1
                DateTime dayEnd = Index2Day(i + 1);

                // Nếu đang thống kê tất cả sản phẩm
                if (product == null)
                {
                    var products = new ObservableCollection<Product>(db.Products.Where(x => dayBeginning <= x.Date && x.Date <= dayEnd));
                    totalCapital[i] = products.Sum(x => x.Capital);
                }
                // Nếu đang thống kê một sản phẩm cụ thể
                else
                {
                    totalCapital[i] = product.Capital; // Đường thẳng
                }
            }

            // Vẽ đồ thị
            line1.Plot(days, totalProfit);
            line2.Plot(days, totalCapital);

            // Chú thích tổng doanh thu và tổng vốn
            editTotalProfit.Text = totalProfit[numOfDay - 1].ToString("N0");
            editTotalCapital.Text = totalCapital[numOfDay - 1].ToString("N0");
        }

        /// <summary>
        /// Xử lý ô giá chỉ nhận giá trị số
        /// </summary>
        private void NumberOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        /// <summary>
        /// Chuyển STT trên tục hoành thành ngày thực
        /// </summary>
        public DateTime Index2Day(int index)
        {
            return dayBeginning.AddDays(index - 1);
        }

        /// <summary>
        /// Chú thích ngày trong tục hoành
        /// </summary>
        private void EditSTT_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (editSTT.Text.Equals("0") || editSTT.Text.Length == 0)
            {
                editRealDay.Text = dayBeginning.ToShortDateString();
            }
            else
            {
                editRealDay.Text = Index2Day(int.Parse(editSTT.Text)).ToShortDateString();
            }
        }
    }
}