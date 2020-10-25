using CSVAnalyser;
using System.Collections.Generic;

namespace PlaceLib
{
    public interface ILocationList
    {
        List<ILocation> Locations { get; }

        string Find(string testValue);

        void Generate();
    }
}