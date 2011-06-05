using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;
using JetBrains.DotPeek.AssemblyExplorer.Content;
using JetBrains.IDE.TreeBrowser;
using JetBrains.TreeModels;
using JetBrains.UI.TreeView;
using Reflector.BamlViewer;

namespace Cprieto.DotPeek
{
    [ActionHandler("Cprieto.ShowBamlResources")]
    public class BamlActionHandler : IActionHandler
    {
        public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
        {
            var resource = GetResourceNode(context);
            if (resource == null)
            {
                presentation.Visible = false;
                return false;
            }

            using (var stream = resource.ResourceDisposition.CreateResourceReader())
                return HasBamlFiles(stream);
        }

        public bool HasBamlFiles(Stream files)
        {
            var reader = new ResourceReader(files);
            return reader.Resources.Length > 0 && reader.Resources.Any(r => r.Name.EndsWith(".baml"));
        }

        public void Execute(IDataContext context, DelegateExecute nextExecute)
        {
            System.Diagnostics.Trace.WriteLine("*** Hello!!! ***");
        }

        private ResourceNode GetResourceNode(IDataContext context)
        {
            if (!(context.GetData(TreeModelBrowser.TREE_MODEL_DESCRIPTOR) is AssemblyExplorerDescriptor))
            {
                return null;
            }
            
            var data = context.GetData(TreeModelBrowser.TREE_MODEL_NODES);
            if (data == null || data.Count != 1)
                return null;
            return data[0].DataValue as ResourceNode;
        }
    }
}