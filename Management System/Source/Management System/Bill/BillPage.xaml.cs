using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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

namespace Management_System
{
    /// <summary>
    /// Interaction logic for BillPage.xaml
    /// </summary>
    public partial class BillPage : Page
    {
        // Khởi tạo danh sách hóa đơn
        ObservableCollection<Bill> bills;
        Bill specificBill = null;

        public BillPage()
        {
            InitializeComponent();

            // Combobox
            comboFilter.ItemsSource = new string[] { "Xem tất cả đơn", "Xem đơn đã hoàn thành",
                        "Xem đơn chưa hoàn thành", "Xem đơn đã hủy" };
            comboFilter.SelectedIndex = 0;
        }

        /// <summary>
        /// Constructor dành cho ProductDetailPage
        /// </summary>
        public BillPage(Bill bill)
        {
            InitializeComponent();

            // Combobox
            comboFilter.ItemsSource = new string[] { "Xem tất cả đơn", "Xem đơn đã hoàn thành",
                        "Xem đơn chưa hoàn thành", "Xem đơn đã hủy" };
            comboFilter.SelectedIndex = 0;

            // Lưu bill cần chọn
            specificBill = bill;
        }

        #region Xử lý hiệu ứng Comboxbox
        /// <summary>
        /// Hiệu ứng khi chọn
        /// </summary>
        private void ComboFilter_DropDownOpened(object sender, EventArgs e)
        {
            comboFilter.Background = Brushes.LightGray;
        }

        /// <summary>
        /// Hiệu ứng khi bỏ chọn
        /// </summary>
        private void ComboFilter_DropDownClosed(object sender, EventArgs e)
        {
            comboFilter.Background = Brushes.Transparent;
        }
        #endregion

        /// <summary>
        /// Lọc theo tình trạng hóa đơn
        /// </summary>
        private void ComboFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get và hiển thị danh sách hóa đơn
            Thread getBills = new Thread(delegate ()
            {
                // Get và hiển thị danh sách hóa đơn
                var db = new Management_SystemEntities();

                // Chỉ lấy những Bill theo bộ lọc
                int filterIndex = 0;
                Dispatcher.Invoke(() => {
                   filterIndex = comboFilter.SelectedIndex;
                });
                switch (filterIndex)
                {
                    case 1: bills = new ObservableCollection<Bill>(db.Bills.Where(x => x.Status.Equals("Đã hoàn thành"))); break;
                    case 2: bills = new ObservableCollection<Bill>(db.Bills.Where(x => x.Status.Equals("Chưa hoàn thành"))); break;
                    case 3: bills = new ObservableCollection<Bill>(db.Bills.Where(x => x.Status.Equals("Đã hủy"))); break;
                    default: bills = new ObservableCollection<Bill>(db.Bills); break;
                }
                
                // Sắp xếp sao cho hóa đơn gần nhất nằm ở trên
                bills = new ObservableCollection<Bill>(bills.Reverse());

                // Đặt Item Source cho List View
                Dispatcher.Invoke(() => {
                    listBill.ItemsSource = bills;
                    ProgressBar.IsEnabled = false;
                    ProgressBar.Visibility = Visibility.Hidden;

                    // Nếu được Navigate từ ProductDetailPage (tức specificBill != null thì chọn nó)
                    if (specificBill != null)
                    {
                        for (int i = 0; i < listBill.Items.Count; i++)
                        {
                            if ((listBill.Items[i] as Bill).Date == specificBill.Date)
                            {
                                listBill.SelectedIndex = i;
                                ListBill_PreviewMouseLeftButtonUp(null, null); // Phải Invoke hàm này nữa
                                break;
                            }
                        }
                    }
                });
            });
            getBills.Start();
        }

        /// <summary>
        /// Chọn một Item trong List View
        /// </summary>
        private void ListBill_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Lấy đối tượng Bill tương ứng
            if (listBill.SelectedItem == null) return;
            Bill bill = listBill.SelectedItem as Bill;

            // Đưa thông tin vào các TextBox
            editAddress.Text = bill.Address == null ? "" : bill.Address;
            editDeposit.Text = bill.Deposit == 0 ? "" : ((double)(bill.Deposit)).ToString("N0");
            editShipCost.Text = bill.Ship == 0 ? "" : ((double)(bill.Ship)).ToString("N0");
            editMoneyWillGet.Text = bill.MoneyWillGet == 0 ? "" : ((double)(bill.MoneyWillGet)).ToString("N0");
            editNumber.Text = bill.Number == 0 ? "" : bill.Number.ToString();
            editNumberGiven.Text = bill.NumberGiven == 0 ? "" : bill.NumberGiven.ToString();
            editOriginalPrice.Text = ((double)(bill.OriginalPrice)).ToString("N0");
            editSellPrice.Text = ((double)(bill.FinalPrice)).ToString("N0");
            editPhone.Text = bill.Phone == null ? "" : bill.Phone;
            editEvent.Text = bill.Event == null ? "" : bill.Event;

            // Nếu hóa đơn chưa hoàn thành thì bật 2 button Hoàn thành và Hủy đơn
            btnIgnore.IsEnabled = btnComplete.IsEnabled = bill.Status.Equals("Chưa hoàn thành");
        }

        /// <summary>
        /// Đánh dấu hoàn thành đơn giao hàng
        /// </summary>
        private void BtnComplete_Click(object sender, RoutedEventArgs e)
        {
            var confirm = new Dialog() { Message = "Giao hàng đã thành công?" };
            confirm.Owner = Window.GetWindow(this);
            confirm.ShowDialog();
            if (true == confirm.DialogResult)
            {
                // Lấy đối tượng Bill đang chọn trong List View
                if (listBill.SelectedItem == null) return;
                Bill bill = listBill.SelectedItem as Bill;

                try
                {
                    // Lấy đối tượng tương ứng trong CSDL ra
                    var db = new Management_SystemEntities();
                    var curBill = db.Bills.Find(bill.Date);

                    // Cập nhật trạng thái
                    curBill.Status = "Đã hoàn thành";
                    db.SaveChanges();

                    // Cập nhật lên List View
                    int curIndex = listBill.SelectedIndex;
                    bills.Insert(curIndex + 1, curBill);
                    bills.RemoveAt(curIndex);
                    listBill.SelectedIndex = curIndex;

                    // Tắt 2 button
                    btnComplete.IsEnabled = btnIgnore.IsEnabled = false;
                }
                catch (Exception) {

                }
            }
        }

        /// <summary>
        /// Hủy đơn hàng
        /// </summary>
        private void BtnIgnore_Click(object sender, RoutedEventArgs e)
        {
            var confirm = new Dialog() { Message = "Hủy đơn hàng đang giao?" };
            confirm.Owner = Window.GetWindow(this);
            confirm.ShowDialog();
            if (true == confirm.DialogResult)
            {
                // Lấy đối tượng Bill đang chọn trong List View
                if (listBill.SelectedItem == null) return;
                Bill bill = listBill.SelectedItem as Bill;

                try
                {
                    // Lấy sản phẩm tương ứng trong CSDL ra
                    var db = new Management_SystemEntities();
                    var curProduct = db.Products.Find(bill.ProductId);
                    var curBill = db.Bills.Find(bill.Date);

                    // Đưa lại vào kho
                    curProduct.CurrentAmount += (bill.Number + bill.NumberGiven); // Tính cả phần tặng

                    // Cập nhật trạng thái
                    curBill.Status = "Đã hủy";
                    db.SaveChanges();

                    // Cập nhật lên List View
                    int curIndex = listBill.SelectedIndex;
                    bills.Insert(curIndex + 1, curBill);
                    bills.RemoveAt(curIndex);
                    listBill.SelectedIndex = curIndex;

                    // Tắt 2 button
                    btnComplete.IsEnabled = btnIgnore.IsEnabled = false;
                }
                catch (Exception) {

                }

            }
        }

        /// <summary>
        /// Xem chi tiết sản phẩm trong hóa đơn
        /// </summary>
        private void BtnSeeProduct_Click(object sender, RoutedEventArgs e)
        {
            if (listBill.SelectedIndex >= 0)
            {
                try
                {
                    var db = new Management_SystemEntities();
                    var target = db.Products.Find((listBill.SelectedItem as Bill).ProductId);
                    NavigationService.Navigate(new ProductDetailPage(target));
                }
                catch (Exception) { }
            }
        }
    }
}
