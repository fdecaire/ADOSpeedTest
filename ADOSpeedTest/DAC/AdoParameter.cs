using System.Data;

namespace ADOSpeedTest.DAC
{
	public class AdoParameter
	{
		public string Name { get; set; }
		public SqlDbType Type { get; set; }
		public object Value { get; set; }
	}
}
