// Decompiled with JetBrains decompiler
// Type: MyFamily.Shared.Properties.DateParserStrings
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
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class DateParserStrings
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal DateParserStrings()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (DateParserStrings.resourceMan == null)
          DateParserStrings.resourceMan = new ResourceManager("MyFamily.Shared.Properties.DateParserStrings", typeof (DateParserStrings).Assembly);
        return DateParserStrings.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => DateParserStrings.resourceCulture;
      set => DateParserStrings.resourceCulture = value;
    }

    internal static string About => DateParserStrings.ResourceManager.GetString(nameof (About), DateParserStrings.resourceCulture);

    internal static string AD => DateParserStrings.ResourceManager.GetString(nameof (AD), DateParserStrings.resourceCulture);

    internal static string After => DateParserStrings.ResourceManager.GetString(nameof (After), DateParserStrings.resourceCulture);

    internal static string And => DateParserStrings.ResourceManager.GetString(nameof (And), DateParserStrings.resourceCulture);

    internal static string BC => DateParserStrings.ResourceManager.GetString(nameof (BC), DateParserStrings.resourceCulture);

    internal static string BCE => DateParserStrings.ResourceManager.GetString(nameof (BCE), DateParserStrings.resourceCulture);

    internal static string Before => DateParserStrings.ResourceManager.GetString(nameof (Before), DateParserStrings.resourceCulture);

    internal static string Between => DateParserStrings.ResourceManager.GetString(nameof (Between), DateParserStrings.resourceCulture);

    internal static string Calculated => DateParserStrings.ResourceManager.GetString(nameof (Calculated), DateParserStrings.resourceCulture);

    internal static string CE => DateParserStrings.ResourceManager.GetString(nameof (CE), DateParserStrings.resourceCulture);

    internal static string Or => DateParserStrings.ResourceManager.GetString(nameof (Or), DateParserStrings.resourceCulture);

    internal static string Quarter => DateParserStrings.ResourceManager.GetString(nameof (Quarter), DateParserStrings.resourceCulture);

    internal static string Range => DateParserStrings.ResourceManager.GetString(nameof (Range), DateParserStrings.resourceCulture);

    internal static string To => DateParserStrings.ResourceManager.GetString(nameof (To), DateParserStrings.resourceCulture);
  }
}
