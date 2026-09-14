# 🔐 Full-Stack Authentication & Authorization Architecture
> **Project:** ISkill.ly Platform  
> **Current Stack:** ASP.NET Core 7.0 Web API (with .NET 8 Architectural Evolution Analysis) | Angular 18 (SSR) | Microsoft SQL Server  
> **Author:** Elfetouri Zidan  

---

## 📌 Executive Summary
This document outlines the end-to-end security architecture implemented in the ISkill.ly platform. The system uses a decoupled, stateless **JSON Web Token (JWT)** workflow between an **Angular 18 Server-Side Rendered (SSR)** frontend and an **ASP.NET Core 7.0 Web API** backend powered by ASP.NET Core Identity.

```
+-----------------------------------------------------------------------------------+
|                                SECURITY ARCHITECTURE                              |
+-----------------------------------------------------------------------------------+
|                                                                                   |
|   [ Angular 18 Client (Browser/SSR) ]                                             |
|        │                                                                          |
|        ├── 1. Route Guards (authGuard): Checks token & role before routing.       |
|        ├── 2. SSR Pass-Through: Skips client storage checks on Node.js server.    |
|        ├── 3. HTTP Interceptor: Injects 'Authorization: Bearer <token>' header.   |
|        └── 4. Reactive State: AuthService (BehaviorSubject) prevents UI flicker.  |
|                                                                                   |
|                                       │ (HTTPS / REST API)                        |
|                                       ▼                                           |
|                                                                                   |
|   [ ASP.NET Core 7.0 Web API (Backend) ]                                          |
|        │                                                                          |
|        ├── 1. JWT Bearer Middleware: Validates signature, issuer, & expiry.       |
|        ├── 2. ClaimsPrincipal: Maps User ID, Email, and Roles (Writer/Reader).    |
|        ├── 3. Role-Based Auth: [Authorize(Roles = "Writer")] on protected APIs.   |
|        └── 4. Global Exception Middleware: Sanitizes errors, hides stack traces.  |

|                                                                                   |
|                                       │ (EF Core / Persistence)                   |
|                                       ▼                                           |
|                                                                                   |
|   [ SQL Server (Identity Database) ]                                              |
|        └── AspNetUsers, AspNetRoles, AspNetUserRoles (PBKDF2 Password Hashing)    |
+-----------------------------------------------------------------------------------+
```

---

## 1. Backend Security: ASP.NET Core 8

### A. JWT Configuration & Token Generation
Tokens are signed using `HMAC-SHA256` with secret keys configured via environment variables / secure settings.
Each generated token contains:
* `JwtRegisteredClaimNames.Sub` / `ClaimTypes.NameIdentifier`: User ID.
* `JwtRegisteredClaimNames.Email`: User email address.
* `ClaimTypes.Role`: User permissions (`Writer` for Admin, `Reader` for standard users).

```csharp
// Token generation snippet
var claims = new List<Claim>
{
    new Claim(ClaimTypes.NameIdentifier, user.Id),
    new Claim(ClaimTypes.Email, user.Email!)
};

foreach (var role in roles)
{
    claims.Add(new Claim(ClaimTypes.Role, role));
}

var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

var token = new JwtSecurityToken(
    issuer: _configuration["Jwt:Issuer"],
    audience: _configuration["Jwt:Audience"],
    claims: claims,
    expires: DateTime.UtcNow.AddMinutes(60),
    signingCredentials: credentials);
```

### B. Pipeline Validation
Authentication middleware is registered before authorization in `Program.cs`:

```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();
```

---

## 2. Frontend Security: Angular 18 (SSR & Browser)

### A. The SSR Hydration Challenge & Solution
In Angular Universal / SSR, the initial HTML is rendered on a Node.js server where `localStorage` is not available. 
* **The Problem:** A standard guard checking `localStorage` on the server throws exceptions or redirects authenticated users to `/login` during page load (causing a jarring screen flash).
* **The Solution:** Using `isPlatformServer(platformId)` to allow pass-through during SSR, deferring token evaluation to the browser hydration stage:

```typescript
export const authGuard: CanActivateFn = (route, state) => {
  const platformId = inject(PLATFORM_ID);
  const authService = inject(AuthServiceService);
  const router = inject(Router);

  // Allow SSR server rendering to pass through; browser validates token
  if (isPlatformServer(platformId)) {
    return true;
  }

  const user = authService.getUser();
  if (user && user.token) {
    return true;
  }

  router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
  return false;
};
```

### B. HTTP Token Interceptor
All outgoing API requests are intercepted. If a valid token exists, the `Authorization` header is automatically attached without polluting component logic:

```typescript
export const tokenInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthServiceService);
  const platformId = inject(PLATFORM_ID);

  if (isPlatformBrowser(platformId)) {
    const token = authService.getToken();
    if (token) {
      req = req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });
    }
  }

  return next(req);
};
```

---

## 3. Best Practices Implemented
1. **Zero Secret Leakage:** Database passwords and JWT secret keys are isolated in `appsettings.Development.json` and excluded via `.gitignore`.
2. **Defensive Error Handling:** A global middleware catches unhandled exceptions and returns standardized `ApiResponse<T>` objects, preventing internal server stack traces from exposing database schemas.
3. **Role-Driven UI:** UI action buttons (e.g. "Add Service", "Edit Category") are dynamically hidden/displayed based on decoded JWT claims, with server-side validation enforcing the same rules.
