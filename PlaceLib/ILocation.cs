using System.Collections.Generic;

namespace CSVAnalyser
{
    public interface ILocation
    {
        string Name { get; set; }

        List<string> Aliases { get;}

        void AddAlias(string value);


    }
}