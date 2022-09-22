// Decompiled with JetBrains decompiler
// Type: MyFamily.Shared.Age
// Assembly: MyFamily.Shared, Version=23.3.0.1570, Culture=neutral, PublicKeyToken=295e8b5bb13c2421
// MVID: CB662BCB-E11E-4698-B7B4-24BC7C8B503C
// Assembly location: C:\Users\GeorgePickworth\Documents\development\GenealogyTools\FTMContextNet\MyFamily.Shared.dll

using FTM.Dates.Interfaces;
using System;

namespace FTM.Dates
{
  public class Age : IAge, IFormattableEx, IFormattable
  {
    protected FormatInfo _formatter = (FormatInfo) AgeFormatInfo.Default;
    protected uint _firstDate;
    protected uint _secondDate;
    protected int _years;
    protected int _months;
    protected int _days;
    protected static uint _todaySerialDayNumber;
    protected static int[] _monthDays = new int[13]
    {
      0,
      31,
      28,
      31,
      30,
      31,
      30,
      31,
      31,
      30,
      31,
      30,
      31
    };
    private bool _calculated;

    public bool WasCalculated => this._calculated;

    public Age(uint firstDate, uint secondDate)
    {
      if (firstDate != 0U && secondDate == 0U)
      {
        this.Initialize(new uint?(firstDate), new uint?(this.TodaySerialDayNumber), true);
      }
      else
      {
        if (firstDate == 0U)
          return;
        this.Initialize(new uint?(firstDate), new uint?(secondDate), true);
      }
    }

    public Age(uint firstDate)
    {
      this.Initialize(new uint?(firstDate), new uint?(this.TodaySerialDayNumber), true);
      this.Calculate();
    }

    public Age(SDNDate firstDate, SDNDate secondDate, bool fixDateOrder)
    {
      if (firstDate == null || !firstDate.Year.HasValue || secondDate != null && (secondDate == null || !secondDate.Year.HasValue))
        return;
      if (secondDate == null)
        this.Initialize(new uint?(firstDate.SerialDayNumber), new uint?(this.TodaySerialDayNumber), fixDateOrder);
      else
        this.Initialize(new uint?(firstDate.SerialDayNumber), new uint?(secondDate.SerialDayNumber), fixDateOrder);
    }

    public Age(SDNDate firstDate, SDNDate secondDate)
      : this(firstDate, secondDate, true)
    {
    }

    public Age(SDNDate firstDate)
      : this(firstDate, (SDNDate) null)
    {
    }

    protected string SDNToString(uint dateSDN) => new SDNDate(dateSDN, DateModifier.None).ToString();

    public string StartDateAsString => this.SDNToString(this._firstDate);

    public string EndDateAsString => this.SDNToString(this._secondDate);

    public int StartDateAsSDN => (int) this._firstDate;

    public int EndDateAsSDN => (int) this._secondDate;

    public void Add(int addYears, int addMonths, int addDays)
    {
      int year;
      int month;
      int day;
      SerialDayNumber.SdnToGregorian((int) this._firstDate, out year, out month, out day);
      if (year < 0 && addYears >= Math.Abs(year))
        ++year;
      int num1 = year + addYears;
      int num2 = month + addMonths;
      while (num2 > 12)
      {
        num2 -= 12;
        ++num1;
        if (num1 == 0)
          ++num1;
      }
      if (day > this.MonthDays(num1, num2))
        day = this.MonthDays(num1, num2);
      int inputDay = day + addDays;
      while (inputDay > this.MonthDays(num1, num2))
      {
        inputDay -= this.MonthDays(num1, num2);
        ++num2;
        if (num2 > 12)
        {
          num2 = 1;
          ++num1;
          if (num1 == 0)
            ++num1;
        }
      }
      this._secondDate = (uint) SerialDayNumber.GregorianToSdn(num1, num2, inputDay);
    }

    public void Subtract(int addYears, int addMonths, int addDays)
    {
      int year;
      int month;
      int day;
      SerialDayNumber.SdnToGregorian((int) this._firstDate, out year, out month, out day);
      this._secondDate = this._firstDate;
      if (day > this.MonthDays(year, month))
        day = this.MonthDays(year, month);
      int inputDay;
      for (inputDay = day - addDays; inputDay < 1; inputDay += this.MonthDays(year, month))
      {
        --month;
        if (month == 0)
        {
          month = 12;
          --year;
          if (year == 0)
            --year;
        }
      }
      int num1 = month - addMonths;
      while (num1 < 1)
      {
        num1 += 12;
        --year;
        if (year == 0)
          --year;
      }
      if (year > 0 && addYears >= year)
        --year;
      int num2 = year - addYears;
      if (inputDay > this.MonthDays(num2, num1))
        inputDay = this.MonthDays(num2, num1);
      this._firstDate = (uint) SerialDayNumber.GregorianToSdn(num2, num1, inputDay);
    }

    protected void Initialize(uint? firstDate, uint? secondDate, bool fixDateOrder)
    {
      if (!firstDate.HasValue || !secondDate.HasValue)
        return;
      this._firstDate = firstDate.Value;
      this._secondDate = secondDate.Value;
      this.Calculate();
    }

    protected uint TodaySerialDayNumber
    {
      get
      {
        if (Age._todaySerialDayNumber == 0U)
          Age._todaySerialDayNumber = new SDNDate(SDNDate.Encode(DateTime.Now)).SerialDayNumber;
        return Age._todaySerialDayNumber;
      }
    }

    protected void Calculate()
    {
      int year1;
      int month1;
      int day1;
      SerialDayNumber.SdnToGregorian((int) this._firstDate, out year1, out month1, out day1);
      int year2;
      int month2;
      int day2;
      SerialDayNumber.SdnToGregorian((int) this._secondDate, out year2, out month2, out day2);
      this._years = year2 - year1;
      if (year1 < 0 && year2 > 0)
        --this._years;
      this._months = month2 >= month1 ? month2 - month1 : 12 + month2 - month1;
      if (day2 < day1)
        --this._months;
      if (this._months < 0)
        this._months += 12;
      if (month2 < month1 || month2 == month1 && day2 < day1)
        --this._years;
      this._days = day2 - day1;
      if (this._days < 0)
      {
        int monthDay = Age._monthDays[month2 - 1 < 1 ? 12 : month2 - 1];
        this._days = day2 + monthDay - day1;
        if (day1 > monthDay)
          this._days = day2;
      }
      this._calculated = true;
    }

    protected int MonthDays(int year, int month)
    {
      int monthDay = Age._monthDays[month];
      if (month == 2 && this.IsLeapYear(year))
        ++monthDay;
      return monthDay;
    }

    protected bool IsLeapYear(int year)
    {
      bool flag = false;
      if (year % 4 == 0)
      {
        flag = true;
        if (year % 100 == 0 && year % 400 != 0)
          flag = false;
      }
      return flag;
    }

    public int Years => this._years;

    public int Months => this._months;

    public int Days => this._days;

    public int InDays => (int) this._secondDate - (int) this._firstDate;

    public override string ToString() => this._formatter.Format((string) null, (object) this, (IFormatProvider) null);

    public string ToString(string format) => this._formatter.Format(format, (object) this, (IFormatProvider) null);

    public string ToString(IFormatProvider formatProvider) => this._formatter.Format((string) null, (object) this, formatProvider);

    public FormatInfo Formatter
    {
      get => this._formatter;
      set => this._formatter = value;
    }

    public string ToString(string format, IFormatProvider formatProvider) => this._formatter.Format(format, (object) this, formatProvider);
  }
}
