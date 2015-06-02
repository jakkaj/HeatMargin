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

        private List<int> _lines;

        private ObservableCollection<HeatMarginItemViewModel> _viewModels;

        public HeatMarginViewModel(IWpfTextView textView, IVerticalScrollBar scrollBar)
        {
            _textView = textView;
            _scrollBar = scrollBar;
            _lines = new List<int>();
            _viewModels = new ObservableCollection<HeatMarginItemViewModel>();
        }

        public void LineUpdated(ITextViewLine line)
        {
            var lineNumber = _textView.TextViewLines.IndexOf(line);

            LineTest = lineNumber.ToString();

            if (_lines.Contains(lineNumber))
            {
                _lines.Remove(lineNumber);
            }

            _lines.Insert(0, lineNumber);

            _refreshLines();
        }

        void _refreshLines()
        {
            var snapShot = _textView.TextSnapshot;

            foreach (var line in _lines)
            {
                if (snapShot.LineCount < line)
                    continue;
                _doViewModel(line, snapShot);
            }
        }

        double _getAge(int lineNumber)
        {
            var percent = Convert.ToDouble(_lines.IndexOf(lineNumber)) / Convert.ToDouble(_lines.Count);
            return percent;
        }

        void _doViewModel(int line, ITextSnapshot snapShot)
        {
            var vm = _viewModels.FirstOrDefault(_ => _.LineNumber == line);

            if (vm == null)
            {
                vm = new HeatMarginItemViewModel
                {
                    LineNumber = line
                };

                _viewModels.Add(vm);
            }

            vm.Age = _getAge(line);

            if (_scrollBar == null)
            {
                _updateViewModelDocument(vm, line);
            }
            else
            {
                _updateViewModelScroller(vm, line, snapShot);
            }
        }

        void _updateViewModelScroller(HeatMarginItemViewModel vm, int lineNumber, ITextSnapshot snapshot)
        {
            var line = snapshot.GetLineFromLineNumber(lineNumber);

            if (line == null) return;

            var mapTop = _scrollBar.Map.GetCoordinateAtBufferPosition(line.Start) - 0.5;
            var mapBottom = _scrollBar.Map.GetCoordinateAtBufferPosition(line.End) + 0.5;


            vm.Top = Math.Round(_scrollBar.GetYCoordinateOfScrollMapPosition(mapTop)) - 2.0;
            vm.Height = Math.Round(_scrollBar.GetYCoordinateOfScrollMapPosition(mapBottom)) - vm.Top + 2.0;
        }

        void _updateViewModelDocument(HeatMarginItemViewModel vm, int lineNumber)
        {
            //{
            //    double startTop;
            //    switch (line.VisibilityState)
            //    {
            //        case VisibilityState.FullyVisible:
            //        case VisibilityState.Hidden:
            //        case VisibilityState.PartiallyVisible:
            //            startTop = line.Top - _textView.ViewportTop;
            //            break;

            //        case VisibilityState.Unattached:
            //            // if the closest line was past the end we would have already returned
            //            startTop = 0;
            //            break;

            //        default:
            //            // shouldn't be reachable, but definitely hide if this is the case
            //            return false;
            //    }

            //    double stopBottom;

            //    switch (line.VisibilityState)
            //    {
            //        case VisibilityState.FullyVisible:
            //        case VisibilityState.Hidden:
            //        case VisibilityState.PartiallyVisible:
            //            stopBottom = line.Bottom - _textView.ViewportTop;
            //            break;

            //        case VisibilityState.Unattached:
            //            // if the closest line was before the start we would have already returned
            //            stopBottom = _textView.ViewportHeight;
            //            break;

            //        default:
            //            // shouldn't be reachable, but definitely hide if this is the case
            //            return false;
            //    }

            //    diffViewModel.Top = startTop;
            //    diffViewModel.Height = stopBottom - startTop;
            //    return true;
            //}    
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
}
