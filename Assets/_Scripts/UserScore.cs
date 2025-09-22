public class UserScore {
    public string id;
    public string username;
    public int score;

    public UserScore(string id, string username, int score) {
        this.id = id;
        this.username = username;
        this.score = score;
    }
    public override string ToString() {
        return $"User(id: {id}, username: {username}, score: {score})";
    }
}
