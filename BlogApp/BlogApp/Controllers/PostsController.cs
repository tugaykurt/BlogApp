using BlogApp.Data.Abstract;
using BlogApp.Data.Concrete.EFCore;
using BlogApp.Entity;
using BlogApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Tasks;
using Microsoft.CodeAnalysis.Elfie.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using NuGet.ProjectModel;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BlogApp.Controllers
{
    public class PostsController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly ITagRepository _tagRepository;
        public PostsController(IPostRepository postRepository, ICommentRepository commentRepository, ITagRepository tagRepository)
        {
            _postRepository = postRepository;
            _commentRepository = commentRepository;
            _tagRepository = tagRepository;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string tag)
        {
            var posts = _postRepository.GetAll.Where(p => p.IsActive && p.IsDelete == false);
            if (!string.IsNullOrEmpty(tag))
            {
                posts = posts.Where(x => x.Tags.Any(t => t.Url == tag));
            }
            return View(new PostsViewModel
            {
                Posts = await posts.Include(t => t.Tags).ToListAsync()
            });
        }
        [HttpGet]
        public async Task<IActionResult> Details(string url)

        {
            return View(await _postRepository
                              .GetAll
                              .Include(u => u.User)
                              .Include(x => x.Tags)
                              .Include(c => c.Comments)
                              .ThenInclude(u => u.User)
                              .FirstOrDefaultAsync(p => p.Url == url));
        }

        [HttpPost]
        public async Task<JsonResult> AddComment(int postId, string text)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = User.FindFirstValue(ClaimTypes.GivenName);
            var image = User.FindFirstValue(ClaimTypes.UserData);
            var entity = new Comment
            {
                PostId = postId,
                Text = text,
                PublishedOn = DateTime.Now,
                UserId = int.Parse(userId ?? "")
            };
            await _commentRepository.CreateComment(entity);
            //return Redirect($"/post/{url}");
            //return RedirectToRoute("posts_details", new {Url = url}); // Burada 2 tip route'ta yönlendirme var. Yukarıdakin de Pattern'ı kullandık. Aşağıdakinde de patter'na verdiğimiz ismi kulladık. 2'si de kullanılabilir.

            return Json(new
            {
                userName,
                text,
                entity.PublishedOn,
                image
            });
        }
        [HttpGet]
        [Authorize] // Bu attribute login olmadan posts/create safsasına gitmeyi engelliyor. Default olarak Account/Login route'una yönlendirir. Bu projede biz login sayfasını Users/Login route'una oluşturduğumuzdan dolayı, program.cs'te bizim route'umuzu tanımlalamız gerekli.
        public async Task<IActionResult> Create()
        {
            ViewBag.Tags = await _tagRepository.GetAll.ToListAsync();
            return View();
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(PostCreateViewModel model, IFormFile? image, int[] tagIds)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ModelState.IsValid)
            {
                if (image != null && image.Length > 0)
                {
                    var allowedExtentions = new[] { ".jpg", ".jpeg", ".png" };
                    var extentions = Path.GetExtension(image.FileName.ToString());

                    if (!allowedExtentions.Contains(extentions))
                    {
                        ModelState.AddModelError("Image", "Please select a valid image format.");
                        ViewBag.Tags = await _tagRepository.GetAll.ToListAsync();
                        ViewBag.SelectedTags = tagIds;
                        return View(model);
                    }

                    var orginalName = Path.GetFileName(image.FileName);

                    orginalName = Regex.Replace(orginalName, @"[^a-zA-Z0-9]", "_");

                    var randomFileName = Guid.NewGuid().ToString();

                    var imageName = $"{randomFileName}_{orginalName}{extentions}";

                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", imageName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }
                    model.Image = imageName;
                }
                else
                {
                    model.Image = "default.jpg";
                }
                if (_postRepository.GetAll.Where(u => u.Url == model.Url).Any())
                {
                    ModelState.AddModelError("Url", "This URL has been used.");
                    ViewBag.Tags = await _tagRepository.GetAll.ToListAsync();
                    ViewBag.SelectedTags = tagIds;
                    return View(model);
                }
                var post = new Post
                {
                    Title = model.Title,
                    Description = model.Description,
                    Content = model.Content,
                    Url = model.Url,
                    Image = model.Image,
                    PublishedOn = DateTime.Now,
                    UserId = int.Parse(userId ?? ""),
                    IsDelete = false,
                    IsActive = false
                };
                if(User.FindFirstValue(ClaimTypes.Role) == "admin")
                {
                    post.IsActive = true;
                }

                await _postRepository.CreatePost(post, tagIds);
                return RedirectToAction("Index");
            }
            ViewBag.Tags = await _tagRepository.GetAll.ToListAsync();
            ViewBag.SelectedTags = tagIds;
            return View(model);
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> List()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "");
            var role = User.FindFirstValue(ClaimTypes.Role);
            var posts = _postRepository.GetAll.Where(p => p.IsDelete == false);
            if (string.IsNullOrEmpty(role))
            {
                posts = posts.Where(u => u.UserId == userId);
            }
            return View(await posts.ToListAsync());
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var post = await _postRepository.GetAll.Include(t => t.Tags).FirstOrDefaultAsync(p => p.PostId == id);
            if (post == null)
            {
                return NotFound();
            }
            ViewBag.Tags = await _tagRepository.GetAll.ToListAsync();
            return View(new PostCreateViewModel
            {
                PostId = post.PostId,
                Title = post.Title,
                Description = post.Description,
                Url = post.Url,
                Content = post.Content,
                Image = post.Image,
                IsActive = post.IsActive,
                Tags = post.Tags
            });
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(PostCreateViewModel model, int postId, IFormFile? image, string? oldImage, int[] tagIds)
        {
            if (model.PostId != postId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                if (image != null && image.Length > 0)
                {
                    var allowedExtentions = new[] { ".jpg", ".jpeg", ".png" };
                    var extentiton = Path.GetExtension(image.FileName.ToString());
                    if (!allowedExtentions.Contains(extentiton))
                    {
                        ModelState.AddModelError("Image", "Please select a valid image format.");
                        ViewBag.Tags = await _tagRepository.GetAll.ToListAsync();
                        ViewBag.SelectedTags = tagIds;
                        return View(model);
                    }
                    var orginalName = Path.GetFileName(image.FileName);

                    orginalName = Regex.Replace(orginalName, @"[^a-zA-Z0-9]", "_");

                    var randomName = Guid.NewGuid().ToString();

                    var fileName = randomName + "_" + orginalName + extentiton;

                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName);

                    using (var scream = new FileStream(path, FileMode.Create))
                    {
                        await image.CopyToAsync(scream);
                    }

                    if (oldImage != "default.jpg" && oldImage != null)
                    {
                        var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", oldImage);
                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }

                    model.Image = fileName;
                }
                else
                {
                    model.Image = oldImage;
                }
                var post = await _postRepository.GetAll.FirstOrDefaultAsync(u => u.PostId == model.PostId);
                if (post!.Url != model.Url && _postRepository.GetAll.Where(u => u.Url == model.Url).Any())
                {
                    ModelState.AddModelError("Url", "This URL has been used.");
                    ViewBag.Tags = await _tagRepository.GetAll.ToListAsync();
                    ViewBag.SelectedTags = tagIds;
                    return View(model);
                }

                var postEdit = new Post
                {
                    PostId = model.PostId,
                    Title = model.Title,
                    Description = model.Description,
                    Url = model.Url,
                    Content = model.Content,
                    Image = model.Image,
                };
                if (User.FindFirstValue(ClaimTypes.Role) == "admin")
                {
                    postEdit.IsActive = model.IsActive;
                }

                await _postRepository.EditPost(postEdit, tagIds);
                return RedirectToAction("Index");
            }
            ViewBag.Tags = await _tagRepository.GetAll.ToListAsync();
            ViewBag.SelectedTags = tagIds;
            model.Image = oldImage;
            return View(model);
        }
        [HttpPost]
        [Authorize]
        public async Task<JsonResult> AddTag(string? tagText)
        {
            var pattern = @"^[a-zA-Z0-9!@#$%^&*()_+=\[\]{};':""\\|,.<>\/?-]{3,30}$";

            if (string.IsNullOrEmpty(tagText) || !Regex.IsMatch(tagText, pattern))
            {
                return Json(new
                {
                    success = false,
                    message = "Tags must be between 3 and 30 characters long and contain only English characters!"
                });
            }

            var tag = _tagRepository.GetAll.FirstOrDefault(t => t.Text == tagText);

            if (tag != null)
            {
                return Json(new
                {
                    success = false,
                    message = "This tag already exists."
                });
            }

            var rnd = new Random();

            var entity = new Tag
            {
                Text = tagText,
                Url = tagText,
                Color = (TagColors)rnd.Next(6)
            };
            var createdTag = await _tagRepository.CreateTag(entity);

            return Json(new
            {
                success = true, tagText = tagText, tagId = createdTag.TagId
            });

        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> PostDelete(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var post = await _postRepository.GetAll.FirstOrDefaultAsync(p => p.PostId == id);
            if (post == null) 
            {
                return NotFound();
            }
            return View(new PostDeleteViewModel
            {
                PostId = post.PostId,
                Title = post.Title,
            });
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostDelete(PostDeleteViewModel model)
        {
            if(model == null)
            {
                return NotFound();
            }
            var post = await _postRepository.GetAll.FirstOrDefaultAsync(p => p.PostId == model.PostId);

            if (post == null)
            {
                return NotFound();
            }

            await _postRepository.DeletePost(post);

            return RedirectToAction("List");
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int? commentId)
        {
            if (commentId == null) 
            {
                return Json(new { success = false, message = "Operation failed." });
            }
            var comment = await _commentRepository.GetAll.FirstOrDefaultAsync(c => c.CommentId == commentId);
            if(comment == null)
            {
                return Json(new { success = false, message = "Operation failed." });
            }
            await _commentRepository.DeleteComment(comment);

            return Json(new { success = true });
        }
    }
}
