using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MockQueryable.Moq;
using Moq;
using VirtoCommerce.DynamicAssociationsModule.Core.Model.Search;
using VirtoCommerce.DynamicAssociationsModule.Data.Model;
using VirtoCommerce.DynamicAssociationsModule.Data.Repositories;
using VirtoCommerce.DynamicAssociationsModule.Data.Search;
using VirtoCommerce.DynamicAssociationsModule.Data.Services;
using VirtoCommerce.Platform.Caching;
using VirtoCommerce.Platform.Core.Events;
using Xunit;

namespace VirtoCommerce.DynamicAssociationsModule.Tests
{
    public class DynamicAssociationSearchServiceTests
    {
        public DynamicAssociationSearchServiceTests()
        {
        }

        public static object[][] ValidEntitiesForIsActive => new object[][]
        {
            new object[]
            {
                new DynamicAssociationEntity()
                {
                    Id = Guid.NewGuid().ToString(),
                    IsActive = true,
                    StartDate = null,
                    EndDate = null,
                },
            },
            new object[]
            {
                new DynamicAssociationEntity()
                {
                    Id = Guid.NewGuid().ToString(),
                    IsActive = true,
                    StartDate = DateTime.UtcNow.AddDays(-1),
                    EndDate = null,
                },
            },
            new object[]
            {
                new DynamicAssociationEntity()
                {
                    Id = Guid.NewGuid().ToString(),
                    IsActive = true,
                    StartDate = null,
                    EndDate = DateTime.UtcNow.AddDays(1),
                },
            },
            new object[]
            {
                new DynamicAssociationEntity()
                {
                    Id = Guid.NewGuid().ToString(),
                    IsActive = true,
                    StartDate = DateTime.UtcNow.AddDays(-1),
                    EndDate = DateTime.UtcNow.AddDays(1),
                },
            },
        };

        public static object[][] NotValidEntitiesForIsActive => new object[][]
        {
            new object[]
            {
                new DynamicAssociationEntity()
                {
                    Id = Guid.NewGuid().ToString(),
                    IsActive = false,
                },
            },
            new object[]
            {
                new DynamicAssociationEntity()
                {
                    Id = Guid.NewGuid().ToString(),
                    IsActive = true,
                    StartDate = DateTime.UtcNow.AddDays(1),
                    EndDate = null,
                }
            },
            new object[]
            {
                new DynamicAssociationEntity()
                {
                    Id = Guid.NewGuid().ToString(),
                    IsActive = true,
                    StartDate = null,
                    EndDate = DateTime.UtcNow.AddDays(-1),
                }
            },
            new object[]
            {
                new DynamicAssociationEntity()
                {
                    Id = Guid.NewGuid().ToString(),
                    IsActive = true,
                    StartDate = DateTime.UtcNow.AddDays(1),
                    EndDate = DateTime.UtcNow.AddDays(-1),
                }
            },
        };

        [Theory]
        [MemberData(nameof(ValidEntitiesForIsActive))]
        public async Task SearchDynamicAssociationsAsync_IsActiveConditionValid_FoundResults(DynamicAssociationEntity entity)
        {
            // Arrange
            var entities = new[]
            {
                entity
            };
            var searchServiceMock = CreateDynamicAssociationSearchServiceMock(entities);

            // Act
            var searchResult = await searchServiceMock.SearchDynamicAssociationsAsync(new DynamicAssociationSearchCriteria() { IsActive = true });

            // Assert
            Assert.Equal(1, searchResult.TotalCount);
        }

        [Theory]
        [MemberData(nameof(NotValidEntitiesForIsActive))]
        public async Task SearchDynamicAssociationsAsync_IsActiveConditionNotValid_NoResults(DynamicAssociationEntity entity)
        {
            // Arrange
            var entities = new[]
            {
                entity
            };
            var searchServiceMock = CreateDynamicAssociationSearchServiceMock(entities);

            // Act
            var searchResult = await searchServiceMock.SearchDynamicAssociationsAsync(new DynamicAssociationSearchCriteria() { IsActive = true });

            // Assert
            Assert.Equal(0, searchResult.TotalCount);
        }


        private DynamicAssociationSearchService CreateDynamicAssociationSearchServiceMock(IEnumerable<DynamicAssociationEntity> entities)
        {
            var dynamicAssociationsRepositoryFactory = CreateRepositoryMock(entities);
            var platformMemoryCache = GetPlatformMemoryCache();
            var eventPublisherMock = new Mock<IEventPublisher>();

            var dynamicAssociationsService = new DynamicAssociationService(dynamicAssociationsRepositoryFactory, platformMemoryCache, eventPublisherMock.Object);
            var result = new DynamicAssociationSearchService(dynamicAssociationsRepositoryFactory, platformMemoryCache, dynamicAssociationsService);

            return result;
        }

        private static Func<IDynamicAssociationsRepository> CreateRepositoryMock(IEnumerable<DynamicAssociationEntity> entities)
        {
            var dynamicAssociationsRepositoryMock = new Mock<IDynamicAssociationsRepository>();
            var entitiesMock = entities.AsQueryable().BuildMock();

            dynamicAssociationsRepositoryMock.Setup(x => x.DynamicAssociations)
                .Returns(entitiesMock.Object);
            dynamicAssociationsRepositoryMock.Setup(x => x.GetDynamicAssociationsByIdsAsync(It.IsAny<string[]>()))
                .Returns<string[]>(ids =>
                    Task.FromResult(entities.Where(x => ids.Contains(x.Id)).ToArray()));

            IDynamicAssociationsRepository func() => dynamicAssociationsRepositoryMock.Object;

            return func;
        }

        private static PlatformMemoryCache GetPlatformMemoryCache()
        {
            var memoryCache = new MemoryCache(Options.Create(new MemoryCacheOptions()));
            var platformMemoryCache = new PlatformMemoryCache(memoryCache, Options.Create(new CachingOptions()), new Mock<ILogger<PlatformMemoryCache>>().Object);
            return platformMemoryCache;
        }
    }
}
