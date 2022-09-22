// Decompiled with JetBrains decompiler
// Type: MyFamily.Shared.DateExtensions
// Assembly: MyFamily.Shared, Version=23.3.0.1570, Culture=neutral, PublicKeyToken=295e8b5bb13c2421
// MVID: CB662BCB-E11E-4698-B7B4-24BC7C8B503C
// Assembly location: C:\Users\GeorgePickworth\Documents\development\GenealogyTools\FTMContextNet\MyFamily.Shared.dll

using FTM.Dates.Interfaces;

namespace FTM.Dates
{
  public static class DateExtensions
  {
    public static bool HasYear(this Date dt) => (dt.Modifier & DateModifier.YearMissing) == DateModifier.None;

    public static bool HasMonth(this Date dt) => (dt.Modifier & DateModifier.MonthMissing) == DateModifier.None;

    public static bool HasDay(this Date dt) => (dt.Modifier & DateModifier.DayMissing) == DateModifier.None;
  }
}
