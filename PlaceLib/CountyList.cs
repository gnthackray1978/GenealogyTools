using PlaceLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVAnalyser
{
    public class Location : ILocation
    {
        private List<string> _aliases = new List<string>();

        public List<string> Aliases => _aliases;

        public string Name { get; set; }

        public void AddAlias(string value)
        {
            if (value != null)
            {
                _aliases.Add(value);
            }
        }
    }

    public class CountyList :ILocationList
    {
        private List<ILocation> _locations = new List<ILocation>();

        public CountyList()
        {

        }

        public List<ILocation> Locations => _locations;

        public static IEnumerable<string> Get()
        {

            #region names
            string PlacesString =
            @"Avon,,,,,,|
Bedfordshire,,,,,,|
Berkshire,,,,,,|
Buckinghamshire,,,,,,|
Cambridgeshire,,,,,,|		
Cheshire,,,,,,|
Cleveland,,,,,,|
Cornwall,,,,,,|
Cumberland,,,,,,|	
Cumbria,,,,,,|
Derbyshire,,,,,,|
Devon,,,,,,|
Dorset,,,,,,|
Durham,,,,,,|
East Suffolk,,,,,,|	
East Sussex,,,,,,|
Essex,,,,,,|
Gloucestershire,,,,,,|
London,,,,,,|
Manchester,,,,,,|
Hampshire,,,,,,|
Herefordshire,,,,,,|
Hertfordshire,,,,,,|
Humberside,,,,,,|
Huntingdonshire,,,,,,|
Ely,,,,,,|
Isle of Wight,,,,,,|
Kent,,,,,,|
Lancashire,,,,,,|
Leicestershire,,,,,,|
Lincolnshire,,,,,,|
London,,,,,,|
Merseyside,,,,,,|
Middlesex,,,,,,|
Norfolk,,,,,,|
Northamptonshire,,,,,,|
Northumberland,,,,,,|
Nottinghamshire,,,,,,|
Oxfordshire,,,,,,|
Rutland,,,,,,|
Shropshire,,,,,,|
Somerset,,,,,,|
Staffordshire,,,,,,|
Suffolk,,,,,,|
Surrey,,,,,,|
Sussex,,,,,,|	
Tyne and Wear,,,,,,|
Warwickshire,,,,,,|
Westmorland,,,,,,|	
Suffolk,,,,,,|		
Wiltshire,,,,,,|
Worcestershire,,,,,,|
		

Antrim,,,,,,|
Armagh,,,,,,|
Belfast,,,,,,|
Down,,,,,,|
Fermanagh,,,,,,|
Londonderry,,,,,,|
Derry,,,,,,|
Tyrone,,,,,,|

Aberdeen,,,,,,|
Aberdeenshire,,,,,,|
Angus,,,,,,|
Argyll,,,,,,|
Ayrshire,,,,,,|
Banffshire,,,,,,|
Berwickshire,,,,,,|
Bute,,,,,,|
Caithness,,,,,,|
Clackmannanshire,,,,,,|
Cromartyshire,,,,,,|		
Dumfriesshire,,,,,,|
Dunbartonshire,,,,,,|
Dundee,,,,,,|
Lothian,,,,,,|
Edinburgh,,,,,,|
Fife,,,,,,|
Glasgow,,,,,,|
Inverness,,,,,,|
Kincardineshire,,,,,,|
Kinross,,,,,,|
Kirkcudbrightshire,,,,,,|
Lanarkshire,,,,,,|
Midlothian,,,,,,|
Moray,,,,,,|
Nairnshire,,,,,,|
Orkney,,,,,,|
Peeblesshire,,,,,,|
Perthshire,,,,,,|
Renfrewshire,,,,,,|
Ross and Cromarty,,,,,,|
Ross-shire,,,,,,|
Roxburghshire,,,,,,|
Selkirkshire,,,,,,|
Shetland,,,,,,|
Stirlingshire,,,,,,|
Sutherland,,,,,,|
West Lothian,,,,,,|
Wigtownshire,,,,,,|

Anglesey,,,,,,|
Brecknockshire,,,,,,|		
Caernarfonshire,,,,,,|	
Cardiganshire,,,,,,|	
Carmarthenshire,,,,,,|
Clwyd,,,,,,|
Denbighshire,,,,,,|	
Dyfed,,,,,,|
Flintshire,,,,,,|
Glamorgan,,,,,,|			
Gwent,,,,,,|
Gwynedd,,,,,,|
Merionethshire,,,,,,|			
Mid Glamorgan,,,,,,|
Monmouthshire,,,,,,|
Montgomeryshire,,,,,,|		
Pembrokeshire,,,,,,|
Powys,,,,,,|
Radnorshire,,,,,,|			
South Glamorgan,,,,,,|
West Glamorgan,,,,,,|
Wrexham,,,,,";


            #endregion

            return PlacesString.Split('|').ToList().Select(s => s.Trim().ToLower()).ToList();
        }

        public string Find(string testValue)
        {
            string retVal = "";

            foreach (var l in _locations)
            {
                if (l.Name.Intersection(testValue))
                {
                    retVal = l.Name;
                    break;
                }

                var found = l.Aliases.Any(alias => alias.Intersection(testValue));

                if (found)
                {
                    retVal = l.Name;
                    break;
                }

            }

            return retVal;
        }

        public void Generate()
        {
            //efdb.m1 m = new m1();

            //foreach (var c in m.Counties)
            //{
            //    var location = new Location { Name = c.Name };

            //    location.AddAlias(c.Alias1);
            //    location.AddAlias(c.Alias2);
            //    location.AddAlias(c.Alias3);
            //    location.AddAlias(c.Alias4);

            //    Locations.Add(location);
            //}
        }
    }
}

