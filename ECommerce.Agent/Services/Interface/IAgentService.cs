
using System.Threading.Tasks;

namespace ECommerce.Agent.Services.Interface
{
  public interface IAgentService 
{
    Task ProcessAndReplyAsync(string phone, string text);
}
}