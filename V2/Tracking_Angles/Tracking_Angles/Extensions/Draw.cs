﻿using Microsoft.Kinect;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Tracking_Angles
{
    public static class Draw
    {
        #region Drawing

        public static void DrawSkeleton(this Canvas canvas, Body body)
        {
            if (body == null) return;

            foreach (Joint joint in body.Joints.Values)
            {
                canvas.DrawPoint(joint);
            }

            canvas.DrawLine(body.Joints[JointType.Head], body.Joints[JointType.Neck]);
            canvas.DrawLine(body.Joints[JointType.Neck], body.Joints[JointType.SpineShoulder]);
            canvas.DrawLine(body.Joints[JointType.SpineShoulder], body.Joints[JointType.ShoulderLeft]);
            canvas.DrawLine(body.Joints[JointType.SpineShoulder], body.Joints[JointType.ShoulderRight]);
            canvas.DrawLine(body.Joints[JointType.SpineShoulder], body.Joints[JointType.SpineMid]);
            canvas.DrawLine(body.Joints[JointType.ShoulderLeft], body.Joints[JointType.ElbowLeft]);
            canvas.DrawLine(body.Joints[JointType.ShoulderRight], body.Joints[JointType.ElbowRight]);
            canvas.DrawLine(body.Joints[JointType.ElbowLeft], body.Joints[JointType.WristLeft]);
            canvas.DrawLine(body.Joints[JointType.ElbowRight], body.Joints[JointType.WristRight]);
            canvas.DrawLine(body.Joints[JointType.WristLeft], body.Joints[JointType.HandLeft]);
            canvas.DrawLine(body.Joints[JointType.WristRight], body.Joints[JointType.HandRight]);
            canvas.DrawLine(body.Joints[JointType.HandLeft], body.Joints[JointType.HandTipLeft]);
            canvas.DrawLine(body.Joints[JointType.HandRight], body.Joints[JointType.HandTipRight]);
            canvas.DrawLine(body.Joints[JointType.HandTipLeft], body.Joints[JointType.ThumbLeft]);
            canvas.DrawLine(body.Joints[JointType.HandTipRight], body.Joints[JointType.ThumbRight]);
            canvas.DrawLine(body.Joints[JointType.SpineMid], body.Joints[JointType.SpineBase]);
            canvas.DrawLine(body.Joints[JointType.SpineBase], body.Joints[JointType.HipLeft]);
            canvas.DrawLine(body.Joints[JointType.SpineBase], body.Joints[JointType.HipRight]);
            canvas.DrawLine(body.Joints[JointType.HipLeft], body.Joints[JointType.KneeLeft]);
            canvas.DrawLine(body.Joints[JointType.HipRight], body.Joints[JointType.KneeRight]);
            canvas.DrawLine(body.Joints[JointType.KneeLeft], body.Joints[JointType.AnkleLeft]);
            canvas.DrawLine(body.Joints[JointType.KneeRight], body.Joints[JointType.AnkleRight]);
            canvas.DrawLine(body.Joints[JointType.AnkleLeft], body.Joints[JointType.FootLeft]);
            canvas.DrawLine(body.Joints[JointType.AnkleRight], body.Joints[JointType.FootRight]);
        }

        public static void DrawPoint(this Canvas canvas, Joint joint)
        {
            // Check if the joint is tracked
            if (joint.TrackingState == TrackingState.NotTracked) return;
            
            // Map the real-world coordinates to screen pixels
            joint = joint.ScaleTo(canvas.ActualWidth, canvas.ActualHeight);

            Ellipse ellipse = new Ellipse
            {
                Width = 20,
                Height = 20,
                Fill = new SolidColorBrush(Colors.LightBlue)
            };

            Canvas.SetLeft(ellipse, joint.Position.X - ellipse.Width / 2);
            Canvas.SetTop(ellipse, joint.Position.Y - ellipse.Height / 2);

            canvas.Children.Add(ellipse);
        }

        public static void DrawLine(this Canvas canvas, Joint first, Joint second)
        {
            if (first.TrackingState == TrackingState.NotTracked || second.TrackingState == TrackingState.NotTracked) return;

            first = first.ScaleTo(canvas.ActualWidth, canvas.ActualHeight);
            second = second.ScaleTo(canvas.ActualWidth, canvas.ActualHeight);

            Line line = new Line
            {
                X1 = first.Position.X,
                Y1 = first.Position.Y,
                X2 = second.Position.X,
                Y2 = second.Position.Y,
                StrokeThickness = 8,
                Stroke = new SolidColorBrush(Colors.LightBlue)
            };

            canvas.Children.Add(line);
        }

        #endregion
    }
}
