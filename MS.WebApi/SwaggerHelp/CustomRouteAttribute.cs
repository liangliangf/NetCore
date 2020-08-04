using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using MS.WebApi.SwaggerHelp;
using System;

namespace MS.WebApi.SwaggerHelp
{
    /// <summary>
    /// 自定义路由 /api/{version}/[controler]/[action]
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class CustomRouteAttribute: Microsoft.AspNetCore.Mvc.RouteAttribute, IApiDescriptionGroupNameProvider
    {
        /// <summary>
        /// 分组名称,是来实现接口 IApiDescriptionGroupNameProvider
        /// </summary>
        public string GroupName { get; set; }

        public CustomRouteAttribute()
            : base($"api/{ApiVersions.v1}/[controller]/[action]")
        {
            GroupName =ApiVersions.v1.ToString();
        }

        /// <summary>
        /// 自定义版本+路由构造函数，继承基类路由
        /// </summary>
        /// <param name="version"></param>
        /// <param name="actionName"></param>
        public CustomRouteAttribute(ApiVersions version, string actionName = "[action]")
            : base($"api/{version}/[controller]/{actionName}")
        {
            GroupName = version.ToString();
        }
    }
}
