namespace MinimalAPI_DotNet6.Model
{
    public class Supplier
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? DocumentationNumber { get; set; }
        public bool Active { get; set; }
    }
}
