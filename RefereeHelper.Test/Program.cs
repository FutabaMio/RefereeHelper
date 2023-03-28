using RefereeHelper.EntityFramework;
using RefereeHelper.EntityFramework.Services;
using RefereeHelper.Models;
using System.Runtime.CompilerServices;


IEnumerable<Member> ms;
var discharge = new Discharge { Name = "228" };
using (var dbContext = new RefereeHelperDbContextFactory().CreateDbContext())
{
    dbContext.Set<Member>().Add(new Member
    {
        Name = "Андрей",
        FamilyName = "Сусалев",
        SecondName = "Сергеевич",
        Phone = "89964429706",
        City = "Владимир",
        BornDate = new DateTime(2004, 6, 1),
        Gender = true,
        Discharge = discharge
    });
    ms = dbContext.Members.Where(x => x.Name == "Андрей");
    dbContext.SaveChanges();
}
foreach (var m in ms)
{
    Console.WriteLine($"{m.Id}-{m.Name}-{m.Discharge?.Name}");
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