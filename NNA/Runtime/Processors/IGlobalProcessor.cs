
namespace nna.processors
{
	/// <summary>
	/// Global Processors are always executed in the context they got registered for.
	/// These can optionally look for a Json component and use its data. This is useful if some functionality can be determined automatically, and will only sometimes require to be specified further.
	/// 
	/// If there is an optional Json component, its type can be added to the ignored types in a context to prevent a warning message when its Json processor isn't found.
	/// </summary>
	public interface IGlobalProcessor
	{
		string Type {get;}
		void Process(NNAContext Context);
	}
}
