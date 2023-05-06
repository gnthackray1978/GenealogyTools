namespace FTMContextNet.Domain.Entities.Persistent.Cache;

public partial class FTMImport
{
    public int Id { get; set; }
    public string DateImported { get; set; }

    public double FileSize { get; set; }

    public string FileName { get; set; }
        

}