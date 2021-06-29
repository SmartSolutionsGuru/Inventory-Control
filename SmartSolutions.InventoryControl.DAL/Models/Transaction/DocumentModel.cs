using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSolutions.InventoryControl.DAL.Models.Transaction
{
    public class DocumentModel:BaseModel
    {
        #region Constructor
        public DocumentModel()
        {

        }
        #endregion

        #region Public Properties
        public Guid DocumentNo { get; set; }
        public string DocumentType { get; set; }
        public string Description { get; set; }
        #endregion
    }
}
