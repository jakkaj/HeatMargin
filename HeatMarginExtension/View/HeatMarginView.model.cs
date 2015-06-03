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
    public class HeatMarginViewModel : ViewModel, IDisposable
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

        public void LineDelta(int position, int newLines)
        {
            if (newLines == 0)
            {
                return;
            }

            //move all items after this position back or forward depending on direction
            var snapshot = _textView.TextSnapshot;

            var changes = false;
            foreach (var line in _lines)
            {
                var pos = line.Line.Start.Position;
                if (pos > position)
                {
                    var newLineNumber = line.Line.LineNumber + newLines;
                    var snapshotLine = snapshot.GetLineFromLineNumber(newLineNumber);

                    line.CurrentLineNumber = newLineNumber;
                    line.Line = snapshotLine;
                    changes = true;
                }
            }
            if (changes)
            {
                RefreshLines();
            }
        }

        public void LineRemoved(ITextViewLine line)
        {
            if (!_textView.TextViewLines.Contains(line))
            {
                return;
            }

            if (_lines == null)
            {
                return;
            }

            var snapshot = _textView.TextSnapshot;
            var snapshotLine = snapshot.GetLineFromPosition(line.Start.Position);

            var lineNumber = snapshotLine.LineNumber;

            var lineWrapper = _lines.FirstOrDefault(_ => _.CurrentLineNumber == lineNumber);
            
            if (lineWrapper != null)
            {
                DisposeItem(lineWrapper);
            }

            RefreshLines();
        }

        public void LineUpdated(ITextViewLine line)
        {
            if (!_textView.TextViewLines.Contains(line))
            {
                return;
            }   

            if (_lines == null)
            {
                return;
            }

            var snapshot = _textView.TextSnapshot;
            var snapshotLine = snapshot.GetLineFromPosition(line.Start.Position);

            var lineNumber = snapshotLine.LineNumber;
            
            var lineWrapper = new LineWrapper
            {
                Line = snapshotLine,
                ViewModel= new HeatMarginItemViewModel
                {
                    Visible = true
                },
                CurrentLineNumber = lineNumber
            };
            
            _viewModels.Add(lineWrapper.ViewModel);
            
            _lines.Insert(0, lineWrapper);
            
            while (_lines.Count > 50)
            {
                DisposeItem(_lines.LastOrDefault());
            }

            RefreshLines();
        }

        public void DisposeItem(LineWrapper wrapper)
        {
            if (_viewModels.Contains(wrapper.ViewModel))
            {
                _viewModels.Remove(wrapper.ViewModel);
            }
            
            wrapper.ViewModel = null;
            
            if (_lines.Contains(wrapper))
            {
                _lines.Remove(wrapper);
            }
        }

        bool _cleanUp(LineWrapper wrapper, ITextSnapshot snapshot)
        {
            var returnValue = true;

            foreach (var existingItem in _lines)
            {
                if (wrapper.CurrentLineNumber == existingItem.CurrentLineNumber && !object.ReferenceEquals(wrapper, existingItem))
                {
                    if (_lines.IndexOf(wrapper) > _lines.IndexOf(existingItem))
                    {
                        returnValue = false;
                    }
                }

                if (snapshot.LineCount <= existingItem.CurrentLineNumber && existingItem.CurrentLineNumber != -1)
                {
                    returnValue = false;
                }
            }

            return returnValue;
        }

        bool _syncCurrentLine(LineWrapper wrapper, ITextSnapshot snapshot)
        {
            if (wrapper.CurrentLineNumber >= snapshot.LineCount)
            {
                return false;
            }

            var lineSnapShot = snapshot.GetLineFromLineNumber(wrapper.Line.LineNumber);

            wrapper.Line = lineSnapShot;

            wrapper.CurrentLineNumber = lineSnapShot.LineNumber;

            return true;
        }

        public void RefreshLines()
        {
            var snapShot = _textView.TextSnapshot;
            
            var toRemove = new List<LineWrapper>();

            if (_lines == null)
            {
                return;
            }

            foreach (var line in _lines)
            {
                if (!_syncCurrentLine(line, snapShot))
                {
                    toRemove.Add(line);
                }
            }

            if (_lines == null)
            {
                return;
            }

            foreach (var line in _lines)
            {
                if (toRemove.Contains(line))
                {
                    continue;
                }

                var success = _cleanUp(line, snapShot);

                if (success)
                {
                    _doViewModel(line);
                }
                else
                {
                    toRemove.Add(line);
                }
            }

            foreach (var item in toRemove)
            {
                DisposeItem(item);
            }
        }

        double _getAge(LineWrapper line)
        {
            var idx = _lines.IndexOf(line);

            if (idx < 20)
            {
                var percent = Convert.ToDouble(_lines.IndexOf(line)) / Convert.ToDouble(_lines.Count);
                return percent; 
            }

            return 1;
        }

        void _doViewModel(LineWrapper line)
        {
            var vm = line.ViewModel;

            var actualLine = _textView.GetTextViewLineContainingBufferPosition(line.Line.Start);

            if (actualLine == null)
            {
                Debugger.Break();
            }

            vm.Age = _getAge(line);

            if (!_isScrollBar())
            {
                _updateViewModelDocument(vm, actualLine, line.CurrentLineNumber);
            }
            else
            {
                _updateViewModelScroller(vm, actualLine);
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

        void _updateViewModelDocument(HeatMarginItemViewModel vm, ITextViewLine line, int lineNumber)
        {
            if (lineNumber < 0)
            {
                vm.Visible = false;
                return;
            }

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
                    visible = false;
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
                    visible = false;
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

        public void Dispose()
        {
            if (_lines == null)
            {
                return;
            }

            _viewModels.Clear();

            foreach (var item in _lines)
            {
                item.ViewModel = null;
            }

            _lines.Clear();
        }
    }

    public class LineWrapper
    {
        public ITextSnapshotLine Line { get; set; }
        public HeatMarginItemViewModel ViewModel { get; set; }
        public int CurrentLineNumber { get; set; }
    }
}
