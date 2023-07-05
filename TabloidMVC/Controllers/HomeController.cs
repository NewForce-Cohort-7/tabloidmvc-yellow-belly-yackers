using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TabloidMVC.Models;
using System.Security.Claims;
using TabloidMVC.Repositories;
using TabloidMVC.Models.ViewModels;
using Microsoft.AspNetCore.SignalR;

namespace TabloidMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPostRepository _postRepo;
        private readonly ISubscriptionRepository _subRepo;

        public HomeController(ILogger<HomeController> logger, IPostRepository postRepository, ISubscriptionRepository subscriptionRepository)
        {
            _logger = logger;
            _postRepo = postRepository;
            _subRepo = subscriptionRepository;
        }

        public IActionResult Index()
        {
            //if user is not authenticated, just return blank view
            if(!User.Identity.IsAuthenticated){

                return View();  
            }

            //otherwise, grab user's id, grab their subscriptions, and get the posts for each subscription
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var viewModel = new HomeViewModel()
            {
                Subs = _subRepo.GetAllSubscribersSubs(currentUserId),
                Posts = new List<Post>()
            };

            foreach (var sub in viewModel.Subs)
            {
                List<Post> postsToAdd = _postRepo.GetAllPostsByUser(sub.ProviderUserProfileId);
                foreach(var post in postsToAdd) 
                {
                    viewModel.Posts.Add(post);
                }
            }

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}