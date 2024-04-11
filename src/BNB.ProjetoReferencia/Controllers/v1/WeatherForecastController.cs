//using BNB.ProjetoReferencia.Core.Common.Interfaces;
//using BNB.ProjetoReferencia.Core.Domain.WeatherForecast.Entities;
//using BNB.ProjetoReferencia.Core.Domain.WeatherForecast.Events;
//using BNB.ProjetoReferencia.Core.Domain.WeatherForecast.Interfaces;
//using BNB.ProjetoReferencia.Extensions;
//using BNB.ProjetoReferencia.Inputs;
//using BNB.ProjetoReferencia.Models;
//using Microsoft.AspNetCore.Mvc;

//namespace BNB.ProjetoReferencia.Controllers.v1;

///// <summary>
///// Representa uma previsão do tempo.
///// </summary>
//[ApiController]
//[Route("api/v1/weather-forecast")]
//public class WeatherForecastController : ControllerBase
//{
//    /// <summary>
//    /// Cria uma previsão do tempo
//    /// </summary>
//    /// <param name="input"></param>
//    /// <param name="criarWeatherForecastEventHandler"></param>
//    /// <param name="cancellationToken"></param>
//    /// <returns></returns>
//    [HttpPost]
//    [ProducesResponseType(StatusCodes.Status200OK)]
//    [ProducesResponseType(StatusCodes.Status417ExpectationFailed)]
//    [ProducesResponseType(StatusCodes.Status400BadRequest)]
//    public async Task<ActionResult<WeatherForecastModel>> Post(
//        [FromBody] CriarWeatherForecastInput input,
//        [FromServices] IRequestHandler<DomainEvent<CriarWeatherForecastEvent>, WeatherForecastEntity> criarWeatherForecastEventHandler,
//        CancellationToken cancellationToken)
//    {
//        if (!ModelState.IsValid)
//            return BadRequest(ModelState);

//        var evento = this.CriarEventoDominio<CriarWeatherForecastEvent>(input);
//        var newWeatherForecast = await criarWeatherForecastEventHandler.Handle(evento, cancellationToken);
//        return Ok(CriarModelo(newWeatherForecast));
//    }

//    /// <summary>
//    /// Obtem as informaçãoes da previsão do tempo
//    /// </summary>
//    /// <param name="local"></param>
//    /// <param name="weatherForecastRepository"></param>
//    /// <param name="cancellationToken"></param>
//    /// <returns></returns>
//    [HttpGet("{local}")]
//    [ProducesResponseType(StatusCodes.Status200OK)]
//    [ProducesResponseType(StatusCodes.Status204NoContent)]
//    [ProducesResponseType(StatusCodes.Status400BadRequest)]
//    public async Task<ActionResult<WeatherForecastModel>> Get(
//        [FromRoute] string local,
//        [FromServices] IWeatherForecastRepository weatherForecastRepository,
//        CancellationToken cancellationToken)
//    {
//        var weatherForecast = await weatherForecastRepository.FindByLocalAsync(local, cancellationToken);
//        if (weatherForecast is null)
//            return NoContent();
//        return Ok(CriarModelo(weatherForecast));
//    }

//    /// <summary>
//    /// Atualiza a temperatura da previsão do tempo
//    /// </summary>
//    /// <param name="input"></param>
//    /// <param name="atualizarTemperaturaWeatherForecastEventHandler"></param>
//    /// <param name="cancellationToken"></param>
//    /// <returns></returns>
//    [HttpPatch]
//    [ProducesResponseType(StatusCodes.Status200OK)]
//    [ProducesResponseType(StatusCodes.Status417ExpectationFailed)]
//    [ProducesResponseType(StatusCodes.Status400BadRequest)]
//    public async Task<ActionResult<WeatherForecastModel>> Patch(
//        [FromBody] AtualizarTemperaturaWeatherForecastInput input,
//        [FromServices] IRequestHandler<DomainEvent<AtualizarTemperaturaWeatherForecastEvent>, WeatherForecastEntity> atualizarTemperaturaWeatherForecastEventHandler,
//        CancellationToken cancellationToken)
//    {
//        if (!ModelState.IsValid)
//            return BadRequest(ModelState);

//        var evento = this.CriarEventoDominio<AtualizarTemperaturaWeatherForecastEvent>(input);
//        var weatherForecast = await atualizarTemperaturaWeatherForecastEventHandler.Handle(evento, cancellationToken);
//        return Ok(CriarModelo(weatherForecast));
//    }


//    /// <summary>
//    /// Deletar uma previsão do tempo
//    /// </summary>
//    /// <param name="local"></param>
//    /// <param name="removerWeatherForecastEventHandler"></param>
//    /// <param name="cancellationToken"></param>
//    /// <returns></returns>
//    [HttpDelete("{local}")]
//    [ProducesResponseType(StatusCodes.Status200OK)]
//    [ProducesResponseType(StatusCodes.Status417ExpectationFailed)]
//    [ProducesResponseType(StatusCodes.Status400BadRequest)]
//    public async Task<ActionResult<WeatherForecastModel>> Delete(
//        [FromRoute] string local,
//        [FromServices] IRequestHandler<DomainEvent<RemoverWeatherForecastEvent>> removerWeatherForecastEventHandler,
//        CancellationToken cancellationToken)
//    {
//        var evento = this.CriarEventoDominio<RemoverWeatherForecastEvent>(new(local));
//        await removerWeatherForecastEventHandler.Handle(evento, cancellationToken);
//        return Ok();
//    }


//    private WeatherForecastModel CriarModelo(WeatherForecastEntity entidade) => new(this, entidade);
//}
