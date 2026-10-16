// День 3. API для ООО «Рога и Копыта»
//
// Пиши код каждого задания под строкой «// Задание N». Подробные условия и подсказки — в README.md.
// Готовый пример — GET /categories — повторяй его, когда не знаешь, с чего начать.
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Dapper;
using Npgsql;

// ============ Настройка (это уже сделано за тебя, менять не нужно) ============

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Русские буквы в ответах — как есть, а не По...
builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All));

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

// Подключение к базе shop. Строка подключения с паролем — в секретах пользователя (README, шаг 3)
var db = NpgsqlDataSource.Create(app.Configuration.GetConnectionString("Shop")!);
// Столбец category_id из базы попадёт в свойство CategoryId
DefaultTypeMap.MatchNamesWithUnderscores = true;

// ============ Пример ============

// GET /categories — все категории
app.MapGet("/categories", () =>
{
    using var conn = db.OpenConnection();
    return conn.Query<Category>("SELECT * FROM categories ORDER BY id");
});

// ============ Часть A. Получаем данные ============

// Задание 1. GET /products — все товары


// Задание 2. GET /products/{id} — один товар


// Задание 3. GET /clients — клиенты, можно отфильтровать по городу


// Задание 4. GET /categories/{id}/products — товары одной категории


// ============ Часть B. Несколько таблиц ============

// Задание 5. GET /orders — все заказы с названием клиента и суммой


// Задание 6. GET /orders/{id} — заказ вместе с составом


// Задание 7. GET /clients/{id}/orders — заказы одного клиента


// ============ Часть C. Изменяем данные ============

// Задание 8. POST /products — добавить товар


// Задание 9. PUT /products/{id} — изменить товар


// Задание 10. DELETE /products/{id} — удалить товар


// ============ Часть D. Заказы ============

// Задание 11. PUT /orders/{id}/status — сменить статус заказа


// Задание 12. POST /orders — оформить заказ


app.Run();

// ============ Модели — как строки таблиц выглядят в C# ============
// Новые классы для заданий добавляй сюда, в самый конец файла.

class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}

class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int CategoryId { get; set; }
    public int Price { get; set; }
    public int Stock { get; set; }
}

class Client
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string City { get; set; } = "";
}
