using AutoMapper;
using Microsoft.Extensions.Localization;
using MS.Common.IDCode;
using MS.Component.Jwt.UserClaim;
using MS.DbContexts;
using MS.Entities;
using MS.Models.ViewModel;
using MS.UnitOfWork.UnitOfWork;
using MS.WebCore.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MS.Services //MS.Services.Role  PS:注意我这里两个类的命名空间都改为namespace MS.Services
{
    public class RoleService : BaseService, IRoleService
    {
        public RoleService(IUnitOfWork<MSDbContext> unitOfWork, IMapper mapper, IdWorker idWorker, IClaimsAccessor claimsAccessor, IStringLocalizer localizer) : base(unitOfWork, mapper, idWorker, claimsAccessor, localizer)
        {
        }

        public async Task<ExecuteResult<Role>> Create(RoleViewModel viewModel)
        {
            ExecuteResult<Role> result = new ExecuteResult<Role>();

            //检查字段
            if (viewModel.CheckField(ExecuteType.Create, _unitOfWork) is ExecuteResult checkResult && !checkResult.IsSucceed)
            {
                return result.SetFailMessage(checkResult.Message);
            }

            //开启一个事务(不用可以去掉)
            using (var trans = _unitOfWork.BeginTransaction())
            {
                Role newRole = _mapper.Map<Role>(viewModel);//_mapper.Map方法把字段从ViewModel映射到实体类中
                newRole.Id = _idWorker.NextId();//_idWorker.NextId()方法获取一个雪花Id
                newRole.CreateTime = DateTime.Now;
                newRole.Creator = _claimsAccessor.UserId; //111111222222;//由于暂时还没有做登录，所以拿不到登录者信息，先随便写一个后面再完善

                _unitOfWork.GetRepository<Role>().Insert(newRole);
                await _unitOfWork.SaveChangesAsync();
                await trans.CommitAsync();//提交事务

                result.SetData(newRole);//添加成功，把新的实体返回回去
            }

            return result;
        }

        public async Task<ExecuteResult> Delete(RoleViewModel viewModel)
        {
            ExecuteResult result = new ExecuteResult();

            //检查字段
            if (viewModel.CheckField(ExecuteType.Delete, _unitOfWork) is ExecuteResult checkResult && !checkResult.IsSucceed)
            {
                return checkResult;
            }

            _unitOfWork.GetRepository<Role>().Delete(viewModel.Id);
            await _unitOfWork.SaveChangesAsync();//提交
            return result;
        }

        public async Task<ExecuteResult> Update(RoleViewModel viewModel)
        {
            ExecuteResult<Role> result = new ExecuteResult<Role>();

            if (viewModel.CheckField(ExecuteType.Update, _unitOfWork) is ExecuteResult checkResult && !checkResult.IsSucceed)
            {
                return result.SetFailMessage(checkResult.Message);
            }

            //在viewModel.CheckField中已经获取了一次用于检查，所以此处不会重复再从数据库取一次，有缓存
            var row =await _unitOfWork.GetRepository<Role>().FindAsync(viewModel.Id);

            //修改对应的值
            row.Name = viewModel.Name;
            row.DisplayName = viewModel.DisplayName;
            row.Remark = viewModel.Remark;
            row.Modifier = 1219490056771866624;//由于暂时还没有做登录，所以拿不到登录者信息，先随便写一个后面再完善
            row.ModifyTime = DateTime.Now;
            _unitOfWork.GetRepository<Role>().Update(row);
            await _unitOfWork.SaveChangesAsync();//提交

            return result;
        }

    }
}
