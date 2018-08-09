using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using Rug.Osc;
using StudyControlApp.Annotations;
using StudyControlApp.Model;
using StudyControlApp.Model.DataStructures;
using StudyControlApp.Utils;

namespace StudyControlApp.ViewModel
{
    class MainWindowViewModel: INotifyPropertyChanged
    {
        #region Properties
        public bool NotRunning { get; private set; }
        public bool NotLogging { get; private set; }
        public string HoloLensAddr { get; set; } = "192.168.1.50";
        public string ParticipantID { get; set; } = "-1";
        public string Condition { get; set; } = "-1";
        public FixedSizeObservablelist<string> OscMessages { get; }

        public StudyConditions CurrentCondition { get; set; }

        public Brush BackgroundColor { get; private set; }

        #endregion

        #region Commands
        private ICommand startCommand;
        public ICommand StartCommand
        {
            get { return startCommand ?? (startCommand = new RelayCommand(call => StartServer())); }
        }

        private ICommand stopCommand;
        public ICommand StopCommand
        {
            get { return stopCommand ?? (stopCommand = new RelayCommand(call => StopServer())); }
        }

        private ICommand controlCommand;
        public ICommand ControlCommand
        {
            get { return controlCommand ?? (controlCommand = new RelayCommand(command => SendToHL(command))); }
        }

        #endregion

        public enum StudyConditions
        {
            A,
            B,
            C
        };

        private OscController oscController;
        private DataLogger dataLogger;

        public MainWindowViewModel()
        {
            
            NotRunning = true;
            NotLogging = true;
            OscMessages = new FixedSizeObservablelist<string>(20);
            BackgroundColor = Brushes.WhiteSmoke;
        }


        private void StartServer()
        {
            oscController = new OscController(HoloLensAddr);
            oscController.StartReceiving();
            oscController.OnDataReceived +=LogDataReceived;
            NotRunning = false;
            OnPropertyChanged(nameof(NotRunning));
        }

        private void LogDataReceived(OscPacket data)
        {
            OscMessages.Add(data.ToString());
            dataLogger?.AddData(data);
        }

        private void StopServer()
        {
            NotRunning = true;
            OnPropertyChanged(nameof(NotRunning));
        }

        private void SendToHL(object command)
        {
             oscController.SendCommand((string)command);

            if (command.Equals("StartLog"))
            {
                if(dataLogger != null)
                    return;
                BackgroundColor = Brushes.OrangeRed;
                OnPropertyChanged(nameof(BackgroundColor));
                dataLogger = new DataLogger(ParticipantID+"-"+CurrentCondition);
                NotLogging = false;
                OnPropertyChanged(nameof(NotLogging));
            }
            else if (command.Equals("StopLog"))
            {
                if (dataLogger == null)
                    return;

                oscController.SendCommand((string)command);

                dataLogger.SaveData();
                dataLogger = null;
                BackgroundColor = Brushes.WhiteSmoke;
                OnPropertyChanged(nameof(BackgroundColor));
                NotLogging = true;
                OnPropertyChanged(nameof(NotLogging));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
