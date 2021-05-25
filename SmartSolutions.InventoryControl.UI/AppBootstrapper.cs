using Caliburn.Micro;
using SmartSolutions.InventoryControl.Core.ViewModels.Dialogs;
using SmartSolutions.InventoryControl.Plugins.IoC;
using SmartSolutions.InventoryControl.UI.Helpers.SettingHelper;
using SmartSolutions.Util.LogUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SmartSolutions.InventoryControl.UI
{
    public class AppBootstrapper: BootstrapperBase
    {
        #region Public members
        public CompositionContainer _container { get; private set; }
        #endregion

        #region Construcotr
        public AppBootstrapper()
        {
            IoCContanier.IoC = new SmartSolutions.InventoryControl.MEF.MEF();
            Initialize();   
        }
       
        #endregion

        protected override void Configure()
        {
            base.Configure();
            var config = new TypeMappingConfiguration
            {
                DefaultSubNamespaceForViews = "SmartSolutions.InventoryControl.UI.Views",
                DefaultSubNamespaceForViewModels = "SmartSolutions.InventoryControl.Core.ViewModels",
            };
            ViewLocator.ConfigureTypeMappings(config);
            ViewModelLocator.ConfigureTypeMappings(config);
            List<string> plugins = new List<string>();
            if(!Directory.Exists(AppSettings.AppDirectory + "Plugins"))
            {
                Directory.CreateDirectory(AppSettings.AppDirectory + "Plugins");
            }
            plugins.Add("SmartSolutions.InventoryControl.Core.dll");
            plugins.Add("SmartSolutions.InventoryControl.DAL.dll");
            plugins.Add("SmartSolutions.InventoryControl.Repositories.SQLiteCipher.dll");
            //if (!Execute.InDesignMode)
            //{
            //    string iniPath = Path.Combine(Helpers.SettingHelper.AppSettings.AppDirectory + "Config.ini");
            //    if (File.Exists(iniPath))
            //    {
            //        string[] plugin_names = new string[]
            //        {
            //                "DBRepository",
            //                "DAL",
            //        };
            //        var inifile = new IniFile(iniPath);
            //        foreach (var p in plugin_names)
            //        {
            //            plugins.Add(Path.Combine(AppSettings.AppDirectory, "Plugins", inifile.ReadValue("Plugins", p)));
            //        }
            //        //plugins.Add(Path.Combine(AppSettings.AppDirectory, "Plugins", "Logs", "SmartBooks.Logs.dll"));

            //        var design_files = inifile.ReadKeys("DesignFiles");
            //        if ((design_files?.Count() ?? 0) > 0)
            //        {
            //            AppSettings.DesignFiles = new Dictionary<string, string>();
            //            foreach (var item in design_files)
            //            {
            //                AppSettings.DesignFiles[item] = inifile.ReadValue("DesignFiles", item);
            //            }
            //        }

            //        var custom_configuration = inifile.ReadKeys("Configuration");
            //        if ((custom_configuration?.Count() ?? 0) > 0)
            //        {
            //            AppSettings.Configurations = new Dictionary<string, string>();
            //            foreach (var item in custom_configuration)
            //            {
            //                AppSettings.Configurations[item] = inifile.ReadValue("Configuration", item);
            //            }
            //        }
            //    }
            //}
            IoCContanier.IoC.Configure(plugins, Execute.InDesignMode);
            IoCContanier.IoC.AddExportedValue<IWindowManager>(new WindowManager());
            IoCContanier.IoC.AddExportedValue<IEventAggregator>(new EventAggregator());
            //IoCContanier.IoC.AddExportedValue(new ApplicationCloseStrategy());
            IoCContanier.IoC.AddExportedValue<Func<IMessageBox>>(() => IoCContanier.IoC.GetExportedValue<IMessageBox>());
            IoCContanier.IoC.Compose();
            
        }
        protected override object GetInstance(Type service, string key) => IoCContanier.IoC.GetInstance(service, key);
        protected override IEnumerable<object> GetAllInstances(Type service) => IoCContanier.IoC.GetAllInstances(service);
        protected override void BuildUp(object instance) => IoCContanier.IoC.BuildUp(instance);
       

        protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
        {
            try
            {
                base.OnStartup(sender, e);
                DisplayRootViewFor<Core.IShell>();
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Fatal);
            }
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            var assemblies = base.SelectAssemblies().ToList();
            assemblies.Add(typeof(Core.ViewModels.ShellViewModel).GetTypeInfo().Assembly);
            return assemblies;
        }

        #region Private Methods
        private static void GetDesignFiles(IniFile inifile)
        {
            try
            {
                var design_files = inifile.ReadKeys("DesignFiles");
                if ((design_files?.Count() ?? 0) > 0)
                {
                    AppSettings.DesignFiles = new Dictionary<string, string>();
                    foreach (var item in design_files)
                    {
                        AppSettings.DesignFiles[item] = inifile.ReadValue("DesignFiles", item);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        #endregion
    }
}
