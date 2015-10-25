using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSharpServerFramework
{ 
    /// <summary>
    /// 自定义用户实体
    /// </summary>
    public interface ICSharpServerUser
    {
        /// <summary>
        /// 返回用户是否通过服务器验证
        /// </summary>
        bool IsUserValidate { get; }

        /// <summary>
        /// 注册后Session会被框架赋值
        /// 实现此属性即可
        /// </summary>
        ICSharpServerSession Session { get; set; }
    }

    /// <summary>
    /// 连接CSharpServer的会话状态接口
    /// 可以自定User信息
    /// </summary>
    public interface ICSharpServerSession
    {

        /// <summary>
        /// 注册后的的User实体
        /// </summary>
        ICSharpServerUser User { get; }

        /// <summary>
        /// 注册当前会话的用户实体
        /// 一般在验证拓展里调用
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns>
        bool RegistUser(ICSharpServerUser User);
    }
}
