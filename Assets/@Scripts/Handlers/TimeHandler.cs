using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TimeHandler : StaticInstance<TimeHandler>, IBind<TimeHandler.TimeData>
{
    [SerializeField] private TimeData timeData;
    [SerializeField] private TimeInfo currentTime;

    [field: SerializeField]public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();


    private void Update()
    {
        timeData.savedTime.Set(DateTime.Now);
    }

    public void Bind(TimeData data)
    {
        timeData = data;
        timeData.Id = data.Id;

        currentTime = timeData.savedTime;

        TriggerTime(DateTime.Now);
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            currentTime = timeData.savedTime;

            TriggerTime(DateTime.Now);
        }
    }
    private void TriggerTime(DateTime timeNow)
    {
        double seconds = (new TimeInfo(timeNow) - currentTime).TotalSeconds;

        if(seconds <= 0) return;

        ITimeListener[] timeListeners = FindObjectsOfType<MonoBehaviour>(true).OfType<ITimeListener>().ToArray();

        foreach (ITimeListener listener in timeListeners)
        {
            listener.CalculateTime(seconds);
        }
    }

    [System.Serializable]
    public class TimeData : ISaveable
    {
        [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();

        public TimeInfo savedTime;

        public void Reset_Ascended()
        {
            
        }

        public void Reset()
        {
            
        }
    }

    [System.Serializable]
    public struct TimeInfo
    {
        public int years;
        public int months;
        public int days;
        public int hours;
        public int minutes;
        public int seconds;
        public int milliseconds;

        public double TotalSeconds => new TimeSpan(years*months*days, hours, minutes, seconds, milliseconds).TotalSeconds;
        public TimeInfo(DateTime dateTime)
        {
            years = dateTime.Year;
            months = dateTime.Month;
            days = dateTime.Day;
            hours = dateTime.Hour;
            minutes = dateTime.Minute;
            seconds = dateTime.Second;
            milliseconds = dateTime.Millisecond;
        }
        public TimeInfo(int years, int months, int days, int hours, int minutes, int seconds, int milliseconds)
        {
            this.years = years;
            this.months = months;
            this.days = days;
            this.hours = hours;
            this.minutes = minutes;
            this.seconds = seconds;
            this.milliseconds = milliseconds;
        }

        public void Set(DateTime time)
        {
            years = time.Year;
            months = time.Month;
            days = time.Day;
            hours = time.Hour;
            minutes = time.Minute;
            seconds = time.Second;
            milliseconds = time.Millisecond;
        }

        public DateTime Convert()
        {
            return new DateTime(years, months, days, hours, minutes, seconds, milliseconds);
        }

        public static TimeInfo operator- (TimeInfo a, TimeInfo b)
        {
            return new TimeInfo(a.years - b.years, a.months - b.months, a.days - b.days, a.hours - b.hours, a.minutes - b.minutes, a.seconds - b.seconds, a.milliseconds - b.milliseconds);
        }
    
        public override string ToString()
        {
            return Convert().ToString();
        }
    }
}


public interface ITimeListener
{
    void CalculateTime(double seconds);
}
