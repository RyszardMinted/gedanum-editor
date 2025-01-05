
    public interface ICommand
    {
        void Execute(BlockInstance project);
        void Undo(BlockInstance project);
    }
