using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADOSpeedTest.DAC;

namespace ADOSpeedTest
{
	public class AdoPerfromanceTests
	{
		private int _departmentKey;

		public AdoPerfromanceTests()
		{
			File.Delete(@"c:\ado_speed_tests.txt");
		}

		public void InitializeData()
		{
			using (var db = new AdoDatabaseContext(@"Server=DECAIREPC;Database=sampledata;Trusted_Connection=True;"))
			{
				// delete any records from previous run
				db.Execute("DELETE FROM Person");

				db.Execute("DELETE FROM Department");

				// insert one department
				db.Add("@name", "Operations", SqlDbType.VarChar);

				db.Execute("INSERT INTO Department (name) VALUES (@name)");

				// select the primary key of the department table for the only record that exists
				using (var result = db.ReadQuery("SELECT d.id FROM Department d where d.name='Operations'"))
				{
					while (result.Read())
					{
						_departmentKey = int.Parse(result.GetFieldNullOrString("id"));
					}
				}
			}
		}

		public void RunAllTests()
		{
			double smallest = -1;
			for (int i = 0; i < 5; i++)
			{
				InitializeData();

				double result = TestInsert();

				if (smallest < 0)
				{
					smallest = result;
				}
				else
				{
					if (result < smallest)
					{
						smallest = result;
					}
				}
			}
			WriteLine("INSERT:" + smallest);
			Console.WriteLine("INSERT:" + smallest);

			smallest = -1;
			for (int i = 0; i < 5; i++)
			{
				InitializeData();
				TestInsert();

				double result = TestUpdate();

				if (smallest < 0)
				{
					smallest = result;
				}
				else
				{
					if (result < smallest)
					{
						smallest = result;
					}
				}
			}
			WriteLine("UPDATE:" + smallest);
			Console.WriteLine("UPDATE:" + smallest);

			smallest = -1;
			for (int i = 0; i < 5; i++)
			{
				InitializeData();
				TestInsert();

				double result = TestSelect();

				if (smallest < 0)
				{
					smallest = result;
				}
				else
				{
					if (result < smallest)
					{
						smallest = result;
					}
				}
			}
			WriteLine("SELECT:" + smallest);
			Console.WriteLine("SELECT:" + smallest);

			smallest = -1;
			for (int i = 0; i < 5; i++)
			{
				InitializeData();
				TestInsert();

				double result = TestDelete();

				if (smallest < 0)
				{
					smallest = result;
				}
				else
				{
					if (result < smallest)
					{
						smallest = result;
					}
				}
			}
			WriteLine("DELETE:" + smallest);
			Console.WriteLine("DELETE:" + smallest);

			WriteLine("");
		}

		public double TestInsert()
		{
			using (var db = new AdoDatabaseContext(@"Server=DECAIREPC;Database=sampledata;Trusted_Connection=True;"))
			{
				// read first and last names
				var firstnames = new List<string>();
				using (var sr = new StreamReader(@"..\..\Data\firstnames.txt"))
				{
					string line;
					while ((line = sr.ReadLine()) != null)
						firstnames.Add(line);
				}

				var lastnames = new List<string>();
				using (var sr = new StreamReader(@"..\..\Data\lastnames.txt"))
				{
					string line;
					while ((line = sr.ReadLine()) != null)
						lastnames.Add(line);
				}

				//test inserting 10000 records (only ~1,000 names in text)
				var startTime = DateTime.Now;
				for (int j = 0; j < 10; j++)
				{
					for (int i = 0; i < 1000; i++)
					{
						db.Add("@first", firstnames[i], SqlDbType.VarChar);
						db.Add("@last", lastnames[i], SqlDbType.VarChar);
						db.Add("@department", _departmentKey, SqlDbType.Int);

						db.Execute("INSERT INTO Person (first,last,department) VALUES (@first,@last,@department)");
					}
				}
				var elapsedTime = DateTime.Now - startTime;

				return elapsedTime.TotalSeconds;
			}
		}

		public double TestSelect()
		{
			using (var db = new AdoDatabaseContext(@"Server=DECAIREPC;Database=sampledata;Trusted_Connection=True;"))
			{
				// select records from the person joined by department table
				var startTime = DateTime.Now;
				for (int i = 0; i < 1000; i++)
				{
					using (var result = db.ReadDataSet("SELECT p.* FROM Department d JOIN Person p ON p.department = d.id"))
					{
						var query = result.Tables[0].Rows.Cast<DataRow>().ToList();
					}
				}
				var elapsedTime = DateTime.Now - startTime;

				return elapsedTime.TotalSeconds;
			}
		}

		public double TestUpdate()
		{
			using (var db = new AdoDatabaseContext(@"Server=DECAIREPC;Database=sampledata;Trusted_Connection=True;"))
			{
				using (var dbwrite = new AdoDatabaseContext(@"Server=DECAIREPC;Database=sampledata;Trusted_Connection=True;"))
				{
					// update all records in the person table
					var startTime = DateTime.Now;
					using (var result = db.ReadQuery("SELECT * FROM Person"))
					{
						while (result.Read())
						{
							dbwrite.Add("@last", result.GetFieldNullOrString("last") + "2", SqlDbType.VarChar);
							dbwrite.Add("@id",result.GetFieldNullOrString("id"),SqlDbType.VarChar);
							dbwrite.Execute("UPDATE Person SET last=@last WHERE id=@id");
						}
					}

					var elapsedTime = DateTime.Now - startTime;

					return elapsedTime.TotalSeconds;
				}
			}
		}

		public double TestDelete()
		{
			using (var db = new AdoDatabaseContext(@"Server=DECAIREPC;Database=sampledata;Trusted_Connection=True;"))
			{
				// delete all records in the person table
				var startTime = DateTime.Now;
				db.Execute("DELETE FROM Person");
				var elapsedTime = DateTime.Now - startTime;

				return elapsedTime.TotalSeconds;
			}
		}

		public void WriteLine(string text)
		{
			using (var writer = new StreamWriter(@"c:\ado_speed_tests.txt", true))
			{
				writer.WriteLine(text);
			}
		}
	}

}
