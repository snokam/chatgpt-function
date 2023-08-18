using System.Collections.Generic;

namespace openai;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class Authorization
{
	public object enterprise_id { get; set; }
	public string team_id { get; set; }
	public string user_id { get; set; }
	public bool is_bot { get; set; }
	public bool is_enterprise_install { get; set; }
}

public class Event
{
	public string type { get; set; }
	public string text { get; set; }
	public List<File> files { get; set; }
	public bool upload { get; set; }
	public string user { get; set; }
	public bool display_as_bot { get; set; }
	public string bot_id { get; set; }
	public string ts { get; set; }
	public string channel { get; set; }
	public string subtype { get; set; }
	public string event_ts { get; set; }
	public string channel_type { get; set; }
}

public class File
{
	public string id { get; set; }
	public int created { get; set; }
	public int timestamp { get; set; }
	public string name { get; set; }
	public string title { get; set; }
	public string mimetype { get; set; }
	public string filetype { get; set; }
	public string pretty_type { get; set; }
	public string user { get; set; }
	public string user_team { get; set; }
	public bool editable { get; set; }
	public int size { get; set; }
	public string mode { get; set; }
	public bool is_external { get; set; }
	public string external_type { get; set; }
	public bool is_public { get; set; }
	public bool public_url_shared { get; set; }
	public bool display_as_bot { get; set; }
	public string username { get; set; }
	public string url_private { get; set; }
	public string url_private_download { get; set; }
	public string permalink { get; set; }
	public string permalink_public { get; set; }
	public string subject { get; set; }
	public List<To> to { get; set; }
	public List<From> from { get; set; }
	public List<object> cc { get; set; }
	public List<object> attachments { get; set; }
	public int original_attachment_count { get; set; }
	public string plain_text { get; set; }
	public string preview { get; set; }
	public string preview_plain_text { get; set; }
	public Headers headers { get; set; }
	public bool has_more { get; set; }
	public bool sent_to_self { get; set; }
	public string bot_id { get; set; }
	public bool has_rich_preview { get; set; }
	public string file_access { get; set; }
}

public class From
{
	public string address { get; set; }
	public string name { get; set; }
	public string original { get; set; }
}

public class Headers
{
	public string date { get; set; }
	public string in_reply_to { get; set; }
	public object reply_to { get; set; }
	public string message_id { get; set; }
}

public class SlackEvent
{
	public string token { get; set; }
	public string team_id { get; set; }
	public string context_team_id { get; set; }
	public object context_enterprise_id { get; set; }
	public string api_app_id { get; set; }
	public Event @event { get; set; }
	public string type { get; set; }
	public string event_id { get; set; }
	public int event_time { get; set; }
	public List<Authorization> authorizations { get; set; }
	public bool is_ext_shared_channel { get; set; }
	public string event_context { get; set; }
}

public class To
{
	public string address { get; set; }
	public string name { get; set; }
	public string original { get; set; }
}

