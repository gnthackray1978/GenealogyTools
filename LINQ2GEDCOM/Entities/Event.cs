using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LINQ2GEDCOM.Entities
{
    public class Event : BaseEntity
    {
        private EventType _eventType;

        public string EventText { get; set; }
        public string Date { get; set; }
        public string Agency { get; set; }
        public string Address { get; set; }
        public string Place { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Cause { get; set; }
        public IList<EntitySource> Sources { get; set; }
        public DateTime Change { get; set; }
        public DateTime Create { get; set; }
        public IList<int> NoteIDs { get; set; }

        public System.DateTime? DateTimeValue
        {
            get
            {
                var result = System.DateTime.MinValue;
                if (System.DateTime.TryParse(Date, out result))
                    return result;
                return null;
            }
        }

        public IEnumerable<Note> Notes
        {
            get
            {
                return Context.Notes.Where(n => NoteIDs.Contains(n.ID));
            }
        }

        private string EventString
        {
            get
            {
                switch (_eventType)
                {
                    case EventType.Burial:
                        return "BURI";
                    case EventType.Death:
                        return "DEAT";
                    case EventType.Birth:
                        return "BIRT";
                    case EventType.Adoption:
                        return "ADOP";
                    case EventType.Immigration:
                        return "IMMI";
                    case EventType.MilitaryService:
                        return "MISE";
                    case EventType.Ordinance:
                        return "ORDI";
                    case EventType.Ordination:
                        return "ORDN";
                    case EventType.Emigration:
                        return "EMIG";
                    case EventType.Marriage:
                        return "MARR";
                    case EventType.Divorce:
                        return "DIV";
                    case EventType.Naturalization:
                        return "NATU";
                    default:
                        throw new Exception("Invalid Event Type");
                }
            }
        }

        public enum EventType
        {
            Burial,
            Death,
            Birth,
            Adoption,
            Immigration,
            MilitaryService,
            Ordinance,
            Ordination,
            Emigration,
            Marriage,
            Divorce,
            Naturalization
        }

        private Event() { }

        public Event(EventType eventType) : this()
        {
            _eventType = eventType;
        }

        #region Output to GEDCOM
        internal override string ToGEDCOMString(int hierarchyRoot)
        {
            var output = new StringBuilder();

            output.AppendLine(FormatEventString(hierarchyRoot));

            if (!string.IsNullOrWhiteSpace(Date))
                output.AppendLine(FormatDateString(hierarchyRoot + 1));

            if (!string.IsNullOrWhiteSpace(Agency))
                output.AppendLine(FormatAgencyString(hierarchyRoot + 1));

            if (!string.IsNullOrWhiteSpace(Cause))
                output.AppendLine(FormatCauseString(hierarchyRoot + 1));

            if (!string.IsNullOrWhiteSpace(Address))
                output.AppendLine(FormatAddressString(hierarchyRoot + 1));

            if (!string.IsNullOrWhiteSpace(Place))
                output.AppendLine(FormatPlaceString(hierarchyRoot + 1));

            if (!string.IsNullOrWhiteSpace(Longitude))
                output.AppendLine(FormatLongitudeString(hierarchyRoot + 1));

            if (!string.IsNullOrWhiteSpace(Latitude))
                output.AppendLine(FormatLatitudeString(hierarchyRoot + 1));

            foreach (var tag in UserDefinedTags)
                output.Append(tag.ToGEDCOMString(hierarchyRoot + 1));

            if (Sources != null)
                output.Append(FormatSourceString(hierarchyRoot + 1));

            if (NoteIDs != null)
                output.Append(FormatNoteString(hierarchyRoot + 1));

            if (Change != null)
                output.Append(FormatChangeString(hierarchyRoot + 1));

            if (Create != null)
                output.Append(FormatCreateString(hierarchyRoot + 1));

            return output.ToString();
        }

        private string FormatEventString(int hierarchyRoot)
        {
            if ((!string.IsNullOrWhiteSpace(EventText)) && (EventString != EventText))
                return string.Format("{0} {1} {2}", hierarchyRoot, EventString, EventText);
            else
                return string.Format("{0} {1}", hierarchyRoot, EventString);
        }

        private string FormatDateString(int hierarchyRoot)
        {
            return string.Format("{0} DATE {1}", hierarchyRoot, Date);
        }

        private string FormatAgencyString(int hierarchyRoot)
        {
            return string.Format("{0} AGNC {1}", hierarchyRoot, Agency);
        }

        private string FormatAddressString(int hierarchyRoot)
        {
            return string.Format("{0} ADDR {1}", hierarchyRoot, Address);
        }

        private string FormatPlaceString(int hierarchyRoot)
        {
            return string.Format("{0} PLAC {1}", hierarchyRoot, Place);
        }

        private string FormatLongitudeString(int hierarchyRoot)
        {
            return string.Format("{0} LONG {1}", hierarchyRoot, Longitude);
        }

        private string FormatLatitudeString(int hierarchyRoot)
        {
            return string.Format("{0} LATI {1}", hierarchyRoot, Latitude);
        }

        private string FormatCauseString(int hierarchyRoot)
        {
            return string.Format("{0} CAUS {1}", hierarchyRoot, Cause);
        }

        private string FormatSourceString(int hierarchyRoot)
        {
            var output = new StringBuilder();
            foreach (var source in Sources)
                output.Append(source.ToGEDCOMString(hierarchyRoot));
            return output.ToString();
        }

        private string FormatChangeString(int hierarchyRoot)
        {
            return Change.ToGEDCOMString(hierarchyRoot);
        }

        private string FormatCreateString(int hierarchyRoot)
        {
            return Create.ToGEDCOMString(hierarchyRoot);
        }

        private string FormatNoteString(int hierarchyRoot)
        {
            var output = new StringBuilder();
            foreach (var noteID in NoteIDs)
                output.AppendLine(string.Format("{0} NOTE @N{1}@", hierarchyRoot, noteID));
            return output.ToString();
        }
        #endregion

        #region Input from GEDCOM
        internal static IList<Event> FromDataHierarchy(IEnumerable<DataHierarchyItem> items, GEDCOMContext context, EventType eventType)
        {
            return items.Select(i => BuildEvent(i, context, eventType)).ToList();
        }

        private static Event BuildEvent(DataHierarchyItem _event, GEDCOMContext context, EventType eventType)
        {
            var result = new Event(eventType);
            result.Context = context;

            var dateItems = _event.Items.Where(i => i.Value.StartsWith("DATE"));
            var agencyItems = _event.Items.Where(i => i.Value.StartsWith("AGNC"));
            var addressITems = _event.Items.Where(i => i.Value.StartsWith("ADDR"));
            var placeItems = _event.Items.Where(i => i.Value.StartsWith("PLAC"));
            var longitudeItems = _event.Items.Where(i => i.Value.StartsWith("LONG"));
            var latitudeItems = _event.Items.Where(i => i.Value.StartsWith("LATI"));
            var causeItems = _event.Items.Where(i => i.Value.StartsWith("CAUS"));
            var sourceItems = _event.Items.Where(i => i.Value.StartsWith("SOUR"));
            var noteItems = _event.Items.Where(i => i.Value.StartsWith("NOTE"));
            var changeItems = _event.Items.Where(i => i.Value.StartsWith("CHAN"));
            var createItems = _event.Items.Where(i => i.Value.StartsWith("CREA"));
            var userDefinedItems = _event.Items.Where(i => i.Value.StartsWith("_"));

            result.EventText = _event.Value.GetSubstring(5);
            result.Date = dateItems.GetValue();
            result.Agency = agencyItems.GetValue();
            result.Address = addressITems.GetValue();
            result.Place = placeItems.GetValue();
            result.Latitude = latitudeItems.GetValue();
            result.Longitude = longitudeItems.GetValue();
            result.Cause = causeItems.GetValue();
            result.Sources = EntitySource.FromDataHierarchy(sourceItems, context);
            result.NoteIDs = noteItems.GetIDs("N");
            result.Change = DateTime.FromDataHierarchy(changeItems, context, DateTime.DateType.Change).LastOrDefault();
            result.Create = DateTime.FromDataHierarchy(createItems, context, DateTime.DateType.Create).LastOrDefault();
            result.UserDefinedTags = UserDefinedTag.FromDataHierarchy(userDefinedItems, context);

            return result;
        }
        #endregion
    }
}
