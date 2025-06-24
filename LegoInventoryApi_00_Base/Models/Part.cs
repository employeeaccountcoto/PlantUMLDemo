namespace LegoInventoryApi.Models
{
    public class Part
    {
        public string PartNum { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int PartCategoryId { get; set; }
        public string PartMaterial { get; set; } = string.Empty;
    }
}
