// Represents a command that can be sent to a Follower.
// The command needs to be be able to be serialized into a string.
// This string should avoid using newlines, as these act weirdly on Windows.


using System.Text;
using static Game;

public class ShowBoard : FollowerCommand
{
    private Field[,] state;
    private int score;
    private string commandType;

    // ShowBoard constructor
    public ShowBoard(Game.Field[,] state, int score)
    {
        commandType = "ShowBoard";
        this.state = state;
        this.score = score;
    }


    // StateToString method
    // Converts the game state to a string, by assigning each field type a number
    // and then concatenating all the numbers into a string.
    private string stateToString(Game.Field[,] state)
    {
        StringBuilder sb = new StringBuilder();
        for (int x = 0; x < state.GetLength(0); x++)
        {
            for (int y = 0; y < state.GetLength(1); y++)
            {
                // Serialize the field type to a number
                sb.Append(serializeField(state[x, y]));
            }
        }
        return sb.ToString();
    }

    private static string serializeField(Game.Field field)
    {
        switch (field)
        {
            case Field.Empty:
                return "0";
            case Field.Snake:
                return "1";
            case Field.SnakeHead:
                return "2";
            case Field.Apple:
                return "3";
        }
        Logger.Error("Invalid field type");
        Environment.Exit(1);
        return "";
    }

    private static Game.Field deserializeField(char field)
    {
        switch (field)
        {
            case '0':
                return Field.Empty;
            case '1':
                return Field.Snake;
            case '2':
                return Field.SnakeHead;
            case '3':
                return Field.Apple;
        }
        Logger.Error($"Invalid field type '{field}'");
        Environment.Exit(1);
        return Field.Empty;
    }

    public string Serialize()
    {
        return $"{commandType},{stateToString(state)}|{score}";
    }

    public static FollowerCommand Deserialize(string serialized)
    {
        string[] parts = serialized.Split(',');
        if (parts.Length != 2)
        {
            Logger.Error($"Invalid serialized command, expected 2 parts but got {parts.Length}");
            Environment.Exit(1);
        }
        if (parts[0] != "ShowBoard")
        {
            Logger.Error($"Invalid serialized command, expected 'ShowBoard' but got '{parts[0]}'");
            Environment.Exit(1);
        }
        string[] stateParts = parts[1].Split('|');
        if (stateParts.Length != 2)
        {
            Logger.Error($"Invalid serialized command, expected 2 parts in data (seperated by |) but got {stateParts.Length}");
            Environment.Exit(1);
        }
        string stateString = stateParts[1];
        
        // Split the score from the state by splitting on the pipe character
        if (!stateString.Split('|').Length.Equals(2))
        {
            Logger.Error($"Invalid serialized command. Either too many or too few pipe characters '{serialized}'");
            Environment.Exit(1);
        }
        string[] scoreParts = stateString.Split('|');
        if (!int.TryParse(scoreParts[1], out int score))
        {
            Logger.Error($"Invalid serialized command. Score is not an integer '{serialized}'");
            Environment.Exit(1);
        }
        stateString = scoreParts[0];


        Game.Field[,] state = new Game.Field[Snake.Settings.xWidthFollower, Snake.Settings.yHeightFollower];
        for (int x = 0; x < 30; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                state[x, y] = deserializeField(stateString[x * Snake.Settings.yHeightFollower + y]);
            }
        }
        return new ShowBoard(state, score);
    }

    public void Execute()
    {
        // Clear the screen
        Console.Clear();

        // Write on the first line that this is the second monitor
        Console.WriteLine($"Second monitor | Score: {score}", ConsoleColor.DarkGray);

        // Write the game state
        for (int y = 0; y < state.GetLength(1); y++)
        {

            // The portal is always on the left side of the screen for follower
            Console.Write(" ", ConsoleColor.Yellow);
            for (int x = 0; x < state.GetLength(0); x++)
            {
                switch (state[x, y])
                {
                    case Field.Empty:
                        Console.Write(" ", ConsoleColor.Black);
                        break;
                    case Field.Snake:
                        Console.Write(" ", ConsoleColor.Green);
                        break;
                    case Field.SnakeHead:
                        Console.Write(" ", ConsoleColor.DarkGreen);
                        break;
                    case Field.Apple:
                        Console.Write(" ", ConsoleColor.Red);
                        break;
                }
            }
            Console.WriteLine();
        }
    }

}