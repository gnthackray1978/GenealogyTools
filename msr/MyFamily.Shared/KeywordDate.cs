// Decompiled with JetBrains decompiler
// Type: MyFamily.Shared.KeywordDate
// Assembly: MyFamily.Shared, Version=23.3.0.1570, Culture=neutral, PublicKeyToken=295e8b5bb13c2421
// MVID: CB662BCB-E11E-4698-B7B4-24BC7C8B503C
// Assembly location: C:\Users\GeorgePickworth\Documents\development\GenealogyTools\FTMContextNet\MyFamily.Shared.dll

namespace FTM.Dates
{
  public class KeywordDate : Date
  {
    internal KeywordDate(uint code)
      : base(new uint?(code))
    {
    }

    public KeywordDate(DateKeyword keyword)
      : base(new uint?(KeywordDate.Encode(keyword)))
    {
    }

    public override uint SortDate => uint.MaxValue;

    public static uint Encode(DateKeyword keyword) => (uint) (keyword);

    public DateKeyword Keyword
    {
      get
      {
        uint? code = this._code;
        return (DateKeyword) (code.HasValue ? new uint?(code.GetValueOrDefault() & (uint) ushort.MaxValue) : new uint?()).Value;
      }
    }

    public override bool Equals(Date date)
    {
      if (!(date is KeywordDate))
        return false;
      uint? code1 = this._code;
      uint? code2 = date._code;
      return (int) code1.GetValueOrDefault() == (int) code2.GetValueOrDefault() & code1.HasValue == code2.HasValue;
    }
  }
}
