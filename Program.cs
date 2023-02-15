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
    leader.run();
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



