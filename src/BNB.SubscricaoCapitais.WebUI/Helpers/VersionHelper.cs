namespace BNB.ProjetoReferencia.WebUI.Helpers
{
    public class VersionHelper
    {
        public static string GetAssemblyVersion()
        {
            // VERSAO ASSEMBLY
            var lobjVersao = System.Reflection.Assembly.GetExecutingAssembly();
            return lobjVersao.GetName().Version?.ToString();

            //AssemblyInformationalVersionAttribute infoVersion = (AssemblyInformationalVersionAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false).FirstOrDefault();
            //return infoVersion.InformationalVersion;
        }
    }
}
