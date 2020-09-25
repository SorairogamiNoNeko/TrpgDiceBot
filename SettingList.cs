using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace TrpgDiceBot
{
	[Serializable()]
	static class SettingList
	{
		private static string _fileName = "CriFanData.txt";
		public static List<Tuple<int, int>> CriFanList { get; set; } = new List<Tuple<int, int>>();

		/// <summary>
		/// オブジェクトの内容をファイルから読み込み復元する
		/// </summary>
		/// <param name="path">読み込むファイル名</param>
		public static void LoadFromBinaryFile()
		{
			FileStream fs = new FileStream(_fileName, FileMode.Open, FileAccess.Read);
			BinaryFormatter f = new BinaryFormatter();

			// 読み込んでシリアル化する
			CriFanList = (List<Tuple<int, int>>)f.Deserialize(fs);
			fs.Close();
		}

		/// <summary>
		/// オブジェクトの内容をファイルに保存する
		/// </summary>
		public static void SaveToBinaryFile()
		{
			FileStream fs = new FileStream(_fileName, FileMode.Create, FileAccess.Write);
			BinaryFormatter f = new BinaryFormatter();

			// シリアル化して書き込む
			f.Serialize(fs, CriFanList);
			fs.Close();
		}
	}
}
