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
using System.Windows.Navigation;
using System.Windows.Shapes;
using BDM.Models;
using BDM;
using Microsoft.Win32;
using System.IO;

namespace Metelica
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MContext mc=new MContext();
            List<Category> lc = mc.Categories.ToList();
            CBCategory.ItemsSource = lc;
            BitmapImage bi = new BitmapImage(new Uri("D:\\logo.ico"));
            this.Icon = bi;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Add_Brend_Category ABC = new Add_Brend_Category(MI11.Header.ToString());
            this.Visibility = Visibility.Hidden;
            ABC.ShowDialog();
            this.Visibility = Visibility.Visible;
        }

        private void CBCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Category c = CBCategory.SelectedItem as Category;
            MContext mc = new MContext();
            List<Subcategory> ls = mc.Subcategories.Where(x => x.ID_Category == c.ID).ToList();
            CBSubcategory.ItemsSource = ls.OrderBy(x=>x.Name);
        }

        private void MI12_Click(object sender, RoutedEventArgs e)
        {
            ADD_Subcategory asc = new ADD_Subcategory(MI12.Header.ToString());
            this.Visibility = Visibility.Hidden;
            asc.ShowDialog();
            this.Visibility = Visibility.Visible;
        }

        private void MI13_Click(object sender, RoutedEventArgs e)
        {
            Add_Brend_Category ABC = new Add_Brend_Category(MI13.Header.ToString());
            this.Visibility = Visibility.Hidden;
            ABC.ShowDialog();
            this.Visibility = Visibility.Visible;
        }

        private void CBSubcategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MContext mc=new MContext();
            List<Brend> lb = mc.Brends.OrderBy(x=>x.Name).ToList();
            CBBrend.ItemsSource = lb;
        }

        private void MI14_Click(object sender, RoutedEventArgs e)
        {
            AddSKU asku = new AddSKU();
            this.Visibility = Visibility.Hidden;
            asku.ShowDialog();
            this.Visibility = Visibility.Visible;
        }

        private void ShowSKUs_Click(object sender, RoutedEventArgs e)
        {
            MContext mc = new MContext();
            List<MyGridMainMenu> lsku = null;
            if(CBCategory.SelectedIndex==-1)
            {
                lsku = mc.SKUs.Select(x => new MyGridMainMenu
                {
                    ID = x.ID,
                    Категория = x.Subcategory.Category.Name,
                    Подкатегория = x.Subcategory.Name,
                    Бренд = x.Brend.Name,
                    Модель = x.Name,
                    Вход = x.InputPrice,
                    Розница = x.Price,
                    Наценка = Math.Round(((x.Price / x.InputPrice) - 1) * 100, 2),
                    Наличие = x.Enable
                }).ToList();
            }
            else if(CBSubcategory.SelectedIndex==-1)
            {
                Category c = CBCategory.SelectedItem as Category;
                lsku = mc.SKUs.Where(x => x.Subcategory.ID_Category == c.ID).Select(x => new MyGridMainMenu
                {
                    ID = x.ID,
                    Категория = x.Subcategory.Category.Name,
                    Подкатегория = x.Subcategory.Name,
                    Бренд = x.Brend.Name,
                    Модель = x.Name,
                    Вход = x.InputPrice,
                    Розница = x.Price,
                    Наценка = Math.Round(((x.Price / x.InputPrice) - 1) * 100, 2),
                    Наличие = x.Enable
                }).ToList();

            }
            else if (CBBrend.SelectedIndex==-1)
            {
                Subcategory s = CBSubcategory.SelectedItem as Subcategory;
                lsku = mc.SKUs.Where(x => x.ID_Subcategory == s.ID).Select(x => new MyGridMainMenu
                {
                    ID = x.ID,
                    Категория = x.Subcategory.Category.Name,
                    Подкатегория = x.Subcategory.Name,
                    Бренд = x.Brend.Name,
                    Модель = x.Name,
                    Вход = x.InputPrice,
                    Розница = x.Price,
                    Наценка = Math.Round(((x.Price / x.InputPrice) - 1) * 100, 2),
                    Наличие = x.Enable
                }).ToList();
            }
            else
            {
                Subcategory s = CBSubcategory.SelectedItem as Subcategory;
                Brend b = CBBrend.SelectedItem as Brend;

                lsku = mc.SKUs.Where(x => x.ID_Subcategory == s.ID && x.ID_Brend == b.ID).Select(x => new MyGridMainMenu
                {
                    ID = x.ID,
                    Категория = x.Subcategory.Category.Name,
                    Подкатегория = x.Subcategory.Name,
                    Бренд = x.Brend.Name,
                    Модель = x.Name,
                    Вход = x.InputPrice,
                    Розница = x.Price,
                    Наценка = Math.Round(((x.Price / x.InputPrice) - 1) * 100, 2),
                    Наличие = x.Enable
                }).ToList();
            }
            
            
            //предусмотреть варианты не выбранных крмбобоксов
            
           

            if (lsku != null)
                DGS.ItemsSource = lsku;
        }

        private void MI15_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "CSV files (*.csv)|*.csv";
            if(ofd.ShowDialog()==System.Windows.Forms.DialogResult.OK)
            {
                List<SKU> ls = new List<SKU>();
                List<SkuAnot> lskuan=new List<SkuAnot>();
                string content;
                using (StreamReader sr=new StreamReader(ofd.FileName))
                {
                    while((content=sr.ReadLine())!=null)
                    {
                        string[] mas = content.Split(';');
                        SKU s= new SKU();
                        SkuAnot sa=new SkuAnot();
                        
                       for(int i=0; i<mas.Length;i++)
                       {
                           switch(i)
                           {
                               case 0: s.Name = mas[i]; break;
                               case 1: s.InputPrice = double.Parse(mas[i]); break;
                               case 2: if(mas[i]!=string.Empty)
                                   s.Price = int.Parse(mas[i]);
                                   else
                                   s.Price=0;
                                   break;
                               case 3: s.idComfy = mas[i]; break;
                               case 4: s.idRozetka = mas[i]; break;
                               case 5: s.idAllo = mas[i]; break;
                               case 6: s.idEldorado = mas[i]; break;
                               case 7: s.UrlHotline = mas[i]; break;
                               case 8: sa.Val1 = mas[i]; break;
                               case 9: sa.Val2 = mas[i]; break;
                               case 10: sa.Val3 = mas[i]; break;
                               case 11: sa.Val4 = mas[i]; break;
                               case 12: sa.Val5 = mas[i]; break;
                               case 13: sa.Val6 = mas[i]; break;
                               case 14: sa.Val7 = mas[i]; break;
                               case 15: sa.Val8 = mas[i]; break;
                               case 16: sa.Val9 = mas[i]; break;
                               case 17: sa.Val10 = mas[i]; break;
                               case 18: sa.Val11 = mas[i]; break;
                               case 19: sa.Val12 = mas[i]; break;
                           }
                       }
                       ls.Add(s);
                       lskuan.Add(sa);
                    }
                }
                int j=0;
                foreach(SKU s in ls)
                {
                    AddSKU asku = new AddSKU(s,lskuan[j]);
                    this.Visibility = Visibility.Hidden;
                    asku.ShowDialog();
                    this.Visibility = Visibility.Visible;
                    j++;
                }
            }
        }

        private void MI21_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MI22_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MI23_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MI24_Click(object sender, RoutedEventArgs e)
        {
            EnterID eid = new EnterID();
            this.Visibility = Visibility.Hidden;
            eid.ShowDialog();
            this.Visibility = Visibility.Visible;
        }

        private void NewParsing_Click(object sender, RoutedEventArgs e)
        {
            NewParsing np = new NewParsing();
            this.Visibility = Visibility.Hidden;
            np.ShowDialog();
            this.Visibility = Visibility.Visible;
        }

        private void OldParsing_Click(object sender, RoutedEventArgs e)
        {
            AnaliticSku asku = new AnaliticSku();
            this.Visibility = Visibility.Hidden;
            asku.ShowDialog();
            this.Visibility = Visibility.Visible;
        }

        private void ResAnalitics_Click(object sender, RoutedEventArgs e)
        {
            ResAnalitic ra = new ResAnalitic();
            this.Visibility = Visibility.Hidden;
            ra.ShowDialog();
            this.Visibility = Visibility.Visible;
        }

        private void EKParcing_Click(object sender, RoutedEventArgs e)
        {
            MContext mc = new MContext();
            List<SKU> lsku = mc.SKUs.Where(x => x.TotalPrice == 0 && x.Enable == true && (x.UrlHotline != null && x.UrlHotline != string.Empty)).ToList();
            NewParsing np = new NewParsing(lsku);
            this.Visibility = Visibility.Hidden;
            np.ShowDialog();
            this.Visibility = Visibility.Visible;
        }

        private void DGS_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                MyGridMainMenu s = DGS.SelectedItem as MyGridMainMenu;
                MContext mc = new MContext();
                InfoSKU isku = new InfoSKU(mc.SKUs.First(x => x.ID == s.ID));
                isku.ShowDialog();
            }
            catch
            {
                SkuAnot s = DGS.SelectedItem as SkuAnot;
                 MContext mc = new MContext();
                 EnterID isku = new EnterID(mc.SAs.First(x => x.ID == s.ID).ID);
            }
        }

        private void PrintPrice_Click(object sender, RoutedEventArgs e)
        {
            MyGridMainMenu s = DGS.SelectedItem as MyGridMainMenu;
            MContext mc = new MContext();
            PrintPrice pp = new PrintPrice(mc.SKUs.First(x => x.ID == s.ID), true);
            pp.ShowDialog();
        }

        private void PrintDataGrid_Click(object sender, RoutedEventArgs e)
        {
            //PrintDialog printDialog = new PrintDialog();
            //if (printDialog.ShowDialog() == true)
            //{
            //    System.Windows.Size pageSize = new System.Windows.Size(370, 1000);
            //    DGS.Measure(new Size(500, 1000));
            //    printDialog.PrintVisual(DGS, "Распечатываем элемент DataGrid");

            //}
        }

        private void MI41_Click(object sender, RoutedEventArgs e)
        {
            MContext mc = new MContext();
            List<SkuAnot> lsku = mc.SAs.Where(x =>x.Val1==string.Empty&&x.SKU.Enable==true).ToList();
            DGS.ItemsSource = lsku;
        }


    }
}
