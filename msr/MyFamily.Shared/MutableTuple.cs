// Decompiled with JetBrains decompiler
// Type: MyFamily.Shared.MutableTuple`2
// Assembly: MyFamily.Shared, Version=23.3.0.1570, Culture=neutral, PublicKeyToken=295e8b5bb13c2421
// MVID: CB662BCB-E11E-4698-B7B4-24BC7C8B503C
// Assembly location: C:\Users\GeorgePickworth\Documents\development\GenealogyTools\FTMContextNet\MyFamily.Shared.dll

using System;

namespace FTM.Dates
{
  public class MutableTuple<T1, T2>
  {
    public MutableTuple(T1 val1, T2 val2)
    {
      this.Item1 = val1;
      this.Item2 = val2;
    }

    public T1 Item1 { get; set; }

    public T2 Item2 { get; set; }

    public override bool Equals(object obj)
    {
      if (obj == null || this.GetType() != obj.GetType())
        return false;
      MutableTuple<T1, T2> mutableTuple = obj as MutableTuple<T1, T2>;
      return object.Equals((object) this.Item1, (object) mutableTuple.Item1) && object.Equals((object) this.Item2, (object) mutableTuple.Item2);
    }

    public override int GetHashCode()
    {
      int hashCode = 0;
      if ((object) this.Item1 is ValueType || (object) this.Item1 != null)
        hashCode = this.Item1.GetHashCode();
      if ((object) this.Item2 is ValueType || (object) this.Item2 != null)
        hashCode += this.Item2.GetHashCode();
      return hashCode;
    }
  }
}
