using System.Globalization;
using System.Text.RegularExpressions;

namespace RentalTourismSystem.Helpers
{
    public static class FormatHelpers
    {
        // Cultura brasileira personalizada
        private static readonly CultureInfo CulturaBrasil = new("pt-BR")
        {
            NumberFormat =
            {
                CurrencySymbol = "R$",
                CurrencyDecimalDigits = 2,
                CurrencyDecimalSeparator = ",",
                CurrencyGroupSeparator = ".",
                NumberDecimalSeparator = ",",
                NumberGroupSeparator = ".",
                CurrencyPositivePattern = 2, // R$ 1.234,56
                CurrencyNegativePattern = 9  // -R$ 1.234,56
            },
            DateTimeFormat =
            {
                ShortDatePattern = "dd/MM/yyyy",
                LongDatePattern = "dddd, d 'de' MMMM 'de' yyyy",
                ShortTimePattern = "HH:mm",
                LongTimePattern = "HH:mm:ss",
                DateSeparator = "/",
                TimeSeparator = ":"
            }
        };

        #region Funções Originais (Compatibilidade)

        /// <summary>
        /// Formatar CPF (versão original - minúsculo)
        /// </summary>
        public static string FormatarCpf(string cpf)
        {
            if (string.IsNullOrEmpty(cpf))
                return "";

            cpf = Regex.Replace(cpf, @"\D", "");

            if (cpf.Length != 11)
                return cpf;

            return $"{cpf.Substring(0, 3)}.{cpf.Substring(3, 3)}.{cpf.Substring(6, 3)}-{cpf.Substring(9, 2)}";
        }

        /// <summary>
        /// Formatar telefone (versão original)
        /// </summary>
        public static string FormatarTelefone(string telefone)
        {
            if (string.IsNullOrEmpty(telefone))
                return "";

            telefone = Regex.Replace(telefone, @"\D", "");

            if (telefone.Length == 10)
                return $"({telefone.Substring(0, 2)}) {telefone.Substring(2, 4)}-{telefone.Substring(6, 4)}";

            if (telefone.Length == 11)
                return $"({telefone.Substring(0, 2)}) {telefone.Substring(2, 5)}-{telefone.Substring(7, 4)}";

            return telefone;
        }

        /// <summary>
        /// Formatar placa (versão original)
        /// </summary>
        public static string FormatarPlaca(string placa)
        {
            if (string.IsNullOrEmpty(placa))
                return "";

            placa = placa.Replace("-", "").ToUpper().Trim();

            if (placa.Length == 7 && Regex.IsMatch(placa, @"^[A-Z]{3}[0-9]{4}$"))
                return $"{placa.Substring(0, 3)}-{placa.Substring(3, 4)}";

            return placa;
        }

        /// <summary>
        /// Obter classe CSS do badge baseado no status (versão original)
        /// </summary>
        public static string ObterStatusBadgeClass(string status)
        {
            return status switch
            {
                "Disponível" => "bg-success",
                "Alugado" => "bg-warning",
                "Manutenção" => "bg-danger",
                "Indisponível" => "bg-secondary",
                "Pendente" => "bg-warning",
                "Confirmada" => "bg-success",
                "Cancelada" => "bg-danger",
                "Realizada" => "bg-info",
                _ => "bg-secondary"
            };
        }

        /// <summary>
        /// Calcular idade (versão original)
        /// </summary>
        public static string CalcularIdade(DateTime dataNascimento)
        {
            var idade = DateTime.Now.Year - dataNascimento.Year;
            if (DateTime.Now.DayOfYear < dataNascimento.DayOfYear)
                idade--;

            return $"{idade} anos";
        }

        /// <summary>
        /// Calcular dias entre datas (versão original)
        /// </summary>
        public static string CalcularDiasEntreDatas(DateTime dataInicio, DateTime dataFim)
        {
            var dias = (dataFim - dataInicio).Days;
            return dias == 1 ? "1 dia" : $"{dias} dias";
        }

        /// <summary>
        /// Formatar moeda brasileira (versão melhorada)
        /// </summary>
        public static string FormatarMoeda(decimal valor)
        {
            return valor.ToString("C", CulturaBrasil);
        }

        /// <summary>
        /// Truncar texto com reticências
        /// </summary>
        public static string TruncarTexto(string texto, int tamanhoMaximo)
        {
            if (string.IsNullOrEmpty(texto) || texto.Length <= tamanhoMaximo)
                return texto ?? string.Empty;

            return texto.Substring(0, tamanhoMaximo) + "...";
        }

        #endregion

        #region Funções Brasileiras Melhoradas

        /// <summary>
        /// Formatar valor monetário sem símbolo
        /// </summary>
        public static string FormatarMoedaSemSimbolo(decimal valor)
        {
            return valor.ToString("N2", CulturaBrasil);
        }

        /// <summary>
        /// Formatar valor monetário compacto (ex: R$ 1,2 mil)
        /// </summary>
        public static string FormatarMoedaCompacta(decimal valor)
        {
            return valor switch
            {
                >= 1_000_000_000 => $"R$ {valor / 1_000_000_000:N1} bi",
                >= 1_000_000 => $"R$ {valor / 1_000_000:N1} mi",
                >= 1_000 => $"R$ {valor / 1_000:N1} mil",
                _ => FormatarMoeda(valor)
            };
        }

        /// <summary>
        /// Formatar data brasileira (dd/MM/yyyy)
        /// </summary>
        public static string FormatarData(DateTime data)
        {
            return data.ToString("dd/MM/yyyy", CulturaBrasil);
        }

        /// <summary>
        /// Formatar data com hora (dd/MM/yyyy HH:mm)
        /// </summary>
        public static string FormatarDataHora(DateTime dataHora)
        {
            return dataHora.ToString("dd/MM/yyyy HH:mm", CulturaBrasil);
        }

        /// <summary>
        /// Formatar data por extenso
        /// </summary>
        public static string FormatarDataExtenso(DateTime data)
        {
            return data.ToString("dddd, d 'de' MMMM 'de' yyyy", CulturaBrasil);
        }

        /// <summary>
        /// Formatar data relativa (ex: "há 2 dias")
        /// </summary>
        public static string FormatarDataRelativa(DateTime data)
        {
            var agora = DateTime.Now;
            var diferenca = agora - data;

            return diferenca.TotalDays switch
            {
                < 1 when diferenca.TotalHours < 1 => diferenca.TotalMinutes switch
                {
                    < 1 => "Agora mesmo",
                    < 2 => "há 1 minuto",
                    var minutos => $"há {Math.Floor(minutos)} minutos"
                },
                < 1 => diferenca.TotalHours switch
                {
                    < 2 => "há 1 hora",
                    var horas => $"há {Math.Floor(horas)} horas"
                },
                < 2 => "Ontem",
                < 7 => $"há {Math.Floor(diferenca.TotalDays)} dias",
                < 30 => $"há {Math.Floor(diferenca.TotalDays / 7)} semanas",
                < 365 => $"há {Math.Floor(diferenca.TotalDays / 30)} meses",
                _ => $"há {Math.Floor(diferenca.TotalDays / 365)} anos"
            };
        }

        /// <summary>
        /// Formatar CNPJ (12.345.678/0001-90)
        /// </summary>
        public static string FormatarCNPJ(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
                return string.Empty;

            var numeros = new string(cnpj.Where(char.IsDigit).ToArray());

            return numeros.Length == 14
                ? $"{numeros[..2]}.{numeros[2..5]}.{numeros[5..8]}/{numeros[8..12]}-{numeros[12..]}"
                : cnpj;
        }

        /// <summary>
        /// Formatar CEP (12345-678)
        /// </summary>
        public static string FormatarCEP(string cep)
        {
            if (string.IsNullOrWhiteSpace(cep))
                return string.Empty;

            var numeros = new string(cep.Where(char.IsDigit).ToArray());

            return numeros.Length == 8
                ? $"{numeros[..5]}-{numeros[5..]}"
                : cep;
        }

        /// <summary>
        /// Formatar percentual brasileiro
        /// </summary>
        public static string FormatarPercentual(decimal valor, int casasDecimais = 1)
        {
            return (valor / 100).ToString($"P{casasDecimais}", CulturaBrasil);
        }

        /// <summary>
        /// Formatar número inteiro com separador de milhares
        /// </summary>
        public static string FormatarNumero(int numero)
        {
            return numero.ToString("N0", CulturaBrasil);
        }

        /// <summary>
        /// Formatar número decimal brasileiro
        /// </summary>
        public static string FormatarNumeroDecimal(decimal numero, int casasDecimais = 2)
        {
            return numero.ToString($"N{casasDecimais}", CulturaBrasil);
        }

        #endregion

        #region Funções Utilitárias

        /// <summary>
        /// Calcular apenas o número da idade
        /// </summary>
        public static int CalcularIdadeNumero(DateTime dataNascimento)
        {
            var idade = DateTime.Now.Year - dataNascimento.Year;
            if (DateTime.Now.DayOfYear < dataNascimento.DayOfYear)
                idade--;

            return idade;
        }

        /// <summary>
        /// Converter string de moeda brasileira para decimal
        /// </summary>
        public static decimal? ConverterMoedaParaDecimal(string moeda)
        {
            if (string.IsNullOrWhiteSpace(moeda))
                return null;

            var valor = moeda
                .Replace("R$", "")
                .Replace(" ", "")
                .Trim();

            return decimal.TryParse(valor, NumberStyles.Currency, CulturaBrasil, out var resultado)
                ? resultado
                : null;
        }

        /// <summary>
        /// Converter data string brasileira para DateTime
        /// </summary>
        public static DateTime? ConverterDataBrasileiraParaDateTime(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                return null;

            return DateTime.TryParseExact(data, "dd/MM/yyyy", CulturaBrasil, DateTimeStyles.None, out var resultado)
                ? resultado
                : null;
        }

        /// <summary>
        /// Gerar iniciais do nome
        /// </summary>
        public static string GerarIniciais(string nomeCompleto)
        {
            if (string.IsNullOrWhiteSpace(nomeCompleto))
                return "??";

            var nomes = nomeCompleto.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            return nomes.Length switch
            {
                1 => nomes[0].Length >= 2 ? nomes[0][..2].ToUpper() : nomes[0].ToUpper(),
                _ => $"{char.ToUpper(nomes[0][0])}{char.ToUpper(nomes[^1][0])}"
            };
        }

        /// <summary>
        /// Mascarar informações sensíveis (CPF, telefone, etc.)
        /// </summary>
        public static string MascararInformacao(string informacao, int caracteresVisiveis = 4)
        {
            if (string.IsNullOrWhiteSpace(informacao) || informacao.Length <= caracteresVisiveis)
                return informacao ?? string.Empty;

            var visivel = informacao[^caracteresVisiveis..];
            var mascarado = new string('*', informacao.Length - caracteresVisiveis);

            return $"{mascarado}{visivel}";
        }

        /// <summary>
        /// Formatar status com ícone e cor
        /// </summary>
        public static string FormatarStatusComIcone(string status)
        {
            return status.ToLower() switch
            {
                "ativo" or "disponível" or "confirmado" => $"<i class='fas fa-check-circle text-success'></i> {status}",
                "inativo" or "indisponível" or "cancelado" => $"<i class='fas fa-times-circle text-danger'></i> {status}",
                "pendente" or "aguardando" => $"<i class='fas fa-clock text-warning'></i> {status}",
                "em andamento" or "processando" => $"<i class='fas fa-spinner fa-spin text-info'></i> {status}",
                "manutenção" => $"<i class='fas fa-wrench text-secondary'></i> {status}",
                _ => $"<i class='fas fa-info-circle text-muted'></i> {status}"
            };
        }

        /// <summary>
        /// Validar se é um CPF válido
        /// </summary>
        public static bool ValidarCPF(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return false;

            var numeros = new string(cpf.Where(char.IsDigit).ToArray());

            if (numeros.Length != 11 || numeros.All(c => c == numeros[0]))
                return false;

            // Calcular primeiro dígito verificador
            var soma = 0;
            for (var i = 0; i < 9; i++)
                soma += int.Parse(numeros[i].ToString()) * (10 - i);

            var resto = soma % 11;
            var digito1 = resto < 2 ? 0 : 11 - resto;

            if (int.Parse(numeros[9].ToString()) != digito1)
                return false;

            // Calcular segundo dígito verificador
            soma = 0;
            for (var i = 0; i < 10; i++)
                soma += int.Parse(numeros[i].ToString()) * (11 - i);

            resto = soma % 11;
            var digito2 = resto < 2 ? 0 : 11 - resto;

            return int.Parse(numeros[10].ToString()) == digito2;
        }

        /// <summary>
        /// Formatar file size (bytes) em formato legível
        /// </summary>
        public static string FormatarTamanhoArquivo(long bytes)
        {
            string[] sufixos = { "B", "KB", "MB", "GB", "TB" };

            if (bytes == 0)
                return "0 B";

            var lugar = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            var num = Math.Round(bytes / Math.Pow(1024, lugar), 1);

            return $"{num:N1} {sufixos[lugar]}";
        }

        /// <summary>
        /// Obter cultura brasileira configurada
        /// </summary>
        public static CultureInfo ObterCulturaBrasileira() => CulturaBrasil;

        #endregion
    }
}