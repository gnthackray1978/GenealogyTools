using LoggingLib;
using PlaceLibNet.Data.Repositories;

namespace PlaceLibNet.Application.Services
{
    public class UpdatePlaceMetaData
    {
        private readonly Ilog _iLog;
        private readonly PlaceRepository _placeRepository;

        public UpdatePlaceMetaData(PlaceRepository placeRepository,Ilog iLog)
        {
            _iLog = iLog;
            _placeRepository = placeRepository;
        }

        public void Execute()
        {
            _iLog.WriteLine("Executing UpdatePlaceMetaData");

            _placeRepository.SetGeolocatedResult();

            _placeRepository.SetCounties();
        }
    }
}
