using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpServerFramework;
using CSharpServerFramework.Message;

namespace CSServerJsonProtocol
{

    public static class JsonProtocolExension
    {

        /// <summary>
        /// 发送Json格式数据
        /// </summary>
        /// <param name="Users">群发的用户</param>
        /// <param name="Object">发送的数据，发送数据时会将Object转换成Json格式的字符串发送</param>
        /// <param name="CommandId">发送的命令Id，命令名必须在本Extension里有对应调用该函数的Command的CommandId属性</param>
        public static void SendJsonResponseToUsers(this ICSharpServerExtension Extension, IEnumerable<ICSharpServerUser> Users, object Object, string ExtensionName, int CommandId)
        {
            var msgBytes = JsonProtocolUtil.SerializeMessage(ExtensionName, CommandId, Object);
            var msg = new SendMessage()
            {
                BufferLength = msgBytes.Length,
                DataBuffer = msgBytes
            };
            Extension.SendResponseToUsers(Users, msg);
        }

        /// <summary>
        /// 发送Json格式数据
        /// </summary>
        /// <param name="Users">群发的用户</param>
        /// <param name="Object">发送的数据，发送数据时会将Object转换成Json格式的字符串发送</param>
        /// <param name="Command">发送的命令名，命令名必须在本Extension里有对应调用该函数的Command的Name属性</param>
        public static void SendJsonResponseToUsers(this ICSharpServerExtension Extension, IEnumerable<ICSharpServerUser> Users, object Object, string ExtensionName, string Command)
        {
            var msgBytes = JsonProtocolUtil.SerializeMessage(ExtensionName, Command, Object);
            var msg = new SendMessage()
            {
                BufferLength = msgBytes.Length,
                DataBuffer = msgBytes
            };
            Extension.SendResponseToUsers(Users, msg);
        }

        /// <summary>
        /// 发送Json格式数据
        /// </summary>
        /// <param name="Session">当前会话</param>
        /// <param name="Object">发送的数据，发送数据时会将Object转换成Json格式的字符串发送</param>
        /// <param name="CommandId">发送的命令Id，命令名必须在本Extension里有对应调用该函数的Command的CommandId属性</param>
        public static void SendJsonResponse(this ICSharpServerExtension Extension,ICSharpServerSession Session, object Object,string ExtensionName, int CommandId)
        {
            var msgBytes = JsonProtocolUtil.SerializeMessage(ExtensionName, CommandId, Object);
            var msg = new SendMessage()
            {
                BufferLength = msgBytes.Length,
                DataBuffer = msgBytes
            };
            Extension.SendResponse(Session, msg);
        }

        /// <summary>
        /// 发送Json格式数据
        /// </summary>
        /// <param name="Session">当前会话</param>
        /// <param name="Object">发送的数据，发送数据时会将Object转换成Json格式的字符串发送</param>
        /// <param name="Command">发送的命令名，命令名必须在本Extension里有对应调用该函数的Command的Name属性</param>
        public static void SendJsonResponse(this ICSharpServerExtension Extension, ICSharpServerSession Session, object Object, string ExtensionName, string Command)
        {
            var msgBytes = JsonProtocolUtil.SerializeMessage(ExtensionName, Command, Object);
            var msg = new SendMessage()
            {
                BufferLength = msgBytes.Length,
                DataBuffer = msgBytes
            };
            Extension.SendResponse(Session, msg);
        }

    }

}
