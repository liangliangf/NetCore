using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MS.WebCore.Core
{
    /// <summary>
    ///  表示操作数据库类型
    /// </summary>
    public enum ExecuteType
    {
        [Description("读取资源")]
        Retrieve,
        [Description("创建资源")]
        Create,
        [Description("更新资源")]
        Update,
        [Description("删除资源")]
        Delete
    }
}
