using Dan.Main;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class FrogLeaderBoard : MonoBehaviour
{

    [SerializeField] private TMP_Text[] _entryTextObjects;
    [SerializeField] private TMP_InputField _usernameInputField;
    [SerializeField] private GameManager _gameManager;

    void Start()
    {
        
    }

   

    private void LoadEntries()
    {
        // Q: How do I reference my own leaderboard?
        // A: Leaderboards.<NameOfTheLeaderboard>

        Leaderboards.FrogzenLeaderboard.GetEntries(entries =>
        {
            foreach (var t in _entryTextObjects)
                t.text = "";
            var length = Mathf.Min(_entryTextObjects.Length, entries.Length);
            for (int i = 0; i < length; i++)
                _entryTextObjects[i].text = $"{entries[i].Rank}. {entries[i].Username} - {entries[i].Score}";
        });
    }

    public void UploadEntry()
    {
        Leaderboards.FrogzenLeaderboard.UploadNewEntry(_usernameInputField.text, (int)_gameManager.GetGameTime(), isSuccessful =>
        {
            if (isSuccessful)
                LoadEntries();
        });
    }
}
