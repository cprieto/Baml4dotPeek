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
        public ISyntaxLanguage Language { get; set; }

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

            Language = GetEmbeddedLanguageSyntax();
        }

        private ISyntaxLanguage GetEmbeddedLanguageSyntax()
        {
            var assembly = GetType().Assembly;
            var xamlDef = assembly.GetManifestResourceStream("Cprieto.DotPeek.Xaml.langdef");
            
            var serializer = new SyntaxLanguageDefinitionSerializer();
            return serializer.LoadFromStream(xamlDef);
        }
    }
}