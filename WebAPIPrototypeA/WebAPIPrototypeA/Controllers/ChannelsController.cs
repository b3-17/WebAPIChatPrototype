using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;

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
			if (this.IsValidChannelToCreate(channel))
			{
				this.Channels.Add(channel);
				return Ok();
			}
			else
				return new StatusCodeResult(System.Net.HttpStatusCode.NotModified, this);
		}

		private bool IsValidChannelToCreate(Channel channel)
		{
			bool isValidNewChannel = false;
			if (channel.Subscribers != null)
				isValidNewChannel = this.Channels.Count(x => x.ChannelName == channel.ChannelName) == 0 && channel.Subscribers.Count() > 0;

			return isValidNewChannel;
		}

		[HttpPost]
		public IHttpActionResult Subscribe([FromBody]Channel subscriber)
		{
			if (this.IsValidChannelToCreate(subscriber))
			{
				this.Channels.Add(subscriber);
				return Ok();
			}

			Channel channelToSubscribeTo = this.Channels.FirstOrDefault(x => x.ChannelName == subscriber.ChannelName);
			if (channelToSubscribeTo.Subscribers == null)
				channelToSubscribeTo.Subscribers = new List<ChatUser>();

			if (channelToSubscribeTo.Subscribers.Join(subscriber.Subscribers, x => x.UserName, y => y.UserName, (x, y) => new { x.UserName }).Count() == 0)
			{
				channelToSubscribeTo.Subscribers.AddRange(subscriber.Subscribers);
				return Ok();
			}
			else
				return Conflict();
		}

		[HttpPost]
		public IHttpActionResult Unsubscribe([FromBody]Channel subscriber)
		{
			if (this.Channels.Count(x => x.ChannelName == subscriber.ChannelName) > 0)
			{
				Channel channelToUnsubscribeFrom = this.Channels.FirstOrDefault(x => x.ChannelName == subscriber.ChannelName);
				channelToUnsubscribeFrom.Subscribers = channelToUnsubscribeFrom.Subscribers
					.Where(x => subscriber.Subscribers.Any(y => y.UserName != x.UserName)).ToList();
			}

			return Ok();
		}
	}
}
