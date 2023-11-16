using Microsoft.EntityFrameworkCore;
using PlaceLibNet.Domain.Entities.Persistent;

namespace PlaceLibNet.Data.Contexts;

public interface IPlacesContext
{
    DbSet<Places> Places { get; set; }
    DbSet<PlaceCache> PlaceCache { get; set; }

    void InsertPlaceCache(int id,
        string name,
        string nameFormatted,
        string jsonResult,
        string country,
        string county,
        bool searched,
        bool badData,
        string lat,
        string @long,
        string src);

    void UpdateFormattedName(int id, string name);
    void UpdatePlaceCacheLatLong(PlaceCache placeCache);
    void UpdateBadData(int id, bool badData);
    void UpdateJSONCacheResult(int id, string results);
    void UpdateCounty(int id, string county);
}