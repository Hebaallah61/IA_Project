using Webagency.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace pressAgency.Controllers
{
    public class DashboardController : Controller
    {
        PressAgencySysEntities2 db = new PressAgencySysEntities2();

        public ActionResult Index()
        {
            string admin;
            admin = (from x in db.users
                     select x.userName).FirstOrDefault();
            ViewBag.AdminName = admin;
            return View();
        }
        public ActionResult GetAdminInfo()
        {
            var admin = new user();
            admin = (from x in db.users
                     select x).FirstOrDefault();
            return View(admin);
        }

        [HttpGet]
        public ActionResult updateAdminInfo(int id)
        {
            var admin = db.users.Single(c => c.userID == id);
            return View(admin);
        }
        [HttpPost]
        public ActionResult updateAdminInfo(user admin1)

        {
            if (!ModelState.IsValid)
            {
                return View(admin1);
            }
            else
            {
                var obj = db.users.Single(c => c.userID == admin1.userID);
                obj.password = admin1.password;
                obj.confirmPass = admin1.confirmPass;
                obj.phone = admin1.phone;
                obj.userName = admin1.userName;
                obj.email = admin1.email;
                obj.photo = admin1.photo;
                obj.FirstName = admin1.FirstName;
                obj.LastName = admin1.LastName;
                db.SaveChanges();
                return RedirectToAction("GetAdminInfo");
            }
        }

        public ActionResult GetUsers()
        {
            List<user> Editors = new List<user>();
            Editors = db.users.Where(u => u.roleID != 1).ToList();


            return View(Editors);
        }

        [HttpGet]
        public ActionResult Adduser()
        {

            return View();
        }

        [HttpPost]
        public ActionResult Adduser(user user)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { result = 0 });
            }
            else
            {
                if (user.roleID != 1)
                {
                    if (db.users.Any(u => u.email == user.email))
                    {
                        return Json(new { result = 0 });
                    }
                    else
                    {
                        db.users.Add(user);
                        db.SaveChanges();
                        return Json(new { result = 1 });
                    }

                }
                else
                {
                    return Json(new { result = 0 });
                }

            }

        }

        [HttpGet]
        public ActionResult EditUser(int id)
        {
            var user = new user();
            user = db.users.SingleOrDefault(x => x.userID == id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.userRole = user.role.roleName;

            return View(user);
        }

        [HttpPost]
        public ActionResult EditUser(user user)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { result = 0 });
            }
            else
            {


                var userdb = db.users.SingleOrDefault(e => e.userID == user.userID);
                userdb.userName = user.userName;
                userdb.FirstName = user.FirstName;
                userdb.LastName = user.LastName;
                userdb.email = user.email;
                userdb.password = user.password;
                userdb.confirmPass = user.confirmPass;
                userdb.phone = user.phone;
                userdb.photo = user.photo;
                if (user.roleID != 1)
                {
                    db.SaveChanges();
                    return Json(new { result = 1 });
                }
                else
                {
                    return Json(new { result = 0 });
                }

            }
        }

        public ActionResult UserDetails(int id)
        {
            var user = new user();
            user = db.users.SingleOrDefault(e => e.userID == id);
            if (user == null)
                return HttpNotFound();
            ViewBag.userRole = user.role.roleName;
            return View(user);
        }

        [HttpGet]
        public ActionResult DeleteUser(int id)
        {
            var user = new user();
            user = db.users.SingleOrDefault(e => e.userID == id);
            if (user == null)
                return HttpNotFound();
            else if (user.roleID == 2)
            {
                var post = new articlePost();
                post = db.articlePosts.SingleOrDefault(p => p.editorID == id);
                if (post == null)
                {
                    //do nothing
                }
                else
                {
                    db.articlePosts.Remove(post);
                }
            }
            db.users.Remove(user);
            db.SaveChanges();
            return Json(new { result = 1 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PostRequests()
        {
            List<articlePost> Articles = new List<articlePost>();
            Articles = db.articlePosts.Where(p => p.isAccepted == false).ToList();
            return View(Articles);
        }

        public ActionResult AcceptPosts(int id)
        {
            var post = new articlePost();
            post = db.articlePosts.FirstOrDefault(p => p.articleID == id);
            if (post == null)
            {
                return HttpNotFound();
            }
            post.isAccepted = true;
            db.SaveChanges();
            return RedirectToAction("PostRequests");
        }










        public ActionResult GetPosts()
        {
            List<articlePost> Articles = new List<articlePost>();
            Articles = (from obj in db.articlePosts
                        where obj.isAccepted == true
                        select obj).ToList();

            return View(Articles);

        }

        public ActionResult GetPostDetails(int id)
        {
            articlePost Article = new articlePost();
            Article = (from obj in db.articlePosts
                       where obj.articleID == id
                       select obj).Single();

            return View(Article);

        }

        [HttpGet]
        public ActionResult EditPost(int id)
        {
            var Post = db.articlePosts.Single(c => c.articleID == id);
            return View(Post);
        }

        [HttpPost]
        public ActionResult EditPost(articlePost post)
        {
            if (!ModelState.IsValid)
            {
                return View(post);
            }
            else
            {
                var Post = db.articlePosts.Single(c => c.articleID == post.articleID);
                Post.articleID = post.articleID;
                Post.articlebody = post.articlebody;
                Post.articleTitle = post.articleTitle;
                Post.articleType = post.articleType;
                Post.postCreationDate = post.postCreationDate;
                db.SaveChanges();


                return RedirectToAction("GetPosts");
            }
        }
        public ActionResult DeletePost(int id)
        {
            if (!ModelState.IsValid)
            {
                return HttpNotFound();
            }
            else
            {
                articlePost article = new articlePost();
                article = (from obj in db.articlePosts
                           where obj.articleID == id
                           select obj).FirstOrDefault();
                db.articlePosts.Remove(article);
                db.SaveChanges();

                return RedirectToAction("GetPosts");
            }
        }






        public ActionResult LogOut()
        {
            Session.Clear();
            return RedirectToAction("Index1","Wall");
        }
    }
}