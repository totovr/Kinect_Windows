using System.Windows;
using System;
using Microsoft.Kinect;
using System.ComponentModel;

namespace KinectSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private KinectSensor kinect = null;
        private string statusText = null;

        public MainWindow()
        {
            // Check the kinect that is connected 
            this.kinect = KinectSensor.GetDefault();
            this.kinect.IsAvailableChanged += this.Sensor_IsAvailableChanged;
            this.StatusText = this.kinect.IsAvailable ? "Hello World! Kinect is Ready!" : "Goodbye World! Kinect is Unavailable!";
            this.DataContext = this;
            // Is a Visual Studio–generated piece of code used to bootstrap the UI
            InitializeComponent();
            // Turn on the Kinect
            this.kinect.Open();

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Sensor_IsAvailableChanged(object sender, IsAvailableChangedEventArgs e)
        {
            this.StatusText = this.kinect.IsAvailable ? "Hello World! Kinect is Ready!" :
            "Goodbye World! Kinect is Unavailable!";
        }

        public string StatusText
        {
            get
            {
                return this.statusText;
            }
            set
            {
                if (this.statusText != value)
                {
                    this.statusText = value;
                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("StatusText"));
                    }
                }
            }
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (this.kinect != null)
            {
                this.kinect.Close();
                this.kinect = null;
            }
        }

    }
}
