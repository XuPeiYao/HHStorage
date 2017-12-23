using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HHStorage.Base;
using HHStorage.Models.EF;
using Microsoft.AspNetCore.Mvc;

namespace HHStorage.Controllers {
    /// <summary>
    /// 儲存庫控制器
    /// </summary>
    public class RepositoryController : BaseController {
        public RepositoryController(HHStorageContext database) : base(database) { }

        public IActionResult Index() {
            return View();
        }
    }
}