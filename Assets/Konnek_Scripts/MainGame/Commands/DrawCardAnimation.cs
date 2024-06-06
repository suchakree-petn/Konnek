using System;

public class DrawCardAnimation : Command
{
    public Action OnFinishDrawCard;
    ulong clientId;
    ulong cardId;
    public DrawCardAnimation(ulong clientId, ulong cardId)
    {
        this.clientId = clientId;
        this.cardId = cardId;
    }

    public override void Execute()
    {
        uint cardInstanceId = KonnekManager.CardInstanceIdPointer;
        DeckManager.Instance.SpawnCard(cardId, clientId, cardInstanceId, this);
        OnFinishDrawCard += HandleOnFinishDrawCard;

    }

    private void HandleOnFinishDrawCard()
    {
        // OnFinishDrawCard -= HandleOnFinishDrawCard;
        base.Execute();
    }
}
