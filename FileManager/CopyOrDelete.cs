using System.IO;

namespace FileManager
{
    class CopyOrDelete
    {

        public string bufferPath { get; set; }
        public bool copy { get; set; }

        private static void CopyAllDirectory(string pathFrom, string pathTo)
        {

            DirectoryInfo directory = new DirectoryInfo(pathFrom);
            DirectoryInfo[] newDirectories = directory.GetDirectories();
            FileInfo[] newFiles = directory.GetFiles();
            Directory.CreateDirectory(pathTo);
            foreach (var item in newFiles)
            {
                string resultPath = Path.Combine(pathTo, item.Name);
                item.CopyTo(resultPath);
            }
            foreach (var item in newDirectories)
            {

                string resultPathFrom = Path.Combine(pathFrom, item.Name);
                string resultPathTo = Path.Combine(pathTo, item.Name);
                CopyAllDirectory(resultPathFrom, resultPathTo);

            }

        }


        public static void CopyAllDirectoryOrFile(string pathFrom, string pathTo)
        {
            if (File.Exists(pathFrom))
            {
                FileInfo file = new FileInfo(pathFrom);
                string resultPath = Path.Combine(pathTo, file.Name);
                file.CopyTo(resultPath);
            }
            else
            {
                FileInfo fileInfo = new FileInfo(pathFrom);
                string pathToTotal = Path.Combine(pathTo, fileInfo.Name);
                CopyAllDirectory(pathFrom, pathToTotal);
            }
        }


        public static void DeleteAllDirectoryOrFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            else
            {
                Directory.Delete(path, true);
            }
        
        }

    }
}
