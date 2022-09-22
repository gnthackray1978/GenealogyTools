// Decompiled with JetBrains decompiler
// Type: MyFamily.Shared.AgeFormatInfo
// Assembly: MyFamily.Shared, Version=23.3.0.1570, Culture=neutral, PublicKeyToken=295e8b5bb13c2421
// MVID: CB662BCB-E11E-4698-B7B4-24BC7C8B503C
// Assembly location: C:\Users\GeorgePickworth\Documents\development\GenealogyTools\FTMContextNet\MyFamily.Shared.dll

using FTM.Dates.Interfaces;
using FTM.Dates.Properties;
using System;

namespace FTM.Dates
{
  public class AgeFormatInfo : FormatInfo
  {
    public static readonly AgeFormatInfo Default = new AgeFormatInfo();

    public AgeFormatInfo()
    {
    }

    public AgeFormatInfo(string defaultFormat)
      : base(defaultFormat)
    {
    }

    protected override string InnerFormat(
      string format,
      object arg,
      IFormatProvider formatProvider)
    {
      if (arg is IAge)
      {
        IAge age = (IAge) arg;
        if (format == "G")
        {
          if (age.Years >= 2)
            return age.Years.ToString();
          if (age.Years > 0 || age.Months > 0)
            return string.Format(Resources.AgeMonthsFormat, (object) (age.Years * 12 + age.Months));
          if (age.Days > 1)
            return string.Format(Resources.AgeDaysFormat, (object) age.Days);
          return age.Days == 1 ? string.Format(Resources.AgeDayFormat, (object) age.Days) : string.Empty;
        }
      }
      return base.InnerFormat(format, arg, formatProvider);
    }
  }
}
