# Sitemap — Semantički model usmjeravanja

Ovaj dokument mapira dostupne URL-ove aplikacije na odgovarajući controller, action i view.

- URL: `/` (default)
  - Controller: `EscapeRoomController`
  - Action: `Index(string[]? city, string[]? difficulty, string[]? theme, string? sort)`
  - View: [EscapeRoom/Index.cshtml](EscapeRoomReviews/Views/EscapeRoom/Index.cshtml)

- URL: `/EscapeRoom` or `/EscapeRoom/Index`
  - Controller: `EscapeRoomController`
  - Action: `Index(string[]? city, string[]? difficulty, string[]? theme, string? sort)`
  - View: [EscapeRoom/Index.cshtml](EscapeRoomReviews/Views/EscapeRoom/Index.cshtml)

- URL: `/soba/{id}`
  - Controller: `EscapeRoomController`
  - Action: `Details(int id)`
  - View: [EscapeRoom/Details.cshtml](EscapeRoomReviews/Views/EscapeRoom/Details.cshtml)

- URL: `/Company` or `/Company/Index`
  - Controller: `CompanyController`
  - Action: `Index()`
  - View: [Company/Index.cshtml](EscapeRoomReviews/Views/Company/Index.cshtml)

- URL: `/tvrtka/{id}`
  - Controller: `CompanyController`
  - Action: `Details(int id)`
  - View: [Company/Details.cshtml](EscapeRoomReviews/Views/Company/Details.cshtml)

- URL: `/Location` or `/Location/Index`
  - Controller: `LocationController`
  - Action: `Index()`
  - View: [Location/Index.cshtml](EscapeRoomReviews/Views/Location/Index.cshtml)

- URL: `/lokacija/{id}`
  - Controller: `LocationController`
  - Action: `Details(int id)`
  - View: [Location/Details.cshtml](EscapeRoomReviews/Views/Location/Details.cshtml)

- URL: `/Review` or `/Review/Index`
  - Controller: `ReviewController`
  - Action: `Index()`
  - View: [Review/Index.cshtml](EscapeRoomReviews/Views/Review/Index.cshtml)

- URL: `/recenzija/{id}`
  - Controller: `ReviewController`
  - Action: `Details(int id)`
  - View: [Review/Details.cshtml](EscapeRoomReviews/Views/Review/Details.cshtml)

- URL: `/Theme` or `/Theme/Index`
  - Controller: `ThemeController`
  - Action: `Index()`
  - View: [Theme/Index.cshtml](EscapeRoomReviews/Views/Theme/Index.cshtml)

- URL: `/teme/{id}`
  - Controller: `ThemeController`
  - Action: `Details(int id)`
  - View: [Theme/Details.cshtml](EscapeRoomReviews/Views/Theme/Details.cshtml)

- URL: `/User` or `/User/Index`
  - Controller: `UserController`
  - Action: `Index()`
  - View: [User/Index.cshtml](EscapeRoomReviews/Views/User/Index.cshtml)

- URL: `/korisnik/{id}`
  - Controller: `UserController`
  - Action: `Details(int id)`
  - View: [User/Details.cshtml](EscapeRoomReviews/Views/User/Details.cshtml)

- URL: `/Home/Error` (error handler)
  - Controller: `HomeController`
  - Action: `Error()`
  - View: [Shared/Error.cshtml](EscapeRoomReviews/Views/Shared/Error.cshtml)

Notes:
- Default route is configured in [Program.cs](EscapeRoomReviews/Program.cs) as `{controller=EscapeRoom}/{action=Index}/{id?}` — therefore `/` resolves to `EscapeRoomController.Index`.
- Some Details actions use attribute routing (localized paths such as `/soba/{id}`, `/tvrtka/{id}`, etc.).
- Query parameters for filtering/sorting (e.g. `city`, `difficulty`, `theme`, `sort`) are accepted by `EscapeRoomController.Index` as optional query string values.
