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
    public class CommentController : Controller
    {
        private readonly ICommentsRepository _commentsRepository;

        public CommentController(ICommentsRepository commentsRepository)
        {
            _commentsRepository = commentsRepository;
        }

        // GET: CommentController
        public ActionResult Index(int postId)
        {
            var viewModel = new PostCommentsViewModel()
            {
                Comments = _commentsRepository.GetByPostId(postId),
                PostId = postId
            };

            return View(viewModel);
        }


        // GET: CommentController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }


        // GET: CommentController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CommentController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CommentController/Delete/5
        public ActionResult Delete(int id)
        {
            var comment = _commentsRepository.GetById(id);
            return View(comment);
        }

        // POST: CommentController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize] // Ensure the user is logged in
        public ActionResult Delete(int id, Comment comment)
        {
            try
            {
                Comment commentToDelete = _commentsRepository.GetById(id); // Fetch the comment from the database

                var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)); // Get the current logged in user's id

                if (commentToDelete.UserProfileId != currentUserId)
                {
                    // If the current user is not the author of the comment, return Unauthorized
                    return Unauthorized();
                }

                _commentsRepository.Delete(id);

                // Redirects the user back to the post
                return RedirectToAction("ForPost", "Comment", new { postId = commentToDelete.PostId });
            }
            catch
            {
                return View(comment);
            }
        }


        // For Post Details Page
        // This is so we can access the comments for a specific post
        public IActionResult ForPost(int postId)
        {
            var viewModel = new PostCommentsViewModel()
            {
                Comments = _commentsRepository.GetByPostId(postId),
                PostId = postId
            };

            return View(viewModel);
        }


        // This ensures that we have the current user's profile id when creating a comment
        private int GetCurrentUserProfileId()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(id);
        }

        [HttpGet]
        [Authorize]
        public ActionResult Create(int postId)
        {
            Comment comment = new Comment { PostId = postId };
            return View(comment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create(Comment comment)
        {
            try
            {
                // Retrieve the current user's profile ID
                var userProfileId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                // Set the UserProfileId property of the comment
                comment.UserProfileId = userProfileId;

                comment.CreateDateTime = DateTime.Now;
                _commentsRepository.Add(comment);

                // Redirect to the "ForPost" action with the appropriate postId
                return RedirectToAction("ForPost", new { postId = comment.PostId });
            }
            catch
            {
                // If something goes wrong, redirect back to the form
                return View(comment);
            }
        }




    }
}
