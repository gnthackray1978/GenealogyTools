using System.Data;
using Microsoft.Data.SqlClient;

namespace DNAGedLib
{
    public class ImportStageUpdateStats : ImportStage
    {
        public ImportStageUpdateStats(ImportationContext personImporter)
        {
            base.personImporter = personImporter;
        }
        public override Response Import()
        {
            using (var conn =
                new SqlConnection(base.personImporter.Destination))
            using (var command = new SqlCommand("UpdateKits", conn)
            {
                CommandType = CommandType.StoredProcedure
            })
            {
                conn.Open();
                command.ExecuteNonQuery();
            }

            return new Response()
            {
                State = ReturnState.Success,
                Message = "Updated"
            };
        }

    }






}
