using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpectraLibraryParser
{
    class SpectraLibrary
    {
        public List<Spectrum> Library;
        public List<Edge> SimilarityEdges { get; private set; }
        
        public SpectraLibrary(string[] libraryFile)
        {
            Library = new List<Spectrum>();
            StringBuilder spectrum = new StringBuilder();
            foreach (var line in libraryFile)
            {
                if (String.IsNullOrEmpty(line))
                {
                    Library.Add(new Spectrum(spectrum.ToString()));
                    spectrum = new StringBuilder();
                    continue;
                }
                spectrum.AppendLine(line);
            }
        }

        /// <summary>
        /// Creates a similarity matrix using the Spectral Constant Angle for scoring
        /// </summary>
        /// <returns></returns>
        public List<Edge> BuildSimilarityScores()
        {
            SimilarityEdges = (from spectrum_a in Library
                    from spectrum_b in Library
                    let score = Compare(spectrum_a, spectrum_b)
                    select new Edge(spectrum_a.Name, spectrum_b.Name, score)).Distinct().ToList();
            return SimilarityEdges;
        }

        /// <summary>
        /// Compares two spectra to return the Spectral Constant Angle
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private double Compare(Spectrum a, Spectrum b)
        {
            var aKeys = a.Peaks.Keys;
            var bKeys = b.Peaks.Keys;
            var allKeys = aKeys.Concat(bKeys).ToList().Distinct(); //List of all M/Zs for peaks in both spectrum a and b

            double sum_ab = 0; //Sum of peaks a_i * b_i
            double sum_asq = 0; //Sum of peaks a_i^2
            double sum_bsq = 0; //Sum of peaks b_i^2
            foreach (var key in allKeys)
            {
                int a_int = 0; //Intensity of peak a_i
                int b_int = 0; //Intensity of peak b_i
                if (aKeys.Contains(key)) a_int = a.Peaks[key];
                if (bKeys.Contains(key)) b_int = b.Peaks[key];
                sum_ab += a_int * b_int;
                sum_asq += Math.Pow(a_int,2);
                sum_bsq += Math.Pow(b_int,2);
            }
            //var score = (180/Math.PI)*Math.Acos(sum_ab/Math.Sqrt(sum_asq*sum_bsq)); //Spectral Constant Angle in degrees
            var score = sum_ab / Math.Sqrt(sum_asq * sum_bsq); //Cos(theta) of Spectral Constant Angle
            return score;
        }

        /// <summary>
        /// Writes the SpectraLibrary SimilarityEdges to a TSV for network analysis
        /// </summary>
        public void WriteUndirectedGraph(string path, double threshold)
        {
            TextWriter file = new StreamWriter(path.Replace(".msl", "_edges.tsv"));
            List<Edge> edgeTracker = new List<Edge>();
            foreach (var edge in SimilarityEdges)
            {
                if (!edgeTracker.Contains(edge) && edge.Item1 != edge.Item2 && edge.Item3 >= threshold)
                {
                    file.WriteLine(edge.ToString());
                }
                edgeTracker.Add(edge);
            }
            
        }

        /// <summary>
        /// Writes the SpectraLibrary SimilarityEdges to a TSV for network analysis
        /// </summary>
        public void WriteDirectedGraph(string path, double threshold)
        {
            TextWriter file = new StreamWriter(path.Replace(".msl", "_edges.tsv"));
            foreach (var edge in SimilarityEdges)
            {
                if (edge.Item1 != edge.Item2 && edge.Item3 >= threshold)
                {
                    file.WriteLine(edge.ToString());
                }
            }
        }

        /// <summary>
        /// Writes a MassBank searchable list of spectra to a text file
        /// </summary>
        /// <param name="path"></param>
        public void WritePeakLists(string path)
        {
            TextWriter file = new StreamWriter(path.Replace(".msl","_peaklist.txt"));
            foreach (var spectrum in Library)
            {
                file.WriteLine("NAME: {0}",spectrum.Name);
                file.WriteLine(spectrum.PeakListString());
            }
        }

    }
}
