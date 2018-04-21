using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ActivityCenter.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace ActivityCenter.Controllers
{
    public class ActivityController : Controller
    {
        private DojoActivityContext _context;

        public ActivityController(DojoActivityContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("Dashboard")]
        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetInt32("CurrentUserID") != null)
            {
                ViewBag.Loggedin = (int)HttpContext.Session.GetInt32("CurrentUserID");
            }
            if( TempData["Error"] != null){
                ViewBag.Error = TempData["Error"];
            }

            var CurUserName = _context.Users
                                    .Where(e => e.UserID == (int)HttpContext.Session.GetInt32("CurrentUserID"))
                                    .SingleOrDefault()
                                    .FirstName;
            ViewBag.CurUserName = CurUserName;

            var allActivity = _context.Activities
                                .Include(p => p.Creator)
                                .Include(t => t.Participants)
                                    .ThenInclude(e => e.User)
                                .OrderByDescending(r => r.ActivityDate)
                                .Where(w => w.ActivityDate >= DateTime.Now)
                                .ToList();
            return View("ActivityHome", allActivity);
        }

        [HttpPost]
        [Route("processRsvp")]
        public IActionResult RSVP(string rsvpAction, int activityID)
        {
            if (!CheckLoggedIn())
            {
                return RedirectToAction("Index", "Home");
            }

            int? CurrentUser = HttpContext.Session.GetInt32("CurrentUserID");
            if (rsvpAction == "Join")
            {
                var alreadyParticipate = _context.ActivityParticipants
                                                    .Include(p => p.DojoActivity)
                                                    .Include(e => e.User)
                                                        .ThenInclude(r => r.JoinedActivities).ToList();
                var time = _context.Activities.Where(e => e.DojoActivityID == activityID).SingleOrDefault().ActivityTime;
                TimeSpan durTime;
                foreach (var item in alreadyParticipate)
                {
                    durTime = TimeSpan.FromHours(item.DojoActivity.Duration);
                    if (item.DojoActivity.ActivityTime <= time && item.DojoActivity.ActivityTime + durTime >= time)
                    {
                        TempData["Error"] = "You already joined that about same time as this. Check your schedule!";
                        return RedirectToAction("Dashboard");
                    }
                    else
                    {
                        ActivityParticipant newPerson = new ActivityParticipant
                        {
                            UserID = (int)CurrentUser,
                            DojoActivityID = activityID
                        };
                        _context.ActivityParticipants.Add(newPerson);
                    }
                }
            }
            else if (rsvpAction == "Leave")
            {
                var undo = _context.ActivityParticipants
                                        .Where(e => e.UserID == (int)CurrentUser)
                                        .Where(e => e.DojoActivityID == activityID)
                                        .SingleOrDefault();
                _context.ActivityParticipants.Remove(undo);
            }
            else if (rsvpAction == "Delete")
            {
                var deleteActivity = _context.Activities
                                    .Where(e => e.DojoActivityID == activityID)
                                    .SingleOrDefault();
                _context.Activities.Remove(deleteActivity);
            }

            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        [Route("New")]
        public IActionResult New()
        {
            if (!CheckLoggedIn())
            {
                return RedirectToAction("Index", "Home");
            }
            return View("New");

        }

        [HttpPost]
        [Route("AddNew")]
        public IActionResult AddNew(ActivityViewModel model, string unit)
        {
            if (!CheckLoggedIn())
            {
                return RedirectToAction("Index", "Home");
            }
            if (ModelState.IsValid)
            {
                DojoActivity NewActivity = new DojoActivity
                {
                    Title = model.Title,
                    ActivityTime = model.ActivityTime,
                    ActivityDate = model.ActivityDate,
                    Duration = model.Duration,
                    Description = model.Description,
                    CreatorID = (int)HttpContext.Session.GetInt32("CurrentUserID"),
                    HourOrMin = unit
                };

                _context.Activities.Add(NewActivity);
                _context.SaveChanges();
                int activityID = _context.Activities
                                        .Where(e => e.Title == model.Title)
                                        .SingleOrDefault().DojoActivityID;

                return RedirectToAction("ShowActivity", new { ID = activityID });
            }
            ViewBag.Error = "Something went wrong..";
            return View("New");
        }

        [HttpGet]
        [Route("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        [Route("ShowActivity/{ID}")]
        public IActionResult ShowActivity(int ID)
        {
            var activity = _context.Activities.Where(e => e.DojoActivityID == ID)
                                            .Include(p => p.Creator)
                                            .Include(r => r.Participants)
                                                .ThenInclude(t => t.User).ToList();
            ViewBag.Activity = activity;
            return View("Activity", activity);

        }

        [HttpPost]
        [Route("Join")]
        public IActionResult Join(int activityID)
        {
            if (!CheckLoggedIn())
            {
                return RedirectToAction("Index", "Home");
            }
            int? personID = HttpContext.Session.GetInt32("CurrentUserID");
            var alreadyParticipate = _context.ActivityParticipants
                                                .Include(p => p.DojoActivity)
                                                .Include(e => e.User)
                                                    .ThenInclude(r => r.JoinedActivities).ToList();
            var time = _context.Activities.Where(e => e.DojoActivityID == activityID).SingleOrDefault().ActivityTime;
            bool can = false;
            TimeSpan durTime;
            foreach (var item in alreadyParticipate)
            {
                durTime = TimeSpan.FromHours(item.DojoActivity.Duration);
                if (item.DojoActivity.ActivityTime <= time && item.DojoActivity.ActivityTime + durTime >= time)
                {
                    ViewBag.Error = "You already joined that about same time as this. Check your schedule!";
                    return View("Dashboard");
                }
            }
            ActivityParticipant newPerson = new ActivityParticipant
            {
                UserID = (int)personID,
                DojoActivityID = activityID
            };
            _context.ActivityParticipants.Add(newPerson);
            _context.SaveChanges();
            System.Console.WriteLine(activityID);
            return RedirectToAction("ShowActivity", new { ID = activityID });
        }
        public bool CheckLoggedIn()
        {
            if (HttpContext.Session.GetInt32("CurrentUserID") == null)
            {
                return false;
            }
            return true;
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
