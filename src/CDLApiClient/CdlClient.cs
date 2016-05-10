using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using CDLApiClient.Models;
using Flurl;
using Flurl.Http;

namespace CDLApiClient
{
    public class CdlClient : ICdlClient
    {
        private const string URL_BASE_CDL = "http://apiassociados.cdlfor.com.br/";
        private const string CONSULTA_URL = "consulta";
        private const string AUTENTICACAO_URL = "oauth/access_token";
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _userName;
        private readonly string _password;

        public CdlClient()
        {
            _clientId = ConfigurationManager.AppSettings.Get("CdlClientId");
            _clientSecret = ConfigurationManager.AppSettings.Get("CdlClientSecret");
            _userName = ConfigurationManager.AppSettings.Get("CdlUserName");
            _password = ConfigurationManager.AppSettings.Get("CdlPassword");
            var fields = new[] { _clientId, _clientSecret, _userName, _password };
            if (fields.Any(string.IsNullOrEmpty))
                throw new ArgumentException("As chaves CdlClientId, CdlClientSecret, CdlUserName e CdlPassword precisam ser configuradas.");
        }

        public bool ConsultaCpfCnpj(string cpfCnpj, TimeSpan timeout)
        {
            var autheticationResponse = Autenticar(timeout);

            return URL_BASE_CDL
                .AppendPathSegment(CONSULTA_URL)
                .WithOAuthBearerToken(autheticationResponse.AccessToken)
                .WithTimeout(timeout)
                .PostUrlEncodedAsync(new { cnpjcpf = cpfCnpj })
                .ReceiveJson<ConsultaResponse>().Result.Status == "success";
        }

        public bool ConsultaCpfCnpj(string cpfCnpj) => ConsultaCpfCnpj(cpfCnpj, TimeSpan.FromSeconds(10));

        public async Task<bool> ConsultaCpfCnpjAsync(string cpfCnpj, TimeSpan timeout)
        {
            var autheticationResponse = await AutenticarAsync(timeout);

            var consultaResponse = await URL_BASE_CDL
                .AppendPathSegment(CONSULTA_URL)
                .WithOAuthBearerToken(autheticationResponse.AccessToken)
                .WithTimeout(timeout)
                .PostUrlEncodedAsync(new { cnpjcpf = cpfCnpj })
                .ReceiveJson<ConsultaResponse>();

            return consultaResponse.Status == "success";
        }

        public async Task<bool> ConsultaCpfCnpjAsync(string cpfCnpj) => await ConsultaCpfCnpjAsync(cpfCnpj, TimeSpan.FromSeconds(10));

        private AuthenticationResponse Autenticar(TimeSpan timeout)
        {
            return URL_BASE_CDL
                .AppendPathSegment(AUTENTICACAO_URL)
                .WithTimeout(timeout)
                .PostUrlEncodedAsync($"grant_type=password&client_id={_clientId}&client_secret={_clientSecret}&username={_userName}&password={_password}")
                .ReceiveJson<AuthenticationResponse>().Result;
        }

        private async Task<AuthenticationResponse> AutenticarAsync(TimeSpan timeout)
        {
            return await URL_BASE_CDL
                .AppendPathSegment(AUTENTICACAO_URL)
                .WithTimeout(timeout)
                .PostUrlEncodedAsync($"grant_type=password&client_id={_clientId}&client_secret={_clientSecret}&username={_userName}&password={_password}")
                .ReceiveJson<AuthenticationResponse>();
        }
    }
}
