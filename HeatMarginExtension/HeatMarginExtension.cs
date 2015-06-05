using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using HeatMarginExtension.View;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;


namespace HeatMarginExtension
{
    /// <summary>
    /// A class detailing the margin's visual definition including both size and content.
    /// </summary>
    class HeatMarginExtension : Canvas, IWpfTextViewMargin
    {
        public const string MarginName = "HeatMargin";
        public const string MarginScrollerName = "HeatMarginScroller";
        private IWpfTextView _textView;
        private bool _isDisposed = false;



        private HeatMarginViewModel _viewModel;
        private UserControl _userControl;

        private IVerticalScrollBar _scrollBar;

        /// <summary>
        /// Creates a <see cref="HeatMarginExtension"/> for a given <see cref="IWpfTextView"/>.
        /// </summary>
        /// <param name="textView">The <see cref="IWpfTextView"/> to attach the margin to.</param>
        /// <param name="buffer"></param>
        public HeatMarginExtension(IWpfTextView textView, IWpfTextViewMargin margin)
        {
            _textView = textView;

            var scrollBarMargin = margin.GetTextViewMargin(PredefinedMarginNames.VerticalScrollBar);
            // ReSharper disable once SuspiciousTypeConversion.Global
            
            _scrollBar = (IVerticalScrollBar)scrollBarMargin;

            _viewModel = new HeatMarginViewModel(textView, _scrollBar);
            _userControl = new HeatMarginView();
            _userControl.DataContext = _viewModel;
            
            var buffer = _textView.TextBuffer;

            _textView.LayoutChanged += _textView_LayoutChanged;
            buffer.Changed += buffer_Changed;
        }

        void _textView_LayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            _viewModel.RefreshLines();
        }

        void buffer_Changed(object sender, TextContentChangedEventArgs e)
        {
            var snapShot = _textView.TextSnapshot;

            foreach (var item in e.Changes)
            {
                _viewModel.LineDelta(item.NewPosition, item.LineCountDelta);

                if (snapShot.Length < item.NewPosition)
                {
                    continue;
                }

                var lineStart = snapShot.GetLineFromPosition(item.NewPosition);
                var actualLineStart = _textView.GetTextViewLineContainingBufferPosition(lineStart.Start);
                _viewModel.LineUpdated(actualLineStart);
               
                //Process lines in between start and end
                for (var i = lineStart.LineNumber + 1; i <= lineStart.LineNumber + item.LineCountDelta; i++)
                {
                    var lineNumber = i;
                    
                    while (lineNumber >= snapShot.LineCount)
                    {
                        lineNumber--;
                    }

                    var lineBetween = snapShot.GetLineFromLineNumber(lineNumber);
                    var actualLineBetween = _textView.GetTextViewLineContainingBufferPosition(lineBetween.End);
                    _viewModel.LineUpdated(actualLineBetween);
                }
            }
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(MarginName);
        }

        #region IWpfTextViewMargin Members

        /// <summary>
        /// The <see cref="FrameworkElement"/> that implements the visual representation
        /// of the margin.
        /// </summary>
        public FrameworkElement VisualElement
        {
            // Since this margin implements Canvas, this is the object which renders
            // the margin.
            get
            {
                ThrowIfDisposed();
                return _userControl;
            }
        }

        #endregion

        #region ITextViewMargin Members

        public double MarginSize
        {
            // Since this is a horizontal margin, its width will be bound to the width of the text view.
            // Therefore, its size is its height.
            get
            {
                ThrowIfDisposed();
                return this.ActualHeight;
            }
        }

        public bool Enabled
        {
            // The margin should always be enabled
            get
            {
                ThrowIfDisposed();
                return true;
            }
        }

        /// <summary>
        /// Returns an instance of the margin if this is the margin that has been requested.
        /// </summary>
        /// <param name="marginName">The name of the margin requested</param>
        /// <returns>An instance of HeatMargin or null</returns>
        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            return (marginName == MarginName) ? (IWpfTextViewMargin)this : null;
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _viewModel.Dispose();
                GC.SuppressFinalize(this);
                _isDisposed = true;
            }
        }
        #endregion
    }
}
