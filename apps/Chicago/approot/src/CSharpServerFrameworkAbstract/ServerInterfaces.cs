using CSharpServerFramework.Log;
using CSharpServerFramework.Message;
using System;
using System.Net;

namespace CSharpServerFramework
{
    public class CSServerEventArgs : EventArgs
    {
        public object State { get; set; }

    }

    /// <summary>
    /// 服务器的异常
    /// </summary>
    [Serializable]
    public class CSServerException : Exception
    {
        public CSServerException() { }
        public CSServerException(string message) : base(message) { }
        public CSServerException(string message, Exception inner) : base(message, inner) { }
        protected CSServerException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        { }
    }

    public interface ICSServerBuilder: IUseExtension, ILoggerBuilder, IUseMessageRouter
    {
        void UseServerConfig(IGetServerConfig ServerConfig);
        void UseNetConfig(IGetNetConfig NetConfig);
    }

    public interface IGetServerConfig
    {
        /// <summary>
        /// Buffer的长度，单位byte
        /// </summary>
        int GetBufferSize();

        /// <summary>
        /// 工作线程数量
        /// </summary>
        int GetWorkerThreadCount();

        /// <summary>
        /// 网络通信超时时间
        /// </summary>
        /// <returns>毫秒数</returns>
        int GetNetTimeOut();

        /// <summary>
        /// 缓存初始化数量
        /// </summary>
        /// <returns></returns>
        uint GetBufferInitCount();

        /// <summary>
        /// 缓存每次增加的数量
        /// </summary>
        /// <returns></returns>
        uint GetBufferAddPerTimeCount();

        /// <summary>
        /// 认证超时
        /// </summary>
        /// <returns></returns>
        uint GetValidateTimeout();
    }

    public interface IGetNetConfig
    {
        /// <summary>
        /// 服务器监听的接口
        /// </summary>
        /// <returns></returns>
        int GetListenPort();
        /// <summary>
        /// 服务器最大连接数
        /// </summary>
        /// <returns></returns>
        int GetMaxListenConnection();
        /// <summary>
        /// 服务器监听的IP
        /// </summary>
        /// <returns></returns>
        IPAddress GetServerBindIP();
    }
}
