using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MS.UnitOfWork.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace MS.UnitOfWork
{
    public static class UnitOfWorkServiceExtensions
    {
        public static IServiceCollection AddUnitOfWorkService<TContext>(this IServiceCollection services,System.Action<DbContextOptionsBuilder> action) where TContext:DbContext
        {
            services.AddDbContext<TContext>(action);//注册dbcontext
            services.AddScoped<IUnitOfWork<TContext>, UnitOfWork<TContext>>();//注册工作单元
            return services;
        }
    }
}
