// Decompiled with JetBrains decompiler
// Type: MyFamily.Shared.DateRange
// Assembly: MyFamily.Shared, Version=23.3.0.1570, Culture=neutral, PublicKeyToken=295e8b5bb13c2421
// MVID: CB662BCB-E11E-4698-B7B4-24BC7C8B503C
// Assembly location: C:\Users\GeorgePickworth\Documents\development\GenealogyTools\FTMContextNet\MyFamily.Shared.dll

using FTM.Dates.Interfaces;
using System;

namespace FTM.Dates
{
  public class DateRange : 
    SDNDate,
    IRange<Date>,
    IComparable<IRange<Date>>,
    IRange<IDate>,
    IComparable<IRange<IDate>>,
    IEquatable<DateRange>
  {
    private SDNDate _end;

    internal DateRange(uint begin, uint end)
      : base(begin)
    {
      this._end = new SDNDate(end);
    }

    public SDNDate Begin => new SDNDate(this._code.Value);

    public SDNDate End => this._end;

    Date IRange<Date>.Begin => (Date) this.Begin;

    Date IRange<Date>.End => (Date) this.End;

    IDate IRange<IDate>.Begin => (IDate) this.Begin;

    IDate IRange<IDate>.End => (IDate) this.End;

    public override int GetHashCode() => base.GetHashCode() ^ this._end.GetHashCode();

    bool IEquatable<DateRange>.Equals(DateRange other) => this.Equals((object) other.Begin) && this._end.Equals((Date) other.End);

    int IComparable<IRange<IDate>>.CompareTo(IRange<IDate> other) => ((IComparable<IRange<Date>>) this).CompareTo((IRange<Date>) other);

    int IComparable<IRange<Date>>.CompareTo(IRange<Date> other)
    {
      int num = ((IComparable<Date>) this).CompareTo(other.Begin);
      if (num == 0)
        num = ((IComparable<Date>) this._end).CompareTo(other.End);
      return num;
    }
  }
}
