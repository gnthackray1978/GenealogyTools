using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTMContextNet.Data.Repositories;
using LoggingLib;

namespace FTMContextNet.Application.Services
{
    public class CreateTreeGroupMappings
    {
        private readonly PersistedCacheRepository _persistedCacheRepository;
        private readonly FTMMakerRepository _ftmMakerRepository;
        private readonly Ilog _ilog;

        public CreateTreeGroupMappings(PersistedCacheRepository persistedCacheRepository, FTMMakerRepository ftmMakerRepository, Ilog outputHandler)
        {
            _persistedCacheRepository = persistedCacheRepository;
            _ftmMakerRepository = ftmMakerRepository;
            _ilog = outputHandler;
        }

        public void Execute()
        {

            _ilog.WriteLine("Executing Create Tree Group Mappings");

            _persistedCacheRepository.DeleteRecordMapGroups();

            var tp = _ftmMakerRepository.GetGroups();

            //
            var idx = 0;

            foreach (var grp in tp)
            {
                foreach (var mapping in grp.Value)
                {
                    _persistedCacheRepository.SaveTreeRecordMapGroup(idx, mapping, grp.Key);

                    idx++;
                }
            }

            _ilog.WriteLine("Finished Create Tree Group Mappings");

        }
    }
}
