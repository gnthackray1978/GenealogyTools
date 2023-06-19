namespace PlaceLibNet.Domain;

public interface IPlaceNameFormatter
{
    string Format(string place);

    string FormatComponent(string place);
}