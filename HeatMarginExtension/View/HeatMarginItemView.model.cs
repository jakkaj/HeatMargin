using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using HeatMarginExtension.Infrastructure;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace HeatMarginExtension.View
{
    public class HeatMarginItemViewModel : ViewModel
    {
        private double _age;
        private double _top;
        private double _height;
        private bool _visible;
        private int _lineNumber;

        public HeatMarginItemViewModel()
        {
            
        }

        public double Age
        {
            get { return _age; }
            set
            {
                _age = value;
                OnPropertyChanged();
            }
        }

        public double Top
        {
            get { return _top; }
            set
            {
                _top = value;
                OnPropertyChanged();
            }
        }

        public double Height
        {
            get { return _height; }
            set
            {
                _height = value;
                OnPropertyChanged();
            }
        }

        public bool Visible
        {
            get { return _visible; }
            set
            {
                _visible = value;
                OnPropertyChanged();
            }
        }

        public int LineNumber
        {
            get { return _lineNumber; }
            set
            {
                _lineNumber = value;
                OnPropertyChanged();
            }
        }
    }
}
