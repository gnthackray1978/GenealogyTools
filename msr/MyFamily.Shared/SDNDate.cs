// Decompiled with JetBrains decompiler
// Type: MyFamily.Shared.SDNDate
// Assembly: MyFamily.Shared, Version=23.3.0.1570, Culture=neutral, PublicKeyToken=295e8b5bb13c2421
// MVID: CB662BCB-E11E-4698-B7B4-24BC7C8B503C
// Assembly location: C:\Users\GeorgePickworth\Documents\development\GenealogyTools\FTMContextNet\MyFamily.Shared.dll

using FTM.Dates.Interfaces;
using System;
using System.Globalization;
using System.Linq;

namespace FTM.Dates
{
  public class SDNDate : Date
  {
    private const int MAX_YEAR = 6770;
    public static readonly int MaxYear = 6770;
    private uint? _sortDate;

    internal SDNDate(uint code)
      : base(new uint?(code))
    {
      this._modifier = (DateModifier) ((int) this._code.Value & 511);
    }

    public SDNDate(uint sdn, DateModifier modifier)
      : base(new uint?(SDNDate.Encode(sdn, modifier)))
    {
      this._modifier = modifier;
    }

    public static uint Encode(DateTime dt) => SDNDate.Encode(new int?(dt.Year), new int?(dt.Month), new int?(dt.Day), DateModifier.None);

    public static uint Encode(uint serialDayNumber) => SDNDate.Encode(serialDayNumber, DateModifier.None);

    public static uint Encode(uint serialDayNumber, DateModifier modifier) => (uint) ((DateModifier) ((int) serialDayNumber << 9) | modifier);

    public static uint Encode(int? year, int? month, int? day, DateModifier modifier)
    {
      int? nullable;
      int inputYear;
      if (year.HasValue)
      {
        nullable = year;
        int num = 0;
        if (!(nullable.GetValueOrDefault() == num & nullable.HasValue))
        {
          inputYear = year.Value;
          goto label_4;
        }
      }
      inputYear = -4713;
label_4:
      int num1;
      if (month.HasValue)
      {
        nullable = month;
        int num2 = 0;
        if (!(nullable.GetValueOrDefault() == num2 & nullable.HasValue))
        {
          num1 = month.Value;
          goto label_8;
        }
      }
      num1 = 1;
label_8:
      int num3 = num1;
      int num4;
      if (day.HasValue)
      {
        nullable = day;
        int num5 = 0;
        if (!(nullable.GetValueOrDefault() == num5 & nullable.HasValue))
        {
          num4 = day.Value;
          goto label_12;
        }
      }
      num4 = 1;
label_12:
      int num6 = num4;
      int inputMonth = num3;
      int inputDay = num6;
      int sdn = FTM.Dates.SerialDayNumber.GregorianToSdn(inputYear, inputMonth, inputDay);
      if (year.HasValue)
      {
        nullable = year;
        int num7 = 0;
        if (!(nullable.GetValueOrDefault() == num7 & nullable.HasValue))
          goto label_15;
      }
      modifier |= DateModifier.YearMissing;
label_15:
      if (month.HasValue)
      {
        nullable = month;
        int num8 = 0;
        if (!(nullable.GetValueOrDefault() == num8 & nullable.HasValue))
          goto label_18;
      }
      modifier |= DateModifier.MonthMissing;
label_18:
      if (day.HasValue)
      {
        nullable = day;
        int num9 = 0;
        if (!(nullable.GetValueOrDefault() == num9 & nullable.HasValue))
          goto label_21;
      }
      modifier |= DateModifier.DayMissing;
label_21:
      return (uint) ((DateModifier) (sdn << 9) | modifier);
    }

    public override uint SortDate
    {
      get
      {
        if (!this._sortDate.HasValue)
        {
          if (this._code.HasValue)
          {
            uint? code = this._code;
            uint num = 0;
            if (!((int) code.GetValueOrDefault() == (int) num & code.HasValue))
            {
              this._sortDate = this.Year.HasValue ? new uint?(this._code.Value) : new uint?(SDNDate.Encode(new int?(6770), this.Month, this.Day, this.Modifier));
              goto label_5;
            }
          }
          this._sortDate = new uint?(uint.MaxValue);
        }
label_5:
        return this._sortDate.Value;
      }
    }

    public static Date CalculateMean(Date[] dates)
    {
      ulong num1 = 0;
      ulong num2 = 0;
      foreach (SDNDate sdnDate in dates.OfType<SDNDate>())
      {
        uint serialDayNumber = sdnDate.SerialDayNumber;
        num1 += (ulong) serialDayNumber;
        ++num2;
      }
      return (Date) new SDNDate(SDNDate.Encode(num2 == 0UL ? 0U : (uint) (num1 / num2)));
    }

    public override DateModifier Modifier => this._modifier;

    public uint SerialDayNumber => (this._code.Value & 2147483136U) >> 9;

    public override bool Equals(Date date)
    {
      if (!(date is SDNDate))
        return false;
      uint? code1 = this._code;
      uint? code2 = date._code;
      return (int) code1.GetValueOrDefault() == (int) code2.GetValueOrDefault() & code1.HasValue == code2.HasValue;
    }

    protected override int? GetYear(Calendar cal)
    {
      if ((this.Modifier & DateModifier.YearMissing) != 0)
        return new int?();
      switch (cal)
      {
        case GregorianCalendar _:
          int year1;
          FTM.Dates.SerialDayNumber.SdnToGregorian((int) this.SerialDayNumber, out year1, out int _, out int _);
          return new int?(year1);
        case JulianCalendar _:
          int year2;
          FTM.Dates.SerialDayNumber.SdnToJulian((int) this.SerialDayNumber, out year2, out int _, out int _);
          return new int?(year2);
        default:
          return new int?();
      }
    }

    protected override int? GetMonth(Calendar cal)
    {
      if ((this.Modifier & DateModifier.MonthMissing) != 0)
        return new int?();
      switch (cal)
      {
        case GregorianCalendar _:
          int month1;
          FTM.Dates.SerialDayNumber.SdnToGregorian((int) this.SerialDayNumber, out int _, out month1, out int _);
          return new int?(month1);
        case JulianCalendar _:
          int month2;
          FTM.Dates.SerialDayNumber.SdnToJulian((int) this.SerialDayNumber, out int _, out month2, out int _);
          return new int?(month2);
        default:
          return new int?();
      }
    }

    protected override int? GetDay(Calendar cal)
    {
      if ((this.Modifier & DateModifier.DayMissing) != 0)
        return new int?();
      switch (cal)
      {
        case GregorianCalendar _:
          int day1;
          FTM.Dates.SerialDayNumber.SdnToGregorian((int) this.SerialDayNumber, out int _, out int _, out day1);
          return new int?(day1);
        case JulianCalendar _:
          int day2;
          FTM.Dates.SerialDayNumber.SdnToJulian((int) this.SerialDayNumber, out int _, out int _, out day2);
          return new int?(day2);
        default:
          return new int?();
      }
    }
  }
}
