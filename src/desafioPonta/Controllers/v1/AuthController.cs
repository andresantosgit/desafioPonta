using desafioPonta.Core.Common.Interfaces;
using desafioPonta.Core.Domain.Usuario.Entities;
using desafioPonta.Core.Domain.Usuario.Events;
using desafioPonta.Core.Domain.Usuario.Interfaces;
using desafioPonta.Extensions;
using desafioPonta.Infrastructure.Database.EF.Repositories;
using desafioPonta.Inputs;
using desafioPonta.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace desafioPonta.Controllers.v1
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;

        private readonly IConfiguration _configuration;
        private readonly ICryptoService _cryptoService;
        private readonly IUsuarioRepository _usuarioRepository;

        public AuthController(IConfiguration configuration, ILogger<AuthController> logger, ICryptoService cryptoService, IUsuarioRepository usuarioRepository)
        {
            _configuration = configuration;
            _logger = logger;
            _cryptoService = cryptoService;
            _usuarioRepository = usuarioRepository;
        }


        // Endpoint para login de usuário
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TarefaModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorModel))]
        public async Task<IActionResult> Login(
            [FromBody] UserLogin userLogin,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Executando método: Login.");

            string senhaCriptografada = _cryptoService.EncryptPassword(userLogin.Password);

            // Validar usuário e senha
            var usuario = await _usuarioRepository.ValidateAsync(userLogin.Username, senhaCriptografada, cancellationToken);
            if (usuario)
            {
                _logger.LogInformation("Gerando token: " + userLogin.Username);
                var token = GenerateJwtToken(userLogin.Username);
                return Ok(new { Token = token });
            }

            return Unauthorized(new { Message = "Usuário ou senha inválidos" });
        }

        /// <summary>
        /// Cria um usuário.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="criarUsuarioEventHandler"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("criar")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UsuarioModel))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorModel))]        
        public async Task<ActionResult<UsuarioModel>> Post(
            [FromBody] CriarUsuarioInput input,
            [FromServices] IRequestHandler<DomainEvent<CriarUsuarioEvent>, UsuarioEntity> criarUsuarioEventHandler,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Executando método: Cria um usuário.");

            // Criptografar a senha antes de salvar no banco de dados
            string senhaCriptografada = _cryptoService.EncryptPassword(input.Senha);

            var evento = this.CriarEventoDominio(new CriarUsuarioEvent(input.Usuario, input.Email, senhaCriptografada));
            var novaUsuario = await criarUsuarioEventHandler.Handle(evento, cancellationToken);
            return Ok(CriarModelo(novaUsuario));
        }

        /// <summary>
        /// Atualiza senha Usuario.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="atualizarUsuarioEventHandler"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("atualizar")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UsuarioModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorModel))]
        public async Task<ActionResult> Post(
            [FromBody] AtualizarUsuarioInput input,
            [FromServices] IRequestHandler<DomainEvent<AtualizarSenhaUsuarioEvent>, UsuarioEntity> atualizarUsuarioEventHandler,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Executando método: Atualiza uma Tarefa.");

            // Criptografar a senha antes de salvar no banco de dados
            string senhaCriptografada = _cryptoService.EncryptPassword(input.Senha);

            var evento = this.CriarEventoDominio(new AtualizarSenhaUsuarioEvent(input.Usuario, senhaCriptografada));
            var usuario = await atualizarUsuarioEventHandler.Handle(evento, cancellationToken);
            return Ok(CriarModelo(usuario));
        }

        /// <summary>
        /// Exclui um usuario.
        /// </summary>
        /// <param name="usuario">O Usuario a ser excluído.</param>
        /// <param name="excluirUsuarioEventHandler">O manipulador do evento de exclusão do usuario.</param>
        /// <param name="cancellationToken">O token de cancelamento da operação.</param>
        /// <returns>Um ActionResult representando o resultado da operação.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TarefaModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorModel))]
        public async Task<ActionResult> Delete(
            string usuario,
            [FromServices] IRequestHandler<DomainEvent<ExcluirUsuarioEvent>, UsuarioEntity> excluirUsuarioEventHandler,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Executando método: Exclui uma Tarefa.");

            var evento = this.CriarEventoDominio(new ExcluirUsuarioEvent(usuario));
            var tarefa = await excluirUsuarioEventHandler.Handle(evento, cancellationToken);
            return Ok(CriarModelo(tarefa));
        }

        private string GenerateJwtToken(string username)
        {

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private UsuarioModel CriarModelo(UsuarioEntity entidade) => new(this, entidade);
    }    

    public class UserLogin
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
