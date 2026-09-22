# تقرير توثيق إنجاز المشروع: الموديول 1 والموديول 2
## Inventory Management System — ITI Graduation Project

---

## 📌 1. نظرة عامة (Executive Summary)

تم إنجاز الجزء الخاص بك بالكامل بنجاح تام وفقاً لجميع متطلبات الـ Prompt بدقة متناهية:
* **الموديول الأول (Module 1):** الفئات والمنتجات (**Categories & Products**)
* **الموديول الثاني (Module 2):** الموردين وربط الموردين بالمنتجات (**Suppliers & Supplier-Product Mapping**)

تم تصميم الكود باتباع أعلى معايير هندسة البرمجيات الاحترافية لبيئة عمل فرق التطوير المشتركة (**Shared Team Codebase**)، مع ضمان العزل التام (**Complete Isolation**) وعدم المساس بقاعدة البيانات أو عمل باقي أفراد الفريق.

---

## 🛡️ 2. قواعد حماية عمل الفريق المشترك (Zero Team Conflicts)

تم الالتزام التام بالقواعد الصارمة للمشروع:
1. **قاعدة البيانات (Database):** لم يتم تعديل أي جدول، عمود، نوع بيانات، مفتاح أجنبي (FK)، أو علاقة في الـ Schema.
2. **الـ Migrations:** لم يتم إنشاء أو تنفيذ أي Migration إطلاقاً.
3. **أجزاء الفريق الأخرى:** لم يتم لمس أو تعديل كود المبيعات (**Sales/SaleItems**)، المشتريات (**Purchases/PurchaseItems**)، لوحة التحكم (**Dashboard**)، أو الذكاء الاصطناعي (**AI**).
4. **الملفات المشتركة المعدلة فقط (3 ملفات):**
   * `Program.cs`: تم تسجيل الـ 4 Services الجديدة فقط بنطاق `AddScoped`.
   * `GlobalSettings.cs`: إضافة سطر واحد فقط `global using Inventory_Management_System.Services;`.
   * `_Layout.cshtml`: إضافة رابط "Supplier Mappings" أسفل قائمة Inventory، واستدعاء partial الـ `_Alerts`.

---

## 🏗️ 3. الهيكلية البرمجية المتبعة (Layered Architecture)

تم بناء النظام وفق النمط القياسي:
```
Database Entity (المصدر الثابت للحقيقة - Read/Write)
       ↓
ViewModels (عزل الكيانات عن المستخدم + DataAnnotations)
       ↓
Services & Interfaces (طبقة منطق العمل المستقلة + قواعد الأمان)
       ↓
Controllers (Thin Controllers تعتمد بالكامل على الـ Dependency Injection)
       ↓
Razor Views (واجهات متجاوبة مبنية على SB Admin 2 و Bootstrap 4)
```

---

## 📦 4. تفاصيل ما تم إنجازه في الموديول 1 (Products & Categories)

### أ. إدارة الفئات (Categories)
* **عرض القائمة (Index):**
  * جدول يعرض: اسم الفئة، الوصف، وعدد المنتجات المرتبطة بها (`ProductsCount`).
  * بحث غير متزامن بالاسم من جهة السيرفر (Server-Side Search).
  * ترقيم الصفحات (Pagination).
* **إضافة فئة (Create):**
  * نموذج إدخال مع شروط تحقق (الاسم إجباري، الطول بين 2 و 100 حرف).
  * التحقق من تفرد اسم الفئة برمجياً (Category Name Uniqueness).
* **تعديل فئة (Edit):**
  * تعديل الاسم والوصف مع استثناء الفئة الحالية أثناء فحص التفرد.
* **تفاصيل الفئة (Details):**
  * عرض بيانات الفئة مع **جدول تفصيلي بجميع المنتجات التابعة لها** وحالتها وأسعارها وإمكانية الانتقال للمنتج مباشرة.
* **حذف آمن (Safe Delete):**
  * **نظام حظر ذكي:** إذا كانت الفئة تحتوي على أي منتجات، يُحظر زر الحذف نهائياً وتظهر رسالة تحذيرية:
    > *"This category contains X product(s). Move or delete those products before deleting the category."* مع زر لتحويل المستخدم لعرض تلك المنتجات.

---

### ب. إدارة المنتجات (Products)
* **عرض القائمة (Index):**
  * جدول يعرض: كود الـ SKU، اسم المنتج، الفئة، السعر، الكمية، والحد الأدنى للمخزون.
  * فلترة متقدمة متعددة الشروط:
    1. بحث بنص الاسم أو كود الـ SKU.
    2. فلترة حسب الفئة (Category Dropdown).
    3. فلترة حسب حالة المخزون (All, In Stock, Low Stock, Out of Stock).
  * ترقيم سيرفر ديناميكي يحافظ على خيارات الفلتر والبحث أثناء التنقل بين الصفحات.
* **حساب حالة المخزون ديناميكياً (Stock Status):**
  * **لم يتم إضافة عمود بالداتابيز**، بل تم حسابها برمجياً في الـ ViewModel والـ Service:
    * إذا كانت الكمية `StockQuantity == 0` ← يظهر باللون الأحمر: **Out of Stock**.
    * إذا كانت الكمية `StockQuantity <= LowStockThreshold` ← يظهر باللون الأصفر: **Low Stock**.
    * إذا كانت الكمية `StockQuantity > LowStockThreshold` ← يظهر باللون الأخضر: **In Stock**.
* **إضافة وتعديل منتج (Create / Edit):**
  * التحقق من عدم تكرار كود الـ SKU على مستوى السيرفر وقاعدة البيانات.
  * في التعديل: يتم استثناء المنتج الحالي حتى لا يظهر خطأ تكرار لنفس المنتج.
  * فحص صحة الفئة المختارة والتأكد من وجودها.
  * فحص الأسعار والكميات (موجبة وغير سالبة).
* **تفاصيل المنتج (Details):**
  * كروت إحصائية ملخصة: عدد الموردين المرتبطين، عدد سجلات فواتير الشراء، وعدد سجلات المبيعات.
  * **جدول تفصيلي بالموردين الذين يوردون هذا المنتج** مع أسعار العقود وأيام التوريد وكود المورد.
* **حذف المنتج الآمن (Safe Delete):**
  * فحص السجلات التاريخية في `PurchaseItems` و `SaleItems`.
  * **إذا كان المنتج مرتبطاً بمشتريات أو مبيعات، يُمنع حذفه نهائياً لحماية التاريخ المالي** وتظهر رسالة توضح سبب المنع وعدد السجلات.
  * إذا كان آمناً للحذف وله ارتباطات بموردين فقط (SupplierProducts)، يتم إعلام المستخدم بأنه سيتم فك ارتباط الموردين تلقائياً (Cascade).

---

## 🚚 5. تفاصيل ما تم إنجازه في الموديول 2 (Suppliers & Mappings)

### أ. إدارة الموردين (Suppliers)
* **عرض القائمة (Index):**
  * جدول منظم يعرض: اسم المورد، جهة الاتصال، الهاتف، البريد الإلكتروني، وعدد المنتجات الموردة.
  * بحث بالاسم أو بجهة الاتصال مع الترقيم.
* **إضافة وتعديل مورد (Create / Edit):**
  * التحقق من صيغة الهاتف والبريد الإلكتروني والاسم والعنوان.
* **تفاصيل المورد (Details):**
  * عرض بيانات المورد والتواصل معه بروابط سريعة (`mailto:` و `tel:`).
  * **جدول بالمنتجات المربوطة بهذا المورد** مع أسعار التوريد وأيام التسليم.
* **حذف المورد الآمن (Safe Delete):**
  * فحص جدول المشتريات `Purchases`.
  * إذا كان للمورد فواتير مشتريات تاريخية، **يُمنع الحذف نهائياً** حفاظاً على تكامل البيانات.

---

### ب. ربط الموردين بالمنتجات (Supplier-Product Mapping)
* **عرض القائمة (Index):**
  * جدول العلاقات الشامل: المورد، المنتج، الفئة، كود المورد (Supplier SKU)، سعر العقد (Contract Price)، ومدة التوريد بالأيام (Lead Time).
  * فلتر حسب المورد، فلتر حسب المنتج، وبحث بالأكواد.
* **إضافة ربط جديد (Create):**
  * اختيار المورد والمنتج من Dropdowns معبأة تلقائياً.
  * **منع التكرار (Duplicate Prevention):** فحص مسبق لمنع ربط نفس المورد بنفس المنتج مرتين، وعرض رسالة خطأ واضحة بدون التسبب في Database Exception.
* **تعديل الربط (Edit):**
  * تعديل أسعار العقود وأيام التوريد وأكواد المورد.
* **فك الارتباط (Delete / Unlink):**
  * شاشة تأكيد توضح للمستخدم أنه سيتم فك الارتباط فقط بين المورد والمنتج دون حذف أي منهما من النظام.

---

## 🔒 6. منظومة التحقق خماسية المستويات (5-Level Validation)

1. **Level 1 (Client-Side):**
   * تفعيل Unobtrusive Validation مع مكتبات `jquery.validate`.
   * رسائل فورية تظهر بجوار كل حقل دون الحاجة لتحميل الصفحة من جديد.
2. **Level 2 (Server-Side):**
   * فحص إجباري لـ `if (!ModelState.IsValid)` في جميع عمليات الـ POST.
   * الحفاظ التام على البيانات التي أدخلها المستخدم وإعادة ملء الـ Dropdowns في حال وجود خطأ.
3. **Level 3 (Business Logic):**
   * فحص تفرد الـ SKU مع استثناء الـ ID الحالي في التعديل.
   * فحص تكرار الربط (SupplierID + ProductID).
   * التحقق من الحدود الرقمية (أسعار أكبر من 0، كميات غير سالبة).
4. **Level 4 (Database Integrity):**
   * الاعتماد على قيود الفهرسة الفريدة (Unique Indexes) الموجودة أصلاً في الداتابيز كحماية نهائية.
5. **Level 5 (User-Friendly Presentation):**
   * لا يرى المستخدم أي خطأ برمجيات (SQL/EF Core Exception)، بل تظهر رسائل واضحة ومفهومة.

---

## 📄 7. نظام ترقيم الصفحات الاحترافي (Server-Side Pagination)

* **الأداء العالي:** تنفيذ استعلامات `IQueryable` متبوعة بـ `CountAsync` ثم `Skip()` و `Take()` مباشرة على مستوى السيرفر دون سحب الجداول للذاكرة (In-Memory).
* **الحفاظ على الفلاتر:** كود ذكي داخل `_Pagination.cshtml` يقوم بدمج Query Parameters تلقائياً (مثل `?page=2&Search=Laptop&CategoryID=1`) حتى لا تضيع الفلاتر أثناء التنقل.
* **التحكم بحجم الصفحة:** إمكانية التبديل بين (10, 25, 50, 100) عنصر لكل صفحة.
* **محددات الصفحات:** أزرار First, Prev, أرقام الصفحات الحالية مع علامة `...` عند تعدد الصفحات، Next, Last.

---

## 🎨 8. التصميم وتجربة المستخدم (UI/UX Design System)

* **التناسق مع ثيم المشروع:** استخدام نفس ألوان وستايل **SB Admin 2** و **Bootstrap 4.6.0**.
* **شريط المسار (Breadcrumbs):** موجود في جميع الشاشات لتسهيل الرجوع للوحة التحكم والقوائم الرئيسية.
* **تنبيهات النظام المنبثقة (_Alerts):** استخدام `TempData["SuccessMessage"]` و `TempData["ErrorMessage"]` مع تصميم Bootstrap Alert يغلق تلقائياً.
* **حالات الفراغ (Empty States):** عند عدم وجود بيانات أو نتائج بحث تظهر أيقونة توضيحية وزر سريع للإضافة أو تفريغ الفلتر بدلاً من جدول فارغ وممل.

---

## 📁 9. فهرس الملفات المنفذة بالكامل (Complete File Inventory)

### أ. ملفات تم إنشاؤها (New Files)
```
📁 ViewModels/
├── PagedResult.cs
├── OperationResult.cs
├── CategoryViewModels.cs
├── ProductViewModels.cs
├── SupplierViewModels.cs
└── SupplierProductViewModels.cs

📁 Services/
├── ICategoryService.cs & CategoryService.cs
├── IProductService.cs & ProductService.cs
├── ISupplierService.cs & SupplierService.cs
└── ISupplierProductService.cs & SupplierProductService.cs

📁 Controllers/
├── CategoriesController.cs
├── ProductsController.cs
├── SuppliersController.cs
└── SupplierProductsController.cs

📁 Views/
├── Shared/_Alerts.cshtml
├── Shared/_Pagination.cshtml
├── Categories/ (Index, Create, Edit, Details, Delete)
├── Products/ (Index, Create, Edit, Details, Delete)
├── Suppliers/ (Index, Create, Edit, Details, Delete)
└── SupplierProducts/ (Index, Create, Edit, Delete)
```

### ب. ملفات تم تعديلها بحذر (Safely Modified Files)
* `Program.cs` (تسجيل الخدمات في الـ DI Container).
* `GlobalSettings.cs` (إضافة سطر الـ using للخدمات).
* `Views/Shared/_Layout.cshtml` (إضافة رابط Supplier Mappings و partial التنبيهات).

---

## 🚀 10. نتيجة البناء والتشغيل (Build & Verification)

* **أمر الفحص:** `dotnet build`
* **النتيجة:** **0 Errors (صفر أخطاء برمجية)**.
* **حالة الجاهزية:** جاهز 100% للتشغيل والعرض والتقييم مباشرة دون الحاجة لأي تعديلات إضافية.
