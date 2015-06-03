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

        private List<string> _colors;

        private SolidColorBrush _brush;

        public HeatMarginItemViewModel()
        {
            _colors = new List<string>
            {
                "#CC00CC",
                "#CF0ACF",
                "#D114D1",
                "#D41FD4",
                "#D629D6",
                "#D933D9",
                "#DB3DDB",
                "#DE47DE",
                "#E052E0",
                "#E35CE3",
                "#E666E6",
                "#E870E8",
                "#EB7AEB",
                "#ED85ED",
                "#F08FF0",
                "#F299F2",
                "#F5A3F5",
                "#F7ADF7",
                "#FAB8FA",
                "#FCC2FC",
                "#FFCCFF"
            };
        }

        void _doColor()
        {
            var ageColor = Convert.ToInt32(Math.Round(_age * _colors.Count));

            if (ageColor >= _colors.Count)
            {
                ageColor = _colors.Count - 1;
            }

            Color color = (Color) ColorConverter.ConvertFromString(_colors[ageColor]);
            ColorBrush = new SolidColorBrush(color);
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
    }
}
