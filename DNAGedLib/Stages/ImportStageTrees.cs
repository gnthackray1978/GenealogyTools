using System;
using System.Collections.Generic;
using System.Linq;
using DNAGedLib.Models;
using GenDBContext.Models;

namespace DNAGedLib
{
    public class ImportStageTrees : ImportStage
    {
        public ImportStageTrees(ImportationContext personImporter)
        {
            base.personImporter = personImporter;
        }
        public override Response Import()
        {
            using (var dnagedContext = new DNAGEDContext())
            {
                long counter = 0;
                long percentage = 0;


                var treesContextHash = dnagedContext.
                    MatchTrees.Select(s => s.PersonId).Distinct().ToHashSet();

                var treesSQLLiteHash = base.personImporter.SQLliteTrees.Select(t => t.PersonId).Distinct().ToHashSet();

                // all ids not in treesContextHash that are in treesSQLLiteHash
                Console.WriteLine(treesSQLLiteHash.Count + " MatchTrees in SQL Lite DB");
                Console.WriteLine(treesContextHash.Count + " MatchTrees in SQL SERVER");
                Console.WriteLine(treesContextHash.Count - treesSQLLiteHash.Count + " Correct Number");

                //remove the persons that are already in the sql server instance
                treesSQLLiteHash.ExceptWith(treesContextHash);

                Console.WriteLine(treesSQLLiteHash.Count + " Persons that are in MatchTrees that need adding");


                decimal percentagecompleted = 0;



                var guidList = dnagedContext.MatchTrees.Select(s => s.MatchId).ToHashSet();

                long addedRecords = 0;
                //total of people we need to add


                //use the list of people we need to add
                //to filter out a list of records from the sqllite db
                //they might not be the same length
                //because the same person can be in multiple trees
                //so in the table twice
                var listOfTreesToAdd = base.personImporter.SQLliteTrees.Where(w => treesSQLLiteHash.Contains(w.PersonId)).ToList();

                Console.WriteLine(listOfTreesToAdd.Count + " Trees in MatchTrees that need adding");

                var total = listOfTreesToAdd.Count;

                List<MatchTrees> matchTrees = new List<MatchTrees>();

                Console.WriteLine("importing trees now..");
                foreach (var s in listOfTreesToAdd)
                {
                    percentagecompleted = Decimal.Divide(counter, total) * 100;


                    if (addedRecords % 1000 == 0)
                        Console.Write("\r" + percentagecompleted + " %   ");

                    counter++;

                    //dnagedContext.MatchTrees.Add(new MatchTrees
                    //{
                    //    Id = s.Id,
                    //    PersonId = s.PersonId,
                    //    RelId = s.RelId,
                    //    MatchId = s.MatchId,
                    //    CreatedDate = DateTime.Now
                    //});

                    matchTrees.Add(new MatchTrees
                    {
                        Id = s.Id,
                        PersonId = s.PersonId,
                        RelId = s.RelId,
                        MatchId = s.MatchId,
                        CreatedDate = DateTime.Now
                    });


                    addedRecords++;

                    //  if (addedRecords % 50000 == 0)
                    //      dnagedContext.SaveChanges();

                    guidList.Add(s.MatchId);
                }

                //  dnagedContext.SaveChanges();

               // dnagedContext.BulkInsert(matchTrees);

                Console.WriteLine("saved " + addedRecords + " new records");
            };

            return new Response()
            {
                State = ReturnState.Success,
                Message = "Updated"
            };
        }

    }






}
