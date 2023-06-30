using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using System.Security.Claims;
using TabloidMVC.Models;
using TabloidMVC.Models.ViewModels;
using TabloidMVC.Repositories;

namespace TabloidMVC.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly ICategoryRepository _categoryRepository;

        public PostController(IPostRepository postRepository, ICategoryRepository categoryRepository)
        {
            _postRepository = postRepository;
            _categoryRepository = categoryRepository;
        }

        public IActionResult Index()
        {
            var posts = _postRepository.GetAllPublishedPosts();
            return View(posts);
        }

        [Authorize]
        public IActionResult MyPosts()
        {
            int authorId = GetCurrentUserProfileId();
            var posts = _postRepository.GetAllPostsByUser(authorId);
            return View(posts);
        }

        public IActionResult Details(int id)
        {
            var post = _postRepository.GetPublishedPostById(id);
            if (post == null)
            {
                int userId = GetCurrentUserProfileId();
                post = _postRepository.GetUserPostById(id, userId);
                if (post == null)
                {
                    return NotFound();
                }
            }
            return View(post);
        }

        public IActionResult Create()
        {
            var vm = new PostCreateViewModel();
            vm.CategoryOptions = _categoryRepository.GetAll();
            return View(vm);
        }

        [HttpPost]
        public IActionResult Create(PostCreateViewModel vm)
        {
            try
            {
                vm.Post.CreateDateTime = DateAndTime.Now;
                vm.Post.IsApproved = true;
                vm.Post.UserProfileId = GetCurrentUserProfileId();

                _postRepository.Add(vm.Post);

                return RedirectToAction("Details", new { id = vm.Post.Id });
            }
            catch
            {
                vm.CategoryOptions = _categoryRepository.GetAll();
                return View(vm);
            }
        }

        private int GetCurrentUserProfileId()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(id);
        }

        [Authorize]
        public ActionResult Edit(int id)
        {
            Post post = _postRepository.GetPublishedPostById(id);

            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Edit(Post updatedPost)
        {
            if (ModelState.IsValid)
            {
               
                _postRepository.Update(updatedPost);

                return RedirectToAction("Index"); 
            }

            return View(updatedPost);
        }

        [Authorize]
        public ActionResult Delete(int id)
        {
            Post post = _postRepository.GetPublishedPostById(id);

            if (post == null || (post.UserProfileId != int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)) && !User.IsInRole("Admin")))
            {
                return NotFound();
            }

            return View(post);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Post post)
        {
            try
            {
                _postRepository.Delete(post);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(post);
            }

        }
    }
}