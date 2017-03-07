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
    /// Interaction logic for ADD_Subcategory.xaml
    /// </summary>
    /// 

    //Окно добавляет подкатегорию
    public partial class ADD_Subcategory : Window
    {
        public ADD_Subcategory(string str)
        {
            //в конструкторе приходит строка для заголовка
            InitializeComponent();
            this.Title = str;
            BitmapImage bi = new BitmapImage(new Uri("D:\\logo.ico"));
            this.Icon = bi;
            //инициализируем комбобокс категориями
            MContext mc=new MContext();
            List<Category> lc = mc.Categories.ToList();
            CBCategory.ItemsSource = lc;
            List<PriceFormat> lpf = mc.PriceFormat.ToList();
            CBFP.ItemsSource = lpf;
        }

        private void CBCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //если категория выбрана активируем текстбокс для названия 
            if(CBCategory.SelectedIndex!=-1)
            {
                TBTitle.IsEnabled = true;
            }
            else
            {
                TBTitle.IsEnabled = false;
            }
        }

        private void TBTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            // если название введено активируем кнопку для добавления
            if(TBTitle.Text.Length!=0)
            {
                ADD.IsEnabled = true;
            }
            else
            {
                ADD.IsEnabled = false;
            }
        }

        private void ADD_Click(object sender, RoutedEventArgs e)
        {

            //по нажаьтию кнопки проверяем есть ли ткое название в базе если нет то добавляем
            MContext mc = new MContext();
            if(mc.Subcategories.Any(x=>x.Name==TBTitle.Text))
            {
                MessageBox.Show("Подкатегория {0} уже существует", TBTitle.Text);
                TBTitle.Text = string.Empty;
                TBTitle.Focus();
            }
            else
            {
                Category c=CBCategory.SelectedItem as Category;
                PriceFormat pf = CBFP.SelectedItem as PriceFormat;
                Subcategory s = new Subcategory() { Name = TBTitle.Text, ID_Category = c.ID, UrlComfy = Comfy.Text, 
                                                    UrlAllo = Allo.Text, UrlEldorado = Eldorado.Text, UrlRozetka = Rozetka.Text, 
                                                    ID_PriceFormat=pf.ID};
                mc.Subcategories.Add(s);
                mc.SaveChanges();
                AnotSubCat asc = new AnotSubCat()
                {
                    ID = s.ID,
                    Har1 = Value1.Text,
                    Har2 = Value2.Text,
                    Har3 = Value3.Text,
                    Har4 = Value4.Text,
                    Har5 = Value5.Text,
                    Har6 = Value6.Text,
                    Har7 = Value7.Text,
                    Har8 = Value8.Text,
                    Har9 = Value9.Text,
                    Har10 = Value10.Text,
                    Har11 = Value11.Text,
                    Har12 = Value12.Text,
                };
                mc.ASCs.Add(asc);
                mc.SaveChanges();
                MessageBox.Show("Подкатегория добавлена");
                this.Close();
            }
        }
    }
}
