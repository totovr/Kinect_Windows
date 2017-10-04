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
using System.IO;

using System.IO.Ports; // Dont forget.

namespace MapearCu
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPort serialPort1 = new SerialPort(); //We create a new class

        KinectSensor miKinect;

        public MainWindow()
        {
            // Configuramos el puerto serie.
            serialPort1.BaudRate = 115200; // Speed of Baudios, we use the same in the Arduino
            serialPort1.PortName = "COM3"; // Port COM3, in my case is the one that use the Arduino in my Laptop.
            serialPort1.Parity = Parity.None; // We dont use parity .
            serialPort1.DataBits = 8; // 8 Bits.
            serialPort1.StopBits = StopBits.Two; // it works better with 2 bits of Stop an start.

            // Open the port while this application is Open.
            if (!serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.Open();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (KinectSensor.KinectSensors.Count == 0)
            {
                MessageBox.Show("Kinect Initialization Failure", "Articulation visor position");
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
                MessageBox.Show("Kinect Initialization Failure", "Articulation visor position");
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

                    //Estas joints las generamos para mapear el cuerpo

                    Joint ShoulderCenterJoint = esqueleto.Joints[JointType.ShoulderCenter]; // Base del cuello
                    Joint HeadJoint = esqueleto.Joints[JointType.Head]; // Cabeza
                    Joint ShoulderLeftJoint = esqueleto.Joints[JointType.ShoulderLeft]; // Shoulder Left
                    Joint ElbowLeftJoint = esqueleto.Joints[JointType.ElbowLeft]; // Elbow Left
                    Joint WristLeftJoint = esqueleto.Joints[JointType.WristLeft]; // Wrist Left
                    Joint HandLeftJoint = esqueleto.Joints[JointType.HandLeft]; // Hand Left
                    //Centro
                    Joint SpineJoint = esqueleto.Joints[JointType.Spine]; // Spine
                    Joint HipCJoint = esqueleto.Joints[JointType.HipCenter]; // Hip Center
                    Joint HipLJoint = esqueleto.Joints[JointType.HipLeft]; // HipLeft
                    Joint HipRJoint = esqueleto.Joints[JointType.HipRight]; // Hip Right
                    //Parte inferior
                    Joint KneeRJoint = esqueleto.Joints[JointType.KneeRight]; // Knee Right
                    Joint AnkleRJoint = esqueleto.Joints[JointType.AnkleRight]; // Ankle Right
                    Joint FootRJoint = esqueleto.Joints[JointType.FootRight]; // Foot Right
                    Joint KneeLJoint = esqueleto.Joints[JointType.KneeLeft]; // Knee Left
                    Joint AnkleLJoint = esqueleto.Joints[JointType.AnkleLeft]; // Ankle Left
                    Joint FootLJoint = esqueleto.Joints[JointType.FootLeft]; // Foot Left
                    

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

                    /////Generamos el punto X y Y para el hombro (Vector)

                    Xhombro = puntoHombro.X;
                    Yhombro = puntoHombro.Y;

                    canvasesqueleto.Children.Add(Humero);//Lo dibujamos en el canvas

                    //Coordenadas en XYZ

                    SkeletonPoint handPosition = handJoint.Position; //Almacenar la posicion 3D, variables de este tipo se guardan en tipo SkeletonPoint
                    mensajeMa = string.Format("Right Hand: X:{0:0.0} Y:{1:0.0} Z:{2:0.0}", handPosition.X, handPosition.Y, handPosition.Z);

                    SkeletonPoint wristPosition = wristJoint.Position; //Almacenar la posicion 3D, variables de este tipo se guardan en tipo SkeletonPoint
                    mensajeMu = string.Format("Right Wrist: X:{0:0.0} Y:{1:0.0} Z:{2:0.0}", wristPosition.X, wristPosition.Y, wristPosition.Z);

                    SkeletonPoint elbowPosition = elbowJoint.Position; //Almacenar la posicion 3D, variables de este tipo se guardan en tipo SkeletonPoint
                    mensajeC = string.Format("Right Elbow: X:{0:0.0} Y:{1:0.0} Z:{2:0.0}", elbowPosition.X, elbowPosition.Y, elbowPosition.Z);

                    SkeletonPoint shoulderPosition = shoulderJoint.Position; //Almacenar la posicion 3D, variables de este tipo se guardan en tipo SkeletonPoint
                    mensajeH = string.Format("Right Shoulder: X:{0:0.0} Y:{1:0.0} Z:{2:0.0}", shoulderPosition.X, shoulderPosition.Y, shoulderPosition.Z);

                    //Vector3D RightHand = new Vector3D(body.Joints[JointType.ElbowRight].Position.X, body.Joints[JointType.ElbowRight].Position.Y, body.Joints[JointType.ElbowRight].Position.Z);

                    //Calculamos los vectores que necesitamos para la union del codo

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
                    AnguloUni = System.Convert.ToString(angleBetween); //Angulo
                   
                    //Generamos el mapa del cuerpo

                    //Hombro a Base del cuello derecho

                    Line BaseCuelloD = new Line();//Generamos la linea que va a mapear
                    BaseCuelloD.Stroke = new SolidColorBrush(Colors.Blue); //Stroke para indicar el color de la linea
                    BaseCuelloD.StrokeThickness = 5; //ancho de la linea

                    ColorImagePoint puntoBC = miKinect.CoordinateMapper.MapSkeletonPointToColorPoint(ShoulderCenterJoint.Position, ColorImageFormat.RgbResolution640x480Fps30); //Cambiamos de 3 dimensiones a dos dimensiones de la varibale almacenada -puntoMano-
                    BaseCuelloD.X1 = puntoBC.X;
                    BaseCuelloD.Y1 = puntoBC.Y;

                    BaseCuelloD.X2 = puntoHombro.X;
                    BaseCuelloD.Y2 = puntoHombro.Y;

                    canvasesqueleto.Children.Add(BaseCuelloD);//Lo dibujamos en el canvas
                    
                    //Cabeza

                    Line Cabeza = new Line();//Generamos la linea que va a mapear
                    Cabeza.Stroke = new SolidColorBrush(Colors.GreenYellow); //Stroke para indicar el color de la linea
                    Cabeza.StrokeThickness = 5; //ancho de la linea

                    ColorImagePoint puntoCabeza = miKinect.CoordinateMapper.MapSkeletonPointToColorPoint(HeadJoint.Position, ColorImageFormat.RgbResolution640x480Fps30); //Cambiamos de 3 dimensiones a dos dimensiones de la varibale almacenada -puntoMano-
                    Cabeza.X1 = puntoCabeza.X;
                    Cabeza.Y1 = puntoCabeza.Y;

                    Cabeza.X2 = puntoBC.X;
                    Cabeza.Y2 = puntoBC.Y;

                    canvasesqueleto.Children.Add(Cabeza);//Lo dibujamos en el canvas

                    //Hombro a Base del cuello izquierdo========

                    Line BaseCuelloI = new Line();//Generamos la linea que va a mapear
                    BaseCuelloI.Stroke = new SolidColorBrush(Colors.GreenYellow); //Stroke para indicar el color de la linea
                    BaseCuelloI.StrokeThickness = 5; //ancho de la linea

                    ColorImagePoint puntoShoulderL = miKinect.CoordinateMapper.MapSkeletonPointToColorPoint(ShoulderLeftJoint.Position, ColorImageFormat.RgbResolution640x480Fps30); //Cambiamos de 3 dimensiones a dos dimensiones de la varibale almacenada -puntoMano-
                    BaseCuelloI.X1 = puntoShoulderL.X;
                    BaseCuelloI.Y1 = puntoShoulderL.Y;

                    BaseCuelloI.X2 = puntoBC.X;
                    BaseCuelloI.Y2 = puntoBC.Y;

                    canvasesqueleto.Children.Add(BaseCuelloI);//Lo dibujamos en el canvas

                    //Carpo Izquierdo

                    Line CarpoI = new Line();//Generamos la linea que va a mapear
                    CarpoI.Stroke = new SolidColorBrush(Colors.GreenYellow); //Stroke para indicar el color de la linea
                    CarpoI.StrokeThickness = 5; //ancho de la linea

                    ColorImagePoint puntoElbowLeftJoint = miKinect.CoordinateMapper.MapSkeletonPointToColorPoint(ElbowLeftJoint.Position, ColorImageFormat.RgbResolution640x480Fps30); //Cambiamos de 3 dimensiones a dos dimensiones de la varibale almacenada -puntoMano-
                    CarpoI.X1 = puntoElbowLeftJoint.X;
                    CarpoI.Y1 = puntoElbowLeftJoint.Y;

                    CarpoI.X2 = puntoShoulderL.X;
                    CarpoI.Y2 = puntoShoulderL.Y;

                    canvasesqueleto.Children.Add(CarpoI);//Lo dibujamos en el canvas

                    //Radio Izquierdo

                    Line RadioI = new Line();//Generamos la linea que va a mapear
                    RadioI.Stroke = new SolidColorBrush(Colors.GreenYellow); //Stroke para indicar el color de la linea
                    RadioI.StrokeThickness = 5; //ancho de la linea

                    ColorImagePoint puntoWristLeftJoint = miKinect.CoordinateMapper.MapSkeletonPointToColorPoint(WristLeftJoint.Position, ColorImageFormat.RgbResolution640x480Fps30); //Cambiamos de 3 dimensiones a dos dimensiones de la varibale almacenada -puntoMano-
                    RadioI.X1 = puntoWristLeftJoint.X;
                    RadioI.Y1 = puntoWristLeftJoint.Y;

                    RadioI.X2 = puntoElbowLeftJoint.X;
                    RadioI.Y2 = puntoElbowLeftJoint.Y;

                    canvasesqueleto.Children.Add(RadioI);//Lo dibujamos en el canvas

                    //Mano Izquierdo

                    Line ManoI = new Line();//Generamos la linea que va a mapear
                    ManoI.Stroke = new SolidColorBrush(Colors.GreenYellow); //Stroke para indicar el color de la linea
                    ManoI.StrokeThickness = 5; //ancho de la linea

                    ColorImagePoint puntoHandLeftJoint = miKinect.CoordinateMapper.MapSkeletonPointToColorPoint(HandLeftJoint.Position, ColorImageFormat.RgbResolution640x480Fps30); //Cambiamos de 3 dimensiones a dos dimensiones de la varibale almacenada -puntoMano-
                    ManoI.X1 = puntoHandLeftJoint.X;
                    ManoI.Y1 = puntoHandLeftJoint.Y;

                    ManoI.X2 = puntoWristLeftJoint.X;
                    ManoI.Y2 = puntoWristLeftJoint.Y;

                    canvasesqueleto.Children.Add(ManoI);//Lo dibujamos en el canvas

                    //Tronco

                    Line Tronco = new Line();//Generamos la linea que va a mapear
                    Tronco.Stroke = new SolidColorBrush(Colors.GreenYellow); //Stroke para indicar el color de la linea
                    Tronco.StrokeThickness = 5; //ancho de la linea

                    ColorImagePoint puntoSpineJoint = miKinect.CoordinateMapper.MapSkeletonPointToColorPoint(SpineJoint.Position, ColorImageFormat.RgbResolution640x480Fps30); //Cambiamos de 3 dimensiones a dos dimensiones de la varibale almacenada -puntoMano-
                    Tronco.X1 = puntoSpineJoint.X;
                    Tronco.Y1 = puntoSpineJoint.Y;

                    Tronco.X2 = puntoBC.X;
                    Tronco.Y2 = puntoBC.Y;

                    canvasesqueleto.Children.Add(Tronco);//Lo dibujamos en el canvas

                    //Parte inferior del cuerpo
                    //Sacro

                    Line Sacro = new Line();//Generamos la linea que va a mapear
                    Sacro.Stroke = new SolidColorBrush(Colors.GreenYellow); //Stroke para indicar el color de la linea
                    Sacro.StrokeThickness = 5; //ancho de la linea

                    ColorImagePoint puntoHipCJoint = miKinect.CoordinateMapper.MapSkeletonPointToColorPoint(HipCJoint.Position, ColorImageFormat.RgbResolution640x480Fps30); //Cambiamos de 3 dimensiones a dos dimensiones de la varibale almacenada -puntoMano-
                    Sacro.X1 = puntoHipCJoint.X;
                    Sacro.Y1 = puntoHipCJoint.Y;

                    Sacro.X2 = puntoSpineJoint.X;
                    Sacro.Y2 = puntoSpineJoint.Y;

                    canvasesqueleto.Children.Add(Sacro);//Lo dibujamos en el canvas

                    //Cabeza de femur Izquierdo

                    Line CFemurI = new Line();//Generamos la linea que va a mapear
                    CFemurI.Stroke = new SolidColorBrush(Colors.GreenYellow); //Stroke para indicar el color de la linea
                    CFemurI.StrokeThickness = 5; //ancho de la linea

                    ColorImagePoint puntoHipLJoint = miKinect.CoordinateMapper.MapSkeletonPointToColorPoint(HipLJoint.Position, ColorImageFormat.RgbResolution640x480Fps30); //Cambiamos de 3 dimensiones a dos dimensiones de la varibale almacenada -puntoMano-
                    CFemurI.X1 = puntoHipLJoint.X;
                    CFemurI.Y1 = puntoHipLJoint.Y;

                    CFemurI.X2 = puntoHipCJoint.X;//Spine
                    CFemurI.Y2 = puntoHipCJoint.Y;

                    canvasesqueleto.Children.Add(CFemurI);//Lo dibujamos en el canvas

                    //Cabeza de femur Derecho

                    Line CFemurD = new Line();//Generamos la linea que va a mapear
                    CFemurD.Stroke = new SolidColorBrush(Colors.GreenYellow); //Stroke para indicar el color de la linea
                    CFemurD.StrokeThickness = 5; //ancho de la linea

                    ColorImagePoint puntoHipRJoint = miKinect.CoordinateMapper.MapSkeletonPointToColorPoint(HipRJoint.Position, ColorImageFormat.RgbResolution640x480Fps30); //Cambiamos de 3 dimensiones a dos dimensiones de la varibale almacenada -puntoMano-
                    CFemurD.X1 = puntoHipRJoint.X;
                    CFemurD.Y1 = puntoHipRJoint.Y;

                    CFemurD.X2 = puntoHipCJoint.X;
                    CFemurD.Y2 = puntoHipCJoint.Y;

                    canvasesqueleto.Children.Add(CFemurD);//Lo dibujamos en el canvas

                    //Piernas

                    //Derecha
                    //Femur Derecho

                    Line FemurD = new Line();//Generamos la linea que va a mapear
                    FemurD.Stroke = new SolidColorBrush(Colors.GreenYellow); //Stroke para indicar el color de la linea
                    FemurD.StrokeThickness = 5; //ancho de la linea

                    ColorImagePoint puntoKneeRJoint = miKinect.CoordinateMapper.MapSkeletonPointToColorPoint(KneeRJoint.Position, ColorImageFormat.RgbResolution640x480Fps30); //Cambiamos de 3 dimensiones a dos dimensiones de la varibale almacenada -puntoMano-
                    FemurD.X1 = puntoKneeRJoint.X;
                    FemurD.Y1 = puntoKneeRJoint.Y;

                    FemurD.X2 = puntoHipRJoint.X;
                    FemurD.Y2 = puntoHipRJoint.Y;

                    canvasesqueleto.Children.Add(FemurD);//Lo dibujamos en el canvas

                    //Tibia derecha

                    Line TibiaD = new Line();//Generamos la linea que va a mapear
                    TibiaD.Stroke = new SolidColorBrush(Colors.GreenYellow); //Stroke para indicar el color de la linea
                    TibiaD.StrokeThickness = 5; //ancho de la linea

                    ColorImagePoint puntoAnkleRJoint = miKinect.CoordinateMapper.MapSkeletonPointToColorPoint(AnkleRJoint.Position, ColorImageFormat.RgbResolution640x480Fps30); //Cambiamos de 3 dimensiones a dos dimensiones de la varibale almacenada -puntoMano-
                    TibiaD.X1 = puntoAnkleRJoint.X;
                    TibiaD.Y1 = puntoAnkleRJoint.Y;

                    TibiaD.X2 = puntoKneeRJoint.X;
                    TibiaD.Y2 = puntoKneeRJoint.Y;

                    canvasesqueleto.Children.Add(TibiaD);//Lo dibujamos en el canvas

                    //Pie derecho

                    Line PieD = new Line();//Generamos la linea que va a mapear
                    PieD.Stroke = new SolidColorBrush(Colors.GreenYellow); //Stroke para indicar el color de la linea
                    PieD.StrokeThickness = 5; //ancho de la linea

                    ColorImagePoint puntoFootRJoint = miKinect.CoordinateMapper.MapSkeletonPointToColorPoint(FootRJoint.Position, ColorImageFormat.RgbResolution640x480Fps30); //Cambiamos de 3 dimensiones a dos dimensiones de la varibale almacenada -puntoMano-
                    PieD.X1 = puntoFootRJoint.X;
                    PieD.Y1 = puntoFootRJoint.Y;

                    PieD.X2 = puntoAnkleRJoint.X;
                    PieD.Y2 = puntoAnkleRJoint.Y;

                    canvasesqueleto.Children.Add(PieD);//Lo dibujamos en el canvas

                    //Izquierda
                    //Femur Izquierdo

                    Line FemurI = new Line();//Generamos la linea que va a mapear
                    FemurI.Stroke = new SolidColorBrush(Colors.GreenYellow); //Stroke para indicar el color de la linea
                    FemurI.StrokeThickness = 5; //ancho de la linea

                    ColorImagePoint puntoKneeLJoint = miKinect.CoordinateMapper.MapSkeletonPointToColorPoint(KneeLJoint.Position, ColorImageFormat.RgbResolution640x480Fps30); //Cambiamos de 3 dimensiones a dos dimensiones de la varibale almacenada -puntoMano-
                    FemurI.X1 = puntoKneeLJoint.X;
                    FemurI.Y1 = puntoKneeLJoint.Y;

                    FemurI.X2 = puntoHipLJoint.X;
                    FemurI.Y2 = puntoHipLJoint.Y;

                    canvasesqueleto.Children.Add(FemurI);//Lo dibujamos en el canvas

                    //Tibia Izquierda

                    Line TibiaI = new Line();//Generamos la linea que va a mapear
                    TibiaI.Stroke = new SolidColorBrush(Colors.GreenYellow); //Stroke para indicar el color de la linea
                    TibiaI.StrokeThickness = 5; //ancho de la linea

                    ColorImagePoint puntoAnkleLJoint = miKinect.CoordinateMapper.MapSkeletonPointToColorPoint(AnkleLJoint.Position, ColorImageFormat.RgbResolution640x480Fps30); //Cambiamos de 3 dimensiones a dos dimensiones de la varibale almacenada -puntoMano-
                    TibiaI.X1 = puntoAnkleLJoint.X;
                    TibiaI.Y1 = puntoAnkleLJoint.Y;

                    TibiaI.X2 = puntoKneeLJoint.X;
                    TibiaI.Y2 = puntoKneeLJoint.Y;

                    canvasesqueleto.Children.Add(TibiaI);//Lo dibujamos en el canvas

                    //Pie Izquierdo

                    Line PieI = new Line();//Generamos la linea que va a mapear
                    PieI.Stroke = new SolidColorBrush(Colors.GreenYellow); //Stroke para indicar el color de la linea
                    PieI.StrokeThickness = 5; //ancho de la linea

                    ColorImagePoint puntoFootLJoint = miKinect.CoordinateMapper.MapSkeletonPointToColorPoint(FootLJoint.Position, ColorImageFormat.RgbResolution640x480Fps30); //Cambiamos de 3 dimensiones a dos dimensiones de la varibale almacenada -puntoMano-
                    PieI.X1 = puntoFootLJoint.X;
                    PieI.Y1 = puntoFootLJoint.Y;

                    PieI.X2 = puntoAnkleLJoint.X;
                    PieI.Y2 = puntoAnkleLJoint.Y;

                    canvasesqueleto.Children.Add(PieI);//Lo dibujamos en el canvas

                }
            }

            textBlockEstatusMa.Text = mensajeMa;
            textBlockEstatusMu.Text = mensajeMu;
            textBlockEstatusC.Text = mensajeC;
            textBlockEstatusH.Text = mensajeH;
            VectorAngle.Text = AnguloUni;

        } //Here I end of map the body

        

        private void ON_Click_1(object sender, RoutedEventArgs e)
        {

            byte[] mBuffer = Encoding.ASCII.GetBytes("Led_ON");
            serialPort1.Write(mBuffer, 0, mBuffer.Length);

        }

        private void OFF_Click_1(object sender, RoutedEventArgs e)
        {
            byte[] mBuffer = Encoding.ASCII.GetBytes("Led_OFF");
            serialPort1.Write(mBuffer, 0, mBuffer.Length);
        }
    }
}
