using System;

namespace Misc
{
	public struct BigNumber
	{
		private int _maxValue;
		private int _val;
		private int _power;

		public BigNumber(int value, int exponent)
		{
			_val = value;
			_power = exponent;
			_maxValue = 1 << 15;
			Normalize();
		}

		#region Arithmetic Operations

		public static BigNumber operator +(BigNumber a, BigNumber b)
		{
			AlignPowers(ref a, ref b);
			return new BigNumber(a._val + b._val, a._power);
		}

		public static BigNumber operator -(BigNumber a, BigNumber b)
		{
			AlignPowers(ref a, ref b);
			return new BigNumber(a._val - b._val, a._power);
		}

		public static BigNumber operator *(BigNumber a, BigNumber b)
		{
			return new BigNumber(a._val * b._val, a._power + b._power);
		}

		public static BigNumber operator /(BigNumber a, BigNumber b)
		{
			return new BigNumber(a._val / b._val, a._power - b._power);
		}

		#endregion

		#region Comparison Operators

		public static bool operator ==(BigNumber a, BigNumber b)
		{
			AlignPowers(ref a, ref b);
			return a._val == b._val;
		}

		public static bool operator !=(BigNumber a, BigNumber b)
		{
			return !(a == b);
		}

		#endregion

		#region Conversion Operators

		public static implicit operator BigNumber(int val)
		{
			return new BigNumber(val, 0);
		}
		public static implicit operator BigNumber(float val)
		{
			var precision = GetDecimalPrecision(val);
			var intVal = (int)(val * precision);

			return new BigNumber(intVal, -precision);
		}
		public static implicit operator int(BigNumber val)
		{
			return val._val << val._power;
		}
		public static implicit operator float(BigNumber val)
		{
			return (float)val._val / (1 << val._power);
		}

		#endregion

		# region Helpers

		private static void AlignPowers(ref BigNumber a, ref BigNumber b)
		{
			if(a._power > b._power)
			{
				int shift = a._power - b._power;
				b._val <<= shift;
				b._power += shift;
			}
			else if(b._power > a._power)
			{
				int shift = b._power - a._power;
				a._val <<= shift;
				a._power += shift;
			}
		}
		private static int GetDecimalPrecision(float val)
		{
			float absoluteValue = Math.Abs(val);
			float decimalPart = absoluteValue - (int)absoluteValue;
			int precision = 0;
			while (decimalPart > 0 && precision < 15)
			{
				decimalPart *= 2;
				decimalPart -= (int)decimalPart;
				precision++;
			}
			return precision;
		}

		// Helper method to normalize the BigNumber to prevent overflow
		private void Normalize()
		{
			while (_val > _maxValue)
			{
				_val >>= 1; // Right shift the mantissa
				_power++; // Increase the exponent
			}
		}

		#endregion

		#region Object Overrides

		public override string ToString()
		{
			return $"{_val} * 2^{_power}";
		}

		public override bool Equals(object obj)
		{
			if(obj is BigNumber other)
			{
				AlignPowers(ref this, ref other);
				return this._val == other._val;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return _val.GetHashCode() ^ _power.GetHashCode();
		}

		#endregion

	}
}