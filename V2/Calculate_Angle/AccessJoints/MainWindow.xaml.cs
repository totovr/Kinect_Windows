using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Kinect;
using System.Windows.Media.Media3D;

namespace AccessJoints
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        // Kinect sensor and Kinect stream reader objects
        // Kinect sensor
        KinectSensor _sensor;
        // Method used to read incoming frames
        MultiSourceFrameReader _reader;
        // List to save the body in scene
        IList<Body> _bodies;

        public MainWindow()
        {
            _sensor = KinectSensor.GetDefault();

            if (_sensor != null)
            {
                _sensor.Open();
            }

            _reader = _sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color |
                                                         FrameSourceTypes.Depth |
                                                         FrameSourceTypes.Infrared |
                                                         FrameSourceTypes.Body);
            _reader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;

            InitializeComponent();
        }

        void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            // This is the label of the angle 
            string AnguloUni = "No hay angulo";

            // Here we save the frames comming 
            var reference = e.FrameReference.AcquireFrame();

            // Body
            using (var frame = reference.BodyFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    _bodies = new Body[frame.BodyFrameSource.BodyCount];

                    frame.GetAndRefreshBodyData(_bodies);

                    foreach (var body in _bodies)
                    {
                        if (body != null)
                        {
                            Joint handJoint = body.Joints[JointType.HandRight];
                            Joint wristJoint = body.Joints[JointType.WristRight];
                            Joint elbowJoint = body.Joints[JointType.ElbowRight];
                            Joint shoulderJoint = body.Joints[JointType.ShoulderRight];            

                            Vector3D ElbowLeft = new Vector3D(body.Joints[JointType.ElbowLeft].Position.X, body.Joints[JointType.ElbowLeft].Position.Y, body.Joints[JointType.ElbowLeft].Position.Z);
                            Vector3D WristLeft = new Vector3D(body.Joints[JointType.WristLeft].Position.X, body.Joints[JointType.WristLeft].Position.Y, body.Joints[JointType.WristLeft].Position.Z);
                            Vector3D ShoulderLeft = new Vector3D(body.Joints[JointType.ShoulderLeft].Position.X, body.Joints[JointType.ShoulderLeft].Position.Y, body.Joints[JointType.ShoulderLeft].Position.Z);

                            Vector3D Head = new Vector3D(body.Joints[JointType.Head].Position.X, body.Joints[JointType.Head].Position.Y, body.Joints[JointType.Head].Position.Z);
                            Vector3D Neck = new Vector3D(body.Joints[JointType.Neck].Position.X, body.Joints[JointType.Neck].Position.Y, body.Joints[JointType.Neck].Position.Z);
                            Vector3D SpineShoulder = new Vector3D(body.Joints[JointType.SpineShoulder].Position.X, body.Joints[JointType.SpineShoulder].Position.Y, body.Joints[JointType.SpineShoulder].Position.Z);

                            double LeftElbowAngle = AngleBetweenTwoVectors(ElbowLeft - ShoulderLeft, ElbowLeft - WristLeft);
                            double NeckAngle = AngleBetweenTwoVectors(Neck - Head, Neck - SpineShoulder);

                            AnguloUni = Convert.ToString(LeftElbowAngle);
                        }
                    }
                    // Send the angle to the box
                    VectorAngle.Text = AnguloUni;
                }
            }
        }

        public double AngleBetweenTwoVectors(Vector3D vectorA, Vector3D vectorB)
        {
            double dotProduct = 0.0;
            vectorA.Normalize();
            vectorB.Normalize();
            dotProduct = Vector3D.DotProduct(vectorA, vectorB);

            return (double)Math.Acos(dotProduct) / Math.PI * 180;
        }
    }
}
