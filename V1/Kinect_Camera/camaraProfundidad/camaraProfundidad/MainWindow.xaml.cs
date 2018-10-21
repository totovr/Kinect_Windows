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

using Microsoft.Kinect;
using System.IO;


namespace camaraProfundidad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        KinectSensor miKinect;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (KinectSensor.KinectSensors.Count == 0)
            {
                MessageBox.Show("No se detecta ningun Kinect", "Visor de Camara");
                Application.Current.Shutdown();
            }

            try
            {
                miKinect = KinectSensor.KinectSensors.FirstOrDefault();//Comenzar a usar el Kinect
                miKinect.DepthStream.Enable();
                miKinect.Start();
                miKinect.DepthFrameReady += miKinect_DepthFrameReady; ;
            }
            catch
            {
                MessageBox.Show("Fallo al inicializar kinect", "Visor de KInect");
                Application.Current.Shutdown();
            }

        }

        short[] datosDistancia = null; //Aqui guardamos los datos que recibimos los datos de distancia
        byte[] colorImagenDistancia = null; // Se guarda cada una de las propiedades
        WriteableBitmap bitmapImagenDistancia = null;//Sirve para mostrar los frames

        void miKinect_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
