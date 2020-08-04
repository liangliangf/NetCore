﻿using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace MS.Entities.Core
{
    public enum StatusCode
    {
        [Description("已删除")]
        delete=-1,//软删除，已删除的无法恢复，无法看见，暂未使用
        [Description("生效")]
        Enable =0,
        [Description("失效")]
        Disable = 1//失效的还可以改为生效
    }
}
