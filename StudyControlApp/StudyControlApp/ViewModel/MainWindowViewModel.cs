using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
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
        public string HoloLensAddr { get; set; }
        public string ParticipantID { get; set; }
        public string Condition { get; set; }
        public FixedSizeObservablelist<string> OscMessages { get; }

        private ICommand startCommand;
        public ICommand StartCommand
        {
            get { return startCommand ?? (startCommand = new RelayCommand(call => StartStudy())); }
        }

        private ICommand stopCommand;
        public ICommand StopCommand
        {
            get { return stopCommand ?? (stopCommand = new RelayCommand(call => StopStudy())); }
        }

        private ICommand controlCommand;
        public ICommand ControlCommand
        {
            get { return controlCommand ?? (controlCommand = new RelayCommand(command => SendToHL(command))); }
        }


        #endregion

        private OscController oscController;


        public MainWindowViewModel()
        {
            
            NotRunning = true;
        }


        private void StartStudy()
        {
            oscController = new OscController(HoloLensAddr);
            oscController.StartReceiving();
            oscController.OnDataReceived += data => OscMessages.Add(data.ToString());
            NotRunning = false;
            OnPropertyChanged(nameof(NotRunning));
        }

        private void StopStudy()
        {
            NotRunning = true;
            OnPropertyChanged(nameof(NotRunning));
        }

        private void SendToHL(object command)
        {
             oscController.SendCommand((string)command);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
