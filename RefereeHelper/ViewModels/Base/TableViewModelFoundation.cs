using RefereeHelper.Commands;
using RefereeHelper.Commands.Base;
using RefereeHelper.Domain.Models.Base;
using RefereeHelper.EntityFramework;
using RefereeHelper.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Windows;
using Configuration = RefereeHelper.Commands.Configuration;

namespace RefereeHelper.ViewModels.Base
{
    public class TableViewModelFoundation<TModel> : ViewModelBase
         where TModel : BaseEntity, new()
    {
        protected readonly RefereeHelperDbContext _refereeHelperDbContextFactory;

        private ObservableCollection<TModel> _items;
        private TModel _selectedItem;
        private List<int> _updatedItemsIds;
        protected string _filter;
        
        public TModel SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                if (_selectedItem!=null)
                {
                    _updatedItemsIds.Add(_selectedItem.Id);
                }
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        public ObservableCollection<TModel> Items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
                OnPropertyChanged(nameof(Items));
            }
        }

        #region Фильтр
        public string Filter
        {
            get => _filter;
            set
            {
                _filter = value;
                FilterAction(value);
                OnPropertyChanged(nameof(Filter));
            }
        }

        protected virtual void FilterAction(string value) { }
        #endregion

        //#region Операции с данными

        #region Команды
        public CommandBase AddNewRecord { get; }
        public CommandBase DeleteSelectedItem { get; }
        public CommandBase CommitChanges { get; set; }
        #endregion

        
  /*      private void Commit(object obj)
        {
            try
            {
                List<TModel> dbData;
                var itemsIds = Items.Select(x => x.Id);

                //удаляем убранные объекты
                using(var dbContext = _refereeHelperDbContextFactory.CreateDbContext())
                {
                    dbContext.Set<TModel>().RemoveRange(dbContext.Set<TModel>().Where(XamlGeneratedNamespace => !itemsIds.Contains(XamlGeneratedNamespace.Id)));
                    dbData = dbContext.Set<TModel>().ToList();
                    dbContext.SaveChanges();
                }

                //добавляем выбранный объект
                using(var dbContext = _refereeHelperDbContextFactory.CreateDbContext())
                {
                    foreach(var item in Items)
                    {
                        //добавляем запись есои её не существует в БД
                        if (!dbData.Any(x => x.Id==item.Id))
                        {
                            //item.Id==0
                            dbContext.Set<TModel>().Add(item);
                        }
                    }

                    //обновляем выбранный элемент
                    dbContext.UpdateRange(Items.Where(x => _updatedItemsIds.Contains(x.Id)));
                    _updatedItemsIds.Clear();

                    dbContext.SaveChanges();

                    Configuration.Members = new ObservableCollection<Member>(dbContext.Member);
                    Configuration.Groups = new ObservableCollection<Group>(dbContext.Group);
                    Configuration.Distances = new ObservableCollection<Distance>(dbContext.Distance);
                    Configuration.Clubs = new ObservableCollection<Club>(dbContext.Club);
                    Configuration.Competitions = new ObservableCollection<Competition>(dbContext.Competitions);
                    Configuration.Regions = new ObservableCollection<Region>(dbContext.Regions);
                    Configuration.Timings = new ObservableCollection<Timing>(dbContext.Timings);
                }
                MessageBox.Show("Успех!");
            }
            catch(Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void AddRecord(object obj)
        {
            if (Items.Count!=0)
            {
                Items.Add(new TModel() { Id=MaxId()+1 });
            }
            else
            {
                Items.Add(new TModel() { Id=1 });
            }
        }

        private int MaxId()
        {
            int max = Items[0].Id;
            foreach(var item in Items)
            {
                if (item.Id>max)
                {
                    max=item.Id;
                }
            }
            return max;
        }

        private void DeleteItem(object obj)
        {
            if(new DialogWindow().ShowDialog()==true)
            {
                if(Items.Count!=0 && SelectedItem!=null)
                {
                    var previousItemIndex = Items.IndexOf(SelectedItem)-1;
                    if(previousItemIndex>=0 && previousItemIndex<Items.Count)
                    {
                        var beforeSelectedItem = Items[previousItemIndex];
                        Items.Remove(SelectedItem);
                        SelectedItem=beforeSelectedItem;
                    }
                    else
                    {
                        Items.Remove(SelectedItem);
                    }
                }
            }
        }

        #endregion

        #endregion

        //конструктор
   /*     public TableViewModelFoundation()
        {
            _refereeHelperDbContextFactory=new RefereeHelperDbContextFactory();
            _updatedItemsIds= new List<int>();
            DeleteSelectedItem = new RelayCommand(DeleteItem, (obj) => String.IsNullOrEmpty(_filter));
        } */
    }
}
