using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Helpers
{
    // a generic class for paging users or messages etc...
    public class PagedList<T> : List<T>
    {
        // paging information will return to clients through a response header
        public int CurrentPage { get; set; } // represents number of clients requests
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; } // total number of db entities

        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            this.AddRange(items);
        }

        // PAGING: args(except 1st) will be sent by clients
        // CreateAsync() will create instance of this class along with initializing members
        // IQueryable allows us to apply filters before fetching data from db
        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize) 
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedList<T>(items, count, pageNumber, pageSize); 
        }
    }
}