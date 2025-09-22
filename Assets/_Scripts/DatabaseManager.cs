using Firebase.Database;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class DatabaseManager : MonoBehaviour {
    [SerializeField] private TMP_InputField nameInput, ScoreInput;
    private string userId;
    [SerializeField] private DatabaseReference databaseReference;
    [SerializeField] private TMP_Text displayText;
    private void Start() {
        // Get the unique device identifier.
        this.userId = SystemInfo.deviceUniqueIdentifier;

        // Get the root reference location of the database.
        this.databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

        DisplayCurrentUserScore();

    }

    //public void CreateUserEntry() {
    //    UserScore newUser = new UserScore(id: this.userId, username: nameInput.text, score: int.Parse(ScoreInput.text));
    //    Debug.Log(newUser.ToString());

    //    string json = JsonUtility.ToJson(newUser);

    //    this.databaseReference.Child("HighScore").SetRawJsonValueAsync(json).ContinueWith(task => {
    //        if (task.IsFaulted || task.IsCanceled) {
    //            Debug.LogError("Failed to create user: " + task.Exception);
    //        } else {
    //            Debug.Log("User created successfully.");
    //        }
    //    });
    //}

    public void CreateSimpleUserEntry() {
        SimpleUserScore newUser = new SimpleUserScore(nameInput.text, int.Parse(ScoreInput.text));
        Debug.Log(newUser.ToString());

        string json = JsonUtility.ToJson(newUser);

        this.databaseReference.Child("HighScore").Child(this.userId).SetRawJsonValueAsync(json).ContinueWith(task => {
            if (task.IsFaulted || task.IsCanceled) {
                Debug.LogError("Failed to create user: " + task.Exception);
            } else {
                Debug.Log("User created successfully.");
            }
        });

        DisplayCurrentUserScore();
    }

    public IEnumerator GetCurrentUserScore(Action<SimpleUserScore> OnCallback) {

        var DBTask = this.databaseReference.Child("HighScore").Child(this.userId).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask != null) {
            DataSnapshot snapshot = DBTask.Result;
            OnCallback(new SimpleUserScore(
                snapshot.Child("username").Value.ToString(),
                int.Parse(snapshot.Child("score").Value.ToString())
            ));
        }

    }

    public IEnumerator GetEveryUserScore(Action<SimpleUserScore[]> OnCallback) {
        var DBTask = this.databaseReference.Child("HighScore").GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        if (DBTask != null) {
            DataSnapshot snapshot = DBTask.Result;
            SimpleUserScore[] allUsers = new SimpleUserScore[snapshot.ChildrenCount];
            int index = 0;
            foreach (var childSnapshot in snapshot.Children) {
                allUsers[index] = new SimpleUserScore(
                    childSnapshot.Child("username").Value.ToString(),
                    int.Parse(childSnapshot.Child("score").Value.ToString())
                );
                index++;
            }

            Array.Sort(allUsers, (x, y) => y.score.CompareTo(x.score));

            OnCallback(allUsers);
        }
    }

    public void DisplayCurrentUserScore() {
        //StartCoroutine(GetCurrentUserScore((userScore) => {
        //    displayText.text = userScore.ToString();
        //}));

        StartCoroutine(GetEveryUserScore((allUsers) => {
            displayText.text = string.Empty;
            foreach (var user in allUsers) {
                displayText.text += $"{user.username}: {user.score}" + "\n";
            }
        }));
    }
}
