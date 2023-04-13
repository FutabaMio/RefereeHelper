using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using RefereeHelper.EntityFramework;
using RefereeHelper.EntityFramework.Services;
using RefereeHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RefereeHelper
{
    /// <summary>
    /// Класс для получения необходимых данных
    /// </summary>
    public class UDPReceive
    {
        private static UDPReceive instance;
        public TimeOnly time;
        public int secondOfDifference = 5;
        public TimeSpan timeOfDifference;
        UdpReceiveResult result;
        byte[]? datagram;
        string? received;
        public UdpClient client;


        /// <summary>
        /// Конструктор класса. Создаёт UDPClient с портом и обновляет время разницы.
        /// </summary>
        private UDPReceive()
        {
            timeOfDifference = new(0, 0, secondOfDifference);
            client = new UdpClient(27069);
        }
        public static UDPReceive GetUdpClient()
        {
            if (instance == null)
                instance = new UDPReceive();
        return instance;
        }

        /// <summary>
        /// Получение номера тега
        /// </summary>
        public async Task<string> Receive()
        {
            result = await client.ReceiveAsync();
            datagram = result.Buffer;
            received = Encoding.UTF8.GetString(datagram);
            time = TimeOnly.FromDateTime(DateTime.Now);
            received = received.Substring(received.IndexOf("Tag:")+4);
            received = received.Substring(0, received.IndexOf(" "));
            return received;
        }
        /// <summary>
        /// Закрывает UDPClient
        /// </summary>
        public void Close()
        {
            client.Close();
        }
    }
    public class Processing 
    {
        GenericDataService<Timing> DataService = new(new RefereeHelperDbContextFactory());
        public DbContext dbContext;

        public Processing()
        {
            dbContext = new RefereeHelperDbContextFactory().CreateDbContext();
        }
        /// <summary>
        /// Обновляет все позиции
        /// </summary>
        /// <param name="idOfTiming"> - идентификационный номер Timing, относительно которого будут обновляться позиции</param>
        public void RefrechPlace(DbContext dbContext, Timing t)
        {
            var d = t;
            d.Start.Partisipation = dbContext.Set<Partisipation>().First(x => x.Id == t.Start.PartisipationId);
            d.Start.Partisipation.Group = dbContext.Set<Group>().First(x => x.Id == d.Start.Partisipation.GroupId);
            d.Start.Partisipation.Group.Distance = dbContext.Set<Distance>().First(x => x.Id == d.Start.Partisipation.Group.DistanceId);
            var ts = dbContext.Set<Timing>().Include(x => x.Start).ThenInclude(c => c.Partisipation).ThenInclude(v => v.Group)
                .Where(m => m.Start.Partisipation.Group.Distance.Id == d.Start.Partisipation.Group.Distance.Id && m.Circle == d.Circle)
                .OrderByDescending(n => n.TimeFromStart).ToList();
            int i = 0;
            foreach (var k in ts)
            {
                k.Place = i + 1; i++;
                dbContext.Update(k);
            }
            dbContext.SaveChanges();
        }

        /// <summary>
        /// Обновляет все абсолютные позиции
        /// </summary>
        /// <param name="idOfTiming"> - идентификационный номер Timing, относительно которого будут обновляться абсолютные позиции</param>
        public void RefrechAbsolutePlace(DbContext dbContext, Timing t)
        {
            var d = t;
            d.Start.Partisipation = dbContext.Set<Partisipation>().First(x => x.Id == t.Start.PartisipationId);
            d.Start.Partisipation.Group = dbContext.Set<Group>().First(x => x.Id == d.Start.Partisipation.GroupId);
            d.Start.Partisipation.Group.Distance = dbContext.Set<Distance>().First(x => x.Id == d.Start.Partisipation.Group.DistanceId);
            var ts = dbContext.Set<Timing>().Include(x => x.Start).ThenInclude(c => c.Partisipation).ThenInclude(v => v.Group).Where(m => m.Start.Partisipation.Group.DistanceId == d.Start.Partisipation.Group.DistanceId).OrderByDescending(n => n.TimeFromStart).ToList();
            int i = 0;
            foreach (var k in ts)
            {
                k.PlaceAbsolute = i + 1; i++;
                dbContext.Update(k);
            }
            dbContext.SaveChanges();
        }

        /// <summary>
        /// Определяет, финишировал ли или нет.
        /// </summary>
        /// <param name="idOfTiming"> - идентификационный номер Timing, для которого будет определяться финишировал ли спортсмен</param>
        /// <returns>True в случае, если финишировал, False в ином случае</returns>
        public bool GetIsFinish(int idOfTiming)
        {
            var ts = dbContext.Set<Timing>().Include(x => x.Start).ThenInclude(x => x.Partisipation).ThenInclude(x => x.Group).ThenInclude(x => x.Distance).ToList();
            //    .Select(x => new Timing
            //{
            //    Id = x.Id,
            //    Circle = x.Circle,
            //    Start = new Start
            //    {
            //        Partisipation = new Partisipation
            //        {
            //            Group = new Group
            //            {
            //                Distance = new Distance
            //                {
            //                    Name = x.Start.Partisipation.Group.Distance.Name,
            //                    Circles = x.Start.Partisipation.Group.Distance.Circles
            //                }
            //            }
            //        }
            //    }
            //}).ToList();
            var t = ts.First(x => x.Id == idOfTiming);

            //var t = DataService.Get(idOfTiming).Result;
            if (t.Start?.Partisipation.Group?.Distance.Circles-1 < t.Circle)
            {
                if(t.Start?.Partisipation.Group?.Distance.Circles == t.Circle)
                {
                    return true;
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Определяет время круга для определённого спортсмена.
        /// </summary>
        /// <param name="idOfTiming"> - идентификационный номер Timing, по которому будет определяться время круга.</param>
        /// <returns>Время круга.</returns>
        public TimeOnly GetTimeOfLap(DbContext dbContext, Timing timing)
        {
            var t = dbContext.Set<Timing>().Include(x => x.Start).First(z => z.Id == timing.Id);
            if (dbContext.Set<Timing>().Select(x => new Timing
            {
                Id = x.Id,
                Start = x.Start
            }).Where(x => x.Start.Id == t.Start.Id).ToList().Count() == 0)
            {
                return t.TimeFromStart.Value;
            }
            else
            {
                var k = dbContext.Set<Timing>().Where(x => x.StartId == t.StartId).ToList().LastOrDefault(z => z.Id != t.Id);
                return TimeOnly.FromTimeSpan(k.TimeFromStart.Value - t.TimeFromStart.Value);
            }
            //if (DataService.GetAll(x => x.Start == t.Start).Result.Count() == 0)
        }
        /// <summary>
        /// Определяет количество кругов для определённого человека.
        /// </summary>
        /// <param name="idOfTiming"> - идентификационный номер Timing, по которому будет определяться количество кругов для определённого спортсмена.</param>
        /// <returns>количество кругов.</returns>
        public int GetOfLapsForHim(DbContext dbContext, Timing t)
        {
            var timingEntity = dbContext.Set<Timing>().Include(z => z.Start).First(x => x.Id == t.Id);
            return dbContext.Set<Timing>().Where(x => x.Start.Number == timingEntity.Start.Number).Count();
        }
        /// <summary>
        /// Определяет количество людей на определённом круге
        /// </summary>
        /// <param name="idOfTiming"> - ификационный номер Timing, по которому будет определяться количество людей на определённом кругу.</param>
        /// <returns>Кол-во людей на данном круге</returns>
        public int GetCountPeopleOnThisLap(int idOfTiming)
        {
            var t = dbContext.Set<Timing>().First(x => x.Id == idOfTiming);
            return dbContext.Set<Timing>().Where(x => x.Start.Partisipation.Competition == t.Start.Partisipation.Competition
                                        && x.Start.Partisipation.Group == t.Start.Partisipation.Group
                                        && x.Circle == t.Circle).Count();
        }
    }
}
