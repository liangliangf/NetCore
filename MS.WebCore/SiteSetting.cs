using System;
using System.Collections.Generic;
using System.Text;

namespace MS.WebCore
{
    public class SiteSetting
    {
        /// <summary>
        /// 雪花算法 workerid
        /// </summary>
        public long WorkerId { get; set; }

        /// <summary>
        /// 雪花算法 datacenterid
        /// </summary>
        public long DatacenterId { get; set; }

        /// <summary>
        /// 用户登录失败的次数限制
        /// </summary>
        public int LoginFailedCountLimits { get; set; }

        /// <summary>
        /// 用户锁定后，多久可以重新登录（分钟） 
        /// </summary>
        public int LoginLockTimeout { get; set; }


        /// <summary>
        /// 默认语言
        /// </summary>
        public string DefaultLanguage { get; set; }
    }
}
