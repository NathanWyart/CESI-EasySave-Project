using System.Windows.Controls;
using EasySave_WPF.ViewModels;

namespace EasySave_WPF.View
{
    public partial class AddWorkControl : UserControl
    {
        public AddWorkViewModel ViewModel { get; } = new();

        public AddWorkControl()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }


        public void Initialize(Action<AddWorkViewModel> onConfirm, Action onCancel)
        {
            ViewModel.Confirmed += (_, _) => onConfirm?.Invoke(ViewModel);
            ViewModel.Canceled += (_, _) => onCancel?.Invoke();
        }
    }
}
