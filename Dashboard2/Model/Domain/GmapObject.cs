using GMap.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using System.Drawing;
using System.Collections.ObjectModel;
using System.ComponentModel;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using System.Runtime.CompilerServices;
using Dashboard2.Model.Infrastructure.DataAccessLayer.ViasatApiPhysicalLayer;
using System.Diagnostics;
using System.Reflection;

namespace Dashboard2.Model.Domain
{
    public class GmapObject
    {
        // 53.03743273173656, 18.63845506491909
        public GMap.NET.WindowsForms.GMapControl MapObject { get; set; }
        private GMap.NET.WindowsForms.GMapOverlay markersOverlay;
        private GMap.NET.WindowsForms.GMapOverlay CompanyBranchLayer;
        public GMap.NET.WindowsForms.GMapOverlay CheckpointsForSelectedCarLayer { get; set; }
        private List<CheckpointMarkerForMap> checkpointMarkerForMaps;
     //   public List<CheckpointMarkerForMap> CheckpointMarkerForMaps { get { return checkpointMarkerForMaps; } set { checkpointMarkerForMaps = value; } }
        
        private GMap.NET.WindowsForms.GMapOverlay RouteForSelectedCarLayer;

        private double srednia_lat { get; set; }
       private double srednia_lng { get; set; }



        //=====================================================================
        //                   CONSTRUCTOR
        //=====================================================================
      
        public GmapObject()
        {
            InitializeGmap();
           
          //  this.CompanyBranchLayer = new GMap.NET.WindowsForms.GMapOverlay("CompanyBranchLayer");

            InitialCompanyBranchLayer();
          
  

            UstawMape();
        }


        //=====================================================================
        //                   METHODS
        //=====================================================================
      
        private CompanyBranch  CheckIsCheckpointNextToBranch(CheckPoint checkpoint)
        {
            Location loc1 = new Location(){Longitude = checkpoint.X, Latitude = checkpoint.Y};
          //  MessageBox.Show($"Longitude = {checkpoint.X}, Latitude = {checkpoint.Y}");

            Location loc2 = new Location();
            
                foreach (var el in AppGeneralConfigStaticClass.ListOfCompanyBranches)
                {

                loc2.Longitude = el.X;
                loc2.Latitude = el.Y;

                    if(  Location.CalculateDistance(loc1, loc2) < 100.00)
                    {
                        return el;
                    }

                }

            return null;
            
        }

        private bool CheckIsCheckpointNextToOtherCheckpoint(CheckPoint presentCheckpoint, CheckPoint previousCheckpoint )
        {
            Location loc1 = new Location() { Longitude = presentCheckpoint.X, Latitude = presentCheckpoint.Y };

            Location loc2 = new Location() { Longitude = previousCheckpoint.X, Latitude = previousCheckpoint.Y };

            if (Location.CalculateDistance(loc2, loc1) < 100.00)
                return true;
            else
                return false;

        }

      
        
   //=================================================================================================================================
     
        private List<PointLatLng> InitializeRoutesBetweenCheckpoints(PointLatLng startPoint, PointLatLng endPoint, int index)
        {
            int Index = index;
            MapRoute Route;
            GMapRoute RouteForDrawing;

            Route = OpenStreetMapProvider.Instance.GetRoute(startPoint, endPoint, false, false, 12);
 
            return Route.Points;

           // MapObject.Invoke(new Action(() => MapObject.Update()));
           // MapObject.Invoke(new Action(() => MapObject.Refresh()));

        }



        public ObservableCollection<ObservableCollection<string>> InitializeCheckpointMarkersListAndRoutes(List<CheckPoint> checkpointsList, int CarParkTime, DTOForGetLocationsForCar CarObject, Func<DateTime, DateTime, string, string> GetMileageFromApiDelegate)
        { 
            List<string> lista = new List<string>();
                      

            if (checkpointsList.Count != 0 || checkpointsList != null)
            {
                //clear CheckpointLayer
                if (MapObject.Overlays.Any(x => x.Id == "CheckpointsLayer"))
                {
                    CheckpointsForSelectedCarLayer.Markers.Clear();                  
                    this.checkpointMarkerForMaps = new List<CheckpointMarkerForMap>();
                     MapObject.Invoke(new Action(() => MapObject.Update()));
                     MapObject.Invoke(new Action(() => MapObject.Refresh()));
                }
                //clear RoutesLayer
                if (MapObject.Overlays.Any(x => x.Id == "RoutesLayer"))
                {
                    this.RouteForSelectedCarLayer.Routes.Clear();
                    MapObject.Invoke(new Action(() => MapObject.Update()));
                    MapObject.Invoke(new Action(() => MapObject.Refresh()));
                }

                //init checkpoint and route layers
                this.CheckpointsForSelectedCarLayer = new GMap.NET.WindowsForms.GMapOverlay("CheckpointsLayer");
                this.checkpointMarkerForMaps = new List<CheckpointMarkerForMap>();
                MapObject.Overlays.Add(this.CheckpointsForSelectedCarLayer);

                this.RouteForSelectedCarLayer = new GMapOverlay("RoutesLayer");

                Location tempLocation = new Location();
                string? tempLocationString = CheckIsCheckpointNextToBranch(checkpointsList[0]) != null ? CheckIsCheckpointNextToBranch(checkpointsList[0]).Name + " (ODDZIAŁ)" :  checkpointsList[0].LocalizationDescription;        
                CheckpointMarkerForMap? tempCheckpoint = new CheckpointMarkerForMap(1, checkpointsList[0].DateTimeReading, checkpointsList[0].DateTimeReading, tempLocationString, checkpointsList[0].X, checkpointsList[0].Y);

                var PointsForDrawingRoute = new List<PointLatLng>();

                /*
                                Stopwatch stopWatch = new Stopwatch();
                                stopWatch.Start();
                */



                for (int i = 1, index=1; i < checkpointsList.Count; i++)
                {                 
                    //check is distance between checkpoint <100
                    if (CheckIsCheckpointNextToOtherCheckpoint(checkpointsList[i], checkpointsList[i-1]))
                    {
                        tempCheckpoint.EndReadingTime = checkpointsList[i].DateTimeReading;
                        if(i== checkpointsList.Count-1)
                    {
                       this.checkpointMarkerForMaps.Add(tempCheckpoint);
                    }          
                    }
                    //distance between checkpoint > 100
                    else
                    {
                        if(tempCheckpoint != null)
                        {
                            this.checkpointMarkerForMaps.Add(tempCheckpoint);
                            tempCheckpoint = null;
                        }
                                              
                        var j = i;
                        var Index = index;

                        PointsForDrawingRoute.AddRange(  InitializeRoutesBetweenCheckpoints(new PointLatLng(checkpointsList[j - 1].Y, checkpointsList[j - 1].X), new PointLatLng(checkpointsList[j].Y, checkpointsList[j].X), Index));
                                        
                         if (i != checkpointsList.Count - 1)
                        {
                            if (CheckIsCheckpointNextToOtherCheckpoint(checkpointsList[i + 1], checkpointsList[i]))
                            {
                                index++;
                                tempLocationString = CheckIsCheckpointNextToBranch(checkpointsList[i]) != null ? CheckIsCheckpointNextToBranch(checkpointsList[i]).Name + " (ODDZIAŁ)" : checkpointsList[i].LocalizationDescription;
                                tempCheckpoint = new CheckpointMarkerForMap(index, checkpointsList[i].DateTimeReading, checkpointsList[i].DateTimeReading, tempLocationString, checkpointsList[i].X, checkpointsList[i].Y);
                            }
                        }                             
                    }                                        
                }

/*
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;

                string elapsedTime = $"etap 1 (petla):{stopWatch.Elapsed.Milliseconds}\n";
                lista.Add(elapsedTime);
                stopWatch.Start();
*/
 
                Action action1 = () =>
                {
                    var RouteForDrawing = new GMapRoute(PointsForDrawingRoute, $"route{1}");
                    RouteForDrawing.Stroke = new System.Drawing.Pen(System.Drawing.Color.DeepSkyBlue, 4);
                    this.RouteForSelectedCarLayer.Routes.Add(RouteForDrawing);
                    MapObject.Overlays.Add(this.RouteForSelectedCarLayer);


                };
                                         
/*
                stopWatch.Stop();
                 ts = stopWatch.Elapsed;

                elapsedTime = $"etap 2 (generowanie drogi):{stopWatch.Elapsed.Milliseconds}\n";    
                lista.Add(elapsedTime);
                stopWatch.Start();

                */
                ObservableCollection<ObservableCollection<string>> ListOfSummaryResultForSelectedCar = new ObservableCollection<ObservableCollection<string>>();

                Action action2 = () =>
                {
                    int RemberedIndexOfThePreviousCar = 0;
                    for (int i = 0, index = 1; i < this.checkpointMarkerForMaps.Count; i++)
                    {
                        if(i == 0)
                        {
                            checkpointMarkerForMaps[i].Id = index;
                            index++;
                            AddMarkerToMarkersLayer(checkpointMarkerForMaps[i].Y, checkpointMarkerForMaps[i].X, $"\n{checkpointMarkerForMaps[i].Id} -> {checkpointMarkerForMaps[i].StartReadingTime}-{checkpointMarkerForMaps[i].EndReadingTime}\n{checkpointMarkerForMaps[i].Address}");
                            ListOfSummaryResultForSelectedCar.Add(new ObservableCollection<string>() { $"{checkpointMarkerForMaps[i].Id}", $"{checkpointMarkerForMaps[i].StartReadingTime}-{checkpointMarkerForMaps[i].EndReadingTime}", "POSTÓJ", $"{checkpointMarkerForMaps[i].Address}" });
                            
                        }
                        else if (IsTheCheckpointABrake(checkpointMarkerForMaps[i], CarParkTime))
                        {
                            DateTime startDate = new DateTime(DateOnly.FromDateTime(CarObject.DateFrom), new TimeOnly( checkpointMarkerForMaps[RemberedIndexOfThePreviousCar].StartReadingTime.Hour, checkpointMarkerForMaps[RemberedIndexOfThePreviousCar].StartReadingTime.Minute, checkpointMarkerForMaps[RemberedIndexOfThePreviousCar].StartReadingTime.Second));
                            var IndexOfStartReadingTime = RemberedIndexOfThePreviousCar;
                            RemberedIndexOfThePreviousCar = i;

                            DateTime endDate = new DateTime(DateOnly.FromDateTime(CarObject.DateTo), new TimeOnly( checkpointMarkerForMaps[RemberedIndexOfThePreviousCar].EndReadingTime.Hour, checkpointMarkerForMaps[RemberedIndexOfThePreviousCar].EndReadingTime.Minute, checkpointMarkerForMaps[RemberedIndexOfThePreviousCar].EndReadingTime.Second));
                            string tempMileage = GetMileageFromApiDelegate(startDate, endDate, CarObject.Id);

                            ListOfSummaryResultForSelectedCar.Add(new ObservableCollection<string>() { $"{index}", $"{checkpointMarkerForMaps[IndexOfStartReadingTime].StartReadingTime}-{checkpointMarkerForMaps[RemberedIndexOfThePreviousCar].EndReadingTime}", "PRZEJAZD", $"({tempMileage} km)" });
                            index++;
                            checkpointMarkerForMaps[i].Id = index;    
                            index++;

                            AddMarkerToMarkersLayer(checkpointMarkerForMaps[i].Y, checkpointMarkerForMaps[i].X, $"\n{checkpointMarkerForMaps[i].Id} -> {checkpointMarkerForMaps[i].StartReadingTime}-{checkpointMarkerForMaps[i].EndReadingTime}\n{checkpointMarkerForMaps[i].Address}");
                            ListOfSummaryResultForSelectedCar.Add(new ObservableCollection<string>() { $"{checkpointMarkerForMaps[i].Id}", $"{checkpointMarkerForMaps[i].StartReadingTime}-{checkpointMarkerForMaps[i].EndReadingTime}", "POSTÓJ", $"{checkpointMarkerForMaps[i].Address}" });
                          }
                        else if (i == this.checkpointMarkerForMaps.Count - 1)
                        {
                            checkpointMarkerForMaps[i].Id = index;
                            index++;
                            AddMarkerToMarkersLayer(checkpointMarkerForMaps[i].Y, checkpointMarkerForMaps[i].X, $"\n{checkpointMarkerForMaps[i].Id} -> {checkpointMarkerForMaps[i].StartReadingTime}-{checkpointMarkerForMaps[i].EndReadingTime}\n{checkpointMarkerForMaps[i].Address}");
                            ListOfSummaryResultForSelectedCar.Add(new ObservableCollection<string>() { $"{checkpointMarkerForMaps[i].Id}", $"{checkpointMarkerForMaps[i].StartReadingTime}-{checkpointMarkerForMaps[i].EndReadingTime}", "POSTÓJ", $"{checkpointMarkerForMaps[i].Address}" });
                        }
                    }


                    DateTime startDate2 = new DateTime(DateOnly.FromDateTime(CarObject.DateFrom), new TimeOnly(checkpointMarkerForMaps[0].StartReadingTime.Hour, checkpointMarkerForMaps[0].StartReadingTime.Minute, checkpointMarkerForMaps[0].StartReadingTime.Second));
                    DateTime endDate2 = new DateTime(DateOnly.FromDateTime(CarObject.DateTo), new TimeOnly(checkpointMarkerForMaps[this.checkpointMarkerForMaps.Count-1].EndReadingTime.Hour, checkpointMarkerForMaps[this.checkpointMarkerForMaps.Count-1].EndReadingTime.Minute, checkpointMarkerForMaps[this.checkpointMarkerForMaps.Count-1].EndReadingTime.Second));
                    
                    string TotalMileage = GetMileageFromApiDelegate(startDate2, endDate2, CarObject.Id);
                    ListOfSummaryResultForSelectedCar.Add(new ObservableCollection<string>() { "", "", "", "" });
                    ListOfSummaryResultForSelectedCar.Add(new ObservableCollection<string>() { "", "", "", $"SUMA: {TotalMileage}km" });
               

                };


                List<Action> list = new List<Action>();
                list.Add(action1);
                list.Add(action2);
                Parallel.Invoke(list.ToArray());


/*
                    stopWatch.Stop();
                ts = stopWatch.Elapsed;

                elapsedTime = $"etap 3 (po wygenerowaniu markerow):{stopWatch.Elapsed.Milliseconds}";
                lista.Add(elapsedTime);
*/


                MapObject.Invoke(new Action(() => MapObject.Zoom = 10)); 
                MapObject.Invoke(new Action(() => MapObject.Zoom = 11));
                // MapObject.Invoke(new Action(() => MapObject.ReloadMap()));
                string finalstring = "";
                foreach(var el in lista)
                {
                    finalstring += el;
                }
             //  MessageBox.Show(finalstring);
                return ListOfSummaryResultForSelectedCar;
            }
            else

            return null;
        }




        private bool IsTheCheckpointABrake(CheckpointMarkerForMap Chckpnt,  int CarParkTime)
        {
            TimeSpan BreakTime = new TimeSpan(00, CarParkTime, 00);
            if (Chckpnt.EndReadingTime - Chckpnt.StartReadingTime >= BreakTime)
            {
                return true;
            }
            else
                return false;
        }




        //=============================================================================================================================================================
        private void InitializeRoutesBetweenCheckpoints(List<CheckpointMarkerForMap> checkpointList)
        {
            this.RouteForSelectedCarLayer = new GMapOverlay("RoutesLayer");
            // var route = GoogleMapProvider.Instance.GetRoadsRoute(points, true);


            List<PointLatLng> points = new List<PointLatLng>();
            foreach (var checkpoint in checkpointList)
            {
                points.Add(new PointLatLng(checkpoint.Y, checkpoint.X));
            }


            MapRoute Route;
            GMapRoute RouteForDrawing;
            for (int i = 0; i < points.Count; i++)
            {
                Route = OpenStreetMapProvider.Instance.GetRoute(points[i], points[i + 1], false, false, 12);
                RouteForDrawing = new GMapRoute(Route.Points, $"route{i + 1}");
                RouteForDrawing.Stroke = new System.Drawing.Pen(System.Drawing.Color.DeepSkyBlue, 3);
                this.RouteForSelectedCarLayer.Routes.Add(RouteForDrawing);

                if (i == points.Count - 2)
                    break;
            }


            //  System.Windows.MessageBox.Show("dystans: " + route.Distance);
            // BingMapProvider.Instance.GetRoute()
            // var route2 = OpenStreetMapProvider.Instance.GetRoute(points[1], points[2], false, false, 12);
            //  System.Windows.MessageBox.Show("czerwony, liczba km" + route2.Distance.ToString());

            MapObject.Overlays.Add(this.RouteForSelectedCarLayer);

        }

        public void InitializeGmap()
        {
            MapObject = new GMap.NET.WindowsForms.GMapControl();
            MapObject.MapProvider = GMap.NET.MapProviders.GMapProviders.GoogleMap;
            MapObject.MinZoom = 1;
            MapObject.MaxZoom = 20;
            MapObject.Dock = DockStyle.Fill;
            MapObject.DragButton = MouseButtons.Left;
            MapObject.ShowCenter = false;
        
            MapObject.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(gmap_MouseDoubleClick);                                                             ////object? sender, System.Windows.Forms.MouseEventArgs e
            MapObject.MouseClick += new System.Windows.Forms.MouseEventHandler(gmap_MouseOneClick);
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerAndCache;
        }

        private void InitialCompanyBranchLayer()
        {
            this.CompanyBranchLayer = new GMap.NET.WindowsForms.GMapOverlay("CompanyBranchLayer");
            MapObject.Overlays.Add(this.CompanyBranchLayer);

            foreach(var branch in AppGeneralConfigStaticClass.ListOfCompanyBranches)
            {
                AddMarkerForCompanyBranch( branch.Y, branch.X, $"\n{branch.Id} - {branch.Name}\n{branch.Address}");
            }
            // this.MapObject.ZoomAndCenterMarkers(this.CompanyBranchLayer.Id);
        }


        

        private void AddMarkerToMarkersLayer(double lat, double lng, string description)
        {            
            //new PointLatLng(Convert.ToDouble(latitude.Text), Convert.ToDouble(longitude.Text)),

            var marker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(
                new PointLatLng(lat, lng),
                GMap.NET.WindowsForms.Markers.GMarkerGoogleType.blue_pushpin);

            marker.ToolTipText = description;
           
            marker.ToolTip.Fill = System.Drawing.Brushes.AliceBlue;
            marker.ToolTip.Foreground = System.Drawing.Brushes.Black;
            marker.ToolTip.Stroke = Pens.Black;
            marker.ToolTip.TextPadding = new System.Drawing.Size(5, 5);
          
            this.MapObject.UpdateMarkerLocalPosition(marker);
            this.CheckpointsForSelectedCarLayer.Markers.Add(marker);

          
            MapObject.Invoke(new Action(() =>  marker.Overlay.Control.ZoomAndCenterMarkers(this.CheckpointsForSelectedCarLayer.Id)));
           
    
        //    MapObject.Update();
            //  marker.Overlay.Control.ZoomAndCenterMarkers(this.CheckpointsForSelectedCarLayer.Id);

        }


        private void AddMarkerForCompanyBranch(double lat, double lng, string s)
        {
            //Layer count is just a variable to add new OverLays with different names
            var marker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(
                new PointLatLng(lat, lng),
                GMap.NET.WindowsForms.Markers.GMarkerGoogleType.blue_small);

            marker.ToolTipText = s;
            SolidBrush myBrush = new SolidBrush(System.Drawing.Color.FromArgb(255, 45, 83, 218));

            marker.ToolTip.Fill = myBrush;
            marker.ToolTip.Foreground = System.Drawing.Brushes.White;
            marker.ToolTip.Stroke = Pens.Black;
            marker.ToolTip.TextPadding = new System.Drawing.Size(5, 5);
            
           this.MapObject.UpdateMarkerLocalPosition(marker);
            this.CompanyBranchLayer.Markers.Add(marker);
       
          //  this.CompanyBranchLayer.Control.ZoomAndCenterMarkers(this.CompanyBranchLayer.Id);
            //  marker.Overlay.Control.ZoomAndCenterMarkers(CompanyBranchLayer.Id);
            //    marker.Overlay.Control.ZoomAndCenterMarkers("CompanyBranchLayer");
            // marker.Overlay.Control.ZoomAndCenterMarkers(markersOverlay.Id);

        }


        private void DodajMarker(double lat, double lng)
        {
            //Layer count is just a variable to add new OverLays with different names
            // var markersOverlay = new GMap.NET.WindowsForms.GMapOverlay("marker1");

            //Marker far away in Quebec, Canada just to check my point in discussion    
            var marker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(
              //new PointLatLng(Convert.ToDouble(latitude.Text), Convert.ToDouble(longitude.Text)),
              new PointLatLng(lat, lng),
              GMap.NET.WindowsForms.Markers.GMarkerGoogleType.red_pushpin);

            marker.ToolTipText = $"pozycja markera (localposition) - X:{marker.LocalPosition.X.ToString()} Y:{marker.LocalPosition.Y.ToString()}\npozycja markera (localposition) - {marker.Position.ToString()}";
            marker.ToolTip.Fill = System.Drawing.Brushes.Black;
            marker.ToolTip.Foreground = System.Drawing.Brushes.White;
            marker.ToolTip.Stroke = Pens.Black;
            marker.ToolTip.TextPadding = new System.Drawing.Size(20, 20);
            // System.Windows.MessageBox.Show("pozycja markera - X:"+ marker.LocalPosition.X.ToString()+", Y:"+marker.po.LocalPosition.Y.ToString());
            // marker.


            // BY NIE BYLO PROBLEMU ZE PIERWSZY MARKER POJAWIA SIE W ZLYM MIEJSCU TO
            // ALBO DODAJ MapObject.UpdateMarkerLocalPosition(marker);
            //ALBO NAJPIERW DODAJ WARSTWE Z MARKERAMI DO GMAP A DOPIERO POTEM MARKER DO TEJ WARSTWY
            MapObject.UpdateMarkerLocalPosition(marker);
            markersOverlay.Markers.Add(marker);
            MapObject.Overlays.Add(markersOverlay);

            // System.Windows.MessageBox.Show("MapObject.:" + MapObject.Zoom.ToString());

            marker.Overlay.Control.ZoomAndCenterMarkers(markersOverlay.Id);

        }




      


        private void UstawMape()
        {
            //MapObject.Position = new PointLatLng(Convert.ToDouble(latitude.Text), Convert.ToDouble(longitude.Text));

            //MapObject.Position = new PointLatLng(50.7200783, 23.264245);
            MapObject.Position = new PointLatLng(52.16315010387308, 19.108330673707925);
            MapObject.Zoom = 6.0;
            MapObject.Update();
            MapObject.Refresh();
        }

       


        public void fun1(object? sender, System.EventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("fun1 wyzwolona");
        }


        private void gmap_MouseOneClick(object? sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                System.Windows.Forms.MessageBox.Show("gmap_oneclick 2 wyzwolona");
            }
        }

        private void gmap_MouseDoubleClick(object? sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                double lat = MapObject.FromLocalToLatLng(e.X, e.Y).Lat;
                double lng = MapObject.FromLocalToLatLng(e.X, e.Y).Lng;
                DodajMarker(lat, lng);
                // throw new NotImplementedException();
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                System.Windows.Forms.MessageBox.Show("gmap_doubleclick wyzwolona");
            }
        }


     

      




       


    }
}
