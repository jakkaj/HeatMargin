using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace HeatMarginExtension
{
    #region HeatMarginExtension Factory
    /// <summary>
    /// Export a <see cref="IWpfTextViewMarginProvider"/>, which returns an instance of the margin for the editor
    /// to use.
    /// </summary>

    [Export(typeof(IWpfTextViewMarginProvider))]
    [Name(HeatMarginExtension.MarginScrollerName + "2013")]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    [MarginContainer(PredefinedMarginNames.VerticalScrollBar)]
    [Order(After = "OverviewChangeTrackingMargin")]
    [Order(Before = "OverviewErrorMargin")]
    [Order(Before = "OverviewMarkMargin")]
    [Order(Before = "OverviewSourceImageMargin")]
    internal sealed class MarginScrollerFactory : IWpfTextViewMarginProvider
    {
        public IWpfTextViewMargin CreateMargin(IWpfTextViewHost textViewHost, IWpfTextViewMargin containerMargin)
        {
            return new HeatMarginExtension(textViewHost.TextView, containerMargin);
        }
    }
    #endregion
}
