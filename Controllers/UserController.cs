using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using sampleapi.Models;
using sampleapi.Services;
using System.Text.Json;

namespace sampleapi.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        
        private readonly ILogger<UserController> _logger;
        private readonly IMailService _mailService;
        private readonly IMapper _mapper; 
        private readonly ISampleApiRepository _sampleApiRepository;
        const int maxUsersPageSize = 20;

        public UserController(ILogger<UserController> logger, 
                              IMailService mailService,
                              IMapper mapper,
                              ISampleApiRepository sampleApiRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _sampleApiRepository = sampleApiRepository ?? throw new ArgumentNullException(nameof(sampleApiRepository));
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers([FromQuery] string? username, 
            string? searchQuery, int pageNumber  = 1, int pageSize = 10)
        {
            if (pageSize > maxUsersPageSize) 
            { 
                pageSize = maxUsersPageSize;
            }
            
            var (userEntities, paginationMetadata) = await _sampleApiRepository
                .GetUsersAsync(username, searchQuery, pageNumber, pageSize);

            Response.Headers.Append("X-Pagination", 
                JsonSerializer.Serialize(paginationMetadata));

            return Ok(_mapper.Map<IEnumerable<Models.User>>(userEntities));
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUser(int userId)
        {
            try
            {
                var userEntity = await _sampleApiRepository.GetUserAsync(userId);                

                if (userEntity is null)
                {

                    _logger.LogInformation($"User with id {userId} whasn't found.");
                    return NotFound();
                }

                return Ok(_mapper.Map<Models.User>(userEntity));
            }
            catch (Exception ex)
            {

                _logger.LogCritical($"Exception while getting user with id {userId}", ex);
                return StatusCode(500, "A problem happened while handling your request.");
            }            
        }

        [HttpPost(Name = "GetUser")]
        public async Task<ActionResult<User>> CreateUser([FromBody] UserCreationDto user)
        {

            var userToCreate = _mapper.Map<Entities.User>(user);
            await _sampleApiRepository.AddUserAsync(userToCreate);
            await _sampleApiRepository.SaveChangesAsync();

            var createdUserToReturn = _mapper.Map<User>(userToCreate);

            return CreatedAtRoute("GetUser",
                new
                {
                    userId = userToCreate.Id
                },
                createdUserToReturn);
        }

        [HttpPut("{userId}")]
        public async Task<ActionResult> UpdateUser(int userId, [FromBody] UserUpdateDto user)
        {

            var userEntity = await _sampleApiRepository.GetUserAsync(userId);
            
            if (userEntity is null)
            {
                return NotFound();
            }

            _mapper.Map(user, userEntity);

            await _sampleApiRepository.SaveChangesAsync(); 

            return NoContent();
        }

        [HttpPatch("{userId}")]
        public async Task<ActionResult> PartiallyUpdateUser(int userId, JsonPatchDocument<UserUpdateDto> patchDocument)
        {
            var userEntity = await _sampleApiRepository.GetUserAsync(userId);
            if (userEntity is null)
            {
                return NotFound();
            }

            var userToPath = _mapper.Map<UserUpdateDto>(userEntity);

            patchDocument.ApplyTo(userToPath, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(userToPath))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(userToPath, userEntity);
            await _sampleApiRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{userId}")]
        public async Task<ActionResult> DeleteUser(int userId)
        {
            var userEntity = await _sampleApiRepository.GetUserAsync(userId);
            if (userEntity is null)
            {
                return NotFound();
            }

            _sampleApiRepository.DeleteUser(userEntity);
            await _sampleApiRepository.SaveChangesAsync();


            _mailService.Send("User deleted", $"User {userEntity.UserName} with id {userEntity.Id} was deleted");

            return NoContent();
        }
    }
}
