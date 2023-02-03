// Check that two arguments are passed
if (args.Length != 2)
{
    Logger.Error("Invalid number of arguments");
    Logger.Error("Usage: dotnet run [leader|follower] [follower-ip]");
    Environment.Exit(1);
}

// Check that the first argument is either "leader" or "follower"
if (args[0] != "leader" && args[0] != "follower")
{
    Logger.Error("Invalid first argument");
    Logger.Error("Usage: dotnet run [leader|follower] [follower-ip]");
    Environment.Exit(1);
}


// Define a variable to store the IP address
System.Net.IPAddress ip;

// Check that the second argument is a valid IP address
try
{
    ip = System.Net.IPAddress.Parse(args[1]);
}
catch (System.FormatException)
{
    Logger.Error("Invalid IP address");
    Logger.Error("Usage: dotnet run [leader|follower] [ip]");
    Environment.Exit(1);
}

Console.WriteLine("Starting client");

// if the first argument is "leader" then create a leader
if (args[0] == "leader")
{
    // Set thread name to LeaderMain
    Thread.CurrentThread.Name = "LeaderMain";

    ip = System.Net.IPAddress.Parse(args[1]);

    // create a leader
    Leader leader = new Leader(ip, Snake.Settings.port);

    // Create a new game
    Game game = new Game();

    // Have a thread that listens for keystrokes
    Thread thread = new Thread(() =>
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
    thread.Name = "ControlThread";

    thread.Start();
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
        leader.sendCommand(commandShowBoard);

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
            leader.sendCommand(commandGameOver);
            Logger.Debug("Game over");
            Environment.Exit(0);
        }

        // Wait for a second
        Thread.Sleep(400);

        game.Step();

    }
}

// if the first argument is "follower" then create a follower
else if (args[0] == "follower")
{
    ip = System.Net.IPAddress.Parse(args[1]);
    // Set thread name to FollowerMain
    Thread.CurrentThread.Name = "FollowerMain";
    // create a follower
    Follower follower = new Follower(ip, Snake.Settings.port);

    // Wait for the follower to receive a command
    while (true)
    {
        follower.waitForCommand();
    }
}

// if the first argument is neither "leader" or "follower" then print an error
else
{
    Console.WriteLine("Error: First argument must be either \"leader\" or \"follower\"");
}



