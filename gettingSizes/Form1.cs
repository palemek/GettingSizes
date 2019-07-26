using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gettingSizes
{
    public struct dispFile
    {
        public Rectangle rect;
        public string name;
        public dispFile(Rectangle _r, string _n)
        {
            rect = _r;
            name = _n;
        }
    }

    public struct dispDir
    {
        public Rectangle rect;
        public string name;

        List<dispDir> dirs; //it should be also recursive i think
        public List<dispFile> files;

        public dispDir(Rectangle _r, string _n, List<dispDir> _d, List<dispFile> _f)
        {
            rect = _r;
            name = _n;
            dirs = _d;
            files = _f;
        }
    }

    public partial class Form1 : Form
    {
        Program.data info;

        List<dispDir> dispDirs = new List<dispDir>();

        public Random rnd = new Random();

        public Form1(Program.data _info)
        {
            info = _info;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private List<int>sortDir(Program.data _info)
        {

            List<int> temp = new List<int>();

            for (int i = 0; i < _info.directories.Count + _info.files.Count; i++)
                temp.Add(i);

            for (int i = 0; i < temp.Count; i++)
            {
                var currSize = temp[i] < _info.directories.Count ? _info.directories[temp[i]].size : _info.files[temp[i] - _info.directories.Count].size;
                for (int j = i; j < temp.Count; j++)
                {
                    var checkSize = temp[j] < _info.directories.Count ? _info.directories[temp[j]].size : _info.files[temp[j] - _info.directories.Count].size;
                    if(checkSize>currSize)
                    {
                        int t = temp[i];
                        temp[i] = temp[j];
                        temp[j] = t;
                    }
                }
            }

            return temp;
        }


        static Pen pen = new Pen(Color.Black);
        static SolidBrush brush = new SolidBrush(Color.Black);


        public void drawDir(PaintEventArgs _e, Rectangle _rec, Program.data _info, string _dir = null)
        {
            //dispDir those;
            if (_info.directory != null)
            {
                _dir = _info.directory;
                //those = new dispDir(_rec, _dir, new List<dispDir>(),new List<dispFile>());
                //dispDirs.Add(those);
            }
            else
            {
                //those = dispDirs.Where(d => d.name == _dir).ToList()[0];
            }

            if (_info.files.Count + _info.directories.Count == 0)
                return;
            if(_info.files.Count + _info.directories.Count == 1)
            {
                if(_info.directories.Count==1)
                {
                    _info.directories[0].rec = _rec;

                    int recSize = 2;

                    Rectangle tempRec = new Rectangle(_rec.X + recSize, _rec.Y + recSize, _rec.Width - 2* recSize, _rec.Height - 2* recSize);
                    //_e.Graphics.FillRectangle(brush, tempRec);
                    
                    drawDir(_e, tempRec, _info.directories[0]);

                    _e.Graphics.DrawRectangle(new Pen(Color.FromArgb(rnd.Next(127), rnd.Next(127), rnd.Next(127)), recSize), _rec);

                    return;
                }
                else
                {
                    //those.files.Add(new dispFile(_rec, _info.files[0].name));
                    brush.Color = Color.FromArgb(128 + rnd.Next(127), 128 + rnd.Next(127), 128 + rnd.Next(127));
                    _info.files[0].rec = _rec;
                    _e.Graphics.FillRectangle(brush, _rec);
                    return;
                }
            }

            List<int> sI = sortDir(_info);

            long sumSize = 0;
            int endindex = 0;

            List<Program.data> tempDir1 = new List<Program.data>();
            List<Program.filedata> tempFiles1 = new List<Program.filedata>();

            for (int i = 0; i < sI.Count; i++)
            {
                bool isDir = sI[i] < _info.directories.Count;
                var currSize = isDir ? _info.directories[sI[i]].size : _info.files[sI[i] - _info.directories.Count].size;
                if (currSize + sumSize < _info.size / 2 || i == 0)
                {
                    if (isDir)
                        tempDir1.Add(_info.directories[sI[i]]);
                    else
                        tempFiles1.Add(_info.files[sI[i] - _info.directories.Count]);
                    sumSize += currSize;
                    endindex = i;
                }
                else
                    break;
            }


            var tempDir2 = new List<Program.data>();
            var tempFiles2 = new List<Program.filedata>();

            tempDir2 = _info.directories.Where(dir => !tempDir1.Contains(dir)).ToList();
            tempFiles2 = _info.files.Where(file => !tempFiles1.Contains(file)).ToList();

            Program.data dataInfo1 = new Program.data(null, sumSize,tempFiles1, tempDir1);
            Program.data dataInfo2 = new Program.data(null, _info.size - sumSize, tempFiles2, tempDir2);
            var rec1 = new Rectangle();
            var rec2 = new Rectangle();
            if (_rec.Width>_rec.Height)
            {
                rec1.X = _rec.X;
                rec1.Y = _rec.Y;
                rec1.Width = Convert.ToInt32(_rec.Width * sumSize / (_info.size==0?1:_info.size));
                rec1.Height = _rec.Height;
                rec2.X = _rec.X + rec1.Width;
                rec2.Y = _rec.Y;
                rec2.Width = _rec.Width - rec1.Width;
                rec2.Height = _rec.Height;
            }
            else
            {
                rec1.X = _rec.X;
                rec1.Y = _rec.Y;
                rec1.Width = _rec.Width;
                rec1.Height = Convert.ToInt32(_rec.Height * sumSize / (_info.size == 0 ? 1 : _info.size));
                rec2.X = _rec.X;
                rec2.Y = _rec.Y + rec1.Height;
                rec2.Width = _rec.Width;
                rec2.Height = _rec.Height - rec1.Height;
            }
            drawDir(_e, rec1, dataInfo1, _dir);
            drawDir(_e, rec2, dataInfo2, _dir);
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            drawDir(e, new Rectangle(0,0,pictureBox1.Width,pictureBox1.Height), info);
            var info2 = info;
            info2 = null;
        }

        private Program.filedata getClosest(Program.data _data, Point _mouse, ref List<Program.data> _folderData)
        {
            foreach(Program.filedata f in _data.files)
            {
                if ((_mouse.X > f.rec.X && _mouse.X < f.rec.X + f.rec.Width) && (_mouse.Y > f.rec.Y && _mouse.Y < f.rec.Y + f.rec.Height))
                    return f;
            }
            foreach(Program.data f in _data.directories)
            {
                if ((_mouse.X > f.rec.X && _mouse.X < f.rec.X + f.rec.Width) && (_mouse.Y > f.rec.Y && _mouse.Y < f.rec.Y + f.rec.Height))
                {
                    _folderData.Add(f);
                    return getClosest(f, _mouse, ref _folderData);
                }
            }
            return null;
        }

        Program.filedata closest = null;
        ToolTip tt;
        List<ToolTip> dirTT = new List<ToolTip>();
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            
            var position = pictureBox1.PointToClient(MousePosition);
            if (position.X < 0 || position.X > pictureBox1.Width || position.Y < 0 || position.Y > pictureBox1.Height)
                return;

            List<Program.data> folderData = new List<Program.data>();
            var closest2 = getClosest(info, position, ref folderData);

            folderData.Reverse();

            if (closest2 == null)
                return;
            
            if (closest != closest2)
            {
                for (int i = 0; i < folderData.Count(); i++)
                {
                    var d = folderData[i];
                    if (i >= dirTT.Count())
                    {
                        dirTT.Add(new ToolTip());
                    }

                    Point ttloc = new Point(d.rec.X + 16, d.rec.Y + 38);
                    string ttdisplay = "";
                    if (i == folderData.Count() - 1)
                        ttdisplay = d.directory;
                    else
                        ttdisplay = d.directory.Remove(0, folderData[i + 1].directory.Count());
                    
                    dirTT[i].Show(ttdisplay, this, ttloc);
                }
                if (dirTT.Count() >= folderData.Count())
                    for (int i = dirTT.Count() - 1; i >= folderData.Count(); i--)
                        dirTT[i].Hide(this);
            }

            
            closest = closest2;
            if (tt == null)
            {
                tt = new ToolTip();
            }
            string sizeText = "";
            long sizeint = closest.size;
            if (sizeint >= 1024)
            {
                sizeint /= 1024;
                if (sizeint >= 1024)
                {
                    sizeint /= 1024;
                    if (sizeint >= 1024)
                    {
                        sizeint /= 1024;
                        sizeText = sizeint + "GB";
                    }
                    else
                    {
                        sizeText = sizeint + "MB";
                    }
                }
                else
                {
                    sizeText = sizeint + "kB";
                }
            }
            else
            {
                sizeText = sizeint + "B";
            }
            tt.Show(closest.name + ", " + sizeText, this, new Point(position.X + 16, position.Y + 38));
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (closest != null)
            {
                Console.WriteLine(closest.adress);
                Process.Start("explorer.exe", "/select, @" + closest.adress);
            }
                
        }
    }
}
