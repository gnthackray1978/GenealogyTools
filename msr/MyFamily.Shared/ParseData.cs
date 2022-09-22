// Decompiled with JetBrains decompiler
// Type: MyFamily.Shared.ParseData
// Assembly: MyFamily.Shared, Version=23.3.0.1570, Culture=neutral, PublicKeyToken=295e8b5bb13c2421
// MVID: CB662BCB-E11E-4698-B7B4-24BC7C8B503C
// Assembly location: C:\Users\GeorgePickworth\Documents\development\GenealogyTools\FTMContextNet\MyFamily.Shared.dll

using FTM.Dates.Interfaces;
using System.Collections.Generic;

namespace FTM.Dates
{
  internal class ParseData
  {
    private string _dateStr;
    private uint? _firstDate;
    private uint? _secondDate;
    private DateModifier _qualifier;
    public bool ParsingSecondDate;
    private IList<FTM.Dates.DateParseErrorInfo> _dateParseErrorInfo = (IList<FTM.Dates.DateParseErrorInfo>) new List<FTM.Dates.DateParseErrorInfo>();

    internal ParseData(string dateStr, uint? firstDate, uint? secondDate)
    {
      this._dateStr = dateStr;
      this._firstDate = firstDate;
      this._secondDate = secondDate;
    }

    internal string DateStr
    {
      get => this._dateStr;
      set => this._dateStr = value;
    }

    internal uint? FirstDate
    {
      get => this._firstDate;
      set => this._firstDate = value;
    }

    internal uint? SecondDate
    {
      get => this._secondDate;
      set => this._secondDate = value;
    }

    internal DateModifier Qualifier
    {
      get => this._qualifier;
      set => this._qualifier = value;
    }

    internal IList<FTM.Dates.DateParseErrorInfo> DateParseErrorInfo => this._dateParseErrorInfo;
  }
}
