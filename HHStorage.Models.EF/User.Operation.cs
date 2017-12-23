using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using EzCoreKit.Cryptography;
using HHStorage.Exceptions;

namespace HHStorage.Models.EF {
    public partial class User {
        /// <summary>
        /// 建立新使用者帳戶
        /// </summary>
        /// <param name="context">資料庫內容</param>
        /// <param name="id">帳戶</param>
        /// <param name="password">密碼</param>
        /// <returns>使用者實例</returns>
        public static async Task<User> Create(HHStorageContext context, string id, string password) {
            if (id == null || password == null) {
                throw new NotNullException("使用者帳戶名稱與密碼不該為null");
            }
            if (context.User.Any(x => x.Id == id)) {
                throw new DuplicateException("使用者帳戶名稱重複");
            }

            var result = new User() { Id = id, Password = password.ToHashString<MD5>() };
            await context.User.AddAsync(result);
            return result;
        }

        /// <summary>
        /// 使用帳號與密碼登入並取得使用者實例
        /// </summary>
        /// <param name="context">資料庫內容</param>
        /// <param name="id">帳號</param>
        /// <param name="password">密碼</param>
        /// <returns>使用者實例</returns>
        public static async Task<User> Login(HHStorageContext context, string id, string password) {
            if (id == null || password == null) {
                throw new NotNullException("使用者帳戶名稱與密碼不該為null");
            }

            var passwordHash = password.ToHashString<MD5>();

            return context.User.SingleOrDefault(x => x.Id == id && x.Password == passwordHash);
        }

        /// <summary>
        /// 刪除指定使用者帳戶所有資料
        /// </summary>
        /// <param name="context">資料庫內容</param>
        /// <param name="id">使用者帳戶</param>
        /// <returns>非同步操作</returns>
        public static async Task Delete(HHStorageContext context, string id) {
            if (id == null) {
                throw new NotNullException("使用者帳戶名稱不該為null");
            }
            if (id.ToLower() == "admin") {
                throw new OperationException("系統管理者帳戶admin不可刪除");
            }
            var targetUser = context.User.SingleOrDefault(x => x.Id == id);
            if (targetUser == null) {
                throw new NotFoundException("找不到指定的使用者");
            }

            // 刪除檔案
            foreach (var file in context.File.Where(x => x.UserId == id)) {
                await EF.File.Delete(context, file.Id);
            }
            context.RemoveRange(context.WebHook.Where(x => x.UserId == id));
            context.RemoveRange(context.Origin.Where(x => x.UserId == id));
            context.RemoveRange(context.Repository.Where(x => x.UserId == id));
        }
    }
}
