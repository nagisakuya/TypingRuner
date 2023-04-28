using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypingRunner
{
	class RandomBoxMuller
	{
		static Random rnd = new Random();
		private double X;
		private double Y;
		private int count = 0;
		public double Next(double variance = 1, double average = 0)
		{
			if (count++ % 2 == 0)
			{
				X = Math.Sqrt(-2.0 * Math.Log(rnd.NextDouble()));
				Y = 2.0 * Math.PI * rnd.NextDouble();
				return variance * X * Math.Sin(Y) + average;
			}
			else
			{
				return variance * X * Math.Cos(Y) + average;
			}
		}
		public double Next(double v, double a, double lower_limit)
		{
			for (int i = 0; i < 100; i++)
			{
				double temp = Next(v, a);
				if (temp >= lower_limit)
				{
					return temp;
				}
			}
			return lower_limit;
		}
	}
}
