using AutoMapper;
using FTMContextNet.Application.Mapping;
using FTMContextNet.Application.Services;
using FTMContextNet.Data;
using FTMContextNet.Data.Repositories;
using FTMContextNet.Data.Repositories.GedImports;
using FTMContextNet.Domain.Entities.NonPersistent;
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
    public void MappedInfo_ReturnedCorrectly_WhenServiceCalled()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new AutoMapperConfiguration());
        });

        var mapper = config.CreateMapper();

        this._mockPersistedCacheRepository
            .Setup(s => s.GetInfo())
            .Returns(new Info()
            {
                BadLocationsCount = 1,
                DupeEntryCount = 2,
                MarriagesCount = 3
            });

        var gis = new GetInfoService(_mockPersistedCacheRepository.Object, _mockLog.Object, mapper);

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

        mod[1].Selected.Should().Be(false);
        mod[1].UserId.Should().Be(2);

        mod.Count.Should().Be(2);
    }
}