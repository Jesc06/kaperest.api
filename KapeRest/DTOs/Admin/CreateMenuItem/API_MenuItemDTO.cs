namespace KapeRest.DTOs.Admin.CreateMenuItem
{
    public class API_MenuItemDTO
    {
        public string Name { get; set; }
        public int price { get; set; }
        public List<API_IngredientDTO> Ingredients { get; set; } = new();
    }
}
