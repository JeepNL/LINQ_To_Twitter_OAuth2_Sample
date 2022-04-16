namespace LINQ_To_Twitter_OAuth2_Sample.Shared.Models;
public class L2TTimelineRequest : L2TBase
{
	public string ForUserName { get; set; } = "";
	public string ForUserId { get; set; } = "";
	public int MaxResults { get; set; } = 10;
	public long SinceId { get; set; } = 0;
	public string Filter { get; set; } = "";
	//public L2TTimelineRequest(string forUserName = "",
	//						  string forUserId = "0",
	//						  int maxResults = 10,
	//						  long sinceId = 0,
	//						  string filter = "")
	//{
	//	ForUserName = forUserName;
	//	ForUserId = forUserId;
	//	MaxResults = maxResults;
	//	SinceId = sinceId;
	//	Filter = filter;
	//}
}
