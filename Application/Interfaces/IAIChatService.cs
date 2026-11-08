using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{

    public interface IAIChatService
    {
        Task<string> GetResponseAsync(string prompt, string context = "");
        Task<float[]> GenerateEmbeddingAsync(string prompt);

    }

}
