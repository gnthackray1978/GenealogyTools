using System;
using System.Linq;
using GenDBContextNET.Contexts;

namespace DNAGedLib
{
    public class SelectProfile : ImportStage
    {
    
        public SelectProfile(ImportDataStore personImporter)
        {
            base.personImporter = personImporter;
            
        }
        public override Response Import()
        {
            DNAGEDContext dnagedContext = new DNAGEDContext();

            base.personImporter.Profiles = dnagedContext.MatchKitName.ToList();

            if (base.personImporter.Profiles.Count == 0)
            {                
                return new Response()
                {
                    State = ReturnState.Failure,
                    Message = "No profiles to select from"
                };
            }

            int idx = 0;
            foreach (var p in base.personImporter.Profiles)
            {
                Console.WriteLine(idx + ". " + p.Name + " " +
                    p.LastUpdated + " " + p.PersonCount);
                idx++;
            }

            int sin = 0;
            int countofprofiles = base.personImporter.Profiles.Count;

            var input = Console.ReadKey();

            while (!int.TryParse(input.KeyChar.ToString(), out sin) || sin >= countofprofiles)
            {
                Console.WriteLine("Not a valid user: ");
                input = Console.ReadKey();
            }


            base.personImporter.ImportTestId = base.personImporter.Profiles[sin].Id;

            
            if (base.personImporter.Profiles[sin].LastUpdated != null)
                base.personImporter.CutOff = base.personImporter.Profiles[sin].LastUpdated.Value;
             
            return new Response()
            {
                State = ReturnState.Pause,
                Message = "User: " + base.personImporter.Profiles[sin].Name
            };
        }

         

    }






}
