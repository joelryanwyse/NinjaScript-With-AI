//
// Copyright (C) 2021, Affordable Indicators, Inc. <www.affordableindicators.com>.
// Affordable Indicators, Inc. reserves the right to modify or overwrite this NinjaScript component with each release.
//

using System;
using System.Collections;
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Core;
using NinjaTrader.Data;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.NinjaScript;
using NinjaTrader.Gui.Tools;
using NinjaTrader.NinjaScript;
using NinjaTrader.NinjaScript.DrawingTools;
using NinjaTrader.NinjaScript.Indicators;
using NinjaTrader.Core.FloatingPoint;
using SharpDX.DirectWrite;

//This namespace holds Indicators in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Indicators
{
	    
	[Gui.CategoryOrder("Parameters", 0)]
	[Gui.CategoryOrder("Signals", 1)]
    [Gui.CategoryOrder("Arrows", 2)]
    [Gui.CategoryOrder("Labels", 3)]
    [Gui.CategoryOrder("Background", 4)]
    [Gui.CategoryOrder("Audio", 5)]
    [Gui.CategoryOrder("Email", 6)]
    [Gui.CategoryOrder("Popup", 7)]
	
	
	[Gui.CategoryOrder("Setup", 9000)]
	[Gui.CategoryOrder("License", 10000)]
		
	
	public class aiSIGIchimokuCloud : Indicator
	{
		
		
		private string ThisName = "aiSIGIchimokuCloud";
				


		private string PriceCloudStatus = string.Empty;
		private string AlertText = string.Empty;
		
	
	
		
		
	private Point MP;
      

		private double dpiX = 0;
		private	double dpiY = 0;
		
		

		private double FinalXPixel = 0;
		private	double FinalYPixel = 0;		
					
		
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
		

			


			
		private bool LicenseWordPress (string machineid, string pLicensingEmailAddress)
		{
			pLicensingEmailAddress = pLicensingEmailAddress.Replace(" ", "");
			
			List<int> ThisProductMainIDs = new List<int>();
			List<int> ThisProductSecondaryIDs = new List<int>();
			
			// Product IDs for Main Indicator
			
			ThisProductMainIDs.Add(8820);
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

				
		
		
		
		
		
		
		
		
		
		// replace inputs
		
		private bool Isignal_Strong;
		private bool Isignal_Neutral;
		private bool Isignal_Weak;
		private bool Ilegend;
		
		private int Lag;
		
		
		//	for Ichimoku Signals by kkc2015, see document from IchimokuTrader.com
		private const int icTrendSpan	= 4;	//	4 previous trading days, used for avoiding whipsaw signals
		private const int icNoSignal	= 0;
		private const int icBelowCloud	= 1;	//	signal occurred below the cloud
		private const int icInCloud		= 2;
		private const int icAboveCloud	= 3;
		private const int icSignal_Base	= 11;	//	use for adjusting icSignal_ to start from 0
		private const int icSignal_TK	= icSignal_Base+0;	//	TK = signal for Tenkan / Kijun sen crossed up
		private const int icSignal_PK	= icSignal_Base+1;	//	do not change number sequence from _TK to _CP
		private const int icSignal_KB	= icSignal_Base+2;
		private const int icSignal_SS	= icSignal_Base+3;
		private const int icSignal_CP	= icSignal_Base+4;	//	do not change number sequence from _TK to _CP
		private const int icSignal_LS	= icSignal_Base+5;	//	trade record [Long/Short]
		private const int icSignal_DU	= icSignal_Base+6;	//	Alert [Down / Up trend]
		private const int icSignalType	= 5;	//	exclude icSignal_LS
		private const int icReadTradeData = -2;	//	flag to show that ReadTradeData is completed, do not repeat
		private const int icMACD_data = -3;		//	flag to show that MACD data is completed, do not repeat
		private const int icMACD_Peak = 1;		//	check for MACD peaks
		private const int icMACD_Valley = 2;	//	check for MACD valleys
		private const bool bMACD_Completed = true;	//	used by MACD function
		
		private static readonly int[] icSignals = {icSignal_TK,icSignal_PK,icSignal_KB,icSignal_SS,icSignal_CP};
		private static readonly string[] scSignals = {"TK", "PK", "KB", "SS", "CP", "LS", "DU"};
		
		private const int icSignal_start	= 100;	//	use for adjusting TK_DN to start from 1
		private const int icSignal_TK_DN	= 101;	//	_DN = odd number = signal for Tenkan / Kijun sen crossed down
		private const int icSignal_TK_UP	= 102;	//	_UP = even number = signal for Tenkan / Kijun sen crossed up
		private const int icSignal_PK_DN	= 103;
		private const int icSignal_PK_UP	= 104;
		private const int icSignal_KB_DN	= 105;
		private const int icSignal_KB_UP	= 106;
		private const int icSignal_SS_DN	= 107;
		private const int icSignal_SS_UP	= 108;
		private const int icSignal_CP_DN	= 109;
		private const int icSignal_CP_UP	= 110;
		
		private const int icSignal_Weak		= 1;
		private const int icSignal_Neutral	= 2;
		private const int icSignal_Strong	= 3;
		private const int icSignal_Trade	= 4;
		//	5 Ichimoku signal types
		private int iSignal			= icNoSignal;
		private string sSignalCode	= " ";
		private bool bArrowDn		= false;
		private int iSignalStrength	= icSignal_Neutral;
		private double dChart_Ymin	= 90.0; 	//	Y value (=Price) at the bottom of chart 
		private double dChart_Ymax	= 134.0; 	//	Y value (=Price) at the top of chart
		private double dChart_Y		= 0.0;		//	Y location for showing the arrow
		private	double dChart_Yspan = 100.0;
		private int iNbrSameBar = 0;
		private string sLegend = "SignalColor: LightGray(weak), NoColor(Neutral), Green/Red(strong)\n" +
							"Cross:\tTK - Tenkan / Kijun,  PK - Price / Kijun,  KB - Price / Kumo,\n\tSS - Senkou SpanA / SpanB,  CP - Chikou / Price\n";
							
		NinjaTrader.Gui.Tools.SimpleFont LegendFont = new NinjaTrader.Gui.Tools.SimpleFont("Courier New", 12);
		private Brush ArrowBrush	= Brushes.DarkGray;		//	use for debugging, do not remove
		
		//	for SharpDX drawing of Ichimoku cloud and Indicator ColorBar
		private Brush upAreaBrush	= Brushes.LightGreen;
		private Brush dnAreaBrush	= Brushes.Pink;
		private Brush upLineBrush	= Brushes.DarkGreen;
		private Brush dnLineBrush	= Brushes.Red;
		private Brush textBrush		= Brushes.Black;
		int iareaOpacity = 55;		//	this provides reasonable cloud density, can be changed by user input
		const float fontHeight		= 12f;
		int iX_barWidth = 10;		//	space for each bar, initialize at OnRender()

		private SharpDX.Direct2D1.Brush	upAreaBrushDx, dnAreaBrushDx, upLineBrushDx, dnLineBrushDx, textBrushDx, finalLineBrushDx;
	private SharpDX.Direct2D1.Brush arrowUpBrushDX, arrowDownBrushDX, chartTextBrushDXCached, chartBackgroundBrushDXCached;
	private SharpDX.DirectWrite.TextFormat cachedLabelTextFormat;

	// cached buffers for cloud rendering (reused across frames)
	private int[] cloudIX = new int[0];
	private int[] cloudIA = new int[0];
	private int[] cloudIB = new int[0];
		
		private int iSignalIdx = 0;
		private bool bRendering = false;
		private bool bGetSignal = false;
		private const int icSignalMax = 2000;
		private const int icSignalSort = -999;	//	to indicate that stSignal_all[] requires sorting
		private struct stSignal
		{
			public int iBar;
			public int iSignal;
			public bool bTrendDown;
			public int iStrength;
			public int iNbrSignal;
		};
		//	[0].iBar = total number of signals. [0].iNbrSignal = icReadTradeData, [0].iStrength = icMACD_data
		private stSignal[] stSignal_all = new stSignal[icSignalMax+1];
		
		
		private Series<double> Signal;
		private Series<double> TenkanKijunCross;
		private Series<double> Trend;
		private Series<double> Trend2;
		
		private Series<double> UpperCloud;
		private Series<double> LowerCloud;
		
		private Series<double> SpanALine;
		private Series<double> SpanBLine;		
		
		
		

		private bool RunInit = true;

		private int LastEmailBar = 0;
		private int LastAudioBar = 0;
		private int BB = 0;
		
		private string subject = string.Empty;
		private string message = string.Empty;
		
		private int PriceDigits = 0;
		private string PriceString = string.Empty;
		
        private SharpDX.Vector2 StartPoint = (new Point(0, 0)).ToVector2();
        private SharpDX.Vector2 EndPoint = (new Point(0, 0)).ToVector2();

		private const int FLAT = 0;
		private const int LONG = 1;
		private const int SHORT = -1;

		
		
		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				
				
					
				Name = ThisName;
				Description					= @"Display Ichimoku cloud / indicators.";
			
					
				
				Calculate					= Calculate.OnPriceChange;
				IsOverlay					= true;
				DisplayInDataBox			= true;
				DrawOnPricePanel			= true;
				DrawHorizontalGridLines		= true;
				DrawVerticalGridLines		= true;
				PaintPriceMarkers			= true;
				ScaleJustification			= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				//Disable this property if your indicator requires custom values that cumulate with each new market data event. 
				//See Help Guide for additional information.
				IsSuspendedWhileInactive	= true;
				Conversion					= 9;
				BBase						= 26;
				SpanB						= 52;
				
				Lag							= 26;
				Lag = BBase;
				
				
				Isignal_Strong				= true;
				Isignal_Neutral				= false;
				Isignal_Weak				= false;
				Ilegend						= true;
				
				upAreaBrush 				= Brushes.DarkGreen;
				dnAreaBrush 				= Brushes.DarkRed;
				upLineBrush 				= Brushes.LimeGreen;
				dnLineBrush 				= Brushes.Red;
				iareaOpacity				= 40;
				
				//Tenkan sen = ConversionLine, Kijun sen = BaseLine, Senkou Span A = SpanALine, Senkou Span B = SpanBLine
				
				AddPlot(new Stroke(Brushes.Purple, DashStyleHelper.Solid, 1),	PlotStyle.Line, "Tenkan Sen");
				AddPlot(new Stroke(Brushes.Teal, DashStyleHelper.Solid, 1),	PlotStyle.Line, "Kijun Sen");
				AddPlot(new Stroke(Brushes.SteelBlue, DashStyleHelper.Solid, 1),	PlotStyle.Line, "Chikou Span");
				AddPlot(new Stroke(Brushes.Transparent, DashStyleHelper.Solid, 1),	PlotStyle.Dot, "Signals");
				
//				AddPlot(Brushes.Purple, "Tenkan Sen");	//	Plots[0]
//				AddPlot(Brushes.Teal, "Kijun Sen");			//	Plots[1]
//				//AddPlot(Brushes.Transparent, "SpanALine");	//	Plots[2]
//				//AddPlot(Brushes.Transparent, "SpanBLine");	//	Plots[3]
//				AddPlot(Brushes.SteelBlue, "Chikou Span");	//	Plots[5]
				
				
				
			}
			else if (State == State.Configure)
			{
				
				Signal = new Series<double>(this, MaximumBarsLookBack.Infinite);
				TenkanKijunCross = new Series<double>(this, MaximumBarsLookBack.TwoHundredFiftySix);
				Trend = new Series<double>(this, MaximumBarsLookBack.TwoHundredFiftySix);
				Trend2 = new Series<double>(this, MaximumBarsLookBack.TwoHundredFiftySix);

				UpperCloud = new Series<double>(this, MaximumBarsLookBack.TwoHundredFiftySix);
				LowerCloud = new Series<double>(this, MaximumBarsLookBack.TwoHundredFiftySix);

				SpanALine = new Series<double>(this, MaximumBarsLookBack.Infinite);
				SpanBLine = new Series<double>(this, MaximumBarsLookBack.Infinite);
				
				
				
				
			//	Permission = CheckLicense(NinjaTrader.Cbi.License.MachineId, "");
			
				//Print("IC I");
				
				
				if (!Permission)
					return;
				
				//AddDataSeries(Instrument.FullName, Data.BarsPeriodType.Day, 1, Data.MarketDataType.Last);
				ZOrder = -1;	//	2016.05.24, per ReusableBrushExample
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
							
					
					
					

				Permission = LicenseWordPress(NinjaTrader.Cbi.License.MachineId, pLicensingEmailAddress);	
				
				
				
								
				
				
				
				
							
				if (ChartControl != null)
				if (ChartPanel != null)
                {
					ChartPanel.MouseMove += new MouseEventHandler(OnMouseMove);
                    ChartPanel.MouseDown += new MouseButtonEventHandler(OnMouseDown);
					ChartPanel.MouseLeave += new MouseEventHandler(OnMouseLeave);
                }
				
				
				
				
				
				//	2016.05.24	from Ninjascript ReuseDxBrushesExample.cs
				
				// remove all of this code 7/29/2017 to fix an error, value cannot be null, rendertarget
				
				
//				if (upAreaBrush.IsFrozen)
//					upAreaBrush = upAreaBrush.Clone();		//	this will ensure that previous drawing using this brush is not affected
//				upAreaBrush.Opacity = iareaOpacity / 100d;		//	.Opacity[0..1]
//				upAreaBrush.Freeze();	//	freeze brush so that it can be changed later by other functions		
//				upAreaBrushDx = upAreaBrush.ToDxBrush(RenderTarget);
				
//				if (dnAreaBrush.IsFrozen)
//					dnAreaBrush = dnAreaBrush.Clone();
//				dnAreaBrush.Opacity = iareaOpacity / 100d;
//				dnAreaBrush.Freeze();
//				dnAreaBrushDx = dnAreaBrush.ToDxBrush(RenderTarget);

//				//	the following brushes are not to be changed
//				upLineBrushDx = upLineBrush.ToDxBrush(RenderTarget);		
//				dnLineBrushDx = dnLineBrush.ToDxBrush(RenderTarget);
//				textBrushDx = textBrush.ToDxBrush(RenderTarget);
			}
				else if( State == State.Terminated)
			{
				if (ChartControl != null)
                if (ChartPanel != null)
                {
                    ChartPanel.MouseMove -= new MouseEventHandler(OnMouseMove);
                    ChartPanel.MouseDown -= new MouseButtonEventHandler(OnMouseDown);
					ChartPanel.MouseLeave -= new MouseEventHandler(OnMouseLeave);
                }

				if (upAreaBrushDx != null) { upAreaBrushDx.Dispose(); upAreaBrushDx = null; }
				if (dnAreaBrushDx != null) { dnAreaBrushDx.Dispose(); dnAreaBrushDx = null; }
				if (upLineBrushDx != null) { upLineBrushDx.Dispose(); upLineBrushDx = null; }
				if (dnLineBrushDx != null) { dnLineBrushDx.Dispose(); dnLineBrushDx = null; }
				if (textBrushDx != null) { textBrushDx.Dispose(); textBrushDx = null; }
				if (arrowUpBrushDX != null) { arrowUpBrushDX.Dispose(); arrowUpBrushDX = null; }
				if (arrowDownBrushDX != null) { arrowDownBrushDX.Dispose(); arrowDownBrushDX = null; }
				if (chartTextBrushDXCached != null) { chartTextBrushDXCached.Dispose(); chartTextBrushDXCached = null; }
				if (chartBackgroundBrushDXCached != null) { chartBackgroundBrushDXCached.Dispose(); chartBackgroundBrushDXCached = null; }
				if (cachedLabelTextFormat != null) { cachedLabelTextFormat.Dispose(); cachedLabelTextFormat = null; }
			}
			
		}

		
		

		
				
        internal void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.MP = e.GetPosition(this.ChartPanel);

 
			FinalXPixel = MP.X / 100 * dpiX;
			FinalYPixel = MP.Y / 100 * dpiY;
         
 		
			
			if (AllErrorMessages.Count > 0)
			{
				AllErrorMessages.Clear();
				
				//ChartBarsSwitch2(true);
				ChartControl.InvalidateVisual();
				
				//myProperties.AllowSelectionDragging = PreviousDrag;
				
				return;
				
			}

			
			
		


        }
		

		
		internal void OnMouseLeave(object sender, MouseEventArgs e)
    	{
            this.MP = e.GetPosition(this.ChartPanel);


			FinalXPixel = MP.X / 100 * dpiX;
			FinalYPixel = MP.Y / 100 * dpiY;
			
			//InMenu = false;
			this.ChartControl.InvalidateVisual();
		}
		
		internal void OnMouseMove(object sender, MouseEventArgs e)
    	{
            this.MP = e.GetPosition(this.ChartPanel);


			FinalXPixel = MP.X / 100 * dpiX;
			FinalYPixel = MP.Y / 100 * dpiY;
         
			
			// hover rects
			
//			MouseInRectPre = MouseInRectNow;
//			MouseInRectNow = false;
//			foreach(SharpDX.RectangleF rr in new List<SharpDX.RectangleF>(AllRefreshRects))
//			{
//				if (MouseIn(rr,3,3))
//					MouseInRectNow = true;
				
//			}
			
//			if (MouseInRectNow != MouseInRectPre)
//				this.ChartControl.InvalidateVisual();
			
		
			
			
			
			
			
			
//            foreach (KeyValuePair<double, ButtonZ> thisbutton in AllButtonZ)
//                {
//                    bool hoverednew = MouseIn(thisbutton.Value.Rect, 2, 2);
//                    bool hoverednow = thisbutton.Value.Hovered;

//                    if (hoverednew && !hoverednow)
//                    {
//                        thisbutton.Value.Hovered = true;
//                        this.ChartControl.InvalidateVisual();
//                    }
//                    if (!hoverednew && hoverednow)
//                    {
//                        thisbutton.Value.Hovered = false;
//                        this.ChartControl.InvalidateVisual();
//                    }

//                }

//                InMenuP = InMenu;
//                InMenu = MouseIn(B2, 8, 8);
           
//            if (InMenu != InMenuP)
//                this.ChartControl.InvalidateVisual();

			
			
			//e.Handled = true;
		}
		
		
		
		
		
		
		
		protected override void OnBarUpdate()
		{
				if (!Permission)
					return;
				
				
			
			//Add your custom indicator logic here.
			if(CurrentBar < SpanB)
			{
				iSignalIdx = 0;
				stSignal_all[0].iBar = iSignalIdx;	//	initialize maximum number of bars
				stSignal_all[0].iSignal = icSignalSort;	//	initialize signal to sort the struct
				stSignal_all[0].iNbrSignal = -1;		//	initialize to indicate fresh set of data
				stSignal_all[0].iStrength = -1;
			}
			
			
			if(CurrentBar < SpanB || CurrentBar < Conversion || CurrentBar < Lag || CurrentBar < BBase){return;}

			//	Tenkan sen = ConversionLine, Kijun sen = BaseLine, Senkou Span A = SpanALine, Senkou Span B = SpanBLine
			ConversionLine[0] = ((MAX(High,Conversion)[0] + MIN(Low,Conversion)[0]) / 2);
			BaseLine[0] = ((MAX(High,BBase)[0] + MIN(Low,BBase)[0]) / 2);
			SpanALine[0] = ((ConversionLine[0] + BaseLine[0]) / 2);
			SpanBLine[0] = ((MAX(High,SpanB)[0] + MIN(Low,SpanB)[0]) / 2);
			LagLine[Lag] = Close[0];

			
			UpperCloud[0] = Math.Max(SpanALine[0],SpanBLine[0]);
			LowerCloud[0] = Math.Min(SpanALine[0],SpanBLine[0]);
			
			
			//PlotBrushes[2][0] = Brushes.Orange;
			//PlotBrushes[3][0] = Brushes.Orange;
			
			if (SpanALine[BBase] > SpanBLine[BBase])
			{
				Trend2[0] = 1;
			}
			else if (SpanALine[BBase] < SpanBLine[BBase])
			{
				Trend2[0] = -1;
			}
			else
			{
				Trend2[0] = Trend2[1];
			}
			
			
			if (Close[0] > UpperCloud[BBase])
			{
				Trend[0] = 1;
				PriceCloudStatus = "Above Cloud";
			}
			else if (Close[0] < LowerCloud[BBase])
			{
				Trend[0] = -1;	
				PriceCloudStatus = "Below Cloud";
			}
			else
			{
				PriceCloudStatus = "Within Cloud";
				Trend[0] = 0;
			}			
			
			
			
			
			
			Signal[0] = 0;
		
			if (pCEEnabled)
			{
				if (Trend[0] == 1 && Trend[1] != 1)
					Signal[0] = 1;
			
				if (Trend[0] == -1 && Trend[1] != -1)
					Signal[0] = -1;
			}
			
			// new signals
			
			
			if (pC1Enabled)
			{			
				if (ConversionLine[0] > BaseLine[0])
					TenkanKijunCross[0] = 1;
				else if (ConversionLine[0] < BaseLine[0])
					TenkanKijunCross[0] = -1;	
				else
					TenkanKijunCross[0] = TenkanKijunCross[1];	
				
				
				
				if (TenkanKijunCross[0] == 1 && TenkanKijunCross[1] == -1)
					Signal[0] = 2;
				
				if (TenkanKijunCross[0] == -1 && TenkanKijunCross[1] == 1)
					Signal[0] = -2;	
				
			}
				
			Signals[0] = 0;
			
			if (Signal[0] > 0)
				Signals[0] = 1;
			
			if (Signal[0] < 0)
				Signals[0] = -1;		
			
			BB = 0;
			if (Calculate != Calculate.OnBarClose && !pQuickAudio)
				BB = 1;
			
			
			if (pAudioEnabled)
			{

				if (Signal[BB] > 0 && CurrentBar != LastAudioBar)
				{
					if (Signal[BB] == 1)
						AlertText = "Cloud Exit Long | " + BarsPeriod;
					if (Signal[BB] == 2)
						AlertText = PriceCloudStatus + " Tenkan Kijun Cross Long | " + BarsPeriod;
					
					Alert(CurrentBar.ToString(),Priority.High,AlertText,NinjaTrader.Core.Globals.InstallDir+@"\sounds\"+pWAVFileName,1,pArrowUpFBrush,Brushes.White);
					LastAudioBar = CurrentBars[0];
					
					
					
				}
				if (Signal[BB] < 0 && CurrentBar != LastAudioBar)
				{
					if (Signal[BB] == -1)
						AlertText = "Cloud Exit Short | " + BarsPeriod;
					if (Signal[BB] == -2)
						AlertText = PriceCloudStatus + " Tenkan Kijun Cross Short | " + BarsPeriod;
					
					Alert(CurrentBar.ToString(),Priority.High,AlertText,NinjaTrader.Core.Globals.InstallDir+@"\sounds\"+pWAVFileName,1,pArrowDownFBrush,Brushes.White);
					LastAudioBar = CurrentBar;					
				}	
				
			}							
				
	
		
						
			BB = 0;
			if (Calculate != Calculate.OnBarClose && !pQuickEmail)
				BB = 1;
			
			if (pEmailEnabled)
			{
				
				if (Signal[BB] > 0)
				{
					if (Signal[BB] == 1)
						subject = Instrument.FullName + " " + BarsPeriod + " Chart  |  " + "Cloud Exit Long " + Close[0].ToString(PriceString);
					if (Signal[BB] == 2)
						subject = Instrument.FullName + " " + BarsPeriod + " Chart  |  " + "Tenkan Kijun Cross Long " + Close[0].ToString(PriceString);
						
				}
				if (Signal[BB] < 0)
				{
					if (Signal[BB] == -1)
						subject = Instrument.FullName + " " + BarsPeriod + " Chart  |  " + "Cloud Exit Short " + Close[0].ToString(PriceString);
					if (Signal[BB] == -2)
						subject = Instrument.FullName + " " + BarsPeriod + " Chart  |  " + "Tenkan Kijun Cross Short " + Close[0].ToString(PriceString);
				}	
		
				
				message = subject;						
				if (Signal[BB] > 0 && LastEmailBar != CurrentBars[0])
				{
					//subject = Instrument.FullName + " " + BarsPeriod + " Chart  |  " + "New Signal Long " + Close[0].ToString(PriceString);
					
					//subject = "JSArrows2 Buy Signal | " + Bars.BarsPeriod.ToString() + " Chart | " + Instrument.FullName;					
					//message = subject;

					if (pEmailAddress != "" && State != State.Historical)
						SendMail(pEmailAddress, subject, message);
					
					LastEmailBar = CurrentBars[0];
				}

				if (Signal[BB] < 0 && LastEmailBar != CurrentBars[0])
				{
					//subject = Instrument.FullName + " " + BarsPeriod + " Chart  |  " + "New Signal Long " + Close[0].ToString(PriceString);
					
					//subject = "JSArrows2 Buy Signal | " + Bars.BarsPeriod.ToString() + " Chart | " + Instrument.FullName;					
					//message = subject;

					if (pEmailAddress != "" && State != State.Historical)
						SendMail(pEmailAddress, subject, message);
					
					LastEmailBar = CurrentBars[0];
				}			
					
			}
				
				
				
			if (pBackEnabled)
			{
				if (Signal[0] >= 1)
				{					
					SetBackColor(pBrush01);
				}				
				else if (Signal[0] <= -1)
				{
					SetBackColor(pBrush02);
				}
			}
			
			
			
			// removed: unused StartTime/EndTime loop ran ~739K iterations on first bar
			
			
			
			
			
			
		}

		
		
		
		
		
		
		//	2016.05.24
		public override void OnRenderTargetChanged()
		{
			if (upAreaBrushDx != null)
				upAreaBrushDx.Dispose();
			if (dnAreaBrushDx != null)
				dnAreaBrushDx.Dispose();
			if (upLineBrushDx != null)
				upLineBrushDx.Dispose();
			if (dnLineBrushDx != null)
				dnLineBrushDx.Dispose();
			if (textBrushDx != null)
				textBrushDx.Dispose();
			if (arrowUpBrushDX != null)
				arrowUpBrushDX.Dispose();
			if (arrowDownBrushDX != null)
				arrowDownBrushDX.Dispose();
			if (chartTextBrushDXCached != null)
				chartTextBrushDXCached.Dispose();
			if (chartBackgroundBrushDXCached != null)
				chartBackgroundBrushDXCached.Dispose();
			if (cachedLabelTextFormat != null)
				cachedLabelTextFormat.Dispose();

			if (RenderTarget != null)
			{
				try
				{
					upAreaBrushDx	= upAreaBrush.ToDxBrush(RenderTarget);
					dnAreaBrushDx	= dnAreaBrush.ToDxBrush(RenderTarget);
					upLineBrushDx	= upLineBrush.ToDxBrush(RenderTarget);
					dnLineBrushDx	= dnLineBrush.ToDxBrush(RenderTarget);
					textBrushDx		= textBrush.ToDxBrush(RenderTarget);

					if (pArrowUpFBrush != null)
						arrowUpBrushDX = pArrowUpFBrush.ToDxBrush(RenderTarget);
					if (pArrowDownFBrush != null)
						arrowDownBrushDX = pArrowDownFBrush.ToDxBrush(RenderTarget);

					if (ChartControl != null)
					{
						chartTextBrushDXCached = ChartControl.Properties.ChartText.ToDxBrush(RenderTarget);
						chartBackgroundBrushDXCached = ChartControl.Properties.ChartBackground.ToDxBrush(RenderTarget);
					}

					cachedLabelTextFormat = pTextFont.ToDirectWriteTextFormat();
					cachedLabelTextFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
					cachedLabelTextFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
					cachedLabelTextFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;
				}
				catch (Exception e) { }

				pArrowUpStroke.RenderTarget = RenderTarget;
				pArrowDownStroke.RenderTarget = RenderTarget;
			}
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
		
		private SharpDX.Direct2D1.Brush ChartBackgroundErrorBrushDX = null;
	
				
		
		
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
		
				
				// brushes now cached in OnRenderTargetChanged
							

			
			if (!IsInHitTest)
 			if (AllErrorMessages.Count > 0)
				{
				
					
					ChartBarsSwitch2(false);
					myProperties.AllowSelectionDragging = false;
					
					
					
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
					RenderTarget.FillRectangle(CenterRect, chartBackgroundBrushDXCached);
					RenderTarget.FillRectangle(CenterRect, ChartBackgroundErrorBrushDX);
					RenderTarget.DrawText(txt, CenterText, ExpandRect(CenterRect,-10,0), chartTextBrushDXCached);
					
					
					RenderTarget.AntialiasMode = oldAntialiasMode;
					
				
				
				ChartBackgroundErrorBrushDX.Dispose();		
				CenterText.Dispose();	
				
					//Print("bug2");
					
				return;
			}
			
			
			
			
			
			
			
			
			
			
				if (!Permission)
					return;
			
			if( Bars == null || ChartControl == null || Bars.Instrument == null || !IsVisible || IsInHitTest || bRendering) 
				return;

			bRendering = true;
			iX_barWidth = chartControl.GetXByBarIndex(chartControl.BarsArray[0], ChartBars.FromIndex+1)
							- chartControl.GetXByBarIndex(chartControl.BarsArray[0], ChartBars.FromIndex);
			//SharpDX.Direct2D1.AntialiasMode oldAntialiasMode = RenderTarget.AntialiasMode;	//	save for restore later			
			RenderTarget.AntialiasMode = SharpDX.Direct2D1.AntialiasMode.Aliased;
			RenderTarget.AntialiasMode = SharpDX.Direct2D1.AntialiasMode.PerPrimitive;
			
			SharpDxDrawCLoud(chartControl, chartScale, SpanALine, SpanBLine, BBase);	
			base.OnRender(chartControl, chartScale);
			
			
			
			
			// get chart XYscale data, global variables with Indicator
			dChart_Ymin = chartScale.MinValue;
			dChart_Ymax = chartScale.MaxValue;
			dChart_Yspan = chartScale.MaxMinusMin;
			dChart_Y = dChart_Ymin + dChart_Yspan*0.01;

		// START ARROWS
		
		SharpDX.Direct2D1.AntialiasMode oldAntialiasMode2 = RenderTarget.AntialiasMode;

		if (pArrowsEnabled && arrowUpBrushDX != null && arrowDownBrushDX != null)
		{
			SharpDX.Direct2D1.Brush arrowBrushDX = arrowUpBrushDX;
			Stroke ThisStroke = pArrowDownStroke;

			int FB = ChartBars.FromIndex;
			int LB = ChartBars.ToIndex;
			int BB = 0;
			int xt = 0;
			int yt = 0;
			double yt2 = 0;

			LB = Math.Min(CurrentBars[0], LB);

			ChartPanel chartPanel = chartControl.ChartPanels[chartScale.PanelIndex];

			if (ChartBars != null)
			for (int i = FB; i <= LB; i++)
			{
				BB = i;

				double ThisSignal = Signal.GetValueAt(BB);

				if (ThisSignal != 0)
				{
					xt = chartControl.GetXByBarIndex(ChartBars, i);

					int pTextOffset = 0;
					string lb = string.Empty;
					float newy = 0;
					float newx = 0;
					float totalarrowheight = pArrowOffset + pArrowSize + pArrowBarHeight;

					if (ThisSignal > 0)
					{
						yt = chartScale.GetYByValue(Low.GetValueAt(BB));
						yt2 = chartScale.GetYByValueWpf(Low.GetValueAt(BB));
						arrowBrushDX = arrowUpBrushDX;
						ThisStroke = pArrowUpStroke;
						lb = pLabelBuy;
					}

					if (ThisSignal < 0)
					{
						yt = chartScale.GetYByValue(High.GetValueAt(BB));
						yt2 = chartScale.GetYByValueWpf(High.GetValueAt(BB));
						arrowBrushDX = arrowDownBrushDX;
						ThisStroke = pArrowDownStroke;
						lb = pLabelSell;
					}

					SharpDX.Direct2D1.PathGeometry arrowGeo = CreateArrowGeometry(chartControl, chartPanel, chartScale, xt, yt, (int) Signal.GetValueAt(BB));

					RenderTarget.AntialiasMode = SharpDX.Direct2D1.AntialiasMode.PerPrimitive;

					RenderTarget.FillGeometry(arrowGeo, arrowBrushDX);
					RenderTarget.DrawGeometry(arrowGeo, ThisStroke.BrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
					arrowGeo.Dispose();

					if (pLabelsEnabled && cachedLabelTextFormat != null)
					{
						TextLayout labelLayout = new TextLayout(NinjaTrader.Core.Globals.DirectWriteFactory, lb, cachedLabelTextFormat, 1000, 1000);

						float RectWidth = labelLayout.Metrics.Width + (float) pTextFont.Size;
						float RectHeight = labelLayout.Metrics.Height + (float) pTextFont.Size / 2f + 1;

						if (ThisSignal > 0)
							newy = yt + totalarrowheight + 1 + pTextOffset;

						if (ThisSignal < 0)
							newy = yt - totalarrowheight - RectHeight - 1 - pTextOffset;

						newx = xt - RectWidth/2 - 2;

						SharpDX.RectangleF TextRect = new SharpDX.RectangleF(newx, newy, RectWidth, RectHeight);
						RenderTarget.DrawText(lb, cachedLabelTextFormat, TextRect, chartTextBrushDXCached);

						labelLayout.Dispose();
					}

					RenderTarget.AntialiasMode = SharpDX.Direct2D1.AntialiasMode.Aliased;
				}
			}
		}

		RenderTarget.AntialiasMode = oldAntialiasMode2;

		// END ARROWS
			
			
			
			
			
			
			
			
			//	restore render properties and dispose of resources
			RenderTarget.AntialiasMode = oldAntialiasMode;
			bRendering = false;	//	rendering is completed
			
			
			
			
			
			
			
			
			
			
			
			
			
			
			
			
			
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

			if (dir < 0)
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
		
		
		
//		public override string DisplayName
//		{
//			get
//			{
//				if (State == State.SetDefaults) // indicator window
//				{
//					if (Name == string.Empty)
//						return ThisName;
//					else
//						return Name;				
//				}
//				else // chart left corner label
//				{
//					// force display on chart to indicator name
					
////						if (Name != string.Empty) 
////							return ThisName;	
					
					
//					// allow the name to be removed from the chart
//					return Name;

//				}
//			}		
//		}		
			
			
		
        private double RTTS(double p)
        {
            return Instrument.MasterInstrument.RoundToTickSize(p);
        }
		
		
		
		#region Misc_functions
		//	kkc_2015, 2016.05.25	call from OnRender() only
		protected void DrawSignals(ChartControl chartControl, ChartScale chartScale)
		{
			int ibar_count = stSignal_all[0].iBar;
			if(ibar_count < 1)
				return;
			//	ChartBars properties are visible to all functions
			int ibar_start = ChartBars.FromIndex;
			int ibar_end = ChartBars.ToIndex;
			int ibar_max = ChartBars.Bars.Count;
			double dChart_Ymin = chartScale.MinValue;
			double dChart_Span = chartScale.MaxMinusMin;
			int iYmin = chartScale.GetYByValue(dChart_Ymin);	//	chart coordinate value
			int iYmax = chartScale.GetYByValue(dChart_Ymax);
			
			int iChart_X1 = ibar_max - ibar_start - 2;
			int iChart_X2 = ibar_max - ibar_end - 2;
			int iChart_X  = iChart_X1;
			int iSignalMax = stSignal_all[0].iBar;
			bool bDownTrend = false;
			bool bPrtSignal = true;
			string sSignal = "NO";		//	initialize string
			double Y_step = dChart_Span*0.05;	//	for vertical spacing of indicator codes on the chart
			double dChart_Y = dChart_Ymin + Y_step;
			string sTag = "SignalCode";
			int iCurrentBar = 0;
			int iCurrentBar_prev = 0;
			Point P1 = new Point();
			Point P2 = new Point();
			Brush AreaBrush = Brushes.LightGray;		//	use stock brushes, no need to dispose
			Brush outlineBrush = Brushes.LightGray;
			Brush lineBrush;

			SharpDX.Direct2D1.StrokeStyle strokeStyle = new Stroke(Brushes.Gray, DashStyleHelper.DashDotDot, 2f).StrokeStyle;

			int i = 0;
			while(++i < iSignalMax && stSignal_all[i].iBar < ibar_start);
			if(i >= iSignalMax || stSignal_all[i].iBar > ibar_end)
				return;
			
			sSignal = "Nbr.Signal = " + stSignal_all[0].iBar;	//	for display at bottom right
			Draw.TextFixed(this, "Profit", sSignal, TextPosition.BottomRight, Brushes.Black, LegendFont, Brushes.Blue, Brushes.White, 100);
			
			bBarSort();		//	perform sorting after additional data from stBuySell_all
			
			while(i <= iSignalMax && stSignal_all[i].iBar <= ibar_end)
			{
				iCurrentBar = stSignal_all[i].iBar;
				iSignal = stSignal_all[i].iSignal;
				if(iCurrentBar_prev == iCurrentBar)
					iNbrSameBar++;	//	for Signal printing Ypos
				else
				{
					if(iCurrentBar - stSignal_all[i-1].iBar == 1)
						iNbrSameBar = stSignal_all[i-1].iNbrSignal+1;
					else
						iNbrSameBar = 0;
					iCurrentBar_prev = iCurrentBar;
				}

				if((iSignal < icSignal_TK) || (iSignal > icSignal_DU))
				{
					i++;
					iSignal = icNoSignal;
					iCurrentBar_prev = 0;
					continue;
				}
				
				iChart_X = ibar_max - iCurrentBar - 2;
				stSignal_all[i].iNbrSignal = iNbrSameBar;

				if(iChart_X < iChart_X1 && iChart_X > iChart_X2)
				{
					iSignalStrength = stSignal_all[i].iStrength;
					bDownTrend = stSignal_all[i].bTrendDown;
					AreaBrush = bDownTrend ? Brushes.Pink : Brushes.LightGreen;
					outlineBrush = bDownTrend ? Brushes.Red : Brushes.DarkGreen;
					sSignal = scSignals[iSignal-icSignal_Base];
					if(iSignal == icSignal_LS)
						sSignal = bDownTrend ? "S" : "L";
					else
						if(iSignal == icSignal_DU)
							sSignal = bDownTrend ? "D" : "U";
					
					if(iSignalStrength == icSignal_Neutral)
					{
						AreaBrush = Brushes.White;
						outlineBrush = Brushes.DarkGray;
					}
					else if(iSignalStrength == icSignal_Weak)
					{
						AreaBrush = Brushes.LightGray;
						outlineBrush = Brushes.DarkGray;
					}

					bPrtSignal = (iSignalStrength == icSignal_Strong && Isignal_Strong);
					bPrtSignal = bPrtSignal || (iSignalStrength == icSignal_Neutral && Isignal_Neutral);
					bPrtSignal = bPrtSignal || (iSignalStrength == icSignal_Weak && Isignal_Weak);
					bPrtSignal = bPrtSignal || (iSignal == icSignal_LS);
					if(!bPrtSignal)
					{
						i++;
						continue;
					}
					if(Low.GetValueAt(iCurrentBar) > dChart_Ymin + 0.3*dChart_Yspan)
						dChart_Y = dChart_Ymin + (iNbrSameBar+2) * Y_step;	//	bottom position
					else
					dChart_Y = dChart_Ymax - (iNbrSameBar+2) * Y_step;	//	top position
					
					//	get chart coordinate data
					P1.X = chartControl.GetXByBarIndex(chartControl.BarsArray[0], iCurrentBar);
					P1.Y = iYmin;
					P2.X = P1.X;
					P2.Y = iYmax;

					//	set up color brushes
					lineBrush = bDownTrend ? Brushes.Red : Brushes.DarkGreen;
					RenderTarget.DrawLine(P1.ToVector2(), P2.ToVector2(), lineBrush.ToDxBrush(RenderTarget), 1f, strokeStyle);
 
					P1.Y = chartScale.GetYByValue(dChart_Y);;
					SharpDxDrawSignalCode(sSignal, P1, outlineBrush.ToDxBrush(RenderTarget), AreaBrush.ToDxBrush(RenderTarget));				
				}
				i++;
			}		

			strokeStyle.Dispose();
		}
	
		//	sort the bar into sequential order
		protected bool bBarSort()
		{
			bool bSortReqd = false;
			if(stSignal_all[0].iSignal != icSignalSort)
				return(bSortReqd);

			int i = 1, j = 1, k = 1;
			stSignal stTemp;				
			while(++i <= stSignal_all[0].iBar)
			{
				if(stSignal_all[i].iBar < stSignal_all[i-1].iBar)
				{
					stTemp = stSignal_all[i];
					j = i-1;
					while((--j>0) && (stSignal_all[j].iBar > stSignal_all[i].iBar));
					for(k=i; k>j+1; k--)
						stSignal_all[k] = stSignal_all[k-1];
					stSignal_all[j+1] = stTemp;
				}
			}
			stSignal_all[0].iSignal = 0;		//	all sorted
			return(bSortReqd);
		}
		
		protected void SharpDxDrawDiamond(Point pStart, SharpDX.Direct2D1.Brush lineBrushDx, SharpDX.Direct2D1.Brush fillBrushDx)
		{
			const int icPoints = 5;
			float iX = (pStart.X > 6) ? (float)pStart.X : 6f;		//	make sure there is at least n pixels on chart left side
			float iY = (float)pStart.Y;		//	Y is always positive
			float [,] XY = new float[icPoints,2] {{0,0}, {6,-6}, {0,-12}, {-6,-6}, {0,0}};
			Point P = new Point();
			SharpDX.Vector2[] vector = new SharpDX.Vector2[icPoints];

			for(int i=0; i<icPoints; i++)
			{
				P.X = iX + XY[i,0];
				P.Y = iY + XY[i,1];
				vector[i] = P.ToVector2();
			}

			DrawGeometry(vector, lineBrushDx, fillBrushDx, true);
		}

		protected void SharpDxDrawArrow(Point pStart, SharpDX.Direct2D1.Brush lineBrushDx, SharpDX.Direct2D1.Brush fillBrushDx, bool iUpArrow=true)
		{
			const int icPoints = 9;
			float iX = (pStart.X > 6) ? (float)pStart.X : 6f;		//	make sure there is at least n pixels on chart left side
			float iY = (float)pStart.Y;		//	Y is always positive
			float [,] XY = new float[icPoints,2] {{0,0}, {2,0}, {2,-4}, {6,-4}, {0,-10}, {-6,-4}, {-2,-4}, {-2,0}, {0,0}};
			Point P = new Point();
			SharpDX.Vector2[] vector = new SharpDX.Vector2[icPoints];
			float sgn = iUpArrow ? 1f : -1f;

			for(int i=0; i<icPoints; i++)
			{
				P.X = iX + XY[i,0];
				P.Y = iY + sgn*XY[i,1];
				vector[i] = P.ToVector2();
			}

			DrawGeometry(vector, lineBrushDx, fillBrushDx, true);
		}

		//	kkc_2015, 2016.05.25, must be called by DrawSignals	
		protected void SharpDxDrawSignalCode(string sSignal, Point pStart, SharpDX.Direct2D1.Brush lineBrushDx, SharpDX.Direct2D1.Brush fillBrushDx)
		{
			float iX = (float)((iX_barWidth-1f)*2f);		//	leave one pixel on each side of the rect
			float fXstart = (float)(pStart.X - iX/2f);
			fXstart = (fXstart < 0) ? 0 : fXstart;		//	negative value will cause error preparing textLayout
			
			Point P = new Point(fXstart+1f, pStart.Y+2);	//	adjust location to place text
			SharpDX.RectangleF rectf = new SharpDX.RectangleF(fXstart, (float)pStart.Y, (float)iX, fontHeight+4f);		//	equal x, y dimensions
			SharpDX.DirectWrite.TextFormat textFormat = new SharpDX.DirectWrite.TextFormat(Core.Globals.DirectWriteFactory, "Courier New", SharpDX.DirectWrite.FontWeight.Normal,
															SharpDX.DirectWrite.FontStyle.Normal, SharpDX.DirectWrite.FontStretch.Normal, fontHeight)
			{
				TextAlignment = SharpDX.DirectWrite.TextAlignment.Leading,
				WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap
			};
			RenderTarget.FillRectangle(rectf, fillBrushDx);		//	fill, then draw outline box
			RenderTarget.DrawRectangle(rectf, lineBrushDx);
			SharpDX.DirectWrite.TextLayout textLayout = new SharpDX.DirectWrite.TextLayout(Core.Globals.DirectWriteFactory, sSignal, textFormat, rectf.X+2f, fontHeight+4f);
			RenderTarget.DrawTextLayout(P.ToVector2(), textLayout, textBrushDx, SharpDX.Direct2D1.DrawTextOptions.NoSnap);	//	use textBrushDx
			
			textLayout.Dispose();
			textFormat.Dispose();		
		}

		//	kkc_2015, 2016.05.25, must be called by OnRender	
		protected void SharpDxDrawCLoud(ChartControl chartControl, ChartScale chartScale, ISeries<double> SpanA_s, ISeries<double> SpanB_s, int iOffset)
		{
			// RenderTarget is always full panel, so we need to be mindful which sub ChartPanel we're dealing with
			// always use ChartPanel X, Y, W, H - as ActualWidth, Width, ActualHeight, Height are in WPF units, so they can be drastically different depending on DPI set

			var bars0 = chartControl.BarsArray[0];
			int iX_start = chartControl.GetXByBarIndex(bars0, ChartBars.FromIndex);		//	panel.X location
			int iX_End = chartControl.GetXByBarIndex(bars0, ChartBars.ToIndex);
			int iBarWidth = iX_barWidth;
			if (iBarWidth <= 0)
				iBarWidth = chartControl.GetXByBarIndex(bars0, 1) - chartControl.GetXByBarIndex(bars0, 0);
			int iBar_Start = (ChartBars.FromIndex > iOffset*3) ? ChartBars.FromIndex : iOffset*3;		//	global parameter, absolute bar number
			int iNbrBarSpaceAvailable = (ChartPanel.W - iX_End) / iBarWidth - 1;
			if(ChartBars.ToIndex - iBar_Start < 2)		//
				return;

			int iBar_End = ChartBars.ToIndex + Math.Min(iOffset, iNbrBarSpaceAvailable);

			int iPointMax = iBar_End - iBar_Start +1;
			if (cloudIX.Length < iPointMax)
			{
				cloudIX = new int[iPointMax];
				cloudIA = new int[iPointMax];
				cloudIB = new int[iPointMax];
			}
			int[] iX = cloudIX;
			int[] iA = cloudIA;
			int[] iB = cloudIB;

			int iX_iBar_Start = iX_start + (iBar_Start - ChartBars.FromIndex) * iBarWidth;
			int idx, idx_offset;		//	index for series A, B
			int i, j, k, m;
   			for(i = 0; i < iPointMax; i++)
			{
				idx = i + iBar_Start;
				idx_offset = idx - iOffset;
				iX[i] = iX_iBar_Start + i * iBarWidth;
				iA[i] = chartScale.GetYByValue(SpanA_s.GetValueAt(idx_offset));
				iB[i] = chartScale.GetYByValue(SpanB_s.GetValueAt(idx_offset));
			}

			double dX = iX[0];		//	initialize prior to entering while() loop
			double dY = iA[0];
			Point p = new Point();
			int iVectorMemberMax;
			bool bUpCloud = (iA[0] > iB[0]);		//	Uptrend
			i = 0;
			while(i < iPointMax-1)
			{
				k = i;	//	k has the starting point for each geometry
				while((i < iPointMax) && (iA[i] > iB[i]) == bUpCloud)		//	must check i<iPointMax first, prior to == bUpCloud
					i++;
				iVectorMemberMax = 2*(i-k+1);	//	Points for iA + iB + connecting point between iA and iB
				SharpDX.Vector2[] vectorSpan = new SharpDX.Vector2[iVectorMemberMax];
				m = 0;
				p.X = dX;	//	use last data point at the cross of SpanA_s and SpanB_s as starting point
				p.Y = dY;
				vectorSpan[m++] = p.ToVector2();
				
				for(j=k; j<i; j++)
				{
					p.X = iX[j];
					p.Y = iA[j]; 	//	data for SpanA_s series
					vectorSpan[m++] = p.ToVector2();
				}
				if(i < iPointMax-1)	//	Not the last point, need to create the intersection point
				{
					//	calculate iA, iB connecting point using equation
					dX = (iA[j-1]-iB[j-1]) * iX_barWidth / (iB[j]-iA[j]+iA[j-1]-iB[j-1]);	//	dX portion of iX_barWidth
					dY = (iB[j]-iB[j-1]) / iX_barWidth * dX + iB[j-1];
					dX += iX[j-1];	//	actual dX including iX_barWidth portion
					p.X = dX;
					p.Y = dY;
				}
				else
				{
					//	no iA / iB crossed, therefore draw straight line between iA & iB
					p.X = iX[j-1];
					p.Y = iB[j-1];
				}
				vectorSpan[m++] = p.ToVector2();
				for(j=i-1; j>=k; j--)
				{
					p.X = iX[j];
					p.Y = iB[j]; 	//	data for SpanB_s series
					vectorSpan[m++] = p.ToVector2();
				}
				//	no need to connect the line end to start
				if(!bUpCloud)	//	bUpCloud has the next cloud status, previous status = !bUpCloud
					DrawGeometry(vectorSpan, upLineBrushDx, upAreaBrushDx, false);
				else
					DrawGeometry(vectorSpan, dnLineBrushDx, dnAreaBrushDx, false);
				if(i < iPointMax)
					bUpCloud = (iA[i] > iB[i]);		//	placed in this location to ensure iA[i] is not out of range
			}
			//	draw the cloud outline
			Point p1 = new Point();
			Point p2 = new Point();

			for(i = 0; i < iPointMax-2; i++)
			{
				
				int thisget = ChartBars.FromIndex + i +1;
				//Print(thisget);
				
				double thistrend = Trend2.GetValueAt(thisget);
				
				finalLineBrushDx = upLineBrushDx;
				if (thistrend == -1)
				finalLineBrushDx = dnLineBrushDx;
				
				p1.X = iX[i];
				p2.X = iX[i+1];
				p1.Y = iA[i];
				p2.Y = iA[i+1];
				RenderTarget.DrawLine(p1.ToVector2(), p2.ToVector2(), finalLineBrushDx, 1);
				p1.Y = iB[i];
				p2.Y = iB[i+1];
				RenderTarget.DrawLine(p1.ToVector2(), p2.ToVector2(), finalLineBrushDx, 1);
				
			}
		}

		protected void DrawGeometry(SharpDX.Vector2[] vectorSpan, SharpDX.Direct2D1.Brush LineBrushDx, SharpDX.Direct2D1.Brush AreaBrushDx, bool bDrawOutline)
		{
			//	the line in the vector must be continuous, unclosed gemoetry will be closed automatically
			//	all elements in the vector must be provided with data
			SharpDX.Direct2D1.PathGeometry geo1 = new SharpDX.Direct2D1.PathGeometry(Core.Globals.D2DFactory);
			SharpDX.Direct2D1.GeometrySink sink1 = geo1.Open();
			Point iP_start = new Point(vectorSpan[0].X, vectorSpan[0].Y);
			sink1.BeginFigure(iP_start.ToVector2(), SharpDX.Direct2D1.FigureBegin.Filled);
			sink1.AddLines(vectorSpan);
			sink1.EndFigure(SharpDX.Direct2D1.FigureEnd.Closed);
			sink1.Close();
			RenderTarget.FillGeometry(geo1, AreaBrushDx);
			if(bDrawOutline)
				RenderTarget.DrawGeometry(geo1, LineBrushDx);	//	draw shape outline
			geo1.Dispose();
			sink1.Dispose();
		}		
		
				
		//	kkc_2015 2015.12.31	Assign Arrow color code & Signal code; return Signal location status
		protected int ArrowCodes(int iSeekSignal)
		{
			int iCloudStatus = icInCloud;
			//	ConversionLine[barsAgo] = Tenkan Sen
			//	BaseLine[barsAgo] = Kijun Sen
			//	SpanALine[barsAgo] = Senkou Span A
			//	SpanBLine[barsAgo] = Senkou Span B
			ArrowBrush = Brushes.LightGray;
			sSignalCode = " ";		//	no signal code
			
			switch(iSeekSignal)
			{
				case icSignal_TK_DN:	//	Tenkan / Kijun Cross in relation to cloud
				case icSignal_TK_UP:
					sSignalCode = "TK";
					iCloudStatus = iGetCloudStatus(ConversionLine[0]);		//	use data[0 BarAgo], *** neeed checking ***
					break;
				case icSignal_PK_DN:
				case icSignal_PK_UP:
					sSignalCode = "PK";
					iCloudStatus = iGetCloudStatus(Close[0]);		//	use data[0 BarAgo]
					break;
				case icSignal_KB_DN:	//	the price is always inside Kumo cloud, waiting for breakout
				case icSignal_KB_UP:
					sSignalCode = "KB";
					break;
				case icSignal_SS_DN:
				case icSignal_SS_UP:
					sSignalCode = "SS";
					iCloudStatus = iGetCloudStatus(SpanALine[0]);		//	use data[0 BarAgo]
					break;
				case icSignal_CP_DN:
				case icSignal_CP_UP:
					sSignalCode = "CP";
					//	cloud and Chikoou Span data should be at Lag bar ago
					iCloudStatus = iGetCloudStatus(Close[Lag],Lag);
					break;
				default:
					break;
			}
			//	ArrowBrush color is for testing only, but it is also used to determine iSignalStrength
			bArrowDn = (iSeekSignal % 2 != 0);	//	down signal = odd number
			if(iSeekSignal==icSignal_KB_DN || iSeekSignal==icSignal_KB_UP)
				ArrowBrush = (iSeekSignal==icSignal_KB_DN) ? Brushes.Red : Brushes.Green;
			else
				switch(iCloudStatus)
				{
					case icBelowCloud:
						ArrowBrush = bArrowDn ? Brushes.Red : Brushes.DarkGray;
						break;
					case icInCloud:
						ArrowBrush = Brushes.LightGray;
						break;
					case icAboveCloud:
						ArrowBrush = bArrowDn ? Brushes.DarkGray : Brushes.Green;
						break;
					default:
						break;
				}
			//	set signal strength
			iSignalStrength = icSignal_Strong;
			if(ArrowBrush == Brushes.DarkGray)
				iSignalStrength = icSignal_Weak;
			else if(ArrowBrush == Brushes.LightGray)
				iSignalStrength = icSignal_Neutral;
			return(iSignalStrength);
		}
		
		protected int iGetCloudStatus(double dY, int iLag=0)
		{
			//	dY has data from 1 BarAgo 
			int iRetSignal = icInCloud;
			
			if(dY < Math.Min(SpanALine[0+BBase+iLag],SpanBLine[0+BBase+iLag]))
				iRetSignal = icBelowCloud;
			else
				if(dY > Math.Max(SpanALine[0+BBase+iLag],SpanBLine[0+BBase+iLag]))
					iRetSignal = icAboveCloud;
			return(iRetSignal);
		}
		
		//	2015.12.31, return CrossSignal, _DN, _UP
		protected int iGetCrossSignal(int iSeekSignal)
		{
			int i, iUpDown;
			int iRetSignal = icNoSignal;
			bool bConditionMet = false;
			if(CurrentBar < SpanB+icTrendSpan || CurrentBar < Conversion+icTrendSpan || CurrentBar < Lag+icTrendSpan || CurrentBar < BBase+icTrendSpan)
				return(iRetSignal);	

			switch(iSeekSignal)
			{
				case icSignal_TK:	//	check signal for Tenkan / Kijun Crossed
					bConditionMet = true;
					//	check for Tankan above Kijun BaseLine
					for(i=1; i<=icTrendSpan; i++)
						if(ConversionLine[i] <= BaseLine[i])
							bConditionMet = false;
					if(bConditionMet)	//	Tankan is above Kijan sen, precursory for downtrend
						iRetSignal = (ConversionLine[0] <= BaseLine[0]) ? icSignal_TK_DN : icNoSignal;
					else
					{
						bConditionMet = true;
						//	check for Tankan below Kijun BaseLine
						for(i=1; i<=icTrendSpan; i++)
							if(ConversionLine[i] >= BaseLine[i])
								bConditionMet = false;
						if(bConditionMet)	//	Tankan is below Kijan sen, precursory for uptrend
							iRetSignal = (ConversionLine[0] >= BaseLine[0]) ? icSignal_TK_UP : icNoSignal;
					}
					break;
				case icSignal_PK:	//	check signal for price crosses Kijun sen
					bConditionMet = true;
					//	check for Price above Kijun BaseLine
					for(i=1; i<=icTrendSpan; i++)
						if(Low[i] <= BaseLine[i])
							bConditionMet = false;
					if(bConditionMet)	//	Close Price is above Kijan sen, precursory for downtrend
						iRetSignal = (Close[0] <= BaseLine[0]) ? icSignal_PK_DN : icNoSignal;
					else
					{
						bConditionMet = true;
						//	check for Price below Kijun BaseLine
						for(i=1; i<=icTrendSpan; i++)
							if(High[i] >= BaseLine[i])
								bConditionMet = false;
						if(bConditionMet)	//	Price is below Kijun sen, precursory for uptrend
							iRetSignal = (Close[0] >= BaseLine[0]) ? icSignal_PK_UP : icNoSignal;
					}
					break;
				case icSignal_KB:	//	check for Kumo Breakout, Kumo data is from older data[BBase]
					bConditionMet = true;
					//	check for Price below top of Kumo cloud
					for(i=1; i<=icTrendSpan; i++)
						if(Close[i] >= Math.Max(SpanALine[i+BBase],SpanBLine[i+BBase]))
							bConditionMet = false;
					if(bConditionMet)	//	Close Price is below top of cloud, precursory for uptrend breakout
						iRetSignal = (Close[0] >= Math.Max(SpanALine[0+BBase],SpanBLine[0+BBase])) ? icSignal_KB_UP : icNoSignal;
					if(iRetSignal == icNoSignal)
					{
						bConditionMet = true;
						//	check for Price above bottom of Kumo cloud
						for(i=1; i<=icTrendSpan; i++)
							if(Close[i] <= Math.Min(SpanALine[i+BBase],SpanBLine[i+BBase]))
								bConditionMet = false;
						if(bConditionMet)	//	Price is above bottom of cloud, precursory for downtrend
							iRetSignal = (Close[0] <= Math.Min(SpanALine[0+BBase],SpanBLine[0+BBase])) ? icSignal_KB_DN : icNoSignal;
					}
					break;
				case icSignal_SS:	//	check signal for Senkou SpanA / SpanB cross
					bConditionMet = true;
					//	check for SpanA above SpanB
					for(i=1; i<=icTrendSpan; i++)
						if(SpanALine[i] <= SpanBLine[i])
							bConditionMet = false;
					if(bConditionMet)	//	SpanALine is above SpanBLine, precursory for downtrend
						iRetSignal = (SpanALine[0] <= SpanBLine[0]) ? icSignal_SS_DN : icNoSignal;
					else
					{
						bConditionMet = true;
						//	check for SpanA below SpanB
						for(i=1; i<=icTrendSpan; i++)
							if(SpanALine[i] >= SpanBLine[i])
								bConditionMet = false;
						if(bConditionMet)	//	SpanALine is below SpanBLine, precursory for uptrend
							iRetSignal = (SpanALine[0] >= SpanBLine[0]) ? icSignal_SS_UP : icNoSignal;
					}
					break;
				case icSignal_CP:	//	check signal for Chikou Span Cross, all data to be based on [Lag]
					bConditionMet = true;
					//	check for Chikou Span above Price, LagLine[Lag] = Close[0]
					for(i=1; i<=icTrendSpan; i++)
						if(LagLine[Lag+i] <= High[Lag+i])
							bConditionMet = false;
					if(bConditionMet)	//	Chikou Span is above Price, precursory for downtrend
						iRetSignal = (LagLine[Lag] <= Close[Lag]) ? icSignal_CP_DN : icNoSignal;
					else
					{
						bConditionMet = true;
						//	check for Chikou Span below Price
						for(i=1; i<=icTrendSpan; i++)
							if(LagLine[Lag+i] >= Low[Lag+i])
								bConditionMet = false;
						if(bConditionMet)	//	Price is below Kijun sen, precursory for uptrend
							iRetSignal = (LagLine[Lag] >= Close[Lag]) ? icSignal_CP_UP : icNoSignal;
					}
					break;
				default:
					break;
			}
			return(iRetSignal);
		}

		#endregion
		
		
//        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
//        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
//       public Series<double> Signals
//        {
//            get { return Signal; }
//        }		
		
		
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Cloud Fill Positive Color", Order = 21, GroupName = "Plots")]
		public Brush UpAreaBrush
		{
			get { return upAreaBrush; }
			set
			{
				upAreaBrush = value;
				if (upAreaBrush != null)
				{
					if (upAreaBrush.IsFrozen)
						upAreaBrush = upAreaBrush.Clone();
					upAreaBrush.Opacity = iareaOpacity / 100d;
					upAreaBrush.Freeze();
				}
			}
		}

		[Browsable(false)]
		public string UpAreaBrushSerialize
		{
			get { return Serialize.BrushToString(UpAreaBrush); }
			set { UpAreaBrush = Serialize.StringToBrush(value); }
		}

		[XmlIgnore]
		//[Display(ResourceType = typeof(Custom.Resource), Name = "NinjaScriptDrawingToolShapesAreaBrush", Order=2, GroupName = "NinjaScriptGeneral")]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Cloud Fill Negative Color", Order = 22, GroupName = "Plots")]
		public Brush DnAreaBrush
			
		{
			get { return dnAreaBrush; }
			set
			{
				dnAreaBrush = value;
				if (dnAreaBrush != null)
				{
					if (dnAreaBrush.IsFrozen)
						dnAreaBrush = dnAreaBrush.Clone();
					dnAreaBrush.Opacity = iareaOpacity / 100d;
					dnAreaBrush.Freeze();
				}
			}
		}

		[Browsable(false)]
		public string DnAreaBrushSerialize
		{
			get { return Serialize.BrushToString(DnAreaBrush); }
			set { DnAreaBrush = Serialize.StringToBrush(value); }
		}

		[Range(0, 100)]
		//[Display(ResourceType = typeof(Custom.Resource), Name = "NinjaScriptDrawingToolAreaOpacity", Order=3, GroupName = "NinjaScriptGeneral")]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Opacity (%)", Order = 23, GroupName = "Plots")]
		public int iAreaOpacity
		{
			get { return iareaOpacity; }
			set
			{
				iareaOpacity = Math.Max(0, Math.Min(100, value));
				if (upAreaBrush != null)
				{
					upAreaBrush = upAreaBrush.Clone();
					upAreaBrush.Opacity = iareaOpacity / 100d;
					upAreaBrush.Freeze();
				}
				if (dnAreaBrush != null)
				{
					dnAreaBrush = dnAreaBrush.Clone();
					dnAreaBrush.Opacity = iareaOpacity / 100d;
					dnAreaBrush.Freeze();
				}
			}
		}

		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Cloud Line Positive Color", Order = 10, GroupName = "Plots")]
		public Brush UpLineBrush
		{
			get { return upLineBrush; }
			set { upLineBrush = value; }
		}

		[Browsable(false)]
		public string UpLineBrushSerialize
		{
			get { return Serialize.BrushToString(UpLineBrush); }
			set { UpLineBrush = Serialize.StringToBrush(value); }
		}

		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Cloud Line Negative Color", Order = 11, GroupName = "Plots")]
		public Brush DnLineBrush
		{
			get { return dnLineBrush; }
			set { dnLineBrush = value; }
		}

		[Browsable(false)]
		public string DnLineBrushSerialize
		{
			get { return Serialize.BrushToString(DnLineBrush); }
			set { DnLineBrush = Serialize.StringToBrush(value); }
		}
		
//		[Range(1, 27)]
//		[NinjaScriptProperty]
//		[Display(Name="Conversion", Description = "Tenkan line - 9 typical", Order=1, GroupName="Parameters")]
//		public int Conversion
//		{ get; set; }

//		[Range(1, 78)]
//		[NinjaScriptProperty]
//		[Display(Name="Base", Description = "Kijun line - 26 typical", Order=2, GroupName="Parameters")]
//		public int BBase
//		{ get; set; }

//		[Range(1, 156)]
//		[NinjaScriptProperty]
//		[Display(Name="SpanB", Description = "Senkou Span B - 52 typical", Order=3, GroupName="Parameters")]
//		public int SpanB
//		{ get; set; }
		
		[Range(1, 27)]
		[NinjaScriptProperty]
		[Display(Name="Fast (Tenkan)", Description = "Fast (Tenkan)", Order=1, GroupName="Parameters")]
		public int Conversion
		{ get; set; }

		[Range(1, 78)]
		[NinjaScriptProperty]
		[Display(Name="Medium (Kijun)", Description = "Medium (Kijun)", Order=2, GroupName="Parameters")]
		public int BBase
		{ get; set; }

		[Range(1, 156)]
		[NinjaScriptProperty]
		[Display(Name="Slow (Senkou)", Description = "Slow (Senkou)", Order=3, GroupName="Parameters")]
		public int SpanB
		{ get; set; }
		

//		[Range(1, 78)]
//		[NinjaScriptProperty]
//		[Display(Name="Lag", Description = "Chikou Span - 26 typical", Order=4, GroupName="Parameters")]
//		public int Lag
//		{ get; set; }
	
//		[NinjaScriptProperty]
//		[Display(Name="Strong signal", Order=5, GroupName="Settings")]
//		public bool Isignal_Strong
//		{ get; set; }
		
//		[NinjaScriptProperty]
//		[Display(Name="Neutral signal", Order=6, GroupName="Settings")]
//		public bool Isignal_Neutral
//		{ get; set; }
		
//		[NinjaScriptProperty]
//		[Display(Name="Weak signal", Order=7, GroupName="Settings")]
//		public bool Isignal_Weak
//		{ get; set; }
		
//		[NinjaScriptProperty]
//		[Display(Name="Signal legend", Order=8, GroupName="Settings")]
//		public bool Ilegend
//		{ get; set; }
		
		
		
		
		
		
		
		
		
		
		

		
		private TimeSpan pStartTime = new TimeSpan(0,00,0);
//		[Display(ResourceType = typeof(Custom.Resource), Name = "TimeBegin", Description = "", GroupName = "Parameters", Order = 11)]
//		public string StartT
//		{
//			get { return pStartTime.Hours.ToString("0")+":"+pStartTime.Minutes.ToString("00"); }
//			set { if(!TimeSpan.TryParse(value, out pStartTime)) pStartTime=new TimeSpan(0,0,0); }
//		}		
		
		private TimeSpan pEndTime = new TimeSpan(0,00,0);
//		[Display(ResourceType = typeof(Custom.Resource), Name = "TimeEnd", Description = "", GroupName = "Parameters", Order = 12)]
//		public string EndT
//		{
//			get { return pEndTime.Hours.ToString("0")+":"+pEndTime.Minutes.ToString("00"); }
//			set { if(!TimeSpan.TryParse(value, out pEndTime)) pEndTime=new TimeSpan(0,0,0); }
//		}	
		
		
		
		// AUDIO
		
		private bool pAudioEnabled = false;
		[Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", Description = "", GroupName = "Audio", Order = 1)]
        public bool AudioEnabled
        {
            get { return pAudioEnabled; }
            set { pAudioEnabled = value; }
        }		
		
		private bool pQuickAudio = false;
		[Display(ResourceType = typeof(Custom.Resource), Name = "IntraBar", GroupName = "Audio", Order = 2)]
        public bool QuickAudio
        {
            get { return pQuickAudio; }
            set { pQuickAudio = value; }
        }	
		
		private string pWAVFileName = "Alert2.wav";
		[Display(ResourceType = typeof(Custom.Resource), Name = "Buy WAV File", Description = "Sound file to play when a buy signal occurs.", GroupName = "Audio", Order = 3)]
		public string WAVFileName
		{
			get { return pWAVFileName; }
			set { pWAVFileName = value; }
		}

		private string pWAVFileName2 = "Alert2.wav";
		[Display(ResourceType = typeof(Custom.Resource), Name = "Sell WAV File", Description = "Sound file to play when a sell signal occurs.", GroupName = "Audio", Order = 4)]
		public string WAVFileName2
		{
			get { return pWAVFileName2; }
			set { pWAVFileName2 = value; }
		}
		
		
		
		// SIGNALS
		

		
		
		
        private bool pCEEnabled = false;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Cloud Exit", Description = "", GroupName = "Signals", Order = 1)]
        public bool CEEnabled
        {
            get { return pCEEnabled; }
            set { pCEEnabled = value; }
        }			
				
        private bool pC1Enabled = true;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Tenkan Kijun Cross", Description = "", GroupName = "Signals", Order = 2)]
        public bool C1Enabled
        {
            get { return pC1Enabled; }
            set { pC1Enabled = value; }
        }				
		
		
		// ARROW INPUTS

        private bool pArrowsEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", Description = "", GroupName = "Arrows", Order = 0)]
        public bool ArrowsEnabled
        {
            get { return pArrowsEnabled; }
            set { pArrowsEnabled = value; }
        }
		
        private float pArrowOffset = 20;
        [Range(0, 1000)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Offset (Pixels)", Description = "", GroupName = "Arrows", Order = 4)]
        public float ArrowOffset
        {
            get { return pArrowOffset; }
            set { pArrowOffset = value; }
        }
		
        private float pArrowBarWidth = 3;
        [Range(0, 1000)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Structure - Bar Width", Description = "", GroupName = "Arrows", Order = 3)]
        public float ArrowBarWidth
        {
            get { return pArrowBarWidth; }
            set { pArrowBarWidth = value; }
        }
		
        private float pArrowBarHeight = 18;
        [Range(0, 1000)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Structure - Bar Height", Description = "", GroupName = "Arrows", Order = 2)]
        public float ArrowBarHeight
        {
            get { return pArrowBarHeight; }
            set { pArrowBarHeight = value; }
        }
		
        private float pArrowSize = 9;
        [Range(0, 1000)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Structure - Arrow Size", Description = "", GroupName = "Arrows", Order = 1)]
        public float ArrowSize
        {
            get { return pArrowSize; }
            set { pArrowSize = value; }
        }

		private Brush pArrowUpFBrush	= Brushes.Green;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Buy Fill", Description = "", GroupName = "Arrows", Order = 20)]
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
		[Display(ResourceType = typeof(Custom.Resource), Name = "Sell Fill", Description = "", GroupName = "Arrows", Order = 22)]
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
        [Display(ResourceType = typeof(Custom.Resource), Name = "Buy Outline", Description = "", GroupName = "Arrows", Order = 21)]
        public Stroke ArrowUpStroke
        {
            get { return pArrowUpStroke; }
            set { pArrowUpStroke = value; }
        }

        private Stroke pArrowDownStroke = new Stroke(Brushes.DarkRed, DashStyleHelper.Solid, 1);
        [Display(ResourceType = typeof(Custom.Resource), Name = "Sell Outline", Description = "", GroupName = "Arrows", Order = 23)]
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
        [Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", Description = "", GroupName = "Labels", Order = 1)]
        public bool LabelsEnabled
        {
            get { return pLabelsEnabled; }
            set { pLabelsEnabled = value; }
        }			
		
		private SimpleFont pTextFont = new SimpleFont("Arial", 11);
		[Display(ResourceType = typeof(Custom.Resource), Name = "Font", Description = "", GroupName = "Labels", Order = 2)]
		public SimpleFont TextFont
        {
            get { return pTextFont; }
            set { pTextFont = value; }
        }	
		
		
		private string pLabelBuy = "Buy";	
		[Display(ResourceType = typeof(Custom.Resource), Name = "Buy Label", Description = "", GroupName = "Labels", Order = 3)]
        public string LabelBuy
        {
            get { return pLabelBuy; }
            set { pLabelBuy = value; }
        }		
		
		private string pLabelSell = "Sell";
		[Display(ResourceType = typeof(Custom.Resource), Name = "Sell Label", Description = "", GroupName = "Labels", Order = 4)]
        public string LabelSell
        {
            get { return pLabelSell; }
            set { pLabelSell = value; }
        }	

		// BACKGROUND
		
        private bool pBackEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", Description = "", GroupName = "Background", Order = 1)]
        public bool BackEnabled
        {
            get { return pBackEnabled; }
            set { pBackEnabled = value; }
        }	
		
        private bool pColorAll = true;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Color All", Description = "", GroupName = "Background", Order = 2)]
        public bool ColorAll
        {
            get { return pColorAll; }
            set { pColorAll = value; }
        }		
		
		
		// BUY COLOR
		
		private System.Windows.Media.Brush	pBrush01 = Brushes.Green;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Buy Color", Description = "", GroupName = "Background", Order = 3)]
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

		private int	pOpacity01 = 20;
		[Range(0, 100)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Buy Opacity (%)", Description = "", GroupName = "Background", Order = 4)]
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
		[Display(ResourceType = typeof(Custom.Resource), Name = "Sell Color", Description = "", GroupName = "Background", Order = 5)]
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

		private int	pOpacity02 = 20;
		[Range(0, 100)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Sell Opacity (%)", Description = "", GroupName = "Background", Order = 6)]
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
		
	
		// EMAIL

		
		private bool pEmailEnabled = false;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", GroupName = "Email", Order = 1)]
        public bool EmailEnabled
        {
            get { return pEmailEnabled; }
            set { pEmailEnabled = value; }
        }	

		private string pEmailAddress = @"";
		[Display(ResourceType = typeof(Custom.Resource), Name = "Email Address", GroupName = "Email", Order = 3)]
        public string EmailAddress
        {
            get { return pEmailAddress; }
            set { pEmailAddress = value; }
        }
		
		private bool pQuickEmail = true;
		[Display(ResourceType = typeof(Custom.Resource), Name = "IntraBar", GroupName = "Email", Order = 2)]
        public bool QuickEmail
        {
            get { return pQuickEmail; }
            set { pQuickEmail = value; }
        }
		
		
		
	
		private string pLicensingEmailAddress = "";
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "License", Name = "Email Address", Order = 54, Description = "")]
        public string LicensingEmailAddress
        {
            get { return pLicensingEmailAddress; }
            set { pLicensingEmailAddress = value; }
        }			
		
					
					
		
				
		
		
		
		
		[Browsable(false)]
		[XmlIgnore]
		public Series<double> ConversionLine
		{
			get { return Values[0]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> BaseLine
		{
			get { return Values[1]; }
		}

//		[Browsable(false)]
//		[XmlIgnore]
//		public Series<double> SpanALine
//		{
//			get { return Values[2]; }
//		}

//		[Browsable(false)]
//		[XmlIgnore]
//		public Series<double> SpanBLine
//		{
//			get { return Values[3]; }
//		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> LagLine
		{
			get { return Values[2]; }
		}
		
		[Browsable(false)]
		[XmlIgnore]
		public Series<double> Signals
		{
			get { return Values[3]; }
		}
			
		
		
		
		
		
		
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private aiSIGIchimokuCloud[] cacheaiSIGIchimokuCloud;
		public aiSIGIchimokuCloud aiSIGIchimokuCloud(int conversion, int bBase, int spanB)
		{
			return aiSIGIchimokuCloud(Input, conversion, bBase, spanB);
		}

		public aiSIGIchimokuCloud aiSIGIchimokuCloud(ISeries<double> input, int conversion, int bBase, int spanB)
		{
			if (cacheaiSIGIchimokuCloud != null)
				for (int idx = 0; idx < cacheaiSIGIchimokuCloud.Length; idx++)
					if (cacheaiSIGIchimokuCloud[idx] != null && cacheaiSIGIchimokuCloud[idx].Conversion == conversion && cacheaiSIGIchimokuCloud[idx].BBase == bBase && cacheaiSIGIchimokuCloud[idx].SpanB == spanB && cacheaiSIGIchimokuCloud[idx].EqualsInput(input))
						return cacheaiSIGIchimokuCloud[idx];
			return CacheIndicator<aiSIGIchimokuCloud>(new aiSIGIchimokuCloud(){ Conversion = conversion, BBase = bBase, SpanB = spanB }, input, ref cacheaiSIGIchimokuCloud);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.aiSIGIchimokuCloud aiSIGIchimokuCloud(int conversion, int bBase, int spanB)
		{
			return indicator.aiSIGIchimokuCloud(Input, conversion, bBase, spanB);
		}

		public Indicators.aiSIGIchimokuCloud aiSIGIchimokuCloud(ISeries<double> input , int conversion, int bBase, int spanB)
		{
			return indicator.aiSIGIchimokuCloud(input, conversion, bBase, spanB);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.aiSIGIchimokuCloud aiSIGIchimokuCloud(int conversion, int bBase, int spanB)
		{
			return indicator.aiSIGIchimokuCloud(Input, conversion, bBase, spanB);
		}

		public Indicators.aiSIGIchimokuCloud aiSIGIchimokuCloud(ISeries<double> input , int conversion, int bBase, int spanB)
		{
			return indicator.aiSIGIchimokuCloud(input, conversion, bBase, spanB);
		}
	}
}

#endregion
