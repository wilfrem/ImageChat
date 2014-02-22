using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageChat
{
    public class Room
    {
        public string Name { get; private set; }
        public string ImageUrl { get; private set; }

        public Room(string name, string imageUrl)
        {
            Name = name;
            ImageUrl = imageUrl;
        }
    }
}
