using CSharpServerFramework.Message;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSServerJsonProtocol
{
    public class JsonRouteFilter : IGetMessageRoute
    {
        public MessageRoute Filter(byte[] ReceivedData, int Length)
        {
            return JsonProtocolUtil.DeserializeRoute(ReceivedData, Length);
        }
    }

    public class JsonMessageDeserializer : IDeserializeMessage
    {
        public object DeserializeMessage(int CommandId, byte[] ReceivedData, int Length)
        {
            return JsonProtocolUtil.DeserializeMessage(CommandId, ReceivedData, Length);
        }

        private static JsonMessageDeserializer _instance = new JsonMessageDeserializer();
        public static JsonMessageDeserializer Instance
        {
            get
            {
                return _instance;
            }
        }
    }
}
