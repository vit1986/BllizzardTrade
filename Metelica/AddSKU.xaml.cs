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
    /// Interaction logic for AddSKU.xaml
    /// </summary>
    /// 
    // окно для добавления товара
    public partial class AddSKU : Window
    {
        SKU sk;
        public AnotSubCat s {get;set;}
        SkuAnot sa;
        public AddSKU()
        {
            //конструктор без параметров для добавления товара в ручную
            InitializeComponent();
            MContext mc = new MContext();
            //инициализируем комбобокс категориями
            List<Category> lc = mc.Categories.ToList();
            CBCategory.ItemsSource = lc;
            BitmapImage bi = new BitmapImage(new Uri("D:\\logo.ico"));
            this.Icon = bi;
        }

        public AddSKU(SKU s, SkuAnot sa)
        {
            //конструктор в параметрах приходит  товар и  аннотация
            InitializeComponent();
            MContext mc = new MContext();
            //инициализируем комобобокс категориями
            List<Category> lc = mc.Categories.ToList();
            CBCategory.ItemsSource = lc;
            BitmapImage bi = new BitmapImage(new Uri("D:\\logo.ico"));
            this.Icon = bi;
            //вызываем метод устонавливающий товары
            SetSKU(s,sa);
        }

        public AddSKU(SKU s)
        {
            //конструктор в параметрах приходит товар для изменения
            InitializeComponent();
            ADD.Visibility = Visibility.Hidden;
            Save.Visibility = Visibility.Visible;
            sk = s;
            MContext mc = new MContext();
            sa = mc.SAs.First(x => x.ID == sk.ID);
            BitmapImage bi = new BitmapImage(new Uri("D:\\logo.ico"));
            this.Icon = bi;
            this.Title = "Изменить товар";
            SetSKU(s, sa);
            List<Category> lc = mc.Categories.ToList();
            CBCategory.ItemsSource = lc;
            int i=0;
            foreach(Category c in CBCategory.ItemsSource)
            {
                Subcategory sub = mc.Subcategories.First(x => x.ID == s.ID_Subcategory);
                if(c.Name==sub.Category.Name)
                {
                    break;
                }
                i++;
            }
            CBCategory.SelectedIndex = i;
            i = 0;
            foreach(Subcategory sc in CBSubcategory.ItemsSource)
            {
                if (sc.ID == s.ID_Subcategory)
                    break;
                i++;
            }
            CBSubcategory.SelectedIndex = i;
            i = 0;
            foreach (Brend b in CBBrend.ItemsSource)
            {
                if (b.ID == s.ID_Brend)
                    break;
                i++;
            }
            CBBrend.SelectedIndex = i;
        }

        private void CBSubcategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //метод инициализирует комобобокс бренд и подтягивает список характеристик для данной 
            //подкатегории при выборе подкатегории
            Subcategory sub=CBSubcategory.SelectedItem as Subcategory;
            MContext mc=new MContext();
            var list = mc.ASCs.Where(x => x.ID == sub.ID).ToList();
            s = list[0];
            //вызов метода устонавливает значения списка характеристик
            SetLabel();
            List<Brend> lb = mc.Brends.OrderBy(x => x.Name).ToList();
            CBBrend.ItemsSource = lb;
           
        }

        private void TBTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            //при вводе названия активируется кнопка
            if (TBTitle.Text.Length != 0)
            {
                ADD.IsEnabled = true;
            }
            else
            {
                ADD.IsEnabled = false;
            }
        }

        private void CBCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //при выборе каьегории инициализируется комобобокс подкатегорий
            Category c = CBCategory.SelectedItem as Category;
            MContext mc = new MContext();
            List<Subcategory> ls = mc.Subcategories.Where(x => x.ID_Category == c.ID).ToList();
            CBSubcategory.ItemsSource = ls.OrderBy(x=>x.Name);
        }

        void SetLabel()
        {
            //устонавливается список характеристик
            TBVal1.Content = s.Har1;
            TBVal2.Content = s.Har2;
            TBVal3.Content = s.Har3;
            TBVal4.Content = s.Har4;
            TBVal5.Content = s.Har5;
            TBVal6.Content = s.Har6;
            TBVal7.Content = s.Har7;
            TBVal8.Content = s.Har8;
            TBVal9.Content = s.Har9;
            TBVal10.Content = s.Har10;
            TBVal11.Content = s.Har11;
            TBVal12.Content = s.Har12;
        }

        private void ADD_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                //по нажатию кнопки добавляеися товар
                MContext mc = new MContext();
                Brend b = CBBrend.SelectedItem as Brend;
                Subcategory s = CBSubcategory.SelectedItem as Subcategory;
                SKU sku = new SKU()
                {
                    ID_Brend = b.ID,
                    ID_Subcategory = s.ID,
                    Enable = true,
                    idAllo = Allo.Text,
                    idComfy = Comfy.Text,
                    idEldorado = Eldorado.Text,
                    idRozetka = Rozetka.Text,
                    InputPrice = double.Parse(TBInputPrice.Text),
                    Name = TBTitle.Text,
                    Price = int.Parse(TBPrice.Text),
                    UrlHotline = Hotline.Text
                };
                mc.SKUs.Add(sku);
                mc.SaveChanges();
                SkuAnot sa = new SkuAnot()
                {
                    ID = sku.ID,
                    Val1 = Value1.Text,
                    Val2 = Value2.Text,
                    Val3 = Value3.Text,
                    Val4 = Value4.Text,
                    Val5 = Value5.Text,
                    Val6 = Value6.Text,
                    Val7 = Value7.Text,
                    Val8 = Value8.Text,
                    Val9 = Value9.Text,
                    Val10 = Value10.Text,
                    Val11 = Value11.Text,
                    Val12 = Value12.Text
                };
                mc.SAs.Add(sa);
                mc.SaveChanges();
                MessageBox.Show("Товар успешно добавлен");
                this.Close();
            }
            catch
            {
                MessageBox.Show("Неверно заполнено поле");
            }

        }

        private void CBBrend_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //при выборе бренда активируется бокс для ввода модели
            if(CBBrend.SelectedIndex!=-1)
                 TBTitle.IsEnabled = true;
            else
                TBTitle.IsEnabled = false;
        }

        void SetSKU(SKU s, SkuAnot sa)
        {
            //установка данных из файла
            Allo.Text = s.idAllo;
            Comfy.Text = s.idComfy;
            Eldorado.Text = s.idEldorado;
            Rozetka.Text = s.idRozetka;
            TBInputPrice.Text = string.Format("{0}", s.InputPrice);
            TBTitle.Text = s.Name;
            TBPrice.Text = string.Format("{0}", s.Price);
            Hotline.Text = s.UrlHotline;
            Value1.Text = sa.Val1;
            Value2.Text = sa.Val2;
            Value3.Text = sa.Val3;
            Value4.Text = sa.Val4;
            Value5.Text = sa.Val5;
            Value6.Text = sa.Val6;
            Value7.Text = sa.Val7;
            Value8.Text = sa.Val8;
            Value9.Text = sa.Val9;
            Value10.Text = sa.Val10;
            Value11.Text = sa.Val11;
            Value12.Text = sa.Val12;
        }

        private void ADDBrend_Click(object sender, RoutedEventArgs e)
        {
            //кнопка добавления бренда вызывает окно добавления бренда
            Add_Brend_Category abc = new Add_Brend_Category("Бренд");
            abc.ShowDialog();
            MContext mc = new MContext();
            List<Brend> lb = mc.Brends.OrderBy(x => x.Name).ToList();
            CBBrend.ItemsSource = lb;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MContext mc = new MContext();
                Brend b = CBBrend.SelectedItem as Brend;
                Subcategory s = CBSubcategory.SelectedItem as Subcategory;
                SKU savesku = mc.SKUs.First(x => x.ID == sk.ID);
                savesku.ID_Brend = b.ID;
                savesku.ID_Subcategory = s.ID;
                savesku.Enable = true;
                savesku.idAllo = Allo.Text;
                savesku.idComfy = Comfy.Text;
                savesku.idEldorado = Eldorado.Text;
                savesku.idRozetka = Rozetka.Text;
                savesku.InputPrice = double.Parse(TBInputPrice.Text);
                savesku.Name = TBTitle.Text;
                savesku.Price = int.Parse(TBPrice.Text);
                savesku.UrlHotline = Hotline.Text;
                mc.SaveChanges();



                SkuAnot sa = mc.SAs.First(x => x.ID == savesku.ID);
                sa.Val1 = Value1.Text;
                sa.Val2 = Value2.Text;
                sa.Val3 = Value3.Text;
                sa.Val4 = Value4.Text;
                sa.Val5 = Value5.Text;
                sa.Val6 = Value6.Text;
                sa.Val7 = Value7.Text;
                sa.Val8 = Value8.Text;
                sa.Val9 = Value9.Text;
                sa.Val10 = Value10.Text;
                sa.Val11 = Value11.Text;
                sa.Val12 = Value12.Text;
                mc.SaveChanges();
                MessageBox.Show("Товар успешно изменен");
                this.Close();
            }
            catch
            {
                MessageBox.Show("Неверно заполнено поле");
            }
        }
    }
}
