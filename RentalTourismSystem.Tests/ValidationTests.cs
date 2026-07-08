using System.ComponentModel.DataAnnotations;
using RentalTourismSystem.Helpers;
using RentalTourismSystem.Models;
using RentalTourismSystem.ViewModels;

namespace RentalTourismSystem.Tests;

public class ValidationTests
{
    [Theory]
    [InlineData("529.982.247-25", true)]
    [InlineData("111.111.111-11", false)]
    [InlineData("123", false)]
    public void CpfValidation_DeveValidarDigitos(string cpf, bool esperado) =>
        Assert.Equal(esperado, new CpfValidationAttribute().IsValid(cpf));

    [Theory]
    [InlineData("ABC-1234", true)]
    [InlineData("BRA2E19", true)]
    [InlineData("<script>", false)]
    public void PlacaValidation_DeveAceitarSomentePadroesBrasileiros(string placa, bool esperado) =>
        Assert.Equal(esperado, new PlacaValidationAttribute().IsValid(placa));

    [Fact]
    public void PreCadastro_DeveRejeitarDataFinalAnteriorAoInicio()
    {
        var model = new PreCadastroCliente
        {
            DataInicioLocacao = new DateTime(2030, 1, 10),
            DataFinalLocacao = new DateTime(2030, 1, 9)
        };
        var results = new List<ValidationResult>();

        Validator.TryValidateObject(model, new ValidationContext(model), results, validateAllProperties: true);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(model.DataFinalLocacao)) ||
                                      r.ErrorMessage?.Contains("posterior") == true);
    }

    [Fact]
    public void RegisterViewModel_DeveExigirNoMinimoDezCaracteres()
    {
        var property = typeof(RegisterViewModel).GetProperty(nameof(RegisterViewModel.Password))!;
        var rule = property.GetCustomAttributes(typeof(StringLengthAttribute), true)
            .Cast<StringLengthAttribute>().Single();

        Assert.Equal(10, rule.MinimumLength);
    }
}
