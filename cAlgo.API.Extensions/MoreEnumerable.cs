using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cAlgo.API.Extensions
{
    public static class MoreEnumerable
    {
        /// <summary>
        /// Returns a specified number of contiguous elements from the end of 
        /// a sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The sequence to return the last element of.</param>
        /// <param name="count">The number of elements to return.</param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> that contains the specified number of 
        /// elements from the end of the input sequence.
        /// </returns>
        /// <remarks>
        /// This operator uses deferred execution and streams its results.
        /// </remarks>
        /// <example>
        /// <code>
        /// int[] numbers = { 12, 34, 56, 78 };
        /// IEnumerable&lt;int&gt; result = numbers.TakeLast(2);
        /// </code>
        /// The <c>result</c> variable, when iterated over, will yield 
        /// 56 and 78 in turn.
        /// </example>

        public static IEnumerable<TSource> TakeLast<TSource>(this IEnumerable<TSource> source, int count)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (source.Count() == 0) throw new ArgumentException("source empty");

            return source.Skip(source.Count() - count);
        }
    }
}
