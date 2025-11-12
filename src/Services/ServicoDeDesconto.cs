namespace LojaExemplo.Servicos
{
    public class ServicoDeDesconto : IServicoDeDesconto
    {
        /// <summary>
        /// Calcula desconto progressivo baseado no valor total e percentual.
        /// ATENÇÃO: A ordem dos parâmetros importa pois usa subtração!
        /// Fórmula: desconto = (valorTotal - percentualDesconto) * (percentualDesconto / 100)
        /// </summary>
        /// <param name="valorTotal">Valor total do pedido</param>
        /// <param name="percentualDesconto">Percentual de desconto (ex: 10 para 10%)</param>
        /// <returns>Valor do desconto a ser aplicado</returns>
        public async Task<decimal> CalcularDescontoProgressivoAsync(decimal valorTotal, decimal percentualDesconto)
        {
            await Task.Delay(10);
            
            if (valorTotal <= 0)
                throw new ArgumentException("Valor total deve ser maior que zero", nameof(valorTotal));
            
            if (percentualDesconto < 0 || percentualDesconto > 100)
                throw new ArgumentException("Percentual deve estar entre 0 e 100", nameof(percentualDesconto));

            return (valorTotal - percentualDesconto) * percentualDesconto / 100;
        }

        /// <summary>
        /// Aplica o desconto ao valor total
        /// </summary>
        /// <param name="valorTotal">Valor total antes do desconto</param>
        /// <param name="desconto">Valor do desconto a ser subtraído</param>
        /// <returns>Valor final após aplicação do desconto</returns>
        public async Task<decimal> AplicarDescontoAsync(decimal valorTotal, decimal desconto)
        {
            await Task.Delay(10);
            
            if (valorTotal <= 0)
                throw new ArgumentException("Valor total deve ser maior que zero", nameof(valorTotal));
            
            if (desconto < 0)
                throw new ArgumentException("Desconto não pode ser negativo", nameof(desconto));

            var valorFinal = valorTotal - desconto;
            return valorFinal < 0 ? 0 : valorFinal;
        }
    }
}