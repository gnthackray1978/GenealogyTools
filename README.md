## GenealogyTools

### AzureContext
  *Personal projects db mostly holding genealogy information*  
  
  EFCore AzureDB   
    
### LoggingLib
 *essentially a logging interface.
 different implementations can be provided for different situations.
 so when running in a console implementation is for the console
    when running in a backend service output is sent via signalr to the browser.*
    
### DNAGedLib 
  **dnaged is a program that downloads the trees of ancestrydna matches and stores
  them in sqlite db.*

  DNAGedImporter contains a ImportDataStore object. This has a bunch of collections related to what's in the dnaged sqlite db. It also has DNAGEDContext which a EFCore context for the genDB.db sqlite db. 
  
  importation process broken down into stages

  first stage reads dnaged data into  SQLliteTrees  ICWs  MatchDetails  MatchGroups Profiles 

  subsequent stages import that data into genDB.db
  
  read/write is mostly handled by SqlLiteCommands as EFCore turned out to be extremely slow.

  whole thing is started by calling 
  DNAGedImporter.Import();
  
### FTMConsole2
 - cmd line ui

### FTMContextNet
 *project to extract and transform data from the ftm db
  
  ftm db typically will contain numerous family trees.
  ftm cache stores the people from those trees, and meta data about them. 
  meta data includes which of them are duplicates, the lat and long of where they were born/died, aswell 
  as things like which tree they belonged to originally.
  
  
 *

 2 sqlite Context files accessed with efcore and for performance reasons custom sqlite commands
 
 FTMakerContext
  this is the original decrypted ftm context
  
 FTMakerCacheContext 
  this is basically all the usefull stuff out of the ftmakercontext
  It has 5 tables FTMPersonOrigins  TreeRecords  DupeEntries   FTMPersonView  FTMPlaceCache 
  FTMPersonOrigins  TreeRecords  DupeEntries   FTMPersonView all cleared down at the start of every new
  data import.
  FTMPlaceCache is never cleared down. We only add missing entries to it and update any changed locations.
  

### GedcomImporter

 *Code to import ged files into SQLite database. I'm pretty sure I found the ged parser online many years ago. But can't now remember the source. CRUD is mostly performed by SqliteCommands as the performance of EFCore was rubbish with Sqlite at the time of writing.  *



### GenDataAPI
 Basic WebAPI implementation with no auth. To allow Web based UI.
 Logging support provided by signalr.
 Calls methods on facade in FTMContextNet.
 
 
### GenDBContextNET
 data context for GedcomImporter
 
### GenealogyTools
 command line UI for PersonDupeLib
 
### LincsWillScraper
 basically a mad load of regex hacks that parses pages downloaded using HttpClient. results then saved to the db with efcore.
 
### PersonDupeLib
 code to try and find potential dupes in the genDB.db works by comparing birthyear christianname surname birthcounty and grouping people  together if all 4 things are the same.

### PlaceLib
 wrapper around massive uk place db. 
 
 PlaceLib
  sqlite db containing UK places with lat and long county etc

### Rootslib
 parser to import family trees from RootsChat into a sqlite db. used even less than the rest of this stuff! 

