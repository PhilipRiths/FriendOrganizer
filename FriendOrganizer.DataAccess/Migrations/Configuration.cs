using System.Collections.Generic;
using FriendOrganizer.Model;

namespace FriendOrganizer.DataAccess.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<FriendOrganizer.DataAccess.FriendOrganizerDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(FriendOrganizer.DataAccess.FriendOrganizerDbContext context)
        {
            context.Friends.AddOrUpdate(f => f.FirstName,
             new Friend { FirstName = "Kalle", LastName = "Anka" },
             new Friend { FirstName = "Knatte", LastName = "Anka" },
             new Friend { FirstName = "Fnatte", LastName = "Anka" },
             new Friend { FirstName = "Tjatte", LastName = "Anka" });

            context.ProgrammingLanguages.AddOrUpdate(pl => pl.Name,
                new ProgrammingLanguage() { Name = "C#" },
            new ProgrammingLanguage() { Name = "TypeScript" },
            new ProgrammingLanguage() { Name = "F#" },
            new ProgrammingLanguage() { Name = "Swift" },
            new ProgrammingLanguage() { Name = "Java" }
                );
            context.SaveChanges();

            context.FriendPhoneNumbers.AddOrUpdate(pn => pn.Number,
                new FriendPhoneNumber{Number = "+49 345678956", FriendId = context.Friends.First().Id});

            context.Meetings.AddOrUpdate(m => m.Title, 
                 new Meeting()
                 {
                     Title = "Watching soccer",
                     DateFrom = new DateTime(2018, 5, 26),
                     DateTo = new DateTime(2018, 5, 26),
                     Friends = new List<Friend>
                     {
                         context.Friends.Single(f => f.FirstName == "Kalle" && f.LastName == "Anka"),
                         context.Friends.Single(f => f.FirstName == "Knatte" && f.LastName =="Anka")
                     }
                 });
        }
    }
}
