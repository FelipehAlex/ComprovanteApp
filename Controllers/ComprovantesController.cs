using ComprovantesApp.Models;
using ComprovantesApp.Models.Dtos;
using ComprovantesApp.Models.Filtros;
using ComprovantesApp.Services;
using ComprovantesApp.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace ComprovantesApp.Controllers
{
    [ApiController]
    [Route("api/comprovantes")]
    public class ComprovantesController : ControllerBase
    {
        private readonly IComprovanteService _comprovanteService;

        public ComprovantesController(IComprovanteService comprovanteService)
        {
            _comprovanteService = comprovanteService;
        }

        // GET /api/comprovantes?fornecedorId=&numeroDocumento=&status=&emissaoDe=&emissaoAte=
        [HttpGet]
        public async Task<ActionResult<List<Comprovante>>> Listar([FromQuery] ComprovanteFiltro filtro)
        {
            var comprovantes = await _comprovanteService.ListarAsync(filtro);
            return Ok(comprovantes);
        }

        // GET /api/comprovantes/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Comprovante>> ObterPorId(int id)
        {
            var comprovante = await _comprovanteService.ObterPorIdAsync(id);
            if (comprovante is null)
                return NotFound(new { mensagem = "Comprovante não encontrado." });

            return Ok(comprovante);
        }

        // GET /api/comprovantes/{id}/historico
        [HttpGet("{id:int}/historico")]
        public async Task<ActionResult<List<HistoricoComprovante>>> ObterHistorico(int id)
        {
            var comprovante = await _comprovanteService.ObterPorIdAsync(id);
            if (comprovante is null)
                return NotFound(new { mensagem = "Comprovante não encontrado." });

            var historico = await _comprovanteService.ObterHistoricoAsync(id);
            return Ok(historico);
        }

        // POST /api/comprovantes
        [HttpPost]
        public async Task<ActionResult<Comprovante>> Criar(ComprovanteRequest request)
        {
            var comprovante = new Comprovante
            {
                NumeroDocumento = request.NumeroDocumento,
                FornecedorId = request.FornecedorId,
                DataEmissao = request.DataEmissao,
                DataVencimento = request.DataVencimento,
                Valor = request.Valor,
                Descricao = request.Descricao
            };

            try
            {
                await _comprovanteService.CriarAsync(comprovante);
            }
            catch (RegraDeNegocioException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }

            return CreatedAtAction(nameof(ObterPorId), new { id = comprovante.Id }, comprovante);
        }

        // PUT /api/comprovantes/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Atualizar(int id, ComprovanteRequest request)
        {
            var comprovante = new Comprovante
            {
                Id = id,
                NumeroDocumento = request.NumeroDocumento,
                FornecedorId = request.FornecedorId,
                DataEmissao = request.DataEmissao,
                DataVencimento = request.DataVencimento,
                Valor = request.Valor,
                Descricao = request.Descricao
            };

            try
            {
                await _comprovanteService.AtualizarAsync(comprovante);
            }
            catch (RegraDeNegocioException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }

            return NoContent();
        }

        // DELETE /api/comprovantes/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Excluir(int id)
        {
            try
            {
                await _comprovanteService.ExcluirAsync(id);
            }
            catch (RegraDeNegocioException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }

            return NoContent();
        }

        // PATCH /api/comprovantes/{id}/validar
        [HttpPatch("{id:int}/validar")]
        public async Task<IActionResult> Validar(int id)
        {
            try
            {
                await _comprovanteService.ValidarAsync(id);
            }
            catch (RegraDeNegocioException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }

            return NoContent();
        }

        // PATCH /api/comprovantes/{id}/inconsistencia
        [HttpPatch("{id:int}/inconsistencia")]
        public async Task<IActionResult> MarcarInconsistencia(int id, InconsistenciaRequest request)
        {
            try
            {
                await _comprovanteService.MarcarInconsistenciaAsync(id, request.Motivo);
            }
            catch (RegraDeNegocioException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }

            return NoContent();
        }

        // POST /api/comprovantes/{id}/integrar
        [HttpPost("{id:int}/integrar")]
        public async Task<IActionResult> Integrar(int id)
        {
            try
            {
                await _comprovanteService.IntegrarAsync(id);
            }
            catch (RegraDeNegocioException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }

            return NoContent();
        }

        // PATCH /api/comprovantes/{id}/cancelar
        [HttpPatch("{id:int}/cancelar")]
        public async Task<IActionResult> Cancelar(int id, InconsistenciaRequest request)
        {
            try
            {
                await _comprovanteService.CancelarAsync(id, request.Motivo);
            }
            catch (RegraDeNegocioException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }

            return NoContent();
        }

    }
}
