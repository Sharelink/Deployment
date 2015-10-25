#define DEV_DEBUGN
using CSharpServerFramework.Client;
using CSharpServerFramework.Extension.ServerBaseExtensions;
using CSharpServerFramework.Message;
using CSharpServerFramework.Server;
using CSharpServerFramework.WorkThread;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CSharpServerFramework.Extension
{

    /// <summary>
    /// Extension管理器
    /// 负责加载Extension，重定向消息
    /// </summary>
    public class ExtensionManager
    {
        internal CSServer Server { get; private set; }
        protected IDictionary<string, ICSharpServerExtension> ExtensionMap;
        protected IDictionary<string, ValidateExtensionBase>  ValidateExtensions { get; set; }
        public bool IsValidateExtensionExists { get { return ValidateExtensions != null && ValidateExtensions.Count > 0; } }
        protected UnknowCommandExtensionBase UnknowCommandExtension { get; set; }
        public bool IsUnknowCommandExtensionExists { get { return UnknowCommandExtension != null; } }
        public ExtensionManager(CSServer Server)
        {
            this.Server = Server;
            ManagerInit();
        }

        protected void ManagerInit()
        {
            ExtensionMap = new Dictionary<string, ICSharpServerExtension>();
        }

        public static IList<ICSharpServerExtension> CreateExtensionsFromFile(string ExtensionFileName)
        {
            List<ICSharpServerExtension> result = new List<ICSharpServerExtension>();
            var assembly = Assembly.LoadFile(ExtensionFileName);
            var definedTypes = assembly.GetTypes();
            foreach (var type in definedTypes)
            {
                if (type.IsSubclassOf(typeof(ICSharpServerExtension)))
                {
                    ICSharpServerExtension extension = assembly.CreateInstance(type.FullName) as ICSharpServerExtension;
                    result.Add(extension);
                }
            }
            return result;
        }

        public static ICSharpServerExtension CreateExtensionFromFile(string ExtensionFileName,string ExtensionClassName,object[] ConstructorArgs = null)
        {
            ICSharpServerExtension extension = null;
            var assembly = Assembly.LoadFile(ExtensionFileName);
            
            if(ConstructorArgs == null)
            {
                extension = assembly.CreateInstance(ExtensionClassName) as ICSharpServerExtension;
            }
            else
            {
                extension = assembly.CreateInstance(ExtensionClassName, false, BindingFlags.CreateInstance, null, ConstructorArgs, null, null) as ICSharpServerExtension;
            }
            return extension;
        }

        public void RegistExtension(ICSharpServerExtension Extension)
        {
            if(ExtensionMap.ContainsKey(Extension.ExtensionName))
            {
                throw new ExtensionException("Extension Name Is Exists:" + Extension.ExtensionName);
            }
            else
            {
                ExtensionBase extension = null;
                if (Extension is ExtensionBaseEx)
                {
                    var attr = Attribute.GetCustomAttribute(Extension.GetType(), typeof(ValidateExtensionAttribute));
                    if (attr != null)
                    {
                        extension = new ValidateExtensionBase(Extension as ExtensionBaseEx);
                    }
                    else if (Extension is HandleUnknowCommand)
                    {
                        extension = new UnknowCommandExtensionBase(Extension as HandleUnknowCommand);
                    }
                    else
                    {
                        extension = new ExtensionBase(Extension as ExtensionBaseEx);
                    }
                }
                else if(Extension is ExtensionBase)
                {
                    extension = Extension as ExtensionBase;
                }
                else
                {
                    throw new ExtensionException("Error Extension:" + Extension.ExtensionName);
                }
                ExtensionMap.Add(extension.ExtensionName, extension);
                extension.ExtensionManagerInstance = this;
                if(extension is ValidateExtensionBase)
                {
                    if (ValidateExtensions == null)
                    {
                        ValidateExtensions = new Dictionary<string, ValidateExtensionBase>();
                    }
                    ValidateExtensions.Add(Extension.ExtensionName, Extension as ValidateExtensionBase);
                }
                else if(extension is UnknowCommandExtensionBase)
                {
                    UnknowCommandExtension = Extension as UnknowCommandExtensionBase;
                }
                extension.Init();
            }
        }

        public void RemoveExtension(string ExtensionName)
        {
            ExtensionMap.Remove(ExtensionName);
        }

        public void Log(string LogMessage)
        {
            Server.Logger.Log(LogMessage);
        }

        public void RedirectReceiveMessage(ReceiveMessage Message,UserSession Session)
        {
            try
            {
                ExtensionBase extension;
                WorkerTask task = null;
                if (IsValidateExtensionExists && Session.IsSessionNotValidate)
                {
                    try
                    {
                        extension = ValidateExtensions[Message.ExtensionName];
                        task = new WorkerTask(extension, Message, Session);
                    }
                    catch (Exception)
                    {
                        throw new ExtensionException("Not Validate Client");
                    }
                    
                }
                else
                {
                    try
                    {
                        extension = (ExtensionBase)ExtensionMap[Message.ExtensionName];
                        task = new WorkerTask(extension, Message, Session);
                    }
                    catch (Exception)
                    {
                        if (IsUnknowCommandExtensionExists)
                        {
                            extension = UnknowCommandExtension;
                            task = new WorkerTask(extension, Message, Session);
                        }
                        else
                        {
                            throw new ExtensionException("Unknow Extension:" + Message.ExtensionName);
                        }
                    }
                }
#if DEV_DEBUG
                Server.Logger.Log("RedirectReceiveMessage Interval:" + (DateTime.UtcNow.Ticks - CSServerBaseDefine.ReceiveMessageTick));
#endif
                
                Server.WorkThreadManager.AddNewWork(task);
            }
            catch (Exception ex)
            {
                Log("ExtensionManager Exception");
                throw new ExtensionException(ex.Message);
            }                        
        }

        public void RedirectSendMessage(ICSharpServerMessageData Message)
        {
            Server.MessageManager.PushSendMessage(Message);
        }

        public void Init()
        {
            Server.Logger.Log("Extension Manager Inited");
        }
    }
}
