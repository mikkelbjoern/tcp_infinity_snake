// Represents a command that can be sent to a Follower.
// The command needs to be be able to be serialized into a string (using the Serialize method)
// This string should avoid using newlines, as these act weirdly on Windows.
//
// It should also be able to be deserialized from a string (using the Deserialize method)
// And lastly it should be able to be executed on a Follower (using the Execute method)

public interface FollowerCommand
{
    // Serialize the command to a string
    public abstract string Serialize();

    // Deserialize the command from a string
    public static abstract FollowerCommand Deserialize(string data);

    // Execute the command on a Follower
    public abstract void Execute();
}