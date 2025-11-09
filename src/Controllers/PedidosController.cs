using Microsoft.AspNetCore.Mvc;
using LojaExemplo.Modelos;
using LojaExemplo.Servicos;
using LojaExemplo.DTOs;

namespace LojaExemplo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidosController : ControllerBase
    {
        private readonly IServicoDePedidos _servicoDePedidos;
        private readonly IServicoDePagamentos _servicoDePagamentos;

        public PedidosController(IServicoDePedidos servicoDePedidos, IServicoDePagamentos servicoDePagamentos)
        {
            _servicoDePedidos = servicoDePedidos;
            _servicoDePagamentos = servicoDePagamentos;
        }

        [HttpPost]
        public async Task<ActionResult<Pedido>> CriarPedido([FromBody] CriarPedidoRequest request)
        {
            try
            {
                var pedido = await _servicoDePedidos.CriarPedidoAsync(request.ClienteEmail, request.Itens);
                return CreatedAtAction(nameof(ObterPedido), new { id = pedido.Id }, pedido);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Pedido>> ObterPedido(int id)
        {
            var pedido = await _servicoDePedidos.ObterPedidoPorIdAsync(id);
            if (pedido == null)
            {
                return NotFound();
            }
            return Ok(pedido);
        }

        [HttpGet("cliente/{email}")]
        public async Task<ActionResult<List<Pedido>>> ObterPedidosPorCliente(string email)
        {
            var pedidos = await _servicoDePedidos.ObterPedidosPorClienteAsync(email);
            return Ok(pedidos);
        }

        [HttpPost("{id}/confirmar")]
        public async Task<ActionResult> ConfirmarPedido(int id)
        {
            var sucesso = await _servicoDePedidos.ConfirmarPedidoAsync(id);
            if (!sucesso)
            {
                return BadRequest("Não foi possível confirmar o pedido");
            }
            return Ok();
        }

        [HttpPost("{id}/cancelar")]
        public async Task<ActionResult> CancelarPedido(int id)
        {
            var sucesso = await _servicoDePedidos.CancelarPedidoAsync(id);
            if (!sucesso)
            {
                return BadRequest("Não foi possível cancelar o pedido");
            }
            return Ok();
        }

        [HttpPost("{id}/pagar")]
        public async Task<ActionResult> ProcessarPagamento(int id, [FromBody] ProcessarPagamentoRequest request)
        {
            try
            {
                var sucesso = await _servicoDePagamentos.ProcessarPagamentoAsync(id, request.MetodoPagamento, request.Valor);
                if (!sucesso)
                {
                    return BadRequest("Pagamento rejeitado");
                }
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}/pagamento/status")]
        public async Task<ActionResult<bool>> VerificarStatusPagamento(int id)
        {
            var pago = await _servicoDePagamentos.VerificarStatusPagamentoAsync(id);
            return Ok(pago);
        }

        [HttpPost("{id}/pagamento/estornar")]
        public async Task<ActionResult> EstornarPagamento(int id)
        {
            var sucesso = await _servicoDePagamentos.EstornarPagamentoAsync(id);
            if (!sucesso)
            {
                return BadRequest("Não foi possível estornar o pagamento");
            }
            return Ok();
        }

        [HttpGet("metodos-pagamento")]
        public async Task<ActionResult<List<string>>> ObterMetodosPagamento()
        {
            var metodos = await _servicoDePagamentos.ObterMetodosPagamentoDisponiveisAsync();
            return Ok(metodos);
        }
    }
}
