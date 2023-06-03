using FTMContextNet.Data;
using FTMContextNet.Data.Repositories;
using FTMContextNet.Domain.Entities.Persistent.Cache;
using LoggingLib;
using Moq.EntityFrameworkCore;
using QuickGed.Types;

namespace FTMContextNet.Tests
{
    public class PersistedCacheRepositoryTests
    {
        private readonly Mock<IPersistedCacheRepository> _mockPersistedCacheRepository;
        private readonly Mock<IPersistedCacheContext> _mockPersistedCacheContext;
        private readonly Mock<Ilog> _mockLog;

        public PersistedCacheRepositoryTests()
        {
            this._mockPersistedCacheRepository = new Mock<IPersistedCacheRepository>();
            this._mockPersistedCacheContext = new Mock<IPersistedCacheContext>();
            this._mockLog = new Mock<Ilog>();
        }


        [Fact]
        public void GetRelationships_ReturnsAllRelations_WhenSeeded()
        {
            var testData = new List<FTMPersonView>
            {
                new() {Id = 0,FirstName = "",Surname = "_20_Jones", RootPerson = true,LinkNode = false},
                new() {Id = 1,FirstName = "",Surname = "_21_Smith", RootPerson = true,LinkNode = false},
                new() {Id = 2,FirstName = "",Surname = "group common names", RootPerson = false,LinkNode = true}

            };

            var relations = new List<FTMMarriage>
            {
                new(){Id = 1,GroomId = 0,BrideId = 2},
                new(){Id = 2,GroomId = 1,BrideId = 2},
            };

            this._mockPersistedCacheContext
                .Setup(s => s.FTMPersonView)
                .ReturnsDbSet(testData);

            this._mockPersistedCacheContext
                .Setup(s => s.FTMMarriages)
                .ReturnsDbSet(relations);

            IPersistedCacheRepository cacheRepository =
                new PersistedCacheRepository(this._mockPersistedCacheContext.Object, this._mockLog.Object);

            var results = cacheRepository.GetRelationships();

            results.Should().HaveCount(2);

            results[0].Person1Id.Should().Be(relations[0].GroomId);
            results[0].Person2Id.Should().Be(relations[0].BrideId);

            results[1].Person1Id.Should().Be(relations[1].GroomId);
            results[1].Person2Id.Should().Be(relations[1].BrideId);
        }


        [Fact]
        public void GetGroupNames_ReturnsAllGroupNames_WhenSeeded()
        {
            var testData = new List<FTMPersonView>
            {
                new() {Id = 0,FirstName = "",Surname = "_20_Jones", RootPerson = true,LinkNode = false},
                new() {Id = 1,FirstName = "John",Surname = "Smith", RootPerson = true,LinkNode = true},
                new() {Id = 2,FirstName = "",Surname = "group common names", RootPerson = false,LinkNode = true}
            };
             
            _mockPersistedCacheContext
                .Setup(s => s.FTMPersonView)
                .ReturnsDbSet(testData);

            var cacheRepository =
                new PersistedCacheRepository(this._mockPersistedCacheContext.Object, this._mockLog.Object);

            var results = cacheRepository.GetGroupNamesDictionary();

            results.Should().HaveCount(2);

            results.Keys.Contains(1).Should().BeTrue();
            results.Keys.Contains(2).Should().BeTrue();

            results[1].Should().Be("John Smith");
        }


        [Fact]
        public void GetTreeIds_ReturnsAllTreeIds_WhenSeeded()
        {
            var testData = new List<FTMPersonView>
            {
                new() {Id = 0,FirstName = "",Surname = "_20_Jones", RootPerson = true,LinkNode = false},
                new() {Id = 1,FirstName = "John",Surname = "Smith", RootPerson = false,LinkNode = true},
                new() {Id = 2,FirstName = "",Surname = "common names", RootPerson = true,LinkNode = true}
            };

            _mockPersistedCacheContext
                .Setup(s => s.FTMPersonView)
                .ReturnsDbSet(testData);

            var cacheRepository =
                new PersistedCacheRepository(this._mockPersistedCacheContext.Object, this._mockLog.Object);

            var results = cacheRepository.GetTreeIds();

            results.Should().HaveCount(2);

            results[0].Should().Be(0);
            results[0].Should().Be(2);
        }


        [Fact]
        public void GetRootNameDictionary_ReturnsAllRootNames_WhenSeeded()
        {
            ////var nameDict = GetRootNameDictionary();
            //this._mockPersistedCacheRepository
            //    .Setup(s => s.GetRootNameDictionary())
            //    .Returns(rootNames);
            var testData = new List<FTMPersonView>
            {
                new() {Id = 0,FirstName = "",Surname = "_20_Jones", RootPerson = true,LinkNode = false},
                new() {Id = 1,FirstName = "John",Surname = "Smith", RootPerson = true,LinkNode = true},
                new() {Id = 2,FirstName = "",Surname = "group common names", RootPerson = false,LinkNode = true}
            };

            _mockPersistedCacheContext
                .Setup(s => s.FTMPersonView)
                .ReturnsDbSet(testData);

            var cacheRepository =
                new PersistedCacheRepository(this._mockPersistedCacheContext.Object, this._mockLog.Object);

            var results = cacheRepository.GetRootNameDictionary();

            results.Should().HaveCount(2);

            results.Keys.Contains(0).Should().BeTrue();
            results.Keys.Contains(1).Should().BeTrue();

            results[0].Should().Be("_20_Jones");
        }

        [Fact]
        public void GetGroups_ReturnsAllGroups_WhenSeeded()
        {
          
            var testData = new List<FTMPersonView>
            {
                new() {Id = 0,FirstName = "",Surname = "_20_Jones", RootPerson = true,LinkNode = false},
                new() {Id = 1,FirstName = "",Surname = "_21_Smith", RootPerson = true,LinkNode = false},
                new() {Id = 2,FirstName = "",Surname = "group common names", RootPerson = false,LinkNode = true}

            };

            var relations = new List<FTMMarriage>
            {
                new(){Id = 1,GroomId = 0,BrideId = 2},
                new(){Id = 2,GroomId = 1,BrideId = 2},
            };

            this._mockPersistedCacheContext
                .Setup(s => s.FTMPersonView)
                .ReturnsDbSet(testData);

            this._mockPersistedCacheContext
                .Setup(s => s.FTMMarriages)
                .ReturnsDbSet(relations);



            //// Create instance of ITestClass implementation you want to use
            var inst = new PersistedCacheRepository(this._mockPersistedCacheContext.Object, _mockLog.Object);

            //// Setup to call method of an actual instance
            //// if method returns void use mock.Setup(...).Callback(...)
            _mockPersistedCacheRepository.Setup(m => m.GetGroups())
                .Returns(() => inst.GetGroups());

            var result = _mockPersistedCacheRepository.Object.GetGroups();


        }
    }
}