using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using EzCoreKit.Cryptography;
using HHStorage.Base;
using HHStorage.Models.API.Request;
using HHStorage.Models.API.Response;
using HHStorage.Models.EF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HHStorage.Controllers {
    /// <summary>
    /// 使用者控制器
    /// </summary>
    public class UserController : BaseController {
        public UserController(HHStorageContext database) : base(database) { }

        /// <summary>
        /// 登入系統並取得Token
        /// </summary>
        /// <param name="loginInfo">登入資訊</param>
        /// <returns>Bearer Token</returns>
        [HttpPost("login")]
        public APIResponse Login([FromBody]UserLoginViewModel loginInfo) {
            if (!ModelState.IsValid) {
                throw new Exception("登入失敗");
            }

            var hashedPassword = loginInfo.Password.ToHashString<MD5>();

            var userInstance = Database.User.SingleOrDefault(
                x => x.Id == loginInfo.UserId &&
                     x.Password == hashedPassword);

            if (userInstance == null) {
                throw new Exception("登入失敗");
            }

            var claims = new[]{
                new Claim(JwtRegisteredClaimNames.Sub, loginInfo.UserId)
            };

            return new APIResponse() {
                Result = EzCoreKit.AspNetCore.EzJwtBearerHelper.GenerateToken(DateTime.MaxValue, claims)
            };
        }

        /// <summary>
        /// 建立新使用者
        /// </summary>
        /// <param name="userInfo">使用者資訊</param>
        /// <returns>建立結果</returns>
        [Authorize]
        [HttpPost]
        public async Task<APIResponse> NewUser([FromBody]UserLoginViewModel userInfo) {
            if (!ModelState.IsValid) {
                throw new Exception("資訊不全");
            }

            var user = Database.User.SingleOrDefault(x => x.Id == userInfo.UserId);
            if (user != null) {
                throw new Exception("重複使用者");
            }

            await Database.User.AddAsync(new User() {
                Id = userInfo.UserId,
                Password = userInfo.Password ?? ""
            });

            await Database.SaveChangesAsync();

            return new APIResponse();
        }

        /// <summary>
        /// 更新使用者密碼
        /// </summary>
        /// <param name="password">新密碼</param>
        /// <returns>密碼更新結果</returns>
        [Authorize]
        [HttpPut("password")]
        public async Task<APIResponse> UpdatePassword([FromBody]string password) {
            if (string.IsNullOrEmpty(password)) {
                throw new Exception("密碼不該為空");
            }

            User.Password = password.ToHashString<MD5>();

            await Database.SaveChangesAsync();

            return new APIResponse();
        }

        /// <summary>
        /// 刪除指定帳戶
        /// </summary>
        /// <param name="userId">指定使用者帳號</param>
        /// <returns>刪除帳號結果</returns>
        [Authorize]
        [HttpDelete("{userId}")]
        public async Task<APIResponse> Delete([FromRoute]string userId) {
            if (User.Id == userId) {
                throw new Exception("無法刪除自身帳號");
            }

            var user = Database.User.SingleOrDefault(x => x.Id == userId);
            if (user == null) {
                throw new Exception("找不到指定使用者");
            } else {
                Database.User.Remove(user);
            }

            await Database.SaveChangesAsync();

            return new APIResponse();
        }
    }
}