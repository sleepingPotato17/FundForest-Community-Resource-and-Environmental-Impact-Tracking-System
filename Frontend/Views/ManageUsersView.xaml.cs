using System.Windows.Controls;
using FundForest.ViewModels;

namespace FundForest.Views
{
    public partial class ManageUsersView : UserControl
    {
        public ManageUsersView()
        {
            InitializeComponent();
            DataContext = new ManageUsersViewModel();
        }
    }
}