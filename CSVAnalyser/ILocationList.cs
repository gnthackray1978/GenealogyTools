using System.Collections.Generic;

namespace CSVAnalyser
{
    public interface ILocationList
    {
        List<ILocation> Locations { get; }

        string Find(string testValue);

        void Generate();
    }
}