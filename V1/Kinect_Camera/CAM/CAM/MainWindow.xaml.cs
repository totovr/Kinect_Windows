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

namespace CAM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        KinectSensor miKinect; //variable de objeto

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) //evento para ver imagen
        {
            miKinect = KinectSensor.KinectSensors[0]; //guardamos las varibles de los sensores en miKinect
            miKinect.Start(); //comenzamos el Kinect
            miKinect.ColorStream.Enable(); //formato RGB
            miKinect.ColorFrameReady += miKinect_ColorFrameReady; //Evento    
        }

        void miKinect_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e) //Lo esta guardando en la propiedad e
        {
            using (ColorImageFrame frameImagen = e.OpenColorImageFrame()) { //Usamos 
                if (frameImagen == null) return;

                byte[] datosColor = new byte[frameImagen.PixelDataLength];

                frameImagen.CopyPixelDataTo(datosColor);

                mostrarVideo.Source = BitmapSource.Create(  //Bitmap para mostrarlo en la ventana que generamos
                    frameImagen.Width, frameImagen.Height,
                    96,
                    96,
                    PixelFormats.Bgr32,
                    null, //Paleta del bitmap
                    datosColor, //array que generamos en la parte de arriba
                    frameImagen.Width * frameImagen.BytesPerPixel
                    );
            }
        }
    }
}
