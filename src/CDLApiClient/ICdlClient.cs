using System;

namespace CDLApiClient
{
    public interface ICdlClient
    {
        bool ConsultaCpfCnpj(string cpfCnpj, TimeSpan timeout);
        bool ConsultaCpfCnpj(string cpfCnpj);
    }
}