using System.Collections.Generic;
using DatingApp.API.Models;
using Newtonsoft.Json;

namespace DatingApp.API.Data
{
    public class Seed
    {
        private readonly DataContext _context;

        public Seed(DataContext context)
        {
            _context = context;
        }

        public void SeedUsers() {
            var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
            //convert json text into desired User Class formate
            var users = JsonConvert.DeserializeObject<List<User>>(userData);
            foreach (var user in users)
            {
                // create password hash
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash ("password", out passwordHash, out passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                user.Username = user.Username.ToLower();

                _context.Users.Add(user);
            }

            _context.SaveChanges();
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            // use 'using()' for releasing all resources when HMAC finishes its tasks
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                // convert password into byte array in ComputeHash()
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                passwordSalt = hmac.Key;

            }
        }
    }
}