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
using BDM;
using BDM.Models;

namespace Metelica
{
    /// <summary>
    /// Interaction logic for EnterID.xaml
    /// </summary>
    public partial class EnterID : Window
    {
        public EnterID()
        {
            InitializeComponent();
        }

        public EnterID(int ID)
        {
            InitializeComponent();
            FSerch(ID);
        }

        private void IDNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IDNumber.Text.Length != 0)
                Serch.IsEnabled = true;
            else
                Serch.IsEnabled = false;
        }

        private void Serch_Click(object sender, RoutedEventArgs e)
        {
            FSerch();
        }

        void FSerch()
        {
            try
            {
                int id = int.Parse(IDNumber.Text);
                MContext mc = new MContext();
                if (mc.SKUs.Any(x => x.ID == id))
                {
                    SKU s = mc.SKUs.First(x => x.ID == id);
                    InfoSKU asku = new InfoSKU(s);
                    asku.ShowDialog();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Товар с таким ID не найден");
                }
            }
            catch
            {
                MessageBox.Show("Введен не корректный ID");
                IDNumber.Clear();
            }
        }
        void FSerch(int id)
        {
            try
            {
                MContext mc = new MContext();
                if (mc.SKUs.Any(x => x.ID == id))
                {
                    SKU s = mc.SKUs.First(x => x.ID == id);
                    InfoSKU asku = new InfoSKU(s);
                    asku.ShowDialog();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Товар с таким ID не найден");
                }
            }
            catch
            {
                MessageBox.Show("Введен не корректный ID");
                IDNumber.Clear();
            }
        }
    }
}
