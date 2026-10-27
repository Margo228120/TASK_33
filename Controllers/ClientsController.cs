namespace ShopApi.Controllers;

[ApiController]
[Route("clients")]
public class ClientsController(ShopContext db) : ControllerBase
{
    // ============ Часть A ============

    // Задание 3. GET /clients — все клиенты. GET /clients?city=Казань — только клиенты из этого города
    [HttpGet]
    public List<Client> GetAll(string? city)
    {
        var clients = db.Clients.AsQueryable();

        // Подсказка: Если город УКАЗАН, то добавляем поиск по нему. А как мы проверяем сейчас?
        if (city != null)
            clients = clients.Where(c => c.City == city);

        return clients.OrderBy(c => c.Id).ToList();
    }

    // ============ Часть B ============

    // Задание 7. GET /clients/{id}/orders — заказы одного клиента, по возрастанию Id.
    // Если такого клиента нет — 404.
    // Этот метод напиши сам. Подсказки — в README.
    [HttpGet("{id}/orders")]
    public ActionResult<List<Order>> GetOrders(int id)
    {
        // Сюда проверку на существование клиента:
        // if (...)
        //    return NotFound();
        if (!db.Clients.Any(o => o.Id == id))
            return NotFound();

        return db.Orders
            .Where(o => o.ClientId == id)
            .OrderBy(o => o.Id)
            .ToList(); // замени эту строку своим кодом. Сделай поиск заказов (Orders) по ClientId с сортировкой по Id и верни список (List)
    }
}
