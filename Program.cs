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
    for (int i = 0; i < 10; i++)
    {
        leader.sendCommand($"Hello world! {i}");
        Thread.Sleep(1000);
    }
}

// if the first argument is "follower" then create a follower
else if (args[0] == "follower")
{
    // create a follower
    Follower follower = new Follower("127.0.0.1", port);
    // Wait for the follower to receive a command
    while (true) {
        follower.waitForCommand();
    }
}

// if the first argument is neither "leader" or "follower" then print an error
else
{
    Console.WriteLine("Error: First argument must be either \"leader\" or \"follower\"");
}



