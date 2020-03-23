using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace AuthorizationAPI.Utilities
{
    public class FirebaseUtils
    {
        private readonly IConfiguration _config;
        private readonly HttpClient client = new HttpClient();

        public FirebaseUtils(IConfiguration config)
        {
            _config = config;
        }

        public async Task<FirebaseToken> ValidateCredentials(string email, string password)
        {
            var credentials = new LoginCredentials
            {
                email = email,
                password = password
            };

            // Get ID token
            var stringContent = new StringContent(JsonConvert.SerializeObject(credentials), Encoding.UTF8,
                "application/json");
            var response =
                await client.PostAsync($"{"https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyPassword"}?key={"AIzaSyBDrGBxMKixIlVpw6cwQJSn-plHfw9SGfw"}",
                    stringContent);
            if ((int) response.StatusCode >= 500) throw new UnableToConnectToFirebaseException(response.ReasonPhrase);
            if (response.StatusCode == HttpStatusCode.BadRequest)
                throw new BadUserOrPasswordException(response.ReasonPhrase);

            var httpResponseContent = await response.Content.ReadAsStringAsync();
            var authResponse = JsonConvert.DeserializeObject<LoginResponse>(httpResponseContent);
            // Verify token

            // throws error if null
            try
            {
                return await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(authResponse.idToken);
            }
            catch (Exception e)
            {
                throw new UnableToVerifyTokenException(e.Message);
            }
        }

        public async Task UpdatePassword(string email, string password)
        {
            try
            {
                var userRecord = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(email);

                var args = new UserRecordArgs
                {
                    Uid = userRecord.Uid,
                    Email = email,
                    Password = password,
                    EmailVerified = true
                };

                await FirebaseAuth.DefaultInstance.UpdateUserAsync(args);
            }
            catch (Exception exc)
            {
                throw new UpdateFirebaseUserException(exc.Message);
            }
        }

        public async Task DeleteUsers(List<string> uidList)
        {
            foreach (var uid in uidList) await FirebaseAuth.DefaultInstance.DeleteUserAsync(uid);
        }

        public class LoginCredentials
        {
            public string email { get; set; }
            public string password { get; set; }

            public bool returnSecureToken
            {
                get { return true; }
            }
        }

        public class LoginResponse
        {
            public string kind { get; set; }
            public string localId { get; set; }
            public string email { get; set; }
            public string displayName { get; set; }
            public string idToken { get; set; }
            public bool registered { get; set; }
            public string refreshToken { get; set; }
            public string expiresIn { get; set; }
        }
    }

    public class BadUserOrPasswordException : Exception
    {
        public BadUserOrPasswordException(string message) : base(message)
        {
        }
    }

    public class UnableToVerifyTokenException : Exception
    {
        public UnableToVerifyTokenException(string message) : base(message)
        {
        }
    }

    public class UnableToConnectToFirebaseException : Exception
    {
        public UnableToConnectToFirebaseException(string message) : base(message)
        {
        }
    }

    public class UpdateFirebaseUserException : Exception
    {
        public UpdateFirebaseUserException(string message) : base(message)
        {
        }
    }
}