
using LoggingLib;
using AutoMapper;
using GoogleMapsHelpers;
using System.Linq;
using PlaceLibNet;
using PlaceLibNet.Data.Repositories;

namespace FTMContextNet.Application.Services
{
    public class UpdatePlaceMetaData
    {
        private readonly Ilog _iLog;
        private readonly FtmPlaceCacheRepository _persistedCacheCacheRepository;
        private readonly PlacesRepository _placesRepository;
        private readonly IMapper _iMapper;

        public UpdatePlaceMetaData(FtmPlaceCacheRepository persistedCacheCacheRepository, PlacesRepository placesRepository, Ilog iLog, IMapper iMapper)
        {
            _iLog = iLog;
            _persistedCacheCacheRepository = persistedCacheCacheRepository;
            _placesRepository = placesRepository;
            _iMapper = iMapper;
        }

        public void Execute()
        {
            _iLog.WriteLine("Executing UpdatePlaceMetaData");

            var unset = _persistedCacheCacheRepository.GetUnsetCountiesAndCountrys();
            
            _iLog.WriteLine("FTMPlaceCache Updating");

            _iLog.WriteLine("FTMPlaceCache has ~" + unset.Count + " records with no county or country");

            _iLog.WriteLine("FTMPlaceCache Setting");

            var unsetCounties = _persistedCacheCacheRepository.GetUnsetUkCounties();

            int foreignCountrysCount = unsetCounties.Count(w => !w.LocationInfo.IsUK());

            _iLog.WriteLine("FTMPlaceCache " + foreignCountrysCount + " non UK records");

            foreach (var emptyCounty in unsetCounties)
            {
                if (emptyCounty.ProspectivePostal())
                {
                    emptyCounty.Place.County = _placesRepository.SearchPlacesDBForCounty(emptyCounty.LocationInfo.PostalTown);
                }

                if (emptyCounty.ProspectivePolitical())
                {
                    if (EnglishHistoricCountyList.Get.Contains(emptyCounty.LocationInfo.Political))
                    {
                        emptyCounty.Place.County = emptyCounty.LocationInfo.Political;
                    }

                    if (emptyCounty.Place.County == "")
                        emptyCounty.Place.County = _placesRepository.SearchPlacesDBForCounty(emptyCounty.LocationInfo.Political);
                }
            }

            _persistedCacheCacheRepository.SaveChanges();

            var unsetCountyCount = _persistedCacheCacheRepository.GetUnsetUkCountiesCount();

            var unsetCountyCountJSONResult = _persistedCacheCacheRepository.GetUnsetJsonResultCount();

            _iLog.WriteLine("FTMPlaceCache has ~" + unsetCountyCount + " records with no county or country");

            _iLog.WriteLine("FTMPlaceCache has ~" + unsetCountyCountJSONResult + " records with no JSON result");

            _iLog.WriteLine("FTMPlaceCache finished setting");
        }
    }
}
