using System.Collections.Generic;
using Models;
using System.Linq;

namespace Repository
{
	public class ChannelRepository : IRepository<Channel>
	{
		private IContext context { get; set; }

		public ChannelRepository(IContext passedContext)
		{
			this.context = passedContext;
		}

		public void Save(Channel itemToSave)
		{
			List<Channel> channels = this.context.Channels.ToList();
			channels.Add(itemToSave);
			this.context.Channels = channels;
		}

		public IEnumerable<Channel> All()
		{
			return this.context.Channels;
		}
	}
}
