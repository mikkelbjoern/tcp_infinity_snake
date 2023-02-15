// Represents the leader client

// The leader client is the client that sends commands to the followers
// To create a leader client, an IP address and a port number must be specified
// The leader client will then open a TCP client, where it will send data to the follower
// Initial action (development): The leader client will send a string to the follower
// and then close the connection

using System.Net.Sockets;

public class Leader
{
    
        private string ip;
        private int port;
        private Thread controlThread;
        public Game game;
    
        // Constructor
        // When a Leader has finished its construction, 
        // it has established connection with the Follower.
        public Leader(System.Net.IPAddress ip, int port)
        {
            this.ip = ip.ToString();
            this.port = port;
            // Create a new game
            game = new Game();

            // Have a thread that listens for keystrokes
            controlThread = new Thread(() =>
            {
                Logger.Debug("Starting control thread");
                // Listen for keystrokes
                while (true)
                {
                    // Get the key that was pressed
                    ConsoleKeyInfo key = Console.ReadKey(true);

                    // Up arrow
                    if (key.Key == ConsoleKey.UpArrow)
                    {
                        // Update the game state
                        game.Update(Game.Action.Up);
                    }
                    // Down arrow
                    else if (key.Key == ConsoleKey.DownArrow)
                    {
                        // Update the game state
                        game.Update(Game.Action.Down);
                    }
                    // Left arrow
                    else if (key.Key == ConsoleKey.LeftArrow)
                    {
                        // Update the game state
                        game.Update(Game.Action.Left);
                    }
                    // Right arrow
                    else if (key.Key == ConsoleKey.RightArrow)
                    {
                        // Update the game state
                        game.Update(Game.Action.Right);
                    }
                }
            });

            // Set thread name
            controlThread.Name = "ControlThread";

            controlThread.Start();

            Logger.Debug($"Leader created with IP {this.ip} and port {this.port}");
        }

        // This method will run the game on the leader side
        public Task run()
        {
            while (true)
            {
                
                // Create a follower command from the game state
                // Seperate the game state that the follower need out
                // That is only the right side of the board
                // and the score
                Game.Field[,] rightSide = new Game.Field[Snake.Settings.xWidthFollower, Snake.Settings.yHeightFollower];
                Game.Field[,] leftSide = new Game.Field[Snake.Settings.xWidthLeader, Snake.Settings.yHeightLeader];

                // Copy the right side of the board to the rightSide array
                for (int i = 0; i < Snake.Settings.xWidthFollower; i++)
                {
                    for (int j = 0; j < Snake.Settings.yHeightFollower; j++)
                    {
                        rightSide[i, j] = game.state[i + Snake.Settings.xWidthLeader, j];
                    }
                }

                // Copy the left side of the board to the leftSide array
                for (int i = 0; i < Snake.Settings.xWidthLeader; i++)
                {
                    for (int j = 0; j < Snake.Settings.yHeightLeader; j++)
                    {
                        leftSide[i, j] = game.state[i, j];
                    }
                }

                // Send the game state to the follower
                FollowerCommand commandShowBoard = new ShowBoard(rightSide, game.score);
                sendCommand(commandShowBoard);

                // Show the game state on the leader monitor
                Game.View(leftSide, game.score, Game.PortalSide.Right);

                // Check if the game is over and don't format the string if it is
                // Just check if the string "game over" is in the string
                if (game.gameOver)
                {
                    Console.WriteLine("Game over, Leader! The final score is {0}", game.score);
                    // Send a game over command to the follower
                    int score = game.score;
                    FollowerCommand commandGameOver = new GameOver(score);
                    sendCommand(commandGameOver);
                    Logger.Debug("Game over");
                    Environment.Exit(0);
                }

                // Wait for a second
                Thread.Sleep(400);

                game.Step();

            }
        }
    
        // Sends a command to the follower
        // This method will block until the follower has received the command
        // and should be run continously sending commands
        public void sendCommand(FollowerCommand command)
        {
            TcpClient tcpClient = new TcpClient(ip, port);
            // Get the stream from the TCP client
            NetworkStream stream = tcpClient.GetStream();
            // Create a buffer to store the data
            // We add a semicolon to the end of the command so the receiver knows when the command ends
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(command.Serialize() + ";");
            // Write the data to the stream
            stream.Write(data, 0, data.Length);
            // Close the TCP client
            tcpClient.Close();
        }
}