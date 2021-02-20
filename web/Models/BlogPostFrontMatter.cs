using System;
using YamlDotNet.Serialization;

namespace Website.Models
{
    public class BlogPostFrontMatter
    {
        [YamlMember(Alias = "title")]
        public string Title { get; set; }
        
        [YamlMember(Alias = "description")]
        public string Description { get; set; }
        
        [YamlMember(Alias = "date")]
        public DateTime Date { get; set; }
        
        [YamlMember(Alias = "public")]
        public bool Public { get; set; }
        
        [YamlMember(Alias = "link")]
        public string LinkPath { get; set; }
    }
}