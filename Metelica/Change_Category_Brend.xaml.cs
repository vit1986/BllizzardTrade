using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Metelica
{
    /// <summary>
    /// Interaction logic for Change_Category_Brend.xaml
    /// </summary>
    public partial class Change_Category_Brend : Window
    {
        public Change_Category_Brend(string str)
        {
            InitializeComponent();
            this.Title = "Изменить " + str;
        }

        private void CBCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CBCategory.SelectedIndex != -1)
                AddItems.IsEnabled = true;
            else
                AddItems.IsEnabled = false;
        }

        private void AddItems_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (AddItems.Text.Length != 0)
                ADD.IsEnabled = true;
            else
                ADD.IsEnabled = false;
        }

    }
}
