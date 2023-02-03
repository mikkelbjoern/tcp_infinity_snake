// This code represents the game.
// It exposes the following methods:
// - An `Action` Enum that represents the possible actions that can be taken
// - View() which shows how the game should be rendered
// - Update(`Action`) which updates the game state
// Internally the game implements a Step() method which
// is called to make the game progress one step.
// Control of the game in this manner makes it possible 
// to correct for network latency and other issues in the future in 
// the code around the Game class.

using System.Text;
using static Logger;

public class Game
{
    // Represents the possible actions that can be taken
    public enum Action { Up, Down, Left, Right, None };

    private enum Direction { North, East, South, West };

    public enum Field { Snake, SnakeHead, Apple, Empty };

    // The game state
    public Field[,] state;

    // The player's position
    private int playerX;
    private int playerY;

    public int score = 0;

    private Direction direction = Direction.East;

    // Whether or not the game is over
    public bool gameOver = false;


    // Represents the tail of the snake (not including the head)
    private List<Tuple<int, int>> tail = new List<Tuple<int, int>>();

    // The width and height of the game state
    private int xWidth = Snake.Settings.xWidth;
    private int yHeight = Snake.Settings.yHeight;

    // The constructor
    public Game()
    {
        // Create a new game state
        state = new Field[xWidth, yHeight];
        // Set the player's position to the center of the game state
        playerX = xWidth / 2;
        playerY = yHeight / 2;

        // Set all positions in the game state to empty
        for (int x = 0; x < xWidth; x++)
        {
            for (int y = 0; y < yHeight; y++)
            {
                state[x, y] = Field.Empty;
            }
        }

        // Set the player's position in the game state to 1
        state[playerX, playerY] = Field.SnakeHead;

        // Generate three apples
        generateApple();
        generateApple();
        generateApple();
    }

    // Generates an apple in a random position
    private void generateApple()
    {
        Logger.Debug("Generating apple");
        // Generate a random position
        int x = new Random().Next(0, xWidth);
        int y = new Random().Next(0, yHeight);

        // If the position is not empty, generate a new position
        while (state[x, y] != Field.Empty)
        {
            x = new Random().Next(0, xWidth);
            y = new Random().Next(0, yHeight);
        }

        // Set the position to an apple
        state[x, y] = Field.Apple;
    }

    // Updates the game state based on user input
    public void Update(Action action)
    {
        Logger.Debug($"Updating game with action {action}");
        // Update the direction based on the action
        switch (action)
        {
            case Action.Up:
                direction = Direction.North;
                break;
            case Action.Down:
                direction = Direction.South;
                break;
            case Action.Left:
                direction = Direction.West;
                break;
            case Action.Right:
                direction = Direction.East;
                break;
        }
    }

    public enum PortalSide { Left, Right };

    // Given a game state, a score and a portal side, this method
    // prints the game state to the console
    public static Task View(Field[,] board, int score, PortalSide side)
    {
        // Clear the screen
        Console.Clear();

        string sideString = side == PortalSide.Left ? "Left" : "Right";

        // Write on the first line that this is the second monitor
        Console.WriteLine($"{sideString} monitor | Score: {score}", ConsoleColor.DarkGray);

        // Make a line above the game state the length of the Follower screen
        Console.WriteLine("┌" + new string('─', Snake.Settings.xWidthFollower) + "┐");

        // Write the game state
        for (int y = 0; y < board.GetLength(1); y++)
        {
            // The portal is always on the left side of the screen for follower
            if (side == PortalSide.Left)
            {
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.Write("│");
                Console.ResetColor();
            } else {
                Console.Write("│");
            }

            for (int x = 0; x < board.GetLength(0); x++)
            {
                switch (board[x, y])
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
            // The portal is always on the left side of the screen for follower
            if (side == PortalSide.Right)
            {
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.Write("│");
                Console.ResetColor();
            } else {
                Console.Write("│");
            }

            Console.WriteLine();
        }
        // Bottom line
        Console.WriteLine("└" + new string('─', Snake.Settings.xWidthFollower) + "┘");

        return Task.CompletedTask;
    }

    // Steps the game forward one step
    public void Step()
    {
        // Add the player's position to the tail
        tail.Add(new Tuple<int, int>(playerX, playerY));

        // Move the player
        switch (direction)
        {
            case Direction.North:
                playerY--;
                break;
            case Direction.South:
                playerY++;
                break;
            case Direction.West:
                playerX--;
                break;
            case Direction.East:
                playerX++;
                break;
        }

        // If the player is out of bounds, she is dead
        if (playerX < 0 || playerX >= xWidth || playerY < 0 || playerY >= yHeight)
        {
            GameOver();
            return;
        }

        // If the player is on an apple, generate a new apple
        if (state[playerX, playerY] == Field.Apple)
        {
            generateApple();
            score++;
        }
        // If the player is on the snake, she is dead
        else if (state[playerX, playerY] == Field.Snake)
        {
            GameOver();
            return;
        }

        // Clear the game state
        for (int y = 0; y < yHeight; y++)
        {
            for (int x = 0; x < xWidth; x++)
            {
                // Don't clear the apple
                if (state[x, y] == Field.Apple)
                {
                    continue;
                }
                state[x, y] = Field.Empty;
            }
        }

        // Set the player's position in the game state
        state[playerX, playerY] = Field.SnakeHead;

        // If the tail is longer than the score, remove the first element
        if (tail.Count > score)
        {
            tail.RemoveAt(0);
        }

        // Set the tail's positions in the game state
        foreach (Tuple<int, int> pos in tail)
        {
            state[pos.Item1, pos.Item2] = Field.Snake;
        }

    }

    // Called when the game is over
    private void GameOver()
    {
        gameOver = true;
    }

}
