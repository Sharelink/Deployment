﻿#define DEV_DEBUG_SENDN
using CSharpServerFramework.Buffer;
using CSharpServerFramework.Client;
using CSharpServerFramework.Server;
using CSharpServerFramework.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CSharpServerFramework.Message
{
    /// <summary>
    /// 消息管理器
    /// 主要负责处理接收的信息和发送信息到客户端
    /// </summary>
    public class MessageManager
    {
        private const int TCP_PACKAGE_HEAD_SIZE = 4;
        protected CSServer Server { get; private set; }
        public IList<IGetMessageRoute> ReceiveMessageFilters { get; set; }
        protected MessageQueueBase SendMessageQueue { get; set; }
        protected Thread SendMessageThread { get; set; }
        private ManualResetEventSlim NewSendMessageAdded;
        /// <summary>
        /// 构造消息管理器
        /// </summary>
        /// <param name="Server">服务端实例</param>
        /// <param name="ReceiveMessageCreator">处理接收到的数据包的消息构造器</param>
        public MessageManager(CSServer Server)
        {
            this.Server = Server;
            ManagerInit();
        }

        private void ManagerInit()
        {
            ReceiveMessageFilters = new List<IGetMessageRoute>();
            SendMessageQueue = new MessageQueueBase();
            NewSendMessageAdded = new ManualResetEventSlim(false);
            SendMessageThread = new Thread(DoSendMessageProc);
        }

        public void AddFilter(IGetMessageRoute RouteFilter)
        {
            ReceiveMessageFilters.Add(RouteFilter);
        }

        public void Init()
        {
            try
            {
                SendMessageThread.Start();
                Server.BaseRunningThreadCount++;
            }
            catch (Exception ex)
            {
                string errMsg = "Message Manager Init Faild:" + ex.Message;
                Server.Logger.Log(errMsg);
                throw new CSServerException(errMsg);
            }
            Server.Logger.Log("Message Manager Inited");
        }
        /// <summary>
        /// 将要发送的的消息传递到发送队列，并试图唤醒消息发送线程
        /// </summary>
        /// <param name="Message">要发送的消息,可以是SendUserMessage,也可以是SendUsersMessage</param>
        public void PushSendMessage(ICSharpServerMessageData Message)
        {
            
            if (SendMessageQueue.PushMessage(Message))
            {
                NewSendMessageAdded.Set();
            }
            
        }

        private void DoSendMessageProc(object obj)
        {
            SendMessage msgBase;
            SendUserMessage msg;
            SendUsersMessage usmsg;
            while(Server.ServerState == CSServer.CSServerState.RUNNING)
            {
                if (!SendMessageQueue.IsEmpty)
                {
                    msgBase = SendMessageQueue.Dequeue() as SendMessage;
                    msg = msgBase as SendUserMessage;
                    usmsg = msgBase as SendUsersMessage;
                    if(msg!=null)
                    {
                        ClientSendMessage(msg, msg.Client);
                    }else if(usmsg!=null)
                    {
                        foreach (var user in usmsg.Users)
                        {
                            try
                            {
                                var client = (user.Session as UserSession).Client;
                                ClientSendMessage(usmsg, client);
                            }
                            catch (Exception)
                            {
                                Server.Logger.Log("Send Users Message Exception");
                            }
                            
                        }
                    }                    
                }
                else
                {                    
                    NewSendMessageAdded.Reset();
                    NewSendMessageAdded.Wait(5*1000);//每5秒检查一次
                }
                
            }
        }

        private void ClientSendMessage(SendMessage msg, CSServerClientBase client)
        {
            try
            {
                if(client == null || client.BaseTcpClient == null || client.BaseTcpClient.Client == null || !client.BaseTcpClient.Connected)
                {
                    return;
                }
                client.BaseTcpClient.Client.SendTimeout = CSServerBaseDefine.TIME_OUT;
                client.BaseTcpClient.Client.BeginSend(msg.DataBuffer, 0, msg.BufferLength, SocketFlags.None, OnSendMessageCallback, client);
            }
            catch (Exception ex)
            {
                Server.Logger.Log("Send Message Exception:" + ex.Message);
                Server.ClientManager.CheckClientIsDisconnected(client);
            }
        }

        private void OnSendMessageCallback(IAsyncResult ar)
        {
            CSServerClientBase client = ar.AsyncState as CSServerClientBase;
            SocketError se;
            try
            {
#if DEV_DEBUG_SEND
                Server.Logger.Log("Client End Send");
#endif
                client.BaseTcpClient.Client.EndSend(ar, out se);
            }
            catch (Exception ex)
            {
                Server.Logger.Log("Send Message Exception:" + ex.Message);
                Server.ClientManager.CheckClientIsDisconnected(client);
            }
        }

        /// <summary>
        /// 异步开始接收Client的发过来的数据
        /// </summary>
        /// <param name="Client">客户端</param>
        /// <returns></returns>
        public bool RegistClient(CSServerClientBase Client)
        {            
            try
            {
                if (Client.NotValidateRecvTimeOut > 0)
                {
                    Client.BaseTcpClient.Client.ReceiveTimeout = (int)Client.NotValidateRecvTimeOut;
                }
                ReceiveHead(Client, new MessageAsyncState(Client));
                return true;
            }
            catch (Exception ex)
            {
                Server.Logger.Log("Regist Client Failed:" + ex.Message);
                Server.ClientManager.CheckClientIsDisconnected(Client);
                return false;
            }            
        }

        /// <summary>
        /// 包头接收处理
        /// </summary>
        /// <param name="ar"></param>
        private void DoReveivePackageHead(IAsyncResult ar)
        {
            MessageAsyncState asyncState = ar.AsyncState as MessageAsyncState;
            SocketError se = SocketError.TimedOut;
            CSServerClientBase client = asyncState.State as CSServerClientBase;
            int len;
            try
            {
                len = client.BaseTcpClient.Client.EndReceive(ar,out se);
                if (se == SocketError.TimedOut && client.Session.IsSessionNotValidate)
                {
                    Server.ClientManager.DisconnectSession(client.Session);
                    return;
                }
                if (TCP_PACKAGE_HEAD_SIZE == len)
                {
                    int packLen = BitConverter.ToInt32(client.ReceiveBuffer.Buffer, 0);
                    ///开始接收实际数据包
                    ReceiveData(client, asyncState,packLen,out se);
                }
                else
                {
                    Server.Logger.Log("Server Receive Invalid Head");
                    Server.ClientManager.CheckClientIsDisconnected(client);
                }
            }
            catch (Exception ex)
            {
                Server.Logger.Log("Server Receive Exception:" + ex.Message);
                Server.Logger.Log("Socket Error:" + se.ToString());
                Server.ClientManager.CheckClientIsDisconnected(client);
            }
        }

        private void ReceiveHead(CSServerClientBase Client,MessageAsyncState AsyncState)
        {
            ///黏包问题解决方法：定义包头，包头代表整一个完整的包的长度
            ///客户端发送数据必须先计算要发送的数据的长度，然后将数据长度作为包头，后连接业务数据，作为完整的一个包发送
            ///接收包头，包头为长度CSServerBaseDefine.TCP_PACKAGE_HEAD_SIZE的byte数组
            ///byte[]数组转换成数字，代表接下来接收的数据长度
            Client.ReceiveBuffer = Server.BufferManager.GetFreeBuffer();
            Client.ReceiveBuffer.NextBuffer = null;
            Client.ReceiveBuffer.TailBuffer = Client.ReceiveBuffer;
            Client.BaseTcpClient.Client.BeginReceive(
                Client.ReceiveBuffer.Buffer,
                0,
                TCP_PACKAGE_HEAD_SIZE,
                SocketFlags.None,
                DoReveivePackageHead,
                AsyncState);
        }

        private void ReceiveData(CSServerClientBase Client, MessageAsyncState AsyncState,int LeftDataLen,out SocketError se)
        {            

            int recvLen;
            if(LeftDataLen > CSServerBaseDefine.BUFFER_SIZE)
            {
                recvLen = CSServerBaseDefine.BUFFER_SIZE;
                AsyncState.LeftDataLength = LeftDataLen - CSServerBaseDefine.BUFFER_SIZE;
            }
            else
            {
                recvLen = LeftDataLen;
                AsyncState.LeftDataLength = 0;
            }
            Client.BaseTcpClient.Client.BeginReceive(Client.ReceiveBuffer.TailBuffer.Buffer, 0, recvLen, SocketFlags.None, out se, DoClientLoopReceiveCallback, AsyncState);
        }

        /// <summary>
        /// 实际数据包接收处理
        /// </summary>
        /// <param name="ar"></param>
        protected void DoClientLoopReceiveCallback(IAsyncResult ar)
        {
            MessageAsyncState asyncState = ar.AsyncState as MessageAsyncState;
            CSServerClientBase client = asyncState.State as CSServerClientBase;
            SocketError se;
            int len;
            CSServerBuffer tmpBuffer = null;
            try
            {
                len = client.BaseTcpClient.Client.EndReceive(ar, out se);
#if DEV_DEBUG
                CSServerBaseDefine.ReceiveMessageTick = DateTime.UtcNow.Ticks;
                Server.Logger.Log("Receive Time:" + CSServerBaseDefine.ReceiveMessageTick);
#endif
                if(len > 0)
                {
                    tmpBuffer = client.ReceiveBuffer;
                    tmpBuffer.TailBuffer.BufferLength = len;
                    if(asyncState.LeftDataLength == 0)
                    {
                        ReceiveHead(client, asyncState);
                        var msg = TransformBufferToMessage(tmpBuffer);

                        //给ExtensionManager处理,实际上ExtensionManager会简单封装一下然后将消息传递给WorkerThread处理
                        Server.ExtensionManager.RedirectReceiveMessage(msg, client.Session);
                    }
                    else
                    {
                        ///分包接收数据
                        if (client.ReceiveBuffer.TailBuffer.NextBuffer == null)
                        {
                            client.ReceiveBuffer.TailBuffer.NextBuffer = Server.BufferManager.GetFreeBuffer();
                            client.ReceiveBuffer.TailBuffer = client.ReceiveBuffer.TailBuffer.NextBuffer;
                        }
                        ReceiveData(client, asyncState, asyncState.LeftDataLength, out se);
                    }                                     
                }
                else
                {
                    Server.Logger.Log("Client Loop Receive Exception:Nothing");
                    Server.ClientManager.CheckClientIsDisconnected(client);
                    return;
                }
            }
            catch (Exception ex)
            {
                Server.Logger.Log("Client Loop Receive Exception:" + ex.Message);
                return;
            }            
        }

        private ReceiveMessage TransformBufferToMessage(CSServerBuffer tmpBuffer)
        {
            try
            {
                ReceiveMessage msg = new ReceiveMessage();
                ///处理消息中的路由信息
                foreach (var ReceiveMessageFilter in ReceiveMessageFilters)
                {
                    var route = ReceiveMessageFilter.Filter(tmpBuffer.AllBuffer, tmpBuffer.BufferTotalLength);
                    if (route == null)
                    {
                        continue;
                    }
                    msg.CommandId = route.CmdId;
                    msg.ExtensionName = route.ExtName;
                    msg.CommandName = route.CmdName;
                    msg.ReceiveDataBuffer = tmpBuffer;
                    return msg;
                }
                throw new Exception();
            }
            catch (Exception ex)
            {
                Server.Logger.Log("Transform Buffer Exception" + ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// 停止消息服务器线程
        /// </summary>
        public void Stop()
        {
            while (!SendMessageQueue.IsEmpty) ;
            NewSendMessageAdded.Set();
        }
    }
}
