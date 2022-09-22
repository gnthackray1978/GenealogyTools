// Decompiled with JetBrains decompiler
// Type: MyFamily.Shared.Date
// Assembly: MyFamily.Shared, Version=23.3.0.1570, Culture=neutral, PublicKeyToken=295e8b5bb13c2421
// MVID: CB662BCB-E11E-4698-B7B4-24BC7C8B503C
// Assembly location: C:\Users\GeorgePickworth\Documents\development\GenealogyTools\FTMContextNet\MyFamily.Shared.dll

using FTM.Dates.Interfaces;
using System;
using System.Globalization;

namespace FTM.Dates
{
  public abstract class Date : 
    IDate,
    IComparable<IDate>,
    IFormattableEx,
    IFormattable,
    IEquatable<Date>,
    IComparable<Date>
  {
    private static FormatInfo s_formatter = (FormatInfo) DateFormatInfo.Default;
    public uint? _code;
    protected DateModifier _modifier;
    internal const uint MODIFIER_MASK = 511;
    internal const uint PROXIMITY_MASK = 7;
    internal const uint SDN_MASK = 2147483136;
    internal const uint ENCODING_MASK = 2147483648;
    internal const uint DAYMISSING_MASK = 128;
    internal const uint MONTHMISSING_MASK = 64;
    internal const uint YEARMISSING_MASK = 32;
    internal const uint DUALDATE_MASK = 16;
    internal const uint QUARTER_DATE_MASK = 8;
    internal const uint MODIFIER_NONE = 0;
    internal const uint MODIFIER_BEFORE = 1;
    internal const uint MODIFIER_AFTER = 2;
    internal const uint MODIFIER_ABOUT = 3;
    internal const uint MODIFIER_CALCULATED = 4;
    internal const uint ENCODING_SDN = 0;
    internal const uint ENCODING_KEYWORD = 2147483648;
    internal const uint KEYWORD_MASK = 65535;
    internal const int SDN_SHIFT = 9;

    protected Date(uint? code) => this._code = code;

    public abstract uint SortDate { get; }

    public static int SortModifierFromModifier(uint modifier)
    {
      int num = 0;
      if (((int) modifier & 1) != 0)
        num = -1;
      else if (((int) modifier & 2) != 0)
        num = 1;
      return num;
    }

    public int SortModifier => Date.SortModifierFromModifier(this.SortDate & 7U);

    public static Date CreateInstance(string unparsed) => (Date) new TextDate(unparsed);

    public static Date CreateInstance(uint? code) => Date.CreateInstance(code, new uint?());

    public static Date CreateInstance(uint? code1, uint? code2)
    {
      if (!code1.HasValue)
        return (Date) null;
      uint? nullable1 = code1;
      uint num1 = 0;
      if ((int) nullable1.GetValueOrDefault() == (int) num1 & nullable1.HasValue)
        return (Date) null;
      if (code2.HasValue)
        return (Date) new DateRange(code1.Value, code2.Value);
      uint? nullable2 = code1;
      nullable1 = nullable2.HasValue ? new uint?(nullable2.GetValueOrDefault() & 2147483648U) : new uint?();
      uint num2 = 0;
      return (int) nullable1.GetValueOrDefault() == (int) num2 & nullable1.HasValue ? (Date) new SDNDate(code1.Value) : (Date) new KeywordDate(code1.Value);
    }

    public static uint? Encode(Date date) => date._code;

    public virtual DateModifier Modifier => DateModifier.None;

    public int? Year => (this._modifier & DateModifier.YearMissing) > DateModifier.None ? new int?() : this.GetYear((Calendar) new GregorianCalendar(GregorianCalendarTypes.Localized));

    public int? Month => (this._modifier & DateModifier.MonthMissing) > DateModifier.None ? new int?() : this.GetMonth((Calendar) new GregorianCalendar(GregorianCalendarTypes.Localized));

    public int? Day => (this._modifier & DateModifier.DayMissing) > DateModifier.None ? new int?() : this.GetDay((Calendar) new GregorianCalendar(GregorianCalendarTypes.Localized));

    protected virtual int? GetYear(Calendar cal) => new int?();

    protected virtual int? GetMonth(Calendar cal) => new int?();

    protected virtual int? GetDay(Calendar cal) => new int?();

    public override bool Equals(object obj) => this.Equals(obj as Date);

    public abstract bool Equals(Date date);

    public override int GetHashCode() => !this._code.HasValue ? 0 : (int) this._code.Value;

    public virtual int CompareTo(Date other)
    {
      if (other == null)
        return -1;
      int num1 = this.SortDate.CompareTo(other.SortDate);
      if (this._modifier != DateModifier.None || other._modifier != DateModifier.None)
      {
        uint num2 = this.SortDate & 4294967288U;
        uint num3 = other.SortDate & 4294967288U;
        DateModifier dateModifier1 = (DateModifier) ((int) this.SortDate & 7);
        DateModifier dateModifier2 = (DateModifier) ((int) other.SortDate & 7);
        if (num2.CompareTo(num3) == 0)
        {
          if (dateModifier1 == DateModifier.None && dateModifier2 == DateModifier.Before)
            num1 = 1;
          else if (dateModifier1 == DateModifier.None && dateModifier2 == DateModifier.After)
            num1 = -1;
          else if (dateModifier1 == DateModifier.Before && dateModifier2 == DateModifier.None)
            num1 = -1;
          else if (dateModifier1 == DateModifier.Before && dateModifier2 == DateModifier.After)
            num1 = -1;
          else if (dateModifier1 == DateModifier.After && dateModifier2 == DateModifier.None)
            num1 = 1;
          else if (dateModifier1 == DateModifier.After && dateModifier2 == DateModifier.Before)
            num1 = 1;
        }
      }
      return num1;
    }

    int IComparable<IDate>.CompareTo(IDate other)
    {
      int num = ((IComparable<Date>) this).CompareTo((Date) other);
      if (num != 0 || !(other is IRange<IDate>))
        return num;
      return !(this is IRange<IDate>) ? -1 : ((IComparable<Date>) (this as IRange<IDate>).End).CompareTo((Date) (other as IRange<IDate>).End);
    }

    public static FormatInfo DefaultFormatter
    {
      get => Date.s_formatter;
      set => Date.s_formatter = value;
    }

    public override string ToString() => Date.s_formatter.Format((string) null, (object) this, (IFormatProvider) null);

    public string ToString(string format) => Date.s_formatter.Format(format, (object) this, (IFormatProvider) null);

    public string ToString(IFormatProvider formatProvider) => Date.s_formatter.Format((string) null, (object) this, formatProvider);

    public FormatInfo Formatter
    {
      get => Date.s_formatter;
      set
      {
      }
    }

    public string ToString(string format, IFormatProvider formatProvider) => Date.s_formatter.Format(format, (object) this, formatProvider);
  }
}
