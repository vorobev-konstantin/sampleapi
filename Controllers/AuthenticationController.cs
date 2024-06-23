using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using sampleapi.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace sampleapi.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {

        private readonly ISampleApiRepository _sampleApiRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AuthenticationController(ISampleApiRepository sampleApiRepository, 
                IMapper mapper, IConfiguration configuration)
        {
            _sampleApiRepository = sampleApiRepository ?? throw new ArgumentNullException(nameof(sampleApiRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }


        public class AuthenticationRequestBody
        {
            public string? UserName { get; set; }
            public string? Password { get; set; }
        }
        
        [HttpPost("authenticate")]
        public async Task<ActionResult<string>> Authenticate(AuthenticationRequestBody authenticationRequestBody)
        {
            
            // Step 1: validate the username/password
            var user = await ValidateUserCredentialsAsync(
                authenticationRequestBody.UserName,
                authenticationRequestBody.Password);

            if (user is null) 
            {
                return Unauthorized();
            }

            // Step 2: create a jwt token
            var securityKey = new SymmetricSecurityKey(
                Convert.FromBase64String(_configuration["Authentication:SecretForKey"]));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claimsForTokens = new List<Claim>
            {
                new Claim("sub", user.Id.ToString()),
                new Claim("given_name", user.FirstName),
                new Claim("family_name", user.LastName),
                new Claim("country", user.Country ?? string.Empty)
            };

            var jwtSecurityToken = new JwtSecurityToken(
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claimsForTokens,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                signingCredentials);

            var tokenToReturn = new JwtSecurityTokenHandler()
                .WriteToken(jwtSecurityToken);

            return Ok(tokenToReturn);
        }

        private async Task<Models.User?> ValidateUserCredentialsAsync(string? userName, string? password)
        {
            var(userEntities, paginationMetadata) = await _sampleApiRepository
                .GetUsersAsync(userName, string.Empty, 1, 1);
            // TODO check validate password as hash from database
            if (userEntities.Any())
            {
                return _mapper.Map<Models.User>(userEntities.FirstOrDefault());
            }
            return null;
        }
    }
}
