using System;
using System.Collections.Generic;
using System.Text;

namespace MS.WebCore.Core
{
    public class ExecuteResult
    {
        public virtual ExecuteResult Set(bool isSucceed, string message)
        {
            IsSucceed = isSucceed;
            Message = message;
            return this;
        }

        /// <summary>
        /// 设定错误信息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public virtual ExecuteResult SetFailMessage(string message)
        {
            return Set(false, message);
        }
        public virtual ExecuteResult SetFail()
        {
            return Set(false, string.Empty);
        }

        /// <summary>
        /// 如果是空的，没有信息，默认IsSucceed=true
        /// </summary>
        public ExecuteResult()
        {
        }

        /// <summary>
        /// 如果是给字符串，表示有错误信息，默认IsSucceed=false
        /// </summary>
        /// <param name="message"></param>
        public ExecuteResult(string message)
        {
            Set(false, message);
        }
        public ExecuteResult(bool isSucceed, string message)
        {
            Set(isSucceed, message);
        }

        /// <summary>
        /// 是否执行成功
        /// </summary>
        public bool IsSucceed { get; set; } = true;

        /// <summary>
        /// 执行信息（一般是错误信息）
        /// </summary>
        public string Message { get; set; } = string.Empty;

    }

    public class ExecuteResult<T> : ExecuteResult
    {
        public T Result { get; set; }
        public ExecuteResult<T> Set(bool isSuccess, string message, T result)
        {
            IsSucceed = isSuccess;
            Message = message;
            Result = result;
            return this;
        }

        public ExecuteResult<T> SetData(T result)
        {
            return Set(true, string.Empty, result);
        }

        public new ExecuteResult<T> SetFail()
        {
            return Set(false, string.Empty, default);
        }

        public new ExecuteResult<T> SetFailMessage(string message)
        {
            return Set(false, message, default);
        }

        /// <summary>
        /// 如果是空的，没有信息，默认IsSucceed=true-
        /// </summary>
        public ExecuteResult()
        {
        }
        public ExecuteResult(string message)
        {
            Set(false, message);
        }
        public ExecuteResult(bool isSucceed, string message)
        {
            Set(isSucceed, message);
        }
        public ExecuteResult(T result)
        {
            SetData(result);
        }
    }
}
