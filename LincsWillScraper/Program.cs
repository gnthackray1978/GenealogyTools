using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using LincsWillScraper.Models;
//
namespace LincsWillScraper
{
    public class LinkEntry
    {
        public string Date {get;set;}
        public string Url { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Collection { get; set; }
        public string Reference { get; set; }
        public bool PlaceKnown { get; set; }
        public string Place { get; set; }
        public int WillType { get; set; }
        public int Year { get; set; }
        public string Aliases { get; set; }
        public string Ocupation { get; set; }
    }



    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");



            //var wc = new WillsContext();

            //int idx = wc.LincsWills.Count()+1;

            //foreach (var e in linkEntries)
            //{
            //    wc.LincsWills.Add(new LincsWills()
            //    {
            //        Url = "",
            //        Description = e.Name,
            //        FirstName = e.FirstName,
            //        Surname = e.Surname,
            //        Collection = "LCC Wills 1700-1800",
            //        Reference = e.Reference,
            //        Year = e.Year,
            //        DateString = e.Date,
            //        Id = idx,
            //        Place = e.Place,
            //        Occupation = e.Ocupation,
            //        Aliases =  e.Aliases,
            //        Typ = 1

            //    });
            //    idx++;
            //}

            //wc.SaveChanges();

            ParseNorfolkHtmls();

            Console.WriteLine("finished");

            Console.ReadKey();
        }

        private static void ParseNorfolkHtmls()
        {
            List<string> aliases = new List<string>();

            List<string> newFile = new List<string>();

            var fpath = @"C:\Users\george\Downloads\Genealogy\data\nro";

            var wc = new WillsContext();
            var folders = Directory.EnumerateDirectories(fpath);

            var counter = folders.Count();

            foreach (var folder in folders)
            {
                var files = Directory.GetFiles(folder);
                foreach (var file in files)
                {
                    Console.WriteLine(file);

                    string contents = File.ReadAllText(file);

                    ParseNorfolkHtml(contents,wc, wc.NorfolkWillsRaw.Count());

                    wc.SaveChanges();
                }

                counter--;

                Console.WriteLine(counter);
            }
        }

        public static void ParseNorfolkHtml(string contents, WillsContext wc, int recordCount)
        {
            string pattern = @"<tr>.*?</tr>";


            var results = Regex.Matches(contents, pattern, RegexOptions.Singleline);

            

            foreach (var row in results)
            {
           //     Console.WriteLine(row.ToString());

                string patOverviewKey = @"<td class=""OverviewKey"">.*?</td>";

                var overviewKey = Regex.Match(row.ToString(), patOverviewKey, RegexOptions.Singleline);

                string patOverviewCellTitle = @"<td class=""OverviewCell OverviewCellTitle"">.*?</td>";

                var overOverviewCellTitle = Regex.Match(row.ToString(), patOverviewCellTitle, RegexOptions.Singleline);

                string patOverviewCellDate = @"<td class=""OverviewCell OverviewCellDate"">.*?</td>";

                var overOverviewCellDate = Regex.Match(row.ToString(), patOverviewCellDate, RegexOptions.Singleline);

                string patOverviewCellAltRefNo = @"<td class=""OverviewCell OverviewCellAltRefNo"">.*?</td>";

                var overOverviewCellAltRefNo = Regex.Match(row.ToString(), patOverviewCellAltRefNo, RegexOptions.Singleline);


               
                string link = "";
                string _ref ="";
                string date = "";
                string altRef = "";

                if (overviewKey.Success)
                {
                    link = overviewKey.Value;
                    link = link.Replace(@"<td class=""OverviewKey"">", "");
                    link = link.Replace(@"</td>", "");
                }
                if (overOverviewCellTitle.Success)
                {
                    _ref = overOverviewCellTitle.Value;
                    _ref = _ref.Replace(@"<td class=""OverviewCell OverviewCellTitle"">", "");
                    _ref = _ref.Replace(@"</td>", "");

                }
                if (overOverviewCellDate.Success)
                {
                    date = overOverviewCellDate.Value;
                    date = date.Replace(@"<td class=""OverviewCell OverviewCellDate"">", "");
                    date = date.Replace(@"</td>", "");
                }
                if (overOverviewCellAltRefNo.Success)
                {
                    altRef = overOverviewCellAltRefNo.Value;
                    altRef = altRef.Replace(@"<td class=""OverviewCell OverviewCellAltRefNo"">", "");
                    altRef = altRef.Replace(@"</td>", "");
                }


                if (link != "" && _ref != "" && date != "" && altRef != "")
                {
                    //   Console.WriteLine(_ref + " " + date + " " + altRef);

                    recordCount++;

                    wc.NorfolkWillsRaw.Add(new NorfolkWillsRaw()
                    {
                        CatalogueRef = altRef,
                        DateRange = date,
                        Link = link,
                        Title = _ref,
                        Id = recordCount
                    });


                }

            }




        }

        public static string FirstCharToUpper(string s)
        {
            // Check for empty string.  
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.  
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        private static void ParseWillPDFs()
        {
            List<string> aliases = new List<string>();

            List<string> newFile = new List<string>();

            var fpath = @"E:\data\documents\code\Visual Studio 2013\Projects\CSVAnalyser\LincsWillScraper\willindexs";

            var files = Directory.GetFiles(fpath);

            foreach (var f in files)
            {
                var str = File.ReadAllLines(f);

                foreach (var lin in str)
                {
                    string pattern = @"\d\d\d\d";

                    Match m = Regex.Match(lin, pattern, RegexOptions.Singleline);

                    if (!m.Success)
                    {
                        if (lin.Trim() == "") continue;

                        if (lin.Contains("contd")) continue;

                        if (lin.Contains("L.C.C.")) continue;

                        if (lin.Contains(":"))
                        {
                            newFile.Add(lin);
                            continue;
                        }

                        if (lin.Contains(" see ") || lin.Contains("_see ") || lin.Contains(" seleW"))
                        {
                            aliases.Add(lin);
                            continue;
                        }

                        if (lin.Length > 2 && lin[0] == ' ' && lin[1] == ' ' && lin[2] == ' ')
                        {
                            newFile.Add(lin);
                            continue;
                        }


                        //  lin.Remove(0, lin.Length - 1);

                        var newRow = lin.Substring(0, lin.Length - 1) + ":";

                        newFile.Add(newRow);

                        // Debug.WriteLine("fixed " + lin + " " + newRow);


                        //  Debug.WriteLine(lin);
                    }
                    else
                    {
                        if (!lin.Contains("(contd)"))
                            newFile.Add(lin);
                    }
                }

                File.WriteAllLines(
                    @"E:\data\documents\code\Visual Studio 2013\Projects\CSVAnalyser\LincsWillScraper\willindexs\output\completeindex.txt",
                    newFile);
                File.WriteAllLines(
                    @"E:\data\documents\code\Visual Studio 2013\Projects\CSVAnalyser\LincsWillScraper\willindexs\output\aliases.txt",
                    aliases);
                //Match m = Regex.Match(str, pattern, RegexOptions.Singleline);

                //if (m.Success)
                //{

                //}
            }
        }

        private static void DownloadPlaces()
        {
            var wc = new WillsContext();

            var yearGroups = wc.LincsWills.Where(y => y.Place == "" && y.Typ == 1).GroupBy(g => g.Year);

            foreach (var g in yearGroups)
            {
                Console.WriteLine(g.Key + " " + g.Count());
            }


            //var wills = wc.LincsWills.Where(y => y.Typ == 1).ToList();

            //foreach (var will in wills)
            //{
            //    var m = Regex.Match(will.Description ?? "", @"\d\d\d\d", RegexOptions.None);

            //    if (m.Success)
            //    {
            //        int localyear = Convert.ToInt32(m.Value);

            //        if (localyear != will.Year)
            //        {
            //            will.Year = Convert.ToInt32(m.Value);

            //            wc.SaveChanges();
            //        }
            //    }


            //}


            int yearint = 0;

            bool validYear = false;

            while (!validYear)
            {
                var year = Console.ReadLine();
                validYear = Int32.TryParse(year.ToString(), out yearint);
            }

            Console.WriteLine("year: " + yearint);

            Console.WriteLine("press a key to continue");
            Console.ReadKey();


            var data = wc.LincsWills.Where(y => y.Year <= yearint && y.Place == "" && y.Typ == 1).ToList();

            int opCount = 0;
            foreach (var d in data)
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        using (HttpResponseMessage response = client.GetAsync(d.Url).Result)
                        {
                            using (HttpContent content = response.Content)
                            {
                                string result = content.ReadAsStringAsync().Result;

                                string lookup = "";


                                lookup = LookupPlace(result);

                                d.Place = lookup;

                                wc.SaveChanges();
                                Console.WriteLine("saved: " + opCount + " of " + data.Count);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                    Debug.WriteLine("!Exception: " + e.Message);
                }


                Thread.Sleep(RandomNumber(4000, 6000));

                //Task.Delay(RandomNumber(3000,5000));
                opCount++;
            }

            //var fpath = @"E:\data\documents\code\Visual Studio 2013\Projects\CSVAnalyser\LincsWillScraper\pages\sample.txt";

            //var sample = File.ReadAllText(fpath);
            //string lookup = "";


            //lookup = LookupPlace(sample);
        }

        public static int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        private static string LookupPlace(string sample)
        {
            string lookup="";
            string pattern = @"<div class=""recordDescription"">.*?</div>";


            Match m = Regex.Match(sample, pattern, RegexOptions.Singleline);

            if (m.Success)
            {
                pattern = @"(?<=(<strong>Place: </strong>)).*?(?=(</p>))";
                Match m2 = Regex.Match(m.Value, pattern, RegexOptions.Singleline);

                if (m2.Success)
                {
                    lookup = m2.Value;
                }
            }

            return lookup;
        }

        private static void CreateIndex()
        {
            var fpath = @"E:\data\documents\code\Visual Studio 2013\Projects\CSVAnalyser\LincsWillScraper\pages";

            string pattern = @"<div class=""content"">.*?</div>";

            List<LinkEntry> entrys = new List<LinkEntry>();


            var files = Directory.GetFiles(fpath);

            Console.WriteLine(files.Length + " files");

            foreach (var f in files)
            {
                var str = File.ReadAllText(f);

                Match m = Regex.Match(str, pattern, RegexOptions.Singleline);

                if (m.Success)
                {
                    //     Console.WriteLine("file has data");


                    while (m.Success)
                    {
                        LinkEntry newEntry = new LinkEntry() {WillType = 0};

                        //Console.WriteLine(match);

                        bool isValid = true;


                        XDocument htmlXml = XDocument.Parse(m.ToString());


                        foreach (var spanElement in htmlXml.Descendants())
                        {
                            if (spanElement.Name == "div" || spanElement.Name == "h3" || spanElement.Name == "p") continue;

                            string attr = "";

                            if (spanElement.LastAttribute != null)
                            {
                                attr = spanElement.LastAttribute.Value;
                            }

                            if (attr == "date")
                            {
                                if (spanElement.Value.Contains("-"))
                                {
                                    isValid = false;
                                }
                            }

                            //   Console.WriteLine(spanElement.Name + " " + spanElement.Value + " " + attr);

                            if (attr == "collection")
                            {
                                newEntry.Collection = spanElement.Value;
                            }

                            if (attr == "date")
                            {
                                newEntry.Date = spanElement.Value;
                            }

                            if (attr == "reference")
                            {
                                newEntry.Reference = spanElement.Value;
                            }

                            if (spanElement.Name == "a")
                            {
                                newEntry.Url = attr;
                                newEntry.Name = spanElement.Value;
                            }
                        }


                        //var standardMatch = Regex.Match(newEntry.Name, @"Will*.-.*,", RegexOptions.Singleline);

                        //if (standardMatch.Success)
                        //{
                        //    newEntry.PlaceKnown = false;
                        //}
                        //else
                        //{
                        //    newEntry.PlaceKnown = true;
                        //}

                        newEntry.Url = newEntry.Url.Replace("-", "");


                        var willtype = Regex.Match(newEntry.Url, @"\/Will[A-Z]", RegexOptions.None);

                        if (willtype.Success)
                        {
                            newEntry.WillType = 1;

                            var temp = newEntry.Name.Replace("Will - ", "");

                            temp = temp.Replace(@"(", "").Replace(@")", "");

                            var parts = temp.Split(",");

                            if (parts.Length > 1)
                            {
                                newEntry.FirstName = Regex.Replace(parts[1], @"\d", "");
                                newEntry.Surname = parts[0];
                            }
                        }
                        else
                        {
                            newEntry.WillType = 0;

                            if (newEntry.Name.Contains(","))
                            {
                                var temp = Regex.Match(newEntry.Name, @"\w+.,", RegexOptions.None);

                                if (temp.Success)
                                {
                                    newEntry.Surname = temp.Value.Replace(",", "");
                                }

                                temp = Regex.Match(newEntry.Name, @",.\w+", RegexOptions.None);

                                if (temp.Success)
                                {
                                    newEntry.FirstName = temp.Value.Replace(",", "");
                                }
                            }
                        }

                        #region add year

                        var date3 = Regex.Match(newEntry.Date ?? "", @"\d\d\d\d", RegexOptions.None);


                        var date1 = Regex.Match(newEntry.Reference ?? "", @"\d\d\d\d", RegexOptions.None);


                        if (date3.Success)
                            newEntry.Year = Convert.ToInt32(date3.Value);

                        var date2 = Regex.Match(newEntry.Url ?? "", @"[a-zA-Z]\d\d\d\d[a-zA-Z]", RegexOptions.None);
                        if (date2.Success)
                        {
                            var tp = Regex.Match(date2.Value, @"\d\d\d\d", RegexOptions.None);
                            newEntry.Year = Convert.ToInt32(tp.Value);
                        }

                        if (date1.Success)
                            newEntry.Year = Convert.ToInt32(date1.Value);

                        #endregion


                        newEntry.Collection = Regex.Replace(newEntry.Collection ?? "", "Collection:", "").Trim();
                        newEntry.Reference = Regex.Replace(newEntry.Reference ?? "", "Reference:", "").Trim();

                        if (newEntry.Collection == "Lincolnshire Archives")
                        {
                            entrys.Add(newEntry);
                        }


                        m = m.NextMatch();
                    }
                }
            }


            //foreach (var g in entrys)
            //{
            //    Debug.WriteLine(g.Url);
            //}

            Debug.WriteLine(entrys.Count + " " + entrys.Count(a => a.PlaceKnown) + " " + entrys.Count(c => c.WillType == 1));


            Console.WriteLine(entrys.Count + " entries");

            var wc = new WillsContext();

            int idx = 0;

            foreach (var e in entrys)
            {
                wc.LincsWills.Add(new LincsWills()
                {
                    Url = e.Url,
                    Description = e.Name,
                    FirstName = e.FirstName,
                    Surname = e.Surname,
                    Collection = e.Collection,
                    Reference = e.Reference,
                    Year = e.Year,
                    DateString = e.Date,
                    Id = idx,
                    Place = "",
                    Typ = e.WillType
                });
                idx++;
            }

            wc.SaveChanges();
        }
    }
}
