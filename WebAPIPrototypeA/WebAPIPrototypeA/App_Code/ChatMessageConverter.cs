using System;
using Models;
using Newtonsoft.Json;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace WebAPIPrototypeA
{
	public class ChatMessageConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return (objectType == typeof(ChatMessage));
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			JObject json = JObject.Load(reader);
			if (json["Channel"] != null)
				return json.ToObject<ChannelMessage>(serializer);
			if (json["To"] != null)
				return json.ToObject<DirectMessage>(serializer);

			return null;
		}

		public override bool CanWrite
		{
			get { return false; }
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}
