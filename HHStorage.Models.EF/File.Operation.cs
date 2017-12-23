using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HHStorage.Exceptions;

namespace HHStorage.Models.EF {
    public partial class File {
        public static string SaveFilePath = "/Files";

        public static string GetFilePathById(Guid id) {
            return SaveFilePath + "/" + id;
        }


        /// <summary>
        /// 建立新檔案
        /// </summary>
        /// <param name="context">資料庫內容</param>
        /// <param name="userId">使用者帳號</param>
        /// <param name="repositoryId">儲存庫唯一識別號</param>
        /// <param name="stream">檔案串流</param>
        /// <param name="contentType">檔案ContentType</param>
        /// <param name="name">檔案名稱</param>
        /// <param name="accessModifier">存取限制</param>
        /// <returns>檔案實例</returns>
        public static async Task<File> Create(
            HHStorageContext context,
            string userId,
            Guid repositoryId,
            Stream stream,
            string contentType,
            string name = null,
            AccessModifierTypes accessModifier = AccessModifierTypes.Private
            ) {
            if (userId == null) {
                throw new NotNullException("使用者帳號不該為null");
            }
            if (contentType == null) {
                throw new NotNullException("ContentType不該為null");
            }
            if (!context.Repository.Any(x => x.Id == repositoryId && x.UserId == userId)) {
                throw new NotFoundException("找不到該使用者指定儲存庫");
            }

            var result = new File() {
                UserId = userId,
                RepositoryId = repositoryId,
                ContentType = contentType
            };
            result.Name = name;
            result.AccessModifier = accessModifier;
            result.Size = stream.Length;

            try {
                using (FileStream fileStream = System.IO.File.Create(GetFilePathById(result.Id))) {
                    await stream.CopyToAsync(fileStream);
                    await fileStream.FlushAsync();
                }
            } catch {
                throw new OperationInterruptedException("檔案上傳過程遭到中斷");
            }

            context.File.Add(result);
            await context.SaveChangesAsync();

            return result;
        }

        /// <summary>
        /// 附加現有檔案
        /// </summary>
        /// <param name="context">資料庫內容</param>
        /// <param name="fileId">檔案唯一識別號</param>
        /// <param name="stream">檔案串流</param>
        /// <returns>檔案實例</returns>
        public static async Task<File> Append(HHStorageContext context, Guid fileId, Stream stream) {
            var file = context.File.SingleOrDefault(x => x.Id == fileId);
            if (file == null) {
                throw new NotFoundException("找不到指定檔案");
            }

            try {
                using (FileStream fileStream = System.IO.File.OpenWrite(GetFilePathById(file.Id))) {
                    await stream.CopyToAsync(fileStream);
                    await fileStream.FlushAsync();
                }
            } catch {
                throw new OperationInterruptedException("檔案上傳過程遭到中斷");
            }

            file.Size += stream.Length;

            await context.SaveChangesAsync();

            return file;
        }

        /// <summary>
        /// 刪除指定檔案
        /// </summary>
        /// <param name="context">資料庫內容</param>
        /// <param name="fileId">檔案唯一識別號</param>
        /// <returns>非同步操作</returns>
        public static async Task Delete(HHStorageContext context, Guid fileId) {
            var file = context.File.SingleOrDefault(x => x.Id == fileId);
            if (file == null) {
                throw new NotFoundException("找不到指定檔案");
            }

            // 刪除實體檔案
            if (System.IO.File.Exists(GetFilePathById(fileId))) {
                System.IO.File.Delete(GetFilePathById(fileId));
            }

            context.File.Remove(file);

            await context.SaveChangesAsync();
        }
    }
}
