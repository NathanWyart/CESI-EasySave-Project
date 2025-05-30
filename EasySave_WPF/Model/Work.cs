using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave_WPF.Model
{
    // This class represents a work to be done.
    public class Work
    {
        public string Name { get; set; }
        public string Src { get; set; }
        public string Dst { get; set; }
        public BackupType BackupType { get; set; }
        public DateTime? LastBackupDate { get; set; }

        // Constructor to initialize a work.
        public Work() { } 

        // Constructor to initialize a work with parameters.
        public Work(string name, string src, string dst, BackupType type)
        {
            Name = name;
            Src = src;
            Dst = dst;
            BackupType = type;
            LastBackupDate = DateTime.Now;
        }
    }
}
