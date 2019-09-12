namespace PlaceLib
{
    public class State
    {
        public State(string ab, string name)
        {
            Name = name;
            Abbreviation = ab;
        }

        public string Name { get; set; }

        public string Abbreviation { get; set; }

        public override string ToString()
        {
            return string.Format("{0} - {1}", Abbreviation, Name);
        }
    }
}