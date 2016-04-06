using System;
using System.Configuration;
using System.Linq;
using CDLApiClient.Models;
using Flurl;
using Flurl.Http;

namespace CDLApiClient
{
    public class CdlClient : ICdlClient
    {
        const string URL_BASE_CDL = "http://apiassociados.cdlfor.com.br/";
        const string CONSULTA_URL = "consulta";
        const string AUTENTICACAO_URL = "oauth/access_token";
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

        public bool ConsultaCpfCnpj(string cpfCnpj)
        {
            var autheticationResponse = Autenticar();

            return URL_BASE_CDL
                .AppendPathSegment(CONSULTA_URL)
                .WithOAuthBearerToken(autheticationResponse.AccessToken)
                .PostUrlEncodedAsync(cpfCnpj)
                .ReceiveJson<ConsultaResponse>().Result.Status == "success";
        }

        private AuthenticationResponse Autenticar()
        {
            return URL_BASE_CDL
                .AppendPathSegment(AUTENTICACAO_URL)
                .PostUrlEncodedAsync($"grant_type=password&client_id={_clientId}&client_secret={_clientSecret}&username={_userName}&password={_password}")
                .ReceiveJson<AuthenticationResponse>().Result;
        }
    }
}
