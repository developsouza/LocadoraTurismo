using RentalTourismSystem.Helpers;
using RentalTourismSystem.Models;

namespace RentalTourismSystem.Extensions
{
    public static class ModelExtensions
    {
        public static string CpfFormatado(this Cliente cliente)
        {
            return FormatHelpers.FormatarCpf(cliente.Cpf);
        }

        public static string TelefoneFormatado(this Cliente cliente)
        {
            return FormatHelpers.FormatarTelefone(cliente.Telefone);
        }

        public static string PlacaFormatada(this Veiculo veiculo)
        {
            return FormatHelpers.FormatarPlaca(veiculo.Placa);
        }

        public static string IdadeFormatada(this Cliente cliente)
        {
            return FormatHelpers.CalcularIdade(cliente.DataNascimento);
        }

        public static string StatusBadgeClass(this StatusCarro status)
        {
            return FormatHelpers.ObterStatusBadgeClass(status.Status);
        }

        public static string StatusBadgeClass(this StatusReservaViagem status)
        {
            return FormatHelpers.ObterStatusBadgeClass(status.Status);
        }

        public static bool EstahVencida(this Cliente cliente)
        {
            return cliente.ValidadeCNH.HasValue &&
                   cliente.ValidadeCNH.Value.Date < DateTime.Now.Date;
        }

        public static bool VenceEm30Dias(this Cliente cliente)
        {
            if (!cliente.ValidadeCNH.HasValue)
                return false;

            var diasParaVencimento = (cliente.ValidadeCNH.Value.Date - DateTime.Now.Date).Days;
            return diasParaVencimento <= 30 && diasParaVencimento > 0;
        }

        public static bool EstaAtrasada(this Locacao locacao)
        {
            return !locacao.DataDevolucaoReal.HasValue &&
                   locacao.DataDevolucao.Date < DateTime.Now.Date;
        }

        public static int DiasAtraso(this Locacao locacao)
        {
            if (!locacao.EstaAtrasada())
                return 0;

            return (DateTime.Now.Date - locacao.DataDevolucao.Date).Days;
        }

        public static decimal CalcularMultaAtraso(this Locacao locacao, decimal percentualMulta = 0.1m)
        {
            if (!locacao.EstaAtrasada())
                return 0;

            return locacao.ValorTotal * percentualMulta * locacao.DiasAtraso();
        }

        public static bool EhViagemFutura(this ReservaViagem reserva)
        {
            return reserva.DataViagem.Date > DateTime.Now.Date;
        }

        public static string DescricaoStatus(this ReservaViagem reserva)
        {
            if (reserva.DataViagem.Date < DateTime.Now.Date &&
                reserva.StatusReservaViagem.Status == "Confirmada")
                return "Realizada";

            return reserva.StatusReservaViagem.Status;
        }
    }
}