using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FakeRomanHsieh.API.Helper
{
    public class PaginationList<T>: List<T>
    {
        public int TotalPage { get; private set; }
        public int TotalCount { get; private set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPage;

        public int CurrentPage { get; set; }
        public int PageSize { get; set; }

        public PaginationList(int totalCount, int currentPage, int pageSize, List<T> Items)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            AddRange(Items);
            TotalCount = totalCount;
            TotalPage = (int)Math.Ceiling(totalCount / (double)pageSize);
        }

        public static async Task<PaginationList<T>> CreateAsync(int currentPage, int pageSize, IQueryable<T> result) {

            int totalCount = await result.CountAsync();
            //skip
            var skip = (currentPage - 1) * pageSize;
            result = result.Skip(skip);
            //take
            result = result.Take(pageSize);

            var items =  await result.ToListAsync();
            return new PaginationList<T>(totalCount, currentPage, pageSize, items);






            /**/
        }
    }
}
