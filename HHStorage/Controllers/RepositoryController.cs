using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HHStorage.Base;
using HHStorage.Models.EF;
using HHStorage.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HHStorage.Models.API.Response;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace HHStorage.Controllers {
    /// <summary>
    /// 儲存庫控制器
    /// </summary>
    public class RepositoryController : BaseController {
        public RepositoryController(HHStorageContext database) : base(database) { }

        /// <summary>
        /// 取得指定或目前使用者所有儲存庫列表的分頁結果
        /// </summary>
        /// <param name="userId">使用者帳號</param>
        /// <param name="skip">起始索引</param>
        /// <param name="take">取得筆數</param>
        /// <returns>儲存庫列表的分頁結果</returns>
        [Authorize]
        [HttpGet]
        [HttpGet("{userId}")]
        [ProducesResponseType(200, Type = typeof(APIPaging<Repository>))]
        public async Task<APIPaging<Repository>> GetPagingList(
            [FromRoute]string userId,
            [FromQuery]int? skip = 0,
            [FromQuery]int? take = 10) {
            var user = this.User;
            if (userId != null) {
                if (this.User.IsSuperUser()) {
                    user = Database.User.SingleOrDefault(x => x.Id == userId);
                } else if (user.Id != userId) {
                    throw new AuthorizeException();
                }
            }

            var result = Database.Repository.Where(x => x.User == user);
            return new APIPaging<Repository>() {
                TotalLength = result.Count(),
                Index = skip.Value,
                Length = take.Value,
                Result = result.Skip(skip.Value).Take(take.Value)
            };
        }

        /// <summary>
        /// 取得指定或目前使用者所有儲存庫列表
        /// </summary>
        /// <param name="userId">使用者帳號</param>
        /// <returns>儲存庫列表</returns>
        [Authorize]
        [HttpGet("list")]
        [HttpGet("{userId}/list")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Repository>))]
        public async Task<IEnumerable<Repository>> GetList(
            [FromRoute]string userId) {
            var user = this.User;
            if (userId != null) {
                if (this.User.IsSuperUser()) {
                    user = Database.User.SingleOrDefault(x => x.Id == userId);
                } else if (user.Id != userId) {
                    throw new AuthorizeException();
                }
            }

            return Database.Repository.Where(x => x.User == user);
        }

        /// <summary>
        /// 在指定使用者或目前使用者建立新的儲存庫
        /// </summary>
        /// <param name="userId">使用者帳號</param>
        /// <param name="name">名稱</param>
        /// <returns>儲存庫實例</returns>
        [Authorize]
        [HttpPost]
        [HttpPost("{userId}")]
        [ProducesResponseType(200, Type = typeof(Repository))]
        public async Task<Repository> CreateRepository(
            [FromRoute]string userId,
            [Required][FromBody]string name) {
            var user = this.User;
            if (userId != null) {
                if (this.User.IsSuperUser()) {
                    user = Database.User.SingleOrDefault(x => x.Id == userId);
                    if (user == null) {
                        throw new NotFoundException("找不到指定使用者");
                    }
                } else if (user.Id != userId) {
                    throw new AuthorizeException();
                }
            }

            return await Repository.Create(Database, user.Id, name);
        }

        /// <summary>
        /// 更新儲存庫
        /// </summary>
        /// <param name="repository">儲存庫實例更新資訊</param>
        /// <returns>儲存庫實例</returns>
        [Authorize]
        [HttpPut]
        [ProducesResponseType(200, Type = typeof(Repository))]
        public async Task<Repository> Update([FromBody]Repository repository) {
            if (!ModelState.IsValid) {
                throw new ParameterException();
            }

            var repos = Database.Repository.SingleOrDefault(x => x.Id == repository.Id);
            if (repos != null) throw new NotFoundException();

            if (!User.IsSuperUser() && repos.UserId != User.Id) {
                throw new AuthorizeException();
            }


            repos.Name = repository.Name;
            repos.UserId = repository.UserId;
            repos.AccessModifier = repository.AccessModifier;

            await Database.SaveChangesAsync();

            return repos;
        }

        /// <summary>
        /// 刪除儲存庫
        /// </summary>
        /// <param name="id">儲存庫唯一識別號</param>
        /// <returns>非同步操作</returns>
        [Authorize]
        [HttpDelete("{id}")]
        public async Task Delete(Guid id) {
            var repos = Database.Repository.SingleOrDefault(x => x.Id == id);
            if (repos == null) throw new NotFoundException();

            if (!User.IsSuperUser() && repos.UserId != User.Id) {
                throw new AuthorizeException();
            }

            await Repository.Delete(Database, id);
        }

        /// <summary>
        /// 取得指定儲存庫存取域列表
        /// </summary>
        /// <param name="repositoryId">儲存庫唯一識別號</param>
        /// <returns>存取域列表</returns>
        [Authorize]
        [HttpGet("{repositoryId}/origin")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Origin>))]
        public async Task<IEnumerable<Origin>> GetOriginList(Guid repositoryId) {
            var repos = Database.Repository.Include(x => x.Origin).SingleOrDefault(x => x.Id == repositoryId);

            if (repos == null) throw new NotFoundException();

            if (!User.IsSuperUser() && repos.UserId != User.Id) {
                throw new AuthorizeException();
            }

            return repos.Origin;
        }

        /// <summary>
        /// 更新存取域
        /// </summary>
        /// <param name="origin">存取域實例更新資訊</param>
        /// <returns>存取域實例</returns>
        [Authorize]
        [HttpPut("origin")]
        [ProducesResponseType(200, Type = typeof(Origin))]
        public async Task<Origin> UpdateOrigin([FromBody]Origin origin) {
            var originInstance = Database.Origin.Include(x => x.Repository).SingleOrDefault(x => x.Id == origin.Id);

            if (originInstance == null) throw new NotFoundException();

            if (!User.IsSuperUser() && originInstance.UserId != User.Id) {
                throw new AuthorizeException();
            }

            originInstance.OriginURI = origin.OriginURI;

            await Database.SaveChangesAsync();

            return originInstance;
        }

        /// <summary>
        /// 刪除存取域
        /// </summary>
        /// <param name="originId">存取域唯一識別號</param>
        /// <returns>非同步操作</returns>
        [Authorize]
        [HttpDelete("{originId}")]
        public async Task DeleteOrigin([FromRoute]Guid originId) {
            var originInstance = Database.Origin.Include(x => x.Repository).SingleOrDefault(x => x.Id == originId);

            if (originInstance == null) throw new NotFoundException();

            if (!User.IsSuperUser() && originInstance.UserId != User.Id) {
                throw new AuthorizeException();
            }

            Database.Origin.Remove(originInstance);

            await Database.SaveChangesAsync();
        }
    }
}