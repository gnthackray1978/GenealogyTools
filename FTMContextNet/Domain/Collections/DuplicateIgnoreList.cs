using System.Collections.Generic;
using FTMContextNet.Domain.Entities.Persistent.Cache;

namespace FTMContextNet.Domain.Collections
{
    public class DuplicateIgnoreList
    {
        private readonly List<IgnoreList> _ignoreList;

        public DuplicateIgnoreList(List<IgnoreList> ignoreList)
        {
            _ignoreList = ignoreList;
        }


        public bool ContainsPair(string nameA, string nameB)
        {
            var matchPair = new List<string>
            {
                nameA,
                nameB
            };

            foreach (var ignoreItem in _ignoreList)
            {
                if (ignoreItem.Person1.ToLower().Trim() == matchPair[0].ToLower().Trim() && ignoreItem.Person2.ToLower().Trim() == matchPair[1].ToLower().Trim())
                {
                    return true;
                }
            }

            matchPair.Reverse();

            foreach (var ignoreItem in _ignoreList)
            {
                if (ignoreItem.Person1.ToLower().Trim() == matchPair[0].ToLower().Trim() && ignoreItem.Person2.ToLower().Trim() == matchPair[1].ToLower().Trim())
                {
                    return true;
                }
            }

            return false;
        }

    }
}
