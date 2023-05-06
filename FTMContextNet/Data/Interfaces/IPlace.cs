using FTMContextNet.Domain.Entities.Persistent.Cache;
using Microsoft.EntityFrameworkCore;
using PlaceLib.Model;

namespace FTMContextNet.Data.Interfaces
{
    public interface IPlace
    {
        DbSet<FtmPlaceCache> FTMPlaceCache { get; set; }

        int SaveChanges();
    }
}