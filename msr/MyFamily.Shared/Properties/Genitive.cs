// Decompiled with JetBrains decompiler
// Type: MyFamily.Shared.Properties.Genitive
// Assembly: MyFamily.Shared, Version=23.3.0.1570, Culture=neutral, PublicKeyToken=295e8b5bb13c2421
// MVID: CB662BCB-E11E-4698-B7B4-24BC7C8B503C
// Assembly location: C:\Users\GeorgePickworth\Documents\development\GenealogyTools\FTMContextNet\MyFamily.Shared.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace FTM.Dates.Properties
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class Genitive
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal Genitive()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (Genitive.resourceMan == null)
          Genitive.resourceMan = new ResourceManager("MyFamily.Shared.Properties.Genitive", typeof (Genitive).Assembly);
        return Genitive.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => Genitive.resourceCulture;
      set => Genitive.resourceCulture = value;
    }

    internal static string DEFAULT => Genitive.ResourceManager.GetString(nameof (DEFAULT), Genitive.resourceCulture);
  }
}
