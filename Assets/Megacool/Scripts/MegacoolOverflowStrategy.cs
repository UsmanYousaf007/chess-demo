public sealed class MegacoolOverflowStrategy {
    private readonly string name;

    public static readonly MegacoolOverflowStrategy LATEST = new MegacoolOverflowStrategy("latest");
    public static readonly MegacoolOverflowStrategy HIGHLIGHT = new MegacoolOverflowStrategy("highlight");
    public static readonly MegacoolOverflowStrategy TIMELAPSE = new MegacoolOverflowStrategy("timelapse");

    private MegacoolOverflowStrategy(string name) {
        this.name = name;
    }

    public override string ToString() {
        return name;
    }
}
