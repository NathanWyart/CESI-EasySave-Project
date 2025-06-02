using System.Windows;
using System.Windows.Controls;

namespace EasySave.View
{
    public partial class AddWorkWindow : Window
    {
        public string WorkName => NameBox.Text;
        public string WorkSrc => SrcBox.Text;
        public string WorkDst => DstBox.Text;
        public string WorkType => ((ComboBoxItem)TypeBox.SelectedItem).Content.ToString();

        public AddWorkWindow()
        {
            InitializeComponent();
        }

        private void OnValidate(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}