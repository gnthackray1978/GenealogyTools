namespace QuickGed.Domain
{
    public class NodeTypeCalculator : INodeTypeCalculator
    {
        public bool IsRootPerson(string forename, string surname)
        {
            var isMatch = false;

            if (forename.ToLower().Contains("group") || surname.ToLower().Contains("group")) return false;

            if ((forename.ToLower().Contains("chr") || surname.ToLower().Contains("chr")) && !surname.ToLower().Contains("christ")) return false;

            if (forename.ToLower().Contains("|") || surname.ToLower().Contains("|")) return true;

            return isMatch;
        }

        public bool IsRootPerson(string fullName)
        {
            var isMatch = false;

            if (fullName.ToLower().Contains("group")) return false;

            if (fullName.ToLower().Contains("chr")) return false;

            if (fullName.ToLower().Contains("|")) return true;

            return isMatch;
        }

        public bool IsLinkNode(string forename, string surname)
        {
            if (forename.ToLower().Contains("group") || surname.ToLower().Contains("group")) return true;

            if (forename.ToLower().Contains("chr") || surname.ToLower().Contains("chr")) return true;
 
            return false;
        }

        public bool IsLinkNode(string fullName)
        {
            if (fullName.ToLower().Contains("group")) return true;

            if (fullName.ToLower().Contains("chr")) return true;
 
            return false;
        }
    }
}
