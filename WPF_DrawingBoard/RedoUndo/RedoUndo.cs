using System;
using System.Collections.Generic;
using System.Windows.Ink;

namespace WPF_DrawingBoard.RedoUndo
{
    sealed class CommandStack
    {
        public CommandStack(StrokeCollection strokeCollection)
        {
            if (strokeCollection == null)
            {
                throw new ArgumentNullException(nameof(strokeCollection));
            }

            StrokeCollection = strokeCollection;
            _undoStack = new Stack<CommandItem>();
            _redoStack = new Stack<CommandItem>();
            _disableChangeTracking = false;

        }

        public StrokeCollection StrokeCollection { get; set; }

        private Stack<CommandItem> _undoStack;
        private Stack<CommandItem> _redoStack;
        private bool _disableChangeTracking;


        public bool CanUndo => (_undoStack.Count > 0);

        public bool CanRedo => (_redoStack.Count > 0);

        public void Undo()
        {
            if (!CanUndo)
            {
                throw new InvalidOperationException("No actions to undo");
            }

            CommandItem commandItem = _undoStack.Pop();
            _disableChangeTracking = true;

            try
            {
                commandItem.Undo();
            }
            finally
            {
                _disableChangeTracking = false;
            }

            _redoStack.Push(commandItem);
        }

        public void Redo()
        {
            if (!CanRedo)
            {
                throw new InvalidOperationException("No actions to redo");
            }

            CommandItem commandItem = _redoStack.Pop();

            _disableChangeTracking = true;

            try
            {
                commandItem.Redo();
            }
            finally
            {
                _disableChangeTracking = false;
            }

            _undoStack.Push(commandItem);
        }

        public void Enqueue(CommandItem commandItem)
        {
            if (commandItem == null)
            {
                throw new ArgumentNullException(nameof(commandItem));
            }

            if (_disableChangeTracking)
            {
                return;
            }

            bool merged = false;
            if (_undoStack.Count > 0)
            {
                CommandItem previousCommandItem = _undoStack.Peek();
                merged = previousCommandItem.Merge(commandItem);
            }

            if (!merged)
            {
                _undoStack.Push(commandItem);
            }

            if (_redoStack.Count > 0)
            {
                _redoStack.Clear();
            }
        }
    }
}
