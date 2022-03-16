public class MusicFactory {
    public const string Kuwo = "kuwo";
    public const string Kugou = "kugou";
    public const string Cloud = "cloud";
    public static MusicBase Create(string type) {
        switch (type.ToLower()) {
            case Kuwo: return new MusicKuwo();
            case Kugou: return new MusicKugou();
            case Cloud: return new MusicCloud();
            default: return new MusicKuwo();
        }
    }
}