using System;
using System.Collections.Generic;

namespace SmartSolutions.InventoryControl.Plugins.IoC
{
    public interface IIoC : IPlugin
    {
        void Configure(IEnumerable<string> plugins, bool designer = false);
        object GetInstance(Type serviceType, string key = null);
        IEnumerable<object> GetAllInstances(Type serviceType);
        void BuildUp(object instance);
        object AddExportedValue<T>(T exportedValue);
        object AddExportedValue<T>(string contractName, T exportedValue);
        void Compose();

        T GetExportedValue<T>();
        T GetExportedValue<T>(string contractName);
        T GetExportedValueOrDefault<T>();
        T GetExportedValueOrDefault<T>(string contractName);
        IEnumerable<T> GetExportedValues<T>();
        IEnumerable<T> GetExportedValues<T>(string contractName);
        
    }

    public class IoCContanier
    {
        public static IIoC IoC { get; set; }
    }
}
