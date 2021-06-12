using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace SmartSolutions.InventoryControl.UI.CustomControls
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:GuardStation.Controls"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:GuardStation.Controls;assembly=GuardStation.Controls"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:SearchableComboBox/>
    ///
    /// </summary>
    [TemplatePart(Name = "PART_CurrentList", Type = typeof(ListBox))]
    [TemplatePart(Name = "PART_SearchTextBox", Type = typeof(TextBox))]
    public class SearchableComboBox : ComboBox
    {
        ListBox PART_CurrentList = null;
        TextBox PART_SearchTextBox = null;
        Popup PART_Popup = null;
        ItemsPresenter ItemsPresenter = null;
        static SearchableComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SearchableComboBox), new FrameworkPropertyMetadata(typeof(SearchableComboBox)));
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);
            CurrentList = ItemsSource;

            if (PART_SearchTextBox != null)
                SearchableComboBox_SelectionChanged(PART_SearchTextBox, new SelectionChangedEventArgs(TextBox.SelectionChangedEvent, new List<object>(), new List<object>()));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (PART_CurrentList != null)
            {
                PART_CurrentList.SelectionChanged -= PART_CurrentList_SelectionChanged;
                this.SelectionChanged -= SearchableComboBox_SelectionChanged;
            }

            if (PART_SearchTextBox != null)
            {
                PART_SearchTextBox.TextChanged -= PART_SearchTextBox_TextChanged;
            }

            if (PART_Popup != null)
            {
                PART_Popup.Opened -= PART_Popup_Opened;
                PART_Popup.Closed -= PART_Popup_Closed;
            }
            Presenter = this.GetTemplateChild("contentPresenter") as ContentPresenter;
            PART_CurrentList = GetTemplateChild("PART_CurrentList") as ListBox;
            PART_SearchTextBox = GetTemplateChild("PART_SearchTextBox") as TextBox;
            PART_Popup = GetTemplateChild("PART_Popup") as Popup;
            ItemsPresenter = GetTemplateChild("ItemsPresenter") as ItemsPresenter;

            if (PART_CurrentList != null)
            {
                PART_CurrentList.SelectionMode = SelectionMode.Single;
                PART_CurrentList.SelectionChanged += PART_CurrentList_SelectionChanged;
                this.SelectionChanged += SearchableComboBox_SelectionChanged;

                Binding binding = new Binding(nameof(this.ItemTemplate)) { Source = this, Mode = BindingMode.OneWay };
                PART_CurrentList.SetBinding(ItemTemplateProperty, binding);
                binding = new Binding(nameof(this.CurrentList)) { Source = this, Mode = BindingMode.OneWay };
                PART_CurrentList.SetBinding(ItemsSourceProperty, binding);
            }

            if (PART_SearchTextBox != null)
            {
                PART_SearchTextBox.TextChanged += PART_SearchTextBox_TextChanged;
            }

            if (PART_Popup != null)
            {
                PART_Popup.Opened += PART_Popup_Opened;
                PART_Popup.Closed += PART_Popup_Closed;
            }


            if (AltSelectionBoxItemTemplate != null && Presenter != null)
            {
                Presenter.ContentTemplate = AltSelectionBoxItemTemplate;
            }

            CurrentList = ItemsSource;
        }

        private void PART_Popup_Opened(object sender, EventArgs e)
        {
            if (PART_SearchTextBox != null)
            {
                PART_SearchTextBox.Text = null;

                PART_SearchTextBox.Focus();
                SearchCounter?.Invoke(sender, e);
                Keyboard.Focus(PART_SearchTextBox);
            }
        }

        private void PART_Popup_Closed(object sender, EventArgs e)
        {
        }

        private void SearchableComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combobox = sender as ComboBox;

            if (PART_CurrentList != null && (SelectedItem == null || CurrentList?.OfType<object>()?.Contains(SelectedItem) == true))
            {
                ignore_currentlist_selection = true;
                PART_CurrentList.SelectedItem = SelectedItem;
                ignore_currentlist_selection = false;
            }
        }

        private void PART_CurrentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listbox = sender as ListBox;
            if (ignore_currentlist_selection == false)
            {
                this.SelectedItem = PART_CurrentList?.SelectedItem;
                IsDropDownOpen = false;
            }
        }

        bool ignore_currentlist_selection = false;
        private void PART_SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textbox = sender as TextBox;
            ignore_currentlist_selection = true;

            if (ItemsSource != null)
            {
                var textpath = TextSearch.GetTextPath(this);
                if (string.IsNullOrEmpty(textpath))
                    textpath = DisplayMemberPath;
                if (string.IsNullOrEmpty(textpath))
                    textpath = SelectedValuePath;

                if (string.IsNullOrEmpty(textpath))
                    CurrentList = ItemsSource?.OfType<object>()?.Where(x => x.ToString()?.ToLower()?.Contains(textbox.Text?.ToLower()) == true);
                else
                    CurrentList = ItemsSource?.OfType<object>()?.Where(x =>
                    {
                        if (PART_CurrentList != null && x == SelectedItem)
                            PART_CurrentList.SelectedItem = SelectedItem;

                        object value = x.GetType()?.GetProperty(textpath)?.GetValue(x);
                        return value?.ToString()?.ToLower()?.Contains(textbox.Text?.ToLower()) == true;
                    });
            }

            SearchCounter?.Invoke(sender, e);
            ignore_currentlist_selection = false;
        }

        #region PROPERTIES
        public IEnumerable CurrentList
        {
            get { return (IEnumerable)GetValue(CurrentListProperty); }
            set { SetValue(CurrentListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentListProperty =
            DependencyProperty.Register("CurrentList", typeof(IEnumerable), typeof(SearchableComboBox), new PropertyMetadata(null));

        public int? SearchCharCount
        {
            get { return (int?)GetValue(SearchCharCountProperty); }
            set { SetValue(SearchCharCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SearchCharCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SearchCharCountProperty =
            DependencyProperty.Register("SearchCharCount", typeof(int?), typeof(SearchableComboBox), new PropertyMetadata(0));


        /// <summary>
        /// Property Created for changing Template of Selected Box Display 
        /// Task # GBL-3195
        /// </summary>
        public DataTemplate AltSelectionBoxItemTemplate
        {
            get { return (DataTemplate)GetValue(AltSelectionBoxItemTemplateProperty); }
            set { SetValue(AltSelectionBoxItemTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AltSelectionBoxItemTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AltSelectionBoxItemTemplateProperty =
            DependencyProperty.Register("AltSelectionBoxItemTemplate", typeof(DataTemplate), typeof(SearchableComboBox), new UIPropertyMetadata(null, (s, e) => 
            {
                if((s is SearchableComboBox) && ((SearchableComboBox)s).Presenter != null && (e.NewValue is DataTemplate))
                {
                    ((SearchableComboBox)s).AltSelectionBoxItemTemplate = (DataTemplate)e.NewValue;
                }

            }));

        #endregion

        #region Events
        public event EventHandler SearchCounter = (s, e) => { };
        #endregion

        #region Elements
        ContentPresenter Presenter { get; set; }
        #endregion
    }
}
