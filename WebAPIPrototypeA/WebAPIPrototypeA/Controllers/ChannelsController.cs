using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace WebAPIPrototypeA.Controllers
{
	public class ChannelsController : ApiController
	{
		public List<Channel> Channels { get; set; }
		public ChannelsController()
		{
			this.Channels = new List<Channel>();
		}

		[HttpGet]
		public IHttpActionResult GetAllChannels()
		{
			return Ok(this.Channels);
		}

		[HttpGet]
		public IHttpActionResult GetChannel(string channelName)
		{
			return Ok(this.Channels.Where(x => x.ChannelName == channelName));
		}

		[HttpPost]
		public IHttpActionResult Save([FromBody]Channel channel)
		{
			if (this.IsValidChannel(channel))
			{
				this.Channels.Add(channel);
				return Ok();
			}
			else
				return new System.Web.Http.Results.StatusCodeResult(System.Net.HttpStatusCode.NotModified, this);
		}

		private bool IsValidChannel(Channel channel)
		{
			bool isValidNewChannel = false;
			if (channel.Subscribers != null)
			{
				isValidNewChannel = this.Channels.Count(x => x.ChannelName == channel.ChannelName) == 0 && channel.Subscribers.Count() > 0;
			}

			return isValidNewChannel;
		}
	}
}
