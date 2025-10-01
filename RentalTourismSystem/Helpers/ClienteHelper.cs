using RentalTourismSystem.Models;

namespace RentalTourismSystem.Helpers
{
    /// <summary>
    /// Helper estático para operações com Cliente (alternativa às extensions)
    /// </summary>
    public static class ClienteHelper
    {
        /// <summary>
        /// Verifica se o cliente pode ser excluído com segurança
        /// </summary>
        public static bool PodeSerExcluido(Cliente cliente)
        {
            if (cliente == null) return false;
            return !TemRelacionamentos(cliente);
        }

        /// <summary>
        /// Verifica se o cliente possui relacionamentos que impedem a exclusão
        /// </summary>
        public static bool TemRelacionamentos(Cliente cliente)
        {
            if (cliente == null) return false;
            return TemLocacoes(cliente) || TemReservas(cliente);
        }

        /// <summary>
        /// Verifica se o cliente possui locações
        /// </summary>
        public static bool TemLocacoes(Cliente cliente)
        {
            return cliente?.Locacoes != null && cliente.Locacoes.Any();
        }

        /// <summary>
        /// Verifica se o cliente possui reservas de viagem
        /// </summary>
        public static bool TemReservas(Cliente cliente)
        {
            return cliente?.ReservasViagens != null && cliente.ReservasViagens.Any();
        }

        /// <summary>
        /// Conta o número de locações ativas (não devolvidas)
        /// </summary>
        public static int ContarLocacoesAtivas(Cliente cliente)
        {
            if (cliente?.Locacoes == null) return 0;
            return cliente.Locacoes.Where(l => l.DataDevolucaoReal == null).Count();
        }

        /// <summary>
        /// Conta o número de locações finalizadas
        /// </summary>
        public static int ContarLocacoesFinalizadas(Cliente cliente)
        {
            if (cliente?.Locacoes == null) return 0;
            return cliente.Locacoes.Where(l => l.DataDevolucaoReal != null).Count();
        }

        /// <summary>
        /// Conta o número de reservas ativas (pendentes ou confirmadas)
        /// </summary>
        public static int ContarReservasAtivas(Cliente cliente)
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
        public static int ContarReservasFinalizadas(Cliente cliente)
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
        public static decimal CalcularValorTotalLocacoes(Cliente cliente)
        {
            if (cliente?.Locacoes == null) return 0;
            return cliente.Locacoes.Sum(l => l.ValorTotal);
        }

        /// <summary>
        /// Calcula o valor total gasto pelo cliente em reservas de viagem
        /// </summary>
        public static decimal CalcularValorTotalReservas(Cliente cliente)
        {
            if (cliente?.ReservasViagens == null) return 0;
            return cliente.ReservasViagens.Sum(r => r.ValorTotal);
        }

        /// <summary>
        /// Calcula o valor total gasto pelo cliente em todos os serviços
        /// </summary>
        public static decimal CalcularValorTotalGeral(Cliente cliente)
        {
            return CalcularValorTotalLocacoes(cliente) + CalcularValorTotalReservas(cliente);
        }

        /// <summary>
        /// Gera uma lista de impedimentos para exclusão
        /// </summary>
        public static List<string> ListarImpedimentosExclusao(Cliente cliente)
        {
            var impedimentos = new List<string>();

            if (cliente == null) return impedimentos;

            var locacoesAtivas = ContarLocacoesAtivas(cliente);
            var locacoesFinalizadas = ContarLocacoesFinalizadas(cliente);
            var reservasAtivas = ContarReservasAtivas(cliente);
            var reservasFinalizadas = ContarReservasFinalizadas(cliente);

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
        public static string GerarMensagemImpedimento(Cliente cliente)
        {
            if (PodeSerExcluido(cliente))
                return string.Empty;

            var impedimentos = ListarImpedimentosExclusao(cliente);

            if (!impedimentos.Any())
                return string.Empty;

            return $"Não é possível excluir este cliente pois ele possui: {string.Join(", ", impedimentos)}. " +
                   "Para excluir um cliente, primeiro finalize ou cancele todas as locações e reservas associadas, " +
                   "ou considere desativar o cliente em vez de excluí-lo.";
        }

        /// <summary>
        /// Verifica se a CNH do cliente está próxima do vencimento
        /// </summary>
        public static bool CnhProximaVencimento(Cliente cliente, int diasAlerta = 30)
        {
            if (cliente?.ValidadeCNH == null) return false;
            return cliente.ValidadeCNH.Value <= DateTime.Now.AddDays(diasAlerta);
        }

        /// <summary>
        /// Verifica se a CNH do cliente está vencida
        /// </summary>
        public static bool CnhVencida(Cliente cliente)
        {
            if (cliente?.ValidadeCNH == null) return false;
            return cliente.ValidadeCNH.Value < DateTime.Now.Date;
        }

        /// <summary>
        /// Calcula a idade do cliente
        /// </summary>
        public static int CalcularIdade(Cliente cliente)
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
        public static bool MaiorIdade(Cliente cliente)
        {
            return CalcularIdade(cliente) >= 18;
        }

        /// <summary>
        /// Gera um resumo do cliente para relatórios
        /// </summary>
        public static string GerarResumo(Cliente cliente)
        {
            if (cliente == null) return "Cliente não encontrado";

            var idade = CalcularIdade(cliente);
            var totalLocacoes = cliente.Locacoes?.Count ?? 0;
            var totalReservas = cliente.ReservasViagens?.Count ?? 0;
            var valorTotal = CalcularValorTotalGeral(cliente);

            return $"Cliente: {cliente.Nome} | " +
                   $"Idade: {idade} anos | " +
                   $"Locações: {totalLocacoes} | " +
                   $"Reservas: {totalReservas} | " +
                   $"Valor Total: {valorTotal:C}";
        }

        /// <summary>
        /// Formata valor monetário de forma segura
        /// </summary>
        public static string FormatarMoeda(decimal valor)
        {
            return valor.ToString("C");
        }

        /// <summary>
        /// Formata data de forma segura
        /// </summary>
        public static string FormatarData(DateTime? data)
        {
            return data?.ToString("dd/MM/yyyy") ?? "Não informado";
        }
    }
}