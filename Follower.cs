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
    public Follower(string ip, int port)
    {
        // Create a TCP listener on the specified port
        tcpListener = new TcpListener(System.Net.IPAddress.Parse(ip), port);
        // Start the TCP listener
        tcpListener.Start();
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
        
        // Clear the screen
        Console.Clear();
        // Print the data to the console (for now) with green text
        Console.WriteLine(System.Text.Encoding.ASCII.GetString(data), ConsoleColor.Green);
        // Close the TCP client
        tcpClient.Close();
    }

}