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
using System.Net;
using System.IO;


namespace Metelica
{
    /// <summary>
    /// Interaction logic for NewParsing.xaml
    /// </summary>
    public partial class NewParsing : Window
    {
        int ComfyProc = 0;
        int EldoradoProc = 0;
        int AlloProc = 0;
        int RozetkaProc = 0;
        int ComfyTotalProc = 0;
        int EldoradoTotalProc = 0;
        int AlloTotalProc = 0;
        int RozetkaTotalProc = 0;
        List<SKU> list;
        public NewParsing()
        {
            InitializeComponent();
            MContext mc = new MContext();
            List<Subcategory> ls = mc.Subcategories.ToList();
            CheckSubcategory.ItemsSource = ls.OrderBy(x=>x.Name);
            BitmapImage bi = new BitmapImage(new Uri("D:\\logo.ico"));
            this.Icon = bi;
        }

        public NewParsing(List<SKU> list)
        {
            InitializeComponent();
            MContext mc = new MContext();
            List<Subcategory> ls = mc.Subcategories.ToList();
            CheckSubcategory.ItemsSource = ls.OrderBy(x=>x.Name);
            BitmapImage bi = new BitmapImage(new Uri("D:\\logo.ico"));
            this.Icon = bi;
            this.list = list;
            NComfy.Content = "EK";
            NRozetka.Visibility = Visibility.Hidden;
            PRozetka.Visibility = Visibility.Hidden;
            LRozetka.Visibility = Visibility.Hidden;
            NAllo.Visibility = Visibility.Hidden;
            PAllo.Visibility = Visibility.Hidden;
            LAllo.Visibility = Visibility.Hidden;
            NEldorado.Visibility = Visibility.Hidden;
            PEldorado.Visibility = Visibility.Hidden;
            LEldorado.Visibility = Visibility.Hidden;
            Start.Visibility = Visibility.Hidden;
            EKStart.Visibility = Visibility.Visible;
        }

        private void SelectAll_Checked(object sender, RoutedEventArgs e)
        {
            if (SelectAll.IsChecked == true)
                CheckSubcategory.SelectAll();
            else
                CheckSubcategory.UnselectAll();
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            Start.IsEnabled = false;
            MContext mc = new MContext();
            List<SKU> lsku = mc.SKUs.ToList();
            foreach (SKU s in lsku)
            {
                s.AlloPrice = 0;
                s.AlloPromo = 0;
                s.ChangePrice = false;
                s.ComfyPrice = 0;
                s.ComfyPromo = 0;
                s.EldoradoPrice = 0;
                s.EldoradoPromo = 0;
                s.HotlinePrice = 0;
                s.RozetkaPrice = 0;
                s.RozetkaPromo = 0;
                s.TotalPrice = 0;
                s.TotalPromo = 0;
                mc.SaveChanges();
            }
            List<string> lUrlComfy = new List<string>();
            List<string> lUrlRozetka = new List<string>();
            List<string> lUrlAllo = new List<string>();
            List<string> lUrlEldorado = new List<string>();
            foreach (Subcategory s in CheckSubcategory.SelectedItems)
            {
                lUrlComfy.Add(s.UrlComfy);
                lUrlRozetka.Add(s.UrlRozetka);
                lUrlAllo.Add(s.UrlAllo);
                lUrlEldorado.Add(s.UrlEldorado);
            }
            UrlCompetitors uc = new UrlCompetitors() { lUrlComfy = lUrlComfy, lUrlRozetka = lUrlRozetka, lUrlAllo = lUrlAllo, lUrlEldorado = lUrlEldorado };
            Thread th = new Thread(SeeTread);
            th.Start(uc);

        }

        void SeeTread(object obj)
        {
            UrlCompetitors uc = obj as UrlCompetitors;
            //создвем  4 потока запускающих парсинг данных
            Thread[] th = new Thread[4];
            th[0] = new Thread(Comfy);
            th[0].Start(uc.lUrlComfy);
            th[1] = new Thread(Rozetka);
            th[1].Start(uc.lUrlRozetka);
            th[2] = new Thread(Allo);
            th[2].Start(uc.lUrlAllo);
            th[3] = new Thread(Eldorado);
            th[3].Start(uc.lUrlEldorado);
            Thread ProcTh = new Thread(SetProcTh);
            ProcTh.Start();
            for (int i = 0; i < th.Length; ++i)
            {
                th[i].Join();
            }
            MessageBox.Show("Обновление БД завершенно");
        }

        void SetProcTh(object obj)
        {
            try
            {
                do
                {
                    PComfy.Dispatcher.Invoke(new Action(() => { PComfy.Value = ComfyProc; }));
                    PEldorado.Dispatcher.Invoke(new Action(() => { PEldorado.Value = EldoradoProc; }));
                    PAllo.Dispatcher.Invoke(new Action(() => { PAllo.Value = AlloProc; }));
                    PRozetka.Dispatcher.Invoke(new Action(() => { PRozetka.Value = RozetkaProc; }));
                    LComfy.Dispatcher.Invoke(new Action(() => { LComfy.Content = string.Format("{0}%", ComfyTotalProc); }));
                    LEldorado.Dispatcher.Invoke(new Action(() => { LEldorado.Content = string.Format("{0}%", EldoradoTotalProc); }));
                    LAllo.Dispatcher.Invoke(new Action(() => { LAllo.Content = string.Format("{0}%", AlloTotalProc); }));
                    LRozetka.Dispatcher.Invoke(new Action(() => { LRozetka.Content = string.Format("{0}%", RozetkaTotalProc); }));
                }
                while (ComfyTotalProc != 100 || AlloTotalProc != 100 || EldoradoTotalProc != 100 || RozetkaTotalProc != 100);
            }
            catch { }
        }

        #region Comfy
        void Comfy(object obj)
        {
            List<string> lUrl = obj as List<string>;
            //часть
            int part = 1;
            //пока не пройдет все категории
            foreach (string u in lUrl)
            {
                if (u == string.Empty)
                {
                    part++;
                    continue;
                }
                //получаем кол-во страниц в категории
                int count = GetCountPagesComfy(u);
                //лист хранящий продукты
                List<Product> ListProduct = new List<Product>();
                //пока не пройдем все листы в категории
                for (int i = 0; i < count; i++)
                {
                    //получаем данные в лист с каждой страницы
                    GetListProductComfy(GetNextPageComfy(u, i), ListProduct);
                    ComfyProc = (int)(i * 100 / count);
                }

                //создаем поток и отправляем туда лист с ценами для обновления в базе
                //PComfy.Value = (int)(part / lUrl.Count)*100;             
                ComfyTotalProc = (int)(part*100 / lUrl.Count);
                part++;
                UBD ubd = new UBD() { Name = "Comfy", lp = ListProduct };
                Thread th = new Thread(UpdateBD);
                th.Start(ubd);
            }
            ComfyProc = 100;
            ComfyTotalProc = 100;
        }

        static void GetListProductComfy(string Url, List<Product> lp)
        {
            string content;

            StreamReader sr = GetSR(Url);
            string promo = string.Empty;
            try
            {
                while ((content = sr.ReadLine()) != null)
                {
                    Product pr = new Product();
                    int index_promo = -1;
                    int index = -1;
                    string parserch_promo = "title=\"ВЕРНЕМ ";
                    string parserch = "data-content-name=\"";
                    index_promo = content.IndexOf(parserch_promo);
                    index = content.IndexOf(parserch);
                    if (index_promo > 0)
                    {
                        string s = content.Remove(index_promo, parserch_promo.Length);
                        s = s.Remove(0, index_promo);
                        s = s.Remove(s.IndexOf('%'));
                        promo = s;
                    }
                    if (index > 0)
                    {
                        string s = content.Remove(index, parserch.Length);
                        s = s.Remove(0, index);
                        s = s.Remove(s.IndexOf('"'));
                        pr.FullName = s;
                        parserch = "data-content-ids=\"";
                        do
                        {
                            content = sr.ReadLine();
                            index = content.IndexOf(parserch);
                            if (index > 0)
                            {
                                s = content.Remove(index, parserch.Length);
                                s = s.Remove(0, index);
                                s = s.Remove(s.IndexOf('"'));
                                pr.ID = s;
                            }
                        }
                        while (index < 0);
                        parserch = "data-content-price=\"";
                        do
                        {
                            content = sr.ReadLine();
                            index = content.IndexOf(parserch);
                            if (index > 0)
                            {
                                s = content.Remove(index, parserch.Length);
                                s = s.Remove(0, index);
                                s = s.Remove(s.IndexOf('.'));
                                pr.Price = s;
                                pr.Promo = promo;
                                lp.Add(pr);
                                promo = string.Empty;
                            }
                        }
                        while (index < 0);
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(string.Format("Ошибка на comfy\r\n {0}\r\n{2}", Url, lp.Count));
                
            }
            finally
            {
                sr.Close();
            }
        }

        static string GetNextPageComfy(string Url, int i)
        {
            return string.Format("{0}?p={1}", Url, i + 1);
        }

        static int GetCountPagesComfy(string Url)
        {
            string content;
            int count = 0;
            StreamReader sr = GetSR(Url);
            while ((content = sr.ReadLine()) != null)
            {
                int index = -1;
                string parserch = "category__list__top__count\" id=\"productsCount";
                index = content.IndexOf(parserch);
                if (index > 0)
                {
                    content = sr.ReadLine();
                    string s = content.Replace(" ", string.Empty);
                    string[] mas = s.Split('м');
                    count = int.Parse(mas[0]) / 22;
                }

            }
            return count + 1;
        }

        #endregion Comfy
        #region Rozetka
        void Rozetka(object obj)
        {
            List<string> lUrl = obj as List<string>;
            //часть
            int part = 1;
            //пока не пройдет все категории
            foreach (string u in lUrl)
            {
                if (u == string.Empty)
                {
                    part++;
                    continue;
                }
                //получаем кол-во страниц в категории
                int count = GetCountPagesRozetka(u);
                //лист хранящий продукты
                List<Product> ListProduct = new List<Product>();
                //пока не пройдем все листы в категории
                for (int i = 0; i < count; i++)
                {
                    //получаем данные в лист с каждой страницы
                    GetListProductRozetka(GetNextPageRozetka(u, i), ListProduct);
                    RozetkaProc = (int)(i * 100 / count);
                }

                //создаем поток и отправляем туда лист с ценами для обновления в базе
                //PComfy.Value = (int)(part / lUrl.Count)*100;
                
                RozetkaTotalProc = (int)(part*100 / lUrl.Count);
                part++;
                UBD ubd = new UBD() { Name = "Rozetka", lp = ListProduct };
                Thread th = new Thread(UpdateBD);
                th.Start(ubd);
            }
            RozetkaProc = 100;
            RozetkaTotalProc = 100;
        }

        static string GetNextPageRozetka(string Url, int i)
        {
            string end = "view=list/";
            int index = Url.IndexOf(end);
            string tempfirst = Url.Substring(0, (Url.Length - end.Length));
            string templast = Url.Remove(0, index);
            return string.Format("{0}page={1};{2}", tempfirst, i + 1, templast);
        }
        static int GetCountPagesRozetka(string Url)
        {
            string content;
            StreamReader sr = GetSR(Url);
            string count = null;
            while ((content = sr.ReadLine()) != null)
            {
                int index = -1;
                string parserch = "<span class=\"paginator-catalog-l-i-active hidden\">";
                index = content.IndexOf(parserch);
                if (index > 0)
                {
                    string s = content.Remove(index, parserch.Length);
                    s = s.Remove(0, index);
                    string[] mas = s.Split('<');
                    count = mas[0];
                }
            }
            sr.Close();
            if(count==null)
            {
                MessageBox.Show(string.Format("Rozetka значение страницы 0 ссылка {0}",Url));
                return 0;
            }
                
            return int.Parse(count);
        }

        static void GetListProductRozetka(string Url, List<Product> list)
        {
            string content;
            StreamReader sr = GetSR(Url);

            while ((content = sr.ReadLine()) != null)
            {
                int index = -1;
                //страница пуста
                string parserch = "header-filter-empty-title-i sprite-side";
                index = content.IndexOf(parserch);
                //если находит этот индекс то конец цикла
                if (index > 0) break;
                bool flag = true;

                //в этой строке цена
                parserch = "var pricerawjson = \"%7B%22price%22%3A";
                index = content.IndexOf(parserch);
                //если нашла заходим и вытаскиваем цену
                if (index > 0)
                {
                    Product prod = new Product();
                    string s = content.Remove(index, parserch.Length);
                    s = s.Remove(0, index);
                    s = s.Remove(s.IndexOf('%'));
                    // s = s.Replace('.', ',');
                    //double pr = double.Parse(s);
                    // prod.Price = Math.Round(pr * kurs, 0);
                    prod.Price = s;
                    //дальше ищем видимость
                    parserch = "price_json.hide_old_price =";
                    do
                    {
                        content = sr.ReadLine();
                        index = content.IndexOf(parserch);
                        string no = "\t\t\tprice_json.hide_old_price = 1;";
                        if (index > 0)
                        {
                            if (content == no)
                            {
                                flag = false;
                            }
                        }
                    }
                    while (index < 0);
                    //дальше код товара
                    parserch = "priceFormatter.setFormattedPrice(";
                    do
                    {
                        content = sr.ReadLine();
                        index = content.IndexOf(parserch);
                        if (index > 0)
                        {

                            s = content.Remove(index, parserch.Length);
                            s = s.Remove(0, index);
                            string[] mas = s.Split(')');
                            prod.ID = mas[0];
                        }
                    }
                    while (index < 0);
                    // дальше если найдем эту строку то это имя
                    parserch = "class=\"underline\">";
                    // если до имени найдем эту строку то это промо цена
                    string promo = "class=\"g-addprice-text-price\">";
                    int index_promo = -1;
                    do
                    {
                        content = sr.ReadLine();
                        index_promo = content.IndexOf(promo);
                        string count = null;
                        if (index_promo > 0)
                        {
                            s = content.Remove(index_promo, promo.Length);
                            s = s.Remove(0, index_promo);
                            string[] mas = s.Split('<');
                            count = mas[0];
                            count = count.Replace(" ", string.Empty);
                            prod.Price = count;
                        }
                        index = content.IndexOf(parserch);
                        if (index > 0)
                        {
                            content = sr.ReadLine();
                            //s = content.Remove(index, parserch.Length);
                            s = content.TrimStart().TrimEnd();
                            prod.FullName = s;
                            if (flag)
                                list.Add(prod);
                        }
                    }
                    while (index < 0);
                }
            }

            sr.Close();
        }
        #endregion Rozetka
        #region Allo
        void Allo(object obj)
        {
            List<string> lUrl = obj as List<string>;
            //часть
            int part = 1;
            //пока не пройдет все категории
            foreach (string u in lUrl)
            {
                if (u == string.Empty)
                {
                    part++;
                    continue;
                }
                //получаем кол-во страниц в категории
                int count = GetCountPagesAllo(u);
                //лист хранящий продукты
                List<Product> ListProduct = new List<Product>();
                //пока не пройдем все листы в категории
                for (int i = 0; i < count; i++)
                {
                    //получаем данные в лист с каждой страницы
                    GetListProductAllo(GetNextPageAllo(u, i), ListProduct);
                    AlloProc = (int)(i * 100 / count);
                }

                //создаем поток и отправляем туда лист с ценами для обновления в базе
                //PComfy.Value = (int)(part / lUrl.Count)*100;
               
                AlloTotalProc = (int)(part*100 / lUrl.Count);
                part++;
                UBD ubd = new UBD() { Name = "Allo", lp = ListProduct };
                Thread th = new Thread(UpdateBD);
                th.Start(ubd);
            }
            AlloProc = 100;
            AlloTotalProc = 100;
        }

        static string GetNextPageAllo(string Url, int i)
        {
            return string.Format("{0}p-{1}/", Url, i + 1);
        }

        static void GetListProductAllo(string Url, List<Product> lp)
        {
            StreamReader sr = GetSR(Url);
            string content;
            while (((content = sr.ReadLine()) != null))
            {
                Product pr = new Product();
                bool flag = true;
                int index = -1;
                //строка продукта
                string preserch = "class=\"product-container-all\"";
                index = content.IndexOf(preserch);
                //нашли заходим
                if (index > 0)
                {
                    //строка имени
                    preserch = "title=\"";
                    do
                    {
                        content = sr.ReadLine();
                        index = content.IndexOf(preserch);
                        //нашли имя заходим достаем его
                        if (index > 0)
                        {

                            string s = content.Remove(0, index);
                            s = s.Remove(0, preserch.Length);
                            s = s.Remove(s.IndexOf('"'));
                            int index_del = s.IndexOf("&amp;");
                            if (index_del > 0) s = s.Replace("&amp;", "&");
                            index_del = s.IndexOf("&quot;");
                            if (index_del > 0) s = s.Replace("&quot;", "'");
                            pr.FullName = s;
                            //Console.WriteLine(s);
                        }
                    }
                    while (index < 0);
                    //строка кода товара
                    preserch = "код товара: ";
                    do
                    {
                        content = sr.ReadLine();
                        index = content.IndexOf(preserch);
                        //нашли код товара зашли достали его
                        if (index > 0)
                        {

                            string s = content.Remove(0, index);
                            s = s.Remove(0, preserch.Length);
                            s = s.Remove(s.IndexOf('<'));
                            s = s.TrimEnd(' ');
                            pr.ID = s;
                            //Console.WriteLine(s);
                        }
                    }
                    while (index < 0);
                    //строка цены
                    preserch = "class=\"sum\"";
                    //строка промо
                    string old = "class=\"old-price-box\"";
                    //строка наличия
                    string preserchability = "class=\"no-stock-message\"";
                    string last="class=\"last-price\"";
                    do
                    {
                        content = sr.ReadLine();
                        index = content.IndexOf(preserch);
                        int old_index = content.IndexOf(old);
                        int indexability = content.IndexOf(preserchability);
                        int lastprice = content.IndexOf(last);
                        //если нет в наличии выходим из цикла и меняем флаг добавления в корзину
                        if (indexability > 0||lastprice>0)
                        {
                            flag = false;
                            break;
                        }
                        //если нашли цену заходим достаем ее
                        if (index > 0)
                        {

                            string s = content.Remove(0, index);
                            s = s.Remove(0, preserch.Length + 1);
                            if (s == string.Empty)
                            {
                                flag = false;
                                break;
                            }
                            s = s.Remove(0, 1);
                            s = s.Remove(s.IndexOf('<'));
                            // Console.WriteLine(s);
                            string price = string.Empty;
                            if (s.Length > 3)
                            {
                                for (int i = 0; i < s.Length; i++)
                                {
                                    if (char.IsDigit(s[i]))
                                        price += s[i];
                                }
                                pr.Price = price;
                            }
                            else
                            {
                                pr.Price = s;
                            }
                        }
                        //если нашли промо
                        if (old_index > 0)
                        {
                            do
                            {
                                content = sr.ReadLine();
                                index = content.IndexOf(preserch);
                                //ищем цену достаем ее
                                if (index > 0)
                                {
                                    content = sr.ReadLine();

                                    string s = content.Remove(content.IndexOf('<'));
                                    s = s.TrimStart(' ');
                                    s = s.TrimEnd(' ');
                                    // Console.WriteLine(s);
                                    string price = string.Empty;
                                    if (s.Length > 3)
                                    {
                                        for (int i = 0; i < s.Length; i++)
                                        {
                                            if (char.IsDigit(s[i]))
                                                price += s[i];
                                        }
                                        pr.Price = price;
                                    }
                                    else
                                    {
                                        pr.Price = s;
                                    }
                                }

                            }
                            while (index < 0);
                            //затем ищем промо
                            preserch = "class=\"new_sum\"";
                            do
                            {
                                content = sr.ReadLine();
                                index = content.IndexOf(preserch);
                                if (index > 0)
                                {
                                    content = sr.ReadLine();
                                    string s = content.Remove(content.IndexOf('<'));
                                    s = s.TrimStart(' ');
                                    s = s.TrimEnd(' ');
                                    string price = string.Empty;
                                    if (s.Length > 3)
                                    {
                                        for (int i = 0; i < s.Length; i++)
                                        {
                                            if (char.IsDigit(s[i]))
                                                price += s[i];
                                        }
                                        pr.Price = price;
                                    }
                                    else
                                    {
                                        pr.Price = s;
                                    }
                                }

                            }
                            while (index < 0);

                        }

                    }
                    while (index < 0);
                    //проверяем есть ли товар в наличии
                    preserch = "class=\"no-stock-message\"";
                    for (int i = 0; i < 7; i++)
                    {
                        content = sr.ReadLine();
                        int indexflag = content.IndexOf(preserch);
                        if (indexflag > 0) flag = false;
                    }
                    //если товар в наличии добавляем его в лист
                    if (flag) lp.Add(pr);
                }

            }
            sr.Close();
        }
        static int GetCountPagesAllo(string Url)
        {

            StreamReader sr = GetSR(Url);
            string content;
            int count = 0;
            bool flag = true;

            while (((content = sr.ReadLine()) != null) && flag)
            {

                int index = -1;
                index = content.IndexOf("class=\"current\"");
                if (index > 0)
                {
                    string parserch = "class=\"last\"";
                    string pr_break = "class=\"next i-next \"";
                    do
                    {
                        content = sr.ReadLine();
                        index = content.IndexOf(parserch);
                        int index_p = content.IndexOf(Url);
                        int index_break = content.IndexOf(pr_break);
                        if (index_break > 0) flag = false;
                        if (index > 0)
                        {
                            content = sr.ReadLine();
                            content = sr.ReadLine();
                            content = sr.ReadLine();
                            index = content.IndexOf(">");
                            string s = content.Remove(0, index + 1);
                            s = s.Remove(s.IndexOf('<'));
                            count = int.Parse(s);
                            flag = false;
                        }
                        if (index_p > 0)
                        {
                            content = sr.ReadLine();
                            content = sr.ReadLine();
                            index = content.IndexOf(">");
                            string s = content.Remove(0, index + 1);
                            s = s.Remove(s.IndexOf('<'));
                            count = int.Parse(s);
                        }
                    }
                    while (flag);
                }

            }
            sr.Close();
            return count;
        }
        #endregion
        #region Eldorado
        void Eldorado(object obj)
        {
            List<string> lUrl = obj as List<string>;
            //часть
            int part = 1;
            //пока не пройдет все категории
            foreach (string u in lUrl)
            {
                if (u == string.Empty)
                {
                    part++;
                    continue;
                }
                //получаем кол-во страниц в категории
                int count = GetCountPagesEldorado(u);
                //лист хранящий продукты
                List<Product> ListProduct = new List<Product>();
                //пока не пройдем все листы в категории
                for (int i = 0; i < count; i++)
                {
                    //получаем данные в лист с каждой страницы
                    GetListProductEldorado(GetNextPageEldorado(u, i), ListProduct);
                    EldoradoProc = (int)(i * 100 / count);
                }

                //создаем поток и отправляем туда лист с ценами для обновления в базе
                //PComfy.Value = (int)(part / lUrl.Count)*100;
               
                EldoradoTotalProc = (int)(part*100 / lUrl.Count);
                part++;
                UBD ubd = new UBD() { Name = "Eldorado", lp = ListProduct };
                Thread th = new Thread(UpdateBD);
                th.Start(ubd);
            }
            EldoradoProc = 100;
            EldoradoTotalProc = 100;
        }


        static int GetCountPagesEldorado(string Url)
        {
            StreamReader sr = GetSR(Url);
            string content;
            int count = 1;
            string s = string.Empty;
            while (((content = sr.ReadLine()) != null))
            {

                int index = -1;
                index = content.IndexOf("class=\"page-i\"");
                if (index > 0)
                {
                    string tmp = "page=";
                    s = content.Remove(0, content.IndexOf(tmp));
                    s = s.Remove(0, tmp.Length);
                    string[] mas = s.Split('/');
                    s = mas[0];
                }

            }
            sr.Close();
            if (s != string.Empty)
                return int.Parse(s);
            else return count;
        }

        static string GetNextPageEldorado(string Url, int i)
        {
            return string.Format("{0};page={1}//", Url, i + 1);
        }

        static void GetListProductEldorado(string Url, List<Product> lp)
        {
            StreamReader sr = GetSR(Url);
            string content;

            while (((content = sr.ReadLine()) != null))
            {
                Product pr = new Product();
                int index = -1;
                //строка продукта

                string preserch = "class=\"pp-code forCodeColor\"";
                index = content.IndexOf(preserch);
                //нашли заходим
                if (index > 0)
                {
                    content = sr.ReadLine();
                    string tmp = "Код товара:";
                    content = content.Remove(0, content.IndexOf(tmp));
                    content = content.Remove(0, tmp.Length);
                    string[] mas = content.Split('-');
                    pr.ID = mas[0];
                    preserch = "class=\"g-i-title \"";
                    do
                    {
                        content = sr.ReadLine();
                        index = content.IndexOf(preserch);
                        //нашли заходим
                        if (index > 0)
                        {
                            mas = content.Split('>');
                            mas[2] = mas[2].Remove(mas[2].IndexOf('<'));
                            pr.FullName = mas[2];
                            preserch = "class=\"to-right price";
                            int j = 0;
                            do
                            {
                                content = sr.ReadLine();
                                index = content.IndexOf(preserch);
                                if (index > 0)
                                {
                                    mas = content.Split('>');

                                    mas = mas[1].Split('.');
                                    mas[0] = mas[0].Replace(" ", string.Empty);
                                    pr.Price = mas[0];
                                    lp.Add(pr);
                                }
                                j++;
                                if (j > 20)
                                    index = 1;
                            }
                            while (index < 0);
                        }
                    }
                    while (index < 0);
                }


            }
            sr.Close();
        }
        #endregion

        private void CheckSubcategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CheckSubcategory.SelectedItems.Count != 0)
            {
                if (Start.Visibility != Visibility.Hidden)
                    Start.IsEnabled = true;
                if (EKStart.Visibility != Visibility.Hidden)
                    EKStart.IsEnabled = true;
            }
            else
            {
                if (Start.Visibility != Visibility.Hidden)
                    Start.IsEnabled = false;
                if (EKStart.Visibility != Visibility.Hidden)
                    EKStart.IsEnabled = false;
            }
            if (CheckSubcategory.SelectedItems.Count != CheckSubcategory.Items.Count)
            {
                SelectAll.IsChecked = false;
            }
        }

        static StreamReader GetSR(string Url)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Url);
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            StreamReader sr = new StreamReader(resp.GetResponseStream());
            return sr;
        }

        void UpdateBD(object obj)
        {
            MContext mc = new MContext();
            UBD u = obj as UBD;
            List<Product> lp = new List<Product>();
            foreach (Product p in u.lp)
            {
                switch (u.Name)
                {
                    case "Comfy":
                        {
                            List<SKU> ls = mc.SKUs.Where(x => x.idComfy == p.ID).ToList();
                            if (ls.Count != 0)
                            {
                                foreach (SKU s in ls)
                                {
                                    s.ComfyPrice = int.Parse(p.Price);
                                    if (p.Promo != string.Empty)
                                        s.ComfyPromo = int.Parse(p.Promo);
                                    mc.SaveChanges();
                                }
                            }
                            else
                            {
                                lp.Add(p);
                            }
                            break;
                        }
                    case "Rozetka":
                        {
                            List<SKU> ls = mc.SKUs.Where(x => x.idRozetka == p.ID).ToList();
                            if (ls.Count != 0)
                            {
                                foreach (SKU s in ls)
                                {
                                    s.RozetkaPrice = int.Parse(p.Price);
                                    if (p.Promo != string.Empty && p.Promo != null)
                                        s.RozetkaPromo = int.Parse(p.Promo);
                                    mc.SaveChanges();
                                }
                            }
                            else
                            {
                                lp.Add(p);
                            }
                            break;
                        }
                    case "Allo":
                        {
                            List<SKU> ls = mc.SKUs.Where(x => x.idAllo == p.ID).ToList();
                            if (ls.Count != 0)
                            {
                                foreach (SKU s in ls)
                                {
                                    s.AlloPrice = int.Parse(p.Price);
                                    if (p.Promo != string.Empty && p.Promo != null)
                                        s.AlloPromo = int.Parse(p.Promo);
                                    mc.SaveChanges();
                                }
                            }
                            else
                            {
                                lp.Add(p);
                            }
                            break;
                        }
                    case "Eldorado":
                        {
                            List<SKU> ls = mc.SKUs.Where(x => x.idEldorado == p.ID).ToList();
                            if (ls.Count != 0)
                            {
                                foreach (SKU s in ls)
                                {
                                    s.EldoradoPrice = int.Parse(p.Price);
                                    if (p.Promo != string.Empty && p.Promo != null)
                                        s.EldoradoPromo = int.Parse(p.Promo);
                                    mc.SaveChanges();
                                }
                            }
                            else
                            {
                                lp.Add(p);
                            }
                            break;
                        }
                }
            }
            using (StreamWriter sw = new StreamWriter(string.Format("D:\\{0}.csv", u.Name), true, Encoding.UTF8))
            {
                foreach (Product p in lp)
                {
                    sw.WriteLine(GetStringOfFile(p));
                }
            }
        }

        static string GetStringOfFile(Product p)
        {
            return string.Format("{0};{1};{2};{3}", p.ID, p.FullName, p.Price, p.Promo);
        }

        void ThreadEK(object o)
        {
            Thread th = new Thread(EK);
            th.Start(o);
            Thread seePB = new Thread(SetProcTh);
            seePB.Start();
            th.Join();
            MessageBox.Show("Мониторинг EK завершен");

        }

        private void EKStart_Click(object sender, RoutedEventArgs e)
        {
            EKStart.IsEnabled = false;
            List<SKU> ls = new List<SKU>();
            foreach (Subcategory s in CheckSubcategory.SelectedItems)
            {
                ls.AddRange(list.Where(x => x.ID_Subcategory == s.ID).ToList());              
            }
            if (ls.Count != 0)
            {
                Thread th = new Thread(ThreadEK);
                th.Start(ls);
            }
            else
            {
                MessageBox.Show("Анализ ЕК не требуется");
            }
        }

        void EK(object o)
        {
            List<SKU> ls = o as List<SKU>;
            int priceEK=0;
            for(int i=0;i<ls.Count;i++)
            {
                if (ls[i].UrlHotline != string.Empty && ls[i].UrlHotline != null)
                {
                    if (ParsingEK(ls[i].UrlHotline, ref priceEK))
                    {
                        int id = ls[i].ID;
                        MContext mc = new MContext();
                        SKU sk = mc.SKUs.First(x => x.ID == id);
                        if (sk.Price != priceEK && priceEK != 0)
                        {
                            sk.TotalPrice = priceEK;
                            sk.ChangePrice = true;
                            sk.Who = "EK";
                            mc.SaveChanges();                          
                        }                    
                    }
                    else
                    {
                        i--;

                    }                
                }
                ComfyTotalProc = ComfyProc = (int)(i * 100 / ls.Count);
                priceEK = 0;
            }
            ComfyTotalProc = ComfyProc = 100;
        }

        bool ParsingEK(string s,ref int priceEK)
        {
            try
            {
                string content;
                StreamReader sr = GetSR(s);

                while ((content = sr.ReadLine()) != null)
                {
                    int index = -1;
                    string parserch = "content=\"";
                    index = content.IndexOf(parserch);
                    if (index > 0)
                    {
                        content = content.Remove(0, content.LastIndexOf(parserch));
                        content = content.Remove(0, parserch.Length);
                        content = content.Remove(content.IndexOf('.'));
                        string tmp = "по цене от ";
                        content = content.Remove(0, content.LastIndexOf(tmp));
                        content = content.Remove(0, tmp.Length);
                        string[] mas = content.Split(' ');
                        if (mas.Length > 1)
                        {
                            int first = int.Parse(mas[0]);
                            int second = int.Parse(mas[2]);
                            priceEK = (first + second) / 2;
                        }
                        else
                        {
                            priceEK = int.Parse(mas[0]);
                        }
                        break;
                    }                  
                }
                sr.Close();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка EK");
                MessageBox.Show(ex.Message);
                priceEK = 0;
                return false;
            }
        }
    }
}
