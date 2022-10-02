using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTMContextNet.Data.Repositories;
using LoggingLib;

namespace FTMContextNet.Application.Services
{
    public class CreateTreeGroups
    {
        private readonly PersistedCacheRepository _persistedCacheRepository;
        private readonly FTMMakerRepository _ftmMakerRepository;
        private readonly Ilog _ilog;

        public CreateTreeGroups(PersistedCacheRepository persistedCacheRepository, FTMMakerRepository ftmMakerRepository, Ilog outputHandler)
        {
            _persistedCacheRepository = persistedCacheRepository;
            _ftmMakerRepository = ftmMakerRepository;
            _ilog = outputHandler;
        }

        public void Execute()
        {

            _ilog.WriteLine("Executing Creating Tree Groups");

            _persistedCacheRepository.DeleteTreeGroups();

            foreach (var group in _ftmMakerRepository.GetGroupPerson())
            {
                _persistedCacheRepository.SaveTreeGroups(group.Id, group.FullName);
            }

            _ilog.WriteLine("Finished Creating Tree Groups");

        }
    }
}
