using System.Threading;
using System.Threading.Tasks;
using LoggingLib;
using MediatR;
using MSGIdent;
using PlaceLibNet.Data.Repositories;
using PlaceLibNet.Domain.Caching;
using PlaceLibNet.Domain.Commands;

namespace PlaceLibNet.Application.Services.UpdatePlaceMetaData
{
    public class UpdatePlaceMetaData : IRequestHandler<UpdatePlaceMetaDataCommand, CommandResult>
    {
        private readonly Ilog _iLog;
        private readonly PlaceRepository _placeRepository;
        private readonly IAuth _auth;

        public UpdatePlaceMetaData(PlaceRepository placeRepository, Ilog iLog, IAuth auth)
        {
            _iLog = iLog;
            _auth = auth;
            _placeRepository = placeRepository;
        }
         
        public async Task<CommandResult> Handle(UpdatePlaceMetaDataCommand request, CancellationToken cancellationToken)
        {
            if (_auth.GetUser() == -1)
            {
                return CommandResult.Fail(CommandResultType.Unauthorized);
            }

            _iLog.WriteLine("Executing UpdatePlaceMetaData");

            await Task.Run(()=>{

                _placeRepository.SetGeolocatedResult();
                _iLog.WriteLine("Finished - SetGeolocatedResult");
                _placeRepository.SetCounties();
                _iLog.WriteLine("Finished - SetCounties");
            }, cancellationToken);
            _iLog.WriteLine("Finished UpdatePlaceMetaData");

            return CommandResult.Success();
        }
    }
}
