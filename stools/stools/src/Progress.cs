using Scorpio.Commons;
public class Progress {
    private int total;
    private int phase;
    private int next;
    private string prefix;
    public Progress(int total, string prefix = "", int phase = 20) {
        this.total = total;
        this.prefix = prefix ?? "";
        this.phase = total / phase;
        this.next = this.phase;
    }
    public void SetProgress(int progress) {
        progress += 1;
        if (progress == total || progress >= next) {
            next += phase;
            var p = string.Format("{0:00.00}", progress * 100f / total);
            logger.info($"{prefix}进度:{progress}/{total}({p}%)");
        }
    }
}
