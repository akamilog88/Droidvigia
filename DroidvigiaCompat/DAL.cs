using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Data;
using Microsoft.Data.Sqlite;


namespace DroidvigiaCompat
{
	public class DAL
	{
		string[]  init_commands = new string[]{
			"CREATE TABLE Zone ( Id INTEGER PRIMARY KEY AUTOINCREMENT, Code TEXT UNIQUE NOT NULL, Name STRING UNIQUE NOT NULL, Fired BOOLEAN NOT NULL,Inmediate BOOLEAN NOT NULL,PartitionId INT REFERENCES Partition (Id) ON DELETE CASCADE ON UPDATE CASCADE)"
			,"CREATE TABLE Partition ( Id INTEGER PRIMARY KEY AUTOINCREMENT, Code TEXT UNIQUE NOT NULL, Name STRING NOT NULL UNIQUE, Activated BOOLEAN NOT NULL,ChangeStateTime DATETIME)"
			,"CREATE TABLE C_Action ( Id INTEGER PRIMARY KEY AUTOINCREMENT, Name STRING UNIQUE NOT NULL)"
			,"CREATE TABLE Schudle (Hour INTEGER NOT NULL,Minute INTEGER NOT NULL, ActionId INT NOT NULL, PartitionNames STRING NOT NULL, Days STRING NOT NULL,Label STRING NOT NULL UNIQUE,Enabled BOOLEAN NOT NULL)"
			,"CREATE TABLE History (Id INTEGER PRIMARY KEY AUTOINCREMENT, Time DATETIME NOT NULL, State INT NOT NULL, PartitionName STRING NOT NULL, Detail TEXT)"
			,"CREATE INDEX TIME_INDEX ON History (Time DESC)"
		};

		const string DB_Name="AlarmaStore.db3";
		private SqliteConnection connection;
		private bool exists=false;

		public DAL(){
			InitDb ();
		}

		void PopulateDefaults(){
			Partition master = new Partition {Name="master",Code="0000",Activated=false,};
			master.Id= NewPartition (master);
			List<Zone> zones = new List<Zone> {
				new Zone { Name="Zona-1",Code="1",PartitionId = master.Id,Fired=false,Inmediate=true},
				new Zone { Name="Zona-2",Code="2",PartitionId = master.Id,Fired=false,Inmediate=true},
				new Zone { Name="Zona-3",Code="3",PartitionId = master.Id,Fired=false,Inmediate=true},
				new Zone { Name="Zona-4",Code="4",PartitionId = master.Id,Fired=false,Inmediate=true},
				new Zone { Name="Zona-5",Code="5",PartitionId = master.Id,Fired=false,Inmediate=true},
				new Zone { Name="Zona-6",Code="6",PartitionId = master.Id,Fired=false,Inmediate=true},
				new Zone { Name="Zona-7",Code="7",PartitionId = master.Id,Fired=false,Inmediate=true},
				new Zone { Name="Zona-8",Code="8",PartitionId = master.Id,Fired=false,Inmediate=true},
				new Zone { Name="Zona-9",Code="9",PartitionId = master.Id,Fired=false,Inmediate=true},
				new Zone { Name="Zona-0",Code="0",PartitionId = master.Id,Fired=false,Inmediate=true},
				new Zone { Name="Zona-#",Code="#",PartitionId = master.Id,Fired=false,Inmediate=true},
			};

			zones.ForEach ((z)=>{NewZone(z,master.Id);});
		}
		void InitDb(){	
			try{
				string dbPath = Path.Combine (
					System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal),
					DB_Name);
				bool exists = File.Exists (dbPath);
				if (!exists){
					//SqliteConnection.CreateFile (dbPath);
                    
					if(connection==null)
                        connection = new SqliteConnection("Data Source=" + dbPath);
                    //connection = new SqliteConnection(dbPath);

                    connection.Open ();	
					foreach(string cmd in init_commands){
						using (var c = connection.CreateCommand ()) {
							c.CommandText = cmd;
							c.CommandType = CommandType.Text;
							c.ExecuteNonQuery ();
						}
					}
					connection.Close();
					exists = true;
					PopulateDefaults();
				}else{
                    connection = new SqliteConnection ("Data Source=" + dbPath);
                    //connection = new SqliteConnection(dbPath);
                    connection.Open ();	
				}
			}catch(Exception e){

            }
			finally{
				if(connection!=null && connection.State!= ConnectionState.Closed)
					connection.Close ();
			}
		}

		public int NewZone(Zone zone,int PartitionId){
			int res = -1;
			try{
				connection.Open();
				using (var c = connection.CreateCommand ()) {
					c.CommandText = "Insert into Zone(Code,Name,PartitionId,Fired,Inmediate) values(@Code,@Name,@PartitionId,@Fired,@Inmediate)" ;
					c.CommandType = CommandType.Text;
					c.Parameters.AddWithValue("@Code",zone.Code);
					c.Parameters.AddWithValue("@Name",zone.Name);
					c.Parameters.AddWithValue("@PartitionId",PartitionId);
					c.Parameters.AddWithValue("@Fired",zone.Fired);
					c.Parameters.AddWithValue("@Inmediate",zone.Inmediate);
					res=c.ExecuteNonQuery ();
				}
			}
			catch(Exception){}
			finally{
				connection.Close ();
			}
			return res;
		}
		public int NewSchudle(Schudle s){
			int res = -1;
			try{
				connection.Open();
				using (var c = connection.CreateCommand ()) {
					c.CommandText = "Insert into Schudle(Label,Hour,Minute,Days,PartitionNames,ActionId,Enabled) values(@Label,@Hour,@Minute,@Days,@PartitionNames,@ActionId,@Enabled)" ;
					c.CommandType = CommandType.Text;				
					c.Parameters.AddWithValue("@Label",s.Label);
					c.Parameters.AddWithValue("@PartitionNames",s.PartitionNames);
					c.Parameters.AddWithValue("@Hour",s.Hour);
					//c.Parameters.AddWithValue("@Minute",s.Minute);
					var param = new SqliteParameter();
					param.DbType = DbType.Int16;
					param.ParameterName="@Minute";
					param.Value= s.Minute;
					c.Parameters.Add(param);
					c.Parameters.AddWithValue("@Days",s.Days);
					c.Parameters.AddWithValue("@Enabled",s.Enabled);
					c.Parameters.AddWithValue("@ActionId",s.ActionId);
					res=c.ExecuteNonQuery ();
				}
			}
			catch(Exception){}
			finally{
				connection.Close ();
			}
			return res;
		}
		public int NewPartition(Partition partition){
			int res = -1;
			try{
				connection.Open();
				using (var c = connection.CreateCommand ()) {
					c.CommandText = "INSERT INTO Partition(Name,Code,Activated,ChangeStateTime) VALUES (@Name,@Code,@Activated,@ChangeStateTime)" ;
					c.CommandType = CommandType.Text;
					var p1 =new SqliteParameter();
                    p1.DbType = DbType.String;
					p1.ParameterName="@Code";
					p1.Value=partition.Code.ToString();
					c.Parameters.Add(p1);
					c.Parameters.Add(new SqliteParameter("@Name",partition.Name));
					c.Parameters.Add(new SqliteParameter("@Activated",partition.Activated));
					var p2 =new SqliteParameter();
                    p2.DbType = DbType.DateTime;
					p2.ParameterName="@ChangeStateTime";
					p2.Value=partition.ChangeStateTime;
					c.Parameters.Add(p2);
					res= c.ExecuteNonQuery ();
				}
			}
			catch(Exception){}
			finally{
				connection.Close ();
			}
			return res;
		}
		public int RegiterEvent(HistoryItem item){
			int res = -1;
			try{
				connection.Open();
				using (var c = connection.CreateCommand ()) {
					c.CommandText = "INSERT INTO History(Time,State,PartitionName,Detail) VALUES (@Time,@State,@PartitionName,@Detail)" ;
					c.CommandType = CommandType.Text;				
					var p =new SqliteParameter();
                    p.DbType = DbType.DateTime;
					p.ParameterName="@Time";
					p.Value=item.Time;
					c.Parameters.Add(p);
					c.Parameters.Add(new SqliteParameter("@State",item.State));
					c.Parameters.Add(new SqliteParameter("@PartitionName",item.PartitionName));
					c.Parameters.Add(new SqliteParameter("@Detail",item.Detail));
					res= c.ExecuteNonQuery ();
				}
			}
			catch(Exception){}
			finally{
				connection.Close ();
			}
			return res;
		}
		public List<HistoryItem> GetTop10Events(){
			List<HistoryItem> result = new List<HistoryItem> ();

			try{
				connection.Open();
				using (var c = connection.CreateCommand ()) {
					c.CommandText = "SELECT * FROM History ORDER BY Id DESC LIMIT 10" ;
					c.CommandType = CommandType.Text;				
					SqliteDataReader reader = c.ExecuteReader ();
					while(reader.Read()){
						var i= MapHistoryItemFromReader(reader);
						result.Add(i);
					}
				}
			}
			catch(Exception){}
			finally{
				connection.Close ();
			}
			return result;
		}
		public int ActivateAllPartitions(){
			int res = -1;
			try{
				connection.Open();
				using (var c = connection.CreateCommand ()) {
					c.CommandText = "UPDATE Partition set Activated= 1,ChangeStateTime=@ChangeStateTime" ;
					var p = new SqliteParameter();
                    p.DbType = DbType.DateTime;
					p.ParameterName="@ChangeStateTime";
					p.Value=DateTime.Now;
					c.Parameters.Add(p);
					c.CommandType = CommandType.Text;
					res= c.ExecuteNonQuery ();
				}
			}
			catch(Exception){}
			finally{
				connection.Close ();
			}
			return res;
		}
		public int DesactivateAllPartitions(){
			int res = -1;
			try{
				connection.Open();
				using (var c = connection.CreateCommand ()) {
					c.CommandText = "UPDATE Partition set Activated= 0,ChangeStateTime=@ChangeStateTime" ;
					var p = new SqliteParameter();
                    p.DbType = DbType.DateTime;
					p.ParameterName="@ChangeStateTime";
					p.Value=DateTime.Now;
					c.Parameters.Add(p);
					c.CommandType = CommandType.Text;
					res= c.ExecuteNonQuery ();
				}
				using(var c2= connection.CreateCommand()){
					c2.CommandText = "UPDATE Zone set Fired=0" ;
					c2.CommandType = CommandType.Text;
					res= c2.ExecuteNonQuery ();
				}
			}
			catch(Exception){}
			finally{
				connection.Close ();
			}
			return res;
		}
		public Partition GetPartitionByName(String nombre){
			Partition p =null;
			try{
				connection.Open();
				using (var c = connection.CreateCommand ()) {
					c.CommandText = "SELECT * FROM Partition WHERE Name=@name" ;
					c.CommandType = CommandType.Text;
					var p1 =new SqliteParameter();
                    p1.DbType = DbType.String;
					p1.ParameterName="@name";
					p1.Value=nombre;
					c.Parameters.Add(p1);
					SqliteDataReader reader = c.ExecuteReader ();

					while(reader.Read()){
					p	= MapPartitionFromReader(reader);
					}
				}
			}
			catch(Exception){}
			finally{
				connection.Close ();
			}
			return p;
		}
	
		public Zone GetZoneByName(String nombre){
			Zone z =null;
			try{
				connection.Open();
				using (var c = connection.CreateCommand ()) {
					c.CommandText = "SELECT * FROM Zone WHERE Name=@name" ;
					c.CommandType = CommandType.Text;
					var p1 =new SqliteParameter();
                    p1.DbType = DbType.String;
					p1.ParameterName="@name";
					p1.Value=nombre;
					c.Parameters.Add(p1);
					SqliteDataReader reader = c.ExecuteReader ();

					while(reader.Read()){
						z	= MapZoneFromReader(reader);
					}
				}
			}
			catch(Exception){}
			finally{
				connection.Close ();
			}
			return z;
		}
		public Zone GetZoneByCode(String code){
			Zone z =null;
			try{
				connection.Open();
				using (var c = connection.CreateCommand ()) {
					c.CommandText = "SELECT * FROM Zone WHERE Code=@Code" ;
					c.CommandType = CommandType.Text;
					var p1 =new SqliteParameter();
                    p1.DbType = DbType.String;
					p1.ParameterName="@Code";
					p1.Value=code;
					c.Parameters.Add(p1);
					SqliteDataReader reader = c.ExecuteReader ();

					while(reader.Read()){
						z	= MapZoneFromReader(reader);
					}
				}
			}
			catch(Exception){}
			finally{
				connection.Close ();
			}
			return z;
		}
	
		public void ChangePartitionName(int Id,string name){
			try{
				connection.Open();
				using (var c = connection.CreateCommand ()) {
					c.CommandText = "UPDATE Partition set Name=@Name WHERE Id=@Id";
					c.CommandType = CommandType.Text;
					c.Parameters.AddWithValue("@Id",Id);
					c.Parameters.AddWithValue("@Name",name);
					c.ExecuteNonQuery();
				}
			}
			catch(Exception){}
			finally{
				connection.Close ();
			}
		}
		public void EditZone(int Id,Zone z){
			try{
				connection.Open();
				using (var c = connection.CreateCommand ()) {
					c.CommandText = "UPDATE Zone set Name=@Name,Inmediate=@Inmediate WHERE Id=@Id";
					c.CommandType = CommandType.Text;
					c.Parameters.AddWithValue("@Id",Id);
					c.Parameters.AddWithValue("@Name",z.Name);
					c.Parameters.AddWithValue("@Inmediate",z.Inmediate);
					c.ExecuteNonQuery();
				}
			}
			catch(Exception){}
			finally{
				connection.Close ();
			}
		}
		public List<Partition> GetAllPartitions(){
			List<Partition> result  = new List<Partition>();
			try{
				connection.Open();
				using (var c = connection.CreateCommand ()) {
					c.CommandText = "Select * from Partition" ;
					c.CommandType =CommandType.Text;
					SqliteDataReader reader = c.ExecuteReader ();

					while(reader.Read()){
						Partition p = MapPartitionFromReader(reader);					
						result.Add(p);
					}
				}
			}
			catch(Exception e){
				;
			}
			finally{
				connection.Close ();
			}
			return result;
		}
		public List<Schudle> GetAllSchudles(){
			List<Schudle> result  = new List<Schudle>();
			try{
				connection.Open();
				using (var c = connection.CreateCommand ()) {
					c.CommandText = "Select * from Schudle" ;
					c.CommandType =CommandType.Text;
					SqliteDataReader reader = c.ExecuteReader ();

					while(reader.Read()){
						Schudle s = MapSchudleFromReader(reader);					
						result.Add(s);
					}
				}
			}
			catch(Exception e){
				;
			}
			finally{
				connection.Close ();
			}
			return result;
		}
		public Schudle GetSchudleByName(string name){
			Schudle s=null;
			try{
				connection.Open();
				using (var c = connection.CreateCommand ()) {
					c.CommandText = "Select * from Schudle where Label=@Label" ;
					c.Parameters.AddWithValue("@Label",name);
					c.CommandType =CommandType.Text;
					SqliteDataReader reader = c.ExecuteReader ();
					while(reader.Read()){
						s = MapSchudleFromReader(reader);
					}
				}
			}
			catch(Exception e){
				;
			}
			finally{
				connection.Close ();
			}
			return s;
		}
		public void DeletePartition(int PartitionId){
			try{
				connection.Open();				
				using (var c = connection.CreateCommand ()) {
					c.CommandText ="PRAGMA foreign_keys=ON;";
					c.ExecuteNonQuery();
					c.CommandText = "Delete from Partition where Id=@PartitionId";
					c.CommandType =CommandType.Text;
					c.Parameters.AddWithValue("@PartitionId",PartitionId);
					c.ExecuteNonQuery();
				}
			}
			catch(Exception){}
			finally{
				connection.Close ();
			}
		}
		public void DeleteSchudle(string Label){
			try{
				connection.Open();
				using (var c = connection.CreateCommand ()) {
					c.CommandText = "Delete from Schudle where Label=@Label";
					c.CommandType =CommandType.Text;
					c.Parameters.AddWithValue("@Label",Label);
					c.ExecuteNonQuery();
				}
			}
			catch(Exception){}
			finally{
				connection.Close ();
			}
		}
		public void UpdateZoneState(int ZoneId,bool fired){
			try{
				connection.Open();
				using (var c = connection.CreateCommand ()) {
					c.CommandText = "UPDATE Zone set Fired=@Fired WHERE Id=@ZoneId";
					c.CommandType =CommandType.Text;
					c.Parameters.AddWithValue("@ZoneId",ZoneId);
					c.Parameters.AddWithValue("@Fired",fired);
					c.ExecuteNonQuery();
				}
			}
			catch(Exception){}
			finally{
				connection.Close ();
			}
		}
		public void UpdatePartitionState(int PartitionId,bool activated){
			try{
				connection.Open();
				DateTime changetime =  DateTime.Now;
				using (var c = connection.CreateCommand ()) {
					c.CommandText = "UPDATE Partition set Activated=@Activated,ChangeStateTime=@ChangeStateTime WHERE Id=@PartitionId";
					c.CommandType =CommandType.Text;
					c.Parameters.AddWithValue("@PartitionId",PartitionId);
					c.Parameters.AddWithValue("@Activated",activated);
					c.Parameters.AddWithValue("@ChangeStateTime",changetime);
					c.ExecuteNonQuery();
				}
				if(!activated){
					using (var c2 = connection.CreateCommand ()) {
						c2.CommandText = "UPDATE Zone set Fired=0 WHERE PartitionId=@PartitionId";
						c2.Parameters.AddWithValue("@PartitionId",PartitionId);
						c2.CommandType =CommandType.Text;
						c2.ExecuteNonQuery();
					}
				}
			}
			catch(Exception){}
			finally{
				connection.Close ();
			}
		}
		public void UpdateSchudleState(string label,bool enabled){
			try{
				connection.Open();
				DateTime changetime =  DateTime.Now;
				using (var c = connection.CreateCommand ()) {
					c.CommandText = "UPDATE Schudle set Enabled=@Enabled WHERE Label=@Label";
					c.CommandType =CommandType.Text;
					c.Parameters.AddWithValue("@Label",label);
					c.Parameters.AddWithValue("@Enabled",enabled);
					c.ExecuteNonQuery();
				}
			}
			catch(Exception){}
			finally{
				connection.Close ();
			}
		}

		public void DeleteZone(int ZoneId){
			try{
				connection.Open();
				using (var c = connection.CreateCommand ()) {
					c.CommandText = "Delete from Zone where Id=@ZoneId";
					c.CommandType =CommandType.Text;
					c.Parameters.AddWithValue("@ZoneId",ZoneId);
					c.ExecuteNonQuery();
				}
			}
			catch(Exception){}
			finally{
				connection.Close ();
			}
		}

        public void ClearHistory()
        {
            try
            {
                connection.Open();
                using (var c = connection.CreateCommand())
                {
                    c.CommandText = "Delete from History";
                    c.CommandType = CommandType.Text;                   
                    c.ExecuteNonQuery();
                }
            }
            catch (Exception) { }
            finally
            {
                connection.Close();
            }
        }
        public List<Zone> GetPartitionsZones(int PartitionId){
			List<Zone> result = new List<Zone>();
			try{
				connection.Open();
				using (var c = connection.CreateCommand ()) {
					c.CommandText = "Select * from Zone where PartitionId=@PartitionId" ;
					c.CommandType =CommandType.Text;
					c.Parameters.AddWithValue("@PartitionId",PartitionId);
					SqliteDataReader reader = c.ExecuteReader ();

					while(reader.Read()){
						result.Add(MapZoneFromReader(reader));
					}
				}
			}
			catch(Exception){}
			finally{
				connection.Close ();
			}
			return result;
		}	

		Partition MapPartitionFromReader(IDataReader reader,bool use_Subfix=false){
			string s_id=(use_Subfix?"p_Id":"Id");
			string s_Code=(use_Subfix?"p_Code":"Code");
			string s_Name=(use_Subfix?"p_Name":"Name");
			string s_Activated=(use_Subfix?"p_Activated":"Activated");
			string s_ChangeStateTime=(use_Subfix?"p_ChangeStateTime":"ChangeStateTime");

			Partition p= new Partition ();
			p.Id = Convert.ToInt16( reader [s_id]);
			p.Code = Convert.ToString(reader[s_Code]);
			p.Name = reader [s_Name].ToString();
			p.Activated = Convert.ToBoolean( reader [s_Activated]);
			p.ChangeStateTime = Convert.ToDateTime (reader [s_ChangeStateTime]);
			return p;
		}
		Schudle MapSchudleFromReader(IDataReader reader,bool use_Subfix=false){
			string s_Label=(use_Subfix?"s_Label":"Label");
			string s_Hour=(use_Subfix?"s_Hour":"Hour");
			string s_Minute=(use_Subfix?"s_Minute":"Minute");
			string s_Days=(use_Subfix?"s_Days":"Days");
			string s_PartitionNames=(use_Subfix?"s_PartitionNames":"PartitionNames");
			string s_Enabled=(use_Subfix?"p_Enabled":"Enabled");
			string s_ActionId=(use_Subfix?"p_ActionId":"ActionId");


			Schudle s= new Schudle ();
			s.Label = reader [s_Label].ToString();
			s.PartitionNames = Convert.ToString(reader [s_PartitionNames]);
			s.Days = Convert.ToString(reader [s_Days]);
			s.Enabled = Convert.ToBoolean( reader [s_Enabled]);
			s.Hour = Convert.ToInt32(reader [s_Hour]);
			s.Minute = Convert.ToInt32(reader [s_Minute]);
			s.ActionId = Convert.ToInt32(reader [s_ActionId]);
			return s;
		}
		Zone MapZoneFromReader(IDataReader reader,bool use_Subfix=false){
			string s_id=(use_Subfix?"z_Id":"Id");
			string s_Code=(use_Subfix?"z_Code":"Code");
			string s_Name=(use_Subfix?"z_Name":"Name");
			string s_PId=(use_Subfix?"z_PartitionId":"PartitionId");
			string s_Fired=(use_Subfix?"z_Fired":"Fired");
			string s_Inmediate=(use_Subfix?"z_Inmediate":"Inmediate");

			Zone z= new Zone ();
			z.Id = Convert.ToInt16( reader [s_id]);
			z.Code =  Convert.ToString(reader[s_Code]);
			z.Name = reader [s_Name].ToString();
			z.PartitionId = Convert.ToInt16( reader[s_PId]);
			z.Fired = Convert.ToBoolean( reader[s_Fired]);
			z.Inmediate =  Convert.ToBoolean( reader[s_Inmediate]);
			return z;
		}
		HistoryItem MapHistoryItemFromReader(IDataReader reader){
			var item = new HistoryItem ();
			item.PartitionName = reader ["PartitionName"].ToString();
			item.State = Convert.ToInt16(reader ["State"]);
			item.Time= Convert.ToDateTime (reader ["Time"]);
			item.Detail= reader ["Detail"].ToString();
			return item;
		}
	}
}

