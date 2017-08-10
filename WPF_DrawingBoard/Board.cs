using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Ink;
using Prism.Mvvm;

namespace WPF_DrawingBoard
{
    public class Board : BindableBase
    {
        public Board()
        {
            StrokeCollection = new StrokeCollection();
        }

        private double _width;

        public double Width
        {
            get { return _width; }
            set { SetProperty(ref _width, value); }
        }

        private double _height;

        public double Height
        {
            get { return _height; }
            set { SetProperty(ref _height, value); }
        }

        private Point _position;

        public Point Position
        {
            get { return _position; }
            set { SetProperty(ref _position, value); }
        }

        private StrokeCollection _strokeCollection;

        public StrokeCollection StrokeCollection
        {
            get { return _strokeCollection; }
            set { SetProperty(ref _strokeCollection, value); }
        }

    }
}
