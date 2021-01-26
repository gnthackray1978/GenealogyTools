using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace PlaceLib
{
    public class YorkshirePlaces
    {
        public static IEnumerable<string> ShortListGet()
        {
            string PlacesString = @"Leeds, hunsingore,aberford";



            return PlacesString.Split(',').ToList().Select(s => s.Trim().ToLower()).ToList();
        }

        public static IEnumerable<string> Get()
        {
            // Aston, 



            #region names
            string PlacesString =
            @"Aberford, Acaster Malbis, Acaster Selby, Acklam, Acomb, Addingham, Adlingfleet, Adwick le Street, Agbrigg, Agglethorpe, 
Aike, Ainderby Quernhow, Ainderby Steeple, Ainthorpe, Aire View, Airmyn, Airton, Aiskew, Aislaby, Akebar, Aldborough, Aldbrough, Aldbrough St John, 
Aldfield, Aldwark, Allerston, Allerthorpe, Allerton, Allerton Mauleverer, Almondbury, Alne, Altofts, Alverthorpe, Amotherby, Ampleforth, Angram, 
Anlaby, Anlaby Common, Anston, Appersett, Applegarth, Appleton Roebuck, Appleton Wiske, Appleton-le-Moors, Appleton-le-Street, Ardsley, Arkendale, 
Arkle Town, Armthorpe, Arncliffe, Arnold, Arram, Arrathorne, Arthington, Asenby, Askham Bryan, Askham Richard, Askrigg, Askwith, Asselby, 
Athersley, Atley Hill, Atwick, Aughton,Austonley, Austwick, Aysgarth, Azerley,

Bagby, Baildon, Bainbridge, Bainton, Balkholme, Bardsey, Barlby, Barmby Moor, Barmby on the Marsh, Barmston, Barnoldswick, Barkston Ash, Barnsley, 
Barugh, Barugh-Green, Barwick-in-Elmet, Batley, Battersby, Beal, Beeford, Beggarington Hill, Belby, Bellasize, Bempton, Beningbrough, Bennetland, 
Benningholme, Bentley, Berry Brow, Bessacarr, Bessingby, Beswick, Beverley, Bewholme, Bielby, Bilbrough, Billingley, Bilton,Bilton-in-Ainsty, Bingley, Birdsedge, 
Birdwell, Birstall, Birstwith, Bishop Burton, Bishop Wilton, Blacktoft, Blubberhouses, Bolton, Bolton on Dearne, Boothferry, Boothtown, Boroughbridge,
Borrowby, Boston Spa, Boulderclough, Boynton, Bradford, Bradley, Braithwell, Bramham, Bramham cum Oglethorpe, Bramhope, Bramley, 
Leeds, Bramley, South Yorkshire, Brampton, Brandesburton, Brantingham, Brayton, Breighton, Bridge Hewick, Bridlington, Brierley, Brigham, Brighouse, Brind, 
Brinsworth, Broadgate, Brockholes, Brokes, Broomfleet, Brotherton, Brough, Broughton, Buckden, Buckton, Bubwith, Bugthorpe, Burley, Burley-in-Wharfedale, 
Burley Woodhead, Burnby, Burnsall, Burnt Yates, Bursea, Burshill, Burstwick, Burton Agnes, Burton Constable, Burton Fleming, Burton Leonard, Burton Pidsea, 
Burton Salmon, Buttercrambe,

Calcutt, Calverley, Camblesforth, Camerton, Canklow, Carlecotes, Carleton, Carlton in Cleveland,  Carlton, Carlton Husthwaite, Carlton Miniott, Carnaby, Cartworth, 
Castleford, Catcliffe, Catfoss, Catterick, Catwick, Cawthorne, Cawood, Cherry Burton, Church End, Clayton,Cleckheaton, 
Clifton, Coniston, Conistone, Cononley, Copley, Cotness, Cottam, Cottingham, Countersett, Cowden, Cowlam, Cowling, Coxwold, Crambe, Cranswick, 
Crawshaw, Cray, Crofton, Croome, Cropton, Crossflatts, Cross Hills, Cubeck, Cubley, South Yorkshire, Cudworth, Cundy Cross,

Dallowgill, Dalton , Damems, Danby, Danby Wiske, Danthorpe, Darfield, Darton, Denby Dale, Denholme, Denton, Dewsbury, Dinnington, Dodworth, Doncaster, Drewton, 
Driffield, Drighlington, Dringhoe, Drub, Dunnington, Dunford Bridge, Dunswell,

Earby, Easington,  Easingwold, East Barnby, East Cottingwith, East Cowick, East Knapton, East Newton, Eastburn, Eastrington, East Rigton, Eccleshill, Edlington, 
Egton, Egton Bridge, Elland, Ellerker, Ellerton, Elloughton, Elmswell, Elsecar, Elstronwick, Embsay, Emley, Emmotland, Eppleworth, Eryholme, Eske, Eston, 
Etherdwick, Etton, Everingham, Everthorpe,

Fangfoss, Farnhill, Farnley, Farsley, Faxfleet, Featherstone, Felixkirk, Fewston, Filey, Fimber, Firbeck, Fitling, Flamborough, Fleatwood, Flinton, Flockton, 
Flockton Green, Foggathorpe, Fordon, Foston on the Wolds, Foxholes, Fox Royd, Fraisthorpe, Frickley, Fridaythorpe, Full Sutton, Fylingdales,

Ganstead, Gardham, Garforth, Gargrave, Garrowby, Garton, Garton on the Wolds, Gawber, Gembling, Giggleswick, Gilberdyke, Gildersome, Gildingwells, Gilling East, 
Gilling West, Gilroyd, Girsby, Glasshouses, Glusburn, Goathland, Goldsborough,  Goldthorpe, Goodmanham, Goole, Gowdall, 
Gowthorpe, Goxhill, Gransmoor, Grassington, Greasbrough, Great Ayton, Great Cowden, Great Givendale, Great Hatfield, Great Houghton, Great Kelk, Green Hammerton, 
Greenhow Hill, Grewelthorpe, Gribthorpe, Grimethorpe, Grimston, Grindale, Grosmont, Guisborough, Guiseley, Gunby,

Haisthorpe, Halifax, Halsham, Hambleton, Harden, Harlthorpe, Harehills, Harehills Corner, Harpham, Harrogate, Harswell, Harthill, Hasholme, Hawes, Haworth, 
Haxby, Hayton, Hazlewood, Hebden, Hebden Bridge, Heckmondwike, Hedon, Hellaby, Hellifield, Helme, Helmsley, Helwith, Hemingbrough, Hempholme, Hemsworth, 
Hensall, Heptonstall, Hepworth, Herringthorpe, Heslington, Hessle, Heworth, Hickleton, Highgate, High Birkwith, High Catton, High Gardham, High Green, High Hoyland, 
High Hunsley, Higham, Hilston, Hive, Hollym, Holme, Holme on the Wolds, Holme-on-Spalding-Moor, Holmfirth, Holmpton, Honeywell, Honley, Hood Green, Hook, Hooton Pagnell, 
Horbury, Hornby, Hornsea, Horsforth, Hotham, Hotheroyd, Houghton, Hovingham, Howden, Howdendyke, Hoyland, Hoylandswaine, Hoyland Common, Hubberholme, Huddersfield, 
Huggate, Hull, Hull Bridge, Humbleton, Hunmanby, Hunslet, Hunton, Hutton Buscel, Hutton Cranswick, Hutton Rudby,

Idle, Ilkley, Ilton, Ingbirchworth, Ingleton, Ingrow,

Jagger Green, Jaw Hill, Jump,

Kearton, Keighley, Kelleythorpe, Kendray, Kettlewell, Kexbrough, Kexby, Keyingham, Kildwick, Kilham, Kilnsea, Kilnsey, Kilnwick, Kilnwick Percy, Kilpin, 
Kilpin Pike, Kimberworth, Kimberworth Park, Kingstone, Kingston upon Hull, Kinsley, Kiplingcotes, Kirby Grindalythe, Kirby Misperton, Kirby Underdale, 
Kirk Deighton, Kirk Ella, Kirkburn, Kirkbymoorside, Kirkheaton, Kirkleatham, Kirklington, Kiveton Park, Knapton, Knaresborough, Knedlington, Knottingley,

Lane, Langsett, Langtoft, Laughton Common, Laughton-en-le-Morthen, Laverton, Laxton, Laytham, Lealholm, Leconfield, Leeds, Lelley, Leppington, Letwell, 
Leven, Levisham, Levitt Hagg, Leyburn, Lightcliffe, Linthorpe, Linthwaite, Lissett, Little Catwick, Little Driffield, Little Hatfield, Little Houghton, 
Little Kelk, Little London, Little Preston, Little Reedness, Little Weighton, Lockington, Lockton, Lofthouse, Loftus, Londesborough, Londonderry, 
Long Riston, Long Preston, Lothersdale, Low Catton, Lowthorpe, Low Worsall, Luddenden, Lund, Lundwood,

Malham, Maltby, Malton, Manvers, Mappleton, Mapplewell, Market Weighton, Marsden, Marske-by-the-Sea, Marton, Meaux, Melbourne, Meltham, Melton, Meltonby, 
Menston, Metham, Methley, Mexborough, Micklebring, Micklefield, Middlecliffe, Middleham, Middlesbrough, Middlesmoor, Middlethorpe, Middleton, Middleton on the Wolds, 
Millhouses, Millhouse Green, Millington, Mixenden, Molescroft, Monk Bretton, Morley, Morthen, Muker, Mytholmroyd,

Nafferton, Nether Poppleton, Nether Silton, Netherthong, Nettleton Hill, New Earswick, New Edlington, New Ellerby, New Farnley, New Lodge, New Mill, 
New Village, Newland, Newport, Newsholme, Newton upon Derwent, Noblethorpe, Normanby, Normanton, Northallerton, Norton-on-Derwent, 
North Cave, North Cliffe, North Dalton, North Duffield, North Ferriby, North Frodingham, North Howden, North Newbald, North Rigton, Northowram,
Norton-on-Derwent, Nosterfield, Notton, Nunburnholme, Nunkeeling, Nunthorpe,

Oakworth, Octon, Old Edlington, Old Ellerby, Old Lindley, Oldtown, Ormesby, Osgodby, Osmotherley, Ossett, Otley, Ottringham, Ousefleet, Ousethorpe, Out Newton, 
Outwood, Ovenden, Overthorpe, Overton, Owsthorpe, Owstwick, Oxenhope, Oxspring,

Painsthorpe, Pannal, Parkgate, Pateley Bridge, Patrington, Patrington Haven, Paull, Penistone, Pickering, Pickhill, Pilley, Platts Common, Pocklington, 
Pollington, Pontefract, Portington, Pool-in-Wharfedale, Preston, Preston-under-Scar, Primrose Valley, Pudsey,

Queensbury,

Ramsgill, Rawcliffe, Rawcliffe Bridge, Ravenfield, Ravenscar, Rawdon, Rawmarsh, Raywell, Redcar, Redmire, Reedness, Reighton, Richmond, Rievaulx, Rimswell, 
Riplingham, Ripon, Risby, Rise, Robin Hood's Bay, Rolston, Rookwith, Roos, Rossington, Rotherham, Rothwell, Rotsea, Routh, Rowley, Roxby, Roydhouse, Royston, 
Rudston, Ruston, Ruston Parva, Ruswarp, Ryecroft, Ryehill,

Salt End, Saltaire, Saltburn-by-the-Sea, Saltmarshe, Sancton, Sandholme, Sandsend, Sawdon Scaling, Scalby, Scarborough, Scarcroft, Scawsby,  Scholes, Scholes, 
Scorborough, Scorton, Scotton,Seamer, Seaton, Seaton Ross, Sedbergh, Selby, Settle, Sewerby, Shafton, Sheffield, Shelf, Shibden, Shipley, Shiptonthorpe, Sigglesthorne, 
Silkstone, Silkstone Common, Silpho, Silsden, Skeffling, Skelton, Skerne, Skidby, Skipsea, Skipsea Brough, Skipton, Skirlaugh, Skirlington, Skirpenbeck, Slaithwaite, 
Sledmere, Sleights, Smithies, Snainton, Snaith, Sneaton, South Bank, South Cave, South Cliffe, South Dalton, South Hiendley, South Newbald, South Ossett, Southburn, 
Southowram, Sowerby, Sowerby Bridge, Spaldington, Speeton, Spennithorne, Spofforth, Sproatley, Spurn Head, Stainburn, Stainland, Staincross, Stainsacre, Staintondale, 
Stairfoot, Staithes, Stamford Bridge, Stanbury, Stanley, Starbeck, Staxton, Steeton, Stean, Stocksbridge, Stockton-on-the-Forest, Stokesley, Storwood, Strensall, Suffield, 
Summerbridge, Sunderlandwick, Sutton Bank, Sutton-in-Craven, Sutton-on-Hull, Sutton-on-the-Forest, Sutton-under-Whitestonecliffe, Sutton upon Derwent, Swallownest, 
Swanland, Swillington, Swine, Swinefleet, Swinton,

Tadcaster, Tankersley, Tansterne, Teesville, Templeborough, Terrington, Thearne, Thirn, Thirsk, Thirtleby, Thornaby-on-Tees, Thorncliff, Thorne, Thorner, Thorngumbald, 
Thornholme,  Thornton, Thornton Dale, Thornton-in-Craven, Thornton in Lonsdale, Thornton-le-Clay, Thornton-le-Moor, Thornton-le-Street, 
Thornton-on-the-Hill, Thornton Rust, Thornton Steward, Thornton Watlass, Thorpe, Thorpe le Street, Thorpe Hesley, Thorpe Salvin, Threshfield, Thrintoft, Thrybergh, 
Thunder Bridge, Thurcroft, Thurgoland, Thurlstone, Thurnscoe, Thwing, Tibthorpe, Tickhill, Tickton, Todmorden, Todwick, Tollingham, Towthorpe, Treeton, Trumfleet,
Tunstall, Tyersal,

Uckerby, Ugthorpe, Ulleskelf, Ulley, Ulrome, Uncleby,

Vale of Pickering,

Wakefield, Walkington, Wansford, Waplington, Ward Green, Warter, Wassand, Wath, Wath-in-Nidderdale, Wath-upon-Dearne, Watton, Wauldby, Wawne, 
Waxholme, Weel, Weeton, Welhambridge, Welton, Welwick, Wentworth, West Ayton, West Barnby, West Burton, West Cowick, West Ella, West Hauxwell, 
West Knapton, West Melton, West Newton, Wetherby, Wetwang, Wheldrake, Whiston, Whitby, Whitgift, Whitwell-on-the-Hill, Wickersley, Wigginton, 
Wigglesworth, Wilberfoss, Wilfholme, Willerby, Willitoft, Wilsden, Wilsill, Wilsthorpe, Wilthorpe, Windel, Winestead, Winterburn, Withernsea, 
Withernwick, Wold Newton, Woodale, Woodhall, Woodmansey, Woolley Colliery, Wombwell, Woodsetts, Wortley, Worsbrough, Worton, Wrelton, Wressle,  Wykeham, Wyton


Yafforth, Yapham, Yarm, Yeadon, Yearsley, Yedingham, Yockenthwaite, Yokefleet, York, Youlthorpe, Youlton";


            #endregion

            return PlacesString.Split(',').ToList().Select(s => s.Trim().ToLower()).ToList();
        }
    }


}
