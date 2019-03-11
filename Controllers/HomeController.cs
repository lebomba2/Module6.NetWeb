using System.Linq;
using Blogs.Models;
using Microsoft.AspNetCore.Mvc;

namespace Blogs.Controllers
{
    public class HomeController : Controller
    {
        // this controller depends on the BloggingRepository
        private IBloggingRepository repository;
        public HomeController(IBloggingRepository repo) => repository = repo;

        public IActionResult Index() => View(repository.Blogs.OrderBy(b => b.Name));

        public IActionResult AddBlog() => View();

        // BlogDetail method returns a view with blog and associated posts
        public IActionResult BlogDetail(int id) => View(new PostViewModel
        {
            // select the first blog with the blog ID
            blog = repository.Blogs.FirstOrDefault(b => b.BlogId == id),
            // select the posts associated with the blog
            Posts = repository.Posts.Where(p => p.BlogId == id)
        });

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddBlog(Blog model)
        {
            // if the form is valid
            if (ModelState.IsValid)
            {
                // if the argument blog name exists in the database
                if (repository.Blogs.Any(b => b.Name == model.Name))
                {
                    // custom error message
                    ModelState.AddModelError("", "Name must be unique");
                }
                else
                {
                    // add the model to the repository
                    repository.AddBlog(model);
                    // return the call to the Index method in the home controller
                    return RedirectToAction("Index");
                }
            }
            // return the view that was returned from the Index method in the home controller
            return View();
        }

        // DeleteBlog method accepts the id of the blog to delete
        public IActionResult DeleteBlog(int id)
        {
            // use the repository to delete the blog.
            // THe first or default will return the first blog returned or null
            repository.DeleteBlog(repository.Blogs.FirstOrDefault(b => b.BlogId == id));
            // redirect to the index method
            return RedirectToAction("Index");
        }

        // return the add post view
        public IActionResult AddPost(int id)
        {
            ViewBag.BlogId = id;
            return View();
        }

        // uses http post, add the post through the repository to the context
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddPost(int id, Post post)
        {
            post.BlogId = id;
            // if the form is valid
            if (ModelState.IsValid)
            {
                // add the post to the blog
                repository.AddPost(post);
                // return the view to the GlogDetail page with the id of the blog
                return RedirectToAction("BlogDetail", new { id = id });
            }

            @ViewBag.BlogId = id;
            // return the view
            return View();
        }

        // delete a post from a blog
        public IActionResult DeletePost(int id)
        {
            // find the post by id
            Post post = repository.Posts.FirstOrDefault(p => p.PostId == id);
            // get the blog id from the post
            int BlogId = post.BlogId;
            // delete the post using the repository
            repository.DeletePost(post);
            // redirect to BlogDetail method in home controller, using blogId from deleted post
            return RedirectToAction("BlogDetail", new { id = BlogId });
        }
    }
}
