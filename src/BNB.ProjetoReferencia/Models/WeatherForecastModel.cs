//using BNB.ProjetoReferencia.Controllers.v1;
//using BNB.ProjetoReferencia.Core.Domain.WeatherForecast.Entities;
//using BNB.ProjetoReferencia.Extensions;
//using Microsoft.AspNetCore.Mvc;

//namespace BNB.ProjetoReferencia.Models;

///// <summary>
///// Modelo de previsão do tempo.
///// </summary>
//public class WeatherForecastModel
//{
//    /// <summary>
//    /// Construtor padrão.
//    /// </summary>
//    /// <param name="ctrl"></param>
//    /// <param name="weatherForecast"></param>
//    public WeatherForecastModel(ControllerBase ctrl, WeatherForecastEntity weatherForecast)
//    {
//        TemperaturaC = weatherForecast.TemperaturaC;
//        Local = weatherForecast.Local;

//        // Adiciona links HATEOAS ao modelo
//        Links["self"] = ctrl.Link<WeatherForecastController>(
//           nameof(WeatherForecastController.Get), routeValues: new { local = Local }
//        );

//        Links["patch"] = ctrl.Link<WeatherForecastController>(
//           nameof(WeatherForecastController.Patch), routeValues: new { }
//        );

//        Links["delete"] = ctrl.Link<WeatherForecastController>(
//           nameof(WeatherForecastController.Delete), routeValues: new { local = Local }
//        );
//    }

//    /// <summary>
//    /// Temperatura em graus Celsius.
//    /// </summary>
//    public int TemperaturaC { get; set; }

//    /// <summary>
//    /// Local da previsão do tempo.
//    /// </summary>
//    public string Local { get; set; } = string.Empty;

//    /// <summary>
//    /// Links para ações relacionadas a esta previsão do tempo.
//    /// </summary>
//    public Dictionary<string, string> Links { get; set; } = new();
//}

