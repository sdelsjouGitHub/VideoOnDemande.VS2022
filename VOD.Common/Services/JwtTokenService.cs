using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using VOD.Common.DTOModels;
using VOD.Common.Entities;
using VOD.Common.Extensions;

namespace VOD.Common.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        #region Properties
        private readonly IHttpClientFactoryService _http;
        private readonly UserManager<VODUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion

        #region Constructor
        public JwtTokenService(IHttpClientFactoryService http, UserManager<VODUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _http = http;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion

        #region Token Methods
#pragma warning disable CS8613 // Die NULL-Zulässigkeit von Verweistypen im Rückgabetyp entspricht nicht dem implizit implementierten Member.
        public async Task<TokenDTO?> CreateTokenAsync()
#pragma warning restore CS8613 // Die NULL-Zulässigkeit von Verweistypen im Rückgabetyp entspricht nicht dem implizit implementierten Member.
        {
            try
            {
#pragma warning disable CS8602 // Dereferenzierung eines möglichen Nullverweises.
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
#pragma warning restore CS8602 // Dereferenzierung eines möglichen Nullverweises.
                var user = await _userManager.FindByIdAsync(userId);
                var tokenUser = new LoginUserDTO
                {
                    Email = user.Email,
                    Password = "",
                    PasswordHash = user.PasswordHash
                };
                var token = await _http.CreateTokenAsync(tokenUser, "api/token", "AdminClient");

                return token;
            }
            catch
            {
                return default;
            }
        }
#pragma warning disable CS8613 // Die NULL-Zulässigkeit von Verweistypen im Rückgabetyp entspricht nicht dem implizit implementierten Member.
        public async Task<TokenDTO?> GetTokenAsync()
#pragma warning restore CS8613 // Die NULL-Zulässigkeit von Verweistypen im Rückgabetyp entspricht nicht dem implizit implementierten Member.
        {
            try
            {
#pragma warning disable CS8602 // Dereferenzierung eines möglichen Nullverweises.
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
#pragma warning restore CS8602 // Dereferenzierung eines möglichen Nullverweises.
                var user = await _userManager.FindByIdAsync(userId);
                var claims = await _userManager.GetClaimsAsync(user);
                var token = claims.Single(c => c.Type.Equals("Token")).Value;
                var date = claims.Single(c => c.Type.Equals("TokenExpires")).Value;
                DateTime expires;
                var succeeded = DateTime.TryParse(date, out expires);

                // Return token from the user object
                if (succeeded && !token.IsNullOrEmptyOrWhiteSpace()) return new TokenDTO(token, expires);

                // Return token from the API
                var tokenUser = new LoginUserDTO
                {
                    Email = user.Email,
                    Password = "",
                    PasswordHash = user.PasswordHash
                };
                var newToken = await _http.GetTokenAsync(tokenUser, $"api/token/{user.Id}", "AdminClient");
                return newToken;
            }
            catch
            {
                return default;
            }
        }
#pragma warning disable CS8613 // Die NULL-Zulässigkeit von Verweistypen im Rückgabetyp entspricht nicht dem implizit implementierten Member.
        public async Task<TokenDTO?> CheckTokenAsync(TokenDTO token)
#pragma warning restore CS8613 // Die NULL-Zulässigkeit von Verweistypen im Rückgabetyp entspricht nicht dem implizit implementierten Member.
        {
            try
            {
                if (token.TokenHasExpired)
                {
#pragma warning disable CS8600 // Das NULL-Literal oder ein möglicher NULL-Wert wird in einen Non-Nullable-Typ konvertiert.
                    token = await GetTokenAsync();
#pragma warning restore CS8600 // Das NULL-Literal oder ein möglicher NULL-Wert wird in einen Non-Nullable-Typ konvertiert.
#pragma warning disable CS8602 // Dereferenzierung eines möglichen Nullverweises.
#pragma warning disable CS8600 // Das NULL-Literal oder ein möglicher NULL-Wert wird in einen Non-Nullable-Typ konvertiert.
                    if (token.TokenHasExpired) token = await CreateTokenAsync();
#pragma warning restore CS8600 // Das NULL-Literal oder ein möglicher NULL-Wert wird in einen Non-Nullable-Typ konvertiert.
#pragma warning restore CS8602 // Dereferenzierung eines möglichen Nullverweises.
                }

#pragma warning disable CS8603 // Mögliche Nullverweisrückgabe.
                return token;
#pragma warning restore CS8603 // Mögliche Nullverweisrückgabe.
            }
            catch
            {
                return default;
            }
        }
        #endregion
    }
}
