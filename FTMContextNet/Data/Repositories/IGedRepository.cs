using QuickGed;

namespace FTMContextNet.Data.Repositories;

public interface IGedRepository
{
    GedDb ParseLabelledTree(string path);
}