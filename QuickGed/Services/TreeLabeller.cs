using QuickGed.Types;

namespace QuickGed.Services;

public static class TreeLabeller
{
    public static void LabelAncestors(Dictionary<int, List<Node>> parentsDictionary,
        HashSet<Node> spouseList, List<Node> siblingList, Node startNode, string label, bool isDirectAncestor =true)
    {
        var stack = new Stack<Node>();
        stack.Push(startNode);

        while (stack.Count > 0)
        {
            var current = stack.Pop();

            //if (current.Name == "Missy")
            //{
            //    Console.WriteLine("z");
            //}d
            current.Origin = label;
            current.IsDirectAncestor = isDirectAncestor;
            spouseList.UnionWith(current.Spouses.Where(w => !w.IsParent));

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

            foreach (var child in current.Children)
                stack.Push(child);

        }
    }


    public static void LabelTree(Dictionary<int, List<Node>> parents, Node startNode, string label)
    {
        var siblings = new List<Node>();
        var spouses = new HashSet<Node>();

        LabelAncestors(parents, spouses, siblings, startNode, label);

        LabelDescendants(siblings, label,false);

        LabelDescendants(spouses, label,false);

        foreach (var spouse in spouses.ToList())
        {
            LabelAncestors(parents, spouses, siblings, spouse, label,false);
        }


    }
}