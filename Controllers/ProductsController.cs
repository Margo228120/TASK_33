namespace ShopApi.Controllers;

[ApiController]
[Route("products")]
public class ProductsController(ShopContext db) : ControllerBase
{
    // ============ Часть A ============

    // Задание 1. GET /products — все товары, по возрастанию Id
    // Подсказка: Наведи курсор на ошибку рядом с ToList(). Прочитай пункт "Правила C#, которые сэкономят тебе нервы" в README
    [HttpGet]
    public List<Product> GetAll()
    {
        return db.Products.OrderBy(p => p.Id).ToList();
    }

    // Задание 1. GET /products/{id} — ОДИН товар. Если такого товара нет — 404
    // Подсказка: List<...> внутри ActionResult<...> означает, что ожидается НЕСКОЛЬКО объектов в ответе.
    // Подсказка: Посмотри, как написан метод Create в этом файле (написано Задание 8)
    [HttpGet("{id}")]
    public ActionResult <Product> Get(int id)
    {
        var product = db.Products.Find(id);
        if (product == null) return NotFound();

        return product;
    }

    // Задание 2. GET /products/cheap?max=500 — товары, которые есть на складе И стоят не дороже max.
    // Сортировка — по цене, от дешёвых к дорогим.
    // Подсказка: OrderByDescending - от дорогих к дешёвым, а OrderBy - от дешёвых к дорогим.
    //            desc от слова "descending", т.е. "убывающий", а asc - "ascending" - "поднимающийся"
    [HttpGet("cheap")]
    public List<Product> GetCheap(int max)
    {
        return db.Products
            .Where(p => p.Stock > 0 && p.Price <= max)
            .OrderBy(p => p.Price) // Подсказка: Эта строка
            .ToList();
    }

    // ============ Часть C ============

    // Проверка товара перед сохранением. Возвращает текст ошибки или null, если всё хорошо.
    // Правила: название не пустое, цена больше нуля, остаток не меньше нуля, категория есть в базе.
    // Этой проверкой пользуются задания 8 и 9.
    private string? Check(Product p)
    {
        if (string.IsNullOrWhiteSpace(p.Name)) return "Название не может быть пустым"; // null - значения нет. whiteSpace - значение пустое ""
        if (p.Price <= 0) return "Цена должна быть больше нуля";
        if (p.Stock < 0) return "Остаток не может быть меньше нуля";
        if (!db.Categories.Any(c => c.Id == p.CategoryId)) return "Нет такой категории"; // Подсказка: Проверь условие

        return null;
    }

    // Задание 8. POST /products — добавить товар. Ответ — 201 и созданный товар с его id
    // Подсказка: Нет вызова метода сохранения в базу. Посмотри на метод Delete ниже (написано Задание 10). Видишь там SaveChanges?
    [HttpPost]
    public ActionResult<Product> Create(Product p)
    {
        // Проверяем наш товар с помощью метода Check, который описан выше.
        var error = Check(p);
        if (error != null)
            return BadRequest(error);

        db.Products.Add(p);
        db.SaveChanges();
        // Что-то нужно добавить...
        return Created($"/products/{p.Id}", p);
    }

    // Задание 9. PUT /products/{id} — изменить товар. Нет товара — 404, ошибка в данных — 400
    [HttpPut("{id}")]
    public ActionResult<Product> Update(int id, Product input)
    {
        var product = db.Products.Find(id);
        // Подсказка: что-то тут не так...
        if (product == null)
            return NotFound();

        var error = Check(input);
        if (error != null)
            return BadRequest(error);

        // Задаём новые параметры товару
        product.Name = input.Name;
        product.CategoryId = input.CategoryId;
        product.Price = input.Price;
        product.Stock = input.Stock;

        db.SaveChanges();
        return product;
    }

    // Задание 10. DELETE /products/{id} — удалить товар. Ответ — 204, нет товара — 404.
    // Товар, который есть в заказах, удалять нельзя — 409.
    // Подсказка: Нужно проверить, есть ли этот товар хоть в одной строке заказа
    // Подсказка 2: ПОДОБНАЯ проверка уже написана в нашей функции Check, но там проверяются категории, а нам нужны строки заказов (OrderItems)
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var product = db.Products.Find(id);
        if (product == null)
            return NotFound();
        if (!db.OrderItems.Any(oi => oi.ProductId == id))
        {
            return Conflict("Товар, который есть в заказах, удалять нельзя — 409");
        }
            db.Products.Remove(product);
        db.SaveChanges();
        return NoContent();
    }
}
