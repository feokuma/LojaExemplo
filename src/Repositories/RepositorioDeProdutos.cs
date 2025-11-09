using LojaExemplo.Modelos;

namespace LojaExemplo.Repositorios
{
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

    public class RepositorioDeProdutos : IRepositorioDeProdutos
    {
        private readonly List<Produto> _produtos;

        public RepositorioDeProdutos()
        {
            // Simulando alguns produtos para exemplo
            _produtos = new List<Produto>
            {
                new Produto { Id = 1, Nome = "Notebook", Preco = 2500.00m, EstoqueDisponivel = 10, Descricao = "Notebook para trabalho", DataCriacao = DateTime.Now.AddDays(-30), Ativo = true },
                new Produto { Id = 2, Nome = "Mouse", Preco = 50.00m, EstoqueDisponivel = 25, Descricao = "Mouse wireless", DataCriacao = DateTime.Now.AddDays(-20), Ativo = true },
                new Produto { Id = 3, Nome = "Teclado", Preco = 150.00m, EstoqueDisponivel = 15, Descricao = "Teclado mecânico", DataCriacao = DateTime.Now.AddDays(-15), Ativo = true }
            };
        }

        public async Task<Produto?> ObterPorIdAsync(int id)
        {
            await Task.Delay(10); // Simula operação assíncrona
            return _produtos.FirstOrDefault(p => p.Id == id && p.Ativo);
        }

        public async Task<List<Produto>> ObterTodosAsync()
        {
            await Task.Delay(10);
            return _produtos.Where(p => p.Ativo).ToList();
        }

        public async Task<List<Produto>> ObterPorNomeAsync(string nome)
        {
            await Task.Delay(10);
            return _produtos.Where(p => p.Nome.Contains(nome, StringComparison.OrdinalIgnoreCase) && p.Ativo).ToList();
        }

        public async Task<Produto> AdicionarAsync(Produto produto)
        {
            await Task.Delay(10);
            produto.Id = _produtos.Max(p => p.Id) + 1;
            produto.DataCriacao = DateTime.Now;
            _produtos.Add(produto);
            return produto;
        }

        public async Task<Produto> AtualizarAsync(Produto produto)
        {
            await Task.Delay(10);
            var produtoExistente = _produtos.FirstOrDefault(p => p.Id == produto.Id);
            if (produtoExistente != null)
            {
                produtoExistente.Nome = produto.Nome;
                produtoExistente.Preco = produto.Preco;
                produtoExistente.EstoqueDisponivel = produto.EstoqueDisponivel;
                produtoExistente.Descricao = produto.Descricao;
                produtoExistente.Ativo = produto.Ativo;
            }
            return produtoExistente ?? produto;
        }

        public async Task<bool> RemoverAsync(int id)
        {
            await Task.Delay(10);
            var produto = _produtos.FirstOrDefault(p => p.Id == id);
            if (produto != null)
            {
                produto.Ativo = false; // Soft delete
                return true;
            }
            return false;
        }

        public async Task<bool> VerificarEstoqueDisponivel(int produtoId, int quantidade)
        {
            await Task.Delay(10);
            var produto = await ObterPorIdAsync(produtoId);
            return produto != null && produto.EstoqueDisponivel >= quantidade;
        }

        public async Task<bool> ReduzirEstoqueAsync(int produtoId, int quantidade)
        {
            await Task.Delay(10);
            var produto = _produtos.FirstOrDefault(p => p.Id == produtoId && p.Ativo);
            if (produto != null && produto.EstoqueDisponivel >= quantidade)
            {
                produto.EstoqueDisponivel -= quantidade;
                return true;
            }
            return false;
        }

        public async Task<bool> AdicionarEstoqueAsync(int produtoId, int quantidade)
        {
            await Task.Delay(10);
            var produto = _produtos.FirstOrDefault(p => p.Id == produtoId && p.Ativo);
            if (produto != null)
            {
                produto.EstoqueDisponivel += quantidade;
                return true;
            }
            return false;
        }
    }
}