using RefereeHelper;
using RefereeHelper.EntityFramework;
using RefereeHelper.EntityFramework.Services;
using RefereeHelper.Models;
using System.Net;
using System.Net.Sockets;
using System.Text;

int countOfStartingPeople = 0;
string[] codes = { "Первичная обработка данных", "Сбор", "Заполнить БД", "Проверить БД", "help", "close"};
decimal CountOfLapsForHim;
int CountOfFinishingPeople = 0;
TimeSpan timeOfDifference = new TimeSpan(0, 0, 5);
while (true)
{
    Console.WriteLine("Введите запрос операции:");
    string code = Console.ReadLine();
    Console.WriteLine($"\n\n\t\t{code}\n");
    switch (code)
    {
        case "1":
            First();
            break;
        case "Сбор":
            Second();
            break;
        case "Заполнить БД":
            Third();
            break;
        case "Проверить БД":
            CheckDB();
            break;
        case "help":
            GiveInfo();
            break;
        case "close":
            break;            
        default:
            GiveInfo();
            Console.WriteLine("TryNow");
            break;
    }
}
void First()
{

    countOfStartingPeople = 0;
    while (true)
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
    Process("Xi");
    Thread.Sleep(10000);
    Process("P4BK");

    Thread.Sleep(330);
    Process("Xi");
    Thread.Sleep(10000);
    Process("P4BK");
    Console.WriteLine("Имя\tТекущее время\tВремя со старта\tКруг\tПозиция\tВремя круга");
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
                        $"\t{s.Partisipation?.Member?.Club?.Region?.Name}");
    }

}

void GiveInfo()
{
    for(int i = 0; i < codes.Length; i++)
    {
        Console.WriteLine($"\t-{codes[i]}");
    }
}
void Process(string received)
{
    Processing p = new();
    TimeOnly to = TimeOnly.FromDateTime(DateTime.Now);
    GenericDataService<Timing> DataService = new(new RefereeHelperDbContextFactory());
    if (!p.dbContext.Set<Timing>().Any(x => x.Start.Chip == received))
    {
        to = TimeOnly.FromDateTime(DateTime.Now);
        countOfStartingPeople++;
        var t = p.dbContext.Set<Timing>().Add(new Timing
        {
            TimeNow = to,
            Start = p.dbContext.Set<Start>().First(x => x.Chip == received)
        });

        p.dbContext.SaveChanges();
        //var t = p.dbContext.Set<Timing>().First(x => x.TimeNow == to);

        CountOfLapsForHim = p.dbContext.Set<Timing>().First(x => x.Id == t.Entity.Id).Start.Partisipation.Group.Distance.Circles;
        t.Entity.TimeFromStart = TimeOnly.FromTimeSpan((TimeSpan)(t.Entity.Start.StartTime - t.Entity.TimeNow));
        t.Entity.Circle = p.GetOfLapsForHim(t.Entity.Id);
        t.Entity.CircleTime = p.GetTimeOfLap(t.Entity.Id);
        t.Entity.IsFinish = p.GetIsFinish(t.Entity.Id);
        p.RefrechPlace(t.Entity.Id);
        p.RefrechAbsolutePlace(t.Entity.Id);
        p.dbContext.Update(t.Entity);
        p.dbContext.SaveChanges();

    }
    else
    {

        for (int i = p.dbContext.Set<Timing>().Count() - 1; i > -1; i--)
        {
            var k = p.dbContext.Set<Timing>().Select(x => new Timing
            {
                Id = x.Id,
                Start = new Start
                {
                    Id = x.Start.Id,
                    Chip = x.Start.Chip
                }
            });
            if (k.First(x => x.Id == i)?.Start?.Chip == received)
            {
                if (TimeOnly.FromDateTime(DateTime.Now) - DataService.Get(i).Result.TimeNow > timeOfDifference)
                {
                    var t = p.dbContext.Add(new Timing
                    {
                        TimeNow = to,
                        
                        Start = p.dbContext.Set<Start>().Select(x => new Start
                        {
                            Chip = x.Chip,
                            Partisipation = new Partisipation
                            {
                                Group = new Group
                                {
                                    Name = x.Partisipation.Group.Name,
                                    Distance = new Distance
                                    {
                                        Id = x.Partisipation.Group.Distance.Id,
                                        Name = x.Partisipation.Group.Distance.Name
                                    }
                                }
                            }
                        }).ToList().First(x => x.Chip == received)
                    });
                    Console.WriteLine(t.Entity.Id);
                    p.dbContext.SaveChanges();
                    CountOfLapsForHim = p.dbContext.Set<Timing>().Select(x => new Timing
                    {
                        Id = x.Id,
                        Start = new Start
                        {
                            Id = x.Start.Id,
                            Partisipation = new Partisipation
                            {
                                Id = x.Start.Partisipation.Id,
                                Group = new Group
                                {
                                    Id = x.Start.Partisipation.Group.Id,
                                    Name = x.Start.Partisipation.Group.Name,
                                    Distance = new Distance()
                                    {
                                        Id = x.Start.Partisipation.Group.Distance.Id,
                                        Circles = x.Start.Partisipation.Group.Distance.Circles
                                    }
                                }
                            }
                        }
                    }).ToList().First(x => x.Id == t.Entity.Id).Start.Partisipation.Group.Distance.Circles;
                    t.Entity.TimeFromStart = TimeOnly.FromTimeSpan((TimeSpan)(t.Entity.Start?.StartTime - t.Entity.TimeNow));
                    p.dbContext.SaveChanges();
                    Console.WriteLine($"{t.Entity.Id}");
                    t.Entity.Circle = p.GetOfLapsForHim(t.Entity.Id);
                    t.Entity.CircleTime = p.GetTimeOfLap(t.Entity.Id);
                    t.Entity.IsFinish = p.GetIsFinish(t.Entity.Id);
                    if (t.Entity.IsFinish.Value)
                    {
                        CountOfFinishingPeople++;
                    }
                    p.RefrechPlace(t.Entity.Id);
                    p.RefrechAbsolutePlace(t.Entity.Id);
                    p.dbContext.Update(t.Entity);
                    p.dbContext.SaveChanges();
                }
                break;
            }
        }

    }
}

void Second()
{
    Console.Write("Введите номер:");
    string msg = Console.ReadLine();
    UDPReceive u = new(27069);
    
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
        StartTime = new TimeOnly(12, 00, 00)
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
        Chip = "P4BK"
    };
    Start startMichle = new()
    {
        Partisipation = partisipationMichle,
        Team = team,
        StartTime = distance.StartTime,
        Number = 54,
        Chip = "Xi"
    };
    using (var dbContext = new RefereeHelperDbContextFactory().CreateDbContext())
    {
        dbContext.Set<Start>().Add(startMe);
        dbContext.Set<Start>().Add(startMichle);
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