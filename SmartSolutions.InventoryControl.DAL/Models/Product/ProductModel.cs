namespace SmartSolutions.InventoryControl.DAL.Models.Product
{
    public class ProductModel : BaseModel
    {
        public ProductTypeModel ProductType { get; set; }
        public ProductSubTypeModel ProductSubType { get; set; }
        public ProductColorModel ProductColor { get; set; }
        public ProductSizeModel ProductSize { get; set; }
        public byte[] Image { get; set; }
       // public string ProductImage { get; set; }
    }
}
