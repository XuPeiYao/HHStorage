using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HHStorage.Models.EF;
using Microsoft.AspNetCore.Mvc;

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
        /// 控制器基底建構子
        /// </summary>
        /// <param name="database">資料庫內容</param>
        public BaseController(HHStorageContext database) {
            this.Database = database;
        }
    }
}