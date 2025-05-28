using EasySave_WPF.ViewModels;
using System.Windows.Controls;

namespace EasySave_WPF.View
{
    public partial class WorkListPage : UserControl
    {
        public WorkListPage()
        {
            InitializeComponent();
            this.DataContext = new WorkListViewModel(); // important
        }
    }
}
