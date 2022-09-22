// Decompiled with JetBrains decompiler
// Type: MyFamily.Shared.DateAuthority
// Assembly: MyFamily.Shared, Version=23.3.0.1570, Culture=neutral, PublicKeyToken=295e8b5bb13c2421
// MVID: CB662BCB-E11E-4698-B7B4-24BC7C8B503C
// Assembly location: C:\Users\GeorgePickworth\Documents\development\GenealogyTools\FTMContextNet\MyFamily.Shared.dll

using FTM.Dates.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace FTM.Dates
{
  public class DateAuthority
  {
    private static DateAuthority.DoubleDate _doubleDateTreatment;
    private static DateAuthority.Century _centuryTreatment;
    private static readonly DateParser[] Parsers = DateAuthority.SuppordedCultures().Select<CultureInfo, DateParser>((Func<CultureInfo, DateParser>) (cultureInfo => new DateParser(cultureInfo))).ToArray<DateParser>();

    public event EventHandler<DateParseEventArgs> ParseFailure;

    public static bool DayBeforeMonth
    {
      get => DateParser.DayBeforeMonth;
      set => DateParser.DayBeforeMonth = value;
    }

    public static DateAuthority.DoubleDate DoubleDateTreatment
    {
      get => DateAuthority._doubleDateTreatment;
      set
      {
        DateAuthority._doubleDateTreatment = value;
        if (DateAuthority._doubleDateTreatment == DateAuthority.DoubleDate.PromptMe)
          DateParser.AutoCreateDoubleDates = new bool?();
        else
          DateParser.AutoCreateDoubleDates = new bool?(DateAuthority._doubleDateTreatment == DateAuthority.DoubleDate.UseDoubleDate);
      }
    }

    public static int DoubleDateCutoffYear
    {
      get => DateParser.DoubleDateCutoffYear;
      set => DateParser.DoubleDateCutoffYear = value;
    }

    public static DateAuthority.Century CenturyTreatment
    {
      get => DateAuthority._centuryTreatment;
      set
      {
        DateAuthority._centuryTreatment = value;
        if (DateAuthority._centuryTreatment == DateAuthority.Century.PromptMe)
          DateParser.TwoDigitYearAsCurrentCentury = new bool?();
        else
          DateParser.TwoDigitYearAsCurrentCentury = new bool?(DateAuthority._centuryTreatment == DateAuthority.Century.ChangeToMostRecent);
      }
    }

    public static bool AncientDateTypeWarning
    {
      get => DateParser.AncientDateTypeWarning;
      set => DateParser.AncientDateTypeWarning = value;
    }

    private static IEnumerable<CultureInfo> SuppordedCultures()
    {
      HashSet<string> usedCultures = new HashSet<string>()
      {
        CultureInfo.InvariantCulture.Name,
        CultureInfo.CurrentCulture.Name,
        CultureInfo.CurrentUICulture.Name
      };
      yield return CultureInfo.InvariantCulture;
      yield return CultureInfo.CurrentCulture;
      foreach (string name in new List<string>()
      {
        "en-US",
        "en-GB",
        "en-CA",
        "fr-CA",
        "en-AU",
        "de-DE",
        "it-IT",
        "fr-FR",
        "sv-SE",
        "es-MX"
      }.ToList<string>())
      {
        if (!usedCultures.Contains(name))
        {
          CultureInfo cultureInfo = (CultureInfo) null;
          try
          {
            cultureInfo = new CultureInfo(name);
            usedCultures.Add(cultureInfo.Name);
          }
          catch
          {
          }
          yield return cultureInfo;
        }
      }
      yield return CultureInfo.CurrentUICulture;
    }

    public IDate ParseDate(string dateStr)
    {
      uint? date1;
      uint? date2;
      this.ParseDate(dateStr, out date1, out date2);
      return (IDate) Date.CreateInstance(date1, date2);
    }

    protected void OnParseFailure(DateParseEventArgs e)
    {
      if (this.ParseFailure == null)
        return;
      this.ParseFailure((object) this, e);
    }

    [DebuggerStepThrough]
    public void ParseDate(string dateStr, out uint? local1, out uint? local2)
    {
     // ref uint? local1 = ref date1;
      //ref uint? local2 = ref date2;
      uint? nullable1 = new uint?();
      uint? nullable2;
      uint? nullable3 = nullable2 = nullable1;
      local2 = nullable2;
      uint? nullable4 = nullable3;
      local1 = nullable4;
      try
      {
        for (int index = 0; index < DateAuthority.Parsers.Length; ++index)
        {
          DateParser parser = DateAuthority.Parsers[index];
          try
          {
            parser.Parse(dateStr, out local1, out local2, out DateModifier _);
            break;
          }
          catch (DateParseException ex)
          {
            if (index != DateAuthority.Parsers.Length - 1)
            {
              if (ex.Error.Severity == DateParseError.SeverityLevel.Critical)
                continue;
            }
            throw;
          }
        }
      }
      catch (DateParseException ex)
      {
        if (this.ParseFailure == null)
          throw;
        else
          this.OnParseFailure(new DateParseEventArgs(ex.Error));
      }
      catch (Exception ex)
      {
        List<DateParseErrorInfo> errorInfo = new List<DateParseErrorInfo>();
        errorInfo.Add(new DateParseErrorInfo(DateParseErrorType.Unknown, "", local1.HasValue));
        DateParseError error = new DateParseError((IList<DateParseErrorInfo>) errorInfo);
        if (this.ParseFailure == null)
          throw new DateParseException(error, "Unknown Error", ex);
        this.OnParseFailure(new DateParseEventArgs(error));
      }
    }

    public enum DoubleDate
    {
      PromptMe,
      LeaveAsItIs,
      UseDoubleDate,
    }

    public enum Century
    {
      PromptMe,
      ChangeToFirstCentury,
      ChangeToMostRecent,
    }
  }
}
