using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webagency.Models;



namespace WebApplication3.Controllers
{
    public class FactoryController : Controller
    {
        PressAgencySysEntities2 db = new PressAgencySysEntities2();
        // GET: Factory
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewProfile(int id)
        {
            user User = db.users.Single(x => x.userID == id);


            return View(User);

        }


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
                    User.roleID = 2;
                else
                    User.roleID = editProfile.roleID;
                db.Entry(User).State = EntityState.Modified;
                db.SaveChanges();
                return View("Index");
            }
            return View(editProfile);
        }



        public ActionResult MyPostes(int id)
        {
            Database.SetInitializer<PressAgencySysEntities2>(null);

            postsphotosview postesImages = new postsphotosview();
            postesImages.articleposts = db.articlePosts.ToList().Where(x => x.editorID == id);
            postesImages.photos = db.photos.ToList();
            return View(postesImages);
        }


        public ActionResult CreatePost()
        {
            newpostsview post = new newpostsview();

            return View(post);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult CreatePost(newpostsview post, HttpPostedFileBase file)
        {
            var lastpost = db.articlePosts.ToList();
            photo Images = new photo();
            articlePost article = new articlePost();
            int lastArtId = 0;

            if (ModelState.IsValid)
            {
                if (Session["user"] != null)
                {
                    int userid = int.Parse(((user)Session["user"]).userID.ToString());
                    article.editorID = userid;
                    article.articleTitle = post.articleTitle;
                    article.articlebody = post.articleBody;
                     DateTime date = DateTime.Now;
                    article.postCreationDate = date;
                    article.articleType = post.articleType;
                    article.isAccepted = false;
                    article.NumberOfViewers = 1;
                    article.NumberOfLikes = 0;
                    article.NumberOfDislikes = 0;
                    db.articlePosts.Add(article);
                    db.SaveChanges();

                }
                if (file != null)
                {
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(Server.MapPath("~/images/profiles/"), pic);

                    file.SaveAs(path);
                    post.photo = pic;
                    Images.path = pic;
                }
                if (lastpost != null)
                {
                    foreach (var item in lastpost)
                    {
                        lastArtId = item.articleID;
                    }
                    lastArtId += 1;


                }
                else
                {
                    lastArtId = 1;
                }

                Images.articleid = lastArtId;
                Images.title = post.articleTitle;
                db.photos.Add(Images);
                db.SaveChanges();
                return RedirectToAction("Index");
            }



            return View(post);
        }

        public ActionResult Receivedquestions(int id)
        {
            IEnumerable<question> questions = db.questions.ToList().Where(x => x.editorID == id && x.isAnswer == false);

            return View(questions);
        }


        [HttpGet]
        public ActionResult Answer(int id, String answer)
        {
            if (answer != "")
            {

                if (ModelState.IsValid)
                {
                    question questions = db.questions.Single(x => x.questionID == id);
                    int userId = int.Parse(((user)Session["user"]).userID.ToString());
                    if (questions != null)
                    {
                        questions.isAnswer = true;
                        questions.editorID = userId;
                        questions.Answer = answer;
                        db.Entry(questions).State = EntityState.Modified;
                        db.SaveChanges();
                        return Json(new { result = 1 }, JsonRequestBehavior.AllowGet);

                    }



                }

            }
            return Json(new { result = 0 }, JsonRequestBehavior.AllowGet);
        }
    }
}