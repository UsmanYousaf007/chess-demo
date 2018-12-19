/*
 private string FormatTimer(TimeSpan timer)
{
    // TODO: localize clock

    if (timer.TotalHours > 1)
    {
        int days = timer.Days;
        int hours = timer.Hours;
        int minutes = timer.Minutes;

        // Day calculation
        if (timer.Hours == 23 && (timer.Minutes > 0 || timer.Seconds > 0))
        {
            days++;
            hours = 0;
            minutes = 0;
        }
        // Hours calculation
        else if (timer.Minutes == 59 && timer.Seconds > 0)
        {
            hours++;
            minutes = 0;
        }
        else if (days > 0 && (timer.Minutes > 0 || timer.Seconds > 0))
        {
            hours++;
            minutes = 0;
        }
        // Minutes calculation
        else if (timer.Seconds > 0)
        {
            minutes++;
        }

        StringBuilder sb = new StringBuilder();
        if (days > 0)
        {
            sb.Append(days);
            sb.Append("d");
        }

        if (hours > 0)
        {
            if (days > 0)
            {
                sb.Append(" ");

            }
            sb.Append(hours);
            sb.Append("h");
        }

        if (minutes > 0 && (days == 0))
        {
            if (hours > 0)
            {
                sb.Append(" ");

            }
            sb.Append(minutes);
            sb.Append("m");
        }

        return sb.ToString();
    }

    // else show 00:00 format
    // This code is similar to rounding the seconds to a ceiling
    if (timer.TotalMilliseconds > 0)
    {
        timer = TimeSpan.FromMilliseconds(timer.TotalMilliseconds + 999);
    }

    return string.Format("{0:00}:{1:00}", Mathf.FloorToInt((float)timer.TotalMinutes), timer.Seconds);
}
*/