using MS.DbContexts;
using MS.Entities;
using MS.UnitOfWork.UnitOfWork;
using MS.WebCore.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MS.Models.ViewModel
{
    public class RoleViewModel
    {
        
        public long Id { get; set; }

        [Display(Name = "角色名称")] //Display是该字段的显示名称
        [Required(ErrorMessage ="{0}必填")] //Required注解标记该字段必填，不可为空
        [StringLength(10,ErrorMessage = "不能超过{1}个字符")] //StringLength注解标记该字段长度
        [RegularExpression(@"^[a-zA-Z0-9_]{4,6}",ErrorMessage = "只能包含字符、数字和下划线（4到6位）")] //RegularExpression注解是正则表达式验证 ;还有个Range注解特性是验证值的范围的，这里没用到
        public string Name { get; set; }

        [Display(Name = "角色显示名")]
        [Required(ErrorMessage = "{0}必填")]
        [StringLength(50, ErrorMessage = "不能超过{0}个字符")]
        public string DisplayName { get; set; }

        [Display(Name = "备注")]
        [StringLength(4000, ErrorMessage = "不能超过{0}个字符")]
        public string Remark { get; set; }

        /// <summary>
        /// 把对象字段的逻辑验证写在了ViewModel中，没有把它放在业务层是因为，
        /// 我认为对象字段本身的合法性和对象是强相关的，就和注解直接写在ViewModel中而不是Service中一样，
        /// 所以把字段的验证也写在了ViewModel里
        /// </summary>
        /// <param name="executeType"></param>
        /// <param name="unitOfWork"></param>
        /// <returns></returns>
        public ExecuteResult CheckField(ExecuteType executeType,IUnitOfWork<MSDbContext> unitOfWork)
        {
            ExecuteResult result = new ExecuteResult();
            var repo = unitOfWork.GetRepository<Role>();

            //如果不是新增角色，操作之前都要先检查角色是否存在
            if (executeType != ExecuteType.Create && !repo.Exists(x => x.Id == Id))
            {
                return result.SetFailMessage("角色不存在");
            }

            switch (executeType)
            {
                case ExecuteType.Delete:
                    //删除角色前检查角色下还没有员工
                    if (unitOfWork.GetRepository<User>().Exists(x => x.RoleId == Id))
                    {
                        return result.SetFailMessage("还有员工正在使用该角色，无法删除");
                    }
                    break;
                case ExecuteType.Update:
                    //如果存在Id不同，角色名相同的实体，则返回报错
                    if (repo.Exists(x => x.Id != Id && x.Name == Name))
                    {
                        return result.SetFailMessage($"已存在相同的角色名称：{Name}");
                    }
                    break;
                case ExecuteType.Create:
                    //如果存在相同的角色名，则返回报错
                    if (repo.Exists(x => x.Name == Name))
                    {
                        return result.SetFailMessage($"已存在相同的角色名称:{Name}");
                    }
                    break;
            }

            return result;
        }
    }
}
