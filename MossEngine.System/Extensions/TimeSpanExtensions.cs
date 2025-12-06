namespace MossEngine.System.Extensions;

public static class TimeSpanExtensions
{
	extension( TimeSpan span )
	{
		public string ToRelativeTimeString()
		{
			if ( span.TotalMinutes < 30 ) return "just now";

			return span.TotalHours switch
			{
				< 6 => "recently",
				< 24 => "today",
				< 48 => "yesterday",
				_ => span.TotalDays switch
				{
					< 8 => "this week",
					< 15 => "last week",
					< 30 => "this month",
					< 60 => "last month",
					< 365 => "this year",
					< 365 * 2 => "last year",
					_ => "ages ago"
				}
			};
		}

		public string ToRemainingTimeString()
		{
			if ( span == TimeSpan.Zero )
				return "";

			if ( span == TimeSpan.MaxValue )
				return "Calculating...";

			if ( span.TotalDays >= 1 )
				return $"{span.Days} day{(span.Days > 1 ? "s" : "")} remaining";
			
			if ( span.TotalHours >= 1 )
				return $"{span.Hours} hour{(span.Hours > 1 ? "s" : "")} remaining";
			
			if ( span.TotalMinutes >= 1 )
				return $"{span.Minutes} minute{(span.Minutes > 1 ? "s" : "")} remaining";

			return $"{span.Seconds} second{(span.Seconds > 1 ? "s" : "")} remaining";
		}
	}
}
