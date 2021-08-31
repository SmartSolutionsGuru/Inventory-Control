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
        #endregion
    }
}
