using System;
using System.Windows;
using System.Windows.Controls;
using HeatMargin.View;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace HeatMargin
{
    /// <summary>
    /// A class detailing the margin's visual definition including both size and content.
    /// </summary>
    class HeatMargin : Canvas, IWpfTextViewMargin
    {
        public const string MarginName = "HeatMargin";
        private IWpfTextView _textView;
        private bool _isDisposed = false;

       

        private HeatMarginViewModel _viewModel;
        private UserControl _userControl;

        /// <summary>
        /// Creates a <see cref="HeatMargin"/> for a given <see cref="IWpfTextView"/>.
        /// </summary>
        /// <param name="textView">The <see cref="IWpfTextView"/> to attach the margin to.</param>
        /// <param name="buffer"></param>
        public HeatMargin(IWpfTextView textView, IWpfTextViewMargin margin)
        {
            _textView = textView;
            
            _viewModel = new HeatMarginViewModel();
            _userControl = new HeatMarginView();
            _userControl.DataContext = _viewModel;
            
         //   buffer.Changing+=buffer_Changing;
        }

        void buffer_Changing(object sender, TextContentChangingEventArgs e)
        {
            var line = _textView.Caret.ContainingTextViewLine;

           // var lineTransform = _textView.LineTransformSource.GetLineTransform(line, 0, ViewRelativePosition.Top);
            
           
        }

       

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(MarginName);
        }

        #region IWpfTextViewMargin Members

        /// <summary>
        /// The <see cref="Sytem.Windows.FrameworkElement"/> that implements the visual representation
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
                GC.SuppressFinalize(this);
                _isDisposed = true;
            }
        }
        #endregion
    }
}
