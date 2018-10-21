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
using System.Windows.Media.Media3D;

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
                MessageBox.Show("Ningun kinect detectado", "Visor de Posicion de Articulacion");
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
            string mensajeMa = "No hay datos de esqueleto";
            string mensajeMu = "No hay datos de esqueleto";
            string mensajeC = "No hay datos de esqueleto";
            string mensajeH = "No hay datos de esqueleto";
            string AnguloUni = "No hay angulo";

            //Variables donde vamos a guardar los puntos para generar el vector
            //Los pongo de manera separada para evitar confusion

            float Xmuneca;
            float Ymuneca;
            float Xcodo;
            float Ycodo;
            float Xhombro;
            float Yhombro;
            
            //Variables para guardar los vectores

            float X1Ver;
            float Y1Ver;
            float X2Ver;
            float Y2Ver;

            //Comenzamos a detectar si hay algun individuo

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

                    //////Generamos el punto X y Y para la muneca

                    Xmuneca = puntoMuneca.X;
                    Ymuneca = puntoMuneca.Y;

                    /////

                    ColorImagePoint puntoCodo = miKinect.CoordinateMapper.MapSkeletonPointToColorPoint(elbowJoint.Position, ColorImageFormat.RgbResolution640x480Fps30); //Cambiamos de 3 dimensiones a dos dimensiones de la varibale almacenada -elBowJoint-
                    Radio.X2 = puntoCodo.X;
                    Radio.Y2 = puntoCodo.Y;

                    /////Generamos el punto X y Y para el codo

                    Xcodo = puntoCodo.X;
                    Ycodo = puntoCodo.Y;

                    canvasesqueleto.Children.Add(Radio);//Lo dibujamos en el canvas

                    /////Linea que se va a generar para el Humero

                    Line Humero = new Line();//Generamos la linea que va a mapear
                    Humero.Stroke = new SolidColorBrush(Colors.Green); //Stroke para indicar el color de la linea
                    Humero.StrokeThickness = 5; //ancho de la linea

                    Humero.X1 = puntoCodo.X;
                    Humero.Y1 = puntoCodo.Y;

                    /////

                    ColorImagePoint puntoHombro = miKinect.CoordinateMapper.MapSkeletonPointToColorPoint(shoulderJoint.Position, ColorImageFormat.RgbResolution640x480Fps30); //Cambiamos de 3 dimensiones a dos dimensiones de la varibale almacenada -elBowJoint-
                    Humero.X2 = puntoHombro.X;
                    Humero.Y2 = puntoHombro.Y;

                    /////Generamos el punto X y Y para el hombro

                    Xhombro = puntoHombro.X;
                    Yhombro = puntoHombro.Y;

                    canvasesqueleto.Children.Add(Humero);//Lo dibujamos en el canvas

                    //Coordenadas en XYZ

                    SkeletonPoint handPosition = handJoint.Position; //Almacenar la posicion 3D, variables de este tipo se guardan en tipo SkeletonPoint
                    mensajeMa = string.Format("Hand: X:{0:0.0} Y:{1:0.0} Z:{2:0.0}", handPosition.X, handPosition.Y, handPosition.Z);

                    SkeletonPoint wristPosition = wristJoint.Position; //Almacenar la posicion 3D, variables de este tipo se guardan en tipo SkeletonPoint
                    mensajeMu = string.Format("Wrist: X:{0:0.0} Y:{1:0.0} Z:{2:0.0}", wristPosition.X, wristPosition.Y, wristPosition.Z);

                    SkeletonPoint elbowPosition = elbowJoint.Position; //Almacenar la posicion 3D, variables de este tipo se guardan en tipo SkeletonPoint
                    mensajeC = string.Format("Elbow: X:{0:0.0} Y:{1:0.0} Z:{2:0.0}", elbowPosition.X, elbowPosition.Y, elbowPosition.Z);

                    SkeletonPoint shoulderPosition = shoulderJoint.Position; //Almacenar la posicion 3D, variables de este tipo se guardan en tipo SkeletonPoint
                    mensajeH = string.Format("Shoulder: X:{0:0.0} Y:{1:0.0} Z:{2:0.0}", shoulderPosition.X, shoulderPosition.Y, shoulderPosition.Z);

                    //Vector3D RightHand = new Vector3D(body.Joints[JointType.ElbowRight].Position.X, body.Joints[JointType.ElbowRight].Position.Y, body.Joints[JointType.ElbowRight].Position.Z);

                    //Calculamos los vectores que necesitamos

                    X1Ver = Xhombro - Xcodo;
                    Y1Ver = Yhombro - Ycodo;
                    X2Ver = Xmuneca - Xcodo;
                    Y2Ver = Xmuneca - Xcodo;

                    //Calcular el vector de para el codo
                    //Si funciona y da los grados en hexa

                    Vector vector1 = new Vector(X1Ver, Y1Ver);
                    Vector vector2 = new Vector(X2Ver, Y2Ver);
                    Double angleBetween;
                    angleBetween = Vector.AngleBetween(vector1, vector2);
                    AnguloUni = System.Convert.ToString(angleBetween);
                    
                }
            }
            textBlockEstatusMa.Text = mensajeMa;
            textBlockEstatusMu.Text = mensajeMu;
            textBlockEstatusC.Text = mensajeC;
            textBlockEstatusH.Text = mensajeH;
            VectorAngle.Text = AnguloUni;

        }

    }
}
