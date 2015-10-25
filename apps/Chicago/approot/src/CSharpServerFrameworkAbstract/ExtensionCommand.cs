using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CSharpServerFramework.Extension
{
    public class ExtensionCommand
    {
        public bool isOrder { get; private set; }
        public int CommandId { get; private set; }
        public string CommandName { get; private set; }
        public MethodInfo CommandMethod { get; private set; }
        public bool IsAsyncInvoke { get; private set; }
        public bool IsAcceptRawDataCommand { get; set; }

        public ExtensionCommand()
        {
            CommandId = -1;
        }

        public ExtensionCommand(MethodInfo method,CommandInfoAttribute attr)
        {
            CommandMethod = method;
            isOrder = attr.IsOrderCommand;
            CommandId = attr.CommandId;
            CommandName = attr.CommandName;
            IsAsyncInvoke = attr.IsAsyncInvoke;
            IsAcceptRawDataCommand = attr.IsAcceptRawDataCommand;
        }
    }
}
