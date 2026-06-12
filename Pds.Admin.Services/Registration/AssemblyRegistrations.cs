using Pds.Admin.Services.Implementations.Adapters;
using Pds.Admin.Services.Implementations.Configuration;
using Pds.Admin.Services.Implementations.Coordinators;
using Pds.Admin.Services.Interfaces.Adapters;
using Pds.Admin.Services.Interfaces.Configuration;
using Pds.Admin.Services.Interfaces.Coordinators;
using Pds.Services.Common.Registration.Attributes;

// inherited, package level
// [assembly: ExternalRegistration(typeof(ILoggerHelper), typeof(LoggerHelper), TypeOfRegistrationScope.Singleton)]

// Project level
// Adapters
[assembly: InternalRegistration(typeof(IAdaptNotifyDetails), typeof(NotifyDetailsAdapter), TypeOfRegistrationScope.Singleton)]

// Coordinators
[assembly: InternalRegistration(typeof(ICoordinateViewAsProviderDetails), typeof(ViewAsProviderDetailsCoordinator), TypeOfRegistrationScope.Singleton)]
[assembly: InternalRegistration(typeof(ICoordinateNotifyDetails), typeof(NotifyDetailsCoordinator), TypeOfRegistrationScope.Singleton)]

// Configuration
[assembly: ConfigurationRegistration(typeof(IConfigureCacheStorage), typeof(CacheStorageConfiguration), "StorageCache")]
