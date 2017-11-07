using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.View.Services;
using Prism.Commands;
using Prism.Events;

namespace FriendOrganizer.UI.ViewModel
{
   public abstract class DetailViewModelBase : ViewModelBase, IDetailViewModel
   {
       protected readonly IMessageDialogService MessageDialogService;

        protected readonly IEventAggregator EventAggregator;
        private int _id;
        private string _title;

        public DetailViewModelBase(IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService)
        {
            MessageDialogService = messageDialogService;
                EventAggregator = eventAggregator;
            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
            DeleteCommand = new DelegateCommand(OnDeleteExecute);
            CloseDetailViewCommand = new DelegateCommand(OnCloseDetailViewExecute);
        }

        protected  virtual async void OnCloseDetailViewExecute()
        {
            if (HasChanges)
            {
                var result = await MessageDialogService.ShowOkCancelDialogAsync(
                    "You've made changes. Close this item?", "Question");
                if (result == MessageDialogResult.Cancel)
                {
                    return;
                }
            }
          

           EventAggregator.GetEvent<AfterDetailClosedEvent>()
                .Publish(new AfterDetailClosedEventArgs()
               {
                   Id = this.Id,
                   ViewModelName = this.GetType().Name
               });
        }

        public ICommand CloseDetailViewCommand { get; set; }

        protected abstract void OnDeleteExecute();

        protected abstract void OnSaveExecute();

        protected abstract bool OnSaveCanExecute();

       

        public ICommand DeleteCommand { get; set; }

        public ICommand SaveCommand { get; set; }

        public abstract Task LoadAsync(int id);

        public int Id
        {
            get { return _id; }
            protected set { _id = value; }
        }

        public string Title
        {
            get { return _title; }
            protected set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        private bool _hasChanges;

        public bool HasChanges
        {
            get { return _hasChanges; }
            set
            {
                if (_hasChanges != value)
                {
                    _hasChanges = value;
                    OnPropertyChanged();
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
               
                
            }
        }

       protected virtual void RaiseCollectionSavedEvent()
       {
           EventAggregator.GetEvent<AfterCollectionSavedEvent>()
                .Publish(new AfterCollectionSavedEventArgs()
               {
                   ViewModelName = this.GetType().Name
               });
       }

        public virtual void RaiseDetailDeletedEvent(int modelId)
        {
            EventAggregator.GetEvent<AfterDetailDeletedEvent>().Publish(new AfterDetailDeletedEventArgs
            {
                Id = modelId,
                ViewModelName = this.GetType().Name
            });
        }

        public virtual void RaiseDetailSavedEvent(int modelId, string displayMember)
        {
            EventAggregator.GetEvent<AfterDetailSavedEvent>().Publish(new AfterDetailsSavedEventArgs
            {
                Id = modelId,
                DisplayMember = displayMember,
                ViewModelName = this.GetType().Name
            });
        }
       protected  async Task SaveWithOptimisticConcurrencyAsync(Func<Task> saveFunc,
           Action afterSaveAction)
       {
           try
           {
               await saveFunc();
           }
           catch (DbUpdateConcurrencyException ex)
           {
               var databaseValues = ex.Entries.Single().GetDatabaseValues();
               if (databaseValues == null)
               {
                 await  MessageDialogService.ShowInfoDialogAsync("The entity has been deleted by another user");
                   RaiseDetailDeletedEvent(Id);
                   return;

               }

               var result = await MessageDialogService.ShowOkCancelDialogAsync("The entity has been changed in "
                                                                    + "the meantime by someone else. Click Ok to save you're changes anyways, click Cancel"
                                                                    + "to reload the entity from the database",
                   "Question");

               if (result == MessageDialogResult.OK)
               {
                   var entry = ex.Entries.Single();
                   entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                   await saveFunc();
               }
               else
               {
                   await ex.Entries.Single().ReloadAsync();
                   await LoadAsync(Id);
               }
           }

           afterSaveAction();

       }

    }
}
