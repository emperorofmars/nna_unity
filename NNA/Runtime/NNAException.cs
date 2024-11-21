
namespace nna
{
	public class NNAException : System.Exception
	{
		public NNAException(string NNAError, System.Exception UnderlyingException = null)
		{
			this.NNAError = NNAError;
			this.UnderlyingException = UnderlyingException;
		}

		public string NNAError;
		public System.Exception UnderlyingException = null;
	}
}
