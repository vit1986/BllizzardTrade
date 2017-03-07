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
    /// Interaction logic for Add_Brend_Category.xaml
    /// </summary>
    /// 
    
    
    
    
    // Для добавления категории и бренда
    public partial class Add_Brend_Category : Window
    {
        // в конструктор приход строка категория или бренд
        public Add_Brend_Category(string str)
        {
            InitializeComponent();
            BitmapImage bi = new BitmapImage(new Uri("D:\\logo.ico"));
            this.Icon = bi;
            this.Title = str;
            lTitle.Content = "Введите новую " + str;
        }

        
        private void AddItems_TextChanged(object sender, TextChangedEventArgs e)
        {
            //активирует кнопку добавить
            if (AddItems.Text.Length != 0)
                ADD.IsEnabled = true;
            else
                ADD.IsEnabled = false;
        }

        private void ADD_Click(object sender, RoutedEventArgs e)
        {
            if (this.Title == "Категорию")
            {
                //в зависимотси от того что пришло в конструкторе добавляет или категорию или бренд
                MContext mc = new MContext();
                if (mc.Categories.Any(x => x.Name == AddItems.Text))
                {
                    MessageBox.Show(string.Format("Категория {0} уже существует", AddItems.Text));
                    AddItems.Text = string.Empty;
                    AddItems.Focus();
                }
                else
                {
                    Category c = new Category() { Name = AddItems.Text };
                    mc.Categories.Add(c);
                    mc.SaveChanges();
                    MessageBox.Show(string.Format("Категория {0} успешно добавленна", AddItems.Text));
                    this.Close();
                }
            }
            else
            {
                MContext mc = new MContext();
                if (mc.Brends.Any(x => x.Name == AddItems.Text))
                {
                    MessageBox.Show(string.Format("Бренд {0} уже существует", AddItems.Text));
                    AddItems.Text = string.Empty;
                    AddItems.Focus();
                }
                else
                {
                    Brend c = new Brend() { Name = AddItems.Text };
                    mc.Brends.Add(c);
                    mc.SaveChanges();
                    MessageBox.Show(string.Format("Бренд {0} успешно добавленна", AddItems.Text));
                    this.Close();
                }
            }
        }
    }
}
