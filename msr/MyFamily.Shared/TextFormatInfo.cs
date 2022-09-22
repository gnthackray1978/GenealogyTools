// Decompiled with JetBrains decompiler
// Type: MyFamily.Shared.TextFormatInfo
// Assembly: MyFamily.Shared, Version=23.3.0.1570, Culture=neutral, PublicKeyToken=295e8b5bb13c2421
// MVID: CB662BCB-E11E-4698-B7B4-24BC7C8B503C
// Assembly location: C:\Users\GeorgePickworth\Documents\development\GenealogyTools\FTMContextNet\MyFamily.Shared.dll

using FTM.Dates.Interfaces;
using System;

namespace FTM.Dates
{
  public class TextFormatInfo : FormatInfo
  {
    public static readonly TextFormatInfo Default = new TextFormatInfo();

    protected override string InnerFormat(
      string format,
      object obj,
      IFormatProvider formatProvider)
    {
      return obj is IText ? this.FormatText(format, (IText) obj, formatProvider) : base.InnerFormat(format, obj, formatProvider);
    }

    private string FormatText(string format, IText arg, IFormatProvider formatProvider)
    {
      if (format == null)
        format = "G";
      string empty = string.Empty;
      return !(format == "G") ? base.InnerFormat(format, (object) arg, formatProvider) : arg.Text;
    }

    protected override FormatInfo.CustomFormatter GetCustomFormatter(object arg) => new FormatInfo.CustomFormatter();
  }
}
