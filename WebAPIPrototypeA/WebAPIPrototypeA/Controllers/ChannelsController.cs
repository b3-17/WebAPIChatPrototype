using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.SessionState;
using Repository;
using Models;

namespace WebAPIPrototypeA.Controllers
{
	public class ChannelsController : ApiController
	{
		private IRepository<Channel> channelRepo { get; set; }

		public ChannelsController(IRepository<Channel> passedChannelData)
		{
			this.channelRepo = passedChannelData;
		}

		public ChannelsController()
			:this(new ChannelRepository(new SessionStateContext()))
		{

		}

		[HttpGet]
		public IHttpActionResult GetAllChannels()
		{
			return Ok(this.channelRepo.All());
		}

		[HttpGet]
		public IHttpActionResult GetChannel(string channelName)
		{
			return Ok(this.channelRepo.All().Where(x => x.ChannelName == channelName));
		}

		[HttpPost]
		public IHttpActionResult Save([FromBody]Channel channel)
		{
			if (this.IsValidChannelToCreate(channel))
			{
				this.channelRepo.Save(channel);
				return Ok();
			}
			else
				return new StatusCodeResult(System.Net.HttpStatusCode.NotModified, this);
		}

		private bool IsValidChannelToCreate(Channel channel)
		{
			bool isValidNewChannel = false;
			if (channel.Subscribers != null)
				isValidNewChannel = this.channelRepo.All().Count(x => x.ChannelName == channel.ChannelName) == 0 && channel.Subscribers.Count() > 0;

			return isValidNewChannel;
		}

		[HttpPost]
		public IHttpActionResult Subscribe([FromBody]Channel subscriber)
		{
			if (this.IsValidChannelToCreate(subscriber))
			{
				this.channelRepo.Save(subscriber);
				return Ok();
			}

			Channel channelToSubscribeTo = this.channelRepo.All().FirstOrDefault(x => x.ChannelName == subscriber.ChannelName);
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
			var channels = channelRepo.All();
			if (channels.Count(x => x.ChannelName == subscriber.ChannelName) > 0)
			{
				Channel channelToUnsubscribeFrom = channels.FirstOrDefault(x => x.ChannelName == subscriber.ChannelName);
				channelToUnsubscribeFrom.Subscribers = channelToUnsubscribeFrom.Subscribers
					.Where(x => subscriber.Subscribers.Any(y => y.UserName != x.UserName)).ToList();
			}

			return Ok();
		}
	}
}
