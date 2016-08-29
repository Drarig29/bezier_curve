using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace bézier_curve.Controls {

    public class BézierArea : Grid {

        Canvas cvCurve = new Canvas() { Background = new SolidColorBrush(Colors.Transparent) };
        Canvas cvHandles = new Canvas() { Background = new SolidColorBrush(Colors.Transparent) };
        Canvas cvConnectors = new Canvas() { Background = new SolidColorBrush(Colors.Transparent) };

        HandleConnector hc1;
        HandleConnector hc2;

        private int _degree = 1;
        private bool _showConnectors;

        public BézierArea() {
            Background = Brushes.White;

            hc1 = new HandleConnector();
            hc2 = new HandleConnector();

            this.Children.Add(cvCurve);
            this.Children.Add(cvConnectors);
            this.Children.Add(cvHandles);

            this.SizeChanged += BézierArea_SizeChanged;
        }

        private void BézierArea_SizeChanged(object sender, SizeChangedEventArgs e) {
            if (!DesignerProperties.GetIsInDesignMode(this) && IsLoaded) {
                foreach (Handle h in cvHandles.Children) {
                    h.Left = (h.Left / e.PreviousSize.Width) * e.NewSize.Width;
                    h.Top = (h.Top / e.PreviousSize.Height) * e.NewSize.Height;
                    h.OnMoved();
                }

                DrawCurve();
            }
        }

        private void AddHandle(ref Handle h) {
            cvHandles.Children.Add(h);
            h.Moved += () => DrawCurve();
        }

        public void SetHandles(int count) {
            var margin = 80;
            var width = this.ActualWidth - (2 * margin);
            var height = this.ActualHeight - (2 * margin);

            cvHandles.Children.Clear();
            cvConnectors.Children.Clear();

            for (int i = 0; i <= count; i++) {
                var h = new Handle() {
                    Center = new Point(margin + (((double) i / count) * width), margin + ((i % 2) * height))
                };

                AddConnector(i, count, h);
                AddHandle(ref h);
            }

            AddConnectors(count);
            DrawCurve();
        }

        private void AddConnector(int i, int count, Handle h) {
            if (ShowConnectors && count > 1) {
                if (i == 0)
                    hc1.Handle1 = h;

                if (i == 1)
                    hc1.Handle2 = h;

                if (i == count - 1)
                    hc2.Handle1 = h;

                if (i == count)
                    hc2.Handle2 = h;
            }
        }

        private void AddConnectors(int count) {
            if (ShowConnectors && count > 1) {
                hc1.Initialize();
                hc2.Initialize();

                cvConnectors.Children.Add(hc1);
                cvConnectors.Children.Add(hc2);
            }
        }

        private void SetConnectors() {
            for (int i = 0; i <= _degree; i++)
                AddConnector(i, _degree, (Handle) cvHandles.Children[i]);

            AddConnectors(_degree);
        }

        public bool ShowConnectors {
            get { return _showConnectors; }
            set {
                _showConnectors = value;

                if (value)
                    SetConnectors();
                else
                    cvConnectors.Children.Clear();
            }
        }

        public int Degree {
            get { return _degree; }
            set {
                _degree = value;
                SetHandles(value);
            }
        }

        /// <summary>
        /// Clears the old curve, and draws the new one, point by point.
        /// </summary>
        public void DrawCurve() {
            cvCurve.Children.Clear();

            for (double i = 0; i < 1; i += 0.01)
                DrawPoint(B(i, cvHandles.Children));
        }

        /// <summary>
        /// Draw a red <see cref="Point"/> on the <see cref="cvCurve"/> Canvas.
        /// </summary>
        public void DrawPoint(Point p) {
            var rect = new Ellipse() {
                Fill = new SolidColorBrush(Colors.Red),
                Width = 3,
                Height = 3
            };

            Canvas.SetLeft(rect, p.X);
            Canvas.SetTop(rect, p.Y);

            cvCurve.Children.Add(rect);
        }

        #region Maths
        /// <summary>
        /// Gets the <see cref="Point"/> at a specific time, given the handles.
        /// </summary>
        public Point B(double t, UIElementCollection h) {
            return new Point(Bx(t, h), By(t, h));
        }

        public double Bx(double t, UIElementCollection h) {
            ulong n = (ulong) h.Count - 1; //i.e. with 4 elements in p (4 handles), the degree is 3 : it's a cubic-bézier curve !
            var x = 0.0;

            for (ulong i = 0; i <= n; i++)
                x += C(i, n) * Math.Pow(1 - t, n - i) * Math.Pow(t, i) * ((Handle) h[(int) i]).Center.X;

            return x;
        }

        public double By(double t, UIElementCollection p) {
            ulong n = (ulong) p.Count - 1; //i.e. with 4 elements in p (4 handles), the degree is 3 : it's a cubic-bézier curve !
            var y = 0.0;

            for (ulong i = 0; i <= n; i++)
                y += C(i, n) * Math.Pow(1 - t, n - i) * Math.Pow(t, i) * ((Handle) p[(int) i]).Center.Y;

            return y;
        }

        public ulong C(ulong k, ulong n) {
            return fact(n) / (fact(k) * fact(n - k));
        }

        public ulong fact(ulong n) {
            ulong x = 1;

            for (ulong i = 2; i < n + 1; i++)
                x *= i;

            return x;
        }
        #endregion
    }
}
