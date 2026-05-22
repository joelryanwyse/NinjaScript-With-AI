
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml.Linq;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Core;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.Data;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.NinjaScript;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Gui.Tools;
using NinjaTrader.NinjaScript;
using NinjaTrader.NinjaScript.DrawingTools;
using NinjaTrader.NinjaScript.Indicators;
using SharpDX.DirectWrite;


// This namespace holds indicators in this folder and is required. Do not change it.
namespace NinjaTrader.NinjaScript.Indicators
{
	
	
	[Gui.CategoryOrder("Parameters ", 2)]
	
	
	[Gui.CategoryOrder("Calculate Events", 3)]
	[Gui.CategoryOrder("Latest Pivots Display", 4)]	
	
	
	[Gui.CategoryOrder("Minute Data 1", 6)]
	[Gui.CategoryOrder("Minute Data 2", 7)]
	[Gui.CategoryOrder("Minute Data 3", 8)]
	[Gui.CategoryOrder("Daily Data", 9)]
	[Gui.CategoryOrder("Weekly Data", 10)]
	
	
	[Gui.CategoryOrder("Time Filter", 21)]
	[Gui.CategoryOrder("Display", 30)]
	[Gui.CategoryOrder("Zone Strength", 31)]
	[Gui.CategoryOrder("Zone Freshness", 32)]
	[Gui.CategoryOrder("Zone Export", 33)]
	
	
	[Gui.CategoryOrder("Chart Buttons", 40)]
	
	
	[Gui.CategoryOrder("Visual", 156)]
	[Gui.CategoryOrder("Data Series", 165)]
	
	
	[Gui.CategoryOrder("Setup", 9000)]
	[Gui.CategoryOrder("License", 10000)]
	
	
	[TypeConverter("NinjaTrader.NinjaScript.Indicators.aiSRPriceActionConverter")]	
	public class aiSRPriceAction : Indicator
	{
		
		private string ThisName = "aiSRPriceAction";
		
		
		private string LicensingMessage = string.Empty;		

		private SortedDictionary<int, int> ProductIDToMachineIDs = new SortedDictionary<int, int>();
		private ConcurrentDictionary<int, List<string>> ProductIDToInstruments = new ConcurrentDictionary<int, List<string>>();
	
		
		private bool Permission = false;
		

		private void AddError(string eee)
		{
		
			if (pErrorMessagesEnabled)
			if (!AllErrorMessages.Contains(eee))
				AllErrorMessages.Add(eee);
			
		}
		private bool pErrorMessagesEnabled = true;		
			private List<string> AllErrorMessages = new List<string>();		
		

		private string Data1Name = string.Empty;
		private string Data2Name = string.Empty;
		private string Data3Name = string.Empty;
		
		private string Data1NameFinal = string.Empty;
		private string Data2NameFinal = string.Empty;
		private string Data3NameFinal = string.Empty;		
		
		
		private string DailyDataName = string.Empty;
		private string WeeklyDataName = string.Empty;
		
		private DateTime CurrentChartTime = DateTime.MaxValue;
			
		private bool LicenseWordPress (string machineid, string pLicensingEmailAddress)
		{
			
			pLicensingEmailAddress = pLicensingEmailAddress.Replace(" ", "");
			
			
			List<int> ThisProductMainIDs = new List<int>();
			List<int> ThisProductSecondaryIDs = new List<int>();
					// Product IDs for Main Indicator
			
			ThisProductMainIDs.Add(8571);
			ThisProductMainIDs.Add(503567);  // support and resistance suite
			
			// Product IDs for Secondary Features
			
//			ThisProductSecondaryIDs.Add(19318);

			
			string pContactEmail = "'license@affordableindicators.com'";
			
			
//			pLicensingEmailAddress = "chrisgauci@gmail.com";
//			machineid = "9ED08010BF576B7A7AD3150B4A0A585B";
					
					
			//Print("Check License Now Indicator");
			
			
			string url = "";
			string result = "";
			//string 	machineid = "";
			string 	instrument = "";
			string 	symbol = "";
			string 	module = "";
			string 	location = "";
			string 	filename = "";
			string 	contact = "";
			string 	contact2 = "";
			string 	message = "";
			string s1 = "";
			string	s2 = "";
			string	s3 = "";
			string	s4 = "";
			string	s5 = "";
			string	s6 = "";
			string	s7 = "";
			string	s8 = "";
			string	s9 = "";
			string	s10 = "";
			string thisuniqueid = "";
			int daysremaining = 0;
			int	warningdays = 0;
				
			// Creates a StreamReader object
			System.IO.StreamReader sr;
			bool CheckLicense = true;
			bool Permission = false;
			
			DateTime StartDate = DateTime.MinValue;
			DateTime ExpireDate = DateTime.MinValue;
			DateTime CurrentDate = DateTime.MinValue;
			
		
			CurrentDate = DateTime.Now;
			
			Random random = new Random();
					
			string chartssss = BarsArray[0].BarsPeriod.ToString();
			
			//chartssss = "RJay's  RenkoSpectrum 3.0 Bars  100 - 20";
			
			if (chartssss.Contains("tdRenkoBar"))
				chartssss = chartssss + " " + BarsArray[0].BarsPeriod.Value;
			
			chartssss = chartssss.Replace("'", "XXXXX");
			chartssss = chartssss.Replace(".", "YYYYY");	
			chartssss = chartssss.Replace("/", "ZZZZZ");
			chartssss = chartssss.Replace("|", "WWWWW");

			location = "https://affordableindicators.com/";	
			filename = "ninjatrader.php";
			url = location + filename;
			url = url + "?user=" + pLicensingEmailAddress.ToString() + "&id=" + machineid;
			url = url + "&ind=" + ThisName;
			url = url + "&inst=" + Instrument.MasterInstrument.Name;
			url = url + "&chart=" + chartssss;
			url = url + "&random=" + random.Next().ToString();

			url = url.Replace(" ", "_");
			

			try
			{
				try { System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12; } catch { }
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
				request.Timeout = 15000;
				request.ReadWriteTimeout = 15000;
				request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36";
				using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
				using (System.IO.StreamReader reader = new System.IO.StreamReader(response.GetResponseStream()))
				{
					result = reader.ReadToEnd();
				}
			}
			catch (Exception ex)
			{
				string cause = "Check your Internet connection, then right-click the chart and select Reload NinjaScript.";
				WebException wex = ex as WebException;
				if (wex != null)
				{
					switch (wex.Status)
					{
						case WebExceptionStatus.NameResolutionFailure: cause = "DNS lookup failed — your computer cannot resolve www.affordableindicators.com. Try switching to a public DNS like Google (8.8.8.8) or Cloudflare (1.1.1.1), or contact your network administrator. Then right-click the chart and select Reload NinjaScript."; break;
						case WebExceptionStatus.ConnectFailure:         cause = "Outbound port 443 appears to be blocked. If you use a VPS, firewall, or corporate network, allow outbound HTTPS to www.affordableindicators.com on port 443. Then right-click the chart and select Reload NinjaScript."; break;
						case WebExceptionStatus.SecureChannelFailure:   cause = "TLS 1.2 handshake failed. Your Windows installation may need updates (run Windows Update), or a proxy or antivirus is interfering with HTTPS traffic. Then right-click the chart and select Reload NinjaScript."; break;
						case WebExceptionStatus.TrustFailure:           cause = "The server certificate could not be verified. Check that your system clock is set correctly, or disable any security software that inspects HTTPS traffic. Then right-click the chart and select Reload NinjaScript."; break;
						case WebExceptionStatus.Timeout:                cause = "The server took too long to respond. Check your Internet connection, then right-click the chart and select Reload NinjaScript."; break;
						case WebExceptionStatus.ProtocolError:          cause = "The server returned an HTTP error. Please contact Affordable Indicators support — this is a server-side issue."; break;
						default:                                        cause = "Error: " + wex.Status + ". Check your Internet connection, then right-click the chart and select Reload NinjaScript."; break;
					}
				}
				LicensingMessage = "Unable to connect to the Affordable Indicators licensing server. " + cause;
				AddError(LicensingMessage);
				return false;
			}
	
				
	// split the string into lines
				string[] lines = result.Split(new[] { "\r\n" }, StringSplitOptions.None);

					// Sets the file the StreamReader will read from

			
			string machineidstring = string.Empty;
			string notestring = string.Empty;
			
				//Print(lines.Length);
			
				string line = "";
				int lineCounter = 0;
				// Read lines and calculate the current day's OHLC from the file. While loop will go through the whole file.
				while (lineCounter < lines.Length - 0) 
				{
					
					
					line = lines[lineCounter];
					
					//Print(line);
					
					if (line.Contains("Connection Failed:"))
					{
					
						LicensingMessage = "NinjaTrader is unable to connect to Affordable Indicators, Inc. licensing database.  Right click on the Chart window, and select Reload NinjaScript.";
						
					}
					else if (line == "DISABLED")
					{
						LicensingMessage = "Your Affordable Indicators, Inc. account has been suspended. Contact " + pContactEmail + " to resolve the issue. Thanks!";
						
					}
					else if (line == "VIP")
					{
						Permission = true;
					}					
					
					else
					{
						
						
						// Splits the line at every white space. Places each split into the string array
						string [] split = line.Split(',');
			
						//split.rever
						Array.Reverse(split);
						
						// Creates a counter that will mark which string from the split array we are dealing with
						int splitCounter = 0;
						
					
						int ordernumber = 0;
						
						foreach (string s in split)
						{

				
							splitCounter++;
							switch(splitCounter)       
							{         
						                
								                
									default:   
									
									
									if (s.Contains("NOTE"))
									{
										notestring = s.Replace("NOTE", "");
										
										
									}
									else if (s.Contains("-"))
									{
										string [] split2 = s.Split('-');
										
						
										int productid = Convert.ToInt32(split2[0]);
										int numbermachineidsok = Convert.ToInt32(split2[1]);
										
										
										string allinstruments = "";
					
										
										if (split2.Length > 2) // contains info with instrument restriction
										{
											allinstruments = split2[2];
											//Print(allinstruments);
											
											string [] split3 = allinstruments.Split('|');
											
											foreach (string sss in split3)
											{
												
												// sss is the instrument 
												
												ProductIDToInstruments.TryAdd(productid, new List<string>());
												ProductIDToInstruments[productid].Add(sss);
												
												
												//Print(sss);
											}
											
										}
										
										
										if (ProductIDToMachineIDs.ContainsKey(productid))
											ProductIDToMachineIDs[productid] = ProductIDToMachineIDs[productid] + numbermachineidsok;
										else
											ProductIDToMachineIDs.Add(productid, numbermachineidsok);
										
									}
									else
									{
										machineidstring = s;	
									}
									

									// add all licensed products to a list
									//Print(s);
									
									break;      
							}

						
							// Resets the split counter for the next line read from the file
							if (splitCounter == 10)
								splitCounter = 0;
						}

				
					}
					
					lineCounter = lineCounter + 1;
					
				}
				
				
				//Print(LicensingMessage);
				if (!Permission)
				if (LicensingMessage == "")
				{
				
					//Print("machineidstring: " + machineidstring);
					
					int machineidnumber = 0;
					
					if (machineidstring == "NOUSER")
					{
						
//						LicensingMessage = "Please enter the 'Email Address' connected to your Affordable Indicators, Inc. account in the 'License' section at the bottom of the indicator settings.";
						
						
						if (pLicensingEmailAddress == "")
						{
							LicensingMessage = "Please enter the 'Email Address' connected to your Affordable Indicators, Inc. account in the 'License' section at the bottom of the indicator settings.";
						}
						else if (!pLicensingEmailAddress.Contains(@"@"))
						{
							LicensingMessage = "Please enter the 'Email Address' connected to your Affordable Indicators, Inc. account in the 'License' section at the bottom of the indicator settings.";
						}
						else
						{
							LicensingMessage = "Your 'Email Address' was not found in the Affordable Indicators, Inc. database.  Please enter the 'Email Address' connected to your Affordable Indicators, Inc. account in the 'License' section at the bottom of the indicator settings.  Contact " + pContactEmail + " if you need further assistance.";
						}
						
					}
					else if (machineidstring == "ID0")
					{
						// no machine ids connected to account
						
						LicensingMessage = "Please login to your Affordable Indicators, Inc. account, go to the Members Area Dashboard, and enter your NinjaTrader Machine ID(s).  Contact " + pContactEmail + " if you need further assistance.";
					
						
					}
					else if (machineidstring == "IDX")
					{
						
						// no machine ids match
						
						LicensingMessage = "Your NinjaTrader Machine ID has changed.  Please login to your Affordable Indicators, Inc. account, go to the Members Area Dashboard, and update your NinjaTrader Machine ID.  Contact " + pContactEmail + " if you need further assistance.";
						
						
					}
					else
					{
						machineidnumber = Convert.ToInt32(machineidstring.Replace("ID", ""));
							//Print(machineidnumber);
					}
					
					
					Permission = false;
					//EnableOrderExecution = false;			
									
					
					int maxids = 0;
					
					 foreach (KeyValuePair<int, int> oneproduct in ProductIDToMachineIDs)
           			 {		
				
						 int productid = oneproduct.Key;
						int numbermachineidsok = oneproduct.Value;
						
						 //Print(productid + "-" + numbermachineidsok);
						 
						if (ThisProductMainIDs.Contains(productid))
						//if (productid == ThisProductMainID)
						{
							
							maxids = Math.Max(maxids, numbermachineidsok);
							

							// check for instrument restrictions for this particular product
							
							if (Permission)
							if (ProductIDToInstruments.ContainsKey(productid))
							{
								//Print("This one has restrictions");
	
								string thischarti = Instrument.MasterInstrument.Name;
								
								if (thischarti != "YM" && thischarti != "MYM" )
									thischarti = Instrument.MasterInstrument.Name.Replace("M", "");
								
								if (thischarti == "MYM")
									thischarti = "YM";								
								
								if (thischarti == "2K")
									thischarti = "RTY";
								
								string thischartfulli = Instrument.FullName;							
								string allinst = string.Empty;
								
								foreach (string sssss in ProductIDToInstruments[productid].OrderBy(d => d))
								{
									if (allinst == string.Empty)
										allinst = sssss;
									else
										allinst = allinst + ", " + sssss;
									
								}
								
								if (ProductIDToInstruments[productid].Contains(thischarti)) // an instrument in the list matches the chart instrument
								{
		
								}
								else
								{
									LicensingMessage = "This product is only licensed for the following instruments: " + allinst + ".  Contact " + pContactEmail + " if you need further assistance." ;
									Permission = false;
								}
						
							}
							
							
						}
						
						
//						Enabling extra features
						
						
						//if (ThisProductSecondaryIDs.Contains(productid))
						//if (productid == ThisProductXID)
						{
							
							//EnableOrderExecution = true;
							//pEXYES = true;
							
						}
						
						
					 }
					 
					  
					 if (maxids != 0)
			 		if (machineidnumber > maxids)
					{
						
						
						if (maxids == 1)
							LicensingMessage = "This product is only licensed for 1 NinjaTrader Machine ID.  Please login to your Affordable Indicators, Inc. account, go to the Members Area Dashboard, and update your NinjaTrader Machine ID(s) accordingly." ;
						else
							LicensingMessage = "This product is only licensed for 1-" + maxids.ToString() + " NinjaTrader Machine IDs.  Please login to your Affordable Indicators, Inc. account, go to the Members Area Dashboard, and update your NinjaTrader Machine ID(s) accordingly." ;
							
					}
					else
					{
						Permission = true;
					}
						
							
					 if (!Permission && LicensingMessage == "")
					 {
							LicensingMessage = "You haven't purchased this product.  Contact " + pContactEmail + " if you need further assistance.";
							
						 //EnableOrderExecution = false;
						 
						 
					 }
					 					 
					
				}
				
				//Print(message);
					if (LicensingMessage != "")
					{
						//LicensingMessage = message;
						AddError(LicensingMessage);
						
						//MessageBox.Show(message, "Licensing for '" + module2 + "'");
					}

					
					if (!Permission)
					{
						return false;
					}
					else
					{

						return true;
					}
					
				
			}

					
	private Point MP;
        private SharpDX.RectangleF B2 = new SharpDX.RectangleF(0, 0, 0, 0);
        private bool InMenu;
        private bool InMenuP;
        private bool ButtonOff = false;
		private double CurrentMousePrice = 0;		
        private int space = 5;
        SortedDictionary<double, ButtonZ> AllButtonZ = new SortedDictionary<double, ButtonZ>();	
		SortedDictionary<double, ButtonZ> AllButtonZ2 = new SortedDictionary<double, ButtonZ>();
	
        public class ButtonZ
        {
            string iText;
            string iName;
            int iWidth;
            bool iSwitch;
            SharpDX.RectangleF iRect;
            bool iHovered;

            public string Text { get { return iText; } set { iText = value; } }
            public string Name { get { return iName; } set { iName = value; } }
            public int Width { get { return iWidth; } set { iWidth = value; } }
            public bool Switch { get { return iSwitch; } set { iSwitch = value; } }
            public SharpDX.RectangleF Rect { get { return iRect; } set { iRect = value; } }
            public bool Hovered { get { return iHovered; } set { iHovered = value; } }

        }
		
		private bool MouseInRectNow, MouseInRectPre = false;
			
		private SharpDX.DirectWrite.TextLayout textLayout2 = null;
		

		private double dpiX = 0;
		private	double dpiY = 0;

		// DPI scaling helpers — at 100% returns input unchanged, at 150% returns input * 1.5.
		// dpiX is stored as percentage * 100 (so 125%-DPI = 125), so divide by 100 to get scale.
		private float S(float v) { return v * (float)(dpiX > 0 ? dpiX / 100.0 : 1.0); }
		private int S(int v) { return (int)(v * (dpiX > 0 ? dpiX / 100.0 : 1.0)); }
		private float S(double v) { return (float)(v * (dpiX > 0 ? dpiX / 100.0 : 1.0)); }


		private double FinalXPixel = 0;
		private	double FinalYPixel = 0;
			
        private void AddButtonZ(string iText, string iName, int iWidth, bool iSwitch)
        {
            ButtonZ Z = new ButtonZ();
            Z.Text = iText;
            Z.Name = iName;
            Z.Width = iWidth;
            Z.Switch = iSwitch;
            Z.Rect = new SharpDX.RectangleF(0, 0, 0, 0);
            Z.Hovered = false;

            AllButtonZ.Add(AllButtonZ.Count + 1, Z);

        }
					
        private void AddButtonZ2(string iText, string iName, int iWidth, bool iSwitch, SharpDX.RectangleF iRect)
        {
            ButtonZ Z = new ButtonZ();
            Z.Text = iText;
            Z.Name = iName;
            Z.Width = iWidth;
            Z.Switch = iSwitch;
            Z.Rect = iRect;
            Z.Hovered = false;

            AllButtonZ2.Add(AllButtonZ2.Count + 1, Z);

        }	
		
        private bool MouseIn(SharpDX.RectangleF RR, int XF, int YF)
        {
            //Print(RR.Left);
            
			if (FinalXPixel != 0)
            if (FinalXPixel >= RR.Left - XF && FinalXPixel <= RR.Right + XF && FinalYPixel >= RR.Top - YF && FinalYPixel <= RR.Bottom + YF)
                return true;
           
                return false;

        }
		
		
		List<SharpDX.RectangleF> AllRefreshRects = new List<SharpDX.RectangleF>();
		
				
		SharpDX.RectangleF rectangleF2 = new SharpDX.RectangleF(0,0,0,0);
		
		      
        // IsInSession removed — never written
		
		private DateTime StartTime, EndTime;
		private DateTime StartTime2, EndTime2;
		private DateTime StartTime3, EndTime3;				

		private DateTime CalcTime1, CalcTime2;	
		
		private bool FirstRun = true;
		
		
		// Unused Series and indicator fields removed for faster load
		// (BarHigh, BarLow, BodyHigh, BodyLow, Direction, Sequence, SignalReady,
		//  ThisTrend1-3, LastSignal, CurrentHigh/Low, iEMA1/2, iATR1/2 — all commented-out code)
		

			private DateTime StartTime4 = DateTime.MinValue;
			private DateTime StartTime5 = DateTime.MinValue;
			private DateTime StartTime6 = DateTime.MinValue;
			private DateTime StartTime7 = DateTime.MinValue;
			private DateTime StartTime8 = DateTime.MinValue;
			private DateTime StartTime9 = DateTime.MinValue;
	
			private DateTime EndTime4 = DateTime.MinValue;
			private DateTime EndTime5 = DateTime.MinValue;
			private DateTime EndTime6 = DateTime.MinValue;
			private DateTime EndTime7 = DateTime.MinValue;
			private DateTime EndTime8 = DateTime.MinValue;
			private DateTime EndTime9 = DateTime.MinValue;
			private DateTime EndTime10 = DateTime.MinValue;
			private DateTime PriorDTnow = DateTime.MinValue;
			private DateTime InfoPrintedAt = DateTime.MinValue;
			//private DateTime pStartColorizingAt = new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day,0,1,0);
			private bool RunFirst = true;
			private bool ParameterInputError = false;
		
		
			private DateTime dateBegin = DateTime.Today;
		
			private bool betaTest = false;
			
			//private Font textFont = new Font("Arial", 8);
			private float yOffset = 15; // Gap up from the bottom of the Panel
			
		
			private List<DateTime> pivotLowsTimes = new List<DateTime>();
			private List<DateTime> pivotHighsTimes = new List<DateTime>();
	
		
		private struct OneLevel {   //LIST
			
			public double ThisPrice;
			public string ThisName;
			public string ThisData;
			public DateTime ThisTime;
		
			
			public OneLevel (double thisPrice, string thisName, string thisData, DateTime thisTime) 
			
			{this.ThisPrice = thisPrice; this.ThisName = thisName; this.ThisData = thisData; this.ThisTime = thisTime;}
		}
	
	
		private struct OneLevelStats {   //LIST
			
			public double ThisPrice;
			public string ThisName;
			public string ThisData;
			public DateTime ThisTime;
			public int TotalHits;
			
			public OneLevelStats (double thisPrice, string thisName, string thisData, DateTime thisTime, int totalHits) 
			
			{this.ThisPrice = thisPrice; this.ThisName = thisName; this.ThisData = thisData; this.ThisTime = thisTime; this.TotalHits = totalHits;}
		}
		
		
		private List<OneLevel> AllHits = new List<OneLevel>();
		
		private List<OneLevel> AllHitsChart = new List<OneLevel>();
		
		private SortedList<string, OneLevelStats> AllHitsByData = new SortedList<string, OneLevelStats>();
		

			private List<int> zoneStart;
			private List<int> zoneEnd;

			private List<int> chartdates;
			private List<int> startbar;
			private List<int> endbar;
		
		
		private struct ZoneDetails {   //LIST
			public double Balance;
			public int HighCount;
			public int LowCount;
			public double FHigh;
			public double FLow;
			public DateTime LatestHit;
			public List<OneLevel> ThisAllHits; 
	
			 
			public ZoneDetails(double balance, int highCount, int lowCount, double fHigh, double fLow, DateTime latestHit, List<OneLevel> thisAllHits) 
			
			{this.Balance = balance; this.HighCount = highCount; this.LowCount = lowCount; this.FHigh = fHigh; this.FLow = fLow; this.LatestHit = latestHit; this.ThisAllHits = thisAllHits; }
		}
		
			private List<ZoneDetails> AllZones = new List<ZoneDetails>();   //LIST
		 
		
//			private double	currentSwingLow = 0;	
//			private double	currentSwingHigh = 0;
			private int		countLow = 0;	
			private int		countHigh = 0;
			private int		countLowPivots = 0;
			private int		countHighPivots = 0;
			private double 	balance = 0;
		
			private bool	timeReady = false;
		    private bool calczones = false;
		    private bool deferredZonesCalculated = false;
		    private bool needsDeferredZoneCalc = false;
		
		
		private DateTime LaunchedAt = DateTime.MinValue;
        
		private struct TickLevel {   //LIST
			public double Price;
			public int HighCount;
			public int LowCount;
			public double FHigh;
			public double FLow;
			public DateTime LatestHit;
			public List<OneLevel> ThisAllHits; 
			
			public TickLevel(double price, int highCount, int lowCount, double fHigh, double fLow, DateTime latestHit, List<OneLevel> thisAllHits) 
			
			{this.Price = price; this.HighCount = highCount; this.LowCount = lowCount; this.FHigh = fHigh; this.FLow = fLow; this.LatestHit = latestHit; this.ThisAllHits = thisAllHits;}
		}
		
		
		private enum ZoneState { Untested, Tested, Broken }

		private struct Zone {
			public double High;
			public double Low;
			public int StartBar;
			public int EndBar;
			public int TotalPivots;
			public double Balance;
			public DateTime LatestHit;
			public OneLevel LatestOneLevel;
			public List<OneLevel> ThisAllHits;
			public string AllDetails;
			public double Strength;
			public ZoneState State;
			public int TestCount;

			public Zone(double high, double low, int startBar, int endBar, int totalPivots, double balance, DateTime latestHit, OneLevel latestOneLevel, List<OneLevel> thisAllHits, string allDetails, double strength, ZoneState state, int testCount)
			{this.High = high; this.Low = low; this.StartBar = startBar; this.EndBar = endBar; this.TotalPivots = totalPivots; this.Balance = balance; this.LatestHit = latestHit; this.LatestOneLevel = latestOneLevel; this.ThisAllHits = thisAllHits; this.AllDetails = allDetails; this.Strength = strength; this.State = state; this.TestCount = testCount;}
		}
		

		private List<TickLevel> ticklevels;
		private List<Zone> zones;
		private int totalEnabledTimeframes = 1;

		
		// AlreadyDrawnLines removed — was populated but never read
		
		
			Point startPoint	= new Point(0,0);
			Point endPoint		= new Point(0,0);			
		
				
		private int			constant;
		private double		currentSwingHigh;
		private double		currentSwingLow;
		private double		lastSwingHighValue;
		private double		lastSwingLowValue;
		private int			saveCurrentBar;

		private Series<double> swingHighSeries;
		private Series<double> swingHighSwings;
		private Series<double> swingLowSeries;
		private Series<double> swingLowSwings;
		
		// SwingHigh2, SwingLow2 removed — never written
					
		
		private Swing iThisSwing;
		
		private ZigZag iZZ;
		
		private int pZoneWidth = 0;
		
		
		private string Descccc = @"Price Action Confluence Indicator by Joel Ryan Wyse.";
		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{

				Name = ThisName;
				Description = Descccc;
				
				
				IsSuspendedWhileInactive	= true;
				//Period						= 21;
				
				Calculate				= Calculate.OnBarClose;
				DisplayInDataBox		= false;
				DrawOnPricePanel		= false;
				
				IsAutoScale				= false;
				
				IsOverlay				= true;
				PaintPriceMarkers		= true;
				ScaleJustification		= ScaleJustification.Right;
				
				
				ArePlotsConfigurable = false;
	            AreLinesConfigurable = false;				
				
				
//				AddPlot(new Stroke(Brushes.Transparent,	2), PlotStyle.Dot, NinjaTrader.Custom.Resource.SwingHigh);
//				AddPlot(new Stroke(Brushes.Transparent,	2), PlotStyle.Dot, NinjaTrader.Custom.Resource.SwingLow);
				
//				AddPlot(new Stroke(Brushes.Transparent, DashStyleHelper.Solid, 1),	PlotStyle.Line, "Signals");
				
				string ThisPName = "Levels ";
				
					for (int i = 0; i<=99; i++)
					{
						AddPlot(new Stroke(Brushes.Black, DashStyleHelper.Solid, 1),	PlotStyle.Line, ThisPName + i.ToString());
					}
									
				
			}
			else if (State == State.Configure)
			{
	
				
				Description = Descccc;
				
				
//				Permission = CheckLicense(NinjaTrader.Cbi.License.MachineId, "");
			

//				if (!Permission)
//					return;
							
			
				int FinalBarsToLoad = 0;
				
				
				if (pUseSData1)
				{
					BarsPeriod BP = new BarsPeriod();					
					BP.BarsPeriodType = AcceptableBasePeriodType1;
					BP.Value = pThisBarPeriod1;
					
					FinalBarsToLoad = (int) ((pNumberOfDaysToLoadMinute1 * 1440 / pThisBarPeriod1) / 7 * 5);				
					AddDataSeries(null, BP, FinalBarsToLoad, null, true);
					
					// "CME US Index Futures RTH"
					
					// TradingHours.Name doesn't work for null
					
					//AddDataSeries(AcceptableBasePeriodType1, pThisBarPeriod1);
					//AddDataSeries(null, BP, null);
				}
				
				
				if (pUseSData2)
				{
					BarsPeriod BP = new BarsPeriod();					
					BP.BarsPeriodType = AcceptableBasePeriodType2;
					BP.Value = pThisBarPeriod2;
					
					FinalBarsToLoad = (int) ((pNumberOfDaysToLoadMinute2 * 1440 / pThisBarPeriod2) / 7 * 5);				
					AddDataSeries(null, BP, FinalBarsToLoad, null, true);
					
					
					//AddDataSeries(AcceptableBasePeriodType2, pThisBarPeriod2);
				
				}
								
		
				if (pUseSData3)
				{
					BarsPeriod BP = new BarsPeriod();					
					BP.BarsPeriodType = AcceptableBasePeriodType3;
					BP.Value = pThisBarPeriod3;
					
					FinalBarsToLoad = (int) ((pNumberOfDaysToLoadMinute3 * 1440 / pThisBarPeriod3) / 7 * 5);				
					AddDataSeries(null, BP, FinalBarsToLoad, null, true);
					
					
					//AddDataSeries(AcceptableBasePeriodType3, pThisBarPeriod3);
				
					
				}
								
								
				if (pUseDailyData)
				{
					BarsPeriod BP = new BarsPeriod();
					
					BP.BarsPeriodType = BarsPeriodType.Day;
					BP.Value = 1;
					
					
					AddDataSeries(null, BP, pNumberOfMonthsToLoadDaily*30, null, false);
					
				}
		
				
				if (pUseWeeklyData)
				{
					BarsPeriod BP = new BarsPeriod();
					
					BP.BarsPeriodType = BarsPeriodType.Week;
					BP.Value = 1;
					
					int weekstoload = (int) (pNumberOfMonthsToLoadWeekly*4.4);
					
					//Print(weekstoload);
					
					AddDataSeries(null, BP, weekstoload, null, false);
					
				}		
				
				
				//AddDataSeries(Base, pThisBarPeriod1);
				
			}
			else if (State == State.DataLoaded)
			{
				
				
				// force this for existing chart windows
				Calculate				= Calculate.OnBarClose;
				
				
				if (Name != ThisName && Name != string.Empty)
					Name = ThisName;					
				
				
				pZoneWidth = pZoneWidth2 + 1;
				
				
			// checking for existing email tied to this computer
							
							
							string pFileLocation = NinjaTrader.Core.Globals.UserDataDir;
							
							if(!Directory.Exists(pFileLocation))
								Directory.CreateDirectory(pFileLocation);
							
							
							//look for file
								
	
							string pFileName = "AI";
						string MainFileName = pFileName + ".txt";
							
		
							string location2 = pFileLocation + MainFileName;	
							string final2 = pLicensingEmailAddress;
		
					string emailfound = string.Empty;
							
					string[] readText = null;
			        try 
			        {
						readText = File.ReadAllLines(location2);
						emailfound = readText[0];						
			        } 
			        catch (Exception e) 
			        {
			        }
					
					if (!pLicensingEmailAddress.Contains("@"))
					{
						pLicensingEmailAddress = "";
					}
					
					if (pLicensingEmailAddress == string.Empty)
						pLicensingEmailAddress = emailfound;
					
		
					if (pLicensingEmailAddress != string.Empty)
					if (pLicensingEmailAddress != emailfound || !File.Exists(location2))
					{			
						System.IO.File.WriteAllText(location2,final2);
					}
							
					
				Permission = LicenseWordPress(NinjaTrader.Cbi.License.MachineId, pLicensingEmailAddress);	
				
				
				//iThisSwing = Swing(pStrength);

				DeviationType dddd = DeviationType.Points;

				//iZZ = ZigZag(dddd,6,true);
				
				
				// Unused Series/indicator initializations removed for faster load
				
							
				LaunchedAt = DateTime.Now; // get the time when the indicator is launched
			
	
				ticklevels = new List<TickLevel>();
				zones = new List<Zone>();

				zoneStart = new List<int>();
				zoneEnd = new List<int>();

				chartdates = new List<int>();
				startbar = new List<int>();
				endbar = new List<int>();
				
				
				if (ChartControl != null)
				if (ChartPanel != null)
                {
					ChartPanel.MouseMove += new MouseEventHandler(OnMouseMove);
					ChartPanel.PreviewMouseMove += new MouseEventHandler(OnPanelPreviewMouseMoveSuppress);
                    ChartPanel.MouseDown += new MouseButtonEventHandler(OnMouseDown);
					ChartPanel.PreviewMouseDown += new MouseButtonEventHandler(OnPanelPreviewLeftDown);
					ChartPanel.MouseUp += new MouseButtonEventHandler(OnPanelMouseUp);
					ChartPanel.MouseLeave += new MouseEventHandler(OnMouseLeave);
					ChartPanel.MouseWheel += new MouseWheelEventHandler(OnMouseWheel);
                }
				
				
			}
			
						
			if (State == State.Configure)
			{
				currentSwingHigh	= 0;
				currentSwingLow		= 0;
				lastSwingHighValue	= 0;
				lastSwingLowValue	= 0;
				saveCurrentBar		= -1;
				//constant			= (2 * Strength) + 1;
			}
			else if (State == State.DataLoaded)
			{
				swingHighSeries = new Series<double>(this);
				swingHighSwings = new Series<double>(this);
				swingLowSeries	= new Series<double>(this);
				swingLowSwings	= new Series<double>(this);
				
	
				// SwingHigh2, SwingLow2, IsInSession removed — never written, Infinite lookback was expensive
				
				
				AddButtonZ("Zones", "Zones", 40, pShowAll);
				AddButtonZ("Labels", "Labels", 40, pLabelsEnabled);

				if (pLatestDataAllEnabled)
					AddButtonZ("Latest Pivots", "Latest Pivots", 40, pShowID);
				else
					pShowID = pLatestDataAllEnabled;

				if (pExportEnabled)
					AddButtonZ("Export Zones", "Export Zones", 40, false);

				RegisterPanel();

			}
			else if (State == State.Realtime)
			{
				if (!deferredZonesCalculated)
				{
					deferredZonesCalculated = true;
					calczones = true;
					try
					{
						CalculateZones();
						ExportZonesToCsv();
					}
					catch { }
				}
			}
			else if( State == State.Terminated)
			{
				UnregisterPanel();

                if (ChartPanel != null)
                {
                    ChartPanel.MouseMove -= new MouseEventHandler(OnMouseMove);
					ChartPanel.PreviewMouseMove -= new MouseEventHandler(OnPanelPreviewMouseMoveSuppress);
                    ChartPanel.MouseDown -= new MouseButtonEventHandler(OnMouseDown);
					ChartPanel.PreviewMouseDown -= new MouseButtonEventHandler(OnPanelPreviewLeftDown);
					ChartPanel.MouseUp -= new MouseButtonEventHandler(OnPanelMouseUp);
					ChartPanel.MouseLeave -= new MouseEventHandler(OnMouseLeave);
					ChartPanel.MouseWheel -= new MouseWheelEventHandler(OnMouseWheel);
                }

				// Dispose all cached DirectX/DirectWrite resources — these are unmanaged
				// and leak on every chart close, workspace switch, or script reload if
				// not disposed here. Accumulated leaks cause platform-wide slowdown.
				if (cachedZoneColor1DX != null) { cachedZoneColor1DX.Dispose(); cachedZoneColor1DX = null; }
				if (cachedZoneColor2DX != null) { cachedZoneColor2DX.Dispose(); cachedZoneColor2DX = null; }
				if (cachedZoneColor3DX != null) { cachedZoneColor3DX.Dispose(); cachedZoneColor3DX = null; }
				if (cachedZoneColor4DX != null) { cachedZoneColor4DX.Dispose(); cachedZoneColor4DX = null; }
				if (cachedZoneColor5DX != null) { cachedZoneColor5DX.Dispose(); cachedZoneColor5DX = null; }
				if (cachedChartTextBrushDX != null) { cachedChartTextBrushDX.Dispose(); cachedChartTextBrushDX = null; }
				if (cachedChartBackgroundBrushDX != null) { cachedChartBackgroundBrushDX.Dispose(); cachedChartBackgroundBrushDX = null; }
				if (cachedLabelBrushDX != null) { cachedLabelBrushDX.Dispose(); cachedLabelBrushDX = null; }
				if (cachedButtonBrushDX != null) { cachedButtonBrushDX.Dispose(); cachedButtonBrushDX = null; }
				if (cachedButtonHBrushDX != null) { cachedButtonHBrushDX.Dispose(); cachedButtonHBrushDX = null; }
				if (cachedButtonFHBrushDX != null) { cachedButtonFHBrushDX.Dispose(); cachedButtonFHBrushDX = null; }
				if (cachedButtonFOFFBrushDX != null) { cachedButtonFOFFBrushDX.Dispose(); cachedButtonFOFFBrushDX = null; }
				if (cachedButtonFONBrushDX != null) { cachedButtonFONBrushDX.Dispose(); cachedButtonFONBrushDX = null; }
				if (cachedButtonFH2BrushDX != null) { cachedButtonFH2BrushDX.Dispose(); cachedButtonFH2BrushDX = null; }
				if (cachedAreaBrushDX != null) { cachedAreaBrushDX.Dispose(); cachedAreaBrushDX = null; }
				if (cachedPanelBackdropBrushDX != null) { cachedPanelBackdropBrushDX.Dispose(); cachedPanelBackdropBrushDX = null; }
				if (cachedHeaderBgBrushDX != null) { cachedHeaderBgBrushDX.Dispose(); cachedHeaderBgBrushDX = null; }
				if (cachedHeaderBgHoverBrushDX != null) { cachedHeaderBgHoverBrushDX.Dispose(); cachedHeaderBgHoverBrushDX = null; }
				if (cachedHeaderTextBrushDX != null) { cachedHeaderTextBrushDX.Dispose(); cachedHeaderTextBrushDX = null; }
				if (cachedHoverGlowBrushDX != null) { cachedHoverGlowBrushDX.Dispose(); cachedHoverGlowBrushDX = null; }
				if (cachedErrorBrushDX != null) { cachedErrorBrushDX.Dispose(); cachedErrorBrushDX = null; }

				if (cachedLabelTextFormat != null) { cachedLabelTextFormat.Dispose(); cachedLabelTextFormat = null; }
				if (cachedButtonTextFormat != null) { cachedButtonTextFormat.Dispose(); cachedButtonTextFormat = null; }
				if (cachedHeaderTextFormat != null) { cachedHeaderTextFormat.Dispose(); cachedHeaderTextFormat = null; }
				if (cachedErrorTextFormat != null) { cachedErrorTextFormat.Dispose(); cachedErrorTextFormat = null; }

				if (cachedStrokeUntested != null) { cachedStrokeUntested.Dispose(); cachedStrokeUntested = null; }
				if (cachedStrokeTested != null) { cachedStrokeTested.Dispose(); cachedStrokeTested = null; }
				if (cachedStrokeBroken != null) { cachedStrokeBroken.Dispose(); cachedStrokeBroken = null; }

				if (reusableTextLayout != null) { reusableTextLayout.Dispose(); reusableTextLayout = null; }

				if (textMeasureCache != null) textMeasureCache.Clear();
			}
			
			
		}


        internal void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.MP = e.GetPosition(this.ChartPanel);


			FinalXPixel = MP.X / 100 * dpiX;
			FinalYPixel = MP.Y / 100 * dpiY;

			// Panel header click
			if (MouseIn(headerRect, 3, 3))
			{
				draggingPanelId = PANEL_ID;
				dragStartY = (float)FinalYPixel;
				dragOffsetY = 0;
				if (ChartPanel != null) ChartPanel.CaptureMouse();
				e.Handled = true;
				return;
			}

			// Panel button clicks
			if (IsPanelExpanded())
			{
				foreach (KeyValuePair<double, ButtonZ> thisbutton in AllButtonZ)
				{
					if (thisbutton.Value.Text == "") continue;
					if (thisbutton.Value.Rect.Bottom < PANEL_TOP_MARGIN || thisbutton.Value.Rect.Top > ChartPanel.H) continue;
					if (!MouseIn(thisbutton.Value.Rect, 2, 2)) continue;

					string buttonn = thisbutton.Value.Text;

					if (buttonn == "Zones")
					{
						pShowAll = !pShowAll;
						thisbutton.Value.Switch = pShowAll;
					}
					else if (buttonn == "Labels")
					{
						pLabelsEnabled = !pLabelsEnabled;
						thisbutton.Value.Switch = pLabelsEnabled;
					}
					else if (buttonn == "Latest Pivots")
					{
						pShowID = !pShowID;
						thisbutton.Value.Switch = pShowID;
					}
					else if (buttonn == "Export Zones")
					{
						ExportZonesToCsv(true);
					}
					else if (buttonn == "Calculate Zones")
					{
						CalculateZones();
						ExportZonesToCsv();
					}

					ChartControl.InvalidateVisual();
					e.Handled = true;
					return;
				}
			}


			if (AllErrorMessages.Count > 0)
			{
				AllErrorMessages.Clear();
				ChartControl.InvalidateVisual();

//				myProperties.AllowSelectionDragging = PreviousDrag;

				return;

			}

			
//			}

			// top chart buttons
			
			
//               if (hoverednew && buttonn == SFeed1)
//                {
					
//					//Print(buttonn);
					
//					pFeed1Included = !pFeed1Included;
//					OneDone = true;
					
					
//				}
				
//               if (hoverednew && buttonn == SFeed2)
//                {
					
					
//				}				
				
//               if (hoverednew && buttonn == SFeed3)
//                {
					
					
//				}	
				
//               if (hoverednew && buttonn == SFeed4)
//                {
					
//					//Print(buttonn);
					
//					pFeed4Included = !pFeed4Included;
//					OneDone = true;
					
						
//				}
				
//			}
			
			
        }


		internal void OnMouseLeave(object sender, MouseEventArgs e)
    	{
            this.MP = e.GetPosition(this.ChartPanel);

			FinalXPixel = MP.X / 100 * dpiX;
			FinalYPixel = MP.Y / 100 * dpiY;

			InMenu = false;
			SetPanelMenuOpen(false);
			if (draggingPanelId != null) { draggingPanelId = null; dragOffsetY = 0; if (ChartPanel != null) ChartPanel.ReleaseMouseCapture(); }
			this.ChartControl.InvalidateVisual();
		}

		internal void OnMouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (!InMenu && !IsAnyPanelMenuOpen()) return;

			var scrollReg = GetScrollRegistry();
			float[] state;
			if (!scrollReg.TryGetValue(panelHash, out state)) return;
			if (state[SC_TOTALH] <= state[SC_VIEWH]) return;

			float delta = e.Delta > 0 ? -30f : 30f;
			state[SC_OFFSET] += delta;
			float maxScroll = Math.Max(0, state[SC_TOTALH] - state[SC_VIEWH] + 20f);
			state[SC_OFFSET] = Math.Max(0, Math.Min(state[SC_OFFSET], maxScroll));
			ChartControl.InvalidateVisual();
			e.Handled = true;
		}

		// Suppress chart bar pan while user is click-dragging a panel header to reorder.
		// Calls OnMouseMove first so drag tracking continues, then marks Handled so
		// NT's chart-pan logic on bubble MouseMove never sees the event.
		internal void OnPanelPreviewMouseMoveSuppress(object sender, MouseEventArgs e)
		{
			if (draggingPanelId == PANEL_ID)
			{
				try { OnMouseMove(sender, e); } catch { }
				e.Handled = true;
			}
		}

		// Panel-header drag release: toggle expand immediately on mouse-up so the
		// user doesn't have to wiggle the mouse to see the state change. Also
		// invalidates the chart so sibling panels reposition right away.
		internal void OnPanelMouseUp(object sender, MouseButtonEventArgs e)
		{
			if (draggingPanelId == PANEL_ID)
			{
				if (Math.Abs(dragOffsetY) < 5f)
					SetPanelExpanded(!panelExpanded);
				draggingPanelId = null;
				dragOffsetY = 0;
				if (ChartPanel != null) ChartPanel.ReleaseMouseCapture();
				if (ChartControl != null) ChartControl.InvalidateVisual();
			}
		}

		// Intercept press on the panel header BEFORE NT's chart-pan logic sees
		// it. Uses generic PreviewMouseDown so Handled propagates to bubble
		// MouseDown (same args) and OnMouseDown runs exactly once.
		internal void OnPanelPreviewLeftDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton != MouseButton.Left) return;
			try
			{
				if (ChartPanel == null) return;
				var mp = e.GetPosition(this.ChartPanel);
				double mx = mp.X / 100 * dpiX;
				double my = mp.Y / 100 * dpiY;
				if (mx >= headerRect.X - 3 && mx <= headerRect.X + headerRect.Width + 3
				 && my >= headerRect.Y - 3 && my <= headerRect.Y + headerRect.Height + 3)
				{
					try { OnMouseDown(sender, e); } catch { }
					e.Handled = true;
				}
			}
			catch { }
		}

		internal void OnMouseMove(object sender, MouseEventArgs e)
    	{
            this.MP = e.GetPosition(this.ChartPanel);

			FinalXPixel = MP.X / 100 * dpiX;
			FinalYPixel = MP.Y / 100 * dpiY;

			// hover rects
			MouseInRectPre = MouseInRectNow;
			MouseInRectNow = false;
			for (int rrIdx = 0; rrIdx < AllRefreshRects.Count; rrIdx++)
			{
				if (MouseIn(AllRefreshRects[rrIdx],3,3))
					MouseInRectNow = true;
			}

			if (MouseInRectNow != MouseInRectPre)
				this.ChartControl.InvalidateVisual();

			// Button hover detection
			bool needsInvalidate = false;
			foreach (KeyValuePair<double, ButtonZ> thisbutton in AllButtonZ)
			{
				if (thisbutton.Value.Rect.Bottom < PANEL_TOP_MARGIN || thisbutton.Value.Rect.Top > ChartPanel.H) continue;
				bool hoverednew = MouseIn(thisbutton.Value.Rect, 2, 2);
				if (hoverednew != thisbutton.Value.Hovered)
				{
					thisbutton.Value.Hovered = hoverednew;
					needsInvalidate = true;
				}
			}

			// Menu state with panel coordination
			InMenuP = InMenu;
			InMenu = MouseIn(B2, 8, 8);
			SetPanelMenuOpen(InMenu);

			// Drag-to-reorder
			if (draggingPanelId != null && e.LeftButton == MouseButtonState.Released)
			{
				if (Math.Abs(dragOffsetY) < 5f)
					SetPanelExpanded(!panelExpanded);
				draggingPanelId = null;
				dragOffsetY = 0;
				if (ChartPanel != null) ChartPanel.ReleaseMouseCapture();
				needsInvalidate = true;
			}
			else if (draggingPanelId != null)
			{
				dragOffsetY = (float)FinalYPixel - dragStartY;
				needsInvalidate = true;
				e.Handled = true;
				if (Math.Abs(dragOffsetY) > 10f)
				{
					var registry = GetPanelRegistry();
					ConcurrentDictionary<string, object[]> panels;
					if (registry.TryGetValue(panelHash, out panels))
					{
						float myCenterY = headerRect.Y + headerRect.Height / 2f + dragOffsetY;
						foreach (var kvp in panels)
						{
							if (kvp.Key == draggingPanelId) continue;
							var s = kvp.Value;
							if (s.Length < SL_SLOT_SIZE) continue;
							if ((bool)s[SL_BOTTOMUP]) continue;
							float otherY = (float)s[SL_HEADERY];
							float otherH = (float)s[SL_HEADERH];
							if (myCenterY > otherY && myCenterY < otherY + otherH)
							{
								SwapPanelOrder(draggingPanelId, kvp.Key);
								dragStartY = (float)FinalYPixel;
								dragOffsetY = 0;
								break;
							}
						}
					}
				}
			}

			bool anyOpen = IsAnyPanelMenuOpen();
			if (InMenu != InMenuP || anyOpen != InMenuP || needsInvalidate || MouseIn(headerRect, 3, 3))
				this.ChartControl.InvalidateVisual();
		}
		
		private bool reset3 = false;
			
		
		private int BarsInProgressMinute1 = 0;
		private int BarsInProgressMinute2 = 0;
		private int BarsInProgressMinute3 = 0;
		private int BarsInProgressDaily = 0;
		private int BarsInProgressWeekly = 0;
		
		
		private string FinalDataString(string defaulta)
		{
			
			
					defaulta = defaulta.Replace(" Minute", "M");
					//defaulta = defaulta.Replace(" Second", "Sec");
					//defaulta = defaulta.Replace(" Volume", "Vol");
					
				return defaulta;
			
		}
					
		int TotalBarsInProgress = 0;
		bool FirstTickAll = true;
		bool FirstTickOfLoad = true;
		
		
		bool RealTimeDailyAllowed = false;
		bool RealTimeWeeklyAllowed = false;
		
		double ThisBodyHigh = 0;
		double ThisBodyLow = 0;
		
		
		int TotalHighs = 0;
		int TotalLows = 0;
			
			
		protected override void OnBarUpdate()
		{
			
			
			if (!Permission)
				return;
           			
			
			if (FirstTickAll)
			{
		
				if (pUseSData1)
				{					
					TotalBarsInProgress = TotalBarsInProgress + 1;
					BarsInProgressMinute1 = TotalBarsInProgress;					
					Data1Name = "c" + FinalDataString(BarsPeriods[BarsInProgressMinute1].ToString());
					Data1NameFinal = FinalDataString(BarsPeriods[BarsInProgressMinute1].ToString());
				}
				
				if (pUseSData2)
				{
					TotalBarsInProgress = TotalBarsInProgress + 1;
					BarsInProgressMinute2 = TotalBarsInProgress;
					Data2Name = "b" + FinalDataString(BarsPeriods[BarsInProgressMinute2].ToString());
					Data2NameFinal = FinalDataString(BarsPeriods[BarsInProgressMinute2].ToString());
				}
				if (pUseSData3)
				{
					TotalBarsInProgress = TotalBarsInProgress + 1;
					BarsInProgressMinute3 = TotalBarsInProgress;
					Data3Name = "a" + FinalDataString(BarsPeriods[BarsInProgressMinute3].ToString());
					Data3NameFinal = FinalDataString(BarsPeriods[BarsInProgressMinute3].ToString());
				}				
				
				if (pUseDailyData)
				{
					TotalBarsInProgress = TotalBarsInProgress + 1;
					BarsInProgressDaily = TotalBarsInProgress;
					DailyDataName = "Daily";
				}
					
				if (pUseWeeklyData)
				{
					TotalBarsInProgress = TotalBarsInProgress + 1;
					BarsInProgressWeekly = TotalBarsInProgress;
					WeeklyDataName = "Weekly";
				}					
					
			
				totalEnabledTimeframes = Math.Max(1, TotalBarsInProgress);

				FirstTickAll = false;

			}
					

//			if (State == State.Realtime)
//			{
			
					
//			}			
			
			//return;
			
			
			//Print("hey 1");
			
//			Print(BarsInProgress);
			
			
			//IsInSession[0] = 0;
			
			if (BarsInProgress == 0)
			{
				
						
				if (FirstRun)
				{ 
					//Print("test");
					
//					StartTime  = new DateTime(Time[0].Year, Time[0].Month, Time[0].Day, pStartTime.Hours, pStartTime.Minutes, 0);
//					EndTime    = new DateTime(Time[0].Year, Time[0].Month, Time[0].Day, pEndTime.Hours, pEndTime.Minutes, 0);
					
//					StartTime2  = new DateTime(Time[0].Year, Time[0].Month, Time[0].Day, pStartTime2.Hours, pStartTime2.Minutes, 0);
//					EndTime2    = new DateTime(Time[0].Year, Time[0].Month, Time[0].Day, pEndTime2.Hours, pEndTime2.Minutes, 0);					
					
//					StartTime3  = new DateTime(Time[0].Year, Time[0].Month, Time[0].Day, pStartTime3.Hours, pStartTime3.Minutes, 0);
//					EndTime3    = new DateTime(Time[0].Year, Time[0].Month, Time[0].Day, pEndTime3.Hours, pEndTime3.Minutes, 0);					
					
					
					//Print("test3");
					
					CalcTime1 = new DateTime(Time[0].Year, Time[0].Month, Time[0].Day, pCalc1Time.Hours, pCalc1Time.Minutes, 0);
					CalcTime2 = new DateTime(Time[0].Year, Time[0].Month, Time[0].Day, pCalc2Time.Hours, pCalc2Time.Minutes, 0);
					
					
					//Print(StartTime);
					//Print(EndTime);
					FirstRun = false;
					
				}				
					
				reset3 = false;
				
				
				if (pTime1Reset)
				while(Time[0].Ticks > CalcTime1.Ticks)
				{
					CalcTime1 = CalcTime1.AddDays(1);
					reset3 = true;
				}				
				
				
				if (pTime2Reset)
				while(Time[0].Ticks > CalcTime2.Ticks)
				{
					CalcTime2 = CalcTime2.AddDays(1);
					reset3 = true;
				}				
				
					
//						//BackBrush = Brushes.Navy;
						
//					}
					
					
//				}
			
			
				 CurrentChartTime = Times[0][0];
				
				
			}
			
			
			//Print("hey 2");
			
			//Print(BarsArray.Length);
			
			int ThisBIP = 0;
			string ThisDataName = string.Empty;
			bool ThisTimeFilter = false;
			string ThisHitsIncluded = string.Empty;
			string ThisBarsIncluded = string.Empty;
			
			
			bool TimeIsOK = true;
			int ThisBar = 0;
			
			bool IsOneBarSwingHigh = false;
			bool IsOneBarSwingLow = false;
			
			
			ThisBIP = BarsInProgressMinute1;
			ThisDataName = Data1Name;
			ThisHitsIncluded = pHitsIncluded1;
			ThisBarsIncluded = pBarsIncluded1;
			
			ThisTimeFilter = pUseTimeFilter1;
			
			
			if (ThisBIP != 0)
			if (BarsInProgress == ThisBIP)
			{
				
				//if (IsInSession[0] == 1)
				{
				
					TimeIsOK = true;
					ThisBar = 0;
					
					if (ThisBarsIncluded == "Swings")
						ThisBar = 1;
					
					if (pUseTimeFilter && ThisTimeFilter)
					{
						StartTime  = new DateTime(Time[0].Year, Time[0].Month, Time[0].Day, pStartTime.Hours, pStartTime.Minutes, 0);
						EndTime    = new DateTime(Time[0].Year, Time[0].Month, Time[0].Day, pEndTime.Hours, pEndTime.Minutes, 0);	
						TimeIsOK = Time[ThisBar].Ticks > StartTime.Ticks && Time[ThisBar].Ticks <= EndTime.Ticks;
					}
	
					 if (IsFirstTickOfBar && TimeIsOK && CurrentBars[ThisBIP] > 1)
					 {
						
						//Print(Times[ThisBIP][0] + "  " + Closes[ThisBIP][0]);
						 
					 	ThisBodyHigh = Math.Max(Closes[ThisBIP][ThisBar], Opens[ThisBIP][ThisBar]);
						ThisBodyLow = Math.Min(Closes[ThisBIP][ThisBar], Opens[ThisBIP][ThisBar]);

						IsOneBarSwingHigh = true;
						IsOneBarSwingLow = true;
						 
						
						if (ThisBarsIncluded == "Swings")
						{						
							IsOneBarSwingHigh = Highs[ThisBIP][1] > Highs[ThisBIP][0] && Highs[ThisBIP][1] > Highs[ThisBIP][2];
							IsOneBarSwingLow = Lows[ThisBIP][1] < Lows[ThisBIP][0] &&  Lows[ThisBIP][1] < Lows[ThisBIP][2];
							ThisBar = 1;
						}
						
						
						if (IsOneBarSwingHigh)
						{
							TotalHighs = TotalHighs+1;
							
							//Print(TotalHighs);
							
							if (ThisHitsIncluded == "Both" || ThisHitsIncluded == "Wick")
							{
								AllHits.Add(new OneLevel(Highs[ThisBIP][ThisBar], "High", ThisDataName, Times[ThisBIP][ThisBar]));
								//Print(Highs[ThisBIP][ThisBar]);
							}
							
							if (ThisBodyHigh != Highs[ThisBIP][ThisBar])
							if (ThisHitsIncluded == "Both" || ThisHitsIncluded == "Body")
							{
								//Print(ThisBodyHigh);
								AllHits.Add(new OneLevel(ThisBodyHigh, "Body High", ThisDataName, Times[ThisBIP][ThisBar]));
							}
						}
						if (IsOneBarSwingLow)
						{
							 TotalLows = TotalLows+1;
							//Print(TotalLows);
							
							if (ThisHitsIncluded == "Both" || ThisHitsIncluded == "Wick")
							{
								AllHits.Add(new OneLevel(Lows[ThisBIP][ThisBar], "Low", ThisDataName, Times[ThisBIP][ThisBar]));
								//Print(Lows[ThisBIP][ThisBar]);
							}
							
							if (ThisBodyLow != Lows[ThisBIP][ThisBar])
							if (ThisHitsIncluded == "Both" || ThisHitsIncluded == "Body")
							{
								//Print(ThisBodyLow);
								AllHits.Add(new OneLevel(ThisBodyLow, "Body Low", ThisDataName, Times[ThisBIP][ThisBar]));
							}
						}			 
					
					 }
				
					
				}

				
				return;	
				
				
			}
			
			
			//Print("hey 3");
			
			
			ThisBIP = BarsInProgressMinute2;
			ThisDataName = Data2Name;
			ThisHitsIncluded = pHitsIncluded2;
			ThisBarsIncluded = pBarsIncluded2;
			
			ThisTimeFilter = pUseTimeFilter2;
			
			
			if (ThisBIP != 0)
			if (BarsInProgress == ThisBIP)
			{
				
				//if (IsInSession[0] == 1)
				{
				
					TimeIsOK = true;
					ThisBar = 0;
					
					if (ThisBarsIncluded == "Swings")
						ThisBar = ThisBar + 1;
					
					if (pUseTimeFilter && ThisTimeFilter)
					{
						StartTime  = new DateTime(Time[0].Year, Time[0].Month, Time[0].Day, pStartTime.Hours, pStartTime.Minutes, 0);
						EndTime    = new DateTime(Time[0].Year, Time[0].Month, Time[0].Day, pEndTime.Hours, pEndTime.Minutes, 0);	
						TimeIsOK = Time[ThisBar].Ticks > StartTime.Ticks && Time[ThisBar].Ticks <= EndTime.Ticks;
					}
	
					 if (IsFirstTickOfBar && TimeIsOK && CurrentBars[ThisBIP] > 1)
					 {
						 
						//Print(Times[ThisBIP][0] + "  " + Closes[ThisBIP][0]);
						 
					 	ThisBodyHigh = Math.Max(Closes[ThisBIP][ThisBar], Opens[ThisBIP][ThisBar]);
						ThisBodyLow = Math.Min(Closes[ThisBIP][ThisBar], Opens[ThisBIP][ThisBar]);

						IsOneBarSwingHigh = true;
						IsOneBarSwingLow = true;
						 
						if (ThisBarsIncluded == "Swings")
						{						
							IsOneBarSwingHigh = Highs[ThisBIP][1] > Highs[ThisBIP][0] && Highs[ThisBIP][1] > Highs[ThisBIP][2];
							IsOneBarSwingLow = Lows[ThisBIP][1] < Lows[ThisBIP][0] &&  Lows[ThisBIP][1] < Lows[ThisBIP][2];
							ThisBar = 1;
						}
						
						
						if (IsOneBarSwingHigh)
						{
							if (ThisHitsIncluded == "Both" || ThisHitsIncluded == "Wick")
								AllHits.Add(new OneLevel(Highs[ThisBIP][ThisBar], "High", ThisDataName, Times[ThisBIP][ThisBar]));
							
							if (ThisBodyHigh != Highs[ThisBIP][ThisBar])
							if (ThisHitsIncluded == "Both" || ThisHitsIncluded == "Body")
								AllHits.Add(new OneLevel(ThisBodyHigh, "Body High", ThisDataName, Times[ThisBIP][ThisBar]));
						}
						if (IsOneBarSwingLow)
						{
							
							if (ThisHitsIncluded == "Both" || ThisHitsIncluded == "Wick")
								AllHits.Add(new OneLevel(Lows[ThisBIP][ThisBar], "Low", ThisDataName, Times[ThisBIP][ThisBar]));
							
							if (ThisBodyLow != Lows[ThisBIP][ThisBar])
							if (ThisHitsIncluded == "Both" || ThisHitsIncluded == "Body")
								AllHits.Add(new OneLevel(ThisBodyLow, "Body Low", ThisDataName, Times[ThisBIP][ThisBar]));
						}	 
	
						//Print("AllHits " + AllHits.Count);
					 }
				
					
				}

				
				return;	
				
				
			}
					
			
			//Print("hey 3a");
			
			
			ThisBIP = BarsInProgressMinute3;
			ThisDataName = Data3Name;			
			ThisHitsIncluded = pHitsIncluded3;
			ThisBarsIncluded = pBarsIncluded3;
			
			ThisTimeFilter = pUseTimeFilter3;
			
			
			if (ThisBIP != 0)
			if (BarsInProgress == ThisBIP)
			{
				
				
				//if (IsInSession[0] == 1)
				{
					
					TimeIsOK = true;
					ThisBar = 0;
					
					if (ThisBarsIncluded == "Swings")
						ThisBar = 1;
					
					if (pUseTimeFilter && ThisTimeFilter)
					{
						StartTime  = new DateTime(Time[0].Year, Time[0].Month, Time[0].Day, pStartTime.Hours, pStartTime.Minutes, 0);
						EndTime    = new DateTime(Time[0].Year, Time[0].Month, Time[0].Day, pEndTime.Hours, pEndTime.Minutes, 0);	
						TimeIsOK = Time[ThisBar].Ticks > StartTime.Ticks && Time[ThisBar].Ticks <= EndTime.Ticks;
					}
	
					if (IsFirstTickOfBar && TimeIsOK && CurrentBars[ThisBIP] > 1)
					{

						//Print(Times[ThisBIP][0] + "  " + Closes[ThisBIP][0]);
						 
					 	ThisBodyHigh = Math.Max(Closes[ThisBIP][ThisBar], Opens[ThisBIP][ThisBar]);
						ThisBodyLow = Math.Min(Closes[ThisBIP][ThisBar], Opens[ThisBIP][ThisBar]);

						IsOneBarSwingHigh = true;
						IsOneBarSwingLow = true;
						 
						if (ThisBarsIncluded == "Swings")
						{						
							IsOneBarSwingHigh = Highs[ThisBIP][1] > Highs[ThisBIP][0] && Highs[ThisBIP][1] > Highs[ThisBIP][2];
							IsOneBarSwingLow = Lows[ThisBIP][1] < Lows[ThisBIP][0] &&  Lows[ThisBIP][1] < Lows[ThisBIP][2];
							ThisBar = 1;
						}
						
						
						if (IsOneBarSwingHigh)
						{
							if (ThisHitsIncluded == "Both" || ThisHitsIncluded == "Wick")
								AllHits.Add(new OneLevel(Highs[ThisBIP][ThisBar], "High", ThisDataName, Times[ThisBIP][ThisBar]));
							
							if (ThisBodyHigh != Highs[ThisBIP][ThisBar])
							if (ThisHitsIncluded == "Both" || ThisHitsIncluded == "Body")
								AllHits.Add(new OneLevel(ThisBodyHigh, "Body High", ThisDataName, Times[ThisBIP][ThisBar]));
						}
						if (IsOneBarSwingLow)
						{
							
							if (ThisHitsIncluded == "Both" || ThisHitsIncluded == "Wick")
								AllHits.Add(new OneLevel(Lows[ThisBIP][ThisBar], "Low", ThisDataName, Times[ThisBIP][ThisBar]));
							
							if (ThisBodyLow != Lows[ThisBIP][ThisBar])
							if (ThisHitsIncluded == "Both" || ThisHitsIncluded == "Body")
								AllHits.Add(new OneLevel(ThisBodyLow, "Body Low", ThisDataName, Times[ThisBIP][ThisBar]));
						}		 
						 
					 }
	
				}
			
				return;	
			
			}
					
			
			//Print("hey 3b");
			
			
			ThisBIP = BarsInProgressDaily;
			ThisDataName = DailyDataName;
			ThisHitsIncluded = pHitsIncluded4;
			ThisBarsIncluded = pBarsIncluded4;
			
			
			if (ThisBIP != 0)
			if (BarsInProgress == ThisBIP)
			{
				
				ThisBar = 0;

				 if (Times[ThisBIP][0] <= CurrentChartTime) // filters out current bar when chart loads
				 if (IsFirstTickOfBar && CurrentBars[ThisBIP] > 1)
				 {

						//Print(Times[ThisBIP][0] + "  " + Closes[ThisBIP][0]);
						 
					 	ThisBodyHigh = Math.Max(Closes[ThisBIP][ThisBar], Opens[ThisBIP][ThisBar]);
						ThisBodyLow = Math.Min(Closes[ThisBIP][ThisBar], Opens[ThisBIP][ThisBar]);

						IsOneBarSwingHigh = true;
						IsOneBarSwingLow = true;
						 
						if (ThisBarsIncluded == "Swings")
						{						
							IsOneBarSwingHigh = Highs[ThisBIP][1] > Highs[ThisBIP][0] && Highs[ThisBIP][1] > Highs[ThisBIP][2];
							IsOneBarSwingLow = Lows[ThisBIP][1] < Lows[ThisBIP][0] &&  Lows[ThisBIP][1] < Lows[ThisBIP][2];
							ThisBar = 1;
						}
						
						
						if (IsOneBarSwingHigh)
						{
							if (ThisHitsIncluded == "Both" || ThisHitsIncluded == "Wick")
								AllHits.Add(new OneLevel(Highs[ThisBIP][ThisBar], "High", ThisDataName, Times[ThisBIP][ThisBar]));
							
							if (ThisBodyHigh != Highs[ThisBIP][ThisBar])
							if (ThisHitsIncluded == "Both" || ThisHitsIncluded == "Body")
								AllHits.Add(new OneLevel(ThisBodyHigh, "Body High", ThisDataName, Times[ThisBIP][ThisBar]));
						}
						if (IsOneBarSwingLow)
						{
							
							if (ThisHitsIncluded == "Both" || ThisHitsIncluded == "Wick")
								AllHits.Add(new OneLevel(Lows[ThisBIP][ThisBar], "Low", ThisDataName, Times[ThisBIP][ThisBar]));
							
							if (ThisBodyLow != Lows[ThisBIP][ThisBar])
							if (ThisHitsIncluded == "Both" || ThisHitsIncluded == "Body")
								AllHits.Add(new OneLevel(ThisBodyLow, "Body Low", ThisDataName, Times[ThisBIP][ThisBar]));
						}
						
						
//					AllHits.Add(new OneLevel(Opens[ThisBIP][0], "Open", ThisDataName, Times[ThisBIP][0]));	
//					AllHits.Add(new OneLevel(Closes[ThisBIP][0], "Close", ThisDataName, Times[ThisBIP][0]));	

				 }
	
				return;	

			}
			
			
		//Print("hey 3c");
			
			
			ThisBIP = BarsInProgressWeekly;
			ThisDataName = WeeklyDataName;
			ThisHitsIncluded = pHitsIncluded5;
			ThisBarsIncluded = pBarsIncluded5;
							
				
			if (ThisBIP != 0)
			if (BarsInProgress == ThisBIP)
			{

				//Print("hey 3d");
				
				
				ThisBar = 0;
				
				if (Times[ThisBIP][0] <= CurrentChartTime) // filters out current bar when chart loads
				 if (IsFirstTickOfBar && CurrentBars[ThisBIP] > 1)
				 {

					 
					 //Print("hey 3e");

						//Print(Times[ThisBIP][0] + "  " + Closes[ThisBIP][0]);
						 
					 	ThisBodyHigh = Math.Max(Closes[ThisBIP][ThisBar], Opens[ThisBIP][ThisBar]);
						ThisBodyLow = Math.Min(Closes[ThisBIP][ThisBar], Opens[ThisBIP][ThisBar]);

						IsOneBarSwingHigh = true;
						IsOneBarSwingLow = true;
						 
						if (ThisBarsIncluded == "Swings")
						{						
							IsOneBarSwingHigh = Highs[ThisBIP][1] > Highs[ThisBIP][0] && Highs[ThisBIP][1] > Highs[ThisBIP][2];
							IsOneBarSwingLow = Lows[ThisBIP][1] < Lows[ThisBIP][0] &&  Lows[ThisBIP][1] < Lows[ThisBIP][2];
							ThisBar = 1;
						}
						
						
						//Print("hey 3f");
						
						if (IsOneBarSwingHigh)
						{
							if (ThisHitsIncluded == "Both" || ThisHitsIncluded == "Wick")
								AllHits.Add(new OneLevel(Highs[ThisBIP][ThisBar], "High", ThisDataName, Times[ThisBIP][ThisBar]));
							
							if (ThisBodyHigh != Highs[ThisBIP][ThisBar])
							if (ThisHitsIncluded == "Both" || ThisHitsIncluded == "Body")
								AllHits.Add(new OneLevel(ThisBodyHigh, "Body High", ThisDataName, Times[ThisBIP][ThisBar]));
						}
						if (IsOneBarSwingLow)
						{
							
							if (ThisHitsIncluded == "Both" || ThisHitsIncluded == "Wick")
								AllHits.Add(new OneLevel(Lows[ThisBIP][ThisBar], "Low", ThisDataName, Times[ThisBIP][ThisBar]));
							
							if (ThisBodyLow != Lows[ThisBIP][ThisBar])
							if (ThisHitsIncluded == "Both" || ThisHitsIncluded == "Body")
								AllHits.Add(new OneLevel(ThisBodyLow, "Body Low", ThisDataName, Times[ThisBIP][ThisBar]));
						}
						
						//Print("hey 3g");
						
						
//					AllHits.Add(new OneLevel(Opens[ThisBIP][0], "Open", ThisDataName, Times[ThisBIP][0]));	
//					AllHits.Add(new OneLevel(Closes[ThisBIP][0], "Close", ThisDataName, Times[ThisBIP][0]));	
						
						
				 }
	
				return;	

			}
						
			
			//Print("hey 4");
			
			
//			if (State == State.Realtime)
//			{
				
			
			//return;
			
			
			//
			
			
			bool useringedpivots = false;
			
			
//			SwingLow2[0] = iZZ.ZigZagLow[0];
//			SwingHigh2[0] = iZZ.ZigZagHigh[0];		
			
			if (CurrentBar <= 0)
				return;
	
			
			//if (FirstTickOfLoad)
			
			if (pShowPriceMarkers)
			if (VisibleBarsStart != -1 && VisibleBarsEnd != -1)
			if (CurrentBars[0] == BarsArray[0].Count - 1)
			if (IsFirstTickOfBar || RedoMarkers)
			{
			
						for (int i = 0; i<=99; i++)
						{
							Values[i].Reset();
						}
						
						
					int j = 0;
				
					for (int i=0; i < zones.Count; i++)  //loop through each zone and draw it
					{
						if (zoneEnd[i] < VisibleBarsStart || zoneStart[i] > VisibleBarsEnd)
							continue;
						
						
						if (zones[i].Low > VisiblePriceHigh || zones[i].High < VisiblePriceLow)		
							continue;
					
						if (zoneEnd[i] < VisibleBarsEnd)
							continue;
						
						
						if (j <= 99)
						{
						
						
							if (Closes[0][0] <= zones[i].High)
							{
								
								Values[j][0] = zones[i].Low;
							}
							else
							{
								Values[j][0] = zones[i].High;
								
							}
							
							
							if (Data3NameFinal != string.Empty)
							if (zones[i].AllDetails.Contains(Data3NameFinal))
								PlotBrushes[j][0] = pZoneColor1;
			
							if (Data2NameFinal != string.Empty)
							if (zones[i].AllDetails.Contains(Data2NameFinal))
								PlotBrushes[j][0] = pZoneColor2;
							
							if (Data1NameFinal!= string.Empty)
							if (zones[i].AllDetails.Contains(Data1NameFinal))
								PlotBrushes[j][0] = pZoneColor3;		
							
				
							if (zones[i].AllDetails.Contains("Daily"))
								PlotBrushes[j][0] = pZoneColor4;
																									
							
							if (zones[i].AllDetails.Contains("Weekly"))
								PlotBrushes[j][0] = pZoneColor5;
							
						
						}
						
						j++;
						
					}
				
			
				//FirstTickOfLoad = false;
					
			}
			
			
			//Values[0][0] = 13390;
			
			
			// Dead code removed — SwingHigh2/SwingLow2 were never written, conditions always false
				
			
//				if (IsInSession[0] == 1)
//				{
				
					
//					pivotHighs.Add(swingHighSeries[0]);
//					pivotHighsTimes.Add(Times[0][0]);
					
//				}
							
//				}
				
				
//				}					
				

//				}	
				
				
//			}
			
			
//			if (timeBegin == 0 && timeEnd == 0)
//				timeReady = true;
			
			// find and split the chart into sections based on time
			
			calczones = false;
			
			if (CurrentBar == 0)  // Don't do anything on bar 0
				return;
			
			if (CurrentBar == 1)  // Start once we have at least 1 bar
			{
				chartdates.Add(ToDay(Time[0]));
				startbar.Add(CurrentBar);	
			}
			
//			bool reset1 = ToTime(Time[0]) > timeCalculate && ToTime(Time[1]) <= timeCalculate;
//			reset1 = false;
			
			bool reset2 = ToTime(Time[0]) > 180000 && ToTime(Time[1]) <= 180000;
			reset2 = Bars.IsFirstBarOfSession && pSessionReset;
			
			if (reset2 || reset3)
				
			{
				endbar.Add(CurrentBar - 1);	
				chartdates.Add(ToDay(Time[0]));
				startbar.Add(CurrentBar);
				
				calczones = true;	
					
			}	
				

// Find and store swing low and high pivots based on pStrength
				
//			if (CurrentBar < (2 * pStrength))  // Start once we have enough data in lastHighCache / lastLowCache
//				return;
				
			if (timeReady == true)
			{
				
			
			}
			
// Begin calculating confluence levels, on the next bar
			
//  start by count up all qualifying high and low pivots at each tick level on the chart
		
			
			if (IsFirstTickOfBar)
			//if (calczones == true && (ToDay(Time[0]) >= ToDay(DateTime.Now.AddDays(-pDaysBack))))
			if (calczones == true)
			{
				// During historical processing, only accumulate hits — defer zone calculation
				// to avoid dozens of expensive CalculateZones() calls (one per session boundary).
				if (State == State.Historical)
				{
					for (int i = 0; i < AllHits.Count; i++)
						AllHitsChart.Add(AllHits[i]);
					AllHits.Clear();
					calczones = false;
				}
				else
				{
					CalculateZones();
					ExportZonesToCsv();
				}
			}

			// Deferred zone calc normally runs in OnStateChange(State.Realtime). Fallback here
			// in case OnStateChange doesn't trigger it (covers edge cases).
			if (!deferredZonesCalculated && State != State.Historical && BarsInProgress == 0)
			{
				deferredZonesCalculated = true;
				calczones = true;
				CalculateZones();
				ExportZonesToCsv();
			}

			return;
			
			
//			SignalReady[0] = 0;
			
//			//if (CurrentBar < pMinRun || CurrentBar < pMinSec || ParameterInputError) return;

			
////			BackBrushes[0] = null;
////			BackBrushesAll[0] = null;
				
//			if (CurrentBar == 0)
//				return;
			
				
//			SignalReady[0] = SignalReady[1];
//				Signals[0] = 0;
		
//			if (SignalReady[0] == 0)
//				SignalReady[0] = 1;
			
			
//			}
			
				
//			}
			
//			LastSignal[0] = LastSignal[1];
			
//			if (Signals[0] != 0)
//				LastSignal[0] = Signals[0];
			
			
//			bool BuyOK = true;
//			bool SellOK = true;
			
		
//			// cancel trade based on ema trend
			
//			ThisTrend1[0] = 0;
			
//			if (iEMA1[0] > iEMA2[0])
//				ThisTrend1[0] = 1;
			
			
////			if (ThisTrend1[0] != Signals[0])
////				Signals[0] = 0;
			
			
//			// look for next trade after expansion bar.
			
//			ThisTrend2[0] = ThisTrend2[1];
			
//			if (iATR1[0] > iATR2[0]*2)
//				ThisTrend2[0] = LastSignal[0]*-1;
			
			
//			if (ThisTrend2[1] == 0)
//				Signals[0] = 0;
			
//			if (Signals[0] != 0)
//				ThisTrend2[0] = 0;
			
			
//			}
			
			
//			//SellOK = Close[1] > Open[1] && Close[0] < Open[0];
			
		
//			}
			
			
		}

			
		private void ExportZonesToCsv(bool forceExport = false)
		{
			if (!pExportEnabled)
				return;

			// Auto-recalc exports: only in realtime, and only if auto-export is on
			if (!forceExport)
			{
				if (State != State.Realtime)
					return;
				if (!pExportAutoOnRecalc)
					return;
			}

			if (zones == null || zones.Count == 0)
				return;

			try
			{
				string folder = System.IO.Path.Combine(NinjaTrader.Core.Globals.UserDataDir, "Affordable Indicators", "Price Action Confluence");

				if (!System.IO.Directory.Exists(folder))
					System.IO.Directory.CreateDirectory(folder);

				string instrument = Instrument.MasterInstrument.Name;
				string barPeriod = BarsArray[0].BarsPeriod.ToString().Replace(" ", "_");

				string fileName;
				if (pExportTimestamp)
					fileName = instrument + "_" + barPeriod + "_Zones_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";
				else
					fileName = instrument + "_" + barPeriod + "_Zones.csv";

				string filePath = System.IO.Path.Combine(folder, fileName);

				string delim = ",";
				if (pExportDelimiter == "Tab") delim = "\t";
				else if (pExportDelimiter == "Semicolon") delim = ";";

				StringBuilder sb = new StringBuilder();

				// Header
				string header = "Zone#" + delim + "High" + delim + "Low" + delim + "StartTime" + delim + "HitCount" + delim + "LatestHit" + delim + "Details" + delim + "Strength" + delim + "State" + delim + "TestCount";
				sb.AppendLine(header);

				int exported = 0;

				for (int i = 0; i < zones.Count; i++)
				{
					// Max age filter (0 = no limit for backwards compat)
					if (pExportMaxAgeDays > 0)
					{
						double ageDays = (DateTime.Now - zones[i].LatestHit).TotalDays;
						if (ageDays > pExportMaxAgeDays)
							continue;
					}

					DateTime startTime = DateTime.MinValue;
					int startBarIdx = zoneStart[i];
					if (startBarIdx >= 0 && Times[0].IsValidDataPointAt(startBarIdx))
						startTime = Times[0].GetValueAt(startBarIdx);

					string state = "N/A";
					string strength = "N/A";
					int testCount = 0;

					if (pStrengthEnabled)
						strength = zones[i].Strength.ToString("F3");

					if (pFreshnessEnabled)
					{
						state = zones[i].State.ToString();
						testCount = zones[i].TestCount;
					}

					string details = zones[i].AllDetails.Replace("\"", "'");
					if (delim == ",")
						details = "\"" + details + "\"";

					sb.AppendLine(
						(i + 1).ToString() + delim +
						zones[i].High + delim +
						zones[i].Low + delim +
						startTime.ToString("yyyy-MM-dd HH:mm") + delim +
						zones[i].ThisAllHits.Count + delim +
						zones[i].LatestHit.ToString("yyyy-MM-dd HH:mm") + delim +
						details + delim +
						strength + delim +
						state + delim +
						testCount
					);

					// Optional individual hit detail rows
					if (pExportIncludeHitDetails && zones[i].ThisAllHits != null)
					{
						for (int h = 0; h < zones[i].ThisAllHits.Count; h++)
						{
							OneLevel hit = zones[i].ThisAllHits[h];
							string hitData = GetDataName(hit.ThisData);
							sb.AppendLine(
								"" + delim +
								hit.ThisPrice + delim +
								"" + delim +
								hit.ThisTime.ToString("yyyy-MM-dd HH:mm") + delim +
								"" + delim +
								"" + delim +
								hitData + " " + hit.ThisName + delim +
								"" + delim +
								"" + delim +
								""
							);
						}
					}

					exported++;
				}

				System.IO.File.WriteAllText(filePath, sb.ToString());
				//Print("Zone export: " + filePath + " (" + exported + " zones)");
			}
			catch (Exception ex)
			{
				//Print("Zone export error: " + ex.Message);
			}
		}

		private void CalculateZones()
		{
			
			
			// add hits to list of all hits for chart, and then reset
			
			// allhits becomes the list of hits that have NOT YET been configured in with the zones
			
			for (int i=0; i < AllHits.Count; i++)
				AllHitsChart.Add(AllHits[i]);
			
			AllHits.Clear();
			
			
			{

				calczones = false;

				// CRITICAL: Clear zones list before rebuild — previously grew unbounded
				// every session boundary, accumulating duplicate zones indefinitely.
				// Since CalculateZones rebuilds all zones from AllHitsChart (the full
				// cumulative hit list), we always produce the complete correct set.
				zones.Clear();
				zoneStart.Clear();
				zoneEnd.Clear();

				AllZones.Clear();
				List<OneLevel> AllHitsInZone = new List<OneLevel>();
				
				
				ticklevels.Clear();
				double sessionHigh;
				double sessionLow;

				if (pZoneDistance == 0)
				{
					sessionHigh = High[HighestBar(High, Bars.Count)];
					sessionLow = Low[LowestBar(Low, Bars.Count)];
				}
				else
				{
					sessionHigh = Close[0] + TickSize * pZoneDistance;
					sessionLow = Close[0] - TickSize * pZoneDistance;
				}

				// Build a dictionary keyed by tick-rounded price for O(1) lookups
				// instead of scanning all hits for every tick level (O(N*M) -> O(N+M))
				Dictionary<long, List<OneLevel>> hitsByTick = new Dictionary<long, List<OneLevel>>();
				for (int i = 0; i < AllHitsChart.Count; i++)
				{
					long key = (long) Math.Round(AllHitsChart[i].ThisPrice / TickSize);
					List<OneLevel> bucket;
					if (!hitsByTick.TryGetValue(key, out bucket))
					{
						bucket = new List<OneLevel>();
						hitsByTick[key] = bucket;
					}
					bucket.Add(AllHitsChart[i]);
				}

				double n = sessionLow;
				DateTime LatestT = DateTime.MinValue;

				List<OneLevel> emptyHits = new List<OneLevel>(); // shared empty list to avoid thousands of allocations
				while (n < sessionHigh)
				{
					long tickKey = (long) Math.Round(n / TickSize);
					List<OneLevel> bucket;
					if (hitsByTick.TryGetValue(tickKey, out bucket))
					{
						LatestT = DateTime.MinValue;
						countHighPivots = 0;
						countLowPivots = 0;
						for (int i = 0; i < bucket.Count; i++)
						{
							if (bucket[i].ThisName.Contains("High"))
								countHighPivots++;
							if (bucket[i].ThisName.Contains("Low"))
								countLowPivots++;
							if (DateTime.Compare(bucket[i].ThisTime, LatestT) > 0)
								LatestT = bucket[i].ThisTime;
						}
						ticklevels.Add(new TickLevel(n, countHighPivots, countLowPivots, 0, 0, LatestT, bucket));
					}
					else
					{
						ticklevels.Add(new TickLevel(n, 0, 0, 0, 0, DateTime.MinValue, emptyHits));
					}
					n = n + TickSize;
				}
				
				
				for (int i=0; i < ticklevels.Count-pZoneWidth+1; i++)  //loop through each tick level, and check for balance in the pivots
				{	
					int h = 0;
					int l = 0;
					int htotal = 0;
					int ltotal = 0;
					int total = 0;
					
					
					double zl = ticklevels[i].Price;
					double zh = zl + TickSize*(pZoneWidth-1);
					
					double finallisthigh = 0;
					double finallistlow = 99999999;
					
					LatestT = DateTime.MinValue;
					
					AllHitsInZone.Clear();
					
					
					for (int j=0; j < pZoneWidth; j++)  // define high and low levels
					{
						
						foreach(OneLevel ThisHit in ticklevels[i+j].ThisAllHits)	
							AllHitsInZone.Add(ThisHit);
							
						h = ticklevels[i+j].HighCount; 
						l = ticklevels[i+j].LowCount;

						
						if (DateTime.Compare(ticklevels[i+j].LatestHit, LatestT) > 0)
							LatestT = ticklevels[i+j].LatestHit;
						
//						Print("----------------");
//						Print(LatestT);
						
//						Print(ticklevels[i+j].LatestHit);
						
						
							htotal = h + htotal;
							ltotal = l + ltotal;
							total = htotal + ltotal;
							

						// if a pivot exists, then update the high and low 2018
						
						if (h != 0 || l != 0)
						{
							finallisthigh = Math.Max(finallisthigh, ticklevels[i+j].Price);
							finallistlow = Math.Min(finallistlow, ticklevels[i+j].Price);
													
							
						}
						
						
					}
					
					
					//Print("111 TOTAL " + total);
					
					if (total >= pMinimumPivots) // 2018
					//if (htotal >= pMinimumPivots || ltotal >= pMinimumPivots)  // check for the minimum pivots per one side, high or now
						
						
					{
						
						
						bool qualified = false;
						if (htotal > ltotal)  // if we have more highs than lows, make sure we have balance
						{
							if (ltotal == 0)
								balance = 0;
							else
								balance = (double)ltotal / (double)htotal * 100;
							if (balance >= pPivotBalance)
								qualified = true;
						}
						
						if (ltotal > htotal)  // if we have more highs than lows, make sure we have balance
						{
							if (htotal == 0)
								balance = 0;
							else
								balance = (double)htotal / (double)ltotal * 100;
							if (balance >= pPivotBalance)
								qualified = true;
						}
						
						if (qualified)
						{
							
							
							//zoneLow.Add(zl);	
							//zoneHigh.Add(zh);
							
							// new show actual high and low based on the the pivots 2018
							
							
							AllZones.Add(new ZoneDetails(balance, htotal, ltotal, finallisthigh, finallistlow, LatestT, new List<OneLevel>(AllHitsInZone)));
							
							//Print("111 ADD ZONE " + htotal + " " + ltotal + " " + AllHitsInZone.Count);
							
						}
						
						
					}
						
				}
				
				
				{
					double spacetogo = 0;  // the amount of space before a new zone is ok.
					double zlf = 0;  // store the latest low pivot for a zone
					double zhf = 0;  // store the latest high pivot for a zone
					double zhp = 0;
					double zlp = 0;
					double zpb = 0;
					double zhlast = 0;
					double zllast = 0;
					int totallast = 0;  // store the total pivots for the latest and greatest zone
					List<OneLevel> FinalHitsLast = new List<OneLevel>();
					
					
					DateTime datelast = DateTime.MinValue;
					OneLevel lastOneLevel = new OneLevel();
					
					
					long sdasd = 0;
					
				
					for (int k=0; k < AllZones.Count; k++) 
					//for (int k=0; k < zoneLow.Count; k++)  //loop through the zones
					{		
//						double zl = (double) zoneLow[k];
//						double zh = (double) zoneHigh[k];
						
//						int ltotal = (int) zoneLowCount[k];
//						int htotal = (int) zoneHighCount[k];
						

//						DateTime latesthitttt = DateTime.FromBinary((long)zoneTime[k]);
						
						
						double zl = (double) AllZones[k].FLow;
						double zh = (double) AllZones[k].FHigh;
						
						int ltotal = (int) AllZones[k].HighCount;
						int htotal = (int) AllZones[k].LowCount;
						
						int total = ltotal + htotal;
						double balance = (double) AllZones[k].Balance;
						double spaceTicks = pZoneSpace * TickSize;

						DateTime latesthitttt = AllZones[k].LatestHit;
						
						
						List<OneLevel> FinalHits = new List<OneLevel>(AllZones[k].ThisAllHits);
						
						
						//Print("-----------------");
						//Print(total + " " + AllZones[k].ThisAllHits.Count);
						
						//Print(latesthitttt);
						
					//	int asfsa = (int) latesthitttt;
						
						
						bool qualified = true;
						if (k == 0)  // for the first zone, the bottom zone
						{
							qualified = false;  // do not paint the first zone on the chart, because we don't know yet if it is qualified
							zlf = zl;
							zhf = zh;
							zpb = balance;
							
							totallast = total;
							FinalHitsLast = FinalHits;
							datelast = latesthitttt;
							
						}	
						if (k > 0)
						{	
							qualified = false;  //should be false, then we decide when to paint based on logic below.
							//Print(zl + " low current zone versus high of the last zone " + zhf);
							//Print(spaceTicks);
							if (zl >= zhf + spaceTicks )  // if we have enough space between the potential zone and the next qualified level, go ahead and paint the zone and move on
								qualified = true;
				
							
							if (qualified)
							{
								if (pZoneWidth == 1)
								{
									//Print("-------------------- " + zlf.ToString("0.0000"));
								}
									else
								{
										//Print("-------------------- " + zlf.ToString("0.0000") + " to " + zhf.ToString("0.0000"));	
								}	
								
								
								double balancep = Math.Round(zpb, 2);
							
//								if (pZoneWidth == 1)
									
//									Draw.HorizontalLine(this, tagline, zlf, Brushes.SteelBlue);
								
							
								//DrawRectangle(tagrect, false, 0, zlf, Bars.Count-2, zhf, colorTrim, colorFill, colorShade);
								
								
								AllHitsByData.Clear();
								
										string alldetails = string.Empty;
								
								
											foreach(OneLevel oneee in FinalHitsLast)
											{
												
												// combine data stats into one list
												
												// number of hits, latest hit
												
												// get details for latest hits
												
												
												if (datelast == oneee.ThisTime)
													lastOneLevel = oneee;
												
												if (!AllHitsByData.ContainsKey(oneee.ThisData))
												{
													OneLevelStats OLS = new OneLevelStats();
													OLS.ThisData = oneee.ThisName + " | ";
													OLS.ThisName = string.Empty;
													OLS.ThisTime = oneee.ThisTime;
													OLS.ThisPrice = oneee.ThisPrice;
													OLS.TotalHits = 1;
													AllHitsByData.Add(oneee.ThisData, OLS);
												}
												else
												{
													OneLevelStats OLS = AllHitsByData[oneee.ThisData];
													OLS.TotalHits = OLS.TotalHits + 1;
													AllHitsByData[oneee.ThisData] = OLS;
												}
											}

											// show
											string spacer = "   ";
											StringBuilder sbDetails = new StringBuilder();
											foreach (KeyValuePair<string, OneLevelStats> kvp in AllHitsByData)
											{
												string dataname = GetDataName(kvp.Key);
												if (sbDetails.Length > 0) sbDetails.Append(spacer);
												sbDetails.Append(dataname).Append(" [").Append(kvp.Value.TotalHits).Append(']');
											}
											alldetails = sbDetails.ToString();
											
											
								// Store raw hit density in Strength field temporarily for normalization pass
			double rawHitDensity = totallast / (double) Math.Max(1, pZoneWidth);

			// StartBar = 0 so zones always span the full chart (matches user intent)
			zones.Add(new Zone(zhf, zlf, 0, CurrentBar+10000, totallast, balancep, datelast, lastOneLevel, FinalHitsLast, alldetails, rawHitDensity, ZoneState.Untested, 0));

								zoneStart.Add(0);
								zoneEnd.Add(CurrentBar+10000);

								
								//zlf = zllast;
								//zhf = zhlast;
								
								zlf = zl;	// set the next potential zone low
								zhf = zh;	// set the next potential zone high
								zpb = balance;  //added
								
								totallast = total;	// we have no "last" zone, so set total pivots for last zone to total for the new one
								FinalHitsLast = FinalHits;
								datelast = latesthitttt;
							}
							
							if (total > totallast)   // if the latest zone is more qualified
							{
								//Print("New Zone More Qualified:    " + total + " pivots >= " + totallast + " pivots");
								//Print("-------------------- ");
							
								zlp = zlf;
								zhp = zhf;
									
								zlf = zl;	// update the next potential zone low
								zhf = zh;	// update the next potential zone high
								zpb = balance;  //added
									
								spacetogo = zhf + (pZoneSpace * TickSize);  // reset the space to go
								
								totallast = total;
								FinalHitsLast = FinalHits;
								datelast = latesthitttt;
								
							}
							
							if (total == totallast && zh - zlf <= TickSize*pZoneWidth - 0.0001 + TickSize*pZoneExpansion )   // if the latest zone is equal in value
							{
								//Print("Zone Equal" + total + " pivots new >= verses pivots last" + totallast);	
								zhf = zh;  // update only the next potential zone high, because the low stays the same.  since two zones are the same qualified, we go ahead and expand the overall zone width
								spacetogo = zhf + (pZoneSpace * TickSize);  // reset the space to go
								
								totallast = total;
								FinalHitsLast = FinalHits;
								datelast = latesthitttt;
									
									
								//Print("potential zone is " + zlf + "to " + zhf);
								//Print("-------------------- ");
								
							}
							if (total < totallast)   // if the latest zone is NOT more qualified
							{	
								
					
							}		
						}
					}	
				
				}

			}

			// --- Strength normalization pass ---
			DateTime nowCached = DateTime.Now;
			if (pStrengthEnabled && zones.Count > 0)
			{
				double maxRawDensity = 0;
				for (int i = 0; i < zones.Count; i++)
					maxRawDensity = Math.Max(maxRawDensity, zones[i].Strength);

				for (int i = 0; i < zones.Count; i++)
				{
					Zone z = zones[i];
					double hitDensity = maxRawDensity > 0 ? z.Strength / maxRawDensity : 0;

					// Count distinct timeframes from AllDetails (format: "DataName [N]   DataName [N]")
					int tfCount = 0;
					if (!string.IsNullOrEmpty(z.AllDetails))
					{
						string[] parts = z.AllDetails.Split(new[] {"   "}, StringSplitOptions.RemoveEmptyEntries);
						tfCount = parts.Length;
					}
					double tfDiversity = tfCount / (double) totalEnabledTimeframes;

					double recency = Math.Exp(-(nowCached - z.LatestHit).TotalDays / Math.Max(0.1, pRecencyHalfLife));
					double balanceQ = Math.Min(z.Balance / 50.0, 1.0);

					z.Strength = 0.35 * hitDensity + 0.25 * Math.Min(tfDiversity, 1.0) + 0.25 * recency + 0.15 * balanceQ;
					zones[i] = z;
				}
			}

			// --- Zone freshness classification pass ---
			if (pFreshnessEnabled && zones.Count > 0)
			{
				for (int i = 0; i < zones.Count; i++)
				{
					Zone z = zones[i];
					z.State = ZoneState.Untested;
					z.TestCount = 0;

					int startIdx = z.StartBar;
					int barsBack = CurrentBars[0] - startIdx;

					for (int b = barsBack; b >= 0; b--)
					{
						if (!Lows[0].IsValidDataPointAt(CurrentBars[0] - b))
							continue;

						double barLow = Lows[0].GetValueAt(CurrentBars[0] - b);
						double barHigh = Highs[0].GetValueAt(CurrentBars[0] - b);
						double barClose = Closes[0].GetValueAt(CurrentBars[0] - b);

						bool overlaps = barLow <= z.High && barHigh >= z.Low;
						if (!overlaps)
							continue;

						bool closedInside = barClose >= z.Low && barClose <= z.High;
						if (closedInside)
						{
							z.State = ZoneState.Broken;
							z.TestCount++;
							break;
						}
						else
						{
							z.State = ZoneState.Tested;
							z.TestCount++;
						}
					}

					zones[i] = z;
				}
			}

		}

		public override void OnRenderTargetChanged()
		{
			// Dispose all cached brushes
			if (cachedZoneColor1DX != null) { cachedZoneColor1DX.Dispose(); cachedZoneColor1DX = null; }
			if (cachedZoneColor2DX != null) { cachedZoneColor2DX.Dispose(); cachedZoneColor2DX = null; }
			if (cachedZoneColor3DX != null) { cachedZoneColor3DX.Dispose(); cachedZoneColor3DX = null; }
			if (cachedZoneColor4DX != null) { cachedZoneColor4DX.Dispose(); cachedZoneColor4DX = null; }
			if (cachedZoneColor5DX != null) { cachedZoneColor5DX.Dispose(); cachedZoneColor5DX = null; }
			if (cachedChartTextBrushDX != null) { cachedChartTextBrushDX.Dispose(); cachedChartTextBrushDX = null; }
			if (cachedChartBackgroundBrushDX != null) { cachedChartBackgroundBrushDX.Dispose(); cachedChartBackgroundBrushDX = null; }
			if (cachedLabelBrushDX != null) { cachedLabelBrushDX.Dispose(); cachedLabelBrushDX = null; }
			if (cachedButtonBrushDX != null) { cachedButtonBrushDX.Dispose(); cachedButtonBrushDX = null; }
			if (cachedButtonHBrushDX != null) { cachedButtonHBrushDX.Dispose(); cachedButtonHBrushDX = null; }
			if (cachedButtonFHBrushDX != null) { cachedButtonFHBrushDX.Dispose(); cachedButtonFHBrushDX = null; }
			if (cachedButtonFOFFBrushDX != null) { cachedButtonFOFFBrushDX.Dispose(); cachedButtonFOFFBrushDX = null; }
			if (cachedButtonFONBrushDX != null) { cachedButtonFONBrushDX.Dispose(); cachedButtonFONBrushDX = null; }
			if (cachedButtonFH2BrushDX != null) { cachedButtonFH2BrushDX.Dispose(); cachedButtonFH2BrushDX = null; }
			if (cachedAreaBrushDX != null) { cachedAreaBrushDX.Dispose(); cachedAreaBrushDX = null; }
			if (cachedPanelBackdropBrushDX != null) { cachedPanelBackdropBrushDX.Dispose(); cachedPanelBackdropBrushDX = null; }
			if (cachedHeaderBgBrushDX != null) { cachedHeaderBgBrushDX.Dispose(); cachedHeaderBgBrushDX = null; }
			if (cachedHeaderBgHoverBrushDX != null) { cachedHeaderBgHoverBrushDX.Dispose(); cachedHeaderBgHoverBrushDX = null; }
			if (cachedHeaderTextBrushDX != null) { cachedHeaderTextBrushDX.Dispose(); cachedHeaderTextBrushDX = null; }
			if (cachedHoverGlowBrushDX != null) { cachedHoverGlowBrushDX.Dispose(); cachedHoverGlowBrushDX = null; }
			if (cachedErrorBrushDX != null) { cachedErrorBrushDX.Dispose(); cachedErrorBrushDX = null; }

			// Dispose cached text formats
			if (cachedLabelTextFormat != null) { cachedLabelTextFormat.Dispose(); cachedLabelTextFormat = null; }
			if (cachedButtonTextFormat != null) { cachedButtonTextFormat.Dispose(); cachedButtonTextFormat = null; }
			if (cachedHeaderTextFormat != null) { cachedHeaderTextFormat.Dispose(); cachedHeaderTextFormat = null; }
			if (cachedErrorTextFormat != null) { cachedErrorTextFormat.Dispose(); cachedErrorTextFormat = null; }

			// Dispose reusable text layout
			if (reusableTextLayout != null) { reusableTextLayout.Dispose(); reusableTextLayout = null; }

			// Dispose cached stroke styles
			if (cachedStrokeUntested != null) { cachedStrokeUntested.Dispose(); cachedStrokeUntested = null; }
			if (cachedStrokeTested != null) { cachedStrokeTested.Dispose(); cachedStrokeTested = null; }
			if (cachedStrokeBroken != null) { cachedStrokeBroken.Dispose(); cachedStrokeBroken = null; }

			if (RenderTarget != null)
			{
				// Recreate zone color brushes
				cachedZoneColor1DX = pZoneColor1.ToDxBrush(RenderTarget);
				cachedZoneColor1DX.Opacity = pZoneOpacity1 / 100f;

				cachedZoneColor2DX = pZoneColor2.ToDxBrush(RenderTarget);
				cachedZoneColor2DX.Opacity = pZoneOpacity2 / 100f;

				cachedZoneColor3DX = pZoneColor3.ToDxBrush(RenderTarget);
				cachedZoneColor3DX.Opacity = pZoneOpacity3 / 100f;

				cachedZoneColor4DX = pZoneColor4.ToDxBrush(RenderTarget);
				cachedZoneColor4DX.Opacity = pZoneOpacity4 / 100f;

				cachedZoneColor5DX = pZoneColor5.ToDxBrush(RenderTarget);
				cachedZoneColor5DX.Opacity = pZoneOpacity5 / 100f;

				// Chart brushes
				if (ChartControl != null)
				{
					cachedChartTextBrushDX = ChartControl.Properties.ChartText.ToDxBrush(RenderTarget);
					cachedChartBackgroundBrushDX = ChartControl.Properties.ChartBackground.ToDxBrush(RenderTarget);
					cachedLabelBrushDX = ChartControl.Properties.ChartText.ToDxBrush(RenderTarget);

					// Button brushes
					cachedButtonBrushDX = GetTextColor(ChartControl.Properties.ChartBackground).ToDxBrush(RenderTarget);
					cachedButtonHBrushDX = GetTextColor(ChartControl.Properties.ChartBackground).ToDxBrush(RenderTarget);
					cachedButtonHBrushDX.Opacity = 0.4f;
					cachedButtonFHBrushDX = ChartControl.Properties.AxisPen.Brush.ToDxBrush(RenderTarget);
					cachedButtonFHBrushDX.Opacity = 0.0f;
					cachedButtonFOFFBrushDX = ChartControl.Properties.ChartBackground.ToDxBrush(RenderTarget);
					cachedButtonFH2BrushDX = ChartControl.Properties.AxisPen.Brush.ToDxBrush(RenderTarget);

					// Panel system brushes
					cachedPanelBackdropBrushDX = ChartControl.Properties.ChartBackground.ToDxBrush(RenderTarget);
					cachedPanelBackdropBrushDX.Opacity = 1.0f;
					cachedHeaderTextBrushDX = GetTextColor(ChartControl.Properties.ChartBackground).ToDxBrush(RenderTarget);
					cachedHeaderTextBrushDX.Opacity = 0.7f;
					{
						Color bgC = ((SolidColorBrush)ChartControl.Properties.ChartBackground).Color;
						Color fgC = ((SolidColorBrush)GetTextColor(ChartControl.Properties.ChartBackground)).Color;
						byte nr = (byte)(bgC.R + (fgC.R - bgC.R) * 0.07);
						byte ng = (byte)(bgC.G + (fgC.G - bgC.G) * 0.07);
						byte nb = (byte)(bgC.B + (fgC.B - bgC.B) * 0.07);
						cachedHeaderBgBrushDX = new SharpDX.Direct2D1.SolidColorBrush(RenderTarget, new SharpDX.Color(nr, ng, nb, (byte)255));
						byte hr = (byte)(bgC.R + (fgC.R - bgC.R) * 0.16);
						byte hg = (byte)(bgC.G + (fgC.G - bgC.G) * 0.16);
						byte hb = (byte)(bgC.B + (fgC.B - bgC.B) * 0.16);
						cachedHeaderBgHoverBrushDX = new SharpDX.Direct2D1.SolidColorBrush(RenderTarget, new SharpDX.Color(hr, hg, hb, (byte)255));
					}
					cachedHoverGlowBrushDX = GetTextColor(ChartControl.Properties.ChartBackground).ToDxBrush(RenderTarget);
					cachedHoverGlowBrushDX.Opacity = 0.08f;
				}

				if (pZoneColor1 != null)
					cachedButtonFONBrushDX = pZoneColor1.ToDxBrush(RenderTarget);

				if (areaBrush != null)
					cachedAreaBrushDX = areaBrush.ToDxBrush(RenderTarget);

				// Recreate text formats
				cachedLabelTextFormat = pTextFont.ToDirectWriteTextFormat();
				cachedLabelTextFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
				cachedLabelTextFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
				cachedLabelTextFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;

				if (ChartControl != null)
				{
					var btnFont = new NinjaTrader.Gui.Tools.SimpleFont(ChartControl.Properties.LabelFont.Family.ToString(), (float)Math.Max(11, ChartControl.Properties.LabelFont.Size + pChartMenuTextSize));
					cachedButtonTextFormat = btnFont.ToDirectWriteTextFormat();
					cachedButtonTextFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
					cachedButtonTextFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
					cachedButtonTextFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;

					var headerFont = new NinjaTrader.Gui.Tools.SimpleFont("Arial", 11) { Bold = true };
					cachedHeaderTextFormat = headerFont.ToDirectWriteTextFormat();
					cachedHeaderTextFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Leading;
					cachedHeaderTextFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
					cachedHeaderTextFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;
				}

				// Clear text measure cache on render target change
				textMeasureCache.Clear();

				// Create cached stroke styles from user-selected freshness border properties
				cachedStrokeUntested = new SharpDX.Direct2D1.StrokeStyle(Core.Globals.D2DFactory, new SharpDX.Direct2D1.StrokeStyleProperties { DashStyle = DashStyleHelperToD2D(pUntestedStyle) });
				cachedStrokeTested = new SharpDX.Direct2D1.StrokeStyle(Core.Globals.D2DFactory, new SharpDX.Direct2D1.StrokeStyleProperties { DashStyle = DashStyleHelperToD2D(pTestedStyle) });
				cachedStrokeBroken = new SharpDX.Direct2D1.StrokeStyle(Core.Globals.D2DFactory, new SharpDX.Direct2D1.StrokeStyleProperties { DashStyle = DashStyleHelperToD2D(pBrokenStyle) });

				// Error display brush + text format (cached instead of per-frame alloc)
				cachedErrorBrushDX = new SharpDX.Direct2D1.SolidColorBrush(RenderTarget, SharpDX.Color.Red);
				cachedErrorBrushDX.Opacity = 25 / 100f;
				if (ChartControl != null)
				{
					cachedErrorTextFormat = new SimpleFont(ChartControl.Properties.LabelFont.Family.ToString(), 16).ToDirectWriteTextFormat();
					cachedErrorTextFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
					cachedErrorTextFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
					cachedErrorTextFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.Wrap;
				}
			}
		}
		
				
		private string GetDataName(string inp)
		{
			
			string dataname = inp;
		
			if (!dataname.Contains("Daily"))
			{
				dataname = dataname.Replace("a", "");
				dataname = dataname.Replace("b", "");
				dataname = dataname.Replace("c", "");
			}
				
			return dataname;
			
		}
		
		private SharpDX.Direct2D1.DashStyle DashStyleHelperToD2D(DashStyleHelper helper)
		{
			switch (helper)
			{
				case DashStyleHelper.Dash: return SharpDX.Direct2D1.DashStyle.Dash;
				case DashStyleHelper.DashDot: return SharpDX.Direct2D1.DashStyle.DashDot;
				case DashStyleHelper.DashDotDot: return SharpDX.Direct2D1.DashStyle.DashDotDot;
				case DashStyleHelper.Dot: return SharpDX.Direct2D1.DashStyle.Dot;
				default: return SharpDX.Direct2D1.DashStyle.Solid;
			}
		}

		public Brush GetTextColor(Brush bg2)
		{
			
			//Color bg = new Pen(bg2,1).;
			Color bg = (bg2 as SolidColorBrush).Color;
			
			
			double a = 1 - ( 0.299 * bg.R + 0.587 * bg.G + 0.114 * bg.B)/255;
            if (a < 0.5)
               return Brushes.Black;
            else
               return Brushes.WhiteSmoke;
			

//		    Brush foreColor = (255 - bgDelta < nThreshold) ? Brushes.Black : Brushes.White;
//		    return foreColor;
		}


		private SharpDX.RectangleF ExpandRect (SharpDX.RectangleF RR, float left, float right, float top, float bottom)
		{
			
			SharpDX.RectangleF FF = new SharpDX.RectangleF(RR.X-left, RR.Y-top, RR.Width+left+right, RR.Height+top+bottom);
				
			return FF;
			
		}
		
		private SharpDX.RectangleF ExpandRect (SharpDX.RectangleF RR, float xe, float ye)
		{
			
			SharpDX.RectangleF FF = new SharpDX.RectangleF(RR.X-xe, RR.Y-ye, RR.Width+xe*2, RR.Height+ye*2);
				
			return FF;
			
		}
						
		
			private bool FirstRender2 = true;

		private ChartControlProperties myProperties;

		private bool PreviousDrag = false;

		SharpDX.DirectWrite.TextFormat CenterText;
		SharpDX.RectangleF CenterRect;

		// --- Cached DX brushes (recreated in OnRenderTargetChanged) ---
		private SharpDX.Direct2D1.Brush cachedZoneColor1DX;
		private SharpDX.Direct2D1.Brush cachedZoneColor2DX;
		private SharpDX.Direct2D1.Brush cachedZoneColor3DX;
		private SharpDX.Direct2D1.Brush cachedZoneColor4DX;
		private SharpDX.Direct2D1.Brush cachedZoneColor5DX;
		private SharpDX.Direct2D1.Brush cachedChartTextBrushDX;
		private SharpDX.Direct2D1.Brush cachedChartBackgroundBrushDX;
		private SharpDX.Direct2D1.Brush cachedLabelBrushDX;
		private SharpDX.Direct2D1.Brush cachedButtonBrushDX;
		private SharpDX.Direct2D1.Brush cachedButtonHBrushDX;
		private SharpDX.Direct2D1.Brush cachedButtonFHBrushDX;
		private SharpDX.Direct2D1.Brush cachedButtonFOFFBrushDX;
		private SharpDX.Direct2D1.Brush cachedButtonFONBrushDX;
		private SharpDX.Direct2D1.Brush cachedButtonFH2BrushDX;
		private SharpDX.Direct2D1.Brush cachedAreaBrushDX;

		// --- Panel system brushes ---
		private SharpDX.Direct2D1.Brush cachedPanelBackdropBrushDX = null;
		private SharpDX.Direct2D1.Brush cachedHeaderBgBrushDX = null;
		private SharpDX.Direct2D1.Brush cachedHeaderBgHoverBrushDX = null;
		private SharpDX.Direct2D1.Brush cachedHeaderTextBrushDX = null;
		private SharpDX.Direct2D1.Brush cachedHoverGlowBrushDX = null;
		private SharpDX.Direct2D1.Brush cachedErrorBrushDX = null;

		// --- Cached TextFormat (recreated in OnRenderTargetChanged) ---
		private SharpDX.DirectWrite.TextFormat cachedLabelTextFormat;
		private SharpDX.DirectWrite.TextFormat cachedButtonTextFormat;
		private SharpDX.DirectWrite.TextFormat cachedHeaderTextFormat;
		private SharpDX.DirectWrite.TextFormat cachedErrorTextFormat;

		// --- Cached StrokeStyles for zone freshness borders ---
		private SharpDX.Direct2D1.StrokeStyle cachedStrokeUntested;
		private SharpDX.Direct2D1.StrokeStyle cachedStrokeTested;
		private SharpDX.Direct2D1.StrokeStyle cachedStrokeBroken;

		// --- Reusable TextLayout (disposed/recreated per use) ---
		private SharpDX.DirectWrite.TextLayout reusableTextLayout;

		// --- Reusable rectangle structs ---
		private SharpDX.RectangleF reusableTextRect;
		private SharpDX.RectangleF reusableZoneRect;

		// --- Text measure cache ---
		private Dictionary<string, SharpDX.Size2F> textMeasureCache = new Dictionary<string, SharpDX.Size2F>();

		private SharpDX.Size2F MeasureText(string text, SharpDX.DirectWrite.TextFormat format)
		{
			SharpDX.Size2F size;
			if (textMeasureCache.TryGetValue(text, out size))
				return size;
			using (var layout = new TextLayout(Core.Globals.DirectWriteFactory, text, format, 10000, 10000))
			{
				size = new SharpDX.Size2F(layout.Metrics.Width, layout.Metrics.Height);
			}
			textMeasureCache[text] = size;
			return size;
		}

		#region Shared Button Panel Coordinator

		private int panelHash = 0;
		private const string PANEL_REGISTRY_KEY = "aiButtonPanelRegistry_v2";
		private const string SCROLL_REGISTRY_KEY = "aiButtonPanelScroll_v2";
		private const string PANEL_ID = "PriceAction";
		private const int PANEL_PRIORITY = 200;
		private const string PANEL_HEADER_TEXT = "PRICE ACTION";
		private const float PANEL_TOP_MARGIN = 30f;
		private const float PANEL_GROUP_GAP = 10f;
		private const float chartButtonRadius = 4f;
		private bool panelExpanded = false;
		private bool? _xmlExpandState = null;

		private float panelAnimProgress = 0f;
		private DateTime panelAnimStartTime = DateTime.MinValue;
		private float panelAnimStartValue = 0f;
		private float panelAnimTargetValue = 0f;
		private const float PANEL_ANIM_DURATION_MS = 200f;
		private SharpDX.RectangleF headerRect = new SharpDX.RectangleF(0, 0, 0, 0);
		private SharpDX.RectangleF lastPanelRect = new SharpDX.RectangleF(0, 0, 0, 0);
		private bool panelWasVisible = false;
		private int currentButtonHover = -1;

		// Slot array indices — object[] avoids private-class type mismatch across indicators
		private const int SL_PRIORITY = 0;
		private const int SL_LASTHEIGHT = 1;
		private const int SL_MENUOPEN = 2;
		private const int SL_EXPANDED = 3;
		private const int SL_BOTTOMUP = 4;
		private const int SL_USERPRI = 5;
		private const int SL_HEADERY = 6;
		private const int SL_HEADERH = 7;
		private const int SL_PANELW = 8;   // last rendered panel width
		private const int SL_SLOT_SIZE = 9;

		private string draggingPanelId = null;
		private float dragStartY = 0;
		private float dragOffsetY = 0;

		// Scroll array indices — float[]
		private const int SC_OFFSET = 0;
		private const int SC_TOTALH = 1;
		private const int SC_VIEWH = 2;

		private static ConcurrentDictionary<int, ConcurrentDictionary<string, object[]>> cachedPanelRegistry;
		private static ConcurrentDictionary<int, float[]> cachedScrollRegistry;

		private static ConcurrentDictionary<int, ConcurrentDictionary<string, object[]>> GetPanelRegistry()
		{
			var reg = cachedPanelRegistry;
			if (reg != null) return reg;
			lock (typeof(NinjaTrader.NinjaScript.IndicatorBase))
			{
				var existing = AppDomain.CurrentDomain.GetData(PANEL_REGISTRY_KEY)
					as ConcurrentDictionary<int, ConcurrentDictionary<string, object[]>>;
				if (existing != null) { cachedPanelRegistry = existing; return existing; }
				var registry = new ConcurrentDictionary<int, ConcurrentDictionary<string, object[]>>();
				AppDomain.CurrentDomain.SetData(PANEL_REGISTRY_KEY, registry);
				cachedPanelRegistry = registry;
				return registry;
			}
		}

		private static ConcurrentDictionary<int, float[]> GetScrollRegistry()
		{
			var reg = cachedScrollRegistry;
			if (reg != null) return reg;
			lock (typeof(NinjaTrader.NinjaScript.IndicatorBase))
			{
				var existing = AppDomain.CurrentDomain.GetData(SCROLL_REGISTRY_KEY)
					as ConcurrentDictionary<int, float[]>;
				if (existing != null) { cachedScrollRegistry = existing; return existing; }
				var registry = new ConcurrentDictionary<int, float[]>();
				AppDomain.CurrentDomain.SetData(SCROLL_REGISTRY_KEY, registry);
				cachedScrollRegistry = registry;
				return registry;
			}
		}

		private const string HOVER_REGISTRY_KEY = "aiButtonPanelHoverRegistry_v1";
		private static ConcurrentDictionary<int, ConcurrentDictionary<string, bool>> cachedHoverRegistry;
		private static ConcurrentDictionary<int, ConcurrentDictionary<string, bool>> GetHoverRegistry()
		{
			var reg = cachedHoverRegistry;
			if (reg != null) return reg;
			lock (typeof(NinjaTrader.NinjaScript.IndicatorBase))
			{
				var existing = AppDomain.CurrentDomain.GetData(HOVER_REGISTRY_KEY)
					as ConcurrentDictionary<int, ConcurrentDictionary<string, bool>>;
				if (existing != null) { cachedHoverRegistry = existing; return existing; }
				var registry = new ConcurrentDictionary<int, ConcurrentDictionary<string, bool>>();
				AppDomain.CurrentDomain.SetData(HOVER_REGISTRY_KEY, registry);
				cachedHoverRegistry = registry;
				return registry;
			}
		}
		private bool _hoverClickableThisFrame = false;
		private System.Windows.Input.Cursor _lastAppliedChartCursor = null;
		private void ApplyHoverCursor()
		{
			if (ChartControl == null) return;
			try
			{
				var reg = GetHoverRegistry();
				var dict = reg.GetOrAdd(panelHash, k => new ConcurrentDictionary<string, bool>());
				dict[PANEL_ID] = _hoverClickableThisFrame;
				bool any = false;
				foreach (var kvp in dict) if (kvp.Value) { any = true; break; }
				var want = any ? System.Windows.Input.Cursors.Hand : System.Windows.Input.Cursors.Arrow;
				if (want == _lastAppliedChartCursor) return;
				_lastAppliedChartCursor = want;
				ChartControl.Cursor = want;
			}
			catch { }
		}

		private float GetCurrentScrollOffset()
		{
			var scrollReg = GetScrollRegistry();
			float[] state;
			if (scrollReg.TryGetValue(panelHash, out state))
				return state[SC_OFFSET];
			return 0f;
		}

		private static string PanelOrderFilePath
		{
			get { return System.IO.Path.Combine(NinjaTrader.Core.Globals.UserDataDir, "aiPanelOrder.txt"); }
		}

		// File line format: "chartHash|panelId|priority|expanded"  (expanded optional for back-compat)
		private static int LoadPanelPriority(int chartHash, string panelId, int defaultPri)
		{
			try
			{
				if (!System.IO.File.Exists(PanelOrderFilePath)) return defaultPri;
				string prefix = chartHash + "|" + panelId + "|";
				foreach (string line in System.IO.File.ReadAllLines(PanelOrderFilePath))
				{
					if (line.StartsWith(prefix))
					{
						string rest = line.Substring(prefix.Length);
						int bar = rest.IndexOf('|');
						string priStr = bar >= 0 ? rest.Substring(0, bar) : rest;
						int val;
						if (int.TryParse(priStr, out val))
							return val;
					}
				}
			}
			catch { }
			return defaultPri;
		}

		private static bool LoadPanelExpanded(int chartHash, string panelId, bool defaultExpanded)
		{
			try
			{
				if (!System.IO.File.Exists(PanelOrderFilePath)) return defaultExpanded;
				string prefix = chartHash + "|" + panelId + "|";
				foreach (string line in System.IO.File.ReadAllLines(PanelOrderFilePath))
				{
					if (line.StartsWith(prefix))
					{
						string rest = line.Substring(prefix.Length);
						int bar = rest.IndexOf('|');
						if (bar < 0) return defaultExpanded;
						string expStr = rest.Substring(bar + 1);
						bool val;
						if (bool.TryParse(expStr, out val))
							return val;
					}
				}
			}
			catch { }
			return defaultExpanded;
		}

		private static void SavePanelPriorities(int chartHash, ConcurrentDictionary<string, object[]> panels)
		{
			try
			{
				// Merge: preserve every existing line whose key is NOT in panels dict
				// (prevents wiping sibling panels that haven't registered yet).
				var linesByKey = new Dictionary<string, string>();
				if (System.IO.File.Exists(PanelOrderFilePath))
				{
					foreach (string line in System.IO.File.ReadAllLines(PanelOrderFilePath))
					{
						int firstBar = line.IndexOf('|');
						if (firstBar < 0) continue;
						int secondBar = line.IndexOf('|', firstBar + 1);
						if (secondBar < 0) continue;
						string key = line.Substring(0, secondBar);
						linesByKey[key] = line;
					}
				}
				foreach (var kvp in panels)
				{
					if (kvp.Value.Length >= SL_SLOT_SIZE)
					{
						string key = chartHash + "|" + kvp.Key;
						bool exp = (bool)kvp.Value[SL_EXPANDED];
						linesByKey[key] = key + "|" + (int)kvp.Value[SL_USERPRI] + "|" + exp;
					}
				}
				System.IO.File.WriteAllLines(PanelOrderFilePath, linesByKey.Values.ToArray());
			}
			catch { }
		}

		private void RegisterPanel()
		{
			if (ChartPanel == null) return;
			panelHash = ChartPanel.GetHashCode();
			var registry = GetPanelRegistry();
			var panels = registry.GetOrAdd(panelHash, k => new ConcurrentDictionary<string, object[]>());
			int savedPri = LoadPanelPriority(panelHash, PANEL_ID, PANEL_PRIORITY);
			// Hybrid: file persists toggles across NinjaScript reload; XML (PanelExpandedState)
			// persists template/workspace saves. If both have a value AND they disagree,
			// XML wins (template override).
			bool fileExp = LoadPanelExpanded(panelHash, PANEL_ID, panelExpanded);
			bool useExp = (_xmlExpandState.HasValue && _xmlExpandState.Value != fileExp)
				? _xmlExpandState.Value
				: fileExp;
			panelExpanded = useExp;
			panels[PANEL_ID] = new object[] { PANEL_PRIORITY, 0f, false, useExp, false, savedPri, 0f, 0f, 0f };
			SetPanelExpanded(panelExpanded);
		}

		private void UnregisterPanel()
		{
			if (panelHash == 0) return;
			var registry = GetPanelRegistry();
			ConcurrentDictionary<string, object[]> panels;
			if (registry.TryGetValue(panelHash, out panels))
			{
				object[] removed;
				panels.TryRemove(PANEL_ID, out removed);
				if (panels.Count == 0)
				{
					ConcurrentDictionary<string, object[]> removedPanels;
					registry.TryRemove(panelHash, out removedPanels);
				}
			}
			var hoverReg = GetHoverRegistry();
			ConcurrentDictionary<string, bool> hoverDict;
			if (hoverReg.TryGetValue(panelHash, out hoverDict))
			{
				bool removedH;
				hoverDict.TryRemove(PANEL_ID, out removedH);
				if (hoverDict.Count == 0)
				{
					ConcurrentDictionary<string, bool> removedHD;
					hoverReg.TryRemove(panelHash, out removedHD);
				}
			}
		}

		private int GetUserPriority(object[] s)
		{
			return s.Length >= SL_SLOT_SIZE ? (int)s[SL_USERPRI] : (int)s[SL_PRIORITY];
		}

		private int GetMyUserPriority()
		{
			var registry = GetPanelRegistry();
			ConcurrentDictionary<string, object[]> panels;
			if (!registry.TryGetValue(panelHash, out panels)) return PANEL_PRIORITY;
			object[] slot;
			if (panels.TryGetValue(PANEL_ID, out slot))
				return GetUserPriority(slot);
			return PANEL_PRIORITY;
		}

		private float GetPanelYOffset()
		{
			var registry = GetPanelRegistry();
			ConcurrentDictionary<string, object[]> panels;
			if (!registry.TryGetValue(panelHash, out panels))
				return PANEL_TOP_MARGIN;

			int myPri = GetMyUserPriority();
			float y = PANEL_TOP_MARGIN - GetCurrentScrollOffset();
			foreach (var kvp in panels)
			{
				if (kvp.Key == PANEL_ID) continue;
				var s = kvp.Value;
				if ((bool)s[SL_BOTTOMUP]) continue;
				int otherPri = GetUserPriority(s);
				// Tie-break equal priorities by panel ID so duplicates don't stack at same Y.
				bool before = otherPri < myPri
					|| (otherPri == myPri && string.CompareOrdinal(kvp.Key, PANEL_ID) < 0);
				if (before)
					y += (float)s[SL_LASTHEIGHT] + PANEL_GROUP_GAP;
			}
			return y;
		}

		private void ReportPanelHeight(float height)
		{
			var registry = GetPanelRegistry();
			ConcurrentDictionary<string, object[]> panels;
			if (registry.TryGetValue(panelHash, out panels))
			{
				object[] slot;
				if (panels.TryGetValue(PANEL_ID, out slot))
				{
					slot[SL_LASTHEIGHT] = height;
					if (slot.Length >= SL_SLOT_SIZE)
					{
						slot[SL_HEADERY] = headerRect.Y;
						slot[SL_HEADERH] = headerRect.Height;
					}
				}
			}
		}

		private float GetMaxPanelWidth()
		{
			var registry = GetPanelRegistry();
			ConcurrentDictionary<string, object[]> panels;
			if (!registry.TryGetValue(panelHash, out panels)) return 0;
			float maxW = 0;
			foreach (var kvp in panels)
			{
				if (kvp.Value.Length >= SL_SLOT_SIZE)
				{
					float w = (float)kvp.Value[SL_PANELW];
					if (w > maxW) maxW = w;
				}
			}
			return maxW;
		}

		// Detects a left Y-axis on the chart panel and returns its width.
		private float ComputePanelLeftXForClip(ChartControl chartControl)
		{
			try
			{
				if (ChartPanel == null) return 0f;
				// When a left Y-axis is present, ChartPanel.X equals its width.
				float left = (float)ChartPanel.X;
				if (left < 0f) return 0f;
				if (left > ChartPanel.W * 0.25f) return 0f;
				return left;
			}
			catch { }
			return 0f;
		}

		private void SwapPanelOrder(string draggedId, string targetId)
		{
			var registry = GetPanelRegistry();
			ConcurrentDictionary<string, object[]> panels;
			if (!registry.TryGetValue(panelHash, out panels)) return;
			object[] dragSlot, targetSlot;
			if (!panels.TryGetValue(draggedId, out dragSlot) || dragSlot.Length < SL_SLOT_SIZE) return;
			if (!panels.TryGetValue(targetId, out targetSlot) || targetSlot.Length < SL_SLOT_SIZE) return;
			int tmp = (int)dragSlot[SL_USERPRI];
			dragSlot[SL_USERPRI] = targetSlot[SL_USERPRI];
			targetSlot[SL_USERPRI] = tmp;
			SavePanelPriorities(panelHash, panels);
		}

		private void UpdateScrollBounds()
		{
			if (ChartPanel == null) return;
			var registry = GetPanelRegistry();
			ConcurrentDictionary<string, object[]> panels;
			if (!registry.TryGetValue(panelHash, out panels)) return;

			float totalHeight = PANEL_TOP_MARGIN;
			bool anyExpanded = false;
			foreach (var kvp in panels)
			{
				var s = kvp.Value;
				if (!(bool)s[SL_BOTTOMUP])
					totalHeight += (float)s[SL_LASTHEIGHT] + PANEL_GROUP_GAP;
				if ((bool)s[SL_EXPANDED]) anyExpanded = true;
			}

			float viewportH = ChartPanel.H;
			var scrollReg = GetScrollRegistry();
			var state = scrollReg.GetOrAdd(panelHash, k => new float[3]);
			state[SC_TOTALH] = totalHeight;
			state[SC_VIEWH] = viewportH;

			if (!anyExpanded)
				state[SC_OFFSET] = 0;

			float maxScroll = Math.Max(0, totalHeight - viewportH + 20f);
			state[SC_OFFSET] = Math.Max(0, Math.Min(state[SC_OFFSET], maxScroll));
		}

		private void SetPanelExpanded(bool expanded)
		{
			panelExpanded = expanded;
			var registry = GetPanelRegistry();
			ConcurrentDictionary<string, object[]> panels;
			if (!registry.TryGetValue(panelHash, out panels)) return;
			object[] slot;
			if (panels.TryGetValue(PANEL_ID, out slot))
				slot[SL_EXPANDED] = expanded;
			SavePanelPriorities(panelHash, panels);
		}

		private bool IsPanelExpanded()
		{
			var registry = GetPanelRegistry();
			ConcurrentDictionary<string, object[]> panels;
			if (!registry.TryGetValue(panelHash, out panels)) return panelExpanded;
			object[] slot;
			if (panels.TryGetValue(PANEL_ID, out slot))
			{
				panelExpanded = (bool)slot[SL_EXPANDED];
				return panelExpanded;
			}
			return panelExpanded;
		}

		private void SetPanelMenuOpen(bool open)
		{
			var registry = GetPanelRegistry();
			ConcurrentDictionary<string, object[]> panels;
			if (!registry.TryGetValue(panelHash, out panels)) return;
			object[] slot;
			if (panels.TryGetValue(PANEL_ID, out slot))
				slot[SL_MENUOPEN] = open;
		}

		private bool IsAnyPanelMenuOpen()
		{
			var registry = GetPanelRegistry();
			ConcurrentDictionary<string, object[]> panels;
			if (!registry.TryGetValue(panelHash, out panels)) return InMenu;
			foreach (var kvp in panels)
				if ((bool)kvp.Value[SL_MENUOPEN]) return true;
			return false;
		}

		private void FillChartButton(SharpDX.RectangleF rect, SharpDX.Direct2D1.Brush brush)
		{
			var saved = RenderTarget.AntialiasMode;
			RenderTarget.AntialiasMode = SharpDX.Direct2D1.AntialiasMode.PerPrimitive;
			RenderTarget.FillRoundedRectangle(new SharpDX.Direct2D1.RoundedRectangle() { Rect = rect, RadiusX = chartButtonRadius, RadiusY = chartButtonRadius }, brush);
			RenderTarget.AntialiasMode = saved;
		}

		private const float headerBottomStripH = 4f;

		private void FillHeaderButton(SharpDX.RectangleF rect, SharpDX.Direct2D1.Brush brush)
		{
			float r = chartButtonRadius + 2;
			SharpDX.RectangleF extended = new SharpDX.RectangleF(rect.Left, rect.Top, rect.Width, rect.Height + r);
			var saved = RenderTarget.AntialiasMode;
			RenderTarget.AntialiasMode = SharpDX.Direct2D1.AntialiasMode.PerPrimitive;
			RenderTarget.FillRoundedRectangle(new SharpDX.Direct2D1.RoundedRectangle() { Rect = extended, RadiusX = r, RadiusY = r }, brush);
			RenderTarget.FillRectangle(new SharpDX.RectangleF(rect.Left, rect.Top, r, r), brush);
			RenderTarget.AntialiasMode = saved;
		}

		private void DrawChartButton(SharpDX.RectangleF rect, SharpDX.Direct2D1.Brush brush, float strokeWidth = 1)
		{
			var saved = RenderTarget.AntialiasMode;
			RenderTarget.AntialiasMode = SharpDX.Direct2D1.AntialiasMode.PerPrimitive;
			RenderTarget.DrawRoundedRectangle(new SharpDX.Direct2D1.RoundedRectangle() { Rect = rect, RadiusX = chartButtonRadius, RadiusY = chartButtonRadius }, brush, strokeWidth);
			RenderTarget.AntialiasMode = saved;
		}

		#endregion

		private void ChartBarsSwitch2(bool onoff)
		{
			if (ChartPanel == null)
			return;
			
			
			if (onoff)
			{
				ChartBars.Properties.ChartStyle.DownBrushDX.Opacity = 1;
				ChartBars.Properties.ChartStyle.UpBrushDX.Opacity = 1;
				ChartBars.Properties.ChartStyle.Stroke.BrushDX.Opacity = 1; // outline
				ChartBars.Properties.ChartStyle.Stroke2.BrushDX.Opacity = 1; // wick
			}
			else
			{
				ChartBars.Properties.ChartStyle.DownBrushDX.Opacity = 0;
				ChartBars.Properties.ChartStyle.UpBrushDX.Opacity = 0;				
				ChartBars.Properties.ChartStyle.Stroke.BrushDX.Opacity = 0; // outline
				ChartBars.Properties.ChartStyle.Stroke2.BrushDX.Opacity = 0; // wick
				
			}
			
			//ChartBars.Properties.AutoScale = onoff;
			
		}
		
		SharpDX.Direct2D1.AntialiasMode oldAntialiasMode;
		
        private SharpDX.Direct2D1.Brush ChartTextBrushDX = null;
		private SharpDX.Direct2D1.Brush ChartBackgroundBrushDX = null;
		// ChartBackgroundErrorBrushDX removed — now using cachedErrorBrushDX
		private SharpDX.Direct2D1.Brush ThisBrushDX = null;
	
		
		private Brush PlotBrush = null;
		
		
		private string GetDateString(DateTime nsafg)
		{
			
			
											string xxssf = nsafg.ToString("ddd  H:mm");
											
											//if (TotalDaysBack >=7)
												xxssf = nsafg.ToString("M/d  H:mm");
							
											return xxssf;
											
		}
				
		
		private double PreviousMaxValue = 0;
		private double PreviousMinValue = 0;
				
		public override void OnCalculateMinMax()
		{

			//Print(MaxValue);
			//Print(MinValue);
			
			
		}
		

		private int VisibleBarsStart = -1;
		private int VisibleBarsEnd = -1;
		
		private double VisiblePriceHigh = -1;
		private double VisiblePriceLow = -1;
		
		private bool RedoMarkers = true;
		
		protected override void OnRender(ChartControl chartControl, ChartScale chartScale)
		{
			_hoverClickableThisFrame = false;
			// Lazy-register if DataLoaded ran before ChartPanel was ready.
			if (panelHash == 0 && ChartPanel != null)
				RegisterPanel();
			// First-add recovery: on first add, OnRenderTargetChanged fires before ChartControl
			// is set up. Chart-dependent brushes (cachedChartTextBrushDX etc.) never get created,
			// causing the early-return below to fire on every render until click. Detect that state
			// and re-run brush setup now that ChartControl is available.
			if (cachedChartTextBrushDX == null && ChartControl != null && RenderTarget != null)
			{
				OnRenderTargetChanged();
			}

			if (cachedChartTextBrushDX == null || cachedLabelTextFormat == null)
				return;

			// Clip chart content away from panel zone
			// Panel-zone clip removed — see ECT note. Panel backdrop covers content.
			bool panelClipActive = false;

			oldAntialiasMode = RenderTarget.AntialiasMode;

			if (FirstRender2)
			{
				myProperties = chartControl.Properties;

				// First-add race condition fallback: if OnStateChange(Realtime) hasn't
				// run the deferred calc yet (can happen on first add to chart), do it here
				// so the very first paint has zones. Safe because deferredZonesCalculated
				// prevents double-calc.
				if (!deferredZonesCalculated && State != State.Historical && (AllHitsChart.Count > 0 || AllHits.Count > 0))
				{
					deferredZonesCalculated = true;
					calczones = true;
					try
					{
						CalculateZones();
						ExportZonesToCsv();
					}
					catch { }
				}

				FirstRender2 = false;

				// On first add, ChartBars may not be fully initialized when first OnRender
				// fires. Start a one-shot DispatcherTimer (fires on UI thread after a short
				// delay) to re-invalidate the chart — mimicking what a mouse click does.
				try
				{
					if (chartControl != null && chartControl.Dispatcher != null)
					{
						var refreshTimer = new System.Windows.Threading.DispatcherTimer(
							System.Windows.Threading.DispatcherPriority.Render,
							chartControl.Dispatcher);
						refreshTimer.Interval = TimeSpan.FromMilliseconds(50);
						refreshTimer.Tick += (s, args) =>
						{
							try
							{
								refreshTimer.Stop();
								if (chartControl != null)
									chartControl.InvalidateVisual();
							}
							catch { }
						};
						refreshTimer.Start();
					}
				}
				catch { }
			}

			if (!IsInHitTest)
			if (AllErrorMessages.Count > 0 && cachedErrorBrushDX != null && cachedErrorTextFormat != null)
			{
				CenterRect = new SharpDX.RectangleF(ChartPanel.X, ChartPanel.Y, ChartPanel.W, ChartPanel.H);

				RenderTarget.AntialiasMode = SharpDX.Direct2D1.AntialiasMode.Aliased;

				string txt = string.Empty;
				foreach (string sss in AllErrorMessages)
					txt = txt + sss + "\r\n\r\n";
				txt = txt + "Click here to continue.";

				RenderTarget.FillRectangle(CenterRect, cachedChartBackgroundBrushDX);
				RenderTarget.FillRectangle(CenterRect, cachedErrorBrushDX);
				RenderTarget.DrawText(txt, cachedErrorTextFormat, ExpandRect(CenterRect, -10, 0), cachedChartTextBrushDX);

				RenderTarget.AntialiasMode = oldAntialiasMode;

				return;
			}

			if (!Permission)
				return;

			if (!IsVisible)
				return;

			if (Bars == null || Bars.Instrument == null || IsInHitTest)
				return;

			// Use cached brushes and formats from OnRenderTargetChanged
			SharpDX.Direct2D1.Brush LabelBrushDX = cachedLabelBrushDX;
			SharpDX.Direct2D1.Brush buttonFONBrushDX = cachedButtonFONBrushDX;

			RenderTarget.AntialiasMode = oldAntialiasMode;

			DateTime frameNow = DateTime.Now;

			float y = 0;
			float x = 0;
			float x1 = 0;
			float x2 = 0;
			float x3 = 0;
			float x4 = 0;
			float y1 = 0;
			float y2 = 0;
			float y3 = 0;
			double vtextadjust = 0;

			if (dpiX == 0)
			{
				PresentationSource source = PresentationSource.FromVisual(this.ChartPanel);
				if (source != null)
				{
					dpiX = 100.0 * source.CompositionTarget.TransformToDevice.M11;
					dpiY = 100.0 * source.CompositionTarget.TransformToDevice.M22;
				}
			}
			
			FinalXPixel = MP.X / 100 * dpiX;
			FinalYPixel = MP.Y / 100 * dpiY;
			
						
           CurrentMousePrice = chartScale.MaxValue - chartScale.MaxMinusMin * (MP.Y / chartScale.Height);

			CurrentMousePrice = RTTS(CurrentMousePrice);

            
            double mousebar = (ChartControl.GetXByBarIndex(ChartBars, ChartBars.ToIndex) - ChartControl.GetXByBarIndex(ChartBars, ChartBars.FromIndex)) / Math.Max(1,(ChartBars.ToIndex - ChartBars.FromIndex)); //chartControl.GetBarPaintWidth(ChartBars);

            double mousebar2 = ChartBars.FromIndex + (MP.X - ChartControl.GetXByBarIndex(ChartBars, ChartBars.FromIndex)) / mousebar;

            int mousebar3 = (int) Math.Round(mousebar2, 0);
						
						
			AllRefreshRects.Clear();
			
			int RMB = ChartBars.ToIndex;
			int FB2 = ChartBars.FromIndex;
			
				
			VisibleBarsStart = ChartBars.FromIndex;
			VisibleBarsEnd = ChartBars.ToIndex;			
				
					
			// draw hits that haven't been yet included in a zone		
						 
						
				if (pLatestDataAllEnabled)	
				if (pShowID)
				for (int i=0; i < AllHits.Count; i++)			
				{
			
					double HitPrice = AllHits[i].ThisPrice;
					string HitName = AllHits[i].ThisName;
					string HitData = GetDataName(AllHits[i].ThisData);

					if (DailyDataName == HitData)
						continue;

					if (WeeklyDataName == HitData)
						continue;

					if (!pLatestData3Enabled)
					if (Data3NameFinal == HitData)
						continue;

					if (!pLatestData2Enabled)
					if (Data2NameFinal == HitData)
						continue;

					if (!pLatestData1Enabled)
					if (Data1NameFinal == HitData)
						continue;

					// Expensive call — deferred past data name filters
					int HitStartBar = (int) ChartControl.GetSlotIndexByTime(AllHits[i].ThisTime);

					if (HitStartBar > VisibleBarsEnd)
						continue;

					if (VisiblePriceHigh != -1)
					if (HitPrice > VisiblePriceHigh || HitPrice < VisiblePriceLow)
						continue;

					string lb = AllHits[i].ThisTime.ToString() + "    " + HitData + " " + HitName;
					
				
					x1 = chartControl.GetXByBarIndex(ChartBars, HitStartBar);
					//x2 = chartControl.GetXByBarIndex(ChartBars, 10000);
					x2 = 10000;
					y1 = chartScale.GetYByValue(HitPrice);
					y2 = chartScale.GetYByValue(HitPrice);	
					
					
					startPoint	= new Point(x1,y1);
					endPoint		= new Point(x2,y2);	
						
					//OrderLineBrushDX = buttonFONBrushDX;
		
	
					if (Data3NameFinal != string.Empty)
					if (HitData.Contains(Data3NameFinal))
						ThisBrushDX = cachedZoneColor1DX;

					if (Data2NameFinal != string.Empty)
					if (HitData.Contains(Data2NameFinal))
						ThisBrushDX = cachedZoneColor2DX;

					if (Data1NameFinal != string.Empty)
					if (HitData.Contains(Data1NameFinal))
						ThisBrushDX = cachedZoneColor3DX;

					if (HitData.Contains("Daily"))
						ThisBrushDX = cachedZoneColor4DX;

					if (HitData.Contains("Weekly"))
						ThisBrushDX = cachedZoneColor5DX;


					RenderTarget.DrawLine(startPoint.ToVector2(), endPoint.ToVector2(), ThisBrushDX, 2);

					if (reusableTextLayout != null) reusableTextLayout.Dispose();
					reusableTextLayout = new TextLayout(NinjaTrader.Core.Globals.DirectWriteFactory, lb, cachedLabelTextFormat, 1000, 1000);

					float RectWidth = reusableTextLayout.Metrics.Width + (float) pTextFont.Size;
					float RectHeight = reusableTextLayout.Metrics.Height + 3;

					float x1f = x1;

					reusableTextRect = new SharpDX.RectangleF(x1f, y1 - RectHeight, RectWidth, RectHeight);

					RenderTarget.DrawText(lb, cachedLabelTextFormat, reusableTextRect, LabelBrushDX);
						
				}
				
			
				// draw all sone


						if (pShowAll)
			for (int i=0; i < zones.Count; i++)  //loop through each zone and draw it
				{
					if (zoneEnd[i] < FB2 || zoneStart[i] > RMB)
						continue;
					
					
					if (zones[i].Low > chartScale.MaxValue || zones[i].High < chartScale.MinValue)
						continue;

					if (pStrengthEnabled && zones[i].Strength < pZoneMinStrength)
						continue;

					if (pFreshnessEnabled && pHideBrokenZones && zones[i].State == ZoneState.Broken)
						continue;

					DateTime ZoneStartTime = ChartBars.GetTimeByBarIdx(chartControl, zoneStart[i]);
					
					
					double TotalDaysBack = (ZoneStartTime - zones[i].LatestHit).TotalDays;
					
					bool IsRecent1 = TotalDaysBack <= pRecentDays;
					bool IsRecent2 = TotalDaysBack <= pRecentDays2;

					RedoMarkers = false;
		
					
					// Default brush — critical because on first add, Data1/2/3NameFinal
					// may still be empty strings (populated later in OnBarUpdate's FirstTickAll).
					// Without this default, ThisBrushDX was null → NRE on .Opacity access
					// below → entire OnRender aborted → zones invisible until a tick
					// arrived (or user clicked after a tick had fired).
					ThisBrushDX = cachedZoneColor1DX;

					if (Data3NameFinal != string.Empty)
					if (zones[i].AllDetails.Contains(Data3NameFinal))
						ThisBrushDX = cachedZoneColor1DX;

					if (Data2NameFinal != string.Empty)
					if (zones[i].AllDetails.Contains(Data2NameFinal))
						ThisBrushDX = cachedZoneColor2DX;

					if (Data1NameFinal != string.Empty)
					if (zones[i].AllDetails.Contains(Data1NameFinal))
						ThisBrushDX = cachedZoneColor3DX;

					if (zones[i].AllDetails.Contains("Daily"))
						ThisBrushDX = cachedZoneColor4DX;

					if (zones[i].AllDetails.Contains("Weekly"))
						ThisBrushDX = cachedZoneColor5DX;


					buttonFONBrushDX = ThisBrushDX;
					if (buttonFONBrushDX == null) continue; // safety — skip zone if no brush available

					// Strength-based opacity modulation
					float savedOpacity = buttonFONBrushDX.Opacity;
					if (pStrengthEnabled)
						buttonFONBrushDX.Opacity = savedOpacity * (float)(0.3 + 0.7 * zones[i].Strength);

					x1 = chartControl.GetXByBarIndex(ChartBars, zoneEnd[i]);
					x2 = chartControl.GetXByBarIndex(ChartBars, zoneStart[i]);
					y1 = chartScale.GetYByValue(zones[i].Low);
					y2 = chartScale.GetYByValue(zones[i].High);

					float widthsa = x1 - x2;
					float heightsa = y1 - y2;

					if (!IsInHitTest)
					{
						if (y1 == y2)
						{
								startPoint = new Point(x1, y1);
								endPoint = new Point(x2, y2);

								RenderTarget.DrawLine(startPoint.ToVector2(), endPoint.ToVector2(), buttonFONBrushDX, 2);

								rectangleF2 = new SharpDX.RectangleF(x2 - 1, y2, widthsa, heightsa + 2);
								AllRefreshRects.Add(rectangleF2);
						}
						else
						{
							rectangleF2 = new SharpDX.RectangleF(x2, y2, widthsa, heightsa);
							RenderTarget.FillRectangle(rectangleF2, buttonFONBrushDX);
							AllRefreshRects.Add(rectangleF2);

							// Freshness border and glow
							if (pFreshnessEnabled)
							{
								SharpDX.Direct2D1.StrokeStyle zoneStroke = cachedStrokeUntested;
								if (zones[i].State == ZoneState.Tested) zoneStroke = cachedStrokeTested;
								else if (zones[i].State == ZoneState.Broken) zoneStroke = cachedStrokeBroken;

								float borderOpacity = buttonFONBrushDX.Opacity;
								buttonFONBrushDX.Opacity = Math.Min(1.0f, borderOpacity * 2.0f);
								RenderTarget.DrawRectangle(rectangleF2, buttonFONBrushDX, 1, zoneStroke);
								buttonFONBrushDX.Opacity = borderOpacity;

								// Glow for untested zones
								if (zones[i].State == ZoneState.Untested)
								{
									SharpDX.RectangleF glowRect = new SharpDX.RectangleF(rectangleF2.X - 2, rectangleF2.Y - 2, rectangleF2.Width + 4, rectangleF2.Height + 4);
									buttonFONBrushDX.Opacity = 0.10f;
									RenderTarget.FillRectangle(glowRect, buttonFONBrushDX);
								}
							}
						}

					// Restore opacity
					buttonFONBrushDX.Opacity = savedOpacity;

						if (pLabelsEnabled)
						if (pHitTotalEnabled || pShowLatest || pShowHitData)
								{
									{
										
										
										//lb = string.Empty;
										
									//	Print(zones[i].ThisAllHits);
										
										
										string lb = string.Empty;
										
										if (pHitTotalEnabled)
										lb = zones[i].ThisAllHits.Count.ToString();
										
								
										string bigspacer = "      ";
			

										if (pShowLatest || pShowHitData)
										if (MouseIn(rectangleF2,3,3))
										{
										
											if (pShowLatest)
											{
											
												string latestdata = GetDataName(zones[i].LatestOneLevel.ThisData);
												string latesttype = zones[i].LatestOneLevel.ThisName;
												DateTime latesttime = zones[i].LatestOneLevel.ThisTime;
												
												
												string xxssf = string.Empty;
											
												
												if (latestdata.Contains("Weekly"))
												{
													xxssf = "";
												}
												else if (latestdata.Contains("Daily"))
												{
													xxssf = latesttime.ToString("ddd");	
												}
												else
												{
				
													
													xxssf = latesttime.ToString("ddd H:mm");
													
													int dowwww = (int) frameNow.DayOfWeek - 1;
													
													
													//Print(dowwww);
													
													
													//if (TotalDaysBack >=7)
														
													if (TotalDaysBack > dowwww)	
														xxssf = zones[i].LatestHit.ToString("M/d H:mm");
																									
												}
												
												
												xxssf = latestdata + " " + latesttype + " " + xxssf;
												
												lb = xxssf + bigspacer + lb;
												
											}
											
											
											if (pShowHitData)
											{
												string alldetails = zones[i].AllDetails;
												lb = alldetails + bigspacer + lb;
											}

										}

										if (pStrengthEnabled && pShowHitData)
											lb = lb + " | S: " + zones[i].Strength.ToString("F2");

										if (pFreshnessEnabled && pZoneTestCountLabel)
											lb = lb + " | T: " + zones[i].TestCount;

										if (reusableTextLayout != null) reusableTextLayout.Dispose();
										reusableTextLayout = new TextLayout(NinjaTrader.Core.Globals.DirectWriteFactory, lb, cachedLabelTextFormat, 1000, 1000);

										float RectWidth = reusableTextLayout.Metrics.Width + (float) pTextFont.Size;
										float RectHeight = reusableTextLayout.Metrics.Height + 3;

										x1 = (float) ChartPanel.X + (float) ChartPanel.W;

										float x1f = Math.Min(x1, chartControl.GetXByBarIndex(ChartBars, zoneEnd[i]));

										y1 = chartScale.GetYByValue((float) zones[i].Low) - RectHeight;

										reusableTextRect = new SharpDX.RectangleF(x1f - RectWidth, y1, RectWidth, RectHeight);

										LabelBrushDX.Opacity = pMainTextOpacity / 100f;

										if (MouseIn(rectangleF2, 3, 3))
											LabelBrushDX.Opacity = pHoverTextOpacity / 100f;

										RenderTarget.DrawText(lb, cachedLabelTextFormat, reusableTextRect, LabelBrushDX);
										
									}

							}
						
						
					}
			
				}
			
				
			// Dispose reusable text layout from this frame
			if (reusableTextLayout != null) { reusableTextLayout.Dispose(); reusableTextLayout = null; }
				
				
				if (chartScale.MaxValue != PreviousMaxValue || chartScale.MinValue != PreviousMinValue)
				{
					RedoMarkers = true;
					
							
				}
				
							
				PreviousMaxValue = chartScale.MaxValue;
				PreviousMinValue = chartScale.MinValue;
				
				
				VisiblePriceHigh = PreviousMaxValue;
				VisiblePriceLow = PreviousMinValue;
				
				
		// Pop chart-content clip before rendering panel
			if (panelClipActive) { RenderTarget.PopAxisAlignedClip(); panelClipActive = false; }

		// BUTTONS — Accordion panel with shared coordinator
			SharpDX.Direct2D1.Brush buttonBrushDX = cachedButtonBrushDX;
			SharpDX.Direct2D1.Brush buttonHBrushDX = cachedButtonHBrushDX;
			SharpDX.Direct2D1.Brush buttonFOFFBrushDX = cachedButtonFOFFBrushDX;
			buttonFONBrushDX = cachedAreaBrushDX;

			if (!IsInHitTest && pButtonsEnabled && cachedButtonBrushDX != null && cachedHeaderTextFormat != null)
			{
				bool anyMenuOpen = IsAnyPanelMenuOpen();
				bool expanded = IsPanelExpanded();
				bool showButtons = anyMenuOpen || InMenu;

				// Detect a left Y-axis on the chart panel and offset our panel so
				// it renders past the axis. Single source of truth — same helper
				// used by the panelZoneW clip so they stay aligned.
				float panelLeftX = ComputePanelLeftXForClip(chartControl);
				// Add a few extra pixels of breathing room between the axis and
				// the buttons so the gap matches what the user sees when no left
				// axis is present.
				if (panelLeftX > 0) panelLeftX += S(2);

				// Default B2 detection zone. Width matches the last-known panel
				// width (via GetMaxPanelWidth) so the initial hover zone covers
				// wider headers, not just the leftmost ~80px.
				float defaultZoneW = Math.Max(S(80), GetMaxPanelWidth() + S(16));
				B2 = new SharpDX.RectangleF(0, 0, defaultZoneW - panelLeftX, 10000);

				if (!showButtons && panelWasVisible && lastPanelRect.Width > 0)
				{
					ChartControl.InvalidateVisual();
					panelWasVisible = false;
					lastPanelRect = new SharpDX.RectangleF(0, 0, 0, 0);
				}

				if (showButtons)
				{
					SharpDX.RectangleF panelClipRect = new SharpDX.RectangleF(
						panelLeftX, PANEL_TOP_MARGIN - 10, 250, ChartPanel.H - PANEL_TOP_MARGIN + 20);
					RenderTarget.PushAxisAlignedClip(panelClipRect, SharpDX.Direct2D1.AntialiasMode.PerPrimitive);

					float CY = GetPanelYOffset();
					float startCY = CY;

					SharpDX.Size2F headerSize = MeasureText(PANEL_HEADER_TEXT, cachedHeaderTextFormat);
					float headerH = Math.Max(S(22f), headerSize.Height + S(9));
					float headerW = headerSize.Width + S(28);

					float maxBtnW = headerW;
					float totalBtnH = 0;
					SharpDX.DirectWrite.TextFormat ButtonText = cachedButtonTextFormat;
					if (expanded || panelAnimProgress > 0.01f)
					{
						bool hasDrawnButtonM = false;
						bool lastWasBlankM = false;
						foreach (KeyValuePair<double, ButtonZ> tb in AllButtonZ)
						{
							if (tb.Value.Text == "") { if (!hasDrawnButtonM || lastWasBlankM) continue; lastWasBlankM = true; totalBtnH += 9; continue; }
							lastWasBlankM = false; hasDrawnButtonM = true;
							SharpDX.Size2F bs = MeasureText(tb.Value.Text, ButtonText);
							// Must match actual button height used in draw loop (FinalH = btnSize.Height + S(3))
							// otherwise backdrop ends ~1px/button too early and the last button gets cut off.
							totalBtnH += bs.Height + S(3) + space;
							float bw = bs.Width + S(14);
							if (bw > maxBtnW) maxBtnW = bw;
						}
					}
					float panelW = maxBtnW + S(16);
					float unifiedW = Math.Max(panelW, GetMaxPanelWidth());

					B2 = new SharpDX.RectangleF(0, 0, Math.Max(S(80), unifiedW + S(16)) - panelLeftX, 10000);

					headerW = headerSize.Width + S(32);
					headerRect = new SharpDX.RectangleF(panelLeftX, CY, headerW, headerH);
					panelAnimProgress = expanded ? 1f : 0f;

					bool headerHovered = MouseIn(headerRect, 1, 1)
						|| (panelAnimProgress > 0.5f && MouseIn(new SharpDX.RectangleF(panelLeftX, CY, unifiedW + 16, headerH + totalBtnH + 20), 1, 1));
					if (headerHovered || draggingPanelId != null) _hoverClickableThisFrame = true;

					// Unified backdrop
					{
						float collapsedW = Math.Max(headerW + 10, unifiedW);
						float collapsedH = headerH + 7;
						float expandedW = Math.Max(unifiedW, headerW + 10);
						float expandedH = headerH + space + 5 + totalBtnH + 4;
						float bdW = collapsedW + (expandedW - collapsedW) * panelAnimProgress;
						float bdH = collapsedH + (expandedH - collapsedH) * panelAnimProgress;
						SharpDX.RectangleF bdRect = new SharpDX.RectangleF(panelLeftX, CY - 2, bdW, bdH);
						lastPanelRect = new SharpDX.RectangleF(bdRect.Left - 1, bdRect.Top - 10, bdRect.Width + 18, bdRect.Height + 18);
						panelWasVisible = true;
						cachedPanelBackdropBrushDX.Opacity = 1.0f;
						RenderTarget.FillRectangle(bdRect, cachedPanelBackdropBrushDX);
						float savedBdOp2 = cachedPanelBackdropBrushDX.Opacity;
						for (int fade = 0; fade < 8; fade++)
						{
							cachedPanelBackdropBrushDX.Opacity = savedBdOp2 * (7 - fade) / 7f;
							RenderTarget.FillRectangle(new SharpDX.RectangleF(bdRect.Right + fade, bdRect.Top, 1, bdRect.Height), cachedPanelBackdropBrushDX);
						}
						for (int fade = 0; fade < 8; fade++)
						{
							cachedPanelBackdropBrushDX.Opacity = savedBdOp2 * (7 - fade) / 7f;
							RenderTarget.FillRectangle(new SharpDX.RectangleF(bdRect.Left, bdRect.Top - 8 + fade, bdRect.Width + 8, 1), cachedPanelBackdropBrushDX);
						}
						for (int fade = 0; fade < 8; fade++)
						{
							cachedPanelBackdropBrushDX.Opacity = savedBdOp2 * (7 - fade) / 7f;
							RenderTarget.FillRectangle(new SharpDX.RectangleF(bdRect.Left, bdRect.Bottom + fade, bdRect.Width + 8, 1), cachedPanelBackdropBrushDX);
						}
						cachedPanelBackdropBrushDX.Opacity = savedBdOp2;

							if (draggingPanelId == PANEL_ID && Math.Abs(dragOffsetY) > 5f)
							{
								cachedHoverGlowBrushDX.Opacity = 0.06f;
								RenderTarget.FillRectangle(bdRect, cachedHoverGlowBrushDX);
								cachedHoverGlowBrushDX.Opacity = 0.08f;
							}
					}

					FillHeaderButton(headerRect, headerHovered ? cachedHeaderBgHoverBrushDX : cachedHeaderBgBrushDX);

					SharpDX.Direct2D1.Brush headerEraseBrush = expanded ? cachedPanelBackdropBrushDX : cachedChartBackgroundBrushDX;
					if (expanded) cachedPanelBackdropBrushDX.Opacity = 1.0f;
					SharpDX.RectangleF eraseRect = new SharpDX.RectangleF(
						headerRect.Left, headerRect.Bottom, headerRect.Width + 10, chartButtonRadius + 4);
					RenderTarget.FillRectangle(eraseRect, headerEraseBrush);
					// Reapply drag-highlight glow on the erase strip, but only when expanded
					if (expanded && draggingPanelId == PANEL_ID && Math.Abs(dragOffsetY) > 5f)
					{
						cachedHoverGlowBrushDX.Opacity = 0.06f;
						RenderTarget.FillRectangle(eraseRect, cachedHoverGlowBrushDX);
						cachedHoverGlowBrushDX.Opacity = 0.08f;
					}

					SharpDX.RectangleF bottomStrip = new SharpDX.RectangleF(
						headerRect.Left, headerRect.Bottom - headerBottomStripH, headerRect.Width, headerBottomStripH);
					RenderTarget.FillRectangle(bottomStrip, headerEraseBrush);
					cachedHoverGlowBrushDX.Opacity = headerHovered ? 0.25f : 0.14f;
					RenderTarget.FillRectangle(bottomStrip, cachedHoverGlowBrushDX);
					cachedHoverGlowBrushDX.Opacity = 0.08f;

					buttonBrushDX.Opacity = headerHovered ? 0.5f : 0.2f;
					float lineY = (float)Math.Floor(headerRect.Bottom) + 0.5f;
					var savedAA = RenderTarget.AntialiasMode;
					RenderTarget.AntialiasMode = SharpDX.Direct2D1.AntialiasMode.Aliased;
					RenderTarget.DrawLine(
						new SharpDX.Vector2(headerRect.Left, lineY),
						new SharpDX.Vector2(headerRect.Right, lineY),
						buttonBrushDX, headerHovered ? 1.5f : 1f);
					RenderTarget.AntialiasMode = savedAA;
					buttonBrushDX.Opacity = 1.0f;

					// Triangle arrow
					float triSize = 8f;
					float triX = headerRect.X + 8;
					float triCY = headerRect.Y + headerRect.Height / 2f - 2f;
					var savedAA2 = RenderTarget.AntialiasMode;
					RenderTarget.AntialiasMode = SharpDX.Direct2D1.AntialiasMode.PerPrimitive;
					if (expanded)
					{
						float triTop = triCY - triSize * 0.4f;
						float triBot = triCY + triSize * 0.4f;
						int slices = 10;
						float sliceH = (triBot - triTop) / slices;
						for (int s = 0; s < slices; s++)
						{
							float frac = (s + 0.5f) / slices;
							float barW = triSize * (1f - frac);
							float barX = triX - 2f + (triSize - barW) / 2f;
							float barY = triTop + s * sliceH;
							RenderTarget.FillRectangle(new SharpDX.RectangleF(barX, barY, barW, sliceH + 0.5f), cachedHeaderTextBrushDX);
						}
					}
					else
					{
						float triLeft = triX + 1f;
						float triRight = triLeft + triSize * 0.5f;
						int slices = 10;
						float sliceW = (triRight - triLeft) / slices;
						for (int s = 0; s < slices; s++)
						{
							float frac = (s + 0.5f) / slices;
							float barH = triSize * (1f - frac);
							float barY2 = triCY - barH / 2f;
							float barX = triLeft + s * sliceW;
							RenderTarget.FillRectangle(new SharpDX.RectangleF(barX, barY2, sliceW + 0.5f, barH), cachedHeaderTextBrushDX);
						}
					}
					RenderTarget.AntialiasMode = savedAA2;

					SharpDX.RectangleF headerTextRect = new SharpDX.RectangleF(
						headerRect.X + 20, headerRect.Y, headerRect.Width - 20, headerRect.Height - 3);
					RenderTarget.DrawText(PANEL_HEADER_TEXT, cachedHeaderTextFormat, headerTextRect, cachedHeaderTextBrushDX);

					CY += headerH + space + 5;

					if (panelAnimProgress > 0.01f)
					{
						// Suppress button hover while header drag is in progress.
						currentButtonHover = -1;
						if (draggingPanelId == null && System.Windows.Input.Mouse.LeftButton != System.Windows.Input.MouseButtonState.Pressed)
						{
							int preIdx = 0;
							foreach (KeyValuePair<double, ButtonZ> thisbutton in AllButtonZ)
							{
								if (thisbutton.Value.Text != "" && MouseIn(thisbutton.Value.Rect, 2, 2))
								{ currentButtonHover = preIdx; break; }
								preIdx++;
							}
						}

						var savedButtonAA = RenderTarget.AntialiasMode;
						RenderTarget.AntialiasMode = SharpDX.Direct2D1.AntialiasMode.PerPrimitive;

						int buttonIndex = 0;
						bool hasDrawnButton = false;
						bool lastWasBlank = false;
						foreach (KeyValuePair<double, ButtonZ> thisbutton in AllButtonZ)
						{
							string btext = thisbutton.Value.Text;

							if (btext == "")
							{
								if (!hasDrawnButton || lastWasBlank) { buttonIndex++; continue; }
								lastWasBlank = true;
								float divY = (float)Math.Floor(CY + (9f - space) / 2f) + 0.5f;
								buttonBrushDX.Opacity = 0.18f;
								var savedDivAA = RenderTarget.AntialiasMode;
								RenderTarget.AntialiasMode = SharpDX.Direct2D1.AntialiasMode.Aliased;
								RenderTarget.DrawLine(new SharpDX.Vector2(panelLeftX, divY), new SharpDX.Vector2(panelLeftX + space + maxBtnW, divY), buttonBrushDX, 1f);
								RenderTarget.AntialiasMode = savedDivAA;
								buttonBrushDX.Opacity = 1.0f;
								CY += 9;
								buttonIndex++;
								continue;
							}
							lastWasBlank = false; hasDrawnButton = true;

							SharpDX.Size2F btnSize = MeasureText(btext, ButtonText);
							float FinalH = btnSize.Height + S(3);
							float FinalW = btnSize.Width + S(14);

							thisbutton.Value.Rect = new SharpDX.RectangleF(panelLeftX + space, CY, FinalW, FinalH);

							bool btnHovered = (buttonIndex == currentButtonHover);
								if (btnHovered) _hoverClickableThisFrame = true;

							// DPI-scale the corner radius so it matches the larger buttons at high DPI.
							float btnR = S(chartButtonRadius);
							var rrect = new SharpDX.Direct2D1.RoundedRectangle() { Rect = thisbutton.Value.Rect, RadiusX = btnR, RadiusY = btnR };

							buttonFOFFBrushDX.Opacity = 0.5f;
							RenderTarget.FillRoundedRectangle(rrect, buttonFOFFBrushDX);

							if (!thisbutton.Value.Switch)
							{
								buttonFONBrushDX.Opacity = 0.35f;
								RenderTarget.FillRoundedRectangle(rrect, buttonFONBrushDX);
							}

							if (btnHovered)
							{
								buttonHBrushDX.Opacity = 0.12f;
								RenderTarget.FillRoundedRectangle(rrect, buttonHBrushDX);
								buttonBrushDX.Opacity = 0.85f;
								RenderTarget.DrawRoundedRectangle(rrect, buttonBrushDX, 1.5f);
							}
							else
							{
								// Outline brightness also carries state: bright for ON, faint for OFF.
								buttonBrushDX.Opacity = thisbutton.Value.Switch ? 0.55f : 0.2f;
								RenderTarget.DrawRoundedRectangle(rrect, buttonBrushDX, 1f);
							}

							SharpDX.RectangleF textRect = new SharpDX.RectangleF(
								thisbutton.Value.Rect.X + S(7), thisbutton.Value.Rect.Y,
								thisbutton.Value.Rect.Width - S(7), thisbutton.Value.Rect.Height);
							float textOpacity = thisbutton.Value.Switch ? 0.95f : 0.45f;
							buttonBrushDX.Opacity = textOpacity;
							ButtonText.TextAlignment = SharpDX.DirectWrite.TextAlignment.Leading;
							RenderTarget.DrawText(btext, ButtonText, textRect, buttonBrushDX);
							ButtonText.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;

							CY += FinalH + space;
							buttonIndex++;
						}

						RenderTarget.AntialiasMode = savedButtonAA;
						buttonBrushDX.Opacity = 1.0f;
						buttonFOFFBrushDX.Opacity = 1.0f;
						if (buttonFONBrushDX != null) buttonFONBrushDX.Opacity = 1.0f;
						buttonHBrushDX.Opacity = 0.4f;
						cachedHoverGlowBrushDX.Opacity = 0.08f;
					}

					// Mirror the backdrop's collapsed/expanded formulas so the reported
					// height matches the backdrop's actual bottom edge — otherwise the
					// next panel's header positions inside this panel's backdrop.
					float bdCollapsedHR = headerH + 7;
					float bdExpandedHR = headerH + space + 5 + totalBtnH + 4;
					float animatedH = bdCollapsedHR + (bdExpandedHR - bdCollapsedHR) * panelAnimProgress;
					ReportPanelHeight(animatedH);
					{
						var registry2 = GetPanelRegistry();
						ConcurrentDictionary<string, object[]> panels2;
						if (registry2.TryGetValue(panelHash, out panels2))
						{
							object[] slot2;
							if (panels2.TryGetValue(PANEL_ID, out slot2) && slot2.Length >= SL_SLOT_SIZE)
								slot2[SL_PANELW] = panelW;
						}
					}
					UpdateScrollBounds();
					RenderTarget.PopAxisAlignedClip();

					// Scroll indicators
					{
						var scrollReg = GetScrollRegistry();
						float[] scrollState;
						if (scrollReg.TryGetValue(panelHash, out scrollState) && scrollState[SC_TOTALH] > scrollState[SC_VIEWH])
						{
							float indicatorX = panelLeftX + 40f;
							float maxScroll = Math.Max(0, scrollState[SC_TOTALH] - scrollState[SC_VIEWH] + 20f);

							if (scrollState[SC_OFFSET] > 1f)
							{
								float opacity = Math.Min(0.6f, scrollState[SC_OFFSET] / 50f);
								cachedHeaderTextBrushDX.Opacity = opacity;
								float cy2 = PANEL_TOP_MARGIN - 2f;
								float sz = 6f;
								int slices2 = 6;
								for (int s = 0; s < slices2; s++)
								{
									float frac = (s + 0.5f) / slices2;
									float barW2 = sz * frac;
									RenderTarget.FillRectangle(new SharpDX.RectangleF(
										indicatorX - barW2 / 2f, cy2 + s * 1.2f, barW2, 1.2f), cachedHeaderTextBrushDX);
								}
							}

							if (scrollState[SC_OFFSET] < maxScroll - 1f)
							{
								float opacity = Math.Min(0.6f, (maxScroll - scrollState[SC_OFFSET]) / 50f);
								cachedHeaderTextBrushDX.Opacity = opacity;
								float cy2 = ChartPanel.H - 10f;
								float sz = 6f;
								int slices2 = 6;
								for (int s = 0; s < slices2; s++)
								{
									float frac = (s + 0.5f) / slices2;
									float barW2 = sz * frac;
									RenderTarget.FillRectangle(new SharpDX.RectangleF(
										indicatorX - barW2 / 2f, cy2 - s * 1.2f, barW2, 1.2f), cachedHeaderTextBrushDX);
								}
							}
							cachedHeaderTextBrushDX.Opacity = 0.7f;
						}
					}
				}
				else
				{
					ReportPanelHeight(0);
					UpdateScrollBounds();
					foreach (KeyValuePair<double, ButtonZ> thisbutton in AllButtonZ)
						thisbutton.Value.Rect = new SharpDX.RectangleF(-100, -100, 0, 0);
				}
			}

			// Dispose reusable text layout at end of frame
			if (reusableTextLayout != null) { reusableTextLayout.Dispose(); reusableTextLayout = null; }

			// Safety: ensure clip is popped if panel section was skipped
			if (panelClipActive) { RenderTarget.PopAxisAlignedClip(); panelClipActive = false; }

			ApplyHoverCursor();
		}

  private double RTTS(double p)
        {
            return Instrument.MasterInstrument.RoundToTickSize(p);
        }

		
		private void DrawLinePlot(Series<double> ThisSeries, int StartBar, float x1, float x2, Stroke ThisStroke, ChartControl chartControl, ChartScale chartScale)
		{
				
				float thisy = (float) ThisSeries.GetValueAt(StartBar);
					
				//Print(thisy);
			
				float y1 = chartScale.GetYByValue(thisy);
			
			
					startPoint	= new Point(x1, y1);
					endPoint = new Point(x2, y1);
			
			RenderTarget.DrawLine(startPoint.ToVector2(), endPoint.ToVector2(), ThisStroke.BrushDX , ThisStroke.Width, ThisStroke.StrokeStyle);
		}
		
		
		private void CheckSession(DateTime StartTime, DateTime EndTime)
		{
			
		}
		
		private void SetBackColor (Brush BB)
		{
			if (pColorAll) 
				BackBrushesAll[0] = BB;
			else
				BackBrushes[0] = BB;			
		}
		

	public override string DisplayName
		{
			get
			{
					if (State == State.SetDefaults)
						return ThisName;
					else
						return Name;
			}
		
		}				
		
		
			//Period						= 14;
		//Smooth						= 3;
		
		
			private int pMinimumPivots = 2;
			[Range(0, int.MaxValue), NinjaScriptProperty]
			[Display(ResourceType = typeof(Custom.Resource), GroupName = "Parameters ", Name = "Zone Minimum Hits", Description = "The minimum number of total high and low hits required.", Order = 7)]
			public int MinimumPivots
	        {
				get { return pMinimumPivots; }
				set { pMinimumPivots = value; }
	        }		
			
			private int pPivotBalance = 0;
		

			private int pZoneWidth2 = 4;
			[Range(0, int.MaxValue), NinjaScriptProperty]
			[Display(ResourceType = typeof(Custom.Resource), GroupName = "Parameters ", Name = "Zone Maximum Height (Ticks)", Description = "Enter the width of each confluence zone, in number of ticks.", Order = 17)]
			public int ZoneWidth2
	        {
				get { return pZoneWidth2; }
				set { pZoneWidth2 = value; }
	        }			
			
		
			private int pZoneSpace = 20;
			[Range(0, int.MaxValue), NinjaScriptProperty]
			[Display(ResourceType = typeof(Custom.Resource), GroupName = "Parameters ", Name = "Zone Minimum Space Between (Ticks)", Description = "Enter the amount of space required between two confluence zones, in number of ticks.", Order = 18)]
			public int ZoneSpace
	        {
				get { return pZoneSpace; }
				set { pZoneSpace = value; }
	        }			
						
			
			// wick, body, both       Hits Included
			// bars, swings			  Bars Included
			
//			ThisHitsIncluded = "Wick";	
//			ThisBarsIncluded = "Bars";	
			
			
		private string pBarsIncluded1 = "All";
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Minute Data 1", Name = "Bars Included", Description = "",  Order = 35)]
		[Description("")]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(BarsIncludedOptions))]
		public string BarsIncluded1
		{
			get { return pBarsIncluded1; }
			set { pBarsIncluded1 = value; }
		}			
	
		private string pHitsIncluded1 = "Wick";
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Minute Data 1", Name = "Hits Included", Description = "",  Order = 36)]
		[Description("")]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(HitsIncludedOptions))]
		public string HitsIncluded1
		{
			get { return pHitsIncluded1; }
			set { pHitsIncluded1 = value; }
		}	
		
		
		private string pBarsIncluded2 = "All";
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Minute Data 2", Name = "Bars Included", Description = "",  Order = 35)]
		[Description("")]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(BarsIncludedOptions))]
		public string BarsIncluded2
		{
			get { return pBarsIncluded2; }
			set { pBarsIncluded2 = value; }
		}			
	
		private string pHitsIncluded2 = "Wick";
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Minute Data 2", Name = "Hits Included", Description = "",  Order = 36)]
		[Description("")]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(HitsIncludedOptions))]
		public string HitsIncluded2
		{
			get { return pHitsIncluded2; }
			set { pHitsIncluded2 = value; }
		}			
		
			
		private string pBarsIncluded3 = "All";
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Minute Data 3", Name = "Bars Included", Description = "",  Order = 35)]
		[Description("")]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(BarsIncludedOptions))]
		public string BarsIncluded3
		{
			get { return pBarsIncluded3; }
			set { pBarsIncluded3 = value; }
		}			
	
		private string pHitsIncluded3 = "Wick";
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Minute Data 3", Name = "Hits Included", Description = "",  Order = 36)]
		[Description("")]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(HitsIncludedOptions))]
		public string HitsIncluded3
		{
			get { return pHitsIncluded3; }
			set { pHitsIncluded3 = value; }
		}			
		
		private string pBarsIncluded4 = "All";
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Daily Data", Name = "Bars Included", Description = "",  Order = 35)]
		[Description("")]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(BarsIncludedOptions))]
		public string BarsIncluded4
		{
			get { return pBarsIncluded4; }
			set { pBarsIncluded4 = value; }
		}			
	
		private string pHitsIncluded4 = "Both";
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Daily Data", Name = "Hits Included", Description = "",  Order = 36)]
		[Description("")]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(HitsIncludedOptions))]
		public string HitsIncluded4
		{
			get { return pHitsIncluded4; }
			set { pHitsIncluded4 = value; }
		}			
		
		
		private string pBarsIncluded5 = "All";
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Weekly Data", Name = "Bars Included", Description = "",  Order = 35)]
		[Description("")]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(BarsIncludedOptions))]
		public string BarsIncluded5
		{
			get { return pBarsIncluded5; }
			set { pBarsIncluded5 = value; }
		}			
	
		private string pHitsIncluded5 = "Both";
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Weekly Data", Name = "Hits Included", Description = "",  Order = 36)]
		[Description("")]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(HitsIncludedOptions))]
		public string HitsIncluded5
		{
			get { return pHitsIncluded5; }
			set { pHitsIncluded5 = value; }
		}			
		
		
		internal class BarsIncludedOptions : StringConverter
		{
			public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
			{
			//true means show a combobox
				return true;
			}
			
			public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
			{
			//true will limit to list. false will show the list, but allow free-form entry
				return true;
			}
		
			public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
			{
				//return new StandardValuesCollection( new String[] {"Bars", "Swing", "Zig Zag"} );
				return new StandardValuesCollection( new String[] {"All", "Swings"} );
			}
		}	

		
		internal class HitsIncludedOptions : StringConverter
		{
			public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
			{
			//true means show a combobox
				return true;
			}
			
			public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
			{
			//true will limit to list. false will show the list, but allow free-form entry
				return true;
			}
		
			public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
			{
				//return new StandardValuesCollection( new String[] {"Bars", "Swing", "Zig Zag"} );
				return new StandardValuesCollection( new String[] {"Wick", "Body", "Both"} );
			}
		}	

	
		private bool pLatestDataAllEnabled = false;
 		[Description("")]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", Description = "", GroupName = "Latest Pivots Display", Order = 0)]
       [RefreshProperties(RefreshProperties.All)]
		public bool LatestDataAllEnabled
        {
            get { return pLatestDataAllEnabled; }
            set { pLatestDataAllEnabled = value; }
        }		
			
		
	     private bool pLatestData3Enabled = false;
 		[Description("")]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Data 3", Description = "", GroupName = "Latest Pivots Display", Order = 100)]
        public bool LatestData3Enabled
        {
            get { return pLatestData3Enabled; }
            set { pLatestData3Enabled = value; }
        }		
			
		
	     private bool pLatestData2Enabled = true;
 		[Description("")]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Data 2", Description = "", GroupName = "Latest Pivots Display", Order = 101)]
        public bool LatestData2Enabled
        {
            get { return pLatestData2Enabled; }
            set { pLatestData2Enabled = value; }
        }	
		
		
	     private bool pLatestData1Enabled = true;
 		[Description("")]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Data 1", Description = "", GroupName = "Latest Pivots Display", Order = 102)]
        public bool LatestData1Enabled
        {
            get { return pLatestData1Enabled; }
            set { pLatestData1Enabled = value; }
        }			
		
		
	      private bool pSessionReset = true;
 		[Description("")]
		[Display(ResourceType = typeof(Custom.Resource), Name = "On New Session", Description = "", GroupName = "Calculate Events", Order = 100)]
        public bool SessionReset
        {
            get { return pSessionReset; }
            set { pSessionReset = value; }
        }		
					
				
	      private bool pTime1Reset = true;
 		[Description("")]
		[RefreshProperties(RefreshProperties.All)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Time 1 Enabled", Description = "Enter a time, in military time, to recalculate the zones.", GroupName = "Calculate Events", Order = 110)]
        public bool Time1Reset
        {
            get { return pTime1Reset; }
            set { pTime1Reset = value; }
        }	
				
		
			private TimeSpan pCalc1Time = new TimeSpan(8,00,0);
	//		[NinjaScriptProperty]
			[Display(ResourceType = typeof(Custom.Resource), Name = "   Time", GroupName = "Calculate Events", Order = 111)]
			public string Calc1T
			{
				get { return pCalc1Time.Hours.ToString("0")+":"+pCalc1Time.Minutes.ToString("00"); }
				set { if(!TimeSpan.TryParse(value, out pCalc1Time)) pCalc1Time=new TimeSpan(0,0,0); }
			}

	      private bool pTime2Reset = false;
		[Description("")]
		[RefreshProperties(RefreshProperties.All)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Time 2 Enabled", Description = "Enter a time, in military time, to recalculate the zones.", GroupName = "Calculate Events", Order = 120)]
        public bool Time2Reset
        {
            get { return pTime2Reset; }
            set { pTime2Reset = value; }
        }	
					
			private TimeSpan pCalc2Time = new TimeSpan(16,00,0);
	//		[NinjaScriptProperty]
			[Display(ResourceType = typeof(Custom.Resource), Name = "   Time", GroupName = "Calculate Events", Order = 121)]
			public string Calc2T
			{
				get { return pCalc2Time.Hours.ToString("0")+":"+pCalc2Time.Minutes.ToString("00"); }
				set { if(!TimeSpan.TryParse(value, out pCalc2Time)) pCalc2Time=new TimeSpan(0,0,0); }
			}
			

			private int pStrength = 10;
		
	
			private int pZoneExpansion = 0;
			
			private int pZoneDistance = 1000;
									
			private int pDaysBack = 10000;
		
			
            private int timeCalculate = 70000; // Default setting for DayBack
            private int timeBegin = 93000; // Default setting for TimeBegin
            private int timeEnd = 160000; // Default setting for TimeEnd
		
			
		private bool pColorAll = true;
		
				
		// ARROW INPUTS

		
   private bool pShowAll = true;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Main Render Switch", Description = "", GroupName = "", Order = 32)]
        public bool ShowAll
        {
            get { return pShowAll; }
            set { pShowAll = value; }
        }	
		
			
   private bool pShowID = true;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Main Render Switch", Description = "", GroupName = "", Order = 32)]
        public bool ShowID
        {
            get { return pShowID; }
            set { pShowID = value; }
        }	
				
		
		// LABELS		
		
		
        private bool pShowPriceMarkers = true;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Price Markers Enabled", Description = "", GroupName = "Display", Order = 59)]
        public bool ShowPriceMarkers
        {
            get { return pShowPriceMarkers; }
            set { pShowPriceMarkers = value; }
        }				
			
		
        private bool pLabelsEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Labels Enabled", Description = "", GroupName = "Display", Order = 60)]
        public bool LabelsEnabled
        {
            get { return pLabelsEnabled; }
            set { pLabelsEnabled = value; }
        }		
		

	       private bool pHitTotalEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), Name = "    Hit Total", Description = "", GroupName = "Display", Order = 61)]
        public bool HitTotalEnabled
        {
            get { return pHitTotalEnabled; }
            set { pHitTotalEnabled = value; }
        }	
		
		
       private bool pShowBalance = false;
       private bool pShowHitData = true;
        [Display(ResourceType = typeof(Custom.Resource), Name = "    Hit Data Details", Description = "", GroupName = "Display", Order = 62)]
        public bool ShowHitData
        {
            get { return pShowHitData; }
            set { pShowHitData = value; }
        }		
				
		
       private bool pShowLatest = true;
        [Display(ResourceType = typeof(Custom.Resource), Name = "    Latest Hit Details", Description = "", GroupName = "Display", Order = 63)]
        public bool ShowLatest
        {
            get { return pShowLatest; }
            set { pShowLatest = value; }
        }	
		
		
		private SimpleFont pTextFont = new SimpleFont("Arial", 11);
		[Display(ResourceType = typeof(Custom.Resource), Name = "Font", Description = "", GroupName = "Display", Order = 64)]
		public SimpleFont TextFont
        {
            get { return pTextFont; }
            set { pTextFont = value; }
        }	
		
			private int pMainTextOpacity = 50;
			[Range(0, 100)]
			[Display(ResourceType = typeof(Custom.Resource), GroupName = "Display", Name = "Label Main Opacity (%)", Description = "", Order = 67)]
			public int MainTextOpacity
	        {
				get { return pMainTextOpacity; }
				set { pMainTextOpacity = value; }
	        }			
					
			private int pHoverTextOpacity = 100;
			[Range(0, 100)]
			[Display(ResourceType = typeof(Custom.Resource), GroupName = "Display", Name = "Label Hover Opacity (%)", Description = "", Order = 68)]
			public int HoverTextOpacity
	        {
				get { return pHoverTextOpacity; }
				set { pHoverTextOpacity = value; }
	        }			

									
		// BACKGROUND
		
      
//					if (zones[i].AllDetails.Contains("Daily"))
//						buttonFONBrushDX = pBrush04.ToDxBrush(RenderTarget);
																							
					
//					if (zones[i].AllDetails.Contains("Weekly"))
//						buttonFONBrushDX = pBrush05.ToDxBrush(RenderTarget);
					
				
			private int pRecentDays2 = 7;
		
	
		private int pRecentDays = 2;
									
	
//		// BUY COLOR
		

//		// SESSION 2 COLOR
		

//		// SESSION 3 COLOR
		

////		// SESSION 4 COLOR
		

////		// SESSION 5 COLOR
		

		// ==================== Zone Strength ====================

		private bool pStrengthEnabled = false;
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Zone Strength", Name = "Strength Scoring Enabled", Description = "Enable composite zone strength scoring based on hit density, timeframe diversity, recency, and balance.", Order = 1)]
		public bool StrengthEnabled
		{
			get { return pStrengthEnabled; }
			set { pStrengthEnabled = value; }
		}

		private double pRecencyHalfLife = 7.0;
		[Range(0.1, 365)]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Zone Strength", Name = "Recency Half-Life (Days)", Description = "Number of days for recency score to decay by half. Lower values favor recent zones more heavily.", Order = 2)]
		public double RecencyHalfLife
		{
			get { return pRecencyHalfLife; }
			set { pRecencyHalfLife = value; }
		}

		private double pZoneMinStrength = 0.0;
		[Range(0.0, 1.0)]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Zone Strength", Name = "Zone Minimum Strength", Description = "Minimum composite strength score (0-1) for a zone to be displayed. Zones below this are hidden.", Order = 3)]
		public double ZoneMinStrength
		{
			get { return pZoneMinStrength; }
			set { pZoneMinStrength = value; }
		}

		// ==================== Zone Freshness ====================

		private bool pFreshnessEnabled = false;
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Zone Freshness", Name = "Freshness Classification Enabled", Description = "Enable untested/tested/broken classification for zones based on price interaction.", Order = 1)]
		public bool FreshnessEnabled
		{
			get { return pFreshnessEnabled; }
			set { pFreshnessEnabled = value; }
		}

		private DashStyleHelper pUntestedStyle = DashStyleHelper.Solid;
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Zone Freshness", Name = "Untested Zone Border", Description = "Border style for zones that price has never touched.", Order = 2)]
		public DashStyleHelper UntestedStyle
		{
			get { return pUntestedStyle; }
			set { pUntestedStyle = value; }
		}

		private DashStyleHelper pTestedStyle = DashStyleHelper.Dash;
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Zone Freshness", Name = "Tested Zone Border", Description = "Border style for zones that price has wicked into but not closed inside.", Order = 3)]
		public DashStyleHelper TestedStyle
		{
			get { return pTestedStyle; }
			set { pTestedStyle = value; }
		}

		private DashStyleHelper pBrokenStyle = DashStyleHelper.Dot;
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Zone Freshness", Name = "Broken Zone Border", Description = "Border style for zones where price has closed inside.", Order = 4)]
		public DashStyleHelper BrokenStyle
		{
			get { return pBrokenStyle; }
			set { pBrokenStyle = value; }
		}

		private bool pHideBrokenZones = false;
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Zone Freshness", Name = "Hide Broken Zones", Description = "Do not render zones that have been broken by price closing inside them.", Order = 5)]
		public bool HideBrokenZones
		{
			get { return pHideBrokenZones; }
			set { pHideBrokenZones = value; }
		}

		private bool pZoneTestCountLabel = true;
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Zone Freshness", Name = "Zone Test Count Label", Description = "Show the number of times price has tested (wicked into) the zone on hover.", Order = 6)]
		public bool ZoneTestCountLabel
		{
			get { return pZoneTestCountLabel; }
			set { pZoneTestCountLabel = value; }
		}

		// ==================== Zone Export ====================

		private bool pExportEnabled = false;
		[RefreshProperties(RefreshProperties.All)]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Zone Export", Name = "Enabled", Description = "Enable CSV export of zone data.", Order = 1)]
		public bool ExportEnabled
		{
			get { return pExportEnabled; }
			set { pExportEnabled = value; }
		}

		private bool pExportTimestamp = true;
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Zone Export", Name = "Timestamp Each Export", Description = "When enabled, each export creates a new timestamped file (for historical analysis). When disabled, the file is overwritten each time (for live trading).", Order = 2)]
		public bool ExportTimestamp
		{
			get { return pExportTimestamp; }
			set { pExportTimestamp = value; }
		}

		private bool pExportAutoOnRecalc = true;
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Zone Export", Name = "Auto-Export on Recalc", Description = "Automatically export zones after each scheduled recalculation. When disabled, zones are only exported when the Export Zones button is clicked.", Order = 3)]
		public bool ExportAutoOnRecalc
		{
			get { return pExportAutoOnRecalc; }
			set { pExportAutoOnRecalc = value; }
		}

		private int pExportMaxAgeDays = 90;
		[Range(0, 9999)]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Zone Export", Name = "Max Zone Age (Days)", Description = "Only export zones whose latest hit is within this many days.", Order = 4)]
		public int ExportMaxAgeDays
		{
			get { return pExportMaxAgeDays; }
			set { pExportMaxAgeDays = value; }
		}

		private string pExportDelimiter = "Comma";
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Zone Export", Name = "CSV Delimiter", Description = "Column separator: Comma, Tab, or Semicolon.", Order = 5)]
		[NinjaScriptProperty]
		[TypeConverter(typeof(ExportDelimiterConverter))]
		public string ExportDelimiter
		{
			get { return pExportDelimiter; }
			set { pExportDelimiter = value; }
		}

		private bool pExportIncludeHitDetails = false;
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Zone Export", Name = "Include Hit Details", Description = "When enabled, adds individual hit rows below each zone row showing each pivot hit's data series, type, price, and time.", Order = 6)]
		public bool ExportIncludeHitDetails
		{
			get { return pExportIncludeHitDetails; }
			set { pExportIncludeHitDetails = value; }
		}

		// ==================== Zone Colors ====================

		private Brush pZoneColor1 = Brushes.Gray;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Display", Name = "Minute Data 3 Color", Order = 10)]
		public Brush ZoneColor1
		{
			get { return pZoneColor1; } set { pZoneColor1 = value; }
		}
		[Browsable(false)]
		public string ZoneColor1S
		{
			get { return Serialize.BrushToString(pZoneColor1); } set { pZoneColor1 = Serialize.StringToBrush(value); }
		}
		
		
		private Brush pZoneColor2 = Brushes.SteelBlue;
		[XmlIgnore]
		//[Description("Market Depth Display")]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Display", Name = "Minute Data 2 Color", Order = 20)]
		public Brush ZoneColor2
		{
			get { return pZoneColor2; } set { pZoneColor2 = value; }
		}
		[Browsable(false)]
		public string ZoneColor2S
		{
			get { return Serialize.BrushToString(pZoneColor2); } set { pZoneColor2 = Serialize.StringToBrush(value); }
		}		
		
		
		private Brush pZoneColor3 = Brushes.SkyBlue;
		[XmlIgnore]
		//[Description("Market Depth Display")]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Display", Name = "Minute Data 1 Color", Order = 30)]
		public Brush ZoneColor3
		{
			get { return pZoneColor3; } set { pZoneColor3 = value; }
		}
		[Browsable(false)]
		public string ZoneColor3S
		{
			get { return Serialize.BrushToString(pZoneColor3); } set { pZoneColor3 = Serialize.StringToBrush(value); }
		}		
		
		
		private Brush pZoneColor4 = Brushes.Brown;
		[XmlIgnore]
		//[Description("Market Depth Display")]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Display", Name = "Daily Data 3 Color", Order = 40)]
		public Brush ZoneColor4
		{
			get { return pZoneColor4; } set { pZoneColor4 = value; }
		}
		[Browsable(false)]
		public string ZoneColor4S
		{
			get { return Serialize.BrushToString(pZoneColor4); } set { pZoneColor4 = Serialize.StringToBrush(value); }
		}		
		
		
		private Brush pZoneColor5 = Brushes.Goldenrod;
		[XmlIgnore]
		//[Description("Market Depth Display")]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Display", Name = "Weekly Data 3 Color", Order = 50)]
		public Brush ZoneColor5
		{
			get { return pZoneColor5; } set { pZoneColor5 = value; }
		}
		[Browsable(false)]
		public string ZoneColor5S
		{
			get { return Serialize.BrushToString(pZoneColor5); } set { pZoneColor5 = Serialize.StringToBrush(value); }
		}		
		
		
		private int pZoneOpacity1 = 20;
        [Range(0, 100)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Display", Name = "Minute Data 3 Opacity (%)", Order = 11)]
        public int ZoneOpacity1
        {
            get { return pZoneOpacity1; }
            set { pZoneOpacity1 = value; }
        }			
		
		private int pZoneOpacity2 = 20;
        [Range(0, 100)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Display", Name = "Minute Data 2 Opacity (%)", Order = 22)]
        public int ZoneOpacity2
        {
            get { return pZoneOpacity2; }
            set { pZoneOpacity2 = value; }
        }			
		
		private int pZoneOpacity3 = 30;
        [Range(0, 100)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Display", Name = "Minute Data 1 Opacity (%)", Order = 33)]
        public int ZoneOpacity3
        {
            get { return pZoneOpacity3; }
            set { pZoneOpacity3 = value; }
        }		
		
		private int pZoneOpacity4 = 50;
        [Range(0, 100)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Display", Name = "Daily Data 3 Opacity (%)", Order = 44)]
        public int ZoneOpacity4
        {
            get { return pZoneOpacity4; }
            set { pZoneOpacity4 = value; }
        }		
		
		private int pZoneOpacity5 = 50;
        [Range(0, 100)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Display", Name = "Weekly Data 3 Opacity (%)", Order = 55)]
        public int ZoneOpacity5
        {
            get { return pZoneOpacity5; }
            set { pZoneOpacity5 = value; }
        }
		
		
		private bool pUseTimeFilter = false;
		[RefreshProperties(RefreshProperties.All)]
//		[NinjaScriptProperty]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", GroupName = "Time Filter", Order = 0, Description = "")]
        public bool UseTimeFilter
        {
            get { return pUseTimeFilter; }
            set { pUseTimeFilter = value; }
        }
		
	
		private TimeSpan pStartTime = new TimeSpan(9,30,0);
//		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Time Start 1", GroupName = "Time Filter", Order = 1)]
		public string StartT
		{
			get { return pStartTime.Hours.ToString("0")+":"+pStartTime.Minutes.ToString("00"); }
			set { if(!TimeSpan.TryParse(value, out pStartTime)) pStartTime=new TimeSpan(0,0,0); }
		}
				
		private TimeSpan pEndTime = new TimeSpan(16,0,0);
//		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Time End 1", GroupName = "Time Filter", Order = 2)]
		public string EndT
		{
			get { return pEndTime.Hours.ToString("0")+":"+pEndTime.Minutes.ToString("00"); }
			set { if(!TimeSpan.TryParse(value, out pEndTime)) pEndTime=new TimeSpan(0,0,0); }
		}			
		
		private TimeSpan pStartTime2 = new TimeSpan(0,00,0);
//		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Time Start 2", GroupName = "Time Filter", Order = 3)]
		public string StartT2
		{
			get { return pStartTime2.Hours.ToString("0")+":"+pStartTime2.Minutes.ToString("00"); }
			set { if(!TimeSpan.TryParse(value, out pStartTime2)) pStartTime2=new TimeSpan(0,0,0); }
		}
				
		private TimeSpan pEndTime2 = new TimeSpan(0,00,0);
//		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Time End 2", GroupName = "Time Filter", Order = 4)]
		public string EndT2
		{
			get { return pEndTime2.Hours.ToString("0")+":"+pEndTime2.Minutes.ToString("00"); }
			set { if(!TimeSpan.TryParse(value, out pEndTime2)) pEndTime2=new TimeSpan(0,0,0); }
		}	
		
		private TimeSpan pStartTime3 = new TimeSpan(0,00,0);
//		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Time Start 3", GroupName = "Time Filter", Order = 5)]
		public string StartT3
		{
			get { return pStartTime3.Hours.ToString("0")+":"+pStartTime3.Minutes.ToString("00"); }
			set { if(!TimeSpan.TryParse(value, out pStartTime3)) pStartTime3=new TimeSpan(0,0,0); }
		}
				
		private TimeSpan pEndTime3 = new TimeSpan(0,00,0);
//		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Time End 3", GroupName = "Time Filter", Order = 6)]
		public string EndT3
		{
			get { return pEndTime3.Hours.ToString("0")+":"+pEndTime3.Minutes.ToString("00"); }
			set { if(!TimeSpan.TryParse(value, out pEndTime3)) pEndTime3=new TimeSpan(0,0,0); }
		}			
				
					
		private bool pUseTimeFilter1 = false;
		[RefreshProperties(RefreshProperties.All)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Time Filter", GroupName = "Minute Data 1", Order = 100, Description = "")]
        public bool UseTimeFilter1
        {
            get { return pUseTimeFilter1; }
            set { pUseTimeFilter1 = value; }
        }
				
		
		private bool pUseTimeFilter2 = false;
		[RefreshProperties(RefreshProperties.All)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Time Filter", GroupName = "Minute Data 2", Order = 100, Description = "")]
        public bool UseTimeFilter2
        {
            get { return pUseTimeFilter2; }
            set { pUseTimeFilter2 = value; }
        }
				
		
		private bool pUseTimeFilter3 = false;
		[RefreshProperties(RefreshProperties.All)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Time Filter", GroupName = "Minute Data 3", Order = 100, Description = "")]
        public bool UseTimeFilter3
        {
            get { return pUseTimeFilter3; }
            set { pUseTimeFilter3 = value; }
        }
				
		
		// Secondary Feed
	
		
		private bool pUseSData1 = true;
		[RefreshProperties(RefreshProperties.All)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", Description = "", GroupName = "Minute Data 1", Order = 31)]
        public bool UseSData1
        {
            get { return pUseSData1; }
            set { pUseSData1 = value; }
        }					
		
		
		private bool pUseSData2 = true;
		[RefreshProperties(RefreshProperties.All)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", Description = "", GroupName = "Minute Data 2", Order = 31)]
        public bool UseSData2
        {
            get { return pUseSData2; }
            set { pUseSData2 = value; }
        }					
				
	
		private bool pUseSData3 = true;
		[RefreshProperties(RefreshProperties.All)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", Description = "", GroupName = "Minute Data 3", Order = 31)]
        public bool UseSData3
        {
            get { return pUseSData3; }
            set { pUseSData3 = value; }
        }			
		
		private bool pUseDailyData = true;
		[RefreshProperties(RefreshProperties.All)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", Description = "", GroupName = "Daily Data", Order = 31)]
        public bool UseDailyData
        {
            get { return pUseDailyData; }
            set { pUseDailyData = value; }
        }			
			
		
		private bool pUseWeeklyData = true;
		[RefreshProperties(RefreshProperties.All)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", Description = "", GroupName = "Weekly Data", Order = 31)]
        public bool UseWeeklyData
        {
            get { return pUseWeeklyData; }
            set { pUseWeeklyData = value; }
        }			
			
		
        private int pNumberOfMonthsToLoadDaily = 12;
        [Range(0, int.MaxValue)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Months To Load", GroupName = "Daily Data", Order = 50)]
        public int NumberOfMonthsToLoadDaily
        {
            get { return pNumberOfMonthsToLoadDaily; }
            set { pNumberOfMonthsToLoadDaily = value; }
        }		
		
	
        private int pNumberOfMonthsToLoadWeekly = 24;
        [Range(0, int.MaxValue)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Months To Load", GroupName = "Weekly Data", Order = 50)]
        public int NumberOfMonthsToLoadWeekly
        {
            get { return pNumberOfMonthsToLoadWeekly; }
            set { pNumberOfMonthsToLoadWeekly = value; }
        }		
				
		
		private int pNumberOfDaysToLoadMinute1 = 90;
        [Range(0, int.MaxValue)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Days To Load", GroupName = "Minute Data 1", Order = 50)]
        public int NumberOfDaysToLoadMinute1
        {
            get { return pNumberOfDaysToLoadMinute1; }
            set { pNumberOfDaysToLoadMinute1 = value; }
        }		
				
   
		private int pNumberOfDaysToLoadMinute2 = 30;
        [Range(0, int.MaxValue)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Days To Load", GroupName = "Minute Data 2", Order = 50)]
        public int NumberOfDaysToLoadMinute2
        {
            get { return pNumberOfDaysToLoadMinute2; }
            set { pNumberOfDaysToLoadMinute2 = value; }
        }		
						

		private int pNumberOfDaysToLoadMinute3 = 7;
        [Range(0, int.MaxValue)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Days To Load", GroupName = "Minute Data 3", Order = 50)]
        public int NumberOfDaysToLoadMinute3
        {
            get { return pNumberOfDaysToLoadMinute3; }
            set { pNumberOfDaysToLoadMinute3 = value; }
        }		
								
		
		internal class PTS : StringConverter
		{
			public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
			{
			//true means show a combobox
				return true;
			}
			
			public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
			{
			//true will limit to list. false will show the list, but allow free-form entry
				return true;
			}
		
			public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
			{
				return new StandardValuesCollection( new String[] { "Tick", "Volume", "Range", "Second", "Minute", "Renko", "Day", "Week", "Month" } );
			}
		}	
		
		
		private BarsPeriodType AcceptableBasePeriodType1 = BarsPeriodType.Minute;

			
//				}	
				
//			}
//		}
		
	
        private int pThisBarPeriod1 = 240;
        [Range(1, int.MaxValue)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Minute Bars", GroupName = "Minute Data 1", Order = 32)]
        public int ThisBarPeriod1
        {
            get { return pThisBarPeriod1; }
            set { pThisBarPeriod1 = value; }
        }		
		
		
		private BarsPeriodType AcceptableBasePeriodType2 = BarsPeriodType.Minute;

			
//				}	
				
//			}
//		}
		
        private int pThisBarPeriod2 = 60;
        [Range(1, int.MaxValue)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Minute Bars", GroupName = "Minute Data 2", Order = 32)]
        public int ThisBarPeriod2
        {
            get { return pThisBarPeriod2; }
            set { pThisBarPeriod2 = value; }
        }		
		
		
		private BarsPeriodType AcceptableBasePeriodType3 = BarsPeriodType.Minute;
		
		
        private int pThisBarPeriod3 = 5;
        [Range(1, int.MaxValue)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Minute Bars", GroupName = "Minute Data 3", Order = 32)]
        public int ThisBarPeriod3
        {
            get { return pThisBarPeriod3; }
            set { pThisBarPeriod3 = value; }
        }		
				
		
private Brush areaBrush = Brushes.DimGray;
        private Brush textBrush = Brushes.Blue;
        //private Brush smallAreaBrush = Brushes.Red;
        private int areaOpacity = 30;
       // const float fontHeight = 30f;


        private bool pButtonsEnabled = true;
			[RefreshProperties(RefreshProperties.All)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", GroupName = "Chart Buttons", Order = 1)]
        public bool ButtonsEnabled
        {
            get { return pButtonsEnabled; }
            set { pButtonsEnabled = value; }
        }

        [XmlIgnore]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Off Color", GroupName = "Chart Buttons", Order = 2)]
        public Brush AreaBrush
        {
            get { return areaBrush; }
            set
            {
                areaBrush = value;
                if (areaBrush != null)
                {
                    if (areaBrush.IsFrozen)
                        areaBrush = areaBrush.Clone();
                    areaBrush.Opacity = areaOpacity / 100d;
                    areaBrush.Freeze();
                }
            }
        }

        [Browsable(false)]
        public string AreaBrushSerialize
        {
            get { return Serialize.BrushToString(AreaBrush); }
            set { AreaBrush = Serialize.StringToBrush(value); }
        }

        [Browsable(false)]
        public bool PanelExpandedState
        {
            get { return panelExpanded; }
            set
            {
                panelExpanded = value;
                _xmlExpandState = value;
                if (panelHash != 0)
                {
                    var registry = GetPanelRegistry();
                    ConcurrentDictionary<string, object[]> panels;
                    if (registry.TryGetValue(panelHash, out panels))
                    {
                        object[] slot;
                        if (panels.TryGetValue(PANEL_ID, out slot) && slot.Length > SL_EXPANDED)
                            slot[SL_EXPANDED] = value;
                    }
                }
            }
        }


        [Range(0, 100)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Off Opacity (%)", GroupName = "Chart Buttons", Order = 3)]
        public int AreaOpacity
        {
            get { return areaOpacity; }
            set
            {
                areaOpacity = Math.Max(0, Math.Min(100, value));
                if (areaBrush != null)
                {
                    Brush newBrush = areaBrush.Clone();
                    newBrush.Opacity = areaOpacity / 100d;
                    newBrush.Freeze();
                    areaBrush = newBrush;
                }
            }
        }

        private int pButtonSize = 20;
        [Range(1, int.MaxValue)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Size (Pixels)", GroupName = "Chart Buttons", Order = 4)]
        public int ButtonSize
        {
            get { return pButtonSize; }
            set { pButtonSize = value; }
        }

        private int pChartMenuTextSize = 1;
        [Range(0, 10)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Text Size", GroupName = "Chart Buttons", Order = 17)]
        public int ChartMenuTextSize
        {
            get { return pChartMenuTextSize; }
            set { pChartMenuTextSize = value; }
        }


		private string pLicensingEmailAddress = "";
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "License", Name = "Email Address", Order = 54, Description = "")]
        public string LicensingEmailAddress
        {
            get { return pLicensingEmailAddress; }
            set { pLicensingEmailAddress = value; }
        }			
		
		
	}
	
	
		// Hide UserDefinedValues properties when not in use by the HLCCalculationMode.UserDefinedValues
	// When creating a custom type converter for indicators it must inherit from NinjaTrader.NinjaScript.IndicatorBaseConverter to work correctly with indicators
	public class ExportDelimiterConverter : TypeConverter
	{
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) { return true; }
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) { return true; }
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			return new StandardValuesCollection(new string[] { "Comma", "Tab", "Semicolon" });
		}
	}

	public class aiSRPriceActionConverter : NinjaTrader.NinjaScript.IndicatorBaseConverter
	{
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) { return true; }

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			PropertyDescriptorCollection propertyDescriptorCollection = base.GetPropertiesSupported(context) ? base.GetProperties(context, value, attributes) : TypeDescriptor.GetProperties(value, attributes);

			aiSRPriceAction   jbb = (aiSRPriceAction) value;
			
			//Pivots						thisPivotsInstance			= (Pivots) value;
			
			//bool MagnetsOn = ;
			
			List<string> DeleteThese = new List<string>();
			List<string> DeleteThese2 = new List<string>();
			
			
			DeleteThese.Add("ShowAll");
			DeleteThese.Add("ShowID");
			
			
			DeleteThese.Add("IsLifeTime");
			
				DeleteThese.Add("EntriesEnabled");
				DeleteThese.Add("LongEnabled");
				DeleteThese.Add("ShortEnabled");
			DeleteThese.Add("TrendOnlyEnabled");	
			
				DeleteThese.Add("AutoEnabled");	
			
			DeleteThese.Add("SLTrailOrdersEnabled");	
			DeleteThese.Add("ExitOrdersEnabled");	
			

			DeleteThese.Add("Calculate");
			
			//DeleteThese.Add("Name");
      		DeleteThese.Add("MaximumBarsLookBack");
			
			DeleteThese.Add("Input");
			
			DeleteThese.Add("IsAutoScale");
			DeleteThese.Add("Displacement");
			DeleteThese.Add("DisplayInDataBox");
			DeleteThese.Add("Panel");
			DeleteThese.Add("PaintPriceMarkers");
			DeleteThese.Add("ScaleJustification");
			//DeleteThese.Add("IsVisible");
			
			
	if (!jbb.UseTimeFilter)
			{			
				DeleteThese.Add("StartT");
				DeleteThese.Add("EndT");
				DeleteThese.Add("StartT2");
				DeleteThese.Add("EndT2");
				DeleteThese.Add("StartT3");
				DeleteThese.Add("EndT3");
								
				DeleteThese.Add("UseTimeFilter1");
				DeleteThese.Add("UseTimeFilter2");
				DeleteThese.Add("UseTimeFilter3");
		
		
			}			
					
			
			if (!jbb.LatestDataAllEnabled)
			{			
				DeleteThese.Add("LatestData3Enabled");
				DeleteThese.Add("LatestData2Enabled");
				DeleteThese.Add("LatestData1Enabled");
				
		
			}			
					
		
			if (!jbb.UseSData1)
			{			
				DeleteThese.Add("ThisBarPeriod1");
				DeleteThese.Add("NumberOfDaysToLoadMinute1");
				DeleteThese.Add("UseTimeFilter1");
				DeleteThese.Add("BarsIncluded1");
				DeleteThese.Add("HitsIncluded1");
			}			
							
			if (!jbb.UseSData2)
			{			
				DeleteThese.Add("ThisBarPeriod2");
				DeleteThese.Add("NumberOfDaysToLoadMinute2");
				DeleteThese.Add("UseTimeFilter2");
				DeleteThese.Add("BarsIncluded2");
				DeleteThese.Add("HitsIncluded2");
			}			
								
						
			if (!jbb.UseSData3)
			{			
				DeleteThese.Add("ThisBarPeriod3");
				DeleteThese.Add("NumberOfDaysToLoadMinute3");
				DeleteThese.Add("UseTimeFilter3");
				DeleteThese.Add("BarsIncluded3");
				DeleteThese.Add("HitsIncluded3");
			}				
										
		
			if (!jbb.UseDailyData)
			{			
			
				DeleteThese.Add("NumberOfMonthsToLoadDaily");
				DeleteThese.Add("BarsIncluded4");
				DeleteThese.Add("HitsIncluded4");				
			}		
			

			if (!jbb.UseWeeklyData)
			{			
			
				DeleteThese.Add("NumberOfMonthsToLoadWeekly");
				DeleteThese.Add("BarsIncluded5");
				DeleteThese.Add("HitsIncluded5");				
			}			
			
			
		if (!jbb.ButtonsEnabled)
			{			
			
				DeleteThese.Add("AreaBrush");
				DeleteThese.Add("AreaOpacity");
				DeleteThese.Add("ButtonSize");				
			}				
			
    
			if (!jbb.Time1Reset)
			{			
				DeleteThese.Add("Calc1T");
			}			
				
			
			if (!jbb.Time2Reset)
			{			
				DeleteThese.Add("Calc2T");
			}					
			
	 
			// order execution
			
			// hide these inputs but still save last settings in templates and everything.
			
		
			DeleteThese.Add("UseFixedVerticalScale");
			DeleteThese.Add("UseYScroll");
		
			
			DeleteThese.Add("OrderPanelOn");
			DeleteThese.Add("OrdersDisplayOn");
			
			
			DeleteThese.Add("LastQuantity");
			DeleteThese.Add("LastAccount");
			DeleteThese.Add("LastATM");
			DeleteThese.Add("LastTIF");
			
			DeleteThese.Add("ThisEntryType1");
			DeleteThese.Add("LimitOrderOffset1");
			DeleteThese.Add("ThisEntryType2");
			DeleteThese.Add("LimitOrderOffset2");
			DeleteThese.Add("ThisEntryType3");
			DeleteThese.Add("LimitOrderOffset3");
			
			DeleteThese.Add("ThisEntryType4");
			DeleteThese.Add("LimitOrderOffset6");
			
			DeleteThese.Add("WashoutEntryOrdersIB");
			
			
			DeleteThese.Add("ShowRealizedPNLWhenFlat");  
			
			
			DeleteThese.Add("ProfitAndLossType");  
			DeleteThese.Add("ProfitAndLossType2");  
			
			
			DeleteThese.Add("SplitStopDisplay");  
			
			
			DeleteThese.Add("BuyStackMode");  
			DeleteThese.Add("SellStackMode");  
			DeleteThese.Add("BuyCloseMode");  		
			DeleteThese.Add("SellCloseMode");  
			DeleteThese.Add("BuyWashoutMode");  		
			DeleteThese.Add("SellWashoutMode");  			
			
			
			DeleteThese.Add("UseMIT");  		
			DeleteThese.Add("UseSLM");  			

			DeleteThese.Add("SLOffset");
			DeleteThese.Add("SLOffset2");

			if (!jbb.StrengthEnabled)
			{
				DeleteThese.Add("RecencyHalfLife");
				DeleteThese.Add("ZoneMinStrength");
			}

			if (!jbb.FreshnessEnabled)
			{
				DeleteThese.Add("UntestedStyle");
				DeleteThese.Add("TestedStyle");
				DeleteThese.Add("BrokenStyle");
				DeleteThese.Add("HideBrokenZones");
				DeleteThese.Add("ZoneTestCountLabel");
			}

			if (!jbb.ExportEnabled)
			{
				DeleteThese.Add("ExportTimestamp");
				DeleteThese.Add("ExportAutoOnRecalc");
				DeleteThese.Add("ExportMaxAgeDays");
				DeleteThese.Add("ExportDelimiter");
				DeleteThese.Add("ExportIncludeHitDetails");
			}

			if (DeleteThese.Count == 0 && DeleteThese2.Count == 0)
				return propertyDescriptorCollection;

			
			PropertyDescriptorCollection adjusted = new PropertyDescriptorCollection(null);
			foreach (PropertyDescriptor thisDescriptor in propertyDescriptorCollection)
			{
				
				
				if (DeleteThese.Contains(thisDescriptor.Name))
					adjusted.Add(new PropertyDescriptorExtended(thisDescriptor, o => value, null, new Attribute[] {new BrowsableAttribute(false), }));
				
				else if (DeleteThese2.Contains(thisDescriptor.DisplayName))	
					adjusted.Add(new PropertyDescriptorExtended(thisDescriptor, o => value, null, new Attribute[] {new BrowsableAttribute(false), }));
				
				else if (thisDescriptor.Category == "Data Series")	
					adjusted.Add(new PropertyDescriptorExtended(thisDescriptor, o => value, null, new Attribute[] {new BrowsableAttribute(false), }));
				else
					adjusted.Add(thisDescriptor);
				
			}
			return adjusted;
			
			
		}
	}
	
		
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private aiSRPriceAction[] cacheaiSRPriceAction;
		public aiSRPriceAction aiSRPriceAction(int minimumPivots, int zoneWidth2, int zoneSpace)
		{
			return aiSRPriceAction(Input, minimumPivots, zoneWidth2, zoneSpace);
		}

		public aiSRPriceAction aiSRPriceAction(ISeries<double> input, int minimumPivots, int zoneWidth2, int zoneSpace)
		{
			if (cacheaiSRPriceAction != null)
				for (int idx = 0; idx < cacheaiSRPriceAction.Length; idx++)
					if (cacheaiSRPriceAction[idx] != null && cacheaiSRPriceAction[idx].MinimumPivots == minimumPivots && cacheaiSRPriceAction[idx].ZoneWidth2 == zoneWidth2 && cacheaiSRPriceAction[idx].ZoneSpace == zoneSpace && cacheaiSRPriceAction[idx].EqualsInput(input))
						return cacheaiSRPriceAction[idx];
			return CacheIndicator<aiSRPriceAction>(new aiSRPriceAction(){ MinimumPivots = minimumPivots, ZoneWidth2 = zoneWidth2, ZoneSpace = zoneSpace }, input, ref cacheaiSRPriceAction);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.aiSRPriceAction aiSRPriceAction(int minimumPivots, int zoneWidth2, int zoneSpace)
		{
			return indicator.aiSRPriceAction(Input, minimumPivots, zoneWidth2, zoneSpace);
		}

		public Indicators.aiSRPriceAction aiSRPriceAction(ISeries<double> input , int minimumPivots, int zoneWidth2, int zoneSpace)
		{
			return indicator.aiSRPriceAction(input, minimumPivots, zoneWidth2, zoneSpace);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.aiSRPriceAction aiSRPriceAction(int minimumPivots, int zoneWidth2, int zoneSpace)
		{
			return indicator.aiSRPriceAction(Input, minimumPivots, zoneWidth2, zoneSpace);
		}

		public Indicators.aiSRPriceAction aiSRPriceAction(ISeries<double> input , int minimumPivots, int zoneWidth2, int zoneSpace)
		{
			return indicator.aiSRPriceAction(input, minimumPivots, zoneWidth2, zoneSpace);
		}
	}
}

#endregion
