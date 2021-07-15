using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using SmartSolutions.InventoryControl.DAL.Models.Shipping;

namespace SmartSolutions.InventoryControl.DAL.Models.PurchaseOrder
{
    public class PurchaseOrderModel : BaseModel
    {
        #region Constructor
        public PurchaseOrderModel()
        {
            Partner = new BussinessPartnerModel();
        }
        #endregion

        #region Properties
        public BussinessPartnerModel Partner { get; set; }
        /// <summary>
        /// Status Of Order Like Completed,NotComplete etc...
        /// </summary>
        public OrderStatus Status { get; set; }
        public string Description { get; set; }
        public ShippingModel Shipping { get; set; }
        public decimal SubTotaL { get; set; }
        public decimal Discount { get; set; }
        public decimal GrandTotal { get; set; }

        #endregion

        #region Enum
        public enum OrderStatus
        {
            None = 0,
            /// <summary>
            /// Order is Created
            /// </summary>
            New = 1,
            /// <summary>
            /// Sent To Vender/Supplier
            /// </summary>
            Relesed = 2,
            /// <summary>
            /// Completed or No Item Remaining
            /// </summary>
            Received = 3,
            /// <summary>
            ///  purchase order will not be processed any more. 
            /// </summary>
            Canceled = 4,
            /// <summary>
            /// Completed Successfully and Closed
            /// </summary>
            Closed = 5
        }
        #endregion
    }
}
