using System;
namespace Domain.Azure
{
    public class Blob
    {
        public string? Uri { get; set; }
        public string? Name { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public Stream Content { get; set; } = Stream.Null;
        public DateTimeOffset? LastModifiedDate { get; set; }
    }
}