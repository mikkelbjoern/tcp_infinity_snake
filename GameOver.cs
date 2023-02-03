// The command that is sent to the follower when the game is over

class GameOver : FollowerCommand
{
    // The score of the game
    private int score;

    // GameOver constructor
    public GameOver(int score)
    {
        this.score = score;
    }

    // Serialize the command
    public string Serialize()
    {
        return "GameOver," + score;
    }

    // Deserialize the command
    public static FollowerCommand Deserialize(string serialized)
    {
        // Split the string into the command type and the score
        string[] split = serialized.Split(',');
        // Check if the command type is correct
        if (split[0] != "GameOver")
        {
            Logger.Error("Invalid command type");
            Environment.Exit(1);
        }
        // Check if the score is a valid integer
        if (!int.TryParse(split[1], out int score))
        {
            Logger.Error("Invalid score");
            Environment.Exit(1);
        }
        // Return a new GameOver command
        return new GameOver(score);
    }

    // Execute the command
    public void Execute()
    {
        // Print the score
        Console.WriteLine("Game over, Follower! Your final score was: " + score);
        // Exit the program
        Environment.Exit(0);
    }
}