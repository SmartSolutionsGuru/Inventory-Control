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
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        private void AutoCompleteTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ViewModel?.IsPartnerSelected();
        }
        /// <summary>
        /// text Box For Total for Invoice
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var control = sender as TextBox;
            if (!string.IsNullOrEmpty(control.Text))
            {
                System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");
                var valueBefore = Int64.Parse(control.Text, System.Globalization.NumberStyles.AllowThousands);
                control.Text = String.Format(culture, "{0:N0}", valueBefore);
                control.Select(control.Text.Length, 0);
            }
        }
        /// <summary>
        /// Grand total TextBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            var control = sender as TextBox;
            if (!string.IsNullOrEmpty(control.Text))
            {
                System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");
                var valueBefore = Int64.Parse(control.Text, System.Globalization.NumberStyles.AllowThousands);
                control.Text = String.Format(culture, "{0:N0}", valueBefore);
                control.Select(control.Text.Length, 0);
            }
        }
        /// <summary>
        /// TextBox For Invoice total
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged_2(object sender, TextChangedEventArgs e)
        {
            var control = sender as TextBox;
            if (!string.IsNullOrEmpty(control.Text))
            {
                System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");
                var valueBefore = Int64.Parse(control.Text, System.Globalization.NumberStyles.AllowThousands);
                control.Text = String.Format(culture, "{0:N0}", valueBefore);
                control.Select(control.Text.Length, 0);
            }
        }
        /// <summary>
        /// TextBox for Payment
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged_3(object sender, TextChangedEventArgs e)
        {
            var control = sender as TextBox;
            if (!string.IsNullOrEmpty(control.Text))
            {
                System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");
                var valueBefore = Int64.Parse(control.Text, System.Globalization.NumberStyles.AllowThousands);
                control.Text = String.Format(culture, "{0:N0}", valueBefore);
                control.Select(control.Text.Length, 0);
            }
        }
    }
}
