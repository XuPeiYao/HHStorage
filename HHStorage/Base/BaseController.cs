using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using HHStorage.Models.API.Response;
using HHStorage.Models.EF;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HHStorage.Base {
    /// <summary>
    /// 控制器基底
    /// </summary>
    [Route("api/[controller]")]
    public class BaseController : Controller {
        /// <summary>
        /// 資料庫內容
        /// </summary>
        public HHStorageContext Database { get; private set; }

        /// <summary>
        /// 目前登入使用者
        /// </summary>
        public new User User {
            get {
                var userId = base.User.Claims.SingleOrDefault(x =>
                    x.Properties.Any(y => y.Value == JwtRegisteredClaimNames.Sub)
                )?.Value;

                return Database.User.SingleOrDefault(x => x.Id == userId);
            }
        }

        /// <summary>
        /// 控制器基底建構子
        /// </summary>
        /// <param name="database">資料庫內容</param>
        public BaseController(HHStorageContext database) {
            this.Database = database;
        }

        public override void OnActionExecuted(ActionExecutedContext context) {
            if (context.Exception != null) {
                context.Result = new JsonResult(new APIResponse() {
                    Result = new APIError() {
                        Message = context.Exception.Message
                    }
                });
                context.ExceptionHandled = true;
            }

            base.OnActionExecuted(context);
        }
    }
}