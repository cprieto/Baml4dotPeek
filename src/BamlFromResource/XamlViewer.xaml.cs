using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ActiproSoftware.Text;
using ActiproSoftware.Text.Implementation;

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
