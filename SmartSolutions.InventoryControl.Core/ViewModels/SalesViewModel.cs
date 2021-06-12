using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using SmartSolutions.InventoryControl.DAL.Managers.Product.ProductColor;
using SmartSolutions.InventoryControl.DAL.Managers.Product.ProductSize;
using SmartSolutions.InventoryControl.DAL.Managers.Product;
using System.Text;
using SmartSolutions.InventoryControl.DAL.Models;
using System.Linq;
using SmartSolutions.InventoryControl.DAL.Models.Product;
using SmartSolutions.InventoryControl.Core.Helpers.SuggestionProvider;
using SmartSolutions.InventoryControl.DAL.Models.Inventory;
using SmartSolutions.Util.LogUtils;

namespace SmartSolutions.InventoryControl.Core.ViewModels
{
    [Export(typeof(SalesViewModel)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SalesViewModel : BaseViewModel
    {
        #region Private Members
        private readonly IProductColorManager _productColorManager;
        private readonly IProductSizeManager _productSizeManager;
        private readonly IProductManager _productManager;
        private readonly DAL.Managers.Invoice.IInvoiceManager _invoiceManager;
        private readonly DAL.Managers.Bussiness_Partner.IBussinessPartnerManager _bussinessPartnerManager;
        #endregion

        #region Constructor
        [ImportingConstructor]
        public SalesViewModel(IProductColorManager productColorManager
                              , IProductSizeManager productSizeManager, IProductManager productManager
                              , DAL.Managers.Invoice.IInvoiceManager invoiceManager
                             , DAL.Managers.Bussiness_Partner.IBussinessPartnerManager bussinessPartnerManager)
        {
            _productColorManager = productColorManager;
            _productSizeManager = productSizeManager;
            _productManager = productManager;
            _invoiceManager = invoiceManager;
            _bussinessPartnerManager = bussinessPartnerManager;
        }
        #endregion

        #region Methods
        protected async override void OnActivate()
        {
            IsLoading = true;
            LoadingMessage = "Loading...";
            base.OnActivate();
            InvoiceTypes = new List<string> { "Sales", "Sales Return" };
            Partners = (await _bussinessPartnerManager.GetAllBussinessPartnersAsync()).ToList();
            Products = (await _productManager.GetAllProductsAsync()).ToList();
            ProductSizes = (await _productSizeManager.GetProductAllSizeAsync()).ToList();
            ProductColors = (await _productColorManager.GetProductAllColorsAsync()).ToList();
            SaleInvoice = new InvoiceModel();
            SaleInvoice.InvoiceId = _invoiceManager.GenrateInvoiceNumber("S");
            if (Partners != null || Partners?.Count > 0)
                PartnerSuggetion = new PartnerSuggestionProvider(Partners);
            if (Products != null || Products?.Count > 0)
                ProductSuggetion = new ProductSuggestionProvider(Products);
            IsLoading = false;
        }
        public void OnGettingPrice()
        {
            if (Quantity > 0 && Price > 0)
            {
                TotalPrice = Quantity * Price;
                NotifyOfPropertyChange(nameof(TotalPrice));
            }
            else
                TotalPrice = 0;
        }
        public void GetProductAvailableStock(int? Id)
        {
            try
            {
                if (Id == null || Id <= 0) return;

            }
            catch (Exception ex)
            {

                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        public void AddProduct(InventoryModel model)
        {
            try
            {

            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        public void RemoveProduct(InventoryModel model)
        {

        }
        public void SaveTransaction()
        {
            try
            {

            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        public void Cancel()
        {
            TryClose();
        }
        #endregion


        #region Properties
        private bool _CustomerError;
         /// <summary>
         /// if Customer is Not Selected
         /// </summary>
        public bool CustomerError
        {
            get { return _CustomerError; }
            set { _CustomerError = value; NotifyOfPropertyChange(nameof(CustomerError)); }
        }

        private bool _SaleTypeError;
         /// <summary>
         /// if Sale type is Not Selected
         /// </summary>
        public bool SaleTypeError
        {
            get { return _SaleTypeError; }
            set { _SaleTypeError = value; NotifyOfPropertyChange(nameof(SaleTypeError)); }
        }

        private int _AvailableStock;
         /// <summary>
         /// Stock available for Sale
         /// </summary>
        public int AvailableStock
        {
            get { return _AvailableStock; }
            set { _AvailableStock = value; NotifyOfPropertyChange(nameof(AvailableStock)); }
        }

        private List<string> _InvoiceTypes;

        public List<string> InvoiceTypes
        {
            get { return _InvoiceTypes; }
            set { _InvoiceTypes = value; NotifyOfPropertyChange(nameof(InvoiceTypes)); }
        }

        private string _SelectedInvoiceType;
        /// <summary>
        /// Selected Invoice
        /// </summary>
        public string SelectedInvoiceType
        {
            get { return _SelectedInvoiceType; }
            set { _SelectedInvoiceType = value; NotifyOfPropertyChange(nameof(SelectedInvoiceType)); }
        }

        private List<BussinessPartnerModel> _Partners;

        public List<BussinessPartnerModel> Partners
        {
            get { return _Partners; }
            set { _Partners = value; NotifyOfPropertyChange(nameof(Partners)); }
        }

        private PartnerSuggestionProvider _PartnerSuggetion;

        public PartnerSuggestionProvider PartnerSuggetion
        {
            get { return _PartnerSuggetion; }
            set { _PartnerSuggetion = value; NotifyOfPropertyChange(nameof(PartnerSuggetion)); }
        }


        private BussinessPartnerModel _SelectedPartner;
        /// <summary>
        /// Selected bussiness Partner
        /// </summary>
        public BussinessPartnerModel SelectedPartner
        {
            get { return _SelectedPartner; }
            set { _SelectedPartner = value; NotifyOfPropertyChange(nameof(SelectedPartner)); }
        }

        private DAL.Models.Inventory.InvoiceModel _SaleInvoice;
        /// <summary>
        /// Sales Transaction Model
        /// </summary>
        public DAL.Models.Inventory.InvoiceModel SaleInvoice
        {
            get { return _SaleInvoice; }
            set { _SaleInvoice = value; NotifyOfPropertyChange(nameof(SaleInvoice)); }
        }
        private ProductSuggestionProvider _ProductSuggetion;
        /// <summary>
        /// List of Product Suggetions
        /// </summary>
        public ProductSuggestionProvider ProductSuggetion
        {
            get { return _ProductSuggetion; }
            set { _ProductSuggetion = value; NotifyOfPropertyChange(nameof(ProductSuggetion)); }
        }
        private List<ProductModel> _Products;
        /// <summary>
        /// List Of Products
        /// </summary>
        public List<ProductModel> Products
        {
            get { return _Products; }
            set { _Products = value; NotifyOfPropertyChange(nameof(Products)); }
        }
        private ProductModel _SelectedProduct;
        /// <summary>
        /// SElected Product 
        /// </summary>
        public ProductModel SelectedProduct
        {
            get { return _SelectedProduct; }
            set { _SelectedProduct = value; NotifyOfPropertyChange(nameof(SelectedProduct)); ProductSuggetion = new ProductSuggestionProvider(Products); GetProductAvailableStock(SelectedProduct?.Id); }
        }
        private string _SelectedSaleType;

        public string SelectedSaleType
        {
            get { return _SelectedSaleType; }
            set { _SelectedSaleType = value; NotifyOfPropertyChange(nameof(SelectedSaleType)); }
        }
        private List<ProductColorModel> _ProductColors;
        /// <summary>
        /// List of Selected Product Colors
        /// </summary>
        public List<ProductColorModel> ProductColors
        {
            get { return _ProductColors; }
            set { _ProductColors = value; NotifyOfPropertyChange(nameof(ProductColors)); }
        }
        private ProductColorModel _SelectedProductColor;
        /// <summary>
        /// Selected Product Color
        /// </summary>
        public ProductColorModel SelectedProductColor
        {
            get { return _SelectedProductColor; }
            set { _SelectedProductColor = value; NotifyOfPropertyChange(nameof(SelectedProductColor)); }
        }
        private List<ProductSizeModel> _ProductSizes;
        /// <summary>
        /// Size List Of Selected Product
        /// </summary>
        public List<ProductSizeModel> ProductSizes
        {
            get { return _ProductSizes; }
            set { _ProductSizes = value; NotifyOfPropertyChange(nameof(ProductSizes)); }
        }
        private ProductSizeModel _SelectedProductSize;
        /// <summary>
        /// Selected Product Size
        /// </summary>
        public ProductSizeModel SelectedProductSize
        {
            get { return _SelectedProductSize; }
            set { _SelectedProductSize = value; NotifyOfPropertyChange(nameof(SelectedProductSize)); }
        }
        private int _Quantity;
        /// <summary>
        /// Quantity Of Product
        /// </summary>
        public int Quantity
        {
            get { return _Quantity; }
            set { _Quantity = value; NotifyOfPropertyChange(nameof(Quantity)); }
        }
        private double _Price;
        /// <summary>
        /// Price Of Product
        /// </summary>
        public double Price
        {
            get { return _Price; }
            set { _Price = value; NotifyOfPropertyChange(nameof(Price)); OnGettingPrice(); }
        }

        private double _TotalPrice;

        public double TotalPrice
        {
            get { return _TotalPrice; }
            set { _TotalPrice = value; NotifyOfPropertyChange(nameof(TotalPrice)); }
        }
        private int _PercentDiscount;
        /// <summary>
        /// Discount In Percentage
        /// </summary>
        public int PercentDiscount
        {
            get { return _PercentDiscount; }
            set { _PercentDiscount = value; NotifyOfPropertyChange(nameof(PercentDiscount)); }
        }
        private double _GrandTotal;
        /// <summary>
        /// Grand Total Amount Of Transaction
        /// </summary>
        public double GrandTotal
        {
            get { return _GrandTotal; }
            set { _GrandTotal = value; NotifyOfPropertyChange(nameof(GrandTotal)); }
        }
        private byte[] _PaymentImage;
        /// <summary>
        /// Payment Image 
        /// </summary>
        public byte[] PaymentImage
        {
            get { return _PaymentImage; }
            set { _PaymentImage = value; NotifyOfPropertyChange(nameof(PaymentImage)); }
        }
        private double _DiscountPrice;
        /// <summary>
        /// Price After Calculating Discount
        /// </summary>
        public double DiscountPrice
        {
            get { return _DiscountPrice; }
            set { _DiscountPrice = value; NotifyOfPropertyChange(nameof(DiscountPrice)); }
        }
        #endregion
    }
}
