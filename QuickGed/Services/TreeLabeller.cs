using QuickGed.Types;

namespace QuickGed.Services;

public static class TreeLabeller
{
    public static void LabelAncestors(Dictionary<int, List<Node>> parentsDictionary,
        HashSet<Node> parentsToLookup, List<Node> siblingList, Node startNode, string label, bool isDirectAncestor =true)
    {
        var stack = new Stack<Node>();
        stack.Push(startNode);

        while (stack.Count > 0)
        {
            var current = stack.Pop();
            //if (current.FullName.Contains("Estelle A Belcher"))
            //{
            //    Console.WriteLine("z");
            //}


            current.Origin = label;
            current.IsDirectAncestor = isDirectAncestor;

            if (!current.IsLinkNode && !current.IsRootPerson)
            {
                parentsToLookup.UnionWith(current.Spouses.Where(w => !w.IsLinkNode));
                parentsToLookup.Add(current); //for single parents - this does lead to so redundancy down
            }


            if (parentsDictionary.ContainsKey(current.Id))
            {
                foreach (var parent in parentsDictionary[current.Id])
                {
                    stack.Push(parent);
                }
            }

            siblingList.AddRange(current.Siblings);

        }


    }

    public static void LabelDescendants(IEnumerable<Node> originator, string label, bool isDirectAncestor)
    {
        foreach (var nodeWithChildren in originator)
        {
            LabelDescendant(nodeWithChildren, label,isDirectAncestor);
        }
    }

    public static void LabelDescendant(Node originator, string label, bool isDirectAncestor)
    {
        var stack = new Stack<Node>();
        stack.Push(originator);

        while (stack.Count > 0)
        {
            var current = stack.Pop();

            current.Origin = label;
            current.IsDirectAncestor = isDirectAncestor;

            foreach (var spouse in current.Spouses)
            {
                spouse.Origin = label;
                spouse.IsDirectAncestor = isDirectAncestor;
            }
            
            //we need a better algorithm for calculating origins of descendants 
            //this is because the same child can be added twice.
            //the algorithm has a flaw so that when a ancestors descendants are calculated
            //it uses the a ancestors spouses to work out who their children are.
            //if the ancestor has children where the spouse is unknown there is no spouse
            //so in order to get those children, the ancestor him/herself is added
            //into the spouse list. 
            //this in turn means that iterating over the spouse list to get children
            //sometimes means that the same child will be added twice because
            //they are in there from the spouse and from the ancestor.
            foreach (var child in current.Children.Where(w=>w.Origin==""))
                stack.Push(child);

        }
    }


    public static void LabelTree(Dictionary<int, List<Node>> parentsCache, Node startNode, string label)
    {
        var siblings = new List<Node>();
        var parentsToLookup = new HashSet<Node>();

        LabelAncestors(parentsCache, parentsToLookup, siblings, startNode, label);

        LabelDescendants(siblings, label,false);

        LabelDescendants(parentsToLookup, label,false);

        foreach (var spouse in parentsToLookup.ToList())
        {
            LabelAncestors(parentsCache, parentsToLookup, siblings, spouse, label,false);

            LabelDescendants(siblings, label, false);

            LabelDescendants(parentsToLookup, label, false);
        }


    }
}