using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Threading;
using HeatMarginExtension.Infrastructure;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace HeatMarginExtension.View
{
    public class HeatMarginViewModel : ViewModel
    {
        private readonly IWpfTextView _textView;
        private readonly IVerticalScrollBar _scrollBar;
        private string _lineTest;

        private List<LineWrapper> _lines;

        private ObservableCollection<HeatMarginItemViewModel> _viewModels;

        public HeatMarginViewModel(IWpfTextView textView, IVerticalScrollBar scrollBar)
        {
            _textView = textView;
            _scrollBar = scrollBar;
            _lines = new List<LineWrapper>();
            _viewModels = new ObservableCollection<HeatMarginItemViewModel>();
        }

        bool _isScrollBar()
        {
            return _scrollBar != null;
        }

        public void LineUpdated(ITextViewLine line)
        {
            var lineNumber = _textView.TextViewLines.IndexOf(line);
            
            var lineWrapper = new LineWrapper
            {
                Line = line,
                LineNumber = lineNumber
            };

            _cleanUp(lineWrapper);

            _lines.Insert(0, lineWrapper);

            RefreshLines();
        }

        void _cleanUp(LineWrapper wrapper)
        {
            var existing = _lines.FirstOrDefault(_ => _.LineNumber == wrapper.LineNumber);
            if (existing != null)
            {
                var vm = _viewModels.FirstOrDefault(_ => _.LineNumber == existing.LineNumber);
                _viewModels.Remove(vm);
                _lines.Remove(existing);
            }
        }

        public void RefreshLines()
        {
            var snapShot = _textView.TextSnapshot;
            
            List<LineWrapper> toRemove = new List<LineWrapper>();

            foreach (var line in _lines)
            {
                var lineNumber = line.LineNumber;

                if (snapShot.LineCount <= lineNumber || lineNumber < 0)
                {
                    toRemove.Add(line);
                    continue;
                }

                var currentLine = snapShot.GetLineFromLineNumber(lineNumber);
                var textLine = _textView.GetTextViewLineContainingBufferPosition(currentLine.Start);

                var actualNumber = _textView.TextViewLines.IndexOf(textLine);

                line.Line = textLine;

                if (snapShot.LineCount <= actualNumber || actualNumber < 0)
                {
                    toRemove.Add(line);
                    continue;
                }

                _doViewModel(line);
            }

            foreach (var item in toRemove)
            {
                _cleanUp(item);
            }
        }

        double _getAge(LineWrapper line)
        {
            var percent = Convert.ToDouble(_lines.IndexOf(line)) / Convert.ToDouble(_lines.Count);
            return percent;
        }

        void _doViewModel(LineWrapper line)
        {
            var vm = _viewModels.FirstOrDefault(_ => _.LineNumber == line.LineNumber);

            if (vm == null)
            {
                vm = new HeatMarginItemViewModel
                {
                    LineNumber = line.LineNumber
                };

                _viewModels.Add(vm);
            }

            vm.Age = _getAge(line);

            if (!_isScrollBar())
            {
                _updateViewModelDocument(vm, line.Line);
            }
            else
            {
                _updateViewModelScroller(vm, line.Line);
            }
        }

        void _updateViewModelScroller(HeatMarginItemViewModel vm, ITextViewLine line)
        {
            

            if (line == null) return;

            var mapTop = _scrollBar.Map.GetCoordinateAtBufferPosition(line.Start) - 0.5;
            var mapBottom = _scrollBar.Map.GetCoordinateAtBufferPosition(line.End) + 0.5;


            vm.Top = Math.Round(_scrollBar.GetYCoordinateOfScrollMapPosition(mapTop)) - 2.0;
            vm.Height = Math.Round(_scrollBar.GetYCoordinateOfScrollMapPosition(mapBottom)) - vm.Top + 2.0;
        }

        void _updateViewModelDocument(HeatMarginItemViewModel vm, ITextViewLine line)
        {
            var lineStart = _textView.GetTextViewLineContainingBufferPosition(line.Start);
            var lineEnd = _textView.GetTextViewLineContainingBufferPosition(line.End);

            double startTop = 0;

            var visible = true;

            switch (line.VisibilityState)
            {
                case VisibilityState.FullyVisible:
                case VisibilityState.Hidden:
                case VisibilityState.PartiallyVisible:
                    startTop = lineStart.Top - _textView.ViewportTop;
                    break;

                case VisibilityState.Unattached:
                    // if the closest line was past the end we would have already returned
                    startTop = 0;
                    break;

                default:
                    // shouldn't be reachable, but definitely hide if this is the case
                    visible = false;
                    break;
            }

            double stopBottom = 0;

            switch (lineEnd.VisibilityState)
            {
                case VisibilityState.FullyVisible:
                case VisibilityState.Hidden:
                case VisibilityState.PartiallyVisible:
                    stopBottom = lineEnd.Bottom - _textView.ViewportTop;
                    break;

                case VisibilityState.Unattached:
                    // if the closest line was before the start we would have already returned
                    stopBottom = _textView.ViewportHeight;
                    break;

                default:
                    // shouldn't be reachable, but definitely hide if this is the case
                    visible = false;
                    break;
            }

            vm.Top = startTop;
            vm.Height = stopBottom - startTop;
            vm.Visible = visible;
        }

        public string LineTest
        {
            get { return _lineTest; }
            set
            {
                _lineTest = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<HeatMarginItemViewModel> ViewModels
        {
            get { return _viewModels; }
            set
            {
                _viewModels = value;
                OnPropertyChanged();
            }
        }
    }

    public class LineWrapper
    {
        public int LineNumber { get; set; }
        public ITextViewLine Line { get; set; }
    }
}
