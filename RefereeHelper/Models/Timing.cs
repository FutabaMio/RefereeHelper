using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefereeHelper.Models
{
    public class Timing
    {
        public int Id { get; set; }
        public bool autoMode { get; set; } = false;            //почитать об инициализации переменной после указания её свойств
        public DateTime currentTime { get; set; }              //надо распарсить только время, поискать как


         public void GetChipNumber()                            //интеграция функции Сусла, надо через доп. проект и вызов из проекта (?)
         {
            autoMode = true;
            //Console.WriteLine("Номер чипа: ");
         } 

         async Task GetCurrentTime()                            //каждый тик (тик-проверка-тик) получается текущая дата, оттуда берётся время
        {                                                       //если включен (true) автомод, то считываем чип, иначе снова получаем время
                                                                //разобраться поподробнее, мб получать каждый тик не получится из-за проверок
            while (true)                                        //и загруженности функции Сусла
            { 
                currentTime = DateTime.Now;
                 if (autoMode)
            {
                await Task.Run(()=> GetChipNumber());
            }
            else
            {
                continue;
            }
            }
        }
    }
}
