using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace bézier_curve.Controls {

    public class HandleConnector : Shape {

        public Handle Handle1 { get; set; } = null;
        public Handle Handle2 { get; set; } = null;

        public HandleConnector() {
            Stroke = Brushes.Black;
            StrokeThickness = 1.5;
        }

        public void Initialize() {
            Panel.SetZIndex(this, Math.Min(Panel.GetZIndex(Handle1), Panel.GetZIndex(Handle2)) - 1);

            Handle1.Moved += () => this.InvalidateVisual();
            Handle2.Moved += () => this.InvalidateVisual();
        }

        protected override Geometry DefiningGeometry {
            get {
                if (Handle1 == null || Handle2 == null)
                    return null;

                return new LineGeometry(Handle1.Center, Handle2.Center);
            }
        }
    }
}
