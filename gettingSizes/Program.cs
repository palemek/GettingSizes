using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gettingSizes
{
    public class Program
    {
        public class data
        {
            public string           directory;
            public long             size;
            public List<filedata>   files;
            public List<data>       directories;
            public Rectangle        rec;

            public data(string _dir, long _size,List<filedata> _files, List<data> _dirs)
            {
                directory = _dir;
                size = _size;
                files = _files;
                directories = _dirs;
                rec = new Rectangle();
            }
        }

        public class filedata
        {
            public string  adress;
            public long size;
            public string name;
            public Rectangle rec;

            public filedata(string _adr, long _size, string _name)
            {
                adress = _adr;
                size = _size;
                name = _name;
                rec = new Rectangle();
            }
        }

        static void Main(string[] args)
        {
            string loc = "";
            loc = Console.ReadLine();
            data allData = getThem(loc);
            long size = allData.size;
            if(size<1024)
                Console.WriteLine("allsize: " + size + "B");
            else
                if (size/1024 < 1024)
                    Console.WriteLine("allsize: " + size/1024 + "KB");
                else
                    if (size / (1024*1024) < 1024)
                        Console.WriteLine("allsize: " + size / (1024*1024) + "MB");
                    else
                        Console.WriteLine("allsize: " + size / (1024 * 1024 * 1024) + "GB");

            var frm = new Form1(allData);
            Application.Run(frm);

            Console.Read();

        }

        static List<data> directoriesData = new List<data>();

        static data getThem(string sdir)
        {
            Console.WriteLine(sdir + ":");

            string tempDirectory = sdir;
            long tempSize = 0;
            List<filedata> tempFiles = new List<filedata>();
            List<data> tempDirectories = new List<data>();
            try
            {
                foreach (string f in Directory.GetFiles(sdir))
                {
                    long size = new FileInfo(f).Length;
                    string name = new FileInfo(f).Name;
                    tempFiles.Add(new filedata(f, size, name));
                    tempSize += size;
                    /*
                    Console.Write("\t" + name);
                    Console.CursorLeft = Console.BufferWidth - 20;
                    Console.Write(size);
                    Console.WriteLine("");*/
                }
            }
            catch
            {
                Console.WriteLine("WRONG DIRECTORY: " + sdir);
            }
            try
            {
                foreach (string f in Directory.GetDirectories(sdir))
                {
                    data dir = getThem(f);
                    tempDirectories.Add(dir);
                    tempSize += dir.size;
                }
            }
            catch
            {
                Console.WriteLine("WRONG DIRECTORY: " + sdir);
            }
            return new data(tempDirectory, tempSize, tempFiles, tempDirectories);
        }
    }
}
