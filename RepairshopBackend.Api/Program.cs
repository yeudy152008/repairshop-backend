using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RepairshopBackend.Application.Interfaces;
using RepairshopBackend.Domain.Entities;
using RepairshopBackend.Infrastructure.Data;
using RepairshopBackend.Infrastructure.Services;
using RepairshopBackend.Application.Security;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .WriteTo.Console(new CompactJsonFormatter())
    .WriteTo.File(new CompactJsonFormatter(), "logs/repairshop-.json", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Iniciando RepairshopBackend.Api");


    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = Microsoft.OpenApi.SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = Microsoft.OpenApi.ParameterLocation.Header,
            Description = "Ingrese el token JWT (sin la palabra 'Bearer', Swagger la agrega automáticamente).",
        });

        options.AddSecurityRequirement(document => new Microsoft.OpenApi.OpenApiSecurityRequirement
        {
            [new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
        });
    });

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<ICustomerService, CustomerService>();
    builder.Services.AddScoped<IVehicleService, VehicleService>();
    builder.Services.AddScoped<IWorkOrderService, WorkOrderService>();
    builder.Services.AddScoped<ICategoryService, CategoryService>();
    builder.Services.AddScoped<IInventoryService, InventoryService>();
    builder.Services.AddScoped<IRoleService, RoleService>();
    builder.Services.AddScoped<IAuditLogService, AuditLogService>();
    builder.Services.AddScoped<ISupplierService, SupplierService>();
    builder.Services.AddScoped<IPurchaseService, PurchaseService>();
    builder.Services.AddScoped<IInvoiceService, InvoiceService>();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAngularApp", policy =>
        {
            policy.WithOrigins(
                      "http://localhost:4200",
                      "http://repairshop-os-frontend-ygchdevs.s3-website-us-east-1.amazonaws.com",
                      "https://djlj0tn3dy7og.cloudfront.net"
                  )
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    });

    var jwtSettings = builder.Configuration.GetSection("Jwt");
    var signingKey = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(signingKey),
        };
    });

    builder.Services.AddAuthorization(options =>
    {
        foreach (var key in PermissionKeys.All)
        {
            options.AddPolicy(key, policy => policy.RequireClaim(PermissionKeys.ClaimType, key));
        }
    });

    var app = builder.Build();

    app.UseMiddleware<RepairshopBackend.Api.Middleware.ExceptionHandlingMiddleware>();
    app.UseSerilogRequestLogging();
    // Crea el usuario administrador inicial la primera vez que arranca la API
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (!db.Permissions.Any())
        {
            var permissions = new List<Permission>
        {
            new() { Key = "orders.read", Module = "orders", Label = "Leer Órdenes", Description = "Ver listado y detalles.", Danger = false },
            new() { Key = "orders.create", Module = "orders", Label = "Crear Órdenes", Description = "Ingresar nuevos trabajos.", Danger = false },
            new() { Key = "orders.update", Module = "orders", Label = "Editar Órdenes", Description = "Modificar estado y tareas.", Danger = false },
            new() { Key = "orders.delete", Module = "orders", Label = "Eliminar Órdenes", Description = "Borrado permanente.", Danger = true },

            new() { Key = "customers.read", Module = "customers", Label = "Leer Clientes", Description = "Ver directorio de clientes.", Danger = false },
            new() { Key = "customers.create", Module = "customers", Label = "Crear Clientes", Description = "Registrar nuevos clientes.", Danger = false },
            new() { Key = "customers.update", Module = "customers", Label = "Editar Clientes", Description = "Modificar datos existentes.", Danger = false },
            new() { Key = "customers.delete", Module = "customers", Label = "Eliminar Clientes", Description = "Borrado permanente.", Danger = true },

            new() { Key = "vehicles.read", Module = "vehicles", Label = "Leer Vehículos", Description = "Ver vehículos registrados.", Danger = false },
            new() { Key = "vehicles.create", Module = "vehicles", Label = "Crear Vehículos", Description = "Registrar nuevos vehículos.", Danger = false },
            new() { Key = "vehicles.update", Module = "vehicles", Label = "Editar Vehículos", Description = "Modificar datos de vehículos.", Danger = false },
            new() { Key = "vehicles.delete", Module = "vehicles", Label = "Eliminar Vehículos", Description = "Borrado permanente.", Danger = true },

            new() { Key = "inventory.read", Module = "inventory", Label = "Leer Inventario", Description = "Ver stock de repuestos y categorías.", Danger = false },
            new() { Key = "inventory.create", Module = "inventory", Label = "Crear Repuestos/Categorías", Description = "Agregar nuevos registros.", Danger = false },
            new() { Key = "inventory.update", Module = "inventory", Label = "Editar Inventario", Description = "Ajustar cantidades y precios.", Danger = false },
            new() { Key = "inventory.delete", Module = "inventory", Label = "Eliminar Inventario", Description = "Borrado permanente.", Danger = true },

            new() { Key = "suppliers.read", Module = "suppliers", Label = "Leer Proveedores", Description = "Ver listado de proveedores.", Danger = false },
            new() { Key = "suppliers.create", Module = "suppliers", Label = "Crear Proveedores", Description = "Registrar nuevos proveedores.", Danger = false },
            new() { Key = "suppliers.update", Module = "suppliers", Label = "Editar Proveedores", Description = "Modificar datos de proveedores.", Danger = false },
            new() { Key = "suppliers.delete", Module = "suppliers", Label = "Eliminar Proveedores", Description = "Borrado permanente.", Danger = true },

            new() { Key = "purchases.read", Module = "purchases", Label = "Leer Compras", Description = "Ver historial de compras.", Danger = false },
            new() { Key = "purchases.create", Module = "purchases", Label = "Registrar Compras", Description = "Registrar compras a proveedores.", Danger = false },

            new() { Key = "users.read", Module = "users", Label = "Leer Usuarios", Description = "Ver listado de usuarios.", Danger = false },
            new() { Key = "users.create", Module = "users", Label = "Crear Usuarios", Description = "Registrar nuevos usuarios.", Danger = false },
            new() { Key = "users.update", Module = "users", Label = "Editar Usuarios", Description = "Modificar datos y roles asignados.", Danger = false },
            new() { Key = "users.delete", Module = "users", Label = "Eliminar Usuarios", Description = "Borrado permanente.", Danger = true },

            new() { Key = "invoices.read", Module = "invoices", Label = "Leer Facturas", Description = "Ver listado de facturas y ventas.", Danger = false },
            new() { Key = "invoices.create", Module = "invoices", Label = "Crear Facturas", Description = "Generar ventas directas y facturar órdenes.", Danger = false },

            new() { Key = "logs.read", Module = "logs", Label = "Leer Bitácoras", Description = "Ver bitácoras de accesos y movimientos del sistema.", Danger = false },

            new() { Key = "roles.read", Module = "roles", Label = "Leer Roles", Description = "Ver listado de roles y sus permisos.", Danger = false },
            new() { Key = "roles.create", Module = "roles", Label = "Crear Roles", Description = "Registrar nuevos roles.", Danger = false },
            new() { Key = "roles.update", Module = "roles", Label = "Editar Roles", Description = "Modificar permisos de un rol.", Danger = false },
            new() { Key = "roles.delete", Module = "roles", Label = "Eliminar Roles", Description = "Borrado permanente.", Danger = true },

            new() { Key = "reports.read", Module = "reports", Label = "Leer Reportes", Description = "Ver reportes dinámicos del sistema.", Danger = false },
        };

            db.Permissions.AddRange(permissions);
            db.SaveChanges();
        }

        if (!db.Roles.Any())
        {
            var allPermissions = db.Permissions.ToList();

            var technicianKeys = new[]
            {
            "orders.read", "orders.create", "orders.update",
            "customers.read",
            "vehicles.read",
            "inventory.read", "inventory.create", "inventory.update",
        };

            var receptionKeys = new[]
            {
            "orders.read", "orders.create",
            "customers.read", "customers.create", "customers.update",
            "vehicles.read", "vehicles.create", "vehicles.update",
            "inventory.read",
            "invoices.read", "invoices.create",
        };

            Role BuildRole(string name, string description, IEnumerable<string>? allowedKeys)
            {
                var role = new Role { Name = name, Description = description };
                var keys = allowedKeys?.ToHashSet() ?? allPermissions.Select(p => p.Key).ToHashSet();

                foreach (var permission in allPermissions.Where(p => keys.Contains(p.Key)))
                {
                    role.RolePermissions.Add(new RolePermission { PermissionId = permission.Id });
                }

                return role;
            }

            db.Roles.AddRange(
                BuildRole("Administrador", "Acceso total al sistema.", null),
                BuildRole("Técnico", "Órdenes de trabajo e inventario básico.", technicianKeys),
                BuildRole("Recepción", "Registro de clientes, vehículos, órdenes y ventas.", receptionKeys)
            );
            db.SaveChanges();
        }

        if (!db.Users.Any())
        {
            db.Users.Add(new User
            {
                Username = "admin",
                FullName = "Administrador del Sistema",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Role = "Administrador",
                Active = true,
            });
            db.SaveChanges();
        }
    }

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseCors("AllowAngularApp");
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "La aplicación terminó inesperadamente durante el arranque");
}
finally
{
    Log.CloseAndFlush();
}