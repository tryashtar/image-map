using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace ImageMap
{
    public static class Util
    {
        public static readonly string[] ImageExtensions = new[] { ".png", ".bmp", ".jpg", ".jpeg", ".gif" };

        public static string GenerateFilter(string description, string[] extensions)
        {
            string result = description + "|";
            foreach (string extension in extensions)
            {
                result += "*" + extension + ";";
            }
            result += "|All Files|*.*";
            return result;
        }

        public static string Pluralize(int amount, string singular, string plural)
        {
            if (amount == 1)
                return $"1 {singular}";
            return $"{amount} {plural}";
        }
        public static string Pluralize(int amount, string singular) => Pluralize(amount, singular, singular + "s");

        // Enumerable.Range() but for longs
        public static IEnumerable<long> CreateRange(long start, long count)
        {
            var limit = start + count;
            while (start < limit)
            {
                yield return start;
                start++;
            }
        }

        public static bool IsSingleSequence<T>(IEnumerable<T> items)
        {
            return items.Take(2).Count() == 1;
        }

        public static string ExceptionMessage(Exception ex)
        {
            if (ex is AggregateException agg)
                return String.Join("\n", agg.InnerExceptions.Select(x => ExceptionMessage(x)));
            string result = $"{ex.GetType().Name}: {ex.Message}";
            if (ex.InnerException != null)
                result += "\n" + ExceptionMessage(ex.InnerException);
            return result;
        }

        public static string MapName(long id)
        {
            return $"map_{id}";
        }

        public static bool MapString(string input, out long mapid)
        {
            var match = Regex.Match(input, @"^map_(-?\d+)$");
            if (match.Success)
            {
                mapid = Int64.Parse(match.Groups[1].Value);
                return true;
            }
            else
            {
                mapid = 0;
                return false;
            }
        }

        public static Dictionary<T, U> Copy<T, U>(this IReadOnlyDictionary<T, U> dict) => dict.ToDictionary(x => x.Key, y => y.Value);

        public static void SetControls<T>(Control control, IEnumerable<T> source) where T : Control
        {
            var destination = control.Controls;
            var typed = destination.OfType<T>();
            var add = source.Except(typed).ToArray();
            var remove = typed.Except(source).ToList();
            control.SuspendLayout();
            if (remove.Count == destination.Count)
                destination.Clear();
            else
            {
                foreach (var item in remove)
                {
                    destination.Remove(item);
                }
            }
            destination.AddRange(add);
            if (control is FlowLayoutPanel flow)
            {
                int i = -1;
                foreach (var item in source)
                {
                    i++;
                    flow.Controls.SetChildIndex(item, i);
                }
            }
            control.ResumeLayout();
        }

        // fall back if unsupported
        public static DialogResult ShowCompatibleOpenDialog(OpenFileDialog d)
        {
            try
            {
                return d.ShowDialog();
            }
            catch (COMException)
            {
                d.AutoUpgradeEnabled = false;
                return d.ShowDialog();
            }
        }
    }

    // fall back if unsupported
    public class FolderPicker
    {
        public readonly string Title;
        public readonly string InitialFolder;
        public string SelectedFolder { get; private set; } = null;
        public FolderPicker(string title, string initial_folder)
        {
            Title = title;
            InitialFolder = initial_folder;
        }

        public DialogResult ShowDialog()
        {
            var good_browser = new CommonOpenFileDialog()
            {
                Title = this.Title,
                InitialDirectory = this.InitialFolder,
                IsFolderPicker = true
            };
            try
            {
                var result = good_browser.ShowDialog() == CommonFileDialogResult.Ok ? DialogResult.OK : DialogResult.Cancel;
                if (result == DialogResult.OK)
                    SelectedFolder = good_browser.FileName;
                return result;
            }
            catch (COMException)
            {
                var crappy_browser = new FolderBrowserDialog()
                {
                    Description = this.Title,
                    SelectedPath = this.InitialFolder
                };
                var result = crappy_browser.ShowDialog();
                if (result == DialogResult.OK)
                    SelectedFolder = crappy_browser.SelectedPath;
                return result;
            }
        }
    }
}
