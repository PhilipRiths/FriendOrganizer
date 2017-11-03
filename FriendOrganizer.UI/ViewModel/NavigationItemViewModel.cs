﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FriendOrganizer.UI.Event;
using Prism.Commands;
using Prism.Events;

namespace FriendOrganizer.UI.ViewModel
{
    public class NavigationItemViewModel : ViewModelBase
    {
        public NavigationItemViewModel(int id, string displayMember,
            IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            Id = id;
            DisplayMember = displayMember;
            OpenFriendDetailViewCommand = new DelegateCommand(OnOpenFriendDetailView);

        }

  

        private string _displayMember;
        private IEventAggregator _eventAggregator;

        public string DisplayMember
        {
            get { return _displayMember; }
            set
            {
                _displayMember = value; 
                OnPropertyChanged();
            }
        }

        public int Id { get; set; }

        public ICommand OpenFriendDetailViewCommand { get; }

        private void OnOpenFriendDetailView()
        {
            _eventAggregator.GetEvent<OpenFriendDetailViewEvent>()
                .Publish(Id);
        }
    }
}
