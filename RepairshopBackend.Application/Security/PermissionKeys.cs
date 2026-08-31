namespace RepairshopBackend.Application.Security;

public static class PermissionKeys
{
    public const string ClaimType = "permission";

    public static readonly string[] All =
    {
        "orders.read", "orders.create", "orders.update", "orders.delete",
        "customers.read", "customers.create", "customers.update", "customers.delete",
        "vehicles.read", "vehicles.create", "vehicles.update", "vehicles.delete",
        "inventory.read", "inventory.create", "inventory.update", "inventory.delete",
        "suppliers.read", "suppliers.create", "suppliers.update", "suppliers.delete",
        "purchases.read", "purchases.create",
        "users.read", "users.create", "users.update", "users.delete",
        "invoices.read", "invoices.create",
        "logs.read",
        "roles.read", "roles.create", "roles.update", "roles.delete",
        "reports.read",
    };
}