using JetBrains.DotPeek.ReSharper;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ExternalSources.Core;

namespace Cprieto.DotPeek
{
  [SolutionComponent]
  public class BamlExternalSourcesProvider : BindExternalSourcesProviderBase
  {
    public BamlExternalSourcesProvider(IDecompilationCache decompilationCache)
      : base(decompilationCache)
    {
    }

    public override string PresentableShortName
    {
      get { return BamlNavigationProvider.PresentableName; }
    }

    public override string Id
    {
      get { return BamlNavigationProvider.DecompilerId; }
    }
  }
}