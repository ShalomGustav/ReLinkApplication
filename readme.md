
# ReLinkApplication

**Автор**: Фатихов Максим Сергеевич

## Описание

ReLinkApplication — это `REST API`, разработанное на платформе `.NET 8.0`, для работы с `URL`. Приложение предоставляет два основных функционала:
1. Создание короткого `URL` на основе длинного.
2. Преобразование короткого `URL` обратно в длинный.

## Функционал

- **Middleware**: Обработка ошибок через пользовательский компонент.
- **Entity Framework Core**: ORM для работы с базой данных.
- **Swagger**: Генерация документации и тестирование API.
- **Validation**: Валидация `URL` с использованием регулярных выражений.

## Содержание
- [Требования](#требования)
- [Структура проекта](#структура-проекта)
  - [Controllers](#controllers)
  - [Middleware](#middleware)
  - [Models](#models)
  - [Services](#services)
  - [Repositories](#repositories)
  - [Migrations](#migrations)
- [Конфигурация проекта. Файл appsettings.json](#конфигурация-проекта-файл-appsettingsjson)
- [Program.cs](#programcs)
- [Тестирование](#тестирование)

## Требования

- .NET 8.0 SDK или выше.
- Microsoft SQL Server. **Выбор обусловлен тем, что это мини-проект, для моих задач  функционал MS SQL, более чем достаточен**.  
- Среда разработки: Visual Studio 2022 / Rider / VS Code.

---

## Структура проекта

### **Controllers**
- **UrlControllers.cs**: Контроллер для работы с URL:
  - `POST /create`: Создание короткого URL.
  - `GET /{shortUrl}`: Перенаправление на оригинальный URL.

### **Middleware**
- **ErrorHandlingMiddleware.cs**: Перехватывает исключения и возвращает JSON с описанием ошибки.

### **Models**
- **Url.cs**:
  - `Id`: Уникальный идентификатор. 
  - `LongUrl`: Оригинальный URL.
  - `ShortUrl`: Сокращённый URL.

### **Services**
- **UrlServices.cs**:
  - `CreateShortUrlAsync`: Генерация короткого URL.
  - `GetLongUrlByShortUrlAsync`: Получение длинного URL.

### **Repositories**
- **UrlDbContext.cs**: Контекст базы данных, настроенный для работы с Entity Framework Core.

### **Migrations**
- Содержит миграции базы данных для работы с таблицей `Urls`.

---

## Конфигурация проекта. Файл appsettings.json

Файл `appsettings.json` содержит ключевые настройки:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ReLinkDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "ApplicationSettings": {
    "BaseUrl": "http://localhost:7065/"
  },
  "AllowedHosts": "*"
}
```

---

## Program.cs

Файл `Program.cs` содержит основную логику запуска приложения.

### Основные компоненты

1. **Настройка сервисов**
   ```csharp
   builder.Services.AddDbContext<UrlDbContext>(options =>
       options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
   builder.Services.AddScoped<UrlServices>();
   builder.Services.AddControllers();
   builder.Services.AddEndpointsApiExplorer();
   builder.Services.AddSwaggerGen();
   ```

2. **Промежуточное ПО**
   ```csharp
   app.UseMiddleware<ErrorHandlingMiddleware>();
   app.UseSwagger();
   app.UseSwaggerUI();
   app.UseHttpsRedirection();
   app.UseAuthorization();
   app.MapControllers();
   app.Run();
   ```

---

## Тестирование

1. Перейдите в папку с тестами:
   ```bash
   cd ReLinkApplication.Tests
   ```

2. Запустите тесты:
   ```bash
   dotnet test
   ```
3. Если в качестве среды программирования используется Visual Studio, то запуск тестов можно упростить, нажав `Test` -> `Run All Tests`.
   
Тесты покрывают логику генерации коротких URL и получения оригинальных ссылок.

---
