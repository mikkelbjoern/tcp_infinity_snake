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

public class Game
{
    // Represents the possible actions that can be taken
    public enum Action { Up, Down, Left, Right, None };

    private enum Direction { North, East, South, West };

    private enum Field { Snake, SnakeHead, Apple, Empty };

    // The game state
    private Field[,] state;

    // The player's position
    private int playerX;
    private int playerY;

    private int score = 0;

    private Direction direction = Direction.East;

    // Whether or not the game is over
    public bool gameOver = false;


    // Represents the tail of the snake (not including the head)
    private List<Tuple<int, int>> tail = new List<Tuple<int, int>>();

    // The width and height of the game state
    private int xWidth = 20;
    private int yHeight = 10;

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

        // Generate an apple
        generateApple();
    }

    // Generates an apple in a random position
    private void generateApple()
    {
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

    // Returns the game state as a string
    public string View()
    {
        // If the game is over, write Game over in ascii art
        if (gameOver)
        {
            return "Game Over!";
        }

        // Create a new string builder
        StringBuilder sb = new StringBuilder();

        // Loop through the game state
        for (int y = 0; y < yHeight; y++)
        {
            for (int x = 0; x < xWidth; x++)
            {
                // If the position is empty, add a space
                if (state[x, y] == Field.Empty)
                {
                    sb.Append(" ");
                }
                // If the position is the player, add an X
                else if (state[x, y] == Field.SnakeHead)
                {
                    sb.Append("X");
                }
                // If the position is the player, add an X
                else if (state[x, y] == Field.Snake)
                {
                    sb.Append("x");
                }
                // If the position is an apple, add an O
                else if (state[x, y] == Field.Apple)
                {
                    sb.Append("O");
                }
            }
            // Add a new line
            sb.Append(Environment.NewLine);
        }

        // Return the string
        return sb.ToString();
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
