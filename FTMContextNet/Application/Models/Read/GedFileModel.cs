using System;

namespace FTMContextNet.Application.Models.Read;

public class GedFileModel
{
    public string FileName { get; set; }

    public double FileSize { get; set; }

    public int Id { get; set; }

    public DateTime DateImported { get; set; }
  
}