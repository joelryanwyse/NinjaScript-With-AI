// 
// Copyright (C) 2017, Affordable Indicators <www.affordableindicators.com>.
// Affordable Indicators reserves the right to modify or overwrite this NinjaScript component with each release.
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
using SharpDX.DirectWrite;
using System.Globalization;


//This namespace holds Indicators in this folder and is required. Do not change it. 

namespace NinjaTrader.NinjaScript.Indicators
{
	
    [Gui.CategoryOrder("Parameters", 1)]
	[Gui.CategoryOrder("Multi Time Frame", 2)]

	[Gui.CategoryOrder("Plots", 100)]

   
	
	[Gui.CategoryOrder("Visual", 156)]
	[Gui.CategoryOrder("Data Series", 165)]
	
	
		
	
	
	[Gui.CategoryOrder("Setup", 9000)]
	[Gui.CategoryOrder("License", 10000)]
	
	
	
	
	[TypeConverter("NinjaTrader.NinjaScript.Indicators.aiBestMAsConverter")]		
	public class aiBestMAs : Indicator
	{

		private Series<double> Trend1;

		
		//private bool PlaceOrders = false;
		private double MovingAverageMTFLastBar = 0;
		private double MovingAveragePriceNow = 0;

		private double constant1;
		private double constant2;				
		
		
		
	     private Stroke pLevel1Stroke = new Stroke(Brushes.HotPink, DashStyleHelper.Solid, 1);
//        [Display(ResourceType = typeof(Custom.Resource), Name = "Middle", Description = "", GroupName = "Levels Display", Order = 4)]
//        public Stroke Level1Stroke
//        {
//            get { return pLevel1Stroke; }
//            set { pLevel1Stroke = value; }
//        }		
		
		private Brush pBackBrush1	= Brushes.DodgerBlue;
		
		protected override void OnStateChange()
		{

			if (State == State.SetDefaults)
			{
				
				Description					= @"";
				
                Name						= "aiBestMAs";

				Calculate					= Calculate.OnPriceChange;
				IsOverlay					= true;
				DisplayInDataBox			= true;
				DrawOnPricePanel			= true;
				DrawHorizontalGridLines		= true;
				DrawVerticalGridLines		= true;
				PaintPriceMarkers			= true;
				ScaleJustification			= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				IsAutoScale = false;
				
				IsChartOnly = true;			
				
				//Disable this property if your indicator requires custom values that cumulate with each new market data event. 
				//See Help Guide for additional information.
				IsSuspendedWhileInactive	= false;
				
                ArePlotsConfigurable = false;
                AreLinesConfigurable = false;

				AddPlot(pLevel1Stroke, PlotStyle.Line, "MA");
//				AddPlot(pLevel1Stroke, PlotStyle.Line, "MA 2a");
				
//                AddPlot(pLevel1Stroke, PlotStyle.Line, "MA 1");
//                AddPlot(new Stroke(Brushes.Black, DashStyleHelper.Solid, 1), PlotStyle.Line, "MA 2");

            }
			else if (State == State.Configure)
		    {
				
				if (pMTFEnabled)
				{
					
					string instttttt = Instrument.FullName;
					
					if (pInstrumentString != string.Empty)
						instttttt = pInstrumentString;
					
					
					if (FinalBasePeriodType1 == BarsPeriodType.Renko)
					{
						AddRenko(instttttt, pMTFBarsPeriod1, MarketDataType.Last);
					}
					else
					{
						AddDataSeries(instttttt, FinalBasePeriodType1, pMTFBarsPeriod1);
					}
					
					
					
				}

		    }
			else if (State == State.DataLoaded)
			{ 

				
				constant1 = 2.0 / (1 + pEMA1Fast);
				constant2 = 1 - (2.0 / (1 + pEMA1Fast));				
				
				
//				constant3 = 2.0 / (1 + pEMA1Slow);
//				constant4 = 1 - (2.0 / (1 + pEMA1Slow));				
				
				

//					TriggerCustomEvent(o => {
	
//						ChartControl.Dispatcher.InvokeAsync((Action)(() =>
//					{
						
						
//							if (ChartControl != null)
//							pBackBrush1 = ChartControl.Background;
						
//							//Print(pBackBrush1.ToString());
						
//					}));
				
					
								
			
						
			
//					}, 0, null);
			
			
	
				
				
               // ChartType = Bars.BarsPeriod.BarsPeriodTypeName;

				Trend1 = new Series<double>(this);			

				Plots[0].DashStyleHelper = pDashS1;
//				Plots[1].DashStyleHelper = pDashS1;
//				Plots[2].DashStyleHelper = pDashS1;
//				Plots[3].DashStyleHelper = pDashS1;
				
				//Plots[0].DashStyleHelper = DashStyleHelper.Dot;
				//Plots[1].DashStyleHelper = DashStyleHelper.Dot;
				
				Plots[0].Width = pWidthPlot1;//+2;
//				Plots[1].Width = pWidthPlot1;//+2;
//				Plots[2].Width = pWidthPlot1;
//				Plots[3].Width = pWidthPlot1;				
//				Plots[0].BrushDX.Opacity = (float) pOpacityPlot1/100;
//				Plots[1].BrushDX.Opacity = (float) pOpacityPlot1/100;
				
				
				//Plots[0].DashStyleDX = pDashS1.to
					
			}
			else if (State == State.Terminated)
			{
               
				
				
			}
			else if (State == State.Historical)
			{
				
	
				
			}
			
			
			
			
		}

		
 		private double priorSum = 0;
		private double sum = 0;

	
		

		protected override void OnBarUpdate()
		{
           
			
			
//			Print(pMTFEnabled);
			
			
			for(int i = 0; i <= BarsArray.Length-1; i++) // return for all bars on the chart if there isn't a bar
				if (CurrentBars[i] < 0)
					return;
				
				
            if (BarsInProgress == 0)
            {
				// Dead series removed (BodyHigh/Low, WickHigh/Low, Direction — were set but never read)
            }

			
			

			
			
			
				int BZB = 0;				
				if (CurrentBars[0] == BarsArray[0].Count - 1)
					BZB = 1;			
				
				if (BarsInProgress == 1)
				{
					if (pThisMAType == "HMA")
					{
						//MovingAverageMTFLastBar = EMA(Inputs[1],pEMA1Fast)[BZB];
						
						MovingAverageMTFLastBar = HMA(Inputs[1],pEMA1Fast)[BZB];
						
					}
					else if (pThisMAType == "EMA")
					{
						MovingAverageMTFLastBar = EMA(Inputs[1],pEMA1Fast)[BZB];
						
					}
					else if (pThisMAType == "SMA")
					{
						MovingAverageMTFLastBar = SMA(Inputs[1],pEMA1Fast)[BZB];
						
						if (IsFirstTickOfBar)
							priorSum = sum;

						sum = priorSum + Inputs[1][0] - (CurrentBar >= pEMA1Fast ? Input[pEMA1Fast] : 0);
						MovingAverageMTFLastBar = sum / (CurrentBar < pEMA1Fast ? CurrentBar + 1 : pEMA1Fast);    
						
						MovingAveragePriceNow = MovingAverageMTFLastBar;
						
					}
					//EMAPriceMTF2 = EMA(Inputs[1],pEMA1Slow)[BZB];
					
					return;			
		
				}
				
		 
				//OverlayPlot1[0] = EMAPrice1;
				
				
				
				//Value[0] = (CurrentBar == 0 ? Input[0] : Input[0] * constant1 + constant2 * Value[1]);
				
				
				if (pMTFEnabled)
				{
					
					
					if (pThisMAType == "HMA")
					{
						//MovingAveragePriceNow = Closes[0][0] * constant1 + constant2 * MovingAverageMTFLastBar;
						
						MovingAveragePriceNow = HMA(BarsArray[1], pEMA1Fast)[0];
						
					}				
					else if (pThisMAType == "EMA")
					{
						MovingAveragePriceNow = Closes[0][0] * constant1 + constant2 * MovingAverageMTFLastBar;
						
					}
					else if (pThisMAType == "SMA")
					{
						
						sum = priorSum + Inputs[0][0] - (CurrentBars[1] >= pEMA1Fast ? Inputs[1][pEMA1Fast] : 0);
						MovingAveragePriceNow = sum / (CurrentBars[1] < pEMA1Fast ? CurrentBars[1] + 1 : pEMA1Fast);    				
	
					}
					
					
					
					
					Line1[0] = MovingAveragePriceNow;
					
							
				}
				else
				{
					
						if (pThisMAType == "EMA")
						Line1[0] = EMA(BarsArray[0], pEMA1Fast)[0];
					else if (pThisMAType == "HMA")
						Line1[0] = HMA(BarsArray[0], pEMA1Fast)[0];
					else if (pThisMAType == "SMA")
						Line1[0] = SMA(BarsArray[0], pEMA1Fast)[0];						
					
					
					
				}
				
					
//					if (BarsArray[0].BarsType.IsRemoveLastBarSupported)
//					{
//						if (CurrentBar == 0)
//							Value[0] = Input[0];
//						else
//						{
//							double last = Value[1] * Math.Min(CurrentBar, Period);

//							if (CurrentBar >= Period)
//								Value[0] = (last + Input[0] - Input[Period]) / Math.Min(CurrentBar, Period);
//							else
//								Value[0] = ((last + Input[0]) / (Math.Min(CurrentBar, Period) + 1));
//						}
//					}
//					else
//					{
//						if (IsFirstTickOfBar)
//							priorSum = sum;

//						sum = priorSum + Input[0] - (CurrentBar >= Period ? Input[Period] : 0);
//						Value[0] = sum / (CurrentBar < Period ? CurrentBar + 1 : Period);    
					
//					}
						
					
					
				
			//	EMAPriceNow2 = Closes[0][0] * constant3 + constant4 * EMAPriceMTF2;
				
				//OverlayPlot1[0] = EMAPrice2;
				
				
			
			
			
			
		
//				Line3[0] = Line1[0];
//				Line4[0] = Line2[0];
				
				
				if (CurrentBars[0] == 0)
					return;
				
				
				double compaaa = Line1[1];
				
				if (pMTFEnabled)
					compaaa = MovingAverageMTFLastBar;
				
				if (Line1[0] > compaaa)
				{
					Trend1[0] = 1;
				}
				else if (Line1[0] < compaaa)
				{
					Trend1[0] = -1;
				}
				else
				{
					Trend1[0] = Trend1[1];
				}
								
//				LastSwitchBar1[0] = LastSwitchBar1[1];
		
//				if (Trend1[0] != Trend1[1])
//					LastSwitchBar1[0] = CurrentBars[0];	
				
//				//if (ChartControl != null)
//				{
////					PlotBrushes[0][0] = pBackBrush1;
////					PlotBrushes[1][0] = pBackBrush1;
//				}
				
				if (Trend1[0] == 1)
				{
					PlotBrushes[0][0] = pBrush01;
					//PlotBrushes[0][0] = pColorUpBrush1;
					//PlotBrushes[1][0] = pColorUpBrush1; //pBrush01;
					
//					if (pAreaOpacity1 != 0) 
//						Draw.Region(this,"MAL1"+LastSwitchBar1[0], Math.Min(CurrentBars[0],CurrentBars[0]-LastSwitchBar1[0]+1), 0, Values[1], Values[0], null, pColorUpBrush1, pAreaOpacity1);
				} 
				else
				{
					PlotBrushes[0][0] = pBrush02;
					//PlotBrushes[0][0] = pColorDownBrush1;
					//PlotBrushes[1][0] = pColorDownBrush1; //pBrush02;
					
//					if (pAreaOpacity1 != 0) 
//						Draw.Region(this,"MAS1"+LastSwitchBar1[0], Math.Min(CurrentBars[0],CurrentBars[0]-LastSwitchBar1[0]+1), 0, Values[1], Values[0], null, pColorDownBrush1, pAreaOpacity1);
				}

					

        }



        private double RTTS(double p)
        {
            return Instrument.MasterInstrument.RoundToTickSize(p);
        }


		public override string DisplayName
		{
			get
			{
					if (State == State.SetDefaults)
					{
						
						 
									
						if (pMTFEnabled)
						{			
							//finalname = finalname + " " + BarsArray[1].BarsType.DisplayName;				
							if (pInstrumentString != string.Empty && pInstrumentString != Instrument.FullName)
								return "aiBestMAs " + "(" + pEMA1Fast.ToString() + " " + pThisMAType + ", " + pMTFBarsPeriod1.ToString() + " " + pMTFBasePeriodType1 + ", " + pInstrumentString + ")";
							else
								return "aiBestMAs " + "(" + pEMA1Fast.ToString() + " " + pThisMAType + ", " + pMTFBarsPeriod1.ToString() + " " + pMTFBasePeriodType1 + ")";
						}
							
				
						return "aiBestMAs " + "(" + pEMA1Fast.ToString() + " " + pThisMAType + ")";
						
					}
					else
						return "";
			}
		
		}	

		public override string FormatPriceMarker(double value)
		{
			
			return AllPriceMarker(value);

		}
		
		private string AllPriceMarker (double price)
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
		
				
		
		
		[Browsable(false)]
		[XmlIgnore()]
		public Series<double> Line1 {get { return Values[0]; }}
		
//		[Browsable(false)]
//		[XmlIgnore()]
//		public Series<double> Line2 {get { return Values[1]; }}
		
		
		
//		[Browsable(false)]
//		[XmlIgnore()]
//		public Series<double> Line3 {get { return Values[2]; }}
		
//		[Browsable(false)]
//		[XmlIgnore()]
//		public Series<double> Line4 {get { return Values[3]; }}
		
		
		
		
		
		
  		private bool pMTFEnabled = false;
       [NinjaScriptProperty]
			[RefreshProperties(RefreshProperties.All)] // Update UI when value is changed
		[Display(ResourceType = typeof(Custom.Resource), Name = "Enabled", GroupName = "Multi Time Frame", Order = 0)]
        public bool MTFEnabled
        {
            get { return pMTFEnabled; }
            set { pMTFEnabled = value; }
        }
		
		
		private string pInstrumentString = "";
		[Description(""), NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Instrument", GroupName = "Multi Time Frame", Description = "",  Order = 2)]
		public string InstrumentString
		{
			get { return pInstrumentString; }
			set { pInstrumentString = value; }
		}
		
		
		private BarsPeriodType FinalBasePeriodType1 = BarsPeriodType.Minute;		
		private string pMTFBasePeriodType1 = "Minute";
		[NinjaScriptProperty]
		[Description("")]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Bars Type", Description = "", GroupName = "Multi Time Frame", Order = 4)]
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
        [Display(ResourceType = typeof(Custom.Resource), Name = "Bars Period", GroupName = "Multi Time Frame", Order = 5)]
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
				return new StandardValuesCollection( new String[] {"EMA", "SMA"} );
			}
		}	
		
		
		
		
		
        private int pEMA1Fast = 20;
        [Range(1, int.MaxValue), NinjaScriptProperty]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Period", GroupName = "Parameters", Order = 2)]
        public int EMA1Fast
        {
            get { return pEMA1Fast; }
            set { pEMA1Fast = value; }
        }
		
		
//        private int pEMA1Slow = 20;
//        [Range(1, int.MaxValue), NinjaScriptProperty]
//        [Display(ResourceType = typeof(Custom.Resource), Name = "Slow Period", GroupName = "Parameters", Order = 3)]
//        public int EMA1Slow
//        {
//            get { return pEMA1Slow; }
//            set { pEMA1Slow = value; }
//        }
		
		
     

		
		// BUY COLOR
		
		private System.Windows.Media.Brush	pBrush01 = Brushes.DimGray;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Up Color", Description = "", GroupName = "Plots", Order = 1)]
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

		private int	pOpacity01 = 80;
		[Range(0, 100)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Up Opacity (%)", Description = "", GroupName = "Plots", Order = 2)]
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
		
		private System.Windows.Media.Brush	pBrush02 = Brushes.DimGray;
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Down Color", Description = "", GroupName = "Plots", Order = 3)]
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

		private int	pOpacity02 = 80;
		[Range(0, 100)]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Down Opacity (%)", Description = "", GroupName = "Plots", Order = 4)]
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
		
		
		
		
		
 
//		private Brush pColorUpBrush1	= Brushes.DodgerBlue;
//		[XmlIgnore]
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Color Up", GroupName = "Plots", Order = 4)]
//		public Brush ColorUpBrush1
//		{
//			get { return pColorUpBrush1; } set { pColorUpBrush1 = value; }
//		}
//		[Browsable(false)]
//		public string ColorUpBrush1S
//		{
//			get { return Serialize.BrushToString(pColorUpBrush1); } set { pColorUpBrush1 = Serialize.StringToBrush(value); }
//		}			
		
//		private Brush pColorDownBrush1	= Brushes.Red;
//		[XmlIgnore]
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Color Down", GroupName = "Plots", Order = 5)]
//		public Brush ColorDownBrush1
//		{
//			get { return pColorDownBrush1; } set { pColorDownBrush1 = value; }
//		}
//		[Browsable(false)]
//		public string ColorDownBrush1S
//		{
//			get { return Serialize.BrushToString(pColorDownBrush1); } set { pColorDownBrush1 = Serialize.StringToBrush(value); }
//		}	
		
//		private int pOpacityPlot1 = 100;
//		[Display(ResourceType = typeof(Custom.Resource), Name = "Opacity (%)", GroupName = "Plots", Order = 6)]
//		[Range(0, 100)]
//		public int OpacityPlot1
//		{
//			get { return pOpacityPlot1; }
//			set { pOpacityPlot1 = value; }
//		}		
			
				
		private int pWidthPlot1 = 1;
		[Display(ResourceType = typeof(Custom.Resource), Name = "Width", GroupName = "Plots", Order = 7)]
		[Range(1, int.MaxValue)]
		public int WidthPlot1
		{
			get { return pWidthPlot1; }
			set { pWidthPlot1 = value; }
		}	
		
	
		
		

        private DashStyleHelper pDashS1  = DashStyleHelper.Solid;
        [Display(ResourceType = typeof(Custom.Resource), Name = "Dash style", GroupName = "Plots", Order = 8)]
		public DashStyleHelper DashS1
		{
			get { return pDashS1; }
			set { pDashS1 = value; }
		}	
       		

 
		


		
		private int pAreaOpacity1 = 20;
//        [Range(0, 100)]
//        [Display(ResourceType = typeof(Custom.Resource), Name = "Area Opacity (%)", GroupName = "Plots", Order = 10)]
//        public int AreaOpacity1
//        {
//            get { return pAreaOpacity1; }
//            set { pAreaOpacity1 = value; }
//        }
		     


		
		
		
		
	}
	
	
	
	
			// Hide UserDefinedValues properties when not in use by the HLCCalculationMode.UserDefinedValues
	// When creating a custom type converter for indicators it must inherit from NinjaTrader.NinjaScript.IndicatorBaseConverter to work correctly with indicators
	public class aiBestMAsConverter : NinjaTrader.NinjaScript.IndicatorBaseConverter
	{
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) { return true; }

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			PropertyDescriptorCollection propertyDescriptorCollection = base.GetPropertiesSupported(context) ? base.GetProperties(context, value, attributes) : TypeDescriptor.GetProperties(value, attributes);

			aiBestMAs   jbb = (aiBestMAs) value;
			
			//Pivots						thisPivotsInstance			= (Pivots) value;
			
			//bool MagnetsOn = ;
			
			List<string> DeleteThese = new List<string>();
			List<string> DeleteThese2 = new List<string>();
				
	
			//DeleteThese.Add("Calculate");
			DeleteThese.Add("Name");
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
				
				DeleteThese.Add("InstrumentString");	
				DeleteThese.Add("MTFBasePeriodType1");	
				DeleteThese.Add("MTFBarsPeriod1");
	
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

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private aiBestMAs[] cacheaiBestMAs;
		public aiBestMAs aiBestMAs(bool mTFEnabled, string instrumentString, string mTFBasePeriodType1, int mTFBarsPeriod1, string thisMAType, int eMA1Fast)
		{
			return aiBestMAs(Input, mTFEnabled, instrumentString, mTFBasePeriodType1, mTFBarsPeriod1, thisMAType, eMA1Fast);
		}

		public aiBestMAs aiBestMAs(ISeries<double> input, bool mTFEnabled, string instrumentString, string mTFBasePeriodType1, int mTFBarsPeriod1, string thisMAType, int eMA1Fast)
		{
			if (cacheaiBestMAs != null)
				for (int idx = 0; idx < cacheaiBestMAs.Length; idx++)
					if (cacheaiBestMAs[idx] != null && cacheaiBestMAs[idx].MTFEnabled == mTFEnabled && cacheaiBestMAs[idx].InstrumentString == instrumentString && cacheaiBestMAs[idx].MTFBasePeriodType1 == mTFBasePeriodType1 && cacheaiBestMAs[idx].MTFBarsPeriod1 == mTFBarsPeriod1 && cacheaiBestMAs[idx].ThisMAType == thisMAType && cacheaiBestMAs[idx].EMA1Fast == eMA1Fast && cacheaiBestMAs[idx].EqualsInput(input))
						return cacheaiBestMAs[idx];
			return CacheIndicator<aiBestMAs>(new aiBestMAs(){ MTFEnabled = mTFEnabled, InstrumentString = instrumentString, MTFBasePeriodType1 = mTFBasePeriodType1, MTFBarsPeriod1 = mTFBarsPeriod1, ThisMAType = thisMAType, EMA1Fast = eMA1Fast }, input, ref cacheaiBestMAs);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.aiBestMAs aiBestMAs(bool mTFEnabled, string instrumentString, string mTFBasePeriodType1, int mTFBarsPeriod1, string thisMAType, int eMA1Fast)
		{
			return indicator.aiBestMAs(Input, mTFEnabled, instrumentString, mTFBasePeriodType1, mTFBarsPeriod1, thisMAType, eMA1Fast);
		}

		public Indicators.aiBestMAs aiBestMAs(ISeries<double> input , bool mTFEnabled, string instrumentString, string mTFBasePeriodType1, int mTFBarsPeriod1, string thisMAType, int eMA1Fast)
		{
			return indicator.aiBestMAs(input, mTFEnabled, instrumentString, mTFBasePeriodType1, mTFBarsPeriod1, thisMAType, eMA1Fast);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.aiBestMAs aiBestMAs(bool mTFEnabled, string instrumentString, string mTFBasePeriodType1, int mTFBarsPeriod1, string thisMAType, int eMA1Fast)
		{
			return indicator.aiBestMAs(Input, mTFEnabled, instrumentString, mTFBasePeriodType1, mTFBarsPeriod1, thisMAType, eMA1Fast);
		}

		public Indicators.aiBestMAs aiBestMAs(ISeries<double> input , bool mTFEnabled, string instrumentString, string mTFBasePeriodType1, int mTFBarsPeriod1, string thisMAType, int eMA1Fast)
		{
			return indicator.aiBestMAs(input, mTFEnabled, instrumentString, mTFBasePeriodType1, mTFBarsPeriod1, thisMAType, eMA1Fast);
		}
	}
}

#endregion
