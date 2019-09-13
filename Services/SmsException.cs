[System.Serializable]
public class SmsException : System.Exception
{
    public SmsException() { }
    public SmsException(string message) : base(message) { }
    public SmsException(string message, System.Exception inner) : base(message, inner) { }
    protected SmsException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}