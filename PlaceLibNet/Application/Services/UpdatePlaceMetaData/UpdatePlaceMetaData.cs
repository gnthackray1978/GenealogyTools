using System.Threading;
using System.Threading.Tasks;
using LoggingLib;
using MediatR;
using MSG.CommonTypes;
using MSGIdent;
using PlaceLibNet.Data.Repositories;
using PlaceLibNet.Domain.Caching;
using PlaceLibNet.Domain.Commands;

namespace PlaceLibNet.Application.Services.UpdatePlaceMetaData
{
    /// <summary>
    /// Update place cache lat long
    /// Update place cache county
    /// Update place cache bad data where appropriate
    /// </summary>
    public class UpdatePlaceMetaData : IRequestHandler<UpdatePlaceMetaDataCommand, CommandResult>
    {
        private readonly Ilog _iLog;
        private readonly IPlaceRepository _placeRepository;
        private readonly IAuth _auth;

        public UpdatePlaceMetaData(IPlaceRepository placeRepository, Ilog iLog, IAuth auth)
        {
            _iLog = iLog;
            _auth = auth;
            _placeRepository = placeRepository;
        }
         
        /// <summary>
        /// Update place cache lat long
        /// Update place cache county
        /// Update place cache bad data where appropriate
        /// </summary>
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
