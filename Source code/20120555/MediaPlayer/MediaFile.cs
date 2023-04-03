using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPlayer
{
    public class MediaFile
    {
        public int Id { get; set; } = 0;
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public string Path { get; set; } = "";
        public string Type { get; set; } = "";
    }
}
