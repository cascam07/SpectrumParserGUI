using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SpectraLibraryParser
{
    class Spectrum
    {
        public string Name;
        public string Formula;
        public double MW;
        public long CASNO;
        public double RT;
        public Dictionary<int, int> Peaks;

        public Spectrum(string spectrumString)
        {
            //TODO: Parse strings into spectrum object
            string[] spectrumLines = spectrumString.Split(new string[]{"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
            
            Peaks = new Dictionary<int, int>();
            foreach (var line in spectrumLines)
            {
                var lowLine = line.ToLower();
                if (lowLine.StartsWith("name"))
                {
                    Name = line.Remove(0, 5).Trim();
                }
                if (lowLine.StartsWith("form"))
                {
                    Formula = line.Remove(0, 5).Trim();
                }
                if (lowLine.StartsWith("mw"))
                {
                    //MW = Double.Parse(line.Remove(0, 3).Trim());
                    Double.TryParse(line.Remove(0, 3).Trim(), out MW);
                }
                if (lowLine.StartsWith("casno"))
                {
                    //CASNO = Int64.Parse(line.Remove(0, 6).Trim());
                    Int64.TryParse(line.Remove(0, 6).Trim(), out CASNO);
                }
                if (lowLine.StartsWith("rt"))
                {
                    //RT = Double.Parse(line.Remove(0, 3).Trim());
                    Double.TryParse(line.Remove(0, 3).Trim(), out RT);
                }
                if (lowLine.StartsWith("(")) //List of peaks that look like: (  55    6) (  58    7) (  59   23) (  69   12) (  71    5) 
                {
                    var peaks = lowLine.Split(new char[] {')'});
                    foreach (var peak in peaks)
                    {
                        if (String.IsNullOrWhiteSpace(peak)) continue;
                        var trimmedPeak = peak.Replace("(","").Trim().Split(null);
                        int mz = Int32.Parse(trimmedPeak.First());
                        int intensity = Int32.Parse(trimmedPeak.Last());
                        Peaks.Add(mz, intensity);
                    }
                }
            }
        }

        /// <summary>
        /// Returns a string representation of all peaks with each peak on a separate line in the form: "mz intensity"
        /// </summary>
        /// <returns></returns>
        public string PeakListString()
        {
            StringBuilder peakList = new StringBuilder();
            foreach (var peak in Peaks)
            {
                peakList.AppendLine(String.Format("{0} {1}", peak.Key, peak.Value));
            }
            return peakList.ToString();
        }

    }
}
