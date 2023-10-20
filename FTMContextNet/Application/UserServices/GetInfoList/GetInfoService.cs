using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LoggingLib;
using FTMContextNet.Application.Models.Read;
using AutoMapper;
using MSGIdent;
using MediatR;
using FTMContextNet.Data.Repositories.TreeAnalysis;

namespace FTMContextNet.Application.UserServices.GetInfoList
{
    public class GetInfoService : IRequestHandler<GetInfoServiceQuery, InfoModel>
    {
        private readonly Ilog _iLog;
        private readonly IPersistedCacheRepository _persistedCacheRepository;
        private readonly IMapper _iMapper;
        private readonly IAuth _auth;

        public GetInfoService(IPersistedCacheRepository persistedCacheRepository,
            Ilog outputHandlerp, IMapper iMapper, IAuth auth)
        {
            _iLog = outputHandlerp;
            _persistedCacheRepository = persistedCacheRepository;
            _iMapper = iMapper;
            _auth = auth;
        }
         

        public async Task<InfoModel> Handle(GetInfoServiceQuery request, CancellationToken cancellationToken)
        {
            _iLog.WriteLine("Executing GetInfoService");

            var infoModal = new InfoModel();

             await Task.Run(()=> infoModal = _iMapper.Map<InfoModel>(_persistedCacheRepository.GetInfo(_auth.GetUser())), cancellationToken);
           
            return infoModal;
        }


    }
}
