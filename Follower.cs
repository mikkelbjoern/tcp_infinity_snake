// Represents the Follower in the network.
// The Follower shows exactly the data that the Leader sends to it.
// To create a follower, an IP address and a port number must be specified.
// The follower will then open a TCP server, where it will listen for data from the Leader.
// Initial action (development): The follower will then print the data that it receives from the Leader.

using System.Net.Sockets;

public class Follower
{
    private TcpListener tcpListener;

    // Constructor
    // When a Follower has finished its construction, 
    // it has established connection with the Leader.
    public Follower(System.Net.IPAddress ip, int port)
    {
        // Create a TCP listener on the specified port
        tcpListener = new TcpListener(ip, port);

        // Make sure that the TCP listener isn't sending RST ACK
        // when client tries to connect
        tcpListener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

        // Start the TCP listener
        tcpListener.Start();
        Logger.Debug($"Follower created with IP {ip} and port {port}");
    }

    // Accepts a command from the Leader
    // This method will block until the Leader sends a command
    // and should be run continously waiting for commands
    public void waitForCommand()
    {
        // Accept the client
        TcpClient tcpClient = tcpListener.AcceptTcpClient();
        // Get the stream from the TCP client
        NetworkStream stream = tcpClient.GetStream();
        // Create a buffer to store the data
        Byte[] data = new Byte[256];
        // Read the data from the stream
        stream.Read(data, 0, data.Length);
        Logger.Debug($"Received data: {System.Text.Encoding.ASCII.GetString(data)}");
        
        printData(System.Text.Encoding.ASCII.GetString(data));
        // Close the TCP client
        tcpClient.Close();
    }

    private void printData(string data)
    {
        // Clear the screen
        Console.Clear();

        // Write on the first line that this is the second monitor
        Console.WriteLine("Second monitor", ConsoleColor.DarkGray);

        // Split the data into lines
        string[] lines = data.Split(Environment.NewLine);

        // Print each line
        for (int j = 0; j < lines.Length; j++)
        {
            // Print a line at the top of the screen
            if (j == 0)
            {
                Console.WriteLine("┌" + new string('─', lines[j].Length) + "┐");
            }

            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.Write("│");
            Console.ResetColor();


            // Print each character in the line
            for (int k = 0; k < lines[j].Length; k++)
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.Write(lines[j][k]);
                Console.ResetColor();
            }

            Console.Write("│");
            Console.WriteLine();
        }

        // Print a line at the bottom of the screen
        Console.WriteLine("└" + new string('─', lines[1].Length) + "┘");

    }

}