namespace WizCloud;

public class WizProgress {
    public int Retrieved { get; }
    public int? Total { get; }

    public WizProgress(int retrieved, int? total) {
        Retrieved = retrieved;
        Total = total;
    }
}