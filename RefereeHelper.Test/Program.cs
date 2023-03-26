using RefereeHelper.EntityFramework;
using RefereeHelper.EntityFramework.Services;
using RefereeHelper.Models;

/*using (var dbContext = new RefereeHelperDbContextFactory().CreateDbContext())
{
    dbContext.Set<Discharge>().Add(new Discharge { Name = "228"});

    dbContext.SaveChanges();
}*/

GenericDataService<Discharge> genericDataService = new(new RefereeHelperDbContextFactory());
genericDataService.Create(new Discharge() { Name = "220 Вольт" });
genericDataService.Create(new Discharge() { Name = "220 Вольтаж" });
var discharges = genericDataService.GetAll().Result;
foreach (var discharge in discharges)
{
    Console.WriteLine(discharge.Name);
}
//genericDataService.Update(1, new Discharge() { Name = "228 Вольт" });
//Console.WriteLine(genericDataService.Get(1).Result.Name);
//if (genericDataService.Delete(1).Result)
//{
//    Console.WriteLine("Удалено");
//}