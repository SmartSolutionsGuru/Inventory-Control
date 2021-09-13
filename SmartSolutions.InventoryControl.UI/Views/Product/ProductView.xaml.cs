using Microsoft.Win32;
using SmartSolutions.InventoryControl.UI.Helpers;
using SmartSolutions.InventoryControl.UI.Helpers.Image;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace SmartSolutions.InventoryControl.UI.Views.Product
{
    /// <summary>
    /// Interaction logic for ProductView.xaml
    /// </summary>
    public partial class ProductView : UserControl
    {
        public Core.ViewModels.Product.ProductViewModel ViewModel { get; set; }
        public ProductView()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ViewModel = DataContext as Core.ViewModels.Product.ProductViewModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "Image files (*.bmp, *.jpg,*.png)|*.bmp;*.jpg|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileNames.FirstOrDefault();
                var encoder = new PngBitmapEncoder();
                var Image = new BitmapImage(new Uri(fileName,UriKind.Relative));
                encoder.Frames.Add(BitmapFrame.Create(Image));
                using (var stram = new FileStream("Testing",FileMode.Create,FileAccess.Write))
                {
                    encoder.Save(stram);
                }
                ViewModel.ProductImage = Image?.ToByteArray();
                ViewModel.ImageName = System.IO.Path.GetFileName(fileName);
            }
        }
    }
}
