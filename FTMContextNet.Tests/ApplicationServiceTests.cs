using AutoMapper;
using Castle.Components.DictionaryAdapter;
using FTMContextNet.Application.Mapping;
using FTMContextNet.Application.Services;
using FTMContextNet.Data;
using FTMContextNet.Data.Repositories;
using FTMContextNet.Data.Repositories.GedImports;
using FTMContextNet.Domain.Auth;
using FTMContextNet.Domain.Collections;
using FTMContextNet.Domain.Entities.NonPersistent;
using FTMContextNet.Domain.Entities.NonPersistent.Person;
using FTMContextNet.Domain.Entities.Persistent.Cache;
using LoggingLib;

namespace FTMContextNet.Tests;

public class ApplicationServiceTests
{
    private readonly Mock<IPersistedCacheRepository> _mockPersistedCacheRepository;
    private readonly Mock<IPersistedImportCacheRepository> _mockPersistedImportCacheRepository;
    private readonly Mock<IPersistedCacheContext> _mockPersistedCacheContext;
    private readonly Mock<Ilog> _mockLog;

    public ApplicationServiceTests()
    {
        this._mockPersistedCacheRepository = new Mock<IPersistedCacheRepository>();
        this._mockPersistedCacheContext = new Mock<IPersistedCacheContext>();
        this._mockPersistedImportCacheRepository = new Mock<IPersistedImportCacheRepository>();
        this._mockLog = new Mock<Ilog>();
    }

    [Fact]
    public void CreateDupes_CorrectDupes_WhenServiceCalled()
    {
      
        this._mockPersistedCacheRepository
            .Setup(s => s.GetComparisonPersons(1))
            .Returns(new List<PersonIdentifier>()
            {
                PersonIdentifier.Create(1,1500,1600,"a1","Sleaford","Jones","John"),
                PersonIdentifier.Create(2,1500,1600,"a2","Sleaford","Jones","John"),
                PersonIdentifier.Create(2,1500,1600,"o1","Grantham","Smith","John"),
                PersonIdentifier.Create(2,1500,1600,"o2","Grantham","Smitz","John")
            });

        var d = new DuplicateIgnoreList(new List<IgnoreList>()
        {
            new (){Id = 1,Person1 = "Smith", Person2 = "Smitz"}
        });

        this._mockPersistedCacheRepository
            .Setup(s => s.GetIgnoreList())
            .Returns(d);

       // this._mockPersistedCacheRepository
      //      .Setup(s => s.DeleteDupes());

        List<KeyValuePair<int, string>> x = null;

        this._mockPersistedCacheRepository
            .Setup(s => s.AddDupeEntrys(It.IsAny<List<KeyValuePair<int, string>>>(), It.IsAny<int>()))
            .Callback<List<KeyValuePair<int, string>>,int>((a, b) =>
            {
                x = a;
            });

        _mockPersistedImportCacheRepository
            .Setup(s => s.GetCurrentImportId())
            .Returns(1);

        var gis = new CreateDupeEntrys(_mockPersistedCacheRepository.Object, _mockPersistedImportCacheRepository.Object,   new Auth(), _mockLog.Object);

        gis.Execute();
        
        x.Should().HaveCount(2);

        x.First().Key.Should().Be(1);
        x.Last().Key.Should().Be(2);
    }

    [Fact]
    public void MappedInfo_ReturnedCorrectly_WhenServiceCalled()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new AutoMapperConfiguration());
        });

        var mapper = config.CreateMapper();

        this._mockPersistedCacheRepository
            .Setup(s => s.GetInfo(1))
            .Returns(new Info()
            {
                BadLocationsCount = 1,
                DupeEntryCount = 2,
                MarriagesCount = 3
            });

        var gis = new GetInfoService(_mockPersistedCacheRepository.Object, _mockLog.Object, mapper, new Auth());

        var mod =  gis.Execute();

        gis.Should().NotBeNull();

        mod.BadLocationsCount.Should().Be(1);
    }

    [Fact]
    public void GedFileInfo_ReturnedCorrectly_WhenServiceCalled()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new AutoMapperConfiguration());
        });

        var mapper = config.CreateMapper();

        var imports = new List<FTMImport>
        {
            new (){UserId =1, Selected =false, DateImported = "1 Jan 2023", FileName = "x", FileSize = 1, Id = 2},
            new (){UserId =2, Selected =true,DateImported = "2 Jan 2023", FileName = "x", FileSize = 1, Id = 1}
        };

        _mockPersistedImportCacheRepository
            .Setup(s => s.GetImportData())
            .Returns(imports);

        var gis = new GetGedFiles(_mockPersistedImportCacheRepository.Object, _mockLog.Object, mapper);

        var mod = gis.Execute();

        DateTime.TryParse("1 Jan 2023", out DateTime dt);

        mod[0].DateImported.Should().Be(dt);

        mod[1].Selected.Should().Be(true);
        mod[1].UserId.Should().Be(2);

        mod.Count.Should().Be(2);
    }
}