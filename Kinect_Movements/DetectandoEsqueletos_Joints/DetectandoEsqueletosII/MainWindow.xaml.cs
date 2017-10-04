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

namespace DetectandoEsqueletosII
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
                MessageBox.Show("Ningun kinect detectado", "Visor de Posicion de Articulasion");
                Application.Current.Shutdown();
                return;
            }

            miKinect = KinectSensor.KinectSensors.FirstOrDefault(); //Seleccionar el primero que se encuentra conectado 

            try
            {
                miKinect.SkeletonStream.Enable(); //Para que comience a detectar esqueletos
                miKinect.Start();
            }
            catch
            {
                MessageBox.Show("Fallo en la Iniciacion de Kinect", "Visor de Posicion de Articulasion");
                Application.Current.Shutdown();
            }

            miKinect.SkeletonFrameReady += miKinect_SkeletonFrameReady;
        }

        void miKinect_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            canvasesqueleto.Children.Clear();//Limpiamos la linea que estamos generando 
            Skeleton[] esqueletos = null;// Guardar los datos del esqueleto

            using (SkeletonFrame framesEsqueleto = e.OpenSkeletonFrame())// Detectamos que exista algun valor
            {
                if (framesEsqueleto != null)
                {
                    esqueletos = new Skeleton[framesEsqueleto.SkeletonArrayLength];
                    framesEsqueleto.CopySkeletonDataTo(esqueletos);
                }
            }

            if (esqueletos == null) return; //Detectamos si no hay esqueletos

            foreach (Skeleton esqueleto in esqueletos)
            {
                if (esqueleto.TrackingState == SkeletonTrackingState.Tracked) //guardamos los datos en la variable esqueleto
                {
                    Joint handJoint = esqueleto.Joints[JointType.HandRight]; // mano
                    Joint wristJoint = esqueleto.Joints[JointType.WristRight];// muneca
                    Joint elbowJoint = esqueleto.Joints[JointType.ElbowRight];// codo                    
                    Joint shoulderJoint = esqueleto.Joints[JointType.ShoulderRight]; // Hombro

                    /////Linea que se va a generar para el humero 

                    Line Carpo = new Line();//Generamos la linea que va a mapear
                    Carpo.Stroke = new SolidColorBrush(Colors.Blue); //Stroke para indicar el color de la linea
                    Carpo.StrokeThickness = 5; //ancho de la linea

                    ColorImagePoint puntoMano = miKinect.CoordinateMapper.MapSkeletonPointToColorPoint(handJoint.Position, ColorImageFormat.RgbResolution640x480Fps30); //Cambiamos de 3 dimensiones a dos dimensiones de la varibale almacenada -puntoMano-
                    Carpo.X1 = puntoMano.X;
                    Carpo.Y1 = puntoMano.Y;
                    /////
                    ColorImagePoint puntoMuneca = miKinect.CoordinateMapper.MapSkeletonPointToColorPoint(wristJoint.Position, ColorImageFormat.RgbResolution640x480Fps30); //Cambiamos de 3 dimensiones a dos dimensiones de la varibale almacenada -puntoMano-
                    Carpo.X2 = puntoMuneca.X;
                    Carpo.Y2 = puntoMuneca.Y;

                    canvasesqueleto.Children.Add(Carpo);//Lo dibujamos en el canvas

                    /////Linea que se va a generar para el radio y cubito

                    Line Radio = new Line();//Generamos la linea que va a mapear
                    Radio.Stroke = new SolidColorBrush(Colors.Red); //Stroke para indicar el color de la linea
                    Radio.StrokeThickness = 5; //ancho de la linea

                    Radio.X1 = puntoMuneca.X;
                    Radio.Y1 = puntoMuneca.Y;

                    ColorImagePoint puntoCodo = miKinect.CoordinateMapper.MapSkeletonPointToColorPoint(elbowJoint.Position, ColorImageFormat.RgbResolution640x480Fps30); //Cambiamos de 3 dimensiones a dos dimensiones de la varibale almacenada -elBowJoint-
                    Radio.X2 = puntoCodo.X;
                    Radio.Y2 = puntoCodo.Y;

                    canvasesqueleto.Children.Add(Radio);//Lo dibujamos en el canvas

                    /////Linea que se va a generar para el Humero

                    Line Humero = new Line();//Generamos la linea que va a mapear
                    Humero.Stroke = new SolidColorBrush(Colors.Green); //Stroke para indicar el color de la linea
                    Humero.StrokeThickness = 5; //ancho de la linea

                    Humero.X1 = puntoCodo.X;
                    Humero.Y1 = puntoCodo.Y;

                    ColorImagePoint puntoHombro = miKinect.CoordinateMapper.MapSkeletonPointToColorPoint(shoulderJoint.Position, ColorImageFormat.RgbResolution640x480Fps30); //Cambiamos de 3 dimensiones a dos dimensiones de la varibale almacenada -elBowJoint-
                    Humero.X2 = puntoHombro.X;
                    Humero.Y2 = puntoHombro.Y;

                    canvasesqueleto.Children.Add(Humero);//Lo dibujamos en el canvas

                }
            }
        }

    }
}
