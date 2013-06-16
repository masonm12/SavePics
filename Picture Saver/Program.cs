using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PictureSaver
{
    public static class Program
    {
        static private DirectoryInfo pictureDirectory;

        private static void SetPictureDirectory(string subDir = null)
        {
            pictureDirectory = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
            if (subDir != null)
            {
                string subDirPath = string.Format("{0}\\{1}", pictureDirectory.FullName, subDir);
                if (!Directory.Exists(subDirPath))
                {
                    Directory.CreateDirectory(subDirPath);
                }

                pictureDirectory = new DirectoryInfo(subDirPath);
            }
        }

        private static void ProcessPath(string path)
        {
            if (Utils.IsDirectory(path))
            {
                ProcessDirectory(path);
            }
            else if (Utils.IsFile(path))
            {
                ProcessFile(path);
            }
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
                if (Utils.FileCompare(files[0].FullName, info.FullName))
                {
                    Console.WriteLine("File {0} already exists", info.Name);
                    return;
                }
                else
                {
                    Console.WriteLine("File name {0} already exists", info.Name);
                }

                string numbers = Utils.ExtractNumbers(Path.GetFileNameWithoutExtension(newFullPath));
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

        public static void ExecuteCommand(Command command)
        {
            switch (command.Type)
            {
                case CommandType.Path:
                    ProcessPath(command.Value);
                    break;
                case CommandType.SubDir:
                    SetPictureDirectory(command.Args[0]);
                    break;
            }
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("Picture Saver");

            if (args.Length == 0)
            {
                Console.WriteLine("Usage: directory [options]");
            }

            SetPictureDirectory();
            Command lastCommand = null;
            foreach (string arg in args)
            {
                if (lastCommand == null || lastCommand.RemainingArgCount == 0)
                {
                    lastCommand = new Command(arg);
                }
                else
                {
                    lastCommand.AddArg(arg);
                }

                if (lastCommand.RemainingArgCount == 0)
                {
                    ExecuteCommand(lastCommand);
                }
            }
        }
    }
}
