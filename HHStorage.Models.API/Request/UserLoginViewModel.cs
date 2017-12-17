using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HHStorage.Models.API.Request {
    /// <summary>
    /// 使用者登入資訊
    /// </summary>
    public class UserLoginViewModel {
        /// <summary>
        /// 帳號
        /// </summary>
        [Required]
        public string UserId { get; set; }

        /// <summary>
        /// 密碼
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}
