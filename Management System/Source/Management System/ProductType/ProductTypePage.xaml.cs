using Aspose.Cells;
using MaterialDesignThemes.Wpf;
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
    /// Interaction logic for ProductType.xaml
    /// </summary>
    public partial class ProductTypePage : Page
    {
        // Delegate để refresh combobox ở NewProductPage
        public delegate void DelegateRefeshProductTypeList(bool Data);
        public DelegateRefeshProductTypeList RefreshProductTypeList;

        // Khởi tạo danh sách loại sản phẩm
        ObservableCollection<ProductType> productTypes;

        public ProductTypePage()
        {
            InitializeComponent();

            // Combobox
            comboArrange.ItemsSource = new string[] { "Tăng dần ngày tạo", "Giảm dần ngày tạo",
                        "Tăng dần số sản phẩm", "Giảm dần số sản phẩm" };
            comboArrange.SelectedIndex = 0;

            // Get và hiển thị danh sách loại sản phẩm
            Thread getPTypes = new Thread(delegate ()
            {
                // Get và hiển thị danh sách loại sản phẩm
                var db = new Management_SystemEntities();

                // ObservableCollection có implements INotifyCollectionChanged interface
                productTypes = new ObservableCollection<ProductType>(db.ProductTypes);

                // Đặt Item Source cho List View
                Dispatcher.Invoke(() => {
                    listProductType.ItemsSource = productTypes;
                    ProgressBar.IsEnabled = false;
                    ProgressBar.Visibility = Visibility.Hidden;
                });
            });
            getPTypes.Start();
        }

        #region Xử lý hiệu ứng Comboxbox
        /// <summary>
        /// Hiệu ứng khi chọn
        /// </summary>
        private void ComboProductTypes_DropDownOpened(object sender, EventArgs e)
        {
            comboArrange.Background = Brushes.LightGray;
        }

        /// <summary>
        /// Hiệu ứng khi bỏ chọn
        /// </summary>
        private void ComboProductTypes_DropDownClosed(object sender, EventArgs e)
        {
            comboArrange.Background = Brushes.Transparent;
        }
        #endregion

        /// <summary>
        /// 3 hàm hỗ trợ sắp xếp loại sản phẩm
        /// </summary>
        private void ComboArrange_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ArrageProductType(comboArrange.SelectedIndex);
        }
        public void ArrageProductType(int choice)
        {
            if (productTypes != null)
            {
                for (int i = 0; i < productTypes.Count; i++)
                {
                    for (int j = i; j < productTypes.Count; j++)
                    {
                        switch (choice)
                        {
                            case 0: if (productTypes[i].Date > productTypes[j].Date) Swap(i, j); break;
                            case 1: if (productTypes[i].Date < productTypes[j].Date) Swap(i, j); break;
                            case 2: if (productTypes[i].NumOfProduct > productTypes[j].NumOfProduct) Swap(i, j); break;
                            case 3: if (productTypes[i].NumOfProduct < productTypes[j].NumOfProduct) Swap(i, j); break;
                        }

                    }
                }
            }
        }
        public void Swap(int i, int j)
        {
            ProductType temp = productTypes[i];
            productTypes[i] = productTypes[j];
            productTypes[j] = temp;
        }

        /// <summary>
        /// Chọn một Item trong List View
        /// </summary>
        private void ListProductType_SelectionChanged(object sender, RoutedEventArgs e)
        {
            // Lấy đối tượng ProductType tương ứng
            if (listProductType.SelectedItem == null) return;
            ProductType productType = listProductType.SelectedItem as ProductType;

            // Đưa thông tin vào các TextBox
            editProductTypeName.Text = productType.Name;
            editProductTypeId.Text = productType.Id;
            editProductTypeDescription.Text = productType.Description;

            // Bật 2 button Sửa và Xóa
            btnUpdateProductType.IsEnabled = true;
            btnRemoveProductType.IsEnabled = true;
        }

        /// <summary>
        /// Nút Thêm (chỉ cho phép nhập dữ liệu)
        /// </summary>
        private void BtnAddProductType_Click(object sender, RoutedEventArgs e)
        {
            // Kéo ListView lên đầu và vô hiệu hóa tạm thời
            if (listProductType.Items.Count > 0)
            {
                listProductType.ScrollIntoView(listProductType.Items[0]);
            }
            listProductType.SelectedIndex = -1;
            listProductType.IsEnabled = false;

            // Bật và thay nội dung 2 button Sửa và Xóa
            btnUpdateProductType.IsEnabled = true;
            btnUpdateProductType.Content = "Lưu";
            btnUpdateProductType.Tag = "Thêm";
            btnRemoveProductType.IsEnabled = true;
            btnRemoveProductType.Content = "Hủy";

            // Tắt button Thêm
            btnAddProductType.IsEnabled = false;

            // Cho phép người dùng chỉnh sửa các TextBox
            editProductTypeName.IsEnabled = true;
            editProductTypeName.Clear();
            editProductTypeId.IsEnabled = true;
            editProductTypeId.Clear();
            editProductTypeDescription.IsEnabled = true;
            editProductTypeDescription.Clear();

            // Focus vào Tên loại sản phẩm cho người dùng nhập
            editProductTypeName.Focus();
        }

        /// <summary>
        /// Nút Xóa hoặc Hủy (có thao tác CSDL)
        /// </summary>
        private void BtnRemoveProductType_Click(object sender, RoutedEventArgs e)
        {
            // Nếu user muốn hủy bỏ dữ liệu đang nhập
            if (btnRemoveProductType.Content.Equals("Hủy"))
            {
                // Hiện thông báo xác nhận
                var dialog = new Dialog() { Message = "Hủy bỏ dữ liệu đã nhập?" };
                dialog.Owner = Window.GetWindow(this);
                if (dialog.ShowDialog() == false) return;

                // Hủy cho phép chỉnh sửa các TextBox
                editProductTypeName.IsEnabled = false;
                editProductTypeId.IsEnabled = false;
                editProductTypeDescription.IsEnabled = false;

                // Làm sạch các TextBox nếu vừa định Thêm mới
                if (btnUpdateProductType.Tag.Equals("Thêm"))
                {
                    editProductTypeName.Clear();
                    editProductTypeId.Clear();
                    editProductTypeDescription.Clear();

                    // Tắt luôn hai button Sửa và Xóa (nếu vừa định Sửa thì thôi)
                    btnUpdateProductType.IsEnabled = false;
                    btnRemoveProductType.IsEnabled = false;
                }
                // Reset dữ liệu cũ nếu vừa định Sửa
                else
                {
                    // Lấy đối tượng ProductType tương ứng
                    ProductType productType = listProductType.SelectedItem as ProductType;
                    editProductTypeName.Text = productType.Name;
                    editProductTypeId.Text = productType.Id;
                    editProductTypeDescription.Text = productType.Description;
                }

                // Reset nội dung 2 button Sửa và Xóa
                btnUpdateProductType.Content = "Sửa";
                btnRemoveProductType.Content = "Xóa";
                btnAddProductType.IsEnabled = true;

                // Bật lại List View
                listProductType.IsEnabled = true;
            }
            // Nếu muốn xóa một loại sản phẩm
            else
            {
                // Hiện thông báo xác nhận
                var dialog = new Dialog() { Message = "Xóa loại sản phẩm đã chọn?" };
                dialog.Owner = Window.GetWindow(this);
                if (dialog.ShowDialog() == false) return;

                // Lấy đối tượng từ List View
                ProductType selectedItem = listProductType.SelectedItem as ProductType;

                #region THAO TÁC VỚI CSDL
                // Tìm đối tượng tương ứng trong CSDL và xóa
                var db = new Management_SystemEntities();
                try
                {
                    ProductType type = db.ProductTypes.Where(x => x.Id == selectedItem.Id).FirstOrDefault();
                    db.ProductTypes.Remove(type);
                    db.SaveChanges();

                    // Cập nhật lên List View
                    productTypes.RemoveAt(listProductType.SelectedIndex);
                    listProductType.ItemsSource = null;
                    listProductType.ItemsSource = productTypes;
                }
                catch (Exception ex)
                {
                    // Nếu bắt exception tức đang có sản phẩm thuộc loại muốn xóa
                    var dialogError = new Dialog()
                    {
                        Message = "Tồn tại sản phẩm thuộc loại sản phẩm này" +
                                "\nXóa tất cả sản phẩm đó?"
                    };
                    dialogError.Owner = Window.GetWindow(this);

                    // Nếu user đồng ý, thì xóa hết sản phẩm tương ứng
                    if (dialogError.ShowDialog() == true)
                    {
                        // Tìm danh sách sản phẩm tương ứng sao đó RemoveRange
                        db.Products.RemoveRange(db.Products.Where(x => x.ProductType == selectedItem.Id).ToList());

                        // Sau cùng mới xóa loại sản phẩm
                        db.ProductTypes.Remove(db.ProductTypes.Where(x => x.Id == selectedItem.Id).FirstOrDefault());

                        // Cập nhật lên List View
                        productTypes.Remove(selectedItem);
                        listProductType.ItemsSource = null;
                        listProductType.ItemsSource = productTypes;
                        db.SaveChanges();
                    }
                    else return;
                }
                #endregion

                // Xóa xong thì tắt 2 nút Sửa và Xóa + làm sạch TextBox
                btnUpdateProductType.IsEnabled = false;
                btnRemoveProductType.IsEnabled = false;
                editProductTypeId.Clear();
                editProductTypeName.Clear();
                editProductTypeDescription.Clear();

                // Cập nhật combobox ở trang trước
                if (RefreshProductTypeList != null)
                {
                    RefreshProductTypeList.Invoke(true);
                }
            }
        }

        /// <summary>
        /// Nút Sửa hoặc Lưu (có thao tác CSDL)
        /// </summary>
        private void BtnUpdateProductType_Click(object sender, RoutedEventArgs e)
        {
            // Nếu nội dung button là "Sửa" thì user muốn sửa một loại sản phẩm (bắt đầu nhập thông tin)
            if (btnUpdateProductType.Content.Equals("Sửa"))
            {
                // Thay nội dung các button
                btnUpdateProductType.Content = "Lưu";
                btnUpdateProductType.Tag = "Sửa";
                btnRemoveProductType.Content = "Hủy";
                btnAddProductType.IsEnabled = false;

                // Cho phép chỉnh sửa các TextBox
                editProductTypeName.IsEnabled = true;
                editProductTypeId.IsEnabled = true;
                editProductTypeDescription.IsEnabled = true;
                editProductTypeName.Focus();

                // Tạm thời vô hiệu hóa List View
                listProductType.IsEnabled = false;
            }

            // Nếu nội dung button là "Lưu" thì user muốn xác nhận việc thêm/sửa vừa làm
            else
            {
                // Kiểm tra dữ liệu có nhập đầy đủ không
                if (editProductTypeId.Text.Length == 0
                    || editProductTypeName.Text.Length == 0)
                {
                    var dialogError1 = new Dialog() { Message = "Vui lòng nhập đầy đủ các thông tin" };
                    dialogError1.Owner = Window.GetWindow(this);
                    dialogError1.ShowDialog();
                    return;
                }

                // Tạo đối tượng từ các TextBox
                ProductType type = new ProductType()
                {
                    Name = editProductTypeName.Text.Length == 0 ? null : editProductTypeName.Text,
                    Id = editProductTypeId.Text.Length == 0 ? null : editProductTypeId.Text,
                    Description = editProductTypeDescription.Text.Length == 0 ? null : editProductTypeDescription.Text
                    // Còn thiếu trường SỐ SẢN PHẨM và NGÀY TẠO
                };

                #region THAO TÁC VỚI CSDL
                // Nếu xác nhận thêm mới (1 nút 2 chức năng, lưu khi thêm và lưu khi sửa)
                if (btnUpdateProductType.Tag.Equals("Thêm"))
                {
                    // Hiện thông báo xác nhận
                    var dialog = new Dialog() { Message = "Thêm loại sản phẩm đã nhập?" };
                    dialog.Owner = Window.GetWindow(this);
                    if (dialog.ShowDialog() == false) return;

                    var db = new Management_SystemEntities();
                    try
                    {
                        // Thêm các thuộc tính còn lại
                        type.NumOfProduct = 0;
                        type.Date = DateTime.Now;

                        db.ProductTypes.Add(type);
                        db.SaveChanges();

                        // Cập nhật lên List View
                        productTypes.Add(type);
                        listProductType.SelectedIndex = productTypes.Count - 1;
                    }
                    catch (Exception ex)
                    {
                        // Hiện thông báo lỗi
                        var dialogError = new Dialog() { Message = "Mã loại sản phẩm đã tồn tại" };
                        dialogError.Owner = Window.GetWindow(this);
                        dialogError.ShowDialog();
                        return;
                    }
                }

                // Nếu xác nhận sửa
                if (btnUpdateProductType.Tag.Equals("Sửa"))
                {
                    // Hiện thông báo xác nhận
                    var dialog = new Dialog() { Message = "Sửa loại sản phẩm đã chọn?" };
                    dialog.Owner = Window.GetWindow(this);
                    if (dialog.ShowDialog() == false) return;
                    var db = new Management_SystemEntities();

                    // Thêm hai thuộc tính SỐ SẢN PHẨM và NGÀY TẠO
                    type.NumOfProduct = ((ProductType)listProductType.SelectedItem).NumOfProduct;
                    type.Date = ((ProductType)listProductType.SelectedItem).Date;

                    // Kiểm tra xem user có sửa mã loại hay không (so sánh với List View)
                    if (type.Id == ((ProductType)listProductType.SelectedItem).Id)
                    {
                        // Nếu không thì chỉ cần sửa thuộc tính khác
                        var target = db.ProductTypes.Find(type.Id);
                        if (target != null)
                        {
                            // SỐ SẢN PHẨM với NGÀY TẠO để nguyên
                            target.Name = type.Name;
                            target.Description = type.Description;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        // Nếu có sửa, xác nhận lần nữa
                        var dialogError = new Dialog() { Message = "Để sửa đổi mã loại sản phẩm bạn cần sửa tất cả các sản phẩm tương ứng?" };
                        dialogError.Owner = Window.GetWindow(this);

                        // Nếu user đồng ý, thì sửa lại thuộc tính loại sản phẩm của tất cả sản phẩm tương ứng
                        if (dialogError.ShowDialog() == true)
                        {
                            // B1: Kiểm tra mã mới đã tồn tại chưa
                            if (db.ProductTypes.Find(type.Id) != null)
                            {
                                var dialogError1 = new Dialog() { Message = "Mã loại sản phẩm đã tồn tại" };
                                dialogError1.Owner = Window.GetWindow(this);
                                dialogError1.ShowDialog();
                                return;
                            }

                            // B2: Thêm đối tượng dữ liệu mới (mã loại mới)
                            db.ProductTypes.Add(type);

                            // B3: Sửa tất cả sản phẩm
                            var list = db.Products.Where(x => x.ProductType == ((ProductType)listProductType.SelectedItem).Id).ToList();
                            for (int i = 0; i < list.Count; i++) list[i].ProductType = type.Id;

                            // B4: Xóa dữ liệu cũ
                            var target = db.ProductTypes.Find(((ProductType)listProductType.SelectedItem).Id);
                            if (target != null) db.ProductTypes.Remove(target);
                            db.SaveChanges();
                        }
                    }

                    // Cập nhật lên List View
                    int curIndex = listProductType.SelectedIndex;
                    productTypes.Insert(curIndex + 1, type);
                    productTypes.RemoveAt(curIndex);
                    listProductType.SelectedIndex = curIndex;
                }
                #endregion

                // Reset nội dung 2 button Sửa và Xóa
                btnUpdateProductType.Content = "Sửa";
                btnRemoveProductType.Content = "Xóa";

                // Vô hiệu hóa các TextBox
                editProductTypeId.IsEnabled = false;
                editProductTypeName.IsEnabled = false;
                editProductTypeDescription.IsEnabled = false;

                // Bật lại button Thêm và List View
                btnAddProductType.IsEnabled = true;
                listProductType.IsEnabled = true;

                // Cập nhật combobox ở trang trước
                if (RefreshProductTypeList != null)
                {
                    RefreshProductTypeList.Invoke(true);
                }
            }
        }

        /// <summary>
        /// Nút Export - Xuất dữ liệu ra Excel
        /// </summary>
        private void BtnExportProductType_Click(object sender, RoutedEventArgs e)
        {
            // Hiện thông báo xác nhận
            var dialog = new Dialog() { Message = "Xuất dữ liệu ra tập tin Excel?" };
            dialog.Owner = Window.GetWindow(this);
            if (dialog.ShowDialog() == false) return;

            // Mở hộp thoại lưu tập tin
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.DefaultExt = ".xlsx";
            saveFileDialog.Filter = "Excel Workbook (.xlsx)|*.xlsx";

            if (false == saveFileDialog.ShowDialog()) return;
            string filename = saveFileDialog.FileName;

            Workbook workbook = new Workbook();
            Worksheet worksheet = workbook.Worksheets[0];
            worksheet.Name = "Product Type";
            
            // Ghi cột tên loại
            worksheet.Cells["A1"].PutValue("TÊN LOẠI");
            worksheet.Cells["B1"].PutValue("MÃ LOẠI");
            worksheet.Cells["C1"].PutValue("MÔ TẢ");
            worksheet.Cells["D1"].PutValue("NGÀY TẠO");
            for (int i = 0; i < productTypes.Count; i++)
            {
                worksheet.Cells[$"A{i+2}"].PutValue(productTypes[i].Name);
                worksheet.Cells[$"B{i + 2}"].PutValue(productTypes[i].Id);
                worksheet.Cells[$"C{i + 2}"].PutValue(productTypes[i].Description);
                worksheet.Cells[$"D{i + 2}"].PutValue(productTypes[i].Date.ToString());
            }

            // Cột số sản phẩm không cần lưu
            // Vì khi import, nếu import được tức loại sản phẩm này mới => Số sản phẩm là 0
            // Nếu loại sản phẩm này đã có trong CSDL thì không cho import

            // Lưu lại
            worksheet.AutoFitColumns();
            workbook.Save(filename);
        }

        /// <summary>
        /// Nút Import - Nhập dữ liệu từ Excel
        /// </summary>
        private void BtnImportProductType_Click(object sender, RoutedEventArgs e)
        {
            // Mở hộp thoại mở tập tin
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.DefaultExt = ".xlsx";
            openFileDialog.Filter = "Excel Workbook (.xlsx)|*.xlsx";

            if (false == openFileDialog.ShowDialog()) return;
            string filename = openFileDialog.FileName;

            var excel = new ImportFromExcel(filename);
            excel.Owner = Window.GetWindow(this);
            excel.SendProductType = Import;
            excel.Show();
        }

        /// <summary>
        /// Hàm Import cho delegate
        /// </summary>
        public void Import(ObservableCollection<ProductType> Data)
        {
            if (Data != null)
            {
                var db = new Management_SystemEntities();
                for (int i = 0; i < Data.Count; i++)
                {
                    try
                    {
                        // Thêm các thuộc tính còn lại
                        Data[i].NumOfProduct = 0;

                        db.ProductTypes.Add(Data[i]);
                        db.SaveChanges();

                        // Cập nhật lên List View
                        productTypes.Add(Data[i]);
                        listProductType.SelectedIndex = productTypes.Count - 1;
                    }
                    catch (Exception ex)
                    {
                        continue; // Không xảy ra lỗi trùng mã vì đã xử lý trước
                    }
                }
            }
            ArrageProductType(comboArrange.SelectedIndex);
        }
    }

    /// <summary>
    /// Hàm chuyển đổi STT cho List View
    /// </summary>
    public class DataConverter : IValueConverter
    {
        // Reference: https://social.msdn.microsoft.com/Forums/vstudio/en-US/4376333f-3f91-416c-aa6c-1df6a9991f8a/how-can-i-bind-current-row-index-in-gridview-or-listview-in-wpf
        public object Convert(object value, Type TargetType, object parameter, CultureInfo culture)
        {
            ListViewItem item = (ListViewItem)value;
            ListView listView = ItemsControl.ItemsControlFromItemContainer(item) as ListView;
            int index = listView.ItemContainerGenerator.IndexFromContainer(item) + 1;
            return index.ToString();
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}