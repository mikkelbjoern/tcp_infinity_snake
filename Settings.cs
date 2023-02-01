// Defines the global settings for the application

namespace Snake
{
    public static class Settings
    {
        // The width and height of the game state
        public static int xWidth = 30;
        public static int yHeight = 10;

        public static int xWidthLeader = 15;
        public static int yHeightLeader = yHeight;

        public static int xWidthFollower = xWidth - xWidthLeader;
        public static int yHeightFollower = yHeight;


    }
}