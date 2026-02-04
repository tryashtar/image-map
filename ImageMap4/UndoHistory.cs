using System;
using System.Collections.Generic;

namespace ImageMap4;

public class UndoHistory
{
    private readonly Stack<Undoable> Undos = new();
    private readonly Stack<Undoable> Redos = new();
    public void Perform(Action action, Action undo)
    {
        action();
        Undos.Push(new(action, undo) { Context = null });
        Redos.Clear();
    }
    public void PerformContext<TActionContext, TUndoContext>(Func<TUndoContext?, TActionContext?> action, Func<TActionContext?, TUndoContext?> undo)
    {
        var result = action(default);
        Undos.Push(new(x => action((TUndoContext)x), x => undo((TActionContext)x)) { Context = result });
        Redos.Clear();
    }
    public void Undo()
    {
        if (CanUndo)
        {
            var action = Undos.Pop();
            action.Context = action.Undo(action.Context);
            Redos.Push(action);
        }
    }
    public void Redo()
    {
        if (CanRedo)
        {
            var action = Redos.Pop();
            action.Context = action.Action(action.Context);
            Undos.Push(action);
        }
    }
    public void Clear()
    {
        Undos.Clear();
        Redos.Clear();
    }
    public bool CanUndo => Undos.Count > 0;
    public bool CanRedo => Redos.Count > 0;

    private class Undoable
    {
        public readonly Func<object?, object?> Action;
        public readonly Func<object?, object?> Undo;
        public object? Context = null;
        public Undoable(Func<object?, object?> action, Func<object?, object?> undo)
        {
            Action = action;
            Undo = undo;
        }
        public Undoable(Action action, Action undo)
        {
            Action = x => { action(); return null; };
            Undo = x => { undo(); return null; };
        }
    }
}
