using Microsoft.EntityFrameworkCore;
using RefereeHelper.Domain.Models;
using RefereeHelper.EntityFramework.Services;
using RefereeHelper.EntityFramework;
using RefereeHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RefereeHelper.OptionsWindows
{
    /// <summary>
    /// Логика взаимодействия для TimingEdit.xaml
    /// </summary>
    public partial class TimingEdit : Window
    {
        public TimingDataItem Timing { get; set; }

        public TimingEdit()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        List<int> NumbersOfFinishingPeople = new List<int>();
        int countOfStartingPeople = 0;
        Processing p = new();
        int CountOfFinishingPeople = 0;

        void Process(int startNumber, DbContext dbContext)
        {
            int CountOfLapsForHim = 0;
            if (!NumbersOfFinishingPeople.Any(x => x == startNumber))
            {
                TimeOnly to = TimeOnly.FromDateTime(DateTime.Now);
                GenericDataService<Timing> DataService = new(new RefereeHelperDbContextFactory());
                if (!dbContext.Set<Timing>().Any(x => x.Start.Number == startNumber))
                {

                    
                    countOfStartingPeople++;
                    //var t = dbContext.Set<Timing>().Add(new Timing
                    //{
                    //    TimeNow = to
                    //});
                    if (dbContext.Set<Models.Start>().ToList().Any(x => x.Number == startNumber))
                    {
                        var t = dbContext.Set<Timing>().ToList().First(x => x.Id == Int32.Parse(Timing.Id));
                        
                        var ll = TimeOnly.FromTimeSpan((TimeOnly.FromDateTime(t.Start.StartTime.Value) - t.TimeNow).Value);
                        t.TimeFromStart = ll;
                        t.Circle = p.GetOfLapsForHim(dbContext, t);

                        t.CircleTime = t.TimeFromStart;
                        t.IsFinish = p.GetIsFinish(t.Id);
                        if (t.IsFinish.Value)
                        {
                            CountOfFinishingPeople++;
                            NumbersOfFinishingPeople.Add(startNumber);
                        }
                        dbContext.SaveChanges();
                        p.RefrechPlace(dbContext, t);
                        p.RefrechAbsolutePlace(dbContext, t);
                    }


                    dbContext.SaveChanges();
                    //var t = p.dbContext.Set<Timing>().First(x => x.TimeNow == to);

                    //CountOfLapsForHim = p.dbContext.Set<Timing>().First(x => x.Id == t.Entity.Id).Start.Partisipation.Group.Distance.Circles;

                    //p.dbContext.Update(t.Entity);
                    //p.dbContext.SaveChanges();
                }
                else
                {

                    if (p.dbContext.Set<Timing>().Include(z => z.Start).Any(x => x.Start.Number == startNumber))
                    {
                        var t = dbContext.Set<Models.Timing>().ToList().First(x => x.Id == Int32.Parse(Timing.Id));
                        p.dbContext.SaveChanges();
                        CountOfLapsForHim = p.dbContext.Set<Timing>().Include(x => x.Start).ThenInclude(z => z.Partisipation).ThenInclude(c => c.Group).ThenInclude(v => v.Distance).ToList().First(x => x.Id == t.Id).Start.Partisipation.Group.Distance.Circles;

                        t.Circle = p.GetOfLapsForHim(p.dbContext, t);
                        t.TimeFromStart = TimeOnly.FromTimeSpan((TimeOnly.FromDateTime((t.Start?.StartTime).Value) - t.TimeNow).Value);

                        p.dbContext.SaveChanges();
                        t.CircleTime = p.GetTimeOfLap(p.dbContext, t);
                        t.IsFinish = p.GetIsFinish(t.Id);
                        if (t.IsFinish.Value)
                        {
                            CountOfFinishingPeople++;
                            NumbersOfFinishingPeople.Add(startNumber);
                        }
                        p.RefrechPlace(p.dbContext, t);
                        p.RefrechAbsolutePlace(p.dbContext, t);
                        p.dbContext.SaveChanges();
                        //p.dbContext.Update(t.Entity);
                        
                    }
                }
            }
        }

        

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            
                int StartNumber = Int32.Parse(startNumberTbx.Text.ToString());
                Process(StartNumber, p.dbContext);
                this.Close();
            
        }

        public void ShowTiming(TimingDataItem timing)
        {
            Timing = timing;
            //FamilyNameTbx.Text=$"{timing.FamilyName}";
            //NameTbx.Text=$"{timing.MemberName}";
            startNumberTbx.Text=$"{timing.Startnumber}";
            this.Show();
        }
    }
}
