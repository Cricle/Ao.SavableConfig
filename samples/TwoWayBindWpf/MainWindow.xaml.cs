using Ao.SavableConfig;
using Ao.SavableConfig.Binder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Extensions.Configuration;
using Ao.SavableConfig.Saver;

namespace TwoWayBindWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SavableConfigurationRoot root;
        public MainWindow()
        {
            InitializeComponent();
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("app.json", true, true);
            root = builder.BuildSavable();
            DataContext = ProxyHelper.Default.EnsureCreateProxWithAttribute<ThemeService>(root);
            root.BindTwoWay(DataContext, JsonChangeTransferCondition.Instance);
        }
    }
}
