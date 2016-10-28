using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace SpectraLibraryParser
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Select spectra library");
                string path;
                var libraryFile = ReadLines(out path);
                SpectraLibrary library = new SpectraLibrary(libraryFile);
                var similarity = library.BuildSimilarityScores();
                library.WriteUndirectedGraph(path, 0.6);
                library.WritePeakLists(path);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw (ex);
            }
        }

        private static string[] ReadLines(out string path)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = "Libraries\\Documents",
                Filter = "msl files (*.msl)|*.msl|All files (*.*)|*.*"
            };
            openFileDialog1.ShowDialog();
            string[] file = File.ReadAllLines(openFileDialog1.FileName);
            path = openFileDialog1.FileName;
            return file;
        }
    }
}
