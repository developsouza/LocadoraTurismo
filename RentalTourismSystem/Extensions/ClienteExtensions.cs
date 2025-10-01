using RentalTourismSystem.Models;

namespace RentalTourismSystem.Extensions
{
    /// <summary>
    /// Métodos de extensão para a classe Cliente
    /// </summary>
    public static class ClienteExtensions
    {
        /// <summary>
        /// Verifica se o cliente pode ser excluído com segurança
        /// </summary>
        /// <param name="cliente">Cliente a ser verificado</param>
        /// <returns>True se pode excluir, False caso contrário</returns>
        public static bool PodeSerExcluido(this Cliente cliente)
        {
            if (cliente == null) return false;
            return !cliente.TemRelacionamentos();
        }

        /// <summary>
        /// Verifica se o cliente possui relacionamentos que impedem a exclusão
        /// </summary>
        /// <param name="cliente">Cliente a ser verificado</param>
        /// <returns>True se tem relacionamentos, False caso contrário</returns>
        public static bool TemRelacionamentos(this Cliente cliente)
        {
            if (cliente == null) return false;
            return cliente.TemLocacoes() || cliente.TemReservas();
        }

        /// <summary>
        /// Verifica se o cliente possui locações
        /// </summary>
        /// <param name="cliente">Cliente a ser verificado</param>
        /// <returns>True se tem locações, False caso contrário</returns>
        public static bool TemLocacoes(this Cliente cliente)
        {
            if (cliente?.Locacoes == null) return false;
            return cliente.Locacoes.Any();
        }

        /// <summary>
        /// Verifica se o cliente possui reservas de viagem
        /// </summary>
        /// <param name="cliente">Cliente a ser verificado</param>
        /// <returns>True se tem reservas, False caso contrário</returns>
        public static bool TemReservas(this Cliente cliente)
        {
            if (cliente?.ReservasViagens == null) return false;
            return cliente.ReservasViagens.Any();
        }

        /// <summary>
        /// Conta o número de locações ativas (não devolvidas)
        /// </summary>
        /// <param name="cliente">Cliente a ser verificado</param>
        /// <returns>Número de locações ativas</returns>
        public static int ContarLocacoesAtivas(this Cliente cliente)
        {
            if (cliente?.Locacoes == null) return 0;
            return cliente.Locacoes.Where(l => l.DataDevolucaoReal == null).Count();
        }

        /// <summary>
        /// Conta o número de locações finalizadas
        /// </summary>
        /// <param name="cliente">Cliente a ser verificado</param>
        /// <returns>Número de locações finalizadas</returns>
        public static int ContarLocacoesFinalizadas(this Cliente cliente)
        {
            if (cliente?.Locacoes == null) return 0;
            return cliente.Locacoes.Where(l => l.DataDevolucaoReal != null).Count();
        }

        /// <summary>
        /// Conta o número de reservas ativas (pendentes ou confirmadas)
        /// </summary>
        /// <param name="cliente">Cliente a ser verificado</param>
        /// <returns>Número de reservas ativas</returns>
        public static int ContarReservasAtivas(this Cliente cliente)
        {
            if (cliente?.ReservasViagens == null) return 0;
            return cliente.ReservasViagens.Where(r =>
                r.StatusReservaViagemId == 1 || // Pendente
                r.StatusReservaViagemId == 2    // Confirmada
            ).Count();
        }

        /// <summary>
        /// Conta o número de reservas finalizadas (concluídas ou canceladas)
        /// </summary>
        /// <param name="cliente">Cliente a ser verificado</param>
        /// <returns>Número de reservas finalizadas</returns>
        public static int ContarReservasFinalizadas(this Cliente cliente)
        {
            if (cliente?.ReservasViagens == null) return 0;
            return cliente.ReservasViagens.Where(r =>
                r.StatusReservaViagemId == 3 || // Concluída
                r.StatusReservaViagemId == 4    // Cancelada
            ).Count();
        }

        /// <summary>
        /// Calcula o valor total gasto pelo cliente em locações
        /// </summary>
        /// <param name="cliente">Cliente a ser verificado</param>
        /// <returns>Valor total em locações</returns>
        public static decimal CalcularValorTotalLocacoes(this Cliente cliente)
        {
            if (cliente?.Locacoes == null) return 0;
            return cliente.Locacoes.Sum(l => l.ValorTotal);
        }

        /// <summary>
        /// Calcula o valor total gasto pelo cliente em reservas de viagem
        /// </summary>
        /// <param name="cliente">Cliente a ser verificado</param>
        /// <returns>Valor total em reservas</returns>
        public static decimal CalcularValorTotalReservas(this Cliente cliente)
        {
            if (cliente?.ReservasViagens == null) return 0;
            return cliente.ReservasViagens.Sum(r => r.ValorTotal);
        }

        /// <summary>
        /// Calcula o valor total gasto pelo cliente em todos os serviços
        /// </summary>
        /// <param name="cliente">Cliente a ser verificado</param>
        /// <returns>Valor total geral</returns>
        public static decimal CalcularValorTotalGeral(this Cliente cliente)
        {
            return cliente.CalcularValorTotalLocacoes() + cliente.CalcularValorTotalReservas();
        }

        /// <summary>
        /// Gera uma lista de impedimentos para exclusão
        /// </summary>
        /// <param name="cliente">Cliente a ser verificado</param>
        /// <returns>Lista de strings com os impedimentos</returns>
        public static List<string> ListarImpedimentosExclusao(this Cliente cliente)
        {
            var impedimentos = new List<string>();

            if (cliente == null) return impedimentos;

            var locacoesAtivas = cliente.ContarLocacoesAtivas();
            var locacoesFinalizadas = cliente.ContarLocacoesFinalizadas();
            var reservasAtivas = cliente.ContarReservasAtivas();
            var reservasFinalizadas = cliente.ContarReservasFinalizadas();

            if (locacoesAtivas > 0)
            {
                impedimentos.Add($"{locacoesAtivas} locação(ões) ativa(s)");
            }

            if (locacoesFinalizadas > 0)
            {
                impedimentos.Add($"{locacoesFinalizadas} locação(ões) no histórico");
            }

            if (reservasAtivas > 0)
            {
                impedimentos.Add($"{reservasAtivas} reserva(s) de viagem ativa(s)");
            }

            if (reservasFinalizadas > 0)
            {
                impedimentos.Add($"{reservasFinalizadas} reserva(s) no histórico");
            }

            return impedimentos;
        }

        /// <summary>
        /// Gera uma mensagem explicativa sobre impedimentos de exclusão
        /// </summary>
        /// <param name="cliente">Cliente a ser verificado</param>
        /// <returns>Mensagem explicativa ou string vazia se pode excluir</returns>
        public static string GerarMensagemImpedimento(this Cliente cliente)
        {
            if (cliente?.PodeSerExcluido() == true)
                return string.Empty;

            var impedimentos = cliente?.ListarImpedimentosExclusao() ?? new List<string>();

            if (!impedimentos.Any())
                return string.Empty;

            return $"Não é possível excluir este cliente pois ele possui: {string.Join(", ", impedimentos)}. " +
                   "Para excluir um cliente, primeiro finalize ou cancele todas as locações e reservas associadas, " +
                   "ou considere desativar o cliente em vez de excluí-lo.";
        }

        /// <summary>
        /// Verifica se a CNH do cliente está próxima do vencimento
        /// </summary>
        /// <param name="cliente">Cliente a ser verificado</param>
        /// <param name="diasAlerta">Número de dias para alerta (padrão: 30)</param>
        /// <returns>True se CNH está próxima do vencimento</returns>
        public static bool CnhProximaVencimento(this Cliente cliente, int diasAlerta = 30)
        {
            if (cliente?.ValidadeCNH == null) return false;
            return cliente.ValidadeCNH.Value <= DateTime.Now.AddDays(diasAlerta);
        }

        /// <summary>
        /// Verifica se a CNH do cliente está vencida
        /// </summary>
        /// <param name="cliente">Cliente a ser verificado</param>
        /// <returns>True se CNH está vencida</returns>
        public static bool CnhVencida(this Cliente cliente)
        {
            if (cliente?.ValidadeCNH == null) return false;
            return cliente.ValidadeCNH.Value < DateTime.Now.Date;
        }

        /// <summary>
        /// Calcula a idade do cliente
        /// </summary>
        /// <param name="cliente">Cliente a ser verificado</param>
        /// <returns>Idade em anos</returns>
        public static int CalcularIdade(this Cliente cliente)
        {
            if (cliente == null) return 0;

            var hoje = DateTime.Now.Date;
            var idade = hoje.Year - cliente.DataNascimento.Year;

            if (cliente.DataNascimento.Date > hoje.AddYears(-idade))
                idade--;

            return idade;
        }

        /// <summary>
        /// Verifica se o cliente é maior de idade
        /// </summary>
        /// <param name="cliente">Cliente a ser verificado</param>
        /// <returns>True se é maior de idade</returns>
        public static bool MaiorIdade(this Cliente cliente)
        {
            return cliente.CalcularIdade() >= 18;
        }

        /// <summary>
        /// Gera um resumo do cliente para relatórios
        /// </summary>
        /// <param name="cliente">Cliente a ser verificado</param>
        /// <returns>String com resumo do cliente</returns>
        public static string GerarResumo(this Cliente cliente)
        {
            if (cliente == null) return "Cliente não encontrado";

            var idade = cliente.CalcularIdade();
            var totalLocacoes = cliente.Locacoes?.Count ?? 0;
            var totalReservas = cliente.ReservasViagens?.Count ?? 0;
            var valorTotal = cliente.CalcularValorTotalGeral();

            return $"Cliente: {cliente.Nome} | " +
                   $"Idade: {idade} anos | " +
                   $"Locações: {totalLocacoes} | " +
                   $"Reservas: {totalReservas} | " +
                   $"Valor Total: {valorTotal:C}";
        }
    }
}