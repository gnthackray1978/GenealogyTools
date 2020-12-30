using FTMContext;
using FTMContext.Models;
using MyFamily.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace FTMContext
{

    public class FTMTools {


        public static string GetDateString(string date)
        {

            //var o = a.ToCardinal();


            uint.TryParse(date, out uint parsed);

            var tp = Date.CreateInstance(parsed);

            string result = tp.Modifier.ToString();

            if (tp.HasDay())
                result += " " + tp.Day.ToString();

            if (tp.HasMonth())
                result += " " + tp.Month.ToString();

            if (tp.HasYear())
                result += " " + tp.Year.ToString();

            return result;

        }


        public static int ExtractYear(object originalText)
        {

            if (originalText == null) return 0;

            var parsedText = originalText.ToString();

            Regex regex = new Regex(@"\d\d\d\d");
            var v = regex.Match(parsedText);
            string anyoString = v.Groups[0].ToString();

            if (anyoString != String.Empty)
                return Convert.ToInt32(anyoString);
            else
                return 0;

        }

        public static PersonDataObj GetFact(List<Fact> facts,  int personId)
        {
            var returnObj = new PersonDataObj();

            var origin = facts.FirstOrDefault(w => w.FactTypeId == 14 && w.LinkId == personId);

            returnObj.Origin = origin?.Text ?? "";

            var existingFacts = facts.Where(w => w.FactTypeId == 90 && w.LinkId == personId);
            // write regex to get year

            if (existingFacts.Count() > 0)
            {
                var ef = existingFacts.First();

                var parts = Regex.Split(ef.Text, @"\|\|");

                if (parts.Length == 2) {
                    var datePart = parts[0].Trim();

                    var r1 = new Regex(@"\d\d\d\d");
                    var r1Matches = r1.Matches(parts[0]);

                    
                    if (r1Matches.Count > 1) {
                        //r1Matches[0]
                        int.TryParse(r1Matches[0].ToString(), out int year1);

                        int.TryParse(r1Matches[1].ToString(), out int year2);

                        returnObj.BirthYearFrom = year1;
                        returnObj.BirthYearTo = year2;

                    }

                    var countyListPart = parts[1].Trim();

                    if (countyListPart.Contains(","))
                        returnObj.Counties.AddRange(countyListPart.Split(','));
                    else
                        returnObj.Counties.Add(countyListPart);

                    return returnObj;
                }
                
            }

            return null;
        }


        public static void SaveFact(FTMakerContext f, int factTypeId,
            string originString, int personId)
        {
            var existingFacts = f.Fact.Where(w => w.FactTypeId == factTypeId && w.LinkId == personId);
            // write regex to get year

            if (existingFacts.Count() > 0)
            {
                
                var ef = existingFacts.First();
                //if it's actually changed then update it
                if (ef.Text != originString)
                {
                    ef.LinkId = personId;
                    ef.LinkTableId = 5;
                    ef.Text = originString;
                    ef.FactTypeId = factTypeId;//origin
                    ef.CreateDate = DateTime.Today;
                    ef.UpdateDate = DateTime.Today;
                }
            }
            else
            {
                var fact = new Fact()
                {
                    LinkId = personId,
                    LinkTableId = 5,
                    FactTypeId = factTypeId,//origin
                    Private = false,
                    Preferred = true,
                    Date = null,
                    PlaceId = null,
                    Text = originString,
                    MediaLinkedCounts = 0,
                    SourceLinkedCounts = 0,
                    CreateDate = DateTime.Today,
                    UpdateDate = DateTime.Today
                };

                f.Fact.Add(fact);
            }


           
        }

        public static void TestConnections() {
            var f = new FTMakerContext(new ConfigObj
            {
                // Path = @"C:\Users\george\Documents\Software MacKiev\Family Tree Maker\",
                Path = @"C:\Users\george\Documents\Family Tree Maker\",
                FileName = @"Allen Herbert - possible ancestors.ftm",
                IsEncrypted = true
            });

            var a = new FTMakerContext(new ConfigObj
            {
                Path = @"C:\Users\george\Documents\Repos\FTMCRUD\ftmframework\",
                FileName = @"decrrypted.db",
                IsEncrypted = false
            });

            var p = f.Person.FirstOrDefault(n => n.FamilyName.Contains("Allen"));

            Console.WriteLine("First entry FTDNA Person table: " + p.FullName);

            var p2 = a.Person.FirstOrDefault(n => n.FamilyName.Contains("Allen"));

            Console.WriteLine("First entry Decrypt Person table: " + p2.FullName);

            Console.WriteLine(a.FTMPlaceCache.Count());
        }

        public static void ExtractFTMDB()
        {

            Console.WriteLine("copying db");

            var f = new FTMakerContext(new ConfigObj
            {
                Path = @"C:\Users\george\Documents\Software MacKiev\Family Tree Maker\",
                FileName = @"DNA Match File.ftm",
                IsEncrypted = true
            });


            var a = new FTMakerContext(new ConfigObj
            {
                Path = @"C:\Users\george\Documents\Repos\FTMCRUD\ftmframework\",
                FileName = @"decrrypted.db",
                IsEncrypted = false
            });

            Console.WriteLine("Deleting Existing Data");

            a.DeleteAll();

            Console.WriteLine("Importing New Data");

            a.ChildRelationship.AddRange(f.ChildRelationship);
            a.Deleted.AddRange(f.Deleted);
            a.FactType.AddRange(f.FactType);
            a.HistoryList.AddRange(f.HistoryList);
            a.MasterSource.AddRange(f.MasterSource);
            a.MediaLink.AddRange(f.MediaLink);
            a.Note.AddRange(f.Note);
            a.PersonExternal.AddRange(f.PersonExternal);
            a.PersonGroup.AddRange(f.PersonGroup);
            a.Publication.AddRange(f.Publication);
            a.Repository.AddRange(f.Repository);
            a.Setting.AddRange(f.Setting);
            a.Source.AddRange(f.Source);
            a.SourceLink.AddRange(f.SourceLink);
            a.Tag.AddRange(f.Tag);
            a.TagLink.AddRange(f.TagLink);
            a.Task.AddRange(f.Task);
            a.Person.AddRange(f.Person);
            a.Fact.AddRange(f.Fact);
            a.Place.AddRange(f.Place);
            a.Relationship.AddRange(f.Relationship);
            a.MediaFile.AddRange(f.MediaFile);
            a.WebLink.AddRange(f.WebLink);

            Console.WriteLine("Saving New Data");
            a.SaveChanges();
        }
    }
}
