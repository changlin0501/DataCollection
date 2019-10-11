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
		double temperature1 = 0;
		string GalvanicCurrent = null;
		//string MachineAirPressure = null;

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


		//温度量程(-50-+100)
		double k = 9.375;
		double b = -87.5;

		//压力量程（0-1Mpa）
		double a = 16;
		double c = -64;

		//压力量程（0-30Mpa）
		double f = 1.875;
		double g = -7.5;

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
			string topic = txtPubTopic.Text.Trim();

			if (string.IsNullOrEmpty(topic))
			{
				MessageBox.Show("发布主题不能为空！");
				return;
			}

			string inputString = txtSendMessage.Text.Trim();
			var appMsg = new MqttApplicationMessage(topic, Encoding.UTF8.GetBytes(inputString), MqttQualityOfServiceLevel.AtMostOnce, false);
			mqttClient.PublishAsync(appMsg);
		}


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
				Common.LogHandler.WriteLog("温度串口获取失败");
			}

			portconfigure();

			Pressure();


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

				Voltage();
			}
			catch (Exception ex)
			{
				Common.LogHandler.WriteLog("电压串口获取失败");
			}
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
				Common.LogHandler.WriteLog("系统初始化失败", ex);
				throw;
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
				Common.LogHandler.WriteLog("系统初始化失败", ex);
				throw;
			}
		}


		/// <summary>
		/// 温度值
		/// </summary>
		private void portconfigure()
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

					//电流值
					decimal ElectricCurrent_d = Math.Round(((decimal)wd_d3 / 1000), 2);
					temperature1 = k * (double)ElectricCurrent_d + b;
					Console.WriteLine(temperature1);
				}

				else
				{
					MessageBox.Show("温度读取失败：" + read_1.ToMessageShowString());
				}
			}

			catch (Exception ex)
			{
				//MessageBox.Show("读取失败：" + ex.Message);
				Common.LogHandler.WriteLog("温度地址读取失败");
			}

		}

		/// <summary>
		/// 压力值
		/// </summary>
		private void Pressure()
		{
			try
			{
				//压力地址
				OperateResult<byte[]> read_2 = busRtuClient.ReadBase(HslCommunication.Serial.SoftCRC16.CRC16(HslCommunication.BasicFramework.SoftBasic.HexStringToBytes("02 03 00 02 00 04")));
				if (read_2.IsSuccess)
				{
					string Voltage = HslCommunication.BasicFramework.SoftBasic.ByteToHexString(read_2.Content, ' ');

					//截取指定字符串
					string[] sArray2 = Voltage.Split(' ');
					string Pressure_str1 = sArray2[3];
					string Pressure_str2 = sArray2[4];
					string Pressure_str3 = sArray2[5];
					string Pressure_str4 = sArray2[6];
					string Pressure_str5 = sArray2[6];
					string Pressure_str6 = sArray2[6];
					string Pressure_str7 = sArray2[6];
					string Pressure_str8 = sArray2[6];

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
					
					decimal ElectricCurrent3 = Math.Round(((decimal)Pressure_d3 / 1000), 2);
					double Pr03 = f * (double)ElectricCurrent3 + g;
					Console.WriteLine(Pr03);
				}
				else
				{
					MessageBox.Show("压力地址读取失败：" + read_2.ToMessageShowString());
				}
			}
			catch (Exception ex)
			{
				Common.LogHandler.WriteLog("压力地址读取失败", ex);
			}
		}

		/// <summary>
		/// 电压值
		/// </summary>
		private void Voltage()
		{
			try
			{
				//电压地址
				OperateResult<byte[]> read_3 = busRtuClient.ReadBase(HslCommunication.Serial.SoftCRC16.CRC16(HslCommunication.BasicFramework.SoftBasic.HexStringToBytes("01 03 01 6e 00 02")));
				if (read_3.IsSuccess)
				{
					string Voltage = HslCommunication.BasicFramework.SoftBasic.ByteToHexString(read_3.Content, ' ');

					//截取指定字符串
					string[] sArray2 = Voltage.Split(' ');
					string Voltage_str1 = sArray2[3];
					string Voltage_str2 = sArray2[4];
					string Voltage_str3 = sArray2[5];
					string Voltage_str4 = sArray2[6];
					string Voltage_str5 = sArray2[6];
					string Voltage_str6 = sArray2[6];
					string Voltage_str7 = sArray2[6];
					string Voltage_str8 = sArray2[6];

					//4个地址的指标
					string Voltage_h1 = Voltage_str1 + Voltage_str2;
					string Voltage_h2 = Voltage_str3 + Voltage_str4;
					string Voltage_h3 = Voltage_str5 + Voltage_str6;
					string Voltage_h4 = Voltage_str7 + Voltage_str8;

					//16进制转10进制
					double Voltage_d1 = Convert.ToInt32(Voltage_h3, 16);
					decimal Voltage_d2 = Math.Round(((decimal)Voltage_d1 / 10000), 2);
					Console.WriteLine(Voltage_d2);
				}
				else
				{
					MessageBox.Show("电压地址读取失败：" + read_3.ToMessageShowString());
				}
			}
			catch (Exception ex)
			{
				Common.LogHandler.WriteLog("电压地址读取失败", ex);
			}

		}
	}

}
