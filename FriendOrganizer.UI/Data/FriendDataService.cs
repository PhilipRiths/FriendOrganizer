using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FriendOrganizer.Model;

namespace FriendOrganizer.UI.Data
{
    public class FriendDataService : IFriendDataService
    {
        public IEnumerable<Friend> GetAll()
        {
            // TODO: Load data from a real database
            yield return new Friend {FirstName = "Kalle", LastName = "Anka"};
            yield return new Friend { FirstName = "Knatte", LastName = "Anka" };
            yield return new Friend { FirstName = "Fnatte", LastName = "Anka" };
            yield return new Friend { FirstName = "Tjatte", LastName = "Anka" };

        }
    }
}
