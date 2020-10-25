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

                Console.WriteLine(p.FirstName + " " + p.LastName);
                if(parents.Count() > 1)
                {
                    Console.WriteLine(" " + parents[0].To.FirstName + " " + parents[0].To.LastName);
                    Console.WriteLine(" " + parents[1].To.FirstName + " " + parents[1].To.LastName);
                }
                //p.FirstName + p.LastName + p.Baptized + p.Birth.Place;
            }

            using (var context = new DNAGEDContext())
            {
                context.Persons.Add(new Persons()
                {
                    
                });
               
            }
        }
           


    }
}
