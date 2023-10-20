using QuickGed;

namespace FTMContextNet.Data.Repositories.GedProcessing;

public interface IGedRepository
{
    GedDb ParseLabelledTree(string path);
}