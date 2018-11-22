using System;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        // 1st. inject DataContext in this repository using constructor
        // 2nd. Convert password into passwordHash and passwordSalt
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context = context;

        }
        // Login and Register methods are threads that directly access databse
        // via EF functions
        public async Task<User> Login(string username, string password)
        {
            // fetch data from db upon username match
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);

            if(user == null)
                return null;
            if(!VerifyPasswordHash(password,user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            // use 'using()' for releasing all resources when HMAC finishes its tasks
            using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                // convert password into byte array in ComputeHash()
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if( computedHash[i] != passwordHash[i]) return false;
                }

            }
            return true;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            // for pass by reference use 'out'
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            // set user instance members
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            // parse 'user' to ef via DataContext (_context)
            await _context.Users.AddAsync(user);
            //save changes in DB
            await _context.SaveChangesAsync();

            return user;
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

        public async Task<bool> UserExist(string username)
        {
            if(await _context.Users.AnyAsync(x => x.Username == username))
                return true;
            
            return false;
        }
    }
}