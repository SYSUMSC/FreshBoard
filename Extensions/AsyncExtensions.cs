using System.Collections.Generic;
using System.Threading.Tasks;

namespace FreshBoard.Extensions
{
    public static class AsyncExtensions
    {
        public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> enumerable)
        {
            var list = new List<T>();
            await foreach (var i in enumerable)
            {
                list.Add(i);
            }

            return list;
        }
    }
}
