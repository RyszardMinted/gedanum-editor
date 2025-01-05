using System.Collections.Generic;
using UnityEngine;

public class ProjectManager
{
    public BlockInstance CurrentProject;


    private BlockManager blockManager;
    

    private Stack<ICommand> undoStack = new Stack<ICommand>();
    private Stack<ICommand> redoStack = new Stack<ICommand>();

    public ProjectManager(BlockManager blockManager)
    {
        this.blockManager = blockManager;
    }
    
    public void ClearProject(bool createEmpty)
    {
        ClearHistory();

        if (CurrentProject != null)
        {
           blockManager.DestroyProject(CurrentProject.gameObject);
        }

        if (createEmpty)
        {
            CurrentProject = blockManager.InstantiateEmptyBlock();
        }
        else
        {
            CurrentProject = null;
        }
    }
    
    public void CreateNewProject(Vector3Int size, string name)
    {
        ClearProject(true);
        CurrentProject.data.size = size;
        CurrentProject.data.blockName = name;
    }

    public void ExecuteCommand(ICommand command) {
        command.Execute(CurrentProject);
        undoStack.Push(command);
        redoStack.Clear();
    }

    public void Undo() {
        if (undoStack.Count > 0) {
            ICommand command = undoStack.Pop();
            command.Undo(CurrentProject);
            redoStack.Push(command);
        }
    }

    public void Redo() {
        if (redoStack.Count > 0) {
            ICommand command = redoStack.Pop();
            command.Execute(CurrentProject);
            undoStack.Push(command);
        }
    }

    private void ClearHistory() {
        undoStack.Clear();
        redoStack.Clear();
    }
}