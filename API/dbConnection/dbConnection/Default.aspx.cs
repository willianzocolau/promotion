/*
 * Created by SharpDevelop.
 * User: Vó Maegima
 * Date: 26/08/2018
 * Time: 17:33
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using Npgsql;

namespace dbConnection
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public class Default : Page
	{	
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		#region Data

		protected	HtmlInputButton		_Button_Ok;
		protected	HtmlInputButton		_SQL_Button_Ok;
		protected	HtmlInputText 		_Input_Name;

		#endregion
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		#region Page Init & Exit (Open/Close DB connections here...)
		
		protected void PageInit(object sender, EventArgs e)
		{
			string connectionString = null;
            NpgsqlConnection cnn ;
            NpgsqlCommand db_cmd;
			connectionString = "Host=178.128.186.9;Port=5432;Username=promotion_admin;Password=promotion;Database=promotion;";
			Response.Write(connectionString);
			try
			{
				Response.Write("OK<br>");
				cnn = new NpgsqlConnection(connectionString);
				Response.Write("OK<br>");
				try
	            {
					cnn.Open();
	                Response.Write ("Connection Open ! <br>");
	                db_cmd = new NpgsqlCommand();
	                db_cmd.Connection = cnn;
	                using (var cmd = new NpgsqlCommand("SELECT * FROM teste", cnn))
    				using (var reader = cmd.ExecuteReader())
	                		while (reader.Read()){
	                			Response.Write(reader.GetDataTypeName(0) + " " + reader.GetInt32(0) + "<br>");
	                			Response.Write(reader.GetDataTypeName(1) + " " + reader.GetString(1) + "<br>");
	                			Response.Write(reader.GetDataTypeName(2) + " " + reader.GetString(2) + "<br>");
	                		}
	                cnn.Close();
	            } 
	            catch (Exception ex)
	            {
	                Response.Write("Can not open connection ! <br>");
	                Response.Write(ex);
	            }				
			}
			catch (Exception ex)
	        {
	        	Response.Write("ERRO! <br>");
	            Response.Write(ex);
	        }
		}
		//----------------------------------------------------------------------
		protected void PageExit(object sender, EventArgs e)
		{
		}
		#endregion
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		#region Page Load
		private void Page_Load(object sender, EventArgs e)
		{
			Response.Write(@"OI<br>");
			//------------------------------------------------------------------
			if(IsPostBack)
			{
			}
			//------------------------------------------------------------------
		}
		#endregion
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		#region Click_Button_OK

		//----------------------------------------------------------------------
		protected void Click_Button_Ok(object sender, EventArgs e)
		{
			Response.Write( _Button_Ok.Value + " botao! ae<br>");
		}

		#endregion
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		#region Changed_Input_Name

		//----------------------------------------------------------------------
		protected void Changed_Input_Name(object sender, EventArgs e)
		{
			Response.Write( _Input_Name.Value + " mudou!<br>");
		}

		#endregion
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		#region More...		
		#endregion
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		#region Initialize Component

		protected override void OnInit(EventArgs e)
		{
			InitializeComponent();
			base.OnInit(e);
		}
		//----------------------------------------------------------------------
		private void InitializeComponent()
		{
			this.Load	+= new System.EventHandler(Page_Load);
			this.Init   += new System.EventHandler(PageInit);
			this.Unload += new System.EventHandler(PageExit);
			
			_Button_Ok.ServerClick	 += new EventHandler(Click_Button_Ok);
			_Input_Name.ServerChange += new EventHandler(Changed_Input_Name);
		}
		#endregion
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
	}
}