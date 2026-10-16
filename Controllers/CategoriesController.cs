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

    // Задание 4. GET /categories/{id}/products — товары одной категории

}
