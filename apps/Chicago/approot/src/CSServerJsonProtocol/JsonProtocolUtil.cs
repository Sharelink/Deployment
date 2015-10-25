using CSharpServerFramework.Message;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSServerJsonProtocol
{
    public class JsonProtocolUtil
    {
        public static byte[] SerializeMessage(string ExtensionName, int CommandId, object Message)
        {
            MessageRoute route = new MessageRoute()
            {
                CmdId = CommandId,
                ExtName = ExtensionName
            };
            return SerializeMessage(route, Message);
        }

        public static byte[] SerializeMessage(string ExtensionName, string CommandName, object Message)
        {
            MessageRoute route = new MessageRoute()
            {
                CmdId = -1,
                ExtName = ExtensionName,
                CmdName = CommandName
            };
            return SerializeMessage(route, Message);
        }

        public static byte[] SerializeMessage(MessageRoute Route, object Message)
        {
            var routeJson = JsonConvert.SerializeObject(Route);
            var routeBytes = Encoding.UTF8.GetBytes(routeJson);
            var routeLengthBytes = BitConverter.GetBytes(routeBytes.Length);
            var msgJson = JsonConvert.SerializeObject(Message);
            var msgBytes = Encoding.UTF8.GetBytes(msgJson);

            var result = new byte[routeLengthBytes.Length + routeBytes.Length + msgBytes.Length];
            Array.Copy(routeLengthBytes, result, routeLengthBytes.Length);
            Array.Copy(routeBytes, 0, result, routeLengthBytes.Length, routeBytes.Length);
            Array.Copy(msgBytes, 0, result, routeLengthBytes.Length + routeBytes.Length, msgBytes.Length);
            return result;
        }

        public static object DeserializeMessage(int CommandId, byte[] ReceivedData, int Length)
        {
            int routeJsonObjectLength = BitConverter.ToInt32(ReceivedData, 0);
            var routeInfoLength = 4 + routeJsonObjectLength;
            string receiveJsonObjectString = Encoding.UTF8.GetString(ReceivedData, routeInfoLength, Length - routeInfoLength);
            return JsonConvert.DeserializeObject(receiveJsonObjectString);
        }

        public static MessageRoute DeserializeRoute(byte[] ReceivedData, int Length)
        {
            int routeJsonObjectLength = BitConverter.ToInt32(ReceivedData, 0);
            string routeJsonObjectString = Encoding.UTF8.GetString(ReceivedData, 4, routeJsonObjectLength);
            MessageRoute route = JsonConvert.DeserializeObject<MessageRoute>(routeJsonObjectString);
            return route;
        }
    }

}
