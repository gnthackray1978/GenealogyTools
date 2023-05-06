﻿using System.Diagnostics;
using QuickGed.Types;

namespace QuickGed.Services;

public class GedParser
{

    public static GedDb Parse(string gedPath)
    {
        var db = GedDb.Create();

        var timer = new Stopwatch();
        timer.Start();

        var gedcomLines = File.ReadAllLines(gedPath).Select(GedcomLine.Parse);

       

        var idLookupDictionary = new Dictionary<string, int>();


        var childList = new List<Node>();
        int personiId = 1;
        PersonSubset currentPerson = null;
        var currentLevelOneType = "";
        var currentDate = "";
        var currentPlace = "";
        int currentHusband = 0;
        int currentWife = 0;
        bool inFamily = false;
        int relationshipId = 1;
        int childRelationId = 1;

        foreach (var line in gedcomLines)
        {
            //line.Level    
            if (line.Level == 1) currentLevelOneType = line.Type;

            if (line.Level == 0 && line.Type == "FAM")
                inFamily = true;

            if (line.Level == 0 && line.Type != "FAM")
                inFamily = false;
            
            if (line.Type == "INDI")
            {
                personiId++;
                
                if (currentPerson != null)
                {
                    db.Persons.Add(currentPerson);
                    db.PersonDictionary.Add(currentPerson.Id, currentPerson);
                }
                currentPlace = "";
                currentDate = "";
                currentPerson = new PersonSubset
                {
                    Id = personiId,
                    StrId = line.Id
                };

                idLookupDictionary.Add(line.Id, personiId);
            }
            
            if (line.Type == "FAM" && currentPerson != null)// we have moved on to the families tidy up the last entry in the persons list
            {
                db.Persons.Add(currentPerson);
                db.PersonDictionary.Add(currentPerson.Id, currentPerson);
                currentPerson = null; //todo tidy this logic up
                currentPlace = "";
                currentDate = "";
            }

            if(currentPerson!=null)
                ProcessIndividuals(line, ref currentPerson, currentLevelOneType);


            if (inFamily)
            {

                switch (line.Type)
                {
                    case "DATE":
                        if (line.Level == 2)
                        {
                            currentDate = line.Data;
                        }

                        break;

                    case "PLAC":
                        if (line.Level == 2)
                        {
                            currentPlace = line.Data;
                        }

                        break;

                    case "FAM":
                        if (currentHusband != 0 || currentWife != 0)
                        {
                            var marriageYear = MatchTreeHelpers.ExtractYear(currentDate);

                            db.Relationships.Add(RelationSubSet.Create(relationshipId, currentDate
                                , currentPlace, currentHusband, currentWife, marriageYear));

                            PersonSubset husband = null;
                            PersonSubset wife = null;

                            if (db.PersonDictionary.ContainsKey(currentHusband))
                            {
                                husband = db.PersonDictionary[currentHusband];

                                if (husband.BirthYearFrom == 0 && marriageYear > 0)
                                {
                                    husband.BirthYearFrom = marriageYear - 30;
                                    husband.BirthYearTo = marriageYear - 18;
                                }

                            }

                            if (db.PersonDictionary.ContainsKey(currentWife))
                            {
                                wife = db.PersonDictionary[currentWife];

                                if (wife.BirthYearFrom == 0 && marriageYear > 0)
                                {
                                    wife.BirthYearFrom = marriageYear - 30;
                                    wife.BirthYearTo = marriageYear - 18;
                                }

                            }

                            if (husband != null && childList.Count > 0)
                            {
                                husband.IsParent = true;
                                husband.Children = childList;
                            }

                            if (wife != null && childList.Count > 0)
                            {
                                wife.IsParent = true;
                                wife.Children = childList;
                            }

                            if (husband != null && wife != null)
                            {
                                husband.Spouses.Add(wife);
                                wife.Spouses.Add(husband);
                            }


                            foreach (var i in childList)
                            {
                                db.ChildRelationships.Add(new ChildRelationship()
                                {
                                    Id = childRelationId,
                                    PersonId = i.Id,
                                    RelationshipId = relationshipId
                                });

                                i.Siblings = childList.Where(w => w.Id != i.Id).ToList();

                                childRelationId++;
                            }

                            childList = new List<Node>();

                            relationshipId++;
                        }
                        //reset variables after each family.
                        currentPlace = "";
                        currentDate = "";
                        currentHusband = 0;
                        currentWife = 0;
                        break;
                    case "HUSB":
                        if (line.Reference != null)
                            currentHusband = idLookupDictionary[line.Reference];
                        break;
                    case "WIFE":
                        if (line.Reference != null)
                            currentWife = idLookupDictionary[line.Reference];
                        break;
                    case "CHIL":
                        //currentHusband = 0;
                        //currentWife = 0;
                        if (line.Reference != null)
                        {
                            var child = idLookupDictionary[line.Reference];

                            db.PersonDictionary[child].FatherId = currentHusband;
                            db.PersonDictionary[child].MotherId = currentWife;



                            childList.Add(db.PersonDictionary[child]);

                            if (!db.ParentDictionary.ContainsKey(child))
                            {
                                var parentList = new List<Node>();

                                if (currentHusband != 0)
                                    parentList.Add(db.PersonDictionary[currentHusband]);

                                if (currentWife != 0)
                                    parentList.Add(db.PersonDictionary[currentWife]);

                                db.ParentDictionary.Add(child, parentList);
                            }
                        }

                        break;
                }

            }

        }


        foreach (var p in db.Persons.Where(w => (w.BirthYearFrom == 0) || !w.IsValidLocation()))
        {
            if (p.BirthYearFrom == 0)
            {
                var r = AgeRangeCalculator.GetPersonBirthDateRange(p);

                p.BirthYearFrom = r.YearFrom;
                p.BirthYearTo = r.YearTo;
            }

            if (!p.IsValidLocation())
            {
                if (FindLocation(p, out string location))
                {
                    p.Residence = location;
                    p.ResidenceDescription = "ChildBirthLocation";
                }
            }

        }



        timer.Stop();

        TimeSpan timeTaken = timer.Elapsed;
        string foo = "Time taken: " + timeTaken.ToString(@"m\:ss\.fff");

        Console.WriteLine(foo);

        return db;
    }

    private static bool FindLocation(PersonSubset person, out string location)
    {
        location = "";

        //there is already a valid location in there for one of the below
        //fields so we can use that.
        if (!string.IsNullOrEmpty(person.BirthLocation) ||
            !string.IsNullOrEmpty(person.DeathLocation) ||
            !string.IsNullOrEmpty(person.Residence)) return false;

        foreach (var c in person.Children.Cast<PersonSubset?>().Where(c => !string.IsNullOrEmpty(c.BirthLocation)))
        {
            location = c.BirthLocation;
            return true;
        }

        return false;
        
    }

    private static void ProcessIndividuals(GedcomLine line, ref PersonSubset currentPerson, string currentLevelOneType)
    {
        switch (currentLevelOneType)
        {
            case "BAPM":
                PopulateBirth(line, currentPerson, true);

                break;
            case "BIRT":
                PopulateBirth(line, currentPerson, false);
                break;
            case "BURI":
                PopulateDeath(line, currentPerson, false);
                break;
            case "DEAT":
                PopulateDeath(line, currentPerson, true);
                break;


            case "FACT":
                break;


            case "NAME":
                //currentPerson. = line.Data;
                if (line.Type == "GIVN") currentPerson.Forename = line.Data;
                if (line.Type == "SURN") currentPerson.FamilyName = line.Data;

                break;


            case "NOTE":
                break;

            case "OCCU":
                currentPerson.Occupation = line.Data;
                break;

            case "RESI":
                PopulateResidence(line, currentPerson);
                break;

            case "SEX":
                currentPerson.Gender = line.Data;
                break;

            case "TITL":
                currentPerson.Title = line.Data;
                break;


        }

    }

    private static void PopulateBirth(GedcomLine line, PersonSubset datePlace, bool isBaptism)
    {
        if (line.Type == "DATE")
        {
            var isValidDate = MatchTreeHelpers.ExtractYear(line.Data) > 0;
            if (!isValidDate) return;

            if (!isBaptism && MatchTreeHelpers.ExtractYear(line.Data) > 0)
            {
                datePlace.BirthDate = line.Data;
                datePlace.BirthYearFrom = MatchTreeHelpers.ExtractYear(line.Data);
                datePlace.BirthYearTo = datePlace.BirthYearFrom;
                return;
            }
            //set the birth date to the baptism
            datePlace.BirthDate = line.Data;
        }

        if (line.Type == "PLAC")
        {
            var isValidString = !string.IsNullOrEmpty(line.Data);
            if (!isValidString) return;

            if (!isBaptism && !string.IsNullOrEmpty(line.Data))
            {
                datePlace.BirthLocation = line.Data;
                return;
            }
            //set the birth date to the baptism
            datePlace.BirthLocation = line.Data;
        }
        if (line.Type == "NOTE") datePlace.BirthNote = line.Data;

    }

    private static void PopulateResidence(GedcomLine line, PersonSubset datePlace)
    {
        if (line.Type == "DATE")
        {

            datePlace.ResidenceDate = line.Data;
            datePlace.ResidenceYear = MatchTreeHelpers.ExtractYear(line.Data);
        }

        if (line.Type == "PLAC")
        {

            datePlace.Residence = line.Data;
        }

    }

    private static void PopulateDeath(GedcomLine line, PersonSubset datePlace, bool isDeath)
    {
        if (line.Type == "DATE")
        {
            var isValidDate = MatchTreeHelpers.ExtractYear(line.Data) > 0;
            if (!isValidDate) return;

            if (!isDeath && !(MatchTreeHelpers.ExtractYear(datePlace.DeathDate) > 0))
            {
                datePlace.DeathDate = line.Data;
                datePlace.DeathYear = MatchTreeHelpers.ExtractYear(line.Data);
                return;
            }
            //set the birth date to the death
            datePlace.DeathDate = line.Data;
        }

        if (line.Type == "PLAC")
        {
            var isValidString = !string.IsNullOrEmpty(line.Data);
            if (!isValidString) return;

            if (!isDeath && !string.IsNullOrEmpty(line.Data))
            {
                datePlace.DeathLocation = line.Data;
                return;
            }
            //set the death location to the death
            datePlace.DeathLocation = line.Data;
        }
        if (line.Type == "NOTE") datePlace.DeathNote = line.Data;

    }
}