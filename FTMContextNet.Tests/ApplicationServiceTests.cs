using AutoMapper;
using FTMContextNet.Application.Mapping;
using FTMContextNet.Application.UserServices.CreateDuplicateList;
using FTMContextNet.Application.UserServices.GetGedList;
using FTMContextNet.Application.UserServices.GetInfoList;
using FTMContextNet.Data;
using FTMContextNet.Data.Repositories.GedImports;
using MSGIdent;
using FTMContextNet.Domain.Collections;
using FTMContextNet.Domain.Commands;
using FTMContextNet.Domain.Entities.NonPersistent;
using FTMContextNet.Domain.Entities.NonPersistent.Person;
using FTMContextNet.Domain.Entities.Persistent.Cache;
using LoggingLib;
using FTMContextNet.Data.Repositories.TreeAnalysis;

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
    public void CreateDupes_CorrectDupes2_WhenServiceCalled()
    {

        this._mockPersistedCacheRepository
            .Setup(s => s.GetComparisonPersons(0))
            .Returns(new List<PersonIdentifier>()
            {
                PersonIdentifier.Create(3,1798,1798,"|22.5|Ketchum","lincolnshire",0,0,"Waterson","William"),
                PersonIdentifier.Create(1173,1795,1795,"|00|Ballam|wakerley","lincolnshire",0,0,"Watterson","William"),
                PersonIdentifier.Create(31961,1861,1861,"|22|saritajones","nottinghamshire",0,0,"Gosling","Robert"),
                
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
            .Callback<List<KeyValuePair<int, string>>, int>((a, b) =>
            {
                x = a;
            });

        _mockPersistedImportCacheRepository
            .Setup(s => s.GetCurrentImportId())
            .Returns(1);

        var gis = new CreateDupeEntrys(_mockPersistedCacheRepository.Object, _mockPersistedImportCacheRepository.Object, new Auth(), _mockLog.Object);

        gis.Handle(new CreateDuplicateListCommand(), new CancellationToken(false)).Wait();

        x.Should().HaveCount(2);

        x.First().Key.Should().Be(1);
        x.Last().Key.Should().Be(2);
    }

    [Fact]
    public void CreateDupes_CorrectDupes_WhenServiceCalled()
    {
      
        this._mockPersistedCacheRepository
            .Setup(s => s.GetComparisonPersons(0))
            .Returns(new List<PersonIdentifier>()
            {
                PersonIdentifier.Create(1,1500,1600,"a1","Lincolnshire",0,0,"Jones","John"),
                PersonIdentifier.Create(2,1500,1600,"a2","Lincolnshire",0,0,"Jones","John"),
                PersonIdentifier.Create(2,1500,1600,"o1","Nottinghamshire",0,0,"Smith","John"),
                PersonIdentifier.Create(2,1500,1600,"o2","Nottinghamshire",0,0,"Smitz","John")
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

        gis.Handle(new CreateDuplicateListCommand(),new CancellationToken(false)).Wait();
        
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

        var mod =  gis.Handle(new GetInfoServiceQuery(),new CancellationToken(false));

        gis.Should().NotBeNull();

        mod.Result.BadLocationsCount.Should().Be(1);
    }

    [Fact]
    public void GedFileInfo_ReturnedCorrectly_WhenServiceCalled()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new AutoMapperConfiguration());
        });

        var mapper = config.CreateMapper();

        var imports = new List<TreeImport>
        {
            new (){UserId =1, Selected =false, DateImported = "1 Jan 2023", FileName = "x", FileSize = "1", Id = 2},
            new (){UserId =2, Selected =true,DateImported = "2 Jan 2023", FileName = "x", FileSize = "1", Id = 1}
        };

        _mockPersistedImportCacheRepository
            .Setup(s => s.GetImportData())
            .Returns(imports);

        var gis = new GetGedFiles(_mockPersistedImportCacheRepository.Object, _mockLog.Object, mapper);

        var mod = gis.Handle(new GetGedFilesQuery(), new CancellationToken(false)).Result;

        DateTime.TryParse("1 Jan 2023", out DateTime dt);

        mod[0].DateImported.Should().Be(dt);

        mod[1].Selected.Should().Be(true);
        mod[1].UserId.Should().Be(2);

        mod.Count.Should().Be(2);
    }
}