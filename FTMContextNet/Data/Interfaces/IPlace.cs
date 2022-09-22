using FTMContextNet.Domain.Entities.Persistent.Cache;
using Microsoft.EntityFrameworkCore;

namespace FTMContextNet.Data.Interfaces
{
    public interface IPlace
    {
        DbSet<FtmPlaceCache> FTMPlaceCache { get; set; }

        int SaveChanges();
    }
}