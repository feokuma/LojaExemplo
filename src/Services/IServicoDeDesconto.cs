namespace LojaExemplo.Servicos
{
    public interface IServicoDeDesconto
    {
        Task<decimal> CalcularDescontoProgressivoAsync(decimal valorTotal, decimal percentualDesconto);
        Task<decimal> AplicarDescontoAsync(decimal valorTotal, decimal desconto);
    }
}
