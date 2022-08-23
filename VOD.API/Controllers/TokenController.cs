﻿using Microsoft.AspNetCore.Mvc;
using VOD.API.Services;
using VOD.Common.DTOModels;

namespace VOD.API.Controllers
{
    [Route("api/token")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        #region Properties
        private readonly ITokenService _tokenService;
        #endregion

        #region Constructor
        public TokenController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }
        #endregion

        #region Actions
        [HttpPost]
        public async Task<ActionResult<TokenDTO>> GenerateTokenAsync(LoginUserDTO loginUserDto)
        {
            try
            {
                var jwt = await _tokenService.GenerateTokenAsync(loginUserDto);
                if (jwt.Token == null) return Unauthorized();
                return jwt;
            }
            catch
            {
                return Unauthorized();
            }
        }
        [HttpGet("{userId}")]
        public async Task<ActionResult<TokenDTO>> GetTokenAsync(string userId, LoginUserDTO loginUserDto)
        {
            try
            {
                var jwt = await _tokenService.GetTokenAsync(loginUserDto, userId);
                if (jwt.Token == null) return Unauthorized();
                return jwt;
            }
            catch
            {
                return Unauthorized();
            }
        }
        #endregion
    }

}
