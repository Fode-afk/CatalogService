using CatalogService.Domain.Models;
using CatalogService.Domain.RequestData;
using CatalogService.Domain.ValueObjects;
using migApp.Shared.Enums.Characteristics;

namespace CatalogService.TestCommon.Fixtures;

public static class ProductDataTestFactory
{
    public static ProductAttribute CreateAttribute(Guid characteristicId, bool isUnifying = false) =>
        ProductAttribute.Create(
            characteristicId,
            AttributeName.Create("Attr").Value,
            AttributeValue.Create("Value").Value,
            AttributeCharType.Text,
            isUnifying: isUnifying).Value;

    public static ProductAttribute CreateVariableAttribute(Guid characteristicId) =>
        ProductAttribute.CreateVariable(
            characteristicId,
            AttributeName.Create("Attr").Value,
            AttributeCharType.Text).Value;

    public static (
        Guid CharacteristicId,
        AttributeName Name,
        AttributeCharType CharType,
        AttributeGroupName? GroupName,
        AttributeVariableValue VariableValue)
        CreateVariantValue(
            Guid? characteristicId = null,
            string name = "Color",
            AttributeCharType charType = AttributeCharType.Text,
            AttributeGroupName? groupName = null,
            string value = "Red",
            Guid? valueId = null)
    {
        return (
            characteristicId ?? Guid.NewGuid(),
            AttributeName.Create(name).Value,
            charType,
            groupName,
            new AttributeVariableValue(value, valueId));
    }

    public static ProductCreationData CreateData(string suffix = "")
    {
        return new ProductCreationData(
            ProductName.Create($"Name{suffix}").Value,
            Slug.Create($"slug{suffix}").Value,
            Description.Create($"Description{suffix}").Value,
            ShortDescription.Create($"Short{suffix}").Value,
            SeoMetadata.Create(
                SeoTitle.Create($"Meta title{suffix}").Value,
                SeoDescription.Create($"Meta desc{suffix}").Value,
                SeoKeywords.Create($"keyword{suffix}").Value).Value);
    }

    public static ProductUpdateInfoData UpdateInfoData(string suffix = "")
    {
        return new ProductUpdateInfoData(
            ProductName.Create($"Name{suffix}").Value,
            Slug.Create($"slug{suffix}").Value,
            Description.Create($"Description{suffix}").Value,
            ShortDescription.Create($"Short{suffix}").Value,
            SeoMetadata.Create(
                SeoTitle.Create($"Meta title{suffix}").Value,
                SeoDescription.Create($"Meta desc{suffix}").Value,
                SeoKeywords.Create($"keyword{suffix}").Value).Value);
    }
}
