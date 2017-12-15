using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HHStorage.Client {
    public class HHStorageClient {
        /// <summary>
        /// 存取權杖
        /// </summary>
        private string token;

        /// <summary>
        /// 是否已登入
        /// </summary>
        public bool IsLogin => token != null;
        
        /// <summary>
        /// 使用帳號密碼登入儲存體
        /// </summary>
        /// <param name="user">帳號</param>
        /// <param name="password">密碼</param>
        public async Task Login(string host, string user, string password) {

        }

        /// <summary>
        /// 釋放存取權杖
        /// </summary>
        public void Logout() {
            token = null;
        }
    }
}
