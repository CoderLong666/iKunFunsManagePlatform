using System;
using System.Text;
using NodaTime;
using NodaTime.Text;
using CultureInfo = System.Globalization.CultureInfo;

namespace JBBS.Common.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// 获取生日
        /// </summary>
        /// <param name="birthdate"></param>
        /// <returns></returns>
        public static int GetAgeByBirthdate(this DateTime birthdate)
        {
            var now = DateTime.Now;
            var age = now.Year - birthdate.Year;
            if (now.Month < birthdate.Month || (now.Month == birthdate.Month && now.Day < birthdate.Day))
            {
                age--;
            }

            return age < 0 ? 0 : age;
        }

        private static readonly DateTimeZone TimeZone = DateTimeZoneProviders.Tzdb["Asia/Shanghai"];

        /// <summary>
        /// DateTime转换为JS时间戳
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long ToEpochTimestamp(this DateTime dateTime)
        {
            return Instant.FromDateTimeUtc(dateTime.ToUniversalTime()).ToUnixTimeMilliseconds();
        }

        public static Instant ToInstant(this DateTime dateTime)
        {
            return Instant.FromDateTimeUtc(dateTime.ToUniversalTime());
        }

        /// <summary>
        /// Instant转换为JS时间戳
        /// </summary>
        /// <param name="instant"></param>
        /// <returns></returns>
        public static long ToEpochTimestamp(this Instant instant)
        {
            return instant.ToUnixTimeMilliseconds();
        }

        public static DateTime ToUniversalTime(this Instant instant)
        {
            return instant.ToDateTimeUtc();
        }

        public static LocalDate ToLocalDate(this Instant instant)
        {
            return instant.InZone(TimeZone).Date;
        }

        public static LocalTime ToLocalTime(this Instant instant)
        {
            return instant.InZone(TimeZone).TimeOfDay;
        }

        public static LocalDateTime ToLocalDateTime(this Instant instant)
        {
            var date = instant.ToLocalDate();
            var time = instant.ToLocalTime();
            return new LocalDateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second, time.Millisecond);
        }

        /// <summary>
        /// LocalDate转换为JS时间戳
        /// </summary>
        /// <param name="localDate"></param>
        /// <returns></returns>
        public static long ToEpochTimestamp(this LocalDate localDate)
        {
            return localDate.ToInstant().ToEpochTimestamp();
        }

        public static DateTime ToUniversalTime(this LocalDate localDate)
        {
            return localDate.ToInstant().ToUniversalTime();
        }

        public static Instant ToInstant(this LocalDate localDate)
        {
            return localDate.AtStartOfDayInZone(TimeZone).ToInstant();
        }

        public static Instant ToInstant(this LocalDateTime localDateTime)
        {
            return localDateTime.Date.ToInstant().PlusTicks(localDateTime.TimeOfDay.TickOfDay);
        }

        public static DateTime ToDateTime(this LocalDate localDate)
        {
            return new DateTime(localDate.Year, localDate.Month, localDate.Day);
        }

        /// <summary>
        /// JS时间戳转换为UTC DateTime
        /// </summary>
        /// <param name="epochTimestamp"></param>
        /// <returns></returns>
        public static DateTime ToUniversalTime(this long epochTimestamp)
        {
            return Instant.FromUnixTimeMilliseconds(epochTimestamp).ToDateTimeUtc();
        }

        /// <summary>
        /// JS时间戳转换为Instant
        /// </summary>
        /// <param name="epochTimestamp"></param>
        /// <returns></returns>
        public static Instant ToInstant(this long epochTimestamp)
        {
            return Instant.FromUnixTimeMilliseconds(epochTimestamp);
        }

        public static LocalDate ToLocalDate(this long epochTimestamp)
        {
            return epochTimestamp.ToInstant().ToLocalDate();
        }

        public static LocalTime ToLocalTime(this long epochTimestamp)
        {
            return epochTimestamp.ToInstant().ToLocalTime();
        }

        public static string ToString(this LocalDate value, string patternText)
        {
            return LocalDatePattern.CreateWithInvariantCulture(patternText).Format(value);
        }

        public static string ToString(this LocalTime value, string patternText)
        {
            return LocalTimePattern.CreateWithInvariantCulture(patternText).Format(value);
        }
        public static string ToString(this LocalDateTime value, string patternText)
        {
            return LocalDateTimePattern.CreateWithInvariantCulture(patternText).Format(value);
        }

        public static string ToString(this Instant value, string patternText)
        {
            var zonedTime = value.InZone(TimeZone);
            return LocalDateTimePattern.CreateWithInvariantCulture(patternText).Format(zonedTime.LocalDateTime);
        }

        public static Instant? ToInstant(this string value, string patternText = "yyyy-MM-dd HH:mm:ss")
        {
            var result = LocalDateTimePattern.CreateWithInvariantCulture(patternText).Parse(value);
            if (!result.Success) return null;
            var zonedTime = result.Value.InZoneStrictly(TimeZone);
            return zonedTime.ToInstant();
        }

        public static LocalDate? ToLocalDate(this string value, string patternText = "yyyy-MM-dd")
        {
            var result = LocalDatePattern.CreateWithInvariantCulture(patternText).Parse(value);
            return result.Success ? result.Value : null;
        }

        public static LocalTime? ToLocalTime(this string value, string patternText = "HH:mm:ss")
        {
            var result = LocalTimePattern.CreateWithInvariantCulture(patternText).Parse(value);
            return result.Success ? result.Value : null;
        }

        private static readonly CultureInfo CultureInfo = CultureInfo.GetCultureInfo("zh");

        public static string FormatString(this LocalDateTime dt, string format)
        {
            var builder = new StringBuilder();
            foreach (var chr in format)
            {
                switch (chr)
                {
                    case '年':
                        builder.Append($"{dt.Year}年");
                        break;
                    case '季':
                        builder.Append($"{(int)Math.Ceiling(dt.Month / 3.0)}季");
                        break;
                    case '月':
                        builder.Append($"{dt.Month}月");
                        break;
                    case '周':
                        builder.Append($"{CultureInfo.Calendar.GetWeekOfYear(dt.ToDateTimeUnspecified().ToLocalTime(), CultureInfo.DateTimeFormat.CalendarWeekRule, CultureInfo.DateTimeFormat.FirstDayOfWeek)}周");
                        break;
                    case '日':
                        builder.Append($"{dt.Day}日");
                        break;
                    case '时':
                        builder.Append($"{dt.Hour}时");
                        break;
                    default:
                        builder.Append(chr);
                        break;
                }
            }

            return builder.ToString();
        }

        public static int DaysBetween(LocalDate d1, LocalDate d2)
        {
            var day1 = d1.ToUniversalTime();
            var day2 = d2.ToUniversalTime();
            if (day1 < day2)
                return (int)(day2 - day1).TotalDays + 1;
            return (int)(day1 - day2).TotalDays + 1;
        }

        public static Instant PlusDays(this Instant instant, double days)
        {
            return instant.Plus(Duration.FromHours(24 * days));
        }

        public static Instant PlusHours(this Instant instant, double hours)
        {
            return instant.Plus(Duration.FromHours(hours));
        }

        public static Instant PlusMinutes(this Instant instant, double minutes)
        {
            return instant.Plus(Duration.FromMinutes(minutes));
        }

        public static Instant PlusSeconds(this Instant instant, double seconds)
        {
            return instant.Plus(Duration.FromSeconds(seconds));
        }

        /// <summary>
        /// 当前整点时间
        /// </summary>
        /// <param name="instant"></param>
        /// <returns></returns>
        public static Instant FloorHour(this Instant instant)
        {
            return Instant.FromUnixTimeTicks(instant.ToUnixTimeTicks() / 36000000000 * 36000000000);
        }

        public static Instant FloorDay(this Instant instant)
        {
            return instant.ToLocalDate().ToInstant();
        }
    }
}
