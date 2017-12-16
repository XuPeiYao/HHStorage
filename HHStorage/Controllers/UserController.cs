using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HHStorage.Controllers {
    /// <summary>
    /// 使用者控制器
    /// </summary>
    [Authorize]
    public class UserController : Controller {

    }
}