using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Storage;
using MS.UnitOfWork.Repository;
using System.Threading.Tasks;
using System.Linq;

namespace MS.UnitOfWork.UnitOfWork
{
    public interface IUnitOfWork<TContext>:IDisposable where TContext : DbContext
    {
        /// <summary>
        /// 获取DbContext
        /// </summary>
        TContext DbContext { get; }

        /// <summary>
        /// 开始一个事务
        /// </summary>
        IDbContextTransaction BeginTransaction();

        /// <summary>
        /// 获取指定仓储
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="hasCustomRepository"></param>
        /// <returns></returns>
        IRepository<TEntity> GetRepository<TEntity>(bool hasCustomRepository = false) where TEntity :class ;

        /// <summary>
        /// DbContext提交修改
        /// </summary>
        /// <returns></returns>
        int SaveChanges();

        /// <summary>
        /// DbContext提交修改（异步）
        /// </summary>
        /// <returns></returns>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// 执行原生sql语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        int ExecuteSqlCommand(string sql, params object[] parameters);

        /// <summary>
        /// 使用原生sql查询来获取指定数据
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        IQueryable<TEntity> FromSql<TEntity>(string sql, params object[] parameters) where TEntity : class;

    }
}
