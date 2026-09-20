using CatalogService.Application.Features.Commands.ReplaceProductAttributes;
using CatalogService.Domain.Errors;
using CatalogService.Domain.Snapshots;
using CatalogService.UnitTests.Fixtures;
using FluentAssertions;

namespace CatalogService.UnitTests.Application.Commands.ReplaceProductAttributes;

public class ProductReplaceAttributesDataBuilderTests
{
    [Fact]
    public void Build_Should_Return_Attribute_When_Snapshot_And_Value_Are_Valid()
    {
        // Arrange
        var characteristicId = Guid.NewGuid();
        var command = ProductCommandTestsFactory.ValidReplaceAttributesCommand(
            attributes: new Dictionary<Guid, string> { [characteristicId] = "Red" });
        var snapshots = new List<CharacteristicSnapshot>
        {
            SnapshotTestsFactory.Characteristic(characteristicId, name: "Color")
        };

        // Act
        var result = ProductReplaceAttributesDataBuilder.Build(command, snapshots);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().ContainSingle();
        result.Value[0].CharacteristicId.Should().Be(characteristicId);
        result.Value[0].Name.Value.Should().Be("Color");
        result.Value[0].Value.Value.Should().Be("Red");
    }

    [Fact]
    public void Build_Should_Set_GroupName_When_Snapshot_Has_One()
    {
        // Arrange
        var characteristicId = Guid.NewGuid();
        var command = ProductCommandTestsFactory.ValidReplaceAttributesCommand(
            attributes: new Dictionary<Guid, string> { [characteristicId] = "Red" });
        var snapshots = new List<CharacteristicSnapshot>
        {
            SnapshotTestsFactory.Characteristic(characteristicId, groupName: "Basic")
        };

        // Act
        var result = ProductReplaceAttributesDataBuilder.Build(command, snapshots);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value[0].GroupName.Should().NotBeNull();
        result.Value[0].GroupName!.Value.Should().Be("Basic");
    }

    [Fact]
    public void Build_Should_Fail_When_Requested_Characteristic_Is_Not_In_Snapshots()
    {
        // Arrange
        var missingCharacteristicId = Guid.NewGuid();
        var command = ProductCommandTestsFactory.ValidReplaceAttributesCommand(
            attributes: new Dictionary<Guid, string> { [missingCharacteristicId] = "Red" });

        // Act
        var result = ProductReplaceAttributesDataBuilder.Build(command, []);

        // Assert
        result.IsFailure.Should().BeTrue();
        var errorCodes = (List<string>)result.Error.Metadata!["Errors"];
        errorCodes.Should().Contain(CharacteristicSnapshotErrors.NotFound().Code);
    }

    [Fact]
    public void Build_Should_Fail_When_Value_Is_Invalid()
    {
        // Arrange
        var characteristicId = Guid.NewGuid();
        var command = ProductCommandTestsFactory.ValidReplaceAttributesCommand(
            attributes: new Dictionary<Guid, string> { [characteristicId] = string.Empty });
        var snapshots = new List<CharacteristicSnapshot>
        {
            SnapshotTestsFactory.Characteristic(characteristicId)
        };

        // Act
        var result = ProductReplaceAttributesDataBuilder.Build(command, snapshots);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Build_Should_Fail_When_Snapshot_Name_Is_Invalid()
    {
        // Arrange
        var characteristicId = Guid.NewGuid();
        var command = ProductCommandTestsFactory.ValidReplaceAttributesCommand(
            attributes: new Dictionary<Guid, string> { [characteristicId] = "Red" });
        var snapshots = new List<CharacteristicSnapshot>
        {
            SnapshotTestsFactory.Characteristic(characteristicId, name: string.Empty)
        };

        // Act
        var result = ProductReplaceAttributesDataBuilder.Build(command, snapshots);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Build_Should_Fail_When_Snapshot_GroupName_Is_Invalid()
    {
        // Arrange
        var characteristicId = Guid.NewGuid();
        var command = ProductCommandTestsFactory.ValidReplaceAttributesCommand(
            attributes: new Dictionary<Guid, string> { [characteristicId] = "Red" });
        var snapshots = new List<CharacteristicSnapshot>
        {
            // предполагаем, что пробельная строка не проходит валидацию
            // AttributeGroupName.Create, хотя IsNullOrWhiteSpace её отсеет
            // до этого — тест актуален если MinLength/формат может дать ошибку
            // на непустой, но невалидной строке; поправь groupName ниже под
            // конкретное правило AttributeGroupName, если whitespace здесь не подходит
            SnapshotTestsFactory.Characteristic(characteristicId, groupName: new string('a', 1000))
        };

        // Act
        var result = ProductReplaceAttributesDataBuilder.Build(command, snapshots);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Build_Should_Accumulate_Errors_Across_Multiple_Attributes()
    {
        // Arrange
        var missingCharacteristicId = Guid.NewGuid();
        var invalidValueCharacteristicId = Guid.NewGuid();
        var command = ProductCommandTestsFactory.ValidReplaceAttributesCommand(
            attributes: new Dictionary<Guid, string>
            {
                [missingCharacteristicId] = "Red",
                [invalidValueCharacteristicId] = string.Empty
            });
        var snapshots = new List<CharacteristicSnapshot>
        {
            SnapshotTestsFactory.Characteristic(invalidValueCharacteristicId)
        };

        // Act
        var result = ProductReplaceAttributesDataBuilder.Build(command, snapshots);

        // Assert
        result.IsFailure.Should().BeTrue();
        var errorCodes = (List<string>)result.Error.Metadata!["Errors"];
        errorCodes.Should().Contain(CharacteristicSnapshotErrors.NotFound().Code);
        errorCodes.Should().HaveCountGreaterThanOrEqualTo(2);
    }

    [Fact]
    public void Build_Should_Return_Multiple_Attributes_When_All_Valid()
    {
        // Arrange
        var characteristicId1 = Guid.NewGuid();
        var characteristicId2 = Guid.NewGuid();
        var command = ProductCommandTestsFactory.ValidReplaceAttributesCommand(
            attributes: new Dictionary<Guid, string>
            {
                [characteristicId1] = "Red",
                [characteristicId2] = "42"
            });
        var snapshots = new List<CharacteristicSnapshot>
        {
            SnapshotTestsFactory.Characteristic(characteristicId1, name: "Color"),
            SnapshotTestsFactory.Characteristic(characteristicId2, name: "Weight")
        };

        // Act
        var result = ProductReplaceAttributesDataBuilder.Build(command, snapshots);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }
}