using MS.Entities.Core;
using System;

namespace MS.Entities
{
    public class TestTable:BaseEntity
    {
        /// <summary>
        /// 测试名称
        /// </summary>
        public string TestName { get; set; }

        /// <summary>
        /// 是否管理员
        /// </summary>
        public bool IsAdmin { get; set; }
    }
}
