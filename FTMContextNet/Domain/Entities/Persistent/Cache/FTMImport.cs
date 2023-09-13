namespace FTMContextNet.Domain.Entities.Persistent.Cache;

public partial class TreeImport
{
    public int Id { get; set; }
    public string DateImported { get; set; }

    public double FileSize { get; set; }

    public string FileName { get; set; }

    public bool Selected { get; set; }

    public int UserId { get; set; }

}