using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Registration;
using SmartSolutions.InventoryControl.Plugins.IoC;
using System.IO;
using System.Reflection;
using SmartSolutions.Util.LogUtils;

namespace SmartSolutions.InventoryControl.MEF
{
    public class MEF : IIoC
    {
        public string Name => "Meneged Extensiable Framework";

        #region Properties
        private string AppDirectory => AppDomain.CurrentDomain?.BaseDirectory;
        private CompositionContainer container;
        private CompositionBatch batch;
        private AggregateCatalog catalog;
        private RegistrationBuilder builder;
        #endregion

        public void Configure(IEnumerable<string> plugins, bool designer = false)
        {
            try
            {
                if (!Directory.Exists(AppDirectory + "Plugins"))
                {
                    Directory.CreateDirectory(AppDirectory + "Plugins");
                }
                if (designer == false)
                {
                    builder = new RegistrationBuilder();
                    catalog = new AggregateCatalog();
                    catalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetEntryAssembly(),builder));
                    foreach (var p in plugins)
                    {
                        if (File.Exists(p))
                        {
                            catalog.Catalogs.Add(new AssemblyCatalog(p,builder));
                        }
                    }
                }
                else
                {
                    catalog = new AggregateCatalog();
                }

                container = new CompositionContainer(catalog);
                batch = new CompositionBatch();
            }
            catch (ReflectionTypeLoadException ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
                LogMessage.Write("Loader Exception :" + string.Join(",", ex.LoaderExceptions.Select(x => x.Message)));
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        public object GetInstance(Type serviceType, string key = null)
        {
            try
            {
                string contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
                var exports = container?.GetExportedValues<object>(contract);


                if (exports?.Any() == true)
                    return exports.First();

                if (contract.ToUpper().Contains("PLUGINS"))
                    return null;
                else
                {
                    string error = string.Format("Could Not Locate any Instance of contract {0}.", contract);
                    LogMessage.Write(error.ToString());
                    throw new Exception(error);
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                LogMessage.Write(ex.ToString());
                return null;
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString());
                return null;
            }
        }
        public IEnumerable<object> GetAllInstances(Type serviceType) => container?.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType)) ?? new List<object>();
        public void BuildUp(object instance)
        {
            try
            {
                container?.SatisfyImportsOnce(instance);
            }
            catch (Exception ex)
            {

                LogMessage.Write(ex.ToString());
            }
        }
        public object AddExportedValue<T>(T exportedValue) => batch?.AddExportedValue<T>(exportedValue);
        public object AddExportedValue<T>(string contractName, T exportedValue) => batch?.AddExportedValue<T>(contractName, exportedValue);
        public void Compose()
        {
            batch.AddExportedValue(container);
            container.Compose(batch);
        }
        public T GetExportedValue<T>() => container != null ? container.GetExportedValue<T>() : default(T);
        public T GetExportedValue<T>(string cotractName) => container != null ? container.GetExportedValue<T>(cotractName) : default(T);
        public T GetExportedValueOrDefault<T>() => container != null ? container.GetExportedValue<T>() : default(T);
        public T GetExportedValueOrDefault<T>(string cotractName) => container != null ? container.GetExportedValue<T>(cotractName) : default(T);
        public IEnumerable<T> GetExportedValues<T>() => container?.GetExportedValues<T>() ?? default(IEnumerable<T>);
        public IEnumerable<T> GetExportedValues<T>(string contractName) => container?.GetExportedValues<T>(contractName) ?? default(IEnumerable<T>);

        //Post-build event command line:
        //COPY/Y "$(TargetPath)" "$(SolutionDir)$(SolutionName)$(OutDir)Plugins\MySQL\$(TargetName).dll"
        //if($(ConfigurationName)==Relese(COPY/Y "$(TargetPath)" "$(SolutionDir)$(SolutionName)\bin\Plugins\MySQL\$(TargetName).dll")
    }
}
