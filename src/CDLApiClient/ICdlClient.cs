using System;
using System.Threading.Tasks;

namespace CDLApiClient
{
    public interface ICdlClient
    {
        bool ConsultaCpfCnpj(string cpfCnpj, TimeSpan timeout);
        bool ConsultaCpfCnpj(string cpfCnpj);
        Task<bool> ConsultaCpfCnpjAsync(string cpfCnpj, TimeSpan timeout);
        Task<bool> ConsultaCpfCnpjAsync(string cpfCnpj);
    }
}
