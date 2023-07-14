namespace PlaceLibNet.Domain;

public interface IPlaceNameFormatter
{
    string ValidateSupportedNation(string place, char placeMarker = '/');
    string Format(string place);
    string FormatComponent(string place);
}