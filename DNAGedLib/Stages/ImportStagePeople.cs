using System;
using System.Collections.Generic;
using System.Linq;
using DNAGedLib.Models;

namespace DNAGedLib
{
    public class ImportStagePeople : ImportStage
    {
        public ImportStagePeople(ImportationContext personImporter)
        {
            base.personImporter = personImporter;
        }
        public override Response Import()
        {
            #region adding persons


            HashSet<long> personsAdded = null;

            personsAdded = base.personImporter.DNAGedContext.Persons.Select(p => p.Id).ToHashSet();
            List<Persons> personToAdd = new List<Persons>();



            var filteredlist = base.personImporter.SQLliteTrees.ToList();

            //&& !personsAdded.Contains(l.PersonId)


            Console.WriteLine("adding persons: " + filteredlist.Count);

            double total = filteredlist.Count;
            double counter = 0;
            double percentage = 0.0;

            int addedRecords = 0;

            foreach (var s in filteredlist)
            {
                percentage = ((counter / total) * 100);
                Console.Write("\r" + percentage + " %   ");
                counter++;

                if (personsAdded.Contains(s.PersonId)) continue;

                personToAdd.Add(new Persons
                {
                    Id = s.PersonId,
                    ChristianName = s.GivenName,
                    Surname = s.Surname,
                    DeathPlace = s.DeathPlace,
                    DeathDate = s.DeathString,
                    DeathYear = s.DeathInt,
                    BirthDate = s.BirthString,
                    BirthPlace = s.BirthPlace,
                    BirthYear = s.BirthInt,
                    BirthCountry = "Unknown",
                    DeathCountry = "Unknown",
                    FatherId = s.FatherId,
                    MotherId = s.MotherId,
                    CountyUpdated = false,
                    AmericanParentsChecked = false,
                    EnglishParentsChecked = false,
                    CountryUpdated = false

                });

                addedRecords++;
                personsAdded.Add(s.PersonId);

                if (addedRecords % 50000 == 0)
                    base.personImporter.DNAGedContext.SaveChanges();
            }

            #endregion

            base.personImporter.DNAGedContext.BulkInsert(personToAdd);

            //   dnagedContext.SaveChanges();

            Console.WriteLine("saved " + addedRecords + " new records");

            return new Response()
            {
                State = ReturnState.Success,
                Message = "saved " + addedRecords + " new records"
            };
        }

    }






}
