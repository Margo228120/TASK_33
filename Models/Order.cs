namespace ShopApi.Models;

// Заказ — строка таблицы orders
public class Order
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public DateOnly OrderDate { get; set; }
    public string Status { get; set; } = "";

    // Навигационные свойства — связанные строки из других таблиц.
    // В запросе можно писать o.Client.Name и o.Items — EF Core сам сделает JOIN.
    public Client Client { get; set; } = null!;
    public List<OrderItem> Items { get; set; } = new();
}
