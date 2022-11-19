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
        public bool autoMode { get; set; }
        public DateTime currentTime { get; set; }


         public void GetChipNumber()
         {
            Console.WriteLine("Номер чипа: ");
         } 

         async Task GetCurrentTime()
        {
           
            while (true)
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
