using System.Threading.Tasks;
namespace StudentSystem.Infrastructure.Messaging;

public interface IMessageHandler
{
    void Start(IMessageHandlerCallback callback);
    void Stop();
}