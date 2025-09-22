
public class SimpleUserScore {
    public string username;
    public int score;

    public SimpleUserScore(string username, int score) {
        this.username = username;
        this.score = score;
    }

    public override string ToString() {
        return $"User(username: {username}, score: {score})";
    }
}