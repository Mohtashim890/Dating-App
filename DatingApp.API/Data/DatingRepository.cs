using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using DatingApp.API.Helpers;
using System;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _context;
        public DatingRepository(DataContext context)
        {
            _context = context;

        }
    
        // no Async methods b/c no specific table
        // data initially stored in memory until we call SaveAll() 
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);  // no need to specify table (ef will take care of it from T)
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);
            return photo;
        }


        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await _context.Photos.Where(u => u.UserId == userId).FirstOrDefaultAsync(p => p.IsMain);
        }

        public async Task<User> GetUser(int id)
        {
            // photos are consided as navigation properties so use include() to
            // use ICollection<Photos> property of User Class
            var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
           // var users = await _context.Users.Include(p => p.Photos).ToListAsync();
           // Paging: we have to defer execution(See PagedList.cs), convert output to IQueryable for filtering
           // and sorting. using LINQ include() to add(join tables) navigation property before querying
           var users = _context.Users.Include(p => p.Photos).OrderByDescending(u => u.LastActive).AsQueryable();
           // filter out users
           users = users.Where(u => u.Id != userParams.UserId);
           users = users.Where(u => u.Gender == userParams.Gender);
           if(userParams.Likers) // if someone like us
           {
               var userLikers = await GetUserLikes(userParams.UserId, userParams.Likers);
               users = users.Where(u => userLikers.Contains(u.Id));
           }
           if (userParams.Likees) // if we like someone
           {
                var userLikees = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(u => userLikees.Contains(u.Id));
           }
           if (userParams.MinAge != 18 || userParams.MaxAge != 99) // if not default values
           {
               var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
               var maxDob = DateTime.Today.AddYears(-userParams.MinAge);
               users = users.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <=maxDob);
           }

           // sorting users
           if (!string.IsNullOrEmpty(userParams.OrderBy))
           {
               switch (userParams.OrderBy)
               {
                case "created":
                    users =users.OrderByDescending(u => u.Created);
                    break;
                default:
                    users = users.OrderByDescending(u => u.LastActive);
                    break;
               }
           }

           // using static method CreateAsync(). specify its returntype from 'T' to 'User'
            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        // method for retrieving likees and likers from like table
        private async Task<IEnumerable<int>> GetUserLikes(int id, bool likers)
        {
            // retrive user with given id...join navigation props using include() so that we can access
            // tables via them???
            var user = await _context.Users.Include(x => x.Likers).Include(x => x.Likees)
                .FirstOrDefaultAsync(u => u.Id == id);
            // retrive likers (our id will be in LikeeId column) and likees(vice versa) from like table
            if (likers)
            {
                // Likers prop will give us Like objects so get id's using select()
                // by selecting LikerId from that object
                return user.Likers.Where(u => u.LikeeId == id).Select(i => i.LikerId);
            }
            else
            {
                return user.Likees.Where(u => u.LikerId == id).Select(i => i.LikeeId);
            }
        }

        public async Task<bool> SaveAll()
        {
            // to write models in context to database
            return await _context.SaveChangesAsync() > 0; // to make result boolean
        }

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            // firstordefault() returns like(likerid, likeeid) if exist 
            return await _context.Likes.FirstOrDefaultAsync(u => u.LikerId == userId && u.LikeeId == recipientId);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FirstOrDefaultAsync(m => m.Id == id); 
        }

        public async Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams)
        {
            // fetch messages table along with users and their photos
            var messages = _context.Messages.Include(s => s.Sender).ThenInclude(p => p.Photos)
                .Include(r => r.Recipient).ThenInclude(p => p.Photos).AsQueryable();
            
            // fetch messages for container
            // only fetch those whose Deleted prop set to false
            switch(messageParams.MessageContainer)
            {
                case "Inbox":
                    messages = messages.Where(m => m.RecipientId == messageParams.UserId && m.RecipientDeleted == false);
                    break;
                case "Outbox":
                    messages = messages.Where(m => m.SenderId == messageParams.UserId && m.SenderDeleted == false);
                    break;
                default: // for UnRead messages
                    messages = messages.Where(m => m.RecipientId == messageParams.UserId && m.IsRead == false && m.RecipientDeleted == false);
                    break;
            }
            messages = messages.OrderByDescending(m => m.MessageRead); // DateTime MessageRead == MessageSent
            return await PagedList<Message>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId)
        {
            // fetch complete conversation messages
            var messages = await _context.Messages.Include(s => s.Sender).ThenInclude(p => p.Photos)
                .Include(r => r.Recipient).ThenInclude(p => p.Photos)
                .Where(m => m.RecipientId == userId && m.RecipientDeleted == false && m.SenderId == recipientId
                    || m.RecipientId == recipientId && m.SenderId == userId && m.SenderDeleted == false)
                .OrderByDescending(m => m.MessageRead)
                .ToListAsync();
            return messages;
        }
    }
}