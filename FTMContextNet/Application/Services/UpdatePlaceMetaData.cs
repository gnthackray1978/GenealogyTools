
using LoggingLib;
using AutoMapper;
using GoogleMapsHelpers;
using System.Linq;
using PlaceLibNet.Data.Repositories;

namespace FTMContextNet.Application.Services
{
    public class UpdatePlaceMetaData
    {
        private readonly Ilog _iLog;
        private readonly PlaceRepository _placeRepository;
        private readonly IMapper _iMapper;

        public UpdatePlaceMetaData(PlaceRepository placeRepository,Ilog iLog, IMapper iMapper)
        {
            _iLog = iLog;
            _placeRepository = placeRepository;
            _iMapper = iMapper;
        }

        public void Execute()
        {
            _iLog.WriteLine("Executing UpdatePlaceMetaData");

            var unset = _placeRepository.GetUnsetCountiesAndCountrys();
            
            _iLog.WriteLine("PlaceCache Updating");

            _iLog.WriteLine("PlaceCache has ~" + unset.Count + " records with no county or country");

            _iLog.WriteLine("PlaceCache Setting");

            var unsetCounties = _placeRepository.GetUnsetUkCounties();

            int foreignCountrysCount = unsetCounties.Count(w => !w.LocationInfo.IsUK());

            _iLog.WriteLine("PlaceCache " + foreignCountrysCount + " non UK records");

            foreach (var emptyCounty in unsetCounties)
            {
                if (emptyCounty.ProspectivePostal())
                {
                    emptyCounty.Place.County = _placeRepository.SearchPlacesDBForCounty(emptyCounty.LocationInfo.PostalTown);
                }

                if (emptyCounty.ProspectivePolitical())
                {
                    if (EnglishHistoricCountyList.Get.Contains(emptyCounty.LocationInfo.Political))
                    {
                        emptyCounty.Place.County = emptyCounty.LocationInfo.Political;
                    }

                    if (emptyCounty.Place.County == "")
                        emptyCounty.Place.County = _placeRepository.SearchPlacesDBForCounty(emptyCounty.LocationInfo.Political);
                }
            }

            _placeRepository.SaveChanges();

            var unsetCountyCount = _placeRepository.GetUnsetUkCountiesCount();

            var unsetCountyCountJSONResult = _placeRepository.GetUnsetJsonResultCount();

            _iLog.WriteLine("PlaceCache has ~" + unsetCountyCount + " records with no county or country");

            _iLog.WriteLine("PlaceCache has ~" + unsetCountyCountJSONResult + " records with no JSON result");

            _iLog.WriteLine("PlaceCache finished setting");
        }
    }
}
