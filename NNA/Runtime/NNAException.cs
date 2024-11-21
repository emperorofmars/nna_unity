
namespace nna
{
	public class NNAException : System.Exception
	{
		public NNAException(string Message, UnityEngine.Object Target, System.Exception InnerException = null)
			 : base(Message, InnerException)
		{
			this.Target = Target;
		}

		public readonly UnityEngine.Object Target;
	}
}
