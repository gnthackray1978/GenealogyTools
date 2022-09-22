// Decompiled with JetBrains decompiler
// Type: MyFamily.Shared.DateFormatInfo
// Assembly: MyFamily.Shared, Version=23.3.0.1570, Culture=neutral, PublicKeyToken=295e8b5bb13c2421
// MVID: CB662BCB-E11E-4698-B7B4-24BC7C8B503C
// Assembly location: C:\Users\GeorgePickworth\Documents\development\GenealogyTools\FTMContextNet\MyFamily.Shared.dll

using FTM.Dates.Interfaces;
using FTM.Dates.Properties;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;

namespace FTM.Dates
{
  public class DateFormatInfo : FormatInfo
  {
    private CultureInfo _culture;
    private char[] _delimiters = new char[4]
    {
      ',',
      '/',
      '-',
      '.'
    };
    protected static string _BC;
    protected static string _AD;
    protected static int _ADCutoffYear = 999;
    protected static string _fuzzinessText;
    protected bool _suppressModifier;
    public const string DMonY = "DMY";
    public const string dMonY = "dMY";
    public const string DMonthY = "LDMY";
    public const string MonDY = "MDY";
    public const string MonDDY = "MDDY";
    public const string MonthDY = "LMDY";
    public const string MonthDDY = "LMDDY";
    public const string Year = "yyyy";
    public const string Month = "m";
    public const string LZMonth = "mm";
    public const string MonStr = "Mmm";
    public const string MonthStr = "Mmmm";
    public const string Day = "d";
    public const string LZDay = "dd";
    public const string Modifier = "mod";
    public const string Standard = "DMY";
    public static readonly DateFormatInfo Default = new DateFormatInfo();

    public int ADCutoffYear
    {
      get => DateFormatInfo._ADCutoffYear;
      set => DateFormatInfo._ADCutoffYear = value;
    }

    public bool CommonEra
    {
      set
      {
        DateParser.CommonEra = value;
        DateFormatInfo._BC = DateParser.CommonEra ? Resources.DateBCE : Resources.DateBC;
        DateFormatInfo._AD = DateParser.CommonEra ? Resources.DateCE : Resources.DateAD;
      }
    }

    public string FuzzinessText
    {
      get => DateFormatInfo._fuzzinessText;
      set => DateFormatInfo._fuzzinessText = value;
    }

    public CultureInfo Culture
    {
      get => this._culture;
      set
      {
        this._culture = value;
        Resources.Culture = this._culture;
      }
    }

    public bool SuppressModifier
    {
      get => this._suppressModifier;
      set => this._suppressModifier = value;
    }

    public DateFormatInfo() => this.Initialize(Thread.CurrentThread.CurrentUICulture);

    public DateFormatInfo(CultureInfo culture) => this.Initialize(culture);

    public DateFormatInfo(string defaultFormat)
      : base(defaultFormat)
    {
      this.Initialize(Thread.CurrentThread.CurrentUICulture);
    }

    private void Initialize(CultureInfo culture)
    {
      this.Culture = culture;
      this.CommonEra = false;
    }

    protected override string GetCacheKey(object obj)
    {
      switch (obj)
      {
        case IRange<Date> _:
          IRange<Date> range = (IRange<Date>) obj;
          return string.Format("{0}_{1}{2}", (object) this._culture.LCID, (object) Date.Encode(range.Begin), (object) Date.Encode(range.End));
        case TextDate _:
          return ((TextDate) obj).Text;
        case Date _:
          return string.Format("{0}_{1}", (object) this._culture.LCID, (object) Date.Encode((Date) obj).ToString());
        default:
          return base.GetCacheKey(obj);
      }
    }

    protected override string InnerFormat(
      string format,
      object arg,
      IFormatProvider formatProvider)
    {
      switch (arg)
      {
        case TextDate _:
          return ((TextDate) arg).Text;
        case KeywordDate _:
          return DateFormatInfo.FormatKeyword(((KeywordDate) arg).Keyword);
        case Date _:
          return this.FormatDate(format, (Date) arg, formatProvider);
        default:
          return base.InnerFormat(format, arg, formatProvider);
      }
    }

    private string FormatDate(string format, Date date, IFormatProvider formatProvider)
    {
      formatProvider = formatProvider ?? (IFormatProvider) this;
      int index = format.IndexOfAny(this._delimiters);
      char delimiter = ' ';
      if (index >= 0)
        delimiter = format[index];
      if (date is DateRange && format == "yyyy")
        date = (Date) ((DateRange) date).Begin;
      if (date is DateRange)
      {
        DateRange dateRange = (DateRange) date;
        int? year1 = dateRange.Begin.Year;
        int? year2 = dateRange.End.Year;
        int? month1 = dateRange.Begin.Month;
        int? month2 = dateRange.End.Month;
        int? day1 = dateRange.Begin.Day;
        int? day2 = dateRange.End.Day;
        DateModifier modifier1 = dateRange.Begin.Modifier;
        DateModifier modifier2 = dateRange.End.Modifier;
        if ((modifier1 & DateModifier.DoubleDate) == DateModifier.None)
        {
          int? nullable1 = year1;
          int? nullable2 = year2;
          if (nullable1.GetValueOrDefault() == nullable2.GetValueOrDefault() & nullable1.HasValue == nullable2.HasValue)
          {
            year1 = new int?();
            modifier1 &= ~DateModifier.YearMissing;
            int? nullable3 = month1;
            int? nullable4 = month2;
            if (nullable3.GetValueOrDefault() == nullable4.GetValueOrDefault() & nullable3.HasValue == nullable4.HasValue)
            {
              string str = format.Replace("mod", "");
              int num1 = str.IndexOf("m");
              int num2 = str.IndexOf("d");
              if (format == "MDY" || format == "MDDY" || format == "LMDY" || format == "LMDDY" || num1 < num2)
              {
                month2 = new int?();
                modifier2 &= ~DateModifier.MonthMissing;
              }
              else
              {
                month1 = new int?();
                modifier1 &= ~DateModifier.MonthMissing;
              }
            }
          }
        }
        Date date1 = (Date) new SDNDate(SDNDate.Encode(year1, month1, day1, modifier1));
        Date date2 = (Date) new SDNDate(SDNDate.Encode(year2, month2, day2, modifier2));
        string format1 = string.Format(!this._suppressModifier ? Resources.DateRangeFormat : "{{0:{0}}}–{{1:{0}}}", (object) format);
        return string.Format(formatProvider, format1, (object) date1, (object) date2);
      }
      int? nullable = date.Year;
      if (!nullable.HasValue)
      {
        nullable = date.Month;
        if (!nullable.HasValue)
        {
          nullable = date.Day;
          if (!nullable.HasValue)
            return string.Empty;
        }
      }
      string empty = string.Empty;
      string s1;
      switch (format)
      {
        case "DMY":
        case "G":
          s1 = string.Format("{0} {1} {2} {3}", (object) this.FormatModifier(date.Modifier), (object) this.FormatDay((IDate) date, "d2"), (object) this.FormatMonthStrAbbrev((IDate) date, this._culture), (object) this.FormatFullYear((IDate) date));
          break;
        case "LDMY":
          s1 = string.Format("{0} {1} {2} {3}", (object) this.FormatModifier(date.Modifier), (object) this.FormatDay((IDate) date, "d2"), (object) this.FormatMonthStr((IDate) date, this._culture), (object) this.FormatFullYear((IDate) date));
          break;
        case "LMDDY":
          s1 = !date.HasMonth() || !date.HasDay() || !date.HasYear() ? string.Format(formatProvider, "{0:mod Mmmm dd yyyy}", (object) date) : string.Format(formatProvider, "{0:mod Mmmm dd, yyyy}", (object) date);
          break;
        case "LMDY":
          s1 = !date.HasMonth() || !date.HasDay() || !date.HasYear() ? string.Format(formatProvider, "{0:mod Mmmm d yyyy}", (object) date) : string.Format(formatProvider, "{0:mod Mmmm d, yyyy}", (object) date);
          break;
        case "MDDY":
          s1 = !date.HasMonth() || !date.HasDay() || !date.HasYear() ? string.Format(formatProvider, "{0:mod Mmm dd yyyy}", (object) date) : string.Format(formatProvider, "{0:mod Mmm dd, yyyy}", (object) date);
          break;
        case "MDY":
          s1 = !date.HasMonth() || !date.HasDay() || !date.HasYear() ? string.Format(formatProvider, "{0:mod Mmm d yyyy}", (object) date) : string.Format(formatProvider, "{0:mod Mmm d, yyyy}", (object) date);
          break;
        case "dMY":
          s1 = string.Format("{0} {1} {2} {3}", (object) this.FormatModifier(date.Modifier), (object) this.FormatDay((IDate) date, "d"), (object) this.FormatMonthStrAbbrev((IDate) date, this._culture), (object) this.FormatFullYear((IDate) date));
          break;
        default:
          s1 = base.InnerFormat(format, (object) date, formatProvider);
          break;
      }
      string s2 = this.CollapseWhitespace(s1);
      return this.CollapseDelimiters(delimiter, s2).TrimEnd(delimiter);
    }

    private string FormatModifier(DateModifier modifier)
    {
      if (this._suppressModifier)
        return string.Empty;
      switch (modifier & (DateModifier.About | DateModifier.Calculated))
      {
        case DateModifier.None:
          return string.Empty;
        case DateModifier.Before:
          return Resources.DateModifierBefore;
        case DateModifier.After:
          return Resources.DateModifierAfter;
        case DateModifier.About:
          return DateFormatInfo._fuzzinessText ?? Resources.DateModifierAbout;
        case DateModifier.Calculated:
          return Resources.DateModifierCalculated;
        default:
          return string.Empty;
      }
    }

    private static string FormatKeyword(DateKeyword keyword)
    {
      switch (keyword)
      {
        case DateKeyword.BIC:
          return Resources.DateKeywordBIC;
        case DateKeyword.Cancelled:
          return Resources.DateKeywordCancelled;
        case DateKeyword.Child:
          return Resources.DateKeywordChild;
        case DateKeyword.Cleared:
          return Resources.DateKeywordCleared;
        case DateKeyword.Completed:
          return Resources.DateKeywordCompleted;
        case DateKeyword.Dead:
          return Resources.DateKeywordDead;
        case DateKeyword.Deceased:
          return Resources.DateKeywordDeceased;
        case DateKeyword.Done:
          return Resources.DateKeywordDone;
        case DateKeyword.DNS:
          return Resources.DateKeywordDNS;
        case DateKeyword.DNS_CAN:
          return Resources.DateKeywordDNSCAN;
        case DateKeyword.Infant:
          return Resources.DateKeywordInfant;
        case DateKeyword.NeverMarried:
          return Resources.DateKeywordNeverMarried;
        case DateKeyword.Pre_1970:
          return Resources.DateKeywordPre1970;
        case DateKeyword.Stillborn:
          return Resources.DateKeywordStillborn;
        case DateKeyword.Submitted:
          return Resources.DateKeywordSubmitted;
        case DateKeyword.Uncleared:
          return Resources.DateKeywordUncleared;
        case DateKeyword.Young:
          return Resources.DateKeywordYoung;
        case DateKeyword.Unknown:
          return Resources.DateKeywordUnknown;
        case DateKeyword.Private:
          return Resources.DateKeywordPrivate;
        case DateKeyword.NotMarried:
          return Resources.DateKeywordNotMarried;
        default:
          return Resources.DateKeywordUnknown;
      }
    }

    private string FormatDay(IDate date, string format) => date.Day.HasValue && (date.Modifier & DateModifier.DayMissing) != DateModifier.DayMissing ? date.Day.Value.ToString(format) : string.Empty;

    private string FormatMonthStrAbbrev(IDate date, CultureInfo culture)
    {
      if (date.Month.HasValue)
      {
        int? month = date.Month;
        int num = 0;
        if (month.GetValueOrDefault() > num & month.HasValue && (date.Modifier & DateModifier.MonthMissing) != DateModifier.MonthMissing)
        {
          string[] abbreviatedMonthNames = culture.DateTimeFormat.AbbreviatedMonthNames;
          month = date.Month;
          int index = (int) (byte) month.Value - 1;
          string str = abbreviatedMonthNames[index];
          if ((date.Modifier & DateModifier.Quarter) == DateModifier.Quarter)
            str += " Qtr";
          return str;
        }
      }
      return string.Empty;
    }

    private string FormatMonthStr(IDate date, CultureInfo culture)
    {
      if (!date.Month.HasValue || (date.Modifier & DateModifier.MonthMissing) == DateModifier.MonthMissing)
        return string.Empty;
      string monthName = culture.DateTimeFormat.MonthNames[(int) (byte) date.Month.Value - 1];
      if ((date.Modifier & DateModifier.Quarter) == DateModifier.Quarter)
        monthName += " Qtr";
      return monthName;
    }

    private string FormatMonth(IDate date, string format)
    {
      if (!date.Month.HasValue || (date.Modifier & DateModifier.MonthMissing) == DateModifier.MonthMissing)
        return string.Empty;
      string str = date.Month.Value.ToString(format);
      if ((date.Modifier & DateModifier.Quarter) == DateModifier.Quarter)
        str += " Qtr";
      return str;
    }

    private string FormatFullYear(IDate date)
    {
      string str1 = string.Empty;
      if (date.Year.HasValue && (date.Modifier & DateModifier.YearMissing) != DateModifier.YearMissing)
      {
        if ((date.Modifier & DateModifier.DoubleDate) == DateModifier.DoubleDate)
        {
          int num1 = Math.Abs(date.Year.Value);
          int num2 = num1 - 1;
          string str2 = num1 > 9 ? num1.ToString("d2") : num1.ToString();
          if (str2.Length > 2)
            str2 = str2.Substring(str2.Length - 2);
          // ISSUE: variable of a boxed type
         // __Boxed<int> local = (ValueType) num2;
          string str3 = str2;
          int? year = date.Year;
          int num3 = 0;
          string str4;
          if (!(year.GetValueOrDefault() < num3 & year.HasValue))
          {
            year = date.Year;
            int adCutoffYear = DateFormatInfo._ADCutoffYear;
            str4 = year.GetValueOrDefault() <= adCutoffYear & year.HasValue ? DateFormatInfo._AD : string.Empty;
          }
          else
            str4 = DateFormatInfo._BC;
          str1 = string.Format("{0}/{1} {2}", (object)num2, (object) str3, (object) str4).Trim();
        }
        else
        {
          // ISSUE: variable of a boxed type
          var local = Math.Abs(date.Year.Value);
          int? year = date.Year;
          int num = 0;
          string str5;
          if (!(year.GetValueOrDefault() < num & year.HasValue))
          {
            year = date.Year;
            int adCutoffYear = DateFormatInfo._ADCutoffYear;
            str5 = year.GetValueOrDefault() <= adCutoffYear & year.HasValue ? DateFormatInfo._AD : string.Empty;
          }
          else
            str5 = DateFormatInfo._BC;
          str1 = string.Format("{0} {1}", (object) local, (object) str5).Trim();
        }
      }
      return str1;
    }

    protected override FormatInfo.CustomFormatter GetCustomFormatter(object arg) => (FormatInfo.CustomFormatter) new DateFormatInfo.MyCustomFormatter((IDate) arg, this);

    private string CollapseDelimiters(char delimiter, string s)
    {
      string pattern = string.Format("([^\\s\\{0}]*)(\\{0})+", (object) delimiter);
      return Regex.Replace(s, pattern, new MatchEvaluator(this.EvaluateDelimiterReplacement)).Trim();
    }

    private string EvaluateDelimiterReplacement(Match match) => match.Groups[1].Value == string.Empty ? string.Empty : string.Format("{0}{1}", (object) match.Groups[1], (object) match.Groups[2]);

    private class MyCustomFormatter : FormatInfo.CustomFormatter
    {
      protected IDate _date;
      protected DateFormatInfo _outer;

      public MyCustomFormatter(IDate date, DateFormatInfo outer)
      {
        this._outer = outer;
        this._date = date;
      }

      public override string ReplaceTerm(string term)
      {
        switch (term)
        {
          case "Mmm":
            return this._outer.FormatMonthStrAbbrev(this._date, this._outer._culture);
          case "Mmmm":
            return this._outer.FormatMonthStr(this._date, this._outer._culture);
          case "d":
            return this._outer.FormatDay(this._date, "d");
          case "dd":
            return this._outer.FormatDay(this._date, "d2");
          case "m":
            return this._outer.FormatMonth(this._date, "d");
          case "mm":
            return this._outer.FormatMonth(this._date, "d2");
          case "mod":
            return this._outer.FormatModifier(this._date.Modifier);
          case "yyyy":
            return this._outer.FormatFullYear(this._date);
          default:
            return base.ReplaceTerm(term);
        }
      }
    }
  }
}
