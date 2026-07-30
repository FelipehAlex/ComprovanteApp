using ComprovantesApp.Models;
using ComprovantesApp.Models.Dtos;
using ComprovantesApp.Services;
using ComprovantesApp.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace ComprovantesApp.Controllers
{
    [ApiController]
    [Route("api/fornecedores")]
    public class FornecedoresController : ControllerBase
    {
        private readonly IFornecedorService _fornecedorService;

        public FornecedoresController(IFornecedorService fornecedorService)
        {
            _fornecedorService = fornecedorService;
        }

        // GET /api/fornecedores
        [HttpGet]
        public async Task<ActionResult<List<Fornecedor>>> Listar()
        {
            var fornecedores = await _fornecedorService.ListarAsync();
            return Ok(fornecedores);
        }

        // GET /api/fornecedores/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Fornecedor>> ObterPorId(int id)
        {
            var fornecedor = await _fornecedorService.ObterPorIdAsync(id);
            if (fornecedor is null)
                return NotFound(new { mensagem = "Fornecedor não encontrado." });

            return Ok(fornecedor);
        }

        // POST /api/fornecedores
        [HttpPost]
        public async Task<ActionResult<Fornecedor>> Criar(FornecedorRequest request)
        {
            var fornecedor = new Fornecedor
            {
                Nome = request.Nome,
                Cnpj = request.Cnpj,
                TipoFornecedor = request.TipoFornecedor,
                Ativo = request.Ativo
            };

            try
            {
                await _fornecedorService.CriarAsync(fornecedor);
            }
            catch (RegraDeNegocioException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }

            return CreatedAtAction(nameof(ObterPorId), new { id = fornecedor.Id }, fornecedor);
        }

        // PUT /api/fornecedores/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Atualizar(int id, FornecedorRequest request)
        {
            var fornecedor = new Fornecedor
            {
                Id = id,
                Nome = request.Nome,
                Cnpj = request.Cnpj,
                TipoFornecedor = request.TipoFornecedor,
                Ativo = request.Ativo
            };

            try
            {
                await _fornecedorService.AtualizarAsync(fornecedor);
            }
            catch (RegraDeNegocioException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }

            return NoContent();
        }

        // PATCH /api/fornecedores/{id}/inativar
        [HttpPatch("{id:int}/inativar")]
        public async Task<IActionResult> Inativar(int id)
        {
            try
            {
                await _fornecedorService.InativarAsync(id);
            }
            catch (RegraDeNegocioException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }

            return NoContent();
        }

        // PATCH /api/fornecedores/{id}/ativar
        [HttpPatch("{id:int}/ativar")]
        public async Task<IActionResult> Ativar(int id)
        {
            try
            {
                await _fornecedorService.AtivarAsync(id);
            }
            catch (RegraDeNegocioException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }

            return NoContent();
        }
    }
}
