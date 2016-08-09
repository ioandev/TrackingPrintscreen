using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackPrintScreen
{
    public static class OptionsFiledatabase
    {
        public static Options Load(string path)
        {
            string data = System.IO.File.ReadAllText(path);
            return new Options
            {
                FolderPath = data
            };
        }
        public static void Save(string path, Options options)
        {
            System.IO.File.WriteAllText(path, options.FolderPath);
        }
    }
}
