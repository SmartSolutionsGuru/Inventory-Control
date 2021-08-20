using Caliburn.Micro;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using SmartSolutions.InventoryControl.DAL.Managers.Bussiness_Partner;
using SmartSolutions.InventoryControl.DAL.Managers.Payments;
using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using SmartSolutions.Util.LogUtils;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Windows;
using HorizontalAlignment = iText.Layout.Properties.HorizontalAlignment;
using TextAlignment = iText.Layout.Properties.TextAlignment;

namespace SmartSolutions.InventoryControl.Core.ViewModels.BussinessPartner
{
    [Export(typeof(PartnerBalanceViewModel)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PartnerBalanceViewModel : BaseViewModel
    {
        #region Private Members
        private readonly IPartnerLedgerManager _partnerLedgerManager;
        private readonly IPaymentTypeManager _paymentTypeManager;
        #endregion

        #region Constructor
        [ImportingConstructor]
        public PartnerBalanceViewModel(IPartnerLedgerManager partnerLedgerManager, IPaymentTypeManager paymentTypeManager)
        {
            _partnerLedgerManager = partnerLedgerManager;
            _paymentTypeManager = paymentTypeManager;
        }
        #endregion

        #region Methods
        protected override void OnActivate()
        {
            base.OnActivate();

        }
        public async void GetPartnerBalanceSheet(BussinessPartnerModel selectedPartner)
        {
            try
            {
                if (SelectedPartner != null)
                {
                    var partnerBalnceSheetResult = (await _partnerLedgerManager.GetPartnerBalanceSheetAsync(SelectedPartner?.Id)).ToList();
                    foreach (var partnersheet in partnerBalnceSheetResult)
                    {
                        partnersheet.Partner = SelectedPartner;
                    }
                    PartnerBalanceSheet = new ObservableCollection<BussinessPartnerLedgerModel>(partnerBalnceSheetResult);

                    HeadingText = $"{SelectedPartner.Name} Balance Sheet";
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }

        public string ConvertToExcel(System.Data.DataTable DT, string title)
        {
            //null guard
            if (DT == null) return null;
            try
            {
                // Create Excel
                Microsoft.Office.Interop.Excel.Application ExcelApp = new Microsoft.Office.Interop.Excel.Application();
                Microsoft.Office.Interop.Excel.Workbook ExcelBook = ExcelApp.Workbooks.Add(Type.Missing);
                // Create a worksheet (that is, a subsheet in Excel) 1 to represent data export in the subsheet 1
                Microsoft.Office.Interop.Excel.Worksheet ExcelSheet = (Microsoft.Office.Interop.Excel.Worksheet)ExcelBook.Worksheets[1];
                // If there is a numeric type in the data, it can be displayed in a different text format.
                ExcelSheet.Cells.NumberFormat = "@";
                // Setting the name of the worksheet
                //ExcelSheet.Name = title;
                ExcelSheet.Name = $"{SelectedPartner.Name} Balance Sheet";
                // Setting Sheet Title
                string start = "A1";
                string end = ChangeASC(DT.Columns.Count) + "1";
                Microsoft.Office.Interop.Excel.Range _Range = (Microsoft.Office.Interop.Excel.Range)ExcelSheet.get_Range(start, end);

                _Range.Merge(0); // cell merge action (designed with get_Range() above)
                _Range = (Microsoft.Office.Interop.Excel.Range)ExcelSheet.get_Range(start, end);
                _Range.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                _Range.Font.Size = 22; // Set font size
                                       // _Range.Font.Name = SongStyle; // Types of fonts set 
                ExcelSheet.Cells[1, 1] = title; //Excel cell assignment
                _Range.EntireColumn.AutoFit(); // Auto-adjust column widths
                // write the header.
                for (int m = 1; m <= DT.Columns.Count; m++)
                {
                    ExcelSheet.Cells[2, m] = DT.Columns[m - 1].ColumnName.ToString();
                    start = "A2";
                    end = ChangeASC(DT.Columns.Count) + "2";
                    _Range = (Microsoft.Office.Interop.Excel.Range)ExcelSheet.get_Range(start, end);
                    _Range.Font.Size = 14; // Set font size
                                           // _Range.Font.Name = SongStyle; // Types of fonts set  
                    _Range.EntireColumn.AutoFit(); // Auto-adjust column widths 
                    _Range.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                }
                // write data
                for (int i = 0; i < DT.Rows.Count; i++)
                {
                    for (int j = 1; j <= DT.Columns.Count; j++)
                    {
                        // Excel cells begin with index 1
                        // if (j == 0) j = 1;
                        ExcelSheet.Cells[i + 3, j] = DT.Rows[i][j - 1].ToString();
                    }
                }
                // Table property settings
                for (int n = 0; n < DT.Rows.Count + 1; n++)
                {
                    start = "A" + (n + 3).ToString();
                    end = ChangeASC(DT.Columns.Count) + (n + 3).ToString();
                    // Get Excel multiple cell areas
                    _Range = (Microsoft.Office.Interop.Excel.Range)ExcelSheet.get_Range(start, end);
                    _Range.Font.Size = 12; // Set font size
                    //_Range.Font.Name = SongStyle; // Types of fonts set
                    _Range.EntireColumn.AutoFit(); // Auto-adjust column widths
                    _Range.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter; //设置字体在单元格内的对其方式 _ Range. EntireColumn. AutoFit (); // Auto-adjust column widths 
                }
                ExcelApp.DisplayAlerts = false; // Save Excel without popping up a window to save it directly 
                //// Pop up the save dialog box and save the file
                Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
                sfd.DefaultExt = ".xlsx";
                sfd.Filter = "Office 2007 File |*. xlsx | Office 2000-2003 File |*. xls | All Files |*. *";
                if (sfd.ShowDialog() == true)
                {
                    if (sfd.FileName != "")
                    {
                        ExcelBook.SaveAs(sfd.FileName); // Save it to the specified path
                        //System.Windows.MessageBox.Show("export file has been stored as:" + sfd.FileName, "warm reminder");
                        IoC.Get<IDialogManager>().ShowMessageBox($"Export File has been stored as: {sfd.FileName}", options: Dialogs.MessageBoxOptions.Ok);
                    }
                }
                // The process of release that may not have been released
                ExcelBook.Close();
                ExcelApp.Quit();
                return sfd.FileName;
            }
            catch
            {
                // System. Windows. MessageBox. Show ("Export file failed to save, probably because the file was opened! Warning! "";
                NotificationManager.Show(new Notifications.Wpf.NotificationContent { Title = "Error", Message = "Export file failed to save", Type = Notifications.Wpf.NotificationType.Error });
                return null;
            }
        }

        public void ConvertToPdf(System.Data.DataTable DT, string title)
        {
            try
            {
                ExportToPDF(DT, SelectedPartner.Name);
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        public void Print()
        {
            try
            {

            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }

        public void Close()
        {
            TryClose();
        }
        #endregion

        #region Private Helpers
        /// <summary>
        ///  Get the current column name and get the corresponding column in EXCEL
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private string ChangeASC(int count)
        {
            string ascstr = "";
            switch (count)
            {
                case 1:
                    ascstr = "A";
                    break;
                case 2:
                    ascstr = "B";
                    break;
                case 3:
                    ascstr = "C";
                    break;
                case 4:
                    ascstr = "D";
                    break;
                case 5:
                    ascstr = "E";
                    break;
                case 6:
                    ascstr = "F";
                    break;
                case 7:
                    ascstr = "G";
                    break;
                case 8:
                    ascstr = "H";
                    break;
                case 9:
                    ascstr = "I";
                    break;
                case 10:
                    ascstr = "J";
                    break;
                case 11:
                    ascstr = "K";
                    break;
                case 12:
                    ascstr = "L";
                    break;
                case 13:
                    ascstr = "M";
                    break;
                case 14:
                    ascstr = "N";
                    break;
                case 15:
                    ascstr = "O";
                    break;
                case 16:
                    ascstr = "P";
                    break;
                case 17:
                    ascstr = "Q";
                    break;
                case 18:
                    ascstr = "R";
                    break;
                case 19:
                    ascstr = "S";
                    break;
                case 20:
                    ascstr = "T";
                    break;
                default:
                    ascstr = "U";
                    break;
            }
            return ascstr;
        }

        private Document document;
        public string ExportToPDF(System.Data.DataTable DT, string title)
        {
            if (DT == null) return null;
            try
            {
                var stream = new MemoryStream();
                // Create PDF Writer
                PdfWriter writer = new PdfWriter(stream);

                //Initialize PDF document
                PdfDocument pdf = new PdfDocument(writer);
                // Creates a PDF document
                document = new Document(pdf, PageSize.A4);
                document.SetMargins(0, 0, 15, 5);

                PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                PdfFont bold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                Table table = new Table(UnitValue.CreatePercentArray(new float[] { DT.Columns.Count }));
                table.SetTextAlignment(alignment: TextAlignment.CENTER)
                    .SetHorizontalAlignment(horizontalAlignment: HorizontalAlignment.CENTER);
                document.Add(table);

                for (int i = 0; i < DT.Columns.Count; i++)
                {
                    var cellName = $"cell {i}";
                    Cell cellname = new Cell(i, DT.Columns.Count).Add(new Paragraph($"{i}"))
                        .SetFont(font)
                        .SetFontSize(13)
                        .SetTextAlignment(TextAlignment.CENTER);
                    table.AddHeaderCell(cellName);
                }
                for (int i = 0; i < DT.Rows.Count; i++)
                {
                    for (int j = 1; j < DT.Columns.Count; j++)
                    {
                        table.AddCell(new Cell().SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph(DT.Columns[j].ToString())));
                    }
                }
                Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
                sfd.DefaultExt = ".pdf";
                sfd.Filter = "PDF document (*.pdf)|*.pdf";
                if (sfd.ShowDialog() == true)
                {
                    if (sfd.FileName != "")
                    {
                        document.Add(table);
                        document.Close();
                         
                        //document.Save(sfd.FileName); // Save it to the specified path
                        IoC.Get<IDialogManager>().ShowMessageBox($"Export File has been stored as: {sfd.FileName}", options: Dialogs.MessageBoxOptions.Ok);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return title;
        }

        #endregion

        #region Properties
        private BussinessPartnerModel _SelectedPartner;
        /// <summary>
        /// Selected Partner 
        /// </summary>
        public BussinessPartnerModel SelectedPartner
        {
            get { return _SelectedPartner; }
            set { _SelectedPartner = value; NotifyOfPropertyChange(nameof(SelectedPartner)); }
        }

        private ObservableCollection<BussinessPartnerLedgerModel> _PartnerBalanceSheet;
        /// <summary>
        /// List for Displaying Partner Balance
        /// </summary>
        public ObservableCollection<BussinessPartnerLedgerModel> PartnerBalanceSheet
        {
            get { return _PartnerBalanceSheet; }
            set { _PartnerBalanceSheet = value; NotifyOfPropertyChange(nameof(PartnerBalanceSheet)); }
        }

        //private List<BussinessPartnerLedgerModel> _PartnerBalanceSheet;

        //public List<BussinessPartnerLedgerModel> PartnerBalanceSheet
        //{
        //    get { return _PartnerBalanceSheet; }
        //    set { _PartnerBalanceSheet = value; NotifyOfPropertyChange(nameof(PartnerBalanceSheet)); }
        //}

        private BussinessPartnerLedgerModel _PartnerSelectedBalance;
        /// <summary>
        /// Partner  Selected  Balance 
        /// </summary>
        public BussinessPartnerLedgerModel PartnerSelectedBalance
        {
            get { return _PartnerSelectedBalance; }
            set { _PartnerSelectedBalance = value; NotifyOfPropertyChange(nameof(PartnerSelectedBalance)); }
        }

        private string _HeadingText;
        /// <summary>
        /// Heading Text With Partner Name
        /// </summary>
        public string HeadingText
        {
            get { return _HeadingText; }
            set { _HeadingText = value; NotifyOfPropertyChange(nameof(HeadingText)); }
        }

        #endregion
    }
}
