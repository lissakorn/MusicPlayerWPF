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

namespace WpfApp_Tags
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MediaCollection mediaCollection;
        public MainWindow()
        {
            InitializeComponent();
            mediaCollection = new MediaCollection();
            this.DataContext = mediaCollection;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mediaCollection.SetMediaElement(mediaElement);
        }

      
    }
}
