using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace WPF_DrawingBoard.Thumbs
{
    class MoveThumb:Thumb
    {
        public MoveThumb()
        {
            DragDelta += MoveThumb_DragDelta;
        }

        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            DrawingBoard drawingBoard = DataContext as DrawingBoard;

            if (drawingBoard?.Board != null)
            {
                Point point = drawingBoard.Board.Position;
                point.X += e.HorizontalChange;
                point.Y += e.VerticalChange;
                if (point.X < 0)
                {
                    point.X = 0;
                }
                if (point.Y < 0)
                {
                    point.Y = 0;
                }
                drawingBoard.Board.Position = point;
            }
        }
    }
}
