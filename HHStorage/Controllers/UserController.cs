using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HHStorage.Base;
using HHStorage.Models.API.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HHStorage.Controllers {
    /// <summary>
    /// 使用者控制器
    /// </summary>
    public class UserController : BaseController {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="kk"></param>
        /// <returns></returns>
        [HttpGet]
        public APIResponse<object> Get(string kk) {
            return new Models.API.Response.APIResponse<object>();
        }
    }
}