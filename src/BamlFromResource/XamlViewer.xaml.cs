using System.Windows.Controls;
using ActiproSoftware.Text;
using ActiproSoftware.Text.Implementation;
using ActiproSoftware.Windows.Controls.SyntaxEditor;

namespace Cprieto.DotPeek
{
    /// <summary>
    /// Interaction logic for XamlViewer.xaml
    /// </summary>
    public partial class XamlViewer : UserControl
    {
        public XamlViewer()
        {
            InitializeComponent();
            xmlEditor.Document.Language = GetEmbeddedLanguageSyntax();
            xmlEditor.CanScrollPastDocumentEnd = false;
            xmlEditor.Document.IsReadOnly = true;
            xmlEditor.IsLineNumberMarginVisible = true;
            xmlEditor.WordWrapMode = WordWrapMode.Word;
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
