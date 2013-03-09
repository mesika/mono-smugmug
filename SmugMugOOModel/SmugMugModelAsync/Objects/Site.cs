using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;

namespace SmugMugModel
{
    public class Site
    {
        static public IWebProxy Proxy { get; set; }

        #region Sync
        /// <summary>
        /// Pings SmugMug
        /// </summary>
        /// <returns>Returns an empty successful response, if it completes without error.</returns>
        public bool Ping()
        {
            return PingAsync().Result;
        }

        /// <summary>
        /// Authenticates a user based on the specified email address (or nickname) and password
        /// </summary>
        /// <param name="EmailAddress">The email address (or nickname) for the user.</param>
        /// <param name="Password">The password for the user.</param>
        /// <returns>Login (AccountStatus, Type, FileSizeLimit, PasswordHash, SessionID, SmugVault, User (id, DisplayName, NickName))</returns>
        public MyUser Login(string EmailAddress, string Password)
        {
            return Login(EmailAddress, Password, "");
        }
        /// <summary>
        /// Authenticates a user based on the specified email address (or nickname) and password
        /// </summary>
        /// <param name="EmailAddress">The email address (or nickname) for the user.</param>
        /// <param name="Password">The password for the user.</param>
        /// <param name="Extras">A comma separated string of additional attributes to return in the response</param>
        /// <returns>Login (AccountStatus, Type, FileSizeLimit, PasswordHash, SessionID, SmugVault, User (id, DisplayName, NickName))</returns>
        public MyUser Login(string EmailAddress, string Password, string Extras)
        {
            return LoginAsync(EmailAddress, Password, Extras).Result;
        }
        /// <summary>
        /// Authenticates a user based on the specified user id and password hash.
        /// </summary>
        /// <param name="UserId">The id for a specific user</param>
        /// <param name="PasswordHash">The password hash for the user</param>
        /// <returns>Login (AccountStatus, AccountType, FileSizeLimit, SessionID, SmugVault, User (id, DisplayName, NickName, URL))</returns>
        public MyUser Login(int UserId, string PasswordHash)
        {
            return Login(UserId, PasswordHash, "");
        }

        /// <summary>
        /// Authenticates a user based on the specified user id and password hash.
        /// </summary>
        /// <param name="UserId">The id for a specific user</param>
        /// <param name="PasswordHash">The password hash for the user</param>
        /// <param name="Extras">A comma separated string of additional attributes to return in the response</param>
        /// <returns>Login (AccountStatus, AccountType, FileSizeLimit, SessionID, SmugVault, User (id, DisplayName, NickName, URL))</returns>
        public MyUser Login(int UserId, string PasswordHash, string Extras)
        {
            return LoginAsync(UserId, PasswordHash, Extras).Result;
        }

        /// <summary>
        /// Establishes an anonymous session.
        /// </summary>
        /// <returns>Login (Session (id))</returns>
        public MyUser Login()
        {
            return LoginAsync().Result;
        }

        /// <summary>
        /// Retrieves a list of style templates
        /// </summary>
        /// <returns>List of Templates (id and Name)</returns>
        public List<Template> StylesGetTemplates()
        {
            return StylesGetTemplatesAsync(false).Result;
        }

        /// <summary>
        /// Retrieves a list of style templates
        /// </summary>
        /// <param name="Associative">Returns an associative array. Default: false </param>
        /// <returns>List of Templates (id and Name)</returns>
        public List<Template> StylesGetTemplates(bool Associative)
        {
            return StylesGetTemplatesAsync(Associative).Result;
        }
        #endregion

        #region Async
        /// <summary>
        /// Pings SmugMug
        /// </summary>
        /// <returns>Returns an empty successful response, if it completes without error.</returns>
        public async Task<bool> PingAsync()
        {
            // APIKey [required], Callback, Pretty
            CommunicationHelper ch = new CommunicationHelper();
            var resp = await ch.ExecuteMethod<SmugMugResponse>("smugmug.service.ping", null);
            if (resp.stat == "ok")
                return true;
            else
            {
                Console.WriteLine(resp.message);
                throw new SmugMugException(resp.code, resp.message, resp.method);
            }
        }

        /// <summary>
        /// Authenticates a user based on the specified email address (or nickname) and password
        /// </summary>
        /// <param name="EmailAddress">The email address (or nickname) for the user.</param>
        /// <param name="Password">The password for the user.</param>
        /// <returns>Login (AccountStatus, Type, FileSizeLimit, PasswordHash, SessionID, SmugVault, User (id, DisplayName, NickName))</returns>
        public async Task<MyUser> LoginAsync(string EmailAddress, string Password)
        {
            return await LoginAsync(EmailAddress, Password, "");
        }

        /// <summary>
        /// Authenticates a user based on the specified email address (or nickname) and password
        /// </summary>
        /// <param name="EmailAddress">The email address (or nickname) for the user.</param>
        /// <param name="Password">The password for the user.</param>
        /// <param name="Extras">A comma separated string of additional attributes to return in the response</param>
        /// <returns>Login (AccountStatus, Type, FileSizeLimit, PasswordHash, SessionID, SmugVault, User (id, DisplayName, NickName))</returns>
        public async Task<MyUser> LoginAsync(string EmailAddress, string Password, string Extras)
        {
            CommunicationHelper ch = new CommunicationHelper();

            // APIKey [required], EmailAddress [required], Password [required], Callback (JSON & PHP responses only), Extras, Pretty, Sandboxed, Strict
            var tuple = await ch.ExecuteMethodReturnCookie<LoginResponse>("smugmug.login.withPassword", null, "EmailAddress", EmailAddress, "Password", Password, "Extras", Extras);
            var resp = tuple.Item1;
            string su = tuple.Item2;
            if (resp.stat == "ok")
            {
                MyUser u = new MyUser();
                resp.Login.PopulateUser(u);
                u.basic._su = su;
                return u;
            }
            else
            {
                Console.WriteLine(resp.message);
                throw new SmugMugException(resp.code, resp.message, resp.method);
            }
        }

        /// <summary>
        /// Authenticates a user based on the specified user id and password hash.
        /// </summary>
        /// <param name="UserId">The id for a specific user</param>
        /// <param name="PasswordHash">The password hash for the user</param>
        /// <returns>Login (AccountStatus, AccountType, FileSizeLimit, SessionID, SmugVault, User (id, DisplayName, NickName, URL))</returns>
        public async Task<MyUser> LoginAsync(int UserId, string PasswordHash)
        {
            return await LoginAsync(UserId, PasswordHash, "");
        }

        /// <summary>
        /// Authenticates a user based on the specified user id and password hash.
        /// </summary>
        /// <param name="UserId">The id for a specific user</param>
        /// <param name="PasswordHash">The password hash for the user</param>
        /// <param name="Extras">A comma separated string of additional attributes to return in the response</param>
        /// <returns>Login (AccountStatus, AccountType, FileSizeLimit, SessionID, SmugVault, User (id, DisplayName, NickName, URL))</returns>
        public async Task<MyUser> LoginAsync(int UserId, string PasswordHash, string Extras)
        {
            CommunicationHelper ch = new CommunicationHelper();
            // APIKey [required], Callback, Extras, PasswordHash [required], Pretty, Sandboxed, Strict, UserID [required]

            var tuple = await ch.ExecuteMethodReturnCookie<LoginResponse>("smugmug.login.withHash", null, "UserID", UserId, "PasswordHash", PasswordHash, "Extras", Extras);
            var resp = tuple.Item1;
            string su = tuple.Item2;
            if (resp.stat == "ok")
            {
                MyUser u = new MyUser();
                resp.Login.PopulateUser(u);
                u.basic._su = su;
                return u;
            }
            else
            {
                Console.WriteLine(resp.message);
                throw new SmugMugException(resp.code, resp.message, resp.method);
            }
        }

        /// <summary>
        /// Establishes an anonymous session.
        /// </summary>
        /// <returns>Login (Session (id))</returns>
        public async Task<MyUser> LoginAsync()
        {
            // APIKey [required], Callback, Pretty, Strict
            CommunicationHelper ch = new CommunicationHelper();
            var resp = await ch.ExecuteMethod<LoginResponse>("smugmug.login.anonymously", null);
            if (resp.stat == "ok")
            {
                MyUser currentUser = new MyUser();
                if (currentUser.basic == null)
                    currentUser.basic = new SmugMugBase();
                currentUser.basic.SessionID = resp.Login.Session.id;
                currentUser.basic.NickName = "";
                currentUser.DisplayName = "";
                return currentUser;
            }
            else
            {
                Console.WriteLine(resp.message);
                throw new SmugMugException(resp.code, resp.message, resp.method);
            }
        }

        /// <summary>
        /// Retrieves a list of style templates
        /// </summary>
        /// <returns>List of Templates (id and Name)</returns>
        public async Task<List<Template>> StylesGetTemplatesAsync()
        {
            return await StylesGetTemplatesAsync(false);
        }

        /// <summary>
        /// Retrieves a list of style templates
        /// </summary>
        /// <param name="Associative">Returns an associative array. Default: false </param>
        /// <returns>List of Templates (id and Name)</returns>
        public async Task<List<Template>> StylesGetTemplatesAsync(bool Associative)
        {
            CommunicationHelper ch = new CommunicationHelper();
            // Associative, Callback, Pretty, Strict
            var resp = await ch.ExecuteMethod<TemplateResponse>("smugmug.styles.getTemplates", null, "Associative", Associative);
            if (resp.stat == "ok")
            {
                var temp = new List<Template>();
                temp.AddRange(resp.Templates);
                return temp;
            }
            else
            {
                Console.WriteLine(resp.message);
                throw new SmugMugException(resp.code, resp.message, resp.method);
            }
        }
        #endregion
    }
}
