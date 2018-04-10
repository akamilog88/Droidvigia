using System;

namespace DroidvigiaCompat
{
	public class Action
	{
		public const int ACTION_ID_ACTIVATED=1;
		public const int ACTION_ID_DESACTIVATED=2;
		public const int ACTION_ID_FIRED=3;
		public const int ACTION_ID_INCOMING_CALL = 4;
	
		public static string GetStateName(int id){ 
			string name = "";
			switch (id) {
			case ACTION_ID_ACTIVATED:
				name="Activada";
				break;
			case ACTION_ID_DESACTIVATED:
				name="Desactivada";
				break;
			case ACTION_ID_FIRED:
				name="Alarma";
				break;
			case ACTION_ID_INCOMING_CALL:
				name="Llamada";
				break;
			}
			return name;			
		}
	}
}

