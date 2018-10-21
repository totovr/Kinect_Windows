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

namespace CamaraWebSencilla
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        KinectSensor miKinect; //Variable 

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MostrarVideo_Loaded(object sender, RoutedEventArgs e)
        {
            miKinect = KinectSensor.KinectSensors.FirstOrDefault(); // almacenamos los datos de los sensores
            miKinect.Start(); //Inicie 
            miKinect.ColorStream.Enable();  //Activa la camara RGB 648x480 Fps30
            miKinect.ColorFrameReady += miKinect_ColorFrameReady; // indica que va a hacer el flujo de eventos
            
        }

        void miKinect_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)//Almacena datos en la propiedad e
        {
            using (ColorImageFrame frameImagen = e.OpenColorImageFrame())//Genera una imagen y lo borra para generar el video
            {
                if (frameImagen == null) return;//Si es valor nulo

                byte[] datosColor = new byte[frameImagen.PixelDataLength];//Array de 8 bits 0-255 (8*8*8)

                frameImagen.CopyPixelDataTo(datosColor);//el array se va a guardar en datosColor

                MostrarVideo.Source = BitmapSource.Create(
                    frameImagen.Width, frameImagen.Height,//ancho y alto de la imagen
                    96,//puntos por pulgada horizontales
                    96,//verticales
                    PixelFormats.Bgr32,//formato de los pixeles
                    null, //paleta del bitmap
                    datosColor,//array de bits
                    frameImagen.Width * frameImagen.BytesPerPixel//medida de la multiplicacion del ancho
                    );

            }
        }
    }
}
