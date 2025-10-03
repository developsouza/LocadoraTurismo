using RentalTourismSystem.Models;

namespace RentalTourismSystem.Extensions
{
    /// <summary>
    /// Extension methods para ReservaViagem
    /// </summary>
    public static class ReservaViagemExtensions
    {
        /// <summary>
        /// Calcula o valor total real da reserva (pacote + serviços adicionais)
        /// </summary>
        /// <param name="reserva">Reserva a ser calculada</param>
        /// <returns>Valor total incluindo serviços adicionais</returns>
        public static decimal ObterValorTotalComServicos(this ReservaViagem reserva)
        {
            if (reserva == null) return 0;

            var valorPacote = reserva.ValorTotal;
            var valorServicos = reserva.ServicosAdicionais?.Sum(s => s.Preco) ?? 0;

            return valorPacote + valorServicos;
        }

        /// <summary>
        /// Verifica se a reserva tem serviços adicionais
        /// </summary>
        /// <param name="reserva">Reserva a ser verificada</param>
        /// <returns>True se possui serviços adicionais</returns>
        public static bool PossuiServicosAdicionais(this ReservaViagem reserva)
        {
            return reserva?.ServicosAdicionais?.Any() ?? false;
        }

        /// <summary>
        /// Obtém o valor total dos serviços adicionais
        /// </summary>
        /// <param name="reserva">Reserva a ser verificada</param>
        /// <returns>Valor total dos serviços</returns>
        public static decimal ObterValorServicosAdicionais(this ReservaViagem reserva)
        {
            return reserva?.ServicosAdicionais?.Sum(s => s.Preco) ?? 0;
        }

        /// <summary>
        /// Verifica se a reserva está confirmada
        /// </summary>
        /// <param name="reserva">Reserva a ser verificada</param>
        /// <returns>True se está confirmada</returns>
        public static bool EstaConfirmada(this ReservaViagem reserva)
        {
            return reserva?.StatusReservaViagem?.Status == "Confirmada";
        }

        /// <summary>
        /// Verifica se a reserva pode ser editada
        /// </summary>
        /// <param name="reserva">Reserva a ser verificada</param>
        /// <returns>True se pode ser editada</returns>
        public static bool PodeSerEditada(this ReservaViagem reserva)
        {
            return reserva?.StatusReservaViagem?.Status != "Cancelada";
        }
    }
}