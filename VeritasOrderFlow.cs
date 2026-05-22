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
using System.Windows.Controls.WpfPropertyGrid;
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
using SharpDX.DirectWrite;


//This namespace holds Indicators in this folder and is required. Do not change it. 

namespace NinjaTrader.NinjaScript.Indicators
{
	
	
	[Gui.CategoryOrder("Details", -200)]
	[Gui.CategoryOrder("Parameters", -100)]
	[Gui.CategoryOrder("Audio Alerts", 0)]
	[Gui.CategoryOrder("Chart Display", 1)] 
	[Gui.CategoryOrder("Scale Display", 2)] 
    [Gui.CategoryOrder("Bar Display", 10)] 
	[Gui.CategoryOrder("Print Display", 11)]
	[Gui.CategoryOrder("Crosshair Display", 12)]
	[Gui.CategoryOrder("Last Price Display", 13)]
	[Gui.CategoryOrder("Zones Display", 14)] 
	[Gui.CategoryOrder("Bid / Ask Imbalances", 15)] 
	[Gui.CategoryOrder("Block Trades", 16)] 
	[Gui.CategoryOrder("Magnets", 17)] 
	[Gui.CategoryOrder("Unfinished Auctions", 18)] 
	[Gui.CategoryOrder("Washout Signals", 19)] 	
	[Gui.CategoryOrder("Market Depth Display", 20)] 
	[Gui.CategoryOrder("Volume Profile (Bar)", 21)] 
	[Gui.CategoryOrder("Volume Profile (Composite)", 22)]
	[Gui.CategoryOrder("Volume Profile (Composite) Levels", 23)]
	
//	[Gui.CategoryOrder("Volume Profile (Manual)", 23)] 
	
	[Gui.CategoryOrder("Swing Levels", 25)]
	[Gui.CategoryOrder("Heads Up Display", 27)]
	[Gui.CategoryOrder("Bar Counter", 30)] 
	
	[Gui.CategoryOrder("Chart Buttons", 50)]
	
	[Gui.CategoryOrder("Data Series", 150)] 
	//[Gui.CategoryOrder("Set up", 151)] 
	[Gui.CategoryOrder("Visual", 152)] 
	
	
	[Gui.CategoryOrder("Order Panel Display", 200)]
	[Gui.CategoryOrder("Order Flag Display", 203)] 
	[Gui.CategoryOrder("Profit And Loss Display", 204)] 
	[Gui.CategoryOrder("Trade Management", 205)] 
	[Gui.CategoryOrder("Market Entry Orders", 211)]
	[Gui.CategoryOrder("Limit Entry Orders 1", 212)]
	[Gui.CategoryOrder("Limit Entry Orders 2", 213)]
	[Gui.CategoryOrder("Click Entry Orders", 215)]
	[Gui.CategoryOrder("Close Entry Orders", 216)]
	[Gui.CategoryOrder("Washout Entry Orders", 220)]
	[Gui.CategoryOrder("Stack Entry Orders", 221)]
	[Gui.CategoryOrder("Bracket Entry Orders", 222)]
	
	
	
	

	[Gui.CategoryOrder("Data", 599)] 
	[Gui.CategoryOrder("Version", 600)] 
	
	
	
	[Gui.CategoryOrder("Setup", 9000)]
	[Gui.CategoryOrder("License", 10000)]
	
	
	
	
	
		
	[TypeConverter("NinjaTrader.NinjaScript.Indicators.VeritasOrderFlowConverter")]
	public class VeritasOrderFlow : Indicator
	{
		

		private string ThisName = "aiImpactOrderFlow";
		
		
		
		
		private int SelectedStopRender = 0;
		private int CurrentStopRender = 0;
		
		private bool TestBeta = true;	
		
		
		private bool TestLoad = false;		
		private bool TestRender = false;			
		private bool TestScale = false;	
		
		
		private int TicksAdjust = 0;
		
		private bool EmptyBarError = false;
			
		private List<string> AllErrorMessages = new List<string>();
			private VeritasOrderFlowPropertyGridWindow ThisPropertyGridWindow;

		
		private int pRenderMilliseconds = 150;
		private		bool pOutlineZones = false;
		
		private int minimumtextsize = 7;
				
		private int pSpaceBetweenColumns = 20; // pixels between composite and market depth
		
		private int pMenuOutline = 9; // outline chart button menus in pixels
		private int space = 8; // space between buttons
		
		private bool ShowMenuBackground = true;


	
//        [Range(0, 10000)]
//        [Display(ResourceType = typeof(Custom.Resource), Name = "Space Between Columns(Pixels)", GroupName = "Scale Display", Order = 0)]
//        public int ThisBarMarginRight
//        {
//            get { return pThisBarMarginRight; }
//            set { pThisBarMarginRight = value; }
//        }		
		
		private bool BuyCloseReady = false;
		private bool SellCloseReady = false;		
		private bool BuyStackReady = false;
		private bool SellStackReady = false;
		private bool BuyClickReady = false;
		private bool SellClickReady = false;
		
		private bool BuyWashoutReady = false;
		private bool SellWashoutReady = false;		
		
		
		
		
		private bool BuyStackGo = false;
		private int BuyStackCount = 0;
		private int BuyStackCountP = 0;
							
				      
		private bool SellStackGo = false;
		private int SellStackCount = 0;
		private int SellStackCountP = 0;									
		
		private double CurrentLongSignals = 0;
		private double CurrentShortSignals = 0;
		
		
		private double PreviousLongSignals = 0;
		private double PreviousShortSignals = 0;
		
	
		
		private string ClickText = string.Empty;
		
		//bool pPrintEnabled = true;
				
		private		bool DrawOneBarZones = false; // don't draw broken zones that only painted for one bar.
		
		// end customize
		

		private DateTime LastTickTime = DateTime.MinValue;
		
		private DateTime LastProcess1Time = DateTime.MinValue;
		private DateTime LastProcess2Time = DateTime.MinValue;
		private DateTime LastProcess3Time = DateTime.MinValue;
		private DateTime LastProcess4Time = DateTime.MinValue;
		private DateTime LastProcess5Time = DateTime.MinValue;
		private DateTime LastProcess6Time = DateTime.MinValue;
		private DateTime LastProcess7Time = DateTime.MinValue;
		private DateTime LastProcess8Time = DateTime.MinValue;
		
		private bool ProcessOK = false;
		
		// toolbar
		
		private NinjaTrader.Gui.Chart.ChartTab		chartTab;
		private NinjaTrader.Gui.Chart.Chart			chartWindow;
		private bool								isToolBarButtonAdded;
		private System.Windows.DependencyObject		searchObject;
		private System.Windows.Controls.TabItem		tabItem;
		private System.Windows.Controls.Menu		theMenu;
		
		
			
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItem;
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem1;
		private NinjaTrader.Gui.Tools.NTMenuItem	topMenuItemSubItem2;
		
		
		// sidebar
		
		private System.Windows.Controls.Button		toggle1, toggle2, toggle3, toggle4, toggle5, toggle6, toggle7, toggle8, toggle9, toggle10, toggle11, toggle12;
		
		private System.Windows.Controls.Button		button1, button2, button3, button4, button5, button6, button7, button8, button9, button10, button11, button12, button13, button14, button15, button16, button17, button18, button19, button20;
		
		
		private System.Windows.Controls.Grid		twoButtonGrid1, twoButtonGrid2, twoButtonGrid3, twoButtonGrid4, twoButtonGrid5, twoButtonGrid6;
		
		private System.Windows.Controls.Grid		buttonGrid, chartGrid;
		private System.Windows.Controls.Grid		buttonTwoGrid1, buttonTwoGrid2, buttonTwoGrid3, buttonTwoGrid4, buttonTwoGrid5;
		private System.Windows.Controls.Grid		TypeGrid1, TypeGrid2, TypeGrid3, TypeGrid4, TypeGrid5;
		private System.Windows.Controls.Grid		QtyTIFGrid;
		private System.Windows.Controls.Grid		PNLGrid;
		private System.Windows.Controls.Grid		MarketButtons, CloseButtons, WashoutButtons, StackButtons, ClickButtons, OtherButtons, SetupButtons, BracketButtons, Limit1Buttons, Limit2Buttons;
		
		
		
		private System.Windows.Controls.TextBlock label = new System.Windows.Controls.TextBlock();
		
		private System.Windows.Controls.TextBlock PNLBox = new System.Windows.Controls.TextBlock();
		private System.Windows.Controls.TextBlock QTYBox = new System.Windows.Controls.TextBlock();
		
		
		//private NinjaTrader.Gui.Chart.ChartTab		chartTab;
		//private NinjaTrader.Gui.Chart.Chart			chartWindow;
		private bool								panelActive;
		private int									tabControlStartColumn;
		//private System.Windows.Controls.TabItem		tabItem;
		
		System.Windows.Controls.ComboBox cb1, cb2, cb3, cb4;

		private NinjaTrader.Gui.Tools.QuantityUpDown OffsetBox1;
		private NinjaTrader.Gui.Tools.QuantityUpDown OffsetBox2;
		private NinjaTrader.Gui.Tools.QuantityUpDown OffsetBox3;
		private NinjaTrader.Gui.Tools.QuantityUpDown OffsetBox4;
		private NinjaTrader.Gui.Tools.QuantityUpDown OffsetBox5;
		private NinjaTrader.Gui.Tools.QuantityUpDown OffsetBox6;
		
		//private Brush ButtonBrush = Brushes.DarkGreen;
		private Brush ButtonBrush2 = Brushes.Black;
		
		private Brush ButtonSBrush = Brushes.DarkRed;
		private Brush ButtonSBrush2 = Brushes.Black;
		
		private Brush Button3Brush = Brushes.DimGray;
		private Brush Button3Brush2 = Brushes.Black;		
		
		
		private AccountSelector		accountSelector;
       // private QuantityUpDown                  qudSelector;
		private QuantityUpDown                  qudSelector;
		private QuantitySelector                  qudSelector2;
		
        private TifSelector                     tifSelector;
        private NinjaTrader.Gui.NinjaScript.AtmStrategy.AtmStrategySelector atmStrategySelector;	
		
		
		private bool MouseWheelDone = false;
		
		private int SentToChartQty = 0;
		private double SentToChartPNL = 0;
		private double SentToChartAvgPrice = 0;
		
		
		private int maininitialspace = 3;
		private int pOrderOutlineOpacity = 40;
		private bool pMakeOrderOutlineNotSeeThru = false;
		
		private int pOrderDisplayWidth = 125;
		private int pOrderFlagButtonOpacity = 35;
		
		private				int orderhspace = 9;
						
		private string OCOBracketID = string.Empty;
		
		SortedDictionary<double, float> AllOrderPrices = new SortedDictionary<double, float>();
		SortedDictionary<double, int> AllStopLossPrices = new SortedDictionary<double, int>();
		SortedDictionary<double, int> TotalOrdersPrices = new SortedDictionary<double, int>();
		SortedDictionary<double, int> TotalOrdersPrices2 = new SortedDictionary<double, int>();
		
		SortedDictionary<double, int> FoundStopLossPrices = new SortedDictionary<double, int>();
		
				
		SortedDictionary<double, OrderDetails> AllOrderCancelButtons = new SortedDictionary<double, OrderDetails>();
		SortedDictionary<double, OrderDetails> AllOrderMoveButtons = new SortedDictionary<double, OrderDetails>();
		SortedDictionary<double, OrderDetails> AllStopCombinationButtons = new SortedDictionary<double, OrderDetails>();
		
	
        public class OrderDetails
        {
            Order iThisOrder;
			SharpDX.RectangleF iThisRectA;
            string iName;
            double iWidth;
            bool iSwitch;
            SharpDX.RectangleF iRect;
            bool iHovered;

            public Order ThisOrder { get { return iThisOrder; } set { iThisOrder = value; } }
			public SharpDX.RectangleF ThisRectA { get { return iThisRectA; } set { iThisRectA = value; } }
            public string Name { get { return iName; } set { iName = value; } }
            public double Width { get { return iWidth; } set { iWidth = value; } }
            public bool Switch { get { return iSwitch; } set { iSwitch = value; } }
            public SharpDX.RectangleF Rect { get { return iRect; } set { iRect = value; } }
            public bool Hovered { get { return iHovered; } set { iHovered = value; } }

        }
		
		private int OrderDisplayMinHeight = 15;
		
		private Order MovingOrder;
		
		private List<Order> MovingOrders = new List<Order>(); 
		private List<Order> DeleteOrders = new List<Order>(); 
		
		private double ThisTickSizze = 0;
		
		private bool GetBoxYPixelStatus = false;
		
		private bool IsFirstRealTimeTick = true;
		
		private double FinalXPixel = 0;
		private	double FinalYPixel = 0;		
		
		private int currentbuttonhover3 = -10;
		private int currentbuttonhover5 = -10;
		
		private double dpiX = 0;
		private	double dpiY = 0;
		private int LaunchNumber = 0;

		// DPI scaling helpers — at 100% returns input unchanged, at 150% returns input * 1.5.
		// dpiX is stored as percentage * 100 (so 125%-DPI = 125), so divide by 100 to get scale.
		private float S(float v) { return v * (float)(dpiX > 0 ? dpiX / 100.0 : 1.0); }
		private int S(int v) { return (int)(v * (dpiX > 0 ? dpiX / 100.0 : 1.0)); }
		private float S(double v) { return (float)(v * (dpiX > 0 ? dpiX / 100.0 : 1.0)); }

						
		private double PreviousHighRender = 0;
		private double PreviousLowRender = 0;
		private double CurrentHighRender = 0;
		private double CurrentLowRender = 0;
		
		private double PreviousHighRender2 = 0;
		private double PreviousLowRender2 = 0;
		private double CurrentHighRender2 = 0;
		private double CurrentLowRender2 = 0;
		
		
		private SharpDX.Direct2D1.Brush FinalThisBrushDX = null;
		
		private SharpDX.Direct2D1.Brush TextBrushDX2 = null;
		private SharpDX.Direct2D1.Brush TextBrushDX1 = null;

        private SharpDX.Direct2D1.Brush ChartTextBrushDX = null;
		private SharpDX.Direct2D1.Brush ChartBackgroundBrushDX = null;
		private SharpDX.Direct2D1.Brush ChartBackgroundFadeBrushDX = null;
		private SharpDX.Direct2D1.Brush ChartBackgroundFade2BrushDX = null;
		private SharpDX.Direct2D1.Brush ChartBackgroundMenuFadeBrushDX = null;
		private SharpDX.Direct2D1.Brush ChartBackgroundErrorBrushDX = null;
		
		private SharpDX.Direct2D1.Brush buttonBrushDX = null;
		private SharpDX.Direct2D1.Brush buttonHBrushDX = null;
		private SharpDX.Direct2D1.Brush buttonFHBrushDX = null;
		private SharpDX.Direct2D1.Brush buttonFOFFBrushDX = null;
			
		private SharpDX.Direct2D1.Brush buttonFONBrushDX = null;
		private SharpDX.Direct2D1.Brush cachedPanelBackdropBrushDX = null;
		private SharpDX.Direct2D1.Brush cachedHeaderBgBrushDX = null;
		private SharpDX.Direct2D1.Brush cachedHeaderBgHoverBrushDX = null;
		private SharpDX.Direct2D1.Brush cachedHeaderTextBrushDX = null;
		private SharpDX.Direct2D1.Brush cachedHoverGlowBrushDX = null;
		private SharpDX.DirectWrite.TextFormat cachedHeaderTextFormat = null;
		private SharpDX.DirectWrite.TextFormat cachedButtonTextFormat = null;
		// Change-detection keys for panel brush / text format cache
		private Brush _panelCacheBgRef = null;
		private Brush _panelCacheTextRef = null;
		private string _btnFmtCacheFamily = null;
		private double _btnFmtCacheSize = 0;
		private SharpDX.Direct2D1.Brush MainFillBrushDX = null;
		private SharpDX.Direct2D1.Brush BidDotsBrushDX = null;
		private SharpDX.Direct2D1.Brush AskDotsBrushDX = null;
		private SharpDX.Direct2D1.Brush BidTriBrushDX = null;
		private SharpDX.Direct2D1.Brush AskTriBrushDX = null;

		private SharpDX.Direct2D1.Brush MagnetFillBrushDX = null;
		private SharpDX.Direct2D1.Brush UFAFillBrushDX = null;
		private SharpDX.Direct2D1.Brush POCFillBrushDX = null;
		private SharpDX.Direct2D1.Brush VWAPFillBrushDX = null;
		private SharpDX.Direct2D1.Brush ThisBrushDX = null;
		private SharpDX.Direct2D1.Brush ThisBrushDX2 = null;
			
		private SharpDX.Direct2D1.Brush MainPrintBrushDX = null;
		
		private SharpDX.Direct2D1.Brush LastPriceBrushDX = null;
		private SharpDX.Direct2D1.Brush CrossHairBrushDX = null;
		
		private SharpDX.Direct2D1.Brush TransparentBrushDX = null;
		
		private SharpDX.Direct2D1.Brush PrintShortBrushDX = null;
		private SharpDX.Direct2D1.Brush PrintLongBrushDX = null;
		private SharpDX.Direct2D1.Brush AboveLongBrushDX = null;
		private SharpDX.Direct2D1.Brush AboveNeutralBrushDX = null;
		private SharpDX.Direct2D1.Brush AboveShortBrushDX = null;
		
					
				SharpDX.Direct2D1.Brush Plot1BrushDX= null;
				SharpDX.Direct2D1.Brush Plot2BrushDX= null;
				SharpDX.Direct2D1.Brush Plot3BrushDX= null;
				SharpDX.Direct2D1.Brush Plot4BrushDX= null;
				SharpDX.Direct2D1.Brush Plot5BrushDX= null;
				SharpDX.Direct2D1.Brush Plot6BrushDX= null;		
				SharpDX.Direct2D1.Brush Plot7BrushDX= null;
				SharpDX.Direct2D1.Brush Plot8BrushDX= null;			
		
		
		private SharpDX.Direct2D1.Brush ResistanceBrush1DX = null;
		private SharpDX.Direct2D1.Brush ResistanceBrush2DX = null;
		private SharpDX.Direct2D1.Brush ResistanceBrush3DX = null;
		private SharpDX.Direct2D1.Brush SupportBrush1DX = null;
		private SharpDX.Direct2D1.Brush SupportBrush2DX = null;
		private SharpDX.Direct2D1.Brush SupportBrush3DX = null;
		private SharpDX.Direct2D1.Brush ZoneButtonBrushDX;
		
		
		
		private SharpDX.Direct2D1.Brush longBrushDX = null;
		private SharpDX.Direct2D1.Brush shortBrushDX = null;
		private SharpDX.Direct2D1.Brush arrowBrushDX = null;
		private SharpDX.Direct2D1.Brush LabelBrushDX = null;
		
		private SharpDX.Direct2D1.Brush HUDVOLColorDX = null;
		private SharpDX.Direct2D1.Brush HUDNEColorDX = null;
		private SharpDX.Direct2D1.Brush HUDUPColorDX = null;
		private SharpDX.Direct2D1.Brush HUDDNColorDX = null;
		
		private SharpDX.Direct2D1.Brush CompVOLColorDX = null;
		private SharpDX.Direct2D1.Brush CompNEColorDX = null;
		private SharpDX.Direct2D1.Brush CompUPColorDX = null;
		private SharpDX.Direct2D1.Brush CompDNColorDX = null;		
		
		
		// on execution
		
		private EMA TrendEMA;
		
		private int LastBarLong = 0;
		private int LastBarShort = 0;
		
		private NinjaTrader.Gui.Tools.QuantityUpDown chartTraderQty;
		private NinjaTrader.Gui.Tools.TifSelector chartTraderTIF;
		private NinjaTrader.Gui.Tools.AccountSelector chartTraderAcct;
		private NinjaTrader.Gui.NinjaScript.AtmStrategy.AtmStrategySelector chartTraderATM;
		
		
		
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
		private string pTIF = "GTC";
		private int pQty = 0;
				
		
		private string OCOID = string.Empty;
      private ChartTrader chartTrader;

		 private Account myAccount;
		

		private Order LongEntryOrder = null;
		private Order ShortEntryOrder = null;	
		
		private bool SaveNextOrder = false;
		private bool SaveLongOrder = false;
		private bool SaveShortOrder = false;
		
		private double StopLimitOffset, NewStopPrice, NewLimitPrice, CurrentStopPrice = 0;
		private bool OrderInstrumentOK, OrderStateOK, OrderTypeOK, OrderNameOK = false;
		
		private string PreviousAccountName = string.Empty;
		
		private double SLMPrice = 0;

			private double EntryOrderPrice = 0;
		
				
		private string CurrentType = string.Empty;
		
		DateTime R1Time = DateTime.MinValue;
		
		// on render
		
		private string CellString = string.Empty;
		private SharpDX.DirectWrite.TextLayout CellLayout;
		private SharpDX.DirectWrite.TextLayout CellLayoutCalc;
		
        private SharpDX.DirectWrite.TextFormat CellFormat;
		private SharpDX.DirectWrite.TextFormat CellFormatFinal;
		private SharpDX.DirectWrite.TextFormat CellFormatImb;
		private SharpDX.DirectWrite.TextFormat CellFormatCalc;
		SortedList<int, double> HeightToTextSize = new SortedList<int, double>();
		
		private double PriceRowHeight = 100000;
		private double MaxTextXPixels = 0;
		private double MaxTextYPixels = 0;

		private double MaxBoxHeight = 0;
		private double MaxBoxHeightP = 1;
		
		private bool IsDragging = false;

		
				
		
		private string LicensingMessage = string.Empty;		

		private SortedDictionary<int, int> ProductIDToMachineIDs = new SortedDictionary<int, int>();
		private ConcurrentDictionary<int, List<string>> ProductIDToInstruments = new ConcurrentDictionary<int, List<string>>();
	
		
		private bool Permission = false;
		
		
		
		private bool EnableOrderExecution = false;
		
		
			
		private bool LicenseWordPress (string machineid, string pLicensingEmailAddress)
		{
			
			
			
			
			
			
			
			
			
			List<int> ThisProductMainIDs = new List<int>();
			List<int> ThisProductSecondaryIDs = new List<int>();
			
			// Product IDs for Order Flow Indicator
			
			ThisProductMainIDs.Add(19317); // TTPOrderFlow
			ThisProductMainIDs.Add(502073); // Impact Order Flow Indicator
			ThisProductMainIDs.Add(502078); // Impact Order Flow Complete System
			ThisProductMainIDs.Add(527487); // Impact Order Flow Entry Orders
			
			
			
			// Product IDs for Order Flow Execution Tool Set
			
			ThisProductSecondaryIDs.Add(19318);
			ThisProductSecondaryIDs.Add(502079);
			ThisProductSecondaryIDs.Add(502078);		
			
			
			
			
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
					EnableOrderExecution = false;			
									
//					Print("-----------");
//					Print(machineidstring + " " + notestring);
//					Print("-----------");
					
					 foreach (KeyValuePair<int, int> oneproduct in ProductIDToMachineIDs)
           			 {		
				
						 int productid = oneproduct.Key;
						int numbermachineidsok = oneproduct.Value;
						
						 //Print(productid + "-" + numbermachineidsok);
						 
						if (ThisProductMainIDs.Contains(productid))
						//if (productid == ThisProductMainID)
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
						
						
						if (ThisProductSecondaryIDs.Contains(productid))
						//if (productid == ThisProductXID)
						{
							
							EnableOrderExecution = true;
							//pEXYES = true;
							
						}
						
						
											
						
						 
						 
					 }
					 
		 
					  
					 if (!Permission && LicensingMessage == "")
					 {
							LicensingMessage = "You haven't purchased this product.  Contact " + pContactEmail + " if you need further assistance.";
							
						 EnableOrderExecution = false;
						 
						 
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

		
		
		
		
		private int FinalTextSize1 = 0;
		private int FinalTextSize1P = 0;
		private int FinalTextSize2 = 0;
		private int FinalTextSize2P = 0;		
		
		private bool IsTickReplay = false;
		
		private double CurrentBid = 0;
		private double CurrentAsk = 0;
		private double CurrentLast = 0;
		private double CurrentLastData = 0;
		private int FB = 0;
		private int PrintFB = 0;
		private int LB = 0;
		private int BB = 0;
		private int xt = 0;
		private int yt = 0;
		
		private float topmenu = 0;
		private float leftmenu = 9999999;
		private float leftmenu2 = 9999999;
		
		private bool HistoricalReset = false;
		private	bool RealTimeReset = false;
		private	bool TickReplayReset = false;
		private int lastFinalizedBar = -1;
		private DateTime lastFormingCompute = DateTime.MinValue;
		private	bool IsNowRealTime = false;
		
		private	bool FirstRender = true;	

		private double x1 = 0;
		private double x2 = 0;
		private double x3 = 0;
		private double x4 = 0;
				
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
		
		private double TopPrice = 0;
		private double BottomPrice = 0;
		
		private int LastAudioBar1 = 0;
		private int LastAudioBar2 = 0;
		private int LastAudioBar3 = 0;
		private int LastAudioBar4 = 0;
		private int LastAudioBar5 = 0;
		private int LastAudioBar6 = 0;
		private int LastAudioBar7 = 0;
		private int LastAudioBar8 = 0;
		
		
		private double ThisMousePrice = 0;
		private double ThisMousePriceP = 0;	
		
		private double ThisMouseBar = 0;
		private double ThisMouseBarP = 0;			
		
		private int RightMoveX = 0;
		
		private int XE = 0;

		private SharpDX.Direct2D1.AntialiasMode oldAntialiasMode = SharpDX.Direct2D1.AntialiasMode.Aliased;
		
		private int barWidth = 0;
		private int barDistance = 0;
		private int barBetween = 0;
		private int halfRowHeight = 0;
		
		private bool IsHardRightEdge = false;
		
		private MarketDepth<MarketDepthRow> ThisMarketDepth;
		

		
		private int HUDNumber = 1;
		
		private System.Windows.Threading.DispatcherTimer timer3;

		private double BarsProcessed = 0;
		private double BarsRemaining = 0;		
		
		private bool RealTimeIn = false;
		
		private double MaxValue2 = 0;
		private double MinValue2 = 0;
		
		
		
		// Removed unused MAG SortedList and MAG2 List<Level> — they were allocated but never used.
		// (DrawExtensions has a parameter named MAG that shadowed this field; no actual reads against these fields anywhere.)
		
		private SortedList<double, PriceBox> PriceBoxes2 = new SortedList<double, PriceBox>();
		
		private ConcurrentDictionary<double, PriceBox> PriceBoxes1 = new ConcurrentDictionary<double, PriceBox>();
		
		private ConcurrentDictionary<double, PriceBox> PriceX1Boxes = new ConcurrentDictionary<double, PriceBox>();
		private ConcurrentDictionary<double, PriceBox> PriceX2Boxes = new ConcurrentDictionary<double, PriceBox>();
		
		private ConcurrentDictionary<double, long> AskLevels = new ConcurrentDictionary<double, long>();
		private ConcurrentDictionary<double, long> BidLevels = new ConcurrentDictionary<double, long>();
		private DateTime							lastRefresh;
		private List<LadderRow>						askRows, bidRows, rows;
			
		private class LadderRow
		{
			public string	MarketMaker;    // relevant for stocks only
			public double	Price;
			public long		Volume;

			public LadderRow(double myPrice, long myVolume, string myMarketMaker)
			{
				MarketMaker		= myMarketMaker;
				Price			= myPrice;
				Volume			= myVolume;
			}
		}
		
		SortedDictionary<double, RowData> SD = new SortedDictionary<double, RowData>();
		
		public class PriceBox
		{

			double top;
			double bottom;
			double height;
			
						
			public double Top { get{return top;} set{top = value; }}
			public double Bottom { get{return bottom;} set{bottom = value; }}
			public double Height { get{return height;} set{height = value; }}

		}
		
		private PriceBox PP = new PriceBox();
		
		private int ClickedZoneBar = 0;
		private double ClickedZoneTop = 0;
		private double ClickedZoneBottom = 0;
		
		Stroke ThisStroke = new Stroke(Brushes.DarkGreen, DashStyleHelper.Solid, 2);
		Stroke ThisStrokeH = new Stroke(Brushes.DarkGreen, DashStyleHelper.Solid, 2);
		
		
		
		private bool IsCurrentBar = false;
		
		private int buttonh = 27;
		
		private Brush ButtonBrush = Brushes.DarkGreen;

		private Point MP;
		
	
		// vertical line move
		
		
		private Stroke pHighlightStroke = new Stroke(Brushes.White, DashStyleHelper.Solid, 3);
		private Stroke pOneWidthStroke = new Stroke(Brushes.White, DashStyleHelper.Solid, 1);
		
		
		private SharpDX.Direct2D1.Brush VerticalLineHighlightDX = null;

		// Pre-created brushes for inner render loops (class-level for helper method access)
		private SharpDX.Direct2D1.Brush MainTextColorDX = null;
		private SharpDX.Direct2D1.Brush BidFillDX = null;
		private SharpDX.Direct2D1.Brush BidTextDX = null;
		private SharpDX.Direct2D1.Brush AskFillDX = null;
		private SharpDX.Direct2D1.Brush AskTextDX = null;
		private SharpDX.Direct2D1.Brush LVFillDX = null;
		private SharpDX.Direct2D1.Brush AxisBrushDX = null;
		private SharpDX.Direct2D1.Brush BackgroundColorDX = null;
		private SharpDX.Direct2D1.Brush TimerMainColorDX = null;
		private SharpDX.Direct2D1.Brush ClickUPColorDX = null;
		private SharpDX.Direct2D1.Brush ClickDNColorDX = null;
		private SharpDX.Direct2D1.Brush GreenBrushDX = null;
		private SharpDX.Direct2D1.Brush RedBrushDX = null;
		private SharpDX.Direct2D1.Brush GrayBrushDX = null;
		private SharpDX.Direct2D1.Brush BidHashDX = null;
		private SharpDX.Direct2D1.Brush AskHashDX = null;

		// Active (unclosed) magnets and UFAs for O(n) closure checking
		private List<Level> ActiveMagnets = new List<Level>();
		private List<Level> ActiveUFAs = new List<Level>();

		// Active (unclosed) zones for O(n) termination checking
		private List<Zone> ActiveResistanceZones = new List<Zone>();
		private List<Zone> ActiveSupportZones = new List<Zone>();
		private List<Zone> ActiveResistanceZones2 = new List<Zone>();
		private List<Zone> ActiveSupportZones2 = new List<Zone>();

		// Pre-computed string comparison flags
		private bool isBidAskMode = false;
		private bool isVolumeMode = false;
		private bool isDeltaMode = false;
		private bool isCandlestickFill = false;
		private bool isDeltaFill = false;
		private bool isVolumeOpacity = false;
		private bool isDeltaOpacity = false;
		
		
		private int pMoveVerticalLinePadding = 6;
		
		
		// Composite
		private SharpDX.RectangleF MoveComposite = new SharpDX.RectangleF(0, 0, 0, 0);
		private bool IsHoverComposite = false;
		private bool IsHoverCompositeP = false;
		
		private bool IsMoveComposite = false;
		private int StartCompositeLength = 0;
		private int StartCompositeX = 0;
		private double CurrentCompositeX = 0;

		// MD
		private SharpDX.RectangleF MoveMD = new SharpDX.RectangleF(0, 0, 0, 0);
		private bool IsHoverMD = false;
		private bool IsHoverMDP = false;
		
		private bool IsMoveMD = false;		
		private int StartMDLength = 0;
		private int StartMDX = 0;
		private double CurrentMDX = 0;
		// RM
		private SharpDX.RectangleF MoveRM = new SharpDX.RectangleF(0, 0, 0, 0);
		private bool IsHoverRM = false;
		private bool IsHoverRMP = false;
		
		private bool IsMoveRM = false;		
		private int StartRMLength = 0;
		private int StartRMX = 0;
		private double CurrentRMX = 0;
		
		
		
		
		
		
		
		
		
		
		
        private SharpDX.RectangleF B2 = new SharpDX.RectangleF(0, 0, 0, 0);
		private SharpDX.RectangleF B22 = new SharpDX.RectangleF(0, 0, 0, 0);
		private SharpDX.RectangleF B23 = new SharpDX.RectangleF(0, 0, 0, 0);
		private SharpDX.RectangleF B232 = new SharpDX.RectangleF(0, 0, 0, 0);
		private SharpDX.RectangleF B24 = new SharpDX.RectangleF(0, 0, 0, 0);
		private SharpDX.RectangleF B44 = new SharpDX.RectangleF(0, 0, 0, 0);
		
		
		private DateTime StartLoad;
		private DateTime EndLoad;
		private TimeSpan TotalLoadTime = new TimeSpan(0,0,0,0,0);
		
		private SharpDX.RectangleF BidRect5 = new SharpDX.RectangleF(0,0,0,0);
		private SharpDX.RectangleF BidRect = new SharpDX.RectangleF(0,0,0,0);
		private SharpDX.RectangleF AskRect = new SharpDX.RectangleF(0,0,0,0);
		
		private SharpDX.RectangleF BidRect2 = new SharpDX.RectangleF(0,0,0,0);
		private SharpDX.RectangleF AskRect2 = new SharpDX.RectangleF(0,0,0,0);
		
		private SharpDX.RectangleF BidSignalRect = new SharpDX.RectangleF(0,0,0,0);
		private SharpDX.RectangleF AskSignalRect = new SharpDX.RectangleF(0,0,0,0);
		
		private SharpDX.RectangleF BidRectF = new SharpDX.RectangleF(0,0,0,0);
		private SharpDX.RectangleF BidRectF2 = new SharpDX.RectangleF(0,0,0,0);
		private SharpDX.RectangleF BidRectF25 = new SharpDX.RectangleF(0,0,0,0);
		private SharpDX.RectangleF B1 = new SharpDX.RectangleF(0,0,0,0);
		

        private bool InMenu;
        private bool InMenuP;
		
        private bool InMenu2;
        private bool InMenu2P;
		
        private bool InMenu3;
        private bool InMenu3P;
		
        private bool InMenu4;
        private bool InMenu4P;
		
        private bool ButtonOff = false;
		private bool DebugBarSizeSync = false;

		private double CurrentMousePrice = 0;

		int MenuButtonExpandP = 0;

        SortedDictionary<double, ButtonZ> AllButtonZ1 = new SortedDictionary<double, ButtonZ>();
		SortedDictionary<double, ButtonZ> AllButtonZ2 = new SortedDictionary<double, ButtonZ>();
		SortedDictionary<double, ButtonZ> AllButtonZ3 = new SortedDictionary<double, ButtonZ>();
		SortedDictionary<double, ButtonZ> AllButtonZ4 = new SortedDictionary<double, ButtonZ>();
		SortedDictionary<double, ButtonZ> AllButtonZ5 = new SortedDictionary<double, ButtonZ>();
		
		
        const float fontHeight = 15f;

        private int PriceDigits = 0;
        private string PriceString = string.Empty;

        private List<double> All50Levels = new List<double>();
        private List<double> All100Levels = new List<double>();

		private List<SharpDX.RectangleF> BlockTradeButtons = new List<SharpDX.RectangleF>();
		private bool InBlockNow = false;
		private bool InBlockNowP = false;
		
 
		
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


		private string SpreadName = string.Empty;
		private string CHLI = string.Empty;
        private int news = 22;
		
	
		private Series<int> Signals;

        private Series<double> BodyHigh;
        private Series<double> BodyLow;
        private Series<double> WickHigh;
        private Series<double> WickLow;

        private Series<int> Direction;
		private Series<int> Direction2;

		//private Series<double> Signal;
		
		private bool ForceHighPriority = false;
		private bool ForceLowPriority = false;
		
		private Series<double> ScaleHigh;
		private Series<double> ScaleLow;
		
		private double CurrentScaleHigh = 0;
		private double CurrentScaleLow = 0;
					
		private bool IsScrolled	= false;
		
		private Series<double> ThisEMA1;
		private Series<double> ThisEMA2;
		
		private Series<double> CurrentEMATrend;
		
	 	Point StartPoint	= new Point(0, 0);
		Point EndPoint		= new Point(0, 0);
		Point TextPoint		= new Point(0, 0);

		private SharpDX.RectangleF ThisRect = new SharpDX.RectangleF(0, 0, 0, 0);
		private SharpDX.RectangleF ThisRect2 = new SharpDX.RectangleF(0, 0, 0, 0);
		private SharpDX.RectangleF ThisRect3 = new SharpDX.RectangleF(0, 0, 0, 0);
		private SharpDX.RectangleF ThisRect4 = new SharpDX.RectangleF(0, 0, 0, 0);
		private SharpDX.RectangleF ThisRect5 = new SharpDX.RectangleF(0, 0, 0, 0);
		private SharpDX.RectangleF ThisRect6 = new SharpDX.RectangleF(0, 0, 0, 0);
		private SharpDX.RectangleF ThisRect7 = new SharpDX.RectangleF(0, 0, 0, 0);
		
		private SharpDX.RectangleF ClickEntryCancelRect = new SharpDX.RectangleF(0, 0, 0, 0);	
		private SharpDX.RectangleF MoveOrderCancelRect = new SharpDX.RectangleF(0, 0, 0, 0);	
		
		
		private SharpDX.Direct2D1.Ellipse ThisEllipse;
		private SharpDX.Direct2D1.PathGeometry ThisGeometry;
		
        private DateTime FirstBarTime = DateTime.MinValue;

		
		
		public class BarItem
		{
			public double tv = 0.0;
			public double av = 0.0;
			public double bv = 0.0;
			public double dv = 0.0;
			public double dvmax = 0.0;
			public double dvmin = 0.0;
			
			public double POC = 0.0;
			public double VWAP = 0.0;
			
			public double ClusterTop = 0.0;
			public double ClusterBottom = 0.0;			
			
			public ConcurrentDictionary<double, RowData> l = new ConcurrentDictionary<double, RowData>();
			//public SortedDictionary<double, RowData> l = new SortedDictionary<double, RowData>();
			
			public List<double> BidI = new List<double>();
			public List<double> AskI = new List<double>();
			public List<double> BidB = new List<double>();
			public List<double> AskB = new List<double>();
			
			
			public ConcurrentDictionary<double, List<double>> BidBlocks = new ConcurrentDictionary<double, List<double>>();
			public ConcurrentDictionary<double, List<double>> AskBlocks = new ConcurrentDictionary<double, List<double>>();
			
			//public SortedList<double, int> Magnets = new SortedList<double, int>();
			//public SortedList<double, int> UnfinishedAuctions = new SortedList<double, int>();
			
			public List<Level> Magnets = new List<Level>();
			public List<Level> UnfinishedAuctions = new List<Level>();
			
			
			
		}
		
		public class Level
		{
			public double Price;
			
			public int EndBar;
		}	
		
		public class Profile
		{
			public double tv = 0.0;
			public double av = 0.0;
			public double bv = 0.0;
			public double dv = 0.0;
			
			public double maxav = 0;
			public double maxbv = 0;
			public double maxtv = 0;
			public double maxdv = 0;
			
			public ConcurrentDictionary<double, RowData> l = new ConcurrentDictionary<double, RowData>();
		}
		
	
		
		public class RowData
		{
			public double tv = 0.0; // total volume
			public double av = 0.0; // ask volume
			public double bv = 0.0; // bid volume
			public double dv = 0.0; // delta volume
		}
		
	private double CurrentTrend = 0;
		
//		// getPoc
//		//
//		private double getPoc(ConcurrentDictionary<double, RowData> dict)
//		{
//			double poc = 0.0;
			
//			if(!dict.IsEmpty)
//			{
//				poc = dict.Keys.Aggregate((i, j) => dict[i].tv > dict[j].tv ? i : j);
//			}
			
//			return poc;
//		}
		
//		// getDelta
//		//
//		private double getDelta(ConcurrentDictionary<double, RowData> dict)
//		{
//			double askSum = 0.0;
//			double bidSum = 0.0;
			
//			if(!dict.IsEmpty)
//			{
//				foreach(KeyValuePair<double, RowData> rd in dict)
//				{
//					askSum += rd.Value.av;
//					bidSum += rd.Value.bv;
//				}
//			}
			
//			return (askSum - bidSum);
//		}
		
//		// getVolume
//		//
//		private double getVolume(ConcurrentDictionary<double, RowData> dict, double key)
//		{
//			double tv = 0.0;
			
//			key = ThisMasterInstrument.RoundToTickSize(key);
			
//			if(dict.ContainsKey(key))
//			{
//				tv = dict[key].tv;
//			}
			
//			return tv;
//		}
		
//		// getMaxVolume
//		//
//		private double getMaxVolume(ConcurrentDictionary<double, RowData> dict)
//		{
//			double mv = 0.0;
			
//			if(!dict.IsEmpty)
//			{
//				foreach(KeyValuePair<double, RowData> rd in dict)
//				{
//					mv = (rd.Value.av > mv) ? rd.Value.av : mv;
//					mv = (rd.Value.bv > mv) ? rd.Value.bv : mv;
//				}
//			}
			
//			return mv;
//		}
		
//		// getTotalVolume
//		//
//		private double getTotalVolume(ConcurrentDictionary<double, RowData> dict)
//		{
//			double tv = 0.0;
			
//			if(!dict.IsEmpty)
//			{
//				foreach(KeyValuePair<double, RowData> rd in dict)
//				{
//					tv += rd.Value.tv;
//				}
//			}
			
//			return tv;
//		}
		
		// getAskImbalanceRatio
		//
		private double getAskImbalanceRatio(ConcurrentDictionary<double, RowData> dict, double key)
		{
			double volRatio = 0.0;
			double askPrice = ThisMasterInstrument.RoundToTickSize(key);
			double bidPrice = ThisMasterInstrument.RoundToTickSize(key - ThisTickSizze);

			RowData askRow, bidRow;
			if (dict == null || !dict.TryGetValue(askPrice, out askRow) || !dict.TryGetValue(bidPrice, out bidRow))
				return volRatio;

			double askVolume = askRow.av;
			double bidVolume = bidRow.bv;
			
			
			if(askVolume > bidVolume)
			{
//				if(askVolume - bidVolume >= minImbalance)
//				{
//					volRatio = (askVolume - bidVolume) / (askVolume + bidVolume);
//				}
				
				if (askVolume != 0 && bidVolume != 0)
					volRatio = askVolume / bidVolume;
				else
					volRatio = 1000;				
				
				
			}
			
			double delta = Math.Abs(bidVolume - askVolume);
			
			if (delta < pMinimumImbDelta)
				volRatio = 0;
			
			return volRatio;
		}
		
		// getBidImbalanceRatio
		//
		private double getBidImbalanceRatio(ConcurrentDictionary<double, RowData> dict, double key)
		{
			double volRatio = 0.0;
			double askPrice = ThisMasterInstrument.RoundToTickSize(key + ThisTickSizze);
			double bidPrice = ThisMasterInstrument.RoundToTickSize(key);

			RowData askRow, bidRow;
			if (!dict.TryGetValue(askPrice, out askRow) || !dict.TryGetValue(bidPrice, out bidRow))
				return volRatio;

			double askVolume = askRow.av;
			double bidVolume = bidRow.bv;
			
			//double AskMin = (double)askt/(double)SL.Count/(double)100*pVolumeQualifier;
			//double BidMin = (double)bidt/(double)SL.Count/(double)100*pVolumeQualifier;
			
			
			if(bidVolume > askVolume)
			{
//				if(bidVolume - askVolume >= minImbalance)
//				{
//					volRatio = (bidVolume - askVolume) / (bidVolume + askVolume);
//				}
				if (askVolume != 0 && bidVolume != 0)
					volRatio = bidVolume / askVolume;
				else
					volRatio = 1000;
			}
			
			
			double delta = Math.Abs(bidVolume - askVolume);
			
			if (delta < pMinimumImbDelta)
				volRatio = 0;
			
			return volRatio;
		}
		
		
		private bool isActiveTab(ChartControl cControl)
		{
			if (TestRender)
				return true;
			
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
		
		
//		// getValueArea
//		//
//		private double[] getValueArea(ConcurrentDictionary<double, RowData> dict)
//		{
//			double vah = 0.0;
//			double val = 0.0;
			
//			double[] ret = {vah,val};
			
//			if(!dict.IsEmpty)
//			{
//				int    iteCnt = 0;
//				double maxPrc = dict.Keys.Max();
//				double minPrc = dict.Keys.Min();
//				double pocPrc = getPoc(dict);
//				double volSum = getTotalVolume(dict);
//				double maxVol = volSum * 0.7;
//				double volTmp = 0.0;
//				double upperP = pocPrc + TickSize;
//				double lowerP = pocPrc - TickSize;
//				double upperV = 0.0;
//				double lowerV = 0.0;
				
//				volTmp = getVolume(dict, pocPrc);
				
//				while(volTmp < maxVol)
//				{
//					if((upperP == maxPrc && lowerP == minPrc) || iteCnt >= 500) { break; }
					
//					upperV = getVolume(dict, upperP) + getVolume(dict, upperP + TickSize);
//					lowerV = getVolume(dict, lowerP) + getVolume(dict, lowerP - TickSize);
					
//					if(upperV > lowerV)
//					{
//						vah	   = ThisMasterInstrument.RoundToTickSize(upperP + TickSize);
//						volTmp = volTmp + upperV; 
//						upperP = ThisMasterInstrument.RoundToTickSize(vah + TickSize);
//					}
//					else
//					{
//						val	   = ThisMasterInstrument.RoundToTickSize(lowerP - TickSize);
//						volTmp = volTmp + lowerV; 
//						lowerP = ThisMasterInstrument.RoundToTickSize(val - TickSize);
//					}
					
//					iteCnt++;
//				}
				
//				ret[0] = Math.Min(maxPrc, vah);
//				ret[1] = Math.Max(minPrc, val);
//			}
			
//			return ret;
//		}
		

		
		private void fillMissingTicks5(int index)
		{
			double hi = Bars.GetHigh(CurrentBar - index);
			double lo = Bars.GetLow(CurrentBar - index);
			double pr = ThisMasterInstrument.RoundToTickSize(hi);
			
			//Print(hi + "   " + lo);
			
			//return;
			
			int gggg = 0;
			
			//Print("hey");
			
			if (BI != null)
			while(pr > lo && gggg < 1000)
			{
				
				//Print(pr);
				
				if (!BI.l.ContainsKey(pr))
				{
					BI.l.TryAdd(pr, new RowData());
					
					BI.l[pr].av = 0;
					BI.l[pr].bv = 0;
					BI.l[pr].dv = 0;
					BI.l[pr].tv = 0;
				}
				
				gggg = gggg + 1;
				
				pr = ThisMasterInstrument.RoundToTickSize(pr - ThisTickSizze);
			}
		}
		
		
		
		private double ask = 0.0;
		private double bid = 0.0;
		private double cls = 0.0;
		private double vol = 0.0;
		private double tmp = 0.0;
		
		private double min = 0.0;
		private double max = 0.0;
		private double rng = 0.0;
		private double off = 0.0;
		private double dif = 0.0;
		
		// ---
		
		private bool 	 setDate = false;
		private DateTime getDate;
		private DateTime lastRender;
		
		// ---
		
		private SimpleFont sfimb;
		
		private SimpleFont TextFontCalc;
		private SimpleFont FinalFont1;
		private SimpleFont FinalFont2;
		// ---
		
		private double 			 tSize = 0.0;
		private MasterInstrument ThisMasterInstrument = null;
		
	
		private BarItem BI;
		
		
		
		private Series<BarItem> BarItems;
		private Series<Profile> Profiles;
		
		private Series<BarItem> BarItems2;
		
		public class ZoneItem
		{
			public List<Zone> ResistanceZones = new List<Zone>();
			public List<Zone> SupportZones = new List<Zone>();
			
			public List<Zone> ResistanceZones2 = new List<Zone>();
			public List<Zone> SupportZones2 = new List<Zone>();
			
			
		}
		private Series<ZoneItem> ZoneItems;
		
		public class Zone
		{

			//long bidSize;
			long ticksWidth;
			double bottomPrice;
			double topPrice;
			double testedPrice;
			bool isBroken;
			bool isHidden;
			int endBar;
			int startBar; // creation bar — termination loop must not break a zone on its own bar
			//List<long> bidBlocks = new List<long>();
			//List<long> askBlocks = new List<long>();
			//long bidTotal;
			//long askTotal;
			public bool IsBroken { get{return isBroken;} set{isBroken = value; }}			
				public bool IsHidden { get{return isHidden;} set{isHidden = value; }}		
				
			//public long BidSize { get{return bidSize;} set{bidSize = value; }}
			public long TicksWidth { get{return ticksWidth;} set{ticksWidth = value; }}
			
			public int EndBar { get{return endBar;} set{endBar = value; }}
			public int StartBar { get{return startBar;} set{startBar = value; }}
			//public long BidTotal { get{return bidTotal;} set{bidTotal = value; }}
			//public long AskTotal { get{return askTotal;} set{askTotal = value; }}			
			
			public double TestedPrice { get{return testedPrice;} set{testedPrice = value; }}
			public double BottomPrice { get{return bottomPrice;} set{bottomPrice = value; }}
			public double TopPrice { get{return topPrice;} set{topPrice = value; }}
			//public List<long> BidBlocks { get{return bidBlocks;} set{bidBlocks = value; }}
			//public List<long> AskBlocks { get{return askBlocks;} set{askBlocks = value; }}
		}
		
				private	int bsp = 0;
		private	int bsp2 = 8;	
		
		SortedList<int, List<Zone>> ResistanceZones = new SortedList<int, List<Zone>>();
		SortedList<int, List<Zone>> SupportZones = new SortedList<int, List<Zone>>();	
		
		SortedList<int, List<Zone>> ResistanceZones2 = new SortedList<int, List<Zone>>();
		SortedList<int, List<Zone>> SupportZones2 = new SortedList<int, List<Zone>>();
		
		private Series<double> TestD;
		
		
		//
		private DateTime getStartDate(int workDays)
	    {
			int 			dir = workDays < 0 ? -1 : 1;
			DateTime        now = DateTime.UtcNow;
			SessionIterator sit = new SessionIterator(Bars); sit.GetNextSession(now, true);
			DateTime 		act = sit.ActualSessionBegin;
			
		    while(workDays != 0)
		    {
		     	act = act.AddDays(dir);
				
		      	if(act.DayOfWeek != DayOfWeek.Saturday && act.DayOfWeek != DayOfWeek.Sunday)
		      	{
		        	workDays -= dir;
		      	}
		    }
			
		    return act;
	    }
				
		
		SortedDictionary<int, double> HighLA = new SortedDictionary<int, double>();
		SortedDictionary<int, double> LowLA = new SortedDictionary<int, double>();
		
		SortedDictionary<int, double> DeleteLA = new SortedDictionary<int, double>();
		
		SortedDictionary<int, int> HighLF = new SortedDictionary<int, int>();
		SortedDictionary<int, int> LowLF = new SortedDictionary<int, int>();			
		
		private int CurrentHighBar, CurrentLowBar = 0;
		private bool WaitingForHigh = true;
		private bool WaitingForLow = true;		
		private double CurrentHighPrice, CurrentLowPrice = 0;
		
        private Series<double> AllPivots;
        private Series<double> FinalHigh;
        private Series<double> FinalLow;
		
		
		// TIMER
		
		private string			timeLeft	= string.Empty;
		private DateTime		now		 	= Core.Globals.Now;
		private bool			connected,
								hasRealtimeData;
		private SessionIterator sessionIterator;

		private System.Windows.Threading.DispatcherTimer timer;

		private string BarTimerString = string.Empty;
		
		private bool	isRangeDerivate;
		
		private long volume;
		private bool isVolume, isVolumeBase;		
		
		
		
		
		private int FirstPlot = 25;
		
		protected override void OnStateChange()
		{
			
			try
			{
			
				if (TestLoad) Print(State);
				
				//Print(State);
				
//				indicatorVersion = "1.0.9.0";
//				ntVersion = "8.0.18.0";
//				releaseDate = "May 22, 2019";
				
				
				
				if (State == State.SetDefaults)
				{
					

					Name = ThisName;
					Description = "";
					
					

				
	
					UFAStroke.Opacity = 90;
					MagnetStroke.Opacity = 90;
					
					UFAStroke2.Opacity = 30;
					MagnetStroke2.Opacity = 30;
					
					
					
					ColorSwingLow.Opacity = 90;
					ColorSwingHigh.Opacity = 90;
					
					ColorSwingLow2.Opacity = 50;
					ColorSwingHigh2.Opacity = 50;
					
					pTextFont4.Bold = true;
					TextFont2Imb.Bold = true;
					
					
					Calculate					= Calculate.OnEachTick;
					IsOverlay					= true;
					DisplayInDataBox			= false;
					DrawOnPricePanel			= true;
					IsAutoScale = true;
					
					
					DrawHorizontalGridLines		= true;
					DrawVerticalGridLines		= true;
					
					PaintPriceMarkers			= true;
					
					ScaleJustification			= NinjaTrader.Gui.Chart.ScaleJustification.Right;

					IsSuspendedWhileInactive	= false;
					
	                ArePlotsConfigurable = false;
	                AreLinesConfigurable = false;

					
					AddPlot(new Stroke(Brushes.Transparent, DashStyleHelper.Solid, 1),	PlotStyle.Line, "Long Signals"); // 0
					AddPlot(new Stroke(Brushes.Transparent, DashStyleHelper.Solid, 1),	PlotStyle.Line, "Short Signals");
					
					AddPlot(new Stroke(Brushes.Black, DashStyleHelper.Solid, 1),	PlotStyle.Line, "COMP POC"); // 2
					AddPlot(new Stroke(Brushes.Black, DashStyleHelper.Solid, 1),	PlotStyle.Line, "COMP VWAP");
					AddPlot(new Stroke(Brushes.Black, DashStyleHelper.Solid, 1),	PlotStyle.Line, "COMP VAH 1");
					AddPlot(new Stroke(Brushes.Black, DashStyleHelper.Solid, 1),	PlotStyle.Line, "COMP VAL 1");
					AddPlot(new Stroke(Brushes.Black, DashStyleHelper.Solid, 1),	PlotStyle.Line, "COMP VAH 2");
					AddPlot(new Stroke(Brushes.Black, DashStyleHelper.Solid, 1),	PlotStyle.Line, "COMP VAL 2");
					
					AddPlot(new Stroke(Brushes.Transparent, DashStyleHelper.Solid, 1),	PlotStyle.Line, "Total Volume"); // 8
					AddPlot(new Stroke(Brushes.Transparent, DashStyleHelper.Solid, 1),	PlotStyle.Line, "Bar Volume");
					AddPlot(new Stroke(Brushes.Transparent, DashStyleHelper.Solid, 1),	PlotStyle.Line, "Total Delta");
					AddPlot(new Stroke(Brushes.Transparent, DashStyleHelper.Solid, 1),	PlotStyle.Line, "Bar Delta");				
					AddPlot(new Stroke(Brushes.Transparent, DashStyleHelper.Solid, 1),	PlotStyle.Line, "Bar Bid"); // 12
					AddPlot(new Stroke(Brushes.Transparent, DashStyleHelper.Solid, 1),	PlotStyle.Line, "Bar Ask");					
					AddPlot(new Stroke(Brushes.Transparent, DashStyleHelper.Solid, 1),	PlotStyle.Line, "Bar Delta Percent");				
					AddPlot(new Stroke(Brushes.Transparent, DashStyleHelper.Solid, 1),	PlotStyle.Line, "Bar Bid Percent"); 
					AddPlot(new Stroke(Brushes.Transparent, DashStyleHelper.Solid, 1),	PlotStyle.Line, "Bar Ask Percent");
					
					AddPlot(new Stroke(Brushes.Transparent, DashStyleHelper.Solid, 1),	PlotStyle.Line, "COMP Session High");
					AddPlot(new Stroke(Brushes.Transparent, DashStyleHelper.Solid, 1),	PlotStyle.Line, "COMP Session Low"); // 18				
	               
					AddPlot(new Stroke(Brushes.Transparent, DashStyleHelper.Solid, 1),	PlotStyle.Line, "StackBuyIB"); // 19
					AddPlot(new Stroke(Brushes.Transparent, DashStyleHelper.Solid, 1),	PlotStyle.Line, "StackSellIB"); // 20						
					AddPlot(new Stroke(Brushes.Transparent, DashStyleHelper.Solid, 1),	PlotStyle.Line, "StackBuyClose");
					AddPlot(new Stroke(Brushes.Transparent, DashStyleHelper.Solid, 1),	PlotStyle.Line, "StackSellClose"); // 18	
					AddPlot(new Stroke(Brushes.Transparent, DashStyleHelper.Solid, 1),	PlotStyle.Line, "BlockBuy");
					AddPlot(new Stroke(Brushes.Transparent, DashStyleHelper.Solid, 1),	PlotStyle.Line, "BlockSell"); // 24
					
					AddPlot(new Stroke(Brushes.Transparent, DashStyleHelper.Solid, 1),	PlotStyle.Line, "BlockMin"); // 25
					AddPlot(new Stroke(Brushes.Transparent, DashStyleHelper.Solid, 1),	PlotStyle.Line, "StackMin"); // 26
					
					
					
					
					FirstPlot = 27;
					
					//Print(FirstPlot);
					
					// update first plot variable if adding plots above this // 25
					
					
					
					
					
					
					AddPlot(new Stroke(Brushes.Black, DashStyleHelper.Solid, 1),	PlotStyle.Line, "moving"); // 19
					AddPlot(new Stroke(Brushes.Black, DashStyleHelper.Solid, 1),	PlotStyle.Line, "click"); // 20
					AddPlot(new Stroke(Brushes.Black, DashStyleHelper.Solid, 1),	PlotStyle.Line, "position"); // 21
					AddPlot(new Stroke(Brushes.Black, DashStyleHelper.Solid, 1),	PlotStyle.Line, "4");
					AddPlot(new Stroke(Brushes.Black, DashStyleHelper.Solid, 1),	PlotStyle.Line, "5");
					AddPlot(new Stroke(Brushes.Black, DashStyleHelper.Solid, 1),	PlotStyle.Line, "6");
					AddPlot(new Stroke(Brushes.Black, DashStyleHelper.Solid, 1),	PlotStyle.Line, "7");
					AddPlot(new Stroke(Brushes.Black, DashStyleHelper.Solid, 1),	PlotStyle.Line, "8");
					AddPlot(new Stroke(Brushes.Black, DashStyleHelper.Solid, 1),	PlotStyle.Line, "9");
					AddPlot(new Stroke(Brushes.Black, DashStyleHelper.Solid, 1),	PlotStyle.Line, "10");
					AddPlot(new Stroke(Brushes.Black, DashStyleHelper.Solid, 1),	PlotStyle.Line, "11");
					AddPlot(new Stroke(Brushes.Black, DashStyleHelper.Solid, 1),	PlotStyle.Line, "12");
					AddPlot(new Stroke(Brushes.Black, DashStyleHelper.Solid, 1),	PlotStyle.Line, "13");
					AddPlot(new Stroke(Brushes.Black, DashStyleHelper.Solid, 1),	PlotStyle.Line, "14");
					AddPlot(new Stroke(Brushes.Black, DashStyleHelper.Solid, 1),	PlotStyle.Line, "15");
					AddPlot(new Stroke(Brushes.Black, DashStyleHelper.Solid, 1),	PlotStyle.Line, "16");
					AddPlot(new Stroke(Brushes.Black, DashStyleHelper.Solid, 1),	PlotStyle.Line, "17");
					AddPlot(new Stroke(Brushes.Black, DashStyleHelper.Solid, 1),	PlotStyle.Line, "18");
					AddPlot(new Stroke(Brushes.Black, DashStyleHelper.Solid, 1),	PlotStyle.Line, "19");
					AddPlot(new Stroke(Brushes.Black, DashStyleHelper.Solid, 1),	PlotStyle.Line, "20");
					AddPlot(new Stroke(Brushes.Black, DashStyleHelper.Solid, 1),	PlotStyle.Line, "21");
					AddPlot(new Stroke(Brushes.Black, DashStyleHelper.Solid, 1),	PlotStyle.Line, "22");
					AddPlot(new Stroke(Brushes.Black, DashStyleHelper.Solid, 1),	PlotStyle.Line, "23");
					AddPlot(new Stroke(Brushes.Black, DashStyleHelper.Solid, 1),	PlotStyle.Line, "24");
					AddPlot(new Stroke(Brushes.Black, DashStyleHelper.Solid, 1),	PlotStyle.Line, "25");
					AddPlot(new Stroke(Brushes.Black, DashStyleHelper.Solid, 1),	PlotStyle.Line, "26");
					AddPlot(new Stroke(Brushes.Black, DashStyleHelper.Solid, 1),	PlotStyle.Line, "27");
					AddPlot(new Stroke(Brushes.Black, DashStyleHelper.Solid, 1),	PlotStyle.Line, "28");
					AddPlot(new Stroke(Brushes.Black, DashStyleHelper.Solid, 1),	PlotStyle.Line, "29");					
					
	            }
				else if (State == State.Configure)
			    {
					
					

					DisplayInDataBox			= false;
					
					
					
					if (pCompNumberDisplayMode == "None")
					if (pCompHistogramDisplayMode == "None")
					{
						pCompHistogramDisplayMode = "Volume & Delta";
						pCompNumberDisplayMode = "Volume";
					}
					
					MenuButtonExpandP = (int) Math.Ceiling((double)pSpaceBetweenButtons/2); 
					
					
				
				
					
					//				
					
	                
					
					
					//Bars.IsTickReplay = true; // can't assign this variable
					
					
					if(!Bars.IsTickReplay)
					{
						
				
						
						if (pDataCalcMode2 == "Volumetric")
						{
//							if (pDataCalcMode == "Tick")
//								AddVolumetric(Instrument.FullName, BarsPeriodType.Minute, 1, VolumetricDeltaType.UpDownTick, 1);
//							else
//								AddVolumetric(Instrument.FullName, BarsPeriodType.Minute, 1, VolumetricDeltaType.BidAsk, 1);
							
							if (pDataCalcMode == "Tick")
								AddVolumetric(Instrument.FullName, BarsArray[0].BarsPeriod.BarsPeriodType, Bars.BarsPeriod.Value, VolumetricDeltaType.UpDownTick, 1);
							else
								AddVolumetric(Instrument.FullName, BarsArray[0].BarsPeriod.BarsPeriodType, Bars.BarsPeriod.Value, VolumetricDeltaType.BidAsk, 1);
							
						}
    						
						else if (pDataCalcMode == "Tick")
						{
						
							AddDataSeries(BarsPeriodType.Tick, 1);
						}
    						
						else if (pDataCalcMode == "Bid/Ask")
						{
							AddDataSeries(BarsPeriodType.Tick, 1);
							AddDataSeries(Instrument.FullName, BarsPeriodType.Tick, 1, MarketDataType.Ask);
	 						AddDataSeries(Instrument.FullName, BarsPeriodType.Tick, 1, MarketDataType.Bid);
						}
    
						
						
						
						//Add another AAPL data series using the Bid series, with other settings identical

						
						
						IsTickReplay = false;
					}
					else
					{
						IsTickReplay = true;	
						
					}
	                
					
					
//					ChartControl.Dispatcher.InvokeAsync(() =>
//					{
						
//						ChartBarsSwitch2(false);
//					});		
					
					
					

	//				int maxDays = 10000;
					
	//				if(!setDate)
	//				{
	//					getDate = getStartDate((maxDays - 1) * -1);
	//					setDate = true;
	//				}
					
					
					if (TestLoad)
					StartLoad = DateTime.Now;
					
					
					
					
					
				
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
					
					
						if (!EnableOrderExecution)
						{
							pOrderPanelOn = false;
							pOrdersDisplayOn = false;
						}
						else
						{
							if (pOrderPanelOn)
								pOrdersDisplayOn = true;
							
							
						}
						 
						//Name = ThisName;				
										
					
					
					
					if (!NinjaTrader.Core.Globals.AtiOptions.IsAtiEnabled)
						NinjaTrader.Core.Globals.AtiOptions.IsAtiEnabled = true;
					
					//Permission = true;
					

			    }
				else if (State == State.Terminated)
				{
					// Release cross-frame-cached DX brushes / text formats
					SafeDispose(cachedPanelBackdropBrushDX); cachedPanelBackdropBrushDX = null;
					SafeDispose(cachedHeaderBgBrushDX); cachedHeaderBgBrushDX = null;
					SafeDispose(cachedHeaderBgHoverBrushDX); cachedHeaderBgHoverBrushDX = null;
					SafeDispose(cachedHeaderTextBrushDX); cachedHeaderTextBrushDX = null;
					SafeDispose(cachedHoverGlowBrushDX); cachedHoverGlowBrushDX = null;
					SafeDispose(cachedHeaderTextFormat); cachedHeaderTextFormat = null;
					SafeDispose(cachedButtonTextFormat); cachedButtonTextFormat = null;

					UnregisterPanel();




					//ChartPanel.Dispatcher.InvokeAsync(() =>
					//{
						if (ChartControl != null)
		                {
							// Restore the user's chart-series selection setting if we overrode it.
							if (_selectionDraggingOverridden && ChartControl.Properties != null)
								ChartControl.Properties.AllowSelectionDragging = PreviousDrag;

							// Release the hand-cursor override so it cannot outlive the indicator.
							if (_cursorOverrideActive) { System.Windows.Input.Mouse.OverrideCursor = null; _cursorOverrideActive = false; }

							ChartControl.MouseMove -= new MouseEventHandler(OnMouseMove);
							ChartControl.PreviewMouseMove -= new MouseEventHandler(OnPanelPreviewMouseMoveSuppress);
		                    ChartControl.MouseDown -= new MouseButtonEventHandler(OnMouseDown);
							ChartControl.PreviewMouseDown -= new MouseButtonEventHandler(OnPanelPreviewLeftDown);
							ChartControl.MouseUp -= new MouseButtonEventHandler(OnMouseUp);

							ChartControl.MouseLeave -= new MouseEventHandler(OnMouseLeave);


							ChartPanel.MouseDoubleClick -= new MouseButtonEventHandler(OnMouseDoubleClick);
							//this.ChartPanel.DragOver += new DragEventHandler(this.DragOver);

							ChartControl.PreviewMouseWheel -= ChartControl_PreviewMouseWheel;
							ChartControl.PreviewKeyDown -= ChartControl_PreviewKeyDown;
							ChartPanel.MouseWheel -= new MouseWheelEventHandler(OnPanelMouseWheel);
							NinjaTrader.Cbi.Log.LogEvent -= OnNewLogEvent;

							ChartControl.Dispatcher.InvokeAsync(() =>
							{

								ChartBarsSwitch2(true);
							});		
							
							
						
		                }
			
					//});		
					

				
					if(Instrument != null)
					{
						
						ThisMasterInstrument = Instrument.MasterInstrument;
						
						ThisTickSizze = ThisMasterInstrument.TickSize;
						tSize = ThisTickSizze;

					}
					
					if (timer != null)
					{
						
						timer.IsEnabled = false;
						timer = null;
					}

					if (timer3 != null)
					{
					
						timer3.IsEnabled = false;
						timer3 = null;
					}
					
						
					ClosePropertiesWindow();
					
					
					
					
					
					
					
					
	//				ChartPanel.Dispatcher.InvokeAsync(() =>
	//				{
						
	//					ChartBarsSwitch(true);
	//				});		
					
					
					
				}

				else if (State == State.DataLoaded)
				{
		

				if (Name != ThisName && Name != string.Empty)
					Name = ThisName;	
				
					//if (!Permission)
					//	return;
					
					
					
					if (pCompLevelsPriceOn && pCompAllLevelsEnabled)
					{
						Plots[2].Brush = Brushes.Black;
						Plots[3].Brush = Brushes.Black;
						Plots[4].Brush = Brushes.Black;
						Plots[5].Brush = Brushes.Black;
						Plots[6].Brush = Brushes.Black;
						Plots[7].Brush = Brushes.Black;
					}
					else
					{
					
						Plots[2].Brush = Brushes.Transparent;
						Plots[3].Brush = Brushes.Transparent;
						Plots[4].Brush = Brushes.Transparent;
						Plots[5].Brush = Brushes.Transparent;
						Plots[6].Brush = Brushes.Transparent;
						Plots[7].Brush = Brushes.Transparent;
					}
					
					
					
					
					//ChartBarsSwitch(false);
						
					TrendEMA = EMA(50);
		
					askRows					= new List<LadderRow>();
					bidRows					= new List<LadderRow>();
					
					
					
					BarItems2 = new Series<BarItem>(this, MaximumBarsLookBack.Infinite);
					BarItems = new Series<BarItem>(this, MaximumBarsLookBack.Infinite);
					Profiles = new Series<Profile>(this, MaximumBarsLookBack.Infinite);
					
					ZoneItems = new Series<ZoneItem>(this, MaximumBarsLookBack.Infinite);
					
					
					
					
			        Signals = new Series<int>(this, MaximumBarsLookBack.Infinite);
	                Direction = new Series<int>(this, MaximumBarsLookBack.Infinite);
					Direction2 = new Series<int>(this, MaximumBarsLookBack.Infinite);
					
	                BodyHigh = new Series<double>(this, MaximumBarsLookBack.Infinite);
	                BodyLow = new Series<double>(this, MaximumBarsLookBack.Infinite);
	                WickHigh = new Series<double>(this, MaximumBarsLookBack.Infinite);
	                WickLow = new Series<double>(this, MaximumBarsLookBack.Infinite);

					ScaleHigh = new Series<double>(this, MaximumBarsLookBack.Infinite);
					ScaleLow = new Series<double>(this, MaximumBarsLookBack.Infinite);
					
					ThisEMA1 = new Series<double>(this, MaximumBarsLookBack.Infinite);
					ThisEMA2 = new Series<double>(this, MaximumBarsLookBack.Infinite);
					
					CurrentEMATrend = new Series<double>(this, MaximumBarsLookBack.Infinite);
					
					TestD = new Series<double>(this, MaximumBarsLookBack.Infinite);
					
					AllPivots = new Series<double>(this, MaximumBarsLookBack.Infinite);
					FinalHigh = new Series<double>(this, MaximumBarsLookBack.Infinite);
					FinalLow = new Series<double>(this, MaximumBarsLookBack.Infinite);				
		
					

					
					if(Instrument != null)
					{
						
						ThisMasterInstrument = Instrument.MasterInstrument;
						
						ThisTickSizze = ThisMasterInstrument.TickSize;
						tSize = ThisTickSizze;

					}
									
					//ChartPanel.Dispatcher.InvokeAsync(() =>
					//{
						if (ChartControl != null)
		                {
							ChartControl.MouseMove += new MouseEventHandler(OnMouseMove);
							ChartControl.PreviewMouseMove += new MouseEventHandler(OnPanelPreviewMouseMoveSuppress);
		                    ChartControl.MouseDown += new MouseButtonEventHandler(OnMouseDown);
							ChartControl.PreviewMouseDown += new MouseButtonEventHandler(OnPanelPreviewLeftDown);
							ChartControl.MouseUp += new MouseButtonEventHandler(OnMouseUp);
							
							
							//ChartPanel.MouseLeave += new System.EventHandler(this.OnMouseLeave);
							
						
							ChartControl.MouseLeave += new MouseEventHandler(OnMouseLeave);
		
							ChartPanel.MouseDoubleClick += new MouseButtonEventHandler(OnMouseDoubleClick);
							//this.ChartPanel.DragOver += new DragEventHandler(this.DragOver);
							
							ChartControl.PreviewMouseWheel += ChartControl_PreviewMouseWheel;
							ChartControl.PreviewKeyDown += ChartControl_PreviewKeyDown;
							ChartPanel.MouseWheel += new MouseWheelEventHandler(OnPanelMouseWheel);

							NinjaTrader.Cbi.Log.LogEvent += OnNewLogEvent;
		                }
			
					//});		
					
					
				if (EnableOrderExecution)
					{
//						AddButtonZ(AllButtonZ1, "Orders", "", 8, pOrdersDisplayOn);
//						AddButtonZ(AllButtonZ1, "Ghost", "", 8, pShowOrdersOffChart);
						
//						AddButtonZ(AllButtonZ1, "Summary", "", 2, pShowOrderSummary);
//						AddButtonZ(AllButtonZ1, "Split Stops", "", 2, pSplitStopDisplay);	
//						AddButtonZ(AllButtonZ1, "MIT", "", 2, pUseMIT);
//						AddButtonZ(AllButtonZ1, "SLM", "", 2, pUseSLM);
						
					
						
					}
					
					
					
			
						
						
	//				AddButtonZ(AllButtonZ, "Block Trades", "", 2, pShowBlocks);		
	//				AddButtonZ(AllButtonZ, "Unfinished Auctions", "", 2, pUFAEnabled);
	//				AddButtonZ(AllButtonZ, "Magnets", "", 2, pMAGEnabled);			
	//				AddButtonZ(AllButtonZ, "Imbalances", "", 2, pShowImbalance);			

	//				AddButtonZ(AllButtonZ, "Above Bar", pAboveTotalMode, 2, pAboveTotalMode == "None" ? false : true);
	//				AddButtonZ(AllButtonZ, "Body Mode", pBarBodyMode, 2, pBarBodyMode == "None" ? false : true);	
	//				AddButtonZ(AllButtonZ, "Opacity Mode", pPrintBarOpacityMode, 2, pPrintBarOpacityMode == "None" ? false : true);	
	//				AddButtonZ(AllButtonZ, "Bar Profile", "", 2, pBarCompositeEnabled);
	//				AddButtonZ(AllButtonZ, "Color Mode", pPrintBarFillMode, 2, false);					
	//				//AddButtonZ(AllButtonZ, "Split", "", 2, pSplit);
	//				AddButtonZ(AllButtonZ, "Numbers", PrintNumberDisplayMode, 2, false);	
					
					
	//				if (TestBeta) AddButtonZ(AllButtonZ2, "Order Panel Display", "", 2, pOrderPanelOn);
	//				if (TestBeta) AddButtonZ(AllButtonZ2, "Properties", "", 2, false);
	//				AddButtonZ(AllButtonZ2, "Audio Alerts", "", 2, pAudioEnabledMain);
	//				AddButtonZ(AllButtonZ2, "Heads Up Display", "", 2, pHUDEnabled);	
	//				AddButtonZ(AllButtonZ2, "Zones (Momentum) ", "", 2, pZonesEnabled2);
	//				AddButtonZ(AllButtonZ2, "Zones (Imbalance)", "", 2, pZonesEnabled);	
	//				AddButtonZ(AllButtonZ2, "Market Depth", "", 2, pDepthEnabled);
	//				AddButtonZ(AllButtonZ2, "Swing Levels", "", 2, pPivotLinesEnabled);
	//				AddButtonZ(AllButtonZ2, "Composite Levels", "", 2, pCompAllLevelsEnabled);
	//				AddButtonZ(AllButtonZ2, "Composite Profile", "", 2, pCompositeLocation != "None");	
	//				AddButtonZ(AllButtonZ2, "Numbers", pCompNumberDisplayMode, 2, pCompNumberDisplayMode == "None" ? false : true);	
	//				AddButtonZ(AllButtonZ2, "Histogram", pCompHistogramDisplayMode, 2, pCompHistogramDisplayMode == "None" ? false : true);				
						
						
						
						
//					if (EnableOrderExecution)
//					{
//						AddButtonZ(AllButtonZ2, "Order Panel Display", "", 8, pOrderPanelOn);
						
						
//					}
					
					
					AddButtonZ(AllButtonZ5, "Location", pCompositeLocation, 0, true);	
					AddButtonZ(AllButtonZ5, "Numbers", pCompNumberDisplayMode, 1, pCompNumberDisplayMode == "None" ? false : true);	
					AddButtonZ(AllButtonZ5, "Histogram", pCompHistogramDisplayMode, 2, pCompHistogramDisplayMode == "None" ? false : true);	
						
						
					bool DisableOldExecution = true;
					

					
					if (DisableOldExecution)
					{
						pOrderPanelOn = false;
						pOrdersDisplayOn = false;
					}
					
						
					if (EnableOrderExecution)
					{
						if (DisableOldExecution)
						{
							pOrderPanelOn = false;
							pOrdersDisplayOn = false;
							
							AddButtonZ(AllButtonZ3, "Blank", "", 3, false);
							AddButtonZ(AllButtonZ3, "Blank", "", 3, false);
							
						}
						else
						{
							AddButtonZ(AllButtonZ3, "Order Panel", "", 1, pOrderPanelOn);
							AddButtonZ(AllButtonZ3, "Order Display", "", 2, pOrdersDisplayOn);
						}
						
						
						
						
						AddButtonZ(AllButtonZ3, "Blank", "", 3, false);
						
					}					
					
					
					AddButtonZ(AllButtonZ3, "Zoom", "", 4, pZoomEnabled);
					
//					AddButtonZ(AllButtonZ3, "Y Scale -", "", 2, ButtonOff);
//					AddButtonZ(AllButtonZ3, "Y Scale +", "", 2, ButtonOff);
					
					AddButtonZ(AllButtonZ3, "Y Scale", "", 5, pUseFixedVerticalScale);
					AddButtonZ(AllButtonZ3, "Y Scroll", "", 6, pUseYScroll);
					AddButtonZ(AllButtonZ3, "X Size -",  "", 6, false);
					AddButtonZ(AllButtonZ3, "X Size +",  "", 6, false);
					AddButtonZ(AllButtonZ3, "X Space -", "", 6, false);
					AddButtonZ(AllButtonZ3, "X Space +", "", 6, false);

					
					AddButtonZ(AllButtonZ3, "Blank", "", 7, false);
					
					AddButtonZ(AllButtonZ3, "Properties", "", 10, true);
					
					AddButtonZ(AllButtonZ3, "Blank", "", 7, false);
						

					
					
					AddButtonZ(AllButtonZ3, "Audio Alerts", "", 10, pAudioEnabledMain);
					AddButtonZ(AllButtonZ3, "Heads Up Display", "", 11, pHUDEnabled);	
					AddButtonZ(AllButtonZ3, "Zones (Momentum) ", "", 12, pZonesEnabled2);
					AddButtonZ(AllButtonZ3, "Zones (Imbalance)", "", 13, pZonesEnabled);	
					AddButtonZ(AllButtonZ3, "Market Depth", "", 14, pDepthEnabled);
					AddButtonZ(AllButtonZ3, "Swing Levels", "", 15, pPivotLinesEnabled);
					AddButtonZ(AllButtonZ3, "Composite Levels", "", 16, pCompAllLevelsEnabled);
					AddButtonZ(AllButtonZ3, "Composite Profile", "", 17, pCompositeLocation != "None");	
//					AddButtonZ(AllButtonZ3, "Numbers", pCompNumberDisplayMode, 18, pCompNumberDisplayMode == "None" ? false : true);	
//					AddButtonZ(AllButtonZ3, "Histogram", pCompHistogramDisplayMode, 19, pCompHistogramDisplayMode == "None" ? false : true);				
						
					AddButtonZ(AllButtonZ3, "Blank", "", 20, false);
									
					
					AddButtonZ(AllButtonZ3, "Washout Signals", "", 21, pArrowsEnabled);	
					AddButtonZ(AllButtonZ3, "Block Trades", "", 22, pShowBlocks);		
					AddButtonZ(AllButtonZ3, "Unfinished Auctions", "", 23, pUFAEnabled);
					AddButtonZ(AllButtonZ3, "Magnets", "", 24, pMAGEnabled);			
					AddButtonZ(AllButtonZ3, "Imbalances", "", 25, pShowImbalance);			
					AddButtonZ(AllButtonZ3, "Below Bar", pBelowTotalMode, 26, pBelowTotalMode == "None" ? false : true);
					AddButtonZ(AllButtonZ3, "Above Bar", pAboveTotalMode, 27, pAboveTotalMode == "None" ? false : true);
					AddButtonZ(AllButtonZ3, "Bar Profile", "", 28, pBarCompositeEnabled);
					AddButtonZ(AllButtonZ3, "Body Mode", pBarBodyMode, 29, pBarBodyMode == "None" ? false : true);	
					AddButtonZ(AllButtonZ3, "Opacity Mode", pPrintBarOpacityMode, 30, pPrintBarOpacityMode == "None" ? false : true);	
					AddButtonZ(AllButtonZ3, "Color Mode", pPrintBarFillMode, 31, true);					
					AddButtonZ(AllButtonZ3, "Print Mode", PrintNumberDisplayMode, 32, true);					
					
					RegisterPanel();
					
					
					
					
					
					
					
					
//					AddButtonZ(AllButtonZ4, "Auto", "", 2, pAutoEnabled);
//					AddButtonZ(AllButtonZ4, "Close", "", 2, true);
//					AddButtonZ(AllButtonZ4, "B/E", "", 2, true);
//					AddButtonZ(AllButtonZ4, pThisEntryType, "", 2, true);	
//					AddButtonZ(AllButtonZ4, "Sell Stack", "", 2, SellStackReady);	
//					AddButtonZ(AllButtonZ4, "Buy Stack", "", 2, BuyStackReady);
//					AddButtonZ(AllButtonZ4, "Sell Market", "", 2, true);	
//					AddButtonZ(AllButtonZ4, "Buy Market", "", 2, true);		
					
					
				
					
					
					pAutoEnabled = false;
					
					
	                string FS = ThisTickSizze.ToString();
	                if (FS.Contains("E-"))
	                {
	                    FS = FS.Substring(FS.IndexOf("E-") + 2);
	                    PriceDigits = int.Parse(FS);
	                }
	                else PriceDigits = Math.Max(0, FS.Length - 2);
	                PriceString = "n" + PriceDigits;


					
						


				}
				else if (State == State.Historical)
				{
					//if (!Permission)
					//	return;				
					
			

				}			
				else if (State == State.Transition)
				{
					//if (!Permission)
					//	return;
					
					//ChartBarsSwitch(false);
				

					
					// force display to show the current bar in progress in on render
					
					if (ChartControl != null)
					ChartControl.Dispatcher.InvokeAsync(() =>
					{
						
										
					if (!pZoomEnabled)
						SetBarSize(pCurrentSetting, pSpaceBetweenBars);	
					else
						SetBarSize(pCurrentSetting2, pSpaceBetweenBars2);	
					
						ChartControl.ChartPanels[0].InvalidateVisual();	
			
					});		
					
					
				}
				else if (State == State.Realtime)
				{
					//if (!Permission)
					//	return;
					
					lastRefresh	= DateTime.Now;
					
					EndLoad = DateTime.Now;
					
					TotalLoadTime = EndLoad.Subtract(StartLoad);

					if (TestLoad) Print("Total Load Time:  " + TotalLoadTime);
					
					
					// Bar Counter
					
									
	//				if (timer == null)
	//				{
	//					if (Bars.BarsType.IsTimeBased && Bars.BarsType.IsIntraday)
	//					{
	//						lock (Connection.Connections)
	//						{
	//							if (Connection.Connections.ToList().FirstOrDefault(c => c.Status == ConnectionStatus.Connected && c.InstrumentTypes.Contains(Instrument.MasterInstrument.InstrumentType)) == null)
	//							{
	//								//Draw.TextFixed(this, "NinjaScriptInfo", NinjaTrader.Custom.Resource.BarTimerDisconnectedError, TextPosition.BottomRight);
	//							}
	//							else
	//							{
	//								if (!SessionIterator.IsInSession(Now, false, true))
	//								{
	//									//Draw.TextFixed(this, "NinjaScriptInfo", NinjaTrader.Custom.Resource.BarTimerSessionTimeError, TextPosition.BottomRight);
	//								}
	//								else
	//								{
	//									//Draw.TextFixed(this, "NinjaScriptInfo", NinjaTrader.Custom.Resource.BarTimerWaitingOnDataError, TextPosition.BottomRight);
	//								}
	//							}
	//						}
	//					}
	//					else
	//					{
	//						//Draw.TextFixed(this, "NinjaScriptInfo", NinjaTrader.Custom.Resource.BarTimerTimeBasedError, TextPosition.BottomRight);
	//					}
	//				}
					
					
				}
				
				
							
//			    if (State == State.DataLoaded)
//			    {
//			        ChartControl.PreviewMouseWheel += ChartControl_PreviewMouseWheel;
//			    }
//			    else if (State == State.Terminated)
//			    {
//			        ChartControl.PreviewMouseWheel -= ChartControl_PreviewMouseWheel;
//			    }






				
			if(State == State.DataLoaded)
			{
				isVolume 		= BarsPeriod.BarsPeriodType == BarsPeriodType.Volume;
				isVolumeBase 	= (BarsPeriod.BarsPeriodType == BarsPeriodType.HeikenAshi || BarsPeriod.BarsPeriodType == BarsPeriodType.Volumetric) && BarsPeriod.BaseBarsPeriodType == BarsPeriodType.Volume;
			}
			
//				if ( State == State.Terminated )
//				{
	                
//					if (ChartControl != null)
//					{
//						ChartControl.Dispatcher.InvokeAsync((Action)(() =>
//						{
//							RemoveWPFControls();
//						}));
//					}
//				}
					

//				else if (State == State.Historical)
//				{
//					if (ChartControl != null)
//					{
//						ChartControl.Dispatcher.InvokeAsync((Action)(() =>
//						{
//							InsertWPFControls();
//						}));
//					}
//				}
				
				
			
			
			
			
			// toolbar button
				
//				if (State == State.Historical)
//				{
//					if (ChartControl != null)
//					{
//						ChartControl.Dispatcher.InvokeAsync((Action)(() =>
//						{
//							InsertWPFToolbarControls();
//						}));
//					}
//				}
//				else if (State == State.Terminated)
//				{
//					if (ChartControl != null)
//					{
//						ChartControl.Dispatcher.InvokeAsync((Action)(() =>
//						{
//							RemoveWPFToolbarControls();
//						}));
//					}
//				}			
			
				
				
				
				if (State == State.Historical)
				{
					if (pOrderPanelOn)
					if (ChartControl != null)
					{
						ChartControl.Dispatcher.InvokeAsync((Action)(() =>
						{
							CreateWPFOrderControls();
							
						//	if (!pOrderPanelOn)
						//		DisposeWPFOrderControls();
						}));
					}
				}
				else if (State == State.Terminated)
				{
					if (pOrderPanelOn)
					if (ChartControl != null)
					{
						ChartControl.Dispatcher.InvokeAsync((Action)(() =>
						{
							DisposeWPFOrderControls();
						}));
					}
				}
				
				
					
			
			
			
			}
			catch (Exception ex)
			{
				if (TestRender) Print("OnStateChange: " + ex.Message + " ");
				
			}
			
		}

		private void buttonGridKey(object sender, KeyEventArgs e)
		{
		
			
		//	e.Handled = true;
			
		}
		
		
		private void ChartControl_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			bool ctrl  = (System.Windows.Input.Keyboard.Modifiers & System.Windows.Input.ModifierKeys.Control) == System.Windows.Input.ModifierKeys.Control;
			bool shift = (System.Windows.Input.Keyboard.Modifiers & System.Windows.Input.ModifierKeys.Shift)   == System.Windows.Input.ModifierKeys.Shift;
			bool alt   = (System.Windows.Input.Keyboard.Modifiers & System.Windows.Input.ModifierKeys.Alt)     == System.Windows.Input.ModifierKeys.Alt;

			if (!ctrl || !shift) return;

			// Ctrl+Shift + [= / OemPlus]  -> wider bars
			// Ctrl+Shift + [- / OemMinus] -> narrower bars
			// Add Alt to adjust spacing instead of width.

			bool widen   = (e.Key == System.Windows.Input.Key.OemPlus  || e.Key == System.Windows.Input.Key.Add);
			bool narrow  = (e.Key == System.Windows.Input.Key.OemMinus || e.Key == System.Windows.Input.Key.Subtract);

			if (!widen && !narrow) return;

			int delta = widen ? 2 : -2;

			if (alt)
			{
				int sDelta = widen ? 1 : -1;
				if (!pZoomEnabled) pSpaceBetweenBars  = Math.Max(1, pSpaceBetweenBars  + sDelta);
				else               pSpaceBetweenBars2 = Math.Max(1, pSpaceBetweenBars2 + sDelta);
			}
			else
			{
				if (!pZoomEnabled) pCurrentSetting  = Math.Max(8, pCurrentSetting  + delta);
				else               pCurrentSetting2 = Math.Max(8, pCurrentSetting2 + delta);
			}

			if (!pZoomEnabled) SetBarSize(pCurrentSetting,  pSpaceBetweenBars);
			else               SetBarSize(pCurrentSetting2, pSpaceBetweenBars2);

			e.Handled = true;
		}
		private void ChartControl_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			
				if (!pUseYScroll)
				return;

				// Over the left-side button menu: don't consume the wheel — let it
				// bubble to OnPanelMouseWheel so it scrolls the buttons instead.
				if (InMenu3 || IsAnyPanelMenuOpen())
				return;
			
			//chartScale.Properties.YAxisRangeType = YAxisRangeType.Automatic;	
			
			MouseWheelDone = true;
			
			
			//ChartControl.LastSlotPainted = CurrentBars[0];
			
			
			int pAmountPerScroll = pScrollTicks;
			
			
			
			int moveit = -1 * pAmountPerScroll;
			
			if (e.Delta > 0)
				moveit = Math.Abs(moveit);
			
			TicksAdjust = TicksAdjust + moveit;
			
					//	MaxValue = ScaleHigh.GetValueAt(ChartBars.ToIndex) + TickSize*TicksAdjust;
					//		MinValue = ScaleLow.GetValueAt(ChartBars.ToIndex) + TickSize*TicksAdjust;
			
		
//					if (!pZoomEnabled)
//						SetBarSize(pCurrentSetting, pSpaceBetweenBars);
//					else
//						SetBarSize(pCurrentSetting2, pSpaceBetweenBars2);			

			IsScrolled = true;	
			
			//ChartControl.ChartPanels[0].InvalidateVisual();	
			this.ChartControl.InvalidateVisual();
			
			//ForceRefresh();
			//ChartControl.InvalidateVisual();
		
			//ChartControl.ChartPanels[0].InvalidateVisual(); 
			
		    e.Handled = true;
		}





		// chart sidebar
		
//							if (buttonn == "Market" || buttonn == "Limit")
//					{
//						if (pThisEntryType == "Market")
//						{
//							pThisEntryType = "Limit";
							
//						}
//						else
//						{
//							pThisEntryType = "Market";
//						}
						
//						thisbutton.Value.Text = pThisEntryType;
//					    this.ChartControl.InvalidateVisual();
//						this.ChartControl.InvalidateVisual();							
//					}				
								
//					else if (buttonn == "Auto")
//					{
									
//					    if (pAutoEnabled)
//					    {
//					        pAutoEnabled = false;
//					    }
//					    else
//					    {
//					        pAutoEnabled = true;
//					    }
//					    thisbutton.Value.Switch = pAutoEnabled;
//					    this.ChartControl.InvalidateVisual();
//						this.ChartControl.InvalidateVisual();					
						
//					}
				
					
			
		private void UpdateButtons()
		{
			
			if (ChartControl != null)
			ChartControl.Dispatcher.InvokeAsync((Action)(() =>
			{
				
				
			
		

			if (pUseSLM)
		    {
				
				button11.Background = pColorButtonOrderType;
				button11.Foreground = GetTextColor(pColorButtonOrderType);
				button11.Content = "SLM";
		    }
		    else
		    {
				button11.Background = pColorButtonOrderType;
				button11.Foreground = GetTextColor(pColorButtonOrderType);	
				button11.Content = "STP";		
		    }
			
		
			
			if (pUseMIT)
		    {
				button12.Background = pColorButtonOrderType;
				button12.Foreground = GetTextColor(pColorButtonOrderType);
				button12.Content = "MIT";
		    }
		    else
		    {
				button12.Background = pColorButtonOrderType;
				button12.Foreground = GetTextColor(pColorButtonOrderType);	
				button12.Content = "LMT";
						
		    }
			
			if (pWashoutEntryOrdersIB)
		    {
				button20.Background = pColorNeutralEMA;
				button20.Foreground = GetTextColor(pColorNeutralEMA);
				button20.Content = "IB";
		    }
		    else
		    {
				button20.Background = pColorNeutralEMA2;
				button20.Foreground = GetTextColor(pColorNeutralEMA2);	
				button20.Content = "IB";
						
		    }		
				

				
				button1.Background = pColorBuyEMA2;
				button1.Foreground = GetTextColor(pColorBuyEMA2);	
				button1.Content = "Market";

				button2.Background = pColorSellEMA2;
				button2.Foreground = GetTextColor(pColorSellEMA2);
				button2.Content = "Market";
			
				button14.Background = pColorBuyEMA2;
				button14.Foreground = GetTextColor(pColorBuyEMA2);	
				button14.Content = "Limit " + pLimitOrderOffset4;
			
				button15.Background = pColorSellEMA2;
				button15.Foreground = GetTextColor(pColorSellEMA2);
				button15.Content = "Limit " + pLimitOrderOffset4;
			
				button16.Background = pColorBuyEMA2;
				button16.Foreground = GetTextColor(pColorBuyEMA2);	
				button16.Content = "Limit " + pLimitOrderOffset5;
			
				button17.Background = pColorSellEMA2;
				button17.Foreground = GetTextColor(pColorSellEMA2);
				button17.Content = "Limit " + pLimitOrderOffset5;
			
			
			
			
			
			
				button5.Background = pColorButtonCloseAll;
				button5.Foreground = GetTextColor(pColorButtonCloseAll);
				
				button13.Background = pColorButtonReverse;
				button13.Foreground = GetTextColor(pColorButtonReverse);		
			
			
			
			

		
		
			
			
			
			
			if (pBuyStackMode == "Long")
			{
				if (BuyStackReady)
					toggle1.Background = pColorBuyEMA;
				else
					toggle1.Background = pColorBuyEMA2;
			}
			else
			{
				if (BuyStackReady)
					toggle1.Background = pColorSellEMA;
				else
					toggle1.Background = pColorSellEMA2;
			}
						
			if (pSellStackMode == "Long")
			{
				if (SellStackReady)
					toggle2.Background = pColorBuyEMA;
				else
					toggle2.Background = pColorBuyEMA2;
			}
			else
			{
				if (SellStackReady)
					toggle2.Background = pColorSellEMA;
				else
					toggle2.Background = pColorSellEMA2;
			}			
			
			
			if (pBuyCloseMode == "Long")
			{
				if (BuyCloseReady)
					toggle3.Background = pColorBuyEMA;
				else
					toggle3.Background = pColorBuyEMA2;
			}
			else if (pBuyCloseMode == "Short")
			{
				if (BuyCloseReady)
					toggle3.Background = pColorSellEMA;
				else
					toggle3.Background = pColorSellEMA2;
			}
			else
			{
				if (BuyCloseReady)
					toggle3.Background = pColorNeutralEMA;
				else
					toggle3.Background = pColorNeutralEMA2;
			}
			
				
			if (pSellCloseMode == "Long")
			{
				if (SellCloseReady)
					toggle4.Background = pColorBuyEMA;
				else
					toggle4.Background = pColorBuyEMA2;
			}
			else if (pSellCloseMode == "Short")
			{
				if (SellCloseReady)
					toggle4.Background = pColorSellEMA;
				else
					toggle4.Background = pColorSellEMA2;
			}
			else
			{
				if (SellCloseReady)
					toggle4.Background = pColorNeutralEMA;
				else
					toggle4.Background = pColorNeutralEMA2;
			}
			
			

			if (pBuyWashoutMode == "Long")
			{
				if (BuyWashoutReady)
					toggle8.Background = pColorBuyEMA;
				else
					toggle8.Background = pColorBuyEMA2;
			}
			else if (pBuyWashoutMode == "Short")
			{
				if (BuyWashoutReady)
					toggle8.Background = pColorSellEMA;
				else
					toggle8.Background = pColorSellEMA2;
			}

			
				
			if (pSellWashoutMode == "Long")
			{
				if (SellWashoutReady)
					toggle9.Background = pColorBuyEMA;
				else
					toggle9.Background = pColorBuyEMA2;
			}
			else if (pSellWashoutMode == "Short")
			{
				if (SellWashoutReady)
					toggle9.Background = pColorSellEMA;
				else
					toggle9.Background = pColorSellEMA2;
			}

			
			
			
				if (BuyStackReady)
			    {
			        button3.Background = pColorBuyEMA;
					button3.Foreground = GetTextColor(pColorBuyEMA);
					
				}
			    else
			    {
			        button3.Background = pColorBuyEMA2;
					button3.Foreground = GetTextColor(pColorBuyEMA2);
			    }			
				
				if (SellStackReady)
			    {
			        button4.Background = pColorSellEMA;
					button4.Foreground = GetTextColor(pColorSellEMA);
			    }
			    else
			    {
					button4.Background = pColorSellEMA2;
					button4.Foreground = GetTextColor(pColorSellEMA2);
			    }	
				
				
				
				
				if (BuyCloseReady)
			    {
			        button9.Background = pColorBuyEMA;
					button9.Foreground = GetTextColor(pColorBuyEMA);
					
				}
			    else
			    {
			        button9.Background = pColorBuyEMA2;
					button9.Foreground = GetTextColor(pColorBuyEMA2);
			    }			
				
				if (SellCloseReady)
			    {
			        button10.Background = pColorSellEMA;
					button10.Foreground = GetTextColor(pColorSellEMA);
			    }
			    else
			    {
			       button10.Background = pColorSellEMA2;
					button10.Foreground = GetTextColor(pColorSellEMA2);
			    }	
				
				
				if (BuyWashoutReady)
			    {
			        button18.Background = pColorBuyEMA;
					button18.Foreground = GetTextColor(pColorBuyEMA);
					
				}
			    else
			    {
			        button18.Background = pColorBuyEMA2;
					button18.Foreground = GetTextColor(pColorBuyEMA2);
			    }			
				
				if (SellWashoutReady)
			    {
			        button19.Background = pColorSellEMA;
					button19.Foreground = GetTextColor(pColorSellEMA);
			    }
			    else
			    {
			       button19.Background = pColorSellEMA2;
					button19.Foreground = GetTextColor(pColorSellEMA2);
			    }	
				
				
				
				
				
				
				if (BuyClickReady)
			    {
			        button7.Background = pColorBuyEMA;
					button7.Foreground = GetTextColor(pColorBuyEMA);
					button7.Content = "Cancel";
					
				}
			    else
			    {
			        button7.Background = pColorBuyEMA2;
					button7.Foreground = GetTextColor(pColorBuyEMA2);
					button7.Content = "Buy Click";
			    }			
				
				if (SellClickReady)
			    {
			        button8.Background = pColorSellEMA;
					button8.Foreground = GetTextColor(pColorSellEMA);
					button8.Content = "Cancel";
			    }
			    else
			    {
			        button8.Background = pColorSellEMA2;
					button8.Foreground = GetTextColor(pColorSellEMA2);
					button8.Content = "Sell Click";
			    }					
				
				
				if (LongEntryOrder != null)
			    {
			        button6.Background = pColorBracket;
					button6.Foreground = GetTextColor(pColorBracket);	
					button6.Content = "Cancel";
					
				}
			    else
			    {
			        button6.Background = pColorBracket2;
					button6.Foreground = GetTextColor(pColorBracket2);		
					button6.Content = "Bracket";
			    }	
				
				//button6.Foreground = Brushes.Black;
				
				
				// OffsetBox fade and combo bbox
				
				if (pThisEntryType1 == "Market")
				{
					cb1.SelectedIndex = 0;
					OffsetBox1.IsEnabled = false;
				}
				else if (pThisEntryType1 == "Limit")
				{
					cb1.SelectedIndex = 1;
					OffsetBox1.IsEnabled = true;
				}
				
				if (pThisEntryType2 == "Market")
				{
					cb2.SelectedIndex = 0;
					OffsetBox2.IsEnabled = false;
				}
				else if (pThisEntryType2 == "Limit")
				{
					cb2.SelectedIndex = 1;
					OffsetBox2.IsEnabled = true;
				}
				
				if (pThisEntryType3 == "STP")
				{
					cb3.SelectedIndex = 0;
					OffsetBox5.IsEnabled = false;
				}
				else if (pThisEntryType3 == "SLM")
				{
					cb3.SelectedIndex = 1;
					OffsetBox5.IsEnabled = true;
				}
				
				if (pUseSLM)
				{
					OffsetBox4.IsEnabled = true;	
				}
				else
				{
					OffsetBox4.IsEnabled = false;	
				}
				
				if (pThisEntryType4 == "Market")
				{
					cb4.SelectedIndex = 0;
					OffsetBox6.IsEnabled = false;
				}
				else if (pThisEntryType4 == "Limit")
				{
					cb4.SelectedIndex = 1;
					OffsetBox6.IsEnabled = true;
				}
				
				
				
				
			}));
					
			
			
							
//				CheckExecutionStatus();
			
				
//				// ENTRY BUTTON
				
//				if (pEntriesEnabled)
//				{
//					newButton3.Content = "Entry On";
//					newButton3.Background = pButtonColorOn;
//				}
//				else
//				{
//					newButton3.Content = "Entry Off";
//					newButton3.Background = pButtonColorOff;
//				}
				
				

//				// TRADES BUTTON
				
//				if (pLongEnabled && pShortEnabled)
//				{
//					newButton4.Content = "All Trades";
//					newButton4.Background = pButtonColorOn;
//				}
//				else if (pLongEnabled)
//				{
//					newButton4.Content = "Long Only";	
//					newButton4.Background = pButtonColorLong;
//				}
//				else
//				{
//					newButton4.Content = "Short Only";
//					newButton4.Background = pButtonColorShort;
//				}		
				

				
		
				
				
//				if (pAutoEnabled)
//				{
//					newButton7.Background = pButtonColorOn;
					
//				}
//				else
//				{
//					newButton7.Background = pButtonColorOff;
//				}
					
		
				
//				if (pDoReverse)
//				{
//					newButton8.Background = pButtonColorOn;
//				}
//				else
//				{
//					newButton8.Background = pButtonColorOff;
//				}
							
			
				
		
		
			
		}
		
		private bool CheckOrderPanelReady()
		{
				
			if (!connected)
				return false;
			
//			pQty = chartTraderQty.Value;
			

//			if (chartTraderAcct.SelectedAccount != null)
//			{
//				myAccount = chartTraderAcct.SelectedAccount;
//				pAccountName = myAccount.Name;
//			}
			
//			//if (!chartTraderATM.SelectedAtmStrategy.DisplayName.Contains(" - "))
			
//			try
//			{
//				pATMName = chartTraderATM.SelectedAtmStrategy.DisplayName;
				
//				if (IsActiveATM())
//					pATMName = sep(pATMName);
			
//				pTIF = chartTraderTIF.SelectedTif.ToString().ToUpper();
			
//				if (PreviousAccountName != pAccountName)
//				{
//					Subscribe();
					
//					PreviousAccountName = pAccountName;
//				}
			
//			}
//			catch
//			{}
		
			//chartTrader.
			
			
			
			
			
			// old code to require charttrader to be up for order flags
			
			
//			if (chartTraderAcct == null)
//			{

//				if (chartTrader.Visibility != Visibility.Collapsed)
//				{				
//					InsertWPFControls();
//				}
//				else
//				{
//					//AddError("Please make sureChartTrader is visible or hidden and Reload NinjaScript.");
//					AddError("Please make sure ChartTrader is set to visible or hidden.");
//					ChartControl.InvalidateVisual();
					
//					return false;					
						
//				}
					

//			}
			
//			if (chartTrader.Visibility == Visibility.Collapsed)
//			{
				
//				AddError("Please make sure ChartTrader is set to visible or hidden.");
//				ChartControl.InvalidateVisual();
				
//				return false;				
//			}
					
			
					
			return true;
			
		}
		
		protected void CB1Click (object sender, RoutedEventArgs e)
		{
			if (cb1.SelectedIndex == 0)
				pThisEntryType1 = "Market";
			
			if (cb1.SelectedIndex == 1)
				pThisEntryType1 = "Limit";
			
			UpdateButtons();
			
			
			ChartControl.InvalidateVisual();
		}

		protected void CB2Click (object sender, RoutedEventArgs e)
		{
			if (cb2.SelectedIndex == 0)
				pThisEntryType2 = "Market";
			
			if (cb2.SelectedIndex == 1)
				pThisEntryType2 = "Limit";
			
			UpdateButtons();
			
			
			ChartControl.InvalidateVisual();
		}
		
		protected void CB3Click (object sender, RoutedEventArgs e)
		{
			if (cb3.SelectedIndex == 0)
				pThisEntryType3 = "STP";
			
			if (cb3.SelectedIndex == 1)
				pThisEntryType3 = "SLM";
			
			UpdateButtons();
			
			
			ChartControl.InvalidateVisual();
		}
		
		protected void CB4Click (object sender, RoutedEventArgs e)
		{
			if (cb4.SelectedIndex == 0)
				pThisEntryType4 = "Market";
			
			if (cb4.SelectedIndex == 1)
				pThisEntryType4 = "Limit";
			
			UpdateButtons();
			
			
			ChartControl.InvalidateVisual();
		}
		
		protected void OffsetBox4Click (object sender, RoutedEventArgs e)
		{

			pSLOffset = Math.Max(0,OffsetBox4.Value);
			
			UpdateButtons();
			
			ChartControl.InvalidateVisual();
		}
		
		protected void OffsetBox5Click (object sender, RoutedEventArgs e)
		{

			pSLOffset2 = Math.Max(0,OffsetBox5.Value);
			
			UpdateButtons();
			
			ChartControl.InvalidateVisual();
		}
		
		protected void OffsetBox6Click (object sender, RoutedEventArgs e)
		{

			pLimitOrderOffset6 = Math.Max(0,OffsetBox6.Value);
			
			UpdateButtons();
			
			ChartControl.InvalidateVisual();
		}
		
		
		
		
		protected void OffsetBox3Click (object sender, RoutedEventArgs e)
		{

			pLimitOrderOffset3 = Math.Max(0,OffsetBox3.Value);
			
			UpdateButtons();
			
			ChartControl.InvalidateVisual();
		}
		
		
		protected void OffsetBox2Click (object sender, RoutedEventArgs e)
		{

			pLimitOrderOffset2 = Math.Max(-10000,OffsetBox2.Value);
			
			UpdateButtons();
			
			ChartControl.InvalidateVisual();
		}
		
		
		protected void OffsetBox1Click (object sender, RoutedEventArgs e)
		{

			pLimitOrderOffset1 = Math.Max(-10000,OffsetBox1.Value);
			
			UpdateButtons();
			
			ChartControl.InvalidateVisual();
		}	
				
		
		
		
		
		
		protected void toggle1_Click(object sender, RoutedEventArgs e)
		{
//			if (!CheckOrderPanelReady())
//				return;
			
			if (BuyStackReady)
			{
				BuyStackReady = false;
				UpdateButtons();
				ChartControl.InvalidateVisual();
				return;
				
			}
			
			if (pBuyStackMode == "Long")
			{
				pBuyStackMode = "Short";
			}
			else
			{
				pBuyStackMode = "Long";
			}
			
			UpdateButtons();
			ChartControl.InvalidateVisual();
		}
		
		
		protected void toggle2_Click(object sender, RoutedEventArgs e)
		{
//			if (!CheckOrderPanelReady())
//				return;
						
			if (SellStackReady)
			{
				SellStackReady = false;
				UpdateButtons();
				ChartControl.InvalidateVisual();
				return;
				
			}
			
			if (pSellStackMode == "Long")
			{
				pSellStackMode = "Short";
			}
			else
			{
				pSellStackMode = "Long";
			}
			
			UpdateButtons();
			ChartControl.InvalidateVisual();
		}
		
		protected void toggle3_Click(object sender, RoutedEventArgs e)
		{
//			if (!CheckOrderPanelReady())
//				return;
			
//			if (BuyCloseReady)
//				return;
			
			if (pBuyCloseMode == "Long")
			{
				pBuyCloseMode = "Short";
			}
			else if (pBuyCloseMode == "Short")
			{
				pBuyCloseMode = "All";
			}
			else
			{
				pBuyCloseMode = "Long";
			}
			
			UpdateButtons();
			ChartControl.InvalidateVisual();
		}
				
		protected void toggle4_Click(object sender, RoutedEventArgs e)
		{
//			if (!CheckOrderPanelReady())
//				return;
			
//			if (SellCloseReady)
//				return;
						
			if (pSellCloseMode == "Short")
			{
				pSellCloseMode = "Long";
			}
			else if (pSellCloseMode == "Long")
			{
				pSellCloseMode = "All";
			}
			else
			{
				pSellCloseMode = "Short";
			}
			
			UpdateButtons();
			ChartControl.InvalidateVisual();
		}
		
		
	protected void toggle8_Click(object sender, RoutedEventArgs e)
		{
//			if (!CheckOrderPanelReady())
//				return;
		
			if (pBuyWashoutMode == "Long")
			{
				pBuyWashoutMode = "Short";
			}
			else if (pBuyWashoutMode == "Short")
			{
				pBuyWashoutMode = "Long";
			}
			
			
			UpdateButtons();
			ChartControl.InvalidateVisual();
		}
				
		protected void toggle9_Click(object sender, RoutedEventArgs e)
		{
//			if (!CheckOrderPanelReady())
//				return;
			
			if (pSellWashoutMode == "Short")
			{
				pSellWashoutMode = "Long";
			}
			else if (pSellWashoutMode == "Long")
			{
				pSellWashoutMode = "Short";
			}

			
			
			UpdateButtons();
			ChartControl.InvalidateVisual();
		}
		
		
		
		
		
		
		protected void Button1_Click(object sender, RoutedEventArgs e)
		{
			if (!CheckOrderPanelReady())
				return;
			
			
			SetOrderParams();
			BuyMarket(pQty, pATMName);
			ChartControl.InvalidateVisual();
		}

		protected void Button2_Click(object sender, RoutedEventArgs e)
		{
			if (!CheckOrderPanelReady())
				return;
			
			SetOrderParams();
			SellMarket(pQty, pATMName);
			ChartControl.InvalidateVisual();
		}
		
					
				

		
		
		protected void Button14_Click(object sender, RoutedEventArgs e)
		{
			if (!CheckOrderPanelReady())
				return;
			


			ChartControl.Dispatcher.InvokeAsync((Action)(() =>
			{
				

				int thisoffset = pLimitOrderOffset4;

			
				SetOrderParams();
					
				//EntryOrderPrice = RTTS(CurrentLastData - thisoffset*tSize);
				EntryOrderPrice = CurrentAsk - thisoffset*tSize;
					
				BuyLimit(RTTS(EntryOrderPrice), false);
				
				
				//EntryOrderPrice = RTTS(CurrentLastData + thisoffset*tSize);
				//EntryOrderPrice = CurrentBid + thisoffset*tSize;
					
				//SellLimit(RTTS(EntryOrderPrice), false);
				
				
			}));
		
			
			ChartControl.InvalidateVisual();
		}

		protected void Button15_Click(object sender, RoutedEventArgs e)
		{
			if (!CheckOrderPanelReady())
				return;
			


			ChartControl.Dispatcher.InvokeAsync((Action)(() =>
			{
				

				int thisoffset = pLimitOrderOffset4;

			
				SetOrderParams();
					
				//EntryOrderPrice = RTTS(CurrentLastData - thisoffset*tSize);
				//EntryOrderPrice = CurrentAsk - thisoffset*tSize;
					
				//BuyLimit(RTTS(EntryOrderPrice), false);
				
				
				//EntryOrderPrice = RTTS(CurrentLastData + thisoffset*tSize);
				EntryOrderPrice = CurrentBid + thisoffset*tSize;
					
				SellLimit(RTTS(EntryOrderPrice), false);
				
				
			}));
		
			
			ChartControl.InvalidateVisual();
		}
		
		protected void Button16_Click(object sender, RoutedEventArgs e)
		{
			if (!CheckOrderPanelReady())
				return;
			
				

				int thisoffset = pLimitOrderOffset5;

			
				SetOrderParams();
					
				//EntryOrderPrice = RTTS(CurrentLastData - thisoffset*tSize);
				EntryOrderPrice = CurrentAsk - thisoffset*tSize;
					
				BuyLimit(RTTS(EntryOrderPrice), false);
				
				
				//EntryOrderPrice = RTTS(CurrentLastData + thisoffset*tSize);
				//EntryOrderPrice = CurrentBid + thisoffset*tSize;
					
				//SellLimit(RTTS(EntryOrderPrice), false);
				
			
			
			ChartControl.InvalidateVisual();
		}

		protected void Button17_Click(object sender, RoutedEventArgs e)
		{
			if (!CheckOrderPanelReady())
				return;
			
				int thisoffset = pLimitOrderOffset5;

			
				SetOrderParams();
					
				//EntryOrderPrice = RTTS(CurrentLastData - thisoffset*tSize);
				//EntryOrderPrice = CurrentAsk - thisoffset*tSize;
					
				//BuyLimit(RTTS(EntryOrderPrice), false);
				
				
				//EntryOrderPrice = RTTS(CurrentLastData + thisoffset*tSize);
				EntryOrderPrice = CurrentBid + thisoffset*tSize;
					
				SellLimit(RTTS(EntryOrderPrice), false);
			
			ChartControl.InvalidateVisual();
		}		
		
		protected void Button3_Click(object sender, RoutedEventArgs e)
		{
			if (!CheckOrderPanelReady())
				return;
			
			if (BuyStackReady)
		    {
		        BuyStackReady = false;
		    }
		    else
		    {
		        BuyStackReady = true;
				
//				BuyStackReady = true;
//				SellStackReady = false;
//				BuyCloseReady = false;
//				SellCloseReady = false;
		    }
			UpdateButtons();
			UpdateButtons();
			ChartControl.InvalidateVisual();
		}

		protected void Button4_Click(object sender, RoutedEventArgs e)
		{
			if (!CheckOrderPanelReady())
				return;
			
			if (SellStackReady)
		    {
		        SellStackReady = false;
		    }
		    else
		    {
		        SellStackReady = true;
				
//				BuyStackReady = false;
//				SellStackReady = true;
//				BuyCloseReady = false;
//				SellCloseReady = false;
				
		    }
			UpdateButtons();
			ChartControl.InvalidateVisual();
		}
		
		protected void Button9_Click(object sender, RoutedEventArgs e)
		{
			if (!CheckOrderPanelReady())
				return;
			
			if (BuyCloseReady)
		    {
		        BuyCloseReady = false;
		    }
		    else
		    {
		        BuyCloseReady = true;
				
//				BuyStackReady = false;
//				SellStackReady = false;
				BuyCloseReady = true;
				SellCloseReady = false;				
		    }
			UpdateButtons();
			ChartControl.InvalidateVisual();
		}

		protected void Button10_Click(object sender, RoutedEventArgs e)
		{
			if (!CheckOrderPanelReady())
				return;
			if (SellCloseReady)
		    {
		        SellCloseReady = false;
		    }
		    else
		    {
		        SellCloseReady = true;
				
//				BuyStackReady = false;
//				SellStackReady = false;
				BuyCloseReady = false;
				SellCloseReady = true;					
		    }
			UpdateButtons();			
			ChartControl.InvalidateVisual();
		}	
		
		

			
	protected void Button18_Click(object sender, RoutedEventArgs e)
		{
			if (!CheckOrderPanelReady())
				return;
			
			if (BuyWashoutReady)
		    {
		        BuyWashoutReady = false;
		    }
		    else
		    {
				
				PreviousLongSignals = CurrentLongSignals;
				PreviousShortSignals = CurrentShortSignals;
				
		        BuyWashoutReady = true;
		
//				BuyWashoutReady = true;
//				SellWashoutReady = false;				
		    }
			UpdateButtons();
			ChartControl.InvalidateVisual();
		}

		protected void Button19_Click(object sender, RoutedEventArgs e)
		{
			if (!CheckOrderPanelReady())
				return;
			if (SellWashoutReady)
		    {
		        SellWashoutReady = false;
		    }
		    else
		    {
				
				PreviousLongSignals = CurrentLongSignals;
				PreviousShortSignals = CurrentShortSignals;
				
				
				SellWashoutReady = true;

//				BuyWashoutReady = false;
//				SellWashoutReady = true;					
		    }
			UpdateButtons();			
			ChartControl.InvalidateVisual();
		}	
		
		
		
		
		
		
		protected void Button11_Click(object sender, RoutedEventArgs e)
		{
			if (!CheckOrderPanelReady())
				return;
			
			if (pUseSLM)
		    {
		        pUseSLM = false;
		    }
		    else
		    {
		        pUseSLM = true;
						
		    }
			
			UpdateButtons();	
			
			ChartControl.InvalidateVisual();
		}	
		
		
		protected void Button12_Click(object sender, RoutedEventArgs e)
		{
			if (!CheckOrderPanelReady())
				return;
			
			
					if (TestBeta)
					{
						SelectedStopRender = SelectedStopRender + 1;
						Print(SelectedStopRender);
						
					}
					
					
			if (pUseMIT)
		    {
		        pUseMIT = false;
		    }
		    else
		    {
		        pUseMIT = true;
						
		    }
			
			UpdateButtons();	
			
			ChartControl.InvalidateVisual();
		}	
				
	
		protected void Button20_Click(object sender, RoutedEventArgs e)
		{
			if (!CheckOrderPanelReady())
				return;
			
			if (pWashoutEntryOrdersIB)
		    {
		        pWashoutEntryOrdersIB = false;
		    }
		    else
		    {
		        pWashoutEntryOrdersIB = true;
						
		    }
			
			UpdateButtons();	
			
			ChartControl.InvalidateVisual();
		}	
		
		
		
		protected void Button7_Click(object sender, RoutedEventArgs e)
		{
			if (!CheckOrderPanelReady())
				return;
			
			if (BuyClickReady)
		    {
		        BuyClickReady = false;
		    }
		    else
		    {
		        BuyClickReady = true;	
				SellClickReady = false;	
		    }

			UpdateButtons();
			ChartControl.InvalidateVisual();
		}

		protected void Button8_Click(object sender, RoutedEventArgs e)
		{
			if (!CheckOrderPanelReady())
				return;
			
			if (SellClickReady)
		    {
		        SellClickReady = false;
		    }
		    else
		    {
				BuyClickReady = false;
		        SellClickReady = true;	
				
		    }
			
			UpdateButtons();
			ChartControl.InvalidateVisual();
		}
		
		protected void Button13_Click(object sender, RoutedEventArgs e)
		{
			
			if (!CheckOrderPanelReady())
				return;
						
			ReverseAll();
			
			ChartControl.InvalidateVisual();
		}

		
		private void ReverseAll()
		{
//			instruction = OIF_ClosePosition(pAccountName, Instrument.FullName);

////			if (ThisPositionNow() != null)
//			if (instruction != string.Empty)
//			{

//				Submit();
//			}
			
			
			CancelStopOrders();	
			CancelTargetOrders();
			//CancelAllOrders();
			
			
			
			
			if (ThisPositionNow() != null)
			{
				if (ThisPositionNow().MarketPosition == MarketPosition.Long)
				{
					SellMarket(ThisPositionNow().Quantity,"");
					
					SetOrderParams();
					SellMarket(pQty, pATMName);
					ChartControl.InvalidateVisual();
				
				}
				
				if (ThisPositionNow().MarketPosition == MarketPosition.Short)
				{
					BuyMarket(ThisPositionNow().Quantity,"");
					
					SetOrderParams();
					BuyMarket(pQty, pATMName);
					ChartControl.InvalidateVisual();
							
				}					
					
					
					
					
			}
			

			
			
		}		
		
		
		
		protected void Button5_Click(object sender, RoutedEventArgs e)
		{
			if (!CheckOrderPanelReady())
				return;
			
				BuyCloseReady = false;
				SellCloseReady = false;		
				BuyClickReady = false;
		        SellClickReady = false;				
				BuyStackReady = false;
		        SellStackReady = false;	
				BuyWashoutReady = false;
		        SellWashoutReady = false;				
			
			UpdateButtons();
			
			CloseAll();
			
			
			
			//ClosePosition();
			ChartControl.InvalidateVisual();
		}

		
		
		
		protected void Button6_Click(object sender, RoutedEventArgs e)
		{
			if (!CheckOrderPanelReady())
				return;
			
			OCOBracketID = string.Concat("VTFBR",pAccountName,SpreadName,DateTime.Now.Ticks);
			
			 
			double ExecutionHigh = CurrentLastData + pLimitOrderOffset3*tSize;
			double ExecutionLow = CurrentLastData +- pLimitOrderOffset3*tSize;
			

		
		
		
						// cancel
						if (LongEntryOrder != null)
						{
							CancelOrder(LongEntryOrder);
							LongEntryOrder = null;
						}
						else
						{
							SaveLongOrder = true;
							BuyStop2(ExecutionHigh);
							
						}
						
						
				
						
						// cancel
						
						if (ShortEntryOrder != null)
						{
							CancelOrder(ShortEntryOrder);
							ShortEntryOrder = null;
						}
						else
						{
							SaveShortOrder = true;
							SellStop2(ExecutionLow);
						}
						
						
				UpdateButtons();		
						
		}
		


		
		
		
		
		private int CTWidth = 200;
		private int CTLeftSpace = 5;
		private int endadd = 3;
		
		
        private void OnAtmCustomPropertiesChanged(object sender, NinjaTrader.Gui.NinjaScript.AtmStrategy.CustomPropertiesChangedEventArgs args)
        {
            // Adjust our TIF and Quantity selectors to the new ATM strategy values
            tifSelector.SelectedTif = args.NewTif;
            qudSelector.Value = args.NewQuantity;
			
			//Print("OnAtmCustomPropertiesChanged");
        }
		
	
				
		protected void CreateWPFOrderControls()
		{
			
			
			int MainButtonHeight = 30;
			int MainButtonHeight2 = 26; // vertical space for order type areas
			
			int MainButtonSpace = 2;
			int MainButtonSpace2 = 0;
			
			int TotalButtonRowH = MainButtonHeight + MainButtonSpace;
			int TotalButtonRowH2 = MainButtonHeight2 + MainButtonSpace2;
			int TotalButtonRowH3 = TotalButtonRowH + TotalButtonRowH2 + 3;
				
				
			chartWindow			= System.Windows.Window.GetWindow(ChartControl.Parent) as Chart;
			chartGrid			= chartWindow.MainTabControl.Parent as System.Windows.Controls.Grid;
			
			buttonGrid = new System.Windows.Controls.Grid();

			buttonGrid.PreviewKeyDown += buttonGridKey;
			
			
			
			buttonGrid.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(CTLeftSpace) }); // width of market / limit
			buttonGrid.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(CTWidth-CTLeftSpace) }); // spacer
			
			buttonGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(0) }); // label at top
			buttonGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(83) }); // setup buttons
			buttonGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(60) }); // pnl display
			buttonGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(TotalButtonRowH + 2 + pSpaceBetweenGroups) }); // close button
			
			bool OneEnabled = pMarketEntryOrdersEnabled || pLimit1EntryOrdersEnabled || pLimit2EntryOrdersEnabled;
			
			if (pMarketEntryOrdersEnabled) 
			{
				buttonGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(TotalButtonRowH + 0) });
			}
			
			if (pLimit1EntryOrdersEnabled)
			{
				buttonGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(TotalButtonRowH + 0) });
			}
			if (pLimit2EntryOrdersEnabled)
			{
				buttonGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(TotalButtonRowH + 0) });
			}
			
			if (OneEnabled)
				buttonGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(pSpaceBetweenGroups) });
			
		
			if (pClickEntryOrdersEnabled) buttonGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(TotalButtonRowH3 + pSpaceBetweenGroups - 1) });
			if (pCloseEntryOrdersEnabled) buttonGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(TotalButtonRowH3 + pSpaceBetweenGroups) });
			if (pWashoutEntryOrdersEnabled) buttonGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(TotalButtonRowH3 + pSpaceBetweenGroups) });
			if (pStackEntryOrdersEnabled) buttonGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(TotalButtonRowH3 + pSpaceBetweenGroups) });
			if (pBracketEntryOrdersEnabled) buttonGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(TotalButtonRowH3 + pSpaceBetweenGroups) });
		
		
					
//			if (pMarketEntryOrdersEnabled)
//			{
//				ii++;
//				System.Windows.Controls.Grid.SetRow(MarketButtons, ii);
//			}
//			if (pClickEntryOrdersEnabled)
//			{
//				ii++;
//				System.Windows.Controls.Grid.SetRow(ClickButtons, ii);
//			}
//			if (pCloseEntryOrdersEnabled)
//			{
//				ii++;
//				System.Windows.Controls.Grid.SetRow(CloseButtons, ii);
//			}
//			if (pStackEntryOrdersEnabled)
//			{
//				ii++;
//				System.Windows.Controls.Grid.SetRow(StackButtons, ii);
//			}
			
			
				

			accountSelector = new NinjaTrader.Gui.Tools.AccountSelector();

			if (pLastAccount != string.Empty)
				accountSelector.DesiredAccount = pLastAccount;

			accountSelector.ToolTip = pLastAccount;
			//accountSelector.sele
//			pQty = qudSelector.Value;
			
//			myAccount = accountSelector.SelectedAccount;
//			pAccountName = myAccount.Name;
		
//			pATMName = atmStrategySelector.SelectedAtmStrategy.DisplayName;
				
//			if (IsActiveATM())
//				pATMName = sep(pATMName);
			
//			pTIF = tifSelector.SelectedTif.ToString().ToUpper();
				
			
			
			
						// When the account selector's selection changes, unsubscribe and resubscribe
						accountSelector.SelectionChanged += (o, args) =>
						{
							
										
							//Print("account selection changes");
							
							SetOrderParams();
							
							if (accountSelector.SelectedAccount != null)
							{
								myAccount = accountSelector.SelectedAccount;
								accountSelector.ToolTip = myAccount.DisplayName;

								bool ISSIM = myAccount.Provider == Provider.Simulator;
								
								
								pLastAccount = accountSelector.DesiredAccount;

								//if (myAccount.DisplayName.Contains("Sim"))
									
								if (ISSIM)
									buttonGrid.Background = pSimBackColor;
								else
									buttonGrid.Background =pLiveBackColor;
								
								
								// Unsubscribe to any prior account subscriptions
								accountSelector.SelectedAccount.AccountItemUpdate -= OnAccountItemUpdate2;
								accountSelector.SelectedAccount.ExecutionUpdate -= OnExecutionUpdate2;
								accountSelector.SelectedAccount.OrderUpdate -= OnOrderUpdate2;
								accountSelector.SelectedAccount.PositionUpdate -= OnPositionUpdate2;

								// Subscribe to new account subscriptions
								accountSelector.SelectedAccount.AccountItemUpdate   += OnAccountItemUpdate2;
								accountSelector.SelectedAccount.ExecutionUpdate     += OnExecutionUpdate2;
								accountSelector.SelectedAccount.OrderUpdate         += OnOrderUpdate2;
								accountSelector.SelectedAccount.PositionUpdate      += OnPositionUpdate2;
								
								//Print(myAccount.Name);
								
								UpdatePNLBox(ThisPositionNow());
								UpdateQTYBox(ThisPositionNow());
								
								
								
							}
						};			
			
			
			
					
						qudSelector2 = new NinjaTrader.Gui.Tools.QuantitySelector();
						
						//qudSelector = LogicalTreeHelper.FindLogicalNode(pageContent, "qudSelector") as QuantityUpDown;
					           
						qudSelector = new NinjaTrader.Gui.Tools.QuantityUpDown();
						qudSelector.Value = pLastQuantity;
						qudSelector.Minimum = 1;
						qudSelector.Height = 21;
						
						qudSelector.ValueChanged += (o, args) =>
						{
							if (!CheckOrderPanelReady())
								return;
										
							SetOrderParams();		
				
							
						};
							
						qudSelector.KeyDown += (o, args) =>
						{
							
							//Print(args.Key.ToString());
							
							
							
							args.Handled = true;
						};
						// Find ATM Strategy selector and attach event handler
						//atmStrategySelector = LogicalTreeHelper.FindLogicalNode(pageContent, "atmStrategySelector") as AtmStrategy.AtmStrategySelector;
						
						
						atmStrategySelector = new NinjaTrader.Gui.NinjaScript.AtmStrategy.AtmStrategySelector();
						
						
						
						atmStrategySelector.Id = Guid.NewGuid().ToString("N");
						
						
						if (atmStrategySelector != null)
							atmStrategySelector.CustomPropertiesChanged += OnAtmCustomPropertiesChanged;

						// Be sure to bind our account selector to our ATM strategy selector to ensure proper functionality
						atmStrategySelector.SetBinding(NinjaTrader.Gui.NinjaScript.AtmStrategy.AtmStrategySelector.AccountProperty, new Binding { Source = accountSelector, Path = new PropertyPath("SelectedAccount") });

						
						if (pLastATM != -10)
							atmStrategySelector.SelectedIndex = pLastATM;
						
						// When our ATM selector's selection changes
						atmStrategySelector.SelectionChanged += (o, args) =>
						{
							
							if (!CheckOrderPanelReady())
								return;
										
							//SetOrderParams();							
							
							
							if (atmStrategySelector.SelectedItem == null)
							{
								//Print("null");	
								
								
								return;
							}
							//tifSelector.SelectedTif = args.NewTif;
            				
							
							
							if (args.AddedItems.Count > 0)
							{
								// Change the selected TIF in our TIF selector too
								NinjaTrader.NinjaScript.AtmStrategy selectedAtmStrategy = args.AddedItems[0] as NinjaTrader.NinjaScript.AtmStrategy;
								
								pLastATM = -10;
								
								if (selectedAtmStrategy != null)
								{
									//Print(selectedAtmStrategy.EntryQuantity);
									
									pLastATM = atmStrategySelector.SelectedIndex;
									
									
									//Print(atmStrategySelector.SelectedIndex);
									
									qudSelector.Value = selectedAtmStrategy.EntryQuantity;
									tifSelector.SelectedTif = selectedAtmStrategy.TimeInForce;
								}
								
								
							}
							
							SetOrderParams();
							
						};			
						
						// Find TIF selector
						//tifSelector = LogicalTreeHelper.FindLogicalNode(pageContent, "tifSelector") as TifSelector;
						
						tifSelector = new NinjaTrader.Gui.Tools.TifSelector();
						tifSelector.Height = 21;
						
						// Be sure to bind our account selector to our TIF selector to ensure proper functionality
						tifSelector.SetBinding(TifSelector.AccountProperty, new Binding { Source = accountSelector, Path = new PropertyPath("SelectedAccount") });


						// When our TIF selector's selection changes
						tifSelector.SelectionChanged += (o, args) =>
						{
							
							if (!CheckOrderPanelReady())
								return;
										
							//SetOrderParams();							
														
							// Change the selected TIF in the ATM strategy too
							if (atmStrategySelector.SelectedAtmStrategy != null)
							{
								atmStrategySelector.SelectedAtmStrategy.TimeInForce = tifSelector.SelectedTif;
								
							
							}
							
							SetOrderParams();
							
						};
						
						
						
						
						
				//SetOrderParams();		
						
					
						
	
						
						
						
			QtyTIFGrid = new System.Windows.Controls.Grid();
			QtyTIFGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(0) }); // spacer
			QtyTIFGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(25) }); // height of market / limit / offset	
						
			//QtyTIFGrid.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(5) }); 
				
			int widetif = 95;	
												
			int wideqty = CTWidth - CTLeftSpace - CTLeftSpace - widetif - 5 - endadd;
						
			QtyTIFGrid.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(wideqty) }); // width of market / limit
			QtyTIFGrid.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(5) }); // spacer
			QtyTIFGrid.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(widetif) }); // width of offset		
						
			QtyTIFGrid.Children.Add(qudSelector);
			System.Windows.Controls.Grid.SetColumn(qudSelector, 0);
			System.Windows.Controls.Grid.SetRow(qudSelector, 1);
						
//			QtyTIFGrid.Children.Add(qudSelector2);
//			System.Windows.Controls.Grid.SetColumn(qudSelector2, 0);
//			System.Windows.Controls.Grid.SetRow(qudSelector2, 1);
						
			QtyTIFGrid.Children.Add(tifSelector);
			System.Windows.Controls.Grid.SetColumn(tifSelector, 2);
			System.Windows.Controls.Grid.SetRow(tifSelector, 1);
			//tifSelector.Width = widetif	- 10;		
						
						
			SetupButtons = new System.Windows.Controls.Grid();			
						
			SetupButtons.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(0) }); // width of market / limit
			SetupButtons.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(CTWidth - CTLeftSpace - CTLeftSpace - endadd) }); // spacer	
						
			SetupButtons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(5) });
			SetupButtons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(25) });
			SetupButtons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(25) });
			SetupButtons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(25) });
						
			
						
						
			SetupButtons.Children.Add(accountSelector);
			SetupButtons.Children.Add(atmStrategySelector);
			SetupButtons.Children.Add(QtyTIFGrid);					
			//SetupButtons.Children.Add(tifSelector);
			
			
		
			System.Windows.Controls.Grid.SetColumn(accountSelector, 1);
			System.Windows.Controls.Grid.SetColumn(atmStrategySelector, 1);
			System.Windows.Controls.Grid.SetColumn(QtyTIFGrid, 1);
			//System.Windows.Controls.Grid.SetColumn(tifSelector, 0);
			
			System.Windows.Controls.Grid.SetRow(accountSelector, 1);
			System.Windows.Controls.Grid.SetRow(atmStrategySelector, 2);			
			System.Windows.Controls.Grid.SetRow(QtyTIFGrid, 3);
			//System.Windows.Controls.Grid.SetRow(tifSelector, 4);						
						
					
			label = new System.Windows.Controls.TextBlock()
			{
				FontFamily			= ChartControl.Properties.LabelFont.Family,
				FontSize			= 13,
				Foreground			= ChartControl.Properties.ChartText,
				HorizontalAlignment	= HorizontalAlignment.Center,
				Margin				= new Thickness(5, 5, 5, 5),
				Text				= string.Format("{0} {1} {2}", Instrument.FullName, BarsPeriod.Value, BarsPeriod.BarsPeriodType)
			};

			//label.Text = indicatorVersion;
			
			System.Windows.Controls.Grid.SetRow(label, 0);
			//buttonGrid.Children.Add(label);
			
			
			
			// pnl grid
					
			PNLGrid = new System.Windows.Controls.Grid();
			//PNLGrid.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(4) }); // width of market / limit
			//PNLGrid.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(CTWidth - CTLeftSpace) }); // spacer	
						
			
			int pnlheight = 26;
			
			PNLGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(0) }); // spacer
			PNLGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(pnlheight) }); // height of market / limit / offset	
			PNLGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(3) }); // spacer
			PNLGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(pnlheight) }); // height of market / limit / offset		
			
			
				
			PNLBox = new System.Windows.Controls.TextBlock()
			{
				FontFamily			= ChartControl.Properties.LabelFont.Family,
				FontSize			= 14,
				Foreground			= ChartControl.Properties.ChartText,
				HorizontalAlignment	= HorizontalAlignment.Left,
				Margin				= new Thickness(0, 0, 0, 0),
				//Padding				= new Thickness(0, 0, 0, 0),
				Padding				= new Thickness(5, 5, 5, 5),	
				Width = 110,
				Height = pnlheight,	
				Text				= string.Format("{0} {1} {2}", Instrument.FullName, BarsPeriod.Value, BarsPeriod.BarsPeriodType)
			};

						
			QTYBox = new System.Windows.Controls.TextBlock()
			{
				FontFamily			= ChartControl.Properties.LabelFont.Family,
				FontSize			= 14,
				Foreground			= ChartControl.Properties.ChartText,
				HorizontalAlignment	= HorizontalAlignment.Left,
				Margin				= new Thickness(0, 0, 0, 0),
				//Padding				= new Thickness(0, 0, 0, 0),
				Padding				= new Thickness(5, 5, 5, 5),
				Width = 110,
				Height = pnlheight,
				Text				= string.Format("{0} {1} {2}", Instrument.FullName, BarsPeriod.Value, BarsPeriod.BarsPeriodType)
			};
			
			PNLBox.Width = CTWidth - CTLeftSpace - CTLeftSpace - endadd + 2;
			QTYBox.Width = CTWidth - CTLeftSpace - CTLeftSpace - endadd + 2;
			
			QTYBox.Background = Brushes.Black;
			PNLBox.Background = Brushes.Black;
			
			PNLBox.TextAlignment = System.Windows.TextAlignment.Center;
			QTYBox.TextAlignment = System.Windows.TextAlignment.Center;
			
			
			PNLBox.MouseDown += PNLMouseDown;
			
			//PNLBox.parap.ParagraphAlignment = System.Windows.ParagraphAlignment.Center;
			//QTYBox.ParagraphAlignment = System.WindowsParagraphAlignment.Center;			
			
		
			PNLBox.Text = "PNL";
			QTYBox.Text = "QTY";
			PNLBox.Text = "";
			QTYBox.Text = "";			

			PNLGrid.Children.Add(QTYBox);
			System.Windows.Controls.Grid.SetColumn(QTYBox, 0);
			System.Windows.Controls.Grid.SetRow(QTYBox, 1);
						
			PNLGrid.Children.Add(PNLBox);
			System.Windows.Controls.Grid.SetColumn(PNLBox, 0);
			System.Windows.Controls.Grid.SetRow(PNLBox, 3);
						
			
			
			//UpdatePNLBox(ThisPositionNow());
			//UpdateQTYBox(ThisPositionNow());
		
			
			
			
			buttonTwoGrid1 = new System.Windows.Controls.Grid();
			buttonTwoGrid2 = new System.Windows.Controls.Grid();
			buttonTwoGrid3 = new System.Windows.Controls.Grid();
			buttonTwoGrid4 = new System.Windows.Controls.Grid();
			buttonTwoGrid5 = new System.Windows.Controls.Grid();
			
			MarketButtons = new System.Windows.Controls.Grid();
			CloseButtons = new System.Windows.Controls.Grid();
			WashoutButtons = new System.Windows.Controls.Grid();
			StackButtons = new System.Windows.Controls.Grid();
			ClickButtons = new System.Windows.Controls.Grid();
			OtherButtons = new System.Windows.Controls.Grid();
			BracketButtons = new System.Windows.Controls.Grid();
			Limit1Buttons = new System.Windows.Controls.Grid();
			Limit2Buttons = new System.Windows.Controls.Grid();			
				
			
			TypeGrid1 = new System.Windows.Controls.Grid();
			TypeGrid2 = new System.Windows.Controls.Grid();			
			TypeGrid3 = new System.Windows.Controls.Grid();	
			TypeGrid4 = new System.Windows.Controls.Grid();	
			TypeGrid5 = new System.Windows.Controls.Grid();	
			
//		private System.Windows.Controls.Button		button1, button2, button3, button4, button5, button6, button7, button8;
//		private System.Windows.Controls.Grid		buttonGrid, chartGrid;
//		private System.Windows.Controls.Grid		buttonTwoGrid1, buttonTwoGrid2, buttonTwoGrid3, buttonTwoGrid4, buttonTwoGrid5;
//		private System.Windows.Controls.Grid		TypeGrid1, TypeGrid2;
//		private System.Windows.Controls.Grid		MarketButtons, CloseButtons, StackButtons, ClickButtons;			
			

			
			
			

			int buttonh = MainButtonHeight - 2;
			int buttonw = 112;
				
			int buttonspacetwo = 5;
			int totalwidth = CTWidth - CTLeftSpace - CTLeftSpace - buttonspacetwo;
			int buttonleftwidth = (int) Math.Round((double)totalwidth / 2, 0);
			int buttonrightwidth = totalwidth - buttonleftwidth;
			
						
			
			BracketButtons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonSpace) });
			BracketButtons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonHeight) });
			BracketButtons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonSpace2) });
			BracketButtons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonHeight2-1) });
			
			
//			StackButtons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(90) });
//			StackButtons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(5) });
//			StackButtons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(20) });

			
			
			StackButtons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonSpace) });
			StackButtons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonHeight) });
			//StackButtons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonSpace) });
			//StackButtons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonHeight) });
			StackButtons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonSpace2) });
			StackButtons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonHeight2) });
			
			
			
			CloseButtons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonSpace) }); 
			CloseButtons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonHeight) });
			//CloseButtons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonSpace) });
			//CloseButtons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonHeight) });
			CloseButtons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonSpace2) });
			CloseButtons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonHeight2) });
			
			
			WashoutButtons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonSpace) }); 
			WashoutButtons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonHeight) });
			WashoutButtons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonSpace2) });
			WashoutButtons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonHeight2) });			
			
			
			MarketButtons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonSpace) });
			MarketButtons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonHeight) });
			//MarketButtons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonSpace) });
			//MarketButtons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonHeight) });
			MarketButtons.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(buttonleftwidth) }); // width of market / limit
			MarketButtons.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(MainButtonSpace + 2) }); // spacer
			MarketButtons.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(buttonrightwidth) });
			
			
			
			Limit1Buttons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonSpace) });
			Limit1Buttons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonHeight) });
			//MarketButtons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonSpace) });
			//MarketButtons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonHeight) });
			Limit1Buttons.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(buttonleftwidth) }); // width of market / limit
			Limit1Buttons.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(MainButtonSpace + 2) }); // spacer
			Limit1Buttons.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(buttonrightwidth) });			
			
			
			
			
			Limit2Buttons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonSpace) });
			Limit2Buttons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonHeight) });
			//MarketButtons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonSpace) });
			//MarketButtons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonHeight) });
			Limit2Buttons.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(buttonleftwidth) }); // width of market / limit
			Limit2Buttons.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(MainButtonSpace + 2) }); // spacer
			Limit2Buttons.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(buttonrightwidth) });					
			
			
			
			
//			ClickButtons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonSpace) });
//			ClickButtons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonHeight) });
			ClickButtons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonSpace) });
			ClickButtons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonHeight) });			
			ClickButtons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonSpace-2) });
			ClickButtons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonHeight2-4) });
						
			
			
			
			OtherButtons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonSpace) });
			OtherButtons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonHeight) });
			
			if (pShowReverse)
			{
			
				OtherButtons.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(buttonleftwidth) }); // width of market / limit
				OtherButtons.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(MainButtonSpace + 2) }); // spacer
				OtherButtons.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(buttonrightwidth) });		
			}
			else
			{
				OtherButtons.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(CTWidth - CTLeftSpace - CTLeftSpace) });
				
			}
			//OtherButtons.Width = CTWidth - CTLeftSpace - CTLeftSpace;
			
			
			
//			buttonTwoGrid2.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonSpace) });
			buttonTwoGrid2.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonHeight) });			
			buttonTwoGrid2.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(buttonleftwidth) }); // width of market / limit
			buttonTwoGrid2.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(buttonspacetwo-1) }); // spacer
			buttonTwoGrid2.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(buttonrightwidth) });		
			
			
			// close and stack buttons
			
			buttonTwoGrid3.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonHeight) });			
			buttonTwoGrid3.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(buttonleftwidth) }); // width of market / limit
			buttonTwoGrid3.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(MainButtonSpace) }); // spacer
			buttonTwoGrid3.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(buttonrightwidth+1) });					
			
			buttonTwoGrid4.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonHeight) });			
			buttonTwoGrid4.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(buttonleftwidth) }); // width of market / limit
			buttonTwoGrid4.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(MainButtonSpace) }); // spacer
			buttonTwoGrid4.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(buttonrightwidth+1) });			
			
			buttonTwoGrid5.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonHeight) });			
			buttonTwoGrid5.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(buttonleftwidth) }); // width of market / limit
			buttonTwoGrid5.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(MainButtonSpace) }); // spacer
			buttonTwoGrid5.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(buttonrightwidth+1) });				
			
			
			
			
			
			
			OtherButtons.HorizontalAlignment = HorizontalAlignment.Left;
			MarketButtons.HorizontalAlignment = HorizontalAlignment.Left;
			Limit1Buttons.HorizontalAlignment = HorizontalAlignment.Left;
			Limit2Buttons.HorizontalAlignment = HorizontalAlignment.Left;
			
			buttonTwoGrid2.HorizontalAlignment = HorizontalAlignment.Left;
			buttonTwoGrid3.HorizontalAlignment = HorizontalAlignment.Left;
			buttonTwoGrid4.HorizontalAlignment = HorizontalAlignment.Left;
			buttonTwoGrid5.HorizontalAlignment = HorizontalAlignment.Left;
			
			
//			OtherButtons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonSpace) });
//			OtherButtons.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonHeight) });				
			
			
			int widdd1 = 70;
			int spacebt = 4;
			int widdd2 = 50;
			
			int widdda1 = 110;
			//int spaceabt = 4;
			int widdda2 = CTWidth - CTLeftSpace - CTLeftSpace - 4 - widdda1 - 5;
			
			//TypeGrid1.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(2) }); // spacer
			TypeGrid1.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonHeight2) }); // height of market / limit / offset		
			TypeGrid1.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(0) }); 
			TypeGrid1.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(widdda1) }); // width of market / limit
			TypeGrid1.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(spacebt) }); // spacer
			TypeGrid1.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(widdda2) }); // width of offset			
			
			//TypeGrid2.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(2) }); // spacer
			
			
			
			TypeGrid2.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonHeight2) }); // height of market / limit / offset		
			TypeGrid2.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(0) }); 
			TypeGrid2.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(widdda1) }); // width of market / limit
			TypeGrid2.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(spacebt) }); // spacer
			TypeGrid2.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(widdda2) }); // width of offset				
			
			
			TypeGrid5.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonHeight2) }); // height of market / limit / offset		
			TypeGrid5.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(0) }); 
			TypeGrid5.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(widdda1-18) }); // width of market / limit
			TypeGrid5.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(spacebt) }); // spacer
			TypeGrid5.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(widdda2-18) }); // width of offset		
			TypeGrid5.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(spacebt) }); // spacer
			TypeGrid5.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(30) }); // width of offset					
			
			
			
			
			
			// bracket
			
			TypeGrid3.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonHeight2) }); // height of market / limit / offset		
			//TypeGrid3.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(0) }); 
			TypeGrid3.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(widdd2+5) }); // width of offset					
			TypeGrid3.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(spacebt) }); // spacer
			TypeGrid3.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(widdd1) }); // width of market / limit
			TypeGrid3.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(spacebt) }); // spacer
			TypeGrid3.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(widdd2+5) }); // width of offset					
			TypeGrid3.HorizontalAlignment = HorizontalAlignment.Left;
			
			// click
			
			int sizew = 53;
			
			
			TypeGrid4.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonHeight2) }); // height of market / limit / offset		
			TypeGrid4.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(0) }); 
			TypeGrid4.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(sizew) }); // width of market / limit
			TypeGrid4.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(spacebt-1) }); // spacer
			TypeGrid4.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(sizew) }); // width of offset				
			TypeGrid4.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(spacebt-1) }); // spacer
			TypeGrid4.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(71) }); // width of offset				
			
//			buttonTwoGrid2.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(5) }); // spacer
//			buttonTwoGrid2.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(30) }); // height of market / limit / offset
			
			
//			buttonTwoGrid2.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(5) }); 
//			buttonTwoGrid2.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(64) }); // width of market / limit
//			buttonTwoGrid2.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(2) }); // spacer
//			buttonTwoGrid2.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(44) }); // width of offset
			
			
			
//			buttonTwoGrid1.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(5) }); // spacer
//			buttonTwoGrid1.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(buttonw) }); // width of offset			
			
			
//			buttonTwoGrid1.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonSpace) });
//			buttonTwoGrid1.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonHeight) });
//			buttonTwoGrid1.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonSpace) });
//			buttonTwoGrid1.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonHeight) });
			
//			buttonTwoGrid1.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonSpace2) });
//			buttonTwoGrid1.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonHeight) });
//			buttonTwoGrid1.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonSpace) });
//			buttonTwoGrid1.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonHeight) });	
			
//			buttonTwoGrid1.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonSpace2) });
//			buttonTwoGrid1.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonHeight) });
//			buttonTwoGrid1.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonSpace) });
//			buttonTwoGrid1.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonHeight) });	
			
//			buttonTwoGrid1.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonSpace2) });
//			buttonTwoGrid1.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonHeight) });
//			buttonTwoGrid1.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonSpace) });
//			buttonTwoGrid1.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonHeight) });
			
			
			
			// stack close and direction button combos
			
			int col0 = 0;
			
			int col2 = 0;
			int col3 = 20;
			int col1 = buttonleftwidth - col3 - col0 - col2 - 3;
			
			//if (!pShowDirectionButtons)
			
			
			twoButtonGrid1 = new System.Windows.Controls.Grid();
			twoButtonGrid2 = new System.Windows.Controls.Grid();			
			twoButtonGrid3 = new System.Windows.Controls.Grid();	
			twoButtonGrid4 = new System.Windows.Controls.Grid();	
			twoButtonGrid5 = new System.Windows.Controls.Grid();	
			twoButtonGrid6 = new System.Windows.Controls.Grid();	
			
			
			twoButtonGrid1.Width = CTWidth;
			twoButtonGrid1.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonHeight) });	
			twoButtonGrid1.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(col0) });
			twoButtonGrid1.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(col1) }); // width of market / limit
			twoButtonGrid1.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(col2) }); // spacer
			twoButtonGrid1.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(col3) }); // width of offset					
			

			
			twoButtonGrid3.Width = CTWidth;
			twoButtonGrid3.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonHeight) });	
			twoButtonGrid3.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(col0) });
			twoButtonGrid3.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(col1) }); // width of market / limit
			twoButtonGrid3.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(col2) }); // spacer
			twoButtonGrid3.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(col3) }); // width of offset		
			
			twoButtonGrid5.Width = CTWidth;
			twoButtonGrid5.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonHeight) });	
			twoButtonGrid5.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(col0) });
			twoButtonGrid5.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(col1) }); // width of market / limit
			twoButtonGrid5.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(col2) }); // spacer
			twoButtonGrid5.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(col3) }); // width of offset		
			
			
			
			
			
			col1 = buttonrightwidth - col3 - col0 - col2 - 3;
			
			twoButtonGrid2.Width = CTWidth;
			twoButtonGrid2.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonHeight) });	
			twoButtonGrid2.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(col0) });
			twoButtonGrid2.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(col1) }); // width of market / limit
			twoButtonGrid2.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(col2) }); // spacer
			twoButtonGrid2.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(col3) }); // width of offset	
			
			
			twoButtonGrid4.Width = CTWidth;
			twoButtonGrid4.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonHeight) });	
			twoButtonGrid4.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(col0) });
			twoButtonGrid4.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(col1) }); // width of market / limit
			twoButtonGrid4.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(col2) }); // spacer
			twoButtonGrid4.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(col3) }); // width of offset	
			
			twoButtonGrid6.Width = CTWidth;
			twoButtonGrid6.RowDefinitions.Add(new System.Windows.Controls.RowDefinition() { Height = new GridLength(MainButtonHeight) });	
			twoButtonGrid6.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(col0) });
			twoButtonGrid6.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(col1) }); // width of market / limit
			twoButtonGrid6.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(col2) }); // spacer
			twoButtonGrid6.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(col3) }); // width of offset	
			
			
			
			
			twoButtonGrid1.HorizontalAlignment = HorizontalAlignment.Left;
			twoButtonGrid2.HorizontalAlignment = HorizontalAlignment.Left;
			twoButtonGrid3.HorizontalAlignment = HorizontalAlignment.Left;
			twoButtonGrid4.HorizontalAlignment = HorizontalAlignment.Left;
			twoButtonGrid5.HorizontalAlignment = HorizontalAlignment.Left;
			twoButtonGrid6.HorizontalAlignment = HorizontalAlignment.Left;			
			
			
			
			int toggleh = MainButtonHeight - 2;
			int togglew = col3;
			
			toggle1 = new System.Windows.Controls.Button()
			{
				Background = pColorBuyEMA2,
				BorderBrush = ButtonBrush2,
				Content = "",
				Height = toggleh,
				Width = togglew,
                MaxWidth = 999,
                MinWidth = 0,
				HorizontalAlignment	= HorizontalAlignment.Center
			};
			toggle1.Click += toggle1_Click;
			
			
			toggle2 = new System.Windows.Controls.Button()
			{
				Background = pColorSellEMA2,
				BorderBrush = ButtonBrush2,
				Content = "",
				Height = toggleh,
				Width = togglew,
                MaxWidth = 999,
                MinWidth = 0,
				HorizontalAlignment	= HorizontalAlignment.Center
			};
			toggle2.Click += toggle2_Click;			
			
			toggle3 = new System.Windows.Controls.Button()
			{
				Background = pColorBuyEMA2,
				BorderBrush = ButtonBrush2,
				Content = "",
				Height = toggleh,
				Width = togglew,
                MaxWidth = 999,
                MinWidth = 0,
				HorizontalAlignment	= HorizontalAlignment.Center
			};
			toggle3.Click += toggle3_Click;
			
			toggle4 = new System.Windows.Controls.Button()
			{
				Background = pColorSellEMA2,
				BorderBrush = ButtonBrush2,
				Content = "",
				Height = toggleh,
				Width = togglew,
                MaxWidth = 999,
                MinWidth = 0,
				HorizontalAlignment	= HorizontalAlignment.Center
			};
			toggle4.Click += toggle4_Click;
			
			
			
			 
			button1 = new System.Windows.Controls.Button()
			{
				Background = pColorBuyEMA2,
				BorderBrush = ButtonBrush2,
				Content = "Buy Market",
				Height = buttonh,
				Width = buttonw,
                MaxWidth = 999,
                MinWidth = 0,
				HorizontalAlignment	= HorizontalAlignment.Center
			};
			button1.Click += Button1_Click;
			
			button2 = new System.Windows.Controls.Button()
			{
				Background = pColorSellEMA2,
				BorderBrush = ButtonBrush2,
				Content = "Sell Market",
				Height = buttonh,
				Width = buttonw,
                MaxWidth = 999,
                MinWidth = 0,
				HorizontalAlignment	= HorizontalAlignment.Center
			};
			button2.Click += Button2_Click;			
			
			
			button14 = new System.Windows.Controls.Button()
			{
				Background = pColorBuyEMA2,
				BorderBrush = ButtonBrush2,
				Content = "Limit 0",
				Height = buttonh,
				Width = buttonw,
                MaxWidth = 999,
                MinWidth = 0,
				HorizontalAlignment	= HorizontalAlignment.Center
			};
			button14.Click += Button14_Click;
			
			button15 = new System.Windows.Controls.Button()
			{
				Background = pColorSellEMA2,
				BorderBrush = ButtonBrush2,
				Content = "Limit 0",
				Height = buttonh,
				Width = buttonw,
                MaxWidth = 999,
                MinWidth = 0,
				HorizontalAlignment	= HorizontalAlignment.Center
			};
			button15.Click += Button15_Click;					
			
			
			button16 = new System.Windows.Controls.Button()
			{
				Background = pColorBuyEMA2,
				BorderBrush = ButtonBrush2,
				Content = "Limit 1",
				Height = buttonh,
				Width = buttonw,
                MaxWidth = 999,
                MinWidth = 0,
				HorizontalAlignment	= HorizontalAlignment.Center
			};
			button16.Click += Button16_Click;
			
			button17 = new System.Windows.Controls.Button()
			{
				Background = pColorSellEMA2,
				BorderBrush = ButtonBrush2,
				Content = "Limit 1",
				Height = buttonh,
				Width = buttonw,
                MaxWidth = 999,
                MinWidth = 0,
				HorizontalAlignment	= HorizontalAlignment.Center
			};
			button17.Click += Button17_Click;			
			
			
			
			button3 = new System.Windows.Controls.Button()
			{
				Background = pColorBuyEMA,
				BorderBrush = ButtonBrush2,
				Content = "Stack",
				Height = buttonh,
				Width = buttonw,
                MaxWidth = 999,
                MinWidth = 0,
				HorizontalAlignment	= HorizontalAlignment.Center
			};
			button3.Click += Button3_Click;
			

			
			button4 = new System.Windows.Controls.Button()
			{
				Background = pColorSellEMA,
				BorderBrush = ButtonBrush2,
				Content = "Stack",
				Height = buttonh,
				Width = buttonw,
                MaxWidth = 999,
                MinWidth = 0,
				HorizontalAlignment	= HorizontalAlignment.Center
			};
			button4.Click += Button4_Click;	

			button3.Width = col1;
			button4.Width = col1;
			
			button5 = new System.Windows.Controls.Button()
			{
				Background = pColorButtonCloseAll,
				BorderBrush = ButtonBrush2,
				Content = "Close All",
				Height = buttonh,
				Width = buttonw,
                MaxWidth = 999,
                MinWidth = 0,
				HorizontalAlignment	= HorizontalAlignment.Center
			};
			button5.Click += Button5_Click;
			
			
			button13 = new System.Windows.Controls.Button()
			{
				Background = pColorButtonReverse,
				BorderBrush = ButtonBrush2,
				Content = "Reverse",
				Height = buttonh,
				Width = buttonw,
                MaxWidth = 999,
                MinWidth = 0,
				HorizontalAlignment	= HorizontalAlignment.Center
			};
			button13.Click += Button13_Click;			
			
			
			button6 = new System.Windows.Controls.Button()
			{
				Background = pColorButtonCloseAll,
				BorderBrush = ButtonBrush2,
				Content = "Bracket",
				Height = buttonh,
				Width = buttonw,
                MaxWidth = 999,
                MinWidth = 0,
				HorizontalAlignment	= HorizontalAlignment.Center
			};
			button6.Click += Button6_Click;					


			button7 = new System.Windows.Controls.Button()
			{
				Background = pColorBuyEMA,
				BorderBrush = ButtonBrush2,
				Content = "Buy Click",
				Height = buttonh,
				Width = buttonw,
                MaxWidth = 999,
                MinWidth = 0,
				HorizontalAlignment	= HorizontalAlignment.Center
			};
			button7.Click += Button7_Click;
			
			button8 = new System.Windows.Controls.Button()
			{
				Background = pColorSellEMA,
				BorderBrush = ButtonBrush2,
				Content = "Sell Click",
				Height = buttonh,
				Width = buttonw,
                MaxWidth = 999,
                MinWidth = 0,
				HorizontalAlignment	= HorizontalAlignment.Center
			};
			button8.Click += Button8_Click;				
			
			button9 = new System.Windows.Controls.Button()
			{
				Background = pColorBuyEMA,
				BorderBrush = ButtonBrush2,
				Content = "Close",
				Height = buttonh,
				Width = buttonw,
                MaxWidth = 999,
                MinWidth = 0,
				HorizontalAlignment	= HorizontalAlignment.Center
			};
			button9.Click += Button9_Click;
			
			button10 = new System.Windows.Controls.Button()
			{
				Background = pColorSellEMA,
				BorderBrush = ButtonBrush2,
				Content = "Close",
				Height = buttonh,
				Width = buttonw,
                MaxWidth = 999,
                MinWidth = 0,
				HorizontalAlignment	= HorizontalAlignment.Center
			};
			button10.Click += Button10_Click;						
			

			button9.Width = col1;
			button10.Width = col1;
			
			
			
			
			button18 = new System.Windows.Controls.Button()
			{
				Background = pColorBuyEMA2,
				BorderBrush = ButtonBrush2,
				Content = "Wash",
				Height = buttonh,
				Width = buttonw,
                MaxWidth = 999,
                MinWidth = 0,
				HorizontalAlignment	= HorizontalAlignment.Center
			};
			button18.Click += Button18_Click;
			
			button19 = new System.Windows.Controls.Button()
			{
				Background = pColorSellEMA2,
				BorderBrush = ButtonBrush2,
				Content = "Wash",
				Height = buttonh,
				Width = buttonw,
                MaxWidth = 999,
                MinWidth = 0,
				HorizontalAlignment	= HorizontalAlignment.Center
			};
			button19.Click += Button19_Click;						
			

			button18.Width = col1;
			button19.Width = col1;
			
			
			toggle8 = new System.Windows.Controls.Button()
			{
				Background = pColorBuyEMA2,
				BorderBrush = ButtonBrush2,
				Content = "",
				Height = toggleh,
				Width = togglew,
                MaxWidth = 999,
                MinWidth = 0,
				HorizontalAlignment	= HorizontalAlignment.Center
			};
			toggle8.Click += toggle8_Click;
			
			toggle9 = new System.Windows.Controls.Button()
			{
				Background = pColorSellEMA2,
				BorderBrush = ButtonBrush2,
				Content = "",
				Height = toggleh,
				Width = togglew,
                MaxWidth = 999,
                MinWidth = 0,
				HorizontalAlignment	= HorizontalAlignment.Center
			};
			toggle9.Click += toggle9_Click;			
			
			
			
			
			
			
			button11 = new System.Windows.Controls.Button()
			{
				Background = pColorSellEMA,
				BorderBrush = ButtonBrush2,
				Content = "STP",
				//Height = buttonh,
				//Width = buttonw,
                MaxWidth = 999,
                MinWidth = 0,
				Height = 21,
              	Width = sizew,
				HorizontalAlignment	= HorizontalAlignment.Center
			};
			button11.Click += Button11_Click;					
			
			button12 = new System.Windows.Controls.Button()
			{
				Background = pColorSellEMA,
				BorderBrush = ButtonBrush2,
				Content = "LMT",
				//Height = buttonh,
				//Width = buttonw,
                MaxWidth = 999,
                MinWidth = 0,
				Height = 21,
              	Width = sizew,
				HorizontalAlignment	= HorizontalAlignment.Center
			};
			button12.Click += Button12_Click;	
			

			button11.FontSize = 12;
			button12.FontSize = 12;
			button11.Padding = new Thickness(0, 0, 0, 0);
			button12.Padding = new Thickness(0, 0, 0, 0);
			
			
			
			button20 = new System.Windows.Controls.Button()
			{
				Background = pColorSellEMA,
				BorderBrush = ButtonBrush2,
				Content = "CA",
				//Height = buttonh,
				//Width = buttonw,
                MaxWidth = 999,
                MinWidth = 0,
				Height = 21,
              	Width = sizew,
				HorizontalAlignment	= HorizontalAlignment.Center
			};
			button20.Click += Button20_Click;	
			button20.Width = 30;
			button20.FontSize = 12;
			button20.Padding = new Thickness(0, 0, 0, 0);			
			
			
		
//			button2 = new System.Windows.Controls.Button()
//			{
//				Content				= "Button 2",
//				HorizontalAlignment	= HorizontalAlignment.Center
//			};
//			button2.Click += Button2_Click;
			
//			button1 = new System.Windows.Controls.Button()
//			{
//				Content				= "Button 1",
//				HorizontalAlignment	= HorizontalAlignment.Center
//			};
//			button1.Click += Button1_Click;



			
////			System.Windows.Controls.Grid.SetColumn(button1, 1);
////			System.Windows.Controls.Grid.SetColumn(button2, 1);
//			System.Windows.Controls.Grid.SetColumn(button3, 1);
//			System.Windows.Controls.Grid.SetColumn(button4, 1);
//			System.Windows.Controls.Grid.SetColumn(button5, 1);
//			System.Windows.Controls.Grid.SetColumn(button6, 1);			
			
////			System.Windows.Controls.Grid.SetRow(button1, 1);
////			System.Windows.Controls.Grid.SetRow(button2, 3);
//			System.Windows.Controls.Grid.SetRow(button3, 5);
//			System.Windows.Controls.Grid.SetRow(button4, 7);
//			System.Windows.Controls.Grid.SetRow(button5, 9);
//			System.Windows.Controls.Grid.SetRow(button6, 11);			
////			System.Windows.Controls.Grid.SetRow(button7, 13);
////			System.Windows.Controls.Grid.SetRow(button8, 15);			
			
////			buttonTwoGrid1.Children.Add(button1);
////			buttonTwoGrid1.Children.Add(button2);
//			//buttonTwoGrid1.Children.Add(button3);
//			//buttonTwoGrid1.Children.Add(button4);			
//			buttonTwoGrid1.Children.Add(button5);
//			buttonTwoGrid1.Children.Add(button6);
////			buttonTwoGrid1.Children.Add(button7);
////			buttonTwoGrid1.Children.Add(button8);				

			
			cb4 = new System.Windows.Controls.ComboBox()
			{
				//Background = ButtonBrush,
				//BorderBrush = ButtonBrush2,
				//Content = "Buy Click",
				//Height = buttonh,
				//Width = aaaa,
                MaxWidth = 200,
                MinWidth = 0
			};
			
			cb4.Items.Add("Market");
			cb4.Items.Add("Limit");
			cb4.SelectionChanged += CB4Click;
			System.Windows.Automation.AutomationProperties.SetAutomationId(cb4, "cb4");
			
			
			TypeGrid5.Children.Add(cb4);
			System.Windows.Controls.Grid.SetColumn(cb4, 1);
			System.Windows.Controls.Grid.SetRow(cb4, 0);	
			
			
			OffsetBox6 = new NinjaTrader.Gui.Tools.QuantityUpDown();
			OffsetBox6.Value = pLimitOrderOffset6;
			OffsetBox6.Height = 21;
			//OffsetBox1.Width = 45;
			OffsetBox6.IsPopupEnabled = false;
			OffsetBox6.Minimum = -10000;
			//OffsetBox2.
			OffsetBox6.ValueChanged += OffsetBox6Click;
			TypeGrid5.Children.Add(OffsetBox6);
			System.Windows.Controls.Grid.SetColumn(OffsetBox6, 3);
			System.Windows.Controls.Grid.SetRow(OffsetBox6, 0);	
			
			TypeGrid5.Children.Add(button20);
			System.Windows.Controls.Grid.SetColumn(button20, 5);
			System.Windows.Controls.Grid.SetRow(button20, 0);	
			
			
			
			// stack entry orders
			
			
			
			cb2 = new System.Windows.Controls.ComboBox()
			{
				//Background = ButtonBrush,
				//BorderBrush = ButtonBrush2,
				//Content = "Buy Click",
				//Height = buttonh,
				//Width = aaaa,
                MaxWidth = 200,
                MinWidth = 0
			};
			
			cb2.Items.Add("Market");
			cb2.Items.Add("Limit");
			cb2.SelectionChanged += CB2Click;
			System.Windows.Automation.AutomationProperties.SetAutomationId(cb2, "cb2");
			
			
			TypeGrid2.Children.Add(cb2);
			System.Windows.Controls.Grid.SetColumn(cb2, 1);
			System.Windows.Controls.Grid.SetRow(cb2, 0);	
			
			
			OffsetBox2 = new NinjaTrader.Gui.Tools.QuantityUpDown();
			OffsetBox2.Value = pLimitOrderOffset2;
			OffsetBox2.Height = 21;
			//OffsetBox1.Width = 45;
			OffsetBox2.IsPopupEnabled = false;
			OffsetBox2.Minimum = -10000;
			//OffsetBox2.
			OffsetBox2.ValueChanged += OffsetBox2Click;
			TypeGrid2.Children.Add(OffsetBox2);
			System.Windows.Controls.Grid.SetColumn(OffsetBox2, 3);
			System.Windows.Controls.Grid.SetRow(OffsetBox2, 0);	
			
			
			
			
		// bracket
			
			
			cb3 = new System.Windows.Controls.ComboBox()
			{
				//Background = ButtonBrush,
				//BorderBrush = ButtonBrush2,
				//Content = "Buy Click",
				//Height = buttonh,
				//Width = aaaa,
                MaxWidth = 200,
                MinWidth = 0
			};
			
			cb3.Items.Add("STP");
			cb3.Items.Add("SLM");
			cb3.SelectionChanged += CB3Click;
			System.Windows.Automation.AutomationProperties.SetAutomationId(cb3, "cb3");			
			
			
			TypeGrid3.Children.Add(cb3);
			System.Windows.Controls.Grid.SetColumn(cb3, 2);
			System.Windows.Controls.Grid.SetRow(cb3, 0);	
			
			
			OffsetBox3 = new NinjaTrader.Gui.Tools.QuantityUpDown();
			OffsetBox3.Value = pLimitOrderOffset3;
			OffsetBox3.Height = 21;
			//OffsetBox1.Width = 45;
			OffsetBox3.IsPopupEnabled = false;
			OffsetBox3.ValueChanged += OffsetBox3Click;
			
			TypeGrid3.Children.Add(OffsetBox3);
			System.Windows.Controls.Grid.SetColumn(OffsetBox3, 0);
			System.Windows.Controls.Grid.SetRow(OffsetBox3, 0);	
			
			
			OffsetBox5 = new NinjaTrader.Gui.Tools.QuantityUpDown();
			OffsetBox5.Value = pSLOffset2;
			OffsetBox5.Height = 21;
			//OffsetBox1.Width = 45;
			OffsetBox5.IsPopupEnabled = false;
			OffsetBox5.ValueChanged += OffsetBox5Click;				
			
			TypeGrid3.Children.Add(OffsetBox5);
			System.Windows.Controls.Grid.SetColumn(OffsetBox5, 4);
			System.Windows.Controls.Grid.SetRow(OffsetBox5, 0);				
			
			
			
			// Close BUTTONS
		
//			CloseButtons.Children.Add(button9);
//			CloseButtons.Children.Add(button10);					
//			CloseButtons.Children.Add(TypeGrid1);
			
			
		
//			System.Windows.Controls.Grid.SetColumn(button9, 0);
//			System.Windows.Controls.Grid.SetColumn(button10, 0);
//			System.Windows.Controls.Grid.SetColumn(TypeGrid1, 0);
			
//			System.Windows.Controls.Grid.SetRow(button9, 1);
//			System.Windows.Controls.Grid.SetRow(button10, 3);
//			System.Windows.Controls.Grid.SetRow(TypeGrid1, 5);
			
			
			//StackButtons.Children.Add(button3);
			//StackButtons.Children.Add(button4);	
			
			//CloseButtons.Children.Add(twoButtonGrid3);
			//CloseButtons.Children.Add(twoButtonGrid4);
			
			buttonTwoGrid3.Children.Add(twoButtonGrid3);
			buttonTwoGrid3.Children.Add(twoButtonGrid4);
			
			CloseButtons.Children.Add(buttonTwoGrid3);
			CloseButtons.Children.Add(TypeGrid1);
			
			twoButtonGrid3.Children.Add(button9);
			twoButtonGrid3.Children.Add(toggle3);
			twoButtonGrid4.Children.Add(button10);
			twoButtonGrid4.Children.Add(toggle4);			
			
			System.Windows.Controls.Grid.SetColumn(button9, 1);
			System.Windows.Controls.Grid.SetColumn(toggle3, 3);
			System.Windows.Controls.Grid.SetColumn(button10, 1);
			System.Windows.Controls.Grid.SetColumn(toggle4, 3);	
			

			System.Windows.Controls.Grid.SetRow(buttonTwoGrid3, 1);
			
		//	System.Windows.Controls.Grid.SetColumn(button3, 0);
		//	System.Windows.Controls.Grid.SetColumn(button4, 0);
			System.Windows.Controls.Grid.SetColumn(twoButtonGrid3, 0);
			System.Windows.Controls.Grid.SetColumn(twoButtonGrid4, 2);
			System.Windows.Controls.Grid.SetColumn(TypeGrid1, 0);
			
			//System.Windows.Controls.Grid.SetRow(button3, 1);
			//System.Windows.Controls.Grid.SetRow(button4, 3);			
			System.Windows.Controls.Grid.SetRow(twoButtonGrid3, 0);
			System.Windows.Controls.Grid.SetRow(twoButtonGrid4, 0);
			System.Windows.Controls.Grid.SetRow(TypeGrid1, 5);
			
			
			
			
			
			
			// washout buttons
			
			
			buttonTwoGrid5.Children.Add(twoButtonGrid5);
			buttonTwoGrid5.Children.Add(twoButtonGrid6);
			
			WashoutButtons.Children.Add(buttonTwoGrid5);
			WashoutButtons.Children.Add(TypeGrid5);
			
			twoButtonGrid5.Children.Add(button18);
			twoButtonGrid5.Children.Add(toggle8);
			twoButtonGrid6.Children.Add(button19);
			twoButtonGrid6.Children.Add(toggle9);			
			
			System.Windows.Controls.Grid.SetColumn(button18, 1);
			System.Windows.Controls.Grid.SetColumn(toggle8, 3);
			System.Windows.Controls.Grid.SetColumn(button19, 1);
			System.Windows.Controls.Grid.SetColumn(toggle9, 3);	
			

			System.Windows.Controls.Grid.SetRow(buttonTwoGrid5, 1);
			

			System.Windows.Controls.Grid.SetColumn(twoButtonGrid5, 0);
			System.Windows.Controls.Grid.SetColumn(twoButtonGrid6, 2);
			System.Windows.Controls.Grid.SetColumn(TypeGrid5, 0);
			
			
			System.Windows.Controls.Grid.SetRow(twoButtonGrid5, 0);
			System.Windows.Controls.Grid.SetRow(twoButtonGrid6, 0);
			System.Windows.Controls.Grid.SetRow(TypeGrid5, 5);
			
			
			
			
			
			
			
			
			
			// STACK BUTTONS
		
			//StackButtons.Children.Add(button3);
			//StackButtons.Children.Add(button4);	
			buttonTwoGrid4.Children.Add(twoButtonGrid1);
			buttonTwoGrid4.Children.Add(twoButtonGrid2);
			
//			StackButtons.Children.Add(twoButtonGrid1);
//			StackButtons.Children.Add(twoButtonGrid2);	
			StackButtons.Children.Add(buttonTwoGrid4);
			StackButtons.Children.Add(TypeGrid2);
			
			twoButtonGrid1.Children.Add(button3);
			twoButtonGrid1.Children.Add(toggle1);
			twoButtonGrid2.Children.Add(button4);
			twoButtonGrid2.Children.Add(toggle2);			
			
			System.Windows.Controls.Grid.SetColumn(button3, 1);
			System.Windows.Controls.Grid.SetColumn(toggle1, 3);
			System.Windows.Controls.Grid.SetColumn(button4, 1);
			System.Windows.Controls.Grid.SetColumn(toggle2, 3);	
			
			System.Windows.Controls.Grid.SetRow(buttonTwoGrid4, 1);
			
			
		//	System.Windows.Controls.Grid.SetColumn(button3, 0);
		//	System.Windows.Controls.Grid.SetColumn(button4, 0);
			System.Windows.Controls.Grid.SetColumn(twoButtonGrid1, 0);
			System.Windows.Controls.Grid.SetColumn(twoButtonGrid2, 2);
			System.Windows.Controls.Grid.SetColumn(TypeGrid2, 0);
			
			//System.Windows.Controls.Grid.SetRow(button3, 1);
			//System.Windows.Controls.Grid.SetRow(button4, 3);			
			System.Windows.Controls.Grid.SetRow(twoButtonGrid1, 0);
			System.Windows.Controls.Grid.SetRow(twoButtonGrid2, 0);
			System.Windows.Controls.Grid.SetRow(TypeGrid2, 5);
			
			
			
			
			
			
			
			
			
			// BRACKET BUTTONS
		
			BracketButtons.Children.Add(button6);			
			BracketButtons.Children.Add(TypeGrid3);
			
			
		
			System.Windows.Controls.Grid.SetColumn(button6, 0);
			System.Windows.Controls.Grid.SetColumn(TypeGrid3, 0);
			
			System.Windows.Controls.Grid.SetRow(button6, 1);
			System.Windows.Controls.Grid.SetRow(TypeGrid3, 5);
			button6.Width = CTWidth - CTLeftSpace - CTLeftSpace;			
			button6.HorizontalAlignment = HorizontalAlignment.Left;
			
			

		

			// Limit 1 BUTTONS
		
			Limit1Buttons.Children.Add(button14);
			Limit1Buttons.Children.Add(button15);					
			
		
			System.Windows.Controls.Grid.SetColumn(button14, 0);
			System.Windows.Controls.Grid.SetColumn(button15, 2);
			
			System.Windows.Controls.Grid.SetRow(button14, 1);
			System.Windows.Controls.Grid.SetRow(button15, 1);
			
			button14.Width = buttonleftwidth;
			button15.Width = buttonrightwidth;			
			
			// Limit 2 BUTTONS
		
			Limit2Buttons.Children.Add(button16);
			Limit2Buttons.Children.Add(button17);					
			
		
			System.Windows.Controls.Grid.SetColumn(button16, 0);
			System.Windows.Controls.Grid.SetColumn(button17, 2);
			
			System.Windows.Controls.Grid.SetRow(button16, 1);
			System.Windows.Controls.Grid.SetRow(button17, 1);
			
			button16.Width = buttonleftwidth;
			button17.Width = buttonrightwidth;	
			
			
			
			
			
			// Market BUTTONS
		
			MarketButtons.Children.Add(button1);
			MarketButtons.Children.Add(button2);					
			
		
			System.Windows.Controls.Grid.SetColumn(button1, 0);
			System.Windows.Controls.Grid.SetColumn(button2, 2);
			
			System.Windows.Controls.Grid.SetRow(button1, 1);
			System.Windows.Controls.Grid.SetRow(button2, 1);
			
			button1.Width = buttonleftwidth;
			button2.Width = buttonrightwidth;
			
			
			// CLICK BUTTONS
		
			
			TypeGrid4.Children.Add(button11);
			System.Windows.Controls.Grid.SetColumn(button11, 3);
			System.Windows.Controls.Grid.SetRow(button11, 0);	
	
			
			TypeGrid4.Children.Add(button12);
			System.Windows.Controls.Grid.SetColumn(button12, 1);
			System.Windows.Controls.Grid.SetRow(button12, 0);	
			
			
			OffsetBox4 = new NinjaTrader.Gui.Tools.QuantityUpDown();
			OffsetBox4.Value = pSLOffset;
			OffsetBox4.Height = 21;
			//OffsetBox1.Width = 45;
			OffsetBox4.IsPopupEnabled = false;
			OffsetBox4.ValueChanged += OffsetBox4Click;
			
			TypeGrid4.Children.Add(OffsetBox4);
			System.Windows.Controls.Grid.SetColumn(OffsetBox4, 5);
			System.Windows.Controls.Grid.SetRow(OffsetBox4, 0);	
			OffsetBox4.Minimum = -100;
			
			
			buttonTwoGrid2.Children.Add(button7);
			buttonTwoGrid2.Children.Add(button8);
			
			ClickButtons.Children.Add(buttonTwoGrid2);
//			ClickButtons.Children.Add(button7);
//			ClickButtons.Children.Add(button8);					
			ClickButtons.Children.Add(TypeGrid4);	
		
			System.Windows.Controls.Grid.SetColumn(button7, 0);
			System.Windows.Controls.Grid.SetColumn(button8, 2);
			System.Windows.Controls.Grid.SetColumn(TypeGrid4, 0);
			
			System.Windows.Controls.Grid.SetRow(button7, 0);
			System.Windows.Controls.Grid.SetRow(button8, 0);
			System.Windows.Controls.Grid.SetRow(TypeGrid4, 3);	
			System.Windows.Controls.Grid.SetRow(buttonTwoGrid2, 1);	
			
			button7.Width = buttonleftwidth;
			button8.Width = buttonrightwidth;			
			
			
			
			// Other BUTTONS
		
			OtherButtons.Children.Add(button5);
			if (pShowReverse) OtherButtons.Children.Add(button13);					
			
			int col242 = 0;
			
			if (pShowReverse) col242 = 2;
		
			System.Windows.Controls.Grid.SetColumn(button5, col242);
			System.Windows.Controls.Grid.SetColumn(button13, 0);
			
			System.Windows.Controls.Grid.SetRow(button5, 1);
			System.Windows.Controls.Grid.SetRow(button13, 1);
			
			button13.Width = buttonleftwidth;
			button5.Width = buttonrightwidth;
			
			if (!pShowReverse)
			{
				
				button5.Width = CTWidth - CTLeftSpace - CTLeftSpace;
			}
			
			
			
			
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
			
			
			TypeGrid1.Children.Add(cb1);
			System.Windows.Controls.Grid.SetColumn(cb1, 1);
			System.Windows.Controls.Grid.SetRow(cb1, 0);			
			
			
			OffsetBox1 = new NinjaTrader.Gui.Tools.QuantityUpDown();
			OffsetBox1.Value = pLimitOrderOffset1;
			OffsetBox1.Height = 21;
			OffsetBox1.Minimum = -10000;
			OffsetBox1.IsPopupEnabled = false;
			//OffsetBox1.Width = 45;
			OffsetBox1.ValueChanged += OffsetBox1Click;
			TypeGrid1.Children.Add(OffsetBox1);
			System.Windows.Controls.Grid.SetColumn(OffsetBox1, 3);
			System.Windows.Controls.Grid.SetRow(OffsetBox1, 0);		
			
			
			
			OffsetBox1.KeyDown += (o, args) =>
			{
				args.Handled = true;
			};			
			OffsetBox2.KeyDown += (o, args) =>
			{
				args.Handled = true;
			};					
			OffsetBox3.KeyDown += (o, args) =>
			{
				args.Handled = true;
			};		
			OffsetBox4.KeyDown += (o, args) =>
			{
				args.Handled = true;
			};					
			OffsetBox5.KeyDown += (o, args) =>
			{
				args.Handled = true;
			};				
			
			// final organization
			
			
			buttonGrid.Children.Add(SetupButtons);
			System.Windows.Controls.Grid.SetRow(SetupButtons, 1);
			System.Windows.Controls.Grid.SetColumn(SetupButtons, 1);
			
			
			buttonGrid.Children.Add(PNLGrid);
			System.Windows.Controls.Grid.SetRow(PNLGrid, 2);
			System.Windows.Controls.Grid.SetColumn(PNLGrid, 1);

			buttonGrid.Children.Add(OtherButtons);
			System.Windows.Controls.Grid.SetRow(OtherButtons, 3);
			System.Windows.Controls.Grid.SetColumn(OtherButtons, 1);
			
			int ii = 3;
				
			if (pMarketEntryOrdersEnabled)
			{
				ii++;
				buttonGrid.Children.Add(MarketButtons);
				System.Windows.Controls.Grid.SetRow(MarketButtons, ii);
				System.Windows.Controls.Grid.SetColumn(MarketButtons, ii);
			}
			if (pLimit1EntryOrdersEnabled)
			{
				ii++;
				buttonGrid.Children.Add(Limit1Buttons);
				System.Windows.Controls.Grid.SetRow(Limit1Buttons, ii);
				System.Windows.Controls.Grid.SetColumn(Limit1Buttons, ii);
			}			
			
			if (pLimit2EntryOrdersEnabled)
			{
				ii++;
				buttonGrid.Children.Add(Limit2Buttons);
				System.Windows.Controls.Grid.SetRow(Limit2Buttons, ii);
				System.Windows.Controls.Grid.SetColumn(Limit2Buttons, ii);
			}
			if (OneEnabled)
			{
				ii++;
				//buttonGrid.Children.Add(ClickButtons);
				//System.Windows.Controls.Grid.SetRow(ClickButtons, ii);
				//System.Windows.Controls.Grid.SetColumn(ClickButtons, ii);
			}
			
			if (pClickEntryOrdersEnabled)
			{
				ii++;
				buttonGrid.Children.Add(ClickButtons);
				System.Windows.Controls.Grid.SetRow(ClickButtons, ii);
				System.Windows.Controls.Grid.SetColumn(ClickButtons, ii);
			}			
			if (pCloseEntryOrdersEnabled)
			{
				ii++;
				buttonGrid.Children.Add(CloseButtons);
				System.Windows.Controls.Grid.SetRow(CloseButtons, ii);
				System.Windows.Controls.Grid.SetColumn(CloseButtons, ii);
			}
			if (pWashoutEntryOrdersEnabled)
			{
				ii++;
				buttonGrid.Children.Add(WashoutButtons);
				System.Windows.Controls.Grid.SetRow(WashoutButtons, ii);
				System.Windows.Controls.Grid.SetColumn(WashoutButtons, ii);
			}			
			
			
			if (pStackEntryOrdersEnabled)
			{
				ii++;
				buttonGrid.Children.Add(StackButtons);
				System.Windows.Controls.Grid.SetRow(StackButtons, ii);
				System.Windows.Controls.Grid.SetColumn(StackButtons, ii);
			}
			
			if (pBracketEntryOrdersEnabled)
			{
				ii++;
				buttonGrid.Children.Add(BracketButtons);
				System.Windows.Controls.Grid.SetRow(BracketButtons, ii);
				System.Windows.Controls.Grid.SetColumn(BracketButtons, ii);
			}			
			
			ii++;
//			buttonGrid.Children.Add(OtherButtons);
//			System.Windows.Controls.Grid.SetRow(OtherButtons, ii);			

			
			
			
			
				
			
			
			
			
			
			UpdateButtons();

			
			
			
			if (TabSelected())
				InsertWPFOrderControls();

			chartWindow.MainTabControl.SelectionChanged += TabChangedHandler;
			
						
			if (!CheckOrderPanelReady())
				return;
			
		}
	

		private void DisposeWPFOrderControls()
		{
	
			
//			if (button1 != null)
//				button1.Click -= Button1_Click;

//			if (button2 != null)
//				button2.Click -= Button2_Click;

			if (chartWindow != null)
				chartWindow.MainTabControl.SelectionChanged -= TabChangedHandler;

			if (accountSelector != null)
							if (accountSelector.SelectedAccount != null)
							{
								//myAccount = accountSelector.SelectedAccount;
								
								// Unsubscribe to any prior account subscriptions
								accountSelector.SelectedAccount.AccountItemUpdate -= OnAccountItemUpdate2;
								accountSelector.SelectedAccount.ExecutionUpdate -= OnExecutionUpdate2;
								accountSelector.SelectedAccount.OrderUpdate -= OnOrderUpdate2;
								accountSelector.SelectedAccount.PositionUpdate -= OnPositionUpdate2;

						
							}
							
//			accountSelector.Cleanup();
//            atmStrategySelector.Cleanup();
//			tifSelector.Cleanup();
							
							
			RemoveWPFOrderControls();
		}

		private void InsertWPFOrderControls()
		{
			if (panelActive)
				return;

			tabControlStartColumn = System.Windows.Controls.Grid.GetColumn(chartWindow.MainTabControl);

			// a new column is added to the right of MainTabControl
			chartGrid.ColumnDefinitions.Insert((tabControlStartColumn + 1), new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(CTWidth) });

			// all items to the right of the MainTabControl are shifted to the right
			for (int i = 0; i < chartGrid.Children.Count; i++)
				if (System.Windows.Controls.Grid.GetColumn(chartGrid.Children[i]) > tabControlStartColumn)
					System.Windows.Controls.Grid.SetColumn(chartGrid.Children[i], System.Windows.Controls.Grid.GetColumn(chartGrid.Children[i]) + 1);

			// and then we set our new grid to be within the new column of the chart grid (and on the same row as the MainTabControl)
			System.Windows.Controls.Grid.SetColumn(buttonGrid, System.Windows.Controls.Grid.GetColumn(chartWindow.MainTabControl) + 1);
			System.Windows.Controls.Grid.SetRow(buttonGrid, System.Windows.Controls.Grid.GetRow(chartWindow.MainTabControl));

			chartGrid.Children.Add(buttonGrid);

			//UpdateQTYBox(ThisPositionNow());
				
			// let the script know the panel is active
			panelActive = true;
				
				
				
		}

		private void RemoveWPFOrderControls()
		{
			if (!panelActive)
				return;

			buttonGrid.PreviewKeyDown -= buttonGridKey;
			// remove the column of our added grid
			chartGrid.ColumnDefinitions.RemoveAt(System.Windows.Controls.Grid.GetColumn(buttonGrid));
			// then remove the grid
			chartGrid.Children.Remove(buttonGrid);

			// if the childs column is 1 (so we can move it to 0) and the column is to the right of the column we are removing, shift it left
			for (int i = 0; i < chartGrid.Children.Count; i++)
				if ( System.Windows.Controls.Grid.GetColumn(chartGrid.Children[i]) > 0 && System.Windows.Controls.Grid.GetColumn(chartGrid.Children[i]) > System.Windows.Controls.Grid.GetColumn(buttonGrid) )
					System.Windows.Controls.Grid.SetColumn(chartGrid.Children[i], System.Windows.Controls.Grid.GetColumn(chartGrid.Children[i]) - 1);

			panelActive = false;
		}

		private bool TabSelected()
		{
			bool tabSelected = false;

			// loop through each tab and see if the tab this indicator is added to is the selected item
			foreach (System.Windows.Controls.TabItem tab in chartWindow.MainTabControl.Items)
				if ((tab.Content as ChartTab).ChartControl == ChartControl && tab == chartWindow.MainTabControl.SelectedItem)
					tabSelected = true;

			return tabSelected;
		}

		private void TabChangedHandler(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count <= 0)
				return;

			tabItem = e.AddedItems[0] as System.Windows.Controls.TabItem;
			if (tabItem == null)
				return;

			chartTab = tabItem.Content as NinjaTrader.Gui.Chart.ChartTab;
			if (chartTab == null)
				return;

			if (TabSelected())
				InsertWPFOrderControls();
			else
				RemoveWPFOrderControls();
		}
		
		
		
		// chart toolbar
		
				
		protected void InsertWPFToolbarControls()
		{
			chartWindow = System.Windows.Window.GetWindow(ChartControl.Parent) as Chart;

			chartWindow.MainTabControl.SelectionChanged += MySelectionChangedHandler;

			
			// prevents multiple buttons for same chart window.
			
//			foreach (System.Windows.DependencyObject item in chartWindow.MainMenu)
//				if (System.Windows.Automation.AutomationProperties.GetAutomationId(item) == "VeritasOrderFlowToolbarMenu")
//					return;

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

			System.Windows.Automation.AutomationProperties.SetAutomationId(theMenu, "VeritasOrderFlowToolbarMenu");

			// thanks to Jesse for these figures to use t
			System.Windows.Media.Geometry topMenuItem1Icon = System.Windows.Media.Geometry.Parse("m 70.5 173.91921 c -4.306263 -1.68968 -4.466646 -2.46776 -4.466646 -21.66921 0 -23.88964 -1.364418 -22.5 22.091646 -22.5 23.43572 0 22.08568 -1.36412 22.10832 22.33888 0.0184 19.29356 -0.19638 20.3043 -4.64473 21.85501 -2.91036 1.01455 -32.493061 0.99375 -35.08859 -0.0247 z M 21 152.25 l 0 -7.5 20.25 0 20.25 0 0 7.5 0 7.5 -20.25 0 -20.25 0 0 -7.5 z m 93.75 0 0 -7.5 42.75 0 42.75 0 0 7.5 0 7.5 -42.75 0 -42.75 0 0 -7.5 z m 15.75 -38.33079 c -4.30626 -1.68968 -4.46665 -2.46775 -4.46665 -21.66921 0 -23.889638 -1.36441 -22.5 22.09165 -22.5 23.43572 0 22.08568 -1.364116 22.10832 22.338885 0.0185 19.293555 -0.19638 20.304295 -4.64473 21.855005 -2.91036 1.01455 -32.49306 0.99375 -35.08859 -0.0247 z M 21 92.25 l 0 -7.5 50.25 0 50.25 0 0 7.5 0 7.5 -50.25 0 -50.25 0 0 -7.5 z m 153.75 0 0 -7.5 12.75 0 12.75 0 0 7.5 0 7.5 -12.75 0 -12.75 0 0 -7.5 z M 55.5 53.919211 C 51.193737 52.229528 51.033354 51.451456 51.033354 32.25 51.033354 8.3603617 49.668936 9.75 73.125 9.75 96.560723 9.75 95.210685 8.3858835 95.23332 32.088887 95.25177 51.382441 95.03694 52.393181 90.588593 53.943883 87.678232 54.95844 58.095529 54.93764 55.5 53.919211 Z M 21 32.25 l 0 -7.5 12.75 0 12.75 0 0 7.5 0 7.5 -12.75 0 -12.75 0 0 -7.5 z m 78.75 0 0 -7.5 50.25 0 50.25 0 0 7.5 0 7.5 -50.25 0 -50.25 0 0 -7.5 z");

			// this is the menu item which will appear on the chart's Main Menu
			
		
			topMenuItem = new Gui.Tools.NTMenuItem()
			{
				Header				= "Order Flow",
				//Foreground			= Brushes.PowderBlue,
				//Icon				= topMenuItem1Icon,
				Margin				= new System.Windows.Thickness(0),
				Padding				= new System.Windows.Thickness(1),
				VerticalAlignment	= VerticalAlignment.Center,
				Style				= System.Windows.Application.Current.TryFindResource("MainMenuItem") as Style
					
			};

			theMenu.Items.Add(topMenuItem);

			topMenuItem.Click += TopMenuItem_Click;
			
			// subitems
			
//			topMenuItemSubItem1 = new Gui.Tools.NTMenuItem()
//			{
//				BorderThickness		= new System.Windows.Thickness(0),
//				Header				= "Submenu Item 1",
//				Style				= System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style
//			};

//			topMenuItemSubItem1.IsCheckable = true;
//			topMenuItemSubItem1.IsChecked = true;
			
//			topMenuItemSubItem1.Click += TopMenuItem1SubItem1_Click;
//			topMenuItem.Items.Add(topMenuItemSubItem1);

//			topMenuItemSubItem2 = new Gui.Tools.NTMenuItem()
//			{
//				Header				= "Submenu Item 2",
//				Style				= System.Windows.Application.Current.TryFindResource("InstrumentMenuItem") as Style
//			};

//			topMenuItemSubItem2.Click += TopMenuItem1SubItem2_Click;
//			topMenuItem.Items.Add(topMenuItemSubItem2);			

			
			
			// add the menu which contains all menu items to the chart
			
			
			chartWindow.MainMenu.Add(theMenu);

			foreach (System.Windows.Controls.TabItem tab in chartWindow.MainTabControl.Items)
				if ((tab.Content as ChartTab).ChartControl == ChartControl && tab == chartWindow.MainTabControl.SelectedItem)
				{
					topMenuItem.Visibility = Visibility.Visible;
				}
				else
				{
					//topMenuItem.Width = 0;
				}

			chartWindow.MainTabControl.SelectionChanged += MySelectionChangedHandler;
		}
		
		protected void TopMenuItem_Click (object sender, System.Windows.RoutedEventArgs e)
		{
		
//						Globals.RandomDispatcher.InvokeAsync(new Action(() =>
//						{
							
//							LaunchPropertiesWindow();
							
//						}));
					
			
			TriggerCustomEvent(o =>
   			{
      
				LaunchPropertiesWindow();
				 
				
				
			}, null);		
			
			
			//Draw.TextFixed(this, "infobox", "Main Menu Clicked", TextPosition.BottomLeft, Brushes.Green, new Gui.Tools.SimpleFont("Arial", 25), Brushes.Transparent, Brushes.Transparent, 100);
			//ChartControl.InvalidateVisual();
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
			{
				if (topMenuItem != null)
				{
					topMenuItem.Visibility = chartTab.ChartControl == ChartControl ? Visibility.Visible : Visibility.Hidden;
					
					if (chartTab.ChartControl == ChartControl)
					{
						topMenuItem.Visibility = Visibility.Visible;
						topMenuItem.Width = 100;
						
					}
					else
					{
						topMenuItem.Visibility = Visibility.Collapsed;
						topMenuItem.MinWidth = 0;
						topMenuItem.Width = 0;
						
					}
					
				}
				
				
			}
			
		}

		

		protected void RemoveWPFToolbarControls()
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
			Draw.TextFixed(this, "infobox", "M1I1 - Top menu subitem 1 selected", TextPosition.BottomLeft, Brushes.Green, new Gui.Tools.SimpleFont("Arial", 25), Brushes.Transparent, Brushes.Transparent, 100);
			ChartControl.InvalidateVisual();
		}

		protected void TopMenuItem1SubItem2_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Draw.TextFixed(this, "infobox", "M1I2 - Top menu subitem 2 selected", TextPosition.BottomLeft, Brushes.ForestGreen, new Gui.Tools.SimpleFont("Arial", 25), Brushes.Transparent, Brushes.Transparent, 100);
			ChartControl.InvalidateVisual();
		}
		
		
		
		
        protected void OnNewLogEvent(object sender, LogEventArgs e)
		{
			//Print(e.Message);
			
			if (ChartControl != null)			
			{
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

					string finalstring = "Your entry order" + ErrorCode + ".  Click To Continue.";

					finalstring = finalstring.Replace("strategy", "ATM Strategy");
					
					AddError(finalstring);
					ChartControl.InvalidateVisual();
					ChartControl.InvalidateVisual();
				}
					
			}
			//if (e.Message.Contains("Chart Trader close position"))// && chartTrader.IsFocused)
			//	pEntriesEnabled = false;
			
			//UpdateButtons();
			
		}
		
        protected void CTQtyUpdate(object sender, RoutedEventArgs e)
		{
			//Print(chartTraderQty.Value);
			
			//SetOrderParams();
			
			if (ChartControl != null)
			ChartControl.InvalidateVisual();
		}
				
			
		protected void InsertWPFControls()
		{
			
            //find chart trader from parent chart by it's automation id "ChartWindowChartTrader"
			

            chartTrader = Window.GetWindow(ChartControl.Parent).FindFirst("ChartWindowChartTraderControl") as ChartTrader;

       
            //Print(chartTrader.Width);
			
			
          // chartTrader.Width = pCTWidth;

            //chartTrader.ChartTraderVisibility = ChartTraderVisibility.Visible;


           // chartTrader.Background = pColorCTB;

            

            if (chartTrader == null)
            {

				

               // Print("ChartTrader is null");
                return;
            }
            else
            {

				


                chartTraderQty = chartTrader.FindFirst("ChartTraderControlQuantitySelector") as NinjaTrader.Gui.Tools.QuantityUpDown;

				chartTraderQty.Minimum = -100;
				
				chartTraderATM = chartTrader.FindFirst("ChartTraderControlATMStrategySelector") as NinjaTrader.Gui.NinjaScript.AtmStrategy.AtmStrategySelector;
				
				chartTraderTIF = chartTrader.FindFirst("ChartTraderControlTIFSelector") as NinjaTrader.Gui.Tools.TifSelector;
				
				chartTraderAcct = chartTrader.FindFirst("ChartTraderControlAccountSelector") as NinjaTrader.Gui.Tools.AccountSelector;

				//ThisCloseButton = chartTrader.FindFirst("ChartTraderControlButtonClose") as System.Windows.Controls.Button;
				
				//chartTrader. += CloseButtonClick;
				
                //ChartTraderProperties aaaa = chartTrader.FindFirst("ChartTraderProperties") as ChartTraderProperties;

			
                chartTraderQty.ValueChanged += CTQtyUpdate;
				chartTraderATM.SelectionChanged += CTQtyUpdate;
				chartTraderTIF.SelectionChanged += CTQtyUpdate;
				chartTraderAcct.SelectionChanged += CTQtyUpdate;

				
				//Print("adding log events");
				
				NinjaTrader.Cbi.Log.LogEvent += OnNewLogEvent;

//NinjaTrader.NinjaScript.AtmStrategy.StartAtmStrategy(selector.SelectedAtmStrategy, entryOrder);
				


				
                //chartTraderQty.Value = 2; // set value



                //SetOrderParams();

               // Print("ChartTrader is ok");
             //   return;
            }
						
						
		
        }

		protected void RemoveWPFControls()
		{

			if (ChartControl == null)
				return;
			
			chartTraderQty.ValueChanged -= CTQtyUpdate;
			chartTraderATM.SelectionChanged -= CTQtyUpdate;
			chartTraderTIF.SelectionChanged -= CTQtyUpdate;
			chartTraderAcct.SelectionChanged -= CTQtyUpdate;
			
				//ThisCloseButton.Click -= CloseButtonClick;

			NinjaTrader.Cbi.Log.LogEvent -= OnNewLogEvent;

			
            //chartTrader.Width = 212;


//            if (myAccount != null)
//			  {
//			      // Unsubscribe to any prior account subscriptions
//			      myAccount.AccountItemUpdate -= OnAccountItemUpdate;
//				 // myAccount.StateChanged -= OnAccountStatusUpdate;
//			      myAccount.ExecutionUpdate -= OnExecutionUpdate;
//			      myAccount.OrderUpdate -= OnOrderUpdate;
//			      myAccount.PositionUpdate -= OnPositionUpdate;
			 
//			  }
			  
			


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
		
			    public static string sep(string s)
		    {
		        int l = s.IndexOf(" -");
		        if (l >0)
		        {
		            return s.Substring(0, l);
		        }
		        return "";

		    }
			
		private void SetOrderParams()
		{
			
			//if (!pOrderPanelOn)
			//	return;
			
			//chartTraderQty.Value = qudSelector.Value;
			//if (chartTraderAcct != null)
			//chartTraderAcct.SelectedAccount = accountSelector.SelectedAccount;

			
			
			if (accountSelector != null)	
			try
			{
			
				
				pQty = qudSelector.Value;
				pLastQuantity = pQty;
					
				myAccount = accountSelector.SelectedAccount;
				pAccountName = myAccount.Name;
			
				pATMName = "";
				if (atmStrategySelector.SelectedAtmStrategy != null)
					pATMName = atmStrategySelector.SelectedAtmStrategy.DisplayName;
				
				//Print(pATMName);
				
				if (IsActiveATM())
					pATMName = sep(pATMName);
			
				pTIF = tifSelector.SelectedTif.ToString().ToUpper();
			

			
			}
			catch
			{}
			
			
				
			return;
			
	

			
			if (chartTraderAcct.SelectedAccount != null)
			{
				myAccount = chartTraderAcct.SelectedAccount;
				pAccountName = myAccount.Name;
			}
			
			//if (!chartTraderATM.SelectedAtmStrategy.DisplayName.Contains(" - "))
			
			try
			{
				pATMName = chartTraderATM.SelectedAtmStrategy.DisplayName;
				
				if (IsActiveATM())
					pATMName = sep(pATMName);
			
				pTIF = chartTraderTIF.SelectedTif.ToString().ToUpper();
			
				if (PreviousAccountName != pAccountName)
				{
					Subscribe();
					
					PreviousAccountName = pAccountName;
				}
			
			}
			catch
			{}
			

			if (label != null)
			label.Text = pAccountName + "\r\n" + pATMName + "  (" + pQty + ")";
			
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

		private void Subscribe()
		{
	
			//Print("NEW EVENTS " + pAccountName);
			
			
//		  if (myAccount != null)
//		  {
//		      // Unsubscribe to any prior account subscriptions
//		      myAccount.AccountItemUpdate -= OnAccountItemUpdate;
//			 // myAccount.StateChanged -= OnAccountStatusUpdate;
//		      myAccount.ExecutionUpdate -= OnExecutionUpdate;
//		      myAccount.OrderUpdate -= OnOrderUpdate;
//		      myAccount.PositionUpdate -= OnPositionUpdate;
		 
//		      // Subscribe to new account subscriptions
//		      myAccount.AccountItemUpdate   += OnAccountItemUpdate;
//			 // myAccount.StateChanged += OnAccountStatusUpdate;
//		      myAccount.ExecutionUpdate     += OnExecutionUpdate;
//		      myAccount.OrderUpdate         += OnOrderUpdate;
//		      myAccount.PositionUpdate     += OnPositionUpdate;
			  
			  
//		  }
		}	
		
		 private void OnAccountItemUpdate2 (object sender, AccountItemEventArgs e)
	    {
	         // Output the account item
			

			UpdatePNLBox(ThisPositionNow());
			
	    }
	
		private void OnAccountStatusUpdate2 (object sender, AccountStatusEventArgs e)
		{
			
			UpdatePNLBox(ThisPositionNow());
			
		    // Do something with the account status update
		}
		 
		 
		private void OnExecutionUpdate2 (object sender, ExecutionEventArgs e)
		{
		    // Do something with the execution update
			if (e.Execution.Instrument != Instrument || e.Execution.Account.Name != pAccountName)
				return;			
			
		//	Print(e.Execution.Position);
		//	Print(e.Execution.MarketPosition);	// doesnt show flat
			
			int q = e.Execution.Position;
			MarketPosition mp = MarketPosition.Flat;
			
			if (q != 0)
			{
				
				mp = e.Execution.MarketPosition;
				
			}
			
			//UpdateQTYBox(mp, q);
			
			
			UpdateQTYBox(ThisPositionNow());
			
//			foreach (Position pos in e.Execution.Account.Positions)
//			{
//				if (pos.Instrument == Instrument)
//					UpdateQTYBox(pos.MarketPosition, pos.Quantity);
				
//			}
			
			
			
			
		}

		private void OnOrderUpdate2 (object sender, OrderEventArgs  e)
		{
			
				
			// bracket trade
			
			Order ThisOrder = e.Order;
			
			if (ThisOrder.OrderState == OrderState.Filled)
			{
				if (MovingOrder != null)
				{
					if (ThisOrder == MovingOrder)
						MovingOrder = null;
					
				}
				
				if (MovingOrders.Count > 0)
				{
					if (ThisOrder == MovingOrder)
						MovingOrder = null;
					
					foreach (Order oo in MovingOrders.ToList())
					{
						if (oo == ThisOrder)
							DeleteOrders.Add(oo);
						
					}
					
					foreach (Order oo in DeleteOrders)
					{
						MovingOrders.Remove(oo);
						
					}				
					DeleteOrders.Clear();
					
				}
			}
			
			
			if (LongEntryOrder == null && SaveLongOrder)
			{						
				LongEntryOrder = ThisOrder;
				SaveLongOrder = false;
				
				UpdateButtons();
			}
			
			if (ThisOrder == LongEntryOrder)
			if (ThisOrder.OrderState == OrderState.Filled || ThisOrder.OrderState == OrderState.Cancelled)
			{
				LongEntryOrder = null;
				
				UpdateButtons();
			}
			
			
			
			if (ShortEntryOrder == null && SaveShortOrder)
			{						
				ShortEntryOrder = ThisOrder;
				SaveShortOrder = false;
				
				UpdateButtons();
			}
			
			if (ThisOrder == ShortEntryOrder)
			if (ThisOrder.OrderState == OrderState.Filled || ThisOrder.OrderState == OrderState.Cancelled)
			{
				ShortEntryOrder = null;
				
				UpdateButtons();
			}		
			
			
			
			// order totals and flags
			
			TriggerCustomEvent(o =>
   			{
      
				//TotalTheOrders();
				
				if (e.Order.OrderState == OrderState.Accepted || e.Order.OrderState == OrderState.Working || e.Order.OrderState == OrderState.Cancelled || e.Order.OrderState == OrderState.Filled)
					SetOrderFlags();
				
			
			}, null);
			
				
//			if (ChartControl != null)
//			ChartControl.Dispatcher.InvokeAsync((Action)(() =>
//			{
			
				
//				//SetOrderFlags();
				
				
//				//ChartControl.InvalidateVisual();
				
				
				
				
				
				
	
//			}));
								

		}		
		 
	    private void OnPositionUpdate2 (object sender, PositionEventArgs e)
	    {
			
			

//			UpdateQTYBox(ThisPositionNow());
//			UpdatePNLBox(ThisPositionNow());
			
			// Output the new position
	       

			
	

		
			//private System.Windows.Controls.TextBlock QTYBox 
			
						
	    }
		

		
		private string GetDollarString(string Sign, double Price)
		{
				
//					bool isInt = Price == (int)Price;
		
			
			
			bool IsNeg = Price < 0;
			
			double Price2 = Math.Abs(Price);
			
	
			
			string pr = Price2.ToString("N2");
			
//					if (isInt)
//					{
//						return Sign + pr.Substring(0, pr.Length-3);
//					}
//					else
			
			pr = Sign + pr;
			
			if (IsNeg)
				pr = "(" + pr + ")";
				//pr = "-" + pr;
				
						return pr;
						
        	
			
		}
		
		
		private void UpdatePNLBox (Position pp)
		{
				
			string flatstring = "PNL";
			double amount = 0;
			string ssss = "";
			
				double totalrealized = 0;
				double totalunrealized = 0;
			double totalunrealizedchart = 0;

			//Print(CurrentLast);
			
			
//			if (CurrentLast == 0)
//				return;
			
				if (pp != null)
				{
					
					totalunrealized = pp.GetUnrealizedProfitLoss(PerformanceUnit.Currency,CurrentLastData);
					
							if (pProfitAndLossType2 == "Points")
							{
								totalunrealized = pp.GetUnrealizedProfitLoss(PerformanceUnit.Points,CurrentLastData);
								
							}						
							if (pProfitAndLossType2 == "Ticks")
							{
								totalunrealized = pp.GetUnrealizedProfitLoss(PerformanceUnit.Ticks,CurrentLastData);
								
							}
							
							
					totalunrealizedchart = pp.GetUnrealizedProfitLoss(PerformanceUnit.Currency,CurrentLastData);
					
							if (pProfitAndLossType == "Points")
							{
								totalunrealizedchart = pp.GetUnrealizedProfitLoss(PerformanceUnit.Points,CurrentLastData);
								
							}						
							if (pProfitAndLossType == "Ticks")
							{
								totalunrealizedchart = pp.GetUnrealizedProfitLoss(PerformanceUnit.Ticks,CurrentLastData);
								
							}
							
							
					
					
					amount = totalunrealized;
				}
				else
				{
					totalrealized = myAccount.Get(AccountItem.RealizedProfitLoss, Currency.UsDollar);
					
					if (!pShowRealizedPNLWhenFlat)
						ssss = flatstring;
					
					amount = totalrealized;
				}
				
			
			SentToChartPNL = totalunrealizedchart;
			
			
			if (ChartControl != null)
			ChartControl.Dispatcher.InvokeAsync((Action)(() =>
			{
			
				
				
				
				//PNLBox.Text = "$" + Math.Round(amount,2).ToString("n2");
				
				PNLBox.Text = GetDollarString("$", amount);
				
				if (pProfitAndLossType2 == "Points")
				{
					PNLBox.Text = AllPriceMarker(amount);
					
				}						
				if (pProfitAndLossType2 == "Ticks")
				{
					PNLBox.Text = Math.Round(amount, 0).ToString();
					
				}
				
			
				
				PNLBox.Background = Brushes.Black;
				
//				if (amount == 0)
//				{
//					PNLBox.Foreground = Brushes.White;
//					//PNLBox.Foreground = Brushes.Black;
					
//				}
				if (amount > 0)
				{
					PNLBox.Foreground = Brushes.LimeGreen;
					//PNLBox.Foreground = Brushes.Black;
					
				}
								
				else
				{
					PNLBox.Foreground = Brushes.Red;	
					//PNLBox.Foreground = Brushes.White;
					
				}				
				
				if (ssss == flatstring)
				{
					PNLBox.Text = ssss;
					PNLBox.Foreground = Brushes.White;
				}
				
				
				
			}));
		}
			
		private void UpdateQTYBox (MarketPosition mp, int qt)
		{
				
			if (ChartControl != null)
			ChartControl.Dispatcher.InvokeAsync((Action)(() =>
			{
			
				
				int q = qt;
					
				int thisdir = 0;
				
		
					if (mp == MarketPosition.Long)
						thisdir = 1;
					if (mp == MarketPosition.Short)
						thisdir = -1;		
				
							
				DrawQTYBox(q, thisdir);
	
			}));
				
		}
					
		
		
		
		private void UpdateQTYBox (Position pp)
		{
				
			if (ChartControl != null)
			ChartControl.Dispatcher.InvokeAsync((Action)(() =>
			{
			
				
				int q = 0;
					
				int thisdir = 0;
				
				if (pp != null)
				{
					q = pp.Quantity;
					
					if (pp.MarketPosition == MarketPosition.Long)
						thisdir = 1;
					if (pp.MarketPosition == MarketPosition.Short)
						thisdir = -1;		
				}
					
				
				DrawQTYBox(q, thisdir);
				
				SentToChartQty = q;
				
				
			}));
				
		}
					
		
		
		private void DrawQTYBox (int q, int thisdir)
		{
				
			if (ChartControl != null)
			ChartControl.Dispatcher.InvokeAsync((Action)(() =>
			{
			
				
			
				
				QTYBox.Text = q.ToString();
				
				if (q == 0)
					QTYBox.Text = "Flat";
				
				QTYBox.Background = Brushes.Black;
				QTYBox.Foreground = Brushes.White;
				
				if (thisdir == 1)
				{
					QTYBox.Background = Brushes.LimeGreen;
					QTYBox.Foreground = Brushes.Black;
					
				}
				if (thisdir == -1)
				{
					QTYBox.Background = Brushes.Red;	
					QTYBox.Foreground = Brushes.White;
					
				}
				
				
				
				
				
			}));
				
		}
					
		
		
		 private void OnAccountItemUpdate (object sender, AccountItemEventArgs e)
	    {
	         // Output the account item
	        
	    }
	
		private void OnAccountStatusUpdate (object sender, AccountStatusEventArgs e)
		{
		    // Do something with the account status update
		}
		 
		 
		private void OnExecutionUpdate (object sender, ExecutionEventArgs e)
		{
		    // Do something with the execution update
		}

		private void OnOrderUpdate (object sender, OrderEventArgs  e)
		{

//			Order ThisOrder = e.Order;
			
//			if (LongEntryOrder == null && SaveLongOrder)
//			{						
//				LongEntryOrder = ThisOrder;
//				SaveLongOrder = false;
//			}
			
//			if (ThisOrder == LongEntryOrder)
//			if (ThisOrder.OrderState == OrderState.Filled || ThisOrder.OrderState == OrderState.Cancelled)
//			{
				
//				if (ThisOrder.OrderState == OrderState.Filled)
//				{

//				}				
				
//				LongEntryOrder = null;
//			}
			
//			if (ShortEntryOrder == null && SaveShortOrder)
//			{						
//				ShortEntryOrder = ThisOrder;
//				SaveShortOrder = false;
				
				
//			}
			
//			if (ThisOrder == ShortEntryOrder)
//			if (ThisOrder.OrderState == OrderState.Filled || ThisOrder.OrderState == OrderState.Cancelled)
//			{
//				if (ThisOrder.OrderState == OrderState.Filled)
//				{
					
							
					
//				}
				
				
//				ShortEntryOrder = null;
//			}

		}		
		 
	    private void OnPositionUpdate (object sender, PositionEventArgs e)
	    {
	         // Output the new position
			
			
//			if (e.Position.Quantity == 0)
//			{
//				Print("is o");
				
//			}
			
			
//			Print(e.Position);
			
//			UpdateQTYBox(e.Position);	
			
			
//			if (e.Position.Instrument != Instrument || e.Position.Account.Name != pAccountName)
//				return;
						
				

			//UpdateQTYBox(ThisPositionNow());
			//UpdatePNLBox(ThisPositionNow());
			
		
						
	    }
		
		
		private Position ThisPositionNow()
		{
			
			
			if (myAccount != null)
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
		
				
		private void CancelOrder(Order o)
		{
			
			
			
			instruction = OIF_CancelOrder(o.OrderId,"");

			if (instruction != string.Empty)
			{

				Submit();
			}
			
		}
		
	
		
		private void MoveOrder(Order o, double ThisMousePrice)
		{
		
		
			
			foreach (Order or in myAccount.Orders.ToList())
			{
				
				NewStopPrice = 0;
				
				//TrackedOrders(
						//OrderStateOK = or.OrderState == OrderState.Working || or.OrderState == OrderState.Accepted;
						//OrderTypeOK = or.OrderType == OrderType.StopLimit || or.OrderType == OrderType.StopMarket;
						//OrderNameOK = or.Name == "Stop1" ||  or.Name == "Stop2" ||  or.Name == "Stop3";
						OrderInstrumentOK = or.Instrument.FullName == Instrument.FullName;
				
				//OrderNameOK = or.Name.Contains("Stop");
				
				OrderStateOK = or.OrderState == OrderState.Working || or.OrderState == OrderState.Accepted;
				OrderTypeOK = true;
				OrderNameOK = true;
				
				if (or == o)
				//if (OrderTypeOK && OrderStateOK && OrderNameOK && OrderInstrumentOK)
				{
					//Print("fff");
					
					bool OKToMove = false;
					
					
					
					if (or.OrderType == OrderType.Limit)
					{
						
						NewLimitPrice = ThisMousePrice;
						OKToMove = true;
					}
					else if (or.OrderType == OrderType.MIT)
					{
						
						NewLimitPrice = ThisMousePrice;
						StopLimitOffset = or.StopPrice - or.LimitPrice;
						NewStopPrice = NewLimitPrice - StopLimitOffset;
						OKToMove = true;
					}		
					else
					{
						NewStopPrice = ThisMousePrice;
						StopLimitOffset = or.StopPrice - or.LimitPrice;
						NewLimitPrice = NewStopPrice - StopLimitOffset;
									
						CurrentStopPrice = or.StopPrice;
						
						if (or.LimitPrice == 0)
							NewLimitPrice = 0;						
						
						if (or.IsShort) // In Long Trade
						{

							
							OKToMove = NewStopPrice <= GetCurrentBid();
							
						}
						else // In Short Trade
						{
						

							OKToMove = NewStopPrice >= GetCurrentAsk();
							
						}
					
					}
					
					if (OKToMove)
					//if (NewStopPrice != 0 && OKToMove)
						instruction = OIF_ChangeOrder(0,NewLimitPrice,NewStopPrice,or.OrderId.ToString(),"");
					
					Submit();
					
					
					
					
					//Print(or.Name);
				}
				
			}
			
					

		}
		
		
		
		
		
		
		private void BuyNow (string WhichEntry)
		{
			ChartControl.Dispatcher.InvokeAsync((Action)(() =>
			{
				
				
//				Print("BuyNow");
				
				
//				if (LongEntryOrder != null)
//					CancelOrder(LongEntryOrder);
					
//				if (ShortEntryOrder != null)
//					CancelOrder(ShortEntryOrder);
				
				int thisoffset = pLimitOrderOffset2;
				string thisordert = pThisEntryType2;
				
				if (WhichEntry == "Stack")
				{
					thisoffset = pLimitOrderOffset2;
					thisordert = pThisEntryType2;
				}
				else if (WhichEntry == "Close")
				{
					thisoffset = pLimitOrderOffset1;
					thisordert = pThisEntryType1;					
					
				}				
				else if (WhichEntry == "Washout")
				{
					thisoffset = pLimitOrderOffset6;
					thisordert = pThisEntryType4;					
					
				}	
				
				SetOrderParams();
				
				if (thisordert == "Market")
					BuyMarket(pQty, pATMName);
				if (thisordert == "Limit")
				{
					EntryOrderPrice = RTTS(CurrentLastData - thisoffset*tSize);
					EntryOrderPrice = CurrentAsk - thisoffset*tSize;
					
					BuyLimit(RTTS(EntryOrderPrice), false);
				}
						

			}));
			
		}
		
		private void SellNow (string WhichEntry)
		{
			ChartControl.Dispatcher.InvokeAsync((Action)(() =>
			{
				
//				Print("SellNow");
				
				
//				if (LongEntryOrder != null)
//					CancelOrder(LongEntryOrder);
					
//				if (ShortEntryOrder != null)
//					CancelOrder(ShortEntryOrder);
				
				int thisoffset = pLimitOrderOffset2;
				string thisordert = pThisEntryType2;
				
				if (WhichEntry == "Stack")
				{
					thisoffset = pLimitOrderOffset2;
					thisordert = pThisEntryType2;
				}
				else if (WhichEntry == "Close")
				{
					thisoffset = pLimitOrderOffset1;
					thisordert = pThisEntryType1;					
					
				}		
				else if (WhichEntry == "Washout")
				{
					thisoffset = pLimitOrderOffset6;
					thisordert = pThisEntryType4;					
					
				}	
				
				SetOrderParams();
				
				if (thisordert == "Market")
					SellMarket(pQty, pATMName);
				if (thisordert == "Limit")
				{
					EntryOrderPrice = RTTS(CurrentLastData + thisoffset*tSize);
					EntryOrderPrice = CurrentBid + thisoffset*tSize;
					
					SellLimit(RTTS(EntryOrderPrice), false);
				}
				
//				if (!pAutoEnabled)
//				{
//					DisableTheSystem();
//				}
				
			}));			
		}
		
		
		private void BuyLimit (double FinalPrice, bool pUseMIT)
		{

			SetOrderParams();
			
			CurrentType = "LIMIT";
			SLMPrice = 0;
			if (pUseMIT)
			{
				SLMPrice = FinalPrice;
				CurrentType = "MIT";	
			}
			
			UniqueOrderId = string.Concat("ECBC",pAccountName,SpreadName,DateTime.Now.Ticks);		
			
				if (pATMName == "" || pATMName == "Custom")
					instruction = OIF_PlaceOrder(pAccountName, Instrument.FullName, "BUY", pQty, CurrentType, FinalPrice, SLMPrice, pTIF, OCOID, "", "", "");
//				else if (ActiveATMSelected())
//					instruction = OIF_PlaceOrder(pAccountName, Instrument.FullName, "BUY", pQty, "LIMIT", FinalPrice, 0, pTIF, CHLI, "","",ActiveATM(pATMName)+UniqueOrderId);
				else			
					instruction = OIF_PlaceOrder(pAccountName, Instrument.FullName, "BUY", pQty, CurrentType, FinalPrice, SLMPrice, pTIF, OCOID, "", pATMName, pATMName+UniqueOrderId);
					
				
				
			Submit();		
				
			//BarToCancelLimitOrder = LastCurrentBar + pLimitOrderBars;
				
			//SaveLongOrder = true;
			
		}

		private void SellLimit (double FinalPrice, bool pUseMIT)
		{

			SetOrderParams(); 
			
			CurrentType = "LIMIT";
			SLMPrice = 0;
			if (pUseMIT)
			{
				SLMPrice = FinalPrice;
				CurrentType = "MIT";	
			}	
			
			UniqueOrderId = string.Concat("ECSC",pAccountName,SpreadName,DateTime.Now.Ticks);		
			
				if (pATMName == "" || pATMName == "Custom")
					instruction = OIF_PlaceOrder(pAccountName, Instrument.FullName, "SELL", pQty, CurrentType, FinalPrice, SLMPrice, pTIF, OCOID, "", "", "");
//				else if (ActiveATMSelected())
//					instruction = OIF_PlaceOrder(pAccountName, Instrument.FullName, "BUY", pQty, "LIMIT", FinalPrice, 0, pTIF, CHLI, "","",ActiveATM(pATMName)+UniqueOrderId);
				else			
					instruction = OIF_PlaceOrder(pAccountName, Instrument.FullName, "SELL", pQty, CurrentType, FinalPrice, SLMPrice, pTIF, OCOID, "", pATMName, pATMName+UniqueOrderId);
					
				
				
			Submit();	
				
			//BarToCancelLimitOrder = LastCurrentBar + pLimitOrderBars;
			
			//SaveShortOrder = true;
				
		}
		
	
		private void BuyStop(double FinalPrice)
		{	
			SetOrderParams();
			
			CurrentType = "STOPMARKET";
			SLMPrice = 0;
			if (pUseSLM)
			{
				SLMPrice = RTTS(FinalPrice+pSLOffset*TickSize);
				CurrentType = "STOPLIMIT";	
			}
			
			UniqueOrderId = string.Concat("ECTBC",pAccountName,SpreadName,DateTime.Now.Ticks);
			//CHLI = UniqueOrderId;
					
				if (pATMName == "" || pATMName == "Custom")
					instruction = OIF_PlaceOrder(pAccountName, Instrument.FullName, "BUY", pQty, CurrentType, SLMPrice, FinalPrice, pTIF, OCOID,"", "", "");
				//else if (ActiveATMSelected())
				//	instruction = OIF_PlaceOrder(pAccountName, Instrument.FullName, "BUY", pQty, "STOP", 0, BuyEntryPrice, pTIF,UniqueOrderId,BuyUniqueOrderId,"",ActiveATM(pATMName)+UniqueOrderId);
				else
					instruction = OIF_PlaceOrder(pAccountName, Instrument.FullName, "BUY", pQty, CurrentType, SLMPrice, FinalPrice, pTIF, OCOID, "", pATMName, pATMName+UniqueOrderId);
			
			if (instruction != string.Empty)
			{

				Submit();
			}
		}
		
		private void SellStop(double FinalPrice)
		{	
			SetOrderParams();
			
			CurrentType = "STOPMARKET";
			SLMPrice = 0;
			if (pUseSLM)
			{
				SLMPrice = RTTS(FinalPrice-pSLOffset*TickSize);
				CurrentType = "STOPLIMIT";	
			}
			
			
			UniqueOrderId = string.Concat("ECTSC",pAccountName,SpreadName,DateTime.Now.Ticks);
			//CHLI = UniqueOrderId;
			
			if (pATMName == "" || pATMName == "Custom")
				instruction = OIF_PlaceOrder(pAccountName, Instrument.FullName, "SELL", pQty, CurrentType, SLMPrice, FinalPrice, pTIF, OCOID ,"", "", "");
			//else if (ActiveATMSelected())
			//	instruction = OIF_PlaceOrder(pAccountName, Instrument.FullName, "SELL", pQty, "STOP", 0, SellEntryPrice, pTIF, UniqueOrderId ,SellUniqueOrderId,"",ActiveATM(pATMName)+UniqueOrderId);
			else
				instruction = OIF_PlaceOrder(pAccountName, Instrument.FullName, "SELL", pQty, CurrentType, SLMPrice, FinalPrice, pTIF, OCOID ,"", pATMName, pATMName+UniqueOrderId);	

			if (instruction != string.Empty)
			{
				Submit();
			}
			
		}			
	
		private void BuyStop2(double FinalPrice)
		{	
			SetOrderParams();
			

			
			CurrentType = "STOPMARKET";
			SLMPrice = 0;
			if (pThisEntryType3 == "SLM")
			{
				SLMPrice = RTTS(FinalPrice+pSLOffset2*TickSize);
				CurrentType = "STOPLIMIT";	
			}
			
			UniqueOrderId = string.Concat("ECTBC",pAccountName,SpreadName,DateTime.Now.Ticks);
			//CHLI = UniqueOrderId;
					
				if (pATMName == "" || pATMName == "Custom")
					instruction = OIF_PlaceOrder(pAccountName, Instrument.FullName, "BUY", pQty, CurrentType, SLMPrice, FinalPrice, pTIF, OCOBracketID,"", "", "");
				//else if (ActiveATMSelected())
				//	instruction = OIF_PlaceOrder(pAccountName, Instrument.FullName, "BUY", pQty, "STOP", 0, BuyEntryPrice, pTIF,UniqueOrderId,BuyUniqueOrderId,"",ActiveATM(pATMName)+UniqueOrderId);
				else
					instruction = OIF_PlaceOrder(pAccountName, Instrument.FullName, "BUY", pQty, CurrentType, SLMPrice, FinalPrice, pTIF, OCOBracketID, "", pATMName, pATMName+UniqueOrderId);
			
			if (instruction != string.Empty)
			{

				Submit();
			}
		}
		
		private void SellStop2(double FinalPrice)
		{	
			SetOrderParams();
			
			CurrentType = "STOPMARKET";
			SLMPrice = 0;
			if (pThisEntryType3 == "SLM")
			{
				SLMPrice = RTTS(FinalPrice-pSLOffset2*TickSize);
				CurrentType = "STOPLIMIT";	
			}
			
			UniqueOrderId = string.Concat("ECTSC",pAccountName,SpreadName,DateTime.Now.Ticks);
			//CHLI = UniqueOrderId;
			
			if (pATMName == "" || pATMName == "Custom")
				instruction = OIF_PlaceOrder(pAccountName, Instrument.FullName, "SELL", pQty, CurrentType, SLMPrice, FinalPrice, pTIF, OCOBracketID ,"", "", "");
			//else if (ActiveATMSelected())
			//	instruction = OIF_PlaceOrder(pAccountName, Instrument.FullName, "SELL", pQty, "STOP", 0, SellEntryPrice, pTIF, UniqueOrderId ,SellUniqueOrderId,"",ActiveATM(pATMName)+UniqueOrderId);
			else
				instruction = OIF_PlaceOrder(pAccountName, Instrument.FullName, "SELL", pQty, CurrentType, SLMPrice, FinalPrice, pTIF, OCOBracketID ,"", pATMName, pATMName+UniqueOrderId);	

			if (instruction != string.Empty)
			{
				Submit();
			}
			
		}	
		
		
		
		private void BuyMarket (int pQty, string pATMName)
		{
			SetOrderParams();
			
			UniqueOrderId = string.Concat("BS",pAccountName,SpreadName,DateTime.Now.Ticks);
			
			if (pATMName == "" || pATMName == "Custom")
					instruction = OIF_PlaceOrder(pAccountName, Instrument.FullName, "BUY", pQty, "MARKET", 0, 0, pTIF, "", "", "", "");
				//else if (ActiveATMSelected())
				//	instruction = OIF_PlaceOrder(pAccountName, Instrument.FullName, "BUY", pQty, "MARKET", 0, 0, pTIF, "", "","",ActiveATM(pATMName)+UniqueOrderId);
				else			
					instruction = OIF_PlaceOrder(pAccountName, Instrument.FullName, "BUY", pQty, "MARKET", 0, 0, pTIF, "", "", pATMName, pATMName+UniqueOrderId);
							
			
			Submit();			
			
		}
		
		
		private void SellMarket (int pQty, string pATMName)
		{
			SetOrderParams();
			
			UniqueOrderId = string.Concat("BS",pAccountName,SpreadName,DateTime.Now.Ticks);
			
				if (pATMName == "" || pATMName == "Custom")
					instruction = OIF_PlaceOrder(pAccountName, Instrument.FullName, "SELL", pQty, "MARKET", 0, 0, pTIF, "", "", "", "");
				//else if (ActiveATMSelected())
				//	instruction = OIF_PlaceOrder(pAccountName, Instrument.FullName, "SELL", pQty, "MARKET", 0, 0, pTIF, "", "","",ActiveATM(pATMName)+UniqueOrderId);
				else			
					instruction = OIF_PlaceOrder(pAccountName, Instrument.FullName, "SELL", pQty, "MARKET", 0, 0, pTIF, "", "", pATMName, pATMName+UniqueOrderId);
				
			Submit();	
		}
			
				
		private double GetOriginalPrice(Order ord)
		{
			foreach (Order or in myAccount.Orders.ToList())
			{
				
				NewStopPrice = 0;
				
				//TrackedOrders(
						OrderStateOK = or.OrderState == OrderState.Working || or.OrderState == OrderState.Accepted;
						OrderTypeOK = or.OrderType == OrderType.StopLimit || or.OrderType == OrderType.StopMarket;
						OrderNameOK = or.Name == "Stop1" ||  or.Name == "Stop2" ||  or.Name == "Stop3";
						OrderInstrumentOK = or.Instrument.FullName == Instrument.FullName;
				
				OrderNameOK = or.Name.Contains("Stop");
				
				//if (OrderTypeOK && OrderStateOK && OrderNameOK && OrderInstrumentOK)
				
				
				
				if (ord == or)
				{
						
					
//					foreach (OrderEventArgs oea in ord.OrderUpdates)
//					{
					
//						Print(oea.or.OrderAction);
						
//					}
					
//					ord.OrderUpdates.Where(p => p.Order > 5)
//      .Select(p => p.Nationality)
//      .AsParallel()
//      .ForAll(n => AssignCitizenShip(n);
					
					bool OKToMove = false;
					
//					if (or.IsShort) // In Long Trade
//					{
//						if (Mode == "BE")
//							NewStopPrice = RTTS(ThisPositionNow().AveragePrice + pBEOffset*TickSize);
						
////						if (Mode == "SA")
////							NewStopPrice = LongSAPrice;
						
//						OKToMove = NewStopPrice <= GetCurrentBid();
						
//					}
//					else // In Short Trade
//					{
//						if (Mode == "BE")
//							NewStopPrice = RTTS(ThisPositionNow().AveragePrice - pBEOffset*TickSize);;
						
////						if (Mode == "SA")
////							NewStopPrice = ShortSAPrice;

//						OKToMove = NewStopPrice >= GetCurrentAsk();
						
//					}
					
					
					
					StopLimitOffset = or.StopPrice - or.LimitPrice;
					NewLimitPrice = NewStopPrice - StopLimitOffset;
								
					CurrentStopPrice = or.StopPrice;
					
					if (or.LimitPrice == 0)
						NewLimitPrice = 0;
					
					
					
					
					
					
					//Print(or.Name);
				}
				
			}
			
			return 0;
						
			
		}
		
		private void UpdateStopLoss(string Mode)
		{
		
		
			
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
						if (Mode == "BE")
							NewStopPrice = RTTS(ThisPositionNow().AveragePrice + pBEOffset*TickSize);
						
//						if (Mode == "SA")
//							NewStopPrice = LongSAPrice;
						
						OKToMove = NewStopPrice <= GetCurrentBid();
						
					}
					else // In Short Trade
					{
						if (Mode == "BE")
							NewStopPrice = RTTS(ThisPositionNow().AveragePrice - pBEOffset*TickSize);;
						
//						if (Mode == "SA")
//							NewStopPrice = ShortSAPrice;

						OKToMove = NewStopPrice >= GetCurrentAsk();
						
					}
					
					Print(or.OrderUpdates.Count);
					
					StopLimitOffset = or.StopPrice - or.LimitPrice;
					NewLimitPrice = NewStopPrice - StopLimitOffset;
								
					CurrentStopPrice = or.StopPrice;
					
					if (or.LimitPrice == 0)
						NewLimitPrice = 0;
					
					
					if (NewStopPrice != 0 && OKToMove)
						instruction = OIF_ChangeOrder(0,NewLimitPrice,NewStopPrice,or.OrderId.ToString(),"");
					
					Submit();
					
					
					
					
					//Print(or.Name);
				}
				
			}
			
					

		}
		
		private void UpdateProfitTargets(string Mode)
		{
		
		
			
			foreach (Order or in myAccount.Orders.ToList())
			{
				
				NewStopPrice = 0;
				
				//TrackedOrders(
						OrderStateOK = or.OrderState == OrderState.Working || or.OrderState == OrderState.Accepted;
						OrderTypeOK = or.OrderType == OrderType.StopLimit || or.OrderType == OrderType.StopMarket;
						OrderNameOK = or.Name == "Stop1" ||  or.Name == "Stop2" ||  or.Name == "Stop3";
						OrderInstrumentOK = or.Instrument.FullName == Instrument.FullName;
				
				OrderNameOK = or.Name.Contains("Target");
				OrderTypeOK = true;
				
				if (OrderTypeOK && OrderStateOK && OrderNameOK && OrderInstrumentOK)
				{
						
					bool OKToMove = false;
					
					if (or.IsShort) // In Long Trade
					{
						if (Mode == "BE")
							NewLimitPrice = RTTS(ThisPositionNow().AveragePrice + pBEOffset*TickSize);
						
//						if (Mode == "SA")
//							NewStopPrice = LongSAPrice;
						
						//OKToMove = NewStopPrice <= GetCurrentBid();
						
					}
					else // In Short Trade
					{
						if (Mode == "BE")
							NewLimitPrice = RTTS(ThisPositionNow().AveragePrice - pBEOffset*TickSize);;
						
//						if (Mode == "SA")
//							NewStopPrice = ShortSAPrice;

						//OKToMove = NewStopPrice >= GetCurrentAsk();
						
					}
					
//					StopLimitOffset = or.StopPrice - or.LimitPrice;
//					NewLimitPrice = NewStopPrice - StopLimitOffset;
								
//					CurrentStopPrice = or.StopPrice;
					
					NewStopPrice = 0;
					
//					if (or.LimitPrice == 0)
//						NewLimitPrice = 0;

					
					OKToMove = true;
					
					if (NewLimitPrice != 0 && OKToMove)
						instruction = OIF_ChangeOrder(0,NewLimitPrice,NewStopPrice,or.OrderId.ToString(),"");
					
					Submit();
					
					
					
					
					//Print(or.Name);
				}
				
			}
			
					

		}
				
		private void CancelStopOrders()
		{ 
					
foreach (Order or in myAccount.Orders.ToList())
			{
				
			
				OrderStateOK = or.OrderState == OrderState.Working || or.OrderState == OrderState.Accepted || or.OrderState == OrderState.ChangePending  || or.OrderState == OrderState.ChangeSubmitted || or.OrderState == OrderState.CancelPending || or.OrderState == OrderState.CancelSubmitted;
				OrderInstrumentOK = or.Instrument.FullName == Instrument.FullName;
				OrderNameOK = true;
				OrderTypeOK = true;		
			
				if (OrderTypeOK && OrderStateOK && OrderNameOK && OrderInstrumentOK)						
				if (or.Name.Contains ("Stop"))
				{
					
					//if (thisbutton.Value.Width == or.StopPrice)
						CancelOrder(or);
	
				}
				
			}			
		}
		
		private void CancelTargetOrders()
		{ 
						
			foreach (Order or in myAccount.Orders.ToList())
			{
				
			
				OrderStateOK = or.OrderState == OrderState.Working || or.OrderState == OrderState.Accepted || or.OrderState == OrderState.ChangePending  || or.OrderState == OrderState.ChangeSubmitted || or.OrderState == OrderState.CancelPending || or.OrderState == OrderState.CancelSubmitted;
				OrderInstrumentOK = or.Instrument.FullName == Instrument.FullName;
				OrderNameOK = true;
				OrderTypeOK = true;		
			
				if (OrderTypeOK && OrderStateOK && OrderNameOK && OrderInstrumentOK)						
				if (or.Name.Contains ("Target"))
				{
					
					//if (thisbutton.Value.Width == or.StopPrice)
						CancelOrder(or);
	
				}
				
			}						
		}		
		
		private void CancelATMOrders()
		{ 
	
			CancelStopOrders();	
			CancelTargetOrders();
				
			
		}
		
		private void CancelAllOrders()
		{
					foreach (Order or in myAccount.Orders.ToList())
								{
									
								
									OrderStateOK = or.OrderState == OrderState.Working || or.OrderState == OrderState.Accepted || or.OrderState == OrderState.ChangePending  || or.OrderState == OrderState.ChangeSubmitted || or.OrderState == OrderState.CancelPending || or.OrderState == OrderState.CancelSubmitted;
									OrderInstrumentOK = or.Instrument.FullName == Instrument.FullName;
									OrderNameOK = true;
									OrderTypeOK = true;		
								
									if (OrderTypeOK && OrderStateOK && OrderNameOK && OrderInstrumentOK)						
									//if (or.Name.Contains ("Stop"))
									{
										
										//if (thisbutton.Value.Width == or.StopPrice)
											CancelOrder(or);
						
									}
									
								}
								
			
		}
		
		private void ClosePosition()
		{
			
							
			CancelATMOrders();
								
			if (ThisPositionNow() != null)
			{
				if (ThisPositionNow().MarketPosition == MarketPosition.Long)
				{
					SellMarket(ThisPositionNow().Quantity,"");
				}
				
				if (ThisPositionNow().MarketPosition == MarketPosition.Short)
				{
					BuyMarket(ThisPositionNow().Quantity,"");
				}					
					
					
					
					
			}
			
			
			
			
			
			
		}		
		
		
		private void CloseAll()
		{
//			instruction = OIF_ClosePosition(pAccountName, Instrument.FullName);

////			if (ThisPositionNow() != null)
//			if (instruction != string.Empty)
//			{

//				Submit();
//			}
			
			
			CancelStopOrders();	
			CancelTargetOrders();
			CancelAllOrders();
			
			
			
			
			if (ThisPositionNow() != null)
			{
				if (ThisPositionNow().MarketPosition == MarketPosition.Long)
				{
					SellMarket(ThisPositionNow().Quantity,"");
				}
				
				if (ThisPositionNow().MarketPosition == MarketPosition.Short)
				{
					BuyMarket(ThisPositionNow().Quantity,"");
				}					
					
					
					
					
			}
			

			
			
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

		
		
 		protected override void OnConnectionStatusUpdate(ConnectionStatusEventArgs connectionStatusUpdate)
		{
			//return;
			
			//ChartControl.Dispatcher.InvokeAsync(() =>
			//{
			
			
			
				if (connectionStatusUpdate.PriceStatus == ConnectionStatus.Connected
					&& connectionStatusUpdate.Connection.InstrumentTypes.Contains(ThisMasterInstrument.InstrumentType)
					&& Bars.BarsType.IsTimeBased
					&& Bars.BarsType.IsIntraday)
				{
					connected = true;

					//if (pTimerEnabled)
					//if (DisplayTime() && timer == null)
				if (DisplayTime() && timer == null)
				{
					ChartControl.Dispatcher.InvokeAsync(() =>
					{
						timer			= new System.Windows.Threading.DispatcherTimer { Interval = new TimeSpan(0, 0, 1), IsEnabled = true };
						timer.Tick		+= OnTimerTick;
					});
				}
										
									
					if (TestLoad && timer3 == null)
					{
						ChartControl.Dispatcher.InvokeAsync(() =>
						{
							timer3			= new System.Windows.Threading.DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 500), IsEnabled = true };
							timer3.Tick		+= OnTimerTick3;
						});
					}

									
									
				}
				else if (connectionStatusUpdate.PriceStatus == ConnectionStatus.Disconnected)
					connected = false;
			
				
				
				
			//});
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

			if (DisplayTime())
			{
				if (timer != null && !timer.IsEnabled)
					timer.IsEnabled = true;

				if (connected)
				{
					if (SessionIterator.IsInSession(Now, false, true))
					{
						if (hasRealtimeData)
						{
							TimeSpan barTimeLeft = BarsArray[0].GetTime(BarsArray[0].Count - 1).Subtract(Now);

							
							timeLeft = (barTimeLeft.Ticks < 0
								? "0:00"
								: barTimeLeft.Hours.ToString("00") + ":" + barTimeLeft.Minutes.ToString("00") + ":" + barTimeLeft.Seconds.ToString("00"));
							
							if (barTimeLeft.TotalHours < 1 && barTimeLeft.Ticks >= 0)
								timeLeft = barTimeLeft.Minutes.ToString("0") + ":" + barTimeLeft.Seconds.ToString("00");
							
							
							BarTimerString = timeLeft;
							
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
		
		private void OnTimerTick3(object sender, EventArgs e)
		{

				
			if (TestLoad)
			if (State == State.Historical)
			{
				
			
				double perc = Math.Round(BarsProcessed/BarsRemaining*100, 0);
				
				string ssss = perc + " %";
				EndLoad = DateTime.Now;
				TotalLoadTime = EndLoad.Subtract(StartLoad);
				
				if (ssss.Contains("N"))
					ssss = string.Empty;
				
				Print(TotalLoadTime.Seconds + "." + TotalLoadTime.Milliseconds + "   " + ssss);
				//Print(BarsProcessed);
				//Print(BarsRemaining);
				//Print(ssss);
				
				
				//Draw.TextFixed(this, "infobox", ssss, TextPosition.BottomLeft);
				
				//Draw.TextFixed(this, ssss, TextPosition.BottomLeft, Brushes.Green, new Gui.Tools.SimpleFont("Arial", 25), Brushes.Transparent, Brushes.Transparent, 100);
				ChartControl.InvalidateVisual();
				
				
				
			}
			
			
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

		

	
		
        internal void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
			 		if (InMenu || InMenu2 || InMenu3 || InMenu4)
						return;
					
                    if (pCrossHairEnabled)
                    {
                        pCrossHairEnabled = false;
                    }
                    else
                    {
                        pCrossHairEnabled = true;
                    }
					
			foreach (KeyValuePair<double, ButtonZ> thisbutton in AllButtonZ1)
            {
                bool hoverednew = MouseIn(thisbutton.Value.Rect, MenuButtonExpandP, MenuButtonExpandP);
                string buttonn = thisbutton.Value.Text;

				hoverednew = true;
				if (hoverednew && buttonn == "Crosshair")
                {
					
                 
                    thisbutton.Value.Switch = pCrossHairEnabled;
                   
				}		
				
                   
			}
			
			this.ChartControl.InvalidateVisual();
		}
		
		public void UpdateFromPropertyWindow()
		{
			//message = string.Format( "Instrument: {0} | Integer: {1} | String: {2}", ((InstrumentValue != null) ? InstrumentValue.FullName : "null"), IntValue, StringValue );
			//Draw.TextFixed(this, "valuesBox", message, TextPosition.Center, ChartControl.Properties.ChartText, new SimpleFont("Arial", 20), Brushes.Black, ChartControl.Properties.ChartBackground, 100);
			
								
			myProperties.ChartBackground = pChartBackgroundBrush;	
					myProperties.ChartText = pChartAxisBrush;	
					
					myProperties.GridLineHPen.DashStyleHelper = pGridLineHStroke.DashStyleHelper;
					myProperties.GridLineHPen.Width = pGridLineHStroke.Width;
					myProperties.GridLineHPen.Pen = pGridLineHStroke.Pen;
					
					myProperties.GridLineVPen.DashStyleHelper = pGridLineVStroke.DashStyleHelper;
					myProperties.GridLineVPen.Width = pGridLineVStroke.Width;
					myProperties.GridLineVPen.Pen = pGridLineVStroke.Pen;					
					
				
					//ChartBars.Properties.PriceMarker.IsVisible = pShowLastPriceMarker2;
					ChartBars.Properties.PaintPriceMarker = pShowLastPriceMarker2 && pShowLastPriceAll;
					ChartBars.Properties.PriceMarker.Background = pColorLastPriceMarker;
			
			
			ForceRefresh();
		}
		
		private void LaunchPropertiesWindow()
		{
			// Window is kept alive between opens — re-show the existing one (fast).
			if (ThisPropertyGridWindow != null)
			{
				try { ThisPropertyGridWindow.ReShow(); } catch { }
				return;
			}
			ThisPropertyGridWindow = new VeritasOrderFlowPropertyGridWindow()
			{
				selectedIndicator = this
			};
			try { ThisPropertyGridWindow.Owner = Window.GetWindow(ChartControl); } catch { }
			// Show() (modeless), not ShowDialog() — a modal dialog disables the chart
			// window and freezes its rendering. Close just hides it (see OnWindowClosing).
			ThisPropertyGridWindow.Show();
		}
	
		private void ClosePropertiesWindow()
		{
			try
			{
				if (ThisPropertyGridWindow != null)
					ThisPropertyGridWindow.ForceClose();
				ThisPropertyGridWindow = null;
			}
			catch { ThisPropertyGridWindow = null; }
		}
		
		
		private void SetBarSize(int width, int space)
		{
			ChartBars.Properties.ChartStyle.BarWidth = width/2;
			//ChartBars.Properties.ChartStyle.BarWidthUI = (int) ChartBars.Properties.ChartStyle.BarWidth + 3;
			ChartControl.Properties.BarDistance = (int) (width) + 1 + space;
			
			this.ChartControl.InvalidateVisual();
			
		}
		
		
//		private void GetBarSize(int width, int space)
//		{
//			ChartBars.Properties.ChartStyle.BarWidth = width/2;
//			//ChartBars.Properties.ChartStyle.BarWidthUI = (int) ChartBars.Properties.ChartStyle.BarWidth + 3;
//			ChartControl.Properties.BarDistance = (int) (width) + 1 + space;
			
//			this.ChartControl.InvalidateVisual();
			
//		}		
		
		
        internal void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
			// Panel-header drag release: toggle expand immediately on mouse-up
			// so the user doesn't have to wiggle the mouse to see the state change.
			if (draggingPanelId == PANEL_ID)
			{
				if (Math.Abs(dragOffsetY) < 5f)
					SetPanelExpanded(!panelExpanded);
				draggingPanelId = null;
				dragOffsetY = 0;
				if (ChartPanel != null) ChartPanel.ReleaseMouseCapture();
				if (ChartControl != null) ChartControl.InvalidateVisual();
			}

			 IsDragging = false;

		}
		

//		internal void OnMouseMove(object sender, MouseEventArgs e)
//    	{
//            this.MP = e.GetPosition(this.ChartPanel);
			
//			FinalXPixel = MP.X / 100 * dpiX;
//			FinalYPixel = MP.Y / 100 * dpiY;
			
//		}
         
			
//        private void OnMouseLeave(object sender, System.EventArgs e) 
//        {
//               InMenu = false;
//                InMenu2 = false;
//				InMenu3 = false;
//				InMenu4 = false;
//        }		
					
        internal void OnMouseLeave(object sender, MouseEventArgs e)
        {
            // Release the hand-cursor override when the mouse leaves the chart.
            if (_cursorOverrideActive) { System.Windows.Input.Mouse.OverrideCursor = null; _cursorOverrideActive = false; }
               InMenu = false;
                InMenu2 = false;
				InMenu3 = false;
				InMenu4 = false;
				SetPanelMenuOpen(false);
				if (draggingPanelId != null) { draggingPanelId = null; dragOffsetY = 0; if (ChartPanel != null) ChartPanel.ReleaseMouseCapture(); }

			//BuyClickReady = false;
			//SellClickReady = false;

		}

        internal void OnPanelMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!InMenu3 && !IsAnyPanelMenuOpen()) return;

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


        internal void PNLMouseDown(object sender, MouseButtonEventArgs e)
        {		
			
			if (ThisPositionNow() != null)
			{
				
				
					if (pProfitAndLossType2 == "Currency")
							{								
								pProfitAndLossType2 = "Ticks";
								
							}
							else if(pProfitAndLossType2 == "Ticks")
							{								
								pProfitAndLossType2 = "Points";
								
							}
							else if(pProfitAndLossType2 == "Points")
							{								
								pProfitAndLossType2 = "Currency";
								
							}				
				
			}
			else
			{
			
				if (pShowRealizedPNLWhenFlat)
				{
					pShowRealizedPNLWhenFlat = false;
				}
				else
				{
					pShowRealizedPNLWhenFlat = true;
				}
			}
			
			UpdatePNLBox(ThisPositionNow());
							
			
		}
		
		
	
		
		
		
        internal void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
			//Print("MouseDown");
			
            this.MP = e.GetPosition(this.ChartPanel);
            if (dpiX > 0)
            {
                FinalXPixel = MP.X / 100 * dpiX;
                FinalYPixel = MP.Y / 100 * dpiY;
            }

			//return;


			if (e.RightButton == MouseButtonState.Pressed)
				return;

            // Panel header click — start drag
            if (e.LeftButton == MouseButtonState.Pressed && MouseIn(headerRect, 3, 3))
            {
                draggingPanelId = PANEL_ID;
                dragStartY = (float)FinalYPixel;
                dragOffsetY = 0;
                if (ChartPanel != null) ChartPanel.CaptureMouse();
                e.Handled = true;
                return;
            }


            IsDragging = false;

			InBlockNow = false;


		if (AllErrorMessages.Count > 0)
			{
				AllErrorMessages.Clear();
				ChartControl.InvalidateVisual();
				
				//myProperties.AllowSelectionDragging = PreviousDrag;
				
				return;
				
			}
	// EXECUTION BUTTONS
			
			
				if (MouseIn(ClickEntryCancelRect,0,0))
				{
					BuyClickReady = false;
					SellClickReady = false;
					
					UpdateButtons();
					
					ChartControl.InvalidateVisual();
					
					return;
					
				}
				
				
				if (MouseIn(MoveOrderCancelRect,0,0))
				{
					MovingOrder = null;
					MovingOrders.Clear();
					
					ChartControl.InvalidateVisual();
					
					return;
					
				}
				
				
			
				if (BuyClickReady)				
				{
					
					if (CurrentMousePrice <= CurrentAsk)
					{
						BuyLimit(CurrentMousePrice, pUseMIT);
					}       
					else
					{
						BuyStop(CurrentMousePrice);
					}					

					BuyClickReady = false;
					UpdateButtons();
					
					//this.ChartControl.InvalidateVisual();
					
					return;
				}
				
				if (SellClickReady)
					
				{
				
					if (CurrentMousePrice >= CurrentBid)
					{
						SellLimit(CurrentMousePrice, pUseMIT);    
					}       
					else
					{
						SellStop(CurrentMousePrice);
					}						
					
				
 
					
					SellClickReady = false;
					UpdateButtons();
					//this.ChartControl.InvalidateVisual();
						
					return;
					
				}
				
							
			
			
			// all order cancel buttons
			
            foreach (KeyValuePair<double, OrderDetails> thisbutton in AllOrderCancelButtons)
                {
	   				
					//foreach (SharpDX.RectangleF R in BlockTradeButtons)
					//{
						if (MouseIn(thisbutton.Value.ThisRectA,0,0))
						{
							InBlockNow = true;	
							
							if (thisbutton.Value.Width == 0) // cancel a single order
							{
								
								Order or = thisbutton.Value.ThisOrder;
								
								CancelOrder(or);
		
								if (or.Name.Contains ("Stop") || or.Name.Contains ("Target"))
								{
								
									if (pCloseOnStopCancel)
									{
										//Print(or.IsLong);
										
										if (or.IsLong)
										{
											BuyMarket(or.Quantity, "");
										}
										if (or.IsShort)
										{
											SellMarket(or.Quantity, "");
										}												
									}
									
											
								}
										
							}
							
							else if (thisbutton.Value.Switch == true) // cancel all orders at a single price
							{
								
								foreach (Order or in myAccount.Orders.ToList())
								{
									
								
									OrderStateOK = or.OrderState == OrderState.Working || or.OrderState == OrderState.Accepted || or.OrderState == OrderState.ChangePending  || or.OrderState == OrderState.ChangeSubmitted || or.OrderState == OrderState.CancelPending || or.OrderState == OrderState.CancelSubmitted;
									OrderInstrumentOK = or.Instrument.FullName == Instrument.FullName;
									OrderNameOK = true;
									OrderTypeOK = true;		
								
									if (OrderTypeOK && OrderStateOK && OrderNameOK && OrderInstrumentOK)						
									//if (or.Name.Contains ("Stop"))
									{
										
										double MainPrice = or.StopPrice;
																
										if (or.OrderType == OrderType.Limit)
										{
											MainPrice = or.LimitPrice;											
										}
										
										if (thisbutton.Value.Width == MainPrice)
										{
											
											
											CancelOrder(or);
											
											if (or.Name.Contains ("Stop") || or.Name.Contains ("Target"))
											{
											
												if (pCloseOnStopCancel)
												{
													//Print(or.IsLong);
													
													if (or.IsLong)
													{
														BuyMarket(or.Quantity, "");
													}
													if (or.IsShort)
													{
														SellMarket(or.Quantity, "");
													}												
												}
												
														
											}
											
											
										}
									}
									
								}
								
								
							}							
							else // cancel a group of stop orders
							{
							
								foreach (Order or in myAccount.Orders.ToList())
								{
									
								
									OrderStateOK = or.OrderState == OrderState.Working || or.OrderState == OrderState.Accepted || or.OrderState == OrderState.ChangePending  || or.OrderState == OrderState.ChangeSubmitted || or.OrderState == OrderState.CancelPending || or.OrderState == OrderState.CancelSubmitted;
									OrderInstrumentOK = or.Instrument.FullName == Instrument.FullName;
									OrderNameOK = true;
									OrderTypeOK = true;		
								
									if (OrderTypeOK && OrderStateOK && OrderNameOK && OrderInstrumentOK)						
									if (or.Name.Contains ("Stop"))
									{
										
										if (thisbutton.Value.Width == or.StopPrice)
										{
											
											
											CancelOrder(or);
											
											if (pCloseOnStopCancel)
											{
												if (or.IsLong)
												{
													BuyMarket(or.Quantity, "");
												}
												if (or.IsShort)
												{
													SellMarket(or.Quantity, "");
												}												
											}
											
											
										}
									}
									
								}
								
							}
							
							
							
							return;
						}
						
					//}
				
				
//					bool hoverednew = MouseIn(thisbutton.Value.Rect, 2, 2);
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

                }

				
			// COMPLETE MOVING ORDERS PROCESS
		
			if (MovingOrder != null)
			{
				
				
				MoveOrder(MovingOrder, CurrentMousePrice);	
				MovingOrder = null;
				
				return;
			}
				
			if (MovingOrders.Count > 0)
			{
				
				//Print(MovingOrders.Count);
				
				foreach (Order oo in MovingOrders)
				{
					MoveOrder(oo, CurrentMousePrice);	
					
				}
				
				MovingOrders.Clear();
				
				return;
			}				
				
				
			
			// split stop buttons
			
            foreach (KeyValuePair<double, OrderDetails> thisbutton in AllStopCombinationButtons)
                {
	   				
					//foreach (SharpDX.RectangleF R in BlockTradeButtons)
					//{
						if (MouseIn(thisbutton.Value.ThisRectA,0,0))
						{
							
							if (pSplitStopDisplay)
						    {
						        pSplitStopDisplay = false;
						    }
						    else
						    {
						        pSplitStopDisplay = true;
						    }
								
							ChartControl.InvalidateVisual();
							return;
							
						}
						
					//}
				
				
//					bool hoverednew = MouseIn(thisbutton.Value.Rect, 2, 2);
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

                }				
					
				
				
			// INITIATE MOVING ORDERS PROCESS
				
            foreach (KeyValuePair<double, OrderDetails> thisbutton in AllOrderMoveButtons)
                {
	   				
					//foreach (SharpDX.RectangleF R in BlockTradeButtons)
					//{
						if (MouseIn(thisbutton.Value.ThisRectA,0,0))
						{
							InBlockNow = true;	
							
							//CancelOrder(thisbutton.Value.ThisOrder);
							
							//Print(thisbutton.Value.Width);
							if (thisbutton.Value.Switch == true) // move all orders at a price
							{
								foreach (Order or in myAccount.Orders.ToList())
								{
									
								
									OrderStateOK = or.OrderState == OrderState.Working || or.OrderState == OrderState.Accepted || or.OrderState == OrderState.ChangePending  || or.OrderState == OrderState.ChangeSubmitted || or.OrderState == OrderState.CancelPending || or.OrderState == OrderState.CancelSubmitted;
									OrderInstrumentOK = or.Instrument.FullName == Instrument.FullName;
									OrderNameOK = true;
									OrderTypeOK = true;		
								
									if (OrderTypeOK && OrderStateOK && OrderNameOK && OrderInstrumentOK)						
									//if (or.Name.Contains ("Stop"))
									{
	
										double MainPrice = or.StopPrice;
																
										if (or.OrderType == OrderType.Limit)
										{
											MainPrice = or.LimitPrice;											
										}
										
										
										if (thisbutton.Value.Width == MainPrice)										
										//if (thisbutton.Value.Width == or.StopPrice)
											MovingOrders.Add(or);
						
											
									}
									
								}
								
								
							}							
							else if (thisbutton.Value.Width != 0) // move all stop orders at a price
							{
								foreach (Order or in myAccount.Orders.ToList())
								{
									
								
									OrderStateOK = or.OrderState == OrderState.Working || or.OrderState == OrderState.Accepted || or.OrderState == OrderState.ChangePending  || or.OrderState == OrderState.ChangeSubmitted || or.OrderState == OrderState.CancelPending || or.OrderState == OrderState.CancelSubmitted;
									OrderInstrumentOK = or.Instrument.FullName == Instrument.FullName;
									OrderNameOK = true;
									OrderTypeOK = true;		
								
									if (OrderTypeOK && OrderStateOK && OrderNameOK && OrderInstrumentOK)						
									if (or.Name.Contains ("Stop"))
									{
										
										if (thisbutton.Value.Width == or.StopPrice)
											MovingOrders.Add(or);
						
									}
									
								}
								
								
							}
							else // move a single order
							{
								
								
								
							
								MovingOrder = thisbutton.Value.ThisOrder;
								
							}
							
							return;
						}
						
					//}
				
				
//					bool hoverednew = MouseIn(thisbutton.Value.Rect, 2, 2);
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

                }				
				

	
				
					//Print(InBlockNow);
				
				if (InBlockNow != InBlockNowP)
				{
					//Print("Refresh Block");
					this.ChartControl.InvalidateVisual();
				}
				
				InBlockNowP = InBlockNow;       	
						

			// position display
				// close
						if (MouseIn(ThisRect6,0,0))
						{
							ClosePosition();
							return;
						}

									
// BE buttons 1
												
						
						if (MouseIn(ThisRect4,0,0))
						{
							if (ThisPositionNow() != null)
							{
								if (ThisPositionNow().MarketPosition == MarketPosition.Long) // In Long Trade
								{
									
									NewLimitPrice = RTTS(ThisPositionNow().AveragePrice + pBEOffset*TickSize);

									if (NewLimitPrice < CurrentLastData)
									{
										if (pBEMoveStops) UpdateStopLoss("BE");
									}
									else
									{
										if (pBEMoveTargets) UpdateProfitTargets("");
									}
									
									
								}
								
								if (ThisPositionNow().MarketPosition == MarketPosition.Short) // In Short Trade
								{
									//if (Mode == "BE")
										NewLimitPrice = RTTS(ThisPositionNow().AveragePrice - pBEOffset*TickSize);;
									
			//						if (Mode == "SA")
			//							NewStopPrice = ShortSAPrice;

									//OKToMove = NewStopPrice >= GetCurrentAsk();
								
									if (NewLimitPrice > CurrentLastData)
									{
										if (pBEMoveStops) UpdateStopLoss("BE");
									}
									else
									{
										if (pBEMoveTargets) UpdateProfitTargets("");
									}
								
								}
								
								
							}
							
							return;
							
						}
						
						
						
	// area for pnl display'
							
						if (MouseIn(ThisRect3,0,0))
						{
							if (pProfitAndLossType == "Currency")
							{								
								pProfitAndLossType = "Ticks";
								
							}
							else if(pProfitAndLossType == "Ticks")
							{								
								pProfitAndLossType = "Points";
								
							}
							else if(pProfitAndLossType == "Points")
							{								
								pProfitAndLossType = "Currency";
								
							}							
							
							this.ChartControl.InvalidateVisual();
							
							return;
						}							
							 
			
			
			
			
			
//            foreach (KeyValuePair<double, ButtonZ> thisbutton in AllButtonZ4)
//            {
//                bool hoverednew = MouseIn(thisbutton.Value.Rect, MenuButtonExpandP, MenuButtonExpandP);
//                string buttonn = thisbutton.Value.Text;

//				if (hoverednew)
//				{
					
					
					
								
//					if (buttonn == "Auto")
//					{
									
//					    if (pAutoEnabled)
//					    {
//					        pAutoEnabled = false;
//					    }
//					    else
//					    {
//					        pAutoEnabled = true;
//					    }
//					    thisbutton.Value.Switch = pAutoEnabled;
//					    this.ChartControl.InvalidateVisual();
//						this.ChartControl.InvalidateVisual();					
						
//					}
					
//					else if (buttonn == "Buy Stack")
//					{
									
//					    if (BuyStackReady)
//					    {
//					        BuyStackReady = false;
//					    }
//					    else
//					    {
//					        BuyStackReady = true;
//					    }
//					    thisbutton.Value.Switch = BuyStackReady;
//					    this.ChartControl.InvalidateVisual();
//						this.ChartControl.InvalidateVisual();					
						
//					}
//					else if (buttonn == "Sell Stack")
//					{
							
//					    if (SellStackReady)
//					    {
//					        SellStackReady = false;
//					    }
//					    else
//					    {
//					        SellStackReady = true;
//					    }
//					    thisbutton.Value.Switch = SellStackReady;
//					    this.ChartControl.InvalidateVisual();
//						this.ChartControl.InvalidateVisual();							
						
//					}				
//					else if (buttonn == "Buy Market")
//					{
						
//					    BuyMarket();
//						this.ChartControl.InvalidateVisual();
//						this.ChartControl.InvalidateVisual();									
						
//					}
//					else if (buttonn == "Sell Market")
//					{
							
					   
//					    SellMarket();
//						this.ChartControl.InvalidateVisual();
//						this.ChartControl.InvalidateVisual();					   
//					}											
//					else if (buttonn == "Close")
//					{
							
					   
//					    ClosePosition();
//						this.ChartControl.InvalidateVisual();
//						this.ChartControl.InvalidateVisual();					   
//					}	
//					else if (buttonn == "B/E")
//					{
							
					   
//					    UpdateStopLoss("BE");
//						this.ChartControl.InvalidateVisual();
//						this.ChartControl.InvalidateVisual();					   
//					}						
					
					
//				}
					
//			}
			

						bool IsClickedOn = false;
						
						
			foreach (KeyValuePair<double, ButtonZ> thisbutton in AllButtonZ3)
//            foreach (KeyValuePair<double, ButtonZ> thisbutton in AllButtonZ2)
            {
                bool hoverednew = MouseIn(thisbutton.Value.Rect, MenuButtonExpandP, MenuButtonExpandP);
                string buttonn = thisbutton.Value.Text;


				 
				if (hoverednew && buttonn == "Order Panel")
				{
					//if (CheckOrderPanelReady())
					{
					    if (pOrderPanelOn)
					    {
					        pOrderPanelOn = false;
							
					    }
					    else
					    {
							
							//Print ("IsClickedOn");
							
							IsClickedOn = true;
							
					        pOrderPanelOn = true;
							pOrdersDisplayOn = true;
							
							TriggerCustomEvent(o =>
				   			{
				      
								//TotalTheOrders();
									SetOrderFlags();
								
							
							}, null);
				
					    }
					    thisbutton.Value.Switch = pOrderPanelOn;
				
					}
					
					
					
					
					if (pOrderPanelOn)
					{
				
			//						if (myProperties.ChartTraderVisibility == ChartTraderVisibility.Collapsed)
//							myProperties.ChartTraderVisibility = ChartTraderVisibility.VisibleCollapsed;
						
						
						
						//chartTrader.IsEnabled = true;						
						//buttonTwoGrid1.IsEnabled = true;
						
						
						//if (chartTrader.Visibility == Visibility.Collapsed)
						//	chartTrader.Visibility = Visibility.Hidden;
						
			
						if (ChartControl != null)
						{
							ChartControl.Dispatcher.InvokeAsync((Action)(() =>
							{
								CreateWPFOrderControls();
							}));
						}
				
				
						//buttonTwoGrid1.Visibility = Visibility.Visible;
					}
					else
					{
						
						//chartTrader.Visibility = Visibility.Collapsed;
						
						//chartTrader.IsEnabled = false;						
						//buttonTwoGrid1.IsEnabled = false;
						
						if (ChartControl != null)
						{
							ChartControl.Dispatcher.InvokeAsync((Action)(() =>
							{
								DisposeWPFOrderControls();
							}));
						}
			
					
						//buttonTwoGrid1.Visibility = Visibility.Collapsed;
					}
					
					
					

					
					

										
					
					
					
				}
			}
			
			
			
			if (IsClickedOn)
			{
				
						foreach (KeyValuePair<double, ButtonZ> thisbuttonn in AllButtonZ3)
			            {
			               // bool hoverednew2 = MouseIn(thisbuttonn.Value.Rect, MenuButtonExpandP, MenuButtonExpandP);
			                string buttonnn = thisbuttonn.Value.Text;

							if (buttonnn == "Order Display")
			                {
								
			                 
			                    thisbuttonn.Value.Switch = true;
			                   
							}		
							
			                   
						}	
						
				    this.ChartControl.InvalidateVisual();
					this.ChartControl.InvalidateVisual();
						
						return;
						
			}
			
			
			
			
			
			
			foreach (KeyValuePair<double, ButtonZ> thisbutton in AllButtonZ3)
			//foreach (KeyValuePair<double, ButtonZ> thisbutton in AllButtonZ1)
            {
                bool hoverednew = MouseIn(thisbutton.Value.Rect, MenuButtonExpandP, MenuButtonExpandP);
                string buttonn = thisbutton.Value.Text;

			
				if (hoverednew && buttonn == "Order Display")
				{
				    if (pOrdersDisplayOn)
				    {
				        pOrdersDisplayOn = false;
				    }
				    else
				    {
				        pOrdersDisplayOn = true;
				    }
				    thisbutton.Value.Switch = pOrdersDisplayOn;
					
							TriggerCustomEvent(o =>
				   			{
				      
								//TotalTheOrders();
									SetOrderFlags();
								
							
							}, null);					
					
			

				    this.ChartControl.InvalidateVisual();
					this.ChartControl.InvalidateVisual();
					
					return;
										
					
				}	
				
				
				
				if (hoverednew && buttonn == "Ghost")
				{
				    if (pShowOrdersOffChart)
				    {
				        pShowOrdersOffChart = false;
				    }
				    else
				    {
				        pShowOrdersOffChart = true;
				    }
				    thisbutton.Value.Switch = pShowOrdersOffChart;
					
					
					
			

				    this.ChartControl.InvalidateVisual();
					this.ChartControl.InvalidateVisual();
					
					return;
										
					
				}		
				
				
				if (hoverednew && buttonn == "Summary")
				{
				    if (pShowOrderSummary)
				    {
				        pShowOrderSummary = false;
				    }
				    else
				    {
				        pShowOrderSummary = true;
				    }
				    thisbutton.Value.Switch = pShowOrderSummary;
					
					
					
			

				    this.ChartControl.InvalidateVisual();
					this.ChartControl.InvalidateVisual();
					
					return;
										
					
				}					
				
				if (hoverednew && buttonn == "Split Stops")
				{
				    if (pSplitStopDisplay)
				    {
				        pSplitStopDisplay = false;
				    }
				    else
				    {
				        pSplitStopDisplay = true;
				    }
				    thisbutton.Value.Switch = pSplitStopDisplay;

				    this.ChartControl.InvalidateVisual();
					this.ChartControl.InvalidateVisual();
										
					return;
				}	
				
				
				if (hoverednew && buttonn == "MIT")
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
					this.ChartControl.InvalidateVisual();
										
					return;
				}					
				
				
				if (hoverednew && buttonn == "SLM")
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
					this.ChartControl.InvalidateVisual();
										
					return;
				}	
				
				
				
								
			}
			
			
			
			foreach (KeyValuePair<double, ButtonZ> thisbutton in AllButtonZ3)
            //foreach (KeyValuePair<double, ButtonZ> thisbutton in AllButtonZ2)
            {
                bool hoverednew = MouseIn(thisbutton.Value.Rect, MenuButtonExpandP, MenuButtonExpandP);
                string buttonn = thisbutton.Value.Text;


	
				if (hoverednew && buttonn == "Properties")
				{
					
		
					TriggerCustomEvent(o =>
		   			{
		      
						LaunchPropertiesWindow();
						 
						
						
					}, null);					  
					
				    thisbutton.Value.Switch = false;
				    this.ChartControl.InvalidateVisual();
					this.ChartControl.InvalidateVisual();
					
				}					
				
				else if (hoverednew && buttonn == "Audio Alerts")
				{
					
				    if (pAudioEnabledMain)
				    {
				        pAudioEnabledMain = false;
				    }
				    else
				    {
				        pAudioEnabledMain = true;
				    }
				    thisbutton.Value.Switch = pAudioEnabledMain;
				    this.ChartControl.InvalidateVisual();
					this.ChartControl.InvalidateVisual();
					
				}	
				else if (hoverednew && buttonn == "Heads Up Display")
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
					this.ChartControl.InvalidateVisual();
					
				}	
				else if (hoverednew && buttonn == "Composite Profile")
				{
					
				

//					if (pCompositeLocation == "Left")
//					{
//						pCompositeLocation = "Right";	
						
//					}
//					else if (pCompositeLocation == "Right")
//					{
//						pCompositeLocation = "None";	
						
//					}
//					else
//					{
//						pCompositeLocation = "Left";	
						
//					}						
					
					
					
				    if (pCompositeEnabled)
				    {
				        pCompositeEnabled = false;
				    }
				    else
				    {
				        pCompositeEnabled = true;
				    }
					
				    thisbutton.Value.Switch = pCompositeLocation != "None";
					thisbutton.Value.Name = pCompositeLocation;
					
				    this.ChartControl.InvalidateVisual();
					this.ChartControl.InvalidateVisual();
					
				}					
				else if (hoverednew && buttonn == "Market Depth")
				{
					
				    if (pDepthEnabled)
				    {
				        pDepthEnabled = false;
				    }
				    else
				    {
				        pDepthEnabled = true;
				    }
				    thisbutton.Value.Switch = pDepthEnabled;
				    this.ChartControl.InvalidateVisual();
					this.ChartControl.InvalidateVisual();
					
				}					
				else if (hoverednew && buttonn == "Composite Levels")
				{
					
				    if (pCompAllLevelsEnabled)
				    {
				        pCompAllLevelsEnabled = false;
				    }
				    else
				    {
				        pCompAllLevelsEnabled = true;
				    }
				    thisbutton.Value.Switch = pCompAllLevelsEnabled;
				    this.ChartControl.InvalidateVisual();

				}					

				else if (hoverednew && buttonn == "Zones (Momentum) ")
				{
					
				    if (pZonesEnabled2)
				    {
				        pZonesEnabled2 = false;
				    }
				    else
				    {
				        pZonesEnabled2 = true;
				    }
				    thisbutton.Value.Switch = pZonesEnabled2;
				    this.ChartControl.InvalidateVisual();

				}						
				else if (hoverednew && buttonn == "Zones (Imbalance)")
				{
					
				    if (pZonesEnabled)
				    {
				        pZonesEnabled = false;
				    }
				    else
				    {
				        pZonesEnabled = true;
				    }
				    thisbutton.Value.Switch = pZonesEnabled;
				    this.ChartControl.InvalidateVisual();

				}	
				else if (hoverednew && buttonn == "Swing Levels")
				{
					
				    if (pPivotLinesEnabled)
				    {
				        pPivotLinesEnabled = false;
				    }
				    else
				    {
				        pPivotLinesEnabled = true;
				    }
				    thisbutton.Value.Switch = pPivotLinesEnabled;
				    this.ChartControl.InvalidateVisual();

				}					
				else if (hoverednew && buttonn == "Numbers")
				{
					
				 	if (pCompNumberDisplayMode == "Bid / Ask")
						{
							pCompNumberDisplayMode = "Volume";
						}
						else if (pCompNumberDisplayMode == "Volume")
						{
							pCompNumberDisplayMode = "Delta";
						}
						else if (pCompNumberDisplayMode == "Delta")
						{
							pCompNumberDisplayMode = "None";
						}					
						else if (pCompNumberDisplayMode == "None")
						{
							pCompNumberDisplayMode = "Bid / Ask";
						}	
						
				    thisbutton.Value.Switch = pCompNumberDisplayMode == "None" ? false : true;
					thisbutton.Value.Name = pCompNumberDisplayMode;
						
						
				    this.ChartControl.InvalidateVisual();
					this.ChartControl.InvalidateVisual();

				}				
				else if (hoverednew && buttonn == "Histogram")
				{
					
				 	if (pCompHistogramDisplayMode == "Volume & Delta")
						{
							pCompHistogramDisplayMode = "Volume";
						}
						else if (pCompHistogramDisplayMode == "Volume")
						{
							pCompHistogramDisplayMode = "None";
						}
						else if (pCompHistogramDisplayMode == "None")
						{
							pCompHistogramDisplayMode = "Volume & Delta";
						}					
							
						
				    thisbutton.Value.Switch = pCompHistogramDisplayMode == "None" ? false : true;
					thisbutton.Value.Name = pCompHistogramDisplayMode;
						
				    this.ChartControl.InvalidateVisual();
					this.ChartControl.InvalidateVisual();

				}						

						
				
				
			}
			
			
			
			
			foreach (KeyValuePair<double, ButtonZ> thisbutton in AllButtonZ5)
            {
                bool hoverednew = MouseIn(thisbutton.Value.Rect, MenuButtonExpandP, MenuButtonExpandP);
                string buttonn = thisbutton.Value.Text;


				if (hoverednew && buttonn == "Location")
				{
					
				

					if (pCompositeLocation == "Left")
					{
						pCompositeLocation = "Right";	
						
					}
					else if (pCompositeLocation == "Right")
					{
						pCompositeLocation = "Left";	
						
					}
					else
					{
						pCompositeLocation = "Left";	
						
					}						
					
					
					
//				    if (pCompositeEnabled)
//				    {
//				        pCompositeEnabled = false;
//				    }
//				    else
//				    {
//				        pCompositeEnabled = true;
//				    }
					
				    thisbutton.Value.Switch = pCompositeLocation != "None";
					thisbutton.Value.Name = pCompositeLocation;
					
				    this.ChartControl.InvalidateVisual();
					this.ChartControl.InvalidateVisual();
					
				}					
					
				else if (hoverednew && buttonn == "Composite Levels")
				{
					
				    if (pCompAllLevelsEnabled)
				    {
				        pCompAllLevelsEnabled = false;
				    }
				    else
				    {
				        pCompAllLevelsEnabled = true;
				    }
				    thisbutton.Value.Switch = pCompAllLevelsEnabled;
				    this.ChartControl.InvalidateVisual();

				}					

				
				else if (hoverednew && buttonn == "Numbers")
				{
					
				 	if (pCompNumberDisplayMode == "Bid / Ask")
						{
							pCompNumberDisplayMode = "Volume";
						}
						else if (pCompNumberDisplayMode == "Volume")
						{
							pCompNumberDisplayMode = "Delta";
						}
						else if (pCompNumberDisplayMode == "Delta")
						{
							if (pCompHistogramDisplayMode != "None")
								pCompNumberDisplayMode = "None";
							else
								pCompNumberDisplayMode = "Bid / Ask";
						}					
						else if (pCompNumberDisplayMode == "None")
						{
							pCompNumberDisplayMode = "Bid / Ask";
						}	
						
				    thisbutton.Value.Switch = pCompNumberDisplayMode == "None" ? false : true;
					thisbutton.Value.Name = pCompNumberDisplayMode;
						
						
				    this.ChartControl.InvalidateVisual();
					this.ChartControl.InvalidateVisual();

				}				
				else if (hoverednew && buttonn == "Histogram")
				{
					
				 	if (pCompHistogramDisplayMode == "Volume & Delta")
						{
							pCompHistogramDisplayMode = "Volume";
						}
						else if (pCompHistogramDisplayMode == "Volume")
						{
							pCompHistogramDisplayMode = "None";
							
							if (pCompNumberDisplayMode != "None")
								pCompHistogramDisplayMode = "None";
							else
								pCompHistogramDisplayMode = "Volume & Delta";
														
						}
						else if (pCompHistogramDisplayMode == "None")
						{
							pCompHistogramDisplayMode = "Volume & Delta";
						}					
							
						
				    thisbutton.Value.Switch = pCompHistogramDisplayMode == "None" ? false : true;
					thisbutton.Value.Name = pCompHistogramDisplayMode;
						
				    this.ChartControl.InvalidateVisual();
					this.ChartControl.InvalidateVisual();

				}						

						
				
				
			}
			
			
			
			
			// TOP CHART BUTTONS
			
			foreach (KeyValuePair<double, ButtonZ> thisbutton in AllButtonZ3)
           // foreach (KeyValuePair<double, ButtonZ> thisbutton in AllButtonZ)
            {
                bool hoverednew = MouseIn(thisbutton.Value.Rect, MenuButtonExpandP, MenuButtonExpandP);
                string buttonn = thisbutton.Value.Text;


				if (hoverednew && buttonn == "Crosshair")
                {
					
                    if (pCrossHairEnabled)
                    {
                        pCrossHairEnabled = false;
                    }
                    else
                    {
                        pCrossHairEnabled = true;
                    }
                    thisbutton.Value.Switch = pCrossHairEnabled;
                    this.ChartControl.InvalidateVisual();
				
				}		
				else if (hoverednew && buttonn == "Split")
                {
                   
					
                    if (pSplit)
                    {
                        pSplit = false;
                    }
                    else
                    {
                        pSplit = true;
                    }
                    thisbutton.Value.Switch = pSplit;
                    this.ChartControl.InvalidateVisual();
					
					
					//return;
					
                }              
                else if (hoverednew && buttonn == "Print Mode")
                {
					
						
					
						if (pPrintNumberDisplayMode == "Bid / Ask")
						{
							pPrintNumberDisplayMode = "Volume";
						}
						else if (pPrintNumberDisplayMode == "Volume")
						{
							pPrintNumberDisplayMode = "Delta";
						}
						else if (pPrintNumberDisplayMode == "Delta")
						{
							pPrintNumberDisplayMode = "Bid / Ask";
						}					
						
						
//                    if (pPrintEnabled2)
//                    {
//                        pPrintEnabled2 = false;
//                    }
//                    else
//                    {
//                        pPrintEnabled2 = true;
//                    }
                    thisbutton.Value.Switch = true;
					thisbutton.Value.Name = pPrintNumberDisplayMode;	
						
					
					//ChartBarsSwitch(!pPrintEnabled);
					
                    this.ChartControl.InvalidateVisual();					
					
					
					
				}
  			
                else if (hoverednew && buttonn == "Block Trades")
                {
					
                    if (pShowBlocks)
                    {
                        pShowBlocks = false;
                    }
                    else
                    {
                        pShowBlocks = true;
                    }
                    thisbutton.Value.Switch = pShowBlocks;
                    this.ChartControl.InvalidateVisual();
				
				}					

				else if (hoverednew && buttonn == "POC")
                {
					
                    if (pBarCompositeEnabled)
                    {
                        pBarCompositeEnabled = false;
                    }
                    else
                    {
                        pBarCompositeEnabled = true;
                    }
                    thisbutton.Value.Switch = pBarCompositeEnabled;
                    this.ChartControl.InvalidateVisual();
				
				}		
				else if (hoverednew && buttonn == "Bar Profile")
                {
					
                    if (pBarCompositeEnabled)
                    {
                        pBarCompositeEnabled = false;
                    }
                    else
                    {
                        pBarCompositeEnabled = true;
                    }
                    thisbutton.Value.Switch = pBarCompositeEnabled;
                    this.ChartControl.InvalidateVisual();
				
				}	
						
              else if (hoverednew && buttonn == "Magnets")
                {
		
					
	                    if (pMAGEnabled)
	                    {
	                        pMAGEnabled = false;
	                    }
	                    else
	                    {
	                        pMAGEnabled = true;
	                    }
	                    thisbutton.Value.Switch = pMAGEnabled;
	                    this.ChartControl.InvalidateVisual();
						
					
				
				}					
              else if (hoverednew && buttonn == "Unfinished Auctions")
                {
					
                    if (pUFAEnabled)
                    {
                        pUFAEnabled = false;
                    }
                    else
                    {
                        pUFAEnabled = true;
                    }
                    thisbutton.Value.Switch = pUFAEnabled;
                    this.ChartControl.InvalidateVisual();
				
				}						
              else if (hoverednew && buttonn == "Imbalances")
                {
					
                    if (pShowImbalance)
                    {
                        pShowImbalance = false;
                    }
                    else
                    {
                        pShowImbalance = true;
                    }
                    thisbutton.Value.Switch = pShowImbalance;
                    this.ChartControl.InvalidateVisual();
				
				}						
              else if (hoverednew && buttonn == "Washout Signals")
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
                    this.ChartControl.InvalidateVisual();
				
				}									
              else if (hoverednew && buttonn == "Color Mode")
                {
					if (pPrintBarFillMode == "Delta")
					{
						pPrintBarFillMode = "Candlestick";
						
					}
					else if (pPrintBarFillMode == "Candlestick")
					{
						pPrintBarFillMode = "Delta";
						
					}					
                    
                    thisbutton.Value.Switch = true;
					thisbutton.Value.Name = pPrintBarFillMode;
                    this.ChartControl.InvalidateVisual();
				
				}	
              else if (hoverednew && buttonn == "Opacity Mode")
                {
					if (pPrintBarOpacityMode == "Volume")
					{
						pPrintBarOpacityMode = "None";
						
					}
//					else if (pPrintBarOpacityMode == "Delta")
//					{
//						pPrintBarOpacityMode = "None";
						
//					}					
 					else if (pPrintBarOpacityMode == "None")
					{
						pPrintBarOpacityMode = "Volume";
						
					}	
					
                    thisbutton.Value.Switch = pPrintBarOpacityMode == "None" ? false : true;
					thisbutton.Value.Name = pPrintBarOpacityMode;
                    this.ChartControl.InvalidateVisual();
				
				}					
              else if (hoverednew && buttonn == "Body Mode")
                {
					if (pBarBodyMode == "Left")
					{
						pBarBodyMode = "Middle";
						
					}
					else if (pBarBodyMode == "Middle")
					{
						pBarBodyMode = "Right";
						
					}	
					else if (pBarBodyMode == "Right")
					{
						pBarBodyMode = "None";
						
					}						
 					else if (pBarBodyMode == "None")
					{
						pBarBodyMode = "Left";
						
					}	
					
                    thisbutton.Value.Switch = pBarBodyMode == "None" ? false : true;
					thisbutton.Value.Name = pBarBodyMode;
                    this.ChartControl.InvalidateVisual();
				
				}					
              else if (hoverednew && buttonn == "Above Bar")
                {
					if (pAboveTotalMode == "Delta Percent")
					{
						pAboveTotalMode = "Delta Total";
						
					}
					else if (pAboveTotalMode == "Delta Total")
					{
						pAboveTotalMode = "None";
						
					}					
 					else if (pAboveTotalMode == "None")
					{
						pAboveTotalMode = "Delta Percent";
						
					}	
					
                    thisbutton.Value.Switch = pAboveTotalMode == "None" ? false : true;
					thisbutton.Value.Name = pAboveTotalMode;
                    this.ChartControl.InvalidateVisual();
				
				}					
             
				else if (hoverednew && buttonn == "Below Bar")
                {
					if (pBelowTotalMode == "Delta Percent")
					{
						pBelowTotalMode = "Delta Total";
						
					}
					else if (pBelowTotalMode == "Delta Total")
					{
						pBelowTotalMode = "None";
						
					}					
 					else if (pBelowTotalMode == "None")
					{
						pBelowTotalMode = "Delta Percent";
						
					}	
					
                    thisbutton.Value.Switch = pBelowTotalMode == "None" ? false : true;
					thisbutton.Value.Name = pBelowTotalMode;
                    this.ChartControl.InvalidateVisual();
				
				}	
				
				
				
				
			}
	
			
			
			
			
			bool SomethingClicked = false;
			
 
			foreach (KeyValuePair<double, ButtonZ> thisbutton in AllButtonZ3)
            {
                bool hoverednew = MouseIn(thisbutton.Value.Rect, MenuButtonExpandP, MenuButtonExpandP);
                string buttonn = thisbutton.Value.Text;



                if (hoverednew && buttonn == "Y Scroll")
                {
					
					
                    if (pUseYScroll)
                    {
                        pUseYScroll = false;
						TicksAdjust = 0;
                    }
                    else
                    {
                        pUseYScroll = true;
						TicksAdjust = 0;
                    }
                    thisbutton.Value.Switch = pUseYScroll;
					
					//ChartBarsSwitch(!pPrintEnabled);
					
					//ForceRefresh();
					
					ChartControl.InvalidateVisual();
       

					//SomethingClicked = true;
					
				
				}                  
               else if (hoverednew && buttonn == "Y Scale")
                {
					
					
                    if (pUseFixedVerticalScale)
                    {
                        pUseFixedVerticalScale = false;
                    }
                    else
                    {
                        pUseFixedVerticalScale = true;
                    }
                    thisbutton.Value.Switch = pUseFixedVerticalScale;
					
					//ChartBarsSwitch(!pPrintEnabled);
					
					ForceRefresh();
					
       				ChartControl.InvalidateVisual();

					//SomethingClicked = true;
					
				
				}
                else if (hoverednew && buttonn == "Y Scale +")
                {

					//TicksAdjust = TicksAdjust + pScrollTicks;
			
					//ForceRefresh();
					ChartControl.InvalidateVisual();
				
					ChartControl.ChartPanels[0].InvalidateVisual(); 
		 
					pScaleTicks = pScaleTicks + 1;
					
						
          			 SetScaleData();	
			
										

                  // SomethingClicked = true;
									
				}               
                else  if (hoverednew && buttonn == "Y Scale -")
                {

					
 					//TicksAdjust = TicksAdjust - pScrollTicks;
			
					//ForceRefresh();
					ChartControl.InvalidateVisual();
				
					ChartControl.ChartPanels[0].InvalidateVisual(); 
					
					
					pScaleTicks = pScaleTicks - 1;
					

					
          			 SetScaleData();	
			
				
					
                  // SomethingClicked = true;
									
				}                  
               else if (hoverednew && buttonn == "Zoom")
                {
					
					
                    if (pZoomEnabled)
                    {
                        pZoomEnabled = false;
                    }
                    else
                    {
                        pZoomEnabled = true;
                    }
					
					
					
					
				//	pUseFixedVerticalScale = !pZoomEnabled;
					
                    thisbutton.Value.Switch = pZoomEnabled;
					
					//ChartBarsSwitch(!pPrintEnabled);
					
					
					
					if (!pZoomEnabled)
						SetBarSize(pCurrentSetting, pSpaceBetweenBars);
					else
						SetBarSize(pCurrentSetting2, pSpaceBetweenBars2);
					
					
					
                 //   SomethingClicked = true;
				
				}				
				

                
               else if (hoverednew && buttonn == "X Size -")
                {

					if (!pZoomEnabled)
						pCurrentSetting = Math.Max(8, pCurrentSetting - 2);
					else
						pCurrentSetting2 = Math.Max(8, pCurrentSetting2 - 2);

					if (!pZoomEnabled)
						SetBarSize(pCurrentSetting, pSpaceBetweenBars);
					else
						SetBarSize(pCurrentSetting2, pSpaceBetweenBars2);


                }
               else if (hoverednew && buttonn == "X Size +")
                {

					if (!pZoomEnabled)
						pCurrentSetting = Math.Max(8, pCurrentSetting + 2);
					else
						pCurrentSetting2 = Math.Max(8, pCurrentSetting2 + 2);

					if (!pZoomEnabled)
						SetBarSize(pCurrentSetting, pSpaceBetweenBars);
					else
						SetBarSize(pCurrentSetting2, pSpaceBetweenBars2);

                }

               else if (hoverednew && buttonn == "X Space -")
                {

					if (!pZoomEnabled)
						pSpaceBetweenBars = Math.Max(1, pSpaceBetweenBars - 1);
					else
						pSpaceBetweenBars2 = Math.Max(1, pSpaceBetweenBars2 - 1);

					if (!pZoomEnabled)
						SetBarSize(pCurrentSetting, pSpaceBetweenBars);
					else
						SetBarSize(pCurrentSetting2, pSpaceBetweenBars2);

                }
               else if (hoverednew && buttonn == "X Space +")
                {

					if (!pZoomEnabled)
						pSpaceBetweenBars = Math.Max(1, pSpaceBetweenBars + 1);
					else
						pSpaceBetweenBars2 = Math.Max(1, pSpaceBetweenBars2 + 1);

					if (!pZoomEnabled)
						SetBarSize(pCurrentSetting, pSpaceBetweenBars);
					else
						SetBarSize(pCurrentSetting2, pSpaceBetweenBars2);

                }
				

				
			}
			
//			if (SomethingClicked)
//			{
				
				
//					ChartControl.Dispatcher.InvokeAsync(() =>
//					{
						
//						if (pZoomEnabled)
//							SetBarSize(pCurrentSetting, pSpaceBetweenBars+2);
//						else
//							SetBarSize(pCurrentSetting2, pSpaceBetweenBars2+2);
//						ChartControl.InvalidateVisual();	
					
					
//						if (pZoomEnabled)
//						{
//							if(ChartControl.Properties.BarDistance != (int) (pCurrentSetting*1) + 1 + pSpaceBetweenBars)
//							{
//								pCurrentSetting = Math.Max(2,(int) ChartBars.Properties.ChartStyle.BarWidth*2);
//								ChartControl.Properties.BarDistance = (int) (pCurrentSetting*1) + 1 + pSpaceBetweenBars;
								
//								//SetBarSize(pCurrentSetting, pSpaceBetweenBars);
//							}						
							
//						}
//						else
//						{
//							if(ChartControl.Properties.BarDistance != (int) (pCurrentSetting2*1) + 1 + pSpaceBetweenBars2)
//							{
//								pCurrentSetting2 = Math.Max(2,(int) ChartBars.Properties.ChartStyle.BarWidth*2);
//								ChartControl.Properties.BarDistance = (int) (pCurrentSetting2*1) + 1 + pSpaceBetweenBars2;
								
//								//SetBarSize(pCurrentSetting, pSpaceBetweenBars);
//							}								
							
							
//						}	
						
						
							
//						this.ChartControl.InvalidateVisual();
//						this.ChartControl.InvalidateVisual();
				
//					});		
				
				
//			}
			
						
			
			// mouse down vertical lines
			
				if (IsHoverMD && IsHoverRM)
					IsHoverMD = false;
				
				
				
				if (IsMoveComposite)
				{
					IsMoveComposite = false;
					

				}
				else if (IsHoverComposite)
				{
					IsMoveComposite = true;
					StartCompositeX = (int) MoveComposite.Left;
					StartCompositeLength = pCompLength;
				}						
									
				
				
				
						
				if (IsMoveMD)
				{
					IsMoveMD = false;
					

				}
				else if (IsHoverMD)
				{
					IsMoveMD = true;
					StartMDX = (int) MoveMD.Left;
					StartMDLength = pInvLength;
				}						
						
				
				
						
				if (IsMoveRM)
				{
					IsMoveRM = false;
					

				}
				else if (IsHoverRM)
				{
					IsMoveRM = true;
					StartRMX = (int) MoveRM.Left;
					StartRMLength = pThisBarMarginRight;
				}						
						
				
				

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
         
			// EXECUTION BUTTONS
			
				if (BuyClickReady)				
				{
				
					if (CurrentMousePrice <= CurrentAsk)
					{
						ClickText = "LMT";
						if (pUseMIT)
							ClickText = "MIT";
  
					}       
					else
					{
						ClickText = "STP";
						if (pUseSLM)
							ClickText = "SLM";
					}					
					
					
					ChartControl.InvalidateVisual();
					
					return;
				}
				
				if (SellClickReady)
					
				{
					
						if (CurrentMousePrice >= CurrentBid)
						{
							ClickText = "LMT";
							if (pUseMIT)
								ClickText = "MIT";
						}       
						else
						{
							ClickText = "STP";
							if (pUseSLM)
								ClickText = "SLM";
						}						
					
						ChartControl.InvalidateVisual();
						
						return;
					
				}
				
				
		
				
				if (MovingOrder != null || MovingOrders.Count > 0)
				{
						ChartControl.InvalidateVisual();
					
				}
				
			
			
			// BLOCK TRADES HOVER
			//Print(BlockTradeButtons.Count);
			
			
			if (pShowBlocks && pShowBlocksHover)
			{
			
				InBlockNow = false;	
				foreach (SharpDX.RectangleF R in BlockTradeButtons)
				{
					if (MouseIn(R,0,0))
					{
						InBlockNow = true;	
					}
					
				}
				
				//Print(InBlockNow);
				
				if (InBlockNow != InBlockNowP)
				{
					//Print("Refresh Block");
					this.ChartControl.InvalidateVisual();
				}
				
				InBlockNowP = InBlockNow;
				
			}
			
			
			// END BLOCKS
			
			
			// hover updates for buttons
			
			
			//currentbuttonhover5 = -10;	
			
			bool OneIsH = false;
            foreach (KeyValuePair<double, ButtonZ> thisbutton in AllButtonZ5)
                {
                    bool hoverednew = MouseIn(thisbutton.Value.Rect, MenuButtonExpandP, MenuButtonExpandP);
                    bool hoverednow = thisbutton.Value.Hovered;


					
                    if (hoverednew && !hoverednow)
                    {
                        thisbutton.Value.Hovered = true;
                        this.ChartControl.InvalidateVisual();
                    }
                    else if (hoverednow)
                    {
                        thisbutton.Value.Hovered = false;
                        this.ChartControl.InvalidateVisual();
                    }
					
					
					hoverednow = thisbutton.Value.Hovered;
					if (hoverednow)
					{
						currentbuttonhover5 = thisbutton.Value.Width;
						OneIsH = true;
					}
					
//					if (hoverednow)
//						currentbuttonhover5 = thisbutton.Value.Width;
                }    
				
 			//if (!OneIsH)
			//	currentbuttonhover5 = -10;	
			
				
			//currentbuttonhover5 = -10;		
            foreach (KeyValuePair<double, ButtonZ> thisbutton in AllButtonZ3)
                {
                    bool hoverednew = MouseIn(thisbutton.Value.Rect, MenuButtonExpandP, MenuButtonExpandP);
                    bool hoverednow = thisbutton.Value.Hovered;

                    if (hoverednew && !hoverednow)
                    {
                        thisbutton.Value.Hovered = true;
						//currentbuttonhover3 = thisbutton.Value.Width;
                        this.ChartControl.InvalidateVisual();
                    }         
					else if (hoverednow)
                    {
                        thisbutton.Value.Hovered = false;
                        this.ChartControl.InvalidateVisual();
                    }
					
					
					hoverednow = thisbutton.Value.Hovered;
					if (hoverednow)
					{
						currentbuttonhover3 = thisbutton.Value.Width;
						OneIsH = true;
					}
					
                }  			
 				
				
				
            foreach (KeyValuePair<double, ButtonZ> thisbutton in AllButtonZ4)
                {
                    bool hoverednew = MouseIn(thisbutton.Value.Rect, MenuButtonExpandP, MenuButtonExpandP);
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
				
				
            foreach (KeyValuePair<double, ButtonZ> thisbutton in AllButtonZ2)
                {
                    bool hoverednew = MouseIn(thisbutton.Value.Rect, MenuButtonExpandP, MenuButtonExpandP);
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

            foreach (KeyValuePair<double, ButtonZ> thisbutton in AllButtonZ1)
                {
                    bool hoverednew = MouseIn(thisbutton.Value.Rect, MenuButtonExpandP, MenuButtonExpandP);
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

				// show menus
				
                InMenuP = InMenu;
                InMenu = MouseIn(B2, 0, 0);
            
				if (InMenu != InMenuP)
                	ChartControl.InvalidateVisual();				
				
				
				InMenu2P = InMenu2;
                InMenu2 = MouseIn(B22, 0, 0);

            	if (InMenu2 != InMenu2P)
                	ChartControl.InvalidateVisual();				
				
				
				InMenu3P = InMenu3;

				if (!InMenu3)
					InMenu3 = MouseIn(B232, 0, 0);
				else
               		InMenu3 = MouseIn(B23, 0, 0);

				SetPanelMenuOpen(InMenu3);

				bool headerNowHovered = MouseIn(headerRect, 3, 3);
            	if (InMenu3 != InMenu3P || headerNowHovered || IsAnyPanelMenuOpen() != InMenu3P)
                	ChartControl.InvalidateVisual();

				// Drag-to-reorder tracking
				if (draggingPanelId != null && e.LeftButton == MouseButtonState.Released)
				{
					if (Math.Abs(dragOffsetY) < 5f)
						SetPanelExpanded(!panelExpanded);
					draggingPanelId = null;
					dragOffsetY = 0;
					if (ChartPanel != null) ChartPanel.ReleaseMouseCapture();
					ChartControl.InvalidateVisual();
				}
				else if (draggingPanelId != null)
				{
					dragOffsetY = (float)FinalYPixel - dragStartY;
					e.Handled = true;
					ChartControl.InvalidateVisual();

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



				
				
					//SetBarSize(pCurrentSetting, pSpaceBetweenBars+2);
					//ChartControl.InvalidateVisual();				
				
				
				// hover and move vertical lines
				
				// COMPOSITE
				
				IsHoverComposite = MouseIn(MoveComposite,pMoveVerticalLinePadding,0);
				
				if (IsMoveComposite || IsHoverComposite != IsHoverCompositeP)
				{
					if (IsMoveComposite)
					{
						

						
						int moveamount = StartCompositeX - (int) (Math.Max(ChartPanel.W/2, FinalXPixel));
						
						if (pCompositeLocation == "Left")
						{
							moveamount = StartCompositeX - (int) (Math.Min(ChartPanel.W/2, FinalXPixel));
							moveamount = moveamount * -1;
						}
						
						pCompLength = StartCompositeLength + moveamount; 
						pCompLength = Math.Max(pCompLength,100);
					}
					
					
					this.ChartControl.InvalidateVisual();	
					
				}
				
				IsHoverCompositeP = IsHoverComposite;	
				
				
				
				// MARKET DEPTH
				
				IsHoverMD = MouseIn(MoveMD,pMoveVerticalLinePadding,0);
				
				if (IsMoveMD || IsHoverMD != IsHoverMDP)
				{
					if (IsMoveMD)
					{
						

						
						int moveamount = StartMDX - (int) (Math.Max(ChartPanel.W/2, FinalXPixel));
						
						pInvLength = StartMDLength + moveamount; 
						pInvLength = Math.Max(pInvLength,50);
					}
					
					
					this.ChartControl.InvalidateVisual();	
					
				}
				
				IsHoverMDP = IsHoverMD;	
				
				
				// RIGHT MARGIN
				
				IsHoverRM = MouseIn(MoveRM,pMoveVerticalLinePadding,0);
				
				if (IsMoveRM || IsHoverRM != IsHoverRMP)
				{
					if (IsMoveRM)
					{
						

						
						int moveamount = StartRMX - (int) (Math.Max(ChartPanel.W/2, FinalXPixel));
						
						pThisBarMarginRight = StartRMLength + moveamount; 
						pThisBarMarginRight = Math.Max(pThisBarMarginRight,0);
					}
					
					
					this.ChartControl.InvalidateVisual();	
					
				}
				
				IsHoverRMP = IsHoverRM;	
				
				
				
				InMenu4P = InMenu4;
                InMenu4 = MouseIn(B44, 0, 0) || IsHoverComposite || IsMoveComposite;

            	if (InMenu4 != InMenu4P)
				{
                	ChartControl.InvalidateVisual();	
				}				
				
	
				
			if (pCrossHairEnabled || IsDragging || BuyClickReady || SellClickReady)// && ThisMousePrice != ThisMousePriceP)
			{
				
				this.ChartControl.InvalidateVisual();
			}
			
			//e.Handled = true;
		}

		private void calculateProfile(int ind)
		{
			//int ind  = (hist) ? 1 : 0;
			
			if(Profiles[ind] == null)
			{
				Profiles[ind] = new Profile();
			}
			
			double thismaxav = 0;
			double thismaxbv = 0;
			double thismaxtv = 0;
			double thismaxdv = 0;
			
			double thisav = 0;
			double thisbv = 0;
			double thistv = 0;
			double thisdv = 0;
			
			Profiles[ind].tv = 0.0;
			Profiles[ind].av = 0.0;
			Profiles[ind].bv = 0.0;
			Profiles[ind].dv = 0.0;
			
			Profiles[ind].l.Clear();
			
			bool DoReset = Bars.IsFirstBarOfSessionByIndex(CurrentBar - ind);
			
			
			if (pCompositeResetMode == "Time Of Day")
			{

				DoReset = false;
				
				if (R1Time == DateTime.MinValue)
				{
					R1Time = new DateTime(Times[0][0].Year,Times[0][0].Month,Times[0][0].Day,pResetTime1.Hours,pResetTime1.Minutes,0);
					//R1Time = R1Time.AddMinutes(pResetMinutes);
					
				}
//				if (R2Time == DateTime.MinValue)
//				{
//					R2Time = new DateTime(Times[0][0].Year,Times[0][0].Month,Times[0][0].Day,pResetTime2.Hours,pResetTime2.Minutes,0);
//					R2Time = R2Time.AddMinutes(pResetMinutes);
					
//				}							

				
				DateTime CurrentTime = Times[0][0];
				DateTime LastBarTime = Times[0][0];
				
				if (CurrentBars[0] > 0)
				LastBarTime = Times[0][1];
				
				
				if (ind == 1) // real time
				{
					while (CurrentTime.Ticks >= R1Time.Ticks)
					{

						//R1Time = R1Time.AddMinutes(pResetMinutes);
						
						R1Time = R1Time.AddDays(1);
						
						DoReset = true;
						
					}					
					
				}
				else // historical
				{
					
					
					
					while (CurrentTime.Ticks > R1Time.Ticks)
					{

						R1Time = R1Time.AddDays(1);
						DoReset = true;
						
					}
				}
				
			}
	
		

			
			if(DoReset)
			{
				if(BarItems[ind] == null)   { return; }
				if(BarItems[ind].l.IsEmpty) { return; }
				
				Profiles[ind].tv += BarItems[ind].tv;
				Profiles[ind].av += BarItems[ind].av;
				Profiles[ind].bv += BarItems[ind].bv;
				
				foreach(KeyValuePair<double, RowData> rd in BarItems[ind].l)
				{
					if(!Profiles[ind].l.ContainsKey(rd.Key))
					{
						Profiles[ind].l.TryAdd(rd.Key, new RowData());
					}
					
					thistv = BarItems[ind].l[rd.Key].tv;
					thisav = BarItems[ind].l[rd.Key].av;
					thisbv = BarItems[ind].l[rd.Key].bv;
					thisdv = thisav - thisbv;
					
					Profiles[ind].l[rd.Key].tv += thistv;
					Profiles[ind].l[rd.Key].av += thisav;
					Profiles[ind].l[rd.Key].bv += thisbv;
					Profiles[ind].l[rd.Key].dv += thisdv;
				}
				
			}
			else
			{
				if(Profiles.IsValidDataPoint(ind + 1))
				{
					Profiles[ind].tv = Profiles[ind + 1].tv;
					Profiles[ind].av = Profiles[ind + 1].av;
					Profiles[ind].bv = Profiles[ind + 1].bv;
					Profiles[ind].dv = Profiles[ind + 1].dv;
					
					foreach(KeyValuePair<double, RowData> rd in Profiles[ind + 1].l)
					{
						if(!Profiles[ind].l.ContainsKey(rd.Key))
						{
							Profiles[ind].l.TryAdd(rd.Key, new RowData());
						}
						
						Profiles[ind].l[rd.Key].tv = Profiles[ind + 1].l[rd.Key].tv;
						Profiles[ind].l[rd.Key].av = Profiles[ind + 1].l[rd.Key].av;
						Profiles[ind].l[rd.Key].bv = Profiles[ind + 1].l[rd.Key].bv;
						Profiles[ind].l[rd.Key].dv = Profiles[ind + 1].l[rd.Key].dv;
					}
					
					if(BarItems[ind] == null)   { return; }
					if(BarItems[ind].l.IsEmpty) { return; }
					
					Profiles[ind].tv += BarItems[ind].tv;
					Profiles[ind].av += BarItems[ind].av;
					Profiles[ind].bv += BarItems[ind].bv;
					
					
					foreach(KeyValuePair<double, RowData> rd in BarItems[ind].l)
					{
						if(!Profiles[ind].l.ContainsKey(rd.Key))
						{
							Profiles[ind].l.TryAdd(rd.Key, new RowData());
						}
						
						thistv = BarItems[ind].l[rd.Key].tv;
						thisav = BarItems[ind].l[rd.Key].av;
						thisbv = BarItems[ind].l[rd.Key].bv;
						thisdv = thisav - thisbv;
						
						Profiles[ind].l[rd.Key].tv += thistv;
						Profiles[ind].l[rd.Key].av += thisav;
						Profiles[ind].l[rd.Key].bv += thisbv;
						Profiles[ind].l[rd.Key].dv += thisdv;
					}
					

					
				}
			}
			
			Profiles[ind].dv = Profiles[ind].av - Profiles[ind].bv;
			
			TotalVolume[ind] = Profiles[ind].tv;
			TotalDelta[ind] = Profiles[ind].dv; //Profiles[ind].v;
				        //BarVolume
				        //TotalDelta
				    //BarDelta
			
			thismaxav = 0;
			thismaxbv = 0;
			thismaxtv = 0;
			thismaxdv = 0;
			
			double thisprice = 0;
			double thispoc = 0;
			double thisvwap = 0;
			double thisvah1 = 0;
			double thisval1 = 0;
			double thisvah2 = 0;
			double thisval2 = 0;

			
			double VWAPTotal = 0;
			double AllVolume = Profiles[ind].tv;
			
			double lowestprice = 99999999;
			double highestprice = 0;
			
			foreach(KeyValuePair<double, RowData> rd in Profiles[ind].l)
			{
						thisprice = rd.Key;
						thistv = Profiles[ind].l[rd.Key].tv;
						thisav = Profiles[ind].l[rd.Key].av;
						thisbv = Profiles[ind].l[rd.Key].bv;
						thisdv = thisav - thisbv;
				
						VWAPTotal = VWAPTotal + thistv*thisprice;
						
						thismaxav = Math.Max(thisav, thismaxav);
						thismaxbv = Math.Max(thisbv, thismaxbv);
						thismaxdv = Math.Max(thisdv, thismaxdv);
				
						if (thistv > thismaxtv)
						{
							thispoc = thisprice;
							thismaxtv = Math.Max(thistv, thismaxtv);
						}
						
						lowestprice = Math.Min(lowestprice, thisprice);
						highestprice = Math.Max(highestprice, thisprice);
			}
				
			thisvwap = VWAPTotal / AllVolume;
			thisvwap = RTTS(thisvwap); // 5/10/2018

			double PercentVolume = AllVolume * pVA1Percent / 100;
			
			double CurrentVolume = thismaxtv;
			double PriceAbove = thisvwap;
			double PriceBelow = thisvwap;
			
			if (pVA1Basis == "POC")
			{
				PriceAbove = thispoc;
				PriceBelow = thispoc;				
			}
			
			double VolumeAbove = 0;
			double VolumeBelow = 0;			
			double ThisPrice = 0;

			int iii = 1;
			
			do 
			{
				
				VolumeAbove = 0;
				VolumeBelow = 0;
				
				ThisPrice = RTTS(PriceAbove + iii*ThisTickSizze);		
				if (Profiles[ind].l.ContainsKey(ThisPrice))
					VolumeAbove = VolumeAbove + Profiles[ind].l[ThisPrice].tv;
				
				ThisPrice = RTTS(PriceBelow - iii*ThisTickSizze);		
				if (Profiles[ind].l.ContainsKey(ThisPrice))
					VolumeBelow = VolumeBelow + Profiles[ind].l[ThisPrice].tv;
				
				PriceBelow = RTTS(PriceBelow - iii*ThisTickSizze);
				CurrentVolume = CurrentVolume + VolumeBelow;
				PriceAbove = RTTS(PriceAbove + iii*ThisTickSizze);
				CurrentVolume = CurrentVolume + VolumeAbove;	
		
				
			}
			while (CurrentVolume < PercentVolume);

			thisvah1 = Math.Min(PriceAbove,highestprice);
			thisval1 = Math.Max(PriceBelow,lowestprice);			
			
			
			// second set of value area
			
			PercentVolume = AllVolume * pVA2Percent / 100;
			
			CurrentVolume = thismaxtv;
			PriceAbove = thisvwap; // 5/10/2018
			PriceBelow = thisvwap; // 5/10/2018
			
			if (pVA2Basis == "POC")
			{
				PriceAbove = thispoc;
				PriceBelow = thispoc;				
			}
			
			VolumeAbove = 0;
			VolumeBelow = 0;			
			ThisPrice = 0;
			
			//Print( " -------------------------------");
			
			do 
			{
				
				VolumeAbove = 0;
				VolumeBelow = 0;
				
				ThisPrice = RTTS(PriceAbove + iii*ThisTickSizze);		
				if (Profiles[ind].l.ContainsKey(ThisPrice))
					VolumeAbove = VolumeAbove + Profiles[ind].l[ThisPrice].tv;

				ThisPrice = RTTS(PriceBelow - iii*ThisTickSizze);		
				if (Profiles[ind].l.ContainsKey(ThisPrice))
					VolumeBelow = VolumeBelow + Profiles[ind].l[ThisPrice].tv;

				PriceBelow = RTTS(PriceBelow - iii*ThisTickSizze);
				CurrentVolume = CurrentVolume + VolumeBelow;
				PriceAbove = RTTS(PriceAbove + iii*ThisTickSizze);
				CurrentVolume = CurrentVolume + VolumeAbove;	
				
				
			}
			while (CurrentVolume < PercentVolume);
			
			//Print(AllVolume + "   " + CurrentVolume + " " + PercentVolume);
			
			thisvah2 = Math.Min(PriceAbove,highestprice);
			thisval2 = Math.Max(PriceBelow,lowestprice);			
						
			
			Profiles[ind].maxav = thismaxav;
			Profiles[ind].maxbv = thismaxbv;
			Profiles[ind].maxtv = thismaxtv;
			Profiles[ind].maxdv = thismaxdv;
			CompPOC[ind] = thispoc;
			CompVWAP[ind] = thisvwap;
			CompVAH1[ind] = thisvah1;
			CompVAL1[ind] = thisval1;
			CompVAH2[ind] = thisvah2;
			CompVAL2[ind] = thisval2;
						
			CompDH[ind] = highestprice;
			CompDL[ind] = lowestprice;		
			
//			PlotBrushes[2][ind] = pPlot1Brush;
//			PlotBrushes[3][ind] = pPlot2Brush;
//			PlotBrushes[4][ind] = pPlot3Brush;
//			PlotBrushes[5][ind] = pPlot4Brush;
//			PlotBrushes[6][ind] = pPlot5Brush;
//			PlotBrushes[7][ind] = pPlot6Brush;
			
		}
				
		private void AddError(string eee)
		{
		
			if (!AllErrorMessages.Contains(eee))
				AllErrorMessages.Add(eee);
			
		}
		
		private void calculateBar(int ii)
		{
			
			if (BarItems[ii] != null)
				{
					//Print(CurrentBars[0] + "--------");
					
					
					
    				BarVolume[ii] = BarItems[ii].tv;

					//if (BarVolume[ii] == 0)
					//	AddError("You have missing tick data.  Please connect to a data feed.");
					
					
					BarBid[ii] = BarItems[ii].bv;
					BarAsk[ii] = BarItems[ii].av;
					BarDelta[ii] = BarItems[ii].dv;
					
					BarBidPercent[ii] = BarBid[ii] / BarVolume[ii] * 100;
					BarAskPercent[ii] = BarAsk[ii] / BarVolume[ii] * 100;
					BarDeltaPercent[ii] = BarAskPercent[ii] - BarBidPercent[ii];
					
					BarItems[ii].AskI = new List<double>();
					BarItems[ii].BidI = new List<double>();
						

					SD.Clear();
					BarItems[ii].UnfinishedAuctions.Clear();
					BarItems[ii].Magnets.Clear();
					
					double deltamax = -99999;
					double deltamin = 99999;
					
				
					foreach (KeyValuePair<double, RowData> pair in BarItems[ii].l) //.OrderBy(key=> key.))
					{
						
						
						SD.Add(pair.Key,pair.Value);
						
						
						double price = pair.Key;
						
						bool IsTopPrice = price == Highs[0][ii];
						bool IsBottomPrice = price == Lows[0][ii];
							
						//Print(price + " " + BarItems[ii].l[price].bv + " " + BarItems[ii].l[price].av + " ");
						
						double bidv = BarItems[ii].l[price].bv;
						double askv = BarItems[ii].l[price].av;
						double deltav = askv - bidv;
						
						deltamax = Math.Max(deltamax, deltav);
						deltamin = Math.Min(deltamin, deltav);
						
						// bid / ask imbalances
						
						double askratio = getAskImbalanceRatio(BarItems[ii].l,price);
						double bidratio = getBidImbalanceRatio(BarItems[ii].l,price);
						
						
						//Print(askratio);
						double bidv2 = 0;
						double askv2 = 0;
						
						RowData rowBelow, rowAbove;
						if (BarItems[ii].l.TryGetValue(RTTS(price - TickSize), out rowBelow))
							bidv2 = rowBelow.bv;
						if (BarItems[ii].l.TryGetValue(RTTS(price + TickSize), out rowAbove))
							askv2 = rowAbove.av;	
						

						
						double AskS = Math.Max(askv2,1);
						double BidS = Math.Max(bidv2,1);
						

						double AskMin = (double)BarItems[ii].av/(double)BarItems[ii].l.Count/(double)100*pVolumeQualifier;
						double BidMin = (double)BarItems[ii].bv/(double)BarItems[ii].l.Count/(double)100*pVolumeQualifier;
						
						AskMin = 0;
						BidMin = 0;
						
//						if (CurrentBars[0] == 320)
//						{
//							Print(price + "  " + BidS + "  " + BidMin);
//						}
						
						if (askratio >= pImbalanceOffset && BidS >= BidMin)
						{
							BarItems[ii].AskI.Add(price);
						}
						if (bidratio >= pImbalanceOffset && AskS >= AskMin)
						{
							BarItems[ii].BidI.Add(price);
						}
						
						// magnets
						
						//BarItems[ii].Magnets.Clear();
						if (Math.Abs(bidv - askv) <= pMAGSpreadMax && askv >= pMAGSizeMin && bidv >= pMAGSizeMin)
						{
							Level LLL = new Level();
							LLL.Price = price;
							LLL.EndBar = 0;
							
							BarItems[ii].Magnets.Add(LLL);
							ActiveMagnets.Add(LLL);
						}



						// unfinished auctions

						//BarItems[ii].UnfinishedAuctions.Clear();
						if (IsTopPrice || IsBottomPrice)
						{
							if (bidv != 0 && askv != 0)
							{
								Level LLL = new Level();
								LLL.Price = price;
								LLL.EndBar = 0;

								BarItems[ii].UnfinishedAuctions.Add(LLL);
								ActiveUFAs.Add(LLL);	
								
							}
							
						}
						
						
						
					}
					
					BarItems[ii].dvmax = deltamax;
					BarItems[ii].dvmin = deltamin;
					
					
					// loop through prices in order
					
					bool IsWashLong = true;
					bool IsWashShort = true;
					
					bool topdone = false;
					
					int totalbidlevels = 1;
					int totalasklevels = 1;
					double POC = 0;
					int start = 0;
					double pavol = 0;
					double pbvol = 0;
					double maxvol = 0;
					double ThisPOC = 0;
					double thisvwap = 0;
					
//					double thisvah1 = 0;
//					double thisval1 = 0;
//					double thisvah2 = 0;
//					double thisval2 = 0;
			
					double VWAPTotal = 0;
					//double AllVolume = Profiles[ind].tv;
					
					//double lowestprice = 99999999;
					//double highestprice = 0;
					
					double clustertop = 0;
					double clusterbottom = 0;
					double currentcluster = 0;
					double largestcluster = 0;
					
					Queue<double> lastHighCache = new Queue<double>(pClusterSize + 1);

					foreach (KeyValuePair<double, RowData> pair in SD)
					{
						double price = pair.Key;
						double avol = pair.Value.av;
						double bvol = pair.Value.bv;
						double tvol = pair.Value.tv;

						lastHighCache.Enqueue(tvol);
						if (lastHighCache.Count > pClusterSize)
							lastHighCache.Dequeue();

						currentcluster = 0;

						if (lastHighCache.Count == pClusterSize)
						{
							foreach (double v in lastHighCache)
								currentcluster += v;
							
							if (currentcluster > largestcluster)
							{
								largestcluster = currentcluster;
								clustertop = price;
								clusterbottom = clustertop - (pClusterSize-1)*TickSize;
								
							}		
							
							
						}
						

						
						VWAPTotal = VWAPTotal + tvol*price;
						
						if (tvol > maxvol)
						{
							maxvol = tvol;
							ThisPOC = price;
						}
							
						
						if (start != 0)
						{
							if (bvol > pbvol && !topdone)
							{
								totalbidlevels = totalbidlevels + 1;
							}
							else
							{
								topdone = true;
							}
							
							if (avol < pavol)
							{
								totalasklevels = totalasklevels + 1;
							}
							else
							{
								totalasklevels = 1;
							}
							
						}
						
						
						//bool IsTopPrice = price == Highs[0][0];
						//bool IsBottomPrice = price == Lows[0][0];
							
						//Print(price + " " + pair.Value.bv + " " + pair.Value.av + " ");
						
						// looping bottom of bar to the top 
						
						IsWashLong = true;
						
						pavol = avol;
						pbvol = bvol;
						start = 1;
					}
					
					thisvwap = VWAPTotal / BarItems[ii].tv;
					
					BarItems[ii].POC = ThisPOC;
					BarItems[ii].VWAP = thisvwap;
					
					BarItems[ii].ClusterTop = clustertop;
					BarItems[ii].ClusterBottom = clusterbottom;					
								
					LongSignals[ii] = 0;
					ShortSignals[ii] = 0;
					
					bool ShortSignalOK = false;
		
					if (pHighLowRangeFilter)
					{
								
						//if (Highs[0].IsValidPlot(CurrentBars[0]-1))
						if (Highs[0][ii] > Highs[0][ii+1] && (!pHighLowRangeFilter2 || Closes[0][ii] <= Opens[0][ii]))		
							ShortSignalOK = true;
					}
					else
					{
						if (!pHighLowRangeFilter2 || Closes[0][ii] <= Opens[0][ii])	
						ShortSignalOK = true;
						
					}
		
					bool LongSignalOK = false;
					
					if (pHighLowRangeFilter)
					{
								
						//if (Lows[0].IsValidPlot(CurrentBars[0]-1))
						if (Lows[0][ii] < Lows[0][ii+1] && (!pHighLowRangeFilter2 || Closes[0][ii] >= Opens[0][ii]))				
						LongSignalOK = true;
					}
					else
					{
						if (!pHighLowRangeFilter2 || Closes[0][0] >= Opens[0][0])
						LongSignalOK = true;
						
					}
		
		
					if (totalbidlevels >= pMinimumDecliningLevels && LongSignalOK)
						LongSignals[ii] = totalbidlevels;
					
					if (totalasklevels >= pMinimumDecliningLevels && ShortSignalOK)
						ShortSignals[ii] = totalasklevels;
					
					
					
					// add Zones (Imbalance)
					
					
					//Print("-------------");
					// new ZoneItem() already initializes ResistanceZones/SupportZones — no need to re-allocate
					ZoneItems[ii] = new ZoneItem();
		
					double bottomn = 0;
					int csize = 0;
					Zone Z;		
					
					// support / ask — Sort in-place (avoids LINQ OrderBy allocation)
					BarItems[ii].BidI.Sort();
					foreach(double d in BarItems[ii].BidI)
					{
						// going bottom to top
						//Print(i);
						
						// logic to qualify a zone
						
						if (d <= Highs[0][ii])
						{
							if (bottomn == 0)
							{
								bottomn = d;
								csize++;
							}
							else
							{
								if (RTTS(bottomn + TickSize*csize) == d)
								{
									csize++;
								}
								else
								{
									if (csize >= pMinZWidth)
									{
										Z = new Zone();
										Z.BottomPrice = bottomn;
										Z.TopPrice = bottomn + (csize-1)*TickSize;
										Z.TicksWidth = csize;
										Z.TestedPrice = 0;
										Z.IsBroken = false;
										Z.IsHidden = false;
										Z.EndBar = 0;
										
										
										ZoneItems[ii].ResistanceZones.Add(Z);
										Z.StartBar = CurrentBars[0] - ii; ActiveResistanceZones.Add(Z);
										//Print(bottomn + "  " + csize);
									}
									
									csize = 1;
									bottomn = d;
									
								}
							}
						}
					}
					
					if (csize >= pMinZWidth)
					{
						Z = new Zone();
						Z.BottomPrice = bottomn;
						Z.TopPrice = bottomn + (csize-1)*TickSize;
						Z.TicksWidth = csize;
						Z.TestedPrice = 0;
						Z.IsBroken = false;
						Z.IsHidden = false;
						Z.EndBar = 0;
						
						//SupplyZones.Add(CurrentBar,
						ZoneItems[ii].ResistanceZones.Add(Z);
						Z.StartBar = CurrentBars[0] - ii; ActiveResistanceZones.Add(Z);

					}
			
					csize = 0;

					BarItems[ii].AskI.Sort();
					foreach(double d in BarItems[ii].AskI)
					{
						if (d <= Highs[0][ii])
						{
							if (bottomn == 0)
							{
								bottomn = d;
								csize++;
							}
							else
							{
								if (RTTS(bottomn + TickSize*csize) == d)
								{
									csize++;
								}
								else
								{
									if (csize >= pMinZWidth)
									{
										Z = new Zone();
										Z.BottomPrice = bottomn;
										Z.TopPrice = bottomn + (csize-1)*TickSize;
										Z.TicksWidth = csize;
										Z.TestedPrice = 0;
										Z.IsBroken = false;
										Z.IsHidden = false;
										Z.EndBar = 0;
										
										//SupplyZones.Add(CurrentBar,
										ZoneItems[ii].SupportZones.Add(Z);
										Z.StartBar = CurrentBars[0] - ii; ActiveSupportZones.Add(Z);
										//Print(bottomn + "  " + csize);
									}
									
									csize = 1;
									bottomn = d;
									
								}

							}
						}
						
					}
					
					if (csize >= pMinZWidth)
					{
						Z = new Zone();
						Z.BottomPrice = bottomn;
						Z.TopPrice = bottomn + (csize-1)*TickSize;
						Z.TicksWidth = csize;
						Z.TestedPrice = 0;
						Z.IsBroken = false;
						Z.IsHidden = false;
						Z.EndBar = 0;
						
						//SupplyZones.Add(CurrentBar,
						ZoneItems[ii].SupportZones.Add(Z);
						Z.StartBar = CurrentBars[0] - ii; ActiveSupportZones.Add(Z);

					}
					
					
						
				}	
					
			
		}
		
//		private void calculateProfile(int ind)
//		{
//			//int ind  = (hist) ? 1 : 0;
			
//			if(Profiles[ind] == null)
//			{
//				Profiles[ind] = new Profile();
//			}
			
//			double thismaxav = 0;
//			double thismaxbv = 0;
//			double thismaxtv = 0;
//			double thismaxdv = 0;
			
//			double thisav = 0;
//			double thisbv = 0;
//			double thistv = 0;
//			double thisdv = 0;
			
//			Profiles[ind].tv = 0.0;
//			Profiles[ind].av = 0.0;
//			Profiles[ind].bv = 0.0;
//			Profiles[ind].dv = 0.0;
			
//			Profiles[ind].l.Clear();
			
//			bool DoReset = Bars.IsFirstBarOfSessionByIndex(CurrentBar - ind);
			
			
//			if (pCompositeResetMode == "Time Of Day")
//			{

//				DoReset = false;
				
//				if (R1Time == DateTime.MinValue)
//				{
//					R1Time = new DateTime(Times[0][0].Year,Times[0][0].Month,Times[0][0].Day,pResetTime1.Hours,pResetTime1.Minutes,0);
//					//R1Time = R1Time.AddMinutes(pResetMinutes);
					
//				}
////				if (R2Time == DateTime.MinValue)
////				{
////					R2Time = new DateTime(Times[0][0].Year,Times[0][0].Month,Times[0][0].Day,pResetTime2.Hours,pResetTime2.Minutes,0);
////					R2Time = R2Time.AddMinutes(pResetMinutes);
					
////				}							

				
//				DateTime CurrentTime = Times[0][0];
//				DateTime LastBarTime = Times[0][0];
				
//				if (CurrentBars[0] > 0)
//				LastBarTime = Times[0][1];
				
				
//				if (ind == 1) // real time
//				{
//					while (CurrentTime.Ticks >= R1Time.Ticks)
//					{

//						//R1Time = R1Time.AddMinutes(pResetMinutes);
						
//						R1Time = R1Time.AddDays(1);
						
//						DoReset = true;
						
//					}					
					
//				}
//				else // historical
//				{
					
					
					
//					while (CurrentTime.Ticks > R1Time.Ticks)
//					{

//						R1Time = R1Time.AddDays(1);
//						DoReset = true;
						
//					}
//				}
				
//			}
	
		

			
//			if(DoReset)
//			{
//				if(BarItems[ind] == null)   { return; }
//				if(BarItems[ind].l.IsEmpty) { return; }
				
//				Profiles[ind].tv += BarItems[ind].tv;
//				Profiles[ind].av += BarItems[ind].av;
//				Profiles[ind].bv += BarItems[ind].bv;
				
//				foreach(KeyValuePair<double, RowData> rd in BarItems[ind].l)
//				{
//					if(!Profiles[ind].l.ContainsKey(rd.Key))
//					{
//						Profiles[ind].l.TryAdd(rd.Key, new RowData());
//					}
					
//					thistv = BarItems[ind].l[rd.Key].tv;
//					thisav = BarItems[ind].l[rd.Key].av;
//					thisbv = BarItems[ind].l[rd.Key].bv;
//					thisdv = thisav - thisbv;
					
//					Profiles[ind].l[rd.Key].tv += thistv;
//					Profiles[ind].l[rd.Key].av += thisav;
//					Profiles[ind].l[rd.Key].bv += thisbv;
//					Profiles[ind].l[rd.Key].dv += thisdv;
//				}
				
//			}
//			else
//			{
//				if(Profiles.IsValidDataPoint(ind + 1))
//				{
//					Profiles[ind].tv = Profiles[ind + 1].tv;
//					Profiles[ind].av = Profiles[ind + 1].av;
//					Profiles[ind].bv = Profiles[ind + 1].bv;
//					Profiles[ind].dv = Profiles[ind + 1].dv;
					
//					foreach(KeyValuePair<double, RowData> rd in Profiles[ind + 1].l)
//					{
//						if(!Profiles[ind].l.ContainsKey(rd.Key))
//						{
//							Profiles[ind].l.TryAdd(rd.Key, new RowData());
//						}
						
//						Profiles[ind].l[rd.Key].tv = Profiles[ind + 1].l[rd.Key].tv;
//						Profiles[ind].l[rd.Key].av = Profiles[ind + 1].l[rd.Key].av;
//						Profiles[ind].l[rd.Key].bv = Profiles[ind + 1].l[rd.Key].bv;
//						Profiles[ind].l[rd.Key].dv = Profiles[ind + 1].l[rd.Key].dv;
//					}
					
//					if(BarItems[ind] == null)   { return; }
//					if(BarItems[ind].l.IsEmpty) { return; }
					
//					Profiles[ind].tv += BarItems[ind].tv;
//					Profiles[ind].av += BarItems[ind].av;
//					Profiles[ind].bv += BarItems[ind].bv;
					
					
//					foreach(KeyValuePair<double, RowData> rd in BarItems[ind].l)
//					{
//						if(!Profiles[ind].l.ContainsKey(rd.Key))
//						{
//							Profiles[ind].l.TryAdd(rd.Key, new RowData());
//						}
						
//						thistv = BarItems[ind].l[rd.Key].tv;
//						thisav = BarItems[ind].l[rd.Key].av;
//						thisbv = BarItems[ind].l[rd.Key].bv;
//						thisdv = thisav - thisbv;
						
//						Profiles[ind].l[rd.Key].tv += thistv;
//						Profiles[ind].l[rd.Key].av += thisav;
//						Profiles[ind].l[rd.Key].bv += thisbv;
//						Profiles[ind].l[rd.Key].dv += thisdv;
//					}
					

					
//				}
//			}
			
//			Profiles[ind].dv = Profiles[ind].av - Profiles[ind].bv;
			
//			TotalVolume[ind] = Profiles[ind].tv;
//			TotalDelta[ind] = Profiles[ind].dv; //Profiles[ind].v;
//				        //BarVolume
//				        //TotalDelta
//				    //BarDelta
			
//			thismaxav = 0;
//			thismaxbv = 0;
//			thismaxtv = 0;
//			thismaxdv = 0;
			
//			double thisprice = 0;
//			double thispoc = 0;
//			double thisvwap = 0;
//			double thisvah1 = 0;
//			double thisval1 = 0;
//			double thisvah2 = 0;
//			double thisval2 = 0;

			
//			double VWAPTotal = 0;
//			double AllVolume = Profiles[ind].tv;
			
//			double lowestprice = 99999999;
//			double highestprice = 0;
			
//			foreach(KeyValuePair<double, RowData> rd in Profiles[ind].l)
//			{
//						thisprice = rd.Key;
//						thistv = Profiles[ind].l[rd.Key].tv;
//						thisav = Profiles[ind].l[rd.Key].av;
//						thisbv = Profiles[ind].l[rd.Key].bv;
//						thisdv = thisav - thisbv;
				
//						VWAPTotal = VWAPTotal + thistv*thisprice;
						
//						thismaxav = Math.Max(thisav, thismaxav);
//						thismaxbv = Math.Max(thisbv, thismaxbv);
//						thismaxdv = Math.Max(thisdv, thismaxdv);
				
//						if (thistv > thismaxtv)
//						{
//							thispoc = thisprice;
//							thismaxtv = Math.Max(thistv, thismaxtv);
//						}
						
//						lowestprice = Math.Min(lowestprice, thisprice);
//						highestprice = Math.Max(highestprice, thisprice);
//			}
				
//			thisvwap = VWAPTotal / AllVolume;
//			thisvwap = RTTS(thisvwap); // 5/10/2018

//			double PercentVolume = AllVolume * pVA1Percent / 100;
			
//			double CurrentVolume = thismaxtv;
//			double PriceAbove = thisvwap;
//			double PriceBelow = thisvwap;
			
//			if (pVA1Basis == "POC")
//			{
//				PriceAbove = thispoc;
//				PriceBelow = thispoc;				
//			}
			
//			double VolumeAbove = 0;
//			double VolumeBelow = 0;			
//			double ThisPrice = 0;

//			int iii = 1;
			
//			do 
//			{
				
//				VolumeAbove = 0;
//				VolumeBelow = 0;
				
//				ThisPrice = RTTS(PriceAbove + iii*ThisTickSizze);		
//				if (Profiles[ind].l.ContainsKey(ThisPrice))
//					VolumeAbove = VolumeAbove + Profiles[ind].l[ThisPrice].tv;
				
//				ThisPrice = RTTS(PriceBelow - iii*ThisTickSizze);		
//				if (Profiles[ind].l.ContainsKey(ThisPrice))
//					VolumeBelow = VolumeBelow + Profiles[ind].l[ThisPrice].tv;
				
//				PriceBelow = RTTS(PriceBelow - iii*ThisTickSizze);
//				CurrentVolume = CurrentVolume + VolumeBelow;
//				PriceAbove = RTTS(PriceAbove + iii*ThisTickSizze);
//				CurrentVolume = CurrentVolume + VolumeAbove;	
		
				
//			}
//			while (CurrentVolume < PercentVolume);

//			thisvah1 = Math.Min(PriceAbove,highestprice);
//			thisval1 = Math.Max(PriceBelow,lowestprice);			
			
			
//			// second set of value area
			
//			PercentVolume = AllVolume * pVA2Percent / 100;
			
//			CurrentVolume = thismaxtv;
//			PriceAbove = thisvwap; // 5/10/2018
//			PriceBelow = thisvwap; // 5/10/2018
			
//			if (pVA2Basis == "POC")
//			{
//				PriceAbove = thispoc;
//				PriceBelow = thispoc;				
//			}
			
//			VolumeAbove = 0;
//			VolumeBelow = 0;			
//			ThisPrice = 0;
			
//			//Print( " -------------------------------");
			
//			do 
//			{
				
//				VolumeAbove = 0;
//				VolumeBelow = 0;
				
//				ThisPrice = RTTS(PriceAbove + iii*ThisTickSizze);		
//				if (Profiles[ind].l.ContainsKey(ThisPrice))
//					VolumeAbove = VolumeAbove + Profiles[ind].l[ThisPrice].tv;

//				ThisPrice = RTTS(PriceBelow - iii*ThisTickSizze);		
//				if (Profiles[ind].l.ContainsKey(ThisPrice))
//					VolumeBelow = VolumeBelow + Profiles[ind].l[ThisPrice].tv;

//				PriceBelow = RTTS(PriceBelow - iii*ThisTickSizze);
//				CurrentVolume = CurrentVolume + VolumeBelow;
//				PriceAbove = RTTS(PriceAbove + iii*ThisTickSizze);
//				CurrentVolume = CurrentVolume + VolumeAbove;	
				
				
//			}
//			while (CurrentVolume < PercentVolume);
			
//			//Print(AllVolume + "   " + CurrentVolume + " " + PercentVolume);
			
//			thisvah2 = Math.Min(PriceAbove,highestprice);
//			thisval2 = Math.Max(PriceBelow,lowestprice);			
						
			
//			Profiles[ind].maxav = thismaxav;
//			Profiles[ind].maxbv = thismaxbv;
//			Profiles[ind].maxtv = thismaxtv;
//			Profiles[ind].maxdv = thismaxdv;
//			CompPOC[ind] = thispoc;
//			CompVWAP[ind] = thisvwap;
//			CompVAH1[ind] = thisvah1;
//			CompVAL1[ind] = thisval1;
//			CompVAH2[ind] = thisvah2;
//			CompVAL2[ind] = thisval2;
						
//			CompDH[ind] = highestprice;
//			CompDL[ind] = lowestprice;		
			
////			PlotBrushes[2][ind] = pPlot1Brush;
////			PlotBrushes[3][ind] = pPlot2Brush;
////			PlotBrushes[4][ind] = pPlot3Brush;
////			PlotBrushes[5][ind] = pPlot4Brush;
////			PlotBrushes[6][ind] = pPlot5Brush;
////			PlotBrushes[7][ind] = pPlot6Brush;
			
//		}
				
//		private void AddError(string eee)
//		{
		
//			if (!AllErrorMessages.Contains(eee))
//				AllErrorMessages.Add(eee);
			
//		}
		
//		private void calculateBar(int ii)
//		{
			
//			if (BarItems[ii] != null)
//				{
//					//Print(CurrentBars[0] + "--------");
					
					
					
//    				BarVolume[ii] = BarItems[ii].tv;

//					//if (BarVolume[ii] == 0)
//					//	AddError("You have missing tick data.  Please connect to a data feed.");
					
					
//					BarBid[ii] = BarItems[ii].bv;
//					BarAsk[ii] = BarItems[ii].av;
//					BarDelta[ii] = BarItems[ii].dv;
					
//					BarBidPercent[ii] = BarBid[ii] / BarVolume[ii] * 100;
//					BarAskPercent[ii] = BarAsk[ii] / BarVolume[ii] * 100;
//					BarDeltaPercent[ii] = BarAskPercent[ii] - BarBidPercent[ii];
					
//					BarItems[ii].AskI = new List<double>();
//					BarItems[ii].BidI = new List<double>();
						

//					SD.Clear();
//					BarItems[ii].UnfinishedAuctions.Clear();
//					BarItems[ii].Magnets.Clear();
					
//					double deltamax = -99999;
//					double deltamin = 99999;
					
				
//					foreach (KeyValuePair<double, RowData> pair in BarItems[ii].l) //.OrderBy(key=> key.))
//					{
						
						
//						SD.Add(pair.Key,pair.Value);
						
						
//						double price = pair.Key;
						
//						bool IsTopPrice = price == Highs[0][ii];
//						bool IsBottomPrice = price == Lows[0][ii];
							
//						//Print(price + " " + BarItems[ii].l[price].bv + " " + BarItems[ii].l[price].av + " ");
						
//						double bidv = BarItems[ii].l[price].bv;
//						double askv = BarItems[ii].l[price].av;
//						double deltav = askv - bidv;
						
//						deltamax = Math.Max(deltamax, deltav);
//						deltamin = Math.Min(deltamin, deltav);
						
//						// bid / ask imbalances
						
//						double askratio = getAskImbalanceRatio(BarItems[ii].l,price);
//						double bidratio = getBidImbalanceRatio(BarItems[ii].l,price);
						
						
//						//Print(askratio);
//						double bidv2 = 0;
//						double askv2 = 0;
						
//						if (BarItems[ii].l.ContainsKey(RTTS(price-TickSize)))
//						{
//							bidv2 = BarItems[ii].l[RTTS(price-TickSize)].bv;
//						}
//						if (BarItems[ii].l.ContainsKey(RTTS(price+TickSize)))
//						{
//							askv2 = BarItems[ii].l[RTTS(price+TickSize)].av;
//						}	
						

						
//						double AskS = Math.Max(askv2,1);
//						double BidS = Math.Max(bidv2,1);
						

//						double AskMin = (double)BarItems[ii].av/(double)BarItems[ii].l.Count/(double)100*pVolumeQualifier;
//						double BidMin = (double)BarItems[ii].bv/(double)BarItems[ii].l.Count/(double)100*pVolumeQualifier;
						
//						AskMin = 0;
//						BidMin = 0;
						
////						if (CurrentBars[0] == 320)
////						{
////							Print(price + "  " + BidS + "  " + BidMin);
////						}
						
//						if (askratio >= pImbalanceOffset && BidS >= BidMin)
//						{
//							BarItems[ii].AskI.Add(price);
//						}
//						if (bidratio >= pImbalanceOffset && AskS >= AskMin)
//						{
//							BarItems[ii].BidI.Add(price);
//						}
						
//						// magnets
						
//						//BarItems[ii].Magnets.Clear();
//						if (Math.Abs(bidv - askv) <= pMAGSpreadMax && askv >= pMAGSizeMin && bidv >= pMAGSizeMin)
//						{
//							Level LLL = new Level();
//							LLL.Price = price;
//							LLL.EndBar = 0;
							
//							BarItems[ii].Magnets.Add(LLL);
//						}
						
						
						
//						// unfinished auctions
						
//						//BarItems[ii].UnfinishedAuctions.Clear();
//						if (IsTopPrice || IsBottomPrice)
//						{
//							if (bidv != 0 && askv != 0)
//							{
//								Level LLL = new Level();
//								LLL.Price = price;
//								LLL.EndBar = 0;
								
//								BarItems[ii].UnfinishedAuctions.Add(LLL);	
								
//							}
							
//						}
						
						
						
//					}
					
//					BarItems[ii].dvmax = deltamax;
//					BarItems[ii].dvmin = deltamin;
					
					
//					// loop through prices in order
					
//					bool IsWashLong = true;
//					bool IsWashShort = true;
					
//					bool topdone = false;
					
//					int totalbidlevels = 1;
//					int totalasklevels = 1;
//					double POC = 0;
//					int start = 0;
//					double pavol = 0;
//					double pbvol = 0;
//					double maxvol = 0;
//					double ThisPOC = 0;
//					double thisvwap = 0;
					
////					double thisvah1 = 0;
////					double thisval1 = 0;
////					double thisvah2 = 0;
////					double thisval2 = 0;
			
//					double VWAPTotal = 0;
//					//double AllVolume = Profiles[ind].tv;
					
//					//double lowestprice = 99999999;
//					//double highestprice = 0;
					
//					double clustertop = 0;
//					double clusterbottom = 0;
//					double currentcluster = 0;
//					double largestcluster = 0;
					
//					ArrayList	lastHighCache	= new ArrayList();
				
//					lastHighCache.Clear();
					
					
//					foreach (KeyValuePair<double, RowData> pair in SD) //.OrderBy(key=> key.))
//					{
//						double price = pair.Key;
//						double avol = pair.Value.av;
//						double bvol = pair.Value.bv;
//						double tvol = pair.Value.tv;
						
						
//						lastHighCache.Add(tvol);
//						if (lastHighCache.Count > pClusterSize)
//							lastHighCache.RemoveAt(0);
						
//						currentcluster = 0;
						
//						if (lastHighCache.Count == pClusterSize)
//						{
							
//							for (int i=0; i < pClusterSize; i++)
//								currentcluster = currentcluster + (double) lastHighCache[i];
							
//							if (currentcluster > largestcluster)
//							{
//								largestcluster = currentcluster;
//								clustertop = price;
//								clusterbottom = clustertop - (pClusterSize-1)*TickSize;
								
//							}		
							
							
//						}
						

						
//						VWAPTotal = VWAPTotal + tvol*price;
						
//						if (tvol > maxvol)
//						{
//							maxvol = tvol;
//							ThisPOC = price;
//						}
							
						
//						if (start != 0)
//						{
//							if (bvol > pbvol && !topdone)
//							{
//								totalbidlevels = totalbidlevels + 1;
//							}
//							else
//							{
//								topdone = true;
//							}
							
//							if (avol < pavol)
//							{
//								totalasklevels = totalasklevels + 1;
//							}
//							else
//							{
//								totalasklevels = 1;
//							}
							
//						}
						
						
//						//bool IsTopPrice = price == Highs[0][0];
//						//bool IsBottomPrice = price == Lows[0][0];
							
//						//Print(price + " " + pair.Value.bv + " " + pair.Value.av + " ");
						
//						// looping bottom of bar to the top 
						
//						IsWashLong = true;
						
//						pavol = avol;
//						pbvol = bvol;
//						start = 1;
//					}
					
//					thisvwap = VWAPTotal / BarItems[ii].tv;
					
//					BarItems[ii].POC = ThisPOC;
//					BarItems[ii].VWAP = thisvwap;
					
//					BarItems[ii].ClusterTop = clustertop;
//					BarItems[ii].ClusterBottom = clusterbottom;					
								
//					LongSignals[ii] = 0;
//					ShortSignals[ii] = 0;
					
//					bool ShortSignalOK = false;
		
//					if (pHighLowRangeFilter)
//					{
								
//						//if (Highs[0].IsValidPlot(CurrentBars[0]-1))
//						if (Highs[0][ii] > Highs[0][ii+1] && (!pHighLowRangeFilter2 || Closes[0][ii] <= Opens[0][ii]))		
//							ShortSignalOK = true;
//					}
//					else
//					{
//						if (!pHighLowRangeFilter2 || Closes[0][ii] <= Opens[0][ii])	
//						ShortSignalOK = true;
						
//					}
		
//					bool LongSignalOK = false;
					
//					if (pHighLowRangeFilter)
//					{
								
//						//if (Lows[0].IsValidPlot(CurrentBars[0]-1))
//						if (Lows[0][ii] < Lows[0][ii+1] && (!pHighLowRangeFilter2 || Closes[0][ii] >= Opens[0][ii]))				
//						LongSignalOK = true;
//					}
//					else
//					{
//						if (!pHighLowRangeFilter2 || Closes[0][0] >= Opens[0][0])
//						LongSignalOK = true;
						
//					}
		
		
//					if (totalbidlevels >= pMinimumDecliningLevels && LongSignalOK)
//						LongSignals[ii] = totalbidlevels;
					
//					if (totalasklevels >= pMinimumDecliningLevels && ShortSignalOK)
//						ShortSignals[ii] = totalasklevels;
					
//					PreviousLongSignals = CurrentLongSignals;
//					PreviousShortSignals = CurrentShortSignals;
				
					
					
//					CurrentLongSignals = LongSignals[ii];
//					CurrentShortSignals = ShortSignals[ii];
					
//					// add Zones (Imbalance)
					
					
//					//Print("-------------");
//					ZoneItems[ii] = new ZoneItem();
									
					
//					ZoneItems[ii].ResistanceZones = new List<Zone>();
//					ZoneItems[ii].SupportZones = new List<Zone>();
		
//					double bottomn = 0;
//					int csize = 0;
//					Zone Z;		
					
//					// support / ask
//					foreach(double d in BarItems[ii].BidI.OrderBy(d => d))
//					{
//						// going bottom to top
//						//Print(i);
						
//						// logic to qualify a zone
						
//						if (d <= Highs[0][ii])
//						{
//							if (bottomn == 0)
//							{
//								bottomn = d;
//								csize++;
//							}
//							else
//							{
//								if (RTTS(bottomn + TickSize*csize) == d)
//								{
//									csize++;
//								}
//								else
//								{
//									if (csize >= pMinZWidth)
//									{
//										Z = new Zone();
//										Z.BottomPrice = bottomn;
//										Z.TopPrice = bottomn + (csize-1)*TickSize;
//										Z.TicksWidth = csize;
//										Z.TestedPrice = 0;
//										Z.IsBroken = false;
//										Z.IsHidden = false;
//										Z.EndBar = 0;
										
										
//										ZoneItems[ii].ResistanceZones.Add(Z);
										
//										//Print(bottomn + "  " + csize);
//									}
									
//									csize = 1;
//									bottomn = d;
									
//								}
//							}
//						}
//					}
					
//					if (csize >= pMinZWidth)
//					{
//						Z = new Zone();
//						Z.BottomPrice = bottomn;
//						Z.TopPrice = bottomn + (csize-1)*TickSize;
//						Z.TicksWidth = csize;
//						Z.TestedPrice = 0;
//						Z.IsBroken = false;
//						Z.IsHidden = false;
//						Z.EndBar = 0;
						
//						//SupplyZones.Add(CurrentBar,
//						ZoneItems[ii].ResistanceZones.Add(Z);
						
						
//					}
			
//					csize = 0;
								
//					foreach(double d in BarItems[ii].AskI.OrderBy(d => d))
//					{
//						if (d <= Highs[0][ii])
//						{
//							if (bottomn == 0)
//							{
//								bottomn = d;
//								csize++;
//							}
//							else
//							{
//								if (RTTS(bottomn + TickSize*csize) == d)
//								{
//									csize++;
//								}
//								else
//								{
//									if (csize >= pMinZWidth)
//									{
//										Z = new Zone();
//										Z.BottomPrice = bottomn;
//										Z.TopPrice = bottomn + (csize-1)*TickSize;
//										Z.TicksWidth = csize;
//										Z.TestedPrice = 0;
//										Z.IsBroken = false;
//										Z.IsHidden = false;
//										Z.EndBar = 0;
										
//										//SupplyZones.Add(CurrentBar,
//										ZoneItems[ii].SupportZones.Add(Z);
										
//										//Print(bottomn + "  " + csize);
//									}
									
//									csize = 1;
//									bottomn = d;
									
//								}

//							}
//						}
						
//					}
					
//					if (csize >= pMinZWidth)
//					{
//						Z = new Zone();
//						Z.BottomPrice = bottomn;
//						Z.TopPrice = bottomn + (csize-1)*TickSize;
//						Z.TicksWidth = csize;
//						Z.TestedPrice = 0;
//						Z.IsBroken = false;
//						Z.IsHidden = false;
//						Z.EndBar = 0;
						
//						//SupplyZones.Add(CurrentBar,
//						ZoneItems[ii].SupportZones.Add(Z);
						
						
//					}						
					
					
						
//				}	
					
			
//		}
		
//	    // This method is fired on market depth events and after the snapshot data is updated.
//	    private void OnMarketDepth2 (object sender, MarketDepthEventArgs e)
//	    {
//			if(State == State.Realtime)
//			{
//				if((DateTime.Now - lastRender).TotalMilliseconds >= pRenderMilliseconds)
//				{
//					ForceRefresh();
//				}
				
//				//ForceRefresh();
//			}
			
			
//		}
			
		protected override void OnMarketDepth(MarketDepthEventArgs marketDepthUpdate)
		{
			
			if (BarsInProgress != 0)
				return;
			
			rows = null;
			
			//if (pDepthEnabled)
			{
				
				try
				{
				
					//Print(marketDepthUpdate.Price);
					
					// Checks to see if the Market Data is of the Ask type
					if (marketDepthUpdate.MarketDataType == MarketDataType.Ask)
						rows = askRows;

					// Checks to see if the Market Data is of the Bid type
					else if (marketDepthUpdate.MarketDataType == MarketDataType.Bid)
						rows = bidRows;

					if (rows == null)
						return;

					// Checks to see if the action taken by the Ask data was an insertion into the ladder
					if (marketDepthUpdate.Operation == Operation.Add)
						rows.Insert(marketDepthUpdate.Position, new LadderRow(marketDepthUpdate.Price, marketDepthUpdate.Volume, marketDepthUpdate.MarketMaker));

					// Checks to see if the action taken by the Ask data was a removal of itself from the ladder
					// Note: Due to the multi threaded architecture of the NT core, race conditions could occur
					// -> check if e.Position is within valid range 
					else if (marketDepthUpdate.Operation == Operation.Remove && marketDepthUpdate.Position < rows.Count)
						rows.RemoveAt(marketDepthUpdate.Position);

					// Checks to see if the action taken by the Ask data was to update a data already on the ladder
					// Note: Due to the multi threaded architecture of the NT core, race conditions could occur
					// -> check if e.Position is within valid range 
					else if (marketDepthUpdate.Operation == Operation.Update && marketDepthUpdate.Position < rows.Count)
					{
						rows[marketDepthUpdate.Position].MarketMaker	= marketDepthUpdate.MarketMaker;
						rows[marketDepthUpdate.Position].Price			= marketDepthUpdate.Price;
						rows[marketDepthUpdate.Position].Volume			= marketDepthUpdate.Volume;
					}

					// Calling ChartControl.Refresh() will cause the Level II data to update real-time.  Adding
					// this delay routine will prevent CPU utilization from going through the roof.  Setting a 
					// really high value will effectly delay the update until the next tick arrives which could
					// be a really long time is the market is really slow.  This may or may not matter to some.
					
					if (lastRefresh.AddMilliseconds(pRenderMilliseconds) < DateTime.Now)
					{
// this may need to be ChartControl.InvalidateVisual();
						ForceRefresh();
						lastRefresh = DateTime.Now;
					}
				}
				catch (Exception ex)
				{
					if (TestRender) Print("OnMarketDepth: " + ex.Message + " ");
					
				}
			}
		}
		
		

//		protected override void OnMarketDepth (MarketDepthEventArgs marketDepthUpdate)
//		{
//			if (BarsInProgress != 0)
//				return;
			
//			//ThisMarketDepth. = marketDepthUpdate
			
//		    // Print some data to the Output window
//		    double Price = marketDepthUpdate.Price;
//			long Volume = marketDepthUpdate.Volume;
//			string Op = string.Empty;
			
//			bool IsAsk = marketDepthUpdate.MarketDataType == MarketDataType.Ask;
//			bool IsBid = marketDepthUpdate.MarketDataType == MarketDataType.Bid;
			
//			//AskLevels.Clear();
//			//BidLevels.Clear();
			
//			if (IsAsk || IsBid)
//			{
//				if (marketDepthUpdate.Operation == Operation.Update)
//				{
//					if (IsAsk) AskLevels[Price] = Volume;
//					if (IsBid) BidLevels[Price] = Volume;
//					//Op = "Update: ";
//				}
//				if (marketDepthUpdate.Operation == Operation.Add)
//				{
//					if (IsAsk) AskLevels.TryAdd(Price,Volume);
//					if (IsBid) BidLevels.TryAdd(Price,Volume);
					
//				}
//				if (marketDepthUpdate.Operation == Operation.Remove)
//				{
//					long ll = 0;
					
//					if (IsAsk) AskLevels.TryRemove(Price, out ll);
//					if (IsBid) BidLevels.TryRemove(Price, out ll);
					
//					Print("Remove: " + Price.ToString(PriceString) + "  " + Volume.ToString());
					
//				}				
//				//Print(Op + Price.ToString(PriceString) + "  " + Volume.ToString());
				
//			}
//		        //Print(string.Format("The most recent ask change is {0} {1}", marketDepthUpdate.Price, marketDepthUpdate.Volume));
		
//		}   


		
		protected override void OnMarketData (MarketDataEventArgs e)
		{
			if(e.MarketDataType == MarketDataType.Bid)
				CurrentBid = e.Price;
			
			if(e.MarketDataType == MarketDataType.Ask)
				CurrentAsk = e.Price;
			
		if(e.MarketDataType == MarketDataType.Last)
				CurrentLastData = e.Price;			
			
			//Print(CurrentBid);
			
			//return;
			
			if (!IsTickReplay)
				return;
			
			
			if(Bars.Count < 0)   { return; }
			if(CurrentBar < 1)   { return; }
			if(e.Time < getDate) { return; }
			
			if(State == State.Realtime)
			{
				if((DateTime.Now - lastRender).TotalMilliseconds >= pRenderMilliseconds)
				{
					ForceRefresh();
				}
			}
			
//			if(BarItems[0] == null)
//			{
//				BarItems[0] = new BarItem();
//			}
			
			if(e.MarketDataType == MarketDataType.Last)
			{
				ask = e.Ask;
				bid = e.Bid;
				cls = e.Price;
				vol = e.Volume;
				
				if (vol < pAllTradesMin)
					vol = 0;	
					
				int CurrentTrend = 1;
				
				if(cls >= ask)
					CurrentTrend = 1;
				if(cls <= bid)
					CurrentTrend = -1;	
				
				
				//cls = Closes[1][0];
				//vol = Volumes[1][0];
					
					
					
				if (BI == null)
					BI = new BarItem();
				
				if(!BI.l.ContainsKey(cls))
				{
					BI.l.TryAdd(cls, new RowData());
				}
				
				
				
				if(CurrentTrend == 1)
				{
					BI.tv += vol;
					BI.av += vol;
					
					BI.l[cls].tv += vol;
					BI.l[cls].av += vol;
					
					
					//fillMissingTicks2(0);
				}
				
				//return;
				
				if(CurrentTrend == -1)
				{
					BI.tv += vol;
					BI.bv += vol;
					
					BI.l[cls].tv += vol;
					BI.l[cls].bv += vol;
					
					//fillMissingTicks2(0);
				}
				
				BI.dv = BI.av - BI.bv;
				
				// block trades
				
				
					
				Values[23][0] = 0;
				Values[24][0] = 0;
				if (IsFirstTickOfBar)
				{
					Values[25][0] = pBlockSize;
					Values[26][0] = pMinZWidth;
				}
						
				if (vol >= pBlockSize)
				{
					if(CurrentTrend == -1)
					{
						Values[24][0] = vol;
						
						BI.BidBlocks.AddOrUpdate(cls, new List<double> { vol }, (k, list) => { list.Add(vol); return list; });
					}
					else
					{
						Values[23][0] = vol;
						
						BI.AskBlocks.AddOrUpdate(cls, new List<double> { vol }, (k, list) => { list.Add(vol); return list; });
					}
					
					
				}
				
				
				// process something after the last tick of Tick Replay is finished and we are now real time
				
				if (State == State.Realtime && IsFirstRealTimeTick)
				{
					
					fillMissingTicks5(0);
					BarItems[0] = BI;
					calculateBar(0);
					calculateProfile(0);

					//Print("OnMarketData: Is RealTime");	
					
					IsFirstRealTimeTick = false;
				}				
					
								
				if (RealTimeReset)
				{
					
					fillMissingTicks5(0);
					BarItems[0] = BI;
					calculateBar(0);
					calculateProfile(0);
					
					//Print("RTS");
					
				}
				
				
//				if (State == State.Realtime)
//				{
				
//					BarItems[0] = BI;
//					calculateBar(0);
//					calculateProfile(0);	
															
//				}

			}
			
			

			
			
//				if (RealTimeReset)
//				{
//					//Print("RTRS");
//					int ii = 0;
//					BarItems[ii] = BI;
//					calculateBar(ii);
//					calculateProfile(ii);
					
//				}
				
			
			
			
//			if (State == State.Realtime)
//			{
//				Print("OnMarketData: Is RealTime");	
//			}
			
			
			
			
			
//			if(BarItems[0] == null)
//			{
//				BarItems[0] = new BarItem();
//			}
	
//			if(e.MarketDataType == MarketDataType.Last)
//			{
//				ask = e.Ask;
//				bid = e.Bid;
//				cls = e.Price;
//				vol = e.Volume;
		
//				if(!BarItems[0].l.ContainsKey(cls))
//				{
//					BarItems[0].l.TryAdd(cls, new RowData());
//				}
		
//				if(cls >= ask)
//				{
//					BarItems[0].v += vol;
//					BarItems[0].l[cls].tv += vol;
//					BarItems[0].l[cls].av += vol;
			
//					fillMissingTicks(0);
//				}
		
//				if(cls <= bid)
//				{
//					BarItems[0].v += vol;
//					BarItems[0].l[cls].tv += vol;
//					BarItems[0].l[cls].bv += vol;
			
//					fillMissingTicks(0);
//				}
		
//				if(!showProfile && !showPoc) { return; }
		
//				if(State == State.Realtime)
//				{
//					calculateProfile(false);
//				}
//			}
	
			
			
		}
		
	
		protected override void OnBarUpdate()
		{
			if (!Permission)
				return;		
			
			
			// timer
			
					
			// range
			
			if (BarsArray == null || BarsArray.Length == 0)
				return;

			if (BarsInProgress == 0)
			{
			
				if (!isRangeDerivate && BarsArray[0].BarsType.BuiltFrom == BarsPeriodType.Tick && BarsArray[0].BarsPeriod.ToString().IndexOf("Range") >= 0)
					isRangeDerivate = true;
		
				if (isRangeDerivate)
				{
					double high = High.GetValueAt(Bars.Count - 1 - (Calculate == NinjaTrader.NinjaScript.Calculate.OnBarClose ? 1 : 0));
					double low = Low.GetValueAt(Bars.Count - 1 - (Calculate == NinjaTrader.NinjaScript.Calculate.OnBarClose ? 1 : 0));
					double close = Close.GetValueAt(Bars.Count - 1 - (Calculate == NinjaTrader.NinjaScript.Calculate.OnBarClose ? 1 : 0));
					int actualRange = (int)Math.Round(Math.Max(close - low, high - close) / Bars.Instrument.MasterInstrument.TickSize);
					double rangeCount = pCountDown ? BarsPeriod.Value - actualRange : actualRange;

					BarTimerString = rangeCount.ToString();
					
					//Draw.TextFixed(this, "NinjaScriptInfo", range1, TextPosition.BottomRight);					
						
				}
				else if (BarsPeriod.BarsPeriodType == BarsPeriodType.Tick)
				{
					
					// tick
					
					double periodValue = (BarsPeriod.BarsPeriodType == BarsPeriodType.Tick) ? BarsPeriod.Value : BarsPeriod.BaseBarsPeriodValue;
					double tickCount = pShowPercent ? pCountDown ? (1 - Bars.PercentComplete) * 100 : Bars.PercentComplete * 100 : pCountDown ? periodValue - Bars.TickCount : Bars.TickCount;

					BarTimerString = tickCount.ToString() + (pShowPercent ? "%" : "");
					
					//Draw.TextFixed(this, "NinjaScriptInfo", tick1, TextPosition.BottomRight, ChartControl.Properties.ChartText, ChartControl.Properties.LabelFont, Brushes.Transparent, Brushes.Transparent, 0);			
					
									
					
				}
				else if (BarsPeriod.BarsPeriodType == BarsPeriodType.Volume)
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

					BarTimerString = volumeCount.ToString() + (pShowPercent ? "%" : "");
					
					//Draw.TextFixed(this, "NinjaScriptInfo", volume1, TextPosition.BottomRight);				
					
				}
				else
				{
					
					
					
				}
			
			
			}
			
			// end counter
			
			

			

			
			
			

			
			
			
			if (State == State.Realtime)
			{
				hasRealtimeData = true;
				connected = true;
			}
			
 			if (CurrentBars[0] < 0)
				return;
			
			if (!IsTickReplay)
			if (CurrentBars[1] < 0)
				return;
			
			if (BarsInProgress == 0)
			{
				BodyHigh[0] = Math.Max(Close[0], Open[0]);
				BodyLow[0] = Math.Min(Close[0], Open[0]);
				
				if (Close[0] > Open[0])
				{
					Direction[0] = 1;
					Direction2[0] = 1;
				}
				else if (Close[0] < Open[0])
				{
					Direction[0] = -1;
					Direction2[0] = -1;
				}	
				else
				{
					if (CurrentBars[0] == 0)
						Direction[0] = 1;
					else
						Direction[0] = Direction[1];
					
					Direction2[0] = 0;
				}
					
				if (pCompLevelsPriceOn)
				{	
					PlotBrushes[2][0] = pPlot1Brush;
					PlotBrushes[3][0] = pPlot2Brush;
					PlotBrushes[4][0] = pPlot3Brush;
					PlotBrushes[5][0] = pPlot4Brush;
					PlotBrushes[6][0] = pPlot5Brush;
					PlotBrushes[7][0] = pPlot6Brush;
					PlotBrushes[17][0] = pPlot7Brush;
					PlotBrushes[18][0] = pPlot8Brush;					
					
				}
				
		
			}
			
		
			

			
			//if (State == State.Realtime)
			//Print(BarsInProgress + "  " + CurrentBar + " - " + Volume[0]);

			
			
			
//			if (BarsInProgress == 0)
//			{
//				ThisEMA1[0] = EMA(20)[0];
//				ThisEMA2[0] = EMA(50)[0];
//			}
			
			
			
			
			
			// SCALE
			if (TestLoad)
			{
				if (!IsTickReplay)
				if (State == State.Historical)
				{
					BarsProcessed = CurrentBars[1];
					
					//if (CurrentBars[1] == 0)
					BarsRemaining = BarsArray[1].Count;
				}
			}
				
			
			
			if (BarsInProgress == 0)
			{
				
				
				if (IsFirstTickOfBar)
				{
					
					ForceHighPriority = false;	
					ForceLowPriority = false;
				}
				
				
				if (CurrentBar == 0)
				{
					ScaleHigh[0] = High[0];
					ScaleLow[0] = High[0]-pScaleTicks*TickSize;
					
				}
				else
				{
		
					SetThisScale(0);
				}
				

			}
			
		


	
			// INDIVIDUAL TICKS
				
			if (!IsTickReplay)
			if (BarsInProgress == 1)
			{
				
				//Print(CurrentLast + "  " + Times[1][0].ToString() + "   " + Times[1][0].Millisecond.ToString() + "   " + Volumes[1][0]);
				
				// add iceberg order detection
				
				CurrentLast = Closes[1][0];
				
				if (CurrentBars[1] == 0)
				{
					CurrentTrend = 1;
				}
				else
				{
					
					if (Closes[1][0] > Closes[1][1])
						CurrentTrend = 1;
					
					if (Closes[1][0] < Closes[1][1])
						CurrentTrend = -1;
				}
				
				
				if(BarsArray[1].Count < 0)   { return; }
				if(CurrentBars[1] < 0)   { return; }
				if(Times[1][0] < getDate) { return; }
				

				//if(e.MarketDataType == MarketDataType.Last)
				
				
					//ask = e.Ask;
					//bid = e.Bid;
					cls = Closes[1][0];
					vol = Volumes[1][0];
					
					if (vol < pAllTradesMin)
						vol = 0;					
					
					if (BI == null)
						BI = new BarItem();
					
					if(!BI.l.ContainsKey(cls))
					{
						BI.l.TryAdd(cls, new RowData());
					}
					

					
					if(CurrentTrend == 1)
					{
						BI.tv += vol;
						BI.av += vol;
						
						BI.l[cls].tv += vol;
						BI.l[cls].av += vol;
						
						//fillMissingTicks2(0);
					}
					
					//return;
					
					if(CurrentTrend == -1)
					{
						BI.tv += vol;
						BI.bv += vol;
						
						BI.l[cls].tv += vol;
						BI.l[cls].bv += vol;
						
						//fillMissingTicks2(0);
					}
					
					//fillMissingTicks5(0);
					
					BI.dv = BI.av - BI.bv;
					
					// block trades
					
					
						
//											AddPlot(new Stroke(Brushes.Transparent, DashStyleHelper.Solid, 1),	PlotStyle.Line, "BlockBuy");
//					AddPlot(new Stroke(Brushes.Transparent, DashStyleHelper.Solid, 1),	PlotStyle.Line, "BlockSell"); // 24
						
							
								
					Values[23][0] = 0;
					Values[24][0] = 0;
					Values[25][0] = pBlockSize;
					Values[26][0] = pMinZWidth;
					
					if (vol >= pBlockSize)
					{
						
						string NName = "Impact Order Flow";
						
						if(CurrentTrend == -1)
						{
							
							if (pAudioEnabledMain && pAudioEnabled9)
								Alert(CurrentBar.ToString(),Priority.High, BarsArray[0].BarsPeriod + " | " + NName + " Bid Block Trade ("+vol.ToString()+")", NinjaTrader.Core.Globals.InstallDir+@"\sounds\"+pWAVFile9Buy,1,pArrowDownFBrush,Brushes.White);
							
							Values[24][0] = vol;
							
							BI.BidBlocks.TryAdd(cls, new List<double>());
							BI.BidBlocks[cls].Add(vol);
						}
						else
						{
							
							if (pAudioEnabledMain && pAudioEnabled9)
								Alert(CurrentBar.ToString(),Priority.High, BarsArray[0].BarsPeriod + " | " + NName + " Ask Block Trade ("+vol.ToString()+")", NinjaTrader.Core.Globals.InstallDir+@"\sounds\"+pWAVFile9Sell,1,pArrowUpFBrush,Brushes.White);
							
							Values[23][0] = vol;
							
							BI.AskBlocks.TryAdd(cls, new List<double>());
							BI.AskBlocks[cls].Add(vol);
						}
						
						
					}
					
		 
					
				
				
				
				if (RealTimeReset)
				{
					int ii = 0;
					
					
					BarItems[ii] = BI;
					calculateBar(ii);
					calculateProfile(ii);
					
					//Print("RTS");
					
				}
				
				
			}
				
			
			int BBHist = 0;
			
			
			if (IsTickReplay)
			{
				
				//Draw.TextFixed(this, "IsTickReplayMessage", "You must disable Tick Replay.  Right click, Data Series.  Uncheck Tick Replay.", TextPosition.BottomRight);
				
				//Draw.TextFixed(this, "IsTickReplayMessage", "Using Tick Replay.", TextPosition.BottomRight);
				
				//return;
				
				if (State == State.Historical)
				{
					if (!IsFirstTickOfBar)
					{
						return;
						
					}
					else
					{
						BBHist = 1;	
					}
					
				}
				
					
			}			


			// BAR CLOSES
			
			if (BarsInProgress == 0)
			{


				IsNowRealTime = false;
			
				if (BarsArray[0].Count == CurrentBars[0]+1)
				{
					//Print("IS GOING REAL TIME");	
					IsNowRealTime = true;
				}
				
				HistoricalReset = State == State.Historical && !IsNowRealTime;
				RealTimeReset = State == State.Realtime && IsFirstTickOfBar;
				TickReplayReset = State == State.Historical && IsTickReplay;
				
				int ii = 0;
				// NinjaTrader re-fires IsFirstTickOfBar for the SAME realtime range bar
				// when it rebuilds bars. Only finalize the previous bar (ii=1) on a
				// genuine forward advance — otherwise a re-fire commits an empty/partial
				// BI over the prior bar and wipes it to zeros.
				if ((RealTimeReset || TickReplayReset) && CurrentBars[0] > lastFinalizedBar)
				{
					ii = 1;

				}
				
				
				// process the bar details and what not
				
				if (CurrentBar < ii)
					return;
				
				
				fillMissingTicks5(ii);

				BarItems[ii] = BI;
				
				// Forming-bar recompute is throttled to the render cadence (Calculation
				// Frequency); a finalize (ii==1) and historical processing always recompute.
				if (ii == 1 || State != State.Realtime
					|| (DateTime.Now - lastFormingCompute).TotalMilliseconds >= pRenderMilliseconds)
				{
					calculateBar(ii);
					calculateProfile(ii);
					if (ii == 0)
						lastFormingCompute = DateTime.Now;
				}
				
				
				
				

				if (HistoricalReset || ii == 1)
				{
					BI = new BarItem();
				}

				if (ii == 1)
					lastFinalizedBar = CurrentBars[0];
				
				
			}					
	
			
			
			
	
			//return;
			
			
				
			if (BarsInProgress == 0)
			{		
				
				// EMA TREND
				if (CurrentBars[0] > 0)
				{
					if (TrendEMA[0] > TrendEMA[1])
						CurrentEMATrend[0] = 1;
					else
						CurrentEMATrend[0] = -1;
				}
				
				
				
				// SWING LEVELS
				
				IsCurrentBar = State == State.Realtime;
				
				FinalHigh[0] = Highs[0][BBHist];
				FinalLow[0] = Lows[0][BBHist];
				//AllPivots[0] = 0;
			
				if (WaitingForHigh && IsCurrentBar)
				{
					
					if (HighLA.ContainsKey(CurrentHighBar)) HighLA.Remove(CurrentHighBar);
					
				}
				if (WaitingForLow && IsCurrentBar)
				{
					
					if (LowLA.ContainsKey(CurrentLowBar)) LowLA.Remove(CurrentLowBar);
					
			
					
				}
				



				if (WaitingForHigh)
				{

					if (FinalHigh[0] > CurrentHighPrice)
					{
						if (CurrentBar > 1)
						{
	
							AllPivots[CurrentBar-CurrentHighBar] = 0;
							
						}
					

						
						CurrentHighPrice = FinalHigh[0];
						CurrentHighBar = CurrentBar-BBHist;
			
						
					}
					
					
					
					
					if (CurrentHighBar != CurrentBar && FinalLow[0] <= CurrentHighPrice - pTicksMove*TickSize+TickSize*0.5)
					{

						AllPivots[CurrentBar-CurrentHighBar] = 1;

						WaitingForLow = true;
						WaitingForHigh = false;
						
						CurrentLowPrice = FinalLow[0];
						CurrentLowBar = CurrentBar-BBHist;
						

						if (!HighLA.ContainsKey(CurrentHighBar)) HighLA.Add(CurrentHighBar, CurrentHighPrice);					
						
						
					}
					
				}
				
				if (WaitingForLow)
				{

					if (FinalLow[0] < CurrentLowPrice)
					{
						if (CurrentBar > 1)
						{
						
							AllPivots[CurrentBar-CurrentLowBar] = 0;
						}
					
						
						CurrentLowPrice = FinalLow[0];
						CurrentLowBar = CurrentBar-BBHist;
						
				
					}
					
					
					
					
					if (CurrentLowBar != CurrentBar && FinalHigh[0] >= CurrentLowPrice + pTicksMove*TickSize-TickSize*0.5)
					{

						AllPivots[CurrentBar-CurrentLowBar] = -1;
		
						WaitingForLow = false;
						WaitingForHigh = true;
						
						CurrentHighPrice = FinalHigh[0];
						CurrentHighBar = CurrentBar-BBHist;
						

							if (!LowLA.ContainsKey(CurrentLowBar))
						LowLA.Add(CurrentLowBar,CurrentLowPrice);			
						
					}				
				
					
					
				}

			
			
						
				if (IsCurrentBar)
				{
					
			
					
					if (WaitingForHigh)
					{

						AllPivots[CurrentBar-CurrentHighBar] = 1;

						if (!HighLA.ContainsKey(CurrentHighBar)) HighLA.Add(CurrentHighBar, CurrentHighPrice);	
						
					
					}
					if (WaitingForLow)
					{

						AllPivots[CurrentBar-CurrentLowBar] = -1;


						
						if (!LowLA.ContainsKey(CurrentLowBar)) LowLA.Add(CurrentLowBar, CurrentLowPrice);	
						
					}
				
					
				
					
				}
				
				
				
				DeleteLA.Clear();	

				foreach (KeyValuePair<int,double> L in HighLA)
				{
					if (Highs[0][BBHist] > L.Value)
					{
						DeleteLA.Add(L.Key,L.Value);
					}
				}	
				
				foreach (KeyValuePair<int,double> L in DeleteLA)
				{
					HighLA.Remove(L.Key);
					//if (pHistoricalLevelsEnabled) 
					
					if (!HighLF.ContainsKey(L.Key))
					HighLF.Add(L.Key,CurrentBars[0]-BBHist);
				}
				
				DeleteLA.Clear();

				foreach (KeyValuePair<int,double> L in LowLA)
				{
					//Print(L.Value.Price);
					
					if (Lows[0][BBHist] < L.Value)
					{
						//if (!DeleteLA.ContainsKey(L.Key))
							DeleteLA.Add(L.Key,L.Value);
						
					}
				}	
				
				foreach (KeyValuePair<int,double> L in DeleteLA)
				{
					LowLA.Remove(L.Key);
					
					if (!LowLF.ContainsKey(L.Key))
					LowLF.Add(L.Key,CurrentBars[0]-BBHist);
				}
				
				
				
				
				
				// CLOSE MAGNETS AND UNFINISHED AUCTIONS
				
				
//				if (BarsInProgress == 0)
//				{
					
//					Print(CurrentBar);
					
					
//				}
					
				
				
				//BBHist = 0;
				
				// O(n) closure: only check active (unclosed) levels against current bar range
				double currentHigh = Highs[0][BBHist];
				double currentLow = Lows[0][BBHist];
				int currentEndBar = CurrentBars[0] - BBHist;

				for (int ami = ActiveMagnets.Count - 1; ami >= 0; ami--)
				{
					Level kvp2 = ActiveMagnets[ami];
					if (currentHigh >= kvp2.Price && currentLow <= kvp2.Price)
					{
						kvp2.EndBar = currentEndBar;
						ActiveMagnets.RemoveAt(ami);
					}
				}

				for (int aui = ActiveUFAs.Count - 1; aui >= 0; aui--)
				{
					Level kvp2 = ActiveUFAs[aui];
					if (currentHigh >= kvp2.Price && currentLow <= kvp2.Price)
					{
						kvp2.EndBar = currentEndBar;
						ActiveUFAs.RemoveAt(aui);
					}
				}
				
					
				
				
				
			
				
				// update Zones (Momentum) 
				
				if (CurrentBars[0] > 2)
				{
				
				//BodyHigh[0] = RTTS(Math.Max(Close[0],Open[0]));
				//BodyLow[0] = RTTS(Math.Min(Close[0],Open[0]));
				
				
				
				Zone Z;
				int bbbb = 1;
				
				
				//ZoneItems[bbbb] = null;
				
//				ZoneItems[bbbb] = new ZoneItem();
//				ZoneItems[bbbb].SupportZones2 = new List<Zone>();
//				ZoneItems[bbbb].ResistanceZones2 = new List<Zone>();
				
				
				if (Low[1] <= High[2] && Low[0] <= High[1])
				if (High[2] + RTTS((pMinZWidth2-1)*TickSize) <= Low[0])
				{
						
//					if (!ZoneItems.IsValidDataPoint(bbbb))
//					{
//						ZoneItems[bbbb] = new ZoneItem();
						
//					}
					
//					ZoneItems[bbbb].SupportZones2 = new List<Zone>();
//					ZoneItems[bbbb].ResistanceZones2 = new List<Zone>();
					
				//ZoneItems[bbbb] = new ZoneItem();
				ZoneItems[bbbb].SupportZones2 = new List<Zone>();
				ZoneItems[bbbb].ResistanceZones2 = new List<Zone>();
					
					Z = new Zone();
					Z.BottomPrice = High[2];
					Z.TopPrice = Low[0];
					Z.TicksWidth = (long) RTTS((Low[0]-High[2])/TickSize) + 1;
					Z.TestedPrice = 0;
					Z.IsBroken = false;
					Z.IsHidden = false;
					Z.EndBar = 0;

					ZoneItems[bbbb].SupportZones2.Add(Z);
					Z.StartBar = CurrentBars[0] - bbbb; ActiveSupportZones2.Add(Z);
				}
				
				
				
				if (High[1] >= Low[2] && High[0] >= Low[1])
				if (Low[2] - RTTS((pMinZWidth2-1)*TickSize) >= High[0])
				{
					
//					if (!ZoneItems.IsValidDataPoint(bbbb))
//					{
//						ZoneItems[bbbb] = new ZoneItem();
						
//					}
					
//					ZoneItems[bbbb].SupportZones2 = new List<Zone>();
//					ZoneItems[bbbb].ResistanceZones2 = new List<Zone>();
					
				//ZoneItems[bbbb] = new ZoneItem();
				ZoneItems[bbbb].SupportZones2 = new List<Zone>();
				ZoneItems[bbbb].ResistanceZones2 = new List<Zone>();
					
					Z = new Zone();
					Z.BottomPrice = High[0];
					Z.TopPrice = Low[2];
					Z.TicksWidth = (long) RTTS((Low[2]-High[0])/TickSize) + 1;
					Z.TestedPrice = 0;
					Z.IsBroken = false;
					Z.IsHidden = false;
					Z.EndBar = 0;
					
					ZoneItems[bbbb].ResistanceZones2.Add(Z);
					Z.StartBar = CurrentBars[0] - bbbb; ActiveResistanceZones2.Add(Z);
				}


				// TERMINATE ZONES
				
//				bool IsCurrentBar = false;
//				if (BarsArray[0].Count == CurrentBars[0]+1)
//					{
//						//Print("IS GOING REAL TIME");	
//						IsCurrentBar = true;
//					}
				
					// O(n) zone termination: only check active (unclosed) zones
					{
						double zoneHigh = Highs[0][0];
						double zoneLow = Lows[0][0];
						int zoneCurrentBar = CurrentBars[0];

						for (int zi = ActiveResistanceZones2.Count - 1; zi >= 0; zi--)
						{
							Zone ZS = ActiveResistanceZones2[zi];
								if (ZS.StartBar >= zoneCurrentBar) continue;
							if (zoneHigh >= ZS.BottomPrice)
							{
								if (ZS.TestedPrice == 0) ZS.TestedPrice = zoneHigh;
								else ZS.TestedPrice = Math.Max(ZS.TestedPrice, zoneHigh);
							}
							if (zoneHigh > ZS.TopPrice)
							{
								ZS.EndBar = zoneCurrentBar; ZS.IsBroken = true; ZS.TestedPrice = 0;
								ActiveResistanceZones2.RemoveAt(zi);
							}
						}
						for (int zi = ActiveSupportZones2.Count - 1; zi >= 0; zi--)
						{
							Zone ZS = ActiveSupportZones2[zi];
								if (ZS.StartBar >= zoneCurrentBar) continue;
							if (zoneLow <= ZS.TopPrice)
							{
								if (ZS.TestedPrice == 0) ZS.TestedPrice = zoneLow;
								else ZS.TestedPrice = Math.Min(ZS.TestedPrice, zoneLow);
							}
							if (zoneLow < ZS.BottomPrice)
							{
								ZS.EndBar = zoneCurrentBar; ZS.IsBroken = true; ZS.TestedPrice = 0;
								ActiveSupportZones2.RemoveAt(zi);
							}
						}
						for (int zi = ActiveResistanceZones.Count - 1; zi >= 0; zi--)
						{
							Zone ZS = ActiveResistanceZones[zi];
								if (ZS.StartBar >= zoneCurrentBar) continue;
							if (zoneHigh >= ZS.BottomPrice)
							{
								if (ZS.TestedPrice == 0) ZS.TestedPrice = zoneHigh;
								else ZS.TestedPrice = Math.Max(ZS.TestedPrice, zoneHigh);
							}
							if (zoneHigh > ZS.TopPrice)
							{
								ZS.EndBar = zoneCurrentBar; ZS.IsBroken = true; ZS.TestedPrice = 0;
								ActiveResistanceZones.RemoveAt(zi);
							}
						}
						for (int zi = ActiveSupportZones.Count - 1; zi >= 0; zi--)
						{
							Zone ZS = ActiveSupportZones[zi];
								if (ZS.StartBar >= zoneCurrentBar) continue;
							if (zoneLow <= ZS.TopPrice)
							{
								if (ZS.TestedPrice == 0) ZS.TestedPrice = zoneLow;
								else ZS.TestedPrice = Math.Min(ZS.TestedPrice, zoneLow);
							}
							if (zoneLow < ZS.BottomPrice)
							{
								ZS.EndBar = zoneCurrentBar; ZS.IsBroken = true; ZS.TestedPrice = 0;
								ActiveSupportZones.RemoveAt(zi);
							}
						}
					}

					// Old O(n²) zone loop removed - replaced by active zone tracking above
				
				
			
				} // end is currentbar
				
				
				
				
				BB = 0;
				if (Calculate != Calculate.OnBarClose && !pQuickAudio)
					BB = 1;
				
				if (State == State.Realtime)
				if(pAudioEnabledMain && CurrentBars[0] >= BB)
				{
					
					string NName = "Impact Order Flow";
					
					if (pAudioEnabled1)
					{
						if(LongSignals[BB] != 0 && LastAudioBar1 != CurrentBars[0])
						{
							Alert(CurrentBar.ToString(),Priority.High, BarsArray[0].BarsPeriod + " | " + NName + " Washout Buy", NinjaTrader.Core.Globals.InstallDir+@"\sounds\"+pWAVFileName,1,pArrowUpFBrush,Brushes.White);
							LastAudioBar1 = CurrentBars[0];
							
							//BackBrushes[0] = Brushes.Green;
							
						}
						if(ShortSignals[BB] != 0 && LastAudioBar2 != CurrentBars[0])
						{
							Alert(CurrentBar.ToString(),Priority.High, BarsArray[0].BarsPeriod + " | " + NName + " Washout Sell", NinjaTrader.Core.Globals.InstallDir+@"\sounds\"+pWAVFileName2,1,pArrowDownFBrush,Brushes.White);
							LastAudioBar2 = CurrentBars[0];
							
							//BackBrushes[0] = Brushes.Red;
						}
					}
					
					if (ZoneItems[BB] != null)
					{
						if (pAudioEnabled2)
						{
							
							BB = 1; // force this to happen after close
							
							//if (ZoneItems[BB] != null)
							if(ZoneItems[BB].SupportZones.Count != 0 && LastAudioBar3 != CurrentBars[0])
							{
								Alert(CurrentBar.ToString(),Priority.High, BarsArray[0].BarsPeriod + " | " + NName + " Ask Imbalance Zone", NinjaTrader.Core.Globals.InstallDir+@"\sounds\"+pWAVFileZone,1,pArrowUpFBrush,Brushes.White);
								LastAudioBar3 = CurrentBars[0];
								
								//BackBrushes[0] = Brushes.Green;
								
							}
							
							//if (ZoneItems[BB] != null)
							if(ZoneItems[BB].ResistanceZones.Count != 0 && LastAudioBar4 != CurrentBars[0])
							{
								Alert(CurrentBar.ToString(),Priority.High, BarsArray[0].BarsPeriod + " | " + NName + " Bid Imbalance Zone", NinjaTrader.Core.Globals.InstallDir+@"\sounds\"+pWAVFileZone2,1,pArrowDownFBrush,Brushes.White);
								LastAudioBar4 = CurrentBars[0];
								
								//BackBrushes[0] = Brushes.Red;
							}
						}
						
//						if (pAudioEnabled2)
//						{
//							//if (ZoneItems[BB] != null)
//							if(ZoneItems[BB].SupportZones.Count != 0 && LastAudioBar5 != CurrentBars[0])
//							{
//								Alert(CurrentBar.ToString(),Priority.High, BarsPeriod + " | " + NName + " Ask Momentum Zone", NinjaTrader.Core.Globals.InstallDir+@"\sounds\"+pWAVFileZone,1,pArrowUpFBrush,Brushes.White);
//								LastAudioBar3 = CurrentBars[0];
								
//								//BackBrushes[0] = Brushes.Green;
								
//							}
							
//							//if (ZoneItems[BB] != null)
//							if(ZoneItems[BB].ResistanceZones.Count != 0 && LastAudioBar6 != CurrentBars[0])
//							{
//								Alert(CurrentBar.ToString(),Priority.High, BarsPeriod + " | " + NName + " Bid Momentum Zone", NinjaTrader.Core.Globals.InstallDir+@"\sounds\"+pWAVFileZone2,1,pArrowDownFBrush,Brushes.White);
//								LastAudioBar4 = CurrentBars[0];
								
//								//BackBrushes[0] = Brushes.Red;
//							}
//						}						
						
						
						
					}
					
					
					
		

		

		
					// Fixed: was a for-loop iterating ALL bars but loop variable 'i' was never used inside.
					// Only the current bar needs to be checked for magnet/UFA alerts.
					if (pAudioEnabled5 || pAudioEnabled6)
					{
						if (BarItems.IsValidDataPointAt(CurrentBars[0]))
						{
							BarItem BBBBB = BarItems.GetValueAt(CurrentBars[0]);

							if (pAudioEnabled6)
							if (LastAudioBar6 != CurrentBars[0])
							if (BBBBB.Magnets.Count > 0)
							{
								Alert(CurrentBar.ToString(),Priority.High, BarsPeriod + " | " + NName + " Magnet", NinjaTrader.Core.Globals.InstallDir+@"\sounds\"+pWAVFileName6,1,pMagnetStroke.Brush,GetTextColor(pMagnetStroke.Brush));
								LastAudioBar6 = CurrentBars[0];
							}

							if (pAudioEnabled5)
							if (LastAudioBar5 != CurrentBars[0])
							if (BBBBB.UnfinishedAuctions.Count > 0)
							{
								Alert(CurrentBar.ToString(),Priority.High, BarsPeriod + " | " + NName + " Unfinished Auction", NinjaTrader.Core.Globals.InstallDir+@"\sounds\"+pWAVFileName5,1,pUFAStroke.Brush,GetTextColor(pUFAStroke.Brush));
								LastAudioBar5 = CurrentBars[0];
							}

						}
					}
					
					
				}
				
			
				
				
				// order execution
				
 

				
				
				
				
				// order execution
				
				IsNowRealTime = false;
				
				if (!IsTickReplay)
				if (BarsArray[0].Count == CurrentBars[0]+1)
				{
					//Print("IS GOING REAL TIME");	
					IsNowRealTime = true;
				}
				
				
				
				//if (pOrderPanelOn)
				if (State == State.Realtime || IsNowRealTime)
				{
					
					
					
					
					if (!IsTickReplay)
					CurrentLast = Closes[1][0];			
			
					
					if (IsFirstTickOfBar)	
						SetOrderFlags();
						
				
	
					//UpdateQTYBox(ThisPositionNow());
					
					//if (ThisPositionNow() != null)
					//UpdatePNLBox(ThisPositionNow());
					
					BB = 0;
					
					int pBarsBack = 10;
					int pTicksFromHL = 8;
					
					bool LongAutoOK = CurrentEMATrend[0] == 1 && High[0] <= MAX(High,pBarsBack)[0] - TickSize * pTicksFromHL;
					bool ShortAutoOK = CurrentEMATrend[0] == -1 && Low[0] >= MIN(Low,pBarsBack)[0] + TickSize * pTicksFromHL;
					bool AutoIsOK = false;
					
					if (!pAutoEnabled)
					{
						LongAutoOK = true;
						ShortAutoOK = true;
						
					}

					
					
					
					
					
					
					//Print("LongAutoOK = " + LongAutoOK);
					//Print("ShortAutoOK = " + ShortAutoOK);
					
	
					//if (BuyStackReady)
				    {
						//if (LastBarLong != CurrentBars[0])
						if (ZoneItems[BB] != null)
						{
							
//							BuyStackGo = ZoneItems[BB].SupportZones.Count != 0;

							BuyStackGo = false;
							
							if (pBuyStackMode == "Long")
								BuyStackCount = ZoneItems[BB].SupportZones.Count;
							else
								BuyStackCount = ZoneItems[BB].ResistanceZones.Count;
							
							if (BuyStackCount > BuyStackCountP)
								BuyStackGo = true;
							
							Values[19][0] = 0;
							if (BuyStackGo)
								Values[19][0] = 1;
							
							
							BuyStackCountP = BuyStackCount;
							
							if (BuyStackReady)
							if (BuyStackGo)
							{
								
								AutoIsOK = ThisPositionNow() == null && LongAutoOK;
								
								if (!pAutoEnabled)
									AutoIsOK = true;
								
								//Print("AutoIsOK = " + AutoIsOK);
								
								if (AutoIsOK)
								{
									//if (!pAutoEnabled)
										
									BuyStackReady = false;	
									SellStackReady = false;
									
									
									BuyStackGo = false;
									
									UpdateButtons();
									
									LastBarLong = CurrentBars[0];

										
									BuyNow("Stack");
										
								}
									
								//Print(ThisMasterInstrument.Name + " Buy Stack Entry");
								
								
							}
							
							
						}
				        
				    }
					
					//if (SellStackReady)
				    {
						//if (LastBarShort != CurrentBars[0])
						if (ZoneItems[BB] != null)
						{
							SellStackGo = false;
							
							if (pSellStackMode == "Long")
								SellStackCount = ZoneItems[BB].SupportZones.Count;
							else
								SellStackCount = ZoneItems[BB].ResistanceZones.Count;
					
							if (SellStackCount > SellStackCountP)
								SellStackGo = true;
							
							Values[20][0] = 0;
							if (SellStackGo)
								Values[20][0] = -1;
							
							SellStackCountP = SellStackCount;
							
							if (SellStackReady)
							if (SellStackGo)
							{
							
								AutoIsOK = ThisPositionNow() == null && ShortAutoOK;
								
								if (!pAutoEnabled)
									AutoIsOK = true;
								
								if (AutoIsOK)
								{
									
									//if (!pAutoEnabled)
									BuyStackReady = false;	
									SellStackReady = false;
									
									
									SellStackGo = false;
									
									UpdateButtons();
									
									LastBarShort = CurrentBars[0];
									

									SellNow("Stack");
										
									
								}
								//Print(ThisMasterInstrument.Name + " Sell Stack Entry");
								
								
							}
							
							
						}
				        
				    }
					
					
					
					
					
					
					
					
					
			
					
					if (BuyCloseReady)
				    {
						//if (LastBarLong != CurrentBars[0])
						if (IsFirstTickOfBar)
						{
							
							bool SendTrade = false;
					
							if (pBuyCloseMode == "Long")
							{
								if (Direction2[1] == 1)
									SendTrade = true;
							}
							else if (pBuyCloseMode == "Short")
							{
								if (Direction2[1] == -1)
									SendTrade = true;						
							}
							else
							{
								SendTrade = true;
							}
			
							if (SendTrade)
							{
		
								BuyCloseReady = false;	
								UpdateButtons();

								BuyNow("Close");
							}
									
							
						}
				        
				    }
					
					if (SellCloseReady)
				    {
						if (IsFirstTickOfBar)
						{
					
							bool SendTrade = false;
					
							if (pSellCloseMode == "Long")
							{
								if (Direction2[1] == 1)
									SendTrade = true;
							}
							else if (pSellCloseMode == "Short")
							{
								if (Direction2[1] == -1)
									SendTrade = true;						
							}
							else
							{
								SendTrade = true;
							}
			
							if (SendTrade)
							{
		
								SellCloseReady = false;
								UpdateButtons();
							
								SellNow("Close");
							}
							
							
							
						
										
							
						}
				        
				    }
					
					
					
					
					
			
				
					
					int BBBB = 1;
					bool FTOOOB = IsFirstTickOfBar;
					
					if (pWashoutEntryOrdersIB) 
					{
						FTOOOB = true;
						BBBB = 0;
					}
		

			
				
					//if (pWashoutEntryOrdersIB)
					{

						
					
						bool SendBuyTrade = false;
						bool SendSellTrade = false;
						
						
						if (BuyWashoutReady)
					    {
							//if (LastBarLong != CurrentBars[0])
							
							if (FTOOOB)
							{
							
								if (pBuyWashoutMode == "Long")
								{
									if (LongSignals[BBBB] != 0)
									//if (PreviousLongSignals == 0 && CurrentLongSignals != 0)
										SendBuyTrade = true;
								}
								else if (pBuyWashoutMode == "Short")
								{
									if (ShortSignals[BBBB] != 0)
									//if (PreviousShortSignals == 0 && CurrentShortSignals != 0)
										SendBuyTrade = true;						
								}
							
				
							}
					        
					    }
						
						if (SellWashoutReady)
					    {
							
								
							if (FTOOOB)
							{						
								if (pSellWashoutMode == "Long")
								{
									if (LongSignals[BBBB] != 0)
									//if (PreviousLongSignals == 0 && CurrentLongSignals != 0)
										SendSellTrade = true;
								}
								else if (pSellWashoutMode == "Short")
								{
									if (ShortSignals[BBBB] != 0)
									//if (PreviousShortSignals == 0 && CurrentShortSignals != 0)
										SendSellTrade = true;						
								}

							}
								
								
							
					        
					    }
						
						
						if (SendBuyTrade && SendSellTrade)
						{
							
										BuyWashoutReady = false;	
									SellWashoutReady = false;
							SendBuyTrade = false;
							SendSellTrade = false;
							
						}
						
						
								if (SendBuyTrade)
								{
			
									BuyWashoutReady = false;	
									SellWashoutReady = false;
									UpdateButtons();

									BuyNow("Washout");
								}
										
							
								if (SendSellTrade)
								{
			
									BuyWashoutReady = false;	
									SellWashoutReady = false;
									UpdateButtons();
								
									SellNow("Washout");
								}
								
								
				}
//				else
//				{
					
//					if (BuyWashoutReady)
//					    {
//							//if (LastBarLong != CurrentBars[0])
							
							
							
//							if (IsFirstTickOfBar)
//							{
								
//								bool SendTrade = false;
						
//								if (pBuyWashoutMode == "Long")
//								{
//									if (LongSignals[BBBB] != 0)
//										SendTrade = true;
//								}
//								else if (pBuyWashoutMode == "Short")
//								{
//									if (ShortSignals[BBBB] != 0)
//										SendTrade = true;						
//								}
							
				
//								if (SendTrade)
//								{
			
//									BuyWashoutReady = false;	
//									SellWashoutReady = false;
//									UpdateButtons();

//									BuyNow("Washout");
//								}
										
								
//							}
					        
//					    }
						
//						if (SellWashoutReady)
//					    {
//							if (IsFirstTickOfBar)
//							{
						
//								bool SendTrade = false;
						
//								if (pSellWashoutMode == "Long")
//								{
//									if (LongSignals[BBBB] != 0)
//										SendTrade = true;
//								}
//								else if (pSellWashoutMode == "Short")
//								{
//									if (ShortSignals[BBBB] != 0)
//										SendTrade = true;						
//								}
							
				
//								if (SendTrade)
//								{
			
//									BuyWashoutReady = false;	
//									SellWashoutReady = false;
//									UpdateButtons();
								
//									SellNow("Washout");
//								}
								
								
								
							
											
								
//							}
					        
//					    }
						
						
//				}
				
					
					
					
					
					
				} // end state = real time
				
				
				
				
			
				
			}  // end bars in progress 0
			
			
			
			
			
			
				
				
				
				
		
			
			
				

        }
		
		
		
		
		
 //this on bar update doesn't match the volumetric bars		
		
		
		
		
		
		
		
//		protected override void OnBarUpdate()
//		{
//			if (!Permission)
//				return;		
			

			
////			if (TestBeta)
////				return;
//			// timer

			
//			// range
			
//			if (BarsArray == null || BarsArray.Length == 0)
//				return;

			
//			if (BarsInProgress == 0)
//			{
			
//				if (BarsArray[0].BarsType.BuiltFrom == BarsPeriodType.Tick && BarsArray[0].BarsPeriod.ToString().IndexOf("Range") >= 0)
//					isRangeDerivate = true;
		
//				if (isRangeDerivate)
//				{
//					double high = High.GetValueAt(Bars.Count - 1 - (Calculate == NinjaTrader.NinjaScript.Calculate.OnBarClose ? 1 : 0));
//					double low = Low.GetValueAt(Bars.Count - 1 - (Calculate == NinjaTrader.NinjaScript.Calculate.OnBarClose ? 1 : 0));
//					double close = Close.GetValueAt(Bars.Count - 1 - (Calculate == NinjaTrader.NinjaScript.Calculate.OnBarClose ? 1 : 0));
//					int actualRange = (int)Math.Round(Math.Max(close - low, high - close) / Bars.Instrument.MasterInstrument.TickSize);
//					double rangeCount = pCountDown ? BarsPeriod.Value - actualRange : actualRange;

//					string label = string.Empty; //NinjaTrader.Custom.Resource.RangeCounterRemaing;  //NinjaTrader.Custom.Resource.RangerCounterCount
					
//			string range1 = (BarsPeriod.BarsPeriodType == BarsPeriodType.Range || isRangeDerivate ? CountDown
//				? string.Format(NinjaTrader.Custom.Resource.RangeCounterRemaing, rangeCount) : string.Format(NinjaTrader.Custom.Resource.RangerCounterCount, rangeCount)
//				: NinjaTrader.Custom.Resource.RangeCounterBarError);
					
					
			
//					range1 = (BarsPeriod.BarsPeriodType == BarsPeriodType.Range || isRangeDerivate ? CountDown
//				? string.Format(label, rangeCount) : string.Format(label, rangeCount)
//				: NinjaTrader.Custom.Resource.RangeCounterBarError);
					
					
//					BarTimerString = range1;			
//					BarTimerString = rangeCount.ToString();
					
//					//Draw.TextFixed(this, "NinjaScriptInfo", range1, TextPosition.BottomRight);					
						
//				}
//				else if (BarsPeriod.BarsPeriodType == BarsPeriodType.Tick)
//				{
					
//					// tick
					
//					double periodValue = (BarsPeriod.BarsPeriodType == BarsPeriodType.Tick) ? BarsPeriod.Value : BarsPeriod.BaseBarsPeriodValue;
//					double tickCount = pShowPercent ? pCountDown ? (1 - Bars.PercentComplete) * 100 : Bars.PercentComplete * 100 : pCountDown ? periodValue - Bars.TickCount : Bars.TickCount;

//					string label = NinjaTrader.Custom.Resource.TickCounterTicksRemaining;  //NinjaTrader.Custom.Resource.TickCounterTickCount
//					label = string.Empty;
					
//					string tick1 = (BarsPeriod.BarsPeriodType == BarsPeriodType.Tick 
//								|| ((BarsPeriod.BarsPeriodType == BarsPeriodType.HeikenAshi || BarsPeriod.BarsPeriodType == BarsPeriodType.Volumetric) && BarsPeriod.BaseBarsPeriodType == BarsPeriodType.Tick) ? ((pCountDown
//												? label + tickCount : label + tickCount) + (pShowPercent ? "%" : ""))
//												: NinjaTrader.Custom.Resource.TickCounterBarError);

//					BarTimerString = tick1;
					
//					//Draw.TextFixed(this, "NinjaScriptInfo", tick1, TextPosition.BottomRight, ChartControl.Properties.ChartText, ChartControl.Properties.LabelFont, Brushes.Transparent, Brushes.Transparent, 0);			
					
									
					
//				}
//				else if (BarsPeriod.BarsPeriodType == BarsPeriodType.Volume)
//				{
					
//					// volume
					
//					volume = (long)Volume[0];

//					double volumeCount = pShowPercent
//						? pCountDown
//							? (1 - Bars.PercentComplete) * 100
//							: Bars.PercentComplete * 100
//						: pCountDown
//							? (isVolumeBase
//								? BarsPeriod.BaseBarsPeriodValue
//								: BarsPeriod.Value) - volume
//							: volume;

//					string label = NinjaTrader.Custom.Resource.VolumeCounterVolumeRemaining; // NinjaTrader.Custom.Resource.VolumeCounterVolumeCount
//					label = string.Empty;
					
//					string volume1 = (isVolume || isVolumeBase)
//						? ((pCountDown
//							? label + volumeCount
//							: label + volumeCount) + (pShowPercent ? "%" : ""))
//						: NinjaTrader.Custom.Resource.VolumeCounterBarError;

//					BarTimerString = volume1;
					
//					//Draw.TextFixed(this, "NinjaScriptInfo", volume1, TextPosition.BottomRight);				
					
//				}
//				else
//				{
					
					
					
//				}
			
			
//			}
			
//			// end counter
			
			

			
				

//			if (BarsInProgress == 2) // Ask
//			{
//				CurrentAsk = Closes[2][0];
//				return;
//			}
			
//			if (BarsInProgress == 3) // Bid
//			{
//				CurrentBid = Closes[3][0];
//				return;				
				
//			}			
			

//			// remove this and crashes 30 second chart
//			if (CurrentBars[0] < 1 || CurrentBars[1] < 1)
//				return;
			
			
			
//			if (State == State.Realtime)
//			{
//				hasRealtimeData = true;
//				connected = true;
//			}
			
// 			if (CurrentBars[0] < 0)
//				return;
			
//			if (!IsTickReplay)
//			if (CurrentBars[1] < 0)
//				return;
			
//			if (BarsInProgress == 0)
//			{
//				BodyHigh[0] = Math.Max(Close[0], Open[0]);
//				BodyLow[0] = Math.Min(Close[0], Open[0]);
				
//				if (Close[0] > Open[0])
//				{
//					Direction[0] = 1;
//					Direction2[0] = 1;
//				}
//				else if (Close[0] < Open[0])
//				{
//					Direction[0] = -1;
//					Direction2[0] = -1;
//				}	
//				else
//				{
//					if (CurrentBars[0] == 0)
//						Direction[0] = 1;
//					else
//						Direction[0] = Direction[1];
					
//					Direction2[0] = 0;
//				}
					
//				if (pCompLevelsPriceOn)
//				{	
//					PlotBrushes[2][0] = pPlot1Brush;
//					PlotBrushes[3][0] = pPlot2Brush;
//					PlotBrushes[4][0] = pPlot3Brush;
//					PlotBrushes[5][0] = pPlot4Brush;
//					PlotBrushes[6][0] = pPlot5Brush;
//					PlotBrushes[7][0] = pPlot6Brush;
//					PlotBrushes[17][0] = pPlot7Brush;
//					PlotBrushes[18][0] = pPlot8Brush;					
					
//				}
				
		
//			} 
			
			

			
//			//if (State == State.Realtime)
//			//Print(BarsInProgress + "  " + CurrentBar + " - " + Volume[0]);

			
			
			
////			if (BarsInProgress == 0)
////			{
////				ThisEMA1[0] = EMA(20)[0];
////				ThisEMA2[0] = EMA(50)[0];
////			}
			
			
			
			
			
//			// SCALE
//			if (TestLoad)
//			{
//				if (!IsTickReplay)
//				if (State == State.Historical)
//				{
//					BarsProcessed = CurrentBars[1];
					
//					//if (CurrentBars[1] == 0)
//					BarsRemaining = BarsArray[1].Count;
//				}
//			}
				
			
			
//			if (BarsInProgress == 0)
//			{
				
				
				
				
//				if (IsFirstTickOfBar)
//				{
					
//					ForceHighPriority = false;	
//					ForceLowPriority = false;
//				}
				
				
//				if (CurrentBar == 0)
//				{
//					ScaleHigh[0] = High[0];
//					ScaleLow[0] = High[0]-pScaleTicks*TickSize;
					
//				}
//				else
//				{
		
//					SetThisScale(0);
//				}
				

//			}
			
		
			
//			int BBHist = 0;
			
			
			
			
					
//			if (BarsInProgress == 1) 
//			{
//				LastTickTime = Times[1][0];
//				//return;				
				
//			}			
			

//			if (pDataCalcMode2 == "Volumetric")
//			{
				
				

////	 INDIVIDUAL TICKS VOLUMETRICS
				
//			//
//			//	if (!IsTickReplay)
//				if (BarsInProgress == 1)
//				{
					
			
					
//					if(BarsArray[1].Count < 0)   { return; }
//					if(CurrentBars[1] < 0)   { return; }
//					if(Times[1][0] < getDate) { return; }
					

//					//if(e.MarketDataType == MarketDataType.Last)
					
					
					
					
					

//					NinjaTrader.NinjaScript.BarsTypes.VolumetricBarsType barsType = BarsArray[1].BarsType as 
//	       			NinjaTrader.NinjaScript.BarsTypes.VolumetricBarsType;
						
					
//		        if (barsType == null)
//	          return;
	        
				
//				//if (State == State.Historical)
//				//	return;
				
//		       // This sample assumes the Volumetric series is the primary DataSeries on the chart, if you would want to add a Volumetric series to a   
//		       // script, you could call AddVolumetric() in State.Configure and then for example use
//		       // NinjaTrader.NinjaScript.BarsTypes.VolumetricBarsType barsType = BarsArray[1].BarsType as 
//		       // NinjaTrader.NinjaScript.BarsTypes.VolumetricBarsType;
		 
////		        NinjaTrader.NinjaScript.BarsTypes.VolumetricBarsType barsType = Bars.BarsSeries.BarsType as     
////		        NinjaTrader.NinjaScript.BarsTypes.VolumetricBarsType;
		        
////		        if (barsType == null)
////		          return;
		 
////		        try
////		        {
////		          double price;
////		          Print("=========================================================================");
////		          Print("Bar: " + CurrentBar);
////		          Print("Trades: " + barsType.Volumes[CurrentBar].Trades);
////		          Print("Total Volume: " + barsType.Volumes[CurrentBar].TotalVolume);
////		          Print("Total Buying Volume: " + barsType.Volumes[CurrentBar].TotalBuyingVolume);
////		          Print("Total Selling Volume: " + barsType.Volumes[CurrentBar].TotalSellingVolume);
////		          Print("Delta for bar: " + barsType.Volumes[CurrentBar].BarDelta);
////		          Print("Delta for bar (%): " + barsType.Volumes[CurrentBar].GetDeltaPercent());
////		          Print("Delta for Close: " + barsType.Volumes[CurrentBar].GetDeltaForPrice(Close[0]));
////		          Print("Ask for Close: " + barsType.Volumes[CurrentBar].GetAskVolumeForPrice(Close[0]));
////		          Print("Bid for Close: " + barsType.Volumes[CurrentBar].GetBidVolumeForPrice(Close[0]));
////		          Print("Volume for Close: " + barsType.Volumes[CurrentBar].GetTotalVolumeForPrice(Close[0]));
////		          Print("Maximum Ask: " + barsType.Volumes[CurrentBar].GetMaximumVolume(true, out price) + " at price: " + price);
////		          Print("Maximum Bid: " + barsType.Volumes[CurrentBar].GetMaximumVolume(false, out price) + " at price: " + price);
////		          Print("Maximum Combined: " + barsType.Volumes[CurrentBar].GetMaximumVolume(null, out price) + " at price: " + price);
////		          Print("Maximum Positive Delta: " + barsType.Volumes[CurrentBar].GetMaximumPositiveDelta());
////		          Print("Maximum Negative Delta: " + barsType.Volumes[CurrentBar].GetMaximumNegativeDelta());
////		          Print("Max seen delta (bar): " + barsType.Volumes[CurrentBar].MaxSeenDelta);
////		          Print("Min seen delta (bar): " + barsType.Volumes[CurrentBar].MinSeenDelta);
////		          Print("Cumulative delta (bar): " + barsType.Volumes[CurrentBar].CumulativeDelta);
////		        }
////		        catch{}
				
				
			
					
//						//ask = e.Ask;
//						//bid = e.Bid;
////						cls = Closes[1][0];
////						vol = Volumes[1][0];
						
	
//				double LastPrice = Highs[1][0];
//				double LowPrice = Lows[1][0];
				
	
//				//if (BI == null)
//						BI = new BarItem();
				
					
//				//Print("--------- " + CurrentBar);
				
//				do
//					{			

//						//Print(LastPrice);
						
//						double cls = LastPrice;
							
//						if(!BI.l.ContainsKey(cls))
//						{
//							BI.l.TryAdd(cls, new RowData());
//						}
						
		      
//						BI.l[cls].tv = barsType.Volumes[CurrentBar].GetTotalVolumeForPrice(cls);
//						BI.l[cls].av = barsType.Volumes[CurrentBar].GetAskVolumeForPrice(cls);
//						BI.l[cls].bv = barsType.Volumes[CurrentBar].GetBidVolumeForPrice(cls);
//						BI.l[cls].dv = barsType.Volumes[CurrentBar].GetDeltaForPrice(cls);
						
				
					
					
//						LastPrice = RTTS(LastPrice - ThisTickSizze);
//					} 
//					while (LastPrice >= LowPrice);	
					
					
					
						

				

					
//						BI.dv = barsType.Volumes[CurrentBar].BarDelta;
//						BI.av = barsType.Volumes[CurrentBar].TotalBuyingVolume;
//						BI.bv = barsType.Volumes[CurrentBar].TotalSellingVolume;
//						BI.tv = barsType.Volumes[CurrentBar].TotalVolume;
						
					

						
//						// block trades
						
////						if (vol >= pBlockSize)
////						{
////							if(CurrentTrend == -1)
////							{
////								BI.BidBlocks.TryAdd(cls, new List<double>());
////								BI.BidBlocks[cls].Add(vol);
////							}
////							else
////							{
////								BI.AskBlocks.TryAdd(cls, new List<double>());
////								BI.AskBlocks[cls].Add(vol);
////							}
							
							
////						}
						
			 
					
//					//	Print(CurrentBars[0] + "   " + CurrentBars[1]);
						
					
//						int ii = 0;
						
						
//						BarItems[ii] = BI;
		
////					ProcessOK = LastTickTime >= LastProcess5Time.AddMilliseconds(pProcessFrequencyMS);	
////					if (BuyStackReady || SellStackReady)
////						ProcessOK = true;
					
////					if (ProcessOK)			
////					{
////						LastProcess5Time = LastTickTime; 
						
//					//if (State == State.Realtime)
//					//{
//						calculateBar(ii);
//						calculateProfile(ii);					
//					//}
					
					
////					}
					

					
					
					
////					if (RealTimeReset)
////					{
////						int ii = 0;
						
						
////						BarItems[ii] = BI;
//						//calculateBar(ii);
////						calculateProfile(ii);
						
////						//Print("RTS");
						
////					}
					
					
//				}				
				
				
				
					
//			}
		
			

			
//			int BBBaBB = 0;
			
////			if (pDataCalcMode2 == "Volumetric")	
////				BBBaBB = 1;
			
			
//			if (BarsInProgress == BBBaBB)
//			{		
				
//				// EMA TREND
//				if (CurrentBars[0] > 0)
//				{
//					if (TrendEMA[0] > TrendEMA[1])
//						CurrentEMATrend[0] = 1;
//					else
//						CurrentEMATrend[0] = -1;
//				}
				
				
				
//				// SWING LEVELS
				
//				IsCurrentBar = State == State.Realtime;
				
//				FinalHigh[0] = Highs[0][BBHist];
//				FinalLow[0] = Lows[0][BBHist];
//				//AllPivots[0] = 0;
			
//				if (WaitingForHigh && IsCurrentBar)
//				{
					
//					if (HighLA.ContainsKey(CurrentHighBar))
//						HighLA.Remove(CurrentHighBar);
					
//				}
//				if (WaitingForLow && IsCurrentBar)
//				{
					
//					if (LowLA.ContainsKey(CurrentLowBar))
//						LowLA.Remove(CurrentLowBar);
					
			
					
//				}
				



//				if (WaitingForHigh)
//				{

//					if (FinalHigh[0] > CurrentHighPrice)
//					{
//						if (CurrentBar > 1)
//						{
	
//							AllPivots[CurrentBar-CurrentHighBar] = 0;
							
//						}
					

						
//						CurrentHighPrice = FinalHigh[0];
//						CurrentHighBar = CurrentBar-BBHist;
			
						
//					}
					
					
					
					
//					if (CurrentHighBar != CurrentBar && FinalLow[0] <= CurrentHighPrice - pTicksMove*TickSize+TickSize*0.5)
//					{

//						AllPivots[CurrentBar-CurrentHighBar] = 1;

//						WaitingForLow = true;
//						WaitingForHigh = false;
						
//						CurrentLowPrice = FinalLow[0];
//						CurrentLowBar = CurrentBar-BBHist;
						

//						if (!HighLA.ContainsKey(CurrentHighBar))
//							HighLA.Add(CurrentHighBar,CurrentHighPrice);					
						
						
//					}
					
//				}
				
//				if (WaitingForLow)
//				{

//					if (FinalLow[0] < CurrentLowPrice)
//					{
//						if (CurrentBar > 1)
//						{
						
//							AllPivots[CurrentBar-CurrentLowBar] = 0;
//						}
					
						
//						CurrentLowPrice = FinalLow[0];
//						CurrentLowBar = CurrentBar-BBHist;
						
				
//					}
					
					
					
					
//					if (CurrentLowBar != CurrentBar && FinalHigh[0] >= CurrentLowPrice + pTicksMove*TickSize-TickSize*0.5)
//					{

//						AllPivots[CurrentBar-CurrentLowBar] = -1;
		
//						WaitingForLow = false;
//						WaitingForHigh = true;
						
//						CurrentHighPrice = FinalHigh[0];
//						CurrentHighBar = CurrentBar-BBHist;
						

//							if (!LowLA.ContainsKey(CurrentLowBar))
//						LowLA.Add(CurrentLowBar,CurrentLowPrice);			
						
//					}				
				
					
					
//				}

			
			
						
//				if (IsCurrentBar)
//				{
					
			
					
//					if (WaitingForHigh)
//					{

//						AllPivots[CurrentBar-CurrentHighBar] = 1;

//						if (!HighLA.ContainsKey(CurrentHighBar))
//							HighLA.Add(CurrentHighBar,CurrentHighPrice);	
						
					
//					}
//					if (WaitingForLow)
//					{

//						AllPivots[CurrentBar-CurrentLowBar] = -1;


						
//						if (!LowLA.ContainsKey(CurrentLowBar))
//							LowLA.Add(CurrentLowBar,CurrentLowPrice);	
						
//					}
				
					
				
					
//				}
				
				
				
				
//				//ProcessOK = LastTickTime >= LastProcess1Time.AddMilliseconds(pProcessFrequencyMS);
				
				
//				//return;
				
			
//				ProcessOK = true;
				
//				if (ProcessOK)

//				{
//					//Print("Process 1");
//					LastProcess1Time = LastTickTime; 
					
					
//					DeleteLA.Clear();	

					
//					foreach (KeyValuePair<int,double> L in HighLA)
//					{
//						if (Highs[0][BBHist] > L.Value)
//						{
//							DeleteLA.Add(L.Key,L.Value);
//						}
//					}	
					
//					foreach (KeyValuePair<int,double> L in DeleteLA)
//					{
//						HighLA.Remove(L.Key);
//						//if (pHistoricalLevelsEnabled) 
						
//						if (!HighLF.ContainsKey(L.Key))
//						HighLF.Add(L.Key,CurrentBars[0]-BBHist);
//					}
					
//					DeleteLA.Clear();

//					foreach (KeyValuePair<int,double> L in LowLA)
//					{
//						//Print(L.Value.Price);
						
//						if (Lows[0][BBHist] < L.Value)
//						{
//							//if (!DeleteLA.ContainsKey(L.Key))
//								DeleteLA.Add(L.Key,L.Value);
							
//						}
//					}	
					
//					foreach (KeyValuePair<int,double> L in DeleteLA)
//					{
//						LowLA.Remove(L.Key);
						
//						if (!LowLF.ContainsKey(L.Key))
//						LowLF.Add(L.Key,CurrentBars[0]-BBHist);
//					}
					
					
					
					
					
//					// CLOSE MAGNETS AND UNFINISHED AUCTIONS
					
					
//	//				if (BarsInProgress == 0)
//	//				{
						
//	//					Print(CurrentBar);
						
						
//	//				}
					
					
//				}
					
				
				
			
				
//				//ProcessOK = true;
//				//ProcessOK = LastTickTime >= LastProcess2Time.AddMilliseconds(pProcessFrequencyMS);				
//				if (ProcessOK)
//				{
//					//Print("Process 1");
//					LastProcess2Time = LastTickTime; 
					
					
				
//					//BBHist = 0;
					
//					for (int i = 1; i < CurrentBars[0]-0-BBHist; i++)
//					{
//						if (BarItems.IsValidDataPointAt(i))
//						{							
//							BarItem BBBBB = BarItems.GetValueAt(i);
							
							
//							if (BBBBB.Magnets != null)
//							foreach (Level kvp2 in BBBBB.Magnets)
//							{
//								int closedbar = kvp2.EndBar;
//								double price = kvp2.Price;

//								if (closedbar == 0)
//								{		
//									if (Highs[0][BBHist] >= price && Lows[0][BBHist] <= price)
//									{
										
//										//Print(price);
//								//Print(Highs[0][BBHist] + "  " + Lows[0][BBHist]);
										
//										kvp2.EndBar = CurrentBars[0]-BBHist;			
//									}
//								}
								
//							}
							
//							if (BBBBB.UnfinishedAuctions != null)
//							foreach (Level kvp2 in BBBBB.UnfinishedAuctions)
//							{
//								int closedbar = kvp2.EndBar;
//								double price = kvp2.Price;

								

								
//								if (closedbar == 0)
//								{		
//									if (Highs[0][BBHist] >= price && Lows[0][BBHist] <= price)
//										kvp2.EndBar = CurrentBars[0]-BBHist;		
									
																
									
//								}
								
//							}						
							
//						}
//					}
					
					
				
				
//				}
			
				
				
				
				
//				// update Zones (Momentum) 
						
////				ProcessOK = LastTickTime >= LastProcess3Time.AddMilliseconds(pProcessFrequencyMS);	
////				if (BuyStackReady || SellStackReady)
////					ProcessOK = true;
				
//				if (ProcessOK)			
//				if (CurrentBars[0] > 2)
//				{
//					LastProcess3Time = LastTickTime; 
					
					
//				//BodyHigh[0] = RTTS(Math.Max(Close[0],Open[0]));
//				//BodyLow[0] = RTTS(Math.Min(Close[0],Open[0]));
				
				
				
//				Zone Z;
//				int bbbb = 1;
				
				
//				//ZoneItems[bbbb] = null;
				
////				ZoneItems[bbbb] = new ZoneItem();
////				ZoneItems[bbbb].SupportZones2 = new List<Zone>();
////				ZoneItems[bbbb].ResistanceZones2 = new List<Zone>();
				
				
//				if (Low[1] <= High[2] && Low[0] <= High[1])
//				if (High[2] + RTTS((pMinZWidth2-1)*TickSize) <= Low[0])
//				{
						
////					if (!ZoneItems.IsValidDataPoint(bbbb))
////					{
////						ZoneItems[bbbb] = new ZoneItem();
						
////					}
					
////					ZoneItems[bbbb].SupportZones2 = new List<Zone>();
////					ZoneItems[bbbb].ResistanceZones2 = new List<Zone>();
					
//					//ZoneItems[bbbb] = new ZoneItem();
//					ZoneItems[bbbb].SupportZones2 = new List<Zone>();
//					ZoneItems[bbbb].ResistanceZones2 = new List<Zone>();
					
//					Z = new Zone();
//					Z.BottomPrice = High[2];
//					Z.TopPrice = Low[0];
//					Z.TicksWidth = (long) RTTS((Low[0]-High[2])/TickSize) + 1;
//					Z.TestedPrice = 0;
//					Z.IsBroken = false;
//					Z.IsHidden = false;
//					Z.EndBar = 0;

//					ZoneItems[bbbb].SupportZones2.Add(Z);	
//				}
				
				
				
//				if (High[1] >= Low[2] && High[0] >= Low[1])
//				if (Low[2] - RTTS((pMinZWidth2-1)*TickSize) >= High[0])
//				{
					
////					if (!ZoneItems.IsValidDataPoint(bbbb))
////					{
////						ZoneItems[bbbb] = new ZoneItem();
						
////					}
					
////					ZoneItems[bbbb].SupportZones2 = new List<Zone>();
////					ZoneItems[bbbb].ResistanceZones2 = new List<Zone>();
					
//					//ZoneItems[bbbb] = new ZoneItem();
//					ZoneItems[bbbb].SupportZones2 = new List<Zone>();
//					ZoneItems[bbbb].ResistanceZones2 = new List<Zone>();
					
//					Z = new Zone();
//					Z.BottomPrice = High[0];
//					Z.TopPrice = Low[2];
//					Z.TicksWidth = (long) RTTS((Low[2]-High[0])/TickSize) + 1;
//					Z.TestedPrice = 0;
//					Z.IsBroken = false;
//					Z.IsHidden = false;
//					Z.EndBar = 0;
					
//					ZoneItems[bbbb].ResistanceZones2.Add(Z);	
//				}
				
				
				
				
				
//				// TERMINATE ZONES
				
////				bool IsCurrentBar = false;
////				if (BarsArray[0].Count == CurrentBars[0]+1)
////					{
////						//Print("IS GOING REAL TIME");	
////						IsCurrentBar = true;
////					}
				
//					//if (IsCurrentBar)
//					//{
						
						
//					//foreach (ZoneItem zzz in ZoneItems)	

					
				
				
//					for (int i = 1; i < CurrentBars[0]; i++)
//					//foreach ()	
						
//					//foreach (KeyValuePair<int, List<Zone>> DZ in ResistanceZones2)
//					{
//						if (ZoneItems.IsValidDataPoint(i))
//						{
						
//							ZoneItem zzz = ZoneItems.GetValueAt(i);
							

//							if (zzz == null)
//								return;
							
//							bool DOlNO = true;
//							bool DOlYES = true;
							
					
//							if (DOlYES)
//							if (zzz.ResistanceZones2 != null)
//							{
								
//							}
							
//							if (DOlYES)
//							foreach (Zone ZS in zzz.ResistanceZones2)
//							{
								
								
								
								
								
//								//TotalNumberOfZones = TotalNumberOfZones + 1;
//								//TotalWidthOfAllZones = TotalWidthOfAllZones + Math.Round((ZS.TopPrice - ZS.BottomPrice)/TickSize,0);
								
//								if (ZS.IsBroken)
//									continue;
								
								
								
								
								
//	//							LastResistanceZoneLow = Math.Min(ZS.BottomPrice, LastResistanceZoneLow);
//	//							
//	//							if(	(CurveHighZ[0] >= ZS.BottomPrice) &&
//	//							(CurveHighZ[0] <= ZS.TopPrice) )
//	//							CurveHighZ[0] = Math.Max(CurveHighZ[0],ZS.TopPrice);
								
								
//								if (ZS.EndBar == 0 && CurrentBars[0] > i)
//								{
//									if (ZS.IsBroken)
//										continue;							
															
//									//TESTED
									
//									//if (Print(CurrentBars[0]))
//									//	Print(i +
									
									
//										if (Highs[0][0] >= ZS.BottomPrice)
//										{
											
											
//											if (ZS.TestedPrice == 0)
//											{
//												//Print(CurrentBars[0]);
//												//shortSignalDefault[0] = 1;
//												ZS.TestedPrice = Highs[0][0];
//											}
//											else
//												ZS.TestedPrice = Math.Max(ZS.TestedPrice,Highs[0][0]);
//										}
									
									
//									// BROKEN	
//									if (Highs[0][0] > ZS.TopPrice)
//									{
										
										
										
//										ZS.EndBar = CurrentBars[0];
//										ZS.IsBroken = true;
//										//if (ZS.TestedPrice != 0)
//											ZS.TestedPrice = 0;
										
//									}

									
//								}
//							}
							
//							if (DOlNO)
//							if (zzz.SupportZones2 != null)
//							foreach (Zone ZS in zzz.SupportZones2)	
//							{
	
//								if (ZS.IsBroken)
//									continue;
								
//	//							LastSupportZoneHigh = Math.Max(ZS.TopPrice, LastSupportZoneHigh);
//	//											
//	//						if(	(CurveLowZ[0] >= ZS.BottomPrice) &&
//	//							(CurveLowZ[0] <= ZS.TopPrice) )
//	//							CurveLowZ[0] = Math.Min(CurveLowZ[0],ZS.BottomPrice);
//	//			
				
//								if (ZS.EndBar == 0 && CurrentBars[0] > i)
//								{
									
//									// TESTED
									
									
//										if (Lows[0][0] <= ZS.TopPrice) // ZONE
//										{
//											if (ZS.TestedPrice == 0)
//											{
//												//longSignalDefault[0] = 1;

//												ZS.TestedPrice = Lows[0][0];
//											}
//											else
//												ZS.TestedPrice = Math.Min(ZS.TestedPrice,Lows[0][0]);
											
//											//Print(Time[0] + "   " + Lows[0][0]);
											
//										}
									
									
//									// BROKEN	
//									if (Lows[0][0] < ZS.BottomPrice)
//									{
//										ZS.EndBar = CurrentBars[0];
//										ZS.IsBroken = true;
//										//if (ZS.TestedPrice != 0)
//											ZS.TestedPrice = 0;
										
//									}

									
//								}
//							}
							
//							// imbalance
							
//							if (DOlNO)
//							if (zzz.ResistanceZones != null)
//							foreach (Zone ZS in zzz.ResistanceZones)
//							{
//								//TotalNumberOfZones = TotalNumberOfZones + 1;
//								//TotalWidthOfAllZones = TotalWidthOfAllZones + Math.Round((ZS.TopPrice - ZS.BottomPrice)/TickSize,0);
								
//								if (ZS.IsBroken)
//									continue;
													
//	//							LastResistanceZoneLow = Math.Min(ZS.BottomPrice, LastResistanceZoneLow);
//	//							
//	//							if(	(CurveHighZ[0] >= ZS.BottomPrice) &&
//	//							(CurveHighZ[0] <= ZS.TopPrice) )
//	//							CurveHighZ[0] = Math.Max(CurveHighZ[0],ZS.TopPrice);
								
								
//								if (ZS.EndBar == 0 && CurrentBars[0] > i)
//								{
//									if (ZS.IsBroken)
//										continue;							
															
//									//TESTED
//										if (Highs[0][0] >= ZS.BottomPrice)
//										{
//											if (ZS.TestedPrice == 0)
//											{ 
//												//shortSignalDefault[0] = 1;
//												ZS.TestedPrice = Highs[0][0];
//											}
//											else
//												ZS.TestedPrice = Math.Max(ZS.TestedPrice,Highs[0][0]);
//										}
									
									
//									// BROKEN	
//									if (Highs[0][0] > ZS.TopPrice)
//									{
//										ZS.EndBar = CurrentBars[0];
//										ZS.IsBroken = true;
//										//if (ZS.TestedPrice != 0)
//											ZS.TestedPrice = 0;
										
//									}

									
//								}
//							}
							
//							if (DOlNO)
//							if (zzz.SupportZones != null)
//							foreach (Zone ZS in zzz.SupportZones)	
//							{
	
//								if (ZS.IsBroken)
//									continue;
								
//	//							LastSupportZoneHigh = Math.Max(ZS.TopPrice, LastSupportZoneHigh);
//	//											
//	//						if(	(CurveLowZ[0] >= ZS.BottomPrice) &&
//	//							(CurveLowZ[0] <= ZS.TopPrice) )
//	//							CurveLowZ[0] = Math.Min(CurveLowZ[0],ZS.BottomPrice);
//	//			
				
//								if (ZS.EndBar == 0 && CurrentBars[0] > i)
//								{
									
//									// TESTED
									
									
//										if (Lows[0][0] <= ZS.TopPrice) // ZONE
//										{
//											if (ZS.TestedPrice == 0)
//											{
//												//longSignalDefault[0] = 1;

//												ZS.TestedPrice = Lows[0][0];
//											}
//											else
//												ZS.TestedPrice = Math.Min(ZS.TestedPrice,Lows[0][0]);
											
//											//Print(Time[0] + "   " + Lows[0][0]);
											
//										}
									
									
//									// BROKEN	
//									if (Lows[0][0] < ZS.BottomPrice)
//									{
//										ZS.EndBar = CurrentBars[0];
//										ZS.IsBroken = true;
//										//if (ZS.TestedPrice != 0)
//											ZS.TestedPrice = 0;
										
//									}

									
//								}
//							}
						
//						}
//					}
					
					
				
			
				
//				//}
				
				
			
//				} // end is currentbar
				
				
				
//				BB = 0;
//				if (Calculate != Calculate.OnBarClose && !pQuickAudio)
//					BB = 1;
				
//				if (State == State.Realtime)
//				if(pAudioEnabledMain && CurrentBars[0] >= BB)
//				{
					
//					string NName = "Veritas OF";
					
//					if (pAudioEnabled1)
//					{
//						if(LongSignals[BB] != 0 && LastAudioBar1 != CurrentBars[0])
//						{
//							Alert(CurrentBar.ToString(),Priority.High, BarsPeriod + " | " + NName + " Washout Buy", NinjaTrader.Core.Globals.InstallDir+@"\sounds\"+pWAVFileName,1,pArrowUpFBrush,GetTextColor(pArrowUpFBrush));
//							LastAudioBar1 = CurrentBars[0];
							
//							//BackBrushes[0] = Brushes.Green;
							
//						}
//						if(ShortSignals[BB] != 0 && LastAudioBar2 != CurrentBars[0])
//						{
//							Alert(CurrentBar.ToString(),Priority.High, BarsPeriod + " | " + NName + " Washout Sell", NinjaTrader.Core.Globals.InstallDir+@"\sounds\"+pWAVFileName2,1,pArrowDownFBrush,GetTextColor(pArrowDownFBrush));
//							LastAudioBar2 = CurrentBars[0];
							
//							//BackBrushes[0] = Brushes.Red;
//						}
//					}
					
//					if (ZoneItems[BB] != null)
//					{
//						if (pAudioEnabled2)
//						{
							
//							BB = 1; // force this to happen after close
							
//							//if (ZoneItems[BB] != null)
//							if(ZoneItems[BB].SupportZones.Count != 0 && LastAudioBar3 != CurrentBars[0])
//							{
//								Alert(CurrentBar.ToString(),Priority.High, BarsPeriod + " | " + NName + " Ask Imbalance Zone", NinjaTrader.Core.Globals.InstallDir+@"\sounds\"+pWAVFileZone,1,pSupportZColor1,GetTextColor(pSupportZColor1));
//								LastAudioBar3 = CurrentBars[0];
								
//								//BackBrushes[0] = Brushes.Green;
								
//							}
							
//							//if (ZoneItems[BB] != null)
//							if(ZoneItems[BB].ResistanceZones.Count != 0 && LastAudioBar4 != CurrentBars[0])
//							{
//								Alert(CurrentBar.ToString(),Priority.High, BarsPeriod + " | " + NName + " Bid Imbalance Zone", NinjaTrader.Core.Globals.InstallDir+@"\sounds\"+pWAVFileZone2,1,pResistanceZColor1,GetTextColor(pResistanceZColor1));
//								LastAudioBar4 = CurrentBars[0];
								
//								//BackBrushes[0] = Brushes.Red;
//							}
//						}
						
////						if (pAudioEnabled2)
////						{
////							//if (ZoneItems[BB] != null)
////							if(ZoneItems[BB].SupportZones.Count != 0 && LastAudioBar5 != CurrentBars[0])
////							{
////								Alert(CurrentBar.ToString(),Priority.High, BarsPeriod + " | " + NName + " Ask Momentum Zone", NinjaTrader.Core.Globals.InstallDir+@"\sounds\"+pWAVFileZone,1,pArrowUpFBrush,Brushes.White);
////								LastAudioBar3 = CurrentBars[0];
								
////								//BackBrushes[0] = Brushes.Green;
								
////							}
							
////							//if (ZoneItems[BB] != null)
////							if(ZoneItems[BB].ResistanceZones.Count != 0 && LastAudioBar6 != CurrentBars[0])
////							{
////								Alert(CurrentBar.ToString(),Priority.High, BarsPeriod + " | " + NName + " Bid Momentum Zone", NinjaTrader.Core.Globals.InstallDir+@"\sounds\"+pWAVFileZone2,1,pArrowDownFBrush,Brushes.White);
////								LastAudioBar4 = CurrentBars[0];
								
////								//BackBrushes[0] = Brushes.Red;
////							}
////						}						
						
						
						
//					}
					

		

		

		
//					if (pAudioEnabled5 || pAudioEnabled6)
//					for (int i = 1; i < CurrentBars[0]-0-BBHist; i++)
//					{
//						//if (BarItems.IsValidDataPointAt(CurrentBars[0]))
//						{							
//							BarItem BBBBB = BarItems.GetValueAt(CurrentBars[0]);
							
//							if (pAudioEnabled6)
//							if (LastAudioBar6 != CurrentBars[0])
//							foreach (Level kvp2 in BBBBB.Magnets)
//							{
//								Alert(CurrentBar.ToString(),Priority.High, BarsPeriod + " | " + NName + " Magnet", NinjaTrader.Core.Globals.InstallDir+@"\sounds\"+pWAVFileName6,1,pMagnetStroke.Brush,GetTextColor(pMagnetStroke.Brush));
//								LastAudioBar6 = CurrentBars[0];								
//							}
							
//							if (pAudioEnabled5)
//							if (LastAudioBar5 != CurrentBars[0])								
//							foreach (Level kvp2 in BBBBB.UnfinishedAuctions)
//							{
//								//int closedbar = kvp2.EndBar;
//								//double price = kvp2.Price;

//								Alert(CurrentBar.ToString(),Priority.High, BarsPeriod + " | " + NName + " Unfinished Auction", NinjaTrader.Core.Globals.InstallDir+@"\sounds\"+pWAVFileName5,1,pUFAStroke.Brush,GetTextColor(pUFAStroke.Brush));
//								LastAudioBar5 = CurrentBars[0];	
								
//							}						
							
//						}
//					}
				
					
					
					
//				}
				
			
				
				

				
//			}  // end bars in progress 0
			
			
			
			
			
			
				
				
				
				
		
			
			
				

//        }

		
		private void SetOrderFlags()
		{
			
			
					// Values[19][0] = 0;
		
					// order y axis flags
					
				TriggerCustomEvent(o =>
	   			{
	      
//					Print(FirstPlot);
//					Print(Values.Length);
					
					Values[FirstPlot][0] = 0;
					Values[FirstPlot+1][0] = 0;
					Values[FirstPlot+2][0] = 0;
					
					
				}, null);					
					
					int currentplot = FirstPlot+3;
					for (int i = currentplot-1; i <= Values.Length-1; i++)
					{
						Values[i][0]=0;
						Values[i][1]=0;
					}
					
				

					if (pOrdersDisplayOn)						
					// position price flag
					if (ThisPositionNow() != null)
					{
						
						Values[currentplot-1][0] = RTTS(ThisPositionNow().AveragePrice);	
					
							if (ThisPositionNow().MarketPosition == MarketPosition.Long)
								PlotBrushes[currentplot-1][0] = Brushes.Lime;
							else
								PlotBrushes[currentplot-1][0] = Brushes.Red;
						
					}

						
					//TotalOrdersPrices.Clear();
					TotalOrdersPrices2.Clear();

					if (myAccount == null)
						return;
					
			
					
					
//					bool gdsfgsg2 = false;
//					if (gdsfgsg2)
						
					if (pOrdersDisplayOn)
						foreach (Order or in myAccount.Orders.ToList())
						{
							
							
								
							double ThisPrice = or.StopPrice;
							
							if (or.OrderType == OrderType.Limit)
							{
								ThisPrice = or.LimitPrice;
							
							}
							
							int thisq = 1;
						
							OrderStateOK = or.OrderState == OrderState.Working || or.OrderState == OrderState.Accepted || or.OrderState == OrderState.ChangePending  || or.OrderState == OrderState.ChangeSubmitted || or.OrderState == OrderState.CancelPending || or.OrderState == OrderState.CancelSubmitted;
							OrderInstrumentOK = or.Instrument.FullName == Instrument.FullName;
							OrderNameOK = true;
							OrderTypeOK = true;
								
								
							if (OrderTypeOK && OrderStateOK && OrderNameOK && OrderInstrumentOK)						
							//if (or.Name.Contains ("Stop"))
							{
								

								if (!TotalOrdersPrices2.ContainsKey(ThisPrice))
								{
									TotalOrdersPrices2.Add(ThisPrice, thisq);
									Values[currentplot][0] = ThisPrice;
									
									int totalatprice = 0;
									
									if (TotalOrdersPrices.ContainsKey(ThisPrice))
										totalatprice = TotalOrdersPrices[ThisPrice];
											
									
									//Print(totalatprice);
									
									
									if (totalatprice > 1)
									{
										PlotBrushes[currentplot][0] = pOrderSummaryColor;
									}
									else if (or.Name.Contains("Target"))
									{
										
										PlotBrushes[currentplot][0] = pOrderTargetColor;
										
									
									}
									else if (or.Name.Contains("Stop"))
									{
										PlotBrushes[currentplot][0] = pOrderStopColor;
										
										
									}					
				
									
									else if (or.OrderType == OrderType.Limit)
									{
										PlotBrushes[currentplot][0] = pOrderLimitColor;
										
										
									}
									else if (or.OrderType == OrderType.MIT)
									{
										PlotBrushes[currentplot][0] = pOrderMITColor;
										
										
									}
									else if (or.OrderType == OrderType.StopLimit)
									{
										PlotBrushes[currentplot][0] = pOrderStopLimitColor;
										
									}
									else if (or.OrderType == OrderType.StopMarket)
									{
										PlotBrushes[currentplot][0] = pOrderStopMarketColor;
										
										
									}					
							
									currentplot = currentplot + 1;
									
									
								}
								else
								{
									int newq =  TotalOrdersPrices2[ThisPrice] +  thisq;
									TotalOrdersPrices2[ThisPrice] = newq;
								}
								
//								if (TotalOrdersPrices[ThisPrice] > 1)
//								{
									
//									//Print(TotalOrdersPrices[ThisPrice]);
									
//									//if (!AllOrderPrices.ContainsKey(ThisPrice))
//									//	AllOrderPrices.Add(ThisPrice, startright - 30);
								
//								}
								
									
							}
								
						
					
							
						}
						
						
		}

		
		
		private void ChartBarsSwitch(bool onoff)
		{
			if (ChartPanel == null)
			return;
			
			IList<Gui.NinjaScript.IChartObject> myObjects = ChartPanel.ChartObjects;

					foreach (Gui.NinjaScript.IChartObject thisObject in myObjects)
					{
					
						if (thisObject.GetType().FullName.Contains("ChartBars"))
						{
							
							if (!thisObject.IsVisible)
			                    {
			                        thisObject.IsVisible = onoff;
									//thisObject.IsAutoScale = onoff;
									
			                    }
			                    else
			                    {
			                        thisObject.IsVisible = onoff;
									//thisObject.IsAutoScale = onoff;
									
			                    }
					
						}
						
						//Print(String.Format("{0} is of type {1}", thisObject.Name, thisObject.GetType().FullName));
						
						//saveObjects.Add(thisObject);
						//myObjects.Remove(thisObject);
						
					}
					
			//ChartBars.Properties.AutoScale = onoff;
			
		}

        private void AddButtonZ( SortedDictionary<double, ButtonZ> ThisList, string iText, string iName, int iWidth, bool iSwitch)
        {
            ButtonZ Z = new ButtonZ();
            Z.Text = iText;
            Z.Name = iName;
            Z.Width = iWidth;
            Z.Switch = iSwitch;
            Z.Rect = new SharpDX.RectangleF(0, 0, 0, 0);
            Z.Hovered = false;

            ThisList.Add(ThisList.Count + 1, Z);

        }

        // Cache text measurements to avoid per-level TextLayout creation
        private Dictionary<string, SharpDX.Size2F> textMeasureCache = new Dictionary<string, SharpDX.Size2F>();

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
        private const string PANEL_ID = "OrderFlow";
        private const int PANEL_PRIORITY = 300;
        private const string PANEL_HEADER_TEXT = "ORDER FLOW";
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
        private bool _cursorOverrideActive = false;
        private bool? _lastAppliedSelectionDragging = null;
        private bool _selectionDraggingOverridden = false;
        private void ApplyHoverCursor()
        {
            if (ChartControl == null) return;
            // Draggable vertical-line handles (volume-histogram width, right margin,
            // composite width) are clickable UI too — show the hand while hovering
            // one, or mid-drag before the finalizing click.
            if (IsHoverMD || IsMoveMD || IsHoverRM || IsMoveRM || IsHoverComposite || IsMoveComposite)
                _hoverClickableThisFrame = true;
            try
            {
                var reg = GetHoverRegistry();
                var dict = reg.GetOrAdd(panelHash, k => new ConcurrentDictionary<string, bool>());
                dict[PANEL_ID] = _hoverClickableThisFrame;
                bool any = false;
                foreach (var kvp in dict) if (kvp.Value) { any = true; break; }

                // Suppress NT's chart-series selection while the mouse is over our UI
                // so clicking our buttons/handles can't select background bars or
                // indicators. Restored to the user's setting when off our UI.
                if (myProperties != null)
                {
                    bool wantDrag = !any && PreviousDrag;
                    if (_lastAppliedSelectionDragging != wantDrag)
                    {
                        _lastAppliedSelectionDragging = wantDrag;
                        _selectionDraggingOverridden = true;
                        myProperties.AllowSelectionDragging = wantDrag;
                    }
                }

                // Hand cursor via Mouse.OverrideCursor: unlike ChartControl.Cursor it
                // updates immediately without mouse movement and is not clobbered when
                // NT resets the chart cursor on a click or drag. Released as soon as
                // the mouse is no longer over our UI.
                if (any)
                {
                    if (System.Windows.Input.Mouse.OverrideCursor != System.Windows.Input.Cursors.Hand)
                        System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Hand;
                    _cursorOverrideActive = true;
                }
                else if (_cursorOverrideActive)
                {
                    System.Windows.Input.Mouse.OverrideCursor = null;
                    _cursorOverrideActive = false;
                }
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
                // Merge: preserve existing lines not in panels dict (prevents wiping
                // sibling panels that haven't registered yet on staggered startup).
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
            if (!registry.TryGetValue(panelHash, out panels)) return InMenu3;
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
            // Sharp top-left corner: fill a square over the rounded corner
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

        #endregion Shared Button Panel Coordinator

        private bool MouseIn(SharpDX.RectangleF RR, int XF, int YF)
        {

			if (FinalXPixel != 0)
            if (FinalXPixel >= RR.Left - XF && FinalXPixel <= RR.Right + XF && FinalYPixel >= RR.Top - YF && FinalYPixel <= RR.Bottom + YF)
                return true;
           
                return false;

        }		

//        private bool MouseIn(SharpDX.RectangleF RR, int XF, int YF)
//        {
//            //Print(RR.Left);
            
//            if (MP.X >= RR.Left - XF && MP.X <= RR.Right + XF && MP.Y >= RR.Top - YF && MP.Y <= RR.Bottom + YF)
//                return true;
//            else
//                return false;

//        }

        private double RTTS(double p)
        {
            return ThisMasterInstrument.RoundToTickSize(p);
			
			//return p;
        }

		
		public override void OnCalculateMinMax()
		{
			try
			{
				
				

				if (!Permission)
					return;
				
					// make sure to always start fresh values to calculate new min/max values
				double tmpMin = double.MaxValue;
				double tmpMax = double.MinValue;
				 
				 // For performance optimization, only loop through what is viewable on the chart
					
				for (int index = ChartBars.FromIndex; index <= ChartBars.ToIndex; index++)
				{
				    // since using Close[0] is not guaranteed to be in sync
				    // retrieve "Close" value at the current viewable range index
					double plotValue1 = Highs[0].GetValueAt(index);
					double plotValue2 = Lows[0].GetValueAt(index);

				// return min/max of close value
				    tmpMin = Math.Min(tmpMin, plotValue2);
				    tmpMax = Math.Max(tmpMax, plotValue1);
				}
				 
				// Finally, set the minimum and maximum Y-Axis values to +/- 50 ticks from the primary close value

				int extraticks = 0;

				// TicksAdjust is the mouse-wheel Y-scroll offset (0 unless Y Scroll is active).
				// Applied here so scrolling pans the axis even on the auto (non-fixed) scale;
				// the fixed-scale branch below re-applies it on its own ScaleHigh/ScaleLow values.
				MinValue = tmpMin - extraticks * TickSize + TickSize * TicksAdjust;
				MaxValue = tmpMax + extraticks * TickSize + TickSize * TicksAdjust;

				//Print(MaxValue);
				//Print(MinValue);
				
				if (pUseFixedVerticalScale)
				{
					
					
	//				for (int index = ChartBars.FromIndex; index <= ChartBars.ToIndex; index++)
	//				{
	//				    // since using Close[0] is not guaranteed to be in sync
	//				    // retrieve "Close" value at the current viewable range index
	//					double plotValue1 = Highs[0].GetValueAt(index);
	//					double plotValue2 = Lows[0].GetValueAt(index);

	//				// return min/max of close value
	//				    tmpMin = Math.Min(tmpMin, plotValue2);
	//				    tmpMax = Math.Max(tmpMax, plotValue1);
	//				}
					
	//				int ToIndex = ChartBars.ToIndex;
					
	//				if (Highs[0].GetValueAt(ToIndex) == tmpMax)
	//				{
	//					MaxValue = tmpMax;
	//					MinValue = tmpMax - pScaleTicks*TickSize;
	//				}
	//				else if (Lows[0].GetValueAt(ToIndex) == tmpMin)
	//				{
	//					MaxValue = tmpMin + pScaleTicks*TickSize;
	//					MinValue = tmpMin;
	//				}
	//				else
	//				{
						
	////					double feerr = tmpMax - (tmpMax - tmpMin)/2;
						
	////					if (Closes[0].GetValueAt(ToIndex-1) >= feerr)
	////					{
	////						MaxValue = tmpMax;
	////						MinValue = tmpMax - pScaleTicks*TickSize;
	////					}
	////					else
	////					{
	////						MaxValue = tmpMin + pScaleTicks*TickSize;
	////						MinValue = tmpMin;
	////					}					
						
	//					//if (Closes[0].GetValueAt(ToIndex) < MinValue || Closes[0].GetValueAt(ToIndex) > MaxValue)
	//					{
	//						MaxValue = ScaleHigh.GetValueAt(ChartBars.ToIndex);
	//						MinValue = ScaleLow.GetValueAt(ChartBars.ToIndex);						
									
	//					}
							
						
	//				}
					
					
				//	if (CurrentBars[0] <= ChartBars.ToIndex)
				//	{
						
						
//						if (CurrentLast > ScaleHigh.GetValueAt(ChartBars.ToIndex))
//						{
//							ForceHighPriority = true;
//							ForceLowPriority = false;
							
//						}
//						if (CurrentLast < ScaleLow.GetValueAt(ChartBars.ToIndex))
//						{
//							ForceHighPriority = false;
//							ForceLowPriority = true;
							
							
//						}						
						
				//	}
					
					
					// old fixed scale method
					

			//TriggerCustomEvent(o =>
		   //			{
		      
					
					MaxValue = ScaleHigh.GetValueAt(ChartBars.ToIndex) + TickSize*TicksAdjust;
					MinValue = ScaleLow.GetValueAt(ChartBars.ToIndex) + TickSize*TicksAdjust;
					
					
					//MaxValue = CurrentScaleHigh + TickSize*TicksAdjust;
					//MinValue = CurrentScaleLow + TickSize*TicksAdjust;
							
					
					
			//		}, null);					
					 
					
					if (TestScale)
					{
						Print("MinMax2: " + CurrentScaleHigh);
						Print("MinMax2: " + CurrentScaleLow);						
						Print("MinMax: " + TicksAdjust);
						Print("MinMax: " + MaxValue);
						Print("MinMax: " + MinValue);		
						
					}
				
				}
				
				
				
				//MinValue = tmpMin - extraticks * TickSize + TickSize*TicksAdjust;;
				//MaxValue = tmpMax + extraticks * TickSize + TickSize*TicksAdjust;;	
				
				
			//MaxValue2 = MaxValue;
			//MinValue2 = MinValue;
			//MaxValue = 50;
			//MinValue = 51;
			
			}
				
			catch (Exception ex)
			{
				if (TestRender) Print("OnCalculateMinMax: " + ex.Message + " ");
				
			}
				
		}

		private void SetThisScale(int BB)
		{
			//int BTH = CurrentBars[0]-1;
			

			//Print(BB);
			
			TriggerCustomEvent(o =>
   			{
      
				if (CurrentBars[0] == 0)
				//if ((State == State.Historical && CurrentBars[0] == 0) || (IsCurrentBar && BB == BTH))
				{
					ScaleHigh[BB] = High[BB] + pScaleMarginTicks*TickSize;
					ScaleLow[BB] = ScaleHigh[BB] - pScaleTicks*TickSize;
					
				}
				else
				{
					
					
				
//					if (State != State.Historical)
//					if (CurrentLast > ScaleHigh[BB])
//					{
//						ForceHighPriority = true;
//						ForceLowPriority = false;
						
//						ScaleHigh[BB] = CurrentLast + pScaleMarginTicks*TickSize;
//						ScaleLow[BB] = ScaleHigh[BB] - pScaleTicks*TickSize;	
						
//					}
//					if (CurrentLast < ScaleLow[BB])
//					{
//						ForceHighPriority = false;
//						ForceLowPriority = true;
						
//						ScaleLow[BB] = CurrentLast - pScaleMarginTicks*TickSize;					
//						ScaleHigh[BB] = ScaleLow[BB] + pScaleTicks*TickSize;							
//					}			
				
					
					
					
					if (!ForceHighPriority && !ForceLowPriority)
					{
						if (IsFirstTickOfBar)
						{
							ScaleHigh[BB] = ScaleHigh[BB+1];
							ScaleLow[BB] = ScaleLow[BB+1];
						}

						double cccchigh = High[BB];
						double cccclow = Low[BB];

						if (State == State.Realtime)
						{
							cccchigh = CurrentLast;
							cccclow = CurrentLast;
						}

						if (pScalePanMode == "Center")
						{
							// Center mode: always keep price centered in the window
							double centerPrice = State == State.Realtime ? CurrentLast : (High[BB] + Low[BB]) / 2.0;
							double halfRange = pScaleTicks * TickSize / 2.0;
							ScaleHigh[BB] = centerPrice + halfRange + pScaleMarginTicks * TickSize;
							ScaleLow[BB] = centerPrice - halfRange - pScaleMarginTicks * TickSize;
						}
						else
						{
							// Edge mode: only pan when price nears the boundary
							double deadZone = pScaleEdgeMarginTicks * TickSize;

							if (cccchigh > ScaleHigh[BB] - deadZone)
							{
								ScaleHigh[BB] = cccchigh + pScaleMarginTicks * TickSize + deadZone;
								ScaleLow[BB] = ScaleHigh[BB] - pScaleTicks * TickSize;
							}

							if (cccclow < ScaleLow[BB] + deadZone)
							{
								ScaleLow[BB] = cccclow - pScaleMarginTicks * TickSize - deadZone;
								ScaleHigh[BB] = ScaleLow[BB] + pScaleTicks * TickSize;
							}
						}
					}
					
					
					CurrentScaleHigh = ScaleHigh[BB];
					CurrentScaleLow = ScaleLow[BB];
					
						
					
				}	
				
			
			}, null);
			
		}
		
		private void SetScaleData()
		{
			//int BTH = ChartControl.LastBarPainted - ChartControl.FirstBarPainted;
			
			int BTH = 0;
			
			TriggerCustomEvent(o =>
   			{
				
				BTH = CurrentBars[0]-1;
			}, null);
			
			
			int st = 1;
			//if (!AllCOBC())
				st = 0;
			
			//Print(pScaleTicks);
			//pScaleMarginTicks = Math.Min(1000, Math.Max(pScaleMarginTicks, 0));
			pScaleTicks = Math.Min(1000, Math.Max(pScaleTicks, 1));
			
			//Print(pScaleMarginTicks);
				
			for (int i = BTH; i >= st; i--)
			{

				SetThisScale(i);
				
			}	
			
			//ChartControl.ChartPanel.Invalidate();
			
		}
		
		
		public override void OnRenderTargetChanged()
		{
				if (!Permission)
					return;
		  // Explicitly set the Stroke RenderTarget

			//Print("OnRenderTargetChanged");

			// Invalidate cross-frame-cached DX brushes — they're bound to the previous RenderTarget
			SafeDispose(cachedPanelBackdropBrushDX); cachedPanelBackdropBrushDX = null;
			SafeDispose(cachedHeaderBgBrushDX); cachedHeaderBgBrushDX = null;
			SafeDispose(cachedHeaderBgHoverBrushDX); cachedHeaderBgHoverBrushDX = null;
			SafeDispose(cachedHeaderTextBrushDX); cachedHeaderTextBrushDX = null;
			SafeDispose(cachedHoverGlowBrushDX); cachedHoverGlowBrushDX = null;
			_panelCacheBgRef = null;
			_panelCacheTextRef = null;
			// TextFormats are NOT render-target-bound — keep across RT changes

			if (RenderTarget != null)
			{

				pHighlightStroke.RenderTarget = RenderTarget;
				pOneWidthStroke.RenderTarget = RenderTarget;
				
				
				pONESupport.RenderTarget = RenderTarget;
				//pONEResistance.RenderTarget = RenderTarget;
				
				pTMSupport.RenderTarget = RenderTarget;
				pTMResistance.RenderTarget = RenderTarget;
				
				pMagnetStroke.RenderTarget = RenderTarget;
				pUFAStroke.RenderTarget = RenderTarget;
				
				pMagnetStroke2.RenderTarget = RenderTarget;
				pUFAStroke2.RenderTarget = RenderTarget;
				
				pLastPriceLineStroke.RenderTarget = RenderTarget;
//				pBarOutlineStroke.RenderTarget = RenderTarget;
				pBarUpOutlineStroke.RenderTarget = RenderTarget;
				pBarDnOutlineStroke.RenderTarget = RenderTarget;
				
				pOrderUpOutlineStroke.RenderTarget = RenderTarget;
				pOrderDnOutlineStroke.RenderTarget = RenderTarget;
				pOrderBothOutlineStroke.RenderTarget = RenderTarget;
				
				pClusterStroke.RenderTarget = RenderTarget;
//				pBrush06.RenderTarget = RenderTarget;

				pBarBodyUpStroke.RenderTarget = RenderTarget;
				pBarBodyDownStroke.RenderTarget = RenderTarget;
				
				pColorSwingLow.RenderTarget = RenderTarget;
				pColorSwingHigh.RenderTarget = RenderTarget;
				pColorSwingLow2.RenderTarget = RenderTarget;
				pColorSwingHigh2.RenderTarget = RenderTarget;				
				
				pGridLineHStroke.RenderTarget = RenderTarget;
				pGridLineVStroke.RenderTarget = RenderTarget;
				
				
//				pBrush09.RenderTarget = RenderTarget;
//				pBrush10.RenderTarget = RenderTarget;	
//				pBrush11.RenderTarget = RenderTarget;
//				pBrush12.RenderTarget = RenderTarget;
//				pBrush13.RenderTarget = RenderTarget;
//				pBrush14.RenderTarget = RenderTarget;
//				pBrush15.RenderTarget = RenderTarget;
//				pBrush16.RenderTarget = RenderTarget;
//				pBrush17.RenderTarget = RenderTarget;
//				pBrush18.RenderTarget = RenderTarget;
//				pBrush19.RenderTarget = RenderTarget;
//				pBrush20.RenderTarget = RenderTarget;
				
				pArrowUpStroke.RenderTarget = RenderTarget;
				pArrowDownStroke.RenderTarget = RenderTarget;
				
		
				ThisStroke.RenderTarget = RenderTarget;
				ThisStrokeH.RenderTarget = RenderTarget;
				
			}
			

			
			
			
			
		}
		
		private void ClearBrush(ref SharpDX.Direct2D1.Brush BBB)
		{
			if (BBB != null && !BBB.IsDisposed)
			{
				BBB.Dispose();
				BBB = null;
			}
		}

		private void SafeDispose(SharpDX.Direct2D1.Brush brush)
		{
			if (brush != null && !brush.IsDisposed)
				brush.Dispose();
		}

		private void SafeDispose(SharpDX.DirectWrite.TextFormat tf)
		{
			if (tf != null && !tf.IsDisposed)
				tf.Dispose();
		}

		// NOTE: A simple WPF-Brush-keyed DX brush cache was considered and rejected here:
		// many DX brushes in this file share the same WPF source but need different per-frame
		// opacities (e.g. pChartAxisBrush backs buttonBrushDX / buttonHBrushDX / buttonFHBrushDX
		// with differing opacities). Using a single cached instance causes the last opacity
		// setter to "win", producing visual bugs. A future refactor should use a (brush, role)
		// composite key before caching is re-introduced.
			
	
		private bool RenderTest(int CurrentStopRender)
		{
			
			if (!TestBeta)
				return false;
			
			//Print(SelectedStopRender + " " + CurrentStopRender);
			
			if (SelectedStopRender == CurrentStopRender)
				return true;
			else
				return false;
			
			//CurrentStopRender = CurrentStopRender + 1;
		}
		
		
			//private bool FirstRender2 = true;
		
		
		   // ChartControlProperties myProperties;
					
			
	
		
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
		
				
		
		protected override void OnRender(ChartControl chartControl, ChartScale chartScale)
		{
			_hoverClickableThisFrame = false;
			// Lazy-register if DataLoaded ran before ChartPanel was ready.
			if (panelHash == 0 && ChartPanel != null)
				RegisterPanel();

			oldAntialiasMode	= RenderTarget.AntialiasMode;

			// Panel-zone clip removed — see ECT note. Panel backdrop covers content.
			bool panelClipActive = false;

			if (FirstRender2)
			{
			
				ChartBarsSwitch2(true);

				
            	myProperties = chartControl.Properties;
				PreviousDrag = myProperties.AllowSelectionDragging;
				_lastAppliedSelectionDragging = PreviousDrag;
				
				
				
				
				
				chartTrader = Window.GetWindow(ChartControl.Parent).FindFirst("ChartWindowChartTraderControl") as ChartTrader;	
				
				FirstRender2 = false;
				
				
			}
		
				
				ChartTextBrushDX = myProperties.ChartText.ToDxBrush(RenderTarget);
				ChartBackgroundBrushDX = myProperties.ChartBackground.ToDxBrush(RenderTarget);				 			
				//ChartBackgroundErrorBrushDX = Brushes.Red.ToDxBrush(RenderTarget);
							

			
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
						RenderTarget.FillRectangle(CenterRect, ChartBackgroundBrushDX);
						RenderTarget.FillRectangle(CenterRect, ChartBackgroundErrorBrushDX);
						RenderTarget.DrawText(txt, CenterText, ExpandRect(CenterRect,-10,0), ChartTextBrushDX);
						
						
						RenderTarget.AntialiasMode = oldAntialiasMode;
						
					
					
					ChartBackgroundErrorBrushDX.Dispose();
					CenterText.Dispose();
					ChartTextBrushDX.Dispose();
					ChartBackgroundBrushDX.Dispose();

				return;
			}
			
				
				
				
		
			
			
			
			//return;
			
			SelectedStopRender = 100;
			
			//Print(CurrentStopRender);
			
			
			//CurrentStopRender = 0;
			
			
			if (RenderTest(0)) return; // 0
			
			
				
			//Print("OnRender: " + DateTime.Now + " " + CurrentBar);
	
			if (MouseWheelDone)
			{
				
				chartScale.Properties.YAxisRangeType = YAxisRangeType.Automatic;	
			
				MouseWheelDone = false;
			}
			
			try  
			{ 
				
				if (!Permission)
					return;
				
				try  
				{ 
					
				if(Bars == null || ThisMasterInstrument == null || IsInHitTest || CurrentBar < 0 || !isActiveTab(chartControl))
				{ 
					return;
				}
			
				}
				catch (Exception ex)
				{
					if (TestRender) Print("OnRender Start: " + ThisMasterInstrument.Name + " " + ex.Message + " " + DateTime.Now.ToString("T"));
					
				}
							
			//TriggerCustomEvent(o =>
   			//{	
								
				
			//base.OnRender(chartControl, chartScale);	
				
				ChartBarsSwitch2(false);
				
			if (FirstRender)
			{
				
				FirstRender = false;
				
//				if (EnableOrderExecution && connected && pOrderPanelOn)
//					TotalTheOrders();
				
				if (ChartControl != null)
				{
					ChartBarsSwitch2(false);
					
					//ChartBars.isv
					ChartBars.Properties.AutoScale = false;
					
					// Instantiate a ChartControlProperties object to hold a reference to chartControl.Properties
		            myProperties = ChartControl.Properties;
		
					// Set the AllowSelectionDragging property to false
		            //myProperties.AllowSelectionDragging = false;

					myProperties.ChartBackground = pChartBackgroundBrush;	
					myProperties.ChartText = pChartAxisBrush;	
					
					myProperties.GridLineHPen.DashStyleHelper = pGridLineHStroke.DashStyleHelper;
					myProperties.GridLineHPen.Width = pGridLineHStroke.Width;
					myProperties.GridLineHPen.Pen = pGridLineHStroke.Pen;
					
					myProperties.GridLineVPen.DashStyleHelper = pGridLineVStroke.DashStyleHelper;
					myProperties.GridLineVPen.Width = pGridLineVStroke.Width;
					myProperties.GridLineVPen.Pen = pGridLineVStroke.Pen;					
					
				
					//ChartBars.Properties.PriceMarker.IsVisible = pShowLastPriceMarker2;
					ChartBars.Properties.PaintPriceMarker = pShowLastPriceMarker2 && pShowLastPriceAll;
					ChartBars.Properties.PriceMarker.Background = pColorLastPriceMarker;
					

					
					VerticalLineHighlightDX = pChartAxisBrush.ToDxBrush(RenderTarget);
					VerticalLineHighlightDX.Opacity = 60/100f;
			
					
						//ChartControl.ChartPanels[0].InvalidateVisual();	
			
					
				}	
				

				//minimumtextsize
				
				int maxsize = Math.Max((int)pTextFont2Imb.Size, (int)pTextFont2.Size);
				
				
				for(int u = maxsize; u >= 1; u--)
				{
					
					double hhh = 0;
					SimpleFont TextFontCalc = new SimpleFont(pTextFont2.Family.ToString(), u);
					CellFormatCalc = TextFontCalc.ToDirectWriteTextFormat();
					CellLayoutCalc = new TextLayout(Core.Globals.DirectWriteFactory, "L1343", CellFormatCalc, 10000, 10000);

					hhh = CellLayoutCalc.Metrics.Height;

					HeightToTextSize.Add(u, hhh);

					CellLayoutCalc.Dispose();
					CellFormatCalc.Dispose();
					
				}
				
			
				//this.ChartControl.InvalidateVisual();
				//ChartControl.ChartPanels[0].InvalidateVisual();	
				
			}
			
				
				if (RenderTest(1)) return; // 0
			
			
			if (AllErrorMessages.Contains("Please make sure ChartTrader is set to visible or hidden."))
				
				if (chartTrader.Visibility != Visibility.Collapsed)
				{
					AllErrorMessages.Remove("Please make sure ChartTrader is set to visible or hidden.");
				}
				
//				if (pCompLevelsPriceOn && pCompAllLevelsEnabled)
//				{	
//					PlotBrushes[2][0] = pPlot1Brush;
//					PlotBrushes[3][0] = pPlot2Brush;
//					PlotBrushes[4][0] = pPlot3Brush;
//					PlotBrushes[5][0] = pPlot4Brush;
//					PlotBrushes[6][0] = pPlot5Brush;
//					PlotBrushes[7][0] = pPlot6Brush;	
//				}
//				else
//				{
//					PlotBrushes[2][0] = Brushes.Transparent;
//					PlotBrushes[3][0] = Brushes.Transparent;
//					PlotBrushes[4][0] = Brushes.Transparent;
//					PlotBrushes[5][0] = Brushes.Transparent;
//					PlotBrushes[6][0] = Brushes.Transparent;
//					PlotBrushes[7][0] = Brushes.Transparent;					

//				}								
//				if (pCompLevelsPriceOn && pCompAllLevelsEnabled)
//				{
//					PaintPriceMarkers = true;
					
//				}
//				else
//				{
				
//					PaintPriceMarkers = false;
//				}
				
				
				
				
				
			lastRender = DateTime.Now;
			
			bool Change1 = false;
			bool Change2 = false;	
			bool Change3 = false;
			
				
				//Print(IsScrolled);
				
			if (!pZoomEnabled)
			{
				Change1 = myProperties.BarDistance != (int) (pCurrentSetting*1) + 1 + pSpaceBetweenBars;
				Change2 = pSpaceBetweenBars != (int) myProperties.BarDistance - (int) (pCurrentSetting*1) - 1;
				Change3 = pCurrentSetting != Math.Max(2,(int) ChartBars.Properties.ChartStyle.BarWidth*2);
				
				if(Change3 || Change2 || IsScrolled)
				{
					pCurrentSetting = Math.Max(2,(int) ChartBars.Properties.ChartStyle.BarWidth*2);
					pSpaceBetweenBars = (int) myProperties.BarDistance - (int) (pCurrentSetting*1) - 1;

					if (DebugBarSizeSync)
						Print("[OrderFlow] BarWidth sync (ZoomIn): width=" + pCurrentSetting + " space=" + pSpaceBetweenBars);

					IsScrolled = false;
				}

			}
			else
			{

				Change1 = myProperties.BarDistance != (int) (pCurrentSetting2*1) + 1 + pSpaceBetweenBars2;
				Change2 = pSpaceBetweenBars2 != (int) myProperties.BarDistance - (int) (pCurrentSetting2*1) - 1;
				Change3 = pCurrentSetting2 != Math.Max(2,(int) ChartBars.Properties.ChartStyle.BarWidth*2);

				if(Change3 || Change2 || IsScrolled)
				{
					pCurrentSetting2 = Math.Max(2,(int) ChartBars.Properties.ChartStyle.BarWidth*2);
					pSpaceBetweenBars2 = (int) myProperties.BarDistance - (int) (pCurrentSetting2*1) - 1;

					if (DebugBarSizeSync)
						Print("[OrderFlow] BarWidth sync (ZoomOut): width=" + pCurrentSetting2 + " space=" + pSpaceBetweenBars2);

					IsScrolled = false;
				}								
				
				
			}
				
				if (RenderTest(2)) return; // 0
			
			//Print(TicksAdjust);

			

			if (CurrentBars[0] < 0)
				return;


			
			oldAntialiasMode = RenderTarget.AntialiasMode;
            RenderTarget.AntialiasMode = SharpDX.Direct2D1.AntialiasMode.Aliased;

       
			
          
			TextBrushDX1 = Brushes.RoyalBlue.ToDxBrush(RenderTarget);

			// Pre-created brushes for inner footprint loop (must be before working pointer assignments)
			MainTextColorDX = pMainTextColor.ToDxBrush(RenderTarget);
			BidFillDX = pColorBidFill.ToDxBrush(RenderTarget);
			BidTextDX = pColorBidText.ToDxBrush(RenderTarget);
			AskFillDX = pColorAskFill.ToDxBrush(RenderTarget);
			AskTextDX = pColorAskText.ToDxBrush(RenderTarget);
			LVFillDX = pColorLV.ToDxBrush(RenderTarget);
			AxisBrushDX = pChartAxisBrush.ToDxBrush(RenderTarget);
			BackgroundColorDX = pColorStatus2.ToDxBrush(RenderTarget);
			TimerMainColorDX = pTimerMainColor.ToDxBrush(RenderTarget);
			ClickUPColorDX = pClickUPColor.ToDxBrush(RenderTarget);
			ClickDNColorDX = pClickDNColor.ToDxBrush(RenderTarget);
			GreenBrushDX = Brushes.Green.ToDxBrush(RenderTarget);
			RedBrushDX = Brushes.Red.ToDxBrush(RenderTarget);
			GrayBrushDX = Brushes.Gray.ToDxBrush(RenderTarget);
			BidHashDX = pColorBidHash.ToDxBrush(RenderTarget);
			AskHashDX = pColorAskHash.ToDxBrush(RenderTarget);

			// Visual overhaul brushes
			SharpDX.Direct2D1.Brush TextShadowBrushDX = null;
			if (pTextShadowEnabled)
			{
				TextShadowBrushDX = new SharpDX.Direct2D1.SolidColorBrush(RenderTarget, new SharpDX.Color(0, 0, 0, 153)); // Black 60%
			}

			SharpDX.Direct2D1.Brush DarkTextBrushDX = null;
			if (pAdaptiveTextColor)
			{
				DarkTextBrushDX = new SharpDX.Direct2D1.SolidColorBrush(RenderTarget, new SharpDX.Color(20, 20, 20, 255));
			}

			// Pre-computed string comparison flags
			isBidAskMode = pPrintNumberDisplayMode == "Bid / Ask";
			isVolumeMode = pPrintNumberDisplayMode == "Volume";
			isDeltaMode = pPrintNumberDisplayMode == "Delta";
			isCandlestickFill = pPrintBarFillMode == "Candlestick";
			isDeltaFill = pPrintBarFillMode == "Delta";
			isVolumeOpacity = pPrintBarOpacityMode == "Volume";
			isDeltaOpacity = pPrintBarOpacityMode == "Delta";

            TextBrushDX2 = MainTextColorDX;

			ChartBackgroundFadeBrushDX = ChartControl.Properties.ChartBackground.ToDxBrush(RenderTarget);
			ChartBackgroundFadeBrushDX.Opacity = pCompBackOpacity/100f;

			ChartBackgroundMenuFadeBrushDX = ChartControl.Properties.ChartBackground.ToDxBrush(RenderTarget);
			ChartBackgroundMenuFadeBrushDX.Opacity = 60/100f;

			ChartBackgroundErrorBrushDX = Brushes.Red.ToDxBrush(RenderTarget);
			ChartBackgroundErrorBrushDX.Opacity = 25/100f;

			//ChartBackgroundBrushDX = Brushes.White.ToDxBrush(RenderTarget);

			buttonBrushDX = pChartAxisBrush.ToDxBrush(RenderTarget);
			buttonHBrushDX = pChartAxisBrush.ToDxBrush(RenderTarget);
			buttonFHBrushDX = pChartAxisBrush.ToDxBrush(RenderTarget);


			if (pButtonHighlightMode == "Off")
			{

				buttonFONBrushDX = ChartControl.Properties.ChartBackground.ToDxBrush(RenderTarget);

				buttonFOFFBrushDX = pColorButtonsOff.ToDxBrush(RenderTarget);
				buttonFOFFBrushDX.Opacity = OnOpacity/100f;
			}
			else
			{


				buttonFOFFBrushDX = ChartControl.Properties.ChartBackground.ToDxBrush(RenderTarget);

				buttonFONBrushDX = pColorButtonsOff.ToDxBrush(RenderTarget);
				buttonFONBrushDX.Opacity = OnOpacity/100f;


			}

			// Panel system brushes — only recreate when source brushes change.
			Brush currentChartBg = ChartControl.Properties.ChartBackground;
			Brush currentTextColor = GetTextColor(currentChartBg);
			bool panelBrushesNeedRebuild = _panelCacheBgRef != currentChartBg || _panelCacheTextRef != currentTextColor
				|| cachedPanelBackdropBrushDX == null || cachedPanelBackdropBrushDX.IsDisposed;

			if (panelBrushesNeedRebuild)
			{
				SafeDispose(cachedPanelBackdropBrushDX);
				SafeDispose(cachedHeaderBgBrushDX);
				SafeDispose(cachedHeaderBgHoverBrushDX);
				SafeDispose(cachedHeaderTextBrushDX);
				SafeDispose(cachedHoverGlowBrushDX);

				cachedPanelBackdropBrushDX = currentChartBg.ToDxBrush(RenderTarget);
				cachedHeaderTextBrushDX = currentTextColor.ToDxBrush(RenderTarget);
				// Pre-blend header bg colors at full opacity for sharp-corner rendering
				Color bgC = ((SolidColorBrush)currentChartBg).Color;
				Color fgC = ((SolidColorBrush)currentTextColor).Color;
				byte nr = (byte)(bgC.R + (fgC.R - bgC.R) * 0.07);
				byte ng = (byte)(bgC.G + (fgC.G - bgC.G) * 0.07);
				byte nb = (byte)(bgC.B + (fgC.B - bgC.B) * 0.07);
				cachedHeaderBgBrushDX = new SharpDX.Direct2D1.SolidColorBrush(RenderTarget, new SharpDX.Color((int)nr, (int)ng, (int)nb, 255));
				byte hr = (byte)(bgC.R + (fgC.R - bgC.R) * 0.16);
				byte hg = (byte)(bgC.G + (fgC.G - bgC.G) * 0.16);
				byte hb = (byte)(bgC.B + (fgC.B - bgC.B) * 0.16);
				cachedHeaderBgHoverBrushDX = new SharpDX.Direct2D1.SolidColorBrush(RenderTarget, new SharpDX.Color((int)hr, (int)hg, (int)hb, 255));
				cachedHoverGlowBrushDX = currentTextColor.ToDxBrush(RenderTarget);

				_panelCacheBgRef = currentChartBg;
				_panelCacheTextRef = currentTextColor;
			}
			// Opacities still set each frame (cheap property write, cached brush retains last value)
			cachedPanelBackdropBrushDX.Opacity = 1.0f;
			cachedHeaderTextBrushDX.Opacity = 0.7f;
			cachedHoverGlowBrushDX.Opacity = 0.08f;

			// Header text format (Arial 11 Bold) never changes — create once, keep forever
			if (cachedHeaderTextFormat == null || cachedHeaderTextFormat.IsDisposed)
			{
				var headerFont = new NinjaTrader.Gui.Tools.SimpleFont("Arial", 11) { Bold = true };
				cachedHeaderTextFormat = headerFont.ToDirectWriteTextFormat();
				cachedHeaderTextFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Leading;
				cachedHeaderTextFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
				cachedHeaderTextFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;
			}

			// Button text format depends on user's LabelFont — cache keyed on family/size
			string btnFontFamily2 = myProperties != null ? myProperties.LabelFont.Family.ToString() : "Arial";
			double btnFontSize2 = myProperties != null ? Math.Max(11, myProperties.LabelFont.Size + pChartMenuTextSize) : 12;
			if (cachedButtonTextFormat == null || cachedButtonTextFormat.IsDisposed
				|| _btnFmtCacheFamily != btnFontFamily2 || _btnFmtCacheSize != btnFontSize2)
			{
				SafeDispose(cachedButtonTextFormat);
				var btnSimpleFont = new NinjaTrader.Gui.Tools.SimpleFont(btnFontFamily2, btnFontSize2);
				cachedButtonTextFormat = btnSimpleFont.ToDirectWriteTextFormat();
				cachedButtonTextFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
				cachedButtonTextFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
				cachedButtonTextFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;
				_btnFmtCacheFamily = btnFontFamily2;
				_btnFmtCacheSize = btnFontSize2;
			}
			if (textMeasureCache != null) textMeasureCache.Clear();


				if (RenderTest(3)) return; // 0
			
			MainFillBrushDX = pColorStatus2.ToDxBrush(RenderTarget);
			BidDotsBrushDX = pColorBidDots.ToDxBrush(RenderTarget);
			AskDotsBrushDX = pColorAskDots.ToDxBrush(RenderTarget);
			BidTriBrushDX = pColorBidTri.ToDxBrush(RenderTarget);
			AskTriBrushDX = pColorAskTri.ToDxBrush(RenderTarget);

			MagnetFillBrushDX = pMagnetFillColor.ToDxBrush(RenderTarget);
			UFAFillBrushDX = pUFAFillColor.ToDxBrush(RenderTarget);
			POCFillBrushDX = pBarPOCFillColor.ToDxBrush(RenderTarget);
			VWAPFillBrushDX = pBarVWAPFillColor.ToDxBrush(RenderTarget);
			
			ThisBrushDX = BackgroundColorDX;
			ThisBrushDX2 = BackgroundColorDX;
			
			MainPrintBrushDX = pColorStatus2.ToDxBrush(RenderTarget);

			int Opac = 30;
			MainPrintBrushDX.Opacity = Opac/100f;

			LastPriceBrushDX = pLastPriceBrush.ToDxBrush(RenderTarget);
			LastPriceBrushDX.Opacity = pLastPriceOpacity/100f;
			CrossHairBrushDX = pCrossHairBrush.ToDxBrush(RenderTarget);
			CrossHairBrushDX.Opacity = pCrossHairOpacity/100f;			
			
			
			TransparentBrushDX = Brushes.Transparent.ToDxBrush(RenderTarget);
			
			PrintLongBrushDX = pColorPrintLong.ToDxBrush(RenderTarget);
			PrintShortBrushDX = pColorPrintShort.ToDxBrush(RenderTarget);
			
			AboveLongBrushDX = pColorAboveLong.ToDxBrush(RenderTarget);
			AboveNeutralBrushDX = pColorAboveNeutral.ToDxBrush(RenderTarget);
			AboveShortBrushDX = pColorAboveShort.ToDxBrush(RenderTarget);			
			
			
				Plot1BrushDX = pPlot1Brush.ToDxBrush(RenderTarget);
				Plot2BrushDX = pPlot2Brush.ToDxBrush(RenderTarget);
				Plot3BrushDX = pPlot3Brush.ToDxBrush(RenderTarget);
				Plot4BrushDX = pPlot4Brush.ToDxBrush(RenderTarget);
				Plot5BrushDX = pPlot5Brush.ToDxBrush(RenderTarget);
				Plot6BrushDX = pPlot6Brush.ToDxBrush(RenderTarget);		
				Plot7BrushDX = pPlot7Brush.ToDxBrush(RenderTarget);
				Plot8BrushDX = pPlot8Brush.ToDxBrush(RenderTarget);	
			
			
				Plot1BrushDX.Opacity = pCompPlotsOpacity/100f;
				Plot2BrushDX.Opacity = pCompPlotsOpacity/100f;
				Plot3BrushDX.Opacity = pCompPlotsOpacity/100f;
				Plot4BrushDX.Opacity = pCompPlotsOpacity/100f;
				Plot5BrushDX.Opacity = pCompPlotsOpacity/100f;
				Plot6BrushDX.Opacity = pCompPlotsOpacity/100f;
				Plot7BrushDX.Opacity = pCompPlotsOpacity/100f;
				Plot8BrushDX.Opacity = pCompPlotsOpacity/100f;

				//Print("A");			
				

            //SharpDX.Direct2D1.StrokeStyle 
            //buttonBrushDX = new SharpDX.Direct2D1.Brush(ChartControl.Properties.AxisPen.BrushDX);
            // buttonHBrushDX = ChartControl.Properties.AxisPen.BrushDX;

            FB = ChartBars.FromIndex;
            LB = ChartBars.ToIndex;
			PrintFB = Math.Max(FB, 1);
			
            BB = 0;
            xt = 0;
            yt = 0;
			
			
			x1 = 0;
			x2 = 0;
			x3 = 0;
			x4 = 0;
				
			y1 = 0;
			y2 = 0;
			y3 = 0;
			y4 = 0;
			y5 = 0;
			y6 = 0;	
			
			barWidth = (int) chartControl.BarWidth;
			barDistance = (int) ChartControl.Properties.BarDistance;
			barBetween = barDistance - barWidth - barWidth - 1;
			
            LB = Math.Min(CurrentBars[0], LB);
            BB = CurrentBars[0] - LB;

			IsHardRightEdge = LB == CurrentBars[0];
			
		
	        if (dpiX == 0)
	        {
				PresentationSource source = PresentationSource.FromVisual(this.ChartPanel);
				
				if (source != null)
				{
	             dpiX = 100.0 * source.CompositionTarget.TransformToDevice.M11;
	            dpiY = 100.0 * source.CompositionTarget.TransformToDevice.M22;
				}
				
			}

				if (RenderTest(4)) return; // 0	
			//Print(dpiX);
			
			FinalXPixel = MP.X / 100 * dpiX;
			FinalYPixel = MP.Y / 100 * dpiY;
			
			//Print(FinalXPixel);
			
           CurrentMousePrice = chartScale.MaxValue - chartScale.MaxMinusMin * (FinalYPixel / chartScale.Height) / dpiY * 100;

			CurrentMousePrice = RTTS(CurrentMousePrice);

			
			ThisMousePriceP = ThisMousePrice;
			CurrentMousePrice = RTTS(CurrentMousePrice);
			ThisMousePrice = CurrentMousePrice;
	

			
			
           //Print("B");
				

            double mousebar = (ChartControl.GetXByBarIndex(ChartBars, ChartBars.ToIndex) - ChartControl.GetXByBarIndex(ChartBars, ChartBars.FromIndex)) / Math.Max(1,(ChartBars.ToIndex - ChartBars.FromIndex)); //chartControl.GetBarPaintWidth(ChartBars);

            double mousebar2 = ChartBars.FromIndex + (FinalXPixel - ChartControl.GetXByBarIndex(ChartBars, ChartBars.FromIndex)) / mousebar;

            int mousebar3 = (int) Math.Round(mousebar2, 0);
			
			
			ThisMouseBarP = ThisMouseBar;
			
			// plot vertical cross
			
			ThisMouseBar = mousebar3;
			
			
            // Removed unused textFormat (Arial 12pt) - was never used for rendering


            double yt22 = chartScale.GetYByValue(chartScale.MinValue) - 17;
            SharpDX.RectangleF BottomRect = new SharpDX.RectangleF(140, (float)yt22, 2000F, 21);

            //Print("D");

            SharpDX.DirectWrite.TextFormat BottomText = ChartControl.Properties.LabelFont.ToDirectWriteTextFormat();

            SharpDX.DirectWrite.TextFormat CenterText = new SimpleFont(ChartControl.Properties.LabelFont.Family.ToString(), 16).ToDirectWriteTextFormat();
            CenterText.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
            CenterText.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
            CenterText.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;
			
			//CellFormat = FinalFont1.ToDirectWriteTextFormat();
			
			SharpDX.RectangleF CenterRect = new SharpDX.RectangleF(ChartPanel.X, ChartPanel.Y, ChartPanel.W, ChartPanel.H);
			// Redundant oldAntialiasMode save removed (already saved at line 16866 and set at 17195)


			if (AllErrorMessages.Count > 0)
			{
				RenderTarget.AntialiasMode = SharpDX.Direct2D1.AntialiasMode.Aliased;

				string errorText = string.Empty;

				foreach (string sss in AllErrorMessages)
					errorText = errorText + sss + "   ";

				RenderTarget.FillRectangle(CenterRect, ChartBackgroundErrorBrushDX);
				RenderTarget.DrawText(errorText, CenterText, CenterRect, ChartTextBrushDX);

				RenderTarget.AntialiasMode = oldAntialiasMode;

				// Dispose brushes created before this early return
				ChartTextBrushDX.Dispose();
				ChartBackgroundBrushDX.Dispose();
				ChartBackgroundErrorBrushDX.Dispose();
				ChartBackgroundFadeBrushDX.Dispose();
				ChartBackgroundMenuFadeBrushDX.Dispose();
				buttonBrushDX.Dispose();
				buttonHBrushDX.Dispose();
				buttonFHBrushDX.Dispose();
				buttonFOFFBrushDX.Dispose();
				buttonFONBrushDX.Dispose();
				MainFillBrushDX.Dispose();
				BidDotsBrushDX.Dispose();
				AskDotsBrushDX.Dispose();
				BidTriBrushDX.Dispose();
				AskTriBrushDX.Dispose();
				MagnetFillBrushDX.Dispose();
				UFAFillBrushDX.Dispose();
				POCFillBrushDX.Dispose();
				VWAPFillBrushDX.Dispose();
				MainPrintBrushDX.Dispose();
				LastPriceBrushDX.Dispose();
				CrossHairBrushDX.Dispose();
				TransparentBrushDX.Dispose();
				PrintLongBrushDX.Dispose();
				PrintShortBrushDX.Dispose();
				AboveLongBrushDX.Dispose();
				AboveShortBrushDX.Dispose();
				AboveNeutralBrushDX.Dispose();
				TextBrushDX1.Dispose();
				Plot1BrushDX.Dispose();
				Plot2BrushDX.Dispose();
				Plot3BrushDX.Dispose();
				Plot4BrushDX.Dispose();
				Plot5BrushDX.Dispose();
				Plot6BrushDX.Dispose();
				Plot7BrushDX.Dispose();
				Plot8BrushDX.Dispose();
				MainTextColorDX.Dispose();
				BidFillDX.Dispose();
				BidTextDX.Dispose();
				AskFillDX.Dispose();
				AskTextDX.Dispose();
				LVFillDX.Dispose();
				AxisBrushDX.Dispose();
				BackgroundColorDX.Dispose();
				TimerMainColorDX.Dispose();
				ClickUPColorDX.Dispose();
				ClickDNColorDX.Dispose();
				GreenBrushDX.Dispose();
				RedBrushDX.Dispose();
				GrayBrushDX.Dispose();
				BidHashDX.Dispose();
				AskHashDX.Dispose();
				BottomText.Dispose();
				CenterText.Dispose();
				return;
			}
			
			
	

          //  Print("F");

           // if (ChartControl.Properties.ChartBackground == blackBrush)
            {
               // Print("asf");

                //buttonHBrushDX.Opacity = 0.7f;
                //buttonFHBrushDX.Opacity = 0.4f;
                //buttonFONBrushDX.Opacity = 0.9f;

            }
           // else
            {
               buttonHBrushDX.Opacity = 0.5f;
               // buttonFHBrushDX.Opacity = 0.0f;
                //buttonFONBrushDX.Opacity = 0.4f;
            }

            

            float CY = (float)chartControl.CanvasRight - 48f;
			


			// XX Bar Values
			
			double pxL2 = 0;
			double pxL1 = 0;
			
			int StartLoopBar = Math.Max(0, FB - 5);
			int EndLoopBar = LB + 5; //Math.Min(CurrentBars[0], LB + 5);
			
			
			
			// adjusted to fix proper display of swing levels
			
			//StartLoopBar = 0;  // this line really slows charts down
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
				
					PriceBox PP = new PriceBox();
					
					PP.Top = xL;
					PP.Bottom = xR;
					PP.Height = xW;
				
					storep = i;
					
					if (PriceX1Boxes.ContainsKey(storep))
						PriceX1Boxes.TryUpdate(storep,PP,PriceX1Boxes[storep]);
					else
						PriceX1Boxes.TryAdd(storep,PP);
				
					
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
					
				PP = new PriceBox();
				
				PP.Top = pxL2;
				PP.Bottom = xRa;
				PP.Height = xWa;

				storep = storep - 1;	
					
				if (pxL2 != 0)
					
				if (PriceX2Boxes.ContainsKey(storep))
					PriceX2Boxes.TryUpdate(storep,PP,PriceX2Boxes[storep]);
				else
					PriceX2Boxes.TryAdd(storep,PP);
				
				pxL2 = xLa;
				
				

				

			}
		
				if (RenderTest(5)) return; // 0
				 
				
			int prevbottom2 = 0;
				
			TopPrice = ChartPanel.MaxValue; // actual top and bottom of price panel
			BottomPrice = ChartPanel.MinValue;
			
			
			TopPrice = TopPrice + ThisTickSizze*2;
			BottomPrice = BottomPrice - ThisTickSizze*2;
			
	
			
			
			double TotalPrice = TopPrice - BottomPrice;
			
			//double expandticks = TotalPrice / ThisTickSizze;
			
			
			double StartPrice = RTTS(BottomPrice - TotalPrice*5);
			double EndPrice = RTTS(TopPrice + TotalPrice*5);
			
			
			
			double AverageBoxHeight = 0;
			
			double TotalBoxHeight = 0;
			double TotalLevels = 0;
			
			
			int pBoxSpace = 1;
			int barSpacing = pBoxSpace;

			
			
			double yy1 = 0;
			double yy2 = 0;
			double yy3 = 0;
			storep = 0;			
	
			double yA5 = 0;
			double yB5 = 0;
			double yH5 = 0;
			double yT5 = 0;					

			CurrentHighRender = EndPrice;
			CurrentLowRender = StartPrice;
			
			CurrentHighRender2 = TopPrice;
			CurrentLowRender2 = BottomPrice;
			
			//Print(CurrentHighRender);
			
			bool DoRender = CurrentHighRender2 != PreviousHighRender2 || CurrentLowRender2 != PreviousLowRender2;
			double jj = StartPrice; // do
			

			PreviousHighRender = CurrentHighRender;
			PreviousLowRender = CurrentLowRender;	
			
			PreviousHighRender2 = TopPrice;
			PreviousLowRender2 = BottomPrice;				
			
			
			
//			if (!GetBoxYPixelStatus)
//				DoRender = true;

//			DoRender = true;
			
			
			
			if (DoRender)
			//TriggerCustomEvent(o =>
   			{

				try 
				{
				    
					//PriceBoxes1.Clear();
					PriceBoxes2.Clear();
					
					
					MaxBoxHeight = 0;
					
					do
					{
						//double thePrice = jj;

						  						
							yy1 = chartScale.GetYByValue(jj);
													
							y1 = yy2;
							y3 = yy1;
							y2 = yy3;

							yA5 = y1 - (y1 - y3) / 2 + 0;
							yB5 = y1 + (y2 - y1) / 2 - 1;
							yH5 = yB5-yA5;
							yT5 = yA5;
							
							halfRowHeight = (int) Math.Round(yH5 / 2, 0);
									

							PriceBox PP = new PriceBox();
							
							PP.Top = yT5;
							PP.Bottom = yB5;
							PP.Height = yH5;

							if (yy1 != 0 && yy2 != 0 && yy3 != 0)
							{
								
									 if (!PriceBoxes2.ContainsKey(storep))
									PriceBoxes2.Add(storep,PP);	
							}

							yy3 = yy2;
							yy2 = yy1;
							
							storep = jj;
							
								
							jj = RTTS(jj + ThisTickSizze); // do
							
							MaxBoxHeight = Math.Max(MaxBoxHeight,yH5);
							
							TotalBoxHeight = TotalBoxHeight + yH5;
							TotalLevels = TotalLevels + 1;
									
						
							
								
					}	
					while (jj <= EndPrice); // do
					
					AverageBoxHeight = TotalBoxHeight/TotalLevels;
				
					
				}
					

				
				catch (Exception ex)
				{
					if (TestRender) Print("PriceBoxes1: " + ex.Message + " " + DateTime.Now.ToString("T") + " | " + StartPrice + " | " + jj + " | " + yT5 + " | " + yB5 + " | " + yH5);
					
				}
				

			}//, null);
			
			
				if (RenderTest(6)) return; // 0

			// end get y price boxes
			
			// set size of all text on the indicaotr
			
			
			int biggesttextsize = 0;
			
			if (MaxBoxHeight != MaxBoxHeightP)
			{
			
				foreach (KeyValuePair<int, double> KKK in HeightToTextSize)
				{
					if (KKK.Value < MaxBoxHeight + 5) // increase number to allow for tighter fix of text vertically in box
						biggesttextsize = Math.Max(biggesttextsize, KKK.Key);
					
					
				}
			}
			
			MaxBoxHeightP = MaxBoxHeight;
			
			//Print(FinalTextSize1);
			
			if (biggesttextsize != 0)
			{
				FinalTextSize1 = Math.Min(biggesttextsize, (int)pTextFont2.Size);
				FinalTextSize2 = Math.Min(biggesttextsize, (int)pTextFont2Imb.Size);
			}
			
			if (FinalTextSize1 != FinalTextSize1P)
			{
				FinalFont1 = new SimpleFont(pTextFont2.Family.ToString(), FinalTextSize1);					
				FinalFont1.Bold = pTextFont2.Bold;
				FinalFont1.Italic = pTextFont2.Italic;
				
			}
			
			if (FinalTextSize2 != FinalTextSize2P)
			{
				FinalFont2 = new SimpleFont(pTextFont2Imb.Family.ToString(), FinalTextSize2);
				FinalFont2.Bold = pTextFont2Imb.Bold;
				FinalFont2.Italic = pTextFont2Imb.Italic;
				
			}
			
			FinalTextSize1P = FinalTextSize1;
			FinalTextSize2P = FinalTextSize2;
		
			
			
		
			
			
						
			if (pPrintEnabled)
				halfRowHeight = 0;
			

			
			
			if (!IsDragging)
			if (pCrossHairEnabled)
			{
				if (pCrossHairStyle == "Line" || pCrossHairStyle == "Line + Box")
				{
					// Line-style crosshair
					if (pCrossHairXEnabled)
					{
						double yMid = GetBoxYPixel(CurrentMousePrice, "Top") + GetBoxYPixel(CurrentMousePrice, "Height") / 2.0;
						RenderTarget.DrawLine(
							new SharpDX.Vector2(0, (float)yMid),
							new SharpDX.Vector2(ChartPanel.W, (float)yMid),
							CrossHairBrushDX, 1.0f);
					}

					if (pCrossHairYEnabled && ThisMouseBar <= LB)
					{
						x1 = GetBoxXPixel(ThisMouseBar, "Left", true);
						x2 = GetBoxXPixel(ThisMouseBar, "Width", true);
						float xMid = (float)(x1 + x2 / 2.0);
						RenderTarget.DrawLine(
							new SharpDX.Vector2(xMid, 0),
							new SharpDX.Vector2(xMid, ChartPanel.H),
							CrossHairBrushDX, 1.0f);

						// Line + Box: highlight the hovered cell
						if (pCrossHairStyle == "Line + Box")
						{
							double yA = GetBoxYPixel(CurrentMousePrice, "Top");
							double yH = GetBoxYPixel(CurrentMousePrice, "Height");
							SharpDX.RectangleF cellRect = new SharpDX.RectangleF((float)x1, (float)yA, (float)x2, (float)yH);
							RenderTarget.DrawRectangle(cellRect, CrossHairBrushDX, 1.5f);
						}
					}
				}
				else
				{
					// Default "Box" mode
					if (pCrossHairXEnabled)
						DrawLastPriceBox(chartControl, chartScale, CrossHairBrushDX, CurrentMousePrice);

					if (pCrossHairYEnabled && ThisMouseBar <= LB)
					{
						x1 = GetBoxXPixel(ThisMouseBar, "Left", true);
						x2 = GetBoxXPixel(ThisMouseBar, "Width", true);

						double yA = GetBoxYPixel(CurrentMousePrice, "Top");
						double yB = GetBoxYPixel(CurrentMousePrice, "Bottom");

						ThisBrushDX = CrossHairBrushDX;

						ThisRect = new SharpDX.RectangleF((float)x1, (float)0, (float)x2, (float)yA);
						RenderTarget.FillRectangle(ThisRect, ThisBrushDX);

						ThisRect = new SharpDX.RectangleF((float)x1, (float)yB, (float)x2, (float)5000);
						RenderTarget.FillRectangle(ThisRect, ThisBrushDX);
					}
				}
			}
				
			
			if (pShowLastPriceAll)
			if (pShowLastPriceBox)
				DrawLastPriceBox(chartControl, chartScale, LastPriceBrushDX, CurrentLastData);
				
			
			
			 
			
			
				
			// ZONES

			try
			{
				
				DrawZones(chartControl, chartScale);
				
			}
			catch (Exception ex)
			{
				if (TestRender) Print("OnRender DrawZones: " + ThisMasterInstrument.Name + " " + ex.Message + " " + DateTime.Now.ToString("T"));
				
			}
			
			// COMPOSITE LEVELS

			try
			{
				
				DrawCompositeLevels(chartControl, chartScale, false);
			
			}
			catch (Exception ex)
			{
				if (TestRender) Print("OnRender DrawCompositeLevels: " + ThisMasterInstrument.Name + " " + ex.Message + " " + DateTime.Now.ToString("T"));
				
			}
			
			// SWING LEVELS
			
			
			try
			{				
				DrawSwingLevels(chartControl, chartScale);
			}
			catch (Exception ex)
			{
				if (TestRender) Print("OnRender DrawSwingLevels: " + ThisMasterInstrument.Name + " " + ex.Message + " " + DateTime.Now.ToString("T"));
				
			}
			
			// MAGNETS AND UNFINISHED AUCTIONS
			try
			{
				
			if (pMAGEnabled || pUFAEnabled)
			{
			int extStart = Math.Max(1, FB - 200); // lookback for extensions that started off-screen
			int extEnd = LB;
			for (int i = extStart; i <= extEnd; i++)
			{
				
				if (!BarItems.IsValidDataPointAt(i))
					continue;
				
				if (BarItems.GetValueAt(i) == null)
					continue;
			
				if (pMAGEnabled)
			   	if (pMAGEnabled2 || pMAGEnabled5)
				{
					// Iterate directly — no defensive copy needed (list not mutated during render)
					DrawExtensions(i, BarItems.GetValueAt(i).Magnets, pMagnetStroke, pMagnetStroke2, chartControl, chartScale, pMAGEnabled2, pMAGEnabled5);
				}

				if (pUFAEnabled)
			    if (pUFAEnabled2 || pUFAEnabled5)
				{
					// Iterate directly — no defensive copy needed (list not mutated during render)
					DrawExtensions(i, BarItems.GetValueAt(i).UnfinishedAuctions, pUFAStroke, pUFAStroke2, chartControl, chartScale, pUFAEnabled2, pUFAEnabled5);
				}

			}
			} // end if (pMAGEnabled || pUFAEnabled)

			}
			catch (Exception ex)
			{
				if (TestRender) Print("OnRender DrawExtensions: " + ThisMasterInstrument.Name + " " + ex.Message + " " + DateTime.Now.ToString("T"));
				
			}
			
			// COMPOSITE LEVELS TEXT
			try
			{			
			DrawCompositeLevels(chartControl, chartScale, true);
			}
			catch (Exception ex)
			{
				if (TestRender) Print("OnRender DrawCompositeLevels Text: " + ThisMasterInstrument.Name + " " + ex.Message + " " + DateTime.Now.ToString("T"));
				
			}			
			// FOOTPRINT
			
			
			
			
			double maxv = 0;
			
			try
			{
				
				// get max text width
				for (int i = PrintFB; i <= LB; i++) 
				{
					BarItem BBB = null;
					
					if (BarItems.IsValidDataPointAt(i))
						BBB = BarItems.GetValueAt(i);

					double LastPrice = RTTS(Highs[0].GetValueAt(i));
					double LowPrice = Lows[0].GetValueAt(i);
					
					if (BBB != null)	
					do
					{			
						RowData _rd;
						if (BBB.l.TryGetValue(LastPrice, out _rd))
						{
							double BidVol = _rd.bv;
							double AskVol = _rd.av;
							double TotalVol = _rd.tv;

							if (isBidAskMode)
							{
								maxv = Math.Max(maxv,BidVol);
								maxv = Math.Max(maxv,AskVol);

							}
							else
							{
								maxv = Math.Max(maxv,TotalVol);

							}
						}
					
						LastPrice = RTTS(LastPrice - ThisTickSizze);
					} 
					while (LastPrice >= LowPrice);	
					
				}
			
			}
			
			catch (Exception ex)
			{
				if (TestRender) Print("OnRender Check: " + ThisMasterInstrument.Name + " " + ex.Message + " " + DateTime.Now.ToString("T"));
				
			}
			
			
			string CellString2 = maxv.ToString();
			CellFormat = FinalFont1.ToDirectWriteTextFormat();
			CellFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
			CellFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
			CellFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;

			CellFormatImb = FinalFont2.ToDirectWriteTextFormat();
			CellFormatImb.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
			CellFormatImb.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
			CellFormatImb.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;

			CellLayout = new TextLayout(Core.Globals.DirectWriteFactory, CellString2, CellFormat, 10000, 10000);

  			//Print(AverageBoxHeight);

            MaxTextXPixels = CellLayout.Metrics.Width + 3-10; // decrease to allow less space at left and right edge of text
			//MaxTextYPixels = CellLayout.Metrics.Height + -3-10; // decrease to allow less space at top and bottom edge of text  // discontinued
			CellLayout.Dispose();
			
			
			// PRINT BAR DISPLAY

			// Pre-create stacked imbalance brushes outside per-bar loop (avoid per-bar ToDxBrush)
			SharpDX.Direct2D1.Brush stackBidBrush = null;
			SharpDX.Direct2D1.Brush stackAskBrush = null;
			if (pStackedImbalanceEnabled)
			{
				stackBidBrush = pStackedImbalanceBidColor.ToDxBrush(RenderTarget);
				stackAskBrush = pStackedImbalanceAskColor.ToDxBrush(RenderTarget);
			}

			// Pre-create Above/Below/Timer TextFormats once per frame (avoid per-bar ToDirectWriteTextFormat)
			SharpDX.DirectWrite.TextFormat aboveBelowFormat = null;
			SharpDX.DirectWrite.TextFormat timerFormat = null;
			if (pAboveTotalMode != "None" || pBelowTotalMode != "None")
			{
				aboveBelowFormat = pTextFont4.ToDirectWriteTextFormat();
				aboveBelowFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
				aboveBelowFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
				aboveBelowFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;
			}
			if (pTimerEnabled)
			{
				timerFormat = pTextFontTime.ToDirectWriteTextFormat();
				timerFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
				timerFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
				timerFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;
			}

			// Reusable lists for stacked imbalance regions (avoid per-bar allocation)
			List<double[]> stackedBidRegions = pStackedImbalanceEnabled ? new List<double[]>() : null;
			List<double[]> stackedAskRegions = pStackedImbalanceEnabled ? new List<double[]>() : null;

			try
			{

			PriceRowHeight = 100000;

			for (int i = PrintFB; i <= LB; i++)
			{
				x1 = chartControl.GetXByBarIndex(ChartBars,i);
				
				double xL = x1 - barWidth - 1;
				//double xL2 = x1;
				double xR = x1 + barWidth;
				double xW = barWidth + barWidth + 1;
				double xW2 = barWidth;
				
				xL = GetBoxXPixel(i,"Left",true);
//				//xL2 = x1;
				xR = GetBoxXPixel(i,"Right",true);
				xW = GetBoxXPixel(i,"Width",true);
				xW2 = barWidth + 1;
				
				double LastPrice = RTTS(Highs[0].GetValueAt(i));
				double LowPrice = Lows[0].GetValueAt(i);

				if (LastPrice < BottomPrice || LowPrice > TopPrice)
					continue;

				// Cache per-bar series lookups — read each Series once instead of 5+ times per cell
				double barBodyLow = BodyLow.GetValueAt(i);
				double barBodyHigh = BodyHigh.GetValueAt(i);
				double barDirection = Direction.GetValueAt(i);
				double barHigh = Highs[0].GetValueAt(i);
				double barLow = Lows[0].GetValueAt(i);

				// loop through price levels

				BarItem BBB = null;

				if (BarItems.IsValidDataPointAt(i))
					BBB = BarItems.GetValueAt(i);

					// CellFormat/CellFormatImb created once before the loop — no per-bar recreation needed
					// (Above/Below/Timer now use aboveBelowFormat/timerFormat instead of overwriting CellFormat)


					CellString = "";
					
					float boxsize = pRSSize;
				
			
				if (BBB != null)
				{

					// Stacked imbalance tracking
					int stackedBidCount = 0;
					int stackedAskCount = 0;
					double stackedBidTopPrice = 0;
					double stackedBidBottomPrice = 0;
					double stackedAskTopPrice = 0;
					double stackedAskBottomPrice = 0;
					if (pStackedImbalanceEnabled)
					{
						stackedBidRegions.Clear();
						stackedAskRegions.Clear();
					}

					// Pre-build HashSets for O(1) magnet/UFA lookup instead of per-cell linear scan
					HashSet<double> magnetPrices = null;
					HashSet<double> ufaPrices = null;
					if (pMAGEnabled && BBB.Magnets.Count > 0)
						magnetPrices = new HashSet<double>(BBB.Magnets.Select(m => m.Price));
					if (pUFAEnabled && BBB.UnfinishedAuctions.Count > 0)
						ufaPrices = new HashSet<double>(BBB.UnfinishedAuctions.Select(u => u.Price));

					do
					{
						
						
//				if (LastPrice > TopPrice || LastPrice < BottomPrice)
//					continue;
				
						//LastPrice = Price.Key;  //foreach
						
						//BarItem BBB = BarItems.GetValueAt(i);
						
						double BidVol = 0;
						double AskVol = 0;
						double TotalVol = 0;
						double DeltaVol = 0;
						double ThisMaxVol = 0;
						
						bool IsBidImb = false;
						bool IsAskImb = false;
						
						double BidBlockTotal = 0;
						double AskBlockTotal = 0;
						
						bool IsMagnet = false;
						bool IsUFA = false;
						bool IsPOC = false;
						bool IsVWAP = false;
						

						
						bool pppp = false;
						
						//if (pppp)
						RowData _rdCell;
						if (BBB.l.TryGetValue(LastPrice, out _rdCell))
						{
							BidVol = _rdCell.bv;
							AskVol = _rdCell.av;
							TotalVol = _rdCell.tv;
							DeltaVol = Math.Abs(AskVol - BidVol);

							RowData _rdPOC;
							if (TotalVol != 0 && BBB.l.TryGetValue(BBB.POC, out _rdPOC))
								ThisMaxVol = _rdPOC.tv;
							
							
							if (pShowImbalance)
							{
								if (BBB.BidI != null)
									IsBidImb = BBB.BidI.Contains(LastPrice);

								if (BBB.AskI != null)
									IsAskImb = BBB.AskI.Contains(LastPrice);
							}

							// Stacked imbalance tracking
							if (pStackedImbalanceEnabled)
							{
								if (IsBidImb)
								{
									if (stackedBidCount == 0) stackedBidTopPrice = LastPrice;
									stackedBidBottomPrice = LastPrice;
									stackedBidCount++;
								}
								else
								{
									if (stackedBidCount >= pStackedImbalanceCount)
										stackedBidRegions.Add(new double[] { stackedBidTopPrice, stackedBidBottomPrice });
									stackedBidCount = 0;
								}

								if (IsAskImb)
								{
									if (stackedAskCount == 0) stackedAskTopPrice = LastPrice;
									stackedAskBottomPrice = LastPrice;
									stackedAskCount++;
								}
								else
								{
									if (stackedAskCount >= pStackedImbalanceCount)
										stackedAskRegions.Add(new double[] { stackedAskTopPrice, stackedAskBottomPrice });
									stackedAskCount = 0;
								}
							}

	//						if (pShowBlocks)
	//						{
							
	//							if (BBB.BidBlocks != null)
	//							if (BBB.BidBlocks.ContainsKey(LastPrice))
	//								BidBlockTotal = BBB.BidBlocks[LastPrice].Count;
								
	//							if (BBB.AskBlocks != null)	
	//							if (BBB.AskBlocks.ContainsKey(LastPrice))
	//								AskBlockTotal = BBB.AskBlocks[LastPrice].Count;
	//						}
							
							
								
							
							IsMagnet = magnetPrices != null && magnetPrices.Contains(LastPrice);
							IsUFA = ufaPrices != null && ufaPrices.Contains(LastPrice);
							
							if (pBarCompositeEnabled)
							{
								if (pDisplayPOC)
									IsPOC = BBB.POC == LastPrice;	
								
								if (pDisplayVWAP)
									IsVWAP = RTTS(BBB.VWAP) == LastPrice;								
							}
								
						
						}
						
					
					
						
						y1 = chartScale.GetYByValue(LastPrice);
						
						double yA = GetBoxYPixel(LastPrice,"Top");
						double yB = GetBoxYPixel(LastPrice,"Bottom");
						double yH = GetBoxYPixel(LastPrice,"Height");
						double yT = yA;					
						
						//if (rowheight == 0)
							PriceRowHeight = Math.Min(PriceRowHeight,yH);
						
						StartPoint	= new Point(xL,y1);
						EndPoint = new Point(xR,y1);
						
						//ThisStroke = pBrush01;
						//RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ThisStroke.BrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
						

						//if (!pZoomEnabled)
						if (pPrintEnabled)
						{
							
			
							// set opacity
							
								//pPrintBarOpacityMode
							
							double ThisVol = TotalVol;
							double MinVOL = 0;
							double MaxVOL = ThisMaxVol;
							double OpacityLow2 = 0;
							double OpacityHigh2 = 0;
							double OpacityMultiplier2 = 0;
							double ThisOpacity2 = 0;
							
						if (isVolumeOpacity)
						{
							MinVOL = 0;
							MaxVOL = ThisMaxVol;

							ThisVol = Math.Max(0,TotalVol);
								OpacityLow2 = pPrintMinOpacity;
								OpacityHigh2 = pPrintMaxOpacity;
								double normalizedVol = MaxVOL > 0 ? Math.Max(0, (ThisVol - MinVOL)) / MaxVOL : 0;
								// Skip Math.Pow when gamma is 1.0 (default) — identity transform
								if (pHeatmapGamma != 1.0)
									normalizedVol = Math.Pow(normalizedVol, pHeatmapGamma);
								ThisOpacity2 = Math.Round(OpacityLow2 + (OpacityHigh2 - OpacityLow2) * normalizedVol, 0);

							if (ThisVol == 0)
								ThisOpacity2 = 0;

						}
						else if (isDeltaOpacity)
						{

								MaxVOL = Math.Max(Math.Abs(BBB.dvmin), BBB.dvmax);
								MinVOL = Math.Min(Math.Abs(BBB.dvmin), BBB.dvmax);

								ThisVol = Math.Abs(AskVol-BidVol);
								OpacityLow2 = pPrintMinOpacity;
								OpacityHigh2 = pPrintMaxOpacity;
								double deltaRange = Math.Max(1, MaxVOL - MinVOL);
								double normalizedDelta = Math.Max(0, (ThisVol - MinVOL)) / deltaRange;
								// Skip Math.Pow when gamma is 1.0 (default) — identity transform
								if (pHeatmapGamma != 1.0)
									normalizedDelta = Math.Pow(normalizedDelta, pHeatmapGamma);
								ThisOpacity2 = Math.Round(OpacityLow2 + (OpacityHigh2 - OpacityLow2) * normalizedDelta, 0);

						}		
						else
						{
							ThisOpacity2 = (pPrintMaxOpacity+pPrintMinOpacity)/2;	
						}
						
						
						PrintLongBrushDX.Opacity = (float)ThisOpacity2/100f;
						PrintShortBrushDX.Opacity = (float)ThisOpacity2/100f;
						
//						MagnetFillBrushDX.Opacity = (float)ThisOpacity2/100f;
//						UFAFillBrushDX.Opacity = (float)ThisOpacity2/100f;
//						VWAPFillBrushDX.Opacity = (float)ThisOpacity2/100f;
//						POCFillBrushDX.Opacity = (float)ThisOpacity2/100f;				
						
						MagnetFillBrushDX.Opacity = (float)pMagnetFillOpacity/100f; //lanest
						UFAFillBrushDX.Opacity = (float)pUFAFillOpacity/100f;
						VWAPFillBrushDX.Opacity = (float)pProfileBarOpacity/100f;
						POCFillBrushDX.Opacity = (float)pProfileBarOpacity/100f;		
						
							// cell fill colors
							
							// main color is set in brush at beginning
							ThisBrushDX = MainPrintBrushDX;
							ThisBrushDX2 = ChartBackgroundBrushDX;
						
						
						
						if (pUseBackgroundColor)
							ThisBrushDX2 = BackgroundColorDX;
						
						
							if (isCandlestickFill)
							{
								if (LastPrice >= barBodyLow && LastPrice <= barBodyHigh)
								{
									if (barDirection == 1)
									{
										ThisBrushDX = PrintLongBrushDX;
									}
									else
									{
										ThisBrushDX = PrintShortBrushDX;
									}
								}
								else
								{
									ThisBrushDX = TransparentBrushDX;
									ThisBrushDX2 = TransparentBrushDX;
								}


							}
							if (isDeltaFill)
							{
									if (AskVol >= BidVol)
									{
										ThisBrushDX = PrintLongBrushDX;
									}
									else
									{
										ThisBrushDX = PrintShortBrushDX;
									}


							}	
							
						
								if (IsMagnet && pMAGEnabled3)
									ThisBrushDX = MagnetFillBrushDX;
								
								if (IsUFA && pUFAEnabled3)
									ThisBrushDX = UFAFillBrushDX;
								
//								if (IsVWAP && VWAPFillBrushDX != TransparentBrushDX)
//									ThisBrushDX = VWAPFillBrushDX;
								
//								if (IsPOC && POCFillBrushDX != TransparentBrushDX)
//									ThisBrushDX = POCFillBrushDX;							
							
							

						
							//bool doit = false;
						
						//if (doit)
							
						//bool DrawTextOK = xW2 >= MaxTextXPixels && PriceRowHeight >= MaxTextYPixels;
								
						bool DrawTextOK = FinalTextSize1 >= minimumtextsize && xW2 >= MaxTextXPixels;
							
	                    CellFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
	                    CellFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
	                    CellFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;
							
							
								//if (pSplit)
							if (isBidAskMode)
							{
								
		
						
								// BID CELLS
								
								FinalThisBrushDX = ThisBrushDX;	
								CellFormatFinal = CellFormat;
								TextBrushDX2 = MainTextColorDX;
						
								if (IsBidImb)
								{
									if (pPrintFillImbalances)
										FinalThisBrushDX = BidFillDX;
									
									if (pPrintTextImbalances)
									{
										CellFormatFinal			= CellFormatImb;
										TextBrushDX2 = BidTextDX;
									}
								}
								
	
								if (pPrintFillLV)
								{
									double totalv = BidVol;
									
									if (totalv <= pLightV)
										FinalThisBrushDX = LVFillDX;
								}
		
								
								
								
								if (IsVWAP && VWAPFillBrushDX != TransparentBrushDX)
									FinalThisBrushDX = VWAPFillBrushDX;
								
								if (IsPOC && POCFillBrushDX != TransparentBrushDX)
									FinalThisBrushDX = POCFillBrushDX;
								
								
								if (isCandlestickFill)
								{
									if (LastPrice >= barBodyLow && LastPrice <= barBodyHigh)
									{

									}
									else
									{
										
										TextBrushDX2 = AxisBrushDX;
									}
									
									
								}							
								
								
							
		

								
								ThisRect = new SharpDX.RectangleF((float)xL, (float)yT, (float)xW2, (float)yH);
								
								if (isCandlestickFill)
								{
									if (LastPrice >= barBodyLow && LastPrice <= barBodyHigh)
										RenderTarget.FillRectangle(ExpandRect(ThisRect,1,1,1,1), ChartBackgroundBrushDX);
								}
								else
								{
									RenderTarget.FillRectangle(ExpandRect(ThisRect,1,1,1,1), ChartBackgroundBrushDX);
								}
								
								RenderTarget.FillRectangle(ThisRect, ThisBrushDX2);
								
								RenderTarget.FillRectangle(ThisRect, FinalThisBrushDX);
								

								CellString = BidVol.ToString();
								if (pPrintEnabled2 && DrawTextOK)
								{
									SharpDX.Direct2D1.Brush finalTextBrush = TextBrushDX2;
									if (pAdaptiveTextColor && DarkTextBrushDX != null && ThisOpacity2 > 70)
										finalTextBrush = DarkTextBrushDX;
									if (pTextShadowEnabled && TextShadowBrushDX != null)
										RenderTarget.DrawText(CellString, CellFormatFinal, new SharpDX.RectangleF(ThisRect.X+1, ThisRect.Y+1, ThisRect.Width, ThisRect.Height), TextShadowBrushDX);
									RenderTarget.DrawText(CellString, CellFormatFinal, ThisRect, finalTextBrush);
								}


								// ASK CELLS
								
								
								
								FinalThisBrushDX = ThisBrushDX;	
								CellFormatFinal = CellFormat;
								TextBrushDX2 = MainTextColorDX;
						
								if (IsAskImb)
								{
									if (pPrintFillImbalances)
										FinalThisBrushDX = AskFillDX;
									
									if (pPrintTextImbalances)
									{
										TextBrushDX2 = AskTextDX;
										CellFormatFinal			= CellFormatImb;
									}
								}
								
								if (pPrintFillLV)
								{
									double totalv = AskVol;
									
									if (totalv <= pLightV)
										FinalThisBrushDX = LVFillDX;
								}
								
								
								if (IsVWAP && VWAPFillBrushDX != TransparentBrushDX)
									FinalThisBrushDX = VWAPFillBrushDX;
								
								if (IsPOC && POCFillBrushDX != TransparentBrushDX)
									FinalThisBrushDX = POCFillBrushDX;
								
								if (isCandlestickFill)
								{
									if (LastPrice >= barBodyLow && LastPrice <= barBodyHigh)
									{

									}
									else
									{
										
										TextBrushDX2 = AxisBrushDX;
									}
									
									
								}	
								
								ThisRect = new SharpDX.RectangleF((float)x1, (float)yT, (float)xW2, (float)yH);
								
								if (isCandlestickFill)
								{
									if (LastPrice >= barBodyLow && LastPrice <= barBodyHigh)
										RenderTarget.FillRectangle(ExpandRect(ThisRect,1,1,1,1), ChartBackgroundBrushDX);
								}
								else
								{
									RenderTarget.FillRectangle(ExpandRect(ThisRect,1,1,1,1), ChartBackgroundBrushDX);
								}
								
								RenderTarget.FillRectangle(ThisRect, ThisBrushDX2);
								
								RenderTarget.FillRectangle(ThisRect, FinalThisBrushDX);
								

								CellString = AskVol.ToString();
								if (pPrintEnabled2 && DrawTextOK)
								{
									SharpDX.Direct2D1.Brush finalTextBrush = TextBrushDX2;
									if (pAdaptiveTextColor && DarkTextBrushDX != null && ThisOpacity2 > 70)
										finalTextBrush = DarkTextBrushDX;
									if (pTextShadowEnabled && TextShadowBrushDX != null)
										RenderTarget.DrawText(CellString, CellFormatFinal, new SharpDX.RectangleF(ThisRect.X+1, ThisRect.Y+1, ThisRect.Width, ThisRect.Height), TextShadowBrushDX);
									RenderTarget.DrawText(CellString, CellFormatFinal, ThisRect, finalTextBrush);
								}
								
				
								
								
								//CellLayout.Dispose();
								
							}
							else
							{
							
			
							
								FinalThisBrushDX = ThisBrushDX;						
								CellFormatFinal = CellFormat;
								TextBrushDX2 = MainTextColorDX;
								
								if (IsAskImb)
								{
									if (pPrintFillImbalances)
										FinalThisBrushDX = AskFillDX;
									
									if (pPrintTextImbalances)
									{
										TextBrushDX2 = AskTextDX;
										CellFormatFinal			= CellFormatImb;
									}
								}

								
								if (IsBidImb)
								{
									if (pPrintFillImbalances)
										FinalThisBrushDX = BidFillDX;
									
									if (pPrintTextImbalances)
									{
										CellFormatFinal			= CellFormatImb;
										TextBrushDX2 = BidTextDX;
									}
								}
								
								
								if (pPrintFillLV)
								{
									double totalv = 0;

								if (isVolumeMode)
									totalv = TotalVol;
								if (isDeltaMode)
									totalv = DeltaVol;
								
								
								
									if (totalv <= pLightV)
										FinalThisBrushDX = LVFillDX;
								}
								
								
								
								
								if (IsVWAP && VWAPFillBrushDX != TransparentBrushDX)
									FinalThisBrushDX = VWAPFillBrushDX;
								
								if (IsPOC && POCFillBrushDX != TransparentBrushDX)
									FinalThisBrushDX = POCFillBrushDX;								
								
								
								ThisRect = new SharpDX.RectangleF((float)xL, (float)yT, (float)xW, (float)yH);
								
								if (isCandlestickFill)
								{
									if (LastPrice >= barBodyLow && LastPrice <= barBodyHigh)
										RenderTarget.FillRectangle(ExpandRect(ThisRect,1,1,1,1), ChartBackgroundBrushDX);
								}
								else
								{
									RenderTarget.FillRectangle(ExpandRect(ThisRect,1,1,1,1), ChartBackgroundBrushDX);
								}
								
								
								RenderTarget.FillRectangle(ThisRect, ThisBrushDX2);
								
								RenderTarget.FillRectangle(ThisRect, FinalThisBrushDX);
							

								if (isVolumeMode)
									CellString = TotalVol.ToString();
								if (isDeltaMode)
									CellString = DeltaVol.ToString();

								if (pPrintEnabled2 && DrawTextOK)
								{
									SharpDX.Direct2D1.Brush finalTextBrush = TextBrushDX2;
									if (pAdaptiveTextColor && DarkTextBrushDX != null && ThisOpacity2 > 70)
										finalTextBrush = DarkTextBrushDX;
									if (pTextShadowEnabled && TextShadowBrushDX != null)
										RenderTarget.DrawText(CellString, CellFormatFinal, new SharpDX.RectangleF(ThisRect.X+1, ThisRect.Y+1, ThisRect.Width, ThisRect.Height), TextShadowBrushDX);
									RenderTarget.DrawText(CellString, CellFormatFinal, ThisRect, finalTextBrush);
								}	
								

				
							}
							
							
							TextBrushDX2 = MainTextColorDX;	
							
							
						}
						else
						{
										
							

							
						}
						
					
					
						
						
						
						LastPrice = RTTS(LastPrice - ThisTickSizze);
						
						
					} // end for each
					
					
					while (LastPrice >= LowPrice);

					// Flush any remaining stacked imbalances at end of bar
					if (pStackedImbalanceEnabled)
					{
						if (stackedBidCount >= pStackedImbalanceCount)
							stackedBidRegions.Add(new double[] { stackedBidTopPrice, stackedBidBottomPrice });
						if (stackedAskCount >= pStackedImbalanceCount)
							stackedAskRegions.Add(new double[] { stackedAskTopPrice, stackedAskBottomPrice });

						// Render stacked imbalance markers (brushes pre-created outside loop)
						float stackWidth = 3f;

						foreach (double[] region in stackedBidRegions)
						{
							double sYT = GetBoxYPixel(region[0], "Top");
							double sYB = GetBoxYPixel(region[1], "Bottom");
							ThisRect = new SharpDX.RectangleF((float)xL - stackWidth - 1, (float)sYT, stackWidth, (float)(sYB - sYT));
							RenderTarget.FillRectangle(ThisRect, stackBidBrush);
						}

						foreach (double[] region in stackedAskRegions)
						{
							double sYT = GetBoxYPixel(region[0], "Top");
							double sYB = GetBoxYPixel(region[1], "Bottom");
							ThisRect = new SharpDX.RectangleF((float)xR + 1, (float)sYT, stackWidth, (float)(sYB - sYT));
							RenderTarget.FillRectangle(ThisRect, stackAskBrush);
						}
					}

				}


						//ThisStroke = pMagnetStroke;
					//	
					
				// show current bar or bar by bar values for testing
				
				double dir = barDirection;
				
				
				// draw bar outlines
				if (isCandlestickFill || !pPrintEnabled)
				{

					y1 = GetBoxYPixel(barBodyHigh,"Top");
					y2 = GetBoxYPixel(barBodyLow,"Bottom");
					y3 = y2 - y1 + 0;
					int expp = 0;



					ThisRect = new SharpDX.RectangleF((float)xL, (float)y1, (float)xW, (float)y3);

					if (dir == 1)
					{
						ThisStroke = pBarUpOutlineStroke;

					}
					else
					{
						ThisStroke = pBarDnOutlineStroke;

					}

					RenderTarget.DrawRectangle(ExpandRect(ThisRect,-1+expp,expp,expp,expp), ThisStroke.BrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);

					if (barBodyHigh != barHigh)
					{
						y1 = GetBoxYPixel(barBodyHigh,"Top");
						y2 = GetBoxYPixel(barHigh,"Top");

						StartPoint	= new Point(x1,y1);
						EndPoint = new Point(x1,y2);
						RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ThisStroke.BrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
					}

					if (barBodyLow != barLow)
					{
						y1 = GetBoxYPixel(barBodyLow,"Bottom");
						y2 = GetBoxYPixel(barLow,"Bottom");

						StartPoint	= new Point(x1,y1);
						EndPoint = new Point(x1,y2);
						RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ThisStroke.BrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
					}

				}
				
				// SIDE OF BAR BODY DISPLAY
				
				if (pBarBodyMode != "None" && !isCandlestickFill)
				{
					
					string FinalBarBodyMode = pBarBodyMode;
					
					bool sppplit = !isBidAskMode;
					
					if (sppplit && pBarBodyMode == "Middle")
						FinalBarBodyMode = "Left";
					
					
					if (dir == 1)
					{
						ThisStroke = pBarBodyUpStroke;
						
					}
					else
					{
						ThisStroke = pBarBodyDownStroke;
						
					}
					
					float xxbd = 0;
					
					if (FinalBarBodyMode == "Left")
						xxbd = (float)xL-0;
					if (FinalBarBodyMode == "Right")
						xxbd = (float)xR+0;
					if (FinalBarBodyMode == "Middle")
						xxbd = (float)x1;
					
					y1 = GetBoxYPixel(barBodyHigh,"Top");
						y2 = GetBoxYPixel(barBodyLow,"Bottom");

						StartPoint	= new Point(xxbd,y1-1);
						EndPoint = new Point(xxbd,y2+1);
					
					RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ChartBackgroundBrushDX, ThisStroke.Width+2, ThisStroke.StrokeStyle);
					
						StartPoint	= new Point(xxbd,y1);
						EndPoint = new Point(xxbd,y2);
										
					
						RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ThisStroke.BrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
					
					
				}
					
				// Cluster Display
				
				if (pDisplayCL && pBarCompositeEnabled)
				{
				
					if (BBB.ClusterTop != 0)
					{
						y1 = GetBoxYPixel(BBB.ClusterTop,"Top");
						y2 = GetBoxYPixel(BBB.ClusterBottom,"Bottom");
						y3 = y2 - y1 + 0;
						int expp = 0;
						
						
						
						ThisRect = new SharpDX.RectangleF((float)xL, (float)y1, (float)xW, (float)y3);
								
						ThisStroke = pClusterStroke;
						RenderTarget.DrawRectangle(ExpandRect(ThisRect,-1+expp,expp,expp,expp), ThisStroke.BrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
					}

				}
				
				
				
				

				if (pAboveTotalMode != "None")
				if (pAboveBarDisplayEnabled)
				{
					// Uses aboveBelowFormat cached once per frame (not per bar)
					double dddd = 0;
					double fd = 0;

						if (pAboveTotalMode == "Delta Percent")
						{
							fd = pNDThreshold;
							dddd = BarDeltaPercent.GetValueAt(i);

							int rounddigits = 0;

							CellString = Math.Abs(Math.Round(dddd,rounddigits)).ToString() + "%";

							if (double.IsNaN(dddd))
								CellString = "NA";
						}
						else
						{
							fd = pNDThreshold2;
							dddd = BarDelta.GetValueAt(i);

							CellString = dddd.ToString();
						}

						if (dddd >= fd)
						{
							ThisBrushDX = AboveLongBrushDX;
						}
						else if (dddd <= -1*fd)
						{
							ThisBrushDX = AboveShortBrushDX;
						}
						else
						{
							ThisBrushDX = AboveNeutralBrushDX;
						}


						if (CellLayout != null && !CellLayout.IsDisposed) CellLayout.Dispose();
						CellLayout = new TextLayout(Core.Globals.DirectWriteFactory, CellString, aboveBelowFormat, 10000, 10000);

						double yA = GetBoxYPixel(RTTS(barHigh),"Top");
						double yH = CellLayout.Metrics.Height;
						double yT = yA - yH - pAboveBarYOffset;

					ThisRect = new SharpDX.RectangleF((float)xL, (float)yT, (float)xW, (float)yH);

					RenderTarget.DrawText(CellString, aboveBelowFormat, ThisRect, ThisBrushDX);
					CellLayout.Dispose();
				}
				
				
				
				

				
				if (pBelowTotalMode != "None")
				if (pAboveBarDisplayEnabled)
				if (!(pTimerEnabled && i == LB && LB >= CurrentBars[0]))   // forming bar: hide Below Bar where the Bar Counter draws
				{
					// Uses aboveBelowFormat cached once per frame (not per bar)
					double dddd = 0;
					double fd = 0;

						if (pBelowTotalMode == "Delta Percent")
						{
							fd = pNDThreshold;
							dddd = BarDeltaPercent.GetValueAt(i);

							int rounddigits = 0;

							CellString = Math.Abs(Math.Round(dddd,rounddigits)).ToString() + "%";

							if (double.IsNaN(dddd))
								CellString = "NA";
						}
						else
						{
							fd = pNDThreshold2;
							dddd = BarDelta.GetValueAt(i);

							CellString = dddd.ToString();
						}

						if (dddd >= fd)
						{
							ThisBrushDX = AboveLongBrushDX;
						}
						else if (dddd <= -1*fd)
						{
							ThisBrushDX = AboveShortBrushDX;
						}
						else
						{
							ThisBrushDX = AboveNeutralBrushDX;
						}


						if (CellLayout != null && !CellLayout.IsDisposed) CellLayout.Dispose();
						CellLayout = new TextLayout(Core.Globals.DirectWriteFactory, CellString, aboveBelowFormat, 10000, 10000);

						double yA = GetBoxYPixel(RTTS(barLow),"Bottom");
						double yH = CellLayout.Metrics.Height;
						double yT = yA + pAboveBarYOffset;

					ThisRect = new SharpDX.RectangleF((float)xL, (float)yT, (float)xW, (float)yH);

					RenderTarget.DrawText(CellString, aboveBelowFormat, ThisRect, ThisBrushDX);
					CellLayout.Dispose();
				}
				
				
				
				
				
				
				
				
				
				
				
				
				
				
				
				
				
				
				
				// TIMER!!!!!!!!!!
				
				if (pTimerEnabled && i == LB && LB >= CurrentBars[0])
				{
				
					// Uses timerFormat cached once per frame (not per bar)
					CellString = BarTimerString;

					if (CellLayout != null && !CellLayout.IsDisposed) CellLayout.Dispose();
					CellLayout = new TextLayout(Core.Globals.DirectWriteFactory, CellString, timerFormat, 10000, 10000);

					double yB2 = GetBoxYPixel(RTTS(barLow),"Bottom") + pTimerOffset;

					double yT2 = CellLayout.Metrics.Height;
					ThisRect = new SharpDX.RectangleF((float)xL, (float)yB2, (float)xW, (float)yT2);

					ThisBrushDX = TimerMainColorDX;
					RenderTarget.DrawText(BarTimerString, timerFormat, ThisRect, ThisBrushDX);

					CellLayout.Dispose();
				}
				
				

				
//							RenderTarget.FillRectangle(ExpandRect(ThisRect,1,1,1,1), ThisBrushDX2);
							
//							RenderTarget.FillRectangle(ThisRect, ThisBrushDX);
							
//							//RenderTarget.DrawRectangle(ThisRect, ThisStroke.BrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
													
//							CellString = BidVol.ToString();
//							if (pPrintEnabled2 && DrawTextOK) RenderTarget.DrawText(CellString, CellFormat, ThisRect, TextBrushDX2);	
			
						
				// CellFormat no longer disposed per bar — disposed once after the loop

			}

			// end print bar display

			}

			catch (Exception ex)
			{
				if (TestRender) Print("OnRender Print Bar: " + ThisMasterInstrument.Name + " " + ex.Message + " " + DateTime.Now.ToString("T"));

			}

			// Dispose footprint TextFormats (created once before the loop)
			if (CellFormat != null && !CellFormat.IsDisposed) CellFormat.Dispose();
			if (CellFormatImb != null && !CellFormatImb.IsDisposed) CellFormatImb.Dispose();

			// Dispose pre-cached stacked imbalance brushes
			SafeDispose(stackBidBrush);
			SafeDispose(stackAskBrush);

			// Dispose pre-cached Above/Below/Timer TextFormats
			SafeDispose(aboveBelowFormat);
			SafeDispose(timerFormat);
			
			
			
			
			
			
			
			
			// IMBALANCE DOTS AND HASH
			
			try
			{
				
			//if (!pZoomEnabled)
				DrawImbalanceDots(chartControl, chartScale);
			
			}
			
			catch (Exception ex)
			{
				if (TestRender) Print("OnRender DrawImbalanceDots: " + ThisMasterInstrument.Name + " " + ex.Message + " " + DateTime.Now.ToString("T"));
				
			}			
		
		
	
			// BLOCK TRADES
			try
			{
				
			DrawBlockTrades(chartControl, chartScale);
			
			}
			
			catch (Exception ex)
			{
				if (TestRender) Print("OnRender DrawBlockTrades: " + ThisMasterInstrument.Name + " " + ex.Message + " " + DateTime.Now.ToString("T"));
				
			}				
			
				
			// END BLOCK TRADES
			
			try
			{
			
			
			if (pShowLastPriceAll)
			if (pShowLastPriceLine)
				DrawLastStrokes(chartControl, chartScale);
			
			}
			
			catch (Exception ex)
			{
				if (TestRender) Print("OnRender DrawLastStrokes: " + ThisMasterInstrument.Name + " " + ex.Message + " " + DateTime.Now.ToString("T"));
				
			}			
			// ARROWS
			try
			{
				DrawWashoutSignals(chartControl, chartScale);
			
			
			}
			
			catch (Exception ex)
			{
				if (TestRender) Print("OnRender DrawWashoutSignals: " + ThisMasterInstrument.Name + " " + ex.Message + " " + DateTime.Now.ToString("T"));
				
			}				
			
			
			
			RightMoveX = 0;

			
			
			
			// order execution menu
			
			bool ShowOldMenu = false;
			
			if (ShowOldMenu)
			if (pOrderPanelOn)
			{
				
				
				
				//if (InMenu2)
				
				if (ShowMenuBackground)
					RenderTarget.FillRectangle(B24, ChartBackgroundMenuFadeBrushDX);
				
				
				topmenu = 0;
				leftmenu = 9999999;
								
				float CrY = (float)chartScale.Height - space + 3;
				
				CrY = 200;
				
				
				foreach (KeyValuePair<double, ButtonZ> thisbutton in AllButtonZ4)
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


                    



                    SharpDX.DirectWrite.TextFormat ButtonText = myProperties.LabelFont.ToDirectWriteTextFormat();

                    ButtonText.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
                    ButtonText.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
                    ButtonText.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;

                    TextLayout textLayout1 = new TextLayout(Core.Globals.DirectWriteFactory, thisbutton.Value.Text, ButtonText, 10000, 10000);

                   // Print(textLayout1.Metrics.Width);

					

                    float FinalH = textLayout1.Metrics.Height;

                    FinalH = Math.Max(pButtonSize, FinalH);

                    float FinalW = Math.Max(FinalH,textLayout1.Metrics.Width);

                    if (thisbutton.Value.Width == 1)
                        FinalW = FinalH;
                    else
                        FinalW = textLayout1.Metrics.Width + 8;

					
					double wwdfw = chartScale.Width * dpiY / 100;
					wwdfw = ChartPanel.X + ChartPanel.W;
					
					
                    CrY = CrY - FinalH;
                    thisbutton.Value.Rect = new SharpDX.RectangleF((float)wwdfw - FinalW - space + 3 , CrY, FinalW, FinalH);
                    CrY = CrY - space;

					topmenu = CrY;
					leftmenu = (float) Math.Min(leftmenu, wwdfw - FinalW);
                    //CY = CY - widdd - space;
                   // thisbutton.Value.Rect = new SharpDX.RectangleF(CY, space, widdd, pButtonSize);
                   
//                   Print(sd);
//					Print(thisbutton.Value.Rect.X);
//					Print(thisbutton.Value.Rect.Y);
					
					//if (InMenu2)
					{
					
						//RenderTarget.FillRectangle(ExpandRect(thisbutton.Value.Rect, 4), ChartBackgroundFadeBrushDX);
							
	                    RenderTarget.FillRectangle(thisbutton.Value.Rect, buttonFOFFBrushDX);
	                    if (thisbutton.Value.Switch)
						{
							if (xxx.Contains("Buy S"))
								RenderTarget.FillRectangle(thisbutton.Value.Rect, GreenBrushDX);
							else if (xxx.Contains("Sell S"))
								RenderTarget.FillRectangle(thisbutton.Value.Rect, RedBrushDX);
							else
								RenderTarget.FillRectangle(thisbutton.Value.Rect, buttonFONBrushDX);
						}

	                    if (MouseIn(thisbutton.Value.Rect, MenuButtonExpandP, MenuButtonExpandP))
	                    {
	                        _hoverClickableThisFrame = true;
	                        if (!thisbutton.Value.Switch)
	                            RenderTarget.FillRectangle(thisbutton.Value.Rect, buttonFHBrushDX);

	                        RenderTarget.DrawRectangle(thisbutton.Value.Rect, buttonHBrushDX, 3);
							
							string tbbb = thisbutton.Value.Name;
							//tbbb = "safasg";
							
							textLayout1 = new TextLayout(Core.Globals.DirectWriteFactory, tbbb, ButtonText, 10000, 10000);
							
							SharpDX.RectangleF NNNNN = new SharpDX.RectangleF(thisbutton.Value.Rect.X - 8 - textLayout1.Metrics.Width ,thisbutton.Value.Rect.Y,200,thisbutton.Value.Rect.Height);
							
							ButtonText.TextAlignment = SharpDX.DirectWrite.TextAlignment.Leading;
							
							RenderTarget.DrawText(tbbb, ButtonText, NNNNN, buttonBrushDX);
							
	                    }

						ButtonText.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
	                    RenderTarget.DrawRectangle(thisbutton.Value.Rect, buttonBrushDX, 1);
	                    RenderTarget.DrawText(thisbutton.Value.Text, ButtonText, thisbutton.Value.Rect, buttonBrushDX);
					textLayout1.Dispose();
					ButtonText.Dispose();

					}
					
					RightMoveX = 0 + (int)wwdfw - (int)leftmenu;
					
                }	
				
				//Print(leftmenu + "   " + topmenu);
				
				B24 = new SharpDX.RectangleF(leftmenu-pMenuOutline-2, 0 + space-1, 2000, 5000);
				

			}
				
				
// COMPOSITE
						
			
//			private int pCompMinOpacity = 10;
	     
//			private int pCompMaxOpacity 
//				pCompDefaultOpacity
			
			try
			{
			
			bool BothOff = pCompHistogramDisplayMode == "None" && pCompNumberDisplayMode == "None";
			
			if (pCompositeLocation != "None")
			if (pCompositeEnabled && !BothOff)			
			{
				
				CompVOLColorDX = pCompVOLColor.ToDxBrush(RenderTarget);
				CompNEColorDX = pCompNEColor.ToDxBrush(RenderTarget);
				CompUPColorDX = pCompUPColor.ToDxBrush(RenderTarget);
				CompDNColorDX = pCompDNColor.ToDxBrush(RenderTarget);
				
				DrawComposite(chartControl, chartScale);
				
				//pCompDefaultOpacity
				
				CompVOLColorDX.Dispose();
				CompNEColorDX.Dispose();
				CompUPColorDX.Dispose();
				CompDNColorDX.Dispose();
				
			
			}		
			
			
			}
			
		catch (Exception ex)
			{
				if (TestRender) Print("OnRender DrawComposite: " + ThisMasterInstrument.Name + " " + ex.Message + " " + DateTime.Now.ToString("T"));
				
			}				
			
			
			
			// HEADS UP DISPLAY
				try
			{
		
			HUDNumber = 0;
			
			MinRightMarginHUD = 0;
			
			if (pHUDEnabled)
			{
				

				HUDVOLColorDX = pHUDVOLColor.ToDxBrush(RenderTarget);
				HUDNEColorDX = pHUDNEColor.ToDxBrush(RenderTarget);
				HUDUPColorDX = pHUDUPColor.ToDxBrush(RenderTarget);
				HUDDNColorDX = pHUDDNColor.ToDxBrush(RenderTarget);
				

//				HUDVOLColorDX.Opacity = pHUDMaxOpacity/100f;
//				HUDNEColorDX.Opacity = pHUDMaxOpacity/100f;
//				HUDUPColorDX.Opacity = pHUDMaxOpacity/100f;
//				HUDDNColorDX.Opacity = pHUDMaxOpacity/100f;
				
				
				if (pHUD1) DrawHUD(TotalVolume, chartControl, chartScale, "Total Volume");
				if (pHUD2) DrawHUD(BarVolume, chartControl, chartScale, "Bar Volume");
				if (pHUD3) DrawHUD(TotalDelta, chartControl, chartScale, "Total Delta");
				if (pHUD4) DrawHUD(BarDelta, chartControl, chartScale, "Bar Delta");
				if (pHUD5)
				{
					DrawHUD(BarAsk, chartControl, chartScale, "Bar Ask");
					DrawHUD(BarBid, chartControl, chartScale, "Bar Bid");					
				}
				
				
	
				HUDVOLColorDX.Dispose();
				HUDNEColorDX.Dispose();
				HUDUPColorDX.Dispose();
				HUDDNColorDX.Dispose();
				
				
			}
			
			
			}
			
			
			catch (Exception ex)
			{
				if (TestRender) Print("OnRender DrawHUD: " + ThisMasterInstrument.Name + " " + ex.Message + " " + DateTime.Now.ToString("T"));
				
			}				
			
				
			

			// MARKET DEPTH
	try
			{			
			DrawMarketDepth(chartControl, chartScale);
			
			}
		catch (Exception ex)
			{
				if (TestRender) Print("OnRender DrawMarketDepth: " + ThisMasterInstrument.Name + " " + ex.Message + " " + DateTime.Now.ToString("T"));
				
			}							
			
			
			
			try
			{
			
			// right margin
			
			if (chartScale.Properties.AutoScaleMarginType != AutoScaleMarginType.Percent)
				chartScale.Properties.AutoScaleMarginType = AutoScaleMarginType.Percent;
			
			
			
			double BasePercentMarginUpperPixels = ChartPanel.ActualHeight * pDefaultTopBottomMargin / 100 + halfRowHeight;		
			double BasePercentMarginLowerPixels = ChartPanel.ActualHeight * pDefaultTopBottomMargin / 100 + halfRowHeight;
			

			
			double pPixelsAboveHUD = 20;
			if (HUDNumber == 0)
				pPixelsAboveHUD = 0;
			
			
			double AdjustPixelsUpper = BasePercentMarginUpperPixels;
			double AdjustPixelsLower = BasePercentMarginLowerPixels + HUDNumber*HUDHeight + pPixelsAboveHUD;
			
			double NewPercentUpper = 100 * AdjustPixelsUpper / ChartPanel.ActualHeight;
			double NewPercentLower = 100 * AdjustPixelsLower / ChartPanel.ActualHeight;
			
			
			//Print("AdjustPixels  " + AdjustPixels);
			//Print("ChartControl.Height  " + ChartPanel.ActualHeight);
			//Print("ChartPanel.Y + ChartPanel.H  " + ChartPanel.Y + ChartPanel.H);
				
			
			chartScale.Properties.AutoScaleMarginLower = NewPercentLower;
			chartScale.Properties.AutoScaleMarginUpper = NewPercentUpper;
				
		if (TestScale)
		{
			
			Print("OnRender: " + TicksAdjust);
			Print("OnRender: " + chartScale.MaxValue);
			Print("OnRender: " + chartScale.MinValue);
			
			double lowdiff = chartScale.MaxMinusMin * NewPercentLower / 100;
			double highdiff = chartScale.MaxMinusMin * NewPercentUpper / 100;
			
			double righthigh = chartScale.MaxValue - highdiff;
			double rightlow = chartScale.MinValue + lowdiff;
			
			Print("OnRender: " + "AAAAA");
			Print("OnRender: " + righthigh);
			Print("OnRender: " + rightlow);
			
			Print("OnRender: Upper P " + NewPercentUpper);
			Print("OnRender: Lower P " + NewPercentLower);
			
		
			Print(ScaleHigh.GetValueAt(CurrentBars[0]));
		
			chartScale.MaxValue = ScaleHigh.GetValueAt(CurrentBars[0]);
			//pDefaultTopBottomMargin
			
		}
		
			// BAR RIGHT MARGIN
			
			int ThisMin = barBetween / 2;
			ThisMin = barBetween-1;
			
		//	if (!pCompositeEnabled
			
			int colsp = pSpaceBetweenColumns;
			
			if (pCompositeLocation != "Right" && !pDepthEnabled)
				colsp = 0;
			
			int InputMarginRight = pThisBarMarginRight + RightMoveX + colsp;
			
			int NewMarginRight = Math.Max(InputMarginRight, (int) MinRightMarginHUD) + ThisMin;
			
			//NewMarginRight = InputMarginRight;
			
			
					// vertical line
				
				
				x1 = ChartPanel.W - InputMarginRight;
			
				CurrentRMX = x1;
			
			
				// stand alone
			
				StartPoint	= new Point(CurrentRMX, 0);
				EndPoint = new Point(CurrentRMX, 5000);
				
			
				MoveRM = new SharpDX.RectangleF((float)StartPoint.X, (float)0, (float)1, (float)5000);						
						
				if (IsMoveRM || IsHoverRM)	
				{
					//ThisStroke = new Stroke(Brushes.WhiteSmoke, DashStyleHelper.Solid, 2);
					ThisStroke = pHighlightStroke;
					RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), VerticalLineHighlightDX, ThisStroke.Width, ThisStroke.StrokeStyle);
				}
				
				
				
				ThisStroke = myProperties.GridLineVPen;
				RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ThisStroke.BrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
				
				
				
			
			
			
			if (myProperties.BarMarginRight != NewMarginRight)
			{
				myProperties.BarMarginRight = NewMarginRight;
				ChartPanel.InvalidateVisual();
			}
			
			
			}
		catch (Exception ex)
			{
				if (TestRender) Print("OnRender Margin: " + ThisMasterInstrument.Name + " " + ex.Message + " " + DateTime.Now.ToString("T"));
				
			}							
			
		
	
			
//						int minRightMargin = (int)(minBarDistance / 2);
//			    minRightMargin = (showBidAsk)  ? (minRightMargin + domWidth + 2)      			 : minRightMargin;
//				minRightMargin = (showProfile) ? (minRightMargin + imbWidth * 2 + 14 + proWidth) : minRightMargin;
			
//			if(chartControl.Properties.BarMarginRight < minRightMargin)
//			{
//				chartControl.Properties.BarMarginRight = minRightMargin;
//			}
			
//			if(chartControl.Properties.BarDistance < minBarDistance)
//			{
//				chartControl.Properties.BarDistance = minBarDistance;
//			}
			
//			if(showDelta && chartScale.Properties.AutoScaleMarginLower < 8)
//			{
//				chartScale.Properties.AutoScaleMarginLower = 8;
//			}
			
//			// ---
			
//			if(autoScroll && chartScale.Properties.YAxisRangeType == YAxisRangeType.Fixed)
//			{
//				rng = chartScale.MaxMinusMin;
//				off = TickSize * 3;
//				dif = TickSize * 4;
				
//				if(prc >= chartScale.MaxValue - off)
//				{
//					chartScale.Properties.FixedScaleMax = prc + dif;
//					chartScale.Properties.FixedScaleMin = (prc + dif) - rng;
//				}
				
//				if(prc <= chartScale.MinValue + off)
//				{
//					chartScale.Properties.FixedScaleMin = (prc - dif);
//					chartScale.Properties.FixedScaleMax = (prc - dif) + rng;
//				}
//			}
						
		try
			{

            if (pButtonsEnabled)
			{
				
				
				space = pSpaceBetweenButtons;

	            // if (MouseIn(B2, 2, 2))

				 B2 = new SharpDX.RectangleF(0, 0, 10000, pButtonSize + space + pMenuOutline);
				
				
				//if (InMenu && ShowMenuBackground)
				//	RenderTarget.FillRectangle(B2, ChartBackgroundMenuFadeBrushDX);
					
				
				leftmenu2 = 0;					
				
				
				if (EnableOrderExecution)
				if (InMenu)
                foreach (KeyValuePair<double, ButtonZ> thisbutton in AllButtonZ1)
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


                    



                    SharpDX.DirectWrite.TextFormat ButtonText = myProperties.LabelFont.ToDirectWriteTextFormat();

                    ButtonText.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
                    ButtonText.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
                    ButtonText.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;

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
                        _hoverClickableThisFrame = true;
                        if (!thisbutton.Value.Switch)
                            RenderTarget.FillRectangle(thisbutton.Value.Rect, buttonFOFFBrushDX);

                        RenderTarget.DrawRectangle(thisbutton.Value.Rect, buttonHBrushDX, 3);

						string tbbb = thisbutton.Value.Name;
						//tbbb = "safasg";
						SharpDX.RectangleF NNNNN = new SharpDX.RectangleF(thisbutton.Value.Rect.X + 8,thisbutton.Value.Rect.Y+18,200,20);
						
						ButtonText.TextAlignment = SharpDX.DirectWrite.TextAlignment.Leading;
						
						RenderTarget.DrawText(tbbb, ButtonText, NNNNN, buttonBrushDX);
						
                    }

                    RenderTarget.DrawRectangle(thisbutton.Value.Rect, buttonBrushDX, 1);
					
					ButtonText.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
                    RenderTarget.DrawText(thisbutton.Value.Text, ButtonText, thisbutton.Value.Rect, buttonBrushDX);
					textLayout1.Dispose();
					ButtonText.Dispose();
                }

				
				
				
				if (InMenu2 && ShowMenuBackground)
				
					RenderTarget.FillRectangle(B22, ChartBackgroundMenuFadeBrushDX);
				
				
				topmenu = 0;
				leftmenu = 9999999;
								
				float CrY = (float)chartScale.Height - space + 3;
				
				
				foreach (KeyValuePair<double, ButtonZ> thisbutton in AllButtonZ2)
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


                    



                    SharpDX.DirectWrite.TextFormat ButtonText = myProperties.LabelFont.ToDirectWriteTextFormat();

                    ButtonText.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
                    ButtonText.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
                    ButtonText.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;

                    TextLayout textLayout1 = new TextLayout(Core.Globals.DirectWriteFactory, thisbutton.Value.Text, ButtonText, 10000, 10000);

                   // Print(textLayout1.Metrics.Width);

					

                    float FinalH = textLayout1.Metrics.Height;

                    FinalH = Math.Max(pButtonSize, FinalH);

                    float FinalW = Math.Max(FinalH,textLayout1.Metrics.Width);

                    if (thisbutton.Value.Width == 1)
                        FinalW = FinalH;
                    else
                        FinalW = textLayout1.Metrics.Width + 8;

					
					double wwdfw = chartScale.Width * dpiY / 100;
					wwdfw = ChartPanel.X + ChartPanel.W;
					
					
                    CrY = CrY - FinalH;
                    thisbutton.Value.Rect = new SharpDX.RectangleF((float)wwdfw - FinalW - space + 3 , CrY, FinalW, FinalH);
                    CrY = CrY - space;

					topmenu = CrY;
					leftmenu = (float) Math.Min(leftmenu, wwdfw - FinalW);
                    //CY = CY - widdd - space;
                   // thisbutton.Value.Rect = new SharpDX.RectangleF(CY, space, widdd, pButtonSize);
                   
//                   Print(sd);
//					Print(thisbutton.Value.Rect.X);
//					Print(thisbutton.Value.Rect.Y);
					
					if (InMenu2)
					{
					
						//RenderTarget.FillRectangle(ExpandRect(thisbutton.Value.Rect, 4), ChartBackgroundFadeBrushDX);
							
	                    RenderTarget.FillRectangle(thisbutton.Value.Rect, buttonFOFFBrushDX);
	                    if (thisbutton.Value.Switch)
	                        RenderTarget.FillRectangle(thisbutton.Value.Rect, buttonFONBrushDX);

	                    if (MouseIn(thisbutton.Value.Rect, MenuButtonExpandP, MenuButtonExpandP))
	                    {
	                        _hoverClickableThisFrame = true;
	                        //if (!thisbutton.Value.Switch)
	                        //    RenderTarget.FillRectangle(thisbutton.Value.Rect, buttonFHBrushDX);

	                        RenderTarget.DrawRectangle(thisbutton.Value.Rect, buttonHBrushDX, 3);
							
							string tbbb = thisbutton.Value.Name;
							//tbbb = "safasg";
							
							textLayout1 = new TextLayout(Core.Globals.DirectWriteFactory, tbbb, ButtonText, 10000, 10000);
							
							SharpDX.RectangleF NNNNN = new SharpDX.RectangleF(thisbutton.Value.Rect.X - 8 - textLayout1.Metrics.Width ,thisbutton.Value.Rect.Y,200,thisbutton.Value.Rect.Height);
							
							ButtonText.TextAlignment = SharpDX.DirectWrite.TextAlignment.Leading;
							
							RenderTarget.DrawText(tbbb, ButtonText, NNNNN, buttonBrushDX);
							
	                    }

						ButtonText.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
	                    RenderTarget.DrawRectangle(thisbutton.Value.Rect, buttonBrushDX, 1);
	                    RenderTarget.DrawText(thisbutton.Value.Text, ButtonText, thisbutton.Value.Rect, buttonBrushDX);
					textLayout1.Dispose();
					ButtonText.Dispose();

					}
					
                }	
				
				//Print(leftmenu + "   " + topmenu);
				
				B22 = new SharpDX.RectangleF(leftmenu-pMenuOutline-2, topmenu-pMenuOutline + space-1, 200, 500);
				

				
				// Pop chart-content clip before rendering panel
				if (panelClipActive) { RenderTarget.PopAxisAlignedClip(); panelClipActive = false; }

				// MENU 3 — Shared Button Panel System
				if (!IsInHitTest && cachedHeaderTextFormat != null && cachedButtonTextFormat != null)
				{
					bool anyMenuOpen = IsAnyPanelMenuOpen();
					bool expanded = IsPanelExpanded();
					bool showButtons = anyMenuOpen || InMenu3;

					// Detect a left Y-axis on the chart panel and offset our panel so
					// it renders past the axis. Single source of truth — same helper
					// used by the panelZoneW clip so they stay aligned.
					float panelLeftX = ComputePanelLeftXForClip(chartControl);
					// Add a few extra pixels of breathing room between the axis and
					// the buttons so the gap matches what the user sees when no left
					// axis is present.
					if (panelLeftX > 0) panelLeftX += S(2);

					// Default B232/B23 detection zones. Width matches last-known
					// panel width so wider headers can be hovered before the
					// menu opens.
					float defaultZoneW = Math.Max(S(80), GetMaxPanelWidth() + S(16));
					B232 = new SharpDX.RectangleF(0, 0 + space - 1, defaultZoneW - panelLeftX, 2000);
					B23 = new SharpDX.RectangleF(0, 0, defaultZoneW - panelLeftX, 2000);

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
							panelLeftX, PANEL_TOP_MARGIN - 10, 250, ChartPanel.H - PANEL_TOP_MARGIN + 20);
						RenderTarget.PushAxisAlignedClip(panelClipRect, SharpDX.Direct2D1.AntialiasMode.PerPrimitive);

						CY = GetPanelYOffset();
						float startCY = CY;

						// Measure header
						SharpDX.Size2F headerSize = MeasureText(PANEL_HEADER_TEXT, cachedHeaderTextFormat);
						float headerH = Math.Max(S(22f), headerSize.Height + S(9));
						float headerW = headerSize.Width + S(28);

						// Single-pass measurement for buttons (width + height)
						float maxBtnW = headerW;
						float totalBtnH = 0;
						SharpDX.DirectWrite.TextFormat ButtonText = cachedButtonTextFormat;
						if (expanded || panelAnimProgress > 0.01f)
						{
							bool measLastBlank = false;
							bool measHasButton = false;
							foreach (KeyValuePair<double, ButtonZ> tb in AllButtonZ3)
							{
								if (tb.Value.Text == "Blank") { if (measHasButton && !measLastBlank) { totalBtnH += 9; measLastBlank = true; } continue; }
								measLastBlank = false;
								measHasButton = true;
								SharpDX.Size2F bs = MeasureText(tb.Value.Text, ButtonText);
								totalBtnH += bs.Height + S(3) + space;
								float bw = bs.Width + S(14);
								if (bw > maxBtnW) maxBtnW = bw;
							}
						}
						float panelW = maxBtnW + S(16);
						float unifiedW = Math.Max(panelW, GetMaxPanelWidth());

						// Update B23/B232 zones to match actual panel width
						B23 = new SharpDX.RectangleF(0, 0, Math.Max(S(80), unifiedW + S(16)) - panelLeftX, 2000);
						B232 = new SharpDX.RectangleF(0, 0 + space - 1, S(20), 2000);

						// Draw header tab
						headerW = headerSize.Width + S(32);
						headerRect = new SharpDX.RectangleF(panelLeftX, CY, headerW, headerH);

						// Instant expand/collapse (no animation)
						panelAnimProgress = expanded ? 1f : 0f;

						// Header stays highlighted when mouse is anywhere in the expanded panel
						bool headerHovered = MouseIn(headerRect, 1, 1)
							|| (panelAnimProgress > 0.5f && MouseIn(new SharpDX.RectangleF(panelLeftX, CY, unifiedW + 16, headerH + totalBtnH + 20), 1, 1));
						if (headerHovered || draggingPanelId != null) _hoverClickableThisFrame = true;

						// Unified backdrop — drawn BEFORE header so header renders on top
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
							if (draggingPanelId == PANEL_ID && Math.Abs(dragOffsetY) > 5f)
							{
								cachedHoverGlowBrushDX.Opacity = 0.06f;
								RenderTarget.FillRectangle(bdRect, cachedHoverGlowBrushDX);
								cachedHoverGlowBrushDX.Opacity = 0.08f;
							}
						}

						FillHeaderButton(headerRect, headerHovered ? cachedHeaderBgHoverBrushDX : cachedHeaderBgBrushDX);

						// Erase rounded-rect overflow below header
						SharpDX.Direct2D1.Brush headerEraseBrush = expanded ? cachedPanelBackdropBrushDX : ChartBackgroundBrushDX;
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

						// Bottom strip
						SharpDX.RectangleF bottomStrip = new SharpDX.RectangleF(
							headerRect.Left, headerRect.Bottom - headerBottomStripH, headerRect.Width, headerBottomStripH);
						RenderTarget.FillRectangle(bottomStrip, headerEraseBrush);
						cachedHoverGlowBrushDX.Opacity = headerHovered ? 0.25f : 0.14f;
						RenderTarget.FillRectangle(bottomStrip, cachedHoverGlowBrushDX);
						cachedHoverGlowBrushDX.Opacity = 0.08f;

						// Crisp 1px separator below header — brighter on hover
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
							// Down arrow
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
							// Right arrow
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

						// Panel collapsed: park the menu buttons off-screen so their stale
						// rects can't be clicked through (e.g. over the Chart Trader header).
						// The block below only refreshes these rects while expanded.
						if (panelAnimProgress <= 0.01f)
						{
							currentButtonHover = -1;
							foreach (KeyValuePair<double, ButtonZ> tb in AllButtonZ3)
								tb.Value.Rect = new SharpDX.RectangleF(-100000f, -100000f, 0f, 0f);
						}

						// Draw buttons when expanded
						if (panelAnimProgress > 0.01f)
						{
							// Pre-pass: find hovered button index. Suppressed while header drag is active.
							currentButtonHover = -1;
							if (draggingPanelId == null && System.Windows.Input.Mouse.LeftButton != System.Windows.Input.MouseButtonState.Pressed)
							{
								int preIdx = 0;
								foreach (KeyValuePair<double, ButtonZ> thisbutton in AllButtonZ3)
								{
									if (thisbutton.Value.Text != "Blank" && MouseIn(thisbutton.Value.Rect, 2, 2))
									{ currentButtonHover = preIdx; break; }
									preIdx++;
								}
							}

							// Batch AA mode for all button rendering
							var savedButtonAA = RenderTarget.AntialiasMode;
							RenderTarget.AntialiasMode = SharpDX.Direct2D1.AntialiasMode.PerPrimitive;

							int buttonIndex = 0;
							bool lastWasBlank = false;
							bool hasDrawnButton = false;
							foreach (KeyValuePair<double, ButtonZ> thisbutton in AllButtonZ3)
							{
								string btext = thisbutton.Value.Text;

								if (btext == "Blank")
								{
									if (!hasDrawnButton || lastWasBlank) { buttonIndex++; continue; }
									lastWasBlank = true;
									// Section divider line
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
								lastWasBlank = false;
								hasDrawnButton = true;

								SharpDX.Size2F btnSize = MeasureText(btext, ButtonText);
								float FinalH = btnSize.Height + S(3);
								float FinalW = btnSize.Width + S(14);

								thisbutton.Value.Rect = new SharpDX.RectangleF(panelLeftX + space, CY, FinalW, FinalH);

								bool btnHovered = (buttonIndex == currentButtonHover);
								if (btnHovered) _hoverClickableThisFrame = true;

								// Inline rounded rect draws.
								// DPI-scale the corner radius so it matches the larger buttons at high DPI.
								float btnR = S(chartButtonRadius);
								var rrect = new SharpDX.Direct2D1.RoundedRectangle() { Rect = thisbutton.Value.Rect, RadiusX = btnR, RadiusY = btnR };

								// Background fill — match Key Levels / Price Action exactly.
								// pButtonHighlightMode swaps which of buttonF{ON,OFF}BrushDX holds
								// the chart background vs accent (see :17630). Map explicitly.
								SharpDX.Direct2D1.Brush bgFillBrushDX =
									(pButtonHighlightMode == "Off") ? buttonFONBrushDX : buttonFOFFBrushDX;
								SharpDX.Direct2D1.Brush accentBrushDX =
									(pButtonHighlightMode == "Off") ? buttonFOFFBrushDX : buttonFONBrushDX;

								// Base fill (chart bg)
								bgFillBrushDX.Opacity = 0.5f;
								RenderTarget.FillRoundedRectangle(rrect, bgFillBrushDX);

								// OFF state overlay (accent color)
								if (!thisbutton.Value.Switch)
								{
									accentBrushDX.Opacity = 0.35f;
									RenderTarget.FillRoundedRectangle(rrect, accentBrushDX);
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
									// Outline brightness also carries state: bright for ON, faint for OFF.
									buttonBrushDX.Opacity = thisbutton.Value.Switch ? 0.55f : 0.2f;
									RenderTarget.DrawRoundedRectangle(rrect, buttonBrushDX, 1f);
								}

								// Text — left-aligned, brighter when ON
								SharpDX.RectangleF textRect = new SharpDX.RectangleF(
									thisbutton.Value.Rect.X + S(7), thisbutton.Value.Rect.Y,
									thisbutton.Value.Rect.Width - S(7), thisbutton.Value.Rect.Height);
								float textOpacity = thisbutton.Value.Switch ? 0.95f : 0.45f;
								buttonBrushDX.Opacity = textOpacity;
								ButtonText.TextAlignment = SharpDX.DirectWrite.TextAlignment.Leading;
								RenderTarget.DrawText(btext, ButtonText, textRect, buttonBrushDX);
								ButtonText.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;

								// Name label to the right of button when hovered
								if (btnHovered && thisbutton.Value.Name != null && thisbutton.Value.Name.Length > 0)
								{
									SharpDX.RectangleF nameRect = new SharpDX.RectangleF(
										thisbutton.Value.Rect.X + 8 + thisbutton.Value.Rect.Width, thisbutton.Value.Rect.Y,
										200, thisbutton.Value.Rect.Height);
									ButtonText.TextAlignment = SharpDX.DirectWrite.TextAlignment.Leading;
									buttonBrushDX.Opacity = 0.6f;
									RenderTarget.DrawText(thisbutton.Value.Name, ButtonText, nameRect, buttonBrushDX);
									ButtonText.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
								}

								CY += FinalH + space;
								buttonIndex++;
							}

							// Restore AA mode and brush opacities
							RenderTarget.AntialiasMode = savedButtonAA;
							buttonBrushDX.Opacity = 1.0f;
							buttonFOFFBrushDX.Opacity = 1.0f;
							buttonFONBrushDX.Opacity = 1.0f;
							buttonHBrushDX.Opacity = 0.4f;
							ChartBackgroundBrushDX.Opacity = 1.0f;
							cachedHoverGlowBrushDX.Opacity = 0.08f;
						}

							// Mirror backdrop formulas so reported height matches the visible bottom.
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

						// Scroll indicators (drawn outside clip)
						{
							var scrollReg = GetScrollRegistry();
							float[] scrollState;
							if (scrollReg.TryGetValue(panelHash, out scrollState) && scrollState[SC_TOTALH] > scrollState[SC_VIEWH])
							{
								float indicatorX = panelLeftX + 40f;
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
						foreach (KeyValuePair<double, ButtonZ> thisbutton in AllButtonZ3)
							thisbutton.Value.Rect = new SharpDX.RectangleF(-100, -100, 0, 0);
					}
				}
				
//				if (topmenu != 0 && InMenu2)
//				RenderTarget.FillRectangle(B22, ChartBackgroundFadeBrushDX);
				
				
				
				
				
				
				
								
//				if (InMenu2 && ShowMenuBackground)
				
//					RenderTarget.FillRectangle(B44, ChartBackgroundMenuFadeBrushDX);
				
				
				// composite buttons
				topmenu = 0;
				leftmenu = 9999999;

				CrY = 30;

				if (InMenu4)
				{
				var savedBtnAA5 = RenderTarget.AntialiasMode;
				RenderTarget.AntialiasMode = SharpDX.Direct2D1.AntialiasMode.PerPrimitive;
				foreach (KeyValuePair<double, ButtonZ> thisbutton in AllButtonZ5)
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

                    float FinalH = textLayout1.Metrics.Height;

                    FinalH = Math.Max(pButtonSize, FinalH);

                    float FinalW = Math.Max(FinalH,textLayout1.Metrics.Width);

                    if (thisbutton.Value.Width == -10)
                        FinalW = FinalH;
                    else
                        FinalW = textLayout1.Metrics.Width + 14;

					float xleft2 = B44.Right - space - FinalW;
					if (pCompositeLocation == "Right")
						xleft2 = B44.Left + space + 1;

                    thisbutton.Value.Rect = new SharpDX.RectangleF(xleft2, CrY, FinalW, FinalH);
                    CrY = CrY + FinalH + space;

					topmenu = CrY;
					leftmenu = (float) Math.Max(leftmenu,FinalW);
                    //CY = CY - widdd - space;
                   // thisbutton.Value.Rect = new SharpDX.RectangleF(CY, space, widdd, pButtonSize);
                   
//                   Print(sd);
//					Print(thisbutton.Value.Rect.X);
//					Print(thisbutton.Value.Rect.Y);
					
					
					if (xxx != "Blank")
					{
						var rrect5 = new SharpDX.Direct2D1.RoundedRectangle() { Rect = thisbutton.Value.Rect, RadiusX = chartButtonRadius, RadiusY = chartButtonRadius };

						// Background fill — pButtonHighlightMode-aware mapping (see :17630).
						// In default "Off" mode, buttonFONBrushDX IS chart background, so
						// using it as an overlay on chart bg would be invisible.
						SharpDX.Direct2D1.Brush bgFillBrushDX5 =
							(pButtonHighlightMode == "Off") ? buttonFONBrushDX : buttonFOFFBrushDX;
						SharpDX.Direct2D1.Brush accentBrushDX5 =
							(pButtonHighlightMode == "Off") ? buttonFOFFBrushDX : buttonFONBrushDX;

						// Base fill (chart bg)
						bgFillBrushDX5.Opacity = 0.5f;
						RenderTarget.FillRoundedRectangle(rrect5, bgFillBrushDX5);

						// OFF state overlay (accent color)
						if (!thisbutton.Value.Switch)
						{
							accentBrushDX5.Opacity = 0.35f;
							RenderTarget.FillRoundedRectangle(rrect5, accentBrushDX5);
						}

						bool btnHov5 = draggingPanelId == null
							&& System.Windows.Input.Mouse.LeftButton != System.Windows.Input.MouseButtonState.Pressed
							&& MouseIn(thisbutton.Value.Rect, 2, 2);
						// Not btnHov5: the hand cursor must persist mid-click while the mouse is still over the button.
						if (MouseIn(thisbutton.Value.Rect, 2, 2)) _hoverClickableThisFrame = true;

						// Hover: bright border + subtle fill
						if (btnHov5)
						{
							buttonHBrushDX.Opacity = 0.12f;
							RenderTarget.FillRoundedRectangle(rrect5, buttonHBrushDX);
							buttonBrushDX.Opacity = 0.85f;
							RenderTarget.DrawRoundedRectangle(rrect5, buttonBrushDX, 1.5f);
						}
						else
						{
							buttonBrushDX.Opacity = 0.2f;
							RenderTarget.DrawRoundedRectangle(rrect5, buttonBrushDX, 1f);
						}

						// Text — left-aligned, brighter when ON
						SharpDX.RectangleF textRect5 = new SharpDX.RectangleF(
							thisbutton.Value.Rect.X + 7, thisbutton.Value.Rect.Y,
							thisbutton.Value.Rect.Width - 7, thisbutton.Value.Rect.Height);
						buttonBrushDX.Opacity = thisbutton.Value.Switch ? 0.95f : 0.45f;
						ButtonText.TextAlignment = SharpDX.DirectWrite.TextAlignment.Leading;
						RenderTarget.DrawText(thisbutton.Value.Text, ButtonText, textRect5, buttonBrushDX);

						// Name label to the right when hovered
						if (btnHov5 && thisbutton.Value.Name != null && thisbutton.Value.Name.Length > 0)
						{
							SharpDX.RectangleF nameRect5 = new SharpDX.RectangleF(
								thisbutton.Value.Rect.X + 8 + thisbutton.Value.Rect.Width, thisbutton.Value.Rect.Y,
								200, thisbutton.Value.Rect.Height);
							buttonBrushDX.Opacity = 0.6f;
							RenderTarget.DrawText(thisbutton.Value.Name, ButtonText, nameRect5, buttonBrushDX);
						}

						textLayout1.Dispose();
					}

                }
				cachedButtonTextFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
				RenderTarget.AntialiasMode = savedBtnAA5;
				buttonBrushDX.Opacity = 1.0f;
				buttonFONBrushDX.Opacity = 1.0f;
				buttonHBrushDX.Opacity = 0.4f;
				ChartBackgroundBrushDX.Opacity = 1.0f;
				}

			}

			}
			 catch (Exception ex)
			{
				if (TestRender) Print("OnRender Buttons: " + ThisMasterInstrument.Name + " " + ex.Message + " " + DateTime.Now.ToString("T"));
				
			}		

			

			
			
						
			
//			try
//			{
				
//				if (BuyClickReady || SellClickReady)
//				{
//					if (BuyClickReady) ThisBrushDX = pClickUPColor.ToDxBrush(RenderTarget);
					
//					if (SellClickReady) ThisBrushDX = pClickDNColor.ToDxBrush(RenderTarget);
//					DrawClickEntry(chartControl, chartScale, ThisBrushDX, CurrentMousePrice);
//				}
			
//			}
//			 catch (Exception ex)
//			{
//				if (TestRender) Print("OnRender Click Entry: " + ThisMasterInstrument.Name + " " + ex.Message + " " + DateTime.Now.ToString("T"));
				
//			}		
			
			
			
				
			if (EnableOrderExecution)
			if (pOrdersDisplayOn)
			try
			{

				
				ThisBrushDX = ClickUPColorDX;
				ThisBrushDX = ClickDNColorDX;

				DrawOrders(chartControl, chartScale, ThisBrushDX, CurrentMousePrice);

				if (MovingOrder != null || MovingOrders.Count > 0)
				DrawMoveOrders(chartControl, chartScale, GrayBrushDX, CurrentMousePrice);
			
			}
			 catch (Exception ex)
			{
				if (TestRender) Print("OnRender Order Display: " + ThisMasterInstrument.Name + " " + ex.Message + " " + DateTime.Now.ToString("T"));
				
			}		
			
					
			
			
				
			try
			{
				
				if (BuyClickReady || SellClickReady)
				{
					if (BuyClickReady) ThisBrushDX = ClickUPColorDX;

					if (SellClickReady) ThisBrushDX = ClickDNColorDX;
					DrawClickEntry(chartControl, chartScale, ThisBrushDX, CurrentMousePrice);
				}
			
			}
			 catch (Exception ex)
			{
				if (TestRender) Print("OnRender Click Entry: " + ThisMasterInstrument.Name + " " + ex.Message + " " + DateTime.Now.ToString("T"));
				
			}				
		
			
			
			
			
			
			
			
			
			
			
			
			try
			{
			
				
				//ThisRect = new SharpDX.RectangleF(ChartPanel.X, ChartPanel.Y, ChartPanel.W, ChartPanel.H);
				//RenderTarget.DrawRectangle(ExpandRect(ThisRect,-1,-1,-1,-1), ThisStroke.BrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
				if (pActiveOutlineEnabled)
				{
					
					bool SomeBuy = BuyCloseReady || BuyClickReady || BuyStackReady || BuyWashoutReady;
					bool SomeSell = SellCloseReady || SellClickReady || SellStackReady || SellWashoutReady;
					
					if (SomeBuy && SomeSell)
					{
						ThisStroke = pOrderBothOutlineStroke;
						
					}
					else if (SomeBuy)
					{
						ThisStroke = pOrderUpOutlineStroke;					
		
					}
						
					else if (SomeSell)
					{						
						ThisStroke = pOrderDnOutlineStroke;

					}
						
					if (SomeBuy || SomeSell)
					{
						ThisRect = new SharpDX.RectangleF(ChartPanel.X, 0, ChartPanel.W, ThisStroke.Width);
						RenderTarget.FillRectangle(ThisRect,ThisStroke.BrushDX);
						
						ThisRect = new SharpDX.RectangleF(ChartPanel.X, ChartPanel.H-ThisStroke.Width, ChartPanel.W, ThisStroke.Width);
						RenderTarget.FillRectangle(ThisRect,ThisStroke.BrushDX);
						
						ThisRect = new SharpDX.RectangleF(0, 0, ThisStroke.Width, ChartPanel.H);
						RenderTarget.FillRectangle(ThisRect,ThisStroke.BrushDX);
						
						ThisRect = new SharpDX.RectangleF(ChartPanel.W-ThisStroke.Width, 0, ThisStroke.Width, ChartPanel.H);
						RenderTarget.FillRectangle(ThisRect,ThisStroke.BrushDX);
						
					}
					
				}
			
			}
			 catch (Exception ex)
			{
				if (TestRender) Print("OnRender Entry Highlight: " + ThisMasterInstrument.Name + " " + ex.Message + " " + DateTime.Now.ToString("T"));
				
			}		
			
					
			
								
			
			
			
			
			
			
			// Normal path disposal with null/IsDisposed checks
			SafeDispose(TextBrushDX1);
			SafeDispose(ChartTextBrushDX);
			SafeDispose(ChartBackgroundBrushDX);
			SafeDispose(ChartBackgroundFadeBrushDX);
			SafeDispose(ChartBackgroundMenuFadeBrushDX);
			SafeDispose(ChartBackgroundErrorBrushDX);
			SafeDispose(buttonBrushDX);
			SafeDispose(buttonHBrushDX);
			SafeDispose(buttonFHBrushDX);
			SafeDispose(buttonFOFFBrushDX);
			SafeDispose(buttonFONBrushDX);
			SafeDispose(MainFillBrushDX);
			SafeDispose(BidDotsBrushDX);
			SafeDispose(AskDotsBrushDX);
			SafeDispose(BidTriBrushDX);
			SafeDispose(AskTriBrushDX);
			SafeDispose(MagnetFillBrushDX);
			SafeDispose(UFAFillBrushDX);
			SafeDispose(POCFillBrushDX);
			SafeDispose(VWAPFillBrushDX);
			SafeDispose(MainPrintBrushDX);
			SafeDispose(LastPriceBrushDX);
			SafeDispose(CrossHairBrushDX);
			SafeDispose(TransparentBrushDX);
			SafeDispose(PrintLongBrushDX);
			SafeDispose(PrintShortBrushDX);
			SafeDispose(AboveLongBrushDX);
			SafeDispose(AboveShortBrushDX);
			SafeDispose(AboveNeutralBrushDX);
			SafeDispose(Plot1BrushDX);
			SafeDispose(Plot2BrushDX);
			SafeDispose(Plot3BrushDX);
			SafeDispose(Plot4BrushDX);
			SafeDispose(Plot5BrushDX);
			SafeDispose(Plot6BrushDX);
			SafeDispose(Plot7BrushDX);
			SafeDispose(Plot8BrushDX);
			SafeDispose(MainTextColorDX);
			SafeDispose(BidFillDX);
			SafeDispose(BidTextDX);
			SafeDispose(AskFillDX);
			SafeDispose(AskTextDX);
			SafeDispose(LVFillDX);
			SafeDispose(AxisBrushDX);
			SafeDispose(BackgroundColorDX);
			SafeDispose(TimerMainColorDX);
			SafeDispose(ClickUPColorDX);
			SafeDispose(ClickDNColorDX);
			SafeDispose(GreenBrushDX);
			SafeDispose(RedBrushDX);
			SafeDispose(GrayBrushDX);
			SafeDispose(BidHashDX);
			SafeDispose(AskHashDX);
			SafeDispose(TextShadowBrushDX);
			SafeDispose(DarkTextBrushDX);
			SafeDispose(BottomText);
			SafeDispose(CenterText);
			// cachedPanelBackdropBrushDX/cachedHeaderBgBrushDX/cachedHeaderBgHoverBrushDX/
			// cachedHeaderTextBrushDX/cachedHoverGlowBrushDX and the two text formats are
			// now kept alive across frames (change-detection cache). They are released in State.Terminated.


			}

			catch (Exception ex)
			{
				if (TestRender) Print("OnRender: " + ThisMasterInstrument.Name + " " + ex.Message + " " + DateTime.Now.ToString("T"));

				// Dispose class-level brushes on exception path
				SafeDispose(TextBrushDX1);
				SafeDispose(ChartTextBrushDX);
				SafeDispose(ChartBackgroundBrushDX);
				SafeDispose(ChartBackgroundFadeBrushDX);
				SafeDispose(ChartBackgroundMenuFadeBrushDX);
				SafeDispose(ChartBackgroundErrorBrushDX);
				SafeDispose(buttonBrushDX);
				SafeDispose(buttonHBrushDX);
				SafeDispose(buttonFHBrushDX);
				SafeDispose(buttonFOFFBrushDX);
				SafeDispose(buttonFONBrushDX);
				SafeDispose(MainFillBrushDX);
				SafeDispose(BidDotsBrushDX);
				SafeDispose(AskDotsBrushDX);
				SafeDispose(BidTriBrushDX);
				SafeDispose(AskTriBrushDX);
				SafeDispose(MagnetFillBrushDX);
				SafeDispose(UFAFillBrushDX);
				SafeDispose(POCFillBrushDX);
				SafeDispose(VWAPFillBrushDX);
				SafeDispose(MainPrintBrushDX);
				SafeDispose(LastPriceBrushDX);
				SafeDispose(CrossHairBrushDX);
				SafeDispose(TransparentBrushDX);
				SafeDispose(PrintLongBrushDX);
				SafeDispose(PrintShortBrushDX);
				SafeDispose(AboveLongBrushDX);
				SafeDispose(AboveShortBrushDX);
				SafeDispose(AboveNeutralBrushDX);
				SafeDispose(Plot1BrushDX);
				SafeDispose(Plot2BrushDX);
				SafeDispose(Plot3BrushDX);
				SafeDispose(Plot4BrushDX);
				SafeDispose(Plot5BrushDX);
				SafeDispose(Plot6BrushDX);
				SafeDispose(Plot7BrushDX);
				SafeDispose(Plot8BrushDX);
				// Panel-cache brushes/formats kept alive across frames; released in State.Terminated.
				// If an exception leaves them in a bad state, null them so next frame rebuilds.
				cachedPanelBackdropBrushDX = null;
				cachedHeaderBgBrushDX = null;
				cachedHeaderBgHoverBrushDX = null;
				cachedHeaderTextBrushDX = null;
				cachedHoverGlowBrushDX = null;
				cachedHeaderTextFormat = null;
				cachedButtonTextFormat = null;
				_panelCacheBgRef = null;
				_panelCacheTextRef = null;
				_btnFmtCacheFamily = null;
			}

			ApplyHoverCursor();
		}

		//


		private double GetMaxDouble(List<double> DDD)
		{
			
			double returnd = 0;
			
			foreach (double D in DDD)
				returnd = Math.Max(returnd,D);
			
			return returnd;
			
			
		}
		
		private SharpDX.RectangleF MoveRect (SharpDX.RectangleF RR, float xe, float ye)
		{
			
			SharpDX.RectangleF FF = new SharpDX.RectangleF(RR.X+xe, RR.Y+ye, RR.Width, RR.Height);
				
			return FF;
			
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
		
		private void DrawSwingLevels(ChartControl chartControl, ChartScale chartScale)
		{			
								
			if (pPivotLinesEnabled)
			{
				 
				int DS = 0;

				int latest = 0;
			
				try
				{
					if (pLevelsEnabled)
					{
						SortedDictionary<int, double> ThisLL2 = new SortedDictionary<int, double>(LowLA);
						foreach (KeyValuePair<int,double> L in ThisLL2)
						//foreach(KeyValuePair<int,double> L in LowLA)
						{
							latest = Math.Max(latest,L.Key);
						}	
						
						SortedDictionary<int, double> ThisLL3 = new SortedDictionary<int, double>(HighLA);
						foreach (KeyValuePair<int,double> L in ThisLL3)		
						//foreach(KeyValuePair<int,double> L in HighLA)
						{
							latest = Math.Max(latest,L.Key);
						}	
												
					}
					
				}
				catch (Exception ex)
				{
					if (TestRender) Print("OnRender DrawSwingLevels 1: " + ThisMasterInstrument.Name + " " + ex.Message + " " + DateTime.Now.ToString("T"));
					
				}	
				
				try
				{
				
					if (pLevelsEnabled) 
					{
						SortedDictionary<int, double> ThisListA1 = new SortedDictionary<int, double>(LowLA);
						SortedDictionary<int, double> ThisListA2 = new SortedDictionary<int, double>(HighLA);
						
						DrawOpenLevels(latest, ThisListA1, pColorSwingLow, chartControl, chartScale);
						DrawOpenLevels(latest, ThisListA2, pColorSwingHigh, chartControl, chartScale);
					}

				}
				catch (Exception ex)
				{
					if (TestRender) Print("OnRender DrawSwingLevels 2: " + ThisMasterInstrument.Name + " " + ex.Message + " " + DateTime.Now.ToString("T"));
					
				}
				
				try
				{
					if (pHistoricalLevelsEnabled)
					{
						SortedDictionary<int, int> ThisListB1 = new SortedDictionary<int, int>(LowLF);
						SortedDictionary<int, int> ThisListB2 = new SortedDictionary<int, int>(HighLF);
						
						DrawClosedLevels("Lows", ThisListB1, pColorSwingLow2, chartControl, chartScale);
						DrawClosedLevels("Highs", ThisListB2, pColorSwingHigh2, chartControl, chartScale);
					}
				
			
				}
				catch (Exception ex)
				{
					if (TestRender) Print("OnRender DrawSwingLevels 3: " + ThisMasterInstrument.Name + " " + ex.Message + " " + DateTime.Now.ToString("T"));
					
				}
				
			}
			
		}
		
		private void DrawOpenLevels(int latest, SortedDictionary<int, double> DICT, Stroke ThisStroke, ChartControl chartControl, ChartScale chartScale)
		{	
			foreach(KeyValuePair<int,double> L in DICT)
			{
				if (L.Key != latest)
				{					
							
					if (L.Key > LB) // hide entensions for bars past the last bar painted
						continue;	
							
					x1 = GetBoxXPixel(L.Key,"Right",true);
					x2 = 20000;
					
					y1 = chartScale.GetYByValue(L.Value);
					y2 = y1;

					StartPoint	= new Point(x1,y2);
					EndPoint = new Point(x2,y2);
					
					RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ThisStroke.BrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);					
				}
			}				
		}
		
		private void DrawClosedLevels(string hl, SortedDictionary<int, int> DICT, Stroke ThisStroke, ChartControl chartControl, ChartScale chartScale)
		{	
			
			
			
			foreach(KeyValuePair<int,int> L in DICT)
			{
				
				if (L.Key > LB) // hide entensions for bars past the last bar painted
					continue;	
				
				if (L.Value < FB) // hide entensions for bars past the last bar painted
					continue;	
				
				x1 = GetBoxXPixel(L.Key,"Right",true);
				x2 = GetBoxXPixel(L.Value,"Left",true);
				
				if (L.Value > LB)
					x2 = 20000;
				
				if (hl == "Highs")
					y1 = chartScale.GetYByValue(Highs[0].GetValueAt(L.Key));
				else
					y1 = chartScale.GetYByValue(Lows[0].GetValueAt(L.Key));
				
				y2 = y1;

				StartPoint	= new Point(x1,y2);
				EndPoint = new Point(x2,y2);
							
				RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ThisStroke.BrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);	
			}				
		}
		
//			SortedDictionary<int, double> HighLA = new SortedDictionary<int, double>();
//		SortedDictionary<int, double> LowLA = new SortedDictionary<int, double>();
		
//		SortedDictionary<int, double> DeleteLA = new SortedDictionary<int, double>();
		
//		SortedDictionary<int, int> HighLF = new SortedDictionary<int, int>();
//		SortedDictionary<int, int> LowLF 
			
			
		private void DrawExtensions(int i, List<Level> MAG, Stroke StrokeFresh, Stroke StrokeBroken, ChartControl chartControl, ChartScale chartScale, bool DrawFresh, bool DrawBroken)
		{		
												
			int CloseSS = 0; // hide extensions that immediately got invalidated = 1.  show all = 0.

			foreach (Level kvp2 in MAG)
			{

				int closedbar = kvp2.EndBar;
				
				bool ISCLOSED = closedbar != 0;
				
				if (ISCLOSED && !DrawBroken)
					continue;
				if (!ISCLOSED && !DrawFresh)
					continue;					
					
				ThisStroke = StrokeFresh;
				
				if (ISCLOSED)
				ThisStroke = StrokeBroken;
				
				// hide extensions that immediately got invalidated
				if (ISCLOSED && i + CloseSS >= closedbar)
				{
					//Print(i + "   " + closedbar);
					continue;
					
				}
											
				if (i > LB) // hide entensions for bars past the last bar painted
					continue; 
				if (closedbar != 0 && closedbar < FB) // hide entensions for bars past the last bar painted
					continue; 
				
				x1 = ChartControl.GetXByBarIndex(ChartBars, i) + barWidth;
				x1 = GetBoxXPixel(i,"Right",true);
				
				if (i < FB) // if we cant see the bar that formed the extension, then draw it completely to the left.
					x1 = 0;
				
				//x2 = ChartControl.GetXByBarIndex(ChartBars,LB) + 2000;
				x2 = 10000;
				
				if (ISCLOSED)
				if (closedbar <= LB) // prevent it from cutting off when the bar is off to the right side.
				x2 = GetBoxXPixel(closedbar,"Left",true);
				
				y1 = chartScale.GetYByValue(kvp2.Price);
				y2 = y1;	
				
				StartPoint	= new Point(x1,y2);
				EndPoint = new Point(x2,y2);
							
				//ThisStroke = pMagnetStroke;
				RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ThisStroke.BrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
			}
		}
		
		
		
		
		// SIGNALS
		
		private void DrawWashoutSignals(ChartControl chartControl, ChartScale chartScale)
		{

			if (pArrowsEnabled)
			{
			
				int count = 0;
				double ThisSignal2 = 0;
				
				for (int i = FB; i <= LB; i++)
				{
					ThisSignal2 = ShortSignals.GetValueAt(i);
					if (ThisSignal2 != 0)
						count++;
					
					ThisSignal2 = LongSignals.GetValueAt(i);
					if (ThisSignal2 != 0)
						count++;					
					
				}

				if (count != 0)
				{
				
					oldAntialiasMode = RenderTarget.AntialiasMode;
					
					longBrushDX = pArrowUpFBrush.ToDxBrush(RenderTarget);
					shortBrushDX = pArrowDownFBrush.ToDxBrush(RenderTarget);
					arrowBrushDX = pArrowUpFBrush.ToDxBrush(RenderTarget);
					//LabelBrushDX = TextBrushDX2;
					LabelBrushDX = pChartAxisBrush.ToDxBrush(RenderTarget);
					
					ThisStroke = pArrowDownStroke;

		           // int FB = ChartBars.FromIndex;
		           // int LB = ChartBars.ToIndex;
		           // int BB = 0;
		           // int xt = 0;
		            //int yt = 0;
		            double yt2 = 0;

		            LB = Math.Min(CurrentBars[0], LB);
		            BB = CurrentBars[0] - LB;

					// ARROWS

					TextFormat	LabelTextFormat			= pTextFont.ToDirectWriteTextFormat();	
				
					TextLayout LabelTextLayout = new TextLayout(NinjaTrader.Core.Globals.DirectWriteFactory, "", LabelTextFormat, 1000, LabelTextFormat.FontSize);
				
					LabelTextFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
					LabelTextFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
					LabelTextFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;
				
	            							
					Point TextPoint		= new Point(0, 0);

					
					ChartPanel chartPanel	= chartControl.ChartPanels[chartScale.PanelIndex];
					SharpDX.Direct2D1.PathGeometry arrowGeo;
					
					if (ChartBars != null)
					for (int i = FB; i <= LB; i++)
					{
								
						
						
						//BB = CurrentBars[0] - i;				
						BB = i;
						
						double ThisSignal = ShortSignals.GetValueAt(BB);

						if (ThisSignal != 0)
						{
								
							xt = chartControl.GetXByBarIndex(ChartBars, i) + barWidth/2;
							
							int pTextOffset = 0;
							string lb = string.Empty;
							float newy = 0;
							float newx = 0;
							float totalarrowheight = pArrowOffset + pArrowSize + pArrowBarHeight;
								
		
						
							
							if (ThisSignal >= 1)
							{			
								yt = chartScale.GetYByValue(Highs[0].GetValueAt(BB));
								yt2 = chartScale.GetYByValueWpf(Highs[0].GetValueAt(BB));
								
								yt = (int) GetBoxYPixel(Highs[0].GetValueAt(BB), "Top");
								yt2 = yt;
								
								arrowBrushDX = shortBrushDX;
								ThisStroke = pArrowDownStroke;	
								
							}	
							
							arrowGeo = CreateArrowGeometry(chartControl, chartPanel, chartScale, xt, yt, -1);

							RenderTarget.AntialiasMode = SharpDX.Direct2D1.AntialiasMode.PerPrimitive;

							RenderTarget.FillGeometry(arrowGeo, arrowBrushDX);
							RenderTarget.DrawGeometry(arrowGeo, ThisStroke.BrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
							arrowGeo.Dispose();

							if (LabelTextLayout != null && !LabelTextLayout.IsDisposed) LabelTextLayout.Dispose();
							LabelTextLayout = new TextLayout(NinjaTrader.Core.Globals.DirectWriteFactory, lb, LabelTextFormat, 1000, 1000);

							float boxpadding = LabelTextFormat.FontSize;
							
	           
							float RectWidth = LabelTextLayout.Metrics.Width + (float) pTextFont.Size;
							float RectHeight = LabelTextLayout.Metrics.Height  + (float) pTextFont.Size / 2f + 1;
							
							if (ThisSignal <= -1)
							{
								lb = pLabelBuy;
								newy = yt + totalarrowheight + 1 + pTextOffset;
								
							}
							
							if (ThisSignal >= 1)
							{
								lb = pLabelSell;
								newy = yt - totalarrowheight - RectHeight - 1 - pTextOffset;

							}	
							
							lb = pLabelSell + " (" + ThisSignal.ToString() + ")";
													
							newx = xt - RectWidth/2 - 4;								
														
							TextPoint = new Point(newx, newy);
						
							
							
							
							SharpDX.RectangleF TextRect = new SharpDX.RectangleF(newx, newy, RectWidth, RectHeight);

	//								{
							
	//									RenderTarget.AntialiasMode							= SharpDX.Direct2D1.AntialiasMode.Aliased;
	//									RenderTarget.DrawRectangle(rect2, pBrush08.BrushDX, pBrush08.Width, pBrush08.StrokeStyle);
	//									RenderTarget.AntialiasMode							= SharpDX.Direct2D1.AntialiasMode.PerPrimitive;
								
	//								}
							
							
							
							if (pLabelsEnabled)
								RenderTarget.DrawText(lb, LabelTextFormat, TextRect, LabelBrushDX);
							
							RenderTarget.AntialiasMode							= SharpDX.Direct2D1.AntialiasMode.Aliased;
									
						}	
						
						ThisSignal = LongSignals.GetValueAt(BB);

						if (ThisSignal != 0)
						{
								
							xt = chartControl.GetXByBarIndex(ChartBars, i) - barWidth/2;
							
							int pTextOffset = 0;
							string lb = string.Empty;
							float newy = 0;
							float newx = 0;
							float totalarrowheight = pArrowOffset + pArrowSize + pArrowBarHeight;
								
		
							
							if (ThisSignal >= 1)
							{

								yt = chartScale.GetYByValue(Lows[0].GetValueAt(BB));
								yt2 = chartScale.GetYByValueWpf(Lows[0].GetValueAt(BB));
								
								yt = (int) GetBoxYPixel(Lows[0].GetValueAt(BB), "Bottom");
								yt2 = yt;
								
								arrowBrushDX = longBrushDX;
								ThisStroke = pArrowUpStroke;	
								
							}
							
							if (ThisSignal <= -1)
							{			
								yt = chartScale.GetYByValue(Highs[0].GetValueAt(BB));
								yt2 = chartScale.GetYByValueWpf(Highs[0].GetValueAt(BB));
								
								yt = (int) GetBoxYPixel(Highs[0].GetValueAt(BB), "Top");
								yt2 = yt;
								
								
								arrowBrushDX = shortBrushDX;
								ThisStroke = pArrowDownStroke;	
								
							}	
							
							arrowGeo = CreateArrowGeometry(chartControl, chartPanel, chartScale, xt, yt, (int)ThisSignal);

							RenderTarget.AntialiasMode = SharpDX.Direct2D1.AntialiasMode.PerPrimitive;

							RenderTarget.FillGeometry(arrowGeo, arrowBrushDX);
							RenderTarget.DrawGeometry(arrowGeo, ThisStroke.BrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
							arrowGeo.Dispose();

							if (LabelTextLayout != null && !LabelTextLayout.IsDisposed) LabelTextLayout.Dispose();
							LabelTextLayout = new TextLayout(NinjaTrader.Core.Globals.DirectWriteFactory, lb, LabelTextFormat, 1000, 1000);

							float boxpadding = LabelTextFormat.FontSize;
							
	           
							float RectWidth = LabelTextLayout.Metrics.Width + (float) pTextFont.Size;
							float RectHeight = LabelTextLayout.Metrics.Height  + (float) pTextFont.Size / 2f + 1;
							
							if (ThisSignal >= 1)
							{
								lb = pLabelBuy;
								newy = yt + totalarrowheight + 1 + pTextOffset;
								
							}
							
							if (ThisSignal <= -1)
							{
								lb = pLabelSell;
								newy = yt - totalarrowheight - RectHeight - 1 - pTextOffset;

							}	
									
							lb = pLabelBuy + " (" + ThisSignal.ToString() + ")";
							
							newx = xt - RectWidth/2 - 4;								
														
							TextPoint = new Point(newx, newy);
						
							
							
							
							SharpDX.RectangleF TextRect = new SharpDX.RectangleF(newx, newy, RectWidth, RectHeight);

	//								{
							
	//									RenderTarget.AntialiasMode							= SharpDX.Direct2D1.AntialiasMode.Aliased;
	//									RenderTarget.DrawRectangle(rect2, pBrush08.BrushDX, pBrush08.Width, pBrush08.StrokeStyle);
	//									RenderTarget.AntialiasMode							= SharpDX.Direct2D1.AntialiasMode.PerPrimitive;
								
	//								}
							
							
							
							if (pLabelsEnabled)
								RenderTarget.DrawText(lb, LabelTextFormat, TextRect, LabelBrushDX);
							
									
						}	
						
						
					}			
					
					//arrowGeo.Dispose();
					
					longBrushDX.Dispose();
					shortBrushDX.Dispose();
					arrowBrushDX.Dispose();
					LabelBrushDX.Dispose();
					
					LabelTextFormat.Dispose();
					LabelTextLayout.Dispose();
					
					RenderTarget.AntialiasMode = oldAntialiasMode;
					
				}
				
			}
				
	}

			
			
			
			// END ARROWS
		
		
		
		
		
		
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
			
                    SharpDX.DirectWrite.TextFormat CellFormat = pTextFont2.ToDirectWriteTextFormat();	

                    //CellText = myProperties.LabelFont.ToDirectWriteTextFormat();

		
                    CellFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
                    CellFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
                    CellFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;
			
			
			
			
			string CellString = string.Empty;

			TextLayout CellLayout = new TextLayout(Core.Globals.DirectWriteFactory, "0", CellFormat, 10000, 10000);

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
				MaxVOL = Math.Max(MaxVOL, BarVolume.GetValueAt(i));
				MinVOL = Math.Min(MinVOL, BarVolume.GetValueAt(i));
				MaxVOLBid = Math.Max(MaxVOLBid, BarBid.GetValueAt(i));
				MinVOLBid = Math.Min(MinVOLBid, BarBid.GetValueAt(i));
				MaxVOLAsk = Math.Max(MaxVOLAsk, BarAsk.GetValueAt(i));
				MinVOLAsk = Math.Min(MinVOLAsk, BarAsk.GetValueAt(i));				
			}
			
			MaxBidAsk = Math.Max(MaxVOLBid,MaxVOLAsk);
			MinBidAsk = Math.Max(MinVOLBid,MinVOLAsk);
			
			for (int i = Start; i <= LB; i++)
			{
				x1 = chartControl.GetXByBarIndex(ChartBars,i);
				

				xL = x1 - barDistance/2;
				//xL2 = x1;
				xR = xL + barDistance + 1;
				xW = barDistance + 1;
				xW2 = barWidth;	
				
				xL = GetBoxXPixel(i,"Left",false) + 1;
				xR = GetBoxXPixel(i,"Right",false);
				xW = GetBoxXPixel(i,"Width",false) - 1;
				
				//if (LastPrice < BottomPrice || LowPrice > TopPrice)
				//	continue;

				// loop through price levels
				

				double dddd = 0;
				double fd = pNDThreshold2;
				
				if (ThisSeries.IsValidDataPointAt(i))
					dddd = ThisSeries.GetValueAt(i);
				
				double LastPrice = dddd;
					// tect
					
                    CellFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
                    CellFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
                    CellFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;
				
				CellString = LastPrice.ToString();	
//					float boxsize = pRSSize;
				
			
				
					
					
					y1 = chartScale.GetYByValue(LastPrice);

					
					yA = GetBoxYPixel(LastPrice,"Top");
					yB = GetBoxYPixel(LastPrice,"Bottom");
					yH = CellLayout.Metrics.Height + 4;
					yT = ChartPanel.Y + ChartPanel.H - yH*HUDNumber - 1*(HUDNumber-1);					
					
					HUDHeight = yH;
					
					StartPoint	= new Point(xL,y1);
					EndPoint = new Point(xR,y1);
					
					//ThisStroke = pBrush01;
					//RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ThisStroke.BrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
					

					
					oldAntialiasMode	= RenderTarget.AntialiasMode;
					RenderTarget.AntialiasMode							= SharpDX.Direct2D1.AntialiasMode.Aliased;
						
				
				
					ThisRect = new SharpDX.RectangleF((float)xL, (float)yT-1, (float)xW+1, (float)yH+1);
				
					ThisBrushDX = ChartBackgroundBrushDX;

					RenderTarget.FillRectangle(ThisRect, ThisBrushDX);
				
				
					// cell fill colors
					

					double ThisVol = 0;
					double OpacityLow2 = 0;
					double OpacityHigh2 = 0;
					double OpacityMultiplier2 = 0;
					double ThisOpacity2 = 0;
		
					//Print(ThisVol + "   " + MinVOL + "   " + MaxVOL);
				
					if (Label == "Total Volume" || Label == "Bar Volume")
					{
						ThisVol = BarVolume.GetValueAt(i);
						OpacityLow2 = pHUDMinOpacity;
						OpacityHigh2 = pHUDMaxOpacity;
						OpacityMultiplier2 = (OpacityHigh2 - OpacityLow2) / MaxVOL;
						ThisOpacity2 = Math.Round(OpacityLow2 + OpacityMultiplier2*(ThisVol-MinVOL),0);
						
						HUDVOLColorDX.Opacity = (float) ThisOpacity2/100f;
						HUDNEColorDX.Opacity = (float) ThisOpacity2/100f;
						HUDUPColorDX.Opacity = (float) ThisOpacity2/100f;
						HUDDNColorDX.Opacity = (float) ThisOpacity2/100f;
						
					}
					else if (Label == "Bar Bid")
					{
						ThisVol = BarBid.GetValueAt(i);
						OpacityLow2 = pHUDMinOpacity;
						OpacityHigh2 = pHUDMaxOpacity;
						OpacityMultiplier2 = (OpacityHigh2 - OpacityLow2) / MaxBidAsk;
						ThisOpacity2 = Math.Round(OpacityLow2 + OpacityMultiplier2*(ThisVol-MinBidAsk),0);
						
						HUDVOLColorDX.Opacity = (float) ThisOpacity2/100f;
						HUDNEColorDX.Opacity = (float) ThisOpacity2/100f;
						HUDUPColorDX.Opacity = (float) ThisOpacity2/100f;
						HUDDNColorDX.Opacity = (float) ThisOpacity2/100f;
						
					}
					else if (Label == "Bar Ask")
					{
						ThisVol = BarAsk.GetValueAt(i);
						OpacityLow2 = pHUDMinOpacity;
						OpacityHigh2 = pHUDMaxOpacity;
						OpacityMultiplier2 = (OpacityHigh2 - OpacityLow2) / MaxBidAsk;
						ThisOpacity2 = Math.Round(OpacityLow2 + OpacityMultiplier2*(ThisVol-MinBidAsk),0);
						
						HUDVOLColorDX.Opacity = (float) ThisOpacity2/100f;
						HUDNEColorDX.Opacity = (float) ThisOpacity2/100f;
						HUDUPColorDX.Opacity = (float) ThisOpacity2/100f;
						HUDDNColorDX.Opacity = (float) ThisOpacity2/100f;						

					}	
					else
					{
						HUDVOLColorDX.Opacity = pHUDDefaultOpacity/100f;
						HUDNEColorDX.Opacity = pHUDDefaultOpacity/100f;
						HUDUPColorDX.Opacity = pHUDDefaultOpacity/100f;
						HUDDNColorDX.Opacity = pHUDDefaultOpacity/100f;
						
					}
				
				

							
					
					if (Label == "Bar Ask")
					{
						
						ThisBrushDX = HUDUPColorDX;
						
					}
					else if (Label == "Bar Bid")
					{
						ThisBrushDX = HUDDNColorDX;
						
					}										
					else if (Label == "Total Volume")
					{

						ThisBrushDX = HUDVOLColorDX;
						
					}
					else if (Label == "Bar Delta")
					{
						
						
						
						if (pNetDMode2 == "Percent")
						{
							fd = pNDThreshold;
							dddd = BarDeltaPercent.GetValueAt(i);
							
							CellString = Math.Abs(Math.Round(dddd,1)).ToString() + "%";
							CellString = Math.Abs(Math.Round(dddd,1)).ToString() + "%"; 
							
							if (CellString.Contains("NaN"))
								CellString = "NA";
						}
						else
						{
							fd = pNDThreshold2;
							dddd = BarDelta.GetValueAt(i);
							
							CellString = dddd.ToString();
						}
							
						if (dddd >= fd)
						{
							ThisBrushDX = HUDUPColorDX;
						}
						else if (dddd <= -1*fd)
						{
							ThisBrushDX = HUDDNColorDX;
						}						
						else
						{
							ThisBrushDX = HUDNEColorDX;
						}						
						
						
					}
					else if (Label == "Total Delta")
					{
						
						
							
						if (dddd > 0)
						{
							ThisBrushDX = HUDUPColorDX;
						}
						else if (dddd < 0)
						{
							ThisBrushDX = HUDDNColorDX;
						}						
						else
						{
							ThisBrushDX = HUDNEColorDX;
						}
					}
					else if (Label == "Bar Volume")
					{
						// vary by strength
						ThisBrushDX = HUDVOLColorDX;
					}					
					else
					{
						// main color is set in brush at beginning
						ThisBrushDX = HUDVOLColorDX;
						
					}
							
			
				
				
				
				
						ThisRect = new SharpDX.RectangleF((float)xL, (float)yT, (float)xW, (float)yH);
						
						
					if (pUseBackgroundColor)
						RenderTarget.FillRectangle(ThisRect, BackgroundColorDX);
					
									
					RenderTarget.FillRectangle(ThisRect, ThisBrushDX);
				
				
					RenderTarget.AntialiasMode							= oldAntialiasMode;
						
//							//RenderTarget.DrawRectangle(ThisRect, ThisStroke.BrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
					
					//CellString = TotalVol.ToString();
					
					//if (i == LB)
					
					
					
						
					if (!pZoomEnabled)
					{
					
						oldAntialiasMode	= RenderTarget.AntialiasMode;
						RenderTarget.AntialiasMode							= SharpDX.Direct2D1.AntialiasMode.PerPrimitive;
							
						ThisBrushDX = TextBrushDX2;
		
						RenderTarget.DrawText(CellString, CellFormat, ThisRect, ThisBrushDX);	

						RenderTarget.AntialiasMode = oldAntialiasMode;
						
					}
				
				

					
	
								
				// show current bar or bar by bar values for testing
				

				
			}
			
			
			ThisRect = new SharpDX.RectangleF((float)xR, (float)yT-1, (float)xW+5000, (float)yH+1);
			RenderTarget.FillRectangle(ThisRect, ChartBackgroundFadeBrushDX);
			
			ThisRect = new SharpDX.RectangleF((float)xR+5, (float)yT, (float)xW, (float)yH);

			CellFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Leading;
            CellFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
            CellFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;		
		
			CellString = Label.ToString();

			if (CellLayout != null) CellLayout.Dispose();
			CellLayout = new TextLayout(Core.Globals.DirectWriteFactory, CellString, CellFormat,  10000, 10000);
			MinRightMarginHUD = Math.Max (MinRightMarginHUD, CellLayout.Metrics.Width + 10);
			
			oldAntialiasMode	= RenderTarget.AntialiasMode;
			RenderTarget.AntialiasMode							= SharpDX.Direct2D1.AntialiasMode.PerPrimitive;
				

			
			ThisBrushDX = AxisBrushDX;
			RenderTarget.DrawText(CellString, CellFormat, ThisRect, ThisBrushDX);	

			RenderTarget.AntialiasMode = oldAntialiasMode;

			CellFormat.Dispose();
			CellLayout.Dispose();
			
	}
		
		
		
		private void DrawLinePlot(Series<double> ThisSeries, SharpDX.Direct2D1.Brush Plot1BrushDX, ChartControl chartControl, ChartScale chartScale, string Label, bool IsTextOnly)
		{
			
			int Start = Math.Max(FB-1,0);
			
			for (int i = Start; i <= LB; i++)
			{
				x1 = chartControl.GetXByBarIndex(ChartBars,i);
				
				double xL = GetBoxXPixel(i,"Left",false);
				//double xL2 = x1;
				double xR = GetBoxXPixel(i,"Right",false);
				double xW = GetBoxXPixel(i,"Width",false);
				
				if (i == LB)
				{
					xW = xW *2;
				
					if (pExtendCompRight)
						xW = 2000;
				}
				
				
				
				//double xW2 = barWidth;
							
//				xL = x1 - barDistance/2;
//				xL2 = x1;
//				xR = xL + barDistance + 1;
//				xW = barDistance + 1;
//				xW2 = barWidth;	
				
				
				double LastPrice = RTTS(Highs[0].GetValueAt(i));
				double LowPrice = Lows[0].GetValueAt(i);
				
				
				
				


				// loop through price levels
				

				double dddd = 0;
				
				if (ThisSeries.IsValidDataPointAt(i))
					dddd = ThisSeries.GetValueAt(i);
				
				LastPrice = dddd;
				
//				if (LastPrice < BottomPrice || LowPrice > TopPrice)
//					continue;
				if (LastPrice < CurrentLowRender || LastPrice > CurrentHighRender)
					continue;				
				
				
					SharpDX.DirectWrite.TextFormat CellFormat = FinalFont1.ToDirectWriteTextFormat();	
				
                    //CellText = myProperties.LabelFont.ToDirectWriteTextFormat();

				
                    CellFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Leading;
                    CellFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
                    CellFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;
			
					string CellString = Label;
					
//					float boxsize = pRSSize;
				
			
				
					
					
					y1 = chartScale.GetYByValue(LastPrice);

					double yA = GetBoxYPixel(LastPrice,"Top");
					double yB = GetBoxYPixel(LastPrice,"Bottom");
					double yH = GetBoxYPixel(LastPrice,"Height");
					double yT = yA;					
					
					//Print(yH);
					
					
					StartPoint	= new Point(xL,y1);
					EndPoint = new Point(xR,y1);
					
					//ThisStroke = pBrush01;
					//RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ThisStroke.BrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
					

					
					//if (pPrintEnabled)
					{
						
						// cell fill colors
						
						// main color is set in brush at beginning

						ThisRect = new SharpDX.RectangleF((float)xL, (float)yT, (float)xW, (float)yH);
						
						
					
						ThisBrushDX = ChartBackgroundBrushDX;
						if (pUseBackgroundColor)
							ThisBrushDX = BackgroundColorDX;						
			
						
						if (!IsTextOnly) RenderTarget.FillRectangle(ExpandRect(ThisRect,0,0), ThisBrushDX);
						
						
//						StartPoint	= new Point(x1, yT);
//						EndPoint = new Point(x1, yB);
						
//						ThisStroke = myProperties.GridLineVPen;
//						RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ThisStroke.BrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
							
						
						
						
						ThisBrushDX = Plot1BrushDX;
						if (!IsTextOnly) RenderTarget.FillRectangle(ThisRect, ThisBrushDX);
					
//							//RenderTarget.DrawRectangle(ThisRect, ThisStroke.BrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
						
						//CellString = TotalVol.ToString();
						
						ThisRect = new SharpDX.RectangleF((float)xR+6, (float)yT, (float)xW, (float)yH);
						
						if (i == LB && IsTextOnly && pCompLabelsEnabled)
						{
							
							if (CellString == "POC")								
							{
								if (RTTS(ThisSeries.GetValueAt(i)) == RTTS(CompVWAP.GetValueAt(i)))
								{
									CellString = "POC / VWAP";							
								}
							}
							
							if (CellString == "VWAP")								
							{
								if (RTTS(ThisSeries.GetValueAt(i)) == RTTS(CompPOC.GetValueAt(i)))
								{
									CellString = "POC / VWAP";							
								}
							}								
						
							
							
							oldAntialiasMode	= RenderTarget.AntialiasMode;
							RenderTarget.AntialiasMode							= SharpDX.Direct2D1.AntialiasMode.PerPrimitive;
								
							
							
							ThisBrushDX = TextBrushDX2;
							
							
							
							
							RenderTarget.DrawText(CellString, CellFormat, ThisRect, ThisBrushDX);	

							RenderTarget.AntialiasMode = oldAntialiasMode;
						}

					
					
						
						
					}
					//else
					{
						
						
					}
					
				
				CellFormat.Dispose();
				
			}
	}
		
		
		private void DrawCompositeLevels(ChartControl chartControl, ChartScale chartScale, bool IsTextOnly)
		{	
			if (pCompAllLevelsEnabled)
			{
				if (pCompVA1Enabled) DrawLinePlot(CompVAH1, Plot3BrushDX, chartControl, chartScale, "VAH " + "(" + pVA1Percent.ToString() + "%)", IsTextOnly);
				if (pCompVA1Enabled) DrawLinePlot(CompVAL1, Plot4BrushDX, chartControl, chartScale, "VAL " + "(" + pVA1Percent.ToString() + "%)", IsTextOnly);
				if (pCompVA2Enabled) DrawLinePlot(CompVAH2, Plot5BrushDX, chartControl, chartScale, "VAH " + "(" + pVA2Percent.ToString() + "%)", IsTextOnly);
				if (pCompVA2Enabled) DrawLinePlot(CompVAL2, Plot6BrushDX, chartControl, chartScale, "VAL " + "(" + pVA2Percent.ToString() + "%)", IsTextOnly);

				if (pCompVWAPEnabled) DrawLinePlot(CompVWAP, Plot2BrushDX, chartControl, chartScale, "VWAP", IsTextOnly);
				if (pCompPOCEnabled) DrawLinePlot(CompPOC, Plot1BrushDX, chartControl, chartScale, "POC", IsTextOnly);
				
				if (pCompDHLEnabled) DrawLinePlot(CompDH, Plot8BrushDX, chartControl, chartScale, "HIGH", IsTextOnly);
				if (pCompDHLEnabled) DrawLinePlot(CompDL, Plot7BrushDX, chartControl, chartScale, "LOW", IsTextOnly);				
				
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
			
//		    int nThreshold = 150;
//		    int bgDelta = Convert.ToInt32((bg.R * 0.299) + (bg.G * 0.587) + 
//		                                  (bg.B * 0.114));

//		    Brush foreColor = (255 - bgDelta < nThreshold) ? Brushes.Black : Brushes.White;
//		    return foreColor;
		}

		
		
		
		private void DrawBlockTrades(ChartControl chartControl, ChartScale chartScale)
		{		
		
		if (pShowBlocks)
			{
				
				BlockTradeButtons.Clear();
				
					
				for (int i = PrintFB; i <= LB; i++)
				{
					x1 = chartControl.GetXByBarIndex(ChartBars,i);
					
					double xL = x1 - barWidth - 1;
					//double xL2 = x1;
					double xR = x1 + barWidth;
					double xW = barWidth + barWidth + 1;
					double xW2 = barWidth;
					
					xL = GetBoxXPixel(i,"Left",true);
	//				//xL2 = x1;
					xR = GetBoxXPixel(i,"Right",true);
					xW = GetBoxXPixel(i,"Width",true);
					xW2 = barWidth + 1;
					
					double LastPrice = Highs[0].GetValueAt(i);
					double LowPrice = Lows[0].GetValueAt(i);
					
					if (LastPrice < BottomPrice || LowPrice > TopPrice)
						continue;

					// loop through price levels
					

					BarItem BBB = null;
					
					if (BarItems.IsValidDataPointAt(i))
						BBB = BarItems.GetValueAt(i);
					
					
	   					//CellFormat = new SharpDX.DirectWrite.TextFormat(Core.Globals.DirectWriteFactory, "Arial", SharpDX.DirectWrite.FontWeight.Normal,
	                   // SharpDX.DirectWrite.FontStyle.Normal, SharpDX.DirectWrite.FontStretch.Normal, 11.0F);

						CellFormat			= FinalFont1.ToDirectWriteTextFormat();	
	                    //CellText = myProperties.LabelFont.ToDirectWriteTextFormat();

	                    CellFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
	                    CellFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
	                    CellFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;			
						
					
					
					
//					LastPrice = RTTS(Highs[0].GetValueAt(i));
//					LowPrice = RTTS(Lows[0].GetValueAt(i));
					
//					if (LastPrice < BottomPrice || LowPrice > TopPrice)
//						continue;
					
					
					
					if (BBB != null)	
					do
					{
						//BarItem BBB = BarItems.GetValueAt(i);
						
						double BidVol = 0;
						double AskVol = 0;
						double TotalVol = 0;
						double ThisMaxVol = 0;
						
						bool IsBidImb = false;
						bool IsAskImb = false;
						

						
						bool IsMagnet = false;
						bool IsUFA = false;
						bool IsPOC = false;
						bool IsVWAP = false;
						
						double BidBlockTotal = 0;
						double AskBlockTotal = 0;
				
						if (BBB.l.ContainsKey(LastPrice))
						{

							if (pShowBlocks)
							{
								List<double> _bidBlockList, _askBlockList;
								if (BBB.BidBlocks != null && BBB.BidBlocks.TryGetValue(LastPrice, out _bidBlockList))
									BidBlockTotal = GetMaxDouble(_bidBlockList);

								if (BBB.AskBlocks != null && BBB.AskBlocks.TryGetValue(LastPrice, out _askBlockList))
									AskBlockTotal = GetMaxDouble(_askBlockList);
							}
					
							Level LLL = new Level();
					
					
					
						
				
						}
				
					
					
						
						y1 = chartScale.GetYByValue(LastPrice);

						double yA = GetBoxYPixel(LastPrice,"Top");
						double yB = GetBoxYPixel(LastPrice,"Bottom");
						double yH = GetBoxYPixel(LastPrice,"Height");
						double yT = yA;					
						
						//if (rowheight == 0)
						PriceRowHeight = Math.Min(PriceRowHeight,yH);
						PriceRowHeight = PriceRowHeight + 1;
						
						StartPoint	= new Point(xL,y1);
						EndPoint = new Point(xR,y1);
						
						//ThisStroke = pBrush01;
						//RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ThisStroke.BrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
						

						float adjustleftright = 13; // how far to show pop up from the center of the bar
						
						
					
						//if (pShowBlocks)
						{
							
							oldAntialiasMode = RenderTarget.AntialiasMode;
							RenderTarget.AntialiasMode							= SharpDX.Direct2D1.AntialiasMode.PerPrimitive;
												
							ChartPanel chartPanel	= chartControl.ChartPanels[chartScale.PanelIndex];
								
							if (BidBlockTotal > 0)
							{
							
								CellString = BidBlockTotal.ToString();
								if (CellLayout != null && !CellLayout.IsDisposed) CellLayout.Dispose();
								CellLayout = new TextLayout(Core.Globals.DirectWriteFactory, CellString, CellFormat,  10000, 10000);

								float xwidid = CellLayout.Metrics.Width + 2;
								CellLayout.Dispose();

								ThisRect = new SharpDX.RectangleF((float)x1-10-1, (float)yA, 10, (float)yH);
								BlockTradeButtons.Add(ThisRect);

								if (pShowBlocksHover && MouseIn(ThisRect,0,0))
								{
									double yy22 = y1;

									ThisRect = new SharpDX.RectangleF((float)x1-adjustleftright-xwidid, (float)y1 , xwidid, (float)BBB.BidBlocks[LastPrice].Count*(float)PriceRowHeight);

									ThisBrushDX = ChartBackgroundBrushDX;
									if (pUseBackgroundColor)
										ThisBrushDX = BackgroundColorDX;

									RenderTarget.FillRectangle(ExpandRect(ThisRect,3,3,3,3), ThisBrushDX);

									foreach (double d in BBB.BidBlocks[LastPrice])
									{
										ThisRect = new SharpDX.RectangleF((float)x1-adjustleftright-xwidid, (float)yy22, xwidid, (float)PriceRowHeight);
										CellString = d.ToString();
										ThisBrushDX = TextBrushDX2;
										RenderTarget.DrawText(CellString, CellFormat, ThisRect, ThisBrushDX);
										yy22 = yy22 + PriceRowHeight;

									}

									ThisGeometry = CreateTriangleGeometry(chartControl, chartPanel, chartScale, (int)x1+1, y1, -1, pTriSize+2);

									ThisBrushDX = buttonHBrushDX;
									RenderTarget.FillGeometry(ThisGeometry, ThisBrushDX);
									ThisGeometry.Dispose();

								}

								ThisGeometry = CreateTriangleGeometry(chartControl, chartPanel, chartScale, (int)x1, y1, -1, pTriSize);

								ThisBrushDX = BidTriBrushDX;
								RenderTarget.FillGeometry(ThisGeometry, ThisBrushDX);
								ThisGeometry.Dispose();
								
								


							}						
							
							if (AskBlockTotal > 0)
							{
							
								CellString = AskBlockTotal.ToString();
								if (CellLayout != null && !CellLayout.IsDisposed) CellLayout.Dispose();
								CellLayout = new TextLayout(Core.Globals.DirectWriteFactory, CellString, CellFormat,  10000, 10000);

								float xwidid = CellLayout.Metrics.Width + 2;
								CellLayout.Dispose();

								ThisRect = new SharpDX.RectangleF((float)x1, (float)yA, 10, (float)yH);
								BlockTradeButtons.Add(ThisRect);

								if (pShowBlocksHover && MouseIn(ThisRect,0,0))
								{
									double yy22 = y1;

									ThisRect = new SharpDX.RectangleF((float)x1+adjustleftright-2, (float)y1 , xwidid, (float)BBB.AskBlocks[LastPrice].Count*(float)PriceRowHeight);

									ThisBrushDX = ChartBackgroundBrushDX;
									if (pUseBackgroundColor)
										ThisBrushDX = BackgroundColorDX;

									RenderTarget.FillRectangle(ExpandRect(ThisRect,3,3,3,3), ThisBrushDX);

									foreach (double d in BBB.AskBlocks[LastPrice])
									{
										ThisRect = new SharpDX.RectangleF((float)x1+adjustleftright-2, (float)yy22, xwidid, (float)PriceRowHeight);
										CellString = d.ToString();
										ThisBrushDX = TextBrushDX2;
										RenderTarget.DrawText(CellString, CellFormat, ThisRect, ThisBrushDX);
										yy22 = yy22 + PriceRowHeight;

									}

									ThisGeometry = CreateTriangleGeometry(chartControl, chartPanel, chartScale, (int)x1-1, y1, 1, pTriSize+2);

									ThisBrushDX = buttonHBrushDX;
									RenderTarget.FillGeometry(ThisGeometry, ThisBrushDX);
									ThisGeometry.Dispose();

								}

								ThisGeometry = CreateTriangleGeometry(chartControl, chartPanel, chartScale, (int)x1, y1, 1, pTriSize);

								ThisBrushDX = AskTriBrushDX;
								RenderTarget.FillGeometry(ThisGeometry, ThisBrushDX);
								ThisGeometry.Dispose();
								
								
							}
								
							RenderTarget.AntialiasMode = oldAntialiasMode;
								
							
						}
							
							
						
						
						
						LastPrice = RTTS(LastPrice - ThisTickSizze);
					} 
					while (LastPrice >= LowPrice);	
					
					
					
					
				}
				
				
			}			
			
			
	}
	
	
		private void DrawImbalanceDots(ChartControl chartControl, ChartScale chartScale)
		{
				
			if (pShowImbalance)	
			{
				for (int i = PrintFB; i <= LB; i++)
				{
					x1 = chartControl.GetXByBarIndex(ChartBars,i);
					
					
					double LastPrice = RTTS(Highs[0].GetValueAt(i));
					double LowPrice = Lows[0].GetValueAt(i);
					
					if (LastPrice < BottomPrice || LowPrice > TopPrice)
						continue;

					// loop through price levels
					

					BarItem BBB = null;
					
					if (BarItems.IsValidDataPointAt(i))
						BBB = BarItems.GetValueAt(i);
					
					
					float boxsize = pRSSize;
					
					double xL = GetBoxXPixel(i,"Left",true);
					double xR = GetBoxXPixel(i,"Right",true)-1;
					
				
					if (BBB != null)	
					do
					{
						//BarItem BBB = BarItems.GetValueAt(i);
						
						double BidVol = 0;
						double AskVol = 0;
						double TotalVol = 0;
						
						bool IsBidImb = false;
						bool IsAskImb = false;
						
					
						
						
						if (BBB.l.ContainsKey(LastPrice))
						{
							BidVol = BBB.l[LastPrice].bv;
							AskVol = BBB.l[LastPrice].av;
							TotalVol = BBB.l[LastPrice].tv;	
							
							if (pShowImbalance)
							{
								if (BBB.BidI != null)
									IsBidImb = BBB.BidI.Contains(LastPrice);
								
								if (BBB.AskI != null)
									IsAskImb = BBB.AskI.Contains(LastPrice);
							}
								
							
							
							
						}
						
					
					
						
						y1 = chartScale.GetYByValue(LastPrice);

						
						StartPoint	= new Point(xL,y1);
						EndPoint = new Point(xR,y1);
						
						
						
						
					
							
						// IMBALANCE RECTANGLES
						
						if (ShowImbalanceHash)
						{
							
							y1 = GetBoxYPixel(LastPrice, "Top");
							y2 = GetBoxYPixel(LastPrice,"Height");
									
							int pImbRectSize = pRSSize2;
								
								ShowImbalanceHash = true;
	      
			

							
							if (IsBidImb)
							{
							
								ThisRect = new SharpDX.RectangleF((float)xL, (float)y1, (float)pImbRectSize, (float)y2);	
								
								ThisBrushDX = BidHashDX;
								RenderTarget.FillRectangle(ThisRect, ThisBrushDX);							
								
								if (!isBidAskMode)
								{
									ThisRect = new SharpDX.RectangleF((float)xR-pImbRectSize, (float)y1, (float)pImbRectSize, (float)y2);	
									
									ThisBrushDX = BidHashDX;
		                            RenderTarget.FillRectangle(ThisRect, ThisBrushDX);										
									
								}
							
								
							}						
							
							if (IsAskImb)
							{
							
								ThisRect = new SharpDX.RectangleF((float)xR-pImbRectSize, (float)y1, (float)pImbRectSize, (float)y2);	
								
								ThisBrushDX = AskHashDX;
	                            RenderTarget.FillRectangle(ThisRect, ThisBrushDX);	
								
								if (!isBidAskMode)
								{
									ThisRect = new SharpDX.RectangleF((float)xL, (float)y1, (float)pImbRectSize, (float)y2);	
									
									ThisBrushDX = AskHashDX;
		                            RenderTarget.FillRectangle(ThisRect, ThisBrushDX);										
									
								}
							
							}
						}

//						StartPoint	= new Point(xL,y1);
//						EndPoint = new Point(xR,y2);
						
	// IMBALANCE DOTS
						
						if (pShowImbalanceA)
						{
						
							oldAntialiasMode = RenderTarget.AntialiasMode;
							RenderTarget.AntialiasMode							= SharpDX.Direct2D1.AntialiasMode.PerPrimitive;
						
							if (IsBidImb)
							{
							
								ThisEllipse = new SharpDX.Direct2D1.Ellipse(StartPoint.ToVector2(), boxsize, boxsize);
								
								ThisBrushDX = BidDotsBrushDX;
	                            RenderTarget.FillEllipse(ThisEllipse, ThisBrushDX);
	                            //RenderTarget.DrawEllipse(ThisEllipse, MainFillBrushDX);
								
							}						
							
							if (IsAskImb)
							{
							
								ThisEllipse = new SharpDX.Direct2D1.Ellipse(EndPoint.ToVector2(), boxsize, boxsize);
								
								ThisBrushDX = AskDotsBrushDX;
	                            RenderTarget.FillEllipse(ThisEllipse, ThisBrushDX);
	                            //RenderTarget.DrawEllipse(ThisEllipse, MainFillBrushDX);
								
							}
								
							RenderTarget.AntialiasMode = oldAntialiasMode;
							
						}
							
						
						LastPrice = RTTS(LastPrice - ThisTickSizze);
					} 
					while (LastPrice >= LowPrice);	
					
					
					
					
				}
			}
			
			
		}
			
		private void DrawZones(ChartControl chartControl, ChartScale chartScale)
		{
					//            	SharpDX.Direct2D1.Brush

					
			    ResistanceBrush1DX = pResistanceZColor1.ToDxBrush(RenderTarget);
				ResistanceBrush1DX.Opacity = pResistanceZOpacity1/100f;

            	ResistanceBrush2DX = pResistanceZColor2.ToDxBrush(RenderTarget);
				ResistanceBrush2DX.Opacity = pResistanceZOpacity2/100f;

            	ResistanceBrush3DX = pResistanceZColor3.ToDxBrush(RenderTarget);
				ResistanceBrush3DX.Opacity = pResistanceZOpacity3/100f;				

            	SupportBrush1DX = pSupportZColor1.ToDxBrush(RenderTarget);
				SupportBrush1DX.Opacity = pSupportZOpacity1/100f;

            	SupportBrush2DX = pSupportZColor2.ToDxBrush(RenderTarget);
				SupportBrush2DX.Opacity = pSupportZOpacity2/100f;

            	SupportBrush3DX = pSupportZColor3.ToDxBrush(RenderTarget);
				SupportBrush3DX.Opacity = pSupportZOpacity3/100f;	

            	ZoneButtonBrushDX = Brushes.White.ToDxBrush(RenderTarget);
				ZoneButtonBrushDX.Opacity = 20/100f;

					
					
									
												//foreach (ZoneItem zzz in ZoneItems)	
			
//				bool doitn = false;
			
//			if (doitn)
				for (int i = 1; i <= CurrentBars[0]; i++)
				{
					
					if (!ZoneItems.IsValidDataPointAt(i))
						continue;
					
					ZoneItem zzz = ZoneItems.GetValueAt(i);
					
					
					int jjj = i; //DZ.Key
					
//					if (jjj == CurrentBars[0])
//						continue;
					
					if (jjj >= LB+1)
						continue;
					
				
					
					XE = 0;
					
					if (!pOutlineZones)
						XE = 1;
					
					x1 = chartControl.GetXByBarIndex(ChartBars, jjj) + barWidth;
					

						
					if (pZonesEnabled)
					{
						List<Zone> Zones1R = new List<Zone>(zzz.ResistanceZones);
						List<Zone> Zones1S = new List<Zone>(zzz.SupportZones);

						DrawResistanceZones(Zones1R, jjj, chartControl, chartScale);
						DrawSupportZones(Zones1S, jjj, chartControl, chartScale);
					}

					if (pZonesEnabled2)
					{
						List<Zone> Zones2R = new List<Zone>(zzz.ResistanceZones2);
						List<Zone> Zones2S = new List<Zone>(zzz.SupportZones2);

						DrawResistanceZones(Zones2R, jjj, chartControl, chartScale);
						DrawSupportZones(Zones2S, jjj, chartControl, chartScale);
					}
					
					
				}
				
				
						
						
					
					
									
						
					ResistanceBrush1DX.Dispose();
					ResistanceBrush2DX.Dispose();
					ResistanceBrush3DX.Dispose();
					SupportBrush1DX.Dispose();
					SupportBrush2DX.Dispose();
					SupportBrush3DX.Dispose();
					
					ZoneButtonBrushDX.Dispose();
					
		}
		
		
		private void DrawMarketDepth (ChartControl chartControl, ChartScale chartScale)
		{		
			
			if (pDepthEnabled) // && IsHardRightEdge)
			{
				if (RightMoveX != 0)
					RightMoveX = RightMoveX + pSpaceBetweenColumns; // space between two histograms
				
				
				// vertical line
				
				x1 = ChartPanel.X + ChartPanel.W - RightMoveX + 0;
				StartPoint	= new Point(x1+1, 0);
				EndPoint = new Point(x1+1, 5000);
				
				ThisStroke = myProperties.GridLineVPen;
				//ThisStroke = myProperties.AxisPen;
				RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ThisStroke.BrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
					
 
				
				
				SharpDX.Direct2D1.Brush AskHistColor1DX = pAskHistColor1.ToDxBrush(RenderTarget);
				SharpDX.Direct2D1.Brush AskHistColor2DX = pAskHistColor2.ToDxBrush(RenderTarget);	
				SharpDX.Direct2D1.Brush AskOutColor1DX = pAskOutColor1.ToDxBrush(RenderTarget);
				AskHistColor1DX.Opacity = pAskOpacity/100f;
				AskHistColor2DX.Opacity = pAskOpacity2/100f;
				
				SharpDX.Direct2D1.Brush BidHistColor1DX = pBidHistColor1.ToDxBrush(RenderTarget);
				SharpDX.Direct2D1.Brush BidHistColor2DX = pBidHistColor2.ToDxBrush(RenderTarget);	
				SharpDX.Direct2D1.Brush BidOutColor1DX = pBidOutColor1.ToDxBrush(RenderTarget);
				BidHistColor1DX.Opacity = pBidOpacity/100f;
				BidHistColor2DX.Opacity = pBidOpacity2/100f;
				
			
				//TextBrushDX2 = MainTextColorDX;
				SharpDX.Direct2D1.Brush TextColorMDDX = pMainTextColor.ToDxBrush(RenderTarget); // same as TextBrushDX2
				
				//pChartAxisBrush
				

				
				long maxtv = 0;
			
				SharpDX.DirectWrite.TextFormat CellFormat			= FinalFont1.ToDirectWriteTextFormat();	

                CellFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Trailing;
                CellFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
                CellFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;
		
				string CellString = string.Empty;
			
				TextLayout CellTextLayout = null;

				long largestAskVol = 0;
				long largestBidVol = 0;
				double totAVol = 0;
				double totBVol = 0;
				double lastBPrice = 999999999;
				double lastAPrice = 0;
				//ThisMarketDepth.Asks[i].Price, marketDepth.Asks[i].Volume)

				double HighestAllowed = RTTS(CurrentAsk + (pMaxLevels-1)*ThisTickSizze);
				double LowestAllowed = RTTS(CurrentBid - (pMaxLevels-1)*ThisTickSizze);
				
				List<LadderRow> AskLadder = new List<LadderRow>(askRows);				
				foreach (LadderRow MDR in AskLadder)
				//foreach (LadderRow MDR in askRows)	
				//foreach (MarketDepthRow MDR in ThisMarketDepth.Asks)
				//foreach (KeyValuePair<double, long> DepthLevels in AskLevels)
				{
					//double ThisVolume = DepthLevels.Value;
					long ThisVolume = MDR.Volume;
					double ThisPrice = MDR.Price;
					
					if (ThisPrice > HighestAllowed)
						continue;					
					
					maxtv = Math.Max(maxtv, ThisVolume);
					largestAskVol = Math.Max(largestAskVol, ThisVolume);
					totAVol = totAVol + ThisVolume;
					
					lastAPrice = Math.Max(lastAPrice, ThisPrice);
				}
				
				
				List<LadderRow> BidLadder = new List<LadderRow>(bidRows);
				foreach (LadderRow MDR in BidLadder)					
				//foreach (LadderRow MDR in bidRows)
				//foreach (MarketDepthRow MDR in ThisMarketDepth.Bids)
				//foreach (KeyValuePair<double, long> DepthLevels in BidLevels)
				{
					long ThisVolume = MDR.Volume;
					double ThisPrice = MDR.Price;
					
					if (ThisPrice < LowestAllowed)
						continue;
					
					maxtv = Math.Max(maxtv, ThisVolume);
					largestBidVol = Math.Max(largestBidVol, ThisVolume);
					totBVol = totBVol + ThisVolume;
					
					lastBPrice = Math.Min(lastBPrice, ThisPrice);
				}
				
				lastAPrice = RTTS(lastAPrice + 2*ThisTickSizze);
				lastBPrice = RTTS(lastBPrice - 2*ThisTickSizze);
				
				double totVol = totAVol + totBVol;
				double pAVol = Math.Round(totAVol / totVol * 100, 1);
				double pBVol = Math.Round(totBVol / totVol * 100, 1);	
				
				string sBid = string.Empty;
				string sAsk = string.Empty;
				
				if (pDisplayTotal)
				{								
					if (pDepthTotalMode == "Contracts")
						sAsk = totAVol.ToString();
					else if (pDepthTotalMode == "Percent")
						sAsk = pAVol.ToString("n1") + "(%)";
					else
						sAsk = totAVol.ToString() + " (" + pAVol.ToString("n1") + "%)";
				
				}	
				
				if (pDisplayTotal)
				{	
					if (pDepthTotalMode == "Contracts")
						sBid = totBVol.ToString();							
					else if (pDepthTotalMode == "Percent")
						sBid = pBVol.ToString("n1") + "(%)";
					else
						sBid = totBVol.ToString() + " (" + pBVol.ToString("n1") + "%)";	
				}	
				
				
				foreach (LadderRow MDR in AskLadder)
				//foreach (LadderRow MDR in askRows)
				//foreach (MarketDepthRow MDR in ThisMarketDepth.Asks)		
				//foreach (KeyValuePair<double, long> DepthLevels in AskLevels)
				{
										
					//double ThisPrice = DepthLevels.Key;
					//double TotalVol = DepthLevels.Value;
					
					double ThisPrice = MDR.Price;
					double TotalVol = MDR.Volume;
					
					if (ThisPrice > HighestAllowed)
						continue;
					
					if (ThisPrice < BottomPrice || ThisPrice > TopPrice)
						continue;
				
					double yA = GetBoxYPixel(ThisPrice,"Top");
					double yB = GetBoxYPixel(ThisPrice,"Bottom");
					double yH = GetBoxYPixel(ThisPrice,"Height");
					double yT = yA;		

					double maxwidth = pInvLength;
					double ptv = TotalVol / maxtv;

					double thishwidth = maxwidth * ptv;

					CellString = TotalVol.ToString();
                    
					if (CellTextLayout != null && !CellTextLayout.IsDisposed) CellTextLayout.Dispose();
					CellTextLayout = new TextLayout(Core.Globals.DirectWriteFactory, CellString, CellFormat, 10000, 10000);

                    float FinalH = CellTextLayout.Metrics.Height;
                    float FinalW = CellTextLayout.Metrics.Width + 8;

					thishwidth = Math.Max(thishwidth,FinalW);
					CellTextLayout.Dispose();
					
					x1 = ChartPanel.X + ChartPanel.W - RightMoveX;
					x2 = x1 - thishwidth;
					
					ThisRect = new SharpDX.RectangleF((float)x2, (float)yT, (float)thishwidth, (float)yH);
														
					ThisBrushDX = ChartBackgroundBrushDX;
				
					RenderTarget.FillRectangle(ExpandRect(ThisRect,1,1,1,1), ThisBrushDX);
					
					ThisBrushDX = ChartBackgroundBrushDX;
					if (pUseBackgroundColor)
						ThisBrushDX = BackgroundColorDX;
					
					RenderTarget.FillRectangle(ThisRect, ThisBrushDX);
					
					ThisBrushDX = AskHistColor1DX;
					
					if (TotalVol == largestAskVol)
						ThisBrushDX = AskHistColor2DX;
					
					RenderTarget.FillRectangle(ThisRect, ThisBrushDX);

					ThisRect = new SharpDX.RectangleF((float)x2-4, (float)yT, (float)thishwidth, (float)yH);
					
					ThisBrushDX = TextColorMDDX;
					RenderTarget.DrawText(CellString, CellFormat, ThisRect, ThisBrushDX);	
					
				}
				
				foreach (LadderRow MDR in BidLadder)
				//foreach (LadderRow MDR in bidRows)
				//foreach (MarketDepthRow MDR in ThisMarketDepth.Bids)
				//foreach (KeyValuePair<double, long> DepthLevels in BidLevels)
				{
					
					//double ThisPrice = DepthLevels.Key;
					//double TotalVol = DepthLevels.Value;
					
					double ThisPrice = MDR.Price;
					double TotalVol = MDR.Volume;
					
					if (ThisPrice < LowestAllowed)
						continue;
					
					if (ThisPrice < BottomPrice || ThisPrice > TopPrice)
						continue;
					
					double yA = GetBoxYPixel(ThisPrice,"Top");
					double yB = GetBoxYPixel(ThisPrice,"Bottom");
					double yH = GetBoxYPixel(ThisPrice,"Height");
					double yT = yA;		

					double maxwidth = pInvLength;
					double ptv = TotalVol / maxtv;

					double thishwidth = maxwidth * ptv;

					CellString = TotalVol.ToString();
                    
					if (CellTextLayout != null && !CellTextLayout.IsDisposed) CellTextLayout.Dispose();
					CellTextLayout = new TextLayout(Core.Globals.DirectWriteFactory, CellString, CellFormat, 10000, 10000);

                    float FinalH = CellTextLayout.Metrics.Height;
                    float FinalW = CellTextLayout.Metrics.Width + 8;

					thishwidth = Math.Max(thishwidth,FinalW);
					CellTextLayout.Dispose();
					
					x1 = ChartPanel.X + ChartPanel.W - RightMoveX;
					x2 = x1 - thishwidth;
					
					ThisRect = new SharpDX.RectangleF((float)x2, (float)yT, (float)thishwidth, (float)yH);
					
					ThisBrushDX = ChartBackgroundBrushDX;
				
					RenderTarget.FillRectangle(ExpandRect(ThisRect,1,1,1,1), ThisBrushDX);
					
					ThisBrushDX = ChartBackgroundBrushDX;
					if (pUseBackgroundColor)
						ThisBrushDX = BackgroundColorDX;
					
					RenderTarget.FillRectangle(ThisRect, ThisBrushDX);
					
					ThisBrushDX = BidHistColor1DX;
					
					if (TotalVol == largestBidVol)
						ThisBrushDX = BidHistColor2DX;
					
					RenderTarget.FillRectangle(ThisRect, ThisBrushDX);

					ThisRect = new SharpDX.RectangleF((float)x2-4, (float)yT, (float)thishwidth, (float)yH);
					
					ThisBrushDX = TextColorMDDX;
					RenderTarget.DrawText(CellString, CellFormat, ThisRect, ThisBrushDX);	
					
				}	
				
				// totals
				
				ThisBrushDX = AxisBrushDX;
			
				CellString = sAsk.ToString();
			
				double yA1 = GetBoxYPixel(lastAPrice,"Top");
				double yB1 = GetBoxYPixel(lastAPrice,"Bottom");
				double yH1 = GetBoxYPixel(lastAPrice,"Height");
				double yT1 = yA1;		

				ThisRect = new SharpDX.RectangleF((float)x1 - pInvLength - 3, (float)yT1, (float)pInvLength, (float)yH1);
				
				if (yT1 != 0)
				RenderTarget.DrawText(CellString, CellFormat, ThisRect, ThisBrushDX);		
			
				CellString = sBid.ToString();
								
				yA1 = GetBoxYPixel(lastBPrice,"Top");
				yB1 = GetBoxYPixel(lastBPrice,"Bottom");
				yH1 = GetBoxYPixel(lastBPrice,"Height");
				yT1 = yA1;	
			
				ThisRect = new SharpDX.RectangleF((float)x1 - pInvLength - 3, (float)yT1, (float)pInvLength, (float)yH1);
				
				
				
				if (yT1 != 0)
				RenderTarget.DrawText(CellString, CellFormat, ThisRect, ThisBrushDX);	
			

					
				AskHistColor1DX.Dispose();
				AskHistColor2DX.Dispose();
				AskOutColor1DX.Dispose();
				BidHistColor1DX.Dispose();
				BidHistColor2DX.Dispose();
				BidOutColor1DX.Dispose();

				TextColorMDDX.Dispose();
					
				
				CellFormat.Dispose();
				CellTextLayout.Dispose();
				
				RightMoveX = RightMoveX + pInvLength;
					
				
				
				// vertical line
				
				
				x1 = ChartPanel.X + ChartPanel.W - RightMoveX - pSpaceBetweenColumns;
				CurrentMDX = x1;
				
				
				
				// stand alone
				
				StartPoint	= new Point(CurrentMDX, 0);
				EndPoint = new Point(CurrentMDX, 5000);
				
				
				MoveMD = new SharpDX.RectangleF((float)StartPoint.X, (float)0, (float)1, (float)5000);						
						
				if (IsMoveMD || IsHoverMD)	
				{
					//ThisStroke = new Stroke(Brushes.WhiteSmoke, DashStyleHelper.Solid, 2);
					ThisStroke = pHighlightStroke;
					RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), VerticalLineHighlightDX, ThisStroke.Width, ThisStroke.StrokeStyle);
				}
				
				
				
				ThisStroke = myProperties.GridLineVPen;
				RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ThisStroke.BrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
				
				
			
				
				
				
			}
			
		}
		
		private void DrawLastStrokes (ChartControl chartControl, ChartScale chartScale)
		{			
			if (!IsHardRightEdge)
				return;
			
			x1 = chartControl.GetXByBarIndex(ChartBars,LB);
				
			double xL = x1 - barWidth - 1;
			//double xL2 = x1;
			double xR = x1 + barWidth;
			double xW = barWidth + barWidth + 1;
			double xW2 = barWidth;
			
			ThisStroke = pLastPriceLineStroke;
			
			y1 = chartScale.GetYByValue(CurrentLastData);

			
			double yA = GetBoxYPixel(CurrentLastData,"Top");
			double yB = GetBoxYPixel(CurrentLastData,"Bottom");
			double yH = GetBoxYPixel(CurrentLastData,"Height");
			double yT = yA;		
			
			StartPoint	= new Point(xL,yT);
			EndPoint = new Point(xL,yT+yH);			
			RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ThisStroke.BrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
											
			StartPoint	= new Point(xR,yT);
			EndPoint = new Point(xR,yT+yH);
			RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ThisStroke.BrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);		
		
		}
		
		private void DrawLastPriceBox (ChartControl chartControl, ChartScale chartScale, SharpDX.Direct2D1.Brush LastPriceBrushDX, double CurrentLast)
		{			
//							x1 = chartControl.GetXByBarIndex(ChartBars,LB);
				
//							double xL = x1 - barWidth - 1;
//							double xL2 = x1;
//							double xR = x1 + barWidth;
//							double xW = barWidth + barWidth + 1;
//							double xW2 = barWidth;
							

							
							y1 = chartScale.GetYByValue(CurrentLast);

							double yA = GetBoxYPixel(CurrentLast,"Top");
							double yB = GetBoxYPixel(CurrentLast,"Bottom");
							double yH = GetBoxYPixel(CurrentLast,"Height");
							double yT = yA;		
							
			
							float chartW = ChartPanel.W;

							if (pLastPriceStyle == "Line + Label")
							{
								float lineY = (float)(yT + yH / 2.0);
								RenderTarget.DrawLine(
									new SharpDX.Vector2(0, lineY),
									new SharpDX.Vector2(chartW, lineY),
									LastPriceBrushDX, 1.0f);

								string priceLabel = CurrentLast.ToString(PriceString);
								SharpDX.DirectWrite.TextFormat pillFormat = ChartControl.Properties.LabelFont.ToDirectWriteTextFormat();
								pillFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
								pillFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
								TextLayout pillLayout = new TextLayout(Core.Globals.DirectWriteFactory, priceLabel, pillFormat, 200, 20);
								float pillW = pillLayout.Metrics.Width + 10;
								float pillH = pillLayout.Metrics.Height + 4;
								float pillX = chartW - pillW - 5;
								float pillY = lineY - pillH / 2;

								SharpDX.RectangleF pillRect = new SharpDX.RectangleF(pillX, pillY, pillW, pillH);
								SharpDX.Direct2D1.RoundedRectangle roundedPill = new SharpDX.Direct2D1.RoundedRectangle
								{
									Rect = pillRect,
									RadiusX = 3,
									RadiusY = 3
								};

								var savedAA = RenderTarget.AntialiasMode;
								RenderTarget.AntialiasMode = SharpDX.Direct2D1.AntialiasMode.PerPrimitive;
								RenderTarget.FillRoundedRectangle(roundedPill, LastPriceBrushDX);
								RenderTarget.DrawText(priceLabel, pillFormat, pillRect, ChartTextBrushDX);
								RenderTarget.AntialiasMode = savedAA;

								pillLayout.Dispose();
								pillFormat.Dispose();
							}
							else
							{
								ThisRect = new SharpDX.RectangleF((float)0, (float)yT, chartW, (float)yH);
								ThisBrushDX = LastPriceBrushDX;
								RenderTarget.FillRectangle(ThisRect, ThisBrushDX);
							}

		}
		

		private void DrawClickEntry (ChartControl chartControl, ChartScale chartScale, SharpDX.Direct2D1.Brush LastPriceBrushDX, double LastMousePrice)
		{		
			
						
			ChartBackgroundFade2BrushDX = ChartControl.Properties.ChartBackground.ToDxBrush(RenderTarget);
			ChartBackgroundFade2BrushDX.Opacity = pOrderFlagButtonOpacity/100f;
			
			ClickEntryCancelRect = new SharpDX.RectangleF(0, 0, 0, 0);	
			
			
			 SharpDX.DirectWrite.TextFormat ButtonText = pTextFont66.ToDirectWriteTextFormat();
			
 				TriggerCustomEvent(o =>
	   			{
	      
					Values[FirstPlot+1][0] = LastMousePrice;
					//PlotBrushes[20][0] = LastPriceBrushDX;
					
					if (BuyClickReady) PlotBrushes[FirstPlot+1][0] = pClickUPColor;
					
					if (SellClickReady) PlotBrushes[FirstPlot+1][0] = pClickDNColor;
					
					
				}, null);
			
			
			
			
			
			
			
							
				double ThisPrice = LastMousePrice; // main price of everything
						
				if (LastMousePrice == 0)
					ThisPrice = CurrentLastData;
												
						double ThisPrice2 = ThisPrice;
					
						
						int allopacity = 100;

						
						
						
						
						//	if (AllOrderPrices.ContainsKey(ThisPrice))
							//startright = AllOrderPrices[ThisPrice];
						
						
							
							double ThisPrice3 = ThisPrice2;
							
						
y1 = chartScale.GetYByValue(ThisPrice3);

							// adjust for screen size
							
							
							
				CellFormat			= pTextFont2.ToDirectWriteTextFormat();	
				
				TextLayout CellTextLayout33 = new TextLayout(Core.Globals.DirectWriteFactory, CellString, CellFormat, 10000, 10000);
				
				OrderDisplayMinHeight = (int) CellTextLayout33.Metrics.Height + (int) Math.Ceiling(CellTextLayout33.Metrics.Height/8) + 2;
				
				CellTextLayout33.Dispose();
							
							double yA = y1 - OrderDisplayMinHeight/2-1;
							double yB = y1 - OrderDisplayMinHeight/2-1;
							double yH = OrderDisplayMinHeight;
							double yT = yA;
							
//							yA = GetBoxYPixel(ThisPrice3,"Top");
//							yB = GetBoxYPixel(ThisPrice3,"Bottom");
//							yH = GetBoxYPixel(ThisPrice3,"Height");
//							yT = yA;		
						
//						if (yH < OrderDisplayMinHeight)
//						{
//							double diff = OrderDisplayMinHeight - yH;
//							yH = Math.Max(OrderDisplayMinHeight, yH);
							
//							yT = yT - diff/2;
//						}
				
				
				
				
				
//							double y1 = chartScale.GetYByValue(ThisPrice3);

//							double yA = GetBoxYPixel(ThisPrice3,"Top");
//							double yB = GetBoxYPixel(ThisPrice3,"Bottom");
//							double yH = GetBoxYPixel(ThisPrice3,"Height");
//							double yT = yA;		
						
//						if (yH < OrderDisplayMinHeight)
//						{
//							double diff = OrderDisplayMinHeight - yH;
//							yH = Math.Max(OrderDisplayMinHeight, yH);
							
//							yT = yT - diff/2;
//						}
						
						
						float orderdwidth = 10000;
											
			
						float startright = ChartPanel.W - maininitialspace;
						
							//ThisRect = new SharpDX.RectangleF((float)0, (float)yT, (float)startright, (float)yH);
						
						ThisRect = new SharpDX.RectangleF((float)startright - orderdwidth, (float)yT, orderdwidth, (float)yH);
						
						
							// change colors
												
							//ThisBrushDX = LastPriceBrushDX;
						
				
						if (pMakeOrderOutlineNotSeeThru) RenderTarget.FillRectangle(ExpandRect(ThisRect,3,3), ChartBackgroundBrushDX);
						
							ThisBrushDX = ChartControl.Properties.ChartText.ToDxBrush(RenderTarget);
							ThisBrushDX.Opacity = (float) pOrderOutlineOpacity/100f;
							
							RenderTarget.FillRectangle(ExpandRect(ThisRect,3,3), ThisBrushDX);
						
						
						
						
						
							RenderTarget.FillRectangle(ExpandRect(ThisRect,1,1), ChartBackgroundBrushDX);
						
							
													
						if (BuyClickReady) 
						{
							ThisBrushDX = pClickUPColor.ToDxBrush(RenderTarget);
							ThisBrushDX2 = GetTextColor(pClickUPColor).ToDxBrush(RenderTarget);						
						}
						else
						{
							ThisBrushDX = pClickDNColor.ToDxBrush(RenderTarget);
							ThisBrushDX2 = GetTextColor(pClickDNColor).ToDxBrush(RenderTarget);	
							
						}
		
				
						
//							if (or.Name.Contains("Target"))
//							{
//								ThisBrushDX = pOrderTargetColor.ToDxBrush(RenderTarget);
//								ThisBrushDX2 = GetTextColor(pOrderTargetColor).ToDxBrush(RenderTarget);
								
//								typee = or.Name;
//							}
//							else if (or.Name.Contains("Stop"))
//							{
//								ThisBrushDX = pOrderStopColor.ToDxBrush(RenderTarget);
//								ThisBrushDX2 = GetTextColor(pOrderStopColor).ToDxBrush(RenderTarget);
							
//								typee = or.Name;
//							}					
		
							
//							else if (or.OrderType == OrderType.Limit)
//							{
//								ThisBrushDX = pOrderLimitColor.ToDxBrush(RenderTarget);
//								ThisBrushDX2 = GetTextColor(pOrderLimitColor).ToDxBrush(RenderTarget);
								
//								typee = "LMT";
//							}
//							else if (or.OrderType == OrderType.MIT)
//							{
//								ThisBrushDX = pOrderMITColor.ToDxBrush(RenderTarget);
//								ThisBrushDX2 = GetTextColor(pOrderMITColor).ToDxBrush(RenderTarget);
								
//								typee = "MIT";
//							}
//							else if (or.OrderType == OrderType.StopLimit)
//							{
//								ThisBrushDX = pOrderStopLimitColor.ToDxBrush(RenderTarget);
//								ThisBrushDX2 = GetTextColor(pOrderStopLimitColor).ToDxBrush(RenderTarget);
								
//								typee = "SLM";
//							}
//							else if (or.OrderType == OrderType.StopMarket)
//							{
//								ThisBrushDX = pOrderStopMarketColor.ToDxBrush(RenderTarget);
//								ThisBrushDX2 = GetTextColor(pOrderStopMarketColor).ToDxBrush(RenderTarget);
								
//								typee = "STP";
//							}							
							
//							if (or.OrderState == OrderState.ChangePending || or.OrderState == OrderState.ChangeSubmitted || or.OrderState == OrderState.CancelPending || or.OrderState == OrderState.CancelSubmitted)
//							{
//								ThisBrushDX = pOrderChangeColor.ToDxBrush(RenderTarget);
//								ThisBrushDX2 = GetTextColor(pOrderChangeColor).ToDxBrush(RenderTarget);
//							}
							
							
//							ThisBrushDX.Opacity = (float) allopacity/100f;
//							ThisBrushDX2.Opacity = (float) Math.Min(100,allopacity+10)/100f;
							
							
							//if (OrderIsAboveChart)
//							else if (or.OrderAction == OrderAction.Sell || or.OrderAction == OrderAction.SellShort)
//							{
//								ThisBrushDX = pClickDNColor.ToDxBrush(RenderTarget);
							
//							}
//							else
//							{
//								ThisBrushDX = pClickUPColor.ToDxBrush(RenderTarget);
							
							
								
//							}
							
							
//							if (or.OrderAction == OrderAction.Sell || or.OrderAction == OrderAction.SellShort)
//							{
								
//								dire = "Sell";	
//							}
//							else
//							{
//								dire = "Buy";	
//							}
							
								
							
//							if (or.Name.Contains ("Stop") || or.Name.Contains ("Target"))
//							{
								
//								dire = "";
								
//							}
								
						
//							string dirrec = "";

//							if (dire != "")
//								dirrec = dirrec + " " + dire;
							
//							if (typee != "")
//								dirrec = dirrec + " " + typee;						
						
							
//		                  //  CellString = cancelmessage + dirrec + " " + qudSelector.Value + " " + ClickText;
						
//							dirrec = dirrec.Replace("Target", "Target ");
//							dirrec = dirrec.Replace("Stop", "Stop ");
							
							
//							CellString = dirrec;
							
							
						
							

							
							
							RenderTarget.FillRectangle(ThisRect, ThisBrushDX);
						
						
							//RenderTarget.FillRectangle(ExpandRect(ThisRect,3,3), ChartBackgroundMenuFadeBrushDX);
			
						// cancel buttons
												
							ThisRect2 = new SharpDX.RectangleF((float)startright-(float)yH-6, (float)yT, (float)yH+2, (float)yH);
						
						ClickEntryCancelRect = ThisRect2;
						
						
							RenderTarget.FillRectangle(ThisRect2, ChartBackgroundFade2BrushDX);
						
			
							//return;
							
						// Xsssss
							
						ThisStroke = pOneWidthStroke;
						
									
						oldAntialiasMode = RenderTarget.AntialiasMode;
			            RenderTarget.AntialiasMode = SharpDX.Direct2D1.AntialiasMode.PerPrimitive;

			       
			
							
//						StartPoint	= new Point(ThisRect2.Left, ThisRect2.Top);							
//						EndPoint = new Point(ThisRect2.Right, ThisRect2.Bottom);		
						
						
//						RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ChartBackgroundBrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
						
//						StartPoint	= new Point(ThisRect2.Left, ThisRect2.Bottom);							
//						EndPoint = new Point(ThisRect2.Right, ThisRect2.Top);		
						
						
//						RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ChartBackgroundBrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);

							
						
						CellFormat			= pTextFont2.ToDirectWriteTextFormat();	
						CellFormat			= ButtonText;
				
	                    CellFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
	                    CellFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
	                    CellFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;
						//CellFormat.FontSize = 15;
					
						
						ThisBrushDX = ChartTextBrushDX;
						RenderTarget.DrawText("X", CellFormat, ThisRect2, ThisBrushDX2);	
//						RenderTarget.DrawText("X", CellFormat, ThisRect2, Brushes.Red.ToDxBrush(RenderTarget));		
							
							
						RenderTarget.AntialiasMode = oldAntialiasMode;
							
							
						StartPoint	= new Point(ThisRect2.Left, ThisRect2.Bottom);							
						EndPoint = new Point(ThisRect2.Left, ThisRect2.Top);								
						
						RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ChartBackgroundBrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
									
						StartPoint	= new Point(ThisRect2.Right, ThisRect2.Bottom);							
						EndPoint = new Point(ThisRect2.Right, ThisRect2.Top);								
						
						RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ChartBackgroundBrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
							
						
						
						
						// end cancel buttons
								
//							double orderprice = 0;
							
//							if (docombinestopsperprice && or.Name.Contains("Stop"))
//								orderprice = ThisPrice2;
										
							
//							buttonseq = buttonseq + 1;
						
//						    OrderDetails Z = new OrderDetails();
//				            Z.ThisOrder = or;
//							Z.ThisRectA = ThisRect2;
//				            Z.Name = "";
//				            Z.Width = orderprice;
//				            Z.Switch = false;
//				            Z.Rect = new SharpDX.RectangleF(0, 0, 0, 0);
//				            Z.Hovered = false;

							//AllOrderCancelButtons.Add(buttonseq,Z);
		
						// move buttons
												
							//ThisRect2 = new SharpDX.RectangleF((float)startright-(float)yH-(float)yH-20f, (float)yT, (float)yH+1, (float)yH);					
							//RenderTarget.FillRectangle(ThisRect2, ChartBackgroundMenuFadeBrushDX);
						//	buttonseq = buttonseq + 1;
						
//						    Z = new OrderDetails();
//				            Z.ThisOrder = or;
//							Z.ThisRectA = ThisRect2;
//							Z.ThisRectA = ThisRect;
						
//				            Z.Name = "";
//				            Z.Width = orderprice;
//				            Z.Switch = false;
//				            Z.Rect = new SharpDX.RectangleF(0, 0, 0, 0);
//				            Z.Hovered = false;

							
//							if (!OrderIsBelowChart && !OrderIsAboveChart)
//							AllOrderMoveButtons.Add(buttonseq,Z);
					
						// end
						
						// text on top of order
						
					
						float sidepadding = 5;	
							
							float lefttt = startright - ThisRect2.Left + sidepadding + 1;
						
							
							
//							if (docombinestopsperprice && or.Name.Contains("Stop"))
//								CellString = "Stop Loss";
							
							
							
							CellFormat			= pTextFont2.ToDirectWriteTextFormat();	
							CellFormat			= ButtonText;
					
		                    CellFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Trailing;
		                    CellFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
		                    CellFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;

			
//					CellFormat			= pTextFont2.ToDirectWriteTextFormat();	
//					CellFormat			= ButtonText;
			
//                    CellFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Trailing;
//                    CellFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
//                    CellFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;
				
			string dirrec = "BUY";
				
				if (SellClickReady)
					dirrec = "SELL";
				
				string cancelmessage = "(Click button to cancel.)           ";
				cancelmessage = string.Empty;
				
                    CellString = cancelmessage + qudSelector.Value + "  " + dirrec + " " + ClickText;
			
				//ThisBrushDX = ChartTextBrushDX;
			//	RenderTarget.DrawText(CellString, CellFormat, ExpandRect(ThisRect,-10,0), ThisBrushDX);	
			
							
							
						
						//ThisBrushDX = ChartTextBrushDX;
							
				
						RenderTarget.DrawText(CellString, CellFormat, ExpandRect(ThisRect,0,lefttt*-1-10,0,0), ThisBrushDX2);	
					
					
							// quanitity text
				
//						CellString = or.Quantity.ToString();
//						if (docombinestopsperprice && or.Name.Contains ("Stop"))
//							CellString = AllStopLossPrices[ThisPrice2].ToString();							
							
//						CellFormat			= pTextFont2.ToDirectWriteTextFormat();	
//						CellFormat			= ButtonText;
					
//		                    CellFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Leading;
//		                    CellFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
//		                    CellFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;

						
						
						//ThisBrushDX = ChartTextBrushDX;
							
						
						//RenderTarget.DrawText(CellString, CellFormat, ExpandRect(ThisRect,sidepadding*-1,0,0,0), ThisBrushDX2);							
							
							
				
//						if (!AllOrderPrices.ContainsKey(ThisPrice))
//							AllOrderPrices.Add(ThisPrice, startright - orderdwidth - orderhspace);
//						else
//						{
//							startright =  AllOrderPrices[ThisPrice] - orderdwidth - orderhspace;
//							AllOrderPrices[ThisPrice] = startright;
//						}
						
						
					
					
						
			
			
			
			
			
			
			
			
			
//			SharpDX.DirectWrite.TextFormat ButtonText = pTextFont66.ToDirectWriteTextFormat();

							
//							y1 = chartScale.GetYByValue(LastMousePrice);

//							double yA = GetBoxYPixel(LastMousePrice,"Top");
//							double yB = GetBoxYPixel(LastMousePrice,"Bottom");
//							double yH = GetBoxYPixel(LastMousePrice,"Height");
//							double yT = yA;		
							
			
//							ThisRect = new SharpDX.RectangleF((float)0, (float)yT, (float)ChartPanel.W, (float)yH);
						
//							// change colors
												
//							ThisBrushDX = LastPriceBrushDX;
//							RenderTarget.FillRectangle(ThisRect, ThisBrushDX);
//							RenderTarget.FillRectangle(ExpandRect(ThisRect,3,3), ChartBackgroundMenuFadeBrushDX);
			
			
		
//					CellFormat			= pTextFont2.ToDirectWriteTextFormat();	
//					CellFormat			= ButtonText;
			
//                    CellFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Trailing;
//                    CellFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
//                    CellFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;
				
//			string dirrec = "BUY";
				
//				if (SellClickReady)
//					dirrec = "SELL";
				
//				string cancelmessage = "(Click button to cancel.)           ";
//				cancelmessage = string.Empty;
				
//                    CellString = cancelmessage + dirrec + " " + qudSelector.Value + " " + ClickText;
			
//				ThisBrushDX = ChartTextBrushDX;
//				RenderTarget.DrawText(CellString, CellFormat, ExpandRect(ThisRect,-10,0), ThisBrushDX);	
			
			ChartBackgroundFade2BrushDX.Dispose();
				
		}
		
		
		
		
		
		private void DrawMoveOrders (ChartControl chartControl, ChartScale chartScale, SharpDX.Direct2D1.Brush LastPriceBrushDX, double CurrentLast)
		{			
			
		
			
			ChartBackgroundFade2BrushDX = ChartControl.Properties.ChartBackground.ToDxBrush(RenderTarget);
			ChartBackgroundFade2BrushDX.Opacity = pOrderFlagButtonOpacity/100f;
			
            SharpDX.DirectWrite.TextFormat ButtonText = pTextFont66.ToDirectWriteTextFormat();
			//ButtonText = myProperties.LabelFont.ToDirectWriteTextFormat();

//							x1 = chartControl.GetXByBarIndex(ChartBars,LB);
	
//							double xL = x1 - barWidth - 1;
//							double xL2 = x1;
//							double xR = x1 + barWidth;
//							double xW = barWidth + barWidth + 1;
//							double xW2 = barWidth;
				
				TriggerCustomEvent(o =>
	   			{
	      
					Values[FirstPlot][0] = CurrentLast;
					PlotBrushes[FirstPlot][0] = Brushes.Gray;
				
				}, null);
				
			
			
				double ThisPrice = 0;
			bool docombinestopsperprice = !pSplitStopDisplay;
			float orderdwidth = pOrderDisplayWidth;
											
			
			CellFormat			= pTextFont2.ToDirectWriteTextFormat();	
				
				TextLayout CellTextLayout3333 = new TextLayout(Core.Globals.DirectWriteFactory, "Stop Loss 100", CellFormat, 10000, 10000);
				
				orderdwidth = (int) CellTextLayout3333.Metrics.Width + (int) Math.Ceiling(CellTextLayout3333.Metrics.Width/2);
				
				CellTextLayout3333.Dispose();
			

					
		float changeallbuttonswidth = 0;
				
				
			FoundStopLossPrices.Clear();
			
			List<Order> ThisMovingOrders = new List<Order>(); 
		
			if (MovingOrders.Count > 0)
				ThisMovingOrders = new List<Order>(MovingOrders);
			else
				ThisMovingOrders.Add(MovingOrder);
	
			
			AllOrderPrices.Clear();
			FoundStopLossPrices.Clear();
			//Order or = MovingOrder;
			
			
			//Print(ThisMovingOrders.Count);
			
			bool firstone = true;
			
				foreach (Order or in ThisMovingOrders.ToList())
				{
					
					NewStopPrice = 0;
					
					//TrackedOrders(
					
					
							OrderStateOK = or.OrderState == OrderState.Working || or.OrderState == OrderState.Accepted || or.OrderState == OrderState.ChangePending  || or.OrderState == OrderState.ChangeSubmitted || or.OrderState == OrderState.CancelPending || or.OrderState == OrderState.CancelSubmitted;
							//OrderTypeOK = or.OrderType == OrderType.StopLimit || or.OrderType == OrderType.StopMarket;
							//OrderNameOK = or.Name == "Stop1" || or.Name == "Stop2" ||  or.Name == "Stop3";
							//OrderNameOK = or.Oco.Contains("LE1") || or.Oco.Contains("LE2") || or.Oco.Contains("SE1") || or.Oco.Contains("SE2");
				
//							if (whichone == "1")
//								OrderNameOK = or.Oco.Contains("LE1") || or.Oco.Contains("SE1");
//							if (whichone == "2")
//								OrderNameOK = or.Oco.Contains("LE2") || or.Oco.Contains("SE2");				
					
							OrderInstrumentOK = or.Instrument.FullName == Instrument.FullName;
							OrderNameOK = true;
							OrderTypeOK = true;
					//OrderStateOK = true;
					
					//if (OrderStateOK)
					//Print(or.OrderId);
					
					//if (ThisMovingOrders.Contains(or))
					//if (OrderTypeOK && OrderStateOK && OrderNameOK && OrderInstrumentOK)
					{
							
						ThisPrice = or.StopPrice;
						
						//ThisPrice = CurrentLast;
						
						if (or.OrderType == OrderType.Limit)
						{
							ThisPrice = or.LimitPrice;
							
						}
						

												
						double ThisPrice2 = ThisPrice;
					double ThisPrice3 = CurrentLast;
						
						

						
						
						
//						Print(chartScale.MaxValue);
//						Print(MaxValue);
						
//						double HighPrice = ThisMasterInstrument.RoundDownToTickSize(chartScale.MaxValue - tSize);
//						double LowPrice = ThisMasterInstrument.RoundToTickSize(chartScale.MinValue + tSize);
						
//						bool OrderIsAboveChart = false;
//						bool OrderIsBelowChart = false;
						
						
						if (docombinestopsperprice && or.Name.Contains ("Stop"))
						{
							
							if (FoundStopLossPrices.ContainsKey(ThisPrice))
							{
								
								continue;	
								
							}
							
							
								if (!FoundStopLossPrices.ContainsKey(ThisPrice))
									FoundStopLossPrices.Add(ThisPrice, 1);
							
							
														
						}
											
//						if (pShowOrdersOffChart)
//						{
							
							
//							if (ThisPrice > HighPrice)
//							{
//								ThisPrice = 1000000;
//								OrderIsAboveChart = true;
//							}
							
//							if (ThisPrice < LowPrice)
//							{
//								ThisPrice = 1000001;
//								OrderIsBelowChart = true;
//							}
							
//						}
						
						
							

						
					
						

//						if (OrderIsBelowChart || OrderIsAboveChart)
//							allopacity = pHiddenOrdersOpacity;
						
						
						
						
						
						string dire = "";
						string typee = "STP";
						

						//Print(ThisPrice);
						
						
						if (or.IsShort) // In Long Trade
						{
							//if (Mode == "BE")
							//	NewStopPrice = RTTS(ThisPositionNow().AveragePrice + beoo*TickSize);

						}
						else // In Short Trade
						{
							//if (Mode == "BE")
							//	NewStopPrice = RTTS(ThisPositionNow().AveragePrice - beoo*TickSize);

						}
						
//						StopLimitOffset = or.StopPrice - or.LimitPrice;
//									NewLimitPrice = NewStopPrice - StopLimitOffset;
									
//									CurrentStopPrice = or.StopPrice;
						
//						if (or.LimitPrice == 0)
//							NewLimitPrice = 0;
						
						
						//if (NewStopPrice != 0 && NewStopPrice != CurrentStopPrice)
						//	instruction = OIF_ChangeOrder(0,NewLimitPrice,NewStopPrice,or.Id.ToString(),"");
						
						//Submit();
						

			
//							x1 = chartControl.GetXByBarIndex(ChartBars,LB);
				
//							double xL = x1 - barWidth - 1;
//							double xL2 = x1;
//							double xR = x1 + barWidth;
//							double xW = barWidth + barWidth + 1;
//							double xW2 = barWidth;
							
						
						float startright = ChartPanel.W - maininitialspace;
						//startright = ChartPanel.W - maininitialspace - adjusttoright 
						
							double y1 = 0;

//							double yA = 0;
//							double yB = 0;
//							double yH = 0;
//							double yT = yA;	
						
y1 = chartScale.GetYByValue(ThisPrice3);

							// adjust for screen size
							
							
							
				CellFormat			= pTextFont2.ToDirectWriteTextFormat();	
				
				TextLayout CellTextLayout33 = new TextLayout(Core.Globals.DirectWriteFactory, CellString, CellFormat, 10000, 10000);
				
				OrderDisplayMinHeight = (int) CellTextLayout33.Metrics.Height + (int) Math.Ceiling(CellTextLayout33.Metrics.Height/8) + 2;
				
				CellTextLayout33.Dispose();
							
							double yA = y1 - OrderDisplayMinHeight/2-1;
							double yB = y1 - OrderDisplayMinHeight/2-1;
							double yH = OrderDisplayMinHeight;
							double yT = yA;
							
//							yA = GetBoxYPixel(ThisPrice3,"Top");
//							yB = GetBoxYPixel(ThisPrice3,"Bottom");
//							yH = GetBoxYPixel(ThisPrice3,"Height");
//							yT = yA;		
						
//						if (yH < OrderDisplayMinHeight)
//						{
//							double diff = OrderDisplayMinHeight - yH;
//							yH = Math.Max(OrderDisplayMinHeight, yH);
							
//							yT = yT - diff/2;
//						}
						
					float sidepadding = 5;	
							
					float	lefttt = 	0;					
						
						//insert summary code
						
						 
						if (firstone && pShowOrderSummary)
						{
	
						CellString = "   ";
						
						if (pShowOrderSummary)
								if (pThisSummaryTypeMode == "Ticks")
								{
									if (ThisPositionNow() != null)
									{
										double tickstoentry = Math.Round(Math.Abs(RTTS(ThisPositionNow().AveragePrice) - ThisPrice3)/tSize, 0);
										
										CellString = tickstoentry.ToString();
										
									}
									else
									{
										double tickstolasttrade = Math.Round(Math.Abs(CurrentLastData - ThisPrice3)/tSize, 0);
										CellString = tickstolasttrade.ToString();
										
									}
								}
								else
								{
									
									
									
								}
									
								
									
								CellFormat			= ButtonText;
							
				                    CellFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Leading;
				                    CellFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
				                    CellFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;

		
						if (CellString.Length <= 1)
							CellString = "00";
						
						TextLayout CellTextLayout = new TextLayout(Core.Globals.DirectWriteFactory, CellString, CellFormat, 10000, 10000);
						
					
						int addt = (int) yH + 2 + 6;
						
						bool showcancel = true;
						if (!showcancel)
							addt = 0;
						
						int textsize = (int) CellTextLayout.Metrics.Width + 10;
						CellTextLayout.Dispose();

						int minimumspacetoclick = (int) yH + 2; // 20
						
						textsize = Math.Max(textsize, minimumspacetoclick);
						
						changeallbuttonswidth = textsize + addt;
								
						
						ThisRect = new SharpDX.RectangleF((float)startright - changeallbuttonswidth, (float)yT, changeallbuttonswidth, (float)yH);
						
						
							// change colors
												
							//ThisBrushDX = LastPriceBrushDX;
						
				
							if (pMakeOrderOutlineNotSeeThru) RenderTarget.FillRectangle(ExpandRect(ThisRect,3,3), ChartBackgroundBrushDX);
						
							ThisBrushDX = ChartControl.Properties.ChartText.ToDxBrush(RenderTarget);
							ThisBrushDX.Opacity = (float) pOrderOutlineOpacity/100f;
							
							RenderTarget.FillRectangle(ExpandRect(ThisRect,3,3), ThisBrushDX);
						
						
						
						
						
							RenderTarget.FillRectangle(ExpandRect(ThisRect,1,1), ChartBackgroundBrushDX);
						
						
						
			
//							{
								ThisBrushDX = pOrderSummaryColor.ToDxBrush(RenderTarget);
								ThisBrushDX2 = GetTextColor(pOrderSummaryColor).ToDxBrush(RenderTarget);

						
						
						OrderDetails Z = new OrderDetails();
						//Order or = new Order();
						double orderprice = ThisPrice2;
						

							
							
							
							CellString = string.Empty;
						
							
						
							
							RenderTarget.FillRectangle(ThisRect, ThisBrushDX);
						
				
							//RenderTarget.FillRectangle(ExpandRect(ThisRect,3,3), ChartBackgroundMenuFadeBrushDX);
			
						// cancel buttons
												
							ThisRect2 = new SharpDX.RectangleF((float)startright-(float)yH-6, (float)yT, (float)yH+2, (float)yH);
						
						//if (OrdersAtPrice.Value > 1)
						{							
							if (showcancel) RenderTarget.FillRectangle(ThisRect2, ChartBackgroundFade2BrushDX);
						
				
								
							// Xsssss
								
							ThisStroke = pOneWidthStroke;
							
										
							oldAntialiasMode = RenderTarget.AntialiasMode;
				            RenderTarget.AntialiasMode = SharpDX.Direct2D1.AntialiasMode.PerPrimitive;

				       
				
								
	//						StartPoint	= new Point(ThisRect2.Left, ThisRect2.Top);							
	//						EndPoint = new Point(ThisRect2.Right, ThisRect2.Bottom);		
							
							
	//						RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ChartBackgroundBrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
							
	//						StartPoint	= new Point(ThisRect2.Left, ThisRect2.Bottom);							
	//						EndPoint = new Point(ThisRect2.Right, ThisRect2.Top);		
							
							
	//						RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ChartBackgroundBrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);

							

						
							CellFormat			= pTextFont2.ToDirectWriteTextFormat();	
							CellFormat			= ButtonText;
					
		                    CellFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
		                    CellFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
		                    CellFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;
							//CellFormat.FontSize = 15;
						
							
							ThisBrushDX = ChartTextBrushDX;
							if (showcancel) 
								RenderTarget.DrawText("X", CellFormat, ThisRect2, ThisBrushDX2);	
	//						RenderTarget.DrawText("X", CellFormat, ThisRect2, Brushes.Red.ToDxBrush(RenderTarget));		
								
								
							RenderTarget.AntialiasMode = oldAntialiasMode;
								
								
							StartPoint	= new Point(ThisRect2.Left, ThisRect2.Bottom);							
							EndPoint = new Point(ThisRect2.Left, ThisRect2.Top);								
							
							//if (OrdersAtPrice.Value > 1) 
							if (showcancel) 	RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ChartBackgroundBrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
										
							StartPoint	= new Point(ThisRect2.Right, ThisRect2.Bottom);							
							EndPoint = new Point(ThisRect2.Right, ThisRect2.Top);								
							
							//if (OrdersAtPrice.Value > 1) 
							if (showcancel) 	RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ChartBackgroundBrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
								
								
							// end cancel buttons
									
								
								
							
						
							//if (docombinestopsperprice && or.Name.Contains("Stop"))
								orderprice = ThisPrice2;
										
							
							//buttonseq = buttonseq + 1;
						
						    Z = new OrderDetails();
				            Z.ThisOrder = or;
							Z.ThisRectA = ThisRect2;
				            Z.Name = "";
				            Z.Width = orderprice;
				            Z.Switch = true;
				            Z.Rect = new SharpDX.RectangleF(0, 0, 0, 0);
				            Z.Hovered = false;

							//if (showcancel) AllOrderCancelButtons.Add(1,Z);
		if (showcancel)
							MoveOrderCancelRect = ThisRect2;
							
							
						}
						
					
						// end
						
						// text on top of order
						
						sidepadding = 5;	
							
							lefttt = startright - ThisRect2.Left + sidepadding + 1;
						
							
							
//							if (docombinestopsperprice && or.Name.Contains("Stop"))
//								CellString = "Stop Loss";
							
							
							
					
					
							CellFormat			= pTextFont2.ToDirectWriteTextFormat();	
							CellFormat			= ButtonText;
					
		                    CellFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Trailing;
		                    CellFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
		                    CellFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;

						
						//TextLayout CellTextLayout = new TextLayout(Core.Globals.DirectWriteFactory, CellString, CellFormat, 10000, 10000);
						//ThisBrushDX = ChartTextBrushDX;
							
						
					//	RenderTarget.DrawText(CellString, CellFormat, ExpandRect(ThisRect,0,lefttt*-1,0,0), ThisBrushDX2);	
					
					
							// quanitity text
				
						//CellString = or.Quantity.ToString();
						//if (docombinestopsperprice && or.Name.Contains ("Stop"))
						//	CellString = AllStopLossPrices[ThisPrice].ToString();							
							
							
							if (pShowOrderSummary)
							{
								if (pThisSummaryTypeMode == "Ticks")
								{
									if (ThisPositionNow() != null)
									{
										double tickstoentry = Math.Round(Math.Abs(RTTS(ThisPositionNow().AveragePrice) - ThisPrice3)/tSize, 0);
										
										CellString = tickstoentry.ToString();
										
									}
									else
									{
										double tickstolasttrade = Math.Round(Math.Abs(CurrentLastData - ThisPrice3)/tSize, 0);
										CellString = tickstolasttrade.ToString();
										
									}
								}
								else
								{
									
									
									
								}
									
								
									
								CellFormat			= ButtonText;
							
				                    CellFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Leading;
				                    CellFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
				                    CellFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;

								
								
								//ThisBrushDX = ChartTextBrushDX;
									
								
								RenderTarget.DrawText(CellString, CellFormat, ExpandRect(ThisRect,sidepadding*-1,0,0,0), ThisBrushDX2);							
									
									
							}
							
							
							
						}
						
						
						float adjusttoright2 = changeallbuttonswidth + 9;
						
						if (!pShowOrderSummary)
							adjusttoright2 = 0;
						
						float adjusttoright = 30;
						
						
						if (pShowOrderSummary)
						adjusttoright = adjusttoright - ( float) yH - 7;
						
						startright = ChartPanel.W - maininitialspace - adjusttoright - adjusttoright2;
				
						
						
	
//						if (!AllOrderPrices.ContainsKey(ThisPrice))
//							AllOrderPrices.Add(ThisPrice, startright - orderdwidth - orderhspace);
//						else
//						{
//							startright =  AllOrderPrices[ThisPrice] - orderdwidth - orderhspace;
//							AllOrderPrices[ThisPrice] = startright;
//						}		
						
						//double ThisPrice2 = ThisPrice;
						
						//if (OrderIsAboveChart || OrderIsBelowChart)
						//if (ThisPrice == 1000000 || ThisPrice == 1000001)	
							
						//	ThisPrice2 = CurrentLast;
						
							if (AllOrderPrices.ContainsKey(ThisPrice))
							startright = AllOrderPrices[ThisPrice];
						
						
							
							ThisPrice3 = ThisPrice2;
							
							//if (OrderIsAboveChart || OrderIsBelowChart)
								ThisPrice3 = CurrentLast;
							
//							y1 = chartScale.GetYByValue(ThisPrice3);

//							yA = GetBoxYPixel(ThisPrice3,"Top");
//							yB = GetBoxYPixel(ThisPrice3,"Bottom");
//							yH = GetBoxYPixel(ThisPrice3,"Height");
//							yT = yA;		
						

						
//							if (OrderIsAboveChart)
//								yT = 5;
//							if (OrderIsBelowChart)
//								yT = ChartPanel.H - 3 - yH;							
			
							//ThisRect = new SharpDX.RectangleF((float)0, (float)yT, (float)startright, (float)yH);
						
						
						
						
							// change colors
												
							//ThisBrushDX = LastPriceBrushDX;
						
								
							
							

							if (firstone)
							{
								
								orderdwidth = 20000;
								ThisRect = new SharpDX.RectangleF((float)startright - orderdwidth + adjusttoright, (float)yT, orderdwidth, (float)yH);
								
								
								if (pMakeOrderOutlineNotSeeThru) RenderTarget.FillRectangle(ExpandRect(ThisRect,3,3), ChartBackgroundBrushDX);
							
								ThisBrushDX = ChartControl.Properties.ChartText.ToDxBrush(RenderTarget);
								ThisBrushDX.Opacity = (float) pOrderOutlineOpacity/100f;
								
								RenderTarget.FillRectangle(ExpandRect(ThisRect,3,3), ThisBrushDX);

								RenderTarget.FillRectangle(ExpandRect(ThisRect,1,1), ChartBackgroundBrushDX);
								
								ThisBrushDX = pOrderMovingColor.ToDxBrush(RenderTarget);	
								RenderTarget.FillRectangle(ThisRect, ThisBrushDX);
								
							}
									
							orderdwidth = 105;	
							
										
			CellFormat			= pTextFont2.ToDirectWriteTextFormat();	
				
				TextLayout CellTextLayout33433 = new TextLayout(Core.Globals.DirectWriteFactory, "Stop Loss 100", CellFormat, 10000, 10000);
				
				orderdwidth = (int) CellTextLayout33433.Metrics.Width + (int) Math.Ceiling(CellTextLayout33433.Metrics.Width/2);
				
				CellTextLayout33433.Dispose();

							

							ThisRect = new SharpDX.RectangleF((float)startright - orderdwidth, (float)yT, orderdwidth, (float)yH);
							
							
//							if (pMakeOrderOutlineNotSeeThru) RenderTarget.FillRectangle(ExpandRect(ThisRect,3,3), ChartBackgroundBrushDX);
						
//							ThisBrushDX = ChartControl.Properties.ChartText.ToDxBrush(RenderTarget);
//							ThisBrushDX.Opacity = (float) pOrderOutlineOpacity/100f;
							
//							RenderTarget.FillRectangle(ExpandRect(ThisRect,3,3), ThisBrushDX);

							RenderTarget.FillRectangle(ExpandRect(ThisRect,1,1), ChartBackgroundBrushDX);

							
										
							ThisBrushDX = pOrderMovingColor.ToDxBrush(RenderTarget);
							ThisBrushDX2 = GetTextColor(pOrderMovingColor).ToDxBrush(RenderTarget);
							
							RenderTarget.FillRectangle(ThisRect, ThisBrushDX);
						
						
						
							if (or.Name.Contains("Target"))
							{
								ThisBrushDX = pOrderTargetColor.ToDxBrush(RenderTarget);
								ThisBrushDX2 = GetTextColor(pOrderTargetColor).ToDxBrush(RenderTarget);
								
								typee = or.Name;
							}
							else if (or.Name.Contains("Stop"))
							{
								ThisBrushDX = pOrderStopColor.ToDxBrush(RenderTarget);
								ThisBrushDX2 = GetTextColor(pOrderStopColor).ToDxBrush(RenderTarget);
							
								typee = or.Name;
							}					
		
							
							else if (or.OrderType == OrderType.Limit)
							{
								ThisBrushDX = pOrderLimitColor.ToDxBrush(RenderTarget);
								ThisBrushDX2 = GetTextColor(pOrderLimitColor).ToDxBrush(RenderTarget);
								
								typee = "LMT";
							}
							else if (or.OrderType == OrderType.MIT)
							{
								ThisBrushDX = pOrderMITColor.ToDxBrush(RenderTarget);
								ThisBrushDX2 = GetTextColor(pOrderMITColor).ToDxBrush(RenderTarget);
								
								typee = "MIT";
							}
							else if (or.OrderType == OrderType.StopLimit)
							{
								ThisBrushDX = pOrderStopLimitColor.ToDxBrush(RenderTarget);
								ThisBrushDX2 = GetTextColor(pOrderStopLimitColor).ToDxBrush(RenderTarget);
								
								typee = "SLM";
							}
							else if (or.OrderType == OrderType.StopMarket)
							{
								ThisBrushDX = pOrderStopMarketColor.ToDxBrush(RenderTarget);
								ThisBrushDX2 = GetTextColor(pOrderStopMarketColor).ToDxBrush(RenderTarget);
								
								typee = "STP";
							}							
							
							if (or.OrderState == OrderState.ChangePending || or.OrderState == OrderState.ChangeSubmitted || or.OrderState == OrderState.CancelPending || or.OrderState == OrderState.CancelSubmitted)
							{
								ThisBrushDX = pOrderChangeColor.ToDxBrush(RenderTarget);
								ThisBrushDX2 = GetTextColor(pOrderChangeColor).ToDxBrush(RenderTarget);
							}
							
							
							
							
							int allopacity = 30;
							
							ThisBrushDX.Opacity = (float) allopacity/100f;
							//ThisBrushDX2.Opacity = (float) Math.Min(100,allopacity+10)/100f;
							
							RenderTarget.FillRectangle(ThisRect, ThisBrushDX);
							
							
							//if (OrderIsAboveChart)
//							else if (or.OrderAction == OrderAction.Sell || or.OrderAction == OrderAction.SellShort)
//							{
//								ThisBrushDX = pClickDNColor.ToDxBrush(RenderTarget);
							
//							}
//							else
//							{
//								ThisBrushDX = pClickUPColor.ToDxBrush(RenderTarget);
							
							
								
//							}
							
							
							if (or.OrderAction == OrderAction.Sell || or.OrderAction == OrderAction.SellShort)
							{
								
								dire = "Sell";	
							}
							else
							{
								dire = "Buy";	
							}
							
								
							
							if (or.Name.Contains ("Stop") || or.Name.Contains ("Target"))
							{
								
								dire = "";
								
							}
								
						
							string dirrec = "";

							if (dire != "")
								dirrec = dirrec + " " + dire;
							
							if (typee != "")
								dirrec = dirrec + " " + typee;						
						
							
		                  //  CellString = cancelmessage + dirrec + " " + qudSelector.Value + " " + ClickText;
						
							dirrec = dirrec.Replace("Target", "Target ");
							dirrec = dirrec.Replace("Stop", "Stop ");
							
							
							CellString = dirrec;
							
							
							
							
		
									
							//orderdwidth = 105;	
							
						
						
							
						
					if (firstone && !pShowOrderSummary)
					{

							
							//RenderTarget.FillRectangle(ExpandRect(ThisRect,3,3), ChartBackgroundMenuFadeBrushDX);
			
						// cancel buttons
												
						
						
						float startright2 = ChartPanel.W - maininitialspace - adjusttoright2;
						
							MoveOrderCancelRect = new SharpDX.RectangleF((float)startright2-(float)yH-6, (float)yT, (float)yH+2, (float)yH);
						
							RenderTarget.FillRectangle(MoveOrderCancelRect, ChartBackgroundFade2BrushDX);
						
			
							
						// Xsssss
							
						ThisStroke = pOneWidthStroke;
						
									
						oldAntialiasMode = RenderTarget.AntialiasMode;
			            RenderTarget.AntialiasMode = SharpDX.Direct2D1.AntialiasMode.PerPrimitive;

			       
			
							
//						StartPoint	= new Point(ThisRect2.Left, ThisRect2.Top);							
//						EndPoint = new Point(ThisRect2.Right, ThisRect2.Bottom);		
						
						
//						RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ChartBackgroundBrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
						
//						StartPoint	= new Point(ThisRect2.Left, ThisRect2.Bottom);							
//						EndPoint = new Point(ThisRect2.Right, ThisRect2.Top);		
						
						
//						RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ChartBackgroundBrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);

							
						
						CellFormat			= pTextFont2.ToDirectWriteTextFormat();	
						CellFormat			= ButtonText;
				
	                    CellFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
	                    CellFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
	                    CellFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;
						//CellFormat.FontSize = 15;
					
						
						ThisBrushDX = ChartTextBrushDX;
						RenderTarget.DrawText("X", CellFormat, MoveOrderCancelRect, ThisBrushDX2);	
//						RenderTarget.DrawText("X", CellFormat, ThisRect2, Brushes.Red.ToDxBrush(RenderTarget));		
							
							
						RenderTarget.AntialiasMode = oldAntialiasMode;
							
							
						StartPoint	= new Point(MoveOrderCancelRect.Left, MoveOrderCancelRect.Bottom);							
						EndPoint = new Point(MoveOrderCancelRect.Left, MoveOrderCancelRect.Top);								
						
						//RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), Brushes.Black.ToDxBrush(RenderTarget), ThisStroke.Width, ThisStroke.StrokeStyle);
							
						RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ChartBackgroundBrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
							
						StartPoint	= new Point(MoveOrderCancelRect.Right, MoveOrderCancelRect.Bottom);							
						EndPoint = new Point(MoveOrderCancelRect.Right, MoveOrderCancelRect.Top);								
						
						RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ChartBackgroundBrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
					
							
					}
							
						firstone = false;	
						
						
							//RenderTarget.FillRectangle(ExpandRect(ThisRect,3,3), ChartBackgroundMenuFadeBrushDX);
			
						// cancel buttons
												
						//	ThisRect2 = new SharpDX.RectangleF((float)startright-(float)yH-6, (float)yT, (float)yH+2, (float)yH);
					
					// remove cancel button
						ThisRect2 = new SharpDX.RectangleF((float)startright-(float)1, (float)yT, (float)1, (float)yH);
					
							//RenderTarget.FillRectangle(ThisRect2, ChartBackgroundFade2BrushDX);
						
			
							
						// Xsssss
							
						ThisStroke = pOneWidthStroke;
						
									
						oldAntialiasMode = RenderTarget.AntialiasMode;
			            RenderTarget.AntialiasMode = SharpDX.Direct2D1.AntialiasMode.PerPrimitive;

			       
			
							
//						StartPoint	= new Point(ThisRect2.Left, ThisRect2.Top);							
//						EndPoint = new Point(ThisRect2.Right, ThisRect2.Bottom);		
						
						
//						RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ChartBackgroundBrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
						
//						StartPoint	= new Point(ThisRect2.Left, ThisRect2.Bottom);							
//						EndPoint = new Point(ThisRect2.Right, ThisRect2.Top);		
						
						
//						RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ChartBackgroundBrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);

							
						
						CellFormat			= pTextFont2.ToDirectWriteTextFormat();	
						CellFormat			= ButtonText;
				
	                    CellFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
	                    CellFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
	                    CellFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;
						//CellFormat.FontSize = 15;
					
						
						ThisBrushDX = ChartTextBrushDX;
						//RenderTarget.DrawText("X", CellFormat, ThisRect2, ThisBrushDX2);	
//						RenderTarget.DrawText("X", CellFormat, ThisRect2, Brushes.Red.ToDxBrush(RenderTarget));		
							
							
						RenderTarget.AntialiasMode = oldAntialiasMode;
							
							
						StartPoint	= new Point(ThisRect2.Left, ThisRect2.Bottom);							
						EndPoint = new Point(ThisRect2.Left, ThisRect2.Top);								
						
						//RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), Brushes.Black.ToDxBrush(RenderTarget), ThisStroke.Width, ThisStroke.StrokeStyle);
							
						//RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ChartBackgroundBrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
							
						StartPoint	= new Point(ThisRect2.Right, ThisRect2.Bottom);							
						EndPoint = new Point(ThisRect2.Right, ThisRect2.Top);								
						
						//RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ChartBackgroundBrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
							
							
						// end cancel buttons
								
						//	double orderprice = 0;
							
//							if (docombinestopsperprice && or.Name.Contains("Stop"))
//								orderprice = ThisPrice2;
										
							
//							buttonseq = buttonseq + 1;
						
//						    OrderDetails Z = new OrderDetails();
//				            Z.ThisOrder = or;
//							Z.ThisRectA = ThisRect2;
//				            Z.Name = "";
//				            Z.Width = orderprice;
//				            Z.Switch = false;
//				            Z.Rect = new SharpDX.RectangleF(0, 0, 0, 0);
//				            Z.Hovered = false;

//							AllOrderCancelButtons.Add(buttonseq,Z);
		
						// move buttons
												
							//ThisRect2 = new SharpDX.RectangleF((float)startright-(float)yH-(float)yH-20f, (float)yT, (float)yH+1, (float)yH);					
							//RenderTarget.FillRectangle(ThisRect2, ChartBackgroundMenuFadeBrushDX);
						//	buttonseq = buttonseq + 1;
						
//						    Z = new OrderDetails();
//				            Z.ThisOrder = or;
//							Z.ThisRectA = ThisRect2;
//							Z.ThisRectA = ThisRect;
						
//				            Z.Name = "";
//				            Z.Width = orderprice;
//				            Z.Switch = false;
//				            Z.Rect = new SharpDX.RectangleF(0, 0, 0, 0);
//				            Z.Hovered = false;

							
//							if (!OrderIsBelowChart && !OrderIsAboveChart)
//							AllOrderMoveButtons.Add(buttonseq,Z);
					
						// end
						
						// text on top of order
						
						sidepadding = 5;	
							
						lefttt = startright - ThisRect2.Left + sidepadding + 1;
						
							
							
							if (docombinestopsperprice && or.Name.Contains("Stop"))
								CellString = "Stop Loss";
							
							
							
							CellFormat			= pTextFont2.ToDirectWriteTextFormat();	
							CellFormat			= ButtonText;
					
		                    CellFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Trailing;
		                    CellFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
		                    CellFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;

						
						
						//ThisBrushDX = ChartTextBrushDX;
							
						
						RenderTarget.DrawText(CellString, CellFormat, ExpandRect(ThisRect,0,lefttt*-1,0,0), ThisBrushDX2);	
					
					
							// quanitity text
				
						CellString = or.Quantity.ToString();
						if (docombinestopsperprice && or.Name.Contains ("Stop"))
							CellString = AllStopLossPrices[ThisPrice2].ToString();							
							
						CellFormat			= pTextFont2.ToDirectWriteTextFormat();	
						CellFormat			= ButtonText;
					
		                    CellFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Leading;
		                    CellFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
		                    CellFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;

						
						
						//ThisBrushDX = ChartTextBrushDX;
							
						
						RenderTarget.DrawText(CellString, CellFormat, ExpandRect(ThisRect,sidepadding*-1,0,0,0), ThisBrushDX2);							
							
							
				
						if (!AllOrderPrices.ContainsKey(ThisPrice))
							AllOrderPrices.Add(ThisPrice, startright - orderdwidth - orderhspace);
						else
						{
							startright =  AllOrderPrices[ThisPrice] - orderdwidth - orderhspace;
							AllOrderPrices[ThisPrice] = startright;
						}
						
						
					}
					
				}
				
				
			ChartBackgroundFade2BrushDX.Dispose();			
			
			
			
			
//				y1 = chartScale.GetYByValue(CurrentLast);

//				double yA = GetBoxYPixel(CurrentLast,"Top");
//				double yB = GetBoxYPixel(CurrentLast,"Bottom");
//				double yH = GetBoxYPixel(CurrentLast,"Height");
//				double yT = yA;		
				

//				ThisRect = new SharpDX.RectangleF((float)0, (float)yT, (float)ChartPanel.W, (float)yH);
			
//				// change colors
									
//				ThisBrushDX = LastPriceBrushDX;
//				RenderTarget.FillRectangle(ThisRect, ThisBrushDX);
//				RenderTarget.FillRectangle(ExpandRect(ThisRect,3,3), ChartBackgroundMenuFadeBrushDX);


	
//				CellFormat			= pTextFont2.ToDirectWriteTextFormat();	
//				CellFormat			= ButtonText;
		
//                CellFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Trailing;
//                CellFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
//                CellFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;
			

		
//				Order or = MovingOrder;
		
		
		
		
		
		
//				string dire = string.Empty;
//				string typee = "STP";
		
				
			
//				if (or.Name.Contains("Target"))
//				{
					
//					typee = or.Name;
//				}
//				else if (or.Name.Contains("Stop"))
//				{
					
//					typee = or.Name;
//				}					

				
//				else if (or.OrderType == OrderType.Limit)
//				{
					
//					typee = "LMT";
//				}
//				else if (or.OrderType == OrderType.MIT)
//				{
					
//					typee = "MIT";
//				}
//				else if (or.OrderType == OrderType.StopLimit)
//				{
					
//					typee = "SLM";
//				}
//				else if (or.OrderType == OrderType.StopMarket)
//				{
					
//					typee = "STP";
//				}							

				
//				if (or.OrderAction == OrderAction.Sell || or.OrderAction == OrderAction.SellShort)
//				{
					
//					dire = "Sell";	
//				}
//				else
//				{
//					dire = "Buy";	
//				}
				
					
				
//				if (or.Name.Contains ("Stop") || or.Name.Contains ("Target"))
//				{
					
//					dire = "";
					
//				}
					
			
//				string dirrec = "";

//				if (dire != "")
//					dirrec = dirrec + " " + dire;
				
//				if (typee != "")
//					dirrec = dirrec + " " + typee;						
			
				
//              //  CellString = cancelmessage + dirrec + " " + qudSelector.Value + " " + ClickText;
			
//				dirrec = dirrec.Replace("Target", "Target ");
//				dirrec = dirrec.Replace("Stop", "Stop ");
				
				
				
//				dirrec = "(" + or.Quantity.ToString() + ")" + " " + dirrec;
//					CellString = "MOVING " + dirrec;
			
			
		
//		if (MovingOrders.Count > 0)
//			CellString = "MOVING MULTIPLE ORDERS";
					
					
		
//			ThisBrushDX = ChartTextBrushDX;
//			RenderTarget.DrawText(CellString, CellFormat, ExpandRect(ThisRect,-10,0), ThisBrushDX);	
		
		
		}
		
				
		private void TotalTheOrders()
		{
			
			// count total orders for drawing later
			
						AllStopLossPrices.Clear();
			
			double ThisPrice = 0;
			bool docombinestopsperprice = !pSplitStopDisplay;
			
			
			
				foreach (Order or in myAccount.Orders.ToList())
				{
					
					
							
					ThisPrice = or.StopPrice;
					
					int thisq = or.Quantity;
					
						OrderStateOK = or.OrderState == OrderState.Working || or.OrderState == OrderState.Accepted || or.OrderState == OrderState.ChangePending  || or.OrderState == OrderState.ChangeSubmitted || or.OrderState == OrderState.CancelPending || or.OrderState == OrderState.CancelSubmitted;
						OrderInstrumentOK = or.Instrument.FullName == Instrument.FullName;
						OrderNameOK = true;
						OrderTypeOK = true;		
						
					
					
					if (OrderTypeOK && OrderStateOK && OrderNameOK && OrderInstrumentOK)						
					if (or.Name.Contains ("Stop"))
					{
						

						
						if (!AllStopLossPrices.ContainsKey(ThisPrice))
							AllStopLossPrices.Add(ThisPrice, thisq);
						else
						{
							int newq =  AllStopLossPrices[ThisPrice] +  thisq;
							AllStopLossPrices[ThisPrice] = newq;
						}
						
							
					}
						
					
				
					
				}
				
			TotalOrdersPrices.Clear();
			
			FoundStopLossPrices.Clear();
			
		

						
						
				foreach (Order or in myAccount.Orders.ToList())
				{
					
					
						
					ThisPrice = or.StopPrice;
					
					
					
					
					if (or.OrderType == OrderType.Limit)
						{
							ThisPrice = or.LimitPrice;
						
						}
						
						int thisq = 1;
					
						OrderStateOK = or.OrderState == OrderState.Working || or.OrderState == OrderState.Accepted || or.OrderState == OrderState.ChangePending  || or.OrderState == OrderState.ChangeSubmitted || or.OrderState == OrderState.CancelPending || or.OrderState == OrderState.CancelSubmitted;
						OrderInstrumentOK = or.Instrument.FullName == Instrument.FullName;
						OrderNameOK = true;
						OrderTypeOK = true;		
						
						
					if (OrderTypeOK && OrderStateOK && OrderNameOK && OrderInstrumentOK)						
					//if (or.Name.Contains ("Stop"))
					{
						
						
						
						if (docombinestopsperprice && or.Name.Contains ("Stop"))
						{
							
							if (FoundStopLossPrices.ContainsKey(ThisPrice))
							{
								
								continue;	
								
							}
							
						
							if (!FoundStopLossPrices.ContainsKey(ThisPrice))
								FoundStopLossPrices.Add(ThisPrice, 1);
						
							
														
						}

						if (!TotalOrdersPrices.ContainsKey(ThisPrice))
							TotalOrdersPrices.Add(ThisPrice, thisq);
						else
						{
							int newq =  TotalOrdersPrices[ThisPrice] +  thisq;
							TotalOrdersPrices[ThisPrice] = newq;
						}
						
						if (TotalOrdersPrices[ThisPrice] > 1)
						{
							
							//Print(TotalOrdersPrices[ThisPrice]);
							
//							if (!AllOrderPrices.ContainsKey(ThisPrice))
//								AllOrderPrices.Add(ThisPrice, startright - 30);
						
						}
						
							
					}
						
				
			
					
				}

		}
				
			
	
		private void DrawOrders (ChartControl chartControl, ChartScale chartScale, SharpDX.Direct2D1.Brush LastPriceBrushDX, double CurrentLast)
		{			
			
            SharpDX.DirectWrite.TextFormat ButtonText = pTextFont66.ToDirectWriteTextFormat();
			//ButtonText = myProperties.LabelFont.ToDirectWriteTextFormat();
			
			
			
			ChartBackgroundFade2BrushDX = ChartControl.Properties.ChartBackground.ToDxBrush(RenderTarget);
			ChartBackgroundFade2BrushDX.Opacity = pOrderFlagButtonOpacity/100f;
			
			
				AllOrderCancelButtons.Clear();
				AllOrderMoveButtons.Clear();
				AllStopCombinationButtons.Clear();
				AllOrderPrices.Clear();
			
			
				int buttonseq = 0;
			
			
			
			
			
			
				bool docombinestopsperprice = !pSplitStopDisplay;
			
			
						float startright = ChartPanel.W - maininitialspace;
						float orderdwidth = pOrderDisplayWidth; // order display width	
			
						
			CellFormat			= pTextFont2.ToDirectWriteTextFormat();	
				
				TextLayout CellTextLayout3333 = new TextLayout(Core.Globals.DirectWriteFactory, "Stop Loss 100", CellFormat, 10000, 10000);
				
				orderdwidth = (int) CellTextLayout3333.Metrics.Width + (int) Math.Ceiling(CellTextLayout3333.Metrics.Width/2);
				
				CellTextLayout3333.Dispose();

			
			
			//float orderdwidth2 = 200; // position display width
			float orderdwidth2 = orderdwidth*1.5f; // position display width
			
			
			
					double yA = 0;
							double yB = 0;
							double yH = 0;
							double yT = yA;		
							
							
			float lefttt = 0;
			
			double ThisPrice = 0;
			
	
			TotalTheOrders();
				
				
			if (FirstRender2)
			{
				
				FirstRender2 = false;
				
				SetOrderFlags();
			}
				
//				if (EnableOrderExecution && connected && pOrderPanelOn)
//					TotalTheOrders();
				
				
			// show position details on the chart window.....
			
			
//					ChartControl.Dispatcher.InvokeAsync(() =>
//					{
						
//						Values[19][0] = 0;
//					});		
						
				
			UpdateQTYBox (ThisPositionNow());
			UpdatePNLBox(ThisPositionNow());
				
				
			if (ThisPositionNow() != null)
			{
				
				
				startright = ChartPanel.W - maininitialspace;
				
				ThisPrice = RTTS(ThisPositionNow().AveragePrice);
				
		
//					ChartControl.Dispatcher.InvokeAsync(() =>
//					{
						
//						Values[19][0] = ThisPrice;
//					});		
					
					
				
				
//				Values[19][1] = ThisPrice;
				
				//Print(Values[19][0]);
				
				
				int thisqqq = ThisPositionNow().Quantity;
					
				if (AllOrderPrices.ContainsKey(ThisPrice))
					startright =  AllOrderPrices[ThisPrice];	
					

							
							y1 = chartScale.GetYByValue(ThisPrice);

							yA = GetBoxYPixel(ThisPrice,"Top");
							yB = GetBoxYPixel(ThisPrice,"Bottom");
							yH = GetBoxYPixel(ThisPrice,"Height");
							yT = yA;		
							
				
						if (yH < OrderDisplayMinHeight)
						{
							double diff = OrderDisplayMinHeight - yH;
							yH = Math.Max(OrderDisplayMinHeight, yH);
							
							yT = yT - diff/2;
						}
													
			
						ThisRect = new SharpDX.RectangleF((float)startright - orderdwidth2, (float)yT, orderdwidth2, (float)yH);
						
						
						
						
							
							
						if (pMakeOrderOutlineNotSeeThru) RenderTarget.FillRectangle(ExpandRect(ThisRect,3,3), ChartBackgroundBrushDX);
						
							ThisBrushDX = ChartControl.Properties.ChartText.ToDxBrush(RenderTarget);
							ThisBrushDX.Opacity = (float) pOrderOutlineOpacity/100f;
							
							RenderTarget.FillRectangle(ExpandRect(ThisRect,3,3), ThisBrushDX);
			
									
							RenderTarget.FillRectangle(ExpandRect(ThisRect,1,1), ChartBackgroundBrushDX);
						
						
						
							
							// change colors
												
							if (ThisPositionNow().MarketPosition == MarketPosition.Long)
							{
								ThisBrushDX = Brushes.Lime.ToDxBrush(RenderTarget);
								ThisBrushDX2 = GetTextColor(Brushes.Lime).ToDxBrush(RenderTarget);
							}
							else
							{		
								ThisBrushDX = Brushes.Red.ToDxBrush(RenderTarget);
								ThisBrushDX2 = GetTextColor(Brushes.Red).ToDxBrush(RenderTarget);
							}
					
							
							
							
							RenderTarget.FillRectangle(ThisRect, ThisBrushDX);
						
							
	// cancel buttons 1
			
						ThisRect6 = new SharpDX.RectangleF((float)startright-4, (float)yT, (float)0, (float)yH);	
						
							
							
						if (pShowCloseButton)
						{											
							ThisRect6 = new SharpDX.RectangleF((float)startright-(float)yH-6, (float)yT, (float)yH+2, (float)yH);
						
							RenderTarget.FillRectangle(ThisRect6, ChartBackgroundFade2BrushDX);
						
				
								
							// Xsssss
								
							ThisStroke = pOneWidthStroke;
		
							oldAntialiasMode = RenderTarget.AntialiasMode;
				            RenderTarget.AntialiasMode = SharpDX.Direct2D1.AntialiasMode.PerPrimitive;

							CellFormat			= pTextFont2.ToDirectWriteTextFormat();	
							CellFormat			= ButtonText;
					
		                    CellFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
		                    CellFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
		                    CellFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;
							//CellFormat.FontSize = 15;
						
							
							//ThisBrushDX = ChartTextBrushDX;
							RenderTarget.DrawText("X", CellFormat, ThisRect6, ThisBrushDX2);	
	//						RenderTarget.DrawText("X", CellFormat, ThisRect6, Brushes.Red.ToDxBrush(RenderTarget));		
								
								
							RenderTarget.AntialiasMode = oldAntialiasMode;
								
								
							StartPoint	= new Point(ThisRect6.Left, ThisRect6.Bottom);							
							EndPoint = new Point(ThisRect6.Left, ThisRect6.Top);		
							
							
							RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ChartBackgroundBrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
						}	
						
						if (pShowCloseButton || pShowBEButton)
						{
							ThisStroke = pOneWidthStroke;
							
							StartPoint	= new Point(ThisRect6.Right, ThisRect6.Bottom);							
							EndPoint = new Point(ThisRect6.Right, ThisRect6.Top);		
							
							
							RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ChartBackgroundBrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
						}
						
	// BE buttons 1
							
						if (pShowBEButton)
						{
							float expandit = 8; // how much to expand BE button
													
								ThisRect4 = new SharpDX.RectangleF(ThisRect6.Left-(float)yH-expandit, (float)yT, (float)yH+expandit, (float)yH);
							
								RenderTarget.FillRectangle(ThisRect4, ChartBackgroundFade2BrushDX);
							//RenderTarget.DrawRectangle(ThisRect4,ChartBackgroundBrushDX);
				
								
							// Xsssss
								
							ThisStroke = pOneWidthStroke;
		
							oldAntialiasMode = RenderTarget.AntialiasMode;
				            RenderTarget.AntialiasMode = SharpDX.Direct2D1.AntialiasMode.PerPrimitive;

							CellFormat			= pTextFont2.ToDirectWriteTextFormat();	
							CellFormat			= ButtonText;
					
		                    CellFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
		                    CellFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
		                    CellFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;
							//CellFormat.FontSize = 15;
						
							
							//ThisBrushDX = ChartTextBrushDX;
							RenderTarget.DrawText("BE", CellFormat, ThisRect4, ThisBrushDX2);	
	//						RenderTarget.DrawText("X", CellFormat, ThisRect6, Brushes.Red.ToDxBrush(RenderTarget));		
								
								
							RenderTarget.AntialiasMode = oldAntialiasMode;
								
								
							StartPoint	= new Point(ThisRect4.Left, ThisRect4.Bottom);							
							EndPoint = new Point(ThisRect4.Left, ThisRect4.Top);		
							
							
							RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ChartBackgroundBrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
							
						}
// end							
							
				
							
	// area for pnl display'
									CellFormat			= pTextFont2.ToDirectWriteTextFormat();	
				
				TextLayout CellTextLayout33333 = new TextLayout(Core.Globals.DirectWriteFactory, "($000000.00)", CellFormat, 10000, 10000);
				
				float plnwidth = (int) CellTextLayout33333.Metrics.Width + (int) Math.Ceiling(CellTextLayout33333.Metrics.Width/6);
				
				CellTextLayout33333.Dispose();

							
							ThisRect3 = new SharpDX.RectangleF(ThisRect.Left, (float)yT, (float)plnwidth, (float)yH);
						
							RenderTarget.FillRectangle(ThisRect3, Brushes.Black.ToDxBrush(RenderTarget));
						
						
						
						
							
						oldAntialiasMode = RenderTarget.AntialiasMode;
			            RenderTarget.AntialiasMode = SharpDX.Direct2D1.AntialiasMode.PerPrimitive;

						CellFormat			= pTextFont2.ToDirectWriteTextFormat();	
						CellFormat			= ButtonText;
				
	                    CellFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
	                    CellFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
	                    CellFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;
						//CellFormat.FontSize = 15;
					
						CellString = GetDollarString("$", SentToChartPNL);
						
							
						double pointsnow = 0;	
						double ticksnow = 0;
						
							
						
						//Print(CurrentLast);
							
							
							
//						if (ThisPositionNow().MarketPosition == MarketPosition.Long)
//						{
							
//							pointsnow = CurrentLast - ThisPrice;
							
//						}
//						else
//						{
							
//							pointsnow = ThisPrice - CurrentLast;
//						}
							
//						ticksnow = Math.Round(pointsnow / tSize);
						
						
							if (pProfitAndLossType == "Points")
							{
								CellString = AllPriceMarker(SentToChartPNL);
								
							}						
							if (pProfitAndLossType == "Ticks")
							{
								CellString = Math.Round(SentToChartPNL, 0).ToString();
								
							}
							
							if (SentToChartPNL > 0)
							{
								ThisBrushDX = Brushes.LimeGreen.ToDxBrush(RenderTarget);
								
							}
							else
							{
								ThisBrushDX = Brushes.Red.ToDxBrush(RenderTarget);
								
							}
							
						//ThisBrushDX = ChartTextBrushDX;
						RenderTarget.DrawText(CellString, CellFormat, ThisRect3, ThisBrushDX);	
//						RenderTarget.DrawText("X", CellFormat, ThisRect6, Brushes.Red.ToDxBrush(RenderTarget));		
							
							
						RenderTarget.AntialiasMode = oldAntialiasMode;
							
	
// area for quantity display
							
							
							
									
							ThisRect5 = new SharpDX.RectangleF(ThisRect3.Right, (float)yT, ThisRect4.Left - ThisRect3.Right, (float)yH);
						
							//RenderTarget.FillRectangle(ThisRect3, Brushes.Black.ToDxBrush(RenderTarget));
							
							
						oldAntialiasMode = RenderTarget.AntialiasMode;
			            RenderTarget.AntialiasMode = SharpDX.Direct2D1.AntialiasMode.PerPrimitive;

						CellFormat			= pTextFont2.ToDirectWriteTextFormat();	
						CellFormat			= ButtonText;
				
	                    CellFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
	                    CellFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
	                    CellFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;
						//CellFormat.FontSize = 15;
					
					
							
						CellString = thisqqq.ToString();
				

	
						//RenderTarget.DrawText(CellString, CellFormat, ExpandRect(ThisRect,0,lefttt*-1,0,0), ThisBrushDX);	
						RenderTarget.DrawText(CellString, CellFormat, ThisRect5, ThisBrushDX2);	
							
							
							
//							if (SentToChartPNL > 0)
//							{
//								ThisBrushDX = Brushes.LimeGreen.ToDxBrush(RenderTarget);
								
//							}
//							else
//							{
//								ThisBrushDX = Brushes.Red.ToDxBrush(RenderTarget);
								
//							}
						
//							CellFormat			= pTextFont2.ToDirectWriteTextFormat();	
						
					
//		                    CellFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Trailing;
//		                    CellFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
//		                    CellFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;
						
						
					
							
							
//						//ThisBrushDX = ChartTextBrushDX;
//						RenderTarget.DrawText(CellString, CellFormat, ThisRect5, ThisBrushDX);	
//						RenderTarget.DrawText("X", CellFormat, ThisRect6, Brushes.Red.ToDxBrush(RenderTarget));		
							
							
						RenderTarget.AntialiasMode = oldAntialiasMode;
							
							
							
					

						
						
						if (!AllOrderPrices.ContainsKey(ThisPrice))
							AllOrderPrices.Add(ThisPrice, startright - orderdwidth2 - 9);
						else
						{
							startright =  AllOrderPrices[ThisPrice] - orderdwidth2 - 9;
							AllOrderPrices[ThisPrice] = startright;
						}
								
			
			
			}
			else
			{
			
				
				UpdateQTYBox(null);
				UpdatePNLBox(null);
			
				
			}
			
			

	// all orders buttons
				
				int changeallbuttonswidth = pSummaryColumnWidth;
							
			
			
			
				foreach (KeyValuePair<double, int> OrdersAtPrice in TotalOrdersPrices)
				{
				
				
					
					if (OrdersAtPrice.Value > 1|| pShowOrderSummary)
					
					{
							
						
						ThisPrice = OrdersAtPrice.Key;
						double ThisPrice2 = ThisPrice;;	
						
						double HighPrice = ThisMasterInstrument.RoundDownToTickSize(chartScale.MaxValue - tSize);
						double LowPrice = ThisMasterInstrument.RoundToTickSize(chartScale.MinValue + tSize);
						
						bool OrderIsAboveChart = false;
						bool OrderIsBelowChart = false;
						
											
						if (pShowOrdersOffChart)
						{
							
							
							if (ThisPrice > HighPrice)
							{
								ThisPrice = 1000000;
								OrderIsAboveChart = true;
							}
							
							if (ThisPrice < LowPrice)
							{
								ThisPrice = 1000001;
								OrderIsBelowChart = true;
							}
							
						}
					
						if (OrderIsBelowChart || OrderIsAboveChart)
						{
							
							continue;
						
						}
						
						
						
						startright = ChartPanel.W - maininitialspace;
						
						
						if (AllOrderPrices.ContainsKey(ThisPrice))
							startright = AllOrderPrices[ThisPrice];
						
						
					
							
							y1 = chartScale.GetYByValue(ThisPrice);

							yA = GetBoxYPixel(ThisPrice,"Top");
							yB = GetBoxYPixel(ThisPrice,"Bottom");
							yH = GetBoxYPixel(ThisPrice,"Height");
							yT = yA;		
							
							
			
						//	ThisRect = new SharpDX.RectangleF((float)0, (float)yT, (float)startright, (float)yH);
						
						if (yH < OrderDisplayMinHeight)
						{
							double diff = OrderDisplayMinHeight - yH;
							yH = Math.Max(OrderDisplayMinHeight, yH);
							
							yT = yT - diff/2;
						}
						

						
						CellString = "   ";
						
						if (pShowOrderSummary)
								if (pThisSummaryTypeMode == "Ticks")
								{
									if (ThisPositionNow() != null)
									{
										double tickstoentry = Math.Round(Math.Abs(RTTS(ThisPositionNow().AveragePrice) - ThisPrice)/tSize, 0);
										
										CellString = tickstoentry.ToString();
										
									}
									else
									{
										double tickstolasttrade = Math.Round(Math.Abs(CurrentLastData - ThisPrice)/tSize, 0);
										CellString = tickstolasttrade.ToString();
										
									}
								}
								else
								{
									
									
									
								}
									
								
									
								CellFormat			= ButtonText;
							
				                    CellFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Leading;
				                    CellFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
				                    CellFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;

		
						if (CellString.Length <= 1)
							CellString = "00";
						
						TextLayout CellTextLayout = new TextLayout(Core.Globals.DirectWriteFactory, CellString, CellFormat, 10000, 10000);
						
					
						int addt = (int) yH + 2 + 6;
						
						
						if (OrdersAtPrice.Value <= 1)
							addt = 0;
						
						int textsize = (int) CellTextLayout.Metrics.Width + 10;
						CellTextLayout.Dispose();

						int minimumspacetoclick = (int) yH + 2; // 20
						
						textsize = Math.Max(textsize, minimumspacetoclick);
						
						changeallbuttonswidth = textsize + addt;
								
						
						ThisRect = new SharpDX.RectangleF((float)startright - changeallbuttonswidth, (float)yT, changeallbuttonswidth, (float)yH);
						
						
							// change colors
												
							//ThisBrushDX = LastPriceBrushDX;
						
				
							if (pMakeOrderOutlineNotSeeThru) RenderTarget.FillRectangle(ExpandRect(ThisRect,3,3), ChartBackgroundBrushDX);
						
							ThisBrushDX = ChartControl.Properties.ChartText.ToDxBrush(RenderTarget);
							ThisBrushDX.Opacity = (float) pOrderOutlineOpacity/100f;
							
							RenderTarget.FillRectangle(ExpandRect(ThisRect,3,3), ThisBrushDX);
						
						
						
						
						
							RenderTarget.FillRectangle(ExpandRect(ThisRect,1,1), ChartBackgroundBrushDX);
						
						
						
						
					
						
						
						
						
//							if (or.Name.Contains("Target"))
//							{
//								ThisBrushDX = pOrderTargetColor.ToDxBrush(RenderTarget);
//								ThisBrushDX2 = GetTextColor(pOrderTargetColor).ToDxBrush(RenderTarget);
								
//								typee = or.Name;
//							}
//							else if (or.Name.Contains("Stop"))
//							{
//								ThisBrushDX = pOrderStopColor.ToDxBrush(RenderTarget);
//								ThisBrushDX2 = GetTextColor(pOrderStopColor).ToDxBrush(RenderTarget);
							
//								typee = or.Name;
//							}					
		
							
//							else if (or.OrderType == OrderType.Limit)
//							{
//								ThisBrushDX = pOrderLimitColor.ToDxBrush(RenderTarget);
//								ThisBrushDX2 = GetTextColor(pOrderLimitColor).ToDxBrush(RenderTarget);
								
//								typee = "LMT";
//							}
//							else if (or.OrderType == OrderType.MIT)
//							{
//								ThisBrushDX = pOrderMITColor.ToDxBrush(RenderTarget);
//								ThisBrushDX2 = GetTextColor(pOrderMITColor).ToDxBrush(RenderTarget);
								
//								typee = "MIT";
//							}
//							else if (or.OrderType == OrderType.StopLimit)
//							{
								ThisBrushDX = pOrderSummaryColor.ToDxBrush(RenderTarget);
								ThisBrushDX2 = GetTextColor(pOrderSummaryColor).ToDxBrush(RenderTarget);
								
//								typee = "SLM";
//							}
//							else if (or.OrderType == OrderType.StopMarket)
//							{
//								ThisBrushDX = pOrderStopMarketColor.ToDxBrush(RenderTarget);
//								ThisBrushDX2 = GetTextColor(pOrderStopMarketColor).ToDxBrush(RenderTarget);
								
//								typee = "STP";
//							}							
							
//							else if (or.OrderAction == OrderAction.Sell || or.OrderAction == OrderAction.SellShort)
//							{
//								ThisBrushDX = pClickDNColor.ToDxBrush(RenderTarget);
							
//							}
//							else
//							{
//								ThisBrushDX = pClickUPColor.ToDxBrush(RenderTarget);
							
							
								
//							}
							
							
//							if (or.OrderAction == OrderAction.Sell || or.OrderAction == OrderAction.SellShort)
//							{
								
//								dire = "Sell";	
//							}
//							else
//							{
//								dire = "Buy";	
//							}
							
								
							
//							if (or.Name.Contains ("Stop") || or.Name.Contains ("Target"))
//							{
								
//								dire = "";
								
//							}
								
						
//							string dirrec = "";

//							if (dire != "")
//								dirrec = dirrec + " " + dire;
							
//							if (typee != "")
//								dirrec = dirrec + " " + typee;						
						
							
//		                  //  CellString = cancelmessage + dirrec + " " + qudSelector.Value + " " + ClickText;
						
//							dirrec = dirrec.Replace("Target", "Target ");
//							dirrec = dirrec.Replace("Stop", "Stop ");
							
							
//							CellString = dirrec;
						
						
						OrderDetails Z = new OrderDetails();
						Order or = new Order();
						double orderprice = ThisPrice2;
						

							
							
							
							CellString = string.Empty;
						
							
						
							
							RenderTarget.FillRectangle(ThisRect, ThisBrushDX);
						
				
							//RenderTarget.FillRectangle(ExpandRect(ThisRect,3,3), ChartBackgroundMenuFadeBrushDX);
			
						// cancel buttons
												
							ThisRect2 = new SharpDX.RectangleF((float)startright-(float)yH-6, (float)yT, (float)yH+2, (float)yH);
						
						//if (OrdersAtPrice.Value > 1)
						{							
							if (OrdersAtPrice.Value > 1) RenderTarget.FillRectangle(ThisRect2, ChartBackgroundFade2BrushDX);
						
				
								
							// Xsssss
								
							ThisStroke = pOneWidthStroke;
							
										
							oldAntialiasMode = RenderTarget.AntialiasMode;
				            RenderTarget.AntialiasMode = SharpDX.Direct2D1.AntialiasMode.PerPrimitive;

				       
				
								
	//						StartPoint	= new Point(ThisRect2.Left, ThisRect2.Top);							
	//						EndPoint = new Point(ThisRect2.Right, ThisRect2.Bottom);		
							
							
	//						RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ChartBackgroundBrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
							
	//						StartPoint	= new Point(ThisRect2.Left, ThisRect2.Bottom);							
	//						EndPoint = new Point(ThisRect2.Right, ThisRect2.Top);		
							
							
	//						RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ChartBackgroundBrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);

							

						
							CellFormat			= pTextFont2.ToDirectWriteTextFormat();	
							CellFormat			= ButtonText;
					
		                    CellFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
		                    CellFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
		                    CellFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;
							//CellFormat.FontSize = 15;
						
							
							ThisBrushDX = ChartTextBrushDX;
							if (OrdersAtPrice.Value > 1) RenderTarget.DrawText("X", CellFormat, ThisRect2, ThisBrushDX2);	
	//						RenderTarget.DrawText("X", CellFormat, ThisRect2, Brushes.Red.ToDxBrush(RenderTarget));		
								
								
							RenderTarget.AntialiasMode = oldAntialiasMode;
								
								
							StartPoint	= new Point(ThisRect2.Left, ThisRect2.Bottom);							
							EndPoint = new Point(ThisRect2.Left, ThisRect2.Top);								
							
							if (OrdersAtPrice.Value > 1) RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ChartBackgroundBrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
										
							StartPoint	= new Point(ThisRect2.Right, ThisRect2.Bottom);							
							EndPoint = new Point(ThisRect2.Right, ThisRect2.Top);								
							
							if (OrdersAtPrice.Value > 1) RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ChartBackgroundBrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
								
								
							// end cancel buttons
									
								
								
							
						
							//if (docombinestopsperprice && or.Name.Contains("Stop"))
								orderprice = ThisPrice2;
										
							
							buttonseq = buttonseq + 1;
						
						    Z = new OrderDetails();
				            Z.ThisOrder = or;
							Z.ThisRectA = ThisRect2;
				            Z.Name = "";
				            Z.Width = orderprice;
				            Z.Switch = true;
				            Z.Rect = new SharpDX.RectangleF(0, 0, 0, 0);
				            Z.Hovered = false;

							if (OrdersAtPrice.Value > 1) AllOrderCancelButtons.Add(buttonseq,Z);
		
						}
						
						// move buttons
												
							//ThisRect2 = new SharpDX.RectangleF((float)startright-(float)yH-(float)yH-20f, (float)yT, (float)yH+1, (float)yH);					
							//RenderTarget.FillRectangle(ThisRect2, ChartBackgroundMenuFadeBrushDX);
						//	buttonseq = buttonseq + 1;
						
						    Z = new OrderDetails();
				            Z.ThisOrder = or;
							Z.ThisRectA = ThisRect2;
							Z.ThisRectA = ThisRect;
						
				            Z.Name = "";
				            Z.Width = orderprice;
				            Z.Switch = true;
				            Z.Rect = new SharpDX.RectangleF(0, 0, 0, 0);
				            Z.Hovered = false;

						
						if (OrdersAtPrice.Value > 1) 
							AllOrderMoveButtons.Add(buttonseq,Z);
					
						// end
						
						// text on top of order
						
						float sidepadding = 5;	
							
							lefttt = startright - ThisRect2.Left + sidepadding + 1;
						
							
							
//							if (docombinestopsperprice && or.Name.Contains("Stop"))
//								CellString = "Stop Loss";
							
							
							
					
					
							CellFormat			= pTextFont2.ToDirectWriteTextFormat();	
							CellFormat			= ButtonText;
					
		                    CellFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Trailing;
		                    CellFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
		                    CellFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;

						
						//TextLayout CellTextLayout = new TextLayout(Core.Globals.DirectWriteFactory, CellString, CellFormat, 10000, 10000);
						//ThisBrushDX = ChartTextBrushDX;
							
						
					//	RenderTarget.DrawText(CellString, CellFormat, ExpandRect(ThisRect,0,lefttt*-1,0,0), ThisBrushDX2);	
					
					
							// quanitity text
				
						//CellString = or.Quantity.ToString();
						//if (docombinestopsperprice && or.Name.Contains ("Stop"))
						//	CellString = AllStopLossPrices[ThisPrice].ToString();							
							
							
							if (pShowOrderSummary)
							{
								if (pThisSummaryTypeMode == "Ticks")
								{
									if (ThisPositionNow() != null)
									{
										double tickstoentry = Math.Round(Math.Abs(RTTS(ThisPositionNow().AveragePrice) - ThisPrice)/tSize, 0);
										
										CellString = tickstoentry.ToString();
										
									}
									else
									{
										double tickstolasttrade = Math.Round(Math.Abs(CurrentLastData - ThisPrice)/tSize, 0);
										CellString = tickstolasttrade.ToString();
										
									}
								}
								else
								{
									
									
									
								}
									
								
									
								CellFormat			= ButtonText;
							
				                    CellFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Leading;
				                    CellFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
				                    CellFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;

								
								
								//ThisBrushDX = ChartTextBrushDX;
									
								
								RenderTarget.DrawText(CellString, CellFormat, ExpandRect(ThisRect,sidepadding*-1,0,0,0), ThisBrushDX2);							
									
									
							}
//						if (!AllOrderPrices.ContainsKey(ThisPrice))
//							AllOrderPrices.Add(ThisPrice, startright - orderdwidth - orderhspace);
//						else
//						{
//							startright =  AllOrderPrices[ThisPrice] - orderdwidth - orderhspace;
//							AllOrderPrices[ThisPrice] = startright;
//						}
						
						
//						if (!AllOrderPrices.ContainsKey(ThisPrice))
//								AllOrderPrices.Add(ThisPrice, startright - changeallbuttonswidth - 9);
						
						
						if (!AllOrderPrices.ContainsKey(ThisPrice))
							AllOrderPrices.Add(ThisPrice, startright - changeallbuttonswidth - 9);
						else
						{
							startright =  AllOrderPrices[ThisPrice] - changeallbuttonswidth - 9;
							AllOrderPrices[ThisPrice] = startright;
						}
						
						
//							Al
						
					}
					

								
					
				}
				
				
				
				
				
				
			FoundStopLossPrices.Clear();
			
		
			
				foreach (Order or in myAccount.Orders.ToList())
				{
					
					NewStopPrice = 0;
					
					//TrackedOrders(
					
					
							OrderStateOK = or.OrderState == OrderState.Working || or.OrderState == OrderState.Accepted || or.OrderState == OrderState.ChangePending  || or.OrderState == OrderState.ChangeSubmitted || or.OrderState == OrderState.CancelPending || or.OrderState == OrderState.CancelSubmitted;
							//OrderTypeOK = or.OrderType == OrderType.StopLimit || or.OrderType == OrderType.StopMarket;
							//OrderNameOK = or.Name == "Stop1" || or.Name == "Stop2" ||  or.Name == "Stop3";
							//OrderNameOK = or.Oco.Contains("LE1") || or.Oco.Contains("LE2") || or.Oco.Contains("SE1") || or.Oco.Contains("SE2");
				
//							if (whichone == "1")
//								OrderNameOK = or.Oco.Contains("LE1") || or.Oco.Contains("SE1");
//							if (whichone == "2")
//								OrderNameOK = or.Oco.Contains("LE2") || or.Oco.Contains("SE2");				
					
							OrderInstrumentOK = or.Instrument.FullName == Instrument.FullName;
							OrderNameOK = true;
							OrderTypeOK = true;
					//OrderStateOK = true;
					
					//if (OrderStateOK)
					//Print(or.OrderId);
					
					if (OrderTypeOK && OrderStateOK && OrderNameOK && OrderInstrumentOK)
					{
							
						ThisPrice = or.StopPrice;
						
						if (or.OrderType == OrderType.Limit)
						{
							ThisPrice = or.LimitPrice;
							
						}
						

												
						double ThisPrice2 = ThisPrice;
					
						
						

						
						
						
//						Print(chartScale.MaxValue);
//						Print(MaxValue);
						
						double HighPrice = ThisMasterInstrument.RoundDownToTickSize(chartScale.MaxValue - tSize);
						double LowPrice = ThisMasterInstrument.RoundToTickSize(chartScale.MinValue + tSize);
						
						bool OrderIsAboveChart = false;
						bool OrderIsBelowChart = false;
						
						
						
						{
							
							if (FoundStopLossPrices.ContainsKey(ThisPrice))
							{
								int sfsfsfd = FoundStopLossPrices[ThisPrice];
								
								
								FoundStopLossPrices[ThisPrice] = sfsfsfd + 1;
								
								
								if (docombinestopsperprice && or.Name.Contains ("Stop"))
								continue;	
								
							}
							
							
							if (!FoundStopLossPrices.ContainsKey(ThisPrice))
								FoundStopLossPrices.Add(ThisPrice, 1);
							
							
														
						}
											
						if (pShowOrdersOffChart)
						{
							
							
							if (ThisPrice > HighPrice)
							{
								ThisPrice = 1000000;
								OrderIsAboveChart = true;
							}
							
							if (ThisPrice < LowPrice)
							{
								ThisPrice = 1000001;
								OrderIsBelowChart = true;
							}
							
						}
						
						
							

						
					
						int allopacity = 100;

						if (OrderIsBelowChart || OrderIsAboveChart)
							allopacity = pHiddenOrdersOpacity;
						
						
						
						
						
						string dire = "";
						string typee = "STP";
						

						//Print(ThisPrice);
						
						
						if (or.IsShort) // In Long Trade
						{
							//if (Mode == "BE")
							//	NewStopPrice = RTTS(ThisPositionNow().AveragePrice + beoo*TickSize);

						}
						else // In Short Trade
						{
							//if (Mode == "BE")
							//	NewStopPrice = RTTS(ThisPositionNow().AveragePrice - beoo*TickSize);

						}
						
						StopLimitOffset = or.StopPrice - or.LimitPrice;
									NewLimitPrice = NewStopPrice - StopLimitOffset;
									
									CurrentStopPrice = or.StopPrice;
						
						if (or.LimitPrice == 0)
							NewLimitPrice = 0;
						
						
						//if (NewStopPrice != 0 && NewStopPrice != CurrentStopPrice)
						//	instruction = OIF_ChangeOrder(0,NewLimitPrice,NewStopPrice,or.Id.ToString(),"");
						
						//Submit();
						

			
//							x1 = chartControl.GetXByBarIndex(ChartBars,LB);
				
//							double xL = x1 - barWidth - 1;
//							double xL2 = x1;
//							double xR = x1 + barWidth;
//							double xW = barWidth + barWidth + 1;
//							double xW2 = barWidth;
							
						
						startright = ChartPanel.W - maininitialspace;
						
						

						
	
//						if (!AllOrderPrices.ContainsKey(ThisPrice))
//							AllOrderPrices.Add(ThisPrice, startright - orderdwidth - orderhspace);
//						else
//						{
//							startright =  AllOrderPrices[ThisPrice] - orderdwidth - orderhspace;
//							AllOrderPrices[ThisPrice] = startright;
//						}		
						
						//double ThisPrice2 = ThisPrice;
						
						//if (OrderIsAboveChart || OrderIsBelowChart)
						//if (ThisPrice == 1000000 || ThisPrice == 1000001)	
							
						//	ThisPrice2 = CurrentLast;
						
							if (AllOrderPrices.ContainsKey(ThisPrice))
							startright = AllOrderPrices[ThisPrice];
						
						
							
							double ThisPrice3 = ThisPrice2;
							
							if (OrderIsAboveChart || OrderIsBelowChart)
								ThisPrice3 = CurrentLast;
							
							
							
							y1 = chartScale.GetYByValue(ThisPrice3);

							// adjust for screen size
							
							
							
				CellFormat			= pTextFont2.ToDirectWriteTextFormat();	
				
				TextLayout CellTextLayout33 = new TextLayout(Core.Globals.DirectWriteFactory, CellString, CellFormat, 10000, 10000);
				
				OrderDisplayMinHeight = (int) CellTextLayout33.Metrics.Height + (int) Math.Ceiling(CellTextLayout33.Metrics.Height/8) + 2;
				
				CellTextLayout33.Dispose();
							
							yA = y1 - OrderDisplayMinHeight/2-1;
							yB = y1 - OrderDisplayMinHeight/2-1;
							yH = OrderDisplayMinHeight;
							yT = yA;
							
//							yA = GetBoxYPixel(ThisPrice3,"Top");
//							yB = GetBoxYPixel(ThisPrice3,"Bottom");
//							yH = GetBoxYPixel(ThisPrice3,"Height");
//							yT = yA;		
						
//						if (yH < OrderDisplayMinHeight)
//						{
//							double diff = OrderDisplayMinHeight - yH;
//							yH = Math.Max(OrderDisplayMinHeight, yH);
							
//							yT = yT - diff/2;

							
							
//						}
							
						
							if (OrderIsAboveChart)
								yT = 5;
							if (OrderIsBelowChart)
								yT = ChartPanel.H - 3 - yH;							
			
							ThisRect = new SharpDX.RectangleF((float)0, (float)yT, (float)startright, (float)yH);
						
						ThisRect = new SharpDX.RectangleF((float)startright - orderdwidth, (float)yT, orderdwidth, (float)yH);
						
						
							// change colors
												
							//ThisBrushDX = LastPriceBrushDX;
						
				
							if (pMakeOrderOutlineNotSeeThru) RenderTarget.FillRectangle(ExpandRect(ThisRect,3,3), ChartBackgroundBrushDX);
						
							ThisBrushDX = ChartControl.Properties.ChartText.ToDxBrush(RenderTarget);
							ThisBrushDX.Opacity = (float) pOrderOutlineOpacity/100f;
							
							RenderTarget.FillRectangle(ExpandRect(ThisRect,3,3), ThisBrushDX);
						
						
						
						
						
							RenderTarget.FillRectangle(ExpandRect(ThisRect,1,1), ChartBackgroundBrushDX);
						
						
						
						
					
						
						
						
						
							if (or.Name.Contains("Target"))
							{
								ThisBrushDX = pOrderTargetColor.ToDxBrush(RenderTarget);
								ThisBrushDX2 = GetTextColor(pOrderTargetColor).ToDxBrush(RenderTarget);
								
								typee = or.Name;
							}
							else if (or.Name.Contains("Stop"))
							{
								ThisBrushDX = pOrderStopColor.ToDxBrush(RenderTarget);
								ThisBrushDX2 = GetTextColor(pOrderStopColor).ToDxBrush(RenderTarget);
							
								typee = or.Name;
							}					
		
							
							else if (or.OrderType == OrderType.Limit)
							{
								ThisBrushDX = pOrderLimitColor.ToDxBrush(RenderTarget);
								ThisBrushDX2 = GetTextColor(pOrderLimitColor).ToDxBrush(RenderTarget);
								
								typee = "LMT";
							}
							else if (or.OrderType == OrderType.MIT)
							{
								ThisBrushDX = pOrderMITColor.ToDxBrush(RenderTarget);
								ThisBrushDX2 = GetTextColor(pOrderMITColor).ToDxBrush(RenderTarget);
								
								typee = "MIT";
							}
							else if (or.OrderType == OrderType.StopLimit)
							{
								ThisBrushDX = pOrderStopLimitColor.ToDxBrush(RenderTarget);
								ThisBrushDX2 = GetTextColor(pOrderStopLimitColor).ToDxBrush(RenderTarget);
								
								typee = "SLM";
							}
							else if (or.OrderType == OrderType.StopMarket)
							{
								ThisBrushDX = pOrderStopMarketColor.ToDxBrush(RenderTarget);
								ThisBrushDX2 = GetTextColor(pOrderStopMarketColor).ToDxBrush(RenderTarget);
								
								typee = "STP";
							}							
							
							if (or.OrderState == OrderState.ChangePending || or.OrderState == OrderState.ChangeSubmitted || or.OrderState == OrderState.CancelPending || or.OrderState == OrderState.CancelSubmitted)
							{
								ThisBrushDX = pOrderChangeColor.ToDxBrush(RenderTarget);
								ThisBrushDX2 = GetTextColor(pOrderChangeColor).ToDxBrush(RenderTarget);
							}
							
							
							ThisBrushDX.Opacity = (float) allopacity/100f;
							ThisBrushDX2.Opacity = (float) Math.Min(100,allopacity+10)/100f;
							
							
							//if (OrderIsAboveChart)
//							else if (or.OrderAction == OrderAction.Sell || or.OrderAction == OrderAction.SellShort)
//							{
//								ThisBrushDX = pClickDNColor.ToDxBrush(RenderTarget);
							
//							}
//							else
//							{
//								ThisBrushDX = pClickUPColor.ToDxBrush(RenderTarget);
							
							
								
//							}
							
							
							if (or.OrderAction == OrderAction.Sell || or.OrderAction == OrderAction.SellShort)
							{
								
								dire = "Sell";	
							}
							else
							{
								dire = "Buy";	
							}
							
								
							
							if (or.Name.Contains ("Stop") || or.Name.Contains ("Target"))
							{
								
								dire = "";
								
							}
								
						
							string dirrec = "";

							if (dire != "")
								dirrec = dirrec + " " + dire;
							
							if (typee != "")
								dirrec = dirrec + " " + typee;						
						
							
		                  //  CellString = cancelmessage + dirrec + " " + qudSelector.Value + " " + ClickText;
						
							dirrec = dirrec.Replace("Target", "Target ");
							dirrec = dirrec.Replace("Stop", "Stop ");
							
							
							CellString = dirrec;
							
							
						
							

							
							
							RenderTarget.FillRectangle(ThisRect, ThisBrushDX);
						
						
							//RenderTarget.FillRectangle(ExpandRect(ThisRect,3,3), ChartBackgroundMenuFadeBrushDX);
			
						// cancel buttons
												
							ThisRect2 = new SharpDX.RectangleF((float)startright-(float)yH-6, (float)yT, (float)yH+2, (float)yH);
						
							RenderTarget.FillRectangle(ThisRect2, ChartBackgroundFade2BrushDX);
						
			
							
						// Xsssss
							
						ThisStroke = pOneWidthStroke;
						
									
						oldAntialiasMode = RenderTarget.AntialiasMode;
			            RenderTarget.AntialiasMode = SharpDX.Direct2D1.AntialiasMode.PerPrimitive;

			       
			
							
//						StartPoint	= new Point(ThisRect2.Left, ThisRect2.Top);							
//						EndPoint = new Point(ThisRect2.Right, ThisRect2.Bottom);		
						
						
//						RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ChartBackgroundBrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
						
//						StartPoint	= new Point(ThisRect2.Left, ThisRect2.Bottom);							
//						EndPoint = new Point(ThisRect2.Right, ThisRect2.Top);		
						
						
//						RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ChartBackgroundBrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);

							
						
						CellFormat			= pTextFont2.ToDirectWriteTextFormat();	
						CellFormat			= ButtonText;
				
	                    CellFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Center;
	                    CellFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
	                    CellFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;
						//CellFormat.FontSize = 15;
					
						
						ThisBrushDX = ChartTextBrushDX;
						RenderTarget.DrawText("X", CellFormat, ThisRect2, ThisBrushDX2);	
//						RenderTarget.DrawText("X", CellFormat, ThisRect2, Brushes.Red.ToDxBrush(RenderTarget));		
							
							
						RenderTarget.AntialiasMode = oldAntialiasMode;
							
							
						StartPoint	= new Point(ThisRect2.Left, ThisRect2.Bottom);							
						EndPoint = new Point(ThisRect2.Left, ThisRect2.Top);								
						
						//RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), Brushes.Black.ToDxBrush(RenderTarget), ThisStroke.Width, ThisStroke.StrokeStyle);
							
						RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ChartBackgroundBrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
							
						StartPoint	= new Point(ThisRect2.Right, ThisRect2.Bottom);							
						EndPoint = new Point(ThisRect2.Right, ThisRect2.Top);								
						
						RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ChartBackgroundBrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
							
							
						// end cancel buttons
								
							double orderprice = 0;
							
							if (docombinestopsperprice && or.Name.Contains("Stop"))
								orderprice = ThisPrice2;
										
							
							buttonseq = buttonseq + 1;
						
						    OrderDetails Z = new OrderDetails();
				            Z.ThisOrder = or;
							Z.ThisRectA = ThisRect2;
				            Z.Name = "";
				            Z.Width = orderprice;
				            Z.Switch = false;
				            Z.Rect = new SharpDX.RectangleF(0, 0, 0, 0);
				            Z.Hovered = false;

							AllOrderCancelButtons.Add(buttonseq,Z);
		
						// move buttons
												
							//ThisRect2 = new SharpDX.RectangleF((float)startright-(float)yH-(float)yH-20f, (float)yT, (float)yH+1, (float)yH);					
							//RenderTarget.FillRectangle(ThisRect2, ChartBackgroundMenuFadeBrushDX);
						//	buttonseq = buttonseq + 1;
						
						    Z = new OrderDetails();
				            Z.ThisOrder = or;
							Z.ThisRectA = ThisRect2;
							Z.ThisRectA = ThisRect;
						
				            Z.Name = "";
				            Z.Width = orderprice;
				            Z.Switch = false;
				            Z.Rect = new SharpDX.RectangleF(0, 0, 0, 0);
				            Z.Hovered = false;

							
							if (!OrderIsBelowChart && !OrderIsAboveChart)
							AllOrderMoveButtons.Add(buttonseq,Z);
					

							// split stop button
							bool showit = false;
							
							
							if (FoundStopLossPrices.ContainsKey(ThisPrice2))
								if (FoundStopLossPrices[ThisPrice2] == 1)
									showit = true;
							
							if (or.Name.Contains("Stop"))
							if (docombinestopsperprice || showit)
							{
								
								//ThisRect2 = new SharpDX.RectangleF((float)startright-(float)yH-6, (float)yT, (float)yH+2, (float)yH);
								
								
								ThisRect2 = new SharpDX.RectangleF((float)startright-(float)yH-6-(float)yH-2, (float)yT, (float)yH+2, (float)yH);					
								RenderTarget.FillRectangle(ThisRect2, ChartBackgroundFade2BrushDX);
						//	buttonseq = buttonseq + 1;
								
								orderprice = ThisPrice2;						
							    Z = new OrderDetails();
					            Z.ThisOrder = or;
								Z.ThisRectA = ThisRect2;
								//Z.ThisRectA = ThisRect;
							
					            Z.Name = "";
					            Z.Width = orderprice;
					            Z.Switch = false;
					            Z.Rect = new SharpDX.RectangleF(0, 0, 0, 0);
					            Z.Hovered = false;

								StartPoint	= new Point(ThisRect2.Left, ThisRect2.Bottom);							
								EndPoint = new Point(ThisRect2.Left, ThisRect2.Top);								
								
								//RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), Brushes.Black.ToDxBrush(RenderTarget), ThisStroke.Width, ThisStroke.StrokeStyle);
									
								RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ChartBackgroundBrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
								
								RenderTarget.DrawText("S", CellFormat, ThisRect2, ThisBrushDX2);	
								
								//if (!OrderIsBelowChart && !OrderIsAboveChart)
								AllStopCombinationButtons.Add(buttonseq,Z);
							
							}
							
							 
							
							
							
							
						// end
						
						// text on top of order
						
						float sidepadding = 5;	
							
							lefttt = startright - ThisRect2.Left + sidepadding + 1;
						
							
							
							if (docombinestopsperprice && or.Name.Contains("Stop"))
								CellString = "Stop Loss";
							
							
							
							CellFormat			= pTextFont2.ToDirectWriteTextFormat();	
							CellFormat			= ButtonText;
					
		                    CellFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Trailing;
		                    CellFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
		                    CellFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;

						
						
						//ThisBrushDX = ChartTextBrushDX;
							
						
						RenderTarget.DrawText(CellString, CellFormat, ExpandRect(ThisRect,0,lefttt*-1,0,0), ThisBrushDX2);	
					
					
							// quanitity text
				
						CellString = or.Quantity.ToString();
						if (docombinestopsperprice && or.Name.Contains ("Stop"))
							CellString = AllStopLossPrices[ThisPrice2].ToString();							
							
						CellFormat			= pTextFont2.ToDirectWriteTextFormat();	
						CellFormat			= ButtonText;
					
		                    CellFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Leading;
		                    CellFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
		                    CellFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;

						
						
						//ThisBrushDX = ChartTextBrushDX;
							
						
						RenderTarget.DrawText(CellString, CellFormat, ExpandRect(ThisRect,sidepadding*-1,0,0,0), ThisBrushDX2);							
							
							
				
						if (!AllOrderPrices.ContainsKey(ThisPrice))
							AllOrderPrices.Add(ThisPrice, startright - orderdwidth - orderhspace);
						else
						{
							startright =  AllOrderPrices[ThisPrice] - orderdwidth - orderhspace;
							AllOrderPrices[ThisPrice] = startright;
						}
						
						
					}
					
				}
				
				
			ChartBackgroundFade2BrushDX.Dispose();
		}
		
	
		
		private void DrawComposite (ChartControl chartControl, ChartScale chartScale)
		{		
			if (pCompositeLocation != "None")
			if (pCompositeEnabled)
			{
			
				
				
				//SharpDX.Direct2D1.Brush ProfileBrush1DX = pResistanceZColor1.ToDxBrush(RenderTarget);
				//SharpDX.Direct2D1.Brush TextColorMDDX = pMainTextColor.ToDxBrush(RenderTarget);	
				
				
				
				if (Profiles.IsValidDataPointAt(LB))
				{
					Profile CurrentProfile = Profiles.GetValueAt(LB);
					
					double maxtv = CurrentProfile.maxtv;
					double maxav = CurrentProfile.maxav;
					double maxbv = CurrentProfile.maxbv;
					double maxdv = CurrentProfile.maxdv;
					
					
					SharpDX.DirectWrite.TextFormat CellFormat			= FinalFont1.ToDirectWriteTextFormat();	

	                CellFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Trailing;
	                CellFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Center;
	                CellFormat.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;
			
					string CellString = string.Empty;
				
					TextLayout CellTextLayout = new TextLayout(Core.Globals.DirectWriteFactory, CellString, CellFormat, 10000, 10000);
					
					
					// shift profile to left side
					
					bool isleft = true;
					
					if (pCompositeLocation == "Right")
						isleft = false;
					
					
					//if (isleft)
					{
						// add background behind profile
						
						float xcdf = ChartPanel.X + ChartPanel.W - RightMoveX + 1 - (float)pCompLength - pSpaceBetweenColumns - 1;
						
						if (isleft) 
							xcdf = 0;
						
						ThisRect = new SharpDX.RectangleF(xcdf, (float)0, (float)pCompLength+pSpaceBetweenColumns, (float)3000);
						B44 = ThisRect;
						
						//if (isleft)
						if (pCompositeBackEnabled || InMenu4)
						RenderTarget.FillRectangle(ThisRect, ChartBackgroundFadeBrushDX);

						
						
						
						
						
						
						
						// vertical line			
				
						if (isleft)
						{
							StartPoint	= new Point(pCompLength+pSpaceBetweenColumns, 0);
							EndPoint = new Point(pCompLength+pSpaceBetweenColumns, 5000);
							
							CurrentCompositeX = pCompLength+pSpaceBetweenColumns;
						}
						else
						{
							StartPoint	= new Point(xcdf+1, 0);
							EndPoint = new Point(xcdf+1, 5000);		
							
							CurrentCompositeX = xcdf+1;
							
						}
					
						
						// stand aloone
							
						StartPoint	= new Point(CurrentCompositeX, 0);							
						EndPoint = new Point(CurrentCompositeX, 5000);							
						
						
						// vertical line
						
						MoveComposite = new SharpDX.RectangleF((float)StartPoint.X, (float)0, (float)1, (float)5000);						
						
						if (IsMoveComposite || IsHoverComposite)	
						{
							//ThisStroke = new Stroke(Brushes.WhiteSmoke, DashStyleHelper.Solid, 2);
							ThisStroke = pHighlightStroke;
							RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), VerticalLineHighlightDX, ThisStroke.Width, ThisStroke.StrokeStyle);
						}
						
						
						
						ThisStroke = myProperties.GridLineVPen;
						RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ThisStroke.BrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);
						
						//ThisStroke = myProperties.AxisPen;
						
						

						
						
							
		 
						
					}
					
					
					ConcurrentDictionary<double, RowData> NewCD = new ConcurrentDictionary<double, RowData>(CurrentProfile.l);
					foreach (KeyValuePair<double, RowData> Price in NewCD)
					//foreach (KeyValuePair<double, RowData> Price in CurrentProfile.l)
					{
						
						
						double ThisPrice = Price.Key;
						double BidVol = Price.Value.bv;
						double AskVol = Price.Value.av;
						double TotalVol = Price.Value.tv;
						double TotalDelta = Price.Value.dv;
						
						if (ThisPrice < BottomPrice || ThisPrice > TopPrice)
							continue;		
						
						//Print(ThisPrice);
						
						double yA = GetBoxYPixel(ThisPrice,"Top");
						double yB = GetBoxYPixel(ThisPrice,"Bottom");
						double yH = GetBoxYPixel(ThisPrice,"Height");
						double yT = yA;		
						
						//bool DrawTextOK = PriceRowHeight >= MaxTextYPixels;
						bool DrawTextOK = FinalTextSize1 >= minimumtextsize;
						
						float leftspace = 6;
						
						// NEW COLUMN

						double maxwidth = pCompLength;
						double lastx2 = ChartPanel.X + ChartPanel.W - RightMoveX + 1;
						
						
							double ThisVol =TotalVol;
							double OpacityLow2 = pCompMinOpacity;
							double OpacityHigh2 = pCompMaxOpacity;
							double OpacityMultiplier2 = (OpacityHigh2 - OpacityLow2) / CurrentProfile.maxtv;
							double ThisOpacity2 = Math.Round(OpacityLow2 + OpacityMultiplier2*(ThisVol-ThisVol/6),0);
			
							CompVOLColorDX.Opacity = (float) ThisOpacity2/100f;
							CompUPColorDX.Opacity = (float) ThisOpacity2/100f;
							CompDNColorDX.Opacity = (float) ThisOpacity2/100f;
							CompNEColorDX.Opacity = (float) ThisOpacity2/100f;
						
							
						if (isleft)
							lastx2 = 0;	
							// color
													

							
							// text
						if (pCompNumberDisplayMode != "None")
						{		
							// get biggest width strings
							
							if (pCompNumberDisplayMode == "Bid / Ask")
							{
								CellString = maxav.ToString();
								
								if (isleft)
								CellString = maxbv.ToString();	
							}
							else if (pCompNumberDisplayMode == "Delta")
							{
								CellString = maxtv.ToString();
							}
							else if (pCompNumberDisplayMode == "Volume")
							{
								CellString = maxtv.ToString();
							}
							
							
							CellTextLayout = new TextLayout(Core.Globals.DirectWriteFactory, CellString, CellFormat, 10000, 10000);
							
		                    float FinalH = CellTextLayout.Metrics.Height;
		                    float FinalW = CellTextLayout.Metrics.Width + leftspace*2;
							maxwidth = maxwidth - FinalW - 1;
							
							// set actual strings
							
							if (pCompNumberDisplayMode == "Bid / Ask")
							{
								CellString = AskVol.ToString();
								
								if (isleft)
									CellString = BidVol.ToString();	
																
							}
							else if (pCompNumberDisplayMode == "Delta")
							{
								CellString = Math.Abs(TotalDelta).ToString();
							}
							else if (pCompNumberDisplayMode == "Volume")
							{
								CellString = TotalVol.ToString();
							}
							
							CellFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Leading;
							if (isleft)
								CellFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Trailing;
															
							x1 = ChartPanel.X + ChartPanel.W - RightMoveX;
							x2 = x1 - FinalW;
							
							if (isleft)
								x2 = 1;
							
							//ThisRect = new SharpDX.RectangleF((float)x2, (float)yT, (float)thishwidth, (float)yH);
							ThisRect = new SharpDX.RectangleF((float)x2, (float)yT, (float)FinalW, (float)yH);
							
							//background brush
												
							ThisBrushDX = ChartBackgroundBrushDX;

							RenderTarget.FillRectangle(ExpandRect(ThisRect,0,0,1,1), ThisBrushDX);
							
							if (pUseBackgroundColor)
							{
								ThisBrushDX = BackgroundColorDX;
							RenderTarget.FillRectangle(ThisRect, ThisBrushDX);
								}
							
							
							//RenderTarget.FillRoundedRectangle
							// change colors
							
							ThisBrushDX = CompVOLColorDX;
							
							if (pCompNumberDisplayMode == "Delta")
							{

								CompVOLColorDX.Opacity = (float) ThisOpacity2/100f;
								CompUPColorDX.Opacity = (float) ThisOpacity2/100f;
								CompDNColorDX.Opacity = (float) ThisOpacity2/100f;
								CompNEColorDX.Opacity = (float) ThisOpacity2/100f;
								// text
								
								if (TotalDelta > 0)
								{
									ThisBrushDX = CompUPColorDX;
								}
								else if (TotalDelta < 0)
								{
									ThisBrushDX = CompDNColorDX;
								}
								else
								{
									ThisBrushDX = CompNEColorDX;
								}
							
							}						
							
							SetKeyLevelsColor(ThisPrice);
							RenderTarget.FillRectangle(ThisRect, ThisBrushDX);

							
							ThisBrushDX = TextBrushDX2;
							
							float adjle = 0;
							if (pCompNumberDisplayMode == "Bid / Ask")
							adjle = 2;	
								
							
								if (DrawTextOK) 
								{
									if (isleft)
										RenderTarget.DrawText(CellString, CellFormat, MoveRect(ThisRect,-leftspace-adjle,0), TextBrushDX2);	
									else
										RenderTarget.DrawText(CellString, CellFormat, MoveRect(ThisRect,leftspace,0), TextBrushDX2);
								}							
							
							
							
							// show column divider
							float exp = 1;
							
							// NEW COLUMN ----------------------------------------------
							
							if (pCompNumberDisplayMode == "Bid / Ask")
							{
								
								CellString = maxbv.ToString();
								
								if (isleft)
									CellString = maxav.ToString();	
																
								CellTextLayout = new TextLayout(Core.Globals.DirectWriteFactory, CellString, CellFormat, 10000, 10000);
								
			                    FinalH = CellTextLayout.Metrics.Height;
			                    FinalW = CellTextLayout.Metrics.Width + leftspace*2;
								maxwidth = maxwidth - FinalW - 1;
								
								if (isleft)
									x2 = x2 + FinalW-exp;							
								else
									x2 = x2 - FinalW-exp;
								
								CellString = BidVol.ToString();
								
								if (isleft)
									CellString = AskVol.ToString();	
																
								CellFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Trailing;
								if (isleft)
								CellFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Leading;
								
								ThisRect = new SharpDX.RectangleF((float)x2, (float)yT, (float)FinalW, (float)yH);
								//x2 = x2 - ThisRect.Width - 1;
													
								ThisBrushDX = ChartBackgroundBrushDX;

								RenderTarget.FillRectangle(ExpandRect(ThisRect,exp,exp,1,1), ThisBrushDX);
								
								if (pUseBackgroundColor)
								{
									ThisBrushDX = BackgroundColorDX;
								RenderTarget.FillRectangle(ThisRect, ThisBrushDX);
								}
								
							
							
								//RenderTarget.FillRoundedRectangle
								// change colors
								
								
								ThisBrushDX = CompVOLColorDX;
								SetKeyLevelsColor(ThisPrice);
								RenderTarget.FillRectangle(ThisRect, ThisBrushDX);

								
								ThisBrushDX = TextBrushDX2;
								
								if (DrawTextOK) 
								{
									if (isleft)
										RenderTarget.DrawText(CellString, CellFormat, MoveRect(ThisRect,leftspace,0), TextBrushDX2);	
									else
										RenderTarget.DrawText(CellString, CellFormat, MoveRect(ThisRect,-leftspace,0), TextBrushDX2);
								}
								
							}
							
						    lastx2 = x2;
							
							if (isleft)
							lastx2 = x2 + FinalW;
							
							
						}
					
						
						
						
							//if (!isleft)
							if (pCompHistogramDisplayMode != "None")
							{
								
								
								//Print(maxwidth);
								
								// HISTOGRAM

								double ptv = TotalVol / maxtv;

								double ptv2 = TotalVol / maxtv;
									
									if (TotalDelta > 0)
									{
										ptv2 = AskVol / maxtv;
									}
									else if (TotalDelta < 0)
									{
										ptv2 = BidVol / maxtv;
									}
									else
									{
										ptv2 = 1 / maxtv;
									}
										
										
									double thishwidth2 = maxwidth * ptv2;
									//x2 = lastx2 - thishwidth - 1;
									
									
								double thishwidth = maxwidth * ptv;
								

									
										
								if (isleft)
									x2 = lastx2 + 1;					
								else
									x2 = lastx2 - thishwidth - 1;
									
								x1 = ChartPanel.X + ChartPanel.W - RightMoveX - ThisRect.Width;
								
								ThisRect = new SharpDX.RectangleF((float)x2, (float)yT, (float)thishwidth, (float)yH);
								
		//						ThisVol = TotalVol;
		//						OpacityLow2 = pCompMinOpacity;
		//						OpacityHigh2 = pCompMaxOpacity;
		//						OpacityMultiplier2 = (OpacityHigh2 - OpacityLow2) / CurrentProfile.maxtv;
		//						ThisOpacity2 = Math.Round(OpacityLow2 + OpacityMultiplier2*(ThisVol-ThisVol/6),0);
								
		//						CompVOLColorDX.Opacity = (float) ThisOpacity2/100f;
		//						HUDNEColorDX.Opacity = (float) ThisOpacity2/100f;
		//						HUDUPColorDX.Opacity = (float) ThisOpacity2/100f;
		//						HUDDNColorDX.Opacity = (float) ThisOpacity2/100f;
								
		//						ThisBrushDX = CompVOLColorDX;

								
								//background brush
									
									
							
							RenderTarget.FillRectangle(ExpandRect(ThisRect,1,1,1,1), ChartBackgroundBrushDX);
								
								
							
							if (pUseBackgroundColor)
							{
								ThisBrushDX = BackgroundColorDX;
								RenderTarget.FillRectangle(ThisRect, ThisBrushDX);
							}
							
							
							
							

								//RenderTarget.FillRoundedRectangle
								// change colors
								ThisBrushDX = CompVOLColorDX;
								
								SetKeyLevelsColor(ThisPrice);
								
								RenderTarget.FillRectangle(ThisRect, ThisBrushDX);

								ThisRect = new SharpDX.RectangleF((float)x2-4, (float)yT, (float)thishwidth, (float)yH);
							
						
								if (pCompHistogramDisplayMode == "Volume & Delta")
								{
									// HISTOGRAM 2
									
								
									
									if (isleft)
										x2 = lastx2 + 1;					
									else
										x2 = lastx2 - thishwidth2 - 1;
									
									
									x1 = ChartPanel.X + ChartPanel.W - RightMoveX - ThisRect.Width;
									
									ThisRect = new SharpDX.RectangleF((float)x2, (float)yT, (float)thishwidth2, (float)yH);
									
			//						ThisVol = TotalVol;
			//						OpacityLow2 = pCompMinOpacity;
			//						OpacityHigh2 = pCompMaxOpacity;
			//						OpacityMultiplier2 = (OpacityHigh2 - OpacityLow2) / CurrentProfile.maxtv;
			//						ThisOpacity2 = Math.Round(OpacityLow2 + OpacityMultiplier2*(ThisVol-ThisVol/6),0);
									
			//						CompVOLColorDX.Opacity = (float) ThisOpacity2/100f;
			//						HUDNEColorDX.Opacity = (float) ThisOpacity2/100f;
			//						HUDUPColorDX.Opacity = (float) ThisOpacity2/100f;
			//						HUDDNColorDX.Opacity = (float) ThisOpacity2/100f;
									
			//						ThisBrushDX = CompVOLColorDX;

									
									//background brush
									
									
					
									RenderTarget.FillRectangle(ExpandRect(ThisRect,1,1,1,1), ChartBackgroundBrushDX);
									
									if (pUseBackgroundColor)
									{
										ThisBrushDX = BackgroundColorDX;
										RenderTarget.FillRectangle(ThisRect, ThisBrushDX);
									}
									
								
						
									//RenderTarget.FillRoundedRectangle
									// change colors
								
									if (TotalDelta > 0)
									{
										ThisBrushDX = CompUPColorDX;
									}
									else if (TotalDelta < 0)
									{
										ThisBrushDX = CompDNColorDX;
									}
									else
									{
										ThisBrushDX = CompNEColorDX;
									}
								
									SetKeyLevelsColor(ThisPrice);
								
									
																	
									RenderTarget.FillRectangle(ThisRect, ThisBrushDX);

									//ThisRect = new SharpDX.RectangleF((float)x2-4, (float)yT, (float)thishwidth, (float)yH);
								
								}
						
							}
						
						
					}
					
					CellFormat.Dispose();
					CellTextLayout.Dispose();
					
	//				do
	//				{
						
	//				} 
	//				while (LastPrice >= LowPrice);	
				}
				
				//ProfileBrush1DX.Dispose();
				//TextColorMDDX.Dispose();
				
									
				if (pCompositeLocation == "Right")		
					RightMoveX = RightMoveX + pCompLength;
				
				
				
			}
		}

		private void SetKeyLevelsColor(double ThisPrice)
		{
			// set color for rolling composite levels on the profile itself
			return;
						if (ThisPrice == CompPOC.GetValueAt(LB))
							ThisBrushDX = Plot1BrushDX;
						
						if (ThisPrice == RTTS(CompVWAP.GetValueAt(LB)))
							ThisBrushDX = Plot2BrushDX;

						if (ThisPrice == CompVAH1.GetValueAt(LB))
							ThisBrushDX = Plot3BrushDX;
						
						if (ThisPrice == CompVAL1.GetValueAt(LB))
							ThisBrushDX = Plot4BrushDX;
						
						if (ThisPrice == CompVAH2.GetValueAt(LB))
							ThisBrushDX = Plot5BrushDX;
						
						if (ThisPrice == CompVAL2.GetValueAt(LB))
							ThisBrushDX = Plot6BrushDX;
						
						if (ThisPrice == CompDH.GetValueAt(LB))
							ThisBrushDX = Plot7BrushDX;
						
						if (ThisPrice == CompDL.GetValueAt(LB))
							ThisBrushDX = Plot8BrushDX;
						
						
		}
		
		private void DrawResistanceZones(List<Zone> zzz, int jjj, ChartControl chartControl, ChartScale chartScale)
		{
																//foreach (ZoneItem zzz in ZoneItems)	

			//if (doef)
								foreach (Zone ZS in zzz)	
								//foreach(Zone ZS in DZ.Value)
								{
									
									//Print();
									
									if (!DrawOneBarZones)
									if (ZS.EndBar != 0 && jjj + 1 >= ZS.EndBar) // don't draw broken zones that only painted for one bar.
										continue;
									
									if (ZS.BottomPrice >= TopPrice)
										continue;
									
									if (ZS.TopPrice <= BottomPrice)
										continue;	
									
									if (ZS.EndBar != 0 && !pShowBrokenZones)
										continue;
									
									if (ZS.EndBar == 0 && ZS.TestedPrice == 0 && !pShowFreshZones)
										continue;
									
									if (ZS.TestedPrice != 0 && !pShowTestedZones)
										continue;					
									
									if (ZS.IsHidden)
										continue;
									
									if (ZS.EndBar != 0 && ZS.EndBar < FB)
										continue;	
									
									
									//ThisPen = ResistancePen1;
									MainFillBrushDX = ResistanceBrush1DX;
									
									x2 = ChartControl.GetXByBarIndex(ChartBars, LB) + barWidth+XE;
									x2 = GetBoxXPixel(LB, "Right", false);
									
									if (pExtendZonesRight)
										x2 = x2 + myProperties.BarMarginRight + barWidth*2;					
									
									if (pExtendZonesRight)
										x2 = ChartPanel.W;
									
									if (ZS.EndBar != 0)
									{
										
										//ThisPen = ResistancePen3;
										MainFillBrushDX = ResistanceBrush3DX;
										
										//x2 = Math.Min(ChartControl.GetXByBarIndex(ChartBars, ZS.EndBar) - barWidth, ChartControl.GetXByBarIndex(ChartBars, LB) + barWidth+XE);
										// change to draw to right side of the bar that broke the zone
								
										
										x2 = Math.Min(ChartControl.GetXByBarIndex(ChartBars, ZS.EndBar) + barWidth+XE-1,  ChartControl.GetXByBarIndex(ChartBars, LB) + barWidth+XE-1);
										
										
//										if (ZS.EndBar > LB)
//											x2 = GetBoxXPixel(LB, "Right", false);
//										else
//											x2 = GetBoxXPixel(ZS.EndBar, "Right", false);
										
										
										if (pExtendZonesRight && LB < ZS.EndBar)
											x2 = x2 + myProperties.BarMarginRight + barWidth*2;						
										
									}
									
									if (ZS.TestedPrice != 0)
									{
										
										//ThisPen = ResistancePen2;								
										MainFillBrushDX = ResistanceBrush2DX;
									}
									
									y1 = 0;
									y2 = 0;
									
									if (pPrintEnabled)
									{
										y1 = GetBoxYPixel(ZS.TopPrice,"Top");
										y2 = GetBoxYPixel(ZS.BottomPrice,"Bottom");
									}
									else
									{
										y1 = chartScale.GetYByValue(ZS.TopPrice);//ChartControl.GetYByValue(BarsArray[0], );
										y2 = chartScale.GetYByValue(ZS.BottomPrice);//ChartControl.GetYByValue(BarsArray[0], ZS.BottomPrice);							
									}

									y3 = y2-y1;

									BidRect = new SharpDX.RectangleF((float)x1,(float)y1, (float)x2 - (float)x1,(float)y3);
									BidRectF = new SharpDX.RectangleF((float)x1,(float)y1, (float)x2 - (float)x1+1,(float)y3+1);

									if (ZS.TicksWidth > 1 || pPrintEnabled) // is zone
									{
										
									
										//Print(jjj + "  " + ZS.TopPrice);
										
										if (ZS.TestedPrice == 0 || !pZonesTSEnabled)
										{
											// NOT TESTED

											
											RenderTarget.FillRectangle(BidRect, MainFillBrushDX);
										}
										else
										{
											
											
											
											if (ZS.TestedPrice == ZS.TopPrice)
											{
												// COMPLETELY TESTED
							
												RenderTarget.FillRectangle(BidRect, MainFillBrushDX);
												
											}
											else
											{
												// PARTIALLY TESTED
												
												if (!pPrintEnabled)
												{
													y1 = chartScale.GetYByValue(ZS.TopPrice);
													y2 = chartScale.GetYByValue(ZS.TestedPrice);
												
													y3 = y2-y1;
													
													y4 = chartScale.GetYByValue(ZS.TestedPrice);
													y5 = chartScale.GetYByValue(ZS.BottomPrice);
												
													y6 = y5-y4;
												}
												else
												{
												
													y1 = GetBoxYPixel(ZS.TopPrice,"Top");
													y2 = GetBoxYPixel(ZS.TestedPrice,"Top");
													
													y3 = y2-y1;
												
													y4 = GetBoxYPixel(ZS.TestedPrice,"Top");
													y5 = GetBoxYPixel(ZS.BottomPrice,"Bottom");
													
													y6 = y5-y4;
												}
												
												// FRESH
												
												BidRect5 = new SharpDX.RectangleF((float)x1,(float)y1, (float)x2 - (float)x1,(float)y3);
											
												MainFillBrushDX = ResistanceBrush1DX;										
												RenderTarget.FillRectangle(BidRect5, MainFillBrushDX);
												
											
												// TESTED

												BidRect5 = new SharpDX.RectangleF((float)x1,(float)y4, (float)x2 - (float)x1,(float)y6);
												
												MainFillBrushDX = ResistanceBrush2DX;
												RenderTarget.FillRectangle(BidRect5, MainFillBrushDX);
								

													
													
											}
										}
										
										//if (!InHitTest && pOutlineZones) graphics.DrawRectangle(ThisPen,BidRect);

									}
									else // is line
									{
										
										StartPoint	= new Point(x1,y1);
										EndPoint = new Point(x2,y1);
										
										ThisStroke = pONESupport;
										RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), MainFillBrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);

									}
									
									
									
									if (pZonesTMEnabled && ZS.TestedPrice != 0)
									{
										
										if (!pPrintEnabled)
										{
											y2 = chartScale.GetYByValue(ZS.TestedPrice);
										}
										else
										{
											y2 = GetBoxYPixel(ZS.TestedPrice+ThisTickSizze,"Bottom");	
										}
											
										StartPoint	= new Point(x1,y2);
										EndPoint = new Point(x2,y2);
										
										ThisStroke = pTMResistance;
										RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ThisStroke.BrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);

									}
												
									
									B1 = new SharpDX.RectangleF(Math.Max(ChartPanel.X,BidRectF.Left)+bsp2, BidRectF.Top - bsp-pButtonSize,pButtonSize,pButtonSize);
										
									BidRectF2 = new SharpDX.RectangleF(BidRectF.X,BidRectF.Y,BidRectF.Width-1,BidRectF.Height-1);
									
									if (MouseIn(BidRectF,5,5) || MouseIn(B1,5,5))
									{
										//Print("IN");
										
										if (ZS.TicksWidth == 1 && !pPrintEnabled)
										{
											//if (!InHitTest) graphics.DrawLine(ZoneH,x1,y1,x2,y1);
											
										}
										else
										{
											//if (!InHitTest) graphics.DrawRectangle(ZoneH,BidRectF2);
										}	

																					
										//MainFillBrushDX = ZoneButtonBrushDX;
										//RenderTarget.FillRectangle(B1, MainFillBrushDX);

										//RenderTarget.DrawRectangle(B1, MainFillBrushDX);
										
											//if (!InHitTest) graphics.FillRectangle(ButtonBrush,B1);
																	
											//if (!InHitTest) graphics.DrawRectangle(ButtonPen,B1);
															
											//if (!InHitTest) graphics.DrawString("X",ButtonFont,ButtonTextBrush,B1,FormatCenter);
										

											if (MouseIn(B1,MenuButtonExpandP, MenuButtonExpandP))
											{
												//if (!InHitTest) graphics.DrawRectangle(ZoneH,B1);
											
												ClickedZoneBar = jjj;
												ClickedZoneTop = ZS.TopPrice;
												ClickedZoneBottom = ZS.BottomPrice;		
											}		
									}	
								}	
			
			
			
		}

		
		private void DrawSupportZones(List<Zone> zzz, int jjj, ChartControl chartControl, ChartScale chartScale)
		{
		//bool doef = false;
								
								//if (doef)
								foreach (Zone ZS in zzz)	
								//foreach(Zone ZS in DZ.Value)
								{
									
									//Print();
									
									if (!DrawOneBarZones)
									if (ZS.EndBar != 0 && jjj + 1 >= ZS.EndBar) // don't draw broken zones that only painted for one bar.
										continue;
									
									if (ZS.BottomPrice >= TopPrice)
										continue;
									
									if (ZS.TopPrice <= BottomPrice)
										continue;	
									
									if (ZS.EndBar != 0 && !pShowBrokenZones)
										continue;
									
									if (ZS.EndBar == 0 && ZS.TestedPrice == 0 && !pShowFreshZones)
										continue;
									
									if (ZS.TestedPrice != 0 && !pShowTestedZones)
										continue;					
									
									if (ZS.IsHidden)
										continue;
									
									if (ZS.EndBar != 0 && ZS.EndBar < FB)
										continue;	
									
									
									//ThisPen = SupportPen1;
									MainFillBrushDX = SupportBrush1DX;
									
									x2 = ChartControl.GetXByBarIndex(ChartBars, LB) + barWidth+XE;
									x2 = GetBoxXPixel(LB, "Right", false);
									
									if (pExtendZonesRight)
										x2 = x2 + myProperties.BarMarginRight + barWidth*2;	
									
									if (pExtendZonesRight)
										x2 = ChartPanel.W;	
									
									if (ZS.EndBar != 0)
									{
										
										//ThisPen = SupportPen3;
										MainFillBrushDX = SupportBrush3DX;
										
										//x2 = Math.Min(ChartControl.GetXByBarIndex(ChartBars, ZS.EndBar) - barWidth, ChartControl.GetXByBarIndex(ChartBars, LB) + barWidth+XE);
										
										// change to draw to right side of the bar that broke the zone
								
										x2 = Math.Min(ChartControl.GetXByBarIndex(ChartBars, ZS.EndBar) + barWidth+XE-1,  ChartControl.GetXByBarIndex(ChartBars, LB) + barWidth+XE);
										
//										if (ZS.EndBar > LB)
//											x2 = GetBoxXPixel(LB, "Right", false);
//										else
//											x2 = GetBoxXPixel(ZS.EndBar, "Right", false);
										
										
										if (pExtendZonesRight && LB < ZS.EndBar)
											x2 = x2 + myProperties.BarMarginRight + barWidth*2;						
										
									}
									
									if (ZS.TestedPrice != 0)
									{
										
										//ThisPen = SupportPen2;								
										MainFillBrushDX = SupportBrush2DX;
									}
									
									y1 = 0;
									y2 = 0;
									
									if (pPrintEnabled)
									{
										y1 = GetBoxYPixel(ZS.TopPrice,"Top");
										y2 = GetBoxYPixel(ZS.BottomPrice,"Bottom");
									}
									else
									{
										y1 = chartScale.GetYByValue(ZS.TopPrice);//ChartControl.GetYByValue(BarsArray[0], );
										y2 = chartScale.GetYByValue(ZS.BottomPrice);//ChartControl.GetYByValue(BarsArray[0], ZS.BottomPrice);							
									}

									y3 = y2-y1;

									BidRect = new SharpDX.RectangleF((float)x1,(float)y1, (float)x2 - (float)x1,(float)y3);
									BidRectF = new SharpDX.RectangleF((float)x1,(float)y1, (float)x2 - (float)x1+1,(float)y3+1);

									if (ZS.TicksWidth > 1 || pPrintEnabled) // is zone
									{
										
									
										//Print(jjj + "  " + ZS.TopPrice);
										
										if (ZS.TestedPrice == 0 || !pZonesTSEnabled)
										{
											// NOT TESTED

											//MainFillBrushDX = SupportBrush1DX;
											RenderTarget.FillRectangle(BidRect, MainFillBrushDX);
										}
										else
										{
											
											
											
											if (ZS.TestedPrice == ZS.BottomPrice)
											{
												// COMPLETELY TESTED
							
												RenderTarget.FillRectangle(BidRect, MainFillBrushDX);
												
												
										
											}
											else
											{
												// PARTIALLY TESTED
												
												if (!pPrintEnabled)
												{
													y1 = chartScale.GetYByValue(ZS.TopPrice);
													y2 = chartScale.GetYByValue(ZS.TestedPrice);
												
													y3 = y2-y1;
													
													y4 = chartScale.GetYByValue(ZS.TestedPrice);
													y5 = chartScale.GetYByValue(ZS.BottomPrice);
												
													y6 = y5-y2;
												}
												else
												{
												
													y1 = GetBoxYPixel(ZS.TopPrice,"Top");
													y2 = GetBoxYPixel(RTTS(ZS.TestedPrice),"Bottom");
													
													y3 = y2-y1;
												
													y4 = GetBoxYPixel(ZS.TestedPrice-ThisTickSizze,"Top");
													y5 = GetBoxYPixel(ZS.BottomPrice,"Bottom");
													
													y6 = y5-y4;
												}
												
												BidRect5 = new SharpDX.RectangleF((float)x1,(float)y1, (float)x2 - (float)x1,(float)y3);
												
											
												// TESTED
												
												MainFillBrushDX = SupportBrush2DX;										
												RenderTarget.FillRectangle(BidRect5, MainFillBrushDX);
												
											
												// FRESH

												BidRect5 = new SharpDX.RectangleF((float)x1,(float)y4, (float)x2 - (float)x1,(float)y6);
												
												MainFillBrushDX = SupportBrush1DX;
												RenderTarget.FillRectangle(BidRect5, MainFillBrushDX);
								

													
													
											}
										}
										
										//if (!InHitTest && pOutlineZones) graphics.DrawRectangle(ThisPen,BidRect);

									}
									else // is line
									{
										StartPoint	= new Point(x1,y1);
										EndPoint = new Point(x2,y1);
										
										ThisStroke = pONESupport;
										RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), MainFillBrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);

									}
									
									if (pZonesTMEnabled && ZS.TestedPrice != 0)
									{
										
										if (!pPrintEnabled)
										{
											y2 = chartScale.GetYByValue(ZS.TestedPrice);
										}
										else
										{
											y2 = GetBoxYPixel(ZS.TestedPrice,"Bottom");	
										}
											
										StartPoint	= new Point(x1,y2);
										EndPoint = new Point(x2,y2);
										
										ThisStroke = pTMSupport;
										RenderTarget.DrawLine(StartPoint.ToVector2(), EndPoint.ToVector2(), ThisStroke.BrushDX, ThisStroke.Width, ThisStroke.StrokeStyle);

									}
												
									
									B1 = new SharpDX.RectangleF(Math.Max(ChartPanel.X,BidRectF.Left)+bsp2, BidRectF.Top - bsp-pButtonSize,pButtonSize,pButtonSize);
										
									BidRectF2 = new SharpDX.RectangleF(BidRectF.X,BidRectF.Y,BidRectF.Width-1,BidRectF.Height-1);
									
									
									
									if (MouseIn(BidRectF,5,5) || MouseIn(B1,5,5))
									{
										//Print("IN");
										
										if (ZS.TicksWidth == 1 && !pPrintEnabled)
										{
											//if (!InHitTest) graphics.DrawLine(ZoneH,x1,y1,x2,y1);
											
										}
										else
										{
											//if (!InHitTest) graphics.DrawRectangle(ZoneH,BidRectF2);
										}	

																					
										//MainFillBrushDX = ZoneButtonBrushDX;
										//RenderTarget.FillRectangle(B1, MainFillBrushDX);

										//RenderTarget.DrawRectangle(B1, MainFillBrushDX);
										
											//if (!InHitTest) graphics.FillRectangle(ButtonBrush,B1);
																	
											//if (!InHitTest) graphics.DrawRectangle(ButtonPen,B1);
															
											//if (!InHitTest) graphics.DrawString("X",ButtonFont,ButtonTextBrush,B1,FormatCenter);
										

											if (MouseIn(B1,MenuButtonExpandP, MenuButtonExpandP))
											{
												//if (!InHitTest) graphics.DrawRectangle(ZoneH,B1);
											
												ClickedZoneBar = jjj;
												ClickedZoneTop = ZS.TopPrice;
												ClickedZoneBottom = ZS.BottomPrice;		
											}		
									}	
								}
								
	}
			
		private double GetBoxYPixel(double price, string type)
		{
			price = RTTS(price);
			if (PriceBoxes2.ContainsKey(price))
			{
				if (type == "Top")
				{
					return PriceBoxes2[price].Top;	
				}
				else if (type == "Bottom")
				{
					return PriceBoxes2[price].Bottom;
				}
				else if (type == "Height")
				{
					return PriceBoxes2[price].Height;
				}
				
				GetBoxYPixelStatus = true;
			}
			else
			{
				GetBoxYPixelStatus = false;
				//ChartControl.InvalidateVisual();
			}
			
			return 0;
			
			
			
		}
         
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

	// SampleAddonWindow2 removed — replaced by VeritasOrderFlowPropertyGridWindow at end of file

	
	
		
		private SharpDX.Direct2D1.PathGeometry CreateTriangleGeometry(ChartControl chartControl, ChartPanel chartPanel, ChartScale chartScale, int xt, double yt2, int dir, int szzzz)
		{
			

			Point startPoint			= new Point(0,0);
			Point midPoint				= new Point(100,0);
			Point endPoint 				= new Point(100,100);
				
			
			
			float aw = szzzz;
			
			int pArrowBarWidth = 0;
			int pArrowBarHeight = 0;
			int pArrowOffset = 0;
			
			
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

			if (dir == 1)
			{
				//yt = yt - offset;
				yt2f = yt2f - offset;

				int adx = 1;
				int ady = -1;
				
				// ask triangle isn't plotting same as the bid one
				
				tipPoint2 = new SharpDX.Vector2(xt+adx+aw-1,yt2f-ady);
				triLeftPoint2 = new SharpDX.Vector2(xt+adx,yt2f-aw+0-ady);
				triRightPoint2 = new SharpDX.Vector2(xt+adx,yt2f+aw-0-ady-1); // bottom
				
				tipPoint2 = new SharpDX.Vector2(xt+aw,yt2f);
				triLeftPoint2 = new SharpDX.Vector2(xt+1,yt2f-aw+1);
				triRightPoint2 = new SharpDX.Vector2(xt+1,yt2f+aw-1);				
				
                barLeftPoint12 = new SharpDX.Vector2(xt - aw2 + po, yt2f - aw);
                barRightPoint12 = new SharpDX.Vector2(xt + aw2 + po, yt2f - aw);
                barLeftPoint22 = new SharpDX.Vector2(xt - aw2 + po, yt2f - (aw + barh));
                barRightPoint22 = new SharpDX.Vector2(xt + aw2 + po, yt2f - (aw + barh));
			}
			else
			{
				//yt = yt + offset;
				yt2f = yt2f + offset;

				tipPoint2 = new SharpDX.Vector2(xt-aw,yt2f);
				triLeftPoint2 = new SharpDX.Vector2(xt,yt2f-aw);
				triRightPoint2 = new SharpDX.Vector2(xt,yt2f+aw);
				
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
			
				triLeftPoint2, triRightPoint2,
				triRightPoint2, tipPoint2,
				tipPoint2, triLeftPoint2
				
//				barLeftPoint12, barLeftPoint22,
//				barLeftPoint22, barRightPoint22,
//				barRightPoint22, barRightPoint12,
//				barRightPoint12, triRightPoint2,
				
			});
				
			geometrySink.EndFigure(SharpDX.Direct2D1.FigureEnd.Open);
			geometrySink.Close(); // calls dispose for you
			return pathGeometry;
			
		}


 	private string AllPriceMarker (double price)
		{
			double trunc = Math.Truncate(price);
			int fraction = 0;
			string priceMarker = "";
			if (ThisTickSizze == 0.03125) 
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
			else if (ThisTickSizze == 0.015625)
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
			else if (ThisTickSizze == 0.0078125)
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
				priceMarker = price.ToString(NinjaTrader.Core.Globals.GetTickFormatString(ThisTickSizze));
			}
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
		
		
		
		public override string FormatPriceMarker(double value)
		{
			if (ChartControl == null)
			{
				return value.ToString();
				
			}
			else
			{
				return AllPriceMarker(value);
			}
		
		
		}
		       
		private class SetIndicatorProperties : NTWindow
		{
			private Button					cancelButton;
			private InstrumentSelector		instrumentSelectorInput;
			private Instrument				instrumentValue;
			private Button					okButton;
			private QuantityUpDown			quantityUpDownInput;
			private TextBox					stringValueInput;
			
			private PropertyGrid propertyGrid;
			private IndicatorBase selectedIndicator;
			//public	Indicators.SetIndicatorValueFromAddonWindowExample	TargetIndicator;

			//public	Indicators.SetIndicatorValueFromAddonWindowExample	TargetIndicator;
			
			public SetIndicatorProperties()
			{
				Caption		= "VeritasOrderFlow " + "Properties";
				Width		= 400;
				Height		= 250;
				Height		= 10050;

				Loaded		+= OnWindow_Loaded;
				Closing		+= OnWindow_Close;
			}

			private void OnWindow_Loaded(object sender, RoutedEventArgs e)
			{
				//Content = LoadXaml();
			}

			private void OnWindow_Close(object sender, System.ComponentModel.CancelEventArgs e)
			{
				if (cancelButton != null)
					cancelButton.Click -= OnCancelButton_Click;

				if (instrumentSelectorInput != null)
					instrumentSelectorInput.InstrumentChanged -= OnInstrument_Changed;

				if (okButton != null)
					okButton.Click -= OnOkButton_Click;
			}

//			private DependencyObject LoadXaml()
//			{
//				Page page = new Page();
				
//				FileStream fs = new FileStream(System.IO.Path.Combine(NinjaTrader.Core.Globals.UserDataDir, @"bin\Custom\AddOns\SetIndicatorValueFromAddonWindowExampleToolsContent.xaml"), FileMode.Open);

//				page = (Page)XamlReader.Load(fs);

//				if (page == null)
//					return null;

//				cancelButton = LogicalTreeHelper.FindLogicalNode(page, "CancelButton") as Button;
//				if (cancelButton != null)
//					cancelButton.Click += OnCancelButton_Click;

//				okButton = LogicalTreeHelper.FindLogicalNode(page, "OkButton") as Button;
//				if (okButton != null)
//					okButton.Click += OnOkButton_Click;

//				instrumentSelectorInput = LogicalTreeHelper.FindLogicalNode(page, "InstrumentSelectorInput") as InstrumentSelector;
//				if (instrumentSelectorInput != null)
//					instrumentSelectorInput.InstrumentChanged += OnInstrument_Changed;

//				quantityUpDownInput		= LogicalTreeHelper.FindLogicalNode(page, "QuantityUpDownInput") as QuantityUpDown;
//				stringValueInput		= LogicalTreeHelper.FindLogicalNode(page, "StringValueInput") as TextBox;

//				DependencyObject pageContent = page.Content as DependencyObject;

//				return pageContent;
//			}

			
			private void ReloadIndicators()
			{
				foreach (Window window in Globals.AllWindows)
				{
					window.Dispatcher.BeginInvoke(new Action(() =>
					{
						Chart chart = window as Chart;

						if (chart != null)
						{
							TabControl tabControl = chart.FindFirst("ChartWindowTabControl") as TabControl;
							if (tabControl != null)
							{
								foreach (object dependencyObject in tabControl.Items)
								{
									TabItem tabItem = dependencyObject as TabItem;
									if (tabItem != null)
									{
										ChartTab chartTab = tabItem.Content as ChartTab;
										if (chartTab != null)
										{
											foreach (IndicatorRenderBase indicator in chartTab.ChartControl.Indicators)
											{
												if (indicator == null) return;
												propertyGrid.Dispatcher.InvokeAsync(() =>
												{
													propertyGrid.SelectedObject = indicator;
												});
												break;
											}
										}
									}
								}
							}
						}
					}));
				}
			}

			
			private void OnCancelButton_Click(object sender, RoutedEventArgs e)
			{
				this.Close();
			}

			private void OnInstrument_Changed(object sender, EventArgs e)
			{
				instrumentValue = sender as Cbi.Instrument;
			}

			private void OnOkButton_Click(object sender, RoutedEventArgs e)
			{
//				if (TargetIndicator == null)
//					return;

//				if (instrumentValue != null)
//					TargetIndicator.InstrumentValue = instrumentValue;

//				if (quantityUpDownInput != null)
//					TargetIndicator.IntValue		= quantityUpDownInput.Value;

//				if (stringValueInput != null)
//					TargetIndicator.StringValue		= stringValueInput.Text;

//				TargetIndicator.ShowValues();

				this.Close();
			}
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

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public Series<double> LongSignals
        {
            get { return Values[0]; }
        }		
		
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public Series<double> ShortSignals
        {
            get { return Values[1]; }
        }		
		
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public Series<double> CompPOC
        {
            get { return Values[2]; }
        }		
		
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public Series<double> CompVWAP
        {
            get { return Values[3]; }
        }		
		
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public Series<double> CompVAH1
        {
            get { return Values[4]; }
        }		
		
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public Series<double> CompVAL1
        {
            get { return Values[5]; }
        }		
				
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public Series<double> CompVAH2
        {
            get { return Values[6]; }
        }		
		
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public Series<double> CompVAL2
        {
            get { return Values[7]; }
        }		

		
		
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public Series<double> TotalVolume
        {
            get { return Values[8]; }
        }		
		
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public Series<double> BarVolume
        {
            get { return Values[9]; }
        }		
				
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public Series<double> TotalDelta
        {
            get { return Values[10]; }
        }		
		
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public Series<double> BarDelta
        {
            get { return Values[11]; }
        }		
		
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public Series<double> BarBid
        {
            get { return Values[12]; }
        }		
		
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public Series<double> BarAsk
        {
            get { return Values[13]; }
        }		

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public Series<double> BarDeltaPercent
        {
            get { return Values[14]; }
        }		
		
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public Series<double> BarBidPercent
        {
            get { return Values[15]; }
        }		
		
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public Series<double> BarAskPercent
        {
            get { return Values[16]; }
        }			
		
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public Series<double> CompDH
        {
            get { return Values[17]; }
        }		
		
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public Series<double> CompDL
        {
            get { return Values[18]; }
        }			
		
		
		
	
		
       // private Brush areaBrush = Brushes.DarkGray;
        private Brush textBrush = Brushes.Blue;
        private Brush smallAreaBrush = Brushes.Red;
       // private int areaOpacity = 50;
       // const float fontHeight = 30f;

		

		
		
        private bool pButtonsEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", GroupName = "Chart Buttons", Order = 0)]
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        public bool ButtonsEnabled
        {
            get { return pButtonsEnabled; }
            set { pButtonsEnabled = value; }
        }


		
//        private bool pShowBars = true;
//        [Display(ResourceType = typeof(Custom.Resource), Name = "Split Bid / Ask", GroupName = "Chart Buttons", Order = 1)]
//        public bool ShowBars
//        {
//            get { return pShowBars; }
//            set { pShowBars = value; }
//        }			
	
		
		
		private string pButtonHighlightMode = "Off";
		[Description("")]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Chart Buttons", Name = "Highlight Mode", Order = 2)]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(ButtonMode2))]
		public string ButtonHighlightMode
		{
			get { return pButtonHighlightMode; }
			set { pButtonHighlightMode = value; }
		}
		
		
		internal class ButtonMode2 : StringConverter
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
				return new StandardValuesCollection( new String[] {"On", "Off"} );
			}
		}	
		
		
	
		private Brush pColorButtonsOff = Brushes.DimGray;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Chart Buttons", Name = "Highlight Color", Order = 3)]
		public Brush ColorButtonsOff2
		{
			get { return pColorButtonsOff; } set { pColorButtonsOff = value; }
		}
		[Browsable(false)]
		public string ColorButtonsOff2S
		{
			get { return Serialize.BrushToString(pColorButtonsOff); } set { pColorButtonsOff = Serialize.StringToBrush(value); }
		}
		
		
		private int pOnOpacity = 80;
        [Range(0, 100)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Chart Buttons", Name = "Highlight Opacity (%)", Order = 4)]
        public int OnOpacity
        {
            get { return pOnOpacity; }
            set { pOnOpacity = value; }
        }

		private int pChartMenuTextSize = 1;
        [Range(0, 10)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Text Size", GroupName = "Chart Buttons", Order = 17)]
        public int ChartMenuTextSize
        {
            get { return pChartMenuTextSize; }
            set { pChartMenuTextSize = value; }
        }
		
		

		

		
        private int pButtonSize = 18;
        [Range(1, int.MaxValue)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Size (Pixels)", GroupName = "Chart Buttons", Order = 14)]
        public int ButtonSize
        {
            get { return pButtonSize; }
            set { pButtonSize = value; }
        }
		
        private int pSpaceBetweenButtons = 6;
        [Range(1, int.MaxValue)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Spacing (Pixels)", GroupName = "Chart Buttons", Order = 15)]
        public int SpaceBetweenButtons
        {
            get { return pSpaceBetweenButtons; }
            set { pSpaceBetweenButtons = value; }
        }		
		
		
	
//		private int pWidth1 = 8;
//        [Range(1, int.MaxValue), NinjaScriptProperty]
//        [Display(ResourceType = typeof(Custom.Resource), Name = "Width 1 (Pixels)", GroupName = "Parameters", Order = 1)]
//        public int Width1
//        {
//            get { return pWidth1; }
//            set { pWidth1 = value; }
//        }
		
//		private int pSpace1 = 3;
//        [Range(1, int.MaxValue), NinjaScriptProperty]
//        [Display(ResourceType = typeof(Custom.Resource), Name = "Space 1 (Pixels)", GroupName = "Parameters", Order = 2)]
//        public int Space1
//        {
//            get { return pSpace1; }
//            set { pSpace1 = value; }
//        }		
		

        private bool pAboveBarDisplayEnabled = true;
//        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Print Display", Name = "Above Bar Display Enabled", Order = 100)]
//		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
//        public bool AboveBarDisplayEnabled
//        {
//            get { return pAboveBarDisplayEnabled; }
//            set { pAboveBarDisplayEnabled = value; }
//        }		
		

		
		private string pAboveTotalMode = "Delta Percent";
		[Description("")]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Print Display", Name = "Above Bar Display Mode", Description = "",  Order = 101)]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(AboveMode))]
		public string AboveTotalMode
		{
			get { return pAboveTotalMode; }
			set { pAboveTotalMode = value; }
		}
		
		
		private string pBelowTotalMode = "Delta Percent";
		[Description("")]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Print Display", Name = "Below Bar Display Mode", Description = "",  Order = 110)]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(AboveMode))]
		public string BelowTotalMode
		{
			get { return pBelowTotalMode; }
			set { pBelowTotalMode = value; }
		}
		
		
		
		
		
		
		private string pPrintNumberDisplayMode = "Bid / Ask";
		[Description("")]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Print Display", Name = "Number Display Mode", Description = "",  Order = 6)]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(NumberDisplayMode2))]
		public string PrintNumberDisplayMode
		{
			get { return pPrintNumberDisplayMode; }
			set { pPrintNumberDisplayMode = value; }
		}
				

		
		private SimpleFont pTextFont4 = new SimpleFont("Consolas", 11);
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Print Display", Name = "Above / Below Bar Text Font", Description = "", Order = 112)]
		public SimpleFont TextFont4
        {
            get { return pTextFont4; }
            set { pTextFont4= value; }
        }	
			
		
		private Brush pColorAboveLong = Brushes.LimeGreen;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Print Display", Name = "Above / Below Bar Color +", Order = 113)]
		public Brush ColorAboveLong
		{
			get { return pColorAboveLong; } set { pColorAboveLong = value; }
		}
		[Browsable(false)]
		public string ColorAboveLongS
		{
			get { return Serialize.BrushToString(pColorAboveLong); } set { pColorAboveLong = Serialize.StringToBrush(value); }
		}		
		private Brush pColorAboveNeutral = Brushes.Silver;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Print Display", Name = "Above / Below Bar Color Neutral", Order = 114)]
		public Brush ColorAboveNeutral
		{
			get { return pColorAboveNeutral; } set { pColorAboveNeutral = value; }
		}
		[Browsable(false)]
		public string ColorAboveNeutralS
		{
			get { return Serialize.BrushToString(pColorAboveNeutral); } set { pColorAboveNeutral = Serialize.StringToBrush(value); }
		}		
		
		private Brush pColorAboveShort = Brushes.Red;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Print Display", Name = "Above / Below Bar Color -", Order = 115)]
		public Brush ColorAboveShort
		{
			get { return pColorAboveShort; } set { pColorAboveShort = value; }
		}
		[Browsable(false)]
		public string ColorAboveShortS
		{
			get { return Serialize.BrushToString(pColorAboveShort); } set { pColorAboveShort = Serialize.StringToBrush(value); }
		}		
		
		private int pAboveBarYOffset = 5;
        [Range(-1000, 1000)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Print Display", Name = "Above / Below Bar Y Offset (Pixels)", Order = 116)]
        public int AboveBarYOffset
        {
            get { return pAboveBarYOffset; }
            set { pAboveBarYOffset = value; }
        }	
		

		internal class AboveMode : StringConverter
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
				return new StandardValuesCollection( new String[] {"Delta Total", "Delta Percent", "None" } );
			}
		}	
		
		internal class NumberDisplayMode2 : StringConverter
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
				return new StandardValuesCollection( new String[] {"Bid / Ask", "Delta", "Volume" } );
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
				return new StandardValuesCollection( new String[] {"Contracts", "Percent", "Both"} );
			}
		}	
		
		
	

		
		private bool pPrintEnabled = true;
//        [Display(ResourceType = typeof(Custom.Resource), Name = "Print Enabled", GroupName = "Print Display", Order = 0)]
//		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
//        public bool PrintEnabled
//        {
//            get { return pPrintEnabled; }
//            set { pPrintEnabled = value; }
//        }	

			
        private bool pCrossHairEnabled = false;
		[Description("show mouse crosshair.")]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Crosshair Display", Name = "Enabled", Order = 110)]
		[RefreshProperties(RefreshProperties.All)]
        public bool CrossHairEnabled
        {
            get { return pCrossHairEnabled; }
            set { pCrossHairEnabled = value; }
        }
		
        private bool pCrossHairXEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Crosshair Display", Name = "X Enabled", Order = 111)]
        public bool CrossHairXEnabled
        {
            get { return pCrossHairXEnabled; }
            set { pCrossHairXEnabled = value; }
        }
		private bool pCrossHairYEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Crosshair Display", Name = "Y Enabled", Order = 112)]
        public bool CrossHairYEnabled
        {
            get { return pCrossHairYEnabled; }
            set { pCrossHairYEnabled = value; }
        }
		
		private Brush pCrossHairBrush = Brushes.Gray;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Crosshair Display", Name = "Box Color", Order = 113)]
		public Brush CrossHairBrush
		{
			get { return pCrossHairBrush; } set { pCrossHairBrush = value; }
		}
		[Browsable(false)]
		public string CrossHairBrushS
		{
			get { return Serialize.BrushToString(pCrossHairBrush); } set { pCrossHairBrush = Serialize.StringToBrush(value); }
		}	
		
		private int pCrossHairOpacity = 10;
        [Range(0, 100)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Crosshair Display", Name = "Box Opacity (%)", Order = 114)]
        public int CrossHairOpacity
        {
            get { return pCrossHairOpacity; }
            set { pCrossHairOpacity = value; }
        }

		private string pVersionInfo = "26. 5. 21. 1";
		// No [ReadOnly(true)] — the custom window's standalone PropertyGrid hides read-only props; the empty setter locks the value instead.
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Details", Name = "Version", Order = 0)]
		public string VersionInfo
		{
			get { return pVersionInfo; }
			set { }
		}

		private string pCrossHairStyle = "Box";
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Crosshair Display", Name = "Style", Order = 115)]
        [TypeConverter(typeof(CrossHairStyleMode))]
        public string CrossHairStyle
        {
            get { return pCrossHairStyle; }
            set { pCrossHairStyle = value; }
        }

		internal class CrossHairStyleMode : StringConverter
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
				return new StandardValuesCollection( new String[] {"Box", "Line", "Line + Box"} );
			}
		}

		// Stacked Imbalance Detection (bonus feature)
		private bool pStackedImbalanceEnabled = false;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Print Display", Name = "Stacked Imbalances", Order = 35)]
        public bool StackedImbalanceEnabled
        {
            get { return pStackedImbalanceEnabled; }
            set { pStackedImbalanceEnabled = value; }
        }

		private int pStackedImbalanceCount = 3;
        [Range(2, 10)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Print Display", Name = "Stacked Imb Min Count", Order = 36)]
        public int StackedImbalanceCount
        {
            get { return pStackedImbalanceCount; }
            set { pStackedImbalanceCount = value; }
        }

		private Brush pStackedImbalanceBidColor = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(180, 255, 69, 0));
        [XmlIgnore]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Print Display", Name = "Stacked Imb Bid Color", Order = 37)]
        public Brush StackedImbalanceBidColor
        {
            get { return pStackedImbalanceBidColor; }
            set { pStackedImbalanceBidColor = value; }
        }
		[Browsable(false)]
		public string StackedImbalanceBidColorS
		{
			get { return Serialize.BrushToString(pStackedImbalanceBidColor); }
			set { pStackedImbalanceBidColor = Serialize.StringToBrush(value); }
		}

		private Brush pStackedImbalanceAskColor = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(180, 0, 191, 255));
        [XmlIgnore]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Print Display", Name = "Stacked Imb Ask Color", Order = 38)]
        public Brush StackedImbalanceAskColor
        {
            get { return pStackedImbalanceAskColor; }
            set { pStackedImbalanceAskColor = value; }
        }
		[Browsable(false)]
		public string StackedImbalanceAskColorS
		{
			get { return Serialize.BrushToString(pStackedImbalanceAskColor); }
			set { pStackedImbalanceAskColor = Serialize.StringToBrush(value); }
		}

//        private Stroke pBarOutlineStroke = new Stroke(Brushes.DimGray, DashStyleHelper.Solid, 1);
//        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Print Display", Name = "Bar Outline Display", Order = 39)]
//        public Stroke BarOutlineStroke
//        {
//            get { return pBarOutlineStroke; }
//            set { pBarOutlineStroke = value; }
//        }	
		
        private Stroke pBarUpOutlineStroke = new Stroke(Brushes.Green, DashStyleHelper.Solid, 2);
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Print Display", Name = "Candlestick Up Outline Display", Order = 39)]
        public Stroke BarUpOutlineStroke
        {
            get { return pBarUpOutlineStroke; }
            set { pBarUpOutlineStroke = value; }
        }		
		
        private Stroke pBarDnOutlineStroke = new Stroke(Brushes.Red, DashStyleHelper.Solid, 2);
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Print Display", Name = "Candlestick Down Outline Display", Order = 40)]
        public Stroke BarDnOutlineStroke
        {
            get { return pBarDnOutlineStroke; }
            set { pBarDnOutlineStroke = value; }
        }		
		
		private string pBarBodyMode = "Middle";
		[Description("")]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Print Display", Name = "Body Bar Location", Description = "",  Order = 80)]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(BodyMode))]
		public string BarBodyMode
		{
			get { return pBarBodyMode; }
			set { pBarBodyMode = value; }
		}

		internal class BodyMode : StringConverter
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
				return new StandardValuesCollection( new String[] {"Left", "Right", "Middle", "None"} );
			}
		}	
		
		
        private Stroke pBarBodyUpStroke = new Stroke(Brushes.LimeGreen, DashStyleHelper.Solid, 3);
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Print Display", Name = "Body Bar Up Display", Order = 81)]
        public Stroke BarBodyUpStroke
        {
            get { return pBarBodyUpStroke; }
            set { pBarBodyUpStroke = value; }
        }	
				
        private Stroke pBarBodyDownStroke = new Stroke(Brushes.Red, DashStyleHelper.Solid, 3);
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Print Display", Name = "Body Bar Down Display", Order = 82)]
        public Stroke BarBodyDownStroke
        {
            get { return pBarBodyDownStroke; }
            set { pBarBodyDownStroke = value; }
        }	
		
	internal class ColorMode : StringConverter
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
				return new StandardValuesCollection( new String[] {"Delta", "Candlestick"} );
			}
		}	
		
		private string pPrintBarFillMode = "Delta";
		[Description("")]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Print Display", Name = "Primary Color Mode", Description = "",  Order = 0)]
		//[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(ColorMode))]
		public string PrintBarFillMode
		{
			get { return pPrintBarFillMode; }
			set { pPrintBarFillMode = value; }
		}

		internal class OpacityMode : StringConverter
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
				return new StandardValuesCollection( new String[] {"Volume", "None"} );
			}
		}	
		
		private string pPrintBarOpacityMode = "Volume";
		[Description("")]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Print Display", Name = "Opacity Mode", Description = "",  Order = 1)]
		//[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(OpacityMode))]
		public string PrintBarOpacityMode
		{
			get { return pPrintBarOpacityMode; }
			set { pPrintBarOpacityMode = value; }
		}
		
		private SimpleFont pTextFont2 = new SimpleFont("Consolas", 11);
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Print Display", Name = "Text Font", Description = "", Order = 2)]
		public SimpleFont TextFont2
        {
            get { return pTextFont2; }
            set { pTextFont2= value; }
        }	
			
		private Brush pMainTextColor = Brushes.WhiteSmoke;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Print Display", Name = "Text Color", Order = 3)]
		public Brush MainTextColor
		{
			get { return pMainTextColor; } set { pMainTextColor = value; }
		}
		[Browsable(false)]
		public string MainTextColorS
		{
			get { return Serialize.BrushToString(pMainTextColor); } set { pMainTextColor = Serialize.StringToBrush(value); }
		}			

		
		private bool pPrintEnabled2 = true;
//        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Print Display", Name = "Numbers Enabled", Order = 4)]
//		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
//        public bool PrintEnabled2
//        {
//            get { return pPrintEnabled2; }
//            set { pPrintEnabled2 = value; }
//        }	
			
		
        private bool pSplit = true;
//        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Print Display", Name = "Split Bid / Ask", Order = 5)]
//        public bool Split
//        {
//            get { return pSplit; }
//            set { pSplit = value; }
//        }		
		
	
		
		private bool pUseBackgroundColor = false;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Print Display", Name = "Color Main", Order = 20)]
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        public bool UseBackgroundColor
        {
            get { return pUseBackgroundColor; }
            set { pUseBackgroundColor = value; }
        }

		
		private Brush pColorStatus2 = Brushes.WhiteSmoke;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Print Display", Name = "Color Main", Order = 21)]
		public Brush ColorStatus2
		{
			get { return pColorStatus2; } set { pColorStatus2 = value; }
		}
		[Browsable(false)]
		public string ColorStatus2S
		{
			get { return Serialize.BrushToString(pColorStatus2); } set { pColorStatus2 = Serialize.StringToBrush(value); }
		}	
		
		private Brush pColorPrintShort = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(204, 65, 65));
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Print Display", Name = "Color Bid", Order = 22)]
		public Brush ColorPrintShort
		{
			get { return pColorPrintShort; } set { pColorPrintShort = value; }
		}
		[Browsable(false)]
		public string ColorPrintShortS
		{
			get { return Serialize.BrushToString(pColorPrintShort); } set { pColorPrintShort = Serialize.StringToBrush(value); }
		}	
		
		private Brush pColorPrintLong = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(38, 166, 91));
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Print Display", Name = "Color Ask", Order = 23)]
		public Brush ColorPrintLong
		{
			get { return pColorPrintLong; } set { pColorPrintLong = value; }
		}
		[Browsable(false)]
		public string ColorPrintLongS
		{
			get { return Serialize.BrushToString(pColorPrintLong); } set { pColorPrintLong = Serialize.StringToBrush(value); }
		}		
		
		private int pPrintMinOpacity = 20;
        [Range(0, 100)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Print Display", Name = "Minimum Opacity (%)", Order = 24)]
        public int PrintMinOpacity
        {
            get { return pPrintMinOpacity; }
            set { pPrintMinOpacity = value; }
        }					

		
		private int pPrintMaxOpacity = 90;
        [Range(0, 100)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Print Display", Name = "Maximum Opacity (%)", Order = 26)]
        public int PrintMaxOpacity
        {
            get { return pPrintMaxOpacity; }
            set { pPrintMaxOpacity = value; }
        }

		// --- Visual Overhaul Properties ---

		private double pHeatmapGamma = 1.5;
        [Range(0.5, 3.0)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Print Display", Name = "Heatmap Gamma", Order = 27)]
        public double HeatmapGamma
        {
            get { return pHeatmapGamma; }
            set { pHeatmapGamma = value; }
        }



		private bool pTextShadowEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Print Display", Name = "Text Shadow", Order = 4)]
        public bool TextShadowEnabled
        {
            get { return pTextShadowEnabled; }
            set { pTextShadowEnabled = value; }
        }

		private bool pAdaptiveTextColor = false;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Print Display", Name = "Adaptive Text Color", Order = 5)]
        public bool AdaptiveTextColor
        {
            get { return pAdaptiveTextColor; }
            set { pAdaptiveTextColor = value; }
        }

        private bool pShowLastPriceAll = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Last Price Display", Name = "Enabled", Order = 0)]
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        public bool ShowLastPriceAll
        {
            get { return pShowLastPriceAll; }
            set { pShowLastPriceAll = value; }
        }			

		private bool pShowLastPriceBox = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Last Price Display", Name = "Box Enabled", Order = 30)]
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        public bool ShowLastPriceBox
        {
            get { return pShowLastPriceBox; }
            set { pShowLastPriceBox = value; }
        }		
		
		private Brush pLastPriceBrush = Brushes.Goldenrod;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Last Price Display", Name = "Box Color", Order = 31)]
		public Brush LastPriceBrush
		{
			get { return pLastPriceBrush; } set { pLastPriceBrush = value; }
		}
		[Browsable(false)]
		public string LastPriceBrushS
		{
			get { return Serialize.BrushToString(pLastPriceBrush); } set { pLastPriceBrush = Serialize.StringToBrush(value); }
		}	
		
		private int pLastPriceOpacity = 20;
        [Range(0, 100)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Last Price Display", Name = "Box Opacity (%)", Order = 32)]
        public int LastPriceOpacity
        {
            get { return pLastPriceOpacity; }
            set { pLastPriceOpacity = value; }
        }	
				
		private string pLastPriceStyle = "Box";
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Last Price Display", Name = "Style", Order = 33)]
        [TypeConverter(typeof(LastPriceStyleMode))]
        public string LastPriceStyle
        {
            get { return pLastPriceStyle; }
            set { pLastPriceStyle = value; }
        }

		internal class LastPriceStyleMode : StringConverter
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
				return new StandardValuesCollection( new String[] {"Box", "Line + Label"} );
			}
		}

        private bool pShowLastPriceLine = false;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Last Price Display", Name = "Hash Enabled", Order = 35)]
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        public bool ShowLastPriceLine
        {
            get { return pShowLastPriceLine; }
            set { pShowLastPriceLine = value; }
        }	
		
        private Stroke pLastPriceLineStroke = new Stroke(Brushes.White, DashStyleHelper.Solid, 2);
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Last Price Display", Name = "Hash Display", Order = 36)]
        public Stroke LastPriceLineStroke
        {
            get { return pLastPriceLineStroke; }
            set { pLastPriceLineStroke = value; }
        }	
		
        private bool pShowLastPriceMarker2 = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Last Price Display", Name = "Marker Enabled", Order = 37)]
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        public bool ShowLastPriceMarker2
        {
            get { return pShowLastPriceMarker2; }
            set { pShowLastPriceMarker2 = value; }
        }	
		
        
				
		private Brush pColorLastPriceMarker = Brushes.Goldenrod;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Last Price Display", Name = "Marker Color", Order = 38)]
		public Brush ColorLastPriceMarker
		{
			get { return pColorLastPriceMarker; } set { pColorLastPriceMarker = value; }
		}
		[Browsable(false)]
		public string ColorLastPriceMarkerS
		{
			get { return Serialize.BrushToString(pColorLastPriceMarker); } set { pColorLastPriceMarker = Serialize.StringToBrush(value); }
		}		
		
		
		
       

		

		
	
        private bool pShowBlocks = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Block Trades", Name = "Enabled", Order = 0)]
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        public bool ShowBlocks
        {
            get { return pShowBlocks; }
            set { pShowBlocks = value; }
        }		
		
		
		private int pBlockSize = 20;
        [Range(1, int.MaxValue), NinjaScriptProperty]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Parameters", Name = "Block Trades - Minimum Volume", Order = -10)]
        public int BlockSize
        {
            get { return pBlockSize; }
            set { pBlockSize = value; }
        }	
		
		
		private int pTriSize = 5;
        [Range(1, int.MaxValue), NinjaScriptProperty]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Block Trades", Name = "Triangle Size", Order = 1)]
        public int TriSize
        {
            get { return pTriSize; }
            set { pTriSize = value; }
        }	
				
		private Brush pColorBidTri = Brushes.WhiteSmoke;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Block Trades", Name = "Bid Color", Order = 2)]
		public Brush ColorBidTri
		{
			get { return pColorBidTri; } set { pColorBidTri = value; }
		}
		[Browsable(false)]
		public string ColorBidTriS
		{
			get { return Serialize.BrushToString(pColorBidTri); } set { pColorBidTri = Serialize.StringToBrush(value); }
		}	
	
		private Brush pColorAskTri = Brushes.WhiteSmoke;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Block Trades", Name = "Ask Color", Order = 3)]
		public Brush ColorAskTri
		{
			get { return pColorAskTri; } set { pColorAskTri = value; }
		}
		[Browsable(false)]
		public string ColorAskTriS
		{
			get { return Serialize.BrushToString(pColorAskTri); } set { pColorAskTri = Serialize.StringToBrush(value); }
		}	
		
		private bool pShowBlocksHover = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Block Trades", Name = "Hover Details Enabled", Order = 10, Description= "enable hover event to show trade details for each marker")]
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        public bool ShowBlocksHover
        {
            get { return pShowBlocksHover; }
            set { pShowBlocksHover = value; }
        }		
		
		
		
		
				
		private int pLightV = 10;
//        [Range(0, int.MaxValue)]
//        [Display(ResourceType = typeof(Custom.Resource), GroupName = "New Settings", Name = "Light Volume - Maximum", Order = 11)]
//        public int LightV
//        {
//            get { return pLightV; }
//            set { pLightV = value; }
//        }
		
		
		private Brush pColorLV = Brushes.Gold;
//		[XmlIgnore]
//		[Display(ResourceType = typeof(Custom.Resource), GroupName = "New Settings", Name = "Light Volume Color", Order = 23)]
//		public Brush ColorLV
//		{
//			get { return pColorLV; } set { pColorLV = value; }
//		}
//		[Browsable(false)]
//		public string ColorLVS
//		{
//			get { return Serialize.BrushToString(pColorLV); } set { pColorLV = Serialize.StringToBrush(value); }
//		}			
		
		private bool pPrintFillLV = false;
//        [Display(ResourceType = typeof(Custom.Resource), GroupName = "New Settings", Name = "Light Volume Fill Enabled", Order = 40)]
//		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
//        public bool PrintFillLV
//        {
//            get { return pPrintFillLV; }
//            set { pPrintFillLV = value; }
//        }	
		
		
		
		
		
			        
		private bool pShowImbalance = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Bid / Ask Imbalances", Name = "Enabled", Order = 0)]
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        public bool ShowImbalance
        {
            get { return pShowImbalance; }
            set { pShowImbalance = value; }
        }	
		
		private bool pShowImbalanceA = false;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Bid / Ask Imbalances", Name = "Dots Enabled", Order = 10)]
        public bool ShowImbalanceA
        {
            get { return pShowImbalanceA; }
            set { pShowImbalanceA = value; }
        }	
		
		private int pRSSize = 4;
        [Range(1, int.MaxValue)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Bid / Ask Imbalances", Name = "Dots Size", Order = 11)]
        public int RSSize
        {
            get { return pRSSize; }
            set { pRSSize = value; }
        }
		
		
		private Brush pColorBidDots = Brushes.White;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Bid / Ask Imbalances", Name = "Dots Bid Color", Order = 12)]
		public Brush ColorBidDots
		{
			get { return pColorBidDots; } set { pColorBidDots = value; }
		}
		[Browsable(false)]
		public string ColorBidDotsS
		{
			get { return Serialize.BrushToString(pColorBidDots); } set { pColorBidDots = Serialize.StringToBrush(value); }
		}	
		
		private Brush pColorAskDots = Brushes.LimeGreen;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Bid / Ask Imbalances", Name = "Dots Ask Color", Order = 13)]
		public Brush ColorAskDots
		{
			get { return pColorAskDots; } set { pColorAskDots = value; }
		}
		[Browsable(false)]
		public string ColorAskDotsS
		{
			get { return Serialize.BrushToString(pColorAskDots); } set { pColorAskDots = Serialize.StringToBrush(value); }
		}					
		
		
		private bool pShowImbalanceHash = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Bid / Ask Imbalances", Name = "Hash Enabled", Order = 20)]
        public bool ShowImbalanceHash
        {
            get { return pShowImbalanceHash; }
            set { pShowImbalanceHash = value; }
        }	
		
		private int pRSSize2 = 1;
        [Range(1, int.MaxValue)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Bid / Ask Imbalances", Name = "Hash Size", Order = 21)]
        public int RSSize2
        {
            get { return pRSSize2; }
            set { pRSSize2 = value; }
        }
		
		
		private Brush pColorBidHash = Brushes.White;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Bid / Ask Imbalances", Name = "Hash Bid Color", Order = 22)]
		public Brush ColorBidHash
		{
			get { return pColorBidHash; } set { pColorBidHash = value; }
		}
		[Browsable(false)]
		public string ColorBidHashS
		{
			get { return Serialize.BrushToString(pColorBidHash); } set { pColorBidHash = Serialize.StringToBrush(value); }
		}	
		
		private Brush pColorAskHash = Brushes.White;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Bid / Ask Imbalances", Name = "Hash Ask Color", Order = 23)]
		public Brush ColorAskHash
		{
			get { return pColorAskHash; } set { pColorAskHash = value; }
		}
		[Browsable(false)]
		public string ColorAskHashS
		{
			get { return Serialize.BrushToString(pColorAskHash); } set { pColorAskHash = Serialize.StringToBrush(value); }
		}			
		
		private bool pPrintFillImbalances = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Bid / Ask Imbalances", Name = "Fill Enabled", Order = 40)]
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        public bool PrintFillImbalances
        {
            get { return pPrintFillImbalances; }
            set { pPrintFillImbalances = value; }
        }	
		
		private Brush pColorBidFill = Brushes.Red;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Bid / Ask Imbalances", Name = "Fill Bid Color", Order = 41)]
		public Brush ColorBidFill
		{
			get { return pColorBidFill; } set { pColorBidFill = value; }
		}
		[Browsable(false)]
		public string ColorBidFillS
		{
			get { return Serialize.BrushToString(pColorBidFill); } set { pColorBidFill = Serialize.StringToBrush(value); }
		}	
		
		private Brush pColorAskFill = Brushes.Green;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Bid / Ask Imbalances", Name = "Fill Ask Color", Order = 42)]
		public Brush ColorAskFill
		{
			get { return pColorAskFill; } set { pColorAskFill = value; }
		}
		[Browsable(false)]
		public string ColorAskFillS
		{
			get { return Serialize.BrushToString(pColorAskFill); } set { pColorAskFill = Serialize.StringToBrush(value); }
		}			
		
		
		
		
		private bool pPrintTextImbalances = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Bid / Ask Imbalances", Name = "Text Enabled", Order = 50)]
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        public bool PrintTextImbalances
        {
            get { return pPrintTextImbalances; }
            set { pPrintTextImbalances = value; }
        }		
	
		private SimpleFont pTextFont2Imb = new SimpleFont("Consolas", 14);
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Bid / Ask Imbalances", Name = "Text Font", Description = "", Order = 51)]
		public SimpleFont TextFont2Imb
        {
            get { return pTextFont2Imb; }
            set { pTextFont2Imb = value; }
        }	
					
		
		private Brush pColorBidText = Brushes.WhiteSmoke;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Bid / Ask Imbalances", Name = "Text Bid Color", Order = 52)]
		public Brush ColorBidText
		{
			get { return pColorBidText; } set { pColorBidText = value; }
		}
		[Browsable(false)]
		public string ColorBidTextS
		{
			get { return Serialize.BrushToString(pColorBidText); } set { pColorBidText = Serialize.StringToBrush(value); }
		}	
		
		private Brush pColorAskText = Brushes.WhiteSmoke;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Bid / Ask Imbalances", Name = "Text Ask Color", Order = 53)]
		public Brush ColorAskText
		{
			get { return pColorAskText; } set { pColorAskText = value; }
		}
		[Browsable(false)]
		public string ColorAskTextS
		{
			get { return Serialize.BrushToString(pColorAskText); } set { pColorAskText = Serialize.StringToBrush(value); }
		}			
		
		
		
		private int pClusterSize = 3;
        [Range(1, 10000)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Parameters", Name = "Volume Profile (Bar) - Cluster Size (Ticks)", Order = 70, Description = "")]
        public int ClusterSize
        {
            get { return pClusterSize; }
            set { pClusterSize = value; }
        }	
				
	

		
		
		private int pTextAdjustImbalances = 2;
//        [Range(0, int.MaxValue), NinjaScriptProperty]
//        [Display(ResourceType = typeof(Custom.Resource), Name = "Text Adjustment", GroupName = "Parameters", Order = 13, Description = "")]
//        public int TextAdjustImbalances
//        {
//            get { return pTextAdjustImbalances; }
//            set { pTextAdjustImbalances = value; }
//        }			
		
		
		private int pNDThreshold = 5;
        [Range(0, int.MaxValue), NinjaScriptProperty]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Net Delta Threshold (%)", GroupName = "Parameters", Order = 0, Description = "required % volume threshold to display the long or short colors.")]
        public int NDThreshold
        {
            get { return pNDThreshold; }
            set { pNDThreshold = value; }
        }		
		
		private int pNDThreshold2 = 100;
        [Range(0, int.MaxValue), NinjaScriptProperty]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Net Delta Threshold", GroupName = "Parameters", Order = 0, Description = "required volume threshold to display the long or short colors.")]
        public int NDThreshold2
        {
            get { return pNDThreshold2; }
            set { pNDThreshold2 = value; }
        }		
		
		private int pAllTradesMin = 1;
        [Range(1, int.MaxValue), NinjaScriptProperty]
        [Display(ResourceType = typeof(Custom.Resource), Name = "All Trades - Minimum Volume", GroupName = "Parameters", Order = -20, Description = "minimum volume to include a trade in the order flow and profile calculations.")]
        public int AllTradesMin
        {
            get { return pAllTradesMin; }
            set { pAllTradesMin = value; }
        }
		
		private double pImbalanceOffset = 2.0;
        [Range(1, double.MaxValue), NinjaScriptProperty]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Bid / Ask Imbalances - Multiplier (x)", GroupName = "Parameters", Order = -1)]
        public double ImbalanceOffset
        {
            get { return pImbalanceOffset; }
            set { pImbalanceOffset = value; }
        }
		
		private int pMinimumImbDelta = 0;
        [Range(0, int.MaxValue), NinjaScriptProperty]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Bid / Ask Imbalances - Minimum Delta", GroupName = "Parameters", Order = 0)]
        public int MinimumImbDelta
        {
            get { return pMinimumImbDelta; }
            set { pMinimumImbDelta = value; }
        }		
		
		
		
		
		
		private double pVolumeQualifier = 0.0;
//        [Range(0, 100), NinjaScriptProperty]
//        [Display(ResourceType = typeof(Custom.Resource), Name = "Bid / Ask Imbalances - Volume Qualifier (%)", GroupName = "Parameters", Order = 0)]
//        public double VolumeQualifier
//        {
//            get { return pVolumeQualifier; }
//            set { pVolumeQualifier = value; }
//        }		
		
			
		
		private int pMinZWidth = 3;
        [Range(1, int.MaxValue), NinjaScriptProperty]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Zones Imbalances - Min Width (Ticks)", GroupName = "Parameters", Order = 100)]
        public int MinZWidth
        {
            get { return pMinZWidth; }
            set { pMinZWidth = value; }
        }		
		
		private int pMinZWidth2 = 3;
        [Range(1, int.MaxValue), NinjaScriptProperty]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Zones Momentum - Min Width (Ticks)", GroupName = "Parameters", Order = 101)]
        public int MinZWidth2
        {
            get { return pMinZWidth2; }
            set { pMinZWidth2 = value; }
        }	
		
		
		
		  
		private int pScaleMarginTicks = 0;

		private int minImbalance = 0;
		
		
		
	
		private bool pExtendZonesRight = true;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Extend Right", GroupName = "Zones Display", Order = 100)]
        public bool ExtendZonesRight
        {
            get { return pExtendZonesRight; }
            set { pExtendZonesRight = value; }
        }		
		        
		private bool pShowFreshZones = true;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Show Fresh Zones", GroupName = "Zones Display", Order = 0)]
        public bool ShowFreshZones
        {
            get { return pShowFreshZones; }
            set { pShowFreshZones = value; }
        }	
		
        private bool pShowTestedZones = true;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Show Tested Zones", GroupName = "Zones Display", Order = 1)]
        public bool ShowTestedZones
        {
            get { return pShowTestedZones; }
            set { pShowTestedZones = value; }
        }	
		
        private bool pShowBrokenZones = true;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Show Broken Zones", GroupName = "Zones Display", Order = 2)]
        public bool ShowBrokenZones
        {
            get { return pShowBrokenZones; }
            set { pShowBrokenZones = value; }
        }	
		
        private bool pZonesEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Zones (Imbalance) Enabled", GroupName = "Zones Display", Order = -10)]
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        public bool ZonesEnabled
        {
            get { return pZonesEnabled; }
            set { pZonesEnabled = value; }
        }	
		
        private bool pZonesEnabled2 = false;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Zones (Momentum) Enabled", GroupName = "Zones Display", Order = -10)]
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        public bool ZonesEnabled2
        {
            get { return pZonesEnabled2; }
            set { pZonesEnabled2 = value; }
        }			

		
        private bool pZonesTMEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Tested Marker Enabled", GroupName = "Zones Display", Order = 30)]
        public bool ZonesTMEnabled
        {
            get { return pZonesTMEnabled; }
            set { pZonesTMEnabled = value; }
        }	
		
        private bool pZonesTSEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Tested Shading Enabled", GroupName = "Zones Display", Order = 31)]
        public bool ZonesTSEnabled
        {
            get { return pZonesTSEnabled; }
            set { pZonesTSEnabled = value; }
        }	
		
        private Stroke pTMSupport = new Stroke(Brushes.Black, DashStyleHelper.Solid, 1);
        [Display(ResourceType = typeof(Custom.Resource), Name = "Tested Marker Support", GroupName = "Zones Display", Order = 32)]
        public Stroke TMSupport
        {
            get { return pTMSupport; }
            set { pTMSupport = value; }
        }	
		
        private Stroke pTMResistance = new Stroke(Brushes.Black, DashStyleHelper.Solid, 1);
        [Display(ResourceType = typeof(Custom.Resource), Name = "Tested Marker Resistance", GroupName = "Zones Display", Order = 33)]
        public Stroke TMResistance
        {
            get { return pTMResistance; }
            set { pTMResistance = value; }
        }	
		
        private Stroke pONESupport = new Stroke(Brushes.Black, DashStyleHelper.Solid, 2);
        [Display(ResourceType = typeof(Custom.Resource), Name = "1 Tick Level Display", GroupName = "Zones Display", Order = 40)]
        public Stroke ONESupport
        {
            get { return pONESupport; }
            set { pONESupport = value; }
        }	
		
//        private Stroke pONEResistance = new Stroke(Brushes.Red, DashStyleHelper.Solid, 2);
//        [Display(ResourceType = typeof(Custom.Resource), Name = "1 Tick Resistance Level Display", GroupName = "Zones Display", Order = 40)]
//        public Stroke ONEResistance
//        {
//            get { return pONEResistance; }
//            set { pONEResistance = value; }
//        }	
		
		
		private Brush pResistanceZColor1 = Brushes.Red;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Resistance Fresh Fill Color", GroupName = "Zones Display", Order = 20)]
		public Brush ResistanceZColor1
		{
			get { return pResistanceZColor1; } set { pResistanceZColor1 = value; }
		}
		[Browsable(false)]
		public string ResistanceZColor1S
		{
			get { return Serialize.BrushToString(pResistanceZColor1); } set { pResistanceZColor1 = Serialize.StringToBrush(value); }
		}		
		
		private Brush pResistanceZColor2 = Brushes.Red;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Resistance Tested Fill Color", GroupName = "Zones Display", Order = 22)]
		public Brush ResistanceZColor2
		{
			get { return pResistanceZColor2; } set { pResistanceZColor2 = value; }
		}
		[Browsable(false)]
		public string ResistanceZColor2S
		{
			get { return Serialize.BrushToString(pResistanceZColor2); } set { pResistanceZColor2 = Serialize.StringToBrush(value); }
		}
		
		private Brush pResistanceZColor3 = Brushes.Gray;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Resistance Broken Fill Color", GroupName = "Zones Display", Order = 24)]
		public Brush ResistanceZColor3
		{
			get { return pResistanceZColor3; } set { pResistanceZColor3 = value; }
		}
		[Browsable(false)]
		public string ResistanceZColor3S
		{
			get { return Serialize.BrushToString(pResistanceZColor3); } set { pResistanceZColor3 = Serialize.StringToBrush(value); }
		}
		
		private Brush pSupportZColor1 = Brushes.ForestGreen;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Support Fresh Fill Color", GroupName = "Zones Display", Order = 10)]
		public Brush SupportZColor1
		{
			get { return pSupportZColor1; } set { pSupportZColor1 = value; }
		}
		[Browsable(false)]
		public string SupportZColor1S
		{
			get { return Serialize.BrushToString(pSupportZColor1); } set { pSupportZColor1 = Serialize.StringToBrush(value); }
		}		
		
		private Brush pSupportZColor2 = Brushes.ForestGreen;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Support Tested Fill Color", GroupName = "Zones Display", Order = 12)]
		public Brush SupportZColor2
		{
			get { return pSupportZColor2; } set { pSupportZColor2 = value; }
		}
		[Browsable(false)]
		public string SupportZColor2S
		{
			get { return Serialize.BrushToString(pSupportZColor2); } set { pSupportZColor2 = Serialize.StringToBrush(value); }
		}
		
		private Brush pSupportZColor3 = Brushes.Gray;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Support Broken Fill Color", GroupName = "Zones Display", Order = 14)]
		public Brush SupportZColor3
		{
			get { return pSupportZColor3; } set { pSupportZColor3 = value; }
		}
		[Browsable(false)]
		public string SupportZColor3S
		{
			get { return Serialize.BrushToString(pSupportZColor3); } set { pSupportZColor3 = Serialize.StringToBrush(value); }
		}
		
		private int pResistanceZOpacity1 = 30;
        [Range(0, 100)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Resistance Fresh Fill Opacity (%)", GroupName = "Zones Display", Order = 21)]
        public int ResistanceZOpacity1
        {
            get { return pResistanceZOpacity1; }
            set { pResistanceZOpacity1 = value; }
        }	
		
		private int pResistanceZOpacity2 = 10;
        [Range(0, 100)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Resistance Tested Fill Opacity (%)", GroupName = "Zones Display", Order = 23)]
        public int ResistanceZOpacity2
        {
            get { return pResistanceZOpacity2; }
            set { pResistanceZOpacity2 = value; }
        }	
		
		private int pResistanceZOpacity3 = 10;
        [Range(0, 100)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Resistance Broken Fill Opacity (%)", GroupName = "Zones Display", Order = 25)]
        public int ResistanceZOpacity3
        {
            get { return pResistanceZOpacity3; }
            set { pResistanceZOpacity3 = value; }
        }	
		
		private int pSupportZOpacity1 = 30;
        [Range(0, 100)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Support Fresh Fill Opacity (%)", GroupName = "Zones Display", Order = 11)]
        public int SupportZOpacity1
        {
            get { return pSupportZOpacity1; }
            set { pSupportZOpacity1 = value; }
        }	
		
		private int pSupportZOpacity2 = 10;
        [Range(0, 100)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Support Fresh Tested Opacity (%)", GroupName = "Zones Display", Order = 13)]
        public int SupportZOpacity2
        {
            get { return pSupportZOpacity2; }
            set { pSupportZOpacity2 = value; }
        }	
		
		private int pSupportZOpacity3 = 10;
        [Range(0, 100)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Support Broken Fill Opacity (%)", GroupName = "Zones Display", Order = 15)]
        public int SupportZOpacity3
        {
            get { return pSupportZOpacity3; }
            set { pSupportZOpacity3 = value; }
        }	
		

		
		
	
		private bool pDepthEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Market Depth Display", Name = "Enabled", Order = 0)]
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        public bool DepthEnabled
        {
            get { return pDepthEnabled; }
            set { pDepthEnabled = value; }
        }	
		
		private int pMaxLevels = 10;
        [Range(1, 10000)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Market Depth Display", Name = "Maximum Levels", Order = 1)]
        public int MaxLevels
        {
            get { return pMaxLevels; }
            set { pMaxLevels = value; }
        }		
		
		private int pInvLength = 100;
        [Range(1, 10000)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Market Depth Display", Name = "Histogram Length (Pixels)", Order = 2)]
        public int InvLength
        {
            get { return pInvLength; }
            set { pInvLength = value; }
        }
		

		private bool pDisplayTotal = false;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Market Depth Display", Name = "Total Display", Order = 30)]
        public bool DisplayTotal
        {
            get { return pDisplayTotal; }
            set { pDisplayTotal = value; }
        }	
		
		private string pDepthTotalMode = "Both";
		[Description("")]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Market Depth Display", Name = "Total Mode", Description = "",  Order = 31)]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(TotalMode))]
		public string DepthTotalMode
		{
			get { return pDepthTotalMode; }
			set { pDepthTotalMode = value; }
		}


		
		private Brush pAskHistColor1 = Brushes.Red;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Market Depth Display", Name = "Ask Normal Color", Order = 11)]
		public Brush AskHistColor1
		{
			get { return pAskHistColor1; } set { pAskHistColor1 = value; }
		}
		[Browsable(false)]
		public string AskHistColor1S
		{
			get { return Serialize.BrushToString(pAskHistColor1); } set { pAskHistColor1 = Serialize.StringToBrush(value); }
		}
		
		private Brush pAskHistColor2 = Brushes.Red;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Market Depth Display", Name = "Ask Largest Color", Order = 13)]
		public Brush AskHistColor2
		{
			get { return pAskHistColor2; } set { pAskHistColor2 = value; }
		}
		[Browsable(false)]
		public string AskHistColor2S
		{
			get { return Serialize.BrushToString(pAskHistColor2); } set { pAskHistColor2 = Serialize.StringToBrush(value); }
		}
		
		private Brush pBidHistColor1 = Brushes.Green;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Market Depth Display", Name = "Bid Normal Color", Order = 21)]
		public Brush BidHistColor1
		{
			get { return pBidHistColor1; } set { pBidHistColor1 = value; }
		}
		[Browsable(false)]
		public string BidHistColor1S
		{
			get { return Serialize.BrushToString(pBidHistColor1); } set { pBidHistColor1 = Serialize.StringToBrush(value); }
		}
		
		//private Brush pBidHistColor2 = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0,60,0));
		private Brush pBidHistColor2 = Brushes.Green;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Market Depth Display", Name = "Bid Largest Color", Order = 23)]
		public Brush BidHistColor2
		{
			get { return pBidHistColor2; } set { pBidHistColor2 = value; }
		}
		[Browsable(false)]
		public string BidHistColor2S
		{
			get { return Serialize.BrushToString(pBidHistColor2); } set { pBidHistColor2 = Serialize.StringToBrush(value); }
		}
		
		
		private int pBidOpacity = 30;
        [Range(0, 100)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Market Depth Display", Name = "Bid Normal Opacity (%)", Order = 12)]
        public int BidOpacity
        {
            get { return pBidOpacity; }
            set { pBidOpacity = value; }
        }
		
		private int pAskOpacity = 30;
        [Range(0, 100)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Market Depth Display", Name = "Ask Normal Opacity (%)", Order = 22)]
        public int AskOpacity
        {
            get { return pAskOpacity; }
            set { pAskOpacity = value; }
        }
		
		private int pBidOpacity2 = 80;
        [Range(0, 100)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Market Depth Display", Name = "Bid Largest Opacity (%)", Order = 14)]
        public int BidOpacity2
        {
            get { return pBidOpacity2; }
            set { pBidOpacity2 = value; }
        }
		
		private int pAskOpacity2 = 80;
        [Range(0, 100)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Market Depth Display", Name = "Ask Largest Opacity (%)", Order = 24)]
        public int AskOpacity2
        {
            get { return pAskOpacity2; }
            set { pAskOpacity2 = value; }
        }
		
		
		private Brush pAskOutColor1 = Brushes.Black;
//		[XmlIgnore]
//		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Market Depth Display", Name = "Ask Outline Color", Order = 1)]
//		public Brush AskOutColor1
//		{
//			get { return pAskOutColor1; } set { pAskOutColor1 = value; }
//		}
//		[Browsable(false)]
//		public string AskOutColor1S
//		{
//			get { return Serialize.BrushToString(pAskOutColor1); } set { pAskOutColor1 = Serialize.StringToBrush(value); }
//		}
		

		private Brush pBidOutColor1 = Brushes.Black;
//		[XmlIgnore]
//		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Market Depth Display", Name = "Bid Outline Color", Order = 1)]
//		public Brush BidOutColor1
//		{
//			get { return pBidOutColor1; } set { pBidOutColor1 = value; }
//		}
//		[Browsable(false)]
//		public string BidOutColor1S
//		{
//			get { return Serialize.BrushToString(pBidOutColor1); } set { pBidOutColor1 = Serialize.StringToBrush(value); }
//		}
		

		
	



		
		private bool pZoomEnabled = false;
//        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Scale Display", Name = "Zoom Enabled", Order = 0)]
//		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
//        public bool ZoomEnabled
//        {
//            get { return pZoomEnabled; }
//            set { pZoomEnabled = value; }
//        }	
		
		private int pCurrentSetting = 80;
        [Range(-10000, 10000)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Scale Display", Name = "X Bar Size Zoom In (Pixels)", Order = 21)]
        public int CurrentSetting
        {
            get { return pCurrentSetting; }
            set { pCurrentSetting = value; }
        }		
		
		
		private int pSpaceBetweenBars = 20;
        [Range(-10000, 10000)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Scale Display", Name = "X Bar Space Zoom In (Pixels)", Order = 22)]
        public int SpaceBetweenBars
        {
            get { return pSpaceBetweenBars; }
            set { pSpaceBetweenBars = value; }
        }		
		
		private int pCurrentSetting2 = 20;
        [Range(-10000, 10000)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Scale Display", Name = "X Bar Size Zoom Out (Pixels)", Order = 23)]
        public int CurrentSetting2
        {
            get { return pCurrentSetting2; }
            set { pCurrentSetting2 = value; }
        }		
		
		
		private int pSpaceBetweenBars2 = 10;
        [Range(-10000, 10000)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Scale Display", Name = "X Bar Space Zoom Out (Pixels)", Order = 24)]
        public int SpaceBetweenBars2
        {
            get { return pSpaceBetweenBars2; }
            set { pSpaceBetweenBars2 = value; }
        }		
		
		private int pThisBarMarginRight = 80;
        [Range(0, 10000)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Scale Display", Name = "X Right Margin Minimum (Pixels)", Order = 25)]
        public int ThisBarMarginRight
        {
            get { return pThisBarMarginRight; }
            set { pThisBarMarginRight = value; }
        }	
		
		private bool pUseFixedVerticalScale = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Scale Display", Name = "Y Fixed Scaling Enabled", Order = 14)]
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        public bool UseFixedVerticalScale
        {
            get { return pUseFixedVerticalScale; }
            set { pUseFixedVerticalScale = value; }
        }	
		
		private bool pUseYScroll = false;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Scale Display", Name = "Y Scrolling Enabled", Order = 15)]
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        public bool UseYScroll
        {
            get { return pUseYScroll; }
            set { pUseYScroll = value; }
        }			
		
		
		
		
		private int pScaleTicks = 50;

		private string pScalePanMode = "Edge";
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Scale Display", Name = "Pan Mode", Order = 50)]
		[NinjaTrader.NinjaScript.NinjaScriptProperty]
        [TypeConverter(typeof(PanMode))]
        public string ScalePanMode
        {
            get { return pScalePanMode; }
            set { pScalePanMode = value; }
        }

		internal class PanMode : StringConverter
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
				return new StandardValuesCollection( new String[] {"Edge", "Center"} );
			}
		}

		private int pScaleEdgeMarginTicks = 0;
        [Range(0, 50)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Scale Display", Name = "Edge Margin (Ticks)", Order = 51)]
        public int ScaleEdgeMarginTicks
        {
            get { return pScaleEdgeMarginTicks; }
            set { pScaleEdgeMarginTicks = value; }
        }
        [Range(1, 100000)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Scale Display", Name = "Y Fixed Scaling (Ticks)", Order = 16)]
        public int ScaleTicks
        {
            get { return pScaleTicks; }
            set { pScaleTicks = value; }
        }			
		
		private int pScrollTicks = 4;
        [Range(1, 100000)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Scale Display", Name = "Y Scroll Increment (Ticks)", Order = 17)]
        public int ScrollTicks
        {
            get { return pScrollTicks; }
            set { pScrollTicks = value; }
        }	
		
		
		private int pDefaultTopBottomMargin = 5;
        [Range(0, 10000)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Scale Display", Name = "Y Default Top Bottom Margin (%)", Order = 18)]
        public int DefaultTopBottomMargin
        {
            get { return pDefaultTopBottomMargin; }
            set { pDefaultTopBottomMargin = value; }
        }	
		
		
	
		
		
		private bool pMAGEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Magnets", Name = "Enabled", Order = 0)]
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        public bool MAGEnabled
        {
            get { return pMAGEnabled; }
            set { pMAGEnabled = value; }
        }	


		private int pMAGSpreadMax = 1;
        [Range(0, 10000)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Parameters", Name = "Magnets - Max Spread Volume", Order = 0, Description = "the maximum difference between the bid and ask volume to qualify a magnet.")]
        public int MAGSpreadMax
        {
            get { return pMAGSpreadMax; }
            set { pMAGSpreadMax = value; }
        }			
		
		private int pMAGSizeMin = 10;
        [Range(0, 10000)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Parameters", Name = "Magnets - Minimum Volume", Order = 0, Description = "the minimum size of bid and ask volume to qualify a magnet.")]
        public int MAGSizeMin
        {
            get { return pMAGSizeMin; }
            set { pMAGSizeMin = value; }
        }	
		
		private bool pMAGEnabled3 = false;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Magnets", Name = "Fill Bar", Order = 10)]
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        public bool MAGEnabled3
        {
            get { return pMAGEnabled3; }
            set { pMAGEnabled3 = value; }
        }	
		
		private Brush pMagnetFillColor = Brushes.White;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Magnets", Name = "Fill Color", Order = 11)]
		public Brush MagnetFillColor
		{
			get { return pMagnetFillColor; } set { pMagnetFillColor = value; }
		}
		[Browsable(false)]
		public string MagnetFillColorS
		{
			get { return Serialize.BrushToString(pMagnetFillColor); } set { pMagnetFillColor = Serialize.StringToBrush(value); }
		}		
		
		
		private int pMagnetFillOpacity = 100;
        [Range(0, 100)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Magnets", Name = "Fill Opacity (%)", Order = 12)]
        public int MagnetFillOpacity
        {
            get { return pMagnetFillOpacity; }
            set { pMagnetFillOpacity = value; }
        }	
				
		
		private bool pMAGEnabled2 = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Magnets", Name = "Fresh Levels Enabled", Order = 13)]
        public bool MAGEnabled2
        {
            get { return pMAGEnabled2; }
            set { pMAGEnabled2 = value; }
        }			
		
        private Stroke pMagnetStroke = new Stroke(Brushes.White, DashStyleHelper.Solid, 2);
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Magnets", Name = "Fresh Levels Display", Order = 14)]
        public Stroke MagnetStroke
        {
            get { return pMagnetStroke; }
            set { pMagnetStroke = value; }
        }			

		private bool pMAGEnabled5 = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Magnets", Name = "Broken Levels Enabled", Order = 15)]
        public bool MAGEnabled5
        {
            get { return pMAGEnabled5; }
            set { pMAGEnabled5 = value; }
        }	
		
        private Stroke pMagnetStroke2 = new Stroke(Brushes.White, DashStyleHelper.Solid, 2);
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Magnets", Name = "Broken Levels Display", Order = 16)]
        public Stroke MagnetStroke2
        {
            get { return pMagnetStroke2; }
            set { pMagnetStroke2 = value; }
        }					
		
		private bool pUFAEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Unfinished Auctions", Name = "Enabled", Order = 0)]
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        public bool UFAEnabled
        {
            get { return pUFAEnabled; }
            set { pUFAEnabled = value; }
        }	
		
		private bool pUFAEnabled3 = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Unfinished Auctions", Name = "Fill Bar", Order = 10)]
        public bool UFAEnabled3
        {
            get { return pUFAEnabled3; }
            set { pUFAEnabled3 = value; }
        }	
		
		private Brush pUFAFillColor = Brushes.Goldenrod;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Unfinished Auctions", Name = "Fill Color", Order = 11)]
		public Brush UFAFillColor
		{
			get { return pUFAFillColor; } set { pUFAFillColor = value; }
		}
		[Browsable(false)]
		public string UFAFillColorS
		{
			get { return Serialize.BrushToString(pUFAFillColor); } set { pUFAFillColor = Serialize.StringToBrush(value); }
		}				
		
		private int pUFAFillOpacity = 100;
        [Range(0, 100)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Unfinished Auctions", Name = "Fill Opacity (%)", Order = 12)]
        public int UFAFillOpacity
        {
            get { return pUFAFillOpacity; }
            set { pUFAFillOpacity = value; }
        }	
				
		
		private bool pUFAEnabled2 = false;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Unfinished Auctions", Name = "Fresh Levels Enabled", Order = 13)]
        public bool UFAEnabled2
        {
            get { return pUFAEnabled2; }
            set { pUFAEnabled2 = value; }
        }	
		

        private Stroke pUFAStroke = new Stroke(Brushes.Goldenrod, DashStyleHelper.Solid, 2);
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Unfinished Auctions", Name = "Fresh Levels Display", Order = 14)]
        public Stroke UFAStroke
        {
            get { return pUFAStroke; }
            set { pUFAStroke = value; }
        }			
		
		private bool pUFAEnabled5 = false;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Unfinished Auctions", Name = "Broken Levels Enabled", Order = 15)]
        public bool UFAEnabled5
        {
            get { return pUFAEnabled5; }
            set { pUFAEnabled5 = value; }
        }	
		

        private Stroke pUFAStroke2 = new Stroke(Brushes.Goldenrod, DashStyleHelper.Solid, 2);
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Unfinished Auctions", Name = "Broken Levels Display", Order = 16)]
        public Stroke UFAStroke2
        {
            get { return pUFAStroke2; }
            set { pUFAStroke2 = value; }
        }	
	
		
		
		
		
		
		
		
		
		
		
		
		// ARROW INPUTS

        private bool pArrowsEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Washout Signals", Name = "Enabled", Description = "", Order = 0)]
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        public bool ArrowsEnabled
        {
            get { return pArrowsEnabled; }
            set { pArrowsEnabled = value; }
        }
		
		private int pMinimumDecliningLevels = 3;
        [Range(2, 10000)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Parameters", Name = "Washout Signals - Declining Levels (Min)", Order = 80, Description = "the minimum number of declining levels on the price scale to qualify a signal.")]
        public int MinimumDecliningLevels
        {
            get { return pMinimumDecliningLevels; }
            set { pMinimumDecliningLevels = value; }
        }	
	        
		private bool pHighLowRangeFilter2 = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Parameters", Name = "Washout Signals - Bar Direction Filter", Order = 81, Description = "require an up bar for long and down bar for short")]
        public bool HighLowRangeFilter2
        {
            get { return pHighLowRangeFilter2; }
            set { pHighLowRangeFilter2 = value; }
        }
			
        private bool pHighLowRangeFilter = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Parameters", Name = "Washout Signals - High Low Range Filter", Order = 82, Description = "require a higher high for a short and lower low for a long")]
        public bool HighLowRangeFilter
        {
            get { return pHighLowRangeFilter; }
            set { pHighLowRangeFilter = value; }
        }
		

		

				
		

	
        private float pArrowSize = 9;
        [Range(0, 1000)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Arrow Size", Description = "", GroupName = "Washout Signals", Order = 21)]
        public float ArrowSize
        {
            get { return pArrowSize; }
            set { pArrowSize = value; }
        }		
		
		
        private float pArrowOffset = 15;
        [Range(0, 1000)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Arrow Offset (Pixels)", Description = "", GroupName = "Washout Signals", Order = 22)]
        public float ArrowOffset
        {
            get { return pArrowOffset; }
            set { pArrowOffset = value; }
        }
		
        private float pArrowBarWidth = 3;
//        [Range(0, 1000)]
//        [Display(ResourceType = typeof(Custom.Resource), Name = "Structure - Bar Width", Description = "", GroupName = "Washout Signals", Order = 3)]
//        public float ArrowBarWidth
//        {
//            get { return pArrowBarWidth; }
//            set { pArrowBarWidth = value; }
//        }
		
        private float pArrowBarHeight = 0;
//        [Range(0, 1000)]
//        [Display(ResourceType = typeof(Custom.Resource), Name = "Structure - Bar Height", Description = "", GroupName = "Washout Signals", Order = 2)]
//        public float ArrowBarHeight
//        {
//            get { return pArrowBarHeight; }
//            set { pArrowBarHeight = value; }
//        }
		


		private Brush pArrowUpFBrush	= Brushes.Green;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Buy Fill", Description = "", GroupName = "Washout Signals", Order = 23)]
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
		[Display(ResourceType = typeof(Custom.Resource), Name = "Sell Fill", Description = "", GroupName = "Washout Signals", Order = 25)]
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
        [Display(ResourceType = typeof(Custom.Resource), Name = "Buy Outline", Description = "", GroupName = "Washout Signals", Order = 24)]
        public Stroke ArrowUpStroke
        {
            get { return pArrowUpStroke; }
            set { pArrowUpStroke = value; }
        }

        private Stroke pArrowDownStroke = new Stroke(Brushes.DarkRed, DashStyleHelper.Solid, 1);
        [Display(ResourceType = typeof(Custom.Resource), Name = "Sell Outline", Description = "", GroupName = "Washout Signals", Order = 26)]
        public Stroke ArrowDownStroke
        {
            get { return pArrowDownStroke; }
            set { pArrowDownStroke = value; }
        }

		
		// LABELS		
		
        private bool pLabelsEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Labels Enabled", Description = "", GroupName = "Washout Signals", Order = 31)]
        public bool LabelsEnabled
        {
            get { return pLabelsEnabled; }
            set { pLabelsEnabled = value; }
        }			
		
		private SimpleFont pTextFont = new SimpleFont("Consolas", 11);
		[Display(ResourceType = typeof(Custom.Resource), Name = "Labels Font", Description = "", GroupName = "Washout Signals", Order = 32)]
		public SimpleFont TextFont
        {
            get { return pTextFont; }
            set { pTextFont = value; }
        }	
		
		
		private string pLabelBuy = "";	
		[Display(ResourceType = typeof(Custom.Resource), Name = "Labels Buy", Description = "", GroupName = "Washout Signals", Order = 33)]
        public string LabelBuy
        {
            get { return pLabelBuy; }
            set { pLabelBuy = value; }
        }		
		
		private string pLabelSell = "";
		[Display(ResourceType = typeof(Custom.Resource), Name = "Labels Sell", Description = "", GroupName = "Washout Signals", Order = 34)]
        public string LabelSell
        {
            get { return pLabelSell; }
            set { pLabelSell = value; }
        }	
		
		
		
		
	    private bool pBarCompositeEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Volume Profile (Bar)", Name = "Enabled", Description = "", Order = 0)]
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        public bool BarCompositeEnabled
        {
            get { return pBarCompositeEnabled; }
            set { pBarCompositeEnabled = value; }
        }		
	
		 
		private bool pDisplayPOC = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Volume Profile (Bar)", Name = "POC Enabled", Description = "", Order = 10)]
        public bool DisplayPOC
        {
            get { return pDisplayPOC; }
            set { pDisplayPOC = value; }
        }
		
	
	    private bool pDisplayVWAP = false;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Volume Profile (Bar)", Name = "VWAP Enabled", Description = "", Order = 20)]
        public bool DisplayVWAP
        {
            get { return pDisplayVWAP; }
            set { pDisplayVWAP = value; }
        }
		

			   
		
	    private bool pDisplayCL = false;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Volume Profile (Bar)", Name = "Cluster Enabled ", Description = "", Order = 30)]
        public bool DisplayCL
        {
            get { return pDisplayCL; }
            set { pDisplayCL = value; }
        }		
		

		
		
		private Brush pBarPOCFillColor = Brushes.WhiteSmoke;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Volume Profile (Bar)", Name = "POC Fill Color", Order = 11)]
		public Brush BarPOCFillColor
		{
			get { return pBarPOCFillColor; } set { pBarPOCFillColor = value; }
		}
		[Browsable(false)]
		public string BarPOCFillColorS
		{
			get { return Serialize.BrushToString(pBarPOCFillColor); } set { pBarPOCFillColor = Serialize.StringToBrush(value); }
		}	
		
		private Brush pBarVWAPFillColor = Brushes.DarkCyan;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Volume Profile (Bar)", Name = "VWAP Fill Color", Order = 21)]
		public Brush BarVWAPFillColor
		{
			get { return pBarVWAPFillColor; } set { pBarVWAPFillColor = value; }
		}
		[Browsable(false)]
		public string BarVWAPFillColorS
		{
			get { return Serialize.BrushToString(pBarVWAPFillColor); } set { pBarVWAPFillColor = Serialize.StringToBrush(value); }
		}	
		
		private int pProfileBarOpacity = 50;
        [Range(0, 100)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Volume Profile (Bar)", Name = "Fill Opacity (%)", Order = 25)]
        public int ProfileBarOpacity
        {
            get { return pProfileBarOpacity; }
            set { pProfileBarOpacity = value; }
        }	
		
		private Brush pBarClusterFillColor = Brushes.Transparent;
//		[XmlIgnore]
//		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Volume Profile (Bar)", Name = "Cluster Fill Color", Order = 31)]
//		public Brush BarClusterFillColor
//		{
//			get { return pBarClusterFillColor; } set { pBarClusterFillColor = value; }
//		}
//		[Browsable(false)]
//		public string BarClusterFillColorS
//		{
//			get { return Serialize.BrushToString(pBarClusterFillColor); } set { pBarClusterFillColor = Serialize.StringToBrush(value); }
//		}	
		
        private Stroke pClusterStroke = new Stroke(Brushes.WhiteSmoke, DashStyleHelper.Solid, 2);
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Volume Profile (Bar)", Name = "Cluster Outline", Order = 32)]
        public Stroke ClusterStroke
        {
            get { return pClusterStroke; }
            set { pClusterStroke = value; }
        }			
	

	    private bool pDisplayVA = true;
//        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Volume Profile (Bar)", Name = "Display VAH / VAL", Description = "", Order = 4)]
//        public bool DisplayVA
//        {
//            get { return pDisplayVA; }
//            set { pDisplayVA = value; }
//        }				
		

		
		
		

		private string pVA1Basis = "VWAP";
//		[Description("")]
//		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Parameters", Name = "Value Area 1 Basis",  Order = 50, Description = "")]
//		[RefreshProperties(RefreshProperties.All)]
//		[TypeConverter(typeof(ValueAreaBasis))]
//		public string VA1Basis
//		{
//			get { return pVA1Basis; }
//			set { pVA1Basis = value; }
//		}
		
		private int pVA1Percent = 68;
        [Range(0, 100)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Parameters", Name = "Value Area 1 (%)", Order = 51, Description = "")]
        public int VA1Percent
        {
            get { return pVA1Percent; }
            set { pVA1Percent = value; }
        }	
		
		private string pVA2Basis = "VWAP";
//		[Description("")]
//		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Parameters", Name = "Value Area 2 Basis",  Order = 52, Description = "")]
//		[RefreshProperties(RefreshProperties.All)]
//		[TypeConverter(typeof(ValueAreaBasis))]
//		public string VA2Basis
//		{
//			get { return pVA2Basis; }
//			set { pVA2Basis = value; }
//		}
		
		private int pVA2Percent = 95;
        [Range(0, 100)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Parameters", Name = "Value Area 2 (%)", Order = 53, Description = "")]
        public int VA2Percent
        {
            get { return pVA2Percent; }
            set { pVA2Percent = value; }
        }	
				
		internal class ValueAreaBasis : StringConverter
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
				return new StandardValuesCollection( new String[] {"POC", "VWAP"} );
			}
		}	
		
		
		
		
	
		
		private bool pCompositeEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Volume Profile (Composite)", Name = "Enabled", Order = -10)]
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        public bool CompositeEnabled
        {
            get { return pCompositeEnabled; }
            set { pCompositeEnabled = value; }
        }		
	
		private string pCompositeResetMode = "Session Break";
		[Description("")]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Volume Profile (Composite)", Name = "Reset Mode", Description = "",  Order = -8)]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(ThisCompositeResetMode))]
		public string CompositeResetMode
		{
			get { return pCompositeResetMode; }
			set { pCompositeResetMode = value; }
		}

		internal class ThisCompositeResetMode : StringConverter
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
				return new StandardValuesCollection( new String[] {"Session Break", "Time Of Day"} );
			}
		}	
		
		private TimeSpan pResetTime1 = new TimeSpan(6,00,0);
		[Display(ResourceType = typeof(Custom.Resource), Name = "Reset Time", Description = "", GroupName = "Volume Profile (Composite)", Order = -7)]
		public string MarketOpen
		{
			get { return pResetTime1.Hours.ToString("0")+":"+pResetTime1.Minutes.ToString("00"); }
			set { if(!TimeSpan.TryParse(value, out pResetTime1)) pResetTime1=new TimeSpan(0,0,0); }
		}			
		
		private string pCompositeLocation = "Left";
		[Description("")]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Volume Profile (Composite)", Name = "Location", Description = "",  Order = 0)]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(CompositeLocationMode))]
		public string CompositeLocation
		{
			get { return pCompositeLocation; }
			set { pCompositeLocation = value; }
		}

		internal class CompositeLocationMode : StringConverter
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
				return new StandardValuesCollection( new String[] {"Left", "Right"} );
			}
		}	
		
		
		private string pCompNumberDisplayMode = "Volume";
		[Description("")]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Volume Profile (Composite)", Name = "Number Display Mode", Description = "",  Order = 9)]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(NumberDisplayMode))]
		public string CompNumberDisplayMode
		{
			get { return pCompNumberDisplayMode; }
			set { pCompNumberDisplayMode = value; }
		}
		
		private string pCompHistogramDisplayMode = "Volume & Delta";
		[Description("")]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Volume Profile (Composite)", Name = "Histogram Display Mode", Description = "",  Order = 10)]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(HistogramDisplayMode))]
		public string CompHistogramDisplayMode
		{
			get { return pCompHistogramDisplayMode; }
			set { pCompHistogramDisplayMode = value; }
		}		
		
	
		
		
		internal class NumberDisplayMode : StringConverter
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
				return new StandardValuesCollection( new String[] {"Bid / Ask", "Volume", "Delta", "None"} );
			}
		}
		
		internal class HistogramDisplayMode : StringConverter
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
				return new StandardValuesCollection( new String[] {"Volume & Delta", "Volume", "None"} );
			}
		}
		
		
		
		private int pCompLength = 150;
        [Range(1, 10000)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Volume Profile (Composite)", Name = "Histogram Length (Pixels)",  Order = 15, Description = "the maximum length of the histogram display, in pixels.")]
        public int CompLength
        {
            get { return pCompLength; }
            set { pCompLength = value; }
        }	
		
		
		private bool pCompAllLevelsEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Volume Profile (Composite) Levels", Name = "Enabled", Description = "", Order = 50)]
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        public bool CompAllLevelsEnabled
        {
            get { return pCompAllLevelsEnabled; }
            set { pCompAllLevelsEnabled = value; }
        }
		
		private bool pCompLevelsPriceOn = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Volume Profile (Composite) Levels", Name = "Price Markers", Description = "", Order = 200)]

        public bool CompLevelsPriceOn
        {
            get { return pCompLevelsPriceOn; }
            set { pCompLevelsPriceOn = value; }
        }
				
	
		
			
		private bool pCompLabelsEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Volume Profile (Composite) Levels", Name = "Labels Enabled", Description = "", Order = 70)]
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        public bool CompLabelsEnabled
        {
            get { return pCompLabelsEnabled; }
            set { pCompLabelsEnabled = value; }
        }		
			
		private bool pExtendCompRight = true;
//        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Volume Profile (Composite) Levels", Name = "Extend Right", Order = 71)]
//        public bool ExtendCompRight
//        {
//            get { return pExtendCompRight; }
//            set { pExtendCompRight = value; }
//        }	
		
		private bool pCompPOCEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Volume Profile (Composite) Levels", Name = "POC Enabled", Description = "", Order = 51)]
        public bool CompPOCEnabled
        {
            get { return pCompPOCEnabled; }
            set { pCompPOCEnabled = value; }
        }
		
		private bool pCompVWAPEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Volume Profile (Composite) Levels", Name = "VWAP Enabled", Description = "", Order = 53)]
        public bool CompVWAPEnabled
        {
            get { return pCompVWAPEnabled; }
            set { pCompVWAPEnabled = value; }
        }		
		
		private bool pCompVA1Enabled = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Volume Profile (Composite) Levels", Name = "Value Area 1 Enabled", Description = "", Order = 55)]
        public bool CompVA1Enabled
        {
            get { return pCompVA1Enabled; }
            set { pCompVA1Enabled = value; }
        }
		
		private bool pCompVA2Enabled = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Volume Profile (Composite) Levels", Name = "Value Area 2 Enabled", Description = "", Order = 58)]
        public bool CompVA2Enabled
        {
            get { return pCompVA2Enabled; }
            set { pCompVA2Enabled = value; }
        }
		
		private bool pCompDHLEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Volume Profile (Composite) Levels", Name = "Session High / Low Enabled", Description = "", Order = 62)]
        public bool CompDHLEnabled
        {
            get { return pCompDHLEnabled; }
            set { pCompDHLEnabled = value; }
        }		
			
		
		
		
		private int pCompPlotsOpacity = 80;
        [Range(0, 100)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Opacity (%)", GroupName = "Volume Profile (Composite) Levels", Order = 66)]
        public int CompPlotsOpacity
        {
            get { return pCompPlotsOpacity; }
            set { pCompPlotsOpacity = value; }
        }	
		
		
		private Brush pPlot1Brush = Brushes.DimGray;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "POC Color", GroupName = "Volume Profile (Composite) Levels", Order = 52)]
		public Brush Plot1Brush
		{
			get { return pPlot1Brush; } set { pPlot1Brush = value; }
		}
		[Browsable(false)]
		public string Plot1BrushS
		{
			get { return Serialize.BrushToString(pPlot1Brush); } set { pPlot1Brush = Serialize.StringToBrush(value); }
		}		
		
		private Brush pPlot2Brush = Brushes.DarkCyan;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "VWAP Color", GroupName = "Volume Profile (Composite) Levels", Order = 54)]
		public Brush Plot2Brush
		{
			get { return pPlot2Brush; } set { pPlot2Brush = value; }
		}
		[Browsable(false)]
		public string Plot2BrushS
		{
			get { return Serialize.BrushToString(pPlot2Brush); } set { pPlot2Brush = Serialize.StringToBrush(value); }
		}		
		
		private Brush pPlot3Brush = Brushes.Red;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "VAH 1 Color", GroupName = "Volume Profile (Composite) Levels", Order = 56)]
		public Brush Plot3Brush
		{
			get { return pPlot3Brush; } set { pPlot3Brush = value; }
		}
		[Browsable(false)]
		public string Plot3BrushS
		{
			get { return Serialize.BrushToString(pPlot3Brush); } set { pPlot3Brush = Serialize.StringToBrush(value); }
		}			
		
		private Brush pPlot4Brush = Brushes.Red;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "VAL 1 Color", GroupName = "Volume Profile (Composite) Levels", Order = 57)]
		public Brush Plot4Brush
		{
			get { return pPlot4Brush; } set { pPlot4Brush = value; }
		}
		[Browsable(false)]
		public string Plot4BrushS
		{
			get { return Serialize.BrushToString(pPlot4Brush); } set { pPlot4Brush = Serialize.StringToBrush(value); }
		}		
		
		private Brush pPlot5Brush = Brushes.Green;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "VAH 2 Color", GroupName = "Volume Profile (Composite) Levels", Order = 59)]
		public Brush Plot5Brush
		{
			get { return pPlot5Brush; } set { pPlot5Brush = value; }
		}
		[Browsable(false)]
		public string Plot5BrushS
		{
			get { return Serialize.BrushToString(pPlot5Brush); } set { pPlot5Brush = Serialize.StringToBrush(value); }
		}	
		
		private Brush pPlot6Brush = Brushes.Green;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "VAL 2 Color", GroupName = "Volume Profile (Composite) Levels", Order = 60)]
		public Brush Plot6Brush
		{
			get { return pPlot6Brush; } set { pPlot6Brush = value; }
		}
		[Browsable(false)]
		public string Plot6BrushS
		{
			get { return Serialize.BrushToString(pPlot6Brush); } set { pPlot6Brush = Serialize.StringToBrush(value); }
		}	
		
		private Brush pPlot8Brush = Brushes.SteelBlue;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Session High Color", GroupName = "Volume Profile (Composite) Levels", Order = 63)]
		public Brush Plot8Brush
		{
			get { return pPlot8Brush; } set { pPlot8Brush = value; }
		}
		[Browsable(false)]
		public string Plot8BrushS
		{
			get { return Serialize.BrushToString(pPlot8Brush); } set { pPlot8Brush = Serialize.StringToBrush(value); }
		}	
		
		private Brush pPlot7Brush = Brushes.SteelBlue;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Session Low Color", GroupName = "Volume Profile (Composite) Levels", Order = 64)]
		public Brush Plot7Brush
		{
			get { return pPlot7Brush; } set { pPlot7Brush = value; }
		}
		[Browsable(false)]
		public string Plot7BrushS
		{
			get { return Serialize.BrushToString(pPlot7Brush); } set { pPlot7Brush = Serialize.StringToBrush(value); }
		}	
		

		
		
			
		private Brush pCompVOLColor = Brushes.SteelBlue;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Volume Profile (Composite)",  Name = "Color Volume", Order = 20)]
		public Brush CompVOLColor
		{
			get { return pCompVOLColor; } set { pCompVOLColor = value; }
		}
		[Browsable(false)]
		public string CompVOLColorS
		{
			get { return Serialize.BrushToString(pCompVOLColor); } set { pCompVOLColor = Serialize.StringToBrush(value); }
		}
		
		private Brush pCompNEColor = Brushes.Gray;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Volume Profile (Composite)",  Name = "Color Neutral", Order = 20)]
		public Brush CompNEColor
		{
			get { return pCompNEColor; } set { pCompNEColor = value; }
		}
		[Browsable(false)]
		public string CompNEColorS
		{
			get { return Serialize.BrushToString(pCompNEColor); } set { pCompNEColor = Serialize.StringToBrush(value); }
		}
		
		private Brush pCompUPColor = Brushes.Green;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Volume Profile (Composite)",  Name = "Color Buy", Order = 20)]
		public Brush CompUPColor
		{
			get { return pCompUPColor; } set { pCompUPColor = value; }
		}
		[Browsable(false)]
		public string CompUPColorS
		{
			get { return Serialize.BrushToString(pCompUPColor); } set { pCompUPColor = Serialize.StringToBrush(value); }
		}
		
		
		private Brush pCompDNColor = Brushes.Red;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Volume Profile (Composite)", Name = "Color Sell", Order = 20)]
		public Brush CompDNColor
		{
			get { return pCompDNColor; } set { pCompDNColor = value; }
		}
		[Browsable(false)]
		public string CompDNColorS
		{
			get { return Serialize.BrushToString(pCompDNColor); } set { pCompDNColor = Serialize.StringToBrush(value); }
		}
		
		

		private int pCompMinOpacity = 10;
        [Range(0, 100)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Volume Profile (Composite)", Name = "Minimum Opacity (%)", Order = 21)]
        public int CompMinOpacity
        {
            get { return pCompMinOpacity; }
            set { pCompMinOpacity = value; }
        }		
		
		private int pCompDefaultOpacity = 30;
//        [Range(0, 100)]
//        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Volume Profile (Composite)", Name = "Default Opacity (%)", Order = 22)]
//        public int CompDefaultOpacity
//        {
//            get { return pCompDefaultOpacity; }
//            set { pCompDefaultOpacity = value; }
//        }	
		
		private int pCompMaxOpacity = 90;
        [Range(0, 100)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Volume Profile (Composite)", Name = "Maximum Opacity (%)", Order = 23)]
        public int CompMaxOpacity
        {
            get { return pCompMaxOpacity; }
            set { pCompMaxOpacity = value; }
        }			

		private bool pCompositeBackEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Volume Profile (Composite)", Name = "Background Shading Enabled", Order = 25)]
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        public bool CompositeBackEnabled
        {
            get { return pCompositeBackEnabled; }
            set { pCompositeBackEnabled = value; }
        }		
	
		private int pCompBackOpacity = 80;
        [Range(0, 100)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Volume Profile (Composite)", Name = "Background Shading Opacity (%)", Order = 26)]
        public int CompBackOpacity
        {
            get { return pCompBackOpacity; }
            set { pCompBackOpacity = value; }
        }			
		// HUD
		
		
		private bool pHUDEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Heads Up Display", Name = "Enabled", Description = "", Order = 0)]
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        public bool HUDEnabled
        {
            get { return pHUDEnabled; }
            set { pHUDEnabled = value; }
        }
		
		private bool pHUD1 = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Heads Up Display", Name = "Total Volume Enabled", Description = "", Order = 1)]
        public bool HUD1
        {
            get { return pHUD1; }
            set { pHUD1 = value; }
        }		
		
		private bool pHUD2 = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Heads Up Display", Name = "Bar Volume Enabled", Description = "", Order = 2)]
        public bool HUD2
        {
            get { return pHUD2; }
            set { pHUD2 = value; }
        }	
		
		private bool pHUD3 = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Heads Up Display", Name = "Total Delta Enabled", Description = "", Order = 3)]
        public bool HUD3
        {
            get { return pHUD3; }
            set { pHUD3 = value; }
        }	
		
		private bool pHUD4 = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Heads Up Display", Name = "Bar Delta Enabled", Description = "", Order = 4)]
        public bool HUD4
        {
            get { return pHUD4; }
            set { pHUD4 = value; }
        }		
		
		private bool pHUD5 = false;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Heads Up Display", Name = "Bar Bid / Ask Enabled", Description = "", Order = 5)]
        public bool HUD5
        {
            get { return pHUD5; }
            set { pHUD5 = value; }
        }	
		
		private string pNetDMode2 = "Total";
		[Description("")]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Heads Up Display", Name = "Bar Delta Mode", Description = "",  Order = 10)]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(ChooseNDMode))]
		public string NetDMode2
		{
			get { return pNetDMode2; }
			set { pNetDMode2 = value; }
		}
		
		
		
		internal class ChooseNDMode : StringConverter
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
				return new StandardValuesCollection( new String[] {"Total", "Percent"} );
			}
		}
		

		private Brush pHUDVOLColor = Brushes.SteelBlue;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Heads Up Display",  Name = "Color Volume", Order = 20)]
		public Brush HUDVOLColor
		{
			get { return pHUDVOLColor; } set { pHUDVOLColor = value; }
		}
		[Browsable(false)]
		public string HUDVOLColorS
		{
			get { return Serialize.BrushToString(pHUDVOLColor); } set { pHUDVOLColor = Serialize.StringToBrush(value); }
		}
		
		private Brush pHUDNEColor = Brushes.Gray;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Heads Up Display",  Name = "Color Neutral", Order = 20)]
		public Brush HUDNEColor
		{
			get { return pHUDNEColor; } set { pHUDNEColor = value; }
		}
		[Browsable(false)]
		public string HUDNEColorS
		{
			get { return Serialize.BrushToString(pHUDNEColor); } set { pHUDNEColor = Serialize.StringToBrush(value); }
		}
		
		private Brush pHUDUPColor = Brushes.Green;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Heads Up Display",  Name = "Color Buy", Order = 20)]
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
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Heads Up Display", Name = "Color Sell", Order = 20)]
		public Brush HUDDNColor
		{
			get { return pHUDDNColor; } set { pHUDDNColor = value; }
		}
		[Browsable(false)]
		public string HUDDNColorS
		{
			get { return Serialize.BrushToString(pHUDDNColor); } set { pHUDDNColor = Serialize.StringToBrush(value); }
		}
		
		

		private int pHUDMinOpacity = 30;
        [Range(0, 100)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Heads Up Display", Name = "Minimum Opacity (%)", Order = 21)]
        public int HUDMinOpacity
        {
            get { return pHUDMinOpacity; }
            set { pHUDMinOpacity = value; }
        }					
		private int pHUDDefaultOpacity = 30;
        [Range(0, 100)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Heads Up Display", Name = "Default Opacity (%)", Order = 22)]
        public int HUDDefaultOpacity
        {
            get { return pHUDDefaultOpacity; }
            set { pHUDDefaultOpacity = value; }
        }	
		
		private int pHUDMaxOpacity = 80;
        [Range(0, 100)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Heads Up Display", Name = "Maximum Opacity (%)", Order = 23)]
        public int HUDMaxOpacity
        {
            get { return pHUDMaxOpacity; }
            set { pHUDMaxOpacity = value; }
        }			
	

		
		private Brush pChartBackgroundBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(10,10,10));
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Chart Display", Name = "Background Color", Order = 20)]
		public Brush ChartBackgroundBrush
		{
			get { return pChartBackgroundBrush; } set { pChartBackgroundBrush = value; }
		}
		[Browsable(false)]
		public string ChartBackgroundBrushS
		{
			get { return Serialize.BrushToString(pChartBackgroundBrush); } set { pChartBackgroundBrush = Serialize.StringToBrush(value); }
		}		
		
		private Brush pChartAxisBrush = Brushes.WhiteSmoke;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Chart Display", Name = "Axis Color", Order = 21)]
		public Brush ChartAxisBrush
		{
			get { return pChartAxisBrush; } set { pChartAxisBrush = value; }
		}
		[Browsable(false)]
		public string ChartAxisBrushS
		{
			get { return Serialize.BrushToString(pChartAxisBrush); } set { pChartAxisBrush = Serialize.StringToBrush(value); }
		}		
		
		
        private Stroke pGridLineHStroke = new Stroke(new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(35,35,38)), DashStyleHelper.Dot, 1);
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Chart Display", Name = "Grid Line Horizontal Display", Order = 39)]
        public Stroke GridLineHStroke
        {
            get { return pGridLineHStroke; }
            set { pGridLineHStroke = value; }
        }		
		
        private Stroke pGridLineVStroke = new Stroke(new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(35,35,38)), DashStyleHelper.Dot, 1);
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Chart Display", Name = "Grid Line Vertical Display", Order = 40)]
        public Stroke GridLineVStroke
        {
            get { return pGridLineVStroke; }
            set { pGridLineVStroke = value; }
        }		
				
		
		private int pTicksMove = 12;
        [Range(1, 100000)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Parameters", Name = "Swing Levels - Minimum Move (Ticks)", Order = 0)]
        public int TicksMove
        {
            get { return pTicksMove; }
            set { pTicksMove = value; }
        }
		

		private bool pPivotLinesEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Swing Levels", Name = "Enabled", Description = "", Order = 0)]
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        public bool PivotLinesEnabled
        {
            get { return pPivotLinesEnabled; }
            set { pPivotLinesEnabled = value; }
        }
		
		private bool pLevelsEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Swing Levels", Name = "Fresh Levels Enabled", Description = "", Order = 10)]
        public bool LevelsEnabled
        {
            get { return pLevelsEnabled; }
            set { pLevelsEnabled = value; }
        }

		private bool pHistoricalLevelsEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Swing Levels", Name = "Broken Levels Enabled", Description = "", Order = 20)]
        public bool HistoricalLevelsEnabled
        {
            get { return pHistoricalLevelsEnabled; }
            set { pHistoricalLevelsEnabled = value; }
        }
		
        private Stroke pColorSwingLow = new Stroke(Brushes.Red, DashStyleHelper.Solid, 2);
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Swing Levels", Name = "Fresh Levels Low Display", Order = 11)]
        public Stroke ColorSwingLow
        {
            get { return pColorSwingLow; }
            set { pColorSwingLow = value; }
        }			
		
        private Stroke pColorSwingHigh = new Stroke(Brushes.Blue, DashStyleHelper.Solid, 2);
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Swing Levels", Name = "Fresh Levels High Display", Order = 12)]
        public Stroke ColorSwingHigh
        {
            get { return pColorSwingHigh; }
            set { pColorSwingHigh = value; }
        }			
		
        private Stroke pColorSwingLow2 = new Stroke(Brushes.DarkRed, DashStyleHelper.Solid, 2);
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Swing Levels", Name = "Broken Levels Low Display", Order = 21)]
        public Stroke ColorSwingLow2
        {
            get { return pColorSwingLow2; }
            set { pColorSwingLow2 = value; }
        }			
		
        private Stroke pColorSwingHigh2 = new Stroke(Brushes.DarkBlue, DashStyleHelper.Solid, 2);
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Swing Levels", Name = "Broken Levels High Display", Order = 22)]
        public Stroke ColorSwingHigh2
        {
            get { return pColorSwingHigh2; }
            set { pColorSwingHigh2 = value; }
        }	
		
		
		
		// Bar Counter
		
		
		private bool pTimerEnabled = true;
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

		private Brush pTimerMainColor = Brushes.Silver;
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
		
		
		
		
		private int pTimerOffset = 5;
        [Range(-1000, 1000)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Bar Counter", Name = "Offset (Pixels)", Order = 20)]
        public int TimerOffset
        {
            get { return pTimerOffset; }
            set { pTimerOffset = value; }
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
	
		
		
		
//		private bool pAudioEnabledMain = true;
//		[NinjaScriptProperty]
//		[ReadOnly(true)]
//        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Audio Alerts", Name = "Enabled", Description = "", Order = 4)]
//        public bool AudioEnabledMain
//        {
//            get { return pAudioEnabledMain; }
//            set { pAudioEnabledMain = value; }
//        }	
		
//		private string pWAVFile = "Coming Soon";
//		[ReadOnly(true)]
//		[Description("")]
//		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Audio Alerts", Name = "Details", Description = "",  Order = 10)]
//		[RefreshProperties(RefreshProperties.All)]
//		//[TypeConverter(typeof(ChooseNDMode))]
//		public string WAVFile
//		{
//			get { return pWAVFile; }
//			set { pWAVFile = value; }
//		}	
		
		
		
		private bool pAudioEnabledMain = false;
		[Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", Description = "", GroupName = "Audio Alerts", Order = -10)]
		[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        public bool AudioEnabledMain
        {
            get { return pAudioEnabledMain; }
            set { pAudioEnabledMain = value; }
        }	
		
		private bool pQuickAudio = true;
		[Display(ResourceType = typeof(Custom.Resource), Name = "Intra Bar", GroupName = "Audio Alerts", Order = -9)]
        public bool QuickAudio
        {
            get { return pQuickAudio; }
            set { pQuickAudio = value; }
        }
		
		
		private bool pAudioEnabled1 = false;
		[Display(ResourceType = typeof(Custom.Resource), Name = "Washout Enabled", Description = "", GroupName = "Audio Alerts", Order = 0)]
		//[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        public bool AudioEnabled1
        {
            get { return pAudioEnabled1; }
            set { pAudioEnabled1 = value; }
        }	
		
		private bool pAudioEnabled2 = true;
		[Display(ResourceType = typeof(Custom.Resource), Name = "New Imbalance Zone Enabled", Description = "", GroupName = "Audio Alerts", Order = 10)]
		//[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        public bool AudioEnabled2
        {
            get { return pAudioEnabled2; }
            set { pAudioEnabled2 = value; }
        }	
		
		
		
		private string pWAVFileName = "Alert2.wav";
		[Display(ResourceType = typeof(Custom.Resource), Name = "Washout Buy WAV File", Description = "Sound file to play when a buy signal occurs.", GroupName = "Audio Alerts", Order = 1)]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(LoadFileList))]
		public string WAVFileName
		{
			get { return pWAVFileName; }
			set { pWAVFileName = value; }
		}

		private string pWAVFileName2 = "Alert2.wav";
		[Display(ResourceType = typeof(Custom.Resource), Name = "Washout Sell WAV File", Description = "Sound file to play when a sell signal occurs.", GroupName = "Audio Alerts", Order = 2)]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(LoadFileList))]
		public string WAVFileName2
		{
			get { return pWAVFileName2; }
			set { pWAVFileName2 = value; }
		}
		
		private string pWAVFileZone = "Alert2.wav";
		[Display(ResourceType = typeof(Custom.Resource), Name = "New Imbalance Zone Buy WAV File", Description = "Sound file to play when a buy signal occurs.", GroupName = "Audio Alerts", Order = 11)]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(LoadFileList))]
		public string WAVFileZone
		{
			get { return pWAVFileZone; }
			set { pWAVFileZone = value; }
		}

		private string pWAVFileZone2 = "Alert2.wav";
		[Display(ResourceType = typeof(Custom.Resource), Name = "New Imbalance Zone Sell WAV File", Description = "Sound file to play when a sell signal occurs.", GroupName = "Audio Alerts", Order = 12)]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(LoadFileList))]
		public string WAVFileZone2
		{
			get { return pWAVFileZone2; }
			set { pWAVFileZone2 = value; }
		}		
		
	
		private bool pAudioEnabled5 = true;
		[Display(ResourceType = typeof(Custom.Resource), Name = "New Unfinished Auction Enabled", Description = "", GroupName = "Audio Alerts", Order = 13)]
		//[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        public bool AudioEnabled5
        {
            get { return pAudioEnabled5; }
            set { pAudioEnabled5 = value; }
        }	
		
		private string pWAVFileName5 = "Alert3.wav";
		[Display(ResourceType = typeof(Custom.Resource), Name = "New Unfinished Auction WAV File", Description = "Sound file to play when a buy signal occurs.", GroupName = "Audio Alerts", Order = 14)]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(LoadFileList))]
		public string WAVFileName5
		{
			get { return pWAVFileName5; }
			set { pWAVFileName5 = value; }
		}
		
		private bool pAudioEnabled6 = false;
		[Display(ResourceType = typeof(Custom.Resource), Name = "New Magnet Enabled", Description = "", GroupName = "Audio Alerts", Order = 15)]
		//[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        public bool AudioEnabled6
        {
            get { return pAudioEnabled6; }
            set { pAudioEnabled6 = value; }
        }	
		
		private string pWAVFileName6 = "Alert4.wav";
		[Display(ResourceType = typeof(Custom.Resource), Name = "New Magnet WAV File", Description = "Sound file to play when a buy signal occurs.", GroupName = "Audio Alerts", Order = 16)]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(LoadFileList))]
		public string WAVFileName6
		{
			get { return pWAVFileName6; }
			set { pWAVFileName6 = value; }
		}
		
		
				
		private bool pAudioEnabled9 = true;
		[Display(ResourceType = typeof(Custom.Resource), Name = "New Block Trade Enabled", Description = "", GroupName = "Audio Alerts", Order = 20)]
		//[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
        public bool AudioEnabled9
        {
            get { return pAudioEnabled9; }
            set { pAudioEnabled9 = value; }
        }	
		
	
		
		private string pWAVFile9Buy = "Alert2.wav";
		[Display(ResourceType = typeof(Custom.Resource), Name = "New Block Trade Buy WAV File", Description = "Sound file to play when a buy signal occurs.", GroupName = "Audio Alerts", Order = 21)]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(LoadFileList))]
		public string WAVFile9Buy
		{
			get { return pWAVFile9Buy; }
			set { pWAVFile9Buy = value; }
		}

		private string pWAVFile9Sell = "Alert2.wav";
		[Display(ResourceType = typeof(Custom.Resource), Name = "New Block Trade Sell WAV File", Description = "Sound file to play when a sell signal occurs.", GroupName = "Audio Alerts", Order = 22)]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(LoadFileList))]
		public string WAVFile9Sell
		{
			get { return pWAVFile9Sell; }
			set { pWAVFile9Sell = value; }
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
		
	
		
		
		
		
		private Brush pSimBackColor = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(20,40,60));
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Order Panel Display",  Name = "Background Color Sim", Order = 2001)]
		public Brush SimBackColor
		{
			get { return pSimBackColor; } set { pSimBackColor = value; }
		}
		[Browsable(false)]
		public string SimBackColorS
		{
			get { return Serialize.BrushToString(pSimBackColor); } set { pSimBackColor = Serialize.StringToBrush(value); }
		}
				
		private Brush pLiveBackColor = Brushes.Transparent;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Order Panel Display",  Name = "Background Color Live", Order = 2000)]
		public Brush LiveBackColor
		{
			get { return pLiveBackColor; } set { pLiveBackColor = value; }
		}
		[Browsable(false)]
		public string LiveBackColorS
		{
			get { return Serialize.BrushToString(pLiveBackColor); } set { pLiveBackColor = Serialize.StringToBrush(value); }
		}		
		
		
		
		
		// exeuction inputs
		
		
		
		private string pEH = "FlatEntries";
//		[Description("All - submit an order on every arrow. Unique - submit an order only when the account is flat or when a position will be reversed. Flat - submit an order only when the account is flat.")]
//		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Order Panel Display", Name = "Entry Handling", Description = "",  Order = 7)]
//		//[RefreshProperties(RefreshProperties.All)]
//		[TypeConverter(typeof(EntryHandling))]
//		public string EH
//		{
//			get { return pEH; }
//			set { pEH = value; }
//		}		
		
		private SimpleFont pTextFont66 = new SimpleFont("Arial", 11);
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Order Flag Display", Name = "Text Font", Description = "", Order = 102)]
		public SimpleFont TextFont66
        {
            get { return pTextFont66; }
            set { pTextFont66= value; }
        }	
		
		private Brush pOrderTargetColor = Brushes.Lime;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Order Flag Display",  Name = "Color Target", Order = 20)]
		public Brush OrderTargetColor
		{
			get { return pOrderTargetColor; } set { pOrderTargetColor = value; }
		}
		[Browsable(false)]
		public string OrderTargetColorS
		{
			get { return Serialize.BrushToString(pOrderTargetColor); } set { pOrderTargetColor = Serialize.StringToBrush(value); }
		}
		
		
		private Brush pOrderStopColor = Brushes.Red;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Order Flag Display", Name = "Color Stop", Order = 20)]
		public Brush OrderStopColor
		{
			get { return pOrderStopColor; } set { pOrderStopColor = value; }
		}
		[Browsable(false)]
		public string OrderStopColorS
		{
			get { return Serialize.BrushToString(pOrderStopColor); } set { pOrderStopColor = Serialize.StringToBrush(value); }
		}
		
		
		private Brush pOrderStopMarketColor = Brushes.Pink;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Order Flag Display", Name = "Color StopMarket", Order = 20)]
		public Brush OrderStopMarketColor
		{
			get { return pOrderStopMarketColor; } set { pOrderStopMarketColor = value; }
		}
		[Browsable(false)]
		public string OrderStopMarketColorS
		{
			get { return Serialize.BrushToString(pOrderStopMarketColor); } set { pOrderStopMarketColor = Serialize.StringToBrush(value); }
		}
				
		private Brush pOrderStopLimitColor = Brushes.Violet;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Order Flag Display", Name = "Color StopLimit", Order = 20)]
		public Brush OrderStopLimitColor
		{
			get { return pOrderStopLimitColor; } set { pOrderStopLimitColor = value; }
		}
		[Browsable(false)]
		public string OrderStopLimitColorS
		{
			get { return Serialize.BrushToString(pOrderStopLimitColor); } set { pOrderStopLimitColor = Serialize.StringToBrush(value); }
		}		
		
		private Brush pOrderLimitColor = Brushes.Cyan;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Order Flag Display", Name = "Color Limit", Order = 20)]
		public Brush OrderLimitColor
		{
			get { return pOrderLimitColor; } set { pOrderLimitColor = value; }
		}
		[Browsable(false)]
		public string OrderLimitColorS
		{
			get { return Serialize.BrushToString(pOrderLimitColor); } set { pOrderLimitColor = Serialize.StringToBrush(value); }
		}
		
		
		private Brush pOrderMITColor = Brushes.SpringGreen;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Order Flag Display", Name = "Color MIT", Order = 20)]
		public Brush OrderMITColor
		{
			get { return pOrderMITColor; } set { pOrderMITColor = value; }
		}
		[Browsable(false)]
		public string OrderMITColorS
		{
			get { return Serialize.BrushToString(pOrderMITColor); } set { pOrderMITColor = Serialize.StringToBrush(value); }
		}
		
		
		private Brush pOrderMovingColor = Brushes.DimGray;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Order Flag Display", Name = "Color Order Moving", Order = 28)]
		public Brush OrderMovingColor
		{
			get { return pOrderMovingColor; } set { pOrderMovingColor = value; }
		}
		[Browsable(false)]
		public string OrderMovingColorS
		{
			get { return Serialize.BrushToString(pOrderMovingColor); } set { pOrderMovingColor = Serialize.StringToBrush(value); }
		}		
		
		
		private Brush pOrderChangeColor = Brushes.Orange;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Order Flag Display", Name = "Color Order Change", Order = 29)]
		public Brush OrderChangeColor
		{
			get { return pOrderChangeColor; } set { pOrderChangeColor = value; }
		}
		[Browsable(false)]
		public string OrderChangeColorS
		{
			get { return Serialize.BrushToString(pOrderChangeColor); } set { pOrderChangeColor = Serialize.StringToBrush(value); }
		}		
		
		private Brush pOrderSummaryColor = Brushes.LightGray;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Order Flag Display", Name = "Color Order Summary", Order = 30)]
		public Brush OrderSummaryColor
		{
			get { return pOrderSummaryColor; } set { pOrderSummaryColor = value; }
		}
		[Browsable(false)]
		public string OrderSummaryColorS
		{
			get { return Serialize.BrushToString(pOrderSummaryColor); } set { pOrderSummaryColor = Serialize.StringToBrush(value); }
		}
			
		
	
		private bool pShowOrderSummary = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Order Flag Display", Name = "Summary Display Enabled", Order = 65)]
		[RefreshProperties(RefreshProperties.All)]
        public bool ShowOrderSummary
        {
            get { return pShowOrderSummary; }
            set { pShowOrderSummary = value; }
        }	
		
		private string pThisSummaryTypeMode = "Ticks";
		[Description("")]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Order Flag Display", Name = "Summary Display Mode", Order = 66)]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(SummaryTypeMode))]
		public string ThisSummaryTypeMode
		{
			get { return pThisSummaryTypeMode; }
			set { pThisSummaryTypeMode = value; }
		}
		
		
		internal class SummaryTypeMode : StringConverter
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
				return new StandardValuesCollection( new String[] {"Ticks" } );
			}
		}	
		
		
		
		private bool pShowOrdersOffChart = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Order Flag Display", Name = "Hidden Orders Enabled", Order = 60)]
		[RefreshProperties(RefreshProperties.All)]
        public bool ShowOrdersOffChart
        {
            get { return pShowOrdersOffChart; }
            set { pShowOrdersOffChart = value; }
        }			
		
		private int pHiddenOrdersOpacity = 50;
        [Range(20, 100)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Order Flag Display", Name = "Hidden Orders Opacity (%)", Order = 61)]
        public int HiddenOrdersOpacity
        {
            get { return pHiddenOrdersOpacity; }
            set { pHiddenOrdersOpacity = value; }
        }	
						

		
		private bool pSplitStopDisplay = false;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Order Flag Display", Name = "Split Stop Loss Orders Enabled", Order = 70)]
		[RefreshProperties(RefreshProperties.All)]
        public bool SplitStopDisplay
        {
            get { return pSplitStopDisplay; }
            set { pSplitStopDisplay = value; }
        }			
		

        private int pSummaryColumnWidth = 50;
//        [Range(1, int.MaxValue)]
//        [Display(ResourceType = typeof(Custom.Resource), Name = "Summary Width", GroupName = "Order Flag Display", Order = 51)]
//        public int SummaryColumnWidth
//        {
//            get { return pSummaryColumnWidth; }
//            set { pSummaryColumnWidth = value; }
//        }
		
		
		
		
		
		private bool pShowRealizedPNLWhenFlat = true;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Show Realized PNL When Flat", GroupName = "Profit And Loss Display", Order = 70)]
		[Description("show realized pnl when flat")]
        public bool ShowRealizedPNLWhenFlat
        {
            get { return pShowRealizedPNLWhenFlat; }
            set { pShowRealizedPNLWhenFlat = value; }
        }		
		
		private bool pShowCloseButton = true;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Close Button Enabled", GroupName = "Profit And Loss Display", Order = 70)]
		[Description("")]
        public bool ShowCloseButton
        {
            get { return pShowCloseButton; }
            set { pShowCloseButton = value; }
        }					
		private bool pShowBEButton = true;
        [Display(ResourceType = typeof(Custom.Resource), Name = "BE Button Enabled", GroupName = "Profit And Loss Display", Order = 71)]
		[Description("")]
        public bool ShowBEButton
        {
            get { return pShowBEButton; }
            set { pShowBEButton = value; }
        }	
		
		
        private int pBEOffset = 0;
        [Range(0, int.MaxValue)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "BE - Offset (Ticks)", GroupName = "Profit And Loss Display", Order = 72)]
		[Description("number of ticks to offset the limit order in looking for a better fill price.")]
        public int BEOffset
        {
            get { return pBEOffset; }
            set { pBEOffset = value; }
        }
		

		
		
		
		private bool pBEMoveStops = true;
        [Display(ResourceType = typeof(Custom.Resource), Name = "BE - Move Stop Loss", GroupName = "Profit And Loss Display", Order = 75)]
		[RefreshProperties(RefreshProperties.All)]
        [Description("if the trade is in the profit, move the stop loss to break even.")]
		public bool BEMoveStops
        {
            get { return pBEMoveStops; }
            set { pBEMoveStops = value; }
        }			
				
		private bool pBEMoveTargets = true;
        [Display(ResourceType = typeof(Custom.Resource), Name = "BE - Move Targets", GroupName = "Profit And Loss Display", Order = 76)]
		[RefreshProperties(RefreshProperties.All)]
		[Description("if the trade is in the negative, move the profit targets to break even.")]
        public bool BEMoveTargets
        {
            get { return pBEMoveTargets; }
            set { pBEMoveTargets = value; }
        }			
		

		
		
		private string pProfitAndLossType2 = "Currency";
		[Description("")]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Profit And Loss Display", Name = "Panel Display Mode", Order = 2)]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(ProfitAndLossTypeMode))]
		public string ProfitAndLossType2
		{
			get { return pProfitAndLossType2; }
			set { pProfitAndLossType2 = value; }
		}
		
		
		
		private string pProfitAndLossType = "Points";
		[Description("")]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Profit And Loss Display", Name = "Chart Display Mode", Order = 2)]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(ProfitAndLossTypeMode))]
		public string ProfitAndLossType
		{
			get { return pProfitAndLossType; }
			set { pProfitAndLossType = value; }
		}
		
		
		internal class ProfitAndLossTypeMode : StringConverter
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
				return new StandardValuesCollection( new String[] {"Currency", "Percent", "Pips", "Points", "Ticks", } );
			}
		}	
		

		// CLICK
					
		private bool pUseMIT = false;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Click Entry Orders", Name = "MIT", Order = 2)]
        public bool UseMIT
        {
            get { return pUseMIT; }
            set { pUseMIT = value; }
        }			
		
		private bool pUseSLM = false;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Click Entry Orders", Name = "SLM", Order = 3)]
        public bool UseSLM
        {
            get { return pUseSLM; }
            set { pUseSLM = value; }
        }			
		
		private bool pClickEntryOrdersEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Click Entry Orders", Name = "Enabled", Order = 0)]
		[RefreshProperties(RefreshProperties.All)]
        public bool ClickEntryOrdersEnabled
        {
            get { return pClickEntryOrdersEnabled; }
            set { pClickEntryOrdersEnabled = value; }
        }
		
        private int pSLOffset = 0;
        [Range(0, int.MaxValue)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Click Entry Orders", Name = "SLM Offset (Ticks)", Order = 1)]
        public int SLOffset
        {
            get { return pSLOffset; }
            set { pSLOffset = value; }
        }
		
		
		private Brush pClickUPColor = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0,100,0));
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Click Entry Orders",  Name = "Buy Flag Color", Order = 20)]
		public Brush ClickUPColor
		{
			get { return pClickUPColor; } set { pClickUPColor = value; }
		}
		[Browsable(false)]
		public string ClickUPColorS
		{
			get { return Serialize.BrushToString(pClickUPColor); } set { pClickUPColor = Serialize.StringToBrush(value); }
		}
		
		
		private Brush pClickDNColor = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(100,0,0));
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Click Entry Orders", Name = "Sell Flag Color", Order = 20)]
		public Brush ClickDNColor
		{
			get { return pClickDNColor; } set { pClickDNColor = value; }
		}
		[Browsable(false)]
		public string ClickDNColorS
		{
			get { return Serialize.BrushToString(pClickDNColor); } set { pClickDNColor = Serialize.StringToBrush(value); }
		}
				

		
		private bool pWashoutEntryOrdersEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Washout Entry Orders", Name = "Enabled", Order = 0)]
        public bool WashoutEntryOrdersEnabled
        {
            get { return pWashoutEntryOrdersEnabled; }
            set { pWashoutEntryOrdersEnabled = value; }
        }
		
		
		private bool pWashoutEntryOrdersIB = false;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Washout Entry Orders", Name = "IB Enabled", Order = 0)]
        public bool WashoutEntryOrdersIB
        {
            get { return pWashoutEntryOrdersIB; }
            set { pWashoutEntryOrdersIB = value; }
        }
		
		
		
		
		private bool pCloseEntryOrdersEnabled = false;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Close Entry Orders", Name = "Enabled", Order = 0)]
        public bool CloseEntryOrdersEnabled
        {
            get { return pCloseEntryOrdersEnabled; }
            set { pCloseEntryOrdersEnabled = value; }
        }
		
		
		
		private bool pStackEntryOrdersEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Stack Entry Orders", Name = "Enabled", Order = 0)]
        public bool StackEntryOrdersEnabled
        {
            get { return pStackEntryOrdersEnabled; }
            set { pStackEntryOrdersEnabled = value; }
        }
		
		
		
		private string pBuyStackMode = "Long";
		[Description("")]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Stack Entry Orders", Name = "Buy Entry Mode", Order = 2)]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(StackEntryMode))]
		public string BuyStackMode
		{
			get { return pBuyStackMode; }
			set { pBuyStackMode = value; }
		}
		
		private string pSellStackMode = "Short";
		[Description("")]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Stack Entry Orders", Name = "Sell Entry Mode", Order = 3)]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(StackEntryMode))]
		public string SellStackMode
		{
			get { return pSellStackMode; }
			set { pSellStackMode = value; }
		}
		
		internal class StackEntryMode : StringConverter
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
				return new StandardValuesCollection( new String[] {"Long", "Short"} );
			}
		}	
				

		private string pBuyCloseMode = "All";
		[Description("")]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Close Entry Orders", Name = "Buy Entry Mode", Order = 2)]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(CloseEntryMode))]
		public string BuyCloseMode
		{
			get { return pBuyCloseMode; }
			set { pBuyCloseMode = value; }
		}
		
		private string pSellCloseMode = "All";
		[Description("")]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Close Entry Orders", Name = "Sell Entry Mode", Order = 3)]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(CloseEntryMode))]
		public string SellCloseMode
		{
			get { return pSellCloseMode; }
			set { pSellCloseMode = value; }
		}
		

		private string pBuyWashoutMode = "Long";
		[Description("")]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Close Entry Orders", Name = "Buy Entry Mode", Order = 2)]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(StackEntryMode))]
		public string BuyWashoutMode
		{
			get { return pBuyWashoutMode; }
			set { pBuyWashoutMode = value; }
		}
		
		private string pSellWashoutMode = "Short";
		[Description("")]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Close Entry Orders", Name = "Sell Entry Mode", Order = 3)]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(StackEntryMode))]
		public string SellWashoutMode
		{
			get { return pSellWashoutMode; }
			set { pSellWashoutMode = value; }
		}
		
		
		
		
		
		
		
		internal class CloseEntryMode : StringConverter
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
				return new StandardValuesCollection( new String[] {"Long", "Short", "All"} );
			}
		}			
		
		
		
		
		
		
		
		
		
		private bool pBracketEntryOrdersEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Bracket Entry Orders", Name = "Enabled", Order = 0)]
		[RefreshProperties(RefreshProperties.All)]
        public bool BracketEntryOrdersEnabled
        {
            get { return pBracketEntryOrdersEnabled; }
            set { pBracketEntryOrdersEnabled = value; }
        }	
		
		
		

				
		private string pThisEntryType3 = "SLM";
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Bracket Entry Orders", Name = "Entry Order Type", Description = "",  Order = 8)]
		[Description("")]
		//[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(TotalMode2232))]
		public string ThisEntryType3
		{
			get { return pThisEntryType3; }
			set { pThisEntryType3 = value; }
		}		
			
	    private int pLimitOrderOffset3 = 10;
        [Range(0, int.MaxValue)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Bracket Entry Orders", Name = "Entry Offset (Ticks)", Order = 12)]
		[Description("number of ticks to offset the two stop entry orders.")]
        public int LimitOrderOffset3
        {
            get { return pLimitOrderOffset3; }
            set { pLimitOrderOffset3 = value; }
        }	
		
        private int pSLOffset2 = 5;
        [Range(0, int.MaxValue)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Bracket Entry Orders", Name = "SLM Offset (Ticks)", Order = 14)]
        public int SLOffset2
        {
            get { return pSLOffset2; }
            set { pSLOffset2 = value; }
        }
				

		private Brush pColorBracket2 = Brushes.DimGray;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Normal Button Color", GroupName = "Bracket Entry Orders", Order = 50)]
		public Brush ColorBracket2
		{
			get { return pColorBracket2; } set { pColorBracket2 = value; }
		}
		[Browsable(false)]
		public string ColorBracket2S
		{
			get { return Serialize.BrushToString(pColorBracket2); } set { pColorBracket2 = Serialize.StringToBrush(value); }
		}	
		
	
		
		private Brush pColorBracket = Brushes.White;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Active Button Color", GroupName = "Bracket Entry Orders", Order = 52)]
		public Brush ColorBracket
		{
			get { return pColorBracket; } set { pColorBracket = value; }
		}
		[Browsable(false)]
		public string ColorBracketS
		{
			get { return Serialize.BrushToString(pColorBracket); } set { pColorBracket = Serialize.StringToBrush(value); }
		}	
		
		

					
		private bool pLimit1EntryOrdersEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Limit Entry Orders 1", Name = "Enabled", Order = 0)]
        public bool Limit1EntryOrdersEnabled
        {
            get { return pLimit1EntryOrdersEnabled; }
            set { pLimit1EntryOrdersEnabled = value; }
        }		
		
		
		private bool pLimit2EntryOrdersEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Limit Entry Orders 2", Name = "Enabled", Order = 0)]
        public bool Limit2EntryOrdersEnabled
        {
            get { return pLimit2EntryOrdersEnabled; }
            set { pLimit2EntryOrdersEnabled = value; }
        }		
		
		private int pLimitOrderOffset4 = 0;
        [Range(int.MinValue, int.MaxValue)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Limit Entry Orders 1", Name = "Entry Offset (Ticks)", Order = 12)]
		[Description("number of ticks to offset the two stop entry orders.")]
        public int LimitOrderOffset4
        {
            get { return pLimitOrderOffset4; }
            set { pLimitOrderOffset4 = value; }
        }	
		
		private int pLimitOrderOffset5 = 1;
        [Range(0, int.MaxValue)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Limit Entry Orders 2", Name = "Entry Offset (Ticks)", Order = 12)]
		[Description("number of ticks to offset the two stop entry orders.")]
        public int LimitOrderOffset5
        {
            get { return pLimitOrderOffset5; }
            set { pLimitOrderOffset5 = value; }
        }			
		// MARKET
		
		
		
		
		private bool pMarketEntryOrdersEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Market Entry Orders", Name = "Enabled", Order = 0)]
        public bool MarketEntryOrdersEnabled
        {
            get { return pMarketEntryOrdersEnabled; }
            set { pMarketEntryOrdersEnabled = value; }
        }		
		
		
		// CLOSE
		
		private string pThisEntryType1 = "Market";
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Close Entry Orders", Name = "Entry Order Type", Description = "",  Order = 8)]
		[Description("")]
		//[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(TotalMode22))]
		public string ThisEntryType1
		{
			get { return pThisEntryType1; }
			set { pThisEntryType1 = value; }
		}		
			
		
		
	    private int pLimitOrderOffset1 = 1;
        [Range(0, int.MaxValue)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Close Entry Orders", Name = "Limit Offset (Ticks)", Order = 11)]
		[Description("number of ticks to offset the limit order in looking for a better fill price.")]
        public int LimitOrderOffset1
        {
            get { return pLimitOrderOffset1; }
            set { pLimitOrderOffset1 = value; }
        }	
		
		
			
	    private int pLimitOrderOffset6 = 1;
        [Range(0, int.MaxValue)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Washout Entry Orders", Name = "Limit Offset (Ticks)", Order = 11)]
		[Description("number of ticks to offset the limit order in looking for a better fill price.")]
        public int LimitOrderOffset6
        {
            get { return pLimitOrderOffset6; }
            set { pLimitOrderOffset6 = value; }
        }	
		
		
		private string pThisEntryType4 = "Market";
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Washout Entry Orders", Name = "Entry Order Type", Description = "",  Order = 8)]
		[Description("")]
		//[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(TotalMode22))]
		public string ThisEntryType4
		{
			get { return pThisEntryType4; }
			set { pThisEntryType4 = value; }
		}		
			
		
		
		
		private string pThisEntryType2 = "Market";
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Stack Entry Orders", Name = "Entry Order Type", Description = "",  Order = 8)]
		[Description("")]
		//[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(TotalMode22))]
		public string ThisEntryType2
		{
			get { return pThisEntryType2; }
			set { pThisEntryType2 = value; }
		}		
			
			
			
		
	    private int pLimitOrderOffset2 = 1;
        [Range(0, int.MaxValue)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Stack Entry Orders", Name = "Limit Offset (Ticks)", Order = 12)]
		[Description("number of ticks to offset the limit order in looking for a better fill price.")]
        public int LimitOrderOffset2
        {
            get { return pLimitOrderOffset2; }
            set { pLimitOrderOffset2 = value; }
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
				return new StandardValuesCollection( new String[] {"Market", "Limit"} );
			}
		}				
			
		internal class TotalMode2232 : StringConverter
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
				return new StandardValuesCollection( new String[] {"STP", "SLM"} );
			}
		}	

		

				
	
		
		private bool pAutoEnabled = true;
//        [Display(ResourceType = typeof(Custom.Resource), Name = "Auto Enabled", GroupName = "Order Panel Display", Order = 13)]
//        public bool AutoEnabled
//        {
//            get { return pAutoEnabled; }
//            set { pAutoEnabled = value; }
//        }	
		
		

        private int pLastQuantity = 1;
        [Range(1, int.MaxValue)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Quantity", GroupName = "Order Panel Display", Order = 1)]
        public int LastQuantity
        {
            get { return pLastQuantity; }
            set { pLastQuantity = value; }
        }
		
        private string pLastAccount = null;
       // [Range(1, int.MaxValue)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Account", GroupName = "Order Panel Display", Order = 1)]
        public string LastAccount
        {
            get { return pLastAccount; }
            set { pLastAccount = value; }
        }		
		
        private int pLastATM = -10;
      //  [Range(1, int.MaxValue)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "ATM", GroupName = "Order Panel Display", Order = 1)]
        public int LastATM
        {
            get { return pLastATM; }
            set { pLastATM = value; }
        }
				
		
        private int pSpaceBetweenGroups = 20;
      	[Range(0, int.MaxValue)]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Space Between Sections (Pixels)", GroupName = "Order Panel Display", Order = 111)]
        public int SpaceBetweenGroups
        {
            get { return pSpaceBetweenGroups; }
            set { pSpaceBetweenGroups = value; }
        }		
		

		

		

		private bool pOrderPanelOn = false;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Panel Enabled", GroupName = "Order Panel Display", Order = 13)]
		[RefreshProperties(RefreshProperties.All)]
        public bool OrderPanelOn
        {
            get { return pOrderPanelOn; }
            set { pOrderPanelOn = value; }
        }		
		
		private bool pOrdersDisplayOn = false;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Orders Enabled", GroupName = "Order Panel Display", Order = 13)]
		[RefreshProperties(RefreshProperties.All)]
        public bool OrdersDisplayOn
        {
            get { return pOrdersDisplayOn; }
            set { pOrdersDisplayOn = value; }
        }				
		
				
		private Brush pColorBuyEMA2 = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0,80,0));
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Buy Normal Button Color", GroupName = "Order Panel Display", Order = 50)]
		public Brush ColorBuyEMA2
		{
			get { return pColorBuyEMA2; } set { pColorBuyEMA2 = value; }
		}
		[Browsable(false)]
		public string ColorBuyEMA2S
		{
			get { return Serialize.BrushToString(pColorBuyEMA2); } set { pColorBuyEMA2 = Serialize.StringToBrush(value); }
		}	
		
		
		private Brush pColorBuyEMA = Brushes.Lime;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Buy Active Button Color", GroupName = "Order Panel Display", Order = 51)]
		public Brush ColorBuyEMA
		{
			get { return pColorBuyEMA; } set { pColorBuyEMA = value; }
		}
		[Browsable(false)]
		public string ColorBuyEMAS
		{
			get { return Serialize.BrushToString(pColorBuyEMA); } set { pColorBuyEMA = Serialize.StringToBrush(value); }
		}	
				
		
		
		private Brush pColorSellEMA2 = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(80,0,0));
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Sell Normal Button Color", GroupName = "Order Panel Display", Order = 52)]
		public Brush ColorSellEMA2
		{
			get { return pColorSellEMA2; } set { pColorSellEMA2 = value; }
		}
		[Browsable(false)]
		public string ColorSellEMA2S
		{
			get { return Serialize.BrushToString(pColorSellEMA2); } set { pColorSellEMA2 = Serialize.StringToBrush(value); }
		}	
		
	

		
		
	
		private Brush pColorSellEMA = Brushes.Red;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Sell Active Button Color", GroupName = "Order Panel Display", Order = 53)]
		public Brush ColorSellEMA
		{
			get { return pColorSellEMA; } set { pColorSellEMA = value; }
		}
		[Browsable(false)]
		public string ColorSellEMAS
		{
			get { return Serialize.BrushToString(pColorSellEMA); } set { pColorSellEMA = Serialize.StringToBrush(value); }
		}	
		
		

		

		private Brush pColorNeutralEMA2 = Brushes.DimGray;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Neutral Normal Button Color", GroupName = "Order Panel Display", Order = 54)]
		public Brush ColorNeutralEMA2
		{
			get { return pColorNeutralEMA2; } set { pColorNeutralEMA2 = value; }
		}
		[Browsable(false)]
		public string ColorNeutralEMA2S
		{
			get { return Serialize.BrushToString(pColorNeutralEMA2); } set { pColorNeutralEMA2 = Serialize.StringToBrush(value); }
		}	

		
		
		private Brush pColorNeutralEMA = Brushes.White;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Neutral Active Button Color", GroupName = "Order Panel Display", Order = 55)]
		public Brush ColorNeutralEMA
		{
			get { return pColorNeutralEMA; } set { pColorNeutralEMA = value; }
		}
		[Browsable(false)]
		public string ColorNeutralEMAS
		{
			get { return Serialize.BrushToString(pColorNeutralEMA); } set { pColorNeutralEMA = Serialize.StringToBrush(value); }
		}	
				
		
		
		private Brush pColorButtonReverse = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(70,50,20));
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Reverse Button Color", GroupName = "Order Panel Display", Order = 57)]
		public Brush ColorButtonReverse
		{
			get { return pColorButtonReverse; } set { pColorButtonReverse = value; }
		}
		[Browsable(false)]
		public string ColorButtonReverseS
		{
			get { return Serialize.BrushToString(pColorButtonReverse); } set { pColorButtonReverse = Serialize.StringToBrush(value); }
		}	
				

		
		private Brush pColorButtonCloseAll = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(70,50,20));
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Close All Button Color", GroupName = "Order Panel Display", Order = 57)]
		public Brush ColorButtonCloseAll
		{
			get { return pColorButtonCloseAll; } set { pColorButtonCloseAll = value; }
		}
		[Browsable(false)]
		public string ColorButtonCloseAllS
		{
			get { return Serialize.BrushToString(pColorButtonCloseAll); } set { pColorButtonCloseAll = Serialize.StringToBrush(value); }
		}	
		
				
		
		
		
		
		private Brush pColorButtonOrderType = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(40,40,40));
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Order Panel Display", Name = "Order Type Button Color", Order = 58)]
		public Brush ColorButtonOrderType
		{
			get { return pColorButtonOrderType; } set { pColorButtonOrderType = value; }
		}
		[Browsable(false)]
		public string ColorButtonOrderTypeS
		{
			get { return Serialize.BrushToString(pColorButtonOrderType); } set { pColorButtonOrderType = Serialize.StringToBrush(value); }
		}	
				



		
		private bool pActiveOutlineEnabled = true;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Chart  Active Outline Enabled", GroupName = "Order Panel Display", Order = 60)]
        public bool ActiveOutlineEnabled
        {
            get { return pActiveOutlineEnabled; }
            set { pActiveOutlineEnabled = value; }
        }	
		
				
		
  
		private Stroke pOrderUpOutlineStroke = new Stroke(Brushes.Lime, DashStyleHelper.Solid, 3);
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Order Panel Display", Name = "Chart Buy Active Outline", Order = 61)]
        public Stroke OrderUpOutlineStroke
        {
            get { return pOrderUpOutlineStroke; }
            set { pOrderUpOutlineStroke = value; }
        }		
		
        private Stroke pOrderDnOutlineStroke = new Stroke(Brushes.Red, DashStyleHelper.Solid, 3);
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Order Panel Display", Name = "Chart Sell Active Outline", Order = 62)]
        public Stroke OrderDnOutlineStroke
        {
            get { return pOrderDnOutlineStroke; }
            set { pOrderDnOutlineStroke = value; }
        }		
		
		private Stroke pOrderBothOutlineStroke = new Stroke(Brushes.White, DashStyleHelper.Solid, 3);
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Order Panel Display", Name = "Chart Buy & Sell Active Outline", Order = 63)]
        public Stroke OrderBothOutlineStroke
        {
            get { return pOrderBothOutlineStroke; }
            set { pOrderBothOutlineStroke = value; }
        }					
		
			
		
		private bool pShowReverse = false;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Reverse Button Enabled", GroupName = "Trade Management", Order = 12)]
		[RefreshProperties(RefreshProperties.All)]
        public bool ShowReverse
        {
            get { return pShowReverse; }
            set { pShowReverse = value; }
        }	
		
		
		private bool pCloseOnStopCancel = false;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Close On Exit Order Cancelled", GroupName = "Trade Management", Order = 13)]
		[RefreshProperties(RefreshProperties.All)]
        public bool CloseOnStopCancel
        {
            get { return pCloseOnStopCancel; }
            set { pCloseOnStopCancel = value; }
        }		

		private string pDataCalcMode2 = "Normal";
		[Description("")]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Data", Name = "Process Mode", Order = 1)]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(ProfitAndLossTypeMode3))]
		public string DataCalcMode2
		{
			get { return pDataCalcMode2; }
			set { pDataCalcMode2 = value; }
		}
		
		
		internal class ProfitAndLossTypeMode3 : StringConverter
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
				return new StandardValuesCollection( new String[] {"Normal", "Volumetric"} );
				//return new StandardValuesCollection( new String[] {"Bid/Ask", "Tick", "Volumetric"  } );
			}
		}	
		
	
		private string pDataCalcMode = "Tick";
		[Description("")]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "Data", Name = "Calculation Mode", Order = 2)]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(ProfitAndLossTypeMode2))]
		public string DataCalcMode
		{
			get { return pDataCalcMode; }
			set { pDataCalcMode = value; }
		}
		
		
		internal class ProfitAndLossTypeMode2 : StringConverter
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
				return new StandardValuesCollection( new String[] {"Bid/Ask", "Tick"} );
				//return new StandardValuesCollection( new String[] {"Bid/Ask", "Tick", "Volumetric"  } );
			}
		}	
		

		
		
		
        private int pProcessFrequencyMS = 250;
      //  [Range(1, int.MaxValue)]
        [Display(ResourceType = typeof(Custom.Resource), GroupName = "Data", Name = "Calculation Frequency (MS)", Order = 11)]
        public int ProcessFrequencyMS
        {
            get { return pProcessFrequencyMS; }
            set { pProcessFrequencyMS = value; }
        }
						
		
	
	
		private string pLicensingEmailAddress = "";
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), GroupName = "License", Name = "Email Address", Order = 54, Description = "")]
        public string LicensingEmailAddress
        {
            get { return pLicensingEmailAddress; }
            set { pLicensingEmailAddress = value; }
        }			
		
			
		
		
		
		
//		[NinjaScriptProperty]
//		[ReadOnly(true)]
//		[Display(Name = "Indicator Version", GroupName = "Version", Order = 100)]
//		public string indicatorVersion
//		{ get; set; }
		
//		[NinjaScriptProperty]
//		[ReadOnly(true)]
//		[Display(Name = "NinjaTrader Version", GroupName = "Version", Order = 101)]
//		public string ntVersion
//		{ get; set; }

		
//		[NinjaScriptProperty]
//		[ReadOnly(true)]
//		[Display(Name = "Release Date", GroupName = "Version", Order = 102)]
//		public string releaseDate
//		{ get; set; }		
		
		
		
		
	}
	
	
		// Hide UserDefinedValues properties when not in use by the HLCCalculationMode.UserDefinedValues
	// When creating a custom type converter for indicators it must inherit from NinjaTrader.NinjaScript.IndicatorBaseConverter to work correctly with indicators
	public class VeritasOrderFlowConverter : NinjaTrader.NinjaScript.IndicatorBaseConverter
	{
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) { return true; }

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			PropertyDescriptorCollection propertyDescriptorCollection = base.GetPropertiesSupported(context) ? base.GetProperties(context, value, attributes) : TypeDescriptor.GetProperties(value, attributes);

			VeritasOrderFlow   jbb = (VeritasOrderFlow) value;
			
			//Pivots						thisPivotsInstance			= (Pivots) value;
			
			//bool MagnetsOn = ;
			
			List<string> DeleteThese = new List<string>();
			List<string> DeleteThese2 = new List<string>();

			// Adaptive Text Color: feature kept, input hidden from the property grid for now.
			DeleteThese.Add("AdaptiveTextColor");
			
			if (!jbb.MAGEnabled)
			{			
				//DeleteThese.Add("MAGSpreadMax");
				//DeleteThese.Add("MAGSizeMin");
				DeleteThese.Add("MAGEnabled2");
				DeleteThese.Add("MAGEnabled3");
				DeleteThese.Add("MAGEnabled5");
				DeleteThese.Add("MagnetStroke");
				DeleteThese.Add("MagnetStroke2");
				DeleteThese.Add("MagnetFillColor");
				DeleteThese.Add("MagnetFillOpacity");
			}
			if (!jbb.UFAEnabled)
			{			
				DeleteThese.Add("UFAEnabled2");
				DeleteThese.Add("UFAEnabled3");
				DeleteThese.Add("UFAEnabled5");
				DeleteThese.Add("UFAStroke");
				DeleteThese.Add("UFAStroke2");
				DeleteThese.Add("UFAFillColor");
				DeleteThese.Add("UFAFillOpacity");
				
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
			}			
			
			if (!jbb.ZonesEnabled && !jbb.ZonesEnabled2)
			{			
				DeleteThese.Add("ShowFreshZones");
				DeleteThese.Add("ShowTestedZones");
				DeleteThese.Add("ShowBrokenZones");
				DeleteThese.Add("ZonesTMEnabled");
				DeleteThese.Add("ZonesTSEnabled");
				DeleteThese.Add("TMSupport");
				DeleteThese.Add("TMResistance");
				DeleteThese.Add("ResistanceZColor1");
				DeleteThese.Add("ResistanceZColor2");
				DeleteThese.Add("ResistanceZColor3");
				DeleteThese.Add("SupportZColor1");
				DeleteThese.Add("SupportZColor2");
				DeleteThese.Add("SupportZColor3");
				DeleteThese.Add("ResistanceZOpacity1");
				DeleteThese.Add("ResistanceZOpacity2");
				DeleteThese.Add("ResistanceZOpacity3");
				DeleteThese.Add("SupportZOpacity1");
				DeleteThese.Add("SupportZOpacity2");
				DeleteThese.Add("SupportZOpacity3");
				DeleteThese.Add("ONESupport");
				DeleteThese.Add("ExtendZonesRight");
			
			
			}			
			
		    if (!jbb.ShowImbalance)
			{	
				DeleteThese.Add("ShowImbalanceA");
				DeleteThese.Add("PrintFillImbalances");
				DeleteThese.Add("PrintTextImbalances");
				DeleteThese.Add("TextAdjustImbalances");			
				DeleteThese.Add("RSSize");
				DeleteThese.Add("ColorBidDots");
				DeleteThese.Add("ColorAskDots");
				DeleteThese.Add("ColorBidFill");
				DeleteThese.Add("ColorAskFill");
				DeleteThese.Add("ColorBidText");
				DeleteThese.Add("ColorAskText");				
				DeleteThese.Add("TextFont2Imb");
				
				DeleteThese.Add("ShowImbalanceHash");
				DeleteThese.Add("RSSize2");
				DeleteThese.Add("ColorBidHash");				
				DeleteThese.Add("ColorAskHash");
		
			}
			
			
			
		    if (!jbb.ShowBlocks)
			{	
				DeleteThese.Add("TriSize");
				DeleteThese.Add("ColorBidTri");
				DeleteThese.Add("ColorAskTri");
				DeleteThese.Add("ShowBlocksHover");
				
			}
			
		    if (!jbb.BarCompositeEnabled)
			{	
				DeleteThese.Add("DisplayPOC");
				DeleteThese.Add("DisplayVWAP");
				DeleteThese.Add("DisplayCL");
				DeleteThese.Add("BarPOCFillColor");
				DeleteThese.Add("BarVWAPFillColor");
				DeleteThese.Add("BarClusterFillColor");
				DeleteThese.Add("ClusterStroke");
				
				
			}		
			 
		    if (!jbb.HUDEnabled)
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
			}				
       
	

		    if (!jbb.CompAllLevelsEnabled)
			{	
				DeleteThese.Add("CompPOCEnabled");
				DeleteThese.Add("CompVWAPEnabled");
				DeleteThese.Add("CompVA1Enabled");
				DeleteThese.Add("CompVA2Enabled");
				DeleteThese.Add("CompDHLEnabled");
				DeleteThese.Add("CompPlotsOpacity");
				DeleteThese.Add("Plot1Brush");
				DeleteThese.Add("Plot2Brush");
				DeleteThese.Add("Plot3Brush");
				DeleteThese.Add("Plot4Brush");
				DeleteThese.Add("Plot5Brush");
				DeleteThese.Add("Plot6Brush");
				DeleteThese.Add("Plot7Brush");
				DeleteThese.Add("Plot8Brush");
				DeleteThese.Add("CompLabelsEnabled");
				DeleteThese.Add("ExtendCompRight");		
				DeleteThese.Add("CompLevelsPriceOn");	
				
	

			}
		    if (!jbb.CompositeEnabled)
			{		
		
			
				DeleteThese.Add("CompositeLocation");
				DeleteThese.Add("CompositeResetMode");
				DeleteThese.Add("MarketOpen");
				DeleteThese.Add("CompNumberDisplayMode");
				DeleteThese.Add("CompHistogramDisplayMode");
				DeleteThese.Add("CompLength");
				DeleteThese.Add("CompVOLColor");
				DeleteThese.Add("CompNEColor");
				DeleteThese.Add("CompUPColor");
				DeleteThese.Add("CompDNColor");
				DeleteThese.Add("CompMinOpacity");
				DeleteThese.Add("CompMaxOpacity");
				DeleteThese.Add("CompositeBackEnabled");
				DeleteThese.Add("CompBackOpacity");
				
		
			}				
				
		    if (!jbb.DepthEnabled)
			{	
				DeleteThese.Add("InvLength");
				DeleteThese.Add("BidOpacity");
				DeleteThese.Add("AskOpacity");
				DeleteThese.Add("BidOpacity2");
				DeleteThese.Add("AskOpacity2");				
				DeleteThese.Add("DepthTotalMode");
				DeleteThese.Add("DisplayTotal");
				DeleteThese.Add("TextColorMD");
				DeleteThese.Add("AskHistColor1");
				DeleteThese.Add("AskHistColor2");
				DeleteThese.Add("AskOutColor1");
				DeleteThese.Add("BidHistColor1");
				DeleteThese.Add("BidHistColor2");
				DeleteThese.Add("BidOutColor1");
				DeleteThese.Add("MaxLevels");
				
			}	        
        
		    if (!jbb.PivotLinesEnabled)
			{	
				DeleteThese.Add("LevelsEnabled");
				DeleteThese.Add("HistoricalLevelsEnabled");
				DeleteThese.Add("ColorSwingLow");
				DeleteThese.Add("ColorSwingHigh");
				DeleteThese.Add("ColorSwingLow2");
				DeleteThese.Add("ColorSwingHigh2");

			}								
       
		    if (!jbb.ButtonsEnabled)
			{	
				
				DeleteThese.Add("ColorButtonsOff");
				DeleteThese.Add("ButtonHighlightMode");
				DeleteThese.Add("OnOpacity");
				
				DeleteThese.Add("ButtonSize");
				DeleteThese.Add("SpaceBetweenButtons");
				

			}	
			
		  
		    if (!jbb.AudioEnabledMain)
			{	
				DeleteThese.Add("QuickAudio");
				DeleteThese.Add("WAVFileName");
				DeleteThese.Add("WAVFileName2");
				
				DeleteThese.Add("AudioEnabled1");
				DeleteThese.Add("AudioEnabled2");
				
				DeleteThese.Add("WAVFileZone");
				DeleteThese.Add("WAVFileZone2");	
				
				DeleteThese.Add("AudioEnabled5");
				DeleteThese.Add("WAVFileName5");
				
				DeleteThese.Add("AudioEnabled6");
				DeleteThese.Add("WAVFileName6");	
					
				DeleteThese.Add("AudioEnabled9");
				DeleteThese.Add("WAVFile9Buy");	
				DeleteThese.Add("WAVFile9Sell");	
				
			}			
				
			
			
		
		
		    if (!jbb.TimerEnabled)
			{	
				DeleteThese.Add("TimerOffset");
				DeleteThese.Add("TextFontTime");
				DeleteThese.Add("TimerMainColor");
				DeleteThese.Add("CountDown");
				DeleteThese.Add("ShowPercent");
				
				
			}				
	
		    if (!jbb.ShowLastPriceAll)
			{	
				DeleteThese.Add("ShowLastPriceBox");
				DeleteThese.Add("LastPriceBrush");
				DeleteThese.Add("LastPriceOpacity");
				DeleteThese.Add("ShowLastPriceLine");
				DeleteThese.Add("LastPriceLineStroke");
				DeleteThese.Add("ShowLastPriceMarker2");
				DeleteThese.Add("ColorLastPriceMarker");
				
			}					
					
 		    if (!jbb.ShowLastPriceBox)
			{	
				DeleteThese.Add("LastPriceBrush");
				DeleteThese.Add("LastPriceOpacity");
			}	  
 
 		    if (!jbb.ShowLastPriceLine)
			{	
				DeleteThese.Add("LastPriceLineStroke");				
			}	  
			
 		    if (!jbb.ShowLastPriceMarker2)
			{	
				DeleteThese.Add("ColorLastPriceMarker");			
			}
			
 		    if (!jbb.CrossHairEnabled)
			{	
				DeleteThese.Add("CrossHairXEnabled");
				DeleteThese.Add("CrossHairYEnabled");
				DeleteThese.Add("CrossHairBrush");
				DeleteThese.Add("CrossHairOpacity");
				
			}			
			 
			 
			
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
			
							
			if (!jbb.OrderPanelOn)
			{
				
				DeleteThese.Add("ThisEntryType1");
				DeleteThese.Add("LimitOrderOffset1");
				DeleteThese.Add("ThisEntryType2");
				DeleteThese.Add("LimitOrderOffset2");
				DeleteThese.Add("ColorBuyEMA");
				DeleteThese.Add("ColorBuyEMA2");
				DeleteThese.Add("ColorSellEMA");
				DeleteThese.Add("ColorSellEMA2");	
				DeleteThese.Add("ColorNeutralEMA");
				DeleteThese.Add("ColorNeutralEMA2");					
				DeleteThese.Add("ColorButtonCloseAll");	
				DeleteThese.Add("ColorButtonReverse");	
				DeleteThese.Add("ColorButtonOrderType");	
				
				DeleteThese.Add("ActiveOutlineEnabled");			
 				DeleteThese.Add("OrderUpOutlineStroke");		
				DeleteThese.Add("OrderDnOutlineStroke");  
				DeleteThese.Add("OrderBothOutlineStroke");	
	
				DeleteThese.Add("SpaceBetweenGroups");		
		
					

	
				DeleteThese.Add("TextFont66");
				DeleteThese.Add("OrderTargetColor");
				DeleteThese.Add("OrderStopColor");
				DeleteThese.Add("OrderStopMarketColor");	
				DeleteThese.Add("OrderStopLimitColor");
				DeleteThese.Add("OrderLimitColor");
				DeleteThese.Add("OrderMITColor");
				DeleteThese.Add("OrderMovingColor");
				DeleteThese.Add("OrderChangeColor");
				DeleteThese.Add("OrderSummaryColor");
				DeleteThese.Add("ShowOrderSummary");
				DeleteThese.Add("ThisSummaryTypeMode");
				DeleteThese.Add("ShowOrdersOffChart");
				DeleteThese.Add("HiddenOrdersOpacity");
				
				
				DeleteThese.Add("ShowReverse");
				DeleteThese.Add("CloseOnStopCancel");
							

 				DeleteThese.Add("BracketEntryOrdersEnabled");
				DeleteThese.Add("SLOffset2");
				DeleteThese.Add("ColorBracket2");
				DeleteThese.Add("ColorBracket");
			
		

				
				
				DeleteThese.Add("ShowCloseButton");
				DeleteThese.Add("ShowBEButton");
				DeleteThese.Add("BEMoveStops");
				DeleteThese.Add("BEMoveTargets");
				DeleteThese.Add("BEOffset");	
				
				DeleteThese.Add("UseSLM");
				DeleteThese.Add("UseMIT");
				DeleteThese.Add("SLOffset");
				
				DeleteThese.Add("CloseEntryOrdersEnabled");
				
				DeleteThese.Add("WashoutEntryOrdersEnabled");
				
				
				
				DeleteThese.Add("StackEntryOrdersEnabled");
				DeleteThese.Add("ClickEntryOrdersEnabled");
				DeleteThese.Add("MarketEntryOrdersEnabled");
				
				
 				DeleteThese.Add("ClickUPColor");		
				DeleteThese.Add("ClickDNColor");  
				
				
	

				DeleteThese.Add("ShowRealizedPNLWhenFlat");  
				
				
				DeleteThese.Add("Limit1EntryOrdersEnabled");
				DeleteThese.Add("Limit2EntryOrdersEnabled");
				DeleteThese.Add("LimitOrderOffset4");
				DeleteThese.Add("LimitOrderOffset5");	
				
				DeleteThese.Add("SimBackColor");	
				DeleteThese.Add("LiveBackColor");						

		
			}
	
			if (!jbb.Limit1EntryOrdersEnabled)
			{
 				DeleteThese.Add("LimitOrderOffset4");
		
			}
			
			if (!jbb.Limit2EntryOrdersEnabled)
			{
 				DeleteThese.Add("LimitOrderOffset5");
		
			}
			
			if (!jbb.BracketEntryOrdersEnabled)
			{
 				DeleteThese.Add("ThisEntryType3");
				DeleteThese.Add("LimitOrderOffset3");
 				DeleteThese.Add("SLOffset2");
				DeleteThese.Add("ColorBracket2");
				DeleteThese.Add("ColorBracket");			
			}
			
			
			if (!jbb.ClickEntryOrdersEnabled)
			{
 				DeleteThese.Add("SLOffset");
				DeleteThese.Add("ClickUPColor");
 				DeleteThese.Add("ClickDNColor");
	
			}

	
	
				
		
		
		
		
		
			
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

	public class VeritasOrderFlowPropertyGridWindow : NTWindow, IWorkspacePersistence
	{
		public PropertyGrid propertyGrid;
		public IndicatorBase selectedIndicator;
		private Dictionary<string, object> _openValues = new Dictionary<string, object>();
		private bool _reallyClose = false;

		public VeritasOrderFlowPropertyGridWindow()
		{
			Caption = "Impact Order Flow Properties";
			Width = 600;
			Height = 800;
			WindowStartupLocation = WindowStartupLocation.CenterScreen;
			Content = BuildContent();
			Loaded += OnWindowLoaded;
			Closing += OnWindowClosing;
		}

		private void OnWindowLoaded(object sender, RoutedEventArgs e)
		{
			if (WorkspaceOptions == null)
				WorkspaceOptions = new WorkspaceOptions("VeritasOrderFlowPropertyGridWindow" + Guid.NewGuid().ToString("N"), this);

			if (selectedIndicator == null) return;

			propertyGrid.PropertyValueChanged += OnPropertyValueChanged;

			// Defer the heavy grid build so the window paints and is responsive first.
			Dispatcher.BeginInvoke(new Action(() =>
			{
				RefreshGrid();
				CaptureSnapshot();
			}), System.Windows.Threading.DispatcherPriority.Background);
		}

// Window is reused — re-shown rather than rebuilt — so a normal close just hides it.
		private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (!_reallyClose)
			{
				e.Cancel = true;
				Hide();
			}
		}

		// Re-show the kept-alive window: refresh values and re-snapshot for Cancel/revert.
		public void ReShow()
		{
			Show();
			Activate();
			// Defer the refresh so the window appears instantly, then repopulates.
			Dispatcher.BeginInvoke(new Action(() =>
			{
				RefreshGrid();
				CaptureSnapshot();
			}), System.Windows.Threading.DispatcherPriority.Background);
		}

		// Real close — used when the indicator is removed; bypasses the hide-on-close.
		public void ForceClose()
		{
			_reallyClose = true;
			Close();
		}

		private static PropertyInfo SafeGetProperty(Type type, string name)
		{
			try { return type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly); }
			catch { }
			try { return type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance); }
			catch { }
			return null;
		}

		private void CaptureSnapshot()
		{
			_openValues.Clear();
			if (selectedIndicator == null) return;
			var type = selectedIndicator.GetType();
			foreach (var pi in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
			{
				if (!pi.CanRead || !pi.CanWrite) continue;
				if (pi.GetIndexParameters().Length > 0) continue;
				try { _openValues[pi.Name] = pi.GetValue(selectedIndicator); } catch { }
			}
		}

		private UIElement BuildContent()
		{
			var root = new Grid();
			root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
			root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

			propertyGrid = new PropertyGrid();

			var resetMenuItem = new MenuItem { Header = "Reset to Default" };
			resetMenuItem.Click += OnResetSelectedClick;
			var pgContextMenu = new ContextMenu();
			pgContextMenu.Items.Add(resetMenuItem);
			propertyGrid.ContextMenu = pgContextMenu;

			var scrollViewer = new ScrollViewer
			{
				Content = propertyGrid,
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Stretch
			};
			Grid.SetRow(scrollViewer, 0);
			root.Children.Add(scrollViewer);

			var separator = new Separator
			{
				Margin = new Thickness(0),
				Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(61, 61, 66)),
				Height = 1
			};

			var buttonBar = new StackPanel
			{
				Orientation = Orientation.Horizontal,
				HorizontalAlignment = HorizontalAlignment.Right,
				Margin = new Thickness(0, 21, 0, 0)
			};


			var cancelBtn = new Button
			{
				Content = "Cancel",
				MinWidth = 86,
				MinHeight = 27,
				Margin = new Thickness(0, 0, 7, 0),
				IsCancel = true
			};
			cancelBtn.Click += OnCancelClick;

			var closeBtn = new Button
			{
				Content = "OK",
				MinWidth = 86,
				MinHeight = 27,
				Margin = new Thickness(0),
				IsDefault = true
			};
			closeBtn.Click += (s, ea) => Close();

			buttonBar.Children.Add(cancelBtn);
			buttonBar.Children.Add(closeBtn);

			var bottomPanel = new StackPanel { Orientation = Orientation.Vertical };
			bottomPanel.Children.Add(separator);
			bottomPanel.Children.Add(buttonBar);

			Grid.SetRow(bottomPanel, 1);
			root.Children.Add(bottomPanel);

			return root;
		}

		private void OnPropertyValueChanged(object sender, RoutedEventArgs e)
		{
			if (selectedIndicator == null) return;
			MethodInfo mi = selectedIndicator.GetType().GetMethod("UpdateFromPropertyWindow",
				BindingFlags.Public | BindingFlags.Instance);
			if (mi != null) mi.Invoke(selectedIndicator, null);
		}

		private string GetSelectedPropertyName()
		{
			try
			{
				var selectedPropInfo = propertyGrid.GetType().GetProperty("SelectedProperty");
				if (selectedPropInfo == null) return null;
				var propItem = selectedPropInfo.GetValue(propertyGrid);
				if (propItem == null) return null;

				var pdProp = propItem.GetType().GetProperty("PropertyDescriptor");
				if (pdProp != null)
				{
					var pd = pdProp.GetValue(propItem) as PropertyDescriptor;
					if (pd != null) return pd.Name;
				}

				var nameProp = propItem.GetType().GetProperty("Name");
				if (nameProp != null) return nameProp.GetValue(propItem) as string;
			}
			catch { }
			return null;
		}

		private void OnResetSelectedClick(object sender, RoutedEventArgs e)
		{
			string propName = GetSelectedPropertyName();
			if (propName == null || !_openValues.ContainsKey(propName)) return;
			ResetProperty(propName);
		}

		private void ResetProperty(string name)
		{
			if (!_openValues.ContainsKey(name)) return;
			var pi = SafeGetProperty(selectedIndicator.GetType(), name);
			if (pi == null || !pi.CanWrite) return;
			try
			{
				pi.SetValue(selectedIndicator, _openValues[name]);
				RefreshGrid();
				OnPropertyValueChanged(null, null);
			}
			catch { }
		}

		private void OnResetAllClick(object sender, RoutedEventArgs e)
		{
			var type = selectedIndicator.GetType();
			foreach (var kvp in _openValues)
			{
				var pi = SafeGetProperty(type, kvp.Key);
				if (pi != null && pi.CanWrite)
				{
					try { pi.SetValue(selectedIndicator, kvp.Value); } catch { }
				}
			}
			RefreshGrid();
			OnPropertyValueChanged(null, null);
		}

		private void OnCancelClick(object sender, RoutedEventArgs e)
		{
			OnResetAllClick(null, null);
			Close();
		}

		private void RefreshGrid()
		{
			propertyGrid.SelectedObject = null;
			propertyGrid.SelectedObject = selectedIndicator;
			foreach (CategoryItem ci in propertyGrid.Categories)
				ci.IsExpanded = true;
		}

		public void Restore(XDocument document, XElement element)
		{
			if (MainTabControl != null) MainTabControl.RestoreFromXElement(element);
		}

		public void Save(XDocument document, XElement element)
		{
			if (MainTabControl != null) MainTabControl.SaveToXElement(element);
		}

		public WorkspaceOptions WorkspaceOptions { get; set; }
	}

}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private VeritasOrderFlow[] cacheVeritasOrderFlow;
		public VeritasOrderFlow VeritasOrderFlow(int blockSize, int triSize, int nDThreshold, int nDThreshold2, int allTradesMin, double imbalanceOffset, int minimumImbDelta, int minZWidth, int minZWidth2)
		{
			return VeritasOrderFlow(Input, blockSize, triSize, nDThreshold, nDThreshold2, allTradesMin, imbalanceOffset, minimumImbDelta, minZWidth, minZWidth2);
		}

		public VeritasOrderFlow VeritasOrderFlow(ISeries<double> input, int blockSize, int triSize, int nDThreshold, int nDThreshold2, int allTradesMin, double imbalanceOffset, int minimumImbDelta, int minZWidth, int minZWidth2)
		{
			if (cacheVeritasOrderFlow != null)
				for (int idx = 0; idx < cacheVeritasOrderFlow.Length; idx++)
					if (cacheVeritasOrderFlow[idx] != null && cacheVeritasOrderFlow[idx].BlockSize == blockSize && cacheVeritasOrderFlow[idx].TriSize == triSize && cacheVeritasOrderFlow[idx].NDThreshold == nDThreshold && cacheVeritasOrderFlow[idx].NDThreshold2 == nDThreshold2 && cacheVeritasOrderFlow[idx].AllTradesMin == allTradesMin && cacheVeritasOrderFlow[idx].ImbalanceOffset == imbalanceOffset && cacheVeritasOrderFlow[idx].MinimumImbDelta == minimumImbDelta && cacheVeritasOrderFlow[idx].MinZWidth == minZWidth && cacheVeritasOrderFlow[idx].MinZWidth2 == minZWidth2 && cacheVeritasOrderFlow[idx].EqualsInput(input))
						return cacheVeritasOrderFlow[idx];
			return CacheIndicator<VeritasOrderFlow>(new VeritasOrderFlow(){ BlockSize = blockSize, TriSize = triSize, NDThreshold = nDThreshold, NDThreshold2 = nDThreshold2, AllTradesMin = allTradesMin, ImbalanceOffset = imbalanceOffset, MinimumImbDelta = minimumImbDelta, MinZWidth = minZWidth, MinZWidth2 = minZWidth2 }, input, ref cacheVeritasOrderFlow);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.VeritasOrderFlow VeritasOrderFlow(int blockSize, int triSize, int nDThreshold, int nDThreshold2, int allTradesMin, double imbalanceOffset, int minimumImbDelta, int minZWidth, int minZWidth2)
		{
			return indicator.VeritasOrderFlow(Input, blockSize, triSize, nDThreshold, nDThreshold2, allTradesMin, imbalanceOffset, minimumImbDelta, minZWidth, minZWidth2);
		}

		public Indicators.VeritasOrderFlow VeritasOrderFlow(ISeries<double> input , int blockSize, int triSize, int nDThreshold, int nDThreshold2, int allTradesMin, double imbalanceOffset, int minimumImbDelta, int minZWidth, int minZWidth2)
		{
			return indicator.VeritasOrderFlow(input, blockSize, triSize, nDThreshold, nDThreshold2, allTradesMin, imbalanceOffset, minimumImbDelta, minZWidth, minZWidth2);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.VeritasOrderFlow VeritasOrderFlow(int blockSize, int triSize, int nDThreshold, int nDThreshold2, int allTradesMin, double imbalanceOffset, int minimumImbDelta, int minZWidth, int minZWidth2)
		{
			return indicator.VeritasOrderFlow(Input, blockSize, triSize, nDThreshold, nDThreshold2, allTradesMin, imbalanceOffset, minimumImbDelta, minZWidth, minZWidth2);
		}

		public Indicators.VeritasOrderFlow VeritasOrderFlow(ISeries<double> input , int blockSize, int triSize, int nDThreshold, int nDThreshold2, int allTradesMin, double imbalanceOffset, int minimumImbDelta, int minZWidth, int minZWidth2)
		{
			return indicator.VeritasOrderFlow(input, blockSize, triSize, nDThreshold, nDThreshold2, allTradesMin, imbalanceOffset, minimumImbDelta, minZWidth, minZWidth2);
		}
	}
}

#endregion
