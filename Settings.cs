// Defines the global settings for the application

namespace Snake
{
    public static class Settings
    {

        // The port on which the client and server will communicate
        public static int port = 5000;

        // The width and height of the game state
        public static int xWidth = 20;
        public static int yHeight = 6;

        public static int xWidthLeader = 10;
        public static int yHeightLeader = yHeight;

        public static int xWidthFollower = xWidth - xWidthLeader;
        public static int yHeightFollower = yHeight;


    }
}