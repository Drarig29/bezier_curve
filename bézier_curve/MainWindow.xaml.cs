﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace bézier_curve {

    public partial class MainWindow : Window {

        //For more informations, see https://medium.freecodecamp.com/nerding-out-with-bezier-curves-6e3c0bc48e2f#.z2x0xyy6l

        Point p0, p1, p2, p3;

        /// <summary>
        /// A Canvas containing only the points of the curve. It makes it easier to clear just the curve and not the handles...
        /// </summary>
        Canvas curve = new Canvas() { Background = new SolidColorBrush(Colors.Transparent) };

        public MainWindow() {
            InitializeComponent();

            //get the center location of each handle
            p0 = h0.GetCenter();
            p1 = h1.GetCenter();
            p2 = h2.GetCenter();
            p3 = h3.GetCenter();

            //set the lines
            SetLines();

            //add the curve's container to the area
            area.Children.Add(curve);

            //draw the curve once on MainWindow initialization
            DrawCurve();
        }

        /// <summary>
        /// Sets the black lines between the handles 0/1 and 2/3.
        /// </summary>
        private void SetLines() {
            line01.X1 = p0.X;
            line01.Y1 = p0.Y;
            line01.X2 = p1.X;
            line01.Y2 = p1.Y;

            line23.X1 = p2.X;
            line23.Y1 = p2.Y;
            line23.X2 = p3.X;
            line23.Y2 = p3.Y;
        }

        /// <summary>
        /// Clears the old curve, and draws the new one, point by point.
        /// </summary>
        public void DrawCurve() {
            curve.Children.Clear();

            for (double i = 0; i < 1; i += 0.01)
                DrawPoint(B(i, p0, p1, p2, p3));
        }

        /// <summary>
        /// Gets the <see cref="Point"/> at a specific time, given the handles.
        /// </summary>
        public Point B(double t, params Point[] p) {
            return new Point(Bx(t, p), By(t, p));
        }

        /// <summary>
        /// Gets the x-coordinate of the point at a specific time, given the handles.
        /// </summary>
        public double Bx(double t, params Point[] p) {
            return (Math.Pow(1 - t, 3) * p[0].X) +
                (3 * Math.Pow(1 - t, 2) * t * p[1].X) +
                (3 * (1 - t) * Math.Pow(t, 2) * p[2].X) +
                (Math.Pow(t, 3) * p[3].X);
        }

        /// <summary>
        /// Gets the y-coordinate of the point at a specific time, given the handles.
        /// </summary>
        public double By(double t, params Point[] p) {
            return (Math.Pow(1 - t, 3) * p[0].Y) +
                (3 * Math.Pow(1 - t, 2) * t * p[1].Y) +
                (3 * (1 - t) * Math.Pow(t, 2) * p[2].Y) +
                (Math.Pow(t, 3) * p[3].Y);
        }

        /// <summary>
        /// Draw a red <see cref="Point"/> on the <see cref="curve"/> Canvas.
        /// </summary>
        public void DrawPoint(Point p) {
            var rect = new Ellipse() {
                Fill = new SolidColorBrush(Colors.Red),
                Width = 3,
                Height = 3
            };

            Canvas.SetLeft(rect, p.X);
            Canvas.SetTop(rect, p.Y);

            curve.Children.Add(rect);
        }

        #region Drag and Drop logics
        bool h0_down = false;
        bool h1_down = false;
        bool h2_down = false;
        bool h3_down = false;

        //when a handle is down, set its Fill property to Black
        private void handle0_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            h0_down = true;
            h0.Fill = Brushes.Black;
        }

        private void handle1_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            h1_down = true;
            h1.Fill = Brushes.Black;
        }

        private void handle2_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            h2_down = true;
            h2.Fill = Brushes.Black;
        }

        private void handle3_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            h3_down = true;
            h3.Fill = Brushes.Black;
        }

        //when the mouse moves and a handle is down, move this handle and update the lines and the curve
        private void mainWindow_PreviewMouseMove(object sender, MouseEventArgs e) {
            if (h0_down) {
                Point p = e.GetPosition(this);
                h0.SetCenter(p);
                p0 = h0.GetCenter();
            }

            if (h1_down) {
                Point p = e.GetPosition(this);
                h1.SetCenter(p);
                p1 = h1.GetCenter();
            }

            if (h2_down) {
                Point p = e.GetPosition(this);
                h2.SetCenter(p);
                p2 = h2.GetCenter();
            }

            if (h3_down) {
                Point p = e.GetPosition(this);
                h3.SetCenter(p);
                p3 = h3.GetCenter();
            }

            if (h0_down | h1_down | h2_down | h3_down) {
                //this statement resolves a bug with the handle 0 (h0), don't ask me why
                if (Mouse.LeftButton != MouseButtonState.Pressed) ResetHandles();

                SetLines();
                DrawCurve();
            }
        }

        //when the mouse is up, reset handles
        private void mainWindow_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            ResetHandles();
        }

        //when the mouse leaves the window, reset handles
        private void mainWindow_MouseLeave(object sender, MouseEventArgs e) {
            ResetHandles();
        }

        /// <summary>
        /// Set the state of the handles to their default state.
        /// </summary>
        private void ResetHandles() {
            if (h0_down) {
                h0.Fill = Brushes.Red;
                h0_down = false;
            }

            if (h1_down) {
                h1.Fill = Brushes.Red;
                h1_down = false;
            }

            if (h2_down) {
                h2.Fill = Brushes.Red;
                h2_down = false;
            }

            if (h3_down) {
                h3.Fill = Brushes.Red;
                h3_down = false;
            }
        }
        #endregion
    }

    public static class EllipseExtensions {

        /// <summary>
        /// Gets the point corresponding to the center of the <see cref="Ellipse"/>.
        /// </summary>
        public static Point GetCenter(this Ellipse e) {
            return new Point(Canvas.GetLeft(e) + e.Width / 2, Canvas.GetTop(e) + e.Height / 2);
        }

        /// <summary>
        /// Sets the location of the <see cref="Ellipse"/> according to the given center.
        /// </summary>
        public static void SetCenter(this Ellipse e, Point p) {
            Canvas.SetLeft(e, p.X - e.Width / 2);
            Canvas.SetTop(e, p.Y - e.Height / 2);
        }
    }
}
