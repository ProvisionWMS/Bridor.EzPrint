using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Bridor.EzPrint.ViewModels;

namespace Bridor.EzPrint.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow() {
            InitializeComponent();
            this.Icon = BitmapFrame.Create(new Uri("pack://application:,,,/EzPrint;component/Printer.ico"));
            // Assign the view model
            this.DataContext = new SelectionViewModel();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            // Exit the applicaiton
            this.Close();
        }
    }
}
