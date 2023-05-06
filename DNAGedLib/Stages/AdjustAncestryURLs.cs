using System;
using System.Linq;
using GenDBContextNET.Contexts;

namespace DNAGedLib
{
    public class AdjustAncestryURLs : ImportStage
    {
        public AdjustAncestryURLs(ImportDataStore personImporter)
        {
            base.personImporter = personImporter;
        }
        public override Response Import()
        {
            DNAGEDContext dnagedContext = new DNAGEDContext();

            Console.WriteLine("Updatinging URLS");

            // double total = dnagedContext.MatchGroups.Count(mg => mg.TreeId.Contains("ancestry.com"));
            double counter = 0;
            double percentage = 0.0;

            var data = dnagedContext.MatchGroups.Where(mg => mg.TreeId.Contains("ancestry.com"));
            //dnagedContext.UpdateRange();
            double total = data.Count();

            int saveCounter = 0;

            foreach (var p in data)
            {
                p.TreeId = p.TreeId.Replace(@"ancestry.com", "ancestry.co.uk");

                percentage = ((counter / total) * 100);

                Console.Write("\r" + percentage + " %   ");

                counter++;

                saveCounter++;

                dnagedContext.MatchGroups.Update(p);
            }

            dnagedContext.SaveChanges();

           
            return new Response()
            {
                State = ReturnState.Success,
                Message = "Saved URL changes"
            };
        }

    }






}
