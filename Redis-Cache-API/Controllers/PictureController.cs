using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Redis_Cache_API.Data;
using Redis_Cache_API.Cache_Infrastructure;

namespace Redis_Cache_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PictureController : ControllerBase
{
    private readonly ICacheService _cacheService;
    private static readonly object _lock = new ();

    public PictureController(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    //return a list of pictures
    [HttpGet]
    public IActionResult Index()
    {
        PictureCacheKey pictureCacheKey = new (new[] { "0" });

        var cacheData = _cacheService.GetData<IEnumerable<Picture>>(pictureCacheKey.ToString());
        if (cacheData != null)
        {
            return Ok(cacheData);
        }

        lock (_lock) // lock applied, so only 1 request at a time may be permitted to execute
        {
            var expirationTime = DateTimeOffset.Now.AddMinutes(5.0);

            using var context = new DataContext();
            cacheData = context.Picture.ToList();
            _cacheService.SetData<IEnumerable<Picture>>(pictureCacheKey.ToString(), cacheData, expirationTime);
        }

        return Ok(cacheData);
    }

    [HttpGet("{id}")]
    public IActionResult GetSinglePictureById(int id)
    {
        #region Validation
        if (id == 0)
            return BadRequest(new { message = "Id must be greater than 0" });
        #endregion

        PictureCacheKey pictureCacheKey = new (new[] { id.ToString() });

        //first lets check if we have the picture in our cache db
        var cacheData = _cacheService.GetData<IEnumerable<Picture>>(pictureCacheKey.ToString());
        if (cacheData != null)
        {
            return Ok(cacheData);
        }

        //if picture is not in the cache db, then fetch picture from database. 
        //cache the picture for future rquests
        lock (_lock)
        {
            var expirationTime = DateTimeOffset.Now.AddMinutes(5.0);

            //save picture to database
            using var context = new DataContext();
            cacheData = (IEnumerable<Picture>?)context.Picture.Find(id);
            _cacheService.SetData<IEnumerable<Picture>>(pictureCacheKey.ToString(), cacheData!, expirationTime);
        }

        return Ok(cacheData);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePicture(int id)
    {
        #region Validation
        if (id == 0)
            return BadRequest(new { message = "Id must be greater than 0" });
        #endregion

        int complete = 0;

        //save picture to database
        using (var context = new DataContext())
        {
            Picture pic = await context.Picture.FindAsync(id);

            context.Picture.Remove(pic);
            complete = await context.SaveChangesAsync();
        }

        //remove the picture form cache as well
        PictureCacheKey pictureCacheKey = new (new[] { id.ToString() });
        _cacheService.RemoveData(pictureCacheKey.ToString());


        return Ok(complete);
    }

    [HttpPost()]
    public async Task<IActionResult> CreatePicture([FromBody] Picture model)
    {
        #region Validation
        if (!ModelState.IsValid)
            return BadRequest(new { message = "model is invalid" });
        #endregion

        int complete = 0;

        using (var context = new DataContext())
        {

            var pic = new Picture()
            {
                Url = model.Url,
                Name = model.Name,
                AltText = model.AltText
            };

            await context.Picture.AddAsync(pic);
            complete = await context.SaveChangesAsync();
        }

        return Ok(complete);
    }

    [HttpPut()]
    public async Task<IActionResult> UpdatePicture([FromBody] Picture model)
    {
        #region Validation
        if (!ModelState.IsValid)
            return BadRequest(new { message = "model is invalid" });
        #endregion

        int complete = 0;

        //save picture to database
        using (var context = new DataContext())
        {
            Picture pic = await context.Picture.FindAsync(model.PictureId);

            pic.AltText = model.AltText;
            pic.Url = model.Url;
            pic.Name = model.Name;

            context.Picture.Update(pic);
            complete = await context.SaveChangesAsync();
        }

        return Ok(complete);
    }


}


