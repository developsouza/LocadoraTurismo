namespace RentalTourismSystem.Models.ViewModels
{
    public class ItemVistoria
    {
        public int Numero { get; set; }
        public string Descricao { get; set; }

        // Para itens com Funcionando/Não Funciona
        public bool Funcionando { get; set; }
        public bool NaoFunciona { get; set; }

        // Para itens com OK/Faltando
        public bool Ok { get; set; }
        public bool Faltando { get; set; }

        // Para itens com Em Dia/Vencido
        public bool EmDia { get; set; }
        public bool Vencido { get; set; }

        // Para itens com No Nível/Completar
        public bool NoNivel { get; set; }
        public bool Completar { get; set; }

        // Para itens com Bom/Ruim
        public bool Bom { get; set; }
        public bool Ruim { get; set; }

        // Para itens com Normal/Trincado ou Riscado
        public bool Normal { get; set; }
        public bool Trincado { get; set; }
        public bool RiscadoAmassado { get; set; }

        public string Comentario { get; set; }
    }
}
