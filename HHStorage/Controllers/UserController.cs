using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using EzCoreKit.Cryptography;
using HHStorage.Base;
using HHStorage.Exceptions;
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
        /// 取得所有使用者列表
        /// </summary>
        /// <returns>使用者列表</returns>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<User>))]
        public async Task<IEnumerable<User>> GetList() {
            return Database.User;
        }

        /// <summary>
        /// 登入系統並取得Token
        /// </summary>
        /// <param name="loginInfo">登入資訊</param>
        /// <returns>Bearer Token</returns>
        [HttpPost("login")]
        [ProducesResponseType(200, Type = typeof(string))]
        public async Task<string> Login([Required][FromBody]UserLoginViewModel loginInfo) {
            if (!ModelState.IsValid) {
                throw new ParameterException();
            }

            var userInstance = await User.Login(Database, loginInfo.UserId, loginInfo.Password);

            if (userInstance == null) {
                throw new AuthorizeException();
            }

            var claims = new[]{
                new Claim(JwtRegisteredClaimNames.Sub, loginInfo.UserId)
            };

            return EzCoreKit.AspNetCore.EzJwtBearerHelper.GenerateToken(DateTime.MaxValue, claims);
        }

        /// <summary>
        /// 建立新使用者
        /// </summary>
        /// <param name="userInfo">使用者資訊</param>
        /// <returns>建立結果</returns>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(User))]
        public async Task<User> NewUser([Required][FromBody]UserLoginViewModel userInfo) {
            if (!ModelState.IsValid) {
                throw new ParameterException();
            }

            return await User.Create(Database, userInfo.UserId, userInfo.Password);
        }

        /// <summary>
        /// 更新目前使用者密碼
        /// </summary>
        /// <param name="password">新密碼</param>
        /// <returns>密碼更新結果</returns>
        [Authorize]
        [HttpPut("password")]
        public async Task UpdatePassword([Required][FromBody]string password) {
            if (string.IsNullOrEmpty(password)) {
                throw new NotNullException("密碼不該為null或空字串");
            }

            User.Password = password.ToHashString<MD5>();

            await Database.SaveChangesAsync();
        }

        /// <summary>
        /// 刪除指定帳戶
        /// </summary>
        /// <param name="userId">指定使用者帳號</param>
        /// <returns>刪除帳號結果</returns>
        [Authorize]
        [HttpDelete("{userId}")]
        public async Task Delete([Required][FromRoute]string userId) {
            if (User.Id == userId) {
                throw new Exception("無法刪除自身帳號");
            }

            await User.Delete(Database, userId);
        }
    }
}