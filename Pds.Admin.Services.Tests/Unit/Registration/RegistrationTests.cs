using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pds.Admin.Services.Interfaces.Coordinators;
using Pds.Services.Common.Registration;
using Pds.Services.Common.Registration.Attributes;
using System.Linq;
using System.Reflection;

namespace Pds.Admin.Services.Tests.Unit.Registration
{
    /// <summary>
    /// Test (the) registrations in the module.
    /// This won't test the startup class
    /// but we now have some assurance that the registrations
    /// and the 'exported' classes are in sync.
    /// </summary>
    [TestClass]
    public sealed class RegistrationTests
    {
        [TestMethod]
        public void TestServiceRegistrations()
        {
            // arrange
            var assembly = Assembly.GetAssembly(typeof(ICoordinateViewAsProviderDetails));

            // act
            var types = assembly.GetTypes()
                .Where(x => x.IsClass && typeof(IRequireServiceRegistration).IsAssignableFrom(x));
            var registrations = assembly.GetCustomAttributes<InternalRegistrationAttribute>();

            // assert
            // the product of the two differences, we expect this set to be empty
            var candidates =
                registrations.Select(x => x.ImplementationType.FullName).Except(types.Select(x => x.FullName))
                .Union(
                    types.Select(x => x.FullName).Except(registrations.Select(x => x.ImplementationType.FullName)));
            var missing = string.Join("\n\t", candidates);

            types.Count().Should().Be(registrations.Count(), $"missing are:\n\t{missing}\n");
        }

        [TestMethod]
        public void TestConfigurationRegistrations()
        {
            // arrange
            var assembly = Assembly.GetAssembly(typeof(ICoordinateViewAsProviderDetails));

            // act
            var types = assembly.GetTypes()
                .Where(x => x.IsClass && typeof(IRequireConfigurationRegistration).IsAssignableFrom(x));
            var registrations = assembly.GetCustomAttributes<ConfigurationRegistrationAttribute>();

            // assert
            // the product of the two differences, we expect this set to be empty
            var candidates =
                registrations.Select(x => x.ImplementationType.FullName).Except(types.Select(x => x.FullName))
                .Union(
                    types.Select(x => x.FullName).Except(registrations.Select(x => x.ImplementationType.FullName)));
            var missing = string.Join("\n\t", candidates);

            types.Count().Should().Be(registrations.Count(), $"missing are:\n\t{missing}\n");
        }

        [TestMethod]
        public void TestFaultRegistrations()
        {
            // arrange
            var assembly = Assembly.GetAssembly(typeof(ICoordinateViewAsProviderDetails));

            // act
            var registrations = assembly.GetCustomAttributes<FaultResponseRegistrationAttribute>();

            //assert
            registrations.Count().Should().Be(0); // <= at the moment...
        }
    }
}
