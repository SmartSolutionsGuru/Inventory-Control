using SmartSolutions.InventoryControl.Core.Helpers.SuggestionProvider;
using SmartSolutions.InventoryControl.DAL.Models.Purchase;
using SmartSolutions.InventoryControl.DAL.Models;
using SmartSolutions.InventoryControl.DAL.Models.Product;
using SmartSolutions.Util.LogUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace SmartSolutions.InventoryControl.Core.ViewModels
{
    [Export(typeof(PurchaseViewModel)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PurchaseViewModel : BaseViewModel
    {
        #region Private Members
        private readonly DAL.Managers.Purchase.IPurchaseManager _purchaseManager;
        private readonly DAL.Managers.Product.IProductManager _productManager;
        private readonly DAL.Managers.Bussiness_Partner.IBussinessPartnerManager _bussinessPartnerManager;
        private readonly DAL.Managers.Product.ProductSize.IProductSizeManager _productSizeManager;
        private readonly DAL.Managers.Product.ProductColor.IProductColorManager _productColorManager;
        #endregion

        #region Constructor
        [ImportingConstructor]
        public PurchaseViewModel(DAL.Managers.Purchase.IPurchaseManager purchaseManager
                                , DAL.Managers.Product.IProductManager productManager
                                , DAL.Managers.Bussiness_Partner.IBussinessPartnerManager bussinessPartnerManager
                                , DAL.Managers.Product.ProductColor.IProductColorManager productColorManager
                                , DAL.Managers.Product.ProductSize.IProductSizeManager productSizeManager)
        {
            _purchaseManager = purchaseManager;
            _productManager = productManager;
            _bussinessPartnerManager = bussinessPartnerManager;
            _productColorManager = productColorManager;
            _productSizeManager = productSizeManager;
        }
        #endregion

        #region Public Methods
        protected async override void OnActivate()
        {
            try
            {
                base.OnActivate();
                PurchaseTypes = new List<string> { "Purchase", "Purchase Return" };
                Products = (await _productManager.GetAllProductsAsync()).ToList();
                Venders = (await _bussinessPartnerManager.GetAllBussinessPartnersAsync()).ToList();
                ProductSizes = (await _productSizeManager.GetProductAllSizeAsync()).ToList();
                ProductColors = (await _productColorManager.GetProductAllColorsAsync()).ToList();
                PurchaseTransaction = new PurchaseTransactionModel();
                int Id = await _purchaseManager.GetLastTransationIdAsync();
                PurchaseTransaction.TransactionNo = Id + 1;
                if (Venders != null)
                    PartnerSuggetion = new PartnerSuggestionProvider(Venders);
                if (Products != null)
                    ProductSuggetion = new ProductSuggestionProvider(Products);
                //IsLoading = true;

            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
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
        public void AddProduct(ProductModel product)
        {
            try
            {
                if (CurrentItem != null)
                {
                    CurrentItem.Product = SelectedProduct;
                    CurrentItem.ProductColor = SelectedProductColor;
                    CurrentItem.ProductSize = SelectedProductSize;
                    CurrentItem.Price = Price;
                    CurrentItem.Quantity = Quantity;
                    CurrentItem.Total = TotalPrice;
                    PurchaseTransaction?.Products?.Add(CurrentItem);
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        public void RemoveProduct(ProductModel product)
        {
            try
            {

            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        public void SaveTransaction()
        {
            try
            {
                IsLoading = true;
                LoadingMessage = "Saving...";
                //if (string.IsNullOrEmpty(SelectedPurchaseType))
                //    PurchaseTransaction.SelectedPurchaseType = SelectedPurchaseType;
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
        private bool _QuantityError;
        /// <summary>
        /// Error Quantity 
        /// </summary>
        public bool QuantityError
        {
            get { return _QuantityError; }
            set { _QuantityError = value; NotifyOfPropertyChange(nameof(QuantityError)); }
        }

        private bool _PriceError;
        /// <summary>
        /// Price Error If wrong Value
        /// </summary>
        public bool PriceError
        {
            get { return _PriceError; }
            set { _PriceError = value; NotifyOfPropertyChange(nameof(PriceError)); }
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
        private double _LastBalance;
        /// <summary>
        /// Last Balance Of Partner
        /// </summary>
        public double LastBalance
        {
            get { return _LastBalance; }
            set { _LastBalance = value; NotifyOfPropertyChange(nameof(LastBalance)); }
        }

        private PurchaseTransactionModel _PurchaseTransaction;
        /// <summary>
        /// Transaction Object for Purchase Transaction
        /// </summary>
        public PurchaseTransactionModel PurchaseTransaction
        {
            get { return _PurchaseTransaction; }
            set { _PurchaseTransaction = value; NotifyOfPropertyChange(nameof(PurchaseTransaction)); }
        }

        private PartnerSuggestionProvider _PartnerSuggetion;
        /// <summary>
        /// Peoperty for Partner Suggetion
        /// </summary>
        public PartnerSuggestionProvider PartnerSuggetion
        {
            get { return _PartnerSuggetion; }
            set { _PartnerSuggetion = value; NotifyOfPropertyChange(nameof(PartnerSuggetion)); }
        }
        private BussinessPartnerModel _SelectedPartner;
        public BussinessPartnerModel SelectedPartner
        {
            get { return _SelectedPartner; }
            set { _SelectedPartner = value; NotifyOfPropertyChange(nameof(SelectedPartner)); }
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
        private ProductModel _SelectedProduct;
        /// <summary>
        /// SElected Product 
        /// </summary>
        public ProductModel SelectedProduct
        {
            get { return _SelectedProduct; }
            set { _SelectedProduct = value; NotifyOfPropertyChange(nameof(SelectedProduct)); ProductSuggetion = new ProductSuggestionProvider(Products); }
        }

        private bool _ProductSizeError;
        /// <summary>
        /// If Product Size is Not Selected
        /// </summary>
        public bool ProductSizeError
        {
            get { return _ProductSizeError; }
            set { _ProductSizeError = value; NotifyOfPropertyChange(nameof(ProductSizeError)); }
        }

        private bool _ProductColorError;
        /// <summary>
        /// If Product Color is Not Selected
        /// </summary>
        public bool ProductColorError
        {
            get { return _ProductColorError; }
            set { _ProductColorError = value; NotifyOfPropertyChange(nameof(ProductColorError)); }
        }

        private bool _VenderError;
        /// <summary>
        /// If Vender is not Selected
        /// </summary>
        public bool VenderError
        {
            get { return _VenderError; }
            set { _VenderError = value; NotifyOfPropertyChange(nameof(VenderError)); }
        }

        private bool _ProductError;
        /// <summary>
        /// Product Selection Error
        /// </summary>
        public bool ProductError
        {
            get { return _ProductError; }
            set { _ProductError = value; NotifyOfPropertyChange(nameof(ProductError)); }
        }

        private bool _PurchaseTypeError;
        /// <summary>
        /// Error For Purchase Type
        /// </summary>
        public bool PurchaseTypeError
        {
            get { return _PurchaseTypeError; }
            set { _PurchaseTypeError = value; NotifyOfPropertyChange(nameof(PurchaseTypeError)); }
        }

        private PurchaseModel _CurrentItem;
        /// <summary>
        /// Current Item For Adding Product
        /// </summary>
        public PurchaseModel CurrentItem
        {
            get { return _CurrentItem ?? new PurchaseModel(); }
            set { _CurrentItem = value; NotifyOfPropertyChange(nameof(CurrentItem)); }
        }


        private List<string> _PurchaseTypes;
        /// <summary>
        /// Purchase Types Like Return etc..
        /// </summary>
        public List<string> PurchaseTypes
        {
            get { return _PurchaseTypes; }
            set { _PurchaseTypes = value; NotifyOfPropertyChange(nameof(PurchaseTypes)); }
        }
        private string _SelectedPurchaseType;

        public string SelectedPurchaseType
        {
            get { return _SelectedPurchaseType; }
            set { _SelectedPurchaseType = value; NotifyOfPropertyChange(nameof(SelectedPurchaseType)); }
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
        private List<ProductModel> _Products;
        /// <summary>
        /// List Of Products
        /// </summary>
        public List<ProductModel> Products
        {
            get { return _Products; }
            set { _Products = value; NotifyOfPropertyChange(nameof(Products)); }
        }
        private List<BussinessPartnerModel> _Venders;
        public List<BussinessPartnerModel> Venders
        {
            get { return _Venders; }
            set { _Venders = value; NotifyOfPropertyChange(nameof(Venders)); }
        }
        private BussinessPartnerModel _SelectedVender;
        public BussinessPartnerModel SelectedVender
        {
            get { return _SelectedVender; }
            set { _SelectedVender = value; NotifyOfPropertyChange(nameof(SelectedVender)); }
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
        private int _Discount;
        /// <summary>
        /// Discount In Percentage
        /// </summary>
        public int Discount
        {
            get { return _Discount; }
            set { _Discount = value; NotifyOfPropertyChange(nameof(Discount)); }
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

        private double _GrandTotal;
         /// <summary>
         /// Grand Total Amount Of Transaction
         /// </summary>
        public double GrandTotal
        {
            get { return _GrandTotal; }
            set { _GrandTotal = value; NotifyOfPropertyChange(nameof(GrandTotal)); }
        }

        private double _Payment;
          /// <summary>
          /// Payment Which is Made to Supply Partner
          /// </summary>
        public double Payment
        {
            get { return _Payment; }
            set { _Payment = value; NotifyOfPropertyChange(nameof(Payment)); }
        }




        #endregion
    }
}
