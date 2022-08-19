using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ImageMap4;

public static class UndoHistory
{
    private static readonly Stack<(Action action, Action undo)> Undos = new();
    private static readonly Stack<(Action action, Action undo)> Redos = new();
    public static void Perform(Action action, Action undo)
    {
        action();
        Undos.Push((action, undo));
    }
    public static void Perform<T>(Func<T> action, Action<T> undo)
    {
        var result = action();
        Undos.Push((() => action(), () => undo(result)));
    }
    public static void Undo()
    {
        if (CanUndo)
        {
            var action = Undos.Pop();
            action.undo();
            Redos.Push(action);
        }
    }
    public static void Redo()
    {
        if (CanRedo)
        {
            var action = Redos.Pop();
            action.action();
            Undos.Push(action);
        }
    }
    public static bool CanUndo => Undos.Count > 0;
    public static bool CanRedo => Redos.Count > 0;
}
