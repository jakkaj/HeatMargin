using HeatMargin.Infrastructure;
using Microsoft.VisualStudio.Text.Formatting;

namespace HeatMargin.View
{
    public class HeatMarginViewModel : ViewModel
    {

        public void LineUpdated(ITextViewLine line)
        {
            var snap = line.Snapshot;
        }
    }
}
