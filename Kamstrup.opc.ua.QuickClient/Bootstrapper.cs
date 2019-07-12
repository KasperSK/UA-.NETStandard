using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;


namespace Kamstrup.opc.ua.QuickClient
{
    public class Bootstrapper : BootstrapperBase
    {
        private SimpleContainer container = new SimpleContainer();

        public Bootstrapper() : base(true)
        {
            Initialize();
        }

        public Bootstrapper(bool useApplication = true) : base(useApplication)
        {
            Initialize();
        }

        protected override void BuildUp(object instance)
        {
            container.BuildUp(instance);
        }

        protected override void Configure()
        {
            LogManager.GetLog = t => new DebugLog(t);

            container.PerRequest<IWindowManager, WindowManager>();

            container.PerRequest<ViewModels.ShellViewModel>();
            container.PerRequest<ViewModels.ToolBarViewModel>();
            container.PerRequest<ViewModels.StatusBarViewModel>();
            container.PerRequest<ViewModels.MainWindowViewModel>();
            
            base.Configure();
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.GetAllInstances(service);
        }

        protected override object GetInstance(Type service, string key)
        {
            return container.GetInstance(service, key);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ViewModels.ShellViewModel>();
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            return base.SelectAssemblies();
        }
    }
}
