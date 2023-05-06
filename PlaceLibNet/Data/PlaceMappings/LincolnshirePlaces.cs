using System.Collections.Generic;
using System.Linq;

namespace PlaceLibNet.Data.PlaceMappings
{
    public class LincolnshirePlaces
    {


        public static IEnumerable<string> Get()
        {
            
            #region names
            string PlacesString =
            @"Aby, Acthorpe, Addlethorpe, Ailby, Aisby, South Kesteven, Aisby, West Lindsey, Aisthorpe, Alford, Algarkirk, Alkborough, Allington, Althorpe, Alvingham, Amber Hill, Amcotts, Ancaster, Anderby, Anwick, Anton's Gowt, Apley, Appleby, Asgarby, Sleaford, Asgarby and Howell, Asgarby, Spilsby, Ashby by Partney, Ashby cum Fenby, Ashby de la Launde, Ashby de la Launde and Bloxholm, Ashby Puerorum, Ashington End, Aslackby, Asserby, Asserby Turn, Asterby, Aswarby, Aswarby and Swarby, Aswardby, Atterby, Aubourn, Aubourn and Haddington, Audleby, Aunby, Aunsby, Austerby, Authorpe, Axholme, Aylesby,

Bag Enderby, Bardney, Barholm, Barholme, Barkston, Barkston Heath, Barlings, Barnetby, Barnoldby le Beck, Barrow Haven, Barrow upon Humber, Barrowby, Barton-upon-Humber, Bassingham, Bassingthorpe, Baston, Baumber, Beaumontcote, Beckingham, Beelsby, Beesby, Beesby with Saleby, Belchford, Belleau, Belton, Belnie, Belton, North Lincolnshire, Beltoft, Benington, Benniworth, Bicker, Bigby, Billingborough, Billinghay, Bilsby, Binbrook, Birthorpe, Biscathorpe, Bishop Bridge, Bishop Norton, Bitchfield, Blankney, Bleasby, Bloxholm, Blyborough, Blyton, Bonby, Bonsdale, Bonthorpe, Boothby Graffoe, Boothby Pagnell, Boston, Boswell, Bottesford, Boultham, Bourne, Braceborough, Bracebridge, Bracebridge Heath, Braceby, Brackenborough, Brackenborough with Little Grimsby, Bracon, Bradley, Brampton, Bransby, Branston, Branston Booths, Brandy Wharf, Brant Broughton, Bratoft, Brattleby, Brauncewell, Brigg, Brigsley, Brinkhill, Broadholme Brocklesby, Brookenby, Brothertoft, Broughton, Broxholme, Bucknall, Bulby, Bullington, Burgh le Marsh, Burgh on Bain, Burnham, Burringham, Burtoft, Burton, Burton Coggles, Burton Pedwardine, Burton upon Stather, Burwell, Buslingthorpe, Butterwick, Byards Leap,

Cabourne, Cadney, Cadney cum Howsham, Caenby, Caenby Corner, Cagthorpe, Caistor, Calcethorpe, Calceby, Cammeringham, Candlesby, Canwick, Careby, Carlton-le-Moorland, Carlton Scroop, Carrington, Casthorpe, Castle Carlton, Cawkwell, Cawthorpe, Caythorpe, Chapel Hill, Chapel St Leonards, Cherry Willingham, Claxby by Normanby, Claxby St Andrew, Claxby Pluckacre, Claypole, Claythorpe, Cleatham, Cleethorpes, Clixby, Coates by Stow, Cold Hanworth, Coleby (Lincolnshire), Coleby (North Lincolnshire), Colsterworth, Coningsby, Conisholme, Corby Glen, Corringham, Coskills, Counthorpe, Counthorpe and Creeton, Covenham St Bartholomew, Covenham St Mary, Cowbit, Cranwell, Creeton, Crofton, Croft, Crosby, Crowland, Crowle, Croxby, Croxton, Culverthorpe, Cumberworth, Cuxwold,

Dalby, Dalderby, Deeping Fen, Deeping St James, Dembleby, Denton, Derrythorpe, Derthorpe, Dexthorpe, Digby, Doddington, Dogdyke, Donington, Donington on Bain, Donna Nook, Dorrington, Dowsby, Dragonby, Driby, Drinsey Nook, Dry Doddington, Dunholme, Dunsby, Dunston, Dyke,

Eagle, Eagle Barnsdale, Eagle Moor, Ealand, East Barkwith, East Ferry, East and West Firsby, East Halton, East Heckington, East Keal, East Kirkby, East Lound, East Ravendale, East Stockwith, East Torrington, East Wykeham, Eastoft, Easton, Eastville, Edenham, Edlington, Edlington with Wispington, Elsham, Elsthorpe, Epworth, Eskham, Evedon, Ewerby, Ewerby and Evedon, Ewerby Thorpe,

Faldingworth, Farforth, Farlesthorpe, Fenton, South Kesteven, Fenton, West Lindsey, Fillingham, Firsby, Fishtoft, Fiskerton, Fleet, Fleet Hargate, Flixborough, Fockerby, Folkingham, Fonaby, Fordington, Fosdyke Bridge, Fosdyke, Foston, Fotherby, Frampton, Freiston, Freiston Shore, Friesthorpe, Frieston, Friskney, Frithville, Frodingham, Fulbeck, Fulletby, Fulnetby, Fulsby, Fulstow,

Gainsborough, Garthorpe, Gate Burton, Gautby, Gayton le Marsh, Gayton le Wold, Gedney, Gedney Broadgate, Gedney Church, Gedney Church End, Gedney Dawsmere, Gedney Drove End, Gedney Dyke, Gedney Hill, Gedney Marsh, Gelston, Gibraltar Point, Gipsey Bridge, Girsby, Glentham, Glentworth, Goltho, Gosberton, Gosberton Cheal, Gosberton Clough, Gosberton Risegate, Gosberton Westhorpe, Goulceby, Goxhill, Graby, Grainsby, Grainthorpe, Graizelound, Grange de Lings, Grantham, Grasby, Grayingham, Great Carlton, Great Coates, Great Gonerby, Great Grimsby, Great Hale, Great Limber, Great Ponton, Great Steeping, Great Sturton, Greatford, Greenfield Greetham, Greetham with Somersby, Greetwell, North Lincolnshire, Greetwell, West Lindsey, Grimblethorpe, Grimoldby, Grimsby, Grimsthorpe, Guthram Gowt, Gunby, South Kesteven, Gunby and Stainby, Gunby, East Lindsey, Gunness, Gunthorpe,

Habertoft, Habrough, Haceby, Hackthorn, Haconby, Haddington, Hagnaby, Hagworthingham, Hainton, Hallington, Haltham, Haltoft End, Halton Holegate, Hameringham, Hanby, Hannah cum Hagnaby, Hanthorpe, Hardwick, Hareby, Harlaxton, Harmston, Harpswell, Harrington, Harrowby, Harrowby Hall, Hasthorpe, Harts Ground, Hatcliffe, Hatton, Haugh, Haugham, Haverholme, Haverholme Priory, Hawthorn Hill, Hawthorpe, Haxey, Healing, Heapham, Heckington, Heighington, Helpringham, Helsey, Hemingby, Hemswell, Hemswell Cliff, Heydour, Hibaldstow, High Dyke, High Toynton, Hilldyke, Hoffleet Stow, Hogsthorpe, Holbeach, Holbeach Bank, Holbeach Clough, Holbeach Drove, Holbeach Fen, Holbeach Hurn, Holbeach Marsh, Holbeach St. Johns, Holbeach St Marks, Holbeach St Matthew, Holbeck, Holland Fen, Holland Fen with Brothertoft, Holme, North Lincolnshire, Holme, West Lindsey, Holton cum Beckering, Holton le Clay, Holton le Moor, Holywell, Honington, Hop Pole, Horbling, Horkstow, Horncastle, Horsington, Hough on the Hill, Hougham, Howell, Howsham, Hubberts Bridge, Humberston, Hundleby, Huttoft,

Immingham, Ingham, Ingleby, Ingoldmells, Ingoldsby, Irby in the Marsh, Irby, Irnham, Isle of Axholme,

Jerusalem,

Kate's Bridge, Keadby, Keal Cotes, Keddington, Keelby, Keisby, Kelby, Kelfield, Kelstern, Ketsby, Kettleby, Kettlethorpe, Kexby, Kingerby, Kingthorpe, Kirkby cum Osgodby, Kirkby Green, Kirkby la Thorpe, Kirkby on Bain, Kirkby Underwood, Kirkstead, Kirmington, Kirmond le Mire, Kirton, Kirton End, Kirton Holme, Kirton in Lindsey, Knaith

Laceby, Lade Bank, Langrick, Langtoft, Langton by Spilsby, Langton by Wragby, Langton near Horncastle, Langworth, Laughton, South Kesteven, Laughton, West Lindsey, Laughterton, Lavington, Lea, Leadenham, Leake Commonside, Leake Hurns End, Leasingham, Legbourne, Legsby, Lenton, Lenton, Keisby and Osgodby, Leverton, Limber Parva, Lincoln, Lincolnshire Gate, Linwood, Lissington, Little Bytham, Little Carlton, Little Cawthorpe, Little Coates, Little Gonerby, Little Hale, Little Humby, Little London, Stallingborough, Little London, Spalding, Little Ponton, Little Ponton and Stroxton, Little Steeping, Lobthorpe, Londonthorpe, Londonthorpe and Harrowby Without, Long Bennington, Long Sutton, Louth, Lound, Low Fulney, Low Toynton, Ludborough, Luddington, Ludford, Ludford Magna, Ludford Parva, Ludney, Lusby, Lutton,

Mablethorpe, Maidenwell, Maltby, Maltby le Marsh, Manby, Manthorpe, Bourne, Manthorpe, Grantham, Manton, Mareham le Fen, Mareham on the Hill, Markby, Market Deeping, Market Rasen, Market Stainton, Marshchapel, Marston, Martin, Martin near Horncastle, Marton, Mavis Enderby, Mawthorpe, Medlam, Messingham, Metheringham, Middle Rasen, Midville, Miningsby, Minting, Monksthorpe, Moorby, Moorhouses, Moortown, Morton by Gainsborough, Morton and Hanthorpe, Morton near Eagle, Morton Hall, Moulton Chapel, Moulton near Holbeach, Moulton Seas End, Muckton, Mumby,

Navenby, Nettleham, Nettleton, Newbo, New Bolingbroke, New Holland, New Leake, New Quarrington, New Waltham, New York, Newtoft, Newton, Newton and Haceby, Newton by Toft, Newton on Trent, Nocton, Normanby by Spital, Normanby le Wold, Normanton, Normanton on Cliffe, North Carlton, North Coates, North Cockerington, North Drove, North Elkington, North Forty Foot Bank, North Gulham, North Hykeham, North Kelsey, North Killingholme, North Kyme, North Ormsby, North Owersby, North Rauceby, North Reston, North Scarle, North Somercotes, North Stoke, North Thoresby, North Willingham, North Witham, Northorpe, South Kesteven, Northorpe, South Holland, Northorpe, West Lindsey Norton Disney, Nunsthorpe,

Oasby, Obthorpe, Old Bolingbroke, Old Clee, Old Leake, Old Somerby, Orby, Osbournby, Osgodby, Otby, Owersby, Owmby, Owmby by Spital, Owston Ferry, Oxcombe,

Panton, Partney, Peak Hill, Pelhams Land, Pepper Gowt Plot, Pickworth, Pinchbeck, Pode Hole, Pointon, Poolham, Potterhanworth, Potterhanworth Booths, Pyewipe,

Quadring, Quadring Eaudike, Quadring Fen, Quarrington,

Raithby by Spilsby, Raithby cum Maltby, Ranby, Rand, Redbourne, Reepham, Reston, Revesby, Riby, Rigsby, Rippingale, Riseholme, Ropsley, Ropsley and Humby, Rothwell, Roughton, Rowlands Marsh, Rowston, Roxby, Roxholme, Ruckland, Ruskington,

Saleby, Salmonby, Saltfleet, Saltfleetby, Sandilands, Sandtoft, Sapperton, Saracens Head, Sausthorpe, Saxby, Saxby All Saints, Saxilby, Scamblesby, Scampton, Scartho, Scawby, Scopwick, Scothern, Scotland, Scotter, Scotterthorpe, Scotton, Scrub Hill, Scott Willoughby, Scrafield, Scrane End, Scredington, Scremby, Scrivelsby, Scunthorpe, Scupholme, Scottlethorpe, Searby, Sedgebrook, Sempringham, Shepeau Stow, Short Ferry, Sibsey, Sibsey Northlands, Silk Willoughby, Sixhills, Skegness, Skeldyke, Skellingthorpe, Skendleby, Skidbrooke, Skillington, Skinnand, Skirbeck, Sleaford, Snarford, Snelland, Snitterby, Somerby (Juxta Bigby), Somerby (near Gainsborough), Somersby, Sotby, Sot's Hole, South Carlton, South Cockerington, South Elkington, South Ferriby, South Gulham, South Hykeham, South Kelsey, South Killingholme, South Kyme, South Ormsby, South Owersby, South Rauceby, South Reston, South Somercotes, South Stoke, South Thoresby, South Willingham, South Witham, Southorpe, Southrey, Spalding, Spanby, Spilsby, Spital-in-the-Street, Spitalgate, Spridlington, Springthorpe, Stainby, Stainfield, Stainfield near Bourne, Stainton by Langworth, Stainton le Vale, Stallingborough, Stamford, Stapleford, Stenigot, Stewton, Stickford, Stickney, Stixwould, Stixwould and Woodhall, Stoke Rochford, Stow, Stow Green, Stragglethorpe, Stroxton, Strubby, Stubton, Sturton, North Lincolnshire, Sturton by Stow, Sudbrook, Sudbrooke, Surfleet, Susworth, Sutterby, Sutterton, Sutton Bridge, Sutton Le Marsh, Sutton-on-Sea, Sutton St Edmund, Sutton St James, Sutton St Nicholas, Sutton St Mary, Swaby, Swallow, Swarby, Swaton, Swayfield, Swinderby, Swineshead, Swinethorpe, Swinstead, Syston,

Tallington, Tanvats, Tathwell, Tattershall, Tattershall Bridge, Tattershall Thorpe, Tealby, Temple Bruer, Tetford, Tetley, Tetney, Thealby, Theddlethorpe, Thetford, Thimbleby, Thompson's Bottom, Thoresthorpe, Thoresway, Thorganby, Thornton, Thornton Curtis, Thornton le Fen, Thornton le Moor, Thorpe, Thorpe, Thorpe in the Fallows, Thorpe Latimer, Thorpe Park, Thorpe on the Hill, Thorpe St Peter, Thorpe Tilney, Thonock, Three Bridges, Threekingham, Threekingham Bar, Throckenholt, Thurlby, Thurlby by Bourne, Thurlby, North Kesteven, Timberland, Toft, Toft Hill, Toft next Newton, Tongue End, Torksey, Tothby, Tothill, Toynton All Saints, Toynton Fen Side, Toynton St Peter, Trusthorpe, Tumby, Tumby Moorside, Tumby Woodside, Tupholme, Twenty, Tydd Gote, Tydd St Mary,

Uffington, Ulceby, East Lindsey, Ulceby with Fordington, Ulceby, North Lincolnshire, Ulceby Skitter, Upton, Usselby, Utterby,

Waddingham, Waddington, Waddingworth, Wainfleet, Wainfleet St Mary, Walcot, North Lincolnshire, Walcot, Walcott, Waithe, Walesby, Walmsgate, Walkerith, Waltham, Washingborough, Wasps Nest, Waterton, Welbourn, Welby, Well, Wellingore, Welton, Welton le Marsh, Welton Le Wold, West Butterwick, West Halton, West Ashby, West Barkwith, West Deeping, West Keal, West Marsh, West Rasen, West Ravendale, West Torrington, West Wykeham, Westborough, Westby, Weston, Weston Hills, Westville, Westwoodside, Whaplode, Whaplode Drove, Whaplode St Catherine, Whisby, Whisby Moor, Whitton, Wickenby, Wigtoft, Wildmore, Wildsworth, Wilksby, Willingham by Stow, Willoughby, Willoughton, Wilsford, Wilsthorpe, Winceby, Windsor, Winteringham, Winterton, Winthorpe, Wispington, Witham Marsh, Witham on the Hill, Witham St Hughs, Withcall, Withern, Wold Newton, Wood Enderby, Woodhall Spa, Woolsthorpe-by-Belvoir, Woolsthorpe-by-Colsterworth, Wootton, Worlaby, Wragby, Wragholme, Wrangle, Wrawby, Wressle, Wroot, Wyberton, Wyham, Wykeham (Nettleton), Wykeham (Weston), Wyville with Hungerton,

Yaddlethorpe, Yarburgh, Yawthorpe";


            #endregion

            return PlacesString.Split(',').ToList().Select(s => s.Trim().ToLower()).ToList();
        }
    }


}
