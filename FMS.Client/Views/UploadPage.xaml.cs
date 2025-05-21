using System.Windows;

namespace FMS.Client.Views
{
    public partial class UploadPage : Window
    {
        public UploadPage()
        {
            InitializeComponent();
            DataContext = new UploadViewModel();
        }
    }
}
