using System.Collections;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace Utilities
{
    public partial class DropdownControl
    {
        private Dropdown _dropdown;

        public DropdownControl(Dropdown dropdown)
        {
            _dropdown = dropdown;
            InitializeComponent();
        }

        public void AddItems(ICollection collection)
        {
            Box.ItemsSource = collection;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _dropdown.Index = Box.SelectedIndex;
            _dropdown.Item = Box.SelectedItem;
            //_selection = Box.ItemStringFormat;
        }
    }
}

