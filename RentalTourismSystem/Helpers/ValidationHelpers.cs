using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace RentalTourismSystem.Helpers
{
    public class CpfValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
                return false;

            string cpf = value.ToString()!.Replace(".", "").Replace("-", "").Trim();

            if (cpf.Length != 11)
                return false;

            // Verifica se todos os dígitos são iguais
            if (cpf.All(c => c == cpf[0]))
                return false;

            // Valida primeiro dígito verificador
            int soma = 0;
            for (int i = 0; i < 9; i++)
                soma += int.Parse(cpf[i].ToString()) * (10 - i);

            int resto = soma % 11;
            int digitoVerificador1 = resto < 2 ? 0 : 11 - resto;

            if (int.Parse(cpf[9].ToString()) != digitoVerificador1)
                return false;

            // Valida segundo dígito verificador
            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += int.Parse(cpf[i].ToString()) * (11 - i);

            resto = soma % 11;
            int digitoVerificador2 = resto < 2 ? 0 : 11 - resto;

            return int.Parse(cpf[10].ToString()) == digitoVerificador2;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"O campo {name} deve conter um CPF válido.";
        }
    }

    public class PlacaValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
                return false;

            string placa = value.ToString()!.Replace("-", "").ToUpper().Trim();

            // Formato antigo: ABC1234
            var regexAntigo = new Regex(@"^[A-Z]{3}[0-9]{4}$");

            // Formato Mercosul: ABC1D23
            var regexMercosul = new Regex(@"^[A-Z]{3}[0-9][A-Z][0-9]{2}$");

            return regexAntigo.IsMatch(placa) || regexMercosul.IsMatch(placa);
        }

        public override string FormatErrorMessage(string name)
        {
            return $"O campo {name} deve conter uma placa válida (formato ABC-1234 ou ABC1D23).";
        }
    }

    public class IdadeValidationAttribute : ValidationAttribute
    {
        private readonly int _idadeMinima;
        private readonly int _idadeMaxima;

        public IdadeValidationAttribute(int idadeMinima = 18, int idadeMaxima = 120)
        {
            _idadeMinima = idadeMinima;
            _idadeMaxima = idadeMaxima;
        }

        public override bool IsValid(object? value)
        {
            if (value is DateTime dataNascimento)
            {
                var idade = DateTime.Now.Year - dataNascimento.Year;
                if (DateTime.Now.DayOfYear < dataNascimento.DayOfYear)
                    idade--;

                return idade >= _idadeMinima && idade <= _idadeMaxima;
            }

            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"A idade deve estar entre {_idadeMinima} e {_idadeMaxima} anos.";
        }
    }

    public class DataFuturaValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is DateTime data)
            {
                return data.Date >= DateTime.Now.Date;
            }

            return true; // Permite valores nulos se o campo não for obrigatório
        }

        public override string FormatErrorMessage(string name)
        {
            return $"O campo {name} deve ser uma data futura.";
        }
    }
}