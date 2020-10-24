using System;
using System.Collections.Generic;
using System.Linq;
using DNAGedLib.Models;

namespace DNAGedLib
{
    public class ImportStageMatchDetailsWithGroups : ImportStage
    {
      

        public ImportStageMatchDetailsWithGroups(ImportationContext personImporter)
        {
           
            base.personImporter = personImporter;
        }

        public override Response Import()
        {

            // find all the matches in the existing db for this testid
            var filterGroupList = base.personImporter.DNAGedContext.MatchGroups.Where(w => w.TestGuid == personImporter.ImportTestId).Select(m => m.MatchGuid).ToList();

            //find all the matches in the sqlite db for this testid
            var missingGroupRecordCount = base.personImporter.MatchGroups.Where(w => w.TestGuid == personImporter.ImportTestId).Count(w => !filterGroupList.Contains(w.MatchGuid));

            var newRecords = base.personImporter.MatchGroups.Where(w => !filterGroupList.Contains(w.MatchGuid) && w.TestGuid == personImporter.ImportTestId).ToList();

            Console.WriteLine("missing match groups: " + missingGroupRecordCount);

            List<Guid> insertedRecords = new List<Guid>();

            int newMatchGroupCounter = 0;
            foreach (var s in newRecords)
            {
                if (insertedRecords.Contains(s.MatchGuid)) continue;

                insertedRecords.Add(s.MatchGuid);

                base.personImporter.DNAGedContext.MatchGroups.Add(new Models.MatchGroups()
                {
                    Id = s.Id,
                    SharedSegment = s.SharedSegment,
                    TestGuid = s.TestGuid,
                    MatchGuid = s.MatchGuid,
                    Note = s.Note,
                    GroupName = s.GroupName,
                    TreesPrivate = s.TreesPrivate,
                    UserPhoto = s.UserPhoto,
                    TestAdminDisplayName = s.TestAdminDisplayName,
                    TestDisplayName = s.TestDisplayName,
                    TreeId = s.TreeId,
                    TreeNodeCount = s.TreeNodeCount,
                    Confidence = s.Confidence,
                    Starred = s.Starred,
                    SharedCentimorgans = s.SharedCentimorgans,
                    Viewed = s.Viewed,
                    HasHint = s.HasHint
                });

                newMatchGroupCounter++;
            }

            Console.WriteLine("added: " + newMatchGroupCounter + " to the context");


            base.personImporter.DNAGedContext.SaveChanges();

            using (var context = new DNAGEDContext())
            {

                var existingMatchRecords = context.MatchDetail.Where(w => w.TestGuid == personImporter.ImportTestId)
                    .Select(m => m.MatchGuid).ToList();

                Console.WriteLine("number of match details in the system: " + existingMatchRecords.Count);
                var sqlLiteMatchDetailsToImport = base.personImporter.MatchDetails.Where(w => w.TestGuid == base.personImporter.ImportTestId)
                    .Select(s => s.MatchGuid).ToList();

                var missingRecordCount = sqlLiteMatchDetailsToImport.Count(w => !existingMatchRecords.Contains(w));

                Console.WriteLine("missing match details: " + missingRecordCount);

                Console.WriteLine("adding match details: ");

                var matchDetails = base.personImporter.MatchDetails
                    .Where(w => !existingMatchRecords.Contains(w.MatchGuid)
                    && w.TestGuid == base.personImporter.ImportTestId).Select(s =>
                        new Models.MatchDetail()
                        {
                            Id = s.Id,
                            SharedSegment = s.SharedSegment,
                            TestGuid = s.TestGuid,
                            MatchGuid = s.MatchGuid,
                        }).ToList();

                context.BulkInsert(matchDetails);

                Console.WriteLine("added: " + missingRecordCount + " MatchDetails to the context");

                //  context.SaveChanges();

            }

            return new Response()
            {
                State = ReturnState.Success,
                Message = "saving context"
            };
             
        }

    }






}
