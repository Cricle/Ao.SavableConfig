using Ao.SavableConfig;
using Ao.SavableConfig.Binder;
using Ao.SavableConfig.Saver;
using Microsoft.Extensions.Configuration;
using System;
using System.Windows;
using System.Windows.Input;

namespace TwoWayBindWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SavableConfigurationRoot root;
        public MainWindow()
        {
            InitializeComponent();
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("app.json", true, true);
            root = builder.BuildSavable();
            GoNormalBind();
            GoProxy();
            KeyDown += MainWindow_KeyDown;
        }
        private void GoNormalBind()
        {
            var inst = new ThemeService();
            root.BindNotifyTwoWay(inst, JsonChangeTransferCondition.Instance);
            Normal.DataContext = inst;
        }
        private void GoProxy()
        {
            var inst = root.AutoCreateProxy<ThemeService>();
            root.BindTwoWay(inst, JsonChangeTransferCondition.Instance);
            Proxy.DataContext = inst;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.A)
            {
                var rand = new Random();
                for (int i = 0; i < 100; i++)
                {
                    root["Title"] = rand.Next(100, 99999).ToString();
                    root["ButtonStyle:Background"] = rand.Next(100, 99999).ToString();
                    root["ButtonStyle:ObjectStyle:Order"] = rand.Next(100, 99999).ToString();
                }
            }
            else if (e.Key == Key.Q)
            {
                if (Proxy.DataContext is ThemeService ser)
                {
                    for (int i = 0; i < 100; i++)
                    {
                        ser.ButtonStyle = new ButtonStyle();
                    }
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
