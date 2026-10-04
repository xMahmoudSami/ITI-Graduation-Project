# Enterprise Inventory Management System (IMS) — Technical Architecture Documentation

Welcome to the comprehensive technical documentation for the **Enterprise Inventory Management System (IMS)**. This documentation suite provides an in-depth, engineering-grade analysis of every architectural layer, design pattern, security measure, and implementation detail within the system.

---

## 🏛️ High-Level System Architecture

The application is engineered using **ASP.NET Core (.NET 9 / C#)** following clean architectural principles: **Separation of Concerns (SoC)**, **Dependency Inversion (SOLID)**, **Repository/Service Layer Abstraction**, and **Domain-Driven Design (DDD)** principles for inventory data integrity.

```mermaid
graph TD
    Client[Web Browser / Client UI] -->|HTTP / HTTPS| Presentation[Presentation Layer<br/>Razor Views + SB Admin 2 + Theme Engine]
    Presentation -->|HTTP Requests| Controllers[Controller Layer<br/>MVC Controllers + RESTful API Controllers]
    Controllers -->|DTOs / ViewModels| ViewModels[Presentation Models Layer<br/>ViewModels + PagedResult + Validation Attributes]
    Controllers -->|Invokes Contracts| Services[Service Layer / Business Logic<br/>ICategoryService, IProductService, ISupplierService, IAIService]
    Services -->|RAG Analytics & LLM Completion| Groq[Groq Cloud API<br/>Llama 3.3 70B Versatile]
    Services -->|LINQ Queries / Transactions| DataAccess[Data Access Layer<br/>EF Core 9 + ApplicationDbContext]
    DataAccess -->|SQL Server Protocol| Database[(Relational Database<br/>Microsoft SQL Server)]
    Presentation -->|Culture Selection| Localization[Localization Engine<br/>SharedResource .resx (EN, AR, FR, ES, IT, DE)]
```

---

## 📂 Documentation Structure & Sitemap

Each architectural layer is documented in a dedicated deep-dive guide located in its respective subdirectory:

| Layer | Directory | Primary Topics Covered |
| :--- | :--- | :--- |
| **1. Data & Entities** | [`Docs/Models/README.md`](file:///c:/_Nero/_ITI/Graduation_Project/Inventory_Management_System/Docs/Models/README.md) | The 9 Domain Entities (`Product`, `Category`, `Supplier`, `SupplierProduct`, `Purchase`, `PurchaseItem`, `Sale`, `SaleItem`, `AIChatLog`), Foreign Keys, Data Annotations, `ApplicationDbContext`, Fluent API decimal precision, Unique Constraints, and Delete Behaviors (`Cascade` vs `Restrict`). |
| **2. Presentation Models** | [`Docs/ViewModels/README.md`](file:///c:/_Nero/_ITI/Graduation_Project/Inventory_Management_System/Docs/ViewModels/README.md) | Domain vs View Model separation, Mass-assignment & Over-posting defense, UI-specific validation, Generic Pagination (`PagedResult<T>`), Command Result encapsulation (`OperationResult`), and Delete guard checking. |
| **3. Controllers & Endpoints** | [`Docs/Controllers/README.md`](file:///c:/_Nero/_ITI/Graduation_Project/Inventory_Management_System/Docs/Controllers/README.md) | Breakdown of all 10 MVC & API Controllers, HTTP routing conventions (`[HttpGet]`, `[HttpPost]`, `[HttpDelete]`), Dependency Injection patterns, Anti-Forgery token enforcement (`[ValidateAntiForgeryToken]`), and response handling. |
| **4. Services & Business Logic** | [`Docs/Services/README.md`](file:///c:/_Nero/_ITI/Graduation_Project/Inventory_Management_System/Docs/Services/README.md) | Service layer interfaces (`ICategoryService`, `IProductService`, `ISupplierService`, `IAIService`), loose coupling, transactional stock management, Groq LLM integration, Retrieval-Augmented Generation (RAG) analytics pipeline, and fallback safeguards. |
| **5. User Interface & Frontend** | [`Docs/Views/README.md`](file:///c:/_Nero/_ITI/Graduation_Project/Inventory_Management_System/Docs/Views/README.md) | Modular Razor layout structure, POS Sales interface with dynamic client calculations, Purchases order builder, Chart.js Analytics dashboard, CSS custom properties Theme Engine (Dark/Light modes), dynamic RTL Arabic mirroring, and native alert replacement modal. |
| **6. Internationalization (i18n)** | [`Docs/Localization/README.md`](file:///c:/_Nero/_ITI/Graduation_Project/Inventory_Management_System/Docs/Localization/README.md) | Multi-language infrastructure supporting 6 cultures (`en`, `ar`, `fr`, `es`, `it`, `de`), resource synchronization (`SharedResource.*.resx`), Cookie Provider precedence, RTL text-direction handling, and culture vs invariant currency parsing. |

---

## 🛠️ Technology Stack & Core Specifications

- **Framework:** ASP.NET Core 9.0 (C# 13)
- **Object-Relational Mapper (ORM):** Entity Framework Core 9.0 (SQL Server Provider)
- **Database Engine:** Microsoft SQL Server
- **AI / LLM Integration:** Groq API (`llama-3.3-70b-versatile` / `llama-3.1-8b-instant`) with RAG database context injection
- **Frontend Architecture:** Razor Views (.cshtml), HTML5, Vanilla CSS3 Custom Properties (CSS Tokens), jQuery 3.6, Bootstrap 4.6 (SB Admin 2)
- **Data Visualization:** Chart.js 2.9 (Line, Bar, Doughnut charts)
- **Markdown & Security Sanitization:** `marked.js` + `DOMPurify` for real-time AI reply rendering
- **Internationalization:** ASP.NET Core Localization (`IHtmlLocalizer`, `IStringLocalizer`, `IViewLocalizer`)

---

## 🔒 Enterprise Security & Robustness Highlights

1. **Over-Posting (Mass-Assignment) Prevention:**
   All create/update controller actions exclusively accept strict ViewModels instead of entity instances, preventing malicious injection of internal IDs, timestamps, or unauthorized audit fields.
2. **Anti-CSRF Protection:**
   Every mutating HTTP POST endpoint strictly requires `[ValidateAntiForgeryToken]`, protecting users against Cross-Site Request Forgery attacks.
3. **Domain Integrity & Accidental Deletion Defense:**
   Parent tables (`Products`, `Categories`, `Suppliers`) enforce `DeleteBehavior.Restrict` in EF Core, disallowing hard deletions when active historical transactions (`PurchaseItem`, `SaleItem`) exist.
4. **XSS Prevention in AI Outputs:**
   Incoming LLM Markdown output is parsed through `marked.js` and sanitized through `DOMPurify` before DOM injection, mitigating prompt-injection and stored XSS vectors.
5. **Anti-Flicker Synchronous Theme Loading:**
   Theme preference (`dark` or `light`) is stored in `localStorage` and evaluated via a blocking `<script>` in the `<head>` prior to CSS rendering, completely eliminating UI flicker.
