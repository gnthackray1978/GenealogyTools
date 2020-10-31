using GedcomParser.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using DNAGedLib;
using Microsoft.EntityFrameworkCore;
using GenDBContext.Models;
using DNAGedLib.Models;
using GedcomParser.Entities;
using Microsoft.Data.Sqlite;

namespace GedcomImporter
{
    public class GedcomImporter
    {        
        public static DateObj GetDateObj(DatePlace birthPlace, DatePlace baptismPlace)
        {
            var returnObj = new DateObj() { 
                Place="",
                DateStr="",
                YearInt=0
            };

            if (birthPlace != null)
            {
                if (!string.IsNullOrEmpty(birthPlace.Date))
                {
                    returnObj.YearInt = MatchTreeHelpers.ExtractInt(birthPlace.Date);
                    returnObj.DateStr = birthPlace.Date;
                }

                if (!string.IsNullOrEmpty(birthPlace.Place))
                {
                    returnObj.Place = birthPlace.Place;
                }
            }

            if (baptismPlace != null)
            {
                if (!string.IsNullOrEmpty(baptismPlace.Date))
                {
                    int bapInt = MatchTreeHelpers.ExtractInt(baptismPlace.Date);
                    string bapStr = baptismPlace.Date;

                    if (returnObj.YearInt == 0)
                    {
                        returnObj.YearInt = bapInt;
                        returnObj.DateStr = bapStr;
                    }
                }

                if (!string.IsNullOrEmpty(baptismPlace.Place))
                {
                    if (returnObj.Place == "")
                    {
                        returnObj.Place = baptismPlace.Place;
                    }
                }
            }

            return returnObj;
        }
        
        public static void Run(string filePath)
        {
            //fills new treepersons tables which is clone of persons table
            //reason for this is that deleting the existing tree persons from the old persons table
            //couldn't be made to work in a reasonable time frame!

            var fileParser = new FileParser();

            fileParser.Parse(filePath);

            var relationCount =
                fileParser.PersonContainer.ChildRelations.Count +
                fileParser.PersonContainer.SpouseRelations.Count +
                fileParser.PersonContainer.SiblingRelations.Count;


            var persons = new List<Persons>();


            var context = new DNAGEDContext();


            context.DeleteTreePersons();

            int countPeople =0;

            foreach (var p in fileParser.PersonContainer.Persons)
            {
                // fileParser.PersonContainer.ChildRelations[0].From.Id
                var parents = fileParser.PersonContainer.ChildRelations.Where(w => w.From.Id == p.Id).ToList();
                string fatherId = "";
                string motherId = "";
                Console.WriteLine(p.FirstName + " " + p.LastName);

                if(parents.Count() == 2)
                {
                    fatherId = parents[0].To.Id;
                    motherId = parents[1].To.Id; 
                }

                if (parents.Count() == 1)
                {                     
                    motherId = parents[0].To.Id;
                }

            
 
                var birth = GetDateObj(p.Birth, p.Baptized);
                var death = GetDateObj(p.Death, null);

                persons.Add(new Persons()
                {
                    Id = countPeople,
                    ChristianName = p.FirstName,
                    Surname = p.LastName,
                    FatherId = 0,
                    MotherId = 0,
                    BirthDate = birth.DateStr,
                    BirthYear = birth.YearInt,
                    BirthPlace = birth.Place,                    
                    BirthCountry = fatherId,
                    BirthCounty = "",

                    DeathYear = death.YearInt,
                    DeathDate = death.DateStr,
                    DeathPlace = death.Place,
                    DeathCountry = motherId,
                    DeathCounty = "",
                    Memory = p.Id,
                    CreatedDate = DateTime.Now,
                    RootsEntry = true,
                    Fix = false,
                    EnglishParentsChecked = false,
                    CountryUpdated =false,
                    AmericanParentsChecked = false,
                    CountyUpdated =false                                                           
                });

                countPeople++;
            }

            context.BulkInsertTreePersons(persons);
        }
           


    }
}
