using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ConsoleTools;
using FTMContext.Models;

namespace FTMContext
{
    public class FTMTreeRecordCreator
    {
        private FTMakerContext _context;
        private FTMakerCacheContext _cacheContext;
        private IConsoleWrapper _consoleWrapper;

        public List<int> AddedPersons = new List<int>();

        public FTMTreeRecordCreator(FTMakerContext context,
            FTMakerCacheContext cacheContext,
            IConsoleWrapper consoleWrapper)
        {
            _consoleWrapper = consoleWrapper;
            _context = context;
            _cacheContext = cacheContext;
        }



        public void Create()
        {
 
            _consoleWrapper.WriteLine("Creating Tree Records");

            int idx = _cacheContext.TreeRecords.Count() + 1;

            var rootPeople = _context.Person.Where(w => w.FamilyName.StartsWith("_"));

            foreach (var family in _cacheContext.FTMPersonView.ToList().GroupBy(g => g.Origin))
            {
             
                string familyName = family.First().Origin ?? "Unknown";
                  
                _consoleWrapper.WriteCounter("Adding Tree " + familyName + " " + family.Count() + " ancestors");

                List<string> locationList = new List<string>();

                foreach (var child in family)
                {
                    var parts = child.LinkedLocations.Split(',');

                    foreach (var part in parts)
                    {
                        if (!locationList.Contains(part))
                        {
                            if(HistoricCounties.Get.Contains(part))
                                locationList.Add(part);
                        }
                    }

                }



                string originString = string.Join(",", locationList);



                Regex re = new Regex(@"\d+");

                Match m = re.Match(familyName);

                int cmVal = 0;

                if (m.Success)
                {
                    Int32.TryParse(m.Value, out cmVal);
                }

                re = new Regex(@"[fF]\d+");

                m = re.Match(familyName);
                 
                _cacheContext.TreeRecords.Add(new TreeRecord()
                {
                    Id = idx,
                    PersonCount = family.Count(),
                    Name = familyName,
                    Origin = originString,
                    CM = cmVal,
                    Located = m.Success
                });

                idx++;
            }

          
             
            _cacheContext.SaveChanges();
        }
 

    }
}