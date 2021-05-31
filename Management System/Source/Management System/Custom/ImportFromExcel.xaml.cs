using Aspose.Cells;
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
using System.Windows.Shapes;

namespace Management_System
{
    /// <summary>
    /// Interaction logic for ImportFromExcel.xaml
    /// </summary>
    public partial class ImportFromExcel : Window
    {
        // Khởi tạo danh sách sản phẩm và loại sản phẩm
        ObservableCollection<Product> products;
        ObservableCollection<ProductType> productTypes;
        public string filename;

        // Delegate nhận dữ liệu từ cửa sổ Import
        public delegate void DelegateSendProductType(ObservableCollection<ProductType> Data);
        public DelegateSendProductType SendProductType;
        public delegate void DelegateSendProduct(ObservableCollection<Product> Data);
        public DelegateSendProduct SendProduct;

        public ImportFromExcel(string _filename)
        {
            InitializeComponent();
            filename = _filename;
        }

        /// <summary>
        /// Di chuyển cửa sổ
        /// </summary>
        private void ColorZone_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        /// <summary>
        /// Tắt cửa sổ thông báo
        /// </summary>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Đọc dữ liệu từ tập tin Excel
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Left = Owner.Left + Owner.Width / 2 - Width / 2;
            Top = this.Owner.Top + Owner.Height / 2 - Height / 2;

            Thread thread = new Thread(delegate ()
            {
                // Mở Excel và đọc
                Workbook workbook = new Workbook(filename);
                Worksheet worksheet = workbook.Worksheets[0];
                var db = new Management_SystemEntities();

                // Nếu import loại sản phẩm
                if (worksheet.Name.Equals("Product Type"))
                {
                    productTypes = new ObservableCollection<ProductType>();

                    // Bắt đầu từ hàng thứ 2
                    int i = 2;
                    while (worksheet.Cells[$"B{i}"].Value != null)
                    {
                        // Nếu dữ liệu đã tồn tại thì thôi
                        if (db.ProductTypes.Find(worksheet.Cells[$"B{i}"].Value.ToString()) != null)
                        {
                            i++;
                            continue;
                        }

                        // Kiểm tra tên, ngày có trống không
                        if (worksheet.Cells[$"A{i}"].Value == null
                            || worksheet.Cells[$"D{i}"].Value == null)
                        {
                            i++;
                            continue;
                        }

                        // Kiểm tra ngày có đúng định dạng không
                        string date = worksheet.Cells[$"D{i}"].Value.ToString();
                        DateTime dateTime = new DateTime();
                        try
                        {
                            dateTime = DateTime.Parse(date);
                        }
                        catch (Exception ex)
                        {
                            i++;
                            continue;
                        }

                        // Tới đây được tức có dữ liệu đã đúng
                        ProductType type = new ProductType()
                        {
                            Name = worksheet.Cells[$"A{i}"].Value.ToString(),
                            Id = worksheet.Cells[$"B{i}"].Value.ToString(),
                            Description = worksheet.Cells[$"C{i}"].Value == null ? null : worksheet.Cells[$"C{i}"].Value.ToString(),
                            Date = dateTime
                        };
                        productTypes.Add(type);
                        i++;
                    }

                    // Cập nhật UI
                    Dispatcher.Invoke(() => {
                        itemsControl.ItemsSource = productTypes;
                    });
                }
                // Nếu import sản phẩm
                else if (worksheet.Name.Equals("Product"))
                {
                    products = new ObservableCollection<Product>();

                    // Bắt đầu từ hàng thứ 2
                    int i = 2;
                    while (worksheet.Cells[$"B{i}"].Value != null)
                    {
                        // Nếu dữ liệu đã tồn tại thì thôi
                        if (db.Products.Find(worksheet.Cells[$"B{i}"].Value.ToString()) != null)
                        {
                            i++;
                            continue;
                        }

                        // Nếu loại sản phẩm không tồn tại thì thôi
                        if (db.ProductTypes.Find(worksheet.Cells[$"I{i}"].Value.ToString()) == null)
                        {
                            i++;
                            continue;
                        }

                        // Kiểm tra các cột khác có trống không (trừ MÔ TẢ, LOẠI SP và ẢNH SP)
                        if (worksheet.Cells[$"A{i}"].Value == null
                            || worksheet.Cells[$"C{i}"].Value == null
                            || worksheet.Cells[$"D{i}"].Value == null
                            || worksheet.Cells[$"E{i}"].Value == null
                            || worksheet.Cells[$"F{i}"].Value == null
                            || worksheet.Cells[$"G{i}"].Value == null
                            || worksheet.Cells[$"J{i}"].Value == null)
                        {
                            i++;
                            continue;
                        }

                        // Kiểm tra ngày có đúng định dạng không
                        string date = worksheet.Cells[$"D{i}"].Value.ToString();
                        DateTime dateTime = new DateTime();
                        try
                        {
                            dateTime = DateTime.Parse(date);
                        }
                        catch (Exception ex)
                        {
                            i++;
                            continue;
                        }

                        // Tới đây được tức có dữ liệu đã đúng
                        try
                        {
                            Product product = new Product()
                            {
                                Name = worksheet.Cells[$"A{i}"].Value.ToString(),
                                Id = worksheet.Cells[$"B{i}"].Value.ToString(),
                                Price = long.Parse(worksheet.Cells[$"C{i}"].Value.ToString()),
                                Date = dateTime,
                                InitialAmount = int.Parse(worksheet.Cells[$"E{i}"].Value.ToString()),
                                CurrentAmount = int.Parse(worksheet.Cells[$"F{i}"].Value.ToString()),
                                Capital = long.Parse(worksheet.Cells[$"G{i}"].Value.ToString()),
                                Description = worksheet.Cells[$"H{i}"].Value == null ? null : worksheet.Cells[$"H{i}"].Value.ToString(),
                                ProductType = worksheet.Cells[$"I{i}"].Value.ToString(),
                                ImagePath = worksheet.Cells[$"J{i}"].Value == null ? null : worksheet.Cells[$"J{i}"].Value.ToString()
                            };
                            products.Add(product);
                        }
                        catch (Exception) { }
                        i++;
                        continue;
                    }

                    // Cập nhật UI
                    Dispatcher.Invoke(() => {
                        itemsControl.ItemsSource = products;
                    });
                }

                // Nếu không có dữ liệu nào có thể import thì thông báo
                Dispatcher.Invoke(() => {
                    if (itemsControl.Items.Count == 0) emptyAnnounce.Visibility = Visibility.Visible;
                    ProgressBar.IsEnabled = false;
                    ProgressBar.Visibility = Visibility.Hidden;
                });
            });
            thread.Start();
        }

        /// <summary>
        /// Thêm dữ liệu
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (SendProductType != null)
            {
                SendProductType.Invoke(productTypes);
            }
            if (SendProduct != null)
            {
                SendProduct.Invoke(products);
            }
            this.Close();
        }
    }
}
