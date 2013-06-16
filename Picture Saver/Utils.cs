using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PictureSaver
{
    public static class Utils
    {
        public static bool IsDirectory(string path)
        {
            DirectoryInfo info = new DirectoryInfo(path);
            return info.Exists;
        }

        public static bool IsFile(string path)
        {
            FileInfo info = new FileInfo(path);
            return info.Exists;
        }

        /// <summary>
        /// Extracts the last set of numbers from a string, returns them as a string.
        /// </summary>
        /// <param name="expr">String to extract from.</param>
        /// <returns>Last string of numbers found, blank if no numbers found.</returns>
        public static string ExtractNumbers(string expr)
        {
            List<string> numWords = new List<string>();
            string currWord = "";
            numWords.Add(currWord);

            int temp;

            foreach (char character in expr)
            {
                if (char.IsDigit(character))
                {
                    currWord += character;
                }
                else
                {
                    if (int.TryParse(currWord, out temp))
                        numWords.Add(currWord);
                    currWord = "";
                }
            }

            if (int.TryParse(currWord, out temp))
                numWords.Add(currWord);

            return numWords.Last();
        }

        /// <summary>
        /// Appends a number to the end of a file path, before the file extension.
        /// For num = 1
        /// C:/Path/To/File/File.file -> C:/Path/To/File/File1.file
        /// </summary>
        /// <param name="path">Path to append.</param>
        /// <param name="num">Number to append to path.</param>
        /// <returns>File path with number appended to filename.</returns>
        public static string AppendNumberToFile(string path, int num)
        {
            string baseFilename = Path.GetFileNameWithoutExtension(path);
            return path.Replace(baseFilename, string.Format("{0}{1}", baseFilename, num));
        }

        /// <summary>
        /// Compares the byte contents of two files, returns true if they are the same, false otherwise.
        /// </summary>
        /// <param name="file1"></param>
        /// <param name="file2"></param>
        /// <returns></returns>
        public static bool FileCompare(string file1, string file2)
        {
            int file1byte;
            int file2byte;
            FileStream fs1;
            FileStream fs2;

            // Determine if the same file was referenced two times.
            if (file1 == file2)
            {
                // Return true to indicate that the files are the same.
                return true;
            }

            // Open the two files.
            fs1 = new FileStream(file1, FileMode.Open);
            fs2 = new FileStream(file2, FileMode.Open);

            // Check the file sizes. If they are not the same, the files 
            // are not the same.
            if (fs1.Length != fs2.Length)
            {
                // Close the file
                fs1.Close();
                fs2.Close();

                // Return false to indicate files are different
                return false;
            }

            // Read and compare a byte from each file until either a
            // non-matching set of bytes is found or until the end of
            // file1 is reached.
            do
            {
                // Read one byte from each file.
                file1byte = fs1.ReadByte();
                file2byte = fs2.ReadByte();
            }
            while ((file1byte == file2byte) && (file1byte != -1));

            // Close the files.
            fs1.Close();
            fs2.Close();

            // Return the success of the comparison. "file1byte" is 
            // equal to "file2byte" at this point only if the files are 
            // the same.
            return ((file1byte - file2byte) == 0);
        }
    }
}
