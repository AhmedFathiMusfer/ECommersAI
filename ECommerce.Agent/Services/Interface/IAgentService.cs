
using System.Threading.Tasks;

namespace ECommerce.Agent.Services.Interface
{
  public interface IAgentService
  {
    Task<String> AgentChat(string message);
  }
}