// Decompiled with JetBrains decompiler
// Type: MyFamily.Shared.FormatInfo
// Assembly: MyFamily.Shared, Version=23.3.0.1570, Culture=neutral, PublicKeyToken=295e8b5bb13c2421
// MVID: CB662BCB-E11E-4698-B7B4-24BC7C8B503C
// Assembly location: C:\Users\GeorgePickworth\Documents\development\GenealogyTools\FTMContextNet\MyFamily.Shared.dll

using FTM.Dates.Interfaces;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FTM.Dates
{
  public abstract class FormatInfo : 
    ICustomFormatterEx,
    ICustomFormatter,
    IFormatProvider,
    ICloneable
  {
    protected string defaultFormat = "G";
    private static Regex s_collapse = new Regex("\\s\\s+", RegexOptions.Compiled);
    private static Regex s_terms = new Regex("\\w+", RegexOptions.Compiled);
    private MyCache<string> _cache;
    private string _cacheKey;
    private static Dictionary<Type, FormatInfo> s_formaterMap = new Dictionary<Type, FormatInfo>();
    public const string General = "G";

    protected FormatInfo() => this.Initialize("G");

    protected FormatInfo(string defaultFormat) => this.Initialize(defaultFormat);

    //public static void RemoveFromCache<I>()
    //{
    //  Dictionary<Type, FormatInfo> formaterMap = FormatInfo.s_formaterMap;
    //  // ISSUE: explicit non-virtual call
    //  if ((formaterMap != null ? (__nonvirtual (formaterMap.ContainsKey(typeof (I))) ? 1 : 0) : 0) == 0)
    //    return;
    //  FormatInfo.s_formaterMap.Remove(typeof (I));
    //}

    public static T GetDefault<I, T>() where T : FormatInfo
    {
      if (!FormatInfo.s_formaterMap.ContainsKey(typeof (I)))
        FormatInfo.s_formaterMap[typeof (I)] = (FormatInfo) Activator.CreateInstance<T>();
      return FormatInfo.s_formaterMap[typeof (I)] as T;
    }

    public static FormatInfo SetDefault<I, T>(FormatInfo value) where T : FormatInfo
    {
      FormatInfo formatInfo = (FormatInfo) null;
      FormatInfo.s_formaterMap.TryGetValue(typeof (I), out formatInfo);
      FormatInfo.s_formaterMap[typeof (I)] = value;
      return formatInfo;
    }

    private void Initialize(string defaultFormat)
    {
      this._cacheKey = this.GetType().Name;
      this.defaultFormat = defaultFormat;
    }

    public MyCache<string> Cache
    {
      get => this._cache;
      set => this._cache = value;
    }

    protected virtual string GetCacheKey(object obj) => (string) null;

    public string Format(string format, object arg, IFormatProvider formatProvider)
    {
      if (arg == null)
        throw new ArgumentNullException(nameof (arg));
      IFormatProvider formatProvider1 = formatProvider == null ? (IFormatProvider) this : formatProvider;
      if (format == null)
        format = !(formatProvider1 is ICustomFormatterEx) ? "G" : (formatProvider1 as ICustomFormatterEx).DefaultFormat;
      MyCache<string> myCache = (MyCache<string>) null;
      string str1 = (string) null;
      string str2 = string.Empty;
      if (formatProvider1 is FormatInfo)
      {
        FormatInfo formatInfo = (FormatInfo) formatProvider1;
        myCache = formatInfo._cache;
        if (myCache != null)
          str1 = formatInfo.GetCacheKey(arg);
        str2 = formatInfo._cacheKey;
      }
      if (myCache == null || str1 == null)
        return this._InnerFormat(format, arg, formatProvider);
      string key = str2 + ":" + str1 + ":" + format;
      string str3 = myCache[key];
      if (str3 == null)
      {
        str3 = this._InnerFormat(format, arg, formatProvider);
        myCache.Add(key, str3);
      }
      return str3;
    }

    public virtual string DefaultFormat
    {
      get => this.defaultFormat;
      set => this.defaultFormat = value;
    }

    public object GetFormat(Type formatType) => formatType == typeof (ICustomFormatter) ? (object) this : (object) null;

    protected virtual string FormatCustom(string format, object arg) => FormatInfo.s_terms.Replace(format, new MatchEvaluator(this.GetCustomFormatter(arg).ReplaceTerm));

    protected virtual FormatInfo.CustomFormatter GetCustomFormatter(object arg) => new FormatInfo.CustomFormatter();

    protected string CollapseWhitespace(string s) => FormatInfo.s_collapse.Replace(s, new MatchEvaluator(this.WhitespaceReplace)).Trim();

    private string WhitespaceReplace(Match m) => m.Value.Contains("\n") ? "\n" : " ";

    public void FlushCache()
    {
      if (this._cache == null)
        return;
      this._cache.Flush();
    }

    protected virtual string InnerFormat(string format, object arg, IFormatProvider formatProvider) => formatProvider is FormatInfo ? (formatProvider as FormatInfo).FormatCustom(format, arg) : this.FormatCustom(format, arg);

    protected virtual string PostFormat(string s) => s;

    private string _InnerFormat(string format, object obj, IFormatProvider formatProvider)
    {
      string empty = string.Empty;
      object obj1 = formatProvider == null ? (object) null : formatProvider.GetFormat(typeof (ICustomFormatter));
      return this.PostFormat(obj1 != null ? (obj1 as ICustomFormatter).Format(format, obj, (IFormatProvider) null) : this.InnerFormat(format, obj, formatProvider));
    }

    public object Clone() => this.MemberwiseClone();

    public class CacheSwapper : IDisposable
    {
      private MyCache<string> _cache;
      private FormatInfo[] _formatters;
      private MyCache<string>[] _caches;

      public CacheSwapper(MyCache<string> cache, params FormatInfo[] formatters)
      {
        this._formatters = formatters;
        this._cache = cache;
        this._caches = new MyCache<string>[this._formatters.Length];
        for (int index = 0; index < formatters.Length; ++index)
        {
          this._caches[index] = formatters[index].Cache;
          formatters[index].Cache = cache;
        }
      }

      public void Dispose()
      {
        for (int index = 0; index < this._formatters.Length; ++index)
          this._formatters[index].Cache = this._caches[index];
      }
    }

    protected class CustomFormatter
    {
      public virtual string ReplaceTerm(string term) => term;

      internal string ReplaceTerm(Match m) => this.ReplaceTerm(m.Value);
    }
  }
}
