using SmartSolutions.InventoryControl.DAL.Models.Product;

namespace SmartSolutions.InventoryControl.DAL.Models.PurchaseOrder
{
    public class PurchaseReturnInvoiceDetail : BaseModel
    {
        public PurchaseReturnInvoiceModel PurchaseReturnInvoice { get; set; }
        public ProductModel Product { get; set; }
        public ProductColorModel ProductColor { get; set; }
        public ProductSizeModel ProductSize { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
    }
}
