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
	
using SharpDX;
using SharpDX.Direct2D1;


	

//#region Global Enums

//public enum amaMAFastTrendDefinition {Cross, Thrust}

//#endregion
// This namespace holds indicators in this folder and is required. Do not change it.
namespace NinjaTrader.NinjaScript.Indicators
{
	/// <summary>
	/// The MAFast Lines indicator is built from a linear regression indicator and an exponential moving average calculated from the regression indicator.
	/// The area between the two lines is colored. The color can be used as a trend indication.
	/// </summary>
	//[Gui.CategoryOrder("Algorithmic Options", 0)]
	
	
	[Gui.CategoryOrder("Parameters", 10)]
	[Gui.CategoryOrder("Multi Time Frame", 11)]
	[Gui.CategoryOrder("Plots", 60)]
//	[Gui.CategoryOrder("Cloud", 70)]
	[Gui.CategoryOrder("Bar Color", 75)]


	[Gui.CategoryOrder("Plots", 100)]

	[Gui.CategoryOrder("Audio", 100)]
	[Gui.CategoryOrder("Email", 110)]
   	
	[Gui.CategoryOrder("Visual", 156)]
	[Gui.CategoryOrder("Data Series", 165)]
	
	
	
	[Gui.CategoryOrder("Setup", 9000)]
	[Gui.CategoryOrder("License", 10000)]
	
	//[Gui.CategoryOrder("Version", 90)]
	
	[TypeConverter("NinjaTrader.NinjaScript.Indicators.aiBestMAsTrendConverter")]	
	public class aiBestMAsTrend : Indicator
	{
		
		
	
		private string ThisName = "aiBestMAsTrend";
		
		
		
		
		private int LastEmailBar, LastAudioBar = 0;
		private string message, subject = string.Empty;		
		
		private Series<int>		Signal;
		
		private int 							triggerPeriod 				= 80;
		private int								avgPeriod					= 20;
		private int								displacement				= 0;
		private int								totalBarsRequiredToPlot		= 0;
		private int								areaOpacity					= 20;
		private int								barOpacity					= 40;
		private double							maxMAFast					= 0;
		private double							minMAFast					= 0;
		private bool							showMAFastLines			= true;
		private bool							fillArea					= true;

		private bool							areaIsFilled				= true;
		private bool							calculateFromPriceData		= true;
		private bool							indicatorIsOnPricePanel		= true;
		//private amaMAFastTrendDefinition		trendDefinition				= amaMAFastTrendDefinition.Cross;
		private System.Windows.Media.Brush		triggerBrush				= Brushes.Navy;
		private System.Windows.Media.Brush		averageBrush				= Brushes.Navy;
		private System.Windows.Media.Brush		cloudBrushUpS				= Brushes.DodgerBlue;
		private System.Windows.Media.Brush		cloudBrushDownS				= Brushes.DarkRed;
		private System.Windows.Media.Brush		cloudBrushUp				= Brushes.Transparent;
		private System.Windows.Media.Brush		cloudBrushDown				= Brushes.Transparent;
		private System.Windows.Media.Brush		upBrushUp					= Brushes.LimeGreen;
		private System.Windows.Media.Brush		upBrushDown					= Brushes.DarkGreen;
		private System.Windows.Media.Brush		downBrushUp					= Brushes.DarkRed;
		private System.Windows.Media.Brush		downBrushDown				= Brushes.Red;
		private System.Windows.Media.Brush		upBrushOutline				= Brushes.Black;
		private System.Windows.Media.Brush		downBrushOutline			= Brushes.Black;
		private int								plot0Width					= 1;
		private int								plot1Width					= 1;
		private PlotStyle						plot0Style					= PlotStyle.Line;
		private DashStyleHelper					dash0Style					= DashStyleHelper.Solid;
		private PlotStyle						plot1Style					= PlotStyle.Line;
		private DashStyleHelper					dash1Style					= DashStyleHelper.Solid;
		private System.Windows.Media.Brush		errorBrush					= Brushes.Black;
		private SimpleFont						errorFont					= null;
		private string							versionString				= "v 1.3  -  September 17, 2017";
		private Series<double>					trend;
		private LinReg							trigger;
		private EMA								triggerAverage;
		
		
		// Long_Entry, Long_Stay_In, Long_Exit, Short_Entry, Short_Stay_In, Short_Exit converted to locals in OnBarUpdate
		private Series<double>					Long_State;
		private Series<double>					Short_State;
		
		private Series<double>					Body;
		
		
		private System.Windows.Media.Brush UpNeutralBrush;
		private System.Windows.Media.Brush DownNeutralBrush;
		private System.Windows.Media.Brush UpBuyBrush;
		private System.Windows.Media.Brush DownBuyBrush;
		private System.Windows.Media.Brush UpSellBrush;
		private System.Windows.Media.Brush DownSellBrush;
		
		
		

		private SortedDictionary<int, int> ProductIDToMachineIDs = new SortedDictionary<int, int>();
		private ConcurrentDictionary<int, List<string>> ProductIDToInstruments = new ConcurrentDictionary<int, List<string>>();
		private string LicensingMessage = string.Empty;
		private bool Permission = false;
		
		private List<string> AllErrorMessages = new List<string>();
		
		private SharpDX.Direct2D1.Brush ChartTextBrushDX = null;
		private SharpDX.Direct2D1.Brush ChartBackgroundBrushDX = null;
		private SharpDX.Direct2D1.Brush ChartBackgroundErrorBrushDX = null;
		private SharpDX.DirectWrite.TextFormat CenterText;
		private SharpDX.RectangleF CenterRect;
		
			
		
		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
//				Description					= "\r\nThe MAFast Lines indicator is built from a linear regression indicator and an exponential moving average calculated from the regression indicator."
//												+ " The area between the two lines is colored. The color can be used as a trend indication."; 
				Name						= "aiBestMAsTrend";
				IsSuspendedWhileInactive	= true;
				IsOverlay					= true;
				ArePlotsConfigurable		= false;
				
				IsAutoScale = false;
				
				Calculate = Calculate.OnPriceChange;
				
				AddPlot(new Stroke(Brushes.Black, 2), PlotStyle.Line, "MAFast");
				AddPlot(new Stroke(Brushes.Black, 2), PlotStyle.Line, "MASlow");
				AddPlot(new Stroke(Brushes.Transparent, 2), PlotStyle.Dot, "Signals");
				
				
			}
			else if (State == State.Configure)
			{
				
				if (pMTFEnabled) AddDataSeries((BarsPeriodType)FinalBasePeriodType1, pMTFBarsPeriod1);
				
				displacement = Displacement;
				BarsRequiredToPlot	= Math.Max(triggerPeriod + 3 * avgPeriod, -displacement);
				totalBarsRequiredToPlot = Math.Max(0, BarsRequiredToPlot + displacement);
				
				
				
//				if(showMAFastLines)
//				{	
//					Plots[0].Brush = triggerBrush;
//					Plots[1].Brush = averageBrush;
//				}
//				else
//				{	
//					Plots[0].Brush = Brushes.Transparent;
//					Plots[1].Brush = Brushes.Transparent;
//				}
//				Plots[0].Width = plot0Width;
//				Plots[0].PlotStyle = plot0Style;
//				Plots[0].DashStyleHelper = dash0Style;			
//				Plots[1].Width = plot1Width;
//				Plots[1].PlotStyle = plot1Style;
//				Plots[1].DashStyleHelper = dash1Style;
	

				
				
				cloudBrushUp = cloudBrushUpS.Clone();
				cloudBrushUp.Opacity = (float) areaOpacity/100.0;
				cloudBrushUp.Freeze();
				cloudBrushDown = cloudBrushDownS.Clone();
				cloudBrushDown.Opacity = (float) areaOpacity/100.0;
				cloudBrushDown.Freeze();
			}	
			else if (State == State.DataLoaded)
			{
					
//					Plots[0].Brush = pStroke01.Brush;
//					Plots[1].Brush = pStroke02.Brush;
				
//					Plots[0].Opacity = pStroke01.Opacity;
//					Plots[1].Opacity = pStroke02.Opacity;			
				
//					Plots[0].DashStyleHelper = pStroke01.DashStyleHelper;
//					Plots[1].DashStyleHelper = pStroke02.DashStyleHelper;
//					Plots[0].Width = pStroke01.Width;
//					Plots[1].Width = pStroke02.Width;
				
				
					// if plots are the same color
				
//					Plots[0].Brush = pStroke01.Brush;
//					Plots[1].Brush = pStroke01.Brush;
				
//					Plots[0].Opacity = pStroke01.Opacity;
//					Plots[1].Opacity = pStroke01.Opacity;			
				
//					Plots[0].DashStyleHelper = pStroke01.DashStyleHelper;
//					Plots[1].DashStyleHelper = pStroke01.DashStyleHelper;
//					Plots[0].Width = pStroke01.Width;
//					Plots[1].Width = pStroke01.Width;
				
				
					//Plots[0].Opacity = pPlotsOpacity;
					//Plots[1].Opacity = pPlotsOpacity;			
				
					Plots[0].DashStyleHelper = dash0Style;
					Plots[1].DashStyleHelper = dash0Style;
					Plots[0].Width = plot0Width;
					Plots[1].Width = plot0Width;				

		
				
				
					Plots[0].Name = pEMA1Fast.ToString() + " EMA";
					Plots[1].Name = pEMA1Slow.ToString() + " EMA";
			
			
				if (pMTFEnabled) 
				{
					Plots[0].Name = "5 Min " + pEMA1Fast.ToString() + " EMA";
					Plots[1].Name = "5 Min " + pEMA1Slow.ToString() + " EMA";					
					
				}
				
				
			
				System.Windows.Media.Brush newBrush = pColorUpBrush1.Clone();
				newBrush.Opacity = (double)pPlotsOpacity/(double)100;
				newBrush.Freeze();
				pColorUpBrush1 = newBrush;

				System.Windows.Media.Brush newBrush2 = pColorDownBrush1.Clone();
				newBrush2.Opacity = (double)pPlotsOpacity/(double)100;
				newBrush2.Freeze();
				pColorDownBrush1 = newBrush2;
				
			
				
				
				
				Signal			= new Series<int>(this);
				
				
				Long_State = new Series<double>(this);
				Short_State = new Series<double>(this);

				Body = new Series<double>(this);
				
				trend = new Series<double>(this, MaximumBarsLookBack.Infinite);
				trigger = LinReg(Input, triggerPeriod);
				triggerAverage = EMA(trigger, avgPeriod);
				if(Input is PriceSeries)
					calculateFromPriceData = true;
				else
					calculateFromPriceData = false;
				
				
				
				
				
				
//				UpNeutralBrush = pColorNeutralBrush.Clone();
//				DownNeutralBrush = pColorNeutralBrush.Clone();
//				UpBuyBrush = pColorUpBrush.Clone();
//				DownBuyBrush = pColorUpBrush.Clone();
//				UpSellBrush = pColorDownBrush.Clone();
//				DownSellBrush = pColorDownBrush.Clone();
				
				
				
//				if (UpNeutralBrush != null)
//				{
//					if (UpNeutralBrush.IsFrozen)
//						UpNeutralBrush = UpNeutralBrush.Clone();
//					UpNeutralBrush.Opacity = pBarOpacityUp / 100d;
//					UpNeutralBrush.Freeze();
//				}			
				
//				if (DownNeutralBrush != null)
//				{
//					if (DownNeutralBrush.IsFrozen)
//						DownNeutralBrush = DownNeutralBrush.Clone();
//					DownNeutralBrush.Opacity = pBarOpacityDown / 100d;
//					DownNeutralBrush.Freeze();
//				}			
								
//				if (UpBuyBrush != null)
//				{
//					if (UpBuyBrush.IsFrozen)
//						UpBuyBrush = UpBuyBrush.Clone();
//					UpBuyBrush.Opacity = pBarOpacityUp / 100d;
//					UpBuyBrush.Freeze();
//				}			
				
//				if (DownBuyBrush != null)
//				{
//					if (DownBuyBrush.IsFrozen)
//						DownBuyBrush = DownBuyBrush.Clone();
//					DownBuyBrush.Opacity = pBarOpacityDown / 100d;
//					DownBuyBrush.Freeze();
//				}
				
//				if (UpSellBrush != null)
//				{
//					if (UpSellBrush.IsFrozen)
//						UpSellBrush = UpSellBrush.Clone();
//					UpSellBrush.Opacity = pBarOpacityUp / 100d;
//					UpSellBrush.Freeze();
//				}			
				
//				if (DownSellBrush != null)
//				{
//					if (DownSellBrush.IsFrozen)
//						DownSellBrush = DownSellBrush.Clone();
//					DownSellBrush.Opacity = pBarOpacityDown / 100d;
//					DownSellBrush.Freeze();
//				}					
				
				
				
				
				

				
					
				
				
				
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
                    //ChartPanel.MouseMove += new MouseEventHandler(OnMouseMove);
                    ChartPanel.MouseDown += new MouseButtonEventHandler(OnMouseDown);
					//ChartPanel.MouseLeave += new MouseEventHandler(OnMouseLeave);
                }
				
				
			}	
			
			else if (State == State.Terminated)
			{
		
				if (ChartControl != null)
                if (ChartPanel != null)
                {
                   // ChartPanel.MouseMove -= new MouseEventHandler(OnMouseMove);
                    ChartPanel.MouseDown -= new MouseButtonEventHandler(OnMouseDown);
					//ChartPanel.MouseLeave -= new MouseEventHandler(OnMouseLeave);
                }
				
				
			}
			
			
			else if (State == State.Historical)
			{
				areaIsFilled = fillArea && areaOpacity > 0;
				if(ChartBars != null)
					indicatorIsOnPricePanel = (ChartPanel.PanelIndex == ChartBars.Panel);
				else
					indicatorIsOnPricePanel = false;
				this.ZOrder = -1; //SetZOrder(-1);
			}
		}
		
		
		
		
		
		
		
			
		
		private bool LicenseWordPress (string machineid, string pLicensingEmailAddress)
		{
			
			pLicensingEmailAddress = pLicensingEmailAddress.Replace(" ", "");
			
			
			
			int ThisProductMainID = 518188;
			int ThisProductXID = 0;	
			
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
					//UserHasStrategy = false;
					
//					Print("-----------");
//					Print(machineidstring + " " + notestring);
//					Print("-----------");
					
					 foreach (KeyValuePair<int, int> oneproduct in ProductIDToMachineIDs)
           			 {		
				
						 int productid = oneproduct.Key;
						int numbermachineidsok = oneproduct.Value;
						
						 //Print(productid + "-" + numbermachineidsok);
						 
						if (productid == ThisProductMainID)
						{
							
							if (machineidnumber > numbermachineidsok)
							{
								int maxids = numbermachineidsok;
								if (maxids == 1)
									LicensingMessage = "This product is only licensed for 1 NinjaTrader Machine ID.  Please login to your Affordable Indicators, Inc. account, go to the Members Area Dashboard, and update your NinjaTrader Machine ID(s) accordingly." ;
								else
									LicensingMessage = "This product is only licensed for 1-" + maxids.ToString() + " NinjaTrader Machine IDs.  Please login to your Affordable Indicators, Inc. account, go to the Members Area Dashboard, and update your NinjaTrader Machine ID(s) accordingly." ;
							}
							else
							{
								Permission = true;
							}
								
							
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
						
//						if (productid == ThisProductXID)
//						{
//							UserHasStrategy = true;
							
//						}
						
						
											
						
						 
						 
					 }
					 
		 
					  
					 if (!Permission && LicensingMessage == "")
					 {
							LicensingMessage = "You haven't purchased this product.  Contact " + pContactEmail + " if you need further assistance.";
							
					 }
				
				
					//for (int i=0; i < entries.Count; i++)  //loop through each entry and draw it
					{		
						
						
						
										
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

		

		private void AddError(string eee)
		{
		
			//if (pErrorMessagesEnabled)
			if (!AllErrorMessages.Contains(eee))
				AllErrorMessages.Add(eee);
			
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
		
				

        internal void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            //this.MP = e.GetPosition(this.ChartPanel);

 
//			FinalXPixel = MP.X / 100 * dpiX;
//			FinalYPixel = MP.Y / 100 * dpiY;
         
        
          // Print("mouseddddd");
			
			
		if (AllErrorMessages.Count > 0)
			{
				AllErrorMessages.Clear();
				ChartControl.InvalidateVisual();
				
				//myProperties.AllowSelectionDragging = PreviousDrag;
				
				
				return;
				
			}

	

        }
		

				
		
		
		protected override void OnBarUpdate()
		{
			
			
			
//			PlotBrushes[0][0] = pStroke01.Brush;
//			PlotBrushes[1][0] = pStroke02.Brush;

			
			
//			MAFast[0] = trigger[0];
//			MASlow[0] = triggerAverage[0];
			

			
			//if(CurrentBar < BarsRequiredToPlot)
			
			if (CurrentBars[0] >= 0)
				trend[0] = 0.0;
			
			
			for (int i = 1; i <= BarsArray.Length; i++)
				if (CurrentBars[i-1] <= 0)
					return;
			
							

				
			
				if (!pMTFEnabled)
				{
					
					
					if (pThisMAType == "EMA")
					{

						MAFast[0] = EMA(BarsArray[0], pEMA1Fast)[0];
						MASlow[0] = EMA(BarsArray[0], pEMA1Slow)[0];
							
					}
					else if (pThisMAType == "HMA")
					{
						
						MAFast[0] = HMA(BarsArray[0], pEMA1Fast)[0];
						MASlow[0] = HMA(BarsArray[0], pEMA1Slow)[0];			
	
					}
					else if (pThisMAType == "SMA")
					{
						
						MAFast[0] = SMA(BarsArray[0], pEMA1Fast)[0];
						MASlow[0] = SMA(BarsArray[0], pEMA1Slow)[0];			
	
					}
					

					// this doesn't work
					
//					MAFast[0] = aiBestMAs(false, pMTFBasePeriodType1,pMTFBarsPeriod1,pThisMAType,pEMA1Fast)[0];
//					MASlow[0] = aiBestMAs(false, pMTFBasePeriodType1,pMTFBarsPeriod1,pThisMAType,pEMA1Slow)[0];
					
					
				}
				else
				{
					MAFast[0] = aiBestMAs(true, Instrument.FullName, pMTFBasePeriodType1,pMTFBarsPeriod1,pThisMAType,pEMA1Fast)[0];
					MASlow[0] = aiBestMAs(true, Instrument.FullName, pMTFBasePeriodType1,pMTFBarsPeriod1,pThisMAType,pEMA1Slow)[0];
					
					//MASlow[0] = EMAMinute(pMTFBarsPeriod1,pEMA1Slow)[0];
					
					
					
					
					
					
					
	
					
					
					
					
				}
				
						
				if (MAFast[0] > MASlow[0])
				{
			
				
							
					trend[0] = 1;
				}
				else if (MAFast[0] < MASlow[0])
				{
					
			
							
					
					trend[0] = -1;
				}
				else
				{
					trend[0] = trend[1];
				}
				
				
				
				
				Signals[0] = 0;
				
				if (trend[0] == 1)
				{

					if (trend[1] != 1)
						Signals[0] = 1;

					PlotBrushes[0][0] = pColorUpBrush1;
					PlotBrushes[1][0] = pColorUpBrush1;
				}
				else
				{

					if (trend[1] != -1)
						Signals[0] = -1;

					PlotBrushes[0][0] = pColorDownBrush1;
					PlotBrushes[1][0] = pColorDownBrush1;
				}
				
				
				Signal[0] = (int) Signals[0];
				
				
				
				
				

			int BB = 0;
			if (Calculate != Calculate.OnBarClose && !pQuickAudio)
				BB = 1;


			if (pAudioEnabled && State != State.Historical)
			{
				if (Signal[BB] == 1 && LastAudioBar != CurrentBars[0])
				{
					Alert(CurrentBar.ToString(), Priority.High, ThisName + " New Trend | Long | " + Bars.BarsPeriod.ToString(), NinjaTrader.Core.Globals.InstallDir + @"\sounds\" + pWAVFileName, 1, pColorUpBrush2, GetTextColor(pColorUpBrush2));
					LastAudioBar = CurrentBars[0];
				}
				if (Signal[BB] == -1 && LastAudioBar != CurrentBars[0])
				{
					Alert(CurrentBar.ToString(), Priority.High, ThisName + " New Trend | Short | " + Bars.BarsPeriod.ToString(), NinjaTrader.Core.Globals.InstallDir + @"\sounds\" + pWAVFileName2, 1, pColorDownBrush2, GetTextColor(pColorDownBrush2));
					LastAudioBar = CurrentBars[0];
				}

				//				if(Signal[BB] == 2 && LastAudioBar != CurrentBars[0])
				//				{
				//					Alert(CurrentBar.ToString(),Priority.High, Name + " PFE Cross Long | " + Bars.BarsPeriod.ToString(), NinjaTrader.Core.Globals.InstallDir+@"\sounds\"+pWAVFileName3,1,pColorUpBrush3,GetTextColor(pColorUpBrush3));
				//					LastAudioBar = CurrentBars[0];
				//				}
				//				if(Signal[BB] == -2 && LastAudioBar != CurrentBars[0])
				//				{
				//					Alert(CurrentBar.ToString(),Priority.High, Name + " PFE Cross Short | " + Bars.BarsPeriod.ToString(), NinjaTrader.Core.Globals.InstallDir+@"\sounds\"+pWAVFileName4,1,pColorDownBrush3,GetTextColor(pColorDownBrush3));
				//					LastAudioBar = CurrentBars[0];
				//				}				

			}


			BB = 0;
			if (Calculate != Calculate.OnBarClose && !pQuickEmail)
				BB = 1;

			if (pEmailEnabled && State != State.Historical)
			{

				if (Signal[BB] == 1 && LastEmailBar != CurrentBars[0])
				{
					//subject = Instrument.FullName + " " + BarsPeriod + " Chart  |  " + "New Signal Long " + Close[0].ToString(PriceString);

					subject = ThisName + "New Trend | Long | " + Bars.BarsPeriod.ToString() + " Chart | " + Instrument.FullName;
					message = subject;

					if (pEmailAddress != "" && State != State.Historical)
						SendMail(pEmailAddress, subject, message);

					LastEmailBar = CurrentBars[0];
				}

				if (Signal[BB] == -1 && LastEmailBar != CurrentBars[0])
				{
					//subject = Instrument.FullName + " " + BarsPeriod + " Chart  |  " + "New Signal Long " + Close[0].ToString(PriceString);

					subject = ThisName + "New Trend | Short | " + Bars.BarsPeriod.ToString() + " Chart | " + Instrument.FullName;
					message = subject;

					if (pEmailAddress != "" && State != State.Historical)
						SendMail(pEmailAddress, subject, message);

					LastEmailBar = CurrentBars[0];
				}

			}



				
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

			
			
			if (CurrentBar > 0)
			{	
			
		

		
//				Line3[0] = Line1[0];
//				Line4[0] = Line2[0];
				
				
	
				
				//if (ChartControl != null)
				{
//					PlotBrushes[0][0] = pBackBrush1;
//					PlotBrushes[1][0] = pBackBrush1;
				}
				
//				if (Trend1[0] == 1) 
//				{
//					PlotBrushes[0][0] = pBrush01;
//					PlotBrushes[1][0] = pBrush01;
					
//					if (pAreaOpacity1 != 0) 
//						Draw.Region(this,"MAL1"+LastSwitchBar1[0], Math.Min(CurrentBars[0],CurrentBars[0]-LastSwitchBar1[0]+1), 0, Values[1], Values[0], null, pColorUpBrush1, pAreaOpacity1);
//				} 
//				else 
//				{
//					PlotBrushes[0][0] = pBrush02;
//					PlotBrushes[1][0] = pBrush02;
					
//					if (pAreaOpacity1 != 0) 
//						Draw.Region(this,"MAS1"+LastSwitchBar1[0], Math.Min(CurrentBars[0],CurrentBars[0]-LastSwitchBar1[0]+1), 0, Values[1], Values[0], null, pColorDownBrush1, pAreaOpacity1);
//				}

			}		
			
			
			
			
				
			Body[0] = (Open[0] + Close[0]) / 2;

			if (CurrentBar == 0)
				return;
				
			double longEntry = 0, longStayIn = 0, longExit = 0;
			Long_State[0] = Long_State[1];

			if (Close[0] > trigger[0] && Close[0] > triggerAverage[0] && Body[0] > trigger[0] && Body[0] > triggerAverage[0] && Close[0] > High[1] && Body[0] > Body[1])
				longEntry = 1;

			if (Close[0] > trigger[0] && Close[0] > triggerAverage[0])
				longStayIn = 1;

			if ((Close[0] < trigger[0] || Close[0] < triggerAverage[0]) || longStayIn == 0)
				longExit = 1;

			if (longEntry == 1)
				Long_State[0] = 1;

			if (longExit == 1)
				Long_State[0] = 0;

			double shortEntry = 0, shortStayIn = 0, shortExit = 0;
			Short_State[0] = Short_State[1];

			if (Close[0] < trigger[0] && Close[0] < triggerAverage[0] && Body[0] < trigger[0] && Body[0] < triggerAverage[0] && Close[0] < Low[1] && Body[0] < Body[1])
				shortEntry = 1;

			if (Close[0] < trigger[0] && Close[0] < triggerAverage[0])
				shortStayIn = 1;

			if ((Close[0] > trigger[0] || Close[0] > triggerAverage[0]) || shortStayIn == 0)
				shortExit = 1;

			if (shortEntry == 1)
				Short_State[0] = 1;

			if (shortExit == 1)
				Short_State[0] = 0;				
			
			
//			if (Long_State[0] == 1)
//				BarBrushes[0] = Brushes.DimGray;
			
//			BarBrushes[0] = Brushes.Goldenrod;
//			CandleOutlineBrushes[0] = Brushes.Goldenrod;
			

			
		
//		private System.Windows.Media.Brush UpNeutralBrush;
//		private System.Windows.Media.Brush DownNeutralBrush;
//		private System.Windows.Media.Brush UpBuyBrush;
//		private System.Windows.Media.Brush DownBuyBrush;
//		private System.Windows.Media.Brush UpSellBrush;
//		private System.Windows.Media.Brush DownSellBrush;
		
		
		
			
			
			// Paint bars
//			if(pBarColorEnabled && CurrentBar >= totalBarsRequiredToPlot)
//			{
				
							
			
//						if(Open[0] < Close[0])
//							BarBrushes[0] = UpNeutralBrush;
//						else
//							BarBrushes[0] = DownNeutralBrush;			
			
						
//						CandleOutlineBrushes[0] = pColorNeutralBrush2;
						
						
//				if(displacement >= 0)
//				{
//					if(Long_State[displacement] == 1)
//					{
//						if(Open[0] < Close[0])
//							BarBrushes[0] = UpBuyBrush;
//						else
//							BarBrushes[0] = DownBuyBrush;
//						CandleOutlineBrushes[0] = pColorUpBrush2;
//					}	
//					else if(Short_State[displacement] == 1)
//					{
//						if(Open[0] < Close[0])
//							BarBrushes[0] = UpSellBrush;
//						else
//							BarBrushes[0] = DownSellBrush;
//						CandleOutlineBrushes[0] = pColorDownBrush2;
//					}	
//				}
//				else 
//				{
//					if(Long_State[0] == 1)
//					{
//						if(Open[-displacement] < Close[-displacement])
//							BarBrushes[-displacement] = upBrushUp;
//						else
//							BarBrushes[-displacement] = upBrushDown;
//						CandleOutlineBrushes[-displacement] = pColorUpBrush2;
//					}	
//					else if(Short_State[0] == 1)
//					{
//						if(Open[-displacement] < Close[-displacement])
//							BarBrushes[-displacement] = downBrushUp;
//						else
//							BarBrushes[-displacement] = downBrushDown;
//						CandleOutlineBrushes[-displacement] = pColorDownBrush2;
//					}	
//				}			
//			}
			
			
			
			
			
		}

	
	
//		public override string FormatPriceMarker(double price)
//		{
//			if(indicatorIsOnPricePanel)
//				return Instrument.MasterInstrument.FormatPrice(Instrument.MasterInstrument.RoundToTickSize(price));
//			else
//				return base.FormatPriceMarker(price);
//		}			
		
		public override void OnRenderTargetChanged()
		{

			if (RenderTarget != null)
			{
			
				pStroke01.RenderTarget = RenderTarget;	
				//pStroke02.RenderTarget = RenderTarget;	
	
				
			}
			
		}
		
		
		
		private SharpDX.Direct2D1.Brush ChartBackgroundShade1BrushDX = null;
	
		private bool FirstRender = true;
		private Vector2[] cachedCloudArray;
		private System.Windows.Media.Brush ThisChartBackground = Brushes.Transparent;
		private ChartControlProperties myProperties = null;
		
			
		public System.Windows.Media.Brush  GetTextColor(System.Windows.Media.Brush bg2)
		{
			
			//Color bg = new Pen(bg2,1).;
			System.Windows.Media.Color bg = (bg2 as System.Windows.Media.SolidColorBrush).Color;
			
			
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

		
		protected override void OnRender(ChartControl chartControl, ChartScale chartScale)
		{
			
			
//			if (BarsInProgress == 1)
//				return;
			
			if (ChartBars == null || BarsArray[0].Count < ChartBars.ToIndex || ChartBars.ToIndex < BarsRequiredToPlot || !IsVisible) return;

			
				if (FirstRender)
			{
				
				FirstRender = false;
				
//				if (EnableOrderExecution && connected && pOrderPanelOn)
//					TotalTheOrders();
				
				//if (ChartControl != null)
				{
					//ChartBarsSwitch2(false);
					
					//ChartBars.isv
					//ChartBars.Properties.AutoScale = false;
					
					// Instantiate a ChartControlProperties object to hold a reference to chartControl.Properties
		            myProperties = ChartControl.Properties;
		
//					// Set the AllowSelectionDragging property to false
					
//		            myProperties.AllowSelectionDragging = false;
					
//					AllowSelectionDraggingDefault = myProperties.AllowSelectionDragging;

//					myProperties.ChartBackground = pChartBackgroundBrush;	
//					myProperties.ChartText = pChartAxisBrush;	
					
					
					ChartBars.Properties.PlotExecutions = ChartExecutionStyle.DoNotPlot;
					
					
					
					//ChartBackgroundAxisBrushDX = myProperties.AxisPen.Brush.ToDxBrush(RenderTarget);
					
					if (myProperties.ChartBackground.ToString().Contains("LinearGradientBrush"))
					{
					
						ThisChartBackground = GetTextColor(myProperties.AxisPen.Brush);
						
					}
					else
					{
						ThisChartBackground = myProperties.ChartBackground;
						
					}
					
					//Print(myProperties.ChartBackground.ToString());
					
					//myProperties.ChartBackground.osgr
					
				
						
					ChartBackgroundShade1BrushDX = ThisChartBackground.ToDxBrush(RenderTarget);
					//ChartBackgroundShade1BrushDX.Opacity = 25/100f;
					
					
					
				}
			}
			
			
			
			
			
			ChartBackgroundShade1BrushDX = ThisChartBackground.ToDxBrush(RenderTarget);
			
			SharpDX.Direct2D1.AntialiasMode oldAntialiasMode = RenderTarget.AntialiasMode;
			
			
			
			
			
			
			
			
	
			
			
			
				
			
			// commented out error message because everyone does get basic permission to use this

			
			if (!IsInHitTest)
 			if (AllErrorMessages.Count > 0)
				{
				
					
					ChartBarsSwitch2(false);
					myProperties.AllowSelectionDragging = false;
					
							ChartTextBrushDX = GetTextColor(myProperties.ChartBackground).ToDxBrush(RenderTarget);
			
			
					ChartBackgroundBrushDX = myProperties.ChartBackground.ToDxBrush(RenderTarget);				 			
					
					
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
				
					//Print(txt);
					
					txt = txt + "Click here to continue.";
					//Print(text);
					RenderTarget.FillRectangle(CenterRect, ChartBackgroundBrushDX);
					RenderTarget.FillRectangle(CenterRect, ChartBackgroundErrorBrushDX);
					RenderTarget.DrawText(txt, CenterText, ExpandRect(CenterRect,-10,0), ChartTextBrushDX);
					
					
					RenderTarget.AntialiasMode = oldAntialiasMode;
					
				
				
				ChartBackgroundErrorBrushDX.Dispose();	
				ChartTextBrushDX.Dispose();
				ChartBackgroundBrushDX.Dispose();
					
				CenterText.Dispose();	
				
					//Print("bug2");
					
				return;
			}
			
			
			
			if (!Permission)
				return;

			
			
			
					
			
			
			
			
	
		
			
			SharpDX.Direct2D1.Brush cloudBrushUpDX 	= cloudBrushUp.ToDxBrush(RenderTarget);
			SharpDX.Direct2D1.Brush cloudBrushDownDX = cloudBrushDown.ToDxBrush(RenderTarget);		
			
			
			
			
			
			

	        ChartPanel panel = chartControl.ChartPanels[ChartPanel.PanelIndex];
			
			bool nonEquidistant 	= (chartControl.BarSpacingType == BarSpacingType.TimeBased || chartControl.BarSpacingType == BarSpacingType.EquidistantMulti);
			int	lastBarPainted 	 	= ChartBars.ToIndex;
			
			// fixed to handle Multi Time Frame
			int lastBarCounted		= Inputs[0].Count - 1;
			
			int	lastBarOnUpdate		= lastBarCounted - (Calculate == Calculate.OnBarClose ? 1 : 0);
			int	lastBarIndex		= Math.Min(lastBarPainted, lastBarOnUpdate);
			int firstBarPainted	 	= ChartBars.FromIndex;
			int firstBarIndex  	 	= Math.Max(BarsRequiredToPlot, firstBarPainted);
			int lastIndex			= 0;
			int x 					= 0;
			int y1 					= 0;
			int y2 					= 0;
			int y 					= 0;
			int lastX 				= 0;
			int lastY1 				= 0;
			int lastY2 				= 0;
			int lastY 				= 0;
			int sign 				= 0;
			int lastSign 			= 0;
			int startBar 			= 0;
			int priorStartBar 		= 0;
			int returnBar 			= 0;
			int count	 			= 0;
			double barWidth			= 0;
			
			bool firstLoop 			= true;
			int cloudArraySize = Math.Max(1, 2 * (lastBarIndex - firstBarIndex + Math.Max(0, displacement) + 1));
			if (cachedCloudArray == null || cachedCloudArray.Length < cloudArraySize)
				cachedCloudArray = new Vector2[cloudArraySize];
			Vector2[] cloudArray = cachedCloudArray;
			// plotArray removed — code that used it is commented out
			
			
			
			
			if(lastBarIndex + displacement > firstBarIndex)
			{	
				if (displacement >= 0)
					lastIndex = lastBarIndex + displacement;
				else
					lastIndex = Math.Min(lastBarIndex, lastBarOnUpdate + displacement);
				if(nonEquidistant && lastIndex > lastBarOnUpdate)
					barWidth = Convert.ToDouble(ChartControl.GetXByBarIndex(ChartBars, lastBarPainted) - ChartControl.GetXByBarIndex(ChartBars, lastBarPainted - displacement))/displacement;
				lastY1	= chartScale.GetYByValue(MAFast.GetValueAt(lastIndex - displacement));
				lastY2	= chartScale.GetYByValue(MASlow.GetValueAt(lastIndex - displacement));
				
				
				
				
				//Area
				if(areaIsFilled)
				{	
					lastY = Math.Max(lastY1, lastY2);
					if(lastY1 > lastY2)
						lastSign = -1;
					else if (lastY1 < lastY2)
						lastSign = 1;
					else
						lastSign = 0;
					startBar = lastIndex;
					firstLoop = true;
					
					do
					{
						SharpDX.Direct2D1.PathGeometry 	path;
						SharpDX.Direct2D1.GeometrySink 	sink;
						path = new SharpDX.Direct2D1.PathGeometry(Core.Globals.D2DFactory);
						using (path)
						{
							priorStartBar = startBar;
							if(firstLoop)
								count = -1;
							else
							{	
								count = 0;
								cloudArray[count] = new Vector2(lastX, lastY);
							}	
							for (int idx = startBar; idx >= firstBarIndex; idx --)	
							{
								if(nonEquidistant && displacement > 0 && idx > lastBarCounted)
									x = ChartControl.GetXByBarIndex(ChartBars, lastBarCounted) + (int)((idx - lastBarCounted) * barWidth);
								else
									x = ChartControl.GetXByBarIndex(ChartBars, idx);
								y1 = chartScale.GetYByValue(MAFast.GetValueAt(idx - displacement));   
								y2 = chartScale.GetYByValue(MASlow.GetValueAt(idx - displacement));   
								count = count + 1;
								
								
								if(lastSign < 1 && y1 > y2)
								{
									y = Math.Max(y1,y2);
									sign = -1;
									cloudArray[count] = new Vector2(x,y);
									lastX = x;
									lastY1 = y1;
									lastY2 = y2;
									lastY = y;
									lastSign = -1;
									returnBar = idx;
									startBar = idx - 1;
								}
								else if(lastSign > -1 && y1 < y2)
								{
									y = Math.Max(y1,y2);
									cloudArray[count] = new Vector2(x,y);
									sign = 1;
									lastX = x;
									lastY1 = y1;
									lastY2 = y2;
									lastY = y;
									lastSign = 1;
									returnBar = idx;
									startBar = idx - 1;
								}
								else if (lastSign == 0 && y1 == y2)
								{
									y = y1;
									cloudArray[count] = new Vector2(x,y);
									sign = 0;
									lastX = x;
									lastY1 = y;
									lastY2 = y;
									lastY = y;
									lastSign = 0;
									returnBar = idx;
									startBar = idx - 1;
									break;
								}
								else if (y1 == y2)
								{
									y = y1;
									cloudArray[count] = new Vector2(x,y);
									sign = lastSign;
									lastX = x;
									lastY1 = y;
									lastY2 = y;
									lastY = y;
									lastSign = 0;
									returnBar = idx + 1;
									startBar = idx - 1;
									break;
								}
								else
								{	
									double lastDiff = Convert.ToDouble(lastY2 - lastY1);
									double diff = Convert.ToDouble(y2 - y1);
									double denominator = lastDiff - diff;
									if(denominator.ApproxCompare(0) == 0)
										x = lastX - Convert.ToInt32((lastX - x) * 0.5);
									else	
										x = lastX - Convert.ToInt32((lastX - x) * lastDiff / denominator);
									y = Convert.ToInt32((lastY2 * y1 - y2 * lastY1)/denominator);
									cloudArray[count] = new Vector2(x,y);
									sign = lastSign;
									lastX = x;
									lastY1 = y;
									lastY2 = y;
									lastY = y;
									lastSign = 0;
									returnBar = idx + 1;
									startBar = idx;
									break;
								}
								startBar = idx - 1;
							}
							for (int idx = returnBar ; idx <= priorStartBar; idx ++)	
							{
								if(nonEquidistant && displacement > 0 && idx > lastBarCounted)
									x = ChartControl.GetXByBarIndex(ChartBars, lastBarCounted) + (int)((idx - lastBarCounted) * barWidth);
								else
									x = ChartControl.GetXByBarIndex(ChartBars, idx);
								y1 = chartScale.GetYByValue(MAFast.GetValueAt(idx - displacement));   
								y2 = chartScale.GetYByValue(MASlow.GetValueAt(idx - displacement));
								y = Math.Min(y1, y2);
								count = count + 1;
								cloudArray[count] = new Vector2(x,y);
							}
							sink = path.Open();
							sink.BeginFigure(cloudArray[0], FigureBegin.Filled);
							for (int i = 1; i <= count; i++)
								sink.AddLine(cloudArray[i]);
							sink.EndFigure(FigureEnd.Closed);
			        		sink.Close();
							RenderTarget.AntialiasMode = SharpDX.Direct2D1.AntialiasMode.PerPrimitive;
							if(sign == 1)
							{
								if (pBackFillEnabled)
		 						RenderTarget.FillGeometry(path, ChartBackgroundShade1BrushDX);
								RenderTarget.FillGeometry(path, cloudBrushUpDX);
								
							}
							else if(sign == -1) 
							{
								if (pBackFillEnabled)
								RenderTarget.FillGeometry(path, ChartBackgroundShade1BrushDX);
		 						RenderTarget.FillGeometry(path, cloudBrushDownDX);
								
							}
							RenderTarget.AntialiasMode = oldAntialiasMode;
						}
						path.Dispose();
						sink.Dispose();
						firstLoop = false;
					}	
					while (startBar > firstBarIndex - 1);
				}
				
				// this does not enable multiple colors using plotbrushes
				
				//Plots
//				if(showMAFastLines) 
//				{	
//					SharpDX.Direct2D1.PathGeometry 	pathT;
//					SharpDX.Direct2D1.GeometrySink 	sinkT;
//					pathT = new SharpDX.Direct2D1.PathGeometry(Core.Globals.D2DFactory);
//					using (pathT)
//					{
//						startBar = lastIndex;
//						count = -1;
//						for (int idx = startBar; idx >= firstBarIndex; idx --)	
//						{
//							if(nonEquidistant && idx > lastBarCounted)
//								x = ChartControl.GetXByBarIndex(ChartBars, lastBarCounted) + (int)((idx - lastBarCounted) * barWidth);
//							else
//								x = ChartControl.GetXByBarIndex(ChartBars, idx);
//							y = chartScale.GetYByValue(MAFast.GetValueAt(idx - displacement));   
//							count = count + 1;
//							plotArray[count] = new Vector2(x, y);
//						}
//						sinkT = pathT.Open();
//						sinkT.BeginFigure(plotArray[0], FigureBegin.Filled);
//						for (int i = 1; i <= count; i++)
//							sinkT.AddLine(plotArray[i]);
//						sinkT.EndFigure(FigureEnd.Open);
//		        		sinkT.Close();
//						RenderTarget.AntialiasMode = SharpDX.Direct2D1.AntialiasMode.PerPrimitive;
//						RenderTarget.DrawGeometry(pathT, Plots[0].BrushDX, Plots[0].Width, Plots[0].StrokeStyle);
//						//RenderTarget.DrawGeometry(pathT, PlotBrushes[0].GetDxBrush(idx), Plots[0].Width, Plots[0].StrokeStyle);
						
						
//						RenderTarget.AntialiasMode = oldAntialiasMode;
//					}
//					pathT.Dispose();
//					sinkT.Dispose();
					
//					SharpDX.Direct2D1.PathGeometry 	pathTA;
//					SharpDX.Direct2D1.GeometrySink 	sinkTA;
//					pathTA = new SharpDX.Direct2D1.PathGeometry(Core.Globals.D2DFactory);
//					using (pathTA)
//					{
//						startBar = lastIndex;
//						count = -1;
//						for (int idx = startBar; idx >= firstBarIndex; idx --)	
//						{
//							if(nonEquidistant && idx > lastBarCounted)
//								x = ChartControl.GetXByBarIndex(ChartBars, lastBarCounted) + (int)((idx - lastBarCounted) * barWidth);
//							else
//								x = ChartControl.GetXByBarIndex(ChartBars, idx);
//							y = chartScale.GetYByValue(MASlow.GetValueAt(idx - displacement));   
//							count = count + 1;
//							plotArray[count] = new Vector2(x, y);
//						}
//						sinkTA = pathTA.Open();
//						sinkTA.BeginFigure(plotArray[0], FigureBegin.Filled);
//						for (int i = 1; i <= count; i++)
//							sinkTA.AddLine(plotArray[i]);
//						sinkTA.EndFigure(FigureEnd.Open);
//		        		sinkTA.Close();
//						RenderTarget.AntialiasMode = SharpDX.Direct2D1.AntialiasMode.PerPrimitive;
//						RenderTarget.DrawGeometry(pathTA, Plots[1].BrushDX, Plots[1].Width, Plots[1].StrokeStyle);
//						//RenderTarget.DrawGeometry(pathT, PlotBrushes[1].GetDxBrush(idx), Plots[1].Width, Plots[1].StrokeStyle);
//						RenderTarget.AntialiasMode = oldAntialiasMode;
//					}
//					pathTA.Dispose();
//					sinkTA.Dispose();
//				}
			}			
			cloudBrushUpDX.Dispose();
			cloudBrushDownDX.Dispose();
			
			
			if (ChartBackgroundShade1BrushDX != null)
			ChartBackgroundShade1BrushDX.Dispose();
			
			
			
			
			
			
			
			
			
			
			
				
			
			base.OnRender(chartControl, chartScale);
			
			
			
			
			
			
		}

		
//		public override string DisplayName
//		{
//			get
//			{
//					if (State == State.SetDefaults)
//						return "aiBestMAsTrend";
//					else
//						return Name;
//			}
		
//		}	
		
		
							

		public override string DisplayName
		{
			get
			{
					if (State == State.SetDefaults)
					{
						
						
									
						if (pMTFEnabled)
						{			
							//finalname = finalname + " " + BarsArray[1].BarsType.DisplayName;				
							return "aiBestMAsTrend "  + "(" + pEMA1Fast.ToString() + "/" + pEMA1Slow.ToString() + " " + pThisMAType + "s" + ", " + pMTFBarsPeriod1.ToString() + " " + pMTFBasePeriodType1 + ")";
						}
							
				
						return "aiBestMAsTrend " + "(" + pEMA1Fast.ToString() + "/" + pEMA1Slow.ToString() + " " + pThisMAType + "s" + ")";
						
					}
					else
						return "";
			}
		
		}	
			
		
		
		

//public override string DisplayName
//		{
//			get
//			{
////					if (State == State.SetDefaults)
////						return ThisName + "(a)";
////					else
////						return Name  + "(" + PeriodD + "," + PeriodK + "," + Smooth + ")";

				
//				string finalname = string.Empty;
				
//				if (pStochasticsType == "StochasticsFast")
//					finalname = ThisName + " (Fast " + PeriodDFast + "," + PeriodKFast + ")";
//				else
//					finalname = ThisName + " (" + PeriodD + "," + PeriodK + "," + Smooth + ")";
				
				
//				if (pMTFEnabled)
//				{			
//					//finalname = finalname + " " + BarsArray[1].BarsType.DisplayName;				
//					finalname = finalname + " " + pMTFBarsPeriod1.ToString() + " " + pMTFBasePeriodType1;
//				}
					
//				return finalname;
				
//			}
		
//		}		
			
		
		
				[Browsable(false)]
		[XmlIgnore()]
		public Series<double> MAFast
		{
			get { return Values[0]; }
		}
		
		[Browsable(false)]
		[XmlIgnore()]
		public Series<double> MASlow
		{
			get { return Values[1]; }
		}
		
		[Browsable(false)]
		[XmlIgnore()]
		public Series<double> Signals
		{
			get { return Values[2]; }
		}		

		[Browsable(false)]
		[XmlIgnore()]
		public Series<double> Trend
		{
			get { return trend; }
		}
		
		
		
		
		
  		private bool pMTFEnabled = false;
			[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        [Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", GroupName = "Multi Time Frame", Order = 0)]
        public bool MTFEnabled
        {
            get { return pMTFEnabled; }
            set { pMTFEnabled = value; }
        }
				
	
//		private wyseEMATrendData pBarsType = wyseEMATrendData.Minute;
//		[Display(Name = "Bar Type", GroupName = "Multi Time Frame", Order = 1)]
//		public wyseEMATrendData BarsTypeD
//        {
//            get { return pBarsType; }
//            set { pBarsType = value; }
//        }
	
		
		
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
        [Display(ResourceType = typeof(Custom.Resource), Name = "Bar Period", GroupName = "Multi Time Frame", Order = 2)]
        public int MTFBarsPeriod1
        {
            get { return pMTFBarsPeriod1; }
            set { pMTFBarsPeriod1 = value; }
        }		
		

				
					

		private string pThisMAType = "EMA";
		[Description(""), NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Parameters", Name = "Type", Description = "",  Order = 0)]
		//[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(AllMATypes))]
		public string ThisMAType
		{
			get { return pThisMAType; }
			set { pThisMAType = value; }
		}
		
		internal class AllMATypes : StringConverter
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
				//return new StandardValuesCollection( new String[] {"SMA", "SMMA", "TMA", "WMA", "VWMA", "TEMA", "HMA", "EMA", "VMA", "JMA"} );
				return new StandardValuesCollection( new String[] {"EMA", "HMA", "SMA"} );
			}
		}	
					
					
					
							
        private int pEMA1Fast = 20;
        [Range(1, int.MaxValue), NinjaScriptProperty]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Fast Period", GroupName = "Parameters", Order = 2)]
        public int EMA1Fast
        {
            get { return pEMA1Fast; }
            set { pEMA1Fast = value; }
        }
		
		
        private int pEMA1Slow = 50;
        [Range(1, int.MaxValue), NinjaScriptProperty]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Slow Period", GroupName = "Parameters", Order = 3)]
        public int EMA1Slow
        {
            get { return pEMA1Slow; }
            set { pEMA1Slow = value; }
        }
		
		
		
		
		
//		[NinjaScriptProperty]
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Trend definition", Description = "Select trend definition for the paint bars", GroupName = "Algorithmic Options", Order = 0)]
//		public amaMAFastTrendDefinition TrendDefinition
//		{	
//            get { return trendDefinition; }
//            set { trendDefinition = value; }
//		}
		
		
		
//		[XmlIgnore]
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Fast trigger", Description = "Sets the color for the fast trigger line", GroupName = "Plots", Order = 0)]
//		public System.Windows.Media.Brush MAFastBrush
//		{ 
//			get {return triggerBrush;}
//			set {triggerBrush = value;}
//		}

//		[Browsable(false)]
//		public string MAFastBrushSerializable
//		{
//			get { return Serialize.BrushToString(triggerBrush); }
//			set { triggerBrush = Serialize.StringToBrush(value); }
//		}
		
		
		     
		
 
		private System.Windows.Media.Brush pColorUpBrush1	= Brushes.DodgerBlue;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Color Up", GroupName = "Plots", Order = 4)]
		public System.Windows.Media.Brush ColorUpBrush1
		{
			get { return pColorUpBrush1; } set { pColorUpBrush1 = value; }
		}
		[Browsable(false)]
		public string ColorUpBrush1S
		{
			get { return Serialize.BrushToString(pColorUpBrush1); } set { pColorUpBrush1 = Serialize.StringToBrush(value); }
		}			
		
		private System.Windows.Media.Brush pColorDownBrush1	= Brushes.Red;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Color Down", GroupName = "Plots", Order = 5)]
		public System.Windows.Media.Brush ColorDownBrush1
		{
			get { return pColorDownBrush1; } set { pColorDownBrush1 = value; }
		}
		[Browsable(false)]
		public string ColorDownBrush1S
		{
			get { return Serialize.BrushToString(pColorDownBrush1); } set { pColorDownBrush1 = Serialize.StringToBrush(value); }
		}	
		
		
		
		
		
		
		[Display(ResourceType = typeof(Custom.Resource), Name = "Dash style", Description = "", GroupName = "Plots", Order = 6)]
		public DashStyleHelper Dash0Style
		{
			get { return dash0Style; }
			set { dash0Style = value; }
		}
		
		[Range(1, int.MaxValue)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Width", Description = "", GroupName = "Plots", Order = 7)]
		public int Plot0Width
		{	
            get { return plot0Width; }
            set { plot0Width = value; }
		}
		
			
		
		private int pPlotsOpacity = 50;
		[Display(ResourceType = typeof(Custom.Resource), Name = "Opacity (%)", GroupName = "Plots", Order = 8)]
		[Range(0, 100)]
		public int PlotsOpacity
		{
			get { return pPlotsOpacity; }
			set { pPlotsOpacity = value; }
		}			
		
		
		
			
//		[XmlIgnore]
//		[Display(ResourceType = typeof(Custom.Resource), Name = "MAFast Average", Description = "Sets the color for the trigger average", GroupName = "Plots", Order = 3)]
//		public System.Windows.Media.Brush AverageBrush
//		{ 
//			get {return averageBrush;}
//			set {averageBrush = value;}
//		}

//		[Browsable(false)]
//		public string AverageBrushSerializable
//		{
//			get { return Serialize.BrushToString(averageBrush); }
//			set { averageBrush = Serialize.StringToBrush(value); }
//		}
		
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Dash style average", Description = "Sets the dash style for the trigger average", GroupName = "Plots", Order = 4)]
//		public DashStyleHelper Dash1Style
//		{
//			get { return dash1Style; }
//			set { dash1Style = value; }
//		}
		
//		[Range(1, int.MaxValue)]
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Plot width average", Description = "Sets the plot width for the trigger average", GroupName = "Plots", Order = 5)]
//		public int Plot1Width
//		{	
//            get { return plot1Width; }
//            set { plot1Width = value; }
//		}
		
		
		
		[Display(ResourceType = typeof(Custom.Resource), Name = "Area Fill Enabled", Description = "Fills the space between", GroupName = "Plots", Order = 500)]
        public bool FillArea
        {
            get { return fillArea; }
            set { fillArea = value; }
        }
		
	
		
				
				
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Color Up", Description = "Sets the color for a bullish trend", GroupName = "Plots", Order = 501)]
		public System.Windows.Media.Brush CloudBrushUpS
		{ 
			get {return cloudBrushUpS;}
			set {cloudBrushUpS = value;}
		}

		[Browsable(false)]
		public string CloudBrushUpSSerializable
		{
			get { return Serialize.BrushToString(cloudBrushUpS); }
			set { cloudBrushUpS = Serialize.StringToBrush(value); }
		}
		
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Color Down", Description = "Sets the color for a bullish trend", GroupName = "Plots", Order = 502)]
		public System.Windows.Media.Brush CloudBrushDownS
		{ 
			get {return cloudBrushDownS;}
			set {cloudBrushDownS = value;}
		}

		[Browsable(false)]
		public string CloudBrushDownSSerializable
		{
			get { return Serialize.BrushToString(cloudBrushDownS); }
			set { cloudBrushDownS = Serialize.StringToBrush(value); }
		}
		
		[Range(0, 100)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Opacity (%)", Description = "Sets the opacity for the cloud", GroupName = "Plots", Order = 503)]
		public int AreaOpacity
		{	
            get { return areaOpacity; }
            set { areaOpacity = value; }
		}
		
		
		
		
		
		
		
		
        private bool pBackFillEnabled = false;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Background Fill Enabled", Description = "", GroupName = "Plots", Order = 504)]
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        public bool BackFillEnabled
        {
            get { return pBackFillEnabled; }
            set { pBackFillEnabled = value; }
        }	
		
		

	
		private string pLicensingEmailAddress = "";
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "License", Name = "Email Address", Order = 54, Description = "")]
        public string LicensingEmailAddress
        {
            get { return pLicensingEmailAddress; }
            set { pLicensingEmailAddress = value; }
        }			
				
		
		
//		[XmlIgnore]
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Bullish Upclose", Description = "Sets the color for a bullish trend", GroupName = "Paint Bars", Order = 0)]
//		public System.Windows.Media.Brush UpBrushUp
//		{ 
//			get {return upBrushUp;}
//			set {upBrushUp = value;}
//		}

//		[Browsable(false)]
//		public string UpBrushUpSerializable
//		{
//			get { return Serialize.BrushToString(upBrushUp); }
//			set { upBrushUp = Serialize.StringToBrush(value); }
//		}
		
//		[XmlIgnore]
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Bullish Downclose", Description = "Sets the color for a bullish trend", GroupName = "Paint Bars", Order = 1)]
//		public System.Windows.Media.Brush UpBrushDown
//		{ 
//			get {return upBrushDown;}
//			set {upBrushDown = value;}
//		}

//		[Browsable(false)]
//		public string UpBrushDownSerializable
//		{
//			get { return Serialize.BrushToString(upBrushDown); }
//			set { upBrushDown = Serialize.StringToBrush(value); }
//		}
		
//		[XmlIgnore]
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Bullish candle outline", Description = "Sets the color for candle outlines", GroupName = "Paint Bars", Order = 2)]
//		public System.Windows.Media.Brush UpBrushOutline
//		{ 
//			get {return upBrushOutline;}
//			set {upBrushOutline = value;}
//		}

//		[Browsable(false)]
//		public string UpBrushOutlineSerializable
//		{
//			get { return Serialize.BrushToString(upBrushOutline); }
//			set { upBrushOutline = Serialize.StringToBrush(value); }
//		}					
		
//		[XmlIgnore]
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Bearish Upclose", Description = "Sets the color for a bearish trend", GroupName = "Paint Bars", Order = 3)]
//		public System.Windows.Media.Brush DownBrushUp
//		{ 
//			get {return downBrushUp;}
//			set {downBrushUp = value;}
//		}

//		[Browsable(false)]
//		public string DownBrushUpSerializable
//		{
//			get { return Serialize.BrushToString(downBrushUp); }
//			set { downBrushUp = Serialize.StringToBrush(value); }
//		}
		
//		[XmlIgnore]
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Bearish Downclose", Description = "Sets the color for a bearish trend", GroupName = "Paint Bars", Order = 4)]
//		public System.Windows.Media.Brush DownBrushDown
//		{ 
//			get {return downBrushDown;}
//			set {downBrushDown = value;}
//		}

//		[Browsable(false)]
//		public string DownBrushDownSerializable
//		{
//			get { return Serialize.BrushToString(downBrushDown); }
//			set { downBrushDown = Serialize.StringToBrush(value); }
//		}
		
//		[XmlIgnore]
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Bearish candle outline", Description = "Sets the color for candle outlines", GroupName = "Paint Bars", Order = 5)]
//		public System.Windows.Media.Brush DownBrushOutline
//		{ 
//			get {return downBrushOutline;}
//			set {downBrushOutline = value;}
//		}

//		[Browsable(false)]
//		public string DownBrushOutlineSerializable
//		{
//			get { return Serialize.BrushToString(downBrushOutline); }
//			set { downBrushOutline = Serialize.StringToBrush(value); }
//		}
		
	
		
		private Stroke pStroke01 = new Stroke(Brushes.DimGray, DashStyleHelper.Solid, 1);
//        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Plots", Name = "MA", Order = 13)]
//        public Stroke Stroke01
//        {
//            get { return pStroke01; }
//            set { pStroke01 = value; }
//        }					
				
//		private Stroke pStroke02 = new Stroke(Brushes.DimGray, DashStyleHelper.Solid, 1);
//        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Plots", Name = "Slow EMA", Order = 12)]
//        public Stroke Stroke02
//        {
//            get { return pStroke02; }
//            set { pStroke02 = value; }
//        }			
		
			
		
		
		
		
		
		
//        private bool pBarColorEnabled = false;
//        [Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", Description = "", GroupName = "Bar Color", Order = 0)]
//		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
//        public bool BarColorEnabled
//        {
//            get { return pBarColorEnabled; }
//            set { pBarColorEnabled = value; }
//        }	
		
		

//		private System.Windows.Media.Brush pColorUpBrush	= Brushes.LimeGreen;
//		[XmlIgnore]
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Buy Color (Fill)", GroupName = "Bar Color", Order = 1)]
//		public System.Windows.Media.Brush ColorUpBrush
//		{
//			get { return pColorUpBrush; } set { pColorUpBrush = value; }
//		}
//		[Browsable(false)]
//		public string ColorUpBrushS
//		{
//			get { return Serialize.BrushToString(pColorUpBrush); } set { pColorUpBrush = Serialize.StringToBrush(value); }
//		}	

//		private System.Windows.Media.Brush pColorUpBrush2	= Brushes.LimeGreen;
//		[XmlIgnore]
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Buy Color (Outline)", GroupName = "Bar Color", Order = 2)]
//		public System.Windows.Media.Brush ColorUpBrush2
//		{
//			get { return pColorUpBrush2; } set { pColorUpBrush2 = value; }
//		}
//		[Browsable(false)]
//		public string ColorUpBrush2S
//		{
//			get { return Serialize.BrushToString(pColorUpBrush2); } set { pColorUpBrush2 = Serialize.StringToBrush(value); }
//		}	
		
		
//		private System.Windows.Media.Brush pColorDownBrush	= Brushes.DarkRed;
//		[XmlIgnore]
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Sell Color (Fill)", GroupName = "Bar Color", Order = 3)]
//		public System.Windows.Media.Brush ColorDownBrush
//		{
//			get { return pColorDownBrush; } set { pColorDownBrush = value; }
//		}
//		[Browsable(false)]
//		public string ColorDownBrushS
//		{
//			get { return Serialize.BrushToString(pColorDownBrush); } set { pColorDownBrush = Serialize.StringToBrush(value); }
//		}	

//		private System.Windows.Media.Brush pColorDownBrush2	= Brushes.DarkRed;
//		[XmlIgnore]
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Sell Color (Outline)", GroupName = "Bar Color", Order = 4)]
//		public System.Windows.Media.Brush ColorDownBrush2
//		{
//			get { return pColorDownBrush2; } set { pColorDownBrush2 = value; }
//		}
//		[Browsable(false)]
//		public string ColorDownBrush2S
//		{
//			get { return Serialize.BrushToString(pColorDownBrush2); } set { pColorDownBrush2 = Serialize.StringToBrush(value); }
//		}	
		
		
		
//		private System.Windows.Media.Brush pColorNeutralBrush	= Brushes.Goldenrod;
//		[XmlIgnore]
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Neutral Color (Fill)", GroupName = "Bar Color", Order = 5)]
//		public System.Windows.Media.Brush ColorNeutralBrush
//		{
//			get { return pColorNeutralBrush; } set { pColorNeutralBrush = value; }
//		}
//		[Browsable(false)]
//		public string ColorNeutralBrushS
//		{
//			get { return Serialize.BrushToString(pColorNeutralBrush); } set { pColorNeutralBrush = Serialize.StringToBrush(value); }
//		}	

//		private System.Windows.Media.Brush pColorNeutralBrush2	= Brushes.Goldenrod;
//		[XmlIgnore]
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Neutral Color (Outline)", GroupName = "Bar Color", Order = 6)]
//		public System.Windows.Media.Brush ColorNeutralBrush2
//		{
//			get { return pColorNeutralBrush2; } set { pColorNeutralBrush2 = value; }
//		}
//		[Browsable(false)]
//		public string ColorNeutralBrush2S
//		{
//			get { return Serialize.BrushToString(pColorNeutralBrush2); } set { pColorNeutralBrush2 = Serialize.StringToBrush(value); }
//		}	
		
	
		
//		private int pBarOpacityUp = 20;
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Up Bar Opacity (%)", GroupName = "Bar Color", Order = 40)]
//		[Range(0, 100)]
//		public int BarOpacityUp
//		{
//			get { return pBarOpacityUp; }
//			set { pBarOpacityUp = value; }
//		}			
		
		
//		private int pBarOpacityDown = 80;
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Down Bar Opacity (%)", GroupName = "Bar Color", Order = 41)]
//		[Range(0, 100)]
//		public int BarOpacityDown
//		{
//			get { return pBarOpacityDown; }
//			set { pBarOpacityDown = value; }
//		}				
		
		
		

		
		
		
//		private int pBarOpacitySignal = 80; 
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Signal Opacity (%)", GroupName = "Bar Color", Order = 42)]
//		[Range(0, 100)]
//		public int BarOpacitySignal
//		{
//			get { return pBarOpacitySignal; }
//			set { pBarOpacitySignal = value; }
//		}				
				
	
		// EMAIL


		private bool pEmailEnabled = false;
		[RefreshProperties(RefreshProperties.All)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", GroupName = "Email", Order = 1)]
		public bool EmailEnabled
		{
			get { return pEmailEnabled; }
			set { pEmailEnabled = value; }
		}

		private bool pQuickEmail = false;
		[Display(ResourceType = typeof(Custom.Resource), Name = "IntraBar", GroupName = "Email", Order = 2)]
		public bool QuickEmail
		{
			get { return pQuickEmail; }
			set { pQuickEmail = value; }
		}


		private string pEmailAddress = @"";
		[Display(ResourceType = typeof(Custom.Resource), Name = "Email Address", GroupName = "Email", Order = 3)]
		public string EmailAddress
		{
			get { return pEmailAddress; }
			set { pEmailAddress = value; }
		}



		
		


		private bool pAudioEnabled = false;
		[RefreshProperties(RefreshProperties.All)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", Description = "", GroupName = "Audio", Order = 0)]
		public bool AudioEnabled
		{
			get { return pAudioEnabled; }
			set { pAudioEnabled = value; }
		}

		private bool pQuickAudio = false;
		[Display(ResourceType = typeof(Custom.Resource), Name = "IntraBar", GroupName = "Audio", Order = 1)]
		public bool QuickAudio
		{
			get { return pQuickAudio; }
			set { pQuickAudio = value; }
		}

		private string pWAVFileName3 = "Alert2.wav";
		//		[Display(ResourceType = typeof(Custom.Resource), Name = "WAV Cross Long", Description = "Sound file to play when a cross sell signal occurs.", GroupName = "Audio", Order = 3)]
		//		public string WAVFileName3
		//		{
		//			get { return pWAVFileName3; }
		//			set { pWAVFileName3 = value; }
		//		}		

		private string pWAVFileName4 = "Alert2.wav";
		//		[Display(ResourceType = typeof(Custom.Resource), Name = "WAV Cross Short", Description = "Sound file to play when a cross sell signal occurs.", GroupName = "Audio", Order = 4)]
		//		public string WAVFileName4
		//		{
		//			get { return pWAVFileName4; }
		//			set { pWAVFileName4 = value; }
		//		}		



		
	private string pWAVFileName = "Alert2.wav";
		[Display(ResourceType = typeof(Custom.Resource), Name = "WAV Buy", Description = "Sound file to play when a buy signal occurs.", GroupName = "Audio", Order = 5)]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(LoadFileList))]
		public string WAVFileName
		{
			get { return pWAVFileName; }
			set { pWAVFileName = value; }
		}

		private string pWAVFileName2 = "Alert2.wav";
		[Display(ResourceType = typeof(Custom.Resource), Name = "WAV Sell", Description = "Sound file to play when a sell signal occurs.", GroupName = "Audio", Order = 6)]
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
		

	
		     
		
		
		private System.Windows.Media.Brush pColorUpBrush2 = Brushes.DodgerBlue;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Alert Log Buy", GroupName = "Audio", Order = 8)]
		public System.Windows.Media.Brush ColorUpBrush2
		{
			get { return pColorUpBrush2; }
			set { pColorUpBrush2 = value; }
		}
		[Browsable(false)]
		public string ColorUpBrush2S
		{
			get { return Serialize.BrushToString(pColorUpBrush2); }
			set { pColorUpBrush2 = Serialize.StringToBrush(value); }
		}


		private System.Windows.Media.Brush pColorDownBrush2 = Brushes.Red;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Alert Log Sell", GroupName = "Audio", Order = 10)]
		public System.Windows.Media.Brush ColorDownBrush2
		{
			get { return pColorDownBrush2; }
			set { pColorDownBrush2 = value; }
		}
		[Browsable(false)]
		public string ColorDownBrush2S
		{
			get { return Serialize.BrushToString(pColorDownBrush2); }
			set { pColorDownBrush2 = Serialize.StringToBrush(value); }
		}
		
		
		
		
	}
	
	
	
	
	
	
	
	
	
	
	
	
			// Hide UserDefinedValues properties when not in use by the HLCCalculationMode.UserDefinedValues
	// When creating a custom type converter for indicators it must inherit from NinjaTrader.NinjaScript.IndicatorBaseConverter to work correctly with indicators
	public class aiBestMAsTrendConverter : NinjaTrader.NinjaScript.IndicatorBaseConverter
	{
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) { return true; }

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			PropertyDescriptorCollection propertyDescriptorCollection = base.GetPropertiesSupported(context) ? base.GetProperties(context, value, attributes) : TypeDescriptor.GetProperties(value, attributes);

			aiBestMAsTrend   jbb = (aiBestMAsTrend) value;
			
			//Pivots						thisPivotsInstance			= (Pivots) value;
			
			//bool MagnetsOn = ;
			
			List<string> DeleteThese = new List<string>();
			List<string> DeleteThese2 = new List<string>();
				
	
			//DeleteThese.Add("Calculate");
			DeleteThese.Add("Name");
      		DeleteThese.Add("MaximumBarsLookBack");
			
			DeleteThese.Add("Input");
			DeleteThese.Add("InputSeries");
			
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
   
		

		

			if (!jbb.EmailEnabled)
			{

				DeleteThese.Add("QuickEmail");
				DeleteThese.Add("EmailAddress");

			}

			if (!jbb.AudioEnabled)
			{

				DeleteThese.Add("QuickAudio");
				DeleteThese.Add("WAVFileName");
				DeleteThese.Add("WAVFileName2");
				DeleteThese.Add("ColorUpBrush2");
				DeleteThese.Add("ColorDownBrush2");


			}

			
		

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



//public enum wyseEMATrendData
//{
//	Minute  	= BarsPeriodType.Minute,
//	Range   	= BarsPeriodType.Range,
//	Second  	= BarsPeriodType.Second,
//	Tick    	= BarsPeriodType.Tick,
//	Volume  	= BarsPeriodType.Volume,
//	Renko   	= BarsPeriodType.Renko,
////	Day 		= BarsPeriodType.Day,
////	Week   		= BarsPeriodType.Week,
////	Month		= BarsPeriodType.Month	
	
//}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private aiBestMAsTrend[] cacheaiBestMAsTrend;
		public aiBestMAsTrend aiBestMAsTrend(string mTFBasePeriodType1, int mTFBarsPeriod1, string thisMAType, int eMA1Fast, int eMA1Slow)
		{
			return aiBestMAsTrend(Input, mTFBasePeriodType1, mTFBarsPeriod1, thisMAType, eMA1Fast, eMA1Slow);
		}

		public aiBestMAsTrend aiBestMAsTrend(ISeries<double> input, string mTFBasePeriodType1, int mTFBarsPeriod1, string thisMAType, int eMA1Fast, int eMA1Slow)
		{
			if (cacheaiBestMAsTrend != null)
				for (int idx = 0; idx < cacheaiBestMAsTrend.Length; idx++)
					if (cacheaiBestMAsTrend[idx] != null && cacheaiBestMAsTrend[idx].MTFBasePeriodType1 == mTFBasePeriodType1 && cacheaiBestMAsTrend[idx].MTFBarsPeriod1 == mTFBarsPeriod1 && cacheaiBestMAsTrend[idx].ThisMAType == thisMAType && cacheaiBestMAsTrend[idx].EMA1Fast == eMA1Fast && cacheaiBestMAsTrend[idx].EMA1Slow == eMA1Slow && cacheaiBestMAsTrend[idx].EqualsInput(input))
						return cacheaiBestMAsTrend[idx];
			return CacheIndicator<aiBestMAsTrend>(new aiBestMAsTrend(){ MTFBasePeriodType1 = mTFBasePeriodType1, MTFBarsPeriod1 = mTFBarsPeriod1, ThisMAType = thisMAType, EMA1Fast = eMA1Fast, EMA1Slow = eMA1Slow }, input, ref cacheaiBestMAsTrend);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.aiBestMAsTrend aiBestMAsTrend(string mTFBasePeriodType1, int mTFBarsPeriod1, string thisMAType, int eMA1Fast, int eMA1Slow)
		{
			return indicator.aiBestMAsTrend(Input, mTFBasePeriodType1, mTFBarsPeriod1, thisMAType, eMA1Fast, eMA1Slow);
		}

		public Indicators.aiBestMAsTrend aiBestMAsTrend(ISeries<double> input , string mTFBasePeriodType1, int mTFBarsPeriod1, string thisMAType, int eMA1Fast, int eMA1Slow)
		{
			return indicator.aiBestMAsTrend(input, mTFBasePeriodType1, mTFBarsPeriod1, thisMAType, eMA1Fast, eMA1Slow);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.aiBestMAsTrend aiBestMAsTrend(string mTFBasePeriodType1, int mTFBarsPeriod1, string thisMAType, int eMA1Fast, int eMA1Slow)
		{
			return indicator.aiBestMAsTrend(Input, mTFBasePeriodType1, mTFBarsPeriod1, thisMAType, eMA1Fast, eMA1Slow);
		}

		public Indicators.aiBestMAsTrend aiBestMAsTrend(ISeries<double> input , string mTFBasePeriodType1, int mTFBarsPeriod1, string thisMAType, int eMA1Fast, int eMA1Slow)
		{
			return indicator.aiBestMAsTrend(input, mTFBasePeriodType1, mTFBarsPeriod1, thisMAType, eMA1Fast, eMA1Slow);
		}
	}
}

#endregion
