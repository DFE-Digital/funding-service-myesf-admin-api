using Moq.Language.Flow;
using System.Collections.Generic;

namespace Pds.Admin.Services.Tests
{
    /// <summary>
    /// A moq helper.
    /// </summary>
    public static class MoqHelper
    {
        /// <summary>
        /// Moq returns in order extension.
        /// </summary>
        /// <typeparam name="T">The incoming type.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="setup">The moq setup class.</param>
        /// <param name="results">The set of results being returned in order.</param>
        public static void ReturnsInOrder<T, TResult>(this ISetup<T, TResult> setup, params TResult[] results)
            where T : class
        {
            setup.Returns(new Queue<TResult>(results).Dequeue);
        }

        /// <summary>
        /// Moq returns in order extension.
        /// </summary>
        /// <typeparam name="T">The incoming type.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="setup">The moq setup class.</param>
        /// <param name="results">The set of results being returned in order.</param>
        public static void ReturnsInOrder<T, TResult>(this ISetupGetter<T, TResult> setup, params TResult[] results)
            where T : class
        {
            setup.Returns(new Queue<TResult>(results).Dequeue);
        }
    }
}
