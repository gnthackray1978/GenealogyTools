//using System.Diagnostics;
//using System.Text.RegularExpressions;
//using LoggingLib;
//using QuickGed.Services;
//using QuickGed.Types;

//namespace QuickGed
//{
//    //reading the gedfile into a list of relationships, persons, and childrelationships
//    //labelling file
//    //get lists of tree by group name
//    public class QuickGed
//    {
//        private Ilog _logger;
//        private string _gedPath;
//        public GedDb _GedDb { get; set; }

//        public QuickGed(string filePath, Ilog logger)
//        {
//            this._logger = logger;
//            this._gedPath = filePath;
//        }

//        #region uninterested right now

//        public int GetMyId()
//        {
//            return -1;
//        }

//        public List<Node> GetTreeRootPeople()
//        {
//            int personId = GetMyId();

//            Regex r = new Regex(@"_[1-9]\d*(\.\d+)?_");


//            var result = _GedDb.Persons.Where(p =>
//                p != null && (!p.FullName.ToLower().Contains("group")
//                    && p.FullName.ToLower().Contains("_") || p.Id == personId));

//            var persons = new List<Node>();

//            //this should only be a small number of records so the performance hit
//            //ought to not be noticeable
//            foreach (var person in result)
//            {
//                if (r.IsMatch(person.FullName))
//                {
//                    persons.Add(person);
//                }
//            }

//            return persons;
//        }

//        public HashSet<int> GetListOfTreeIds()
//        {
//            var lst = GetTreeRootPersons().Select(s => s.Id);

//            var set = new HashSet<int>();

//            foreach (var i in lst)
//            {
//                set.Add(i);
//            }

//            return set;
//        }

//        public Dictionary<int, string> GetTreeRootNameDictionary()
//        {
//            var nameDictionary = new Dictionary<int, string>();

//            var lst = GetTreeRootPersons();

//            foreach (var i in lst)
//            {
//                nameDictionary.Add(i.Id, i.FullName);
//            }

//            return nameDictionary;
//        }

//        public Dictionary<int, string> GetTreeGroupNameDictionary()
//        {
//            var nameDictionary = new Dictionary<int, string>();


//            var gps = GetGroupPerson();

//            foreach (var i in gps)
//            {
//                nameDictionary.Add(i.Id, i.FullName);
//            }

//            return nameDictionary;
//        }

//        private IReadOnlyList<Node> GetTreeRootPersons()
//        {
//            int personId = GetMyId();

//            Regex r = new Regex(@"_[1-9]\d*(\.\d+)?_");


//            var result = _GedDb.Persons.Where(p =>
//                p != null && (!p.FullName.ToLower().Contains("group")
//                    && p.FullName.ToLower().Contains("_") || p.Id == personId));

//            //this should only be a small number of records so the performance hit
//            //ought to not be noticeable

//            return result.Where(person => r.IsMatch(person.FullName)).ToList();
//        }

//        public List<IPerson> GetGroupPerson()
//        {
//            var groups = this._GedDb.Persons.Where(p => p != null && p.FullName.ToLower().Contains("group"));

//            return groups.Cast<IPerson>().ToList();
//        }

//        public Dictionary<string, List<string>> GetGroups()
//        {
//            var results = new Dictionary<string, List<string>>();

//            var treeIds = GetListOfTreeIds();


//            var tp = this._GedDb.Relationships
//                .Select(s => new RelationSubSet() { Person1Id = s.Person1Id, Person2Id = s.Person2Id }).ToList();

//            var nameDict = GetTreeRootNameDictionary();

//            var groupNames = GetTreeGroupNameDictionary();

//            foreach (var treeId in treeIds)
//            {

//                var groupMembers = tp.Where(t => t.MatchEither(treeId)).Select(s => s.GetOtherSide(treeId)).Distinct().ToList();

//                var names = IdsToNames(groupMembers, groupNames);

//                results.Add(nameDict[treeId], names);
//            }

//            return results;
//        }

//        private static List<string> IdsToNames(List<int> groupMembers, Dictionary<int, string> nameDict)
//        {
//            return (from gm in groupMembers where nameDict.ContainsKey(gm) select nameDict[gm]).ToList();
//        }

    
//        #endregion

//        public void ParseLabelledTree()
//        {
//            _GedDb = GedParser.Parse(this._gedPath);



//            var rootPersons = this.GetTreeRootPersons();

//            Console.WriteLine(rootPersons.Count);

//            var timer = new Stopwatch();
//            timer.Start();

//            var idx = 0;

//            foreach (var rp in rootPersons)
//            {
//                TreeLabeller.LabelTree(this._GedDb.ParentDictionary, rp, rp.FullName);

//                Console.Write("\r{0}%   ", idx);

//                idx++;

//            }

//            timer.Stop();

//            TimeSpan timeTaken = timer.Elapsed;
//            string foo = "Time taken: " + timeTaken.ToString(@"m\:ss\.fff");

//            Console.WriteLine(foo);

//            Console.WriteLine("finished");
//        }

//    }
//}
