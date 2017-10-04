﻿using System;
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

namespace ControlandoColores
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        KinectSensor miKinect;//Variable de objecto donde guardamos los datos que llegan del kinect

        public MainWindow()
        {
            InitializeComponent();
        }

        
        private void Image_Loaded(object sender, RoutedEventArgs e)
        {
            if(KinectSensor.KinectSensors.Count ==0)// comprueba que este conectado el kinect
            {
                MessageBox.Show("No se ha detectado ningun Kinect", "Visor de Camara");
                Application.Current.Shutdown();
            }

            try
            {
                miKinect = KinectSensor.KinectSensors.FirstOrDefault(); // almacena los datos dentro esta variable
                miKinect.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);//Aqui si indicamos el formato RGB que vamos a utilizar dentro del envio de datos
                miKinect.Start();
            }
            catch
            {
                MessageBox.Show("Ocurrio un error al inicar Kinect", "Visor de Camara");
                Application.Current.Shutdown();
            }

            miKinect.ColorFrameReady += miKinect_ColorFrameReady;
        }

        byte[] datosColor = null; //Almacenamos los datos que se arrojan del frame en esta  variable
        WriteableBitmap bitmapEficiente = null; //Aqui vamos a guardar la imagen que mostramos
        
        int controlAzul = 0; //Variable del slider
        int controlRojo = 0;
        int controlVerde = 0;
        int nuevoValor; //Valor del slider mas el de a varible

        void miKinect_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame framesImagen = e.OpenColorImageFrame())
            {
                if (framesImagen == null) return;

                if (datosColor == null)
                    datosColor = new byte[framesImagen.PixelDataLength];

                framesImagen.CopyPixelDataTo(datosColor);


                for (int i = 0; i < datosColor.Length; i = i + 4)
                {
                    nuevoValor = datosColor[i] + controlAzul;
                    if (nuevoValor < 0) nuevoValor = 0;
                    if (nuevoValor > 255) nuevoValor = 255;
                    datosColor[i] = (byte)nuevoValor;

                    nuevoValor = datosColor[i + 1] + controlVerde;
                    if (nuevoValor < 0) nuevoValor = 0;
                    if (nuevoValor > 255) nuevoValor = 255;
                    datosColor[i + 1] = (byte)nuevoValor;

                    nuevoValor = datosColor[i + 2] + controlRojo;
                    if (nuevoValor < 0) nuevoValor = 0;
                    if (nuevoValor > 255) nuevoValor = 255;
                    datosColor[i + 2] = (byte)nuevoValor;
                }

                if (grabarFoto)
                {
                    bitmapImagen = BitmapSource.Create(
                        framesImagen.Width, framesImagen.Height, 96, 96, PixelFormats.Bgr32, null,
                        datosColor, framesImagen.Width * framesImagen.BytesPerPixel);
                    grabarFoto = false;
                }

                if (bitmapEficiente == null)
                {
                    bitmapEficiente = new WriteableBitmap(
                        framesImagen.Width,
                        framesImagen.Height,
                        96,
                        96,
                        PixelFormats.Bgr32,
                        null);
                    colorStream.Source = bitmapEficiente;
                }

                bitmapEficiente.WritePixels(
                    new Int32Rect(0, 0, framesImagen.Width, framesImagen.Height),
                    datosColor,
                    framesImagen.Width * framesImagen.BytesPerPixel,
                    0
                    );

            }

           
        }

        bool grabarFoto;
        BitmapSource bitmapImagen = null;

        private void Button_Click(object sender, RoutedEventArgs e)//Con este evento vamos a guardar la imagen al momento de que se presione en el boton click
        {
            grabarFoto = true;

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "capturaDeKinect";
            dlg.DefaultExt = ".jpg";
            dlg.Filter = "Pictures (.jpg)|*.jpg";

            if (dlg.ShowDialog() == true)
            {
                string nombreArchivo = dlg.FileName;
                using (FileStream stream = new FileStream(nombreArchivo, FileMode.Create))
                {
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmapImagen));
                    encoder.Save(stream);
                }
            }
        }

        private void sliderAzul_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            controlAzul = (int)sliderAzul.Value;
        }

        private void sliderRojo_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            controlRojo = (int)sliderRojo.Value;
        }

        private void sliderVerde_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            controlVerde = (int)sliderVerde.Value;
        }
        ///////////


        

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            
                 controlAzul = 0; //Variable del slider
                 controlRojo = 0;
                 controlVerde = 0;

                  
                 
                 
            
        }
    }
}
