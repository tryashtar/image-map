using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

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

        public static string ExceptionMessage(Exception ex)
        {
            if (ex is AggregateException agg)
                return String.Join("\n", agg.InnerExceptions.Select(x => ExceptionMessage(x)));
            return $"{ex.GetType().Name}: {ex.Message}";
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
