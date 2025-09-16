using System.Collections.Generic;
using System.Threading.Tasks;
using Cui.Desktop.Models;

namespace Cui.Desktop.Services;

public interface IConversationDataSource
{
    Task<IEnumerable<ChatMessage>> GetConversationAsync();
}
