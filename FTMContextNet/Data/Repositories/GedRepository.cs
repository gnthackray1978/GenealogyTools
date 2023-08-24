using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LoggingLib;
using QuickGed;
using QuickGed.Services;
using QuickGed.Types;


namespace FTMContextNet.Data.Repositories
{
    public class GedRepository : IGedRepository
    {
        private readonly Ilog _logger;
        private readonly IGedParser _gedParser;

        public GedRepository(Ilog logger, IGedParser gedParser)
        {
            this._logger = logger;
            this._gedParser = gedParser;
        }

        #region GetGroups unused for now


        public Dictionary<string, List<string>> GetGroups(GedDb gedDb)
        {
            var results = new Dictionary<string, List<string>>();

            var treeIds = gedDb.Persons.Where(w => w.IsRootPerson).Select(s => s.Id).ToList();
            
            var tp = gedDb.Relationships
                .Select(s => new RelationSubSet() { Person1Id = s.Person1Id, Person2Id = s.Person2Id }).ToList();

            var nameDict = gedDb.Persons.Where(w => w.IsRootPerson).ToDictionary(i => i.Id, i => i.FullName);

            var groupNames = gedDb
                .Persons
                .Where(p => p != null && p.FullName.ToLower().Contains("group"))
                .Cast<IPerson>().ToDictionary(i => i.Id, i => i.FullName); ;

            foreach (var treeId in treeIds)
            {

                var groupMembers = tp.Where(t => t.MatchEither(treeId)).Select(s => s.GetOtherSide(treeId)).Distinct().ToList();

                
                results.Add(nameDict[treeId], (from gm in groupMembers where groupNames.ContainsKey(gm) select groupNames[gm]).ToList());
            }

            return results;
        }

        private static List<string> IdsToNames(List<int> groupMembers, Dictionary<int, string> groupNames)
        {
            return (from gm in groupMembers where groupNames.ContainsKey(gm) select groupNames[gm]).ToList();
        }


        #endregion

        public GedDb ParseLabelledTree(string path)
        {
            var gedDb = _gedParser.Parse(path);
            
            var rootPersons = gedDb.Persons.Where(w => w.IsRootPerson).ToList();
            // if there are no root persons then we need to 
            // give all people in the ged the same label
            // i.e. assume they are the same tree.

            Console.WriteLine(rootPersons.Count);

            var timer = new Stopwatch();
            timer.Start();

            var idx = 1;
            var total = rootPersons.Count;
            foreach (var rp in rootPersons)
            {
                TreeLabeller.LabelTree(gedDb.ParentDictionary, rp, rp.FullName);

                _logger.ProgressUpdate(idx,total,"labelled trees");
             

                idx++;
            }

            if (rootPersons.Count == 0)
            {
                foreach (var p in gedDb.Persons)
                {
                    p.Origin = gedDb.FileName;
                }
            }


            timer.Stop();

            TimeSpan timeTaken = timer.Elapsed;
            string foo = "Time taken: " + timeTaken.ToString(@"m\:ss\.fff");

            Console.WriteLine(foo);

            Console.WriteLine("finished");

            return gedDb;

        }

    }
}
