using Microsoft.EntityFrameworkCore;
using RentalTourismSystem.Data;

namespace RentalTourismSystem.Services
{
    public interface IRelatorioService
    {
        Task<Dictionary<string, object>> GerarDashboardAsync();
        Task<List<dynamic>> ObterTopClientesAsync(int quantidade = 5);
        Task<List<dynamic>> ObterVeiculosMaisAlugadosAsync();
        Task<List<dynamic>> ObterPacotesMaisVendidosAsync();
        Task<Dictionary<string, decimal>> ObterReceitaPorMesAsync(int ano);
    }

    public class RelatorioService : IRelatorioService
    {
        private readonly RentalTourismContext _context;

        public RelatorioService(RentalTourismContext context)
        {
            _context = context;
        }

        public async Task<Dictionary<string, object>> GerarDashboardAsync()
        {
            var hoje = DateTime.Now;
            var inicioMes = new DateTime(hoje.Year, hoje.Month, 1);
            var fimMes = inicioMes.AddMonths(1).AddDays(-1);

            var dashboard = new Dictionary<string, object>
            {
                ["TotalClientes"] = await _context.Clientes.CountAsync(),
                ["TotalVeiculos"] = await _context.Veiculos.CountAsync(),
                ["VeiculosDisponiveis"] = await _context.Veiculos
                    .Include(v => v.StatusCarro)
                    .CountAsync(v => v.StatusCarro.Status == "Disponível"),
                ["LocacoesMes"] = await _context.Locacoes
                    .CountAsync(l => l.DataRetirada >= inicioMes && l.DataRetirada <= fimMes),
                ["ReservasMes"] = await _context.ReservasViagens
                    .CountAsync(r => r.DataReserva >= inicioMes && r.DataReserva <= fimMes),
                ["ReceitaLocacoesMes"] = await _context.Locacoes
                    .Where(l => l.DataRetirada >= inicioMes && l.DataRetirada <= fimMes)
                    .SumAsync(l => l.ValorTotal),
                ["ReceitaReservasMes"] = await _context.ReservasViagens
                    .Where(r => r.DataReserva >= inicioMes && r.DataReserva <= fimMes &&
                               r.StatusReservaViagem.Status == "Confirmada")
                    .SumAsync(r => r.ValorTotal),
                ["LocacoesAtrasadas"] = await _context.Locacoes
                    .CountAsync(l => l.DataDevolucaoReal == null && l.DataDevolucao.Date < hoje.Date),
                ["CNHsVencendo"] = await _context.Clientes
                    .CountAsync(c => c.ValidadeCNH.HasValue &&
                                    c.ValidadeCNH.Value.Date <= hoje.AddDays(30).Date &&
                                    c.ValidadeCNH.Value.Date >= hoje.Date)
            };

            return dashboard;
        }

        public async Task<List<dynamic>> ObterTopClientesAsync(int quantidade = 5)
        {
            return await _context.Clientes
                .Select(c => new
                {
                    c.Nome,
                    c.Email,
                    c.Telefone,
                    TotalGasto = c.Locacoes.Sum(l => l.ValorTotal) + c.ReservasViagens.Sum(r => r.ValorTotal),
                    TotalLocacoes = c.Locacoes.Count(),
                    TotalReservas = c.ReservasViagens.Count(),
                    UltimaAtividade = c.Locacoes.Any() ?
                        c.Locacoes.Max(l => l.DataRetirada) :
                        (DateTime?)null
                })
                .OrderByDescending(c => c.TotalGasto)
                .Take(quantidade)
                .ToListAsync<dynamic>();
        }

        public async Task<List<dynamic>> ObterVeiculosMaisAlugadosAsync()
        {
            return await _context.Veiculos
                .Include(v => v.StatusCarro)
                .Select(v => new
                {
                    Veiculo = v,
                    TotalLocacoes = v.Locacoes.Count(),
                    ReceitaTotal = v.Locacoes.Sum(l => l.ValorTotal),
                    MediaDiasAlugado = v.Locacoes.Count() > 0 ?
                        v.Locacoes.Average(l => EF.Functions.DateDiffDay(l.DataRetirada, l.DataDevolucao)) : 0,
                    UltimaLocacao = v.Locacoes.Any() ?
                        v.Locacoes.Max(l => l.DataRetirada) : (DateTime?)null
                })
                .OrderByDescending(v => v.TotalLocacoes)
                .ToListAsync<dynamic>();
        }

        public async Task<List<dynamic>> ObterPacotesMaisVendidosAsync()
        {
            return await _context.PacotesViagens
                .Select(p => new
                {
                    Pacote = p,
                    TotalReservas = p.ReservasViagens.Count(),
                    TotalPessoas = p.ReservasViagens.Sum(r => r.Quantidade),
                    ReceitaTotal = p.ReservasViagens
                        .Where(r => r.StatusReservaViagem.Status == "Confirmada")
                        .Sum(r => r.ValorTotal),
                    UltimaReserva = p.ReservasViagens.Any() ?
                        p.ReservasViagens.Max(r => r.DataReserva) : (DateTime?)null
                })
                .OrderByDescending(p => p.TotalReservas)
                .ToListAsync<dynamic>();
        }

        public async Task<Dictionary<string, decimal>> ObterReceitaPorMesAsync(int ano)
        {
            var receitas = new Dictionary<string, decimal>();

            for (int mes = 1; mes <= 12; mes++)
            {
                var inicioMes = new DateTime(ano, mes, 1);
                var fimMes = inicioMes.AddMonths(1).AddDays(-1);

                var receitaLocacoes = await _context.Locacoes
                    .Where(l => l.DataRetirada >= inicioMes && l.DataRetirada <= fimMes)
                    .SumAsync(l => l.ValorTotal);

                var receitaReservas = await _context.ReservasViagens
                    .Where(r => r.DataReserva >= inicioMes && r.DataReserva <= fimMes &&
                               r.StatusReservaViagem.Status == "Confirmada")
                    .SumAsync(r => r.ValorTotal);

                receitas.Add($"{mes:D2}/{ano}", receitaLocacoes + receitaReservas);
            }

            return receitas;
        }
    }
}