using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.IO;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Viewer.Models
{
    public class Node : INotifyPropertyChanged
    {
        private bool isHashed;
        public ObservableCollection<Node>? FilesAndFolders { get; set; }
        public string NodeName { get; }
        public string FullPath { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void PropertyChangeNotification([CallerMemberName] String property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public Node(string _FullPath, bool isDisk)
        {
            FilesAndFolders = new ObservableCollection<Node>();
            FullPath = _FullPath;
            if (!isDisk)
                NodeName = Path.GetFileName(_FullPath);
            else
                NodeName = "Диск " + _FullPath.Substring(0, _FullPath.IndexOf(":"));
            isHashed = false;
        }

        public void GetFilesAndFolders()
        {
            if (!isHashed)
            {
                try
                {
                    IEnumerable<string> subdirs = Directory.EnumerateDirectories(FullPath, "*", SearchOption.TopDirectoryOnly);
                    foreach (string dir in subdirs)
                    {
                        Node thisnode = new Node(dir, false);
                        FilesAndFolders.Add(thisnode);
                    }

                    string[] allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    IEnumerable<string> files = Directory.EnumerateFiles(FullPath)
                        .Where(file => allowedExtensions.Any(file.ToLower().EndsWith))
                        .ToList();

                    foreach (string file in files)
                    {
                        FilesAndFolders.Add(new Node(file, false));
                    }
                }
                catch
                {

                }
                isHashed = true;
            }
        }
    }
}
