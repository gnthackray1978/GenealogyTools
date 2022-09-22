// Decompiled with JetBrains decompiler
// Type: MyFamily.Shared.Interfaces.IText
// Assembly: MyFamily.Shared, Version=23.3.0.1570, Culture=neutral, PublicKeyToken=295e8b5bb13c2421
// MVID: CB662BCB-E11E-4698-B7B4-24BC7C8B503C
// Assembly location: C:\Users\GeorgePickworth\Documents\development\GenealogyTools\FTMContextNet\MyFamily.Shared.dll

using System;

namespace FTM.Dates.Interfaces
{
  public interface IText : IFormattableEx, IFormattable
  {
    string Text { get; set; }
  }
}
