// Decompiled with JetBrains decompiler
// Type: MyFamily.Shared.SerialDayNumber
// Assembly: MyFamily.Shared, Version=23.3.0.1570, Culture=neutral, PublicKeyToken=295e8b5bb13c2421
// MVID: CB662BCB-E11E-4698-B7B4-24BC7C8B503C
// Assembly location: C:\Users\GeorgePickworth\Documents\development\GenealogyTools\FTMContextNet\MyFamily.Shared.dll

using FTM.Dates.Properties;

namespace FTM.Dates
{
  public static class SerialDayNumber
  {
    private static readonly int GREG_SDN_OFFSET = 32045;
    private static readonly int GREG_DAYS_PER_5_MONTHS = 153;
    private static readonly int GREG_DAYS_PER_4_YEARS = 1461;
    private static readonly int GREG_DAYS_PER_400_YEARS = 146097;
    public static readonly string[] WeekDays = new string[7]
    {
      Resources.WeekDaysSunday,
      Resources.WeekDaysMonday,
      Resources.WeekDaysTuesday,
      Resources.WeekDaysWednesday,
      Resources.WeekDaysThursday,
      Resources.WeekDaysFriday,
      Resources.WeekDaysSaturday
    };
    private static readonly int JUL_SDN_OFFSET = 32083;
    private static readonly int JUL_DAYS_PER_5_MONTHS = 153;
    private static readonly int JUL_DAYS_PER_4_YEARS = 1461;
    private static readonly int HALAKIM_PER_HOUR = 1080;
    private static readonly int HALAKIM_PER_DAY = 25920;
    private static readonly int HALAKIM_PER_LUNAR_CYCLE = 29 * SerialDayNumber.HALAKIM_PER_DAY + 13753;
    private static readonly int HALAKIM_PER_METONIC_CYCLE = SerialDayNumber.HALAKIM_PER_LUNAR_CYCLE * 235;
    private static readonly int SDN_OFFSET = 347997;
    private static readonly int NEW_MOON_OF_CREATION = 31524;
    private static readonly int SUNDAY = 0;
    private static readonly int MONDAY = 1;
    private static readonly int TUESDAY = 2;
    private static readonly int WEDNESDAY = 3;
    private static readonly int FRIDAY = 5;
    private static readonly int NOON = 18 * SerialDayNumber.HALAKIM_PER_HOUR;
    private static readonly int AM3_11_20 = 9 * SerialDayNumber.HALAKIM_PER_HOUR + 204;
    private static readonly int AM9_32_43 = 15 * SerialDayNumber.HALAKIM_PER_HOUR + 589;
    private static int[] monthsPerYear = new int[19]
    {
      12,
      12,
      13,
      12,
      12,
      13,
      12,
      13,
      12,
      12,
      13,
      12,
      12,
      13,
      12,
      12,
      13,
      12,
      13
    };
    private static int[] yearOffset = new int[19]
    {
      0,
      12,
      24,
      37,
      49,
      61,
      74,
      86,
      99,
      111,
      123,
      136,
      148,
      160,
      173,
      185,
      197,
      210,
      222
    };
    private static string[] JewishMonthName = new string[14]
    {
      "",
      "Tishri",
      "Heshvan",
      "Kislev",
      "Tevet",
      "Shevat",
      "AdarI",
      "AdarII",
      "Nisan",
      "Iyyar",
      "Sivan",
      "Tammuz",
      "Av",
      "Elul"
    };

    public static int DayOfWeek(int sdn) => (sdn + 1) % 7;

    public static void SdnToGregorian(int sdn, out int year, out int month, out int day)
    {
      year = 0;
      month = 0;
      day = 0;
      if (sdn <= 0)
        return;
      int num1 = (sdn + SerialDayNumber.GREG_SDN_OFFSET) * 4 - 1;
      int num2 = num1 / SerialDayNumber.GREG_DAYS_PER_400_YEARS;
      int num3 = num1 % SerialDayNumber.GREG_DAYS_PER_400_YEARS / 4 * 4 + 3;
      year = num2 * 100 + num3 / SerialDayNumber.GREG_DAYS_PER_4_YEARS;
      int num4 = (num3 % SerialDayNumber.GREG_DAYS_PER_4_YEARS / 4 + 1) * 5 - 3;
      month = num4 / SerialDayNumber.GREG_DAYS_PER_5_MONTHS;
      day = num4 % SerialDayNumber.GREG_DAYS_PER_5_MONTHS / 5 + 1;
      if (month < 10)
      {
        month += 3;
      }
      else
      {
        ++year;
        month -= 9;
      }
      year -= 4800;
      if (year > 0)
        return;
      --year;
    }

    public static int GregorianToSdn(int inputYear, int inputMonth, int inputDay)
    {
      if (inputYear == 0 || inputYear < -4714 || inputMonth <= 0 || inputMonth > 12 || inputDay <= 0 || inputDay > 31 || inputYear == -4714 && (inputMonth < 11 || inputMonth == 11 && inputDay < 25))
        return 0;
      int num1 = inputYear >= 0 ? inputYear + 4800 : inputYear + 4801;
      int num2;
      if (inputMonth > 2)
      {
        num2 = inputMonth - 3;
      }
      else
      {
        num2 = inputMonth + 9;
        --num1;
      }
      return num1 / 100 * SerialDayNumber.GREG_DAYS_PER_400_YEARS / 4 + num1 % 100 * SerialDayNumber.GREG_DAYS_PER_4_YEARS / 4 + (num2 * SerialDayNumber.GREG_DAYS_PER_5_MONTHS + 2) / 5 + inputDay - SerialDayNumber.GREG_SDN_OFFSET;
    }

    public static void SdnToJulian(int sdn, out int year, out int month, out int day)
    {
      year = 0;
      month = 0;
      day = 0;
      if (sdn <= 0)
        return;
      int num1 = (sdn + SerialDayNumber.JUL_SDN_OFFSET) * 4 - 1;
      year = num1 / SerialDayNumber.JUL_DAYS_PER_4_YEARS;
      int num2 = (num1 % SerialDayNumber.JUL_DAYS_PER_4_YEARS / 4 + 1) * 5 - 3;
      month = num2 / SerialDayNumber.JUL_DAYS_PER_5_MONTHS;
      day = num2 % SerialDayNumber.JUL_DAYS_PER_5_MONTHS / 5 + 1;
      if (month < 10)
      {
        month += 3;
      }
      else
      {
        ++year;
        month -= 9;
      }
      year -= 4800;
      if (year > 0)
        return;
      --year;
    }

    public static int JulianToSdn(int inputYear, int inputMonth, int inputDay)
    {
      if (inputYear == 0 || inputYear < -4713 || inputMonth <= 0 || inputMonth > 12 || inputDay <= 0 || inputDay > 31 || inputYear == -4713 && inputMonth == 1 && inputDay == 1)
        return 0;
      int num1 = inputYear >= 0 ? inputYear + 4800 : inputYear + 4801;
      int num2;
      if (inputMonth > 2)
      {
        num2 = inputMonth - 3;
      }
      else
      {
        num2 = inputMonth + 9;
        --num1;
      }
      return num1 * SerialDayNumber.JUL_DAYS_PER_4_YEARS / 4 + (num2 * SerialDayNumber.JUL_DAYS_PER_5_MONTHS + 2) / 5 + inputDay - SerialDayNumber.JUL_SDN_OFFSET;
    }

    private static int Tishri1(int metonicYear, int moladDay, int moladHalakim)
    {
      int num1 = moladDay;
      int num2 = num1 % 7;
      bool flag1 = metonicYear == 2 || metonicYear == 5 || metonicYear == 7 || metonicYear == 10 || metonicYear == 13 || metonicYear == 16 || metonicYear == 18;
      bool flag2 = metonicYear == 3 || metonicYear == 6 || metonicYear == 8 || metonicYear == 11 || metonicYear == 14 || metonicYear == 17 || metonicYear == 0;
      if (moladHalakim >= SerialDayNumber.NOON || !flag1 && num2 == SerialDayNumber.TUESDAY && moladHalakim >= SerialDayNumber.AM3_11_20 || flag2 && num2 == SerialDayNumber.MONDAY && moladHalakim >= SerialDayNumber.AM9_32_43)
      {
        ++num1;
        ++num2;
        if (num2 == 7)
          num2 = 0;
      }
      if (num2 == SerialDayNumber.WEDNESDAY || num2 == SerialDayNumber.FRIDAY || num2 == SerialDayNumber.SUNDAY)
        ++num1;
      return num1;
    }

    private static void MoladOfMetonicCycle(
      int metonicCycle,
      out int moladDay,
      out int moladHalakim)
    {
      int num1 = SerialDayNumber.NEW_MOON_OF_CREATION + metonicCycle * (SerialDayNumber.HALAKIM_PER_METONIC_CYCLE & (int) ushort.MaxValue);
      int num2 = (num1 >> 16) + metonicCycle * (SerialDayNumber.HALAKIM_PER_METONIC_CYCLE >> 16 & (int) ushort.MaxValue);
      int num3 = num2 / SerialDayNumber.HALAKIM_PER_DAY;
      int num4 = num2 - num3 * SerialDayNumber.HALAKIM_PER_DAY << 16 | num1 & (int) ushort.MaxValue;
      int num5 = num4 / SerialDayNumber.HALAKIM_PER_DAY;
      int num6 = num4 - num5 * SerialDayNumber.HALAKIM_PER_DAY;
      moladDay = num3 << 16 | num5;
      moladHalakim = num6;
    }

    private static void FindTishriMolad(
      int inputDay,
      out int metonicCycle,
      out int metonicYear,
      out int moladDay,
      out int moladHalakim)
    {
      metonicCycle = (inputDay + 310) / 6940;
      SerialDayNumber.MoladOfMetonicCycle(metonicCycle, out moladDay, out moladHalakim);
      while (moladDay < inputDay - 6940 + 310)
      {
        ++metonicCycle;
        moladHalakim += SerialDayNumber.HALAKIM_PER_METONIC_CYCLE;
        moladDay += moladHalakim / SerialDayNumber.HALAKIM_PER_DAY;
        moladHalakim %= SerialDayNumber.HALAKIM_PER_DAY;
      }
      metonicYear = 0;
      while (metonicYear < 18 && moladDay <= inputDay - 74)
      {
        moladHalakim += SerialDayNumber.HALAKIM_PER_LUNAR_CYCLE * SerialDayNumber.monthsPerYear[metonicYear];
        moladDay += moladHalakim / SerialDayNumber.HALAKIM_PER_DAY;
        moladHalakim %= SerialDayNumber.HALAKIM_PER_DAY;
        ++metonicYear;
      }
    }

    private static void FindStartOfYear(
      int year,
      out int pMetonicCycle,
      out int pMetonicYear,
      out int pMoladDay,
      out int pMoladHalakim,
      out int pTishri1)
    {
      pMetonicCycle = (year - 1) / 19;
      pMetonicYear = (year - 1) % 19;
      SerialDayNumber.MoladOfMetonicCycle(pMetonicCycle, out pMoladDay, out pMoladHalakim);
      pMoladHalakim += SerialDayNumber.HALAKIM_PER_LUNAR_CYCLE * SerialDayNumber.yearOffset[pMetonicYear];
      pMoladDay += pMoladHalakim / SerialDayNumber.HALAKIM_PER_DAY;
      pMoladHalakim %= SerialDayNumber.HALAKIM_PER_DAY;
      pTishri1 = SerialDayNumber.Tishri1(pMetonicYear, pMoladDay, pMoladHalakim);
    }

    public static void SdnToJewish(int sdn, out int pYear, out int pMonth, out int pDay)
    {
      pYear = 0;
      pMonth = 0;
      pDay = 0;
      if (sdn <= SerialDayNumber.SDN_OFFSET)
        return;
      int inputDay = sdn - SerialDayNumber.SDN_OFFSET;
      int metonicCycle;
      int metonicYear;
      int moladDay1;
      int moladHalakim;
      SerialDayNumber.FindTishriMolad(inputDay, out metonicCycle, out metonicYear, out moladDay1, out moladHalakim);
      int num1 = SerialDayNumber.Tishri1(metonicYear, moladDay1, moladHalakim);
      int num2;
      if (inputDay >= num1)
      {
        pYear = metonicCycle * 19 + metonicYear + 1;
        if (inputDay < num1 + 59)
        {
          if (inputDay < num1 + 30)
          {
            pMonth = 1;
            pDay = inputDay - num1 + 1;
            return;
          }
          pMonth = 2;
          pDay = inputDay - num1 - 29;
          return;
        }
        moladHalakim += SerialDayNumber.HALAKIM_PER_LUNAR_CYCLE * SerialDayNumber.monthsPerYear[metonicYear];
        int moladDay2 = moladDay1 + moladHalakim / SerialDayNumber.HALAKIM_PER_DAY;
        moladHalakim %= SerialDayNumber.HALAKIM_PER_DAY;
        num2 = SerialDayNumber.Tishri1((metonicYear + 1) % 19, moladDay2, moladHalakim);
      }
      else
      {
        pYear = metonicCycle * 19 + metonicYear;
        if (inputDay >= num1 - 177)
        {
          if (inputDay > num1 - 30)
          {
            pMonth = 13;
            pDay = inputDay - num1 + 30;
            return;
          }
          if (inputDay > num1 - 60)
          {
            pMonth = 12;
            pDay = inputDay - num1 + 60;
            return;
          }
          if (inputDay > num1 - 89)
          {
            pMonth = 11;
            pDay = inputDay - num1 + 89;
            return;
          }
          if (inputDay > num1 - 119)
          {
            pMonth = 10;
            pDay = inputDay - num1 + 119;
            return;
          }
          if (inputDay > num1 - 148)
          {
            pMonth = 9;
            pDay = inputDay - num1 + 148;
            return;
          }
          pMonth = 8;
          pDay = inputDay - num1 + 178;
          return;
        }
        if (SerialDayNumber.monthsPerYear[(pYear - 1) % 19] == 13)
        {
          pMonth = 7;
          pDay = inputDay - num1 + 207;
          if (pDay > 0)
            return;
          --pMonth;
          pDay += 30;
          if (pDay > 0)
            return;
          --pMonth;
          pDay += 30;
        }
        else
        {
          pMonth = 6;
          pDay = inputDay - num1 + 207;
          if (pDay > 0)
            return;
          --pMonth;
          pDay += 30;
        }
        if (pDay > 0)
          return;
        --pMonth;
        pDay += 29;
        if (pDay > 0)
          return;
        num2 = num1;
        SerialDayNumber.FindTishriMolad(moladDay1 - 365, out metonicCycle, out metonicYear, out moladDay1, out moladHalakim);
        num1 = SerialDayNumber.Tishri1(metonicYear, moladDay1, moladHalakim);
      }
      int num3 = num2 - num1;
      int num4 = inputDay - num1 - 29;
      int num5;
      if (num3 == 355 || num3 == 385)
      {
        if (num4 <= 30)
        {
          pMonth = 2;
          pDay = num4;
          return;
        }
        num5 = num4 - 30;
      }
      else
      {
        if (num4 <= 29)
        {
          pMonth = 2;
          pDay = num4;
          return;
        }
        num5 = num4 - 29;
      }
      pMonth = 3;
      pDay = num5;
    }

    public static int JewishToSdn(int year, int month, int day)
    {
      if (year <= 0 || day <= 0 || day > 30)
        return 0;
      int pMetonicCycle;
      int pMetonicYear1;
      int pMoladDay1;
      int pMoladHalakim1;
      int num1;
      switch (month)
      {
        case 1:
        case 2:
          int pTishri1_1;
          SerialDayNumber.FindStartOfYear(year, out pMetonicCycle, out pMetonicYear1, out pMoladDay1, out pMoladHalakim1, out pTishri1_1);
          num1 = month != 1 ? pTishri1_1 + day + 29 : pTishri1_1 + day - 1;
          break;
        case 3:
          int pMetonicYear2;
          int pMoladDay2;
          int pMoladHalakim2;
          int pTishri1_2;
          SerialDayNumber.FindStartOfYear(year, out pMetonicCycle, out pMetonicYear2, out pMoladDay2, out pMoladHalakim2, out pTishri1_2);
          int num2 = pMoladHalakim2 + SerialDayNumber.HALAKIM_PER_LUNAR_CYCLE * SerialDayNumber.monthsPerYear[pMetonicYear2];
          int moladDay = pMoladDay2 + num2 / SerialDayNumber.HALAKIM_PER_DAY;
          int moladHalakim = num2 % SerialDayNumber.HALAKIM_PER_DAY;
          switch (SerialDayNumber.Tishri1((pMetonicYear2 + 1) % 19, moladDay, moladHalakim) - pTishri1_2)
          {
            case 355:
            case 385:
              num1 = pTishri1_2 + day + 59;
              break;
            default:
              num1 = pTishri1_2 + day + 58;
              break;
          }
          break;
        case 4:
        case 5:
        case 6:
          int pTishri1_3;
          SerialDayNumber.FindStartOfYear(year + 1, out pMetonicCycle, out pMetonicYear1, out pMoladDay1, out pMoladHalakim1, out pTishri1_3);
          int num3 = SerialDayNumber.monthsPerYear[(year - 1) % 19] != 12 ? 59 : 29;
          switch (month)
          {
            case 4:
              num1 = pTishri1_3 + day - num3 - 237;
              break;
            case 5:
              num1 = pTishri1_3 + day - num3 - 208;
              break;
            default:
              num1 = pTishri1_3 + day - num3 - 178;
              break;
          }
          break;
        default:
          int pTishri1_4;
          SerialDayNumber.FindStartOfYear(year + 1, out pMetonicCycle, out pMetonicYear1, out pMoladDay1, out pMoladHalakim1, out pTishri1_4);
          switch (month - 7)
          {
            case 0:
              num1 = pTishri1_4 + day - 207;
              break;
            case 1:
              num1 = pTishri1_4 + day - 178;
              break;
            case 2:
              num1 = pTishri1_4 + day - 148;
              break;
            case 3:
              num1 = pTishri1_4 + day - 119;
              break;
            case 4:
              num1 = pTishri1_4 + day - 89;
              break;
            case 5:
              num1 = pTishri1_4 + day - 60;
              break;
            case 6:
              num1 = pTishri1_4 + day - 30;
              break;
            default:
              return 0;
          }
          break;
      }
      return num1 + SerialDayNumber.SDN_OFFSET;
    }
  }
}
