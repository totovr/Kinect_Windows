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
                    Joint handJoint = esqueleto.Joints[JointType.HandRight]; //JointType tiene guargadas todas las articulaciones
                    Joint elbowJoint = esqueleto.Joints[JointType.ElbowRight]; //JointType tiene guargadas todas las articulaciones

                    Line huesoBrazoDer = new Line();
                    huesoBrazoDer.Stroke = new SolidColorBrush(Colors.Red); //Stroke para indicar el color de la linea
                    huesoBrazoDer.StrokeThickness = 5; //ancho de la linea

                    ColorImagePoint puntoMano = miKinect.CoordinateMapper.MapSkeletonPointToColorPoint(handJoint.Position, ColorImageFormat.RgbResolution640x480Fps30); //Cambiamos de 3 dimensiones a dos dimensiones de la varibale almacenada -puntoMano-
                    huesoBrazoDer.X1 = puntoMano.X;
                    huesoBrazoDer.Y1 = puntoMano.Y;

                    ColorImagePoint puntoCodo = miKinect.CoordinateMapper.MapSkeletonPointToColorPoint(elbowJoint.Position, ColorImageFormat.RgbResolution640x480Fps30); //Cambiamos de 3 dimensiones a dos dimensiones de la varibale almacenada -elBowJoint-
                    huesoBrazoDer.X2 = puntoCodo.X;
                    huesoBrazoDer.Y2 = puntoCodo.Y;

                    canvasesqueleto.Children.Add(huesoBrazoDer);//Lo dibujamos en el canvas



                }
            }
        }

    }
}
