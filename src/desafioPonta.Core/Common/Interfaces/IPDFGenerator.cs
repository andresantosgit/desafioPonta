namespace desafioPonta.Core.Common.Interfaces;

public interface IPDFGenerator
{
    byte[] Generate(string html, CancellationToken cancellationToken);
}