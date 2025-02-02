using UnityEngine;

namespace nna
{
	public enum NNAErrorSeverity
	{
		INFO, WARNING, ERROR, FATAL_ERROR
	}

	[System.Serializable]
	public class NNAReport
	{
		public NNAReport(string Message, NNAErrorSeverity Severity = NNAErrorSeverity.ERROR, string ProcessorType = null, UnityEngine.Object Node = null, System.Exception Exception = null)
		{
			this.Message = Message;
			this.Severity = Severity;
			this.ProcessorType = ProcessorType;
			this.Node = Node;
			this.Exception = Exception;
		}

		public string Message;
		public NNAErrorSeverity Severity;
		public string ProcessorType;
		public Object Node;
		public System.Exception Exception;
	}

	public class NNAException : System.Exception
	{
		public NNAException(NNAReport Report)
			 : base(Report.Message, Report.Exception)
		{
			this.Report = Report;
		}
		public NNAException(string Message, NNAErrorSeverity Severity = NNAErrorSeverity.ERROR, string ProcessorType = null, UnityEngine.Object Node = null, System.Exception Exception = null)
			 : base(Message, Exception)
		{
			this.Report = new NNAReport(Message, Severity, ProcessorType, Node, Exception);
		}

		public readonly NNAReport Report;
	}
}
