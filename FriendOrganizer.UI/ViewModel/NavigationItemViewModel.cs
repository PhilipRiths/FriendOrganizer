using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel
{
    public class NavigationItemViewModel : ViewModelBase
    {
        public NavigationItemViewModel(int id, string displayMember)
        {
            Id = id;
            DisplayMember = displayMember;

        }
        private string _displayMember;

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
    }
}
