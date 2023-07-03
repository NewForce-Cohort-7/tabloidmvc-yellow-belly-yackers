using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TabloidMVC.Models;
using TabloidMVC.Repositories;
using System.Security.Claims;

namespace TabloidMVC.Controllers
{
    public class SubscriptionController : Controller
    {

        private readonly ISubscriptionRepository _subscriptionRepo;
        private readonly IUserProfileRepository _userRepo;
        public SubscriptionController(ISubscriptionRepository subscriptionRepository, IUserProfileRepository userRepo)
        {
            _subscriptionRepo = subscriptionRepository;
            _userRepo = userRepo;

        }

        // GET: SubscriptionController
        public ActionResult Index()
        {
            return View();
        }

        // GET: SubscriptionController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: SubscriptionController/Create
        public ActionResult Create(int postId, int providerId)
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            Subscription subscription = new Subscription()
            {
                SubscriberUserProfileId = currentUserId,
                ProviderUserProfileId = providerId,
                BeginDateTime = DateTime.Now,
                EndDateTime = null,
                Provider = _userRepo.GetProfileById(providerId),
                PostId = postId   
            };

            int? AlreadySubbedID = _subscriptionRepo.AlreadySubbedId(subscription.SubscriberUserProfileId, subscription.ProviderUserProfileId);

            //check if sub already exists
            if (AlreadySubbedID != null)
            {
                //if does, redirect to AlreadySubbed view
                return RedirectToAction("AlreadySubbed", "Subscription", new { postId = postId, alreadySubbedId = AlreadySubbedID });
            }
       
            //if not, display create confirm page
            return View(subscription);
        }

        // POST: SubscriptionController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Subscription subscription)
        {
            try
            {
                _subscriptionRepo.Add(subscription);

                //redirect back to post if successfully subscribed
                return RedirectToAction("Details", "Post", new { id = subscription.PostId });
            }
            catch (Exception ex)
            {
                return View(subscription);
            }
        }

        // GET: SubscriptionController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: SubscriptionController/Edit/5
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

        // GET: SubscriptionController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: SubscriptionController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
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

        // GET: SubscriptionController/AlreadySubbed
        public ActionResult AlreadySubbed(int postId, int alreadySubbedId)
        {
            Subscription existingSub = _subscriptionRepo.GetById(alreadySubbedId);
            existingSub.Provider = _userRepo.GetProfileById(existingSub.ProviderUserProfileId);
            existingSub.PostId = postId;

            return View(existingSub);
        }

    }
}
