using UnityEngine;

public class PlayCardCommand : Command
{
    private Card _card;
    public PlayCardCommand(Card card)
    {
        card.OnFinishPlayCard += base.Execute;

        _card = card;
    }

    public override void Execute()
    {
        if (_card != null)
        {
            _card.PlayCard();
        }
        else
        {
            Debug.LogWarning("No card in card holder");
        }

    }

}
