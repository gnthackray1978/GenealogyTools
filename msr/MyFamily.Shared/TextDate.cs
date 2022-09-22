// Decompiled with JetBrains decompiler
// Type: MyFamily.Shared.TextDate
// Assembly: MyFamily.Shared, Version=23.3.0.1570, Culture=neutral, PublicKeyToken=295e8b5bb13c2421
// MVID: CB662BCB-E11E-4698-B7B4-24BC7C8B503C
// Assembly location: C:\Users\GeorgePickworth\Documents\development\GenealogyTools\FTMContextNet\MyFamily.Shared.dll

namespace FTM.Dates
{
  public class TextDate : Date
  {
    private string _text;

    internal TextDate(string text)
      : base(new uint?())
    {
      this._text = text;
    }

    public override uint SortDate => uint.MaxValue;

    public string Text => this._text;

    public override bool Equals(Date date) => date is TextDate && this._text == ((TextDate) date)._text;
  }
}
