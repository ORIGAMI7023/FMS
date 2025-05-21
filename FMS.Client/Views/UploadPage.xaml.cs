using System.Windows;
using FMS.Client.ViewModels;

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