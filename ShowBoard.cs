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
        string command = $"{commandType},{stateToString(state)}-{score}";
        Logger.Debug($"Serialized command: {command}");
        return command;
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
        string[] stateParts = parts[1].Split('-');
        if (stateParts.Length != 2)
        {
            Logger.Error($"Invalid serialized command, expected 2 parts in data (seperated by -) but got {stateParts.Length}");
            Environment.Exit(1);
        }
        string stateString = stateParts[0];

        if (!int.TryParse(stateParts[1], out int score))
        {
            Logger.Error($"Invalid serialized command. Score is not an integer '{stateParts[1]}'");
            Environment.Exit(1);
        }

        Game.Field[,] state = new Game.Field[Snake.Settings.xWidthFollower, Snake.Settings.yHeightFollower];

        // Make sure that the state string is the correct length
        if (stateString.Length != Snake.Settings.xWidthFollower * Snake.Settings.yHeightFollower)
        {
            Logger.Error($"Invalid serialized command. State string is not the correct length. Expected {Snake.Settings.xWidthFollower * Snake.Settings.yHeightFollower} but got {stateString.Length}");
            Environment.Exit(1);
        }
        for (int x = 0; x < Snake.Settings.xWidthFollower; x++)
        {
            for (int y = 0; y < Snake.Settings.yHeightFollower; y++)
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
        Console.WriteLine($"Follower monitor | Score: {score}", ConsoleColor.DarkGray);

        // Make a line above the game state the length of the Follower screen
        Console.WriteLine("┌" + new string('─', Snake.Settings.xWidthFollower) + "┐");

        // Write the game state
        for (int y = 0; y < state.GetLength(1); y++)
        {
            // The portal is always on the left side of the screen for follower
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("│");
            Console.ResetColor();

            for (int x = 0; x < state.GetLength(0); x++)
            {
                switch (state[x, y])
                {
                    case Field.Empty:
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write(" ");
                        Console.ResetColor();
                        break;
                    case Field.Snake:
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("x");
                        Console.ResetColor();
                        break;
                    case Field.SnakeHead:
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("X");
                        Console.ResetColor();
                        break;
                    case Field.Apple:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("O");
                        Console.ResetColor();
                        break;
                }
            }
            Console.Write("│");

            Console.WriteLine();
        }
        // Bottom line
        Console.WriteLine("└" + new string('─', Snake.Settings.xWidthFollower) + "┘");
    }

}