using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using RadialMenu.CustomControls;

namespace RadialMenuDemo
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RadialMenuItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
                return;
            var isActive = (sender as RadialMenuItem).IsActive;
            (sender as RadialMenuItem)?.RadialMenu.Items.ForEach(item => item.IsActive = false);
            (sender as RadialMenuItem).IsActive = !isActive;

            if (!isActive)
                (sender as RadialMenuItem).RadialMenu.CurrentItem = sender as RadialMenuItem;
            else
                (sender as RadialMenuItem).RadialMenu.CurrentItem = null;
        }
    }
}