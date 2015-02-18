using System.Collections;
using System.Windows.Controls;

namespace Utilities
{
    public partial class DropdownControl
    {
        private string _selection;

        public string Selection
        {
            get { return _selection; }
            set { _selection = value; }
        }

        public DropdownControl()
        {
            InitializeComponent();
        }

        public void AddItems(ICollection collection)
        {
            Box.ItemsSource = collection;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selection = Box.ItemStringFormat;
        }
    }
}

