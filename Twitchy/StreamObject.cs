using System;
using System.Collections.Generic;
using System.Text;

namespace Twitchy
{
    class StreamObject
    {
        public string name { get; set; }
        public string game { get; set; }
        public string title { get; set; }

        public StreamObject(string name, string game, string title)
        {
            this.name = name;
            this.game = game;
            this.title = title;
        }
    }
}
