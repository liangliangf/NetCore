using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MS.Services
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// 获取程序集名称
        /// (用于获取业务层的程序集名称，提供给Autofac进行批量的注册接口和实现)
        /// </summary>
        /// <returns></returns>
        public static string GetAssemblyName()
        {
            return Assembly.GetExecutingAssembly().GetName().Name;
        }
    }
}
