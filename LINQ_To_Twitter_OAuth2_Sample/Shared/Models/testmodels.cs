using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQ_To_Twitter_OAuth2_Sample.Shared.Models;

public class testmodels
{

	public class TstRootobject
	{
		public TstDatum[]? data { get; set; }
		public TstIncludes? includes { get; set; }
		public TstError[]? errors { get; set; }
	}

	public class TstIncludes
	{
		public TstMedium[]? media { get; set; }
		public TstUser[]? users { get; set; }
		public TstTweet[]? tweets { get; set; }
	}

	public class TstMedium
	{
		public int height { get; set; }
		public string? url { get; set; }
		public string? type { get; set; }
		public string? media_key { get; set; }
		public int width { get; set; }
	}

	public class TstUser
	{
		public string? profile_image_url { get; set; }
		public string? username { get; set; }
		public string? id { get; set; }
		public string? name { get; set; }
	}

	public class TstTweet
	{
		public string? id { get; set; }
		public DateTime created_at { get; set; }
		public TstEntities? entities { get; set; }
		public string? text { get; set; }
		public string? author_id { get; set; }
		public string? conversation_id { get; set; }
		public string? source { get; set; }
		public string? lang { get; set; }
		public TstContext_Annotations[]? context_annotations { get; set; }
		public string? reply_settings { get; set; }
	}

	public class TstEntities
	{
		public TstAnnotation[]? annotations { get; set; }
		public TstUrl[]? urls { get; set; }
	}

	public class TstAnnotation
	{
		public int start { get; set; }
		public int end { get; set; }
		public float probability { get; set; }
		public string? type { get; set; }
		public string? normalized_text { get; set; }
	}

	public class TstUrl
	{
		public int start { get; set; }
		public int end { get; set; }
		public string? url { get; set; }
		public string? expanded_url { get; set; }
		public string? display_url { get; set; }
		public TstImage[]? images { get; set; }
		public int status { get; set; }
		public string? title { get; set; }
		public string? description { get; set; }
		public string? unwound_url { get; set; }
	}

	public class TstImage
	{
		public string? url { get; set; }
		public int width { get; set; }
		public int height { get; set; }
	}

	public class TstContext_Annotations
	{
		public TstDomain? domain { get; set; }
		public TstEntity? entity { get; set; }
	}

	public class TstDomain
	{
		public string? id { get; set; }
		public string? name { get; set; }
		public string? description { get; set; }
	}

	public class TstEntity
	{
		public string? id { get; set; }
		public string? name { get; set; }
	}

	public class TstDatum
	{
		public string? id { get; set; }
		public DateTime created_at { get; set; }
		public TstAttachments? attachments { get; set; }
		public string? text { get; set; }
		public string? author_id { get; set; }
		public string? conversation_id { get; set; }
		public TstEntities1? entities { get; set; }
		public string? source { get; set; }
		public string? lang { get; set; }
		public string? reply_settings { get; set; }
		public TstReferenced_Tweets[]? referenced_tweets { get; set; }
	}

	public class TstAttachments
	{
		public string[]? media_keys { get; set; }
	}

	public class TstEntities1
	{
		public TstUrl1[] urls { get; set; }
		public TstMention[]? mentions { get; set; }
	}

	public class TstUrl1
	{
		public int start { get; set; }
		public int end { get; set; }
		public string? url { get; set; }
		public string? expanded_url { get; set; }
		public string? display_url { get; set; }
	}

	public class TstMention
	{
		public int start { get; set; }
		public int end { get; set; }
		public string? username { get; set; }
		public string? id { get; set; }
	}

	public class TstReferenced_Tweets
	{
		public string? type { get; set; }
		public string? id { get; set; }
	}

	public class TstError
	{
		public string? value { get; set; }
		public string? detail { get; set; }
		public string? title { get; set; }
		public string? resource_type { get; set; }
		public string? parameter { get; set; }
		public string? resource_id { get; set; }
		public string? type { get; set; }
	}

}
