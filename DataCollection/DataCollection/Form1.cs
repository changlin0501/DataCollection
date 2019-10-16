using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HslCommunication.Profinet;
using HslCommunication;
using HslCommunication.ModBus;
using System.Threading;
using System.IO.Ports;

using MQTTnet;
using MQTTnet.Core;
using MQTTnet.Core.Client;
using MQTTnet.Core.Packets;
using MQTTnet.Core.Protocol;
using System.Threading.Tasks;

namespace DataCollection
{
	public partial class Form1 : Form
	{
		private MqttClient mqttClient = null;
		//温度4个地址
		double Temperature01 = 0;
		double Temperature02 = 0;
		double Temperature03 = 0;
		double Temperature04 = 0;
		double Temperature03_03 = 0;
		double Temperature03_04 = 0;


		//压力4个地址
		//double Pressure01 = 0;
		//double Pressure02 = 0;
		//double Pressure03 = 0;
		//double Pressure04 = 0;

		//电压2个地址
		decimal Voltage01 = 0;
		decimal Voltage02 = 0;
		decimal Voltage04 = 0;



		public Form1()
		{
			InitializeComponent();

			Task.Run(async () => { await ConnectMqttServerAsync(); });
		}



		//实例化ModbusRtu
		private ModbusRtu busRtuClient = new ModbusRtu();
		private ModbusRtu Modbus_dianya = new ModbusRtu();

		//发送串口数据间隔时间
		private long sendDataTimeInterval = 0;


		//温度量程(-50-+100)   测试
		double k = 9.375;
		double b = -87.5;

		//温度量程(0-+300)
		double h = 18.75;
		double j = -75;

		//温度量程(0-+1000)
		double y = 62.5;
		double z = -250;

		//压力量程（0-1Mpa）
		double a = 0.0625;
		double c = -0.25;

		//压力量程（0-30Mpa）
		double f = 1.875;
		double g = -7.5;

		private void Form1_Load(object sender, EventArgs e)
		{
			try
			{
				//温度压力串口配置
				busRtuClient.SerialPortInni(sp =>
				{
					port4();

					#region 温度串口配置

					//sp.PortName = "COM6";
					//sp.BaudRate = 9600;
					//sp.DataBits = 8;
					//sp.StopBits = System.IO.Ports.StopBits.One;
					//sp.Parity = System.IO.Ports.Parity.None;
					#endregion
				});
				//打开串口
				busRtuClient.Open();
			}
			catch (Exception ex)
			{
				Common.LogHandler.WriteLog("温度压力串口获取失败");
			}

			try
			{
				TemperatureValue();
			}
			catch (Exception ex)
			{
				Common.LogHandler.WriteLog("温度值获取失败");

				throw;
			}

			try
			{
				PressureValue();
			}
			catch (Exception)
			{
				Common.LogHandler.WriteLog("压力值获取失败");

				throw;
			}




			//电压端口
			try
			{
				Modbus_dianya.SerialPortInni(sp =>
				{
					port6();


					#region 电压串口配置
					//sp.PortName = "COM6";
					//sp.BaudRate = 2400;
					//sp.DataBits = 8;
					//sp.StopBits = System.IO.Ports.StopBits.One;
					//sp.Parity = System.IO.Ports.Parity.Even;
					#endregion
				});
				Modbus_dianya.Open();


				try
				{
					VoltageValue();
				}
				catch (Exception)
				{
					Common.LogHandler.WriteLog("电压值获取失败");
				}

			}
			catch (Exception ex)
			{
				Common.LogHandler.WriteLog("电压串口获取失败");
			}
		}


		/// <summary>
		/// MQTT连接服务器
		/// </summary>
		private async Task ConnectMqttServerAsync()
		{
			if (mqttClient == null)
			{
				mqttClient = new MqttClientFactory().CreateMqttClient() as MqttClient;
				//mqttClient.ApplicationMessageReceived += MqttClient_ApplicationMessageReceived;
				mqttClient.Connected += MqttClient_Connected;
				mqttClient.Disconnected += MqttClient_Disconnected;

			}

			try
			{
				var options = new MqttClientTcpOptions
				{

					Server = "172.17.130.127",
					ClientId = Guid.NewGuid().ToString().Substring(0, 5),
					UserName = "OM1m8XFs9rkjc8Dd69DI",
					Password = "",
					CleanSession = true
				};

				await mqttClient.ConnectAsync(options);

			}
			catch (Exception ex)
			{
				Invoke((new Action(() =>
				{
					Common.LogHandler.WriteLog($"连接到MQTT服务器失败！" + Environment.NewLine + ex.Message + Environment.NewLine);
				})));
			}
		}

		/// <summary>
		/// 服务器连接成功
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MqttClient_Connected(object sender, EventArgs e)
		{
			Invoke((new Action(() =>
			{

				Common.LogHandler.WriteLog("已连接到MQTT服务器！" + Environment.NewLine);
			})));
		}

		/// <summary>
		/// 断开服务器连接
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MqttClient_Disconnected(object sender, EventArgs e)
		{
			Invoke((new Action(() =>
			{
				Common.LogHandler.WriteLog("已断开MQTT连接！" + Environment.NewLine);
			})));
		}

		/// <summary>
		/// 发布主题
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BtnPublish_Click(object sender, EventArgs e)
		{
			//string topic = txtPubTopic.Text.Trim();

			//if (string.IsNullOrEmpty(topic))
			//{
			//	MessageBox.Show("发布主题不能为空！");
			//	return;
			//}

			//string inputString = txtSendMessage.Text.Trim();
			//var appMsg = new MqttApplicationMessage(topic, Encoding.UTF8.GetBytes(inputString), MqttQualityOfServiceLevel.AtMostOnce, false);
			//mqttClient.PublishAsync(appMsg);
		}


		/// <summary>
		/// 温度压力端口
		/// </summary>
		private void port4()
		{
			try
			{
				busRtuClient.SerialPortInni(sp =>
				{
					//端口
					sp.PortName = Common.ConfigFileHandler.GetAppConfig("TemperaturePressureSerialPortName");

					//波特率
					int defaultBaudRate = 0;
					int.TryParse(Common.ConfigFileHandler.GetAppConfig("TemperaturePressureSerialBaudRate"), out defaultBaudRate);
					sp.BaudRate = defaultBaudRate;

					//数据位
					int defaultDataBits = 0;
					int.TryParse(Common.ConfigFileHandler.GetAppConfig("TemperaturePressureSerialDataBits"), out defaultDataBits);
					sp.DataBits = defaultDataBits;

					//奇偶性验证
					string defaultParity = Common.ConfigFileHandler.GetAppConfig("TemperaturePressureSerialParity");
					if (defaultParity.ToUpper() == System.IO.Ports.Parity.None.ToString().ToUpper())
					{
						sp.Parity = System.IO.Ports.Parity.None;
					}
					else if (defaultParity.ToUpper() == System.IO.Ports.Parity.Odd.ToString().ToUpper())
					{
						sp.Parity = System.IO.Ports.Parity.Odd;
					}
					else if (defaultParity.ToUpper() == System.IO.Ports.Parity.Even.ToString().ToUpper())
					{
						sp.Parity = System.IO.Ports.Parity.Even;
					}
					else if (defaultParity.ToUpper() == System.IO.Ports.Parity.Mark.ToString().ToUpper())
					{
						sp.Parity = System.IO.Ports.Parity.Mark;
					}
					else if (defaultParity.ToUpper() == System.IO.Ports.Parity.Space.ToString().ToUpper())
					{
						sp.Parity = System.IO.Ports.Parity.Space;
					}

					//停止位
					string defaultStopBits = Common.ConfigFileHandler.GetAppConfig("TemperaturePressureSerialStopBits");
					if (defaultStopBits.ToUpper() == System.IO.Ports.StopBits.None.ToString().ToUpper())
					{
						sp.StopBits = System.IO.Ports.StopBits.None;
					}
					else if (defaultStopBits.ToUpper() == System.IO.Ports.StopBits.One.ToString().ToUpper())
					{
						sp.StopBits = System.IO.Ports.StopBits.One;
					}
					else if (defaultStopBits.ToUpper() == System.IO.Ports.StopBits.OnePointFive.ToString().ToUpper())
					{
						sp.StopBits = System.IO.Ports.StopBits.OnePointFive;
					}
					else if (defaultStopBits.ToUpper() == System.IO.Ports.StopBits.Two.ToString().ToUpper())
					{
						sp.StopBits = System.IO.Ports.StopBits.Two;
					}

				});
			}

			catch (Exception ex)
			{
				Common.LogHandler.WriteLog("温度压力端口获取失败");
			}
		}

		/// <summary>
		/// 电压端口
		/// </summary>
		private void port6()
		{
			try
			{
				Modbus_dianya.SerialPortInni(sp =>
				{
					//端口
					sp.PortName = Common.ConfigFileHandler.GetAppConfig("VoltageSerialPortName");

					//波特率
					int defaultBaudRate = 0;
					int.TryParse(Common.ConfigFileHandler.GetAppConfig("VoltageSerialBaudRate"), out defaultBaudRate);
					sp.BaudRate = defaultBaudRate;

					//数据位
					int defaultDataBits = 0;
					int.TryParse(Common.ConfigFileHandler.GetAppConfig("VoltageSerialDataBits"), out defaultDataBits);
					sp.DataBits = defaultDataBits;

					//奇偶性验证
					string defaultParity = Common.ConfigFileHandler.GetAppConfig("VoltageSerialParity");
					if (defaultParity.ToUpper() == System.IO.Ports.Parity.None.ToString().ToUpper())
					{
						sp.Parity = System.IO.Ports.Parity.None;
					}
					else if (defaultParity.ToUpper() == System.IO.Ports.Parity.Odd.ToString().ToUpper())
					{
						sp.Parity = System.IO.Ports.Parity.Odd;
					}
					else if (defaultParity.ToUpper() == System.IO.Ports.Parity.Even.ToString().ToUpper())
					{
						sp.Parity = System.IO.Ports.Parity.Even;
					}
					else if (defaultParity.ToUpper() == System.IO.Ports.Parity.Mark.ToString().ToUpper())
					{
						sp.Parity = System.IO.Ports.Parity.Mark;
					}
					else if (defaultParity.ToUpper() == System.IO.Ports.Parity.Space.ToString().ToUpper())
					{
						sp.Parity = System.IO.Ports.Parity.Space;
					}

					//停止位
					string defaultStopBits = Common.ConfigFileHandler.GetAppConfig("VoltageSerialStopBits");
					if (defaultStopBits.ToUpper() == System.IO.Ports.StopBits.None.ToString().ToUpper())
					{
						sp.StopBits = System.IO.Ports.StopBits.None;
					}
					else if (defaultStopBits.ToUpper() == System.IO.Ports.StopBits.One.ToString().ToUpper())
					{
						sp.StopBits = System.IO.Ports.StopBits.One;
					}
					else if (defaultStopBits.ToUpper() == System.IO.Ports.StopBits.OnePointFive.ToString().ToUpper())
					{
						sp.StopBits = System.IO.Ports.StopBits.OnePointFive;
					}
					else if (defaultStopBits.ToUpper() == System.IO.Ports.StopBits.Two.ToString().ToUpper())
					{
						sp.StopBits = System.IO.Ports.StopBits.Two;
					}

				});
			}

			catch (Exception ex)
			{
				Common.LogHandler.WriteLog("电压端口获取失败", ex);
			}
		}


		/// <summary>
		/// 温度值
		/// </summary>
		private void TemperatureValue()
		{
			try
			{
				//获取4个温度地址
				OperateResult<byte[]> read_1 = busRtuClient.ReadBase(HslCommunication.Serial.SoftCRC16.CRC16(HslCommunication.BasicFramework.SoftBasic.HexStringToBytes("02 03 00 02 00 04")));
				if (read_1.IsSuccess)
				{
					string wendu = HslCommunication.BasicFramework.SoftBasic.ByteToHexString(read_1.Content, ' ');
					//截取指定字符串
					string[] sArray = wendu.Split(' ');
					string wd_str1 = sArray[3];
					string wd_str2 = sArray[4];
					string wd_str3 = sArray[5];
					string wd_str4 = sArray[6];
					string wd_str5 = sArray[7];
					string wd_str6 = sArray[8];
					string wd_str7 = sArray[9];
					string wd_str8 = sArray[10];

					//4个温度地址16进制
					string wd_h1 = wd_str1 + wd_str2;
					string wd_h2 = wd_str3 + wd_str4;
					string wd_h3 = wd_str5 + wd_str6;
					string wd_h4 = wd_str7 + wd_str8;

					//16进制转10进制
					double wd_d1 = Convert.ToInt32(wd_h1, 16);
					double wd_d2 = Convert.ToInt32(wd_h2, 16);
					double wd_d3 = Convert.ToInt32(wd_h3, 16);
					double wd_d4 = Convert.ToInt32(wd_h4, 16);


					#region -50-+100温度测试
					//decimal ElectricCurrent_d = Math.Round(((decimal)wd_d3 / 1000), 2);
					//Temperature = k * (double)ElectricCurrent_d + b;
					//Console.WriteLine(Temperature);
					#endregion

					//温度(0-300)1路
					decimal ElectricCurrent_d1 = Math.Round(((decimal)wd_d1 / 1000), 2);
					Temperature01 = h * (double)ElectricCurrent_d1 + j;
					double Temperature_01 = (double)((ElectricCurrent_d1 - 4) * 300) / 16;
					Console.WriteLine(Temperature01);
					Console.WriteLine(Temperature_01);

					//温度(0-300)2路
					decimal ElectricCurrent_d2 = Math.Round(((decimal)wd_d2 / 1000), 2);
					Temperature02 = h * (double)ElectricCurrent_d2 + j;
					double Temperature_02 = (double)((ElectricCurrent_d2 - 4) * 300) / 16;
					Console.WriteLine(Temperature02);
					Console.WriteLine(Temperature_02);

					//温度(0-1000)3路
					decimal ElectricCurrent_d3 = Math.Round(((decimal)wd_d3 / 1000), 2);
					Temperature03 = y * (double)ElectricCurrent_d3 + z;
					double Temperature_03 = (double)((ElectricCurrent_d3 - 4) * 300) / 16;
					Console.WriteLine(Temperature03);
					Console.WriteLine(Temperature_03);


					//-50-100测试
					Temperature03_03 = k * (double)ElectricCurrent_d3 + b;
					Temperature03_04 = (double)((ElectricCurrent_d3 - 4) * 150) / 16 - 50;
					//测试
					Console.WriteLine(Temperature03_03);
					Console.WriteLine(Temperature03_04);
					txt_wendu.Text = Temperature03_04.ToString();


					//温度(0-1000)4路
					decimal ElectricCurrent_d4 = Math.Round(((decimal)wd_d4 / 1000), 2);
					Temperature04 = y * (double)ElectricCurrent_d4 + z;
					double Temperature_04 = (double)((ElectricCurrent_d4 - 4) * 300) / 16;
					Console.WriteLine(Temperature04);
					Console.WriteLine(Temperature_04);
				}

				else
				{
					MessageBox.Show("温度读取失败：" + read_1.ToMessageShowString());
				}
			}

			catch (Exception ex)
			{
				Common.LogHandler.WriteLog("温度地址读取失败");
			}

		}

		/// <summary>
		/// 压力值
		/// </summary>
		private void PressureValue()
		{
			try
			{
				//压力地址
				OperateResult<byte[]> read_2 = busRtuClient.ReadBase(HslCommunication.Serial.SoftCRC16.CRC16(HslCommunication.BasicFramework.SoftBasic.HexStringToBytes("03 03 00 02 00 04")));
				if (read_2.IsSuccess)
				{
					string Voltage = HslCommunication.BasicFramework.SoftBasic.ByteToHexString(read_2.Content, ' ');

					//截取指定字符串
					string[] sArray2 = Voltage.Split(' ');
					string Pressure_str1 = sArray2[3];
					string Pressure_str2 = sArray2[4];
					string Pressure_str3 = sArray2[5];
					string Pressure_str4 = sArray2[6];
					string Pressure_str5 = sArray2[7];
					string Pressure_str6 = sArray2[8];
					string Pressure_str7 = sArray2[9];
					string Pressure_str8 = sArray2[10];

					//4个地址的指标
					string Pressure_h1 = Pressure_str1 + Pressure_str2;
					string Pressure_h2 = Pressure_str3 + Pressure_str4;
					string Pressure_h3 = Pressure_str5 + Pressure_str6;
					string Pressure_h4 = Pressure_str7 + Pressure_str8;

					//16进制转10进制
					double Pressure_d1 = Convert.ToInt32(Pressure_h1, 16);
					double Pressure_d2 = Convert.ToInt32(Pressure_h2, 16);
					double Pressure_d3 = Convert.ToInt32(Pressure_h3, 16);
					double Pressure_d4 = Convert.ToInt32(Pressure_h4, 16);

					//量程（0-1Mpa）1路
					decimal PressureElectricCurrent_d1 = Math.Round(((decimal)Pressure_d1 / 1000), 2);
					double Pressure01 = a * (double)PressureElectricCurrent_d1 + c;
					decimal Pressure_01 = (PressureElectricCurrent_d1 - 4) / 16;
					Console.WriteLine(Pressure01);
					Console.WriteLine(Pressure_01);
					text_yali.Text = Pressure_01.ToString();

					//量程（0-1Mpa）2路
					decimal PressureElectricCurrent_d2 = Math.Round(((decimal)Pressure_d2 / 1000), 2);
					double Pressure02 = a * (double)PressureElectricCurrent_d2 + c;
					double Pressure_02 = (double)(PressureElectricCurrent_d2 - 4) / 16;
					Console.WriteLine(Pressure02);
					Console.WriteLine(Pressure_02);


					//量程（0-30Mpa）3路
					decimal PressureElectricCurrent_d3 = Math.Round(((decimal)Pressure_d3 / 1000), 2);
					double Pressure03 = f * (double)PressureElectricCurrent_d3 + g;
					decimal Pressure_03 = (PressureElectricCurrent_d3 - 4) * 30 / 16;
					Console.WriteLine(Pressure03);
					Console.WriteLine(Pressure_03);
					text_yaliB.Text = Pressure_03.ToString();




					//量程（0-30Mpa）4路
					decimal PressureElectricCurrent_d4 = Math.Round(((decimal)Pressure_d3 / 1000), 2);
					double Pressure04 = f * (double)PressureElectricCurrent_d4 + g;
					decimal Pressure_04 = (PressureElectricCurrent_d4 - 4) * 30 / 16;
					Console.WriteLine(Pressure04);
					Console.WriteLine(Pressure_04);
				}
				else
				{
					MessageBox.Show("压力地址读取失败：" + read_2.ToMessageShowString());
				}
			}
			catch (Exception ex)
			{
				Common.LogHandler.WriteLog("压力地址读取失败");
			}
		}

		/// <summary>
		/// 电压值
		/// </summary>
		private void VoltageValue()
		{
			try
			{
				//电压地址
				OperateResult<byte[]> read_3 = Modbus_dianya.ReadBase(HslCommunication.Serial.SoftCRC16.CRC16(HslCommunication.BasicFramework.SoftBasic.HexStringToBytes("01 03 01 6e 00 08")));
				if (read_3.IsSuccess)
				{
					string Voltage = HslCommunication.BasicFramework.SoftBasic.ByteToHexString(read_3.Content, ' ');

					//截取指定字符串
					string[] sArray3 = Voltage.Split(' ');
					string Voltage_str1 = sArray3[3];
					string Voltage_str2 = sArray3[4];
					string Voltage_str3 = sArray3[5];
					string Voltage_str4 = sArray3[6];
					string Voltage_str5 = sArray3[7];
					string Voltage_str6 = sArray3[8];
					string Voltage_str7 = sArray3[9];
					string Voltage_str8 = sArray3[10];


					string Voltage_str9 = sArray3[11];
					string Voltage_str10 = sArray3[12];
					string Voltage_str11 = sArray3[13];
					string Voltage_str12 = sArray3[14];
					string Voltage_str13 = sArray3[15];
					string Voltage_str14 = sArray3[16];
					string Voltage_str15 = sArray3[17];
					string Voltage_str16 = sArray3[18];


					//8个地址的指标
					string Voltage_h1 = Voltage_str1 + Voltage_str2;
					string Voltage_h2 = Voltage_str3 + Voltage_str4;
					string Voltage_h3 = Voltage_str5 + Voltage_str6;
					string Voltage_h4 = Voltage_str7 + Voltage_str8;
					string Voltage_h5 = Voltage_str9 + Voltage_str10;
					string Voltage_h6 = Voltage_str11 + Voltage_str12;
					string Voltage_h7 = Voltage_str13 + Voltage_str14;
					string Voltage_h8 = Voltage_str15 + Voltage_str16;




					//高位字和低位字
					string Voltage_dh1 = Voltage_h1 + Voltage_h2;//A相电压
					string Voltage_dh2 = Voltage_h3 + Voltage_h4;//B相电压
					string Voltage_dh3 = Voltage_h5 + Voltage_h6;//C相电压
					string Voltage_dh4 = Voltage_h7 + Voltage_h8;//A相电流



					//16进制转10进制
					double Voltage_d1 = Convert.ToInt32(Voltage_dh1, 16);
					double Voltage_d2 = Convert.ToInt32(Voltage_dh2, 16);
					double Voltage_d3 = Convert.ToInt32(Voltage_dh3, 16);
					double Voltage_d4 = Convert.ToInt32(Voltage_dh4, 16);



					//电压值（1路）A项电压
					Voltage01 = Math.Round(((decimal)Voltage_d1 / 10000), 2);
					Console.WriteLine(Voltage01);
					text_dianya.Text = Voltage01.ToString();
					//电压值（2路）B项电压
					Voltage02 = Math.Round(((decimal)Voltage_d2 / 10000), 2);
					Console.WriteLine(Voltage02);


					//电流值
					Voltage04 = Math.Round(((decimal)Voltage_d4 / 10000), 2);
					Console.WriteLine(Voltage04);
					text_dianliu.Text = Voltage04.ToString();

				}
				else
				{
					MessageBox.Show("电压地址读取失败：" + read_3.ToMessageShowString());
				}
			}
			catch (Exception ex)
			{
				Common.LogHandler.WriteLog("电压地址读取失败");
			}
		}
	}

}
