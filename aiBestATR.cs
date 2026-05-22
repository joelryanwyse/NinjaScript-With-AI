// 
// Copyright (C) 2022, Affordable Indicators <www.affordableindicators.com>.
// Affordable Indicators reserves the right to modify or overwrite this NinjaScript component with each release.
//




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
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.DrawingTools;
using NinjaTrader.Gui.Tools;

using SharpDX.DirectWrite;

using System.Text.RegularExpressions;
using System.IO;
using System.Net;
using System.Text;
using System.IO;
using System.Globalization;


using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Net;





//This namespace holds Indicators in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Indicators
{
	
	[Gui.CategoryOrder("Parameters", 1)]
	
	[Gui.CategoryOrder("Multi Time Frame", 5)]
	
	
	[Gui.CategoryOrder("History", 10)]

	
	[Gui.CategoryOrder("Plots", 100)]

   
	
	[Gui.CategoryOrder("Visual", 156)]
	[Gui.CategoryOrder("Data Series", 165)]
	
	
		
	
	
	[Gui.CategoryOrder("Setup", 9000)]
	[Gui.CategoryOrder("License", 10000)]
	
	
	
	
	[TypeConverter("NinjaTrader.NinjaScript.Indicators.aiBestATRConverter")]		
	public class aiBestATR : Indicator
	{
				
	private string ThisName = "aiBestATR";
						
			
		private bool RunInIt = true;
		private int BarCount, BarsToSkip, DayCount = 0;
		
		SortedDictionary<DateTime, double> BarToValue = new SortedDictionary<DateTime, double>();
		private int LatestHighBar, LatestLowBar = 0;
		private struct DateToInd {   //LIST
			public DateTime Date;
			public double IndValue;

			
			public DateToInd(DateTime date, double indValue) {this.Date = date; this.IndValue = indValue;}
		}
		private List<DateToInd> All;				
		private List<int> pivotlows;
		private List<int> pivothighs;		
		private bool LowPivotPending, HighPivotPending = false;
		Dictionary<int, double> LowPivots = new Dictionary<int, double>();
		Dictionary<int, double> HighPivots = new Dictionary<int, double>();
		
		private Series<double> 	MyATR;
		private Series<double> 	MyTR;
		
		private ATR iATR;
		
	
		DateTime FutureStartDate = DateTime.MinValue;
		DateTime LastDate = DateTime.MinValue;
		
		private double MaxV = 0;
		private double MinV = 1000000;
		
		private int RightEdgeBar = 0;
		

		SortedDictionary<DateTime, Entry> DayToValue = new SortedDictionary<DateTime, Entry>();
					
		private struct Entry
		{

			public DateTime StartDate;
			public double Plot1;
			public double Plot2;
			public double Plot3;
			public double Plot4;
			public double Plot5;
			public double Plot6;
			public double Plot7;
			public double Plot8;
			public double Plot9;			
			public double Plot10;		
			
			public Entry(DateTime startDate, double plot1, double plot2, double plot3, double plot4, double plot5, double plot6, double plot7, double plot8, double plot9, double plot10 ) {this.StartDate = startDate; this.Plot1 = plot1; this.Plot2 = plot2; this.Plot3 = plot3; this.Plot4 = plot4; this.Plot5 = plot5; this.Plot6 = plot6; this.Plot7 = plot7; this.Plot8 = plot8; this.Plot9 = plot9; this.Plot10 = plot10;}
		}
				
       	List<Entry> entries = new List<Entry>();
		
		
		

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
		

		
		SharpDX.Direct2D1.AntialiasMode oldAntialiasMode;
		
        private SharpDX.Direct2D1.Brush ChartTextBrushDX = null;
		private SharpDX.Direct2D1.Brush ChartBackgroundBrushDX = null;
		private SharpDX.Direct2D1.Brush ChartBackgroundErrorBrushDX = null;
		private SharpDX.Direct2D1.Brush ThisBrushDX = null;
		
		
		
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
			
			
			
//			if (pLicensingEmailAddress == "joelwyse@gmail.com" && machineid == "D403D027B4528B772422B8E882D14882")
//				return true;			
//			if (pLicensingEmailAddress == "koehlerd63@gmail.com" && machineid == "C32AB233848A15D97E70DA974DF014CC")
//				return true;
//			if (pLicensingEmailAddress == "koehlerd63@gmail.com" && machineid == "B9811B980E602BD8E79BD65C7B0CF5DE")
//				return true;			
			
			
			
			
			// test specific user 
			
//			pLicensingEmailAddress = "nextgentrader@hotmail.com";
//			machineid = "6953F65ADDF5B541F8FDE34DD1AFF193";
			
//			pLicensingEmailAddress = "markapl87@gmail.com";
//			machineid = "C75AF30F684FB30C457817ADB133177D";			
			
			
			
			
			
					
			
			
			
			List<int> ThisProductMainIDs = new List<int>();
			List<int> ThisProductSecondaryIDs = new List<int>();
			
			// Product IDs for Main Indicator
			
			ThisProductMainIDs.Add(514361);
			//ThisProductMainIDs.Add(503567); // support and resistance suite
			
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
						//LicensingMessage = "Your Affordable Indicators, Inc. account has been suspended. Contact " + pContactEmail + " to resolve the issue. Thanks!";
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

						if (pLicensingEmailAddress == "")
						{
							LicensingMessage = "Please enter the 'Email Address' connected to your Affordable Indicators, Inc. account in the 'License' section at the bottom of the " + ThisName + " indicator settings.";
						}
						else if (!pLicensingEmailAddress.Contains(@"@"))
						{
							LicensingMessage = "Please enter the 'Email Address' connected to your Affordable Indicators, Inc. account in the 'License' section at the bottom of the " + ThisName + " indicator settings.";
						}
						else
						{
							LicensingMessage = "Your 'Email Address' was not found in the Affordable Indicators, Inc. database.  Please enter the 'Email Address' connected to your Affordable Indicators, Inc. account in the 'License' section at the bottom of the " + ThisName + " indicator settings.  Contact " + pContactEmail + " if you need further assistance.";
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

		
		
		
		
		private int BarsMinutePeriod = 0;
		
		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description				= ""; //NinjaTrader.Custom.Resource.NinjaScriptIndicatorDescriptionPivots;
				Name					= "aiBestATR";
				Calculate				= Calculate.OnPriceChange;
				DisplayInDataBox		= true;
				DrawOnPricePanel		= true;
				IsAutoScale				= false;
				IsOverlay				= false;
				PaintPriceMarkers		= true;
				ScaleJustification		= ScaleJustification.Right;
				IsSuspendedWhileInactive		= true;
				MaximumBarsLookBack				= MaximumBarsLookBack.Infinite;				
				
				AddPlot(new Stroke(Brushes.Silver, 1),	PlotStyle.Line,	"ATR");
				AddPlot(new Stroke(Brushes.DimGray, 1),	PlotStyle.Line,	"History");
				
                Plots[0].DashStyleHelper = DashStyleHelper.Solid;
                Plots[1].DashStyleHelper = DashStyleHelper.Dash;

				ColorBySlope			= false;
				SlopeUpBrush			= Brushes.DodgerBlue;
				SlopeDownBrush			= Brushes.Red;
				
				
//				AddPlot(Brushes.WhiteSmoke, "ATR");
//				AddPlot(Brushes.Silver, "History");
//				AddPlot(Brushes.DimGray, "Max");
//				AddPlot(Brushes.DimGray, "Min");
				

				
				//TextFont						= new SimpleFont("Arial",11);
				
			}
			else if (State == State.Configure)
			{
				//AddDataSeries(BarsPeriodType.Minute, 1);
				
	
				if (pMTFEnabled)
				{
					
					if (FinalBasePeriodType1 == BarsPeriodType.Renko)
						AddRenko(Instrument.FullName, pMTFBarsPeriod1, MarketDataType.Last);
					else
						AddDataSeries(FinalBasePeriodType1, pMTFBarsPeriod1);
				}				
				
			}
			else if (State == State.DataLoaded)
			{
				
	
				if (BarsArray[0].BarsPeriod.BarsPeriodType == BarsPeriodType.Minute)
				{
				
					BarsMinutePeriod = BarsArray[0].BarsPeriod.Value;
					
					//Print(BarsMinutePeriod);
					
				}
				
				
				
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
				
					
					
					
					
				MyATR  = new Series<double>(this, MaximumBarsLookBack.Infinite);
				MyTR   = new Series<double>(this, MaximumBarsLookBack.Infinite);

				if (pSlopeUpBrush != null && pSlopeUpBrush.CanFreeze && !pSlopeUpBrush.IsFrozen)
					pSlopeUpBrush.Freeze();
				if (pSlopeDownBrush != null && pSlopeDownBrush.CanFreeze && !pSlopeDownBrush.IsFrozen)
					pSlopeDownBrush.Freeze();
				
			All = new List<DateToInd>();  //LIST			
			
			pivotlows = new List<int>();
			pivothighs = new List<int>();
			
			iATR = ATR(pATRPeriod);
				
				
				//ChartName = Instrument.MasterInstrument.Name + " | " + ChartBars.Bars.BarsPeriod.ToString();
				
				string append = "IB";
			
		
				
				//Plots[1].BrushDX.Opacity = 0.2F;
			}
			
			else if (State == State.Historical)
			{
				//storedSession = new SessionIterator(Bars);
			}
		}

		double lasthit = 0;
		
		double thisvvvv = 0;
				double high0	= 0;
				double low0		= 0;
			double close1		= 0;
					double trueRange = 0;
			
			
        protected override void OnBarUpdate()
        {

			
				if (!Permission)
					return;
				
			RightEdgeBar = CurrentBars[0];
			FutureStartDate = Times[0][0];
				
				if (RunInIt)
			{
				
			}
			
			
			for(int i = 0; i <= BarsArray.Length-1; i++) // return for all bars on the chart if there isn't a bar
				if (CurrentBars[i] < 1)
					return;
				
				
	
			if (pMTFEnabled)
			{
				high0 = Math.Max(high0, Highs[0][0]);
				low0 = Math.Min(low0, Lows[0][0]);
				
				trueRange	= Math.Max(Math.Abs(low0 - close1), Math.Max(high0 - low0, Math.Abs(high0 - close1)));
				thisvvvv   = ((Math.Min(CurrentBar + 1, pATRPeriod) - 1 ) * ATR(BarsArray[1], pATRPeriod).Value[1] + trueRange) / Math.Min(CurrentBar + 1, pATRPeriod);				
					
			
			}
							
			
			if (BarsInProgress == 1)		
			{
				
				high0	= Highs[1][0];
				low0		= Lows[1][0];

				if (CurrentBars[1] == 0)
					thisvvvv = high0 - low0;
				else
				{
					close1		= Closes[1][1];
					trueRange	= Math.Max(Math.Abs(low0 - close1), Math.Max(high0 - low0, Math.Abs(high0 - close1)));
					thisvvvv   = ((Math.Min(CurrentBar + 1, pATRPeriod) - 1 ) * ATR(BarsArray[1], pATRPeriod).Value[1] + trueRange) / Math.Min(CurrentBar + 1, pATRPeriod);
				}				
				
				
				high0 = 0;
				low0 = 9999999999999;
				
				return;
				
			}
			
		

			if (!pMTFEnabled)
			{
				MyTR[0] = Math.Max(High[0] - Low[0], Math.Max(Math.Abs(High[0] - Close[1]), Math.Abs(Low[0] - Close[1])));

				if (pATRSmoothing == "SMA")
					thisvvvv = SMA(MyTR, pATRPeriod)[0];
				else if (pATRSmoothing == "EMA")
					thisvvvv = EMA(MyTR, pATRPeriod)[0];
				else
					thisvvvv = iATR[0];
			}

			
				if (pATRDMode == "Points")
					MyATR[0] = thisvvvv;
				else
					MyATR[0] = Math.Round(thisvvvv/TickSize,1);
		
				
				
				
			
			Values[0][0] = MyATR[0];

			if (pColorBySlope && CurrentBar > 0)
				PlotBrushes[0][0] = Values[0][0] >= Values[0][1] ? pSlopeUpBrush : pSlopeDownBrush;
			 
			BarCount = BarCount+1;
			if (Bars.IsFirstBarOfSession)
			{
				
				BarsToSkip = (int) Math.Round(BarCount*0.95,0);
				//Print(BarCount + "    " + BarsToSkip);
				BarCount = 0;
				DayCount = DayCount + 1;
				
			}
			
			 
			
			if (IsFirstTickOfBar && CurrentBar > 0)
			{
					
				bool oldcode = false;
				
				if (oldcode)
				{
				
						if (LowPivotPending)
						{
							LatestLowBar = CurrentBar - 1 - pStrength;
							pivotlows.Add(LatestLowBar);
							LowPivots.Add(LatestLowBar,Timing[CurrentBar - LatestLowBar]);
						}
						if (HighPivotPending)
						{
							LatestHighBar = CurrentBar - 1 - pStrength;
							pivothighs.Add(LatestHighBar);
							HighPivots.Add(LatestHighBar,Timing[CurrentBar - LatestHighBar]);
						}
				
				}
						
				int count = 0;
				double total = 0;
						
						double max = 0;
						double min = 99999999;
//				for (int i=All.Count-1; i > 0; i--)  //loop through all lines and draw them
//				{	
//					if (Time[0].Hour == All[i].Date.Hour && Time[0].Minute == All[i].Date.Minute)
//					{
//						count = count + 1;
//						total = total + All[i].IndValue;
//						i = i - BarsToSkip;
//						if (count == pDaysBack)
//						{
//							TimingPlot.Set(0,total/pDaysBack);
//							break;
//						}
//					
//					}
//				}
				
				double t = 0;
				
						
						
				if (!DayToValue.ContainsKey(Times[0][0]))
					DayToValue.Add(Times[0][0],new Entry(Times[0][0],Values[0][0],0,0,0,0,0,0,0,0,0));
					
						
						
						
				//if (DayCount > pDaysBack)
				//for (int i=1; i<=pDaysBack*2;i++)
					
				
				if (pHistoryEnabled)
				for (int i=1; i<=1;i++)	
				{
					//Print(i);
					bool processit = false;
					
					if (pTimerDMode == "Day")
						processit = BarToValue.TryGetValue(Time[0].AddDays(-i),out t);
					else
						processit = BarToValue.TryGetValue(Time[0].AddDays(-i*7),out t);
					
					//if (BarToValue.TryGetValue(Time[0].AddDays(-i),out t))  // every day
					
					if (processit)
					//if (BarToValue.TryGetValue(Time[0].AddDays(-i*7),out t))  // last week
					{
						int barn = Bars.GetBar(Time[0].AddDays(-i*7));
						int bb = CurrentBars[0] - barn;
						//t = Math.Round(ATR(pATRPeriod)[bb]/TickSize,1);
						//t = 5;
						max = Math.Max(max,t);
						min = Math.Min(min,t);
						count = count + 1;
						total = total + t;
						
						//Print(Time[0].AddDays(-i));
						
					}
//					if (BarToValue.TryGetValue(Time[0].AddDays(-i),out t))
//					{
//						count = count + 1;
//						total = total + t;
//					}			
					
					if (t != 0)
					Timing[0] = t;
					
					
						if (count == pDaysBack)
						{
							Timing[0] = total/pDaysBack;
							
							if (pHighLowEnabled)
							{
								//if (max != Timing[0])
								Values[1][0] = max;
								
								//if (min != Timing[0])
								Values[2][0] = min;
							}
							
							break;
						}


				}
				//Print(t);
				
				
				// MATCH VALUES WITH TIME
//				
//				double valuetoadd = MACD(pMACDFast,pMACDSlow,pMACDSmooth)[1];
//				double valuetoadd = VOL()[1];
//				double valuetoadd = Stochastics(3,5,1)[1];
				
				
				double valuetoadd = MyATR[0];
				
			
				//All.Add(new DateToInd(Time[0],valuetoadd));
				if (!BarToValue.ContainsKey(Time[0]))
					BarToValue.Add(Time[0],valuetoadd);
			}
			
					
			
			if (CurrentBar <= pStrength*2)
						return;			
								// find low and high pivots based on strength
						
					LowPivotPending = true;
					HighPivotPending = true;
					for ( int c=0;c<=pStrength-1;c++)  // low right side
					{
						
						if (Timing[c] < Timing[pStrength])
						{
							LowPivotPending = false;
							break;
						}

					}
					for ( int c=pStrength+1;c<=pStrength+pStrength;c++) 	// low left side
					{
						if (Timing[c] <= Timing[pStrength])
						{
							LowPivotPending = false;
							break;
						}						
						
					}
					for ( int c=0;c<=pStrength-1;c++)	// high right side
					{
						
						if (Timing[c] > Timing[pStrength])
						{
							HighPivotPending = false;
							break;
						}

					}
					for ( int c=pStrength+1;c<=pStrength+pStrength;c++)	// high left side
					{
						if (Timing[c] >= Timing[pStrength])
						{
							HighPivotPending = false;
							break;
						}						
						
					}
			
		
			
			
        }
		

		
//		protected override void OnRender(ChartControl chartControl, ChartScale chartScale)
//		{

//			if (!IsVisible)
//				return;
			
//			if(Bars == null || Bars.Instrument == null || IsInHitTest) { return; }
						
//			ChartControlProperties myProperties = chartControl.Properties;

//			// Default plotting in base class. Uncomment if indicators holds at least one plot - so in this case we would expect not to see the SMA plot we have as well in this sample script
//			//base.OnRender(chartControl, chartScale);

//			// RenderTarget is always full panel, so we need to be mindful which sub ChartPanel we're dealing with
//			// always use ChartPanel X, Y, W, H - as ActualWidth, Width, ActualHeight, Height are in WPF units, so they can be drastically different depending on DPI set
			
//			Point startPoint	= new Point(ChartPanel.X, ChartPanel.Y);
//			Point endPoint		= new Point(ChartPanel.X + ChartPanel.W, ChartPanel.Y + ChartPanel.H);
//			Point endPoint2		= new Point(ChartPanel.X + ChartPanel.W, ChartPanel.Y + ChartPanel.H);			
						
//			Point startPoint1	= new Point(ChartPanel.X, ChartPanel.Y + ChartPanel.H);
//			Point endPoint1		= new Point(ChartPanel.X + ChartPanel.W, ChartPanel.Y);

//			Point nextPoint		= new Point(ChartPanel.X + ChartPanel.W, ChartPanel.Y);
			
//			double width		= endPoint.X - startPoint.X;
//			double height		= endPoint.Y - startPoint.Y;

//			TextFormat	textFormat			= TextFont.ToDirectWriteTextFormat();	
			
			
			
////			TextFormat textFormat		= new TextFormat(Core.Globals.DirectWriteFactory, "Calibri", SharpDX.DirectWrite.FontWeight.Normal,
////															SharpDX.DirectWrite.FontStyle.Normal, SharpDX.DirectWrite.FontStretch.Normal, fontHeight) 
////															{ TextAlignment = SharpDX.DirectWrite.TextAlignment.Leading, WordWrapping = WordWrapping.NoWrap };
		

		
			
//			SharpDX.Direct2D1.Brush			areaBrushDx			= areaBrush.ToDxBrush(RenderTarget);
//			SharpDX.Direct2D1.Brush			smallAreaBrushDx	= smallAreaBrush.ToDxBrush(RenderTarget);
//			SharpDX.Direct2D1.Brush			textBrushDx			= textBrush.ToDxBrush(RenderTarget);
//			SharpDX.Direct2D1.Brush			lineBrushDx			= textBrush.ToDxBrush(RenderTarget);
			
			
			
			
			
//			SharpDX.Direct2D1.AntialiasMode oldAntialiasMode	= RenderTarget.AntialiasMode;
//			RenderTarget.AntialiasMode							= SharpDX.Direct2D1.AntialiasMode.Aliased;
		
			
			
//			// Set text to chart label color and font
//			//textFormat			= chartControl.Properties.LabelFont.ToDirectWriteTextFormat();

////			// Loop through each Plot Values on the chart
			
//			//if (pActive2)
//			for (int seriesCount = 0; seriesCount < Values.Length; seriesCount++)
//			{
				
////				if (seriesCount <= 3 && !pShowToday)
////					continue;
				
////				if (seriesCount > 3 && !pShowYesterday)
////					continue;				
		
//				double	y					= -1;
//				double	startX				= -1;
//				double	endX				= -1;
//				int		firstBarIdxToPaint	= -1;
//				int		firstBarPainted		= ChartBars.FromIndex;
//				int		lastBarPainted		= ChartBars.ToIndex;
//				Plot	plot				= Plots[seriesCount];

//				//blueBrush = BrushSeries[seriesCount];
				
//				//lineBrushDx = Plots[0].DashStyleDX;
				
				
//				smallAreaBrush	= plot.Brush;
//				smallAreaBrushDx	= smallAreaBrush.ToDxBrush(RenderTarget);
//				smallAreaBrushDx.Opacity = areaOpacity/100F;
				
				
				
//				int FB = ChartBars.FromIndex;
//				int LB = ChartBars.ToIndex;
				
//				preval = 0;
				
//				//Print("===== " + lastBarPainted + "   " + CurrentBars[0]);
				
//				lastBarPainted		= ChartBars.ToIndex;
				
//				FirstOne = true;
				
//				// Loop through visble bars to render plot values	
				
//				int startbar = firstBarPainted;
//				//startbar = 0;
				
				
				
//				DateTime StartBarTime = Time.GetValueAt(FB);
//				DateTime EndBarTime = Time.GetValueAt(LB);
				
				
//				DateTime StartDrawTime = StartBarTime.AddDays(-4);
				
//				//if (StartBarTime.Ticks <= EndBarTime.Ticks)
//				//	EndBarTime = StartBarTime.AddDays(-2);
				
				
//				startbar = Math.Max(0,Bars.GetBar(StartDrawTime));
				
//				//Print(EndBarTime);
				
//				//int startbar = GetValueAt(
				
				
//				int endbar = lastBarPainted;
//				//endbar = CurrentBars[0];
				
//				//startbar = endbar;
				
				
//				for (int idx = endbar; idx >= startbar; idx--)
//				//for (int idx = endbar; idx >= Math.Max(startbar, endbar - width); idx--)
//				{
					
//					//Print(idx);
					
//					//if (idx < firstBarIdxToPaint)
//					//	break;

//					int adjust = 1;
					
//					int barsadjust = 1;
					
//					startX		= chartControl.GetXByBarIndex(ChartBars, idx - barsadjust) - adjust;
//					endX		= chartControl.GetXByBarIndex(ChartBars, idx + 1 - barsadjust) - adjust;
					
					
//					if (idx == lastBarPainted)
//					{
//						//Print(idx + "  " + endX);
//						endX		= endX+20;
//						//Print(endX);
//					}
						
//					preval = val;
//					val	= Values[seriesCount].GetValueAt(idx);
//					y			= chartScale.GetYByValue(val);
					
//					bool c1 = idx == lastBarPainted;
					
					
//					if (Calculate == Calculate.OnBarClose)
//						c1 = idx == lastBarPainted - 1;
					
					
//					bool c2 = preval != val && preval != 0;
//					c2 = preval != val;
					
					
//					if (c1)
//						//endX = endX + myProperties.BarMarginRight;
//						endX = endX + ChartPanel.W + 0;
						
						
//					// Draw pivot lines
//					startPoint	= new Point(startX, y);
//					endPoint		= new Point(endX, y);

//					//RenderTarget.DrawLine(startPoint.ToVector2(), endPoint.ToVector2(), plot.BrushDX , plot.Width, plot.StrokeStyle);
//					SharpDX.RectangleF			rect2			= new SharpDX.RectangleF((float)startX,(float)y-pShadowWidth-1,(float)endX-(float)startX,pShadowWidth*2+1);
//					//RenderTarget.FillRectangle(rect2, smallAreaBrushDx);	
					
//					// line moved so draw labels

//					if (c1 || c2)
//					{
						
						
						
//						startPoint	= new Point(endX, 0);
//						endPoint	= new Point(endX, 2000);
//						//RenderTarget.DrawLine(startPoint.ToVector2(), endPoint.ToVector2(), lineBrushDx , 1);					
						
						
						
//						startPoint	= new Point(chartControl.GetXByBarIndex(ChartBars, idx+1), y);
//						endPoint		= new Point(endX, y);
						
						
								
//						//if (val == 0)
						
//						if (pLabelsEnabled)
//						if (val != 0)
//						{
						
//							string pp = plot.Name;
													
////							if (seriesCount == 0)
////								pp = "U4";
////							if (seriesCount == 1)
////								pp = "U3";
////							if (seriesCount == 2)
////								pp = "U2";
////							if (seriesCount == 3)
////								pp = "U1";						
////							if (seriesCount == 4)
////								pp = "BP";	
////							if (seriesCount == 5)
////								pp = "L1";
////							if (seriesCount == 6)
////								pp = "L2";
////							if (seriesCount == 7)
////								pp = "L3";
////							if (seriesCount == 8)
////								pp = "L4";	
							
							
//							TextLayout textLayout = new TextLayout(Globals.DirectWriteFactory, pp, textFormat, 1000, textFormat.FontSize);

//							double newy = y-textLayout.Metrics.Height-3;
							
//							// text is on a previous line
//							endPoint2 = new Point(startPoint.X - textLayout.Metrics.Width - 4 - pRightPX, newy);
							
//							if (c1) // text is on right edge
//								endPoint2 = new Point(ChartPanel.W - textLayout.Metrics.Width - 4 - pRightPX, newy); 
							
							
//							//if (c1) // only show text on right edge, delete to show text on all lines
//							RenderTarget.DrawTextLayout(endPoint2.ToVector2(), textLayout, plot.BrushDX);

//							textLayout.Dispose();						
//						}
						
				
						
//						if (preval != 0)
//						if (!FirstOne)
//						{
							

							
//							nextPoint.Y = prey;
//							endPoint.Y = prey;
						
						
//							RenderTarget.DrawLine(nextPoint.ToVector2(), endPoint.ToVector2(),  plot.BrushDX , plot.Width, plot.StrokeStyle);
						
											
//								float xxxwid = (float) endPoint.X - (float) nextPoint.X;
								
//								rect2			= new SharpDX.RectangleF((float)nextPoint.X,(float)endPoint.Y-pShadowWidth-1,xxxwid,pShadowWidth*2+1);
					
								
								
//								RenderTarget.FillRectangle(rect2, smallAreaBrushDx);	
//						}
						
						
							
						
						
						
//						nextPoint = endPoint;
						
//						FirstOne = false;
						
//					}
					
					
//					prey = y;

					
					
//				}



//				// Draw pivot text
				
				

//			}
				
//			textFormat.Dispose();
			

//			areaBrushDx.Dispose();
//			smallAreaBrushDx.Dispose();
//			lineBrushDx.Dispose();
//			textBrushDx.Dispose();
			
//			RenderTarget.AntialiasMode = oldAntialiasMode;
			
			
		
				
//		}

		
		
//				protected override void OnRender(ChartControl chartControl, ChartScale chartScale)
//		{

			
//			if (!IsVisible)
//				return;
			
//			//Print(BarsInProgress);
			
//			//if (BarsInProgress > 0)
//			//	return;
			
//			ChartControlProperties myProperties = chartControl.Properties;

//			// Default plotting in base class. Uncomment if indicators holds at least one plot - so in this case we would expect not to see the SMA plot we have as well in this sample script
//			//base.OnRender(chartControl, chartScale);

//			//return;
			
//			//return;
			
//			// RenderTarget is always full panel, so we need to be mindful which sub ChartPanel we're dealing with
//			// always use ChartPanel X, Y, W, H - as ActualWidth, Width, ActualHeight, Height are in WPF units, so they can be drastically different depending on DPI set
			
//			Point startPoint	= new Point(ChartPanel.X, ChartPanel.Y);
//			Point endPoint		= new Point(ChartPanel.X + ChartPanel.W, ChartPanel.Y + ChartPanel.H);
//			Point endPoint2		= new Point(ChartPanel.X + ChartPanel.W, ChartPanel.Y + ChartPanel.H);			
						
//			Point startPoint1	= new Point(ChartPanel.X, ChartPanel.Y + ChartPanel.H);
//			Point endPoint1		= new Point(ChartPanel.X + ChartPanel.W, ChartPanel.Y);

//			Point nextPoint		= new Point(ChartPanel.X + ChartPanel.W, ChartPanel.Y);
			
//			double width		= endPoint.X - startPoint.X;
//			double height		= endPoint.Y - startPoint.Y;

//			TextFormat	textFormat			= TextFont.ToDirectWriteTextFormat();	
			
			
			
////			TextFormat textFormat		= new TextFormat(Core.Globals.DirectWriteFactory, "Calibri", SharpDX.DirectWrite.FontWeight.Normal,
////															SharpDX.DirectWrite.FontStyle.Normal, SharpDX.DirectWrite.FontStretch.Normal, fontHeight) 
////															{ TextAlignment = SharpDX.DirectWrite.TextAlignment.Leading, WordWrapping = WordWrapping.NoWrap };

//			SharpDX.Direct2D1.Brush			areaBrushDx			= areaBrush.ToDxBrush(RenderTarget);
//			SharpDX.Direct2D1.Brush			smallAreaBrushDx	= smallAreaBrush.ToDxBrush(RenderTarget);
//			SharpDX.Direct2D1.Brush			textBrushDx			= textBrush.ToDxBrush(RenderTarget);
//			SharpDX.Direct2D1.Brush			lineBrushDx			= textBrush.ToDxBrush(RenderTarget);

//			SharpDX.Direct2D1.AntialiasMode oldAntialiasMode	= RenderTarget.AntialiasMode;
//			RenderTarget.AntialiasMode							= SharpDX.Direct2D1.AntialiasMode.Aliased;
		
			
//			for (int seriesCount = 0; seriesCount < Values.Length; seriesCount++)
//			{
				

////				if (seriesCount <= 6 && !pShowToday)
////					continue;
				
////				if (seriesCount > 6 && !pShowYesterday)
////					continue;				
		
//				double	y					= -1;
//				double	startX				= -1;
//				double	endX				= -1;
//				int		firstBarIdxToPaint	= -1;

//				Plot	plot				= Plots[seriesCount];
				
//				smallAreaBrush	= plot.Brush;
//				smallAreaBrushDx	= smallAreaBrush.ToDxBrush(RenderTarget);
//				smallAreaBrushDx.Opacity = areaOpacity/100F;

//				int FB = ChartBars.FromIndex;
//				int LB = ChartBars.ToIndex;
				
//				preval = 0;

//				FirstOne = true;
				
//				int startbar = FB;
//				int endbar = LB;
				
//				for (int idx = endbar; idx >= startbar; idx--)
//				//for (int idx = endbar; idx >= Math.Max(startbar, endbar - width); idx--)
//				{
					
//					//Print(idx);
					
//					//if (idx < firstBarIdxToPaint)
//					//	break;

//					int adjust = 1;
					
//					int barsadjust = 1;
					
//					startX		= chartControl.GetXByBarIndex(ChartBars, idx - barsadjust) - adjust;
//					endX		= chartControl.GetXByBarIndex(ChartBars, idx + 1 - barsadjust) - adjust;
					
					
//					if (idx == LB)
//					{
//						//Print(idx + "  " + endX);
//						endX		= endX+20;
//						//Print(endX);
//					}
						
//					preval = val;
//					val	= Values[seriesCount].GetValueAt(idx);
//					y			= chartScale.GetYByValue(val);
					
//					bool c1 = idx == LB;
//					bool c2 = preval != val && preval != 0;
//					c2 = preval != val;
					
					
//					if (c1)
//						//endX = endX + myProperties.BarMarginRight;
//						endX = endX + ChartPanel.W + 0;
						
						
//					// Draw pivot lines
//					startPoint	= new Point(startX, y);
//					endPoint		= new Point(endX, y);

//					// DRAW A LINE BETWEEN EACH BAR.
					
//					RenderTarget.DrawLine(startPoint.ToVector2(), endPoint.ToVector2(), plot.BrushDX , plot.Width, plot.StrokeStyle);
//					SharpDX.RectangleF			rect2			= new SharpDX.RectangleF((float)startX,(float)y-pShadowWidth-1,(float)endX-(float)startX,pShadowWidth*2+1);
//					RenderTarget.FillRectangle(rect2, smallAreaBrushDx);	
					
//					// end
					
//					// line moved so draw labels

//					if (c1 || c2)
//					{
						
						
						
//						startPoint	= new Point(endX, 0);
//						endPoint	= new Point(endX, 2000);
//						//RenderTarget.DrawLine(startPoint.ToVector2(), endPoint.ToVector2(), lineBrushDx , 1);					
						
						
						
//						startPoint	= new Point(chartControl.GetXByBarIndex(ChartBars, idx+1), y);
//						endPoint		= new Point(endX, y);
						
						
								
//						//if (val == 0)
						
//						if (pLabelsEnabled)
//						if (val != 0)
//						{
						
//							TextLayout textLayout = new TextLayout(Globals.DirectWriteFactory, plot.Name, textFormat, 1000, textFormat.FontSize);

//							double newy = y-textLayout.Metrics.Height-3;
							
//							// text is on a previous line
//							endPoint2 = new Point(startPoint.X - textLayout.Metrics.Width - 4 - pRightPX, newy);
							
//							if (c1) // text is on right edge
//								endPoint2 = new Point(ChartPanel.W - textLayout.Metrics.Width - 4 - pRightPX, newy); 
							
							
//							RenderTarget.DrawTextLayout(endPoint2.ToVector2(), textLayout, plot.BrushDX);

//							textLayout.Dispose();						
//						}
						
				
//						// DRAW A LINE whenever the line changes
//						bool daw = false;
						
						
//						if (daw)
//						if (preval != 0)
//						if (!FirstOne)
//						{
							

							
//							nextPoint.Y = prey;
//							endPoint.Y = prey;
						
							
						
//							RenderTarget.DrawLine(nextPoint.ToVector2(), endPoint.ToVector2(),  plot.BrushDX , plot.Width, plot.StrokeStyle);
						
											
//								float xxxwid = (float) endPoint.X - (float) nextPoint.X;
								
//								rect2			= new SharpDX.RectangleF((float)nextPoint.X,(float)endPoint.Y-pShadowWidth-1,xxxwid,pShadowWidth*2+1);
					
								
								
//								RenderTarget.FillRectangle(rect2, smallAreaBrushDx);	
//						}
						
						
							
						
						
						
//						nextPoint = endPoint;
						
//						FirstOne = false;
						
//					}
					
					
//					prey = y;

					
					
//				}



//				// Draw pivot text
				
				

//			}
				
//			textFormat.Dispose();
			

//			areaBrushDx.Dispose();
//			smallAreaBrushDx.Dispose();
//			lineBrushDx.Dispose();
//			textBrushDx.Dispose();
			
//			RenderTarget.AntialiasMode = oldAntialiasMode;
			
			
		
				
//		}
	
		
		
		
//		public override string FormatPriceMarker(double price)
//		{
//			double trunc = Math.Truncate(price);
//			int fraction = 0;
//			string priceMarker = "";
//			if (TickSize == 0.03125) 
//			{
//				fraction = Convert.ToInt32(32 * Math.Abs(price - trunc));	
//				if (fraction < 10)
//					priceMarker = trunc.ToString() + "'0" + fraction.ToString();
//				else if(fraction == 32)
//				{	
//					trunc = trunc + 1;
//					fraction = 0;
//					priceMarker = trunc.ToString() + "'0" + fraction.ToString();
//				}	
//				else 
//					priceMarker = trunc.ToString() + "'" + fraction.ToString();
//			}
//			else if (TickSize == 0.015625)
//			{
//				fraction = 5 * Convert.ToInt32(64 * Math.Abs(price - trunc));	
//				if (fraction < 10)
//					priceMarker = trunc.ToString() + "'00" + fraction.ToString();
//				else if (fraction < 100)
//					priceMarker = trunc.ToString() + "'0" + fraction.ToString();
//				else if(fraction == 320)
//				{	
//					trunc = trunc + 1;
//					fraction = 0;
//					priceMarker = trunc.ToString() + "'00" + fraction.ToString();
//				}	
//				else	
//					priceMarker = trunc.ToString() + "'" + fraction.ToString();
//			}
//			else if (TickSize == 0.0078125)
//			{
//				fraction = Convert.ToInt32(Math.Truncate(2.5 * Convert.ToInt32(128 * Math.Abs(price - trunc))));	
//				if (fraction < 10)
//					priceMarker = trunc.ToString() + "'00" + fraction.ToString();
//				else if (fraction < 100)
//					priceMarker = trunc.ToString() + "'0" + fraction.ToString();
//				else if(fraction == 320)
//				{	
//					trunc = trunc + 1;
//					fraction = 0;
//					priceMarker = trunc.ToString() + "'00" + fraction.ToString();
//				}	
//				else	
//					priceMarker = trunc.ToString() + "'" + fraction.ToString();
//			}
//			else
//			{
//				priceMarker = price.ToString(NinjaTrader.Core.Globals.GetTickFormatString(TickSize));
//			}
//			return priceMarker;
//		}		
			
		
		// FutureHigh
			
			public override void OnCalculateMinMax()
			{
			  // make sure to always start fresh values to calculate new min/max values
			  double tmpMin = double.MaxValue;
			  double tmpMax = double.MinValue;
			 
			  // For performance optimization, only loop through what is viewable on the chart
			  for (int index = ChartBars.FromIndex; index <= ChartBars.ToIndex; index++)
			  {
			    // since using Close[0] is not guaranteed to be in sync
			    // retrieve "Close" value at the current viewable range index
			    double plotValue1 = Values[0].GetValueAt(index);
			 	double plotValue2 = Values[1].GetValueAt(index);
				  
				  
			    // return min/max of close value
			    tmpMin = Math.Min(tmpMin, plotValue1);
			    tmpMax = Math.Max(tmpMax, plotValue1);
				  
				  
				if (plotValue2 != 0)
				{
				  
				    tmpMin = Math.Min(tmpMin, plotValue2);
				    tmpMax = Math.Max(tmpMax, plotValue2);				  
				}
				  
			  }
			 
			 // Print(FutureHigh);
			  
			  
			  if (FutureHigh != 0)
			  {
				  
				  tmpMin = Math.Min(tmpMin, FutureLow);
				  tmpMax = Math.Max(tmpMax, FutureHigh);
			  }
			  
			  
			  // Finally, set the minimum and maximum Y-Axis values to +/- 50 ticks from the primary close value
			  MinValue = tmpMin;// - 50 * TickSize;
			  MaxValue = tmpMax;// + 50 * TickSize;
			}


		double FutureHigh = 0;
		double FutureLow = 999999999999;
			
			
        private void DrawFuturePlot(ChartControl chartControl, ChartScale chartScale, Series<double> data, int dataOffset, int plotNum)
        {
			
			FutureHigh = 0;
			FutureLow = 999999999999;
			
						
			//Print("drawfuturepolto");
			
			
			Point startPoint	= new Point(ChartPanel.X, ChartPanel.Y);
			Point endPoint		= new Point(ChartPanel.X + ChartPanel.W, ChartPanel.Y + ChartPanel.H);
			
			//return;
			
			
			//Series<double> data = alligatorValue1;
			//int dataOffset = alligatorOffset1;
			//int plotNum = 0;
			
			Plot	plot				= Plots[plotNum];
			SharpDX.Direct2D1.Brush			FinalBrushDX		= plot.Brush.ToDxBrush(RenderTarget);	
			
			int num = 0;
            int x = -1;
            double num3 = -1.0;
           // GraphicsPath path = new GraphicsPath();
			
			
			
			bool ShowBarsRequired = false; //base.ChartControl.ShowBarsRequired 
			
			
			DateTime ThisS2 = ChartBars.GetTimeByBarIdx(chartControl, ChartBars.ToIndex);
			DateTime NewT2 = new DateTime(ThisS2.Year, ThisS2.Month, ThisS2.Day, 0, 0, 0);
//			Print(NewT2);
//			Print(ChartBars.ToIndex);
			
//			Print("VB: " + ChartBars.ToIndex);
//			Print("CB: " + RightEdgeBar);
			
			bool dohistoryplot = false;
			
			int actualright = ChartBars.ToIndex + 10;
			
			if (dohistoryplot)
			
				
			
			//if (RightEdgeBar != actualright)
			//for (int i = actualright - dataOffset; i <= RightEdgeBar; i++)
				
            for (int i = ChartBars.FromIndex - dataOffset; i <= ChartBars.ToIndex+10; i++)
            {
				//Print(i);
				
				
				
                if ((((i >= 0) && (i < base.BarsArray[0].Count)) && (ShowBarsRequired || (i >= base.BarsRequiredToPlot))) )//&& (data.IsValidDataPointAt(i) ))
                {
					
					DateTime ThisS = ChartBars.GetTimeByBarIdx(chartControl, i);
					DateTime NewT = new DateTime(ThisS.Year, ThisS.Month, ThisS.Day, 0, 0, 0);
					
					NewT  = ThisS.AddMinutes(i);
					
					Entry eee = new Entry(DateTime.MinValue,0,0,0,0,0,0,0,0,0,0);
					DayToValue.TryGetValue(NewT, out eee);
					
					
                    double num5 = 0; //data.GetValueAt(i);
					
					num5 = eee.Plot1;
					
					
//					if (plotNum == 0)
//					if (eee.Plot1 != 0 && pC1Enabled)	
//						num5 = eee.Plot1;
					
//					if (plotNum == 1)
//					if (eee.Plot2 != 0 && pC2Enabled)	
//						num5 = eee.Plot2;
					
//					if (plotNum == 2)
//					if (eee.Plot3 != 0 && pC3Enabled)	
//						num5 = eee.Plot3;
					
					
                  //  int xByBarIdx = base.ChartControl.GetXByBarIdx(base.BarsArray[0], i + dataOffset);
					int xByBarIdx = chartControl.GetXByBarIndex(ChartBars, i + dataOffset);
					
					
                    if (x >= 0)
                    {
                       // int yByValue = base.ChartControl.GetYByValue(this, num3);
						int yByValue = chartScale.GetYByValue(num3);
						
						
                        //int y = base.ChartControl.GetYByValue(this, num5);
						int y = chartScale.GetYByValue(num5);
						

						
                        if (num3 == 0)
                        {
                            num++;
							

							
							
                           // path.AddLine(new Point(x, yByValue), new Point(xByBarIdx, y));
                        }
                        else
                        {
							
							startPoint	= new Point(x, yByValue);
							endPoint		= new Point(xByBarIdx, y);

							RenderTarget.DrawLine(startPoint.ToVector2(), endPoint.ToVector2(),  FinalBrushDX, plot.Width, plot.StrokeStyle);
							
                            num++;
                           // path.AddLine(path.PathPoints[path.PathPoints.Length - 1], (PointF) new Point(xByBarIdx, y));
                        }
                    }
                    x = xByBarIdx;
                    num3 = num5;
                }
            }
			
//        int sum = 0;
//        int i = 0;
//        do
//        {
//            sum += ids[i];
//            i++;
//        } while (i < 4);
		
		
			
			//Print("---------------------------------");
			//Print(FutureStartDate);
			//Print(LastDate);
			
			// FUTURE PLOT
			
			
			//Print(FutureStartDate);
			
			int FutureBar = 0;
			
			
			
			//Print(ThisS2);
			
			
			FutureStartDate = ThisS2;
			
			DateTime FutureDay = FutureStartDate.AddMinutes(0);
			
			
			
			int xByBarIdx2 = chartControl.GetXByBarIndex(ChartBars, ChartBars.ToIndex + FutureBar);
			
			double barww = ChartBars.Properties.ChartStyle.BarWidth;
			
			barww = chartControl.Properties.BarDistance;
			
			
			int numberofbarsinfuture = (int) Math.Ceiling((chartScale.Width - xByBarIdx2) / barww);
			
//			Print("xByBarIdx2 " + xByBarIdx2);
//			Print("ChartControl.Width " + chartScale.Width);
//			Print("barww " + barww);
//			Print("numberofbarsinfuture " + numberofbarsinfuture);
			
			
			
			
			int numberofbars = numberofbarsinfuture;
			int minutebarperiod = BarsMinutePeriod;
			
			int numberofminutes = numberofbars * minutebarperiod;
				
			LastDate = FutureStartDate.AddMinutes(numberofminutes);

			SharpDX.Direct2D1.AntialiasMode oldAntialiasMode2 = RenderTarget.AntialiasMode;
			RenderTarget.AntialiasMode = SharpDX.Direct2D1.AntialiasMode.PerPrimitive;
    		do
            {
				//Print(i);
				
				
				
                //if ((((i >= 0) && (i < base.BarsArray[0].Count)) && (ShowBarsRequired || (i >= base.BarsRequiredToPlot))) )//&& (data.IsValidDataPointAt(i) ))
                {
					
//					DateTime ThisS = ChartBars.GetTimeByBarIdx(chartControl, i);
//					DateTime NewT = new DateTime(ThisS.Year, ThisS.Month, ThisS.Day, 0, 0, 0);
					
					Entry eee = new Entry(DateTime.MinValue,0,0,0,0,0,0,0,0,0,0);
					DayToValue.TryGetValue(FutureDay.AddDays(-7), out eee);
					
					
					
                    double num5 = num3;
					
					
					if (num5 != 0 && num5 != -1)
					{
					
						//Print(num5);
						
						FutureHigh = Math.Max(FutureHigh, num5);
						FutureLow = Math.Min(FutureLow, num5);
						
						
					}
					
					
//					if (plotNum == 0)
//					if (eee.Plot1 != 0 && pC1Enabled)	
						
					if (eee.Plot1 != 0)
						num5 = eee.Plot1;
					
//					if (plotNum == 1)
//					if (eee.Plot2 != 0 && pC2Enabled)	
//						num5 = eee.Plot2;
					
//					if (plotNum == 2)
//					if (eee.Plot3 != 0 && pC3Enabled)	
//						num5 = eee.Plot3;
					
//					if (plotNum == 3)
//					if (eee.Plot4 != 0 && pC4Enabled)	
//						num5 = eee.Plot4;
					
//					if (plotNum == 4)
//					if (eee.Plot5 != 0 && pC5Enabled)	
//						num5 = eee.Plot5;
					
//					if (plotNum == 5)
//					if (eee.Plot6 != 0 && pC6Enabled)	
//						num5 = eee.Plot6;
					
//					if (plotNum == 6)
//					if (eee.Plot7 != 0 && pC7Enabled)	
//						num5 = eee.Plot7;
					
//					if (plotNum == 7)
//					if (eee.Plot8 != 0 && pC8Enabled)	
//						num5 = eee.Plot8;
					
//					if (plotNum == 8)
//					if (eee.Plot9 != 0 && pC9Enabled)	
//						num5 = eee.Plot9;
					
//					if (plotNum == 9)
//					if (eee.Plot10 != 0 && pC10Enabled)	
//						num5 = eee.Plot10;
					
					
					
                  //  int xByBarIdx = base.ChartControl.GetXByBarIdx(base.BarsArray[0], i + dataOffset);
					int xByBarIdx = chartControl.GetXByBarIndex(ChartBars, ChartBars.ToIndex + FutureBar);
					
					
                    if (x >= 0)
                    {
                       // int yByValue = base.ChartControl.GetYByValue(this, num3);
						int yByValue = chartScale.GetYByValue(num3);
						
						
                        //int y = base.ChartControl.GetYByValue(this, num5);
						int y = chartScale.GetYByValue(num5);
						

						
                        if (num3 == 0)
                        {
                            num++;
							

							
							
                           // path.AddLine(new Point(x, yByValue), new Point(xByBarIdx, y));
                        }
                        else
                        {
							
							startPoint	= new Point(x, yByValue);
							endPoint		= new Point(xByBarIdx, y);

							RenderTarget.DrawLine(startPoint.ToVector2(), endPoint.ToVector2(),  FinalBrushDX, plot.Width, plot.StrokeStyle);
							
                            num++;
                           // path.AddLine(path.PathPoints[path.PathPoints.Length - 1], (PointF) new Point(xByBarIdx, y));
                        }
                    }
                    x = xByBarIdx;
                    num3 = num5;
                }
				
				
				FutureBar = FutureBar + 1;
				FutureDay = FutureDay.AddMinutes(minutebarperiod);
				
            }			
			 while (FutureDay.Ticks <= LastDate.Ticks);
			
			
			
			
            if (num++ > 0)
            {
				
//                SmoothingMode smoothingMode = graphics.SmoothingMode;
//                graphics.SmoothingMode = SmoothingMode.AntiAlias;
//                graphics.CompositingQuality = CompositingQuality.HighQuality;

//                Pen pen = base.Plots[plotNum].Pen;
//                graphics.DrawPath(pen, path);
//                graphics.SmoothingMode = smoothingMode;
				
            }
			
			
			
			
			RenderTarget.AntialiasMode = oldAntialiasMode2;
			FinalBrushDX?.Dispose();
        }
		
		
		
		
		
		
		protected override void OnRender(ChartControl chartControl, ChartScale chartScale)
		{

			if (!IsVisible)
				return;
			
			if(Bars == null || Bars.Instrument == null || IsInHitTest) { return; }
						
			ChartControlProperties myProperties = chartControl.Properties;

			// Default plotting in base class. Uncomment if indicators holds at least one plot - so in this case we would expect not to see the SMA plot we have as well in this sample script
			base.OnRender(chartControl, chartScale);
			
			if (BarsMinutePeriod != 0)
				for (int j = 1; j <= 1; j++)
					DrawFuturePlot(chartControl, chartScale, Values[j], 0, j);
				

			//return;
			
			
			// RenderTarget is always full panel, so we need to be mindful which sub ChartPanel we're dealing with
			// always use ChartPanel X, Y, W, H - as ActualWidth, Width, ActualHeight, Height are in WPF units, so they can be drastically different depending on DPI set
			
			Point startPoint	= new Point(ChartPanel.X, ChartPanel.Y);
			Point endPoint		= new Point(ChartPanel.X + ChartPanel.W, ChartPanel.Y + ChartPanel.H);
			Point endPoint2		= new Point(ChartPanel.X + ChartPanel.W, ChartPanel.Y + ChartPanel.H);			
						
			Point startPoint1	= new Point(ChartPanel.X, ChartPanel.Y + ChartPanel.H);
			Point endPoint1		= new Point(ChartPanel.X + ChartPanel.W, ChartPanel.Y);

			Point nextPoint		= new Point(ChartPanel.X + ChartPanel.W, ChartPanel.Y);
			
			double width		= endPoint.X - startPoint.X;
			double height		= endPoint.Y - startPoint.Y;

			//TextFormat	textFormat			= TextFont.ToDirectWriteTextFormat();	
			
			
			
//			TextFormat textFormat		= new TextFormat(Core.Globals.DirectWriteFactory, "Calibri", SharpDX.DirectWrite.FontWeight.Normal,
//															SharpDX.DirectWrite.FontStyle.Normal, SharpDX.DirectWrite.FontStretch.Normal, fontHeight) 
//															{ TextAlignment = SharpDX.DirectWrite.TextAlignment.Leading, WordWrapping = WordWrapping.NoWrap };
		

		
			
			// 4 unused ToDxBrush calls removed — were created and disposed without any drawing
			
			
			
			
			
			SharpDX.Direct2D1.AntialiasMode oldAntialiasMode	= RenderTarget.AntialiasMode;
			RenderTarget.AntialiasMode							= SharpDX.Direct2D1.AntialiasMode.Aliased;
		
			
			
		
				
			//textFormat.Dispose();
			

			// disposed calls removed (brushes were removed above)
			
			RenderTarget.AntialiasMode = oldAntialiasMode;
			
			
			
			
				
			
			oldAntialiasMode	= RenderTarget.AntialiasMode;
      

			if (FirstRender2)
			{
			
//				ChartBarsSwitch2(true);

				
            	myProperties = chartControl.Properties;
//				PreviousDrag = myProperties.AllowSelectionDragging;
				
				
				
				
				
				//chartTrader = Window.GetWindow(ChartControl.Parent).FindFirst("ChartWindowChartTraderControl") as ChartTrader;	
				
				FirstRender2 = false;
				
				
			}
		
				
				ChartTextBrushDX = myProperties.ChartText.ToDxBrush(RenderTarget);
				ChartBackgroundBrushDX = myProperties.ChartBackground.ToDxBrush(RenderTarget);				 			
				//ChartBackgroundErrorBrushDX = Brushes.Red.ToDxBrush(RenderTarget);
							

			
			if (!IsInHitTest)
 			if (AllErrorMessages.Count > 0)
				{
				
					
					//ChartBarsSwitch2(false);
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
					RenderTarget.FillRectangle(CenterRect, ChartBackgroundBrushDX);
					RenderTarget.FillRectangle(CenterRect, ChartBackgroundErrorBrushDX);
					RenderTarget.DrawText(txt, CenterText, ExpandRect(CenterRect,-10,0), ChartTextBrushDX);
					
					
					RenderTarget.AntialiasMode = oldAntialiasMode;
					
				
				
				ChartBackgroundErrorBrushDX.Dispose();		
				CenterText.Dispose();	
				
					//Print("bug2");
					
				return;
			}
			
			
			
				
				
				
		
				
		}
		
		
		
		
		
		
		
		
				
				
				

		public override string FormatPriceMarker(double price)
		{
			double trunc = Math.Truncate(price);
			int fraction = 0;
			string priceMarker = "";
			if (TickSize == 0.03125) 
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
			else if (TickSize == 0.015625)
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
			else if (TickSize == 0.0078125)
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
				priceMarker = price.ToString(NinjaTrader.Core.Globals.GetTickFormatString(TickSize));
			}
			return priceMarker;
		}		
			
		
		
		
		
	
		

//		public override string DisplayName
//		{
//			get
//			{
//					if (State == State.SetDefaults)
//						return "aiBestATR";
//					else
//						return Name;
//			}
		
//		}	
		
		

		public override string DisplayName
		{
			get
			{

				
						if (pMTFEnabled)
						{			
							//finalname = finalname + " " + BarsArray[1].BarsType.DisplayName;				
							return "aiBestATR " + "(" + pATRPeriod.ToString() + ", " + pMTFBarsPeriod1.ToString() + " " + pMTFBasePeriodType1 + ")";
						}
							
				
						return "aiBestATR " + "(" + pATRPeriod.ToString() + ")";
						
						
						
						
						
						if (State == State.SetDefaults)
					{
						
						 
									
						if (pMTFEnabled)
						{			
							//finalname = finalname + " " + BarsArray[1].BarsType.DisplayName;				
							return "aiBestATR " + "(" + pATRPeriod.ToString() + ", " + pMTFBarsPeriod1.ToString() + " " + pMTFBasePeriodType1 + ")";
						}
							
				
						return "aiBestATR " + "(" + pATRPeriod.ToString() + ")";
						
					}
					else
						return "";
			}
		
		}	
		
		
		
		
		
		
		

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> Timing
		{
			get { return Values[1]; }
		}
		
		
		
		private string pATRDMode = "Points";
		[Description("")]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Parameters", Name = "ATR Mode", Order = 5)]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(ATRDMode22))]
		public string ATRDMode
		{
			get { return pATRDMode; }
			set { pATRDMode = value; }
		}
		
		
		internal class ATRDMode22 : StringConverter
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
				return new StandardValuesCollection( new String[] {"Points", "Ticks"} );
			}
		}


		private string pATRSmoothing = "Wilder";
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Parameters", Name = "ATR Smoothing", Description = "Averaging method used to smooth True Range into the ATR.", Order = 4)]
		[TypeConverter(typeof(ATRSmoothing22))]
		public string ATRSmoothing
		{
			get { return pATRSmoothing; }
			set { pATRSmoothing = value; }
		}


		internal class ATRSmoothing22 : StringConverter
		{
			public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
			{
				return true;
			}

			public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
			{
				return true;
			}

			public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
			{
				return new StandardValuesCollection( new String[] {"Wilder", "SMA", "EMA"} );
			}
		}	
		
		
		
		

		private string pTimerDMode = "Week";
//		[Description("")]
//		[Display(ResourceType = typeof(Custom.Resource), GroupName = "History", Name = "Interval", Order = 5)]
//		[RefreshProperties(RefreshProperties.All)]
//		[TypeConverter(typeof(ChooseTimerMode))]
//		public string TimerDMode
//		{
//			get { return pTimerDMode; }
//			set { pTimerDMode = value; }
//		}
		
		
		internal class ChooseTimerMode : StringConverter
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
				return new StandardValuesCollection( new String[] {"Day", "Week"} );
			}
		}	
		
		
			
				private int pDaysBack = 2;
//		[Range(0, 1000)]
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Days Back", Description="", GroupName = "History", Order = 6)]
//		public int DaysBack
//		{
//			get { return pDaysBack; }
//			set { pDaysBack= value; }
//		}	
		

		private bool pHistoryEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "History", Name = "Last Week Enabled", Description = "", Order = -1)]
		//[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        public bool HistoryEnabled
        {
            get { return pHistoryEnabled; }
            set { pHistoryEnabled = value; }
        }		
		
		
			
				private int pStrength = 10;
		[Range(0, 1000)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Strength for turning points", Description="", GroupName = "Parameters", Order = 3)]
		public int Strength
		{
			get { return pStrength; }
			set { pStrength = value; }
		}	
				

			
		private int pATRPeriod = 14;
		[Range(0, 1000)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "ATR Period", Description="", GroupName = "Parameters", Order = 3)]
		public int ATRPeriod
		{
			get { return pATRPeriod; }
			set { pATRPeriod = value; }
		}

		private bool pColorBySlope = false;
		[Display(ResourceType = typeof(Custom.Resource), Name = "Color By Slope", Description = "Colors the ATR plot based on its slope - one color when rising, another when falling.", GroupName = "Parameters", Order = 6)]
		[RefreshProperties(RefreshProperties.All)]
		public bool ColorBySlope
		{
			get { return pColorBySlope; }
			set { pColorBySlope = value; }
		}

		private Brush pSlopeUpBrush = Brushes.DodgerBlue;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Slope Up Color", Description = "Plot color when the ATR is rising.", GroupName = "Parameters", Order = 7)]
		public Brush SlopeUpBrush
		{
			get { return pSlopeUpBrush; }
			set { pSlopeUpBrush = value; }
		}

		[Browsable(false)]
		public string SlopeUpBrushSerialize
		{
			get { return Serialize.BrushToString(pSlopeUpBrush); }
			set { pSlopeUpBrush = Serialize.StringToBrush(value); }
		}

		private Brush pSlopeDownBrush = Brushes.Red;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Slope Down Color", Description = "Plot color when the ATR is falling.", GroupName = "Parameters", Order = 8)]
		public Brush SlopeDownBrush
		{
			get { return pSlopeDownBrush; }
			set { pSlopeDownBrush = value; }
		}

		[Browsable(false)]
		public string SlopeDownBrushSerialize
		{
			get { return Serialize.BrushToString(pSlopeDownBrush); }
			set { pSlopeDownBrush = Serialize.StringToBrush(value); }
		}			
		
	
		private bool pHighLowEnabled = false;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Parameters", Name = "Hi Low Enabled", Description = "", Order = 0)]
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        public bool HighLowEnabled
        {
            get { return pHighLowEnabled; }
            set { pHighLowEnabled = value; }
        }		
		
		
		
		
		// joel levels

//		private TimeSpan pStartTime = new TimeSpan(9,00,0);
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Time Start", GroupName = "Time", Order = 1)]
//		public string StartT
//		{
//			get { return pStartTime.Hours.ToString("0")+":"+pStartTime.Minutes.ToString("00")+":"+pStartTime.Seconds.ToString("00"); }
//			set { if(!TimeSpan.TryParse(value, out pStartTime)) pStartTime=new TimeSpan(0,0,0); }
//		}
		
//		private TimeSpan pEndTime = new TimeSpan(15,49,56);
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Time End", GroupName = "Time", Order = 2)]
//		public string EndT
//		{
//			get { return pEndTime.Hours.ToString("0")+":"+pEndTime.Minutes.ToString("00")+":"+pEndTime.Seconds.ToString("00"); }
//			set { if(!TimeSpan.TryParse(value, out pEndTime)) pEndTime=new TimeSpan(0,0,0); }
//		}	
		
//		private TimeSpan pFinalTime = new TimeSpan(15,51,0);
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Time Final", GroupName = "Time", Order = 3)]
//		public string FinalT
//		{
//			get { return pFinalTime.Hours.ToString("0")+":"+pFinalTime.Minutes.ToString("00")+":"+pFinalTime.Seconds.ToString("00"); }
//			set { if(!TimeSpan.TryParse(value, out pFinalTime)) pFinalTime=new TimeSpan(0,0,0); }
//		}		
		
//				private int pEntryTicks = 16;
//		[Range(0, 1000)]
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Entry Offset", Description="", GroupName = "Time", Order = 3)]
//		public int EntryTicks
//		{
//			get { return pEntryTicks; }
//			set { pEntryTicks= value; }
//		}	
		
		
//			[XmlIgnore]
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Text Color", GroupName = "NinjaScriptGeneral")]
//		public Brush TextBrush
//		{
//			get { return textBrush; }
//			set { textBrush = value; }
//		}

//		[Browsable(false)]
//		public string TextBrushSerialize
//		{
//			get { return Serialize.BrushToString(TextBrush); }
//			set { TextBrush = Serialize.StringToBrush(value); }
//		}	
		
//		private bool pUseChartData = true;
//        [Display(ResourceType = typeof(Custom.Resource), Name = "Use Chart Data", Description = "", GroupName = "Time", Order = 1)]
//        public bool UseChartData
//        {
//            get { return pUseChartData; }
//            set { pUseChartData = value; }
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
		
//		private int pRightPX = 0;
//		[Range(0, 1000)]
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Label X Offset", Description="in pixels.", GroupName = "Display", Order = 3)]
//		public int RightPX
//		{
//			get { return pRightPX; }
//			set { pRightPX= value; }
//		}	
		
//		private int pShadowWidth = 3;
//		[Range(0, 100)]
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Shadow Width", Description="in pixels.", GroupName = "Display", Order = 5)]
//		public int ShadowWidth
//		{
//			get { return pShadowWidth; }
//			set { pShadowWidth= value; }
//		}	
		
		

		
		
		private Brush		areaBrush		= Brushes.Blue;
		private Brush		textBrush		= Brushes.White;
		private Brush		smallAreaBrush	= Brushes.Red;
		private	int			areaOpacity		= 20;
		const	float 		fontHeight		= 30f;
		
		
//		[XmlIgnore]
//		[Display(ResourceType = typeof(Custom.Resource), Name = "NinjaScriptDrawingToolShapesAreaBrush", GroupName = "NinjaScriptGeneral")]
//		public Brush AreaBrush
//		{
//			get { return areaBrush; }
//			set
//			{
//				areaBrush = value;
//				if (areaBrush != null)
//				{
//					if (areaBrush.IsFrozen)
//						areaBrush = areaBrush.Clone();
//					areaBrush.Opacity = areaOpacity / 100d;
//					areaBrush.Freeze();
//				}
//			}
//		}

//		[Browsable(false)]
//		public string AreaBrushSerialize
//		{
//			get { return Serialize.BrushToString(AreaBrush); }
//			set { AreaBrush = Serialize.StringToBrush(value); }
//		}

		
		
		
		
//		[Range(0, 100)]
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Shadow Opacity (%)", GroupName = "Display", Order = 4)]
//		public int AreaOpacity
//		{
//			get { return areaOpacity; }
//			set
//			{
//				areaOpacity = Math.Max(0, Math.Min(100, value));
//				if (areaBrush != null)
//				{
//					Brush newBrush		= areaBrush.Clone();
//					newBrush.Opacity	= areaOpacity / 100d;
//					newBrush.Freeze();
//					areaBrush			= newBrush;
//				}
//			}
//		}

		
		
		
//		[XmlIgnore]
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Small area color", GroupName = "NinjaScriptGeneral")]
//		public Brush SmallAreaBrush
//		{
//			get { return smallAreaBrush; }
//			set { smallAreaBrush = value; }
//		}

//		[Browsable(false)]
//		public string SmallAreaBrushSerialize
//		{
//			get { return Serialize.BrushToString(SmallAreaBrush); }
//			set { SmallAreaBrush = Serialize.StringToBrush(value); }
//		}
		
		
		
		
		

		
  		private bool pMTFEnabled = false;
       [NinjaScriptProperty]
			[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
		[Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", GroupName = "Multi Time Frame", Order = 0)]
        public bool MTFEnabled
        {
            get { return pMTFEnabled; }
            set { pMTFEnabled = value; }
        }

		private BarsPeriodType FinalBasePeriodType1 = BarsPeriodType.Minute;		
		private string pMTFBasePeriodType1 = "Minute";
		[NinjaScriptProperty]
		[Description("")]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Bars Type", Description = "", GroupName = "Multi Time Frame", Order = 1)]
		[TypeConverter(typeof(AllMTF))]
		public string MTFBasePeriodType1
		{
            get { return pMTFBasePeriodType1; }
            set 
			{ 
				pMTFBasePeriodType1 = value; 
			
				switch (pMTFBasePeriodType1) 
				{
					case "Tick":   FinalBasePeriodType1 = BarsPeriodType.Tick; break;
					case "Volume":  FinalBasePeriodType1 = BarsPeriodType.Volume; break;
					case "Range": FinalBasePeriodType1 = BarsPeriodType.Range; break;
					case "Second": FinalBasePeriodType1 = BarsPeriodType.Second; break;
					case "Minute": FinalBasePeriodType1 = BarsPeriodType.Minute; break;
					case "Renko": FinalBasePeriodType1 = BarsPeriodType.Renko; break;
//					case "Day": FinalBasePeriodType1 = BarsPeriodType.Day; break;
//					case "Week": FinalBasePeriodType1 = BarsPeriodType.Week; break;
//					case "Month": FinalBasePeriodType1 = BarsPeriodType.Month; break;						
						
				}	
				
			}
		}			

		internal class AllMTF : StringConverter
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
				return new StandardValuesCollection( new String[] { "Tick", "Volume", "Range", "Second", "Minute", "Renko" } );
			}
		}	
		
		
        private int pMTFBarsPeriod1 = 5;
        [Range(1, int.MaxValue)]
		[NinjaScriptProperty]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Bars Period", GroupName = "Multi Time Frame", Order = 2)]
        public int MTFBarsPeriod1
        {
            get { return pMTFBarsPeriod1; }
            set { pMTFBarsPeriod1 = value; }
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
	public class aiBestATRConverter : NinjaTrader.NinjaScript.IndicatorBaseConverter
	{
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) { return true; }

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			PropertyDescriptorCollection propertyDescriptorCollection = base.GetPropertiesSupported(context) ? base.GetProperties(context, value, attributes) : TypeDescriptor.GetProperties(value, attributes);

			aiBestATR   jbb = (aiBestATR) value;
			
			//Pivots						thisPivotsInstance			= (Pivots) value;
			
			//bool MagnetsOn = ;
			
			List<string> DeleteThese = new List<string>();
			List<string> DeleteThese2 = new List<string>();
				
	
			//DeleteThese.Add("Calculate");
			//DeleteThese.Add("Name");
      		DeleteThese.Add("MaximumBarsLookBack");
			
			DeleteThese.Add("Input");
			
			DeleteThese.Add("IsAutoScale");
			//DeleteThese.Add("Displacement");
			//DeleteThese.Add("DisplayInDataBox");
			//DeleteThese.Add("Panel");
			//DeleteThese.Add("PaintPriceMarkers");
			//DeleteThese.Add("ScaleJustification");
			//DeleteThese.Add("IsVisible");
			
			
			
			
		
			
			if (!jbb.MTFEnabled)
			{
				
				DeleteThese.Add("MTFBasePeriodType1");	
				DeleteThese.Add("MTFBarsPeriod1");
	
			}	
   
	
			if (jbb.MTFEnabled)
				DeleteThese.Add("ATRSmoothing");

			if (!jbb.ColorBySlope)
			{
				DeleteThese.Add("SlopeUpBrush");
				DeleteThese.Add("SlopeDownBrush");
			}

			DeleteThese.Add("HighLowEnabled");
			DeleteThese.Add("Strength");

			

//			DeleteThese.Add("Calculate");
//			DeleteThese.Add("Name");
//      	 	DeleteThese.Add("MaximumBarsLookBack");
			
			
			
			
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
		private aiBestATR[] cacheaiBestATR;
		public aiBestATR aiBestATR(bool mTFEnabled, string mTFBasePeriodType1, int mTFBarsPeriod1)
		{
			return aiBestATR(Input, mTFEnabled, mTFBasePeriodType1, mTFBarsPeriod1);
		}

		public aiBestATR aiBestATR(ISeries<double> input, bool mTFEnabled, string mTFBasePeriodType1, int mTFBarsPeriod1)
		{
			if (cacheaiBestATR != null)
				for (int idx = 0; idx < cacheaiBestATR.Length; idx++)
					if (cacheaiBestATR[idx] != null && cacheaiBestATR[idx].MTFEnabled == mTFEnabled && cacheaiBestATR[idx].MTFBasePeriodType1 == mTFBasePeriodType1 && cacheaiBestATR[idx].MTFBarsPeriod1 == mTFBarsPeriod1 && cacheaiBestATR[idx].EqualsInput(input))
						return cacheaiBestATR[idx];
			return CacheIndicator<aiBestATR>(new aiBestATR(){ MTFEnabled = mTFEnabled, MTFBasePeriodType1 = mTFBasePeriodType1, MTFBarsPeriod1 = mTFBarsPeriod1 }, input, ref cacheaiBestATR);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.aiBestATR aiBestATR(bool mTFEnabled, string mTFBasePeriodType1, int mTFBarsPeriod1)
		{
			return indicator.aiBestATR(Input, mTFEnabled, mTFBasePeriodType1, mTFBarsPeriod1);
		}

		public Indicators.aiBestATR aiBestATR(ISeries<double> input , bool mTFEnabled, string mTFBasePeriodType1, int mTFBarsPeriod1)
		{
			return indicator.aiBestATR(input, mTFEnabled, mTFBasePeriodType1, mTFBarsPeriod1);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.aiBestATR aiBestATR(bool mTFEnabled, string mTFBasePeriodType1, int mTFBarsPeriod1)
		{
			return indicator.aiBestATR(Input, mTFEnabled, mTFBasePeriodType1, mTFBarsPeriod1);
		}

		public Indicators.aiBestATR aiBestATR(ISeries<double> input , bool mTFEnabled, string mTFBasePeriodType1, int mTFBarsPeriod1)
		{
			return indicator.aiBestATR(input, mTFEnabled, mTFBasePeriodType1, mTFBarsPeriod1);
		}
	}
}

#endregion
