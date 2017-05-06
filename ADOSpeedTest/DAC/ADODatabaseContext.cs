using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ADOSpeedTest.DAC
{
	public class AdoDatabaseContext : IDisposable
	{
		private SqlConnection _db;
		private readonly List<AdoParameter> _parameters = new List<AdoParameter>();

		public AdoDatabaseContext(string connectionString)
		{
			_db = new SqlConnection(connectionString);
			_db.Open();
		}

		public void Dispose()
		{
			Dispose(true);
		}

		~AdoDatabaseContext()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_db != null)
				{
					_db.Close();
					_db.Dispose();
					_db = null;
				}
			}
		}

		public AdoReader ReadQuery(string queryString)
		{
			var myCommand = new SqlCommand(queryString, _db);

			foreach (var param in _parameters)
			{
				myCommand.Parameters.Add(param.Name, param.Type);
				myCommand.Parameters[param.Name].Value = param.Value;
			}

			_parameters.Clear();

			return new AdoReader(myCommand);
		}

		public DataSet ReadDataSet(string queryString)
		{
			var myCommand = new SqlCommand(queryString, _db);
			var datasetAdapter = new SqlDataAdapter(myCommand);

			var ds = new DataSet();

			foreach (var param in _parameters)
			{
				myCommand.Parameters.Add(param.Name, param.Type);
				myCommand.Parameters[param.Name].Value = param.Value;
			}

			_parameters.Clear();

			datasetAdapter.Fill(ds);

			return ds;
		}

		public void Execute(string queryString)
		{
			var myCommand = new SqlCommand(queryString, _db);

			foreach (var param in _parameters)
			{
				myCommand.Parameters.Add(param.Name, param.Type);
				myCommand.Parameters[param.Name].Value = param.Value;
			}

			_parameters.Clear();

			myCommand.ExecuteNonQuery();
		}

		public void Add(string name, object value, SqlDbType type)
		{
			_parameters.Add(new AdoParameter
			{
				Name = name,
				Value = value,
				Type = type
			});
		}
	}
}
