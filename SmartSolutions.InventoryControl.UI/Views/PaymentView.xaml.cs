using Microsoft.Win32;
using SmartSolutions.InventoryControl.UI.Helpers;
using SmartSolutions.Util.LogUtils;
using System;
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
    /// Interaction logic for PaymentView.xaml
    /// </summary>
    public partial class PaymentView : UserControl
    {
        public Core.ViewModels.PaymentViewModel ViewModel { get; set; }
        public PaymentView()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ViewModel = DataContext as Core.ViewModels.PaymentViewModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Multiselect = false;
                openFileDialog.Filter = "Image files (*.bmp, *.jpg)|*.bmp;*.jpg|All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == true)
                {
                    string fileName = openFileDialog.FileNames.FirstOrDefault();
                    BitmapImage Image = new BitmapImage(new Uri(fileName));
                    ViewModel.PaymentImage = Image?.ToByteArray();
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
    }
}
