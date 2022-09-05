using Scorpio.Commons;
public class Progress {
    private int total;
    private int phase;
    private int next;
    public Progress(int total) : this(total, 50) { }
    public Progress(int total, int phase) {
        this.total = total;
        this.phase = total / phase;
        this.next = this.phase;
    }
    public void SetProgress(int progress) {
        progress += 1;
        if (progress == total || progress >= this.next) {
            this.next += this.phase;
            var p = string.Format("{0:00.00}", progress * 100f / total);
            logger.info($"进度:{progress}/{total}({p}%)");
        }
    }
}
