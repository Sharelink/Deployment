using CSharpServerFramework.Extension;
using CSharpServerFramework.Message;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpServerFramework;

namespace CSServerJsonProtocol
{
    public abstract class JsonExtensionBase:ExtensionBaseEx
    {
        public JsonExtensionBase():base(JsonMessageDeserializer.Instance)
        {

        }
        
    }

}
