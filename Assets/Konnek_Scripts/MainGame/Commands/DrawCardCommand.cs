using System;

public class DrawCardCommand : Command
{
    ulong clientId;
    ulong cardId;
    public DrawCardCommand(ulong clientId, ulong cardId)
    {
        this.clientId = clientId;
        this.cardId = cardId;
    }

    public override void Execute()
    {
        // uint cardInstanceId = KonnekManager.CardInstanceIdPointer;
        // DeckManager.Instance.SpawnCard_ClientRpc(cardId, clientId,cardInstanceId);
        // OnFinishDrawCard += HandleOnFinishDrawCard;
        base.Execute();

    }

    // private void HandleOnFinishDrawCard()
    // {
    //     OnFinishDrawCard -= HandleOnFinishDrawCard;
    // }
}
