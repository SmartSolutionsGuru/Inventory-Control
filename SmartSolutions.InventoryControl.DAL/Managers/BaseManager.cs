namespace SmartSolutions.InventoryControl.DAL.Managers
{
    public class BaseManager
    {
        public Plugins.Repositories.IRepository GetRepository<T>() where T : class, new() => Plugins.IoC.IoCContanier.IoC.GetExportedValue<Plugins.Repositories.IRepository>();
    }
}
