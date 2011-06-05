using System.IO;
using System.Resources;
using ActiproSoftware.Text;
using ActiproSoftware.Text.Implementation;
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
            using (var stream = new MemoryStream())
            {
                var orig = ((Stream) resource.Value);
                orig.CopyTo(stream);
                orig.Seek(0, SeekOrigin.Begin);
                stream.Seek(0, SeekOrigin.Begin);
                var xamlDecompiler = new BamlTranslator(stream);
                SourceCode = xamlDecompiler.ToString();
            }
        }
    }
}