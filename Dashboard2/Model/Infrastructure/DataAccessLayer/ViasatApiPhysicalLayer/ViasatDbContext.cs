using Dashboard2.Model.Domain;
using Dashboard2.Model.Infrastructure.Repositories.ViasatApi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shell;
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



        //=========================================================================================================================
        //                   CONSTRUCTOR
        //=========================================================================================================================   
        public ViasatDbContext()
        {
           InitializeHttpObject();           
           LoginUser();
            Task.Run( () => GetClientObjects());
          // await GetClientObjects();

        }





        //=======================================================================
        //                   API VIASAT Main Methods
        //=======================================================================    

        //LoginUser()
        #region LoginUser() - Viasat API function to login to Viasat interface
        private async void LoginUser()
        {
            string login = AppGeneralConfigStaticClass.ApiLogin;
            string password =  AppGeneralConfigStaticClass.ApiPassword;
                       
            //declaration of universal function (in Httpobject) to send and receive SOAP message: SendAndReceiveHttpRequestNew(string HttpRequestBody, string SOAPAction) 
                Task<string> TaskResult = _HttpObjectForCommunicationWithApi.SendAndReceiveHttpRequestNew(ViasatMethodsTemplateStaticClass.LoginUserTemplate(login, password), "LoginUser");
            try
            {               
                string ReceivedSoapBody = await TaskResult.WaitAsync(TimeSpan.FromSeconds(20));
                if (ReceivedSoapBody != null)
                {                
                    this._SessionId = GetSessionIdFromSoapBody(ReceivedSoapBody);
                    View.CommunicationWindow.Message.ShowCommunicationWindow(this._SessionId);
                    if (this._SessionId == "0")
                    {
                        MessageBox.Show("Nie udało się zalogować do Api Viasat,\n niepoprawne dane logowania");
                        //throw new AccessViolationException("Nie udało się zalogować do Api Viasat,\n niepoprawne dane logowania");

                    }
                }               
              
            }
            catch(TimeoutException) { MessageBox.Show("Nie udalo sie polaczyc z Api Viasat\n upłynął czas próby połączenia."); }
            catch(Exception ex) { MessageBox.Show(ex.Message); }
            
        }
        #endregion


        //LogOffSession()
        #region LogOffSession() - Viasat API function to logout from Viasat interface
        public async Task LogOffSession()
        {      
            Task<string> TaskResult = _HttpObjectForCommunicationWithApi.SendAndReceiveHttpRequestNew(ViasatMethodsTemplateStaticClass.LogOffSessionTemplate(this._SessionId), "LogOffSessionTemplate");

            string ReceivedSoapBody = await TaskResult.WaitAsync(TimeSpan.FromSeconds(20));
           

                string LogOffStatus = (GetLogOffStatusFromSoapBody(ReceivedSoapBody));

                if (LogOffStatus != null)
                {
                    MessageBox.Show("Status wylogowania: " + LogOffStatus);
                }
                else
                {
                    MessageBox.Show("Wartość LogOffStatus = null");
                }

        }
        #endregion



      //  Task<ObservableCollection<ViasatClientObject>> TaskGetClient = Task.Run(() => ViasatApiDbContext.GetClientObjects());
      //  CarsIdAndRegnumListFromApi = TaskGetClient.Result;





        //GetClientObjects()
        #region  GetClientObjects() - Viasat API function to get list of registration numbers of cars and assigned to them Id Gps-devices
        private async Task GetClientObjects()
        {      
            Task<string> TaskResult = _HttpObjectForCommunicationWithApi.SendAndReceiveHttpRequestNew(ViasatMethodsTemplateStaticClass.GetClientObjectTemplate(this._SessionId), "GetClientObjects");

            try
            {
                string ReceivedSoapBody = await TaskResult.WaitAsync(TimeSpan.FromSeconds(20));
              
               
               // MessageBox.Show(ReceivedSoapBody);
                  //  WaitAsync(TimeSpan.FromSeconds(20));
                if (ReceivedSoapBody != null)
                {
                  //  MessageBox.Show("odp liczba znakow: "+ReceivedSoapBody.Count().ToString());
                    this.CarsIdAndRegnumListFromApi =  GetClientListFromSoapBody(ReceivedSoapBody);
                }
               
            }
           catch (TimeoutException) { MessageBox.Show("Nie udalo sie uzyskać odpowiedzi z Api\n upłynął czas oczekiwania na odpowiedź."); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

            
        }
        #endregion


        //GetLocationsExNc()
        #region  GetLocationsExNC() - Viasat API function to get list of time checkpoints for a specific car
        public  async Task<List<CheckPoint>> GetLocationsExNC(ViasatClientObject SelectedAuto, DateTime StartDate, DateTime EndDate)
        {
            Task<string> TaskResult = _HttpObjectForCommunicationWithApi.SendAndReceiveHttpRequestNew(ViasatMethodsTemplateStaticClass.GetLocationsExNCTemplate(this._SessionId, SelectedAuto.Id,StartDate,EndDate), "GetLocationsExNC");   
            
            CheckPoint CheckPoint = new CheckPoint(SelectedAuto.Id, SelectedAuto.Name);

            try
            {
                string ReceivedSoapBody = await TaskResult.WaitAsync(TimeSpan.FromSeconds(20));

                if (ReceivedSoapBody != null)
                {
                    return await CreateCheckpointsListFromSoapResponse(ReceivedSoapBody, CheckPoint);
                }

            }
            catch (TimeoutException) { MessageBox.Show("Nie udalo sie uzyskać odpowiedzi z Api Viasat\n upłynął czas oczekiwania na odpowiedź."); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

            return null;
                
        }
        #endregion

        //GetDeviceStatisticFlow()
        #region GetDeviceStatisticFlow() Viasat API function to get list of numbers of km's in selected duration for every car
        public ObservableCollection<ViasatClientObject> GetDeviceStatisticFlow(string dateFrom, string dateTo)
        {         
            Task<string> TaskResult = _HttpObjectForCommunicationWithApi.SendAndReceiveHttpRequestNew(ViasatMethodsTemplateStaticClass.GetDeviceStatisticFlowTemplate(this._SessionId, dateFrom, dateTo), "GetDeviceStatisticFlow");
            //MessageBox.Show("mam odpowiedz");
            try
            {
                Task.WaitAll(TaskResult);
                MessageBox.Show("1. mam odpowiedz z API");
                string ReceivedSoapBody = TaskResult.Result;
                //  MessageBox.Show("mam odpowiedz z API, ilosc znakow: "+ReceivedSoapBody.Count());
                MessageBox.Show(ReceivedSoapBody);
                if (ReceivedSoapBody != null)
                {
                    
                    var ResultList = GetMileageOfKilometersFromSoapBodyForAllCars(ReceivedSoapBody);
                   // MessageBox.Show("liczba odczytanych aut z przebiegiem: "+ResultList.Count());
                    MessageBox.Show("2. pierwszy element w tej liscie: "+ResultList[1].Id+" -> " + ResultList[1].NumberOfKilometres+"!!!");
                    //var task = GetClientObjects();
                    
                    var TempListOfGetClientObjectsMethod = GetClientObjects();
                    MessageBox.Show("3. lista aut z getclientobject: "+TempListOfGetClientObjectsMethod.Count().ToString());
                    //MessageBox.Show("rozpoczynam petle");
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

                        return ResultList;
                    }
                    else
                        return ResultList;
               
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
        public string GetLogOffStatusFromSoapBody(string ReceivedSoapBody)
        {
           // MessageBox.Show(ReceivedSoapBody);

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

            ObservableCollection<ViasatClientObject> ReturnCarList = new ObservableCollection<ViasatClientObject>();

            for (int i = 0; i < IdDeviceList.Count; i++)
            {
                //Console.WriteLine(IdDeviceList[i].ToString() + " = " + MileageList[i].ToString());
               
                ReturnCarList.Add(new ViasatClientObject(IdDeviceList[i], RegNumList[i]));
            }
            //MessageBox.Show(ReturnCarList[4].Name);
            MessageBox.Show($"el 1 z listy wszystkich nodow: {ReturnCarList[0].Name}  {ReturnCarList[0].Id}");
            return ReturnCarList;
        }
        #endregion

      
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
           // MessageBox.Show("ilosc nodow z przebiegiem: " + IdDeviceList.Count);

            //create list from all ObjectName nodes from Soap body message
            XmlNodeList ObjectNameNodesList = xmlDoc.SelectNodes("//cma:dblDistance", XmlNsManager);
            List<string> MileageList = new List<string>();
            foreach (XmlNode node in ObjectNameNodesList)
            {
                MileageList.Add(node.InnerText);
            }
            MessageBox.Show("ilosc nodow z przebiegiem: " + MileageList.Count);
            ObservableCollection<ViasatClientObject> ReturnCarList = new ObservableCollection<ViasatClientObject>();

            for (int i = 0; i < IdDeviceList.Count; i++)
            {
                //Console.WriteLine(IdDeviceList[i].ToString() + " = " + MileageList[i].ToString());

                ReturnCarList.Add(new ViasatClientObject(IdDeviceList[i], MileageList[i]));
            }
            //MessageBox.Show(ReturnCarList[4].Name);
            
            return ReturnCarList;
        }



        //CreateCheckpointsListFromSoapResponse() 
        #region CreateCheckpointsListFromSoapResponse()
        public async Task<List<CheckPoint>> CreateCheckpointsListFromSoapResponse(string ReceivedSoapBody, CheckPoint TempCheckpoint)
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
                                int index = CompareCheckpointsEachOtherToFindEndOfPauseOrBrake(T1Index, AllCheckpointsfromXML);

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
                                int index = CompareCheckpointsEachOtherToFindEndOfPauseOrBrake(T2Index, AllCheckpointsfromXML);
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
                                int index = CompareCheckpointsEachOtherToFindEndOfPauseOrBrake(T2Index, AllCheckpointsfromXML);
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
                return ListOfCheckPointsForSummaryOfResult;
            }
            else
                return null;
        }
        #endregion


        //CompareCheckpointsEachOtherToFindEndOfPauseOrBrake  - find end checkpoint which is as same as our and compare them is this brake or pause
        #region CompareCheckpointsEachOtherToFindEndOfPauseOrBrake()
        private int CompareCheckpointsEachOtherToFindEndOfPauseOrBrake(int Index, List<CheckPoint> ChckpointsList)
        {
            int i = 0;

            for (i = Index; i < ChckpointsList.Count; i++)
            {
                if (!CheckIsAddressSameAsPrevious(ChckpointsList[Index].LocalizationDescription, ChckpointsList[i].LocalizationDescription))
                {
                    if (IsThisStopOrPauseBetweenCheckpoints(ChckpointsList[Index], ChckpointsList[i]))
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
        private bool IsThisStopOrPauseBetweenCheckpoints(CheckPoint Chckpnt1, CheckPoint Chckpnt2)
        {

            TimeSpan BreakTime = new TimeSpan(00, 15, 00);
            if (Chckpnt2.DateTimeReading - Chckpnt1.DateTimeReading > BreakTime)
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
