using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for NewBillPage.xaml
    /// </summary>
    public partial class NewBillPage : Page
    {
        // Delegate để refresh danh sách khuyến mãi
        public delegate void DelegateRefeshEventList(bool Data);
        public DelegateRefeshEventList RefreshEventList;
        public Product product;

        /// <summary>
        /// View model cho danh sách sự kiện
        /// </summary>
        public class EventViewModel : Event
        {
            public string onScreen { get; set; }
        }

        public NewBillPage()
        {
            InitializeComponent();
            refresh(true);
        }

        /// <summary>
        /// Dùng cho Navigate từ ProductDetailPage
        /// </summary>
        public NewBillPage(Product _product)
        {
            InitializeComponent();
            product = _product;
            editProductId.Text = product.Id;
            refresh(true);
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
        /// Xem các sự kiện
        /// </summary>
        private void BtnAddEvent_Click(object sender, RoutedEventArgs e)
        {
            var eventWin = new EventPage();
            eventWin.RefreshEventList = refresh;
            NavigationService.Navigate(eventWin);
        }

        /// <summary>
        /// Làm mới danh sách khuyến mãi sau khi thêm/xóa
        /// </summary>
        public void refresh(bool Data)
        {
            if (Data)
            {
                // Get và hiển thị danh sách khuyến mãi
                Thread getPTypes = new Thread(delegate ()
                {
                    var db = new Management_SystemEntities();
                    var events = new ObservableCollection<Event>(db.Events
                        .Where(x => x.DateBegin <= DateTime.Now && DateTime.Now <= x.DateEnd));

                    ObservableCollection<EventViewModel> viewModels = new ObservableCollection<EventViewModel>();
                    for (int i = 0; i < events.Count; i++)
                    {
                        // Cắt ghép để thành chuỗi tóm tắt
                        string value = events[i].Name + (events[i].Sale > 0 ? " - Giảm " + events[i].Sale + "%" : "")
                            + (events[i].BuyGet_Get > 0 ? " - Mua " + events[i].BuyGet_Buy + " tặng " + events[i].BuyGet_Get : "");

                        // Thêm vào List View
                        viewModels.Add(new EventViewModel() { onScreen = value
                            , Sale = events[i].Sale
                            , BuyGet_Buy = events[i].BuyGet_Buy
                            , BuyGet_Get = events[i].BuyGet_Get
                            , DateBegin = events[i].DateBegin
                            , DateEnd = events[i].DateEnd });
                    }

                    Dispatcher.Invoke(() => {
                        listEvent.ItemsSource = viewModels; // Tác động lên UI
                        ProgressBar.IsEnabled = false;
                        ProgressBar.Visibility = Visibility.Hidden;

                        // Cập nhật giá cả + lượng tặng (xóa rồi nhập lại để gọi hàm phát sinh)
                        string numberBuy = editNumberBuy.Text;
                        editNumberBuy.Text = "";
                        editNumberBuy.Text = numberBuy;
                    });
                });
                getPTypes.Start();
            }
        }

        /// <summary>
        /// Xem chi tiết một sự kiện khuyến mãi
        /// </summary>
        private void ListEvent_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            EventViewModel viewModel = (sender as ListView).SelectedItem as EventViewModel;
            var dialog = new Dialog() { Message = viewModel.onScreen
                + "\nTừ " + viewModel.DateBegin + "\nĐến " + viewModel.DateEnd};
            dialog.Owner = Window.GetWindow(this);
            dialog.ShowDialog();
        }

        /// <summary>
        /// Làm mới dữ liệu đã nhập
        /// </summary>
        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            editCustomerName.Clear();
            editCustomerPhone.Clear();
            editNumberBuy.Clear();
            editNumberGet.Clear();
            editOriginalPrice.Clear();
            editSellPrice.Clear();
            editMoneyTaken.Clear();
            editMoneyExchange.Clear();
            editAddress.Clear();
            editDeposit.Clear();
            editShipCost.Clear();
            editMoneyWillGet.Clear();
            rdoGoToShop.IsChecked = true;
        }

        /// <summary>
        /// Pick sản phẩm, sau đó làm mới các TextBox giá cả
        /// </summary>
        private void EditProductId_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var chooseProduct = new ProductPage();
            chooseProduct.PickProductId = (x) =>
            {
                product = x;
                editProductId.Text = product.Id;

                // Cập nhật giá cả + lượng tặng (xóa rồi nhập lại để gọi hàm phát sinh)
                string numberBuy = editNumberBuy.Text;
                editNumberBuy.Text = "";
                editNumberBuy.Text = numberBuy;
            };
            NavigationService.Navigate(chooseProduct);
        }

        /// <summary>
        /// Tự động phát sinh giá sau khi nhập số lượng
        /// </summary>
        private void EditNumberBuy_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (editNumberBuy.Text.Length > 0)
            {
                // Lấy số lượng người dùng muốn mua
                int NumberBuy = 0;
                int.TryParse(editNumberBuy.Text, out NumberBuy);

                // Phát sinh giá gốc
                double value = 0;
                value = NumberBuy * product.Price;
                editOriginalPrice.Text = value.ToString("N0");

                // Phát sinh số lượng được tặng và giá bán dựa vào các sự kiện khuyến mãi
                int NumGiven = 0;
                for (int i = 0; i < listEvent.Items.Count; i++)
                {
                    EventViewModel viewModel = listEvent.Items[i] as EventViewModel;
                    if (viewModel.Sale != 0)
                    {
                        value = value - value * (double)(viewModel.Sale / 100.0);
                    }
                    if (viewModel.BuyGet_Buy != 0)
                    {
                        // VD: Mua 3 tặng 2 và mua 2 tặng 1, khách hàng mua 5 sản phẩm
                        // => Tặng (5/3)*2 + (5/2)*1 sản phẩm
                        NumGiven += (NumberBuy / (int)viewModel.BuyGet_Buy) * (int)viewModel.BuyGet_Get;
                    }
                }
                editNumberGet.Text = NumGiven.ToString();

                editSellPrice.Text = value.ToString("N0");
            }
            else
            {
                editOriginalPrice.Clear();
                editNumberGet.Clear();
                editSellPrice.Clear();
            }

            // Tính tiền trả lại + tiền sẽ thu nếu giao hàng
            CountExchange(null, null);
            CountMoneyWillGet(null, null);
        }

        /// <summary>
        /// Sự kiện khi chọn phương thức thanh toán (tắt các Control không liên quan)
        /// </summary>
        private void Radio_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton button = sender as RadioButton;
            if (button == rdoGoToShop)
            {
                if (editAddress != null) editAddress.Clear();
                if (editDeposit != null) editDeposit.Clear();
                if (editShipCost != null) editShipCost.Clear();
                if (editMoneyWillGet != null) editMoneyWillGet.Clear();
            }
            else
            {
                if (editMoneyTaken != null) editMoneyTaken.Clear();
                if (editMoneyExchange != null) editMoneyExchange.Clear();
            }
        }

        /// <summary>
        /// Tự động tính tiền trả lại (exchange)
        /// </summary>
        private void CountExchange(object sender, TextChangedEventArgs e)
        {
            if (editMoneyTaken.Text.Length > 0)
            {
                // Định dạng giá vừa nhập
                double taken = 0;
                double.TryParse(editMoneyTaken.Text, out taken);
                editMoneyTaken.Text = taken.ToString("N0");
                editMoneyTaken.CaretIndex = editMoneyTaken.Text.Length;

                // Tính toán số tiền thối lại
                double sellPrice = 0;
                double.TryParse(editSellPrice.Text, out sellPrice);
                editMoneyExchange.Text = (taken - sellPrice).ToString("N0");
            }
            else editMoneyExchange.Clear();
        }

        /// <summary>
        /// Tự động tính tiền sẽ thu khi đi giao hàng
        /// </summary>
        private void CountMoneyWillGet(object sender, TextChangedEventArgs e)
        {
            if (editDeposit.Text.Length > 0 || editShipCost.Text.Length > 0)
            {
                double sellPrice = 0; // Giá bán
                double.TryParse(editSellPrice.Text, out sellPrice);

                double deposit = 0; // Tiền cọc
                double.TryParse(editDeposit.Text, out deposit);
                editDeposit.CaretIndex = editDeposit.Text.Length;
                editDeposit.Text = deposit.ToString("N0");

                double shipCost = 0; // Phí ship
                double.TryParse(editShipCost.Text, out shipCost);
                editShipCost.CaretIndex = editShipCost.Text.Length;
                editShipCost.Text = shipCost.ToString("N0");

                // Tính số tiền cần thu khi đi giao
                editMoneyWillGet.Text = (sellPrice - deposit + shipCost).ToString("N0");
            }
            else
            {
                editDeposit.Clear();
                editShipCost.Clear();
                editMoneyWillGet.Clear();
            }
        }

        /// <summary>
        /// Xác nhận thêm hóa đơn
        /// </summary>
        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            // Kiểm tra dữ liệu có thiếu không
            if (editCustomerName.Text.Length == 0   // Nếu tên khách hàng trống
                || editProductId.Text.Length == 0   // Nếu sản phẩm mua trống
                || editNumberBuy.Text.Length == 0   // Nếu số lượng mua trống
                || (rdoGoToShop.IsChecked == true && editMoneyTaken.Text.Length == 0)   // Nếu thanh toán trực tiếp mà chưa đưa tiền
                || (rdoShip.IsChecked == true && editAddress.Text.Length == 0)          // Nếu thanh toán giao hàng mà không đưa địa chỉ
                || (rdoShip.IsChecked == true && editMoneyWillGet.Text.Length == 0))    // Nếu thanh toán giao hàng mà không biết số tiền sẽ thu
            {
                var dialog = new Dialog() { Message = "Vui lòng nhập đầy đủ thông tin" };
                dialog.Owner = Window.GetWindow(this);
                dialog.ShowDialog();
                return;
            }

            // Kiểm tra còn đủ hàng hay không
            var db = new Management_SystemEntities();
            var curProduct = db.Products.Find(editProductId.Text);
            int number = 0;                                         // Số lượng mua
            int.TryParse(editNumberBuy.Text, out number);
            int numberGiven = 0;                                    // Số lượng tặng
            int.TryParse(editNumberGet.Text, out numberGiven);
            if (curProduct.CurrentAmount < (number + numberGiven))  // Nếu số lượng không đủ thì thông báo
            {
                var dialog = new Dialog() { Message = "Sản phẩm không đủ số lượng" };
                dialog.Owner = Window.GetWindow(this);
                dialog.ShowDialog();
                return;
            }

            // Tạo đối tượng Bill tương ứng
            var bill = new Bill()
            {
                Date = DateTime.Now,
                Name = editCustomerName.Text,
                Phone = editCustomerPhone.Text.Length == 0 ? null : editCustomerPhone.Text,
                ProductId = editProductId.Text,
                Number = (int)(double.Parse(editNumberBuy.Text)),
                NumberGiven = (int)(double.Parse(editNumberGet.Text)),
                OriginalPrice = (long)(double.Parse(editOriginalPrice.Text)),
                FinalPrice = (long)(double.Parse(editSellPrice.Text)),
                GoToShop = (bool)rdoGoToShop.IsChecked,
                MoneyTaken = (bool)rdoGoToShop.IsChecked ? (long)(double.Parse(editMoneyTaken.Text)) : 0,
                MoneyExchange = (bool)rdoGoToShop.IsChecked ? (long)(double.Parse(editMoneyExchange.Text)) : 0,
                Address = (bool)rdoShip.IsChecked ? editAddress.Text : null,
                Deposit = (bool)rdoShip.IsChecked ? (long)(double.Parse(editDeposit.Text)) : 0,
                Ship = (bool)rdoShip.IsChecked ? (long)(double.Parse(editShipCost.Text)) : 0,
                MoneyWillGet = (bool)rdoShip.IsChecked ? (long)(double.Parse(editMoneyWillGet.Text)) : 0,
                Status = (bool)rdoGoToShop.IsChecked ? "Đã hoàn thành" : "Chưa hoàn thành"
                // Còn thiếu trường Event
            };

            // Thêm trường Event
            for (int i = 0; i < listEvent.Items.Count; i++)
            {
                bill.Event += (listEvent.Items[i] as EventViewModel).onScreen;
                if (i + 1 < listEvent.Items.Count) bill.Event += "; ";
            }

            // Xác nhận người dùng
            var error = new Dialog() { Message = "Xác nhận lưu hóa đơn?" };
            error.Owner = Window.GetWindow(this);
            error.ShowDialog();
            if (true == error.DialogResult)
            {
                // Thêm vào CSDL
                try
                {
                    db.Bills.Add(bill);
                    db.SaveChanges();

                    // Thông báo nhớ đánh dấu hoàn thành nếu giao hàng
                    if ((bool)rdoShip.IsChecked)
                    {
                        var dialog = new Dialog() { Message = "Hãy nhớ đánh dấu hoàn thành khi giao hàng thành công" };
                        dialog.Owner = Window.GetWindow(this);
                        dialog.ShowDialog();
                    }

                    // Trừ lượng tồn kho (sẽ hoàn lại nếu giao hàng không thành công)
                    curProduct.CurrentAmount -= (bill.Number + bill.NumberGiven); // Trừ cả phần tặng
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    var tooFast = new Dialog() { Message = "Bạn đã tạo hóa đơn quá nhanh!" };
                    tooFast.Owner = Window.GetWindow(this);
                    tooFast.ShowDialog();
                }
            }
        }

        /// <summary>
        /// Xem danh sách hóa đơn (lịch sử thanh toán)
        /// </summary>
        private void BtnHistory_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new BillPage());
        }
    }
}