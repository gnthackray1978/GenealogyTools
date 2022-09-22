// Decompiled with JetBrains decompiler
// Type: MyFamily.Shared.DateValidator
// Assembly: MyFamily.Shared, Version=23.3.0.1570, Culture=neutral, PublicKeyToken=295e8b5bb13c2421
// MVID: CB662BCB-E11E-4698-B7B4-24BC7C8B503C
// Assembly location: C:\Users\GeorgePickworth\Documents\development\GenealogyTools\FTMContextNet\MyFamily.Shared.dll

using FTM.Dates.Interfaces;
using System;

namespace FTM.Dates
{
  public static class DateValidator
  {
    public static Tuple<Date, DateRange> Validate(Date inDate)
    {
      int? nullable;
      Date date;
      if (inDate != null)
      {
        nullable = inDate.Year;
        if (!nullable.HasValue)
        {
          nullable = inDate.Month;
          if (!nullable.HasValue)
          {
            nullable = inDate.Day;
            if (!nullable.HasValue)
              goto label_4;
          }
        }
        date = inDate;
        goto label_6;
      }
label_4:
      date = Date.CreateInstance(new uint?(SDNDate.Encode(new int?(SDNDate.MaxYear), new int?(12), new int?(31), DateModifier.None)));
label_6:
      DateRange dateRange = date as DateRange;
      nullable = date.Year;
      if (!nullable.HasValue)
        date = Date.CreateInstance(new uint?(SDNDate.Encode(new int?(SDNDate.MaxYear), date.Month, date.Day, DateModifier.None)));
      return new Tuple<Date, DateRange>(date, dateRange);
    }
  }
}
