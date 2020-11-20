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
using ConsoleTools;

namespace GedcomImporter
{
    public class GedcomImporter
    {     
        private static int recursionCounter =0;

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
            var parts = filePath.Split('/');
            Console.WriteLine("");

            if (parts.Length > 0) {
                Console.WriteLine("Importing GED file :" + parts.Last());
            }
            else
            {
                Console.WriteLine("GED file path looks wrong:" + filePath);
                return;
            }
            var fileParser = new FileParser();

            fileParser.Parse(filePath);

            var relationCount =
                fileParser.PersonContainer.ChildRelations.Count +
                fileParser.PersonContainer.SpouseRelations.Count +
                fileParser.PersonContainer.SiblingRelations.Count;


            var persons = new List<Persons>();


            var context = new DNAGEDContext();
            
            Console.WriteLine("Deleting existing tree");
            context.DeleteTreePersons();

            int countPeople =0;
            int total = fileParser.PersonContainer.Persons.Count;
            Console.WriteLine("Importing " + total + " persons");
            


            foreach (var p in fileParser.PersonContainer.Persons)
            {
                // fileParser.PersonContainer.ChildRelations[0].From.Id
                var parents = fileParser.PersonContainer.ChildRelations.Where(w => w.From.Id == p.Id).ToList();
                string fatherId = "";
                string motherId = "";
            //   

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
                    IDString = p.Id,

                    ChristianName = p.FirstName,
                    Surname = p.LastName,
                    FatherId = 0,
                    FatherString = fatherId,
                    MotherId = 0,
                    MotherString = motherId,
                    BirthDate = birth.DateStr,
                    BirthYear = birth.YearInt,
                    BirthPlace = birth.Place,                    
                    BirthCountry = "Unknown",
                    BirthCounty = "Unknown",

                    DeathYear = death.YearInt,
                    DeathDate = death.DateStr,
                    DeathPlace = death.Place,
                    DeathCountry = "Unknown",
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

                if(countPeople%50 ==0)
                    ConsoleWrapper.ProgressUpdate(countPeople, total, "");

                countPeople++;
            }

            Console.WriteLine("setting home persons");
            var subset = persons.Where(w => !string.IsNullOrEmpty(w.ChristianName) && w.ChristianName.Contains("_"));

            recursionCounter = 0;

            foreach (var homePerson in subset)
            {
                //input.Any(c => char.IsDigit(c))
                if(homePerson.ChristianName.Any(c=>char.IsDigit(c)))
                    update(homePerson.ChristianName, homePerson, persons);
            }

            Console.WriteLine("attempting to save");
            context.BulkInsertTreePersons(persons);
        }
           
        private static void update(string homePerson, Persons child, 
            List<Persons> personList)
        {
            var father = personList.FirstOrDefault(f => f.IDString == child.FatherString);
            
            var mother = personList.FirstOrDefault(f => f.IDString == child.MotherString);

            recursionCounter++;

            if (recursionCounter % 100 == 0)
                ConsoleWrapper.ProgressUpdate(recursionCounter, personList.Count, "");

            if (father != null)
            {
                child.FatherId = father.Id;
                father.Memory = homePerson;
                update(homePerson, father, personList);                  
            }

            if (mother != null)
            {
                child.MotherId = mother.Id;
                mother.Memory = homePerson;
                update(homePerson, mother, personList);
            }

        }

    }
}
