// A proof of concept to see that a
// client can connect to a server and send data

// check the first argument to see if it is a leader or follower

int port = 5000; // port to use

// if the first argument is "leader" then create a leader
if (args[0] == "leader")
{
    // Set thread name to LeaderMain
    Thread.CurrentThread.Name = "LeaderMain";

    // create a leader
    Leader leader = new Leader("127.0.0.1", port);
    // Send 10 commands to the follower in 1 sec intervals
    // for (int i = 0; i < 10; i++)
    // {
    //     leader.sendCommand($"Hello world! {i}");
    //     Thread.Sleep(1000);
    // }

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
    // Show 5 steps of the game
    for (int i = 0; i < 50; i++)
    {
        
        string leaderView = game.View(true);
        string followerView = game.View(false);

        // Trim the end of the string
        leaderView = leaderView.TrimEnd(Environment.NewLine.ToCharArray());
        followerView = followerView.TrimEnd(Environment.NewLine.ToCharArray());

        // Send the game state to the follower
        leader.sendCommand(followerView);

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
            Logger.Debug("Game over");
            break;
        }

        string[] lines = leaderView.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
        Logger.Debug($"Length of lines: {lines.Length}");
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
        Thread.Sleep(1000);

        game.Step();

    }
}

// if the first argument is "follower" then create a follower
else if (args[0] == "follower")
{
    // Set thread name to FollowerMain
    Thread.CurrentThread.Name = "FollowerMain";
    // create a follower
    Follower follower = new Follower("127.0.0.1", port);
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



