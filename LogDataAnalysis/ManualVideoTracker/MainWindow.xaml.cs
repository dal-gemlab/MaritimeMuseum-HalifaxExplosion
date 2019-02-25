using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FileHelpers;
using Microsoft.Win32;
using Path = System.IO.Path;


namespace ManualVideoTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int SKIPTIME = 2;

        private enum CurrentTarget
        {
            HoloLens,
            NoHoloLens
        };

        
        private TimeSpan startTimeTimeSpan, endTimeTimeSpan;
        private List<TrackerCSV> positions;
        private TrackerCSV currentTracker;
        private CurrentTarget currentTarget;
        private int positionIndex = 0;

        private FileHelperEngine<TrackerCSV> engine;
        private string videoName;
        public MainWindow()
        {
            InitializeComponent();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            positions = new List<TrackerCSV>();
            currentTarget = CurrentTarget.HoloLens;
            if (openFileDialog.ShowDialog() == true)
            {
                videoName = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
              
                MediaElement.Source = new Uri(openFileDialog.FileName);
                MediaElement.ScrubbingEnabled = true;
            }
            //MediaElement.Source = new Uri(@"C:\play.mp4");
            //MediaElement.Play();
            




        }

        private void MediaElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition((IInputElement) sender);
            pos.Y = pos.Y /((MediaElement) sender).ActualHeight;
            pos.X = pos.X / ((MediaElement)sender).ActualWidth;

            if (currentTarget == CurrentTarget.HoloLens)
            {
                currentTracker = new TrackerCSV
                {
                    HL_x = pos.X,
                    HL_y = pos.Y
                };
                positions.Add(currentTracker);
                Skip(SKIPTIME);
            }
            else
            {
                if (positionIndex < positions.Count)
                {
                    positions[positionIndex].nHL_x = pos.X;
                    positions[positionIndex].nHL_y = pos.Y;
                    positionIndex++;
                }

                Skip(SKIPTIME);
            }

        }

        private void Skip(int seconds)
        {
            MediaElement.Position += TimeSpan.FromSeconds(2);
            if (MediaElement.Position > endTimeTimeSpan)
            {
                if (currentTarget == CurrentTarget.HoloLens)
                {
                    MessageBox.Show("Track NHL");
                    currentTarget = CurrentTarget.NoHoloLens;
                    MediaElement.Position = startTimeTimeSpan;
                }
                else
                {
                    Save();
                    MessageBox.Show("Done");
                    System.Windows.Application.Current.Shutdown();
                }
                
                
            }
        }

        private void StartClip(object sender, RoutedEventArgs e)
        {
            startTimeTimeSpan = TimeSpan.ParseExact(StartTime.Text, "mm':'ss'.'f", null);
            endTimeTimeSpan= TimeSpan.ParseExact(EndTime.Text, "mm':'ss'.'f", null);
            MediaElement.Position = startTimeTimeSpan;
            MediaElement.Pause();
            
        }

        private void HLOutOfFrame(object sender, RoutedEventArgs e)
        {
            currentTracker = new TrackerCSV
            {
                HL_x = -1,
                HL_y = -1,
            };
        }

        private void nHLOutOfFrame(object sender, RoutedEventArgs e)
        {
            currentTracker.nHL_x = -1;
            currentTracker.nHL_y = -1;
            positions.Add(currentTracker);
            currentTracker = null;
            Skip(SKIPTIME);
        }

        private void Save()
        {
            engine = new FileHelperEngine<TrackerCSV>();
            engine.WriteFile($"{videoName}.csv",positions);
        }

    }
}
