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
        
        string leaderView = game.View(true);

        // Trim the end of the string
        leaderView = leaderView.TrimEnd(Environment.NewLine.ToCharArray());

        // Create a follower command from the game state
        // Seperate the game state that the follower need out
        // That is only the right side of the board
        // and the score
        Game.Field[,] rightSide = new Game.Field[Snake.Settings.xWidthFollower, Snake.Settings.yHeightFollower];

        // Copy the right side of the board to the rightSide array
        for (int i = 0; i < Snake.Settings.xWidthFollower; i++)
        {
            for (int j = 0; j < Snake.Settings.yHeightFollower; j++)
            {
                rightSide[i, j] = game.state[i + Snake.Settings.xWidthLeader, j];
            }
        }

        FollowerCommand commandShowBoard = new ShowBoard(rightSide, game.score);

        // Send the game state to the follower
        leader.sendCommand(commandShowBoard);

        // Draw the game with a border,
        // orange for the snake head and body,
        // red for the apple and
        // gray for the empty space

        Console.Clear();

        // Check if the game is over and don't format the string if it is
        // Just check if the string "game over" is in the string
        if (leaderView.ToLower().Contains("game over"))
        {
            Console.WriteLine(leaderView);
            // Send a game over command to the follower
            int score = game.score;
            FollowerCommand commandGameOver = new GameOver(score);
            leader.sendCommand(commandGameOver);
            Logger.Debug("Game over");
            Environment.Exit(0);
        }

        string[] lines = leaderView.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
        Logger.Debug($"Length of lines: {lines.Length}");

        Console.WriteLine($"Leader monitor | Score: {game.score}");
        for (int j = 0; j < lines.Length; j++)
        {
            // Skip the printing if line is empty
            if (lines[j].Length == 0) {
                // Print to System.Err that the line is empty with a timestamp
                // and the line number
                Logger.Debug($"{DateTime.Now} Line {j} is empty");
                continue;
            }

            // Print a line at the top of the screen
            if (j == 0)
            {
                Console.WriteLine("┌" + new string('─', lines[j].Length) + "┐");
            }

            Console.Write("│", ConsoleColor.DarkGray);

            // Print each character in the line
            for (int k = 0; k < lines[j].Length; k++)
            {
                Console.Write(lines[j][k]);
            }
            // Print single character with a yellow color
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.Write("│");
            Console.ResetColor();

            Console.WriteLine();

        }
        // Sometimes index 0 is empty so use length of 1
        Console.WriteLine("└" + new string('─', lines[1].Length) + "┘");


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



