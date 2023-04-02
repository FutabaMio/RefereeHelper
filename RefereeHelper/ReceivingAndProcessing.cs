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
        public TimeOnly time;
        public int secondOfDifference = 5;
        public TimeSpan timeOfDifference;
        UdpReceiveResult result;
        UdpClient client;
        byte[]? datagram;
        string? received;

        /// <summary>
        /// Конструктор класса. Создаёт UDPClient с портом и обновляет время разницы.
        /// </summary>
        /// <param name="port"> - порт, с которым создаётся UDPClient</param>
        public UDPReceive(int port)
        {
            client = new UdpClient(port);
            timeOfDifference = new(0, 0, secondOfDifference);
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
        public void RefrechPlace(int idOfTiming)
        {
            //var t = DataService.Get(idOfTiming).Result;
            var t = dbContext.Set<Timing>().First(x => x.Id == idOfTiming);
            var ts = dbContext.Set<Timing>().Select(x => new Timing
            {
                Start = new Start
                {
                    Partisipation = new Partisipation
                    {
                        Group = new Group
                        {
                            Distance = x.Start.Partisipation.Group.Distance
                        }
                    }
                }
            }).ToList().OrderBy(x => x.TimeFromStart);
            //var ts = DataService.GetAll(x => x.Start.Partisipation.Group.Distance == t.Start.Partisipation.Group.Distance
            //                              && x.Start.Partisipation.Group == t.Start.Partisipation.Group).Result.
            //                                 OrderBy(x => x.TimeFromStart);
            int i = 0;
            foreach (var k in ts)
            {
                k.Place = i + 1; i++;
                DataService.Update(k.Id, k);
            }
        }

        /// <summary>
        /// Обновляет все абсолютные позиции
        /// </summary>
        /// <param name="idOfTiming"> - идентификационный номер Timing, относительно которого будут обновляться абсолютные позиции</param>
        public void RefrechAbsolutePlace(int idOfTiming)
        {
            var t = dbContext.Set<Timing>().First(x => x.Id == idOfTiming);
            //var t = DataService.Get(idOfTiming).Result;

            var ts = dbContext.Set<Timing>().Select(x => new Timing
            {
                Start = new Start
                {
                    Partisipation = new Partisipation
                    {
                        Group = new Group
                        {
                            Distance = x.Start.Partisipation.Group.Distance
                        }
                    }
                }
            }).ToList().OrderBy(x => x.TimeFromStart);

            //var ts = DataService.GetAll(x => x.Start.Partisipation.Group.Distance == t.Start.Partisipation.Group.Distance)
            //                                       .Result.OrderBy(x => x.TimeFromStart);
            int i = 0;
            foreach (var k in ts)
            {
                k.Place = i + 1; i++;
                DataService.Update(k.Id, k);
            }
        }

        /// <summary>
        /// Определяет, финишировал ли или нет.
        /// </summary>
        /// <param name="idOfTiming"> - идентификационный номер Timing, для которого будет определяться финишировал ли спортсмен</param>
        /// <returns>True в случае, если финишировал, False в ином случае</returns>
        public bool GetIsFinish(int idOfTiming)
        {
            var t = dbContext.Set<Timing>().Select(x => new Timing
            {
                Id = x.Id,
                Start = new Start
                {
                    Partisipation = new Partisipation
                    {
                        Group = new Group
                        {
                            Distance = new Distance
                            {
                                Name = x.Start.Partisipation.Group.Distance.Name
                            }
                        }
                    }
                }
            }).ToList().First(x => x.Id == idOfTiming);

            //var t = DataService.Get(idOfTiming).Result;
            if (t.Start?.Partisipation.Group?.Distance.Circles < t.Circle)
                return false;
            else
                return true;
        }
        /// <summary>
        /// Определяет время круга для определённого спортсмена.
        /// </summary>
        /// <param name="idOfTiming"> - идентификационный номер Timing, по которому будет определяться время круга.</param>
        /// <returns>Время круга.</returns>
        public TimeOnly GetTimeOfLap(int idOfTiming)
        {
            var t = dbContext.Set<Timing>().Select(x => new Timing
            {
                Id = x.Id,
                TimeFromStart = x.TimeFromStart,
                Start = new Start
                {
                    Id = x.Start.Id
                }
            }).First(x => x.Id == idOfTiming);
            if (dbContext.Set<Timing>().Select(x => new Timing
            {
                Id = x.Id,
                Start = x.Start
            }).Where(x => x.Start == t.Start).ToList().Count() == 0)
            //if (DataService.GetAll(x => x.Start == t.Start).Result.Count() == 0)
                return t.TimeFromStart.Value;
            return TimeOnly.FromTimeSpan(t.TimeFromStart.Value - dbContext.Set<Timing>().Where(x => x.Start == t.Start).ToList().Last(x => x.Id != t.Id).TimeFromStart.Value);
        }
        /// <summary>
        /// Определяет количество кругов для определённого человека.
        /// </summary>
        /// <param name="idOfTiming"> - идентификационный номер Timing, по которому будет определяться количество кругов для определённого спортсмена.</param>
        /// <returns>количество кругов.</returns>
        public int GetOfLapsForHim(int idOfTiming)
        {
            var t = dbContext.Set<Timing>().First(x => x.Id == idOfTiming);
            return dbContext.Set<Timing>().Where(x => x.Start.Number == t.Start.Number).Count() + 1;
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
