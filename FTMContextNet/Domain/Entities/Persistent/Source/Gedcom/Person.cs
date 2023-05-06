using FTMContext;
using FTMContextNet.Domain.Entities.Source;
using QuickGed;

namespace FTMContextNet.Domain.Entities.Persistent.Source.Gedcom
{
    public class Person : IPerson
    {
     
        public int Id { get; set; }

        public string StrId { get; set; }
        public string Uid { get; set; } 
        public string FirstName { get; set; }
        public string FamilyName { get; set; }

        public string FullName => FirstName + " " + FamilyName;

        public string Name { get; set; }

        public string Gender { get; set; }
        public DatePlace Birth { get; set; }  
        public DatePlace Death { get; set; }
        public DatePlace Buried { get; set; }
        public DatePlace Baptized { get; set; } 
        public string Note { get; set; } 
        public string Occupation { get; set; } 
        public string Title { get; set; } 
        public DatePlace Residence { get; set; }

        public ProcessLocationReturnType GetLocationReturnType()
        {

            return new ProcessLocationReturnType()
            {
            };
        }
    }
}