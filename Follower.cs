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
        Byte[] data = new Byte[512];
        // Read the data from the stream
        stream.Read(data, 0, data.Length);
        Logger.Debug($"Received data: {System.Text.Encoding.ASCII.GetString(data)}");

        // Check that the data ends with a semicolon and remove it
        string receivedCommand = System.Text.Encoding.ASCII.GetString(data);
        if (receivedCommand.EndsWith(";")) {
            receivedCommand = receivedCommand.Substring(0, receivedCommand.Length - 1);
        } else {
            Logger.Error($"Received data does not end with a semicolon: {receivedCommand}");
        }


        // Parse the command
        FollowerCommand command = parseCommand(receivedCommand);
        // Execute the command
        command.Execute(); 
        

        // Close the TCP client
        tcpClient.Close();
    }

    private FollowerCommand parseCommand(string command)
    {
        Logger.Debug($"Parsing command: {command}");
        // Parse the command by first finding the command type (before ,)
        string commandType = command.Substring(0, command.IndexOf(','));

        // Check what command type it is
        if (commandType == "ShowBoard") {
            // Create a new ShowBoardCommand
            return ShowBoard.Deserialize(command);
        }
        return null;
    }

}