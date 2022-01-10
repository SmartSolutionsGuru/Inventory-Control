using Microsoft.Win32;
using SmartSolutions.InventoryControl.DAL.Models.Product;
using SmartSolutions.InventoryControl.UI.Helpers;
using SmartSolutions.Util.LogUtils;
using System;
using System.Collections;
using System.Collections.Generic;
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

namespace SmartSolutions.InventoryControl.UI.Views
{
    /// <summary>
    /// Interaction logic for SalesView.xaml
    /// </summary>
    public partial class SalesView : UserControl
    {
        public Core.ViewModels.SalesViewModel ViewModel { get; set; }
        public SalesView()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ViewModel = DataContext as Core.ViewModels.SalesViewModel;
        }

        private void BrowsePicture_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "Image files (*.bmp, *.jpg, *.jpeg)|*.bmp;*.jpg|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileNames.FirstOrDefault();
                BitmapImage Image = new BitmapImage(new Uri(fileName));
                ViewModel.PaymentImage = Image?.ToByteArray();
            }
        }

        private void AutoCompleteTextBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var control = sender as TextBox;
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }

        private void SearchableComboBox_SearchCounter(object sender, EventArgs e)
        {
            try
            {
                var control = sender as TextBox;
                if (string.IsNullOrEmpty(control?.Text)) return;
                ViewModel.FilterPartners(control?.Text);
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }

        private void ProductSearchCounter(object sender, EventArgs e)
        {
            try
            {
                var control = sender as TextBox;
                var text = control?.Text;
                //if (string.IsNullOrEmpty(text)) return;
                ViewModel.FilterProducts(text);
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var control = sender as ListView;
                var resultAddItem = e.AddedItems;
                if(resultAddItem[0] is SmartSolutions.InventoryControl.DAL.Models.Product.ProductModel == true)
                {
                    ViewModel.SelectedInventoryProduct.Product = resultAddItem[0] as ProductModel;
                }
                if(ViewModel?.SelectedInventoryProduct.Product.Id != null)
                {
                    ViewModel.GetProductAvailableStock(ViewModel?.SelectedInventoryProduct?.Product?.Id ?? 0);
                }
                //if(ViewModel.SelectedInventoryProduct.ProductSize != null 
                //    && ViewModel.SelectedInventoryProduct.ProductColor != null 
                //    && ViewModel.SelectedInventoryProduct.Product != null)
                //{
                //    ViewModel.SelectedInventoryProduct.Product.ProductColor = ViewModel?.SelectedInventoryProduct?.ProductColor;
                //    ViewModel.SelectedInventoryProduct.Product.ProductSize = ViewModel?.SelectedInventoryProduct?.ProductSize;
                //    ViewModel.GetProductAvailableStock(ViewModel.SelectedInventoryProduct?.Product?.Id ?? 0);
                //}  
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
    }
}
