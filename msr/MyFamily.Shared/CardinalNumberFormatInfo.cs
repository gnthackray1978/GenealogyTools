// Decompiled with JetBrains decompiler
// Type: MyFamily.Shared.CardinalNumberFormatInfo
// Assembly: MyFamily.Shared, Version=23.3.0.1570, Culture=neutral, PublicKeyToken=295e8b5bb13c2421
// MVID: CB662BCB-E11E-4698-B7B4-24BC7C8B503C
// Assembly location: C:\Users\GeorgePickworth\Documents\development\GenealogyTools\FTMContextNet\MyFamily.Shared.dll

using System;
using System.Resources;

namespace FTM.Dates
{
  public class CardinalNumberFormatInfo : FormatInfo
  {
    public const string Word = "W";

    protected override string InnerFormat(
      string format,
      object arg,
      IFormatProvider formatProvider)
    {
      return arg is int num ? this.FormatCardinal(format, num, formatProvider) : base.InnerFormat(format, arg, formatProvider);
    }

    private string FormatCardinal(string format, int arg, IFormatProvider formatProvider) => format == "G" || !(format == "W") ? arg.ToString() : this.FormatWord(arg);

    private string FormatWord(int arg)
    {
      string str;
      return this.TryFormatWord(arg, out str) ? str : arg.ToString();
    }

    private bool TryFormatWord(int arg, out string value)
    {
      ResourceManager resourceManager = FTM.Dates.Properties.Resources.ResourceManager;
      value = (string) null;
      value = resourceManager.GetString("CardinalWord_" + arg.ToString());
      if (value != null)
        return true;
      if (1000000000 <= arg && arg <= int.MaxValue)
      {
        string format = resourceManager.GetString("CardinalWord_#000000000");
        value = string.Format((IFormatProvider) this, format, (object) ((arg - arg % 1000000000) / 1000000000), (object) (arg % 1000000000));
      }
      else if (1000000 <= arg && arg < 1000000000)
      {
        string format = resourceManager.GetString("CardinalWord_#000000");
        value = string.Format((IFormatProvider) this, format, (object) ((arg - arg % 1000000) / 1000000), (object) (arg % 1000000));
      }
      else if (1000 <= arg && arg < 1000000)
      {
        string format = resourceManager.GetString("CardinalWord_#000");
        value = string.Format((IFormatProvider) this, format, (object) ((arg - arg % 1000) / 1000), (object) (arg % 1000));
      }
      else if (100 <= arg && arg < 1000)
      {
        string format = resourceManager.GetString("CardinalWord_#00");
        value = string.Format((IFormatProvider) this, format, (object) ((arg - arg % 100) / 100), (object) (arg % 100));
      }
      else if (10 <= arg && arg < 100)
      {
        string format = resourceManager.GetString("CardinalWord_#0");
        value = string.Format((IFormatProvider) this, format, (object) (arg - arg % 10), (object) (arg % 10));
      }
      if (value != null)
        value = value.Trim();
      return value != null;
    }
  }
}
