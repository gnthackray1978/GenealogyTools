using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using DNAGedLib.Models;
using PlaceLib;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Permissions;
using GenDBContext.Models;

namespace DNAGedLib
{

    public class ImportationContext  
    {
        #region props

        public string Path;

        public string Destination { get; set; }

        public DNAGEDContext DNAGedContext { get; set; }

        public List<MatchTreeEntry> SQLliteTrees { get; set; } = new List<MatchTreeEntry>();

        public List<ICW> ICWs { get; set; } = new List<ICW>();

        public List<MatchDetail> MatchDetails { get; set; } = new List<MatchDetail>();

        public List<MatchGroups> MatchGroups { get; set; } = new List<MatchGroups>();

        public List<MatchKitName> Profiles { get; set; } = new List<MatchKitName>();

        public Guid ImportTestId { get; set; }

        public DateTime CutOff { get; set; }

        #endregion

        public ImportationContext() {
            DNAGedContext = new DNAGEDContext();
            Path = @"C:\Users\george\Documents\DNAGedcom.db";
            Destination = @"Data Source=DESKTOP-KGS70RI\SQL2016EX;Initial Catalog=DNAGED;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            CutOff = new DateTime(2019, 7, 1);
        }


      
       

        private static bool CustomExceptions(Persons p)
        {
            if (p.BirthPlace.ToLower().Contains("victoria"))
            {
                p.BirthCounty = "New South Wales";
                p.BirthCountry = "Australia";
                return true;
            }

            if (p.BirthPlace == "en" || p.BirthPlace == "eng" || p.BirthPlace == "uk")
            {
                p.BirthCounty = "";
                p.BirthCountry = "England";
                return true;
            }

            if (p.BirthPlace == "Godmanchester")
            {
                p.BirthCounty = "Huntingdonshire";
                p.BirthCountry = "England";
                return true;
            }

            if (p.BirthPlace == "Goole")
            {
                p.BirthCounty = "Yorkshire";
                p.BirthCountry = "England";
                return true;
            }
            
            if (p.BirthPlace == "Hartford, Hunts")
            {
                p.BirthCounty = "Huntingdonshire";
                p.BirthCountry = "England";
                return true;
            }
            
            if (p.BirthPlace == "Linconshire")
            {
                p.BirthCounty = "Lincolnshire";
                p.BirthCountry = "England";
                return true;
            }




            return false;
        }



    }






}
