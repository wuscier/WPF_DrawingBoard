using System.Windows.Controls;
using System.Windows.Ink;

namespace WPF_DrawingBoard.RedoUndo
{
    class StrokesChangedCommandItem:CommandItem
    {
        private readonly InkCanvasEditingMode _editingMode;
        private readonly StrokeCollection _added;
        private readonly StrokeCollection _removed;
        private readonly int _editingOperationCount;

        public StrokesChangedCommandItem(CommandStack commandStack,
            InkCanvasEditingMode editingMode,
            StrokeCollection added,
            StrokeCollection removed,
            int editingOperationCount) : base(commandStack)
        {
            _editingMode = editingMode;
            _added = added;
            _removed = removed;
            _editingOperationCount = editingOperationCount;
        }

        public override void Undo()
        {
            CommandStack.StrokeCollection.Remove(_added);
            CommandStack.StrokeCollection.Add(_removed);
        }

        public override void Redo()
        {
            CommandStack.StrokeCollection.Add(_added);
            CommandStack.StrokeCollection.Remove(_removed);
        }

        public override bool Merge(CommandItem newItem)
        {
            StrokesChangedCommandItem newStrokesChangedCommandItem = newItem as StrokesChangedCommandItem;

            if (newStrokesChangedCommandItem == null ||
                newStrokesChangedCommandItem._editingMode != _editingMode ||
                newStrokesChangedCommandItem._editingOperationCount != _editingOperationCount)
            {
                return false;
            }

            if (_editingMode != InkCanvasEditingMode.EraseByPoint) return false;
            if (newStrokesChangedCommandItem._editingMode != InkCanvasEditingMode.EraseByPoint) return false;


            foreach (Stroke doomed in newStrokesChangedCommandItem._removed)
            {
                if (_added.Contains(doomed))
                {
                    _added.Remove(doomed);
                }
                else
                {
                    _removed.Add(doomed);
                }
            }
            _added.Add(newStrokesChangedCommandItem._added);

            return true;

        }
    }
}
