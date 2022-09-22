// Decompiled with JetBrains decompiler
// Type: MyFamily.Shared.Text
// Assembly: MyFamily.Shared, Version=23.3.0.1570, Culture=neutral, PublicKeyToken=295e8b5bb13c2421
// MVID: CB662BCB-E11E-4698-B7B4-24BC7C8B503C
// Assembly location: C:\Users\GeorgePickworth\Documents\development\GenealogyTools\FTMContextNet\MyFamily.Shared.dll

using FTM.Dates.Interfaces;
using System;

namespace FTM.Dates
{
  public class Text : IText, IFormattableEx, IFormattable
  {
    private string _text;
    private FormatInfo _formatter = (FormatInfo) new TextFormatInfo();

    public Text(string text) => this._text = text;

    string IText.Text
    {
      get => this._text;
      set => this._text = value;
    }

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
