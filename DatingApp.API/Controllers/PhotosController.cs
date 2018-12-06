using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Controllers
{
    [Route("api/users/{userId}/photos")]
    [ApiController]
    [Authorize]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        // IOptions class is used for accessing services defined in startup.cs
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private readonly Cloudinary _cloudinary;

        public PhotosController(IDatingRepository repo, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _cloudinaryConfig = cloudinaryConfig;
            _mapper = mapper;
            _repo = repo;

            // setup for cloudinary account
            Account acc = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );

            // for using the Cloudinary library
            _cloudinary = new Cloudinary(acc);

        }

        // to retrive individual photo
        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id) {

            var photoFromRepo = await _repo.GetPhoto(id);

            //  to avoid including navigation properties effects in photos 
            // use photoForReturnedDto and map photoFromRepo over it
            var photo = _mapper.Map<PhotoForReturnedDto>(photoFromRepo);

            return Ok(photo);
        }


        [HttpPost] // no route params b/c route defined above the class
        public async Task<IActionResult> AddPhotoForUser(int userId /* comes from route*/, 
            [FromForm]PhotoForCreationDto photoForCreationDto /* take photos data from user profile */) {

            // check if request is sent by current(tokanized) user
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await _repo.GetUser(userId);

            // get file (photo) from dto
            var file = photoForCreationDto.File;

            // store upload response from Cloudinary
            var uploadResult = new ImageUploadResult ();

            if(file.Length > 0) {
                // load file into memory (OpenReadStream)
                // dispose (using) memory upon completing task 
                using (var stream = file.OpenReadStream()) {
                    // specify data to upload
                    var uploadParams = new ImageUploadParams() {
                        // specify file
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation()
                        .Width(500).Height(500).Crop("fill").Gravity("face")
                    };

                    // upload data to cloudinary
                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }

            // now store necessary data into database
            photoForCreationDto.Url = uploadResult.Uri.ToString();
            photoForCreationDto.publicId = uploadResult.PublicId;

            // map photoForCreationDto to photo model
            var photo = _mapper.Map<Photo>(photoForCreationDto);

            // check if this photo is first one
            if (!userFromRepo.Photos.Any(u => u.IsMain))
                photo.IsMain = true;
            // add() is generic function defined in dating repository
            // must call saveAll() after calling generic functions
            userFromRepo.Photos.Add(photo);
            if (await _repo.SaveAll()) {
                // return CreatedAtRoute response...it requires individual photo's information
                // so we have to implement [httpGet] as above, request it and send response back to client
                var photoToReturn = _mapper.Map<PhotoForReturnedDto>(photo);
                return CreatedAtRoute("GetPhoto", new {id = photo.Id}, photoToReturn);
            }

            return BadRequest("could not add photo");
        }

        /*request route for updating isMain photo property*/
        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> setMainPhoto(int userId, int id) {
            /* authenticate user */
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            /* check if sent photo id matches to one in db */
            var user = await _repo.GetUser(userId);
            if(!user.Photos.Any(p => p.Id == id))
                return Unauthorized();
            
            var photoFromRepo = await _repo.GetPhoto(id);
            /* check if photo is already main photo */
            if(photoFromRepo.IsMain == true)
                return BadRequest("this is already the main photo");

            var currentMainPhoto = await _repo.GetMainPhotoForUser(userId);
            currentMainPhoto.IsMain = false;

            photoFromRepo.IsMain = true;
            if (await _repo.SaveAll())
                return NoContent();
            
            return BadRequest("Could not set photo to main");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto (int userId, int id) {
            /* authenticate user */
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            /* check if sent photo id matches to one in db */
            var user = await _repo.GetUser(userId);
            if(!user.Photos.Any(p => p.Id == id))
                return Unauthorized();
            
            var photoFromRepo = await _repo.GetPhoto(id);
            /* check if photo is already main photo */
            if(photoFromRepo.IsMain == true)
                return BadRequest("You cannot delete your main photo");
            
            // we have to delete image and its url from cloudinary and db respectively
            // images may have null public Id so we put a check
            if (photoFromRepo.PublicId != null) {
                var deleteParams = new DeletionParams(photoFromRepo.PublicId);
                var result = _cloudinary.Destroy(deleteParams);
                if(result.Result == "ok") {
                    _repo.Delete(photoFromRepo);
                }
                if (await _repo.SaveAll())
                    return Ok();
            }

            if (photoFromRepo.PublicId == null) {
                _repo.Delete(photoFromRepo);
                if (await _repo.SaveAll())
                    return Ok();
            }
            
            return BadRequest("Failed to delete photo");

        }
    }
}