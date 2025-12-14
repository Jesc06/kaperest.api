using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using KapeRest.Infrastructures.Persistence.Database;
using KapeRest.Core.Entities.Branch;
using KapeRest.Domain.Entities.SupplierEntities;
using KapeRest.Domain.Entities.InventoryEntities;
using KapeRest.Domain.Entities.MenuEntities;
using KapeRest.Core.Entities.MenuEntities;
using KapeRest.Core.Entities.Tax_Rate;
using KapeRest.Domain.Entities.AuditLogEntities;
using KapeRest.Core.Entities.SalesTransaction;

namespace KapeRest.Infrastructures.Persistence.Seeder
{
    public class DataSeeder
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public static async Task SeedAllData(
            ApplicationDbContext context,
            UserManager<UsersIdentity> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            // 1. Seed Roles
            await SeedRoles(roleManager);

            // 2. Seed Users (3 users: Admin, Staff, Cashier)
            var users = await SeedUsers(userManager);

            // 3. Seed Branches
            var branches = await SeedBranches(context);

            // 3.1. Assign branches to users (after branches are created)
            await AssignBranchesToUsers(userManager, users, branches);

            // 4. Seed Tax and Discounts
            await SeedTaxAndDiscounts(context);

            // 5. Seed Suppliers (20 suppliers)
            var suppliers = await SeedSuppliers(context, users);

            // 6. Seed Products (100 products)
            var products = await SeedProducts(context, suppliers, users, branches);

            // 7. Seed Menu Items (50 items with images from internet)
            var menuItems = await SeedMenuItems(context, users, branches);

            // 8. Seed Menu Item Products (linking menu items to products)
            await SeedMenuItemProducts(context, menuItems, products);

            // 9. Seed Stock Movements (track stock in/out)
            await SeedStockMovements(context, products, users, branches);

            // 10. Seed Sales Transactions (150 transactions)
            await SeedSalesTransactions(context, users, branches, menuItems);

            // 11. Seed Vouchers (10 vouchers)
            await SeedVouchers(context, users);

            // 12. Seed Audit Logs (50 logs)
            await SeedAuditLogs(context, users);

            await context.SaveChangesAsync();
        }

        private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "Staff", "Cashier" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private static async Task<Dictionary<string, UsersIdentity>> SeedUsers(UserManager<UsersIdentity> userManager)
        {
            var users = new Dictionary<string, UsersIdentity>();

            // Cashier User (create first para ma-link sa Staff)
            var cashier = await userManager.FindByEmailAsync("cashier@kaperest.com");
            if (cashier == null)
            {
                cashier = new UsersIdentity
                {
                    UserName = "cashier@kaperest.com",
                    Email = "cashier@kaperest.com",
                    FirstName = "Pedro",
                    MiddleName = "Garcia",
                    LastName = "Ramos",
                    EmailConfirmed = true,
                    PhoneNumber = "09191234567"
                    // BranchId will be assigned later after branches are created
                };
                var result = await userManager.CreateAsync(cashier, "Cashier@123");
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to create cashier user: {errors}");
                }
                await userManager.AddToRoleAsync(cashier, "Cashier");
            }
            else
            {
                // User exists, ensure password is correct
                var token = await userManager.GeneratePasswordResetTokenAsync(cashier);
                var resetResult = await userManager.ResetPasswordAsync(cashier, token, "Cashier@123");
                if (!resetResult.Succeeded)
                {
                    var errors = string.Join(", ", resetResult.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to reset cashier password: {errors}");
                }
                
                // Ensure role is assigned
                if (!await userManager.IsInRoleAsync(cashier, "Cashier"))
                {
                    await userManager.AddToRoleAsync(cashier, "Cashier");
                }
            }
            users["Cashier"] = cashier;

            // Admin User
            var admin = await userManager.FindByEmailAsync("admin@kaperest.com");
            if (admin == null)
            {
                admin = new UsersIdentity
                {
                    UserName = "admin@kaperest.com",
                    Email = "admin@kaperest.com",
                    FirstName = "Juan",
                    MiddleName = "Dela",
                    LastName = "Cruz",
                    EmailConfirmed = true,
                    PhoneNumber = "09171234567"
                };
                var result = await userManager.CreateAsync(admin, "Admin@123");
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to create admin user: {errors}");
                }
                await userManager.AddToRoleAsync(admin, "Admin");
            }
            else
            {
                // User exists, ensure password is correct
                var token = await userManager.GeneratePasswordResetTokenAsync(admin);
                var resetResult = await userManager.ResetPasswordAsync(admin, token, "Admin@123");
                if (!resetResult.Succeeded)
                {
                    var errors = string.Join(", ", resetResult.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to reset admin password: {errors}");
                }
                
                // Ensure role is assigned
                if (!await userManager.IsInRoleAsync(admin, "Admin"))
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
            users["Admin"] = admin;

            // Staff User (with assigned Cashier)
            var staff = await userManager.FindByEmailAsync("staff@kaperest.com");
            if (staff == null)
            {
                staff = new UsersIdentity
                {
                    UserName = "staff@kaperest.com",
                    Email = "staff@kaperest.com",
                    FirstName = "Maria",
                    MiddleName = "Santos",
                    LastName = "Reyes",
                    EmailConfirmed = true,
                    PhoneNumber = "09187654321",
                    CashierId = cashier.Id
                    // BranchId will be assigned later after branches are created
                };
                var result = await userManager.CreateAsync(staff, "Staff@123");
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to create staff user: {errors}");
                }
                await userManager.AddToRoleAsync(staff, "Staff");
            }
            else
            {
                // User exists, ensure password is correct
                var token = await userManager.GeneratePasswordResetTokenAsync(staff);
                var resetResult = await userManager.ResetPasswordAsync(staff, token, "Staff@123");
                if (!resetResult.Succeeded)
                {
                    var errors = string.Join(", ", resetResult.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to reset staff password: {errors}");
                }
                
                // Ensure role is assigned
                if (!await userManager.IsInRoleAsync(staff, "Staff"))
                {
                    await userManager.AddToRoleAsync(staff, "Staff");
                }
                
                // Update existing staff to have cashier
                if (staff.CashierId == null)
                {
                    staff.CashierId = cashier.Id;
                    await userManager.UpdateAsync(staff);
                }
            }
            users["Staff"] = staff;

            return users;
        }

        private static async Task AssignBranchesToUsers(
            UserManager<UsersIdentity> userManager, 
            Dictionary<string, UsersIdentity> users,
            List<BranchEntities> branches)
        {
            if (branches.Count == 0) return;

            var firstBranch = branches[0];
            
            // Assign Cashier to first branch
            var cashier = users["Cashier"];
            if (cashier.BranchId == null)
            {
                cashier.BranchId = firstBranch.Id;
                await userManager.UpdateAsync(cashier);
            }
            
            // Assign Staff to first branch
            var staff = users["Staff"];
            if (staff.BranchId == null)
            {
                staff.BranchId = firstBranch.Id;
                await userManager.UpdateAsync(staff);
            }
        }

        private static async Task<List<BranchEntities>> SeedBranches(ApplicationDbContext context)
        {
            if (await context.Branches.AnyAsync())
            {
                return await context.Branches.ToListAsync();
            }

            var branches = new List<BranchEntities>
            {
                new BranchEntities { BranchName = "KapeRest Manila", Location = "Manila City", Staff = "Active", Status = "Active" },
                new BranchEntities { BranchName = "KapeRest Quezon City", Location = "Quezon City", Staff = "Active", Status = "Active" },
                new BranchEntities { BranchName = "KapeRest Makati", Location = "Makati City", Staff = "Active", Status = "Active" },
                new BranchEntities { BranchName = "KapeRest Cebu", Location = "Cebu City", Staff = "Active", Status = "Active" },
                new BranchEntities { BranchName = "KapeRest Davao", Location = "Davao City", Staff = "Active", Status = "Active" }
            };

            context.Branches.AddRange(branches);
            await context.SaveChangesAsync();
            return branches;
        }

        private static async Task SeedTaxAndDiscounts(ApplicationDbContext context)
        {
            if (!await context.Tax.AnyAsync())
            {
                var taxes = new List<Tax>
                {
                    new Tax { TaxRate = "VAT", Value = 0.12m, Description = "Value Added Tax 12%" },
                    new Tax { TaxRate = "Service Charge", Value = 0.10m, Description = "Service Charge 10%" }
                };
                context.Tax.AddRange(taxes);
            }

            if (!await context.Discount.AnyAsync())
            {
                var discounts = new List<Discount>
                {
                    new Discount { TaxRate = "Senior Citizen", Value = 0.20m, Description = "20% Senior Citizen Discount" },
                    new Discount { TaxRate = "PWD", Value = 0.20m, Description = "20% PWD Discount" },
                    new Discount { TaxRate = "Student", Value = 0.10m, Description = "10% Student Discount" }
                };
                context.Discount.AddRange(discounts);
            }

            await context.SaveChangesAsync();
        }

        private static async Task<List<AddSupplier>> SeedSuppliers(ApplicationDbContext context, Dictionary<string, UsersIdentity> users)
        {
            if (await context.Suppliers.AnyAsync())
            {
                return await context.Suppliers.ToListAsync();
            }

            var suppliers = new List<AddSupplier>
            {
                new AddSupplier { SupplierName = "Coffee Beans PH", ContactPerson = "Roberto Santos", PhoneNumber = "09171112222", Email = "roberto@coffeebeans.ph", Address = "123 Coffee St, Manila", UserId = users["Admin"].Id },
                new AddSupplier { SupplierName = "Fresh Milk Co.", ContactPerson = "Ana Mendoza", PhoneNumber = "09182223333", Email = "ana@freshmilk.com", Address = "456 Dairy Ave, Quezon City", UserId = users["Admin"].Id },
                new AddSupplier { SupplierName = "Sugar Supply Inc.", ContactPerson = "Carlos Reyes", PhoneNumber = "09193334444", Email = "carlos@sugarsupply.com", Address = "789 Sweet Rd, Makati", UserId = users["Admin"].Id },
                new AddSupplier { SupplierName = "Bakery Ingredients", ContactPerson = "Lisa Cruz", PhoneNumber = "09174445555", Email = "lisa@bakery.com", Address = "321 Flour St, Pasig", UserId = users["Staff"].Id },
                new AddSupplier { SupplierName = "Tea Traders", ContactPerson = "Michael Tan", PhoneNumber = "09185556666", Email = "michael@teatraders.ph", Address = "654 Tea Lane, Taguig", UserId = users["Staff"].Id },
                new AddSupplier { SupplierName = "Chocolate Delights", ContactPerson = "Sarah Lee", PhoneNumber = "09196667777", Email = "sarah@chocolate.ph", Address = "987 Cocoa Dr, Mandaluyong", UserId = users["Admin"].Id },
                new AddSupplier { SupplierName = "Fruit Fresh", ContactPerson = "Diego Garcia", PhoneNumber = "09177778888", Email = "diego@fruitfresh.com", Address = "147 Fruit Ave, Marikina", UserId = users["Staff"].Id },
                new AddSupplier { SupplierName = "Syrup Solutions", ContactPerson = "Elena Ramos", PhoneNumber = "09188889999", Email = "elena@syrup.ph", Address = "258 Sweet St, Muntinlupa", UserId = users["Admin"].Id },
                new AddSupplier { SupplierName = "Ice Cold Supplies", ContactPerson = "Frank Torres", PhoneNumber = "09191110000", Email = "frank@icecold.com", Address = "369 Cold Rd, Paranaque", UserId = users["Staff"].Id },
                new AddSupplier { SupplierName = "Cup & Straw Co.", ContactPerson = "Grace Diaz", PhoneNumber = "09172221111", Email = "grace@cupstraw.ph", Address = "741 Package Ln, Las Pinas", UserId = users["Admin"].Id },
                new AddSupplier { SupplierName = "Flavor House", ContactPerson = "Henry Gomez", PhoneNumber = "09183332222", Email = "henry@flavorhouse.com", Address = "852 Taste St, Caloocan", UserId = users["Staff"].Id },
                new AddSupplier { SupplierName = "Organic Beans", ContactPerson = "Irene Castro", PhoneNumber = "09194443333", Email = "irene@organicbeans.ph", Address = "963 Green Ave, Valenzuela", UserId = users["Admin"].Id },
                new AddSupplier { SupplierName = "Matcha Masters", ContactPerson = "Jose Navarro", PhoneNumber = "09175554444", Email = "jose@matchamasters.com", Address = "159 Matcha Dr, Malabon", UserId = users["Staff"].Id },
                new AddSupplier { SupplierName = "Dairy Delight", ContactPerson = "Karen Villar", PhoneNumber = "09186665555", Email = "karen@dairydelight.ph", Address = "357 Milk Rd, Navotas", UserId = users["Admin"].Id },
                new AddSupplier { SupplierName = "Pastry Paradise", ContactPerson = "Luis Fernandez", PhoneNumber = "09197776666", Email = "luis@pastryparadise.com", Address = "753 Pastry Ln, San Juan", UserId = users["Staff"].Id },
                new AddSupplier { SupplierName = "Spice World", ContactPerson = "Monica Aquino", PhoneNumber = "09178887777", Email = "monica@spiceworld.ph", Address = "951 Spice St, Mandaluyong", UserId = users["Admin"].Id },
                new AddSupplier { SupplierName = "Natural Sweeteners", ContactPerson = "Nathan Bautista", PhoneNumber = "09189998888", Email = "nathan@naturalsweeteners.com", Address = "357 Sweet Ave, Pasay", UserId = users["Staff"].Id },
                new AddSupplier { SupplierName = "Coffee Equipment Pro", ContactPerson = "Olivia Santiago", PhoneNumber = "09190009999", Email = "olivia@coffeeequipment.ph", Address = "159 Machine Rd, Taguig", UserId = users["Admin"].Id },
                new AddSupplier { SupplierName = "Beverage Supplies", ContactPerson = "Paul Ramirez", PhoneNumber = "09171230000", Email = "paul@beveragesupplies.com", Address = "753 Drink Ln, BGC", UserId = users["Staff"].Id },
                new AddSupplier { SupplierName = "Kitchen Essentials", ContactPerson = "Queen Morales", PhoneNumber = "09182341111", Email = "queen@kitchenessentials.ph", Address = "951 Kitchen St, Ortigas", UserId = users["Admin"].Id }
            };

            context.Suppliers.AddRange(suppliers);
            await context.SaveChangesAsync();
            return suppliers;
        }

        private static async Task<List<ProductOfSupplier>> SeedProducts(
            ApplicationDbContext context,
            List<AddSupplier> suppliers,
            Dictionary<string, UsersIdentity> users,
            List<BranchEntities> branches)
        {
            if (await context.Products.AnyAsync())
            {
                return await context.Products.ToListAsync();
            }

            var products = new List<ProductOfSupplier>();
            var random = new Random();
            var productNames = new[]
            {
                "Arabica Coffee Beans", "Robusta Coffee Beans", "Fresh Milk", "Almond Milk", "Soy Milk", "Coconut Milk",
                "White Sugar", "Brown Sugar", "Honey", "Artificial Sweetener", "All Purpose Flour", "Bread Flour",
                "Green Tea Leaves", "Black Tea Leaves", "Chamomile Tea", "Earl Grey Tea", "Dark Chocolate", "Milk Chocolate",
                "White Chocolate", "Cocoa Powder", "Vanilla Extract", "Cinnamon Powder", "Nutmeg", "Cardamom",
                "Fresh Strawberries", "Fresh Blueberries", "Bananas", "Mangoes", "Caramel Syrup", "Vanilla Syrup",
                "Hazelnut Syrup", "Chocolate Syrup", "Ice Cubes", "Crushed Ice", "Whipping Cream", "Heavy Cream",
                "Paper Cups 8oz", "Paper Cups 12oz", "Paper Cups 16oz", "Plastic Straws", "Paper Straws", "Cup Lids",
                "Matcha Powder", "Taro Powder", "Ube Powder", "Coffee Creamer", "Condensed Milk", "Evaporated Milk",
                "Butter", "Cream Cheese", "Cheddar Cheese", "Mozzarella Cheese", "Croissant", "Danish Pastry",
                "Muffins", "Cookies", "Brownies", "Cinnamon Rolls", "Salt", "Black Pepper", "Cumin", "Paprika",
                "Agave Syrup", "Maple Syrup", "Coffee Filters", "Espresso Pods", "Coffee Grinder Blades", "Milk Frother",
                "Plastic Cups", "Glass Bottles", "Mason Jars", "Napkins", "Stirrers", "Takeout Bags",
                "Oat Milk", "Cashew Milk", "Peanut Butter", "Nutella", "Raspberry Syrup", "Mint Leaves",
                "Lemon", "Lime", "Orange", "Grapefruit", "Espresso Beans", "Decaf Coffee Beans",
                "Green Coffee Beans", "Instant Coffee", "Coffee Concentrate", "Cold Brew Concentrate",
                "Boba Pearls", "Popping Boba", "Jelly Cubes", "Pudding", "Whipped Cream Chargers", "CO2 Cartridges",
                "Cleaning Solution", "Sanitizer", "Dish Soap", "Hand Soap", "Paper Towels", "Trash Bags"
            };

            var units = new[] { "kg", "ml", "pcs", "g", "L" };

            for (int i = 0; i < 100; i++)
            {
                var supplier = suppliers[random.Next(suppliers.Count)];
                var user = users.Values.ElementAt(random.Next(users.Count));
                var branch = branches[random.Next(branches.Count)];

                products.Add(new ProductOfSupplier
                {
                    ProductName = productNames[i % productNames.Length] + (i >= productNames.Length ? $" Batch {i / productNames.Length + 1}" : ""),
                    Stocks = random.Next(50, 500),
                    Units = units[random.Next(units.Length)],
                    CostPrice = random.Next(50, 1000) + (decimal)random.NextDouble(),
                    TransactionDate = DateTime.UtcNow.AddDays(-random.Next(0, 90)),
                    SupplierId = supplier.Id,
                    CashierId = user.Id,
                    BranchId = branch.Id,
                    UserId = user.Id
                });
            }

            context.Products.AddRange(products);
            await context.SaveChangesAsync();
            return products;
        }

        private static async Task<List<MenuItem>> SeedMenuItems(
            ApplicationDbContext context,
            Dictionary<string, UsersIdentity> users,
            List<BranchEntities> branches)
        {
            if (await context.MenuItems.AnyAsync())
            {
                return await context.MenuItems.ToListAsync();
            }

            var menuItems = new List<MenuItem>();
            var random = new Random();

            // Coffee Menu Items with real image URLs
            var coffeeItems = new[]
            {
                new { Name = "Espresso", Price = 95m, Desc = "Strong and bold shot of pure coffee", ImageUrl = "https://images.unsplash.com/photo-1579992357154-faf4bde95b3d?w=400" },
                new { Name = "Americano", Price = 110m, Desc = "Espresso with hot water", ImageUrl = "https://images.unsplash.com/photo-1514432324607-a09d9b4aefdd?w=400" },
                new { Name = "Cappuccino", Price = 135m, Desc = "Espresso with steamed milk and foam", ImageUrl = "https://images.unsplash.com/photo-1572442388796-11668a67e53d?w=400" },
                new { Name = "Latte", Price = 140m, Desc = "Smooth espresso with steamed milk", ImageUrl = "https://images.unsplash.com/photo-1561882468-9110e03e0f78?w=400" },
                new { Name = "Caramel Macchiato", Price = 155m, Desc = "Vanilla and caramel espresso drink", ImageUrl = "https://images.unsplash.com/photo-1599458448411-a6ab6b564e18?w=400" },
                new { Name = "Mocha", Price = 150m, Desc = "Chocolate and espresso delight", ImageUrl = "https://images.unsplash.com/photo-1607260550778-aa9d29444ce1?w=400" },
                new { Name = "Flat White", Price = 145m, Desc = "Velvety microfoam espresso drink", ImageUrl = "https://images.unsplash.com/photo-1570968915860-54d5c301fa9f?w=400" },
                new { Name = "Cortado", Price = 120m, Desc = "Equal parts espresso and steamed milk", ImageUrl = "https://images.unsplash.com/photo-1556742031-c6961e8560b0?w=400" },
                new { Name = "Affogato", Price = 165m, Desc = "Espresso poured over vanilla ice cream", ImageUrl = "https://images.unsplash.com/photo-1563729784474-d77dbb933a9e?w=400" },
                new { Name = "Irish Coffee", Price = 180m, Desc = "Coffee with Irish whiskey and cream", ImageUrl = "https://images.unsplash.com/photo-1514066558159-fc8c737ef259?w=400" }
            };

            // Cold Coffee Items
            var coldCoffeeItems = new[]
            {
                new { Name = "Iced Americano", Price = 120m, Desc = "Chilled espresso with cold water", ImageUrl = "https://images.unsplash.com/photo-1517487881594-2787fef5ebf7?w=400" },
                new { Name = "Iced Latte", Price = 150m, Desc = "Cold espresso with milk over ice", ImageUrl = "https://images.unsplash.com/photo-1461023058943-07fcbe16d735?w=400" },
                new { Name = "Iced Mocha", Price = 160m, Desc = "Iced chocolate coffee drink", ImageUrl = "https://images.unsplash.com/photo-1578374173705-64e1cf6ca2e9?w=400" },
                new { Name = "Cold Brew", Price = 155m, Desc = "Smooth cold-steeped coffee", ImageUrl = "https://images.unsplash.com/photo-1517487881594-2787fef5ebf7?w=400" },
                new { Name = "Frappuccino", Price = 175m, Desc = "Blended ice coffee drink", ImageUrl = "https://images.unsplash.com/photo-1572490122747-3968b75cc699?w=400" },
                new { Name = "Vietnamese Iced Coffee", Price = 145m, Desc = "Strong coffee with condensed milk", ImageUrl = "https://images.unsplash.com/photo-1559056199-641a0ac8b55e?w=400" }
            };

            // Tea Items
            var teaItems = new[]
            {
                new { Name = "Green Tea", Price = 95m, Desc = "Classic Japanese green tea", ImageUrl = "https://images.unsplash.com/photo-1564890369478-c89ca6d9cde9?w=400" },
                new { Name = "Matcha Latte", Price = 165m, Desc = "Premium matcha with steamed milk", ImageUrl = "https://images.unsplash.com/photo-1536013432416-020a47e81c45?w=400" },
                new { Name = "Earl Grey Tea", Price = 100m, Desc = "Classic bergamot-flavored tea", ImageUrl = "https://images.unsplash.com/photo-1597481499750-3e6b22637e12?w=400" },
                new { Name = "Chamomile Tea", Price = 110m, Desc = "Soothing herbal tea", ImageUrl = "https://images.unsplash.com/photo-1594631252845-29fc4cc8cde9?w=400" },
                new { Name = "Thai Tea", Price = 130m, Desc = "Sweet and creamy Thai iced tea", ImageUrl = "https://images.unsplash.com/photo-1576092768241-dec231879fc3?w=400" },
                new { Name = "Milk Tea", Price = 135m, Desc = "Classic milk tea with pearls", ImageUrl = "https://images.unsplash.com/photo-1558857563-b608d7d5fb3d?w=400" },
                new { Name = "Taro Milk Tea", Price = 145m, Desc = "Purple yam flavored milk tea", ImageUrl = "https://images.unsplash.com/photo-1525385133512-2f3bdd039054?w=400" }
            };

            // Specialty Drinks
            var specialtyItems = new[]
            {
                new { Name = "Hot Chocolate", Price = 125m, Desc = "Rich and creamy chocolate drink", ImageUrl = "https://images.unsplash.com/photo-1542990253-0d0f5be5f0ed?w=400" },
                new { Name = "Chai Latte", Price = 140m, Desc = "Spiced tea latte", ImageUrl = "https://images.unsplash.com/photo-1578899952107-9d9d0d36a5d6?w=400" },
                new { Name = "Strawberry Smoothie", Price = 155m, Desc = "Fresh strawberry blended drink", ImageUrl = "https://images.unsplash.com/photo-1505252585461-04db1eb84625?w=400" },
                new { Name = "Mango Smoothie", Price = 155m, Desc = "Tropical mango smoothie", ImageUrl = "https://images.unsplash.com/photo-1600271886742-f049cd451bba?w=400" },
                new { Name = "Banana Shake", Price = 145m, Desc = "Creamy banana milkshake", ImageUrl = "https://images.unsplash.com/photo-1623065422902-30a2d299bbe4?w=400" },
                new { Name = "Oreo Frappe", Price = 170m, Desc = "Cookies and cream blended drink", ImageUrl = "https://images.unsplash.com/photo-1572490122747-3968b75cc699?w=400" }
            };

            // Pastries
            var pastryItems = new[]
            {
                new { Name = "Croissant", Price = 85m, Desc = "Buttery flaky pastry", ImageUrl = "https://images.unsplash.com/photo-1555507036-ab1f4038808a?w=400" },
                new { Name = "Chocolate Croissant", Price = 95m, Desc = "Croissant filled with chocolate", ImageUrl = "https://images.unsplash.com/photo-1623334044303-241021148842?w=400" },
                new { Name = "Blueberry Muffin", Price = 75m, Desc = "Fresh blueberry muffin", ImageUrl = "https://images.unsplash.com/photo-1607958996333-41aef7caefaa?w=400" },
                new { Name = "Chocolate Chip Cookie", Price = 60m, Desc = "Classic chocolate chip cookie", ImageUrl = "https://images.unsplash.com/photo-1558961363-fa8fdf82db35?w=400" },
                new { Name = "Brownie", Price = 80m, Desc = "Rich chocolate brownie", ImageUrl = "https://images.unsplash.com/photo-1607920591413-4ec007e70023?w=400" },
                new { Name = "Cinnamon Roll", Price = 90m, Desc = "Sweet cinnamon pastry", ImageUrl = "https://images.unsplash.com/photo-1550617931-e17a7b70dce2?w=400" },
                new { Name = "Cheesecake Slice", Price = 120m, Desc = "Creamy New York cheesecake", ImageUrl = "https://images.unsplash.com/photo-1533134486753-c833f0ed4866?w=400" },
                new { Name = "Carrot Cake", Price = 110m, Desc = "Moist carrot cake with cream cheese frosting", ImageUrl = "https://images.unsplash.com/photo-1621303837174-89787a7d4729?w=400" }
            };

            // Combine all items
            var allItems = coffeeItems.Concat(coldCoffeeItems).Concat(teaItems).Concat(specialtyItems).Concat(pastryItems).ToArray();

            for (int i = 0; i < allItems.Length; i++)
            {
                var item = allItems[i];
                var user = users["Cashier"];
                var branch = branches[random.Next(branches.Count)];
                var category = i < 10 ? "Hot Coffee" :
                              i < 16 ? "Cold Coffee" :
                              i < 23 ? "Tea" :
                              i < 29 ? "Specialty" : "Pastry";

                byte[]? imageBytes = null;
                try
                {
                    var response = await _httpClient.GetAsync(item.ImageUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        imageBytes = await response.Content.ReadAsByteArrayAsync();
                    }
                }
                catch
                {
                    // If image download fails, continue without image
                }

                var menuItem = new MenuItem
                {
                    ItemName = item.Name,
                    Price = item.Price, // Base price (Small)
                    Category = category,
                    Description = item.Desc,
                    IsAvailable = random.Next(0, 10) > 0 ? "Available" : "Unavailable",
                    Image = imageBytes,
                    CashierId = user.Id,
                    BranchId = branch.Id
                };

                // Add size variations for beverages (not for pastries)
                if (category != "Pastry")
                {
                    menuItem.MenuItemSizes = new List<MenuItemSize>
                    {
                        new MenuItemSize
                        {
                            Size = "Small",
                            Price = item.Price,
                            IsAvailable = true
                        },
                        new MenuItemSize
                        {
                            Size = "Medium",
                            Price = item.Price + 20m, // +20 pesos for medium
                            IsAvailable = true
                        },
                        new MenuItemSize
                        {
                            Size = "Large",
                            Price = item.Price + 40m, // +40 pesos for large
                            IsAvailable = true
                        }
                    };
                }
                else
                {
                    // Pastries only have one size
                    menuItem.MenuItemSizes = new List<MenuItemSize>
                    {
                        new MenuItemSize
                        {
                            Size = "Regular",
                            Price = item.Price,
                            IsAvailable = true
                        }
                    };
                }

                menuItems.Add(menuItem);
            }

            context.MenuItems.AddRange(menuItems);
            await context.SaveChangesAsync();
            return menuItems;
        }

        private static async Task SeedMenuItemProducts(
            ApplicationDbContext context,
            List<MenuItem> menuItems,
            List<ProductOfSupplier> products)
        {
            if (await context.MenuItemProducts.AnyAsync())
            {
                return;
            }

            var menuItemProducts = new List<MenuItemProduct>();
            var random = new Random();

            foreach (var menuItem in menuItems)
            {
                // Each menu item uses 2-5 products
                var numberOfProducts = random.Next(2, 6);
                var selectedProducts = products.OrderBy(x => random.Next()).Take(numberOfProducts).ToList();

                foreach (var product in selectedProducts)
                {
                    menuItemProducts.Add(new MenuItemProduct
                    {
                        MenuItemId = menuItem.Id,
                        ProductOfSupplierId = product.Id,
                        QuantityUsed = random.Next(1, 10)
                    });
                }
            }

            context.MenuItemProducts.AddRange(menuItemProducts);
            await context.SaveChangesAsync();
        }

        private static async Task SeedStockMovements(
            ApplicationDbContext context,
            List<ProductOfSupplier> products,
            Dictionary<string, UsersIdentity> users,
            List<BranchEntities> branches)
        {
            if (await context.StockMovements.AnyAsync())
            {
                return;
            }

            var stockMovements = new List<StockMovement>();
            var random = new Random();
            var movementTypes = new[] { "Stock In", "Stock Out" };
            var reasons = new Dictionary<string, string[]>
            {
                { "Stock In", new[] { "Purchase", "Return from Customer", "Adjustment - Increase", "Transfer In", "Initial Stock" } },
                { "Stock Out", new[] { "Sale", "Damaged", "Expired", "Transfer Out", "Adjustment - Decrease", "Used in Production" } }
            };

            // Generate 200 stock movements over the past 60 days
            foreach (var product in products)
            {
                // Each product will have 2-5 movements
                var movementCount = random.Next(2, 6);
                
                for (int i = 0; i < movementCount; i++)
                {
                    var movementType = movementTypes[random.Next(movementTypes.Length)];
                    var reasonList = reasons[movementType];
                    var reason = reasonList[random.Next(reasonList.Length)];
                    
                    var user = users.Values.ElementAt(random.Next(users.Count));
                    var branch = branches[random.Next(branches.Count)];
                    
                    // Stock In typically has larger quantities than Stock Out
                    var quantity = movementType == "Stock In" 
                        ? random.Next(50, 200) 
                        : random.Next(5, 50);
                    
                    stockMovements.Add(new StockMovement
                    {
                        ProductId = product.Id,
                        MovementType = movementType,
                        Quantity = quantity,
                        UnitPrice = product.CostPrice,
                        Reason = reason,
                        TransactionDate = DateTime.UtcNow.AddDays(-random.Next(0, 60)).AddHours(-random.Next(0, 24)),
                        UserId = user.Id,
                        BranchId = branch.Id
                    });
                }
            }

            // Sort by date
            stockMovements = stockMovements.OrderBy(x => x.TransactionDate).ToList();
            
            context.StockMovements.AddRange(stockMovements);
            await context.SaveChangesAsync();
        }

        private static async Task SeedSalesTransactions(
            ApplicationDbContext context,
            Dictionary<string, UsersIdentity> users,
            List<BranchEntities> branches,
            List<MenuItem> menuItems)
        {
            if (await context.SalesTransaction.AnyAsync())
            {
                return;
            }

            var transactions = new List<SalesTransactionEntities>();
            var salesItems = new List<SalesItemEntities>();
            var random = new Random();
            var paymentMethods = new[] { "Cash", "GCash", "PayMaya", "Card" };
            var statuses = new[] { "Completed", "Completed", "Completed", "Pending", "Cancelled" };
            var sizes = new[] { "Small", "Medium", "Large" };
            var sugarLevels = new[] { "0%", "25%", "50%", "75%", "100%" };

            // Define peak hours with probability weights (higher = more transactions)
            // Peak hours: 7-9 AM (breakfast), 12-1 PM (lunch), 3-5 PM (afternoon coffee)
            var hourWeights = new Dictionary<int, int>
            {
                { 6, 3 },   // 6 AM - early birds
                { 7, 15 },  // 7 AM - morning rush start
                { 8, 20 },  // 8 AM - peak breakfast
                { 9, 12 },  // 9 AM - late breakfast
                { 10, 6 },  // 10 AM - mid-morning
                { 11, 8 },  // 11 AM - pre-lunch
                { 12, 18 }, // 12 PM - lunch rush
                { 13, 10 }, // 1 PM - late lunch
                { 14, 5 },  // 2 PM - afternoon lull
                { 15, 14 }, // 3 PM - afternoon coffee break
                { 16, 16 }, // 4 PM - peak afternoon
                { 17, 10 }, // 5 PM - after work
                { 18, 7 },  // 6 PM - early evening
                { 19, 4 },  // 7 PM - evening
                { 20, 2 }   // 8 PM - closing time
            };

            // Generate more transactions (150 transactions over 30 days)
            // Mix of transactions between Cashier and Staff users
            for (int i = 0; i < 150; i++)
            {
                // 50% Cashier, 50% Staff transactions
                var cashier = i % 2 == 0 ? users["Cashier"] : users["Staff"];
                var branch = branches[random.Next(branches.Count)];
                var itemCount = random.Next(1, 5);
                var transactionItems = menuItems.OrderBy(x => random.Next()).Take(itemCount).ToList();

                decimal subtotal = 0;
                var transactionId = i + 1;

                // Generate realistic datetime based on peak hours
                var daysAgo = random.Next(0, 30);
                var baseDate = DateTime.UtcNow.AddDays(-daysAgo);
                
                // Select hour based on weights (more transactions during peak hours)
                var totalWeight = hourWeights.Values.Sum();
                var randomWeight = random.Next(totalWeight);
                int selectedHour = 12; // default to noon
                int currentWeight = 0;
                
                foreach (var kvp in hourWeights)
                {
                    currentWeight += kvp.Value;
                    if (randomWeight < currentWeight)
                    {
                        selectedHour = kvp.Key;
                        break;
                    }
                }
                
                // Add random minutes and seconds
                var transactionDateTime = new DateTime(
                    baseDate.Year, 
                    baseDate.Month, 
                    baseDate.Day, 
                    selectedHour, 
                    random.Next(0, 60), 
                    random.Next(0, 60),
                    DateTimeKind.Utc
                );

                foreach (var item in transactionItems)
                {
                    var quantity = random.Next(1, 4);
                    var size = item.Category == "Pastry" ? "Regular" : sizes[random.Next(sizes.Length)];
                    var sugarLevel = item.Category == "Pastry" ? "N/A" : sugarLevels[random.Next(sugarLevels.Length)];
                    
                    // Calculate price based on size
                    decimal itemPrice = item.Price;
                    if (size == "Medium") itemPrice += 20m;
                    else if (size == "Large") itemPrice += 40m;
                    
                    var itemTotal = itemPrice * quantity;
                    subtotal += itemTotal;

                    salesItems.Add(new SalesItemEntities
                    {
                        SalesTransactionId = transactionId,
                        MenuItemId = item.Id,
                        Size = size,
                        SugarLevel = sugarLevel,
                        Quantity = quantity,
                        UnitPrice = itemPrice
                    });
                }

                var tax = subtotal * 0.12m;
                var discount = random.Next(0, 3) == 0 ? subtotal * 0.10m : 0;
                var total = subtotal + tax - discount;
                var status = statuses[random.Next(statuses.Length)];

                transactions.Add(new SalesTransactionEntities
                {
                    ReceiptNumber = $"REC{DateTime.Now.Year}{(i + 1):D6}",
                    MenuItemName = string.Join(", ", transactionItems.Select(x => x.ItemName)),
                    DateTime = transactionDateTime,
                    CashierId = cashier.Id,
                    BranchId = branch.Id,
                    Subtotal = subtotal,
                    Tax = tax,
                    Discount = discount,
                    Total = total,
                    PaymentMethod = paymentMethods[random.Next(paymentMethods.Length)],
                    Status = status,
                    Reason = status == "Cancelled" ? "Customer request" : null
                });
            }

            context.SalesTransaction.AddRange(transactions);
            await context.SaveChangesAsync();

            // Update transaction IDs for sales items
            var savedTransactions = await context.SalesTransaction.ToListAsync();
            for (int i = 0; i < salesItems.Count; i++)
            {
                salesItems[i].SalesTransactionId = savedTransactions[salesItems[i].SalesTransactionId - 1].Id;
            }

            context.SalesItems.AddRange(salesItems);
            await context.SaveChangesAsync();
        }

        private static async Task SeedVouchers(ApplicationDbContext context, Dictionary<string, UsersIdentity> users)
        {
            if (await context.Vouchers.AnyAsync())
            {
                return;
            }

            var vouchers = new List<KapeRest.Core.Entities.VoucherEntities.Voucher>();
            var random = new Random();
            var admin = users["Admin"];

            // Create 10 different vouchers with various states
            var voucherData = new[]
            {
                new { Code = "WELCOME10", Discount = 10m, MaxUses = 100, CurrentUses = 15, DaysUntilExpiry = 30, Description = "Welcome discount for new customers", IsActive = true },
                new { Code = "COFFEE20", Discount = 20m, MaxUses = 50, CurrentUses = 45, DaysUntilExpiry = 15, Description = "20% off all coffee drinks", IsActive = true },
                new { Code = "FREEPASTRY", Discount = 15m, MaxUses = 30, CurrentUses = 28, DaysUntilExpiry = 7, Description = "15% off on pastries", IsActive = true },
                new { Code = "MONDAY25", Discount = 25m, MaxUses = 200, CurrentUses = 50, DaysUntilExpiry = 45, Description = "Monday special discount", IsActive = true },
                new { Code = "LOYALTY50", Discount = 50m, MaxUses = 20, CurrentUses = 8, DaysUntilExpiry = 60, Description = "Loyalty reward for regular customers", IsActive = true },
                new { Code = "EXPIRED15", Discount = 15m, MaxUses = 100, CurrentUses = 45, DaysUntilExpiry = -5, Description = "Expired promotional voucher", IsActive = true },
                new { Code = "FULLUSED", Discount = 10m, MaxUses = 25, CurrentUses = 25, DaysUntilExpiry = 20, Description = "Fully used voucher", IsActive = true },
                new { Code = "INACTIVE20", Discount = 20m, MaxUses = 100, CurrentUses = 10, DaysUntilExpiry = 30, Description = "Deactivated voucher", IsActive = false },
                new { Code = "NOEXPIRY", Discount = 5m, MaxUses = 1000, CurrentUses = 150, DaysUntilExpiry = 0, Description = "Voucher with no expiry date", IsActive = true },
                new { Code = "STUDENT15", Discount = 15m, MaxUses = 500, CurrentUses = 120, DaysUntilExpiry = 90, Description = "Student discount voucher", IsActive = true }
            };

            foreach (var data in voucherData)
            {
                DateTime? expiryDate = null;
                if (data.DaysUntilExpiry != 0)
                {
                    expiryDate = DateTime.UtcNow.AddDays(data.DaysUntilExpiry);
                }

                vouchers.Add(new KapeRest.Core.Entities.VoucherEntities.Voucher
                {
                    Code = data.Code,
                    DiscountPercent = data.Discount,
                    MaxUses = data.MaxUses,
                    CurrentUses = data.CurrentUses,
                    IsActive = data.IsActive,
                    ExpiryDate = expiryDate,
                    CreatedDate = DateTime.UtcNow.AddDays(-random.Next(60, 120)),
                    CreatedBy = admin.Id,
                    Description = data.Description,
                    IsCustomerSpecific = false,
                    CustomerId = null
                });
            }

            context.Vouchers.AddRange(vouchers);
            await context.SaveChangesAsync();
        }

        private static async Task SeedAuditLogs(ApplicationDbContext context, Dictionary<string, UsersIdentity> users)
        {
            if (await context.AuditLog.AnyAsync())
            {
                return;
            }

            var auditLogs = new List<AuditLogEntities>();
            var random = new Random();
            var actions = new[] { "Add", "Update", "Delete", "View", "Login", "Logout", "Create", "Approve", "Reject" };
            var descriptions = new[]
            {
                "Added new menu item",
                "Updated product stock",
                "Deleted old supplier",
                "Viewed sales report",
                "Logged into system",
                "Logged out of system",
                "Created new branch",
                "Approved pending account",
                "Rejected pending request",
                "Updated menu item price",
                "Added new supplier",
                "Delivered 50 units of coffee beans",
                "Updated branch information",
                "Deleted expired product",
                "Viewed inventory report"
            };

            for (int i = 0; i < 50; i++)
            {
                var user = users.Values.ElementAt(random.Next(users.Count));
                var roles = new[] { "Admin", "Staff", "Cashier" };

                auditLogs.Add(new AuditLogEntities
                {
                    Username = user.Email ?? "Unknown",
                    Role = roles[random.Next(roles.Length)],
                    Action = actions[random.Next(actions.Length)],
                    Description = descriptions[random.Next(descriptions.Length)],
                    Date = DateTime.UtcNow.AddDays(-random.Next(0, 60)).AddHours(-random.Next(0, 24))
                });
            }

            context.AuditLog.AddRange(auditLogs);
            await context.SaveChangesAsync();
        }
    }
}
