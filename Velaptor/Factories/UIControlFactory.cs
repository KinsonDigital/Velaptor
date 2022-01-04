using System.Diagnostics.CodeAnalysis;
using Velaptor.UI;

namespace Velaptor.Factories
{
    [ExcludeFromCodeCoverage]
    public static class UIControlFactory
    {
        public static Label CreateLabel(string labelText)
        {
            var contentLoader = ContentLoaderFactory.CreateContentLoader();
            var label = new Label(contentLoader);
            label.Text = labelText;

            return label;
        }
    }
}
