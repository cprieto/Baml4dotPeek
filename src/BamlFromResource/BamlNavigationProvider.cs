using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.DataFlow;
using JetBrains.DotPeek.Features.Resources;
using JetBrains.ProjectModel.Model2.Assemblies.Interfaces;
using JetBrains.ReSharper.ExternalSources.ReSharperIntegration;
using JetBrains.ReSharper.Feature.Services.ExternalSources.Core;
using JetBrains.ReSharper.Feature.Services.Navigation;
using JetBrains.Util;
using Reflector.BamlViewer;

namespace Cprieto.DotPeek
{
  [NavigationProvider]
  public class BamlNavigationProvider : INavigationProvider
  {
    public const string DecompilerId = "Cprieto.baml";
    public const string PresentableName = "Baml Decompiler";
    private const string BamlExtension = ".baml";
    private const string XamlExtension = ".xaml";

    private readonly IDecompilationCache myDecompilationCache;
    private readonly ExternalSourcesNavigator myExternalSourcesNavigator;

    public BamlNavigationProvider(IDecompilationCache decompilationCache, ExternalSourcesNavigator externalSourcesNavigator)
    {
      myDecompilationCache = decompilationCache;
      myExternalSourcesNavigator = externalSourcesNavigator;
    }

    public IEnumerable<Type> GetSupportedTargetTypes()
    {
      yield return typeof(NetResourceEntryData);
    }

    public IEnumerable<INavigationPoint> CreateNavigationPoints(object target, IEnumerable<INavigationPoint> basePoints)
    {
      var navigationInfo = target as NetResourceEntryData;
      if (navigationInfo == null)
        return basePoints;

      string resourceName = navigationInfo.EntryName;
      if (!resourceName.EndsWith(BamlExtension, StringComparison.OrdinalIgnoreCase))
        return basePoints;

      string fileName = resourceName.TrimFromEnd(BamlExtension, StringComparison.OrdinalIgnoreCase) + XamlExtension;
      foreach (var invalidFileNameChar in FileSystemDefinition.InvalidFileNameChars)
        fileName = fileName.Replace(invalidFileNameChar, '_');

      IAssembly assembly = navigationInfo.Assembly;

      var cacheItem = myDecompilationCache.GetCacheItem(DecompilerId, assembly, resourceName, fileName);
      if (cacheItem == null || cacheItem.Expired)
      {
        ResourceNavigationUtil.ResourceNavigationData resource = ResourceNavigationUtil.ReadResource(navigationInfo);
        if (resource != null)
        {
          var content = CreateXaml(resource);

          if (content != null)
          {
            cacheItem = myDecompilationCache.PutCacheItem(DecompilerId, assembly, resourceName, fileName,
                                                          new Dictionary<string, string>(), content);
          }
        }
      }

      if (cacheItem == null)
        return basePoints;

      return new[] { myExternalSourcesNavigator.CreateNavigationPoint(cacheItem.Location, new TextRange(0), assembly.FullAssemblyName, PresentableName) };
    }

    private string CreateXaml(ResourceNavigationUtil.ResourceNavigationData data)
    {
      try
      {
        using (var ms = new MemoryStream())
        {
          Lifetimes.Using(
            lt =>
              {
                using (var sourceStream = data.ReaderFactory(lt))
                  sourceStream.CopyTo(ms);
              });

          ms.Position = 0;
          var bamlTranslator = new BamlTranslator(ms);
          return bamlTranslator.ToString();
        }
      }
      catch (Exception e)
      {
        Logger.LogException(string.Format("Error while decoding {0} from {1}", data.Moniker, data.Assembly.FullAssemblyName), e);
      }

      return null;
    }

    public double Priority
    {
      get { return 0; }
    }
  }
}