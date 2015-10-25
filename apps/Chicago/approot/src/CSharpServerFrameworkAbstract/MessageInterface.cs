using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpServerFramework.Message
{

    public interface ICSharpServerMessage
    {
        string ExtensionName { get; set; }
        int CommandId { get; set; }
        string CommandName { get; set; }
    }

    public interface ICSharpServerMessageData
    {
        byte[] DataBuffer { get; }
        int BufferLength { get; }
    }

    /// <summary>
    /// 消息基类
    /// 每一个消息都带有一个ExtensionName和CommandId，代表处理消息的Extension和处理的方法
    /// 消息的UserInfo代表了消息来源的User或消息接收的用户
    /// </summary>
    public abstract class MessageBase : ICSharpServerMessage
    {
        public MessageBase()
        {
            CommandId = -1;
        }
        public string ExtensionName { get; set; }
        public int CommandId { get; set; }
        public string CommandName { get; set; }
    }

    /// <summary>
    /// 发送的信息
    /// </summary>
    public class SendMessage : ICSharpServerMessageData
    {
        public byte[] DataBuffer { get; set; }
        public int BufferLength { get; set; }
    }

    public interface IDeserializeMessage
    {
        /// <summary>
        /// 根据CommandId处理客户端发送的数据，返回数据实体，数据实体最终会传递给具体的Extension的对应CommandId的方法
        /// </summary>
        /// <param name="CommandId">处理消息的函数Id</param>
        /// <param name="ReceivedData">数据流</param>
        /// <param name="Length">数据流长度</param>
        /// <returns></returns>
        object DeserializeMessage(int CommandId, byte[] ReceivedData, int Length);
    }

    /// <summary>
    /// 使用Router
    /// </summary>
    public interface IUseMessageRouter
    {
        void UseMessageRoute(IGetMessageRoute Router);
    }

    public interface IGetMessageRoute
    {
        MessageRoute Filter(byte[] ReceivedData, int Length);
    }
    /// <summary>
    /// 服务端接收到的消息的路由信息
    /// </summary>
    public class MessageRoute
    {
        public MessageRoute()
        {
            CmdId = -1;
        }
        /// <summary>
        /// 获取或设置处理消息的Extension名
        /// </summary>
        public string ExtName { get; set; }

        /// <summary>
        /// 获取或设置处理消息的Extension的命令Id
        /// </summary>
        public int CmdId { get; set; }

        /// <summary>
        /// 获取或设置处理消息的Extension的命令名，与CmdId类似
        /// </summary>
        public string CmdName { get; set; }
    }

}
