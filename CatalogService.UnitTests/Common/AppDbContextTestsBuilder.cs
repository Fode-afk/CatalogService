using CatalogService.Application.Interfaces.Data;
using CatalogService.Domain.Models;
using CatalogService.Domain.Snapshots;
using MockQueryable.NSubstitute;
using NSubstitute;

namespace CatalogService.UnitTests.Common;

public sealed class AppDbContextTestsBuilder
{
    private readonly List<VendorSnapshot> _vendorSnapshots = [];
    private readonly List<BrandSnapshot> _brandSnapshots = [];
    private readonly List<CategorySnapshot> _categorySnapshots = [];
    private readonly List<CharacteristicSnapshot> _characteristicSnapshots = [];
    private readonly List<ProductVariantSnapshot> _variantSnapshots = [];
    private readonly List<ProductVariantPriceSnapshot> _priceSnapshots = [];
    private readonly List<Product> _products = [];

    public AppDbContextTestsBuilder WithVendor(VendorSnapshot v) { _vendorSnapshots.Add(v); return this; }
    public AppDbContextTestsBuilder WithBrand(BrandSnapshot b) { _brandSnapshots.Add(b); return this; }
    public AppDbContextTestsBuilder WithCategory(CategorySnapshot c) { _categorySnapshots.Add(c); return this; }
    public AppDbContextTestsBuilder WithCharacteristic(CharacteristicSnapshot c) { _characteristicSnapshots.Add(c); return this; }
    public AppDbContextTestsBuilder WithVariant(ProductVariantSnapshot v) { _variantSnapshots.Add(v); return this; }
    public AppDbContextTestsBuilder WithPrice(ProductVariantPriceSnapshot p) { _priceSnapshots.Add(p); return this; }
    public AppDbContextTestsBuilder WithProducts(params Product[] p) { _products.AddRange(p); return this; }

    public IAppDbContext Build()
    {
        var context = Substitute.For<IAppDbContext>();

        var vendorSet = _vendorSnapshots.BuildMockDbSet();
        var brandSet = _brandSnapshots.BuildMockDbSet();
        var categorySet = _categorySnapshots.BuildMockDbSet();
        var characteristicSet = _characteristicSnapshots.BuildMockDbSet();
        var variantSet = _variantSnapshots.BuildMockDbSet();
        var priceSet = _priceSnapshots.BuildMockDbSet();
        var productSet = _products.BuildMockDbSet();

        context.VendorSnapshots.Returns(vendorSet);
        context.BrandSnapshots.Returns(brandSet);
        context.CategorySnapshots.Returns(categorySet);
        context.CharacteristicSnapshots.Returns(characteristicSet);
        context.ProductVariantSnapshots.Returns(variantSet);
        context.ProductVariantPriceSnapshots.Returns(priceSet);
        context.Products.Returns(productSet);

        return context;
    }
}
