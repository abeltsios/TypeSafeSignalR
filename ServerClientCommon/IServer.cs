using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerClientCommon
{
    public interface IServer
    {
        bool BroadcastMessage(string message);
    }
}
