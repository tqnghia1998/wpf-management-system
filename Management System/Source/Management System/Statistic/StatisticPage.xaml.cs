using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public partial class StatisticPage : Page
    {
        /// <summary>
        /// ViewModel cho biểu đồ
        /// </summary>
        public class ProductTypeStatistic
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public int NumOfProduct { get; set; }
            public int Density { get; set; }
            public double Sold { get; set; }
        }
        
        // Ngày bắt đầu và ngày kết thúc
        DateTime dayBegin = new DateTime(2015, 1, 1);
        DateTime dayEnd = new DateTime(2025, 12, 31);

        public StatisticPage()
        {
            InitializeComponent();

            // Thiết lập các Combobox
            editYear.ItemsSource = editYearEnd.ItemsSource = new string[] {
                "2015", "2016", "2017", "2018", "2019", "2020", "2021", "2022", "2023", "2023", "2024", "2025" };
            editQuarter.ItemsSource = new string[] {
                "Tất cả", "I", "II", "III", "IV"
            };
            editWeek.ItemsSource = new string[] { "Tất cả" };
            editMonth.ItemsSource = new string[] {
                "Tất cả", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12"
            };
            editDay.ItemsSource = new string[] {
                "Tất cả", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
                "11", "12", "13", "14", "15", "16", "17", "18", "19", "20",
                "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31"
            };
            editDay.SelectedIndex = 0;
            editWeek.SelectedIndex = 0;
            editMonth.SelectedIndex = 0;
            editQuarter.SelectedIndex = 0;

            // Thiết lập thời gian mặc định
            checkManyYear.IsChecked = true;
            editYear.SelectedIndex = 0;
            editYearEnd.SelectedIndex = 11;

            refreshProductTypeStatistic();
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
        /// Xử lý CheckBox
        /// </summary>
        private void CheckManyYear_Checked(object sender, RoutedEventArgs e)
        {
            // Nếu tắt đi => Chọn ngày, tháng, quý
            if (checkManyYear.IsChecked == false)
            {
                editYearEnd.Visibility = Visibility.Hidden;
                editDay.Visibility = editMonth.Visibility = editQuarter.Visibility = editWeek.Visibility = Visibility.Visible;
            }
            // Nếu bật lên => Chọn nhiều năm
            else
            {
                editYearEnd.Visibility = Visibility.Visible;
                editDay.Visibility = editMonth.Visibility = editQuarter.Visibility = editWeek.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// Làm mới các biểu đồ
        /// </summary>
        public void refreshProductTypeStatistic()
        {
            // Trước tiên lấy thông tin lọc của người dùng
            GetDuration();

            // Sau đó chạy nền lấy dữ liệu và hiển thị
            Thread thread = new Thread(delegate ()
            {
                // Lấy CSDL
                var db = new Management_SystemEntities();
                var Bills = new ObservableCollection<Bill>(db.Bills.Where(x => dayBegin <= x.Date && x.Date <= dayEnd));
                var Products = new ObservableCollection<Product>(db.Products);
                var ProductTypes = new ObservableCollection<ProductType>(db.ProductTypes);

                // ViewModels để hiển thị lên Chart
                ObservableCollection<ProductTypeStatistic> viewModels = new ObservableCollection<ProductTypeStatistic>();

                // Tính tổng số sản phẩm bán được (của tất cả các loại)
                int totalSell = 0;
                try
                {
                    totalSell = db.Bills.Sum(x => x.Number) + db.Bills.Sum(x => x.NumberGiven);
                }
                catch (Exception) { }

                // Xét từng loại sản phẩm
                for (int i = 0; i < ProductTypes.Count; i++)
                {
                    try
                    {
                        // Liệt kê tất cả sản phẩm thuộc loại đang xét (chỉ trích lấy mã)
                        var ProductTypeId = ProductTypes[i].Id;
                        var productIds = Products.Where(x => x.ProductType == ProductTypeId).Select(x => x.Id);

                        // Xét tất cả hóa đơn có mã SP thuộc danh sách trên
                        var neededBills = Bills.Where(x => productIds.Contains(x.ProductId));

                        // Tính tổng sản phẩm & tổng số tiền trong các hóa đơn trên
                        int sumProduct = neededBills.Sum(x => x.Number) + neededBills.Sum(x => x.NumberGiven);
                        double sumMoneySold = neededBills.Sum(x => x.FinalPrice);

                        // Tạo đối tượng ProductTypeStatistic => Đưa vào danh sách View Models
                        viewModels.Add(new ProductTypeStatistic()
                        {
                            Id = ProductTypes[i].Id,
                            Name = ProductTypes[i].Name,
                            NumOfProduct = ProductTypes[i].NumOfProduct,
                            Density = sumProduct * 100 / totalSell,
                            Sold = sumMoneySold
                        });
                    }
                    catch (Exception) { }
                }

                // Đưa lên UI
                Dispatcher.Invoke(() =>
                {
                    columnChart1.ItemsSource = viewModels;
                    columnChart2.ItemsSource = viewModels;
                    pieChart1.ItemsSource = viewModels;
                    ProgressBar.IsEnabled = false;
                    ProgressBar.Visibility = Visibility.Hidden;
                });
            });
            thread.Start();
        }

        /// <summary>
        /// Lấy lại ngày bắt đầu và ngày kết thúc
        /// </summary>
        public void GetDuration()
        {
            // Nếu lọc theo thời gian tùy chọn của người dùng
            if (rdoOption.IsChecked == true)
            {
                // Kiểm tra 2 DatePicker có trống không
                if (editFromDate.Text.Length == 0 || editToDate.Text.Length == 0)
                {
                    var dialog = new Dialog() { Message = "Vui lòng nhập đầy đủ thông tin" };
                    dialog.Owner = Window.GetWindow(this);
                    dialog.ShowDialog();
                    return;
                }

                dayBegin = (DateTime)editFromDate.SelectedDate;
                dayEnd = (DateTime)editToDate.SelectedDate;
            }
            // Nếu lọc theo thời gian cụ thể
            else
            {
                // Nếu lọc nhiều năm
                if (checkManyYear.IsChecked == true)
                {
                    // Kiểm tra dữ liệu có trống không
                    if (editYear.Text.Length == 0 || editYearEnd.Text.Length == 0)
                    {
                        var dialog = new Dialog() { Message = "Vui lòng nhập đầy đủ thông tin" };
                        dialog.Owner = Window.GetWindow(this);
                        dialog.ShowDialog();
                        return;
                    }

                    // Kiểm tra năm cuối có trước năm đầu không
                    if (string.Compare(editYear.Text, editYearEnd.Text, true) > 0)
                    {
                        var dialog = new Dialog() { Message = "Khoảng thời gian không hợp lệ" };
                        dialog.Owner = Window.GetWindow(this);
                        dialog.ShowDialog();
                        return;
                    }

                    dayBegin = new DateTime(int.Parse(editYear.SelectedItem as string), 1, 1);
                    dayEnd = new DateTime(int.Parse(editYearEnd.SelectedItem as string), 12, 31);
                }
                else
                {
                    try
                    {
                        // Nếu không lọc theo ngày
                        if (editDay.SelectedIndex == 0)
                        {
                            // Nếu không lọc theo tháng
                            if (editMonth.SelectedIndex == 0)
                            {
                                // ĐẾN ĐÂY THÌ CÓ THỂ USER ĐANG LỌC THEO QUÝ HOẶC THEO TUẦN
                                if (editQuarter.SelectedIndex != 0) // Nếu lọc theo quý
                                {
                                    int monStart = editQuarter.SelectedIndex == 1 ? 1 : editQuarter.SelectedIndex == 2 ? 4 : editQuarter.SelectedIndex == 3 ? 7 : 10;
                                    dayBegin = new DateTime(int.Parse(editYear.SelectedItem as string), monStart, 1);
                                    int numOfDay = DateTime.DaysInMonth(int.Parse(editYear.SelectedItem as string), monStart + 2);
                                    dayEnd = new DateTime(int.Parse(editYear.SelectedItem as string), monStart + 2, numOfDay);
                                }
                                else if (editWeek.SelectedIndex != 0) // Nếu lọc theo tuần
                                {
                                    string str = editWeek.SelectedItem as string;
                                    int pos1 = str.LastIndexOf(' ');
                                    int pos2 = str.LastIndexOf('/');
                                    int Day = int.Parse(str.Substring(pos1 + 1, pos2 - pos1 - 1));
                                    int Month = int.Parse(str.Substring(pos2 + 1));
                                    dayBegin = new DateTime(int.Parse(editYear.SelectedItem as string), Month, Day);
                                    dayEnd = dayBegin.AddDays(6);
                                }
                                else // Nếu tất cả các Combobox đều "Tất cả" => Lọc theo năm
                                {
                                    dayBegin = new DateTime(int.Parse(editYear.SelectedItem as string), 1, 1);
                                    dayEnd = new DateTime(int.Parse(editYear.SelectedItem as string), 12, 31);
                                }
                            }
                            else // Nếu lọc theo tháng
                            {
                                dayBegin = new DateTime(int.Parse(editYear.SelectedItem as string), int.Parse(editMonth.SelectedItem as string), 1);            // Ngày đầu của tháng luôn là 1
                                int numOfDay = DateTime.DaysInMonth(int.Parse(editYear.SelectedItem as string), int.Parse(editMonth.SelectedItem as string));   // Tính số ngày của tháng
                                dayEnd = new DateTime(int.Parse(editYear.SelectedItem as string), int.Parse(editMonth.SelectedItem as string), numOfDay);       // Ngày cuối của tháng
                            }
                        }
                        else // Nếu lọc theo ngày
                        {
                            dayBegin = dayEnd = new DateTime(int.Parse(editYear.SelectedItem as string), int.Parse(editMonth.SelectedItem as string), int.Parse(editDay.SelectedItem as string));
                        }
                    }
                    catch (Exception)
                    {
                        var dialog = new Dialog() { Message = "Khoảng thời gian không hợp lệ" };
                        dialog.Owner = Window.GetWindow(this);
                        dialog.ShowDialog();
                        return;
                    }
                }
            }

            // Cập nhật lên UI
            if (dayBegin == dayEnd)
            {
                Duration.Content = "Trong ngày " + dayBegin.ToShortDateString();
            }
            else Duration.Content = "Từ ngày " + dayBegin.ToShortDateString() + " đến ngày " + dayEnd.ToShortDateString();
        }

        /// <summary>
        /// Nhấn vào button Làm mới
        /// </summary>
        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            refreshProductTypeStatistic();
        }

        #region XỬ LÝ TƯƠNG TÁC GIỮA CÁC COMBOBOX
        /// <summary>
        /// Chọn Combobox Quý
        /// </summary>
        private void EditQuarter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (editQuarter.SelectedIndex != 0)
            {
                editWeek.SelectedIndex = editMonth.SelectedIndex = editDay.SelectedIndex = 0; // Nếu chọn quý rồi => Không chọn tháng, ngày, tuần
            }
        }

        /// <summary>
        /// Chọn Combobox Tháng
        /// </summary>
        private void EditMonth_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (editMonth.SelectedIndex != 0)
            {
                editWeek.SelectedIndex = editQuarter.SelectedIndex = 0; // Nếu chọn tháng rồi => Không chọn quý, tuần
            }
            else
            {
                editDay.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Chọn Combobox Ngày
        /// </summary>
        private void EditDay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Phải chọn tháng trước ngày
            if (editMonth.SelectedIndex == 0 && editDay.SelectedIndex != 0)
            {
                editDay.SelectedIndex = 0;
                var dialog = new Dialog() { Message = "Vui lòng chọn tháng trước" };
                dialog.Owner = Window.GetWindow(this);
                dialog.ShowDialog();
            }
        }

        /// <summary>
        /// Chọn Combobox Năm => Phát sinh Combobox Tuần
        /// </summary>
        private void EditYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Lấy số năm và khởi tạo danh sách tuần
            if (editYear == null || editYear.Text.Length == 0) return;
            int Year = int.Parse(editYear.SelectedItem as string);
            ObservableCollection<string> weeks = new ObservableCollection<string>();
            weeks.Add("Tất cả");
            
            // Xét tất cả thứ 2 của từng tháng, mỗi thứ 2 là bắt đầu của một tuần
            for (int i = 1; i <= 12; i++)
            {
                var db = Enumerable.Range(1, DateTime.DaysInMonth(Year, i))
                    .Where(d => new DateTime(Year, i, d).ToString("dddd").Equals("Monday"))
                    .Select(d => new DateTime(Year, i, d)).ToList();

                for (int j = 0; j < db.Count; j++)
                {
                    weeks.Add((weeks.Count).ToString() + " - " + db[j].ToString("dd/MM"));
                }
            }
            editWeek.ItemsSource = weeks;
            editWeek.SelectedIndex = 0;
        }

        /// <summary>
        /// Chọn Combobox Tuần
        /// </summary>
        private void EditWeek_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (editWeek.SelectedIndex != 0)
            {
                editQuarter.SelectedIndex = editMonth.SelectedIndex = editDay.SelectedIndex = 0; // Nếu chọn tuần rồi => Không chọn tháng, ngày, quý
            }
        }
        #endregion

        /// <summary>
        /// Xem chi tiết doanh thu của một loại sản phẩm
        /// </summary>
        private void ClusteredColumnChart_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ProductTypeStatistic selected = myChart.SelectedItem as ProductTypeStatistic;
            NavigationService.Navigate(new StatisticProduct(dayBegin, dayEnd, selected.Id, selected.Name));
        }

        /// <summary>
        /// Xem thống kê doanh thu
        /// </summary>
        private void ProfitStatis_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new StatisticProfit());
        }
    }
}
