namespace SmartSolutions.InventoryControl.DAL.Models.Product
{
    public class ProductModel : BaseModel
    {
        #region Constructor
        public ProductModel()
        {
            ProductType = new ProductTypeModel();
            ProductSubType = new ProductSubTypeModel();
            ProductColor = new ProductColorModel();
            ProductSize = new ProductSizeModel();
        }
        #endregion

        #region Properties
        public ProductTypeModel ProductType { get; set; }
        public ProductSubTypeModel ProductSubType { get; set; }
        public ProductColorModel ProductColor { get; set; }
        public ProductSizeModel ProductSize { get; set; }
        public byte[] Image { get; set; }
        public string ImagePath { get; set; }
        private string _NameWithColor;

        public string NameWithColor
        {
            get 
            {
                if(!string.IsNullOrEmpty(ProductColor.Color))
                {
                    return $"{Name} ({ProductColor.Color})";
                }else
                {
                    return $"{Name}";
                }
            }
            set 
            {
                if(ProductColor  != null) 
                {
                    _NameWithColor = $"{value} ( {ProductColor.Color} )";
                }
                else
                {
                    _NameWithColor = value;
                }              
                NotifyOfPropertyChange(nameof(NameWithColor));
            }
        }

        #endregion
    }
}
