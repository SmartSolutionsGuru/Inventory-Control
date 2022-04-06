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

namespace SmartSolutions.InventoryControl.UI.Views.BussinessPartner
{
    /// <summary>
    /// Interaction logic for BussinessPartnerView.xaml
    /// </summary>
    public partial class BussinessPartnerView : UserControl
    {
        public Core.ViewModels.BussinessPartner.BussinessPartnerViewModel ViewModel { get; set; }
        public BussinessPartnerView()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ViewModel = DataContext as Core.ViewModels.BussinessPartner.BussinessPartnerViewModel;
        }

        private void SearchableComboBox_SearchCounter(object sender, EventArgs e)
        {
            try
            {
                var control = sender as TextBox;
                if (string.IsNullOrEmpty(control?.Text)) return;
                ViewModel?.FilterCity(control?.Text);
                
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
    }
}
