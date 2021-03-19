namespace Lib 
{
    public interface IPublisher 
    {
        void Publish(string subjectName, string data);
    }
}