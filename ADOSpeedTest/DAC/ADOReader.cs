using System;
using System.Data.SqlClient;

namespace ADOSpeedTest.DAC
{
	public class AdoReader : IDisposable
	{
		private SqlDataReader _dataReader;

		public SqlDataReader Data => _dataReader;

		public bool HasRows => _dataReader.HasRows;

		public object this[string index] => _dataReader[index];

		public bool IsDBNull(int index)
		{
			return _dataReader.IsDBNull(index);
		}

		public int GetOrdinal(string indexName)
		{
			return _dataReader.GetOrdinal(indexName);
		}

		public string GetFieldNullOrString(string fieldName)
		{
			if (_dataReader.IsDBNull(_dataReader.GetOrdinal(fieldName)))
			{
				return null;
			}
			return _dataReader[fieldName].ToString();
		}

		public void Close()
		{
			_dataReader.Close();
			_dataReader.Dispose();
			_dataReader = null;
		}

		public AdoReader(SqlCommand sqlCommand)
		{
			_dataReader = sqlCommand.ExecuteReader();
		}

		public bool Read()
		{
			return _dataReader.Read();
		}

		public void Dispose()
		{
			Dispose(true);
		}

		~AdoReader()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_dataReader != null)
				{
					_dataReader.Close();
					_dataReader.Dispose();
					_dataReader = null;
				}
			}
		}
	}
}
