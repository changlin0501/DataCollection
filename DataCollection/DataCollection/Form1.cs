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

namespace DataCollection
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
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


		private void Form1_Load(object sender, EventArgs e)
		{

			try
			{
				//串口配置
				busRtuClient.SerialPortInni(sp =>
				{
					sp.PortName = "COM9";
					sp.BaudRate = 9600;
					sp.DataBits = 8;
					sp.StopBits = System.IO.Ports.StopBits.One;
					sp.Parity = System.IO.Ports.Parity.None;
				});
				//打开串口
				busRtuClient.Open();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			try
			{
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




					decimal ElectricCurrent_d = Math.Round(((decimal)wd_d1 / 1000), 2);
					double temperature_result = k * (double)ElectricCurrent_d + b;
					Console.WriteLine(temperature_result);
				}

				else
				{
					MessageBox.Show("温度读取失败：" + read_1.ToMessageShowString());
				}
			}


			catch (Exception ex)
			{
				MessageBox.Show("读取失败：" + ex.Message);
			}

			//压力
			OperateResult<byte[]> read_2 = busRtuClient.ReadBase(HslCommunication.Serial.SoftCRC16.CRC16(HslCommunication.BasicFramework.SoftBasic.HexStringToBytes("02 03 00 02 00 04")));

			if (read_2.IsSuccess)
			{
				string Pressure = HslCommunication.BasicFramework.SoftBasic.ByteToHexString(read_2.Content, ' ');
				//截取指定字符串
				string[] sArray1 = Pressure.Split(' ');
				string Pressure_str1 = sArray1[3];
				string Pressure_str2 = sArray1[4];
				string Pressure_str3 = sArray1[5];
				string Pressure_str4 = sArray1[6];
				string Pressure_str5 = sArray1[7];
				string Pressure_str6 = sArray1[8];
				string Pressure_str7 = sArray1[9];
				string Pressure_str8 = sArray1[10];

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


				//1路
				decimal ElectricCurrent1 = Math.Round(((decimal)Pressure_d1 / 1000), 2);
				double Pr01 = a * (double)ElectricCurrent1 + c;
				Console.WriteLine(Pr01);


				//3路
				decimal ElectricCurrent3 = Math.Round(((decimal)Pressure_d3 / 1000), 2);
				double Pr03 = f * (double)ElectricCurrent3 + g;
				Console.WriteLine(Pr03);
			}
			else
			{
				MessageBox.Show("读取失败：" + read_2.ToMessageShowString());
			}


			try
			{

				Modbus_dianya.SerialPortInni(sp =>
				{
					sp.PortName = "COM6";
					sp.BaudRate = 2400;
					sp.DataBits = 8;
					sp.StopBits = System.IO.Ports.StopBits.One;
					sp.Parity = System.IO.Ports.Parity.Even;
				});
				busRtuClient.Open();
			}
			catch (Exception ex)
			{

				MessageBox.Show(ex.Message);
			}
			try
			{
				//电压
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
					MessageBox.Show("压力读取失败：" + read_3.ToMessageShowString());
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

		}

	}
}
