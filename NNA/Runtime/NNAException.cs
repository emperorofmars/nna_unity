namespace nna
{
	public class NNAException : System.Exception
	{
		public NNAException(string Message, string ProcessorType, UnityEngine.Object Target = null, System.Exception InnerException = null)
			 : base(Message, InnerException)
		{
			this.ProcessorType = ProcessorType;
			this.Target = Target;
		}
		public readonly string ProcessorType;
		public readonly UnityEngine.Object Target;
	}
}
