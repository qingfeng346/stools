public class MusicFactory {
    public const string Kuwo = "kuwo";
    public const string Kugou = "kugou";
    public const string Cloud = "cloud";
    public static MusicBase Create(string type) => type switch {
        Kuwo => new MusicKuwo(),
        Kugou => new MusicKugou(),
        Cloud => new MusicCloud(),
        _ => new MusicKuwo()
    };
}