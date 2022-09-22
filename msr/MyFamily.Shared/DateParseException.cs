// Decompiled with JetBrains decompiler
// Type: MyFamily.Shared.DateParseException
// Assembly: MyFamily.Shared, Version=23.3.0.1570, Culture=neutral, PublicKeyToken=295e8b5bb13c2421
// MVID: CB662BCB-E11E-4698-B7B4-24BC7C8B503C
// Assembly location: C:\Users\GeorgePickworth\Documents\development\GenealogyTools\FTMContextNet\MyFamily.Shared.dll

using System;

namespace FTM.Dates
{
  public class DateParseException : Exception
  {
    private DateParseError _error;

    internal DateParseException(DateParseError error) => this._error = error;

    internal DateParseException(DateParseError error, string msg)
      : base(msg)
    {
      this._error = error;
    }

    internal DateParseException(DateParseError error, string msg, Exception innerException)
      : base(msg, innerException)
    {
      this._error = error;
    }

    public DateParseError Error => this._error;

    public uint? FirstDate => this._error.FirstDate;

    public uint? SecondDate => this._error.SecondDate;
  }
}
