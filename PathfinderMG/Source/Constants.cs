﻿using Microsoft.Xna.Framework;

namespace PathfinderMG.Core.Source
{
    static class Constants
    {
        public const char NODE_EMPTY = '.';
        public const char NODE_WALL = '#';
        public const char NODE_START = 'S';
        public const char NODE_TARGET = 'T';

        public const int DEFAULT_NODE_SIZE = 50;
        public const int PREVIEW_NODE_SIZE = 30;

        public const int SCREEN_WIDTH = 1600;
        public const int SCREEN_HEIGHT = 900;
        public static readonly Rectangle SCREEN_RECT = new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT);

        public static System.Globalization.CultureInfo Culture = new System.Globalization.CultureInfo("pl-PL");
    }
}
