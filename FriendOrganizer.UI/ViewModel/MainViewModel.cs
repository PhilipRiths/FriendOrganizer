using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FriendOrganizer.Model;
using FriendOrganizer.UI.Annotations;
using FriendOrganizer.UI.Data;
using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.View.Services;
using Prism.Commands;
using Prism.Events;

namespace FriendOrganizer.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private IEventAggregator _eventAggregator;
        private Func<IFriendDetailViewModel> _friendDetailViewModelCreator;
        private IFriendDetailViewModel _detailViewModel;
        private IMessageDialogService _messageDialogService;

        public MainViewModel(INavigationViewModel navigationViewModel,
           Func<IFriendDetailViewModel> friendDetailViewModelCreator,
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService)
        {
            _eventAggregator = eventAggregator;
          
            _friendDetailViewModelCreator = friendDetailViewModelCreator;
            _messageDialogService = messageDialogService;

            _eventAggregator.GetEvent<OpenFriendDetailViewEvent>()
                .Subscribe(OnOpenFriendDetailView);

            _eventAggregator.GetEvent<AfterFriendDeletedEvent>()
                .Subscribe(AfterFriendDeleted);

            CreateNewFriendCommand =  new DelegateCommand(OnCreateNewFriendExecute);

            NavigationViewModel = navigationViewModel;
        }

  

        public INavigationViewModel NavigationViewModel { get; }

        public ICommand CreateNewFriendCommand { get; }

        public IFriendDetailViewModel DetailViewModel
        {
            get { return _detailViewModel; }
           private set
            {
                _detailViewModel = value; 
                OnPropertyChanged();
            }
        }


        public async Task LoadAsync()
        {
            await NavigationViewModel.LoadAsync();
        }


        private async void OnOpenFriendDetailView(int? friendId)
        {
            if (DetailViewModel != null && DetailViewModel.HasChanges)
            {
                var result =
                    _messageDialogService.ShowOkCancelDialog("You've made changes. Navigate away?", "Question");
               
                if (result == MessageDialogResult.Cancel)
                {
                    return;
                }
            }
           DetailViewModel = _friendDetailViewModelCreator();
            await DetailViewModel.LoadAsync(friendId);
        }
        private void OnCreateNewFriendExecute()
        {
           OnOpenFriendDetailView(null);
        }

        private void AfterFriendDeleted(int friendId)
        {
            DetailViewModel = null;
        }

    }
}
