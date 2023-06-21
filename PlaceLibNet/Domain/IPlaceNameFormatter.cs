namespace PlaceLibNet.Domain;

public interface IPlaceNameFormatter
{
    bool IsValidEnglandWales(string place, char placeMarker = '/');
    string Format(string place);
    string FormatComponent(string place);
}