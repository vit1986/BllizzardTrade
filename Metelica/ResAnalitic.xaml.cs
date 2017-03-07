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
    /// Interaction logic for ResAnalitic.xaml
    /// </summary>
    public partial class ResAnalitic : Window
    {
        public ResAnalitic()
        {
            InitializeComponent();
            try
            {
                MContext mc = new MContext();
                List<MyTableGrid> data = mc.SKUs.Where(x => x.ChangePrice == true && x.Enable == true).Select(x => new MyTableGrid
                {
                    ID = x.ID,
                    Подкатегория = x.Subcategory.Name,
                    Бренд = x.Brend.Name,
                    Модель = x.Name,
                    Вход = x.InputPrice,
                    Розница = x.Price,
                    Цена_конкурента = x.TotalPrice,
                    Промо = x.TotalPromo,
                    Новая_наценка = Math.Round(((x.TotalPrice / x.InputPrice - 1) * 100), 2),
                    Изменение = Math.Round(((x.TotalPrice / (double)x.Price - 1) * 100)),
                    Комментарий = x.Who
                }).ToList();
                MyGrid.ItemsSource = data;
            }
            catch(System.Data.Entity.Core.EntityCommandExecutionException)
            {
                MessageBox.Show("Нулевая цена");
            }
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            int count = 0;
            foreach (MyTableGrid s in MyGrid.SelectedItems)
            {
                    MContext mc = new MContext();
                    List<SKU> tmp = mc.SKUs.Where(x => x.ID == s.ID).ToList();
                    PrintPrice pp = new PrintPrice(tmp[0]);
                    pp.ShowDialog();
                    count++;
                    if ((count+1) == MyGrid.SelectedItems.Count)
                        break;
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            MyGrid.SelectAll();
        }

        private void MyGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MyTableGrid s = MyGrid.SelectedItem as MyTableGrid;
            MContext mc = new MContext();
            InfoSKU isku = new InfoSKU(mc.SKUs.First(x => x.ID == s.ID));
            isku.ShowDialog();
        }
    }
}
