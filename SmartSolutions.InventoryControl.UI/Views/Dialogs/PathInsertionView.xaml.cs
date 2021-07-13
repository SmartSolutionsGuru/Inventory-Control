using Microsoft.Win32;
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

namespace SmartSolutions.InventoryControl.UI.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for PathInsertionView.xaml
    /// </summary>
    public partial class PathInsertionView : UserControl
    {
        public Core.ViewModels.Dialogs.PathInsertionViewModel ViewModel { get; set; }
        public PathInsertionView()
        {
            InitializeComponent();
            DataContextChanged += (s, e) => { ViewModel = DataContext as Core.ViewModels.Dialogs.PathInsertionViewModel; };
        }

        private void BrowsePath_Click(object sender, RoutedEventArgs e)
        {
            try
            {

               var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
                if(dialog.ShowDialog().GetValueOrDefault())
                {
                    FolderPathTextBox.Text = dialog.SelectedPath;
                    if (!string.IsNullOrEmpty(FolderPathTextBox.Text))
                        ViewModel.BackupPath = FolderPathTextBox.Text;
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
    }
}
