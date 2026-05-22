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

//This namespace holds indicators in this folder and is required. Do not change it.
namespace NinjaTrader.NinjaScript.Indicators
{
	/// <summary>
	/// The PFE (Polarized Fractal Efficiency) is an indicator that uses fractal
	///  geometry to determine how efficiently the price is moving.
	/// </summary>
	/// 


	[Gui.CategoryOrder("Multi Time Frame", 1)]
	[Gui.CategoryOrder("Parameters ", 10)]
	[Gui.CategoryOrder("Lines", 20)]
	[Gui.CategoryOrder("Trend", 40)]
	[Gui.CategoryOrder("Signals", 50)]
	[Gui.CategoryOrder("Labels", 60)]
	[Gui.CategoryOrder("Bar Color", 70)]
	[Gui.CategoryOrder("Background Color", 75)]

	[Gui.CategoryOrder("Plots", 80)]
	[Gui.CategoryOrder("Audio", 100)]
	[Gui.CategoryOrder("Email", 110)]


	[Gui.CategoryOrder("Visual", 156)]
	[Gui.CategoryOrder("Data Series", 165)]





	[Gui.CategoryOrder("Setup", 9000)]
	[Gui.CategoryOrder("License", 10000)]






	[TypeConverter("NinjaTrader.NinjaScript.Indicators.aiBestStochasticsConverter")]
	public class aiBestStochastics : Indicator
	{


		private string ThisName = "aiBestStochastics";


		private Series<double> div;
		private EMA ema;
		private Series<double> pfeSeries;
		private Series<double> singlePfeSeries;

		private int LastEmailBar, LastAudioBar = 0;
		private string message, subject = string.Empty;

		private bool PotentialLong, PotentialShort, CrossLong, CrossShort = false;
		private double UpperLevel, LowerLevel = 0;


		private Series<int> LastSwitchBar;
		private Series<int> Trend;
		private Series<int> Signal;
		private Series<double> SignalPlot;



		private Series<double> den;
		private MAX max;
		private MIN min;
		private Series<double> nom;
		private SMA smaK;
		private SMA smaFastK;
		private Series<double> fastK;



		private Series<double> D;
		private Series<double> K;

		private Series<double> MTFData1;
		private Series<double> MTFData2;

		private Series<int> Trend1;
		private Series<int> Direction1;
		private Series<double> UpperArea;
		private Series<double> LowerArea;
		private Series<double> UpperArea2;
		private Series<double> LowerArea2;

		private Series<double> LastMTFBar;
		private Series<double> LastMTFSignal;

		// bar color

		// bar color (direct assignment — no intermediate Series<int> needed)


		private int FinalBarOpacity = 0;
		private Color TempColor;


		private Brush UpNeutralBrush;
		private Brush DownNeutralBrush;
		private Brush UpBuyBrush;
		private Brush DownBuyBrush;
		private Brush UpSellBrush;
		private Brush DownSellBrush;

		private Brush FinalBarBrush;
		private Brush TempBrush;




		//private StochasticsFast iBestSto;







		private SharpDX.RectangleF ExpandRect(SharpDX.RectangleF RR, float left, float right, float top, float bottom)
		{

			SharpDX.RectangleF FF = new SharpDX.RectangleF(RR.X - left, RR.Y - top, RR.Width + left + right, RR.Height + top + bottom);

			return FF;

		}

		private SharpDX.RectangleF ExpandRect(SharpDX.RectangleF RR, float xe, float ye)
		{

			SharpDX.RectangleF FF = new SharpDX.RectangleF(RR.X - xe, RR.Y - ye, RR.Width + xe * 2, RR.Height + ye * 2);

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

		private bool LicenseWordPress(string machineid, string pLicensingEmailAddress)
		{
			pLicensingEmailAddress = pLicensingEmailAddress.Replace(" ", "");

			if (pLicensingEmailAddress == "joelwyse@gmail.com" && machineid == "932A5E287330F29AA24C46E6687CAB8B")
				return true;
			if (pLicensingEmailAddress == "koehlerd63@gmail.com" && machineid == "C32AB233848A15D97E70DA974DF014CC")
				return true;
			if (pLicensingEmailAddress == "koehlerd63@gmail.com" && machineid == "B9811B980E602BD8E79BD65C7B0CF5DE")
				return true;




			// test specific user 

			//			pLicensingEmailAddress = "nextgentrader@hotmail.com";
			//			machineid = "6953F65ADDF5B541F8FDE34DD1AFF193";

			//			pLicensingEmailAddress = "markapl87@gmail.com";
			//			machineid = "C75AF30F684FB30C457817ADB133177D";			









			List<int> ThisProductMainIDs = new List<int>();
			List<int> ThisProductSecondaryIDs = new List<int>();

			// Product IDs for Main Indicator

			ThisProductMainIDs.Add(514365);
			//ThisProductMainIDs.Add(503567); // support and resistance suite

			// Product IDs for Secondary Features

			//			ThisProductSecondaryIDs.Add(19318);



			string pContactEmail = "'license@affordableindicators.com'";






			//Print("Check License Now Indicator");


			string url = "";
			string result = "";
			//string 	machineid = "";
			string instrument = "";
			string symbol = "";
			string module = "";
			string location = "";
			string filename = "";
			string contact = "";
			string contact2 = "";
			string message = "";
			string s1 = "";
			string s2 = "";
			string s3 = "";
			string s4 = "";
			string s5 = "";
			string s6 = "";
			string s7 = "";
			string s8 = "";
			string s9 = "";
			string s10 = "";
			string thisuniqueid = "";
			int daysremaining = 0;
			int warningdays = 0;

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
					string[] split = line.Split(',');

					//split.rever
					Array.Reverse(split);

					// Creates a counter that will mark which string from the split array we are dealing with
					int splitCounter = 0;



					int ordernumber = 0;

					foreach (string s in split)
					{


						splitCounter++;
						switch (splitCounter)
						{


							default:



								if (s.Contains("NOTE"))
								{
									notestring = s.Replace("NOTE", "");


								}
								else if (s.Contains("-"))
								{
									string[] split2 = s.Split('-');


									int productid = Convert.ToInt32(split2[0]);
									int numbermachineidsok = Convert.ToInt32(split2[1]);


									string allinstruments = "";


									if (split2.Length > 2) // contains info with instrument restriction
									{
										allinstruments = split2[2];
										//Print(allinstruments);

										string[] split3 = allinstruments.Split('|');

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

									if (thischarti != "YM" && thischarti != "MYM")
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
										LicensingMessage = "This product is only licensed for the following instruments: " + allinst + ".  Contact " + pContactEmail + " if you need further assistance.";
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
								LicensingMessage = "This product is only licensed for 1 NinjaTrader Machine ID.  Please login to your Affordable Indicators, Inc. account, go to the Members Area Dashboard, and update your NinjaTrader Machine ID(s) accordingly.";
							else
								LicensingMessage = "This product is only licensed for 1-" + maxids.ToString() + " NinjaTrader Machine IDs.  Please login to your Affordable Indicators, Inc. account, go to the Members Area Dashboard, and update your NinjaTrader Machine ID(s) accordingly.";

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





		private int FinalPeriodD = 0;
		private int FinalPeriodK = 0;



		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{

				Name = ThisName;
				Description = @"";


				IsSuspendedWhileInactive = true;
				//				Period						= 14;

				Smooth = 3;
				PeriodD = 7;
				PeriodK = 14;

				PeriodDFast = 3;
				PeriodKFast = 14;

				Calculate = Calculate.OnPriceChange;
				MaximumBarsLookBack = MaximumBarsLookBack.Infinite;


				//ArePlotsConfigurable = false;



				//				AddPlot(Brushes.DodgerBlue,		NinjaTrader.Custom.Resource.NinjaScriptIndicatorNamePFE);


				//				AddLine(Brushes.DarkGray,	0.6,	"Upper Level");
				//				AddLine(Brushes.DarkGray,	0,	"Zero Level");
				//				AddLine(Brushes.DarkGray,	-0.6,	"Lower Level");


				AddPlot(Brushes.DimGray, NinjaTrader.Custom.Resource.StochasticsD);
				AddPlot(Brushes.DimGray, NinjaTrader.Custom.Resource.StochasticsK);
				AddPlot(Brushes.Transparent, "Signals");

				AddLine(Brushes.DimGray, 20, NinjaTrader.Custom.Resource.NinjaScriptIndicatorLower);
				AddLine(Brushes.DimGray, 80, NinjaTrader.Custom.Resource.NinjaScriptIndicatorUpper);

			}
			else if (State == State.Configure)
			{

				if (pMTFEnabled)
				{
					string FinalInstrument = Instrument.FullName;

					if (pMTFInstrument != string.Empty)
						FinalInstrument = pMTFInstrument;



					if (FinalBasePeriodType1 == BarsPeriodType.Renko)
						AddRenko(Instrument.FullName, pMTFBarsPeriod1, MarketDataType.Last);
					else
						AddDataSeries(Instrument.FullName, FinalBasePeriodType1, pMTFBarsPeriod1);
				}



			}
			else if (State == State.DataLoaded)
			{

				if (Name != ThisName && Name != string.Empty)
					Name = ThisName;






				// checking for existing email tied to this computer


				string pFileLocation = NinjaTrader.Core.Globals.UserDataDir;

				if (!Directory.Exists(pFileLocation))
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
						System.IO.File.WriteAllText(location2, final2);
					}





				Permission = LicenseWordPress(NinjaTrader.Cbi.License.MachineId, pLicensingEmailAddress);





				// Series<int> for bar colors removed — direct BarBrushes[0] assignment used instead






				Trend1 = new Series<int>(this);
				Direction1 = new Series<int>(this);
				LastMTFBar = new Series<double>(this);
				LastMTFSignal = new Series<double>(this);

				UpperArea = new Series<double>(this, MaximumBarsLookBack.Infinite);
				LowerArea = new Series<double>(this, MaximumBarsLookBack.Infinite);
				UpperArea2 = new Series<double>(this, MaximumBarsLookBack.Infinite);
				LowerArea2 = new Series<double>(this, MaximumBarsLookBack.Infinite);


				if (pStochasticsType == "StochasticsFast")
				{

					FinalPeriodD = PeriodDFast;
					FinalPeriodK = PeriodKFast;


				}
				else
				{

					FinalPeriodD = PeriodD;
					FinalPeriodK = PeriodK;

				}


				if (pMTFEnabled)
				{
					MTFData1 = new Series<double>(BarsArray[1]);
					MTFData2 = new Series<double>(BarsArray[1]);

					//iBestSto = StochasticsFast(BarsArray[1], PeriodD,PeriodK);
				}
				else
				{


					//iBestSto = StochasticsFast(PeriodD,PeriodK);
				}


				D = new Series<double>(this);
				K = new Series<double>(this);



				den = new Series<double>(this);
				nom = new Series<double>(this);
				fastK = new Series<double>(this);
				min = MIN(Low, FinalPeriodK);
				max = MAX(High, FinalPeriodK);
				smaFastK = SMA(fastK, Smooth);
				smaK = SMA(K, FinalPeriodD);



				if (pMTFEnabled)
				{

					min = MIN(Lows[1], FinalPeriodK);
					max = MAX(Highs[1], FinalPeriodK);

					smaK = SMA(MTFData1, FinalPeriodD);

					//					min			= MIN(MTFData1, PeriodK);
					//					max			= MAX(MTFData2, PeriodK);

				}
				else
				{
					//min			= MIN(Lows[0], PeriodK);
					//max			= MAX(Highs[0], PeriodK);	

				}

				//				div				= new Series<double>(this);
				//				pfeSeries		= new Series<double>(this);
				//				singlePfeSeries	= new Series<double>(this);
				//				ema				= EMA(pfeSeries, Smooth);



				Signal = new Series<int>(this);
				Trend = new Series<int>(this);
				LastSwitchBar = new Series<int>(this);
				SignalPlot = new Series<double>(this);

				//Plots[1].DashStyleHelper = pDashS2;
				//Plots[1].Pen.DashStyle = pDashS2;
				//Plots[1].Width = pWidthPlot2;



				UpNeutralBrush = pBarNeutralBrush1;
				DownNeutralBrush = pBarNeutralBrush1;
				UpBuyBrush = pBarUpBrush1;
				DownBuyBrush = pBarUpBrush1;
				UpSellBrush = pBarDownBrush1;
				DownSellBrush = pBarDownBrush1;

				TempBrush = UpNeutralBrush.Clone();
				TempBrush.Opacity = pBarOpacityUp / 100f;
				TempBrush.Freeze();
				UpNeutralBrush = TempBrush;

				TempBrush = DownNeutralBrush.Clone();
				TempBrush.Opacity = pBarOpacityDown / 100f;
				TempBrush.Freeze();
				DownNeutralBrush = TempBrush;

				TempBrush = UpBuyBrush.Clone();
				TempBrush.Opacity = pBarOpacityUp / 100f;
				TempBrush.Freeze();
				UpBuyBrush = TempBrush;

				TempBrush = DownBuyBrush.Clone();
				TempBrush.Opacity = pBarOpacityDown / 100f;
				TempBrush.Freeze();
				DownBuyBrush = TempBrush;

				TempBrush = UpSellBrush.Clone();
				TempBrush.Opacity = pBarOpacityUp / 100f;
				TempBrush.Freeze();
				UpSellBrush = TempBrush;

				TempBrush = DownSellBrush.Clone();
				TempBrush.Opacity = pBarOpacityDown / 100f;
				TempBrush.Freeze();
				DownSellBrush = TempBrush;



			}
		}


		double LastMTFClose = 0;
		double valueofsma = 0;


		private double GetMTFData(Series<double> SSS, Series<double> SSS2, int bbb)
		{
			double thisfinal = 0;

			if (CurrentBars[0] < bbb)
				return 0;
			if (CurrentBars[1] < bbb)
				return 0;


			if (SSS[bbb] == 0)
				return SSS2[bbb];
			else
				return SSS[bbb];

			//thisfinal = thisfinal/per;

			//return thisfinal;

		}



		private double GetMTFSMA(Series<double> SSS, int per, double closesss)
		{
			double thisfinal = 0;

			if (CurrentBars[1] < per - 1)
				return 0;

			for (int i = 0; i < per; i++)
			{
				double baradd = SSS[i];

				if (i == 0 && closesss != 0)
					baradd = closesss;


				thisfinal = thisfinal + baradd;
			}

			thisfinal = thisfinal / per;

			return thisfinal;

		}

		private int thissecondarycb = 0;

		protected override void OnBarUpdate()
		{



			if (!Permission)
				return;



			for (int i = 0; i <= BarsArray.Length - 1; i++) // return for all bars on the chart if there isn't a bar
				if (CurrentBars[i] < 1)
					return;





			//EMAPriceNow1 = Closes[0][0] * constant1 + constant2 * EMAPriceMTF1;
			//EMAPriceNow2 = Closes[0][0] * constant3 + constant4 * EMAPriceMTF2;



			//stochastics fast

			//			double min0 = min[0];
			//			nom[0]		= Close[0] - min0;
			//			den[0]		= max[0] - min0;

			//			if (den[0].ApproxCompare(0) == 0)
			//				fastK[0] = CurrentBar == 0 ? 50 : fastK[1];
			//			else
			//				fastK[0] = Math.Min(100, Math.Max(0, 100 * nom[0] / den[0]));

			//			// Slow %K == Fast %D
			//			K[0] = smaFastK[0];
			//			D[0] = smaK[0];






			//			// shochastics


			//			double min0	= min[0];
			//			nom[0]		= Close[0] - min0;
			//			den[0]		= max[0] - min0;

			//			if (den[0].ApproxCompare(0) == 0)
			//				K[0] = CurrentBar == 0 ? 50 : K[1];
			//			else
			//				K[0] = Math.Min(100, Math.Max(0, 100 * nom[0] / den[0]));

			//			D[0] = smaK[0];






			//				int BZB = 0;				
			//				if (CurrentBars[0] == BarsArray[0].Count - 1)
			//					BZB = 1;			




			double min0 = min[0];
			double max0 = max[0];

			//if (BarsInProgress == 1)

			// messes up plots

			//			if (pMTFEnabled)
			//			if (CurrentBars[1] > 0)
			//			{
			//				MTFData1[0] = MTFData1[1];
			//				MTFData2[0] = MTFData2[1];
			//			}

			//IsMTFClose[0] = IsMTFClose[1];

			//IsMTFClose[0] = 0;


			LastMTFBar[0] = LastMTFBar[1];
			LastMTFSignal[0] = LastMTFSignal[1];

			if (BarsInProgress == 1)
			{


				MTFData1[0] = K[0]; // correct values of K on secondary time frame					
				MTFData2[0] = fastK[0];
				//MTFData3[0] = K[0];



				LastMTFBar[0] = CurrentBars[1];


				//IsMTFClose[0] = 1;

				//if (MTFData1[0] != MTFData1[1])

				//SetBackColor(0, pBrush01);

				if (State == State.Realtime)
					return;


			}


			if (BarsInProgress == 0)
				if (pMTFEnabled)
				{


					if (pStochasticsType == "StochasticsFast")
					{


						// doesn't work for updating on each bar

						//MTFData1[0] = K[0]; // correct values of K on secondary time frame
						//valueofsma = SMA(MTFData1, PeriodD)[0];						




						if (CurrentBars[1] > 3)
						{

							//							Print("MTFData1[0]: " + MTFData1[0]);
							//							Print("MTFData1[1]: " + MTFData1[1]);
							//							Print("MTFData1[2]: " + MTFData1[2]);

							//GetMTFSMA(MTFData1,);
						}





						min0 = min[0];
						max0 = max[0];

						nom[0] = Close[0] - min0;
						den[0] = max0 - min0;

						if (den[0].ApproxCompare(0) == 0)
							K[0] = CurrentBar == 0 ? 50 : K[1];
						else
							K[0] = Math.Min(100, Math.Max(0, 100 * nom[0] / den[0]));

						//						//D[0] = smaK[0];		


						D[0] = GetMTFSMA(MTFData1, FinalPeriodD, K[0]);


						//D[0] = GetMTFSMA(MTFData1, PeriodD, 0);


					}
					else
					{

						min0 = min[0];
						nom[0] = Close[0] - min0;
						den[0] = max[0] - min0;

						if (den[0].ApproxCompare(0) == 0)
							fastK[0] = CurrentBar == 0 ? 50 : fastK[1];
						else
							fastK[0] = Math.Min(100, Math.Max(0, 100 * nom[0] / den[0]));

						// Slow %K == Fast %D


						//K[0] = smaFastK[0];
						K[0] = GetMTFSMA(MTFData2, Smooth, fastK[0]);

						//D[0] = smaK[0];
						D[0] = GetMTFSMA(MTFData1, FinalPeriodD, K[0]);




					}






					//iBestSto.Update();

					// working, jummps from bar to bar

					//					D[0] = iBestSto.D[0];
					//					K[0] = iBestSto.K[0];



				}
				else
				{
					//					D[0] = iBestSto.D[0];
					//					K[0] = iBestSto.K[0];


					if (pStochasticsType == "StochasticsFast")
					{
						min0 = min[0];
						max0 = max[0];

						nom[0] = Close[0] - min0;
						den[0] = max0 - min0;

						if (den[0].ApproxCompare(0) == 0)
							K[0] = CurrentBar == 0 ? 50 : K[1];
						else
							K[0] = Math.Min(100, Math.Max(0, 100 * nom[0] / den[0]));

						D[0] = smaK[0];

					}
					else
					{

						min0 = min[0];
						nom[0] = Close[0] - min0;
						den[0] = max[0] - min0;

						if (den[0].ApproxCompare(0) == 0)
							fastK[0] = CurrentBar == 0 ? 50 : fastK[1];
						else
							fastK[0] = Math.Min(100, Math.Max(0, 100 * nom[0] / den[0]));

						// Slow %K == Fast %D
						K[0] = smaFastK[0];
						D[0] = smaK[0];

					}




				}


			if (CurrentBars[0] == 0)
				return;



			Values[0][0] = D[0];
			Values[1][0] = K[0];




			SignalPlot[0] = K[0];

			UpperLevel = Lines[1].Value;
			LowerLevel = Lines[0].Value;

			UpperArea[0] = Math.Max(UpperLevel, SignalPlot[0]);
			LowerArea[0] = Math.Min(LowerLevel, SignalPlot[0]);
			UpperArea2[0] = 150;
			LowerArea2[0] = -50;


			int starrrrrt = 50;


			if (pAreaOpacity1 != 0 && CurrentBars[0] >= starrrrrt)
			{
				DrawOnPricePanel = false;

				if (!pReverseEnabled)
				{

					Draw.Region(this, ThisName + "-Upper", CurrentBars[0] - starrrrrt, 0, UpperArea, UpperArea2, Brushes.Transparent, pColorDownBrush1, pAreaOpacity1, 0);
					Draw.Region(this, ThisName + "-Lower", CurrentBars[0] - starrrrrt, 0, LowerArea, LowerArea2, Brushes.Transparent, pColorUpBrush1, pAreaOpacity1, 0);


				}
				else
				{

					Draw.Region(this, ThisName + "-Upper", CurrentBars[0] - starrrrrt, 0, UpperArea, UpperArea2, Brushes.Transparent, pColorUpBrush1, pAreaOpacity1, 0);
					Draw.Region(this, ThisName + "-Lower", CurrentBars[0] - starrrrrt, 0, LowerArea, LowerArea2, Brushes.Transparent, pColorDownBrush1, pAreaOpacity1, 0);


				}

			}

			// massive slow down to indicator

			//				if (Trend1[0] == 1) 
			//				{

			//					DrawOnPricePanel = false;
			//					int bb = Math.Min(CurrentBars[0],CurrentBars[0]-(int)LastSwitchBar1[0]+1);

			//					if (pAreaOpacity1 != 0) 
			//					{

			//						//Draw.Region(this,"MAL1"+LastSwitchBar1[0], bb, 0, Values[0], Values[1], null, pColorUpBrush1, pAreaOpacity1);


			//					}



			//				} 
			//				else 
			//				{
			//					DrawOnPricePanel = false;
			//					int bb = Math.Min(CurrentBars[0],CurrentBars[0]-(int)LastSwitchBar1[0]+1);

			//						if (pAreaOpacity1 != 0) 
			//					{

			//					//Draw.Region(this,"MAS1"+LastSwitchBar1[0], bb, 0, Values[0], Values[1], null, pColorDownBrush1, pAreaOpacity1);

			//					}
			//				}


			if (pPlotColorMode == "Extreme")
			{


				if (pReverseEnabled)
				{
					if (SignalPlot[0] < LowerLevel)
						PlotBrushes[1][0] = pColorDownBrush;
					else if (SignalPlot[0] > UpperLevel)
						PlotBrushes[1][0] = pColorUpBrush;
					else
						PlotBrushes[1][0] = pColorNuBrush;
				}
				else
				{
					if (SignalPlot[0] < LowerLevel)
						PlotBrushes[1][0] = pColorUpBrush;
					else if (SignalPlot[0] > UpperLevel)
						PlotBrushes[1][0] = pColorDownBrush;
					else
						PlotBrushes[1][0] = pColorNuBrush;
				}

			}
			else if (pPlotColorMode == "Crossover")
			{



			}
			else if (pPlotColorMode == "Slope")
			{



			}


			for (int i = 0; i <= BarsArray.Length - 1; i++) // return for all bars on the chart if there is only two bars
				if (CurrentBars[i] < 2)
					return;


			Signal[0] = 0;

			//Print(MTFData1[0] + " " +CurrentBars[0]);



			double Bar0 = 0;
			double Bar1 = 0;
			double Bar2 = 0;

			double BarD0 = 0;
			double BarD1 = 0;

			bool SignalMTFOK = true;


			if (Signal[1] != 0)
			{
				LastMTFSignal[0] = LastMTFBar[0];
			}



			if (pMTFEnabled)
			{

				// finish writing so that signals match exactly as seen on the real multi time frame chart data

				//				SignalMTFOK = BarsInProgress == 1 && LastMTFSignal[0] != LastMTFBar[0] && Signal[1] == 0;

				//				BarD0 = D[0];
				//				BarD1 = D[1];

				//				Bar0 = GetMTFData(MTFData1, K, 0);
				//				Bar1 = GetMTFData(MTFData1, K, 1);
				//				Bar2 = GetMTFData(MTFData1, K, 2);

				// intrabar logic for signals

				LastMTFBar[0] = 0;
				LastMTFSignal[0] = 0;

				BarD0 = D[0];
				BarD1 = D[1];

				Bar0 = K[0];
				Bar1 = K[1];
				Bar2 = K[2];

			}
			else
			{
				LastMTFBar[0] = 0;
				LastMTFSignal[0] = 0;

				BarD0 = D[0];
				BarD1 = D[1];

				Bar0 = K[0];
				Bar1 = K[1];
				Bar2 = K[2];
			}



			if (SignalMTFOK)
			{
				if (pSignalType == "Enter Extreme")
				{

					if (Bar0 < LowerLevel && Bar1 >= LowerLevel)
						Signal[0] = 1;

					if (Bar0 > UpperLevel && Bar1 <= UpperLevel)
						Signal[0] = -1;

				}
				if (pSignalType == "Exit Extreme")
				{

					if (Bar0 > LowerLevel && Bar1 <= LowerLevel)
						Signal[0] = 1;

					if (Bar0 < UpperLevel && Bar1 >= UpperLevel)
						Signal[0] = -1;

				}
				if (pSignalType == "Crossover In Extreme")
				{

					if (Bar0 < LowerLevel && Bar0 > BarD0 && Bar1 <= BarD1)
						Signal[0] = 1;

					if (Bar0 > UpperLevel && Bar0 < BarD0 && Bar1 >= BarD1)
						Signal[0] = -1;

					// look at D for qualifying in extreme

					if (BarD0 < LowerLevel && Bar0 > BarD0 && Bar1 <= BarD1)
						Signal[0] = 1;

					if (BarD0 > UpperLevel && Bar0 < BarD0 && Bar1 >= BarD1)
						Signal[0] = -1;


				}
				if (pSignalType == "Slope Change In Extreme")
				{

					if (Bar0 < LowerLevel && Bar0 > Bar1 && Bar2 >= Bar1)
						Signal[0] = 1;

					if (Bar0 > UpperLevel && Bar0 < Bar1 && Bar2 <= Bar1)
						Signal[0] = -1;

					// look at previous bar for qualifying in extreme


				}

				if (Signal[0] != 0)
					LastMTFSignal[0] = LastMTFBar[0];

			}



			//IsMTFClose[0] = 0;





			if (pReverseEnabled && Signal[0] != 0)
				Signal[0] = (Signal[0] * -1);


			Values[2][0] = Signal[0];



			if (pArrowsEnabled)
				if (pMA1Type != "None")
				{
					RemoveDrawObject("LEW1" + CurrentBar.ToString());
					RemoveDrawObject("LEW2" + CurrentBar.ToString());

					double MarkerPrice = 0;

					if (Signal[0] == 1)
					{

						//					DrawOnPricePanel = false;
						//					if (pMA3Type == "Diamond") Draw.Diamond(this, "LEW3"+CurrentBar.ToString(), true, 0, SignalPlot[0]-pVisualOffset3, pColorUpBrush3);
						//					if (pMA3Type == "Dot") Draw.Dot(this, "LEW3"+CurrentBar.ToString(), true, 0, SignalPlot[0]-pVisualOffset3, pColorUpBrush3);
						//					if (pMA3Type == "Triangle") Draw.TriangleUp(this, "LEW3"+CurrentBar.ToString(), true, 0, SignalPlot[0]-pVisualOffset3, pColorUpBrush3);
						//					if (pMA3Type == "Arrow") Draw.ArrowUp(this, "LEW3"+CurrentBar.ToString(), true, 0, SignalPlot[0]-pVisualOffset3, pColorUpBrush3);
						//					if (pMA3Type == "Square") Draw.Square(this, "LEW3"+CurrentBar.ToString(), true, 0, SignalPlot[0]-pVisualOffset3, pColorUpBrush3);

						//DrawOnPricePanel = true;

						MarkerPrice = Lows[0][0] - TickSize * pVisualOffset;

						if (pMA1Type == "Diamond") Draw.Diamond(this, "LEW1" + CurrentBar.ToString(), false, 0, MarkerPrice, pColorUpBrush3, true);
						if (pMA1Type == "Dot") Draw.Dot(this, "LEW1" + CurrentBar.ToString(), false, 0, MarkerPrice, pColorUpBrush3, true);
						if (pMA1Type == "Triangle") Draw.TriangleUp(this, "LEW1" + CurrentBar.ToString(), false, 0, MarkerPrice, pColorUpBrush3, true);
						if (pMA1Type == "Arrow") Draw.ArrowUp(this, "LEW1" + CurrentBar.ToString(), false, 0, MarkerPrice, pColorUpBrush3, true);
						if (pMA1Type == "Square") Draw.Square(this, "LEW1" + CurrentBar.ToString(), false, 0, MarkerPrice, pColorUpBrush3, true);


					}
					if (Signal[0] == -1)
					{
						//					DrawOnPricePanel = false;
						//					if (pMA3Type == "Diamond") Draw.Diamond(this, "LEW3"+CurrentBar.ToString(), true, 0, SignalPlot[0]+pVisualOffset3, pColorDownBrush3);
						//					if (pMA3Type == "Dot") Draw.Dot(this, "LEW3"+CurrentBar.ToString(), true, 0, SignalPlot[0]+pVisualOffset3, pColorDownBrush3);
						//					if (pMA3Type == "Triangle") Draw.TriangleUp(this, "LEW3"+CurrentBar.ToString(), true, 0, SignalPlot[0]+pVisualOffset3, pColorDownBrush3);
						//					if (pMA3Type == "Arrow") Draw.ArrowUp(this, "LEW3"+CurrentBar.ToString(), true, 0, SignalPlot[0]+pVisualOffset3, pColorDownBrush3);
						//					if (pMA3Type == "Square") Draw.Square(this, "LEW3"+CurrentBar.ToString(), true, 0, SignalPlot[0]+pVisualOffset3, pColorDownBrush3);

						//DrawOnPricePanel = true;

						MarkerPrice = Highs[0][0] + TickSize * pVisualOffset;

						if (pMA1Type == "Diamond") Draw.Diamond(this, "LEW1" + CurrentBar.ToString(), false, 0, MarkerPrice, pColorDownBrush3, true);
						if (pMA1Type == "Dot") Draw.Dot(this, "LEW1" + CurrentBar.ToString(), false, 0, MarkerPrice, pColorDownBrush3, true);
						if (pMA1Type == "Triangle") Draw.TriangleDown(this, "LEW1" + CurrentBar.ToString(), false, 0, MarkerPrice, pColorDownBrush3, true);
						if (pMA1Type == "Arrow") Draw.ArrowDown(this, "LEW1" + CurrentBar.ToString(), false, 0, MarkerPrice, pColorDownBrush3, true);
						if (pMA1Type == "Square") Draw.Square(this, "LEW1" + CurrentBar.ToString(), false, 0, MarkerPrice, pColorDownBrush3, true);

					}




				}







			if (K[0] > UpperLevel)
			{
				Trend1[0] = -1;
			}
			else if (K[0] < LowerLevel)
			{
				Trend1[0] = 1;
			}
			else
			{
				Trend1[0] = Trend1[1];

				if (pBarColorNeutralEnabled)
					Trend1[0] = 0;
			}

			if (pBarColorReverseEnabled)
			{
				if (Trend1[0] != 0)
					Trend1[0] = Trend1[0] * -1;
			}





			if (Close[0] > Open[0])
			{
				Direction1[0] = 1;
			}
			else if (Close[0] < Open[0])
			{
				Direction1[0] = -1;
			}
			else
			{
				Direction1[0] = Direction1[1];
			}



			BarBrushes[0] = null;
			CandleOutlineBrushes[0] = null;




			//private string pBackColorMode = "Signals";




			if (pBarColorMode != "None")
			{

				int ThisI = 0;

				if (pBarColorMode == "Trend")
				{

					ThisI = Trend1[0];
				}
				else
				{
					ThisI = Signal[0];

				}


				if (ThisI == 1)
				{
					BarBrushes[0] = (Direction1[0] == ThisI) ? UpBuyBrush : DownBuyBrush;
					CandleOutlineBrushes[0] = pBarUpBrush2;
				}
				else if (ThisI == -1)
				{
					BarBrushes[0] = (Direction1[0] == ThisI) ? DownSellBrush : UpSellBrush;
					CandleOutlineBrushes[0] = pBarDownBrush2;
				}
				else
				{
					BarBrushes[0] = (Direction1[0] == 1) ? UpNeutralBrush : DownNeutralBrush;
					CandleOutlineBrushes[0] = pBarNeutralBrush2;
				}

				if (!BarColorFillEnabled)
					BarBrushes[0] = null;

				if (!BarColorOutlineEnabled)
					CandleOutlineBrushes[0] = null;

			}
			else
			{


			}


			//SetColors2 (BarBrushes1i, CandleOutlineBrushes1i,0,true);	
			//if (pBarColorEnabled)





			if (pBackColorMode != "None")
			{

				int ddd = 0;

				BackBrushesAll[ddd] = null;
				BackBrushes[ddd] = null;


				int ThisI = 0;

				if (pBackColorMode == "Trend")
				{

					ThisI = Trend1[0];
				}
				else
				{
					ThisI = Signal[0];

				}



				if (ThisI == 1)
				{
					SetBackColor(ddd, pBrush01);
				}
				else if (ThisI == -1)
				{
					SetBackColor(ddd, pBrush02);
				}
				//				else if (Signal[0] == 2)
				//				{
				//					SetBackColor(1, pBrush03);
				//				}				
				//				else if (Signal[0] == -2)
				//				{
				//					SetBackColor(1, pBrush04);
				//				}				


			}



			int BB = 0;
			if (Calculate != Calculate.OnBarClose && !pQuickAudio)
				BB = 1;


			if (pAudioEnabled && State != State.Historical)
			{
				if (Signal[BB] == 1 && LastAudioBar != CurrentBars[0])
				{
					Alert(CurrentBar.ToString(), Priority.High, ThisName + " Long Signal | " + Bars.BarsPeriod.ToString(), NinjaTrader.Core.Globals.InstallDir + @"\sounds\" + pWAVFileName, 1, pColorUpBrush2, GetTextColor(pColorUpBrush2));
					LastAudioBar = CurrentBars[0];
				}
				if (Signal[BB] == -1 && LastAudioBar != CurrentBars[0])
				{
					Alert(CurrentBar.ToString(), Priority.High, ThisName + " Short Signal | " + Bars.BarsPeriod.ToString(), NinjaTrader.Core.Globals.InstallDir + @"\sounds\" + pWAVFileName2, 1, pColorDownBrush2, GetTextColor(pColorDownBrush2));
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

					subject = ThisName + "Long Signal | " + Bars.BarsPeriod.ToString() + " Chart | " + Instrument.FullName;
					message = subject;

					if (pEmailAddress != "" && State != State.Historical)
						SendMail(pEmailAddress, subject, message);

					LastEmailBar = CurrentBars[0];
				}

				if (Signal[BB] == -1 && LastEmailBar != CurrentBars[0])
				{
					//subject = Instrument.FullName + " " + BarsPeriod + " Chart  |  " + "New Signal Long " + Close[0].ToString(PriceString);

					subject = ThisName + "Short Signal | " + Bars.BarsPeriod.ToString() + " Chart | " + Instrument.FullName;
					message = subject;

					if (pEmailAddress != "" && State != State.Historical)
						SendMail(pEmailAddress, subject, message);

					LastEmailBar = CurrentBars[0];
				}

			}







		}




		// SetColors2 and AdjustBarColor removed — bar colors now assigned directly in OnBarUpdate



		public Brush GetTextColor(Brush bg2)
		{

			//Color bg = new Pen(bg2,1).;
			Color bg = (bg2 as SolidColorBrush).Color;


			double a = 1 - (0.299 * bg.R + 0.587 * bg.G + 0.114 * bg.B) / 255;
			if (a < 0.5)
				return Brushes.Black;
			else
				return Brushes.White;

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
			}
		}


		protected override void OnRender(ChartControl chartControl, ChartScale chartScale)
		{


			//Print("bug1");


			try
			{



				//	if (!IsInHitTest)
				//if (pVisualEnabled)			
				//	base.OnRender(chartControl, chartScale);




				oldAntialiasMode = RenderTarget.AntialiasMode;


				if (FirstRender2)
				{

					//	ChartBarsSwitch2(true);


					myProperties = chartControl.Properties;
					//	PreviousDrag = myProperties.AllowSelectionDragging;





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
						ChartBackgroundErrorBrushDX.Opacity = 25 / 100f;

						CenterText = new SimpleFont(ChartControl.Properties.LabelFont.Family.ToString(), 16).ToDirectWriteTextFormat();
						CenterText.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
						CenterText.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
						CenterText.WordWrapping = SharpDX.DirectWrite.WordWrapping.Wrap;

						//CellFormat = FinalFont1.ToDirectWriteTextFormat();

						CenterRect = new SharpDX.RectangleF(ChartPanel.X, ChartPanel.Y, ChartPanel.W, ChartPanel.H);





						RenderTarget.AntialiasMode = SharpDX.Direct2D1.AntialiasMode.Aliased;

						string txt = string.Empty;

						foreach (string sss in AllErrorMessages)
							txt = txt + sss + "\r\n\r\n";

						txt = txt + "Click here to continue.";
						//Print(text);
						RenderTarget.FillRectangle(CenterRect, ChartBackgroundBrushDX);
						RenderTarget.FillRectangle(CenterRect, ChartBackgroundErrorBrushDX);
						RenderTarget.DrawText(txt, CenterText, ExpandRect(CenterRect, -10, 0), ChartTextBrushDX);


						RenderTarget.AntialiasMode = oldAntialiasMode;



						ChartBackgroundErrorBrushDX?.Dispose();
						CenterText?.Dispose();

						//Print("bug2");

						return;
					}










				if (!Permission)
					return;







				if (!IsVisible)
					return;

				base.OnRender(chartControl, chartScale);



				int FB = ChartBars.FromIndex;
				int LB = ChartBars.ToIndex;
				int BB = 0;
				int xt = 0;
				int yt = 0;
				double yt2 = 0;

				//if (Calculate.OnBarClose)
				LB = Math.Min(CurrentBars[0], LB);
				BB = CurrentBars[0] - LB;


				//Print(FB + "  " + LB + "  " + BB);

				//Print(Lows[0][0]);
				//return;		





				if (Bars == null || Bars.Instrument == null || IsInHitTest) { return; }




				// START ARROWS

				//SharpDX.Direct2D1.AntialiasMode oldAntialiasMode = RenderTarget.AntialiasMode;


				if (pSignalType != "None")
					if (pArrowsEnabled)
					{

						SharpDX.Direct2D1.Brush longBrushDX = null;
						SharpDX.Direct2D1.Brush shortBrushDX = null;
						SharpDX.Direct2D1.Brush arrowBrushDX = null;
						TextFormat LabelTextFormat = null;
						TextLayout LabelTextLayout = null;
						SharpDX.Direct2D1.Brush LabelBrushDX = null;

						try
						{
							longBrushDX = pArrowUpFBrush.ToDxBrush(RenderTarget);
							shortBrushDX = pArrowDownFBrush.ToDxBrush(RenderTarget);

							Stroke ThisStroke = pArrowDownStroke;

							//            int FB = ChartBars.FromIndex;
							//            int LB = ChartBars.ToIndex;
							//            int BB = 0;
							//            int xt = 0;
							//            int yt = 0;
							//            double yt2 = 0;

							//            LB = Math.Min(CurrentBars[0], LB);
							//            BB = CurrentBars[0] - LB;

							// ARROWS

							LabelTextFormat = pTextFont.ToDirectWriteTextFormat();

							LabelTextLayout = new TextLayout(NinjaTrader.Core.Globals.DirectWriteFactory, "", LabelTextFormat, 1000, LabelTextFormat.FontSize);

							LabelTextFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
							LabelTextFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
							LabelTextFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;

							LabelBrushDX = ChartControl.Properties.ChartText.ToDxBrush(RenderTarget);
							Point TextPoint = new Point(0, 0);


							ChartPanel chartPanel = chartControl.ChartPanels[chartScale.PanelIndex];
							SharpDX.Direct2D1.PathGeometry arrowGeo = null;

							if (ChartBars != null)
								for (int i = FB; i <= LB; i++)
								{
									// Safety check for bounds
									if (i < 0 || i >= ChartBars.Count)
										continue;

									BB = i;

									// Safety check for Signal series bounds
									if (BB < 0 || !Signal.IsValidDataPointAt(BB))
										continue;

									double ThisSignal22 = Signal.GetValueAt(BB);

									if (ThisSignal22 != 0)
									{

										// Safety check for SignalPlot bounds
										if (!SignalPlot.IsValidDataPointAt(BB))
											continue;

										xt = chartControl.GetXByBarIndex(ChartBars, i);

										int pTextOffset = 0;
										string lb = string.Empty;
										float newy = 0;
										float newx = 0;
										float totalarrowheight = pArrowOffset + pArrowSize + pArrowBarHeight;



										if (ThisSignal22 == 1)
										{

											yt = chartScale.GetYByValue(SignalPlot.GetValueAt(BB));
											yt2 = chartScale.GetYByValueWpf(SignalPlot.GetValueAt(BB));
											arrowBrushDX = longBrushDX;
											ThisStroke = pArrowUpStroke;

										}

										if (ThisSignal22 == -1)
										{
											yt = chartScale.GetYByValue(SignalPlot.GetValueAt(BB));
											yt2 = chartScale.GetYByValueWpf(SignalPlot.GetValueAt(BB));
											arrowBrushDX = shortBrushDX;
											ThisStroke = pArrowDownStroke;

										}

										arrowGeo = CreateArrowGeometry(chartControl, chartPanel, chartScale, xt, yt, Signal.GetValueAt(BB));

										if (arrowGeo != null)
										{
											RenderTarget.AntialiasMode = SharpDX.Direct2D1.AntialiasMode.PerPrimitive;

											RenderTarget.FillGeometry(arrowGeo, arrowBrushDX);
											RenderTarget.DrawGeometry(arrowGeo, ThisStroke.BrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);

											arrowGeo.Dispose();
											arrowGeo = null;
										}




										LabelTextLayout?.Dispose();
										LabelTextLayout = new TextLayout(NinjaTrader.Core.Globals.DirectWriteFactory, lb, LabelTextFormat, 1000, 1000);

										float boxpadding = LabelTextFormat.FontSize;


										float RectWidth = LabelTextLayout.Metrics.Width + (float)pTextFont.Size;
										float RectHeight = LabelTextLayout.Metrics.Height + (float)pTextFont.Size / 2f + 1;

										if (ThisSignal22 == 1)
										{
											lb = pLabelBuy;
											newy = yt + totalarrowheight + 1 + pTextOffset;

										}

										if (ThisSignal22 == -1)
										{
											lb = pLabelSell;
											newy = yt - totalarrowheight - RectHeight - 1 - pTextOffset;

										}

										newx = xt - RectWidth / 2 - 2;

										TextPoint = new Point(newx, newy);




										SharpDX.RectangleF TextRect = new SharpDX.RectangleF(newx, newy, RectWidth, RectHeight);

										//							{

										//								RenderTarget.AntialiasMode							= SharpDX.Direct2D1.AntialiasMode.Aliased;
										//								RenderTarget.DrawRectangle(rect2, pBrush08.BrushDX, pBrush08.Width, pBrush08.StrokeStyle);
										//								RenderTarget.AntialiasMode							= SharpDX.Direct2D1.AntialiasMode.PerPrimitive;

										//							}



										if (pLabelsEnabled)
											RenderTarget.DrawText(lb, LabelTextFormat, TextRect, LabelBrushDX);

										RenderTarget.AntialiasMode = SharpDX.Direct2D1.AntialiasMode.Aliased;

									}
								}
						}
						catch (Exception ex)
						{
							// Log the exception if needed
							Print("OnRender Exception: " + ex.Message);
						}
						finally
						{
							// Dispose all resources
							longBrushDX?.Dispose();
							shortBrushDX?.Dispose();
							LabelTextFormat?.Dispose();
							LabelTextLayout?.Dispose();
							LabelBrushDX?.Dispose();
						}
					}




				RenderTarget.AntialiasMode = oldAntialiasMode;

				// END ARROWS

			}
			catch (Exception ex)
			{
				// Log the main exception
				Print("OnRender Main Exception: " + ex.Message);
			}
			finally
			{
				// Dispose main resources
				ChartTextBrushDX?.Dispose();
				ChartBackgroundBrushDX?.Dispose();
			}
		}


		private SharpDX.Direct2D1.PathGeometry CreateArrowGeometry(ChartControl chartControl, ChartPanel chartPanel, ChartScale chartScale, int xt, double yt2, int dir)
		{


			Point startPoint = new Point(0, 0);
			Point midPoint = new Point(100, 0);
			Point endPoint = new Point(100, 100);

			float aw = pArrowSize;
			float aw2 = pArrowBarWidth; // bar w
			float barh = pArrowBarHeight;
			float offset = pArrowOffset;

			float yt2f = (float)yt2;

			xt = xt - 1; // adjust arrow to left 1 pixel so it lines up. bug?

			aw2 = Math.Min(aw2, aw);

			SharpDX.Vector2 tipPoint2 = new SharpDX.Vector2(0, 0);
			SharpDX.Vector2 triLeftPoint2 = new SharpDX.Vector2(0, 0);
			SharpDX.Vector2 triRightPoint2 = new SharpDX.Vector2(0, 0);
			SharpDX.Vector2 barLeftPoint12 = new SharpDX.Vector2(0, 0);
			SharpDX.Vector2 barRightPoint12 = new SharpDX.Vector2(0, 0);
			SharpDX.Vector2 barLeftPoint22 = new SharpDX.Vector2(0, 0);
			SharpDX.Vector2 barRightPoint22 = new SharpDX.Vector2(0, 0);

			float po = 0;

			if (dir == -1)
			{
				//yt = yt - offset;
				yt2f = yt2f - offset;

				tipPoint2 = new SharpDX.Vector2(xt, yt2f);
				triLeftPoint2 = new SharpDX.Vector2(xt - aw, yt2f - aw);
				triRightPoint2 = new SharpDX.Vector2(xt + aw, yt2f - aw);
				barLeftPoint12 = new SharpDX.Vector2(xt - aw2 + po, yt2f - aw);
				barRightPoint12 = new SharpDX.Vector2(xt + aw2 + po, yt2f - aw);
				barLeftPoint22 = new SharpDX.Vector2(xt - aw2 + po, yt2f - (aw + barh));
				barRightPoint22 = new SharpDX.Vector2(xt + aw2 + po, yt2f - (aw + barh));
			}
			else
			{
				//yt = yt + offset;
				yt2f = yt2f + offset;

				tipPoint2 = new SharpDX.Vector2(xt, yt2f);
				triLeftPoint2 = new SharpDX.Vector2(xt - aw, yt2f + aw);
				triRightPoint2 = new SharpDX.Vector2(xt + aw, yt2f + aw);
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





		//		private int pAScale = 90;
		//        [Range(0, 100000)]
		//        [Display(ResourceType = typeof(Custom.Resource), Name = "Min / Max Scale", GroupName = "Plots", Order = 20)]
		//        public int AScale
		//        {
		//            get { return pAScale; }
		//            set { pAScale = value; }
		//        }

		public override void OnCalculateMinMax()
		{

			MaxValue = 110;
			MinValue = -10;



		}




		public override string DisplayName
		{
			get
			{
				//					if (State == State.SetDefaults)
				//						return ThisName + "(a)";
				//					else
				//						return Name  + "(" + PeriodD + "," + PeriodK + "," + Smooth + ")";


				string finalname = string.Empty;

				if (pStochasticsType == "StochasticsFast")
					finalname = ThisName + " (Fast " + PeriodDFast + "," + PeriodKFast + ")";
				else
					finalname = ThisName + " (" + PeriodD + "," + PeriodK + "," + Smooth + ")";


				if (pMTFEnabled)
				{
					//finalname = finalname + " " + BarsArray[1].BarsType.DisplayName;				
					finalname = finalname + " " + pMTFBarsPeriod1.ToString() + " " + pMTFBasePeriodType1;
				}

				return finalname;

			}

		}



		[Range(1, int.MaxValue), NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Period D", GroupName = "Parameters ", Order = 10)]
		public int PeriodDFast
		{ get; set; }

		[Range(1, int.MaxValue), NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Period K", GroupName = "Parameters ", Order = 11)]
		public int PeriodKFast
		{ get; set; }


		[Range(1, int.MaxValue), NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Period D", GroupName = "Parameters ", Order = 10)]
		public int PeriodD
		{ get; set; }

		[Range(1, int.MaxValue), NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Period K", GroupName = "Parameters ", Order = 11)]
		public int PeriodK
		{ get; set; }

		[Range(1, int.MaxValue), NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Smooth", GroupName = "Parameters ", Order = 12)]
		public int Smooth
		{ get; set; }




		private string pStochasticsType = "StochasticsFast";
		[Description("")]
		[Display(Name = "Mode", Description = "", GroupName = "Parameters ", Order = 3)]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(TotalMode456789))]
		[NinjaScriptProperty]
		public string StochasticsType
		{
			get { return pStochasticsType; }
			set { pStochasticsType = value; }
		}




		internal class TotalMode456789 : StringConverter
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
				return new StandardValuesCollection(new String[] { "Stochastics", "StochasticsFast" });
			}
		}





		private Brush pColorUpBrush = Brushes.DodgerBlue;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "K Color Up", GroupName = "Plots", Order = 101)]
		public Brush ColorUpBrush
		{
			get { return pColorUpBrush; }
			set { pColorUpBrush = value; }
		}
		[Browsable(false)]
		public string ColorUpBrushS
		{
			get { return Serialize.BrushToString(pColorUpBrush); }
			set { pColorUpBrush = Serialize.StringToBrush(value); }
		}


		private Brush pColorNuBrush = Brushes.Silver;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "K Color Neutral", GroupName = "Plots", Order = 102)]
		public Brush ColorNuBrush
		{
			get { return pColorNuBrush; }
			set { pColorNuBrush = value; }
		}
		[Browsable(false)]
		public string ColorNuBrushS
		{
			get { return Serialize.BrushToString(pColorNuBrush); }
			set { pColorNuBrush = Serialize.StringToBrush(value); }
		}


		private Brush pColorDownBrush = Brushes.Red;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "K Color Down", GroupName = "Plots", Order = 103)]
		public Brush ColorDownBrush
		{
			get { return pColorDownBrush; }
			set { pColorDownBrush = value; }
		}
		[Browsable(false)]
		public string ColorDownBrushS
		{
			get { return Serialize.BrushToString(pColorDownBrush); }
			set { pColorDownBrush = Serialize.StringToBrush(value); }
		}


		//        private DashStyleHelper pDashS2  = DashStyleHelper.Solid;
		//        [Display(ResourceType = typeof(Custom.Resource), Name = "K Dash style", GroupName = "Plots", Order = 105)]
		//        public DashStyleHelper DashS2
		//        { get; set; }		

		//		private int pWidthPlot2 = 1;
		//		[Display(ResourceType = typeof(Custom.Resource), Name = "K Width", GroupName = "Plots", Order = 106)]
		//		[Range(0, int.MaxValue), NinjaScriptProperty]
		//		public int WidthPlot2
		//		{
		//			get { return pWidthPlot2; }
		//			set { pWidthPlot2 = value; }
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
		


		private Brush pColorUpBrush2 = Brushes.DodgerBlue;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Alert Log Buy", GroupName = "Audio", Order = 8)]
		public Brush ColorUpBrush2
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


		private Brush pColorDownBrush2 = Brushes.Red;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Alert Log Sell", GroupName = "Audio", Order = 10)]
		public Brush ColorDownBrush2
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






		private bool pMTFEnabled = false;
		[Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", GroupName = "Multi Time Frame", Order = 0)]
		[RefreshProperties(RefreshProperties.All)]
		[NinjaScriptProperty]
		public bool MTFEnabled
		{
			get { return pMTFEnabled; }
			set { pMTFEnabled = value; }
		}

		private string pMTFInstrument = "";
		//		[Description("")]
		//		[Display(Name = "Instrument", Description = "", GroupName = "Multi Time Frame", Order = 1)]
		//		public string MTFInstrument
		//		{
		//			get { return pMTFInstrument; }
		//			set { pMTFInstrument = value; }
		//		}	

		private BarsPeriodType FinalBasePeriodType1 = BarsPeriodType.Minute;
		private string pMTFBasePeriodType1 = "Minute";
		[NinjaScriptProperty]
		[Description("")]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Bars Type", Description = "", GroupName = "Multi Time Frame", Order = 12)]
		[TypeConverter(typeof(AllMTF))]
		public string MTFBasePeriodType1
		{
			get { return pMTFBasePeriodType1; }
			set
			{
				pMTFBasePeriodType1 = value;

				switch (pMTFBasePeriodType1)
				{
					case "Tick": FinalBasePeriodType1 = BarsPeriodType.Tick; break;
					case "Volume": FinalBasePeriodType1 = BarsPeriodType.Volume; break;
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
				return new StandardValuesCollection(new String[] { "Tick", "Volume", "Range", "Second", "Minute", "Renko" });
			}
		}



		private int pMTFBarsPeriod1 = 5;
		[Range(1, int.MaxValue)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Bars Period", GroupName = "Multi Time Frame", Order = 22)]
		[NinjaScriptProperty]
		public int MTFBarsPeriod1
		{
			get { return pMTFBarsPeriod1; }
			set { pMTFBarsPeriod1 = value; }
		}




		private Brush pColorUpBrush1 = Brushes.DodgerBlue;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Area Color Buy", GroupName = "Lines", Order = 110)]
		public Brush ColorUpBrush1
		{
			get { return pColorUpBrush1; }
			set { pColorUpBrush1 = value; }
		}
		[Browsable(false)]
		public string ColorUpBrush1S
		{
			get { return Serialize.BrushToString(pColorUpBrush1); }
			set { pColorUpBrush1 = Serialize.StringToBrush(value); }
		}

		private Brush pColorDownBrush1 = Brushes.Red;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Area Color Sell", GroupName = "Lines", Order = 120)]
		public Brush ColorDownBrush1
		{
			get { return pColorDownBrush1; }
			set { pColorDownBrush1 = value; }
		}
		[Browsable(false)]
		public string ColorDownBrush1S
		{
			get { return Serialize.BrushToString(pColorDownBrush1); }
			set { pColorDownBrush1 = Serialize.StringToBrush(value); }
		}




		private int pAreaOpacity1 = 10;
		[Range(0, 100)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Area Opacity (%)", GroupName = "Lines", Order = 130)]
		public int AreaOpacity1
		{
			get { return pAreaOpacity1; }
			set { pAreaOpacity1 = value; }
		}



		private string pPlotColorMode = "Extreme";
		//		[Description("")]
		//		[Display(Name = "K Color Mode", Description = "", GroupName = "Plots", Order = -10)]
		//		[RefreshProperties(RefreshProperties.All)]
		//		[TypeConverter(typeof(AllSignalTypes))]
		//		public string PlotColorMode
		//		{
		//			get { return pPlotColorMode; }
		//			set { pPlotColorMode = value; }
		//		}




		internal class PlotColorModes : StringConverter
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
				return new StandardValuesCollection(new String[] { "Crossover", "Extreme", "Slope" });
			}
		}








		// ARROW INPUTS

		private bool pArrowsEnabled = true;
		//        [Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", Description = "", GroupName = "Signals", Order = -20)]
		//		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
		//        public bool ArrowsEnabled
		//        {
		//            get { return pArrowsEnabled; }
		//            set { pArrowsEnabled = value; }
		//        }




		private string pSignalType = "Enter Extreme";
		[Description("")]
		[Display(Name = "Enabled", Description = "", GroupName = "Signals", Order = -10)]
		[RefreshProperties(RefreshProperties.All)]
		[NinjaScriptProperty]
		[TypeConverter(typeof(AllSignalTypes))]
		public string SignalType
		{
			get { return pSignalType; }
			set { pSignalType = value; }
		}




		internal class AllSignalTypes : StringConverter
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
				return new StandardValuesCollection(new String[] { "None", "Enter Extreme", "Exit Extreme", "Crossover In Extreme", "Slope Change In Extreme" });
			}
		}



		private bool pReverseEnabled = false;
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Reverse", Description = "take the opposite direction of the standard signal.", GroupName = "Signals", Order = 0)]
		public bool ReverseEnabled
		{
			get { return pReverseEnabled; }
			set { pReverseEnabled = value; }
		}




		private float pArrowSize = 8;
		[Range(0, 1000)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Arrow Size", Description = "", GroupName = "Signals", Order = 1)]
		public float ArrowSize
		{
			get { return pArrowSize; }
			set { pArrowSize = value; }
		}

		private float pArrowBarHeight = 0;
		[Range(0, 1000)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Arrow Bar Height", Description = "", GroupName = "Signals", Order = 2)]
		public float ArrowBarHeight
		{
			get { return pArrowBarHeight; }
			set { pArrowBarHeight = value; }
		}

		private float pArrowBarWidth = 2;
		[Range(0, 1000)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Arrow Bar Width", Description = "", GroupName = "Signals", Order = 3)]
		public float ArrowBarWidth
		{
			get { return pArrowBarWidth; }
			set { pArrowBarWidth = value; }
		}

		private float pArrowOffset = 10;
		[Range(0, 1000)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Offset (Pixels)", Description = "", GroupName = "Signals", Order = 4)]
		public float ArrowOffset
		{
			get { return pArrowOffset; }
			set { pArrowOffset = value; }
		}






		private Brush pArrowUpFBrush = Brushes.DodgerBlue;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Buy Fill", Description = "", GroupName = "Signals", Order = 20)]
		public Brush ArrowUpFBrush
		{
			get { return pArrowUpFBrush; }
			set { pArrowUpFBrush = value; }
		}
		[Browsable(false)]
		public string ArrowUpFBrushS
		{
			get { return Serialize.BrushToString(pArrowUpFBrush); }
			set { pArrowUpFBrush = Serialize.StringToBrush(value); }
		}

		private Brush pArrowDownFBrush = Brushes.Red;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Sell Fill", Description = "", GroupName = "Signals", Order = 22)]
		public Brush ArrowDownFBrush
		{
			get { return pArrowDownFBrush; }
			set { pArrowDownFBrush = value; }
		}
		[Browsable(false)]
		public string ArrowDownFBrushS
		{
			get { return Serialize.BrushToString(pArrowDownFBrush); }
			set { pArrowDownFBrush = Serialize.StringToBrush(value); }
		}

		private Stroke pArrowUpStroke = new Stroke(Brushes.MidnightBlue, DashStyleHelper.Solid, 1);
		[Display(ResourceType = typeof(Custom.Resource), Name = "Buy Outline", Description = "", GroupName = "Signals", Order = 21)]
		public Stroke ArrowUpStroke
		{
			get { return pArrowUpStroke; }
			set { pArrowUpStroke = value; }
		}

		private Stroke pArrowDownStroke = new Stroke(Brushes.Maroon, DashStyleHelper.Solid, 1);
		[Display(ResourceType = typeof(Custom.Resource), Name = "Sell Outline", Description = "", GroupName = "Signals", Order = 23)]
		public Stroke ArrowDownStroke
		{
			get { return pArrowDownStroke; }
			set { pArrowDownStroke = value; }
		}







		private string pMA1Type = "None";
		[Description("")]
		[Display(Name = "Price Marker", Description = "", GroupName = "Signals", Order = 100)]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(MarkerType))]
		public string MA1Type
		{
			get { return pMA1Type; }
			set { pMA1Type = value; }
		}

		//		private string pMA2Type = "Dot";
		//		[Description("")]
		//		[Display(Name = "Indicator Marker", Description = "", GroupName = "Signals", Order = 3)]
		//		[RefreshProperties(RefreshProperties.All)]
		//		[TypeConverter(typeof(TotalMode))]
		//		public string MA2Type
		//		{
		//			get { return pMA2Type; }
		//			set { pMA2Type = value; }
		//		}



		private int pVisualOffset = 2;
		[Display(ResourceType = typeof(Custom.Resource), Name = "Price Marker Offset (Ticks)", GroupName = "Signals", Order = 101)]
		[Range(0, int.MaxValue)]
		public int VisualOffset
		{
			get { return pVisualOffset; }
			set { pVisualOffset = value; }
		}

		//		private int pVisualOffset2 = 0;
		//		[Display(ResourceType = typeof(Custom.Resource), Name = "Indicator Offset", GroupName = "Signals", Order = 4)]
		//		[Range(0, int.MaxValue), NinjaScriptProperty]
		//		public int VisualOffset2
		//		{
		//			get { return pVisualOffset2; }
		//			set { pVisualOffset2 = value; }
		//		}	

		//None, Arrow, Diamond, Dot, Square, Triangle


		internal class MarkerType : StringConverter
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
				return new StandardValuesCollection(new String[] { "None", "Arrow", "Diamond", "Dot", "Square", "Triangle" });
			}
		}


		private Brush pColorUpBrush3 = Brushes.DodgerBlue;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Price Marker Buy", GroupName = "Signals", Order = 200)]
		public Brush ColorUpBrush3
		{
			get { return pColorUpBrush3; }
			set { pColorUpBrush3 = value; }
		}
		[Browsable(false)]
		public string ColorUpBrush3S
		{
			get { return Serialize.BrushToString(pColorUpBrush3); }
			set { pColorUpBrush3 = Serialize.StringToBrush(value); }
		}


		private Brush pColorDownBrush3 = Brushes.Red;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Price Marker Sell", GroupName = "Signals", Order = 201)]
		public Brush ColorDownBrush3
		{
			get { return pColorDownBrush3; }
			set { pColorDownBrush3 = value; }
		}
		[Browsable(false)]
		public string ColorDownBrush3S
		{
			get { return Serialize.BrushToString(pColorDownBrush3); }
			set { pColorDownBrush3 = Serialize.StringToBrush(value); }
		}







		//		private Brush pArrowUpOBrush	= Brushes.DarkGreen;
		//		[XmlIgnore]
		//		[Display(ResourceType = typeof(Custom.Resource), Name = "Color Up (Outline)", Desciption = "", GroupName = "Signals", Order = 20)]
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
		//		[Display(ResourceType = typeof(Custom.Resource), Name = "Color Down (Outline)", Desciption = "", GroupName = "Signals", Order = 20)]
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

		private bool pLabelsEnabled = false;
		[Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", Description = "", GroupName = "Labels", Order = 1)]
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
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



		private string pBackColorMode = "Signals";
		[Description("")]
		[Display(Name = "Enabled", Description = "", GroupName = "Background Color", Order = -100)]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(ColorType))]
		public string BackColorMode
		{
			get { return pBackColorMode; }
			set { pBackColorMode = value; }
		}


		private string pBarColorMode = "Trend";
		[Description("")]
		[Display(Name = "Enabled", Description = "", GroupName = "Bar Color", Order = -100)]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(ColorType))]
		public string BarColorMode
		{
			get { return pBarColorMode; }
			set { pBarColorMode = value; }
		}

		internal class ColorType : StringConverter
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
				return new StandardValuesCollection(new String[] { "None", "Signals", "Trend" });
			}
		}







		//  private bool pBackEnabled = true;
		//        [Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", Description = "", GroupName = "Background Color", Order = 1)]
		//        public bool BackEnabled
		//        {
		//            get { return pBackEnabled; }
		//            set { pBackEnabled = value; }
		//        }	

		private bool pColorAll = true;
		[Display(ResourceType = typeof(Custom.Resource), Name = "Color All", Description = "", GroupName = "Background Color", Order = 2)]
		public bool ColorAll
		{
			get { return pColorAll; }
			set { pColorAll = value; }
		}


		// BUY COLOR

		private System.Windows.Media.Brush pBrush01 = Brushes.DodgerBlue;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Buy Color", Description = "", GroupName = "Background Color", Order = 3)]
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

		private int pOpacity01 = 10;
		[Range(0, 100)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Buy Opacity (%)", Description = "", GroupName = "Background Color", Order = 4)]
		public int Opacity01
		{
			get { return pOpacity01; }
			set
			{
				pOpacity01 = Math.Max(0, Math.Min(100, value));
				if (pBrush01 != null)
				{
					System.Windows.Media.Brush newBrush = pBrush01.Clone();
					newBrush.Opacity = pOpacity01 / 100d;
					newBrush.Freeze();
					pBrush01 = newBrush;
				}
			}
		}

		// SELL COLOR

		private System.Windows.Media.Brush pBrush02 = Brushes.Red;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Sell Color", Description = "", GroupName = "Background Color", Order = 5)]
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

		private int pOpacity02 = 10;
		[Range(0, 100)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Sell Opacity (%)", Description = "", GroupName = "Background Color", Order = 6)]
		public int Opacity02
		{
			get { return pOpacity02; }
			set
			{
				pOpacity02 = Math.Max(0, Math.Min(100, value));
				if (pBrush02 != null)
				{
					System.Windows.Media.Brush newBrush = pBrush02.Clone();
					newBrush.Opacity = pOpacity02 / 100d;
					newBrush.Freeze();
					pBrush02 = newBrush;
				}
			}
		}







		private bool pBarColorNeutralEnabled = true;
		[Display(ResourceType = typeof(Custom.Resource), Name = "Neutral Status", Description = "", GroupName = "Trend", Order = 0)]
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
		public bool BarColorNeutralEnabled
		{
			get { return pBarColorNeutralEnabled; }
			set { pBarColorNeutralEnabled = value; }
		}


		private bool pBarColorReverseEnabled = false;
		[Display(ResourceType = typeof(Custom.Resource), Name = "Reverse", Description = "", GroupName = "Trend", Order = 0)]
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
		public bool BarColorReverseEnabled
		{
			get { return pBarColorReverseEnabled; }
			set { pBarColorReverseEnabled = value; }
		}




		//        private bool pBarColorEnabled = true;
		//        [Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", Description = "", GroupName = "Bar Color", Order = 0)]
		//		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
		//        public bool BarColorEnabled
		//        {
		//            get { return pBarColorEnabled; }
		//            set { pBarColorEnabled = value; }
		//        }	



		private bool pBarColorFillEnabled = true;
		[Display(ResourceType = typeof(Custom.Resource), Name = "Fill Enabled", Description = "", GroupName = "Bar Color", Order = 1)]
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
		public bool BarColorFillEnabled
		{
			get { return pBarColorFillEnabled; }
			set { pBarColorFillEnabled = value; }
		}

		private bool pBarColorOutlineEnabled = true;
		[Display(ResourceType = typeof(Custom.Resource), Name = "Outline Enabled", Description = "", GroupName = "Bar Color", Order = 1)]
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
		public bool BarColorOutlineEnabled
		{
			get { return pBarColorOutlineEnabled; }
			set { pBarColorOutlineEnabled = value; }
		}



		private Brush pBarUpBrush1 = Brushes.DodgerBlue;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Buy Fill", GroupName = "Bar Color", Order = 11)]
		public Brush BarUpBrush1
		{
			get { return pBarUpBrush1; }
			set { pBarUpBrush1 = value; }
		}
		[Browsable(false)]
		public string BarUpBrush1S
		{
			get { return Serialize.BrushToString(pBarUpBrush1); }
			set { pBarUpBrush1 = Serialize.StringToBrush(value); }
		}

		private Brush pBarUpBrush2 = Brushes.DodgerBlue;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Buy Outline", GroupName = "Bar Color", Order = 12)]
		public Brush BarUpBrush2
		{
			get { return pBarUpBrush2; }
			set { pBarUpBrush2 = value; }
		}
		[Browsable(false)]
		public string BarUpBrush2S
		{
			get { return Serialize.BrushToString(pBarUpBrush2); }
			set { pBarUpBrush2 = Serialize.StringToBrush(value); }
		}


		private Brush pBarDownBrush1 = Brushes.DarkRed;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Sell Fill", GroupName = "Bar Color", Order = 13)]
		public Brush BarDownBrush1
		{
			get { return pBarDownBrush1; }
			set { pBarDownBrush1 = value; }
		}
		[Browsable(false)]
		public string BarDownBrush1S
		{
			get { return Serialize.BrushToString(pBarDownBrush1); }
			set { pBarDownBrush1 = Serialize.StringToBrush(value); }
		}

		private Brush pBarDownBrush2 = Brushes.DarkRed;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Sell Outline", GroupName = "Bar Color", Order = 14)]
		public Brush BarDownBrush2
		{
			get { return pBarDownBrush2; }
			set { pBarDownBrush2 = value; }
		}
		[Browsable(false)]
		public string BarDownBrush2S
		{
			get { return Serialize.BrushToString(pBarDownBrush2); }
			set { pBarDownBrush2 = Serialize.StringToBrush(value); }
		}



		private Brush pBarNeutralBrush1 = Brushes.DimGray;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Neutral Fill", GroupName = "Bar Color", Order = 15)]
		public Brush BarNeutralBrush1
		{
			get { return pBarNeutralBrush1; }
			set { pBarNeutralBrush1 = value; }
		}
		[Browsable(false)]
		public string BarNeutralBrush1S
		{
			get { return Serialize.BrushToString(pBarNeutralBrush1); }
			set { pBarNeutralBrush1 = Serialize.StringToBrush(value); }
		}

		private Brush pBarNeutralBrush2 = Brushes.DimGray;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Neutral Outline", GroupName = "Bar Color", Order = 16)]
		public Brush BarNeutralBrush2
		{
			get { return pBarNeutralBrush2; }
			set { pBarNeutralBrush2 = value; }
		}
		[Browsable(false)]
		public string BarNeutralBrush2S
		{
			get { return Serialize.BrushToString(pBarNeutralBrush2); }
			set { pBarNeutralBrush2 = Serialize.StringToBrush(value); }
		}



		private int pBarOpacityUp = 20;
		[Display(ResourceType = typeof(Custom.Resource), Name = "Up Bar Opacity (%)", GroupName = "Bar Color", Order = 40)]
		[Range(0, 100)]
		public int BarOpacityUp
		{
			get { return pBarOpacityUp; }
			set { pBarOpacityUp = value; }
		}


		private int pBarOpacityDown = 80;
		[Display(ResourceType = typeof(Custom.Resource), Name = "Down Bar Opacity (%)", GroupName = "Bar Color", Order = 41)]
		[Range(0, 100)]
		public int BarOpacityDown
		{
			get { return pBarOpacityDown; }
			set { pBarOpacityDown = value; }
		}

		//		private int pBarOpacitySignal = 80; 
		//		[Display(ResourceType = typeof(Custom.Resource), Name = "Signal Opacity (%)", GroupName = "Trend", Order = 42)]
		//		[Range(0, 100)]
		//		public int BarOpacitySignal
		//		{
		//			get { return pBarOpacitySignal; }
		//			set { pBarOpacitySignal = value; }
		//		}				






		private string pLicensingEmailAddress = "";
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "License", Name = "Email Address", Order = 54, Description = "")]
		public string LicensingEmailAddress
		{
			get { return pLicensingEmailAddress; }
			set { pLicensingEmailAddress = value; }
		}








		private void SetBackColor(int i, Brush BB)
		{

			if (pColorAll)
				BackBrushesAll[i] = BB;
			else
				BackBrushes[i] = BB;
		}





	}






	// Hide UserDefinedValues properties when not in use by the HLCCalculationMode.UserDefinedValues
	// When creating a custom type converter for indicators it must inherit from NinjaTrader.NinjaScript.IndicatorBaseConverter to work correctly with indicators
	public class aiBestStochasticsConverter : NinjaTrader.NinjaScript.IndicatorBaseConverter
	{
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) { return true; }

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			PropertyDescriptorCollection propertyDescriptorCollection = base.GetPropertiesSupported(context) ? base.GetProperties(context, value, attributes) : TypeDescriptor.GetProperties(value, attributes);

			aiBestStochastics jbb = (aiBestStochastics)value;

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










			// 		    if (!jbb.BarPaintEnabled)
			//			{	
			//				DeleteThese.Add("ClickUPColor");	
			//				DeleteThese.Add("ClickDNColor");				
			//			}	  




			// 		    if (!jbb.AudioEnabled)
			//			{	
			//				DeleteThese.Add("AudioIB");	
			//				DeleteThese.Add("WAVFileNameCL");
			//				DeleteThese.Add("WAVFileNameCL2");

			//			}	


			//DeleteThese.Add("ArrowsEnabled");	


			if (jbb.SignalType == "None")
			{
				DeleteThese.Add("ArrowOffset");
				DeleteThese.Add("ArrowBarWidth");
				DeleteThese.Add("ArrowBarHeight");
				DeleteThese.Add("ArrowSize");
				DeleteThese.Add("ArrowUpFBrush");
				DeleteThese.Add("ArrowDownFBrush");
				DeleteThese.Add("ArrowUpStroke");
				DeleteThese.Add("ArrowDownStroke");

				DeleteThese.Add("LabelsEnabled");
				DeleteThese.Add("TextFont");
				DeleteThese.Add("LabelBuy");
				DeleteThese.Add("LabelSell");

				DeleteThese.Add("ReverseEnabled");
				DeleteThese.Add("MA1Type");
				DeleteThese.Add("VisualOffset");
				DeleteThese.Add("ColorUpBrush3");
				DeleteThese.Add("ColorDownBrush3");

			}











			if (!jbb.LabelsEnabled)
			{
				DeleteThese.Add("TextFont");
				DeleteThese.Add("LabelBuy");
				DeleteThese.Add("LabelSell");

			}




			if (jbb.StochasticsType == "StochasticsFast")
			{
				DeleteThese.Add("Smooth");
				DeleteThese.Add("PeriodD");
				DeleteThese.Add("PeriodK");


			}
			else
			{
				DeleteThese.Add("PeriodDFast");
				DeleteThese.Add("PeriodKFast");

			}




			if (jbb.BackColorMode == "None")
			{
				DeleteThese.Add("ColorAll");
				DeleteThese.Add("Brush01");
				DeleteThese.Add("Opacity01");
				DeleteThese.Add("Brush02");
				DeleteThese.Add("Opacity01");
				DeleteThese.Add("Opacity02");





			}


			if (jbb.BarColorMode == "None")
			{

				DeleteThese.Add("BarColorFillEnabled");
				DeleteThese.Add("BarColorOutlineEnabled");
				DeleteThese.Add("BarUpBrush1");
				DeleteThese.Add("BarUpBrush2");
				DeleteThese.Add("BarDownBrush1");
				DeleteThese.Add("BarDownBrush2");
				DeleteThese.Add("BarNeutralBrush1");
				DeleteThese.Add("BarNeutralBrush2");
				DeleteThese.Add("BarOpacityUp");
				DeleteThese.Add("BarOpacityDown");


			}


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
					adjusted.Add(new PropertyDescriptorExtended(thisDescriptor, o => value, null, new Attribute[] { new BrowsableAttribute(false), }));

				else if (DeleteThese2.Contains(thisDescriptor.DisplayName))
					adjusted.Add(new PropertyDescriptorExtended(thisDescriptor, o => value, null, new Attribute[] { new BrowsableAttribute(false), }));

				else if (thisDescriptor.Category == "Data Series")
					adjusted.Add(new PropertyDescriptorExtended(thisDescriptor, o => value, null, new Attribute[] { new BrowsableAttribute(false), }));
				else
					adjusted.Add(thisDescriptor);

			}
			return adjusted;







			//			//DeleteThese.Add("Calculate");
			//			//DeleteThese.Add("Name");
			//      	 	DeleteThese.Add("Anchor Dots");
			//			DeleteThese.Add("Anchor Dots 2");
			//			//DeleteThese.Add("Period");

			//		//	CCIAvePlot

			//			if (DeleteThese.Count == 0)
			//				return propertyDescriptorCollection;


			//			PropertyDescriptorCollection adjusted = new PropertyDescriptorCollection(null);
			//			foreach (PropertyDescriptor thisDescriptor in propertyDescriptorCollection)
			//			{
			//				//Print(thisDescriptor.DisplayName);


			////				if (thisDescriptor.Category != "Plots")
			////					adjusted.Add(new PropertyDescriptorExtended(thisDescriptor, o => value, null, new Attribute[] {new BrowsableAttribute(false), }));
			////				else
			////					adjusted.Add(thisDescriptor);



			//				if (DeleteThese.Contains(thisDescriptor.DisplayName))
			//					adjusted.Add(new PropertyDescriptorExtended(thisDescriptor, o => value, null, new Attribute[] {new BrowsableAttribute(false), }));
			//				else
			//					adjusted.Add(thisDescriptor);


			////				if (DeleteThese.Contains(thisDescriptor.Name))
			////					adjusted.Add(new PropertyDescriptorExtended(thisDescriptor, o => value, null, new Attribute[] {new BrowsableAttribute(false), }));
			////				else
			////					adjusted.Add(thisDescriptor);


			//			}
			//			return adjusted;




		}
	}




}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private aiBestStochastics[] cacheaiBestStochastics;
		public aiBestStochastics aiBestStochastics(int periodDFast, int periodKFast, int periodD, int periodK, int smooth, string stochasticsType, bool mTFEnabled, string mTFBasePeriodType1, int mTFBarsPeriod1, string signalType, bool reverseEnabled)
		{
			return aiBestStochastics(Input, periodDFast, periodKFast, periodD, periodK, smooth, stochasticsType, mTFEnabled, mTFBasePeriodType1, mTFBarsPeriod1, signalType, reverseEnabled);
		}

		public aiBestStochastics aiBestStochastics(ISeries<double> input, int periodDFast, int periodKFast, int periodD, int periodK, int smooth, string stochasticsType, bool mTFEnabled, string mTFBasePeriodType1, int mTFBarsPeriod1, string signalType, bool reverseEnabled)
		{
			if (cacheaiBestStochastics != null)
				for (int idx = 0; idx < cacheaiBestStochastics.Length; idx++)
					if (cacheaiBestStochastics[idx] != null && cacheaiBestStochastics[idx].PeriodDFast == periodDFast && cacheaiBestStochastics[idx].PeriodKFast == periodKFast && cacheaiBestStochastics[idx].PeriodD == periodD && cacheaiBestStochastics[idx].PeriodK == periodK && cacheaiBestStochastics[idx].Smooth == smooth && cacheaiBestStochastics[idx].StochasticsType == stochasticsType && cacheaiBestStochastics[idx].MTFEnabled == mTFEnabled && cacheaiBestStochastics[idx].MTFBasePeriodType1 == mTFBasePeriodType1 && cacheaiBestStochastics[idx].MTFBarsPeriod1 == mTFBarsPeriod1 && cacheaiBestStochastics[idx].SignalType == signalType && cacheaiBestStochastics[idx].ReverseEnabled == reverseEnabled && cacheaiBestStochastics[idx].EqualsInput(input))
						return cacheaiBestStochastics[idx];
			return CacheIndicator<aiBestStochastics>(new aiBestStochastics(){ PeriodDFast = periodDFast, PeriodKFast = periodKFast, PeriodD = periodD, PeriodK = periodK, Smooth = smooth, StochasticsType = stochasticsType, MTFEnabled = mTFEnabled, MTFBasePeriodType1 = mTFBasePeriodType1, MTFBarsPeriod1 = mTFBarsPeriod1, SignalType = signalType, ReverseEnabled = reverseEnabled }, input, ref cacheaiBestStochastics);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.aiBestStochastics aiBestStochastics(int periodDFast, int periodKFast, int periodD, int periodK, int smooth, string stochasticsType, bool mTFEnabled, string mTFBasePeriodType1, int mTFBarsPeriod1, string signalType, bool reverseEnabled)
		{
			return indicator.aiBestStochastics(Input, periodDFast, periodKFast, periodD, periodK, smooth, stochasticsType, mTFEnabled, mTFBasePeriodType1, mTFBarsPeriod1, signalType, reverseEnabled);
		}

		public Indicators.aiBestStochastics aiBestStochastics(ISeries<double> input , int periodDFast, int periodKFast, int periodD, int periodK, int smooth, string stochasticsType, bool mTFEnabled, string mTFBasePeriodType1, int mTFBarsPeriod1, string signalType, bool reverseEnabled)
		{
			return indicator.aiBestStochastics(input, periodDFast, periodKFast, periodD, periodK, smooth, stochasticsType, mTFEnabled, mTFBasePeriodType1, mTFBarsPeriod1, signalType, reverseEnabled);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.aiBestStochastics aiBestStochastics(int periodDFast, int periodKFast, int periodD, int periodK, int smooth, string stochasticsType, bool mTFEnabled, string mTFBasePeriodType1, int mTFBarsPeriod1, string signalType, bool reverseEnabled)
		{
			return indicator.aiBestStochastics(Input, periodDFast, periodKFast, periodD, periodK, smooth, stochasticsType, mTFEnabled, mTFBasePeriodType1, mTFBarsPeriod1, signalType, reverseEnabled);
		}

		public Indicators.aiBestStochastics aiBestStochastics(ISeries<double> input , int periodDFast, int periodKFast, int periodD, int periodK, int smooth, string stochasticsType, bool mTFEnabled, string mTFBasePeriodType1, int mTFBarsPeriod1, string signalType, bool reverseEnabled)
		{
			return indicator.aiBestStochastics(input, periodDFast, periodKFast, periodD, periodK, smooth, stochasticsType, mTFEnabled, mTFBasePeriodType1, mTFBarsPeriod1, signalType, reverseEnabled);
		}
	}
}

#endregion
