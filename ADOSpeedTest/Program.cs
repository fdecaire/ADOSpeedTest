﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOSpeedTest
{
	class Program
	{
		static void Main(string[] args)
		{
			var test = new AdoPerfromanceTests();
			test.InitializeData();
			test.RunAllTests();
		}
	}
}
