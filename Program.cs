// A proof of concept to see that a
// client can connect to a server and send data

// check the first argument to see if it is a leader or follower

int port = 5000; // port to use

// if the first argument is "leader" then create a leader
if (args[0] == "leader")
{
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

    // Show the game state
    Console.WriteLine(game.View());

    // Have a thread that listens for keystrokes
    Thread thread = new Thread(() =>
    {
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

    thread.Start();
    // Show 5 steps of the game
    for (int i = 0; i < 50; i++)
    {
        
        string view = game.View();
        // Draw the game with a border,
        // orange for the snake head and body,
        // red for the apple and
        // gray for the empty space

        Console.Clear();

        string[] lines = view.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
        Console.WriteLine("┌" + new string('─', lines[0].Length) + "┐");
        for (int j = 0; j < lines.Length; j++)
        {
            // Skip the printing if line is empty
            if (lines[j].Length == 0)
                continue;

            Console.Write("│" );
            for (int k = 0; k < lines[j].Length ; k++)
            {

                if (lines[j][k] == 'X')
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write("X");
                    Console.ResetColor();
                }
                else if (lines[j][k] == 'x')
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write("x");
                    Console.ResetColor();
                }
                else if (lines[j][k] == 'O')
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write("O");
                    Console.ResetColor();
                }
                else
                {
                    Console.Write(" ");
                    Console.ResetColor();
                }
            }
            Console.WriteLine("│");
        }
        Console.WriteLine("└" + new string('─', lines[0].Length) + "┘");

        // Wait for a second
        Thread.Sleep(1000);

        game.Step();

    }
}

// if the first argument is "follower" then create a follower
else if (args[0] == "follower")
{
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



