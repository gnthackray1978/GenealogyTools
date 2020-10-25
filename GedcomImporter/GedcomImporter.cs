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

namespace GedcomImporter
{
    public class GedcomImporter
    {
        public static void DeleteFamilyTreePersons() {
            using (var context = new DNAGEDContext())
            {

                var oldPersons = context.Persons.Where(pid => pid.RootsEntry);

                Console.WriteLine("Removing  " + oldPersons.Count() + " old records");

                context.RemoveRange(oldPersons);

                context.SaveChanges();

            }
        }

        public static DateObj GetDateObj(DatePlace birthPlace, DatePlace baptismPlace)
        {
            var returnObj = new DateObj() { 
                Place="unset",
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

            //DeleteFamilyTreePersons();

             var fileParser = new FileParser();

            fileParser.Parse(filePath);

            var relationCount =
                fileParser.PersonContainer.ChildRelations.Count +
                fileParser.PersonContainer.SpouseRelations.Count +
                fileParser.PersonContainer.SiblingRelations.Count;

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
                 
                using (var context = new DNAGEDContext())
                {
                    var birth = GetDateObj(p.Birth, p.Baptized);
                    var death = GetDateObj(p.Death, null);

                    context.Persons.Add(new Persons()
                    {
                        AmericanParentsChecked = false,
                        ChristianName = p.FirstName,
                        Surname = p.LastName,
                        CreatedDate = DateTime.Now,
                        RootsEntry = true,
                        BirthYear = birth.YearInt,
                        BirthPlace = birth.Place,
                        BirthDate = birth.DateStr,
                        DeathYear = death.YearInt,
                        DeathDate = death.DateStr,
                        DeathPlace = death.Place,
                        CountryUpdated =false,
                        CountyUpdated =false,
                        Memory = p.Id ,
                        BirthCountry = fatherId,
                        DeathCountry = motherId
                    });

                }
            }

           
        }
           


    }
}
