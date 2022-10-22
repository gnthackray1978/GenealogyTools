//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using AzureContext.Models;
//using Microsoft.EntityFrameworkCore.Metadata.Conventions;

//namespace FTMConsole2
//{
//    public class GraphPerson
//    {   
//        public int ChildCount { get; set; }
//        public List<int> ChildIdxLst { get; set; }
//        public List<int> ChildLst { get; set; }
//        public List<object> Children { get; set; }
//        public int DescendantCount { get; set; }
//        public object Father { get; set; }
//        public int FatherId { get; set; }
//        public int FatherIdx { get; set; }
//        public int GenerationIdx { get; set; }
//        public int Index { get; set; }
//        public bool IsDisplayed { get; set; }
//        public bool IsFamilyEnd { get; set; }
//        public bool IsFamilyStart { get; set; }
//        public bool IsHtmlLink { get; set; }
//        public bool IsParentalLink { get; set; }
//        public int MotherId { get; set; }
//        public int MotherIdx { get; set; }
//        public int PersonId { get; set; }
//        public List<int> SpouseIdxLst { get; set; }
//        public List<int> SpouseIdLst { get; set; }
//        public string ChristianName { get; set; }
//        public string Surname { get; set; }
//        public string BirthLocation { get; set; }
//        public string DOB { get; set; }

//    }

//    public class GraphMarriage
//    {
//        public int FatherId { get; set; }
//        public int MotherId { get; set; }
//    }

//    public class AncestorGraphBuilder {
//        private readonly AzureDBContext _azureDbContext;
//        private List<FTMPersonView> _persons;
//        private List<GraphMarriage> _graphMarriages;

//        public AncestorGraphBuilder(AzureDBContext azureDbContext)
//        {
//            //_azureDbContext = azureDbContext;

//            //_persons = _azureDbContext.FTMPersonView.Where(w => w.Origin == "_34_Kennington").ToList();

//            //_graphMarriages = new List<GraphMarriage>();

//            //var g = _persons.GroupBy(d => new { d.FatherId, d.MotherId })
//            //    .Select(m => new { m.Key.FatherId, m.Key.MotherId });

//            //foreach (var group in g)
//            //{
//            //    _graphMarriages.Add(new GraphMarriage() { FatherId = group.FatherId.GetValueOrDefault(), MotherId = group.MotherId.GetValueOrDefault() });
//            //}


//        }

//        public void GenerateAncestorGraph(int personId) {
//            List<List<GraphPerson>> results = new List<List<GraphPerson>>();

//            Console.WriteLine("GenerateDescendantGraph");

//            var startPerson = _persons.FirstOrDefault(fd => fd.PersonId == personId);
//            int currentGeneration = 0;

//            results.Add(new List<GraphPerson>());
//            results.Last().Add(new GraphPerson()
//            {
//                PersonId = startPerson.PersonId.GetValueOrDefault(),
//                BirthLocation = startPerson.BirthLocation,
//                ChristianName = startPerson.FirstName,
//                Surname = startPerson.Surname,
//                DOB = startPerson.BirthFrom.ToString(),
//                FatherId = startPerson.FatherId.GetValueOrDefault(),
//                MotherId = startPerson.MotherId.GetValueOrDefault(),
//                ChildIdxLst = new List<int>(),
//                ChildLst = new List<int>(),
//                SpouseIdxLst = new List<int>(),
//                SpouseIdLst = new List<int>(),
//                GenerationIdx =0
//            }
//            );

//            fillParents(results.Last().Last(), ref results, ref currentGeneration);

//            //add to array

//            //look up this persons parents.

//            //

//            foreach(var p in results[5])
//            {
//                Console.WriteLine(p.PersonId + " " + p.ChristianName + " " + p.Surname);
//            }
//        }

//        private void fillParents(GraphPerson child,
//            ref List<List<GraphPerson>> results, ref int currentGeneration)
//        {
//            if (child == null) return;

//            if(child.PersonId == 3219)
//            {
//                Console.WriteLine("test");
//            }

//            int fatherId = child.FatherId;
//            int motherId = child.MotherId;


//            var father = _persons.FirstOrDefault(fd => fd.PersonId == fatherId);
//            var mother = _persons.FirstOrDefault(fd => fd.PersonId == motherId);

//            if (father == null && mother == null)
//                return; 
//        //    results.Add(new List<GraphPerson>());

//            currentGeneration = child.GenerationIdx + 1;

//            if (currentGeneration >= results.Count)
//            {
//                results.Add(new List<GraphPerson>());
//            }

//            GraphPerson newFather = null;
//            GraphPerson newMother = null;
            
//            if (father != null)
//            {
//                newFather = new GraphPerson()
//                {
//                    PersonId = father.PersonId.GetValueOrDefault(),
//                    BirthLocation = father.BirthLocation,
//                    ChristianName = father.FirstName,
//                    Surname = father.Surname,
//                    DOB = father.BirthFrom.ToString(),
//                    FatherId = father.FatherId.GetValueOrDefault(),
//                    MotherId = father.MotherId.GetValueOrDefault(),
//                    ChildIdxLst = new List<int>(),
//                    ChildLst = new List<int>() { child.PersonId },
//                    SpouseIdxLst = new List<int>(),
//                    SpouseIdLst = new List<int>(),
//                    GenerationIdx = currentGeneration,
//                    IsDisplayed = true,
//                    IsHtmlLink = true
//                };

//                results[currentGeneration].Add(newFather);
//            }

//            if (mother != null)
//            {
//                newMother = new GraphPerson()
//                {
//                    PersonId = mother.PersonId.GetValueOrDefault(),
//                    BirthLocation = mother.BirthLocation,
//                    ChristianName = mother.FirstName,
//                    Surname = mother.Surname,
//                    DOB = mother.BirthFrom.ToString(),
//                    FatherId = mother.FatherId.GetValueOrDefault(),
//                    MotherId = mother.MotherId.GetValueOrDefault(),
//                    ChildIdxLst = new List<int>(),
//                    ChildLst = new List<int>() { child.PersonId },
//                    SpouseIdxLst = new List<int>(),
//                    SpouseIdLst = new List<int>(),
//                    GenerationIdx = currentGeneration,
//                    IsDisplayed = true,                    
//                    IsHtmlLink = true
//                };

//                results[currentGeneration].Add(newMother);
//            }

//            fillParents(newFather, ref results, ref currentGeneration);
//            fillParents(newMother, ref results, ref currentGeneration);
//        }
//    }

//    public class GraphBuilder
//    {
//        private readonly AzureDBContext _azureDbContext;
//        private List<FTMPersonView> _persons;
//        private List<GraphMarriage> _graphMarriages;

//        public GraphBuilder(AzureDBContext azureDbContext)
//        {
//            _azureDbContext = azureDbContext;

//            _persons = _azureDbContext.FTMPersonView.Where(w => w.Origin == "Thackray").ToList();

//            _graphMarriages = new List<GraphMarriage>();

//            var g = _persons.GroupBy(d => new {d.FatherId, d.MotherId})
//                .Select(m => new {m.Key.FatherId, m.Key.MotherId});

//            foreach(var group in g)
//            {
//                _graphMarriages.Add(new GraphMarriage(){FatherId = group.FatherId.GetValueOrDefault() , MotherId = group.MotherId.GetValueOrDefault() });
//            }


//        }



//        public void GenerateDescendantGraph()
//        {
//            List<List<GraphPerson>> results = new List<List<GraphPerson>>();

//            Console.WriteLine("GenerateDescendantGraph");

//            var startPerson = _persons.FirstOrDefault(fd => fd.PersonId == 22615);

//            var result = GetNextAncestor(startPerson.FatherId);

//            var child = _persons.FirstOrDefault(r => r.PersonId == result);
         
//            results.Add(new List<GraphPerson>());
//            results.Last().Add(new GraphPerson()
//                {
//                    PersonId = child.PersonId.GetValueOrDefault(),
//                    BirthLocation = child.BirthLocation,
//                    ChristianName = child.FirstName,
//                    Surname = child.Surname,
//                    DOB = child.BirthFrom.ToString(),
//                    FatherId = child.FatherId.GetValueOrDefault(),
//                    MotherId = child.MotherId.GetValueOrDefault(),
//                    ChildIdxLst = new List<int>(),
//                    ChildLst = new List<int>(),
//                    SpouseIdxLst = new List<int>(),
//                    SpouseIdLst = new List<int>()
//            }
//            );

//            int currentGeneration = 0;

//            fillChildGenerations(result, ref results, ref currentGeneration);

//            List<GraphPerson> flattenedResults = new List<GraphPerson>();

//            foreach (var g in results) {
//                var idx = 0;
//                foreach(var p in g)
//                {
//                    p.Index = idx;
//                    idx++;
//                    flattenedResults.Add(p);
//                }
//            }




//            Console.WriteLine(result);

//        }


//        private int childGen(List<List<GraphPerson>> destination, int personId)
//        {
//            int genIdx = 0;

//            foreach (var gen in destination)
//            {
//                foreach (var person in gen)
//                {
//                    if (person.FatherId == personId || person.MotherId == personId)
//                    {
//                        if (destination.Count > (genIdx+1))
//                        {
//                            return genIdx + 1;
//                        }
//                    }
//                }
//                genIdx++;
//            }

           

//            return -1;
//        }

//        private GraphPerson getAncestorFromGraph(List<List<GraphPerson>> destination, int personId)
//        {
//            return destination.SelectMany(gen => gen).FirstOrDefault(person => person.PersonId == personId);
//        }

//        private void fillChildGenerations(int personId, ref List<List<GraphPerson>> destination, ref int currentGeneration)
//        {
//            //look up the children for this father.
//            //add them to generation.

//            //search through generations to find the current the generation we need
             
//            string description = "";

//            var person1 = getAncestorFromGraph(destination,personId);

//            if (person1 != null)
//            { 
//                description = person1.DOB + " " + person1.ChristianName + " " + person1.Surname;
                
//                Console.WriteLine(description);
//            }
             

//            var children = _persons.Where(fd => (fd.FatherId == personId) || fd.MotherId == personId).ToList();

//            if (children.Count == 0)
//            {
//               // currentGeneration--;
//                return;
//            }
//            // does tree already contain this person somewhere? if so what generation.

//            currentGeneration = person1.GenerationIdx + 1;

//            if (currentGeneration >= destination.Count)
//            {
//                destination.Add(new List<GraphPerson>());
//            }
           

//            var workingCopy = destination[currentGeneration];

//            List<int> newlyAdded = new List<int>();
//            int childIdx = 0;
//            int spouseCount = 0;

//            foreach (var child in children)
//            {
                
                
//                int fatherIdx = -1;
//                int motherIdx = -1;

//                if (currentGeneration > 0)
//                { 
//                    fatherIdx = GetIndexFromGeneration(destination[currentGeneration - 1], child.FatherId.GetValueOrDefault()); 
//                    motherIdx = GetIndexFromGeneration(destination[currentGeneration - 1], child.MotherId.GetValueOrDefault());
//                }

               
             
//                workingCopy.Add(new GraphPerson()
//                {
                           
//                    IsFamilyStart = childIdx == 0,
//                    PersonId = child.PersonId.GetValueOrDefault(),
//                    BirthLocation = child.BirthLocation,
//                    ChristianName = child.FirstName,
//                    Surname = child.Surname,
//                    DOB = child.BirthFrom.ToString(),
//                    FatherId = child.FatherId.GetValueOrDefault(),
//                    FatherIdx = fatherIdx,
//                    MotherId = child.MotherId.GetValueOrDefault(),
//                    MotherIdx = motherIdx,
//                    GenerationIdx = currentGeneration,
//                    ChildIdxLst = new List<int>(),
//                    ChildLst = new List<int>(),
//                    SpouseIdxLst = new List<int>(),
//                    SpouseIdLst = new List<int>()
//                }
//            );


//                var spouseList = makeSpouse(child.PersonId.GetValueOrDefault(), currentGeneration);

//                if (workingCopy.Count > 0)
//                {
//                    var lastAdded = workingCopy.Last();

//                    var lastIdx = workingCopy.Count;

//                    var spouseIdx = 0;

//                    //populate spouseIdxList
//                    while (spouseIdx < spouseList.Count)
//                    {
//                        lastAdded.SpouseIdxLst.Add(lastIdx);
//                        lastAdded.SpouseIdLst.Add(spouseList[spouseIdx].PersonId);
//                        lastIdx++;
//                        spouseIdx++;
//                    }
//                }



//                if (currentGeneration > 0)
//                {
//                    if (fatherIdx >= 0)
//                    {
//                        destination[currentGeneration - 1][fatherIdx].ChildIdxLst.Add(workingCopy.Count-1);
//                        destination[currentGeneration - 1][fatherIdx].ChildLst.Add(child.PersonId.GetValueOrDefault());
//                        destination[currentGeneration - 1][fatherIdx].ChildCount = children.Count;
//                    }
//                    if (motherIdx >= 0)
//                    {
//                        destination[currentGeneration - 1][motherIdx].ChildIdxLst.Add(workingCopy.Count - 1);
//                        destination[currentGeneration - 1][motherIdx].ChildLst.Add(child.PersonId.GetValueOrDefault());
//                        destination[currentGeneration - 1][motherIdx].ChildCount = children.Count;
//                    }
//                }
                
//                workingCopy.AddRange(spouseList);

//                newlyAdded.Add(child.PersonId.GetValueOrDefault());
                    
//                spouseCount += spouseList.Count;

//                //the last child
//                if(childIdx == (children.Count-1))
//                {
//                    //family length 
//                    var familyLength = childIdx + spouseCount;

//                    var midPoint = (int)Math.Floor((decimal)familyLength / 2);

//                    workingCopy[midPoint].IsParentalLink = true;
//                    workingCopy[familyLength].IsFamilyEnd = true;
//                }
                    
//                childIdx++;


//            }

//            //gen 1
//            foreach (var person in newlyAdded)
//            {
//                fillChildGenerations(person, ref destination, ref currentGeneration);
//            }

//            //foreach child 
            

//        }

//        private int GetIndexFromGeneration(List<GraphPerson> generation, int searchId)
//        {
//            int idx = 0;

//            foreach (var person in generation)
//            {
//                if (searchId == person.PersonId)
//                    return idx;
//                idx++;
//            }

//            return -1;
//        }

//        private List<GraphPerson> makeSpouse(int personId, int generationNumber)
//        {
//            var marriages = _graphMarriages.Where(w => w.FatherId == personId || w.MotherId == personId).ToList();
//            List<int> spouseIdList = new List<int>();
//            List<GraphPerson> spouses = new List<GraphPerson>();

//            foreach (var m in marriages)
//            {
//                if(m.FatherId == personId) spouseIdList.Add(m.MotherId);
//                if(m.MotherId == personId) spouseIdList.Add(m.FatherId);
//            }

//            var persons = _persons.Where(w => spouseIdList.Contains(w.PersonId.GetValueOrDefault()));

//            foreach (var person in persons)
//            {
//                spouses.Add(new GraphPerson()
//                {
//                    BirthLocation = person.BirthLocation,
//                    ChristianName = person.FirstName,
//                    DOB = person.BirthFrom.GetValueOrDefault().ToString(),
//                    FatherId = person.FatherId.GetValueOrDefault(),
//                    MotherId = person.MotherId.GetValueOrDefault(),
//                    GenerationIdx = generationNumber,
//                    PersonId = person.PersonId.GetValueOrDefault(),
//                    Surname = person.Surname, 
//                    ChildIdxLst = new List<int>(),
//                    ChildLst = new List<int>(),
//                    SpouseIdxLst = new List<int>(),
//                    SpouseIdLst = new List<int>()
//                });
//            }

//            return spouses;
//        }

//        private int GetNextAncestor(int? personId)
//        {
//            var startPerson = _persons.FirstOrDefault(fd => fd.PersonId == personId);
            
//            if (startPerson == null) return 0;

//            return startPerson.FatherId == 0 ? startPerson.PersonId.GetValueOrDefault() : GetNextAncestor(startPerson.FatherId);
//        }
//    }
//}

