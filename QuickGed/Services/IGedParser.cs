namespace QuickGed.Services;

public interface IGedParser
{
    GedDb Parse(string path);
}