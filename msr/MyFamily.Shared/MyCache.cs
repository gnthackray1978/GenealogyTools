// Decompiled with JetBrains decompiler
// Type: MyFamily.Shared.MyCache`1
// Assembly: MyFamily.Shared, Version=23.3.0.1570, Culture=neutral, PublicKeyToken=295e8b5bb13c2421
// MVID: CB662BCB-E11E-4698-B7B4-24BC7C8B503C
// Assembly location: C:\Users\GeorgePickworth\Documents\development\GenealogyTools\FTMContextNet\MyFamily.Shared.dll

using System.Collections.Generic;

namespace FTM.Dates
{
  public abstract class MyCache<T>
  {
    public abstract T this[string key] { get; }

    public virtual void Add(string key, T value) => this.Add(key, value);

    //public virtual void Add(string key, T value, CacheItemPriority priority) => this.Add(key, value, priority, (ICacheItemRefreshAction) null);

    //public abstract void Add(
    //  string key,
    //  T value,
    //  CacheItemPriority priority,
    //  ICacheItemRefreshAction refreshAction,
    //  params ICacheItemExpiration[] expirations);

    public abstract T Remove(string key);

    public void Flush() => this.Flush(false);

    public abstract void Flush(bool dispose);

    public abstract IList<string> GetKeys();
  }
}
