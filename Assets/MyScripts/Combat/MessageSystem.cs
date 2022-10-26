
public enum MessageType
{
    DAMAGED
    //Add your user defined message type after
}

public interface IMessageReceiver
{
    void OnReceiveMessage(MessageType type, object data);
}