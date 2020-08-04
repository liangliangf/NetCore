using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
namespace NetCore.Api.SwaggerHelp
{
    /// <summary>
    /// 自定义路由 /api/{version}/[controler]/[action]
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class CustomRouteAttribute : RouteAttribute, IApiDescriptionGroupNameProvider
    {
        /// <summary>
        /// 分组名称,是来实现接口 IApiDescriptionGroupNameProvider
        /// </summary>
        public string GroupName { get; set; }


        //public CustomRouteAttribute(string actionName = "[action]")
        //    : base($"api/[controller]/{actionName}")
        //{
        //    //GroupName = version.ToString();
        //}


        public CustomRouteAttribute()
            : base($"api/{CustomApiVersion.ApiVersions.v1}/[controller]/[action]")
        {
            GroupName = CustomApiVersion.ApiVersions.v1.ToString();
        }

        /// <summary>
        /// 自定义版本+路由构造函数，继承基类路由
        /// </summary>
        /// <param name="version"></param>
        /// <param name="actionName"></param>
        public CustomRouteAttribute(CustomApiVersion.ApiVersions version, string actionName = "[action]")
            : base($"api/{version}/[controller]/{actionName}")
        {
            GroupName = version.ToString();
        }



    }
}
