﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FriendOrganizer.Model;
using FriendOrganizer.UI.Data.Repositories;
using FriendOrganizer.UI.View.Services;
using FriendOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;

namespace FriendOrganizer.UI.ViewModel
{
   public class MeetingDetailViewModel : DetailViewModelBase, IMeetingDetailViewModel
    {
        private IMeetingRepository _meetingRepository;
        private IMessageDialogService _messageDialogService;
        private Friend _selectedAvailableFriend;
        private Friend _selectedAddedFriend;

        public MeetingDetailViewModel(IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            IMeetingRepository meetingRepository) : base(eventAggregator)
        {
            _meetingRepository = meetingRepository;
            _messageDialogService = messageDialogService;

            AddedFriends = new ObservableCollection<Friend>();
            AvailableFriends = new ObservableCollection<Friend>();
            AddFriendCommand = new DelegateCommand(OnAddFriendExecute, OnAddFriendCanExecute);
            RemoveFriendCommand = new DelegateCommand(OnRemoveFriendExecute, OnRemoveFriendCanExecute);
        }

        private bool OnRemoveFriendCanExecute()
        {
            return SelectedAddedFriend != null;
        }

        private void OnRemoveFriendExecute()
        {
            var friendToRemove = SelectedAddedFriend;

            Meeting.Model.Friends.Remove(friendToRemove);
            AddedFriends.Remove(friendToRemove);
            AvailableFriends.Add(friendToRemove);
            HasChanges = _meetingRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        private bool OnAddFriendCanExecute()
        {
            return SelectedAvailableFriend != null;
        }

        private void OnAddFriendExecute()
        {
            var friendToAdd = SelectedAvailableFriend;

            Meeting.Model.Friends.Add(friendToAdd);
            AddedFriends.Add(friendToAdd);
            AvailableFriends.Remove(friendToAdd);
            HasChanges = _meetingRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        public ICommand RemoveFriendCommand { get; set; }

        public ICommand AddFriendCommand { get; set; }

        public ObservableCollection<Friend> AvailableFriends { get; set; }

        public ObservableCollection<Friend> AddedFriends { get; set; }

        public Friend SelectedAvailableFriend
        {
            get { return _selectedAvailableFriend; }
            set
            {
                _selectedAvailableFriend = value;
                OnPropertyChanged();
                ((DelegateCommand)AddFriendCommand).RaiseCanExecuteChanged();
            }
        }
        public Friend SelectedAddedFriend
        {
            get { return _selectedAddedFriend; }
            set
            {
                _selectedAddedFriend = value;
                OnPropertyChanged();
                ((DelegateCommand)RemoveFriendCommand).RaiseCanExecuteChanged();
            }
        }

        private MeetingWrapper _meeting;
        private List<Friend> _allFriends;

        public MeetingWrapper Meeting  
        {
            get { return _meeting; }
            private set
            {
                _meeting = value;
                OnPropertyChanged();
            }
        }


        protected override void OnDeleteExecute()
        {
            var result =
                _messageDialogService.ShowOkCancelDialog($"Do you really want to delete the meeting {Meeting.Title}",
                    "Question");
            if (result == MessageDialogResult.OK)
            {
                _meetingRepository.Remove(Meeting.Model);
                _meetingRepository.SaveAsync();
                RaiseDetailDeletedEvent(Meeting.Id);
            }
        }

        protected override async void OnSaveExecute()
        {
            await _meetingRepository.SaveAsync();
            HasChanges = _meetingRepository.HasChanges();
            RaiseDetailSavedEvent(Meeting.Id, Meeting.Title);
        }

        protected override bool OnSaveCanExecute()
        {
            return Meeting != null && !Meeting.HasErrors && HasChanges;
        }

        public override async Task LoadAsync(int? meetingid)
        {
            var meeting = meetingid.HasValue
                ? await _meetingRepository.GetByIdAsync(meetingid.Value)
                : CreateNewMeeting();

            InitializeMeeting(meeting);

           _allFriends = await _meetingRepository.GetAllFriendsAsync();

            SetupPicklist();
        }

        private void SetupPicklist()
        {
            var meetingFriendIds = Meeting.Model.Friends.Select(f => f.Id).ToList();
            var addedFriends = _allFriends.Where(f => meetingFriendIds.Contains(f.Id)).OrderBy(f => f.FirstName);
            var availableFriends = _allFriends.Except(addedFriends).OrderBy(f => f.FirstName);

            AddedFriends.Clear();
            AvailableFriends.Clear();
            foreach (var addedFriend in addedFriends)
            {
                AddedFriends.Add(addedFriend);
            }
            foreach (var availableFriend in availableFriends)
            {
                AvailableFriends.Add(availableFriend);
            }
        }

        private void InitializeMeeting(Meeting meeting)
        {
            Meeting = new MeetingWrapper(meeting);
            Meeting.PropertyChanged += (s, e) =>
            {
                if (!HasChanges)
                {
                    HasChanges = _meetingRepository.HasChanges();
                }
                if (e.PropertyName == nameof(Meeting.HasErrors))
                {
                    ((DelegateCommand) SaveCommand).RaiseCanExecuteChanged();
                }
            };
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();

            if (Meeting.Id == 0)
            {
                //Little trick to trigger validation
                Meeting.Title = "";
            }
        }

        private Meeting CreateNewMeeting()
        {
            var meeting = new Meeting()
            {
                DateFrom = DateTime.Now.Date,
                DateTo = DateTime.Now.Date,

            };
            _meetingRepository.Add(meeting);
            return meeting;


        }
    }
}
