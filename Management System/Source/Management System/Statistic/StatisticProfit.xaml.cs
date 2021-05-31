using InteractiveDataDisplay.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Management_System.Statistic
{
    public partial class StatisticProfit : Page
    {
        // Doanh thu
        LineGraph line1 = new LineGraph();

        public StatisticProfit()
        {
            InitializeComponent();

            // Thiết lập các Combobox
            editYear.ItemsSource = new string[] {
                "2015", "2016", "2017", "2018", "2019", "2020", "2021", "2022", "2023", "2023", "2024", "2025" };
            editMonth.ItemsSource = new string[] {
                "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12"
            };
            editMonth.SelectedIndex = 0;
            editYear.SelectedIndex = 4;

            // Đường doanh thu
            Lines.Children.Add(line1);
            line1.Stroke = new SolidColorBrush(Colors.Red);
            line1.Description = "Doanh thu";
            line1.StrokeThickness = 2;

            RdoDayMonth_Checked(null, null);
        }

        #region Xử lý hiệu ứng Comboxbox
        /// <summary>
        /// Hiệu ứng khi chọn
        /// </summary>
        private void ComboProductTypes_DropDownOpened(object sender, EventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            combo.Background = Brushes.LightGray;
        }

        /// <summary>
        /// Hiệu ứng khi bỏ chọn
        /// </summary>
        private void ComboProductTypes_DropDownClosed(object sender, EventArgs e)
        {
            ComboBox combo = sender as ComboBox; combo.Focus();
            combo.Background = Brushes.Transparent;
        }
        #endregion

        /// <summary>
        /// Vẽ theo ngày trong tháng
        /// </summary>
        private void RdoDayMonth_Checked(object sender, RoutedEventArgs e)
        {
            // Chỉ thực hiện khi đã có dữ liệu
            if (editYear.SelectedIndex == -1 || editMonth.SelectedIndex == -1) return;

            // Lấy số ngày trong tháng
            int numOfDay = DateTime.DaysInMonth(int.Parse(editYear.SelectedItem as string), int.Parse(editMonth.SelectedItem as string));

            // Mảng ngày
            double[] months = new double[numOfDay];
            for (int i = 0; i < months.Length; i++) months[i] = i + 1;

            // Mảng doanh thu
            double[] profitByMonth = new double[numOfDay];

            for (int i = 0; i < numOfDay; i++)
            {
                // Lấy ngày đang xét
                DateTime day = new DateTime(int.Parse(editYear.SelectedItem as string), int.Parse(editMonth.SelectedItem as string), i + 1);

                // Lấy tất cả hóa đơn trong ngày
                var db = new Management_SystemEntities();
                var tmpBills = new ObservableCollection<Bill>(db.Bills);
                var Bills = new ObservableCollection<Bill>();
                for (int j = 0; j < tmpBills.Count; j++)
                {
                    if (tmpBills[j].Date.Year == day.Year
                        && tmpBills[j].Date.Month == day.Month
                        && tmpBills[j].Date.Day == day.Day) Bills.Add(tmpBills[j]);
                }

                // Tính số tiền thu được trong các Bill trên
                double y = Bills.Sum(x => x.FinalPrice);
                profitByMonth[i] = y;
            }
            Plotter.BottomTitle = "Ngày (tháng " + editMonth.Text + "/" + editYear.Text + ")";
            line1.Plot(months, profitByMonth);
        }

        /// <summary>
        /// Vẽ theo tháng trong năm
        /// </summary>
        private void RdoMonthYear_Click(object sender, RoutedEventArgs e)
        {
            // Mảng tháng
            double[] months = new double[12];
            for (int i = 0; i < months.Length; i++) months[i] = i + 1;

            // Mảng doanh thu
            double[] profitByMonth = new double[12];

            for (int i = 0; i < 12; i++)
            {
                // Lấy ngày đầu và ngày cuối của tháng
                DateTime dayBegin = new DateTime(int.Parse(editYear.SelectedItem as string), i + 1, 1);
                int numOfDay = DateTime.DaysInMonth(int.Parse(editYear.SelectedItem as string), i + 1);
                DateTime dayEnd = new DateTime(int.Parse(editYear.SelectedItem as string), i + 1, numOfDay);

                // Lấy tất cả hóa đơn trong tháng
                var db = new Management_SystemEntities();
                var Bills = new ObservableCollection<Bill>(db.Bills.Where(x => dayBegin <= x.Date && x.Date <= dayEnd));

                // Tính số tiền thu được trong các Bill trên
                profitByMonth[i] = Bills.Sum(x => x.FinalPrice);
            }
            Plotter.BottomTitle = "Tháng (năm " + editYear.Text + ")";
            line1.Plot(months, profitByMonth);
        }

        /// <summary>
        /// Xem chi tiết tăng trưởng doanh thu
        /// </summary>
        private void BtnGrow_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new StatisticGrowing(null));
        }
    }

    /// <summary>
    /// Chuyển đổi dữ liệu lên Chart
    /// </summary>
    public class VisibilityToCheckedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((Visibility)value) == Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((bool)value) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
