using TabloidMVC.Models;
using TabloidMVC.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TabloidMVC.Controllers
{
    public class UserProfileController : Controller
    {
        private readonly IUserProfileRepository _userProfileRepo;

        public UserProfileController(IUserProfileRepository userProfileRepository)
        {
            _userProfileRepo = userProfileRepository;
        }
        // GET: UserProfileController
        public ActionResult Index()
        {
            List<UserProfile> profiles = _userProfileRepo.GetAllUserProfiles();

            return View(profiles);
        }

        // GET: UserProfileController/Details/5
        public ActionResult Details(int id)
        {
            UserProfile userprofile = _userProfileRepo.GetProfileById(id);

            if (userprofile == null)
            {
                return NotFound();
            }
            else
            {
            return View(userprofile);

            }
        }

        // GET: UserProfileController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserProfileController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserProfile userProfile)
        {
            try
            {
                _userProfileRepo.RegisterUser(userProfile);

                return Redirect("/");
            }
            catch (Exception ex)
            {
                return View(userProfile);
            }
        }

        // GET: UserProfileController/Edit/5
        public ActionResult Edit(int id)
        {
            UserProfile userProfile = _userProfileRepo.GetProfileById(id);

            return View(userProfile);
        }

        // POST: UserProfileController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, UserProfile userProfile)
        {
            try
            {
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View(userProfile);
            }
        }

        // GET: UserProfileController/Delete/5
        public ActionResult Delete(int id)
        {
            UserProfile userProfile = _userProfileRepo.GetProfileById(id);

            if (userProfile == null)
            {
                return NotFound();
            }

            return View(userProfile);
        }

        // POST: UserProfileController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, UserProfile userProfile)
        {
            try
            {
                _userProfileRepo.DeactivateUser(userProfile);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View(userProfile);
            }
        }
    }
}
