using BlogPost.Data;
using BlogPost.DTOs;
using BlogPost.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace BlogPost.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlogPostController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly ILogger<BlogPostController> _logger;

        public BlogPostController(AppDbContext context, IMemoryCache cache, ILogger<BlogPostController> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBlogPost(Guid id)
        {
            string cacheKey = $"BlogPost-{id}";
            if (!_cache.TryGetValue(cacheKey, out BlogPostDto blogPost))
            {
              var _blogPost = await _context.BlogPosts.FindAsync(id);

                if (_blogPost == null)
                {
                    return NotFound();
                }

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));

                _cache.Set(cacheKey, blogPost, cacheEntryOptions);
            }

            return Ok(blogPost);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBlogPost(BlogPostDto blogPost)
        {
            var _blogPost = new BlogPostModel
            { 
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Content = blogPost.Content,
            Title = blogPost.Title,
            };
                     
            _context.BlogPosts.Add(_blogPost);
            await _context.SaveChangesAsync();

            return Ok(_blogPost);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBlogPost(Guid id, BlogPostDto blogPost)
        {
         

            var _blogPost = await _context.BlogPosts.FindAsync(id);
            if (_blogPost == null)
            {
                return NotFound();
            }

            _blogPost.Title = blogPost.Title;
            _blogPost.Content = blogPost.Content;
            _blogPost.UpdatedAt = DateTime.UtcNow;

            _context.Entry(_blogPost).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            _cache.Remove($"BlogPost-{id}");

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlogPost(Guid id)
        {
            var blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost == null)
            {
                return NotFound();
            }

            _context.BlogPosts.Remove(blogPost);
            await _context.SaveChangesAsync();

            _cache.Remove($"BlogPost-{id}");

            return NoContent();
        }
    }

}
