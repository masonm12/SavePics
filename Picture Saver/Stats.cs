using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PictureSaver
{
    public class Stats
    {
        private int picturesScanned;
        private int picturesSaved;
        private int duplicatesFound;
        private int picturesRenamed;

        public int PicturesScanned { get { return picturesScanned; } set { picturesScanned = value; } }
        public int PicturesSaved { get { return picturesSaved; } set { picturesSaved = value; } }
        public int DuplicatesFound { get { return duplicatesFound; } set { duplicatesFound = value; } }
        public int PicturesRenamed { get { return picturesRenamed; } set { picturesRenamed = value; } }

        public Stats()
        {
            picturesScanned = 0;
            picturesSaved = 0;
            duplicatesFound = 0;
            picturesRenamed = 0;
        }

        public void Print()
        {
            Console.Write("Pictures Scanned: ");
            Console.WriteLine(picturesScanned);
            Console.Write("Pictures Saved: ");
            Console.WriteLine(picturesSaved);
            Console.Write("Duplicates Found: ");
            Console.WriteLine(duplicatesFound);
            Console.Write("Pictures Renamed: ");
            Console.WriteLine(picturesRenamed);
        }
    }
}
