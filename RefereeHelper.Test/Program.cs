using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using RefereeHelper;
using RefereeHelper.EntityFramework;
using RefereeHelper.EntityFramework.Services;
using RefereeHelper.Models;
using System.Net;
using System.Net.Sockets;
using System.Text;

int countOfStartingPeople = 0;
string[] codes = { "1- Первичная обработка данных", "2- Сбор", "3- Заполнить БД", "4- Проверить БД", "5- Тайминг", "6- Удалить Тайминг", "7- help", "close" };
decimal CountOfLapsForHim = 0;
int CountOfFinishingPeople = 0;
List<string> idsOfFinishingPeople = new List<string>();
Processing p = new();
TimeSpan timeOfDifference = new TimeSpan(0, 0, 0);
while (true)
{
    Console.WriteLine("Введите запрос операции:");
    string code = Console.ReadLine();
    Console.WriteLine($"\n\n\t\t{code}\n");
    switch (code)
    {
        case "1":
            Demo();
            break;
        case "2":
            Second();
            break;
        case "3":
            Third();
            break;
        case "4":
            CheckDB();
            break;
        case "5":
            ViewTiming(p.dbContext);
            break;
        case "6":
            ClearTiming(p.dbContext);
            break;
        case "7":
            GiveInfo();
            break;
        case "close":
            Environment.Exit(228);
            break;            
        default:
            GiveInfo();
            Console.WriteLine("Try more");
            break;
    }
}
void ViewTiming(DbContext dbContext)
{
    Console.WriteLine("Имя\tТекущее время\tВремя со старта\tКруг\tПозиция\tПозиция по кругу Время круга\tФинишировал");
    var ts = dbContext.Set<Timing>().Select(x => new Timing
    {
        IsFinish = x.IsFinish,
        TimeNow = x.TimeNow,
        TimeFromStart = x.TimeFromStart,
        Circle = x.Circle,
        PlaceAbsolute = x.PlaceAbsolute,
        Place = x.Place,
        CircleTime = x.CircleTime,
        Start = new Start
        {
            Partisipation = new Partisipation
            {
                Member = new Member
                {
                    Name = x.Start.Partisipation.Member.Name,
                },
                Group = new Group
                {
                    Distance = new Distance
                    {
                        Circles = x.Start.Partisipation.Group.Distance.Circles
                    }
                }
            },
            Number = x.Start.Number,
            Chip = x.Start.Chip,
            StartTime = x.Start.StartTime
        }
    }).ToList();
    foreach (var s in ts)
    {
        Console.WriteLine($"{s?.Start?.Partisipation?.Member?.Name}" +
                        $"\t{s?.TimeNow?.Hour}:{s?.TimeNow?.Minute}:{s?.TimeNow?.Second}.{s?.TimeNow?.Millisecond}" +
                        $"\t{s?.TimeFromStart?.Hour}:{s?.TimeFromStart?.Minute}:{s?.TimeFromStart?.Second}.{s?.TimeFromStart?.Millisecond}" +
                        $"\t{s?.Circle}/{s?.Start?.Partisipation?.Group?.Distance.Circles}" +
                        $"\t{s?.PlaceAbsolute}" +
                        $"\t{s?.Place}" +
                        $"\t\t {s?.CircleTime?.Hour}:{s?.CircleTime?.Minute}:{s?.CircleTime?.Second}.{s?.CircleTime?.Millisecond}" +
                        $"\t{s.IsFinish}");
    }
}
void First()
{
    bool flag = true;
    countOfStartingPeople = 0;
    while (flag)
    {        
        Console.WriteLine("Введите команду (<<РВВ>> - разница во времени, по стандарту 5 секунд; <<демо>> для демонстрации):");
        string code = Console.ReadLine();

        switch (code)
        {
            case "РВВ":
                RVV(Console.ReadLine());
                break;
            case "демо":
                Demo();
                break;
            case "exit":
                flag = false; break;
            default:break;
        }       
        
    }
}
TimeSpan RVV(string k)
{
    try
    { 
        return new(0, 0, Int32.Parse(k));
    }
    catch (Exception e)
    {
        Console.WriteLine($"\n\n-----------------------------------------------------------------------------{e.Message}-----------------------------------------------------------------------------\n");
    }
    return new(0, 0, 5);
}
void Demo()
{
    Process("Xi", p.dbContext);
    Thread.Sleep(10);
    Process("P4BK", p.dbContext);
    Thread.Sleep(5000);
    Process("1", p.dbContext);
    Thread.Sleep(10);
    Process("Xi", p.dbContext);
    Thread.Sleep(10);
    Process("P4BK", p.dbContext);
    Thread.Sleep(10);
    Process("1", p.dbContext);

}
void CheckDB()
{
    var dbContext = new RefereeHelperDbContextFactory().CreateDbContext();
    
    var st = dbContext.Set<Start>().Select(x => new Start
    {
        Partisipation = new Partisipation
        {
            Competition = x.Partisipation.Competition,
            Group = new Group
            {
                Name = x.Partisipation.Group.Name,
                Distance = x.Partisipation.Group.Distance
            },
            Member = new Member
            {
                Name = x.Partisipation.Member.Name,
                Discharge = new Discharge
                {
                    Name = x.Partisipation.Member.Discharge.Name
                },
                Club = new Club
                {
                    Name = x.Partisipation.Member.Club.Name,
                    Region = new Region
                    {
                        Name = x.Partisipation.Member.Club.Region.Name
                    }
                }
            }
        },
        Number = x.Number,
        Chip = x.Chip,
        StartTime = x.StartTime
    }).ToList();

    var tts = dbContext.Set<Start>();
    foreach(var t in tts)
    {
        Console.WriteLine(t.TeamId);
    }

    Console.WriteLine($"ИМЯ\tНомер\tЧип\tИмя группы\tИмя дистанции\t\tСтартовое время\tРазряд\t\tКлуб\t\tРегион");
    foreach (var s in st)
    {
        Console.WriteLine($"{s.Partisipation?.Member?.Name}" +
                        $"\t{s.Number}" +
                        $"\t{s.Chip}" +
                        $"\t{s.Partisipation?.Group?.Name}" +
                        $"\t\t{s.Partisipation?.Group?.Distance.Name}" +
                        $"\t{s.StartTime}" +
                        $"\t\t{s.Partisipation?.Member?.Discharge?.Name}" +
                        $"\t{s.Partisipation?.Member?.Club?.Name}" +
                        $"\t{s.Partisipation?.Member?.Club?.Region?.Name}" +
                        $"\t{s.Partisipation.Group.StartAge}");
    }

}

void GiveInfo()
{
    for(int i = 0; i < codes.Length; i++)
    {
        Console.WriteLine($"\t-{codes[i]}");
    }
}

void ClearTiming(DbContext dbContext)
{
    dbContext.Set<Timing>().ExecuteDelete();
    idsOfFinishingPeople.Clear();
}
void Process(string received, DbContext dbContext)
{
    if (!idsOfFinishingPeople.Any(x => x == received))
    {
        TimeOnly to = TimeOnly.FromDateTime(DateTime.Now);
        GenericDataService<Timing> DataService = new(new RefereeHelperDbContextFactory());
        if (!dbContext.Set<Timing>().Any(x => x.Start.Chip == received))
        {
            to = TimeOnly.FromDateTime(DateTime.Now);
            countOfStartingPeople++;
            var t = dbContext.Set<Timing>().Add(new Timing
            {
                TimeNow = to,
                Start = dbContext.Set<Start>().ToList().First(x => x.Chip == received)
            });

            dbContext.SaveChanges();
            //var t = p.dbContext.Set<Timing>().First(x => x.TimeNow == to);

            //CountOfLapsForHim = p.dbContext.Set<Timing>().First(x => x.Id == t.Entity.Id).Start.Partisipation.Group.Distance.Circles;
            t.Entity.TimeFromStart = TimeOnly.FromTimeSpan((TimeSpan)(TimeOnly.FromDateTime(t.Entity.Start.StartTime.Value) - t.Entity.TimeNow));
            t.Entity.Circle = p.GetOfLapsForHim(dbContext, t.Entity);

            t.Entity.CircleTime = t.Entity.TimeFromStart;
            t.Entity.IsFinish = p.GetIsFinish(t.Entity.Id);
            if (t.Entity.IsFinish.Value)
            {
                CountOfFinishingPeople++;
                idsOfFinishingPeople.Add(received);
            }
            dbContext.SaveChanges();
            p.RefrechPlace(dbContext, t.Entity);
            p.RefrechAbsolutePlace(dbContext, t.Entity);
            //p.dbContext.Update(t.Entity);
            //p.dbContext.SaveChanges();
        }
        else
        {

            for (int i = p.dbContext.Set<Timing>().ToList().Last().Id - 1; i > -1; i--)
            {
                if (p.dbContext.Set<Timing>().Include(z => z.Start).Any(x => x.Start.Chip == received))
                {
                    if (TimeOnly.FromDateTime(DateTime.Now) - p.dbContext.Set<Timing>().ToList().First(x => x.Id == i).TimeNow > timeOfDifference)
                    {
                        var t = p.dbContext.Add(new Timing
                        {
                            TimeNow = to,

                            Start = p.dbContext.Set<Start>().ToList().First(x => x.Chip == received)
                        });
                        p.dbContext.SaveChanges();
                        CountOfLapsForHim = p.dbContext.Set<Timing>().Include(x => x.Start).ThenInclude(z => z.Partisipation).ThenInclude(c => c.Group).ThenInclude(v => v.Distance).ToList().First(x => x.Id == t.Entity.Id).Start.Partisipation.Group.Distance.Circles;
                        
                        t.Entity.Circle = p.GetOfLapsForHim(p.dbContext, t.Entity);
                        t.Entity.TimeFromStart = TimeOnly.FromTimeSpan((TimeSpan)(TimeOnly.FromDateTime((t.Entity.Start?.StartTime).Value) - t.Entity.TimeNow));

                        p.dbContext.SaveChanges();
                        t.Entity.CircleTime = p.GetTimeOfLap(p.dbContext, t.Entity);
                        t.Entity.IsFinish = p.GetIsFinish(t.Entity.Id);
                        if (t.Entity.IsFinish.Value)
                        {
                            CountOfFinishingPeople++;
                            idsOfFinishingPeople.Add(received);
                        }
                        p.RefrechPlace(p.dbContext, t.Entity);
                        p.RefrechAbsolutePlace(p.dbContext, t.Entity);
                        //p.dbContext.Update(t.Entity);
                    }
                    break;
                }
            }

        }

    }
}

void Second()
{
    Console.Write("Введите номер:");
    string msg = Console.ReadLine();
    UDPReceive u = UDPReceive.GetUdpClient();
    
    string receive;
    //Send(msg);
    //Console.WriteLine(receive);
    int i = 0, k = 10;
    while (i<k)
    {
        i++;
        try
        {
            receive = u.Receive().Result.ToString();
            if (receive == msg)
            {
                Console.WriteLine($"\tУспешно\n<<{receive}>> равно <<{msg}>>");
            }
            else
            {
                Console.WriteLine($"\tБезуспешно\n<<{receive}>> не равно <<{msg}>>");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        
    }
    u.Close();
    

}
void Third()
{
    Region region = new()
    {
        Name = "Владимир",
        СodeNumber = 33
    };
    Club club = new()
    {
        Region = region,
        Couch = "Пригожин Евгений Викторович",
        Name = "Ватные палочки"
    };
    Discharge discharge = new()
    {
        Name = "1-ый разряд"
    };
    Member Me = new() 
    {
        Name = "Андрей",
        FamilyName = "Сусалев",
        SecondName = "Сергеевич",
        Phone = "89964429706",
        City = "Владимир",
        BornDate = new DateTime(2004, 6, 1),
        Gender = true,
        Discharge = discharge,
        Club = club
    };
    Member Third = new()
    {
        Name = "Николай",
        FamilyName = "Соболев",
        SecondName = "Сергеевич",
        Phone = "89964429706",
        City = "Владимир",
        BornDate = new DateTime(2002, 6, 1),
        Gender = true,
        Discharge = discharge,
        Club = club
    };
    Member Michle = new()
    {
        Name = "Михаил",
        FamilyName = "Щукин",
        SecondName = "Сергеевич",
        Phone = "89542281488",
        City = "Владимир",
        BornDate = new DateTime(2003, 10, 19),
        Gender = true,
        Discharge = discharge,
        Club = club
    };
    Distance distance = new()
    {
        Name = "Общага -> Мегаторг",
        Length = 3000,
        Height = 0,
        Circles = 2,
        //StartTime = new TimeOnly(12, 00, 00)
    };
    Group group = new()
    {
        Distance = distance,
        Gender = true,
        Name = "Палочки",
        StartAge = 18,
        EndAge = 54
    };
    Competition competition = new()
    {
        Name = "Взятие ТЦ",
        Organizer = "Пу",
        Place = "Владимир",
        Date = new DateTime(2023,3,30),
        Judge = "Джо",
        Secretary = "Злнск",
        TypeAge = true
    };
    Partisipation partisipationThird = new()
    {
        Competition = competition,
        Member = Third,
        Group = group
    };
    Partisipation partisipationMe = new()
    {
        Competition = competition,
        Member = Me,
        Group = group
    };
    Partisipation partisipationMichle = new()
    {
        Competition = competition,
        Member = Michle,
        Group = group
    };
    Team team = new()
    {
        Name = "Крутые"
    };
    Start startMe = new()
    {
        Partisipation = partisipationMe,
        Team = team,
        StartTime = distance.StartTime,
        Number = 88,
        Chip = "fdsfdsfsd"
    };
    Start startTh = new()
    {
        Partisipation = partisipationThird,
        Team = team,
        StartTime = distance.StartTime,
        Number = 818,
        Chip = "w24eresfs"
    };
    Start startMichle = new()
    {
        Partisipation = partisipationMichle,
        Team = team,
        StartTime = distance.StartTime,
        Number = 54,
        Chip = "A0000006888"
    };
    using (var dbContext = new RefereeHelperDbContextFactory().CreateDbContext())
    {
        dbContext.Set<Start>().Add(startMe);
        dbContext.Set<Start>().Add(startMichle);
        dbContext.Set<Start>().Add(startTh);
        dbContext.SaveChanges();
    }
}
async Task<string> Send(string msg)
{
    var listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    byte[] data = Encoding.UTF8.GetBytes(msg);
    EndPoint remotePoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 27069);

    SocketAsyncEventArgs e = new SocketAsyncEventArgs();
    Console.WriteLine($"Отправлено. \n\t<<Disc:2000/01/01 00:00:04, Last:2000/01/01 00:00:04, Count:00001, Ant:01, Type:04, Tag:{msg}        >>");
    await listeningSocket.SendToAsync(data, SocketFlags.None, remotePoint);
    Console.WriteLine($"Закончена отправка.\n\t\t{DateTime.Now}");
    return "Успешная отправка";
} 





/*var competition = competitionDataService.Create(
        new Competition
        {
            Date = new DateTime(2023, 03, 27),
            Name = "Чемпионат друзей"
        }).Result;
var discharge = dischargeDataService.Create(
        new Discharge
        {
            Name = "1-ый"
        }).Result;
var member = memberDataService.Create(
        new Member
        {
            Name = "Андрей",
            FamilyName = "Сусалев",
            SecondName = "Сергеевич",
            Phone = "89964429706",
            City = "Владимир",
            BornDate = new DateTime(2004, 6, 1),
            Gender = true,
            Discharge = discharge
        }).Result;
var distance = distanceDataService.Create(
        new Distance
        {
            Name = "FirstDistance",
            Circles = 3,
            startTime = new DateTime(2023, 03, 27, 13, 00, 00)
        }).Result;
var group = groupDataService.Create(
        new Group
        {
            Name = "Мужчины 18+",
            Distance = distance,
            Gender = true,
            StartAge = 18,
            EndAge = 54
        }).Result;
//await partisipationDataService.Create(new Partisipation
//{
//    Competition = competition,
//    Member = member,
//    Group = group 
//});

//genericDataService.Update(1, new Discharge() { Name = "228 Вольт" });
//Console.WriteLine(genericDataService.Get(1).Result.Name);
//if (genericDataService.Delete(1).Result)
//{
//    Console.WriteLine("Удалено");
//}*/