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
using System.Drawing;


namespace Metelica
{
    /// <summary>
    /// Interaction logic for PrintPrice.xaml
    /// </summary>
    public partial class PrintPrice : Window
    {
        List<string> lHarac = new List<string>(12) ;
        List<string> lValue = new List<string>(12);
        SKU s;
        AnotSubCat sa;

        public PrintPrice(SKU s, bool b)
        {
            InitializeComponent();
            
            MContext mc = new MContext();
            mc.SKUs.First(x => x.ID == s.ID).TotalPrice = s.Price;
            mc.SaveChanges();
            this.s = mc.SKUs.First(x => x.ID == s.ID);
            Init(this.s);
            mc.SKUs.First(x => x.ID == s.ID).TotalPrice = 0;

            mc.SaveChanges();
        }

        public PrintPrice(SKU s)
        {
            InitializeComponent();
            this.s = s;
            Init(s);
        }
        void Init(SKU s)
        {
            MContext mc = new MContext();
            List<SkuAnot> lsa = mc.SAs.Where(x => x.ID == s.ID).ToList();
            lValue.Add(lsa[0].Val1);
            lValue.Add(lsa[0].Val2);
            lValue.Add(lsa[0].Val3);
            lValue.Add(lsa[0].Val4);
            lValue.Add(lsa[0].Val5);
            lValue.Add(lsa[0].Val6);
            lValue.Add(lsa[0].Val7);
            lValue.Add(lsa[0].Val8);
            lValue.Add(lsa[0].Val9);
            lValue.Add(lsa[0].Val10);
            lValue.Add(lsa[0].Val11);
            lValue.Add(lsa[0].Val12);

            List<AnotSubCat> lac = mc.ASCs.Where(x => x.ID == s.ID_Subcategory).ToList();
            sa = lac[0];
            lHarac.Add(lac[0].Har1);
            lHarac.Add(lac[0].Har2);
            lHarac.Add(lac[0].Har3);
            lHarac.Add(lac[0].Har4);
            lHarac.Add(lac[0].Har5);
            lHarac.Add(lac[0].Har6);
            lHarac.Add(lac[0].Har7);
            lHarac.Add(lac[0].Har8);
            lHarac.Add(lac[0].Har9);
            lHarac.Add(lac[0].Har10);
            lHarac.Add(lac[0].Har11);
            lHarac.Add(lac[0].Har12);

            Subcategory sub = mc.Subcategories.First(x => x.ID == s.ID_Subcategory);
            PriceFormat pf = mc.PriceFormat.First(x => x.ID == sub.ID_PriceFormat);

            if (pf.ID != 3)
            {
                SetTitle();
                SetTitleAnatation();
                SetHarac();
                SetPrice();
            }
            else
            {
                SetTitle(true);
                SetTitleAnatation(true);
                int size=SetHarac(true);
                SetPrice(true, size);
            }
        }

        void SetTitle()
        {
            System.Drawing.Image temp = System.Drawing.Image.FromFile(@"Logo.jpg");
            Bitmap source = new Bitmap(temp, 80, 10);
            System.Windows.Media.Imaging.BitmapSource b =
               System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(source.GetHbitmap(),
                                                                           IntPtr.Zero,
                                                                           Int32Rect.Empty,
                                                                           BitmapSizeOptions.FromEmptyOptions());
            System.Windows.Shapes.Rectangle r = new System.Windows.Shapes.Rectangle();
            r.Fill = new ImageBrush(b);
            r.Width = 80;
            r.Height = 10;
            MyCanvas.Children.Add(r);
            Canvas.SetLeft(r, 110);
            Canvas.SetTop(r, 10);

            
            SetLabel("ФОП КОНИВЧЕНКО А.В.", "Calibri", false, 7, 60, 20);
            SetLabel("ул. Марии Лысыченко, 5", "Calibri", false, 6, 63, 27);
            SetLabel(string.Format("ID: {0}", s.ID), "Calibri", true, 7, 10, 10);
        }

        void SetTitle(bool f)
        {
            System.Drawing.Image temp = System.Drawing.Image.FromFile(@"Logo.jpg");
            Bitmap source = new Bitmap(temp, 60, 8);
            System.Windows.Media.Imaging.BitmapSource b =
               System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(source.GetHbitmap(),
                                                                           IntPtr.Zero,
                                                                           Int32Rect.Empty,
                                                                           BitmapSizeOptions.FromEmptyOptions());
            System.Windows.Shapes.Rectangle r = new System.Windows.Shapes.Rectangle();
            r.Fill = new ImageBrush(b);
            r.Width = 60;
            r.Height = 8;
            MyCanvas.Children.Add(r);
            Canvas.SetLeft(r, 90);
            Canvas.SetTop(r, 5);


            SetLabel("ФОП КОНИВЧЕНКО А.В.", "Calibri", false, 5, 30, 0);
            SetLabel("ул. Марии Лысыченко, 5", "Calibri", false, 4, 30, 5);
            SetLabel(string.Format("ID: {0}", s.ID), "Calibri", true, 5, 5, 0);
        }

        void SetTitleAnatation()
        {
            MContext mc =new MContext();
            List<Subcategory> ls=mc.Subcategories.Where(x=>x.ID==s.ID_Subcategory).ToList();
            List<Brend> lb = mc.Brends.Where(x => x.ID == s.ID_Brend).ToList();
            int kaf_sub = (int)(61 + (17 - ls[0].Name.Length)*1.75);
            int kaf_brend = (int)(63 + (6 - lb[0].Name.Length)*4.5);
            int kaf_model=(int)(70+(6-s.Name.Length)*3.5);
            SetLabel(ls[0].Name, "Calibri", false, 7, kaf_sub, 40);
            SetLabel(lb[0].Name, "Arial Black", true, 12, kaf_brend, 48);
            SetLabel(s.Name, "Calibri", true, 12, kaf_model, 60);
        }


        void SetTitleAnatation(bool f)
        {
            MContext mc = new MContext();
            List<Subcategory> ls = mc.Subcategories.Where(x => x.ID == s.ID_Subcategory).ToList();
            List<Brend> lb = mc.Brends.Where(x => x.ID == s.ID_Brend).ToList();
            SetLabel(ls[0].Name, "Calibri", false, 7, 5, 10);
            SetLabel(lb[0].Name, "Arial Black", true, 7, 45, 10);
            SetLabel(s.Name, "Calibri", true, 7, 90, 10);
        }

        void SetHarac()
        {
            int size = 80;
            for (int i = 0; i < lHarac.Count; i++)
            {
                if (lHarac[i] != string.Empty && lHarac[i] != null)
                {
                    if ((i + 1) % 2 == 0)
                        SetRec(size + 4);
                    SetLabel(lHarac[i], "Calibri", false, 6, 10, size);
                    SetLabelRaihgt(lValue[i], "Calibri", false, 6, 10, size);
                    size += 10;
                }
            }
        }

        int SetHarac(bool f)
        {
            int size = 20;
            for (int i = 0; i < lHarac.Count; i++)
            {
                if (lHarac[i] != string.Empty && lHarac[i] != null)
                {
                    if ((i + 1) % 2 == 0)
                        SetRec(size + 4,true);
                    SetLabel(lHarac[i], "Calibri", false, 4, 5, size);
                    SetLabelRaihgt(lValue[i], "Calibri", false, 4, 80, size);
                    size += 5;
                }
            }
            return size;
        }

        void SetPrice()
        {
            System.Windows.Shapes.Rectangle rc = new System.Windows.Shapes.Rectangle();
            rc.Fill = new SolidColorBrush(Colors.GreenYellow);
            rc.Width = 100;
            rc.Height = 10;
            MyCanvas.Children.Add(rc);
            Canvas.SetRight(rc, 10);
            Canvas.SetTop(rc, 190);

            Label lc = new Label();
            if (s.TotalPrice / s.InputPrice < 1.25)
            {
                lc.Content = "ИНТЕРНЕТ ЦЕНА";
                lc.FontSize = 9;
                lc.FontWeight = FontWeights.Bold;
                lc.Foreground = new SolidColorBrush(Colors.Black);
                MyCanvas.Children.Add(lc);
                Canvas.SetRight(lc, 16);
                Canvas.SetTop(lc, 184);

            }
            else
            {
                lc.Content = "ЦЕНА";
                lc.FontSize = 9;
                lc.FontWeight = FontWeights.Bold;
                lc.Foreground = new SolidColorBrush(Colors.Black);
                MyCanvas.Children.Add(lc);
                Canvas.SetRight(lc, 40);
                Canvas.SetTop(lc, 184);
            }

            if (s.TotalPromo > 0)
            {
                System.Windows.Shapes.Rectangle rcpromo = new System.Windows.Shapes.Rectangle();
                rcpromo.Fill = new SolidColorBrush(Colors.Blue);
                rcpromo.Width = 80;
                rcpromo.Height = 10;
                MyCanvas.Children.Add(rcpromo);
                Canvas.SetRight(rcpromo, 110);
                Canvas.SetTop(rcpromo, 190);
                Label lcprom = new Label();
                lcprom.Content = "ПОДАРОК";
                lcprom.FontSize = 9;
                lcprom.FontWeight = FontWeights.Bold;
                lcprom.Foreground = new SolidColorBrush(Colors.Black);
                MyCanvas.Children.Add(lcprom);
                Canvas.SetRight(lcprom, 116);
                Canvas.SetTop(lcprom, 184);

                System.Windows.Shapes.Rectangle rprom2 = new System.Windows.Shapes.Rectangle();
                rprom2.Fill = new SolidColorBrush(Colors.Yellow);
                rprom2.Width = 80;
                rprom2.Height = 30;
                MyCanvas.Children.Add(rprom2);
                Canvas.SetRight(rprom2, 110);
                Canvas.SetTop(rprom2, 200);

                Label lprom2 = new Label();
                lprom2.Content = (int)(s.TotalPromo*s.TotalPrice/100);
                lprom2.FontSize = 33;
                lprom2.FontWeight = FontWeights.Bold;
                lprom2.Foreground = new SolidColorBrush(Colors.Black);
                MyCanvas.Children.Add(lprom2);

                Canvas.SetRight(lprom2, 125);
                Canvas.SetTop(lprom2, 186);


            }



            System.Windows.Shapes.Rectangle r = new System.Windows.Shapes.Rectangle();
            r.Fill = new SolidColorBrush(Colors.Red);
            r.Width = 100;
            r.Height = 30;
            MyCanvas.Children.Add(r);
            Canvas.SetRight(r, 10);
            Canvas.SetTop(r, 200);

            Label l = new Label();
            l.Content = s.TotalPrice;
            l.FontSize = 33;
            l.FontWeight = FontWeights.Bold;
            l.Foreground = new SolidColorBrush(Colors.White);
            MyCanvas.Children.Add(l);
            if (s.TotalPrice < 1000)
            {
                Canvas.SetRight(l, 25);
                Canvas.SetTop(l, 186);
            }
            else
            {
                Canvas.SetRight(l, 10);
                Canvas.SetTop(l, 186);
            }

            if(s.TotalPrice / s.InputPrice < 1.15)
            {
                System.Windows.Shapes.Rectangle rc3 = new System.Windows.Shapes.Rectangle();
                rc3.Fill = new SolidColorBrush(Colors.Orange);
                rc3.Width = 100;
                rc3.Height = 10;
                MyCanvas.Children.Add(rc3);
                Canvas.SetRight(rc3, 10);
                Canvas.SetTop(rc3, 230);

                Label lc3 = new Label();
                lc3.Content = string.Format("Терминал {0}",Math.Round(s.InputPrice*1.16,0));
                lc3.FontSize = 9;
                lc3.FontWeight = FontWeights.Bold;
                lc3.Foreground = new SolidColorBrush(Colors.Black);
                MyCanvas.Children.Add(lc3);
                Canvas.SetRight(lc3, 16);
                Canvas.SetTop(lc3, 224);
            }
        }

        void SetPrice(bool f, int size)
        {
            System.Windows.Shapes.Rectangle rc = new System.Windows.Shapes.Rectangle();
            rc.Fill = new SolidColorBrush(Colors.GreenYellow);
            rc.Width = 30;
            rc.Height = 5;
            MyCanvas.Children.Add(rc);
            Canvas.SetRight(rc, 50);
            Canvas.SetTop(rc, size-15);

            Label lc = new Label();
            if (s.TotalPrice / s.InputPrice < 1.25)
            {
                lc.Content = "ИНТЕРНЕТ ЦЕНА";
                lc.FontSize = 3;
                lc.FontWeight = FontWeights.Bold;
                lc.Foreground = new SolidColorBrush(Colors.Black);
                MyCanvas.Children.Add(lc);
                Canvas.SetRight(lc, 48);
                Canvas.SetTop(lc, size-20);
            }
            else
            {
                lc.Content = "ЦЕНА";
                lc.FontSize = 3;
                lc.FontWeight = FontWeights.Bold;
                lc.Foreground = new SolidColorBrush(Colors.Black);
                MyCanvas.Children.Add(lc);
                Canvas.SetRight(lc, 55);
                Canvas.SetTop(lc, size - 20);
            }

            if (s.TotalPromo > 0)
            {
                System.Windows.Shapes.Rectangle rcpromo = new System.Windows.Shapes.Rectangle();
                rcpromo.Fill = new SolidColorBrush(Colors.LightBlue);
                rcpromo.Width = 30;
                rcpromo.Height = 5;
                MyCanvas.Children.Add(rcpromo);
                Canvas.SetRight(rcpromo, 50);
                Canvas.SetTop(rcpromo, size-35);
                Label lcprom = new Label();
                lcprom.Content = "ПОДАРОК";
                lcprom.FontSize = 3;
                lcprom.FontWeight = FontWeights.Bold;
                lcprom.Foreground = new SolidColorBrush(Colors.Black);
                MyCanvas.Children.Add(lcprom);
                Canvas.SetRight(lcprom, 52);
                Canvas.SetTop(lcprom, size-40);

                System.Windows.Shapes.Rectangle rprom2 = new System.Windows.Shapes.Rectangle();
                rprom2.Fill = new SolidColorBrush(Colors.Yellow);
                rprom2.Width = 30;
                rprom2.Height = 15;
                MyCanvas.Children.Add(rprom2);
                Canvas.SetRight(rprom2, 50);
                Canvas.SetTop(rprom2, size-30);

                Label lprom2 = new Label();
                lprom2.Content = (int)(s.TotalPromo * s.TotalPrice / 100);
                lprom2.FontSize = 15;
                lprom2.FontWeight = FontWeights.Bold;
                lprom2.Foreground = new SolidColorBrush(Colors.Black);
                MyCanvas.Children.Add(lprom2);

                Canvas.SetRight(lprom2, 48);
                Canvas.SetTop(lprom2, size-38);


            }



            System.Windows.Shapes.Rectangle r = new System.Windows.Shapes.Rectangle();
            r.Fill = new SolidColorBrush(Colors.Red);
            r.Width = 30;
            r.Height = 15;
            MyCanvas.Children.Add(r);
            Canvas.SetRight(r, 50);
            Canvas.SetTop(r, size - 10);

            Label l = new Label();
            l.Content = s.TotalPrice;
            l.FontSize = 15;
            l.FontWeight = FontWeights.Bold;
            l.Foreground = new SolidColorBrush(Colors.White);
            MyCanvas.Children.Add(l);
            if (s.TotalPrice < 1000)
            {
                Canvas.SetRight(l, 48);
                Canvas.SetTop(l, size - 19);
            }
            else
            {
                l.FontSize = 12;
                Canvas.SetRight(l, 45);
                Canvas.SetTop(l, size - 18);
            }

            if (s.TotalPrice / s.InputPrice < 1.15)
            {
                System.Windows.Shapes.Rectangle rc3 = new System.Windows.Shapes.Rectangle();
                rc3.Fill = new SolidColorBrush(Colors.Orange);
                rc3.Width = 30;
                rc3.Height = 5;
                MyCanvas.Children.Add(rc3);
                Canvas.SetRight(rc3, 50);
                Canvas.SetTop(rc3, size+5);

                Label lc3 = new Label();
                lc3.Content = string.Format("Терминал {0}", Math.Round(s.InputPrice * 1.16, 0));
                lc3.FontSize = 3;
                lc3.FontWeight = FontWeights.Bold;
                lc3.Foreground = new SolidColorBrush(Colors.Black);
                MyCanvas.Children.Add(lc3);
                Canvas.SetRight(lc3, 50);
                Canvas.SetTop(lc3, size);
            }
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                // Скрыть Grid
                grid.Visibility = Visibility.Hidden;
                MContext mc = new MContext();
                Subcategory sub = mc.Subcategories.First(x => x.ID == s.ID_Subcategory);
                PriceFormat pf = mc.PriceFormat.First(x => x.ID == sub.ID_PriceFormat);
                // Увеличить размер в 5 раз
                MyCanvas.LayoutTransform = new ScaleTransform(pf.TransX, pf.TransY);
                // Определить поля
                int pageMargin = pf.Margin;

                // Получить размер страницы
                System.Windows.Size pageSize = new System.Windows.Size(pf.X, pf.Y);
                // Инициировать установку размера элемента
                MyCanvas.Measure(pageSize);
                MyCanvas.Arrange(new Rect(pageMargin, pageMargin, pageSize.Width, pageSize.Height));

                // Напечатать элемент
                printDialog.PrintVisual(MyCanvas, "Распечатываем элемент Canvas");

                // Удалить трансформацию и снова сделать элемент видимым
                MyCanvas.LayoutTransform = null;
                grid.Visibility = Visibility.Visible;
                if (s.TotalPrice != 0)
                {
                    SKU stmp = mc.SKUs.First(x => x.ID == s.ID);
                    stmp.Price = s.TotalPrice;
                    stmp.TotalPrice = 0;
                    stmp.Promo = s.TotalPromo;
                    stmp.ChangePrice = false;
                    mc.SaveChanges();
                }

            }
            this.Close();

        }


        void SetLabel(string Content, string FontFamily, bool Bold, int FontSize, int Set, int SetTop)
        {
            Label lBrand = new Label();
            lBrand.Content = Content;
            lBrand.FontFamily = new System.Windows.Media.FontFamily(FontFamily);
            if (Bold)
                lBrand.FontWeight = FontWeights.Bold;
            lBrand.FontSize = FontSize;
            MyCanvas.Children.Add(lBrand);
            Canvas.SetLeft(lBrand, Set);
            Canvas.SetTop(lBrand, SetTop);
        }
        void SetLabelRaihgt(string Content, string FontFamily, bool Bold, int FontSize, int Set, int SetTop)
        {
            Label lBrand = new Label();
            lBrand.Content = Content;
            lBrand.FontFamily = new System.Windows.Media.FontFamily(FontFamily);
            if (Bold)
                lBrand.FontWeight = FontWeights.Bold;
            lBrand.FontSize = FontSize;
            MyCanvas.Children.Add(lBrand);
            Canvas.SetRight(lBrand, Set);
            Canvas.SetTop(lBrand, SetTop);
        }
        void SetRec(int size)
        {
            System.Windows.Shapes.Rectangle rc = new System.Windows.Shapes.Rectangle();
            rc.Fill = new SolidColorBrush(Colors.LightGray);
            rc.Width = 180;
            rc.Height = 10;
            MyCanvas.Children.Add(rc);
            Canvas.SetRight(rc, 10);
            Canvas.SetTop(rc, size);
        }

        void SetRec(int size, bool f)
        {
            System.Windows.Shapes.Rectangle rc = new System.Windows.Shapes.Rectangle();
            rc.Fill = new SolidColorBrush(Colors.LightGray);
            rc.Width = 112;
            rc.Height = 5;
            MyCanvas.Children.Add(rc);
            Canvas.SetRight(rc, 84);
            Canvas.SetTop(rc, size);

        }
    }
}
