using SmartSolutions.Util.QueryUtils;
using System.Reflection;

namespace SmartSolutions.InventoryControl.DAL.Models
{
    public class QueryProvider : IQueryProvider
    {
        private readonly Assembly CurrentAssembly;

        public QueryProvider()
        {
            CurrentAssembly = GetType().Assembly;
        }
        public Assembly GetAssemblyForQueries()
        {
            return CurrentAssembly;
        }
    }
}
