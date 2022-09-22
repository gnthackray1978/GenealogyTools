// Decompiled with JetBrains decompiler
// Type: MyFamily.Shared.DateParser
// Assembly: MyFamily.Shared, Version=23.3.0.1570, Culture=neutral, PublicKeyToken=295e8b5bb13c2421
// MVID: CB662BCB-E11E-4698-B7B4-24BC7C8B503C
// Assembly location: C:\Users\GeorgePickworth\Documents\development\GenealogyTools\FTMContextNet\MyFamily.Shared.dll

using FTM.Dates.Interfaces;
using FTM.Dates.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace FTM.Dates
{
  internal class DateParser
  {
    private static Dictionary<string, Regex> s_parsers;
    private CultureInfo _culture;
    private RegExParser[] _parsers;
    private Regex[] _specialParsers;
    private const int PARSER01 = 0;
    private const int ILLEGAL1 = 1;
    private const int ILLEGAL2 = 2;
    private const int RANGE1 = 3;
    private const int RANGE2 = 4;
    public static bool DayBeforeMonth;
    public static int DoubleDateCutoffYear = 1753;
    public static bool? TwoDigitYearAsCurrentCentury = new bool?();
    public static bool? AutoCreateDoubleDates = new bool?();
    public static bool AncientDateTypeWarning = true;
    private static bool _commonEra = false;
    protected static uint today;
    private Dictionary<string, DateModifier> WordsToQualifier;
    private string[,] Months;
    private static int[] MonthMaxDays = new int[12]
    {
      31,
      29,
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
    private static int _MAX_LDS_KEYWORDS = 4;
    private static string[,] _LdsKeywords = new string[20, 4]
    {
      {
        Resources.DateKeywordBIC,
        "bic",
        null,
        null
      },
      {
        Resources.DateKeywordCancelled,
        "cancelled",
        "can",
        "canceled"
      },
      {
        Resources.DateKeywordChild,
        "child",
        "chi",
        "infant"
      },
      {
        Resources.DateKeywordCleared,
        "cleared",
        "cle",
        null
      },
      {
        Resources.DateKeywordCompleted,
        "completed",
        "com",
        null
      },
      {
        Resources.DateKeywordDead,
        "dead",
        null,
        null
      },
      {
        Resources.DateKeywordDeceased,
        "deceased",
        null,
        null
      },
      {
        Resources.DateKeywordDone,
        "done",
        null,
        null
      },
      {
        Resources.DateKeywordDNS,
        "dns",
        null,
        null
      },
      {
        Resources.DateKeywordDNSCAN,
        "dns/can",
        null,
        null
      },
      {
        Resources.DateKeywordInfant,
        "infant",
        "inf",
        null
      },
      {
        Resources.DateKeywordNeverMarried,
        "never married",
        null,
        null
      },
      {
        Resources.DateKeywordPre1970,
        "pre-1970",
        null,
        null
      },
      {
        Resources.DateKeywordStillborn,
        "stillborn",
        "sti",
        null
      },
      {
        Resources.DateKeywordSubmitted,
        "submitted",
        "sub",
        null
      },
      {
        Resources.DateKeywordUncleared,
        "uncleared",
        "unc",
        null
      },
      {
        Resources.DateKeywordYoung,
        "young",
        null,
        null
      },
      {
        Resources.DateKeywordUnknown,
        "unknown",
        "?",
        "unk"
      },
      {
        Resources.DateKeywordPrivate,
        "private",
        null,
        null
      },
      {
        Resources.DateKeywordNotMarried,
        "not married",
        null,
        null
      }
    };
    private DateModifier _Qualifier;
    private static readonly string DayEx;
    private static readonly string MonthEx;
    private static readonly string YearEx;
    private static readonly string DoubleYearEx;
    private static readonly string TwoDigitDoubleYearEx;
    private static readonly string OneDigitDoubleYearEx;
    private static readonly string TwoYearEx;
    private static readonly string AnyYearEx;
    private static readonly string MonthStrEx;
    private string BcRegex;
    private string BceRegex;
    private string AdRegex;
    private string CeRegex;
    private string BcAdEx;
    private string Quarter;
    private uint? _dateFirst;
    private uint? _dateSecond;
    private string _orgEntry;
    private IList<DateParseErrorInfo> _dateParseErrorInfo;
    private static int currentYear = DateTime.Now.Year;
    private static bool _ignoreDoubleDates;

    static DateParser()
    {
      DateParser.s_parsers = new Dictionary<string, Regex>();
      DateParser.s_parsers.Add("special0", new Regex("([0-9]+)/([0-9]+)/([0-9]+) - 0/([0-9]+)/0", RegexOptions.Compiled));
      DateParser.s_parsers.Add("special1", new Regex("^ *([0-9]+) *- *([0-9]+)([ -]+\\p{L}{3}[ -]+[0-9]+) *$", RegexOptions.Compiled));
      DateParser.s_parsers.Add("special2", new Regex("Or:([0-9][0-9][0-9]*)/([0-9][0-9])/([0-9][0-9])\\-([0-9][0-9][0-9]*)/00/00", RegexOptions.Compiled));
      DateParser.s_parsers.Add("standard", new Regex("([0-9]+) (\\p{L}+) ([0-9]+)", RegexOptions.Compiled));
      DateParser.DayEx = "([0-9][0-9]?)";
      DateParser.MonthEx = "([0-9][0-9]?)";
      DateParser.YearEx = "([0-9]{3,8})";
      DateParser.DoubleYearEx = "([0-9][0-9][0-9]+ */ *[0-9]+)";
      DateParser.TwoDigitDoubleYearEx = "([0-9][0-9] */ *[0-9]+)";
      DateParser.OneDigitDoubleYearEx = "([0-9] */ *[0-9]+)";
      DateParser.TwoYearEx = "([0-9][0-9]?)";
      DateParser.AnyYearEx = "([0-9]+)";
      DateParser.MonthStrEx = "(\\p{L}{3,})";
    }

    internal DateParser(CultureInfo culture)
    {
      this._culture = culture;
      this.Months = new string[12, 3]
      {
        {
          this._culture.DateTimeFormat.MonthNames[0].ToLower(),
          this._culture.DateTimeFormat.AbbreviatedMonthNames[0].ToLower(),
          null
        },
        {
          this._culture.DateTimeFormat.MonthNames[1].ToLower(),
          this._culture.DateTimeFormat.AbbreviatedMonthNames[1].ToLower(),
          null
        },
        {
          this._culture.DateTimeFormat.MonthNames[2].ToLower(),
          this._culture.DateTimeFormat.AbbreviatedMonthNames[2].ToLower(),
          null
        },
        {
          this._culture.DateTimeFormat.MonthNames[3].ToLower(),
          this._culture.DateTimeFormat.AbbreviatedMonthNames[3].ToLower(),
          null
        },
        {
          this._culture.DateTimeFormat.MonthNames[4].ToLower(),
          this._culture.DateTimeFormat.AbbreviatedMonthNames[4].ToLower(),
          null
        },
        {
          this._culture.DateTimeFormat.MonthNames[5].ToLower(),
          this._culture.DateTimeFormat.AbbreviatedMonthNames[5].ToLower(),
          null
        },
        {
          this._culture.DateTimeFormat.MonthNames[6].ToLower(),
          this._culture.DateTimeFormat.AbbreviatedMonthNames[6].ToLower(),
          null
        },
        {
          this._culture.DateTimeFormat.MonthNames[7].ToLower(),
          this._culture.DateTimeFormat.AbbreviatedMonthNames[7].ToLower(),
          null
        },
        {
          this._culture.DateTimeFormat.MonthNames[8].ToLower(),
          this._culture.DateTimeFormat.AbbreviatedMonthNames[8].ToLower(),
          "sept"
        },
        {
          this._culture.DateTimeFormat.MonthNames[9].ToLower(),
          this._culture.DateTimeFormat.AbbreviatedMonthNames[9].ToLower(),
          null
        },
        {
          this._culture.DateTimeFormat.MonthNames[10].ToLower(),
          this._culture.DateTimeFormat.AbbreviatedMonthNames[10].ToLower(),
          null
        },
        {
          this._culture.DateTimeFormat.MonthNames[11].ToLower(),
          this._culture.DateTimeFormat.AbbreviatedMonthNames[11].ToLower(),
          null
        }
      };
      this.BcRegex = DateParser.CreatePunctuatedRegExFromResource(DateParserStrings.ResourceManager.GetString("BC", this._culture), false);
      this.BceRegex = DateParser.CreatePunctuatedRegExFromResource(DateParserStrings.ResourceManager.GetString("BCE", this._culture), false);
      this.AdRegex = DateParser.CreatePunctuatedRegExFromResource(DateParserStrings.ResourceManager.GetString("AD", this._culture), false);
      this.CeRegex = DateParser.CreatePunctuatedRegExFromResource(DateParserStrings.ResourceManager.GetString("CE", this._culture), false);
      this.BcAdEx = " *(" + this.BceRegex + "|" + this.BcRegex + "|" + this.AdRegex + "|" + this.CeRegex + ")?";
      this.Quarter = "(" + DateParser.CreateRegExFromResource(DateParserStrings.ResourceManager.GetString(nameof (Quarter), this._culture)) + ")";
      this.InitializeWordsToQualifier();
      RegExParser[] regExParserArray = new RegExParser[44];
      RegExParser regExParser1 = new RegExParser();
      regExParser1.Expression = new Regex("^" + DateParser.YearEx + this.BcAdEx + "$", RegexOptions.Compiled);
      RegExParser regExParser2 = regExParser1;
      int[] numArray1 = new int[6];
      numArray1[2] = 1;
      regExParser2.Indices = numArray1;
      regExParserArray[0] = regExParser1;
      regExParserArray[1] = new RegExParser()
      {
        Expression = new Regex("\\b" + DateParser.MonthEx + "[-/\\., ] *" + DateParser.DayEx + "[-/\\., ] *" + DateParser.DoubleYearEx + this.BcAdEx + "\\b", RegexOptions.Compiled),
        Indices = new int[6]{ 2, 1, 3, 0, 1, 0 }
      };
      regExParserArray[2] = new RegExParser()
      {
        Expression = new Regex("\\b" + DateParser.MonthEx + "[-/\\., ] *" + DateParser.DayEx + "[-/\\., ] *" + DateParser.YearEx + "[-]([0-9]+)" + this.BcAdEx + "\\b", RegexOptions.Compiled),
        Indices = new int[6]{ 2, 1, 3, 4, 1, 0 }
      };
      regExParserArray[3] = new RegExParser()
      {
        Expression = new Regex("\\b" + DateParser.MonthEx + "\\.?[-/\\., ] *" + DateParser.DayEx + "\\.?[-/\\., ] *" + DateParser.YearEx + this.BcAdEx + "\\b", RegexOptions.Compiled),
        Indices = new int[6]{ 2, 1, 3, 0, 1, 0 }
      };
      regExParserArray[4] = new RegExParser()
      {
        Expression = new Regex("\\b" + DateParser.MonthStrEx + "[-/\\., ]*" + DateParser.DayEx + "[-/\\., ]+" + DateParser.TwoDigitDoubleYearEx + this.BcAdEx + "\\b", RegexOptions.Compiled),
        Indices = new int[6]{ 2, 1, 3, 0, 0, 0 }
      };
      regExParserArray[5] = new RegExParser()
      {
        Expression = new Regex("\\b" + DateParser.MonthStrEx + "[-/\\., ]*" + DateParser.DayEx + "[-/\\., ]+" + DateParser.OneDigitDoubleYearEx + this.BcAdEx + "\\b", RegexOptions.Compiled),
        Indices = new int[6]{ 2, 1, 3, 0, 0, 0 }
      };
      regExParserArray[6] = new RegExParser()
      {
        Expression = new Regex("\\b" + DateParser.MonthEx + "[-/\\., ] *" + DateParser.DayEx + "[-/\\., ] *" + DateParser.AnyYearEx + this.BcAdEx + "\\b", RegexOptions.Compiled),
        Indices = new int[6]{ 2, 1, 3, 0, 1, 0 }
      };
      regExParserArray[7] = new RegExParser()
      {
        Expression = new Regex("\\b" + DateParser.MonthEx + "[-/\\., ] *" + DateParser.DayEx + "[-/\\., ] *" + DateParser.TwoYearEx + "[-]([0-9]+)" + this.BcAdEx + "\\b", RegexOptions.Compiled),
        Indices = new int[6]{ 2, 1, 3, 4, 1, 0 }
      };
      regExParserArray[8] = new RegExParser()
      {
        Expression = new Regex("\\b" + DateParser.MonthEx + "[-/\\., ] *" + DateParser.DayEx + "[-/\\., ] *" + DateParser.TwoYearEx + this.BcAdEx + "\\b", RegexOptions.Compiled),
        Indices = new int[6]{ 2, 1, 3, 0, 1, 0 }
      };
      regExParserArray[9] = new RegExParser()
      {
        Expression = new Regex("\\b" + DateParser.YearEx + "[-/\\., ] *" + DateParser.MonthEx + "[-/\\., ] *" + DateParser.DayEx + this.BcAdEx + "\\b", RegexOptions.Compiled),
        Indices = new int[6]{ 3, 2, 1, 0, 1, 0 }
      };
      regExParserArray[10] = new RegExParser()
      {
        Expression = new Regex("\\b" + DateParser.YearEx + "[-\\., ] *" + DateParser.MonthEx + this.BcAdEx + "\\b", RegexOptions.Compiled),
        Indices = new int[6]{ 0, 2, 1, 0, 1, 0 }
      };
      regExParserArray[11] = new RegExParser()
      {
        Expression = new Regex("\\b" + DateParser.YearEx + "[-/\\., ]*" + DateParser.MonthStrEx + "[-/\\., ]*" + DateParser.DayEx + this.BcAdEx + "\\b", RegexOptions.Compiled),
        Indices = new int[6]{ 3, 2, 1, 0, 0, 0 }
      };
      regExParserArray[12] = new RegExParser()
      {
        Expression = new Regex("\\b" + DateParser.DayEx + "[-/\\., ]*" + DateParser.MonthStrEx + "[-/\\., ]*" + DateParser.DoubleYearEx + this.BcAdEx + "\\b", RegexOptions.Compiled),
        Indices = new int[6]{ 1, 2, 3, 0, 0, 0 }
      };
      regExParserArray[13] = new RegExParser()
      {
        Expression = new Regex("\\b" + DateParser.DayEx + "[-/\\., ]*" + DateParser.MonthStrEx + "[-/\\., ]*" + DateParser.YearEx + "[-]([0-9]+)" + this.BcAdEx + "\\b", RegexOptions.Compiled),
        Indices = new int[6]{ 1, 2, 3, 4, 0, 0 }
      };
      regExParserArray[14] = new RegExParser()
      {
        Expression = new Regex("\\b" + DateParser.DayEx + "[-/\\., ]*" + DateParser.MonthStrEx + "[-/\\., ]*" + DateParser.YearEx + this.BcAdEx + "\\b", RegexOptions.Compiled),
        Indices = new int[6]{ 1, 2, 3, 0, 0, 0 }
      };
      regExParserArray[15] = new RegExParser()
      {
        Expression = new Regex("\\b" + DateParser.DayEx + "[-/\\., ]*" + DateParser.MonthStrEx + "[-/\\., ]*" + DateParser.TwoDigitDoubleYearEx + this.BcAdEx + "\\b", RegexOptions.Compiled),
        Indices = new int[6]{ 1, 2, 3, 0, 0, 0 }
      };
      regExParserArray[16] = new RegExParser()
      {
        Expression = new Regex("\\b" + DateParser.DayEx + "[-/\\., ]*" + DateParser.MonthStrEx + "[-/\\., ]*" + DateParser.OneDigitDoubleYearEx + this.BcAdEx + "\\b", RegexOptions.Compiled),
        Indices = new int[6]{ 1, 2, 3, 0, 0, 0 }
      };
      regExParserArray[17] = new RegExParser()
      {
        Expression = new Regex("\\b" + DateParser.DayEx + "[-/\\., ]*" + DateParser.MonthStrEx + "[-/\\., ]*" + DateParser.TwoYearEx + this.BcAdEx + "\\b", RegexOptions.Compiled),
        Indices = new int[6]{ 1, 2, 3, 0, 0, 0 }
      };
      regExParserArray[18] = new RegExParser()
      {
        Expression = new Regex("\\b" + DateParser.MonthStrEx + "[-/\\., ]*" + DateParser.DayEx + "[-/\\., ]+" + DateParser.DoubleYearEx + this.BcAdEx + "\\b", RegexOptions.Compiled),
        Indices = new int[6]{ 2, 1, 3, 0, 0, 0 }
      };
      regExParserArray[19] = new RegExParser()
      {
        Expression = new Regex("\\b" + DateParser.MonthStrEx + "[-/\\., ]*" + DateParser.DayEx + "[-/\\., ]+" + DateParser.YearEx + "[-]([0-9]+)" + this.BcAdEx + "\\b", RegexOptions.Compiled),
        Indices = new int[6]{ 2, 1, 3, 4, 0, 0 }
      };
      regExParserArray[20] = new RegExParser()
      {
        Expression = new Regex("\\b" + DateParser.MonthStrEx + "[-/\\., ]*" + DateParser.DayEx + "[-/\\., ]+" + DateParser.YearEx + this.BcAdEx + "\\b", RegexOptions.Compiled),
        Indices = new int[6]{ 2, 1, 3, 0, 0, 0 }
      };
      regExParserArray[21] = new RegExParser()
      {
        Expression = new Regex("\\b" + DateParser.MonthStrEx + "[-/\\., ]*" + DateParser.DayEx + "[-/\\., ]+" + DateParser.TwoYearEx + this.BcAdEx + "\\b", RegexOptions.Compiled),
        Indices = new int[6]{ 2, 1, 3, 0, 0, 0 }
      };
      regExParserArray[22] = new RegExParser()
      {
        Expression = new Regex("\\b" + DateParser.MonthStrEx + "[-/\\., ]*" + this.Quarter + "[-/\\., ]*" + DateParser.DoubleYearEx + this.BcAdEx + "\\b", RegexOptions.Compiled),
        Indices = new int[6]{ 0, 1, 3, 4, 0, 2 }
      };
      regExParserArray[23] = new RegExParser()
      {
        Expression = new Regex("\\b" + DateParser.MonthStrEx + "[-/\\., ]*" + DateParser.DoubleYearEx + this.BcAdEx + "\\b", RegexOptions.Compiled),
        Indices = new int[6]{ 0, 1, 2, 0, 0, 0 }
      };
      regExParserArray[24] = new RegExParser()
      {
        Expression = new Regex("\\b" + DateParser.MonthStrEx + "[-/\\., ]*" + this.Quarter + "[-/\\., ]*" + DateParser.YearEx + "[-]([0-9]+)" + this.BcAdEx + "\\b", RegexOptions.Compiled),
        Indices = new int[6]{ 0, 1, 3, 4, 0, 2 }
      };
      regExParserArray[25] = new RegExParser()
      {
        Expression = new Regex("\\b" + DateParser.MonthStrEx + "[-/\\., ]*" + DateParser.YearEx + "[-]([0-9]+)" + this.BcAdEx + "\\b", RegexOptions.Compiled),
        Indices = new int[6]{ 0, 1, 2, 3, 0, 0 }
      };
      regExParserArray[26] = new RegExParser()
      {
        Expression = new Regex("\\b" + DateParser.MonthStrEx + "[-/\\., ]*" + this.Quarter + "[-/\\., ]*" + DateParser.YearEx + this.BcAdEx + "\\b", RegexOptions.Compiled),
        Indices = new int[6]{ 0, 1, 3, 0, 0, 2 }
      };
      regExParserArray[27] = new RegExParser()
      {
        Expression = new Regex("\\b" + DateParser.MonthStrEx + "[-/\\., ]*" + DateParser.YearEx + this.BcAdEx + "\\b", RegexOptions.Compiled),
        Indices = new int[6]{ 0, 1, 2, 0, 0, 0 }
      };
      regExParserArray[28] = new RegExParser()
      {
        Expression = new Regex("\\b" + DateParser.MonthStrEx + "[-/\\., ]*" + this.Quarter + "[-/\\., ]*(3[2-9]|[4-9][0-9])/([0-9]+)" + this.BcAdEx + "\\b", RegexOptions.Compiled),
        Indices = new int[6]{ 0, 1, 3, 4, 0, 2 }
      };
      regExParserArray[29] = new RegExParser()
      {
        Expression = new Regex("\\b" + DateParser.MonthStrEx + "[-/\\., ]*(3[2-9]|[4-9][0-9])/([0-9]+)" + this.BcAdEx + "\\b", RegexOptions.Compiled),
        Indices = new int[6]{ 0, 1, 2, 3, 0, 0 }
      };
      regExParserArray[30] = new RegExParser()
      {
        Expression = new Regex("\\b" + DateParser.MonthStrEx + "[-/\\., ]*" + this.Quarter + "[-/\\., ]*(3[2-9]|[4-9][0-9])" + this.BcAdEx + "\\b", RegexOptions.Compiled),
        Indices = new int[6]{ 0, 1, 3, 0, 0, 2 }
      };
      regExParserArray[31] = new RegExParser()
      {
        Expression = new Regex("\\b" + DateParser.MonthStrEx + "[-/\\., ]*(3[2-9]|[4-9][0-9])" + this.BcAdEx + "\\b", RegexOptions.Compiled),
        Indices = new int[6]{ 0, 1, 2, 0, 0, 0 }
      };
      regExParserArray[32] = new RegExParser()
      {
        Expression = new Regex("\\b" + DateParser.MonthStrEx + "[-/\\., ]*([1-9]|[0-9][0-9]) *(" + this.BceRegex + "|" + this.BcRegex + "|" + this.AdRegex + "|" + this.CeRegex + ")\\b", RegexOptions.Compiled),
        Indices = new int[6]{ 0, 1, 2, 0, 0, 0 }
      };
      regExParserArray[33] = new RegExParser()
      {
        Expression = new Regex("\\b" + DateParser.MonthStrEx + "[-/\\., ]*" + DateParser.DayEx + " *\\b", RegexOptions.Compiled),
        Indices = new int[6]{ 2, 1, 0, 0, 0, 0 }
      };
      regExParserArray[34] = new RegExParser()
      {
        Expression = new Regex("\\b" + DateParser.MonthEx + "[- \\.,]+" + DateParser.DoubleYearEx + this.BcAdEx + "\\b", RegexOptions.Compiled),
        Indices = new int[6]{ 0, 1, 2, 0, 1, 0 }
      };
      RegExParser regExParser3 = new RegExParser();
      regExParser3.Expression = new Regex("\\b" + DateParser.DoubleYearEx + this.BcAdEx, RegexOptions.Compiled);
      RegExParser regExParser4 = regExParser3;
      int[] numArray2 = new int[6];
      numArray2[2] = 1;
      regExParser4.Indices = numArray2;
      regExParserArray[35] = regExParser3;
      regExParserArray[36] = new RegExParser()
      {
        Expression = new Regex("\\b" + DateParser.MonthEx + "[- \\./,]+" + DateParser.AnyYearEx + this.BcAdEx + "\\b", RegexOptions.Compiled),
        Indices = new int[6]{ 0, 1, 2, 0, 1, 0 }
      };
      regExParserArray[37] = new RegExParser()
      {
        Expression = new Regex("\\b" + DateParser.MonthEx + "[- \\./,]+" + DateParser.YearEx + this.BcAdEx + "\\b", RegexOptions.Compiled),
        Indices = new int[6]{ 0, 1, 2, 0, 1, 0 }
      };
      RegExParser regExParser5 = new RegExParser();
      regExParser5.Expression = new Regex("\\b" + DateParser.YearEx + "[-/\\., ]*" + this.BcAdEx + "\\b", RegexOptions.Compiled);
      RegExParser regExParser6 = regExParser5;
      int[] numArray3 = new int[6];
      numArray3[2] = 1;
      regExParser6.Indices = numArray3;
      regExParserArray[38] = regExParser5;
      regExParserArray[39] = new RegExParser()
      {
        Expression = new Regex("\\b" + DateParser.YearEx + "[-/\\., ]*" + DateParser.MonthStrEx + "\\b", RegexOptions.Compiled),
        Indices = new int[6]{ 0, 2, 1, 0, 0, 0 }
      };
      RegExParser regExParser7 = new RegExParser();
      regExParser7.Expression = new Regex("\\b" + DateParser.TwoYearEx + " *(" + this.BceRegex + "|" + this.BcRegex + "|" + this.AdRegex + "|" + this.CeRegex + ")", RegexOptions.Compiled);
      RegExParser regExParser8 = regExParser7;
      int[] numArray4 = new int[6];
      numArray4[2] = 1;
      regExParser8.Indices = numArray4;
      regExParserArray[40] = regExParser7;
      regExParserArray[41] = new RegExParser()
      {
        Expression = new Regex("\\b" + DateParser.DayEx + "[-/\\., ]*" + DateParser.MonthStrEx + "\\b", RegexOptions.Compiled),
        Indices = new int[6]{ 1, 2, 0, 0, 0, 0 }
      };
      RegExParser regExParser9 = new RegExParser();
      regExParser9.Expression = new Regex("\\b" + DateParser.TwoYearEx, RegexOptions.Compiled);
      RegExParser regExParser10 = regExParser9;
      int[] numArray5 = new int[6];
      numArray5[2] = 1;
      regExParser10.Indices = numArray5;
      regExParserArray[42] = regExParser9;
      RegExParser regExParser11 = new RegExParser();
      regExParser11.Expression = new Regex("\\b" + DateParser.MonthStrEx + "\\b", RegexOptions.Compiled);
      RegExParser regExParser12 = regExParser11;
      int[] numArray6 = new int[6];
      numArray6[1] = 1;
      regExParser12.Indices = numArray6;
      regExParserArray[43] = regExParser11;
      this._parsers = regExParserArray;
      this._specialParsers = new Regex[5]
      {
        new Regex(string.Format("(?i)(-)|(?i)(/)|(–)|{0}|{1}", (object) DateParser.CreateRegExFromResource(DateParserStrings.ResourceManager.GetString("Between", this._culture), true), (object) DateParser.CreateRegExFromResource(DateParserStrings.ResourceManager.GetString("Range", this._culture), true)), RegexOptions.Compiled),
        new Regex("^ *([0-9]+)[ ./\\-,]+([0-9][0-9][0-9]+)[ ./\\-,]*\\p{L}+(?i)(?<!\\b(" + this.BceRegex + "|" + this.BcRegex + "|" + this.AdRegex + "|" + this.CeRegex + ")) *$", RegexOptions.Compiled),
        new Regex("^ *[0-9]+[ ./\\-,]+([0-9][0-9][0-9]+)[ ./\\-,]+[0-9]+ *$", RegexOptions.Compiled),
        new Regex(string.Format("^ *({0}|-|{{~}}|\\u2013) *$", (object) DateParserStrings.ResourceManager.GetString("To", this._culture)), RegexOptions.Compiled),
        new Regex(string.Format(" *({0}|{1}|{{~}}|-|\\\\u2013|\\.) *", (object) DateParserStrings.ResourceManager.GetString("To", this._culture), (object) DateParserStrings.ResourceManager.GetString("And", this._culture)), RegexOptions.Compiled)
      };
    }

    public static bool CommonEra
    {
      get => DateParser._commonEra;
      set => DateParser._commonEra = value;
    }

    public static string LdsKeyword(int i) => DateParser._LdsKeywords[i, 0];

    private static string CreateRegExFromResource(string rsrc) => DateParser.CreateRegExFromResource(rsrc, false);

    private static string CreateRegExFromResource(string rsrc, bool groupTerms)
    {
      string[] strArray = rsrc.Split(',');
      StringBuilder stringBuilder = new StringBuilder();
      foreach (string str in strArray)
      {
        if (stringBuilder.Length > 0)
          stringBuilder.Append("|");
        if (groupTerms)
          stringBuilder.Append('(');
        stringBuilder.Append(Regex.Escape(str));
        if (groupTerms)
          stringBuilder.Append(')');
      }
      return stringBuilder.ToString();
    }

    private static string CreatePunctuatedRegExFromResource(string rsrc, bool groupTerms)
    {
      string[] strArray = rsrc.Split(',');
      StringBuilder stringBuilder = new StringBuilder();
      foreach (string str in strArray)
      {
        if (stringBuilder.Length > 0)
          stringBuilder.Append("|");
        if (groupTerms)
          stringBuilder.Append('(');
        stringBuilder.Append(Regex.Escape(str).Replace("\\.", "\\.?"));
        if (groupTerms)
          stringBuilder.Append(')');
      }
      return stringBuilder.ToString();
    }

    public uint? FirstDate
    {
      get => this._dateFirst;
      set => this._dateFirst = value;
    }

    public uint? SecondDate
    {
      get => this._dateSecond;
      set => this._dateSecond = value;
    }

    public DateParseErrorType Reason => this._dateParseErrorInfo[0].Reason;

    public string Data => this._dateParseErrorInfo[0].Data;

    public bool Parse(string dateStr)
    {
      this._orgEntry = dateStr;
      return this.Parse(dateStr, out this._dateFirst, out this._dateSecond, out this._Qualifier);
    }

    private static uint? StripQualifiers(uint? date) => !date.HasValue ? date : new uint?(date.Value & 4294967036U);

    private static bool IsADate(uint? date)
    {
      if (!date.HasValue)
        return false;
      uint? nullable1 = date;
      uint num1 = 2147483136;
      uint? nullable2 = nullable1.HasValue ? new uint?(nullable1.GetValueOrDefault() & num1) : new uint?();
      uint num2 = 0;
      return nullable2.GetValueOrDefault() > num2 & nullable2.HasValue;
    }

    [DebuggerStepThrough]
    public bool Parse(
      string dateStr,
      out uint? firstDate,
      out uint? secondDate,
      out DateModifier qualifier)
    {
      if (dateStr.IndexOf("-") > -1 && dateStr.IndexOf("0/") > -1)
      {
        Match match = DateParser.s_parsers["special0"].Match(dateStr);
        if (match.Groups.Count > 1 && dateStr.Equals(match.Groups[0].Value))
          dateStr = "Bet. " + (object) match.Groups[1] + "/" + (object) match.Groups[2] + "/" + (object) match.Groups[3] + " - " + (object) match.Groups[1] + "/" + (object) match.Groups[4] + "/" + (object) match.Groups[3];
      }
      DateParser._ignoreDoubleDates = false;
      if (dateStr.IndexOf(Resources.ResourceManager.GetString("DateBC", this._culture) + ":") == 0)
        dateStr = dateStr.Substring(3) + " " + Resources.ResourceManager.GetString("DateBC", this._culture).ToLower();
      long ticks = DateTime.Now.Ticks;
      firstDate = new uint?();
      secondDate = new uint?();
      qualifier = DateModifier.None;
      bool flag1 = false;
      bool flag2 = false;
      dateStr = dateStr.Trim();
      if (dateStr[0] == '<' && dateStr[dateStr.Length - 1] == '>')
      {
        dateStr = dateStr.Substring(1, dateStr.Length - 2);
        flag2 = true;
      }
      Match match1 = DateParser.s_parsers["standard"].Match(dateStr);
      if (match1.Success && match1.Value.Equals(dateStr))
      {
        int int32_1 = Convert.ToInt32(match1.Groups[1].Value);
        int int32_2 = Convert.ToInt32(match1.Groups[3].Value);
        if (int32_1 < 32 && int32_2 > DateParser.DoubleDateCutoffYear && int32_2 < DateParser.currentYear)
        {
          int month = this.GetMonth(match1.Groups[2].Value);
          if (month > 0 && (month != 2 || int32_1 <= 27) && int32_1 <= DateParser.MonthMaxDays[month - 1] && int32_1 > 0)
          {
            firstDate = new uint?(SDNDate.Encode(new int?(int32_2), new int?(month), new int?(int32_1), DateModifier.None));
            flag1 = true;
          }
        }
      }
      if (!flag1)
      {
        try
        {
          Match match2 = DateParser.s_parsers["special1"].Match(dateStr);
          if (match2.Groups.Count == 4)
            dateStr = match2.Groups[1].Value + " " + match2.Groups[3].Value + " - " + match2.Groups[2].Value + " " + match2.Groups[3].Value;
        }
        catch
        {
        }
        try
        {
          Match match3 = DateParser.s_parsers["special2"].Match(dateStr);
          if (match3.Groups.Count == 5)
          {
            if (!match3.Groups[2].Value.Equals("00"))
            {
              if (!match3.Groups[3].Value.Equals("00"))
              {
                if (match3.Groups[1].Value.Equals(match3.Groups[4].Value))
                  dateStr = match3.Groups[2].Value + "/" + match3.Groups[3].Value + "/" + (object) match3.Groups[1] + "/" + (object) (Convert.ToInt32(match3.Groups[4].Value) + 1);
              }
            }
          }
        }
        catch
        {
        }
        if (dateStr.IndexOf(Resources.ResourceManager.GetString("DateOr", this._culture)) == 0)
        {
          dateStr = Resources.ResourceManager.GetString("DateBetween", this._culture) + dateStr.Substring(Resources.ResourceManager.GetString("DateOr", this._culture).Length);
          DateParser._ignoreDoubleDates = true;
        }
        if (dateStr.IndexOf(Resources.ResourceManager.GetString("DateAbout", this._culture)) == 0 && dateStr.IndexOf("-") != -1 && dateStr.IndexOf("-") == dateStr.LastIndexOf("-"))
          dateStr = Resources.ResourceManager.GetString("DateBetween", this._culture) + dateStr.Substring(Resources.ResourceManager.GetString("DateAbout", this._culture).Length);
        if (dateStr.IndexOf(Resources.ResourceManager.GetString("DateBetween", this._culture)) == 0 || dateStr.IndexOf("EstBetween:") == 0 || dateStr.IndexOf(Resources.ResourceManager.GetString("DateRange", this._culture)) == 0 || dateStr.IndexOf("Bet") == 0)
          dateStr = dateStr.Replace("-", " and ");
        ParseData parseData = new ParseData(dateStr, firstDate, secondDate);
        parseData.DateStr = parseData.DateStr.Replace("–", "-");
        bool flag3 = this._specialParsers[0].Match(parseData.DateStr).Groups.Count > 1;
        string dateStr1 = parseData.DateStr;
        flag1 = this.Parse(parseData);
        uint? nullable1;
        if (flag3 && parseData.SecondDate.HasValue)
        {
          int num = dateStr1.IndexOf("-");
          if (num <= -1 || dateStr1.LastIndexOf("-") == num || dateStr1.IndexOf(DateParserStrings.ResourceManager.GetString("And", this._culture)) > 0 || dateStr1.IndexOf(DateParserStrings.ResourceManager.GetString("Or", this._culture)) > 0)
          {
            parseData.DateParseErrorInfo.Clear();
            parseData.FirstDate = new uint?();
            parseData.SecondDate = new uint?();
            parseData.DateStr = dateStr1;
            string dateStr2 = parseData.DateStr;
            string str1 = Regex.Replace(Regex.Replace(dateStr2, string.Format("(?i)\\b({0}|{1}|-|\\u2013)\\b", (object) DateParserStrings.ResourceManager.GetString("And", this._culture), (object) DateParserStrings.ResourceManager.GetString("To", this._culture)), " {~} "), string.Format("(?i)\\b({0}|{1}|{2}|{3})", (object) DateParser.CreateRegExFromResource(DateParserStrings.ResourceManager.GetString("Between", this._culture)), (object) DateParser.CreateRegExFromResource(DateParserStrings.ResourceManager.GetString("Before", this._culture)), (object) DateParser.CreateRegExFromResource(DateParserStrings.ResourceManager.GetString("After", this._culture)), (object) DateParser.CreateRegExFromResource(DateParserStrings.ResourceManager.GetString("Range", this._culture))), "").Replace(":", "").Replace("-", "{~}");
            string input = str1.Substring(0, str1.IndexOf("{~}")).Trim();
            string str2 = str1.Substring(str1.IndexOf("{~}") + 3).Trim();
            uint? firstDate1 = new uint?();
            uint? nullable2 = new uint?();
            Match match4 = Regex.Match(input, "([0-3][0-9]|[0-9])");
            uint? firstDate2;
            if ((match4.Groups.Count == 1 || match4.Groups.Count == 2) && match4.Groups[0].Value.Equals(input) && Convert.ToInt32(input) < 32)
            {
              parseData.DateStr = str2;
              flag1 = this.Parse(parseData);
              firstDate2 = parseData.FirstDate;
              SDNDate sdnDate = new SDNDate(firstDate2.Value);
              int? year = sdnDate.Year;
              int? month = sdnDate.Month;
              DateModifier modifier = sdnDate.Modifier;
              firstDate1 = new uint?(SDNDate.Encode(year, month, new int?(Convert.ToInt32(input)), modifier));
            }
            else
            {
              parseData.DateStr = input;
              this.Parse(parseData);
              firstDate1 = parseData.FirstDate;
              parseData.ParsingSecondDate = true;
              parseData.DateStr = str2;
              flag1 = this.Parse(parseData);
              firstDate2 = parseData.FirstDate;
            }
            DateParser.CopyYears(ref firstDate1, ref firstDate2);
            parseData.ParsingSecondDate = false;
            nullable1 = firstDate1;
            uint? nullable3 = firstDate2;
            if (nullable1.GetValueOrDefault() > nullable3.GetValueOrDefault() & nullable1.HasValue & nullable3.HasValue)
            {
              DateParser.CorrectDateRangeOrder(ref firstDate1, ref firstDate2);
              parseData.DateParseErrorInfo.Add(new DateParseErrorInfo(DateParseErrorType.RangeError, dateStr2, true));
            }
            parseData.FirstDate = firstDate1;
            parseData.SecondDate = firstDate2;
          }
        }
        firstDate = parseData.FirstDate;
        ref uint? local1 = ref secondDate;
        uint? secondDate1 = parseData.SecondDate;
        nullable1 = firstDate;
        uint? nullable4;
        if (!((int) secondDate1.GetValueOrDefault() == (int) nullable1.GetValueOrDefault() & secondDate1.HasValue == nullable1.HasValue))
        {
          nullable4 = parseData.SecondDate;
        }
        else
        {
          nullable1 = new uint?();
          nullable4 = nullable1;
        }
        local1 = nullable4;
        qualifier = parseData.Qualifier;
        if (secondDate.HasValue)
        {
          nullable1 = secondDate;
          uint num = 0;
          if (!((int) nullable1.GetValueOrDefault() == (int) num & nullable1.HasValue))
          {
            firstDate = DateParser.StripQualifiers(firstDate);
            secondDate = DateParser.StripQualifiers(secondDate);
          }
        }
        if (DateParser.today == 0U)
          DateParser.today = SDNDate.Encode(DateTime.Now);
        uint? nullable5 = firstDate;
        uint num1 = 2147483136;
        nullable1 = nullable5.HasValue ? new uint?(nullable5.GetValueOrDefault() & num1) : new uint?();
        uint today = DateParser.today;
        if (nullable1.GetValueOrDefault() > today & nullable1.HasValue)
          parseData.DateParseErrorInfo.Add(new DateParseErrorInfo(DateParseErrorType.FutureDate, dateStr, parseData.ParsingSecondDate));
        if (dateStr.Length > 0)
          dateStr = dateStr.Trim();
        if (DateParser.IsADate(parseData.FirstDate) && DateParser.IsADate(parseData.SecondDate) && !flag3)
          parseData.DateParseErrorInfo.Add(new DateParseErrorInfo(DateParseErrorType.UnknownContent, dateStr, true));
        if (parseData.DateParseErrorInfo.Count > 0)
        {
          DateParser.SetErrorsInPriorityOrder(parseData);
          throw new DateParseException(new DateParseError(parseData.DateParseErrorInfo, parseData.FirstDate, parseData.SecondDate));
        }
        if (flag2)
        {
          ref uint? local2 = ref firstDate;
          nullable1 = firstDate;
          uint? nullable6;
          if (!nullable1.HasValue)
          {
            nullable5 = new uint?();
            nullable6 = nullable5;
          }
          else
            nullable6 = new uint?(nullable1.GetValueOrDefault() | 3U);
          local2 = nullable6;
        }
      }
      return flag1;
    }

    private static uint? ParseLdsKeywords(string dateStr)
    {
      uint? ldsKeywords = new uint?();
      string str = !dateStr.Substring(0, 1).Equals("?") || dateStr.Length <= 1 ? dateStr : dateStr.Substring(1);
      for (int index1 = 0; index1 < DateParser._LdsKeywords.Length / DateParser._MAX_LDS_KEYWORDS; ++index1)
      {
        for (int index2 = 1; index2 < DateParser._MAX_LDS_KEYWORDS && DateParser._LdsKeywords[index1, index2] != null; ++index2)
        {
          if (str.ToLower().Equals(DateParser._LdsKeywords[index1, index2].ToLower()))
          {
            ldsKeywords = new uint?((uint) (int.MinValue + index1));
            break;
          }
        }
      }
      return ldsKeywords;
    }

    protected void FindIllegalPatterns(ParseData parseData)
    {
      Match match1 = this._specialParsers[1].Match(parseData.DateStr);
      if (match1.Groups.Count > 1)
      {
        int int32 = Convert.ToInt32(match1.Groups[2].Value);
        parseData.DateParseErrorInfo.Add(new DateParseErrorInfo(DateParseErrorType.UnknownContent, parseData.DateStr, parseData.FirstDate.HasValue || parseData.ParsingSecondDate));
        throw new DateParseException(new DateParseError(parseData.DateParseErrorInfo, new uint?(SDNDate.Encode(new int?(int32), new int?(0), new int?(0), DateModifier.MonthMissing | DateModifier.DayMissing)), new uint?(0U)));
      }
      Match match2 = this._specialParsers[2].Match(parseData.DateStr);
      if (match2.Groups.Count <= 1)
        return;
      int int32_1 = Convert.ToInt32(match2.Groups[1].Value);
      int num = parseData.DateStr.IndexOf("/");
      if (num == -1 || num != parseData.DateStr.LastIndexOf("/"))
      {
        parseData.DateParseErrorInfo.Add(new DateParseErrorInfo(DateParseErrorType.UnknownContent, parseData.DateStr, parseData.FirstDate.HasValue || parseData.ParsingSecondDate));
        throw new DateParseException(new DateParseError(parseData.DateParseErrorInfo, new uint?(SDNDate.Encode(new int?(int32_1), new int?(0), new int?(0), DateModifier.MonthMissing | DateModifier.DayMissing)), new uint?(0U)));
      }
    }

    protected bool Parse(ParseData parseData)
    {
      bool flag = false;
      this.FindIllegalPatterns(parseData);
      string str1 = parseData.DateStr.Trim().ToLower();
      parseData.Qualifier = DateModifier.None;
      uint? nullable1 = parseData.FirstDate = DateParser.ParseLdsKeywords(str1);
      uint num1 = 0;
      if (nullable1.GetValueOrDefault() > num1 & nullable1.HasValue)
      {
        flag = true;
      }
      else
      {
        foreach (string key in this.WordsToQualifier.Keys)
        {
          if (key != null && str1.IndexOf(key) == 0)
          {
            string str2 = key + "\\.?";
            Match match = Regex.Match(str1, "(^" + str2 + ")[0-9 :]");
            if (match.Groups.Count > 1 && match.Groups[1].Value.Equals(key) || match.Groups[1].Value.Equals(key + "."))
            {
              parseData.Qualifier = this.WordsToQualifier[key];
              str1 = str1.Substring(match.Groups[1].Value.Length).Trim();
              break;
            }
          }
        }
        string str3 = Regex.Replace(Regex.Replace(str1, string.Format("(?i)\\b({0}|{1})\\b", (object) DateParserStrings.ResourceManager.GetString("And", this._culture), (object) DateParserStrings.ResourceManager.GetString("To", this._culture)), " {~} "), string.Format("(?i)\\b({0}|{1}|{2}|{3})", (object) DateParser.CreateRegExFromResource(DateParserStrings.ResourceManager.GetString("Between", this._culture)), (object) DateParser.CreateRegExFromResource(DateParserStrings.ResourceManager.GetString("Range", this._culture)), (object) DateParser.CreateRegExFromResource(DateParserStrings.ResourceManager.GetString("Before", this._culture)), (object) DateParser.CreateRegExFromResource(DateParserStrings.ResourceManager.GetString("After", this._culture))), "").Replace(":", "");
        for (int index = 0; index < this._parsers.Length; ++index)
        {
          str3 = this.ParseDate(str3, this._parsers[index], ref parseData);
          if (str3.Length == 0)
          {
            flag = true;
            break;
          }
          if (parseData.FirstDate.HasValue)
          {
            uint? firstDate = parseData.FirstDate;
            uint num2 = 0;
            if (!((int) firstDate.GetValueOrDefault() == (int) num2 & firstDate.HasValue) && parseData.SecondDate.HasValue)
            {
              uint? secondDate = parseData.SecondDate;
              uint num3 = 0;
              if (!((int) secondDate.GetValueOrDefault() == (int) num3 & secondDate.HasValue))
              {
                Match match = this._specialParsers[3].Match(str3);
                if (match.Groups.Count <= 0 || !match.Groups[0].Value.Equals(str3))
                {
                  parseData.DateParseErrorInfo.Add(new DateParseErrorInfo(DateParseErrorType.UnknownContent, parseData.DateStr, parseData.FirstDate.HasValue || parseData.ParsingSecondDate));
                  return false;
                }
              }
            }
          }
        }
        uint? firstDate1 = parseData.FirstDate;
        uint? secondDate1 = parseData.SecondDate;
        DateParser.CopyYears(ref firstDate1, ref secondDate1);
        uint? nullable2 = firstDate1;
        uint? nullable3 = secondDate1;
        if (nullable2.GetValueOrDefault() > nullable3.GetValueOrDefault() & nullable2.HasValue & nullable3.HasValue)
          DateParser.CorrectDateRangeOrder(ref firstDate1, ref secondDate1);
        ParseData parseData1 = parseData;
        nullable3 = firstDate1;
        uint num4 = 0;
        uint? nullable4;
        if (!((int) nullable3.GetValueOrDefault() == (int) num4 & nullable3.HasValue))
        {
          nullable4 = firstDate1;
        }
        else
        {
          nullable3 = new uint?();
          nullable4 = nullable3;
        }
        parseData1.FirstDate = nullable4;
        ParseData parseData2 = parseData;
        nullable3 = secondDate1;
        uint num5 = 0;
        uint? nullable5;
        if (!((int) nullable3.GetValueOrDefault() == (int) num5 & nullable3.HasValue))
        {
          nullable5 = secondDate1;
        }
        else
        {
          nullable3 = new uint?();
          nullable5 = nullable3;
        }
        parseData2.SecondDate = nullable5;
        if (str3.Length > 0)
        {
          Match match = this._specialParsers[4].Match(str3);
          if (match.Groups.Count == 0 || !match.Groups[0].Value.Equals(str3))
          {
            IList<DateParseErrorInfo> dateParseErrorInfo1 = parseData.DateParseErrorInfo;
            string data = str3;
            nullable3 = parseData.FirstDate;
            int num6 = nullable3.HasValue ? 1 : (parseData.ParsingSecondDate ? 1 : 0);
            DateParseErrorInfo dateParseErrorInfo2 = new DateParseErrorInfo(DateParseErrorType.UnknownContent, data, num6 != 0);
            dateParseErrorInfo1.Add(dateParseErrorInfo2);
          }
        }
      }
      return flag;
    }

    protected static void CopyYears(ref uint? firstDate, ref uint? secondDate)
    {
      if (!firstDate.HasValue)
        return;
      uint? nullable1 = firstDate;
      uint num1 = 0;
      if ((int) nullable1.GetValueOrDefault() == (int) num1 & nullable1.HasValue || !secondDate.HasValue)
        return;
      nullable1 = secondDate;
      uint num2 = 0;
      if ((int) nullable1.GetValueOrDefault() == (int) num2 & nullable1.HasValue)
        return;
      Date instance1 = Date.CreateInstance(new uint?(firstDate.Value));
      Date instance2 = Date.CreateInstance(new uint?(secondDate.Value));
      int? year1 = instance1.Year;
      int? month1 = instance1.Month;
      int? day1 = instance1.Day;
      DateModifier modifier1 = instance1.Modifier;
      int? year2 = instance2.Year;
      int? month2 = instance2.Month;
      int? day2 = instance2.Day;
      DateModifier modifier2 = instance2.Modifier;
      bool flag1 = false;
      bool flag2 = false;
      int? nullable2;
      if (year1.HasValue)
      {
        nullable2 = year1;
        int num3 = 0;
        if (!(nullable2.GetValueOrDefault() == num3 & nullable2.HasValue))
          goto label_8;
      }
      if (year2.HasValue)
      {
        nullable2 = year2;
        int num4 = 0;
        if (!(nullable2.GetValueOrDefault() == num4 & nullable2.HasValue))
        {
          year1 = year2;
          modifier1 &= ~DateModifier.YearMissing;
          flag1 = true;
        }
      }
label_8:
      if (year2.HasValue)
      {
        nullable2 = year2;
        int num5 = 0;
        if (!(nullable2.GetValueOrDefault() == num5 & nullable2.HasValue))
          goto label_13;
      }
      if (year1.HasValue)
      {
        nullable2 = year1;
        int num6 = 0;
        if (!(nullable2.GetValueOrDefault() == num6 & nullable2.HasValue))
        {
          year2 = year1;
          modifier2 &= ~DateModifier.YearMissing;
          flag2 = true;
        }
      }
label_13:
      if (flag1)
        firstDate = new uint?(SDNDate.Encode(year1, month1, day1, modifier1));
      if (!flag2)
        return;
      secondDate = new uint?(SDNDate.Encode(year2, month2, day2, modifier2));
    }

    protected static void CorrectDateRangeOrder(ref uint? firstDate, ref uint? secondDate)
    {
      uint? nullable1 = firstDate;
      uint? nullable2 = secondDate;
      if (!(nullable1.GetValueOrDefault() > nullable2.GetValueOrDefault() & nullable1.HasValue & nullable2.HasValue))
        return;
      uint? nullable3 = secondDate;
      uint num = 0;
      if ((int) nullable3.GetValueOrDefault() == (int) num & nullable3.HasValue)
        return;
      uint? nullable4 = firstDate;
      firstDate = secondDate;
      secondDate = nullable4;
    }

    protected int GetDateMonth(
      Match match,
      int monthIndex,
      bool monthIsNumber,
      ref ParseData parseData)
    {
      int dateMonth = 0;
      if (monthIndex > 0)
      {
        if (monthIsNumber)
        {
          dateMonth = Convert.ToInt32(match.Groups[monthIndex].Value);
        }
        else
        {
          dateMonth = this.GetMonth(match.Groups[monthIndex].Value);
          if (dateMonth == 0)
            parseData.DateParseErrorInfo.Add(new DateParseErrorInfo(DateParseErrorType.UnknownContent, match.Groups[monthIndex].Value, parseData.FirstDate.HasValue || parseData.ParsingSecondDate));
        }
      }
      return dateMonth;
    }

    protected static int GetDateDay(Match match, int dayIndex) => dayIndex > 0 ? Convert.ToInt32(match.Groups[dayIndex].Value) : 0;

    protected bool IsBC(ref ParseData parseData, string bcad)
    {
      bool flag = false;
      if (bcad != null && bcad.Length > 0)
      {
        Match match1 = Regex.Match(bcad, this.BcRegex);
        Match match2 = Regex.Match(bcad, this.BceRegex);
        Match match3 = Regex.Match(bcad, this.AdRegex);
        Match match4 = Regex.Match(bcad, this.CeRegex);
        if (match2.Groups.Count == 1 && match2.Groups[0].Value.Equals(bcad))
        {
          flag = true;
          if (DateParser.AncientDateTypeWarning && !DateParser._commonEra)
            parseData.DateParseErrorInfo.Add(new DateParseErrorInfo(DateParseErrorType.WrongAncientDateFormat, bcad, parseData.FirstDate.HasValue || parseData.ParsingSecondDate));
        }
        else if (match1.Groups.Count == 1 && match1.Groups[0].Value.Equals(bcad))
        {
          flag = true;
          if (DateParser.AncientDateTypeWarning && DateParser._commonEra)
            parseData.DateParseErrorInfo.Add(new DateParseErrorInfo(DateParseErrorType.WrongAncientDateFormat, bcad, parseData.FirstDate.HasValue || parseData.ParsingSecondDate));
        }
        if (match3.Groups.Count == 1 && match3.Groups[0].Value.Equals(bcad))
        {
          if (DateParser.AncientDateTypeWarning && DateParser._commonEra)
            parseData.DateParseErrorInfo.Add(new DateParseErrorInfo(DateParseErrorType.WrongAncientDateFormat, bcad, parseData.FirstDate.HasValue || parseData.ParsingSecondDate));
        }
        else if (match4.Groups.Count == 1 && match4.Groups[0].Value.Equals(bcad) && DateParser.AncientDateTypeWarning && !DateParser._commonEra)
          parseData.DateParseErrorInfo.Add(new DateParseErrorInfo(DateParseErrorType.WrongAncientDateFormat, bcad, parseData.FirstDate.HasValue || parseData.ParsingSecondDate));
      }
      return flag;
    }

    protected int GetDateYear(
      Match match,
      int yearIndex,
      ref ParseData parseData,
      int month,
      int day)
    {
      if (yearIndex <= 0)
        return 0;
      string data = match.Groups[yearIndex].Value;
      int length = data.IndexOf("/");
      int dateYear;
      if (length > -1)
      {
        int int32 = Convert.ToInt32(data.Substring(0, length));
        int num = Convert.ToInt32(data.Substring(length + 1));
        if (num < 10)
        {
          if (num == 0)
            num = 10;
          num = int32 / 10 * 10 + num;
        }
        else if (num < 100)
          num = int32 / 100 * 100 + num;
        if (num != int32 + 1)
          parseData.DateParseErrorInfo.Add(new DateParseErrorInfo(DateParseErrorType.InvalidDoubleDate, data.Trim(), parseData.FirstDate.HasValue || parseData.ParsingSecondDate));
        parseData.Qualifier |= DateModifier.DoubleDate;
        dateYear = int32 + 1;
      }
      else
        dateYear = Convert.ToInt32(data);
      string bcad = match.Groups.Count > yearIndex + 1 ? match.Groups[match.Groups.Count - 1].Value : "";
      bool? nullable;
      if (this.IsBC(ref parseData, bcad))
        dateYear = -dateYear;
      else if (yearIndex > 0)
      {
        if (match.Groups[yearIndex].Value.Length == 2)
        {
          if (!DateParser.TwoDigitYearAsCurrentCentury.HasValue)
          {
            parseData.DateParseErrorInfo.Add(new DateParseErrorInfo(DateParseErrorType.TwoDigitYear, (match.Groups[yearIndex].Value + " " + bcad).Trim(), parseData.FirstDate.HasValue || parseData.ParsingSecondDate));
          }
          else
          {
            nullable = DateParser.TwoDigitYearAsCurrentCentury;
            bool flag = false;
            if (!(nullable.GetValueOrDefault() == flag & nullable.HasValue))
            {
              int year = DateTime.Now.Year;
              if (year % 100 < dateYear)
                dateYear += year - year % 100 - 100;
              else
                dateYear += year - year % 100;
            }
          }
        }
        else if (match.Groups[yearIndex].Value.Length == 1)
          parseData.DateParseErrorInfo.Add(new DateParseErrorInfo(DateParseErrorType.OneDigitYear, (match.Groups[yearIndex].Value + " " + bcad).Trim(), parseData.FirstDate.HasValue || parseData.ParsingSecondDate));
      }
      if (dateYear < -4713 || dateYear > 6000)
        parseData.DateParseErrorInfo.Add(new DateParseErrorInfo(DateParseErrorType.YearOutOfRange, (match.Groups[yearIndex].Value + " " + bcad).Trim(), parseData.FirstDate.HasValue || parseData.ParsingSecondDate));
      if (dateYear > 0 && dateYear <= DateParser.DoubleDateCutoffYear && month > 0 && month < 4 && (parseData.Qualifier & DateModifier.DoubleDate) != DateModifier.DoubleDate && (month < 3 || month == 3 && day < 25))
      {
        if (!DateParser.AutoCreateDoubleDates.HasValue && !DateParser._ignoreDoubleDates)
          parseData.DateParseErrorInfo.Add(new DateParseErrorInfo(DateParseErrorType.DoubleDateAmbiguous, data, parseData.FirstDate.HasValue || parseData.ParsingSecondDate));
        if (!DateParser._ignoreDoubleDates)
        {
          nullable = DateParser.AutoCreateDoubleDates;
          bool flag = true;
          if (!(nullable.GetValueOrDefault() == flag & nullable.HasValue))
            goto label_32;
        }
        parseData.Qualifier |= DateModifier.DoubleDate;
      }
label_32:
      if ((dateYear < 0 || dateYear > DateParser.DoubleDateCutoffYear) && (parseData.Qualifier & DateModifier.DoubleDate) == DateModifier.DoubleDate)
      {
        parseData.DateParseErrorInfo.Add(new DateParseErrorInfo(DateParseErrorType.InvalidDoubleDate, data, parseData.FirstDate.HasValue || parseData.ParsingSecondDate));
        parseData.Qualifier &= ~DateModifier.DoubleDate;
      }
      else if (dateYear > 0 && dateYear < DateParser.DoubleDateCutoffYear && month >= 3 && (month != 3 || day >= 26) && (parseData.Qualifier & DateModifier.DoubleDate) == DateModifier.DoubleDate)
      {
        parseData.DateParseErrorInfo.Add(new DateParseErrorInfo(DateParseErrorType.InvalidDoubleDate, parseData.DateStr, parseData.FirstDate.HasValue || parseData.ParsingSecondDate));
        parseData.Qualifier &= ~DateModifier.DoubleDate;
      }
      return dateYear;
    }

    protected uint? GetDate(
      Match match,
      int dayIndex,
      int monthIndex,
      int yearIndex,
      bool monthIsNumber,
      int qtr,
      ref ParseData parseData)
    {
      uint num1 = 0;
      if (match.Groups.Count > 1)
      {
        int day = DateParser.GetDateDay(match, dayIndex);
        int month = this.GetDateMonth(match, monthIndex, monthIsNumber, ref parseData);
        if (((!DateParser.DayBeforeMonth ? 0 : (dayIndex != 0 ? 1 : 0)) & (monthIsNumber ? 1 : 0)) != 0 && (dayIndex != 3 || monthIndex != 2 || yearIndex != 1))
        {
          int num2 = day;
          day = month;
          month = num2;
        }
        uint? firstDate;
        if (month > 12 || month < 0)
        {
          IList<DateParseErrorInfo> dateParseErrorInfo1 = parseData.DateParseErrorInfo;
          string data1 = month.ToString();
          firstDate = parseData.FirstDate;
          int num3 = firstDate.HasValue ? 1 : (parseData.ParsingSecondDate ? 1 : 0);
          DateParseErrorInfo dateParseErrorInfo2 = new DateParseErrorInfo(DateParseErrorType.MonthOutOfRange, data1, num3 != 0);
          dateParseErrorInfo1.Add(dateParseErrorInfo2);
          if (day < 0 || day > 31)
          {
            IList<DateParseErrorInfo> dateParseErrorInfo3 = parseData.DateParseErrorInfo;
            string data2 = day.ToString();
            firstDate = parseData.FirstDate;
            int num4 = firstDate.HasValue ? 1 : (parseData.ParsingSecondDate ? 1 : 0);
            DateParseErrorInfo dateParseErrorInfo4 = new DateParseErrorInfo(DateParseErrorType.DayOutOfRange, data2, num4 != 0);
            dateParseErrorInfo3.Add(dateParseErrorInfo4);
            day = 0;
          }
        }
        else if (month > 0 && day > DateParser.MonthMaxDays[month - 1] || day < 0)
        {
          IList<DateParseErrorInfo> dateParseErrorInfo5 = parseData.DateParseErrorInfo;
          string data = day.ToString();
          firstDate = parseData.FirstDate;
          int num5 = firstDate.HasValue ? 1 : (parseData.ParsingSecondDate ? 1 : 0);
          DateParseErrorInfo dateParseErrorInfo6 = new DateParseErrorInfo(DateParseErrorType.DayOutOfRange, data, num5 != 0);
          dateParseErrorInfo5.Add(dateParseErrorInfo6);
          day = 0;
        }
        int dateYear = this.GetDateYear(match, yearIndex, ref parseData, month, day);
        if (month == 2 && day == 29 && !DateParser.IsLeapYear(dateYear))
        {
          IList<DateParseErrorInfo> dateParseErrorInfo7 = parseData.DateParseErrorInfo;
          string data = dateYear.ToString();
          firstDate = parseData.FirstDate;
          int num6 = firstDate.HasValue ? 1 : (parseData.ParsingSecondDate ? 1 : 0);
          DateParseErrorInfo dateParseErrorInfo8 = new DateParseErrorInfo(DateParseErrorType.NotLeapYear, data, num6 != 0);
          dateParseErrorInfo7.Add(dateParseErrorInfo8);
          day = 0;
        }
        if (qtr != 0 && match.Groups[qtr].Value.Length > 0)
          parseData.Qualifier |= DateModifier.Quarter;
        if (dateYear != 0 || month != 0 || day != 0)
        {
          if (month == 0)
            day = 0;
          num1 = SDNDate.Encode(new int?(dateYear), new int?(month), new int?(day), parseData.Qualifier);
        }
        parseData.Qualifier = DateModifier.None;
      }
      return new uint?(num1);
    }

    protected static bool IsReasonableDate(uint? date)
    {
      if (!date.HasValue)
        return false;
      uint? nullable = date;
      uint num = 100;
      return nullable.GetValueOrDefault() > num & nullable.HasValue;
    }

    protected static bool SetDate(uint? date, ref uint? dateOut)
    {
      uint? nullable1;
      if (dateOut.HasValue)
      {
        nullable1 = dateOut;
        uint num = 0;
        if (!((int) nullable1.GetValueOrDefault() == (int) num & nullable1.HasValue))
          goto label_3;
      }
      dateOut = new uint?(date.Value);
label_3:
      nullable1 = dateOut;
      uint? nullable2 = date;
      if (!((int) nullable1.GetValueOrDefault() == (int) nullable2.GetValueOrDefault() & nullable1.HasValue == nullable2.HasValue))
        return false;
      nullable2 = date;
      uint num1 = 0;
      return !((int) nullable2.GetValueOrDefault() == (int) num1 & nullable2.HasValue);
    }

    protected string ParseDate(string dateStr, RegExParser parser, ref ParseData parseData)
    {
      Regex expression = parser.Expression;
      int index1 = parser.Indices[0];
      int index2 = parser.Indices[1];
      int index3 = parser.Indices[2];
      int index4 = parser.Indices[3];
      bool monthIsNumber = parser.Indices[4] != 0;
      int index5 = parser.Indices[5];
      string input = dateStr;
      MatchCollection matchCollection = expression.Matches(input);
      if (matchCollection.Count > 0)
      {
        uint? date = this.GetDate(matchCollection[0], index1, index2, index3, monthIsNumber, index5, ref parseData);
        if (DateParser.IsReasonableDate(date))
        {
          int length = dateStr.IndexOf(matchCollection[0].Value);
          dateStr = dateStr.Substring(0, length) + dateStr.Substring(length + matchCollection[0].Length);
        }
        else
          date = new uint?(0U);
        uint? firstDate = parseData.FirstDate;
        uint? dateOut = parseData.SecondDate;
        if (!DateParser.SetDate(date, ref firstDate))
          DateParser.SetDate(date, ref dateOut);
        else if (matchCollection.Count > 1)
        {
          date = this.GetDate(matchCollection[1], index1, index2, index3, monthIsNumber, index5, ref parseData);
          if (DateParser.IsReasonableDate(date))
          {
            if (dateOut.HasValue)
            {
              uint? nullable = dateOut;
              uint num = 0;
              if (!((int) nullable.GetValueOrDefault() == (int) num & nullable.HasValue))
                goto label_17;
            }
            uint? nullable1 = date;
            dateOut = nullable1.HasValue ? new uint?(nullable1.GetValueOrDefault() & 4294967288U) : new uint?();
            dateStr = "";
          }
        }
        else if (matchCollection[0].Groups[index4].Value.Length > 0 && index4 != 0)
        {
          int int32 = Convert.ToInt32(matchCollection[0].Groups[index4].Value);
          if (int32 < 100)
          {
            int? nullable = new SDNDate(firstDate.Value).Year;
            if (!nullable.HasValue)
              nullable = new int?(0);
            int num = (int) (short) nullable.Value / 100;
            dateOut = new uint?(SDNDate.Encode(new int?((int) (short) (int32 + num * 100)), new int?(), new int?(), DateModifier.None));
          }
          else
            dateOut = new uint?(SDNDate.Encode(new int?((int) (short) int32), new int?(), new int?(), DateModifier.None));
        }
label_17:
        parseData.FirstDate = firstDate;
        parseData.SecondDate = dateOut;
      }
      return dateStr;
    }

    protected int GetMonth(string monthStr)
    {
      monthStr = monthStr.ToLower();
      for (int index = 0; index < this.Months.Length / 3; ++index)
      {
        if (monthStr.Equals(this.Months[index, 0]) || monthStr.Equals(this.Months[index, 1]) || this.Months[index, 2] != null && monthStr.Equals(this.Months[index, 2]))
          return index + 1;
      }
      return 0;
    }

    private void InitializeWordsToQualifier(string list, DateModifier modifier)
    {
      string str = list;
      char[] chArray = new char[1]{ ',' };
      foreach (string key in str.Split(chArray))
        this.WordsToQualifier[key] = modifier;
    }

    private void InitializeWordsToQualifier()
    {
      this.WordsToQualifier = new Dictionary<string, DateModifier>();
      this.InitializeWordsToQualifier(DateParserStrings.ResourceManager.GetString("Before", this._culture), DateModifier.Before);
      this.InitializeWordsToQualifier(DateParserStrings.ResourceManager.GetString("About", this._culture), DateModifier.About);
      this.InitializeWordsToQualifier(DateParserStrings.ResourceManager.GetString("After", this._culture), DateModifier.After);
      this.InitializeWordsToQualifier(DateParserStrings.ResourceManager.GetString("Calculated", this._culture), DateModifier.Calculated);
      this.InitializeWordsToQualifier(DateParserStrings.ResourceManager.GetString("Between", this._culture), DateModifier.None);
      this.WordsToQualifier["wft est"] = DateModifier.None;
    }

    protected static bool IsLeapYear(int year)
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

    private static void SetErrorsInPriorityOrder(ParseData parseData)
    {
      if (parseData.DateParseErrorInfo.Count <= 1)
        return;
      IList<DateParseErrorInfo> dateParseErrorInfoList = (IList<DateParseErrorInfo>) new List<DateParseErrorInfo>(parseData.DateParseErrorInfo.Count);
      int count = parseData.DateParseErrorInfo.Count;
      for (int index1 = 0; index1 < count; ++index1)
      {
        int index2 = 0;
        for (int index3 = 1; index3 < parseData.DateParseErrorInfo.Count; ++index3)
        {
          if (parseData.DateParseErrorInfo[index2].Reason < parseData.DateParseErrorInfo[index3].Reason)
            index2 = index3;
        }
        if (dateParseErrorInfoList.Count == 0 || dateParseErrorInfoList[dateParseErrorInfoList.Count - 1].Reason != parseData.DateParseErrorInfo[index2].Reason || dateParseErrorInfoList[dateParseErrorInfoList.Count - 1].SecondDateError != parseData.DateParseErrorInfo[index2].SecondDateError)
          dateParseErrorInfoList.Add(parseData.DateParseErrorInfo[index2]);
        parseData.DateParseErrorInfo.RemoveAt(index2);
      }
      for (int index = 0; index < dateParseErrorInfoList.Count; ++index)
        parseData.DateParseErrorInfo.Add(dateParseErrorInfoList[index]);
    }
  }
}
