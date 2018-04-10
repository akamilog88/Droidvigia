using System;

namespace DroidvigiaCompat
{		
	public class ActionRegisteredEventArgs : EventArgs
	{
		public HistoryItem NewData{ get; private set;}
		public ActionRegisteredEventArgs(HistoryItem new_action){
			NewData=new_action;
		}
	}
}

