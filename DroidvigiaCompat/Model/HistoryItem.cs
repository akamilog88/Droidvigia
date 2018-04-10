using System;

namespace DroidvigiaCompat
{
	public class HistoryItem
	{
		public int Id{ get; set;}
		public DateTime Time{ get; set;}
		public int State{ get; set;}
		public string PartitionName{ get; set;}
		public string Detail{ get; set;}
	}
}

