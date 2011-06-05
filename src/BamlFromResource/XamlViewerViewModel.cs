using System.IO;
using Reflector.BamlViewer;

namespace Cprieto.DotPeek
{
    public class XamlViewerViewModel
    {
        public string SourceCode { get; private set; }
        public string FileName { get; private set; }

        public XamlViewerViewModel(Resource resource)
        {
            FileName = resource.Name;
            Process(resource);
        }

        private void Process(Resource resource)
        {
            using (var stream = (Stream) resource.Value)
            {
                var xamlDecompiler = new BamlTranslator(stream);
                SourceCode = xamlDecompiler.ToString();
            }
        }
    }
}