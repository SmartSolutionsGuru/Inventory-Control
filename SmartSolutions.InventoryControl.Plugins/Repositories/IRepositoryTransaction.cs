using System;
using System.Data.Common;

namespace SmartSolutions.InventoryControl.Plugins.Repositories
{
    public interface IRepositoryTransaction : IDisposable
    {
        #region Public Methods
        DbConnection Connection { get; }
        DbTransaction Transaction { get; }
        void Commit();
        void RollBack();
        #endregion
    }
}
