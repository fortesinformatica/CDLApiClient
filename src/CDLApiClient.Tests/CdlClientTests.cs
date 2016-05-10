using System;
using System.Configuration;
using System.Threading.Tasks;
using Flurl.Http.Testing;
using NUnit.Framework;

namespace CDLApiClient.Tests
{
    [TestFixture]
    public class CdlClientTests
    {
        private const string RETORNO_AUTENTICACAO = "{\"access_token\":\"1\",\"token_type\":\"Bearer\",\"expires_in\":3600,\"refresh_token\":\"a\"}";
        private const string RETORNO_CONSULTA = "{\"status\":\"error\",\"cnpjcpf\":\"\"}";
        private const string CPF_CNPJ = "14052356322";
        private HttpTest _httpTest;
        private CdlClient _sut;
        private string _clientId;
        private string _clientSecret;
        private string _userName;
        private string _password;

        [SetUp]
        public void Setup()
        {
            _clientId = ConfigurationManager.AppSettings.Get("CdlClientId");
            _clientSecret = ConfigurationManager.AppSettings.Get("CdlClientSecret");
            _userName = ConfigurationManager.AppSettings.Get("CdlUserName");
            _password = ConfigurationManager.AppSettings.Get("CdlPassword");

            _httpTest = new HttpTest();
            _sut = new CdlClient();
            _httpTest.RespondWith(200, RETORNO_AUTENTICACAO);
            _httpTest.RespondWith(200, RETORNO_CONSULTA);
        }

        [TestCase("CdlClientId")]
        [TestCase("CdlClientSecret")]
        [TestCase("CdlUserName")]
        [TestCase("CdlPassword")]
        public void Construir(string key)
        {
            var value = ConfigurationManager.AppSettings.Get(key);
            ConfigurationManager.AppSettings.Set(key, null);
            var exception = Assert.Throws<ArgumentException>(() => _sut = new CdlClient());
            Assert.AreEqual("As chaves CdlClientId, CdlClientSecret, CdlUserName e CdlPassword precisam ser configuradas.", exception.Message);
            ConfigurationManager.AppSettings.Set(key, value);
        }

        [Test]
        public void ConsultaCpfCnpjDeveAutenticarPrimeiro()
        {
            _sut.ConsultaCpfCnpj(CPF_CNPJ);
            _httpTest.ShouldHaveCalled("oauth/access_token").WithRequestBody($"grant_type=password&client_id={_clientId}&client_secret={_clientSecret}&username={_userName}&password={_password}");
        }

        [Test]
        public void ConsultaCpfCnpjDeveConsultarCpfCnpj()
        {
            _sut.ConsultaCpfCnpj(CPF_CNPJ);
            _httpTest.ShouldHaveCalled("consulta").WithContentType("application/x-www-form-urlencoded").WithRequestBody($"cnpjcpf={CPF_CNPJ}");
        }

        [Test]
        public async Task ConsultaCpfCnpjAsyncDeveAutenticarPrimeiro()
        {
            await _sut.ConsultaCpfCnpjAsync(CPF_CNPJ);
            _httpTest.ShouldHaveCalled("oauth/access_token").WithRequestBody($"grant_type=password&client_id={_clientId}&client_secret={_clientSecret}&username={_userName}&password={_password}");
        }

        [Test]
        public async Task ConsultaCpfCnpjAsyncDeveConsultarCpfCnpj()
        {
            await _sut.ConsultaCpfCnpjAsync(CPF_CNPJ);
            _httpTest.ShouldHaveCalled("consulta").WithRequestBody(CPF_CNPJ);
        }
    }
}
