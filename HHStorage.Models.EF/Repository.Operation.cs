using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HHStorage.Exceptions;

namespace HHStorage.Models.EF {
    public partial class Repository {
        /// <summary>
        /// 建立新儲存體
        /// </summary>
        /// <param name="context">資料庫內容</param>
        /// <param name="userId">使用者帳號</param>
        /// <param name="name">名稱</param>
        /// <returns>儲存體實例</returns>
        public static async Task<Repository> Create(HHStorageContext context, string userId, string name) {
            if (userId == null) {
                throw new NotNullException("使用者帳號不該為null");
            }
            if (context.User.Any(x => x.Id == userId)) {
                throw new NotFoundException("找不到指定使用者");
            }
            if (name == null) {
                throw new NotNullException("儲存體名稱不該為null");
            }
            var result = new Repository() {
                Name = name,
                UserId = userId
            };

            context.Repository.Add(result);
            await context.SaveChangesAsync();

            return result;
        }

        /// <summary>
        /// 刪除儲存體
        /// </summary>
        /// <param name="context">資料庫內容</param>
        /// <param name="id">儲存庫唯一識別號</param>
        /// <returns>非同步操作</returns>
        public static async Task Delete(HHStorageContext context, Guid id) {
            var repository = context.Repository.SingleOrDefault(x => x.Id == id);

            if (repository == null) {
                throw new NotFoundException("找不到指定的儲存庫");
            }

            foreach (var file in context.File.Where(x => x.RepositoryId == id)) {
                await EF.File.Delete(context, file.Id);
            }

            context.RemoveRange(context.WebHook.Where(x => x.RepositoryId == id));
            context.RemoveRange(context.Origin.Where(x => x.RepositoryId == id));
            context.Repository.Remove(repository);

            await context.SaveChangesAsync();
        }
    }
}
