using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamstrup.opc.ua.QuickClient.ViewModels
{
    class ShellViewModel : Screen
    {
        public ShellViewModel()
        {

        }

        public ShellViewModel(ToolBarViewModel toolBarViewModel, StatusBarViewModel statusBarViewModel, MainWindowViewModel mainWindowViewModel)
        {
            ToolBar = toolBarViewModel;
            StatusBar = statusBarViewModel;
            MainWindow = mainWindowViewModel;
        }


        private ToolBarViewModel _toolBar;
        public ToolBarViewModel ToolBar
        {
            get
            {
                return _toolBar;
            }
            set
            {
                Set(ref _toolBar, value);
            }
        }

        private StatusBarViewModel _statusBar;
        public StatusBarViewModel StatusBar
        {
            get
            {
                return _statusBar;
            }
            set
            {
                Set(ref _statusBar, value);
            }
        }

        private MainWindowViewModel _mainWindow;
        public MainWindowViewModel MainWindow
        {
            get
            {
                return _mainWindow;
            }
            set
            {
                Set(ref _mainWindow, value);
            }
        }

    }
}
