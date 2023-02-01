// Represents the leader client

// The leader client is the client that sends commands to the followers
// To create a leader client, an IP address and a port number must be specified
// The leader client will then open a TCP client, where it will send data to the follower
// Initial action (development): The leader client will send a string to the follower
// and then close the connection

using System.Net.Sockets;

public class Leader
{
    
        private string ip;
        private int port;
    
        // Constructor
        // When a Leader has finished its construction, 
        // it has established connection with the Follower.
        public Leader(System.Net.IPAddress ip, int port)
        {
            this.ip = ip.ToString();
            this.port = port;
            Logger.Debug($"Leader created with IP {this.ip} and port {this.port}");
        }
    
        // Sends a command to the follower
        // This method will block until the follower has received the command
        // and should be run continously sending commands
        public void sendCommand(string command)
        {
            TcpClient tcpClient = new TcpClient(ip, port);
            // Get the stream from the TCP client
            NetworkStream stream = tcpClient.GetStream();
            // Create a buffer to store the data
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(command);
            // Write the data to the stream
            stream.Write(data, 0, data.Length);
            // Close the TCP client
            tcpClient.Close();
        }
}