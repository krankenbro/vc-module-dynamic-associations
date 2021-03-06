using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using VirtoCommerce.DynamicAssociationsModule.Core.Model;
using VirtoCommerce.DynamicAssociationsModule.Data.Model;
using VirtoCommerce.DynamicAssociationsModule.Data.Repositories;
using VirtoCommerce.DynamicAssociationsModule.Data.Services;
using VirtoCommerce.Platform.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Core.Events;
using Xunit;

namespace VirtoCommerce.DynamicAssociationsModule.Tests
{
    public class AssociationServiceTests
    {
        private readonly Mock<IEventPublisher> _eventPublisherMock;
        private readonly Mock<IAssociationsRepository> _dynamicAssociationsRepository;
        private readonly Mock<IUnitOfWork> _unityOfWorkMock;

        public AssociationServiceTests()
        {
            _eventPublisherMock = new Mock<IEventPublisher>();
            _dynamicAssociationsRepository = new Mock<IAssociationsRepository>();
            _unityOfWorkMock = new Mock<IUnitOfWork>();
        }

        [Fact]
        public async Task GetByIdsAsync_GetThenSaveDynamicAssociation_AreNotEquals()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();
            var dynamicAssociation = new Association { Id = id, };
            var dynamicAssociationEntity = AbstractTypeFactory<AssociationEntity>.TryCreateInstance().FromModel(dynamicAssociation, new PrimaryKeyResolvingMap());

            var dynamicAssociationService = CreateDynamicAssociationService();
            _dynamicAssociationsRepository.Setup(x => x.Add(dynamicAssociationEntity)).Callback(() =>
            {
                _dynamicAssociationsRepository
                    .Setup(x => x.GetAssociationsByIdsAsync(new[] { id }))
                    .ReturnsAsync(new[] { dynamicAssociationEntity });
            });

            // Act
            var nullDynamicAssociation = await dynamicAssociationService.GetByIdsAsync(new[] { id });
            await dynamicAssociationService.SaveChangesAsync(new[] { dynamicAssociation });
            var dynamicAssociationFromService = await dynamicAssociationService.GetByIdsAsync(new[] { id });

            // Assert
            Assert.NotEqual(nullDynamicAssociation, dynamicAssociationFromService);
        }


        private AssociationService CreateDynamicAssociationService()
        {
            var memoryCache = new MemoryCache(Options.Create(new MemoryCacheOptions()));
            var platformMemoryCache = new PlatformMemoryCache(memoryCache, Options.Create(new CachingOptions()), new Mock<ILogger<PlatformMemoryCache>>().Object);
            _dynamicAssociationsRepository.Setup(x => x.UnitOfWork).Returns(_unityOfWorkMock.Object);

            var result = new AssociationService(() => _dynamicAssociationsRepository.Object, platformMemoryCache, _eventPublisherMock.Object);

            return result;
        }
    }
}
