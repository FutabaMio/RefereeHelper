using RefereeHelper.EntityFramework;
using RefereeHelper.EntityFramework.Services;
using RefereeHelper.Models;
using System.Runtime.CompilerServices;

/*using (var dbContext = new RefereeHelperDbContextFactory().CreateDbContext())
{
    dbContext.Set<Discharge>().Add(new Discharge { Name = "228"});

    dbContext.SaveChanges();
}*/

GenericDataService<Discharge> dischargeDataService = new(new RefereeHelperDbContextFactory());
GenericDataService<Member> memberDataService = new(new RefereeHelperDbContextFactory());
GenericDataService<Competition> competitionDataService = new(new RefereeHelperDbContextFactory());
GenericDataService<Distance> distanceDataService = new(new RefereeHelperDbContextFactory());
GenericDataService<Group> groupDataService = new(new RefereeHelperDbContextFactory());
GenericDataService<Partisipation> partisipationDataService = new(new RefereeHelperDbContextFactory());
partisipationDataService.Create(new Partisipation() { Competition = competitionDataService.Create(new Competition() { Date = new DateTime(2023, 03, 27), Name = "Чемпионат друзей" }).Result,
                                                      Member = memberDataService.Create(new Member() { Name = "Андрей", FamilyName = "Сусалев", SecondName = "Сергеевич", Phone = "89964429706", City = "Владимир", BornDate = new DateTime(2004, 6, 1), Gender = true, Discharge = dischargeDataService.Create(new Discharge() { Name = "1-ый" }).Result }).Result,
                                                      Group = groupDataService.Create(new Group() { Name = "Мужчины 18+", Distance = distanceDataService.Create(new Distance() { Name = "FirstDistance", Circles = 3, startTime = new DateTime(2023, 03, 27, 13, 00, 00) }).Result, Gender = true, StartAge = 18, EndAge = 54 }).Result});

//genericDataService.Update(1, new Discharge() { Name = "228 Вольт" });
//Console.WriteLine(genericDataService.Get(1).Result.Name);
//if (genericDataService.Delete(1).Result)
//{
//    Console.WriteLine("Удалено");
//}