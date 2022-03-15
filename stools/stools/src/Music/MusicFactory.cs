public class MusicFactory {
    public static MusicBase Create(string type) {
        switch (type.ToLower()) {
            case "kuwo": return new MusicKuwo();
            case "kugou": return new MusicKugou();
            case "cloud": return new MusicCloud();
            default: return new MusicKuwo();
        }
    }
}