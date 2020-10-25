using GedcomParser.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using DNAGedLib;
using Microsoft.EntityFrameworkCore;
using GenDBContext.Models;

namespace GedcomImporter
{
    public class GedcomImporter
    {
        public void Run(string filePath)
        {

            using (var context = new DNAGEDContext())
            {

                var oldPersons = context.Persons.Where(pid => pid.RootsEntry);

                Console.WriteLine("Removing  " + oldPersons.Count() + " old records");

                context.RemoveRange(oldPersons);

                context.SaveChanges();

            }

            var fileParser = new FileParser();

            fileParser.Parse(filePath);

            var relationCount =
                fileParser.PersonContainer.ChildRelations.Count +
                fileParser.PersonContainer.SpouseRelations.Count +
                fileParser.PersonContainer.SiblingRelations.Count;

            foreach (var p in fileParser.PersonContainer.Persons)
            {
               // fileParser.PersonContainer.ChildRelations[0].From.Id

                //p.FirstName + p.LastName + p.Baptized + p.Birth.Place;
            }


            //dnagedContext.Persons.Add(new Persons
            //{
            }


    }
}
