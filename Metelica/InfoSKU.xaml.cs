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
using BDM.Models;
using BDM;

namespace Metelica
{
    /// <summary>
    /// Interaction logic for InfoSKU.xaml
    /// </summary>
    public partial class InfoSKU : Window
    {

        SKU sku;
        public InfoSKU(SKU s)
        {
            InitializeComponent();
            sku = s;
            Set(s);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MContext mc = new MContext();
            SKU s = mc.SKUs.First(x => x.ID == sku.ID);
            if(s.Enable)
            {
                s.Enable = false;
            }
            else
            {
                s.Enable = true;
            }
            mc.SaveChanges();
            sku = s;
            Set(s);
        }

        private void BPrint_Click(object sender, RoutedEventArgs e)
        {
            PrintPrice pp = new PrintPrice(sku, true);
            pp.ShowDialog();
        }

        private void BEdit_Click(object sender, RoutedEventArgs e)
        {
            AddSKU asku = new AddSKU(sku);
            asku.ShowDialog();
        }

        void Set(SKU s)
        {
            MContext mc = new MContext();
            this.Title = s.Name;
            LID.Content = string.Format("{0}", s.ID);
            LSub.Content = mc.Subcategories.First(x=>x.ID==s.ID_Subcategory).Name;
            LBre.Content = mc.Brends.First(x => x.ID == s.ID_Brend).Name;
            LMod.Content = s.Name;
            LPrice.Content = string.Format("{0}", s.Price);
            LInput.Content = string.Format("{0}", s.InputPrice);
            LMarga.Content = string.Format("{0} %", Math.Round(((s.Price/s.InputPrice)-1)*100,2));
            if(s.Enable)
            {
                BEnable.Content = "В наличии";
                BEnable.Background = Brushes.GreenYellow;
            }
            else
            {
                BEnable.Content = "Нет в наличи";
                BEnable.Background = Brushes.Red;
            }
        }
    }
}
