﻿namespace Twitchy.api
{
    public class StreamObject
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
