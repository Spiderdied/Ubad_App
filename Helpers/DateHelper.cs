namespace Ubad.Helpers
{
    public static class DateHelper
    {
        public static string TimeAgo(DateTime dt)
        {
            var span = DateTime.UtcNow - dt.ToUniversalTime();

            if (span.TotalSeconds < 60)   return "just now";
            if (span.TotalMinutes < 60)   return $"{(int)span.TotalMinutes}m ago";
            if (span.TotalHours   < 24)   return $"{(int)span.TotalHours}h ago";
            if (span.TotalDays    < 7)    return $"{(int)span.TotalDays}d ago";
            if (span.TotalDays    < 30)   return $"{(int)(span.TotalDays / 7)}w ago";
            if (span.TotalDays    < 365)  return $"{(int)(span.TotalDays / 30)}mo ago";
            return $"{(int)(span.TotalDays / 365)}y ago";
        }

        public static string FormatDate(DateTime dt) =>
            dt.ToString("MMM d, yyyy");
    }
}