
//using System;
//using System.Text.RegularExpressions;
//using FTM.Dates;
//using FTMContextNet.Domain.Entities.Source;

//namespace FTMContext
//{
//    public class RelationSubSet : IRelationship
//    {
//        public int Id { get; set; }
//        public int? Person1Id { get; set; }
//        public int? Person2Id { get; set; }

//        public int LinkId { get; set; }
        
//        public string DateStr { get; set; }
//        public Date Date { get; set; }

//        public int DateYear { get; set; }

//        public int? PlaceId { get; set; }
//        public string Text { get; set; }

//        public string Origin { get; set; }

//        public string PlaceName { get; set; }

       

//        public static RelationSubSet Create(int id, string date, string location, int groom, int bride, int marriageYear)
//        {

//            return new RelationSubSet()
//            {
//                Id = id,
//                Person1Id = groom,
//                Person2Id = bride,
//                PlaceName = location,
//                DateStr = date,
//                DateYear = marriageYear
//            };
//        }

//        public bool MatchEither(int groupId)
//        {
//            if (Person1Id.GetValueOrDefault() == groupId || Person2Id.GetValueOrDefault() == groupId)
//            {
//                return true;
//            }

//            return false;
//        }

//        public int GetOtherSide(int groupId)
//        {
//            var potentialId = Person2Id.GetValueOrDefault();

//            if (Person2Id == groupId)
//            {
//                potentialId = Person1Id.GetValueOrDefault();
//            }

//            return potentialId;
//        }

//        public static bool ValidYear(Date date)
//        {
//            if (date == null) return false;

//            if (date.Year != null && date.HasYear())
//            {
//                return true;
//            }

//            return false;
//        }
//    }
//}
