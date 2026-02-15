// ==============================================================================
// MVC BEST PRACTICES - Model-View-Controller Pattern in ASP.NET Core
// ==============================================================================
//
// WHAT IS MVC?
// ------------
// A pattern that separates concerns into Models (data), Views (UI), and
// Controllers (application flow). MVC is ideal for server-rendered web apps.
//
// WHY IT MATTERS
// --------------
// - Clear separation of concerns
// - Strong testability for controllers and services
// - Reusable views and view components
// - Server-rendered HTML for SEO and accessibility
//
// WHEN TO USE
// -----------
// - YES: Web apps with server-rendered views
// - YES: Apps needing strong SEO and fast first paint
// - YES: Teams comfortable with MVC conventions
//
// WHEN NOT TO USE
// ---------------
// - NO: Pure JSON APIs (use Web API or Minimal API)
// - NO: SPA-only apps where server renders little HTML
//
// REAL-WORLD EXAMPLE
// ------------------
// E-commerce storefront:
// - Controllers assemble product and cart data
// - Views render HTML pages with partials and components
// ==============================================================================

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace RevisionNotesDemo.WebAPI.MVC;

/// <summary>
/// EXAMPLE 1: BASIC MVC CONTROLLER STRUCTURE
/// 
/// THE PROBLEM:
/// Controllers that mix concerns, duplicate logic, don't follow conventions,
/// or tightly couple to data access.
/// 
/// THE SOLUTION:
/// - Inherit from Controller base class
/// - Follow naming conventions (ControllerName + Controller)
/// - Use dependency injection
/// - Keep controllers thin (delegate to services)
/// - Return appropriate IActionResult types
/// 
/// WHY IT MATTERS:
/// - Conventions enable automatic routing and view discovery
/// - Thin controllers are easier to test and maintain
/// - Dependency injection enables swappable implementations
/// - Proper return types enable content negotiation
/// 
/// BEST PRACTICE: Controllers should orchestrate, not implement business logic
/// </summary>
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IProductService _productService;

    public HomeController(
        ILogger<HomeController> logger,
        IProductService productService)
    {
        _logger = logger;
        _productService = productService;
    }

    // ✅ GOOD: Index action returns a view
    public IActionResult Index()
    {
        _logger.LogInformation("Home page requested");
        var model = new HomeViewModel
        {
            Title = "Welcome",
            Products = _productService.GetFeaturedProducts()
        };

        return View(model); // Returns Views/Home/Index.cshtml
    }

    // ✅ GOOD: Action with parameter
    [HttpGet]
    public IActionResult Product(int id)
    {
        var product = _productService.GetProductById(id);
        if (product == null)
        {
            return NotFound();
        }

        var model = new ProductViewModel
        {
            Product = product,
            RelatedProducts = _productService.GetRelatedProducts(id)
        };

        return View(model);
    }

    // ❌ BAD: Business logic in controller
    // public IActionResult PlaceOrder(OrderModel order)
    // {
    //     var dbContext = new AppDbContext();
    //     var entity = new OrderEntity { /* map properties */ };
    //     dbContext.Orders.Add(entity);
    //     dbContext.SaveChanges();
    //     return RedirectToAction("Confirmation");
    // }

    // ✅ GOOD: Delegate to service
    [HttpPost]
    public IActionResult PlaceOrder(OrderViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model); // Return form with validation errors
        }

        _productService.PlaceOrder(model);
        return RedirectToAction(nameof(OrderConfirmation), new { id = 123 });
    }

    public IActionResult OrderConfirmation(int id)
    {
        var model = new ConfirmationViewModel { OrderId = id };
        return View(model);
    }

    // ✅ GOOD: Error handling
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var model = new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        };
        return View(model);
    }
}

/// <summary>
/// EXAMPLE 2: FORM HANDLING AND MODEL BINDING
/// 
/// THE PROBLEM:
/// Manual form parsing, inconsistent validation, no CSRF protection,
/// or accepting unvalidated input.
/// 
/// THE SOLUTION:
/// - Use model binding with strongly-typed models
/// - Data Annotations for validation
/// - ValidateAntiForgeryToken for POST actions
/// - Display validation errors in views
/// - PRG pattern (Post-Redirect-Get)
/// 
/// WHY IT MATTERS:
/// - Security (CSRF protection, prevent mass assignment)
/// - User experience (inline validation errors)
/// - Data integrity (validation before processing)
/// - Maintainability (declarative validation)
/// 
/// GOTCHA: Always use [ValidateAntiForgeryToken] on POST actions
/// </summary>
public class AccountController : Controller
{
    private readonly IUserService _userService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(IUserService userService, ILogger<AccountController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    // ✅ GOOD: GET action returns empty form
    [HttpGet]
    public IActionResult Register()
    {
        var model = new RegisterViewModel();
        return View(model);
    }

    // ✅ GOOD: POST with validation and CSRF protection    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            // Return view with validation errors
            return View(model);
        }

        // Check for existing user
        if (await _userService.UserExistsAsync(model.Email))
        {
            ModelState.AddModelError("Email", "Email is already registered");
            return View(model);
        }

        // Process registration
        await _userService.RegisterUserAsync(model);

        // PRG pattern: Redirect after POST
        TempData["Message"] = "Registration successful! Please check your email.";
        return RedirectToAction(nameof(Login));
    }

    // ✅ GOOD: Login with return URL
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        LoginViewModel model,
        string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userService.ValidateCredentialsAsync(model.Email, model.Password);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid email or password");
            return View(model);
        }

        // Sign in user (authentication)
        await _userService.SignInAsync(user);

        // Redirect to return URL or home
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _userService.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}

/// <summary>
/// EXAMPLE 3: PARTIAL VIEWS AND VIEW COMPONENTS
/// 
/// THE PROBLEM:
/// Duplicated view code, complex views, or mixing data access in views.
/// 
/// THE SOLUTION:
/// - Partial views for reusable UI fragments
/// - View components for logic + UI (like mini-controllers)
/// - Tag helpers for cleaner syntax
/// - Layout pages for common structure
/// 
/// WHY IT MATTERS:
/// - DRY principle in views
/// - Testable view logic (view components)
/// - Better maintainability
/// - Reusable UI components
/// 
/// TIP: Use partial views for static UI, view components for dynamic UI with logic
/// </summary>
public class ShopController : Controller
{
    // ✅ GOOD: Using partial views
    public IActionResult Products()
    {
        var model = new ProductListViewModel
        {
            Products = GetProducts(),
            Categories = GetCategories()
        };

        // View can render: @Html.Partial("_ProductCard", product)
        return View(model);
    }

    // ✅ GOOD: AJAX endpoint for partial refresh
    [HttpGet]
    public IActionResult SearchProducts(string query)
    {
        var products = SearchProductsByQuery(query);

        // Return partial view for AJAX
        return PartialView("_ProductGrid", products);
    }

    // ✅ GOOD: View component invocation
    public IActionResult Cart()
    {
        // In view: @await Component.InvokeAsync("ShoppingCart")
        // View component can have its own logic and data access
        return View();
    }

    private List<ProductViewModel> GetProducts() => new();
    private List<CategoryViewModel> GetCategories() => new();
    private List<ProductViewModel> SearchProductsByQuery(string query) => new();
}

// Example View Component
public class ShoppingCartViewComponent : ViewComponent
{
    private readonly ICartService _cartService;

    public ShoppingCartViewComponent(ICartService cartService)
    {
        _cartService = cartService;
    }

    // ✅ GOOD: View component with logic
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userId = HttpContext.User.FindFirst("sub")?.Value;
        if (userId == null)
        {
            return View("Empty");
        }

        var cart = await _cartService.GetCartAsync(userId);
        return View(cart);
    }
}

/// <summary>
/// EXAMPLE 4: FILTERS AND ACTION RESULTS
/// 
/// THE PROBLEM:
/// Duplicated authorization, logging, or error handling logic
/// across actions.
/// 
/// THE SOLUTION:
/// - Authorization filters for access control
/// - Action filters for cross-cutting concerns
/// - Result filters for response modification
/// - Exception filters for error handling
/// 
/// WHY IT MATTERS:
/// - DRY principle for cross-cutting concerns
/// - Centralized security and logging
/// - Consistent error handling
/// - Testable filter logic
/// </summary>
[Authorize] // Entire controller requires authentication
[ServiceFilter(typeof(LogActionFilter))]
public class AdminController : Controller
{
    // ✅ GOOD: Role-based authorization
    [Authorize(Roles = "Admin")]
    public IActionResult Dashboard()
    {
        var model = new DashboardViewModel();
        return View(model);
    }

    // ✅ GOOD: Policy-based authorization
    [Authorize(Policy = "RequireAdminPermission")]
    public IActionResult Users()
    {
        var model = new UserListViewModel();
        return View(model);
    }

    // ✅ GOOD: Custom action filter
    [LogPerformance]
    [HttpPost]
    public async Task<IActionResult> ImportData(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded");
        }

        await ProcessFileAsync(file);
        return RedirectToAction(nameof(Dashboard));
    }

    // ✅ GOOD: Different return types
    public IActionResult Download()
    {
        var bytes = GenerateReport();
        return File(bytes, "application/pdf", "report.pdf");
    }

    public IActionResult GetJson()
    {
        var data = new { message = "Success", timestamp = DateTime.UtcNow };
        return Json(data);
    }

    private async Task ProcessFileAsync(IFormFile file) => await Task.CompletedTask;
    private byte[] GenerateReport() => Array.Empty<byte>();
}

// Supporting classes and view models
public class HomeViewModel
{
    public string Title { get; set; } = "";
    public List<ProductViewModel> Products { get; set; } = new();
}

public class ProductViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public List<ProductViewModel> RelatedProducts { get; set; } = new();
    public ProductSummary? Product { get; set; }
}

public class ProductSummary
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}

public class OrderViewModel
{
    public int CustomerId { get; set; }
    public List<OrderItemViewModel> Items { get; set; } = new();
}

public class OrderItemViewModel
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class ConfirmationViewModel
{
    public int OrderId { get; set; }
}

public class ErrorViewModel
{
    public string RequestId { get; set; } = "";
}

public class RegisterViewModel
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = "";

    [Required]
    [EmailAddress]
    public string Email { get; set; } = "";

    [Required]
    [StringLength(100, MinimumLength = 8)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";

    [Required]
    [Compare(nameof(Password))]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = "";
}

public class LoginViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = "";

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";

    public bool RememberMe { get; set; }
}

public class ProductListViewModel
{
    public List<ProductViewModel> Products { get; set; } = new();
    public List<CategoryViewModel> Categories { get; set; } = new();
}

public class CategoryViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}

public class DashboardViewModel { }
public class UserListViewModel { }

// Interfaces
public interface IProductService
{
    List<ProductViewModel> GetFeaturedProducts();
    ProductSummary? GetProductById(int id);
    List<ProductViewModel> GetRelatedProducts(int id);
    void PlaceOrder(OrderViewModel model);
}

public interface IUserService
{
    Task<bool> UserExistsAsync(string email);
    Task RegisterUserAsync(RegisterViewModel model);
    Task<User?> ValidateCredentialsAsync(string email, string password);
    Task SignInAsync(User user);
    Task SignOutAsync();
}

public interface ICartService
{
    Task<object> GetCartAsync(string userId);
}

public class User
{
    public string Id { get; set; } = "";
    public string Email { get; set; } = "";
}

// Custom filter examples
public class LogActionFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context) { }
    public void OnActionExecuted(ActionExecutedContext context) { }
}

public class LogPerformanceAttribute : ActionFilterAttribute { }
public class AuthorizeAttribute : Attribute
{
    public string? Roles { get; set; }
    public string? Policy { get; set; }
}

public class Activity
{
    public static Activity? Current { get; set; }
    public string Id { get; set; } = "";
}
