using System.Text.RegularExpressions;

namespace FTMContextNet.Domain.Entities.Persistent.Cache
{
    public partial class TreeRecord
    {
        public static TreeRecord CreateFromOrigin(string origin, string locationCsv, int personCount, int importId)
        {
            Regex re = new Regex(@"\d+");

            Match m = re.Match(origin);

            int cmVal = 0;

            if (m.Success)
            {
                int.TryParse(m.Value, out cmVal);
            }

            re = new Regex(@"[fF]\d+");

            m = re.Match(origin);

            var tr =new TreeRecord(){
                PersonCount = personCount,
                Name = origin,
                Origin = locationCsv,
                CM = cmVal,
                Located = m.Success,
                ImportId = importId
            };

            return tr;
        }

        public int Id { get; set; }
        public int PersonCount { get; set; }
        public string Origin { get; set; }
        public string Name { get; set; }
        public int CM { get; set; }

        public bool Located { get; set; }

        public int ImportId { get; set; }

        public int UserId { get; set; }
    }
}