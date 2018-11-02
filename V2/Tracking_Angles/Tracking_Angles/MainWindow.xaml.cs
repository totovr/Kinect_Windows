﻿using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;
// Kinect dll
using Microsoft.Kinect;
using System;
using System.Windows.Shapes;
using System.Windows.Controls;

namespace Tracking_Angles
{
    public enum Mode
    {
        Color,
        Depth,
        Infrared
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Members 

        // Default mode 
        public Mode _mode = Mode.Color;

        // Create and object of the Kinect class
        KinectSensor _sensor;
        // Create and object to read the incoming frames
        MultiSourceFrameReader _reader;
        // Create a list to save the ID of the tracked bodys
        IList<Body> _bodies;

        bool _displayBody = false;

        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Event handlers

        // This window will be always updating in the .xaml 
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _sensor = KinectSensor.GetDefault();

            if (_sensor != null)
            {
                _sensor.Open();
                // Read the streams
                _reader = _sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color
                                                            | FrameSourceTypes.Depth
                                                            | FrameSourceTypes.Infrared
                                                            | FrameSourceTypes.Body);
                _reader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;
            }

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (_sensor != null)
            {
                _sensor.Close();
            }
        }

        void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            var reference = e.FrameReference.AcquireFrame();

            // Color
            using (var frame = reference.ColorFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    if (_mode == Mode.Color)
                    {
                        camera.Source = frame.ToBitmap();
                    }
                }
            }

            //// Depth
            //using (var frame = reference.DepthFrameReference.AcquireFrame())
            //{
            //    if (frame != null)
            //    {
            //        if (_mode == Mode.Depth)
            //        {
            //            camera.Source = frame.ToBitmap();
            //        }
            //    }
            //}

            //// Infrared
            //using (var frame = reference.InfraredFrameReference.AcquireFrame())
            //{
            //    if (frame != null)
            //    {
            //        if (_mode == Mode.Infrared)
            //        {
            //            camera.Source = frame.ToBitmap();
            //        }
            //    }
            //}

            // Body joints
            using (var frame = reference.BodyFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    canvas.Children.Clear();

                    _bodies = new Body[frame.BodyFrameSource.BodyCount];

                    frame.GetAndRefreshBodyData(_bodies);

                    foreach (var body in _bodies)
                    {
                        if (body != null)
                        {
                            if (body.IsTracked)
                            {
                                // Draw skeleton.
                                if (_displayBody)
                                {
                                    //canvas.DrawSkeleton(body);

                                    // COORDINATE MAPPING
                                    foreach (Joint joint in body.Joints.Values)
                                    {
                                        if (joint.TrackingState == TrackingState.Tracked)
                                        {
                                            // 3D space point
                                            // pack the X Y Z values
                                            CameraSpacePoint jointPosition = joint.Position;

                                            // 2D space point 
                                            Point point = new Point();

                                            if (_mode == Mode.Color)
                                            {
                                                ColorSpacePoint colorPoint = _sensor.CoordinateMapper.MapCameraPointToColorSpace(jointPosition);

                                                point.X = float.IsInfinity(colorPoint.X) ? 0 : colorPoint.X;
                                                point.Y = float.IsInfinity(colorPoint.Y) ? 0 : colorPoint.Y;
                                            } // Necessary to reviw this to adjust the mapping in the depth or rgb mode
                                            else if (_mode == Mode.Depth || _mode == Mode.Infrared) // Change the Image and Canvas dimensions to 512x424
                                            {
                                                DepthSpacePoint depthPoint = _sensor.CoordinateMapper.MapCameraPointToDepthSpace(jointPosition);

                                                point.X = float.IsInfinity(depthPoint.X) ? 0 : depthPoint.X;
                                                point.Y = float.IsInfinity(depthPoint.Y) ? 0 : depthPoint.Y;
                                            }

                                            // Draw
                                            Ellipse ellipse = new Ellipse
                                            {
                                                Fill = Brushes.Red,
                                                Width = 30,
                                                Height = 30
                                            };

                                            Canvas.SetLeft(ellipse, point.X - ellipse.Width / 2);
                                            Canvas.SetTop(ellipse, point.Y - ellipse.Height / 2);

                                            canvas.Children.Add(ellipse);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        //private void Color_Click(object sender, RoutedEventArgs e)
        //{
        //    _mode = Mode.Color;
        //}

        //private void Depth_Click(object sender, RoutedEventArgs e)
        //{
        //    _mode = Mode.Depth;
        //}

        //private void Infrared_Click(object sender, RoutedEventArgs e)
        //{
        //    _mode = Mode.Infrared;
        //}

        private void Tracking_Click(object sender, RoutedEventArgs e)
        {
            _displayBody = !_displayBody;
        }

        #endregion
    }
}
