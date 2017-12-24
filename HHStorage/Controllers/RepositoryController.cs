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
        [HttpGet("{userId?}")]
        [ProducesResponseType(200, Type = typeof(APIPaging<Repository>))]
        public async Task<APIPaging<Repository>> GetPagingList(
            string userId,
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
        [HttpGet("{userId?}/list")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Repository>))]
        public async Task<IEnumerable<Repository>> GetList(string userId) {
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
    }
}