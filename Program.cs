// День 3. API для ООО «Рога и Копыта»
//
// Это настройка API — её уже сделали за тебя, менять не нужно.
// Задания — в папке Controllers, подробные условия — в README.md.
using System.Text.Encodings.Web;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    // Русские буквы в ответах — как есть, а не непонятными кодами
    .AddJsonOptions(o => o.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All));
builder.Services.AddSwaggerGen();

// Подключение к базе shop. Строка подключения с паролем — в секретах пользователя (README, шаг 3).
// UseSnakeCaseNamingConvention: свойство CategoryId в C# — это столбец category_id в базе
builder.Services.AddDbContext<ShopContext>(o => o
    .UseNpgsql(builder.Configuration.GetConnectionString("Shop"))
    .UseSnakeCaseNamingConvention());

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();
