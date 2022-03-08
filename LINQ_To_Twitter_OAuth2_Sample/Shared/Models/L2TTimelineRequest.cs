namespace LINQ_To_Twitter_OAuth2_Sample.Shared.Models;
public class L2TTimelineRequest : L2TBase
{
	public string ForScreenName { get; set; }
	public long ForUserId { get; set; }
	public int MaxResults { get; set; }
	public long SinceId { get; set; }
	public string Filter { get; set; }
	public L2TTimelineRequest(string forScreenName = "",
							  long forUserId = 0,
							  int maxResults = 10,
							  long sinceId = 0,
							  string filter = "")
	{
		ForScreenName = forScreenName;
		ForUserId = forUserId;
		MaxResults = maxResults;
		SinceId = sinceId;
		Filter = filter;
	}
}
