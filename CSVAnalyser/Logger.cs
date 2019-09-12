using System;

static internal class Logger
{
    public static void LogFinalProgress(int count)
    {
        Console.WriteLine("attempting to save " + count + " remaining records");
    }

    public static void LogFailure(int idx1, Exception e)
    {
        Console.WriteLine("failed:" + idx1 + " " + e.Message);
    }

    public static void LogProgress(int idx1, int count)
    {
        var percentage = (Convert.ToDecimal(idx1)/Convert.ToDecimal(count))*100;

        Console.WriteLine("Saved: " + percentage.ToString("F2") + "%");
    }
}