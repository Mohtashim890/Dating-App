using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;

namespace DatingApp.API.Data
{
    public interface IDatingRepository
    {
         // declaration of generic functions (for photo or user)
       void Add<T> (T entity) where T: class;

       void Delete<T> (T entity) where T: class;

       Task<bool> SaveAll();

       //Task<IEnumerable<User>> GetUsers();
       Task<PagedList<User>> GetUsers(UserParams userParams);

       Task<User> GetUser(int id);

       Task<Photo> GetPhoto(int id);

       Task<Photo> GetMainPhotoForUser(int userId);

       Task<Like> GetLike(int userId, int recipientId); // to check if like already exist

       Task<Message> GetMessage(int id);
       Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams); // inbox, outbox, unread messages
       Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId); // conversation b/w 2 users
       

    }
}