using Dashboard2.Model.Domain;
using Dashboard2.Model.Infrastructure.Repositories.ViasatApi;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Xml;

namespace Dashboard2.Model.Infrastructure.DataAccessLayer.ViasatApiPhysicalLayer
{
    public class ViasatDbContext : INotifyPropertyChanged
    {
        private HttpObjectForCommunicationWithApi? _HttpObjectForCommunicationWithApi;
        private ObservableCollection<ViasatClientObject>? _carsIdAndRegnumListFromApi;
        public ObservableCollection<ViasatClientObject>? CarsIdAndRegnumListFromApi
        { get { return _carsIdAndRegnumListFromApi; } set { _carsIdAndRegnumListFromApi = value; OnPropertyChanged("CarsIdAndRegnumListFromApi"); } }
        private string _SessionId;
        public string SessionId { get { return _SessionId; } set { _SessionId = value; } }
        private bool _isConnectionToApiActive;
        public bool IsConnectionToApiActive { get { return _isConnectionToApiActive; } set { _isConnectionToApiActive = value; OnPropertyChanged("IsConnectionToApiActive"); } }



        //=========================================================================================================================
        //                   CONSTRUCTOR
        //=========================================================================================================================   
        public ViasatDbContext()
        {
            this.IsConnectionToApiActive = false;
           InitializeHttpObject();
         //   MessageBox.Show("utworzono httpobject");
       
          // LoginUser();
          //  MessageBox.Show("po funkcji loginuser()");
           // View.CommunicationWindow.Message.ShowCommunicationWindow(this.SessionId);
          //  GetClientObjects();
          //  MessageBox.Show("glw konstruktor ViasatDbContext\npo funkcji getclientobject()");
          // Task.Run( ()=>  );
         //  await GetClientObjects();

        }



        

        //=======================================================================
        //                   API VIASAT Main Methods
        //=======================================================================    

        //LoginUser()
        #region LoginUser() - Viasat API function to login to Viasat interface
        public void LoginUser()
        {
              string login = AppGeneralConfigStaticClass.ApiLogin;
            string password =  AppGeneralConfigStaticClass.ApiPassword;
             string test = ViasatMethodsTemplateStaticClass.LoginUserTemplate(login, password);

            Task<string> TaskResult = _HttpObjectForCommunicationWithApi.SendAndReceiveHttpRequestNew(test, "LoginUser");
            
            try
            {
                Task.Run(() => TaskResult.WaitAsync(TimeSpan.FromSeconds(10)));
                var ReceivedSoapBody = TaskResult.Result;

                if (ReceivedSoapBody != null)
                {                
                   this._SessionId =  GetSessionIdFromSoapBody(ReceivedSoapBody);                 
                    if(this.SessionId != null && this.SessionId != "0" && this.SessionId!="")
                    {
                        this.IsConnectionToApiActive=true;
                    }

                    if (this._SessionId == "0")
                    {
                        MessageBox.Show("Nie udało się zalogować do Api Viasat,\n niepoprawne dane logowania");
                    }
                }               
              
            }
            catch(TimeoutException) { MessageBox.Show("Nie udalo sie polaczyc z Api Viasat\n upłynął czas próby połączenia."); }
            catch(Exception ex) { MessageBox.Show(ex.Message); }

            GetClientObjects();
        }
        #endregion


        //LogOffSession()
        #region LogOffSession() - Viasat API function to logout from Viasat interface
        public void LogOffSession()
        {   
            Task<string> TaskResult = _HttpObjectForCommunicationWithApi.SendAndReceiveHttpRequestNew(ViasatMethodsTemplateStaticClass.LogOffSessionTemplate(this._SessionId), "LogOffSession");

            Task.Run(() => TaskResult.WaitAsync(TimeSpan.FromSeconds(20)));
            string ReceivedSoapBody = TaskResult.Result;
            string LogOffStatus = (GetLogOffStatusFromSoapBody(ReceivedSoapBody));

                if (LogOffStatus != null)
                {
                   this.IsConnectionToApiActive = false;
                }
                else
                {
                    MessageBox.Show("Wartość LogOffStatus = null");
                }

        }
        #endregion


        //GetClientObjects()
        #region  GetClientObjects() - Viasat API function to get list of registration numbers of cars and assigned to them Id Gps-devices
        private void GetClientObjects()
        {
           string test = ViasatMethodsTemplateStaticClass.GetClientObjectTemplate(this._SessionId);
  
            if (this._HttpObjectForCommunicationWithApi != null)
            {
                Task<string> TaskResult = _HttpObjectForCommunicationWithApi.SendAndReceiveHttpRequestNew(test, "GetClientObjects");
                try
                {
                    Task.Run(() => TaskResult.WaitAsync(TimeSpan.FromSeconds(10)));
                    string ReceivedSoapBody = TaskResult.Result;
                                  
                    if (ReceivedSoapBody != null)
                    {
                        //  MessageBox.Show("odp liczba znakow: "+ReceivedSoapBody.Count().ToString());
                        this.CarsIdAndRegnumListFromApi = GetClientListFromSoapBody(ReceivedSoapBody);
                     /*   string test1 = "";
                        foreach(var el in this.CarsIdAndRegnumListFromApi)
                        {
                            test1 += $"{el.Name} - {el.Id}\n";
                        }
                        MessageBox.Show(test1);*/
                     //   MessageBox.Show("metoda GetClientObject()\npobrano aut z odpowiedzi: " + this.CarsIdAndRegnumListFromApi.Count);
                    }
                }
                catch (TimeoutException) { MessageBox.Show("Nie udalo sie uzyskać odpowiedzi z Api\n upłynął czas oczekiwania na odpowiedź."); }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
            else
                MessageBox.Show("Obiekt Http do komunikacji z API, jest pusty");
        }
        #endregion


        //GetLocationsExNc()
        #region  GetLocationsExNC() - Viasat API function to get list of time checkpoints for a specific car
      //  public List<CheckPoint> GetLocationsExNC(ViasatClientObject SelectedAuto, DateTime StartDate, DateTime EndDate)
        public List<CheckPoint> GetLocationsExNC(DTOForGetLocationsForCar SelectedAuto)
        {           // MessageBox.Show("jedziemy z tematem!");
            Task<string> TaskResult = _HttpObjectForCommunicationWithApi.SendAndReceiveHttpRequestNew(ViasatMethodsTemplateStaticClass.GetLocationsExNCTemplate(this._SessionId, SelectedAuto.Id, SelectedAuto.DateFrom, SelectedAuto.DateTo), "GetLocationsExNC");   
            CheckPoint CheckPoint = new CheckPoint(SelectedAuto.Id, SelectedAuto.Name);
            try
            {
                // Task task1 = Task.Run(async () => await TaskResult.WaitAsync(TimeSpan.FromSeconds(20)));
                Task.Run(async () => await TaskResult);
                Task.WaitAll(TaskResult);
                  //  MessageBox.Show("skonczylem wywolanie taskresult");
                string ReceivedSoapBody = TaskResult.Result;

                // MessageBox.Show("znowu w glownej funkcji: "+ReceivedSoapBody);
                if (ReceivedSoapBody != null)
                {
                    // return  CreateCheckpointsListFromSoapResponse(ReceivedSoapBody, CheckPoint, SelectedAuto);
                    return CreateCheckpointsListFromSoapResponse2(ReceivedSoapBody);
                }
                else
                    return new List<CheckPoint>();

            }
            catch (TimeoutException) { MessageBox.Show("Nie udalo sie uzyskać odpowiedzi z Api Viasat\n upłynął czas oczekiwania na odpowiedź."); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

            return new List<CheckPoint>();
                
        }
        #endregion



        //GetDeviceStatisticFlow()
        #region GetDeviceStatisticFlow() Viasat API function to get list of numbers of km's in selected duration for every car
        public   ObservableCollection<ViasatClientObject> GetDeviceStatisticFlow(string dateFrom, string dateTo)
        {         
            Task<string> TaskResult = _HttpObjectForCommunicationWithApi.SendAndReceiveHttpRequestNew(ViasatMethodsTemplateStaticClass.GetDeviceStatisticFlowTemplate(this._SessionId, dateFrom, dateTo), "GetDeviceStatisticFlow");
            try
            {
                Task.Run(() => TaskResult.WaitAsync(TimeSpan.FromSeconds(10)));
                string ReceivedSoapBody = TaskResult.Result;
              //  MessageBox.Show(ReceivedSoapBody);
            
                if (ReceivedSoapBody != null)
                {
                   // MessageBox.Show("1");         
                    var ResultList = GetMileageOfKilometersFromSoapBodyForAllCars(ReceivedSoapBody);                  
                    var TempListOfGetClientObjectsMethod = this.CarsIdAndRegnumListFromApi;
                   
                    if (TempListOfGetClientObjectsMethod != null)
                    {
                        for (int i = 0; i < ResultList.Count; i++)
                        {
                            foreach (var element in TempListOfGetClientObjectsMethod)
                            {
                                if (element.Id == ResultList[i].Id)
                                {
                                    ResultList[i].Name = element.Name;
                                    break;
                                }
                            }
                        }
/*
                        string test = "";
                        foreach(var el in ResultList)
                        {
                            test += $"{el.Name} - {el.Id} - {el.NumberOfKilometres}\n";
                        }
                        MessageBox.Show(test);
*/
                        return ResultList;
                    }
                    else
                    {
                        MessageBox.Show("TempListOfGetClientObjectsMethod jest null");
                        return ResultList;
                    }
                       
               
                }

            }
            catch (TimeoutException) { MessageBox.Show("Nie udalo sie uzyskać odpowiedzi z Api\n upłynął czas oczekiwania na odpowiedź."); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

            return null;
        }
        #endregion
        
        


        #region GetDeviceStatistic() Viasat API function to get list of numbers of km's in selected duration for every car
        public  string GetDeviceStatistic(DateTime dateFrom, DateTime dateTo, string selectedCarId)
        {         
            Task<string> TaskResult = _HttpObjectForCommunicationWithApi.SendAndReceiveHttpRequestNew(ViasatMethodsTemplateStaticClass.GetDeviceStatisticTemplate(this._SessionId, dateFrom, dateTo, selectedCarId), "GetDeviceStatistic");
            try
            {
                Task.Run(() => TaskResult.WaitAsync(TimeSpan.FromSeconds(10)));
                string ReceivedSoapBody = TaskResult.Result;
              //  MessageBox.Show(ReceivedSoapBody);
            
                if (ReceivedSoapBody != null)
                {
                   // MessageBox.Show("1");         
                    var ResultList = GetMileageOfKilometersFromSoapBodyForOneCar(ReceivedSoapBody);                  
                    var TempListOfGetClientObjectsMethod = this.CarsIdAndRegnumListFromApi;
                   
                 
/*
                        string test = "";
                        foreach(var el in ResultList)
                        {
                            test += $"{el.Name} - {el.Id} - {el.NumberOfKilometres}\n";
                        }
                        MessageBox.Show(test);
*/
                        return ResultList;
                }
                else
                {
                        MessageBox.Show("TempListOfGetClientObjectsMethod jest null");
                        return null;
                }
            }

            catch (TimeoutException) { MessageBox.Show("Nie udalo sie uzyskać odpowiedzi z Api\n upłynął czas oczekiwania na odpowiedź."); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

            return null;
        }
        #endregion






        //===========================================================================================================
        //                   API VIASAT helper methods for main Methods
        //===========================================================================================================


        //LoginUser()  helper functions
        //===========================================================================================================
        #region GetSessionIdFromSoapBody() - helper function for function LoginUser()
        private string GetSessionIdFromSoapBody(string ReceivedSoapBody)
        {
            if (ReceivedSoapBody == "" || ReceivedSoapBody == null)
            {
                MessageBox.Show("Przy próbie uzyskania ID Sessi z odpowiedzi z API Viasat, wiadomość jest pusta");
                throw new ArgumentNullException("Przy próbie uzyskania ID Sessi z odpowiedzi z API Viasat, wiadomość jest pusta");
            }
            else
            {
                try
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(ReceivedSoapBody);

                    XmlNamespaceManager XmlNsManager = CreateXmlManagerToReadSoapReqest(xmlDoc);

                    // First option to get SessionId from Soap message
                    //-----------------------------------------------------
                    XmlNode node = xmlDoc.SelectSingleNode("//cma:LoginUserResult", XmlNsManager);
                    string SessionId = node.InnerText;

                    //Second option to get SessionId from Soap message
                    //---------------------------------------------------
                    //XmlNode node2 = xmlDoc.SelectSingleNode("/soap:Envelope/soap:Body/cma:LoginUserResponse/cma:LoginUserResult", XmlNsManager);
                    //string SessionID = node2.InnerText;

                    return SessionId;
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }

                return null;
            }
        }
        #endregion


        //LogoffSession()  helper functions
        //===========================================================================================================
        #region  GetLogOffStatusFromSoapBody() -  helper function for function LogOffSession()
        private string GetLogOffStatusFromSoapBody(string ReceivedSoapBody)
        {
           // MessageBox.Show(ReceivedSoapBody);

            if (ReceivedSoapBody == "" || ReceivedSoapBody == null)
            {
                MessageBox.Show("Przy próbie uzyskania ID Sesji z odpowiedzi z API Viasat, wiadomość jest pusta");
                throw new ArgumentNullException("Przy próbie uzyskania ID Sessi z odpowiedzi z API Viasat, wiadomość jest pusta");
            }
            else
            {
                try
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(ReceivedSoapBody);

                    XmlNamespaceManager XmlNsManager = CreateXmlManagerToReadSoapReqest(xmlDoc);

                    // First option to get SessionId from Soap message
                    //-----------------------------------------------------
                    XmlNode node = xmlDoc.SelectSingleNode("/soap:Envelope/soap:Body/cma:LogOffSessionResponse/cma:LogOffSessionResult", XmlNsManager);
                    string LogOffStatus = node.InnerText;
                                        
                    return LogOffStatus;
                }
                catch (Exception ex) { MessageBox.Show("Przy próbie uzyskania statusu wylogowania z odpowiedzi z Viasat,\nwystąpił problem: "+ex.Message); }

                return null;
            }
        }
        #endregion


        //GetClientObjects()  helper functions
        //===========================================================================================================
        #region GetClientListFromSoapBody() - helper function for function GetClientObjects()
        public ObservableCollection<ViasatClientObject> GetClientListFromSoapBody(string xmldoc)
        {
           // MessageBox.Show(xmldoc);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmldoc);

            XmlNamespaceManager XmlNsManager = CreateXmlManagerToReadSoapReqest(xmlDoc);
            
            //create list from all IdDevice nodes from Soap body message
            XmlNodeList IdDeviceNodesList = xmlDoc.SelectNodes("//cma:intIdDevice", XmlNsManager);
            List<string> IdDeviceList = new List<string>();
            foreach(XmlNode Node in IdDeviceNodesList)
            {
                IdDeviceList.Add(Node.InnerText);
            }                     

            //create list from all ObjectName nodes from Soap body message
            XmlNodeList ObjectNameNodesList = xmlDoc.SelectNodes("//cma:ObjectName", XmlNsManager);
            List<string> RegNumList = new List<string>();            
            foreach (XmlNode node in ObjectNameNodesList)
            {               
                RegNumList.Add(node.InnerText);
            }
            /*
            string test3 = "";
            foreach(var el in IdDeviceList)
            {
                test3 += $"{el}\n";
            }
            MessageBox.Show(test3);
            */

            ObservableCollection<ViasatClientObject> ReturnCarList = new ObservableCollection<ViasatClientObject>();

            for (int i = 0; i < IdDeviceList.Count; i++)
            {
                //Console.WriteLine(IdDeviceList[i].ToString() + " = " + MileageList[i].ToString());
               
                ReturnCarList.Add(new ViasatClientObject(IdDeviceList[i], RegNumList[i]));
            }
            //MessageBox.Show(ReturnCarList[4].Name);
           // MessageBox.Show($"el 1 z listy wszystkich nodow: {ReturnCarList[0].Name}  {ReturnCarList[0].Id}");
            return ReturnCarList;
        }
        #endregion

      
        //GetDeviceStatistic()  helper functions
        //===========================================================================================================
       private string GetMileageOfKilometersFromSoapBodyForOneCar(string xmldoc)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmldoc);

            XmlNamespaceManager XmlNsManager = CreateXmlManagerToReadSoapReqest(xmlDoc);
     
            XmlNode node  =  xmlDoc.SelectSingleNode("//cma:dblDistance", XmlNsManager);          
            string SessionId = node.InnerText;

            return SessionId;
       
        }




         //GetDeviceStatisticFlow()  helper functions
        //===========================================================================================================
       private ObservableCollection<ViasatClientObject> GetMileageOfKilometersFromSoapBodyForAllCars(string xmldoc)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmldoc);

            XmlNamespaceManager XmlNsManager = CreateXmlManagerToReadSoapReqest(xmlDoc);

            //create list from all IdDevice nodes from Soap body message
            XmlNodeList IdDeviceNodesList = xmlDoc.SelectNodes("//cma:intIdDevice", XmlNsManager);
            List<string> IdDeviceList = new List<string>();
            foreach (XmlNode Node in IdDeviceNodesList)
            {
                IdDeviceList.Add(Node.InnerText);
            }
          //  MessageBox.Show("ilosc nodow z Id device: " + IdDeviceList.Count);

            //create list from all ObjectName nodes from Soap body message
            XmlNodeList ObjectNameNodesList = xmlDoc.SelectNodes("//cma:dblDistance", XmlNsManager);
            List<string> MileageList = new List<string>();
            foreach (XmlNode node in ObjectNameNodesList)
            {
                if(node.InnerText.IndexOf(".") != -1)
                {
                    MileageList.Add(node.InnerText.Substring(0, node.InnerText.IndexOf('.')));
                }
                else
                    MileageList.Add("0");
                
            }
          // MessageBox.Show("ilosc nodow z przebiegiem: " + MileageList.Count);
            ObservableCollection<ViasatClientObject> ReturnCarList = new ObservableCollection<ViasatClientObject>();

            for (int i = 0; i < IdDeviceList.Count; i++)
            {
                //Console.WriteLine(IdDeviceList[i].ToString() + " = " + MileageList[i].ToString());

                ReturnCarList.Add(new ViasatClientObject(IdDeviceList[i], null ,MileageList[i]));
            }
            //MessageBox.Show(ReturnCarList[4].Name);
          //  MessageBox.Show("koniec metody getMileageKilometersfromSoap;\n pierwszy element: " + ReturnCarList[0].Id + " - " + ReturnCarList[0].NumberOfKilometres);
            
            return ReturnCarList;
        }



        //GetLocationsExNC()  helper functions
        //===========================================================================================================

        private List<CheckPoint> CreateCheckpointsListFromSoapResponse2(string ReceivedSoapBody)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(ReceivedSoapBody);
            
            List<XmlNodeList> AllNodesListsFoundInXML = GenerateNodesListsFromXML(xmlDoc);

            List<CheckPoint> AllCheckpointsfromXML = new List<CheckPoint>();

            for (int i = 0; i < AllNodesListsFoundInXML[0].Count; i++)
            {
                //CreateCheckpointBasedOnXMLNodes() - generate checkpoint from list of all nodes (as parameters such as speed, address etc.) 
                AllCheckpointsfromXML.Add(CreateCheckpointBasedOnXMLNodes(AllNodesListsFoundInXML, i));
            }

            return AllCheckpointsfromXML; 
        
        }

        private CheckPoint CreateCheckpointBasedOnXMLNodes(List<XmlNodeList> AllNodesListsFoundInXML, int i)
        {
            CheckPoint TempTimeCheckPoint = new CheckPoint();
            return WriteParamToCheckpointFromNodesLists(TempTimeCheckPoint, AllNodesListsFoundInXML, i);

        }



     

        //======================================================================================================================================================
        //======================================================================================================================================================
        //======================================================================================================================================================
        //======================================================================================================================================================


        //CreateCheckpointsListFromSoapResponse() 
        #region CreateCheckpointsListFromSoapResponse()
        public List<CheckPoint> CreateCheckpointsListFromSoapResponse(string ReceivedSoapBody, CheckPoint TempCheckpoint, DTOForGetLocationsForCar SelectedCar)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(ReceivedSoapBody);

            //get all needed parameteres of checkpoints from Soap response, to one list, consist from lists of parameters such as speed etc.
            List<XmlNodeList> AllNodesListsFoundInXML = GenerateNodesListsFromXML(xmlDoc);

            List<CheckPoint> ListOfCheckPointsForSummaryOfResult = new List<CheckPoint>();

            if (AllNodesListsFoundInXML[0].Count > 0)
            {
                List<CheckPoint> AllCheckpointsfromXML = new List<CheckPoint>();

                for (int i = 0; i < AllNodesListsFoundInXML[0].Count; i++)
                {
                    //CreateCheckpointBasedOnXMLNodes() - generate checkpoint from list of all nodes (as parameters such as speed, address etc.) 
                    AllCheckpointsfromXML.Add(CreateCheckpointBasedOnXMLNodes(TempCheckpoint, AllNodesListsFoundInXML, i));
                }

                if (AllCheckpointsfromXML.Count > 2)
                {
                    CheckPoint T1 = CreateCopyOfCheckPointForWriteToList(AllCheckpointsfromXML[0]);

                    ListOfCheckPointsForSummaryOfResult.Add(CreateCopyOfCheckPointForWriteToList(AllCheckpointsfromXML[0]));

                    int T1Index = 0;
                    int T2Index = 0;

                    for (int i = 0, krok = 1; i < AllCheckpointsfromXML.Count; i++, krok++)
                    {
                        T2Index = i;

                        if (i == AllCheckpointsfromXML.Count - 1)
                        {
                            ListOfCheckPointsForSummaryOfResult.Add(CreateCopyOfCheckPointForWriteToList(AllCheckpointsfromXML[T1Index]));
                            ListOfCheckPointsForSummaryOfResult.Add(CreateCopyOfCheckPointForWriteToList(AllCheckpointsfromXML[T2Index]));
                            break;
                        }
                        else if (CheckIsAddressSameAsPrevious(AllCheckpointsfromXML[T1Index].LocalizationDescription, AllCheckpointsfromXML[T2Index].LocalizationDescription))
                        {
                            if (CheckIfBothCheckpointsAreTheSame(AllCheckpointsfromXML[T1Index], AllCheckpointsfromXML[0]))
                            {
                                int index = CompareCheckpointsEachOtherToFindEndOfPauseOrBrake(T1Index, AllCheckpointsfromXML, SelectedCar.CarParkTime);

                                if (index > 0)
                                {
                                    ListOfCheckPointsForSummaryOfResult.Add(CreateCopyOfCheckPointForWriteToList(AllCheckpointsfromXML[index]));
                                    i = index;
                                    T1Index = index;
                                    continue;
                                }
                                else if (index < 0)
                                {
                                    i = index * (-1);
                                    T2Index = i;
                                    continue;
                                }
                            }
                            else
                            {
                                int index = CompareCheckpointsEachOtherToFindEndOfPauseOrBrake(T2Index, AllCheckpointsfromXML, SelectedCar.CarParkTime);
                                if (index > 0)
                                {
                                    ListOfCheckPointsForSummaryOfResult.Add(CreateCopyOfCheckPointForWriteToList(AllCheckpointsfromXML[T1Index + 1]));
                                    ListOfCheckPointsForSummaryOfResult.Add(CreateCopyOfCheckPointForWriteToList(AllCheckpointsfromXML[T2Index + 1]));
                                    ListOfCheckPointsForSummaryOfResult.Add(CreateCopyOfCheckPointForWriteToList(AllCheckpointsfromXML[index]));
                                    i = index;
                                    continue;
                                }
                                else if (index < 0)
                                {
                                    //Console.WriteLine("indeks < 0 ");
                                    i = index * (-1);
                                    T2Index = i;
                                    continue;
                                }
                            }
                        }
                        else if (!(CheckIsAddressSameAsPrevious(AllCheckpointsfromXML[T1Index].LocalizationDescription, AllCheckpointsfromXML[T2Index].LocalizationDescription)))
                        {
                            if (!(CheckIsAddressSameAsPrevious(AllCheckpointsfromXML[T2Index].LocalizationDescription, AllCheckpointsfromXML[T2Index + 1].LocalizationDescription)))
                            {
                                continue;
                            }
                            else if ((CheckIsAddressSameAsPrevious(AllCheckpointsfromXML[T2Index].LocalizationDescription, AllCheckpointsfromXML[T2Index + 1].LocalizationDescription)))
                            {
                                int index = CompareCheckpointsEachOtherToFindEndOfPauseOrBrake(T2Index, AllCheckpointsfromXML, SelectedCar.CarParkTime);
                                if (index > 0)
                                {
                                    ListOfCheckPointsForSummaryOfResult.Add(CreateCopyOfCheckPointForWriteToList(AllCheckpointsfromXML[T2Index]));
                                    ListOfCheckPointsForSummaryOfResult.Add(CreateCopyOfCheckPointForWriteToList(AllCheckpointsfromXML[index]));
                                    T1Index = index;
                                    i = index;
                                    continue;
                                }
                                else if (index < 0)
                                {
                                    i = index * (-1);
                                    T2Index = index;
                                    continue;
                                }
                            }
                        }
                    }
                }
            }

            if (ListOfCheckPointsForSummaryOfResult != null)
            {
              //  MessageBox.Show($"X: {ListOfCheckPointsForSummaryOfResult[0].X}");
                return ListOfCheckPointsForSummaryOfResult;
            }
            else
                return null;
        }
        #endregion


        //CompareCheckpointsEachOtherToFindEndOfPauseOrBrake  - find end checkpoint which is as same as our and compare them is this brake or pause
        #region CompareCheckpointsEachOtherToFindEndOfPauseOrBrake()
        private int CompareCheckpointsEachOtherToFindEndOfPauseOrBrake(int Index, List<CheckPoint> ChckpointsList, int CarParkTime)
        {
            int i = 0;

            for (i = Index; i < ChckpointsList.Count; i++)
            {
                if (!CheckIsAddressSameAsPrevious(ChckpointsList[Index].LocalizationDescription, ChckpointsList[i].LocalizationDescription))
                {
                    if (IsThisStopOrPauseBetweenCheckpoints(ChckpointsList[Index], ChckpointsList[i], CarParkTime))
                    {
                        return i - 1;
                    }
                    else
                    {
                        return (i - 1) * (-1);
                    }
                }
                else if (i == ChckpointsList.Count - 1)
                {
                    return i;
                }
            }
            return 0;
        }
        #endregion


        //IsThisStopOrPauseBetweenCheckpoints() 
        #region IsThisStopOrPauseBetweenCheckpoints() - and return bool
        private bool IsThisStopOrPauseBetweenCheckpoints(CheckPoint Chckpnt1, CheckPoint Chckpnt2, int CarParkTime)
        {

            TimeSpan BreakTime = new TimeSpan(00, CarParkTime, 00);
            if (Chckpnt2.DateTimeReading - Chckpnt1.DateTimeReading >= BreakTime)
            {
                // Console.WriteLine("roznica czasowa: "+(Chckpnt1.DateTimeReading - Chckpnt2.DateTimeReading));
                return true;
            }
            else
                return false;
        }
        #endregion


        //CreateCopyOfCheckPointForWriteToList()  - in another way you will write reference of checkpoint to list; helper for
        #region CreateCopyOfCheckPointForWriteToList()
        private CheckPoint CreateCopyOfCheckPointForWriteToList(CheckPoint TimeOld)
        {
            CheckPoint TempTimeCheckPoint = new CheckPoint(TimeOld.IdDevice, TimeOld.RegNum);
            RewriteCheckpointToAnother(TimeOld, TempTimeCheckPoint);
            return TempTimeCheckPoint;
        }
        #endregion


        //RewriteCheckpointToAnother()
        #region RewriteCheckpointToAnother()
        private void RewriteCheckpointToAnother(CheckPoint CheckPointSource, CheckPoint CheckPointReceipient)
        {
            CheckPointReceipient.DateTimeReading = CheckPointSource.DateTimeReading;
            CheckPointReceipient.Odometer = CheckPointSource.Odometer;
            CheckPointReceipient.Speed = CheckPointSource.Speed;
            CheckPointReceipient.LocalizationDescription = CheckPointSource.LocalizationDescription;
            CheckPointReceipient.X = CheckPointSource.X;
            CheckPointReceipient.Y = CheckPointSource.Y;
        }
        #endregion


        //CheckIfBothCheckpointsAreTheSame() - and return bool
        #region CheckIfBothCheckpointsAreTheSame()
        private bool CheckIfBothCheckpointsAreTheSame(CheckPoint Chckpnt1, CheckPoint Chckpnt2)
        {
            if (Chckpnt1.DateTimeReading.Hour == Chckpnt2.DateTimeReading.Hour && Chckpnt1.DateTimeReading.Minute == Chckpnt2.DateTimeReading.Minute && Chckpnt1.DateTimeReading.Second == Chckpnt2.DateTimeReading.Second)
            {
                if (Chckpnt1.LocalizationDescription == Chckpnt2.LocalizationDescription)
                    return true;
                else
                    return false;
            }
            else
            { return false; }
        }
        #endregion


        //GenerateNodesListsFromXML() - Generate list consisting of lists, which are parameters such as  DateTimeReading, XCoor,YCoor,Speed,Odometer,AddresLocation
        #region  GenerateNodesListsFromXML() 
        private List<XmlNodeList> GenerateNodesListsFromXML(XmlDocument xmlDoc)
        {
            XmlNamespaceManager xmlnsManager = CreateXmlManagerToReadSoapReqest(xmlDoc);

            //pobranie listy ID urzadzen

            List<XmlNodeList> AllNodesListsFoundInXML = new List<XmlNodeList>();

            XmlNodeList DateTimeReading = xmlDoc.SelectNodes("//cma:dtaIdTime", xmlnsManager);
            AllNodesListsFoundInXML.Add(DateTimeReading);

            XmlNodeList XCoor = xmlDoc.SelectNodes("//cma:dblX", xmlnsManager);
            AllNodesListsFoundInXML.Add(XCoor);

            XmlNodeList YCoor = xmlDoc.SelectNodes("//cma:dblY", xmlnsManager);
            AllNodesListsFoundInXML.Add(YCoor);

            XmlNodeList Speed = xmlDoc.SelectNodes("//cma:intSpeed", xmlnsManager);
            AllNodesListsFoundInXML.Add(Speed);

            XmlNodeList Odometer = xmlDoc.SelectNodes("//cma:intOdometer", xmlnsManager);
            AllNodesListsFoundInXML.Add(Odometer);

            XmlNodeList AddressLoc = xmlDoc.SelectNodes("//cma:strDescription", xmlnsManager);
            AllNodesListsFoundInXML.Add(AddressLoc);

            return AllNodesListsFoundInXML;
        }
        #endregion


        //CheckIsAddressSameAsPrevious() - compare 2 addresses and check do they are same or similar
        #region CheckIsAddressSameAsPrevious()
        public static bool CheckIsAddressSameAsPrevious(string first, string second)
        {
            int CountOfNoFittingLetters = 0;

            if ((first.Length - second.Length) == 1 || (second.Length - first.Length) == 1)
            {
                if (first.Length > second.Length)
                {
                    for (int i = 0; i < second.Length; i++)
                    {
                        if (first[i] != second[i]) { return false; }
                    }
                    return true;
                }
                else
                {
                    for (int i = 0; i < first.Length; i++)
                    {
                        if (first[i] != second[i]) { return false; }
                    }
                    return true;
                }
            }
            else if (first.Length != second.Length)
            {

                return false;
            }
            else
            {
                for (int i = 0; i < first.Length; i++)
                {
                    if (first[i] != second[i]) { CountOfNoFittingLetters++; }
                }
                if (CountOfNoFittingLetters > 1)
                    return false;
                else
                    return true;
            }
        }
        #endregion


        //CreateCheckpointBasedOnXMLNodes() - generate checkpoint from list "AllNodesListsFoundInXML" to save later to list of all checkpoints for specific car
        #region CreateCheckpointBasedOnXMLNodes()
        private CheckPoint CreateCheckpointBasedOnXMLNodes(CheckPoint TempCheckpoint, List<XmlNodeList> AllNodesListsFoundInXML, int i)
        {
            CheckPoint TempTimeCheckPoint = new CheckPoint(TempCheckpoint.IdDevice, TempCheckpoint.RegNum);
            return WriteParamToCheckpointFromNodesLists(TempTimeCheckPoint, AllNodesListsFoundInXML, i);

        }
        #endregion


        //WriteParamToCheckpointFromNodesLists() - helper function for CreateCheckpointBasedOnXMLNodes(), to create checkpoint from list of lists of parameters such as speed
        #region WriteParamToCheckpointFromNodesLists()
        private CheckPoint WriteParamToCheckpointFromNodesLists(CheckPoint TempCheckpoint, List<XmlNodeList> AllNodesListsFoundInXML, int IndexOfListOfListsofAllNodes)
        {
            //AllNodesListsFoundInXML(0) - ReadingTime, 
            //AllNodesListsFoundInXML(1) - X Coordinates, 
            //AllNodesListsFoundInXML(2) - Y Coordinates, 
            //AllNodesListsFoundInXML(3) - Speed,
            //AllNodesListsFoundInXML(4) - Odometer,
            //AllNodesListsFoundInXML(5) - AddressLocalizationDescription,


            TempCheckpoint.X = ConvertStrToDouble(AllNodesListsFoundInXML[1].Item(IndexOfListOfListsofAllNodes).InnerText);
          
            TempCheckpoint.Y = ConvertStrToDouble(AllNodesListsFoundInXML[2].Item(IndexOfListOfListsofAllNodes).InnerText);
          //  MessageBox.Show($"X:"+TempCheckpoint.X.ToString()+$"Y: {TempCheckpoint.Y}");
            TempCheckpoint.LocalizationDescription = AllNodesListsFoundInXML[5].Item(IndexOfListOfListsofAllNodes).InnerText;

            int.TryParse(AllNodesListsFoundInXML[3].Item(IndexOfListOfListsofAllNodes).InnerText, out int s);
            TempCheckpoint.Speed = s;

            int.TryParse(AllNodesListsFoundInXML[4].Item(IndexOfListOfListsofAllNodes).InnerText, out int o);
            TempCheckpoint.Odometer = o;

            TempCheckpoint.DateTimeReading = GetTimeFromXML(AllNodesListsFoundInXML[0].Item(IndexOfListOfListsofAllNodes).InnerText);

            return TempCheckpoint;
        }
        #endregion

        private double ConvertStrToDouble(string StrToConvert)
        {
            //Console.WriteLine("jestem w convertstrdoDouble");
            int index = StrToConvert.IndexOf(".");

            StrToConvert = StrToConvert.Replace(StrToConvert.ElementAt(index).ToString(), Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator.ToString());
            double.TryParse(StrToConvert, out double DoubleValue);
            // Console.WriteLine("koncze");
            return DoubleValue;
        }

        private int GetMinutesFromXMLDate(string xml)
        {
            int.TryParse(xml.Substring(3, 2), out int minutes);
            //Console.WriteLine(minutes);

            return minutes;
        }

        private TimeOnly GetTimeFromXML(string xml)
        {
            //Console.WriteLine("koncze gettimefromxml- " );
            xml = xml.Substring(xml.IndexOf("T") + 1);
            //Console.WriteLine(xml);
            int.TryParse(xml.Substring(0, 2), out int hour);
            //Console.WriteLine(hour);
            int.TryParse(xml.Substring(3, 2), out int minutes);
            //Console.WriteLine(minutes);
            int.TryParse(xml.Substring(6, 2), out int seconds);
            // Console.WriteLine("seconds: "+seconds);
            TimeOnly dt = new TimeOnly(hour, minutes, seconds);
            //Console.WriteLine(dt.ToString());
            return dt;
        }

       



        //other helper functions
        //===========================================================================================================
        #region CreateXmlManagerToReadSoapReqest() - helper function for function GetSessionIDFromSoapBody() and GetLogOffFromSoapRequest()
        private XmlNamespaceManager CreateXmlManagerToReadSoapReqest(XmlDocument XmlDoc)
        {
            XmlNamespaceManager XmlNsManagerTemp = new System.Xml.XmlNamespaceManager(XmlDoc.NameTable);

            XmlNsManagerTemp.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
            XmlNsManagerTemp.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            XmlNsManagerTemp.AddNamespace("xsd", "http://www.w3.org/2001/XMLSchema");
            XmlNsManagerTemp.AddNamespace("cma", "http://www.cma.com.pl/");

            return XmlNsManagerTemp;
        }
        #endregion



        //=======================================================================
        //                 OTHER  METHODS
        //=======================================================================   
        #region InitializeHttpObject() -  function to create one instance of object HttpObject, to communication by SOAP protocol with to Viasat
        private void InitializeHttpObject()
        {
            this._HttpObjectForCommunicationWithApi = new HttpObjectForCommunicationWithApi();
        }
        #endregion

        #region INotifyPropertyChanged Implement
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }
}
