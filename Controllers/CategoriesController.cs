namespace ShopApi.Controllers;

// Контроллер — класс, который отвечает на запросы по адресам /categories/...
// db — контекст базы, ASP.NET передаст его сам
[ApiController]
[Route("categories")]
public class CategoriesController(ShopContext db) : ControllerBase
{
    // Пример. GET /categories — все категории
    [HttpGet]
    public List<Category> GetAll()

    {
        return db.Categories.OrderBy(c => c.Id).ToList();
    }

    // Задание 4. GET /categories/{id}/products — товары одной категории, по возрастанию Id.
    // Если такой категории нет — 404.
    [HttpGet("{id}/products")]
    public ActionResult<List<Product>> GetProducts(int id)
    {
        // Подсказка: Хм, нам нужно проверять существование товара или категории? (Categories)
        if (!db.Categories.Any(с => с.Id == id))
            return NotFound();

        return db.Products
            .Where(p => p.CategoryId == id) // Подсказка: тут мы ищем по Id товаров, а нужно id категорий (CategoryId)
            .OrderBy(p => p.Id)
            .ToList();
    }
}
