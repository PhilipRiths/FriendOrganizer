﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FriendOrganizer.UI.Event;
using Prism.Commands;
using Prism.Events;

namespace FriendOrganizer.UI.ViewModel
{
   public abstract class DetailViewModelBase : ViewModelBase, IDetailViewModel
    {
        protected readonly IEventAggregator EventAggregator;

        public DetailViewModelBase(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
            DeleteCommand = new DelegateCommand(OnDeleteExecute);
        }

        protected abstract void OnDeleteExecute();

        protected abstract void OnSaveExecute();

        protected abstract bool OnSaveCanExecute();

       

        public ICommand DeleteCommand { get; set; }

        public ICommand SaveCommand { get; set; }

        public abstract Task LoadAsync(int? id);


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


    }
}
