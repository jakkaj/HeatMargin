using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
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

        private SolidColorBrush _brush;
        private double _opacity;

       
        void _doColor()
        {
            var color = (Color)ColorConverter.ConvertFromString("#FF00FF");
            
            ColorBrush = new SolidColorBrush(color);
            
            Opacity = 1 - _age;
            
            if (Opacity < .2)
            {
                Opacity = .2;
            }
        }

        public double Age
        {
            get { return _age; }
            set
            {
                _age = value;
                OnPropertyChanged();
                _doColor();
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
                OnPropertyChanged("IsVisible");
            }
        }

        public Visibility IsVisible
        {
            get { return Visible ? Visibility.Visible : Visibility.Collapsed; }
        }

        public SolidColorBrush ColorBrush
        {
            get { return _brush; }
            set
            {
                _brush = value;
                OnPropertyChanged();
            }
        }

        public double Opacity
        {
            get { return _opacity; }
            set
            {
                _opacity = value;
                OnPropertyChanged();
            }
        }
    }
}
