using Dashboard2.Model.Domain;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel;
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
          //  MessageBox.Show("1 send request body - " + HttpRequestBody);
            var HttpRequest = new HttpRequestMessage(HttpMethod.Post, this._httpClient.BaseAddress);

            if (this._httpClient.DefaultRequestHeaders.Contains("SOAPAction"))
            {
                this._httpClient.DefaultRequestHeaders.Remove("SOAPAction");
            }
            HttpRequest.Headers.Add("SOAPAction", $@"{AppGeneralConfigStaticClass.ApiHeaderRequestActionAdress}{SOAPAction}");
           
            HttpRequest.Content = new StringContent(HttpRequestBody, Encoding.UTF8, "text/xml");
            string test = HttpRequest.Content.ReadAsStringAsync().Result;
        //    if(SOAPAction == "GetLocationsExNC")
           //     MessageBox.Show("tresc httprequest przed wyslaniem: "+test);
            // MessageBox.Show("2 send request body - " + test);
            HttpResponseMessage HttpResponse =  new HttpResponseMessage();
            string ReplyBody = "";
            //MessageBox.Show("status przed taskami: " + ReplyBody);

            Task task1 = Task.Run(async () =>
            {
                HttpResponse = await this._httpClient.SendAsync(HttpRequest);
                ReplyBody = await HttpResponse.Content.ReadAsStringAsync();

            });
            Task.WaitAll(task1);


            /*
             *  if (SOAPAction == "GetLocationsExNC")
                MessageBox.Show("task1 zakonczony");
             * 
            Task task2 = Task.Run(async () => { ReplyBody = await HttpResponse.Content.ReadAsStringAsync();
            });
            
            Task.WaitAll(task2);
             if (SOAPAction == "GetLocationsExNC")
                MessageBox.Show("task2 zakonczony");

                if (SOAPAction == "GetLocationsExNC")
            {
                MessageBox.Show("3");
                MessageBox.Show(ReplyBody.Substring(0, 1000));
                MessageBox.Show("4");
            }
            */




            // MessageBox.Show("otrzymana wiadomosc: " + ReplyBody);
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