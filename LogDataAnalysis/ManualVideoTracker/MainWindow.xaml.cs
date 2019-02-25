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



namespace ManualVideoTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MediaElement.Source = new Uri(@"C:\play.mp4");
            MediaElement.Play();
            //MediaElement.Play();




        }

        private void MediaElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine(e.GetPosition(null));
            
        }
    }
}
