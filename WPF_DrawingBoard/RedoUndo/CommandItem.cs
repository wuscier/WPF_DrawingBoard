namespace WPF_DrawingBoard.RedoUndo
{
    abstract class CommandItem
    {
        public abstract void Undo();
        public abstract void Redo();

        public abstract bool Merge(CommandItem newItem);

        protected CommandStack CommandStack;
        protected CommandItem(CommandStack commandStack)
        {
            CommandStack = commandStack;
        }
    }
}
