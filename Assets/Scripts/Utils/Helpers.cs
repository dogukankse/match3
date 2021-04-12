namespace Utils
{
	public class Helpers
	{
		public static long Factorial(int val) 
		{ 
			int result = 1; 
			while (val > 0) 
			{ 
				result = result * val; 
				val--; 
			} 
			return result; 
		} 

	}
}