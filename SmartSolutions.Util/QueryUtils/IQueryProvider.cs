using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SmartSolutions.Util.QueryUtils
{
    public interface IQueryProvider
    {
        /// <summary>
        /// Get the Assembly For Queries
        /// </summary>
        /// <returns></returns>
        Assembly GetAssemblyForQueries();
    }
}
