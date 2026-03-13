public class TurnEventRecord
{
    public int FromTurnDecision;
    public bool IsActiv;
    public Event Event;

    public TurnEventRecord(int fromTurnDecision, Event ev, bool isActiv)
    {
        FromTurnDecision = fromTurnDecision;
        Event = ev;
        IsActiv = isActiv;
    }
}