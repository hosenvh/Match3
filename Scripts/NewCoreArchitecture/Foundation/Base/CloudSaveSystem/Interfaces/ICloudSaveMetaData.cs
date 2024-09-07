using System;
using GooglePlayGames.BasicApi.SavedGame;


// TODO: later we can use this to send meta data to Save/Load method caller 
public interface ICloudSaveMetaData
{
    bool IsOpen { get; }
    string Filename { get; }
    string Description { get; }
    string CoverImageURL { get; }
    TimeSpan TotalTimePlayed { get; }
    DateTime LastModifiedTimestamp { get; }
}
