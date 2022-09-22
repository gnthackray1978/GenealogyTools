// Decompiled with JetBrains decompiler
// Type: MyFamily.Shared.DateParseError
// Assembly: MyFamily.Shared, Version=23.3.0.1570, Culture=neutral, PublicKeyToken=295e8b5bb13c2421
// MVID: CB662BCB-E11E-4698-B7B4-24BC7C8B503C
// Assembly location: C:\Users\GeorgePickworth\Documents\development\GenealogyTools\FTMContextNet\MyFamily.Shared.dll

using System.Collections.Generic;

namespace FTM.Dates
{
  public class DateParseError
  {
    public readonly uint? FirstDate;
    public readonly uint? SecondDate;
    public readonly IList<DateParseErrorInfo> ErrorInfo;

    public DateParseError(IList<DateParseErrorInfo> errorInfo) => this.ErrorInfo = errorInfo;

    public DateParseError(IList<DateParseErrorInfo> errorInfo, uint? firstDate, uint? secondDate)
    {
      this.ErrorInfo = errorInfo;
      this.FirstDate = firstDate;
      this.SecondDate = secondDate;
    }

    protected bool isCritical(DateParseErrorInfo errorInfo)
    {
      switch (errorInfo.Reason)
      {
        case DateParseErrorType.TwoDigitYear:
        case DateParseErrorType.OneDigitYear:
        case DateParseErrorType.DoubleDateAmbiguous:
        case DateParseErrorType.FutureDate:
        case DateParseErrorType.WrongAncientDateFormat:
          return false;
        default:
          return true;
      }
    }

    protected int DefaultErrorIndex
    {
      get
      {
        int defaultErrorIndex = 0;
        for (int index = 0; index < this.ErrorInfo.Count; ++index)
        {
          if (this.isCritical(this.ErrorInfo[index]))
          {
            defaultErrorIndex = index;
            break;
          }
        }
        return defaultErrorIndex;
      }
    }

    public DateParseErrorType Reason => this.ErrorInfo[this.DefaultErrorIndex].Reason;

    public string Data => this.ErrorInfo[this.DefaultErrorIndex].Data.Trim();

    public DateParseError.SeverityLevel Severity
    {
      get
      {
        foreach (DateParseErrorInfo errorInfo in (IEnumerable<DateParseErrorInfo>) this.ErrorInfo)
        {
          if (this.isCritical(errorInfo))
            return DateParseError.SeverityLevel.Critical;
        }
        return DateParseError.SeverityLevel.Warning;
      }
    }

    public string DebugAllErrors
    {
      get
      {
        string debugAllErrors = "";
        foreach (DateParseErrorInfo dateParseErrorInfo in (IEnumerable<DateParseErrorInfo>) this.ErrorInfo)
          debugAllErrors = debugAllErrors + (object) dateParseErrorInfo.Reason + " -> " + dateParseErrorInfo.Data + "\r\n";
        return debugAllErrors;
      }
    }

    public enum SeverityLevel
    {
      Warning,
      Critical,
    }
  }
}
