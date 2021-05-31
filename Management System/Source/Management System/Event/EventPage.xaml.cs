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
    /// Interaction logic for EventPage.xaml
    /// </summary>
    public partial class EventPage : Page
    {
        // Delegate để refresh combobox ở BillPage
        public delegate void DelegateRefeshEventList(bool Data);
        public DelegateRefeshEventList RefreshEventList;

        // Khởi tạo danh sách sự kiện
        ObservableCollection<Event> events;

        public EventPage()
        {
            InitializeComponent();

            // Get và hiển thị danh sách sự kiện
            Thread getPTypes = new Thread(delegate ()
            {
                // Get và hiển thị danh sách loại sản phẩm
                var db = new Management_SystemEntities();

                // ObservableCollection có implements INotifyCollectionChanged interface
                events = new ObservableCollection<Event>(db.Events);

                // Đặt Item Source cho List View
                Dispatcher.Invoke(() => {
                    listEvent.ItemsSource = events;
                    ProgressBar.IsEnabled = false;
                    ProgressBar.Visibility = Visibility.Hidden;
                });
            });
            getPTypes.Start();
        }

        /// <summary>
        /// Chọn một Item trong List View
        /// </summary>
        private void ListEvent_SelectionChanged(object sender, RoutedEventArgs e)
        {
            // Lấy đối tượng ProductType tương ứng
            if (listEvent.SelectedItem == null) return;
            Event aEvent = listEvent.SelectedItem as Event;

            // Đưa thông tin vào các TextBox
            editEventName.Text = aEvent.Name;
            editDateBegin.Text = aEvent.DateBegin.ToString();
            editDateEnd.Text = aEvent.DateEnd.ToString();
            editSale.Text = aEvent.Sale.ToString();
            editBuyGet_Buy.Text = aEvent.BuyGet_Buy.ToString();
            editBuyGet_Get.Text = aEvent.BuyGet_Get.ToString();

            // Bật Xóa
            btnRemoveEvent.IsEnabled = true;

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
        /// Nút Thêm
        /// </summary>
        private void BtnAddEvent_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            // Nếu người dùng muốn bắt đầu nhập dữ liệu để thêm
            if (button.Content.Equals("Thêm"))
            {
                // Kéo ListView lên đầu và vô hiệu hóa tạm thời
                if (listEvent.Items.Count > 0)
                {
                    listEvent.ScrollIntoView(listEvent.Items[0]);
                }
                listEvent.SelectedIndex = -1;
                listEvent.IsEnabled = false;

                // Bật và thay nội dung 2 button Thêm và Xóa
                btnRemoveEvent.IsEnabled = true;
                btnRemoveEvent.Content = "Hủy";
                btnAddEvent.Content = "Lưu";

                // Cho phép người dùng chỉnh sửa các TextBox
                editEventName.IsReadOnly = false;
                editEventName.Clear();
                editSale.IsReadOnly = false;
                editSale.Clear();
                editDateBegin.IsEnabled = true;
                editDateBegin.Text = "";
                editDateEnd.IsEnabled = true;
                editDateEnd.Text = "";
                editBuyGet_Buy.IsReadOnly = false;
                editBuyGet_Buy.Clear();
                editBuyGet_Get.IsReadOnly = false;
                editBuyGet_Get.Clear();

                // Focus vào Tên loại sản phẩm cho người dùng nhập
                editEventName.Focus();
            }

            // Nếu người dùng muốn xác nhận lưu lại sau khi đã nhập
            else
            {
                // Kiểm tra dữ liệu có nhập đầy đủ không
                if (editEventName.Text.Length == 0
                    || editDateBegin.Text.Length == 0
                    || editDateEnd.Text.Length == 0)
                {
                    var dialogError1 = new Dialog() { Message = "Vui lòng nhập đầy đủ các thông tin" };
                    dialogError1.Owner = Window.GetWindow(this);
                    dialogError1.ShowDialog();
                    return;
                }

                // Kiểm tra % giảm giá có hợp lệ
                int sale = -1;
                int.TryParse(editSale.Text, out sale);
                if (sale < 0 || sale > 100)
                {
                    var dialogError1 = new Dialog() { Message = "Tỷ lệ giảm giá phải từ 0 đến 100" };
                    dialogError1.Owner = Window.GetWindow(this);
                    dialogError1.ShowDialog();
                    return;
                }

                var dialogError = new Dialog() { Message = "Xác nhận thêm sự kiện?" };
                dialogError.Owner = Window.GetWindow(this);
                if (true == dialogError.ShowDialog())
                {
                    var db = new Management_SystemEntities();

                    // Tạo đối tượng Event tương ứng
                    var aEvent = new Event()
                    {
                        Name = editEventName.Text,
                        DateBegin = DateTime.Parse(editDateBegin.Text),
                        DateEnd = DateTime.Parse(editDateEnd.Text),
                        Sale = editSale.Text.Length == 0 ? 0 : int.Parse(editSale.Text),
                        BuyGet_Buy = editBuyGet_Buy.Text.Length == 0 ? 0 : int.Parse(editBuyGet_Buy.Text),
                        BuyGet_Get = editBuyGet_Get.Text.Length == 0 ? 0 : int.Parse(editBuyGet_Get.Text)
                    };

                    db.Events.Add(aEvent);
                    db.SaveChanges();

                    // Reset nội dung button Thêm
                    btnAddEvent.Content = "Thêm";
                    btnRemoveEvent.Content = "Xóa";

                    // Vô hiệu hóa các TextBox
                    editEventName.IsReadOnly = true;
                    editSale.IsReadOnly = true;
                    editDateBegin.IsEnabled = false;
                    editDateEnd.IsEnabled = false;
                    editBuyGet_Buy.IsReadOnly = true;
                    editBuyGet_Get.IsReadOnly = true;

                    // Cập nhật lên List View
                    events.Add(aEvent);
                    listEvent.SelectedIndex = events.Count - 1;
                    listEvent.IsEnabled = true;

                    // Cập nhật combobox ở trang trước
                    if (RefreshEventList != null)
                    {
                        RefreshEventList.Invoke(true);
                    }
                }
            }
        }

        /// <summary>
        /// Nút Xóa hoặc Hủy (có thao tác CSDL)
        /// </summary>
        private void BtnRemoveEvent_Click(object sender, RoutedEventArgs e)
        {
            // Nếu user muốn hủy bỏ dữ liệu đang nhập
            if (btnRemoveEvent.Content.Equals("Hủy"))
            {
                // Hiện thông báo xác nhận
                var dialog = new Dialog() { Message = "Hủy bỏ dữ liệu đã nhập?" };
                dialog.Owner = Window.GetWindow(this);
                if (dialog.ShowDialog() == false) return;

                // Hủy cho phép chỉnh sửa các TextBox
                editEventName.IsReadOnly = true;
                editSale.IsReadOnly = false;
                editBuyGet_Buy.IsReadOnly = true;
                editBuyGet_Get.IsReadOnly = true;
                editDateBegin.IsEnabled = false;
                editDateEnd.IsEnabled = false;

                // Làm sạch các TextBox 
                editEventName.Clear();
                editSale.Clear();
                editBuyGet_Buy.Clear();
                editBuyGet_Get.Clear();
                editDateBegin.Text = "";
                editDateEnd.Text = "";

                // Sửa nội dung button Thêm
                btnAddEvent.Content = "Thêm";

                // Bật lại List View
                listEvent.IsEnabled = true;
            }
            // Nếu muốn xóa một loại sản phẩm
            else
            {
                // Hiện thông báo xác nhận
                var dialog = new Dialog() { Message = "Xóa sự kiện đã chọn?" };
                dialog.Owner = Window.GetWindow(this);
                if (dialog.ShowDialog() == false) return;

                // Lấy đối tượng từ List View
                Event selectedItem = listEvent.SelectedItem as Event;

                #region THAO TÁC VỚI CSDL
                // Tìm đối tượng tương ứng trong CSDL và xóa
                var db = new Management_SystemEntities();
                try
                {
                    Event aEvent = db.Events.Find(selectedItem.Name);
                    db.Events.Remove(aEvent);
                    db.SaveChanges();

                    // Cập nhật lên List View
                    events.RemoveAt(listEvent.SelectedIndex);
                    listEvent.ItemsSource = null;
                    listEvent.ItemsSource = events;
                }
                catch (Exception ex)
                {
                    return;
                }
                #endregion

                // Xóa xong thì tắt button Xóa + làm sạch TextBox
                btnRemoveEvent.IsEnabled = false;
                editEventName.Clear();
                editSale.Clear();
                editBuyGet_Buy.Clear();
                editBuyGet_Get.Clear();
                editDateBegin.Text = "";
                editDateEnd.Text = "";

                // Cập nhật combobox ở trang trước
                if (RefreshEventList != null)
                {
                    RefreshEventList.Invoke(true);
                }
            }
        }
    }
}