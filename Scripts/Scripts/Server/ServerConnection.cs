using Match3.Foundation.Base.ServiceLocating;
using System;

namespace Match3.Network
{
    public interface ServerConnection : Service
    {
        void Request(HTTPRequest request, Action<string> onSuccess, Action<string> onFailure);

        bool IsTimeOut(string msg);
    }


}