using System.Windows.Controls;
using System.Windows.Media;

namespace Utilities
{
    /// <summary>
    /// Interaction logic for HellowDynamoControl.xaml
    /// </summary>
    public partial class DropdownControl
    {
        public DropdownControl()
        {
            InitializeComponent();
            cmbColors.ItemsSource = typeof(Colors).GetProperties();
        }
    }
}
