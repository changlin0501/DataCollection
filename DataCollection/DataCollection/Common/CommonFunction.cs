using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollection.Common
{
	public static class CommonFunction
	{
		public static string byteToHexStrH(byte[] bytes, int len)  //数组转十六进制字符显示
		{
			string returnStr = "";
			if (bytes != null)
			{
				for (int i = 0; i < len; i++)
				{
					returnStr += bytes[i].ToString("X2");
					returnStr += ' ';
				}
			}
			return returnStr;
		}
	}
}
