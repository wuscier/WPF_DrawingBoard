using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;
using Prism.Mvvm;

namespace WPF_DrawingBoard
{
    public class DrawingBoardViewModel : BindableBase
    {
        public DrawingBoardViewModel()
        {
            DrawingAttributes = new DrawingAttributes();
        }

        private bool _isPenChecked;

        public bool IsPenChecked
        {
            get { return _isPenChecked; }
            set
            {
                if (value)
                {
                    IsEraserChecked = false;
                    EditingMode = InkCanvasEditingMode.Ink;
                }
                SetProperty(ref _isPenChecked, value);
            }
        }


        private bool _isEraserChecked;

        public bool IsEraserChecked
        {
            get { return _isEraserChecked; }
            set
            {
                if (value)
                {
                    IsPenChecked = false;
                    EditingMode = InkCanvasEditingMode.EraseByStroke;
                }
                SetProperty(ref _isEraserChecked, value);
            }
        }

        private InkCanvasEditingMode _editingMode;

        public InkCanvasEditingMode EditingMode
        {
            get { return _editingMode; }
            set { SetProperty(ref _editingMode, value); }
        }

        private DrawingAttributes _drawingAttributes;
        public DrawingAttributes DrawingAttributes
        {
            get { return _drawingAttributes; }
            set { SetProperty(ref _drawingAttributes, value); }
        }

        private double _inkThickness;
        public double InkThickness
        {
            get { return _inkThickness; }
            set { SetProperty(ref _inkThickness, value); }
        }

        private Color _inkColor;
        public Color InkColor
        {
            get { return _inkColor; }
            set { SetProperty(ref _inkColor, value); }
        }



    }
}