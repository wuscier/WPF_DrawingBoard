using System.Windows;
using System.Windows.Ink;
using System.Windows.Media;

namespace WPF_DrawingBoard.RedoUndo
{
    class SelectionMovedOrResizedCommandItem:CommandItem
    {
        private readonly StrokeCollection _selectedStrokeCollection;
        private  Rect _newRect;
        private  Rect _oldRect;
        private readonly int _editingOperationCount;

        public SelectionMovedOrResizedCommandItem(CommandStack commandStack, StrokeCollection selection, Rect newRect,
            Rect oldRect, int editingOperationCount) : base(commandStack)
        {
            _selectedStrokeCollection = selection;
            _newRect = newRect;
            _oldRect = oldRect;
            _editingOperationCount = editingOperationCount;
        }

        public override void Undo()
        {
            Matrix m = GetTransformFromRectToRect(_newRect, _oldRect);
            _selectedStrokeCollection.Transform(m, false);
        }

        public override void Redo()
        {
            Matrix m = GetTransformFromRectToRect(_oldRect, _newRect);
            _selectedStrokeCollection.Transform(m, false);
        }

        public override bool Merge(CommandItem newItem)
        {
            SelectionMovedOrResizedCommandItem newitemx = newItem as SelectionMovedOrResizedCommandItem;

            if (newitemx == null ||
                newitemx._editingOperationCount != _editingOperationCount ||
                !StrokeCollectionsAreEqual(newitemx._selectedStrokeCollection, _selectedStrokeCollection))
            {
                return false;
            }

            _newRect = newitemx._newRect;

            return true;
        }


        static Matrix GetTransformFromRectToRect(Rect src, Rect dst)
        {
            Matrix m = Matrix.Identity;
            m.Translate(-src.X, -src.Y);
            m.Scale(dst.Width / src.Width, dst.Height / src.Height);
            m.Translate(+dst.X, +dst.Y);
            return m;
        }

        static bool StrokeCollectionsAreEqual(StrokeCollection a, StrokeCollection b)
        {
            if (a == null && b == null) return true;
            if (a == null || b == null) return false;
            if (a.Count != b.Count) return false;

            for (int i = 0; i < a.Count; ++i)
                if (a[i] != b[i]) return false;

            return true;
        }
    }
}
