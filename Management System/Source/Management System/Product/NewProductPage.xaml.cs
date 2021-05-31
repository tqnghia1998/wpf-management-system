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
    /// Interaction logic for AddProduct.xaml
    /// </summary>
    public partial class NewProductPage : Page
    {
        // Cờ xác định vào để sửa hay thêm
        public bool isEdit = false;

        // Delegate để refresh danh sách sản phẩm
        public delegate void DelegateRefeshProductList(bool Data);
        public DelegateRefeshProductList RefreshProductList;

        public NewProductPage()
        {
            InitializeComponent();

            // Get và hiển thị danh sách loại sản phẩm
            Thread getPTypes = new Thread(delegate ()
            {
                var db = new Management_SystemEntities();
                var productTypes = new ObservableCollection<string>(db.ProductTypes.Select(x => x.Name));
                Dispatcher.Invoke(() => {
                    comboProductTypes.ItemsSource = productTypes; // Tác động lên UI
                });
            });
            getPTypes.Start();
        }

        public NewProductPage(Product product)
        {
            InitializeComponent();
            isEdit = true;
            imgProduct.Tag = product.ImagePath;

            // Đưa thông tin lên UI
            Title.Content = "SỬA SẢN PHẨM";
            editProductName.Text = product.Name;
            editProductId.Text = product.Id;
            editProductId.IsEnabled = false;
            editProductPrice.Text = product.Price.ToString("N0");
            // editProductType.Text = product.ProductType;
            if (product.Description != null) editProductDescription.Text = product.Description;
            editProductDate.Text = product.Date.ToString();
            editProductInitialAmount.Text = "0"; // Đây chỉ là lượng sẽ thêm vào (không phải lượng đã có)
            editProductCapital.Text = "0"; // Đây chỉ là vốn bỏ ra cho lượng sẽ thêm vào (không phải vốn ban đầu)
            if (product.ImagePath != null)
            {
                BitmapImage source = new BitmapImage(new Uri(product.ImagePath));
                imgProduct.Source = source;
            }

            // Lấy tên loại sản phẩm tương ứng
            Thread thread = new Thread(delegate ()
            {
                // Lấy loại sản phẩm tương ứng
                var db = new Management_SystemEntities();
                ProductType target = db.ProductTypes.Find(product.ProductType);

                // Trước tiên lấy toàn bộ loại sản phẩm đưa vào Combobox
                var productTypes = new ObservableCollection<string>(db.ProductTypes.Select(x => x.Name));
                Dispatcher.Invoke(() => {
                    comboProductTypes.ItemsSource = productTypes;

                    // Sau đó dò xem item nào đúng thì chọn
                    for (int i = 0; i < productTypes.Count; i++)
                    {
                        if (productTypes[i].Equals(target.Name))
                        {
                            comboProductTypes.SelectedIndex = i; break;
                        }
                    }
                }); 
            });
            thread.Start();
        }

        #region Xử lý hiệu ứng Comboxbox
        /// <summary>
        /// Hiệu ứng khi chọn
        /// </summary>
        private void ComboProductTypes_DropDownOpened(object sender, EventArgs e)
        {
            comboProductTypes.Background = Brushes.LightGray;
        }

        /// <summary>
        /// Hiệu ứng khi bỏ chọn
        /// </summary>
        private void ComboProductTypes_DropDownClosed(object sender, EventArgs e)
        {
            comboProductTypes.Background = Brushes.Transparent;
        }
        #endregion

        /// <summary>
        /// Xử lý ô giá chỉ nhận giá trị số
        /// </summary>
        private void NumberOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        /// <summary>
        /// Thêm một loại sản phẩm
        /// </summary>
        private void BtnAddProductType_Click(object sender, RoutedEventArgs e)
        {
            var typeW = new ProductTypePage();
            typeW.RefreshProductTypeList = refreshCombo;
            NavigationService.Navigate(typeW);
        }

        /// <summary>
        /// Thêm ảnh sản phẩm
        /// </summary>
        private void BtnAddImageProduct_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

            if (true == openFileDialog.ShowDialog())
            {
                string filename = openFileDialog.FileName;
                try
                {
                    BitmapImage source = new BitmapImage(new Uri(filename));
                    imgProduct.Source = source;
                    imgProduct.Tag = filename;
                }
                catch (Exception ex)
                {
                    var dialogError = new Dialog() { Message = "Tập tin ảnh không hợp lệ" };
                    dialogError.Owner = Window.GetWindow(this);
                    dialogError.ShowDialog();
                    return;
                }
            }
        }

        /// <summary>
        /// Nút Lưu (có thao tác với CSDL)
        /// </summary>
        private void BtnAddProductSave_Click(object sender, RoutedEventArgs e)
        {
            // Kiểm tra dữ liệu có nhập đầy đủ không
            if (editProductName.Text.Length == 0
                || editProductId.Text.Length == 0
                || editProductPrice.Text.Length == 0
                || editProductInitialAmount.Text.Length == 0
                || editProductCapital.Text.Length == 0
                || editProductDate.Text.Length == 0
                || comboProductTypes.SelectedIndex == -1)
            {
                var dialogError1 = new Dialog() { Message = "Vui lòng nhập đầy đủ các thông tin" };
                dialogError1.Owner = Window.GetWindow(this);
                dialogError1.ShowDialog();
                return;
            }

            // Hiển thị thông báo xác nhận
            var dialogError = new Dialog() { Message = isEdit ? "Xác nhận sửa sản phẩm?" : "Thêm mới sản phẩm đã nhập?" };
            dialogError.Owner = Window.GetWindow(this);
            if (true == dialogError.ShowDialog())
            {
                try
                {
                    var db = new Management_SystemEntities();

                    // Tạo đối tượng Product tương ứng
                    var product = new Product()
                    {
                        Name = editProductName.Text,
                        Id = editProductId.Text,
                        Price = (long) double.Parse(editProductPrice.Text),
                        Date = DateTime.Parse(editProductDate.Text),
                        InitialAmount = int.Parse(editProductInitialAmount.Text),
                        CurrentAmount = int.Parse(editProductInitialAmount.Text),
                        Capital = (long) double.Parse(editProductCapital.Text),
                        Description = editProductDescription.Text.Length == 0 ? null : editProductDescription.Text,
                        ImagePath = imgProduct.Tag == null ? null: imgProduct.Tag.ToString()
                        // Còn thiếu trường ProductType
                    };

                    // Tìm Id của loại sản phẩm đã chọn
                    ProductType type = db.ProductTypes.FirstOrDefault(x => x.Name == comboProductTypes.Text);
                    if (type != null)
                    {
                        product.ProductType = type.Id;

                        // Nếu sửa
                        if (isEdit)
                        {
                            var oldProduct = db.Products.Find(editProductId.Text);
                            oldProduct.Name = product.Name;
                            oldProduct.Price = product.Price;
                            oldProduct.Description = product.Description;
                            oldProduct.ImagePath = product.ImagePath;
                            oldProduct.Date = product.Date;
                            oldProduct.CurrentAmount += product.CurrentAmount; // Thêm lượng mới nhập vào cả tồn kho ban đầu và tồn kho hiện tại
                            oldProduct.InitialAmount += product.InitialAmount;
                            oldProduct.Capital += product.Capital;  // Giá vốn cũng thêm vào
                            if (oldProduct.ProductType != type.Id) // Nếu có thay đổi mã sản phẩm
                            {
                                type.NumOfProduct++; // Tăng mã mới
                                ProductType oldType = db.ProductTypes.Find(oldProduct.ProductType);
                                oldType.NumOfProduct--; // Giảm mã cũ
                                oldProduct.ProductType = product.ProductType;
                            }
                        }

                        // Nếu thêm
                        else // Nếu thêm
                        {
                            db.Products.Add(product);
                            type.NumOfProduct++;
                        }
                        db.SaveChanges();
                    }

                    // Nếu trang vừa rồi là danh sách sản phẩm thì cập nhật nó
                    if (RefreshProductList != null)
                    {
                        RefreshProductList.Invoke(true);
                    }
                }
                catch (Exception ex)
                {
                    var dialogError1 = new Dialog() { Message = "Mã sản phẩm đã tồn tại" };
                    dialogError1.Owner = Window.GetWindow(this);
                    dialogError1.ShowDialog();
                }
            }
        }

        /// <summary>
        /// Nút làm mới
        /// </summary>
        private void BtnRefesh_Click(object sender, RoutedEventArgs e)
        {
            editProductName.Clear();
            editProductId.Clear();
            editProductPrice.Clear();
            editProductDescription.Clear();
            editProductDate.Text = null;
            editProductInitialAmount.Clear();
            editProductCapital.Clear();
            comboProductTypes.SelectedIndex = -1;
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri("pack://application:,,/Images/NewProductPage/Image.png");
            image.EndInit();
            imgProduct.Source = image;
            imgProduct.Tag = null;
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
        /// Làm mới combobox loại sản phẩm
        /// </summary>
        public void refreshCombo(bool Data)
        {
            if (Data) // Nếu có chỉnh sửa danh sách loại sản phẩm thì refresh combo
            {
                int oldIndex = comboProductTypes.SelectedIndex;

                // Get và hiển thị danh sách loại sản phẩm
                Thread getPTypes = new Thread(delegate ()
                {
                    var db = new Management_SystemEntities();
                    var productTypes = new ObservableCollection<string>(db.ProductTypes.Select(x => x.Name));
                    Dispatcher.Invoke(() => {
                        comboProductTypes.ItemsSource = productTypes; // Tác động lên UI
                        if (oldIndex > 0) comboProductTypes.SelectedIndex = oldIndex;

                        // Cập nhật tiếp trang ở trước
                        if (RefreshProductList != null)
                        {
                            RefreshProductList.Invoke(true);
                        }
                    });
                });
                getPTypes.Start();
            }
        }
    }
}