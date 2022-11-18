using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefereeHelper.Models
{
    public class Distance
    {
        public Distance _distance;

       

        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Length { get; set; }
        public decimal Height { get; set; }
        public TimeOnly startTime { get; set; }

        public Distance(Distance distance)
        {
            _distance=distance;
        }

        public void AddDistanceManually(string name, decimal length, decimal height, TimeOnly startT)
        {
            _distance.Name = name;
            _distance.Length = length;
            _distance.Height = height;
            _distance.startTime = startT;
        }

    }
}
