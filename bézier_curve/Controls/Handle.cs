using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace bézier_curve.Controls {

    public class Handle : Shape {

        Canvas parent = null;
        private bool _mouse_down = false;
        private Point offset;

        public Handle() {
            Width = 8;
            Height = 8;
            Fill = Brushes.Red;

            Loaded += Handle_Loaded;
            PreviewMouseLeftButtonDown += Handle_PreviewMouseLeftButtonDown;
        }

        private void Handle_Loaded(object sender, RoutedEventArgs e) {
            if (Parent is Canvas) {
                parent = (Canvas) Parent;

                parent.PreviewMouseMove += Win_PreviewMouseMove;
                parent.PreviewMouseLeftButtonUp += Win_PreviewMouseLeftButtonUp;
                parent.MouseLeave += Win_MouseLeave;
            }
        }

        #region Events
        public delegate void MovedEventHandler();
        public event MovedEventHandler Moved;
        public void OnMoved() {
            Moved?.Invoke();
        }
        #endregion

        private void Win_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            mouse_down = false;
        }

        private bool mouse_down {
            get { return _mouse_down; }
            set {
                Fill = value ? Brushes.Black : Brushes.Red;
                _mouse_down = value;
            }
        }

        private void Handle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            mouse_down = true;
            offset = e.GetPosition(this);
        }

        private void Win_PreviewMouseMove(object sender, MouseEventArgs e) {
            if (mouse_down) {
                var p = e.GetPosition(parent);
                Left = p.X - offset.X;
                Top = p.Y - offset.Y;

                OnMoved();
            }
        }

        private void Win_MouseLeave(object sender, MouseEventArgs e) {
            mouse_down = false;
        }

        public Point Center {
            get { return new Point(Left + Width / 2, Top + Height / 2); }
            set {
                Left = value.X - Width / 2;
                Top = value.Y - Height / 2;
                OnMoved();
            }
        }

        public double Left {
            get { return Canvas.GetLeft(this); }
            set { Canvas.SetLeft(this, value); }
        }

        public double Top {
            get { return Canvas.GetTop(this); }
            set { Canvas.SetTop(this, value); }
        }

        protected override Geometry DefiningGeometry {
            get {
                return new EllipseGeometry(new Rect(0, 0, Width, Height));
            }
        }
    }
}
