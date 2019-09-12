using System.Collections.Generic;

namespace PlaceLib
{
    public class PlaceLine : List<string>
    {
        public PlaceLine(string place)
        {
            string[] placeComponents;
            if (place.Contains(","))
            {
                placeComponents = place.ToLower().Split(',');
            }
            else
            {
                placeComponents = place.ToLower().Split(' ');
            }

            this.AddRange(placeComponents);
        }

        public void LoadIntoCollection(PlaceDto placetoCheck, PlaceCollection collection)
        {
            foreach (var placeComponent in this)
            {
                if (placeComponent.Trim() == placetoCheck.Place)
                {
                    if (!collection.Contains(placetoCheck))
                        collection.Add(placetoCheck);
                }
            }
        }

        public void Check(PlaceDto originalPlace,  PlaceDto placetoCheck, PlaceCollection collection)
        {
            foreach (var placeComponent in this)
            {
                if (placeComponent.Trim() == placetoCheck.Place)
                {
                    if (!collection.Contains(originalPlace))
                        collection.Add(originalPlace);
                }
            }
        }

    }
}