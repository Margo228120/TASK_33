namespace ShopApi.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController(ShopContext db) : ControllerBase
{
    // ============ Часть B ============

    // GET /orders — все заказы, по возрастанию Id. Готово, ошибок нет
    [HttpGet]
    public List<Order> GetAll()
    {
        return db.Orders.OrderBy(o => o.Id).ToList();
    }

    // Задание 5. GET /orders/{id}/items — строки одного заказа, по возрастанию Id строки.
    // Если такого заказа нет — 404
    [HttpGet("{id}/items")]
    public ActionResult<List<OrderItem>> GetItems(int id)
    {
        if (!db.Orders.Any(o => o.Id == id))
            return NotFound();

        return db.OrderItems
            .Where(i => i.OrderId == id) // Подсказка: Ищем по товару или все таки по заказу?
            .OrderBy(i => i.OrderId) // Подсказка: По чему сортируем?
            .ToList();
    }

    // Задание 5. GET /orders/{id}/total — сумма заказа: количество × цена, сложенные по всем строкам заказа.
    // Если такого заказа нет — 404
    [HttpGet("{id}/total")]
    public ActionResult<int> GetTotal(int id)
    {
        if (!db.Orders.Any(o => o.Id == id))
            return NotFound();

        return db.OrderItems
            .Where(i => i.OrderId == id)
            .Sum(i => i.Quantity * i.Price); // Подсказка: сумма заказа - количество УМНОЖЕННОЕ на цену
    }

    // Задание 6. GET /orders/{id} — один заказ. Если заказа нет — 404
    [HttpGet("{id}")]
    public ActionResult<Order> Get(int id)
    {
        var order = db.Orders.OrderBy(o => o.Id).FirstOrDefault(o => o.Id == id); // Подсказка: Получаем заказ по ЕГО Id, а не Id клиента
        if (order == null)
            return NotFound();
        return order;
    }

    // ============ Часть D ============

    // Задание 11. PUT /orders/{id}/status?status=Оплачен — сменить статус заказа.
    // Допустимы только статусы: Новый, Оплачен, Доставлен, Отменён.
    // Другой статус — 400, и заказ НЕ меняется. Нет заказа — 404
    [HttpPut("{id}/status")]
    public ActionResult<Order> SetStatus(int id, string status)
    {
        var order = db.Orders.Find(id);
        if (order == null)
            return NotFound();

        // Подсказка: Код выполняется сверху вниз. Перенеси эти строчки кода в нужное место.
        order.Status = status;
        db.SaveChanges();

        string[] statuses = ["Новый", "Оплачен", "Доставлен", "Отменён"];
        if (!statuses.Contains(status))
            return BadRequest("Нет такого статуса");

        return order;
    }

    // Задание 12. POST /orders?clientId=11 — оформить заказ.
    // Тело — список строк: [{"productId": 2, "quantity": 5}, {"productId": 14, "quantity": 1}]
    // Цена строки — текущая цена товара за штуку. Остаток товара уменьшается на заказанное количество.
    // Если в запросе что-то не так — 400, и база НЕ должна измениться совсем.
    [HttpPost]
    public ActionResult<Order> Create(int clientId, List<OrderItem> items)
    {
        if (items.Count == 0)
            return BadRequest("В заказе нет товаров");
        if (!db.Clients.Any(c => c.Id == clientId))
            return BadRequest("Нет такого клиента");

        var order = new Order
        {
            ClientId = clientId,
            OrderDate = DateOnly.FromDateTime(DateTime.Today),
            Status = "Новый",
        };
        db.Orders.Add(order);

        foreach (var line in items)
        {
            var product = db.Products.Find(line.ProductId);
            if (product == null)
                return BadRequest($"Нет товара с номером {line.ProductId}");
            // Подсказка: Пользователь может прислать количество (Quantity) 0 или отрицательное. Добавь проверку
            if (product.Stock <= line.Quantity)
                return BadRequest($"Не хватает товара «{product.Name}»");

            order.Items.Add(new OrderItem
            {
                ProductId = product.Id,
                Quantity = line.Quantity,
                Price = product.Price * line.Quantity, // Подсказка: Нам не нужно считать общую стоимость. Price - цена ОДНОГО товара
            });
            product.Stock -= line.Quantity;
            db.SaveChanges(); // Подсказка: Сохранять изменения после каждого товара - очень неоптимизированно и несёт за собой ошибки в будущем
        }

        return Created($"/orders/{order.Id}", order);
    }
}
