using LoggingLib;
using FTMContextNet.Data.Repositories;
using AutoMapper;
using GoogleMapsHelpers;
using System.Linq;

namespace FTMContextNet.Application.Services
{
    public class UpdatePlaceMetaData
    {
        private readonly Ilog _iLog;
        private readonly PersistedCacheRepository _persistedCacheRepository;
        private readonly PlacesRepository _placesRepository;
        private readonly IMapper _iMapper;

        public UpdatePlaceMetaData(PersistedCacheRepository persistedCacheRepository, PlacesRepository placesRepository, Ilog iLog, IMapper iMapper)
        {
            _iLog = iLog;
            _persistedCacheRepository = persistedCacheRepository;
            _placesRepository = placesRepository;
            _iMapper = iMapper;
        }

        public void Execute()
        {
            _iLog.WriteLine("Executing UpdatePlaceMetaData");

            var unset = _persistedCacheRepository.GetUnsetCountiesAndCountrys();
            
            _iLog.WriteLine("FTMPlaceCache Updating");

            _iLog.WriteLine("FTMPlaceCache has ~" + unset.Count + " records with no county or country");

            _iLog.WriteLine("FTMPlaceCache Setting");

            var unsetCounties = _persistedCacheRepository.GetUnsetUkCounties();

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

            _persistedCacheRepository.SavePersons();

            var unsetCountyCount = _persistedCacheRepository.GetUnsetUkCountiesCount();

            var unsetCountyCountJSONResult = _persistedCacheRepository.GetUnsetJsonResultCount();

            _iLog.WriteLine("FTMPlaceCache has ~" + unsetCountyCount + " records with no county or country");

            _iLog.WriteLine("FTMPlaceCache has ~" + unsetCountyCountJSONResult + " records with no JSON result");

            _iLog.WriteLine("FTMPlaceCache finished setting");
        }
    }
}
