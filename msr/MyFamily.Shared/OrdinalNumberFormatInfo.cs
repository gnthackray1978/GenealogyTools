// Decompiled with JetBrains decompiler
// Type: MyFamily.Shared.OrdinalNumberFormatInfo
// Assembly: MyFamily.Shared, Version=23.3.0.1570, Culture=neutral, PublicKeyToken=295e8b5bb13c2421
// MVID: CB662BCB-E11E-4698-B7B4-24BC7C8B503C
// Assembly location: C:\Users\GeorgePickworth\Documents\development\GenealogyTools\FTMContextNet\MyFamily.Shared.dll

using System;
using System.Resources;

namespace FTM.Dates
{
  public class OrdinalNumberFormatInfo : CardinalNumberFormatInfo
  {
    public const string Numeric = "N";

    protected override string InnerFormat(
      string format,
      object arg,
      IFormatProvider formatProvider)
    {
      return arg is int num ? this.FormatOrdinal(format, num, formatProvider) : base.InnerFormat(format, arg, formatProvider);
    }

    private string FormatOrdinal(string format, int arg, IFormatProvider formatProvider)
    {
      if (format == "N")
        return this.FormatSymbol(arg);
      return format == "W" ? this.FormatWord(arg) : base.InnerFormat(format, (object) arg, formatProvider);
    }

    private string FormatSymbol(int arg)
    {
      string str;
      return this.TryFormatSymbol(arg, out str) ? str : arg.ToString();
    }

    private bool TryFormatSymbol(int arg, out string value)
    {
      ResourceManager resourceManager = FTM.Dates.Properties.Resources.ResourceManager;
      value = (string) null;
      value = resourceManager.GetString("OrdinalSymbol_" + arg.ToString());
      if (value != null)
        return true;
      string name1 = "OrdinalSymbol_" + (arg % 100).ToString("00");
      value = resourceManager.GetString(name1);
      if (value != null)
      {
        value = ((arg / 100).ToString() + value).Trim();
        return true;
      }
      string name2 = "OrdinalSymbol_" + (arg % 10).ToString();
      value = resourceManager.GetString(name2);
      if (value != null)
      {
        value = ((arg / 10).ToString() + value).Trim();
        return true;
      }
      string format = resourceManager.GetString("OrdinalNumberSuffix_#");
      if (format == null)
        return false;
      value = string.Format(format, (object) arg);
      return true;
    }

    private string FormatWord(int arg)
    {
      string str;
      return this.TryFormatWord(arg, out str) ? str : arg.ToString();
    }

    private bool TryFormatWord(int arg, out string value)
    {
      ResourceManager resourceManager = FTM.Dates.Properties.Resources.ResourceManager;
      value = (string) null;
      value = resourceManager.GetString("OrdinalWord_" + arg.ToString());
      if (value != null)
        return true;
      string name1 = "OrdinalWord_" + (arg % 100).ToString("00");
      value = resourceManager.GetString(name1);
      if (value != null)
      {
        value = (string.Format((IFormatProvider) new CardinalNumberFormatInfo(), "{0:W}", (object) (arg - arg % 100)) + " " + value).Trim();
        return true;
      }
      string name2 = "OrdinalWord_" + (arg % 10).ToString();
      value = resourceManager.GetString(name2);
      if (value != null)
      {
        value = (string.Format((IFormatProvider) new CardinalNumberFormatInfo(), "{0:W}", (object) (arg - arg % 10)) + " " + value).Trim();
        return true;
      }
      string format = resourceManager.GetString("OrdinalWordSuffix_#");
      if (format == null)
        return false;
      value = string.Format((IFormatProvider) new CardinalNumberFormatInfo(), format, (object) arg);
      return true;
    }
  }
}
