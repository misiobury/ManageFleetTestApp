using Dashboard2.Model.Domain;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Dashboard2.Model.Infrastructure.Repositories.ViasatApi
{
    class HttpObjectForCommunicationWithApi
    {
        private HttpClient? _httpClient;
        


        #region CONSTRUCTOR
        public HttpObjectForCommunicationWithApi() 
        {
            InitializeHttpClient();
        }
        #endregion



        #region InitializeHttpClient()
        private void InitializeHttpClient()
        {
            this._httpClient = new HttpClient();
            this._httpClient.BaseAddress = new Uri(AppGeneralConfigStaticClass.ApiMainHttpAdress);
        }
        #endregion


        #region GetHttpClient() - Return Http Client
        public HttpClient GetHttpClient()
        {           
            if (this._httpClient == null) { throw new InvalidOperationException("Obiekt httpClient nie został zainicjowany do pracy z Api i ma status null"); }
            else
            {
                return _httpClient;
            }
            
        }
        #endregion


        #region SendAndReceiveHttpRequestNew() - Send message in Soap protocol and return reply from Viasat API
        public async Task<string> SendAndReceiveHttpRequestNew(string HttpRequestBody, string SOAPAction)
        {
           
            var HttpRequest = new HttpRequestMessage(HttpMethod.Post, this._httpClient.BaseAddress);

            if (this._httpClient.DefaultRequestHeaders.Contains("SOAPAction"))
            {
                this._httpClient.DefaultRequestHeaders.Remove("SOAPAction");
            }
            HttpRequest.Headers.Add("SOAPAction", $@"{AppGeneralConfigStaticClass.ApiHeaderRequestActionAdress}{SOAPAction}");
            
            HttpRequest.Content = new StringContent(HttpRequestBody, Encoding.UTF8, "text/xml");
            HttpResponseMessage HttpResponse = await this._httpClient.SendAsync(HttpRequest);
           
            string ReplyBody = await HttpResponse.Content.ReadAsStringAsync();
           
            if (ReplyBody==null)
            {
                MessageBox.Show("Uzyskana odpowiedź z API, jest pusta");
                return null;
            }
            else
            return ReplyBody;                  

        }
        #endregion

    }
}


//==============================================================================================================
//                    GetHttpClient
//==============================================================================================================