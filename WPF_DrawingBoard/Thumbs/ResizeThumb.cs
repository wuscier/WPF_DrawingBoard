using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace WPF_DrawingBoard.Thumbs
{
    public class ResizeThumb:Thumb
    {
        public ResizeThumb()
        {
            DragDelta += ResizeThumb_DragDelta;
        }

        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            DrawingBoard drawingBoard = DataContext as DrawingBoard;
            if (drawingBoard?.Board != null)
            {
                drawingBoard.Board.Width = drawingBoard.Board.Width + e.HorizontalChange >= 0
                    ? drawingBoard.Board.Width + e.HorizontalChange
                    : 0;
                drawingBoard.Board.Height = drawingBoard.Board.Height + e.VerticalChange >= 0
                    ? drawingBoard.Board.Height + e.VerticalChange
                    : 0;
            }
        }
    }
}
