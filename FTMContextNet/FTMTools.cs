﻿using FTMContext;
using FTMContext.Models;
using Microsoft.EntityFrameworkCore;
using MyFamily.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using ConfigHelper;

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
                        returnObj.AssociatedLocations.AddRange(countyListPart.Split(','));
                    else
                        returnObj.AssociatedLocations.Add(countyListPart);

                    return returnObj;
                }
                
            }

            return null;
        }

  

        public static void SaveState(FTMakerContext f)
        {
            var ef = f.SyncState.FirstOrDefault();
            // write regex to get year

            if (ef!=null)
            {
                ef.TreeModified = DateTime.Now;                                
            }

            var p = f.Person.FirstOrDefault(pa=>pa.Id ==1);
            // write regex to get year

            if (p != null)
            {
                p.UpdateDate = DateTime.Now;
            }
        }

        public static void SaveFactWithSync(FTMakerContext f, int factTypeId,
           string originString, int personId)
        {
            //NOTE
            //to make sync work with facts
            //I've found that you need to add a completely new fact
            //so delete all old facts of the types you're interested in
            //then just re-add. this is of course slow.

            var existingFacts = f.Fact.Where(w => w.FactTypeId == factTypeId && w.LinkId == personId);
            // write regex to get year

            foreach (var exf in existingFacts)
            {
                f.Fact.Remove(exf);
            }

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
                    ef.CreateDate = DateTime.Now.Subtract(new TimeSpan(2, 2, 2, 2));
                    ef.UpdateDate = DateTime.Now.Subtract(new TimeSpan(1, 1, 1, 1));
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

            IMSGConfigHelper imsgConfigHelper = new MSGConfigHelper();

            var sourceContext = FTMakerContext.CreateSourceDB(imsgConfigHelper);

            var destinationContext = FTMakerContext.CreateDestinationDB(imsgConfigHelper);

            var p = sourceContext.Person.FirstOrDefault(n => n.FamilyName.Contains("Allen"));

            Console.WriteLine("First entry FTDNA Person table: " + p.FullName);

            var p2 = destinationContext.Person.FirstOrDefault(n => n.FamilyName.Contains("Allen"));

            Console.WriteLine("First entry Decrypt Person table: " + p2.FullName);

            //Console.WriteLine(destinationContext.FTMPlaceCache.Count());
        }

        public static void TestDestinationConnections()
        {
            IMSGConfigHelper imsgConfigHelper = new MSGConfigHelper();

            var destinationContext = FTMakerContext.CreateDestinationDB(imsgConfigHelper);
                  
            var p2 = destinationContext.Person.FirstOrDefault(n => n.FamilyName.Contains("Allen"));

            Console.WriteLine("First entry Decrypt Person table: " + p2.FullName);

            //Console.WriteLine(destinationContext.FTMPlaceCache.Count());
        }
        
    }
}
