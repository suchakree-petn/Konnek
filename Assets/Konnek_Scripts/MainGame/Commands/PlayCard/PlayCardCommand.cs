using UnityEngine;

public class PlayCardCommand : Command
{
    private Card _card;
    private ulong clientId;

    public PlayCardCommand(Card card, ulong clientId)
    {
        card.OnFinishPlayCard += base.Execute;

        _card = card;
        this.clientId = clientId;
    }

    public override void Execute()
    {
        if (_card != null)
        {
            _card.PlayCard(clientId);
        }
        else
        {
            Debug.LogWarning("No card in card holder");
        }

    }

}
