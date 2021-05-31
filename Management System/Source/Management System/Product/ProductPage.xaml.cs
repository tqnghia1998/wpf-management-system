using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
    /// Interaction logic for ProductPage.xaml
    /// </summary>
    public partial class ProductPage : Page
    {
        // Khởi tạo danh sách sản phẩm
        ObservableCollection<Product> products;

        // Delegate cho trang tạo đơn hàng ở trước
        public delegate void PickProduct(Product Data);
        public PickProduct PickProductId;

        public ProductPage()
        {
            InitializeComponent();
            comboProductArrange.Items.Add("Nhập kho lâu nhất");
            comboProductArrange.SelectedIndex = 0;
            comboProductArrange.Items.Add("Nhập kho gần đây nhất");
            comboProductArrange.Items.Add("Giá tăng dần");
            comboProductArrange.Items.Add("Giá giảm dần");
            comboProductArrange.Items.Add("Tồn kho nhiều nhất");
            comboProductArrange.Items.Add("Tồn kho ít nhất");
            comboProductArrange.Items.Add("Bán chạy nhất");
            comboProductArrange.Items.Add("Bán ế nhất");

            Thread thread = new Thread(delegate ()
            {
                // Get và xác định số trang
                var db = new Management_SystemEntities();
                int numPages = (int)Math.Ceiling(db.Products.Count() / 12.0);
                if (numPages == 0) numPages = 1;

                // Đưa vào Combobox
                List<string> pageNumber = new List<string>();
                for (int i = 1; i <= numPages; i++)
                {
                    pageNumber.Add(i + "/" + numPages);
                }
                Dispatcher.Invoke(() => {
                    comboPageIndex.ItemsSource = pageNumber;
                    comboPageIndex.SelectedIndex = 0;
                });

                // Lấy danh sách sản phẩm
                products = new ObservableCollection<Product>(db.Products);
                for (int i = 0; i < products.Count; i++)
                {
                    for (int j = i; j < products.Count; j++)
                    {
                        if (compare(products[i], products[j]))
                        {
                            Product temp = products[i];
                            products[i] = products[j];
                            products[j] = temp;
                        }
                    }
                }

                // Cập nhật UI
                Dispatcher.Invoke(() =>
                {
                    listProduct.ItemsSource = products.Skip(comboPageIndex.SelectedIndex * 12).Take(12);
                    ProgressBar.IsEnabled = false;
                    ProgressBar.Visibility = Visibility.Hidden;
                });
            });
            thread.Start();
        }

        /// <summary>
        /// Hàm so sánh 2 Product theo các tiêu chí
        /// </summary>
        public bool compare(Product a, Product b)
        {
            int sortType = 0;
            Dispatcher.Invoke(() => { sortType = comboProductArrange.SelectedIndex; });
            switch (sortType)
            {
                case 0: return a.Date > b.Date; // Tăng dần thời gian nhập kho
                case 1: return a.Date < b.Date; // Giảm dần thời gian nhập kho
                case 2: return a.Price > b.Price; // Tăng dần giá cả
                case 3: return a.Price < b.Price; // Giảm dần giá cả
                case 4: return a.CurrentAmount < b.CurrentAmount; // Tăng dần số lượng tồn kho
                case 5: return a.CurrentAmount > b.CurrentAmount; // Giảm dần số lượng tồn kho
                case 6: return (a.InitialAmount - a.CurrentAmount) < (b.InitialAmount - b.CurrentAmount); // Tăng dần bán chạy nhất
                case 7: return (a.InitialAmount - a.CurrentAmount) > (b.InitialAmount - b.CurrentAmount); // Giảm dần bán ế nhất
                default: return true;
            }
        }

        #region Xử lý hiệu ứng Comboxbox
        /// <summary>
        /// Hiệu ứng khi chọn
        /// </summary>
        private void ComboProductTypes_DropDownOpened(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            comboBox.Background = Brushes.LightGray;
        }

        /// <summary>
        /// Hiệu ứng khi bỏ chọn
        /// </summary>
        private void ComboProductTypes_DropDownClosed(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            comboBox.Background = Brushes.Transparent;
        }
        #endregion

        /// <summary>
        /// Chọn trang bằng Combobox
        /// </summary>
        private void ComboPageIndex_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (products != null)
            {
                // Sắp xếp lại
                for (int i = 0; i < products.Count; i++)
                {
                    for (int j = i; j < products.Count; j++)
                    {
                        if (compare(products[i], products[j]))
                        {
                            Product temp = products[i];
                            products[i] = products[j];
                            products[j] = temp;
                        }
                    }
                }

                listProduct.ItemsSource = products.Skip(comboPageIndex.SelectedIndex * 12).Take(12);
            }
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
        /// Định dạng giá cả
        /// </summary>
        private void Price_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // Chuyển định dạng abc,xyz cho giá cả
            if (textBox.Text.Length > 0)
            {
                double value = 0;
                double.TryParse(textBox.Text, out value);
                textBox.Text = value.ToString("N0");
                textBox.CaretIndex = textBox.Text.Length;
            }
        }
        
        /// <summary>
        /// Quay về trang trước (danh sách sản phẩm)
        /// </summary>
        private void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (products != null)
            {
                if (comboPageIndex.SelectedIndex > 0)
                {
                    listProduct.ItemsSource = products.Skip(--comboPageIndex.SelectedIndex * 12).Take(12);
                }
            }
        }

        /// <summary>
        /// Đi tới trang sau (danh sách sản phẩm)
        /// </summary>
        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if (products != null)
            {
                if (comboPageIndex.SelectedIndex < comboPageIndex.Items.Count - 1)
                {
                    listProduct.ItemsSource = products.Skip(++comboPageIndex.SelectedIndex * 12).Take(12);
                }
            }
        }

        /// <summary>
        /// Nút Export - Xuất dữ liệu ra Excel
        /// </summary>
        private void BtnExportProduct_Click(object sender, RoutedEventArgs e)
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
            worksheet.Name = "Product";

            // Ghi các cột
            worksheet.Cells["A1"].PutValue("TÊN SẢN PHẨM");
            worksheet.Cells["B1"].PutValue("MÃ SẢN PHẨM");
            worksheet.Cells["C1"].PutValue("GIÁ BÁN");
            worksheet.Cells["D1"].PutValue("NGÀY NHẬP");
            worksheet.Cells["E1"].PutValue("TỒN KHO BAN ĐẦU");
            worksheet.Cells["F1"].PutValue("TỒN KHO HIỆN TẠI");
            worksheet.Cells["G1"].PutValue("VỐN BỎ RA");
            worksheet.Cells["H1"].PutValue("MÔ TẢ");
            worksheet.Cells["I1"].PutValue("LOẠI SẢN PHẨM");
            worksheet.Cells["J1"].PutValue("ẢNH SẢN PHẨM");
            for (int i = 0; i < products.Count; i++)
            {
                worksheet.Cells[$"A{i + 2}"].PutValue(products[i].Name);
                worksheet.Cells[$"B{i + 2}"].PutValue(products[i].Id);
                worksheet.Cells[$"C{i + 2}"].PutValue(products[i].Price);
                worksheet.Cells[$"D{i + 2}"].PutValue(products[i].Date.ToString());
                worksheet.Cells[$"E{i + 2}"].PutValue(products[i].InitialAmount);
                worksheet.Cells[$"F{i + 2}"].PutValue(products[i].CurrentAmount);
                worksheet.Cells[$"G{i + 2}"].PutValue(products[i].Capital);
                worksheet.Cells[$"H{i + 2}"].PutValue(products[i].Description);
                worksheet.Cells[$"I{i + 2}"].PutValue(products[i].ProductType);
                worksheet.Cells[$"J{i + 2}"].PutValue(products[i].ImagePath);
            }

            // Lưu lại
            worksheet.AutoFitColumns();
            workbook.Save(filename);
        }

        /// <summary>
        /// Nút Import - Nhập dữ liệu từ Excel
        /// </summary>
        private void BtnImportProduct_Click(object sender, RoutedEventArgs e)
        {
            // Mở hộp thoại mở tập tin
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.DefaultExt = ".xlsx";
            openFileDialog.Filter = "Excel Workbook (.xlsx)|*.xlsx";

            if (false == openFileDialog.ShowDialog()) return;
            string filename = openFileDialog.FileName;

            var excel = new ImportFromExcel(filename);
            excel.Owner = Window.GetWindow(this);
            excel.SendProduct = Import;
            excel.Show();
        }

        /// <summary>
        /// Hàm Import cho delegate
        /// </summary>
        public void Import(ObservableCollection<Product> Data)
        {
            if (Data != null)
            {
                var db = new Management_SystemEntities();
                for (int i = 0; i < Data.Count; i++)
                {
                    try
                    {
                        db.Products.Add(Data[i]);
                        db.SaveChanges();

                        // Cập nhật lên List View
                        products.Add(Data[i]);
                       
                        // Tăng số sản phẩm của loại sản phẩm
                        ProductType type = db.ProductTypes.Find(Data[i].ProductType);
                        type.NumOfProduct++;
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        continue; // Không xảy ra lỗi trùng mã vì đã xử lý trước
                    }
                }
                refresh(true);
            }
        }

        /// <summary>
        /// Thêm một sản phẩm
        /// </summary>
        private void BtnNewProduct_Click(object sender, RoutedEventArgs e)
        {
            var page = new NewProductPage();
            page.RefreshProductList = refresh;
            NavigationService.Navigate(page);
        }

        /// <summary>
        /// Làm mới danh sách sản phẩm (list view)
        /// </summary>
        public void refresh(bool Data)
        {
            if (!Data) return;

            var db = new Management_SystemEntities();
            products = new ObservableCollection<Product>(db.Products);

            // Lọc theo giá
            double priceMin = 0;
            double priceMax = 0;
            double.TryParse(editFilterFrom.Text, out priceMin);
            double.TryParse(editFilterTo.Text, out priceMax);
            if (priceMin > 0 || priceMax > 0)
            {
                products = new ObservableCollection<Product>(products.Where(x => priceMin <= x.Price && x.Price <= priceMax));
            }

            // Nếu lượng thêm vào nhiều và tạo thành trang mới
            int curPage = comboPageIndex.SelectedIndex;
            int newNumPages = (int)Math.Ceiling(products.Count / 12.0);
            List<string> pageNumber = new List<string>();
            for (int j = 1; j <= newNumPages; j++)
            {
                pageNumber.Add(j + "/" + newNumPages);
            }
            comboPageIndex.ItemsSource = pageNumber;
            comboPageIndex.SelectedIndex = curPage;

            // Sắp xếp lại
            for (int i = 0; i < products.Count; i++)
            {
                for (int j = i; j < products.Count; j++)
                {
                    if (compare(products[i], products[j]))
                    {
                        Product temp = products[i];
                        products[i] = products[j];
                        products[j] = temp;
                    }
                }
            }
            listProduct.ItemsSource = products.Skip(comboPageIndex.SelectedIndex * 12).Take(12);
        }

        /// <summary>
        /// Xem một sản phẩm
        /// </summary>
        private void ListProduct_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as ListView).SelectedItem as Product;
            if (item != null)
            {
                // Nếu người dùng đang muốn Pick sản phẩm để tạo hóa đơn
                if (PickProductId != null)
                {
                    PickProductId.Invoke(item);
                    if (NavigationService.CanGoBack) NavigationService.GoBack();
                }
                else
                {
                    var detail = new ProductDetailPage(item);
                    detail.RefreshProductList = refresh;
                    NavigationService.Navigate(detail);
                }
            }
            
        }

        /// <summary>
        /// Tìm kiếm một sản phẩm
        /// </summary>
        private void BtnSearchProduct_Click(object sender, RoutedEventArgs e)
        {
            // Nếu ô tìm kiếm rỗng, thì lấy tất cả sản phẩm
            if (editSearchProduct.Text.Length == 0)
            {
                refresh(true);
            }

            // Nếu ô tìm kiếm có nội dung
            else
            {
                // Tạo mới danh sách sản phẩm có tên chứa nội dung ô tìm kiếm
                ObservableCollection<Product> searchProducts = new ObservableCollection<Product>(); 
                for (int i = 0; i < products.Count; i++)
                {
                    if (products[i].Name.Contains(editSearchProduct.Text)) // Nếu tìm thấy tên phù hợp
                    {
                        searchProducts.Add(products[i]); // Thì thêm vào danh sách mới
                    }
                }

                // Nếu tìm thấy ít nhất 1 sản phẩm thì hiển thị, không thì thông báo
                if (searchProducts.Count > 0)
                {
                    listProduct.ItemsSource = searchProducts;
                }
                else
                {
                    var dialog = new Dialog() { Message = "Không tìm thấy sản phẩm" };
                    dialog.Owner = Window.GetWindow(this);
                    dialog.ShowDialog();
                }

                // Làm trống ô tìm kiếm
                editSearchProduct.Text = "";
            }
        }
    }

    /// <summary>
    /// Hàm chuyển đổi Price cho List View
    /// </summary>
    public class PriceConverter : IValueConverter
    {
        public object Convert(object value, Type TargetType, object parameter, CultureInfo culture)
        {
            double newValue = double.Parse(value.ToString());
            return newValue.ToString("N0");
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}