using Autofac;
using Autofac.Extras.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MS.Component.Aop
{
    public static class AopServiceExtensions
    {
        /// <summary>
        /// 注册aop服务拦截器
        /// 同时注册了各业务层接口与实现
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="serviceAssemblyName">业务层程序集名称</param>
        public static void AddAopService(this ContainerBuilder builder, string serviceAssemblyName)
        {
            //注册拦截器，同步异步都要
            builder.RegisterType<LogInterceptor>().AsSelf();
            builder.RegisterType<LogInterceptorAsync>().AsSelf();

            //注册业务层，同时对业务层的方法进行拦截
            builder.RegisterAssemblyTypes(Assembly.Load(serviceAssemblyName))
                .AsImplementedInterfaces()//按接口-实现类对应注册
                .InstancePerLifetimeScope()//在一个生命周期域中，每一个依赖或调用创建一个单一的共享的实例，且每一个不同的生命周期域，实例是唯一的，不共享的。
                .EnableInterfaceInterceptors()//引用Autofac.Extras.DynamicProxy;
                .InterceptedBy(new Type[] { typeof(LogInterceptor) })//这里只有同步的，因为异步方法拦截器还是先走同步拦截器 
                ;

            //在给出的服务层dll中找到以Service结尾的类，按接口-实现类对应注册，生命周期域为“每次请求”
            //builder.RegisterAssemblyTypes(Assembly.Load(serviceAssemblyName))
            //                .Where(t => t.Name.EndsWith("Service"))//找到以Service结尾的类
            //                .AsImplementedInterfaces()//按接口-实现类对应注册
            //                .InstancePerLifetimeScope();//生命周期域为“每次请求”

            //业务层注册拦截器也可以使用[Intercept(typeof(LogInterceptor))]加在类上，但是上面的方法比较好，没有侵入性
        }




        //public static IServiceCollection AddAopService(this IServiceCollection services, string serviceAssemblyName)
        //{
        //    services.AddScoped<LogInterceptor>();
        //    services.AddScoped<LogInterceptorAsync>();

        //    Assembly assembly = Assembly.Load(serviceAssemblyName);
        //    List<Type> ts = assembly.GetTypes().Where(x => x.IsClass && !x.IsAbstract && !x.IsGenericType).ToList();
        //    foreach (var item in ts)
        //    {
        //        var interfaceType = item.GetInterfaces();
        //        if (interfaceType.Length == 1)
        //        {
        //            services.AddScoped(interfaceType[0], item);
        //        }
        //        if (interfaceType.Length> 1)
        //        {
        //            services.AddScoped(interfaceType[1], item);
        //        }
        //    }
        //    return services;
        //}
    }
}
