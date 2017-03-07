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
using System.Threading;

namespace Metelica
{
    /// <summary>
    /// Interaction logic for AnaliticSku.xaml
    /// </summary>
    public partial class AnaliticSku : Window
    {
        int Procent = 0;
        public AnaliticSku()
        {
           //анализ простой
            InitializeComponent();
            //запускает два потока прогрессбара  и обработки данных
            Thread th = new Thread(SetProg);
            th.Start();
            Thread th2 = new Thread(Analitics);
            th2.Start();
        }
        //public AnaliticSku(List<SKU> ls)
        //{
        //    //в конструкторе лист ску для обработки товаров не имеющихся у прямых конкуренотов
        //    InitializeComponent();
        //    Thread th = new Thread(SetProg);
        //    th.Start();
        //    Thread th2 = new Thread(EK);
        //    th2.Start(ls);
        //}
        void Analitics(object o)
        {
            //процесс анализа
            MContext mc = new MContext();
            var ls = mc.SKUs.Where(x => x.Enable == true);
            int i = 0;
            foreach (SKU s in ls.ToList())
            {
                List<Subcategory> sub = mc.Subcategories.Where(x => x.ID == s.ID_Subcategory).ToList();

                int TotalPrice = 0;
                TotalPrice = s.ComfyPrice + sub[0].DeliveryPriceComfy;
                int TotalPromo = 0;
                TotalPromo = s.ComfyPromo;
                string sWho = "Comfy";
                int delivery = sub[0].DeliveryPriceComfy;
                if (TotalPrice - delivery == 0)
                {
                    TotalPrice = s.EldoradoPrice + sub[0].DeliveryPriceEldorado;
                    TotalPromo = 0;
                    delivery = sub[0].DeliveryPriceEldorado;
                    sWho = "Eldorado";
                }
                else
                {
                    if (((s.EldoradoPrice + sub[0].DeliveryPriceEldorado) < TotalPrice - (TotalPrice * TotalPromo) / 100) && s.EldoradoPrice != 0)
                    {
                        TotalPrice = s.EldoradoPrice + sub[0].DeliveryPriceEldorado;
                        TotalPromo = 0;
                        delivery = sub[0].DeliveryPriceEldorado;
                        sWho = "Eldorado";
                    }
                }
                if (TotalPrice - delivery == 0)
                {
                    TotalPrice = s.AlloPrice + sub[0].DeliveryPriceAllo;
                    TotalPromo = 0;
                    delivery = sub[0].DeliveryPriceAllo;
                    sWho = "Allo";
                }
                else
                {
                    if (((s.AlloPrice + sub[0].DeliveryPriceAllo) < TotalPrice - (TotalPrice * TotalPromo) / 100) && s.AlloPrice != 0)
                    {
                        TotalPrice = s.AlloPrice + sub[0].DeliveryPriceAllo;
                        TotalPromo = 0;
                        delivery = sub[0].DeliveryPriceAllo;
                        sWho = "Allo";
                    }
                }
                if (TotalPrice - delivery == 0)
                {
                    TotalPrice = s.RozetkaPrice + sub[0].DeliveryPriceRozetka;
                    TotalPromo = 0;
                    delivery = sub[0].DeliveryPriceRozetka;
                    sWho = "Rozetka";
                }
                else
                {
                    if (((s.RozetkaPrice + sub[0].DeliveryPriceRozetka) < TotalPrice - (TotalPrice * TotalPromo) / 100) && s.RozetkaPrice != 0)
                    {
                        TotalPrice = s.RozetkaPrice + sub[0].DeliveryPriceRozetka;
                        TotalPromo = 0;
                        delivery = sub[0].DeliveryPriceRozetka;
                        sWho = "Rozetka";
                    }
                }
                s.TotalPrice = TotalPrice;
                s.TotalPromo = TotalPromo;
                if ((TotalPrice - delivery) != 0)
                {
                    if (TotalPrice != s.Price)
                    {
                        s.ChangePrice = true;
                        s.Who = sWho;
                    }
                }
                else
                {
                    s.TotalPrice = 0;
                    s.TotalPromo = 0;
                }
                mc.SaveChanges();
                Procent = (int)(i * 100 / ls.ToList().Count);
                i++;
            }
            Procent = 100;
            MessageBox.Show("Анализ завершен");
            OK.Dispatcher.Invoke(new Action(() => { OK.IsEnabled = true; }));

        }
        void SetProg(object o)
        {
            do
            {
                Prog.Dispatcher.Invoke(new Action(() => { Prog.Value = Procent; }));
                Proc.Dispatcher.Invoke(new Action(() => { Proc.Content = string.Format("{0}%", Procent); }));
            }
            while (Procent != 100);

        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //void EK(object o)
        //{
        //    List<SKU> ls = o as List<SKU>;
        //    foreach(SKU s in ls)
        //    {
        //        //получаем цену с ек
        //    }
        //}
    }
}
