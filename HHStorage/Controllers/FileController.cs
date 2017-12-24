using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EzCoreKit.Extensions;
using HHStorage.Base;
using HHStorage.Exceptions;
using HHStorage.Models.API.Request;
using HHStorage.Models.API.Response;
using HHStorage.Models.EF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HHStorage.Controllers {
    /// <summary>
    /// 檔案控制器
    /// </summary>
    public class FileController : BaseController {
        public FileController(HHStorageContext database) : base(database) { }

        /// <summary>
        /// 下載指定檔案
        /// </summary>
        /// <param name="fileId">檔案唯一識別號</param>
        /// <param name="token">存取權杖</param>
        /// <returns>檔案串流</returns>
        [HttpGet("{fileId}/download")]
        [ProducesResponseType(200, Type = typeof(FileStreamResult))]
        public async Task<FileStreamResult> Download(
            [FromRoute]Guid fileId,
            [FromQuery]string token = null) {
            var file = Database.File.Include(x => x.Repository).ThenInclude(x => x.Origin)
                .SingleOrDefault(x => x.Id == fileId);
            if (file == null) {
                throw new NotFoundException("找不到指定的檔案");
            }

            Response.Headers.Add("Access-Control-Allow-Origin", string.Join(",", file.Repository.Origin.Select(x => x.OriginURI)));

            if (token == null) { // 未使用存取權障
                // 非公開且未登入，必定無法存取
                if (file.AccessModifier != AccessModifierTypes.Public && User == null) {
                    throw new AuthorizeException();
                }

                // 設定為私人，但不是儲存庫擁有者也不是檔案擁有者也不是超級使用者
                if (file.AccessModifier == AccessModifierTypes.Private &&
                    User.Id != file.Repository.UserId &&
                    User.Id != file.UserId &&
                    !User.IsSuperUser()) {
                    throw new AuthorizeException();
                }
            } else { // 檢查權杖
                var tokenInfo = EzCoreKit.AspNetCore.EzJwtBearerHelper.ValidToken(token);
                if (fileId != Guid.Parse(tokenInfo.Claims.SingleOrDefault(x => x.Type == "fileId")?.Value) &&
                    tokenInfo.Claims.SingleOrDefault(x => x.Type == "tokenType")?.Value != FileTokenTypes.Download.ToString()) {
                    throw new AuthorizeException();
                }

                var exp = tokenInfo.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value;
                if (DateTimeConvert.FromUnixTimestamp(long.Parse(exp)) < DateTime.UtcNow) {
                    throw new AuthorizeException("Token過期");
                }
            }

            return File(file.GetFileStream(), file.ContentType, file.Name);
        }

        /// <summary>
        /// 產生檔案存取權杖
        /// </summary>
        /// <param name="fileId">檔案唯一識別號</param>
        /// <param name="type">權杖類型</param>
        /// <returns>檔案存取權杖</returns>
        [Authorize]
        [HttpGet("{fileId}/token")]
        [ProducesResponseType(200, Type = typeof(string))]
        public async Task<string> GenerateToken([FromRoute]Guid fileId, [FromQuery]FileTokenTypes type = FileTokenTypes.Download) {
            var file = Database.File.Include(x => x.Repository).ThenInclude(x => x.Origin)
                .SingleOrDefault(x => x.Id == fileId);
            if (file == null) {
                throw new NotFoundException("找不到指定的檔案");
            }

            Response.Headers.Add("Access-Control-Allow-Origin",
                string.Join(",", file.Repository.Origin.Select(x => x.OriginURI)));

            if (file.Repository.AccessModifier == AccessModifierTypes.Private &&
                !User.IsSuperUser() &&
                file.Repository.UserId != User.Id) {
                throw new AuthorizeException();
            }

            var claims = new[]{
                new Claim("fileId", fileId.ToString()),
                new Claim("tokenType", type.ToString())
            };

            return EzCoreKit.AspNetCore.EzJwtBearerHelper.GenerateToken(DateTime.Now.AddMinutes(10), claims);
        }

        /// <summary>
        /// 取得指定儲存庫檔案列表的分頁結果
        /// </summary>
        /// <param name="repositoryId">儲存庫唯一識別號</param>
        /// <param name="skip">起始索引</param>
        /// <param name="take">取得筆數</param>
        /// <returns>檔案列表的分頁結果</returns>
        [Authorize]
        [HttpGet]
        [HttpGet("{repositoryId}")]
        [ProducesResponseType(200, Type = typeof(APIPaging<Repository>))]
        public async Task<APIPaging<File>> GetPagingList(
            [FromRoute]Guid repositoryId,
            [FromQuery]int? skip = 0,
            [FromQuery]int? take = 10) {
            var repos = Database.Repository.Include(x => x.File).Include(x => x.Origin).SingleOrDefault(x => x.Id == repositoryId);
            if (repos == null) {
                throw new NotFoundException("找不到指定的儲存庫");
            }

            Response.Headers.Add("Access-Control-Allow-Origin", string.Join(",", repos.Origin.Select(x => x.OriginURI)));

            if (!User.IsSuperUser() &&
                repos.UserId != User.Id &&
                repos.AccessModifier == AccessModifierTypes.Private) {
                throw new AuthorizeException();
            }

            return new APIPaging<File>() {
                TotalLength = repos.File.Count,
                Index = skip.Value,
                Length = take.Value,
                Result = repos.File.Skip(skip.Value).Take(take.Value)
            };
        }

        /// <summary>
        /// 上傳檔案
        /// </summary>
        /// <param name="file">檔案</param>
        /// <param name="repositoryId">儲存庫唯一識別號</param>
        /// <param name="accessModifierType">存取限制</param>
        /// <returns>檔案實例</returns>
        [Authorize]
        [HttpPost("{repositoryId}")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<File>))]
        public async Task<File> Upload(
            IFormFile file,
            [FromRoute]Guid repositoryId,
            [FromForm]AccessModifierTypes accessModifierType = AccessModifierTypes.Private) {
            var repository = Database.Repository.Include(x => x.Origin).SingleOrDefault(x => x.Id == repositoryId);
            if (repository == null) {
                throw new NotFoundException("找不到指定的儲存庫");
            }

            Response.Headers.Add("Access-Control-Allow-Origin",
                string.Join(",", repository.Origin.Select(x => x.OriginURI)));

            if (repository.AccessModifier == AccessModifierTypes.Private &&
                !User.IsSuperUser() &&
                repository.UserId != User.Id) {
                throw new AuthorizeException();
            }

            return await Models.EF.File.Create(Database, this.User.Id, repositoryId, file.OpenReadStream(), file.ContentType, file.FileName, accessModifierType);
        }

        [Authorize]
        [HttpPost("{repositoryId}/empty")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<File>))]
        public async Task<File> CreateEmptyFile(
            [FromRoute]Guid repositoryId,
            [FromBody]AccessModifierTypes accessModifierType = AccessModifierTypes.Private) {
            var repository = Database.Repository.Include(x => x.Origin).SingleOrDefault(x => x.Id == repositoryId);
            if (repository == null) {
                throw new NotFoundException("找不到指定的儲存庫");
            }

            Response.Headers.Add("Access-Control-Allow-Origin",
                string.Join(",", repository.Origin.Select(x => x.OriginURI)));

            if (repository.AccessModifier == AccessModifierTypes.Private &&
                !User.IsSuperUser() &&
                repository.UserId != User.Id) {
                throw new AuthorizeException();
            }

            return await Models.EF.File.Create(
                Database,
                this.User.Id,
                repositoryId,
                new System.IO.MemoryStream(),
                null,
                null,
                accessModifierType);
        }


        [HttpPost]
        [ProducesResponseType(200, Type = typeof(IEnumerable<File>))]
        public async Task<File> UploadEmptyFile([FromQuery]string token, IFormFile file) {
            if (string.IsNullOrEmpty(token) && User == null) {
                throw new NotNullException("Token不該為空");
            }

            var tokenInfo = EzCoreKit.AspNetCore.EzJwtBearerHelper.ValidToken(token);

            var fileId = Guid.Parse(tokenInfo.Claims.SingleOrDefault(x => x.Type == "fileId")?.Value);

            var fileInstance = Database.File.Include(x => x.Repository).ThenInclude(x => x.Origin).SingleOrDefault(x => x.Id == fileId);

            if (fileInstance == null) {
                throw new NotFoundException("找不到指定檔案");
            }

            if (User != null &&
                fileInstance.UserId != this.User.Id &&
                fileInstance.Repository.UserId != this.User.Id &&
                fileInstance.Repository.AccessModifier == AccessModifierTypes.Private) {
                throw new AuthorizeException();
            }

            Response.Headers.Add("Access-Control-Allow-Origin",
                string.Join(",", fileInstance.Repository.Origin.Select(x => x.OriginURI)));

            var exp = tokenInfo.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value;
            if (DateTimeConvert.FromUnixTimestamp(long.Parse(exp)) < DateTime.UtcNow) {
                throw new AuthorizeException("Token過期");
            }

            if (tokenInfo.Claims.SingleOrDefault(x => x.Type == "tokenType")?.Value != FileTokenTypes.Upload.ToString()) {
                throw new AuthorizeException();
            }

            await Models.EF.File.Append(Database, fileInstance.Id, file.OpenReadStream());

            fileInstance.Name = file.FileName;
            fileInstance.ContentType = file.ContentType;
            fileInstance.Size = file.Length;

            await Database.SaveChangesAsync();

            return fileInstance;
        }

        /// <summary>
        /// 刪除檔案
        /// </summary>
        /// <param name="fileId">檔案唯一識別號</param>
        /// <returns>非同步操作</returns>
        [Authorize]
        [HttpDelete("{fileId}")]
        public async Task Delete(Guid fileId) {
            var file = Database.File.Include(x => x.Repository).ThenInclude(x => x.Origin).SingleOrDefault(x => x.Id == fileId);
            if (file == null) {
                throw new NotFoundException("找不到指定的檔案");
            }

            Response.Headers.Add("Access-Control-Allow-Origin",
                string.Join(",", file.Repository.Origin.Select(x => x.OriginURI)));

            if (file.Repository.AccessModifier == AccessModifierTypes.Private &&
                !User.IsSuperUser() &&
                file.Repository.UserId != User.Id) {
                throw new AuthorizeException();
            }

            await Models.EF.File.Delete(Database, fileId);
        }
    }
}