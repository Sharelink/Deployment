using CSharpServerFramework.Client;
using CSharpServerFramework.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpServerFramework.Extension.ServerBaseExtensions
{
    /// <summary>
    /// 登陆认证处理Extension
    /// 继承此类，并用IExtensionLoader加载进CSServer可以自定义处理登陆请求
    /// 如果CSServer加载此类的子类的实例，CSServer将消息中的UserInfo的IsNotValidated属性(根据UserId是否为-1判断)为true的消息交给子类来处理
    /// 如果CSServer没有加载任何此类的子类，CSServer默认不做认证处理，所有的消息都正常传递给相应的Extension处理
    /// 子类的工作是和其他Extension一样处理登陆信息，如果登陆成功需要在处理方法里将User的UserId重新设置成自定义的Id(规定了-1表示用户没登陆)
    /// 子类需要指定类的ExtensionInfo特征属性
    /// </summary>
    public class ValidateExtensionBase : ExtensionBase
    {
        public ValidateExtensionBase(ExtensionBaseEx Extend):base(Extend)
        {

        }
    }
}
