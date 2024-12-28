using System.Collections.Generic;
using UnityEngine;

public class RaceResult
{
    public string SessionRunTime { get; set; }
    public string SessionType { get; set; }
    public bool WasGreen { get; set; }
    public string Simulator { get; set; }
    public string SessionLengthType { get; set; }
    public int TotalNumberOfLaps { get; set; }
    public string SessionRunDuration { get; set; }
    public Track TrackInfo { get; set; }
    public List<Driver> Drivers { get; set; }
    public string FuelConsumptionInformation { get; set; }
}

public class Track
{
    public string TrackName { get; set; }
    public string TrackLayoutName { get; set; }
    public LayoutLength LayoutLength { get; set; }
}

public class LayoutLength
{
    public float InMeters { get; set; }
}

public class Driver
{

    public string ClassName { get; set; }
    public string ClassId { get; set; }
    public string DriverId { get; set; }
    public string DriverLongName { get; set; }
    public int FinishingPosition { get; set; }
    public int FinishingPositionInClass { get; set; }
    public float AveragePosition { get; set; }
    public int InitialPosition { get; set; }
    public int InitialPositionInClass { get; set; }
    public string CarName { get; set; }
    public int TotalLaps { get; set; }
    public float TotalDistance { get; set; }
    public string GapToPlayerRelative { get; set; }
    public int LapsDifferenceToPlayer { get; set; }
    public TopSpeed TopSpeed { get; set; }
    public Lap[] Laps { get; set; }
    public bool IsPlayer { get; set; }
    public bool Finished { get; set; }
    public string FinishStatus { get; set; }
    public float Rating { get; set; }
    public float RatingRelativeToPlayer { get; set; }
    public string GapToPlayerByTiming { get; set; }
}

public class TopSpeed
{
    public float InMs { get; set; }
}

public class Lap
{
    public bool IsPitLap { get; set; }
    public int LapNumber { get; set; }
    public bool IsValid { get; set; }
    public int LapStartPosition { get; set; }
    public int LapStartPositionClass { get; set; }
    public string LapTime { get; set; }
    public string Sector1 { get; set; }
    public string Sector2 { get; set; }
    public string Sector3 { get; set; }
}