using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webagency.Models;
using System.Web.Script.Serialization;
using System.Data.Entity;
using System.Collections;
using System;

namespace Webagency.Controllers
{
    public class WallController : Controller
    {

        public PressAgencySysEntities2 db = new PressAgencySysEntities2();
        // GET: Wall
        public ActionResult Index1()
        {
            Database.SetInitializer<PressAgencySysEntities2>(null);

            postsphotosview postsphotos = new postsphotosview();
            postsphotos.articleposts=db.articlePosts.ToList().Where(x => x.isAccepted == true);
            postsphotos.photos = db.photos.ToList();

            return View(postsphotos);
        }

        [HttpGet]
        public ActionResult Filter(string search)
        {
            if (ModelState.IsValid)
            {

                if (search != null)
                {
                    IEnumerable<articlePost> Articles = db.articlePosts.ToList().Where(x => x.isAccepted == true && (x.articleTitle.Contains(search) || x.articleType.Contains(search)));
                    if (Articles == null)
                    {
                        user Users = db.users.FirstOrDefault(x => x.roleID == 2 && (x.LastName.Contains(search) || x.FirstName.Contains(search) || x.userName.Contains(search)));

                        if (Users != null)
                            Articles = db.articlePosts.ToList().Where(x => (x.isAccepted == true && Users.userID == x.editorID));

                    }



                    return Json(new { result = 1, articlePost = Articles }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { result = 0 }, JsonRequestBehavior.AllowGet);
        }




        public ActionResult Login()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel users)
        {



            if (ModelState.IsValid)
            {
                if (db.users.FirstOrDefault(a => a.email.Equals(users.Email) && a.password.Equals(users.Password)) != null)
                {
                    var Use = db.users.FirstOrDefault(a => a.email.Equals(users.Email) && a.password.Equals(users.Password));

                    if (Use != null)
                    {
                        Session["user"] = Use;
                    
                        Response.Cookies.Add(new HttpCookie("UserID", Use.userID.ToString()));
                        Response.Cookies.Add(new HttpCookie("UserName", Use.userName.ToString()));

                        Response.Cookies.Add(new HttpCookie("UserRole", Use.roleID.ToString()));



                        if (Use.roleID == 1)
                            return RedirectToAction("Index", "Dashboard");
                        else if (Use.roleID == 2)
                            return RedirectToAction("Index", "Factory");
                        else if (Use.roleID == 3)
                            return RedirectToAction("Index1", "Wall");
                        else
                            return View(users);
                    }
                    return View(users);
                }
            }
            return View(users);
        }


        [HttpGet]
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index1");
        }



        public ActionResult Register()
        {
            UserRoleViewModel Users = new UserRoleViewModel
            {
                userRoles = db.roles.ToList().Where(x => x.roleID != 1)
            };
            return View(Users);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Register(UserRoleViewModel Users, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {

                if (file != null)
                {
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(Server.MapPath("~/images/profiles/"), pic);

                    file.SaveAs(path);
                    Users.user.photo = pic;

                }
                db.users.Add(Users.user);
                db.SaveChanges();
                Session["user"] = Users.user;
                Session["UserName"] = Users.user.userName.ToString();
                Response.Cookies.Add(new HttpCookie("UserID", Users.user.userID.ToString()));
                Response.Cookies.Add(new HttpCookie("UserName", Users.user.userName.ToString()));
                Response.Cookies.Add(new HttpCookie("UserRole", Users.user.roleID.ToString()));

                if (Users.user.roleID == 3)
                {
                    return RedirectToAction("Index1");
                }
                else if (Users.user.roleID == 2)
                {
                    return RedirectToAction("Index", "Factory");
                }


            }


            IEnumerable<role> department = db.roles.ToList().Where(x => x.roleID != 1);
            Users.userRoles = department;
            return View(Users);
        }


        public ActionResult Show(int id)
        {

            postsphotosview postesphotos = new postsphotosview();
            postesphotos.articleposts = db.articlePosts.ToList().Where(x => x.articleID == id);
            postesphotos.photos = db.photos.ToList();
            if (Session["user"] != null)
            {
                int userid = int.Parse(((user)Session["user"]).userID.ToString());

                savePost save = db.savePosts.FirstOrDefault(x => x.postID == id && x.viewerID == userid);
                if (save != null)
                {
                    db.savePosts.Remove(save);
                    db.SaveChanges();
                }
            }

                return View(postesphotos);
        }

        /// /////////////////////////////////////////////////////////////
        public ActionResult Like(int id)
        {
            if (ModelState.IsValid)
            {

                int userid = int.Parse(((user)Session["user"]).userID.ToString());
                LikesPost like = db.LikesPosts.SingleOrDefault(x => x.articleID == id && x.userID == userid);
                if (like == null)
                {
                    LikesPost like1 = new LikesPost();
                    var article = db.articlePosts.Single(x => x.articleID == id);
                    article.NumberOfLikes = article.NumberOfLikes + 1;
                    DisLike dislike = db.DisLikes.SingleOrDefault(x => x.articleID == id && x.userID == userid);
                    if (dislike != null)
                    {
                        if (article.NumberOfDislikes > 0)
                            article.NumberOfDislikes = article.NumberOfDislikes - 1;
                        db.DisLikes.Remove(dislike);
                    }
                    like1.userID = userid;
                    like1.articleID = id;
                    db.LikesPosts.Add(like1);
                    db.Entry(article).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { respons = 1 }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { respons = 0 }, JsonRequestBehavior.AllowGet);
                }

            }


            return Json(new { respons = 0 }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult DisLike(int id)
        {

            if (ModelState.IsValid)
            {

                int userid = int.Parse(((user)Session["user"]).userID.ToString());
                DisLike dislike = db.DisLikes.FirstOrDefault(x => x.articleID == id && x.userID == userid);
                if (dislike == null)
                {
                    var article = db.articlePosts.Single(x => x.articleID == id);
                    article.NumberOfDislikes = article.NumberOfDislikes + 1;
                    LikesPost like = db.LikesPosts.FirstOrDefault(x => x.articleID == id && x.userID == userid);
                    if (like != null)
                    {
                        if (article.NumberOfLikes > 0)
                            article.NumberOfLikes = article.NumberOfLikes - 1;
                        db.LikesPosts.Remove(like);
                    }
                    DisLike dislike1 = new DisLike();
                    dislike1.userID = userid;
                    dislike1.articleID = id;

                    db.DisLikes.Add(dislike1);
                    db.Entry(article).State = EntityState.Modified;
                    db.SaveChanges();

                    return Json(new { respons = 1 }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { respons = 0 }, JsonRequestBehavior.AllowGet);
                }

            }


            return Json(new { respons = 0 }, JsonRequestBehavior.AllowGet);
        }

        ////////////////////////////////////////////////////////////////////

        [HttpGet]
        public ActionResult Save(int id)
        {

            if (ModelState.IsValid)
            {
                if (Session["user"] != null)
                {

                    int userid = int.Parse(((user)Session["user"]).userID.ToString());
                    savePost saving = db.savePosts.FirstOrDefault(x => x.postID == id && x.viewerID == userid);


                    if (saving == null)
                    {
                        savePost saving1 = new savePost();
                        saving1.postID = id;
                        saving1.viewerID = int.Parse(((user)Session["user"]).userID.ToString());
                        db.savePosts.Add(saving1);
                        db.SaveChanges();
                        return Json(new { respons = 1 }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { respons = 0 }, JsonRequestBehavior.AllowGet);
                    }
                }
            }

            return Json(new { respons = 0 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult savedposts(int id)
        {

            savedposts save = new savedposts();
            //////////
            save.articlePosts = db.articlePosts.ToList();
            save.savePosts = db.savePosts.ToList().Where(x => x.viewerID == id);

            return View(save);
        }

        public ActionResult ViewProfile(int id)
        {
            user person = db.users.Single(x => x.userID == id);


            return View(person);

        }

        /// ////////////////////////////////////////////////////////////////////////

        public ActionResult Edit(int id)
        {

            user User = db.users.Single(x => x.userID == id);
            editprofile editProfile = new editprofile();
            editProfile.Id = User.userID;
            editProfile.FirstName = User.FirstName;
            editProfile.LastName = User.LastName;
            editProfile.UserName = User.userName;
            editProfile.Email = User.email;
            editProfile.PhoneNumber = User.phone;
            editProfile.Password = User.password;
            editProfile.confirmPass = User.confirmPass;
            editProfile.Image = User.photo;
            editProfile.roleID = (int)User.roleID;
            return View(editProfile);
        }
        [HttpPost]
        public ActionResult Edit(editprofile editProfile, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                user User = new user();
                if (file != null)
                {
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(Server.MapPath("~/images/profiles/"), pic);

                    file.SaveAs(path);

                    editProfile.Image = pic;
                    User.photo = pic;
                }
                else
                {
                    User.photo = editProfile.Image;
                }
                User.userID = editProfile.Id;   
                User.FirstName = editProfile.FirstName;
                User.LastName = editProfile.LastName;
                User.userName = editProfile.UserName;
                User.email = editProfile.Email;
                User.phone = editProfile.PhoneNumber;
                User.password = editProfile.Password;
                User.confirmPass = editProfile.confirmPass;
                
                
                
                if (editProfile.roleID == 0)
                    User.roleID = 3;
                else
                    User.roleID = editProfile.roleID;
                db.Entry(User).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index1");
            }
            return View(editProfile);
        }

        [HttpPost]
        public ActionResult AddQuestion(int id, String ques)
        {
            if (ques != "")
            {

                if (ModelState.IsValid)
                {
                    var article = db.articlePosts.Single(x => x.articleID == id);
                    int userId = int.Parse(((user)Session["user"]).userID.ToString());

                    question questions = new question();
                    questions.Answerid = userId;
                    questions.isAnswer = false;
                    questions.editorID = article.editorID;
                    ////////
                    questions.Question1 = ques;
                    db.questions.Add(questions);
                    db.SaveChanges();
                    return Json(new { result = 1 });
                }

            }
            return Json(new { result = 0 });
        }
        public ActionResult Answers(int id)
        {

            int userId = int.Parse(((user)Session["user"]).userID.ToString());

            IEnumerable<question> questions = db.questions.ToList().Where(x => x.Answerid == userId && x.isAnswer == true);
            return View(questions);
        }
        [HttpGet]
        public ActionResult CountViews(int id)
        {
            if (Session["user"] != null)
            {
                int userId = int.Parse(((user)Session["user"]).userID.ToString());
                noOfView NumView = db.noOfViews.FirstOrDefault(x => x.articleID == id && x.userID == userId);
                if (NumView == null)
                {
                    noOfView number = new noOfView();
                    number.userID = userId;
                    number.articleID = id;
                    var article = db.articlePosts.Single(x => x.articleID == id);
                    article.NumberOfViewers = article.NumberOfViewers + 1;

                    db.noOfViews.Add(number);
                    db.Entry(article).State = EntityState.Modified;
                    db.SaveChanges();

                }
            }

            return Json(new { respons = 0 }, JsonRequestBehavior.AllowGet);
        }
    }

}