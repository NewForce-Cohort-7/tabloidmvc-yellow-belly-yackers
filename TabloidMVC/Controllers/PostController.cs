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
        private readonly ITagRepository _tagRepository;

        public PostController(IPostRepository postRepository, ICategoryRepository categoryRepository, ITagRepository tagRepository)
        {
            _postRepository = postRepository;
            _categoryRepository = categoryRepository;
            _tagRepository = tagRepository;
        }

        public IActionResult Index()
        {
            var posts = _postRepository.GetAllPublishedPosts();
            var tags = _tagRepository.GetAll();
            return View(posts);
        }

        [Authorize]
        public IActionResult MyPosts()
        {
            // get current user's ID
            int authorId = GetCurrentUserProfileId();
            // get all posts by user
            var posts = _postRepository.GetAllPostsByUser(authorId);
            return View(posts);
        }

        [Authorize]
        public IActionResult Details(int id)
        {
            // get published post with ID
            var post = _postRepository.GetPublishedPostById(id);
            int userId = GetCurrentUserProfileId();

            if (post == null)
            {
                post = _postRepository.GetUserPostById(id, userId);
                if (post == null)
                {
                    return NotFound();
                }
            }
;           // get selected tags for the post
            var selectedTags = _tagRepository.GetTagsOnPost(id);
            
            // view model to hold the post details and tags
            PostDetailsViewModel vm = new PostDetailsViewModel()
            {
                Post = post,
                Tags = selectedTags
            };


            return View(vm);
        }
        [Authorize]
        public IActionResult CreateTags(int id)
        {
            var post = _postRepository.GetPublishedPostById(id);
            if (post == null)
            {
                int userId = GetCurrentUserProfileId();
                post = _postRepository.GetUserPostById(id, userId);

            }
            var selectedTags = _tagRepository.GetTagsByPostId(id);

            PostTagViewModel tagViewModel = new PostTagViewModel()
            {
                Post = post,
                Tags = _tagRepository.GetAll(),

                PostTag = new PostTag()
                {
                    PostId = post.Id,
                    TagIds = selectedTags
                }
            };


            return View(tagViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateTags(PostTag postTag, int id)
        {
            try
            {
                // delete existing post tags for the post with the given ID
                _postRepository.DeletePostTagsOnPost(id);
                // set the post ID for the new post tag
                postTag.PostId = id;
                _postRepository.AddPostTag(postTag);
                return RedirectToAction("Details", new { id = postTag.PostId });
            }
            catch
            {

                return View(postTag);
            }
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
            int userId = GetCurrentUserProfileId();
            Post post = _postRepository.GetPublishedPostById(id);

            if (post == null)
            {
                post = _postRepository.GetUserPostById(id, userId);
                if (post == null)
                {
                    return NotFound();
                }
            }
            if (!User.IsInRole("Admin") && post.UserProfileId != int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)))
            {
                return NotFound();
            }

            var vm = new PostEditViewModel();
            vm.CategoryOptions = _categoryRepository.GetAll();
            vm.Post = post;
            return View(vm);

        }




        [Authorize]
        [HttpPost]
        public ActionResult Edit(int id, PostEditViewModel vm)
        {
            try
            {
                _postRepository.Update(vm.Post);
                return RedirectToAction("Index");
            }
            catch
            {
                return View(vm.Post);
            }
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
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Post post)
        {
            try
            {
                _tagRepository.DeletePostTagsByPost(id);
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