using alsafqa.Data.Context;
using alsafqa.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace alsafqa.Seed;

public static class DataSeeder
{
    private static readonly Guid AdminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    private static readonly Guid BrandArosaId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly Guid BrandLiptonId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    private static readonly Guid ProductArosa250Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
    private static readonly Guid ProductArosa500Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
    private static readonly Guid ProductLiptonId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");

    private static readonly Guid GalleryCatSeaId = Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffff1");
    private static readonly Guid GalleryCatAirId = Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffff2");
    private static readonly Guid GalleryCatLandId = Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffff3");

    private static readonly Guid GallerySea1Id = Guid.Parse("aaaaaaaa-1111-1111-1111-aaaaaaaaaaaa");
    private static readonly Guid GalleryAir1Id = Guid.Parse("bbbbbbbb-2222-2222-2222-bbbbbbbbbbbb");
    private static readonly Guid GalleryLand1Id = Guid.Parse("cccccccc-3333-3333-3333-cccccccccccc");

    public static async Task SeedAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await SeedRolesAsync(roleManager);
        await SeedUsersAsync(userManager);
        await SeedBrandsAndProductsAsync(db);
        await SeedGalleryAsync(db);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
    {
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>
            {
                Id = AdminRoleId,
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = AdminRoleId.ToString()
            });
        }
    }

    private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
    {
        await EnsureAdminAsync(
            userManager,
            email: "admin@alsafqa.com",
            password: "Admin123",
            fullName: "مدير النظام");
    }

    private static async Task EnsureAdminAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string password,
        string fullName)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = email,
                Email = email,
                FullName = fullName,
                EmailConfirmed = true,
                IsActive = true
            };

            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }

            return;
        }

        if (!await userManager.IsInRoleAsync(user, "Admin"))
        {
            await userManager.AddToRoleAsync(user, "Admin");
        }
    }

    private static async Task SeedBrandsAndProductsAsync(ApplicationDbContext db)
    {
        if (!await db.Brands.IgnoreQueryFilters().AnyAsync(b => b.Id == BrandArosaId))
        {
            db.Brands.Add(new Brand
            {
                Id = BrandArosaId,
                NameAr = "شاي العروسة",
                NameEn = "El Arosa Tea",
                ImageUrl = null,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await db.Brands.IgnoreQueryFilters().AnyAsync(b => b.Id == BrandLiptonId))
        {
            db.Brands.Add(new Brand
            {
                Id = BrandLiptonId,
                NameAr = "ليبتون",
                NameEn = "Lipton",
                ImageUrl = null,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
        }

        await db.SaveChangesAsync();

        if (!await db.Products.IgnoreQueryFilters().AnyAsync(p => p.Id == ProductArosa250Id))
        {
            db.Products.Add(new Product
            {
                Id = ProductArosa250Id,
                NameAr = "شاي العروسه 250جم * 80 عبوه",
                NameEn = "El Arosa Egyptian Tea 250g * 80 packs",
                DescriptionAr = "شاي العروسه المصري 250جرام * 80عبوه",
                DescriptionEn = "Egyptian El Arosa Tea 250 grams * 80 packs",
                WeightInGrams = 250,
                PackageCount = 80,
                BrandId = BrandArosaId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await db.Products.IgnoreQueryFilters().AnyAsync(p => p.Id == ProductArosa500Id))
        {
            db.Products.Add(new Product
            {
                Id = ProductArosa500Id,
                NameAr = "شاي العروسه 500جم * 40 عبوه",
                NameEn = "El Arosa Egyptian Tea 500g * 40 packs",
                DescriptionAr = "شاي العروسه المصري 500جرام * 40عبوه",
                DescriptionEn = "Egyptian El Arosa Tea 500 grams * 40 packs",
                WeightInGrams = 500,
                PackageCount = 40,
                BrandId = BrandArosaId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await db.Products.IgnoreQueryFilters().AnyAsync(p => p.Id == ProductLiptonId))
        {
            db.Products.Add(new Product
            {
                Id = ProductLiptonId,
                NameAr = "ليبتون أصفر ليبل 100 فتلة",
                NameEn = "Lipton Yellow Label 100 Tea Bags",
                DescriptionAr = "شاي ليبتون أصفر ليبل - عبوة 100 فتلة",
                DescriptionEn = "Lipton Yellow Label - pack of 100 tea bags",
                WeightInGrams = 200,
                PackageCount = 100,
                BrandId = BrandLiptonId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
        }

        await db.SaveChangesAsync();
    }

    private static async Task SeedGalleryAsync(ApplicationDbContext db)
    {
        if (!await db.GalleryCategories.IgnoreQueryFilters().AnyAsync(c => c.Id == GalleryCatSeaId))
        {
            db.GalleryCategories.Add(new GalleryCategory
            {
                Id = GalleryCatSeaId,
                NameAr = "شحن بحري",
                NameEn = "Sea Freight",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await db.GalleryCategories.IgnoreQueryFilters().AnyAsync(c => c.Id == GalleryCatAirId))
        {
            db.GalleryCategories.Add(new GalleryCategory
            {
                Id = GalleryCatAirId,
                NameAr = "شحن جوي",
                NameEn = "Air Freight",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await db.GalleryCategories.IgnoreQueryFilters().AnyAsync(c => c.Id == GalleryCatLandId))
        {
            db.GalleryCategories.Add(new GalleryCategory
            {
                Id = GalleryCatLandId,
                NameAr = "شحن بري",
                NameEn = "Land Freight",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
        }

        await db.SaveChangesAsync();

        if (!await db.Galleries.IgnoreQueryFilters().AnyAsync(g => g.Id == GallerySea1Id))
        {
            db.Galleries.Add(new Gallery
            {
                Id = GallerySea1Id,
                TitleAr = "عمليات الشحن البحري",
                TitleEn = "Sea Freight Operations",
                DescriptionAr = "شحن الحاويات عبر الموانئ العالمية",
                DescriptionEn = "Container shipping through global ports",
                GalleryCategoryId = GalleryCatSeaId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await db.Galleries.IgnoreQueryFilters().AnyAsync(g => g.Id == GalleryAir1Id))
        {
            db.Galleries.Add(new Gallery
            {
                Id = GalleryAir1Id,
                TitleAr = "عمليات الشحن الجوي",
                TitleEn = "Air Freight Operations",
                DescriptionAr = "شحن سريع عبر المطارات الدولية",
                DescriptionEn = "Fast shipping through international airports",
                GalleryCategoryId = GalleryCatAirId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await db.Galleries.IgnoreQueryFilters().AnyAsync(g => g.Id == GalleryLand1Id))
        {
            db.Galleries.Add(new Gallery
            {
                Id = GalleryLand1Id,
                TitleAr = "عمليات الشحن البري",
                TitleEn = "Land Freight Operations",
                DescriptionAr = "نقل بري آمن عبر الشاحنات",
                DescriptionEn = "Safe land transport via trucks",
                GalleryCategoryId = GalleryCatLandId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
        }

        await db.SaveChangesAsync();
    }
}
