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
        case "Первичная обработка данных":
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
    Process("P4BK");
    Process("Xi");
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
    GenericDataService<Timing> DataService = new(new RefereeHelperDbContextFactory());
    if (p.dbContext.Set<Timing>().Any(x => x.Start?.Chip == received))
    {
        countOfStartingPeople++;
        p.dbContext.Set<Timing>().Add(new Timing
        {
            TimeNow = TimeOnly.FromDateTime(DateTime.Now),
            Start = p.dbContext.Set<Start>().First(x => x.Chip == received)
        });
        var t =

        CountOfLapsForHim = p.dbContext.Set<Timing>().First(x => x.Id == t.Id).Start.Partisipation.Group.Distance.Circles;
        t.TimeFromStart = TimeOnly.FromTimeSpan((TimeSpan)(t.Start.StartTime - t.TimeNow));
        t.Circle = p.GetOfLapsForHim(t.Id);
        t.CircleTime = p.GetTimeOfLap(t.Id);
        t.IsFinish = p.GetIsFinish(t.Id);
        p.RefrechPlace(t.Id);
        p.RefrechAbsolutePlace(t.Id);
        DataService?.Update(t.Id, t);
    }
    else
    {

        for (int i = DataService.GetAll().Result.Count() - 1; i > -1; i--)
        {

            if (DataService.Get(i).Result.Start?.Chip == received)
            {
                if (TimeOnly.FromDateTime(DateTime.Now) - DataService.Get(i).Result.TimeNow > timeOfDifference)
                {
                    var t = DataService.Create(new Timing()
                    {
                        TimeNow = TimeOnly.FromDateTime(DateTime.Now),
                        Start = p.dbContext.Set<Start>().First(x => x.Chip == received)
                    }).Result;
                    t.TimeFromStart = TimeOnly.FromTimeSpan((TimeSpan)(t.Start?.StartTime - t.TimeNow));
                    CountOfLapsForHim = DataService.Get(t.Id).Result.Start.Partisipation.Group.Distance.Circles;
                    t.Circle = p.GetOfLapsForHim(t.Id);
                    t.CircleTime = p.GetTimeOfLap(t.Id);
                    t.IsFinish = p.GetIsFinish(t.Id);
                    if (t.IsFinish.Value)
                    {
                        CountOfFinishingPeople++;
                    }
                    p.RefrechPlace(t.Id);
                    p.RefrechAbsolutePlace(t.Id);
                    DataService?.Update(t.Id, t);
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

    dbContext.SaveChanges();
}*/

{
}
{
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