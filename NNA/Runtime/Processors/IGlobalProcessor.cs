
namespace nna.processors
{
	public interface IGlobalProcessor
	{
		string Type {get;}
		void Process(NNAContext Context);
	}
}
