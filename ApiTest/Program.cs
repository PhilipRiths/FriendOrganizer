using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FriendOrganizer.UI.Data.RestClient;

namespace ApiTest
{
    class Program
    {
        static void Main(string[] args)
        {
            RestClient rClient = new RestClient();
            rClient.EndPoint = "http://dry-cliffs-19849.herokuapp.com/users.json";

            var strRespone = rClient.MakeRequest();
            Console.WriteLine(strRespone);
            Console.ReadKey();

        }
    }
}
