using System.ComponentModel;
using HeatMarginExtension.Infrastructure;
using Microsoft.VisualStudio.Text.Formatting;

namespace HeatMarginExtension.View
{
    public class HeatMarginViewModel : ViewModel
    {
        private string _lineTest;

        public string LineTest
        {
            get { return _lineTest; }
            set { _lineTest = value; }
        }

        public void LineUpdated(ITextViewLine line)
        {
            var snap = line.Snapshot;
        }
    }
}
