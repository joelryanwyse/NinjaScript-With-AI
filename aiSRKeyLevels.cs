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
	
	/// <summary>
	/// Key Levels Confluence Indicator
	/// </summary>	
	/// 
	
	[Gui.CategoryOrder("Zones", 0)]
	[Gui.CategoryOrder("Levels", 1)]
	
		
			
	
	
	[Gui.CategoryOrder("OHLC", 2)]
	
	[Gui.CategoryOrder("Fibonacci", 5)]
	[Gui.CategoryOrder("Whole Numbers", 6)]
	[Gui.CategoryOrder("Pivots", 7)]
	[Gui.CategoryOrder("Current Day", 8)]
	[Gui.CategoryOrder("Zones Display", 9)]
	[Gui.CategoryOrder("Zone Intelligence", 10)]
	[Gui.CategoryOrder("Levels Display", 15)]
	[Gui.CategoryOrder("Labels Display", 17)]
	
	[Gui.CategoryOrder("Chart Buttons", 19)]
	
	
	
	[Gui.CategoryOrder("Time Zones", 23)]
	[Gui.CategoryOrder("ADR", 24)]
	[Gui.CategoryOrder("Menu", 25)]
	
	
	
	
	[Gui.CategoryOrder("Visual", 156)]
	[Gui.CategoryOrder("Data Series", 165)]
	
	
	
	[Gui.CategoryOrder("Setup", 9000)]
	[Gui.CategoryOrder("License", 10000)]
	
	
	
		 
	/// <summary>
	/// Plots the open, high, low and close values from the session starting on the prior day.
	/// </summary>
	/// 
	
	[TypeConverter("NinjaTrader.NinjaScript.Indicators.aiSRKeyLevelsConverter")]	
	public class aiSRKeyLevels : Indicator
	{
		
		private string ThisName = "aiSRKeyLevels";
		
		

			
		private string LicensingMessage = string.Empty;		

		private SortedDictionary<int, int> ProductIDToMachineIDs = new SortedDictionary<int, int>();
		private ConcurrentDictionary<int, List<string>> ProductIDToInstruments = new ConcurrentDictionary<int, List<string>>();
	
		
		private bool Permission = false;
		

		
		private void AddError(string eee)
		{
		
			if (pErrorMessagesEnabled)
				AllErrorMessages.Add(eee); // HashSet handles duplicates
			
		}
		private bool pErrorMessagesEnabled = true;		
			private HashSet<string> AllErrorMessages = new HashSet<string>();		
		

			


		
			
		// Copies the NinjaTrader Machine ID to the clipboard. Must run on the UI (STA)
		// thread — Clipboard APIs throw on worker threads. Call site may be any thread.
		private void CopyMachineIdToClipboard()
		{
			try
			{
				string id = NinjaTrader.Cbi.License.MachineId ?? "";
				if (string.IsNullOrEmpty(id)) return;

				var dispatcher = System.Windows.Application.Current != null
					? System.Windows.Application.Current.Dispatcher : null;
				if (dispatcher == null) return;

				if (dispatcher.CheckAccess())
					System.Windows.Clipboard.SetText(id);
				else
					dispatcher.BeginInvoke(new Action(() =>
					{
						try { System.Windows.Clipboard.SetText(id); } catch { }
					}));
			}
			catch { }
		}

		private bool LicenseWordPress (string machineid, string pLicensingEmailAddress)
		{

			pLicensingEmailAddress = pLicensingEmailAddress.Replace(" ", "");

			// Fallback: if the passed email is empty/invalid but the AI.txt cache has
			// a valid one, use it. Prevents the first-time onboarding banner from
			// flashing on reload when the field is briefly empty before NT8 property
			// deserialization completes on a freshly-instantiated indicator instance.
			if (string.IsNullOrEmpty(pLicensingEmailAddress) || !pLicensingEmailAddress.Contains("@"))
			{
				try
				{
					string _aiFile = NinjaTrader.Core.Globals.UserDataDir + "AI.txt";
					if (File.Exists(_aiFile))
					{
						string[] _lines = File.ReadAllLines(_aiFile);
						if (_lines != null && _lines.Length > 0
							&& !string.IsNullOrWhiteSpace(_lines[0]) && _lines[0].Contains("@"))
						{
							pLicensingEmailAddress = _lines[0].Trim();
						}
					}
				}
				catch { }
			}

			List<int> ThisProductMainIDs = new List<int>();
			List<int> ThisProductSecondaryIDs = new List<int>();
			
			// Product IDs for Main Indicator
			
			ThisProductMainIDs.Add(8573);
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
				// TLS already set at startup; ensure for this request
				try { System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12; } catch { }
				System.Net.ServicePointManager.ServerCertificateValidationCallback =
				    delegate { return true; };

				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
				request.Method = "GET";
				request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36";
				request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
				request.Headers.Add("Accept-Language", "en-US,en;q=0.5");
				request.Headers.Add("Cache-Control", "no-cache");
				request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
				request.KeepAlive = true;
				request.Timeout = 15000;
				request.ReadWriteTimeout = 15000;

				// Use system proxy so VPS/corporate setups that route through a proxy can reach the internet
				try { request.Proxy = System.Net.WebRequest.GetSystemWebProxy(); if (request.Proxy != null) request.Proxy.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials; } catch { request.Proxy = null; }

				int maxRetries = 2;
				int retryDelayMs = 1500;
				bool requestSucceeded = false;
				for (int attempt = 1; attempt <= maxRetries && !requestSucceeded; attempt++)
				{
					try
					{
						using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
						using (StreamReader reader = new StreamReader(response.GetResponseStream()))
						{
						    result = reader.ReadToEnd();
						}
						requestSucceeded = true;
					}
					catch
					{
						if (attempt < maxRetries)
							System.Threading.Thread.Sleep(retryDelayMs);
						else
							throw;
					}
				}
			}
			catch (Exception ex)
			{
				// can't find the file at all / connection failed (e.g. VPS firewall blocking outbound)

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
							LicensingMessage = "Hello! You are using software from 'Affordable Indicators, Inc.' on this computer for the first time!\r\n\r\nPlease enter the 'Email Address' connected to your Affordable Indicators, Inc. website account.\r\n\r\nClick now to enter your email address.";
						}
						else if (!pLicensingEmailAddress.Contains(@"@"))
						{
							LicensingMessage = "Hello! You are using software from 'Affordable Indicators, Inc.' on this computer for the first time!\r\n\r\nPlease enter the 'Email Address' connected to your Affordable Indicators, Inc. website account.\r\n\r\nClick now to enter your email address.";
						}
						else
						{
							LicensingMessage = "Your 'Email Address' was not found in the Affordable Indicators, Inc. database.\r\n\r\nClick now to enter a different email address.\r\n\r\nContact " + pContactEmail + " if you need further assistance.";
						}

					}
					else if (machineidstring == "ID0")
					{
						// no machine ids connected to account — auto-copy Machine ID to clipboard
						// so the user can immediately paste it into the Members Area.
						CopyMachineIdToClipboard();

						LicensingMessage = "Your NinjaTrader Machine ID has been copied to the clipboard. Please login to your Affordable Indicators, Inc. account, go to the Members Area, and paste your NinjaTrader Machine ID. After adding your NinjaTrader Machine ID in the Members Area, click here to continue.\r\n\r\nContact " + pContactEmail + " if you need further assistance. We are grateful for you!";

					}
					else if (machineidstring == "IDX")
					{
						// no machine ids match — auto-copy Machine ID to clipboard
						CopyMachineIdToClipboard();

						LicensingMessage = "Your NinjaTrader Machine ID has changed. Your NinjaTrader Machine ID has been copied to the clipboard. Please login to your Affordable Indicators, Inc. account, go to the Members Area, and paste your NinjaTrader Machine ID. After adding your NinjaTrader Machine ID in the Members Area, click here to continue.\r\n\r\nContact " + pContactEmail + " if you need further assistance. We are grateful for you!";

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

					

		
		
		
		
		
			
		

		private Point MP;
        private SharpDX.RectangleF B2 = new SharpDX.RectangleF(0, 0, 0, 0);
        private bool InMenu;
        private bool InMenuP;
        private bool ButtonOff = false;
		private double CurrentMousePrice = 0;		
        private int space = 6;
        SortedDictionary<double, ButtonZ> AllButtonZ = new SortedDictionary<double, ButtonZ>();
	
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
			
		// textLayout2 field removed - replaced by MeasureText cache
		

		private double dpiX = 0;
		private	double dpiY = 0;
		
		
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
					
		
        private bool MouseIn(SharpDX.RectangleF RR, int XF, int YF)
        {
            //Print(RR.Left);
            
			if (FinalXPixel != 0)
            if (FinalXPixel >= RR.Left - XF && FinalXPixel <= RR.Right + XF && FinalYPixel >= RR.Top - YF && FinalYPixel <= RR.Bottom + YF)
                return true;
           
                return false;

        }
		
		
		
		List<SharpDX.RectangleF> AllRefreshRects = new List<SharpDX.RectangleF>();
		
				
		
		
			
		
		
		
			
     	private List <double> AllWholeNumbers = new List<double>();
		
		
		Stroke ThisStroke = new Stroke(Brushes.DarkGreen, DashStyleHelper.Solid, 2);
		
		
			private List<List<string>> AllStrings = new List<List<string>>();
			private List<List<int>> AllColors = new List<List<int>>();
		
				
		private SharpDX.RectangleF			rect222;
		
		private int LastZoneStart = 0;
		
		private DateTime 				currentDate 		=	Core.Globals.MinDate;
		private double					currentClose		=	0;
		private double					currentHigh			=	0;
		private double					currentLow			=	0;
		private double					currentOpen			=	0;
		private double					priorDayClose		=	0;
		private double					priorDayHigh		=	0;
		private double					priorDayLow			=	0;
		private double					priorDayOpen		=	0;
		
		private	Data.SessionIterator	sessionIterator;

		private double preval = 0;
		private double val = 0;
		
		private double prey = 0;
		private bool FirstOne = false;
		
		private string ChartName = string.Empty;
		
		
		
		
		
		
			private int PlotFlagsStart = 0;
		
		private bool DisplayZone = false;
		
		private string PriceString = string.Empty;
		private int PriceDigits = 0;
		
		private Series<double> 	BackColorI;
		private Series<double> 	CurrentADR10;
		private Series<double> 	CurrentADR20;
		private Series<double> 	CurrentADRToday;

		private double HolidayIgnore = 0;
		private double HolidayIgnore2 = 0;
		
		private bool AllowedToPlot = false;
		
				
		
		public class ObjectToDraw
		{
			SharpDX.RectangleF rectangle;
			SharpDX.Direct2D1.Brush brush;
			Brush brush2;
			string text;
			double price;
			int startBar;
			
			
			public SharpDX.RectangleF Rectangle { get{return rectangle;} set{rectangle = value; }}			
			public SharpDX.Direct2D1.Brush Brush { get{return brush;} set{brush = value; }}
			public Brush Brush2 { get{return brush2;} set{brush2 = value; }}
			public string Text { get{return text;} set{text = value; }}
			public double Price { get{return price;} set{price = value; }}
			public int StartBar { get{return startBar;} set{startBar = value; }}
		}
		
		List<ObjectToDraw> TextObjectsToDraw = new List<ObjectToDraw>();
		List<ObjectToDraw> FlagObjectsToDraw = new List<ObjectToDraw>();

		// Object pool for ObjectToDraw to avoid per-level per-frame allocations
		private List<ObjectToDraw> objectPool = new List<ObjectToDraw>();
		private int objectPoolIndex = 0;

		private ObjectToDraw GetPooledObject()
		{
			if (objectPoolIndex < objectPool.Count)
				return objectPool[objectPoolIndex++];
			var obj = new ObjectToDraw();
			objectPool.Add(obj);
			objectPoolIndex++;
			return obj;
		}
		
		
		
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
		
		
		
//		private struct Zone {   //LIST
//			public DateTime iiDate;
//			public double iiLow;
//			public double iiHigh;
//			public int iiWidth;
//			public DashStyle iiDashStyle;
//			public Color iiColor;
//			public int iiOpacity;
//			public string iiName;
			
//			public Zone(DateTime iDate, double iLow, double iHigh, int iWidth, DashStyle iDashStyle, Color iColor, int iOpacity, string iName ) {this.iiDate = iDate; this.iiLow = iLow; this.iiHigh = iHigh; this.iiWidth = iWidth; this.iiDashStyle = iDashStyle; this.iiColor = iColor; this.iiOpacity = iOpacity; this.iiName = iName;}
//		}
		
		
		private bool Su1, Su2, Su3, Su4, Su5, Su6, Su7, Su8 = false;
		
		private List<Zone> zones;   //LIST
		private List<ZoneB> zoness = new List<ZoneB>();
	
		
		private List<double> DR;   //LIST
		
		
 		private bool FileError = false;	

		private int FirstMeasurement = 0;
	
		private double HH, H1, H2, H3, H4, H5, H6, H7, H8 = 0;
		private double F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12, F13, F14, F15, F16, F17, F18, F19, F20, F21 = 0;
		
		private double CLF1, CLF2, CLF3, CLF4 = 0;
		private double CHF1, CHF2, CHF3, CHF4 = 0;
		private double CM = 0;
		
		private bool AH, AL, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15 = false;
	
		private double L1, L2, L3, L4, L5, L6, L7, L8 = 999999;
		private double LL = 999999;
	
		private double TT, TT1, TT2 = 0;
		private double DHH = 0;
		private double DLL = 999999;
		private double DTT = 0;

	
		private double RANGE = 0;
		private double PCLOSE, POPEN, PP, S1, S2, S3, S4, S5, S6, R1, R2, R3, R4, R5, R6 = 0;
		private double XXXXX = 0;
	
		private Series<string> VLS;
		

		private Series<List<Zone>> VLsS;
		
		private bool FirstEnd = false;	
	
		private bool RunInit = true;
		private DateTime StartTime, EndTime, FinalTime, CurrentTime, LastBarTime, FirstTime, ExpireTime;
		private DateTime R1TTime, R2TTime, R3TTime, R4TTime, R5TTime, R6TTime, R7TTime, R8TTime;
	
		private DateTime FirstBarTime;
	
		
		
		private TimeSpan pAsianStart = new TimeSpan(18,00,0);
		private TimeSpan pAsianEnd = new TimeSpan(3,0,0);
	
		private TimeSpan pLondonStart = new TimeSpan(3,00,0);
		private TimeSpan pLondonEnd = new TimeSpan(11,30,0);
	
		private TimeSpan pNYStart = new TimeSpan(9,30,0);
		private TimeSpan pNYEnd = new TimeSpan(16,0,0);
	
		private TimeSpan pSwingStart = new TimeSpan(10,0,0);
		private TimeSpan pSwingEnd = new TimeSpan(11,0,0);
		
		
		
//		private TimeSpan pAsianStart = new TimeSpan(19,00,0);
//		private TimeSpan pAsianEnd = new TimeSpan(0,0,0);
	
//		private TimeSpan pLondonStart = new TimeSpan(2,00,0);
//		private TimeSpan pLondonEnd = new TimeSpan(5,0,0);
	
//		private TimeSpan pNYStart = new TimeSpan(8,30,0);
//		private TimeSpan pNYEnd = new TimeSpan(10,0,0);
	
//		private TimeSpan pSwingStart = new TimeSpan(10,0,0);
//		private TimeSpan pSwingEnd = new TimeSpan(11,0,0);
	
		private DateTime AsianSTime, AsianETime, LondonSTime, LondonETime, NYSTime, NYETime, SwingSTime, SwingETime;
	
		private DateTime Midnight;
	
	
		private DateTime LaunchedAt = DateTime.MinValue;
		private bool UseChartData = false;
		private double CurrentHigh = 0;
		private double CurrentLow = 9999999;
	
		private bool PlotNow = false;

		private int DS = 0;
	
		private int HoursFromEST = 0;
	
		private double DSHH, MSHH = 0;
		private double DSLL, MSLL = 9999999;
	
		private double ALL, PALL = 9999999;
		private double AHH, PAHH = 0;
	
		private double LLL, PLLL = 9999999;
		private double LHH, PLHH = 0;
	
		
		private double DLHH, DLSHH, MHH = 0;
		private double DLLL, DLSLL, MLL = 9999999;		
	
		private double PDSHH, PMSHH = 0;
		private double PDSLL, PMSLL = 9999999;

		// Globex (overnight) session tracking
		private double GHH = 0;
		private double GLL = 9999999;
		private double PGHH = 0;
		private double PGLL = 9999999;
		private List<double> AllGlobexHighs = new List<double>();
		private List<double> AllGlobexLows = new List<double>();

		private TimeSpan D1,D2,D3,D4,D5,D6;
		private int B1,B3,B4,B5,B6 = 0;
	
		
		private struct Zone {   //LIST
			public DateTime iiDate;
			public double iiLow;
			public double iiHigh;
			public int iiWidth;
			public DashStyle iiDashStyle;
			public Color iiColor;
			public int iiOpacity;
			public string iiName;
			
			
			public Zone(DateTime iDate, double iLow, double iHigh, int iWidth, DashStyle iDashStyle, Color iColor, int iOpacity, string iName ) {this.iiDate = iDate; this.iiLow = iLow; this.iiHigh = iHigh; this.iiWidth = iWidth; this.iiDashStyle = iDashStyle; this.iiColor = iColor; this.iiOpacity = iOpacity; this.iiName = iName;}
		}
		
		private class ZoneB {
			public int iiBar;
			public double iiLow;
			public double iiHigh;
			public int iiWidth;
			public string iiName;
			public SortedDictionary<string, double> iiDict;
			public double iiScore;     // Zone strength score
			public double iiCentroid;  // Weighted center-of-gravity price

			public ZoneB(int iBar, double iLow, double iHigh, int iWidth, string iName, SortedDictionary<string, double> iDict) {
				this.iiBar = iBar; this.iiLow = iLow; this.iiHigh = iHigh; this.iiWidth = iWidth;
				this.iiName = iName; this.iiDict = iDict; this.iiScore = 0; this.iiCentroid = 0;
			}
		}
		
		SortedList<int, List<ZoneB>> ZonesByBar = new SortedList<int, List<ZoneB>>();
		
		SortedList<int, List<ZoneB>> ZonesByBarStart = new SortedList<int, List<ZoneB>>();
		// Dictionary instead of SortedDictionary - only uses ContainsKey/Add, O(1) vs O(log n)
		SortedList<int, Dictionary<string, double>> LevelsInZonesByBar = new SortedList<int, Dictionary<string, double>>();
		SortedList<int, Dictionary<string, double>> LevelsAtTopOfZonesByBar = new SortedList<int, Dictionary<string, double>>();
		SortedList<int, Dictionary<string, double>> LevelsAtBottomOfZonesByBar = new SortedList<int, Dictionary<string, double>>();
		
		
		// Dictionary instead of SortedDictionary - only uses ContainsKey/Add, no iteration
		Dictionary<int, Dictionary<string, double>> LevelsHoveredByBar = new Dictionary<int, Dictionary<string, double>>();
		
		
		SortedList<int, int> ZonesByBarEnd = new SortedList<int, int>();
		
		
		
		
		SortedList<int, SortedDictionary<string, double>> AllLevelsByBar = new SortedList<int, SortedDictionary<string, double>>();
		
		//SortedList<double> Diffs = new SortedList<double>();
		
		SortedDictionary<string, double> AllDailyLevels = new SortedDictionary<string, double>();
		
		
		SortedDictionary<string, double> AllDailyLevelsRounded = new SortedDictionary<string, double>();
			
		
		
		SortedDictionary<string, double> AllDevelopingLevels = new SortedDictionary<string, double>();
		
		private struct DailyB {   //LIST
			public double iiLow;
			public double iiHigh;
			public double iiOpen;
			public double iiClose;
			public SortedDictionary<string, double> iiDict;
			
			public DailyB(double iLow, double iHigh, double iOpen, double iClose, SortedDictionary<string, double> iDict ) {this.iiLow = iLow; this.iiHigh = iHigh; this.iiOpen = iOpen; this.iiClose = iClose;  this.iiDict = iDict;}
		}
		
		
		
		List<DailyB> AllDailyBars =  new List<DailyB>();
		
		SortedDictionary<string, double> TopToDistance = new SortedDictionary<string, double>();
		
		SortedDictionary<string, double> BottomToDistance = new SortedDictionary<string, double>();
		SortedDictionary<string, string> TopToBottom = new SortedDictionary<string, string>();
		
	
		private bool FirstPlot = true;

				SortedDictionary<string, double> NameLevelsD = new SortedDictionary<string, double>();
		
            private double retracement1 = 38.2; // Default setting for Retracement1
            private double retracement2 = 50.000; // Default setting for Retracement2
            private double retracement3 = 61.8; // Default setting for Retracement3
            private double retracement4 = 78.6; // Default setting for Retracement4
			private double retracement5 = 70.5;
		
		private bool NewHigh, NewLow, MakingOR = false;
		
			private double DHH2, HH2 = 0;
			private double DLL2, LL2 = 99999;
		
		
			private bool NewZones = false;
		
			private DateTime StartTime2, EndTime2;
		
		
		private NinjaTrader.Gui.Chart.ChartTab		chartTab;
		private NinjaTrader.Gui.Chart.Chart			chartWindow;
		private bool								isToolBarButtonAdded;
		private System.Windows.DependencyObject		searchObject;
		private System.Windows.Controls.TabItem		tabItem;
		private System.Windows.Controls.Menu		theMenu;
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItem;
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem1;
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem2;
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem3;
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem4;				 
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem5;
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem6;
		
		
		 
		protected void InsertWPFControls()
		{
			chartWindow = System.Windows.Window.GetWindow(ChartControl.Parent) as Chart;

			chartWindow.MainTabControl.SelectionChanged += MySelectionChangedHandler;

			foreach (System.Windows.DependencyObject item in chartWindow.MainMenu)
				if (System.Windows.Automation.AutomationProperties.GetAutomationId(item) == "NOBSSupportResistance")
					return;

			// this is the actual object that you add to the chart windows Main Menu
			// which will act as a container for all the menu items
			theMenu = new System.Windows.Controls.Menu
			{
				// important to set the alignment, otherwise you will never see the menu populated
				VerticalAlignment			= VerticalAlignment.Top,
				VerticalContentAlignment	= VerticalAlignment.Top,

				// make sure to style as a System Menu	
				Style						= System.Windows.Application.Current.TryFindResource("SystemMenuStyle") as Style
			};

			System.Windows.Automation.AutomationProperties.SetAutomationId(theMenu, "NOBSSupportResistance");

			// thanks to Jesse for these figures to use t
			System.Windows.Media.Geometry topMenuItem1Icon = System.Windows.Media.Geometry.Parse("m 70.5 173.91921 c -4.306263 -1.68968 -4.466646 -2.46776 -4.466646 -21.66921 0 -23.88964 -1.364418 -22.5 22.091646 -22.5 23.43572 0 22.08568 -1.36412 22.10832 22.33888 0.0184 19.29356 -0.19638 20.3043 -4.64473 21.85501 -2.91036 1.01455 -32.493061 0.99375 -35.08859 -0.0247 z M 21 152.25 l 0 -7.5 20.25 0 20.25 0 0 7.5 0 7.5 -20.25 0 -20.25 0 0 -7.5 z m 93.75 0 0 -7.5 42.75 0 42.75 0 0 7.5 0 7.5 -42.75 0 -42.75 0 0 -7.5 z m 15.75 -38.33079 c -4.30626 -1.68968 -4.46665 -2.46775 -4.46665 -21.66921 0 -23.889638 -1.36441 -22.5 22.09165 -22.5 23.43572 0 22.08568 -1.364116 22.10832 22.338885 0.0185 19.293555 -0.19638 20.304295 -4.64473 21.855005 -2.91036 1.01455 -32.49306 0.99375 -35.08859 -0.0247 z M 21 92.25 l 0 -7.5 50.25 0 50.25 0 0 7.5 0 7.5 -50.25 0 -50.25 0 0 -7.5 z m 153.75 0 0 -7.5 12.75 0 12.75 0 0 7.5 0 7.5 -12.75 0 -12.75 0 0 -7.5 z M 55.5 53.919211 C 51.193737 52.229528 51.033354 51.451456 51.033354 32.25 51.033354 8.3603617 49.668936 9.75 73.125 9.75 96.560723 9.75 95.210685 8.3858835 95.23332 32.088887 95.25177 51.382441 95.03694 52.393181 90.588593 53.943883 87.678232 54.95844 58.095529 54.93764 55.5 53.919211 Z M 21 32.25 l 0 -7.5 12.75 0 12.75 0 0 7.5 0 7.5 -12.75 0 -12.75 0 0 -7.5 z m 78.75 0 0 -7.5 50.25 0 50.25 0 0 7.5 0 7.5 -50.25 0 -50.25 0 0 -7.5 z");

			// this is the menu item which will appear on the chart's Main Menu
			topMenuItem = new Gui.Tools.NTMenuItem()
			{
				Header				= "SR",
				Foreground			= pMenuBrush,
				Icon				= topMenuItem1Icon,
				Margin				= new System.Windows.Thickness(0),
				Padding				= new System.Windows.Thickness(1),
				VerticalAlignment	= VerticalAlignment.Center,
				Style				= System.Windows.Application.Current.TryFindResource("MainMenuItem") as Style
			};

			theMenu.Items.Add(topMenuItem);

			// ITEM 1
		
			topMenuItemSubItem1 = new Gui.Tools.NTMenuItem()
			{
				BorderThickness		= new System.Windows.Thickness(0),
				Header				= "Levels",
				Style				= System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style
			};
			
			topMenuItemSubItem1.IsCheckable = true;
			topMenuItemSubItem1.IsChecked = pActive2;
			
			topMenuItemSubItem1.Click += TopMenuItem1SubItem1_Click;
			topMenuItem.Items.Add(topMenuItemSubItem1);

			// ITEM 2
			
			topMenuItemSubItem2 = new Gui.Tools.NTMenuItem()
			{
				Header				= "Labels (Name)",
				Style				= System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style
			};
			
			topMenuItemSubItem2.IsCheckable = true;
			topMenuItemSubItem2.IsChecked = pLabelsEnabled;
			
			topMenuItemSubItem2.Click += TopMenuItem1SubItem2_Click;
			topMenuItem.Items.Add(topMenuItemSubItem2);			

			// ITEM 3
			
//			topMenuItemSubItem3 = new Gui.Tools.NTMenuItem()
//			{
//				Header				= "Labels (Price)",
//				Style				= System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style
//			};
			
//			topMenuItemSubItem3.IsCheckable = true;
//			topMenuItemSubItem3.IsChecked = PaintPriceMarkers;
			
//			topMenuItemSubItem3.Click += TopMenuItem1SubItem3_Click;
//			topMenuItem.Items.Add(topMenuItemSubItem3);				
			
			// ITEM 4
			
			topMenuItemSubItem4 = new Gui.Tools.NTMenuItem()
			{
				Header				= "ADR",
				Style				= System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style
			};
			
			topMenuItemSubItem4.IsCheckable = true;
			topMenuItemSubItem4.IsChecked = pActive6;
			
			topMenuItemSubItem4.Click += TopMenuItem1SubItem4_Click;
			topMenuItem.Items.Add(topMenuItemSubItem4);					
			
//			// ITEM 5
			
//			topMenuItemSubItem5 = new Gui.Tools.NTMenuItem()
//			{
//				Header				= "Text (Volume)",
//				Style				= System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style
//			};
			
//			topMenuItemSubItem5.IsCheckable = true;
//			topMenuItemSubItem5.IsChecked = pShowVolume;
			
//			topMenuItemSubItem5.Click += TopMenuItem1SubItem5_Click;
//			topMenuItem.Items.Add(topMenuItemSubItem5);	
			
			
		
			
		
		
			
			// add the menu which contains all menu items to the chart
			chartWindow.MainMenu.Add(theMenu);

			foreach (System.Windows.Controls.TabItem tab in chartWindow.MainTabControl.Items)
				if ((tab.Content as ChartTab).ChartControl == ChartControl && tab == chartWindow.MainTabControl.SelectedItem)
					topMenuItem.Visibility = Visibility.Visible;

			chartWindow.MainTabControl.SelectionChanged += MySelectionChangedHandler;
		}

		private void MySelectionChangedHandler(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count <= 0)
				return;

			tabItem = e.AddedItems[0] as System.Windows.Controls.TabItem;
			if (tabItem == null)
				return;

			chartTab = tabItem.Content as NinjaTrader.Gui.Chart.ChartTab; 
			if (chartTab != null)
				if (topMenuItem != null)
					topMenuItem.Visibility = chartTab.ChartControl == ChartControl ? Visibility.Visible : Visibility.Collapsed;
		}

		

		protected void RemoveWPFControls()
		{
			if (topMenuItemSubItem1 != null)
				topMenuItemSubItem1.Click -= TopMenuItem1SubItem1_Click;

			if (topMenuItemSubItem2 != null)
				topMenuItemSubItem2.Click -= TopMenuItem1SubItem2_Click;

			if (theMenu != null)
				chartWindow.MainMenu.Remove(theMenu);

			chartWindow.MainTabControl.SelectionChanged -= MySelectionChangedHandler;
		}

		protected void TopMenuItem1SubItem1_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			pActive2 = topMenuItemSubItem1.IsChecked;

			ChartControl.InvalidateVisual();
		}

		protected void TopMenuItem1SubItem2_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			pLabelsEnabled = topMenuItemSubItem2.IsChecked;

			ChartControl.InvalidateVisual();
		}
		
		protected void TopMenuItem1SubItem3_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			PaintPriceMarkers = topMenuItemSubItem3.IsChecked;

			ChartControl.InvalidateVisual();
		}

		protected void TopMenuItem1SubItem4_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			pActive6 = topMenuItemSubItem4.IsChecked;

			ChartControl.InvalidateVisual();
		}
		
//		protected void TopMenuItem1SubItem5_Click(object sender, System.Windows.RoutedEventArgs e)
//		{
//			pShowVolume = topMenuItemSubItem5.IsChecked;

//			ChartControl.InvalidateVisual();
//		}

//		protected void TopMenuItem1SubItem6_Click(object sender, System.Windows.RoutedEventArgs e)
//		{
//			pShowYesterday = topMenuItemSubItem5.IsChecked;

//			ChartControl.InvalidateVisual();
//		}
		 		

		private int StartFlagPlotN = 23;
		private int EndFlagPlotN = 100;
		
		private int DaysNeeded = 0;
		private int DaysLoaded = 0;
		
	
		private string Descccc = @"Key Levels Confluence Indicator by Joel Ryan Wyse.";
		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				
					
				Name = ThisName;
				Description					= Descccc;
				
				
				IsAutoScale					= false;
				IsOverlay					= true;
				IsSuspendedWhileInactive	= true;
				DrawOnPricePanel			= false;
				DisplayInDataBox = false;
                ArePlotsConfigurable = false;
                AreLinesConfigurable = false;			
				
				PaintPriceMarkers = true;
				
				
				Calculate				= Calculate.OnPriceChange;

				AddPlot(new Stroke(Brushes.SeaGreen, DashStyleHelper.Solid, 1),	PlotStyle.Line, "1");
				AddPlot(new Stroke(Brushes.SeaGreen, DashStyleHelper.Solid, 1), PlotStyle.Line, "2");
				AddPlot(new Stroke(Brushes.Firebrick,	DashStyleHelper.Solid, 1), PlotStyle.Line, "3");
				AddPlot(new Stroke(Brushes.Orange, DashStyleHelper.Solid, 1), PlotStyle.Line, "4");
				AddPlot(new Stroke(Brushes.Orange, DashStyleHelper.Solid, 1),	PlotStyle.Line, "5");
				AddPlot(new Stroke(Brushes.Orange, DashStyleHelper.Solid, 1), PlotStyle.Line, "6");
				AddPlot(new Stroke(Brushes.Orange,	DashStyleHelper.Solid, 1), PlotStyle.Line, "7");
				AddPlot(new Stroke(Brushes.Orange, DashStyleHelper.Solid, 1), PlotStyle.Line, "8");
				AddPlot(new Stroke(Brushes.Orange, DashStyleHelper.Solid, 1), PlotStyle.Line, "9");
				AddPlot(new Stroke(Brushes.Lime, DashStyleHelper.Solid, 1), PlotStyle.Line, "10");
				AddPlot(new Stroke(Brushes.Lime, DashStyleHelper.Solid, 1), PlotStyle.Line, "11");
				AddPlot(new Stroke(Brushes.SlateGray, DashStyleHelper.Solid, 1), PlotStyle.Line, "12");
				AddPlot(new Stroke(Brushes.SlateGray, DashStyleHelper.Solid, 1), PlotStyle.Line, "13");
				AddPlot(new Stroke(Brushes.Purple, DashStyleHelper.Solid, 1), PlotStyle.Line, "14");	
				AddPlot(new Stroke(Brushes.Green,	DashStyleHelper.Solid, 1), PlotStyle.Line, "15");
				AddPlot(new Stroke(Brushes.Green, DashStyleHelper.Solid, 1), PlotStyle.Line, "16");
				AddPlot(new Stroke(Brushes.Green, DashStyleHelper.Solid, 1), PlotStyle.Line, "17");
				AddPlot(new Stroke(Brushes.Red, DashStyleHelper.Solid, 1), PlotStyle.Line, "18");
				AddPlot(new Stroke(Brushes.Red, DashStyleHelper.Solid, 1), PlotStyle.Line, "19");
				AddPlot(new Stroke(Brushes.Red, DashStyleHelper.Solid, 1), PlotStyle.Line, "20");
				AddPlot(new Stroke(Brushes.SeaGreen, DashStyleHelper.Solid, 1), PlotStyle.Line, "21");
				AddPlot(new Stroke(Brushes.SeaGreen, DashStyleHelper.Solid, 1), PlotStyle.Line, "22");	
				
				int thisplotn = StartFlagPlotN;
					for (int i = thisplotn; i<=EndFlagPlotN; i++) // flag plots for Y-axis price markers
					{
						AddPlot(new Stroke(Brushes.Transparent, DashStyleHelper.Solid, 1), PlotStyle.Dot, i.ToString());
					}				
				
				
				
				
				
				
				Plots[11].Name = "London Range High";
				Plots[12].Name = "London Range Low";
				Plots[13].Name = "Pivot Point";
				Plots[14].Name = "Daily R1";
				Plots[15].Name = "Daily R2";
				Plots[16].Name = "Daily R3";
				Plots[17].Name = "Daily S1";
				Plots[18].Name = "Daily S2";
				Plots[19].Name = "Daily S3";
				Plots[20].Name = "Range Low";
				Plots[21].Name = "Range High";				
				
			
					
				
				TextFont						= new SimpleFont("Arial",11);
				
				Level1Stroke.Opacity = 60;
				Level2Stroke.Opacity = 60;
				Level3Stroke.Opacity = 60;
				Level4Stroke.Opacity = 60;
				Level5Stroke.Opacity = 60;
				Level6Stroke.Opacity = 60;
				Level7Stroke.Opacity = 100;
				
				
			}
			else if (State == State.Configure)
			{
				
				Description					= Descccc;
				
				DaysNeeded = 0;

				DaysNeeded = Math.Max(DaysNeeded, pShowDaysHighLow);
				DaysNeeded = Math.Max(DaysNeeded, pShowDaysClose);
				DaysNeeded = Math.Max(DaysNeeded, pShowDaysOpen);
				
				
				DaysNeeded = Math.Max(DaysNeeded, pShowDaysMid);
				DaysNeeded = Math.Max(DaysNeeded, pShowDaysFibs);
				
				DaysNeeded = Math.Max(DaysNeeded, pPivotPointDays);
				DaysNeeded = Math.Max(DaysNeeded, pPivotRegularLevelDays);
				DaysNeeded = Math.Max(DaysNeeded, pPivotFibonacciLevelDays);
				DaysNeeded = Math.Max(DaysNeeded, pPivotCamarillaLevelDays);
				DaysNeeded = Math.Max(DaysNeeded, pPivotDemarkPointDays);
				DaysNeeded = Math.Max(DaysNeeded, pPivotDemarkLevelDays);
				DaysNeeded = Math.Max(DaysNeeded, pPivotWoodiesPointDays);
				DaysNeeded = Math.Max(DaysNeeded, pPivotWoodiesLevelDays);
				DaysNeeded = Math.Max(DaysNeeded, pShowDaysGlobex);
		
			
				
				//Print(DaysNeeded);
				
				// 8 days per week
				//DaysNeeded = (int) (DaysNeeded/5*8);
		
				//Print(DaysNeeded);
		
				int pNumberOfDaysToLoadMinute1 = DaysNeeded;
				int FinalBarsToLoad = 0;
				
				UseChartData = false;
				
				//ChartBars.Properties.DaysBack = DaysNeeded;
				
				
				if (!UseChartData)
				{
					
					
					
					pThisBarPeriod1 = 1;
					
					// fix when added to 1 min chart 
					
					if (BarsArray[0].BarsType.BarsPeriod.BarsPeriodType == BarsPeriodType.Minute && BarsArray[0].BarsType.BarsPeriod.Value == 1)
					{
						pThisBarPeriod1 = 5;
						
					}
					
					
					
					BarsPeriod BP = new BarsPeriod();					
					BP.BarsPeriodType = AcceptableBasePeriodType1;
					BP.Value = pThisBarPeriod1;
					
					//FinalBarsToLoad = (int) ((pNumberOfDaysToLoadMinute1 * 1440 / pThisBarPeriod1) / 7 * 5);
					//FinalBarsToLoad = (int) (pNumberOfDaysToLoadMinute1 * 1440 / pThisBarPeriod1);
					
					FinalBarsToLoad = (int) ((pNumberOfDaysToLoadMinute1 * 1440 / pThisBarPeriod1) / 5 * 7);
					
//					Print(BP);
//					Print(FinalBarsToLoad);
					
					AddDataSeries(null, BP, FinalBarsToLoad, null, true);					
					
					
					//Print(DaysNeeded);
					
					// old data load
					
					//AddDataSeries(BarsPeriodType.Minute, 1);
				
					
					
					//SN = 1;	
				}
				
			
			}
			

				
			if (State == State.DataLoaded)
			{
			
				
				

				if (Name != ThisName && Name != string.Empty)
					Name = ThisName;	
				
				Plots[0].Name = "High [C]";
				Plots[1].Name = "Low [C]";	
				Plots[2].Name = "Middle [C]";
				Plots[3].Name = pPercentCD1.ToString() + " H [C]";
				Plots[4].Name = pPercentCD2.ToString() + " H [C]";
				Plots[5].Name = pPercentCD3.ToString() + " H [C]";
				Plots[6].Name = pPercentCD4.ToString() + " H [C]";
				Plots[7].Name = pPercentCD1.ToString() + " L [C]";
				Plots[8].Name = pPercentCD2.ToString() + " L [C]";
				Plots[9].Name = pPercentCD3.ToString() + " L [C]";
				Plots[10].Name = pPercentCD4.ToString() + " L [C]";
				
				
				
				Plots[0].Brush = pLevel2Stroke.Brush;
				//Plots[0].Opacity = pLevel2Stroke.Opacity;
				
				Plots[1].Brush = pLevel2Stroke.Brush;
				//Plots[1].Opacity = pLevel2Stroke.Opacity;
				
				Plots[2].Brush = pLevel1Stroke.Brush;
				//Plots[2].Opacity = pLevel1Stroke.Opacity;				
				
				for (int i=3; i<=10; i++)
				{
				
					Plots[i].Brush = pLevel4Stroke.Brush;
					//Plots[i].Opacity = pLevel4Stroke.Opacity;					
					
				}
	
				for (int i=0; i<=10; i++)
				{
				
					Plots[i].Brush = Brushes.Transparent;
					//Plots[i].Opacity = pLevel4Stroke.Opacity;					
					
				}				
				
				
			
		
		
					
									
										
										
							
			
				
				
				
			
				double thiswholenumber = 0;
				double AutomaticNumber = 0;
				
				if (Instrument.MasterInstrument.Name.Contains("NQ") || Instrument.MasterInstrument.Name.Contains("YM"))
				{
					AutomaticNumber = 100;

				}
				else if (Instrument.MasterInstrument.Name.Contains("ES") 
					|| Instrument.MasterInstrument.Name.Contains("RTY") 
					|| Instrument.MasterInstrument.Name.Contains("M2K") 
					|| Instrument.MasterInstrument.Name.Contains("GC") 
					|| Instrument.MasterInstrument.Name.Contains("ZS") 
					|| Instrument.MasterInstrument.Name.Contains("ZW") 
					)
				{
				
					AutomaticNumber = 10;
					

				}	
				else if (Instrument.MasterInstrument.Name.Contains("CL")
					|| Instrument.MasterInstrument.Name.Contains("ZB")
					|| Instrument.MasterInstrument.Name.Contains("ZB") 
					|| Instrument.MasterInstrument.Name.Contains("ZN") 
					
					)
				{
				
					AutomaticNumber = 1;
					

				}	
					
				//Print(AllWholeNumbers.Count);
				
				
				
				
				thiswholenumber = 0;
				
				if (pAutoNumberEnabled)
				if (AutomaticNumber != 0)
				{
					while(thiswholenumber <= 1000000) 
					{
						AllWholeNumbers.Add(thiswholenumber);
						thiswholenumber = thiswholenumber + AutomaticNumber;
					}
				}
								
				
				
				
				thiswholenumber = 0;
				if (pWholeNumber1 != 0)
				{
					while(thiswholenumber <= 1000000) 
					{
						AllWholeNumbers.Add(thiswholenumber);
						thiswholenumber = thiswholenumber + pWholeNumber1;
					}
				}
				
					
					
				//Print(AllWholeNumbers.Count);
				
					
				thiswholenumber = 0;
				if (pWholeNumber2 != 0)
				{
					while(thiswholenumber <= 1000000) 
					{
						//if (!AllWholeNumbers.Contains(thiswholenumber))
							AllWholeNumbers.Add(thiswholenumber);					
						thiswholenumber = thiswholenumber + pWholeNumber2;	
						
					}
				}
			
				
				//Print(AllWholeNumbers.Count);
				
				
				
				thiswholenumber = 0;
				if (pWholeNumber3 != 0)
				{
					while(thiswholenumber <= 1000000) 
					{
						//if (!AllWholeNumbers.Contains(thiswholenumber))
							AllWholeNumbers.Add(thiswholenumber);					
						thiswholenumber = thiswholenumber + pWholeNumber3;	
						
					}
				}				
				
				thiswholenumber = 0;
				
				
				//Print(AllWholeNumbers.Count);
				
				// Use HashSet for O(1) dedup instead of Distinct().OrderBy() on large list
				var wholeNumberSet = new HashSet<double>(AllWholeNumbers);
				AllWholeNumbers = new List<double>(wholeNumberSet);
				AllWholeNumbers.Sort();
				
				//Print(AllWholeNumbers.Count);
				
				
				
				
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
					ChartPanel.MouseWheel += new MouseWheelEventHandler(OnMouseWheel);
                }
								
				
				
				
				if (!Permission)
					return;
				
				
					
					
					
currentDate 	    = Core.Globals.MinDate;
				currentClose		= 0;
				currentHigh			= 0;
				currentLow			= 0;
				currentOpen			= 0;
				priorDayClose		= 0;
				priorDayHigh		= 0;
				priorDayLow			= 0;
				priorDayOpen		= 0;
				sessionIterator		= null;
				

				//Permission = CheckLicense(NinjaTrader.Cbi.License.MachineId, "");
			
				
				
				
				
				
				string FS = TickSize.ToString();
				if(FS.Contains("E-")) {
					FS = FS.Substring(FS.IndexOf("E-")+2);
					PriceDigits = int.Parse(FS);
				}
				else PriceDigits = Math.Max(0,FS.Length-2);
				PriceString = "#0";
				if (PriceDigits > 0)
					PriceString = PriceString + ".";
				for (int i = 1; i <= PriceDigits; i++)
					PriceString = PriceString + "0";
				
				//UseChartData = true;
				
// SET PIT OPEN AND CLOSE TIMES
			
			pCustomTime = new TimeSpan(0,00,0);
			pCustomETime = new TimeSpan(16,15,0);
				
				
			// set intra day levels recalcualte times

				
				
			
			if (Instrument.MasterInstrument.Name.Contains("CL"))
			{
				pStartTime2 = new TimeSpan(9,00,0);
				//pStartTime = new TimeSpan(11,23,0);
				pEndTime2 = new TimeSpan(14,30,0);
				
				
//				pStartTime2 = pStartTime;
//				pEndTime2 = new TimeSpan(9,05,0);
				
				//17:15
				//18:00
				
				//Print(Instrument.MasterInstrument.Name);

			}
			else if (Instrument.MasterInstrument.Name.Contains("GC") || Instrument.MasterInstrument.Name.Contains("HG") )
			{
			
				pStartTime2 = new TimeSpan(8,20,0);
				pEndTime2 = new TimeSpan(13,30,0);
				
				

			}			
			else
			{
				pStartTime2 = new TimeSpan(9,30,0);
				pEndTime2 = new TimeSpan(16,15,0);
							
				//16:15
				//16:30
				
				
			}
				
			
			pR1Time = pStartTime2.Add(new TimeSpan(0,0,30,0));
			pR2Time = pR1Time.Add(new TimeSpan(0,0,60,0));
			pR3Time = pR2Time.Add(new TimeSpan(0,0,60,0));
			pR4Time = pR3Time.Add(new TimeSpan(0,0,60,0));	
			
			
			
			pR5Time = new TimeSpan(8,00,0);	
			
			// SET TO LOCAL TIME ZONE
			
			
			//TimeZoneInfo LocalTZ = TimeZoneInfo.FindSystemTimeZoneById(TimeZone.CurrentTimeZone.StandardName);
			
			TimeZoneInfo LocalTZ = TimeZoneInfo.Local;
			double HoursLocalMinusUTC = LocalTZ.GetUtcOffset(DateTime.Now.ToUniversalTime()).TotalHours;
			
			double HoursNYMinusUTC = -5;
				
			try
			{
			
				TimeZoneInfo EasternTZ = TimeZoneInfo.FindSystemTimeZoneById("US Eastern Standard Time");
				HoursNYMinusUTC = EasternTZ.GetUtcOffset(DateTime.Now.ToUniversalTime()).TotalHours;
			}
			catch
			{
				Log(Name + " - Did not successfully find US Eastern Standard Time in the list of time zones.",LogLevel.Error);
				
			}
			
			//Print(HoursNYMinusUTC);
				
				
//			Print("CT.id: "+LocalTZ.Id+"  "+LocalTZ.GetUtcOffset(DateTime.Now.ToUniversalTime()).TotalHours);
//			Print("ET.id: "+EasternTZ.Id+"  "+EasternTZ.GetUtcOffset(DateTime.Now.ToUniversalTime()).TotalHours);
				
				
//			Print("UTC offset: "+TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).ToString());
//			Print("UTC time: "+TimeZone.CurrentTimeZone.ToUniversalTime(DateTime.Now).ToString());
			//StartTime  = new DateTime(Time[0].Year, Time[0].Month, Time[0].Day, pStartTime.Hours, pStartTime.Minutes, 0);
			HoursFromEST = (int) (HoursLocalMinusUTC - HoursNYMinusUTC);
			
			
			//HoursFromEST = 1;
			
			//HoursFromEST = 15;
			
			HolidayIgnore = 0+HoursFromEST;
			HolidayIgnore2 = 13+HoursFromEST;
			
			if (HolidayIgnore2 < 0)
				HolidayIgnore2 = HolidayIgnore2 + 24;
			
			
			if (HolidayIgnore < 0)
				HolidayIgnore = HolidayIgnore + 24;
			
			//Print(HoursFromEST);	
			
			
			
			pStartTime = new TimeSpan(18,00,0);	
			pEndTime = new TimeSpan(17,15,0);
			
			
			
			if (HoursFromEST < 0)
			{
				//pCustomTime = pCustomTime.Subtract(new TimeSpan(0,Math.Abs(HoursFromEST),0,0));
				
				pStartTime2 = pStartTime2.Subtract(new TimeSpan(0,Math.Abs(HoursFromEST),0,0));
				pEndTime2 = pEndTime2.Subtract(new TimeSpan(0,Math.Abs(HoursFromEST),0,0));
		
				
				pStartTime = pStartTime.Subtract(new TimeSpan(0,Math.Abs(HoursFromEST),0,0));
				pEndTime = pEndTime.Subtract(new TimeSpan(0,Math.Abs(HoursFromEST),0,0));
				pR5Time = pR5Time.Subtract(new TimeSpan(0,Math.Abs(HoursFromEST),0,0));
				
				if (pCustomTime.Hours < Math.Abs(HoursFromEST))
					pCustomTime = new TimeSpan(0,HoursFromEST+pCustomTime.Hours+24,0,0);
				else
					pCustomTime = pCustomTime.Subtract(new TimeSpan(0,Math.Abs(HoursFromEST),0,0));				
				
				
				if (pAsianStart.Hours < Math.Abs(HoursFromEST))
					pAsianStart = new TimeSpan(0,HoursFromEST+pAsianStart.Hours+24,0,0);
				else
					pAsianStart = pAsianStart.Subtract(new TimeSpan(0,Math.Abs(HoursFromEST),0,0));
				
				
				if (pLondonStart.Hours < Math.Abs(HoursFromEST))
					pLondonStart = new TimeSpan(0,HoursFromEST+pLondonStart.Hours+24,0,0);	
				else
					pLondonStart = pLondonStart.Subtract(new TimeSpan(0,Math.Abs(HoursFromEST),0,0));
				
				if (pNYStart.Hours < Math.Abs(HoursFromEST))
					pNYStart = new TimeSpan(0,HoursFromEST+pNYStart.Hours+24,0,0);	
				else
					pNYStart = pNYStart.Subtract(new TimeSpan(0,Math.Abs(HoursFromEST),0,0));				
				
				if (pSwingStart.Hours < Math.Abs(HoursFromEST))
					pSwingStart = new TimeSpan(0,HoursFromEST+pSwingStart.Hours+24,0,0);	
				else
					pSwingStart = pSwingStart.Subtract(new TimeSpan(0,Math.Abs(HoursFromEST),0,0));	
				
				//Print(pAsianEnd.Hours);
				
				if (pAsianEnd.Hours < Math.Abs(HoursFromEST))
					pAsianEnd = new TimeSpan(0,HoursFromEST+pAsianEnd.Hours+24,0,0);
				else
					pAsianEnd = pAsianEnd.Subtract(new TimeSpan(0,Math.Abs(HoursFromEST),0,0));
				
				
				if (pLondonEnd.Hours < Math.Abs(HoursFromEST))
					pLondonEnd = new TimeSpan(0,HoursFromEST+pLondonEnd.Hours+24,0,0);	
				else
					pLondonEnd = pLondonEnd.Subtract(new TimeSpan(0,Math.Abs(HoursFromEST),0,0));
				
				if (pNYEnd.Hours < Math.Abs(HoursFromEST))
					pNYEnd = new TimeSpan(0,HoursFromEST+pNYEnd.Hours+24,0,0);	
				else
					pNYEnd = pNYEnd.Subtract(new TimeSpan(0,Math.Abs(HoursFromEST),0,0));				
				
				if (pSwingEnd.Hours < Math.Abs(HoursFromEST))
					pSwingEnd = new TimeSpan(0,HoursFromEST+pSwingEnd.Hours+24,0,0);	
				else
					pSwingEnd = pSwingEnd.Subtract(new TimeSpan(0,Math.Abs(HoursFromEST),0,0));	
						
				
			}
			
			if (HoursFromEST > 0)
			{
				pCustomTime = pCustomTime.Add(new TimeSpan(0,Math.Abs(HoursFromEST),0,0));
				
				pStartTime2 = pStartTime2.Add(new TimeSpan(0,Math.Abs(HoursFromEST),0,0));
				pEndTime2 = pEndTime2.Add(new TimeSpan(0,Math.Abs(HoursFromEST),0,0));
				
				pStartTime = pStartTime.Add(new TimeSpan(0,Math.Abs(HoursFromEST),0,0));
				pEndTime = pEndTime.Add(new TimeSpan(0,Math.Abs(HoursFromEST),0,0));
				pR5Time = pR5Time.Add(new TimeSpan(0,Math.Abs(HoursFromEST),0,0));
				
				pAsianStart = pAsianStart.Add(new TimeSpan(0,Math.Abs(HoursFromEST),0,0));
				pLondonStart = pLondonStart.Add(new TimeSpan(0,Math.Abs(HoursFromEST),0,0));
				pNYStart = pNYStart.Add(new TimeSpan(0,Math.Abs(HoursFromEST),0,0));
				pSwingStart = pSwingStart.Add(new TimeSpan(0,Math.Abs(HoursFromEST),0,0));	
				
				pAsianEnd = pAsianEnd.Add(new TimeSpan(0,Math.Abs(HoursFromEST),0,0));
				pLondonEnd = pLondonEnd.Add(new TimeSpan(0,Math.Abs(HoursFromEST),0,0));
				pNYEnd = pNYEnd.Add(new TimeSpan(0,Math.Abs(HoursFromEST),0,0));
				pSwingEnd = pSwingEnd.Add(new TimeSpan(0,Math.Abs(HoursFromEST),0,0));	
				
			}
			

//				pR1Time = pAsianStart.Add(new TimeSpan(0,0,90,0));
//				pR2Time = pLondonStart.Add(new TimeSpan(0,0,90,0));
//				pR3Time = pNYStart.Add(new TimeSpan(0,0,75,0));
//				pR4Time = pSwingStart.Add(new TimeSpan(0,0,30,0));
			
			
				// OTG
			
//				pR1Time = pStartTime2.Add(new TimeSpan(0,0,65,0));
//				pR2Time = pR1Time.Add(new TimeSpan(0,0,35,0));
//				pR3Time = pR2Time.Add(new TimeSpan(0,0,70,0));
//				pR4Time = pR3Time.Add(new TimeSpan(0,0,45,0));
						
				// adjusted for Joel
			

			
			
			
			
				D1 = pStartTime2-pR5Time;
				D2 = pR1Time-pR5Time;
				D3 = pR2Time-pR5Time;
				D4 = pR3Time-pR5Time;
				D5 = pR4Time-pR5Time;
				D6 = pEndTime2-pR5Time;
			
//				B1 = (int) Math.Floor(D1.TotalMinutes / Bars.BarsPeriod.Value);
//				B2 = (int) Math.Floor(D2.TotalMinutes / Bars.BarsPeriod.Value);
//				B3 = (int) Math.Floor(D3.TotalMinutes / Bars.BarsPeriod.Value);
//				B4 = (int) Math.Floor(D4.TotalMinutes / Bars.BarsPeriod.Value);
//				B5 = (int) Math.Floor(D5.TotalMinutes / Bars.BarsPeriod.Value);
//				B6 = (int) Math.Floor(D6.TotalMinutes / Bars.BarsPeriod.Value);
			
			
								
			
				DR = new List<double>();  
			
				zones = new List<Zone>();  //LIST			
			

                VLS = new Series<string>(this, MaximumBarsLookBack.Infinite);
               
				BackColorI = new Series<double>(this, MaximumBarsLookBack.Infinite);
                CurrentADR10 = new Series<double>(this, MaximumBarsLookBack.Infinite);
                CurrentADR20 = new Series<double>(this, MaximumBarsLookBack.Infinite);
				CurrentADRToday = new Series<double>(this, MaximumBarsLookBack.Infinite);
				
				
				
	
				
		
				if (ChartControl != null)
				{
					
	                //ChartType = Bars.BarsPeriod.BarsPeriodTypeName;
					
					
					//AddButtonZ("TRADES", "ButtonOff", 40, ChartBars.Properties.PlotExecutions != ChartExecutionStyle.DoNotPlot);
					//AddButtonZ("Counter", "Counter", 40, pTimerEnabled);
					

				
		  		
					RegisterPanel();

					//AddButtonZ("Lines", "Lines", 40, pVisualEnabled);
					AddButtonZ("Zones", "Zones", 40, pDisplayZonesEnabled);	
					AddButtonZ("Levels", "Levels", 40, pDisplayLevelsEnabled);	
					
					if (pCDIsEnabled)
					{
					
						AddButtonZ("Current Day", "Current Day", 40, pCDEnabled);	
					}
					else
					{
						pCDEnabled = false;
						
					}
					
					
					
					AddButtonZ("Labels", "Labels", 40, pLabelsEnabled);		
					AddButtonZ("", "", 40, false);	
					AddButtonZ("OHLC", "OHLC", 40, pDisplayOHLCEnabled);
					AddButtonZ("Globex", "Globex", 50, pDisplayGlobexEnabled);	
					AddButtonZ("Fibonacci", "Fibonacci", 40, pDisplayFibEnabled);	
					AddButtonZ("Whole Numbers", "Whole Numbers", 40, pDisplayWholeNumbersEnabled);
					
					
					
					AddButtonZ("Pivots", "Pivots", 40, pDisplayPivotsEnabled);	
					AddButtonZ("", "", 40, false);	
					AddButtonZ("History", "History", 40, pDisplayHistoryEnabled);
					AddButtonZ("HUD", "HUD", 40, pNearestZoneHUDEnabled);	
					
					
					
					
					
		
		
		
						
						
//				AddButtonZ("MIT", "pUseMIT", 40, pUseMIT);

				}

				
				
				
			}			
			
						
			
			if (State == State.Historical)
			{
				
				
				
				
				
				if (pShowMenu)
				if (ChartControl != null && !isToolBarButtonAdded)
				{
					ChartControl.Dispatcher.InvokeAsync((Action)(() =>
					{
						InsertWPFControls();
					}));
				}
			}
			else if (State == State.Terminated)
			{
				UnregisterPanel();

				if (ChartControl != null)
				{
					ChartControl.Dispatcher.InvokeAsync((Action)(() =>
					{
						RemoveWPFControls();
					}));
				}



				if (ChartControl != null)
                if (ChartPanel != null)
                {
                    ChartPanel.MouseMove -= new MouseEventHandler(OnMouseMove);
                    ChartPanel.MouseDown -= new MouseButtonEventHandler(OnMouseDown);
					ChartPanel.MouseLeave -= new MouseEventHandler(OnMouseLeave);
					ChartPanel.MouseWheel -= new MouseWheelEventHandler(OnMouseWheel);
                }
				
				
			}
			
			
			
			
		}


	
        internal void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.MP = e.GetPosition(this.ChartPanel);

 
			FinalXPixel = MP.X / 100 * dpiX;
			FinalYPixel = MP.Y / 100 * dpiY;
         
        
          // Print("mouseddddd");
			
			
		if (AllErrorMessages.Count > 0)
			{
				AllErrorMessages.Clear();
				ChartControl.InvalidateVisual();

				//myProperties.AllowSelectionDragging = PreviousDrag;


				e.Handled = true;
				return;

			}

			
			
//		if (AllErrorMessages.Count > 0)
//			{
//				AllErrorMessages.Clear();
//				ChartControl.InvalidateVisual();
//				return;
				
//			}

			// top chart buttons
			
			
			
			
			
//			bool OneDone = false;
//            foreach (KeyValuePair<double, ButtonZ> thisbutton in AllButtonZ2)
//            {
//                bool hoverednew = MouseIn(thisbutton.Value.Rect, 2, 2);
//                string buttonn = thisbutton.Value.Text;


				

//               if (hoverednew && buttonn == SFeed1)
//                {
					
//					//Print(buttonn);
					
//					pFeed1Included = !pFeed1Included;
//					OneDone = true;
					
					
					
//				}
				
//               if (hoverednew && buttonn == SFeed2)
//                {
					
//					//Print(buttonn);
//					pFeed2Included = !pFeed2Included;
//					OneDone = true;
					
					
//				}				
				
//               if (hoverednew && buttonn == SFeed3)
//                {
					
//					//Print(buttonn);
//					pFeed3Included = !pFeed3Included;
//					OneDone = true;
					
										
					
					
//				}	
				
//               if (hoverednew && buttonn == SFeed4)
//                {
					
//					//Print(buttonn);
					
//					pFeed4Included = !pFeed4Included;
//					OneDone = true;
					
						
					
//				}
				
//			}
			
			
//			if (OneDone)
//			{
//					SetAllTrend();
//					UpdateButtons();
//					ChartControl.InvalidateVisual();
//					return;						
//			}
			
			
			
			
			
			// Header click — start drag or toggle
			if (MouseIn(headerRect, 3, 3))
			{
				draggingPanelId = PANEL_ID;
				dragStartY = (float)FinalYPixel;
				dragOffsetY = 0;
				if (ChartPanel != null) ChartPanel.CaptureMouse();
				e.Handled = true;
				return;
			}

			// Button clicks only when expanded
			if (IsPanelExpanded())
            foreach (KeyValuePair<double, ButtonZ> thisbutton in AllButtonZ)
            {
				// Skip buttons outside visible area
				if (thisbutton.Value.Rect.Bottom < PANEL_TOP_MARGIN || thisbutton.Value.Rect.Top > ChartPanel.H)
					continue;
                bool hoverednew = MouseIn(thisbutton.Value.Rect, 2, 2);
                string buttonn = thisbutton.Value.Text;



//               if (hoverednew && buttonn == "TRADES")
//                {
//                    //pSLOffset = Math.Max(0, pSLOffset - 1);
                    
//					//TogglePlotExecutions();
					
//					thisbutton.Value.Switch = ChartBars.Properties.PlotExecutions != ChartExecutionStyle.DoNotPlot;
					
//				//	SetBack(0);
					
//					this.ChartControl.InvalidateVisual();
//					return;

//                }               
  
				
					
//                    thisbutton.Value.Switch = pTimerEnabled;
//                    this.ChartControl.InvalidateVisual();
					
//					return;

//                }

					
//					return;

//                }  

                if (hoverednew && buttonn == "Levels")
                {
                    if (pDisplayLevelsEnabled)
                    {
                        pDisplayLevelsEnabled = false;
                    }
                    else
                    {
                        pDisplayLevelsEnabled = true;
                    }
                    thisbutton.Value.Switch = pDisplayLevelsEnabled;
                    this.ChartControl.InvalidateVisual();
					e.Handled = true;
					return;

                }


                if (hoverednew && buttonn == "Labels")
                {
                    if (pLabelsEnabled)
                    {
                        pLabelsEnabled = false;
                    }
                    else
                    {
                        pLabelsEnabled = true;
                    }
                    thisbutton.Value.Switch = pLabelsEnabled;
                    this.ChartControl.InvalidateVisual();
					e.Handled = true;
					return;

                }
                if (hoverednew && buttonn == "Current Day")
                {
                    if (pCDEnabled)
                    {
                        pCDEnabled = false;
                    }
                    else
                    {
                        pCDEnabled = true;
                    }
                    thisbutton.Value.Switch = pCDEnabled;
                    this.ChartControl.InvalidateVisual();
					e.Handled = true;
					return;

                }
				
				
				
                if (hoverednew && buttonn == "History")
                {
                    if (pDisplayHistoryEnabled)
                    {
                        pDisplayHistoryEnabled = false;
                    }
                    else
                    {
                        pDisplayHistoryEnabled = true;
                    }
                    thisbutton.Value.Switch = pDisplayHistoryEnabled;
                    this.ChartControl.InvalidateVisual();
					e.Handled = true;
					return;

                }

                if (hoverednew && buttonn == "HUD")
                {
                    pNearestZoneHUDEnabled = !pNearestZoneHUDEnabled;
                    thisbutton.Value.Switch = pNearestZoneHUDEnabled;
                    this.ChartControl.InvalidateVisual();
					e.Handled = true;
					return;
                }

                if (hoverednew && buttonn == "Pivots")
                {
                    if (pDisplayPivotsEnabled)
                    {
                        pDisplayPivotsEnabled = false;
                    }
                    else
                    {
                        pDisplayPivotsEnabled = true;
                    }
                    thisbutton.Value.Switch = pDisplayPivotsEnabled;
                    this.ChartControl.InvalidateVisual();
					e.Handled = true;
					return;

                }
							
				
                if (hoverednew && buttonn == "Whole Numbers")
                {
                    if (pDisplayWholeNumbersEnabled)
                    {
                        pDisplayWholeNumbersEnabled = false;
                    }
                    else
                    {
                        pDisplayWholeNumbersEnabled = true;
                    }
                    thisbutton.Value.Switch = pDisplayWholeNumbersEnabled;
                    this.ChartControl.InvalidateVisual();
					e.Handled = true;
					return;

                }
						
                if (hoverednew && buttonn == "OHLC")
                {
                    pDisplayOHLCEnabled = !pDisplayOHLCEnabled;
                    thisbutton.Value.Switch = pDisplayOHLCEnabled;
                    this.ChartControl.InvalidateVisual();
					e.Handled = true;
					return;
                }

                if (hoverednew && buttonn == "Globex")
                {
                    pDisplayGlobexEnabled = !pDisplayGlobexEnabled;
                    thisbutton.Value.Switch = pDisplayGlobexEnabled;
                    this.ChartControl.InvalidateVisual();
					e.Handled = true;
					return;
                }

                if (hoverednew && buttonn == "Fibonacci")
                {
                    if (pDisplayFibEnabled)
                    {
                        pDisplayFibEnabled = false;
                    }
                    else
                    {
                        pDisplayFibEnabled = true;
                    }
                    thisbutton.Value.Switch = pDisplayFibEnabled;
                    this.ChartControl.InvalidateVisual();
					e.Handled = true;
					return;

                }


                if (hoverednew && buttonn == "Zones")
                {
                    if (pDisplayZonesEnabled)
                    {
                        pDisplayZonesEnabled = false;
                    }
                    else
                    {
                        pDisplayZonesEnabled = true;
                    }
                    thisbutton.Value.Switch = pDisplayZonesEnabled;
                    this.ChartControl.InvalidateVisual();
					e.Handled = true;
					return;

                }
				
				
				
								
			
				
//                else if (hoverednew && buttonn == "Lines")
//                {
//                    if (pVisualEnabled)
//                    {
//                        pVisualEnabled = false;
						
//						//PaintPriceMarkers = false;
					
				
//                    }
//                    else
//                    {
//                        pVisualEnabled = true;
//						//PaintPriceMarkers = StartPriceMarkers;
//                    }
//                    thisbutton.Value.Switch = pVisualEnabled;
//                    this.ChartControl.InvalidateVisual();
					
//					return;

//                }  
				
				
				
				
              
					
//					return;

//                }

					
//					return;

//                }  

            }
			
			
			
		


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

			float scrollStep = 30f;
			float delta = -e.Delta / 120f * scrollStep;
			state[SC_OFFSET] += delta;

			float maxScroll = Math.Max(0, state[SC_TOTALH] - state[SC_VIEWH] + 20f);
			state[SC_OFFSET] = Math.Max(0, Math.Min(state[SC_OFFSET], maxScroll));

			this.ChartControl.InvalidateVisual();
			e.Handled = true;
		}


		//double price2222 = 15300;
		 
		
		private List<ObjectToDraw> TextObjectsToDrawHovered = new List<ObjectToDraw>();
		private int OCountNow, OCountP = 0;
		private SharpDX.RectangleF RectNow = new SharpDX.RectangleF(0, 0, 0, 0);
		private SharpDX.RectangleF RectPrevious = new SharpDX.RectangleF(0, 0, 0, 0); 
		
		private int LastPlotN = 23;
		
		internal void OnMouseMove(object sender, MouseEventArgs e)
    	{
            this.MP = e.GetPosition(this.ChartPanel);


			FinalXPixel = MP.X / 100 * dpiX;
			FinalYPixel = MP.Y / 100 * dpiY;
         
			
			// hover rects
			
//			MouseInRectPre = MouseInRectNow;
//			MouseInRectNow = false;
			
			
			RectPrevious = RectNow;
			RectNow = new SharpDX.RectangleF(0, 0, 0, 0);

			// No need to copy list - OnMouseMove and OnRender both run on UI thread
			foreach(SharpDX.RectangleF rr in AllRefreshRects)
			{
				if (MouseIn(rr,3,3))
				{
					RectNow = rr;
				}
			}

			// Trigger re-render when mouse enters or leaves a zone
			if (RectNow.Width != RectPrevious.Width || RectNow.X != RectPrevious.X
				|| RectNow.Y != RectPrevious.Y || RectNow.Height != RectPrevious.Height)
			{
				this.ChartControl.InvalidateVisual();
			}
			
			
			
			if (PaintPriceMarkers)
			{
			// Price markers now set in OnBarUpdate - no hover-based clearing needed
			
			
			//if (HoveredZoneLevels.Count == 0 && Values[10].IsValidDataPoint(CurrentBarsBack))
				
				
			
			
			
			//if (RectNow != RectPrevious)
			{
				//Print("refresh");
				
				
			
					

				
				
				
			//foreach(SharpDX.RectangleF rr in new List<SharpDX.RectangleF>(AllRefreshRects))
				//
				{
								
					//rect222 = ooo.Rectangle;
					
					
					
					//if (LevelsHoveredByBar.ContainsKey(LastHoverLevelStartBar))
					{
						//Print("MouseH " + LevelsHoveredByBar[LastHoverLevelStartBar].Count);
						
						
					//foreach (KeyValuePair<string, double> pair in new SortedDictionary<string, double>(LevelsHoveredByBar[LastHoverLevelStartBar]))
						
					//	if (LevelsHoveredByBar.ContainsKey(LastHoverLevelStartBar))
					//	Print("LevelsHoveredByBar[LastHoverLevelStartBar].Count" + LevelsHoveredByBar[LastHoverLevelStartBar].Count);
						
					//if (LevelsHoveredByBar[LastHoverLevelStartBar].ContainsKey(ooo.Text))
						
					//	if (HoveredZoneLevels.Contains(ooo.Text)) // show price marker if level is hovered
					//	if (MouseIn(ExpandRect(ooo.Rectangle,-5,-3),0,0)) // show price marker if zone is hovered
						
					//	Print("TextObjectsToDrawHovered.Count:  " + TextObjectsToDrawHovered.Count);
					//	Print("TextObjectsToDraw.Count:  " + TextObjectsToDraw.Count);
						
						
					
					}
						
						
						
					
				}
				
				//this.ChartControl.InvalidateVisual();
				
				
				//Print("Add: " + StartFlagPlotN + " " + LastPlotN);		
			
			
			}
			
			
//			if (MouseInRectNow != MouseInRectPre)
//				this.ChartControl.InvalidateVisual();
			
		
//			TextObjectsToDrawHovered.Clear();
			
//			foreach(ObjectToDraw ooo in new List<ObjectToDraw>(TextObjectsToDraw))
//			{
//				if (MouseIn(ooo.Rectangle,3,3))
//				{
					
//					TextObjectsToDrawHovered.Add(ooo);
					
//					//MouseInRectNow = true;
					
//					//Print(ooo.Text);
					
//					//price2222 = price2222 - 1;
					
//					//Values[10][0] = 15420;
					
//					this.ChartControl.InvalidateVisual();
//				}
				
//			}
			
			// Price markers now managed in OnBarUpdate, no hover-based TriggerCustomEvent needed
			
			
			}
			
			
            // Batch button hover changes into a single InvalidateVisual call
            bool needsInvalidate = false;
			currentButtonHover = -1;
            foreach (KeyValuePair<double, ButtonZ> thisbutton in AllButtonZ)
                {
					// Skip buttons outside visible area
					if (thisbutton.Value.Rect.Bottom < PANEL_TOP_MARGIN || thisbutton.Value.Rect.Top > ChartPanel.H)
					{
						if (thisbutton.Value.Hovered) { thisbutton.Value.Hovered = false; needsInvalidate = true; }
						continue;
					}
                    bool hoverednew = MouseIn(thisbutton.Value.Rect, 2, 2);
                    bool hoverednow = thisbutton.Value.Hovered;

                    if (hoverednew != hoverednow)
                    {
                        thisbutton.Value.Hovered = hoverednew;
                        needsInvalidate = true;
                    }

                }

			// Header hover detection — always invalidate when near the panel so hover state updates promptly
			if (MouseIn(B2, 8, 8) || MouseIn(headerRect, 3, 3))
				needsInvalidate = true;

                InMenuP = InMenu;
                InMenu = MouseIn(B2, 8, 8);
				SetPanelMenuOpen(InMenu);

            if (InMenu != InMenuP)
                needsInvalidate = true;

			// Also check if any other panel opened the menu
			if (IsAnyPanelMenuOpen() && !InMenu)
				needsInvalidate = true;

			// Drag-to-reorder tracking
			if (draggingPanelId != null && e.LeftButton == MouseButtonState.Released)
			{
				// Mouse released — toggle expand if no significant drag occurred
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

				// Check if dragged past another panel's header — swap priorities
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

            if (needsInvalidate)
                this.ChartControl.InvalidateVisual();

			//e.Handled = true;
		}
		
		
		private int LastCurrentBar = 0;
		private int ATRMTicks = 0;
		private bool FirstRun = true;
		private bool FirstRun2 = true;
		private string thischart = string.Empty;
		
		
		private bool NewLevels = false;
		private bool NewLevelsEnd = false;

		protected override void OnConnectionStatusUpdate(ConnectionStatusEventArgs connectionStatusUpdate)
		{
			// When a data feed connects (or reconnects), clear any prior "no feed"
			// or "insufficient data" overlay and re-arm FirstRun2 so the indicator
			// re-evaluates with fresh data instead of leaving the user staring at
			// a stale warning.
			if (connectionStatusUpdate != null
				&& connectionStatusUpdate.PriceStatus == ConnectionStatus.Connected)
			{
				if (AllErrorMessages.Count > 0)
					AllErrorMessages.Clear();
				FirstRun2 = true;
				if (ChartControl != null)
					ChartControl.Dispatcher.InvokeAsync(() => { try { ChartControl.InvalidateVisual(); } catch { } });
			}
		}

        protected override void OnBarUpdate()
        {
		
//			if (BarsInProgress == 0)
//				return;
			
//			BarsArray[0].da
			
//			Print(BarsInProgress + "    " + Time[0]);
			
//			return;
			
			
			
			if (!Permission)
				return;		
			
			
//			if (State == State.Realtime)
//			{
				
				
//				if (DaysLoaded < DaysNeeded) 
//					return;
//			}
			
			
//			if (CurrentBars[0] <= 0)
//				return;
			if (CurrentBars[BarsArray.Length-1] <= 0)
				return;
			
			
			if (FirstRun)
			{
				FirstRun = false;
					
				thischart = BarsArray[0].BarsPeriod.ToString();
				
//				if (thischart == "1 Minute")
//					UseChartData = true;
				
				if(UseChartData) 
				{
					//Print("heye1");
					
					
					FirstBarTime = Time[0];
					
					StartTime =	new DateTime(Times[0][0].Year,Times[0][0].Month,Times[0][0].Day,pStartTime.Hours,pStartTime.Minutes,0);
					EndTime	=	new DateTime(Times[0][0].Year,Times[0][0].Month,Times[0][0].Day,pEndTime.Hours,pEndTime.Minutes,0);
					R1TTime =	new DateTime(Times[0][0].Year,Times[0][0].Month,Times[0][0].Day,pR1Time.Hours,pR1Time.Minutes,0);
					R2TTime =	new DateTime(Times[0][0].Year,Times[0][0].Month,Times[0][0].Day,pR2Time.Hours,pR2Time.Minutes,0);
					R3TTime =	new DateTime(Times[0][0].Year,Times[0][0].Month,Times[0][0].Day,pR3Time.Hours,pR3Time.Minutes,0);
					R4TTime =	new DateTime(Times[0][0].Year,Times[0][0].Month,Times[0][0].Day,pR4Time.Hours,pR4Time.Minutes,0);
					R5TTime =	new DateTime(Times[0][0].Year,Times[0][0].Month,Times[0][0].Day,pR5Time.Hours,pR5Time.Minutes,0);
					R6TTime =	new DateTime(Times[0][0].Year,Times[0][0].Month,Times[0][0].Day,pR6Time.Hours,pR6Time.Minutes,0);
					R7TTime =	new DateTime(Times[0][0].Year,Times[0][0].Month,Times[0][0].Day,pR7Time.Hours,pR7Time.Minutes,0);
					R8TTime =	new DateTime(Times[0][0].Year,Times[0][0].Month,Times[0][0].Day,pR8Time.Hours,pR8Time.Minutes,0);
					
					//Print("heye2");
					
					AsianSTime = new DateTime(Times[0][0].Year,Times[0][0].Month,Times[0][0].Day,pAsianStart.Hours,pAsianStart.Minutes,0);
					LondonSTime = new DateTime(Times[0][0].Year,Times[0][0].Month,Times[0][0].Day,pLondonStart.Hours,pLondonStart.Minutes,0);
					NYSTime = new DateTime(Times[0][0].Year,Times[0][0].Month,Times[0][0].Day,pNYStart.Hours,pNYStart.Minutes,0);
					SwingSTime = new DateTime(Times[0][0].Year,Times[0][0].Month,Times[0][0].Day,pSwingStart.Hours,pSwingStart.Minutes,0);
					
					
					
					AsianETime = new DateTime(Times[0][0].Year,Times[0][0].Month,Times[0][0].Day,pAsianEnd.Hours,pAsianEnd.Minutes,0);
					//Print("heye4");
					LondonETime = new DateTime(Times[0][0].Year,Times[0][0].Month,Times[0][0].Day,pLondonEnd.Hours,pLondonEnd.Minutes,0);
					NYETime = new DateTime(Times[0][0].Year,Times[0][0].Month,Times[0][0].Day,pNYEnd.Hours,pNYEnd.Minutes,0);
					SwingETime = new DateTime(Times[0][0].Year,Times[0][0].Month,Times[0][0].Day,pSwingEnd.Hours,pSwingEnd.Minutes,0);
					
					Midnight = new DateTime(Times[0][0].Year,Times[0][0].Month,Times[0][0].Day,pCustomTime.Hours,pCustomTime.Minutes,0);
						
					
					//FirstTime = Times[1][0];
					CurrentTime = Times[0][0];
					//LastBarTime = Times[0][1];
					CurrentHigh = Highs[0][0];
					CurrentLow = Lows[0][0];
					
					StartTime2 =	new DateTime(Times[0][0].Year,Times[0][0].Month,Times[0][0].Day,pStartTime2.Hours,pStartTime2.Minutes,pStartTime2.Seconds);
					EndTime2	=	new DateTime(Times[0][0].Year,Times[0][0].Month,Times[0][0].Day,pEndTime2.Hours,pEndTime2.Minutes,pEndTime2.Seconds);

					
				} 
				else 
				{
					
					StartTime =	new DateTime(Times[1][0].Year,Times[1][0].Month,Times[1][0].Day,pStartTime.Hours,pStartTime.Minutes,0);
					EndTime	=	new DateTime(Times[1][0].Year,Times[1][0].Month,Times[1][0].Day,pEndTime.Hours,pEndTime.Minutes,0);
					
					StartTime2 =	new DateTime(Times[1][0].Year,Times[1][0].Month,Times[1][0].Day,pStartTime2.Hours,pStartTime2.Minutes,pStartTime2.Seconds);
					EndTime2	=	new DateTime(Times[1][0].Year,Times[1][0].Month,Times[1][0].Day,pEndTime2.Hours,pEndTime2.Minutes,pEndTime2.Seconds);
					
					R1TTime =	new DateTime(Times[1][0].Year,Times[1][0].Month,Times[1][0].Day,pR1Time.Hours,pR1Time.Minutes,0);
					R2TTime =	new DateTime(Times[1][0].Year,Times[1][0].Month,Times[1][0].Day,pR2Time.Hours,pR2Time.Minutes,0);
					R3TTime =	new DateTime(Times[1][0].Year,Times[1][0].Month,Times[1][0].Day,pR3Time.Hours,pR3Time.Minutes,0);
					R4TTime =	new DateTime(Times[1][0].Year,Times[1][0].Month,Times[1][0].Day,pR4Time.Hours,pR4Time.Minutes,0);
					R5TTime =	new DateTime(Times[1][0].Year,Times[1][0].Month,Times[1][0].Day,pR5Time.Hours,pR5Time.Minutes,0);					
					R6TTime =	new DateTime(Times[1][0].Year,Times[1][0].Month,Times[1][0].Day,pR6Time.Hours,pR6Time.Minutes,0);
					R7TTime =	new DateTime(Times[1][0].Year,Times[1][0].Month,Times[1][0].Day,pR7Time.Hours,pR7Time.Minutes,0);					
					R8TTime =	new DateTime(Times[1][0].Year,Times[1][0].Month,Times[1][0].Day,pR8Time.Hours,pR8Time.Minutes,0);
					
					AsianSTime = new DateTime(Times[1][0].Year,Times[1][0].Month,Times[1][0].Day,pAsianStart.Hours,pAsianStart.Minutes,0);
					LondonSTime = new DateTime(Times[1][0].Year,Times[1][0].Month,Times[1][0].Day,pLondonStart.Hours,pLondonStart.Minutes,0);
					NYSTime = new DateTime(Times[1][0].Year,Times[1][0].Month,Times[1][0].Day,pNYStart.Hours,pNYStart.Minutes,0);
					SwingSTime = new DateTime(Times[1][0].Year,Times[1][0].Month,Times[1][0].Day,pSwingStart.Hours,pSwingStart.Minutes,0);
					
					AsianETime = new DateTime(Times[1][0].Year,Times[1][0].Month,Times[1][0].Day,pAsianEnd.Hours,pAsianEnd.Minutes,0);
					LondonETime = new DateTime(Times[1][0].Year,Times[1][0].Month,Times[1][0].Day,pLondonEnd.Hours,pLondonEnd.Minutes,0);
					NYETime = new DateTime(Times[1][0].Year,Times[1][0].Month,Times[1][0].Day,pNYEnd.Hours,pNYEnd.Minutes,0);
					SwingETime = new DateTime(Times[1][0].Year,Times[1][0].Month,Times[1][0].Day,pSwingEnd.Hours,pSwingEnd.Minutes,0);
					
					Midnight = new DateTime(Times[1][0].Year,Times[1][0].Month,Times[1][0].Day,pCustomTime.Hours,pCustomTime.Minutes,0);
								
					
					FirstTime = Times[1][0];
					CurrentTime = Times[1][0];
					//LastBarTime = Times[1][1];
					CurrentHigh = Highs[1][0];
					CurrentLow = Lows[1][0];
					
					
					
					
					//if (BarsArray[1].IsFirstBarOfSession)
						
							
				}
				
				
//Print(CurrentTime);
				
				if(EndTime2<=StartTime2) 
				{
					
					
					EndTime2 = EndTime2.AddDays(1);
					
					
				}
								
				
				if(EndTime <= StartTime) 
				{
					EndTime = EndTime.AddDays(1);
					FinalTime = FinalTime.AddDays(1);
				}
				
				if(AsianETime <= AsianSTime) 
				{
					AsianETime = AsianETime.AddDays(1);
					
				}				
				if(NYETime <= NYSTime) 
				{
					NYETime = NYETime.AddDays(1);
					
				}	
				
				if(SwingETime <= SwingSTime) 
				{
					SwingETime = SwingETime.AddDays(1);
					
				}				
				if(LondonETime <= LondonSTime) 
				{
					LondonETime = LondonETime.AddDays(1);
					
				}
				
				
			}
			
		
		
			NewZones = false;
			
//			if (CurrentBars[0] < 1 || CurrentBars[1] < 1)

			
//			return;
			
//			if (CurrentBars[0] < 1)
//				return;			
			
			if (CurrentBars[0] > 0)
			LastCurrentBar = CurrentBars[0];
			

			
//				CurrentADR10[0] = (0);
//				CurrentADR20[0] = (0);
				
			
			
			if(UseChartData) 
			{
				DS = 0;
				CurrentTime = Times[0][0];
				LastBarTime = Times[0][1];
				CurrentHigh = Highs[0][0];
				CurrentLow = Lows[0][0];
			} 
			else 
			{
				DS = 1;
				CurrentTime = Times[1][0];
				LastBarTime = Times[1][1];
				CurrentHigh = Highs[1][0];
				CurrentLow = Lows[1][0];
			}	
			

			//set all plots to 0
			
//			Values[20][0] = 15330;
			
//			Values[10][0] = 15410;
			
//			for (int i = 0; i < Values.Length; i++)
//			{
//				Values[i][0] = (0);
			
//			}
			
			
			
			
			//Print("GG 1");
			
			
			
			bool pDevelopingLevelsEnabled = true;
			
			
			if (CurrentBars[0] > 0)
			if (pDevelopingLevelsEnabled)
			{
			
				NewLevels = false;
				NewLevelsEnd = false;
				
				AllDevelopingLevels.Clear();
				
				
	// AFTER THE DEFINED SESSION TIME IS COMPLETE
				

					DR1[0] = 0;
					DR2[0] = 0;
					DR3[0] = 0;	
					DR4[0] = 0;
					DR5[0] = 0;
					DS1[0] = 0;
					DS2[0] = 0;
					DS3[0] = 0;	
					DS4[0] = 0;
					DS5[0] = 0;
					DS6[0] = 0;
					DS7[0] = 0;
					MS1[0] = 0;
					MS2[0] = 0;
					MS3[0] = 0;	
					MS4[0] = 0;
					MS5[0] = 0;
					MS6[0] = 0;
					MS7[0] = 0;
					MS8[0] = 0;
					MS9[0] = 0;			
					MS10[0] = 0;	
					
					

	//			VLS.Reset(0);
				//VLS.Reset(1);
				
				
				
				while(CurrentTime.Ticks > R5TTime.Ticks) 
				{
						int dow = (int) CurrentTime.DayOfWeek;	
									
						if (dow != 0)
						{

							if (FirstMeasurement == 0)
								FirstMeasurement = CurrentBars[0];
							
							//VLS[0] = (1,R5TTime.Hour.ToString()+" a.m. Measurement");
						}	
						
						R5TTime = R5TTime.AddDays(1);
						
						
						
				}
				
				
			
				
				// Revision time checks - recalculate current day levels at each revision time
				while(CurrentTime.Ticks > R1TTime.Ticks)
				{
					if ((int)CurrentTime.DayOfWeek != 0) RecalcCurrentDayLevels();
					R1TTime = R1TTime.AddDays(1);
				}

				while(CurrentTime.Ticks > R2TTime.Ticks)
				{
					if ((int)CurrentTime.DayOfWeek != 0) RecalcCurrentDayLevels();
					R2TTime = R2TTime.AddDays(1);
				}

				while(CurrentTime.Ticks > R3TTime.Ticks)
				{
					if ((int)CurrentTime.DayOfWeek != 0) RecalcCurrentDayLevels();
					R3TTime = R3TTime.AddDays(1);
				}

				while(CurrentTime.Ticks > R4TTime.Ticks)
				{
					if ((int)CurrentTime.DayOfWeek != 0) RecalcCurrentDayLevels();
					R4TTime = R4TTime.AddDays(1);
				}		
				
				
				
				
				
			
			//	Print(Midnight);
				
				while(CurrentTime.Ticks > Midnight.Ticks) 
				{
					AllowedToPlot = true;
					Midnight = Midnight.AddDays(1);	
					
					
					//Print("Midnight " + Midnight);
				}
				
				

				// end of range
				
				if (IsFirstTickOfBar && CurrentBar > 0)
				{
//					CurrentADR10[0] = CurrentADR10[1];
//					CurrentADR20[0] = CurrentADR20[1];
//					CurrentADRToday[0] = CurrentADRToday[1];
				}
				
				while(CurrentTime.Ticks > EndTime2.Ticks) 
				{
						int dow = (int) CurrentTime.DayOfWeek;	
									

							//if (HH != 0 && Time[1].Hour != HolidayIgnore && Time[1].Hour != HolidayIgnore2)
							{
								
								NewLevelsEnd = true;
								
								
								DLHH = 0;
								DLLL = 99999999;
								
								TT = 0;
								F1 = 0;
								F2 = 0;
								F3 = 0;
								F4 = 0;
								F5 = 0;
								
									CM = 0;
							
									CHF1 = 0;
									CHF2 = 0;
									CHF3 = 0;
									CHF4 = 0;
									
									
									CLF1 = 0;
									CLF2 = 0;
									CLF3 = 0;
									CLF4 = 0;
									
								//DR.Add(PMSHH-PMSLL);
								
								//Print(Time[0]);
								//Print(Time[1].Hour);
								//Print((PMSHH-PMSLL).ToString(PriceString));
								
								
//								CurrentADR10[0] = GetADR(pADRPeriod1);
//								CurrentADR20[0] = GetADR(pADRPeriod2);
								
							
								
//								TT2 = PMSHH-PMSLL;
			
//								F13 = PMSLL+TT2*(50)/100;
//								F14 = PMSLL+TT2*(127)/100;
//								F15 = PMSLL+TT2*(161.8)/100;
//								F16 = PMSLL+TT2*(200)/100;
//								F17 = PMSHH-TT2*(127)/100;
//								F18 = PMSHH-TT2*(161.8)/100;
//								F19 = PMSHH-TT2*(200)/100;						

								
								MHH = 0;
								MLL = 99999999;

								// Save completed Globex session and reset
								if (GHH != 0 && GLL != 9999999)
								{
									PGHH = GHH;
									PGLL = GLL;
								}
								GHH = 0;
								GLL = 9999999;
								
					
	//							Print(Time[0]);
	//							Print(PCLOSE);
	//							//Print(Time[1].Hour);
	//							Print((PMSHH-PMSLL).ToString(PriceString));		
	//							
	//							
							}
							
						
						

							
						
						R8TTime = R8TTime.AddDays(1);
						
					
		
					StartTime2 = StartTime2.AddDays(1);
					EndTime2 =	EndTime2.AddDays(1);
					FinalTime = FinalTime.AddDays(1);					
						
				}		
				

				
		
						
				
				
//				MSHH = Math.Max(MSHH, High[0]);
//				MSLL = Math.Min(MSLL, Low[0]);		
				
				
	// WE ARE CURRENTLY MAKING THE RANGE
		
				
				if (CurrentTime.Ticks > StartTime2.Ticks && CurrentTime.Ticks <= EndTime2.Ticks)
				{
					//PlotNow = true;
						
					//Print(Time[0]);

					MHH = Math.Max(MHH,CurrentHigh);
					MLL = Math.Min(MLL,CurrentLow);

					// Price-tested freshness tracking: mark any level price came within range of
					if (pFreshnessDimmingEnabled && IsFirstTickOfBar && AllDailyLevels.Count > 0)
					{
						double testRange = pFreshnessTestTicks * TickSize;
						double barHigh = CurrentHigh + testRange;
						double barLow = CurrentLow - testRange;
						int currentBarIdx = CurrentBars[0];
						foreach (KeyValuePair<string, double> lvl in AllDailyLevels)
						{
							if (lvl.Value >= barLow && lvl.Value <= barHigh)
								LevelLastTestedBar[lvl.Key] = currentBarIdx;
						}
					}
		
//					CurrentADRToday[0] = HH-LL;
					
//					if (AllowedToPlot)
//					{
//						if (CurrentADR20[0] != 0)
//						{
//							MS9[0] = HH-CurrentADR20[0];
//							MS10[0] = LL+CurrentADR20[0];
//						}
//					}
					
					
					//MSHH = Math.Max(MSHH, High[0]);
					//MSLL = Math.Min(MSLL, Low[0]);
					
			
					
							
								
				}
				else
				{
					
				//	if (


//					DSHH = Math.Max(DSHH, High[0]);
//					DSLL = Math.Min(DSLL, Low[0]);


				}

				// Globex (overnight) session tracking: from session start to RTH open
				if (CurrentTime.Ticks > StartTime.Ticks && CurrentTime.Ticks <= StartTime2.Ticks)
				{
					GHH = Math.Max(GHH, CurrentHigh);
					GLL = Math.Min(GLL, CurrentLow);
				}



				if (NewLevels)
				{
					
					
					
					
				}
				
				
				//if (AllowedToPlot)
				if (DLHH != 0)
				{
					//Print(PMSHH);
					
					
					DR1[0] = DLHH;
					DR2[0] = DLLL;
					DR3[0] = CM;
					DR4[0] = CHF1;
					DR5[0] = CHF2;
					DS1[0] = CHF3;
					DS2[0] = CHF4;
					DS3[0] = CLF1;
					DS4[0] = CLF2;
					DS5[0] = CLF3;
					DS6[0] = CLF4;							

					
//					MS2[0] = PP;
//					MS3[0] = R1;
//					MS4[0] = R2;
//					MS5[0] = R3;
//					MS6[0] = S1;
//					MS7[0] = S2;
//					MS8[0] = S3;
					
						
				}
				
				
				
				
				
				
				
				
			
			
			}
			
		
			// end of developing levels
			
			
			
			
			
//			if (!UseChartData)
//			if (BarsInProgress == 0)
//				return;
			
//			if (UseChartData)
//			if (BarsInProgress == 1)
//				return;				
						
			//Print("GG 2");
			
			
			////Print("GG 1");
			
			
			
			// end of main session range
			
			if (BarsInProgress == 0)
				return;
				
			if (BarsInProgress == 1)
				
			
			while(CurrentTime.Ticks > EndTime.Ticks) 
			{
				
				//Print(CurrentTime);
				
				//BackBrush = Brushes.Red;
				
				
				//
				
					int dow = (int) CurrentTime.DayOfWeek;	
								

						if (HH != 0 && Time[1].Hour != HolidayIgnore && Time[1].Hour != HolidayIgnore2)
						{
//							PAHH = 0;
//							PLHH = 0;
							AllowedToPlot = false;
							
							PMSHH = HH;
						
							
							PMSLL = LL;
							
							
							
							
							
							DR.Add(PMSHH-PMSLL);
							
							
							
							
							
							
							//BackColor = Color.Purple;
							
							//Print(Time[0]);
							
							//Print(Time[1].Hour);
							//Print((PMSHH-PMSLL).ToString(PriceString));
							//sad
							
//							CurrentADR10[0] = (GetADR(pADRPeriod1));
//							CurrentADR20[0] = (GetADR(pADRPeriod2));
							
						
							
							RANGE = PMSHH-PMSLL;
							
							double ATRM = RANGE*pZATRMultiplier/100;
				
							
							ATRMTicks = (int) Math.Round(ATRM/TickSize,0);
							
							ATRMTicks = Math.Max(pMinimumTicks, ATRMTicks);
							
						
							
							
//							PP = (PMSHH + PMSLL + PCLOSE) / 3;
//							S1 = 2 * PP - PMSHH;
//							R1 = 2 * PP - PMSLL;
//							S2 = PP - RANGE;
//							R2 = PP + RANGE;
//							S3 = PP - 2*RANGE;
//							R3 = PP + 2*RANGE;	
//							S3 = PMSLL - 2*(PMSHH - PP);
//							R3 = PMSHH + 2*(PP-PMSLL);						
							
							F13 = PMSLL+RANGE*(50)/100;
							
						//	pPercent1
							
							
//							F21 = PMSLL+RANGE*(78.6)/100;
//							F14 = PMSLL+RANGE*(127)/100;
//							F15 = PMSLL+RANGE*(161.8)/100;
//							F16 = PMSLL+RANGE*(200)/100;
							
//							F20 = PMSHH-RANGE*(78.6)/100;
//							F17 = PMSHH-RANGE*(127)/100;
//							F18 = PMSHH-RANGE*(161.8)/100;
//							F19 = PMSHH-RANGE*(200)/100;						

							
			
//							F21 = PMSLL+RANGE*(pPercent1)/100;
//							F14 = PMSLL+RANGE*(pPercent2)/100;
//							F15 = PMSLL+RANGE*(pPercent3)/100;
//							F16 = PMSLL+RANGE*(pPercent4)/100;
							
//							F20 = PMSHH-RANGE*(pPercent1)/100;
//							F17 = PMSHH-RANGE*(pPercent2)/100;
//							F18 = PMSHH-RANGE*(pPercent3)/100;
//							F19 = PMSHH-RANGE*(pPercent4)/100;						

							
							
							
							
							

				
				
																
							HH = 0;
							LL = 999999;
							
							if(UseChartData) 
							{
								
								PCLOSE = Closes[0][1];
								
							} 
							else 
							{
								PCLOSE = Closes[1][1];
							}			
						
							//PCLOSE = Closes[0][1];
							
							
						
							SortedDictionary<string, double> CalcDaily = new SortedDictionary<string, double>();
							
//				
							
							if (pFibsMidEnabled)
								CalcDaily.Add("Middle", F13);							
							
							
							
					
							
							
							if (pFibsEnabled)
							{

								
								if (pFibsExEnabled)
								{
									
									if (pPercent1 != 0) H1 = PMSLL + (RANGE)*pPercent1/100;
									if (pPercent2 != 0) H2 = PMSLL + (RANGE)*pPercent2/100;
									if (pPercent3 != 0) H3 = PMSLL + (RANGE)*pPercent3/100;
									if (pPercent4 != 0) H4 = PMSLL + (RANGE)*pPercent4/100;
									if (pPercent5 != 0) H5 = PMSLL + (RANGE)*pPercent5/100;
									if (pPercent6 != 0) H6 = PMSLL + (RANGE)*pPercent6/100;
									if (pPercent7 != 0) H7 = PMSLL + (RANGE)*pPercent7/100;
									if (pPercent8 != 0) H8 = PMSLL + (RANGE)*pPercent8/100;
									
									
									if (pPercent1 != 0) L1 = PMSHH - (RANGE)*pPercent1/100;
									if (pPercent2 != 0) L2 = PMSHH - (RANGE)*pPercent2/100;
									if (pPercent3 != 0) L3 = PMSHH - (RANGE)*pPercent3/100;
									if (pPercent4 != 0) L4 = PMSHH - (RANGE)*pPercent4/100;
									if (pPercent5 != 0) L5 = PMSHH - (RANGE)*pPercent5/100;
									if (pPercent6 != 0) L6 = PMSHH - (RANGE)*pPercent6/100;
									if (pPercent7 != 0) L7 = PMSHH - (RANGE)*pPercent7/100;
									if (pPercent8 != 0) L8 = PMSHH - (RANGE)*pPercent8/100;			
									
							
									if (pPercent1 != 0) CalcDaily.Add(pPercent1.ToString() + " H", H1);
									if (pPercent2 != 0) CalcDaily.Add(pPercent2.ToString() + " H", H2);
									if (pPercent3 != 0) CalcDaily.Add(pPercent3.ToString() + " H", H3);
									if (pPercent4 != 0) CalcDaily.Add(pPercent4.ToString() + " H", H4);
									if (pPercent5 != 0) CalcDaily.Add(pPercent5.ToString() + " H", H5);
									if (pPercent6 != 0) CalcDaily.Add(pPercent6.ToString() + " H", H6);
									if (pPercent7 != 0) CalcDaily.Add(pPercent7.ToString() + " H", H7);
									if (pPercent8 != 0) CalcDaily.Add(pPercent8.ToString() + " H", H8);
									
									
									if (pPercent1 != 0) CalcDaily.Add(pPercent1.ToString() + " L", L1);
									if (pPercent2 != 0) CalcDaily.Add(pPercent2.ToString() + " L", L2);
									if (pPercent3 != 0) CalcDaily.Add(pPercent3.ToString() + " L", L3);
									if (pPercent4 != 0) CalcDaily.Add(pPercent4.ToString() + " L", L4);
									if (pPercent5 != 0) CalcDaily.Add(pPercent5.ToString() + " L", L5);
									if (pPercent6 != 0) CalcDaily.Add(pPercent6.ToString() + " L", L6);
									if (pPercent7 != 0) CalcDaily.Add(pPercent7.ToString() + " L", L7);
									if (pPercent8 != 0) CalcDaily.Add(pPercent8.ToString() + " L", L8);									
									
								
									
										
								}
								
							}
							

							
//							Print("-------------");
//							Print(Time[0]);
//							Print("C " + PCLOSE);
//							Print("H " + PMSHH);
//							Print("L " + PMSLL);
							
							AllDailyBars.Add(new DailyB(PMSLL,PMSHH,POPEN,PCLOSE,CalcDaily));
							AllGlobexHighs.Add(PGHH);
							AllGlobexLows.Add(PGLL);			
							
							
							
							
							
							AllDailyLevels.Clear();
							
							
							
							
							
//							Print(CurrentBar);
							
							
							int DaysBack = 0;		
							string DaysBackString = string.Empty;
							string DBL = "[";
							string DBR = "]";
							//int pShowDaysHighLow = 5;
							
			
							
							
							DaysBack = 0;
							
							
							
							
							double percentupdown = 10;
							double toptop = PCLOSE + PCLOSE*10/100;
							double bottombottom = PCLOSE - PCLOSE*10/100;
							
							
							//Print(AllWholeNumbers.Count);
							
								foreach( double d in AllWholeNumbers)
								{
									if (d >= bottombottom && d<= toptop)
									{
										AllDailyLevels.Add(PriceStringF(d).Replace(".00","") + " " + DaysBackString, d);	
										
										//Print(d);
										
										//AllDailyLevels.Add("W " + PriceStringF(d).Replace(".00","") + " " + DaysBackString, d);	
										//Print(d);
										//AllDailyLevels.Add("R " + d.ToString(), d);	
									}
										
									if (d > toptop)
										break;
										
									//Print(d);
								}
								

							
							
							
							
							//if (AllDailyBars.Count-1 >= pPivotsDays)
							for (int j = AllDailyBars.Count-1; j > 0; j--)
							{	
								
								DaysBack = DaysBack + 1;
								DaysBackString = " " + DBL + DaysBack.ToString() + DBR;
								
							
		
								var pair = AllDailyBars.ElementAt(j);
								PMSHH = pair.iiHigh;
								PMSLL = pair.iiLow;
								PCLOSE = pair.iiClose;
								POPEN = pair.iiOpen;
								RANGE = PMSHH-PMSLL;
								
									
								if (DaysBack <= pShowDaysHighLow)
								{
								
									AllDailyLevels.Add("High" + DaysBackString, PMSHH);
									AllDailyLevels.Add("Low" + DaysBackString, PMSLL);
									
								}
								
								if (DaysBack <= pShowDaysOpen)
								AllDailyLevels.Add("Open" + DaysBackString, POPEN);
									
								if (DaysBack <= pShowDaysClose)
									AllDailyLevels.Add("Close" + DaysBackString, PCLOSE);

								// Globex (overnight) High/Low levels with confluence detection
								if (pDisplayGlobexEnabled && DaysBack <= pShowDaysGlobex)
								{
									if (j < AllGlobexHighs.Count && j < AllGlobexLows.Count)
									{
										double gHigh = AllGlobexHighs[j];
										double gLow = AllGlobexLows[j];

										if (gHigh != 0 && gLow != 9999999)
										{
											string gHighKey = "Globex High" + DaysBackString;
											string gLowKey = "Globex Low" + DaysBackString;

											double confluenceThreshold = pGlobexConfluenceTicks * TickSize;

											// Count confluences with existing levels (different categories only)
											int highMatchCount = 0;
											var highMatchedCats = new HashSet<string>();
											foreach (KeyValuePair<string, double> existing in AllDailyLevels)
											{
												if (existing.Key.Contains("Globex")) continue;
												string cat = GetLevelCategory(existing.Key);
												if (highMatchedCats.Contains(cat)) continue;

												if (Math.Abs(existing.Value - gHigh) <= confluenceThreshold)
												{
													highMatchCount++;
													highMatchedCats.Add(cat);
												}
											}
											if (highMatchCount > 0)
												gHighKey = "Globex High" + DaysBackString + " \u2261" + (highMatchCount + 1);

											int lowMatchCount = 0;
											var lowMatchedCats = new HashSet<string>();
											foreach (KeyValuePair<string, double> existing in AllDailyLevels)
											{
												if (existing.Key.Contains("Globex")) continue;
												string cat = GetLevelCategory(existing.Key);
												if (lowMatchedCats.Contains(cat)) continue;

												if (Math.Abs(existing.Value - gLow) <= confluenceThreshold)
												{
													lowMatchCount++;
													lowMatchedCats.Add(cat);
												}
											}
											if (lowMatchCount > 0)
												gLowKey = "Globex Low" + DaysBackString + " \u2261" + (lowMatchCount + 1);

											if (!AllDailyLevels.ContainsKey(gHighKey))
												AllDailyLevels.Add(gHighKey, gHigh);
											if (!AllDailyLevels.ContainsKey(gLowKey))
												AllDailyLevels.Add(gLowKey, gLow);
										}
									}
								}

								// fib extensions on high and low
								
								foreach (KeyValuePair<string, double> pair2 in pair.iiDict)
								{
									//if (!AllDailyLevels.ContainsKey(pair2.Key + " " + DaysBack.ToString()))
										
										
										
										if (pair2.Key.Contains("Mid"))
										{
											if (DaysBack <= pShowDaysMid)
												AllDailyLevels.Add(pair2.Key + DaysBackString, pair2.Value);
										}
										else // all fib extensions
										{
											if (DaysBack <= pShowDaysFibs)
												AllDailyLevels.Add(pair2.Key + DaysBackString, pair2.Value);
											
										}
											
										
										
										
									
								}
								
								
								
								

							
								
		
								
								
								
							//	if (DaysBack <= pPivotsDays)
								
	
										 if (pPivotsDemarkEnabled)
										 {
											 
										
											 
											if (PCLOSE > POPEN)
												XXXXX = (2*PMSHH) + PMSLL + PCLOSE;
											
											if (PCLOSE < POPEN)
												XXXXX = PMSHH + (2*PMSLL) + PCLOSE;	
											
											if (PCLOSE == POPEN)
												XXXXX = PMSHH + PMSLL + (2*PCLOSE);												
												
											PP = XXXXX / 4;
											
											
											
											R1 = XXXXX/2 - PMSLL;
											S1 = XXXXX/2 - PMSHH;
									
											
											if (DaysBack <= pPivotDemarkPointDays)
												AddLevelIfUnique(AllDailyLevels, "DM PP" + DaysBackString, PP);

											if (DaysBack <= pPivotDemarkLevelDays)
											{
												AddLevelIfUnique(AllDailyLevels, "DM S1" + DaysBackString, S1);
												AddLevelIfUnique(AllDailyLevels, "DM R1" + DaysBackString, R1);
											}
																				 
										 }
										 
										 

										 if (pPivotsWoodiesEnabled)
										 {
											 
											PP = (PMSHH + PMSLL + (2*PCLOSE)) / 4;
											S1 = 2 * PP - PMSHH;
											R1 = 2 * PP - PMSLL;
											S2 = PP - RANGE;
											R2 = PP + RANGE;

											
											if (DaysBack <= pPivotWoodiesPointDays)
												AddLevelIfUnique(AllDailyLevels, "W PP" + DaysBackString, PP);

											if (DaysBack <= pPivotWoodiesLevelDays)
											{
												AddLevelIfUnique(AllDailyLevels, "W S1" + DaysBackString, S1);
												AddLevelIfUnique(AllDailyLevels, "W R1" + DaysBackString, R1);
												AddLevelIfUnique(AllDailyLevels, "W S2" + DaysBackString, S2);
												AddLevelIfUnique(AllDailyLevels, "W R2" + DaysBackString, R2);
											}
								 
										 }		
										 
										 
										 
										 
										 
								if (pPivotsRegularEnabled || pPivotsFibonacciEnabled || pPivotsCamarillaEnabled)
								{
								
									
									PP = (PMSHH + PMSLL + PCLOSE) / 3;
									
									
									
									if (DaysBack <= pPivotPointDays)
										AllDailyLevels.Add("PP" + DaysBackString, PP);
									
									
									
									//https://tradingsim.com/blog/pivot-points/
									
									
									
									if (DaysBack <= pPivotRegularLevelDays)
									
										if (pPivotsRegularEnabled)
										{								
											// REGULAR
											
											
											S1 = 2 * PP - PMSHH;
											R1 = 2 * PP - PMSLL;
											S2 = PP - RANGE;
											R2 = PP + RANGE;
											S3 = PP - 2*RANGE;
											R3 = PP + 2*RANGE;	
			//								S3 = PMSLL - 2*(PMSHH - PP);
			//								R3 = PMSHH + 2*(PP-PMSLL);	
											
											
											
											
												AllDailyLevels.Add("S1" + DaysBackString, S1);
												AllDailyLevels.Add("S2" + DaysBackString, S2);
												AllDailyLevels.Add("S3" + DaysBackString, S3);
												AllDailyLevels.Add("R1" + DaysBackString, R1);
												AllDailyLevels.Add("R2" + DaysBackString, R2);
												AllDailyLevels.Add("R3" + DaysBackString, R3);	
											
										}
									
									
									
									
									if (DaysBack <= pPivotFibonacciLevelDays)
									
										if (pPivotsFibonacciEnabled)
										{
											
											// Fibonacci

											S1 = PP - 0.382*RANGE;
											R1 = PP + 0.382*RANGE;
											S2 = PP - 0.618*RANGE;
											R2 = PP + 0.618*RANGE;											
											S3 = PP - 1.000*RANGE;
											R3 = PP + 1.000*RANGE;
//											S4 = PP - 1.382*RANGE;
//											R4 = PP + 1.382*RANGE;
//											S5 = PP - 1.618*RANGE;
//											R5 = PP + 1.618*RANGE;

											
											
											AddLevelIfUnique(AllDailyLevels, "F S1" + DaysBackString, S1);
											AddLevelIfUnique(AllDailyLevels, "F S2" + DaysBackString, S2);
											AddLevelIfUnique(AllDailyLevels, "F S3" + DaysBackString, S3);
											AddLevelIfUnique(AllDailyLevels, "F R1" + DaysBackString, R1);
											AddLevelIfUnique(AllDailyLevels, "F R2" + DaysBackString, R2);
											AddLevelIfUnique(AllDailyLevels, "F R3" + DaysBackString, R3);
											
										}
										
									
									if (DaysBack <= pPivotCamarillaLevelDays)		
										if (pPivotsCamarillaEnabled)
										{
											// CAMARILLA

											S1 = PCLOSE - 1.1/12*RANGE;
											R1 = PCLOSE + 1.1/12*RANGE;
											S2 = PCLOSE - 1.1/6*RANGE;
											R2 = PCLOSE + 1.1/6*RANGE;
											S3 = PCLOSE - 1.1/4*RANGE;
											R3 = PCLOSE + 1.1/4*RANGE;
											S4 = PCLOSE - 1.1/2*RANGE;
											R4 = PCLOSE + 1.1/2*RANGE;								
											
											
//											if(PMSLL != 0)
//											{
//												R5			= PCLOSE * PMSHH / PMSLL; 
//												S5 			= 2 * PCLOSE - R5;
//											}
									
											AddLevelIfUnique(AllDailyLevels, "C S1" + DaysBackString, S1);
											AddLevelIfUnique(AllDailyLevels, "C S2" + DaysBackString, S2);
											AddLevelIfUnique(AllDailyLevels, "C S3" + DaysBackString, S3);
											AddLevelIfUnique(AllDailyLevels, "C S4" + DaysBackString, S4);
											AddLevelIfUnique(AllDailyLevels, "C R1" + DaysBackString, R1);
											AddLevelIfUnique(AllDailyLevels, "C R2" + DaysBackString, R2);
											AddLevelIfUnique(AllDailyLevels, "C R3" + DaysBackString, R3);
											AddLevelIfUnique(AllDailyLevels, "C R4" + DaysBackString, R4);
						
										}
										
									
										 									
									
									
											
								}
								
								
							}
							
							
							
					
							
								
							//				int CurrentPlot = 50;
				
//			
//							foreach (KeyValuePair<string, double> pair2 in AllDailyLevels)
//							{			
//								Values[CurrentPlot][0] = (pair2.Value);
//								Plots[CurrentPlot].Name = pair2.Key.ToString();
//								CurrentPlot++;
//							}
							
							
							DaysLoaded = DaysLoaded + 1;
							
							if (DaysLoaded >= DaysNeeded) 
								
								
							NewZones = true;
							
							
								
							
							
							
							//Print(DaysLoaded + "  " + Times[0][0]);
						}
						

						
					

						
						
					R8TTime = R8TTime.AddDays(1);
					
				
			
					
							
						
			
												
				FirstEnd = true;	
	
				StartTime = StartTime.AddDays(1);
				EndTime =	EndTime.AddDays(1);
				//FinalTime = FinalTime.AddDays(1);					
					
			}		
			
			AllowedToPlot = true;
			
			MSHH = Math.Max(MSHH, High[0]);
			MSLL = Math.Min(MSLL, Low[0]);		
			
			
			
			//Print("GG 3");

				
			
				int CurrentPlot = 50;
				
					
					
////						
////					Values[CurrentPlot][0] = (pair.iiLow+DIFF*(127)/100);
////					Plots[CurrentPlot].Name = "127 H D(" + DaysBack.ToString();
////					CurrentPlot++;
////					
////					Values[CurrentPlot][0] = (pair.iiLow+DIFF*(161.8)/100);
////					Plots[CurrentPlot].Name = "162 H D(" + DaysBack.ToString();
////					CurrentPlot++;			
////
////					Values[CurrentPlot][0] = (pair.iiLow+DIFF*(200)/100);
////					Plots[CurrentPlot].Name = "200 H D(" + DaysBack.ToString();
////					CurrentPlot++;	
////					
////					Values[CurrentPlot][0] = (pair.iiLow+DIFF*(50)/100);
////					Plots[CurrentPlot].Name = "50 H D(" + DaysBack.ToString();
////					CurrentPlot++;	
////					
////					Values[CurrentPlot][0] = (pair.iiHigh-DIFF*(127)/100);
////					Plots[CurrentPlot].Name = "127 L D(" + DaysBack.ToString();
////					CurrentPlot++;
////					
////					Values[CurrentPlot][0] = (pair.iiHigh-DIFF*(161.8)/100);
////					Plots[CurrentPlot].Name = "162 L D(" + DaysBack.ToString();
////					CurrentPlot++;			
////
////					Values[CurrentPlot][0] = (pair.iiHigh-DIFF*(200)/100);
////					Plots[CurrentPlot].Name = "200 L D(" + DaysBack.ToString();
////					CurrentPlot++;
////					
//					
//				
//					
//				}
						
			
			

			
			
// WE ARE CURRENTLY MAKING THE RANGE
			
			if (CurrentTime.Ticks > StartTime.Ticks && LastBarTime.Ticks <= StartTime.Ticks)
			{			
						if(UseChartData) 
						{
							
							POPEN = Opens[0][0];
							
							//Print(Times[0][0] + "   " + POPEN);
							
						} 
						else 
						{
							POPEN = Opens[1][0];
						}			
			}			

			
			
			//Print("GG 4");
			
			
			
			// getting current day session high and low
			
			if (CurrentTime.Ticks > StartTime.Ticks && CurrentTime.Ticks <= EndTime.Ticks)
			{
				//PlotNow = true;
					
				//Print(Time[0]);

				HH = Math.Max(HH,CurrentHigh);
				LL = Math.Min(LL,CurrentLow);
	
				//CurrentADRToday[0] = (HH-LL);
				
//				if (AllowedToPlot)
//				{
//					if (CurrentADR20[0] != 0)
//					{
//						MS9[0] = (HH-CurrentADR20[0]);
//						MS10[0] = (LL+CurrentADR20[0]);
//					}
//				}
//				
				
				if (CurrentHigh == HH)
				{
					//NewZones = true;
					NewHigh = true;
					NewLow = false;
				}
				
				if (CurrentLow == LL)
				{
					//NewZones = true;
					NewHigh = false;
					NewLow = true;
				}
				
				if (!MakingOR)
				{
				
				}
				
				
						
							
			}
			else
			{


				
			}
			
			
			
		
			bool pWorldMarketsEnabled = false;
			
			if (pWorldMarketsEnabled)
			{

				
				
					
				while(CurrentTime.Ticks > AsianETime.Ticks) 
				{
					

					if (AHH != 0)
					{
						PAHH = AHH;
							
					//if (ALL != 999999)
						PALL = ALL;
						//VLS[0] = (1,R5TTime.Hour.ToString()+" a.m. Measurement");
					}
											
							
							AHH = 0;
							ALL = 999999;
					
					AsianSTime = AsianSTime.AddDays(1);
					AsianETime = AsianETime.AddDays(1);
						
						
				}		
							
				if (CurrentTime.Ticks > AsianSTime.Ticks && CurrentTime.Ticks <= AsianETime.Ticks)
				{
					BackColorI[0] = 1;
					
					//BackColorSeries[0] = Color.FromArgb((int)((double) pOpacity1/100*(double)255), pLineColor1);
					
					
					AHH = Math.Max(AHH,CurrentHigh);
					ALL = Math.Min(ALL,CurrentLow);

								
				}			

	//			if (AllowedToPlot)
				if (PAHH != 0)
				{
	//				DS5[0] = (PAHH);
	//				DS6[0] = (PALL);				
				}
				
				// London
				
				while(CurrentTime.Ticks > LondonETime.Ticks) 
				{
					

					
						PLHH = LHH;
							
				
						PLLL = LLL;
											
							LHH = 0;
							LLL = 999999;
					
	//				if (LHH != 0)
	//					PLHH = LHH;
							
	//				if (LLL != 999999)
	//					PLLL = LLL;
											
	//						LHH = 0;
	//						LLL = 999999;
					
					LondonSTime = LondonSTime.AddDays(1);
					LondonETime = LondonETime.AddDays(1);
						
					
						
				}		
							
				if (CurrentTime.Ticks > LondonSTime.Ticks && CurrentTime.Ticks <= LondonETime.Ticks)
				{
					BackColorI[0] = 2;
					//BackColorSeries[0] = Color.FromArgb((int)((double) pOpacity2/100*(double)255), pLineColor2);
					
					LHH = Math.Max(LHH,CurrentHigh);
					LLL = Math.Min(LLL,CurrentLow);

								
				}			
	 
				//if (AllowedToPlot)
				if (PLHH != 0)
				{
	//				DS7[0] = (PLHH);
	//				MS1[0] = (PLLL);				
				}			
				
				//Print(DS7[0]);
				
				
				while(CurrentTime.Ticks > NYETime.Ticks) 
				{

					
					NYSTime = NYSTime.AddDays(1);
					NYETime = NYETime.AddDays(1);
						
						
				}		
							
				if (CurrentTime.Ticks > NYSTime.Ticks && CurrentTime.Ticks <= NYETime.Ticks)
				{
					BackColorI[0] = 3;


								
				}
				
				
			
			
			}
			
			
//			
//			
//			while(CurrentTime.Ticks > SwingETime.Ticks) 
//			{
//				
////
////				if (LHH != 0)
////					PLHH = LHH;
////						
////				if (LLL != 999999)
////					PLLL = LLL;
////										
////						LHH = 0;
////						LLL = 999999;
////				
////				LHH = Math.Max(LHH,CurrentHigh);
////				LLL = Math.Min(LLL,CurrentLow);
			
			
			
//			RTTS(MS1);
		
		

						

			// emas
			
			
//			EMA1[0] = (EMAMinute(5,8)[0]);
//			EMA2[0] = (EMAMinute(5,20)[0]);
//			EMA3[0] = (EMAMinute(5,50)[0]);
//			EMA4[0] = (EMAMinute(5,200)[0]);			
						
						
						

			//if (BarsInProgress == 0)
				
			//Print(CurrentBar);
			
			//NewZones = true;
			
			
			//Print("GG 5");
			
			NewCurrentBar = 0;
			
			
			
			if (CurrentBars[0] > 0)
			{
				NewCurrentBar = CurrentBars[0];
				
			}
			
			// added to speed up loading
					if (NewZones || CurrentBars[0] == BarsArray[0].Count - 1)
					{
						
						//Print("ELSE " + CurrentBars[0]);
						
						// 
						
						
						ZonesByBarEnd[LastZoneStart] = CurrentBars[0];
						
						
						if (ZonesByBar.ContainsKey(CurrentBars[0]-1))
						{
							if (!ZonesByBar.ContainsKey(CurrentBars[0]))
								ZonesByBar[CurrentBars[0]] = ZonesByBar[CurrentBars[0]-1];
						}
										
					}
			
				if (NewZones)
				{
					
					//Print("NEWZONES " + Times[1][0]);
					
						ZonesByBarEnd[LastZoneStart] = CurrentBars[0];
						
						
						if (ZonesByBar.ContainsKey(CurrentBars[0]-1))
						{
							if (!ZonesByBar.ContainsKey(CurrentBars[0]))
								ZonesByBar[CurrentBars[0]] = ZonesByBar[CurrentBars[0]-1];
						}					
					
					// Universal confluence detection before zone calculation
					DetectUniversalConfluence(AllDailyLevels, pUniversalConfluenceTicks * TickSize);

					CalculateZones();
					LastZoneStart = NewCurrentBar;
				}

				// Set flag plots for Y-axis price markers
				if (PaintPriceMarkers && CurrentBars[0] > 0)
				{
					RebuildDimBrushes();
					int plotIdx = StartFlagPlotN;

					// Only show markers if levels are enabled
					if (pDisplayLevelsEnabled && AllDailyLevels.Count > 0)
					{
						foreach (KeyValuePair<string, double> lvl in AllDailyLevels)
						{
							if (plotIdx > EndFlagPlotN) break;
							if (lvl.Value == 0) continue;
							if (!IsLevelTypeVisible(lvl.Key)) continue;

							Values[plotIdx - 1][0] = lvl.Value;
							PlotBrushes[plotIdx - 1][0] = GetMarkerBrush(lvl.Key);
							plotIdx++;
						}
					}

					// Zone top/bottom - use the most recent zone set
					if (pDisplayLevelsEnabled && pDisplayZonesEnabled && ZonesByBarStart.Count > 0)
					{
						int lastKey = ZonesByBarStart.Keys[ZonesByBarStart.Count - 1];
						foreach (ZoneB zz in ZonesByBarStart[lastKey])
						{
							if (plotIdx + 1 > EndFlagPlotN) break;
							if (zz.iiHigh == zz.iiLow) continue;

							Values[plotIdx - 1][0] = zz.iiHigh;
							PlotBrushes[plotIdx - 1][0] = dimBrushZone;
							plotIdx++;

							Values[plotIdx - 1][0] = zz.iiLow;
							PlotBrushes[plotIdx - 1][0] = dimBrushZone;
							plotIdx++;
						}
					}

					// Clear unused flag plots on current bar
					for (int i = plotIdx; i <= EndFlagPlotN; i++)
						Values[i - 1][0] = 0;
				}

        }
		
		

		
		int NewCurrentBar = 0;

		// Add a level, merging labels if another level at the same price already exists
		// e.g. "R2 [1]" already at 19400, adding "F R2 [1]" at 19400 → label becomes "R2 / F R2 [1]"
		private void AddLevelIfUnique(SortedDictionary<string, double> dict, string key, double price)
		{
			if (dict.ContainsKey(key)) return;

			// Extract the DaysBackString portion (e.g. " [1]") from the new key
			string daysTag = "";
			int bracketPos = key.IndexOf(" [");
			string baseName = key;
			if (bracketPos >= 0)
			{
				daysTag = key.Substring(bracketPos);
				baseName = key.Substring(0, bracketPos);
			}

			// Check if an existing level is within 1 tick of this price
			string matchKey = null;
			foreach (var existing in dict)
			{
				if (Math.Abs(existing.Value - price) < TickSize)
				{
					matchKey = existing.Key;
					break;
				}
			}

			if (matchKey != null)
			{
				// Merge: extract the existing base name (strip its days tag) and combine
				string existingBase = matchKey;
				int existingBracket = matchKey.IndexOf(" [");
				if (existingBracket >= 0)
					existingBase = matchKey.Substring(0, existingBracket);

				string mergedKey = existingBase + " / " + baseName + daysTag;

				// Rename: remove old key, add merged key at same price
				if (!dict.ContainsKey(mergedKey))
				{
					double existingPrice = dict[matchKey];
					dict.Remove(matchKey);
					dict.Add(mergedKey, existingPrice);
				}
			}
			else
			{
				dict.Add(key, price);
			}
		}

		// Level type weights for zone scoring
		private double GetLevelWeight(string levelName)
		{
			if (levelName.Contains("High") || levelName.Contains("Low"))
			{
				if (levelName.Contains("Globex")) return 3.0;
				return 5.0;
			}
			if (levelName.Contains("Close")) return 4.0;
			if (levelName.Contains("Open")) return 3.0;
			if (levelName.Contains("PP")) return 4.0;
			if (levelName.Contains("R1") || levelName.Contains("S1")) return 3.5;
			if (levelName.Contains("R2") || levelName.Contains("S2")) return 3.0;
			if (levelName.Contains("R3") || levelName.Contains("S3")) return 2.5;
			if (levelName.Contains("Middle")) return 2.0;
			if (levelName.Contains("H ") || levelName.Contains("L ")) return 2.0; // Fibonacci
			return 2.0; // Whole numbers, other
		}

		// Extract days-back from level name for recency decay
		private double GetRecencyDecay(string levelName)
		{
			int bracketStart = levelName.IndexOf('[');
			int bracketEnd = levelName.IndexOf(']');
			if (bracketStart >= 0 && bracketEnd > bracketStart)
			{
				string numStr = levelName.Substring(bracketStart + 1, bracketEnd - bracketStart - 1);
				int daysBack;
				if (int.TryParse(numStr, out daysBack) && daysBack > 0)
					return 1.0 / daysBack;
			}
			return 1.0; // Current day or no bracket = full weight
		}

		// Extract confluence count from level name (e.g. "≡3" returns 3, otherwise 1)
		private int GetConfluenceCount(string levelName)
		{
			int idx = levelName.IndexOf('\u2261');
			if (idx < 0) return 1;
			// Try to parse number immediately after ≡
			int startNum = idx + 1;
			int endNum = startNum;
			while (endNum < levelName.Length && char.IsDigit(levelName[endNum])) endNum++;
			if (endNum > startNum)
			{
				int count;
				if (int.TryParse(levelName.Substring(startNum, endNum - startNum), out count) && count > 1)
					return count;
			}
			return 2; // Old-style "≡ X" pair counts as 2
		}

		// Compute zone score and centroid after zone is built
		private void ComputeZoneScoreAndCentroid(ZoneB zone)
		{
			double totalScore = 0;
			double weightedPriceSum = 0;
			double totalWeight = 0;

			foreach (KeyValuePair<string, double> level in zone.iiDict)
			{
				double weight = GetLevelWeight(level.Key);
				double decay = GetRecencyDecay(level.Key);

				// Confluence boost - scales with the actual number of coinciding levels
				int confluenceCount = GetConfluenceCount(level.Key);
				if (confluenceCount > 1)
					weight *= (1.0 + 0.5 * (confluenceCount - 1)); // 2-way = 1.5x, 3-way = 2.0x, 4-way = 2.5x

				// Price-tested freshness - if level has been touched recently, give a boost
				if (pFreshnessDimmingEnabled && LevelLastTestedBar.ContainsKey(level.Key))
				{
					int lastTestedBar = LevelLastTestedBar[level.Key];
					int barsAgo = LastCurrentBar - lastTestedBar;
					if (barsAgo >= 0 && barsAgo < pFreshnessStaleBars)
					{
						// Recent tests boost the weight; fully fresh = 1.25x
						double freshBoost = 1.0 + 0.25 * (1.0 - (double)barsAgo / pFreshnessStaleBars);
						weight *= freshBoost;
					}
				}

				double levelScore = weight * decay;

				totalScore += levelScore;
				weightedPriceSum += level.Value * levelScore;
				totalWeight += levelScore;
			}

			zone.iiScore = Math.Round(totalScore, 1);
			zone.iiCentroid = totalWeight > 0 ? weightedPriceSum / totalWeight : (zone.iiHigh + zone.iiLow) / 2.0;
		}

		// Return tier index (0=low, 1=mid, 2=high, 3=max) for a zone's score
		private int GetZoneTier(double score)
		{
			if (score >= pZoneTierMaxThreshold) return 3;
			if (score >= pZoneTierHighThreshold) return 2;
			if (score >= pZoneTierMidThreshold) return 1;
			return 0;
		}

		// Get the tier brush for a given zone score
		private SharpDX.Direct2D1.Brush GetZoneTierBrush(double score)
		{
			int tier = GetZoneTier(score);
			switch (tier)
			{
				case 3: return cachedZoneTierMaxBrush;
				case 2: return cachedZoneTierHighBrush;
				case 1: return cachedZoneTierMidBrush;
				default: return cachedZoneTierLowBrush;
			}
		}

		// Detect universal confluence - count all matches from different categories within threshold
		private void DetectUniversalConfluence(SortedDictionary<string, double> levels, double threshold)
		{
			if (!pUniversalConfluenceEnabled) return;

			var keys = new List<string>(levels.Keys);
			var confluenceMarks = new Dictionary<string, string>();
			var alreadyPaired = new HashSet<string>();

			for (int i = 0; i < keys.Count; i++)
			{
				if (alreadyPaired.Contains(keys[i])) continue;
				if (keys[i].Contains("\u2261")) continue; // Already marked (Globex confluence)

				var matches = new List<string>();
				var matchedCategories = new HashSet<string>();
				string catI = GetLevelCategory(keys[i]);
				matchedCategories.Add(catI);

				for (int j = i + 1; j < keys.Count; j++)
				{
					if (alreadyPaired.Contains(keys[j])) continue;
					if (keys[j].Contains("\u2261")) continue;

					string catJ = GetLevelCategory(keys[j]);
					if (matchedCategories.Contains(catJ)) continue; // Same category already counted

					if (Math.Abs(levels[keys[i]] - levels[keys[j]]) <= threshold)
					{
						matches.Add(keys[j]);
						matchedCategories.Add(catJ);
					}
				}

				if (matches.Count > 0)
				{
					// Count = 1 (anchor) + number of matches
					int totalCount = 1 + matches.Count;
					// Anchor keeps its name with the confluence count suffix
					confluenceMarks[keys[i]] = keys[i] + " \u2261" + totalCount;
					alreadyPaired.Add(keys[i]);
					// Matched levels keep their original prices but are marked as paired
					foreach (var matchedKey in matches)
						alreadyPaired.Add(matchedKey);
				}
			}

			// Apply the confluence marks (rename keys)
			foreach (var mark in confluenceMarks)
			{
				if (levels.ContainsKey(mark.Key) && !levels.ContainsKey(mark.Value))
				{
					double price = levels[mark.Key];
					levels.Remove(mark.Key);
					levels.Add(mark.Value, price);
				}
			}
		}

		private string GetLevelCategory(string name)
		{
			if (name.Contains("Globex")) return "Globex";
			if (name.Contains("High") || name.Contains("Low")) return "OHLC_HL";
			if (name.Contains("Close")) return "OHLC_C";
			if (name.Contains("Open")) return "OHLC_O";
			if (name.Contains("PP") || name.Contains("R1") || name.Contains("S1") || name.Contains("R2") || name.Contains("S2") || name.Contains("R3") || name.Contains("S3")) return "Pivot";
			if (name.Contains("H ") || name.Contains("L ") || name.Contains("Middle")) return "Fibonacci";
			return "Other";
		}

		// Track last bar where price was near each level (for freshness)
		private Dictionary<string, int> LevelLastTestedBar = new Dictionary<string, int>();

		private Brush MakeDimBrush(Brush source)
		{
			Brush b = source.Clone();
			b.Opacity = 1.0; // Full opacity for Y-axis markers
			b.Freeze();
			return b;
		}

		private void RebuildDimBrushes()
		{
			if (!needDimBrushRebuild) return;
			needDimBrushRebuild = false;
			dimBrushGlobex = MakeDimBrush(pGlobexStroke.Brush);
			dimBrushHL = MakeDimBrush(pLevel2Stroke.Brush);
			dimBrushClose = MakeDimBrush(pLevel3Stroke.Brush);
			dimBrushOpen = MakeDimBrush(pLevel6Stroke.Brush);
			dimBrushFib = MakeDimBrush(pLevel4Stroke.Brush);
			dimBrushPivot = MakeDimBrush(pLevel5Stroke.Brush);
			dimBrushWhole = MakeDimBrush(pLevel8Stroke.Brush);
			dimBrushZone = MakeDimBrush(pBrush05);
		}

		private Brush GetMarkerBrush(string levelKey)
		{
			if (levelKey.Contains("Globex")) return dimBrushGlobex;
			if (levelKey.Contains("Low") || levelKey.Contains("High")) return dimBrushHL;
			if (levelKey.Contains("Close")) return dimBrushClose;
			if (levelKey.Contains("Open")) return dimBrushOpen;
			if (levelKey.Contains("H ") || levelKey.Contains("L ") || levelKey.Contains("Middle")) return dimBrushFib;
			if (levelKey.Contains("PP") || levelKey.Contains("R") || levelKey.Contains("S")) return dimBrushPivot;
			return dimBrushWhole;
		}

		// Check if a level should be visible based on its type and current display toggles
		private bool IsLevelTypeVisible(string levelKey)
		{
			// History filter: if history display is off, only show [1] (yesterday) levels
			if (!pDisplayHistoryEnabled && !levelKey.Contains("[1]"))
				return false;

			if (levelKey.Contains("Globex"))
				return pDisplayGlobexEnabled;
			if (levelKey.Contains("Low") || levelKey.Contains("High"))
				return pDisplayOHLCEnabled;
			if (levelKey.Contains("Close"))
				return pDisplayOHLCEnabled;
			if (levelKey.Contains("Open"))
				return pDisplayOHLCEnabled;
			if (levelKey.Contains("H ") || levelKey.Contains("L ") || levelKey.Contains("Middle"))
				return pDisplayFibEnabled;
			if (levelKey.Contains("PP") || levelKey.Contains("R") || levelKey.Contains("S"))
				return pDisplayPivotsEnabled;
			// Whole numbers and other
			return pDisplayWholeNumbersEnabled;
		}

		private void RecalcCurrentDayLevels()
		{
			DLHH = MHH;
			DLLL = MLL;
			TT = DLHH - DLLL;
			CM = DLLL + TT * (50) / 100;

			if (pPercentCD1 != 0) CHF1 = DLLL + (TT) * pPercentCD1 / 100;
			if (pPercentCD2 != 0) CHF2 = DLLL + (TT) * pPercentCD2 / 100;
			if (pPercentCD3 != 0) CHF3 = DLLL + (TT) * pPercentCD3 / 100;
			if (pPercentCD4 != 0) CHF4 = DLLL + (TT) * pPercentCD4 / 100;

			if (pPercentCD1 != 0) CLF1 = DLHH - (TT) * pPercentCD1 / 100;
			if (pPercentCD2 != 0) CLF2 = DLHH - (TT) * pPercentCD2 / 100;
			if (pPercentCD3 != 0) CLF3 = DLHH - (TT) * pPercentCD3 / 100;
			if (pPercentCD4 != 0) CLF4 = DLHH - (TT) * pPercentCD4 / 100;

			NewLevels = true;
		}
		
		
		
		
		void CalculateZones()
		{

			
//			int ad = 100;
//				foreach (KeyValuePair<string, double> pair2 in AllDailyLevels)
//					{			
//						ad++;
//						AllLevels.Add(ad,pair2.Value);
//						NameLevels.Add(ad,pair2.Key.ToString());	
//						
//					}				
		// order prices lowest to highest
			
		
			
			AllDailyLevelsRounded.Clear();
			
			foreach (KeyValuePair<string, double> pair in AllDailyLevels)
			{
				
				AllDailyLevelsRounded.Add(pair.Key, RTTS(pair.Value));
			}
			
			
			AllLevelsByBar.Remove(CurrentBars[0]);
			
			
			if (pRoundAll)
			{
				AllDailyLevels = new SortedDictionary<string, double>(AllDailyLevelsRounded);
				AllLevelsByBar[CurrentBars[0]] = new SortedDictionary<string, double>(AllDailyLevelsRounded);
			}
			else
			{
				AllLevelsByBar[CurrentBars[0]] = new SortedDictionary<string, double>(AllDailyLevels);
			}
			

			//double PreviousLevel = 0;
			double PreviousLevel2 = 0;
			bool FoundZone = false;
			double ZoneBottom = 0;
			double ZoneTop = 0;
			int ZoneFactors = 0;
			
	
			
			ZonesByBarStart.Remove(CurrentBars[0]);
			ZonesByBar.Remove(CurrentBars[0]);
			zoness.Clear();


			
			int jj = 1;
			
		//Print("--------------------------" + Time[0]);
			
			// SORT ALL LEVELS BY PRICE, LOW TO HIGH

			string PreviousName = string.Empty;
			double PreviousLevel = 0;
			
			TopToDistance.Clear();
			BottomToDistance.Clear();
			TopToBottom.Clear();
			
			
			// Sort levels by value using concrete list instead of LINQ lazy enumerable
			var sourceDict = pRoundAll ? AllDailyLevelsRounded : AllDailyLevels;
			var sortedItems = new List<KeyValuePair<string, double>>(sourceDict);
			sortedItems.Sort((a, b) => a.Value.CompareTo(b.Value));

			foreach (KeyValuePair<string, double> pair in sortedItems)
			{
				
				//pair.Value = RTTS(pair.Value);
				
				//Print(pair.Key + "  " + pair.Value);
				
				if (PreviousName != string.Empty)
				{
					TopToDistance.Add(pair.Key,pair.Value - PreviousLevel);
					//BottomToDistance.Add(PreviousName,pair.Value - PreviousLevel);
					
					TopToBottom.Add(pair.Key,PreviousName);
				}
				
				PreviousLevel = pair.Value;
				PreviousName = pair.Key;
			}
			
			// LOOP THROUGH ALL DIFFERENCES LOW TO HIGH
			
			// Sort by value using concrete list instead of LINQ
			var items2 = new List<KeyValuePair<string, double>>(TopToDistance);
			items2.Sort((a, b) => a.Value.CompareTo(b.Value));
			
			NameLevelsD.Clear();
			zoness.Clear();
			
			

				
			float FinalZoneWidth = pZWM;
			float FinalZoneSpace = pTicksSpace;
			
			
			if (pAutoModeEnabled)
			{
				FinalZoneWidth = ATRMTicks;
				FinalZoneSpace = ATRMTicks;
			}

			// Adaptive zone width using lower quartile of inter-level gaps
			// Uses the 25th percentile gap as the clustering threshold - captures
			// what "close together" means for THIS specific set of levels
			if (pAdaptiveZoneWidthEnabled && sortedItems.Count > 2)
			{
				var gaps = new List<double>();
				for (int gi = 1; gi < sortedItems.Count; gi++)
				{
					double gap = (sortedItems[gi].Value - sortedItems[gi - 1].Value) / TickSize;
					if (gap > 0) gaps.Add(gap);
				}
				if (gaps.Count > 2)
				{
					gaps.Sort();
					// Take the 25th percentile gap, scaled by the sigma multiplier
					int q1Index = Math.Max(0, (int)(gaps.Count * 0.25));
					float adaptiveWidth = (float)(gaps[q1Index] * pAdaptiveZoneSigma);

					// Hard cap: never wider than the ATR-based auto width (or manual pZWM)
					float maxWidth = pAutoModeEnabled ? ATRMTicks : pZWM;
					adaptiveWidth = Math.Min(adaptiveWidth, maxWidth);

					if (adaptiveWidth >= pMinimumTicks)
					{
						FinalZoneWidth = adaptiveWidth;
						FinalZoneSpace = adaptiveWidth;
					}
				}
			}
			
//			FinalZoneWidth = (float) FinalZoneWidth + (float) TickSize/10;
//			FinalZoneSpace = (float) FinalZoneSpace - (float) TickSize/10;
			
		
			foreach (KeyValuePair<string, double> pair in items2)
			{
				//Print(pair.Key + "  " + pair.Value);
				
				bool AddZone = true;
				
				// IF THE SPACE BETWEEN TWO LEVELS IS LESS THAN THE MINIMUM SPACE
				if (pair.Value <= FinalZoneWidth*TickSize)
				{			
					// ADD ZONE IF NONE EXIST
					NameLevelsD = new SortedDictionary<string, double>();
					
					double thistop = 0;
					double thisbottom = 0;
					
					if (AllDailyLevels.ContainsKey(pair.Key) && AllDailyLevels.ContainsKey(TopToBottom[pair.Key]))
					{
					
						if (zoness.Count == 0)
						{
							thistop = AllDailyLevels[pair.Key];
							thisbottom = AllDailyLevels[TopToBottom[pair.Key]];
							NameLevelsD.Add(pair.Key,thistop);
							NameLevelsD.Add(TopToBottom[pair.Key],thisbottom);
							ZoneB zzz = new ZoneB(0,thisbottom,thistop,2,"hey",new SortedDictionary<string,double>(NameLevelsD));								
							
							
							
							zoness.Add(zzz);
							
							AllDailyLevels.Remove(pair.Key);
							AllDailyLevels.Remove(TopToBottom[pair.Key]);
							
							NameLevelsD.Clear();
							
							
						}
						else
						{	
							thistop = AllDailyLevels[pair.Key];
							thisbottom = AllDailyLevels[TopToBottom[pair.Key]];
							
								foreach (ZoneB zz in zoness)
								{
			
									
									//if (zz.iiLow + pZWM*TickSize > thistop && zz.iiHigh - pZWM*TickSize < thisbottom)
									
									
									if (zz.iiHigh + FinalZoneSpace*TickSize >= thisbottom && zz.iiLow <= thisbottom) 
										AddZone = false;
									
								if (zz.iiLow - FinalZoneSpace*TickSize <= thistop && zz.iiHigh >= thistop) 
										AddZone = false;
								
									
									
								}
								
								if (AddZone)
								{
									NameLevelsD.Add(pair.Key,thistop);
									NameLevelsD.Add(TopToBottom[pair.Key],thisbottom);
									ZoneB zzz = new ZoneB(0,thisbottom,thistop,2,"hey",new SortedDictionary<string,double>(NameLevelsD));					
									
									zoness.Add(zzz);
									
									AllDailyLevels.Remove(pair.Key);
									AllDailyLevels.Remove(TopToBottom[pair.Key]);
									
									NameLevelsD.Clear();
								}						
							
							
						}
					}
				}
			}	
			
			
				
			
			// loop through leftover levels
//			for (int index = 0; index < AllDailyLevels.Count; index++) 			
			foreach (KeyValuePair<string, double> pair in AllDailyLevels)
			{	
				//var pair = AllDailyLevels.ElementAt(index);
				
				//pair.
				
				double price = pair.Value;
				
				//Print("level -----------------------------------------------------------------");
				
				//Print(AllLevels2[pair.Key]); // // zone high
				//Print(pair.Key+ " " +pair.Value);
				
				bool LevelAdded = false;		
				
				for (int index2 = 0; index2 < zoness.Count; index2++) 
				//foreach (ZoneB zz in zoness)
				{
					ZoneB zz = zoness[index2];  // Direct index - O(1) for List, no copy since ZoneB is now a class

					if (!LevelAdded)
					if (zz.iiHigh - FinalZoneWidth*TickSize <= price && zz.iiLow + FinalZoneWidth*TickSize >= price)
					{
						// Cap: don't let a zone grow beyond the max height multiplier
						double newLow = Math.Min(zz.iiLow, price);
						double newHigh = Math.Max(zz.iiHigh, price);
						double maxZoneHeight = FinalZoneWidth * TickSize * pZoneMaxHeightMultiplier;
						if ((newHigh - newLow) > maxZoneHeight)
							continue; // Skip - would make zone too tall

						// Modify in-place (reference type, no copy-back needed)
						zz.iiLow = newLow;
						zz.iiHigh = newHigh;
						zz.iiWidth = zz.iiWidth + 1;
						zz.iiDict.Add(pair.Key, pair.Value);

						LevelAdded = true;
					}

				}
			}
							
			
//			foreach (ZoneB z in zoness)
//			{
//				Print(z.iiLow + "   " + z.iiHigh);
//				
//			}

			// Compute score and centroid for each zone
			if (pZoneScoreEnabled)
			{
				foreach (ZoneB zz in zoness)
					ComputeZoneScoreAndCentroid(zz);
			}

			// Share the same list reference instead of creating two copies
			var zonesList = new List<ZoneB>(zoness);
			ZonesByBar[CurrentBars[0]] = zonesList;
			ZonesByBarStart[CurrentBars[0]] = zonesList;
			ZonesByBarEnd[CurrentBars[0]] = CurrentBars[0];
			needZoneDictRebuild = true;
			needTextMeasureCacheReset = true;
			//AllLevelsByBar[CurrentBar] = new SortedDictionary<string, double>(AllDailyLevels);
			
		}
				
		
		
		
//		protected override void OnBarUpdate()
//        {
			
		
//			if (!Permission)
//				return;
					
			
			
			
////			if (CurrentBars[0] < 100 || CurrentBars[1] < 100)
////				return;
			
//			if (CurrentBars[0] < 1)
//				return;
			
			
//			if (RunInit) 
//			{  // Don't do anything on bar 0
//				RunInit = false;

				

//				CurrentADR10[0] = 0;
//				CurrentADR20[0] = 0;
			
				
//				if(UseChartData) 
//				{
//					//Print("heye1");
					
//					FirstBarTime = Time[0];
					
//					StartTime =	new DateTime(Times[0][0].Year,Times[0][0].Month,Times[0][0].Day,pStartTime.Hours,pStartTime.Minutes,0);
//					EndTime	=	new DateTime(Times[0][0].Year,Times[0][0].Month,Times[0][0].Day,pEndTime.Hours,pEndTime.Minutes,0);
////					R1TTime =	new DateTime(Times[0][0].Year,Times[0][0].Month,Times[0][0].Day,pR1Time.Hours,pR1Time.Minutes,0);
////					R2TTime =	new DateTime(Times[0][0].Year,Times[0][0].Month,Times[0][0].Day,pR2Time.Hours,pR2Time.Minutes,0);
////					R3TTime =	new DateTime(Times[0][0].Year,Times[0][0].Month,Times[0][0].Day,pR3Time.Hours,pR3Time.Minutes,0);
////					R4TTime =	new DateTime(Times[0][0].Year,Times[0][0].Month,Times[0][0].Day,pR4Time.Hours,pR4Time.Minutes,0);
////					R5TTime =	new DateTime(Times[0][0].Year,Times[0][0].Month,Times[0][0].Day,pR5Time.Hours,pR5Time.Minutes,0);
//					//R6TTime =	new DateTime(Times[0][0].Year,Times[0][0].Month,Times[0][0].Day,pR6Time.Hours,pR6Time.Minutes,0);
//					//R7TTime =	new DateTime(Times[0][0].Year,Times[0][0].Month,Times[0][0].Day,pR7Time.Hours,pR7Time.Minutes,0);
//					//R8TTime =	new DateTime(Times[0][0].Year,Times[0][0].Month,Times[0][0].Day,pR8Time.Hours,pR8Time.Minutes,0);
					
//					//Print("heye2");
					
//					AsianSTime = new DateTime(Times[0][0].Year,Times[0][0].Month,Times[0][0].Day,pAsianStart.Hours,pAsianStart.Minutes,0);
//					LondonSTime = new DateTime(Times[0][0].Year,Times[0][0].Month,Times[0][0].Day,pLondonStart.Hours,pLondonStart.Minutes,0);
//					NYSTime = new DateTime(Times[0][0].Year,Times[0][0].Month,Times[0][0].Day,pNYStart.Hours,pNYStart.Minutes,0);
//					SwingSTime = new DateTime(Times[0][0].Year,Times[0][0].Month,Times[0][0].Day,pSwingStart.Hours,pSwingStart.Minutes,0);
					
					
					
//					AsianETime = new DateTime(Times[0][0].Year,Times[0][0].Month,Times[0][0].Day,pAsianEnd.Hours,pAsianEnd.Minutes,0);
//					//Print("heye4");
//					LondonETime = new DateTime(Times[0][0].Year,Times[0][0].Month,Times[0][0].Day,pLondonEnd.Hours,pLondonEnd.Minutes,0);
//					NYETime = new DateTime(Times[0][0].Year,Times[0][0].Month,Times[0][0].Day,pNYEnd.Hours,pNYEnd.Minutes,0);
//					SwingETime = new DateTime(Times[0][0].Year,Times[0][0].Month,Times[0][0].Day,pSwingEnd.Hours,pSwingEnd.Minutes,0);
					
//					Midnight = new DateTime(Times[0][0].Year,Times[0][0].Month,Times[0][0].Day,pCustomTime.Hours,pCustomTime.Minutes,0);
						
					
//					//FirstTime = Times[1][0];
//					CurrentTime = Times[0][0];
//					LastBarTime = Times[0][1];
//					CurrentHigh = Highs[0][0];
//					CurrentLow = Lows[0][0];
					
//				} 
//				else 
//				{
					
//					StartTime =	new DateTime(Times[1][0].Year,Times[1][0].Month,Times[1][0].Day,pStartTime.Hours,pStartTime.Minutes,0);
//					EndTime	=	new DateTime(Times[1][0].Year,Times[1][0].Month,Times[1][0].Day,pEndTime.Hours,pEndTime.Minutes,0);

//					R1TTime =	new DateTime(Times[1][0].Year,Times[1][0].Month,Times[1][0].Day,pR1Time.Hours,pR1Time.Minutes,0);
//					R2TTime =	new DateTime(Times[1][0].Year,Times[1][0].Month,Times[1][0].Day,pR2Time.Hours,pR2Time.Minutes,0);
//					R3TTime =	new DateTime(Times[1][0].Year,Times[1][0].Month,Times[1][0].Day,pR3Time.Hours,pR3Time.Minutes,0);
//					R4TTime =	new DateTime(Times[1][0].Year,Times[1][0].Month,Times[1][0].Day,pR4Time.Hours,pR4Time.Minutes,0);
//					R5TTime =	new DateTime(Times[1][0].Year,Times[1][0].Month,Times[1][0].Day,pR5Time.Hours,pR5Time.Minutes,0);					
//					R6TTime =	new DateTime(Times[1][0].Year,Times[1][0].Month,Times[1][0].Day,pR6Time.Hours,pR6Time.Minutes,0);
//					R7TTime =	new DateTime(Times[1][0].Year,Times[1][0].Month,Times[1][0].Day,pR7Time.Hours,pR7Time.Minutes,0);					
//					R8TTime =	new DateTime(Times[1][0].Year,Times[1][0].Month,Times[1][0].Day,pR8Time.Hours,pR8Time.Minutes,0);
					
//					AsianSTime = new DateTime(Times[1][0].Year,Times[1][0].Month,Times[1][0].Day,pAsianStart.Hours,pAsianStart.Minutes,0);
//					LondonSTime = new DateTime(Times[1][0].Year,Times[1][0].Month,Times[1][0].Day,pLondonStart.Hours,pLondonStart.Minutes,0);
//					NYSTime = new DateTime(Times[1][0].Year,Times[1][0].Month,Times[1][0].Day,pNYStart.Hours,pNYStart.Minutes,0);
//					SwingSTime = new DateTime(Times[1][0].Year,Times[1][0].Month,Times[1][0].Day,pSwingStart.Hours,pSwingStart.Minutes,0);
					
//					AsianETime = new DateTime(Times[1][0].Year,Times[1][0].Month,Times[1][0].Day,pAsianEnd.Hours,pAsianEnd.Minutes,0);
//					LondonETime = new DateTime(Times[1][0].Year,Times[1][0].Month,Times[1][0].Day,pLondonEnd.Hours,pLondonEnd.Minutes,0);
//					NYETime = new DateTime(Times[1][0].Year,Times[1][0].Month,Times[1][0].Day,pNYEnd.Hours,pNYEnd.Minutes,0);
//					SwingETime = new DateTime(Times[1][0].Year,Times[1][0].Month,Times[1][0].Day,pSwingEnd.Hours,pSwingEnd.Minutes,0);
					
					
//					FirstTime = Times[1][0];
//					CurrentTime = Times[1][0];
//					LastBarTime = Times[1][1];
//					CurrentHigh = Highs[1][0];
//					CurrentLow = Lows[1][0];
//				}

				
				
				
//				if(EndTime <= StartTime) 
//				{
//					EndTime = EndTime.AddDays(1);
//					FinalTime = FinalTime.AddDays(1);
//				}
				
//				if(AsianETime <= AsianSTime) 
//				{
//					AsianETime = AsianETime.AddDays(1);
					
//				}				
//				if(NYETime <= NYSTime) 
//				{
//					NYETime = NYETime.AddDays(1);
					
//				}	
				
//				if(SwingETime <= SwingSTime) 
//				{
//					SwingETime = SwingETime.AddDays(1);
					
//				}				
//				if(LondonETime <= LondonSTime) 
//				{
//					LondonETime = LondonETime.AddDays(1);
					
//				}
				
//				//Print(AsianSTime);
//				//Print(AsianETime);
				
//				return;  
//			}
			
			
			
			
				
			
			
//// AFTER THE DEFINED SESSION TIME IS COMPLETE
			

				
				
////			if (BarsArray[0].FirstBarOfSession)
////			{
////				//Print(Time[0]);
////				
////				DHH = 0;
////				DLL = 99999;
////				HH = 0;
////				LL = 999999;
////				
////
////				
////			}

//			VLS.Reset(0);
//			//VLS.Reset(1);
			
			
			
////			while(CurrentTime.Ticks > R5TTime.Ticks) 
////			{
////					int dow = (int) CurrentTime.DayOfWeek;	
////								
////					if (dow != 0)
////					{
////
////						if (FirstMeasurement == 0)
////							FirstMeasurement = CurrentBars[0];
////						
////						//VLS[0] = (1,R5TTime.Hour.ToString()+" a.m. Measurement");
////					}	
////					
////					R5TTime = R5TTime.AddDays(1);
////					
////					
////					
////			}
			
			
//			//if (pTActive1)	
////			while(CurrentTime.Ticks > R1TTime.Ticks) 
////			{
////					int dow = (int) CurrentTime.DayOfWeek;	
////								
////					if (dow != 0)
////					{
////
////						DHH = HH;
////						DLL = LL;
////						
////						TT = DHH-DLL;
////						F1 = DLL+TT*(50)/100;
////						F2 = DLL+TT*(9.6)/100;
////						F3 = DLL+TT*(90.5)/100;
////						F4 = DLL+TT*(-30.9)/100;
////						F5 = DLL+TT*(130.9)/100;
////						//VLS[0] = (1,"Revision 1");
////					}	
////					
////					R1TTime = R1TTime.AddDays(1);
////					
////					
////					
////			}
			
		
		
		
//		//	Print(Midnight);
			
//			while(CurrentTime.Ticks > Midnight.Ticks) 
//			{
//				AllowedToPlot = true;
//				Midnight = Midnight.AddDays(1);	
//			}
			
			

//			// end of range
			
//			if (IsFirstTickOfBar && CurrentBar > 0)
//			{
//				CurrentADR10[0] = CurrentADR10[1];
//				CurrentADR20[0] = CurrentADR20[1];
//				CurrentADRToday[0] = CurrentADRToday[1];
//			}
			
//			while(CurrentTime.Ticks > EndTime.Ticks) 
//			{
//					int dow = (int) CurrentTime.DayOfWeek;	
								

//						if (HH != 0 && Time[1].Hour != HolidayIgnore && Time[1].Hour != HolidayIgnore2)
//						{
//							PAHH = 0;
//							PLHH = 0;
//							AllowedToPlot = false;
							
//							PMSHH = HH;
						
							
//							PMSLL = LL;
							
//							DR.Add(PMSHH-PMSLL);
							
//							//Print(Time[0]);
//							//Print(Time[1].Hour);
//							//Print((PMSHH-PMSLL).ToString(PriceString));
							
							
//							CurrentADR10[0] = GetADR(pADRPeriod1);
//							CurrentADR20[0] = GetADR(pADRPeriod2);
							
						
							
//							TT2 = PMSHH-PMSLL;
							
													
//							PP = (PMSHH + PMSLL + PCLOSE) / 3;
//							S1 = 2 * PP - PMSHH;
//							R1 = 2 * PP - PMSLL;
//							S2 = PP - TT2;
//							R2 = PP + TT2;
//							S3 = PP - 2*TT2;
//							R3 = PP + 2*TT2;	
//							S3 = PMSLL - 2*(PMSHH - PP);
//							R3 = PMSHH + 2*(PP-PMSLL);						
							
//							F13 = PMSLL+TT2*(50)/100;
//							F14 = PMSLL+TT2*(127)/100;
//							F15 = PMSLL+TT2*(161.8)/100;
//							F16 = PMSLL+TT2*(200)/100;
//							F17 = PMSHH-TT2*(127)/100;
//							F18 = PMSHH-TT2*(161.8)/100;
//							F19 = PMSHH-TT2*(200)/100;						

							
//							HH = 0;
//							LL = 999999;
							
				
////							Print(Time[0]);
////							Print(PCLOSE);
////							//Print(Time[1].Hour);
////							Print((PMSHH-PMSLL).ToString(PriceString));		
////							
////							
//						}
						
//						if(UseChartData) 
//						{
							
//							PCLOSE = Closes[0][1];
							
//						} 
//						else 
//						{
//							PCLOSE = Closes[1][1];
//						}			
					
					

						
					
//					R8TTime = R8TTime.AddDays(1);
					
				
			
					
						
			
												
//				FirstEnd = true;	
	
//				StartTime = StartTime.AddDays(1);
//				EndTime =	EndTime.AddDays(1);
//				FinalTime = FinalTime.AddDays(1);					
					
//			}		
			

			
	
					
			
			
//			MSHH = Math.Max(MSHH, High[0]);
//			MSLL = Math.Min(MSLL, Low[0]);		
			
			
//// WE ARE CURRENTLY MAKING THE RANGE
			
//			if (CurrentTime.Ticks > StartTime.Ticks && LastBarTime.Ticks <= StartTime.Ticks)
//			{			
//				//MSHH = 0;
//				//MSLL = 999999;				
//				//VLS[0] = (1,"PIT Open");
				
////						PDSHH = DSHH;
////						PDSLL = DSLL;
////										
////						TT1 = DSHH-DSLL;
////						F6 = DSLL+TT1*(50)/100;
////						F7 = DSLL+TT1*(9.6)/100;
////						F8 = DSLL+TT1*(90.5)/100;
////						F9 = DSLL+TT1*(-30.9)/100;
////						F10 = DSLL+TT1*(130.9)/100;
////						F11 = DSLL+TT1*(-71.4)/100;
////						F12 = DSLL+TT1*(171.4)/100;	
//			}			

			
//			if (CurrentTime.Ticks > StartTime.Ticks && CurrentTime.Ticks <= EndTime.Ticks)
//			{
//				//PlotNow = true;
					
//				//Print(Time[0]);

//				HH = Math.Max(HH,CurrentHigh);
//				LL = Math.Min(LL,CurrentLow);
	
//				CurrentADRToday[0] = HH-LL;
				
//				if (AllowedToPlot)
//				{
//					if (CurrentADR20[0] != 0)
//					{
//						MS9[0] = HH-CurrentADR20[0];
//						MS10[0] = LL+CurrentADR20[0];
//					}
//				}
				
				
//				//MSHH = Math.Max(MSHH, High[0]);
//				//MSLL = Math.Min(MSLL, Low[0]);
				
		
				
						
							
//			}
//			else
//			{
				
//			//	if (


//				DSHH = Math.Max(DSHH, High[0]);
//				DSLL = Math.Min(DSLL, Low[0]);	
				
////				PMSHH = MSHH;
////				PMSLL = MSLL;
////										
////						TT2 = MSHH-MSLL;
////						F13 = MSLL+TT2*(50)/100;
////						F14 = MSLL+TT2*(9.6)/100;
////						F15 = MSLL+TT2*(90.5)/100;
////						F16 = MSLL+TT2*(-30.9)/100;
////						F17 = MSLL+TT2*(130.9)/100;
////						F18 = MSLL+TT2*(-71.4)/100;
////						F19 = MSLL+TT2*(171.4)/100;	
////						F20 = MSLL+TT2*(-111.9)/100;
////						F21 = MSLL+TT2*(211.9)/100;					
				
//			}
			
//			if (AllowedToPlot)
//			if (PMSHH != 0)
//			{
//				//Print(PMSHH);
				
				
//				DR1[0] = PMSHH;
//				DR2[0] = PMSLL;
//				DR3[0] = F13;
//				DR4[0] = F14;
//				DR5[0] = F15;
//				DS1[0] = F16;
//				DS2[0] = F17;
//				DS3[0] = F18;
//				DS4[0] = F19;
				

//				// calculate auto pivots
				
//				MS2[0] = PP;
//				MS3[0] = R1;
//				MS4[0] = R2;
//				MS5[0] = R3;
//				MS6[0] = S1;
//				MS7[0] = S2;
//				MS8[0] = S3;
				
					
//			}
			
////								VPen = new Pen(Color.FromArgb((int)((double) 30/100*(double)255), ThisColor),1); // vertical line
////								VPen2 = new Pen(Color.FromArgb((int)((double) 30/100*(double)255), ThisColor),1); // box outline
////								VBrush = new SolidBrush(Color.FromArgb((int)((double) 100/100*(double)255), ThisColor)); //text
////								VBrush2 = new SolidBrush(Color.FromArgb((int)((double) 10/100*(double)255), ThisColor)); // box fill
////								VBrush3 = new SolidBrush(Color.FromArgb((int)((double) 20/100*(double)255), ThisColor)); // line highlight 	
////			
			
			
//			BackColorI[0] = 0;
//			// Asian
			
			

			
			
				
//			while(CurrentTime.Ticks > AsianETime.Ticks) 
//			{
				

//				if (AHH != 0)
//				{
//					PAHH = AHH;
						
//				//if (ALL != 999999)
//					PALL = ALL;
//					VLS[1] = R5TTime.Hour.ToString()+" a.m. Measurement";
//				}
										
						
//						AHH = 0;
//						ALL = 999999;
				
//				AsianSTime = AsianSTime.AddDays(1);
//				AsianETime = AsianETime.AddDays(1);
					
					
//			}		
						
//			if (CurrentTime.Ticks > AsianSTime.Ticks && CurrentTime.Ticks <= AsianETime.Ticks)
//			{
//				BackColorI[0] = 1;
				
//				//BackColorSeries[0] = Color.FromArgb((int)((double) pOpacity1/100*(double)255), pLineColor1);
				
				
//				AHH = Math.Max(AHH,CurrentHigh);
//				ALL = Math.Min(ALL,CurrentLow);

							
//			}			

//			if (AllowedToPlot)
//			if (PAHH != 0)
//			{
//				DS5[0] = PAHH;
//				DS6[0] = PALL;				
//			}
			
//			// London
			
//			while(CurrentTime.Ticks > LondonETime.Ticks) 
//			{
				

//				if (LHH != 0)
//					PLHH = LHH;
						
//				if (LLL != 999999)
//					PLLL = LLL;
										
//						LHH = 0;
//						LLL = 999999;
				
//				LondonSTime = LondonSTime.AddDays(1);
//				LondonETime = LondonETime.AddDays(1);
					
					
//			}		
						
//			if (CurrentTime.Ticks > LondonSTime.Ticks && CurrentTime.Ticks <= LondonETime.Ticks)
//			{
//				BackColorI[0] = 2;
//				//BackColorSeries[0] = Color.FromArgb((int)((double) pOpacity2/100*(double)255), pLineColor2);
				
//				LHH = Math.Max(LHH,CurrentHigh);
//				LLL = Math.Min(LLL,CurrentLow);

							
//			}			

//			if (AllowedToPlot)
//			if (PLHH != 0)
//			{
//				DS7[0] = PLHH;
//				MS1[0] = PLLL;				
//			}			
			
			
			
//			while(CurrentTime.Ticks > NYETime.Ticks) 
//			{
				
////
////				if (LHH != 0)
////					PLHH = LHH;
////						
////				if (LLL != 999999)
////					PLLL = LLL;
////										
////						LHH = 0;
////						LLL = 999999;
				
//				NYSTime = NYSTime.AddDays(1);
//				NYETime = NYETime.AddDays(1);
					
					
//			}		
						
//			if (CurrentTime.Ticks > NYSTime.Ticks && CurrentTime.Ticks <= NYETime.Ticks)
//			{
//				BackColorI[0] = 3;
//				//BackColorSeries[0] = Color.FromArgb((int)((double) pOpacity3/100*(double)255), pLineColor3);
////				
////				LHH = Math.Max(LHH,CurrentHigh);
////				LLL = Math.Min(LLL,CurrentLow);

							
//			}
			
			
//			while(CurrentTime.Ticks > SwingETime.Ticks) 
//			{
				
////
////				if (LHH != 0)
////					PLHH = LHH;
////						
////				if (LLL != 999999)
////					PLLL = LLL;
////										
////						LHH = 0;
////						LLL = 999999;
				
//				SwingSTime = SwingSTime.AddDays(1);
//				SwingETime =SwingETime.AddDays(1);
					
					
//			}		
						
//			if (CurrentTime.Ticks > SwingSTime.Ticks && CurrentTime.Ticks <= SwingETime.Ticks)
//			{
//				BackColorI[0] = 4;
				
//				//BackColorSeries[0] = Color.FromArgb((int)((double) pOpacity4/100*(double)255), pLineColor4);
////				
////				LHH = Math.Max(LHH,CurrentHigh);
////				LLL = Math.Min(LLL,CurrentLow);

							
//			}
			
			
			
			
			
//						if (pActive5)
//						{
							
//							if (pColorAll) 
//								BackBrushesAll[0] = null;
//							else
//								BackBrushes[0] = null;
							
								
//							}
						
//						}
			
			
			
//			RTTS(MS1);

//						//pivots
						
//			RTTS(MS2);
//			RTTS(MS3);
//			RTTS(MS4);
//			RTTS(MS5);
//			RTTS(MS6);
//			RTTS(MS7);
//			RTTS(MS8);
						
//			RTTS(MS9);
//			RTTS(MS10);
						
//			RTTS(DR1);
//			RTTS(DR2);
//			RTTS(DR3);
//			RTTS(DR4);
//			RTTS(DR5);
			
//			RTTS(DS1);
//			RTTS(DS2);
//			RTTS(DS3);
//			RTTS(DS4);
//			RTTS(DS5);
//			RTTS(DS6);
//			RTTS(DS7);			
		

			
			
			
			
//        }
		
		private double GetADR(int Period)
		{
			double totalrange = 0;
			
			if (DR.Count >= Period && Period > 0)
			{
				for (int j = DR.Count-1; j > DR.Count-1-Period; j--)
					totalrange = totalrange + DR[j];
				
				totalrange = totalrange/Period;
				
				return totalrange;
				
			}
			
			return 0;
		}
		
		private void RTTS (Series<double> I)
		{
			if (I.IsValidDataPoint(0))
				I[0] = Instrument.MasterInstrument.RoundToTickSize(I[0]);
			
			
		}			
        private double RTTS(double p)
        {
			//if (pRounding)	
            	return Instrument.MasterInstrument.RoundToTickSize(p);
			//else
			//	return p;
        }		
		
		private void SetBackColor (Brush BB)
		{
			if (pColorAll) 
				BackBrushesAll[0] = BB;
			else
				BackBrushes[0] = BB;			
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
				
		 	
		// Check if a candidate rect overlaps any placed label. Returns true if collision found.
		private bool RectHasOverlap(SharpDX.RectangleF RR)
		{
			const int pad = 2;
			for (int i = 0; i < AllLabelRectangles.Count; i++)
			{
				SharpDX.RectangleF F = AllLabelRectangles[i];
				if (RR.Right > F.Left + pad && RR.Left < F.Right - pad &&
				    RR.Bottom > F.Top + pad && RR.Top < F.Bottom - pad)
					return true;
			}
			return false;
		}

		// Try to place a label without overlapping existing labels.
		// Strategy: try original position, then shift left (up to maxShifts), then try vertical nudge up/down.
		private SharpDX.RectangleF ResolveOverlap(SharpDX.RectangleF initial, int maxShifts)
		{
			// 1. Try original position
			if (!RectHasOverlap(initial))
				return initial;

			// 2. Try shifting left (the original approach, but in a loop)
			SharpDX.RectangleF candidate = initial;
			for (int attempt = 0; attempt < maxShifts; attempt++)
			{
				// Find the leftmost colliding rect's left edge and position to its left
				float shiftTo = float.MaxValue;
				const int pad = 2;
				for (int i = 0; i < AllLabelRectangles.Count; i++)
				{
					SharpDX.RectangleF F = AllLabelRectangles[i];
					if (candidate.Right > F.Left + pad && candidate.Left < F.Right - pad &&
					    candidate.Bottom > F.Top + pad && candidate.Top < F.Bottom - pad)
					{
						shiftTo = Math.Min(shiftTo, F.Left);
					}
				}

				if (shiftTo == float.MaxValue)
					return candidate; // No overlap found

				candidate = new SharpDX.RectangleF(
					shiftTo - candidate.Width - XSBetweenLabels,
					candidate.Y, candidate.Width, candidate.Height);

				if (!RectHasOverlap(candidate))
					return candidate;

				// If shifted off screen left, stop trying horizontal
				if (candidate.Left < 0)
					break;
			}

			// 3. Try vertical nudge: shift up from original position
			candidate = new SharpDX.RectangleF(initial.X, initial.Y - initial.Height - 2, initial.Width, initial.Height);
			if (candidate.Top >= 0 && !RectHasOverlap(candidate))
				return candidate;

			// 4. Try vertical nudge: shift below the line
			candidate = new SharpDX.RectangleF(initial.X, initial.Bottom + 2, initial.Width, initial.Height);
			if (candidate.Bottom <= ChartPanel.H && !RectHasOverlap(candidate))
				return candidate;

			// 5. Last resort: return original position (will overlap, but at least it's at the right price)
			return initial;
		}

		private int RectOverlap3(SharpDX.RectangleF RR)
		{
			float rettt = 9999999999999;

			const int ooo = 6;

			for (int i = 0; i < AllLabelRectangles.Count; i++)
			{
				SharpDX.RectangleF FFFF = AllLabelRectangles[i];

				if (FFFF.Bottom + ooo <= RR.Top || FFFF.Top - ooo >= RR.Bottom)
					continue;

				bool three = RR.Left < FFFF.Right && RR.Right >= FFFF.Right;
				if (!three)
					continue;

				if (RR.Bottom > FFFF.Top + ooo && RR.Top <= FFFF.Top)
					rettt = Math.Min(rettt, FFFF.Left);

				if (RR.Top < FFFF.Bottom - ooo && RR.Bottom >= FFFF.Bottom)
					rettt = Math.Min(rettt, FFFF.Left);
			}
			
			
//			if (rettt != 0)
//				AllLabelRectangles.Remove(DeleteOne);
			
			if (rettt == 9999999999999)
				rettt = 0;
			
			return (int) rettt;
			
		}
		
		private int RectOverlap(SharpDX.RectangleF RR)
		{
			float rettt = 0;
			
			//Print ("RectOverlap Start--------------------------");
			
			
			foreach (SharpDX.RectangleF FFFF in AllLabelRectangles)			
			{
				
				//Print(FFFF.Top + " FFFF " + FFFF.Bottom);
				//Print(RR.Top + " RR " + RR.Bottom);
				
				
				
				// overlaping and below
				if (RR.Bottom > FFFF.Top && RR.Top <= FFFF.Top)
				{
					rettt = FFFF.Top - RR.Bottom;
					
					return (int) rettt;
						
					//Print(rettt);
					
					//break;
				}
				
				// overlaping and above
				
				bool ship = false;
				
				if (ship)
				
				if (RR.Top < FFFF.Bottom && RR.Bottom >= FFFF.Bottom)
				{
					rettt = FFFF.Bottom - RR.Top;
					
					return (int) rettt;
					
					
					//Print(rettt);
					
					//break;
				}				
				
			}
			
			return (int) rettt;
		}
										
										
		private int RectOverlap2 (SharpDX.RectangleF RR)
		{
			float rettt = 0;
			
			//Print ("RectOverlap Start--------------------------");
			
			foreach (SharpDX.RectangleF FFFF in AllLabelRectangles)			
			{
				
				//Print(FFFF.Top + " FFFF " + FFFF.Bottom);
				//Print(RR.Top + " RR " + RR.Bottom);
			
				
				// overlaping and above
				
				
				if (RR.Top < FFFF.Bottom && RR.Bottom >= FFFF.Bottom)
				{
					rettt = FFFF.Bottom - RR.Top;
					
					return (int) rettt;
					
					
					//Print(rettt);
					
					//break;
				}				
				
			}
			
			return (int) rettt;
		}										
										
										
										
		private void DisposeCachedBrush(ref SharpDX.Direct2D1.Brush brush)
		{
			if (brush != null) { brush.Dispose(); brush = null; }
		}

		private void DisposeCachedTextFormat(ref SharpDX.DirectWrite.TextFormat tf)
		{
			if (tf != null) { tf.Dispose(); tf = null; }
		}

		public override void OnRenderTargetChanged()
		{

			if (RenderTarget != null)
			{
				pLevel1Stroke.RenderTarget = RenderTarget;
				pLevel2Stroke.RenderTarget = RenderTarget;
				pLevel3Stroke.RenderTarget = RenderTarget;
				pLevel4Stroke.RenderTarget = RenderTarget;
				pLevel5Stroke.RenderTarget = RenderTarget;
				pLevel6Stroke.RenderTarget = RenderTarget;
				pLevel7Stroke.RenderTarget = RenderTarget;
				pLevel8Stroke.RenderTarget = RenderTarget;
				pGlobexStroke.RenderTarget = RenderTarget;

				ThisStroke.RenderTarget = RenderTarget;

				// Flag for lazy brush recreation on next OnRender (properties may not be available yet)
				needBrushRecreation = true;
				needDimBrushRebuild = true;
			}
			else
			{
				// Dispose all cached brushes when render target is gone
				DisposeCachedBrush(ref cachedAreaBrushDx);
				DisposeCachedBrush(ref cachedSmallAreaBrushDx);
				DisposeCachedBrush(ref cachedTextBrushDx);
				DisposeCachedBrush(ref cachedLineBrushDx);
				DisposeCachedBrush(ref cachedSmallAreaBrushDx2);
				DisposeCachedBrush(ref cachedSmallAreaBrushDx3);
				DisposeCachedBrush(ref cachedSmallAreaBrushDx4);
				DisposeCachedBrush(ref cachedZoneTierLowBrush);
				DisposeCachedBrush(ref cachedZoneTierMidBrush);
				DisposeCachedBrush(ref cachedZoneTierHighBrush);
				DisposeCachedBrush(ref cachedZoneTierMaxBrush);
				DisposeCachedBrush(ref cachedButtonBrushDX);
				DisposeCachedBrush(ref cachedButtonHBrushDX);
				DisposeCachedBrush(ref cachedButtonFHBrushDX);
				DisposeCachedBrush(ref cachedButtonFOFFBrushDX);
				DisposeCachedBrush(ref cachedButtonFONBrushDX);
				DisposeCachedBrush(ref cachedPanelBackdropBrushDX);
				DisposeCachedBrush(ref cachedHeaderBgBrushDX);
				DisposeCachedBrush(ref cachedHeaderBgHoverBrushDX);
				DisposeCachedBrush(ref cachedHeaderTextBrushDX);
				DisposeCachedBrush(ref cachedHoverGlowBrushDX);
				DisposeCachedBrush(ref ChartTextBrushDX);
				DisposeCachedBrush(ref ChartBackgroundBrushDX);
				DisposeCachedBrush(ref ChartBackgroundShade1BrushDX);

				DisposeCachedTextFormat(ref cachedTextFormat);
				DisposeCachedTextFormat(ref cachedLabelTextFormat);
				DisposeCachedTextFormat(ref cachedButtonTextFormat);
				DisposeCachedTextFormat(ref cachedHeaderTextFormat);
			}
		}

		private void RecreateCachedBrushes()
		{
			if (!needBrushRecreation || RenderTarget == null || myProperties == null)
				return;
			needBrushRecreation = false;

			// Dispose old brushes
			DisposeCachedBrush(ref cachedAreaBrushDx);
			DisposeCachedBrush(ref cachedSmallAreaBrushDx);
			DisposeCachedBrush(ref cachedTextBrushDx);
			DisposeCachedBrush(ref cachedLineBrushDx);
			DisposeCachedBrush(ref cachedSmallAreaBrushDx2);
			DisposeCachedBrush(ref cachedSmallAreaBrushDx3);
			DisposeCachedBrush(ref cachedSmallAreaBrushDx4);
			DisposeCachedBrush(ref cachedZoneTierLowBrush);
			DisposeCachedBrush(ref cachedZoneTierMidBrush);
			DisposeCachedBrush(ref cachedZoneTierHighBrush);
			DisposeCachedBrush(ref cachedZoneTierMaxBrush);
			DisposeCachedBrush(ref cachedButtonBrushDX);
			DisposeCachedBrush(ref cachedButtonHBrushDX);
			DisposeCachedBrush(ref cachedButtonFHBrushDX);
			DisposeCachedBrush(ref cachedButtonFOFFBrushDX);
			DisposeCachedBrush(ref cachedButtonFONBrushDX);
			DisposeCachedBrush(ref cachedPanelBackdropBrushDX);
			DisposeCachedBrush(ref cachedHeaderBgBrushDX);
			DisposeCachedBrush(ref cachedHeaderTextBrushDX);
			DisposeCachedBrush(ref cachedHoverGlowBrushDX);
			DisposeCachedBrush(ref ChartTextBrushDX);
			DisposeCachedBrush(ref ChartBackgroundBrushDX);
			DisposeCachedBrush(ref ChartBackgroundShade1BrushDX);

			// Recreate chart brushes
			ChartTextBrushDX = myProperties.ChartText.ToDxBrush(RenderTarget);
			ChartBackgroundBrushDX = myProperties.ChartBackground.ToDxBrush(RenderTarget);
			ChartBackgroundShade1BrushDX = myProperties.ChartBackground.ToDxBrush(RenderTarget);
			ChartBackgroundShade1BrushDX.Opacity = 40/100f;

			// Recreate drawing brushes
			cachedAreaBrushDx = areaBrush.ToDxBrush(RenderTarget);
			cachedSmallAreaBrushDx = smallAreaBrush.ToDxBrush(RenderTarget);
			cachedTextBrushDx = textBrush.ToDxBrush(RenderTarget);
			cachedLineBrushDx = textBrush.ToDxBrush(RenderTarget);
			cachedSmallAreaBrushDx2 = pBrush03.ToDxBrush(RenderTarget);
			cachedSmallAreaBrushDx3 = pBrush04.ToDxBrush(RenderTarget);
			cachedSmallAreaBrushDx4 = pBrush05.ToDxBrush(RenderTarget);

			// Zone score tier brushes - cool->warm gradient from low to high strength
			cachedZoneTierLowBrush = System.Windows.Media.Brushes.SlateGray.ToDxBrush(RenderTarget);
			cachedZoneTierMidBrush = System.Windows.Media.Brushes.Gold.ToDxBrush(RenderTarget);
			cachedZoneTierHighBrush = System.Windows.Media.Brushes.DarkOrange.ToDxBrush(RenderTarget);
			cachedZoneTierMaxBrush = System.Windows.Media.Brushes.OrangeRed.ToDxBrush(RenderTarget);

			// Recreate button brushes
			System.Windows.Media.Brush buttonBrush = GetTextColor(myProperties.ChartBackground);
			cachedButtonBrushDX = buttonBrush.ToDxBrush(RenderTarget);
			System.Windows.Media.Brush buttonHBrush = GetTextColor(myProperties.ChartBackground);
			cachedButtonHBrushDX = buttonHBrush.ToDxBrush(RenderTarget);
			cachedButtonHBrushDX.Opacity = 0.4f;
			cachedButtonFHBrushDX = myProperties.AxisPen.Brush.ToDxBrush(RenderTarget);
			cachedButtonFHBrushDX.Opacity = 0.0f;
			cachedButtonFOFFBrushDX = myProperties.ChartBackground.ToDxBrush(RenderTarget);
			cachedButtonFONBrushDX = areaBrush.ToDxBrush(RenderTarget);

			// Panel backdrop and header brushes
			cachedPanelBackdropBrushDX = myProperties.ChartBackground.ToDxBrush(RenderTarget);
			cachedPanelBackdropBrushDX.Opacity = 0.85f;
			System.Windows.Media.Brush headerTextWpf = GetTextColor(myProperties.ChartBackground);
			cachedHeaderTextBrushDX = headerTextWpf.ToDxBrush(RenderTarget);
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
			cachedHoverGlowBrushDX = headerTextWpf.ToDxBrush(RenderTarget);
			cachedHoverGlowBrushDX.Opacity = 0.08f;

			// Dispose old TextFormats
			DisposeCachedTextFormat(ref cachedTextFormat);
			DisposeCachedTextFormat(ref cachedLabelTextFormat);
			DisposeCachedTextFormat(ref cachedButtonTextFormat);
			DisposeCachedTextFormat(ref cachedHeaderTextFormat);

			// Recreate cached TextFormats
			cachedTextFormat = TextFont.ToDirectWriteTextFormat();

			cachedLabelTextFormat = TextFont.ToDirectWriteTextFormat();
			cachedLabelTextFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
			cachedLabelTextFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
			cachedLabelTextFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;

			{
				float baseSize = (float)myProperties.LabelFont.Size;
				float adjustedSize = Math.Max(11, baseSize + pChartMenuTextSize);
				var btnFont = new NinjaTrader.Gui.Tools.SimpleFont(myProperties.LabelFont.Family.ToString(), adjustedSize);
				cachedButtonTextFormat = btnFont.ToDirectWriteTextFormat();
				cachedButtonTextFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
				cachedButtonTextFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
				cachedButtonTextFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;
			}

			var headerSimpleFont = new NinjaTrader.Gui.Tools.SimpleFont("Arial", 11) { Bold = true };
			cachedHeaderTextFormat = headerSimpleFont.ToDirectWriteTextFormat();
			cachedHeaderTextFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Leading;
			cachedHeaderTextFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
			cachedHeaderTextFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;
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
		private SharpDX.Direct2D1.Brush ChartBackgroundShade1BrushDX = null;
		private SharpDX.Direct2D1.Brush ChartBackgroundErrorBrushDX = null;
		private SharpDX.Direct2D1.Brush ThisBrushDX = null;

		// Cached brushes - recreated only in OnRenderTargetChanged instead of every frame
		private SharpDX.Direct2D1.Brush cachedAreaBrushDx = null;
		private SharpDX.Direct2D1.Brush cachedSmallAreaBrushDx = null;
		private SharpDX.Direct2D1.Brush cachedTextBrushDx = null;
		private SharpDX.Direct2D1.Brush cachedLineBrushDx = null;
		private SharpDX.Direct2D1.Brush cachedSmallAreaBrushDx2 = null;
		private SharpDX.Direct2D1.Brush cachedSmallAreaBrushDx3 = null;
		private SharpDX.Direct2D1.Brush cachedSmallAreaBrushDx4 = null;
		// Zone score tier brushes (for color-coding zones by strength)
		private SharpDX.Direct2D1.Brush cachedZoneTierLowBrush = null;
		private SharpDX.Direct2D1.Brush cachedZoneTierMidBrush = null;
		private SharpDX.Direct2D1.Brush cachedZoneTierHighBrush = null;
		private SharpDX.Direct2D1.Brush cachedZoneTierMaxBrush = null;

		// Pre-cached dimmed brushes for Y-axis price markers (built once, not per-bar)
		private Brush dimBrushGlobex, dimBrushHL, dimBrushClose, dimBrushOpen, dimBrushFib, dimBrushPivot, dimBrushWhole, dimBrushZone;
		private bool needDimBrushRebuild = true;
		private SharpDX.Direct2D1.Brush cachedButtonBrushDX = null;
		private SharpDX.Direct2D1.Brush cachedButtonHBrushDX = null;
		private SharpDX.Direct2D1.Brush cachedButtonFHBrushDX = null;
		private SharpDX.Direct2D1.Brush cachedButtonFOFFBrushDX = null;
		private SharpDX.Direct2D1.Brush cachedButtonFONBrushDX = null;
		private SharpDX.Direct2D1.Brush cachedPanelBackdropBrushDX = null;
		private SharpDX.Direct2D1.Brush cachedHeaderBgBrushDX = null;
		private SharpDX.Direct2D1.Brush cachedHeaderBgHoverBrushDX = null;
		private SharpDX.Direct2D1.Brush cachedHeaderTextBrushDX = null;
		private SharpDX.Direct2D1.Brush cachedHoverGlowBrushDX = null;

		// Cached TextFormats - recreated only in OnRenderTargetChanged instead of every frame
		private SharpDX.DirectWrite.TextFormat cachedTextFormat = null;
		private SharpDX.DirectWrite.TextFormat cachedLabelTextFormat = null;
		private SharpDX.DirectWrite.TextFormat cachedButtonTextFormat = null;
		private SharpDX.DirectWrite.TextFormat cachedHeaderTextFormat = null;
		private bool needBrushRecreation = true;
		private bool needZoneDictRebuild = true;

		// Cache text measurements to avoid per-level TextLayout creation
		private Dictionary<string, SharpDX.Size2F> textMeasureCache = new Dictionary<string, SharpDX.Size2F>();
		private bool needTextMeasureCacheReset = true;

		private SharpDX.Size2F MeasureText(string text, SharpDX.DirectWrite.TextFormat format)
		{
			SharpDX.Size2F size;
			if (textMeasureCache.TryGetValue(text, out size))
				return size;

			using (var layout = new SharpDX.DirectWrite.TextLayout(
				NinjaTrader.Core.Globals.DirectWriteFactory, text, format, 500, format.FontSize))
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
		private const string PANEL_ID = "KeyLevels";
		private const int PANEL_PRIORITY = 100;
		private const string PANEL_HEADER_TEXT = "KEY LEVELS";
		private const float PANEL_TOP_MARGIN = 30f;
		private const float PANEL_GROUP_GAP = 4f;
		private const float chartButtonRadius = 4f;
		private bool panelExpanded = false;
		private float panelAnimProgress = 0f;
		private DateTime panelAnimStartTime = DateTime.MinValue;
		private float panelAnimStartValue = 0f;
		private float panelAnimTargetValue = 0f;
		private const float PANEL_ANIM_DURATION_MS = 200f;
		private SharpDX.RectangleF headerRect = new SharpDX.RectangleF(0, 0, 0, 0);
		private SharpDX.RectangleF lastPanelRect = new SharpDX.RectangleF(0, 0, 0, 0);
		private bool panelWasVisible = false;
		private int currentButtonHover = -1;

		// Slot array indices — object[] used instead of a private class so the
		// AppDomain registry can be shared across separately-compiled indicators.
		private const int SL_PRIORITY = 0;
		private const int SL_LASTHEIGHT = 1;
		private const int SL_MENUOPEN = 2;
		private const int SL_EXPANDED = 3;
		private const int SL_BOTTOMUP = 4;
		private const int SL_USERPRI = 5;   // user-drag reorder priority
		private const int SL_HEADERY = 6;   // last rendered header Y for drag hit-test
		private const int SL_HEADERH = 7;   // last rendered header height
		private const int SL_PANELW = 8;   // last rendered panel width
		private const int SL_SLOT_SIZE = 9;

		// Drag-to-reorder state
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
						int val;
						if (int.TryParse(line.Substring(prefix.Length), out val))
							return val;
					}
				}
			}
			catch { }
			return defaultPri;
		}

		private static void SavePanelPriorities(int chartHash, ConcurrentDictionary<string, object[]> panels)
		{
			try
			{
				// Read existing lines, remove entries for this chart, add current
				var lines = new List<string>();
				string chartPrefix = chartHash + "|";
				if (System.IO.File.Exists(PanelOrderFilePath))
				{
					foreach (string line in System.IO.File.ReadAllLines(PanelOrderFilePath))
						if (!line.StartsWith(chartPrefix)) lines.Add(line);
				}
				foreach (var kvp in panels)
				{
					if (kvp.Value.Length >= SL_SLOT_SIZE)
						lines.Add(chartHash + "|" + kvp.Key + "|" + (int)kvp.Value[SL_USERPRI]);
				}
				System.IO.File.WriteAllLines(PanelOrderFilePath, lines.ToArray());
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
			panels[PANEL_ID] = new object[] { PANEL_PRIORITY, 0f, false, false, false, savedPri, 0f, 0f, 0f };
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
				var s = kvp.Value;
				if (!(bool)s[SL_BOTTOMUP] && GetUserPriority(s) < myPri)
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
	
				
		private List <SharpDX.RectangleF> AllLabelRectangles = new List <SharpDX.RectangleF>();
		
		private int pPriceFlagOpacity = 100;
		
		
		private int CurrentBarsBack = 0;
		
		private int LastHoverLevelStartBar = 0;
		private HashSet<string> HoveredZoneLevelNames = new HashSet<string>();

		// Reusable StringBuilders to avoid per-frame allocation
		private StringBuilder reusableSB = new StringBuilder(128);
		private StringBuilder reusableHudSB = new StringBuilder(64);

		// Pre-allocated ADR box row lists to avoid per-frame allocation
		private List<string> adrRow0, adrRow1;
		private List<int> adrColorRow0, adrColorRow1;
		
		
		protected override void OnRender(ChartControl chartControl, ChartScale chartScale)
		{		
			
			//Print("bug1");
			
			
			//try
			//{
				
	
				
			//	if (!isHitTest)
			//if (pVisualEnabled)			
			//	base.OnRender(chartControl, chartScale);
			
	
			
			
			oldAntialiasMode	= RenderTarget.AntialiasMode;

			// Clip chart content away from panel zone so lines don't bleed through
			bool panelClipActive = false;
			if (!IsInHitTest && panelHash != 0)
			{
				float panelZoneW = GetMaxPanelWidth();
				if (panelZoneW > 0 && (IsAnyPanelMenuOpen() || InMenu))
				{
					RenderTarget.PushAxisAlignedClip(new SharpDX.RectangleF(panelZoneW + 8, 0, ChartPanel.W - panelZoneW - 8, ChartPanel.H), SharpDX.Direct2D1.AntialiasMode.PerPrimitive);
					panelClipActive = true;
				}
			}

			// Cache IsInHitTest in local to avoid repeated virtual property access in draw loops
			bool isHitTest = IsInHitTest;

			if (FirstRender2)
			{
			
				//ChartBarsSwitch2(true);

				
            	myProperties = chartControl.Properties;
				//PreviousDrag = myProperties.AllowSelectionDragging;
				
				
				
				
				
				//chartTrader = Window.GetWindow(ChartControl.Parent).FindFirst("ChartWindowChartTraderControl") as ChartTrader;	
				
				FirstRender2 = false;
				
				
			}
			
			if (FirstRun2)
				{
					// Pick the most accurate "why no levels" message based on actual state.
					// Three cases:
					//   1) Disconnected from a feed for this instrument type      -> "connect a feed"
					//   2) Connected but secondary daily data hasn't arrived yet  -> defer (wait)
					//   3) Connected, daily data flowing, but too few days loaded -> "increase Days to load"
					bool feedConnected = false;
					try
					{
						var mi = Instrument != null ? Instrument.MasterInstrument : null;
						if (mi != null)
						{
							lock (Connection.Connections)
							{
								for (int ci = 0; ci < Connection.Connections.Count; ci++)
								{
									var c = Connection.Connections[ci];
									if (c != null
										&& c.Status == ConnectionStatus.Connected
										&& c.InstrumentTypes != null
										&& c.InstrumentTypes.Contains(mi.InstrumentType))
									{
										feedConnected = true;
										break;
									}
								}
							}
						}
					}
					catch (Exception) { }

					if (!feedConnected)
					{
						// Case 1: disconnected. Consume FirstRun2 with a clear, accurate
						// message. OnConnectionStatusUpdate auto-clears it on connect.
						FirstRun2 = false;
						AddError("No data loaded for " + (Instrument != null && Instrument.MasterInstrument != null ? Instrument.MasterInstrument.Name : "this instrument") + ".\r\nConnect a data feed to load Key Levels.");
					}
					else if (DaysLoaded > 0)
					{
						FirstRun2 = false;

						if (DaysLoaded < DaysNeeded && ZonesByBarStart.Count == 0)
						{
							double RecommendedDaysToLoad = (int) (DaysNeeded*9/5);

							AddError("Not enough historical data to calculate Key Levels (needs " + DaysNeeded.ToString() + " trading days, loaded " + DaysLoaded.ToString() + ").\r\nRight-click the chart \u2192 Data Series\u2026 and set 'Days to load' to about " + RecommendedDaysToLoad.ToString() + ".");
						
						//AddError("Please load more data for a complete calculation of Key Levels.  Right click in the Chart window, and select Data Series... in the Properties box, increase 'Days to load' to around "+ RecommendedDaysToLoad.ToString() +".");	
						
						
//							TriggerCustomEvent(o => {
	
						
//						this.ChartControl.InvalidateVisual();
				
//						}, 0, null);
						
					}
				}
			
						
				}
		
				
				RecreateCachedBrushes();

				// Reset text measurement cache when levels change
				if (needTextMeasureCacheReset)
				{
					textMeasureCache.Clear();
					needTextMeasureCacheReset = false;
				}
													
			
				//ChartBackgroundErrorBrushDX = Brushes.Red.ToDxBrush(RenderTarget);
							

			
			if (!isHitTest)
 			if (AllErrorMessages.Count > 0)
				{
				
					
//					ChartBarsSwitch2(false);
//					myProperties.AllowSelectionDragging = false;
					
					
					
				ChartBackgroundErrorBrushDX = new SharpDX.Direct2D1.SolidColorBrush(RenderTarget, SharpDX.Color.Red);
				ChartBackgroundErrorBrushDX.Opacity = 25/100f;
				
	            CenterText = new SharpDX.DirectWrite.TextFormat(Core.Globals.DirectWriteFactory,
	                "Arial",
	                SharpDX.DirectWrite.FontWeight.Normal,
	                SharpDX.DirectWrite.FontStyle.Normal,
	                SharpDX.DirectWrite.FontStretch.Normal,
	                11.0F);
				
				CenterText = ChartControl.Properties.LabelFont.ToDirectWriteTextFormat();
				CenterText = new SimpleFont(ChartControl.Properties.LabelFont.Family.ToString(), 16).ToDirectWriteTextFormat();
	            CenterText.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
	            CenterText.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
	            CenterText.WordWrapping = SharpDX.DirectWrite.WordWrapping.Wrap;
				
				//CellFormat = FinalFont1.ToDirectWriteTextFormat();
				
				CenterRect = new SharpDX.RectangleF(ChartPanel.X, ChartPanel.Y, ChartPanel.W, ChartPanel.H);
				
				
				
				
				
					RenderTarget.AntialiasMode							= SharpDX.Direct2D1.AntialiasMode.Aliased;
					
						string txt = string.Empty;
					
					reusableSB.Clear();
					foreach (string sss in AllErrorMessages)
						reusableSB.Append(sss).Append("\r\n\r\n");
					reusableSB.Append("Click here to continue.");
					txt = reusableSB.ToString();
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
			
			
			
			
		
				
				
				
			if (!Permission)
				return;
			if (!IsVisible)
				return;
			
			//ChartControlProperties myProperties = chartControl.Properties;

			// Default plotting in base class. Uncomment if indicators holds at least one plot - so in this case we would expect not to see the SMA plot we have as well in this sample script
			//base.OnRender(chartControl, chartScale);

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

			// Use cached TextFormat instead of recreating every frame
			TextFormat	textFormat			= cachedTextFormat;
			
			
			
//			TextFormat textFormat		= new TextFormat(Core.Globals.DirectWriteFactory, "Calibri", SharpDX.DirectWrite.FontWeight.Normal,
//															SharpDX.DirectWrite.FontStyle.Normal, SharpDX.DirectWrite.FontStretch.Normal, fontHeight) 
//															{ TextAlignment = SharpDX.DirectWrite.TextAlignment.Leading, WordWrapping = WordWrapping.NoWrap };
		

		
			
			// Use cached brushes instead of recreating every frame
			SharpDX.Direct2D1.Brush			areaBrushDx			= cachedAreaBrushDx;
			SharpDX.Direct2D1.Brush			smallAreaBrushDx	= cachedSmallAreaBrushDx;
			SharpDX.Direct2D1.Brush			textBrushDx			= cachedTextBrushDx;
			SharpDX.Direct2D1.Brush			lineBrushDx			= cachedLineBrushDx;
			
			
			
			
			
			//SharpDX.Direct2D1.AntialiasMode oldAntialiasMode	= RenderTarget.AntialiasMode;
			RenderTarget.AntialiasMode							= SharpDX.Direct2D1.AntialiasMode.Aliased;
		
			
			
			
			
			
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

			
			
			
			// Set text to chart label color and font
			//textFormat			= chartControl.Properties.LabelFont.ToDirectWriteTextFormat();

//			// Loop through each Plot Values on the chart
			

			
			// DRAW LEVELS FOR RANGE HIGH AND LOW
			
			CurrentBarsBack = LastCurrentBar - ChartBars.ToIndex;
			// Clamp to prevent out-of-range barsAgo access when chart has few bars
			if (CurrentBarsBack < 0) CurrentBarsBack = 0;
			if (CurrentBarsBack >= ChartBars.Count) CurrentBarsBack = Math.Max(0, ChartBars.Count - 1);
			
			bool ddddd = false;
			
			if (ddddd)
			
			
			if (pRightPX2 > 0)
			if (pActive2)
			for (int seriesCount = 0; seriesCount < Values.Length; seriesCount++)
			{
				if (seriesCount < 20)
					continue;
				
				// don't draw 20  and 21 plots.
				
				if (seriesCount <= 3 && !pShowToday)
					continue;
				
				if (seriesCount > 3 && !pShowYesterday)
					continue;				
		
				double	y					= -1;
				double	startX				= -1;
				double	endX				= -1;
				int		firstBarIdxToPaint	= -1;
				int		firstBarPainted		= ChartBars.FromIndex;
				int		lastBarPainted		= ChartBars.ToIndex;
				Plot	plot				= Plots[seriesCount];

				//blueBrush = BrushSeries[seriesCount];
				
				//lineBrushDx = Plots[0].DashStyleDX;
				
				
				smallAreaBrush	= plot.Brush;
				// Create per-plot brush as local (overrides cached brush for this iteration)
				smallAreaBrushDx = smallAreaBrush.ToDxBrush(RenderTarget);
				smallAreaBrushDx.Opacity = areaOpacity/100F;
				
				
				

				int FB = ChartBars.FromIndex;
				int LB = ChartBars.ToIndex;
				
				preval = 0;
				
				//Print("===== " + lastBarPainted + "   " + CurrentBars[0]);
				
				lastBarPainted		= ChartBars.ToIndex;
				
				FirstOne = true;
				
				// Loop through visble bars to render plot values	
				
				int startbar = firstBarPainted;
				//startbar = 0;
				
				
				
				DateTime StartBarTime = Time.GetValueAt(FB);
				DateTime EndBarTime = Time.GetValueAt(LB);
				
				
				DateTime StartDrawTime = StartBarTime.AddDays(-4);
				
				//if (StartBarTime.Ticks <= EndBarTime.Ticks)
				//	EndBarTime = StartBarTime.AddDays(-2);
				
				
				startbar = Math.Max(0,Bars.GetBar(StartDrawTime));
				
				//Print(EndBarTime);
				
				//int startbar = GetValueAt(
				
				
				int endbar = lastBarPainted;
				//endbar = CurrentBars[0];
				
				for (int idx = endbar; idx >= endbar; idx--)
				//for (int idx = endbar; idx >= startbar; idx--)
				//for (int idx = endbar; idx >= Math.Max(startbar, endbar - width); idx--)
				{
					
					//Print(idx);
					
					//if (idx < firstBarIdxToPaint)
					//	break;

					int adjust = 1;
					
					int barsadjust = 1;
					
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
					
					bool c1 = idx == lastBarPainted;
					bool c2 = preval != val && preval != 0;
					c2 = preval != val;
					
					
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
						
						
						
						startPoint	= new Point(chartControl.GetXByBarIndex(ChartBars, idx+1), y);
						endPoint		= new Point(endX, y);
						
						
						
						//if (val == 0)
						
						if (pLabelsEnabled)
						if (val != 0)
						{
						
							// Use measurement cache instead of TextLayout per plot per frame
							SharpDX.Size2F plotLabelSize = MeasureText(plot.Name, textFormat);

							double newy = y - plotLabelSize.Height - 3;

							// text is on a previous line
							endPoint2 = new Point(startPoint.X - plotLabelSize.Width - 4 - pRightPX, newy);

							if (c1) // text is on right edge
								endPoint2 = new Point(ChartPanel.W - plotLabelSize.Width - 4 - pRightPX, newy);


							SharpDX.RectangleF plotLabelRect = new SharpDX.RectangleF((float)endPoint2.X, (float)endPoint2.Y, plotLabelSize.Width + 4, plotLabelSize.Height);
							RenderTarget.DrawText(plot.Name, textFormat, plotLabelRect, plot.BrushDX);
						}
						
						
						
						endPoint = new Point(ChartPanel.W, y); 
						nextPoint = new Point(ChartPanel.W - pRightPX2, y); 
						
						if (!isHitTest) RenderTarget.DrawLine(nextPoint.ToVector2(), endPoint.ToVector2(),  plot.BrushDX , plot.Width, plot.StrokeStyle);
								
						float xxxwid = (float) endPoint.X - (float) nextPoint.X;
								
								rect2			= new SharpDX.RectangleF((float)nextPoint.X,(float)endPoint.Y-pShadowWidth-1,xxxwid,pShadowWidth*2+1);
					
								
								
								if (!isHitTest) RenderTarget.FillRectangle(rect2, smallAreaBrushDx);
						
						
						
						if (preval != 0)
						
						
						
							
						
						
						
						nextPoint = endPoint;
						
						FirstOne = false;
						
					}
					
					
					prey = y;

					
					
				}



				// Draw pivot text
				
				

			}
				
			
			// END DRAW LEVELS

			// Restore smallAreaBrushDx to cached version after series loop
			smallAreaBrushDx = cachedSmallAreaBrushDx;

			RenderTarget.AntialiasMode = oldAntialiasMode;
			
			
			
			
			
			// adr box in corner of chart
			
			if (pActive6)
			{
				
				


					

				
					int FB = ChartBars.FromIndex;
		            int LB = ChartBars.ToIndex;
		          
								


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

					double LTT1 = RTTS(CurrentADR10.GetValueAt(LB));
					double LTT2 = RTTS(CurrentADR20.GetValueAt(LB));
					double LTT3 = RTTS(CurrentADRToday.GetValueAt(LB));
				
				
				
					reusableSB.Clear();

						if (LTT3 != 0)
							reusableSB.Append("Today = ").Append(PriceStringF(LTT3));

						if (LTT1 != 0)
						{
							if (reusableSB.Length > 0) reusableSB.Append("\r\n");
							reusableSB.Append("ADR(").Append(pADRPeriod1).Append(") = ").Append(PriceStringF(LTT1));
						}

						if (LTT2 != 0)
						{
							if (reusableSB.Length > 0) reusableSB.Append("\r\n");
							reusableSB.Append("ADR(").Append(pADRPeriod2).Append(") = ").Append(PriceStringF(LTT2));
						}

						string t = reusableSB.ToString();	
						
					AllStrings.Clear();
					if (adrRow0 == null) adrRow0 = new List<string>(2);
					if (adrRow1 == null) adrRow1 = new List<string>(2);
					adrRow0.Clear();
					adrRow1.Clear();
					adrRow1.Add("0"); adrRow1.Add(t);
					AllStrings.Add(adrRow0);

					//if (pShowPreviousRange)
					{
						AllStrings.Add(adrRow1);					
						
//						TempRow.Clear();	
//						TempRow.Add("0");
//						TempRow.Add(S2);	
//						AllStrings.Add(new List<string>(TempRow));	
						
//						TempRow.Clear();	
//						TempRow.Add("0");
//						TempRow.Add(S3);
//						AllStrings.Add(new List<string>(TempRow));	
						
//						TempRow.Clear();	
//						TempRow.Add("0");
//						TempRow.Add(S4);	
//						AllStrings.Add(new List<string>(TempRow));		
						
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
					if (adrColorRow0 == null) adrColorRow0 = new List<int>(2);
					if (adrColorRow1 == null) adrColorRow1 = new List<int>(2);
					adrColorRow0.Clear();
					adrColorRow1.Clear();
					adrColorRow1.Add(0); adrColorRow1.Add(N1);
					AllColors.Add(adrColorRow0);

					//if (pShowPreviousRange)
					{
						AllColors.Add(adrColorRow1);	
						
//						TempRow2.Clear();	
//						TempRow2.Add(0);
//						TempRow2.Add(N2);						
//						AllColors.Add(new List<int>(TempRow2));	
						
//						TempRow2.Clear();	
//						TempRow2.Add(0);
//						TempRow2.Add(N3);						
//						AllColors.Add(new List<int>(TempRow2));							
						
//						TempRow2.Clear();	
//						TempRow2.Add(0);
//						TempRow2.Add(N4);						
//						AllColors.Add(new List<int>(TempRow2));							
						
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

				
	  
					
	        
					
					// Use cached label TextFormat for ADR box
					SharpDX.DirectWrite.TextFormat textFormat2 = cachedLabelTextFormat;
				
					string text = t;
					
					//text.count
					
					
					
					// Use measurement cache for ADR box text
					SharpDX.Size2F adrTextSize = MeasureText(text, textFormat2);

						int pSpaceBtwRects = pMarginB + 1;
						int TableMarginX = pPixelsFromRight;
						int TableMarginY = pPixelsFromBottom;

						int RectHeight = (int) adrTextSize.Height + YCellPadding;
						int RectWidth = (int) adrTextSize.Width + XCellPadding + 2;

						int RectX = ChartPanel.W  - TableMarginX;
						int RectY = ChartPanel.H  - TableMarginY;
						
					// at bottom
						int StartY = RectY + 2;	// two pixel ninja restriction at bottom of chart
					
						// top 
//						if (pTPosition == TablePositionSR.TopLeft || pTPosition == TablePositionSR.TopRight)
						StartY = TableMarginY + RectHeight - 1 + 2;
					
						bool ShowRects = true;
						
						int NumOfColumns = 0;
						int NumOfRows = 0;
				
				
				
						NumOfColumns = 1;
						NumOfRows = 1;
					
						int X11 = RectX;
						int Y11 = RectY;
					
						int TotalWidth = 0;
					
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
								
								// Use measurement cache instead of TextLayout per cell
								SharpDX.Size2F cellSize = MeasureText(text, textFormat2);
								RectWidth2 = (int) cellSize.Width + XCellPadding;
								
								
								if (j != NumOfColumns)
									TotalWidth = TotalWidth + RectWidth2 + pSpaceBtwRects;
								else
									TotalWidth = TotalWidth + RectWidth2;

							}
							
							//RectY = RectY - RectHeight;
							
						}
						
						
						
						
						//TextFormat	textFormat2			= TextFont.ToDirectWriteTextFormat();	
			
						//TextLayout textLayout2 = new TextLayout(Globals.DirectWriteFactory, "", textFormat2, 1000, textFormat2.FontSize);
					
					
						SharpDX.RectangleF			rect2			= new SharpDX.RectangleF(RectX, RectY, RectWidth, RectHeight);

						for (int i = NumOfRows; i >= 1; i--)
						{

							// right side
							RectX = ChartPanel.W - TableMarginX + 0 + 2;  // - for the 2 pixels ninja doesn't allow drawing 
							
							// left side
//							if (pTPosition == TablePositionSR.TopLeft || pTPosition == TablePositionSR.BottomLeft)
							RectX = TotalWidth + TableMarginX + 1;	
							
							for (int j = NumOfColumns; j >= 1; j--)
							{
								
								RectY = StartY - RectHeight;
								
								int RectWidth2 = RectWidth;
//								if (j == 2 || j == 4)
//									RectWidth2 = pColumnWidthP;
								
								text = AllStrings[j][i];
								
								// insert dynamic width
								
								// Use measurement cache instead of TextLayout per cell
								SharpDX.Size2F cellSize = MeasureText(text, textFormat2);
								RectWidth2 = (int) cellSize.Width + XCellPadding;
								
								
								
								
								if (j != NumOfColumns)
									RectX = RectX - RectWidth2 - pSpaceBtwRects;
								else
									RectX = RectX - RectWidth2;
								
								
								
								textBrushDx = ChartTextBrushDX;
								
//								fillBrushDx = pFillNeutralBrush.ToDxBrush(RenderTarget);
//								if (AllColors[j][i] == 1)
//									fillBrushDx = pFillUpBrush.ToDxBrush(RenderTarget);
//								if (AllColors[j][i] == -1)
//									fillBrushDx = pFillDownBrush.ToDxBrush(RenderTarget);
								
								
								//text = "as" + j.ToString() + "/" + i.ToString();
								
								rect2			= new SharpDX.RectangleF(RectX, RectY, RectWidth2, RectHeight);
								
								// Use cached axis brush instead of creating per-cell
								cachedButtonFHBrushDX.Opacity = 0.5f;
								if (!isHitTest) RenderTarget.DrawRectangle(rect2, cachedButtonFHBrushDX);
								cachedButtonFHBrushDX.Opacity = 0.1f;
								if (!isHitTest) RenderTarget.FillRectangle(rect2, cachedButtonFHBrushDX);
								cachedButtonFHBrushDX.Opacity = 0.0f;
											
								
//								fillBrushDx.Opacity = (float) pFillOpacity/100;
//								if (ShowRects) RenderTarget.FillRectangle(rect2, fillBrushDx);	
								
//								fillBrushDx.Opacity = (float) pOutlineOpacity/100;
//								if (ShowRects) RenderTarget.DrawRectangle(rect2, fillBrushDx, 1);
								
				         		if (!isHitTest) RenderTarget.DrawText(text, textFormat2, rect2, textBrushDx);								
								
								
								
							}
							
							
							
						}
						
						
						
						
				
						
						
						
						
						
					// the textLayout object provides a way to measure the described font through a "Metrics" object
					// This allows you to create new vectors on the chart which are entirely dependent on the "text" that is being rendered
					// For example, we can create a rectangle that surrounds our font based off the textLayout which would dynamically change if the text used in the layout changed dynamically
					
					//int TableMargin = 5;	
						
					SharpDX.Vector2 lowerTextPoint = new SharpDX.Vector2(ChartPanel.W - adrTextSize.Width - TableMarginX,
						ChartPanel.Y + (ChartPanel.H - adrTextSize.Height - TableMarginY));



					SharpDX.RectangleF rect1 = new SharpDX.RectangleF(lowerTextPoint.X - XCellPadding, lowerTextPoint.Y - YCellPadding, adrTextSize.Width + XCellPadding,
						adrTextSize.Height + YCellPadding);

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

					// All ADR TextLayouts replaced with MeasureText cache - no disposal needed
						
				}				
				
				
				
				
//				int FB = ChartBars.FromIndex;
//				int LB = ChartBars.ToIndex;
				
//				return;
			
			// Use cached zone brushes
			SharpDX.Direct2D1.Brush			smallAreaBrushDx2	= cachedSmallAreaBrushDx2;
			SharpDX.Direct2D1.Brush			smallAreaBrushDx3	= cachedSmallAreaBrushDx3;
			SharpDX.Direct2D1.Brush			smallAreaBrushDx4	= cachedSmallAreaBrushDx4;
			
			
			

			//HoveredLinesAlreadyDrawn.Clear();
			AllRefreshRects.Clear();
			
			
//			bool sfsdafgsda = false;
			
//			if (sfsdafgsda)
			 
			//Print(ZonesByBarStart.Count);
			

			// Only rebuild static zone dictionaries when zones actually change
			if (needZoneDictRebuild)
			{
				LevelsInZonesByBar.Clear();
				LevelsAtTopOfZonesByBar.Clear();
				LevelsAtBottomOfZonesByBar.Clear();
			}

			HoveredZoneLevelNames.Clear();
			
			//pDisplayZonesEnabled = true;
			
			//if (pDisplayZonesEnabled)
				
			//if (!isHitTest)
			
			
			
			
			//Print("onrender test 1");
						
			
			
			
			bool pShowGapBetweenDailyLevels = false;
			int ThisGap = 0;
			int ThisGap2 = 0;
			
			
			//if (pShowGapBetweenDailyLevels || BarsArray.Length == 2)
			if (!UseChartData)
			{
				ThisGap = 1;
				ThisGap2 = 1;
			}
			else
			{
				ThisGap = 1;
				ThisGap2 = 0;
			}
			
			if (pShowGapBetweenDailyLevels)
			{
				ThisGap = 0;
			}
			
			
			//Print(BarsArray[0] + " ZonesByBarStart  " + ZonesByBarStart.Count);
			
			
			foreach (KeyValuePair<int, List<ZoneB>> kvp in ZonesByBarStart)
			{
				// if bar is visible on chart
				
				//Print(kvp.Key + "----------" );
				
				//kvp.Key
				
				
				if (needZoneDictRebuild)
				{
					LevelsInZonesByBar.Add(kvp.Key, new Dictionary<string, double>());
					LevelsAtTopOfZonesByBar.Add(kvp.Key, new Dictionary<string, double>());
					LevelsAtBottomOfZonesByBar.Add(kvp.Key, new Dictionary<string, double>());
				}

				// LevelsHoveredByBar removed - hover now uses HoveredZoneLevelNames HashSet	
				
				
				int ZoneStartBar = kvp.Key;
				int ZoneEndBar = ZonesByBarEnd[kvp.Key];
				
				if (ZoneEndBar < ChartBars.FromIndex)
					continue;
				if (ZoneStartBar > ChartBars.ToIndex)
					continue;				
				
//				continue;	
				
				// this line causes error on renko cahrts
				//	double CloseBarPrice = Close.GetValueAt(kvp.Key);
				
				//Print(ZoneEndBar + "  " + ChartBars.ToIndex);
				
				
				// added 1 to make it work on load
							bool IsRightEdge = (ZoneEndBar+1 >= ChartBars.ToIndex);
				

							
				
				
				
							x1 = ChartControl.GetXByBarIndex(ChartBars,ZoneStartBar+ThisGap2);
							//x2 = ChartControl.GetXByBarIndex(ChartBars,kvp.Key);
							
							x2 = ChartControl.GetXByBarIndex(ChartBars,ZoneEndBar+ThisGap);
							
							if (IsRightEdge)
								x2 = ChartPanel.X + ChartPanel.W;
							
							
							
							
							
								//x2 = ChartControl.GetXByTime(EndT) + ChartControl.BarWidth + pExtendRight;
							
							//Print(zz.iiHigh);

							x3 = x2 - x1;
				
				
				
				//if (kvp.Key >= ChartBars.FromIndex && kvp.Key <= ChartBars.ToIndex)
				{
					
					
					
					foreach(ZoneB zz in kvp.Value)
					{

						
//						Print(zz.iiHigh);
						
						
		//				Print(zz.iiHigh);
		//				Print(zz.iiLow);
						
						DisplayZone = true;	
						
						//Print(DisplayZone + " " + zz.iiName);
						
						//if (DisplayZone && zz.iiDate.Ticks > FirstBarTime.Ticks)
						{
						

							
//							y2 = ChartControlGetYByValue(BarsArray[0],zz.iiLow);
//							y1 = ChartControl.GetYByValue(BarsArray[0],zz.iiHigh);
							
							y2 = chartScale.GetYByValue(zz.iiLow);
							y1 = chartScale.GetYByValue(zz.iiHigh)-1;


							y3 = y2 - y1;
							bool isSamePriceZone = (zz.iiHigh == zz.iiLow);

							int x6 = (int) x1 - 80;

						//HOVER

							int HoverYO = 3;

							// For same-price zones, create a hover band centered on the line
							if (isSamePriceZone)
								rect222 = new SharpDX.RectangleF((float)x1, (float)y1 - 3, (float)x3, 6);
							else
								rect222 = new SharpDX.RectangleF((float)x1, (float)y1, (float)x3, (float)y3);
							
							//IsHovered = MM.Y <= y2+HoverYO && MM.Y >= y1-HoverYO;
				
							
							
							if (pDisplayZonesEnabled)
								if (!isSamePriceZone)
								{
									// Pick the fill brush - tiered by score if enabled, otherwise default
									SharpDX.Direct2D1.Brush zoneFillBrush = smallAreaBrushDx4;
									if (pZoneScoreEnabled && pZoneTierColorEnabled && zz.iiScore > 0)
										zoneFillBrush = GetZoneTierBrush(zz.iiScore);

									// Zone score-based opacity modulation
									float savedZoneOpacity = zoneFillBrush.Opacity;
									if (pZoneScoreEnabled && zz.iiScore > 0)
									{
										float scoreOpacity = Math.Min(0.55f, Math.Max(0.18f, (float)(zz.iiScore / 12.0)));
										zoneFillBrush.Opacity = scoreOpacity;
									}

									if (!isHitTest) RenderTarget.FillRectangle(rect222, zoneFillBrush);
									zoneFillBrush.Opacity = savedZoneOpacity;

									// Draw centroid line within zone
									if (pZoneCentroidEnabled && zz.iiCentroid > 0 && !isHitTest)
									{
										double yc = chartScale.GetYByValue(zz.iiCentroid);
										if (yc >= y1 && yc <= y2)
										{
											var centroidPt1 = new SharpDX.Vector2((float)x1, (float)yc);
											var centroidPt2 = new SharpDX.Vector2((float)x2, (float)yc);
											ChartTextBrushDX.Opacity = 0.6f;
											RenderTarget.DrawLine(centroidPt1, centroidPt2, ChartTextBrushDX, 1);
											ChartTextBrushDX.Opacity = 1.0f;
										}
									}

									// Draw zone score label with auto-flip and drop shadow
									if (pZoneScoreEnabled && zz.iiScore > 0 && !isHitTest)
									{
										string scoreText = zz.iiScore.ToString("0.0");
										SharpDX.Size2F scoreSize = MeasureText(scoreText, cachedLabelTextFormat);

										// Default: top-right corner of zone
										float scoreX = (float)x2 - scoreSize.Width - 4;
										float scoreY = (float)y1 + 1;

										// Auto-flip to top-left if near the right edge of the chart
										if ((float)x2 >= ChartPanel.W - scoreSize.Width - 8)
										{
											scoreX = (float)x1 + 4;
										}

										// Drop shadow: dark text offset 1px down-right for contrast
										SharpDX.RectangleF shadowRect = new SharpDX.RectangleF(scoreX + 1, scoreY + 1, scoreSize.Width + 2, scoreSize.Height);
										ChartBackgroundBrushDX.Opacity = 0.85f;
										RenderTarget.DrawText(scoreText, cachedLabelTextFormat, shadowRect, ChartBackgroundBrushDX);
										ChartBackgroundBrushDX.Opacity = 1.0f;

										// Foreground text: tier color if tier coloring is on, else chart text
										SharpDX.Direct2D1.Brush scoreTextBrush = ChartTextBrushDX;
										if (pZoneTierColorEnabled)
										{
											int tier = GetZoneTier(zz.iiScore);
											if (tier >= 2) scoreTextBrush = GetZoneTierBrush(zz.iiScore);
										}
										float savedScoreBrushOpacity = scoreTextBrush.Opacity;
										scoreTextBrush.Opacity = 0.85f;
										SharpDX.RectangleF scoreRect = new SharpDX.RectangleF(scoreX, scoreY, scoreSize.Width + 2, scoreSize.Height);
										RenderTarget.DrawText(scoreText, cachedLabelTextFormat, scoreRect, scoreTextBrush);
										scoreTextBrush.Opacity = savedScoreBrushOpacity;
									}

								}
								else
								{
									//if (!InHitTest) graphics.DrawLine(BoxPen,x1,y1,x2,y1);
									
									endPoint = new Point(x1, y1); 
									nextPoint = new Point(x2, y1); 
									
											
									if (!isHitTest) RenderTarget.DrawLine(nextPoint.ToVector2(), endPoint.ToVector2(), smallAreaBrushDx2 , 3, pLevel1Stroke.StrokeStyle);										
									
								}	
								
						
				
						
							
							rect222			= ExpandRect(rect222,0,1);
							
							
							AllRefreshRects.Add(rect222);
								
								
						
							
							bool IsHovered = MouseIn(rect222,3,3);

							// Track which specific zone's levels are hovered
							if (IsHovered)
							{
								foreach (KeyValuePair<string, double> lvl in zz.iiDict)
									HoveredZoneLevelNames.Add(lvl.Key);
							}

							foreach (KeyValuePair<string, double> pai in zz.iiDict)
							{

								string text = pai.Key;
								double price = pai.Value;

									// Populate zone dictionaries for both filled zones AND same-price zones
									{

										if (pDisplayZonesEnabled && needZoneDictRebuild)
										{
											LevelsInZonesByBar[kvp.Key].Add(text,price);

											if (zz.iiHigh == pai.Value)
												LevelsAtTopOfZonesByBar[kvp.Key].Add(text,price);

											if (zz.iiLow == pai.Value)
												LevelsAtBottomOfZonesByBar[kvp.Key].Add(text,price);

											// For same-price zones, all levels are both top and bottom
											if (zz.iiHigh == zz.iiLow)
											{
												if (!LevelsAtTopOfZonesByBar[kvp.Key].ContainsKey(text))
													LevelsAtTopOfZonesByBar[kvp.Key].Add(text,price);
												if (!LevelsAtBottomOfZonesByBar[kvp.Key].ContainsKey(text))
													LevelsAtBottomOfZonesByBar[kvp.Key].Add(text,price);
											}
										}

									}
										
										
									//if (!HoveredZoneLevels.Contains(text)) HoveredZoneLevels.Add(text);
				
									
								}
							
							
								
								
							
							}
							
							DisplayZone = false;
						
						
						
					}					
					
				}
				
				
			}			
			
				
			
			// Zone dictionaries are now up-to-date, clear rebuild flag
			needZoneDictRebuild = false;

			// Nearest Zone Distance HUD
			if (pNearestZoneHUDEnabled && !isHitTest && pDisplayZonesEnabled && ChartBars.ToIndex < Close.Count && ChartBars.ToIndex >= 0)
			{
				double currentPrice = Close.GetValueAt(Math.Min(ChartBars.ToIndex, Close.Count - 1));
				double nearestAboveDist = double.MaxValue;
				double nearestBelowDist = double.MaxValue;
				double nearestAboveScore = 0;
				double nearestBelowScore = 0;

				// Scan all visible zones for nearest above/below
				foreach (KeyValuePair<int, List<ZoneB>> kvpHud in ZonesByBarStart)
				{
					int hudEndBar = ZonesByBarEnd.ContainsKey(kvpHud.Key) ? ZonesByBarEnd[kvpHud.Key] : 0;
					if (hudEndBar + 1 < ChartBars.ToIndex) continue; // Only check right-edge zones

					foreach (ZoneB zzHud in kvpHud.Value)
					{
						if (zzHud.iiLow > currentPrice)
						{
							double dist = zzHud.iiLow - currentPrice;
							if (dist < nearestAboveDist) { nearestAboveDist = dist; nearestAboveScore = zzHud.iiScore; }
						}
						else if (zzHud.iiHigh < currentPrice)
						{
							double dist = currentPrice - zzHud.iiHigh;
							if (dist < nearestBelowDist) { nearestBelowDist = dist; nearestBelowScore = zzHud.iiScore; }
						}
					}
				}

				// Draw HUD - show points for fractional-tick instruments, ticks otherwise
				reusableHudSB.Clear();
				bool usePoints = (TickSize < 1);
				string distUnit = usePoints ? "pts" : "t";

				if (nearestAboveDist < double.MaxValue)
				{
					string distStr = usePoints
						? nearestAboveDist.ToString("0.##")
						: (nearestAboveDist / TickSize).ToString("0");
					reusableHudSB.Append("\u2191 ").Append(distStr).Append(distUnit);
				}
				if (pZoneScoreEnabled && nearestAboveScore > 0)
					reusableHudSB.Append(" (").Append(nearestAboveScore.ToString("0.0")).Append(")");
				if (nearestBelowDist < double.MaxValue)
				{
					if (reusableHudSB.Length > 0) reusableHudSB.Append("   ");
					string distStr = usePoints
						? nearestBelowDist.ToString("0.##")
						: (nearestBelowDist / TickSize).ToString("0");
					reusableHudSB.Append("\u2193 ").Append(distStr).Append(distUnit);
					if (pZoneScoreEnabled && nearestBelowScore > 0)
						reusableHudSB.Append(" (").Append(nearestBelowScore.ToString("0.0")).Append(")");
				}

				if (reusableHudSB.Length > 0)
				{
					string hudText = reusableHudSB.ToString();
					SharpDX.Size2F hudSize = MeasureText(hudText, cachedLabelTextFormat);
					float hudX = ChartPanel.W - hudSize.Width - 10;
					float hudY = 5;
					SharpDX.RectangleF hudRect = new SharpDX.RectangleF(hudX - 4, hudY, hudSize.Width + 8, hudSize.Height + 4);

					ChartBackgroundShade1BrushDX.Opacity = 0.5f;
					RenderTarget.FillRectangle(hudRect, ChartBackgroundShade1BrushDX);
					ChartBackgroundShade1BrushDX.Opacity = 40/100f;

					ChartTextBrushDX.Opacity = 0.8f;
					RenderTarget.DrawText(hudText, cachedLabelTextFormat, hudRect, ChartTextBrushDX);
					ChartTextBrushDX.Opacity = 1.0f;
				}
			}

			// DRAW ALL THE LEVELS
			
			TextObjectsToDraw.Clear();
			objectPoolIndex = 0; // Reset object pool for reuse
			TextObjectsToDrawHovered.Clear();
			//if (!isHitTest)
			
			foreach (KeyValuePair<int, SortedDictionary<string, double>> kvp in AllLevelsByBar)
			{
					
				int ZoneStartBar = kvp.Key;
				int ZoneEndBar = ZonesByBarEnd[kvp.Key];
				
				if (ZoneEndBar < ChartBars.FromIndex)
					continue;
				if (ZoneStartBar > ChartBars.ToIndex)
					continue;	
				
				
	
			
				// added 1 to make it work on load
							bool IsRightEdge = (ZoneEndBar+1 >= ChartBars.ToIndex);
				

				
							x1 = ChartControl.GetXByBarIndex(ChartBars,ZoneStartBar+ThisGap2);
							//x2 = ChartControl.GetXByBarIndex(ChartBars,kvp.Key);
							
							x2 = ChartControl.GetXByBarIndex(ChartBars,ZoneEndBar+ThisGap) ;
							
							if (IsRightEdge)
								x2 = ChartPanel.X + ChartPanel.W;
							
							
								//x2 = ChartControl.GetXByTime(EndT) + ChartControl.BarWidth + pExtendRight;
							
							//Print(zz.iiHigh);

							x3 = x2 - x1;		
				
		
				
							
						//HOVER
							
							int HoverYO = 3;
							
							bool IsHovered = true;
							
				int moveleft = 0;
				
							//IsHovered = MM.Y <= y2+HoverYO && MM.Y >= y1-HoverYO;
							
						
							{
								if (y2 != y1)
								{
								
									rect222			= new SharpDX.RectangleF((float)x1, (float)y1, (float)x3, (float)y3);
										//RenderTarget.FillRectangle(rect222, smallAreaBrushDx2);	
									
								}
								else
								{
									//if (!InHitTest) graphics.DrawLine(BoxPen,x1,y1,x2,y1);
									
									endPoint = new Point(x1, y1); 
									nextPoint = new Point(x2, y1); 
									
											
									//									
										
									//RenderTarget.DrawLine(nextPoint.ToVector2(), endPoint.ToVector2(),  plot.BrushDX , plot.Width, plot.StrokeStyle);
						
									//RenderTarget.DrawLine(nextPoint.ToVector2(), endPoint.ToVector2(), ThisStroke.BrushDX , ThisStroke.Width, ThisStroke.StrokeStyle);	
							
								}	
								
								
							}
								
	

							
								AllLabelRectangles.Clear();
							
								foreach (KeyValuePair<string, double> pai in kvp.Value)
								{
									//Print(pai.Key + "   " + pai.Value);	
									
									string text = pai.Key;
									double price = pai.Value;
									
									int PlotNumber = 0;
									
									bool IsInZone = LevelsInZonesByBar[ZoneStartBar].ContainsKey(text);
									bool IsTopOfZone = LevelsAtTopOfZonesByBar[ZoneStartBar].ContainsKey(text);
									bool IsBottomOfZone = LevelsAtBottomOfZonesByBar[ZoneStartBar].ContainsKey(text);								
									
									
									ThisStroke = pLevel7Stroke;


									if (text.Contains("Globex"))
									{
										ThisStroke = pGlobexStroke;

										if (!pDisplayGlobexEnabled && !IsInZone)
											continue;
									}

									else if (text.Contains("Low") || text.Contains("High"))
									{
										ThisStroke = pLevel2Stroke;
										ThisStroke.Opacity = pLevel2Stroke.Opacity;

										if (!pDisplayOHLCEnabled && !IsInZone)
											continue;
										
									}
									
									else if (text.Contains("Close"))
									{
										ThisStroke = pLevel3Stroke;
										
										if (!pDisplayOHLCEnabled && !IsInZone)
											continue;
									}
									
									else if (text.Contains("Open"))
									{
										ThisStroke = pLevel6Stroke;
										
										if (!pDisplayOHLCEnabled && !IsInZone)
											continue;
									}
									
									
									
									
									else if (text.Contains("H ") || text.Contains("L ") )
									{
										ThisStroke = pLevel4Stroke;
										
										if (!pDisplayFibEnabled && !IsInZone)
											continue;
									}
									
								
										
									else if (text.Contains("Middle"))
									{
										ThisStroke = pLevel1Stroke;
										//ThisStroke.Opacity = pLevel1Stroke.Opacity;
										
										if (!pDisplayFibEnabled && !IsInZone)
											continue;
									}
									
										
									else if (text.Contains("PP") || text.Contains("R") || text.Contains("S") )
									{
										ThisStroke = pLevel5Stroke;
										
										if (!pDisplayPivotsEnabled && !IsInZone)
											continue;
										
									}
									else
									{
										ThisStroke = pLevel8Stroke;
										
										if (!pDisplayWholeNumbersEnabled && !IsInZone)
											continue;										
										
									}
									
									
									
									Plot	plot				= Plots[PlotNumber];


									y6 = chartScale.GetYByValue(pai.Value);

									// Early off-screen check - skip before creating any objects
									if (y6 < 0 || y6 > ChartPanel.H )
										continue;
										//
									
									
									
									
									nextPoint = new Point(x2, y6); 
								endPoint = new Point(x1, y6); 
									
									
							
									
											//if (!HoveredLinesAlreadyDrawn.Contains(text))
									
					
						
									
									
									
									

									
									if (!pDisplayHistoryEnabled)
									if (!IsInZone)
									if (!text.Contains("[1]"))
										continue;
									
									bool IsYesterday = text.Contains("[1]") && pDisplayLevelsEnabled;
									
									bool IsHoveredLevel = false;

									bool oneenabled = pDisplayLevelsEnabled || pDisplayZonesEnabled;

									// Zone hover: check HoveredZoneLevelNames (set from zone rect MouseIn)
									if (oneenabled)
										IsHoveredLevel = HoveredZoneLevelNames.Contains(text);

									// For zone levels, only highlight top/bottom (like Before version)
									if (IsHoveredLevel && IsInZone)
									{
										if (!IsTopOfZone && !IsBottomOfZone)
											IsHoveredLevel = false;
									}
									
									
									//if ()
									
								float halflinewidth = 0;
									
									if (oneenabled || IsHoveredLevel || IsInZone)
											{
									
												
												//if (HoveredZoneLevels.Contains(text))
												//ThisStroke = pLevel8Stroke;
												
												float savedOpacity = ThisStroke.BrushDX.Opacity;
												float finalwidth = ThisStroke.Width;

												if (pLevelWidthOverride != 0)
													finalwidth = pLevelWidthOverride;

												// Level Freshness Dimming - prefer price-tested freshness over days-back
												if (pFreshnessDimmingEnabled)
												{
													float freshness = 1.0f;
													bool wasTested = LevelLastTestedBar.ContainsKey(text);
													if (wasTested)
													{
														int barsAgo = LastCurrentBar - LevelLastTestedBar[text];
														if (barsAgo < 0) barsAgo = 0;
														if (barsAgo >= pFreshnessStaleBars)
															freshness = 0.35f;
														else
															freshness = 1.0f - 0.65f * ((float)barsAgo / pFreshnessStaleBars);
													}
													else
													{
														// Never tested - fall back to days-back decay
														double recencyDecay = GetRecencyDecay(text);
														if (recencyDecay < 0.5) // 3+ days old and never touched
															freshness = Math.Max(0.3f, (float)(recencyDecay * 2.0));
													}
													if (freshness < 1.0f)
														ThisStroke.BrushDX.Opacity = savedOpacity * freshness;
												}

												// Confluence level: thicker line
												if (text.Contains("\u2261"))
													finalwidth = Math.Max(finalwidth, finalwidth + 1);

												if (!IsHoveredLevel && IsInZone && !IsYesterday)
												{
													ThisStroke.BrushDX.Opacity = 20/100f;
													finalwidth = 1;
												}
												bool showline = false;

												if (pDisplayLevelsEnabled)
												{
													showline = true;
												}
												else
												{
													if (IsInZone)
														showline = true;
												}

												if (showline)
													if (!isHitTest)
												RenderTarget.DrawLine(nextPoint.ToVector2(), endPoint.ToVector2(), ThisStroke.BrushDX , finalwidth, ThisStroke.StrokeStyle);

												// Restore original opacity instead of disposing the Stroke's managed brush
												ThisStroke.BrushDX.Opacity = savedOpacity;
												
												float linewidth = ThisStroke.Width;
												halflinewidth = linewidth/2;	
												float totalexp = pMainGlowWidth;
												float totalglowheight = totalexp+totalexp+linewidth;
												
												rect222			= new SharpDX.RectangleF((float)x1, (float)y6-halflinewidth-totalexp, (float)x2-(float)x1, (float)totalglowheight);
							
												bool IsHoveredZone = IsHoveredLevel && IsInZone;
												
												IsHoveredZone = IsHoveredLevel && IsInZone && (IsTopOfZone || IsBottomOfZone);
												// plot all glow if zones are turned off
												// plot glow if a level is not hovered and not in a zone
													
												if (showline)
												if (pMainGlowOpacity != 0)
												if ((!IsHoveredLevel && !IsInZone) || !pDisplayZonesEnabled || IsHoveredZone)
												{
													//Print (x1 + "   " + x2);
													ThisBrushDX = ThisStroke.BrushDX;	
													//ThisBrushDX = ThisStroke.BrushDX;
													ThisBrushDX.Opacity = pMainGlowOpacity/100f;
													
													if (IsTopOfZone)
														
													rect222			= new SharpDX.RectangleF((float)x1, (float)y6-halflinewidth-totalexp, (float)x2-(float)x1, (float)totalexp+linewidth);	
													
													
													if (IsBottomOfZone)
														
													rect222			= new SharpDX.RectangleF((float)x1, (float)y6-halflinewidth, (float)x2-(float)x1, (float)totalexp+linewidth);														
													
													
													if (!isHitTest)
													RenderTarget.FillRectangle(rect222, ThisBrushDX);
												}
												
												totalexp = Math.Max(3,pMainGlowWidth);
												
												rect222			= new SharpDX.RectangleF((float)x1, (float)y6-halflinewidth-totalexp, (float)x2-(float)x1, (float)totalglowheight);
												
												if (pDisplayLevelsEnabled)
												AllRefreshRects.Add(rect222);
												
												
												
											}
							
										// Per-level hover detection (restored from Before version)
										if (pDisplayLevelsEnabled)
										{
											bool levelHovered = MouseIn(rect222, 3, 3);
											if (levelHovered)
											{
												LastHoverLevelStartBar = ZoneStartBar;
												HoveredZoneLevelNames.Add(text);
											}
										}
											
					//if (IsHovered || pLabelsEnabled)
										
					if (pDisplayLevelsEnabled || pDisplayZonesEnabled)		
					//if (pLabelsEnabled)
						
					{
						
						//Print(text);
						
						
						
	// Use cached text measurement instead of creating/disposing TextLayout per level
					SharpDX.DirectWrite.TextFormat textFormat2 = cachedLabelTextFormat;

					// Use measurement cache - avoids TextLayout alloc per level per frame
					SharpDX.Size2F textSize = MeasureText(text, textFormat2);

						int pSpaceBtwRects = pMarginB + 1;
						int TableMarginX = pPixelsFromRight;
						int TableMarginY = pPixelsFromBottom;

						int RectHeight = (int) textSize.Height + YCellPadding;
						int RectWidth = (int) textSize.Width + XCellPadding + 2;

						
						
						int RectX = (int) x2 - RectWidth - 0 - moveleft;
						int RectY = (int) y6 - RectHeight  +1 - pSpaceToLine - 1- (int) halflinewidth;
						
						
						if (IsRightEdge)
							RectX = RectX - pRightPX;
						
						
						
				 		//moveleft = moveleft + 50;
								
								
								textBrushDx = ChartTextBrushDX;
								
//								fillBrushDx = pFillNeutralBrush.ToDxBrush(RenderTarget);
//								if (AllColors[j][i] == 1)
//									fillBrushDx = pFillUpBrush.ToDxBrush(RenderTarget);
//								if (AllColors[j][i] == -1)
//									fillBrushDx = pFillDownBrush.ToDxBrush(RenderTarget);
								
								
								//text = "as" + j.ToString() + "/" + i.ToString();
								
								rect222			= new SharpDX.RectangleF(RectX, RectY, RectWidth, RectHeight);
								
							//	SharpDX.Direct2D1.Brush MessageBrushDX = ChartControl.Properties.AxisPen.Brush.ToDxBrush(RenderTarget);
								
							
											textBrushDx = ThisStroke.BrushDX;
											
											textBrushDx.Opacity = pMainTextOpacity/100f;
													
					
							
										
										
												if (!IsHoveredLevel && IsInZone && !IsYesterday)
												{
													//smallAreaBrushDx5.Opacity = 20/100f;
													textBrushDx.Opacity = 10/100f;
												}
												
												
												
												
											
									//if (!HoveredLinesAlreadyDrawn.Contains(text))
									{

										
										// Resolve label overlap using improved multi-direction algorithm
										rect222 = ResolveOverlap(rect222, 8);
										AllLabelRectangles.Add(rect222);

									ObjectToDraw ooo = GetPooledObject();
									ooo.Rectangle = rect222;
									ooo.Brush = textBrushDx;
									ooo.Brush2 = ThisStroke.Brush;
									//ooo.Brush2 = ThisStroke.Brush;	
									ooo.Text = text;
									ooo.Price = pai.Value;
									ooo.StartBar = ZoneStartBar;
									TextObjectsToDraw.Add(ooo);
						
										
										
									}
									
									
									//RenderTarget.DrawText(text, textFormat2, rect222, ThisStroke.BrushDX);		
									
									
									
//									SizeF SZ = graphics.MeasureString("T",pTextFont);
									
//									if (kvp.Key == ChartControl.LastBarPainted)
//									{
//										if (!InHitTest) graphics.DrawString(pai.Key, pTextFont, AxisBrush, x6, y6-SZ.Height, stringFormatNear);
										
//										y2 = y2 + 15;
//										x6 = x6-60;
//									}
									
									// TextLayout replaced with MeasureText cache - no disposal needed
									
					}
								}
							
								
								
								y4 = y1 + y3/2 - 1;
								
								//if (x1 <= ChartControl.GetXByBarIdx(BarsArray[0],lastBarIndex) && x2 >= ChartControl.GetXByBarIdx(BarsArray[0],lastBarIndex))
									
									
									
								//if (Time[CurrentBars[0]-lastBarIndex] >= StartT && Time[CurrentBars[0]-lastBarIndex] <= EndT)
								{
									
	//								Plots[jjj].Pen.Color = LevelPen.Color;
	//								Values[jjj].Set(CurrentBars[0]-lastBarIndex,zz.iiHigh);
	//								
	//								jjj++;
	//								
	//								Values[jjj].Set(CurrentBars[0]-lastBarIndex,zz.iiLow);
	//								
	//								jjj++;
								}
								
							
								
								
								if (kvp.Key == ChartBars.ToIndex)
								{
							
									int pLabelOffset = 5;
									
										float xx1 = (float) x2 + 1 + pLabelOffset;
										//float yy1 = y4 - pTextFont.GetHeight() / 2 - 1;
										float yy1 = 0;
									
									
										//SizeF SZ = graphics.MeasureString(zz.iiName,pTextFont);
										
//										float xxw = SZ.Width+1;
//										float yyw = SZ.Height;
										float xxw = 0;
										float yyw = 0;
									
									
										float hspace = 1;
										float vspace = 1;
										
										//if (!InHitTest) graphics.DrawString(zz.iiWidth.ToString(), pTextFont, LevelTBrush, xx1+hspace, yy1+vspace, stringFormatNear);
										
										//graphics.SmoothingMode = SmoothingMode.None;
										
										
										
										xxw = xxw + hspace*2;
										yyw = yyw + vspace*2;
										
										
										//if (!InHitTest) graphics.DrawRectangle(BoxPen,xx1,yy1,xxw,yyw);
										//if (!InHitTest) graphics.FillRectangle(BoxBrush,xx1,yy1,xxw,yyw);
								
								}
							
						
								
							//Print(StartT);
							
							//Print(EndT);
							
							
						
							DisplayZone = false;
						
						
						
							
					
					
					
			}
				
			
			
			
			//Print("onrender test 3");
						
			
			
			
			foreach (ObjectToDraw ooo in TextObjectsToDraw)
			{
						
				
				bool DrawAll = pDisplayLevelsEnabled && pLabelsEnabled;		
				string text = ooo.Text;
				
				// Hover: check if level is in hovered zone, only show top/bottom of zone
				bool IsHoveredLevel = HoveredZoneLevelNames.Contains(text) && ooo.StartBar == LastHoverLevelStartBar;

				// For zone levels, only highlight top and bottom labels on hover
				if (IsHoveredLevel && LevelsInZonesByBar.ContainsKey(ooo.StartBar) && LevelsInZonesByBar[ooo.StartBar].ContainsKey(text))
				{
					bool isTop = LevelsAtTopOfZonesByBar.ContainsKey(ooo.StartBar) && LevelsAtTopOfZonesByBar[ooo.StartBar].ContainsKey(text);
					bool isBot = LevelsAtBottomOfZonesByBar.ContainsKey(ooo.StartBar) && LevelsAtBottomOfZonesByBar[ooo.StartBar].ContainsKey(text);
					if (!isTop && !isBot)
						IsHoveredLevel = false;
				}
				
					
			
			
						
			
			
				if (IsHoveredLevel)
					TextObjectsToDrawHovered.Add(ooo);
						
				
				bool IsInZone = false;
				

				if (LevelsInZonesByBar.ContainsKey(ooo.StartBar) && LevelsInZonesByBar[ooo.StartBar].ContainsKey(text))
					{
							IsInZone = true;
					}				
					
					
				bool ZoneLevel = pDisplayZonesEnabled && IsInZone && pLabelsEnabled;
				bool LabelsOffHovered = !pLabelsEnabled	&& IsHoveredLevel;

				if (DrawAll || LabelsOffHovered || ZoneLevel)
				{


					// Use cached label TextFormat
					SharpDX.DirectWrite.TextFormat textFormat2 = cachedLabelTextFormat;

					if (!isHitTest)
					{
						float savedBrushOpacity = ooo.Brush.Opacity;

						// Chart background wash
						ChartBackgroundBrushDX.Opacity = 0.50f;
						SharpDX.Direct2D1.RoundedRectangle bgRect = new SharpDX.Direct2D1.RoundedRectangle()
						{
							Rect = ExpandRect(ooo.Rectangle, -3, -1),
							RadiusX = 3,
							RadiusY = 3
						};
						RenderTarget.FillRoundedRectangle(bgRect, ChartBackgroundBrushDX);
						ChartBackgroundBrushDX.Opacity = 1.0f;

						// Color glow on top - same size as background so no outline edge visible
						ooo.Brush.Opacity = savedBrushOpacity * 0.10f;
						RenderTarget.FillRoundedRectangle(bgRect, ooo.Brush);

						ooo.Brush.Opacity = savedBrushOpacity;
					}

					// Draw the text (dimmed if not hovered while something else is)
					if (!isHitTest)
					{
						RenderTarget.DrawText(ooo.Text, textFormat2, ooo.Rectangle, ooo.Brush);
					}	
					
					//AllRefreshRects.Add(ooo.Rectangle);
					
					//AllLabelRectangles.Add(new SharpDX.RectangleF(rect222.X, rect222.Y, rect222.Width, rect222.Height));
				
				}
				
				
				
			}
			
			
	
										
	//Print("onrender test 4");
			
			
			// DRAW DEVELPOING  LEVELS
			
			bool noooo = false;
			
			//if (noooo)
			
						
						if (pCDEnabled)
			if (pActive2)
			for (int seriesCount = 0; seriesCount < Values.Length; seriesCount++)
			{
				if (seriesCount >= 11)
					continue;
				
				// don't draw 20  and 21 plots.
				
//				if (seriesCount <= 3 && !pShowToday)
//					continue;
				
//				if (seriesCount > 3 && !pShowYesterday)
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
				
				
				string text = plot.Name;

					if (text.Contains("Globex"))
					{
						ThisStroke = pGlobexStroke;

						if (!pDisplayGlobexEnabled)
							continue;
					}

					else if (text.Contains("Low") || text.Contains("High"))
					{
						ThisStroke = pLevel2Stroke;
						ThisStroke.Opacity = pLevel2Stroke.Opacity;

						if (!pDisplayOHLCEnabled)
							continue;

					}

									else if (text.Contains("H ") || text.Contains("L ") )
									{
										ThisStroke = pLevel4Stroke;
										ThisStroke.Opacity = pLevel4Stroke.Opacity;
										
										
										if (!pDisplayFibEnabled)
											continue;
									}
									
								
										
									else if (text.Contains("Middle"))
									{
										ThisStroke = pLevel1Stroke;
										ThisStroke.Opacity = pLevel1Stroke.Opacity;
										
										if (!pDisplayFibEnabled)
											continue;
									}
									
											textBrushDx = ThisStroke.BrushDX;
											
											textBrushDx.Opacity = pMainTextOpacity/100f;									
				
				
				// Use the Stroke's cached DX brush for glow - save opacity to restore after loop
				smallAreaBrushDx = ThisStroke.BrushDX;
				float savedCDOpacity = smallAreaBrushDx.Opacity; 
				


				int FB = ChartBars.FromIndex;
				int LB = ChartBars.ToIndex;

				// Find the current value at the visible left edge by scanning back
				// This ensures lines extend to the right even when their start is off-screen
				double runningVal = 0;
				for (int scanBack = FB; scanBack >= Math.Max(0, FB - 300); scanBack--)
				{
					double sv = Values[seriesCount].GetValueAt(scanBack);
					if (sv != 0) { runningVal = sv; break; }
				}

				float finalCDWidth = ThisStroke.Width;
				if (pLevelWidthOverride != 0) finalCDWidth = pLevelWidthOverride;

				// Iterate only visible bars, left to right
				for (int idx = FB; idx <= LB; idx++)
				{
					this.val = Values[seriesCount].GetValueAt(idx);

					// Use running value if current bar is 0 but we had a value before
					double drawVal = this.val != 0 ? this.val : runningVal;
					if (drawVal == 0) continue;

					// Update running value
					if (this.val != 0) runningVal = this.val;

					y = chartScale.GetYByValue(drawVal);
					startX = chartControl.GetXByBarIndex(ChartBars, idx);
					endX = (idx == LB) ? ChartPanel.W : chartControl.GetXByBarIndex(ChartBars, idx + 1);

					// Draw line segment for this bar
					if (!isHitTest)
					{
						smallAreaBrushDx.Opacity = 60/100F;
						RenderTarget.DrawLine(
							new SharpDX.Vector2((float)startX, (float)y),
							new SharpDX.Vector2((float)endX, (float)y),
							smallAreaBrushDx, finalCDWidth, ThisStroke.StrokeStyle);

						// Glow
						if (pMainGlowOpacity > 0)
						{
							smallAreaBrushDx.Opacity = pMainGlowOpacity/100F;
							SharpDX.RectangleF glowRect = new SharpDX.RectangleF(
								(float)startX, (float)y - pMainGlowWidth - 1,
								(float)(endX - startX), pMainGlowWidth * 2 + 1);
							RenderTarget.FillRectangle(glowRect, smallAreaBrushDx);
						}
					}

					// Draw label only at the right edge of chart to avoid overlap
					if (idx == LB && drawVal != 0 && pLabelsEnabled && !isHitTest)
					{
						SharpDX.Size2F plotLabelSize2 = MeasureText(plot.Name, textFormat);
						double newy = y - plotLabelSize2.Height - 3;
						float labelX = (float)(ChartPanel.W - plotLabelSize2.Width - 4 - pRightPX);
						SharpDX.RectangleF plotLabelRect2 = new SharpDX.RectangleF(
							labelX, (float)newy, plotLabelSize2.Width + 4, plotLabelSize2.Height);

						// Use overlap resolution to prevent stacking
						plotLabelRect2 = ResolveOverlap(plotLabelRect2, 4);
						AllLabelRectangles.Add(plotLabelRect2);

						RenderTarget.DrawText(plot.Name, textFormat, plotLabelRect2, textBrushDx);
					}
				}
					



				// Draw pivot text
			// Restore shared brush opacity to prevent bleed into main level rendering
				smallAreaBrushDx.Opacity = savedCDOpacity;
				
				

			}
				
			
			// END DRAW LEVELS

			// Restore smallAreaBrushDx to cached version after series loop
			smallAreaBrushDx = cachedSmallAreaBrushDx;

			// Update price markers at the visible right-edge bar (also handles toggle on/off)
			if (PaintPriceMarkers && pDisplayLevelsEnabled && !isHitTest && CurrentBarsBack >= 0 && CurrentBarsBack < ChartBars.Count)
			{
				TriggerCustomEvent(o =>
				{
					RebuildDimBrushes();
					int plotIdx = StartFlagPlotN;
					int barsBack = CurrentBarsBack;

					// Levels
					if (AllDailyLevels.Count > 0)
					{
						foreach (KeyValuePair<string, double> lvl in AllDailyLevels)
						{
							if (plotIdx > EndFlagPlotN) break;
							if (lvl.Value == 0) continue;
							if (!IsLevelTypeVisible(lvl.Key)) continue;
							Values[plotIdx - 1][barsBack] = lvl.Value;
							PlotBrushes[plotIdx - 1][barsBack] = GetMarkerBrush(lvl.Key);
							plotIdx++;
						}
					}

					// Zone boundaries
					if (pDisplayZonesEnabled && ZonesByBarStart.Count > 0)
					{
						int lastKey = ZonesByBarStart.Keys[ZonesByBarStart.Count - 1];
						foreach (ZoneB zz in ZonesByBarStart[lastKey])
						{
							if (plotIdx + 1 > EndFlagPlotN) break;
							if (zz.iiHigh == zz.iiLow) continue;
							Values[plotIdx - 1][barsBack] = zz.iiHigh;
							PlotBrushes[plotIdx - 1][barsBack] = dimBrushZone;
							plotIdx++;
							Values[plotIdx - 1][barsBack] = zz.iiLow;
							PlotBrushes[plotIdx - 1][barsBack] = dimBrushZone;
							plotIdx++;
						}
					}

					for (int i = plotIdx; i <= EndFlagPlotN; i++)
						Values[i - 1].Reset(barsBack);

				}, 0, null);
			}
			// If levels are hidden, clear all price markers
			else if (PaintPriceMarkers && !pDisplayLevelsEnabled && !isHitTest && CurrentBarsBack >= 0 && CurrentBarsBack < ChartBars.Count)
			{
				TriggerCustomEvent(o =>
				{
					int barsBack = CurrentBarsBack;
					for (int i = StartFlagPlotN; i <= EndFlagPlotN; i++)
						Values[i - 1].Reset(barsBack);
				}, 0, null);
			}

			// Pop chart-content clip before rendering panel
			if (panelClipActive) { RenderTarget.PopAxisAlignedClip(); panelClipActive = false; }

			// BUTTONS — Accordion panel with shared coordinator

			if (!isHitTest && pButtonsEnabled && cachedButtonBrushDX != null && cachedHeaderTextFormat != null)
			{
				SharpDX.Direct2D1.Brush buttonBrushDX = cachedButtonBrushDX;
				SharpDX.Direct2D1.Brush buttonHBrushDX = cachedButtonHBrushDX;
				SharpDX.Direct2D1.Brush buttonFOFFBrushDX = cachedButtonFOFFBrushDX;
				SharpDX.Direct2D1.Brush buttonFONBrushDX = cachedButtonFONBrushDX;

				bool anyMenuOpen = IsAnyPanelMenuOpen();
				bool expanded = IsPanelExpanded();
				bool showButtons = anyMenuOpen || InMenu;

				// Default B2 detection zone; widened below if buttons are measured
				B2 = new SharpDX.RectangleF(0, 0, 80, 10000);

				// Erase previous panel area when panel becomes hidden
				if (!showButtons && panelWasVisible && lastPanelRect.Width > 0)
				{
					ChartControl.InvalidateVisual();
					panelWasVisible = false;
					lastPanelRect = new SharpDX.RectangleF(0, 0, 0, 0);
				}

				if (showButtons)
				{
					// Clip panel rendering to viewport
					SharpDX.RectangleF panelClipRect = new SharpDX.RectangleF(
						0, PANEL_TOP_MARGIN - 10, 250, ChartPanel.H - PANEL_TOP_MARGIN + 20);
					RenderTarget.PushAxisAlignedClip(panelClipRect, SharpDX.Direct2D1.AntialiasMode.PerPrimitive);

					float CY = GetPanelYOffset();
					float startCY = CY;

					// Measure header
					SharpDX.Size2F headerSize = MeasureText(PANEL_HEADER_TEXT, cachedHeaderTextFormat);
					float headerH = Math.Max(22f, headerSize.Height + 9);
					float headerW = headerSize.Width + 28;

					// Single-pass measurement for buttons (width + height)
					float maxBtnW = headerW;
					float totalBtnH = 0;
					SharpDX.DirectWrite.TextFormat ButtonText = cachedButtonTextFormat;
					if (expanded || panelAnimProgress > 0.01f)
					{
						bool measHasDrawn = false;
						bool measLastBlank = false;
						foreach (KeyValuePair<double, ButtonZ> tb in AllButtonZ)
						{
							if (tb.Value.Text == "") { if (!measHasDrawn || measLastBlank) continue; measLastBlank = true; totalBtnH += 9; continue; }
							measHasDrawn = true;
							measLastBlank = false;
							SharpDX.Size2F bs = MeasureText(tb.Value.Text, ButtonText);
							totalBtnH += bs.Height + 2 + space;
							float bw = bs.Width + 14;
							if (bw > maxBtnW) maxBtnW = bw;
						}
					}
					float panelW = maxBtnW + 16;
					float unifiedW = Math.Max(panelW, GetMaxPanelWidth());

					// Update B2 zone to match actual panel width (prevents menu vanishing when hovering buttons)
					B2 = new SharpDX.RectangleF(0, 0, Math.Max(80, unifiedW + 16), 10000);

					// Draw header tab
					headerW = headerSize.Width + 32;
					headerRect = new SharpDX.RectangleF(0, CY, headerW, headerH);
					// Animate expand/collapse — must run before backdrop so panelAnimProgress is current
					bool pPanelAnimEnabled = false;
					if (pPanelAnimEnabled)
					{
						float animTarget = expanded ? 1f : 0f;
						if (panelAnimTargetValue != animTarget)
						{
							panelAnimStartTime = DateTime.UtcNow;
							panelAnimStartValue = panelAnimProgress;
							panelAnimTargetValue = animTarget;
						}
						if (panelAnimProgress != animTarget)
						{
							float elapsed = (float)(DateTime.UtcNow - panelAnimStartTime).TotalMilliseconds;
							float t = Math.Min(1f, elapsed / PANEL_ANIM_DURATION_MS);
							float eased = 1f - (1f - t) * (1f - t) * (1f - t);
							panelAnimProgress = panelAnimStartValue + (animTarget - panelAnimStartValue) * eased;
							if (t >= 1f)
								panelAnimProgress = animTarget;
							else
								ChartControl.InvalidateVisual();
						}
					}
					else
					{
						panelAnimProgress = expanded ? 1f : 0f;
					}

					// Header stays highlighted when mouse is anywhere in the expanded panel
					bool headerHovered = MouseIn(headerRect, 1, 1)
						|| (panelAnimProgress > 0.5f && MouseIn(new SharpDX.RectangleF(0, CY, unifiedW + 16, headerH + totalBtnH + 20), 1, 1));

					// Unified backdrop — drawn BEFORE header so header renders on top
					{
						float collapsedW = Math.Max(headerW + 10, unifiedW);
						float collapsedH = headerH + PANEL_GROUP_GAP + 6;
						float expandedW = Math.Max(unifiedW, headerW + 10);
						float expandedH = headerH + space + 5 + totalBtnH + PANEL_GROUP_GAP + 14;
						float bdW = collapsedW + (expandedW - collapsedW) * panelAnimProgress;
						float bdH = collapsedH + (expandedH - collapsedH) * panelAnimProgress;
						SharpDX.RectangleF bdRect = new SharpDX.RectangleF(0, CY - 2, bdW, bdH);
						lastPanelRect = new SharpDX.RectangleF(bdRect.Left - 1, bdRect.Top - 10, bdRect.Width + 18, bdRect.Height + 18);
						panelWasVisible = true;
						cachedPanelBackdropBrushDX.Opacity = 0.85f;
						RenderTarget.FillRectangle(bdRect, cachedPanelBackdropBrushDX);
						float savedBdOp2 = cachedPanelBackdropBrushDX.Opacity;
						// Fade right edge
						for (int fade = 0; fade < 8; fade++)
						{
							cachedPanelBackdropBrushDX.Opacity = savedBdOp2 * (7 - fade) / 7f;
							RenderTarget.FillRectangle(new SharpDX.RectangleF(bdRect.Right + fade, bdRect.Top, 1, bdRect.Height), cachedPanelBackdropBrushDX);
						}
						// Fade top edge
						for (int fade = 0; fade < 8; fade++)
						{
							cachedPanelBackdropBrushDX.Opacity = savedBdOp2 * (7 - fade) / 7f;
							RenderTarget.FillRectangle(new SharpDX.RectangleF(bdRect.Left, bdRect.Top - 8 + fade, bdRect.Width + 8, 1), cachedPanelBackdropBrushDX);
						}
						// Fade bottom edge
						for (int fade = 0; fade < 8; fade++)
						{
							cachedPanelBackdropBrushDX.Opacity = savedBdOp2 * (7 - fade) / 7f;
							RenderTarget.FillRectangle(new SharpDX.RectangleF(bdRect.Left, bdRect.Bottom + fade, bdRect.Width + 8, 1), cachedPanelBackdropBrushDX);
						}
						cachedPanelBackdropBrushDX.Opacity = savedBdOp2;

							// Drag highlight overlay
							if (draggingPanelId == PANEL_ID)
							{
								cachedHoverGlowBrushDX.Opacity = 0.06f;
								RenderTarget.FillRectangle(bdRect, cachedHoverGlowBrushDX);
								cachedHoverGlowBrushDX.Opacity = 0.08f;
							}
					}

					FillHeaderButton(headerRect, headerHovered ? cachedHeaderBgHoverBrushDX : cachedHeaderBgBrushDX);

					// Erase rounded-rect overflow below header — use backdrop brush when expanded to avoid punching a hole
					SharpDX.Direct2D1.Brush headerEraseBrush = expanded ? cachedPanelBackdropBrushDX : ChartBackgroundBrushDX;
					if (expanded) cachedPanelBackdropBrushDX.Opacity = 0.85f;
					RenderTarget.FillRectangle(new SharpDX.RectangleF(
						headerRect.Left, headerRect.Bottom, headerRect.Width + 10, chartButtonRadius + 4), headerEraseBrush);

					// Bottom strip — same background-then-glow in both states
					SharpDX.RectangleF bottomStrip = new SharpDX.RectangleF(
						headerRect.Left, headerRect.Bottom - headerBottomStripH, headerRect.Width, headerBottomStripH);
					RenderTarget.FillRectangle(bottomStrip, headerEraseBrush);
					cachedHoverGlowBrushDX.Opacity = headerHovered ? 0.25f : 0.14f;
					RenderTarget.FillRectangle(bottomStrip, cachedHoverGlowBrushDX);
					cachedHoverGlowBrushDX.Opacity = 0.08f;

					// Crisp 1px separator below header
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

					// Draw expand/collapse triangle arrow
					float triSize = 8f;
					float triX = headerRect.X + 8;
					float triCY = headerRect.Y + headerRect.Height / 2f - 2f;
					var savedAA2 = RenderTarget.AntialiasMode;
					RenderTarget.AntialiasMode = SharpDX.Direct2D1.AntialiasMode.PerPrimitive;
					if (expanded)
					{
						// Down arrow ▾
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
						// Right arrow ▸
						float triLeft = triX + 1f;
						float triRight = triLeft + triSize * 0.5f;
						int slices = 10;
						float sliceW = (triRight - triLeft) / slices;
						for (int s = 0; s < slices; s++)
						{
							float frac = (s + 0.5f) / slices;
							float barH = triSize * (1f - frac);
							float barY = triCY - barH / 2f;
							float barX = triLeft + s * sliceW;
							RenderTarget.FillRectangle(new SharpDX.RectangleF(barX, barY, sliceW + 0.5f, barH), cachedHeaderTextBrushDX);
						}
					}
					RenderTarget.AntialiasMode = savedAA2;

					// Header text (after arrow)
					SharpDX.RectangleF headerTextRect = new SharpDX.RectangleF(
						headerRect.X + 20, headerRect.Y, headerRect.Width - 20, headerRect.Height - 3);
					RenderTarget.DrawText(PANEL_HEADER_TEXT, cachedHeaderTextFormat, headerTextRect, cachedHeaderTextBrushDX);

					CY += headerH + space + 5;

					// Draw buttons when animating or expanded
					if (panelAnimProgress > 0.01f)
					{

						// Pre-pass: find hovered button index before drawing
						currentButtonHover = -1;
						int preIdx = 0;
						foreach (KeyValuePair<double, ButtonZ> thisbutton in AllButtonZ)
						{
							if (thisbutton.Value.Text != "" && MouseIn(thisbutton.Value.Rect, 2, 2))
							{ currentButtonHover = preIdx; break; }
							preIdx++;
						}

						// Batch AA mode for all button rendering
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
								// Section divider line
								float divY = (float)Math.Floor(CY + (9f - space) / 2f) + 0.5f;
								buttonBrushDX.Opacity = 0.18f;
								var savedDivAA = RenderTarget.AntialiasMode;
								RenderTarget.AntialiasMode = SharpDX.Direct2D1.AntialiasMode.Aliased;
								RenderTarget.DrawLine(new SharpDX.Vector2(0, divY), new SharpDX.Vector2(space + maxBtnW, divY), buttonBrushDX, 1f);
								RenderTarget.AntialiasMode = savedDivAA;
								buttonBrushDX.Opacity = 1.0f;
								CY += 9;
								buttonIndex++;
								continue;
							}
							hasDrawnButton = true;
							lastWasBlank = false;

							SharpDX.Size2F btnSize = MeasureText(btext, ButtonText);
							float FinalH = btnSize.Height + 3;
							float FinalW = btnSize.Width + 14;

							thisbutton.Value.Rect = new SharpDX.RectangleF(space, CY, FinalW, FinalH);

							bool btnHovered = (buttonIndex == currentButtonHover);

							// Inline rounded rect draws (AA mode already set for batch)
							var rrect = new SharpDX.Direct2D1.RoundedRectangle() { Rect = thisbutton.Value.Rect, RadiusX = chartButtonRadius, RadiusY = chartButtonRadius };

							// Background fill — constant opacity for all buttons
							buttonFOFFBrushDX.Opacity = 0.5f;
							RenderTarget.FillRoundedRectangle(rrect, buttonFOFFBrushDX);

							// State fill (on = colored, off = neutral)
							if (!thisbutton.Value.Switch)
							{
								buttonFONBrushDX.Opacity = 0.35f;
								RenderTarget.FillRoundedRectangle(rrect, buttonFONBrushDX);
							}

							// Hover: bright border + subtle fill
							if (btnHovered)
							{
								buttonHBrushDX.Opacity = 0.12f;
								RenderTarget.FillRoundedRectangle(rrect, buttonHBrushDX);
								buttonBrushDX.Opacity = 0.85f;
								RenderTarget.DrawRoundedRectangle(rrect, buttonBrushDX, 1.5f);
							}
							else
							{
								buttonBrushDX.Opacity = 0.2f;
								RenderTarget.DrawRoundedRectangle(rrect, buttonBrushDX, 1f);
							}

							// Text — left-aligned, brighter when ON
							SharpDX.RectangleF textRect = new SharpDX.RectangleF(
								thisbutton.Value.Rect.X + 7, thisbutton.Value.Rect.Y,
								thisbutton.Value.Rect.Width - 7, thisbutton.Value.Rect.Height);
							float textOpacity = thisbutton.Value.Switch ? 0.5f : 0.7f;
							buttonBrushDX.Opacity = textOpacity;
							ButtonText.TextAlignment = SharpDX.DirectWrite.TextAlignment.Leading;
							RenderTarget.DrawText(btext, ButtonText, textRect, buttonBrushDX);
							ButtonText.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;

							CY += FinalH + space;
							buttonIndex++;
						}

						// Restore AA mode and brush opacities
						RenderTarget.AntialiasMode = savedButtonAA;
						buttonBrushDX.Opacity = 1.0f;
						buttonFOFFBrushDX.Opacity = 1.0f;
						buttonFONBrushDX.Opacity = 1.0f;
						buttonHBrushDX.Opacity = 0.4f;
						cachedHoverGlowBrushDX.Opacity = 0.08f;
					}

					float fullH = CY - startCY;
					float headerOnlyH = headerH + space + 5;
					float animatedH = headerOnlyH + (fullH - headerOnlyH) * panelAnimProgress;
					if (panelAnimProgress > 0.5f) animatedH += 3f;
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

					// Scroll indicators (drawn outside clip)
					{
						var scrollReg = GetScrollRegistry();
						float[] scrollState;
						if (scrollReg.TryGetValue(panelHash, out scrollState) && scrollState[SC_TOTALH] > scrollState[SC_VIEWH])
						{
							float indicatorX = 40f;
							float maxScroll = Math.Max(0, scrollState[SC_TOTALH] - scrollState[SC_VIEWH] + 20f);

							// Up arrow when scrolled down
							if (scrollState[SC_OFFSET] > 1f)
							{
								float opacity = Math.Min(0.6f, scrollState[SC_OFFSET] / 50f);
								cachedHeaderTextBrushDX.Opacity = opacity;
								float cy = PANEL_TOP_MARGIN - 2f;
								float sz = 6f;
								int slices = 6;
								for (int s = 0; s < slices; s++)
								{
									float frac = (s + 0.5f) / slices;
									float barW = sz * frac;
									RenderTarget.FillRectangle(new SharpDX.RectangleF(
										indicatorX - barW / 2f, cy + s * 1.2f, barW, 1.2f), cachedHeaderTextBrushDX);
								}
							}

							// Down arrow when more content below
							if (scrollState[SC_OFFSET] < maxScroll - 1f)
							{
								float opacity = Math.Min(0.6f, (maxScroll - scrollState[SC_OFFSET]) / 50f);
								cachedHeaderTextBrushDX.Opacity = opacity;
								float cy = ChartPanel.H - 10f;
								float sz = 6f;
								int slices = 6;
								for (int s = 0; s < slices; s++)
								{
									float frac = (s + 0.5f) / slices;
									float barW = sz * frac;
									RenderTarget.FillRectangle(new SharpDX.RectangleF(
										indicatorX - barW / 2f, cy - s * 1.2f, barW, 1.2f), cachedHeaderTextBrushDX);
								}
							}
							cachedHeaderTextBrushDX.Opacity = 0.7f;
						}
					}
				}
				else
				{
					// Buttons not shown — reset button rects off-screen
					ReportPanelHeight(0);
					UpdateScrollBounds();
					foreach (KeyValuePair<double, ButtonZ> thisbutton in AllButtonZ)
						thisbutton.Value.Rect = new SharpDX.RectangleF(-100, -100, 0, 0);
				}
			}

		
				
				
//			if (LevelsHoveredByBar.ContainsKey	
				
				
			//	Print("LastHoverLevelStartBar: " + LastHoverLevelStartBar);
//			Print(LevelsHoveredByBar[LastHoverLevelStartBar].Count);
				
				
		}
	
		

		
		private string PriceStringF (double price)
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
				  
		
		
		
		public override string FormatPriceMarker(double price)
		{
			string priceMarker = PriceStringF(price);
			return priceMarker;
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



//        private bool p2PCEnabled = true;
//        [Display(ResourceType = typeof(Custom.Resource), Name = "2PC", Description = "", GroupName = "Plots", Order = 7)]
//        public bool PCEnabled2
//        {
//            get { return p2PCEnabled; }
//            set { p2PCEnabled = value; }
//        }	
		
				
//		[Display(ResourceType = typeof(Custom.Resource), Name = "ShowClose", GroupName = "NinjaScriptParameters", Order = 0)]
//		public bool ShowClose
//		{ get; set; }

//		[Display(ResourceType = typeof(Custom.Resource), Name = "ShowHigh", GroupName = "NinjaScriptParameters", Order = 1)]
//		public bool ShowHigh
//		{ get; set; }

//		[Display(ResourceType = typeof(Custom.Resource), Name = "ShowLow", GroupName = "NinjaScriptParameters", Order = 2)]
//		public bool ShowLow
//		{ get; set; }

//		[Display(ResourceType = typeof(Custom.Resource), Name = "ShowOpen", GroupName = "NinjaScriptParameters", Order = 3)]
//		public bool ShowOpen
//		{ get; set; }
		
	

		
		
        private int pThisBarPeriod1 = 1;
//        [Range(1, int.MaxValue)]
//        [Display(ResourceType = typeof(Custom.Resource), Name = "Minute Bars", GroupName = "Minute Data 1", Order = 32)]
//        public int ThisBarPeriod1
//        {
//            get { return pThisBarPeriod1; }
//            set { pThisBarPeriod1 = value; }
//        }		
		
		
		
		private BarsPeriodType AcceptableBasePeriodType1 = BarsPeriodType.Minute;

		
		
		
		
		
		

        private Stroke pLevel6Stroke = new Stroke(Brushes.DarkTurquoise, DashStyleHelper.Solid, 1);
        [Display(ResourceType = typeof(Custom.Resource), Name = "Open", Description = "", GroupName = "Levels Display", Order = 1)]
        public Stroke Level6Stroke
        {
            get { return pLevel6Stroke; }
            set { pLevel6Stroke = value; }
        }				
        private Stroke pLevel2Stroke = new Stroke(Brushes.Goldenrod, DashStyleHelper.Solid, 1);
        [Display(ResourceType = typeof(Custom.Resource), Name = "High Low", Description = "", GroupName = "Levels Display", Order = 2)]
        public Stroke Level2Stroke
        {
            get { return pLevel2Stroke; }
            set { pLevel2Stroke = value; }
        }
		
        private Stroke pLevel3Stroke = new Stroke(Brushes.DarkTurquoise, DashStyleHelper.Solid, 1);
        [Display(ResourceType = typeof(Custom.Resource), Name = "Close", Description = "", GroupName = "Levels Display", Order = 3)]
        public Stroke Level3Stroke
        {
            get { return pLevel3Stroke; }
            set { pLevel3Stroke = value; }
        }
		
	     private Stroke pLevel1Stroke = new Stroke(Brushes.HotPink, DashStyleHelper.Solid, 1);
        [Display(ResourceType = typeof(Custom.Resource), Name = "Middle", Description = "", GroupName = "Levels Display", Order = 4)]
        public Stroke Level1Stroke
        {
            get { return pLevel1Stroke; }
            set { pLevel1Stroke = value; }
        }		
		
        private Stroke pLevel4Stroke = new Stroke(Brushes.SteelBlue, DashStyleHelper.Solid, 1);
        [Display(ResourceType = typeof(Custom.Resource), Name = "Fibonacci", Description = "", GroupName = "Levels Display", Order = 5)]
        public Stroke Level4Stroke
        {
            get { return pLevel4Stroke; }
            set { pLevel4Stroke = value; }
        }
		
        private Stroke pLevel8Stroke = new Stroke(Brushes.MediumPurple, DashStyleHelper.Solid, 1);
        [Display(ResourceType = typeof(Custom.Resource), Name = "Whole Numbers", Description = "", GroupName = "Levels Display", Order = 6)]
        public Stroke Level8Stroke
        {
            get { return pLevel8Stroke; }
            set { pLevel8Stroke = value; }
        }

        private Stroke pGlobexStroke = new Stroke(Brushes.DodgerBlue, DashStyleHelper.Dash, 1);
        [Display(ResourceType = typeof(Custom.Resource), Name = "Globex", Description = "Globex overnight High/Low line style", GroupName = "Levels Display", Order = 7)]
        public Stroke GlobexStroke
        {
            get { return pGlobexStroke; }
            set { pGlobexStroke = value; }
        }

        private Stroke pLevel5Stroke = new Stroke(Brushes.Coral, DashStyleHelper.Solid, 1);
        [Display(ResourceType = typeof(Custom.Resource), Name = "Pivots", Description = "", GroupName = "Levels Display", Order = 8)]
        public Stroke Level5Stroke
        {
            get { return pLevel5Stroke; }
            set { pLevel5Stroke = value; }
        }
		

		
       private Stroke pLevel7Stroke = new Stroke(Brushes.WhiteSmoke, DashStyleHelper.Solid, 1);
        [Display(ResourceType = typeof(Custom.Resource), Name = "Highlight", Description = "", GroupName = "Levels Display", Order = 10)]
        public Stroke Level7Stroke
        {
            get { return pLevel7Stroke; }
            set { pLevel7Stroke = value; }
        }		
		
		
		
        private bool pActive2 = true;
//        [Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", Description = "", GroupName = "Levels Display", Order = -10)]
//        public bool Active2
//        {
//            get { return pActive2; }
//            set { pActive2 = value; }
//        }	
		
		
        private bool pDisplayLevelsEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", Description = "", GroupName = "Display", Order = -10)]
        public bool DisplayLevelsEnabled
        {
            get { return pDisplayLevelsEnabled; }
            set { pDisplayLevelsEnabled = value; }
        }	
		
        private bool pDisplayOHLCEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", Description = "", GroupName = "Display", Order = -10)]
        public bool DisplayOHLCEnabled
        {
            get { return pDisplayOHLCEnabled; }
            set { pDisplayOHLCEnabled = value; }
        }	
		
        private bool pDisplayFibEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", Description = "", GroupName = "Display", Order = -10)]
        public bool DisplayFibEnabled
        {
            get { return pDisplayFibEnabled; }
            set { pDisplayFibEnabled = value; }
        }	
		

				
        private bool pDisplayPivotsEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", Description = "", GroupName = "Display", Order = -10)]
        public bool DisplayPivotsEnabled
        {
            get { return pDisplayPivotsEnabled; }
            set { pDisplayPivotsEnabled = value; }
        }	
		
		
        private bool pDisplayWholeNumbersEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", Description = "", GroupName = "Display", Order = -10)]
        public bool DisplayWholeNumbersEnabled
        {
            get { return pDisplayWholeNumbersEnabled; }
            set { pDisplayWholeNumbersEnabled = value; }
        }	
		
		
        private bool pDisplayHistoryEnabled = false;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", Description = "show / hide levels from beyond yesterday, unless they form a zone with another level.", GroupName = "Display", Order = -10)]
        public bool DisplayHistoryEnabled
        {
            get { return pDisplayHistoryEnabled; }
            set { pDisplayHistoryEnabled = value; }
        }		
   	
        private bool pDisplayZonesEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", Description = "", GroupName = "Display", Order = -10)]
        public bool DisplayZonesEnabled
        {
            get { return pDisplayZonesEnabled; }
            set { pDisplayZonesEnabled = value; }
        }	
		
		private int pLevelWidthOverride = 0;
			[Range(0, 100)]
			[Display(ResourceType = typeof(Custom.Resource), GroupName = "Levels Display", Name = "Line Width (Override)", Description = "set to a number to override widths for all levels", Order = 43)]
			public int LevelWidthOverride
	        {
				get { return pLevelWidthOverride; }
				set { pLevelWidthOverride = value; }
	        }	

		private int pMainGlowWidth = 3;
			[Range(0, 100)]
			[Display(ResourceType = typeof(Custom.Resource), GroupName = "Levels Display", Name = "Glow Width", Description = "", Order = 45)]
			public int MainGlowWidth
	        {
				get { return pMainGlowWidth; }
				set { pMainGlowWidth = value; }
	        }	
			
			
		private int pMainGlowOpacity = 15;
			[Range(0, 100)]
			[Display(ResourceType = typeof(Custom.Resource), GroupName = "Levels Display", Name = "Glow Opacity (%)", Description = "", Order = 47)]
			public int MainGlowOpacity
	        {
				get { return pMainGlowOpacity; }
				set { pMainGlowOpacity = value; }
	        }			
			
			
//			private int pHoverTextOpacity = 100;
//			[Range(0, 100)]
//			[Display(ResourceType = typeof(Custom.Resource), GroupName = "Levels Display", Name = "Label Hover Opacity (%)", Description = "", Order = 38)]
//			public int HoverTextOpacity
//	        {
//				get { return pHoverTextOpacity; }
//				set { pHoverTextOpacity = value; }
//	        }			

		
			
									
		private bool pCDEnabled = false;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Current Day Enabled", Description = "", GroupName = "Levels Display", Order = 31)]
        public bool CDEnabled
        {
            get { return pCDEnabled; }
            set { pCDEnabled = value; }
        }		
											
									
		private bool pLabelsEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", Description = "", GroupName = "Labels Display", Order = 31)]
        public bool LabelsEnabled
        {
            get { return pLabelsEnabled; }
            set { pLabelsEnabled = value; }
        }		
		
		[Display(ResourceType = typeof(Custom.Resource), Name="Font", Description="", GroupName = "Labels Display", Order = 32)]
		public SimpleFont TextFont
		{ get; set; }	
		
		private int pMainTextOpacity = 70;
			[Range(0, 100)]
			[Display(ResourceType = typeof(Custom.Resource), GroupName = "Labels Display", Name = "Opacity (%)", Description = "", Order = 37)]
			public int MainTextOpacity
	        {
				get { return pMainTextOpacity; }
				set { pMainTextOpacity = value; }
	        }	
			
			
			
		private int pRightPX = 0;
//		[Range(0, 1000)]
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Label X Offset", Description="in pixels.", GroupName = "Levels Display", Order = 33)]
//		public int RightPX
//		{
//			get { return pRightPX; }
//			set { pRightPX= value; }
//		}	
		
		private int pShadowWidth = 3;
//		[Range(0, 100)]
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Shadow Width", Description="in pixels.", GroupName = "Levels Display", Order = 5)]
//		public int ShadowWidth
//		{
//			get { return pShadowWidth; }
//			set { pShadowWidth= value; }
//		}	
		
		private int pRightPX2 = 150;
//		[Range(0, 1000000)]
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Length", Description="for Range High and Range Low", GroupName = "Levels Display", Order = 20)]
//		public int RightPX2
//		{
//			get { return pRightPX2; }
//			set { pRightPX2= value; }
//		}	

		
		
//		private Brush		areaBrush		= Brushes.Blue;
//		private Brush		textBrush		= Brushes.White;
		private Brush		smallAreaBrush	= Brushes.Red;
//		private	int			areaOpacity		= 20;
//		const	float 		fontHeight		= 30f;
		
		

//		[Browsable(false)]
//		public string AreaBrushSerialize
//		{
//			get { return Serialize.BrushToString(AreaBrush); }
//			set { AreaBrush = Serialize.StringToBrush(value); }
//		}

		
		
		
        private bool pShowToday = true;
//        [Display(ResourceType = typeof(Custom.Resource), Name = "Today Enabled", Description = "", GroupName = "Plots", Order = -20)]
//        public bool ShowToday
//        {
//            get { return pShowToday; }
//            set { pShowToday = value; }
//        }		
		
        private bool pShowYesterday = true;
//        [Display(ResourceType = typeof(Custom.Resource), Name = "Yesterday Enabled", Description = "", GroupName = "Plots", Order = -19)]
//        public bool ShowYesterday
//        {
//            get { return pShowYesterday; }
//            set { pShowYesterday = value; }
//        }				
				
		
	private TimeSpan pCustomETime = new TimeSpan(7,30,0);
		private TimeSpan pCustomTime = new TimeSpan(7,30,0);
//		[Description("Enter the start time of the range.")]
//		[Gui.Design.DisplayNameAttribute("\r\rTime Begin")]
//		[GridCategory("\t\t\t\t\t\t\tParameters")]
//		public string TimeBegin
//		{
//			get { return pStartTime.Hours.ToString("0")+":"+pStartTime.Minutes.ToString("00");}
//			set { if(!TimeSpan.TryParse(value, out pStartTime)) pStartTime=new TimeSpan(0,0,0); }
//		}
		
		
				
		private TimeSpan pStartTime2 = new TimeSpan(9,30,0);
		private TimeSpan pEndTime2 = new TimeSpan(16,15,0);
		
		
		private TimeSpan pStartTime = new TimeSpan(9,30,0);
//		[Description("Enter the start time of the range.")]
//		[Gui.Design.DisplayNameAttribute("\r\rTime Begin")]
//		[GridCategory("\t\t\t\t\t\t\tParameters")]
//		public string TimeBegin
//		{
//			get { return pStartTime.Hours.ToString("0")+":"+pStartTime.Minutes.ToString("00");}
//			set { if(!TimeSpan.TryParse(value, out pStartTime)) pStartTime=new TimeSpan(0,0,0); }
//		}

		private TimeSpan pEndTime = new TimeSpan(16,15,0);
//		[Description("Enter the end time of the range.")]
//		[GridCategory("\t\t\t\t\t\t\tParameters")]
//		[Gui.Design.DisplayNameAttribute("Time End")]
//		public string TimeEnd
//		{
//			get { return pEndTime.Hours.ToString("0")+":"+pEndTime.Minutes.ToString("00");}
//			set { if(!TimeSpan.TryParse(value, out pEndTime)) pEndTime=new TimeSpan(0,0,0); }
//		}
		
		private TimeSpan pR1Time = new TimeSpan(10,35,0);
//		[Description("Minutes and Seconds")]
//		[GridCategory("\t\t\t\t\t\t\tParameters")]
//		[Gui.Design.DisplayNameAttribute("\rTime 1")]
//		public string R1Time
//		{
//			get { return pR1Time.Hours.ToString("0")+":"+pR1Time.Minutes.ToString("00"); }
//			set { if(!TimeSpan.TryParse(value, out pR1Time)) pR1Time=new TimeSpan(0,0,0); }
//		}
		

		private TimeSpan pR2Time = new TimeSpan(11,15,0);
//		[Description("Minutes and Seconds")]
//		[GridCategory("\t\t\t\t\t\t\tParameters")]
//		[Gui.Design.DisplayNameAttribute("\rTime 2")]
//		public string R2Time
//		{
//			get { return pR2Time.Hours.ToString("0")+":"+pR2Time.Minutes.ToString("00"); }
//			set { if(!TimeSpan.TryParse(value, out pR2Time)) pR2Time=new TimeSpan(0,0,0); }
//		}
		
		private TimeSpan pR3Time = new TimeSpan(12,20,0);
//		[Description("Minutes and Seconds")]
//		[GridCategory("\t\t\t\t\t\t\tParameters")]
//		[Gui.Design.DisplayNameAttribute("\rTime 3")]
//		public string R3Time
//		{
//			get { return pR3Time.Hours.ToString("0")+":"+pR3Time.Minutes.ToString("00"); }
//			set { if(!TimeSpan.TryParse(value, out pR3Time)) pR3Time=new TimeSpan(0,0,0); }
//		}
		
		private TimeSpan pR4Time = new TimeSpan(13,25,0);
//		[Description("Minutes and Seconds")]
//		[GridCategory("\t\t\t\t\t\t\tParameters")]
//		[Gui.Design.DisplayNameAttribute("\rTime 4")]
//		public string R4Time
//		{
//			get { return pR4Time.Hours.ToString("0")+":"+pR4Time.Minutes.ToString("00"); }
//			set { if(!TimeSpan.TryParse(value, out pR4Time)) pR4Time=new TimeSpan(0,0,0); }
//		}
		
		private TimeSpan pR5Time = new TimeSpan(14,05,0);
//		[Description("Minutes and Seconds")]
//		[GridCategory("\t\t\t\t\t\t\tParameters")]
//		[Gui.Design.DisplayNameAttribute("\rTime 5")]
//		public string R5Time
//		{
//			get { return pR5Time.Hours.ToString("0")+":"+pR5Time.Minutes.ToString("00"); }
//			set { if(!TimeSpan.TryParse(value, out pR5Time)) pR5Time=new TimeSpan(0,0,0); }
//		}
				
		private TimeSpan pR6Time = new TimeSpan(15,10,0);
//		[Description("Minutes and Seconds")]
//		[GridCategory("\t\t\t\t\t\t\tParameters")]
//		[Gui.Design.DisplayNameAttribute("\rTime 6")]
//		public string R6Time
//		{
//			get { return pR6Time.Hours.ToString("0")+":"+pR6Time.Minutes.ToString("00"); }
//			set { if(!TimeSpan.TryParse(value, out pR6Time)) pR6Time=new TimeSpan(0,0,0); }
//		}
		
		private TimeSpan pR7Time = new TimeSpan(16,10,0);
//		[Description("Minutes and Seconds")]
//		[GridCategory("\t\t\t\t\t\t\tParameters")]
//		[Gui.Design.DisplayNameAttribute("\rTime 7")]
//		public string R7Time
//		{
//			get { return pR7Time.Hours.ToString("0")+":"+pR7Time.Minutes.ToString("00"); }
//			set { if(!TimeSpan.TryParse(value, out pR7Time)) pR7Time=new TimeSpan(0,0,0); }
//		}
		
		private TimeSpan pR8Time = new TimeSpan(17,10,0);
//		[Description("* Last Time To Recalculate")]
//		[GridCategory("\t\t\t\t\t\t\tParameters")]
//		[Gui.Design.DisplayNameAttribute("Time End")]
//		public string R8Time
//		{
//			get { return pR8Time.Hours.ToString("0")+":"+pR8Time.Minutes.ToString("00"); }
//			set { if(!TimeSpan.TryParse(value, out pR8Time)) pR8Time=new TimeSpan(0,0,0); }
//		}		
				
		private int pBoxShiftX = 0;
		private int pBoxShiftY = 0;
		private int pPadding = 1;
		
		
		
		
		
        private bool pActive5 = true;
//        [Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", Description = "", GroupName = "Time Zones", Order = -10)]
//        public bool Active5
//        {
//            get { return pActive5; }
//            set { pActive5 = value; }
//        }			
		

		private bool pColorAll = true;
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Color All Panels", Description = "", GroupName = "Time Zones", Order = 1)]
//        public bool ColorAll
//        {
//            get { return pColorAll; }
//            set { pColorAll = value; }
//        }
		
				
		
		
	// SESSION 1 COLOR
		

//		[Browsable(false)]
//		public string Brush01S
//		{
//			get { return Serialize.BrushToString(Brush01); }
//			set { Brush01 = Serialize.StringToBrush(value); }
//		}

		
		// SESSION 2 COLOR
		

//		[Browsable(false)]
//		public string Brush02S
//		{
//			get { return Serialize.BrushToString(Brush02); }
//			set { Brush02 = Serialize.StringToBrush(value); }
//		}

		
		// SESSION 3 COLOR
		
		


		
		    private bool pRoundAll = true;
	        [Display(ResourceType = typeof(Custom.Resource), Name = "Round To Nearest Tick", Description = "", GroupName = "Levels", Order = -10)]
	        public bool RoundAll
	        {
	            get { return pRoundAll; }
	            set { pRoundAll = value; }
	        }
		
		
		
		
		    private bool pAutoModeEnabled = true;
		[RefreshProperties(RefreshProperties.All)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Automatic Daily Sizing", Description = "", GroupName = "Zones", Order = -10)]
        public bool AutoModeEnabled
        {
            get { return pAutoModeEnabled; }
            set { pAutoModeEnabled = value; }
        }
		
		
			private double pZATRMultiplier = 3;
			[Range(0, double.MaxValue)]
			[Display(ResourceType = typeof(Custom.Resource), Name = "Multiplier (%)", Description = "adjust to determine maximum height for synergy zones. increasing number will allow wider zones, decreasing will restrict to skinnier zones.", GroupName = "Zones", Order = 0)]
			public double ZATRMultiplier
			{
				get { return pZATRMultiplier; }
				set { pZATRMultiplier = value; }
			}			
		
			private int pMinimumTicks = 3; // minimum automatic number
			
			
			
			private int	pZWM = 12;
			[Range(1, int.MaxValue)]
			[Display(ResourceType = typeof(Custom.Resource), Name = "Maximum Height (Ticks)", Description = "amount in ticks for maximum height of synergy zone.", GroupName = "Zones", Order = 0)]
			public int ZWM
			{
				get { return pZWM; }
				set { pZWM = value; }
			}		
			
			private int	pTicksSpace = 12;
			[Range(1, int.MaxValue)]
			[Display(ResourceType = typeof(Custom.Resource), Name = "Minimum Separation (Ticks)", Description = "amount in ticks required separation between synergy zones.", GroupName = "Zones", Order = 1)]
			public int TicksSpace
			{
				get { return pTicksSpace; }
				set { pTicksSpace = value; }
			}

			private double pZoneMaxHeightMultiplier = 3.0;
			[Range(1.0, 10.0)]
			[Display(ResourceType = typeof(Custom.Resource), Name = "Maximum Height Multiplier", Description = "Limits how tall a zone can grow. Zone max height = clustering width × this value. Higher = taller zones allowed. 3.0 recommended.", GroupName = "Zones", Order = 2)]
			public double ZoneMaxHeightMultiplier
			{
				get { return pZoneMaxHeightMultiplier; }
				set { pZoneMaxHeightMultiplier = value; }
			}


	
			
			
			
		private System.Windows.Media.Brush	pBrush03 = Brushes.DodgerBlue;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Support Zone Color", Description = "", GroupName = "Zones Display", Order = 6)]
		public Brush Brush03
		{
			get { return pBrush03; }
			set
			{
				pBrush03 = value;
				if (pBrush03 != null)
				{
					if (pBrush03.IsFrozen)
						pBrush03 = pBrush03.Clone();
					pBrush03.Opacity = pOpacity03 / 100d;
					pBrush03.Freeze();
				}
			}
		}

		[Browsable(false)]
		public string Brush03S
		{
			get { return Serialize.BrushToString(Brush03); }
			set { Brush03 = Serialize.StringToBrush(value); }
		}

		private int	pOpacity03 = 30;
		[Range(0, 100)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Support Zone Opacity (%)", Description = "", GroupName = "Zones Display", Order = 7)]
		public int Opacity03
		{
			get { return pOpacity03; }
			set
			{
				pOpacity03 = Math.Max(0, Math.Min(100, value));
				if (pBrush03 != null)
				{
					System.Windows.Media.Brush newBrush		= pBrush03.Clone();
					newBrush.Opacity	= pOpacity03 / 100d;
					newBrush.Freeze();
					pBrush03			= newBrush;
				}
			}
		}
		
//		 SESSION 4 COLOR
		
		private System.Windows.Media.Brush	pBrush04 = Brushes.DarkRed;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Resistance Zone Color", Description = "", GroupName = "Zones Display", Order = 8)]
		public Brush Brush04
		{
			get { return pBrush04; }
			set
			{
				pBrush04 = value;
				if (pBrush04 != null)
				{
					if (pBrush04.IsFrozen)
						pBrush04 = pBrush04.Clone();
					pBrush04.Opacity = pOpacity04 / 100d;
					pBrush04.Freeze();
				}
			}
		}

		[Browsable(false)]
		public string Brush04S
		{
			get { return Serialize.BrushToString(Brush04); }
			set { Brush04 = Serialize.StringToBrush(value); }
		}

		private int	pOpacity04 = 30;
		[Range(0, 100)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Resistance Zone Opacity (%)", Description = "", GroupName = "Zones Display", Order = 9)]
		public int Opacity04
		{
			get { return pOpacity04; }
			set
			{
				pOpacity04 = Math.Max(0, Math.Min(100, value));
				if (pBrush04 != null)
				{
					System.Windows.Media.Brush newBrush		= pBrush04.Clone();
					newBrush.Opacity	= pOpacity04 / 100d;
					newBrush.Freeze();
					pBrush04			= newBrush;
				}
			}
		}		
		
		

		
//		 SESSION 5 COLOR
		
		private System.Windows.Media.Brush	pBrush05 = Brushes.DimGray;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Zone Color", Description = "", GroupName = "Zones Display", Order = 10)]
		public Brush Brush05
		{
			get { return pBrush05; }
			set
			{
				pBrush05 = value;
				if (pBrush05 != null)
				{
					if (pBrush05.IsFrozen)
						pBrush05 = pBrush05.Clone();
					pBrush05.Opacity = pOpacity05 / 100d;
					pBrush05.Freeze();
				}
			}
		}

		[Browsable(false)]
		public string Brush05S
		{
			get { return Serialize.BrushToString(Brush05); }
			set { Brush05 = Serialize.StringToBrush(value); }
		}

		private int	pOpacity05 = 30;
		[Range(0, 100)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Zone Opacity (%)", Description = "", GroupName = "Zones Display", Order = 11)]
		public int Opacity05
		{
			get { return pOpacity05; }
			set
			{
				pOpacity05 = Math.Max(0, Math.Min(100, value));
				if (pBrush05 != null)
				{
					System.Windows.Media.Brush newBrush		= pBrush05.Clone();
					newBrush.Opacity	= pOpacity05 / 100d;
					newBrush.Freeze();
					pBrush05			= newBrush;
				}
			}
		}		
		
		

		
		
		
		
		
        private bool pShowMenu = false;
//        [Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", Description = "", GroupName = "Menu", Order = -3)]
//        public bool ShowMenu
//        {
//            get { return pShowMenu; }
//            set { pShowMenu = value; }
//        }	
		
		private Brush pMenuBrush	= Brushes.LightGray;
		
		
		
		
		
		
        private bool pActive6 = false;
//        [Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", Description = "", GroupName = "ADR", Order = 1)]
//        public bool Active6
//        {
//            get { return pActive6; }
//            set { pActive6 = value; }
//        }	

		private int pADRPeriod1 = 10;
//		[Range(0, 1000)]
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Period 1", Description="", GroupName = "ADR", Order = 2)]
//		public int ADRPeriod1
//		{
//			get { return pADRPeriod1; }
//			set { pADRPeriod1= value; }
//		}			
		
		private int pADRPeriod2 = 20;
//		[Range(0, 1000)]
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Period 2", Description="", GroupName = "ADR", Order = 3)]
//		public int ADRPeriod2
//		{
//			get { return pADRPeriod2; }
//			set { pADRPeriod2= value; }
//		}		
		
		
		
//					[Display(Name="Text Font", Description="", GroupName="Levels", Order = 1)]
//			public SimpleFont TextFont
//			{ get; set; }	
			
			
			private System.Windows.Media.Brush pColorTextBrush	= Brushes.Gainsboro;
		
		
//			private TablePositionSR pTPosition = TablePositionSR.BottomLeft;
//			[NinjaScriptProperty]
//			[Display(Name = "Position", Description = "", GroupName = "ADR", Order = 4)]
//			public TablePositionSR TPosition
//			{
//				get { return pTPosition; }
//				set { pTPosition = value; }
//			}	
			
			private int	pPixelsFromRight = 10;
//			[Range(2, int.MaxValue)]
//			[Display(ResourceType = typeof(Custom.Resource), Name = "Offset X (Pixels)", Description = "", GroupName = "ADR", Order = 5)]
//			public int PixelsFromRight
//			{
//				get { return pPixelsFromRight; }
//				set { pPixelsFromRight = value; }
//			}	
			
			private int	pPixelsFromBottom = 30;
//			[Range(2, int.MaxValue)]
//			[Display(ResourceType = typeof(Custom.Resource), Name = "Offset Y (Pixels)", Description = "", GroupName = "ADR", Order = 6)]
//			public int PixelsFromBottom
//			{
//				get { return pPixelsFromBottom; }
//				set { pPixelsFromBottom = value; }
//			}		
			
			private int pSpaceToLine = 1;
			private int YCellPadding = 6;
			private	int XCellPadding = 12; // label padding
			private	int XSBetweenLabels = 5; // space between labels that get adjusted to the left
		
			private int	pMarginB = 6;
		
		
		
//			[Range(0, int.MaxValue)]
//			[Display(ResourceType = typeof(Custom.Resource), Name = "Margin (Pixels)", Description = "", GroupName = "ADR", Order = 7)]
//			public int MarginB
//			{
//				get { return pMarginB; }
//				set { pMarginB = value; }
//			}					
			

						
//			private int	pFillOpacity = 80;
//			[Range(0, 100)]
//			[Display(ResourceType = typeof(Custom.Resource), Name = "Fill Opacity (%)", Description = "", GroupName = "ADR", Order = 11)]
//			public int FillOpacity
//			{
//				get { return pFillOpacity; }
//				set { pFillOpacity = value; }
//			}	
			
//			private int	pOutlineOpacity = 100;
//			[Range(0, 100)]
//			[Display(ResourceType = typeof(Custom.Resource), Name = "Outline Opacity (%)", Description = "", GroupName = "ADR", Order = 12)]
//			public int OutlineOpacity
//			{
//				get { return pOutlineOpacity; }
//				set { pOutlineOpacity = value; }
//			}	
		
		
		private int	pShowDaysOpen = 5;
			[Range(0, int.MaxValue)]
			[Display(ResourceType = typeof(Custom.Resource), Name = "Open Look Back (Days)", Description = "", GroupName = "OHLC", Order = 8)]
			public int ShowDaysOpen
			{
				get { return pShowDaysOpen; }
				set { pShowDaysOpen = value; }
			}		
						
					
			private int	pShowDaysHighLow = 20;
			[Range(0, int.MaxValue)]
			[Display(ResourceType = typeof(Custom.Resource), Name = "High / Low Look Back (Days)", Description = "", GroupName = "OHLC", Order = 6)]
			public int ShowDaysHighLow
			{
				get { return pShowDaysHighLow; }
				set { pShowDaysHighLow = value; }
			}		
			
			private int	pShowDaysClose = 5;
			[Range(0, int.MaxValue)]
			[Display(ResourceType = typeof(Custom.Resource), Name = "Close Look Back (Days) ", Description = "", GroupName = "OHLC", Order = 7)]
			public int ShowDaysClose
			{
				get { return pShowDaysClose; }
				set { pShowDaysClose = value; }
			}

			private bool pDisplayGlobexEnabled = true;
			[Display(ResourceType = typeof(Custom.Resource), Name = "Globex Enabled", Description = "Show Globex overnight session High/Low levels", GroupName = "OHLC", Order = 10)]
			public bool DisplayGlobexEnabled
			{
				get { return pDisplayGlobexEnabled; }
				set { pDisplayGlobexEnabled = value; }
			}

			private int pShowDaysGlobex = 5;
			[Range(0, int.MaxValue)]
			[Display(ResourceType = typeof(Custom.Resource), Name = "Globex Look Back (Days)", Description = "Number of days to show Globex High/Low levels", GroupName = "OHLC", Order = 11)]
			public int ShowDaysGlobex
			{
				get { return pShowDaysGlobex; }
				set { pShowDaysGlobex = value; }
			}

			private int pGlobexConfluenceTicks = 3;
			[Range(0, 50)]
			[Display(ResourceType = typeof(Custom.Resource), Name = "Globex Confluence (Ticks)", Description = "How close (in ticks) a Globex level must be to another level for confluence marking", GroupName = "OHLC", Order = 12)]
			public int GlobexConfluenceTicks
			{
				get { return pGlobexConfluenceTicks; }
				set { pGlobexConfluenceTicks = value; }
			}

		// ========== ZONE INTELLIGENCE ==========

		private bool pZoneScoreEnabled = true;
		[Display(ResourceType = typeof(Custom.Resource), Name = "Zone Strength Scoring", Description = "Score zones by level type weights and recency. Higher score = stronger zone. Opacity scales with score.", GroupName = "Zone Intelligence", Order = 0)]
		public bool ZoneScoreEnabled
		{
			get { return pZoneScoreEnabled; }
			set { pZoneScoreEnabled = value; }
		}

		private bool pZoneTierColorEnabled = false;
		[Display(ResourceType = typeof(Custom.Resource), Name = "Zone Tier Color Coding", Description = "Color-tint zones by score tier: gray (low), gold (mid), orange (high), red (max). Score number also matches tier.", GroupName = "Zone Intelligence", Order = 1)]
		public bool ZoneTierColorEnabled
		{
			get { return pZoneTierColorEnabled; }
			set { pZoneTierColorEnabled = value; }
		}

		private double pZoneTierMidThreshold = 3.0;
		[Range(0.1, 50.0)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Tier Mid Threshold", Description = "Minimum score for Mid tier (gold tint).", GroupName = "Zone Intelligence", Order = 2)]
		public double ZoneTierMidThreshold
		{
			get { return pZoneTierMidThreshold; }
			set { pZoneTierMidThreshold = value; }
		}

		private double pZoneTierHighThreshold = 6.0;
		[Range(0.1, 50.0)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Tier High Threshold", Description = "Minimum score for High tier (orange tint).", GroupName = "Zone Intelligence", Order = 3)]
		public double ZoneTierHighThreshold
		{
			get { return pZoneTierHighThreshold; }
			set { pZoneTierHighThreshold = value; }
		}

		private double pZoneTierMaxThreshold = 9.0;
		[Range(0.1, 50.0)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Tier Max Threshold", Description = "Minimum score for Max tier (red tint) - the 'do not ignore' zones.", GroupName = "Zone Intelligence", Order = 4)]
		public double ZoneTierMaxThreshold
		{
			get { return pZoneTierMaxThreshold; }
			set { pZoneTierMaxThreshold = value; }
		}

		private bool pZoneCentroidEnabled = true;
		[Display(ResourceType = typeof(Custom.Resource), Name = "Zone Centroid Line", Description = "Draw a weighted center-of-gravity line within each zone. The centroid is weighted by level importance.", GroupName = "Zone Intelligence", Order = 5)]
		public bool ZoneCentroidEnabled
		{
			get { return pZoneCentroidEnabled; }
			set { pZoneCentroidEnabled = value; }
		}

		private bool pNearestZoneHUDEnabled = true;
		[Display(ResourceType = typeof(Custom.Resource), Name = "Nearest Zone Distance", Description = "Show distance in ticks to nearest zone above and below current price in top-right corner.", GroupName = "Zone Intelligence", Order = 6)]
		public bool NearestZoneHUDEnabled
		{
			get { return pNearestZoneHUDEnabled; }
			set { pNearestZoneHUDEnabled = value; }
		}

		private bool pUniversalConfluenceEnabled = true;
		[Display(ResourceType = typeof(Custom.Resource), Name = "Universal Confluence Detection", Description = "Mark any two levels from different categories that coincide within the threshold.", GroupName = "Zone Intelligence", Order = 7)]
		public bool UniversalConfluenceEnabled
		{
			get { return pUniversalConfluenceEnabled; }
			set { pUniversalConfluenceEnabled = value; }
		}

		private int pUniversalConfluenceTicks = 3;
		[Range(0, 50)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Confluence Threshold (Ticks)", Description = "How close two levels from different categories must be to mark as confluence.", GroupName = "Zone Intelligence", Order = 8)]
		public int UniversalConfluenceTicks
		{
			get { return pUniversalConfluenceTicks; }
			set { pUniversalConfluenceTicks = value; }
		}

		private bool pAdaptiveZoneWidthEnabled = true;
		[Display(ResourceType = typeof(Custom.Resource), Name = "Adaptive Zone Width", Description = "Automatically size zones using standard deviation of inter-level gaps. Replaces fixed width when enabled.", GroupName = "Zone Intelligence", Order = 9)]
		public bool AdaptiveZoneWidthEnabled
		{
			get { return pAdaptiveZoneWidthEnabled; }
			set { pAdaptiveZoneWidthEnabled = value; }
		}

		private double pAdaptiveZoneSigma = 0.75;
		[Range(0.1, 3.0)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Adaptive Sigma Multiplier", Description = "Multiplier for standard deviation of level gaps. Lower = tighter zones. 0.75 recommended.", GroupName = "Zone Intelligence", Order = 10)]
		public double AdaptiveZoneSigma
		{
			get { return pAdaptiveZoneSigma; }
			set { pAdaptiveZoneSigma = value; }
		}

		private bool pFreshnessDimmingEnabled = true;
		[Display(ResourceType = typeof(Custom.Resource), Name = "Level Freshness Dimming", Description = "Dim older levels based on when price last touched them. Levels tested recently stay bright; untested stale levels fade.", GroupName = "Zone Intelligence", Order = 11)]
		public bool FreshnessDimmingEnabled
		{
			get { return pFreshnessDimmingEnabled; }
			set { pFreshnessDimmingEnabled = value; }
		}

		private int pFreshnessTestTicks = 5;
		[Range(1, 50)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Freshness Test Range (Ticks)", Description = "How close price must come to a level to count as 'tested'. Tighter = stricter freshness.", GroupName = "Zone Intelligence", Order = 12)]
		public int FreshnessTestTicks
		{
			get { return pFreshnessTestTicks; }
			set { pFreshnessTestTicks = value; }
		}

		private int pFreshnessStaleBars = 500;
		[Range(10, 10000)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Freshness Stale After (Bars)", Description = "A level is considered fully stale if price has not tested it in this many bars. Older = more dimmed.", GroupName = "Zone Intelligence", Order = 13)]
		public int FreshnessStaleBars
		{
			get { return pFreshnessStaleBars; }
			set { pFreshnessStaleBars = value; }
		}

        private bool pPivotsRegularEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Regular Enabled", Description = "", GroupName = "Pivots", Order = -10)]
        public bool PivotsRegularEnabled
        {
            get { return pPivotsRegularEnabled; }
            set { pPivotsRegularEnabled = value; }
        }	
		
        private bool pPivotsFibonacciEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Fibonacci Enabled", Description = "", GroupName = "Pivots", Order = -9)]
        public bool PivotsFibonacciEnabled
        {
            get { return pPivotsFibonacciEnabled; }
            set { pPivotsFibonacciEnabled = value; }
        }	
				
        private bool pPivotsCamarillaEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Camarilla Enabled", Description = "", GroupName = "Pivots", Order = -8)]
        public bool PivotsCamarillaEnabled
        {
            get { return pPivotsCamarillaEnabled; }
            set { pPivotsCamarillaEnabled = value; }
        }	
				
        private bool pPivotsDemarkEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Demark Enabled", Description = "", GroupName = "Pivots", Order = -8)]
        public bool PivotsDemarkEnabled
        {
            get { return pPivotsDemarkEnabled; }
            set { pPivotsDemarkEnabled = value; }
        }	
		
        private bool pPivotsWoodiesEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Woodies Enabled", Description = "", GroupName = "Pivots", Order = -8)]
        public bool PivotsWoodiesEnabled
        {
            get { return pPivotsWoodiesEnabled; }
            set { pPivotsWoodiesEnabled = value; }
        }			
		
		
		
		private int	pPivotPointDays = 3;
			[Range(0, int.MaxValue)]
			[Display(ResourceType = typeof(Custom.Resource), Name = "Floor PP Look Back (Days)", Description = "", GroupName = "Pivots", Order = 0)]
			public int PivotPointDays
			{
				get { return pPivotPointDays; }
				set { pPivotPointDays = value; }
			}				
		
			
			private int	pPivotRegularLevelDays = 1;
			[Range(0, int.MaxValue)]
			[Display(ResourceType = typeof(Custom.Resource), Name = "Floor S/R Look Back (Days)", Description = "", GroupName = "Pivots", Order = 2)]
			public int PivotRegularLevelDays
			{
				get { return pPivotRegularLevelDays; }
				set { pPivotRegularLevelDays = value; }
			}				
		
			private int	pPivotFibonacciLevelDays = 0;
			[Range(0, int.MaxValue)]
			[Display(ResourceType = typeof(Custom.Resource), Name = "Fibonacci S/R Look Back (Days)", Description = "", GroupName = "Pivots", Order = 4)]
			public int PivotFibonacciLevelDays
			{
				get { return pPivotFibonacciLevelDays; }
				set { pPivotFibonacciLevelDays = value; }
			}				
			
			
			private int	pPivotCamarillaLevelDays = 0;
			[Range(0, int.MaxValue)]
			[Display(ResourceType = typeof(Custom.Resource), Name = "Camarilla S/R Look Back (Days)", Description = "", GroupName = "Pivots", Order = 6)]
			public int PivotCamarillaLevelDays
			{
				get { return pPivotCamarillaLevelDays; }
				set { pPivotCamarillaLevelDays = value; }
			}				
						
						
			
		private int	pPivotDemarkPointDays = 0;
			[Range(0, int.MaxValue)]
			[Display(ResourceType = typeof(Custom.Resource), Name = "DeMark PP Look Back (Days)", Description = "", GroupName = "Pivots", Order = 8)]
			public int PivotDemarkPointDays
			{
				get { return pPivotDemarkPointDays; }
				set { pPivotDemarkPointDays = value; }
			}				
		
			
			private int	pPivotDemarkLevelDays = 0;
			[Range(0, int.MaxValue)]
			[Display(ResourceType = typeof(Custom.Resource), Name = "DeMark S/R Look Back (Days)", Description = "", GroupName = "Pivots", Order = 10)]
			public int PivotDemarkLevelDays
			{
				get { return pPivotDemarkLevelDays; }
				set { pPivotDemarkLevelDays = value; }
			}		
			
			
		private int	pPivotWoodiesPointDays = 0;
			[Range(0, int.MaxValue)]
			[Display(ResourceType = typeof(Custom.Resource), Name = "Woodie PP Look Back (Days)", Description = "", GroupName = "Pivots", Order = 12)]
			public int PivotWoodiesPointDays
			{
				get { return pPivotWoodiesPointDays; }
				set { pPivotWoodiesPointDays = value; }
			}				
		
			
			private int	pPivotWoodiesLevelDays = 0;
			[Range(0, int.MaxValue)]
			[Display(ResourceType = typeof(Custom.Resource), Name = "Woodie S/R Look Back (Days)", Description = "", GroupName = "Pivots", Order = 14)]
			public int PivotWoodiesLevelDays
			{
				get { return pPivotWoodiesLevelDays; }
				set { pPivotWoodiesLevelDays = value; }
			}		
						
			
										
									
			
//								if (pPivotsFibonacciEnabled)
							
//								if (pPivotsCamarillaEnabled)
									
									
		
		
	

    
    
        private Brush areaBrush = Brushes.DimGray;
        private Brush textBrush = Brushes.Blue;
        //private Brush smallAreaBrush = Brushes.Red;
        private int areaOpacity = 50;
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
            set { panelExpanded = value; }
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


       // [Description("number of ticks within a level to trigger an output for Market Analyer. The column will display positive numbers for R levels and negative numbers for S levels.  For example, if the row displays -2 for ES, then it means that ES  is close to the S2 level.")]

		
       private bool pFibsEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", Description = "", GroupName = "Fibonacci", Order = -10)]
        public bool FibsEnabled
        {
            get { return pFibsEnabled; }
            set { pFibsEnabled = value; }
        }
		
		
		
		private int	pShowDaysMid = 5;
			[Range(0, int.MaxValue)]
			[Display(ResourceType = typeof(Custom.Resource), Name = "Middle Look Back (Days)", Description = "", GroupName = "Fibonacci", Order = -9)]
			public int ShowDaysMid
			{
				get { return pShowDaysMid; }
				set { pShowDaysMid = value; }
			}				
			
				
			private int	pShowDaysFibs = 1;
			[Range(0, int.MaxValue)]
			[Display(ResourceType = typeof(Custom.Resource), Name = "Extensions Look Back (Days) ", Description = "", GroupName = "Fibonacci", Order = -8)]
			public int ShowDaysFibs
			{
				get { return pShowDaysFibs; }
				set { pShowDaysFibs = value; }
			}	
			
			
		
			
    private bool pAutoNumberEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Automatic", Description = "automatically determine important whole numbers for the specific instrument", GroupName = "Whole Numbers", Order = -10)]
        public bool AutoNumberEnabled
        {
            get { return pAutoNumberEnabled; }
            set { pAutoNumberEnabled = value; }
        }
					
			
		private double pWholeNumber1 = 0;
        [Range(0, double.MaxValue)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Increment 1", GroupName = "Whole Numbers", Order = 1)]
        public double WholeNumber1
        {
            get { return pWholeNumber1; }
            set { pWholeNumber1 = value; }
        }	
			
		
		private double pWholeNumber2 = 0;
        [Range(0, double.MaxValue)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Increment 2", GroupName = "Whole Numbers", Order = 2)]
        public double WholeNumber2
        {
            get { return pWholeNumber2; }
            set { pWholeNumber2 = value; }
        }	
			
		private double pWholeNumber3 = 0;
        [Range(0, double.MaxValue)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Increment 3", GroupName = "Whole Numbers", Order = 3)]
        public double WholeNumber3
        {
            get { return pWholeNumber3; }
            set { pWholeNumber3 = value; }
        }	
		
	
		
		
		private bool pCDIsEnabled = false;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", Description = "", GroupName = "Current Day", Order = 0)]
		[RefreshProperties(RefreshProperties.All)]
        public bool CDIsEnabled
        {
            get { return pCDIsEnabled; }
            set { pCDIsEnabled = value; }
        }		
		
		
	
        private double pPercentCD1 = 78.6;
        [Range(0, double.MaxValue)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Extension 1 (%)", GroupName = "Current Day", Order = 1)]
        public double PercentCD1
        {
            get { return pPercentCD1; }
            set { pPercentCD1 = value; }
        }	
							
		
        private double pPercentCD2 = 127.2;
        [Range(0, double.MaxValue)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Extension 2 (%)", GroupName = "Current Day", Order = 2)]
        public double PercentCD2
        {
            get { return pPercentCD2; }
            set { pPercentCD2 = value; }
        }
		
        private double pPercentCD3 = 161.8;
        [Range(0, double.MaxValue)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Extension 3 (%)", GroupName = "Current Day", Order = 3)]
        public double PercentCD3
        {
            get { return pPercentCD3; }
            set { pPercentCD3 = value; }
        }		
		
        private double pPercentCD4 = 0;
        [Range(0, double.MaxValue)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Extension 4 (%)", GroupName = "Current Day", Order = 4)]
        public double PercentCD4
        {
            get { return pPercentCD4; }
            set { pPercentCD4 = value; }
        }		
				
		
		
		
		
        private double pPercent1 = 78.6;
        [Range(0, double.MaxValue)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Extension 1 (%)", GroupName = "Fibonacci", Order = 1)]
        public double Percent1
        {
            get { return pPercent1; }
            set { pPercent1 = value; }
        }	
							
		
        private double pPercent2 = 127.2;
        [Range(0, double.MaxValue)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Extension 2 (%)", GroupName = "Fibonacci", Order = 2)]
        public double Percent2
        {
            get { return pPercent2; }
            set { pPercent2 = value; }
        }
		
        private double pPercent3 = 161.8;
        [Range(0, double.MaxValue)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Extension 3 (%)", GroupName = "Fibonacci", Order = 3)]
        public double Percent3
        {
            get { return pPercent3; }
            set { pPercent3 = value; }
        }		
		
        private double pPercent4 = 200;
        [Range(0, double.MaxValue)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Extension 4 (%)", GroupName = "Fibonacci", Order = 4)]
        public double Percent4
        {
            get { return pPercent4; }
            set { pPercent4 = value; }
        }		
									
        private double pPercent5 = 0;
        [Range(0, double.MaxValue)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Extension 5 (%)", GroupName = "Fibonacci", Order = 5)]
        public double Percent5
        {
            get { return pPercent5; }
            set { pPercent5 = value; }
        }	
		
        private double pPercent6 = 0;
        [Range(0, double.MaxValue)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Extension 6 (%)", GroupName = "Fibonacci", Order = 6)]
        public double Percent6
        {
            get { return pPercent6; }
            set { pPercent6 = value; }
        }			
		
        private double pPercent7 = 0;
        [Range(0, double.MaxValue)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Extension 7 (%)", GroupName = "Fibonacci", Order = 7)]
        public double Percent7
        {
            get { return pPercent7; }
            set { pPercent7 = value; }
        }	
		
        private double pPercent8 = 0;
        [Range(0, double.MaxValue)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Extension 8 (%)", GroupName = "Fibonacci", Order = 8)]
        public double Percent8
        {
            get { return pPercent8; }
            set { pPercent8 = value; }
        }	
		
		
		
		private string pLicensingEmailAddress = "";
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "License", Name = "Email Address", Order = 54, Description = "")]
        public string LicensingEmailAddress
        {
            get { return pLicensingEmailAddress; }
            set { pLicensingEmailAddress = value; }
        }			
		
		
	
			
       private bool pFibsMidEnabled = true;
//        [Display(ResourceType = typeof(Custom.Resource), Name = "   Middle Enabled", Description = "", GroupName = "Fibonacci", Order = -9)]
//        public bool FibsMidEnabled
//        {
//            get { return pFibsMidEnabled; }
//            set { pFibsMidEnabled = value; }
//        }		
		
		
       private bool pFibsExEnabled = true;
//        [Display(ResourceType = typeof(Custom.Resource), Name = "   Extentions Enabled", Description = "", GroupName = "Fibonacci", Order = -8)]
//        public bool FibsExEnabled
//        {
//            get { return pFibsExEnabled; }
//            set { pFibsExEnabled = value; }
//        }		
		
		
		
		
		
			
			
				
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
       [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public Series<double> DR1
        {
            get { return Values[0]; }
        }
		
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
       [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public Series<double> DR2
        {
            get { return Values[1]; }
        }	
		
				[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
       [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public Series<double> DR3
        {
            get { return Values[2]; }
        }
		
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
       [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public Series<double> DR4
        {
            get { return Values[3]; }
        }	
		
				[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
       [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public Series<double> DR5
        {
            get { return Values[4]; }
        }
		
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
       [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public Series<double> DS1
        {
            get { return Values[5]; }
        }	
		
				[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
       [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public Series<double> DS2
        {
            get { return Values[6]; }
        }

		
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public Series<double> DS3
        {
            get { return Values[7]; }
        }
		
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public Series<double> DS4
        {
            get { return Values[8]; }
        }

		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public Series<double> DS5
        {
            get { return Values[9]; }
        }
		
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public Series<double> DS6
        {
            get { return Values[10]; }
        }
		
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public Series<double> DS7
        {
            get { return Values[11]; }
        }
		
				[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public Series<double> MS1
        {
            get { return Values[12]; }
        }
		
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public Series<double> MS2
        {
            get { return Values[13]; }
        }
		
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public Series<double> MS3
        {
            get { return Values[14]; }
        }
		
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public Series<double> MS4
        {
            get { return Values[15]; }
        }
		
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public Series<double> MS5
        {
            get { return Values[16]; }
        }
		
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public Series<double> MS6
        {
            get { return Values[17]; }
        }
		
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public Series<double> MS7
        {
            get { return Values[18]; }
        }
		
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public Series<double> MS8
        {
            get { return Values[19]; }
        }
		
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public Series<double> MS9
        {
            get { return Values[20]; }
        }
		
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public Series<double> MS10
        {
            get { return Values[21]; }
        }		
		
	}
	
	
	
		
		// Hide UserDefinedValues properties when not in use by the HLCCalculationMode.UserDefinedValues
	// When creating a custom type converter for indicators it must inherit from NinjaTrader.NinjaScript.IndicatorBaseConverter to work correctly with indicators
	public class aiSRKeyLevelsConverter : NinjaTrader.NinjaScript.IndicatorBaseConverter
	{
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) { return true; }

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			PropertyDescriptorCollection propertyDescriptorCollection = base.GetPropertiesSupported(context) ? base.GetProperties(context, value, attributes) : TypeDescriptor.GetProperties(value, attributes);

			aiSRKeyLevels   jbb = (aiSRKeyLevels) value;
			
			//Pivots						thisPivotsInstance			= (Pivots) value;
			
			//bool MagnetsOn = ;
			
			List<string> DeleteThese = new List<string>();
			List<string> DeleteThese2 = new List<string>();
			
			
	
			DeleteThese.Add("CDEnabled");
			
			
		
			if (!jbb.CDIsEnabled)
			{
				DeleteThese.Add("PercentCD1");
				DeleteThese.Add("PercentCD2");
				DeleteThese.Add("PercentCD3");
				DeleteThese.Add("PercentCD4");
				
			}		
			
			
			
	
				
		
		
		
		
			
			DeleteThese.Add("IsLifeTime");
			
				DeleteThese.Add("EntriesEnabled");
				DeleteThese.Add("LongEnabled");
				DeleteThese.Add("ShortEnabled");
			DeleteThese.Add("TrendOnlyEnabled");	
			
				DeleteThese.Add("AutoEnabled");	
			
			DeleteThese.Add("SLTrailOrdersEnabled");	
			DeleteThese.Add("ExitOrdersEnabled");	
			

			
			DeleteThese.Add("DisplayLevelsEnabled");
			DeleteThese.Add("DisplayOHLCEnabled");	
			DeleteThese.Add("DisplayFibEnabled");
			DeleteThese.Add("DisplayPivotsEnabled");	
			DeleteThese.Add("DisplayHistoryEnabled");					
			DeleteThese.Add("DisplayZonesEnabled");
			DeleteThese.Add("DisplayWholeNumbersEnabled");
			
			
			
			DeleteThese.Add("FibsEnabled");
			
	
		if (!jbb.ButtonsEnabled)
			{			
			
				DeleteThese.Add("AreaBrush");
				DeleteThese.Add("AreaOpacity");
				DeleteThese.Add("ButtonSize");				
			}				
			
    
			
			
			if (!jbb.DisplayWholeNumbersEnabled)
			{
				
				
			}
			
				
//			if (jbb.ThisEntryType22 == "Bars")
//			{			
//				DeleteThese.Add("Strength");
//				DeleteThese.Add("DeviationType");
//				DeleteThese.Add("DeviationValue");
//				//DeleteThese.Add("ThisBarType");
//				//DeleteThese.Add("ThisBarPeriod");				
//			}				
		
//			if (jbb.ThisEntryType22 == "Zig Zag")
//			{			
//				DeleteThese.Add("Strength");
//				//DeleteThese.Add("DeviationType");
//				//DeleteThese.Add("DeviationValue");
//				DeleteThese.Add("ThisBarType");
//				DeleteThese.Add("ThisBarPeriod");				
//			}		
		
//			if (jbb.ThisEntryType22 == "Swing")
//			{			
//				//DeleteThese.Add("Strength");
//				DeleteThese.Add("DeviationType");
//				DeleteThese.Add("DeviationValue");
//				DeleteThese.Add("ThisBarType");
//				DeleteThese.Add("ThisBarPeriod");				
//			}				 
			 
			
			DeleteThese.Add("Calculate");
			//DeleteThese.Add("Name");
      		DeleteThese.Add("MaximumBarsLookBack");
			
			DeleteThese.Add("Input");
			
			DeleteThese.Add("IsAutoScale");
			DeleteThese.Add("Displacement");
			DeleteThese.Add("DisplayInDataBox");
			DeleteThese.Add("Panel");
			//DeleteThese.Add("PaintPriceMarkers");
			DeleteThese.Add("ScaleJustification");
			//DeleteThese.Add("IsVisible");
			
			
			
			DeleteThese.Add("PivotsRegularEnabled");
			DeleteThese.Add("PivotsFibonacciEnabled");
			DeleteThese.Add("PivotsCamarillaEnabled");
			DeleteThese.Add("PivotsDemarkEnabled");
			DeleteThese.Add("PivotsWoodiesEnabled");			

       
			
			
			
			if (jbb.AutoModeEnabled)
			{			
				DeleteThese.Add("ZWM");
				DeleteThese.Add("TicksSpace");
				
			}
			else
			{
				DeleteThese.Add("ZATRMultiplier");
			}
		
		
//	if (!jbb.UseTimeFilter)
//			{			
//				DeleteThese.Add("StartT");
//				DeleteThese.Add("EndT");
//				DeleteThese.Add("StartT2");
//				DeleteThese.Add("EndT2");
//				DeleteThese.Add("StartT3");
//				DeleteThese.Add("EndT3");
								
				
		
		
		
//			}			
					
			
			
			
			DeleteThese.Add("Brush03");
			DeleteThese.Add("Opacity03");
			DeleteThese.Add("Brush04");
			DeleteThese.Add("Opacity04");

						
			
			
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

//public enum TablePositionSR {
//	TopLeft, TopRight, BottomLeft, BottomRight, 
//}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private aiSRKeyLevels[] cacheaiSRKeyLevels;
		public aiSRKeyLevels aiSRKeyLevels()
		{
			return aiSRKeyLevels(Input);
		}

		public aiSRKeyLevels aiSRKeyLevels(ISeries<double> input)
		{
			if (cacheaiSRKeyLevels != null)
				for (int idx = 0; idx < cacheaiSRKeyLevels.Length; idx++)
					if (cacheaiSRKeyLevels[idx] != null &&  cacheaiSRKeyLevels[idx].EqualsInput(input))
						return cacheaiSRKeyLevels[idx];
			return CacheIndicator<aiSRKeyLevels>(new aiSRKeyLevels(), input, ref cacheaiSRKeyLevels);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.aiSRKeyLevels aiSRKeyLevels()
		{
			return indicator.aiSRKeyLevels(Input);
		}

		public Indicators.aiSRKeyLevels aiSRKeyLevels(ISeries<double> input )
		{
			return indicator.aiSRKeyLevels(input);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.aiSRKeyLevels aiSRKeyLevels()
		{
			return indicator.aiSRKeyLevels(Input);
		}

		public Indicators.aiSRKeyLevels aiSRKeyLevels(ISeries<double> input )
		{
			return indicator.aiSRKeyLevels(input);
		}
	}
}

#endregion



