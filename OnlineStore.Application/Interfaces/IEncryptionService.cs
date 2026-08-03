using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Application.Interfaces
{
    public interface IEncryptionService
    {
        string Hash(string value);
        bool Verify(string value, string hash);
    }
}
