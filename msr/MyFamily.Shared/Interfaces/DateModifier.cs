// Decompiled with JetBrains decompiler
// Type: MyFamily.Shared.Interfaces.DateModifier
// Assembly: MyFamily.Shared, Version=23.3.0.1570, Culture=neutral, PublicKeyToken=295e8b5bb13c2421
// MVID: CB662BCB-E11E-4698-B7B4-24BC7C8B503C
// Assembly location: C:\Users\GeorgePickworth\Documents\development\GenealogyTools\FTMContextNet\MyFamily.Shared.dll

using System;

namespace FTM.Dates.Interfaces
{
  [Flags]
  public enum DateModifier
  {
    None = 0,
    Before = 1,
    After = 2,
    About = After | Before, // 0x00000003
    Quarter = 8,
    YearMissing = 32, // 0x00000020
    MonthMissing = 64, // 0x00000040
    DayMissing = 128, // 0x00000080
    DoubleDate = 16, // 0x00000010
    Calculated = 256, // 0x00000100
  }
}
