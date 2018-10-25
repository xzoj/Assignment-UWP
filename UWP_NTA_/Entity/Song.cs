using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_NTA_.Entity
{
    class Song
    {
        private string _name;
        private string _singer;
        private string _author;
        private string _thumbnail;
        private string _link;

        public string Name { get => _name; set => _name = value; }
        public string Singer { get => _singer; set => _singer = value; }
        public string Author { get => _author; set => _author = value; }
        public string Thumbnail { get => _thumbnail; set => _thumbnail = value; }
        public string Link { get => _link; set => _link = value; }
    }
}
