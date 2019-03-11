using System;
using System.Linq;

namespace Blogs.Models
{
    public class EFBloggingRepository : IBloggingRepository
    {
        // the repository class depends on the BloggingContext service
        // which was registered at application startup
        // the context is our link to the database
        private BloggingContext context;
        public EFBloggingRepository(BloggingContext ctx)
        {
            context = ctx;
        }
        // create IQueryable for Blogs & Posts
        public IQueryable<Blog> Blogs => context.Blogs;
        public IQueryable<Post> Posts => context.Posts;

        public void AddBlog(Blog blog)
        {
            // context is the link to the database
            context.Add(blog);
            // must call SaveChanges to persist to the database
            context.SaveChanges();
        }

        // delete the blog from the context
        public void DeleteBlog(Blog blog)
        {
            context.Remove(blog);
            // save the changes to persist to the database
            context.SaveChanges();
        }

        // add post to the blog through the context
        public void AddPost(Post post)
        {
            context.Add(post);
            context.SaveChanges();
        }

        // delete post from a blog through the database context
        public void DeletePost(Post post)
        {
            context.Remove(post);
            context.SaveChanges();
        }
    }
}
