using System;
using System.Collections.Generic;

namespace Exe_Demo.Models;

public partial class Blog
{
    public int BlogId { get; set; }

    public string Title { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string? ShortDescription { get; set; }

    public string? ImageUrl { get; set; }

    public int? AuthorId { get; set; }

    public string? AuthorName { get; set; }

    public int? ViewCount { get; set; }

    public bool? IsPublished { get; set; }

    public DateTime? PublishedDate { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual User? Author { get; set; }

    public virtual ICollection<BlogComment> BlogComments { get; set; } = new List<BlogComment>();

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public Microsoft.AspNetCore.Http.IFormFile? ImageFile { get; set; }
}
