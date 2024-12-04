/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

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
