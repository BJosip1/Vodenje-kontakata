using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance
{
    public static class DbInitializer
    {
        public static void Seed(ContactDbContext context)
        {
            if (context.Contacts.Any())
                return;

            var friend = new Tag { Name = "Prijatelj" };
            var work = new Tag { Name = "Posao" };
            var supplier = new Tag { Name = "Dobavljač" };

            context.Tags.AddRange(friend, work, supplier);

            var zagrebAddress = new Address
            {
                Street = "Ilica 15",
                City = "Zagreb",
                PostalCode = "10000",
                Country = "Hrvatska"
            };

            var splitAddress = new Address
            {
                Street = "Velebitska 20",
                City = "Split",
                PostalCode = "21000",
                Country = "Hrvatska"
            };

            var contacts = new[]
            {
                new Contact
                {
                    FirstName = "Ana",
                    LastName = "Grcić",
                    Note = "Organizator maratona",
                    Address = zagrebAddress,
                    PhoneNumbers = { new PhoneNumber { Type = "Mobile", Value = "+385911416735" } },
                    Emails = { new Email { Type = "Personal", Value = "ana.grcic@gmail.com" } },
                    ContactTags = { new ContactTag { Tag = friend } }
                },
                new Contact
                {
                    FirstName = "Ivan",
                    LastName = "Bosnić",
                    Note = "Šef.",
                    Address = zagrebAddress,
                    PhoneNumbers =
                    {
                        new PhoneNumber { Type = "Mobile", Value = "+385952345678" },
                        new PhoneNumber { Type = "Work", Value = "+38521120461" }
                    },
                    Emails = { new Email { Type = "Work", Value = "ivan.bosnic@HRcloud.com" } },
                    ContactTags = { new ContactTag { Tag = work } }
                },
                new Contact
                {
                    FirstName = "Marko",
                    LastName = "Marić",
                    Note = "Dostavljač papira na poslu",
                    Address = splitAddress,
                    PhoneNumbers = { new PhoneNumber { Type = "Work", Value = "+38521122477" } },
                    Emails =
                    {
                        new Email { Type = "Work", Value = "marko@papirus.hr" },
                        new Email { Type = "Personal", Value = "marko.maric@hotmail.com" }
                    },
                    ContactTags = { new ContactTag { Tag = supplier } }
                },
                new Contact
                {
                    FirstName = "Petar",
                    LastName = "Radan",
                    Note = "Prijatelj s posla",
                    Address = splitAddress,
                    PhoneNumbers = { new PhoneNumber { Type = "Mobile", Value = "+385993834164" } },
                    Emails = { new Email { Type = "Personal", Value = "petar.radan@gmail.com" } },
                    ContactTags = { new ContactTag { Tag = friend }, new ContactTag { Tag = work } }
                }
            };

            context.Contacts.AddRange(contacts);
            context.SaveChanges();
        }
    }
}