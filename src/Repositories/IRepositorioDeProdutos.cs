using LojaExemplo.Modelos;

namespace LojaExemplo.Repositorios;

public interface IRepositorioDeProdutos
{
    Task<Produto?> ObterPorIdAsync(int id);
    Task<List<Produto>> ObterTodosAsync();
    Task<List<Produto>> ObterPorNomeAsync(string nome);
    Task<Produto> AdicionarAsync(Produto produto);
    Task<Produto> AtualizarAsync(Produto produto);
    Task<bool> RemoverAsync(int id);
    Task<bool> VerificarEstoqueDisponivel(int produtoId, int quantidade);
    Task<bool> ReduzirEstoqueAsync(int produtoId, int quantidade);
    Task<bool> AdicionarEstoqueAsync(int produtoId, int quantidade);
}