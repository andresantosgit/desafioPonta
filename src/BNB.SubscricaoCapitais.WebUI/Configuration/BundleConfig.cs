//-------------------------------------------------------------------------------------			
// <copyright file="BundleConfig.cs" company="BNB">
//    Copyright statement. All right reserved
// </copyright>	
// <summary>
//   Descrição do arquivo
// </summary>		
//-------------------------------------------------------------------------------------

namespace BNB.ProjetoReferencia.WebUI.Configuration
{

    /// <summary>
    /// Configuração do bundle
    /// </summary>
    public static class BundleConfig
    {
        /// <summary>
        /// Registra os bundles
        /// </summary>
        /// <param name="bundles">Name = "bundles"</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Método padrão do .NET já trata cenários de nulidade do parâmetro.")]
        //public static void RegisterBundles(BundleCollection bundles)
        public static IServiceCollection RegisterBundles(this IServiceCollection services)
        {
            //services.AddWebOptimizer(pipeline =>
            //{
            //    //************* JS *************//
            //    pipeline.AddJavaScriptBundle("/bundles/bnb-theme", "Scripts/bnb-theme.js");
            //    pipeline.AddJavaScriptBundle("/bundles/mascara", "Scripts/mascaras.js");

            //    //************* CSS *************//
            //    pipeline.AddCssBundle("/css/bootstrap", "css/bootstrap/theme.css");

            //    pipeline.AddCssBundle("/css/bnb", "css/bnb/bnb-theme.css",
            //                                               "css/BNBCustom.css");

            //    pipeline.AddCssBundle("/css", "css/font-awesome.css",
            //                                           "css/font-google.css"
            //                                           //"css/jquery-ui.css",
            //                                           /*"css/primeui.min.css",*/
            //                                           /*"css/semantic.min.css"*/);

            //    pipeline.AddCssBundle("/css/custom", "css/semanticCustom.css");
            //    pipeline.AddCssBundle("/css/bnb/style", "css/bnb/style.css");

            //});

            return services;

        }
    }
}
