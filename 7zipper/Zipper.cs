using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace _7zipper
{
    class Zipper
    {
        public delegate void ZipperProgressUpdate(object sender, ProgressEventArgs e);
        public event ZipperProgressUpdate ProgressState;

        private readonly List<Tuple<string,string>> _filesList; // Item 1 Path, Item 2 Output

        public Zipper()
        {
            _filesList = new List<Tuple<string, string>>();
        }

        public void AddFile(params Tuple<string, string>[] files)
        {
            // Add unique file
            foreach (var file in files.Where(file => !_filesList.Contains(file)))
            {
                _filesList.Add(file);
            }
        }

        public void Clear()
        {
            _filesList.Clear();
        }

        public void Execute()
        {
            var count = 0;
            foreach (var file in _filesList)
            {
                ReportProgress(count * 100 / _filesList.Count);
                CompressFileLzma(file.Item1, file.Item2);
                count++;
            }
            ReportProgress(100);
        }

        private static void CompressFileLzma(string inFile, string outFile)
        {
            var coder = new SevenZip.Compression.LZMA.Encoder();
            var input = new FileStream(inFile, FileMode.Open);
            var output = new FileStream(outFile, FileMode.Create);

            // Write the encoder properties
            coder.WriteCoderProperties(output);

            // Write the decompressed file size.
            output.Write(BitConverter.GetBytes(input.Length), 0, 8);

            // Encode the file.
            coder.Code(input, output, input.Length, -1, null);
            output.Flush();
            output.Close();
        }

        private void ReportProgress(int progress)
        {
            var args = new ProgressEventArgs(progress);
            ProgressState?.Invoke(this, args);
        }
    }

    class ProgressEventArgs : EventArgs
    {
        public int Progress { get; private set; }
        public ProgressEventArgs(int progress)
        {
            Progress = progress;
        }
    }
}
