using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace bézier_curve {

    public partial class MainWindow : Window {

        Point p0, p1, p2, p3;

        Canvas curve = new Canvas() { Background = new SolidColorBrush(Colors.Transparent) };

        public MainWindow() {
            InitializeComponent();

            p0 = h0.GetCenter();
            p1 = h1.GetCenter();
            p2 = h2.GetCenter();
            p3 = h3.GetCenter();

            SetLines();

            area.Children.Add(curve);

            DrawCurve();
        }

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

        public void DrawCurve() {
            curve.Children.Clear();

            for (double i = 0; i < 1; i += 0.01)
                DrawPoint(B(i, p0, p1, p2, p3));
        }

        public Point B(double t, params Point[] p) {
            return new Point(Bx(t, p), By(t, p));
        }

        public double Bx(double t, params Point[] p) {
            return (Math.Pow(1 - t, 3) * p[0].X) +
                (3 * Math.Pow(1 - t, 2) * t * p[1].X) +
                (3 * (1 - t) * Math.Pow(t, 2) * p[2].X) +
                (Math.Pow(t, 3) * p[3].X);
        }

        public double By(double t, params Point[] p) {
            return (Math.Pow(1 - t, 3) * p[0].Y) +
                (3 * Math.Pow(1 - t, 2) * t * p[1].Y) +
                (3 * (1 - t) * Math.Pow(t, 2) * p[2].Y) +
                (Math.Pow(t, 3) * p[3].Y);
        }

        public void DrawPoint(Point p) {
            DrawPoint(p.X, p.Y, Color.FromRgb(255, 0, 0));
        }

        public void DrawPoint(double x, double y, Color c) {
            var rect = new Ellipse() {
                Fill = new SolidColorBrush(c),
                Width = 2,
                Height = 2
            };

            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);

            curve.Children.Add(rect);
        }

        bool h0_down = false;
        bool h1_down = false;
        bool h2_down = false;
        bool h3_down = false;

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
                if (Mouse.LeftButton != MouseButtonState.Pressed) ResetHandles();
                SetLines();
                DrawCurve();
            }
        }
        
        private void mainWindow_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            ResetHandles();
        }

        private void mainWindow_MouseLeave(object sender, MouseEventArgs e) {
            ResetHandles();
        }

        private void ResetHandles() {
            h0_down = false;
            h1_down = false;
            h2_down = false;
            h3_down = false;

            h0.Fill = Brushes.Red;
            h1.Fill = Brushes.Red;
            h2.Fill = Brushes.Red;
            h3.Fill = Brushes.Red;
        }
    }

    public static class MyExtensions {

        /// <summary>
        /// Récupère le point correspondant au centre de l'<see cref="Ellipse"/>.
        /// </summary>
        public static Point GetCenter(this Ellipse e) {
            return new Point(Canvas.GetLeft(e) + e.Width / 2, Canvas.GetTop(e) + e.Height / 2);
        }

        /// <summary>
        /// Définit l'emplacement de l'<see cref="Ellipse"/> en fonction du centre donné.
        /// </summary>
        public static void SetCenter(this Ellipse e, Point p) {
            Canvas.SetLeft(e, p.X - e.Width / 2);
            Canvas.SetTop(e, p.Y - e.Height / 2);
        }
    }
}
