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
            _iLog.WriteLine("Finished - SetGeolocatedResult");
            _placeRepository.SetCounties();
            _iLog.WriteLine("Finished - SetCounties");

            _iLog.WriteLine("Finished UpdatePlaceMetaData");
        }
    }
}
