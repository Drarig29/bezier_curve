using bézier_curve.Views;
using System;
using System.Windows;

namespace bézier_curve {

    public partial class App : Application {

        [STAThread]
        public static void Main() {
            new Application().Run(new MainWindow());
        }
    }
}
