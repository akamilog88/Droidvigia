using System;
using System.Collections.Generic;

namespace DroidvigiaCompat
{
	public class Partition
	{
		public int Id{ get; set;}
		public string Name{ get; set;}
		public String Code{ get; set;}
		public bool Activated{ get; set;}
		private DateTime time=new DateTime();
		public DateTime ChangeStateTime { get{return time;} 
			set{
				if(time!=value){
					time = value;
				}
			}}

		public List<Zone> Zones{ get; set;}
	}
}

