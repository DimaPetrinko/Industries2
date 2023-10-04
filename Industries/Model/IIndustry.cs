using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Resources;

namespace Industries.Model
{
	public interface IIndustry
	{
		Task Produce(CancellationToken token);
		Task LoadInput(IEnumerable<ResourcePackage> resources, CancellationToken token);
		Task UnloadOutput(IEnumerable<ResourcePackage> requestedResources, CancellationToken token);
		bool CanProduce();
		bool CanLoadInput(IEnumerable<ResourcePackage> resources);
		bool CanLoadAnyInput();
		bool CanUnloadOutput(IEnumerable<ResourcePackage> requestedResources);
		bool CanUnloadAnyOutput();
	}
}