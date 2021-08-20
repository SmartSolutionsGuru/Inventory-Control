using SmartSolutions.Util.LogUtils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
using SmartSolutions.Util.ValidUtils;
using SmartSolutions.InventoryControl.Core.ViewModels.BussinessPartner;
using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;

namespace SmartSolutions.InventoryControl.UI.Views.BussinessPartner
{
    /// <summary>
    /// Interaction logic for PartnerBalanceView.xaml
    /// </summary>
    public partial class PartnerBalanceView : UserControl
    {
        #region Public Members
        public PartnerBalanceViewModel ViewModel { get; set; }
        #endregion

        
        public PartnerBalanceView()
        {
            InitializeComponent();
            DataContextChanged += PartnerBalanceView_DataContextChanged;
        }

        private void PartnerBalanceView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ViewModel = DataContext as Core.ViewModels.BussinessPartner.PartnerBalanceViewModel; 
        }

        private void ConvertToExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var balaneSheet = ViewModel.PartnerBalanceSheet;
                //DataTable newTB = new DataTable();
                //ObservableCollection instore = this.dataGrid.ItemsSource as ObservableCollection;
                //List list = new List(instore.ToList());
                //newTB = XTools.XHelper.Datavalidation.CopyToDataTable(list);
                //Excel Export(newTB, "Storage Record");

                DataTable newTB = new DataTable();
                ObservableCollection<BussinessPartnerLedgerModel> instore = PartnerBalanceSheet.ItemsSource as ObservableCollection<BussinessPartnerLedgerModel>;
                List<BussinessPartnerLedgerModel> list = new List<BussinessPartnerLedgerModel>(instore);
                newTB = ExcelDatavalidation.CopyToDataTable<BussinessPartnerLedgerModel>(list);

                ViewModel.ConvertToExcel(newTB,"Storage Record"); 
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }

        private void ConvertToPdf_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataTable newTB = new DataTable();
                ObservableCollection<BussinessPartnerLedgerModel> instore = PartnerBalanceSheet.ItemsSource as ObservableCollection<BussinessPartnerLedgerModel>;
                List<BussinessPartnerLedgerModel> list = new List<BussinessPartnerLedgerModel>(instore);
                newTB = ExcelDatavalidation.CopyToDataTable<BussinessPartnerLedgerModel>(list);

                ViewModel.ConvertToPdf(newTB,string.Empty);
                //var pdfConverter = new SmartSolutions.Util.PdfUtils.PdfUtility();
                //pdfConverter.ExportToPDF(newTB, "Storage Record");

            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
    }
}
