using System.Windows;

namespace bézier_curve.Views {

    public partial class MainWindow : Window {

        public MainWindow() {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            nupDegree.Value = 3;
            showConnectors.IsChecked = true;
            area.ShowConnectors = true;
        }

        private void nupDegree_ValueChanged(object sender, int e) {
            if (area != null)
                area.Degree = e;
        }

        private void showConnectors_Click(object sender, RoutedEventArgs e) {
            area.ShowConnectors = showConnectors.IsChecked.Value;
        }
    }
}
