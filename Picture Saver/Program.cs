using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace Picture_Saver
{
    static class Program
    {
        static private DirectoryInfo pictureDirectory;

        static Program()
        {
            pictureDirectory = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
        }

        private static bool IsDirectory(string path)
        {
            DirectoryInfo info = new DirectoryInfo(path);
            return info.Exists;
        }

        private static bool IsFile(string path)
        {
            FileInfo info = new FileInfo(path);
            return info.Exists;
        }

        private static void ProcessPath(string path)
        {
            if (IsDirectory(path))
                ProcessDirectory(path);
            else if (IsFile(path))
                ProcessFile(path);
        }

        private static void ProcessDirectory(string path)
        {
            DirectoryInfo info = new DirectoryInfo(path);
            FileInfo[] files = info.GetFiles("*", SearchOption.AllDirectories);

            foreach (FileInfo file in files)
            {
                if(file.Exists)
                    ProcessFile(file);
            }
        }

        private static void ProcessFile(string path)
        {
            ProcessFile(new FileInfo(path));
        }

        private static void ProcessFile(FileInfo info)
        {
            DateTime time = info.LastWriteTime;
            string newPath = string.Format("{0}\\{1}\\{2:00}", pictureDirectory.FullName, time.Year, time.Month);
            string newFilename = info.Name;

            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }

            DirectoryInfo directoryInfo = new DirectoryInfo(newPath);

            string newFullPath = string.Format("{0}\\{1}", newPath, newFilename);

            FileInfo[] files = directoryInfo.GetFiles(string.Format("{0}{1}", Path.GetFileNameWithoutExtension(newFullPath), "*"));
            while (files.Length > 0)
            {
                if (FileCompare(files[0].FullName, info.FullName))
                {
                    Console.WriteLine("File {0} already exists", info.Name);
                    return;
                }
                else
                {
                    Console.WriteLine("File name {0} already exists", info.Name);
                }

                string numbers = ExtractNumbers(Path.GetFileNameWithoutExtension(newFullPath));
                string format = "{0:";
                for (int i = 0; i < numbers.Length; ++i)
                {
                    format += "0";
                }
                format += "}";
                int count;
                if (!int.TryParse(numbers, out count))
                {
                    return;
                }
                count++;
                newFullPath = newFullPath.Replace(numbers, string.Format(format, count));
                files = directoryInfo.GetFiles(string.Format("{0}{1}", Path.GetFileNameWithoutExtension(newFullPath), "*"));
            }

            File.Copy(info.FullName, newFullPath);
        }

        public static void Main(string[] args)
        {
            foreach (string path in args)
            {
                ProcessPath(path);
            }
        }

        private static string ExtractNumbers(string expr)
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
                    if(int.TryParse(currWord, out temp))
                        numWords.Add(currWord);
                    currWord = "";
                }
            }

            if (int.TryParse(currWord, out temp))
                numWords.Add(currWord);

            return numWords.Last();
        }

        private static bool FileCompare(string file1, string file2)
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
