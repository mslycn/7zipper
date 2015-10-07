using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace _7zipper
{
    public partial class Form1 : Form
    {
        private FileListViewHandler _fileListViewHandler;
        public Form1()
        {
            InitializeComponent();
            fileListView.Columns.Add("Name", 75);
            fileListView.Columns.Add("Path", -2);
            fileListView.View = View.Details;
            _fileListViewHandler=new FileListViewHandler(fileListView);
        }

        private void fileListView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void fileListView_DragDrop(object sender, DragEventArgs e)
        {
            var paths = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (paths == null) return;
            foreach (var path in paths)
            {
                _fileListViewHandler.Add(path);
            }
        }
    }

    class FileListViewHandler
    {
        private readonly ListView _listView;
        private readonly Zipper _zipper;
        public FileListViewHandler(ListView listView)
        {
            _listView = listView;
            _zipper= new Zipper();
        }

        public void Add(string path)
        {
            if (IsPathExists(path)) return;
            _listView.Items.Add(new ListViewItem(new[] { GetName(path), path }));
        }

        public void Zip()
        {
            foreach (ListViewItem item in _listView.Items)
            {
                _zipper.AddFile(new Tuple<string, string>(item.SubItems[0].Text, item.SubItems[1].Text));
            }
            _zipper.Execute();
        }

        private bool IsNameExists(string name)
        {
            return _listView.Items.Cast<ListViewItem>().Any(item => item.SubItems[0].Text == name);
        }

        private bool IsPathExists(string path)
        {
            return _listView.Items.Cast<ListViewItem>().Any(item => item.SubItems[1].Text == path);
        }

        private string GetName(string path)
        {
            var fileName = Path.GetFileName(path);
            var newName = fileName;
            var i = 1;
            while (IsNameExists(newName))
            {
                newName = fileName + "(" + i + ")";
                i++;
            }
            return newName;
        }
    }
}
