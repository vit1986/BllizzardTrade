using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metelica
{
    class MyTableGrid
    {
        public int ID { get; set; }
        public string Подкатегория { get; set; }
        public string Бренд { get; set; }
        public string Модель { get; set; }
        public double Вход { get; set; }
        public int Розница { get; set; }
        public int Промо { get; set; }
        public int Цена_конкурента { get; set; }
        public double Новая_наценка { get; set; }
        public double Изменение { get; set; }
        public string Комментарий { get; set; }
    }
}
