using System;
using System.Collections.Generic;
using System.Text;

namespace MS.Entities.Core
{
    /// <summary>
    /// //没有Id主键的实体继承这个
    /// </summary>
    public interface IEntity
    {
    }

    /// <summary>
    /// 有Id主键的实体继承这个
    /// </summary>
    public class BaseEntity : IEntity
    {
        public long Id { get; set; }
        public StatusCode StatusCode { get; set; }
        public long? Creator { get; set; }
        public DateTime? CreateTime { get; set; }
        public long? Modifier { get; set; }
        public DateTime? ModifyTime { get; set; }
    }
}
