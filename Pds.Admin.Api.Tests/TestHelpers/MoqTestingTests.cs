using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Reflection;

namespace Pds.Admin.Api.Tests
{
    public abstract class MoqTestingTests<TTestSystem>
    {
        /// <summary>
        /// Make strict mock.
        /// </summary>
        /// <typeparam name="TEntity">for this type.</typeparam>
        /// <returns>a strict behaviour mock.</returns>
        public TEntity MakeStrictMock<TEntity>()
            where TEntity : class =>
            new Mock<TEntity>(MockBehavior.Strict).Object;

        /// <summary>
        /// Get mock.
        /// </summary>
        /// <typeparam name="TEntity">the type.</typeparam>
        /// <param name="forItem">for this instance of <typeparamref name="TEntity"/>the type.</param>
        /// <returns>the mock.</returns>
        public Mock<TEntity> GetMock<TEntity>(TEntity forItem)
            where TEntity : class =>
            Mock.Get(forItem);

        /// <summary>
        /// The system under test must support the given contract.
        /// </summary>
        /// <typeparam name="TContract">the given contract type.</typeparam>
        public void TestSystemSupportsGivenContract<TContract>() =>
            BuildTestSystem().Should().BeAssignableTo<TContract>();

        /// <summary>
        /// very all mocks.
        /// </summary>
        /// <param name="sut">the system under test.</param>
        internal abstract void VerifyAllMocks(TTestSystem sut);

        /// <summary>
        /// Build the 'system under test'.
        /// </summary>
        /// <returns>the system under test.</returns>
        internal abstract TTestSystem BuildTestSystem();

        /// <summary>
        /// Make readonly items,  helper function to create list of things.
        /// </summary>
        /// <typeparam name="TItem">the type of item.</typeparam>
        /// <param name="count">the number of items required.</param>
        /// <param name="candidate">a candidate item for the list.</param>
        /// <returns>a collection of items for the test.</returns>
        internal IReadOnlyCollection<TItem> MakeReadonlyItems<TItem>(int count, TItem candidate = null)
            where TItem : class
        {
            var items = new TItem[count];
            for (var i = 0; i < count; i++)
            {
                items[i] = candidate;
            }

            return items;
        }

        /// <summary>
        /// Make readonly items,  helper function to create list of things.
        /// </summary>
        /// <typeparam name="TItem">the type of item.</typeparam>
        /// <param name="count">the number of items required.</param>
        /// <param name="candidate">a candidate item for the list.</param>
        /// <returns>a collection of items for the test.</returns>
        internal IReadOnlyCollection<TItem> MakeReadonlyItems<TItem>(int count, TItem? candidate = null)
            where TItem : struct
        {
            var items = new TItem[count];
            for (var i = 0; i < count; i++)
            {
                items[i] = candidate.GetValueOrDefault();
            }

            return items;
        }

        /// <summary>
        /// Make enumerable items,  helper function to create list of things.
        /// </summary>
        /// <typeparam name="TItem">the type of item.</typeparam>
        /// <param name="count">the number of items rquired.</param>
        /// <param name="candidate">a candidate item for the list.</param>
        /// <returns>a collection of items for the test.</returns>
        internal IEnumerable<TItem> MakeEnumerableItems<TItem>(int count, TItem candidate = null)
            where TItem : class
        {
            var items = new TItem[count];
            for (var i = 0; i < count; i++)
            {
                items[i] = candidate;
            }

            return items;
        }

        /// <summary>
        /// Get (the) assembly for...
        /// </summary>
        /// <typeparam name="TTarget">The target type.</typeparam>
        /// <returns>The assembly.</returns>
        internal Assembly GetAssemblyfor<TTarget>() =>
            typeof(TTarget).Assembly;
    }

#pragma warning disable SA1402 // File may only contain a single type
    public abstract class MoqTestingTests<TTestSystem, TSystemSupports>
#pragma warning restore SA1402 // File may only contain a single type
        : MoqTestingTests<TTestSystem>
    {
        /// <summary>
        /// SUT supports registration contract.
        /// </summary>
        [TestMethod]
        public void TestSystemSupportsRegistrationContract() =>
            TestSystemSupportsGivenContract<TSystemSupports>();
    }
}