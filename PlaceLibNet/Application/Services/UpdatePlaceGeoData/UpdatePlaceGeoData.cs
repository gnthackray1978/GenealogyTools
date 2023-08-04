using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using LoggingLib;
using MediatR;
using MSGIdent;
using PlaceLibNet.Data.Repositories;
using PlaceLibNet.Domain.Caching;
using PlaceLibNet.Domain.Commands;

namespace PlaceLibNet.Application.Services.UpdatePlaceGeoData
{
    public class UpdatePlaceGeoData : IRequestHandler<UpdatePlaceGeoDataCommand, CommandResult>
    {
        private readonly Ilog _iLog;
        private readonly PlaceRepository _placeRepository;
        private readonly IMapper _iMapper;
        private readonly IAuth _auth;

        public UpdatePlaceGeoData(PlaceRepository placeRepository, Ilog iLog, IMapper iMapper, IAuth auth)
        {
            _iLog = iLog;
            _placeRepository = placeRepository;
            _iMapper = iMapper;
            _auth = auth;
        }
         

        /// <summary>
        /// Updates place entry in cacheData.PlaceCache with result we got back from google geocode.
        /// </summary>
        /// <returns></returns>
        public async Task<CommandResult> Handle(UpdatePlaceGeoDataCommand request, CancellationToken cancellationToken)
        {
            if (_auth.GetUser() == -1)
            {
                return CommandResult.Fail(CommandResultType.Unauthorized);
            }

            _iLog.WriteLine("Updating cacheData.FTMPlaceCache with geocode result");

            await Task.Run(() => { _placeRepository.SetPlaceGeoData(request.PlaceId, request.Results); }, cancellationToken);

            return CommandResult.Success();
        }
    }
}
