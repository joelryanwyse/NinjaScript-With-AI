// 
// Copyright (C) 2021, Affordable Indicators, Inc. <www.affordableindicators.com>.
// Affordable Indicators, Inc. reserves the right to modify or overwrite this NinjaScript component with each release.
//

//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.ComponentModel.DataAnnotations;
//using System.Collections.Concurrent;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Xml.Serialization;
//using NinjaTrader.Cbi;
//using NinjaTrader.Gui;
//using NinjaTrader.Gui.Tools; // SimpleFont
//using NinjaTrader.Gui.Chart;
//using NinjaTrader.Gui.SuperDom;
//using NinjaTrader.Data;
//using NinjaTrader.NinjaScript;
//using NinjaTrader.Core.FloatingPoint;
//using NinjaTrader.NinjaScript.DrawingTools;
//using SharpDX.DirectWrite;
//using System.Globalization;

//using NinjaTrader.Gui.Tools;

//using System.Collections.ObjectModel;

//using System.Text.RegularExpressions;
//using System.IO;
//using System.Net;
//using System.Text;
//using System.IO;
//using System.Globalization;

//using NinjaTrader.NinjaScript.Indicators;

//using NinjaTrader.NinjaScript;
//using NinjaTrader.Gui;
//using NinjaTrader.Gui.Chart;
//using NinjaTrader.Gui.NinjaScript;
//using NinjaTrader.Gui.Tools;
//using NinjaTrader.NinjaScript;


using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Tools; // SimpleFont
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.DrawingTools;
using SharpDX.DirectWrite;
using System.Globalization;

//using SharpDX;
//using SharpDX.Direct2D1;
//using SharpDX.DirectWrite;


using System.Text.RegularExpressions;
using System.IO;
using System.Net;
using System.Text;
using System.IO;
using System.Globalization;


using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.WpfPropertyGrid;
using System.Windows.Markup;
using System.Xml.Linq;
using NinjaTrader.Core;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.NinjaScript;
using NinjaTrader.Gui.Tools;
using NinjaTrader.NinjaScript;
using NinjaTrader.NinjaScript.Indicators;

//using NinjaTrader.NinjaScript.Indicators.;
 


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Xml.Linq;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Tools;


//This namespace holds Indicators in this folder and is required. Do not change it. 

namespace NinjaTrader.NinjaScript.Indicators
{
	
	[Gui.CategoryOrder("Indicator", -100)]
	[Gui.CategoryOrder("Parameters ", 0)]
	[Gui.CategoryOrder("Time Filter", 2)]
	//[Gui.CategoryOrder("Execution", 5)]
	
	
	[Gui.CategoryOrder("Delta Trend ", 4)]
	[Gui.CategoryOrder("Delta Trend Display ", 6)]
	
	
	[Gui.CategoryOrder("Secondary Trend", 8)]
	[Gui.CategoryOrder("Secondary Trend Box Display", 10)]
		
	
	[Gui.CategoryOrder("Daily Goal & Loss", 12)]
	[Gui.CategoryOrder("Exit Orders", 14)]
	[Gui.CategoryOrder("Exit Orders Display", 15)]
	[Gui.CategoryOrder("Trailing Stop Loss", 20)]
	
	
	
	
	[Gui.CategoryOrder("Bar Counter", 22)]
	
	
	
	
	[Gui.CategoryOrder("Trigger Line", 24)]

	[Gui.CategoryOrder("Background Color", 34)]
	
	
	
    [Gui.CategoryOrder("Arrows", 30)]
   // [Gui.CategoryOrder("Labels", 32)]
    
    [Gui.CategoryOrder("Audio", 36)]
    [Gui.CategoryOrder("Email", 38)]

	[Gui.CategoryOrder("Chart Trader", 40)]
	[Gui.CategoryOrder("Chart Active Display", 42)]
	[Gui.CategoryOrder("Chart Display", 43)]
	
    [Gui.CategoryOrder("Chart Buttons", 44)]
	
	
	
	[Gui.CategoryOrder("Setup", 9000)]
	[Gui.CategoryOrder("License", 10000)]
			
	
	[TypeConverter("NinjaTrader.NinjaScript.Indicators.aiSIGFirstTouchConverter")]
	public class aiSIGFirstTouch : Indicator
	{
		
	
		private string ThisName = "aiSIGFirstTouch";
		
		
		
		private bool IsMarketAnalyzer = false; 
		private double preval = 0;
		private double val = 0;
		private Brush		smallAreaBrush	= Brushes.Red;
		
		
		private bool AllowInitialExitOrderMove = false;
		
		private double prey = 0;
		private bool FirstOne = false;		
		
		private ChartControlProperties ThisChartProperties;
		private Brush StartBackBrush;
		private Brush NewBackBrush;
		private Brush NewStatusBrush;
		
//        private SharpDX.Direct2D1.Brush ChartTextBrushDX = null;
//		private SharpDX.Direct2D1.Brush ChartBackgroundBrushDX = null;
//		private SharpDX.Direct2D1.Brush ChartBackgroundErrorBrushDX = null;
		
		private bool StartPriceMarkers = false;
		
		private double LongT1, LongT2, ShortT1, ShortT2, LongStopM, ShortStopM, LongStopTr, ShortStopTr = 0;
		
		
		private double LongStopBar, ShortStopBar, LongStopWMA, ShortStopWMA = 0;
		private double High1, Low1, WMA1 = 0;
		
		
		private double PTotalAccountPNL = -987654;
		private double TotalAccountPNL = 0;
		
		private string DailyGoalMessage = string.Empty;
		
		private SharpDX.Direct2D1.Ellipse ThisEllipse;
		
		private List<string> AllErrorMessages = new List<string>();
			//	private AddOns.SetIndicatorValueFromAddonWindowExampleToolsWindow2	SetIndicatorValueFromAddonWindowExampleToolsWindow2;
		
		private List<string> MovedTargets = new List<string>();
		private List<string> MovedStops = new List<string>();
		
		private double SecondsAhead = 0;
		private double SecondsBehind = 0;
		
		private double CurrentTrendStatus = 0;
		private double CurrentTrendStatusButton = 0;
		
		
		private List<int> AllTrends = new List<int>();
		private List<int> AllTrends2 = new List<int>();
		
		
		
		private string LicensingMessage = string.Empty;		

		private SortedDictionary<int, int> ProductIDToMachineIDs = new SortedDictionary<int, int>();
		private ConcurrentDictionary<int, List<string>> ProductIDToInstruments = new ConcurrentDictionary<int, List<string>>();
	
		
		private bool Permission = false;
		

				
		private bool LicenseWordPress (string machineid, string pLicensingEmailAddress)
		{
			pLicensingEmailAddress = pLicensingEmailAddress.Replace(" ", "");
			
			List<int> ThisProductMainIDs = new List<int>();
			List<int> ThisProductSecondaryIDs = new List<int>();
			
			// Product IDs for Main Indicator
			
			ThisProductMainIDs.Add(8575);
			ThisProductMainIDs.Add(503567); // support and resistance suite
			
			// Product IDs for Secondary Features
			
//			ThisProductSecondaryIDs.Add(19318);

			
			
			string pContactEmail = "'license@affordableindicators.com'";
			
			
		
					
					
					
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
				System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
				req.Timeout = 15000;
				req.ReadWriteTimeout = 15000;
				req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36";
				using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)req.GetResponse())
				using (System.IO.StreamReader reader = new System.IO.StreamReader(response.GetResponseStream()))
				{
					result = reader.ReadToEnd();
				}
			}
			catch (Exception ex)
			{
				string cause = "Check your Internet connection, then right-click the chart and select Reload NinjaScript.";
				System.Net.WebException wex = ex as System.Net.WebException;
				if (wex != null)
				{
					switch (wex.Status)
					{
						case System.Net.WebExceptionStatus.NameResolutionFailure: cause = "DNS lookup failed — your computer cannot resolve www.affordableindicators.com. Try switching to a public DNS like Google (8.8.8.8) or Cloudflare (1.1.1.1), or contact your network administrator. Then right-click the chart and select Reload NinjaScript."; break;
						case System.Net.WebExceptionStatus.ConnectFailure:         cause = "Outbound port 443 appears to be blocked. If you use a VPS, firewall, or corporate network, allow outbound HTTPS to www.affordableindicators.com on port 443. Then right-click the chart and select Reload NinjaScript."; break;
						case System.Net.WebExceptionStatus.SecureChannelFailure:   cause = "TLS 1.2 handshake failed. Your Windows installation may need updates (run Windows Update), or a proxy or antivirus is interfering with HTTPS traffic. Then right-click the chart and select Reload NinjaScript."; break;
						case System.Net.WebExceptionStatus.TrustFailure:           cause = "The server certificate could not be verified. Check that your system clock is set correctly, or disable any security software that inspects HTTPS traffic. Then right-click the chart and select Reload NinjaScript."; break;
						case System.Net.WebExceptionStatus.Timeout:                cause = "The server took too long to respond. Check your Internet connection, then right-click the chart and select Reload NinjaScript."; break;
						case System.Net.WebExceptionStatus.ProtocolError:          cause = "The server returned an HTTP error. Please contact Affordable Indicators support — this is a server-side issue."; break;
						default:                                                  cause = "Error: " + wex.Status + ". Check your Internet connection, then right-click the chart and select Reload NinjaScript."; break;
					}
				}
				LicensingMessage = "Unable to connect to the Affordable Indicators licensing server. " + cause;
				AddError(LicensingMessage);
				return false;
			}
	
				

			
	// split the string into lines
				string[] lines = Regex.Split(result, "\r\n");	

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
									
//					Print("-----------");
//					Print(machineidstring + " " + notestring);
//					Print("-----------");
					
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

				
		
		
		
		
		
		
		private struct Entry {   //LIST
			public string Name;
			public string Email;
			public string MachineID;
			public string UniqueID;
			public string Module;
			public DateTime StartDate;
			public DateTime ExpireDate;
			public string Comment1;
			public string Comment2;
			public string Comment3;
			
			public Entry(string name, string email, string machineid, string uniqueid, string module, DateTime startdate, DateTime expiredate, string comment1, string comment2, string comment3) {this.Name = name; this.Email = email; this.MachineID = machineid; this.UniqueID = uniqueid; this.Module = module; this.StartDate = startdate; this.ExpireDate = expiredate; this.Comment1 = comment1; this.Comment2 = comment2; this.Comment3 = comment3;}
		}
	
	

		private double dpiX = 0;
		private	double dpiY = 0;
		
		
		private double FinalXPixel = 0;
		private	double FinalYPixel = 0;		
			
		
		
		
		private bool BuyReady, SellReady = false;

        private IndicatorBase SelectedIndicator = null;
        private int SelectedPlotNumber = 0;
        private Order SelectedOrder = null;

		private int LastCurrentBar = 0;
		private int BarToCancelLimitOrder = 0;

		private double StopLimitOffset, NewStopPrice, NewLimitPrice, CurrentStopPrice = 0;
		private bool OrderInstrumentOK, OrderStateOK, OrderTypeOK, OrderNameOK = false;
		
		private string PreviousAccountName = string.Empty;
	
		private double LastPrice, LastAsk, LastBid = 0;

        private bool SelectPlot = false;

		private int buttonh = 27;
		
		private Brush ButtonBrush = Brushes.DarkGreen;
		private Brush ButtonBrush2 = Brushes.Black;
		
		private Brush ButtonSBrush = Brushes.DarkRed;
		private Brush ButtonSBrush2 = Brushes.Black;
		
		private Brush Button3Brush = Brushes.DimGray;
		private Brush Button3Brush2 = Brushes.Black;
		
		private double SLMPrice = 0;

		private Position ThisPosition;
		                           
		private NinjaTrader.Gui.Tools.QuantityUpDown chartTraderQty;
		private NinjaTrader.Gui.Tools.TifSelector chartTraderTIF;
		private NinjaTrader.Gui.Tools.AccountSelector chartTraderAcct;
		private NinjaTrader.Gui.NinjaScript.AtmStrategy.AtmStrategySelector chartTraderATM;
 		private System.Windows.Controls.Button ThisCloseButton;
		private bool IsCurrentBar = false;

		private bool DoLong = false;
		private bool DoShort = false;
		private bool DoClose = false;
		
		private string CurrentType = string.Empty;
		
		System.Windows.Controls.Grid	chartTraderGrid;
		System.Windows.Controls.Grid	chartTraderButtonsGrid;
		System.Windows.Controls.Grid	myButtonsGrid1;
		System.Windows.Controls.Grid	myButtonsGrid2;
		System.Windows.Controls.Grid	myButtonsGrid3;
        System.Windows.Controls.Grid    myButtonsGrid14;


        System.Windows.Controls.Button miniB1;
        System.Windows.Controls.Button miniB2;
        System.Windows.Controls.Button miniB3;
        System.Windows.Controls.Button miniB4;
        System.Windows.Controls.Button miniB5;
        System.Windows.Controls.Button miniB6;
		
		System.Windows.Controls.Label widelabel;

		System.Windows.Controls.ComboBox cb1;
		
		private NinjaTrader.Gui.Tools.QuantityUpDown OffsetBox;
		//System.Windows.Controls.Slider

        System.Windows.Controls.Button	newButton1;
		System.Windows.Controls.Button	newButton2;
		System.Windows.Controls.Button	newButton3;
		System.Windows.Controls.Button	newButton4;
		System.Windows.Controls.Button	newButton5;
		System.Windows.Controls.Button	newButton6;
		System.Windows.Controls.Button	newButton7;
		System.Windows.Controls.Button	newButton8;
		System.Windows.Controls.Button	newButton9;
		
		System.Windows.Controls.Button	newButton21;
		System.Windows.Controls.Button	newButton22;		
		System.Windows.Controls.Button	newButton23;
		System.Windows.Controls.Button	newButton24;		
		
		private bool SetBuyClick = false;
		private bool SetSellClick = false;
		
		
		private SharpDX.Direct2D1.Brush OrderLineBrushDX;
		private SharpDX.Direct2D1.Brush OrderBoxBrushDX;
		
        private SharpDX.Direct2D1.Brush Plot0BrushDX;
        private SharpDX.Direct2D1.Brush Plot1BrushDX;
        private Brush Plot0Brush;
        private Brush Plot1Brush;

		private Point MP;

        private SharpDX.RectangleF B2 = new SharpDX.RectangleF(0, 0, 0, 0);

        private bool InMenu;
        private bool InMenuP;

        private string ChartType = string.Empty;
        private int TickDirection = 1;

        private bool ButtonOff = false;

		private double CurrentMousePrice = 0;
		
		private double LastClose = 0;
		private double EntryOrderPrice = 0;
		
        private int space = 5;

        SortedDictionary<double, ButtonZ> AllButtonZ = new SortedDictionary<double, ButtonZ>();
		
		SortedDictionary<double, ButtonZ> AllButtonZ2 = new SortedDictionary<double, ButtonZ>();
		
		
        const float fontHeight = 15f;

        private int PriceDigits = 0;
        private string PriceString = string.Empty;

        private List<double> All50Levels = new List<double>();
        private List<double> All100Levels = new List<double>();

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

		private Order LongEntryOrder = null;
		private Order ShortEntryOrder = null;	
		
		private bool SaveNextOrder = false;
		private bool SaveLongOrder = false;
		private bool SaveShortOrder = false;
		
	
		private double CurrentSignal = 0;
		private double LastSignal = 0;
		
			private int LaunchNumber = 0;

		
		private DateTime InitialTime = DateTime.MinValue;
		
        //relplacewith auto location
        private string OIF_file_name = NinjaTrader.Core.Globals.UserDataDir + @"incoming\OIF";
		private string BuyUniqueOrderId, SellUniqueOrderId = string.Empty;
			private string UniqueStrategyId = string.Empty;
			private string UniqueOrderId = string.Empty;
			private int BarAtLaunch = -1;
			//private NinjaTrader.Cbi.LogEventCollection lec = NinjaTrader.Cbi.Globals.LogEvents;
			private string NL = Environment.NewLine;
			private DateTime ErrorMsgPrintedAt = DateTime.MinValue;
			private string ErrorMsg = string.Empty;
			private string instruction = string.Empty;
		private string pAccountName = string.Empty;
		private string pATMName = string.Empty;	
		private string pTIF = "DAY";
		private double pQty = 0;
		
		private string SpreadName = string.Empty;
		private string CHLI = string.Empty;
        private int news = 22;
		
		private string OCOID = string.Empty;
		
		private Series<double> FirstSignals;
		private Series<double> Signals;

        private Series<double> BodyHigh;
        private Series<double> BodyLow;
        private Series<double> WickHigh;
        private Series<double> WickLow;


		private Series<double> TheRange;
		private Series<double> CloseMinusMiddle;
		private Series<double> ThisSMI;

		private int ThisDirection1, ThisDirection2, ThisDirection3, ThisDirection4 = 0;
		
		 private Series<int> Direction1;
		 private Series<int> Direction2;
		
		 private Series<int> Direction3;
		 private Series<int> Direction4;		
		
		 private Series<double> MainATR;
		 private Series<double> StopATR;
		private Series<double> TrailingStopATR;
		private Series<double> TargetATR;
		private Series<double> TargetATR2;
		
		 private Series<double> StopATRPrice;
		private Series<double> TargetATRPrice;	
		private Series<double> TargetATRPrice2;	
	     
		private Series<double> OverallTrend;	
		
		private Series<double> LastSignalBar;
		
		private ATR iATR;
		
        private Series<double> Direction;

        private SharpDX.Vector2 StartPoint = (new Point(0, 0)).ToVector2();
        private SharpDX.Vector2 EndPoint = (new Point(0, 0)).ToVector2();

        private DateTime FirstBarTime = DateTime.MinValue;

        private ChartTrader chartTrader;

		 private Account myAccount;
		

		private string subject = string.Empty;
		private string message = string.Empty;


		private int LastEmailBar = 0;
		private int LastAudioBar = 0;
		private int BB = 0;
				

		private Series<double> Avg;		
		private Series<double> Default;
		private Series<double> LongReady;
		private Series<double> ShortReady;
		
		private Series<double> LongLine;
		private Series<double> ShortLine;			
		
		private Series<double> TrendLine;
		private Series<double> TrendStatus;
		private Series<double> TrendSlope;
		
		private Series<double> TrendLongOK;
		private Series<double> TrendShortOK;
		
		
		private Series<double> BarCount;
		
		private Series<double> IsGap;
		
		private string ChartName = string.Empty;
		
		private bool FirstRender = true;
			
		
		      
        private Series<double> IsInSession;			
		
		private DateTime StartTime, EndTime;
		private DateTime StartTime2, EndTime2;
		private DateTime StartTime3, EndTime3;				

		private bool FirstRun = true;
		
		
		private EMA iEMAFast;
		private EMA iEMASlow;
		
			
		private EMA iEMAFastFeed1;
		private EMA iEMASlowFeed1;	
		
		private EMA iEMAFastFeed2;
		private EMA iEMASlowFeed2;	
		
		private EMA iEMAFastFeed3;
		private EMA iEMASlowFeed3;	
		
		private EMA iEMAFastFeed4;
		private EMA iEMASlowFeed4;			
		
		
		private OrderFlowCumulativeDelta cumulativeDelta;
		
		private SharpDX.Direct2D1.Brush HUDVOLColorDX = null;
		private SharpDX.Direct2D1.Brush HUDNEColorDX = null;
		private SharpDX.Direct2D1.Brush HUDUPColorDX = null;
		private SharpDX.Direct2D1.Brush HUDDNColorDX = null;
		
		private SharpDX.DirectWrite.TextLayout textLayout2 = null;
		
		private int HUDNumber = 0;
		
		//private SharpDX.Direct2D1.Brush ThisBrushDX = null;
		
		
		Stroke ThisStroke = new Stroke(Brushes.DarkGreen, DashStyleHelper.Solid, 2);
		Stroke ThisStrokeH = new Stroke(Brushes.DarkGreen, DashStyleHelper.Solid, 2);
		
				
		
		
		private SharpDX.RectangleF ThisRect = new SharpDX.RectangleF(0, 0, 0, 0);
		
		private SharpDX.Direct2D1.Brush ChartBackgroundFadeBrushDX = null;

		// --- Cached render brushes (created in OnRenderTargetChanged, disposed there + Terminated) ---
		private SharpDX.Direct2D1.Brush cachedChartTextBrushDX;
		private SharpDX.Direct2D1.Brush cachedChartBackgroundBrushDX;
		private SharpDX.Direct2D1.Brush cachedChartBackgroundFadeBrushDX;
		private SharpDX.Direct2D1.Brush cachedSelectBrushDX;
		private SharpDX.Direct2D1.Brush cachedTextBrushDX2;
		private SharpDX.Direct2D1.Brush cachedTextBrushDX;
		private SharpDX.Direct2D1.Brush cachedUpBrushDX;
		private SharpDX.Direct2D1.Brush cachedDownBrushDX;
		private SharpDX.Direct2D1.Brush cachedFinalBrushDX;
		private SharpDX.Direct2D1.Brush cachedLineBrushDX;
		private SharpDX.Direct2D1.Brush cachedLongBrushDX;
		private SharpDX.Direct2D1.Brush cachedBlackBrushDX;
		private SharpDX.Direct2D1.Brush cachedButtonBrushDX;
		private SharpDX.Direct2D1.Brush cachedButtonHBrushDX;
		private SharpDX.Direct2D1.Brush cachedButtonFHBrushDX;
		private SharpDX.Direct2D1.Brush cachedButtonFOFFBrushDX;
		private SharpDX.Direct2D1.Brush cachedButtonFONBrushDX;
		private SharpDX.Direct2D1.Brush cachedArrowUpBrushDX;
		private SharpDX.Direct2D1.Brush cachedArrowDownBrushDX;
		private SharpDX.Direct2D1.Brush cachedStopLineBrushDX;
		private SharpDX.Direct2D1.Brush cachedTargetLineBrushDX;
		private SharpDX.Direct2D1.Brush cachedPlotDownBrush33DX;
		private SharpDX.Direct2D1.Brush cachedTimerBrushDX;
		private SharpDX.Direct2D1.Brush cachedTableTextBrushDX;
		private SharpDX.Direct2D1.Brush cachedFillUpBrushDX;
		private SharpDX.Direct2D1.Brush cachedFillDownBrushDX;
		private SharpDX.Direct2D1.Brush cachedFillNeutralBrushDX;
		private SharpDX.Direct2D1.Brush cachedAreaBrushDX;
		private SharpDX.Direct2D1.Brush cachedActiveOutlineBrushDX;
		private SharpDX.Direct2D1.Brush cachedSmallAreaBrushDX;
		private SharpDX.Direct2D1.Brush cachedOutlineUp03BrushDX;    // pOrderUpOutlineStroke at 0.3 opacity
		private SharpDX.Direct2D1.Brush cachedOutlineDn03BrushDX;    // pOrderDnOutlineStroke at 0.3 opacity
		private SharpDX.Direct2D1.Brush cachedOutlineBoth03BrushDX;  // pOrderBothOutlineStroke at 0.3 opacity
		private SharpDX.Direct2D1.Brush cachedStop50BrushDX;         // pStopStroke at 0.5 opacity (shadow fill)
		private SharpDX.Direct2D1.Brush cachedTarget50BrushDX;       // pTargetStroke at 0.5 opacity (shadow fill)

		// --- Cached TextFormats (created in OnRenderTargetChanged) ---
		private SharpDX.DirectWrite.TextFormat cachedLabelTextFormat;
		private SharpDX.DirectWrite.TextFormat cachedTextFont2Format;
		private SharpDX.DirectWrite.TextFormat cachedTextFontTimeFormat;
		private SharpDX.DirectWrite.TextFormat cachedTextFontFormat;
		private SharpDX.DirectWrite.TextFormat cachedTextFont8Format;
		private SharpDX.DirectWrite.TextFormat cachedTableTextFormat;
		private SharpDX.DirectWrite.TextFormat cachedButtonTextFormat;


		private int FB = 0;
		private int PrintFB = 0;
		private int LB = 0;
		private int xt = 0;
		private int yt = 0;
		private int thistop2 = 0;
		
		private double y1 = 0;
		private double y2 = 0;
		private double y3 = 0;
		private double y4 = 0;
		private double y5 = 0;
		private double y6 = 0;	
			
		private double xL = 0;
		//private	double xL2 = 0;
		private	double xR = 0;
		private	double xW = 0;
		private	double xW2 = 0;
		
		private double HUDHeight = 0;
		private double MinRightMarginHUD = 0;
		
		private int barWidth = 0;
		private int barDistance = 0;
		
		private double x1 = 0;
		private double x2 = 0;
		private double x3 = 0;
		private double x4 = 0;	
		
		private ConcurrentDictionary<double, PriceBox> PriceX1Boxes = new ConcurrentDictionary<double, PriceBox>();
		private ConcurrentDictionary<double, PriceBox> PriceX2Boxes = new ConcurrentDictionary<double, PriceBox>();

		public class PriceBox
		{

			double top;
			double bottom;
			double height;
			
						
			public double Top { get{return top;} set{top = value; }}
			public double Bottom { get{return bottom;} set{bottom = value; }}
			public double Height { get{return height;} set{height = value; }}

		}

//		SharpDX.Direct2D1.AntialiasMode oldAntialiasMode;
		
	
		private List<List<string>> AllStrings = new List<List<string>>();
		private List<List<int>> AllColors = new List<List<int>>();		
		
			private List<List<string>> AllColumns = new List<List<string>>();

		
		
		
		// TIMER
		
		private string			timeLeft	= string.Empty;
		private DateTime		now		 	= Core.Globals.Now;
		private bool			connected,
								hasRealtimeData;
		private SessionIterator sessionIterator;

		private System.Windows.Threading.DispatcherTimer timer;

		private string BarTimerString = string.Empty;
		
		private bool	isRangeDerivate;
		private bool	isRangeDerivateChecked;
		
		private long volume;
		private bool isVolume, isVolumeBase;		
				
		private string CellString = string.Empty;
        private SharpDX.DirectWrite.TextFormat CellFormat;
				private SharpDX.DirectWrite.TextLayout CellLayout;
		
		private int Feed1, Feed2, Feed3, Feed4 = 0;
		
		private string SFeed1, SFeed2, SFeed3, SFeed4 = string.Empty;
		
		
	
		
		private Series<double> TradeStatus;
		private Series<double> StopPrice;
		private Series<double> TargetPrice;		

		private Series<double> LongEntry;
		private Series<double> LongStop;
		private Series<double> LongStopOut;
		private Series<double> LongPT1;
		private Series<double> LongPT2;
		private Series<double> LongPT3;
		private Series<double> LongPTMax;
		private Series<double> LongPTMin;
		
		private Series<double> ShortEntry;
		private Series<double> ShortStop;
		private Series<double> ShortStopOut;
		private Series<double> ShortPT1;
		private Series<double> ShortPT2;
		private Series<double> ShortPT3;
		private Series<double> ShortPTMax;
		private Series<double> ShortPTMin;
		

		private Series<double> ThisTrend1;
		private Series<double> ThisMA;
	
		

//			VendorLicense("YourVendorName", "YourProductName", "www.your-url.com", "yourAddress@your-url.com.com",
//				// This optional callback is triggered right before the actual license verification and allows
//				// you to delay the configuration.
//				// It's defaulted to NULL if not provided. License verification then is triggered as configured above
//				() =>
//				{
//					// The following demonstrates how to set up additional custom configuration for license verification.
//					// For example, if you planned to offer the indicator for free for use with indexes only, you could skip
//					// the verification process like below:
//					if (Instrument.MasterInstrument.InstrumentType == InstrumentType.Index)
//						return false;

//                // For all other instruments the already configured license verification is triggered.
//				return true;
//				});
		
		
		protected override void OnStateChange()
		{
			
				
			if (State == State.SetDefaults)
			{
				
					
				Name = ThisName;
				Description = @"";
			
							
									
				//VendorLicense("JoelWyse", "SRFirstTouch", "http://affordableindicators.com/", "affordableindicators@gmail.com");
		
				
//				foreach (LicensedFeature ll in NinjaTrader.Cbi.License.LicensedFeatures)
//				{
//					Print(ll.ToString());
					
					
//				}
//				Print(NinjaTrader.Cbi.License.IsLifeTime);
				
				//pIsLifeTime = NinjaTrader.Cbi.License.IsLifeTime;
				pIsLifeTime = true;
				
				
				//Print(IsLifeTime);
				
//				if (!IsLifeTime)
//				{
					
//					CuEnabled = false;
//					HUDEnabled = false;
//				}
				
				
					
				IsMarketAnalyzer = ChartControl == null;
				
			
				
                //Name						= "MouseMove";

				Calculate					= Calculate.OnEachTick;
				
				TextFont						= new SimpleFont("Consolas",13);
				
				
				IsAutoScale					= false;
				IsOverlay					= true;
				DisplayInDataBox			= true;
				ShowTransparentPlotsInDataBox = true;   
				
				DrawOnPricePanel			= true;
				DrawHorizontalGridLines		= true;
				DrawVerticalGridLines		= true;
				PaintPriceMarkers			= true;
				
				
				
				
			//	ScaleJustification			= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				//Disable this property if your indicator requires custom values that cumulate with each new market data event. 
				//See Help Guide for additional information.
				IsSuspendedWhileInactive	= false;
                ArePlotsConfigurable = false;
                AreLinesConfigurable = false;

                AddPlot(new Stroke(Brushes.Black, 2), PlotStyle.Line, "Fast EMA");
				 AddPlot(new Stroke(Brushes.Black, 1), PlotStyle.Line, "Slow EMA");
				AddPlot(new Stroke(Brushes.Black, 1), PlotStyle.Line, "Signals");  //transparent, changed, so that it can be used in output
				

				AddPlot(new Stroke(Brushes.Transparent, 2), PlotStyle.Hash, "Stop Loss"); // 3
				AddPlot(new Stroke(Brushes.Transparent, 3), PlotStyle.Dot, "Trailing Stop Loss");
				AddPlot(new Stroke(Brushes.Transparent, 2), PlotStyle.Hash, "Target 1");
				AddPlot(new Stroke(Brushes.Transparent, 2), PlotStyle.Hash, "Target 2");
				
               // AddPlot(new Stroke(Brushes.Blue, 2), PlotStyle.Line, "P1");

               
                //Plots[1].DashStyleHelper = DashStyleHelper.Solid;

                

                 

            }				
           else  if (State == State.Configure)
		    {
				
				
				//VendorLicense("JoelWyse", "aiSIGFirstTouch", "http://affordableindicators.com/", "affordableindicators@gmail.com");
				
				
//				if (ChartControl != null)			
//				StartPriceMarkers = PaintPriceMarkers;
			
				if (!pSLTrailEnabled)
				pSLTrailOrdersEnabled = false;
				
				
				//pIsLifeTime = NinjaTrader.Cbi.License.IsLifeTime;
				pIsLifeTime = true;
				
				IsMarketAnalyzer = ChartControl == null;
				
				
				if (IsMarketAnalyzer)
				{
					//Print("------------MA");
					//ThisName = Name;
				}
				else
				{
					//Print("------------Chart");
					//ThisName = "aiSIGFirstTouch";
					
					Name = "aiSIGFirstTouch";
					
				}
				
			
				// Minute, Range, Tick, Volume, Second, Renko
				
				//AddDataSeries(Data.BarsPeriodType., 10);
			
				if (pCuEnabled)
				 AddDataSeries(Data.BarsPeriodType.Tick, 1);
			
				int ThisFeed = 0;
				
				if (!pSecondaryFeedsEnabled)
					pTrendOnlyEnabled = false;
				
				if (!pFeed1Enabled && !pFeed2Enabled && !pFeed3Enabled && !pFeed4Enabled)
					pTrendOnlyEnabled = false;
				
				if (pSecondaryFeedsEnabled)
				{
					if (pFeed1Enabled) ThisFeed = ThisFeed + 1;
					if (pFeed1Enabled) Feed1 = ThisFeed;
					if (pFeed2Enabled) ThisFeed = ThisFeed + 1;
					if (pFeed2Enabled) Feed2 = ThisFeed;
					if (pFeed3Enabled) ThisFeed = ThisFeed + 1;
					if (pFeed3Enabled) Feed3 = ThisFeed;
					if (pFeed4Enabled) ThisFeed = ThisFeed + 1;
					if (pFeed4Enabled) Feed4 = ThisFeed;
					
					if (pCuEnabled)
					{
						if (pFeed1Enabled) Feed1 = Feed1 + 1;
						if (pFeed2Enabled) Feed2 = Feed2 + 1;
						if (pFeed3Enabled) Feed3 = Feed3 + 1;
						if (pFeed4Enabled) Feed4 = Feed4 + 1;						
					}
					
					
//					AddDataSeries(BarsPeriodType.Minute, pEMAPeriod1);				
//					AddDataSeries(BarsPeriodType.Minute, pEMAPeriod2);
//					AddDataSeries(BarsPeriodType.Minute, pEMAPeriod3);
//					AddDataSeries(BarsPeriodType.Minute, pEMAPeriod4);
					
					
					if (pFeed1Enabled) AddDataSeries(AcceptableBasePeriodType1, pEMAPeriod1);				
					if (pFeed2Enabled) AddDataSeries(AcceptableBasePeriodType2, pEMAPeriod2);
					if (pFeed3Enabled) AddDataSeries(AcceptableBasePeriodType3, pEMAPeriod3);
					if (pFeed4Enabled) AddDataSeries(AcceptableBasePeriodType4, pEMAPeriod4);
					
					
					
					SFeed1 = pEMAPeriod1.ToString() + " " + AcceptableBasePeriodType1.ToString();
					SFeed2 = pEMAPeriod2.ToString() + " " + AcceptableBasePeriodType2.ToString();
					SFeed3 = pEMAPeriod3.ToString() + " " + AcceptableBasePeriodType3.ToString();
					SFeed4 = pEMAPeriod4.ToString() + " " + AcceptableBasePeriodType4.ToString();
					
					
					
				}
				
				
				
				
				
				
				

				
//				if (ChartControl != null)
//				if (!NinjaTrader.Core.Globals.AtiOptions.IsAtiEnabled)
//				{
				
//					AddError("Your Automated trading interface was automatically enabled.  This will allow the indicator to submit trades.  You must restart NinjaTrader now for this change to take effect.");
//					NinjaTrader.Core.Globals.AtiOptions.IsAtiEnabled = true;
//				}
//				if (NinjaTrader.Core.Globals.AtiOptions.IgnoreDuplicateOifFiles)
//				{
//					NinjaTrader.Core.Globals.AtiOptions.IgnoreDuplicateOifFiles = false;
					
//					AddError("Your Automated trading interface settings have changed.  You must restart NinjaTrader now for this change to take effect.");
//					NinjaTrader.Core.Globals.AtiOptions.IsAtiEnabled = true;
//				}
				
				
				
				
				
				
				//if (NinjaTrader.Cbi.License.MachineId != null)
					
					
					

				//Print(NinjaTrader.Cbi.License.MachineId);
				//Print(NinjaTrader.Cbi.License.MachineId2);
				//Print(NinjaTrader.Cbi.licen);
				
		
		
				
				
		    }

			else if (State == State.DataLoaded)
			{
				

					
					if (Name != ThisName && Name != string.Empty)
						Name = ThisName;					
	
							
				
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
//			            Console.WriteLine("The process failed: {0}", e.ToString());
//						Error = "Cannot find file ' " + path  + " '.";
//						Print("Cannot find file ' " + path  + " '.");
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
							
					
					
					
					
					
				
				if (ChartControl != null)
					
					Permission = LicenseWordPress(NinjaTrader.Cbi.License.MachineId, pLicensingEmailAddress);	
				
				
					
				else
					Permission = true;
				
				
				
				
				
				
				
				if (pDL < 0)
					pDL = pDL * -1;
				
				Calculate					= Calculate.OnEachTick;
				
				iATR = ATR(pATRPeriod);
				
				iEMAFast = EMA(pEMAFastPeriod);
				iEMASlow = EMA(pEMASlowPeriod);
				
	      // Instantiate the indicator
					
					isVolume 		= BarsPeriod.BarsPeriodType == BarsPeriodType.Volume;
					isVolumeBase 	= (BarsPeriod.BarsPeriodType == BarsPeriodType.HeikenAshi || BarsPeriod.BarsPeriodType == BarsPeriodType.Volumetric) && BarsPeriod.BaseBarsPeriodType == BarsPeriodType.Volume;
					
					
					if (pCuEnabled)
					{
				     // cumulativeDelta = OrderFlowCumulativeDelta(pThisCumulativeDeltaType, pThisCumulativeDeltaPeriod, pCumulativeSizeFilter);
						cumulativeDelta = OrderFlowCumulativeDelta(CumulativeDeltaType.UpDownTick, CumulativeDeltaPeriod.Session, 0);
					
					}
					
					
					
//					if (pVWAPEnabled)
//						ThisOrderFlowVWAP =	OrderFlowVWAP(pThisVWAPResolution, Bars.TradingHours, VWAPStandardDeviations.None, 1, 1, 1);
		
						
				if (pSecondaryFeedsEnabled)
				{
//					Feed1 = 1;
//					Feed2 = 2;
//					Feed3 = 3;
//					Feed4 = 4;
					
//					if (pCuEnabled)
//					{
//						Feed1 = Feed1 + 1;
//						Feed2 = Feed2 + 1;
//						Feed3 = Feed3 + 1;
//						Feed4 = Feed4 + 1;						
//					}
					
		
					if (pFeed1Enabled)
					{
						iEMAFastFeed1 = EMA(BarsArray[Feed1], pEMAFastPeriod);
						iEMASlowFeed1 = EMA(BarsArray[Feed1], pEMASlowPeriod);
					}
					
					if (pFeed2Enabled)
					{
						iEMAFastFeed2 = EMA(BarsArray[Feed2], pEMAFastPeriod);
						iEMASlowFeed2 = EMA(BarsArray[Feed2], pEMASlowPeriod);
					}
					if (pFeed3Enabled)
					{					
						iEMAFastFeed3 = EMA(BarsArray[Feed3], pEMAFastPeriod);
						iEMASlowFeed3 = EMA(BarsArray[Feed3], pEMASlowPeriod);
					}
					if (pFeed4Enabled)
					{						
						iEMAFastFeed4 = EMA(BarsArray[Feed4], pEMAFastPeriod);
						iEMASlowFeed4 = EMA(BarsArray[Feed4], pEMASlowPeriod);
					}
					
				}
				
				
				if (ChartControl != null)
				{
					
					ChartName = Instrument.MasterInstrument.Name+"-"+Bars.BarsPeriod.BarsPeriodTypeName.ToString().Replace(" ","");
					
					Plots[0].Width = pWidth1;
					Plots[1].Width = pWidth1;
						
//					Plots[3].Width = pStopStroke.Width;							
//					Plots[5].Width = pTargetStroke.Width;	
//					Plots[6].Width = pTargetStroke.Width;	
					
//					Plots[3].DashStyleHelper = pStopStroke.DashStyleHelper;
//					Plots[5].DashStyleHelper = pTargetStroke.DashStyleHelper;	
//					Plots[6].DashStyleHelper = pTargetStroke.DashStyleHelper;						
						
//					Plots[3].Brush = pStopStroke.Brush;
//					Plots[5].Brush = pTargetStroke.Brush;	
//					Plots[6].Brush = pTargetStroke.Brush;	
						
					Plots[4].Brush = Brushes.Transparent;
					Plots[4].Width = pWidth5;
					
		
			
		
		
//				Plots[2].Width = pWidth2;
//				Plots[3].Width = pWidth3;
				}
				
					Direction1 = new Series<int>(this, MaximumBarsLookBack.Infinite);
					Direction2 = new Series<int>(this, MaximumBarsLookBack.Infinite);
					Direction3 = new Series<int>(this, MaximumBarsLookBack.Infinite);
					Direction4 = new Series<int>(this, MaximumBarsLookBack.Infinite);
				
				
                MainATR = new Series<double>(this, MaximumBarsLookBack.Infinite);
                StopATR = new Series<double>(this, MaximumBarsLookBack.Infinite);
				TrailingStopATR = new Series<double>(this, MaximumBarsLookBack.Infinite);
				
                TargetATR = new Series<double>(this, MaximumBarsLookBack.Infinite);	
			 TargetATR2 = new Series<double>(this, MaximumBarsLookBack.Infinite);		
				
                StopATRPrice = new Series<double>(this, MaximumBarsLookBack.Infinite);
                TargetATRPrice = new Series<double>(this, MaximumBarsLookBack.Infinite);							
			TargetATRPrice2 = new Series<double>(this, MaximumBarsLookBack.Infinite);							
				
				
				OverallTrend = new Series<double>(this, MaximumBarsLookBack.Infinite);	
				LastSignalBar = new Series<double>(this, MaximumBarsLookBack.Infinite);
				
	
		      	FirstSignals = new Series<double>(this, MaximumBarsLookBack.Infinite);
				Signals = new Series<double>(this, MaximumBarsLookBack.Infinite);
                Direction = new Series<double>(this, MaximumBarsLookBack.Infinite);

                BodyHigh = new Series<double>(this, MaximumBarsLookBack.Infinite);
                BodyLow = new Series<double>(this, MaximumBarsLookBack.Infinite);
                WickHigh = new Series<double>(this, MaximumBarsLookBack.Infinite);
                WickLow = new Series<double>(this, MaximumBarsLookBack.Infinite);
	
				//Signal = new Series<double>(this, MaximumBarsLookBack.Infinite);
				
                TheRange = new Series<double>(this, MaximumBarsLookBack.Infinite);
                CloseMinusMiddle = new Series<double>(this, MaximumBarsLookBack.Infinite);
                ThisSMI = new Series<double>(this, MaximumBarsLookBack.Infinite);
				
								
				
                Avg = new Series<double>(this, MaximumBarsLookBack.Infinite);
                Default = new Series<double>(this, MaximumBarsLookBack.Infinite);
                LongReady = new Series<double>(this, MaximumBarsLookBack.Infinite);
				ShortReady = new Series<double>(this, MaximumBarsLookBack.Infinite);
				
                LongLine = new Series<double>(this, MaximumBarsLookBack.Infinite);
				ShortLine = new Series<double>(this, MaximumBarsLookBack.Infinite);	
				
                TrendLine = new Series<double>(this, MaximumBarsLookBack.Infinite);
				TrendStatus = new Series<double>(this, MaximumBarsLookBack.Infinite);
				TrendSlope = new Series<double>(this, MaximumBarsLookBack.Infinite);
				
				TrendLongOK = new Series<double>(this, MaximumBarsLookBack.Infinite);
				TrendShortOK = new Series<double>(this, MaximumBarsLookBack.Infinite);				
				
					
				
				BarCount = new Series<double>(this, MaximumBarsLookBack.Infinite);
				
				IsGap = new Series<double>(this, MaximumBarsLookBack.Infinite);
				
				IsInSession = new Series<double>(this, MaximumBarsLookBack.Infinite);
				
				
				

				TradeStatus  = new Series<double>(this, MaximumBarsLookBack.Infinite);
				StopPrice  = new Series<double>(this, MaximumBarsLookBack.Infinite);
				TargetPrice  = new Series<double>(this, MaximumBarsLookBack.Infinite);	

				LongEntry  = new Series<double>(this, MaximumBarsLookBack.Infinite);
				LongStop  = new Series<double>(this, MaximumBarsLookBack.Infinite);
				LongStopOut	 = new Series<double>(this, MaximumBarsLookBack.Infinite);
				LongPT1  = new Series<double>(this, MaximumBarsLookBack.Infinite);
				LongPT2  = new Series<double>(this, MaximumBarsLookBack.Infinite);
				LongPT3  = new Series<double>(this, MaximumBarsLookBack.Infinite);
				LongPTMax  = new Series<double>(this, MaximumBarsLookBack.Infinite);
				LongPTMin  = new Series<double>(this, MaximumBarsLookBack.Infinite);
				
				ShortEntry  = new Series<double>(this, MaximumBarsLookBack.Infinite);
				ShortStop  = new Series<double>(this, MaximumBarsLookBack.Infinite);
				ShortStopOut  = new Series<double>(this, MaximumBarsLookBack.Infinite);
				ShortPT1  = new Series<double>(this, MaximumBarsLookBack.Infinite);
				ShortPT2  = new Series<double>(this, MaximumBarsLookBack.Infinite);
				ShortPT3  = new Series<double>(this, MaximumBarsLookBack.Infinite);
				ShortPTMax  = new Series<double>(this, MaximumBarsLookBack.Infinite);
				ShortPTMin  = new Series<double>(this, MaximumBarsLookBack.Infinite);				
				
				ThisTrend1  = new Series<double>(this, MaximumBarsLookBack.Infinite);	
				ThisMA  = new Series<double>(this, MaximumBarsLookBack.Infinite);
				
				if (ChartControl != null)
				if (ChartPanel != null)
                {
					ChartPanel.MouseMove += new MouseEventHandler(OnMouseMove);
                    ChartPanel.MouseDown += new MouseButtonEventHandler(OnMouseDown);
					ChartPanel.MouseLeave += new MouseEventHandler(OnMouseLeave);
                }

				
				if (ChartControl != null)
				{
					
	                ChartType = Bars.BarsPeriod.BarsPeriodTypeName;
					
					
					AddButtonZ("TRADES", "ButtonOff", 40, ChartBars.Properties.PlotExecutions != ChartExecutionStyle.DoNotPlot);
					//AddButtonZ("Counter", "Counter", 40, pTimerEnabled);
					

						
						if (pCuEnabled)
					AddButtonZ("Delta", "Delta2", 40, pHUDEnabled);
						
						
						if (pSecondaryFeedsEnabled)
					AddButtonZ("Trend", "Trend", 40, pSecondaryFeedsDisplayEnabled);						
								
		  		
						AddButtonZ("Signals", "Signals", 40, pArrowsEnabled);	
					AddButtonZ("MA", "MA", 40, pVisualEnabled);		
					AddButtonZ("Exits", "Exits", 40, pExitOEnabled);	
						
						
						
//				AddButtonZ("MIT", "pUseMIT", 40, pUseMIT);

				}


                string FS = TickSize.ToString();
                if (FS.Contains("E-"))
                {
                    FS = FS.Substring(FS.IndexOf("E-") + 2);
                    PriceDigits = int.Parse(FS);
                }
                else PriceDigits = Math.Max(0, FS.Length - 2);
                PriceString = "n" + PriceDigits;

			}
			else if ( State == State.Terminated )
			{
				
				
				
				if (ChartControl != null)
                if (ChartPanel != null)
                {
                    ChartPanel.MouseMove -= new MouseEventHandler(OnMouseMove);
                    ChartPanel.MouseDown -= new MouseButtonEventHandler(OnMouseDown);
					ChartPanel.MouseLeave -= new MouseEventHandler(OnMouseLeave);
                }
				
				if (ChartControl != null)
				{
					//PaintPriceMarkers = StartPriceMarkers;
					
					ChartControl.Dispatcher.InvokeAsync((Action)(() =>
					{
						RemoveWPFControls();
					}));
				}
				
				
				if (timer != null)
				{
					timer.IsEnabled = false;
					timer = null;
				}

				// --- Dispose cached render brushes ---
				if (cachedChartTextBrushDX != null) { cachedChartTextBrushDX.Dispose(); cachedChartTextBrushDX = null; }
				if (cachedChartBackgroundBrushDX != null) { cachedChartBackgroundBrushDX.Dispose(); cachedChartBackgroundBrushDX = null; }
				if (cachedChartBackgroundFadeBrushDX != null) { cachedChartBackgroundFadeBrushDX.Dispose(); cachedChartBackgroundFadeBrushDX = null; }
				if (cachedSelectBrushDX != null) { cachedSelectBrushDX.Dispose(); cachedSelectBrushDX = null; }
				if (cachedTextBrushDX2 != null) { cachedTextBrushDX2.Dispose(); cachedTextBrushDX2 = null; }
				if (cachedTextBrushDX != null) { cachedTextBrushDX.Dispose(); cachedTextBrushDX = null; }
				if (cachedUpBrushDX != null) { cachedUpBrushDX.Dispose(); cachedUpBrushDX = null; }
				if (cachedDownBrushDX != null) { cachedDownBrushDX.Dispose(); cachedDownBrushDX = null; }
				if (cachedFinalBrushDX != null) { cachedFinalBrushDX.Dispose(); cachedFinalBrushDX = null; }
				if (cachedLineBrushDX != null) { cachedLineBrushDX.Dispose(); cachedLineBrushDX = null; }
				if (cachedLongBrushDX != null) { cachedLongBrushDX.Dispose(); cachedLongBrushDX = null; }
				if (cachedBlackBrushDX != null) { cachedBlackBrushDX.Dispose(); cachedBlackBrushDX = null; }
				if (cachedButtonBrushDX != null) { cachedButtonBrushDX.Dispose(); cachedButtonBrushDX = null; }
				if (cachedButtonHBrushDX != null) { cachedButtonHBrushDX.Dispose(); cachedButtonHBrushDX = null; }
				if (cachedButtonFHBrushDX != null) { cachedButtonFHBrushDX.Dispose(); cachedButtonFHBrushDX = null; }
				if (cachedButtonFOFFBrushDX != null) { cachedButtonFOFFBrushDX.Dispose(); cachedButtonFOFFBrushDX = null; }
				if (cachedButtonFONBrushDX != null) { cachedButtonFONBrushDX.Dispose(); cachedButtonFONBrushDX = null; }
				if (cachedArrowUpBrushDX != null) { cachedArrowUpBrushDX.Dispose(); cachedArrowUpBrushDX = null; }
				if (cachedArrowDownBrushDX != null) { cachedArrowDownBrushDX.Dispose(); cachedArrowDownBrushDX = null; }
				if (cachedStopLineBrushDX != null) { cachedStopLineBrushDX.Dispose(); cachedStopLineBrushDX = null; }
				if (cachedTargetLineBrushDX != null) { cachedTargetLineBrushDX.Dispose(); cachedTargetLineBrushDX = null; }
				if (cachedPlotDownBrush33DX != null) { cachedPlotDownBrush33DX.Dispose(); cachedPlotDownBrush33DX = null; }
				if (cachedTimerBrushDX != null) { cachedTimerBrushDX.Dispose(); cachedTimerBrushDX = null; }
				if (cachedTableTextBrushDX != null) { cachedTableTextBrushDX.Dispose(); cachedTableTextBrushDX = null; }
				if (cachedFillUpBrushDX != null) { cachedFillUpBrushDX.Dispose(); cachedFillUpBrushDX = null; }
				if (cachedFillDownBrushDX != null) { cachedFillDownBrushDX.Dispose(); cachedFillDownBrushDX = null; }
				if (cachedFillNeutralBrushDX != null) { cachedFillNeutralBrushDX.Dispose(); cachedFillNeutralBrushDX = null; }
				if (cachedAreaBrushDX != null) { cachedAreaBrushDX.Dispose(); cachedAreaBrushDX = null; }
				if (cachedActiveOutlineBrushDX != null) { cachedActiveOutlineBrushDX.Dispose(); cachedActiveOutlineBrushDX = null; }
				if (cachedSmallAreaBrushDX != null) { cachedSmallAreaBrushDX.Dispose(); cachedSmallAreaBrushDX = null; }
				if (cachedOutlineUp03BrushDX != null) { cachedOutlineUp03BrushDX.Dispose(); cachedOutlineUp03BrushDX = null; }
				if (cachedOutlineDn03BrushDX != null) { cachedOutlineDn03BrushDX.Dispose(); cachedOutlineDn03BrushDX = null; }
				if (cachedOutlineBoth03BrushDX != null) { cachedOutlineBoth03BrushDX.Dispose(); cachedOutlineBoth03BrushDX = null; }
				if (cachedStop50BrushDX != null) { cachedStop50BrushDX.Dispose(); cachedStop50BrushDX = null; }
				if (cachedTarget50BrushDX != null) { cachedTarget50BrushDX.Dispose(); cachedTarget50BrushDX = null; }

				// --- Dispose cached TextFormats ---
				if (cachedLabelTextFormat != null) { cachedLabelTextFormat.Dispose(); cachedLabelTextFormat = null; }
				if (cachedTextFont2Format != null) { cachedTextFont2Format.Dispose(); cachedTextFont2Format = null; }
				if (cachedTextFontTimeFormat != null) { cachedTextFontTimeFormat.Dispose(); cachedTextFontTimeFormat = null; }
				if (cachedTextFontFormat != null) { cachedTextFontFormat.Dispose(); cachedTextFontFormat = null; }
				if (cachedTextFont8Format != null) { cachedTextFont8Format.Dispose(); cachedTextFont8Format = null; }
				if (cachedTableTextFormat != null) { cachedTableTextFormat.Dispose(); cachedTableTextFormat = null; }
				if (cachedButtonTextFormat != null) { cachedButtonTextFormat.Dispose(); cachedButtonTextFormat = null; }

			}


			else if (State == State.Historical)
			{
			
				if (!Permission)
					return;
			
				SetZOrder(10000);
				
//				if (ChartControl != null)
//				{
//					ChartControl.Dispatcher.InvokeAsync((Action)(() =>
//					{
//						InsertWPFControls();
//					}));
//				}
			}
				

		}

		
	private void ClearPlots ()
		{
			int BTH = 0;
			
			BTH = CurrentBars[0] - 1;
			
			int st = 1;
			//if (!AllCOBC())
				st = 0;
			
			for (int i = st; i < BTH; i++)
			{
				Values[0].Reset(i);	
				
			}
			//ChartControl.Refresh();
			
		}

        private void SetPlots(Series<double> In, Series<double> Out, int BB)
        {
            int BTH = 0;

            BTH = CurrentBars[0] - 1;

            int st = 1;
            //if (!AllCOBC())
            st = 0;

            BTH = BB;

			Values[2].Reset();
			
            //Print(Values[0].Count);
            //Print(Values[0][0]);
            //Print(Values[0].GetValueAt(1));

		//	Print(st + "  " + BTH);
			
            for (int i = st; i < BTH; i++)
            {
                
				//Out[i] = In[i];
				
				Print(In.GetValueAt(CurrentBars[0] - i));
				//Print(i);
				
				Values[2][i] = In.GetValueAt(CurrentBars[0] - i);
				
               // Values[0][i] = 0;

                //Print(i);

            }
            //ChartControl.Refresh();

        }	
		
		
       // private void SetBack(Series<double> In, Series<double> Out, int BB)
			
		
		private void SetAllTrend()
        {
		
			
			
			
			int BTH = 0;

            BTH = CurrentBars[0];

            int st = 1;
            //if (!AllCOBC())
            st = 0;

            //BTH = BB;

			//Values[2].Reset();
			
            //Print(Values[0].Count);
            //Print(Values[0][0]);
            //Print(Values[0].GetValueAt(1));

		//	Print(st + "  " + BTH);
			
            for (int i = st; i <= BTH; i++)
            {
                
				//Print(i);
				
				//BackBrushesAll.Set(i, Brushes.White);
				
				
					ReCalcBack(i);
				
				//SetOverallTrend(i);
				//SetBackground(i);
				
				//Out[i] = In[i];
				
				//Print(In.GetValueAt(CurrentBars[0] - i));
				//Print(i);
				
				//Values[2][i] = In.GetValueAt(CurrentBars[0] - i);
				
               // Values[0][i] = 0;

                //Print(i);

            }
            //ChartControl.Refresh();
			
			
			
			AllTrends.Clear();
			foreach (int ii in AllTrends2)
			{
				AllTrends.Add(ii);
			}
			
			
			bool IsLong = true;
			bool IsShort = true;
			
			foreach (int ii in AllTrends)
			{
				if (ii != 1) IsLong = false;
				if (ii != -1) IsShort = false;
			}
			
			CurrentTrendStatus = 0;
			if (AllTrends.Count == 0)
				CurrentTrendStatus = 0;
			else if (IsLong)
				CurrentTrendStatus = 1;			
			else if (IsShort)
				CurrentTrendStatus = -1;
			
			if (CurrentTrendStatusButton != CurrentTrendStatus)
				UpdateButtons();
						

        }		
		
		

			private void ReCalcBack(int bb)
			{
			
			//OverallTrend.GetValueAt(bb);
			//OverallTrend.Set(0,0);	
				
				int OverallTr = 0;
			

			
			bool IsLong = true;
			bool IsShort = true;
	
				AllTrends2.Clear();
			
			if (pFeed1Enabled && pFeed1Included)
				AllTrends2.Add(Direction1.GetValueAt(bb));
			
			if (pFeed2Enabled && pFeed2Included)
				AllTrends2.Add(Direction2.GetValueAt(bb));				
		
			if (pFeed3Enabled && pFeed3Included)
				AllTrends2.Add(Direction3.GetValueAt(bb));
			
			if (pFeed4Enabled && pFeed4Included)
				AllTrends2.Add(Direction4.GetValueAt(bb));	
						
			foreach (int ii in AllTrends2)
			{
				if (ii != 1) IsLong = false;
				if (ii != -1) IsShort = false;
			}
			
			if (AllTrends2.Count == 0)
				OverallTr = 0;
			else if (IsLong)
				OverallTr = 1;			
			else if (IsShort)
				OverallTr = -1;				
				
			
			
			

			if (bb == CurrentBars[0])
			{
				CurrentTrendStatus = OverallTr;
				//Print(CurrentTrendStatus);
			}
			
			

			
			if (pBackEnabled)
				{
					
					
					BackBrushes.Set(bb, null);
					BackBrushesAll.Set(bb, null);
					
					if (pBackTEnabled)
					{
					

						
						
						if (OverallTr == 1)
							//SetBackColor2(pBrush07, bb);
							SetBackColor3(pBrush07, bb);
						
						if (OverallTr == -1)
							//SetBackColor2(pBrush08, bb);
						SetBackColor3(pBrush08, bb);
						
					
					}
					
					if (pBackMEnabled)
					{
						if (IsInSession.GetValueAt(bb) != 1)
						{					
							//SetBackColor2(pBrush09, bb);
							SetBackColor3(pBrush09, bb);
							
						}	
					
					}
					
					
					if (pArrowsEnabled)
					if (pBackSEnabled)
					{
						
						if (Signals.GetValueAt(bb) == 1)
						{					
							SetBackColor3(pBrush01, bb);
							//BackBrushesAll.Set(bb, pBrush01);
							
						}				
						else if (Signals.GetValueAt(bb) == -1)
						{
							SetBackColor3(pBrush02, bb);
							//BackBrushesAll.Set(bb, pBrush02);
						}
					}
					

					
				}
				
			}
			
				
		
		
		
		//BackBrushesAll[0] = null;
		//		BackBrushes[0] = null;
				
		
		
		protected override void OnConnectionStatusUpdate(ConnectionStatusEventArgs connectionStatusUpdate)
		{
			//return;
			
			//ChartControl.Dispatcher.InvokeAsync(() =>
			//{
			
			if (ChartControl == null)
				return;
			
			
				if (connectionStatusUpdate.PriceStatus == ConnectionStatus.Connected
					&& connectionStatusUpdate.Connection.InstrumentTypes.Contains(Instrument.MasterInstrument.InstrumentType)
					&& Bars.BarsType.IsTimeBased
					&& Bars.BarsType.IsIntraday)
				{
					connected = true;

						//if (pTimerEnabled)
						//if (DisplayTime() && timer == null)
						
						
//					if (DisplayTime() && timer == null)
//					{
//						ChartControl.Dispatcher.InvokeAsync(() =>
//						{
//							timer			= new System.Windows.Threading.DispatcherTimer { Interval = new TimeSpan(0, 0, 1), IsEnabled = true };
//							timer.Tick		+= OnTimerTick;
//						});
//					}
											
										
	//					if (TestLoad && timer3 == null)
	//					{
	//						ChartControl.Dispatcher.InvokeAsync(() =>
	//						{
	//							timer3			= new System.Windows.Threading.DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 500), IsEnabled = true };
	//							timer3.Tick		+= OnTimerTick3;
	//						});
	//					}

										
									
				}
				else if (connectionStatusUpdate.PriceStatus == ConnectionStatus.Disconnected)
					connected = false;
			
				
				
				
			//});
		}

		
		
        private void TogglePlotExecutions()
        {

            if (ChartBars.Properties.PlotExecutions == ChartExecutionStyle.DoNotPlot)
            {

                ChartBars.Properties.PlotExecutions = ChartExecutionStyle.MarkersOnly;

            }
            else if (ChartBars.Properties.PlotExecutions == ChartExecutionStyle.MarkersOnly)
            {
                ChartBars.Properties.PlotExecutions = ChartExecutionStyle.TextAndMarker;


            }
            else
            {
                ChartBars.Properties.PlotExecutions = ChartExecutionStyle.DoNotPlot;


            }


        }

        protected void MiniBClick(object sender, RoutedEventArgs e)
        {

      
            string thissender = sender.ToString();

			
			
			
            ChartControl.InvalidateVisual();
        }


        protected void Buttons2Click(object sender, RoutedEventArgs e)
		{
			//Draw.TextFixed(this, "infobox", "Button 3 Clicked", TextPosition.BottomLeft, Brushes.DarkOrange, new Gui.Tools.SimpleFont("Arial", 25), Brushes.Transparent, Brushes.Transparent, 100);
			
			//Print(sender.ToString());
	
			
			ChartControl.InvalidateVisual();
		}		
		
		protected void CB1Click (object sender, RoutedEventArgs e)
		{
			if (cb1.SelectedIndex == 0)
				pThisEntryType = "Market";
			
			if (cb1.SelectedIndex == 1)
				pThisEntryType = "Limit";
			
			UpdateButtons();
			
			
			ChartControl.InvalidateVisual();
		}
		
		protected void OffsetBoxClick (object sender, RoutedEventArgs e)
		{

			pLimitOrderOffset = Math.Max(0,OffsetBox.Value);
			
			UpdateButtons();
			
			ChartControl.InvalidateVisual();
		}		
		
		
		protected void Button1Click(object sender, RoutedEventArgs e)
		{

			ChartControl.InvalidateVisual();
		}

		protected void Button2Click(object sender, RoutedEventArgs e)
		{

			ChartControl.InvalidateVisual();
		}

		private void ClosePosition()
		{
			instruction = OIF_ClosePosition(pAccountName, Instrument.FullName);

			if (instruction != string.Empty)
			{

				Submit();
			}
			
		}
		
		
		private void CancelOrder(Order o)
		{
			instruction = OIF_CancelOrder(o.OrderId,"");

			if (instruction != string.Empty)
			{

				Submit();
			}
			
		}
		
		protected void Button3Click(object sender, RoutedEventArgs e)
		{

            if (!ClickReady() && !pEntriesEnabled)
			{
				AddError("You must be connected to a data feed turn on the BOT2 system.");
				ChartControl.InvalidateVisual();
				
				return;
			}

			if (pEntriesEnabled)
			{
				pEntriesEnabled = false;
			
				if (LongEntryOrder != null)
					CancelOrder(LongEntryOrder);
					
				if (ShortEntryOrder != null)
					CancelOrder(ShortEntryOrder);

			}
			else
			{
				
//				if (!ExceededDG())
					pEntriesEnabled = true;
			}
			
			SetOrderParams();
			UpdateButtons();
			
			ChartControl.InvalidateVisual();
			
		}

		protected void Button4Click(object sender, RoutedEventArgs e)
		{
            //Draw.TextFixed(this, "infobox", "Button 4 Clicked", TextPosition.BottomLeft, Brushes.CadetBlue, new Gui.Tools.SimpleFont("Arial", 25), Brushes.Transparent, Brushes.Transparent, 100);

           // if (!ClickReady())
			//	return;
			

			if (pTrendOnlyEnabled)
			{
				pTrendOnlyEnabled = false;
				pLongEnabled = true;
				pShortEnabled = false;
			}
			else if (pLongEnabled && !pShortEnabled)
			{
				pLongEnabled = false;
				pShortEnabled = true;			
			}		
			else if (!pLongEnabled && pShortEnabled)
			{
				pLongEnabled = true;
				pShortEnabled = true;
			}
			else if (pLongEnabled && pShortEnabled)
			{
				if (pSecondaryFeedsEnabled)
					pTrendOnlyEnabled = true;
				else
				{
					pLongEnabled = true;
					pShortEnabled = false;	
				}
					
				
			}
			
			UpdateButtons();
			
			ChartControl.InvalidateVisual();
		}
		
		
		private void UpdateDisplay()
		{
							
				// STATUS AREA
		
				if (!pEntriesEnabled)
				{
					newButton5.Content = "BOT Off";
					newButton5.Background = pButtonColorOff;
					//status.ForeColor = Color.Black;
				}			
				else if (DoLong)
				{
					newButton5.Content = "New Long Ready";
					newButton5.Background = pButtonColorLong;
				}
				else if (DoShort)
				{
					newButton5.Content = "New Short Ready";
					newButton5.Background = pButtonColorShort;
				}	
				else if (pTrendOnlyEnabled)
				{
					newButton5.Content = "Waiting For Trend Trades";
					newButton5.Background = pButtonColorOn;
				}
				else if (pLongEnabled && pShortEnabled)
				{
					newButton5.Content = "Waiting For All Trades";
					newButton5.Background = pButtonColorOn;
				}
				else if (pLongEnabled)
				{
					newButton5.Content = "Waiting For Long Trades";
					newButton5.Background = pButtonColorOn;
				}			
				else if (pShortEnabled)
				{
					newButton5.Content = "Waiting For Short Trades";
					newButton5.Background = pButtonColorOn;
				}			
				
		}
		
		
		
		private void UpdateButtons()
		{
			if (ChartControl != null)
			ChartControl.Dispatcher.InvokeAsync((Action)(() =>
			{
				
							
				CheckExecutionStatus();
			
				
				// ENTRY BUTTON
				
				if (pEntriesEnabled)
				{
					newButton3.Content = "BOT On";
					newButton3.Background = pButtonColorOn;
				}
				else
				{
					newButton3.Content = "BOT Off";
					newButton3.Background = pButtonColorOff;
				}
				
				

				// TRADES BUTTON
				
				CurrentTrendStatusButton = CurrentTrendStatus;
				
				if (pTrendOnlyEnabled)
				{
					newButton4.Content = "Trend Only";
					
					if (AllTrends.Count == 0)
						newButton4.Background = pButtonColorOn;						
					else if (CurrentTrendStatusButton == 0)
						newButton4.Background = pButtonColorOff;	
					else if (CurrentTrendStatusButton == 1)
						newButton4.Background = pButtonColorLong;	
					else if (CurrentTrendStatusButton == -1)
						newButton4.Background = pButtonColorShort;						
					
				}
				else if (pLongEnabled && pShortEnabled)
				{
					newButton4.Content = "All Trades";
					newButton4.Background = pButtonColorOn;
				}
				else if (pLongEnabled)
				{
					newButton4.Content = "Long Only";	
					newButton4.Background = pButtonColorLong;
				}
				else
				{
					newButton4.Content = "Short Only";
					newButton4.Background = pButtonColorShort;
				}		
				

				
				
				// OffsetBox fade and combo bbox
				
				if (pThisEntryType == "Market")
				{
					cb1.SelectedIndex = 0;
					OffsetBox.IsEnabled = false;
				}
				if (pThisEntryType == "Limit")
				{
					cb1.SelectedIndex = 1;
					OffsetBox.IsEnabled = true;
				}
				
				
				
				if (pAutoEnabled)
				{
					newButton7.Content = "Auto On";
					newButton7.Background = pButtonColorOn;
					
				}
				else
				{
					newButton7.Content = "Auto Off";
					newButton7.Background = pButtonColorOff;
				}
					
		
				
				if (pExitOrdersEnabled)
				{
					newButton8.Content = "Exits On";
					newButton8.Background = pButtonColorOn;
				}
				else
				{
					newButton8.Content = "Exits Off";
					newButton8.Background = pButtonColorOff;
				}
							
				
				
				
				
				if (pSLTrailOrdersEnabled)
				{
					newButton9.Content = "Trail On";
					newButton9.Background = pButtonColorOn;
				}
				else
				{
					newButton9.Content = "Trail Off";
					newButton9.Background = pButtonColorOff;
				}		
				
		
			}));
			
			
		}
		
		protected void Button5Click(object sender, RoutedEventArgs e)
		{


		}
				
		protected void Button6Click(object sender, RoutedEventArgs e)
		{
            //Draw.TextFixed(this, "infobox", "Button 4 Clicked", TextPosition.BottomLeft, Brushes.CadetBlue, new Gui.Tools.SimpleFont("Arial", 25), Brushes.Transparent, Brushes.Transparent, 100);


            ChartControl.InvalidateVisual();
		}
		
		protected void Button7Click(object sender, RoutedEventArgs e)
		{
		
			if (pAutoEnabled)
			{
				pAutoEnabled = false;
			}
			else
			{
				pAutoEnabled = true;
			}
				
			UpdateButtons();

            ChartControl.InvalidateVisual();
		}
				
		protected void Button8Click(object sender, RoutedEventArgs e)
		{

			if (pExitOrdersEnabled)
			{
				pExitOrdersEnabled = false;
			}
			else
			{
				
				//Print("ATM: " + pATMName);
				
				if (pATMName == "")
				{
					AddError("You must have an ATM Strategy selected for Stop Loss and Target orders to be placed.");
					ChartControl.InvalidateVisual();
					return;
				}
				
				pExitOrdersEnabled = true;
				
				if (myAccount != null)
				{
				
					UpdateStopLoss("IN");
					UpdateProfitTargets("Target1");
					UpdateProfitTargets("Target2");		
					
				}
						
						
			}						
			
			UpdateButtons();
			
           
		}
		
		
		protected void Button9Click(object sender, RoutedEventArgs e)
		{

			if (pSLTrailOrdersEnabled)
			{
				pSLTrailOrdersEnabled = false;
			}
			else
			{
				if (pATMName == "")
				{
					AddError("You must have an ATM Strategy selected for Stop Loss orders to be placed.");
					ChartControl.InvalidateVisual();
					return;
				}
				
				pSLTrailEnabled = true;
				pSLTrailOrdersEnabled = true;

//				if (pSLTrailEnabled)
//				{
//					pSLTrailOrdersEnabled = true;
//				}
//				else
//				{
					
//					//AddError("In the indicator settings, Exit Orders Display section, you must have 'Trailing Stop Loss Enabled' checked. ");
//					ChartControl.InvalidateVisual();
//					return;						
					
//				}
				
				if (myAccount != null)
				{
				
					UpdateStopLoss("TR");

				}
						
						
			}						
			
			UpdateButtons();
			
            ChartControl.InvalidateVisual();
		}
	
		
		
		protected void OnNewLogEvent(object sender, LogEventArgs e)
		{
			//Print(e.Message);
			
			if (ChartControl != null)			
			{
						
				if (pDisabledOnCloseButton)
				if (e.Message.Contains("Chart Trader close position"))
				{
					DisableTheSystem();
					return;
			
				}
				
				
			
				if (e.Message.Contains("must be equal"))
				{
					string ErrorCode = e.Message.Split(new string[]{"order"}, StringSplitOptions.None).Last();
//					string s = e.Message;
//					int index = s.indexOf("order");
//					if(index != -1)
//					{
//					  //DO YOUR LOGIC
//					  string errorCode = s.Substring(index+4);
//					}

					string finalstring = "Your entry order" + ErrorCode + ".";

					
//					ssdasf = "Main Order quantity '" + pQty + "' must be equal to or greater than ATM Strategy quantity '" + chartTraderATM.SelectedAtmStrategy.EntryQuantity + ".  The Main Order quantity was automatically increased.";
						
					
									
//									chartTraderQty.Value = chartTraderATM.SelectedAtmStrategy.EntryQuantity;
//									AddError(ssdasf);
									
									
					
					
					finalstring = finalstring.Replace("strategy", "ATM Strategy");
					
					AddError(finalstring);
					ChartControl.InvalidateVisual();
					ChartControl.InvalidateVisual();
				}
					
			}
	
			
		}		
        protected void CTQtyUpdate(object sender, RoutedEventArgs e)
		{
			//Print(chartTraderQty.Value);
			
			SetOrderParams();
			
			ChartControl.InvalidateVisual();
		}
		
        protected void CloseButtonClick(object sender, RoutedEventArgs e)
		{
			//Print("CLOSE");
			
			
			
			ChartControl.InvalidateVisual();
		}		
		
		
		protected void InsertWPFControls()
		{
			
            //find chart trader from parent chart by it's automation id "ChartWindowChartTrader"
			

            chartTrader = Window.GetWindow(ChartControl.Parent).FindFirst("ChartWindowChartTraderControl") as ChartTrader;

			if (chartTrader == null)
			{
				Print("Chart Trader null");
				return;
			}
            //Print(chartTrader.Width);
			
			
           //chartTrader.Width = pCTWidth;

            //chartTrader.ChartTraderVisibility = ChartTraderVisibility.Visible;


           // chartTrader.Background = pColorCTB;

            

            if (chartTrader == null)
            {



                //Print("ChartTrader is null");
                //return;
            }
            else
            {

				chartTrader.MouseDown += new MouseButtonEventHandler(CTMouseDown);
				chartTrader.MouseMove += new MouseEventHandler(CTMouseMove);
	
					

                chartTraderQty = chartTrader.FindFirst("ChartTraderControlQuantitySelector") as NinjaTrader.Gui.Tools.QuantityUpDown;

				//chartTraderQty.Minimum = -100;
				
				chartTraderATM = chartTrader.FindFirst("ChartTraderControlATMStrategySelector") as NinjaTrader.Gui.NinjaScript.AtmStrategy.AtmStrategySelector;
				
				chartTraderTIF = chartTrader.FindFirst("ChartTraderControlTIFSelector") as NinjaTrader.Gui.Tools.TifSelector;
				
				chartTraderAcct = chartTrader.FindFirst("ChartTraderControlAccountSelector") as NinjaTrader.Gui.Tools.AccountSelector;

				//ThisCloseButton = chartTrader.FindFirst("ChartTraderControlButtonClose") as System.Windows.Controls.Button;
				
				//chartTrader. += CloseButtonClick;
				
                //ChartTraderProperties aaaa = chartTrader.FindFirst("ChartTraderProperties") as ChartTraderProperties;

			
//                chartTraderQty.ValueChanged += CTQtyUpdate;
//				chartTraderATM.SelectionChanged += CTQtyUpdate;
//				chartTraderTIF.SelectionChanged += CTQtyUpdate;
//				chartTraderAcct.SelectionChanged += CTQtyUpdate;

				NinjaTrader.Cbi.Log.LogEvent += OnNewLogEvent; 

//NinjaTrader.NinjaScript.AtmStrategy.StartAtmStrategy(selector.SelectedAtmStrategy, entryOrder);
				


                //chartTraderQty.Value = 2; // set value



                SetOrderParams();

               // Print("ChartTrader is ok");
             //   return;
            }
						
						
			chartTraderGrid = (Window.GetWindow(ChartControl.Parent).FindFirst("ChartWindowChartTraderControl") as ChartTrader).Content as System.Windows.Controls.Grid;
			
			chartTraderButtonsGrid = chartTraderGrid.Children[0] as System.Windows.Controls.Grid;
			


            


			myButtonsGrid1 = new System.Windows.Controls.Grid();

            chartTraderGrid.Children.Add(myButtonsGrid1);

			// add a new space for our custom grid below the ask and bid prices
			chartTraderGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(40+pChartTraderSpace) });
			
				// add a new space for our custom grid below the ask and bid prices
			chartTraderGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(34) });
            chartTraderGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(30) });

			
			
			
            System.Windows.Controls.Grid.SetRow(myButtonsGrid1, 8);

			
			// add rows below PnL
			
//			chartTraderButtonsGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(30)	});
//			chartTraderButtonsGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(1)	});
//			chartTraderButtonsGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(30)	});
//			chartTraderButtonsGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(5)	});
			
			
			
			// spacer row
			myButtonsGrid1.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(3) });
			myButtonsGrid1.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(30) });
//			myButtonsGrid1.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(1) });
//			myButtonsGrid1.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(30) });
//			myButtonsGrid1.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(1) });
//			myButtonsGrid1.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(30) });
//			myButtonsGrid1.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(1) });
//			myButtonsGrid1.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(30) });
//			myButtonsGrid1.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(1) });
//			myButtonsGrid1.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(30) });		
			
			
			myButtonsGrid1.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(97) });
			// spacer column
			myButtonsGrid1.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(6) });
			myButtonsGrid1.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(97) });



			
			

						
			myButtonsGrid2 = new System.Windows.Controls.Grid();
			chartTraderGrid.Children.Add(myButtonsGrid2);
		
            	
			
			System.Windows.Controls.Grid.SetRow(myButtonsGrid2, 9);
			
			// spacer row
			myButtonsGrid2.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(1) });
			myButtonsGrid2.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(24) });
//			myButtonsGrid2.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(1) });
//			myButtonsGrid2.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(30) });
//			myButtonsGrid2.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(1) });
//			myButtonsGrid2.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(30) });
//			myButtonsGrid2.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(1) });
//			myButtonsGrid2.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(30) });
			
			int widsfsf = 63;
			
			myButtonsGrid2.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(widsfsf+1) });
			// spacer column
			myButtonsGrid2.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(4) });
			myButtonsGrid2.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(widsfsf) });
			myButtonsGrid2.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(4) });
			myButtonsGrid2.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(widsfsf+1) });
//			myButtonsGrid2.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(0) });
//			myButtonsGrid2.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(21) });
//            myButtonsGrid2.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(6) });



			// row of buttons

            myButtonsGrid3 = new System.Windows.Controls.Grid();
            chartTraderGrid.Children.Add(myButtonsGrid3);

			
			
			
            System.Windows.Controls.Grid.SetRow(myButtonsGrid3, 7);

			int buttonw = 30;
			int width22 = buttonw+2; // column width
			int spacer = 4;
			
            // spacer row
            myButtonsGrid3.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(12+pChartTraderSpace) });
            myButtonsGrid3.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(30) });
       
            myButtonsGrid3.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(200) });
//           myButtonsGrid3.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(spacer) });
//            myButtonsGrid3.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(width22) });
//           myButtonsGrid3.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(spacer) });
//            myButtonsGrid3.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(width22) });
//            myButtonsGrid3.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(spacer) });
//            myButtonsGrid3.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(width22) });
//            myButtonsGrid3.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(spacer) });
//            myButtonsGrid3.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(width22) });
//            myButtonsGrid3.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(spacer) });
//            myButtonsGrid3.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(width22) });

            //			newButton21 = new System.Windows.Controls.Button()
            //			{
            //				Background = ButtonBrush,
            //				BorderBrush = ButtonBrush2,
            //				Content = "BS",
            //				Height = buttonh
            //			};


            //			newButton21.Height = 30;

            //			newButton21.Click += Buttons2Click;
            //			System.Windows.Automation.AutomationProperties.SetAutomationId(newButton21, "newButton21");
            //			myButtonsGrid2.Children.Add(newButton21);
            //			System.Windows.Controls.Grid.SetRow(newButton21, 1);
            //			System.Windows.Controls.Grid.SetColumn(newButton21, 0);



			int aaaa = 80;



			
//            newButton1 = new System.Windows.Controls.Button()
//			{
//				Background = ButtonBrush,
//				BorderBrush = pButtonColorOutline,
//				Content = "Buy Click",
//				Height = buttonh,
//				//Width = aaaa,
//                MaxWidth = 200,
//                MinWidth = 0
//			};

//			newButton2 = new System.Windows.Controls.Button()
//			{
//				Background = ButtonSBrush,
//				BorderBrush = pButtonColorOutline,
//				Content = "Sell Click",
//				Height = buttonh,
//				//Width = aaaa,
//                MaxWidth = 200,
//                MinWidth = 0
//			};

			newButton3 = new System.Windows.Controls.Button()
			{
				Background = ButtonBrush,
				BorderBrush = pButtonColorOutline,
				Content = "Buy Close",
				Height = buttonh,
				//Width = aaaa,
                MaxWidth = 200,
                MinWidth = 0
			};

			newButton4 = new System.Windows.Controls.Button()
			{
				Background = pButtonColorOff,
				BorderBrush = pButtonColorOutline,
				Content = "Sell Close",
				Height = buttonh,
				//Width = aaaa,
                MaxWidth = 200,
                MinWidth = 0
			};
			
			newButton5 = new System.Windows.Controls.Button()
			{
				Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(30,30,30)),
				BorderBrush = pButtonColorOutline,
				Content = "Trade Messages",
				Height = buttonh
			};

//			newButton6 = new System.Windows.Controls.Button()
//			{
//				Background = Button3Brush,
//				BorderBrush = pButtonColorOutline,
//				Content = "Attach Clear",
//				Height = buttonh
//			};
			
			newButton7 = new System.Windows.Controls.Button()
			{
				Background = Button3Brush,
				BorderBrush = pButtonColorOutline,
				Content = "Auto",
				Height = 21,
                Width = widsfsf+1,
                MaxWidth = 200,
                MinWidth = 0
            };

            newButton8 = new System.Windows.Controls.Button()
            {
                Background = Button3Brush,
                BorderBrush = pButtonColorOutline,
                Content = "R",
                Height = 21,
				
                Width = widsfsf,
                MaxWidth = 200,
                MinWidth = 0
            };

            newButton9 = new System.Windows.Controls.Button()
            {
                Background = Button3Brush,
                BorderBrush = pButtonColorOutline,
                Content = "R",
                Height = 21,
				
                Width = widsfsf,
                MaxWidth = 200,
                MinWidth = 0
            };
			
			
			
			newButton7.FontSize = 12;
			newButton8.FontSize = 12;
			newButton9.FontSize = 12;
			newButton7.Padding = new Thickness(0, 0, 0, 0);
			newButton8.Padding = new Thickness(0, 0, 0, 0);
			newButton9.Padding = new Thickness(0, 0, 0, 0);
			
			
//            miniB1 = new System.Windows.Controls.Button()
//            {
//                Background = Button3Brush,
//                BorderBrush = pButtonColorOutline,
//                Content = "1",
//                Height = buttonh,
//                Width = buttonw,
//                MaxWidth = 200,
//                MinWidth = 0
//            };

//            miniB2 = new System.Windows.Controls.Button()
//            {
//                Background = Button3Brush,
//                BorderBrush = pButtonColorOutline,
//                Content = "2",
//                Height = buttonh,
//                Width = buttonw,
//                MaxWidth = 200,
//                MinWidth = 0
//            };

//            miniB3 = new System.Windows.Controls.Button()
//            {
//                Background = Button3Brush,
//                BorderBrush = pButtonColorOutline,
//                Content = "3",
//                Height = buttonh,
//                Width = buttonw,
//                MaxWidth = 200,
//                MinWidth = 0
//            };

//            miniB4 = new System.Windows.Controls.Button()
//            {
//                Background = Button3Brush,
//                BorderBrush = pButtonColorOutline,
//                Content = "4",
//                Height = buttonh,
//                Width = buttonw,
//                MaxWidth = 200,
//                MinWidth = 0
//            };

//            miniB5 = new System.Windows.Controls.Button()
//            {
//                Background = Button3Brush,
//                BorderBrush = pButtonColorOutline,
//                Content = "5",
//                Height = buttonh,
//                Width = buttonw,
//                MaxWidth = 200,
//                MinWidth = 0
//            };

//            miniB6 = new System.Windows.Controls.Button()
//            {
//                Background = Button3Brush,
//                BorderBrush = pButtonColorOutline,
//                Content = "6",
//                Height = buttonh,
//                Width = buttonw,
//                MaxWidth = 200,
//                MinWidth = 0
//            };


//            miniB1.Click += MiniBClick;
//            System.Windows.Automation.AutomationProperties.SetAutomationId(miniB1, "miniB1");
//            myButtonsGrid3.Children.Add(miniB1);
//            System.Windows.Controls.Grid.SetColumn(miniB1, 0);
//            System.Windows.Controls.Grid.SetRow(miniB1, 1);

//            miniB2.Click += MiniBClick;
//            System.Windows.Automation.AutomationProperties.SetAutomationId(miniB2, "miniB2");
//            myButtonsGrid3.Children.Add(miniB2);
//            System.Windows.Controls.Grid.SetColumn(miniB2, 2);
//            System.Windows.Controls.Grid.SetRow(miniB2, 1);

//            miniB3.Click += MiniBClick;
//            System.Windows.Automation.AutomationProperties.SetAutomationId(miniB3, "miniB3");
//            myButtonsGrid3.Children.Add(miniB3);
//            System.Windows.Controls.Grid.SetColumn(miniB3, 4);
//            System.Windows.Controls.Grid.SetRow(miniB3, 1);

//            miniB4.Click += MiniBClick;
//            System.Windows.Automation.AutomationProperties.SetAutomationId(miniB4, "miniB4");
//            myButtonsGrid3.Children.Add(miniB4);
//            System.Windows.Controls.Grid.SetColumn(miniB4, 6);
//            System.Windows.Controls.Grid.SetRow(miniB4, 1);

//            miniB5.Click += MiniBClick;
//            System.Windows.Automation.AutomationProperties.SetAutomationId(miniB5, "miniB5");
//            myButtonsGrid3.Children.Add(miniB5);
//            System.Windows.Controls.Grid.SetColumn(miniB5, 8);
//            System.Windows.Controls.Grid.SetRow(miniB5, 1);

//            miniB6.Click += MiniBClick;
//            System.Windows.Automation.AutomationProperties.SetAutomationId(miniB6, "miniB6");
//            myButtonsGrid3.Children.Add(miniB6);
//            System.Windows.Controls.Grid.SetColumn(miniB6, 10);
//            System.Windows.Controls.Grid.SetRow(miniB6, 1);





//            newButton1.Click += Button1Click;
//			System.Windows.Automation.AutomationProperties.SetAutomationId(newButton1, "newButton1");
//			chartTraderButtonsGrid.Children.Add(newButton1);
//			System.Windows.Controls.Grid.SetRow(newButton1, 11);
//			System.Windows.Controls.Grid.SetColumn(newButton1, 0);


//			newButton2.Click += Button2Click;
//			System.Windows.Automation.AutomationProperties.SetAutomationId(newButton2, "newButton2");
//			chartTraderButtonsGrid.Children.Add(newButton2);
//			System.Windows.Controls.Grid.SetRow(newButton2, 11);
//			System.Windows.Controls.Grid.SetColumn(newButton2, 2);

			

//			newButton3.Click += Button3Click;
//			System.Windows.Automation.AutomationProperties.SetAutomationId(newButton3, "newButton3");
//			chartTraderButtonsGrid.Children.Add(newButton3);
//			System.Windows.Controls.Grid.SetRow(newButton3, 13);
//			System.Windows.Controls.Grid.SetColumn(newButton3, 0);

//			newButton4.Click += Button4Click;
//			System.Windows.Automation.AutomationProperties.SetAutomationId(newButton4, "newButton4");
//			chartTraderButtonsGrid.Children.Add(newButton4);
//			System.Windows.Controls.Grid.SetRow(newButton4, 13);
//			System.Windows.Controls.Grid.SetColumn(newButton4, 2);
			
			
			
			

			newButton3.Click += Button3Click;
			System.Windows.Automation.AutomationProperties.SetAutomationId(newButton3, "newButton3");
			myButtonsGrid1.Children.Add(newButton3);
			System.Windows.Controls.Grid.SetColumn(newButton3, 0);
			System.Windows.Controls.Grid.SetRow(newButton3, 1);

			newButton4.Click += Button4Click;
			System.Windows.Automation.AutomationProperties.SetAutomationId(newButton4, "newButton4");
			myButtonsGrid1.Children.Add(newButton4);
			System.Windows.Controls.Grid.SetColumn(newButton4, 2);
			System.Windows.Controls.Grid.SetRow(newButton4, 1);
			
			
			
			


//			newButton6.Click += Button6Click;
//			System.Windows.Automation.AutomationProperties.SetAutomationId(newButton6, "newButton6");
//			myButtonsGrid1.Children.Add(newButton6);
//			System.Windows.Controls.Grid.SetColumn(newButton6, 2);
//			System.Windows.Controls.Grid.SetRow(newButton6, 3);
		
			
			cb1 = new System.Windows.Controls.ComboBox()
			{
				//Background = ButtonBrush,
				//BorderBrush = ButtonBrush2,
				//Content = "Buy Click",
				//Height = buttonh,
				//Width = aaaa,
                MaxWidth = 200,
                MinWidth = 0
			};
			
			cb1.Items.Add("Market");
			cb1.Items.Add("Limit");
			
			
			cb1.SelectionChanged += CB1Click;
			System.Windows.Automation.AutomationProperties.SetAutomationId(cb1, "cb1");
			//myButtonsGrid2.Children.Add(cb1);
			

			
			System.Windows.Controls.Grid.SetColumn(cb1, 0);
			System.Windows.Controls.Grid.SetRow(cb1, 1);			
			
			
			OffsetBox = new NinjaTrader.Gui.Tools.QuantityUpDown();
			OffsetBox.Value = pLimitOrderOffset;
			OffsetBox.Height = 21;
			OffsetBox.Width = 54;
			OffsetBox.ValueChanged += OffsetBoxClick;
			//myButtonsGrid2.Children.Add(OffsetBox);
			System.Windows.Controls.Grid.SetColumn(OffsetBox, 2);
			System.Windows.Controls.Grid.SetRow(OffsetBox, 1);		
			
			
			
			
			newButton7.Click += Button7Click;
			System.Windows.Automation.AutomationProperties.SetAutomationId(newButton7, "newButton7");
			myButtonsGrid2.Children.Add(newButton7);
			System.Windows.Controls.Grid.SetColumn(newButton7, 4);
			System.Windows.Controls.Grid.SetRow(newButton7, 1);
			
			
			if (pSLTrailEnabled)
			{
			
				newButton9.Click += Button9Click;
				System.Windows.Automation.AutomationProperties.SetAutomationId(newButton8, "newButton9");
				myButtonsGrid2.Children.Add(newButton9);
				System.Windows.Controls.Grid.SetColumn(newButton9, 2);
				System.Windows.Controls.Grid.SetRow(newButton9, 1);
			}
			else
			{
				pSLTrailOrdersEnabled = false;
			}

				
				newButton8.Click += Button8Click;
				System.Windows.Automation.AutomationProperties.SetAutomationId(newButton8, "newButton8");
				myButtonsGrid2.Children.Add(newButton8);
				System.Windows.Controls.Grid.SetColumn(newButton8, 0);
				System.Windows.Controls.Grid.SetRow(newButton8, 1);


			

//			widelabel = new System.Windows.Controls.Label();
//			widelabel.Name = "safsag";
//			myButtonsGrid3.Children.Add(widelabel);
//			System.Windows.Controls.Grid.SetColumn(widelabel, 0);
//			System.Windows.Controls.Grid.SetRow(widelabel, 0);			
			
			newButton5.Click += Button5Click;
			System.Windows.Automation.AutomationProperties.SetAutomationId(newButton5, "newButton5");
			myButtonsGrid3.Children.Add(newButton5);
			System.Windows.Controls.Grid.SetColumn(newButton5, 0);
			System.Windows.Controls.Grid.SetRow(newButton5, 1);
			
			UpdateButtons();
			
        }

		protected void RemoveWPFControls()
		{

//			if (ChartControl == null)
//				return;
			
			
//			ChartControl.Dispatcher.InvokeAsync((Action)(() =>
//			{
			
			
			
			if (chartTrader != null)
				
				
				chartTrader.MouseDown -= new MouseButtonEventHandler(CTMouseDown);
				chartTrader.MouseMove -= new MouseEventHandler(CTMouseMove);		
			
			
			
					

//			chartTraderQty.ValueChanged -= CTQtyUpdate;
//			chartTraderATM.SelectionChanged -= CTQtyUpdate;
//			chartTraderTIF.SelectionChanged -= CTQtyUpdate;
//			chartTraderAcct.SelectionChanged -= CTQtyUpdate;
			
//			chartTraderQty = null;
//			chartTraderATM = null;
//			chartTraderTIF = null;
//			chartTraderAcct = null;
			
			
			
				//ThisCloseButton.Click -= CloseButtonClick;

			NinjaTrader.Cbi.Log.LogEvent -= OnNewLogEvent;

			
            //chartTrader.Width = 212;


            if (myAccount != null)
			  {
			      // Unsubscribe to any prior account subscriptions
			      myAccount.AccountItemUpdate -= OnAccountItemUpdate;
				 // myAccount.StateChanged -= OnAccountStatusUpdate;
			      myAccount.ExecutionUpdate -= OnExecutionUpdate;
			      myAccount.OrderUpdate -= OnOrderUpdate;
			      myAccount.PositionUpdate -= OnPositionUpdate;
			 
			  }
			  
			 
//			  }));
			
			
			
//			  Print("1");
			  
//			if (cb1 != null)
//			{
//				cb1.SelectionChanged -= CB1Click;
//				myButtonsGrid2.Children.Remove(cb1);
//			}
			
//			if (OffsetBox != null)
//			{
//				OffsetBox.ValueChanged -= OffsetBoxClick;
//				myButtonsGrid2.Children.Remove(OffsetBox);
//			}
			
			
			
			
//			if (newButton1 != null)
//			{
//				newButton1.Click -= Button1Click;
//				chartTraderButtonsGrid.Children.Remove(newButton1);
//			}

//			if (newButton2 != null)
//			{
//				newButton2.Click -= Button2Click;
//				chartTraderButtonsGrid.Children.Remove(newButton2);
//			}
			  
//			if (newButton6 != null)
//			{
//				newButton6.Click -= Button6Click;
//				myButtonsGrid1.Children.Remove(newButton6);
//			}
			  
			if (newButton3 != null)
			{
				newButton3.Click -= Button3Click;
				myButtonsGrid1.Children.Remove(newButton3);
			}

			if (newButton4 != null)
			{
				newButton4.Click -= Button4Click;
				myButtonsGrid1.Children.Remove(newButton4);
			}
			
			if (newButton5 != null)
			{
				newButton5.Click -= Button5Click;
				myButtonsGrid3.Children.Remove(newButton5);
			}


			
			if (newButton7 != null)
			{
				newButton7.Click -= Button7Click;
				myButtonsGrid2.Children.Remove(newButton7);
			}

			
//			Print("2");
			
			
			
			if (newButton8 != null)
			{
				newButton8.Click -= Button8Click;
				myButtonsGrid2.Children.Remove(newButton8);
			}
			
			if (newButton9 != null)
			{
				newButton9.Click -= Button9Click;
				myButtonsGrid2.Children.Remove(newButton9);
			}			
//			if (newButton21 != null)
//			{
//				newButton21.Click -= Buttons2Click;
//				myButtonsGrid1.Children.Remove(newButton21);
//			}
			

//			if (chartTraderButtonsGrid != null)
			{
//				chartTraderButtonsGrid.RowDefinitions.RemoveAt(chartTraderButtonsGrid.RowDefinitions.Count - 1);
//				chartTraderButtonsGrid.RowDefinitions.RemoveAt(chartTraderButtonsGrid.RowDefinitions.Count - 1);
//				chartTraderButtonsGrid.RowDefinitions.RemoveAt(chartTraderButtonsGrid.RowDefinitions.Count - 1);
//				chartTraderButtonsGrid.RowDefinitions.RemoveAt(chartTraderButtonsGrid.RowDefinitions.Count - 1);
				
			}

			if (chartTraderGrid != null && myButtonsGrid1 != null)
			{
				chartTraderGrid.Children.Remove(myButtonsGrid1);
				chartTraderGrid.RowDefinitions.RemoveAt(chartTraderGrid.RowDefinitions.Count - 1);
			}
			


			if (chartTraderGrid != null && myButtonsGrid2 != null)
			{
				chartTraderGrid.Children.Remove(myButtonsGrid2);
				chartTraderGrid.RowDefinitions.RemoveAt(chartTraderGrid.RowDefinitions.Count - 1);
			}

            if (chartTraderGrid != null && myButtonsGrid3 != null)
            {
                chartTraderGrid.Children.Remove(myButtonsGrid3);
                chartTraderGrid.RowDefinitions.RemoveAt(chartTraderGrid.RowDefinitions.Count - 1);
            }
            

//			Print("3");
			
			
//            if (miniB1 != null)
//            {
//                miniB1.Click -= MiniBClick;
//                myButtonsGrid3.Children.Remove(miniB1);
//            }
//            if (miniB2 != null)
//            {
//                miniB2.Click -= MiniBClick;
//                myButtonsGrid3.Children.Remove(miniB2);
//            }
//            if (miniB3 != null)
//            {
//                miniB3.Click -= MiniBClick;
//                myButtonsGrid3.Children.Remove(miniB3);
//            }
//            if (miniB4 != null)
//            {
//                miniB4.Click -= MiniBClick;
//                myButtonsGrid3.Children.Remove(miniB4);
//            }
//            if (miniB5 != null)
//            {
//                miniB5.Click -= MiniBClick;
//                myButtonsGrid3.Children.Remove(miniB5);
//            }
//            if (miniB6 != null)
//            {
//                miniB6.Click -= MiniBClick;re
//                myButtonsGrid3.Children.Remove(miniB6);
//            }



        }
 
        void AddButton()
        {
            //sampleButton = new Button
            //{
            //    Content = "Sample Button",
            //    Style = System.Windows.Application.Current.TryFindResource("Button") as Style
            //};

            //sampleButton.Click += SampleButton_Click;
            //AutomationProperties.SetAutomationId(sampleButton, "SampleButton");

            ////this is the main chart trader grid where the default buttons and controls reside
            //mainGrid = chartTrader.FindName("grdMain") as Grid;

        }

		private void Submit()
		{
			if (instruction != string.Empty)
    		{
    			LaunchNumber = OIF_Submit(instruction, LaunchNumber); 
				
				
				
    			instruction = string.Empty;
			}
 		}
		
		#region OIF methods

//=====================================================================
		private int OIF_Submit (string instruction, int OIFnumber)
		{  
			if(OIFnumber > 500) OIFnumber=0; else OIFnumber++;
			string fname = String.Concat(OIF_file_name, OIFnumber.ToString("0"), ".txt");
			System.IO.File.AppendAllText(fname, instruction);

			return OIFnumber;
		}
//=====================================================================
		private string OIF_CancelOrder (string OrderId, string StrategyId)
		{  
			if(OrderId.Length==0) return string.Empty;
			else
				return "CANCEL;;;;;;;;;;"+OrderId+";;"+StrategyId+Environment.NewLine;
		}
		private string OIF_CancelAllOrders ()
		{  
			return "CANCELALLORDERS;;;;;;;;;;;;"+Environment.NewLine;
		}

		private string OIF_ChangeOrder (int Qty, double LimitPrice, double StopPrice, string OrderId, string StrategyId)
		{  
			if(LimitPrice < 0 || StopPrice < 0 || OrderId.Length==0) return string.Empty;
			//else
				return "CHANGE;;;;"+Qty.ToString()+";;"+LimitPrice.ToString().Replace(",",".")+";"+StopPrice.ToString().Replace(",",".")+";;;"+OrderId+";;"+StrategyId+Environment.NewLine;
		}

		private string OIF_ClosePosition (string Account, string Instrument)
		{ 
			if(Account.Length == 0 || Instrument.Length==0) return string.Empty;
			else
				return "CLOSEPOSITION;"+Account+";"+Instrument+";;;;;;;;;;"+Environment.NewLine;
		}
		private string OIF_CloseStrategy (string StrategyId)
		{
			return "CLOSESTRATEGY;;;;;;;;;;;;"+StrategyId+Environment.NewLine;
		}
		private string OIF_FlattenEverything ()
		{
			return "FLATTENEVERYTHING;;;;;;;;;;;;"+Environment.NewLine;
		}
		private string OIF_PlaceOrder (string Account, string Instrument, string Action, int Qty, string OrderType, double LimitPrice, double StopPrice, string TIF, string OCOId, string OrderId, string Strategy, string StrategyId)
		{  

			if(Qty<=0 || OrderType.Length==0 || TIF.Length==0 || Action.Length==0 || Account.Length == 0 || Instrument.Length==0) return string.Empty;
			else
			{
				Account = Account.ToUpper();
				Instrument = Instrument.ToUpper();
				Action = Action.ToUpper();
				if(Action.CompareTo("BUY") != 0 && Action.CompareTo("SELL") != 0) return string.Empty;
				OrderType = OrderType.ToUpper();
				if(OrderType.CompareTo("MARKET") != 0 && OrderType.CompareTo("LIMIT") != 0 && OrderType.CompareTo("STOPMARKET") != 0 && OrderType.CompareTo("STOPLIMIT") != 0 && OrderType.CompareTo("MIT") != 0) return string.Empty;
				TIF = TIF.ToUpper();
				if(TIF.CompareTo("GTC") != 0 && TIF.CompareTo("DAY") != 0) return string.Empty;
				return "PLACE;"+Account+";"+Instrument+";"+Action+";"+Qty.ToString()+";"+OrderType+";"+LimitPrice.ToString().Replace(",",".")+";"+StopPrice.ToString().Replace(",",".")+";"+TIF+";"+OCOId+";"+OrderId+";"+Strategy+";"+StrategyId+Environment.NewLine;
			}
		}
		private string OIF_PlaceOrder (string Account, string Instrument, string Action, double Qty, string OrderType, double LimitPrice, double StopPrice, string TIF, string OCOId, string OrderId, string Strategy, string StrategyId)
		{  

			if(Qty<=0 || OrderType.Length==0 || TIF.Length==0 || Action.Length==0 || Account.Length == 0 || Instrument.Length==0) return string.Empty;
			else
			{
				Account = Account.ToUpper();
				Instrument = Instrument.ToUpper();
				Action = Action.ToUpper();
				if(Action.CompareTo("BUY") != 0 && Action.CompareTo("SELL") != 0) return string.Empty;
				OrderType = OrderType.ToUpper();
				if(OrderType.CompareTo("MARKET") != 0 && OrderType.CompareTo("LIMIT") != 0 && OrderType.CompareTo("STOPMARKET") != 0 && OrderType.CompareTo("STOPLIMIT") != 0 && OrderType.CompareTo("MIT") != 0) return string.Empty;
				TIF = TIF.ToUpper();
				if(TIF.CompareTo("GTC") != 0 && TIF.CompareTo("DAY") != 0) return string.Empty;
				return "PLACE;"+Account+";"+Instrument+";"+Action+";"+Qty.ToString()+";"+OrderType+";"+LimitPrice.ToString().Replace(",",".")+";"+StopPrice.ToString().Replace(",",".")+";"+TIF+";"+OCOId+";"+OrderId+";"+Strategy+";"+StrategyId+Environment.NewLine;
			}
		}
		private string OIF_ReversePosition (string Account, string Instrument, string Action, double Qty, string OrderType, double LimitPrice, double StopPrice, string TIF, string OCOId, string OrderId, string Strategy, string StrategyId)
		{    

			if(Qty<=0 || OrderType.Length==0 || TIF.Length==0 || Action.Length==0 || Account.Length == 0 || Instrument.Length==0) return string.Empty;
			else
			{
				Account = Account.ToUpper();
				Instrument = Instrument.ToUpper();
				Action = Action.ToUpper();
				if(Action.CompareTo("BUY") != 0 && Action.CompareTo("SELL") != 0) return string.Empty;
				OrderType = OrderType.ToUpper();
				if(OrderType.CompareTo("MARKET") != 0 && OrderType.CompareTo("LIMIT") != 0 && OrderType.CompareTo("STOPMARKET") != 0 && OrderType.CompareTo("STOPLIMIT") != 0) return string.Empty;
				TIF = TIF.ToUpper();
				if(TIF.CompareTo("GTC") != 0 && TIF.CompareTo("DAY") != 0) return string.Empty;
				return "REVERSEPOSITION;"+Account+";"+Instrument+";"+Action+";"+Qty.ToString()+";"+OrderType+";"+LimitPrice.ToString().Replace(",",".")+";"+StopPrice.ToString().Replace(",",".")+";"+TIF+";"+OCOId+";"+OrderId+";"+Strategy+";"+StrategyId+Environment.NewLine;
			}
//=====================================================================

   			 }
	#endregion				

		private bool Connected()
        {
//			if (ChartControl != null
//					&& Bars != null
//					&& Bars.Count > 0
//					&& Bars.MarketData != null
//					&& Bars.MarketData.Connection.PriceStatus == Cbi.ConnectionStatus.Connected
//					//&& Bars.Session.InSession(Now, Bars.Period, true, Bars.BarsType)
				
//				)
					
				
					
				return true;

           // return false;
        }	
		 
		
		
			static void Main(string[] args)
		    {
//		        string s = "223232-1.jpg";
//		        Console.WriteLine(sep(s));
//		        s = "443-2.jpg";
//		        Console.WriteLine(sep(s));
//		        s = "34443553-5.jpg";
//		        Console.WriteLine(sep(s));

		    Console.ReadKey();
		    }

		    public static string sep(string s)
		    {
		        int l = s.IndexOf(" -");
		        if (l >0)
		        {
		            return s.Substring(0, l);
		        }
		        return "";

		    }
			
        private bool ClickReady ()
        {
            if (myAccount == null)
                return false;

            return true;


        }
		private bool isChartFocused(ChartControl cControl)
		{

			bool isActive = false;
			
			NinjaTrader.Gui.Chart.Chart	cWindow = System.Windows.Window.GetWindow(ChartControl.Parent) as Chart;
			
			isActive = cWindow.IsFocused;
			
			
			return isActive;
		}
		
		private bool isActiveTab(ChartControl cControl)
		{
//			if (TestRender)
//				return true;
			
			if(!cControl.Properties.AreTabsVisible)
			{
				return true;
			}
			
			bool isActive = false;
			
			NinjaTrader.Gui.Chart.Chart	cWindow = System.Windows.Window.GetWindow(ChartControl.Parent) as Chart;
			
			
			
			foreach(System.Windows.Controls.TabItem tab in cWindow.MainTabControl.Items)
			{
				if((tab.Content as ChartTab).ChartControl == ChartControl && tab == cWindow.MainTabControl.SelectedItem)
				{
					isActive = true;
					break;
				}
			}
			
			return isActive;
		}
		
		private bool IsActiveATM()
		{
			
			for (int i = 1; i <=20; i++)
			{
				string ss = " - " + i.ToString();
				if (chartTraderATM.SelectedAtmStrategy.DisplayName.EndsWith(ss))
					return true;				
			}
			
			return false;
			

			
		}
		
		
		private void SetOrderParams()
		{
			
			
		
			
			
			pQty = chartTraderQty.Value;
			

//				chartTraderATM.ValueChanged -= CTQtyUpdate;
//				chartTraderTIF.ValueChanged -= CTQtyUpdate;

			
			if (chartTraderAcct.SelectedAccount != null)
			{
				myAccount = chartTraderAcct.SelectedAccount;
				pAccountName = myAccount.Name;
			}
			
			//if (!chartTraderATM.SelectedAtmStrategy.DisplayName.Contains(" - "))
			 
			try
			{
				pATMName = chartTraderATM.SelectedAtmStrategy.DisplayName;
				
				//
				//Print(pATMName);
				
				if (IsActiveATM())
					pATMName = sep(pATMName);
				
			}
			catch
			{}
			
			string ssdasf = string.Empty + "S";

							if (chartTraderATM.SelectedAtmStrategy != null)
							{
								
								//Print(chartTraderQty.Value + "   " + chartTraderATM.SelectedAtmStrategy.EntryQuantity);
								
								if (chartTraderQty.Value < chartTraderATM.SelectedAtmStrategy.EntryQuantity)
								{
									ssdasf = "Main Order quantity '" + pQty + "' must be equal to or greater than ATM Strategy quantity '" + chartTraderATM.SelectedAtmStrategy.EntryQuantity + ".  The Main Order quantity was automatically increased.";
						
					
									
									chartTraderQty.Value = chartTraderATM.SelectedAtmStrategy.EntryQuantity;
									AddError(ssdasf);
									
									
									
								}
									
								
								if (chartTraderATM.SelectedAtmStrategy.DisplayName == "Custom")
								{
									ssdasf = "You must save the ATM Strategy template.  Mouse over the ATM Strategy drop down, click Edit, click Save As Template, choose a name, click Save, then click OK.";
						
					
									AddError(ssdasf);									
										
								}
								
								
								
								
							}
							
							
			//	Print(ssdasf);			
							
							//Print(AllErrorMessages.Count);
							
							ChartControl.InvalidateVisual();
			
			
			pTIF = chartTraderTIF.SelectedTif.ToString().ToUpper();
			
			if (PreviousAccountName != pAccountName)
			{
				Subscribe();
				
				PreviousAccountName = pAccountName;
			}
			
			
//			TimeInForce TIFF;
			
//			TIFF = chartTraderTIF.SelectedTif;
			
//			Print("Name: " + pATMName);
			
//			pAccountName = "Sim101";
//			pATMName = "Test5";
//			pATMName = "";
//			pTIF = "GTC";
			
//			pSLOffset = 0;
//			pUseSLM = false;
//			pUseMIT = false;
			
		}


		private void DisableTheSystem()
		{
						
			ChartControl.Dispatcher.InvokeAsync((Action)(() =>
			{
				
			pEntriesEnabled = false;
			//pExitsEnabled = false;
			UpdateButtons();	
				
			}));
		}
					
		private void BuyNow()
		{
			ChartControl.Dispatcher.InvokeAsync((Action)(() =>
			{
				
				AllowInitialExitOrderMove = true;
//				Print("BuyNow");
				
				
				if (LongEntryOrder != null)
					CancelOrder(LongEntryOrder);
					
				if (ShortEntryOrder != null)
					CancelOrder(ShortEntryOrder);
				
				
				if (pThisEntryType == "Market")
					BuyMarket();
				if (pThisEntryType == "Limit")
				{
					EntryOrderPrice = RTTS(LastClose - pLimitOrderOffset*TickSize);
					//EntryOrderPrice = GetCurrentAsk() - pLimitOrderOffset*TickSize;
					
					BuyLimit(RTTS(EntryOrderPrice));
				}
				
				if (!pAutoEnabled)
				{
					DisableTheSystem();
				}
				
			}));
			
		}
		
		private void SellNow()
		{
			ChartControl.Dispatcher.InvokeAsync((Action)(() =>
			{
				
//				Print("SellNow");
				
				AllowInitialExitOrderMove = true;
				
				
				if (LongEntryOrder != null)
					CancelOrder(LongEntryOrder);
					
				if (ShortEntryOrder != null)
					CancelOrder(ShortEntryOrder);
				
				
				if (pThisEntryType == "Market")
					SellMarket();
				if (pThisEntryType == "Limit")
				{
					EntryOrderPrice = RTTS(LastClose + pLimitOrderOffset*TickSize);
					//EntryOrderPrice = GetCurrentBid() + pLimitOrderOffset*TickSize;
					
					SellLimit(RTTS(EntryOrderPrice));
				}
				
				if (!pAutoEnabled)
				{
					DisableTheSystem();
				}
				
			}));			
		}
		
		
		private void BuyLimit (double FinalPrice)
		{

			SetOrderParams();
			
			CurrentType = "LIMIT";
			SLMPrice = 0;
			if (pUseMIT)
			{
				SLMPrice = FinalPrice;
				CurrentType = "MIT";	
			}
			
			UniqueOrderId = string.Concat("ECTBC",pAccountName,SpreadName,DateTime.Now.Ticks);		
			
				if (pATMName == "" || pATMName == "Custom")
					instruction = OIF_PlaceOrder(pAccountName, Instrument.FullName, "BUY", pQty, CurrentType, FinalPrice, SLMPrice, pTIF, OCOID, "", "", "");
//				else if (ActiveATMSelected())
//					instruction = OIF_PlaceOrder(pAccountName, Instrument.FullName, "BUY", pQty, "LIMIT", FinalPrice, 0, pTIF, CHLI, "","",ActiveATM(pATMName)+UniqueOrderId);
				else			
					instruction = OIF_PlaceOrder(pAccountName, Instrument.FullName, "BUY", pQty, CurrentType, FinalPrice, SLMPrice, pTIF, OCOID, "", pATMName, pATMName+UniqueOrderId);
					
				
				
			Submit();		
				
			BarToCancelLimitOrder = LastCurrentBar + pLimitOrderBars;
				
			SaveLongOrder = true;
			
		}

		private void SellLimit (double FinalPrice)
		{

			SetOrderParams(); 
			
			CurrentType = "LIMIT";
			SLMPrice = 0;
			if (pUseMIT)
			{
				SLMPrice = FinalPrice;
				CurrentType = "MIT";	
			}	
			
			UniqueOrderId = string.Concat("ECTSC",pAccountName,SpreadName,DateTime.Now.Ticks);		
			
				if (pATMName == "" || pATMName == "Custom")
					instruction = OIF_PlaceOrder(pAccountName, Instrument.FullName, "SELL", pQty, CurrentType, FinalPrice, SLMPrice, pTIF, OCOID, "", "", "");
//				else if (ActiveATMSelected())
//					instruction = OIF_PlaceOrder(pAccountName, Instrument.FullName, "BUY", pQty, "LIMIT", FinalPrice, 0, pTIF, CHLI, "","",ActiveATM(pATMName)+UniqueOrderId);
				else			
					instruction = OIF_PlaceOrder(pAccountName, Instrument.FullName, "SELL", pQty, CurrentType, FinalPrice, SLMPrice, pTIF, OCOID, "", pATMName, pATMName+UniqueOrderId);
					
				
				
			Submit();	
				
			BarToCancelLimitOrder = LastCurrentBar + pLimitOrderBars;
			
			SaveShortOrder = true;
				
		}

		private void BuyMarket ()
		{
			SetOrderParams();
			
			UniqueOrderId = string.Concat("BS",pAccountName,SpreadName,DateTime.Now.Ticks,ChartName);
			
			if (pATMName == "" || pATMName == "Custom")
					instruction = OIF_PlaceOrder(pAccountName, Instrument.FullName, "BUY", pQty, "MARKET", 0, 0, pTIF, "", "", "", "");
				//else if (ActiveATMSelected())
				//	instruction = OIF_PlaceOrder(pAccountName, Instrument.FullName, "BUY", pQty, "MARKET", 0, 0, pTIF, "", "","",ActiveATM(pATMName)+UniqueOrderId);
				else			
					instruction = OIF_PlaceOrder(pAccountName, Instrument.FullName, "BUY", pQty, "MARKET", 0, 0, pTIF, "", "", pATMName, pATMName+UniqueOrderId);
							
			
			Submit();			
			
		}
		
		
		private void SellMarket ()
		{
			SetOrderParams();
			
			UniqueOrderId = string.Concat("BS",pAccountName,SpreadName,DateTime.Now.Ticks,ChartName);
			
				if (pATMName == "" || pATMName == "Custom")
					instruction = OIF_PlaceOrder(pAccountName, Instrument.FullName, "SELL", pQty, "MARKET", 0, 0, pTIF, "", "", "", "");
				//else if (ActiveATMSelected())
				//	instruction = OIF_PlaceOrder(pAccountName, Instrument.FullName, "SELL", pQty, "MARKET", 0, 0, pTIF, "", "","",ActiveATM(pATMName)+UniqueOrderId);
				else			
					instruction = OIF_PlaceOrder(pAccountName, Instrument.FullName, "SELL", pQty, "MARKET", 0, 0, pTIF, "", "", pATMName, pATMName+UniqueOrderId);
				
			Submit();	
		}
		
		
		
		
		private bool UseOIF = false;
		
		
		
				
		private void UpdateStopLoss(string Mode)
		{
		
//					LongT1 = LongPT1[0];
//				LongT2 = LongPT2[0];
//				LongStopM = LongStop[0];
//				LongStopTr = LongPT3[0];
				
//				ShortT1 = ShortPT1[0];
//				ShortT2 = ShortPT2[0];
//				ShortStopM = ShortStop[0];
//				ShortStopTr = ShortPT3[0];
			
			foreach (Order or in myAccount.Orders.ToList())
			{
				
				NewStopPrice = 0;
				
				//TrackedOrders(
						OrderStateOK = or.OrderState == OrderState.Working || or.OrderState == OrderState.Accepted;
						OrderTypeOK = or.OrderType == OrderType.StopLimit || or.OrderType == OrderType.StopMarket;
						OrderNameOK = or.Name == "Stop1" ||  or.Name == "Stop2" ||  or.Name == "Stop3";
						OrderInstrumentOK = or.Instrument.FullName == Instrument.FullName;
				
				OrderNameOK = or.Name.Contains("Stop");
				
				if (OrderTypeOK && OrderStateOK && OrderNameOK && OrderInstrumentOK)
				{
						
					bool OKToMove = false;
					
					if (or.IsShort) // In Long Trade
					{
						if (Mode == "IN")
							NewStopPrice = LongStopM;
						
						if (Mode == "TR")
							NewStopPrice = LongStopTr;
						
						OKToMove = NewStopPrice <= LastBid;
						
					}
					else // In Short Trade
					{
						if (Mode == "IN")
							NewStopPrice = ShortStopM;
						
						if (Mode == "TR")
							NewStopPrice = ShortStopTr;

						OKToMove = NewStopPrice >= LastAsk;
						
					}
					
					//Print(or.OrderUpdates.Count);
					
					StopLimitOffset = or.StopPrice - or.LimitPrice;
					NewLimitPrice = NewStopPrice - StopLimitOffset;
								
					CurrentStopPrice = or.StopPrice;
					
					if (or.LimitPrice == 0)
						NewLimitPrice = 0;
					
					
					
					if (NewStopPrice != or.StopPrice)
					
					if (NewStopPrice != 0 && OKToMove)
					{
						
						//Print(NewStopPrice);
						
						if (Mode == "IN")
							MovedStops.Add(or.OrderId);
										
						if (UseOIF)
							
						{
							instruction = OIF_ChangeOrder(0,NewLimitPrice,NewStopPrice,or.OrderId.ToString(),"");
					
							Submit();						
							
						}
						else
						{
						
							Order moveOrder;
							
							moveOrder = or;
							moveOrder.LimitPriceChanged = NewLimitPrice;
							moveOrder.StopPriceChanged = NewStopPrice;		
							
							myAccount.Change(new[] { moveOrder });
							
							//ChartControl.InvalidateVisual();
							
							
//							or.LimitPrice = NewLimitPrice;
//							or.StopPrice = NewStopPrice;		
							
//							myAccount.Change(new[] { or });
		
						}
						
						
						
					}
					
					
					
					
					//Print(or.Name);
				}
				
			}
			
					

		}
		
		private void UpdateProfitTargets(string Mode)
		{
		
		
//			LongT1 = LongPT1[0];
//				LongT2 = LongPT2[0];
//				LongStop = LongStop[0];
//				LongStopTr = LongPT3[0];
				
//				ShortT1 = ShortPT1[0];
//				ShortT2 = ShortPT2[0];
//				ShortStop = ShortStop[0];
//				ShortStopTr = ShortPT3[0];
				
			
			
			foreach (Order or in myAccount.Orders.ToList())
			{
				
				NewStopPrice = 0;
				
				//TrackedOrders(
						OrderStateOK = or.OrderState == OrderState.Working || or.OrderState == OrderState.Accepted;
						OrderTypeOK = or.OrderType == OrderType.StopLimit || or.OrderType == OrderType.StopMarket;
						OrderNameOK = or.Name == "Stop1" ||  or.Name == "Stop2" ||  or.Name == "Stop3";
						OrderInstrumentOK = or.Instrument.FullName == Instrument.FullName;
				
				OrderNameOK = or.Name == Mode;
				OrderTypeOK = true;
				
				if (OrderTypeOK && OrderStateOK && OrderNameOK && OrderInstrumentOK)
				{
						
					bool OKToMove = false;
					
					if (or.IsShort) // In Long Trade
					{
						if (or.Name.Contains("1"))
							NewLimitPrice = RTTS(LongT1);
						
						if (or.Name.Contains("2"))
							NewLimitPrice = RTTS(LongT2);
						
//						if (Mode == "SA")
//							NewStopPrice = LongSAPrice;
						
						//OKToMove = NewStopPrice <= GetCurrentBid();
						
					}
					else // In Short Trade
					{
						if (or.Name.Contains("1"))
							NewLimitPrice = RTTS(ShortT1);
						
						if (or.Name.Contains("2"))
							NewLimitPrice = RTTS(ShortT2);
						
						
//						if (Mode == "SA")
//							NewStopPrice = ShortSAPrice;

						//OKToMove = NewStopPrice >= GetCurrentAsk();
						
					}
					
//					StopLimitOffset = or.StopPrice - or.LimitPrice;
//					NewLimitPrice = NewStopPrice - StopLimitOffset;
								
//					CurrentStopPrice = or.StopPrice;
					
					
					if (or.LimitPrice == 0)
					{
						NewStopPrice = NewLimitPrice;
						NewLimitPrice = 0;
						
					}

					
					OKToMove = true;
					
					if ((NewLimitPrice != 0 || NewStopPrice != 0) && OKToMove)
					{
						
						MovedTargets.Add(or.OrderId);
						
						if (UseOIF)
							
						{
							
							
							
							instruction = OIF_ChangeOrder(0,NewLimitPrice,NewStopPrice,or.OrderId.ToString(),"");
					
							Submit();						
							
						}
						else
						{
						
							Order moveOrder;
							
							moveOrder = or;
							moveOrder.LimitPriceChanged = NewLimitPrice;
							moveOrder.StopPriceChanged = NewStopPrice;		
							
							myAccount.Change(new[] { moveOrder });
							
							//ChartControl.InvalidateVisual();
							
							
//							or.LimitPrice = NewLimitPrice;
//							or.StopPrice = NewStopPrice;		
							
//							myAccount.Change(new[] { or });
		
						}
						

						
						
						
					}
					
					
					
					//Print(or.Name);
				}
				
			}
			
					

		}
				
		
		

		
        internal void CTMouseDown(object sender, MouseButtonEventArgs e)
        {
			if (myAccount == null)
				return;
			
			SetOrderParams();
			ChartControl.InvalidateVisual();
		}
		
			
        internal void CTMouseMove(object sender, MouseEventArgs e)
        {
			if (myAccount == null)
				return;
			
			SetOrderParams();
			ChartControl.InvalidateVisual();
		}
		
		
		
	
        internal void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.MP = e.GetPosition(this.ChartPanel);

 
			FinalXPixel = MP.X / 100 * dpiX;
			FinalYPixel = MP.Y / 100 * dpiY;
         

 		
			
			if (AllErrorMessages.Count > 0)
			{
				AllErrorMessages.Clear();
				ChartControl.InvalidateVisual();
				
				//myProperties.AllowSelectionDragging = PreviousDrag;
				
				return;
				
			}


			// top chart buttons
			
			
			
			
			
			bool OneDone = false;
            foreach (KeyValuePair<double, ButtonZ> thisbutton in AllButtonZ2)
            {
                bool hoverednew = MouseIn(thisbutton.Value.Rect, 2, 2);
                string buttonn = thisbutton.Value.Text;


				

               if (hoverednew && buttonn == SFeed1)
                {
					
					//Print(buttonn);
					
					pFeed1Included = !pFeed1Included;
					OneDone = true;
					
					
					
				}
				
               if (hoverednew && buttonn == SFeed2)
                {
					
					//Print(buttonn);
					pFeed2Included = !pFeed2Included;
					OneDone = true;
					
					
				}				
				
               if (hoverednew && buttonn == SFeed3)
                {
					
					//Print(buttonn);
					pFeed3Included = !pFeed3Included;
					OneDone = true;
					
										
					
					
				}	
				
               if (hoverednew && buttonn == SFeed4)
                {
					
					//Print(buttonn);
					
					pFeed4Included = !pFeed4Included;
					OneDone = true;
					
						
					
				}
				
			}
			
			
			if (OneDone)
			{
					SetAllTrend();
					UpdateButtons();
					ChartControl.InvalidateVisual();
					return;						
			}
			
			
			
			
			
            foreach (KeyValuePair<double, ButtonZ> thisbutton in AllButtonZ)
            {
                bool hoverednew = MouseIn(thisbutton.Value.Rect, 2, 2);
                string buttonn = thisbutton.Value.Text;



               if (hoverednew && buttonn == "TRADES")
                {
                    pSLOffset = Math.Max(0, pSLOffset - 1);
                    
					TogglePlotExecutions();
					
					thisbutton.Value.Switch = ChartBars.Properties.PlotExecutions != ChartExecutionStyle.DoNotPlot;
					
				//	SetBack(0);
					
					this.ChartControl.InvalidateVisual();
					return;

                }               
  
				
				else if (hoverednew && buttonn == "Signals")
                {
                    if (pArrowsEnabled)
                    {
                        pArrowsEnabled = false;
                    }
                    else
                    {
                        pArrowsEnabled = true;
                    }
					
                    thisbutton.Value.Switch = pArrowsEnabled;
					
					SetAllTrend();
					
                    this.ChartControl.InvalidateVisual();
					
					return;

                }

                else if (hoverednew && buttonn == "Delta")
                {
                    if (pHUDEnabled)
                    {
                        pHUDEnabled = false;
                    }
                    else
                    {
                        pHUDEnabled = true;
                    }
                    thisbutton.Value.Switch = pHUDEnabled;
                    this.ChartControl.InvalidateVisual();
					
					return;

                }  

                else if (hoverednew && buttonn == "Trend")
                {
                    if (pSecondaryFeedsDisplayEnabled)
                    {
                        pSecondaryFeedsDisplayEnabled = false;
                    }
                    else
                    {
                        pSecondaryFeedsDisplayEnabled = true;
                    }
                    thisbutton.Value.Switch = pSecondaryFeedsDisplayEnabled;
                    this.ChartControl.InvalidateVisual();
					
					return;

                }  				
				
								
								

                else if (hoverednew && buttonn == "Exits")
                {
                    if (pExitOEnabled && pExitHOEnabled)
                    {
						pExitOEnabled = true;
						pExitHOEnabled = false;
                        
                    }
                    else if (pExitOEnabled)
					{
						pExitOEnabled = false;
						pExitHOEnabled = false;
					}
					else
                    {
						pExitOEnabled = true;
						pExitHOEnabled = true;
                    }
                    thisbutton.Value.Switch = pExitOEnabled;
                    this.ChartControl.InvalidateVisual();
					
					return;

                }  

				
                else if (hoverednew && buttonn == "MA")
                {
                    if (pVisualEnabled)
                    {
                        pVisualEnabled = false;
						
						//PaintPriceMarkers = false;
					
				
                    }
                    else
                    {
                        pVisualEnabled = true;
						//PaintPriceMarkers = StartPriceMarkers;
                    }
                    thisbutton.Value.Switch = pVisualEnabled;
                    this.ChartControl.InvalidateVisual();
					
					return;

                }  
				
				
				
				
              
                else if (hoverednew && buttonn == "SLM")
                {
                    if (pUseSLM)
                    {
                        pUseSLM = false;
                    }
                    else
                    {
                        pUseSLM = true;
                    }
                    thisbutton.Value.Switch = pUseSLM;
                    this.ChartControl.InvalidateVisual();
					
					return;

                }

                else if (hoverednew && buttonn == "MIT")
                {
                    if (pUseMIT)
                    {
                        pUseMIT = false;
                    }
                    else
                    {
                        pUseMIT = true;
                    }
                    thisbutton.Value.Switch = pUseMIT;
                    this.ChartControl.InvalidateVisual();
					
					return;

                }  

            }
			
			
			
		


        }
		

		
		internal void OnMouseLeave(object sender, MouseEventArgs e)
    	{
            this.MP = e.GetPosition(this.ChartPanel);


			FinalXPixel = MP.X / 100 * dpiX;
			FinalYPixel = MP.Y / 100 * dpiY;
			
			InMenu = false;
			this.ChartControl.InvalidateVisual();
		}
		
		internal void OnMouseMove(object sender, MouseEventArgs e)
    	{
            this.MP = e.GetPosition(this.ChartPanel);


			FinalXPixel = MP.X / 100 * dpiX;
			FinalYPixel = MP.Y / 100 * dpiY;
         

      
            foreach (KeyValuePair<double, ButtonZ> thisbutton in AllButtonZ)
                {
                    bool hoverednew = MouseIn(thisbutton.Value.Rect, 2, 2);
                    bool hoverednow = thisbutton.Value.Hovered;

                    if (hoverednew && !hoverednow)
                    {
                        thisbutton.Value.Hovered = true;
                        this.ChartControl.InvalidateVisual();
                    }
                    if (!hoverednew && hoverednow)
                    {
                        thisbutton.Value.Hovered = false;
                        this.ChartControl.InvalidateVisual();
                    }

                }

                InMenuP = InMenu;
                InMenu = MouseIn(B2, 8, 8);
           
            if (InMenu != InMenuP)
                this.ChartControl.InvalidateVisual();

			
			
			//e.Handled = true;
		}

		protected override void OnMarketData(MarketDataEventArgs marketDataUpdate)
		{
			
			if (!pClockErrorMessagesEnabled)
				return;
			
			
//								int totalsec = BarsPeriod.BaseBarsPeriodValue;
								
//								double MaxOffSeconds = 5;
								
//								if (BarsArray[0].BarsPeriod.BarsPeriodType == BarsPeriodType.Minute)
//									totalsec = BarsArray[0].BarsPeriod.Value * 60;
								
//								Print(totalsec);
//								Print(barTimeLeft.TotalSeconds);
								
//								SecondsBehind = Math.Max(SecondsBehind, barTimeLeft.TotalSeconds - totalsec);
//								//SecondsAhead = 0;
								
//								if (barTimeLeft.TotalSeconds <= -1*MaxOffSeconds)
//								SecondsAhead = Math.Max(SecondsAhead, Math.Abs(barTimeLeft.TotalSeconds));
								
//								if (SecondsAhead >= MaxOffSeconds)
//								{
									
//									AllErrorMessages.Clear();
//									AddError("Please adjust your computer clock via Windows Date and Time settings.  You are " + SecondsAhead + " seconds ahead the real time.");
//								}								
								
//								if (SecondsBehind >= MaxOffSeconds)
//								{
									
//									AllErrorMessages.Clear();
//									AddError("Please adjust your computer clock via Windows Date and Time settings.  You are " + SecondsBehind + " seconds behind the real time.");
//								}
								
			
			
			
		    // Print some data to the Output window
		    if (marketDataUpdate.MarketDataType == MarketDataType.Last) 
			{
				
				if (InitialTime == DateTime.MinValue)
				{
					InitialTime = marketDataUpdate.Time;
					
				}
				
				if (InitialTime == marketDataUpdate.Time)
					return;
				
				
//				if (State != State.Realtime)
//					return;
				
//				Print("DATA:" + marketDataUpdate.Time);
//				Print("NOW:" + Now);
				
				if (pClockErrorMessagesEnabled && pTimerEnabled)
				{
					
					TimeSpan barTimeLeft2 = marketDataUpdate.Time.Subtract(Now);
					
					int totalsecdiff = Math.Abs((int) barTimeLeft2.TotalSeconds);
					
					
	//				Print(totalsecdiff);
					
					string thiso = "Please synchronize your clock via Windows Date & time settings.  Your computer clock is more than 5 seconds off when compared to the real market data.";
					
					if (totalsecdiff >= 5)
						AddError(thiso);
					else
						AllErrorMessages.Remove(thiso);
				}
				
		        //Print(string.Format("Last = {0} {1} ", marketDataUpdate.Price, marketDataUpdate.Volume));
				LastPrice = marketDataUpdate.Price;
			}
		    else if (marketDataUpdate.MarketDataType == MarketDataType.Ask)
			{
		       // Print(string.Format("Ask = {0} {1} ", marketDataUpdate.Price, marketDataUpdate.Volume));
				LastAsk = marketDataUpdate.Price;
			}
		    else if (marketDataUpdate.MarketDataType == MarketDataType.Bid)
			{
		        //Print(string.Format("Bid = {0} {1}", marketDataUpdate.Price, marketDataUpdate.Volume));
				LastBid = marketDataUpdate.Price;
			}
		}  


		
		
		private bool DisplayTime()
		{
			return ChartControl != null
					&& BarsArray[0] != null
					&& BarsArray[0].Instrument.MarketData != null;
			
			// changed Bars to BarsArray
		}

		private void OnTimerTick(object sender, EventArgs e)
		{
			//ForceRefresh();

			//if (!isRangeDerivate && BarsPeriod.BarsPeriodType != BarsPeriodType.Tick && BarsPeriod.BarsPeriodType != BarsPeriodType.Volume)

			//Print(BarsPeriod.BarsPeriodType);
			
			//Print(BarsPeriod.BaseBarsPeriodType);
			
			
			//if (BarsArray[0] != null)
			
			if (BarsArray[0].BarsPeriod.BarsPeriodType == BarsPeriodType.Minute || BarsArray[0].BarsPeriod.BarsPeriodType == BarsPeriodType.Second)
			{
				
				
				
				if (DisplayTime())
				{
					if (timer != null && !timer.IsEnabled)
						timer.IsEnabled = true;

					if (connected)
					{
						if (SessionIterator != null)
						if (SessionIterator.IsInSession(Now, false, true))
						{
							if (hasRealtimeData)
							{
								TimeSpan barTimeLeft = BarsArray[0].GetTime(BarsArray[0].Count - 1).Subtract(Now);

//								int totalsec = BarsPeriod.BaseBarsPeriodValue;
								
//								double MaxOffSeconds = 5;
								
//								if (BarsArray[0].BarsPeriod.BarsPeriodType == BarsPeriodType.Minute)
//									totalsec = BarsArray[0].BarsPeriod.Value * 60;
								
//								Print(totalsec);
//								Print(barTimeLeft.TotalSeconds);
								
//								SecondsBehind = Math.Max(SecondsBehind, barTimeLeft.TotalSeconds - totalsec);
//								//SecondsAhead = 0;
								
//								if (barTimeLeft.TotalSeconds <= -1*MaxOffSeconds)
//								SecondsAhead = Math.Max(SecondsAhead, Math.Abs(barTimeLeft.TotalSeconds));
								
//								if (SecondsAhead >= MaxOffSeconds)
//								{
									
//									AllErrorMessages.Clear();
//									AddError("Please adjust your computer clock via Windows Date and Time settings.  You are " + SecondsAhead + " seconds ahead the real time.");
//								}								
								
//								if (SecondsBehind >= MaxOffSeconds)
//								{
									
//									AllErrorMessages.Clear();
//									AddError("Please adjust your computer clock via Windows Date and Time settings.  You are " + SecondsBehind + " seconds behind the real time.");
//								}
								
								timeLeft = (barTimeLeft.Ticks < 0
									? "0:00"
									: barTimeLeft.Hours.ToString("00") + ":" + barTimeLeft.Minutes.ToString("00") + ":" + barTimeLeft.Seconds.ToString("00"));
								
								if (barTimeLeft.TotalHours < 1 && barTimeLeft.Ticks >= 0)
									timeLeft = barTimeLeft.Minutes.ToString("0") + ":" + barTimeLeft.Seconds.ToString("00");
								
								
								
								
								BarTimerString = timeLeft;
								
								
								//Print(BarTimerString);
				
	           					 ChartControl.InvalidateVisual();
								
								//Draw.TextFixed(this, "NinjaScriptInfo", NinjaTrader.Custom.Resource.BarTimerTimeRemaining + timeLeft, TextPosition.BottomRight);
							}
							else
							{
								//Draw.TextFixed(this, "NinjaScriptInfo", NinjaTrader.Custom.Resource.BarTimerWaitingOnDataError, TextPosition.BottomRight);
							}
						}
						else
						{
							//Draw.TextFixed(this, "NinjaScriptInfo", NinjaTrader.Custom.Resource.BarTimerSessionTimeError, TextPosition.BottomRight);
						}
					}
					else
					{
						//Draw.TextFixed(this, "NinjaScriptInfo", NinjaTrader.Custom.Resource.BarTimerDisconnectedError, TextPosition.BottomRight);

						if (timer != null)
							timer.IsEnabled = false;
					}
				}
			}
		}

		private SessionIterator SessionIterator
		{
			get
			{
				if (sessionIterator == null)
					sessionIterator = new SessionIterator(Bars);
				return sessionIterator;
			}
		}

		private DateTime Now
		{
			get
			{
				now = (Cbi.Connection.PlaybackConnection != null ? Cbi.Connection.PlaybackConnection.Now : Core.Globals.Now);

				if (now.Millisecond > 0)
					now = Core.Globals.MinDate.AddSeconds((long)Math.Floor(now.Subtract(Core.Globals.MinDate).TotalSeconds));

				return now;
			}
		}
	
		

			
	private void SetOverallTrend(int bb)
			{
			
			OverallTrend[bb] = 0;
			//OverallTrend.Set(0,0);	
				
			
			AllTrends.Clear();
			
			if (pFeed1Enabled && pFeed1Included)
				AllTrends.Add(Direction1[bb]);
			
			if (pFeed2Enabled && pFeed2Included)
				AllTrends.Add(Direction2[bb]);				
		
			if (pFeed3Enabled && pFeed3Included)
				AllTrends.Add(Direction3[bb]);
			
			if (pFeed4Enabled && pFeed4Included)
				AllTrends.Add(Direction4[bb]);	
			
			bool IsLong = true;
			bool IsShort = true;
			
			foreach (int ii in AllTrends)
			{
				if (ii != 1) IsLong = false;
				if (ii != -1) IsShort = false;
			}
			
			if (AllTrends.Count == 0)
				OverallTrend[bb] = 0;
			else if (IsLong)
				OverallTrend[bb] = 1;			
			else if (IsShort)
				OverallTrend[bb] = -1;				
				
			}
			
	
			private void SetBackground(int bb)
			{
				
				if (pBackEnabled)
				{
					
					BackBrushesAll[bb] = null;
					BackBrushes[bb] = null;
					
						
					if (pBackTEnabled)
					{
					
						
					
						if (OverallTrend[bb] == 1)
							SetBackColor2(pBrush07, bb);
						
						if (OverallTrend[bb] == -1)
							SetBackColor2(pBrush08, bb);
						
						
					
					}
					
					if (pBackMEnabled)
					{
						
						if (IsInSession[bb] != 1)
						{					
							SetBackColor2(pBrush09, bb);
						}	
						
						
				
					
					}
					
					
					if (pArrowsEnabled)
					
					if (pBackSEnabled)
					{
						
						if (Signals[bb] == 1)
						{					
							SetBackColor2(pBrush01, bb);
						}				
						else if (Signals[bb] == -1)
						{
							SetBackColor2(pBrush02, bb);
						}
						
					}
						


					
				}
				
			
			}
				  
		private void SetBackColor2 (Brush BB, int bb)
		{
			
			if (pColorAll) 
				BackBrushesAll[bb] = BB;
			else
				BackBrushes[bb] = BB;			
		}

	private void SetBackColor3 (Brush BB, int index)
		{
			
			if (pColorAll) 
				BackBrushesAll.Set(index, BB);
			else
				BackBrushes.Set(index, BB);	
		}		
		
		private void SetBackColor (Brush BB)
		{
			
			if (pColorAll) 
				BackBrushesAll[0] = BB;
			else
				BackBrushes[0] = BB;			
		}
			
			
		protected override void OnBarUpdate()
		{
			
			//Print("test");
			
			if (!Permission)
				return;		
			
			
			
			for(int i = 0; i <= BarsArray.Length-1; i++) // return for all bars on the chart if there isn't a bar
			{
				if (CurrentBars[i] < 0)
					return;
			}
			
			
			
			if (BarsArray == null || BarsArray.Length == 0)
				return;

			if (BarsInProgress == 0)
			{
				// Only evaluate once — BarsPeriod doesn't change after load (avoids per-tick ToString alloc + IndexOf)
				if (!isRangeDerivateChecked)
				{
					isRangeDerivate = BarsArray[0].BarsType.BuiltFrom == BarsPeriodType.Tick
					                  && BarsArray[0].BarsPeriod.ToString().IndexOf("Range") >= 0;
					isRangeDerivateChecked = true;
				}
		
				if (isRangeDerivate)
				{
					int barIdx = Bars.Count - 1 - (Calculate == NinjaTrader.NinjaScript.Calculate.OnBarClose ? 1 : 0);
					double high = High.GetValueAt(barIdx);
					double low = Low.GetValueAt(barIdx);
					double close = Close.GetValueAt(barIdx);
					int actualRange = (int)Math.Round(Math.Max(close - low, high - close) / Bars.Instrument.MasterInstrument.TickSize);
					double rangeCount = pCountDown ? BarsPeriod.Value - actualRange : actualRange;

					BarTimerString = rangeCount.ToString();
				}
				else if (BarsArray[0].BarsPeriod.BarsPeriodType == BarsPeriodType.Tick)
				{
					
					// tick
					
					double periodValue = (BarsPeriod.BarsPeriodType == BarsPeriodType.Tick) ? BarsPeriod.Value : BarsPeriod.BaseBarsPeriodValue;
					double tickCount = pShowPercent ? pCountDown ? (1 - Bars.PercentComplete) * 100 : Bars.PercentComplete * 100 : pCountDown ? periodValue - Bars.TickCount : Bars.TickCount;

					string label = NinjaTrader.Custom.Resource.TickCounterTicksRemaining;  //NinjaTrader.Custom.Resource.TickCounterTickCount
					label = string.Empty;
					
					string tick1 = (BarsPeriod.BarsPeriodType == BarsPeriodType.Tick 
								|| ((BarsPeriod.BarsPeriodType == BarsPeriodType.HeikenAshi || BarsPeriod.BarsPeriodType == BarsPeriodType.Volumetric) && BarsPeriod.BaseBarsPeriodType == BarsPeriodType.Tick) ? ((pCountDown
												? label + tickCount : label + tickCount) + (pShowPercent ? "%" : ""))
												: NinjaTrader.Custom.Resource.TickCounterBarError);

					BarTimerString = tick1;
					
					//Draw.TextFixed(this, "NinjaScriptInfo", tick1, TextPosition.BottomRight, ChartControl.Properties.ChartText, ChartControl.Properties.LabelFont, Brushes.Transparent, Brushes.Transparent, 0);			
					
									
					
				}
				else if (BarsArray[0].BarsPeriod.BarsPeriodType == BarsPeriodType.Volume)
				{
					
					// volume
					
					volume = (long)Volume[0];

					double volumeCount = pShowPercent
						? pCountDown
							? (1 - Bars.PercentComplete) * 100
							: Bars.PercentComplete * 100
						: pCountDown
							? (isVolumeBase
								? BarsPeriod.BaseBarsPeriodValue
								: BarsPeriod.Value) - volume
							: volume;

					string label = NinjaTrader.Custom.Resource.VolumeCounterVolumeRemaining; // NinjaTrader.Custom.Resource.VolumeCounterVolumeCount
					label = string.Empty;
					
					string volume1 = (isVolume || isVolumeBase)
						? ((pCountDown
							? label + volumeCount
							: label + volumeCount) + (pShowPercent ? "%" : ""))
						: NinjaTrader.Custom.Resource.VolumeCounterBarError;

					BarTimerString = volume1;
					
					//Draw.TextFixed(this, "NinjaScriptInfo", volume1, TextPosition.BottomRight);				
					
				}
				else
				{
					
					
					
				}
			
			
			}
			
			// end counter
			
			
			

			
			
			if (State == State.Realtime && BarsInProgress == 0)
			{
				hasRealtimeData = true;
				connected = true;
				
				LastAsk = GetCurrentAsk();
				LastBid = GetCurrentBid();
			}
			
			
			
						
			if (State != State.Realtime)
			if (pCuEnabled && BarsInProgress == 1)
				return;
			
			
			// EXECUTION


			

			IsCurrentBar = State == State.Realtime;			
			
			LastCurrentBar = CurrentBar;

			if (BarsInProgress == 0)
			if (IsCurrentBar)
			{
				
				if (IsFirstTickOfBar)
				{
					AllowInitialExitOrderMove = false;
					
					// cancel limit order
					
					//Print(BarToCancelLimitOrder + "  " + LastCurrentBar);
					
					if (BarToCancelLimitOrder == LastCurrentBar)
					{
						
						//Print("Is Canceling");
						
						if (LongEntryOrder != null)
						{
							//Print("LongEntryOrder");
							CancelOrder(LongEntryOrder);
						}
							
						if (ShortEntryOrder != null)
						{
							//Print("ShortEntryOrder");
							CancelOrder(ShortEntryOrder);	
						}
						
					}

					
					// close position

					
					// submit entries
					
					//if (pEntriesEnabled)
					{
						
						if (DoClose)
							ClosePosition();
					
						LastClose = Close[1];
						
						if (DoLong)
						
							BuyNow();
						
						if (DoShort)
							SellNow();						
	
					}
										
						
				}

			}		
			
			
			
			if (BarsInProgress == 0)
			{
			      // Print the close of the cumulative delta bar with a delta type of Bid Ask and with a Session period
			     // Print("Delta Close: " + cumulativeDelta.DeltaClose[0]);
				
				
				
			}
			else if (pCuEnabled && BarsInProgress == 1)
			{
			      // We have to update the secondary series of the hosted indicator to make sure the values we get in BarsInProgress == 0 are in sync
			      cumulativeDelta.Update(cumulativeDelta.BarsArray[1].Count - 1, 1);
			}
			
			
			if (pCuEnabled)
			{
				LongLine[0] = cumulativeDelta.DeltaClose[0] - cumulativeDelta.DeltaOpen[0];
				ShortLine[0] = cumulativeDelta.DeltaClose[0];
			}
			
			
			int BB = 0;
			
			if (State == State.Realtime)
				BB = 1;
			
			//Print(BB);
			
			//if (Feed1 != 0 && CurrentBars[Feed1] >= 0)
//			if (Feed1 != 0 && BarsInProgress == Feed1)
//			{
				
//				if (iEMAFastFeed1[BB] > iEMASlowFeed1[BB])
//					ThisDirection1 = 1;
//				if (iEMAFastFeed1[BB] < iEMASlowFeed1[BB])
//					ThisDirection1 = -1;
	
//				if (Lows[Feed1][BB] > iEMASlowFeed1[BB])
//					ThisDirection1 = 1;
//				if (Highs[Feed1][BB] < iEMASlowFeed1[BB])
//					ThisDirection1 = -1;
				
//			}
			
//			//if (Feed2 != 0 && CurrentBars[Feed2] >= 0)
//			if (Feed2 != 0 && BarsInProgress == Feed2)
//			{
				
//				if (iEMAFastFeed2[BB] > iEMASlowFeed2[BB])
//					ThisDirection2 = 1;
//				if (iEMAFastFeed2[BB] < iEMASlowFeed2[BB])
//					ThisDirection2 = -1;				
				
//			}			
			
//			//if (Feed3 != 0 && CurrentBars[Feed3] >= 0)
//			if (Feed3 != 0 && BarsInProgress == Feed3)
//			{
				
//				if (iEMAFastFeed3[BB] > iEMASlowFeed3[BB])
//					ThisDirection3 = 1;
//				if (iEMAFastFeed3[BB] < iEMASlowFeed3[BB])
//					ThisDirection3 = -1;				
				
//			}	
//			//if (Feed4 != 0 && CurrentBars[Feed4] >= 0)
//			if (Feed4 != 0 && BarsInProgress == Feed4)
//			{
				
//				if (iEMAFastFeed4[BB] > iEMASlowFeed4[BB])
//					ThisDirection4 = 1;
//				if (iEMAFastFeed4[BB] < iEMASlowFeed4[BB])
//					ThisDirection4 = -1;				
				
//			}	
			
			
			if (Feed1 != 0 && BarsInProgress == Feed1)
			{
				
		
	
				if (Lows[Feed1][BB] > iEMAFastFeed1[BB])
					ThisDirection1 = 1;
				if (Highs[Feed1][BB] < iEMAFastFeed1[BB])
					ThisDirection1 = -1;
				
			}
			
			//if (Feed2 != 0 && CurrentBars[Feed2] >= 0)
			if (Feed2 != 0 && BarsInProgress == Feed2)
			{
				
				if (Lows[Feed2][BB] > iEMAFastFeed2[BB])
					ThisDirection2 = 1;
				if (Highs[Feed2][BB] < iEMAFastFeed2[BB])
					ThisDirection2 = -1;				
				
			}			
			
			//if (Feed3 != 0 && CurrentBars[Feed3] >= 0)
			if (Feed3 != 0 && BarsInProgress == Feed3)
			{
				
				if (Lows[Feed3][BB] > iEMAFastFeed3[BB])
					ThisDirection3 = 1;
				if (Highs[Feed3][BB] < iEMAFastFeed3[BB])
					ThisDirection3 = -1;				
				
			}	
			//if (Feed4 != 0 && CurrentBars[Feed4] >= 0)
			if (Feed4 != 0 && BarsInProgress == Feed4)
			{
				
				if (Lows[Feed4][BB] > iEMAFastFeed4[BB])
					ThisDirection4 = 1;
				if (Highs[Feed4][BB] < iEMAFastFeed4[BB])
					ThisDirection4 = -1;				
				
			}	
			
			
			
			
			
			Direction1[0] = ThisDirection1;
			Direction2[0] = ThisDirection2;
			Direction3[0] = ThisDirection3;
			Direction4[0] = ThisDirection4;			
			
			
			//SetOverallTrend(0);
			
		
			OverallTrend[0] = 0;
			
			AllTrends.Clear();
			
			if (pFeed1Enabled && pFeed1Included)
				AllTrends.Add(Direction1[0]);
			
			if (pFeed2Enabled && pFeed2Included)
				AllTrends.Add(Direction2[0]);				
		
			if (pFeed3Enabled && pFeed3Included)
				AllTrends.Add(Direction3[0]);
			
			if (pFeed4Enabled && pFeed4Included)
				AllTrends.Add(Direction4[0]);	
			
			bool IsLong = true;
			bool IsShort = true;
			
			foreach (int ii in AllTrends)
			{
				if (ii != 1) IsLong = false;
				if (ii != -1) IsShort = false;
			}
			
			if (AllTrends.Count == 0)
				OverallTrend[0] = 0;
			else if (IsLong)
				OverallTrend[0] = 1;			
			else if (IsShort)
				OverallTrend[0] = -1;
			
			
			CurrentTrendStatus = OverallTrend[0];
			
			if (State == State.Realtime)
			if (CurrentTrendStatusButton != CurrentTrendStatus)
				UpdateButtons();
						
			
//			if (Direction1[0] == 1 && Direction1[0] == Direction2[0])
//				OverallTrend[0] = 1;
			
//			if (Direction1[0] == -1 && Direction1[0] == Direction2[0])
//				OverallTrend[0] = -1;
			
			
			if (BarsInProgress >= 1)
				return;			
	
			
			
			
			MainATR[0] = iATR[0]; 
			StopATR[0] = MainATR[0] * pATRStopM;
			TrailingStopATR[0] = MainATR[0] * pATRStopMT;
			TargetATR[0] = MainATR[0] * pATRTargetM;
			TargetATR2[0] = MainATR[0] * pATRTargetM2;
	
			
			
			
			if (FirstRun)
			{ 
				StartTime  = new DateTime(Time[0].Year, Time[0].Month, Time[0].Day, pStartTime.Hours, pStartTime.Minutes, 0);
				EndTime    = new DateTime(Time[0].Year, Time[0].Month, Time[0].Day, pEndTime.Hours, pEndTime.Minutes, 0);
				
				StartTime2  = new DateTime(Time[0].Year, Time[0].Month, Time[0].Day, pStartTime2.Hours, pStartTime2.Minutes, 0);
				EndTime2    = new DateTime(Time[0].Year, Time[0].Month, Time[0].Day, pEndTime2.Hours, pEndTime2.Minutes, 0);					
				
				StartTime3  = new DateTime(Time[0].Year, Time[0].Month, Time[0].Day, pStartTime3.Hours, pStartTime3.Minutes, 0);
				EndTime3    = new DateTime(Time[0].Year, Time[0].Month, Time[0].Day, pEndTime3.Hours, pEndTime3.Minutes, 0);					
				
				if (EndTime <= StartTime) EndTime = EndTime.AddDays(1);
				if (EndTime2 <= StartTime2) EndTime2 = EndTime2.AddDays(1);
				if (EndTime3 <= StartTime3) EndTime3 = EndTime3.AddDays(1);
				
				//Print(StartTime);
				//Print(EndTime);
				FirstRun = false;
				
			}				
				
				
				if (pUseTimeFilter)
				{
					IsInSession[0] = 0;
					
					if (pStartTime != pEndTime)
					if (Time[0].Ticks > StartTime.Ticks && Time[0].Ticks <= EndTime.Ticks)
					{
						IsInSession[0] = 1;
						
						//BackBrush = Brushes.Navy;
						
					}
					
					if (pStartTime2 != pEndTime2)
					if (Time[0].Ticks > StartTime2.Ticks && Time[0].Ticks <= EndTime2.Ticks)
					{
						IsInSession[0] = 1;
					}
					
					if (pStartTime3 != pEndTime3)
					if (Time[0].Ticks > StartTime3.Ticks && Time[0].Ticks <= EndTime3.Ticks)
					{
						IsInSession[0] = 1;
					}					
						
				}
				else
				{
					IsInSession[0] = 1;
					
				}
			
				
				
				
				while(Time[0].Ticks > EndTime.Ticks)
				{
					StartTime = StartTime.AddDays(1);
					EndTime   = EndTime.AddDays(1);
				}
				
				while(Time[0].Ticks > EndTime2.Ticks)
				{
					StartTime2 = StartTime2.AddDays(1);
					EndTime2   = EndTime2.AddDays(1);
				}
				
				while(Time[0].Ticks > EndTime3.Ticks)
				{
					StartTime3 = StartTime3.AddDays(1);
					EndTime3  = EndTime3.AddDays(1);
				}
				
				
				
				
			// INDICATOR
			
            if (CurrentBars[0] == 0)
                FirstBarTime = Times[0][0];

//            if (BarsInProgress == 0)
//            {
//                BodyHigh[0] = Math.Max(Closes[0][0], Opens[0][0]);
//                BodyLow[0] = Math.Min(Closes[0][0], Opens[0][0]);

//                WickHigh[0] = Highs[0][0];
//                WickLow[0] = Lows[0][0];

//                if (Close[0] > Open[0])
//                    Direction[0] = 1;
//                else if (Close[0] < Open[0])
//                    Direction[0] = -1;
//                else if (CurrentBars[0] == 0)
//                    Direction[0] = 1;
//                else
//                    Direction[0] = Direction[1];
//            }

           // Values[0][0] = 43.2;
           // Values[1][0] = 43.25;

			if (CurrentBar < 1)
				return;			
			
		

	
						
			Signals[0] = 0;
			LastSignalBar[0] = LastSignalBar[1];

			
//			Avg[0] = 0;
			
//			if (Close[0] > Open[0])
//				Avg[0] = 1;
			
//			if (Close[0] < Open[0])
//				Avg[0] = -1;			
			

			if (pThisEntryType22 == "EMA")
			{
				ThisMA[0] = iEMAFast[0];
				
			}
			else if (pThisEntryType22 == "SMA")
			{
				ThisMA[0] = SMA(pEMAFastPeriod)[0];
				
			}		
			else if (pThisEntryType22 == "DEMA")
			{
				ThisMA[0] = DEMA(pEMAFastPeriod)[0];
				
			}		
			else if (pThisEntryType22 == "WMA")
			{
				ThisMA[0] = WMA(pEMAFastPeriod)[0];
				
			}				
			else if (pThisEntryType22 == "HMA")
			{
				ThisMA[0] = HMA(pEMAFastPeriod)[0];
				
			}		
			else if (pThisEntryType22 == "TMA")
			{
				ThisMA[0] = TMA(pEMAFastPeriod)[0];
				
			}		
			
			 
		
			//if (pVisualEnabled)
			{
				
//				ThisFastEMA[0] = iEMAFast[0];
//				ThisSlowEMA[0] = iEMASlow[0];
				
				Values[0][0] = ThisMA[0];
//				Values[1][0] = iEMASlow[0];
				
//				Print(ThisFastEMA[0]);
			}
			
			
//			if (iEMAFast[0] > iEMASlow[0] && iEMAFast[1] <= iEMASlow[1])
//				Signals[0] = 1;
			
//			if (iEMAFast[0] < iEMASlow[0] && iEMAFast[1] >= iEMASlow[1])
//				Signals[0] = -1;			
			

			ThisTrend1[0] = ThisTrend1[1];
			
			if (Highs[0][0] >= ThisMA[0] && Lows[0][0] <= ThisMA[0]) // touching ema
			{
				
		
				
				int pNumberOfBars2 = pNumberOfBars;
				
				if (ThisTrend1[0] >= pNumberOfBars2 && ThisTrend1[0] <= pNumberOfBars3)
					Signals[0] = 1;
				
				if (ThisTrend1[0] <= pNumberOfBars2*-1 && ThisTrend1[0] >= pNumberOfBars3*-1)
					Signals[0] = -1;				
				
			}
			
			
			
			if (Low[0] > ThisMA[0])
				ThisTrend1[0] = ThisTrend1[0] + 1;
			else if (High[0] < ThisMA[0])
				ThisTrend1[0] = ThisTrend1[0] + -1;
			else
				ThisTrend1[0] = 0;
			
			
			
			
			
			if (Signals[0] != 0)
				LastSignalBar[0] = CurrentBars[0];
			
			
			if (Low[0] > iEMAFast[0])
			{
				TrendSlope[0] = 1;
				PlotBrushes[0][0] = pPlotUpFBrush;
				PlotBrushes[1][0] = pPlotUpFBrush;
			}
			else if (High[0] < iEMAFast[0])	
			//else if (iEMAFast[0] < iEMASlow[0])			
			{
				PlotBrushes[0][0] = pPlotDownFBrush;
				PlotBrushes[1][0] = pPlotDownFBrush;
				TrendSlope[0] = -1;
			}
			else
			{
				PlotBrushes[0][0] = PlotBrushes[0][1];
				PlotBrushes[1][0] = PlotBrushes[1][1];
				TrendSlope[0] = TrendSlope[1];
			}
			
	

			
			
			if (IsInSession[0] != 1)
			{
				Signals[0] = 0;
				
				//TrendLongOK[0] = 0;
				//TrendShortOK[0] = 0;				
			
			}
			
			
			
			FirstSignals[0] = Signals[0];
			
//			if (Signals[0] != OverallTrend[0])
//				Signals[0] = 0;
			
			// END TREND

			
			
			//Signals[0] = 1;
			
			
		//	Signal[0] = Signals[0];

			LastSignal = Signals[0];
	
			
			Values[2][0] = Signals[0];
			
			TradeStatus[0] = TradeStatus[1];
			
				LongPT1[0] = LongPT1[1];
				LongPT2[0] = LongPT2[1];
				LongPT3[0] = LongPT3[1];
				LongPTMax[0] = LongPTMax[1];
				LongPTMin[0] = LongPTMin[1];
				LongStop[0] = LongStop[1];
				LongEntry[0] = LongEntry[1];
				
				ShortPT1[0] = ShortPT1[1];
				ShortPT2[0] = ShortPT2[1];
				ShortPT3[0] = ShortPT3[1];
				ShortPTMax[0] = ShortPTMax[1];
				ShortPTMin[0] = ShortPTMin[1];
				ShortStop[0] = ShortStop[1];
				ShortEntry[0] = ShortEntry[1];
			
			
			
//			if (Signal[0] == 1)
//			{
//				TargetATRPrice[0] = Close[0] + TargetATR[0];
//				TargetATRPrice2[0] = Close[0] + TargetATR2[0];
//				StopATRPrice[0] = Close[0] - StopATR[0];	
//			}
			
//			if (Signal[0] == -1)
//			{
//				TargetATRPrice[0] = Close[0] - TargetATR[0];
//				TargetATRPrice2[0] = Close[0] - TargetATR2[0];
//				StopATRPrice[0] = Close[0] + StopATR[0];	
//			}
			
//			TargetATRPrice[0] = RTTS(TargetATRPrice[0]);
//			TargetATRPrice2[0] = RTTS(TargetATRPrice2[0]);
//			StopATRPrice[0] = RTTS(StopATRPrice[0]);
			
			

				if (Signals[0] == 1)
				{
					
					LongEntry[0] = Close[0];
					
					if (pPT1Enabled)	LongPT1[0] = LongEntry[0] + TargetATR[0];
					if (pPT2Enabled)	LongPT2[0] = LongEntry[0] + TargetATR2[0];
					
//					if (pPTCalc == "ATR")
//					{
//						LongPT1[0] = LongEntry[0] + TargetATR[0];
//						LongPT2[0] = LongEntry[0] + TargetATR2[0];
//						//LongPT3[0] = LongEntry[0] + iExitATR* pPT3Percent;
//					}
//					else
//					{
//						LongPT1[0] = LongEntry[0] + TickSize * pPT1Ticks;
//						LongPT2[0] = LongEntry[0] + TickSize * pPT2Ticks;
//						//LongPT3[0] = LongEntry[0] + TickSize * pPT3Ticks;						
//					}
					
					//LongPT1[0] = ttt;
				
					LongStop[0] = LongEntry[0] - StopATR[0];	
					LongPT3[0] = LongStop[0];
					
					
//					if (pSLCalc == "ATR")
//					{
//						LongStop[0] = LongEntry[0] - StopATR[0];	

//					}
//					else
//					{
//						LongStop[0] = LongEntry[0] - TickSize * pSLTicks;
//					}						
					
					
					//LongStop[0] = sss;		
					
//					if (pRTEnabled && pSLRiskEnabled && pSLRiskCalc == "Enforce")
//					{
					
						
//						if (LongEntry[0] - TickSize * pSLMaxTicks > LongStop[0])
//							LongStop[0] = LongEntry[0] - TickSize * pSLMaxTicks;
//					}
					
					
					if (pExitsRTTS)
					{
						LongPT1[0] = RTTS(LongPT1[0]);
						LongPT2[0] = RTTS(LongPT2[0]);
						LongPT3[0] = RTTS(LongPT3[0]);
						LongStop[0] = RTTS(LongStop[0]);				
					}

//					LongPT1[0] = Math.Max(LongPT1[0],RTTS(LongEntry[0] + pPTMinTicks*TickSize));
//					LongPT2[0] = Math.Max(LongPT2[0],RTTS(LongEntry[0] + pPTMinTicks*TickSize));
//					LongPT3[0] = Math.Max(LongPT3[0],RTTS(LongEntry[0] + pPTMinTicks*TickSize));
					
				
					TradeStatus[0] = 1;	
				}
			
				if (Signals[0] == -1)
				{
					
					
					ShortEntry[0] = Close[0];
					
					if (pPT1Enabled) ShortPT1[0] = ShortEntry[0] - TargetATR[0];
					if (pPT2Enabled)	ShortPT2[0] = ShortEntry[0] - TargetATR2[0];
					ShortStop[0] = ShortEntry[0] + StopATR[0];
					ShortPT3[0] = ShortStop[0];
					
//					if (pPTCalc == "ATR")
//					{
//						ShortPT1[0] = ShortEntry[0] - iExitATR[0] * pPT1Percent;
//						ShortPT2[0] = ShortEntry[0] - iExitATR[0] * pPT2Percent;
//						//ShortPT3[0] = ShortEntry[0] - iExitATR* pPT3Percent;
//					}
//					else
//					{
//						ShortPT1[0] = ShortEntry[0] - TickSize * pPT1Ticks;
//						ShortPT2[0] = ShortEntry[0] - TickSize * pPT2Ticks;
//						//ShortPT3[0] = ShortEntry[0] - TickSize * pPT3Ticks;						
//					}
					
//					//ShortPT1[0] = ttt;
				
//					if (pSLCalc == "ATR")
//					{
//						ShortStop[0] = ShortEntry[0] + iExitATR[0] * pSLPercent;

//					}
//					else
//					{
//						ShortStop[0] = ShortEntry[0] + TickSize * pSLTicks;
//					}						
					
				
					
//					if (pRTEnabled && pSLRiskEnabled && pSLRiskCalc == "Enforce")
//					{
					
						 
//						if (ShortEntry[0] + TickSize * pSLMaxTicks < ShortStop[0])
//							ShortStop[0] = ShortEntry[0] + TickSize * pSLMaxTicks;
//					}						
					
					//ShortStop[0] = sss;
					
				
					
					if (pExitsRTTS)
					{
						ShortPT1[0] = RTTS(ShortPT1[0]);
						ShortPT2[0] = RTTS(ShortPT2[0]);
						ShortPT3[0] = RTTS(ShortPT3[0]);
						ShortStop[0] = RTTS(ShortStop[0]);					
					}
					

					
//					ShortPT1[0] = Math.Min(ShortPT1[0],RTTS(ShortEntry[0] - pPTMinTicks*TickSize));
//					ShortPT2[0] = Math.Min(ShortPT2[0],RTTS(ShortEntry[0] - pPTMinTicks*TickSize));
//					ShortPT3[0] = Math.Min(ShortPT3[0],RTTS(ShortEntry[0] - pPTMinTicks*TickSize));
					
				
					TradeStatus[0] = -1;	
				}
						
				

				
				LongPTMax[0] = 0;
				ShortPTMax[0] = 100000000;
				LongPTMin[0] = 100000000;
				ShortPTMin[0] = 0;
				
				if (!pPT1Enabled)
				{
					LongPT1[0] = 0;
					ShortPT1[0] = 0;
				}
				if (!pPT2Enabled)
				{
					LongPT2[0] = 0;
					ShortPT2[0] = 0;
				}
//				if (!pPT3Enabled)
//				{
//					LongPT3[0] = 0;
//					ShortPT3[0] = 0;
//				}
				
				if (pPT1Enabled)
				{
					LongPTMax[0] = Math.Max(LongPT1[0],LongPTMax[0]);
					ShortPTMax[0] = Math.Min(ShortPT1[0],ShortPTMax[0]);
					LongPTMin[0] = Math.Min(LongPT1[0],LongPTMin[0]);
					ShortPTMin[0] = Math.Max(ShortPT1[0],ShortPTMin[0]);					
				}
				if (pPT2Enabled)
				{
					LongPTMax[0] = Math.Max(LongPT2[0],LongPTMax[0]);
					ShortPTMax[0] = Math.Min(ShortPT2[0],ShortPTMax[0]);
					LongPTMin[0] = Math.Min(LongPT2[0],LongPTMin[0]);
					ShortPTMin[0] = Math.Max(ShortPT2[0],ShortPTMin[0]);
				}
//				if (pPT3Enabled)
//				{
//					LongPTMax[0] = Math.Max(LongPT3[0],LongPTMax[0]);
//					ShortPTMax[0] = Math.Min(ShortPT3[0],ShortPTMax[0]);
//					LongPTMin[0] = Math.Min(LongPT3[0],LongPTMin[0]);
//					ShortPTMin[0] = Math.Max(ShortPT3[0],ShortPTMin[0]);
//				}

				
				if (ShortPTMax[0] == 100000000)
					ShortPTMax[0] = 0;
				if (LongPTMin[0] == 100000000)
					LongPTMin[0] = 0;	
				
				
			
				if (pSLTrailEnabled)
				{
					//if (ExitStatus[0] == 1)
					{
						
						
						
						if (TradeStatus[0] == 1 && Signals[0] != 1)	
						{
							
								 
							LongPT3[0] = Math.Max(LongPT3[1],RTTS(High[1] - TrailingStopATR[1] - pTSOffsetTicks*TickSize));
							
						}
						
						if (TradeStatus[0] == -1 && Signals[0] != -1)		
						{
							
							ShortPT3[0] = Math.Min(ShortPT3[1],RTTS(Low[1] + TrailingStopATR[1] + pTSOffsetTicks*TickSize));
							
						}
						
						
						
						
					}
					
					
				}
				
				
				
	// exit tracking
		
				if (pSLTrailEnabled) 
				{
					LongStopOut[0] = LongPT3[0];
					ShortStopOut[0] = ShortPT3[0];
					
					if (LongPT3[0] != LongPT3[1])
						LongStopOut[0] = LongPT3[1];
					
					if (ShortPT3[0] != ShortPT3[1])
						ShortStopOut[0] = ShortPT3[1];
					
					
				}	
				else
				{
					LongStopOut[0] = LongStop[0];
					ShortStopOut[0] = ShortStop[0];					
					
				}
					
					
				if (TradeStatus[0] == 1 && Signals[0] != 1)	
				{
					if (Low[0] <= LongStopOut[0])
					{
						
						TradeStatus[0] = 0;
						
//						if (ExitStatus[0] == 0)
//							TotalLosers[0] = TotalLosers[0] + 1;
					}
					
					if (!pRunnerTrailEnabled)
					if (High[0] >= LongPTMax[0] && LongPTMax[0] != 0)
						TradeStatus[0] = 0;		
					
					//if (
					
				}
				
				if (TradeStatus[0] == -1 && Signals[0] != -1)	
				{
					if (High[0] >= ShortStopOut[0])
					{
						TradeStatus[0] = 0;
						
//						if (ExitStatus[0] == 0)
//							TotalLosers[0] = TotalLosers[0] + 1;
					}
					
					if (!pRunnerTrailEnabled)
					if (Low[0] <= ShortPTMax[0] && ShortPTMax[0] != 0)
						TradeStatus[0] = 0;			
				}
				

				
				
				
				//bool innn = false;
				
				
				
				//if (innn)
				
//				Values[3].Reset(0);
//				Values[4].Reset(0);
//				Values[5].Reset(0);
//				Values[6].Reset(0);
				
				Values[3][0] = 0;
				Values[4][0] = 0;
				Values[5][0] = 0;
				Values[6][0] = 0;
				
				
				if (TradeStatus[0] == 1 || TradeStatus[1] == 1)
				{
					Values[3][0] = LongStop[0];
					Values[5][0] = LongPT1[0];
					Values[6][0] = LongPT2[0];
					if (pSLTrailEnabled && Signals[0] == 0) Values[4][0] = LongPT3[0];
				}
				else if (TradeStatus[0] == -1 || TradeStatus[1] == -1)
				{
					Values[3][0] = ShortStop[0];
					Values[5][0] = ShortPT1[0];
					Values[6][0] = ShortPT2[0];
					if (pSLTrailEnabled && Signals[0] == 0) Values[4][0] = ShortPT3[0];
				}
				else
				{
					
				}				
				
				LongT1 = LongPT1[0];
				LongT2 = LongPT2[0];
				LongStopM = LongStop[0];
				LongStopTr = LongPT3[0];
				
				ShortT1 = ShortPT1[0];
				ShortT2 = ShortPT2[0];
				ShortStopM = ShortStop[0];
				ShortStopTr = ShortPT3[0];
				
				// pExitOrdersEnabled
				
				if (myAccount != null && State == State.Realtime)
				if (pSLTrailEnabled && pSLTrailOrdersEnabled) 
				if (IsFirstTickOfBar)
				{
					UpdateStopLoss("TR");
					
				}
					
		
				
			

			// ALERTS
			
			BB = 0;
			if (Calculate != Calculate.OnBarClose && !pQuickAudio)
				BB = 1;
							
			if(pAudioEnabled)
			{
				
				if(Signals[BB] == 1 && LastAudioBar != CurrentBars[0])
				{
					Alert(CurrentBar.ToString(),Priority.High,"Veritas BOT Long | ",NinjaTrader.Core.Globals.InstallDir+@"\sounds\"+pWAVFileName,1,pArrowUpFBrush,Brushes.White);
					LastAudioBar = CurrentBars[0];
				}
				if(Signals[BB] == -1 && LastAudioBar != CurrentBars[0])
				{
					Alert(CurrentBar.ToString(),Priority.High,"Veritas BOT Short | ",NinjaTrader.Core.Globals.InstallDir+@"\sounds\"+pWAVFileName2,1,pArrowDownFBrush,Brushes.White);
					LastAudioBar = CurrentBars[0];
				}
			}
			
						
//			BB = 0;
//			if (Calculate != Calculate.OnBarClose && !pQuickEmail)
//				BB = 1;
			
//			if (pEmailEnabled)
//			{
				
//				if (Signal[BB] == 1 && LastEmailBar != CurrentBars[0])
//				{
//					//subject = Instrument.FullName + " " + BarsPeriod + " Chart  |  " + "New Signal Long " + Close[0].ToString(PriceString);
					
//					subject = "artMETS Buy Signal | " + Bars.BarsPeriod.ToString() + " Chart | " + Instrument.FullName;					
//					message = subject;

//					if (pEmailAddress != "" && State != State.Historical)
//						SendMail(pEmailAddress, subject, message);
					
//					LastEmailBar = CurrentBars[0];
//				}

//				if (Signal[BB] == -1 && LastEmailBar != CurrentBars[0])
//				{
//					//subject = Instrument.FullName + " " + BarsPeriod + " Chart  |  " + "New Signal Long " + Close[0].ToString(PriceString);
					
//					subject = "artMETS Buy Signal | " + Bars.BarsPeriod.ToString() + " Chart | " + Instrument.FullName;					
//					message = subject;

//					if (pEmailAddress != "" && State != State.Historical)
//						SendMail(pEmailAddress, subject, message);
					
//					LastEmailBar = CurrentBars[0];
//				}			
					
//			}
			
			
			SetBackground(0);
			
		
//			if (pBackEnabled)
//			{
				
//				BackBrushesAll[0] = null;
//				BackBrushes[0] = null;
				
				
//				if (OverallTrend[0] == 1)
//					SetBackColor(pBrush07);
				
//				if (OverallTrend[0] == -1)
//					SetBackColor(pBrush08);
				
//				if (IsInSession[0] != 1)
//				{					
//					SetBackColor(pBrush09);
//				}	
				
				
//				if (Signals[0] == 1)
//				{					
//					SetBackColor(pBrush01);
//				}				
//				else if (Signals[0] == -1)
//				{
//					SetBackColor(pBrush02);
//				}
				
				

				
//			}
			
			
			
			
//			if (pBackEnabled)
//			{
				
//				BackBrushesAll[0] = null;
//				BackBrushes[0] = null;
			
//				if (IsGap[0] == 1)
//				{					
//					SetBackColor(pBrush01);
//				}				
//				else if (IsGap[0] == -1)
//				{
//					SetBackColor(pBrush02);
//				}
				
//			}
						
			
//			if (pBackEnabled)
//			{
				
//				BackBrushesAll[0] = null;
//				BackBrushes[0] = null;
			
//				if (LongReady[0] == 1)
//				{					
//					SetBackColor(pBrush01);
//				}				
//				else if (ShortReady[0] == -1)
//				{
//					SetBackColor(pBrush02);
//				}
				
//			}
							
			
			
			
			
			if (IsCurrentBar)
			CheckExecutionStatus();
			
        }


		private void AddError(string eee)
		{
		
			if (pErrorMessagesEnabled)
			if (!AllErrorMessages.Contains(eee))
				AllErrorMessages.Add(eee);
			
		}
		
		private void RunPNL()
		{
			  
				
				TotalAccountPNL = 0;
				double TotalOpenPNL = 0;
				double TotalClosedPNL = 0;
				
				if (pIncludeCommish)
					TotalClosedPNL = myAccount.Get(AccountItem.RealizedProfitLoss, Currency.UsDollar);
				else
					TotalClosedPNL = myAccount.Get(AccountItem.GrossRealizedProfitLoss, Currency.UsDollar);
				
				TotalOpenPNL = myAccount.Get(AccountItem.UnrealizedProfitLoss, Currency.UsDollar);
				TotalOpenPNL = 0;
				
				
				TotalAccountPNL = TotalClosedPNL + TotalOpenPNL;
				
//				if (pDailyPNLTrailingEnabled)
//				{
//					pDL = Math.Max(pDL, Math.Round(TotalAccountPNL - pDLTrailingAmount, 2));
					
//				}
				
	
		
				//Print(TotalAccountPNL);
				
		
				if (pDGEnabled)
				if (PTotalAccountPNL != TotalAccountPNL)
				{
					//Print("RunPNL Update: " + TotalAccountPNL);
					
				
					if (pDGEnabledDisable)
					
					{
					
						//Print(TotalAccountPNL);
						
						
						if (TotalAccountPNL >= pDG)
						{
		          			//DailyGoalMessage = "Daily Goal has been exceeded.";
							
							DisableTheSystem();
						}
						else if (TotalAccountPNL <= pDL*-1)
						{
							//DailyGoalMessage = "Daily Loss has been exceeded.";
							
							DisableTheSystem();
						}
						else
						{
						
						}
						
						
						
						
//						if (TotalAccountPNL >= pDG)
//						{
//		          			NewBackBrush = pColorGoalBackground;
//							NewStatusBrush = pColorStatus2;
							
//						}
//						else if (TotalAccountPNL <= pDL)
//						{
//							NewBackBrush = pColorLossBackground;
//							NewStatusBrush = pColorStatus3;
//						}
//						else
//						{
//							NewBackBrush = pColorMainBackground;
//							NewStatusBrush = pColorStatus1;
//						}
						
//						if (ThisChartProperties.ChartBackground != NewBackBrush)
//						{
//							ThisChartProperties.ChartBackground = NewBackBrush;
//						}
						
//						if (newButton8.Foreground != NewStatusBrush)
//						{
//							newButton8.Foreground = NewStatusBrush;
//						}
												
					
//						newButton8.Content = "$ " + Math.Round(TotalAccountPNL,2).ToString("N2");
						
						
						
						
						
						//newButton9.Content = "MAX LOSS = $ " + Math.Round(pDL,2).ToString("N2");	
					
					}
					
//					chartTrader.Dispatcher.InvokeAsync(() =>
//					{
//						chartTrader.InvalidateVisual();
//						chartTrader.InvalidateVisual();
//					});
										
				}
				

				PTotalAccountPNL = TotalAccountPNL;
				
				
		}
			
		
		private void CheckExecutionStatus()
		{
			
						if (ChartControl != null)
			ChartControl.Dispatcher.InvokeAsync((Action)(() =>
			{
			
			CurrentSignal = LastSignal;
			
			if (pDoReverse && CurrentSignal != 0)
				CurrentSignal = CurrentSignal * -1;
				
			DoLong = false;
			DoShort = false;
			DoClose = false;

			
			//Print(AllTrends.Count);
//			if (AllTrends.Count == 0)
//						newButton4.Background = pButtonColorOn;						
//					else if (CurrentTrendStatus == 0)
//						newButton4.Background = pButtonColorOff;	
//					else if (CurrentTrendStatus == 1)
//						newButton4.Background = pButtonColorLong;	
//					else if (CurrentTrendStatus == -1)
//						newButton4.Background = pButtonColorShort;		
					
			if (pTrendOnlyEnabled && AllTrends.Count > 0 && CurrentTrendStatus != CurrentSignal)
				CurrentSignal = 0;
			
			
			
			
			if (!pEntriesEnabled)
				CurrentSignal = 0;
						
			//pEH = "UniqueEntries";
			
			if (CurrentSignal == 1 && pLongEnabled)
			{
				if (pAutoEnabled)
				{
					if (pEH == "AllEntries")
					{
						DoLong = true;
						
						
					}
					else if (pEH == "FlatEntries")
					{

						if (ThisPositionNow() == null)
							DoLong = true;
						
					}
					else if (pEH == "UniqueEntries")
					{

						if (ThisPositionNow() != null)
						{
							if (ThisPositionNow().MarketPosition != MarketPosition.Long)
							{
								DoClose = true;
								DoLong = true;				
							}
						}
						else
						{
							DoLong = true;
						
						}
					}
				}
				else
				{
					DoLong = true;
				}
			}
			
			if (CurrentSignal == -1 && pShortEnabled)
			{
				if (pAutoEnabled)
				{
					if (pEH == "AllEntries")
					{
						DoShort = true;
						
						
					}
					else if (pEH == "FlatEntries")
					{

						if (ThisPositionNow() == null)
							DoShort = true;
						
					}
					else if (pEH == "UniqueEntries")
					{

						if (ThisPositionNow() != null)
						{
							if (ThisPositionNow().MarketPosition != MarketPosition.Short)
							{
								DoClose = true;
								DoShort = true;
							}
						}
						else
						{
							DoShort = true;
						}
						
					}						
					
					
				}
				else
				{
					DoShort = true;
				}
			}			



			UpdateDisplay();
			
				
			}));
		}

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

		
        private double RTTS(double p)
        {
            return Instrument.MasterInstrument.RoundToTickSize(p);
        }

		private void Subscribe()
		{
	
			//Print("NEW EVENTS " + pAccountName);
			
			
		  if (myAccount != null)
		  {
		      // Unsubscribe to any prior account subscriptions
		      myAccount.AccountItemUpdate -= OnAccountItemUpdate;
			 // myAccount.StateChanged -= OnAccountStatusUpdate;
		      myAccount.ExecutionUpdate -= OnExecutionUpdate;
		      myAccount.OrderUpdate -= OnOrderUpdate;
		      myAccount.PositionUpdate -= OnPositionUpdate;
		 
		      // Subscribe to new account subscriptions
		      myAccount.AccountItemUpdate   += OnAccountItemUpdate;
			 // myAccount.StateChanged += OnAccountStatusUpdate;
		      myAccount.ExecutionUpdate     += OnExecutionUpdate;
		      myAccount.OrderUpdate         += OnOrderUpdate;
		      myAccount.PositionUpdate     += OnPositionUpdate;
			  
			  
		  }
		}
		 
		 private void OnAccountItemUpdate (object sender, AccountItemEventArgs e)
	    {
	         // Output the account item
	        
			if (myAccount != e.Account)
				return;
			
			//RunPNL();
						
	    }
	
		private void OnAccountStatusUpdate (object sender, AccountStatusEventArgs e)
		{
		    // Do something with the account status update
		}
		 
		 
		private void OnExecutionUpdate (object sender, ExecutionEventArgs e)
		{
		    // Do something with the execution update
		}
	 
	    private void OnPositionUpdate (object sender, PositionEventArgs e)
	    {
	         // Output the new position
	       
			if (e.Position.Instrument != Instrument || e.Position.Account.Name != pAccountName)
				return;
			
			
			//
			
			if (e.Quantity == 0)
				RunPNL();
			
			//Print(e.Quantity);
			
						
	    }
		
		private void OnOrderUpdate (object sender, OrderEventArgs  e)
		{
			
			

			Order ThisOrder = e.Order;
			
			if (ThisOrder.Instrument != Instrument)
				return;
			
			
			if (LongEntryOrder == null && SaveLongOrder)
			{						
				LongEntryOrder = ThisOrder;
				SaveLongOrder = false;
			}
			
			if (ThisOrder == LongEntryOrder)
			if (ThisOrder.OrderState == OrderState.Filled || ThisOrder.OrderState == OrderState.Cancelled)
			{
				
				if (ThisOrder.OrderState == OrderState.Filled)
				{

				}				
				
				LongEntryOrder = null;
			}
			
			if (ShortEntryOrder == null && SaveShortOrder)
			{						
				ShortEntryOrder = ThisOrder;
				SaveShortOrder = false;
				
				
			}
			
			if (ThisOrder == ShortEntryOrder)
			if (ThisOrder.OrderState == OrderState.Filled || ThisOrder.OrderState == OrderState.Cancelled)
			{
				if (ThisOrder.OrderState == OrderState.Filled)
				{
					
							
					
				}
				
				
				ShortEntryOrder = null;
			}
			
			
			// move profit target orders
			
			
			if (AllowInitialExitOrderMove)
			if (pExitOrdersEnabled)
			{
				if (ThisOrder.Name.Contains("Target1") || ThisOrder.Name.Contains("Target2"))
				{
					if (ThisOrder.OrderState == OrderState.Working)
					{
			
						if (!MovedTargets.Contains(ThisOrder.OrderId))
							UpdateProfitTargets(ThisOrder.Name);
						
					}
					
				}
				
				
				if (ThisOrder.Name.Contains("Stop"))
				{
				
					if (ThisOrder.OrderState == OrderState.Accepted)
					{
						
						if (!MovedStops.Contains(ThisOrder.OrderId))
						UpdateStopLoss("IN");
						
						
					}
					
				}		
				
				
				
			}
			

		}		
	
		
		
		private Position ThisPositionNow()
		{
			
			
			
			foreach (Position pos in myAccount.Positions)
			{
				if (pos.Instrument == Instrument)
					return pos;
				
			}
				
			// NT7
			
//			foreach (Account acct in Cbi.Globals.Accounts)
//			{
				
//				if (acct.Name == pAccountName)
//				{
					
//					return acct.Positions.FindByInstrument(Instruments[0]);				
//				}
//			}
			
			
			return null;
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
			
//		    int nThreshold = 150;
//		    int bgDelta = Convert.ToInt32((bg.R * 0.299) + (bg.G * 0.587) + 
//		                                  (bg.B * 0.114));

//		    Brush foreColor = (255 - bgDelta < nThreshold) ? Brushes.Black : Brushes.White;
//		    return foreColor;
		}

		
		
		
	public override void OnRenderTargetChanged()
		{

			if (RenderTarget != null)
			{
				pArrowUpStroke.RenderTarget = RenderTarget;
				pArrowDownStroke.RenderTarget = RenderTarget;
				
					pStopStroke.RenderTarget = RenderTarget;
				pTargetStroke.RenderTarget = RenderTarget;		
				
				
				pOrderUpOutlineStroke.RenderTarget = RenderTarget;
				pOrderDnOutlineStroke.RenderTarget = RenderTarget;
				pOrderBothOutlineStroke.RenderTarget = RenderTarget;						
						
				ThisStroke.RenderTarget = RenderTarget;

				// --- Dispose previous cached brushes ---
				if (cachedChartTextBrushDX != null) { cachedChartTextBrushDX.Dispose(); cachedChartTextBrushDX = null; }
				if (cachedChartBackgroundBrushDX != null) { cachedChartBackgroundBrushDX.Dispose(); cachedChartBackgroundBrushDX = null; }
				if (cachedChartBackgroundFadeBrushDX != null) { cachedChartBackgroundFadeBrushDX.Dispose(); cachedChartBackgroundFadeBrushDX = null; }
				if (cachedSelectBrushDX != null) { cachedSelectBrushDX.Dispose(); cachedSelectBrushDX = null; }
				if (cachedTextBrushDX2 != null) { cachedTextBrushDX2.Dispose(); cachedTextBrushDX2 = null; }
				if (cachedTextBrushDX != null) { cachedTextBrushDX.Dispose(); cachedTextBrushDX = null; }
				if (cachedUpBrushDX != null) { cachedUpBrushDX.Dispose(); cachedUpBrushDX = null; }
				if (cachedDownBrushDX != null) { cachedDownBrushDX.Dispose(); cachedDownBrushDX = null; }
				if (cachedFinalBrushDX != null) { cachedFinalBrushDX.Dispose(); cachedFinalBrushDX = null; }
				if (cachedLineBrushDX != null) { cachedLineBrushDX.Dispose(); cachedLineBrushDX = null; }
				if (cachedLongBrushDX != null) { cachedLongBrushDX.Dispose(); cachedLongBrushDX = null; }
				if (cachedBlackBrushDX != null) { cachedBlackBrushDX.Dispose(); cachedBlackBrushDX = null; }
				if (cachedButtonBrushDX != null) { cachedButtonBrushDX.Dispose(); cachedButtonBrushDX = null; }
				if (cachedButtonHBrushDX != null) { cachedButtonHBrushDX.Dispose(); cachedButtonHBrushDX = null; }
				if (cachedButtonFHBrushDX != null) { cachedButtonFHBrushDX.Dispose(); cachedButtonFHBrushDX = null; }
				if (cachedButtonFOFFBrushDX != null) { cachedButtonFOFFBrushDX.Dispose(); cachedButtonFOFFBrushDX = null; }
				if (cachedButtonFONBrushDX != null) { cachedButtonFONBrushDX.Dispose(); cachedButtonFONBrushDX = null; }
				if (cachedArrowUpBrushDX != null) { cachedArrowUpBrushDX.Dispose(); cachedArrowUpBrushDX = null; }
				if (cachedArrowDownBrushDX != null) { cachedArrowDownBrushDX.Dispose(); cachedArrowDownBrushDX = null; }
				if (cachedStopLineBrushDX != null) { cachedStopLineBrushDX.Dispose(); cachedStopLineBrushDX = null; }
				if (cachedTargetLineBrushDX != null) { cachedTargetLineBrushDX.Dispose(); cachedTargetLineBrushDX = null; }
				if (cachedPlotDownBrush33DX != null) { cachedPlotDownBrush33DX.Dispose(); cachedPlotDownBrush33DX = null; }
				if (cachedTimerBrushDX != null) { cachedTimerBrushDX.Dispose(); cachedTimerBrushDX = null; }
				if (cachedTableTextBrushDX != null) { cachedTableTextBrushDX.Dispose(); cachedTableTextBrushDX = null; }
				if (cachedFillUpBrushDX != null) { cachedFillUpBrushDX.Dispose(); cachedFillUpBrushDX = null; }
				if (cachedFillDownBrushDX != null) { cachedFillDownBrushDX.Dispose(); cachedFillDownBrushDX = null; }
				if (cachedFillNeutralBrushDX != null) { cachedFillNeutralBrushDX.Dispose(); cachedFillNeutralBrushDX = null; }
				if (cachedAreaBrushDX != null) { cachedAreaBrushDX.Dispose(); cachedAreaBrushDX = null; }
				if (cachedActiveOutlineBrushDX != null) { cachedActiveOutlineBrushDX.Dispose(); cachedActiveOutlineBrushDX = null; }
				if (cachedSmallAreaBrushDX != null) { cachedSmallAreaBrushDX.Dispose(); cachedSmallAreaBrushDX = null; }
				if (cachedOutlineUp03BrushDX != null) { cachedOutlineUp03BrushDX.Dispose(); cachedOutlineUp03BrushDX = null; }
				if (cachedOutlineDn03BrushDX != null) { cachedOutlineDn03BrushDX.Dispose(); cachedOutlineDn03BrushDX = null; }
				if (cachedOutlineBoth03BrushDX != null) { cachedOutlineBoth03BrushDX.Dispose(); cachedOutlineBoth03BrushDX = null; }
				if (cachedStop50BrushDX != null) { cachedStop50BrushDX.Dispose(); cachedStop50BrushDX = null; }
				if (cachedTarget50BrushDX != null) { cachedTarget50BrushDX.Dispose(); cachedTarget50BrushDX = null; }

				// --- Dispose previous cached TextFormats ---
				if (cachedLabelTextFormat != null) { cachedLabelTextFormat.Dispose(); cachedLabelTextFormat = null; }
				if (cachedTextFont2Format != null) { cachedTextFont2Format.Dispose(); cachedTextFont2Format = null; }
				if (cachedTextFontTimeFormat != null) { cachedTextFontTimeFormat.Dispose(); cachedTextFontTimeFormat = null; }
				if (cachedTextFontFormat != null) { cachedTextFontFormat.Dispose(); cachedTextFontFormat = null; }
				if (cachedTextFont8Format != null) { cachedTextFont8Format.Dispose(); cachedTextFont8Format = null; }
				if (cachedTableTextFormat != null) { cachedTableTextFormat.Dispose(); cachedTableTextFormat = null; }
				if (cachedButtonTextFormat != null) { cachedButtonTextFormat.Dispose(); cachedButtonTextFormat = null; }

				// --- Create fixed-color brushes ---
				cachedSelectBrushDX = Brushes.Yellow.ToDxBrush(RenderTarget);
				cachedTextBrushDX2 = Brushes.White.ToDxBrush(RenderTarget);
				cachedTextBrushDX = Brushes.RoyalBlue.ToDxBrush(RenderTarget);
				cachedUpBrushDX = Brushes.LimeGreen.ToDxBrush(RenderTarget);
				cachedDownBrushDX = Brushes.Red.ToDxBrush(RenderTarget);
				cachedFinalBrushDX = Brushes.Red.ToDxBrush(RenderTarget);
				cachedLineBrushDX = Brushes.RoyalBlue.ToDxBrush(RenderTarget);
				cachedLongBrushDX = Brushes.Green.ToDxBrush(RenderTarget);
				cachedBlackBrushDX = Brushes.Black.ToDxBrush(RenderTarget);

				// --- Create property-dependent brushes ---
				cachedArrowUpBrushDX = pArrowUpFBrush.ToDxBrush(RenderTarget);
				cachedArrowDownBrushDX = pArrowDownFBrush.ToDxBrush(RenderTarget);
				cachedStopLineBrushDX = pStopStroke.Brush.ToDxBrush(RenderTarget);
				cachedTargetLineBrushDX = pTargetStroke.Brush.ToDxBrush(RenderTarget);
				cachedPlotDownBrush33DX = pPlotDownFBrush33.ToDxBrush(RenderTarget);
				cachedTimerBrushDX = pTimerMainColor.ToDxBrush(RenderTarget);
				cachedTableTextBrushDX = pColorTextBrush.ToDxBrush(RenderTarget);
				cachedFillUpBrushDX = pFillUpBrush.ToDxBrush(RenderTarget);
				cachedFillDownBrushDX = pFillDownBrush.ToDxBrush(RenderTarget);
				cachedFillNeutralBrushDX = pFillNeutralBrush.ToDxBrush(RenderTarget);
				cachedAreaBrushDX = areaBrush.ToDxBrush(RenderTarget);
				cachedSmallAreaBrushDX = smallAreaBrush.ToDxBrush(RenderTarget);

				cachedOutlineUp03BrushDX = pOrderUpOutlineStroke.Brush.ToDxBrush(RenderTarget);
				cachedOutlineUp03BrushDX.Opacity = 0.3f;
				cachedOutlineDn03BrushDX = pOrderDnOutlineStroke.Brush.ToDxBrush(RenderTarget);
				cachedOutlineDn03BrushDX.Opacity = 0.3f;
				cachedOutlineBoth03BrushDX = pOrderBothOutlineStroke.Brush.ToDxBrush(RenderTarget);
				cachedOutlineBoth03BrushDX.Opacity = 0.3f;

				cachedStop50BrushDX = pStopStroke.Brush.ToDxBrush(RenderTarget);
				cachedStop50BrushDX.Opacity = 0.5f;
				cachedTarget50BrushDX = pTargetStroke.Brush.ToDxBrush(RenderTarget);
				cachedTarget50BrushDX.Opacity = 0.5f;

				// --- Create chart-property-dependent brushes ---
				if (ChartControl != null)
				{
					cachedChartTextBrushDX = ChartControl.Properties.ChartText.ToDxBrush(RenderTarget);
					cachedChartBackgroundBrushDX = ChartControl.Properties.ChartBackground.ToDxBrush(RenderTarget);
					cachedChartBackgroundFadeBrushDX = ChartControl.Properties.ChartBackground.ToDxBrush(RenderTarget);
					cachedChartBackgroundFadeBrushDX.Opacity = 80/100f;

					Brush textColorBrush = GetTextColor(ChartControl.Properties.ChartBackground);
					cachedButtonBrushDX = textColorBrush.ToDxBrush(RenderTarget);
					cachedButtonHBrushDX = textColorBrush.ToDxBrush(RenderTarget);
					cachedButtonFHBrushDX = ChartControl.Properties.AxisPen.Brush.ToDxBrush(RenderTarget);
					cachedButtonFOFFBrushDX = ChartControl.Properties.ChartBackground.ToDxBrush(RenderTarget);
					cachedButtonFONBrushDX = areaBrush.ToDxBrush(RenderTarget);

					if (ChartControl.Properties.ChartBackground != Brushes.Black)
					{
						cachedButtonHBrushDX.Opacity = 0.4f;
						cachedButtonFHBrushDX.Opacity = 0.0f;
					}
				}

				// --- Create cached TextFormats ---
				if (ChartControl != null)
				{
					cachedLabelTextFormat = ChartControl.Properties.LabelFont.ToDirectWriteTextFormat();
					cachedButtonTextFormat = ChartControl.Properties.LabelFont.ToDirectWriteTextFormat();
					cachedButtonTextFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
					cachedButtonTextFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
					cachedButtonTextFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;
				}

				cachedTextFont2Format = pTextFont2.ToDirectWriteTextFormat();
				cachedTextFontTimeFormat = pTextFontTime.ToDirectWriteTextFormat();
				cachedTextFontFormat = pTextFont.ToDirectWriteTextFormat();
				cachedTextFont8Format = pTextFont8.ToDirectWriteTextFormat();
				if (TextFont != null)
					cachedTableTextFormat = TextFont.ToDirectWriteTextFormat();
			}

		}
		
//		protected override void OnRender(ChartControl chartControl, ChartScale chartScale)
//		{	
			
			
			
			
			
//			if (!Permission)
//				return;
			
			
//			if (CurrentBars[0] < 0)
//				return;

////            if (Plots[0].BrushDX == null || Plots[1].BrushDX == null)
////                return;

//            // Instantiate a ChartControlProperties object to hold a reference to chartControl.Properties
//            ChartControlProperties myProperties = chartControl.Properties;


//			myProperties.BarMarginRight = pNewMarginRight;
			
			
//			if (pCTButtonsEnabled)
//			{
				
//				chartTrader = Window.GetWindow(ChartControl.Parent).FindFirst("ChartWindowChartTraderControl") as ChartTrader;
//				 if (chartTrader != null && FirstRender)
//				 {
					 
					
//			          //  ChartTraderProperties myprop = chartTrader.FindFirst("ChartTraderProperties") as NinjaTrader.Gui.Chart.ChartTraderProperties;
//						ChartTraderProperties myprop = chartTrader.Properties;
//					 myprop.AtmStrategySelectionMode = AtmStrategySelectionMode.KeepSelectedAtmStrategyTemplateOnOrderSubmission;
					 
					 
//					 myprop.ShowRealizedPnLWhenFlat = pShowReal;
					 
//					 if (chartTrader.ChartTraderVisibility == ChartTraderVisibility.Visible)
//					 {
						 
						 
//						//
					 	
//					  //Print("hello");
						 
						 
//					 	if (ChartControl != null)
//						{
//							ChartControl.Dispatcher.InvokeAsync((Action)(() =>
//							{
//								InsertWPFControls();
//								RunPNL();
//							}));
//						}
					
					
//						 FirstRender = false;
//					 }
//				 }
				 
//			}
				
			
//		//	Print("S1");
			
			
			

//			oldAntialiasMode	= RenderTarget.AntialiasMode;
//            //RenderTarget.AntialiasMode							= SharpDX.Direct2D1.AntialiasMode.Aliased;

			
			
			
//			ChartTextBrushDX = chartControl.Properties.ChartText.ToDxBrush(RenderTarget);
//			ChartBackgroundBrushDX = chartControl.Properties.ChartBackground.ToDxBrush(RenderTarget);	
			 			
//			ChartBackgroundErrorBrushDX = Brushes.Red.ToDxBrush(RenderTarget);
//			ChartBackgroundErrorBrushDX.Opacity = 25/100f;
			
//            SharpDX.DirectWrite.TextFormat CenterText = new SharpDX.DirectWrite.TextFormat(Core.Globals.DirectWriteFactory,
//                "Arial",
//                SharpDX.DirectWrite.FontWeight.Normal,
//                SharpDX.DirectWrite.FontStyle.Normal,
//                SharpDX.DirectWrite.FontStretch.Normal,
//                11.0F);
			
//			CenterText = ChartControl.Properties.LabelFont.ToDirectWriteTextFormat();
//			CenterText = new SimpleFont(ChartControl.Properties.LabelFont.Family.ToString(), 16).ToDirectWriteTextFormat();
//            CenterText.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
//            CenterText.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
//            CenterText.WordWrapping = SharpDX.DirectWrite.WordWrapping.Wrap;
			
//			//CellFormat = FinalFont1.ToDirectWriteTextFormat();
			
//			SharpDX.RectangleF CenterRect = new SharpDX.RectangleF(ChartPanel.X, ChartPanel.Y, ChartPanel.W, ChartPanel.H);
			
//			string txt = string.Empty;
			
			
//			if (!IsInHitTest)
// 			if (AllErrorMessages.Count > 0)
//			{
//				RenderTarget.AntialiasMode							= SharpDX.Direct2D1.AntialiasMode.Aliased;
				
//				txt = string.Empty;
				
//				foreach (string sss in AllErrorMessages)
//				txt = txt + sss + "\r\n\r\n";
			
//				txt = txt + "Click here to continue.";
//				//Print(text);
//				RenderTarget.FillRectangle(CenterRect, ChartBackgroundBrushDX);
//				RenderTarget.FillRectangle(CenterRect, ChartBackgroundErrorBrushDX);
//				RenderTarget.DrawText(txt, CenterText, ExpandRect(CenterRect,-10,0), ChartTextBrushDX);
				
				
//				RenderTarget.AntialiasMode = oldAntialiasMode;
				
//				return;
//			}
			
//			ChartBackgroundErrorBrushDX.Dispose();
			
			
	
		

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
		private SharpDX.Direct2D1.Brush ChartBackgroundErrorBrushDX = null;
		private SharpDX.Direct2D1.Brush ThisBrushDX = null;
	
				
		
		
		protected override void OnRender(ChartControl chartControl, ChartScale chartScale)
		{		
			
			//Print("bug1");
			
			
			//try
			//{
				
	
				
			//	if (!IsInHitTest)
			//if (pVisualEnabled)			
			//	base.OnRender(chartControl, chartScale);
			
	
			
			
			oldAntialiasMode	= RenderTarget.AntialiasMode;
      

			if (FirstRender2)
			{
			
//				ChartBarsSwitch2(true);

				
            	myProperties = chartControl.Properties;
//				PreviousDrag = myProperties.AllowSelectionDragging;
				
				
				
				
				
				//chartTrader = Window.GetWindow(ChartControl.Parent).FindFirst("ChartWindowChartTraderControl") as ChartTrader;	
				
				FirstRender2 = false;
				
				
			}
		
				
				ChartTextBrushDX = cachedChartTextBrushDX;
				ChartBackgroundBrushDX = cachedChartBackgroundBrushDX;
							

			
			if (!IsInHitTest)
 			if (AllErrorMessages.Count > 0)
				{
				
					
//					ChartBarsSwitch2(false);
//					myProperties.AllowSelectionDragging = false;
					
					
					
				ChartBackgroundErrorBrushDX = new SharpDX.Direct2D1.SolidColorBrush(RenderTarget, SharpDX.Color.Red);
				ChartBackgroundErrorBrushDX.Opacity = 25/100f;
				
				CenterText = new SimpleFont(ChartControl.Properties.LabelFont.Family.ToString(), 16).ToDirectWriteTextFormat();
	            CenterText.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
	            CenterText.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
	            CenterText.WordWrapping = SharpDX.DirectWrite.WordWrapping.Wrap;
				
				//CellFormat = FinalFont1.ToDirectWriteTextFormat();
				
				CenterRect = new SharpDX.RectangleF(ChartPanel.X, ChartPanel.Y, ChartPanel.W, ChartPanel.H);
				
				
				
				
				
					RenderTarget.AntialiasMode							= SharpDX.Direct2D1.AntialiasMode.Aliased;
					
						string txt = string.Empty;
					
					foreach (string sss in AllErrorMessages)
					txt = txt + sss + "\r\n\r\n";
				
					txt = txt + "Click here to continue.";
					//Print(text);
					RenderTarget.FillRectangle(CenterRect, ChartBackgroundBrushDX);
					RenderTarget.FillRectangle(CenterRect, ChartBackgroundErrorBrushDX);
					RenderTarget.DrawText(txt, CenterText, ExpandRect(CenterRect,-10,0), ChartTextBrushDX);
					
					
					RenderTarget.AntialiasMode = oldAntialiasMode;
					
				
				
				ChartBackgroundErrorBrushDX.Dispose();		
				CenterText.Dispose();	
				
					//Print("bug2");
					
				return;
			}
			
			
			
            // Set the AllowSelectionDragging property to false
           // myProperties.AllowSelectionDragging = false;
	
            if (pVisualEnabled)
			base.OnRender(chartControl, chartScale);

			// Use cached brushes from OnRenderTargetChanged instead of per-frame ToDxBrush
            SharpDX.Direct2D1.Brush selectBrushDX = cachedSelectBrushDX;
            SharpDX.Direct2D1.Brush textBrushDX2 = cachedTextBrushDX2;
            SharpDX.Direct2D1.Brush textBrushDX = cachedTextBrushDX;
            SharpDX.Direct2D1.Brush upBrushDX = cachedUpBrushDX;
            SharpDX.Direct2D1.Brush downBrushDX = cachedDownBrushDX;
            SharpDX.Direct2D1.Brush finalBrushDX = cachedFinalBrushDX;
            SharpDX.Direct2D1.Brush lineBrushDX = cachedLineBrushDX;
            SharpDX.Direct2D1.Brush longBrushDX = cachedLongBrushDX;
            SharpDX.Direct2D1.Brush blackBrushDX = cachedBlackBrushDX;
            SharpDX.Direct2D1.Brush buttonBrushDX = cachedButtonBrushDX;
            SharpDX.Direct2D1.Brush buttonHBrushDX = cachedButtonHBrushDX;
            SharpDX.Direct2D1.Brush buttonFHBrushDX = cachedButtonFHBrushDX;
            SharpDX.Direct2D1.Brush buttonFOFFBrushDX = cachedButtonFOFFBrushDX;
            SharpDX.Direct2D1.Brush buttonFONBrushDX = cachedButtonFONBrushDX;

			

            float CY = (float)chartControl.CanvasRight - 48f;
            //Print(CY);
           
            //SizeF szvv = graphics.MeasureString("B", ChartControl.Font);


            //bsize2 = (int)szvv.Height + 2;
           


			
	

          

            //SharpDX.RectangleF rectangleF = new SharpDX.RectangleF( (float) MP.X-5F, 0F, 10F, 10000F );
            //SharpDX.Direct2D1.Brush textBrushDX2 = textBrushDX.

            FB = ChartBars.FromIndex;
            LB = ChartBars.ToIndex;
            BB = 0;
			
			
            int xt = 0;
            int yt = 0;

            //if (Calculate.OnBarClose)
            LB = Math.Min(CurrentBars[0], LB);
            BB = CurrentBars[0] - LB;

			// resolve issue with other windows 10 font scaling automatically
			
//				if (ChartControl != null)
//				{
//					ChartControl.Dispatcher.InvokeAsync((Action)(() =>
//					{
									
//						if (this.ChartPanel != null)
				        if (dpiX == 0)
				        {
							PresentationSource source = PresentationSource.FromVisual(this.ChartPanel);
							
							if (source != null)
							{
				            dpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11;
				            dpiY = 96.0 * source.CompositionTarget.TransformToDevice.M22;
							
				             dpiX = 100.0 * source.CompositionTarget.TransformToDevice.M11;
				            dpiY = 100.0 * source.CompositionTarget.TransformToDevice.M22;
							}
							
						}
						
				
//					}));
//				}
				
				
			//Print(dpiX);
			
			FinalXPixel = MP.X / 100 * dpiX;
			FinalYPixel = MP.Y / 100 * dpiY;
			
						
						
						
           CurrentMousePrice = chartScale.MaxValue - chartScale.MaxMinusMin * (MP.Y / chartScale.Height);

			CurrentMousePrice = RTTS(CurrentMousePrice);

            

            double mousebar = (ChartControl.GetXByBarIndex(ChartBars, ChartBars.ToIndex) - ChartControl.GetXByBarIndex(ChartBars, ChartBars.FromIndex)) / Math.Max(1,(ChartBars.ToIndex - ChartBars.FromIndex)); //chartControl.GetBarPaintWidth(ChartBars);

            double mousebar2 = ChartBars.FromIndex + (MP.X - ChartControl.GetXByBarIndex(ChartBars, ChartBars.FromIndex)) / mousebar;

            int mousebar3 = (int) Math.Round(mousebar2, 0);

            SharpDX.DirectWrite.TextFormat textFormat = cachedLabelTextFormat;
						
						
		
            //{ TextAlignment = SharpDX.DirectWrite.TextAlignment.Leading, WordWrapping = WordWrapping.NoWrap };

            String text = "Mouse: (" + this.MP.X.ToString() + " , " + this.MP.Y.ToString() + " )" + Environment.NewLine;

            text = CurrentMousePrice.ToString(PriceString) + " " + mousebar3.ToString() + " " + CurrentBar.ToString();
			text = "NinjaTrader  " + CurrentMousePrice.ToString(PriceString) + " " + mousebar3.ToString() + " SLO = " + pSLOffset.ToString();
			
			
            SharpDX.RectangleF rectangleF = new SharpDX.RectangleF(10F, 20F, 500, 50F);

            
			DateTime TTT = ChartBars.GetTimeByBarIdx(chartControl,ChartBars.ToIndex);
			
			text = text + TTT.DayOfWeek.ToString();


						
						   
						
						
						double yt22 = chartScale.GetYByValue(chartScale.MinValue) - 17;
            SharpDX.RectangleF BottomRect = new SharpDX.RectangleF(160, (float)yt22, 2000, 21);

            

            SharpDX.DirectWrite.TextFormat BottomText = cachedTextFont2Format;

			
			

				
					
			
			string sep = "   |   ";
			string trailing = "Daily Loss Trailing is enabled.  We are trailing the maximum profit for today by $ " + pDLTrailingAmount.ToString("N2") + ".";
			
				trailing = "Daily Loss Trailing by $ " + pDLTrailingAmount.ToString("N2") + " behind the maximum profit is enabled.";
			
			if (!pDailyPNLTrailingEnabled)
			{
				trailing = "Daily Loss Trailing is disabled.  We will adhere to the fixed Daily Loss of $ " + pDL.ToString("N2") + ".";
				trailing = "Daily Loss Trailing is disabled.";
			}
				
			trailing = string.Empty;
			
			text = "Daily Goal =  $ " + pDG.ToString("N2") + sep + "Daily Loss =  $ " + pDL.ToString("N2") + sep + trailing;
			
			oldAntialiasMode	= RenderTarget.AntialiasMode;		
			RenderTarget.AntialiasMode							= SharpDX.Direct2D1.AntialiasMode.PerPrimitive;			
			
			textBrushDX2 = cachedTextBrushDX2;
			
			
			bool pTextDisplayEnabled = true;
			
			//if (pDGEnabled && pTextDisplayEnabled) RenderTarget.DrawText(text, BottomText, BottomRect, textBrushDX2);

			RenderTarget.AntialiasMode = oldAntialiasMode;
			
          



			
			
			
			
			double percentorderl = 50;
			double orderlinew = ChartPanel.W / 100 * percentorderl;
			
			double yyyyy = chartScale.GetYByValue(CurrentMousePrice);
			
			
			Point startPoint	= new Point(ChartPanel.W-orderlinew, MP.Y);
			Point endPoint		= new Point(ChartPanel.W, MP.Y);
			
			startPoint	= new Point(ChartPanel.W-orderlinew, yyyyy);
			endPoint		= new Point(ChartPanel.W, yyyyy);

            SharpDX.DirectWrite.TextFormat PrintText = cachedLabelTextFormat;

            string tttgdf = string.Empty;



			
				
				
				
// Set text to chart label color and font
			//textFormat			= chartControl.Properties.LabelFont.ToDirectWriteTextFormat();

//			// Loop through each Plot Values on the chart
			
			Point endPoint1		= new Point(ChartPanel.X + ChartPanel.W, ChartPanel.Y);
			Point endPoint2		= new Point(ChartPanel.X + ChartPanel.W, ChartPanel.Y);
			Point nextPoint		= new Point(ChartPanel.X + ChartPanel.W, ChartPanel.Y);			
			SharpDX.Direct2D1.Brush			smallAreaBrushDx	= null;
			
		
			if (!IsInHitTest)
				
				if (pArrowsEnabled)
			if (pExitOEnabled)
			for (int seriesCount = 0; seriesCount < Values.Length; seriesCount++)
			{
				
//                AddPlot(new Stroke(Brushes.Magenta, 2), PlotStyle.Line, "Fast EMA");
//				 AddPlot(new Stroke(Brushes.Magenta, 1), PlotStyle.Line, "Slow EMA");
//				AddPlot(new Stroke(Brushes.Transparent, 1), PlotStyle.Line, "Signals");
				

//				AddPlot(new Stroke(Brushes.Red, 2), PlotStyle.Hash, "Stop Loss"); // 3
//				AddPlot(new Stroke(Brushes.Red, 3), PlotStyle.Dot, "Trailing Stop Loss");
//				AddPlot(new Stroke(Brushes.Gray, 2), PlotStyle.Hash, "Target 1");
//				AddPlot(new Stroke(Brushes.Gray, 2), PlotStyle.Hash, "Target 2");
				
				
				

				
				
				
	
				
				if (seriesCount == 0 || seriesCount == 1 || seriesCount == 2)
				//if (seriesCount == 0 || seriesCount == 1 || seriesCount == 2 || seriesCount == 4)
					continue;
				
				
						

				

			
		
				
//				if (seriesCount <= 3 && !pShowToday)
//					continue;
				
//				if (seriesCount > 3 && !pShowYesterday)
//					continue;				
		
//				if (seriesCount <= 1 || seriesCount >=4)
//					continue;
					
				
				
					
				double	y					= -1;
				double	startX				= -1;
				double	endX				= -1;
				int		firstBarIdxToPaint	= -1;
				int		firstBarPainted		= ChartBars.FromIndex;
				int		lastBarPainted		= ChartBars.ToIndex;
				Plot	plot				= Plots[seriesCount];

				//blueBrush = BrushSeries[seriesCount];
				
				//lineBrushDx = Plots[0].DashStyleDX;
				
				
//				smallAreaBrush	= plot.Brush;
//				smallAreaBrushDx	= smallAreaBrush.ToDxBrush(RenderTarget);
//				smallAreaBrushDx.Opacity = areaOpacity/100F;
				
				// Use cached 50%-opacity stop/target brushes instead of allocating per series
				smallAreaBrushDx = (seriesCount == 3) ? cachedStop50BrushDX : cachedTarget50BrushDX;
				
				
//					Plots[3].Width = pStopStroke.Width;							
//					Plots[5].Width = pTargetStroke.Width;	
//					Plots[6].Width = pTargetStroke.Width;	
					
//					Plots[3].DashStyleHelper = pStopStroke.DashStyleHelper;
//					Plots[5].DashStyleHelper = pTargetStroke.DashStyleHelper;	
//					Plots[6].DashStyleHelper = pTargetStroke.DashStyleHelper;						
						
//					Plots[3].Brush = pStopStroke.Brush;
//					Plots[5].Brush = pTargetStroke.Brush;	
//					Plots[6].Brush = pTargetStroke.Brush;	
				
				
//				int FB = ChartBars.FromIndex;
//				int LB = ChartBars.ToIndex;
				
				preval = 0;
				
				//Print("===== " + lastBarPainted + "   " + CurrentBars[0]);
				
				lastBarPainted		= ChartBars.ToIndex;
				
				FirstOne = true;
				
				// Loop through visble bars to render plot values	
				
				int startbar = firstBarPainted;
				//startbar = 0;
				
				
				
				
				
				DateTime StartBarTime = Times[0].GetValueAt(FB);
				DateTime EndBarTime = Times[0].GetValueAt(LB);
				
				
				DateTime StartDrawTime = StartBarTime.AddDays(-4);
				
				//if (StartBarTime.Ticks <= EndBarTime.Ticks)
				//	EndBarTime = StartBarTime.AddDays(-2);
				
				
				startbar = Math.Max(0,Bars.GetBar(StartBarTime));
				
				//Print(EndBarTime);
				
				//int startbar = GetValueAt(
				
				startbar = FB-30; // fis this later
				int endbar = lastBarPainted;
				//endbar = CurrentBars[0];
				
				//startbar = endbar;
				
				if (!pExitHOEnabled)
					startbar = (int) LastSignalBar.GetValueAt(CurrentBars[0]) - 0 - 1;
				
				
				startbar = Math.Max(0, startbar); 
				
				
				for (int idx = endbar; idx >= startbar; idx--)
				//for (int idx = endbar; idx >= Math.Max(startbar, endbar - width); idx--)
				{
					//Print(idx);
					
					//if (idx < firstBarIdxToPaint)
					//	break;

					int adjust = 0;
					
					int barsadjust = 1;
					
					if (seriesCount == 4)
						barsadjust = 0;

					
					startX		= chartControl.GetXByBarIndex(ChartBars, idx - barsadjust) - adjust;
					endX		= chartControl.GetXByBarIndex(ChartBars, idx + 1 - barsadjust) - adjust;
					
					
					if (idx == lastBarPainted)
					{
						//Print(idx + "  " + endX);
						endX		= endX+20;
						//Print(endX);
					}
						
					preval = val;
					val	= Values[seriesCount].GetValueAt(idx);
					y			= chartScale.GetYByValue(val);
					
					
					//if (Signals.GetValueAt(idx) == 0)
					if (seriesCount == 4)
					{
						x1 = chartControl.GetXByBarIndex(ChartBars, idx - barsadjust);
						
						StartPoint = (new Point(x1, y)).ToVector2();
						
						float boxsize = pWidth5;
						ThisEllipse = new SharpDX.Direct2D1.Ellipse(StartPoint, boxsize, boxsize);
									
						
						
						RenderTarget.AntialiasMode							= SharpDX.Direct2D1.AntialiasMode.PerPrimitive;
		                RenderTarget.FillEllipse(ThisEllipse, cachedPlotDownBrush33DX);	
						RenderTarget.AntialiasMode = oldAntialiasMode;
						continue;
						
					}
									
					
					
					
					
					bool c1 = idx == lastBarPainted;
					
					
					if (Calculate == Calculate.OnBarClose)
						c1 = idx == lastBarPainted - 1;
					
					
					bool c2 = preval != val && preval != 0;
					c2 = preval != val;
					
					//c2 = preval != val && val != 0;
					
					if (c1)
						//endX = endX + myProperties.BarMarginRight;
						endX = endX + ChartPanel.W + 0;
						
						
					// Draw pivot lines
					startPoint	= new Point(startX, y);
					endPoint		= new Point(endX, y);

					//RenderTarget.DrawLine(startPoint.ToVector2(), endPoint.ToVector2(), plot.BrushDX , plot.Width, plot.StrokeStyle);
					SharpDX.RectangleF			rect2			= new SharpDX.RectangleF((float)startX,(float)y-pShadowWidth-1,(float)endX-(float)startX,pShadowWidth*2+1);
					//RenderTarget.FillRectangle(rect2, smallAreaBrushDx);	
					
					// line moved so draw labels

					if (c1 || c2)
					{
						
						
						
						startPoint	= new Point(endX, 0);
						endPoint	= new Point(endX, 2000);
						//RenderTarget.DrawLine(startPoint.ToVector2(), endPoint.ToVector2(), lineBrushDx , 1);					
						
						
						
						startPoint	= new Point(chartControl.GetXByBarIndex(ChartBars, idx), y);
						endPoint		= new Point(endX, y);
						
						
								
						//if (val == 0)
						
						if (startbar != idx)
						if (pLabelsEnabled2)
						if (val != 0)
						{
						
							string pp = plot.Name;
													
//							if (seriesCount == 0)
//								pp = "U4";
//							if (seriesCount == 1)
//								pp = "U3";
//							if (seriesCount == 2)
//								pp = "U2";
//							if (seriesCount == 3)
//								pp = "U1";						
//							if (seriesCount == 4)
//								pp = "BP";	
//							if (seriesCount == 5)
//								pp = "L1";
//							if (seriesCount == 6)
//								pp = "L2";
//							if (seriesCount == 7)
//								pp = "L3";
//							if (seriesCount == 8)
//								pp = "L4";	
							
							
							TextLayout textLayout = new TextLayout(Globals.DirectWriteFactory, pp, textFormat, 1000, textFormat.FontSize);

							double newy = y-textLayout.Metrics.Height-4;
							
							// text is on a previous line
							endPoint2 = new Point(startPoint.X - textLayout.Metrics.Width - 4 - pRightPX, newy);
							
							float edgeextra = 0;
							if (pActiveOutlineEnabled)
							{
								//yB2 = yB2 - pOrderBothOutlineStroke.Width;
								edgeextra = pOrderBothOutlineStroke.Width;
								
							}
					
							
							if (c1) // text is on right edge
								endPoint2 = new Point(ChartPanel.W - textLayout.Metrics.Width - 4 - pRightPX - edgeextra, newy); 
							
							
							//if (c1) // only show text on right edge, delete to show text on all lines
							//RenderTarget.DrawTextLayout(endPoint2.ToVector2(), textLayout, plot.BrushDX);
							
							RenderTarget.DrawTextLayout(endPoint2.ToVector2(), textLayout, smallAreaBrushDx);
							

							textLayout.Dispose();						
						}
						
				
						//if (val != 0)
						if (preval != 0)
						if (!FirstOne)
						{
							

							
							nextPoint.Y = prey;
							endPoint.Y = prey;
						

							//if (val != 0)
								if (seriesCount == 3)
								{
									RenderTarget.DrawLine(nextPoint.ToVector2(), endPoint.ToVector2(), cachedStopLineBrushDX, pStopStroke.Width, pStopStroke.StrokeStyle);

								}
								else
								{
									RenderTarget.DrawLine(nextPoint.ToVector2(), endPoint.ToVector2(), cachedTargetLineBrushDX, pTargetStroke.Width, pTargetStroke.StrokeStyle);

								}		
						
				
							
							//RenderTarget.DrawLine(nextPoint.ToVector2(), endPoint.ToVector2(),  plot.BrushDX , plot.Width, plot.StrokeStyle); 		
											
							
							
							
								float xxxwid = (float) endPoint.X - (float) nextPoint.X;
								
								rect2			= new SharpDX.RectangleF((float)nextPoint.X,(float)endPoint.Y-pShadowWidth-1,xxxwid,pShadowWidth*2+1);
					
								
								
								RenderTarget.FillRectangle(rect2, smallAreaBrushDx);	
						}
						
						
							
						
						
						
						nextPoint = endPoint;
						
						FirstOne = false;
						
					}
					
					
					prey = y;

					
					
				}



				// Draw pivot text
				
				

			}
				
			
			// smallAreaBrushDx is now a cached reference — don't dispose
			
				
							
			
			// START ARROWS
			
			SharpDX.Direct2D1.AntialiasMode oldAntialiasMode222 = RenderTarget.AntialiasMode;

			if (!IsInHitTest)
			if (pArrowsEnabled)
			{
			
				SharpDX.Direct2D1.Brush longBrushDX33 = cachedArrowUpBrushDX;
				SharpDX.Direct2D1.Brush shortBrushDX = cachedArrowDownBrushDX;
				SharpDX.Direct2D1.Brush arrowBrushDX = cachedArrowUpBrushDX;

				Stroke ThisStroke = pArrowDownStroke;

	            int FB2 = ChartBars.FromIndex;
	            int LB2 = ChartBars.ToIndex;
	            int BB2 = 0;
	            int xt3 = 0;
	            int yt2 = 0;
	            double yt223 = 0;

	            LB2 = Math.Min(CurrentBars[0], LB2);
	            BB2 = CurrentBars[0] - LB2;

				// ARROWS

				TextFormat	LabelText3Format			= cachedTextFontFormat;
				LabelText3Format.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
				LabelText3Format.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
				LabelText3Format.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;

				TextLayout LabelText3Layout = new TextLayout(NinjaTrader.Core.Globals.DirectWriteFactory, "", LabelText3Format, 1000, LabelText3Format.FontSize);
			
            	SharpDX.Direct2D1.Brush LabeLB2rushDX = cachedChartTextBrushDX;
				Point Text3Point		= new Point(0, 0);

				
				ChartPanel chartPanel	= chartControl.ChartPanels[chartScale.PanelIndex];
				SharpDX.Direct2D1.PathGeometry arrowGeo;
				
				if (ChartBars != null)
				for (int i = FB2; i <= LB2; i++)
				{
									
					BB2 = CurrentBars[0] - i;				
					BB2 = i;
					
					double ThisSignal = Signals.GetValueAt(BB2);
					double ThisBarCount = Math.Abs(ThisTrend1.GetValueAt(Math.Max(0,BB2-1)));
					

					if (pDoReverse)
					ThisSignal = ThisSignal * -1;
					
					if (ThisSignal != 0)
					{
							
						xt3 = chartControl.GetXByBarIndex(ChartBars, i);
						
						int pTextOffset = 0;
						string LB22 = string.Empty;
						float newy = 0;
						float newx = 0;
						float totalarrowheight = pArrowOffset + pArrowSize + pArrowBarHeight;
							
	
						
						if (ThisSignal == 1)
						{

							yt2 = chartScale.GetYByValue(Lows[0].GetValueAt(BB2));
							yt223 = chartScale.GetYByValueWpf(Lows[0].GetValueAt(BB2));
							arrowBrushDX = longBrushDX33;
							ThisStroke = pArrowUpStroke;	
							
						}
						
						if (ThisSignal == -1)
						{			
							yt2 = chartScale.GetYByValue(Highs[0].GetValueAt(BB2));
							yt223 = chartScale.GetYByValueWpf(Highs[0].GetValueAt(BB2));
							arrowBrushDX = shortBrushDX;
							ThisStroke = pArrowDownStroke;	
							
						}	
						
						arrowGeo = CreateArrowGeometry(chartControl, chartPanel, chartScale, xt3, yt2, (int)ThisSignal);

						RenderTarget.AntialiasMode = SharpDX.Direct2D1.AntialiasMode.PerPrimitive;

						RenderTarget.FillGeometry(arrowGeo, arrowBrushDX);
						RenderTarget.DrawGeometry(arrowGeo, ThisStroke.BrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
						arrowGeo.Dispose();
						
							
						
														
						LabelText3Layout.Dispose();
						LabelText3Layout = new TextLayout(NinjaTrader.Core.Globals.DirectWriteFactory, LB22, LabelText3Format, 1000, 1000);

						float boxpadding = LabelText3Format.FontSize;
						
           
						float RectWidth = LabelText3Layout.Metrics.Width + (float) pTextFont.Size;
						float RectHeight = LabelText3Layout.Metrics.Height  + (float) pTextFont.Size / 2f + 1;
						
						if (ThisSignal == 1)
						{
							LB22 = pLabelBuy + " " + ThisBarCount.ToString();
							newy = yt2 + totalarrowheight + 1 + pTextOffset;
							
						}
						
						if (ThisSignal == -1)
						{
							LB22 = pLabelSell + " " + ThisBarCount.ToString();
							newy = yt2 - totalarrowheight - RectHeight - 1 - pTextOffset;

						}	
								
						
//						LB22 = LB22 + " " + BarCount.GetValueAt(BB2).ToString().Replace("-", "");
						
						newx = xt3 - RectWidth/2 - 2;								
													
						Text3Point = new Point(newx, newy);
					
						
						
						
						SharpDX.RectangleF Text3Rect = new SharpDX.RectangleF(newx, newy, RectWidth, RectHeight);

//								{
						
//									RenderTarget.AntialiasMode							= SharpDX.Direct2D1.AntialiasMode.Aliased;
//									RenderTarget.DrawRectangle(rect2, pBrush08.BrushDX, pBrush08.Width, pBrush08.StrokeStyle);
//									RenderTarget.AntialiasMode							= SharpDX.Direct2D1.AntialiasMode.PerPrimitive;
							
//								}
						
						
						LabeLB2rushDX = cachedChartTextBrushDX;
						if (pLabelsEnabled)
							RenderTarget.DrawText(LB22, LabelText3Format, Text3Rect, LabeLB2rushDX);
						
						RenderTarget.AntialiasMode							= SharpDX.Direct2D1.AntialiasMode.Aliased;
						
						
		
		
//						if (pExitOEnabled)
//						{
								
//							string ThisText = string.Empty;	
//							double ThisLine = TargetATRPrice.GetValueAt(BB2);
							
//							x1 = chartControl.GetXByBarIndex(ChartBars,i);
//							x2 = chartControl.GetXByBarIndex(ChartBars,i)+100;
							
								
//							ThisLine = TargetATRPrice.GetValueAt(BB2);	
								
//							y1 = chartScale.GetYByValue(ThisLine);
			
//							Point StartPoint2	= new Point(x1,y1);
//							Point EndPoint2 = new Point(x2,y1);

//							ThisStroke = pTargetStroke;
//							RenderTarget.DrawLine(StartPoint2.ToVector2(), EndPoint2.ToVector2(), ThisStroke.BrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
								
								
//							ThisText = "Target 1";							
//							LabelText3Layout = new TextLayout(NinjaTrader.Core.Globals.DirectWriteFactory, ThisText, LabelText3Format, 1000, 1000);
//							LabeLB2rushDX = ThisStroke.BrushDX;
//							LabeLB2rushDX.Opacity = 0.5f;
							
//							boxpadding = LabelText3Format.FontSize;
//							RectWidth = LabelText3Layout.Metrics.Width + (float) pTextFont.Size;
//							RectHeight = LabelText3Layout.Metrics.Height  + (float) pTextFont.Size / 2f + 1;
					
//							newy = (float) y1 - RectHeight/2;
//							newx = (float) x1 - RectWidth - 1;								

//							Text3Rect = new SharpDX.RectangleF(newx, newy, RectWidth, RectHeight);
							
//							RenderTarget.AntialiasMode							= SharpDX.Direct2D1.AntialiasMode.Aliased;
//							RenderTarget.DrawText(ThisText, LabelText3Format, Text3Rect, LabeLB2rushDX);
//						//	RenderTarget.AntialiasMode							= SharpDX.Direct2D1.AntialiasMode.PerPrimitive;
						
							
							
								
//							ThisLine = TargetATRPrice2.GetValueAt(BB2);	
								
//							y1 = chartScale.GetYByValue(ThisLine);
			
//							StartPoint2	= new Point(x1,y1);
//							EndPoint2 = new Point(x2,y1);

//							ThisStroke = pTargetStroke;
//							RenderTarget.DrawLine(StartPoint2.ToVector2(), EndPoint2.ToVector2(), ThisStroke.BrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
								
//							ThisText = "Target 2";							
//							LabelText3Layout = new TextLayout(NinjaTrader.Core.Globals.DirectWriteFactory, ThisText, LabelText3Format, 1000, 1000);
//							LabeLB2rushDX = ThisStroke.BrushDX;
//							LabeLB2rushDX.Opacity = 0.5f;
							
//							boxpadding = LabelText3Format.FontSize;
//							RectWidth = LabelText3Layout.Metrics.Width + (float) pTextFont.Size;
//							RectHeight = LabelText3Layout.Metrics.Height  + (float) pTextFont.Size / 2f + 1;
					
//							newy = (float) y1 - RectHeight/2;
//							newx = (float) x1 - RectWidth - 1;								

//							Text3Rect = new SharpDX.RectangleF(newx, newy, RectWidth, RectHeight);
//							RenderTarget.DrawText(ThisText, LabelText3Format, Text3Rect, LabeLB2rushDX);													
								
								
						
//							ThisLine = StopATRPrice.GetValueAt(BB2);	
							
								
//							y1 = chartScale.GetYByValue(ThisLine);
			
//							StartPoint2	= new Point(x1,y1);
//							EndPoint2 = new Point(x2,y1);
							
						
						
//							ThisStroke = pStopStroke;
//							RenderTarget.DrawLine(StartPoint2.ToVector2(), EndPoint2.ToVector2(), ThisStroke.BrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
							
//							ThisText = "Stop Loss";							
//							LabelText3Layout = new TextLayout(NinjaTrader.Core.Globals.DirectWriteFactory, ThisText, LabelText3Format, 1000, 1000);
//							LabeLB2rushDX = ThisStroke.BrushDX;
//							LabeLB2rushDX.Opacity = 0.5f;
							
//							boxpadding = LabelText3Format.FontSize;
//							RectWidth = LabelText3Layout.Metrics.Width + (float) pTextFont.Size;
//							RectHeight = LabelText3Layout.Metrics.Height  + (float) pTextFont.Size / 2f + 1;
					
//							newy = (float) y1 - RectHeight/2;
//							newx = (float) x1 - RectWidth - 1;								

//							Text3Rect = new SharpDX.RectangleF(newx, newy, RectWidth, RectHeight);
//							RenderTarget.DrawText(ThisText, LabelText3Format, Text3Rect, LabeLB2rushDX);							
							
							
								
//						}
					
					
					}
				}
		
				// Brushes and LabelText3Format are now cached — only dispose the TextLayout
				LabelText3Layout.Dispose();
				
			}
				
			

			
			RenderTarget.AntialiasMode = oldAntialiasMode222;
			
			// END ARROWS
			
			
			
			
			
				ChartBackgroundFadeBrushDX = cachedChartBackgroundFadeBrushDX;			
			
			
			thistop2 = ChartPanel.H;
			
			
			
			
			
			if (!IsInHitTest)
			if (pCuEnabled)
			if (pHUDEnabled)				
			// HEADS UP DISPLAY
				try
			{
		
				
				// XX Bar Values
				
				
			barWidth = (int) chartControl.BarWidth;
			barDistance = (int) ChartControl.Properties.BarDistance;
			//barBetween = barDistance - barWidth - barWidth - 1;
				
			double pxL2 = 0;
			double pxL1 = 0;
			
			int StartLoopBar = Math.Max(0, FB - 5);
			int EndLoopBar = LB + 5; //Math.Min(CurrentBars[0], LB + 5);
			
			
			
			// adjusted to fix proper display of swing levels
			
			StartLoopBar = 0;
			EndLoopBar = LB + 20;
			
			
			
			double storep = 0;	
			
			for (int i = StartLoopBar; i <= EndLoopBar; i++)
			{
				
				
				x1 = chartControl.GetXByBarIndex(ChartBars,i);
				
				// gap
				
				xL = x1 - barWidth - 2;
				//xL2 = x1;
				xR = x1 + barWidth + 2;
				xW = barWidth + barWidth + 3;
				xW2 = barWidth;
				
					storep = i;

					PriceBox PP;
					if (PriceX1Boxes.TryGetValue(storep, out PP))
					{
						PP.Top = xL;
						PP.Bottom = xR;
						PP.Height = xW;
					}
					else
					{
						PP = new PriceBox();
						PP.Top = xL;
						PP.Bottom = xR;
						PP.Height = xW;
						PriceX1Boxes.TryAdd(storep, PP);
					}
				
					
				// no gap
				
				double spacebetweenpixels = 0;
					
				double xLa = x1 - barDistance/2 - 1;
				double xL2a = x1;
				double xRa = x1 + barDistance + 1;
				double xWa = barDistance + 1;
				double xW2a = barWidth;
					
				
					xRa = xLa - spacebetweenpixels;
					xWa = xRa - pxL2;
					
				//Print(xLa + "  " + xRa);		
					
				storep = storep - 1;

				if (PriceX2Boxes.TryGetValue(storep, out PP))
				{
					PP.Top = pxL2;
					PP.Bottom = xRa;
					PP.Height = xWa;
				}
				else
				{
					PP = new PriceBox();
					PP.Top = pxL2;
					PP.Bottom = xRa;
					PP.Height = xWa;
				}
					
				if (pxL2 != 0)
				if (!PriceX2Boxes.ContainsKey(storep))
					PriceX2Boxes.TryAdd(storep, PP);
				
				pxL2 = xLa;
				
				

				

			}
		
				

				
				
				
			HUDNumber = 0;
			
			MinRightMarginHUD = 0;
			
//			if (pCuEnabled)
//			if (pHUDEnabled)
			{
				

				HUDVOLColorDX = pHUDNEColor.ToDxBrush(RenderTarget);
				HUDNEColorDX = pHUDNEColor.ToDxBrush(RenderTarget);
				HUDUPColorDX = pHUDUPColor.ToDxBrush(RenderTarget);
				HUDDNColorDX = pHUDDNColor.ToDxBrush(RenderTarget);
				

//				HUDVOLColorDX.Opacity = pHUDMaxOpacity/100f;
//				HUDNEColorDX.Opacity = pHUDMaxOpacity/100f;
//				HUDUPColorDX.Opacity = pHUDMaxOpacity/100f;
//				HUDDNColorDX.Opacity = pHUDMaxOpacity/100f;
			
				if (pHUD1) DrawHUD(ShortLine, chartControl, chartScale, "Total Delta");
				if (pHUD2) DrawHUD(LongLine, chartControl, chartScale, "Bar Delta");
//				if (pHUD3) DrawHUD(TotalDelta, chartControl, chartScale, "Total Delta");
//				if (pHUD4) DrawHUD(BarDelta, chartControl, chartScale, "Bar Delta");
//				if (pHUD5)
//				{
//					DrawHUD(BarAsk, chartControl, chartScale, "Bar Ask");
//					DrawHUD(BarBid, chartControl, chartScale, "Bar Bid");					
//				}
				
				
	
				HUDVOLColorDX.Dispose();
				HUDNEColorDX.Dispose();
				HUDUPColorDX.Dispose();
				HUDDNColorDX.Dispose();
				
				
			}
			
			
			}
			
			
			catch (Exception ex)
			{
				//if (TestRender) Print("OnRender DrawHUD: " + ThisMasterInstrument.Name + " " + ex.Message + " " + DateTime.Now.ToString("T"));
				
			}				
			
				
			
			
	// TIMER!!!!!!!!!!
				if (!IsInHitTest)
			if (pTimerEnabled)
				//if (pTimerEnabled && i == LB && LB >= CurrentBars[0])
				{
				
					CellFormat			= cachedTextFontTimeFormat;
                    CellFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
                    CellFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
                    CellFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;
					
					
					CellString = BarTimerString;

					if (CellLayout != null) CellLayout.Dispose();
					CellLayout = new TextLayout(Core.Globals.DirectWriteFactory, CellString, CellFormat, 10000, 10000);

					
					
					//double yB2 = GetBoxYPixel(RTTS(Lows[0].GetValueAt(i)),"Bottom") + pTimerOffset;
					xW = CellLayout.Metrics.Width;	
					xL = ChartPanel.W +ChartPanel.X - xW - 6 - pTimerOffsetX;
					
					double yB2 = thistop2 - CellLayout.Metrics.Height - 4 - pTimerOffset;
					
					double yT2 = CellLayout.Metrics.Height;
					
					if (pActiveOutlineEnabled)
					{
						yB2 = yB2 - pOrderBothOutlineStroke.Width;
						xL = xL - pOrderBothOutlineStroke.Width;
						
					}
					
					
					ThisRect = new SharpDX.RectangleF((float)xL, (float)yB2, (float)xW, (float)yT2);
				
				
					//Print(BarTimerString);
				
					ThisBrushDX = cachedTimerBrushDX;
					RenderTarget.DrawText(BarTimerString, CellFormat, ThisRect, ThisBrushDX);
				
				}			
			
			
		

				// table
				
				
				if (pSecondaryFeedsDisplayEnabled)
				if (pSecondaryFeedsEnabled)
				if (!IsInHitTest)
				{
					// 1.2 - SharpDX Brush Resources

					// RenderTarget commands must use a special brush resource defined in the SharpDX.Direct2D1 namespace
					// These resources exist just like you will find in the WPF/Windows.System.Media namespace
					// such as SolidColorBrushes, LienarGraidentBrushes, RadialGradientBrushes, etc.
					// To begin, we will start with the most basic "Brush" type
					// Warning:  Brush objects must be disposed of after they have been used
					
					SharpDX.Direct2D1.Brush areaBrushDx;
					//SharpDX.Direct2D1.Brush smallAreaBrushDx;
					SharpDX.Direct2D1.Brush textBrushDx;
					SharpDX.Direct2D1.Brush fillBrushDx;
					
					// for convenience, you can simply convert a WPF Brush to a DXBrush using the ToDxBrush() extension method provided by NinjaTrader
					// This is a common approach if you have a Brush property created e.g., on the UI you wish to use in custom rendering routines
					
					areaBrushDx = cachedAreaBrushDX;
					textBrushDx = cachedTableTextBrushDX;
					fillBrushDx = cachedTableTextBrushDX;
					
					
					
					// 1.6 - Simple Text Rendering

					// For rendering custom text to the Chart, there are a few ways you can approach depending on your requirements
					// The most straight forward way is to "borrow" the existing chartControl font provided as a "SimpleFont" class
					// Using the chartControl LabelFont, your custom object will also change to the user defined properties allowing 
					// your object to match different fonts if defined by user.  

					// The code below will use the chartControl Properties Label Font if it exists,
					// or fall back to a default property if it cannot obtain that value
					NinjaTrader.Gui.Tools.SimpleFont wpfFont = chartControl.Properties.LabelFont ??  new NinjaTrader.Gui.Tools.SimpleFont("Arial", 12);

					// the advantage of using a SimpleFont is they are not only very easy to describe 
					// but there is also a convenience method which can be used to convert the SimpleFont to a SharpDX.DirectWrite.TextFormat used to render to the chart
					// Warning:  TextFormat objects must be disposed of after they have been used
					SharpDX.DirectWrite.TextFormat textFormat1 = cachedTableTextFormat;	
						
					// Once you have the format of the font, you need to describe how the font needs to be laid out
					// Here we will create a new Vector2() which draws the font according to the to top left corner of the chart (offset by a few pixels)
					SharpDX.Vector2 upperTextPoint = new SharpDX.Vector2(ChartPanel.X + 10, ChartPanel.Y + 20);
					// Warning:  TextLayout objects must be disposed of after they have been used
					SharpDX.DirectWrite.TextLayout textLayout1 =
						new SharpDX.DirectWrite.TextLayout(NinjaTrader.Core.Globals.DirectWriteFactory,
							NinjaTrader.Custom.Resource.SampleCustomPlotUpperLeftCorner, textFormat1, ChartPanel.X + ChartPanel.W,
							textFormat1.FontSize);

					// With the format and layout of the text completed, we can now render the font to the chart

					

				
					FB = ChartBars.FromIndex;
		            LB = ChartBars.ToIndex;
		          
								
					AllStrings.Clear();
					List<string> TempRow = new List<string>();
					AllStrings.Add(new List<string>(TempRow));

					bool B1 = true;
					
					string UP = "Up";
					string DN = "Down";
					string NU = "Flat";
					
					string S1 = "Trend Is ";
					string S2 = "PC Is ";
					string S3 = "STO Is ";
					string S4 = "MOM Is ";
					string S5 = "DAD Is ";
					
					int N1 = 0;
					int N2 = 0;
					int N3 = 0;
					int N4 = 0;
					int N5 = 0;
					
					if (pFeed4Enabled) N1 = Direction4.GetValueAt(LB);
					if (pFeed3Enabled) N2 = Direction3.GetValueAt(LB);
					if (pFeed2Enabled) N3 = Direction2.GetValueAt(LB);
					if (pFeed1Enabled) N4 = Direction1.GetValueAt(LB);
					
//					N5 = Direction5.GetValueAt(LB);
					
//					if (pFlashEnabled)
//					{
						
//						if (R1Left.TotalSeconds <= pSecondsA1 && IsOdd((int) Math.Round(R1Left.TotalSeconds,0)))
//							N1 = 5;
//						if (R2Left.TotalSeconds <= pSecondsA2 && IsOdd((int) Math.Round(R2Left.TotalSeconds,0)))
//							N2 = 5;	
//						if (R3Left.TotalSeconds <= pSecondsA3 && IsOdd((int) Math.Round(R3Left.TotalSeconds,0)))
//							N3 = 5;
//						if (R4Left.TotalSeconds <= pSecondsA4 && IsOdd((int) Math.Round(R4Left.TotalSeconds,0)))
//							N4 = 5;
						
//					}
					
					
					S1 = SFeed4;
					S2 = SFeed3;
					S3 = SFeed2;
					S4 = SFeed1;
										
					
//					string T1 = pEMAPeriod4.ToString();
//					string T2 = pEMAPeriod3.ToString();
//					string T3 = pEMAPeriod2.ToString();
//					string T4 = pEMAPeriod1.ToString();	
					
					
//					S1 = T1 + " " + AcceptableBasePeriodType4.ToString();
//					S2 = T2 + " " + AcceptableBasePeriodType3.ToString();
//					S3 = T3 + " " + AcceptableBasePeriodType2.ToString();	
//					S4 = T4 + " " + AcceptableBasePeriodType1.ToString();
					
					
//					S3 = T3 + "M"  + pCB3S + TL(R3Left) + pCB3S + VOL3.ToString();
//					S4 = T4 + "M"  + pCB3S + TL(R4Left) + pCB3S + VOL4.ToString();	
					
					
//					if (N1 == 1)
//						S1 = S1 + UP;
//					if (N2 == 1)
//						S2 = S2 + UP;
//					if (N3 == 1)
//						S3 = S3 + UP;
//					if (N4 == 1)
//						S4 = S4 + UP;
//					if (N5 == 1)
//						S5 = S5 + UP;
					
//					if (N1 == -1)
//						S1 = S1 + DN;
//					if (N2 == -1)
//						S2 = S2 + DN;
//					if (N3 == -1)
//						S3 = S3 + DN;
//					if (N4 == -1)
//						S4 = S4 + DN;
//					if (N5 == -1)
//						S5 = S5 + DN;
					
//					if (N1 == 0)
//						S1 = S1 + NU;
//					if (N2 == 0)
//						S2 = S2 + NU;
//					if (N3 == 0)
//						S3 = S3 + NU;
//					if (N4 == 0)
//						S4 = S4 + NU;
//					if (N5 == 0)
//						S5 = S5 + NU;					
					
					//if (pShowPreviousRange)
					{
						
					
						if (pFeed4Enabled) 
						{
					
							TempRow.Clear();	
							TempRow.Add("0");
	//						TempRow.Add(S1 + Environment.NewLine + "ASDSA");
							TempRow.Add(S1);		
							AllStrings.Add(new List<string>(TempRow));					
							
						}
						if (pFeed3Enabled) 
						{
							
							TempRow.Clear();	
							TempRow.Add("0");
							TempRow.Add(S2);	
							AllStrings.Add(new List<string>(TempRow));	
						}
						if (pFeed2Enabled) 
						{
							
							TempRow.Clear();	
							TempRow.Add("0");
							TempRow.Add(S3);
							AllStrings.Add(new List<string>(TempRow));	
						}
						if (pFeed1Enabled) 
						{
							TempRow.Clear();	
							TempRow.Add("0");
							TempRow.Add(S4);	
							AllStrings.Add(new List<string>(TempRow));		
						}
						
						
						
//						TempRow.Clear();	
//						TempRow.Add("0");
//						TempRow.Add(S5);
//						AllStrings.Add(new List<string>(TempRow));		
				
						
						//TempRow.Clear();	
						//TempRow.Add("0");
						//if (pShowUpperWick) TempRow.Add(UpperWickD.GetValueAt(LB-1).ToString(FS));
						//if (pShowBarRange) TempRow.Add(BarRangeD.GetValueAt(LB-1).ToString(FS));
						//if (pShowLowerWick) TempRow.Add(LowerWickD.GetValueAt(LB-1).ToString(FS));
						//AllStrings.Add(new List<string>(TempRow));						
					}

					
					
					
					AllColors.Clear();
					List<int> TempRow2 = new List<int>();
					AllColors.Add(new List<int>(TempRow2));

					//if (pShowPreviousRange)
					{
					
						if (pFeed4Enabled) 
						{
					
									TempRow2.Clear();	
						TempRow2.Add(0);
						TempRow2.Add(N1);						
						AllColors.Add(new List<int>(TempRow2));	
						
						}
						if (pFeed3Enabled) 
						{
							
						TempRow2.Clear();	
						TempRow2.Add(0);
						TempRow2.Add(N2);						
						AllColors.Add(new List<int>(TempRow2));	
						
						}
						if (pFeed2Enabled) 
						{

						TempRow2.Clear();	
						TempRow2.Add(0);
						TempRow2.Add(N3);						
						AllColors.Add(new List<int>(TempRow2));		
						}
						if (pFeed1Enabled) 
						{
						
						TempRow2.Clear();	
						TempRow2.Add(0);
						TempRow2.Add(N4);						
						AllColors.Add(new List<int>(TempRow2));			
						}
						
						
				
					
					
						
//						TempRow2.Clear();	
//						TempRow2.Add(0);
//						TempRow2.Add(N5);						
//						AllColors.Add(new List<int>(TempRow2));	
						
				
						//TempRow2.Clear();	
						//TempRow2.Add(0);
						//if (pShowUpperWick) TempRow2.Add(Direction.GetValueAt(LB-1));
						//if (pShowBarRange) TempRow2.Add(Direction.GetValueAt(LB-1));
						//if (pShowLowerWick) TempRow2.Add(Direction.GetValueAt(LB-1));
						//AllColors.Add(new List<int>(TempRow2));						
					}

				
	  
					
	        
					
					// 1.7 - Advanced Text Rendering

					// Font formatting and text layouts can get as complex as you need them to be
					// This example shows how to use a complete custom font unrelated to the existing user-defined chart control settings
					// Warning:  TextLayout and TextFormat objects must be disposed of after they have been used
//					SharpDX.DirectWrite.TextFormat textFormat2 =
//						new SharpDX.DirectWrite.TextFormat(NinjaTrader.Core.Globals.DirectWriteFactory, "Century Gothic", FontWeight.Bold,
//							FontStyle.Italic, 32f);
						
					SharpDX.DirectWrite.TextFormat textFormat2			= cachedTableTextFormat;
		            textFormat2.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
		            textFormat2.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
		            textFormat2.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;
					
					
					
					string TXT11 = "asd";
					string TXT12 = "as1";
					string TXT13 = "as2";
					string TXT21 = "as2";
					string TXT22 = "as3";
					string TXT23 = "a4d";
					
					  text = "PU";
					//text.count
					
					textLayout2 =
						new SharpDX.DirectWrite.TextLayout(NinjaTrader.Core.Globals.DirectWriteFactory, text, textFormat2, 100, textFormat2.FontSize);

					//textLayout2.TextAlignment = SharpDX.DirectWrite.TextAlignment.Leading;	
					
		          
						int YCellPadding = 6;
						int XCellPadding = 12;
						int pSpaceBtwRects = pMarginB + 1;
						int TableMarginX = pPixelsFromRight + ChartPanel.X;
					
							if (pActiveOutlineEnabled)
							{
								//yB2 = yB2 - pOrderBothOutlineStroke.Width;
								TableMarginX = TableMarginX + (int) pOrderBothOutlineStroke.Width;
								
							}
					
					
						int TableMarginY = pPixelsFromBottom;
					
						int RectHeight = (int) textLayout2.Metrics.Height + YCellPadding;
						int RectWidth = (int) textLayout2.Metrics.Width + XCellPadding + 2;

						int RectX = ChartPanel.W  - TableMarginX;
						int RectY = ChartPanel.H  - TableMarginY;
						
					// at bottom
						int StartY = RectY + 2;	// two pixel ninja restriction at bottom of chart
					
						// top 
						//if (pTPosition == LVVolume_TablePosition.TopLeft || pTPosition == LVVolume_TablePosition.TopRight)
						StartY = TableMarginY + RectHeight - 1 + 2;
					
						bool ShowRects = true;
						
						int NumOfColumns = 0;
						int NumOfRows = 0; 
				
//						if (pShowCurrentRange)
//							NumOfColumns = NumOfColumns + 2;
//						if (pShowPreviousRange)
//							NumOfColumns = NumOfColumns + 2;
//						if (pShowUpperWick)
//							NumOfRows = NumOfRows + 1;
//						if (pShowBarRange)
//							NumOfRows = NumOfRows + 1;
//						if (pShowLowerWick)
//							NumOfRows = NumOfRows + 1;
				
					if (pFeed1Enabled) NumOfColumns = NumOfColumns + 1;
					if (pFeed2Enabled) NumOfColumns = NumOfColumns + 1;
					if (pFeed3Enabled) NumOfColumns = NumOfColumns + 1;
					if (pFeed4Enabled) NumOfColumns = NumOfColumns + 1;
							
							
						//NumOfColumns = 4;
						NumOfRows = 1;
					
						int X11 = RectX;
						int Y11 = RectY;
					
						int TotalWidth = 0;
					
						int MaxWidth = 0;
							
						for (int i = NumOfRows; i >= 1; i--)
						{

							
							RectX = ChartPanel.W - TableMarginX;
								
							
							for (int j = NumOfColumns; j >= 1; j--)
							{
								
								int RectWidth2 = RectWidth;
//								if (j == 2 || j == 4)
//									RectWidth2 = pColumnWidthP;
								
								text = AllStrings[j][i];
								
								// insert dynamic width

								if (textLayout2 != null) textLayout2.Dispose();
								textLayout2 = new TextLayout(Globals.DirectWriteFactory, text, textFormat2, 1000, textFormat2.FontSize);

								RectWidth2 = (int) textLayout2.Metrics.Width + XCellPadding;

								MaxWidth = Math.Max(RectWidth2, MaxWidth);

								if (j != NumOfColumns)
									TotalWidth = TotalWidth + RectWidth2 + pSpaceBtwRects;
								else
								{
									TotalWidth = TotalWidth + RectWidth2;
								}

							}
							
							//RectY = RectY - RectHeight;
							
						}
						
						
						
						
						//TextFormat	textFormat2			= TextFont.ToDirectWriteTextFormat();	
			
						//TextLayout textLayout2 = new TextLayout(Globals.DirectWriteFactory, "", textFormat2, 1000, textFormat2.FontSize);
					
					
						SharpDX.RectangleF			rect2			= new SharpDX.RectangleF(RectX, RectY, RectWidth, RectHeight);

						int MaxRectWidth = MaxWidth + 30; 
						
											
 
		
		
		
						for (int i = NumOfRows; i >= 1; i--)
						{

							// right side
							RectX = ChartPanel.W - TableMarginX + 0 + 2;  // - for the 2 pixels ninja doesn't allow drawing 
							
							// left side
							//if (pTPosition == LVVolume_TablePosition.TopLeft || pTPosition == LVVolume_TablePosition.BottomLeft)
							RectX = TotalWidth + TableMarginX + 1;	
							
							RectY = StartY - RectHeight;
							RectX = TableMarginX + 1;	
							
							AllButtonZ2.Clear();
							
							for (int j = NumOfColumns; j >= 1; j--)
							{
								
								
								
								int RectWidth2 = RectWidth;
//								if (j == 2 || j == 4)
//									RectWidth2 = pColumnWidthP;
								
								text = AllStrings[j][i];
								
								// insert dynamic width

								if (textLayout2 != null) textLayout2.Dispose();
								textLayout2 = new TextLayout(Globals.DirectWriteFactory, text, textFormat2, 1000, textFormat2.FontSize);

								RectWidth2 = (int) textLayout2.Metrics.Width + XCellPadding;
								
								RectWidth2 = MaxWidth;
								
								
//								if (j != NumOfColumns)
//									RectX = RectX - RectWidth2 - pSpaceBtwRects;
//								else
									//RectX = RectX - RectWidth2;
								
								
								
								textBrushDx = cachedTableTextBrushDX;

								fillBrushDx = cachedFillNeutralBrushDX;
								if (AllColors[j][i] == 1)
									fillBrushDx = cachedFillUpBrushDX;
								if (AllColors[j][i] == -1)
									fillBrushDx = cachedFillDownBrushDX;
								
								
								//text = "as" + j.ToString() + "/" + i.ToString();
								
								
								rect2			= new SharpDX.RectangleF(RectX, RectY, MaxRectWidth, RectHeight);
								
								fillBrushDx.Opacity = (float) pFillOpacity/100;
								if (ShowRects) RenderTarget.FillRectangle(rect2, fillBrushDx);	
								
								fillBrushDx.Opacity = (float) pOutlineOpacity/100;
								if (ShowRects) RenderTarget.DrawRectangle(rect2, fillBrushDx, 1);
								
								
								AddButtonZ2(text,text,1,false,rect2);
								
								rect2			= ExpandRect(rect2, -10,-10,0,0);
								
								textFormat2.TextAlignment = SharpDX.DirectWrite.TextAlignment.Leading; // leading = left.
				         		RenderTarget.DrawText(text, textFormat2, rect2, textBrushDx);								
								
								bool IsIncluded = false;
								
								if (text == SFeed4)
									IsIncluded = pFeed4Included;
								if (text == SFeed3)
									IsIncluded = pFeed3Included;
								if (text == SFeed2)
									IsIncluded = pFeed2Included;								
								if (text == SFeed1)
									IsIncluded = pFeed1Included;
								
								text = "\u2713";
								textFormat2.TextAlignment = SharpDX.DirectWrite.TextAlignment.Trailing; // leading = left.
								
								if (IsIncluded)
								RenderTarget.DrawText(text, textFormat2, rect2, textBrushDx);	
								
								RectY = RectY + RectHeight + pSpaceBtwRects;
							}
							
							
							
						}
						
						
						
						
				
						
						
						
						
						
					// the textLayout object provides a way to measure the described font through a "Metrics" object
					// This allows you to create new vectors on the chart which are entirely dependent on the "text" that is being rendered
					// For example, we can create a rectangle that surrounds our font based off the textLayout which would dynamically change if the text used in the layout changed dynamically
					
					//int TableMargin = 5;	
						
					SharpDX.Vector2 lowerTextPoint = new SharpDX.Vector2(ChartPanel.W - textLayout2.Metrics.Width - TableMarginX,
						ChartPanel.Y + (ChartPanel.H - textLayout2.Metrics.Height - TableMarginY));
						
						
						
					SharpDX.RectangleF rect1 = new SharpDX.RectangleF(lowerTextPoint.X - XCellPadding, lowerTextPoint.Y - YCellPadding, textLayout2.Metrics.Width + XCellPadding,
						textLayout2.Metrics.Height + YCellPadding);

					// We can draw the Rectangle based on the TextLayout used above
					//RenderTarget.FillRectangle(rect1, smallAreaBrushDx);
					//RenderTarget.DrawRectangle(rect1, smallAreaBrushDx, 2);

					// And render the advanced text layout using the DrawTextLayout() method
					// Note:  When drawing the same text repeatedly, using the DrawTextLayout() method is more efficient than using the DrawText() 
					// because the text doesn't need to be formatted and the layout processed with each call
					//RenderTarget.DrawTextLayout(lowerTextPoint, textLayout2, textBrushDx, SharpDX.Direct2D1.DrawTextOptions.NoSnap);

					
					// 1.8 - Cleanup
					// This concludes all of the rendering concepts used in the sample
					// However - there are some final clean up processes we should always provided before we are done

					// If changed, do not forget to set the AntialiasMode back to the default value as described above as a best practice
					RenderTarget.AntialiasMode = oldAntialiasMode;

					// We also need to make sure to dispose of every device dependent resource on each render pass
					// Failure to dispose of these resources will eventually result in unnecessary amounts of memory being used on the chart
					// Although the effects might not be obvious as first, if you see issues related to memory increasing over time
					// Objects such as these should be inspected first
					// areaBrushDx, textBrushDx, textFormat1, textFormat2 are now cached — only dispose TextLayouts
					textLayout1.Dispose();
					textLayout2.Dispose();
				
				}
				
				// end table
			
			
			
	

	
				try
			{
			
		
	
			
		
					
			
				
				//ThisRect = new SharpDX.RectangleF(ChartPanel.X, ChartPanel.Y, ChartPanel.W, ChartPanel.H);
				//RenderTarget.DrawRectangle(ExpandRect(ThisRect,-1,-1,-1,-1), ThisStroke.BrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
				
				//pActiveDisplayEnabled = true;
				
				
				if (!IsInHitTest)
				if (pActiveDisplayEnabled)
				{
					
//					bool SomeBuy = (pEntriesEnabled && pLongEnabled && Signals.GetValueAt(CurrentBars[0]) == 1) || (pEntriesEnabled && pLongEnabled && !pShortEnabled);
//					bool SomeSell = (pEntriesEnabled && pShortEnabled && Signals.GetValueAt(CurrentBars[0]) == -1) || (pEntriesEnabled && !pLongEnabled && pShortEnabled);
					
					bool SomeBuy = (DoLong) || (pEntriesEnabled && pLongEnabled && !pShortEnabled);
					bool SomeSell = (DoShort) || (pEntriesEnabled && !pLongEnabled && pShortEnabled);
					
					if (pEntriesEnabled)
							if (pTrendOnlyEnabled)
							{
								if (CurrentTrendStatus == 1)
									SomeBuy = true;
								
								if (CurrentTrendStatus == -1)
									SomeSell = true;								
							}
							
							
					 if (SomeBuy)
					{
						ThisStroke = pOrderUpOutlineStroke;					
		
					}						
					else if (SomeSell)
					{						
						ThisStroke = pOrderDnOutlineStroke;

					}
					else if (pEntriesEnabled)
					{
						ThisStroke = pOrderBothOutlineStroke;
						
					}
					
					
					

					//if (SomeBuy || SomeSell || pEntriesEnabled)// || CancelButtonOn)
					{
						
						
		
						
						
						//CellFormat			= myProperties.LabelFont.ToDirectWriteTextFormat();	
						CellFormat			= cachedTextFont8Format;	
						
	                    CellFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
	                    CellFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
	                    CellFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.Wrap;
						
						

				
						bool pShowCD = true;
						string MSG = string.Empty;
						
							
							
						string sep2 = "   |   ";
			//			string trailing = "Daily Loss Trailing is enabled.  We are trailing the maximum profit for today by $ " + pDLTrailingAmount.ToString("N2") + ".";
						
			//				trailing = "Daily Loss Trailing by $ " + pDLTrailingAmount.ToString("N2") + " behind the maximum profit is enabled.";
						
			//			if (!pDailyPNLTrailingEnabled)
			//			{
			//				trailing = "Daily Loss Trailing is disabled.  We will adhere to the fixed Daily Loss of $ " + pDL.ToString("N2") + ".";
			//				trailing = "Daily Loss Trailing is disabled.";
			//			}
							
						text = "Daily Goal =  $ " + pDG.ToString("N2") + sep + "Daily Loss =  $ " + pDL.ToString("N2") + sep + trailing;
						
						
			
						
						
//			oldAntialiasMode	= RenderTarget.AntialiasMode;		
//			RenderTarget.AntialiasMode							= SharpDX.Direct2D1.AntialiasMode.PerPrimitive;			
			
//			BottomText = pTextFont2.ToDirectWriteTextFormat();
//			textBrushDX2 = pTimeTextColor2.ToDxBrush(RenderTarget);
			
//			if (pDGEnabled && pTextDisplayEnabled) RenderTarget.DrawText(text, BottomText, BottomRect, textBrushDX2);

//			RenderTarget.AntialiasMode = oldAntialiasMode;
			
			
				
						if (pEntriesEnabled)
						{
							
							MSG = "BOT IS READY ON " + pAccountName + " - ";	
							
							string direction = string.Empty;
							
							if (pTrendOnlyEnabled)
							{
								direction = "Trend ";
								
							}
							else if (pLongEnabled && pShortEnabled)
							{
								//MSG = MSG + "waiting for next entry - ";
								direction = "";
							}
							else if (pLongEnabled)
							{
								//MSG = MSG + "waiting for next Long entry - ";
								direction = "Long ";
							}
							else
							{
								//MSG = MSG + "waiting for next Short entry - ";
								direction = "Short ";
							}
							
							if (pAutoEnabled)
							{
								MSG = MSG + "will submit " + direction + "trades until it is manually turned off.";
								//MSG = MSG + "will submit trades until it is turned off or a daily limit is exceeded.";
							}
							else
							{
								MSG = MSG + "will submit next " + direction + " trade only and turn off immediately.";
							}
							
							if (IsInSession.GetValueAt(CurrentBars[0]) == 0)
//								MSG = MSG + "  BOT is not generating signals because of the Time Filter settings.";
								MSG = MSG + "  The Time Filter settings are restricting new signals.";
							
							if (pDGEnabled && pDGEnabledDisable)
								MSG = MSG + "  BOT will be disabled when a " + "Daily Goal of $" + pDG.ToString("N0") + " or " + "Daily Loss of $" + pDL.ToString("N0") + " has been exceeded.";

						

							
//							SomeBuy = pLongEnabled && Signals.GetValueAt(CurrentBars[0]) == 1;
//							SomeSell = pShortEnabled && Signals.GetValueAt(CurrentBars[0]) == -1;	
							
							SomeBuy = DoLong;
							SomeSell = DoShort;
							
							if (SomeBuy)
							{
								MSG = "Ready to submit Long Market order entry when this bar closes.";						
							}
							if (SomeSell)
							{
								MSG = "Ready to submit Short Market order entry when this bar closes.";							
							}
							
								
						}
	
					else if (pDGEnabled)
						{
							
							if (TotalAccountPNL >= pDG)
							{
								MSG = "Daily Goal of $" + pDG.ToString("N0") + " has been exceeded.";
								if (pDGEnabledDisable)
									MSG = MSG + "  System was automatically disabled.";
								ThisStroke = pOrderUpOutlineStroke;	
							}
							else if (TotalAccountPNL <= pDL*-1)
							{
								
								MSG = "Daily Loss of $" + pDL.ToString("N0") + " has been exceeded.";
								if (pDGEnabledDisable)
									MSG = MSG + "  System was automatically disabled.";
								ThisStroke = pOrderDnOutlineStroke;	
							}
							
						
		
							
						}
					

						// Use pre-tinted cached brush matching the active ThisStroke
						if (ThisStroke == pOrderUpOutlineStroke)
							ThisBrushDX = cachedOutlineUp03BrushDX;
						else if (ThisStroke == pOrderDnOutlineStroke)
							ThisBrushDX = cachedOutlineDn03BrushDX;
						else
							ThisBrushDX = cachedOutlineBoth03BrushDX;
						
						
						
						if (MSG != string.Empty)
						{

							float sw = 0;
							sw = ThisStroke.Width+1;
							
							
							float leftrightpad = 8;
							float leftspace = (float)100;
							
							
							float totalwww = ChartPanel.W-sw-sw-leftspace-1-leftrightpad-leftrightpad;
			
							//CellString = "-- " + MSG + " --";
							
							CellString = MSG;
							if (CellLayout != null) CellLayout.Dispose();
							CellLayout = new TextLayout(Core.Globals.DirectWriteFactory, CellString, CellFormat, totalwww, 10000);
							
							float FinalH = CellLayout.Metrics.Height;
		                    float FinalW = CellLayout.Metrics.Width;
							
							float hh = FinalH + 4;
				
							if (!pActiveOutlineEnabled)
								sw = 0;
							
							
							
							//ThisRect = new SharpDX.RectangleF(ChartPanel.X+sw, ChartPanel.H-sw-hh, ChartPanel.W-sw-sw, hh);
							ThisRect = new SharpDX.RectangleF(ChartPanel.X+sw+leftspace+leftrightpad, 0+sw+1, totalwww, hh);
							
							ThisRect = ExpandRect(ThisRect,leftrightpad,leftrightpad,0,0);
							
							RenderTarget.FillRectangle(ThisRect,ChartBackgroundBrushDX);
							RenderTarget.FillRectangle(ExpandRect(ThisRect,2,2,2,2),ChartBackgroundFadeBrushDX);
							RenderTarget.FillRectangle(ThisRect,ThisBrushDX);	
							
							int sidetxtpad = 4;
							
							//ThisRect = new SharpDX.RectangleF(ChartPanel.X+sw+leftspace+sidetxtpad, 0+sw+1, totalwww-sidetxtpad*2, hh);
							ThisRect = new SharpDX.RectangleF(ChartPanel.X+sw+leftspace+leftrightpad, 0+sw+1, totalwww, hh);
							
							ThisBrushDX = ChartTextBrushDX;
							RenderTarget.DrawText(CellString, CellFormat, ThisRect, ThisBrushDX);	
								
						}
						
						
					
						
						if (pActiveOutlineEnabled)
						//if (SomeBuy || SomeSell)
						if (SomeBuy || SomeSell || pEntriesEnabled)	
						{
							int leftside = ChartPanel.X;
							int adj = 0;
							
							if (leftside != 0)
							{
								leftside = leftside + 1;
								adj = 1;
							}
							
							ThisRect = new SharpDX.RectangleF(leftside, 0, ChartPanel.W, ThisStroke.Width); // top
							RenderTarget.FillRectangle(ThisRect,ThisStroke.BrushDX);
							
							ThisRect = new SharpDX.RectangleF(leftside, ChartPanel.H-ThisStroke.Width-adj-0, leftside+ChartPanel.W, ThisStroke.Width); // bottom
							RenderTarget.FillRectangle(ThisRect,ThisStroke.BrushDX);
							
							ThisRect = new SharpDX.RectangleF(leftside, 0, ThisStroke.Width, ChartPanel.H); // left
							RenderTarget.FillRectangle(ThisRect,ThisStroke.BrushDX);
							
							ThisRect = new SharpDX.RectangleF(ChartPanel.W+leftside-ThisStroke.Width-adj, 0, ThisStroke.Width, ChartPanel.H); // right
							RenderTarget.FillRectangle(ThisRect,ThisStroke.BrushDX);
							
						}
						
						
					
					}
					
					
					
				}
			

				
				
			}
			 catch (Exception ex)
			{
				//if (TestRender) Print("OnRender Entry Highlight: " + ThisMasterInstrument.Name + " " + ex.Message + " " + DateTime.Now.ToString("T"));
				
			}		
			

			
			// ChartTextBrushDX and ChartBackgroundBrushDX are now cached — don't dispose per frame
			
			
			
			
			if (textLayout2 != null) textLayout2.Dispose();
            textLayout2 = new TextLayout(Core.Globals.DirectWriteFactory, tttgdf, PrintText, 10000, 10000);

            float OrderButtonSize = 15;

            float FinalH2 = textLayout2.Metrics.Height + 5;

            


            B2 = new SharpDX.RectangleF(0, space - 4, 10000, pButtonSize + 2);



            // if (MouseIn(B2, 2, 2))


			if (!IsInHitTest)
            if (pButtonsEnabled && InMenu)
                foreach (KeyValuePair<double, ButtonZ> thisbutton in AllButtonZ)
                //foreach (string xxx in AllButtons)
                {

                    string xxx = thisbutton.Value.Text;

                    string sd = xxx;



                    // szvv = graphics.MeasureString(sd, ChartControl.Font);

                    // int widdd = (int)szvv.Width + 8;


                    float widdd = 40;
                    widdd = Math.Max(pButtonSize, widdd);

                    if (thisbutton.Value.Width == 1)
                        widdd = pButtonSize;
                    else
                        widdd = thisbutton.Value.Width;


                    



                    SharpDX.DirectWrite.TextFormat ButtonText = cachedButtonTextFormat;

                    TextLayout textLayout1 = new TextLayout(Core.Globals.DirectWriteFactory, thisbutton.Value.Text, ButtonText, 10000, 10000);

                   // Print(textLayout1.Metrics.Width);


                    float FinalH = textLayout1.Metrics.Height;

                    FinalH = Math.Max(pButtonSize, FinalH);

                    float FinalW = Math.Max(FinalH,textLayout1.Metrics.Width);

                    if (thisbutton.Value.Width == 1)
                        FinalW = FinalH;
                    else
                        FinalW = textLayout1.Metrics.Width + 8;


                    CY = CY - FinalW;
                    thisbutton.Value.Rect = new SharpDX.RectangleF(CY, space, FinalW, FinalH);
                    CY = CY - space;

                    //CY = CY - widdd - space;
                   // thisbutton.Value.Rect = new SharpDX.RectangleF(CY, space, widdd, pButtonSize);
                   
                    


                    RenderTarget.FillRectangle(thisbutton.Value.Rect, buttonFOFFBrushDX);
                    if (thisbutton.Value.Switch)
                        RenderTarget.FillRectangle(thisbutton.Value.Rect, buttonFONBrushDX);

                    if (MouseIn(thisbutton.Value.Rect, 2, 2))
                    {
                        if (!thisbutton.Value.Switch)
                            RenderTarget.FillRectangle(thisbutton.Value.Rect, buttonFHBrushDX);

                        RenderTarget.DrawRectangle(thisbutton.Value.Rect, buttonHBrushDX, 3);

                    }

                    RenderTarget.DrawRectangle(thisbutton.Value.Rect, buttonBrushDX, 1);
                    RenderTarget.DrawText(thisbutton.Value.Text, ButtonText, thisbutton.Value.Rect, buttonBrushDX);

					thisbutton.Value.Rect = new SharpDX.RectangleF(CY-ChartPanel.X, space, FinalW, FinalH);

					textLayout1.Dispose();
                }





			// All render brushes are now cached in OnRenderTargetChanged — no per-frame dispose needed	
				
				
				
				
			
			
				
		}
	
		
//		private SharpDX.RectangleF ExpandRect (SharpDX.RectangleF RR, float left, float right, float top, float bottom)
//		{
			
//			SharpDX.RectangleF FF = new SharpDX.RectangleF(RR.X-left, RR.Y-top, RR.Width+left+right, RR.Height+top+bottom);
				
//			return FF;
			
//		}
		
//		private SharpDX.RectangleF ExpandRect (SharpDX.RectangleF RR, float xe, float ye)
//		{
			
//			SharpDX.RectangleF FF = new SharpDX.RectangleF(RR.X-xe, RR.Y-ye, RR.Width+xe*2, RR.Height+ye*2);
				
//			return FF;
			
//		}
		
		
		
//		public override void OnCalculateMinMax()
//		{
			
				
//		}

		
		
 		private double GetBoxXPixel(double barnum, string type, bool gap)
		{
			//price = RTTS(price);
			
			
//			if (price < PriceX1Boxes.Keys.Min())
//			{
//				return 0;
//			}
//			else if (price > PriceX1Boxes.Keys.Max())
//			{
//				return 20000;
				
//			}
			
//			else if (gap)
				
			if (gap)	
			{
			
				if (PriceX1Boxes.ContainsKey(barnum))
				{
					if (type == "Left")
					{
						return PriceX1Boxes[barnum].Top;	
					}
					else if (type == "Right")
					{
						return PriceX1Boxes[barnum].Bottom;
					}
					else if (type == "Width")
					{
						return PriceX1Boxes[barnum].Height;
					}
					
					
				}
			}
			else
			{
				if (PriceX2Boxes.ContainsKey(barnum))
				{
					if (type == "Left")
					{
						return PriceX2Boxes[barnum].Top;	
					}
					else if (type == "Right")
					{
						return PriceX2Boxes[barnum].Bottom;
					}
					else if (type == "Width")
					{
						return PriceX2Boxes[barnum].Height;
					}
					
					
				}				
				
			}
			
			return 0;
			
			
			
		}
         
		
		
		
		
		private void DrawHUD(Series<double> ThisSeries, ChartControl chartControl, ChartScale chartScale, string Label)
		{
			
			HUDNumber = HUDNumber + 1;
			
			int Start = Math.Max(FB-1,0);
			
				double xL = 0;
				//double xL2 = 0;
				double xR = 0;
				double xW = 0;
				double xW2 = 0;
				double yA = 0;
				double yB = 0;
				double yH = 0;
				double yT = 0;
			
                    SharpDX.DirectWrite.TextFormat CellFormat = cachedTextFont2Format;
                    CellFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
                    CellFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
                    CellFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;

			string CellString = string.Empty;

			TextLayout CellLayout = new TextLayout(Core.Globals.DirectWriteFactory, CellString, CellFormat,  10000, 10000);
			
//					double MaxVOL = MAX(TotalVolume,LB-Start).GetValueAt(LB);
//					double MinVOL = MIN(TotalVolume,LB-Start).GetValueAt(LB);
//					double AvgVOL = SMA(TotalVolume,LB-Start).GetValueAt(LB);
			
					double MaxVOL = 0;
					double MinVOL = 99999999999999;
					double MaxVOLBid = 0;
					double MinVOLBid = 99999999999999;			
					double MaxVOLAsk = 0;
					double MinVOLAsk = 99999999999999;	
					double MaxBidAsk = 0;
					double MinBidAsk = 0;
					double AvgVOL = 0;		
			
			for (int i = Start; i <= LB; i++)
			{
				MaxVOL = Math.Max(MaxVOL, Math.Abs(LongLine.GetValueAt(i)));
				MinVOL = Math.Min(MinVOL, Math.Abs(LongLine.GetValueAt(i)));
				
				
				
//				MaxVOLBid = Math.Max(MaxVOLBid, BarBid.GetValueAt(i));
//				MinVOLBid = Math.Min(MinVOLBid, BarBid.GetValueAt(i));
//				MaxVOLAsk = Math.Max(MaxVOLAsk, BarAsk.GetValueAt(i));
//				MinVOLAsk = Math.Min(MinVOLAsk, BarAsk.GetValueAt(i));				
			}
			
			
			
//			MaxBidAsk = Math.Max(MaxVOLBid,MaxVOLAsk);
//			MinBidAsk = Math.Max(MinVOLBid,MinVOLAsk);
			
			// Hoist per-bar invariants out of the loop
			bool isBarDelta = (Label == "Bar Delta");
			float nonBarDeltaOpacity = (float)(pHUDDefaultOpacity / 100f);
			double barDeltaOpacityLow = pHUDMinOpacity;
			double barDeltaOpacityMult = (pHUDMaxOpacity - pHUDMinOpacity) / MaxVOL;

			// Set text format alignment once
			CellFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
			CellFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
			CellFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;

			// Apply the common opacity for non-bar-delta once to both UP/DN brushes
			if (!isBarDelta)
			{
				HUDUPColorDX.Opacity = nonBarDeltaOpacity;
				HUDDNColorDX.Opacity = nonBarDeltaOpacity;
			}

			// Save/restore antialias mode once, not per bar
			oldAntialiasMode = RenderTarget.AntialiasMode;
			RenderTarget.AntialiasMode = SharpDX.Direct2D1.AntialiasMode.Aliased;

			for (int i = Start; i <= LB; i++)
			{
				// Single dict lookup replaces 3x GetBoxXPixel calls
				PriceBox pb;
				if (PriceX2Boxes.TryGetValue(i, out pb))
				{
					xL = pb.Top + 1;
					xR = pb.Bottom;
					xW = pb.Height - 1;
				}
				else
				{
					xL = 1;
					xR = 0;
					xW = -1;
				}

				double dddd = 0;
				if (ThisSeries.IsValidDataPointAt(i))
					dddd = ThisSeries.GetValueAt(i);

				double LastPrice = dddd;

				CellString = LastPrice.ToString();
//					float boxsize = pRSSize;
				
			
				
					
					
					y1 = chartScale.GetYByValue(LastPrice);

					yH = CellLayout.Metrics.Height + 4;
					yT = ChartPanel.Y + ChartPanel.H - yH*HUDNumber - 1*(HUDNumber-1);

					HUDHeight = yH;

					thistop2 = (int) yT;

					ThisRect = new SharpDX.RectangleF((float)xL, (float)yT-1, (float)xW+1, (float)yH+1);
					RenderTarget.FillRectangle(ThisRect, cachedChartBackgroundBrushDX);

					// cell fill colors
					double ThisVol = ThisSeries.GetValueAt(i);

					// Only update the opacity of the brush we'll actually use
					if (isBarDelta)
					{
						float barDeltaOpacity = (float)(Math.Round(barDeltaOpacityLow + barDeltaOpacityMult*(Math.Abs(ThisVol)-MinVOL), 0) / 100f);
						if (ThisVol >= 0)
							HUDUPColorDX.Opacity = barDeltaOpacity;
						else
							HUDDNColorDX.Opacity = barDeltaOpacity;
					}
					// (non-BarDelta opacity is set once before the loop)
					
					
//					else if (Label == "Bar Bid")
//					{
//						ThisVol = BarBid.GetValueAt(i);
//						OpacityLow2 = pHUDMinOpacity;
//						OpacityHigh2 = pHUDMaxOpacity;
//						OpacityMultiplier2 = (OpacityHigh2 - OpacityLow2) / MaxBidAsk;
//						ThisOpacity2 = Math.Round(OpacityLow2 + OpacityMultiplier2*(ThisVol-MinBidAsk),0);
						
//						HUDVOLColorDX.Opacity = (float) ThisOpacity2/100f;
//						HUDNEColorDX.Opacity = (float) ThisOpacity2/100f;
//						HUDUPColorDX.Opacity = (float) ThisOpacity2/100f;
//						HUDDNColorDX.Opacity = (float) ThisOpacity2/100f;
						
//					}
//					else if (Label == "Bar Ask")
//					{
//						ThisVol = BarAsk.GetValueAt(i);
//						OpacityLow2 = pHUDMinOpacity;
//						OpacityHigh2 = pHUDMaxOpacity;
//						OpacityMultiplier2 = (OpacityHigh2 - OpacityLow2) / MaxBidAsk;
//						ThisOpacity2 = Math.Round(OpacityLow2 + OpacityMultiplier2*(ThisVol-MinBidAsk),0);
						
//						HUDVOLColorDX.Opacity = (float) ThisOpacity2/100f;
//						HUDNEColorDX.Opacity = (float) ThisOpacity2/100f;
//						HUDUPColorDX.Opacity = (float) ThisOpacity2/100f;
//						HUDDNColorDX.Opacity = (float) ThisOpacity2/100f;						

//					}	
//					else
//					{
//						HUDVOLColorDX.Opacity = pHUDDefaultOpacity/100f;
//						HUDNEColorDX.Opacity = pHUDDefaultOpacity/100f;
//						HUDUPColorDX.Opacity = pHUDDefaultOpacity/100f;
//						HUDDNColorDX.Opacity = pHUDDefaultOpacity/100f;
						
//					}
				
				

					ThisBrushDX = (ThisVol >= 0) ? HUDUPColorDX : HUDDNColorDX;
//					else if (Label == "Total Volume")
//					{

//						ThisBrushDX = HUDVOLColorDX;
						
//					}
//					else if (Label == "Bar Delta")
//					{
						
						
						
//						if (pNetDMode2 == "Percent")
//						{
//							fd = pNDThreshold;
//							dddd = BarDeltaPercent.GetValueAt(i);
							
//							CellString = Math.Abs(Math.Round(dddd,1)).ToString() + "%";
//							CellString = Math.Abs(Math.Round(dddd,1)).ToString() + "%"; 
							
//							if (CellString.Contains("NaN"))
//								CellString = "NA";
//						}
//						else
//						{
//							fd = pNDThreshold2;
//							dddd = BarDelta.GetValueAt(i);
							
//							CellString = dddd.ToString();
//						}
							
//						if (dddd >= fd)
//						{
//							ThisBrushDX = HUDUPColorDX;
//						}
//						else if (dddd <= -1*fd)
//						{
//							ThisBrushDX = HUDDNColorDX;
//						}						
//						else
//						{
//							ThisBrushDX = HUDNEColorDX;
//						}						
						
						
//					}
//					else if (Label == "Total Delta")
//					{
						
						
							
//						if (dddd > 0)
//						{
//							ThisBrushDX = HUDUPColorDX;
//						}
//						else if (dddd < 0)
//						{
//							ThisBrushDX = HUDDNColorDX;
//						}						
//						else
//						{
//							ThisBrushDX = HUDNEColorDX;
//						}
//					}
//					else if (Label == "Bar Volume")
//					{
//						// vary by strength
//						ThisBrushDX = HUDVOLColorDX;
//					}					
//					else
//					{
//						// main color is set in brush at beginning
//						ThisBrushDX = HUDVOLColorDX;
						
//					}
							
			
				
				
				
				
						ThisRect = new SharpDX.RectangleF((float)xL, (float)yT, (float)xW, (float)yH);
						RenderTarget.FillRectangle(ThisRect, ThisBrushDX);

					if (pHUD3)
					{
						RenderTarget.AntialiasMode = SharpDX.Direct2D1.AntialiasMode.PerPrimitive;
						RenderTarget.DrawText(CellString, CellFormat, ThisRect, cachedChartTextBrushDX);
						RenderTarget.AntialiasMode = SharpDX.Direct2D1.AntialiasMode.Aliased;
					}
				
				

					
	
								
			}

			RenderTarget.AntialiasMode = oldAntialiasMode;


			ThisRect = new SharpDX.RectangleF((float)xR, (float)yT-1, (float)xW+5000, (float)yH+1);
			RenderTarget.FillRectangle(ThisRect, ChartBackgroundFadeBrushDX);
			
			ThisRect = new SharpDX.RectangleF((float)xR+5, (float)yT, (float)xW, (float)yH);

			CellFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Leading;
            CellFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
            CellFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;		
		
			CellString = Label.ToString();

			CellLayout.Dispose();
			CellLayout = new TextLayout(Core.Globals.DirectWriteFactory, CellString, CellFormat,  10000, 10000);
			MinRightMarginHUD = Math.Max (MinRightMarginHUD, CellLayout.Metrics.Width + 10);
			
			oldAntialiasMode	= RenderTarget.AntialiasMode;
			RenderTarget.AntialiasMode							= SharpDX.Direct2D1.AntialiasMode.PerPrimitive;
				

			
			ThisBrushDX = cachedChartTextBrushDX;

			RenderTarget.DrawText(CellString, CellFormat, ThisRect, ThisBrushDX);

			RenderTarget.AntialiasMode = oldAntialiasMode;

			// CellFormat is now cached — only dispose CellLayout
			CellLayout.Dispose();
			
			//ChartBackgroundFadeBrushDX.Dispose();
			
	}
		
		
		
		
		private SharpDX.Direct2D1.PathGeometry CreateArrowGeometry(ChartControl chartControl, ChartPanel chartPanel, ChartScale chartScale, int xt, double yt2, int dir)
		{
			

			Point startPoint			= new Point(0,0);
			Point midPoint				= new Point(100,0);
			Point endPoint 				= new Point(100,100);
				
			float aw = pArrowSize;
			float aw2 = pArrowBarWidth; // bar w
			float barh = pArrowBarHeight;
			float offset = pArrowOffset;

			float yt2f = (float) yt2;
			
			xt=xt-1; // adjust arrow to left 1 pixel so it lines up. bug?
			
			aw2 = Math.Min(aw2,aw);
			
			SharpDX.Vector2 tipPoint2			= new SharpDX.Vector2(0,0);
			SharpDX.Vector2 triLeftPoint2		= new SharpDX.Vector2(0,0);
			SharpDX.Vector2 triRightPoint2 		= new SharpDX.Vector2(0,0);
			SharpDX.Vector2 barLeftPoint12 		= new SharpDX.Vector2(0,0);
			SharpDX.Vector2 barRightPoint12 	= new SharpDX.Vector2(0,0);
			SharpDX.Vector2 barLeftPoint22 		= new SharpDX.Vector2(0,0);
			SharpDX.Vector2 barRightPoint22 	= new SharpDX.Vector2(0,0);

            float po = 0;

			if (dir == -1)
			{
				//yt = yt - offset;
				yt2f = yt2f - offset;

				tipPoint2 = new SharpDX.Vector2(xt,yt2f);
				triLeftPoint2 = new SharpDX.Vector2(xt-aw,yt2f-aw);
				triRightPoint2 = new SharpDX.Vector2(xt+aw,yt2f-aw);
                barLeftPoint12 = new SharpDX.Vector2(xt - aw2 + po, yt2f - aw);
                barRightPoint12 = new SharpDX.Vector2(xt + aw2 + po, yt2f - aw);
                barLeftPoint22 = new SharpDX.Vector2(xt - aw2 + po, yt2f - (aw + barh));
                barRightPoint22 = new SharpDX.Vector2(xt + aw2 + po, yt2f - (aw + barh));
			}
			else
			{
				//yt = yt + offset;
				yt2f = yt2f + offset;

				tipPoint2 = new SharpDX.Vector2(xt,yt2f);
				triLeftPoint2 = new SharpDX.Vector2(xt-aw,yt2f+aw);
				triRightPoint2 = new SharpDX.Vector2(xt+aw,yt2f+aw);
                barLeftPoint12 = new SharpDX.Vector2(xt - aw2 + po, yt2f + aw);
                barRightPoint12 = new SharpDX.Vector2(xt + aw2 + po, yt2f + aw);
                barLeftPoint22 = new SharpDX.Vector2(xt - aw2 + po, yt2f + (aw + barh));
                barRightPoint22 = new SharpDX.Vector2(xt + aw2 + po, yt2f + (aw + barh));
				
			}
			
			//Vector pixelAdjustVec		= new Vector(pixelAdjust, pixelAdjust);

			SharpDX.Direct2D1.PathGeometry pathGeometry = new SharpDX.Direct2D1.PathGeometry(Core.Globals.D2DFactory);
			SharpDX.Direct2D1.GeometrySink geometrySink = pathGeometry.Open();
			geometrySink.BeginFigure(tipPoint2, SharpDX.Direct2D1.FigureBegin.Filled);

			geometrySink.AddLines(new[] 
			{
//				startVec, midVec, 	// start -> mid,
//				midVec, endVec,		// mid -> top
//				endVec, startVec	// top -> start (cap it off)
			
				tipPoint2, triLeftPoint2,
				triLeftPoint2, barLeftPoint12,
				barLeftPoint12, barLeftPoint22,
				barLeftPoint22, barRightPoint22,
				barRightPoint22, barRightPoint12,
				barRightPoint12, triRightPoint2,
				triRightPoint2, tipPoint2 
			});
				
			geometrySink.EndFigure(SharpDX.Direct2D1.FigureEnd.Open);
			geometrySink.Close(); // calls dispose for you
			return pathGeometry;
			
		}

	
	
		
	
		  
//		public override string DisplayName
//		{
//			get
//			{
//					if (State == State.SetDefaults)
//						return ThisName;
//					else
//						return Name;
//			}
		
//		}		
		
		
		

		
		
		
		
		
		
		
//		public override string DisplayName
//		{
//			get
//			{
//				return ThisName;
//			}
		
//		}	
	
		
		
//                  AddPlot(new Stroke(Brushes.Black, 2), PlotStyle.Line, "Fast EMA");
//				 AddPlot(new Stroke(Brushes.Black, 1), PlotStyle.Line, "Slow EMA");
//				AddPlot(new Stroke(Brushes.Transparent, 1), PlotStyle.Line, "Signals");
				

//				AddPlot(new Stroke(Brushes.Transparent, 2), PlotStyle.Hash, "Stop Loss"); // 3
//				AddPlot(new Stroke(Brushes.Transparent, 3), PlotStyle.Dot, "Trailing Stop Loss");
//				AddPlot(new Stroke(Brushes.Transparent, 2), PlotStyle.Hash, "Target 1");
//				AddPlot(new Stroke(Brushes.Transparent, 2), PlotStyle.Hash, "Target 2");
				
		
		
//		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
//        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
//        public Series<double> ThisFastEMA
//        {
//            get { return Values[0]; }
//        }		
		
//        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
//        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
//        public Series<double> ThisSlowEMA
//        {
//            get { return Values[1]; }
//        }		
		
//        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
//        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
//        public Series<double> ThisSignals
//        {
//            get { return Values[2]; }
//        }		
		
//        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
//        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
//        public Series<double> ThisStopLoss
//        {
//            get { return Values[3]; }
//        }		
		
//        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
//        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
//        public Series<double> ThisTrailingStopLoss
//        {
//            get { return Values[4]; }
//        }		
		
//        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
//        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
//        public Series<double> ThisTarget1
//        {
//            get { return Values[5]; }
//        }		
				
//        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
//        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
//        public Series<double> ThisTarget2
//        {
//            get { return Values[6]; }
//        }		
		
  	public override string FormatPriceMarker(double value)
		{
			
			return AllPriceMarker(value);
			
//			if (ChartControl == null)
//			{
//				return value.ToString();
				
//			}
//			else
//			{
//				return AllPriceMarker(value);
//			}
		
		
		}
		

		
		private string AllPriceMarker (double price)
		{
			
			double ThisTickSize = Instrument.MasterInstrument.TickSize;
			
			double trunc = Math.Truncate(price);
			int fraction = 0;
			string priceMarker = "";
			if (price == 0 || price == 1 || price == -1)
			{
				
				return price.ToString();
			}
			else if (ThisTickSize == 0.03125) 
			{
				fraction = Convert.ToInt32(32 * Math.Abs(price - trunc));	
				if (fraction < 10)
					priceMarker = trunc.ToString() + "'0" + fraction.ToString();
				else if(fraction == 32)
				{	
					trunc = trunc + 1;
					fraction = 0;
					priceMarker = trunc.ToString() + "'0" + fraction.ToString();
				}	
				else 
					priceMarker = trunc.ToString() + "'" + fraction.ToString();
			}
			else if (ThisTickSize == 0.015625)
			{
				fraction = 5 * Convert.ToInt32(64 * Math.Abs(price - trunc));	
				if (fraction < 10)
					priceMarker = trunc.ToString() + "'00" + fraction.ToString();
				else if (fraction < 100)
					priceMarker = trunc.ToString() + "'0" + fraction.ToString();
				else if(fraction == 320)
				{	
					trunc = trunc + 1;
					fraction = 0;
					priceMarker = trunc.ToString() + "'00" + fraction.ToString();
				}	
				else	
					priceMarker = trunc.ToString() + "'" + fraction.ToString();
			}
			else if (ThisTickSize == 0.0078125)
			{
				fraction = Convert.ToInt32(Math.Truncate(2.5 * Convert.ToInt32(128 * Math.Abs(price - trunc))));	
				if (fraction < 10)
					priceMarker = trunc.ToString() + "'00" + fraction.ToString();
				else if (fraction < 100)
					priceMarker = trunc.ToString() + "'0" + fraction.ToString();
				else if(fraction == 320)
				{	
					trunc = trunc + 1;
					fraction = 0;
					priceMarker = trunc.ToString() + "'00" + fraction.ToString();
				}	
				else	
					priceMarker = trunc.ToString() + "'" + fraction.ToString();
			}
			else
			{
				priceMarker = price.ToString(NinjaTrader.Core.Globals.GetTickFormatString(ThisTickSize));
			}
			return priceMarker;
		}		
		
		
		
		
		
		
		
		
		// Bar Counter
		
		
		private bool pTimerEnabled = false;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Bar Counter", Name = "Enabled", Description = "", Order = 0)]
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        public bool TimerEnabled
        {
            get { return pTimerEnabled; }
            set { pTimerEnabled = value; }
        }		

		private SimpleFont pTextFontTime = new SimpleFont("Consolas", 13);
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Bar Counter", Name = "Text Font", Description = "", Order = 10)]
		public SimpleFont TextFontTime
        {
            get { return pTextFontTime; }
            set { pTextFontTime= value; }
        }	

		private Brush pTimerMainColor = Brushes.DimGray;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Bar Counter", Name = "Text Color", Order = 11)]
		public Brush TimerMainColor
		{
			get { return pTimerMainColor; } set { pTimerMainColor = value; }
		}
		[Browsable(false)]
		public string TimerMainColorS
		{
			get { return Serialize.BrushToString(pTimerMainColor); } set { pTimerMainColor = Serialize.StringToBrush(value); }
		}		
		
		
		
		
		private int pTimerOffset = 0;
        [Range(-1000, 1000)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Bar Counter", Name = "Offset Y (Pixels)", Order = 20)]
        public int TimerOffset
        {
            get { return pTimerOffset; }
            set { pTimerOffset = value; }
        }				
		
		private int pTimerOffsetX = 0;
        [Range(-1000, 1000)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Bar Counter", Name = "Offset X (Pixels)", Order = 20)]
        public int TimerOffsetX
        {
            get { return pTimerOffsetX; }
            set { pTimerOffsetX = value; }
        }		
		
		
		private bool pCountDown = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Bar Counter", Name = "Count Down", Description = "", Order = 30)]
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        public bool CountDown
        {
            get { return pCountDown; }
            set { pCountDown = value; }
        }		
		
		private bool pShowPercent = false;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Bar Counter", Name = "Show Percent", Description = "", Order = 31)]
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        public bool ShowPercent
        {
            get { return pShowPercent; }
            set { pShowPercent = value; }
        }				
			
	
 		private bool pClockErrorMessagesEnabled = false;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Clock Error Checking Enabled", GroupName = "Bar Counter", Order = 39)]
		[Description("option to enable or disable standard error messages in your Chart window.")]
        public bool ClockErrorMessagesEnabled
        {
            get { return pClockErrorMessagesEnabled; }
            set { pClockErrorMessagesEnabled = value; }
        }
				
		
		
		
// HUD
		
		
		private bool pHUDEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Delta Trend Display ", Name = "Enabled", Description = "", Order = 0)]
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        public bool HUDEnabled
        {
            get { return pHUDEnabled; }
            set { pHUDEnabled = value; }
        }
		
			
		private bool pHUD2 = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Delta Trend Display ", Name = "Bar Delta Enabled", Description = "", Order = 1)]
        public bool HUD2
        {
            get { return pHUD2; }
            set { pHUD2 = value; }
        }	
		
		
		private bool pHUD1 = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Delta Trend Display ", Name = "Total Delta Enabled", Description = "", Order = 2)]
        public bool HUD1
        {
            get { return pHUD1; }
            set { pHUD1 = value; }
        }		
		

		
//		private bool pHUD3 = true;
//        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Delta Trend Display ", Name = "Total Delta Enabled", Description = "", Order = 3)]
//        public bool HUD3
//        {
//            get { return pHUD3; }
//            set { pHUD3 = value; }
//        }	
		
//		private bool pHUD4 = true;
//        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Delta Trend Display ", Name = "Bar Delta Enabled", Description = "", Order = 4)]
//        public bool HUD4
//        {
//            get { return pHUD4; }
//            set { pHUD4 = value; }
//        }		
		
//		private bool pHUD5 = false;
//        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Delta Trend Display ", Name = "Bar Bid / Ask Enabled", Description = "", Order = 5)]
//        public bool HUD5
//        {
//            get { return pHUD5; }
//            set { pHUD5 = value; }
//        }	
		
//		private string pNetDMode2 = "Total";
//		[Description("")]
//		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Delta Trend Display ", Name = "Bar Delta Mode", Description = "",  Order = 10)]
//		[RefreshProperties(RefreshProperties.All)]
//		[TypeConverter(typeof(ChooseNDMode))]
//		public string NetDMode2
//		{
//			get { return pNetDMode2; }
//			set { pNetDMode2 = value; }
//		}
		
		
		
//		internal class ChooseNDMode : StringConverter
//		{
//			public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
//			{
//			//true means show a combobox
//				return true;
//			}
			
//			public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
//			{
//			//true will limit to list. false will show the list, but allow free-form entry
//				return true;
//			}
		
//			public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
//			{
//				return new StandardValuesCollection( new String[] {"Total", "Percent"} );
//			}
//		}
		

//		private Brush pHUDVOLColor = Brushes.SteelBlue;
//		[XmlIgnore]
//		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Delta Trend Display ",  Name = "Color Volume", Order = 20)]
//		public Brush HUDVOLColor
//		{
//			get { return pHUDVOLColor; } set { pHUDVOLColor = value; }
//		}
//		[Browsable(false)]
//		public string HUDVOLColorS
//		{
//			get { return Serialize.BrushToString(pHUDVOLColor); } set { pHUDVOLColor = Serialize.StringToBrush(value); }
//		}
		
		private Brush pHUDNEColor = Brushes.Gray;
//		[XmlIgnore]
//		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Delta Trend Display ",  Name = "Color Neutral", Order = 20)]
//		public Brush HUDNEColor
//		{
//			get { return pHUDNEColor; } set { pHUDNEColor = value; }
//		}
//		[Browsable(false)]
//		public string HUDNEColorS
//		{
//			get { return Serialize.BrushToString(pHUDNEColor); } set { pHUDNEColor = Serialize.StringToBrush(value); }
//		}
		
		private Brush pHUDUPColor = Brushes.Green;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Delta Trend Display ",  Name = "Color Buy", Order = 20)]
		public Brush HUDUPColor
		{
			get { return pHUDUPColor; } set { pHUDUPColor = value; }
		}
		[Browsable(false)]
		public string HUDUPColorS
		{
			get { return Serialize.BrushToString(pHUDUPColor); } set { pHUDUPColor = Serialize.StringToBrush(value); }
		}
		
		
		private Brush pHUDDNColor = Brushes.Red;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Delta Trend Display ", Name = "Color Sell", Order = 20)]
		public Brush HUDDNColor
		{
			get { return pHUDDNColor; } set { pHUDDNColor = value; }
		}
		[Browsable(false)]
		public string HUDDNColorS
		{
			get { return Serialize.BrushToString(pHUDDNColor); } set { pHUDDNColor = Serialize.StringToBrush(value); }
		}
		
		
		private int pHUDDefaultOpacity = 40;
        [Range(0, 100)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Delta Trend Display ", Name = "Default Opacity (%)", Order = 22)]
        public int HUDDefaultOpacity
        {
            get { return pHUDDefaultOpacity; }
            set { pHUDDefaultOpacity = value; }
        }	
		
		
		
		private int pHUDMinOpacity = 20;
        [Range(0, 100)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Delta Trend Display ", Name = "Minimum Opacity (%)", Order = 23)]
        public int HUDMinOpacity
        {
            get { return pHUDMinOpacity; }
            set { pHUDMinOpacity = value; }
        }					

		private int pHUDMaxOpacity = 80;
        [Range(0, 100)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Delta Trend Display ", Name = "Maximum Opacity (%)", Order = 24)]
        public int HUDMaxOpacity
        {
            get { return pHUDMaxOpacity; }
            set { pHUDMaxOpacity = value; }
        }	
		
		private bool pHUD3 = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Delta Trend Display ", Name = "Text Enabled", Description = "", Order = 29)]
        public bool HUD3
        {
            get { return pHUD3; }
            set { pHUD3 = value; }
        }	
		
		
		private SimpleFont pTextFont2 = new SimpleFont("Consolas", 11);
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Delta Trend Display ", Name = "Text Font", Description = "", Order = 30)]
		public SimpleFont TextFont2
        {
            get { return pTextFont2; }
            set { pTextFont2= value; }
        }	
		
		
		
		
		// CUMULATIVE DELTA
		
		
		
		
		private bool pCuEnabled = false;
				[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
		[Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", Description = "", GroupName = "Delta Trend ", Order = 1)]
        public bool CuEnabled
        {
            get { return pCuEnabled; }
            set { pCuEnabled = value; }
        }	
		
//		private CumulativeDeltaType pThisCumulativeDeltaType = CumulativeDeltaType.UpDownTick;
//		[NinjaScriptProperty]
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Delta type", Description = "", GroupName = "Delta Trend ", Order = 2)]
//        public CumulativeDeltaType ThisCumulativeDeltaType
//        {
//            get { return pThisCumulativeDeltaType; }
//			set { pThisCumulativeDeltaType = value; }
//        }		
		
//		private CumulativeDeltaPeriod pThisCumulativeDeltaPeriod = CumulativeDeltaPeriod.Session;
//		[NinjaScriptProperty]
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Period", Description = "", GroupName = "Delta Trend ", Order = 3)]
//        public CumulativeDeltaPeriod ThisCumulativeDeltaPeriod
//        {
//            get { return pThisCumulativeDeltaPeriod; }
//			set { pThisCumulativeDeltaPeriod = value; }
//        }				
		
//		private int	pCumulativeSizeFilter	= 0;
//		[NinjaScriptProperty]
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Size filter", Description = "", GroupName = "Delta Trend ", Order = 5)]
//        public int CumulativeSizeFilter
//        {
//            get { return pCumulativeSizeFilter; }
//			set { pCumulativeSizeFilter = Math.Min(1000000, Math.Max(value, 0)); }
//        }	
		
		
		
			
			private bool pExitsRTTS = true;
							
	 
		
			private int	pATRPeriod = 14;
			[Range(1, int.MaxValue)]
			[Display(ResourceType = typeof(Custom.Resource), Name = "ATR Period", Description = "", GroupName = "Exit Orders", Order = 1)]
			public int ATRPeriod
			{
				get { return pATRPeriod; }
				set { pATRPeriod = value; }
			}	
			
	
	        private bool pPT1Enabled = true;
	        [Display(ResourceType = typeof(Custom.Resource), Name = "Target 1 Enabled", Description = "", GroupName = "Exit Orders", Order = 5)]
	        public bool PT1Enabled
	        {
	            get { return pPT1Enabled; }
	            set { pPT1Enabled = value; }
	        }		
			
			
			private double pATRTargetM = 1.5;
			[Range(0, double.MaxValue)]
			[Display(ResourceType = typeof(Custom.Resource), Name = "Target 1 Multiplier", Description = "", GroupName = "Exit Orders", Order = 10)]
			public double ATRTargetM
			{
				get { return pATRTargetM; }
				set { pATRTargetM = value; }
			}

			
	        private bool pPT2Enabled = true;
	        [Display(ResourceType = typeof(Custom.Resource), Name = "Target 2 Enabled", Description = "", GroupName = "Exit Orders", Order = 15)]
	        public bool PT2Enabled
	        {
	            get { return pPT2Enabled; }
	            set { pPT2Enabled = value; }
	        }		
			
						
			private double pATRTargetM2 = 2.5;
			[Range(0, double.MaxValue)]
			[Display(ResourceType = typeof(Custom.Resource), Name = "Target 2 Multiplier", Description = "", GroupName = "Exit Orders", Order = 20)]
			public double ATRTargetM2
			{
				get { return pATRTargetM2; }
				set { pATRTargetM2 = value; }
			}			
     


		private double pATRStopM = 2;
			[Range(0, double.MaxValue)]
			[Display(ResourceType = typeof(Custom.Resource), Name = "Stop Loss Multiplier", Description = "", GroupName = "Exit Orders", Order = 28)]
			public double ATRStopM
			{
				get { return pATRStopM; }
				set { pATRStopM = value; }
			}				
			
		private bool pSLTrailEnabled = false;
//        [Display(ResourceType = typeof(Custom.Resource), Name = "Trailing Stop Loss Enabled", GroupName = "Exit Orders", Order = 40)]
//        public bool SLTrailEnabled
//        {
//            get { return pSLTrailEnabled; }
//            set { pSLTrailEnabled = value; }
//        }	

		private double pATRStopMT = 2.2;
//			[Range(0, double.MaxValue)]
//			[Display(ResourceType = typeof(Custom.Resource), Name = "Trailing Stop Loss Multiplier", Description = "", GroupName = "Exit Orders", Order = 41)]
//			public double ATRStopMT
//			{
//				get { return pATRStopMT; }
//				set { pATRStopMT = value; }
//			}		
			
			private int	pTSOffsetTicks = 1;
//			[Range(int.MinValue, int.MaxValue)]
//			[Display(ResourceType = typeof(Custom.Resource), Name = "Trailing Stop Loss Offset (Ticks)", Description = "", GroupName = "Exit Orders", Order = 42)]
//			public int TSOffsetTicks
//			{
//				get { return pTSOffsetTicks; }
//				set { pTSOffsetTicks = value; }
//			}					
			
			
			
 		private bool pRunnerTrailEnabled = false;
//        [Display(ResourceType = typeof(Custom.Resource), Name = "Trailing Stop Loss Runner Enabled", GroupName = "Exit Orders", Order = 50)]
//		[Description("allows the trailing stop dots and exit orders to continue drawing until they are hit.")]
//        public bool RunnerTrailEnabled
//        {
//            get { return pRunnerTrailEnabled; }
//            set { pRunnerTrailEnabled = value; }
//        }	
		
		
			private bool pExitOEnabled = true;
					[RefreshProperties(RefreshProperties.All)]
	        [Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", Description = "", GroupName = "Exit Orders Display", Order = 29)]
	        public bool ExitOEnabled
	        {
	            get { return pExitOEnabled; }
	            set { pExitOEnabled = value; }
	        }		
 
			private bool pExitHOEnabled = true;
				//	[RefreshProperties(RefreshProperties.All)]
	        [Display(ResourceType = typeof(Custom.Resource), Name = "Historical Enabled", Description = "", GroupName = "Exit Orders Display", Order = 30)]
	        public bool ExitHOEnabled
	        {
	            get { return pExitHOEnabled; }
	            set { pExitHOEnabled = value; }
	        }							
        private Stroke pTargetStroke = new Stroke(Brushes.Wheat, DashStyleHelper.Dash, 1);
        [Display(ResourceType = typeof(Custom.Resource), Name = "Target Display", Description = "", GroupName = "Exit Orders Display", Order = 31)]
        public Stroke TargetStroke
        {
            get { return pTargetStroke; }
            set { pTargetStroke = value; }
        }			
		
        private Stroke pStopStroke = new Stroke(Brushes.Red, DashStyleHelper.Dash, 1);
        [Display(ResourceType = typeof(Custom.Resource), Name = "Stop Loss Display", Description = "", GroupName = "Exit Orders Display", Order = 32)]
        public Stroke StopStroke
        {
            get { return pStopStroke; }
            set { pStopStroke = value; }
        }


	  private bool pLabelsEnabled2 = true;
		//[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        [Display(ResourceType = typeof(Custom.Resource), Name = "Labels Enabled", Description = "", GroupName = "Exit Orders Display", Order = 35)]
        public bool LabelsEnabled2
        {
            get { return pLabelsEnabled2; }
            set { pLabelsEnabled2 = value; }
        }			
		

		

		private bool pSLTrailOrdersEnabled = false;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Trailing Stop Loss Orders Enabled", GroupName = "Exit Orders Display", Order = 40)]
        public bool SLTrailOrdersEnabled
        {
            get { return pSLTrailOrdersEnabled; }
            set { pSLTrailOrdersEnabled = value; }
        }	
			
		private bool pExitOrdersEnabled = false;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Exit Orders Enabled", GroupName = "Exit Orders Display", Order = 40)]
        public bool ExitOrdersEnabled
        {
            get { return pExitOrdersEnabled; }
            set { pExitOrdersEnabled = value; }
        }			
		
		private Brush pPlotDownFBrush33	= Brushes.Red;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Trailing Stop Loss Dot Color", Description = "", GroupName = "Exit Orders Display", Order = 45)]
		public Brush PlotDownFBrush33
		{
			get { return pPlotDownFBrush33; } set { pPlotDownFBrush33 = value; }
		}
		[Browsable(false)]
		public string PlotDownFBrushS33
		{
			get { return Serialize.BrushToString(pPlotDownFBrush33); } set { pPlotDownFBrush33 = Serialize.StringToBrush(value); }
		}	

		
		private float pWidth5 = 3;
        [Range(1, float.MaxValue)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Trailing Stop Loss Dot Width", GroupName = "Exit Orders Display", Order = 50)]
        public float Width5
        {
            get { return pWidth5; }
            set { pWidth5 = value; }
        }							
	
		
		
   
				
		
			
		
		
		
		
		
		
		private bool pSecondaryFeedsEnabled = false;
				[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
		[Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", Description = "", GroupName = "Secondary Trend", Order = 1)]
        public bool SecondaryFeedsEnabled
        {
            get { return pSecondaryFeedsEnabled; }
            set { pSecondaryFeedsEnabled = value; }
        }	
		
		private bool pFeed1Enabled = true;
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
		[Display(ResourceType = typeof(Custom.Resource), Name = "Trend 1 Enabled", Description = "", GroupName = "Secondary Trend", Order = 10)]
        public bool Feed1Enabled
        {
            get { return pFeed1Enabled; }
            set { pFeed1Enabled = value; }
        }	
		
		private bool pFeed2Enabled = true;
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
		[Display(ResourceType = typeof(Custom.Resource), Name = "Trend 2 Enabled", Description = "", GroupName = "Secondary Trend", Order = 15)]
        public bool Feed2Enabled
        {
            get { return pFeed2Enabled; }
            set { pFeed2Enabled = value; }
        }			
		
		private bool pFeed3Enabled = true;
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
		[Display(ResourceType = typeof(Custom.Resource), Name = "Trend 3 Enabled", Description = "", GroupName = "Secondary Trend", Order = 20)]
        public bool Feed3Enabled
        {
            get { return pFeed3Enabled; }
            set { pFeed3Enabled = value; }
        }	
		
		private bool pFeed4Enabled = true;
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
		[Display(ResourceType = typeof(Custom.Resource), Name = "Trend 4 Enabled", Description = "", GroupName = "Secondary Trend", Order = 25)]
        public bool Feed4Enabled
        {
            get { return pFeed4Enabled; }
            set { pFeed4Enabled = value; }
        }			
		
		private bool pFeed1Included = true;
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
		[Display(ResourceType = typeof(Custom.Resource), Name = "Trend 1 Included", Description = "", GroupName = "Secondary Trend", Order = 10)]
        public bool Feed1Included
        {
            get { return pFeed1Included; }
            set { pFeed1Included = value; }
        }	
		
		private bool pFeed2Included = true;
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
		[Display(ResourceType = typeof(Custom.Resource), Name = "Trend 2 Included", Description = "", GroupName = "Secondary Trend", Order = 15)]
        public bool Feed2Included
        {
            get { return pFeed2Included; }
            set { pFeed2Included = value; }
        }			
		
		private bool pFeed3Included = false;
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
		[Display(ResourceType = typeof(Custom.Resource), Name = "Trend 3 Included", Description = "", GroupName = "Secondary Trend", Order = 20)]
        public bool Feed3Included
        {
            get { return pFeed3Included; }
            set { pFeed3Included = value; }
        }	
		
		private bool pFeed4Included = false;
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
		[Display(ResourceType = typeof(Custom.Resource), Name = "Trend 4 Included", Description = "", GroupName = "Secondary Trend", Order = 25)]
        public bool Feed4Included
        {
            get { return pFeed4Included; }
            set { pFeed4Included = value; }
        }			
		
		
		
		
		
		
			private int	pEMAPeriod1 = 5;
			[Range(1, int.MaxValue)]
			[Display(ResourceType = typeof(Custom.Resource), Name = "   Bars Period", Description = "", GroupName = "Secondary Trend", Order = 11)]
			public int EMAPeriod1
			{
				get { return pEMAPeriod1; }
				set { pEMAPeriod1 = value; }
			}	
			
			private int	pEMAPeriod2 = 15;
			[Range(1, int.MaxValue)]
			[Display(ResourceType = typeof(Custom.Resource), Name = "   Bars Period", Description = "", GroupName = "Secondary Trend", Order = 16)]
			public int EMAPeriod2
			{
				get { return pEMAPeriod2; }
				set { pEMAPeriod2 = value; }
			}				
		
			private int	pEMAPeriod3 = 30;
			[Range(1, int.MaxValue)]
			[Display(ResourceType = typeof(Custom.Resource), Name = "   Bars Period", Description = "", GroupName = "Secondary Trend", Order = 21)]
			public int EMAPeriod3
			{
				get { return pEMAPeriod3; }
				set { pEMAPeriod3 = value; }
			}	
			
			private int	pEMAPeriod4 = 60;
			[Range(1, int.MaxValue)]
			[Display(ResourceType = typeof(Custom.Resource), Name = "   Bars Period", Description = "", GroupName = "Secondary Trend", Order = 26)]
			public int EMAPeriod4
			{
				get { return pEMAPeriod4; }
				set { pEMAPeriod4 = value; }
			}	
			
		private BarsPeriodType AcceptableBasePeriodType1 = BarsPeriodType.Minute;		
		private string pThisBarType1 = "Minute";
		[Description("")]
		[Display(ResourceType = typeof(Custom.Resource), Name = "   Bars Type", Description = "", GroupName = "Secondary Trend", Order = 12)]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(PTS))]
		public string ThisBarType1
		{
            get { return pThisBarType1; }
            set 
			{ 
				pThisBarType1 = value; 
			
				switch (pThisBarType1) 
				{
					case "Tick":   AcceptableBasePeriodType1 = BarsPeriodType.Tick; break;
					case "Volume":  AcceptableBasePeriodType1 = BarsPeriodType.Volume; break;
					case "Range": AcceptableBasePeriodType1 = BarsPeriodType.Range; break;
					case "Second": AcceptableBasePeriodType1 = BarsPeriodType.Second; break;
					case "Minute": AcceptableBasePeriodType1 = BarsPeriodType.Minute; break;
//					case "Renko": AcceptableBasePeriodType1 = BarsPeriodType.Renko; break;
//					case "Day": AcceptableBasePeriodType1 = BarsPeriodType.Day; break;
//					case "Week": AcceptableBasePeriodType1 = BarsPeriodType.Week; break;
//					case "Month": AcceptableBasePeriodType1 = BarsPeriodType.Month; break;						
						
				}	
				
			}
		}			
			
		private BarsPeriodType AcceptableBasePeriodType2 = BarsPeriodType.Minute;		
		private string pThisBarType2 = "Minute";
		[Description("")]
		[Display(ResourceType = typeof(Custom.Resource), Name = "   Bars Type", Description = "", GroupName = "Secondary Trend", Order = 17)]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(PTS))]
		public string ThisBarType2
		{
            get { return pThisBarType2; }
            set 
			{ 
				pThisBarType2 = value; 
			
				switch (pThisBarType2) 
				{
					case "Tick":   AcceptableBasePeriodType2 = BarsPeriodType.Tick; break;
					case "Volume":  AcceptableBasePeriodType2 = BarsPeriodType.Volume; break;
					case "Range": AcceptableBasePeriodType2 = BarsPeriodType.Range; break;
					case "Second": AcceptableBasePeriodType2 = BarsPeriodType.Second; break;
					case "Minute": AcceptableBasePeriodType2 = BarsPeriodType.Minute; break;
//					case "Renko": AcceptableBasePeriodType2 = BarsPeriodType.Renko; break;
//					case "Day": AcceptableBasePeriodType2 = BarsPeriodType.Day; break;
//					case "Week": AcceptableBasePeriodType2 = BarsPeriodType.Week; break;
//					case "Month": AcceptableBasePeriodType2 = BarsPeriodType.Month; break;						
						
				}	
				
			}
		}	
		
		private BarsPeriodType AcceptableBasePeriodType3 = BarsPeriodType.Minute;		
		private string pThisBarType3 = "Minute";
		[Description("")]
		[Display(ResourceType = typeof(Custom.Resource), Name = "   Bars Type", Description = "", GroupName = "Secondary Trend", Order = 22)]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(PTS))]
		public string ThisBarType3
		{
            get { return pThisBarType3; }
            set 
			{ 
				pThisBarType3 = value; 
			
				switch (pThisBarType3) 
				{
					case "Tick":   AcceptableBasePeriodType3 = BarsPeriodType.Tick; break;
					case "Volume":  AcceptableBasePeriodType3 = BarsPeriodType.Volume; break;
					case "Range": AcceptableBasePeriodType3 = BarsPeriodType.Range; break;
					case "Second": AcceptableBasePeriodType3 = BarsPeriodType.Second; break;
					case "Minute": AcceptableBasePeriodType3 = BarsPeriodType.Minute; break;
//					case "Renko": AcceptableBasePeriodType3 = BarsPeriodType.Renko; break;
//					case "Day": AcceptableBasePeriodType3 = BarsPeriodType.Day; break;
//					case "Week": AcceptableBasePeriodType3 = BarsPeriodType.Week; break;
//					case "Month": AcceptableBasePeriodType3 = BarsPeriodType.Month; break;						
						
				}	
				
			}
		}	
		
		
		private BarsPeriodType AcceptableBasePeriodType4 = BarsPeriodType.Minute;		
		private string pThisBarType4 = "Minute";
		[Description("")]
		[Display(ResourceType = typeof(Custom.Resource), Name = "   Bars Type", Description = "", GroupName = "Secondary Trend", Order = 27)]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(PTS))]
		public string ThisBarType4
		{
            get { return pThisBarType4; }
            set 
			{ 
				pThisBarType4 = value; 
			
				switch (pThisBarType4) 
				{
					case "Tick":   AcceptableBasePeriodType4 = BarsPeriodType.Tick; break;
					case "Volume":  AcceptableBasePeriodType4 = BarsPeriodType.Volume; break;
					case "Range": AcceptableBasePeriodType4 = BarsPeriodType.Range; break;
					case "Second": AcceptableBasePeriodType4 = BarsPeriodType.Second; break;
					case "Minute": AcceptableBasePeriodType4 = BarsPeriodType.Minute; break;
//					case "Renko": AcceptableBasePeriodType4 = BarsPeriodType.Renko; break;
//					case "Day": AcceptableBasePeriodType4 = BarsPeriodType.Day; break;
//					case "Week": AcceptableBasePeriodType4 = BarsPeriodType.Week; break;
//					case "Month": AcceptableBasePeriodType4 = BarsPeriodType.Month; break;						
						
				}	
				
			}
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
				//return new StandardValuesCollection( new String[] { "Tick", "Volume", "Range", "Second", "Minute", "Renko" } );
				return new StandardValuesCollection( new String[] { "Tick", "Volume", "Range", "Second", "Minute" } );
			}
		}	
		
		
		
		private bool pSecondaryFeedsDisplayEnabled = true;
				[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
		[Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", Description = "", GroupName = "Secondary Trend Box Display", Order = 0)]
        public bool SecondaryFeedsDisplayEnabled
        {
            get { return pSecondaryFeedsDisplayEnabled; }
            set { pSecondaryFeedsDisplayEnabled = value; }
        }	
		
		
		
		
						[Display(Name="Text Font", Description="", GroupName= "Secondary Trend Box Display", Order = 1)]
			public SimpleFont TextFont
			{ get; set; }	
			
			
			private System.Windows.Media.Brush pColorTextBrush	= Brushes.Gainsboro;
			[XmlIgnore]
			[Display(ResourceType = typeof(Custom.Resource), Name = "Text Color", Description = "", GroupName =  "Secondary Trend Box Display", Order = 2)]
			public System.Windows.Media.Brush ColorTextBrush
			{
				get { return pColorTextBrush; } set { pColorTextBrush = value; }
			}
			[Browsable(false)]
			public string ColorTextBrushS
			{
				get { return Serialize.BrushToString(pColorTextBrush); } set { pColorTextBrush = Serialize.StringToBrush(value); }
			}	
		
			
//					internal class TotalMode : StringConverter
//		{
//			public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
//			{
//			//true means show a combobox
//				return true;
//			}
			
//			public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
//			{
//			//true will limit to list. false will show the list, but allow free-form entry
//				return true;
//			}
		
//			public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
//			{
//				return new StandardValuesCollection( new String[] {"Market", "Limit"} );
//			}
//		}	

//	private string pThisEntryType = "Market";
//		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Execution", Name = "Entry Type", Description = "",  Order = 8)]
//		[Description("")]
//		//[RefreshProperties(RefreshProperties.All)]
//		[TypeConverter(typeof(TotalMode))]
//		public string ThisEntryType
//		{
//			get { return pThisEntryType; }
//			set { pThisEntryType = value; }
//		}		
					
		
		
//			private LVVolume_TablePosition pTPosition = LVVolume_TablePosition.;
//			[NinjaScriptProperty]
//			[Display(Name = "Position", Description = "", GroupName =  "Secondary Trend Box Display", Order = 3)]
//			public LVVolume_TablePosition TPosition
//			{
//				get { return pTPosition; }
//				set { pTPosition = value; }
//			}	
			
			private int	pPixelsFromRight = 10;
			[Range(2, int.MaxValue)]
			[Display(ResourceType = typeof(Custom.Resource), Name = "Offset X (Pixels)", Description = "", GroupName =  "Secondary Trend Box Display", Order = 5)]
			public int PixelsFromRight
			{
				get { return pPixelsFromRight; }
				set { pPixelsFromRight = value; }
			}	
			
			private int	pPixelsFromBottom = 50;
			[Range(2, int.MaxValue)]
			[Display(ResourceType = typeof(Custom.Resource), Name = "Offset Y (Pixels)", Description = "", GroupName =  "Secondary Trend Box Display", Order = 6)]
			public int PixelsFromBottom
			{
				get { return pPixelsFromBottom; }
				set { pPixelsFromBottom = value; }
			}		
			
			private int	pMarginB = 10;
//			[Range(0, int.MaxValue)]
//			[Display(ResourceType = typeof(Custom.Resource), Name = "Margin (Pixels)", Description = "", GroupName =  "Secondary Trend Box Display", Order = 7)]
//			public int MarginB
//			{
//				get { return pMarginB; }
//				set { pMarginB = value; }
//			}					
			
			
			private System.Windows.Media.Brush pFillUpBrush	= new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(3,128,0));
			[XmlIgnore]
			[Display(ResourceType = typeof(Custom.Resource), Name = "Color Up", Description="", GroupName =  "Secondary Trend Box Display", Order = 8)]
			public System.Windows.Media.Brush FillUpBrush
			{
				get { return pFillUpBrush; } set { pFillUpBrush = value; }
			}
			[Browsable(false)]
			public string FillUpBrushS
			{
				get { return Serialize.BrushToString(pFillUpBrush); } set { pFillUpBrush = Serialize.StringToBrush(value); }
			}	
				
			private System.Windows.Media.Brush pFillNeutralBrush	= Brushes.Silver;
//			[XmlIgnore]
//			[Display(ResourceType = typeof(Custom.Resource), Name = "Color Flash", GroupName =  "Secondary Trend Box Display", Order = 9)]
//			public System.Windows.Media.Brush FillNeutralBrush
//			{
//				get { return pFillNeutralBrush; } set { pFillNeutralBrush = value; }
//			}
//			[Browsable(false)]
//			public string FillNeutralBrushS
//			{
//				get { return Serialize.BrushToString(pFillNeutralBrush); } set { pFillNeutralBrush = Serialize.StringToBrush(value); }
//			}	
			
			private System.Windows.Media.Brush pFillDownBrush	= new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(204,0,0));
			[XmlIgnore]
			[Display(ResourceType = typeof(Custom.Resource), Name = "Color Down", GroupName =  "Secondary Trend Box Display", Order = 10)]
			public System.Windows.Media.Brush FillDownBrush
			{
				get { return pFillDownBrush; } set { pFillDownBrush = value; }
			}
			[Browsable(false)]
			public string FillDownBrushS
			{
				get { return Serialize.BrushToString(pFillDownBrush); } set { pFillDownBrush = Serialize.StringToBrush(value); }
			}	
						
			private int	pFillOpacity = 80;
			[Range(0, 100)]
			[Display(ResourceType = typeof(Custom.Resource), Name = "Fill Opacity (%)", Description = "", GroupName =  "Secondary Trend Box Display", Order = 11)]
			public int FillOpacity
			{
				get { return pFillOpacity; }
				set { pFillOpacity = value; }
			}	
			
			private int	pOutlineOpacity = 100;
			[Range(0, 100)]
			[Display(ResourceType = typeof(Custom.Resource), Name = "Outline Opacity (%)", Description = "", GroupName =  "Secondary Trend Box Display", Order = 12)]
			public int OutlineOpacity
			{
				get { return pOutlineOpacity; }
				set { pOutlineOpacity = value; }
			}	
			

			private int	pColumnWidthP = 30;
			
			
		
        // INPUTS
	
		
		internal class EntryHandling : StringConverter
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
				return new StandardValuesCollection( new String[] { "AllEntries", "FlatEntries", "UniqueEntries"} );
			}
		}			
		
		internal class TotalMode : StringConverter
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
				return new StandardValuesCollection( new String[] {"Market", "Limit"} );
			}
		}	

		
		
		private bool pUseTimeFilter = true;
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
				
		
		
		private string pThisEntryType22 = "EMA";
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Parameters ", Name = "Moving Average Type", Description = "",  Order = 2)]
		[Description("")]
		//[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(TotalMode22))]
		public string ThisEntryType22
		{
			get { return pThisEntryType22; }
			set { pThisEntryType22 = value; }
		}		
			
		
		
			
		internal class TotalMode22 : StringConverter
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
				return new StandardValuesCollection( new String[] {"DEMA", "EMA", "HMA", "SMA", "TMA", "WMA"} );
			}
		}	
		
		
   
		
		private int pEMAFastPeriod = 20;
		[Range(1, int.MaxValue)]
        //[Range(1, int.MaxValue), NinjaScriptProperty]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Moving Average Period", GroupName = "Parameters ", Order = 1)]
        public int EMAFastPeriod
        {
            get { return pEMAFastPeriod; }
            set { pEMAFastPeriod = value; }
        }	
		
		private int pEMASlowPeriod = 50;
//        [Range(1, int.MaxValue)]
//        [Display(ResourceType = typeof(Custom.Resource), Name = "Slow Period", GroupName = "Parameters ", Order = 2)]
//        public int EMASlowPeriod
//        {
//            get { return pEMASlowPeriod; }
//            set { pEMASlowPeriod = value; }
//        }	
		
	
		
		private int pNumberOfBars = 8;
		[Range(0, int.MaxValue), NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Bar Count (Minimum)", Description = "", GroupName = "Parameters ", Order = 6)]
		public int NumberOfBars
        {
			get { return pNumberOfBars; }
			set { pNumberOfBars = Math.Min(10000, Math.Max(value, 1)); }
        }
		
		private int pNumberOfBars3 = 50;
		[Range(0, int.MaxValue), NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Bar Count (Maximum)", Description = "", GroupName = "Parameters ", Order = 7)]
		public int NumberOfBars3
        {
			get { return pNumberOfBars3; }
			set { pNumberOfBars3 = Math.Min(10000, Math.Max(value, 1)); }
        }		
		
		
		
//		private int pEMASlowPeriod2 = 1;
//        [Range(1, int.MaxValue), NinjaScriptProperty]
//        [Display(ResourceType = typeof(Custom.Resource), Name = "Entry Offset Maximum (Bars)", GroupName = "Parameters ", Order = 3)]
//        public int EMASlowPeriod2
//        {
//            get { return pEMASlowPeriod2; }
//            set { pEMASlowPeriod2 = value; }
//        }	
		
 
		
// 		private bool pRequireReset = true;
//        [Display(ResourceType = typeof(Custom.Resource), Name = "Gap Reset Enabled", GroupName = "Parameters ", Order = 5)]
//        public bool RequireReset
//        {
//            get { return pRequireReset; }
//            set { pRequireReset = value; }
//        }
				
				
		
//		private bool pRequireSlope = true;
//        [Display(ResourceType = typeof(Custom.Resource), Name = "Slope Filter Enabled", GroupName = "Parameters ", Order = 10)]
//        public bool RequireSlope
//        {
//            get { return pRequireSlope; }
//            set { pRequireSlope = value; }
//        }		
		
						
// 		private bool pRequireReset2 = false;
//        [Display(ResourceType = typeof(Custom.Resource), Name = "Trend Filter Enabled", GroupName = "Parameters ", Order = 11)]
//        public bool RequireReset2
//        {
//            get { return pRequireReset2; }
//            set { pRequireReset2 = value; }
//        }
		

//		private int pLength2 = 15;
//        [Range(1, int.MaxValue), NinjaScriptProperty]
//        [Display(ResourceType = typeof(Custom.Resource), Name = "Trend Filter Period", GroupName = "Parameters ", Order = 12)]
//        public int Length2
//        {
//            get { return pLength2; }
//            set { pLength2 = value; }
//        }	
		
		


		
		
		
		private bool pEntriesEnabled = false;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Entries Enabled", GroupName = "Execution", Order = 0)]
		[Description("You can set this via the Entry button on the ChartTrader Panel.")]
        public bool EntriesEnabled
        {
            get { return pEntriesEnabled; }
            set { pEntriesEnabled = value; }
        }	
		
		
		private bool pLongEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Long Enabled", GroupName = "Execution", Order = 1)]
		[Description("You can set this via the Trades button on the ChartTrader Panel.")]
        public bool LongEnabled
        {
            get { return pLongEnabled; }
            set { pLongEnabled = value; }
        }
				
		private bool pShortEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Short Enabled", GroupName = "Execution", Order = 2)]
		[Description("You can set this via the Trades button on the ChartTrader Panel.")]
        public bool ShortEnabled
        {
            get { return pShortEnabled; }
            set { pShortEnabled = value; }
        }
				
		private bool pTrendOnlyEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Trend Only Enabled", GroupName = "Execution", Order = 2)]
		[Description("You can set this via the Trades button on the ChartTrader Panel.")]
        public bool TrendOnlyEnabled
        {
            get { return pTrendOnlyEnabled; }
            set { pTrendOnlyEnabled = value; }
        }
		
		private bool pAutoEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Auto Mode Enabled", GroupName = "Execution", Order = 3)]
        public bool AutoEnabled
        {
            get { return pAutoEnabled; }
            set { pAutoEnabled = value; }
        }
		
		
	
		
		private bool pDoReverse = false;
//        [Display(ResourceType = typeof(Custom.Resource), Name = "Reverse Signal Enabled", GroupName = "Execution", Order = 4)]
//        public bool DoReverse
//        {
//            get { return pDoReverse; }
//            set { pDoReverse = value; }
//        }	


		
		
	
		private string pEH = "UniqueEntries";
//		[Description("All - submit an order on every arrow. Unique - submit an order only when the account is flat or when a position will be reversed. Flat - submit an order only when the account is flat.")]
//		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Execution", Name = "Entry Handling", Description = "",  Order = 7)]
//		//[RefreshProperties(RefreshProperties.All)]
//		[TypeConverter(typeof(EntryHandling))]
//		public string EH
//		{
//			get { return pEH; }
//			set { pEH = value; }
//		}		
		
		private string pThisEntryType = "Market";
//		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Execution", Name = "Entry Type", Description = "",  Order = 8)]
//		[Description("")]
//		//[RefreshProperties(RefreshProperties.All)]
//		[TypeConverter(typeof(TotalMode))]
//		public string ThisEntryType
//		{
//			get { return pThisEntryType; }
//			set { pThisEntryType = value; }
//		}		
			
		
		
	    private int pLimitOrderOffset = 0;
//        [Range(0, int.MaxValue)]
//        [Display(ResourceType = typeof(Custom.Resource), Name = "Limit Offset (Ticks)", GroupName = "Execution", Order = 12)]
//		[Description("number of ticks to offset the limit order in looking for a better fill price.")]
//        public int LimitOrderOffset
//        {
//            get { return pLimitOrderOffset; }
//            set { pLimitOrderOffset = value; }
//        }	
		
		
		
	    private int pLimitOrderBars = 3;
//        [Range(0, int.MaxValue)]
//        [Display(ResourceType = typeof(Custom.Resource), Name = "Limit Cancel (Bars)", GroupName = "Execution", Order = 13)]
//		[Description("automatically cancel a limit entry if it is not filled within a certain number of bars.")]
//        public int LimitOrderBars
//        {
//            get { return pLimitOrderBars; }
//            set { pLimitOrderBars = value; }
//        }
		
		private bool pUseMIT = false;
//        [Display(ResourceType = typeof(Custom.Resource), Name = "Use MIT", GroupName = "Execution", Order = 13)]
//        public bool UseMIT
//        {
//            get { return pUseMIT; }
//            set { pUseMIT = value; }
//        }
  
        private bool pShowText = true;
        //[Display(ResourceType = typeof(Custom.Resource), Name = "------", GroupName = "Parameters", Order = 7)]
        //public bool ShowText
        //{
        //    get { return pShowText; }
        //    set { pShowText = value; }
        //}

		
     private bool pCTButtonsEnabled = false;
			[RefreshProperties(RefreshProperties.All)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", GroupName = "Chart Trader", Order = -1)]
        public bool CTButtonsEnabled
        {
            get { return pCTButtonsEnabled; }
            set { pCTButtonsEnabled = value; }
        }

		
		private Brush pButtonColorOn = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(30,30,30));
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Chart Trader", Name = "On Color", Order = 0)]
		public Brush ButtonColorOn
		{
			get { return pButtonColorOn; } set { pButtonColorOn = value; }
		}
		[Browsable(false)]
		public string ButtonColorOnS
		{
			get { return Serialize.BrushToString(pButtonColorOn); } set { pButtonColorOn = Serialize.StringToBrush(value); }
		}
		
		private Brush pButtonColorOff = Brushes.DimGray;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Chart Trader",  Name = "Off Color", Order = 1)]
		public Brush ButtonColorOff
		{
			get { return pButtonColorOff; } set { pButtonColorOff = value; }
		}
		[Browsable(false)]
		public string ButtonColorOffS
		{
			get { return Serialize.BrushToString(pButtonColorOff); } set { pButtonColorOff = Serialize.StringToBrush(value); }
		}
		
		
		private Brush pButtonColorLong = Brushes.DarkGreen;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Chart Trader", Name = "Long Color", Order = 2)]
		public Brush ButtonColorLong
		{
			get { return pButtonColorLong; } set { pButtonColorLong = value; }
		}
		[Browsable(false)]
		public string ButtonColorLongS
		{
			get { return Serialize.BrushToString(pButtonColorLong); } set { pButtonColorLong = Serialize.StringToBrush(value); }
		}
		
		
		private Brush pButtonColorShort = Brushes.DarkRed;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Chart Trader", Name = "Short Color", Order = 3)]
		public Brush ButtonColorShort
		{
			get { return pButtonColorShort; } set { pButtonColorShort = value; }
		}
		[Browsable(false)]
		public string ButtonColorShortS
		{
			get { return Serialize.BrushToString(pButtonColorShort); } set { pButtonColorShort = Serialize.StringToBrush(value); }
		}		
		
		private Brush pButtonColorOutline = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(60,60,60));
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Chart Trader", Name = "Outline Color", Order = 20)]
		public Brush ButtonColorOutline
		{
			get { return pButtonColorOutline; } set { pButtonColorOutline = value; }
		}
		[Browsable(false)]
		public string ButtonColorOutlineS
		{
			get { return Serialize.BrushToString(pButtonColorOutline); } set { pButtonColorOutline = Serialize.StringToBrush(value); }
		}						
		
        private int pChartTraderSpace = 0;
//        [Range(0, int.MaxValue)]
//        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Chart Trader", Name = "Y Offset (Pixels)", Order = 30)]
//        public int ChartTraderSpace
//        {
//            get { return pChartTraderSpace; }
//            set { pChartTraderSpace = value; }
//        }
		
        private int pNewMarginRight = 80;
        [Range(0, int.MaxValue)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Chart Display", Name = "Right side margin", Order = 30)]
        public int NewMarginRight
        {
            get { return pNewMarginRight; }
            set { pNewMarginRight = value; }
        }
		
    	 private bool pErrorMessagesEnabled = true;
//        [Display(ResourceType = typeof(Custom.Resource), Name = "Error Messages Enabled", GroupName = "Chart Display", Order = 39)]
//		[Description("option to enable or disable standard error messages in your Chart window.")]
//        public bool ErrorMessagesEnabled
//        {
//            get { return pErrorMessagesEnabled; }
//            set { pErrorMessagesEnabled = value; }
//        }
		
		
		
    	 private bool pDisabledOnCloseButton = false;
//        [Display(ResourceType = typeof(Custom.Resource), Name = "Disable On Close Button", GroupName = "Chart Trader", Order = 39)]
//		[Description("option to disable any further automated trades when the 'Close' button is clicked.")]
//        public bool DisabledOnCloseButton
//        {
//            get { return pDisabledOnCloseButton; }
//            set { pDisabledOnCloseButton = value; }
//        }
		
 		private bool pShowReal = true;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Show gross realized PnL when flat", GroupName = "Chart Trader", Order = 40)]
        public bool ShowReal
        {
            get { return pShowReal; }
            set { pShowReal = value; }
        }				

		

		
		private bool pUseSLM = false;
//        [Display(ResourceType = typeof(Custom.Resource), Name = "Use SLM", GroupName = "Orders", Order = 4)]
//        public bool UseSLM
//        {
//            get { return pUseSLM; }
//            set { pUseSLM = value; }
//        }
		

		
        private int pSLOffset = 0;
//        [Range(0, int.MaxValue), NinjaScriptProperty]
//        [Display(ResourceType = typeof(Custom.Resource), Name = "Stop Limit Offset (Ticks)", GroupName = "Orders", Order = 2)]
//        public int SLOffset
//        {
//            get { return pSLOffset; }
//            set { pSLOffset = value; }
//        }
		

		
        private int pCTWidth = 212;
//        [Range(0, int.MaxValue), NinjaScriptProperty]
//        [Display(ResourceType = typeof(Custom.Resource), Name = "Width (Pixels)", GroupName = "ChartTrader", Order = 2)]
//        public int CTWidth
//        {
//            get { return pCTWidth; }
//            set { pCTWidth = value; }
//        }





    
    
        private Brush areaBrush = Brushes.Goldenrod;
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
        [Display(ResourceType = typeof(Custom.Resource), Name = "On Color", GroupName = "Chart Buttons", Order = 2)]
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

        [Range(0, 100)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "On Opacity (%)", GroupName = "Chart Buttons", Order = 3)]
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


       // [Description("number of ticks within a level to trigger an output for Market Analyer. The column will display positive numbers for R levels and negative numbers for S levels.  For example, if the row displays -2 for ES, then it means that ES  is close to the S2 level.")]

		
		
		
		private bool pAudioEnabled = false;
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
		[Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", Description = "", GroupName = "Audio", Order = 0)]
        public bool AudioEnabled
        {
            get { return pAudioEnabled; }
            set { pAudioEnabled = value; }
        }	
		
		private bool pQuickAudio = true;
//		[Display(ResourceType = typeof(Custom.Resource), Name = "IntraBar", GroupName = "Audio", Order = 1)]
//        public bool QuickAudio
//        {
//            get { return pQuickAudio; }
//            set { pQuickAudio = value; }
//        }
		
		
		
	private string pWAVFileName = "Alert2.wav";
		[Display(ResourceType = typeof(Custom.Resource), Name = "WAV Buy Signal", Description = "Sound file to play when a buy signal occurs.", GroupName = "Audio", Order = 2)]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(LoadFileList))]
		public string WAVFileName
		{
			get { return pWAVFileName; }
			set { pWAVFileName = value; }
		}

		private string pWAVFileName2 = "Alert2.wav";
		[Display(ResourceType = typeof(Custom.Resource), Name = "WAV Sell Signal", Description = "Sound file to play when a sell signal occurs.", GroupName = "Audio", Order = 3)]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(LoadFileList))]
		public string WAVFileName2
		{
			get { return pWAVFileName2; }
			set { pWAVFileName2 = value; }
		}
		
		
		
		
		internal class LoadFileList : StringConverter
		{
			#region LoadFileList
			public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
			{
				//true means show a combobox
				return true;
			}

			public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
			{
				//true will limit to list. false will show the list, 
				//but allow free-form entry
				return false;
			}

			public override System.ComponentModel.TypeConverter.StandardValuesCollection
				GetStandardValues(ITypeDescriptorContext context)
			{
				string folder = System.IO.Path.Combine(Core.Globals.InstallDir,"sounds");
				string search = "*.wav";
				System.IO.DirectoryInfo dirCustom = new System.IO.DirectoryInfo(folder);
				string[] filteredlist = new string[1];
				if(!dirCustom.Exists) {
					filteredlist[0]= "unavailable";
					return new StandardValuesCollection(filteredlist);;
				}
				System.IO.FileInfo[] filCustom = dirCustom.GetFiles(search);

				string[] list = new string[filCustom.Length];
				int i = 0;
				foreach (System.IO.FileInfo fi in filCustom)
				{
					list[i] = fi.Name;
					i++;
				}
				filteredlist = new string[i];
				for(i = 0; i<filteredlist.Length; i++) filteredlist[i] = list[i];
				return new StandardValuesCollection(filteredlist);
			}
			#endregion
		}      
		
		
		
		
		
		// ARROW INPUTS

        private bool pArrowsEnabled = true;
				[RefreshProperties(RefreshProperties.All)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", Description = "", GroupName = "Arrows", Order = 0)]
        public bool ArrowsEnabled
        {
            get { return pArrowsEnabled; }
            set { pArrowsEnabled = value; }
        }
		
 		
        private float pArrowSize = 9;
        [Range(0, 1000)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Structure - Arrow Size", Description = "", GroupName = "Arrows", Order = 1)]
        public float ArrowSize
        {
            get { return pArrowSize; }
            set { pArrowSize = value; }
        }
		
        private float pArrowBarHeight = 18;
        [Range(0, 1000)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Structure - Bar Height", Description = "", GroupName = "Arrows", Order = 2)]
        public float ArrowBarHeight
        {
            get { return pArrowBarHeight; }
            set { pArrowBarHeight = value; }
        }
		
        private float pArrowBarWidth = 3;
        [Range(0, 1000)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Structure - Bar Width", Description = "", GroupName = "Arrows", Order = 3)]
        public float ArrowBarWidth
        {
            get { return pArrowBarWidth; }
            set { pArrowBarWidth = value; }
        }
		
		
		private float pArrowOffset = 20;
        [Range(0, 1000)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Offset (Pixels)", Description = "", GroupName = "Arrows", Order = 4)]
        public float ArrowOffset
        {
            get { return pArrowOffset; }
            set { pArrowOffset = value; }
        }


		


		private Brush pArrowUpFBrush	= Brushes.Green;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Buy Fill Color", Description = "", GroupName = "Arrows", Order = 20)]
		public Brush ArrowUpFBrush
		{
			get { return pArrowUpFBrush; } set { pArrowUpFBrush = value; }
		}
		[Browsable(false)]
		public string ArrowUpFBrushS
		{
			get { return Serialize.BrushToString(pArrowUpFBrush); } set { pArrowUpFBrush = Serialize.StringToBrush(value); }
		}	

		private Brush pArrowDownFBrush	= Brushes.Red;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Sell Fill Color", Description = "", GroupName = "Arrows", Order = 22)]
		public Brush ArrowDownFBrush
		{
			get { return pArrowDownFBrush; } set { pArrowDownFBrush = value; }
		}
		[Browsable(false)]
		public string ArrowDownFBrushS
		{
			get { return Serialize.BrushToString(pArrowDownFBrush); } set { pArrowDownFBrush = Serialize.StringToBrush(value); }
		}			

        private Stroke pArrowUpStroke = new Stroke(Brushes.DarkGreen, DashStyleHelper.Solid, 1);
        [Display(ResourceType = typeof(Custom.Resource), Name = "Buy Outline Color", Description = "", GroupName = "Arrows", Order = 21)]
        public Stroke ArrowUpStroke
        {
            get { return pArrowUpStroke; }
            set { pArrowUpStroke = value; }
        }

        private Stroke pArrowDownStroke = new Stroke(Brushes.DarkRed, DashStyleHelper.Solid, 1);
        [Display(ResourceType = typeof(Custom.Resource), Name = "Sell Outline Color", Description = "", GroupName = "Arrows", Order = 23)]
        public Stroke ArrowDownStroke
        {
            get { return pArrowDownStroke; }
            set { pArrowDownStroke = value; }
        }
		
//		private Brush pArrowUpOBrush	= Brushes.DarkGreen;
//		[XmlIgnore]
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Color Up (Outline)", Desciption = "", GroupName = "Arrows", Order = 20)]
//		public Brush ArrowUpOBrush
//		{
//			get { return pArrowUpOBrush; } set { pArrowUpOBrush = value; }
//		}
//		[Browsable(false)]
//		public string ArrowUpOBrushS
//		{
//			get { return Serialize.BrushToString(pArrowUpOBrush); } set { pArrowUpOBrush = Serialize.StringToBrush(value); }
//		}	

//		private Brush pArrowDownOBrush	= Brushes.DarkRed;
//		[XmlIgnore]
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Color Down (Outline)", Desciption = "", GroupName = "Arrows", Order = 20)]
//		public Brush ArrowDownOBrush
//		{
//			get { return pArrowDownOBrush; } set { pArrowDownOBrush = value; }
//		}
//		[Browsable(false)]
//		public string ArrowDownOBrushS
//		{
//			get { return Serialize.BrushToString(pArrowDownOBrush); } set { pArrowDownOBrush = Serialize.StringToBrush(value); }
//		}	

		
		// LABELS		
		
        private bool pLabelsEnabled = true;
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        [Display(ResourceType = typeof(Custom.Resource), Name = "Labels Enabled", Description = "", GroupName = "Arrows", Order = 51)]
        public bool LabelsEnabled
        {
            get { return pLabelsEnabled; }
            set { pLabelsEnabled = value; }
        }			
		
		private SimpleFont pTextFont = new SimpleFont("Arial", 11);
		[Display(ResourceType = typeof(Custom.Resource), Name = "Labels Font", Description = "", GroupName = "Arrows", Order = 52)]
		public SimpleFont TextFont4
        {
            get { return pTextFont; }
            set { pTextFont = value; }
        }	
		
		
		private string pLabelBuy = "Buy";	
		[Display(ResourceType = typeof(Custom.Resource), Name = "Buy Label", Description = "", GroupName = "Arrows", Order = 53)]
        public string LabelBuy
        {
            get { return pLabelBuy; }
            set { pLabelBuy = value; }
        }		
		
		private string pLabelSell = "Sell";
		[Display(ResourceType = typeof(Custom.Resource), Name = "Sell Label", Description = "", GroupName = "Arrows", Order = 54)]
        public string LabelSell
        {
            get { return pLabelSell; }
            set { pLabelSell = value; }
        }	

		// BACKGROUND
		
        private bool pBackEnabled = true;
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        [Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", Description = "", GroupName = "Background Color", Order = 1)]
        public bool BackEnabled
        {
            get { return pBackEnabled; }
            set { pBackEnabled = value; }
        }	
		
        private bool pColorAll = true;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Color All", Description = "", GroupName = "Background Color", Order = 2)]
        public bool ColorAll
        {
            get { return pColorAll; }
            set { pColorAll = value; }
        }		
		
        private bool pBackSEnabled = true;
	//	[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        [Display(ResourceType = typeof(Custom.Resource), Name = "Signal Enabled", Description = "", GroupName = "Background Color", Order = 10)]
        public bool BackSEnabled
        {
            get { return pBackSEnabled; }
            set { pBackSEnabled = value; }
        }	
		
        private bool pBackTEnabled = true;
	//	[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        [Display(ResourceType = typeof(Custom.Resource), Name = "Trend Enabled", Description = "", GroupName = "Background Color", Order = 20)]
        public bool BackTEnabled
        {
            get { return pBackTEnabled; }
            set { pBackTEnabled = value; }
        }	
		
        private bool pBackMEnabled = true;
	//	[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        [Display(ResourceType = typeof(Custom.Resource), Name = "Time Filter Enabled", Description = "", GroupName = "Background Color", Order = 30)]
        public bool BackMEnabled
        {
            get { return pBackMEnabled; }
            set { pBackMEnabled = value; }
        }	
		
		// BUY COLOR
		
		private System.Windows.Media.Brush	pBrush01 = Brushes.Green;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Buy Signal Color", Description = "", GroupName = "Background Color", Order = 11)]
		public Brush Brush01
		{
			get { return pBrush01; }
			set
			{
				pBrush01 = value;
				if (pBrush01 != null)
				{
					if (pBrush01.IsFrozen)
						pBrush01 = pBrush01.Clone();
					pBrush01.Opacity = pOpacity01 / 100d;
					pBrush01.Freeze();
				}
			}
		}

		[Browsable(false)]
		public string Brush01S
		{
			get { return Serialize.BrushToString(Brush01); }
			set { Brush01 = Serialize.StringToBrush(value); }
		}

		private int	pOpacity01 = 40;
		[Range(0, 100)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Buy Signal Opacity (%)", Description = "", GroupName = "Background Color", Order = 12)]
		public int Opacity01
		{
			get { return pOpacity01; }
			set
			{
				pOpacity01 = Math.Max(0, Math.Min(100, value));
				if (pBrush01 != null)
				{
					System.Windows.Media.Brush newBrush		= pBrush01.Clone();
					newBrush.Opacity	= pOpacity01 / 100d;
					newBrush.Freeze();
					pBrush01			= newBrush;
				}
			}
		}
		
		// SELL COLOR
		
		private System.Windows.Media.Brush	pBrush02 = Brushes.Red;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Sell Signal Color", Description = "", GroupName = "Background Color", Order = 13)]
		public Brush Brush02
		{
			get { return pBrush02; }
			set
			{
				pBrush02 = value;
				if (pBrush02 != null)
				{
					if (pBrush02.IsFrozen)
						pBrush02 = pBrush02.Clone();
					pBrush02.Opacity = pOpacity02 / 100d;
					pBrush02.Freeze();
				}
			}
		}

		[Browsable(false)]
		public string Brush02S
		{
			get { return Serialize.BrushToString(Brush02); }
			set { Brush02 = Serialize.StringToBrush(value); }
		}

		private int	pOpacity02 = 40;
		[Range(0, 100)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Sell Signal Opacity (%)", Description = "", GroupName = "Background Color", Order = 14)]
		public int Opacity02
		{
			get { return pOpacity02; }
			set
			{
				pOpacity02 = Math.Max(0, Math.Min(100, value));
				if (pBrush02 != null)
				{
					System.Windows.Media.Brush newBrush		= pBrush02.Clone();
					newBrush.Opacity	= pOpacity02 / 100d;
					newBrush.Freeze();
					pBrush02			= newBrush;
				}
			}
		}	
		
		

		
		// SESSION 7 COLOR
		
		private System.Windows.Media.Brush	pBrush07 = Brushes.Green;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Long Trend Color", Description = "", GroupName = "Background Color", Order = 21)]
		public Brush Brush07
		{
			get { return pBrush07; }
			set
			{
				pBrush07 = value;
				if (pBrush07 != null)
				{
					if (pBrush07.IsFrozen)
						pBrush07 = pBrush07.Clone();
					pBrush07.Opacity = pOpacity07 / 100d;
					pBrush07.Freeze();
				}
			}
		}

		[Browsable(false)]
		public string Brush07S
		{
			get { return Serialize.BrushToString(Brush07); }
			set { Brush07 = Serialize.StringToBrush(value); }
		}

		private int	pOpacity07 = 10;
		[Range(0, 100)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Long Trend Opacity (%)", Description = "", GroupName = "Background Color", Order = 22)]
		public int Opacity07
		{
			get { return pOpacity07; }
			set
			{
				pOpacity07 = Math.Max(0, Math.Min(100, value));
				if (pBrush07 != null)
				{
					System.Windows.Media.Brush newBrush		= pBrush07.Clone();
					newBrush.Opacity	= pOpacity07 / 100d;
					newBrush.Freeze();
					pBrush07			= newBrush;
				}
			}
		}		
		
		// SESSION 8 COLOR
		
		private System.Windows.Media.Brush	pBrush08 = Brushes.Red;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Short Trend Color", Description = "", GroupName = "Background Color", Order = 23)]
		public Brush Brush08
		{
			get { return pBrush08; }
			set
			{
				pBrush08 = value;
				if (pBrush08 != null)
				{
					if (pBrush08.IsFrozen)
						pBrush08 = pBrush08.Clone();
					pBrush08.Opacity = pOpacity08 / 100d;
					pBrush08.Freeze();
				}
			}
		}

		[Browsable(false)]
		public string Brush08S
		{
			get { return Serialize.BrushToString(Brush08); }
			set { Brush08 = Serialize.StringToBrush(value); }
		}

		private int	pOpacity08 = 10;
		[Range(0, 100)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Short Trend Opacity (%)", Description = "", GroupName = "Background Color", Order = 24)]
		public int Opacity08
		{
			get { return pOpacity08; }
			set
			{
				pOpacity08 = Math.Max(0, Math.Min(100, value));
				if (pBrush08 != null)
				{
					System.Windows.Media.Brush newBrush		= pBrush08.Clone();
					newBrush.Opacity	= pOpacity08 / 100d;
					newBrush.Freeze();
					pBrush08			= newBrush;
				}
			}
		}		
		
		// SESSION 9 COLOR

		private System.Windows.Media.Brush	pBrush09 = Brushes.Navy;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Time Filter Color", Description = "", GroupName = "Background Color", Order = 31)]
		public Brush Brush09
		{
			get { return pBrush09; }
			set
			{
				pBrush09 = value;
				if (pBrush09 != null)
				{
					if (pBrush09.IsFrozen)
						pBrush09 = pBrush09.Clone();
					pBrush09.Opacity = pOpacity09 / 100d;
					pBrush09.Freeze();
				}
			}
		}

		[Browsable(false)]
		public string Brush09S
		{
			get { return Serialize.BrushToString(Brush09); }
			set { Brush09 = Serialize.StringToBrush(value); }
		}

		private int	pOpacity09 = 10;
		[Range(0, 100)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Time Filter Opacity (%)", Description = "", GroupName = "Background Color", Order = 32)]
		public int Opacity09
		{
			get { return pOpacity09; }
			set
			{
				pOpacity09 = Math.Max(0, Math.Min(100, value));
				if (pBrush09 != null)
				{
					System.Windows.Media.Brush newBrush		= pBrush09.Clone();
					newBrush.Opacity	= pOpacity09 / 100d;
					newBrush.Freeze();
					pBrush09			= newBrush;
				}
			}
		}		
		
		// SESSION 10 COLOR
		
//		private System.Windows.Media.Brush	pBrush10 = Brushes.Orange;
//		[XmlIgnore]
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Color", Description = "", GroupName = "Session 10", Order = 4)]
//		public Brush Brush10
//		{
//			get { return pBrush10; }
//			set
//			{
//				pBrush10 = value;
//				if (pBrush10 != null)
//				{
//					if (pBrush10.IsFrozen)
//						pBrush10 = pBrush10.Clone();
//					pBrush10.Opacity = pOpacity10 / 100d;
//					pBrush10.Freeze();
//				}
//			}
//		}

//		[Browsable(false)]
//		public string Brush10S
//		{
//			get { return Serialize.BrushToString(Brush10); }
//			set { Brush10 = Serialize.StringToBrush(value); }
//		}

//		private int	pOpacity10 = 10;
//		[Range(0, 100)]
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Opacity (%)", Description = "", GroupName = "Session 10", Order = 5)]
//		public int Opacity10
//		{
//			get { return pOpacity10; }
//			set
//			{
//				pOpacity10 = Math.Max(0, Math.Min(100, value));
//				if (pBrush10 != null)
//				{
//					System.Windows.Media.Brush newBrush		= pBrush10.Clone();
//					newBrush.Opacity	= pOpacity10 / 100d;
//					newBrush.Freeze();
//					pBrush10			= newBrush;
//				}
//			}
//		}		
		
		
		
		
		
		
		
		
		
		
		
		
		// EMAIL

		
//		private bool pEmailEnabled = false;
//        [Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", GroupName = "Email", Order = 1)]
//        public bool EmailEnabled
//        {
//            get { return pEmailEnabled; }
//            set { pEmailEnabled = value; }
//        }	
		
//		private bool pQuickEmail = true;
//		[Display(ResourceType = typeof(Custom.Resource), Name = "IntraBar", GroupName = "Email", Order = 2)]
//        public bool QuickEmail
//        {
//            get { return pQuickEmail; }
//            set { pQuickEmail = value; }
//        }
		
		
//		private string pEmailAddress = @"";
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Email Address", GroupName = "Email", Order = 3)]
//        public string EmailAddress
//        {
//            get { return pEmailAddress; }
//            set { pEmailAddress = value; }
//        }
		
		
		
	
		private bool pVisualEnabled = true;
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        [Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", GroupName = "Trigger Line", Order = 3)]
        public bool VisualEnabled
        {
            get { return pVisualEnabled; }
            set { pVisualEnabled = value; }
        }
				
		
		private Brush pPlotUpFBrush	= Brushes.Lime;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Up Color", Description = "", GroupName = "Trigger Line", Order = 20)]
		public Brush PlotUpFBrush
		{
			get { return pPlotUpFBrush; } set { pPlotUpFBrush = value; }
		}
		[Browsable(false)]
		public string PlotUpFBrushS
		{
			get { return Serialize.BrushToString(pPlotUpFBrush); } set { pPlotUpFBrush = Serialize.StringToBrush(value); }
		}	

		private Brush pPlotDownFBrush	= Brushes.Red;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Down Color", Description = "", GroupName = "Trigger Line", Order = 22)]
		public Brush PlotDownFBrush
		{
			get { return pPlotDownFBrush; } set { pPlotDownFBrush = value; }
		}
		[Browsable(false)]
		public string PlotDownFBrushS
		{
			get { return Serialize.BrushToString(pPlotDownFBrush); } set { pPlotDownFBrush = Serialize.StringToBrush(value); }
		}	

		
		private int pWidth1 = 2;
        [Range(1, int.MaxValue)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Width", GroupName = "Trigger Line", Order = 30)]
        public int Width1
        {
            get { return pWidth1; }
            set { pWidth1 = value; }
        }						
		
		
	
		private bool pActiveDisplayEnabled = false;
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Chart Active Display", Name = "Enabled", Order = 0)]
        public bool ActiveDisplayEnabled
        {
            get { return pActiveDisplayEnabled; }
            set { pActiveDisplayEnabled = value; }
        }	
		

		
		private SimpleFont pTextFont8 = new SimpleFont("Arial", 12);
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Chart Active Display", Name = "Message Font", Description = "", Order = 1)]
		public SimpleFont TextFont8
        {
            get { return pTextFont8; }
            set { pTextFont8= value; }
        }	
//								
		
		
		
		private Stroke pOrderBothOutlineStroke = new Stroke(Brushes.Gray, DashStyleHelper.Solid, 3);
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Chart Active Display", Name = "Main Display", Order = 60)]
        public Stroke OrderBothOutlineStroke
        {
            get { return pOrderBothOutlineStroke; }
            set { pOrderBothOutlineStroke = value; }
        }					
		
				
  
		private Stroke pOrderUpOutlineStroke = new Stroke(Brushes.Lime, DashStyleHelper.Solid, 3);
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Chart Active Display", Name = "Buy Display", Order = 61)]
        public Stroke OrderUpOutlineStroke
        {
            get { return pOrderUpOutlineStroke; }
            set { pOrderUpOutlineStroke = value; }
        }		
		
        private Stroke pOrderDnOutlineStroke = new Stroke(Brushes.Red, DashStyleHelper.Solid, 3);
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Chart Active Display", Name = "Sell Display ", Order = 62)]
        public Stroke OrderDnOutlineStroke
        {
            get { return pOrderDnOutlineStroke; }
            set { pOrderDnOutlineStroke = value; }
        }		
		

		private bool pActiveOutlineEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Chart Active Display", Name = "Outline Enabled", Order = 100)]
        public bool ActiveOutlineEnabled
        {
            get { return pActiveOutlineEnabled; }
            set { pActiveOutlineEnabled = value; }
        }	
		
		
		
		
  
		private bool pDGEnabled = false;
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
		[Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", GroupName = "Daily Goal & Loss", Order = 0)]
        public bool DGEnabled
        {
            get { return pDGEnabled; }
            set { pDGEnabled = value; }
        }
		
        private double pCommish = 2.42;
//        [Range(0, double.MaxValue), NinjaScriptProperty]
//        [Display(ResourceType = typeof(Custom.Resource), Name = "Commission ($)", GroupName = "Daily Goal & Loss", Order = 1)]
//        public double Commish
//        {
//            get { return pCommish; }
//            set { pCommish = value; }
//        }
		
        private bool pIncludeCommish = false;
//        [Display(ResourceType = typeof(Custom.Resource), Name = "Commission Included", GroupName = "Daily Goal & Loss", Order = 2)]
//        public bool IncludeCommish
//        {
//            get { return pIncludeCommish; }
//            set { pIncludeCommish = value; }
//        }

   


        private double pDG = 500;
        [Range(double.MinValue, double.MaxValue)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Daily Goal ($)", GroupName = "Daily Goal & Loss", Order = 3)]
        public double DG
        {
            get { return pDG; }
            set { pDG = value; }
        }
		
        private double pDL = 500;
        [Range(double.MinValue, double.MaxValue)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Daily Loss ($)", GroupName = "Daily Goal & Loss", Order = 4)]
        public double DL
        {
            get { return pDL; }
            set { pDL = value; }
        }
		
	
		private bool pDGEnabledDisable = true;
		
		[Description("Disable the system immediately when a position is closed and the Daily Goal or Daily Loss has been exceeded.  You can still turn the system back on after this happens.")]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Disable System Automatically", GroupName = "Daily Goal & Loss", Order = 10)]
        public bool DGEnabledDisable
        {
            get { return pDGEnabledDisable; }
            set { pDGEnabledDisable = value; }
        }
		
		
		
        private bool pDailyPNLTrailingEnabled = false;
//        [Display(ResourceType = typeof(Custom.Resource), Name = "Trailing Enabled", GroupName = "Daily Goal & Loss", Order = 5, Description = "automatically trail the Daily Loss ($) amount as daily profit increases.")]
//        public bool DailyPNLTrailingEnabled
//        {
//            get { return pDailyPNLTrailingEnabled; }
//            set { pDailyPNLTrailingEnabled = value; }
//        }
		
        private double pDLTrailingAmount = 500;
//        [Range(double.MinValue, double.MaxValue), NinjaScriptProperty]
//        [Display(ResourceType = typeof(Custom.Resource), Name = "Trailing ($)", GroupName = "Daily Goal & Loss", Order = 6, Description = "reset Daily Loss ($) on first bar of session to this amount in dollars.")]
//        public double DLTrailingAmount
//        {
//            get { return pDLTrailingAmount; }
//            set { pDLTrailingAmount = value; }
//        }	
		
        private double pDLDefault = -200;
//        [Range(double.MinValue, double.MaxValue), NinjaScriptProperty]
//        [Display(ResourceType = typeof(Custom.Resource), Name = "Daily Loss ($) Default", GroupName = "Daily Goal & Loss", Order = 7, Description = "reset Daily Loss ($) on first bar of session to this amount in dollars.")]
//        public double DLDefault
//        {
//            get { return pDLDefault; }
//            set { pDLDefault = value; }
//        }		
		
		

//		private bool pLabelsEnabled = true;
//        [Display(ResourceType = typeof(Custom.Resource), Name = "Labels Enabled", Description = "", GroupName = "Display", Order = 1)]
//        public bool LabelsEnabled
//        {
//            get { return pLabelsEnabled; }
//            set { pLabelsEnabled = value; }
//        }		
		
//		[Display(ResourceType = typeof(Custom.Resource), Name="Label Font", Description="", GroupName="Display", Order = 2)]
//		public SimpleFont TextFont
//		{ get; set; }	
		
		private int pRightPX = 0;
//		[Range(0, 1000)]
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Label X Offset", Description="in pixels.", GroupName = "Display", Order = 3)]
//		public int RightPX
//		{
//			get { return pRightPX; }
//			set { pRightPX= value; }
//		}	
		
		private int pShadowWidth = 0;
//		[Range(0, 100)]
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Shadow Width", Description="in pixels.", GroupName = "Display", Order = 5)]
//		public int ShadowWidth
//		{
//			get { return pShadowWidth; }
//			set { pShadowWidth= value; }
//		}	
		
		
		private bool pIsLifeTime = true;
		
		[Description("")]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Disable System Automatically", GroupName = "Daily Goal & Loss", Order = 10)]
        public bool IsLifeTime
        {
            get { return pIsLifeTime; }
            set { pIsLifeTime = value; }
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
	public class aiSIGFirstTouchConverter : NinjaTrader.NinjaScript.IndicatorBaseConverter
	{
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) { return true; }

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			PropertyDescriptorCollection propertyDescriptorCollection = base.GetPropertiesSupported(context) ? base.GetProperties(context, value, attributes) : TypeDescriptor.GetProperties(value, attributes);

			aiSIGFirstTouch   jbb = (aiSIGFirstTouch) value;
			
			//Pivots						thisPivotsInstance			= (Pivots) value;
			
			//bool MagnetsOn = ;
			
			List<string> DeleteThese = new List<string>();
			List<string> DeleteThese2 = new List<string>();
			
			
	
			
			
			DeleteThese.Add("IsLifeTime");
			
				DeleteThese.Add("EntriesEnabled");
				DeleteThese.Add("LongEnabled");
				DeleteThese.Add("ShortEnabled");
			DeleteThese.Add("TrendOnlyEnabled");	
			
				DeleteThese.Add("AutoEnabled");	
			
			DeleteThese.Add("SLTrailOrdersEnabled");	
			DeleteThese.Add("ExitOrdersEnabled");	
			
			
				
		
	if (!jbb.UseTimeFilter)
			{			
				DeleteThese.Add("StartT");
				DeleteThese.Add("EndT");
				DeleteThese.Add("StartT2");
				DeleteThese.Add("EndT2");
				DeleteThese.Add("StartT3");
				DeleteThese.Add("EndT3");
								
				
		
		
		
			}			
			
			
		
	
			if (!jbb.VisualEnabled)
			{			
				DeleteThese.Add("PlotUpFBrush");
				DeleteThese.Add("PlotDownFBrush");
				DeleteThese.Add("Width1");
		
		
			}		
			
	
			DeleteThese.Add("CuEnabled");
			
			if (!jbb.IsLifeTime)
				DeleteThese.Add("CuEnabled");
				
			if (!jbb.CuEnabled)
			{			
				DeleteThese.Add("ThisCumulativeDeltaType");
				DeleteThese.Add("ThisCumulativeDeltaPeriod");
				DeleteThese.Add("CumulativeSizeFilter");
		
				DeleteThese.Add("HUDEnabled");
			}				
			
			 
		    if (!jbb.CuEnabled || !jbb.HUDEnabled)
			{	
				DeleteThese.Add("HUD1");
				DeleteThese.Add("HUD2");
				DeleteThese.Add("HUD3");
				DeleteThese.Add("HUD4");
				DeleteThese.Add("HUD5");
				DeleteThese.Add("HUDVOLColor");
				DeleteThese.Add("HUDNEColor");
				DeleteThese.Add("HUDUPColor");
				DeleteThese.Add("HUDDNColor");
				DeleteThese.Add("NetDMode2");
				DeleteThese.Add("HUDMinOpacity");
				DeleteThese.Add("HUDDefaultOpacity");
				DeleteThese.Add("HUDMaxOpacity");		
				DeleteThese.Add("TextFont2");		
				
			}				
       	
	
			

			
				DeleteThese.Add("Feed1Included");
				DeleteThese.Add("Feed2Included");
				DeleteThese.Add("Feed3Included");			
				DeleteThese.Add("Feed4Included");	
			
			 
		    if (!jbb.SecondaryFeedsEnabled)
			{	
				DeleteThese.Add("EMAPeriod1");
				DeleteThese.Add("EMAPeriod2");
				DeleteThese.Add("EMAPeriod3");
				DeleteThese.Add("EMAPeriod4");
				
				DeleteThese.Add("ThisBarType1");
				DeleteThese.Add("ThisBarType2");
				DeleteThese.Add("ThisBarType3");
				DeleteThese.Add("ThisBarType4");				
				
				DeleteThese.Add("Feed1Enabled");
				DeleteThese.Add("Feed2Enabled");
				DeleteThese.Add("Feed3Enabled");			
				DeleteThese.Add("Feed4Enabled");	
				
				DeleteThese.Add("SecondaryFeedsDisplayEnabled");
				
			}	
			
			
		    if (!jbb.Feed1Enabled)
			{				
				DeleteThese.Add("EMAPeriod1");
				DeleteThese.Add("ThisBarType1");
			}
			
		    if (!jbb.Feed2Enabled)
			{				
				DeleteThese.Add("EMAPeriod2");
				DeleteThese.Add("ThisBarType2");
			}
			
		    if (!jbb.Feed3Enabled)
			{				
				DeleteThese.Add("EMAPeriod3");
				DeleteThese.Add("ThisBarType3");
			}	
			
		    if (!jbb.Feed4Enabled)
			{				
				DeleteThese.Add("EMAPeriod4");
				DeleteThese.Add("ThisBarType4");
			}
			
		    if (!jbb.SecondaryFeedsEnabled || !jbb.SecondaryFeedsDisplayEnabled)
			{	
				DeleteThese.Add("TextFont");
				DeleteThese.Add("ColorTextBrush");
				DeleteThese.Add("TPosition");
				DeleteThese.Add("PixelsFromRight");
				DeleteThese.Add("PixelsFromBottom");
				DeleteThese.Add("MarginB");
				DeleteThese.Add("FillUpBrush");
				DeleteThese.Add("FillNeutralBrush");
				DeleteThese.Add("FillDownBrush");
				DeleteThese.Add("FillOpacity");
				DeleteThese.Add("OutlineOpacity");
			
			}				
			
		
			
		
	

		
			DeleteThese.Add("DGEnabled");
			
		    if (!jbb.DGEnabled)
			{	

				DeleteThese.Add("DG");
				DeleteThese.Add("DL");
				DeleteThese.Add("DGEnabledDisable");
				
			}				
			
			
			DeleteThese.Add("ActiveDisplayEnabled");
			
		    if (!jbb.ActiveDisplayEnabled)
			{	

				DeleteThese.Add("TextFont8");
				DeleteThese.Add("ActiveOutlineEnabled");
				DeleteThese.Add("OrderBothOutlineStroke");
				DeleteThese.Add("OrderUpOutlineStroke");				
				DeleteThese.Add("OrderDnOutlineStroke");	
			}				
			
			
			DeleteThese.Add("CTButtonsEnabled");
			
		    if (!jbb.CTButtonsEnabled)
			{	

				DeleteThese.Add("TextFont8");
				DeleteThese.Add("ButtonColorOn");
				DeleteThese.Add("ButtonColorOff");
				DeleteThese.Add("ButtonColorLong");				
				DeleteThese.Add("ButtonColorShort");	
				DeleteThese.Add("ButtonColorOutline");
				DeleteThese.Add("ShowReal");
				DeleteThese.Add("DisabledOnCloseButton");
				
				
				
			}				
			
			
			
		

				DeleteThese.Add("RunnerTrailEnabled");	
				DeleteThese.Add("SLTrailEnabled");	
				DeleteThese.Add("PlotDownFBrush33");
				DeleteThese.Add("Width5");		
	

			if (!jbb.ExitOEnabled)
			{			

				
				
				DeleteThese.Add("ExitHOEnabled");
//				DeleteThese.Add("ATRPeriod");
//				DeleteThese.Add("ATRStopM");
//				DeleteThese.Add("ATRTargetM");
//				DeleteThese.Add("ATRTargetM2");
//				DeleteThese.Add("PT1Enabled");
//				DeleteThese.Add("PT2Enabled");
				DeleteThese.Add("StopStroke");
				DeleteThese.Add("TargetStroke");
				DeleteThese.Add("LabelsEnabled2");
				DeleteThese.Add("SLTrailEnabled");	
				DeleteThese.Add("PlotDownFBrush33");
				DeleteThese.Add("Width5");
								
				
		
		
		
			}			
						
			
			
		
		
			if (!jbb.ArrowsEnabled)
			{			
				//DeleteThese.Add("MinimumDecliningLevels");
				//DeleteThese.Add("HighLowRangeFilter2");
				//DeleteThese.Add("HighLowRangeFilter");
				DeleteThese.Add("ArrowOffset");
				DeleteThese.Add("ArrowSize");
				DeleteThese.Add("ArrowUpFBrush");
				DeleteThese.Add("ArrowDownFBrush");
				DeleteThese.Add("ArrowUpStroke");
				DeleteThese.Add("ArrowDownStroke");
				DeleteThese.Add("LabelsEnabled");
				DeleteThese.Add("TextFont");
				DeleteThese.Add("LabelBuy");
				DeleteThese.Add("LabelSell");
				
				DeleteThese.Add("ArrowBarHeight");
				DeleteThese.Add("ArrowBarWidth");
								
				DeleteThese.Add("TextFont4");
		
		
		
			}			
						
			
	
		
				DeleteThese.Add("TimerEnabled");
			
		    if (!jbb.TimerEnabled)
			{	
				DeleteThese.Add("TimerOffsetX");
				DeleteThese.Add("TimerOffset");
				DeleteThese.Add("TextFontTime");
				DeleteThese.Add("TimerMainColor");
				DeleteThese.Add("CountDown");
				DeleteThese.Add("ShowPercent");
				DeleteThese.Add("ClockErrorMessagesEnabled");
				
				
				
			}				
			
			//DeleteThese.Add("ButtonsEnabled");
		
		    if (!jbb.ButtonsEnabled)
			{	
				DeleteThese.Add("AreaBrush");
				DeleteThese.Add("AreaOpacity");
				DeleteThese.Add("ButtonSize");
				
			}				
			
			
		
		    if (!jbb.AudioEnabled)
			{	
				DeleteThese.Add("WAVFileName");
				DeleteThese.Add("WAVFileName2");
				
			}				
		


		    if (!jbb.LabelsEnabled)
			{	
				DeleteThese.Add("TextFont");
				DeleteThese.Add("LabelBuy");
				DeleteThese.Add("LabelSell");
				
			}				
			
				
		
		    if (!jbb.BackEnabled)
			{	
				DeleteThese.Add("ColorAll");
				DeleteThese.Add("Brush01");
				DeleteThese.Add("Opacity01");
				DeleteThese.Add("Brush02");
				DeleteThese.Add("Opacity02");	
				DeleteThese.Add("Brush07");
				DeleteThese.Add("Opacity07");
				DeleteThese.Add("Brush08");
				DeleteThese.Add("Opacity08");					
				DeleteThese.Add("Brush09");
				DeleteThese.Add("Opacity09");
							
				
			}				
			
		
	

	
		
			 
			 
			
			DeleteThese.Add("Calculate");
			//DeleteThese.Add("Name");
      		DeleteThese.Add("MaximumBarsLookBack");
			
			DeleteThese.Add("Input");
			
			DeleteThese.Add("IsAutoScale");
			DeleteThese.Add("Displacement");
			//DeleteThese.Add("DisplayInDataBox");
			DeleteThese.Add("Panel");
			//DeleteThese.Add("PaintPriceMarkers");
			DeleteThese.Add("ScaleJustification");
			DeleteThese.Add("IsVisible");
			
			
			
			
			
			
			
			
			
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
			
							
			
	
		
	
				
		
		
		
		
		
			
//      	DeleteThese2.Add("Calculate");
//			DeleteThese2.Add("Label");
//			DeleteThese2.Add("Maximum bars look back");
//			DeleteThese2.Add("Input series");
			

			
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
		private aiSIGFirstTouch[] cacheaiSIGFirstTouch;
		public aiSIGFirstTouch aiSIGFirstTouch(int numberOfBars, int numberOfBars3)
		{
			return aiSIGFirstTouch(Input, numberOfBars, numberOfBars3);
		}

		public aiSIGFirstTouch aiSIGFirstTouch(ISeries<double> input, int numberOfBars, int numberOfBars3)
		{
			if (cacheaiSIGFirstTouch != null)
				for (int idx = 0; idx < cacheaiSIGFirstTouch.Length; idx++)
					if (cacheaiSIGFirstTouch[idx] != null && cacheaiSIGFirstTouch[idx].NumberOfBars == numberOfBars && cacheaiSIGFirstTouch[idx].NumberOfBars3 == numberOfBars3 && cacheaiSIGFirstTouch[idx].EqualsInput(input))
						return cacheaiSIGFirstTouch[idx];
			return CacheIndicator<aiSIGFirstTouch>(new aiSIGFirstTouch(){ NumberOfBars = numberOfBars, NumberOfBars3 = numberOfBars3 }, input, ref cacheaiSIGFirstTouch);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.aiSIGFirstTouch aiSIGFirstTouch(int numberOfBars, int numberOfBars3)
		{
			return indicator.aiSIGFirstTouch(Input, numberOfBars, numberOfBars3);
		}

		public Indicators.aiSIGFirstTouch aiSIGFirstTouch(ISeries<double> input , int numberOfBars, int numberOfBars3)
		{
			return indicator.aiSIGFirstTouch(input, numberOfBars, numberOfBars3);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.aiSIGFirstTouch aiSIGFirstTouch(int numberOfBars, int numberOfBars3)
		{
			return indicator.aiSIGFirstTouch(Input, numberOfBars, numberOfBars3);
		}

		public Indicators.aiSIGFirstTouch aiSIGFirstTouch(ISeries<double> input , int numberOfBars, int numberOfBars3)
		{
			return indicator.aiSIGFirstTouch(input, numberOfBars, numberOfBars3);
		}
	}
}

#endregion
