﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackApp
{
	public class User
	{
		public string Name { get; set; }

		public int Age { get; set; }

		public override string ToString()
		{
			return this.Name + ", " + this.Age + " years old";
		}
	}
}
