namespace Demo.Repository.Pattern.Domain
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public double UnitPrice { get; set; }
    }
}
